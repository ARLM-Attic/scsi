using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ReadTrackInformationCommand : FixedLengthScsiCommand
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte ADDRESS_OR_NUMBER_TYPE_MASK = 0x3;

		public ReadTrackInformationCommand() : base(ScsiCommandCode.ReadTrackInformation) { }

		public ReadTrackInformationCommand(bool findFirstOpen, TrackIdentificationType addressOrNumberType, uint trackNumberOrSessionNumberOrLogicalBlockAddress)
			: this()
		{
			this.Open = findFirstOpen;
			this.AddressOrNumberType = addressOrNumberType;
			this.TrackNumberOrSessionNumberOrLogicalBlockAddress = trackNumberOrSessionNumberOrLogicalBlockAddress;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		/// <summary>If <c>false</c>, finds the logical track that contains the given LogicalBlockAddress, logical track number, or logical session number. If <c>true</c>, finds the first open logical track with a logical track number that is GREATER than the given LogicalBlockAddress, logical track number, or logical session number.</summary>
		[Description("If false, finds the logical track that contains the given LogicalBlockAddress, logical track number, or logical session number. If true, finds the first open logical track with a logical track number that is GREATER than the given LogicalBlockAddress, logical track number, or logical session number.")]
		public bool Open { get { return Bits.GetBit(this.byte1, 2); } set { this.byte1 = Bits.SetBit(this.byte1, 2, value); } }
		public TrackIdentificationType AddressOrNumberType
		{
			get { return (TrackIdentificationType)Bits.GetValueMask(this.byte1, 0, ADDRESS_OR_NUMBER_TYPE_MASK); }
			set { this.byte1 = Bits.PutValueMask(this.byte1, (byte)value, 0, ADDRESS_OR_NUMBER_TYPE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _TrackNumberOrSessionNumberOrLogicalBlockAddress;
		public uint TrackNumberOrSessionNumberOrLogicalBlockAddress { get { return Bits.BigEndian(this._TrackNumberOrSessionNumberOrLogicalBlockAddress); } set { this._TrackNumberOrSessionNumberOrLogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _AllocationLength;
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public ushort AllocationLength { get { return Bits.BigEndian(this._AllocationLength); } set { this._AllocationLength = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;

		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }
	}
}