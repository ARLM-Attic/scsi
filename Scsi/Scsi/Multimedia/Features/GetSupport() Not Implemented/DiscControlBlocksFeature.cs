using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that is able to read and/or write DCBs from or to the media.</summary>
	[Description("This feature identifies a drive that is able to read and/or write DCBs from or to the media.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class DiscControlBlocksFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr DCB_ENTRIES_OFFSET = Marshal.OffsetOf(typeof(DiscControlBlocksFeature), "_DCBEntries");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.ReadDiscStructure, ScsiCommandCode.SendDiscStructure });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public DiscControlBlocksFeature() : base(FeatureCode.DiscControlBlocks) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private uint[] _DCBEntries;
		[DisplayName("Disc Control Block Entries")]
		public uint[] DCBEntries
		{
			get { return this._DCBEntries; }
			set
			{
				this._DCBEntries = value;
				this.AdditionalLength = (byte)(4 * (value == null ? 0 : value.Length));
			}
		}

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._DCBEntries = new uint[this.AdditionalLength / 4];
			unsafe
			{
				var entriesBuffer = buffer.ExtractSegment(DCB_ENTRIES_OFFSET);
				for (int i = 0; i < this._DCBEntries.Length; i++)
				{
					this._DCBEntries[i] = Bits.BigEndian(entriesBuffer.Read<uint>(i * sizeof(uint)));
				}
			}
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			unsafe
			{
				var entries = buffer.ExtractSegment(DCB_ENTRIES_OFFSET);
				for (int i = 0; i < this._DCBEntries.Length; i++)
				{
					entries.Write(Bits.BigEndian(this._DCBEntries[i]), i * sizeof(uint));
				}
			}
		}
	}
}