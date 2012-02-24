using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class DefectStatusDescriptor : PerformanceDescriptor
	{
		private const byte FIRST_BIT_OFFSET_MASK = 0x7;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _StartLogicalBlockAddress;
		public uint StartLogicalBlockAddress { get { return Bits.BigEndian(this._StartLogicalBlockAddress); } set { this._StartLogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _EndLogicalBlockAddress;
		public uint EndLogicalBlockAddress { get { return Bits.BigEndian(this._EndLogicalBlockAddress); } set { this._EndLogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _BlockingFactor;
		public byte BlockingFactor { get { return this._BlockingFactor; } set { this._BlockingFactor = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte9;
		public byte FirstBitOffset
		{
			get { return Bits.GetValueMask(this.byte9, 0, FIRST_BIT_OFFSET_MASK); }
			set { this.byte9 = Bits.PutValueMask(this.byte9, (byte)value, 0, FIRST_BIT_OFFSET_MASK); }
		}
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048 - 10), DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte[] _DefectStatuses = new byte[2048 - 10];
		public bool GetDefectStatus(ushort index) { return (this._DefectStatuses[index >> 3] & (index % 8)) != 0; }
		public void SetDefectStatus(ushort index, bool value) { this._DefectStatuses[index >> 3] = Bits.SetBit(this._DefectStatuses[index >> 3], 7, value); }
	}
}