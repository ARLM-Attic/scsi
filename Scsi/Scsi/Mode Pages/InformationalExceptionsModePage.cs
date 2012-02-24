using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;
using System;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class InformationalExceptionsModePage : ModePage
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE2_OFFSET = Marshal.OffsetOf(typeof(InformationalExceptionsModePage), "byte2");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE3_OFFSET = Marshal.OffsetOf(typeof(InformationalExceptionsModePage), "byte3");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr INTERVAL_TIMER_OFFSET = Marshal.OffsetOf(typeof(InformationalExceptionsModePage), "_IntervalTimer");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr REPORT_COUNT_OFFSET = Marshal.OffsetOf(typeof(InformationalExceptionsModePage), "_ReportCount");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte MRIE_MASK = 0x0F;
		public InformationalExceptionsModePage() : base(ModePageCode.InformationalExceptionsModePage) { this.PageLength = (byte)this.MarshaledSize; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		public bool LogErrors { get { return Bits.GetBit(this.byte2, 0); } set { this.byte2 = Bits.SetBit(this.byte2, 0, value); } }
		public bool EnableBackgroundError { get { return Bits.GetBit(this.byte2, 1); } set { this.byte2 = Bits.SetBit(this.byte2, 1, value); } }
		public bool TestDeviceFailure { get { return Bits.GetBit(this.byte2, 2); } set { this.byte2 = Bits.SetBit(this.byte2, 2, value); } }
		public bool DisableExceptionControl { get { return Bits.GetBit(this.byte2, 3); } set { this.byte2 = Bits.SetBit(this.byte2, 3, value); } }
		public bool EnableWarningReporting { get { return Bits.GetBit(this.byte2, 4); } set { this.byte2 = Bits.SetBit(this.byte2, 4, value); } }
		public bool EnableBackgroundFunction { get { return Bits.GetBit(this.byte2, 5); } set { this.byte2 = Bits.SetBit(this.byte2, 5, value); } }
		public bool NoPerformanceImpactAllowed { get { return Bits.GetBit(this.byte2, 7); } set { this.byte2 = Bits.SetBit(this.byte2, 7, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		public InformationalExceptionReportingMethod InformationalExceptionReportingMethod { get { return (InformationalExceptionReportingMethod)Bits.GetValueMask(this.byte3, 0, MRIE_MASK); } set { this.byte2 = Bits.PutValueMask(this.byte3, (byte)value, 0, MRIE_MASK); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private uint _IntervalTimer;
		/// <summary>The period in 100-millisecond increments that the device server uses for reporting that an informational exception condition has occurred.</summary>
		public uint IntervalTimer { get { return Bits.BigEndian(this._IntervalTimer); } set { this._IntervalTimer = Bits.BigEndian(value); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private uint _ReportCount;
		public uint ReportCount { get { return Bits.BigEndian(this._ReportCount); } set { this._ReportCount = Bits.BigEndian(value); } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.byte2 = buffer.Read<byte>(BYTE2_OFFSET);
			this.byte3 = buffer.Read<byte>(BYTE3_OFFSET);
			this._IntervalTimer = buffer.Read<uint>(INTERVAL_TIMER_OFFSET);
			this._ReportCount = buffer.Read<uint>(REPORT_COUNT_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.byte2, BYTE2_OFFSET);
			buffer.Write(this.byte3, BYTE3_OFFSET);
			buffer.Write(this._IntervalTimer, INTERVAL_TIMER_OFFSET);
			buffer.Write(this._ReportCount, REPORT_COUNT_OFFSET);
		}

		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<InformationalExceptionsModePage>(); } }
	}

	public enum InformationalExceptionReportingMethod : byte
	{
		NoReporting = 0x00,
		[Obsolete]
		AsynchronousEventReporting = 0x01,
		EstablishUnitAttentionCondition = 0x02,
		ConditionallyGenerateRecoveredError = 0x03,
		UnconditionallyGenerateRecoveredError = 0x04,
		GenerateNoSense = 0x05,
		OnlyReportOnRequest = 0x06,
	}
}