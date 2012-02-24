using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Read06Command : FixedLengthScsiCommand
	{
		public Read06Command() : base(ScsiCommandCode.Read06) { }
		public Read06Command(uint logicalBlockAddress, byte transferBlockCount) : this() { this.LogicalBlockAddress = logicalBlockAddress; this.TransferBlockCount = transferBlockCount; }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte1; //MSB for bits 0-4, reserved for bits 5-7
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte3; //LSB
		public uint LogicalBlockAddress
		{
			get { unchecked { return (uint)this._byte3 | ((uint)this._byte2 << 8) | ((uint)(this._byte1 & 0x1F) << 16); } }
			set { unchecked { this._byte3 = (byte)(value >> 0); this._byte2 = (byte)(value >> 8); this._byte1 = unchecked((byte)((this._byte1 & ~0x1FU) | (value >> 16))); } }
		}
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _TransferBlockCount;
		public byte TransferBlockCount { get { return this._TransferBlockCount; } set { this._TransferBlockCount = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }
	}
}