using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that is able to move media from a storage area to a mechanism and back.</summary>
	[Description("This feature identifies a drive that is able to move media from a storage area to a mechanism and back.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class EmbeddedChangerFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte HIGHEST_SLOT_NUMBER_MASK = 0x1F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.LoadUnloadMedium, ScsiCommandCode.MechanismStatus });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public EmbeddedChangerFeature() : base(FeatureCode.EmbeddedChanger) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DisplayName("Report Slot Contents")]
		public bool GetSupportDiscPresent { get { return Bits.GetBit(this.byte4, 2); } set { this.byte4 = Bits.SetBit(this.byte4, 2, value); } }
		[DisplayName("Change Media Side")]
		public bool SideChangeCapable { get { return Bits.GetBit(this.byte4, 4); } set { this.byte4 = Bits.SetBit(this.byte4, 4, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
		[DisplayName("Highest Slot Number")]
		public byte HighestSlotNumber
		{
			get { return Bits.GetValueMask(this.byte7, 0, HIGHEST_SLOT_NUMBER_MASK); }
			set { this.byte7 = Bits.PutValueMask(this.byte7, value, 0, HIGHEST_SLOT_NUMBER_MASK); }
		}
	}
}