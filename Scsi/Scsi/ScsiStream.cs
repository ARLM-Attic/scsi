using System;
using System.Diagnostics;
using System.IO;

namespace Scsi.Multimedia
{
	public class ScsiStream : System.IO.Stream
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private long _Position;

		/// <summary>Leaves the device open!</summary>
		public ScsiStream(IScsiDevice device) { this.Device = device; }

		public IScsiDevice Device { get; private set; }

		protected override void Dispose(bool disposing) { if (disposing) { this.Flush(); this.Device = null; } base.Dispose(disposing); }

		public override bool CanRead { get { return this.Device.CanRead; } }
		public override bool CanSeek { get { return this.Device.CanSeek; } }
		public override bool CanWrite { get { return this.Device.CanWrite; } }
		public override void Flush() { this.Device.Flush(); }
		public override long Length { get { return this.Device.Capacity; } }
		public override long Position { get { return this._Position; } set { this._Position = value; } }

		public override int Read(byte[] buffer, int offset, int count)
		{
			long pos = this.Position;
			count = this.Read(ref pos, buffer, offset, count);
			this.Position = pos;
			return count;
		}

		private int Read(ref long position, byte[] buffer, int offset, int count)
		{
			if (count < 0) { throw new ArgumentOutOfRangeException("count", count, "Value must be nonnegative."); }
			this.Device.Read(position, buffer, offset, count, false);
			position += count;
			return count;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long newPos;
			switch (origin)
			{
				case SeekOrigin.Begin:
					newPos = 0 + offset;
					break;
				case SeekOrigin.Current:
					newPos = this._Position + offset;
					break;
				case SeekOrigin.End:
					newPos = this.Device.Capacity + offset;
					break;
				default:
					throw new ArgumentOutOfRangeException("origin", origin, "Invalid seek origin.");
			}
			this._Position = newPos;
			return this._Position;
		}

		public override void SetLength(long value) { }

		public override void Write(byte[] buffer, int offset, int count)
		{
			long pos = this.Position;
			this.Write(ref pos, buffer, offset, count);
			this.Position = pos;
		}

		private void Write(ref long pos, byte[] buffer, int offset, int count)
		{
			if (count < 0) { throw new ArgumentOutOfRangeException("count", count, "Value must be nonnegative."); }
			try { this.Device.Write(pos, buffer, offset, count, false, false); }
			catch (ScsiException ex)
			{
				var sense = ex.SenseData;
				if (sense.EndOfMedium || (sense.SenseKey == SenseKey.IllegalRequest &&
					sense.AdditionalSenseCodeAndQualifier == AdditionalSenseInformation.LogicalBlockAddressOutOfRange))
				{ throw new EndOfStreamException(null, ex); }
				throw;
			}
			pos = pos + count;
		}
	}
}