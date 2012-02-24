using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Read16Command : FixedLengthScsiCommand
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte GROUP_NUMBER_MASK = 0x1F;

		public Read16Command() : base(ScsiCommandCode.Read16) { }

		public Read16Command(bool forceUnitAccess, ulong logicalBlockAddress, uint transferBlockCount, bool streaming)
			: this()
		{
			this.ForceUnitAccess = forceUnitAccess;
			this.LogicalBlockAddress = logicalBlockAddress;
			this.TransferBlockCount = transferBlockCount;
			this.Streaming = streaming;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		/// <summary>Must be zero for CD/Dvd logical units.</summary>
		public bool DisablePageOut { get { return Bits.GetBit(this.byte1, 4); } set { this.byte1 = Bits.SetBit(this.byte1, 4, value); } }
		public bool ForceUnitAccess { get { return Bits.GetBit(this.byte1, 3); } set { this.byte1 = Bits.SetBit(this.byte1, 3, value); } }
		/// <summary>Must be zero for CD/Dvd logical units.</summary>
		public bool RelativeAddress { get { return Bits.GetBit(this.byte1, 0); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ulong _LogicalBlockAddress;
		public ulong LogicalBlockAddress { get { return Bits.BigEndian(this._LogicalBlockAddress); } set { this._LogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _TransferBlockCount;
		/// <summary>This field specifies the number of contiguous logical blocks of data that are transferred. A Transfer Length of zero indicates that no logical blocks are transferred. This condition is not considered an error. Any other value indicates the number of logical blocks that are transferred.</summary>
		public uint TransferBlockCount { get { return Bits.BigEndian(this._TransferBlockCount); } set { this._TransferBlockCount = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte10;
		public byte GroupNumber { get { return Bits.GetValueMask(this.byte10, 0, GROUP_NUMBER_MASK); } set { this.byte10 = Bits.PutValueMask(this.byte10, (byte)value, 0, GROUP_NUMBER_MASK); } }
		public bool Streaming { get { return Bits.GetBit(this.byte10, 7); } set { this.byte10 = Bits.SetBit(this.byte10, 7, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;

		public override ScsiTimeoutGroup TimeoutGroup { get { return this.Streaming ? ScsiTimeoutGroup.Group3 : ScsiTimeoutGroup.Group1; } }

		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }
	}
}