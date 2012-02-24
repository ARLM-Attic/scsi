using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct OperationalChangeEvent
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte EVENT_CODE_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte OPERATIONAL_STATUS_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		public OperationalEventCode OperationalEventCode
		{
			get { return (OperationalEventCode)Bits.GetValueMask(this.byte0, 0, EVENT_CODE_MASK); }
			set { this.byte0 = Bits.PutValueMask(this.byte0, (byte)value, 0, EVENT_CODE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool PersistentPrevented { get { return Bits.GetBit(this.byte1, 7); } set { this.byte1 = Bits.SetBit(this.byte1, 7, value); } }
		public OperationalStatusCode OperationalStatusCode
		{
			get { return (OperationalStatusCode)Bits.GetValueMask(this.byte1, 0, OPERATIONAL_STATUS_MASK); }
			set { this.byte1 = Bits.PutValueMask(this.byte1, (byte)value, 0, OPERATIONAL_STATUS_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OperationalChangeClass _OperationalChange;
		public OperationalChangeClass OperationalChangeClass { get { return Bits.BigEndian(this._OperationalChange); } set { this._OperationalChange = Bits.BigEndian(value); } }
	}
}