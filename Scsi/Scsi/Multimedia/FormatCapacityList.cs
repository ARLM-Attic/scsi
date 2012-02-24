using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class FormatCapacityList : IMarshalable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr FORMATTABLE_CAPACITIES_DESCRIPTOR_OFFSET = Marshal.OffsetOf(typeof(FormatCapacityList), "_FormattableCapacityDescriptors");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte DESCRIPTOR_TYPE_MASK = 0x3;

		public FormatCapacityList() { }

		//Begin header
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _CapacityListLength;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public byte CapacityListLength { get { return this._CapacityListLength; } private set { this._CapacityListLength = value; } }

		//Begin current/max capacity descriptor
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _NumberOfBlocks;
		public uint NumberOfBlocks { get { return Bits.BigEndian(this._NumberOfBlocks); } set { this._NumberOfBlocks = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte8;
		public DescriptorType DescriptorType
		{
			get { return (DescriptorType)Bits.GetValueMask(this.byte8, 0, DESCRIPTOR_TYPE_MASK); }
			set { this.byte8 = Bits.PutValueMask(this.byte8, (byte)value, 0, DESCRIPTOR_TYPE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte9; //msb
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte10;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte11; //lsb
		public uint BlockLengthOrSpareAreaSize
		{
			get { unchecked { return (uint)this._byte11 | ((uint)this._byte10 << 8) | ((uint)this._byte9 << 16); } }
			set { unchecked { this._byte11 = (byte)(value >> 0); this._byte10 = (byte)(value >> 8); this._byte9 = (byte)(value >> 16); } }
		}

		//Begin formattable capacity descriptors
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private FormattableCapacityDescriptor[] _FormattableCapacityDescriptors;
		public FormattableCapacityDescriptor[] FormattableCapacityDescriptors
		{
			get { return this._FormattableCapacityDescriptors; }
			set
			{
				this._FormattableCapacityDescriptors = value;
				this.CapacityListLength = (byte)((value == null ? 0 : value.Length * Marshaler.DefaultSizeOf<FormattableCapacityDescriptor>()) + 8);
			}
		}

		protected virtual void MarshalFrom(BufferWithSize buffer)
		{
			Marshaler.DefaultPtrToStructure(buffer, this);
			unsafe
			{
				var formatCapacities = buffer.ExtractSegment(FORMATTABLE_CAPACITIES_DESCRIPTOR_OFFSET);
				this._FormattableCapacityDescriptors = new FormattableCapacityDescriptor[this.CapacityListLength / Marshaler.DefaultSizeOf<FormattableCapacityDescriptor>()];
				for (int i = 0; i < this._FormattableCapacityDescriptors.Length; i++)
				{
					this._FormattableCapacityDescriptors[i] = formatCapacities.Read<FormattableCapacityDescriptor>(i * sizeof(FormattableCapacityDescriptor));
				}
			}
		}
		protected virtual void MarshalTo(BufferWithSize buffer) { throw new InvalidOperationException(); }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected virtual int MarshaledSize { get { return Marshal.SizeOf(this) + (int)this.CapacityListLength; } }

		internal static byte ReadCapacityListLength(IntPtr pBuffer) { unsafe { return *((byte*)pBuffer + 3); } }


		void IMarshalable.MarshalFrom(BufferWithSize buffer) { this.MarshalFrom(buffer); }
		void IMarshalable.MarshalTo(BufferWithSize buffer) { this.MarshalTo(buffer); }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int IMarshalable.MarshaledSize { get { return this.MarshaledSize; } }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct FormattableCapacityDescriptor
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
private const byte FORMAT_TYPE_MASK = 0xFC;
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _NumberOfBlocks;
		public uint NumberOfBlocks { get { return Bits.BigEndian(this._NumberOfBlocks); } set { this._NumberOfBlocks = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		public FormatType FormatType
		{
			get { return (FormatType)Bits.GetValueMask(this.byte4, 2, FORMAT_TYPE_MASK); }
			set { this.byte4 = Bits.PutValueMask(this.byte4, (byte)value, 2, FORMAT_TYPE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte5; //msb
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte7; //lsb
		public uint TypeDependentParameter
		{
			get { unchecked { return (uint)this._byte7 | ((uint)this._byte6 << 8) | ((uint)this._byte5 << 16); } }
			set { unchecked { this._byte7 = (byte)(value >> 0); this._byte6 = (byte)(value >> 8); this._byte5 = (byte)(value >> 16); } }
		}

		public FormatDescriptor TryCreateFormatDescriptor()
		{
			FormatDescriptor descriptor;
			switch (this.FormatType)
			{
				case FormatType.FullFormat:
					descriptor = new FullFormatDescriptor();
					break;
				case FormatType.SpareAreaExpansion:
					descriptor = new SpareAreaExpansionDescriptor();
					break;
				case FormatType.ZoneReformat:
					descriptor = new ZoneReformatDescriptor();
					break;
				case FormatType.ZoneFormat:
					descriptor = new ZoneFormatDescriptor();
					break;
				case FormatType.CDRWOrDvdMinusRWFullFormat:
					descriptor = new CDRWOrDvdMinusRWFullFormatDescriptor();
					break;
				case FormatType.CDRWOrDvdMinusRWGrowSession:
					descriptor = new CDRWOrDvdMinusRWGrowSessionDescriptor();
					break;
				case FormatType.CDRWOrDvdMinusRWAddSession:
					descriptor = new CDRWOrDvdMinusRWAddSessionDescriptor();
					break;
				case FormatType.MountRainierRewritableFormat:
					descriptor = new MountRainierRewritableFullFormatDescriptor();
					break;
				default:
					descriptor = null;
					break;
			}
			return descriptor;
		}

		public override string ToString() { return Internal.GenericToString(this, false); }
	}
}