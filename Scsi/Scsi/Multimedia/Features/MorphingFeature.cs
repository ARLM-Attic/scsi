using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies the ability of the drive to notify a host about operational changes and accept host requests to prevent operational changes.</summary>
	[Description("This feature identifies the ability of the drive to notify a host about operational changes and accept host requests to prevent operational changes.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class MorphingFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.GetConfiguration, ScsiCommandCode.GetEventStatusNotification, ScsiCommandCode.PreventAllowMediumRemoval });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0
				&& (command.OpCode != ScsiCommandCode.PreventAllowMediumRemoval || ((PreventAllowMediumRemovalCommand)command).Persistent == true)
				? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public MorphingFeature() : base(FeatureCode.Morphing) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DisplayName("Asynchronous Event Notification")]
		public bool AsyncEventNotify { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
	}
}