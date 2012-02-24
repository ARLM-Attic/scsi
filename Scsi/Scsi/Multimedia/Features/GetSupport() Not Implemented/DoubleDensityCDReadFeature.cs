using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Scsi.Multimedia
{
	/// <summary></summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class DoubleDensityCDReadFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.ReadCD, ScsiCommandCode.ReadTocPmaAtip });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public DoubleDensityCDReadFeature() : base(FeatureCode.DoubleDensityCDRead) { }
	}
}