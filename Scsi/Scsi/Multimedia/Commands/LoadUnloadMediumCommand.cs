﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class LoadUnloadMediumCommand : FixedLengthScsiCommand
	{
		public LoadUnloadMediumCommand() : base(ScsiCommandCode.LoadUnloadMedium) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool Immediate { get { return Bits.GetBit(this.byte1, 0); } set { this.byte1 = Bits.SetBit(this.byte1, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		public bool LoadUnload { get { return Bits.GetBit(this.byte4, 1); } set { this.byte4 = Bits.SetBit(this.byte4, 1, value); } }
		public bool Start { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _Slot;
		public byte Slot { get { return this._Slot; } set { this._Slot = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte9;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte10;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;

		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return this.Immediate ? ScsiTimeoutGroup.Group1 : ScsiTimeoutGroup.Group2; } }
	}
}