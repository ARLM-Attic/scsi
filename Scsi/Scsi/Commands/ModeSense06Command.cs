﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ModeSense06Command : FixedLengthScsiCommand
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte PAGE_CODE_MASK = 0x3F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte PAGE_CONTROL_MASK = 0xC0;

		public ModeSense06Command() : base(ScsiCommandCode.ModeSense06) { this.DisableBlockDescriptors = true; this.PageControl = PageControl.CurrentValues; }

		public ModeSense06Command(PageControl pageControl) : this() { this.PageControl = pageControl; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool DisableBlockDescriptors { get { return Bits.GetBit(this.byte1, 3); } set { this.byte1 = Bits.SetBit(this.byte1, 3, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public ModePageCode PageCode { get { return (ModePageCode)Bits.GetValueMask(this.byte2, 0, PAGE_CODE_MASK); } set { this.byte2 = Bits.PutValueMask(this.byte2, (byte)value, 0, PAGE_CODE_MASK); } }
		public PageControl PageControl { get { return (PageControl)Bits.GetValueMask(this.byte2, 6, PAGE_CONTROL_MASK); } set { this.byte2 = Bits.PutValueMask(this.byte2, (byte)value, 6, PAGE_CONTROL_MASK); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _SubpageCode;
		public byte SubpageCode { get { return this._SubpageCode; } set { this._SubpageCode = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _AllocationLength;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public byte AllocationLength { get { return Bits.BigEndian(this._AllocationLength); } set { this._AllocationLength = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }


		public static implicit operator ModeSense10Command(ModeSense06Command command)
		{
			return new ModeSense10Command(command.PageControl)
			{
				AllocationLength = checked((byte)command.AllocationLength),
				Control = command.Control,
				DisableBlockDescriptors = command.DisableBlockDescriptors,
				SubpageCode = command.SubpageCode,
			};
		}
	}
}