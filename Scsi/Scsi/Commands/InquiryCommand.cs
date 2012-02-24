using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class InquiryCommand : FixedLengthScsiCommand
	{
		public InquiryCommand() : base(ScsiCommandCode.Inquiry) { }
		public InquiryCommand(VitalProductDataPageCode? pageCode) : this() { this.PageCode = pageCode; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		/// <summary>If <c>false</c>, the <see cref="PageCode"/> property must not be set.</summary>
		private bool EVitalProductData { get { return Bits.GetBit(this.byte1, 0); } set { this.byte1 = Bits.SetBit(this.byte1, 0, value); } }
		public bool CommandSupportData { get { return Bits.GetBit(this.byte1, 1); } set { this.byte1 = Bits.SetBit(this.byte1, 1, value); } }
		private VitalProductDataPageCode _PageCode;
		public VitalProductDataPageCode? PageCode { get { return this.EVitalProductData ? (VitalProductDataPageCode?)this._PageCode : null; } set { this.EVitalProductData = value != null; this._PageCode = value.GetValueOrDefault(); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _AllocationLength;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public ushort AllocationLength { get { return Bits.BigEndian(this._AllocationLength); } set { this._AllocationLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.None; } }
	}
}