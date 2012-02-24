using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public abstract class VariableLengthScsiCommand : ScsiCommand
	{
		protected VariableLengthScsiCommand(ScsiCommandCode operationCode) : base(operationCode) { }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public sealed override CommandControl Control { get { return this._Control; } set { this._Control = value; } }
	}
}