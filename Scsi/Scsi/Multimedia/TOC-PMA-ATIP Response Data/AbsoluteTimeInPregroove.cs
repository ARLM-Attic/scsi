using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class AbsoluteTimeInPregroove : TocPmaAtipResponseData
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SPECIAL_INFORMATION_OFFSET = Marshal.OffsetOf(typeof(AbsoluteTimeInPregroove), "_SpecialInformation");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr ADDITIONAL_INFORMATION_OFFSET = Marshal.OffsetOf(typeof(AbsoluteTimeInPregroove), "_AdditionalInformation");

		public AbsoluteTimeInPregroove() : base() { }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		private int[] _SpecialInformation;
		//TODO: The special information has a very specific format but I haven't implemented it yet.
		public int[] SpecialInformation { get { return this._SpecialInformation; } set { this._SpecialInformation = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		private int[] _AdditionalInformation;
		//TODO: The additional information has a very specific format but I haven't implemented it yet.
		public int[] AdditionalInformation { get { return this._AdditionalInformation; } set { this._AdditionalInformation = value; } }


		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._SpecialInformation = new int[3];
			Marshal.Copy(buffer.ExtractSegment((int)SPECIAL_INFORMATION_OFFSET, this._SpecialInformation.Length * sizeof(int)).Address, this._SpecialInformation, 0, this._SpecialInformation.Length);
			this._AdditionalInformation = new int[3];
			Marshal.Copy(buffer.ExtractSegment((int)ADDITIONAL_INFORMATION_OFFSET, this._AdditionalInformation.Length * sizeof(int)).Address, this._AdditionalInformation, 0, this._AdditionalInformation.Length);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			if (this._SpecialInformation.Length > 3) { throw new OverflowException("Field is too large."); }
			Marshal.Copy(this._SpecialInformation, 0, buffer.ExtractSegment((int)SPECIAL_INFORMATION_OFFSET, this._SpecialInformation.Length * sizeof(int)).Address, this._SpecialInformation.Length);
			buffer.Initialize((int)SPECIAL_INFORMATION_OFFSET + this._SpecialInformation.Length, 3 - this._SpecialInformation.Length);
			if (this._AdditionalInformation.Length > 3) { throw new OverflowException("Field is too large."); }
			Marshal.Copy(this._AdditionalInformation, 0, buffer.ExtractSegment((int)ADDITIONAL_INFORMATION_OFFSET, this._AdditionalInformation.Length * sizeof(int)).Address, this._AdditionalInformation.Length);
			buffer.Initialize((int)ADDITIONAL_INFORMATION_OFFSET + this._AdditionalInformation.Length, 3 - this._AdditionalInformation.Length);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<AbsoluteTimeInPregroove>(); } }
	}
}