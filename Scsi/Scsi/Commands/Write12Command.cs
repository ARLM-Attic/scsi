using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Write12Command : FixedLengthScsiCommand
	{
		public Write12Command() : base(ScsiCommandCode.Write12) { }

		public Write12Command(bool forceUnitAccess, uint logicalBlockAddress, uint blocksToTransfer, bool streaming)
			: this()
		{
			this.ForceUnitAccess = forceUnitAccess;
			this.LogicalBlockAddress = logicalBlockAddress;
			this.TransferBlockCount = blocksToTransfer;
			this.Streaming = streaming;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		/// <summary>Must be zero for CD/Dvd logical units.</summary>
		public bool DisablePageOut { get { return Bits.GetBit(this.byte1, 4); } }
		public bool ForceUnitAccess { get { return Bits.GetBit(this.byte1, 3); } set { this.byte1 = Bits.SetBit(this.byte1, 3, value); } }
		public bool TimelySafeRecording { get { return Bits.GetBit(this.byte1, 2); } set { this.byte1 = Bits.SetBit(this.byte1, 2, value); } }
		/// <summary>Must be zero for CD/Dvd logical units.</summary>
		public bool RelativeAddress { get { return Bits.GetBit(this.byte1, 0); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _LogicalBlockAddress;
		public uint LogicalBlockAddress { get { return Bits.BigEndian(this._LogicalBlockAddress); } set { this._LogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _TransferBlockCount;
		/// <summary>The number of blocks to transfer.</summary>
		public uint TransferBlockCount { get { return Bits.BigEndian(this._TransferBlockCount); } set { this._TransferBlockCount = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte10;
		public bool Streaming { get { return Bits.GetBit(this.byte10, 7); } set { this.byte10 = Bits.SetBit(this.byte10, 7, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;

		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return this.Streaming ? ScsiTimeoutGroup.Group3 : ScsiTimeoutGroup.Group1; } }
	}
}