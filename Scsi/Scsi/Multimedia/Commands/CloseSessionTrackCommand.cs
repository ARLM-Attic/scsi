using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class CloseSessionTrackCommand : FixedLengthScsiCommand
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte FUNCTION_MASK = 0x7;

		public CloseSessionTrackCommand() : base(ScsiCommandCode.CloseSessionOrTrack) { }

		public CloseSessionTrackCommand(bool immediate, TrackSessionCloseFunction function, ushort trackNumber)
			: this()
		{
			this.Immediate = immediate;
			this.Function = function;
			this.TrackNumber = trackNumber;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool Immediate { get { return Bits.GetBit(this.byte1, 0); } set { this.byte1 = Bits.SetBit(this.byte1, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		public TrackSessionCloseFunction Function
		{
			get { return (TrackSessionCloseFunction)Bits.GetValueMask(this.byte2, 0, FUNCTION_MASK); }
			set { this.byte2 = Bits.PutValueMask(this.byte2, (byte)value, 0, FUNCTION_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		private ushort _TrackNumber;
		public ushort TrackNumber { get { return Bits.BigEndian(this._TrackNumber); } set { this._TrackNumber = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte8;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return this.Immediate ? ScsiTimeoutGroup.Group1 : ScsiTimeoutGroup.Group2; } }
	}
}