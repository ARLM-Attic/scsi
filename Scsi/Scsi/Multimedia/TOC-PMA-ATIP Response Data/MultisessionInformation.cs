using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class MultisessionInformation : TocPmaAtipResponseData
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE0_OFFSET = Marshal.OffsetOf(typeof(MultisessionInformation), "byte0");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr ADDRESS_CONTROL_OFFSET = Marshal.OffsetOf(typeof(MultisessionInformation), "_AddressControl");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr FIRST_TRACK_NUMBER_IN_LAST_SESSION_OFFSET = Marshal.OffsetOf(typeof(MultisessionInformation), "_FirstTrackNumberInLastSession");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE3_OFFSET = Marshal.OffsetOf(typeof(MultisessionInformation), "byte3");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr START_ADDRESS_OF_FIRST_TRACK_IN_LAST_SESSION_OFFSET = Marshal.OffsetOf(typeof(MultisessionInformation), "_StartAddressOfFirstTrackInLastSession");

		public MultisessionInformation() : base() { }

		public byte FirstCompleteSessionNumber { get { return base.FirstTrackOrSession; } set { base.FirstTrackOrSession = value; } }
		public byte LastCompleteSessionNumber { get { return base.LastTrackOrSession; } set { base.LastTrackOrSession = value; } }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _AddressControl;
		public CDControl Control { get { return unchecked((CDControl)(this._AddressControl & 0xF0)); } private set { this._AddressControl = (byte)((this._AddressControl & ~0xF0) | ((byte)value & 0xF0)); } }
		public CDAddress Address { get { return unchecked((CDAddress)(this._AddressControl & 0x0F)); } private set { this._AddressControl = (byte)((this._AddressControl & ~0x0F) | ((byte)value & 0x0F)); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _FirstTrackNumberInLastSession;
		public byte FirstTrackNumberInLastSession { get { return this._FirstTrackNumberInLastSession; } set { this._FirstTrackNumberInLastSession = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte3;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private uint _StartAddressOfFirstTrackInLastSession;
		public uint StartAddressOfFirstTrackInLastSession { get { return Bits.BigEndian(this._StartAddressOfFirstTrackInLastSession); } set { this._StartAddressOfFirstTrackInLastSession = Bits.BigEndian(value); } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.byte0 = buffer.Read<byte>(BYTE0_OFFSET);
			this._AddressControl = buffer.Read<byte>(ADDRESS_CONTROL_OFFSET);
			this._FirstTrackNumberInLastSession = buffer.Read<byte>(FIRST_TRACK_NUMBER_IN_LAST_SESSION_OFFSET);
			this.byte3 = buffer.Read<byte>(BYTE3_OFFSET);
			this._StartAddressOfFirstTrackInLastSession = buffer.Read<uint>(START_ADDRESS_OF_FIRST_TRACK_IN_LAST_SESSION_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.byte0, BYTE0_OFFSET);
			buffer.Write(this._AddressControl, ADDRESS_CONTROL_OFFSET);
			buffer.Write(this._FirstTrackNumberInLastSession, FIRST_TRACK_NUMBER_IN_LAST_SESSION_OFFSET);
			buffer.Write(this.byte3, BYTE3_OFFSET);
			buffer.Write(this._StartAddressOfFirstTrackInLastSession, START_ADDRESS_OF_FIRST_TRACK_IN_LAST_SESSION_OFFSET);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<MultisessionInformation>(); } }
	}
}