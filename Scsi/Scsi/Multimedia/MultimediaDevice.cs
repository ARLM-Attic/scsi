using ModeSenseCommand = Scsi.ModeSense10Command;
using ModeSelectCommand = Scsi.ModeSelect10Command;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Helper;
namespace Scsi.Multimedia
{
	/// <summary>Represents a multimedia device (e.g. CD, DVD, BD, etc.). The <see cref="IMultimediaDevice"/> interface is implemented explicitly because it is not meant to be accessed directly (which would cause confusion when similarly named methods would perform differently).
	/// </summary>
	public class MultimediaDevice : ScsiDevice, IMultimediaDevice
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly int POLL_INTERVAL_MILLIS = 250;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int lastMediaEventQueryTick = -1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private GetEventStatusNotificationCommand gesn = new GetEventStatusNotificationCommand();

		/// <summary>Initializes a new instance of the <see cref="MultimediaDevice"/> class.</summary>
		/// <param name="interface">The pass-through interface to use.</param>
		/// <param name="leaveOpen"><c>true</c> to leave the interface open, or <c>false</c> to dispose along with this object.</param>
		public MultimediaDevice(IScsiPassThrough @interface, bool leaveOpen) : base(@interface, leaveOpen) { }

		public void Blank(BlankCommand command) { this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private CapabilitiesMechanicalStatusPage Capabilities { get { return this.GetCapabilities(new ModeSenseCommand(PageControl.CurrentValues)); } }

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private CDParametersPage CDParameters { get { return this.GetCDParameters(new ModeSenseCommand(PageControl.CurrentValues)); } }

		public static ushort GetBlockSize(DataBlockType blockType)
		{
			ushort result;
			switch (blockType)
			{
				case DataBlockType.Raw2352:
					result = 2352;
					break;
				case DataBlockType.Raw2352WithPQSubchannel16:
					result = 2368;
					break;
				case DataBlockType.Raw2352WithPWSubchannel96:
					result = 2448;
					break;
				case DataBlockType.Raw2352WithRawPWSubchannel96:
					result = 2448;
					break;
				case DataBlockType.Mode1:
					result = 2048;
					break;
				case DataBlockType.Mode2:
					result = 2336;
					break;
				case DataBlockType.Mode2XAForm1:
					result = 2048;
					break;
				case DataBlockType.Mode2XAForm1WithSubHeader:
					result = 2056;
					break;
				case DataBlockType.Mode2XAForm2:
					result = 2324;
					break;
				case DataBlockType.Mode2XAMixed:
					result = 2332;
					break;
				default:
					throw new ArgumentOutOfRangeException("blockType", blockType, "Invalid block type.");
			}
			return result;
		}

		public void CloseTrackOrSession(CloseSessionTrackCommand command) { this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }

		public MultimediaProfile CurrentProfile
		{
			get
			{
				unsafe
				{
					int bufferSize = Marshaler.DefaultSizeOf<FeatureHeader>();
					byte* pFeature = stackalloc byte[bufferSize];
					BufferWithSize buffer = new BufferWithSize(pFeature, bufferSize);
					var cmd = new GetConfigurationCommand() { RequestType = FeatureRequestType.OneFeatureHeaderAndZeroOrOneDescriptor, StartingFeatureNumber = 0 };
					cmd.AllocationLength = (ushort)buffer.Length;
					this.ExecuteCommand(cmd, DataTransferDirection.ReceiveData, buffer);
					return ((FeatureHeader*)buffer.Address)->CurrentProfile;
				}
			}
		}

		/// <summary>Tries to detect the disc read speeds.</summary>
		/// <returns>A list of read speeds detected, in BYTES per second.</returns>
		public ushort[] DetectReadSpeeds()
		{
			var senseCmd = new ModeSenseCommand(PageControl.CurrentValues);
			var prevCap = this.GetCapabilities(senseCmd);
			try
			{
				var result = new List<ushort>();
				var stack = new Stack<KeyValuePair<ushort, ushort>>();
				result.Add(prevCap.MaxReadSpeed);
				this.SetCDSpeed(new SetCDSpeedCommand(1, prevCap.CurrentWriteSpeed, prevCap.RotationControlSelected));
				result.Add(this.GetCapabilities(senseCmd).CurrentReadSpeed);
				stack.Push(new KeyValuePair<ushort, ushort>(1, prevCap.MaxReadSpeed));
				for (int counter = 0; stack.Count > 0 && (stack.Peek().Key + stack.Peek().Value) / 2 > 1; counter++)
				{
					KeyValuePair<ushort, ushort> range = stack.Pop();
					this.SetCDSpeed(new SetCDSpeedCommand(range.Key, prevCap.CurrentWriteSpeed, prevCap.RotationControlSelected));
					ushort start = this.GetCapabilities(senseCmd).CurrentReadSpeed;
					this.SetCDSpeed(new SetCDSpeedCommand(range.Value, prevCap.CurrentWriteSpeed, prevCap.RotationControlSelected));
					ushort end = this.GetCapabilities(senseCmd).CurrentReadSpeed;
					this.SetCDSpeed(new SetCDSpeedCommand((ushort)((range.Key + range.Value) / 2), prevCap.CurrentWriteSpeed, prevCap.RotationControlSelected));
					ushort middle = this.GetCapabilities(senseCmd).CurrentReadSpeed;

					KeyValuePair<ushort, ushort> startToMiddle = new KeyValuePair<ushort, ushort>(start, (ushort)(middle - 1));
					KeyValuePair<ushort, ushort> middleToEnd = new KeyValuePair<ushort, ushort>((ushort)(middle + 1), end);

					if (middle == start & middle == end)
					{
						result.Add(middle);
					}
					else if (middle != start & middle == end)
					{
						if (middle != end)
						{
							stack.Push(startToMiddle);
						}
					}
					else if (middle == start & middle != end)
					{
						if (middle != start)
						{
							stack.Push(middleToEnd);
						}
					}
					else
					{
						result.Add(middle);
						stack.Push(startToMiddle);
						stack.Push(middleToEnd);
					}
				}
				result.Sort();
				result.Reverse();
				return result.ToArray();
			}
			finally
			{
				this.SetCDSpeed(new SetCDSpeedCommand(prevCap.CurrentReadSpeed, prevCap.CurrentWriteSpeed, prevCap.RotationControlSelected));
			}
		}

		/// <summary>Tries to detect the disc write speeds.</summary>
		/// <returns>A list of write speeds detected, in BYTES per second.</returns>
		public ushort[] DetectWriteSpeeds()
		{
			var senseCmd = new ModeSenseCommand(PageControl.CurrentValues);
			var prevCap = this.GetCapabilities(senseCmd);
			try
			{
				var result = new List<ushort>();
				var stack = new Stack<KeyValuePair<ushort, ushort>>();
				result.Add(prevCap.MaxWriteSpeed);
				this.SetCDSpeed(new SetCDSpeedCommand(prevCap.CurrentReadSpeed, 1, prevCap.RotationControlSelected));
				result.Add(this.GetCapabilities(senseCmd).CurrentWriteSpeed);
				stack.Push(new KeyValuePair<ushort, ushort>(1, prevCap.MaxWriteSpeed));
				for (int counter = 0; stack.Count > 0 && (stack.Peek().Key + stack.Peek().Value) / 2 > 1; counter++)
				{
					KeyValuePair<ushort, ushort> range = stack.Pop();
					this.SetCDSpeed(new SetCDSpeedCommand(prevCap.CurrentReadSpeed, range.Key, prevCap.RotationControlSelected));
					ushort start = this.GetCapabilities(senseCmd).CurrentWriteSpeed;
					this.SetCDSpeed(new SetCDSpeedCommand(prevCap.CurrentReadSpeed, range.Value, prevCap.RotationControlSelected));
					ushort end = this.GetCapabilities(senseCmd).CurrentWriteSpeed;
					this.SetCDSpeed(new SetCDSpeedCommand(prevCap.CurrentReadSpeed, (ushort)((range.Key + range.Value) / 2), prevCap.RotationControlSelected));
					ushort middle = this.GetCapabilities(senseCmd).CurrentWriteSpeed;

					KeyValuePair<ushort, ushort> startToMiddle = new KeyValuePair<ushort, ushort>(start, (ushort)(middle - 1));
					KeyValuePair<ushort, ushort> middleToEnd = new KeyValuePair<ushort, ushort>((ushort)(middle + 1), end);

					if (middle == start & middle == end)
					{
						result.Add(middle);
					}
					else if (middle != start & middle == end)
					{
						if (middle != end)
						{
							stack.Push(startToMiddle);
						}
					}
					else if (middle == start & middle != end)
					{
						if (middle != start)
						{
							stack.Push(middleToEnd);
						}
					}
					else
					{
						result.Add(middle);
						stack.Push(startToMiddle);
						stack.Push(middleToEnd);
					}
				}
				result.Sort();
				result.Reverse();
				return result.ToArray();
			}
			finally
			{
				this.SetCDSpeed(new SetCDSpeedCommand(prevCap.CurrentReadSpeed, prevCap.CurrentWriteSpeed, prevCap.RotationControlSelected));
			}
		}

		public DeviceBusyEvent? DeviceBusy
		{
			get
			{
				this.gesn.NotificationClassRequest = NotificationClassFlags.DeviceBusy;
				var e = this.GetEventStatusNotification(this.gesn);
				return e.Header.NoEventAvailable ? (DeviceBusyEvent?)null : e.Events.DeviceBusyEvent;
			}
		}

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private DiscInformationBlock DiscInformation { get { return this.ReadDiscInformation(); } }

		public GeneralDiscType DiscType
		{
			get
			{
				switch (this.CurrentProfile)
				{
					case MultimediaProfile.NoProfile:
						return GeneralDiscType.None;
					case MultimediaProfile.CDROM:
					case MultimediaProfile.CDR:
					case MultimediaProfile.CDRW:
						return GeneralDiscType.CompactDisc;
					case MultimediaProfile.DvdRom:
					case MultimediaProfile.DvdMinusRSequentialRecording:
					case MultimediaProfile.DvdRam:
					case MultimediaProfile.DvdMinusRWRestrictedOverwrite:
					case MultimediaProfile.DvdMinusRWSequentialRecording:
					case MultimediaProfile.DvdMinusRDualLayerSequentialRecording:
					case MultimediaProfile.DvdMinusRDualLayerJumpRecording:
					case MultimediaProfile.DvdMinusRWDualLayer:
					case MultimediaProfile.DvdDownloadDiscRecording:
					case MultimediaProfile.DvdPlusRW:
					case MultimediaProfile.DvdPlusR:
					case MultimediaProfile.DvdPlusRWDualLayer:
					case MultimediaProfile.DvdPlusRDualLayer:
						return GeneralDiscType.DigitalVersatileDisc;
					case MultimediaProfile.BDROM:
					case MultimediaProfile.BDRSequentialRecording:
					case MultimediaProfile.BDRERandomRecording:
					case MultimediaProfile.BDRE:
						return GeneralDiscType.BluRayDisc;
					case MultimediaProfile.HDDvdRom:
					case MultimediaProfile.HDDvdR:
					case MultimediaProfile.HDDvdRam:
					case MultimediaProfile.HDDvdRW:
					case MultimediaProfile.HDDvdRDualLayer:
					case MultimediaProfile.HDDvdRWDualLayer:
						return GeneralDiscType.HighDensityDigitalVersatileDisc;
					default:
						return GeneralDiscType.Unknown;
				}
			}
		}

		public void Eject() { this.StartStopUnit(new StartStopUnitCommand(true, false)); }

		public void Erase10(Erase10Command command) { this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }

		public ExternalRequestEvent? ExternalRequest
		{
			get
			{
				this.gesn.NotificationClassRequest = NotificationClassFlags.ExternalRequest;
				var e = this.GetEventStatusNotification(this.gesn);
				return e.Header.NoEventAvailable ? (ExternalRequestEvent?)null : e.Events.ExternalRequestEvent;
			}
		}

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private FormatCapacityList FormatCapacities { get { return this.ReadFormatCapacities(); } }

		public void FormatUnit(FormatUnitCommand command, FormatDescriptor formatInfo)
		{
			GCHandle hFormatInfo = GCHandle.Alloc(formatInfo, GCHandleType.Pinned);
			try
			{
				command.FormatCode = formatInfo.FormatCode;
				this.ExecuteCommand(command, DataTransferDirection.SendData, new BufferWithSize(hFormatInfo.AddrOfPinnedObject(), Marshaler.SizeOf(formatInfo)));
			}
			finally
			{
				hFormatInfo.Free();
			}
		}

		public CapabilitiesMechanicalStatusPage GetCapabilities(ModeSenseCommand command) { return this.ModeSense<CapabilitiesMechanicalStatusPage>(command); }

		public CDParametersPage GetCDParameters(ModeSenseCommand command) { return this.ModeSense<CDParametersPage>(command); }

		public FeatureCollection GetConfiguration(FeatureCode startingFeatureNumber, FeatureRequestType requestType)
		{
			if (!Enum.IsDefined(typeof(FeatureRequestType), requestType)) { throw new ArgumentOutOfRangeException("requestType", requestType, "Invalid request type."); }
			if (requestType == FeatureRequestType.OneFeatureHeaderAndZeroOrOneDescriptor)
			{ throw new ArgumentOutOfRangeException("requestType", requestType, "Request type must return a collection."); }
			unsafe
			{
				int bufferSize = Marshaler.DefaultSizeOf<FeatureHeader>();
				byte* pFeature1 = stackalloc byte[bufferSize];
				var buffer = new BufferWithSize(pFeature1, bufferSize);
				var cmd = new GetConfigurationCommand()
				{
					RequestType = requestType,
					StartingFeatureNumber = startingFeatureNumber
				};
				cmd.AllocationLength = (ushort)buffer.Length;
				this.ExecuteCommand(cmd, DataTransferDirection.ReceiveData, buffer);
				int requiredSize = Marshaler.DefaultSizeOf<MultimediaFeature>() + (int)((FeatureHeader*)buffer.Address)->DataLength;
				if (bufferSize < requiredSize)
				{
					bufferSize = requiredSize;
					byte* pFeature2 = stackalloc byte[bufferSize];
					buffer = new BufferWithSize(pFeature2, bufferSize);
					cmd.AllocationLength = (ushort)buffer.Length;
					this.ExecuteCommand(cmd, DataTransferDirection.ReceiveData, buffer);
				}
				return FeatureCollection.FromBuffer(buffer);
			}
		}

		public TFeature GetConfiguration<TFeature>()
			where TFeature : MultimediaFeature, new()
		{
			MultimediaFeature result = Objects.CreateInstance<TFeature>();
			this.GetConfiguration(result.FeatureCode, ref result);
			return (TFeature)result;
		}

		public MultimediaFeature GetConfiguration(FeatureCode featureCode) { MultimediaFeature feature = null; this.GetConfiguration(featureCode, ref feature); return feature; }

		private void GetConfiguration(FeatureCode featureCode, ref MultimediaFeature feature)
		{
			if (feature == null) { feature = MultimediaFeature.CreateInstance(featureCode); }
			unsafe
			{
				int bufferSize = Marshaler.DefaultSizeOf<MultimediaFeature>();
				byte* pFeature1 = stackalloc byte[bufferSize];
				BufferWithSize buffer = new BufferWithSize(pFeature1, bufferSize);
				var cmd = new GetConfigurationCommand() { RequestType = FeatureRequestType.OneFeatureHeaderAndZeroOrOneDescriptor, StartingFeatureNumber = feature.FeatureCode };
				cmd.AllocationLength = (ushort)buffer.Length;
				this.ExecuteCommand(cmd, DataTransferDirection.ReceiveData, buffer);
				int requiredSize = Marshaler.DefaultSizeOf<FeatureHeader>() + Marshaler.DefaultSizeOf<MultimediaFeature>() + MultimediaFeature.ReadAdditionalLength(buffer);
				if (bufferSize < requiredSize)
				{
					bufferSize = requiredSize;
					byte* pFeature2 = stackalloc byte[bufferSize];
					buffer = new BufferWithSize(pFeature2, bufferSize);
					cmd.AllocationLength = (ushort)buffer.Length;
					this.ExecuteCommand(cmd, DataTransferDirection.ReceiveData, buffer);
				}
				var newBuf = buffer.ExtractSegment(Marshaler.DefaultSizeOf<FeatureHeader>());
				if (newBuf.LengthU32 > 0 && MultimediaFeature.ReadFeatureCode(newBuf) == feature.FeatureCode)
				{
					Marshaler.PtrToStructure(newBuf, ref feature);
				}
				else
				{
					feature = null;
				}
			}
		}

		public Event GetEventStatusNotification(GetEventStatusNotificationCommand command)
		{
			var result = new Event();
			command.AllocationLength = (ushort)Marshaler.DefaultSizeOf<Event>();
			unsafe { this.ExecuteCommand(command, DataTransferDirection.ReceiveData, new BufferWithSize((IntPtr)(&result), command.AllocationLength)); }
			return result;
		}

		public PerformanceData GetPerformance(GetPerformanceCommand command)
		{
			unsafe
			{
				int size = Marshaler.DefaultSizeOf<PerformanceData>();
				byte* pBuffer = stackalloc byte[size];
				ushort prevMaxNumDesc = command.MaximumNumberOfDescriptors;
				command.MaximumNumberOfDescriptors = 0;
				var buffer = new BufferWithSize(pBuffer, size);
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
				int neededLength = PerformanceData.ReadPerformanceDataLength(buffer);
				size += neededLength * 2;
				{ byte* pBuffer2 = stackalloc byte[size]; pBuffer = pBuffer2; }
				buffer = new BufferWithSize(pBuffer, size);
				command.MaximumNumberOfDescriptors = prevMaxNumDesc;
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
				var data = new PerformanceData(command.PerformanceType, command.DataType);
				Marshaler.PtrToStructure(buffer, ref data);
				return data;
			}
		}

		public WriteParametersPage GetWriteParameters(ModeSenseCommand command) { return this.ModeSense<WriteParametersPage>(command); }

		protected override bool HasMediumChanged()
		{
			int tick = Environment.TickCount;
			if (tick - this.lastMediaEventQueryTick >= POLL_INTERVAL_MILLIS)
			{
				bool changed = this.MediaEvent.GetValueOrDefault(new MediaEvent() { MediaEventCode = MediaEventCode.NewMedia }).MediaEventCode != MediaEventCode.NoChange;
				this.lastMediaEventQueryTick = tick;
				return changed;
			}
			else { return false; }
		}

		public void Load() { this.StartStopUnit(new StartStopUnitCommand(true, true)); }

		public MediaEvent? MediaEvent
		{
			get
			{
				this.gesn.NotificationClassRequest = NotificationClassFlags.Media;
				var e = this.GetEventStatusNotification(this.gesn);
				return e.Header.NoEventAvailable ? (MediaEvent?)null : e.Events.MediaEvent;
			}
		}

		public MultipleHostEvent? MultipleHost
		{
			get
			{
				this.gesn.NotificationClassRequest = NotificationClassFlags.MultiHost;
				var e = this.GetEventStatusNotification(this.gesn);
				return e.Header.NoEventAvailable ? (MultipleHostEvent?)null : e.Events.MultipleHostEvent;
			}
		}

		public OperationalChangeEvent? OperationalChange
		{
			get
			{
				this.gesn.NotificationClassRequest = NotificationClassFlags.OperationalChange;
				var e = this.GetEventStatusNotification(this.gesn);
				return e.Header.NoEventAvailable ? (OperationalChangeEvent?)null : e.Events.OperationalChangeEvent;
			}
		}

		public PowerManagementEvent? PowerManagement
		{
			get
			{
				this.gesn.NotificationClassRequest = NotificationClassFlags.PowerManagement;
				var e = this.GetEventStatusNotification(this.gesn);
				return e.Header.NoEventAvailable ? (PowerManagementEvent?)null : e.Events.PowerManagementEvent;
			}
		}

		public void PreventAllowMediumRemoval(PreventAllowMediumRemovalCommand command)
		{ this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }

		public TrackInformationBlock[] ReadAllTracksInformation()
		{
			var diskInfo = this.ReadDiscInformation();
			var tracks = new TrackInformationBlock[diskInfo.LastTrackNumberInLastSession - diskInfo.FirstTrackNumber + 1];
			for (uint i = 0; i < tracks.Length; i++)
			{ tracks[i] = this.ReadTrackInformation(new ReadTrackInformationCommand(false, TrackIdentificationType.LogicalTrackNumber, diskInfo.FirstTrackNumber + i)); }
			return tracks;
		}

		public BufferCapacityStructureInBytes ReadBufferCapacityInBytes() { return this.ReadBufferCapacity(new ReadBufferCapacityCommand(false)).Bytes; }

		public BufferCapacityStructureInBlocks ReadBufferCapacityInBlocks() { return this.ReadBufferCapacity(new ReadBufferCapacityCommand(true)).Blocks; }

		public ReadBufferCapacityInfo ReadBufferCapacity(ReadBufferCapacityCommand command)
		{
			ReadBufferCapacityInfo result = new ReadBufferCapacityInfo();
			command.AllocationLength = (ushort)Marshaler.DefaultSizeOf<ReadBufferCapacityInfo>();
			unsafe { this.ExecuteCommand(command, DataTransferDirection.ReceiveData, new BufferWithSize((IntPtr)(&result), command.AllocationLength)); }
			return result;
		}

		public void ReadCD(ReadCDCommand command, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.ReadCD(command, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		public void ReadCD(ReadCDCommand command, BufferWithSize buffer) { this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer); }

		public DiscInformationBlock ReadDiscInformation() { return this.ReadDiscInformation(new ReadDiscInformationCommand()); }

		public DiscInformationBlock ReadDiscInformation(ReadDiscInformationCommand command)
		{
			DiscInformationBlock result;
			unsafe
			{
				int bufferSize = Marshaler.DefaultSizeOf<DiscInformationBlock>();
				byte* pBuffer1 = stackalloc byte[bufferSize];
				BufferWithSize buffer = new BufferWithSize(pBuffer1, bufferSize);
				command.AllocationLength = (ushort)bufferSize;
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
				byte entries = DiscInformationBlock.ReadNumOptimumPowerCalibrationEntries(buffer);
				int requiredSize = Marshaler.DefaultSizeOf<DiscInformationBlock>() + Marshaler.DefaultSizeOf<OptimumPowerCalibration>() * entries;
				if (bufferSize < requiredSize)
				{
					bufferSize = requiredSize;
					byte* pBuffer2 = stackalloc byte[bufferSize];
					buffer = new BufferWithSize(pBuffer2, bufferSize);
					command.AllocationLength = (ushort)bufferSize;
					this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
				}
				result = Marshaler.PtrToStructure<DiscInformationBlock>(buffer);
			}
			return result;
		}

		public DiscStructureData ReadDiscStructure(ReadDiscStructureCommand command)
		{
			command.AllocationLength = (ushort)Marshaler.DefaultSizeOf<DiscStructureData>();
			BufferWithSize buffer;
			unsafe { byte* pHeader = stackalloc byte[command.AllocationLength]; buffer = new BufferWithSize(pHeader, command.AllocationLength); }
			this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer); //Get the size of the required data

			//This is ADDED to the previous header size!
			command.AllocationLength = (ushort)(sizeof(ushort) /*this is for the size field itself*/ + DiscStructureData.ReadDataLength(buffer) * 2); //Multiply by 2, just to be really sure it won't change due to race conditions
			unsafe { byte* pHeader = stackalloc byte[command.AllocationLength]; buffer = new BufferWithSize(pHeader, command.AllocationLength); }
			this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer); //Read the actual data
			Debug.Assert(command.AllocationLength >= DiscStructureData.ReadDataLength(buffer));
			return DiscStructureData.FromBuffer(command.Format, command.MediaType, buffer);
		}

		public FormatCapacityList ReadFormatCapacities() { return this.ReadFormatCapacities(new ReadFormatCapacitiesCommand()); }

		public FormatCapacityList ReadFormatCapacities(ReadFormatCapacitiesCommand cmd)
		{
			FormatCapacityList cl;
			unsafe
			{
				int bufferSize = Marshaler.DefaultSizeOf<FormatCapacityList>();
				byte* pBuffer1 = stackalloc byte[bufferSize];
				var buffer = new BufferWithSize(pBuffer1, bufferSize);
				cmd.AllocationLength = (ushort)buffer.Length;
				this.ExecuteCommand(cmd, DataTransferDirection.ReceiveData, buffer);
				int requiredSize = FormatCapacityList.ReadCapacityListLength(buffer.Address) + bufferSize;
				if (bufferSize < requiredSize)
				{
					bufferSize = requiredSize;
					var pBuffer2 = stackalloc byte[bufferSize];
					buffer = new BufferWithSize(pBuffer2, bufferSize);
					cmd.AllocationLength = (ushort)buffer.Length;
					this.ExecuteCommand(cmd, DataTransferDirection.ReceiveData, buffer);
				}
				cl = Marshaler.PtrToStructure<FormatCapacityList>(buffer);
			}
			return cl;
		}

		public TrackInformationBlock ReadTrackInformation(ReadTrackInformationCommand command)
		{
			var result = new TrackInformationBlock();
			command.AllocationLength = (ushort)Marshaler.DefaultSizeOf<TrackInformationBlock>();
			unsafe { this.ExecuteCommand(command, DataTransferDirection.ReceiveData, new BufferWithSize((IntPtr)(&result), command.AllocationLength)); }
			return result;
		}

		public TocPmaAtipResponseData ReadTocPmaAtip(ReadTocPmaAtipCommand command)
		{
			TocPmaAtipResponseData result;
			unsafe
			{
				command.AllocationLength = sizeof(ushort) + 1022;
				byte* pData = stackalloc byte[command.AllocationLength];
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, new BufferWithSize(pData, command.AllocationLength));
				var requiredLength = TocPmaAtipResponseData.ReadDataLength((IntPtr)pData) + sizeof(ushort);
				if (requiredLength > command.AllocationLength)
				{
					command.AllocationLength = (ushort)requiredLength;
					byte* pData2 = stackalloc byte[command.AllocationLength];
					pData = pData2;
					this.ExecuteCommand(command, DataTransferDirection.ReceiveData, new BufferWithSize(pData, command.AllocationLength));
					requiredLength = TocPmaAtipResponseData.ReadDataLength((IntPtr)pData) + sizeof(ushort);
				}
				result = TocPmaAtipResponseData.CreateInstance(command.Format);
				if (result != null) { Marshaler.PtrToStructure(new BufferWithSize(pData, command.AllocationLength), ref result); }
			}
			return result;
		}

		public void ReserveTrack(ReserveTrackCommand command) { this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }

		public void SendCueSheet(SendCueSheetCommand command, params CueLine[] cueSheet)
		{
			unsafe
			{
				fixed (CueLine* pCueSheet = cueSheet)
				{
					command.CueSheetSize = (uint)sizeof(CueLine) * (uint)cueSheet.Length;
					this.ExecuteCommand(command, DataTransferDirection.SendData, new BufferWithSize((IntPtr)pCueSheet, command.CueSheetSize));
				}
			}
		}

		public void SendOptimumPowerCalibrationInformation(SendOpcInformationCommand command, OptimumPowerCalibration? info)
		{
			command.DoOptimumPowerCalibration = info == null;
			OptimumPowerCalibration value = info.GetValueOrDefault();
			unsafe { this.ExecuteCommand(command, DataTransferDirection.SendData, command.DoOptimumPowerCalibration ? BufferWithSize.Zero : new BufferWithSize((IntPtr)(&value), Marshaler.DefaultSizeOf<OptimumPowerCalibration>())); }
		}

		public void SetCDParameters(ModeSelectCommand command, CDParametersPage modePage) { this.ModeSelect(command, modePage); }

		public void SetCDSpeed(SetCDSpeedCommand command) { this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }

		public void SetReadAhead(SetReadAheadCommand command) { this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }

		public void SetStreaming(DefectiveBlockInformationCacheZoneDescriptors info) { this.SetStreaming(new SetStreamingCommand(), info); }

		public void SetStreaming(SetStreamingCommand command, DefectiveBlockInformationCacheZoneDescriptors info)
		{
			unsafe
			{
				int bufferSize = Marshaler.SizeOf(info);
				byte* pBuffer = stackalloc byte[bufferSize];
				BufferWithSize buffer = new BufferWithSize(pBuffer, bufferSize);
				Marshaler.StructureToPtr(info, buffer);
				command.ParameterListLength = (ushort)buffer.Length;
				command.Type = StreamingDataType.DefectiveBlockInformationCacheZoneDescriptor;
				this.ExecuteCommand(command, DataTransferDirection.SendData, buffer);
			}
		}

		public void SetStreaming(StreamingPerformanceDescriptor info) { this.SetStreaming(new SetStreamingCommand(), info); }

		public void SetStreaming(SetStreamingCommand command, StreamingPerformanceDescriptor info)
		{
			unsafe
			{
				BufferWithSize buffer = new BufferWithSize((IntPtr)(&info), Marshaler.DefaultSizeOf<StreamingPerformanceDescriptor>());
				command.ParameterListLength = (ushort)buffer.Length;
				command.Type = StreamingDataType.PerformanceDescriptor;
				this.ExecuteCommand(command, DataTransferDirection.SendData, buffer);
			}
		}

		public void SetWriteParameters(ModeSelectCommand command, WriteParametersPage modePage) { command.ParameterListLength = 60; this.ModeSelect(command, modePage); }

		public void StartStopUnit(StartStopUnitCommand command)
		{
			try
			{
				this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero);
			}
			catch (ScsiException ex)
			{
				var sense = ex.SenseData;
				if (sense.SenseKey == SenseKey.IllegalRequest & sense.AdditionalSenseCode == AdditionalSenseCode.InvalidFieldInCommandDescriptorBlock)
				{ throw new InvalidOperationException("An error occurred. This operation may not be supported.", ex); }
				throw;
			}
		}

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private TrackInformationBlock[] Tracks { get { return this.ReadAllTracksInformation(); } }

		public void Verify10(Verify10Command command) { this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }


		public bool WaitForBusyStatus(DeviceBusyStatusCode statusToWaitFor) { return this.WaitForBusyStatus(statusToWaitFor, -1); }

		/// <param name="timeoutMillis">The timeout in waiting for the event. If this member is negative, then timeouts will never occur.</param>
		public bool WaitForBusyStatus(DeviceBusyStatusCode statusToWaitFor, int timeoutMillis)
		{
			int startTick = System.Environment.TickCount;
			for (; ; )
			{
				var e = this.DeviceBusy;
				if (e == null) { return false; }
				var val = e.Value;
				if (val.DeviceBusyStatusCode == statusToWaitFor) { return true; }
				var tick = System.Environment.TickCount;
				if (timeoutMillis != -1 && tick - startTick > timeoutMillis) { return false; }
				var msToWait = val.Time;
				Thread.Sleep(msToWait > 0 ? msToWait * 100 : this.DefaultPollingInterval * 100);
			}
		}

		public bool WaitForMediaEvent(MediaEventCode eventToWaitFor) { return this.WaitForMediaEvent(eventToWaitFor, -1); }

		/// <param name="timeoutMillis">The timeout in waiting for the event. If this member is negative, then timeouts will never occur.</param>
		public bool WaitForMediaEvent(MediaEventCode eventToWaitFor, int timeoutMillis)
		{
			int startTick = System.Environment.TickCount;
			for (; ; )
			{
				var e = this.MediaEvent;
				if (e == null) { return false; }
				var val = e.Value;
				if (val.MediaEventCode == eventToWaitFor) { return true; }
				var tick = System.Environment.TickCount;
				if (timeoutMillis != -1 && tick - startTick > timeoutMillis) { return false; }
				Thread.Sleep(this.DefaultPollingInterval * 100);
			}
		}

		public bool WaitForPowerStatus(PowerStatusCode statusToWaitFor) { return this.WaitForPowerStatus(statusToWaitFor, -1); }

		/// <param name="timeoutMillis">The timeout in waiting for the event. If this member is negative, then timeouts will never occur.</param>
		public bool WaitForPowerStatus(PowerStatusCode statusToWaitFor, int timeoutMillis)
		{
			int startTick = System.Environment.TickCount;
			for (; ; )
			{
				var e = this.PowerManagement;
				if (e == null) { return false; }
				var val = e.Value;
				if (val.PowerStatusCode == statusToWaitFor) { return true; }
				var tick = System.Environment.TickCount;
				if (timeoutMillis != -1 && tick - startTick > timeoutMillis) { return false; }
				Thread.Sleep(this.DefaultPollingInterval * 100);
			}
		}

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private WriteParametersPage WriteParameters { get { return this.GetWriteParameters(new ModeSenseCommand(PageControl.CurrentValues)); } }

		#region IMultimediaDevice
		public virtual System.IO.Stream OpenTrack(int trackNumber)
		{
			//closeNeeded = false;
			var track = this.ReadTrackInformation(new ReadTrackInformationCommand(false, TrackIdentificationType.LogicalTrackNumber, (uint)trackNumber));

			long startPosition = track.LogicalTrackStartAddress * (long)this.BlockSize;
			long maxLength = track.LogicalTrackSize * (long)this.BlockSize;
			long position = track.NextWritableAddress.GetValueOrDefault() * (long)this.BlockSize;

			var stream = new TrackStream(this, track.LogicalTrackNumber, startPosition, maxLength, maxLength, false);
			if (stream.Position != position) { stream.Position = position - startPosition; }
			return stream;
		}

		public virtual int FirstTrackNumber { get { return this.ReadDiscInformation().FirstTrackNumber; } }
		public virtual int TrackCount { get { var info = this.ReadDiscInformation(); return info.LastTrackNumberInLastSession + 1 - info.FirstTrackNumber; } }
		public virtual int SessionCount { get { return this.ReadDiscInformation().SessionCount; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int IMultimediaDevice.FirstTrackNumber { get { return this.FirstTrackNumber; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int IMultimediaDevice.TrackCount { get { return this.TrackCount; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int IMultimediaDevice.SessionCount { get { return this.SessionCount; } }
		System.IO.Stream IMultimediaDevice.OpenTrack(int trackNumber) { return this.OpenTrack(trackNumber); }
		#endregion
	}
}