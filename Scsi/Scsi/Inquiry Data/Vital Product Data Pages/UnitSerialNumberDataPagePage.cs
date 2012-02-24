using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class UnitSerialNumberDataPage : VitalProductDataInquiryData
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private static readonly IntPtr PRODUCT_SERIAL_NUMBER_OFFSET = Marshal.OffsetOf(typeof(UnitSerialNumberDataPage), "_ProductSerialNumber");

		public UnitSerialNumberDataPage() : base(VitalProductDataPageCode.UnitSerialNumber) { }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
		private string _ProductSerialNumber;
		public string ProductSerialNumber { get { return this._ProductSerialNumber; } set { this._ProductSerialNumber = value; this.UpdatePageLength(); } }

		protected override void UpdatePageLength() { this.PageLength = (byte)(this._ProductSerialNumber != null ? this._ProductSerialNumber.Length * sizeof(byte) : 0); }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._ProductSerialNumber = buffer.ToStringAnsi((int)PRODUCT_SERIAL_NUMBER_OFFSET, this.PageLength - base.MarshaledSize);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			Marshaler.StringToPtrAnsi(this._ProductSerialNumber, buffer.ExtractSegment((int)PRODUCT_SERIAL_NUMBER_OFFSET, this.PageLength - base.MarshaledSize).Address);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected override int MarshaledSize { get { this.UpdatePageLength(); return base.MarshaledSize + this.PageLength; } }
	}
}