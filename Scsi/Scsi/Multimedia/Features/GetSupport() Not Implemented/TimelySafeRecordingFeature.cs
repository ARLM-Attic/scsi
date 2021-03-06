﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Scsi.Multimedia
{
	/// <summary>A drive that reports this feature is able to detect and report defective writable units and to manage the defect or not according to instructions from the host.</summary>
	[Description("A drive that reports this feature is able to detect and report defective writable units and to manage the defect or not according to instructions from the host.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class TimelySafeRecordingFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.GetPerformance, ScsiCommandCode.SynchronizeCache10, ScsiCommandCode.Write10, ScsiCommandCode.Write12 });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public TimelySafeRecordingFeature() : base(FeatureCode.TimelySafeRecording) { }
	}
}