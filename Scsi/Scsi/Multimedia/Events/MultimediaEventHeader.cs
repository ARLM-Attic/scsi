using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MultimediaEventHeader : IMarshalable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte NOTIFICATION_CLASS_MASK = 0x7;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _EventDataLength;
		public ushort EventDataLength { get { return Bits.BigEndian(this._EventDataLength); } set { this._EventDataLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool NoEventAvailable { get { return Bits.GetBit(this.byte1, 7); } set { this.byte1 = Bits.SetBit(this.byte1, 7, value); } }
		public NotificationClass NotificationClass
		{
			get { return (NotificationClass)Bits.GetValueMask(this.byte1, 0, NOTIFICATION_CLASS_MASK); }
			set { this.byte1 = Bits.PutValueMask(this.byte1, (byte)value, 0, NOTIFICATION_CLASS_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _SupportedEventClasses;
		public byte SupportedEventClasses { get { return this._SupportedEventClasses; } set { this._SupportedEventClasses = value; } }

		public void MarshalFrom(BufferWithSize buffer) { this = buffer.Read<MultimediaEventHeader>(); }

		public void MarshalTo(BufferWithSize buffer) { buffer.Write(this); }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int MarshaledSize { get { return Marshaler.DefaultSizeOf<MultimediaEventHeader>(); } }
	}
}