using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MultipleHostEvent
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte EVENT_CODE_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte MULTIPLE_HOST_STATUS_MASK = 0x0F;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		public MultipleHostEventCode MultipleHostEventCode
		{
			get { return (MultipleHostEventCode)Bits.GetValueMask(this.byte0, 0, EVENT_CODE_MASK); }
			set { this.byte0 = Bits.PutValueMask(this.byte0, (byte)value, 0, EVENT_CODE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool PersistentPrevented { get { return Bits.GetBit(this.byte1, 7); } set { this.byte1 = Bits.SetBit(this.byte1, 7, value); } }
		public MultipleHostStatusCode MultipleHostStatusCode
		{
			get { return (MultipleHostStatusCode)Bits.GetValueMask(this.byte1, 0, MULTIPLE_HOST_STATUS_MASK); }
			set { this.byte1 = Bits.PutValueMask(this.byte0, (byte)value, 1, MULTIPLE_HOST_STATUS_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _MultipleHostPriority;
		public ushort MultipleHostPriority { get { return Bits.BigEndian(this._MultipleHostPriority); } set { this._MultipleHostPriority = Bits.BigEndian(value); } }
	}
}