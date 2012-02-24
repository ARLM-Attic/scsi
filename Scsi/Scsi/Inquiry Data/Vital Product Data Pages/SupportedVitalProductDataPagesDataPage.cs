using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class SupportedVitalProductDataPagesDataPage : VitalProductDataInquiryData
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SUPPORTED_PAGE_LIST_OFFSET = Marshal.OffsetOf(typeof(SupportedVitalProductDataPagesDataPage), "_SupportedPageList");

		public SupportedVitalProductDataPagesDataPage() : base(VitalProductDataPageCode.SupportedVitalProductDataPages) { }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private VitalProductDataPageCode[] _SupportedPageList;
		public VitalProductDataPageCode[] SupportedPageList { get { return this._SupportedPageList; } set { this._SupportedPageList = value; } }

		protected override void MarshalFrom(Helper.BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._SupportedPageList = new VitalProductDataPageCode[this.PageLength / sizeof(VitalProductDataPageCode)];
			for (int i = 0; i < this._SupportedPageList.Length; i++)
			{ this._SupportedPageList[i] = buffer.Read<VitalProductDataPageCode>((int)SUPPORTED_PAGE_LIST_OFFSET + i); }
		}

		protected override void MarshalTo(Helper.BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			for (int i = 0; i < this._SupportedPageList.Length; i++)
			{ buffer.Write(this._SupportedPageList[i], (int)SUPPORTED_PAGE_LIST_OFFSET + i); }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { this.UpdatePageLength(); return base.MarshaledSize + this.PageLength; } }

		protected override void UpdatePageLength() { this.PageLength = (byte)(this._SupportedPageList != null ? this._SupportedPageList.Length * sizeof(VitalProductDataPageCode) : 0); }
	}
}