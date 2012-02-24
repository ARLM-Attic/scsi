using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Ata
{
	public class AtaStream : Stream
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool leaveOpen;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private long _Position;

		public AtaStream(AtaDevice device, bool leaveOpen) { this.leaveOpen = leaveOpen; this.AtaDevice = device; }

		public AtaDevice AtaDevice { get; private set; }

		//public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) { return new SynchronousAsyncResult(callback, state, this.Read(buffer, offset, count)); }
		//public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) { this.Write(buffer, offset, count); return new SynchronousAsyncResult(callback, state, count); }

		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return true; } }
		public override bool CanWrite { get { return true; } }

		protected override void Dispose(bool disposing) { try { try { if (!this.leaveOpen) { this.AtaDevice.Dispose(); } } finally { this.AtaDevice = null; } } finally { base.Dispose(disposing); } }

		//public override int EndRead(IAsyncResult asyncResult) { var asAsync = asyncResult as SynchronousAsyncResult; if (asAsync != null) { return asAsync.Count; } else { return base.EndRead(asAsync); } }
		//public override void EndWrite(IAsyncResult asyncResult) { var asAsync = asyncResult as SynchronousAsyncResult; if (asAsync != null) { } else { base.EndWrite(asAsync); } }

		public override void Flush() { this.AtaDevice.FlushCache(); }

		public override long Length { get { return this.AtaDevice.LogicalSectorSize * (this.AtaDevice.Supports48BitLogicalBlockAddressing ? this.AtaDevice.NativeMaximumAddress : this.AtaDevice.NativeMaximumAddressExt); } }
		/// <summary>Use only ONCE per method, to ensure asynchronous calls can work!</summary>
		public override long Position { get { return this._Position; } set { if (value < 0) { throw new ArgumentOutOfRangeException("value", value, "Value must be nonnegative."); } this.Seek(value, SeekOrigin.Begin); } }

		/// <summary>The position must be updated manually!</summary>
		private int Process(long position, byte[] buffer, int bufferOffset, int count, bool write)
		{
			int processed;
			if (count > buffer.Length - bufferOffset) { throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."); }
			var bps = this.AtaDevice.LogicalSectorSize;
			if (position % bps == 0 && count % bps == 0)
			{
				unsafe
				{
					if (this.AtaDevice.Supports48BitLogicalBlockAddressing)
					{
						if (this.AtaDevice.DmaSupported) { this.AtaDevice.ReadDmaExt(position / bps, checked((ushort)(count / bps)), buffer, bufferOffset); }
						else { this.AtaDevice.ReadSectorsExt(position / bps, checked((ushort)(count / bps)), buffer, bufferOffset); }
					}
					else
					{
						if (this.AtaDevice.DmaSupported) { this.AtaDevice.ReadDma(checked((int)(position / bps)), checked((byte)(count / bps)), buffer, bufferOffset); }
						else { this.AtaDevice.ReadSectors(checked((int)(position / bps)), checked((byte)(count / bps)), buffer, bufferOffset); }
					}
					processed = count;
					if (write) { throw new NotSupportedException("Writing not supported; it's too dangerous for me to implement!"); }
				}
			}
			else
			{
				long basePosition = position / bps * bps;
				var alignedData = new byte[(position + count - basePosition + bps - 1) / bps * bps];
				unsafe
				{
					if (this.AtaDevice.Supports48BitLogicalBlockAddressing)
					{
						if (this.AtaDevice.DmaSupported) { this.AtaDevice.ReadDmaExt(basePosition / bps, checked((ushort)(alignedData.Length / bps)), alignedData, 0); }
						else { this.AtaDevice.ReadSectorsExt(basePosition / bps, checked((ushort)(alignedData.Length / bps)), alignedData, 0); }
					}
					else
					{
						if (this.AtaDevice.DmaSupported) { this.AtaDevice.ReadDma(checked((int)(basePosition / bps)), checked((byte)(alignedData.Length / bps)), alignedData, 0); }
						else { this.AtaDevice.ReadSectors(checked((int)(basePosition / bps)), checked((byte)(alignedData.Length / bps)), alignedData, 0); }
					}
					processed = alignedData.Length;
					if (write) { throw new NotSupportedException("Writing not supported; it's too dangerous for me to implement!"); }
				}
				if (!write) { checked { Buffer.BlockCopy(alignedData, (int)(position - basePosition), buffer, bufferOffset, count); } }
			}
			processed = Math.Min(processed, count);
			return processed;
		}

		public override int Read(byte[] buffer, int offset, int count) { var pos = this.Position; int processed = this.Process(pos, buffer, offset, count, false); this.Position = pos + processed; return processed; }

		public override int ReadByte()
		{
			var bps = this.AtaDevice.LogicalSectorSize;
			long position = this.Position;
			byte b;
			long basePosition = position / bps * bps;
			int alignedDataLength = checked((int)((position + 1 - basePosition + bps - 1) / bps * bps));
			unsafe
			{
				byte* pAlignedData = stackalloc byte[alignedDataLength];
				unsafe
				{
					if (this.AtaDevice.Supports48BitLogicalBlockAddressing)
					{
						if (this.AtaDevice.DmaSupported) { this.AtaDevice.ReadDmaExt(basePosition / bps, checked((ushort)(alignedDataLength / bps)), new Helper.BufferWithSize(pAlignedData, alignedDataLength)); }
						else { this.AtaDevice.ReadSectorsExt(basePosition / bps, checked((ushort)(alignedDataLength / bps)), new Helper.BufferWithSize(pAlignedData, alignedDataLength)); }
					}
					else
					{
						if (this.AtaDevice.DmaSupported) { this.AtaDevice.ReadDma(checked((int)(basePosition / bps)), checked((byte)(alignedDataLength / bps)), new Helper.BufferWithSize(pAlignedData, alignedDataLength)); }
						else { this.AtaDevice.ReadSectors(checked((int)(basePosition / bps)), checked((byte)(alignedDataLength / bps)), new Helper.BufferWithSize(pAlignedData, alignedDataLength)); }
					}
				}
				b = pAlignedData[position - basePosition];
			}
			this.Position = position + 1;
			return b;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long newPos;
			if (!this.CanSeek) { throw new InvalidOperationException(); }
			{
				switch (origin)
				{
					case SeekOrigin.Begin:
						newPos = 0 + offset;
						break;
					case SeekOrigin.Current:
						newPos = this._Position + offset;
						break;
					case SeekOrigin.End:
						newPos = this.Length + offset;
						break;
					default:
						throw new ArgumentOutOfRangeException("origin", origin, "Invalid seek origin.");
				}
			}
			this._Position = newPos;
			return newPos;
		}

		public override void SetLength(long value) { throw new NotSupportedException(); }

		public override void Write(byte[] buffer, int offset, int count) { var pos = this.Position; this.Position = pos + this.Process(pos, buffer, offset, count, true); }

		private sealed class SynchronousAsyncResult : IAsyncResult
		{
			private static readonly ManualResetEvent RESET_EVENT = new ManualResetEvent(true);
			public SynchronousAsyncResult(AsyncCallback callback, object state, int count) { this.Callback = callback; this.AsyncState = state; this.Count = count; this.AsyncWaitHandle = RESET_EVENT; }
			public int Count { get; private set; }
			public AsyncCallback Callback { get; set; }
			public object AsyncState { get; private set; }
			public WaitHandle AsyncWaitHandle { get; private set; }
			public bool CompletedSynchronously { get { return true; } }
			public bool IsCompleted { get { return true; } }
		}
	}
}