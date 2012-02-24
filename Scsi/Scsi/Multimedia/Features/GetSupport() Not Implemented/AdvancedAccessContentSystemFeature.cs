using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that GetSupport AACS and is able to perform AACS authentication process.</summary>
	[Description("This feature identifies a drive that GetSupport AACS and is able to perform AACS authentication process.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class AdvancedAccessContentSystemFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte NUMBER_OF_AuthenticationGrantIdS_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.ReportKey, ScsiCommandCode.SendKey, ScsiCommandCode.ReadDiscStructure });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public AdvancedAccessContentSystemFeature() : base(FeatureCode.AdvancedAccessContentSystem) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DisplayName("Binding Nonce")]
		public bool BindingNonce { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _BindingNonceBlockCount;
		[DisplayName("Binding Nonce Block Count")]
		public byte BindingNonceBlockCount { get { return this._BindingNonceBlockCount; } set { this._BindingNonceBlockCount = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DisplayName("Authentication Grant ID Count")]
		public byte AuthenticationGrantIdCount
		{
			get { return Bits.GetValueMask(this.byte6, 0, NUMBER_OF_AuthenticationGrantIdS_MASK); }
			set { this.byte6 = Bits.PutValueMask(this.byte6, value, 0, NUMBER_OF_AuthenticationGrantIdS_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _AdvancedAccessContentSystemVersion;
		[DisplayName("Advanced Access Content System Version")]
		public byte AdvancedAccessContentSystemVersion { get { return this._AdvancedAccessContentSystemVersion; } set { this._AdvancedAccessContentSystemVersion = value; } }
	}
}