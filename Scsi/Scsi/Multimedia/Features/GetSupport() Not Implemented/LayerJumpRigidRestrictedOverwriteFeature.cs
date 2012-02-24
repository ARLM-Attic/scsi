using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature indicate the ability to write in layer jump recording mode and the ability to overwrite the logically recorded blocks, but only in blocking boundaries. The layer Jump <see cref="RigidRestrictedOverwriteFeature"/> and the <see cref="RandomWritableFeature"/> are not be concurrently current. If the mounted medium is write protected, this feature is not current. This feature and the <see cref="RigidRestrictedOverwriteFeature"/> may be concurrently current, but when the current recording mode of the mounted disc is layer jump recording mode, <see cref="RigidRestrictedOverwriteFeature"/> is not current.</summary>
	[Description("This feature indicate the ability to write in layer jump recording mode and the ability to overwrite the logically recorded blocks, but only in blocking boundaries.\r\nThe layer Jump RigidRestrictedOverwriteFeature and the RandomWritableFeature are not be concurrently current.\r\nIf the mounted medium is write protected, this feature is not current.\r\nThis feature and the RigidRestrictedOverwriteFeature may be concurrently current, but when the current recording mode of the mounted disc is layer jump recording mode, RigidRestrictedOverwriteFeature is not current.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class LayerJumpRigidRestrictedOverwriteFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.CloseSessionOrTrack, ScsiCommandCode.ReadCapacity, ScsiCommandCode.ReadDiscInformation, ScsiCommandCode.ReadDiscStructure, ScsiCommandCode.ReadTrackInformation, ScsiCommandCode.SendDiscStructure, ScsiCommandCode.SynchronizeCache10, ScsiCommandCode.Verify10, ScsiCommandCode.Write10 });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public LayerJumpRigidRestrictedOverwriteFeature() : base(FeatureCode.LayerJumpRigidRestrictedOverwrite) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DisplayName("Close Layer-Jump Block")]
		public bool CloseLayerJumpBlock { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
		[DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)]
		private byte _BufferBlockSize;
		[DisplayName("Buffer Size (in blocks)")]
		public byte BufferBlockSize { get { return this._BufferBlockSize; } set { this._BufferBlockSize = value; } }
	}
}