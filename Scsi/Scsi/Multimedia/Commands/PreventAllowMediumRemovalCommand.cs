﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class PreventAllowMediumRemovalCommand : FixedLengthScsiCommand
	{
		public PreventAllowMediumRemovalCommand() : base(ScsiCommandCode.PreventAllowMediumRemoval) { }
		public PreventAllowMediumRemovalCommand(bool prevent) : this() { this.Prevent = prevent; }
		public PreventAllowMediumRemovalCommand(bool prevent, bool persistent) : this(prevent) { this.Persistent = persistent; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		public bool Persistent { get { return Bits.GetBit(this.byte4, 1); } set { this.byte4 = Bits.SetBit(this.byte4, 1, value); } }
		public bool Prevent { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }
	}
}