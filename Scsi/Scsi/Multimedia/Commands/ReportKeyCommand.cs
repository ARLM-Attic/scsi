using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ReportKeyCommand : FixedLengthScsiCommand
	{
		private const byte KEY_FORMAT_MASK = 0x3F;
		private const byte AuthenticationGrantId_MASK = 0xC0;
		public ReportKeyCommand() : base(ScsiCommandCode.ReportKey) { }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _ReservedOrLogicalBlockAddressOrStartingOffset;
		public uint ReservedOrLogicalBlockAddressOrStartingOffset { get { return Bits.BigEndian(this._ReservedOrLogicalBlockAddressOrStartingOffset); } set { this._ReservedOrLogicalBlockAddressOrStartingOffset = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _ReservedOrBlockCountOrVCPSFunction;
		public byte ReservedOrBlockCountOrVCPSFunction { get { return this._ReservedOrBlockCountOrVCPSFunction; } set { this._ReservedOrBlockCountOrVCPSFunction = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private KeyClass _KeyClass;
		public KeyClass KeyClass { get { return this._KeyClass; } set { this._KeyClass = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _AllocationLength;
		public ushort AllocationLength { get { return Bits.BigEndian(this._AllocationLength); } set { this._AllocationLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte10;
		public KeyFormat KeyFormat
		{
			get { return (KeyFormat)Bits.GetValueMask(this.byte10, 0, KEY_FORMAT_MASK); }
			set { this.byte10 = Bits.PutValueMask(this.byte10, (byte)value, 0, KEY_FORMAT_MASK); }
		}
		public byte AuthenticationGrantId
		{
			get { return Bits.GetValueMask(this.byte10, 6, AuthenticationGrantId_MASK); }
			set { this.byte10 = Bits.PutValueMask(this.byte10, (byte)value, 6, AuthenticationGrantId_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }
	}
}