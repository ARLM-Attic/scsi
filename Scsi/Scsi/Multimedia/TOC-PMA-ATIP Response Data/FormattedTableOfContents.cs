using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class FormattedTableOfContents : TocPmaAtipResponseData
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr TRACK_DESCRIPTORS_OFFSET = Marshal.OffsetOf(typeof(FormattedTableOfContents), "_TrackDescriptors");

		public FormattedTableOfContents() : base() { }

		public byte FirstTrackNumber { get { return base.FirstTrackOrSession; } set { base.FirstTrackOrSession = value; } }
		public byte LastTrackNumber { get { return base.LastTrackOrSession; } set { base.LastTrackOrSession = value; } }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private TocTrackDescriptor[] _TrackDescriptors;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.RootHidden)]
		public TocTrackDescriptor[] TrackDescriptors { get { return this._TrackDescriptors; } set { this._TrackDescriptors = value; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._TrackDescriptors = new TocTrackDescriptor[(this.DataLength + sizeof(ushort) - base.MarshaledSize) / Marshaler.DefaultSizeOf<TocTrackDescriptor>()];
			for (int i = 0; i < this._TrackDescriptors.Length; i++)
			{ this._TrackDescriptors[i] = Marshaler.PtrToStructure<TocTrackDescriptor>(buffer.ExtractSegment((int)TRACK_DESCRIPTORS_OFFSET + i * Marshaler.DefaultSizeOf<TocTrackDescriptor>(), Marshaler.DefaultSizeOf<TocTrackDescriptor>())); }
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			for (int i = 0; i < this._TrackDescriptors.Length; i++)
			{ Marshaler.StructureToPtr(this._TrackDescriptors[i], buffer.ExtractSegment((int)TRACK_DESCRIPTORS_OFFSET + i * Marshaler.DefaultSizeOf<TocTrackDescriptor>(), Marshaler.SizeOf(this._TrackDescriptors[i]))); }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<FormattedTableOfContents>(); } }
	}

	public struct TocTrackDescriptor
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
#pragma warning disable 0169
		private byte byte0;
#pragma warning restore 0169
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _AddressControl;
		public CDControl Control { get { return unchecked((CDControl)(this._AddressControl & 0xF0)); } private set { this._AddressControl = (byte)((this._AddressControl & ~0xF0) | ((byte)value & 0xF0)); } }
		public CDAddress Address { get { return unchecked((CDAddress)(this._AddressControl & 0x0F)); } private set { this._AddressControl = (byte)((this._AddressControl & ~0x0F) | ((byte)value & 0x0F)); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _TrackNumber;
		public byte TrackNumber { get { return this._TrackNumber; } set { this._TrackNumber = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
#pragma warning disable 0169
		private byte byte3;
#pragma warning restore 0169
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private uint _TrackStartAddress;
		public uint TrackStartAddress { get { return Bits.BigEndian(this._TrackStartAddress); } set { this._TrackStartAddress = Bits.BigEndian(value); } }
	}
}