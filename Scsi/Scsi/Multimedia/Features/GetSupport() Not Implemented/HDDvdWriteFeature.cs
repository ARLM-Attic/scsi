﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature indicates the ability to write to HD DVD-R/-RW media.</summary>
	[Description("This feature indicates the ability to write to HD DVD-R/-RW media.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class HDDvdWriteFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.Format, ScsiCommandCode.Write10, ScsiCommandCode.Write12, ScsiCommandCode.WriteAndVerify10 });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public HDDvdWriteFeature() : base(FeatureCode.HDDvdWrite) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DisplayName("Write HD DVD-R")]
		public bool HDDvdR { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DisplayName("Write HD DVD-RAM")]
		public bool HDDvdRam { get { return Bits.GetBit(this.byte6, 0); } set { this.byte6 = Bits.SetBit(this.byte6, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
	}
}