using ModeSenseCommand = Scsi.ModeSense10Command;
using ModeSelectCommand = Scsi.ModeSelect10Command;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Helper;

namespace Scsi
{
	/// <summary>Represents a generic SCSI device. This class should not be instantiated directly unless no subclasses exist that are appropriate.</summary>
	[Description("Represents a generic SCSI device. This class should not be instantiated directly unless no subclasses exist that are appropriate.")]
	public class ScsiDevice : IScsiDevice
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int MAX_DATA_SIZE = 64 << 10;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private SenseData lastSenseData;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool leaveOpen;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Read10Command read10CommandTemp = new Read10Command();
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Read12Command read12CommandTemp = new Read12Command();
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Read16Command read16CommandTemp = new Read16Command();
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Write10Command write10CommandTemp = new Write10Command();
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private WriteAndVerify10Command writeAndVerify10CommandTemp = new WriteAndVerify10Command();
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Write12Command write12CommandTemp = new Write12Command();
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private uint _TimeoutSeconds = 60;
		//private byte portNumber;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int _PathId = -1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int _TargetId = -1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int _Lun = -1;
		private uint _MaxBlockTransferCount = 16; //It seems like 16 sectors is most optimal... I don't know why
		private ReadCapacityInfo capacityInfo;
		private bool _CanWrite = true; //TODO: Query write protection

		/// <summary>Initializes a new instance of the <see cref="ScsiDevice"/> class.</summary>
		/// <param name="interface">The pass-through interface to use.</param>
		/// <param name="leaveOpen"><c>true</c> to leave the interface open, or <c>false</c> to dispose along with this object.</param>
		public ScsiDevice(IScsiPassThrough @interface, bool leaveOpen)
		{
			this.AutoSense = true;
			this.leaveOpen = leaveOpen;
			this.Interface = @interface;
			this.DefaultPollingInterval = 1;
		}

		public virtual bool AutoSense { get; set; }

		private byte PathId { get { if (this._PathId == -1) { int val; try { val = this.Interface.PathId; } catch { val = 0; } Interlocked.CompareExchange(ref this._PathId, val, -1); } return (byte)this._PathId; } }
		private byte TargetId { get { if (this._TargetId == -1) { int val; try { val = this.Interface.TargetId; } catch { val = 0; } Interlocked.CompareExchange(ref this._TargetId, val, -1); } return (byte)this._TargetId; } }
		private byte LogicalUnitNumber { get { if (this._Lun == -1) { int val; try { val = this.Interface.LogicalUnitNumber; } catch { val = 0; } Interlocked.CompareExchange(ref this._Lun, val, -1); } return (byte)this._Lun; } }

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private CachingModePage Caching { get { return this.GetCachingInformation(new ModeSenseCommand(PageControl.CurrentValues)); } }

		public virtual uint BlockSize { get { return this.GetCapacity().BlockLength; } }

		/// <summary>The capacity of the medium, in bytes.</summary>
		[Description("The capacity of the medium, in bytes.")]
		public long Capacity { get { var cap = this.GetCapacity(); return ((long)cap.LogicalBlockAddress + 1) * (long)cap.BlockLength; } }

		/// <summary>The default polling interval for the drive, in units of 100 ms.</summary>
		public ushort DefaultPollingInterval { get; set; }

		public void Dispose() { this.Dispose(true); GC.SuppressFinalize(this); }

		protected virtual void Dispose(bool disposing) { if (disposing) { try { if (!this.leaveOpen) { this.Interface.Dispose(); } } finally { this.Interface = null; } } }

		[DebuggerHidden]
		protected void ExecuteCommand(ScsiCommand command, DataTransferDirection direction, BufferWithSize buffer) { this.ExecuteCommand(command, direction, buffer, true); }

		//[DebuggerHidden]
		protected ScsiStatus ExecuteCommand(ScsiCommand command, DataTransferDirection direction, BufferWithSize buffer, bool throwOnError)
		{
			bool heapAlloc;
			bool reallocated;

			ScsiStatus status;
			BufferWithSize cdb;
			unsafe
			{
				int cmdSize = Marshaler.SizeOf(command);
				byte* pBuffer = stackalloc byte[cmdSize];
				cdb = new BufferWithSize(pBuffer, cmdSize);
				Marshaler.StructureToPtr(command, cdb);
			}

			BufferWithSize entireAlignedBuffer, portionOfAlignedBuffer;

			unsafe
			{
				int alignmentMask = this.Interface.AlignmentMask;
				if (((ulong)(void*)buffer.Address & (uint)alignmentMask) != 0)
				{
					var alignedLen = buffer.Length32 + alignmentMask;
					bool stackAlloc = Marshaler.ShouldStackAlloc(alignedLen);
					if (stackAlloc)
					{
						byte* pData = stackalloc byte[alignedLen];
						entireAlignedBuffer = new BufferWithSize(pData, alignedLen);
						heapAlloc = false;
					}
					else
					{
						entireAlignedBuffer = BufferWithSize.AllocHGlobal(alignedLen);
						heapAlloc = true;
					}
					portionOfAlignedBuffer = entireAlignedBuffer.ExtractSegment(
						(ulong)((byte*)(unchecked((ulong)((byte*)entireAlignedBuffer.Address + alignmentMask)) & ~unchecked((ulong)(uint)alignmentMask)) - (byte*)entireAlignedBuffer.Address),
						buffer.LengthU64);
					if (direction == DataTransferDirection.SendData) { BufferWithSize.Copy(buffer, UIntPtr.Zero, portionOfAlignedBuffer, UIntPtr.Zero, buffer.Length); }
					heapAlloc = !stackAlloc;
					reallocated = true;
				}
				else
				{
					entireAlignedBuffer = portionOfAlignedBuffer = buffer;
					heapAlloc = false;
					reallocated = false;
				}
			}
			try
			{
				status = this.Interface.ExecuteCommand(cdb, direction, this.PathId, this.TargetId, this.LogicalUnitNumber, portionOfAlignedBuffer, this.TimeoutSeconds, this.AutoSense, out this.lastSenseData);
				if (throwOnError && status != ScsiStatus.Good) { throw ScsiException.CreateException(this.lastSenseData, false); }
			}
			finally
			{
				if (reallocated) { if (direction == DataTransferDirection.ReceiveData) { BufferWithSize.Copy(portionOfAlignedBuffer, UIntPtr.Zero, buffer, UIntPtr.Zero, buffer.Length); } }
				if (reallocated && heapAlloc) { BufferWithSize.FreeHGlobal(entireAlignedBuffer); }
			}

			return status;
		}

		public CachingModePage GetCachingInformation(ModeSenseCommand command) { return this.ModeSense<CachingModePage>(command); }

		private ReadCapacityInfo GetCapacity() { if (this.capacityInfo.BlockLength == 0 || this.HasMediumChanged()) { this.capacityInfo = this.ReadCapacity(); } return this.capacityInfo; }

		public SenseData GetLastSenseData() { return this.lastSenseData.Clone(); }

		public PowerConditionsModePage GetPowerConditions(ModeSenseCommand command) { return this.ModeSense<PowerConditionsModePage>(command); }

		public ReadWriteErrorRecoveryParametersPage GetReadWriteErrorRecoveryParameters(ModeSenseCommand command)
		{ return this.ModeSense<ReadWriteErrorRecoveryParametersPage>(command); }

		protected virtual bool HasMediumChanged() { return false; }

		public StandardInquiryData Inquiry() { return (StandardInquiryData)this.Inquiry(new InquiryCommand(null)); }

		public virtual InquiryData Inquiry(InquiryCommand command)
		{
			if (command.PageCode == null)
			{
				unsafe
				{
					int bufferSize = StandardInquiryData.MinimumSize;
					byte* pData1 = stackalloc byte[(int)bufferSize];
					BufferWithSize buffer = new BufferWithSize(pData1, bufferSize);
					command.AllocationLength = (ushort)buffer.Length;
					this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
					byte additionalLength = command.PageCode == null ? StandardInquiryData.ReadAdditionalLength(buffer) : (byte)0;
					var requiredSize = (int)StandardInquiryData.ADDITIONAL_LENGTH_OFFSET + sizeof(byte) + additionalLength;
					if (bufferSize < requiredSize)
					{
						bufferSize = requiredSize;
						byte* pData2 = stackalloc byte[(int)bufferSize];
						buffer = new BufferWithSize(pData2, bufferSize);
						command.AllocationLength = (ushort)buffer.Length;
						this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
					}
					return Marshaler.PtrToStructure<StandardInquiryData>(buffer);
				}
			}
			else
			{
				unsafe
				{
					int bufferSize = byte.MaxValue;
					byte* pData1 = stackalloc byte[(int)bufferSize];
					BufferWithSize buffer = new BufferWithSize(pData1, bufferSize);
					command.AllocationLength = (ushort)buffer.Length;
					this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
					return VitalProductDataInquiryData.FromBuffer(buffer.Address, buffer.Length32);
				}
			}
		}

		public virtual byte[] InquiryRaw(InquiryCommand command)
		{
			if (command.PageCode == null)
			{
				unsafe
				{
					int bufferSize = StandardInquiryData.MinimumSize;
					byte* pData1 = stackalloc byte[(int)bufferSize];
					BufferWithSize buffer = new BufferWithSize(pData1, bufferSize);
					command.AllocationLength = (ushort)buffer.Length;
					this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
					byte additionalLength = command.PageCode == null ? StandardInquiryData.ReadAdditionalLength(buffer) : (byte)0;
					var requiredSize = (int)StandardInquiryData.ADDITIONAL_LENGTH_OFFSET + sizeof(byte) + additionalLength;
					if (bufferSize < requiredSize)
					{
						bufferSize = requiredSize;
						byte* pData2 = stackalloc byte[(int)bufferSize];
						buffer = new BufferWithSize(pData2, bufferSize);
						command.AllocationLength = (ushort)buffer.Length;
						this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
					}
					return buffer.ToArray();
				}
			}
			else
			{
				unsafe
				{
					int bufferSize = byte.MaxValue;
					byte* pData1 = stackalloc byte[(int)bufferSize];
					BufferWithSize buffer = new BufferWithSize(pData1, bufferSize);
					command.AllocationLength = (ushort)buffer.Length;
					this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
					return buffer.ToArray();
				}
			}
		}

		public IScsiPassThrough Interface { get; private set; }

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private SenseData LastSenseData { get { return this.GetLastSenseData(); } }

		public void ModeSense(ModeSense10Command command, BufferWithSize buffer) { command.AllocationLength = (ushort)buffer.Length; this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer); }

		public TModePage ModeSense<TModePage>(ModeSense10Command command)
			where TModePage : ModePage, new()
		{
			var result = Objects.CreateInstance<TModePage>();
			command.PageCode = result.PageCode;
			unsafe
			{
				checked
				{
					int bufferSize = Marshaler.DefaultSizeOf<Mode10ParametersHeader>() + Marshaler.DefaultSizeOf<TModePage>();
					byte* pBuffer1 = stackalloc byte[bufferSize];
					BufferWithSize buffer = new BufferWithSize(pBuffer1, bufferSize);
					var pHeader = (Mode10ParametersHeader*)buffer.Address;
					command.AllocationLength = (ushort)buffer.LengthU32;
					pHeader->ModeDataLength = (ushort)(buffer.LengthU32 - (int)Mode10ParametersHeader.MODE_DATA_LENGTH_OFFSET);
					this.ModeSense(command, buffer);
					var requiredLength = (int)Mode10ParametersHeader.MODE_DATA_LENGTH_OFFSET + pHeader->ModeDataLength;
					if (buffer.LengthU32 < requiredLength)
					{
						bufferSize = requiredLength;
						byte* pBuffer2 = stackalloc byte[bufferSize];
						buffer = new BufferWithSize(pBuffer2, bufferSize);
						pHeader = (Mode10ParametersHeader*)buffer.Address;
						command.AllocationLength = (ushort)buffer.LengthU32;
						pHeader->ModeDataLength = (ushort)(buffer.LengthU32 - (int)Mode10ParametersHeader.MODE_DATA_LENGTH_OFFSET);
						this.ModeSense(command, buffer);
					}
					BufferWithSize newBuf = buffer.ExtractSegment(Marshaler.DefaultSizeOf<Mode10ParametersHeader>());
					Marshaler.PtrToStructure(newBuf, ref result);
				}
			}
			return result;
		}

		public void ModeSelect(ModeSelect10Command command, BufferWithSize buffer)
		{
			if (command.ParameterListLength == 0) { command.ParameterListLength = (ushort)buffer.Length; }
			else if (command.ParameterListLength > buffer.LengthU32) { throw new ArgumentException("Parameter list length exceeds buffer size.", "command"); }
			this.ExecuteCommand(command, DataTransferDirection.SendData, buffer);
		}

		public void ModeSelect(ModeSelect10Command command, ModePage modePage)
		{
			unsafe
			{
				int bufferSize = (int)Marshaler.SizeOf(modePage) + Marshaler.DefaultSizeOf<Mode10ParametersHeader>();
				byte* pBuffer = stackalloc byte[(int)bufferSize];
				BufferWithSize buffer = new BufferWithSize(pBuffer, bufferSize);
				var pHeader = (Mode10ParametersHeader*)buffer.Address;
				pHeader->ModeDataLength = checked((ushort)(Marshaler.DefaultSizeOf<Mode10ParametersHeader>() + Marshaler.SizeOf(modePage) - sizeof(ushort)));
				var bufferModePage = buffer.ExtractSegment(Marshaler.DefaultSizeOf<Mode10ParametersHeader>());
				Marshaler.StructureToPtr(modePage, bufferModePage);
				this.ModeSelect(command, buffer);
			}
		}

		public void ModeSense(ModeSense06Command command, BufferWithSize buffer) { command.AllocationLength = (byte)buffer.Length; this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer); }

		public TModePage ModeSense<TModePage>(ModeSense06Command command)
			where TModePage : ModePage, new()
		{
			var result = Objects.CreateInstance<TModePage>();
			command.PageCode = result.PageCode;
			unsafe
			{
				int bufferSize = Marshaler.DefaultSizeOf<Mode06ParametersHeader>() + Marshaler.DefaultSizeOf<TModePage>();
				byte* pBuffer1 = stackalloc byte[bufferSize];
				BufferWithSize buffer = new BufferWithSize(pBuffer1, bufferSize);
				var pHeader = (Mode06ParametersHeader*)buffer.Address;
				command.AllocationLength = (byte)buffer.LengthU32;
				pHeader->ModeDataLength = (byte)(buffer.LengthU32 - (int)Mode06ParametersHeader.MODE_DATA_LENGTH_OFFSET);
				this.ModeSense(command, buffer);
				byte requiredLength = (byte)((int)Mode06ParametersHeader.MODE_DATA_LENGTH_OFFSET + pHeader->ModeDataLength);
				if (buffer.LengthU32 < requiredLength)
				{
					bufferSize = requiredLength;
					byte* pBuffer2 = stackalloc byte[bufferSize];
					buffer = new BufferWithSize(pBuffer2, bufferSize);
					pHeader = (Mode06ParametersHeader*)buffer.Address;
					command.AllocationLength = (byte)buffer.LengthU32;
					pHeader->ModeDataLength = (byte)(buffer.LengthU32 - (int)Mode06ParametersHeader.MODE_DATA_LENGTH_OFFSET);
					this.ModeSense(command, buffer);
				}
				BufferWithSize newBuf = buffer.ExtractSegment(Marshaler.DefaultSizeOf<Mode06ParametersHeader>());
				Marshaler.PtrToStructure(newBuf, ref result);
			}
			return result;
		}

		public void ModeSelect(ModeSelect06Command command, BufferWithSize buffer)
		{
			if (command.ParameterListLength == 0) { command.ParameterListLength = (byte)buffer.Length; }
			else if (command.ParameterListLength > buffer.LengthU32) { throw new ArgumentException("Parameter list length exceeds buffer size.", "command"); }
			this.ExecuteCommand(command, DataTransferDirection.SendData, buffer);
		}

		public void ModeSelect(ModeSelect06Command command, ModePage modePage)
		{
			unsafe
			{
				int bufferSize = (int)Marshaler.SizeOf(modePage) + Marshaler.DefaultSizeOf<Mode06ParametersHeader>();
				byte* pBuffer = stackalloc byte[(int)bufferSize];
				BufferWithSize buffer = new BufferWithSize(pBuffer, bufferSize);
				var pHeader = (Mode06ParametersHeader*)buffer.Address;
				pHeader->ModeDataLength = checked((byte)(Marshaler.DefaultSizeOf<Mode06ParametersHeader>() + Marshaler.SizeOf(modePage) - sizeof(byte)));
				var bufferModePage = buffer.ExtractSegment(Marshaler.DefaultSizeOf<Mode06ParametersHeader>());
				Marshaler.StructureToPtr(modePage, bufferModePage);
				this.ModeSelect(command, buffer);
			}
		}

		private PowerConditionsModePage PowerConditions { get { return this.GetPowerConditions(new ModeSenseCommand(PageControl.CurrentValues)); } }

		//public byte[] Read(bool forceUnitAccess, ulong logicalBlockAddress, uint lengthInBlocks) { var bytes = new byte[lengthInBlocks * this.BlockSize]; this.Read(forceUnitAccess, logicalBlockAddress, lengthInBlocks, bytes, 0); return bytes; }

		public void Read(bool forceUnitAccess, ulong logicalBlockAddress, uint lengthInBlocks, byte[] buffer, int bufferOffset)
		{
			//if (lengthInBlocks * this.BlockSize > buffer.Length - bufferOffset) { throw new ArgumentOutOfRangeException("buffer", buffer, "Buffer given was too small."); }
			if (logicalBlockAddress < uint.MaxValue && lengthInBlocks < ushort.MaxValue)
			{
				this.read10CommandTemp.LogicalBlockAddress = (uint)logicalBlockAddress;
				this.read10CommandTemp.TransferBlockCount = (ushort)lengthInBlocks;
				this.read10CommandTemp.ForceUnitAccess = forceUnitAccess;
				this.Read10(this.read10CommandTemp, buffer, bufferOffset);
			}
			else if (logicalBlockAddress < uint.MaxValue && lengthInBlocks < uint.MaxValue)
			{
				this.read12CommandTemp.LogicalBlockAddress = (uint)logicalBlockAddress;
				this.read12CommandTemp.TransferBlockCount = lengthInBlocks;
				this.read12CommandTemp.ForceUnitAccess = forceUnitAccess;
				this.Read12(this.read12CommandTemp, buffer, bufferOffset);
			}
			else
			{
				this.read16CommandTemp.LogicalBlockAddress = logicalBlockAddress;
				this.read16CommandTemp.TransferBlockCount = lengthInBlocks;
				this.read16CommandTemp.ForceUnitAccess = forceUnitAccess;
				this.Read16(this.read16CommandTemp, buffer, bufferOffset);
			}
		}

		//public byte[] Read06(Read06Command command) { var result = new byte[command.TransferBlockCount * this.BlockSize]; this.Read06(command, result, 0); return result; }

		public void Read06(Read06Command command, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.Read06(command, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		public void Read06(Read06Command command, BufferWithSize buffer)
		{
			var blockSize = this.BlockSize;
			if ((ulong)command.TransferBlockCount * blockSize > buffer.LengthU32) { throw new ArgumentException("Buffer was too small for the given transfer length.", "buffer"); }
			var totalTransferBlockCount = command.TransferBlockCount;
			uint maximumTransferBlockCount = this.MaxBlockTransferCount;

			var blocksLeft = totalTransferBlockCount;
			while (blocksLeft > 0)
			{
				var blockCountToProcess = (byte)Math.Min(blocksLeft, maximumTransferBlockCount);
				command.TransferBlockCount = blockCountToProcess;
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer.ExtractSegment(blockSize * (totalTransferBlockCount - blocksLeft), blockSize * blockCountToProcess));
				blocksLeft -= blockCountToProcess;
				unchecked { command.LogicalBlockAddress += blockCountToProcess; }
			}
		}

		//public byte[] Read10(Read10Command command) { var result = new byte[command.TransferBlockCount * this.BlockSize]; this.Read10(command, result, 0); return result; }

		public void Read10(Read10Command command, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.Read10(command, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		public void Read10(Read10Command command, BufferWithSize buffer)
		{
			var blockSize = this.BlockSize;
			if ((ulong)command.TransferBlockCount * blockSize > buffer.LengthU32) { throw new ArgumentException("Buffer was too small for the given transfer length.", "buffer"); }
			var totalTransferBlockCount = command.TransferBlockCount;
			uint maximumTransferBlockCount = this.MaxBlockTransferCount;

			var blocksLeft = totalTransferBlockCount;
			while (blocksLeft > 0)
			{
				var blockCountToProcess = (ushort)Math.Min(blocksLeft, maximumTransferBlockCount);
				command.TransferBlockCount = blockCountToProcess;
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer.ExtractSegment(blockSize * (totalTransferBlockCount - blocksLeft), blockSize * blockCountToProcess));
				blocksLeft -= blockCountToProcess;
				unchecked { command.LogicalBlockAddress += blockCountToProcess; }
			}
		}

		//public byte[] Read12(Read12Command command) { byte[] result = new byte[command.TransferBlockCount * this.BlockSize]; this.Read12(command, result, 0); return result; }

		public void Read12(Read12Command command, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.Read12(command, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		public void Read12(Read12Command command, BufferWithSize buffer)
		{
			var blockSize = this.BlockSize;
			if ((ulong)command.TransferBlockCount * blockSize > buffer.LengthU32) { throw new ArgumentException("Buffer was too small for the given transfer length.", "buffer"); }
			var totalTransferBlockCount = command.TransferBlockCount;
			uint maximumTransferBlockCount = this.MaxBlockTransferCount;

			var blocksLeft = totalTransferBlockCount;
			while (blocksLeft > 0)
			{
				var blockCountToProcess = Math.Min(blocksLeft, maximumTransferBlockCount);
				command.TransferBlockCount = blockCountToProcess;
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer.ExtractSegment(blockSize * (totalTransferBlockCount - blocksLeft), blockSize * blockCountToProcess));
				blocksLeft -= blockCountToProcess;
				unchecked { command.LogicalBlockAddress += blockCountToProcess; }
			}
		}

		//public byte[] Read16(Read16Command command) { var result = new byte[command.TransferBlockCount * this.BlockSize]; this.Read16(command, result, 0); return result; }

		public void Read16(Read16Command command, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.Read16(command, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		public void Read16(Read16Command command, BufferWithSize buffer)
		{
			var blockSize = this.BlockSize;
			if ((ulong)command.TransferBlockCount * blockSize > buffer.LengthU32) { throw new ArgumentException("Buffer was too small for the given transfer length.", "buffer"); }
			var totalTransferBlockCount = command.TransferBlockCount;
			uint maximumTransferBlockCount = this.MaxBlockTransferCount;

			var blocksLeft = totalTransferBlockCount;
			while (blocksLeft > 0)
			{
				var blockCountToProcess = Math.Min(blocksLeft, maximumTransferBlockCount);
				command.TransferBlockCount = blockCountToProcess;
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer.ExtractSegment(blockSize * (totalTransferBlockCount - blocksLeft), blockSize * blockCountToProcess));
				blocksLeft -= blockCountToProcess;
				unchecked { command.LogicalBlockAddress += blockCountToProcess; }
			}
		}

		//public byte[] Read32(Read32Command command) { var result = new byte[command.TransferBlockCount * this.BlockSize]; this.Read32(command, result, 0); return result; }

		public void Read32(Read32Command command, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.Read32(command, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		public void Read32(Read32Command command, BufferWithSize buffer)
		{
			var blockSize = this.BlockSize;
			if ((ulong)command.TransferBlockCount * blockSize > buffer.LengthU32) { throw new ArgumentException("Buffer was too small for the given transfer length.", "buffer"); }
			var totalTransferBlockCount = command.TransferBlockCount;
			uint maximumTransferBlockCount = this.MaxBlockTransferCount;

			var blocksLeft = totalTransferBlockCount;
			while (blocksLeft > 0)
			{
				var blockCountToProcess = Math.Min(blocksLeft, maximumTransferBlockCount);
				command.TransferBlockCount = blockCountToProcess;
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer.ExtractSegment(blockSize * (totalTransferBlockCount - blocksLeft), blockSize * blockCountToProcess));
				blocksLeft -= blockCountToProcess;
				unchecked { command.LogicalBlockAddress += blockCountToProcess; }
			}
		}

		public byte[] ReadBufferData(ReadBufferCommand command, int length) { var result = new byte[length]; this.ReadBufferData(command, result, 0); return result; }

		public void ReadBufferData(ReadBufferCommand command, byte[] buffer, int resultOffset) { this.ReadBufferData(command, buffer, resultOffset, buffer.Length - resultOffset); }

		public void ReadBufferData(ReadBufferCommand command, byte[] buffer, int resultOffset, int resultLength)
		{
			if (resultLength > buffer.Length - resultOffset)
			{ throw new ArgumentOutOfRangeException("resultLength", resultLength, "Result length cannot overflow buffer."); }
			unsafe
			{
				fixed (byte* pBuffer = buffer)
				{ this.ReadBufferData(command, new BufferWithSize(pBuffer + resultOffset, resultLength)); }
			}
		}

		public void ReadBufferData(ReadBufferCommand command, BufferWithSize buffer) { command.Mode = ReadBufferMode.Data; this.ReadBuffer(command, buffer); }

		public ReadBufferDescriptor ReadBufferDescriptor(ReadBufferCommand command)
		{
			ReadBufferDescriptor result = new ReadBufferDescriptor();
			command.Mode = ReadBufferMode.Descriptor;
			unsafe { this.ReadBuffer(command, new BufferWithSize((IntPtr)(&result), Marshaler.DefaultSizeOf<ReadBufferDescriptor>())); }
			return result;
		}

		public BufferCombinedHeaderAndData ReadBufferCombinedHeaderAndData(ReadBufferCommand command)
		{
			command.Mode = ReadBufferMode.CombinedHeaderAndData;
			unsafe
			{
				BufferWithSize buffer;
				int bufferSize = Marshaler.DefaultSizeOf<BufferCombinedHeaderAndData>();
				bool stackAlloc = Marshaler.ShouldStackAlloc(bufferSize);
				if (stackAlloc)
				{
					byte* pResult1 = stackalloc byte[bufferSize];
					buffer = new BufferWithSize(pResult1, bufferSize);
				}
				else { buffer = BufferWithSize.AllocHGlobal(bufferSize); }
				try
				{
					this.ReadBuffer(command, buffer);
					int requiredSize = (int)BufferCombinedHeaderAndData.ReadBufferCapacity(buffer.Address) + Marshaler.DefaultSizeOf<BufferCombinedHeaderAndData>();
					if (bufferSize < requiredSize)
					{
						bufferSize = requiredSize;
						bool stackAlloc2 = Marshaler.ShouldStackAlloc(bufferSize);
						if (stackAlloc)
						{
							if (stackAlloc2)
							{
								byte* pResult2 = stackalloc byte[bufferSize];
								buffer = new BufferWithSize(pResult2, bufferSize);
							}
							else { buffer = BufferWithSize.AllocHGlobal(bufferSize); }
						}
						else
						{
							if (stackAlloc2)
							{
								BufferWithSize.FreeHGlobal(buffer);
								byte* pResult2 = stackalloc byte[bufferSize];
								buffer = new BufferWithSize(pResult2, bufferSize);
							}
							else { buffer = BufferWithSize.ReAllocHGlobal(buffer, bufferSize); }
						}
						stackAlloc = stackAlloc2;
						this.ReadBuffer(command, buffer);
					}
					return Marshaler.PtrToStructure<BufferCombinedHeaderAndData>(buffer);
				}
				finally { if (!stackAlloc) { BufferWithSize.FreeHGlobal(buffer); } }
			}
		}

		private void ReadBuffer(ReadBufferCommand command, BufferWithSize buffer) { command.AllocationLength = (ushort)buffer.Length; this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer); }

		public ReadCapacityInfo ReadCapacity() { return this.ReadCapacity(new ReadCapacityCommand()); }

		public ReadCapacityInfo ReadCapacity(ReadCapacityCommand command)
		{
			var result = new ReadCapacityInfo();
			unsafe { this.ExecuteCommand(command, DataTransferDirection.ReceiveData, new BufferWithSize((IntPtr)(&result), Marshaler.DefaultSizeOf<ReadCapacityInfo>())); }
			return result;
		}

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private ReadWriteErrorRecoveryParametersPage ReadWriteErrorRecoveryParameters { get { return this.GetReadWriteErrorRecoveryParameters(new ModeSenseCommand(PageControl.CurrentValues)); } }

		public LunsParameterList ReportLuns(ReportLunsCommand command) //The AllocationLength is ignored
		{
			unsafe
			{
				BufferWithSize buffer;
				int bufferSize = Marshaler.DefaultSizeOf<LunsParameterList>();
				bool stackAlloc = Marshaler.ShouldStackAlloc(bufferSize);
				if (stackAlloc)
				{
					byte* pResult1 = stackalloc byte[bufferSize];
					buffer = new BufferWithSize(pResult1, bufferSize);
				}
				else { buffer = BufferWithSize.AllocHGlobal(bufferSize); }
				try
				{
					command.AllocationLength = buffer.LengthU32;
					this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
					var requiredSize = (int)LunsParameterList.LUNS_OFFSET + (int)LunsParameterList.ReadLunListLength(buffer);
					if (bufferSize < requiredSize)
					{
						bufferSize = requiredSize;
						bool stackAlloc2 = Marshaler.ShouldStackAlloc(bufferSize);
						if (stackAlloc)
						{
							if (stackAlloc2)
							{
								byte* pResult2 = stackalloc byte[bufferSize];
								buffer = new BufferWithSize(pResult2, bufferSize);
							}
							else { buffer = BufferWithSize.AllocHGlobal(bufferSize); }
						}
						else
						{
							if (stackAlloc2)
							{
								BufferWithSize.FreeHGlobal(buffer);
								byte* pResult2 = stackalloc byte[bufferSize];
								buffer = new BufferWithSize(pResult2, bufferSize);
							}
							else { buffer = BufferWithSize.ReAllocHGlobal(buffer, bufferSize); }
						}
						stackAlloc = stackAlloc2;
						this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
					}
					command.AllocationLength = buffer.LengthU32;
					return Marshaler.PtrToStructure<LunsParameterList>(buffer);
				}
				finally { if (!stackAlloc) { BufferWithSize.FreeHGlobal(buffer); } }
			}
		}

		public SenseData RequestSense() { return this.RequestSense(new RequestSenseCommand()); }

		public SenseData RequestSense(RequestSenseCommand command)
		{
			SenseData result;
			unsafe
			{
				int bufferSize = 252;
				byte* pSenseData = stackalloc byte[bufferSize];
				BufferWithSize buffer = new BufferWithSize(pSenseData, bufferSize);
				command.AllocationLength = (byte)buffer.Length;
				this.ExecuteCommand(command, DataTransferDirection.ReceiveData, buffer);
				result = Marshaler.PtrToStructure<SenseData>(buffer);
			}
			return result;
		}

		public void Seek10(Seek10Command command) { this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }

		public void SendDiagnostic(SendDiagnosticCommand command) { this.SendDiagnostic(command, BufferWithSize.Zero); }

		public void SendDiagnostic(SendDiagnosticCommand command, BufferWithSize buffer)
		{
			command.ParameterListLength = (byte)buffer.Length;
			switch (command.SelfTestCode)
			{
				case SelfTestCode.BackgroundShortSelfTest:
					if (command.ParameterListLength != 0) { throw new InvalidOperationException(); }
					break;
				case SelfTestCode.BackgroundExtendedSelfTest:
					if (command.ParameterListLength != 0) { throw new InvalidOperationException(); }
					break;
				case SelfTestCode.AbortBackgroundSelfTest:
					if (command.ParameterListLength != 0) { throw new InvalidOperationException(); }
					break;
				case SelfTestCode.ForegroundShortSelfTest:
					if (command.ParameterListLength != 0) { throw new InvalidOperationException(); }
					break;
				case SelfTestCode.ForegroundExtendedSelfTest:
					if (command.ParameterListLength != 0) { throw new InvalidOperationException(); }
					break;
				case null:
					if (command.ParameterListLength != 0) { throw new InvalidOperationException(); }
					break;
			}
			this.ExecuteCommand(command, DataTransferDirection.SendData, buffer);
		}

		public void SetCachingInformation(ModeSelectCommand command, CachingModePage modePage) { this.ModeSelect(command, modePage); }

		public void SetPowerConditions(ModeSelectCommand command, PowerConditionsModePage modePage) { this.ModeSelect(command, modePage); }

		public void SetReadWriteErrorRecoveryParameters(ModeSelectCommand command, ReadWriteErrorRecoveryParametersPage modePage) { this.ModeSelect(command, modePage); }

		public void SetRemovableMediaBit(SetRemovableMediaBitCommand command)
		{
			unsafe
			{
				const byte BUFFER_SIZE = 36;
				byte* pBuffer = stackalloc byte[BUFFER_SIZE];
				this.ExecuteCommand(command, DataTransferDirection.SendData, new BufferWithSize(pBuffer, BUFFER_SIZE));
			}
		}

		public void SynchronizeCache() { this.SynchronizeCache(new SynchronizeCache10Command()); }

		public void SynchronizeCache(SynchronizeCache10Command command) { this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero); }

		public ScsiStatus TestUnitReady() { return this.TestUnitReady(new TestUnitReadyCommand()); }

		public ScsiStatus TestUnitReady(TestUnitReadyCommand command) { return this.ExecuteCommand(command, DataTransferDirection.NoData, BufferWithSize.Zero, false); }

		public uint TimeoutSeconds { get { return this._TimeoutSeconds; } set { this._TimeoutSeconds = value; } }

		public uint MaxBlockTransferCount { get { return this._MaxBlockTransferCount; } }

		public void Write(bool forceUnitAccess, bool verify, ulong logicalBlockAddress, uint lengthInBlocks, byte[] buffer, int bufferOffset)
		{
			//if (lengthInBlocks * this.BlockSize > buffer.Length - bufferOffset) { throw new ArgumentOutOfRangeException("buffer", buffer, "Buffer given was too small."); }
			if (logicalBlockAddress < uint.MaxValue && lengthInBlocks < ushort.MaxValue)
			{
				if (verify)
				{
					this.writeAndVerify10CommandTemp.LogicalBlockAddress = (uint)logicalBlockAddress;
					this.writeAndVerify10CommandTemp.TransferBlockCount = (ushort)lengthInBlocks;
					this.WriteAndVerify10(this.writeAndVerify10CommandTemp, buffer, bufferOffset);
				}
				else
				{
					this.write10CommandTemp.LogicalBlockAddress = (uint)logicalBlockAddress;
					this.write10CommandTemp.TransferBlockCount = (ushort)lengthInBlocks;
					this.write10CommandTemp.ForceUnitAccess = forceUnitAccess;
					this.Write10(this.write10CommandTemp, buffer, bufferOffset);
				}
			}
			else if (logicalBlockAddress < uint.MaxValue && lengthInBlocks < uint.MaxValue)
			{
				if (verify) { throw new NotSupportedException("Verification not supported for WRITE (12) command."); }
				this.write12CommandTemp.LogicalBlockAddress = (uint)logicalBlockAddress;
				this.write12CommandTemp.TransferBlockCount = lengthInBlocks;
				this.write12CommandTemp.ForceUnitAccess = forceUnitAccess;
				this.Write12(this.write12CommandTemp, buffer, bufferOffset);
			}
			else { throw new NotSupportedException(); }
		}

		public void Write10(Write10Command command, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.Write10(command, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		public void Write10(Write10Command command, BufferWithSize buffer)
		{
			var blockSize = this.BlockSize;
			if ((ulong)command.TransferBlockCount * blockSize > buffer.LengthU32) { throw new ArgumentException("Buffer was too small for the given transfer length.", "buffer"); }
			var totalTransferBlockCount = command.TransferBlockCount;
			uint maximumTransferBlockCount = this.MaxBlockTransferCount;

			var blocksLeft = totalTransferBlockCount;
			while (blocksLeft > 0)
			{
				var blockCountToProcess = (ushort)Math.Min(blocksLeft, maximumTransferBlockCount);
				command.TransferBlockCount = blockCountToProcess;
				this.ExecuteCommand(command, DataTransferDirection.SendData, buffer.ExtractSegment(blockSize * (totalTransferBlockCount - blocksLeft), blockSize * blockCountToProcess));
				blocksLeft -= blockCountToProcess;
				unchecked { command.LogicalBlockAddress += blockCountToProcess; }
			}
		}

		public void WriteAndVerify10(WriteAndVerify10Command command, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.WriteAndVerify10(command, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		public void WriteAndVerify10(WriteAndVerify10Command command, BufferWithSize buffer)
		{
			var blockSize = this.BlockSize;
			if ((ulong)command.TransferBlockCount * blockSize > buffer.LengthU32) { throw new ArgumentException("Buffer was too small for the given transfer length.", "buffer"); }
			var totalTransferBlockCount = command.TransferBlockCount;
			uint maximumTransferBlockCount = this.MaxBlockTransferCount;

			var blocksLeft = totalTransferBlockCount;
			while (blocksLeft > 0)
			{
				var blockCountToProcess = (ushort)Math.Min(blocksLeft, maximumTransferBlockCount);
				command.TransferBlockCount = blockCountToProcess;
				this.ExecuteCommand(command, DataTransferDirection.SendData, buffer.ExtractSegment(blockSize * (totalTransferBlockCount - blocksLeft), blockSize * blockCountToProcess));
				blocksLeft -= blockCountToProcess;
				unchecked { command.LogicalBlockAddress += blockCountToProcess; }
			}
		}

		public void Write12(Write12Command command, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.Write12(command, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		public void Write12(Write12Command command, BufferWithSize buffer)
		{
			var blockSize = this.BlockSize;
			if ((ulong)command.TransferBlockCount * blockSize > buffer.LengthU32) { throw new ArgumentException("Buffer was too small for the given transfer length.", "buffer"); }
			var totalTransferBlockCount = command.TransferBlockCount;
			uint maximumTransferBlockCount = this.MaxBlockTransferCount;

			var blocksLeft = totalTransferBlockCount;
			while (blocksLeft > 0)
			{
				var blockCountToProcess = Math.Min(blocksLeft, maximumTransferBlockCount);
				command.TransferBlockCount = blockCountToProcess;
				this.ExecuteCommand(command, DataTransferDirection.SendData, buffer.ExtractSegment(blockSize * (totalTransferBlockCount - blocksLeft), blockSize * blockCountToProcess));
				blocksLeft -= blockCountToProcess;
				unchecked { command.LogicalBlockAddress += blockCountToProcess; }
			}
		}

		#region IScsiDevice
		protected virtual void Read(long position, byte[] buffer, int bufferOffset, int length, bool forceUnitAccess)
		{
			bool unaligned = false;
			long rem;
			var blockSize = this.BlockSize;
			long logicalBlockAddress = Math.DivRem(position, blockSize, out rem);
			//if (rem != 0) { var ex = new DataMisalignedException(); throw new ArgumentException("Value is not aligned to the correct boundary.", "position", ex); }
			unaligned |= rem != 0;
			var blockLength = (uint)Math.DivRem(length, blockSize, out rem);
			//if (rem != 0) { var ex = new DataMisalignedException(); throw new ArgumentException("Value is not aligned to the correct boundary.", "length", ex); }
			unaligned |= rem != 0;
			if (length > buffer.Length - bufferOffset) { throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."); }
			if (!unaligned)
			{
				this.Read(forceUnitAccess, (ulong)logicalBlockAddress, blockLength, buffer, bufferOffset);
			}
			else
			{
				var bpsRead = blockSize;
				var bpsWrite = blockSize;
				if (bpsRead != bpsWrite) { throw new NotSupportedException("Read-Modify-Write not supported for unequal block lengths."); }
				long alignedPosition = position / bpsWrite * bpsWrite;

				var alignedData = new byte[(position + length - alignedPosition + bpsWrite - 1) / bpsWrite * bpsWrite];
				this.Read(alignedPosition, alignedData, 0, alignedData.Length, forceUnitAccess);
				Buffer.BlockCopy(alignedData, (int)(position - alignedPosition), buffer, bufferOffset, length);
			}

			/*
			var blockSize = this.BlockSize;
			long rem;
			long logicalBlockAddress = Math.DivRem(position, blockSize, out rem);
			if (rem != 0) { var ex = new DataMisalignedException(); throw new ArgumentException(null, "position", ex); }
			var blockLength = (uint)Math.DivRem(length, blockSize, out rem);
			if (rem != 0) { var ex = new DataMisalignedException(); throw new ArgumentException(null, "length", ex); }
			this.Read(forceUnitAccess, (ulong)logicalBlockAddress, blockLength, buffer, bufferOffset);
			//*/
		}

		protected virtual void Write(long position, byte[] buffer, int bufferOffset, int length, bool forceUnitAccess, bool verify)
		{
			bool unaligned = false;
			long rem;
			var blockSize = this.BlockSize;
			long logicalBlockAddress = Math.DivRem(position, blockSize, out rem);
			//if (rem != 0) { var ex = new DataMisalignedException(); throw new ArgumentException("Value is not aligned to the correct boundary.", "position", ex); }
			unaligned |= rem != 0;
			var blockLength = (uint)Math.DivRem(length, blockSize, out rem);
			//if (rem != 0) { var ex = new DataMisalignedException(); throw new ArgumentException("Value is not aligned to the correct boundary.", "length", ex); }
			unaligned |= rem != 0;
			if (length > buffer.Length - bufferOffset) { throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."); }
			if (!unaligned)
			{
				this.Write(forceUnitAccess, verify, (ulong)logicalBlockAddress, blockLength, buffer, bufferOffset);
			}
			else
			{
				var bpsRead = blockSize;
				var bpsWrite = blockSize;
				if (bpsRead != bpsWrite) { throw new NotSupportedException("Read-Modify-Write not supported for unequal block lengths."); }
				long alignedPosition = position / bpsWrite * bpsWrite;

				var alignedData = new byte[(position + length - alignedPosition + bpsWrite - 1) / bpsWrite * bpsWrite];
				this.Read(alignedPosition, alignedData, 0, alignedData.Length, forceUnitAccess);
				Buffer.BlockCopy(buffer, bufferOffset, alignedData, (int)(position - alignedPosition), length);
				this.Write(forceUnitAccess, verify, (ulong)alignedPosition / bpsWrite, (uint)alignedData.Length / bpsWrite, alignedData, 0);
			}
		}

		protected virtual void Flush() { this.SynchronizeCache(); }
		public virtual ScsiStatus Status { get { return this.TestUnitReady(); } }
		void IScsiDevice.Read(long position, byte[] buffer, int bufferOffset, int length, bool forceUnitAccess) { this.Read(position, buffer, bufferOffset, length, forceUnitAccess); }
		void IScsiDevice.Write(long position, byte[] buffer, int bufferOffset, int length, bool forceUnitAccess, bool verify) { this.Write(position, buffer, bufferOffset, length, forceUnitAccess, verify); }
		void IScsiDevice.Flush() { this.Flush(); }
		#endregion

		public virtual bool CanRead { get { return true; } }
		public virtual bool CanSeek { get { return true; } }
		public virtual bool CanWrite { get { return this._CanWrite; } }



		public static ScsiDevice Create(IScsiPassThrough passThrough, bool leaveOpen)
		{
			var inq = passThrough.ScsiInquiry(true);
			ScsiDevice result;
			switch (inq.PeripheralDeviceType)
			{
				case PeripheralDeviceType.DirectAccessBlockDevice:
					result = new Block.BlockDevice(passThrough, leaveOpen);
					break;
				case PeripheralDeviceType.CDDvdDevice:
					result = new Multimedia.MultimediaDevice(passThrough, leaveOpen);
					break;
				default:
					result = new ScsiDevice(passThrough, leaveOpen);
					break;
			}
			return result;
		}
	}
}