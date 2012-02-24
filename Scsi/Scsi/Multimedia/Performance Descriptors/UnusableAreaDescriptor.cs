using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class UnusableAreaDescriptor : PerformanceDescriptor
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _LogicalBlockAddress;
		public uint LogicalBlockAddress { get { return Bits.BigEndian(this._LogicalBlockAddress); } set { this._LogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _NumberOfUnusablePhysicalBlocks;
		public uint NumberOfUnusablePhysicalBlocks { get { return Bits.BigEndian(this._NumberOfUnusablePhysicalBlocks); } set { this._NumberOfUnusablePhysicalBlocks = Bits.BigEndian(value); } }
	}
}