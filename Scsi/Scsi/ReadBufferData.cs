using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class BufferCombinedHeaderAndData : IMarshalable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr DATA_OFFSET = Marshal.OffsetOf(typeof(BufferCombinedHeaderAndData), "_Data");
		public BufferCombinedHeaderAndData() { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte1; //MSB
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte3; //LSB
		public uint BufferCapacity
		{
			get { unchecked { return (uint)this._byte3 | ((uint)this._byte2 << 8) | ((uint)this._byte1 << 16); } }
			set { unchecked { this._byte3 = (byte)(value >> 0); this._byte2 = (byte)(value >> 8); this._byte1 = (byte)(value >> 16); } }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private byte[] _Data;
		public byte[] Data { get { return this._Data; } set { this._Data = value; this.BufferCapacity = value == null ? 0 : (uint)value.Length; } }

		internal static uint ReadBufferCapacity(IntPtr address) { unsafe { byte* ptr = (byte*)address; unchecked { return (uint)ptr[3] | ((uint)ptr[2] << 8) | ((uint)ptr[1] << 16); } } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected virtual int MarshaledSize { get { return Marshal.SizeOf(this) + this.Data.Length - 1; } }

		protected virtual void MarshalFrom(BufferWithSize buffer)
		{
			Marshaler.DefaultPtrToStructure(buffer, this);
			this.Data = new byte[this.BufferCapacity];
			buffer.CopyTo(DATA_OFFSET, this.Data, IntPtr.Zero, (IntPtr)this.Data.Length);
		}

		protected virtual void MarshalTo(BufferWithSize buffer)
		{
			Marshaler.DefaultStructureToPtr((object)this, buffer);
			buffer.CopyFrom(DATA_OFFSET, this.Data, IntPtr.Zero, (IntPtr)this.Data.Length);
		}


		void IMarshalable.MarshalFrom(BufferWithSize buffer) { this.MarshalFrom(buffer); }
		void IMarshalable.MarshalTo(BufferWithSize buffer) { this.MarshalTo(buffer); }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int IMarshalable.MarshaledSize { get { return this.MarshaledSize; } }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ReadBufferDescriptor
	{
		public byte OffsetBoundary;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte1; //MSB
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte3; //LSB
		public uint BufferCapacity
		{
			get { unchecked { return (uint)this._byte3 | ((uint)this._byte2 << 8) | ((uint)this._byte1 << 16); } }
			set { unchecked { this._byte3 = (byte)(value >> 0); this._byte2 = (byte)(value >> 8); this._byte1 = (byte)(value >> 16); } }
		}
	}
}