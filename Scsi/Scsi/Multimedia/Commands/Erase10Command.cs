using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Erase10Command : FixedLengthScsiCommand
	{
		public Erase10Command() : base(ScsiCommandCode.Erase10) { }

		public Erase10Command(bool eraseAll, bool immediate, uint logicalBlockAddress, ushort numBlocks)
			: this()
		{
			this.EraseAll = eraseAll;
			this.Immediate = immediate;
			this.LogicalBlockAddress = logicalBlockAddress;
			this.NumberOfBlocks = numBlocks;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool EraseAll { get { return Bits.GetBit(this.byte1, 2); } set { this.byte1 = Bits.SetBit(this.byte1, 2, value); } }
		public bool Immediate { get { return Bits.GetBit(this.byte1, 1); } set { this.byte1 = Bits.SetBit(this.byte1, 1, value); } }
		[Obsolete("Must be zero.", true)]
		public bool RelativeAddress { get { return Bits.GetBit(this.byte1, 0); } set { this.byte1 = Bits.SetBit(this.byte1, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _LogicalBlockAddress;
		public uint LogicalBlockAddress { get { return Bits.BigEndian(this._LogicalBlockAddress); } set { this._LogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _NumberOfBlocks;
		public ushort NumberOfBlocks { get { return Bits.BigEndian(this._NumberOfBlocks); } set { this._NumberOfBlocks = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return this.Immediate ? ScsiTimeoutGroup.Group1 : ScsiTimeoutGroup.Group2; } }
	}
}