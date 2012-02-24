using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class CDText : TocPmaAtipResponseData
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr CD_TEXT_DATA_OFFSET = Marshal.OffsetOf(typeof(CDText), "_CDTextData");

		public CDText() : base() { }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
		private byte[] _CDTextData;
		public byte[] CDTextData { get { return this._CDTextData; } set { this._CDTextData = value; } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._CDTextData = new byte[18];
			buffer.CopyTo((int)CD_TEXT_DATA_OFFSET, this._CDTextData, 0, this._CDTextData.Length);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			if (this._CDTextData.Length > 18) { throw new OverflowException("Field is too large."); }
			buffer.CopyFrom((int)CD_TEXT_DATA_OFFSET, this._CDTextData, 0, this._CDTextData.Length);
			buffer.Initialize((int)CD_TEXT_DATA_OFFSET + this._CDTextData.Length, 18 - this._CDTextData.Length);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<CDText>(); } }
	}
}