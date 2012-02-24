using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Write10Command : FixedLengthScsiCommand
	{
		public Write10Command() : base(ScsiCommandCode.Write10) { }

		public Write10Command(bool forceUnitAccess, uint logicalBlockAddress, ushort blocksToTransfer)
			: this() { this.ForceUnitAccess = forceUnitAccess; this.LogicalBlockAddress = logicalBlockAddress; this.TransferBlockCount = blocksToTransfer; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		/// <summary>Must be zero for CD/Dvd logical units.</summary>
		public bool DisablePageOut { get { return Bits.GetBit(this.byte1, 4); } set { this.byte1 = Bits.SetBit(this.byte1, 4, value); } }
		public bool ForceUnitAccess { get { return Bits.GetBit(this.byte1, 3); } set { this.byte1 = Bits.SetBit(this.byte1, 3, value); } }
		public bool TimelySafeRecording { get { return Bits.GetBit(this.byte1, 2); } set { this.byte1 = Bits.SetBit(this.byte1, 2, value); } }
		/// <summary>Must be zero for CD/Dvd logical units.</summary>
		public bool RelativeAddress { get { return Bits.GetBit(this.byte1, 0); } set { this.byte1 = Bits.SetBit(this.byte1, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _LogicalBlockAddress;
		public uint LogicalBlockAddress { get { return Bits.BigEndian(this._LogicalBlockAddress); } set { this._LogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _TransferBlockCount;
		/// <summary>The number of blocks to transfer.</summary>
		public ushort TransferBlockCount { get { return Bits.BigEndian(this._TransferBlockCount); } set { this._TransferBlockCount = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;

		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }
	}
}