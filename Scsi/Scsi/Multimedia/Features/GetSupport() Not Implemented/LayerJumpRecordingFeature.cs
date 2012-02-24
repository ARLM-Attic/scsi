using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that is able to write data to contiguous regions that are allocated on multiple layers, and is able to append data to a limited number of locations on the media. The drive may write two or more recording layers sequentially and alternately.</summary>
	[Description("This feature identifies a drive that is able to write data to contiguous regions that are allocated on multiple layers, and is able to append data to a limited number of locations on the media.\r\nThe drive may write two or more recording layers sequentially and alternately.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class LayerJumpRecordingFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr LINK_SIZES_OFFSET = Marshal.OffsetOf(typeof(LayerJumpRecordingFeature), "_LinkSizes");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public LayerJumpRecordingFeature() : base(FeatureCode.LayerJumpRecording) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _NumberOfLinkSizes;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private byte[] _LinkSizes;
		[DisplayName("Link Sizes (in blocks)")]
		public byte[] LinkSizes
		{
			get { return this._LinkSizes; }
			set
			{
				this._LinkSizes = value;
				this._NumberOfLinkSizes = value == null ? (byte)0 : (byte)value.Length;
				this.AdditionalLength = (byte)(4 + this._NumberOfLinkSizes + 7 + this.CalculatePadBytes()); ;
			}
		}

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._LinkSizes = new byte[this.AdditionalLength - this._NumberOfLinkSizes - this.CalculatePadBytes()];
			buffer.CopyTo(LINK_SIZES_OFFSET, this._LinkSizes, IntPtr.Zero, (IntPtr)this._LinkSizes.Length);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.CopyFrom(LINK_SIZES_OFFSET, this._LinkSizes, IntPtr.Zero, (IntPtr)this._LinkSizes.Length);
		}

		private byte CalculatePadBytes() { return (byte)(3 - (this._NumberOfLinkSizes + 3) % 4); }
	}
}