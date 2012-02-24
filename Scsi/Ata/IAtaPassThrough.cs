using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
namespace Ata
{
	/// <summary>A generic pass-through interface. As long as this can be implemented, the library works.</summary>
	public interface IAtaPassThrough : IDisposable
	{
		void ExecuteCommand(ref AtaTaskFile ataTaskLow, ref AtaTaskFile ataTaskHigh, IntPtr pData, uint dataLength, uint timeout, AtaFlags flags);
		void DismountVolume();
		void LockVolume();
		void UnlockVolume();
	}



	/// <summary>The Windows Ata Pass-Through Interface. This uses a file handle (obtainable through instantiating a <see cref="Helper.IO.Win32FileStream"/> object with read/write access) to send I/O control codes to the underlying Ata device.</summary>
	//[DebuggerStepThrough]
	public class Win32Apti : IAtaPassThrough
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_ATA_PASS_THROUGH_DIRECT = 0x00000004 << 16 | 3 << 14 | 0x040C << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IOCTL_IDE_PASS_THROUGH = 0x00000004 << 16 | 3 << 14 | 0x040A << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FSCTL_LOCK_VOLUME = 0x00000009 << 16 | 0 << 14 | 0x0006 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FSCTL_UNLOCK_VOLUME = 0x00000009 << 16 | 0 << 14 | 0x0007 << 2 | 0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int FSCTL_DISMOUNT_VOLUME = 0x00000009 << 16 | 0 << 14 | 0x0008 << 2 | 0;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool leaveDeviceOpen;
		private SafeFileHandle deviceHandle;

		public Win32Apti(SafeFileHandle deviceHandle, bool leaveDeviceOpen) { this.deviceHandle = deviceHandle; this.leaveDeviceOpen = leaveDeviceOpen; }

		public void Dispose() { this.Dispose(true); GC.SuppressFinalize(this); }

		public void DismountVolume() { int temp; if (DeviceIoControl(this.deviceHandle, FSCTL_DISMOUNT_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out temp, IntPtr.Zero) == 0) { Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); } }

		public void LockVolume() { int temp; if (DeviceIoControl(this.deviceHandle, FSCTL_LOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out temp, IntPtr.Zero) == 0) { Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); } }

		public void UnlockVolume() { int temp; if (DeviceIoControl(this.deviceHandle, FSCTL_UNLOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out temp, IntPtr.Zero) == 0) { Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); } }

		protected virtual void Dispose(bool disposing) { if (disposing) { try { if (!this.leaveDeviceOpen) { this.deviceHandle.Close(); } } finally { this.deviceHandle = null; } } }

		public void ExecuteCommand(ref AtaTaskFile ataTaskLow, ref AtaTaskFile ataTaskHigh, IntPtr pData, uint dataLength, uint timeout, AtaFlags flags)
		{
			unsafe
			{
				if (dataLength != 0 ^ (flags & (AtaFlags.ReceiveData | AtaFlags.SendData)) != 0) { throw new ArgumentOutOfRangeException("flags", flags, "Data transfer invalid (either specify no buffer or indicate direction)."); }

				var aptd = new AtaPassThroughDirect();
				aptd.TimeOutValue = timeout;
				aptd.Length = (ushort)sizeof(AtaPassThroughDirect);
				aptd.PreviousTaskFile = ataTaskHigh;
				aptd.CurrentTaskFile = ataTaskLow;
				aptd.DataBuffer = pData;
				aptd.DataTransferBlockCount = dataLength;
				aptd.AtaFlags = flags;

				int bytesReturned;
				if (DeviceIoControl(this.deviceHandle, IOCTL_ATA_PASS_THROUGH_DIRECT, (IntPtr)(&aptd), aptd.Length, (IntPtr)(&aptd), aptd.Length, out bytesReturned, IntPtr.Zero) == 0) { Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); }

				ataTaskLow = aptd.CurrentTaskFile;
				ataTaskHigh = aptd.PreviousTaskFile;
			}
		}


		[DllImport("Kernel32.dll", SetLastError = true)]
		private static extern unsafe int DeviceIoControl([In] SafeFileHandle hDevice, [In] int dwIoControlCode, [In] IntPtr lpInBuffer, [In] int nInBufferSize, [Out] IntPtr lpOutBuffer, [In] int nOutBufferSize, [Out] out int lpBytesReturned, [In] IntPtr lpOverlapped);

#pragma warning disable 0649
		private struct AtaPassThroughDirect
		{
			public ushort Length;
			public AtaFlags AtaFlags;
			public byte PathId;
			public byte TargetId;
			public byte Lun;
			public byte ReservedAsUchar;
			public uint DataTransferBlockCount;
			public uint TimeOutValue;
			public uint ReservedAsUlong;
			public IntPtr DataBuffer;
			public AtaTaskFile PreviousTaskFile;
			public AtaTaskFile CurrentTaskFile;
		}
#pragma warning restore 0649
	}
}