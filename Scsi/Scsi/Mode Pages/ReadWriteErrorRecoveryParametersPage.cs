using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;
using System;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ReadWriteErrorRecoveryParametersPage : ModePage
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr ERROR_RECOVERY_PARAMETER_OFFSET = Marshal.OffsetOf(typeof(ReadWriteErrorRecoveryParametersPage), "errorRecoveryParameter");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr READ_RETRY_COUNT_OFFSET = Marshal.OffsetOf(typeof(ReadWriteErrorRecoveryParametersPage), "_ReadRetryCount");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr CORRECTION_SPAN_OFFSET = Marshal.OffsetOf(typeof(ReadWriteErrorRecoveryParametersPage), "_CorrectionSpan");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr HEAD_OFFSET_COUNT_OFFSET = Marshal.OffsetOf(typeof(ReadWriteErrorRecoveryParametersPage), "_HeadOffsetCount");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr DATA_STROBE_OFFSET_COUNT_OFFSET = Marshal.OffsetOf(typeof(ReadWriteErrorRecoveryParametersPage), "_DataStrobeOffsetCount");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE7_OFFSET = Marshal.OffsetOf(typeof(ReadWriteErrorRecoveryParametersPage), "byte7");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr WRITE_RETRY_COUNT_OFFSET = Marshal.OffsetOf(typeof(ReadWriteErrorRecoveryParametersPage), "_WriteRetryCount");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE9_OFFSET = Marshal.OffsetOf(typeof(ReadWriteErrorRecoveryParametersPage), "byte9");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr RECOVERY_TIME_LIMIT_OFFSET = Marshal.OffsetOf(typeof(ReadWriteErrorRecoveryParametersPage), "_RecoveryTimeLimit");

		public ReadWriteErrorRecoveryParametersPage() : base(ModePageCode.ReadWriteErrorRecoveryParameters) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte errorRecoveryParameter;
		[DisplayName("Automatic Write Reallocation")]
		public bool AutomaticWriteReallocationEnabled { get { return Bits.GetBit(this.errorRecoveryParameter, 7); } set { this.errorRecoveryParameter = Bits.SetBit(this.errorRecoveryParameter, 7, value); } }
		[DisplayName("Automatic Read Reallocation")]
		public bool AutomaticReadReallocationEnabled { get { return Bits.GetBit(this.errorRecoveryParameter, 6); } set { this.errorRecoveryParameter = Bits.SetBit(this.errorRecoveryParameter, 6, value); } }
		[DisplayName("Transfer Block")]
		public bool TransferBlock { get { return Bits.GetBit(this.errorRecoveryParameter, 5); } set { this.errorRecoveryParameter = Bits.SetBit(this.errorRecoveryParameter, 5, value); } }
		[DisplayName("Read Continuous")]
		public bool ReadContinuous { get { return Bits.GetBit(this.errorRecoveryParameter, 4); } set { this.errorRecoveryParameter = Bits.SetBit(this.errorRecoveryParameter, 4, value); } }
		[DisplayName("Post Error")]
		public bool PostError { get { return Bits.GetBit(this.errorRecoveryParameter, 2); } set { this.errorRecoveryParameter = Bits.SetBit(this.errorRecoveryParameter, 2, value); } }
		[DisplayName("Disable Transfer On Error")]
		public bool DisableTransferOnError { get { return Bits.GetBit(this.errorRecoveryParameter, 1); } set { this.errorRecoveryParameter = Bits.SetBit(this.errorRecoveryParameter, 1, value); } }
		[DisplayName("Disable Correction")]
		public bool DisableCorrection { get { return Bits.GetBit(this.errorRecoveryParameter, 0); } set { this.errorRecoveryParameter = Bits.SetBit(this.errorRecoveryParameter, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _ReadRetryCount;
		[DisplayName("Read Retry Count")]
		public byte ReadRetryCount { get { return this._ReadRetryCount; } set { this._ReadRetryCount = value; } }
		private byte _CorrectionSpan;
		private byte _HeadOffsetCount;
		private byte _DataStrobeOffsetCount;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _WriteRetryCount;
		[DisplayName("Write Retry Count")]
		public byte WriteRetryCount { get { return this._WriteRetryCount; } set { this._WriteRetryCount = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte9;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _RecoveryTimeLimit;
		/// <summary>Should be zero.</summary>
		[DisplayName("Recovery Time Limit")]
		public ushort RecoveryTimeLimit { get { return Bits.BigEndian(this._RecoveryTimeLimit); } set { this._RecoveryTimeLimit = Bits.BigEndian(value); } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.errorRecoveryParameter = buffer.Read<byte>(ERROR_RECOVERY_PARAMETER_OFFSET);
			this._ReadRetryCount = buffer.Read<byte>(READ_RETRY_COUNT_OFFSET);
			this._CorrectionSpan = buffer.Read<byte>(CORRECTION_SPAN_OFFSET);
			this._HeadOffsetCount = buffer.Read<byte>(HEAD_OFFSET_COUNT_OFFSET);
			this._DataStrobeOffsetCount = buffer.Read<byte>(DATA_STROBE_OFFSET_COUNT_OFFSET);
			this.byte7 = buffer.Read<byte>(BYTE7_OFFSET);
			this._WriteRetryCount = buffer.Read<byte>(WRITE_RETRY_COUNT_OFFSET);
			this.byte9 = buffer.Read<byte>(BYTE9_OFFSET);
			this._RecoveryTimeLimit = buffer.Read<ushort>(RECOVERY_TIME_LIMIT_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.errorRecoveryParameter, ERROR_RECOVERY_PARAMETER_OFFSET);
			buffer.Write(this._ReadRetryCount, READ_RETRY_COUNT_OFFSET);
			buffer.Write(this._CorrectionSpan, CORRECTION_SPAN_OFFSET);
			buffer.Write(this._HeadOffsetCount, HEAD_OFFSET_COUNT_OFFSET);
			buffer.Write(this._DataStrobeOffsetCount, DATA_STROBE_OFFSET_COUNT_OFFSET);
			buffer.Write(this.byte7, BYTE7_OFFSET);
			buffer.Write(this._WriteRetryCount, WRITE_RETRY_COUNT_OFFSET);
			buffer.Write(this.byte9, BYTE9_OFFSET);
			buffer.Write(this._RecoveryTimeLimit, RECOVERY_TIME_LIMIT_OFFSET);
		}

		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<ReadWriteErrorRecoveryParametersPage>(); } }


	}
}