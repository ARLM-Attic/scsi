using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;
using System;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Release06Command : FixedLengthScsiCommand
	{
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private const byte THIRD_PARTY_DEVICE_ID = 0x0E;

		public Release06Command() : base(ScsiCommandCode.Release06) { }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _byte1;
		/// <summary>Do not use this field directly; use the nullability of <see cref="ThirdPartyDeviceId"/>.</summary>
		private bool ThirdParty { get { return Bits.GetBit(this._byte1, 4); } set { this._byte1 = Bits.SetBit(this._byte1, 4, value); } }
		[Obsolete("Obsolete in SPC-2; must be zero if not supported by the drive.")]
		public byte? ThirdPartyDeviceId { get { return this.ThirdParty ? Bits.GetValueMask(this._byte1, 1, THIRD_PARTY_DEVICE_ID) : (byte?)null; } set { this.ThirdParty = value != null; this._byte1 = Bits.PutValueMask(this._byte1, value.GetValueOrDefault(), 1, THIRD_PARTY_DEVICE_ID); } }
		[Obsolete("Obsolete in SPC-2; must be zero if not supported by the drive.")]
		public bool Extent { get { return Bits.GetBit(this._byte1, 0); } set { this._byte1 = Bits.SetBit(this._byte1, 0, value); } }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _ReservationIdentification;
		[Obsolete("Obsolete in SPC-2; must be zero if not supported by the drive.")]
		public byte ReservationIdentification { get { return this._ReservationIdentification; } set { this._ReservationIdentification = value; } }

		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private ushort _ParameterListLength;
		public ushort ParameterListLength { get { return Bits.BigEndian(this._ParameterListLength); } set { this._ParameterListLength = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandControl _Control;
		public override CommandControl Control { get { return this._Control; } set { this._Control = value; } }

		public override ScsiTimeoutGroup TimeoutGroup { get { return ScsiTimeoutGroup.None; } }
	}
}