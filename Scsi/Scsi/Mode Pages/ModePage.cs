using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public abstract class ModePage : IMarshalable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE0_OFFSET = Marshal.OffsetOf(typeof(ModePage), "byte0");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr PAGE_LENGTH_OFFSET = Marshal.OffsetOf(typeof(ModePage), "_PageLength");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte PAGE_CODE_MASK = 0x3F;
		protected ModePage(ModePageCode pageCode) : base() { this.PageCode = pageCode; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		/// <summary>For a Mode Sense command, indicates whether the mode page may be saved by the target in a nonvolatile, vendor specific location. Reserved for the Mode Select command.</summary>
		[ReadOnly(true), DisplayName("Parameters Savable"), Description("Whether the mode page may be saved by the target in a nonvolatile, vendor specific location.")]
		public bool ParametersSavable { get { return Bits.GetBit(this.byte0, 7); } set { this.byte0 = Bits.SetBit(this.byte0, 7, value); } }
		[Browsable(false)]
		public ModePageCode PageCode { get { return (ModePageCode)Bits.GetValueMask(this.byte0, 0, PAGE_CODE_MASK); } private set { this.byte0 = Bits.PutValueMask(this.byte0, (byte)value, 0, PAGE_CODE_MASK); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _PageLength;
		[Browsable(false)]
		public byte PageLength { get { return this._PageLength; } protected set { this._PageLength = value; } }

		protected virtual void MarshalFrom(BufferWithSize buffer)
		{
			this.byte0 = buffer.Read<byte>(BYTE0_OFFSET);
			this._PageLength = buffer.Read<byte>(PAGE_LENGTH_OFFSET);
		}

		protected virtual void MarshalTo(BufferWithSize buffer)
		{
			//this.PageLength = (byte)(Marshaler.SizeOf(this) - Marshaler.DefaultSizeOf<ModePage>());
			buffer.Write(this.byte0, BYTE0_OFFSET);
			buffer.Write(this._PageLength, PAGE_LENGTH_OFFSET);
		}

		protected virtual int MarshaledSize { get { return Marshaler.DefaultSizeOf<ModePage>(); } }

#if !DEBUG
		[DebuggerHidden]
#endif
		void IMarshalable.MarshalFrom(BufferWithSize buffer)
		{
#if DEBUG
			//buffer = buffer.ExtractSegment(0, (int)PAGE_LENGTH_OFFSET + sizeof(byte) + buffer.Read<byte>(PAGE_LENGTH_OFFSET));
			var pageCodeBefore = this.PageCode;
#endif
			this.MarshalFrom(buffer);
#if DEBUG
			Debug.Assert(this.PageCode == pageCodeBefore, "The returned mode page's Page Code was different from that requested... something is wrong.", string.Format("Requested page: {0} (0x{0:X})" + Environment.NewLine + "Returned page: {1} (0x{1:X})", pageCodeBefore, this.PageCode));
#endif
		}

		[DebuggerHidden]
		void IMarshalable.MarshalTo(BufferWithSize buffer) { this.MarshalTo(buffer); }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int IMarshalable.MarshaledSize { get { return this.MarshaledSize; } }

		//public override string ToString() { return Internal.GenericToString(this, true); }

		public static ModePage TryCreateInstance(ModePageCode pageCode)
		{
			switch (pageCode)
			{
				case ModePageCode.Control:
					return new ControlModePage();
				case ModePageCode.ReadWriteErrorRecoveryParameters:
					return new ReadWriteErrorRecoveryParametersPage();
				case ModePageCode.WriteParameters:
					return new Multimedia.WriteParametersPage();
				case ModePageCode.Caching:
					return new CachingModePage();
				case ModePageCode.CDDeviceParameters:
					return new Multimedia.CDParametersPage();
				case ModePageCode.CapabilitiesAndMechanicalStatus:
					return new Multimedia.CapabilitiesMechanicalStatusPage();
				case ModePageCode.InformationalExceptionsModePage:
					return new InformationalExceptionsModePage();
				default:
					return null;
			}
		}

		public static ModePage CreateInstance(ModePageCode pageCode) { var result = TryCreateInstance(pageCode); if (result == null) { throw new ArgumentOutOfRangeException("pageCode", pageCode, "Invalid mode page."); } return result; }

		internal static ModePageCode ReadPageCode(IntPtr pBuffer) { unsafe { return *(ModePageCode*)((byte*)pBuffer + (int)BYTE0_OFFSET); } }

		internal static byte ReadPageLength(IntPtr pBuffer) { unsafe { return *(byte*)((byte*)pBuffer + (int)PAGE_LENGTH_OFFSET); } }

		public static ModePage TryFromBuffer(IntPtr pBuffer, int bufferLength) { var mp = TryCreateInstance(ReadPageCode(pBuffer)); if (mp != null) { Marshaler.PtrToStructure(new BufferWithSize(pBuffer, bufferLength), ref mp); } return mp; }
	}

	public enum PageControl : byte
	{
		CurrentValues = 0x00,
		ChangeableValues = 0x01,
		DefaultValues = 0x02,
		SavedValues = 0x03,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Mode06ParametersHeader
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		internal static readonly IntPtr MODE_DATA_LENGTH_OFFSET = Marshal.OffsetOf(typeof(Mode06ParametersHeader), "_ModeDataLength");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _ModeDataLength;
		public byte ModeDataLength { get { return this._ModeDataLength; } set { this._ModeDataLength = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _MediumTypeCode;
		[Obsolete]
		public byte MediumTypeCode { get { return this._MediumTypeCode; } set { this._MediumTypeCode = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _DeviceSpecificParameter;
		public byte DeviceSpecificParameter { get { return this._DeviceSpecificParameter; } set { this._DeviceSpecificParameter = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _BlockDescriptorLength;
		/// <summary>Should be zero for multimedia devices.</summary>
		public byte BlockDescriptorLength { get { return this._BlockDescriptorLength; } set { this._BlockDescriptorLength = value; } }

		public static ModePage TryGetModePageFromBuffer(IntPtr pBuffer, int bufferSize) { var buf = new BufferWithSize(pBuffer, bufferSize); var mpBuf = buf.ExtractSegment(Marshaler.SizeOf(Marshaler.PtrToStructure<Mode06ParametersHeader>(buf))); return ModePage.TryFromBuffer(mpBuf.Address, mpBuf.Length32); }

		/// <remarks>The <see cref="ModeDataLength"/> member is set in this function, but the rest are kept as is.</remarks>
		public byte[] ToBufferWithHeader(params ModePage[] modePages)
		{
			int size = Marshaler.SizeOf(this);
			foreach (var mp in modePages) { size += Marshaler.SizeOf(mp); }
			var bytes = new byte[size];
			this.ModeDataLength = checked((byte)(size - (int)MODE_DATA_LENGTH_OFFSET));
			int offset = Marshaler.StructureToPtr(this, bytes, 0);
			foreach (var mp in modePages) { offset += Marshaler.StructureToPtr(mp, bytes, offset); }
			return bytes;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Mode10ParametersHeader
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		internal static readonly IntPtr MODE_DATA_LENGTH_OFFSET = Marshal.OffsetOf(typeof(Mode10ParametersHeader), "_ModeDataLength");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _ModeDataLength;
		public ushort ModeDataLength { get { return Bits.BigEndian(this._ModeDataLength); } set { this._ModeDataLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _MediumTypeCode;
		[Obsolete]
		public byte MediumTypeCode { get { return this._MediumTypeCode; } set { this._MediumTypeCode = value; } }
		public byte DeviceSpecificParameter;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		public bool LongLogicalBlockAddress { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _BlockDescriptorLength;
		/// <summary>Should be zero for multimedia devices.</summary>
		public ushort BlockDescriptorLength { get { return Bits.BigEndian(this._BlockDescriptorLength); } set { this._BlockDescriptorLength = Bits.BigEndian(value); } }

		public static ModePage TryGetModePageFromBuffer(byte[] buffer, int offset, int length) { unsafe { fixed (byte* pBuf = &buffer[offset]) { return TryGetModePageFromBuffer((IntPtr)pBuf, length != -1 ? length : buffer.Length - offset); } } }

		public static ModePage TryGetModePageFromBuffer(IntPtr pBuffer, int bufferSize) { var buf = new BufferWithSize(pBuffer, bufferSize); var mpBuf = buf.ExtractSegment(Marshaler.SizeOf(Marshaler.PtrToStructure<Mode10ParametersHeader>(buf))); return ModePage.TryFromBuffer(mpBuf.Address, mpBuf.Length32); }

		/// <remarks>The <see cref="ModeDataLength"/> member is set in this function, but the rest are kept as is.</remarks>
		public byte[] ToBufferWithHeader(params ModePage[] modePages)
		{
			int size = Marshaler.SizeOf(this);
			foreach (var mp in modePages) { size += Marshaler.SizeOf(mp); }
			var bytes = new byte[size];
			this.ModeDataLength = checked((byte)(size - (int)MODE_DATA_LENGTH_OFFSET));
			int offset = Marshaler.StructureToPtr(this, bytes, 0);
			foreach (var mp in modePages) { offset += Marshaler.StructureToPtr(mp, bytes, offset); }
			return bytes;
		}
	}

	public enum ModePageCode : byte
	{
		None = 0x00,
		ReadWriteErrorRecoveryParameters = 0x01,
		WriteParameters = 0x05,
		VerifyErrorRecovery = 0x07,
		Caching = 0x08,
		Control = 0x0A,
		MediumTypesSupported = 0x0B,
		CDDeviceParameters = 0x0D,
		CDAudioControl = 0x0E,
		XorControl = 0x10,
		PowerConditions = 0x1A,
		FaultFailureReporting = 0x1C,
		InformationalExceptionsModePage = 0x1C,
		BackgroundControl = 0x1C,
		TimeoutAndProtect = 0x1D,
		CapabilitiesAndMechanicalStatus = 0x2A,
		MountRainierRewritable = 0x2C,
		All = 0x3F,
	}
}