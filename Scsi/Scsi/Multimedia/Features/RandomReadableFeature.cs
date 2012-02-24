using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that is able to read data from logical blocks referenced by logical block addresses, but not requiring that either the addresses or the read sequences occur in any particular order.</summary>
	[Description("This feature identifies a drive that is able to read data from logical blocks referenced by logical block addresses, but not requiring that either the addresses or the read sequences occur in any particular order.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class RandomReadableFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.ReadCapacity, ScsiCommandCode.Read10 });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public RandomReadableFeature() : base(FeatureCode.RandomReadable) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _LogicalBlockSize;
		[DisplayName("Logical Block Size")]
		public uint LogicalBlockSize { get { return Bits.BigEndian(this._LogicalBlockSize); } set { this._LogicalBlockSize = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _Blocking;
		[DisplayName("Logical Blocks per Readable Unit")]
		public ushort Blocking { get { return Bits.BigEndian(this._Blocking); } set { this._Blocking = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte10;
		[DisplayName("Read/Write Error Recovery Page Present")]
		public bool PagePresent { get { return Bits.GetBit(this.byte10, 0); } set { this.byte10 = Bits.SetBit(this.byte10, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte11;
	}
}