using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ReportLunsCommand : FixedLengthScsiCommand
	{
		public ReportLunsCommand() : base(ScsiCommandCode.ReportLuns) { }
		public ReportLunsCommand(SelectReport selectReport) : this() { this.SelectReport = selectReport; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private SelectReport _SelectReport;
		public SelectReport SelectReport { get { return this._SelectReport; } set { this._SelectReport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _AllocationLength;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public uint AllocationLength { get { return Bits.BigEndian(this._AllocationLength); } set { this._AllocationLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte10;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.None; } }
	}

	public enum SelectReport : byte
	{
		MakeAccessible = 0x00,
		WellKnownUnits = 0x01,
		AllUnits = 0x02,
	}
}