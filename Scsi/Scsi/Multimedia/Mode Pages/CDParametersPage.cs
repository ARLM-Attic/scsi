using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;
using System;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class CDParametersPage : ModePage
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE2_OFFSET = Marshal.OffsetOf(typeof(CDParametersPage), "byte2");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE3_OFFSET = Marshal.OffsetOf(typeof(CDParametersPage), "byte3");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SECONDS_PER_MINUTE_OFFSET = Marshal.OffsetOf(typeof(CDParametersPage), "_SecondsPerMinute");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr FRAMES_PER_SECOND_OFFSET = Marshal.OffsetOf(typeof(CDParametersPage), "_FramesPerSecond");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte INACTIVITY_TIMER_MULTIPLIER_MASK = 0x0F;
		public CDParametersPage() : base(ModePageCode.CDDeviceParameters) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		public InactivityTimerMultiplierValue InactivityTimerMultiplierValue
		{
			get { return (InactivityTimerMultiplierValue)Bits.GetValueMask(this.byte3, 0, INACTIVITY_TIMER_MULTIPLIER_MASK); }
			set { this.byte3 = Bits.PutValueMask(this.byte3, (byte)value, 0, INACTIVITY_TIMER_MULTIPLIER_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _SecondsPerMinute;
		public ushort SecondsPerMinute { get { return Bits.BigEndian(this._SecondsPerMinute); } set { this._SecondsPerMinute = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _FramesPerSecond;
		public ushort FramesPerSecond { get { return Bits.BigEndian(this._FramesPerSecond); } set { this._FramesPerSecond = Bits.BigEndian(value); } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.byte2 = buffer.Read<byte>(BYTE2_OFFSET);
			this.byte3 = buffer.Read<byte>(BYTE3_OFFSET);
			this._SecondsPerMinute = buffer.Read<ushort>(SECONDS_PER_MINUTE_OFFSET);
			this._FramesPerSecond = buffer.Read<ushort>(FRAMES_PER_SECOND_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.byte2, BYTE2_OFFSET);
			buffer.Write(this.byte3, BYTE3_OFFSET);
			buffer.Write(this._SecondsPerMinute, SECONDS_PER_MINUTE_OFFSET);
			buffer.Write(this._FramesPerSecond, FRAMES_PER_SECOND_OFFSET);
		}

		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<CDParametersPage>(); } }

	}
}