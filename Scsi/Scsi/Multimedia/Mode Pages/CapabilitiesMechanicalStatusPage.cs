using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class CapabilitiesMechanicalStatusPage : ModePage
	{

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE2_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte2");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE3_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte3");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE4_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte4");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE5_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte5");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE6_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte6");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE7_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte7");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr MAX_READ_SPEED_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_MaxReadSpeed");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr VOLUME_LEVELS_SUPPORTED_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_VolumeLevelsSupported");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BUFFER_SIZE_SUPPORTED_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_BufferSizeSupported");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr CURRENT_READ_SPEED_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_CurrentReadSpeed");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE16_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte16");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE17_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte17");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr MAX_WRITE_SPEED_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_MaxWriteSpeed");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr CURRENT_WRITE_SPEED_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_CurrentWriteSpeed");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr COPY_MANAGEMENT_REVISION_SUPPORTED_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_CopyManagementRevisionSupported");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE24_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte24");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE25_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte25");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE26_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte26");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE27_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "byte27");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr CURRENT_WRITE_SPEED_SELECTED_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_CurrentWriteSpeedSelected");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr NUM_LOGICAL_UNIT_WRITE_SPEED_PERFORMANCE_BLOCKS_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_NumLogicalUnitWriteSpeedPerformanceBlocks");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr LOGICAL_UNIT_WRITE_SPEED_PERFORMANCE_BLOCKS_OFFSET = Marshal.OffsetOf(typeof(CapabilitiesMechanicalStatusPage), "_LogicalUnitWriteSpeedPerformanceBlocks");


		public CapabilitiesMechanicalStatusPage() : base(ModePageCode.CapabilitiesAndMechanicalStatus) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte LOADING_MECHANISM_TYPE_MASK = 0xE0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte LENGTH_MASK = 0x30;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte ROTATION_CONTROL_MASK = 0x03;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		public bool ReadDvdRam { get { return Bits.GetBit(this.byte2, 5); } set { this.byte2 = Bits.SetBit(this.byte2, 5, value); } }
		public bool ReadDvdR { get { return Bits.GetBit(this.byte2, 4); } set { this.byte2 = Bits.SetBit(this.byte2, 4, value); } }
		public bool ReadDvdRom { get { return Bits.GetBit(this.byte2, 3); } set { this.byte2 = Bits.SetBit(this.byte2, 3, value); } }
		public bool Method2 { get { return Bits.GetBit(this.byte2, 2); } set { this.byte2 = Bits.SetBit(this.byte2, 2, value); } }
		public bool ReadCDRW { get { return Bits.GetBit(this.byte2, 1); } set { this.byte2 = Bits.SetBit(this.byte2, 1, value); } }
		public bool ReadCDR { get { return Bits.GetBit(this.byte2, 0); } set { this.byte2 = Bits.SetBit(this.byte2, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		public bool WriteDvdRam { get { return Bits.GetBit(this.byte3, 5); } set { this.byte3 = Bits.SetBit(this.byte3, 5, value); } }
		public bool WriteDvdR { get { return Bits.GetBit(this.byte3, 4); } set { this.byte3 = Bits.SetBit(this.byte3, 4, value); } }
		public bool TestWrite { get { return Bits.GetBit(this.byte3, 2); } set { this.byte3 = Bits.SetBit(this.byte3, 2, value); } }
		public bool WriteCDRW { get { return Bits.GetBit(this.byte3, 1); } set { this.byte3 = Bits.SetBit(this.byte3, 1, value); } }
		public bool WriteCDR { get { return Bits.GetBit(this.byte3, 0); } set { this.byte3 = Bits.SetBit(this.byte3, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		public bool BufferUnderrunProtection { get { return Bits.GetBit(this.byte4, 7); } set { this.byte4 = Bits.SetBit(this.byte4, 7, value); } }
		public bool MultiSession { get { return Bits.GetBit(this.byte4, 6); } set { this.byte4 = Bits.SetBit(this.byte4, 6, value); } }
		public bool Mode2Form2 { get { return Bits.GetBit(this.byte4, 5); } set { this.byte4 = Bits.SetBit(this.byte4, 5, value); } }
		public bool Mode2Form1 { get { return Bits.GetBit(this.byte4, 4); } set { this.byte4 = Bits.SetBit(this.byte4, 4, value); } }
		public bool DigitalPort2 { get { return Bits.GetBit(this.byte4, 3); } set { this.byte4 = Bits.SetBit(this.byte4, 3, value); } }
		public bool DigitalPort1 { get { return Bits.GetBit(this.byte4, 2); } set { this.byte4 = Bits.SetBit(this.byte4, 2, value); } }
		public bool Composite { get { return Bits.GetBit(this.byte4, 1); } set { this.byte4 = Bits.SetBit(this.byte4, 1, value); } }
		public bool AudioPlay { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		public bool ReadBarCode { get { return Bits.GetBit(this.byte5, 7); } set { this.byte5 = Bits.SetBit(this.byte5, 7, value); } }
		public bool UniversalProductCode { get { return Bits.GetBit(this.byte5, 6); } set { this.byte5 = Bits.SetBit(this.byte5, 6, value); } }
		public bool InternationalStandardRecordingCode { get { return Bits.GetBit(this.byte5, 5); } set { this.byte5 = Bits.SetBit(this.byte5, 5, value); } }
		public bool SupportsC2Pointers { get { return Bits.GetBit(this.byte5, 4); } set { this.byte5 = Bits.SetBit(this.byte5, 4, value); } }
		public bool RWDeinterleavedAndCorrected { get { return Bits.GetBit(this.byte5, 3); } set { this.byte5 = Bits.SetBit(this.byte5, 3, value); } }
		public bool RWSupported { get { return Bits.GetBit(this.byte5, 2); } set { this.byte5 = Bits.SetBit(this.byte5, 2, value); } }
		public bool AccurateCDDigitalAudioStream { get { return Bits.GetBit(this.byte5, 1); } set { this.byte5 = Bits.SetBit(this.byte5, 1, value); } }
		public bool SupportsCDDigitalAudioCommands { get { return Bits.GetBit(this.byte5, 0); } set { this.byte5 = Bits.SetBit(this.byte5, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		public LoadingMechanism LoadingMechanismType
		{
			get { return (LoadingMechanism)Bits.GetValueMask(this.byte6, 5, LOADING_MECHANISM_TYPE_MASK); }
			set { this.byte6 = Bits.PutValueMask(this.byte6, (byte)value, 5, LOADING_MECHANISM_TYPE_MASK); }
		}
		public bool Eject { get { return Bits.GetBit(this.byte6, 3); } set { this.byte6 = Bits.SetBit(this.byte6, 3, value); } }
		public bool PreventJumper { get { return Bits.GetBit(this.byte6, 2); } set { this.byte6 = Bits.SetBit(this.byte6, 2, value); } }
		public bool LockState { get { return Bits.GetBit(this.byte6, 1); } set { this.byte6 = Bits.SetBit(this.byte6, 1, value); } }
		public bool Lock { get { return Bits.GetBit(this.byte6, 0); } set { this.byte6 = Bits.SetBit(this.byte6, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
		public bool RWInLeadIn { get { return Bits.GetBit(this.byte7, 5); } set { this.byte7 = Bits.SetBit(this.byte7, 5, value); } }
		public bool SideChangeCapable { get { return Bits.GetBit(this.byte7, 4); } set { this.byte7 = Bits.SetBit(this.byte7, 4, value); } }
		public bool SoftwareSlotSelection { get { return Bits.GetBit(this.byte7, 3); } set { this.byte7 = Bits.SetBit(this.byte7, 3, value); } }
		public bool ChangerSupportsDiscPresent { get { return Bits.GetBit(this.byte7, 2); } set { this.byte7 = Bits.SetBit(this.byte7, 2, value); } }
		public bool SeparateChannelMute { get { return Bits.GetBit(this.byte7, 1); } set { this.byte7 = Bits.SetBit(this.byte7, 1, value); } }
		public bool SeparateVolumeLevels { get { return Bits.GetBit(this.byte7, 0); } set { this.byte7 = Bits.SetBit(this.byte7, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _MaxReadSpeed;
		/// <summary>The maximum read speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)</summary>
		[Description("The maximum read speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)")]
		public ushort MaxReadSpeed { get { return Bits.BigEndian(this._MaxReadSpeed); } set { this._MaxReadSpeed = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _VolumeLevelsSupported;
		public ushort VolumeLevelsSupported { get { return Bits.BigEndian(this._VolumeLevelsSupported); } set { this._VolumeLevelsSupported = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _BufferSizeSupported;
		public ushort BufferSizeSupported { get { return Bits.BigEndian(this._BufferSizeSupported); } set { this._BufferSizeSupported = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _CurrentReadSpeed;
		/// <summary>The current read speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)</summary>
		[Description("The current read speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)")]
		public ushort CurrentReadSpeed { get { return Bits.BigEndian(this._CurrentReadSpeed); } set { this._CurrentReadSpeed = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte16;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte17;
		public byte Length
		{
			get { return Bits.GetValueMask(this.byte17, 4, LENGTH_MASK); }
			set { this.byte17 = Bits.PutValueMask(this.byte17, (byte)value, 4, LENGTH_MASK); }
		}
		/// <summary>Indicates whether the data words are little or big endian.</summary>
		[Description("Indicates whether the data words are little or big endian.")]
		public bool LittleEndian { get { return Bits.GetBit(this.byte17, 3); } set { this.byte17 = Bits.SetBit(this.byte17, 3, value); } }
		public bool RCK { get { return Bits.GetBit(this.byte17, 2); } set { this.byte17 = Bits.SetBit(this.byte17, 2, value); } }
		public bool BCKF { get { return Bits.GetBit(this.byte17, 1); } set { this.byte17 = Bits.SetBit(this.byte17, 1, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _MaxWriteSpeed;
		/// <summary>The maximum write speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)</summary>
		[Description("The maximum write speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)")]
		public ushort MaxWriteSpeed { get { return Bits.BigEndian(this._MaxWriteSpeed); } set { this._MaxWriteSpeed = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _CurrentWriteSpeed;
		/// <summary>The current write speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)</summary>
		[Description("The current write speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)")]
		public ushort CurrentWriteSpeed { get { return Bits.BigEndian(this._CurrentWriteSpeed); } set { this._CurrentWriteSpeed = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _CopyManagementRevisionSupported;
		public ushort CopyManagementRevisionSupported { get { return Bits.BigEndian(this._CopyManagementRevisionSupported); } set { this._CopyManagementRevisionSupported = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte24;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte25;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte26;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte27;
		public RotationControl RotationControlSelected
		{
			get { return (RotationControl)Bits.GetValueMask(this.byte27, 0, ROTATION_CONTROL_MASK); }
			set { this.byte27 = Bits.PutValueMask(this.byte27, (byte)value, 0, ROTATION_CONTROL_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _CurrentWriteSpeedSelected;
		/// <summary>The current write speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)</summary>
		[Description("The current write speed of the drive, in KB (1000 bytes) per second. (This is NOT KiB/s, or 1024 B/s.)")]
		public ushort CurrentWriteSpeedSelected { get { return Bits.BigEndian(this._CurrentWriteSpeedSelected); } set { this._CurrentWriteSpeedSelected = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _NumLogicalUnitWriteSpeedPerformanceBlocks;
		public ushort NumLogicalUnitWriteSpeedPerformanceBlocks { get { return Bits.BigEndian(this._NumLogicalUnitWriteSpeedPerformanceBlocks); } set { this._NumLogicalUnitWriteSpeedPerformanceBlocks = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private LogicalUnitWriteSpeedPerformanceBlock[] _LogicalUnitWriteSpeedPerformanceBlocks;
		public LogicalUnitWriteSpeedPerformanceBlock[] LogicalUnitWriteSpeedPerformanceBlocks
		{
			get { return this._LogicalUnitWriteSpeedPerformanceBlocks; }
			private set
			{
				this._LogicalUnitWriteSpeedPerformanceBlocks = value;
				var count = value != null ? (ushort)value.Length : (ushort)0;
				this._NumLogicalUnitWriteSpeedPerformanceBlocks = count;
				this.PageLength = (byte)(count * 4 + 28);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return (Marshal.SizeOf(this) + (this.LogicalUnitWriteSpeedPerformanceBlocks.Length - 1) * Marshaler.DefaultSizeOf<LogicalUnitWriteSpeedPerformanceBlock>()); } }


		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.byte2 = buffer.Read<byte>(BYTE2_OFFSET);
			this.byte3 = buffer.Read<byte>(BYTE3_OFFSET);
			this.byte4 = buffer.Read<byte>(BYTE4_OFFSET);
			this.byte5 = buffer.Read<byte>(BYTE5_OFFSET);
			this.byte6 = buffer.Read<byte>(BYTE6_OFFSET);
			this.byte7 = buffer.Read<byte>(BYTE7_OFFSET);
			this._MaxReadSpeed = buffer.Read<ushort>(MAX_READ_SPEED_OFFSET);
			this._VolumeLevelsSupported = buffer.Read<ushort>(VOLUME_LEVELS_SUPPORTED_OFFSET);
			this._BufferSizeSupported = buffer.Read<ushort>(BUFFER_SIZE_SUPPORTED_OFFSET);
			this._CurrentReadSpeed = buffer.Read<ushort>(CURRENT_READ_SPEED_OFFSET);
			this.byte16 = buffer.Read<byte>(BYTE16_OFFSET);
			this.byte17 = buffer.Read<byte>(BYTE17_OFFSET);
			this._MaxWriteSpeed = buffer.Read<ushort>(MAX_WRITE_SPEED_OFFSET);
			this._CurrentWriteSpeed = buffer.Read<ushort>(CURRENT_WRITE_SPEED_OFFSET);
			this._CopyManagementRevisionSupported = buffer.Read<ushort>(COPY_MANAGEMENT_REVISION_SUPPORTED_OFFSET);
			this.byte24 = buffer.Read<byte>(BYTE24_OFFSET);
			this.byte25 = buffer.Read<byte>(BYTE25_OFFSET);
			this.byte26 = buffer.Read<byte>(BYTE26_OFFSET);
			this.byte27 = buffer.Read<byte>(BYTE27_OFFSET);
			this._CurrentWriteSpeedSelected = buffer.Read<ushort>(CURRENT_WRITE_SPEED_SELECTED_OFFSET);
			this._NumLogicalUnitWriteSpeedPerformanceBlocks = buffer.Read<ushort>(NUM_LOGICAL_UNIT_WRITE_SPEED_PERFORMANCE_BLOCKS_OFFSET);
			this._LogicalUnitWriteSpeedPerformanceBlocks = new LogicalUnitWriteSpeedPerformanceBlock[this.NumLogicalUnitWriteSpeedPerformanceBlocks];
			unsafe
			{
				var perfBlocks = buffer.ExtractSegment(LOGICAL_UNIT_WRITE_SPEED_PERFORMANCE_BLOCKS_OFFSET);
				for (ushort i = 0; i < this.LogicalUnitWriteSpeedPerformanceBlocks.Length; i++)
				{ this._LogicalUnitWriteSpeedPerformanceBlocks[i] = perfBlocks.Read<LogicalUnitWriteSpeedPerformanceBlock>(i * sizeof(LogicalUnitWriteSpeedPerformanceBlock)); }
			}
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.byte2, BYTE2_OFFSET);
			buffer.Write(this.byte3, BYTE3_OFFSET);
			buffer.Write(this.byte4, BYTE4_OFFSET);
			buffer.Write(this.byte5, BYTE5_OFFSET);
			buffer.Write(this.byte6, BYTE6_OFFSET);
			buffer.Write(this.byte7, BYTE7_OFFSET);
			buffer.Write(this._MaxReadSpeed, MAX_READ_SPEED_OFFSET);
			buffer.Write(this._VolumeLevelsSupported, VOLUME_LEVELS_SUPPORTED_OFFSET);
			buffer.Write(this._BufferSizeSupported, BUFFER_SIZE_SUPPORTED_OFFSET);
			buffer.Write(this._CurrentReadSpeed, CURRENT_READ_SPEED_OFFSET);
			buffer.Write(this.byte16, BYTE16_OFFSET);
			buffer.Write(this.byte17, BYTE17_OFFSET);
			buffer.Write(this._MaxWriteSpeed, MAX_WRITE_SPEED_OFFSET);
			buffer.Write(this._CurrentWriteSpeed, CURRENT_WRITE_SPEED_OFFSET);
			buffer.Write(this._CopyManagementRevisionSupported, COPY_MANAGEMENT_REVISION_SUPPORTED_OFFSET);
			buffer.Write(this.byte24, BYTE24_OFFSET);
			buffer.Write(this.byte25, BYTE25_OFFSET);
			buffer.Write(this.byte26, BYTE26_OFFSET);
			buffer.Write(this.byte27, BYTE27_OFFSET);
			buffer.Write(this._CurrentWriteSpeedSelected, CURRENT_WRITE_SPEED_SELECTED_OFFSET);
			buffer.Write(this._NumLogicalUnitWriteSpeedPerformanceBlocks, NUM_LOGICAL_UNIT_WRITE_SPEED_PERFORMANCE_BLOCKS_OFFSET);
			unsafe
			{
				var perfBlocks = buffer.ExtractSegment(LOGICAL_UNIT_WRITE_SPEED_PERFORMANCE_BLOCKS_OFFSET);
				for (ushort i = 0; i < this._LogicalUnitWriteSpeedPerformanceBlocks.Length; i++)
				{ perfBlocks.Write(this._LogicalUnitWriteSpeedPerformanceBlocks[i], i * sizeof(LogicalUnitWriteSpeedPerformanceBlock)); }
			}
		}
	}


	[StructLayout(LayoutKind.Sequential, Pack = 1), DebuggerDisplay(@"\{RotationControlSelected = {RotationControlSelected}, WriteSpeedSupported = {WriteSpeedSupported}\}")]
	public struct LogicalUnitWriteSpeedPerformanceBlock
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte ROTATION_CONTROL_MASK = 0x03;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public RotationControl RotationControlSelected
		{
			get { return (RotationControl)Bits.GetValueMask(this.byte1, 0, ROTATION_CONTROL_MASK); }
			set { this.byte1 = Bits.PutValueMask(this.byte1, (byte)value, 0, ROTATION_CONTROL_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _WriteSpeedSupported;
		/// <summary>The write speed, in kbytes/sec. (Is this KB or KiB?)</summary>
		[Description("The write speed, in kbytes/sec. (Is this KB or KiB?)")]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public ushort WriteSpeedSupported { get { return Bits.BigEndian(this._WriteSpeedSupported); } set { this._WriteSpeedSupported = Bits.BigEndian(value); } }
	}
}