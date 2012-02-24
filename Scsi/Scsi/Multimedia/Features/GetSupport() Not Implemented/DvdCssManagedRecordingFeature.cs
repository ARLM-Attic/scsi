using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that GetSupport CSS Managed recording on DVDDownload disc. This feature is current only if a recordable DVD-Download disc is loaded.</summary>
	[Description("This feature identifies a drive that GetSupport CSS Managed recording on DVDDownload disc.\r\nThis feature is current only if a recordable DVD-Download disc is loaded.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class DvdCssManagedRecordingFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.ReportKey, ScsiCommandCode.SendDiscStructure, ScsiCommandCode.SendKey });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public DvdCssManagedRecordingFeature() : base(FeatureCode.DvdContentScramblingSystemManagedRecording) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _MaximumNumberOfScrambledExtentInformationSeries;
		[DisplayName("Maximum Number of Scrambled Extent Information Series")]
		public byte MaximumNumberOfScrambledExtentInformationSeries { get { return this._MaximumNumberOfScrambledExtentInformationSeries; } set { this._MaximumNumberOfScrambledExtentInformationSeries = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
	}
}