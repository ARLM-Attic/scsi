using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class WriteParametersPage : ModePage //Not structure because the ISRC & MCN are MANAGED
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE2_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "byte2");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE3_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "byte3");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE4_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "byte4");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr LINK_SIZE_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_LinkSize");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE6_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "byte6");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE7_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "byte7");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SESSION_FORMAT_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_SessionFormat");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE8_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "byte8");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr PACKET_SIZE_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_PacketSize");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr AUDIO_PAUSE_LENGTH_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_AudioPauseLength");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr MEDIA_CATALOG_NUMBER_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_MediaCatalogNumber");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr INTERNATIONAL_STANDARD_RECORDING_CODE_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_InternationalStandardRecordingCode");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SUB_HEADER_BYTE0_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_SubHeaderByte0");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SUB_HEADER_BYTE1_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_SubHeaderByte1");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SUB_HEADER_BYTE2_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_SubHeaderByte2");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SUB_HEADER_BYTE3_OFFSET = Marshal.OffsetOf(typeof(WriteParametersPage), "_SubHeaderByte3");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte WRITE_TYPE_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte MULTI_SESSION_MASK = 0xC0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte TRACK_MODE_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte DATA_BLOCK_TYPE_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte HOST_APPLICATION_CODE_MASK = 0x3F;

		public WriteParametersPage()
			: base(ModePageCode.WriteParameters)
		{
			this.TrackMode = TrackMode.Other;
			this.MultiSession = MultisessionType.SingleSession;
			this.SessionFormat = SessionFormat.CdromOrCddaOrOtherDataDisc;
			this.WriteType = WriteType.TrackAtOnce;
			this.DataBlockType = DataBlockType.Mode1;
			this.PacketSize = 16;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		/// <summary>
		/// <para>
		/// On CD-RW discs
		/// <list type="bullet">
		/// <item><description>When <c>false</c>: Buffer Under-run Free recording is disabled. When performing sequential recording and Drive’s write buffer becomes empty, it performs linking and terminate.</description></item>
		/// <item><description>When <c>true</c>: Buffer Under-run Free recording is enabled for sequential recording. The Drive performs zero-loss linking and continue writing when the buffer becomes nonempty.</description></item>
		/// </list>
		/// </para>
		/// <para>
		/// On DVD-RAM, DVD+R, DVD+RW:
		/// <list type="bullet">
		/// <item><description>The setting of the field has no meaning for either DVD-RAM, DVD+R or DVD+RW media and is ignored.</description></item>
		/// </list>
		/// </para>
		/// <para>
		/// On DVD-R and DVD-RW:
		/// <list type="bullet">
		/// <item><description>When <c>false</c>: Buffer Under-run Free recording is disabled. When performing sequential recording and Drive’s write buffer becomes empty, it performs linking and terminate writing.</description></item>
		/// <item><description>When <c>true</c>: Buffer Under-run Free recording is enabled for sequential recording. The Drive performs zero-loss linking and continue writing when the buffer becomes nonempty.</description></item>
		/// </list>
		/// </para>
		/// </summary>
		[DefaultValue(true)]
		[Description("Whether to enable buffer under-run protection.\r\nAs long as you do not use resource-intensive programs in the background, this should not make a difference.")]
		[DisplayName("Buffer Underrun Protection")]
		public bool BufferUnderrunProtection { get { return Bits.GetBit(this.byte2, 6); } set { this.byte2 = Bits.SetBit(this.byte2, 6, value); } }
		/// <summary>Whether the <see cref="LinkSize"/> field is valid.</summary>
		[Browsable(false)]
		private bool LinkSizeFieldValid { get { return Bits.GetBit(this.byte2, 5); } set { this.byte2 = Bits.SetBit(this.byte2, 5, value); } }
		/// <summary>On CD-R/RW media the Test Write bit is valid only for Write Type 1 or 2 (Track at Once or Session at Once). On DVD-R media, the Test Write bit is valid only for Write Type 0 or 2 (Incremental or Disc-at-once). When the Test Write bit is set to one, it indicates that the device performs the write process, but does not write data to the media. When the bit is set to zero the Write laser power is set such that user data is transferred to the media. In addition, all track and disc information collected, during test write mode, is cleared. It should be noted that the number of tracks reserved or written may be limited in test write mode.</summary>
		[Description("On CD-R/RW media, a test write is valid only for Track at Once or Session at Once mode.\r\nOn DVD-R media, a test write is valid only for Incremental or Disc-at-once mode.\r\nWhen a test write is specified, it indicates that the device performs the write process, but does not write data to the medium.\r\nWhen a test write is not specified, the write laser power is set such that user data is transferred to the media.\r\nIn addition, all track and disc information collected, during test write mode, is cleared.\r\nIt should be noted that the number of tracks reserved or written may be limited in test write mode.")]
		[DefaultValue(false)]
		[DisplayName("Test Write")]
		public bool TestWrite { get { return Bits.GetBit(this.byte2, 4); } set { this.byte2 = Bits.SetBit(this.byte2, 4, value); } }
		[Description("The type of writing to perform. Track-at-once is the least buggy and therefore the recommended option.")]
		[DisplayName("Write Type")]
		[DefaultValue(WriteType.TrackAtOnce)]
		public WriteType WriteType
		{
			get { return (WriteType)Bits.GetValueMask(this.byte2, 0, WRITE_TYPE_MASK); }
			set { this.byte2 = Bits.PutValueMask(this.byte2, (byte)value, 0, WRITE_TYPE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		[Description("The session mode. This is only valid for discs that support multisession mode.")]
		[DefaultValue(MultisessionType.SingleSession)]
		[DisplayName("Multisession Format")]
		public MultisessionType MultiSession { get { return (MultisessionType)Bits.GetValueMask(this.byte3, 6, MULTI_SESSION_MASK); } set { this.byte3 = Bits.PutValueMask(this.byte3, (byte)value, 6, MULTI_SESSION_MASK); } }
		/// <summary>When set to <c>true</c> indicates that the packet type is fixed. Otherwise, the packet type is variable. This bit is ignored unless the write type is set to <see cref="Multimedia.WriteType.PacketOrIncremental"/>. For DVD-R/-RW, this bit defaults to <c>true</c>.</summary>
		[Browsable(false)]
		private bool FixedPacket { get { return Bits.GetBit(this.byte3, 5); } set { this.byte3 = Bits.SetBit(this.byte3, 5, value); } }
		/// <summary>When the media is CD and Copy is set to one, SCMS recording is enabled. During recording, the copyright bit in the control nibble of each mode 1 Q Sub-channel alternates between 1 and 0 at 9.375 Hz. The duty cycle is 50%, changing every 4 blocks. The initial value on the medium is zero. When Copy is zero, SCMS recording is disabled. When the media is DVD-R/-RW, Copy is reserved.</summary>
		[Description("This value should not be modified.")]
		[DefaultValue(false)]
		[DisplayName("Copy")]
		public bool Copy { get { return Bits.GetBit(this.byte3, 4); } set { this.byte3 = Bits.SetBit(this.byte3, 4, value); } }
		/// <summary>On CD, Track Mode is the Control nibble in all Mode 1 Q Sub-channel in the track. For DVD-R/-RW, the default value should be 5.</summary>
		[DefaultValue(TrackMode.Other)]
		[DisplayName("Track Mode")]
		public TrackMode TrackMode
		{
			get { return (TrackMode)Bits.GetValueMask(this.byte3, 0, TRACK_MODE_MASK); }
			set { this.byte3 = Bits.PutValueMask(this.byte3, (byte)value, 0, TRACK_MODE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[Description("The data block type. Only Mode 1 is currently supported; using other modes may result in corrupt data being burned.")]
		[DefaultValue(DataBlockType.Mode1)]
		[DisplayName("Data Block Type")]
		public DataBlockType DataBlockType
		{
			get { return (DataBlockType)Bits.GetValueMask(this.byte4, 0, DATA_BLOCK_TYPE_MASK); }
			set { this.byte4 = Bits.PutValueMask(this.byte4, (byte)value, 0, DATA_BLOCK_TYPE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _LinkSize;
		/// <summary>The Linking Loss area size in sectors. This field is valid only for write type Packet/Incremental. When another Write Type is specified, the Drive ignores both <see cref="LinkSizeFieldValid"/> bit and <see cref="LinkSize"/> field. The Drive accepts values that are valid for the Drive but not valid for the current medium. If writing is attempted when an invalid Link Size is set, the Drive generates CHECK CONDITION status and set SK/ASC/ASCQ values to ILLEGAL REQUEST/ILLEGAL MODE FOR THIS TRACK.</summary>
		[DisplayName("Link Size")]
		[Description("The Linking Loss area size in sectors. If set to zero, assumed to be 7 by the drive.\r\nThis field is valid only for write type Packet/Incremental.\r\nThe Drive accepts values that are valid for the Drive but not valid for the current medium.\r\nIf writing is attempted when an invalid Link Size is set, the Drive generates Check Condition status\r\nand sets the sense key values to Illegal Request/Illegal Mode for This Track.")]
		public byte LinkSize { get { return this._LinkSize; } set { this.LinkSizeFieldValid = value != 0; this._LinkSize = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
		/// <summary>In the case of CD, this field typically has the value zero. When the unrestricted Use Disc bit in the Disc Information Block is set to one, this field is ignored by the device. If the Unrestricted Use Disc bit is zero, then the Host Application Code is set to the appropriate value for the medium in order that writing is allowed. A Host Application Code of zero is used for a Restricted Use – General Purpose Disc. On DVD-R/-RW, Host Application code is ignored.</summary>
		[DefaultValue((byte)0)]
		[DisplayName("Host Application Code")]
		public byte HostApplicationCode
		{
			get { return Bits.GetValueMask(this.byte7, 0, HOST_APPLICATION_CODE_MASK); }
			set { this.byte7 = Bits.PutValueMask(this.byte7, (byte)value, 0, HOST_APPLICATION_CODE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private SessionFormat _SessionFormat;
		[Description("The session format. It should be set to CD-DA/CD-ROM/other data disc.")]
		[DefaultValue(SessionFormat.CdromOrCddaOrOtherDataDisc)]
		[DisplayName("Session Format")]
		public SessionFormat SessionFormat { get { return this._SessionFormat; } set { this._SessionFormat = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte8;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _PacketSize;
		/// <summary>The number of User Data Blocks per fixed packet. For DVD-R/-RW media, the default Packet Size is 16.</summary>
		[DefaultValue((uint)16)]
		[DisplayName("Packet Size")]
		[Description("The number of User Data Blocks per fixed packet. For DVD-R/-RW media, the default Packet Size is 16.")]
		public uint PacketSize { get { return Bits.BigEndian(this._PacketSize); } set { this.FixedPacket = value != 0; this._PacketSize = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _AudioPauseLength;
		/// <summary>
		/// <see cref="AudioPauseLength"/> is applicable only to CD and is ignored for all other media types.
		/// <see cref="AudioPauseLength"/> is the number of blocks from the beginning of the track that the mode 1 Q Sub-channel INDEX is zero.
		/// If this number is zero, then there is no period where the Mode 1 Q Sub-channel INDEX is zero.
		/// The default value is <c>150</c>. This field is valid only for audio tracks, otherwise it is ignored.
		/// </summary>
		[DefaultValue((ushort)150)]
		[DisplayName("Audio Pause Length")]
		public ushort AudioPauseLength { get { return Bits.BigEndian(this._AudioPauseLength); } set { this._AudioPauseLength = Bits.BigEndian(value); } }
		/// <summary>The Media Catalog Number (MCN) is valid only for writable CD media. This field is ignored for other media types. The MCN is written in the mode 2 Q Sub-channel in at least one out of every 100 blocks in the program area. MCN in the Write Parameters mode page is formatted as in Table 677. MCVAL is the MCN valid flag. If MCVAL is zero, then the content of bytes 17 through 31 is ignored. If MCVAL is one, the bytes 17 through 31 contain a valid MCN. The MCN digits are ASCII representations of decimal digits (30h through 39h). The host may specify the content of bytes Zero and AFRAME; however, the Drive ignores these bytes and insert the appropriate values.</summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MediaCatalogNumber _MediaCatalogNumber; //Definition on page 245 in pdf, 209 on document; MMC-3
		[Browsable(false)]
		[DisplayName("Media Catalog Number")]
		public MediaCatalogNumber MediaCatalogNumber { get { return this._MediaCatalogNumber; } set { this._MediaCatalogNumber = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InternationalStandardRecordingCode _InternationalStandardRecordingCode;
		[Browsable(false)]
		[DisplayName("International Standard Recording Code")]
		public InternationalStandardRecordingCode InternationalStandardRecordingCode { get { return this._InternationalStandardRecordingCode; } set { this._InternationalStandardRecordingCode = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _SubHeaderByte0;
		[Browsable(false)]
		public byte SubHeaderByte0 { get { return this._SubHeaderByte0; } set { this._SubHeaderByte0 = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _SubHeaderByte1;
		[Browsable(false)]
		public byte SubHeaderByte1 { get { return this._SubHeaderByte1; } set { this._SubHeaderByte1 = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _SubHeaderByte2;
		[Browsable(false)]
		public byte SubHeaderByte2 { get { return this._SubHeaderByte2; } set { this._SubHeaderByte2 = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _SubHeaderByte3;
		[Browsable(false)]
		public byte SubHeaderByte3 { get { return this._SubHeaderByte3; } set { this._SubHeaderByte3 = value; } }
		/*
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _VendorSpecific;
		[Browsable(false)]
		public uint VendorSpecific { get { return this._VendorSpecific; } set { this._VendorSpecific = value; } }
		//*/

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			//Marshaler.PtrToStructure(buffer.ExtractSegment((int)INTERNATIONAL_STANDARD_RECORDING_CODE_OFFSET, Marshaler.DefaultSizeOf<InternationalStandardRecordingCode>()), ref this._InternationalStandardRecordingCode);
			//Marshaler.PtrToStructure(buffer.ExtractSegment((int)MEDIA_CATALOG_NUMBER_OFFSET, Marshaler.DefaultSizeOf<MediaCatalogNumber>()), ref this._MediaCatalogNumber);
			this.byte2 = buffer.Read<byte>(BYTE2_OFFSET);
			this.byte3 = buffer.Read<byte>(BYTE3_OFFSET);
			this.byte4 = buffer.Read<byte>(BYTE4_OFFSET);
			this._LinkSize = buffer.Read<byte>(LINK_SIZE_OFFSET);
			this.byte6 = buffer.Read<byte>(BYTE6_OFFSET);
			this.byte7 = buffer.Read<byte>(BYTE7_OFFSET);
			this._SessionFormat = buffer.Read<SessionFormat>(SESSION_FORMAT_OFFSET);
			this.byte8 = buffer.Read<byte>(BYTE8_OFFSET);
			this._PacketSize = buffer.Read<uint>(PACKET_SIZE_OFFSET);
			this._AudioPauseLength = buffer.Read<ushort>(AUDIO_PAUSE_LENGTH_OFFSET);
			this._MediaCatalogNumber = Marshaler.PtrToStructure<MediaCatalogNumber>(buffer.ExtractSegment(MEDIA_CATALOG_NUMBER_OFFSET));
			this._InternationalStandardRecordingCode = Marshaler.PtrToStructure<InternationalStandardRecordingCode>(buffer.ExtractSegment(INTERNATIONAL_STANDARD_RECORDING_CODE_OFFSET));
			this._SubHeaderByte0 = buffer.Read<byte>(SUB_HEADER_BYTE0_OFFSET);
			this._SubHeaderByte1 = buffer.Read<byte>(SUB_HEADER_BYTE1_OFFSET);
			this._SubHeaderByte2 = buffer.Read<byte>(SUB_HEADER_BYTE2_OFFSET);
			this._SubHeaderByte3 = buffer.Read<byte>(SUB_HEADER_BYTE3_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			//Marshaler.StructureToPtr(this.InternationalStandardRecordingCode, buffer.ExtractSegment((int)INTERNATIONAL_STANDARD_RECORDING_CODE_OFFSET, Marshaler.DefaultSizeOf<InternationalStandardRecordingCode>()));
			//Marshaler.StructureToPtr(this.MediaCatalogNumber, buffer.ExtractSegment((int)MEDIA_CATALOG_NUMBER_OFFSET, Marshaler.DefaultSizeOf<MediaCatalogNumber>()));
			buffer.Write(this.byte2, BYTE2_OFFSET);
			buffer.Write(this.byte3, BYTE3_OFFSET);
			buffer.Write(this.byte4, BYTE4_OFFSET);
			buffer.Write(this._LinkSize, LINK_SIZE_OFFSET);
			buffer.Write(this.byte6, BYTE6_OFFSET);
			buffer.Write(this.byte7, BYTE7_OFFSET);
			buffer.Write(this._SessionFormat, SESSION_FORMAT_OFFSET);
			buffer.Write(this.byte8, BYTE8_OFFSET);
			buffer.Write(this._PacketSize, PACKET_SIZE_OFFSET);
			buffer.Write(this._AudioPauseLength, AUDIO_PAUSE_LENGTH_OFFSET);
			Marshaler.StructureToPtr(this._MediaCatalogNumber, buffer.ExtractSegment(MEDIA_CATALOG_NUMBER_OFFSET));
			Marshaler.StructureToPtr(this._InternationalStandardRecordingCode, buffer.ExtractSegment(INTERNATIONAL_STANDARD_RECORDING_CODE_OFFSET));
			buffer.Write(this._SubHeaderByte0, SUB_HEADER_BYTE0_OFFSET);
			buffer.Write(this._SubHeaderByte1, SUB_HEADER_BYTE1_OFFSET);
			buffer.Write(this._SubHeaderByte2, SUB_HEADER_BYTE2_OFFSET);
			buffer.Write(this._SubHeaderByte3, SUB_HEADER_BYTE3_OFFSET);
		}

		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<WriteParametersPage>(); } }

	}
}