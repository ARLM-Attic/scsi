using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public sealed class StandardInquiryData : InquiryData
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly char[] TRIM_CHARS = new char[] { '\0', ' ' };
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly int MinimumSize = 36;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE1_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "byte1");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr VERSION_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "_Version");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE3_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "byte3");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static readonly IntPtr ADDITIONAL_LENGTH_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "_AdditionalLength");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE5_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "byte5");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr ADDITIONAL_LENGTH_END_OFFSET = BYTE5_OFFSET;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE6_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "byte6");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE7_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "byte7");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr VENDOR_IDENTIFICATION_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "_VendorIdentification");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr PRODUCT_IDENTIFICATION_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "_ProductIdentification");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr PRODUCT_REVISION_LEVEL_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "_ProductRevisionLevel");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr VENDOR_SPECIFIC_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "_VendorSpecific");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE56_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "byte56");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE57_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "byte57");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr VERSION_DESCRIPTORS_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "_VersionDescriptors");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr RESERVED74_TO95_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "reserved74To95");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr VENDOR_SPECIFIC_PARAMETERS_OFFSET = Marshal.OffsetOf(typeof(StandardInquiryData), "_VendorSpecificParameters");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte VENDOR_IDENTIFICATION_LENGTH = 8;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte PRODUCT_IDENTIFICATION_LENGTH = 16;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte PRODUCT_REVISION_LENGTH = 4;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte RESPONSE_DATA_FORMAT_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte TARGET_PORT_GROUP_SUPPORT_MASK = 0x30;

		public StandardInquiryData() { this.AdditionalLength = (byte)((int)VENDOR_SPECIFIC_PARAMETERS_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET); }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool RemovableMedium { get { return Bits.GetBit(this.byte1, 7); } set { this.byte1 = Bits.SetBit(this.byte1, 7, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private DeviceVersion _Version;
		public DeviceVersion Version { get { return this._Version; } set { this._Version = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		public bool AsynchronousEventReportingCapability { get { return Bits.GetBit(this.byte3, 7); } set { this.byte3 = Bits.SetBit(this.byte3, 7, value); } }
		public bool NormalACA { get { return Bits.GetBit(this.byte3, 5); } set { this.byte3 = Bits.SetBit(this.byte3, 5, value); } }
		public bool HierarchicalSupport { get { return Bits.GetBit(this.byte3, 4); } set { this.byte3 = Bits.SetBit(this.byte3, 4, value); } }
		public byte ResponseDataFormat
		{
			get { return (byte)Bits.GetValueMask(this.byte3, 0, RESPONSE_DATA_FORMAT_MASK); }
			set { this.byte3 = Bits.PutValueMask(this.byte3, (byte)value, 0, RESPONSE_DATA_FORMAT_MASK); }
		}
		/// <summary>This is the size of this structure AFTER this field, according to the SPC documentation.</summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _AdditionalLength;
		public byte AdditionalLength { get { return this._AdditionalLength; } set { this._AdditionalLength = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		public bool EmbeddedStorageArrayControlComponentSupported { get { return Bits.GetBit(this.byte5, 7); } set { this.byte5 = Bits.SetBit(this.byte5, 7, value); } }
		public bool AccessControlsCoordinator { get { return Bits.GetBit(this.byte5, 6); } set { this.byte5 = Bits.SetBit(this.byte5, 6, value); } }
		public TargetPortGroupSupport TargetPortGroupSupport
		{
			get { return (TargetPortGroupSupport)Bits.GetValueMask(this.byte5, 4, TARGET_PORT_GROUP_SUPPORT_MASK); }
			set { this.byte5 = Bits.PutValueMask(this.byte5, (byte)value, 4, TARGET_PORT_GROUP_SUPPORT_MASK); }
		}
		public bool ThirdPartyCopy { get { return Bits.GetBit(this.byte5, 3); } set { this.byte5 = Bits.SetBit(this.byte5, 3, value); } }
		public bool Protect { get { return Bits.GetBit(this.byte5, 0); } set { this.byte5 = Bits.SetBit(this.byte5, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		public bool BasicQueuing { get { return Bits.GetBit(this.byte6, 7); } set { this.byte6 = Bits.SetBit(this.byte6, 7, value); } }
		public bool EnclosureServices { get { return Bits.GetBit(this.byte6, 6); } set { this.byte6 = Bits.SetBit(this.byte6, 6, value); } }
		public bool MultiPort { get { return Bits.GetBit(this.byte6, 4); } set { this.byte6 = Bits.SetBit(this.byte6, 4, value); } }
		public bool MediumChanger { get { return Bits.GetBit(this.byte6, 3); } set { this.byte6 = Bits.SetBit(this.byte6, 3, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
		public bool RelativeAddressing { get { return Bits.GetBit(this.byte7, 7); } set { this.byte7 = Bits.SetBit(this.byte7, 7, value); } }
		public bool Linked { get { return Bits.GetBit(this.byte7, 3); } set { this.byte7 = Bits.SetBit(this.byte7, 3, value); } }
		public bool CommandQueuing { get { return Bits.GetBit(this.byte7, 1); } set { this.byte7 = Bits.SetBit(this.byte7, 1, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
		private string _VendorIdentification;
		public string VendorIdentification { get { return this._VendorIdentification; } set { this._VendorIdentification = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		private string _ProductIdentification;
		public string ProductIdentification { get { return this._ProductIdentification; } set { this._ProductIdentification = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
		private string _ProductRevisionLevel;
		public string ProductRevisionLevel { get { return this._ProductRevisionLevel; } set { this._ProductRevisionLevel = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		private byte[] _VendorSpecific = new byte[20];
		public byte[] VendorSpecific { get { return this._VendorSpecific; } set { this._VendorSpecific = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte56;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte57;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private VersionDescriptorCollection _VersionDescriptors;
		public VersionDescriptorCollection VersionDescriptors { get { return this._VersionDescriptors; } set { this._VersionDescriptors = value; } }
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
		private byte[] reserved74To95;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private byte[] _VendorSpecificParameters = new byte[0];
		public byte[] VendorSpecificParameters { get { return this._VendorSpecificParameters; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.byte1 = buffer.Read<byte>(BYTE1_OFFSET);
			this._Version = buffer.Read<DeviceVersion>(VERSION_OFFSET);
			this.byte3 = buffer.Read<byte>(BYTE3_OFFSET);
			this._AdditionalLength = buffer.Read<byte>(ADDITIONAL_LENGTH_OFFSET);
			if (this._AdditionalLength > (int)BYTE5_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { this.byte5 = buffer.Read<byte>(BYTE5_OFFSET); }
			if (this._AdditionalLength > (int)BYTE6_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { this.byte6 = buffer.Read<byte>(BYTE6_OFFSET); }
			if (this._AdditionalLength > (int)BYTE7_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { this.byte7 = buffer.Read<byte>(BYTE7_OFFSET); }
			if (this._AdditionalLength > (int)VENDOR_IDENTIFICATION_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { this._VendorIdentification = buffer.ToStringAnsi((int)VENDOR_IDENTIFICATION_OFFSET, 8).TrimEnd(TRIM_CHARS); }
			if (this._AdditionalLength > (int)PRODUCT_IDENTIFICATION_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { this._ProductIdentification = buffer.ToStringAnsi((int)PRODUCT_IDENTIFICATION_OFFSET, 16).TrimEnd(TRIM_CHARS); }
			if (this._AdditionalLength > (int)PRODUCT_REVISION_LEVEL_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { this._ProductRevisionLevel = buffer.ToStringAnsi((int)PRODUCT_REVISION_LEVEL_OFFSET, 4).TrimEnd(TRIM_CHARS); }
			if (this._AdditionalLength > (int)VENDOR_SPECIFIC_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET)
			{
				this._VendorSpecific = new byte[20];
				buffer.CopyTo((int)VENDOR_SPECIFIC_OFFSET, this._VendorSpecific, 0, this._VendorSpecific.Length);
			}
			if (this._AdditionalLength > (int)BYTE56_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { this.byte56 = buffer.Read<byte>(BYTE56_OFFSET); }
			if (this._AdditionalLength > (int)BYTE57_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { this.byte57 = buffer.Read<byte>(BYTE57_OFFSET); }
			if (this._AdditionalLength > (int)VERSION_DESCRIPTORS_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { this._VersionDescriptors = Marshaler.PtrToStructure<VersionDescriptorCollection>(buffer.ExtractSegment(VERSION_DESCRIPTORS_OFFSET)); }
			if (this._AdditionalLength > (int)RESERVED74_TO95_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET)
			{
				this.reserved74To95 = new byte[Math.Min(22, buffer.Length32 - (int)RESERVED74_TO95_OFFSET)];
				buffer.CopyTo((int)RESERVED74_TO95_OFFSET, this.reserved74To95, 0, this.reserved74To95.Length);
			}
			int vendorSpecificParamsLength = (int)ADDITIONAL_LENGTH_END_OFFSET + this._AdditionalLength - (int)VENDOR_SPECIFIC_PARAMETERS_OFFSET;
			if (vendorSpecificParamsLength > 0)
			{
				this._VendorSpecificParameters = new byte[vendorSpecificParamsLength];
				buffer.CopyTo(VENDOR_SPECIFIC_PARAMETERS_OFFSET, this._VendorSpecificParameters, IntPtr.Zero, (IntPtr)this._VendorSpecificParameters.Length);
			}
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.byte1, BYTE1_OFFSET);
			buffer.Write(this._Version, VERSION_OFFSET);
			buffer.Write(this.byte3, BYTE3_OFFSET);
			buffer.Write(this._AdditionalLength, ADDITIONAL_LENGTH_OFFSET);
			if (this._AdditionalLength > (int)BYTE5_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { buffer.Write(this.byte5, BYTE5_OFFSET); }
			if (this._AdditionalLength > (int)BYTE6_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { buffer.Write(this.byte6, BYTE6_OFFSET); }
			if (this._AdditionalLength > (int)BYTE7_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { buffer.Write(this.byte7, BYTE7_OFFSET); }
			if (this._AdditionalLength > (int)VENDOR_IDENTIFICATION_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { StringToPtrAnsi(MaskNull(this._VendorIdentification).PadRight(8, TRIM_CHARS[TRIM_CHARS.Length - 1]), buffer.ExtractSegment((int)VENDOR_IDENTIFICATION_OFFSET, 8)); }
			if (this._AdditionalLength > (int)PRODUCT_IDENTIFICATION_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { StringToPtrAnsi(MaskNull(this._ProductIdentification).PadRight(16, TRIM_CHARS[TRIM_CHARS.Length - 1]), buffer.ExtractSegment((int)PRODUCT_IDENTIFICATION_OFFSET, 16)); }
			if (this._AdditionalLength > (int)PRODUCT_REVISION_LEVEL_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { StringToPtrAnsi(MaskNull(this._ProductRevisionLevel).PadRight(4, TRIM_CHARS[TRIM_CHARS.Length - 1]), buffer.ExtractSegment((int)PRODUCT_REVISION_LEVEL_OFFSET, 4)); }
			if (this._AdditionalLength > (int)VENDOR_SPECIFIC_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { buffer.CopyTo((int)VENDOR_SPECIFIC_OFFSET, this._VendorSpecific, 0, this._VendorSpecific.Length); }
			if (this._AdditionalLength > (int)BYTE56_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { buffer.Write(this.byte56, BYTE56_OFFSET); }
			if (this._AdditionalLength > (int)BYTE57_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { buffer.Write(this.byte57, BYTE57_OFFSET); }
			if (this._AdditionalLength > (int)VERSION_DESCRIPTORS_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { Marshaler.StructureToPtr(this._VersionDescriptors, buffer.ExtractSegment(VERSION_DESCRIPTORS_OFFSET)); }
			if (this._AdditionalLength > (int)RESERVED74_TO95_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { buffer.CopyFrom((int)RESERVED74_TO95_OFFSET, this.reserved74To95, 0, this.reserved74To95.Length); }
			if (this._AdditionalLength > (int)VENDOR_SPECIFIC_PARAMETERS_OFFSET - (int)ADDITIONAL_LENGTH_END_OFFSET) { buffer.CopyFrom(VENDOR_SPECIFIC_PARAMETERS_OFFSET, this._VendorSpecificParameters, IntPtr.Zero, (IntPtr)this._VendorSpecificParameters.Length); }
		}

		private static string MaskNull(string str) { return str == null ? string.Empty : str; }

		private static void StringToPtrAnsi(string str, BufferWithSize buffer) { for (int i = 0; i < str.Length; i++) { buffer[i] = (byte)str[i]; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return (int)ADDITIONAL_LENGTH_END_OFFSET + this._AdditionalLength; } }

		public static byte ReadAdditionalLength(BufferWithSize buffer) { return Bits.BigEndian(buffer.Read<byte>(ADDITIONAL_LENGTH_OFFSET)); }

		//public override string ToString() { return Internal.GenericToString(this, true); }
	}

	public enum DeviceVersion : byte { Nonstandard = 0x00, ScsiPrimaryCommands = 0x03, ScsiPrimaryCommands2 = 0x04, ScsiPrimaryCommands3 = 0x05, ScsiPrimaryCommands4 = 0x06 }

	public enum TargetPortGroupSupport : byte { NoneSupported = 0x00, ImplicitAsymmetricLogicalAccessSupported = 0x01, ExplicitAsymmetricLogicalAccessSupported = 0x02, ImplicitAndExplicitAsymmetricLogicalAccessSupported = 0x03 }
}