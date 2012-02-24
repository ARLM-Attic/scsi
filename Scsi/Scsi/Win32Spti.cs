using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;
using Helper.IO;
using Microsoft.Win32.SafeHandles;
using System.Threading;

namespace Scsi
{
	/// <summary>The Windows Scsi Pass-Through Interface. This uses a file handle (obtainable through instantiating a <see cref="Win32FileStream"/> object with read/write access) to send I/O control codes to the underlying Scsi device.</summary>
	//[DebuggerStepThrough]
	public class Win32Spti : IScsiPassThrough
	{
		private delegate void Action<T1, T2>(T1 param1, T2 param2);
		private static Action<Exception, string> SetExceptionMessage;
		private static Converter<Exception, string> GetExceptionMessage;

		#region I/O Control Codes
		// I/O Control Code Format
		// Bit:   [  31  ] [30 ..... 16] [15 ......... 14] [  13  ] [12 ....... 02] [01 ....... 00]
		// Value: [Common] [Device type] [Required access] [Custom] [Function code] [Transfer type]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FILE_DEVICE_MASS_STORAGE = 0x0000002D;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FILE_DEVICE_CD_ROM = 0x00000002;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_CDROM_BASE = FILE_DEVICE_CD_ROM;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_CDROM_GET_CONFIGURATION = IOCTL_CDROM_BASE << 16 | 1 << 14 | 0x0016 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_CDROM_GET_INQUIRY_DATA = IOCTL_CDROM_BASE << 16 | 1 << 14 | 0x0019 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_STORAGE_BASE = FILE_DEVICE_MASS_STORAGE;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_STORAGE_CHECK_VERIFY = IOCTL_STORAGE_BASE << 16 | 1 << 14 | 0x0200 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_STORAGE_CHECK_VERIFY2 = IOCTL_STORAGE_BASE << 16 | 0 << 14 | 0x0200 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_STORAGE_MCN_CONTROL = IOCTL_STORAGE_BASE << 16 | 0 << 14 | 0x0251 << 2 | 0;
		//IOCTL_STORAGE_EJECT_MEDIA CTL_CODE(IOCTL_STORAGE_BASE, 0x0202, METHOD_BUFFERED, FILE_READ_ACCESS)
		//IOCTL_STORAGE_EJECTION_CONTROL CTL_CODE(IOCTL_STORAGE_BASE, 0x0250, METHOD_BUFFERED, FILE_ANY_ACCESS)
		//IOCTL_STORAGE_FIND_NEW_DEVICES CTL_CODE(IOCTL_STORAGE_BASE, 0x0206, METHOD_BUFFERED, FILE_READ_ACCESS)
		//IOCTL_STORAGE_GET_DEVICE_NUMBER CTL_CODE(IOCTL_STORAGE_BASE, 0x0420, METHOD_BUFFERED, FILE_ANY_ACCESS)
		//IOCTL_STORAGE_GET_MEDIA_SERIAL_NUMBER CTL_CODE(IOCTL_STORAGE_BASE, 0x0304, METHOD_BUFFERED, FILE_ANY_ACCESS)
		//IOCTL_STORAGE_LOAD_MEDIA CTL_CODE(IOCTL_STORAGE_BASE, 0x0203, METHOD_BUFFERED, FILE_READ_ACCESS)
		//IOCTL_STORAGE_LOAD_MEDIA2 CTL_CODE(IOCTL_STORAGE_BASE, 0x0203, METHOD_BUFFERED, FILE_ANY_ACCESS)
		//IOCTL_STORAGE_MEDIA_REMOVAL CTL_CODE(IOCTL_STORAGE_BASE, 0x0201, METHOD_BUFFERED, FILE_READ_ACCESS)
		//IOCTL_STORAGE_PREDICT_FAILURE CTL_CODE(IOCTL_STORAGE_BASE, 0x0440, METHOD_BUFFERED, FILE_ANY_ACCESS)
		//IOCTL_STORAGE_RELEASE CTL_CODE(IOCTL_STORAGE_BASE, 0x0205, METHOD_BUFFERED, FILE_READ_ACCESS)
		//IOCTL_STORAGE_RESERVE CTL_CODE(IOCTL_STORAGE_BASE, 0x0204, METHOD_BUFFERED, FILE_READ_ACCESS)
		//IOCTL_STORAGE_GET_HOTPLUG_INFO CTL_CODE(IOCTL_STORAGE_BASE, 0x0305, METHOD_BUFFERED, FILE_ANY_ACCESS)
		//IOCTL_STORAGE_SET_HOTPLUG_INFO CTL_CODE(IOCTL_STORAGE_BASE, 0x0306, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS)
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_STORAGE_QUERY_PROPERTY = IOCTL_STORAGE_BASE << 16 | 0 << 14 | 0x0500 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_STORAGE_GET_MEDIA_TYPES = IOCTL_STORAGE_BASE << 16 | 0 << 14 | 0x0300 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_STORAGE_GET_MEDIA_TYPES_EX = IOCTL_STORAGE_BASE << 16 | 0 << 14 | 0x0301 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_STORAGE_RESET_BUS = IOCTL_STORAGE_BASE << 16 | 3 << 14 | 0x0400 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_STORAGE_RESET_DEVICE = IOCTL_STORAGE_BASE << 16 | 3 << 14 | 0x0401 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FILE_DEVICE_FILE_SYSTEM = 0x00000009;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FSCTL_LOCK_VOLUME = FILE_DEVICE_FILE_SYSTEM << 16 | 0 << 14 | 0x0006 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FSCTL_UNLOCK_VOLUME = FILE_DEVICE_FILE_SYSTEM << 16 | 0 << 14 | 0x0007 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FSCTL_DISMOUNT_VOLUME = FILE_DEVICE_FILE_SYSTEM << 16 | 0 << 14 | 0x0008 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FSCTL_IS_VOLUME_MOUNTED = FILE_DEVICE_FILE_SYSTEM << 16 | 0 << 14 | 0x000A << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_SCSI_BASE = 0x00000004;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_SCSI_MINIPORT = IOCTL_SCSI_BASE << 16 | 3 << 14 | 0x0402 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_SCSI_GET_INQUIRY_DATA = IOCTL_SCSI_BASE << 16 | 0 << 14 | 0x0403 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_SCSI_GET_CAPABILITIES = IOCTL_SCSI_BASE << 16 | 0 << 14 | 0x0404 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_SCSI_PASS_THROUGH = IOCTL_SCSI_BASE << 16 | 3 << 14 | 0x0401 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_SCSI_PASS_THROUGH_DIRECT = IOCTL_SCSI_BASE << 16 | 3 << 14 | 0x0405 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_SCSI_GET_ADDRESS = IOCTL_SCSI_BASE << 16 | 0 << 14 | 0x0406 << 2 | 0;
		#endregion

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool leaveDeviceOpen;
		private SafeFileHandle deviceHandle;
		private int _MaximumDataTransferBlockCount; //uninitialized
		private int passThroughCounter = 0;
		private int _Alignment = -1;
		private byte[] buffer;

		public Win32Spti(SafeFileHandle deviceHandle, bool leaveDeviceOpen) : this(deviceHandle, leaveDeviceOpen, true) { }
		public Win32Spti(SafeFileHandle deviceHandle, bool leaveDeviceOpen, bool passThroughDirect) { this.deviceHandle = deviceHandle; this.leaveDeviceOpen = leaveDeviceOpen; this.UseDirectIO = passThroughDirect; }

		/// <returns>Checks whether the mounted medium has changed. If the value is negative, then the medium has probably changed. If the value is zero, the medium has probably not changed. If the value is positive, it represents the media change count; compare it with a previous value to detect changes.</returns>
		public int CheckVerify()
		{
			unsafe
			{
				int mcn;
				int bytesReturned;
				if (!DeviceIoControl(this.deviceHandle, IOCTL_STORAGE_CHECK_VERIFY, IntPtr.Zero, 0, (IntPtr)(&mcn), sizeof(uint), out bytesReturned, IntPtr.Zero))
				{
					int le = Marshal.GetLastWin32Error();
					if (le != 1110 && le != 1117 && le != 21) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
					else { mcn = -1; }
				}
				return mcn;
			}
		}

		/// <returns>Checks whether the medium has changed. If the value is negative, then the medium has probably changed. If the value is zero, the medium has probably not changed. If the value is positive, it represents the media change count; compare it with a previous value to detect changes.</returns>
		public int CheckVerify2()
		{
			unsafe
			{
				int mcn;
				int bytesReturned;
				if (!DeviceIoControl(this.deviceHandle, IOCTL_STORAGE_CHECK_VERIFY2, IntPtr.Zero, 0, (IntPtr)(&mcn), sizeof(uint), out bytesReturned, IntPtr.Zero))
				{
					int le = Marshal.GetLastWin32Error();
					if (le != 1110 && le != 1117 && le != 21) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
					else { mcn = -1; }
				}
				return mcn;
			}
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		private static void ThrowExceptionForHR(int hr)
		{
			var ex = Marshal.GetExceptionForHR(hr);

			if (GetExceptionMessage == null)
			{
				try
				{
					var dyn = new System.Reflection.Emit.DynamicMethod("GetMessage", typeof(string), new Type[] { typeof(Exception) }, typeof(Exception), true);
					var field = typeof(Exception).GetField("_message", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
					var gen = dyn.GetILGenerator();
					gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
					gen.Emit(System.Reflection.Emit.OpCodes.Ldfld, field);
					gen.Emit(System.Reflection.Emit.OpCodes.Ret);
					System.Threading.Interlocked.CompareExchange(ref GetExceptionMessage, (Converter<Exception, string>)dyn.CreateDelegate(typeof(Converter<Exception, string>)), null);
				}
				catch { }
			}
			if (GetExceptionMessage != null)
			{
				var msg = GetExceptionMessage(ex);
				var i = msg.IndexOf(" (Exception from HRESULT:");
				if (i >= 0)
				{
					if (SetExceptionMessage == null)
					{
						try
						{
							var dyn = new System.Reflection.Emit.DynamicMethod("SetMessage", null, new Type[] { typeof(Exception), typeof(string) }, typeof(Exception), true);
							var field = typeof(Exception).GetField("_message", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
							var gen = dyn.GetILGenerator();
							gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
							gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
							gen.Emit(System.Reflection.Emit.OpCodes.Stfld, field);
							gen.Emit(System.Reflection.Emit.OpCodes.Ret);
							System.Threading.Interlocked.CompareExchange(ref SetExceptionMessage, (Action<Exception, string>)dyn.CreateDelegate(typeof(Action<Exception, string>)), null);
						}
						catch { }
					}
					if (SetExceptionMessage != null)
					{
						SetExceptionMessage(ex, msg.Substring(0, i));
					}
				}
			}
			throw ex;
		}

		public void Dispose() { this.Dispose(true); GC.SuppressFinalize(this); }
		public void DismountVolume() { int temp; if (!DeviceIoControl(this.deviceHandle, FSCTL_DISMOUNT_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out temp, IntPtr.Zero)) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); } }
		public bool IsVolumeMounted() { int temp; return DeviceIoControl(this.deviceHandle, FSCTL_IS_VOLUME_MOUNTED, IntPtr.Zero, 0, IntPtr.Zero, 0, out temp, IntPtr.Zero); }
		public void LockVolume() { int temp; if (!DeviceIoControl(this.deviceHandle, FSCTL_LOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out temp, IntPtr.Zero)) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); } }
		public void UnlockVolume() { int temp; if (!DeviceIoControl(this.deviceHandle, FSCTL_UNLOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out temp, IntPtr.Zero)) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); } }
		protected virtual void Dispose(bool disposing) { if (disposing) { try { if (!this.leaveDeviceOpen) { this.deviceHandle.Close(); } } finally { this.deviceHandle = null; } } }

		public bool UseDirectIO { get; set; }

		//[DebuggerHidden]
		public ScsiStatus ExecuteCommand(BufferWithSize cdb, DataTransferDirection direction, byte pathId, byte targetId, byte logicalUnitNumber, BufferWithSize data, uint timeout, bool autoSense, out SenseData senseData)
		{
			unsafe
			{
				ScsiPassThroughDirectWithSenseBuffer* pSPTD;

				var bufferSize = ScsiPassThroughDirectWithSenseBuffer.DEFAULT_SIZE_WITH_SENSE_BUFFER + (this.UseDirectIO ? 0 : data.Length32);

				bool stackAlloc = Marshaler.ShouldStackAlloc(bufferSize);

				fixed (byte* pBufferFixedBefore = this.buffer)
				{
					if (stackAlloc)
					{
						byte* pBuffer = stackalloc byte[bufferSize];
						pSPTD = (ScsiPassThroughDirectWithSenseBuffer*)pBuffer;
					}
					else
					{
						if (this.buffer == null || this.buffer.Length < bufferSize) { Array.Resize(ref this.buffer, bufferSize); }

						//IGNORE THIS ASSIGNMENT!!! It's just to make the compiler happy!!
						//pSPTD is really set inside the NEXT fixed block!
						pSPTD = null;
					}

					//These two fixed pointers very well point to different places;
					//In fact, either or both could be null.
					//However, this code is optimized for performance and minimizes heap allocations.
					fixed (byte* pBufferFixedAfter = this.buffer)
					{
						if (!stackAlloc) { pSPTD = (ScsiPassThroughDirectWithSenseBuffer*)pBufferFixedAfter; }

						*pSPTD = new ScsiPassThroughDirectWithSenseBuffer()
						{
							Struct = new ScsiPassThroughDirect()
							{
								Length = (ushort)ScsiPassThroughDirect.ORIGINAL_SIZE,
								ScsiStatus = ~(ScsiStatus)0,
								SenseInfoLength = autoSense ? (byte)ScsiPassThroughDirectWithSenseBuffer.DEFAULT_SENSE_SIZE : (byte)0,
								DataIn = direction,
								PathId = pathId,
								LogicalUnitNumber = logicalUnitNumber,
								TargetId = targetId,
								TimeoutValue = timeout,
								CdbLength = (byte)cdb.Length,
								DataTransferBlockCount = data.LengthU32,
							}
						};
						pSPTD->Struct.SenseInfoOffset = autoSense ? (uint)ScsiPassThroughDirectWithSenseBuffer.SENSE_BUFFER_OFFSET : (byte)0;
						BufferWithSize.Copy(cdb, 0, new BufferWithSize(pSPTD->Struct.Cdb, pSPTD->Struct.CdbLength), 0, cdb.Length32);

						if (this.UseDirectIO) { pSPTD->Struct.DataBuffer = data.Address; }
						else
						{
							//It's not a buffer, it's an offset now
							pSPTD->Struct.DataBuffer = (IntPtr)Math.Max(ScsiPassThroughDirectWithSenseBuffer.SENSE_BUFFER_OFFSET, pSPTD->Struct.SenseInfoOffset + pSPTD->Struct.SenseInfoLength);
							if (direction == DataTransferDirection.SendData)
							{ BufferWithSize.Copy(data, UIntPtr.Zero, new BufferWithSize((byte*)pSPTD + (int)pSPTD->Struct.DataBuffer, pSPTD->Struct.DataTransferBlockCount), UIntPtr.Zero, data.Length); }
						}

						int bytesReturned;
						bool success;
						Interlocked.Increment(ref this.passThroughCounter);
						try { success = DeviceIoControl(this.deviceHandle, this.UseDirectIO ? IOCTL_SCSI_PASS_THROUGH_DIRECT : IOCTL_SCSI_PASS_THROUGH, (IntPtr)pSPTD, bufferSize, (IntPtr)pSPTD, bufferSize, out bytesReturned, IntPtr.Zero); }
						finally { Interlocked.Decrement(ref this.passThroughCounter); }
						if (!success)
						{
							try { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
							catch (COMException ex)
							{
								if (ex.ErrorCode == unchecked((int)0x8007051A))
								{ ex = new COMException(string.Format("{0}" + Environment.NewLine + "Length: {1:N0}", ex.Message, pSPTD->Struct.Length), ex); }
								throw ex;
							}
						}

						if (!this.UseDirectIO && direction == DataTransferDirection.ReceiveData)
						{ BufferWithSize.Copy(new BufferWithSize((byte*)pSPTD + (int)pSPTD->Struct.DataBuffer, pSPTD->Struct.DataTransferBlockCount), 0, data, 0, pSPTD->Struct.DataTransferBlockCount); }

						senseData = pSPTD->Struct.SenseInfoLength > 0 ? Marshaler.PtrToStructure<SenseData>(new BufferWithSize((byte*)pSPTD + pSPTD->Struct.SenseInfoOffset, /*pSPTD->SenseInfoLength*/bufferSize - pSPTD->Struct.SenseInfoOffset)) : new SenseData();
						return pSPTD->Struct.ScsiStatus;
					}
				}
			}
		}

		//Returns a set (Bus ID, Device Info[]) of pairs
		public KeyValuePair<byte, KeyValuePair<ScsiDeviceInfo, StandardInquiryData>[]>[] GetScsiInquiryData()
		{
			unsafe
			{
				const int ERROR_INSUFFICIENT_BUFFER = 122;
				const int ERROR_MORE_DATA = 234;
				int bytesReturned;
				int bufferSize = Marshaler.DefaultSizeOf<ScsiInquiryData>() + Marshaler.DefaultSizeOf<StandardInquiryData>();
				byte* pBuffer;
				int lastError;
				do
				{
					bufferSize <<= 1;
					{ byte* pBuffer2 = stackalloc byte[bufferSize]; pBuffer = pBuffer2; }
					if (!DeviceIoControl(this.deviceHandle, IOCTL_SCSI_GET_INQUIRY_DATA, IntPtr.Zero, 0, (IntPtr)pBuffer, bufferSize, out bytesReturned, IntPtr.Zero))
					{ lastError = Marshal.GetLastWin32Error(); }
					else { lastError = 0; }
				} while (lastError == ERROR_INSUFFICIENT_BUFFER | lastError == ERROR_MORE_DATA);
				if (lastError != 0) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }

				var buffer = new BufferWithSize(pBuffer, bytesReturned);
				var adapterBuses = Marshaler.PtrToStructure<ScsiAdapterBusInfo>(buffer);
				var result = new KeyValuePair<byte, KeyValuePair<ScsiDeviceInfo, StandardInquiryData>[]>[adapterBuses.BusData.Length];
				for (int i = 0; i < adapterBuses.BusData.Length; i++)
				{
					var data = adapterBuses.BusData[i];

					var inqs = new KeyValuePair<ScsiDeviceInfo, StandardInquiryData>[data.NumberOfLogicalUnits];
					int inqDataOffset = data.InquiryDataOffset;
					for (int j = 0; j < inqs.Length; j++)
					{
						var inq = Marshaler.PtrToStructure<ScsiInquiryData>(buffer.ExtractSegment(inqDataOffset));
						{
							var inqData = inq.InquiryData;
							int logicalLength;
							unsafe { fixed (byte* pData = inqData) { logicalLength = (int)StandardInquiryData.ADDITIONAL_LENGTH_OFFSET + sizeof(byte) + StandardInquiryData.ReadAdditionalLength(new BufferWithSize(pData, inqData.Length)); } }
							if (inqData.Length < logicalLength) { Array.Resize(ref inqData, logicalLength); inq.InquiryData = inqData; }
						}
						var stdInq = Marshaler.PtrToStructure<StandardInquiryData>(inq.InquiryData, 0);
						inqs[j] = new KeyValuePair<ScsiDeviceInfo, StandardInquiryData>(inq.BusInfo, stdInq);
						inqDataOffset = inq.NextInquiryDataOffset;
					}
					result[i] = new KeyValuePair<byte, KeyValuePair<ScsiDeviceInfo, StandardInquiryData>[]>(data.InitiatorBusId, inqs);
				}

				return result;
			}
		}

		public StandardInquiryData ScsiInquiry(bool throwIfNotFound)
		{
			var myAddress = this.Address;
			var data = this.GetScsiInquiryData();
			for (int i = 0; i < data.Length; i++)
			{
				for (int j = 0; j < data[i].Value.Length; j++)
				{
					var pair = data[i].Value[j];
					if (myAddress.PathId == pair.Key.PathId && myAddress.TargetId == pair.Key.TargetId && myAddress.Lun == pair.Key.Lun)
					{ return pair.Value; }
				}
			}
			if (throwIfNotFound)
			{
				throw new InvalidOperationException(string.Format("Could not find device {{{0}}} among the following:" + Environment.NewLine + "{1}", myAddress, string.Join(", ", Array.ConvertAll(data, d =>
				{
					var result = new string[d.Value.Length];
					for (int i = 0; i < d.Value.Length; i++)
					{ result[i] = string.Format("{0} {1} {2} ({3}) {{{4}}}", VendorIdentifiers.GetVendorName(d.Value[i].Value.VendorIdentification), d.Value[i].Value.ProductIdentification, d.Value[i].Value.ProductRevisionLevel, d.Value[i].Value.PeripheralDeviceType, new ScsiAddress(myAddress.PortNumber, d.Value[i].Key.PathId, d.Value[i].Key.TargetId, d.Value[i].Key.Lun)); }
					return string.Format("• {0}", string.Join(Environment.NewLine, result));
				}))));
			}
			else { return null; }
		}

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern unsafe bool DeviceIoControl([In] SafeFileHandle hDevice, [In] int dwIoControlCode, [In] IntPtr lpInBuffer, [In] int nInBufferSize, [Out] IntPtr lpOutBuffer, [In] int nOutBufferSize, [Out] out int lpBytesReturned, [In] IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern void GetSystemInfo(out SystemInfo lpSystemInfo);

		public StandardInquiryData CdromInquiry()
		{
			unsafe
			{
				int bytesReturned;
				int bufferSize = (Marshaler.DefaultSizeOf<StandardInquiryData>() + 1) >> 1;
				byte* pBuffer = stackalloc byte[bufferSize];
				bool success;
				do
				{
					bufferSize <<= 1;
					byte* pBuf2 = stackalloc byte[bufferSize];
					pBuffer = pBuf2;
					success = DeviceIoControl(this.deviceHandle, IOCTL_CDROM_GET_INQUIRY_DATA, IntPtr.Zero, 0, (IntPtr)pBuffer, bufferSize, out bytesReturned, IntPtr.Zero);
				} while (!success && (Marshal.GetLastWin32Error() == 24 | Marshal.GetLastWin32Error() == 122));
				if (!success) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
				return Marshaler.PtrToStructure<StandardInquiryData>(new BufferWithSize(pBuffer, bufferSize));
			}
		}

		public Multimedia.FeatureCollection GetCdromConfiguration(Multimedia.FeatureCode startingFeature, Multimedia.FeatureRequestType requestType)
		{
			unsafe
			{
				var input = new GetConfigurationIOCtlInput() { Feature = startingFeature, RequestType = (int)requestType };
				int bytesReturned;
				int bufferSize = Marshaler.DefaultSizeOf<Multimedia.FeatureHeader>();
				byte* pBuffer = stackalloc byte[bufferSize];
				bool success;
				var buffer = new BufferWithSize(pBuffer, bufferSize);
				do
				{
					bufferSize <<= 1;
					byte* pBuf2 = stackalloc byte[bufferSize];
					pBuffer = pBuf2;
					buffer = new BufferWithSize(pBuffer, bufferSize);
					success = DeviceIoControl(this.deviceHandle, IOCTL_CDROM_GET_CONFIGURATION, (IntPtr)(&input), Marshaler.SizeOf(input), (IntPtr)pBuffer, bufferSize, out bytesReturned, IntPtr.Zero);
				} while ((!success && (Marshal.GetLastWin32Error() == 24 | Marshal.GetLastWin32Error() == 122)) ||
					(success && Marshaler.DefaultSizeOf<Multimedia.FeatureHeader>() + Marshaler.PtrToStructure<Multimedia.FeatureHeader>(buffer).DataLength > bufferSize));
				if (!success) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
				return Multimedia.FeatureCollection.FromBuffer(buffer);
			}
		}

		public TFeature GetCdromConfiguration<TFeature>()
			where TFeature : Multimedia.MultimediaFeature
		{
			var result = Objects.CreateInstance<TFeature>();
			unsafe
			{
				var input = new GetConfigurationIOCtlInput() { Feature = result.FeatureCode, RequestType = (int)Multimedia.FeatureRequestType.OneFeatureHeaderAndZeroOrOneDescriptor };
				int bytesReturned;
				int bufferSize = Marshaler.DefaultSizeOf<Multimedia.FeatureHeader>();
				byte* pBuffer = stackalloc byte[bufferSize];
				bool success;
				var buffer = new BufferWithSize(pBuffer, bufferSize);
				do
				{
					bufferSize <<= 1;
					byte* pBuf2 = stackalloc byte[bufferSize];
					pBuffer = pBuf2;
					buffer = new BufferWithSize(pBuffer, bufferSize);
					success = DeviceIoControl(this.deviceHandle, IOCTL_CDROM_GET_CONFIGURATION, (IntPtr)(&input), Marshaler.SizeOf(input), (IntPtr)pBuffer, bufferSize, out bytesReturned, IntPtr.Zero);
				} while ((!success && (Marshal.GetLastWin32Error() == 24 | Marshal.GetLastWin32Error() == 122)) ||
					(success && Marshaler.DefaultSizeOf<Multimedia.FeatureHeader>() + Marshaler.PtrToStructure<Multimedia.FeatureHeader>(buffer).DataLength > bufferSize));
				if (!success) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
				var newBuf = buffer.ExtractSegment(Marshaler.DefaultSizeOf<Multimedia.FeatureHeader>());
				if (newBuf.LengthU32 > 0 && Multimedia.MultimediaFeature.ReadFeatureCode(newBuf) == result.FeatureCode)
				{
					Marshaler.PtrToStructure(newBuf, ref result);
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public Multimedia.MultimediaProfile CurrentCdromProfile
		{
			get
			{
				var input = new GetConfigurationIOCtlInput() { Feature = 0, RequestType = (int)Multimedia.FeatureRequestType.OneFeatureHeaderAndZeroOrOneDescriptor };
				int bytesReturned;
				int bufferSize = Marshaler.DefaultSizeOf<Multimedia.FeatureHeader>() + Marshaler.DefaultSizeOf<Scsi.Multimedia.MultimediaFeature>();
				unsafe
				{
					byte* pBuffer = stackalloc byte[bufferSize];
					var buffer = new BufferWithSize(pBuffer, bufferSize);
					if (!DeviceIoControl(this.deviceHandle, IOCTL_CDROM_GET_CONFIGURATION, (IntPtr)(&input), Marshaler.SizeOf(input), (IntPtr)pBuffer, bufferSize, out bytesReturned, IntPtr.Zero)) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
					return Marshaler.PtrToStructure<Multimedia.FeatureHeader>(buffer).CurrentProfile;
				}
			}
		}

		public int MaximumDataTransferBlockCount
		{
			get
			{
				if (this._MaximumDataTransferBlockCount == 0)
				{
					unsafe
					{
						//How to determine the maximum transfer length:
						//The MaximumTotalTransferBlockCount property specifies the maximum SCSI Request Block size, so to get the
						// maximum buffer size, we have to subtract the structure length itself.
						//However, the MaximumTotalPhysicalPages size is more tricky: It's the maximum number of discontiguous pages.
						// We need to subtract one from it, because our data is not necessarily aligned and can take an extra page.
						//I'm just being safe here and taking the Min() of both, but I don't know which one is exactly correct.
						SystemInfo sysInfo;
						GetSystemInfo(out sysInfo);
						var result = (this.MaximumTotalTransferBlockCount - sizeof(ScsiRequestBlock) / sysInfo.PageSize) * sysInfo.PageSize;
						result = Math.Min(result, this.MaximumTotalPhysicalPages - 1);
						this._MaximumDataTransferBlockCount = result; //Round it to page size
					}
				}
				return this._MaximumDataTransferBlockCount;
			}
		}

		public void ResetDevice()
		{
			unsafe
			{
				int temp;
				if (!DeviceIoControl(this.deviceHandle, IOCTL_STORAGE_RESET_DEVICE, IntPtr.Zero, 0, IntPtr.Zero, 0, out temp, IntPtr.Zero))
				{ ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
			}
		}

		public void ResetBus(byte pathId)
		{
			unsafe
			{
				int temp;
				if (!DeviceIoControl(this.deviceHandle, IOCTL_STORAGE_RESET_BUS, (IntPtr)(&pathId), sizeof(byte), IntPtr.Zero, 0, out temp, IntPtr.Zero))
				{ ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
			}
		}

		private ScsiAddress Address
		{
			get
			{
				ScsiAddress address;
				unsafe
				{
					int bytesReturned;
					if (!DeviceIoControl(this.deviceHandle, IOCTL_SCSI_GET_ADDRESS, IntPtr.Zero, 0, (IntPtr)(&address), Marshaler.DefaultSizeOf<ScsiAddress>(), out bytesReturned, IntPtr.Zero))
					{ ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
				}
				if (address.Length < Marshaler.DefaultSizeOf<ScsiAddress>()) { throw new InvalidOperationException("The device did not return as much information as required."); }
				return address;
			}
		}

		public DiskGeometry[] GetMediaTypes()
		{
			unsafe
			{
				int bytesReturned;
				int bufferSize = Marshaler.DefaultSizeOf<DiskGeometry>();
				byte* pBuffer = stackalloc byte[bufferSize];
				bool success;
				do
				{
					bufferSize <<= 1;
					byte* pBuf2 = stackalloc byte[bufferSize];
					pBuffer = pBuf2;
					success = DeviceIoControl(this.deviceHandle, IOCTL_STORAGE_GET_MEDIA_TYPES, IntPtr.Zero, 0, (IntPtr)pBuffer, bufferSize, out bytesReturned, IntPtr.Zero);
				} while (!success && (Marshal.GetLastWin32Error() == 24 | Marshal.GetLastWin32Error() == 122));
				if (!success) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }

				var items = new DiskGeometry[bytesReturned / Marshaler.DefaultSizeOf<DiskGeometry>()];
				var pGeom = (DiskGeometry*)pBuffer;
				for (int i = 0; i < items.Length; i++) { items[i] = pGeom[i]; }
				return items;
			}
		}

		public DiskDeviceMediaInfo[] GetMediaTypesEx(out Win32DeviceType deviceType)
		{
			unsafe
			{
				int bytesReturned;
				int bufferSize = Marshaler.DefaultSizeOf<DeviceMediaTypes>();
				byte* pBuffer = stackalloc byte[bufferSize];
				bool success;
				do
				{
					bufferSize <<= 1;
					byte* pBuf2 = stackalloc byte[bufferSize];
					pBuffer = pBuf2;
					success = DeviceIoControl(this.deviceHandle, IOCTL_STORAGE_GET_MEDIA_TYPES_EX, IntPtr.Zero, 0, (IntPtr)pBuffer, bufferSize, out bytesReturned, IntPtr.Zero);
				} while (!success && (Marshal.GetLastWin32Error() == 24 | Marshal.GetLastWin32Error() == 122));
				if (!success) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
				var result = Marshaler.PtrToStructure<DeviceMediaTypes>(new BufferWithSize(pBuffer, bytesReturned));
				deviceType = result.DeviceType;
				return result.MediaInfo;
			}
		}

		public byte PortNumber { get { return this.Address.PortNumber; } }
		public byte PathId { get { return this.Address.PathId; } }
		public byte TargetId { get { return this.Address.TargetId; } }
		public byte LogicalUnitNumber { get { return this.Address.Lun; } }

		private IOScsiCapabilities Capabilities { get { IOScsiCapabilities result; int temp; unsafe { if (!DeviceIoControl(this.deviceHandle, IOCTL_SCSI_GET_CAPABILITIES, IntPtr.Zero, 0, (IntPtr)(&result), Marshaler.DefaultSizeOf<IOScsiCapabilities>(), out temp, IntPtr.Zero)) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); } } return result; } }

		public void McnControl(bool disableMediaChangeNotifications) { int temp; unsafe { if (!DeviceIoControl(this.deviceHandle, IOCTL_STORAGE_MCN_CONTROL, (IntPtr)(&disableMediaChangeNotifications), sizeof(bool), IntPtr.Zero, 0, out temp, IntPtr.Zero)) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); } } }

		public int MaximumTotalTransferBlockCount { get { return this.Capabilities.MaximumTransferBlockCount; } }
		public int MaximumTotalPhysicalPages { get { return this.Capabilities.MaximumPhysicalPages; } }
		public int SupportedAsynchronousEvents { get { return this.Capabilities.SupportedAsynchronousEvents; } }
		public int AlignmentMask { get { if (this._Alignment == -1) { this._Alignment = this.Capabilities.AlignmentMask; } return this._Alignment; } }
		public bool TaggedQueuing { get { return this.Capabilities.TaggedQueuing; } }
		public bool AdapterScansDown { get { return this.Capabilities.AdapterScansDown; } }
		public bool AdapterUsesProgrammedIO { get { return this.Capabilities.AdapterUsesPio; } }

		public StandardInquiryData QueryStorageInquiryData()
		{
			var query = new StoragePropertyQuery() { PropertyId = StoragePropertyId.StorageDeviceProperty, QueryType = StorageQueryType.PropertyStandardQuery };
			unsafe
			{
				int bytesReturned;
				int bufferSize = Marshaler.DefaultSizeOf<StorageDescriptorHeader>();
				byte* pBuffer = stackalloc byte[bufferSize];
				bool success;
				do
				{
					bufferSize <<= 1;
					byte* pBuf2 = stackalloc byte[bufferSize];
					pBuffer = pBuf2;
					success = DeviceIoControl(this.deviceHandle, IOCTL_STORAGE_QUERY_PROPERTY, (IntPtr)(&query), Marshaler.SizeOf(query), (IntPtr)pBuffer, bufferSize, out bytesReturned, IntPtr.Zero);
				} while (!success && (Marshal.GetLastWin32Error() == 24 | Marshal.GetLastWin32Error() == 122));
				if (!success) { ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }
				var pDescriptor = (StorageDeviceDescriptor*)pBuffer;
				throw new NotImplementedException();
				//return Marshaler.PtrToStructure<StandardInquiryData>(new BufferWithSize((IntPtr)pDescriptor->RawDeviceProperties, pDescriptor->RawPropertiesLength));
			}
		}

		#region Structures
		private struct GetConfigurationIOCtlInput
		{
			public Multimedia.FeatureCode Feature;
			public int RequestType;
			public IntPtr Reserved1;
			public IntPtr Reserved2;
		}

		private struct IOScsiCapabilities
		{
			public int Length;
			public int MaximumTransferBlockCount;
			public int MaximumPhysicalPages;
			public int SupportedAsynchronousEvents;
			public int AlignmentMask;
			[MarshalAs(UnmanagedType.I1)]
			public bool TaggedQueuing;
			[MarshalAs(UnmanagedType.I1)]
			public bool AdapterScansDown;
			[MarshalAs(UnmanagedType.I1)]
			public bool AdapterUsesPio;
		}

		private struct ScsiRequestBlock
		{
			//We don't actually use this structure --
			//  I just defined it so I can take its size dynamically, instead of hard-coding a value (which could fail on x64)
#pragma warning disable 0649
			public ushort Length;
			public byte Function;
			public byte SrbStatus;
			public byte ScsiStatus;
			public byte PathId;
			public byte TargetId;
			public byte Lun;
			public byte QueueTag;
			public byte QueueAction;
			public byte CdbLength;
			public byte SenseInfoBufferLength;
			public int SrbFlags;
			public int DataTransferBlockCount;
			public int TimeOutValue;
			public IntPtr DataBuffer;
			public IntPtr SenseInfoBuffer;
			public unsafe ScsiRequestBlock* NextSrb;
			public IntPtr OriginalRequest;
			public IntPtr SrbExtension;
			public int QueueSortKey;
			public unsafe fixed byte Cdb[16];
#pragma warning restore 0649
		}

		private struct ScsiAddress
		{
			[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
			private static readonly IntPtr PORT_NUMBER_OFFSET = Marshal.OffsetOf(typeof(ScsiAddress), "PortNumber");
			[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
			private static readonly IntPtr PATH_ID_OFFSET = Marshal.OffsetOf(typeof(ScsiAddress), "PathId");
			[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
			private static readonly IntPtr TARGET_ID_OFFSET = Marshal.OffsetOf(typeof(ScsiAddress), "TargetId");
			[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
			private static readonly IntPtr LUN_OFFSET = Marshal.OffsetOf(typeof(ScsiAddress), "Lun");

			public ScsiAddress(byte port, byte path, byte target, byte lun)
			{
				this.Length = Marshaler.DefaultSizeOf<ScsiAddress>();
				this.PortNumber = port;
				this.PathId = path;
				this.TargetId = target;
				this.Lun = lun;
			}

			public int Length;
			public byte PortNumber;
			public byte PathId /*or Bus#*/;
			public byte TargetId;
			public byte Lun;

			public override string ToString() { return string.Format("Port {0:N0}, Path ID {1:N0}, Target ID {2:N0}, LUN {3:N0}", this.PortNumber, this.PathId, this.TargetId, this.Lun); }
		}

		private struct ScsiAdapterBusInfo : IMarshalable
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private static readonly IntPtr NUMBER_OF_BUSES_OFFSET = Marshal.OffsetOf(typeof(ScsiAdapterBusInfo), "NumberOfBuses");
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private static readonly IntPtr BUS_DATA_OFFSET = Marshal.OffsetOf(typeof(ScsiAdapterBusInfo), "_BusData");

			private byte NumberOfBuses;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
			private ScsiBusData[] _BusData;
			public ScsiBusData[] BusData { get { return this._BusData; } set { this._BusData = value; this.NumberOfBuses = (byte)value.Length; } }

			public void MarshalFrom(BufferWithSize buffer)
			{
				this.NumberOfBuses = buffer.Read<byte>(NUMBER_OF_BUSES_OFFSET);
				this._BusData = new ScsiBusData[this.NumberOfBuses];
				var remainingBusBuffer = buffer.ExtractSegment(BUS_DATA_OFFSET);
				for (int i = 0; i < this._BusData.Length; i++)
				{
					this._BusData[i] = Marshaler.PtrToStructure<ScsiBusData>(remainingBusBuffer);
					remainingBusBuffer = remainingBusBuffer.ExtractSegment(Marshaler.SizeOf(this._BusData[i]));
				}
			}

			public void MarshalTo(BufferWithSize buffer)
			{
				buffer.Write(this.NumberOfBuses, NUMBER_OF_BUSES_OFFSET);
				var remainingBusBuffer = buffer.ExtractSegment(BUS_DATA_OFFSET);
				for (int i = 0; i < this._BusData.Length; i++)
				{
					Marshaler.StructureToPtr(this._BusData[i], remainingBusBuffer);
					remainingBusBuffer = remainingBusBuffer.ExtractSegment(Marshaler.SizeOf(this._BusData[i]));
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public int MarshaledSize { get { return Marshaler.DefaultSizeOf<ScsiAdapterBusInfo>() + (Marshaler.DefaultSizeOf<ScsiBusData>() * ((int)this.NumberOfBuses - 1)); } }
		}

#pragma warning disable 0649
		private struct ScsiBusData { public byte NumberOfLogicalUnits; public byte InitiatorBusId; public int InquiryDataOffset; }
#pragma warning restore 0649

		[StructLayout(LayoutKind.Sequential)]
		private struct ScsiPassThroughDirect
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			internal static readonly int ORIGINAL_SIZE = Marshaler.DefaultSizeOf<ScsiPassThroughDirect>();
			public ushort Length;
			public ScsiStatus ScsiStatus;
			public byte PathId;
			public byte TargetId;
			public byte LogicalUnitNumber;
			public byte CdbLength;
			public byte SenseInfoLength;
			public DataTransferDirection DataIn;
			public uint DataTransferBlockCount;
			public uint TimeoutValue;
			public IntPtr DataBuffer;
			public uint SenseInfoOffset;
			public unsafe fixed byte Cdb[16];
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct ScsiPassThroughDirectWithSenseBuffer
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public const int DEFAULT_SENSE_SIZE = 64;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			internal static readonly int DEFAULT_SIZE_WITH_SENSE_BUFFER = Marshaler.DefaultSizeOf<ScsiPassThroughDirectWithSenseBuffer>();
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			internal static readonly int SENSE_BUFFER_OFFSET = (int)Marshal.OffsetOf(typeof(ScsiPassThroughDirectWithSenseBuffer), "SenseBuffer");
			public ScsiPassThroughDirect Struct;
			public unsafe fixed byte SenseBuffer[DEFAULT_SENSE_SIZE];
		}

		private struct SystemInfo { public int OemId; public int PageSize; public IntPtr MinimumApplicationAddress; public IntPtr MaximumApplicationAddress; public IntPtr ActiveProcessorMask; public int NumberOfProcessors; public int ProcessorType; public int AllocationGranularity; public short ProcessorLevel; public short ProcessorRevision; }

		private struct DeviceMediaTypes : IMarshalable
		{
			public Win32DeviceType DeviceType;
			public int MediaInfoCount;
			public DiskDeviceMediaInfo[] MediaInfo;

			public void MarshalFrom(BufferWithSize buffer)
			{
				this.DeviceType = buffer.Read<Win32DeviceType>(0);
				this.MediaInfoCount = buffer.Read<int>(sizeof(Win32DeviceType));
				this.MediaInfo = new DiskDeviceMediaInfo[this.MediaInfoCount];

				var infosBuffer = buffer.ExtractSegment(sizeof(Win32DeviceType) + sizeof(int));
				for (int i = 0; i < this.MediaInfoCount; i++)
				{ this.MediaInfo[i] = Marshaler.PtrToStructure<DiskDeviceMediaInfo>(infosBuffer.ExtractSegment(i * Marshaler.DefaultSizeOf<DiskDeviceMediaInfo>(), Marshaler.DefaultSizeOf<DiskDeviceMediaInfo>())); }
			}

			public void MarshalTo(BufferWithSize buffer) { throw new NotImplementedException(); }
			public int MarshaledSize { get { throw new NotImplementedException(); } }
		}

		public struct ScsiInquiryData : IMarshalable
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private static readonly IntPtr BUS_INFO_OFFSET = Marshal.OffsetOf(typeof(ScsiInquiryData), "_BusInfo");
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private static readonly IntPtr INQUIRY_DATA_LENGTH_OFFSET = Marshal.OffsetOf(typeof(ScsiInquiryData), "InquiryDataLength");
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private static readonly IntPtr NEXT_INQUIRY_DATA_OFFSET_OFFSET = Marshal.OffsetOf(typeof(ScsiInquiryData), "NextInquiryDataOffset");
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private static readonly IntPtr INQUIRY_DATA_OFFSET = Marshal.OffsetOf(typeof(ScsiInquiryData), "_InquiryData");

			[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
			private ScsiDeviceInfo _BusInfo;
			public ScsiDeviceInfo BusInfo { get { return this._BusInfo; } set { this._BusInfo = value; } }
			private int InquiryDataLength;
			public int NextInquiryDataOffset;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
			private byte[] _InquiryData;
			public byte[] InquiryData { get { return this._InquiryData; } set { this._InquiryData = value; this.InquiryDataLength = value != null ? value.Length : 0; this.NextInquiryDataOffset = this.MarshaledSize; } }

			public void MarshalFrom(BufferWithSize buffer)
			{
				this._BusInfo = buffer.Read<ScsiDeviceInfo>(BUS_INFO_OFFSET);
				this.InquiryDataLength = buffer.Read<int>(INQUIRY_DATA_LENGTH_OFFSET);
				this.NextInquiryDataOffset = buffer.Read<int>(NEXT_INQUIRY_DATA_OFFSET_OFFSET);
				this._InquiryData = new byte[this.InquiryDataLength];
				buffer.CopyTo((int)INQUIRY_DATA_OFFSET, this._InquiryData, 0, this._InquiryData.Length);
			}

			public void MarshalTo(BufferWithSize buffer)
			{
				buffer.Write(this._BusInfo, BUS_INFO_OFFSET);
				buffer.Write(this.InquiryDataLength, INQUIRY_DATA_LENGTH_OFFSET);
				buffer.Write(this.NextInquiryDataOffset, NEXT_INQUIRY_DATA_OFFSET_OFFSET);
				if (this._InquiryData.Length > 1) { throw new OverflowException("Field is too large."); }
				buffer.CopyFrom((int)INQUIRY_DATA_OFFSET, this._InquiryData, 0, this._InquiryData.Length);
				buffer.Initialize((int)INQUIRY_DATA_OFFSET + this._InquiryData.Length, 1 - this._InquiryData.Length);
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public int MarshaledSize { get { return Marshaler.DefaultSizeOf<ScsiInquiryData>() + this.InquiryDataLength; } }
		}

		private struct StoragePropertyQuery
		{
			public StoragePropertyId PropertyId;
			public StorageQueryType QueryType;
			public unsafe fixed byte AdditionalParameters[1];
		}

		private enum StoragePropertyId
		{
			StorageDeviceProperty = 0,
			StorageAccessAlignmentProperty,
			StorageAdapterProperty,
			StorageDeviceIdProperty,
			StorageDeviceUniqueIdProperty,
			StorageDeviceWriteCacheProperty
		}

		public enum StorageQueryType
		{
			PropertyStandardQuery = 0,
			PropertyIncludeSwIds,
			PropertyExistsQuery,
			PropertyMaskQuery,
			PropertyQueryMaxDefined
		}

		public struct StorageDescriptorHeader
		{
			public int Version;
			public int Size;
		}

		public struct StorageDeviceDescriptor
		{
			public StorageDescriptorHeader Header;
			public byte DeviceType;
			public byte DeviceTypeModifier;
			[MarshalAs(UnmanagedType.I1)]
			public bool RemovableMedia;
			[MarshalAs(UnmanagedType.I1)]
			public bool CommandQueueing;
			public int VendorIdOffset;
			public int ProductIdOffset;
			public int ProductRevisionOffset;
			public int SerialNumberOffset;
			public StorageBusType BusType;
			public int RawPropertiesLength;
			public unsafe fixed byte RawDeviceProperties[1];
		}

		public enum StorageBusType
		{
			BusTypeUnknown = 0x00,
			BusTypeScsi,
			BusTypeAtapi,
			BusTypeAta,
			BusType1394,
			BusTypeSsa,
			BusTypeFibre,
			BusTypeUsb,
			BusTypeRAID,
			BusTypeiScsi,
			BusTypeSas,
			BusTypeSata,
			BusTypeSd,
			BusTypeMmc,
			BusTypeVirtual,
			BusTypeFileBackedVirtual,
			BusTypeMax,
			BusTypeMaxReserved = 0x7F
		}
		#endregion
	}
}