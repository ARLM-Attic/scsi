using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public abstract class VitalProductDataInquiryData : InquiryData
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr PAGE_CODE_OFFSET = Marshal.OffsetOf(typeof(VitalProductDataInquiryData), "_PageCode");
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr PAGE_LENGTH_OFFSET = Marshal.OffsetOf(typeof(VitalProductDataInquiryData), "_PageLength");

		protected VitalProductDataInquiryData(VitalProductDataPageCode pageCode) : base() { this.PageCode = pageCode; }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private VitalProductDataPageCode _PageCode;
		public VitalProductDataPageCode PageCode { get { return this._PageCode; } private set { this._PageCode = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _PageLength;
		public ushort PageLength { get { return Bits.BigEndian(this._PageLength); } protected set { this._PageLength = Bits.BigEndian(value); } }

		internal static ushort ReadPageLength(BufferWithSize buffer) { return buffer.Read<ushort>(PAGE_LENGTH_OFFSET); }

		protected abstract void UpdatePageLength();

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._PageCode = buffer.Read<VitalProductDataPageCode>(PAGE_CODE_OFFSET);
			this._PageLength = buffer.Read<ushort>(PAGE_LENGTH_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			this.UpdatePageLength();
			base.MarshalTo(buffer);
			buffer.Write(this._PageCode, PAGE_CODE_OFFSET);
			buffer.Write(this._PageLength, PAGE_LENGTH_OFFSET);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<VitalProductDataInquiryData>(); } }

		public static VitalProductDataInquiryData CreateInstance(VitalProductDataPageCode pageCode)
		{
			var result = TryCreateInstance(pageCode);
			if (result == null) { throw new ArgumentOutOfRangeException("pageCode", pageCode, "Unsupported page code."); }
			return result;
		}

		public static VitalProductDataInquiryData TryCreateInstance(VitalProductDataPageCode operationCode)
		{
			VitalProductDataInquiryData result;
			switch (operationCode)
			{
				case VitalProductDataPageCode.SupportedVitalProductDataPages:
					result = new SupportedVitalProductDataPagesDataPage();
					break;
				case VitalProductDataPageCode.UnitSerialNumber:
					result = new UnitSerialNumberDataPage();
					break;
				case VitalProductDataPageCode.BlockLimits:
					result = new Block.BlockLimitsVitalProductDataPage();
					break;
				case VitalProductDataPageCode.DeviceInformation:
					result = new DeviceIdentificationInquiryDataPage();
					break;
				default:
					result = null;
					break;
			}
			return result;
		}

		public static VitalProductDataInquiryData FromBuffer(IntPtr pBuffer, int bufferLength)
		{
			var buffer = new BufferWithSize(pBuffer, bufferLength);
			var instance = CreateInstance(buffer.Read<VitalProductDataPageCode>(PAGE_CODE_OFFSET));
			Marshaler.PtrToStructure(buffer, ref instance);
			return instance;
		}

		public static VitalProductDataInquiryData TryFromBuffer(IntPtr pBuffer, int bufferLength)
		{
			var buffer = new BufferWithSize(pBuffer, bufferLength);
			var instance = TryCreateInstance(buffer.Read<VitalProductDataPageCode>(PAGE_CODE_OFFSET));
			if (instance != null) { Marshaler.PtrToStructure(buffer, ref instance); }
			return instance;
		}
	}

	public enum VitalProductDataPageCode : byte
	{
		SupportedVitalProductDataPages = 0x00,
		UnitSerialNumber = 0x80,
		DeviceInformation = 0x83,
		SoftwareInterfaceIdentification = 0x84,
		ManagementNetworkAddresses = 0x85,
		ExtendedInquiryData = 0x86,
		ModePagePolicy = 0x87,
		ScsiPorts = 0x88,
		AdvancedTechnologyAttachmentInformation = 0x89,
		ProtocolSpecificLogicalUnitInformation = 0x90,
		ProtocolSpecificPortInformation = 0x91,
		BlockLimits = 0xB0,
	}
}