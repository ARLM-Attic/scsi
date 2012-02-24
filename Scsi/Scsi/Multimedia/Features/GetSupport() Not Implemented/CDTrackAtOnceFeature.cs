using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that is able to write data to a CD track.</summary>
	[Description("This feature identifies a drive that is able to write data to a CD track.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class CDTrackAtOnceFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.Blank, ScsiCommandCode.CloseSessionOrTrack, ScsiCommandCode.ReadDiscInformation, ScsiCommandCode.ReadTrackInformation, ScsiCommandCode.ReserveTrack, ScsiCommandCode.SendOpcInformation, ScsiCommandCode.SynchronizeCache10, ScsiCommandCode.Write10 });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public CDTrackAtOnceFeature() : base(FeatureCode.CDTrackAtOnce) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DisplayName("Buffer Under-run Protection")]
		public bool BufferUnderrunProtection { get { return Bits.GetBit(this.byte4, 6); } set { this.byte4 = Bits.SetBit(this.byte4, 6, value); } }
		[DisplayName("R-W Subcode in RAW Mode")]
		public bool RWRaw { get { return Bits.GetBit(this.byte4, 4); } set { this.byte4 = Bits.SetBit(this.byte4, 4, value); } }
		[DisplayName("R-W Subcode in Packed Mode")]
		public bool RWPack { get { return Bits.GetBit(this.byte4, 3); } set { this.byte4 = Bits.SetBit(this.byte4, 3, value); } }
		[DisplayName("Simulation")]
		public bool TestWrite { get { return Bits.GetBit(this.byte4, 2); } set { this.byte4 = Bits.SetBit(this.byte4, 2, value); } }
		[DisplayName("Overwrite Track at Once")]
		public bool CDRW { get { return Bits.GetBit(this.byte4, 1); } set { this.byte4 = Bits.SetBit(this.byte4, 1, value); } }
		[DisplayName("R-W Subcode")]
		public bool RWSubCode { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private DataBlockTypesSupported _DataBlockTypesSupported;
		[DisplayName("Data Types Supported")]
		public DataBlockTypesSupported DataBlockTypesSupported { get { return Bits.BigEndian(this._DataBlockTypesSupported); } set { this._DataBlockTypesSupported = Bits.BigEndian(value); } }
	}
}