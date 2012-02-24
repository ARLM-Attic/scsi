using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class CachingModePage : ModePage
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE2_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "byte2");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE3_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "byte3");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr DISABLE_PREFETCH_TRANSFER_BLOCK_COUNT_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "_DisablePrefetchTransferBlockCount");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr MINIMUM_PREFETCH_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "_MinimumPrefetch");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr MAXIMUM_PREFETCH_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "_MaximumPrefetch");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr MAXIMUM_PREFETCH_CEILING_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "_MaximumPrefetchCeiling");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE12_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "byte12");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr NUMBER_OF_CACHE_SEGMENTS_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "_NumberOfCacheSegments");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr CACHE_SEGMENT_SIZE_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "_CacheSegmentSize");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE16_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "byte16");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE17_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "_byte17");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE18_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "_byte18");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE19_OFFSET = Marshal.OffsetOf(typeof(CachingModePage), "_byte19");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte DEMAND_READ_RETENTION_PRIORITY_MASK = 0xF0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte DEMAND_WRITE_RETENTION_PRIORITY_MASK = 0x0F;

		public CachingModePage() : base(ModePageCode.Caching) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		public bool InitiatorControl { get { return Bits.GetBit(this.byte2, 7); } set { this.byte2 = Bits.SetBit(this.byte2, 7, value); } }
		public bool AbortPrefetch { get { return Bits.GetBit(this.byte2, 6); } set { this.byte2 = Bits.SetBit(this.byte2, 6, value); } }
		public bool CacheAnalysisPermitted { get { return Bits.GetBit(this.byte2, 5); } set { this.byte2 = Bits.SetBit(this.byte2, 5, value); } }
		public bool Discontinuity { get { return Bits.GetBit(this.byte2, 4); } set { this.byte2 = Bits.SetBit(this.byte2, 4, value); } }
		public bool SizeEnable { get { return Bits.GetBit(this.byte2, 3); } set { this.byte2 = Bits.SetBit(this.byte2, 3, value); } }
		public bool WritebackCacheEnable { get { return Bits.GetBit(this.byte2, 2); } set { this.byte2 = Bits.SetBit(this.byte2, 2, value); } }
		public bool MultiplicationFactor { get { return Bits.GetBit(this.byte2, 1); } set { this.byte2 = Bits.SetBit(this.byte2, 1, value); } }
		public bool ReadCacheDisable { get { return Bits.GetBit(this.byte2, 0); } set { this.byte2 = Bits.SetBit(this.byte2, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		public DemandReadRetentionPriority DemandReadRetentionPriority
		{
			get { return (DemandReadRetentionPriority)Bits.GetValueMask(this.byte3, 4, DEMAND_READ_RETENTION_PRIORITY_MASK); }
			set { this.byte3 = Bits.PutValueMask(this.byte3, (byte)value, 4, DEMAND_READ_RETENTION_PRIORITY_MASK); }
		}
		public DemandWriteRetentionPriority DemandWriteRetentionPriority
		{
			get { return (DemandWriteRetentionPriority)Bits.GetValueMask(this.byte3, 4, DEMAND_WRITE_RETENTION_PRIORITY_MASK); }
			set { this.byte3 = Bits.PutValueMask(this.byte3, (byte)value, 4, DEMAND_WRITE_RETENTION_PRIORITY_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _DisablePrefetchTransferBlockCount;
		public ushort DisablePrefetchTransferBlockCount { get { return Bits.BigEndian(this._DisablePrefetchTransferBlockCount); } set { this._DisablePrefetchTransferBlockCount = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _MinimumPrefetch;
		public ushort MinimumPrefetch { get { return Bits.BigEndian(this._MinimumPrefetch); } set { this._MinimumPrefetch = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _MaximumPrefetch;
		public ushort MaximumPrefetch { get { return Bits.BigEndian(this._MaximumPrefetch); } set { this._MaximumPrefetch = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _MaximumPrefetchCeiling;
		public ushort MaximumPrefetchCeiling { get { return Bits.BigEndian(this._MaximumPrefetchCeiling); } set { this._MaximumPrefetchCeiling = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte12;
		public bool ForceSequentialWrite { get { return Bits.GetBit(this.byte12, 7); } set { this.byte12 = Bits.SetBit(this.byte12, 7, value); } }
		public bool LogicalBlockCacheSegmentSize { get { return Bits.GetBit(this.byte12, 6); } set { this.byte12 = Bits.SetBit(this.byte12, 6, value); } }
		public bool DisableReadAhead { get { return Bits.GetBit(this.byte12, 5); } set { this.byte12 = Bits.SetBit(this.byte12, 5, value); } }
		public bool NonVolatileCacheDisabled { get { return Bits.GetBit(this.byte12, 0); } set { this.byte12 = Bits.SetBit(this.byte12, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _NumberOfCacheSegments;
		public byte NumberOfCacheSegments { get { return this._NumberOfCacheSegments; } set { this._NumberOfCacheSegments = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _CacheSegmentSize;
		public ushort CacheSegmentSize { get { return Bits.BigEndian(this._CacheSegmentSize); } set { this._CacheSegmentSize = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#pragma warning disable 0169
		private byte byte16;
#pragma warning restore 0169
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte17; //MSB
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte18;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte19; //LSB
		[Obsolete]
		public uint NonCacheSegmentSize
		{
			get { unchecked { return (uint)this._byte19 | ((uint)this._byte18 << 8) | ((uint)this._byte17 << 16); } }
			set { unchecked { this._byte19 = (byte)(value >> 0); this._byte18 = (byte)(value >> 8); this._byte17 = (byte)(value >> 16); } }
		}

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.byte2 = buffer.Read<byte>(BYTE2_OFFSET);
			this.byte3 = buffer.Read<byte>(BYTE3_OFFSET);
			this._DisablePrefetchTransferBlockCount = buffer.Read<ushort>(DISABLE_PREFETCH_TRANSFER_BLOCK_COUNT_OFFSET);
			this._MinimumPrefetch = buffer.Read<ushort>(MINIMUM_PREFETCH_OFFSET);
			this._MaximumPrefetch = buffer.Read<ushort>(MAXIMUM_PREFETCH_OFFSET);
			this._MaximumPrefetchCeiling = buffer.Read<ushort>(MAXIMUM_PREFETCH_CEILING_OFFSET);
			this.byte12 = buffer.Length32 >= (int)BYTE12_OFFSET ? buffer.Read<byte>(BYTE12_OFFSET) : (byte)0;
			this._NumberOfCacheSegments = buffer.Length32 >= (int)NUMBER_OF_CACHE_SEGMENTS_OFFSET ? buffer.Read<byte>(NUMBER_OF_CACHE_SEGMENTS_OFFSET) : (byte)0;
			this._CacheSegmentSize = buffer.Length32 >= (int)CACHE_SEGMENT_SIZE_OFFSET ? buffer.Read<ushort>(CACHE_SEGMENT_SIZE_OFFSET) : (byte)0;
			this.byte16 = buffer.Length32 >= (int)BYTE16_OFFSET ? buffer.Read<byte>(BYTE16_OFFSET) : (byte)0;
			this._byte17 = buffer.Length32 >= (int)BYTE17_OFFSET ? buffer.Read<byte>(BYTE17_OFFSET) : (byte)0;
			this._byte18 = buffer.Length32 >= (int)BYTE18_OFFSET ? buffer.Read<byte>(BYTE18_OFFSET) : (byte)0;
			this._byte19 = buffer.Length32 >= (int)BYTE19_OFFSET ? buffer.Read<byte>(BYTE19_OFFSET) : (byte)0;
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.byte2, BYTE2_OFFSET);
			buffer.Write(this.byte3, BYTE3_OFFSET);
			buffer.Write(this._DisablePrefetchTransferBlockCount, DISABLE_PREFETCH_TRANSFER_BLOCK_COUNT_OFFSET);
			buffer.Write(this._MinimumPrefetch, MINIMUM_PREFETCH_OFFSET);
			buffer.Write(this._MaximumPrefetch, MAXIMUM_PREFETCH_OFFSET);
			buffer.Write(this._MaximumPrefetchCeiling, MAXIMUM_PREFETCH_CEILING_OFFSET);
			if (buffer.Length32 >= (int)BYTE12_OFFSET) { buffer.Write(this.byte12, BYTE12_OFFSET); }
			if (buffer.Length32 >= (int)NUMBER_OF_CACHE_SEGMENTS_OFFSET) { buffer.Write(this._NumberOfCacheSegments, NUMBER_OF_CACHE_SEGMENTS_OFFSET); }
			if (buffer.Length32 >= (int)CACHE_SEGMENT_SIZE_OFFSET) { buffer.Write(this._CacheSegmentSize, CACHE_SEGMENT_SIZE_OFFSET); }
			if (buffer.Length32 >= (int)BYTE16_OFFSET) { buffer.Write(this.byte16, BYTE16_OFFSET); }
			if (buffer.Length32 >= (int)BYTE17_OFFSET) { buffer.Write(this._byte17, BYTE17_OFFSET); }
			if (buffer.Length32 >= (int)BYTE18_OFFSET) { buffer.Write(this._byte18, BYTE18_OFFSET); }
			if (buffer.Length32 >= (int)BYTE19_OFFSET) { buffer.Write(this._byte19, BYTE19_OFFSET); }
		}

		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<CachingModePage>(); } }
	}

	public enum DemandReadRetentionPriority : byte
	{
		NoDistinction = 0x0,
		LowerReadPriority = 0x1,
		ZeroReadPriority = 0xF,
	}

	public enum DemandWriteRetentionPriority : byte
	{
		NoDistinction = 0x0,
		LowerWritePriority = 0x1,
		ZeroWritePriority = 0xF,
	}
}