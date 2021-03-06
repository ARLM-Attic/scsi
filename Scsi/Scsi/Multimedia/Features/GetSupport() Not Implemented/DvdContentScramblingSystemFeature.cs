﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that is able to perform DVD CSS/CPPM authentication and key management. This feature identifies drives that support CSS for DVD-Video and CPPM for DVD-Audio. The drive maintains the integrity of the keys by only using DVD CSS authentication and key management procedures. This feature is current only if a media containing CSS-protected DVD-Video and/or CPPM-protected DVDAudio content is loaded.</summary>
	[Description("This feature identifies a drive that is able to perform DVD CSS/CPPM authentication and key management.\r\nThis feature identifies drives that support CSS for DVD-Video and CPPM for DVD-Audio.\r\nThe drive maintains the integrity of the keys by only using DVD CSS authentication and key management procedures.\r\nThis feature is current only if a media containing CSS-protected DVD-Video and/or CPPM-protected DVDAudio content is loaded.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class DvdContentScramblingSystemFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.ReportKey, ScsiCommandCode.SendKey, ScsiCommandCode.ReadDiscStructure });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public DvdContentScramblingSystemFeature() : base(FeatureCode.DvdContentScramblingSystem) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _ContentScramblingSystemVersion;
		[DisplayName("Content Scrambling System Version")]
		public byte ContentScramblingSystemVersion { get { return this._ContentScramblingSystemVersion; } set { this._ContentScramblingSystemVersion = value; } }
	}
}