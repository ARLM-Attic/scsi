using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that is able to write a CD in session-at-once or raw mode.</summary>
	[Description("This feature identifies a drive that is able to write a CD in session-at-once or raw mode.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class CDMasteringFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.ReadDiscInformation, ScsiCommandCode.ReadTrackInformation, ScsiCommandCode.SendCueSheet, ScsiCommandCode.SendOpcInformation, ScsiCommandCode.Write10 });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public CDMasteringFeature() : base(FeatureCode.CDMastering) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DisplayName("Buffer Under-run Protection")]
		public bool BufferUnderrunProtection { get { return Bits.GetBit(this.byte4, 6); } set { this.byte4 = Bits.SetBit(this.byte4, 6, value); } }
		[DisplayName("Session-at-Once")]
		public bool SessionAtOnce { get { return Bits.GetBit(this.byte4, 5); } set { this.byte4 = Bits.SetBit(this.byte4, 5, value); } }
		[DisplayName("Record RAW Multisession")]
		public bool RawMS { get { return Bits.GetBit(this.byte4, 4); } set { this.byte4 = Bits.SetBit(this.byte4, 4, value); } }
		[DisplayName("Record RAW")]
		public bool Raw { get { return Bits.GetBit(this.byte4, 3); } set { this.byte4 = Bits.SetBit(this.byte4, 3, value); } }
		[DisplayName("Simulation")]
		public bool TestWrite { get { return Bits.GetBit(this.byte4, 2); } set { this.byte4 = Bits.SetBit(this.byte4, 2, value); } }
		[DisplayName("Overwrite CD-RW")]
		public bool CDRW { get { return Bits.GetBit(this.byte4, 1); } set { this.byte4 = Bits.SetBit(this.byte4, 1, value); } }
		[DisplayName("Record User R-W Subchannels")]
		public bool RWSubchannels { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte5; //msb
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _byte7; //lsb
		[DisplayName("Maximum Cue Sheet Length")]
		public uint MaximumCueSheetLength
		{
			get { unchecked { return (uint)this._byte7 | ((uint)this._byte6 << 8) | ((uint)this._byte5 << 16); } }
			set { unchecked { this._byte7 = (byte)(value >> 0); this._byte6 = (byte)(value >> 8); this._byte5 = (byte)(value >> 16); } }
		}
	}
}