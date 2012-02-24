using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ReadTocPmaAtipCommand : FixedLengthScsiCommand
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte FORMAT_MASK = 0x0F;

		public ReadTocPmaAtipCommand() : base(ScsiCommandCode.ReadTocPmaAtip) { }

		public ReadTocPmaAtipCommand(ReadTocPmaAtipDataFormat format, bool useMSF, byte trackOrSessionNumber)
			: this() { this.UseMsfFormat = useMSF; this.Format = format; this.TrackOrSessionNumber = trackOrSessionNumber; }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte1;
		public bool UseMsfFormat { get { return Bits.GetBit(this.byte1, 1); } set { this.byte1 = Bits.SetBit(this.byte1, 1, value); } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte2;
		public ReadTocPmaAtipDataFormat Format
		{
			get { return (ReadTocPmaAtipDataFormat)Bits.GetValueMask(this.byte1, 0, FORMAT_MASK); }
			set { this.byte1 = Bits.PutValueMask(this.byte1, (byte)value, 0, FORMAT_MASK); }
		}
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte3;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _TrackOrSessionNumber;
		public byte TrackOrSessionNumber { get { return this._TrackOrSessionNumber; } set { this._TrackOrSessionNumber = value; } }
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ushort _AllocationLength;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public ushort AllocationLength { get { return Bits.BigEndian(this._AllocationLength); } set { this._AllocationLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;

		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }
	}

	public enum ReadTocPmaAtipDataFormat : byte
	{
		/// <summary>The <see cref="ReadTocPmaAtipCommand.TrackOrSessionNumber"/> field specifies starting track number for which the data is returned. For multi-session discs, TOC data is returned for all sessions. Track number <c>0xAA</c> is reported only for the Lead-out area of the last complete session.</summary>
		FormattedTableOfContents = 0,
		/// <summary><para>This format returns the first complete session number, last complete session number and last complete session starting address. In this format, the <see cref="ReadTocPmaAtipCommand.TrackOrSessionNumber"/> field is reserved and should be set to <c>0x00</c>.</para><para>NOTE: This format provides the Host access to the last closed session starting address quickly.</para></summary>
		MultisessionInformation = 1,
		/// <summary>This format returns all Q sub-code data in the Lead-In (TOC) areas starting from a session number as specified in the <see cref="ReadTocPmaAtipCommand.TrackOrSessionNumber"/> field. In this mode, the Drive supports Q Sub-channel POINT field value of <c>0xA0</c>, <c>0xA1</c>, <c>0xA2</c>, Track numbers, <c>0xB0</c>, <c>0xB1</c>, <c>0xB2</c>, <c>0xB3</c>, <c>0xB4</c>, <c>0xC0</c>, and <c>0xC1</c>. There is no defined LBA addressing and so the <see cref="ReadTocPmaAtipCommand.UseMsfFormat"/> bit shall be set to <c>true</c>.</summary>
		RawTableOfContents = 2,
		/// <summary>This format returns Q sub-channel data in the PMA area. In this format, the <see cref="ReadTocPmaAtipCommand.TrackOrSessionNumber"/> field is reserved and shall be set to <c>0x00</c>. There is no defined LBA addressing and so the <see cref="ReadTocPmaAtipCommand.UseMsfFormat"/> bit shall be set to <c>true</c>.</summary>
		ProgramMemoryArea = 3,
		/// <summary>This format returns ATIP data. In this format, the <see cref="ReadTocPmaAtipCommand.TrackOrSessionNumber"/> field is reserved and shall be set to <c>0x00</c>. There is no defined LBA addressing and so the <see cref="ReadTocPmaAtipCommand.UseMsfFormat"/> bit shall be set to <c>true</c>.</summary>
		AbsoluteTimeInPreGroove = 4,
		/// <summary>This format returns CD-TEXT information that is recorded in the Lead-in area as R-W Sub-channel Data. In this format, the <see cref="ReadTocPmaAtipCommand.TrackOrSessionNumber"/> field and the <see cref="ReadTocPmaAtipCommand.UseMsfFormat"/> bit are ignored by the drive.</summary>
		CDTextInRWSubChannel = 5,
	}
}