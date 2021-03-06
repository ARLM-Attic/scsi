﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ReadCDCommand : FixedLengthScsiCommand
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte EXPECTED_SECTOR_TYPE_MASK = 0x1C;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte C2_ERROR_CODE_MASK = 0x06;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte HEADER_CODES_MASK = 0x60;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte SUB_CHANNEL_SELECTION_BITS_MASK = 0x07;

		public ReadCDCommand() : base(ScsiCommandCode.ReadCD) { this.UserData = true; }

		public ReadCDCommand(ReadCDExpectedSectorType sectorType, uint logicalBlockAddress, uint transferBlockCount) : this() { this.ExpectedSectorType = sectorType; this.StartingLogicalBlockAddress = logicalBlockAddress; this.TransferBlockCount = transferBlockCount; }

		private byte byte1;
		public ReadCDExpectedSectorType ExpectedSectorType
		{
			get { return (ReadCDExpectedSectorType)Bits.GetValueMask(this.byte1, 2, EXPECTED_SECTOR_TYPE_MASK); }
			set { this.byte1 = Bits.PutValueMask(this.byte1, (byte)value, 2, EXPECTED_SECTOR_TYPE_MASK); }
		}
		/// <summary>
		/// Digital Audio Play (DAP) is used to control error concealment when the data being read is CD-DA. If the data being read is not CD-DA, DAP is ignored.
		/// If the data being read is CD-DA and DAP is set to zero, then the user data returned to the Host should not be modified by flaw obscuring mechanisms such as audio data mute and interpolate.
		/// If the data being read is CD-DA and DAP is set to one, then the user data returned to the Host should be modified by flaw obscuring mechanisms such as audio data mute and interpolate.
		/// </summary>
		public bool DigitalAudioPlay { get { return Bits.GetBit(this.byte1, 1); } set { this.byte1 = Bits.SetBit(this.byte1, 1, value); } }
		private uint _StartingLogicalBlockAddress;
		public uint StartingLogicalBlockAddress { get { return Bits.BigEndian(this._StartingLogicalBlockAddress); } set { this._StartingLogicalBlockAddress = Bits.BigEndian(value); } }
		private byte _byte6; //msb
		private byte _byte7;
		private byte _byte8; //lsb
		/// <summary>Do not use this value unless you are responsible for calling the <see cref="IScsiPassThrough.ExecuteCommand"/> method directly.</summary>
		public uint TransferBlockCount
		{
			get { unchecked { return (uint)this._byte8 | ((uint)this._byte7 << 8) | ((uint)this._byte6 << 16); } }
			set { unchecked { this._byte8 = (byte)(value >> 0); this._byte7 = (byte)(value >> 8); this._byte6 = (byte)(value >> 16); } }
		}
		private byte byte9;
		public C2ErrorCode C2ErrorInformation
		{
			get { return (C2ErrorCode)Bits.GetValueMask(this.byte9, 1, C2_ERROR_CODE_MASK); }
			set { this.byte9 = Bits.PutValueMask(this.byte9, (byte)value, 1, C2_ERROR_CODE_MASK); }
		}
		public HeaderCodes HeaderCode
		{
			get { return (HeaderCodes)Bits.GetValueMask(this.byte9, 5, HEADER_CODES_MASK); }
			set { this.byte9 = Bits.PutValueMask(this.byte9, (byte)value, 5, HEADER_CODES_MASK); }
		}
		public bool ErrorDetectionCodeOrErrorCorrectionCode { get { return Bits.GetBit(this.byte9, 3); } set { this.byte9 = Bits.SetBit(this.byte9, 3, value); } }
		/// <summary>When <see cref="UserData"/> is <c>false</c>, the User Data field is not included in the read data stream. If <see cref="UserData"/> is <c>true</c>, the user data is included in the read data stream. The size of the user data field varies according to sector type.</summary>
		/// <remarks>
		/// The user data is defined according to Sector Type field of the CDB:
		/// <para>For CD-DA, User Data is all <c>2352</c> bytes of main channel.</para>
		/// <para>For data Mode 1, User Data is <c>2048</c> bytes beginning at offset <c>16</c> of the <c>2352</c> bytes of main channel.</para>
		/// <para>For data Mode 2 formless, User Data is <c>2336</c> bytes beginning at offset <c>16</c> of the <c>2352</c> bytes of main channel.</para>
		/// <para>For data Mode 2, form 1, User Data is <c>2048</c> bytes beginning at offset <c>24</c> of the <c>2352</c> bytes of main channel.</para>
		/// <para>For data Mode 2, form 2, User Data is <c>2324</c> bytes beginning at offset <c>24</c> of the <c>2352</c> bytes of main channel.</para>
		/// </remarks>
		public bool UserData { get { return Bits.GetBit(this.byte9, 4); } set { this.byte9 = Bits.SetBit(this.byte9, 4, value); } }
		private byte byte10;
		public SubChannelSelection SubChannelSelectionBits
		{
			get { return (SubChannelSelection)Bits.GetValueMask(this.byte10, 0, SUB_CHANNEL_SELECTION_BITS_MASK); }
			set { this.byte10 = Bits.PutValueMask(this.byte10, (byte)value, 0, SUB_CHANNEL_SELECTION_BITS_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;

		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.Group1; } }
	}
}