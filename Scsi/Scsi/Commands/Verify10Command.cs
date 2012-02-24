using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Verify10Command : FixedLengthScsiCommand
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte GROUP_NUMBER_MASK = 0x1F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte VERIFY_PROTECT_MASK = 0xE0;

		public Verify10Command() : base(ScsiCommandCode.Verify10) { }
		public Verify10Command(uint logicalBlockAddress, ushort numberOfBlocks)
			: this()
		{
			this.LogicalBlockAddress = logicalBlockAddress;
			this.PrefetchLength = numberOfBlocks;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public byte VerifyProtect { get { return Bits.GetValueMask(this.byte1, 5, VERIFY_PROTECT_MASK); } set { this.byte1 = Bits.PutValueMask(this.byte1, value, 5, VERIFY_PROTECT_MASK); } }
		/// <summary>Should be zero.</summary>
		public bool DisablePageOut { get { return Bits.GetBit(this.byte1, 4); } set { this.byte1 = Bits.SetBit(this.byte1, 4, value); } }
		/// <summary>Should be zero.</summary>
		public bool ByteCheck { get { return Bits.GetBit(this.byte1, 1); } set { this.byte1 = Bits.SetBit(this.byte1, 1, value); } }
		[Obsolete("Should be zero.", true)]
		public bool RelativeAddress { get { return Bits.GetBit(this.byte1, 0); } set { this.byte1 = Bits.SetBit(this.byte1, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _LogicalBlockAddress;
		public uint LogicalBlockAddress { get { return Bits.BigEndian(this._LogicalBlockAddress); } set { this._LogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		public bool Group3Timeout { get { return Bits.GetBit(this.byte6, 7); } set { this.byte6 = Bits.SetBit(this.byte6, 7, value); } }
		public byte GroupNumber { get { return Bits.GetValueMask(this.byte6, 0, GROUP_NUMBER_MASK); } set { this.byte6 = Bits.PutValueMask(this.byte6, value, 0, GROUP_NUMBER_MASK); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _PrefetchLength;
		public ushort PrefetchLength { get { return Bits.BigEndian(this._PrefetchLength); } set { this._PrefetchLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group2; } }
	}
}