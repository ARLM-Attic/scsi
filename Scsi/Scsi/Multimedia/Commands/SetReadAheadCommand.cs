﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class SetReadAheadCommand : FixedLengthScsiCommand
	{
		public SetReadAheadCommand() : base(ScsiCommandCode.SetReadAhead) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _TriggerLogicalBlockAddress;
		public uint TriggerLogicalBlockAddress { get { return Bits.BigEndian(this._TriggerLogicalBlockAddress); } set { this._TriggerLogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _ReadAheadLogicalBlockAddress;
		public uint ReadAheadLogicalBlockAddress { get { return Bits.BigEndian(this._ReadAheadLogicalBlockAddress); } set { this._ReadAheadLogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte10;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }
	}
}