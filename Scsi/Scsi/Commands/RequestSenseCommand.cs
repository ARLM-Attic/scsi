using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class RequestSenseCommand : FixedLengthScsiCommand
	{
		public RequestSenseCommand() : base(ScsiCommandCode.RequestSense) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _AllocationLength;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public byte AllocationLength { get { return this._AllocationLength; } set { this._AllocationLength = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.None; } }
	}
}