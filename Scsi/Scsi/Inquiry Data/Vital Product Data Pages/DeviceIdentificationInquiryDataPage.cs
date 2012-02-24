using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;
using System.Collections.Generic;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class DeviceIdentificationInquiryDataPage : VitalProductDataInquiryData
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr DESIGNATION_DESCRIPTORS_OFFSET = Marshal.OffsetOf(typeof(DeviceIdentificationInquiryDataPage), "_DesignationDescriptors");

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private DesignationDescriptor[] _DesignationDescriptors;
		public DesignationDescriptor[] DesignationDescriptors { get { return this._DesignationDescriptors; } set { this._DesignationDescriptors = value; } }

		public DeviceIdentificationInquiryDataPage() : base(VitalProductDataPageCode.DeviceInformation) { }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			var items = new List<DesignationDescriptor>();
			var remaining = buffer.ExtractSegment((int)DESIGNATION_DESCRIPTORS_OFFSET, this.PageLength);
			while (remaining.Length != UIntPtr.Zero)
			{
				var current = remaining.ExtractSegment(0, DesignationDescriptor.GetLength(remaining));
				items.Add(Marshaler.PtrToStructure<DesignationDescriptor>(current));
				remaining = remaining.ExtractSegment(current.Length);
			}
			this._DesignationDescriptors = items.ToArray();
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			var remaining = buffer.ExtractSegment(DESIGNATION_DESCRIPTORS_OFFSET);
			for (int i = 0; i < this._DesignationDescriptors.Length; i++)
			{ remaining = remaining.ExtractSegment(Marshaler.StructureToPtr(this._DesignationDescriptors[i], remaining)); }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { this.UpdatePageLength(); return base.MarshaledSize + this.PageLength; } }

		protected override void UpdatePageLength()
		{
			var len = 0;
			for (int i = 0; i < this._DesignationDescriptors.Length; i++) { len += checked((ushort)Marshaler.SizeOf(this._DesignationDescriptors[i])); }
			this.PageLength = (ushort)len;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DesignationDescriptor : IMarshalable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly byte CODE_SET_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly byte PROTOCOL_IDENTIFIER_MASK = 0xF0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly byte DESIGNATOR_TYPE_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly byte ASSOCIATION_MASK = 0x30;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE0_OFFSET = Marshal.OffsetOf(typeof(DesignationDescriptor), "byte0");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE1_OFFSET = Marshal.OffsetOf(typeof(DesignationDescriptor), "byte1");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE2_OFFSET = Marshal.OffsetOf(typeof(DesignationDescriptor), "byte2");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr DESIGNATOR_LENGTH_OFFSET = Marshal.OffsetOf(typeof(DesignationDescriptor), "_DesignatorLength");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr DESIGNATOR_OFFSET = Marshal.OffsetOf(typeof(DesignationDescriptor), "_Designator");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public CodeSet CodeSet { get { return (CodeSet)Bits.GetValueMask(this.byte0, 0, CODE_SET_MASK); } set { this.byte0 = Bits.PutValueMask(this.byte0, (byte)value, 0, CODE_SET_MASK); } }
		public ProtocolIdentifier ProtocolIdentifier { get { return (ProtocolIdentifier)Bits.GetValueMask(this.byte0, 4, PROTOCOL_IDENTIFIER_MASK); } set { this.byte0 = Bits.PutValueMask(this.byte0, (byte)value, 4, PROTOCOL_IDENTIFIER_MASK); } }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte1;
		public DesignatorType DesignatorType { get { return (DesignatorType)Bits.GetValueMask(this.byte1, 0, DESIGNATOR_TYPE_MASK); } set { this.byte1 = Bits.PutValueMask(this.byte1, (byte)value, 0, DESIGNATOR_TYPE_MASK); } }
		public DesignatorAssociation Association { get { return (DesignatorAssociation)Bits.GetValueMask(this.byte1, 4, ASSOCIATION_MASK); } set { this.byte1 = Bits.PutValueMask(this.byte1, (byte)value, 4, ASSOCIATION_MASK); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _DesignatorLength;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private Designator _Designator;
		public Designator Designator { get { return this._Designator; } set { this._Designator = value; this._DesignatorLength = value != null ? checked((byte)Marshaler.SizeOf(value)) : (byte)0; } }

		void IMarshalable.MarshalFrom(BufferWithSize buffer)
		{
			this.byte0 = buffer.Read<byte>(BYTE0_OFFSET);
			this.byte1 = buffer.Read<byte>(BYTE1_OFFSET);
			this.byte2 = buffer.Read<byte>(BYTE2_OFFSET);
			this._DesignatorLength = buffer.Read<byte>(DESIGNATOR_LENGTH_OFFSET);
			this._Designator = Designator.FromBuffer(this.DesignatorType, buffer.ExtractSegment((int)DESIGNATOR_OFFSET, this._DesignatorLength));
		}

		void IMarshalable.MarshalTo(BufferWithSize buffer)
		{
			this._DesignatorLength = checked((byte)Marshaler.SizeOf(this._Designator));
			buffer.Write(this.byte0, BYTE0_OFFSET);
			buffer.Write(this.byte1, BYTE1_OFFSET);
			buffer.Write(this.byte2, BYTE2_OFFSET);
			buffer.Write(this._DesignatorLength, DESIGNATOR_LENGTH_OFFSET);
			Marshaler.StructureToPtr(this._Designator, buffer.ExtractSegment((int)DESIGNATOR_OFFSET, this._DesignatorLength));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int IMarshalable.MarshaledSize { get { return (int)DESIGNATOR_OFFSET + this._DesignatorLength; } }

		internal static int GetLength(BufferWithSize buffer) { return (int)DESIGNATOR_OFFSET + buffer.Read<byte>(DESIGNATOR_LENGTH_OFFSET); }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public abstract class Designator : IMarshalable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected static readonly byte[] EMPTY_BYTES = new byte[0];
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public abstract DesignatorType DesignatorType { get; }

		protected virtual void MarshalFrom(BufferWithSize buffer) { }
		protected virtual void MarshalTo(BufferWithSize buffer) { }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected virtual int MarshaledSize { get { return 0; } }

		void IMarshalable.MarshalFrom(BufferWithSize buffer) { this.MarshalFrom(buffer); }
		void IMarshalable.MarshalTo(BufferWithSize buffer) { this.MarshalTo(buffer); }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int IMarshalable.MarshaledSize { get { return this.MarshaledSize; } }

		internal static Designator FromBuffer(DesignatorType type, BufferWithSize buffer)
		{
			var obj = CreateInstance(type);
			Marshaler.PtrToStructure(buffer, ref obj);
			return obj;
		}

		public static Designator CreateInstance(DesignatorType type)
		{
			var result = TryCreateInstance(type);
			if (result == null) { throw new ArgumentOutOfRangeException("type", type, "Unsupported type."); }
			return result;
		}

		private static Designator TryCreateInstance(DesignatorType type)
		{
			Designator result;
			switch (type)
			{
				case DesignatorType.VendorSpecific:
					result = new VendorSpecificDesignator();
					break;
				case DesignatorType.T10VendorIdBased:
					result = new T10VendorIdBasedDesignator();
					break;
				case DesignatorType.EUI64Based:
					result = new EUI64BasedDesignator();
					break;
				case DesignatorType.NAA:
					result = new NaaDesignator();
					break;
				case DesignatorType.RelativeTargetPortIdentifier:
					result = new RelativeTargetPortDesignator();
					break;
				case DesignatorType.TargetPortGroup:
					result = new TargetPortGroupDesignator();
					break;
				case DesignatorType.LogicalUnitGroup:
					result = new LogicalUnitGroupDesignator();
					break;
				case DesignatorType.MD5LogicalUnitIdentifier:
					result = new Md5LogicalUnitDesignator();
					break;
				case DesignatorType.ScsiNameString:
					result = new ScsiNameStringDesignator();
					break;
				default:
					result = null;
					break;
			}
			return result;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class VendorSpecificDesignator : Designator
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr VENDOR_SPECIFIC_ID_OFFSET = Marshal.OffsetOf(typeof(VendorSpecificDesignator), "_VendorSpecificIdentifier");

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte[] _VendorSpecificIdentifier = EMPTY_BYTES;
		public byte[] VendorSpecificIdentifier { get { return this._VendorSpecificIdentifier; } set { this._VendorSpecificIdentifier = value; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override DesignatorType DesignatorType { get { return DesignatorType.VendorSpecific; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._VendorSpecificIdentifier = buffer.ExtractSegment(VENDOR_SPECIFIC_ID_OFFSET).ToArray();
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			if (this._VendorSpecificIdentifier != null) { buffer.CopyFrom((int)VENDOR_SPECIFIC_ID_OFFSET, this._VendorSpecificIdentifier, 0, this._VendorSpecificIdentifier.Length); }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return base.MarshaledSize + this._VendorSpecificIdentifier.Length; } }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public class T10VendorIdBasedDesignator : Designator
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr T10_VENDOR_ID_OFFSET = Marshal.OffsetOf(typeof(T10VendorIdBasedDesignator), "_T10VendorIdentification");
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr VENDOR_SPECIFIC_ID_OFFSET = Marshal.OffsetOf(typeof(T10VendorIdBasedDesignator), "_VendorSpecificIdentifier");

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
		private string _T10VendorIdentification = "        ";
		public string T10VendorIdentification { get { return this._T10VendorIdentification; } set { this._T10VendorIdentification = value.PadRight(8, ' '); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte[] _VendorSpecificIdentifier = EMPTY_BYTES;
		public byte[] VendorSpecificIdentifier { get { return this._VendorSpecificIdentifier; } set { this._VendorSpecificIdentifier = value; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override DesignatorType DesignatorType { get { return DesignatorType.T10VendorIdBased; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._T10VendorIdentification = buffer.ToStringAnsi((int)T10_VENDOR_ID_OFFSET, 8);
			this._VendorSpecificIdentifier = buffer.ExtractSegment(VENDOR_SPECIFIC_ID_OFFSET).ToArray();
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			Marshaler.StringToPtrAnsi(this._T10VendorIdentification, buffer.ExtractSegment((int)T10_VENDOR_ID_OFFSET, 8).Address);
			buffer.CopyFrom((int)VENDOR_SPECIFIC_ID_OFFSET, this._VendorSpecificIdentifier, 0, this._VendorSpecificIdentifier.Length);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return base.MarshaledSize + 8 * sizeof(byte) + this._VendorSpecificIdentifier.Length; } }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class EUI64BasedDesignator : Designator
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr FULL_ID_OFFSET = Marshal.OffsetOf(typeof(EUI64BasedDesignator), "_FullId");

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ulong _FullId;
		//NOTE: Keep this unsigned, since the shifting below can become messed up if this is signed!
		public ulong FullId { get { return Bits.BigEndian(this._FullId); } set { this._FullId = Bits.BigEndian(value); } }

		public uint IeeeCompanyId { get { return (uint)((this.FullId & 0xFFFFFF0000000000) >> (8 * 5)); } set { this.FullId = (this.FullId & ~0xFFFFFF0000000000) | ((value << (8 * 5)) & 0xFFFFFF0000000000); } }
		public ulong VendorSpecificExtensionId { get { return this.FullId & 0x000000FFFFFFFFFF; } set { this.FullId = (this.FullId & ~0x000000FFFFFFFFFFU) | (value & 0x000000FFFFFFFFFF); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override DesignatorType DesignatorType { get { return DesignatorType.EUI64Based; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._FullId = buffer.Read<ulong>(FULL_ID_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this._FullId, FULL_ID_OFFSET);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return base.MarshaledSize + 3 + 5; } }
	}

	//TODO: Subclass NaaDesignator for specific NAA types...
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class NaaDesignator : Designator
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private const byte NAA_MASK = 0xF0;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr NAA_SPECIFIC_DATA_OFFSET = Marshal.OffsetOf(typeof(NaaDesignator), "_NaaSpecificData");

		public NamingAddressAuthority NamingAddressAuthority { get { return (NamingAddressAuthority)Bits.GetValueMask(this._NaaSpecificData[0], 4, NAA_MASK); } set { this._NaaSpecificData[0] = Bits.PutValueMask(this._NaaSpecificData[0], (byte)value, 4, NAA_MASK); } }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte[] _NaaSpecificData = new byte[1];
		public byte[] NaaSpecificData { get { return this._NaaSpecificData; } set { if (value.Length < 1) { throw new ArgumentException("Data must have a nonzero, positive length."); } this._NaaSpecificData = value; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override DesignatorType DesignatorType { get { return DesignatorType.NAA; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._NaaSpecificData = buffer.ExtractSegment(NAA_SPECIFIC_DATA_OFFSET).ToArray();
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.CopyFrom((int)NAA_SPECIFIC_DATA_OFFSET, this._NaaSpecificData, 0, this._NaaSpecificData.Length);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return base.MarshaledSize + this._NaaSpecificData.Length; } }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class RelativeTargetPortDesignator : Designator
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr OBSOLETE_FIELD_OFFSET = Marshal.OffsetOf(typeof(RelativeTargetPortDesignator), "_ObsoleteField");
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr RELATIVE_TARGET_PORT_ID_OFFSET = Marshal.OffsetOf(typeof(RelativeTargetPortDesignator), "_RelativeTargetPortIdentifier");

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ushort _ObsoleteField;
		[Obsolete]
		public ushort ObsoleteField { get { return Bits.BigEndian(this._ObsoleteField); } set { this._ObsoleteField = Bits.BigEndian(value); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ushort _RelativeTargetPortIdentifier;
		public ushort RelativeTargetPortIdentifier { get { return Bits.BigEndian(this._RelativeTargetPortIdentifier); } set { this._RelativeTargetPortIdentifier = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override DesignatorType DesignatorType { get { return DesignatorType.RelativeTargetPortIdentifier; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._ObsoleteField = buffer.Read<ushort>(OBSOLETE_FIELD_OFFSET);
			this._RelativeTargetPortIdentifier = buffer.Read<ushort>(RELATIVE_TARGET_PORT_ID_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this._ObsoleteField, OBSOLETE_FIELD_OFFSET);
			buffer.Write(this._RelativeTargetPortIdentifier, RELATIVE_TARGET_PORT_ID_OFFSET);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return base.MarshaledSize + sizeof(ushort) + sizeof(ushort); } }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class TargetPortGroupDesignator : Designator
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr RESERVED_OFFSET = Marshal.OffsetOf(typeof(TargetPortGroupDesignator), "_Reserved");
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr TARGET_PORT_GROUP_OFFSET = Marshal.OffsetOf(typeof(TargetPortGroupDesignator), "_TargetPortGroup");

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ushort _Reserved;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ushort _TargetPortGroup;
		public ushort TargetPortGroup { get { return Bits.BigEndian(this._TargetPortGroup); } set { this._TargetPortGroup = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override DesignatorType DesignatorType { get { return DesignatorType.TargetPortGroup; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._Reserved = buffer.Read<ushort>(RESERVED_OFFSET);
			this._TargetPortGroup = buffer.Read<ushort>(TARGET_PORT_GROUP_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this._Reserved, RESERVED_OFFSET);
			buffer.Write(this._TargetPortGroup, TARGET_PORT_GROUP_OFFSET);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return base.MarshaledSize + sizeof(ushort) + sizeof(ushort); } }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class LogicalUnitGroupDesignator : Designator
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr RESERVED_OFFSET = Marshal.OffsetOf(typeof(LogicalUnitGroupDesignator), "_Reserved");
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr LOGICAL_UNIT_OFFSET = Marshal.OffsetOf(typeof(LogicalUnitGroupDesignator), "_LogicalUnitGroup");

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ushort _Reserved;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ushort _LogicalUnitGroup;
		public ushort LogicalUnitGroup { get { return Bits.BigEndian(this._LogicalUnitGroup); } set { this._LogicalUnitGroup = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override DesignatorType DesignatorType { get { return DesignatorType.LogicalUnitGroup; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._Reserved = buffer.Read<ushort>(RESERVED_OFFSET);
			this._LogicalUnitGroup = buffer.Read<ushort>(LOGICAL_UNIT_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this._Reserved, RESERVED_OFFSET);
			buffer.Write(this._LogicalUnitGroup, LOGICAL_UNIT_OFFSET);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return base.MarshaledSize + sizeof(ushort) + sizeof(ushort); } }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Md5LogicalUnitDesignator : Designator
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr MD5_LOGICAL_UNIT_ID_OFFSET = Marshal.OffsetOf(typeof(Md5LogicalUnitDesignator), "_Md5LogicalUnitId");

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private Guid _Md5LogicalUnitId;
		/// <summary>This is not really a GUID; I just used that type because it's a convenient 16-byte data type.</summary>
		public Guid Md5LogicalUnitId { get { return Bits.BigEndian(this._Md5LogicalUnitId); } set { this._Md5LogicalUnitId = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override DesignatorType DesignatorType { get { return DesignatorType.LogicalUnitGroup; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._Md5LogicalUnitId = buffer.Read<Guid>(MD5_LOGICAL_UNIT_ID_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this._Md5LogicalUnitId, MD5_LOGICAL_UNIT_ID_OFFSET);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return base.MarshaledSize + Marshaler.DefaultSizeOf<Guid>(); } }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ScsiNameStringDesignator : Designator
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SCSI_NAME_STRING_OFFSET = Marshal.OffsetOf(typeof(T10VendorIdBasedDesignator), "_ScsiNameString");

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte[] _ScsiNameString = EMPTY_BYTES;
		/// <summary>This property MUST represent a null-terminated, null-padded UTF-8 string; it must have a multiple of four bytes. I'm too lazy to implement the strings right now.</summary>
		public byte[] ScsiNameString { get { return this._ScsiNameString; } set { this._ScsiNameString = value; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override DesignatorType DesignatorType { get { return DesignatorType.ScsiNameString; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._ScsiNameString = buffer.ExtractSegment(SCSI_NAME_STRING_OFFSET).ToArray();
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.CopyFrom((int)SCSI_NAME_STRING_OFFSET, this._ScsiNameString, 0, this._ScsiNameString.Length);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return base.MarshaledSize + this._ScsiNameString.Length; } }
	}


	public enum DesignatorType : byte
	{
		VendorSpecific = 0x00,
		T10VendorIdBased = 0x01,
		EUI64Based = 0x02,
		NAA = 0x03,
		RelativeTargetPortIdentifier = 0x04,
		TargetPortGroup = 0x05,
		LogicalUnitGroup = 0x06,
		MD5LogicalUnitIdentifier = 0x07,
		ScsiNameString = 0x08,
	}

	public enum NamingAddressAuthority : byte { IeeeExtended = 2, LocallyAssigned = 3, IeeeRegistered = 5, IeeeRegisteredExtended = 6 }

	public enum DesignatorAssociation : byte { AddressedLogicalUnit = 0, TargetPort = 1, ScsiTargetDevice = 2 }
	
	public enum CodeSet : byte { Reserved0 = 0, Binary = 1, AsciiPrintable = 2, Utf8 = 3, }

	public enum ProtocolIdentifier : byte 
	{
		FiberChannel = 0x00,
		ParalellScsi = 0x01,
		SSA = 0x02,
		IEEE1394 = 0x03,
		ScsiRemoteDmaProtocol = 0x04,
		InternetScsi = 0x05,
		SasSerialScsiProtocol = 0x06,
		AutomationOrDriveInterfaceTransportProtocol = 0x07,
		AtaInterface = 0x08,
		NoSpecificProtocol = 0x0F,
	}
}