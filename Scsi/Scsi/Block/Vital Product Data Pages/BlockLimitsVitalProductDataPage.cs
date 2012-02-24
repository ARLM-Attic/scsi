using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;
namespace Scsi.Block
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class BlockLimitsVitalProductDataPage : VitalProductDataInquiryData
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE4_OFFSET = Marshal.OffsetOf(typeof(BlockLimitsVitalProductDataPage), "byte4");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE5_OFFSET = Marshal.OffsetOf(typeof(BlockLimitsVitalProductDataPage), "byte5");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr OPTIMAL_TRANSFER_BLOCK_COUNT_GRANULARITY_OFFSET = Marshal.OffsetOf(typeof(BlockLimitsVitalProductDataPage), "_OptimalTransferBlockCountGranularity");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr MAXIMUM_TRANSFER_BLOCK_COUNT_OFFSET = Marshal.OffsetOf(typeof(BlockLimitsVitalProductDataPage), "_MaximumTransferBlockCount");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr OPTIMAL_TRANSFER_BLOCK_COUNT_OFFSET = Marshal.OffsetOf(typeof(BlockLimitsVitalProductDataPage), "_OptimalTransferBlockCount");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr MAXIMUM_PREFETCH_X_TRANSFER_BLOCK_COUNT_OFFSET = Marshal.OffsetOf(typeof(BlockLimitsVitalProductDataPage), "_MaximumPrefetchXTransferBlockCount");

		public BlockLimitsVitalProductDataPage() : base(VitalProductDataPageCode.BlockLimits) { }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _OptimalTransferBlockCountGranularity;
		public ushort OptimalTransferBlockCountGranularity { get { return Bits.BigEndian(this._OptimalTransferBlockCountGranularity); } set { this._OptimalTransferBlockCountGranularity = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _MaximumTransferBlockCount;
		public uint MaximumTransferBlockCount { get { return Bits.BigEndian(this._MaximumTransferBlockCount); } set { this._MaximumTransferBlockCount = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _OptimalTransferBlockCount;
		public uint OptimalTransferBlockCount { get { return Bits.BigEndian(this._OptimalTransferBlockCount); } set { this._OptimalTransferBlockCount = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _MaximumPrefetchXTransferBlockCount;
		public uint MaximumPrefetchXTransferBlockCount { get { return Bits.BigEndian(this._MaximumPrefetchXTransferBlockCount); } set { this._MaximumPrefetchXTransferBlockCount = Bits.BigEndian(value); } }

		protected override void UpdatePageLength() { this.PageLength = (ushort)(Marshaler.DefaultSizeOf<BlockLimitsVitalProductDataPage>() - base.MarshaledSize); }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { this.UpdatePageLength(); return base.MarshaledSize + this.PageLength; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.byte4 = buffer.Read<byte>(BYTE4_OFFSET);
			this.byte5 = buffer.Read<byte>(BYTE5_OFFSET);
			this._OptimalTransferBlockCountGranularity = buffer.Read<ushort>(OPTIMAL_TRANSFER_BLOCK_COUNT_GRANULARITY_OFFSET);
			this._MaximumTransferBlockCount = buffer.Read<uint>(MAXIMUM_TRANSFER_BLOCK_COUNT_OFFSET);
			this._OptimalTransferBlockCount = buffer.Read<uint>(OPTIMAL_TRANSFER_BLOCK_COUNT_OFFSET);
			this._MaximumPrefetchXTransferBlockCount = buffer.Read<uint>(MAXIMUM_PREFETCH_X_TRANSFER_BLOCK_COUNT_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.byte4, BYTE4_OFFSET);
			buffer.Write(this.byte5, BYTE5_OFFSET);
			buffer.Write(this._OptimalTransferBlockCountGranularity, OPTIMAL_TRANSFER_BLOCK_COUNT_GRANULARITY_OFFSET);
			buffer.Write(this._MaximumTransferBlockCount, MAXIMUM_TRANSFER_BLOCK_COUNT_OFFSET);
			buffer.Write(this._OptimalTransferBlockCount, OPTIMAL_TRANSFER_BLOCK_COUNT_OFFSET);
			buffer.Write(this._MaximumPrefetchXTransferBlockCount, MAXIMUM_PREFETCH_X_TRANSFER_BLOCK_COUNT_OFFSET);
		}

	}
}