using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;
using System;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public sealed class LunsParameterList : IMarshalable
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr LUN_LIST_LENGTH_OFFSET = Marshal.OffsetOf(typeof(LunsParameterList), "_LunListLength");
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr RESERVED_OFFSET = Marshal.OffsetOf(typeof(LunsParameterList), "_Reserved");
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		internal static readonly IntPtr LUNS_OFFSET = Marshal.OffsetOf(typeof(LunsParameterList), "_Luns");
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly ulong[] EMPTY_LUNS = new ulong[0];

		public LunsParameterList() { }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private uint _LunListLength;
		public uint LunListLength { get { return Bits.BigEndian(this._LunListLength); } private set { this._LunListLength = Bits.BigEndian(value); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private uint _Reserved;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ulong[] _Luns = EMPTY_LUNS;

		public ulong[] GetLuns() { var result = (ulong[])this._Luns.Clone(); for (int i = 0; i < result.Length; i++) { result[i] = Bits.BigEndian(result[i]); } return result; }

		public void SetLuns(ulong[] luns)
		{
			if (luns == null) { throw new ArgumentNullException("luns"); }
			this._Luns = (ulong[])luns.Clone();
			for (int i = 0; i < this._Luns.Length; i++) { this._Luns[i] = Bits.BigEndian(this._Luns[i]); }
			this.LunListLength = luns != null ? (uint)luns.Length * sizeof(ulong) : 0;
		}

		void IMarshalable.MarshalFrom(BufferWithSize buffer)
		{
			this._LunListLength = buffer.Read<uint>(LUN_LIST_LENGTH_OFFSET);
			this._Reserved = buffer.Read<uint>(RESERVED_OFFSET);
			this._Luns = new ulong[this._LunListLength / sizeof(ulong)];
			for (int i = 0; i < this._Luns.Length; i++) { this._Luns[i] = buffer.Read<ulong>((int)LUNS_OFFSET + i * sizeof(ulong)); }
		}
		
		void IMarshalable.MarshalTo(BufferWithSize buffer)
		{
			buffer.Write(this._LunListLength, LUN_LIST_LENGTH_OFFSET);
			buffer.Write(this._Reserved, RESERVED_OFFSET);
			for (int i = 0; i < this._Luns.Length; i++) { buffer.Write(this._Luns[i], (int)LUNS_OFFSET + i * sizeof(ulong)); }
		}

		int IMarshalable.MarshaledSize { get { return (int)LUNS_OFFSET + (int)this.LunListLength; } }

		public static uint ReadLunListLength(BufferWithSize buffer) { return buffer.Read<uint>(LUN_LIST_LENGTH_OFFSET); }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ReadCapacityInfo
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _LogicalBlockAddress;
		public uint LogicalBlockAddress { get { return Bits.BigEndian(this._LogicalBlockAddress); } set { this._LogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _BlockLength;
		public uint BlockLength { get { return Bits.BigEndian(this._BlockLength); } set { this._BlockLength = Bits.BigEndian(value); } }
		public override string ToString() { return Internal.GenericToString(this, false); }
	}

	public enum ServiceAction : short { None = 0x00, Read32 = 0x0009 }

	public enum DataTransferDirection : byte
	{
		/// <summary>Send data to the drive</summary>
		[Description("Send data to the drive")]
		SendData,
		/// <summary>Receive data from the drive</summary>
		[Description("Receive data from the drive")]
		ReceiveData,
		/// <summary>No data transfer</summary>
		[Description("No data transfer")]
		NoData,
	}

	public enum ScsiTimeoutGroup { None = 0, Group1 = 1, Group2 = 2, Group3 = 3 }
}