using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DeviceBusyEvent
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte EVENT_CODE_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		public DeviceBusyEventCode DeviceBusyEventCode
		{
			get { return (DeviceBusyEventCode)Bits.GetValueMask(this.byte0, 0, EVENT_CODE_MASK); }
			set { this.byte0 = Bits.PutValueMask(this.byte0, (byte)value, 0, EVENT_CODE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private DeviceBusyStatusCode _DeviceBusyStatusCode;
		public DeviceBusyStatusCode DeviceBusyStatusCode { get { return this._DeviceBusyStatusCode; } set { this._DeviceBusyStatusCode = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _Time;
		/// <summary>The predicted amount of time remaining for the drive to become not busy, in units of 100 ms. This is not valid if the device is not busy.</summary>
		public ushort Time { get { return Bits.BigEndian(this._Time); } set { this._Time = Bits.BigEndian(value); } }
		/// <summary>The predicted amount of time remaining for the drive to become not busy. This is not valid if the device is not busy.</summary>
		public TimeSpan TimeSpan { get { return System.TimeSpan.FromMilliseconds(this.Time * 100); } set { this.Time = (ushort)(value.TotalMilliseconds / 100); ; } }
	}
}