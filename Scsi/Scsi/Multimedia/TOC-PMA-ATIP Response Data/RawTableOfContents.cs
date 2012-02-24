using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class RawTableOfContents : TocPmaAtipResponseData
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr TRACK_DESCRIPTORS_OFFSET = Marshal.OffsetOf(typeof(RawTableOfContents), "_TrackDescriptors");

		public RawTableOfContents() : base() { }

		public byte FirstTrackNumber { get { return base.FirstTrackOrSession; } set { base.FirstTrackOrSession = value; } }
		public byte LastTrackNumber { get { return base.LastTrackOrSession; } set { base.LastTrackOrSession = value; } }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private RawTocTrackDescriptor[] _TrackDescriptors;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.RootHidden)]
		public RawTocTrackDescriptor[] TrackDescriptors { get { return this._TrackDescriptors; } set { this._TrackDescriptors = value; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._TrackDescriptors = Marshaler.PtrToStructure<RawTocTrackDescriptor[]>(buffer.ExtractSegment(TRACK_DESCRIPTORS_OFFSET));
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			Marshaler.StructureToPtr(this._TrackDescriptors, buffer.ExtractSegment(TRACK_DESCRIPTORS_OFFSET));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<RawTableOfContents>(); } }
	}

	public struct RawTocTrackDescriptor
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _SessionNumber;
		public byte SessionNumber { get { return this._SessionNumber; } set { this._SessionNumber = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _AddressControl;
		public CDControl Control { get { return unchecked((CDControl)(this._AddressControl & 0xF0)); } private set { this._AddressControl = (byte)((this._AddressControl & ~0xF0) | ((byte)value & 0xF0)); } }
		public CDAddress Address { get { return unchecked((CDAddress)(this._AddressControl & 0x0F)); } private set { this._AddressControl = (byte)((this._AddressControl & ~0x0F) | ((byte)value & 0x0F)); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _TrackNumber;
		public byte TrackNumber { get { return this._TrackNumber; } set { this._TrackNumber = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _Point;
		public byte Point { get { return this._Point; } set { this._Point = value; } }
		//Do not use the Msf structure, since sometimes these values don't actually represent time
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _Minute;
		public byte Minute { get { return this._Minute; } set { this._Minute = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _Second;
		public byte Second { get { return this._Second; } set { this._Second = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _Frame;
		public byte Frame { get { return this._Frame; } set { this._Frame = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _HourPHour;
		public byte HourPHour { get { return this._HourPHour; } set { this._HourPHour = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _PMinute;
		public byte PMinute { get { return this._PMinute; } set { this._PMinute = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _PSecond;
		public byte PSecond { get { return this._PSecond; } set { this._PSecond = value; } }
	}
}