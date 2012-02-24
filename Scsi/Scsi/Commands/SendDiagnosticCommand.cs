using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class SendDiagnosticCommand : FixedLengthScsiCommand
	{
		private const byte SELF_TEST_CODE_MASK = 0xE0;
		public SendDiagnosticCommand() : base(ScsiCommandCode.SendDiagnostic) { }
		public SendDiagnosticCommand(SelfTestCode? selfTestCode, bool unitOffline, bool deviceOffline)
			: this()
		{
			this.SelfTestCode = selfTestCode;
			this.UnitOffline = unitOffline;
			this.DeviceOffline = deviceOffline;
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public SelfTestCode? SelfTestCode
		{
			get { return this.SelfTest ? (SelfTestCode?)Bits.GetValueMask(this.byte1, 5, SELF_TEST_CODE_MASK) : null; }
			set
			{
				this.byte1 = Bits.PutValueMask(this.byte1, (byte)value.GetValueOrDefault(), 5, SELF_TEST_CODE_MASK);
				this.SelfTest = value == null;
			}
		}
		public bool PageFormat { get { return Bits.GetBit(this.byte1, 4); } set { this.byte1 = Bits.SetBit(this.byte1, 4, value); } }
		private bool SelfTest { get { return Bits.GetBit(this.byte1, 2); } set { this.byte1 = Bits.SetBit(this.byte1, 2, value); } }
		public bool UnitOffline { get { return Bits.GetBit(this.byte1, 1); } set { this.byte1 = Bits.SetBit(this.byte1, 1, value); } }
		public bool DeviceOffline { get { return Bits.GetBit(this.byte1, 0); } set { this.byte1 = Bits.SetBit(this.byte1, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _ParameterListLength;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public ushort ParameterListLength { get { return Bits.BigEndian(this._ParameterListLength); } set { this._ParameterListLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.None; } }
	}

	public enum SelfTestCode : byte
	{
		None = 0x00,
		BackgroundShortSelfTest = 0x01,
		BackgroundExtendedSelfTest = 0x02,
		AbortBackgroundSelfTest = 0x04,
		ForegroundShortSelfTest = 0x05,
		ForegroundExtendedSelfTest = 0x06,
	}
}