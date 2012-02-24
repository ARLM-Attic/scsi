using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ModeSelect06Command : FixedLengthScsiCommand
	{
		public ModeSelect06Command() : base(ScsiCommandCode.ModeSelect06) { this.PageFormat = true; }
		public ModeSelect06Command(bool savePages, bool pageFormat)
			: this()
		{
			this.SavePages = savePages;
			this.PageFormat = pageFormat;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool SavePages { get { return Bits.GetBit(this.byte1, 0); } set { this.byte1 = Bits.SetBit(this.byte1, 0, value); } }
		/// <summary>Indicates whether or not the commands are as defined in the SPC-2 standard.</summary>
		[Description("Indicates whether or not the commands are as defined in the SPC-2 standard.")]
		public bool PageFormat { get { return Bits.GetBit(this.byte1, 4); } set { this.byte1 = Bits.SetBit(this.byte1, 4, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _ParameterListLength;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public byte ParameterListLength { get { return Bits.BigEndian(this._ParameterListLength); } set { this._ParameterListLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;

		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }

		public static explicit operator ModeSelect10Command(ModeSelect06Command command)
		{
			return new ModeSelect10Command(command.SavePages, command.PageFormat)
			{
				ParameterListLength = command.ParameterListLength,
				Control = command.Control,
			};
		}
	}
}