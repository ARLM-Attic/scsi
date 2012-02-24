using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public abstract class FormatDescriptorOther : FormatDescriptor
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte FORMAT_TYPE_MASK = 0xFC;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte FORMAT_SUBTYPE_MASK = 0x03;

		protected FormatDescriptorOther(FormatType formatType) : base() { this.FormatType = formatType; }

		//Descriptor starts here
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _NumberOfBlocks;
		/// <summary>Number of addressable blocks, NOT the block length!</summary>
		[Description("Number of addressable blocks, NOT the block length!")]
		public uint NumberOfBlocks { get { return Bits.BigEndian(this._NumberOfBlocks); } set { this._NumberOfBlocks = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte FormatDescriptor_byte4;
		public FormatType FormatType
		{
			get { return (FormatType)Bits.GetValueMask(this.FormatDescriptor_byte4, 2, FORMAT_TYPE_MASK); }
			private set { this.FormatDescriptor_byte4 = Bits.PutValueMask(this.FormatDescriptor_byte4, (byte)value, 2, FORMAT_TYPE_MASK); }
		}
		protected byte FormatSubType
		{
			get { return (byte)Bits.GetValueMask(this.FormatDescriptor_byte4, 0, FORMAT_SUBTYPE_MASK); }
			private set { this.FormatDescriptor_byte4 = Bits.PutValueMask(this.FormatDescriptor_byte4, (byte)value, 0, FORMAT_SUBTYPE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte5; //msb
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte7; //lsb
		protected uint TypeDependentParameter
		{
			get { unchecked { return (uint)this._byte7 | ((uint)this._byte6 << 8) | ((uint)this._byte5 << 16); } }
			set { unchecked { this._byte7 = (byte)(value >> 0); this._byte6 = (byte)(value >> 8); this._byte5 = (byte)(value >> 16); } }
		}

		public override FormatCode FormatCode { get { return FormatCode.Other; } }
	}
}