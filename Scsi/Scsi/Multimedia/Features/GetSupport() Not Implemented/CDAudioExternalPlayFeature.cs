﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary></summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class CDAudioExternalPlayFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.MechanismStatus, ScsiCommandCode.Pause, ScsiCommandCode.Resume, ScsiCommandCode.PlayAudio10, ScsiCommandCode.PlayAudioMinuteSecondFrame, ScsiCommandCode.ReadTocPmaAtip, ScsiCommandCode.ReadSubChannel, ScsiCommandCode.Seek, ScsiCommandCode.StopPlayScan });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public CDAudioExternalPlayFeature() : base(FeatureCode.CDAudioExternalPlay) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		public bool Scan { get { return Bits.GetBit(this.byte4, 2); } set { this.byte4 = Bits.SetBit(this.byte4, 2, value); } }
		[DisplayName("Separate Channel Mute")]
		public bool SeparateChannelMute { get { return Bits.GetBit(this.byte4, 1); } set { this.byte4 = Bits.SetBit(this.byte4, 1, value); } }
		[DisplayName("Separate Channel Volume")]
		public bool SeparateChannelVolume { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _NumberOfVolumeLevels;
		[DisplayName("Number of Volume Levels")]
		public ushort NumberOfVolumeLevels { get { return Bits.BigEndian(this._NumberOfVolumeLevels); } set { this._NumberOfVolumeLevels = Bits.BigEndian(value); } }
	}
}