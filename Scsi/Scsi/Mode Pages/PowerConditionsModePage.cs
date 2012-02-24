using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class PowerConditionsModePage : ModePage
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE2_OFFSET = Marshal.OffsetOf(typeof(PowerConditionsModePage), "byte2");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE3_OFFSET = Marshal.OffsetOf(typeof(PowerConditionsModePage), "byte3");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr IDLE_CONDITION_TIMER_OFFSET = Marshal.OffsetOf(typeof(PowerConditionsModePage), "_IdleConditionTimer");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr STANDBY_CONDITION_TIMER_OFFSET = Marshal.OffsetOf(typeof(PowerConditionsModePage), "_StandbyConditionTimer");

		public PowerConditionsModePage() : base(ModePageCode.PowerConditions) { }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte3;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private bool IdleTimerActive { get { return Bits.GetBit(this.byte3, 1); } set { this.byte3 = Bits.SetBit(this.byte3, 1, value); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private bool StandbyTimerActive { get { return Bits.GetBit(this.byte3, 0); } set { this.byte3 = Bits.SetBit(this.byte3, 0, value); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private uint _IdleConditionTimer;
		/// <summary>The inactivity time in 100 millisecond increments that the logical unit waits before transitioning to the idle power condition.</summary>
		public uint? IdleConditionTimer { get { return this.IdleTimerActive ? Bits.BigEndian(this._IdleConditionTimer) : (uint?)null; } set { this.IdleTimerActive = value != null; this._IdleConditionTimer = Bits.BigEndian(value.GetValueOrDefault()); } }
		public TimeSpan? IdleCondition { get { var timer = this.IdleConditionTimer; return TimeSpan.FromMilliseconds(timer.GetValueOrDefault() * 100); } set { this.IdleConditionTimer = value != null ? (uint)(value.Value.TotalMilliseconds / 100) : (uint?)0; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private uint _StandbyConditionTimer;
		/// <summary>The inactivity time in 100 millisecond increments that the logical unit waits before transitioning to the standby power condition.</summary>
		public uint? StandbyConditionTimer { get { return this.StandbyTimerActive ? Bits.BigEndian(this._StandbyConditionTimer) : (uint?)null; } set { this.StandbyTimerActive = value != null; this._StandbyConditionTimer = Bits.BigEndian(value.GetValueOrDefault()); } }
		public TimeSpan? StandbyCondition { get { var timer = this.StandbyConditionTimer; return TimeSpan.FromMilliseconds(timer.GetValueOrDefault() * 100); } set { this.StandbyConditionTimer = value != null ? (uint)(value.Value.TotalMilliseconds / 100) : (uint?)0; } }


		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.byte2 = buffer.Read<byte>(BYTE2_OFFSET);
			this.byte3 = buffer.Read<byte>(BYTE3_OFFSET);
			this._IdleConditionTimer = buffer.Read<uint>(IDLE_CONDITION_TIMER_OFFSET);
			this._StandbyConditionTimer = buffer.Read<uint>(STANDBY_CONDITION_TIMER_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.byte2, BYTE2_OFFSET);
			buffer.Write(this.byte3, BYTE3_OFFSET);
			buffer.Write(this._IdleConditionTimer, IDLE_CONDITION_TIMER_OFFSET);
			buffer.Write(this._StandbyConditionTimer, STANDBY_CONDITION_TIMER_OFFSET);
		}

		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<PowerConditionsModePage>(); } }
	}
}