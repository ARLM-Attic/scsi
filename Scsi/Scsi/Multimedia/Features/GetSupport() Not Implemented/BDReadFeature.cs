using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that is able to read control structures and user data from the BD disc.</summary>
	[Description("This feature identifies a drive that is able to read control structures and user data from the BD disc.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class BDReadFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { ScsiCommandCode.Read10, ScsiCommandCode.Read12, ScsiCommandCode.ReadDiscStructure, ScsiCommandCode.ReadTocPmaAtip });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public BDReadFeature() : base(FeatureCode.BDRead) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte6;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte7;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class0BDREReadSupport;
		[DisplayName("BD-RE Read Support, Class 0 Bitmap")]
		public short Class0BDREReadSupport { get { return this._Class0BDREReadSupport; } set { this._Class0BDREReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class1BDREReadSupport;
		[DisplayName("BD-RE Read Support, Class 1 Bitmap")]
		public short Class1BDREReadSupport { get { return this._Class1BDREReadSupport; } set { this._Class1BDREReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class2BDREReadSupport;
		[DisplayName("BD-RE Read Support, Class 2 Bitmap")]
		public short Class2BDREReadSupport { get { return this._Class2BDREReadSupport; } set { this._Class2BDREReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class3BDREReadSupport;
		[DisplayName("BD-RE Read Support, Class 3 Bitmap")]
		public short Class3BDREReadSupport { get { return this._Class3BDREReadSupport; } set { this._Class3BDREReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class0BDRReadSupport;
		[DisplayName("BD-R Read Support, Class 0 Bitmap")]
		public short Class0BDRReadSupport { get { return this._Class0BDRReadSupport; } set { this._Class0BDRReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class1BDRReadSupport;
		[DisplayName("BD-R Read Support, Class 1 Bitmap")]
		public short Class1BDRReadSupport { get { return this._Class1BDRReadSupport; } set { this._Class1BDRReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class2BDRReadSupport;
		[DisplayName("BD-R Read Support, Class 2 Bitmap")]
		public short Class2BDRReadSupport { get { return this._Class2BDRReadSupport; } set { this._Class2BDRReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class3BDRReadSupport;
		[DisplayName("BD-R Read Support, Class 3 Bitmap")]
		public short Class3BDRReadSupport { get { return this._Class3BDRReadSupport; } set { this._Class3BDRReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class0BDRomReadSupport;
		[DisplayName("BD-ROM Read Support, Class 0 Bitmap")]
		public short Class0BDRomReadSupport { get { return this._Class0BDRomReadSupport; } set { this._Class0BDRomReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class1BDRomReadSupport;
		[DisplayName("BD-ROM Read Support, Class 1 Bitmap")]
		public short Class1BDRomReadSupport { get { return this._Class1BDRomReadSupport; } set { this._Class1BDRomReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class2BDRomReadSupport;
		[DisplayName("BD-ROM Read Support, Class 2 Bitmap")]
		public short Class2BDRomReadSupport { get { return this._Class2BDRomReadSupport; } set { this._Class2BDRomReadSupport = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private short _Class3BDRomReadSupport;
		[DisplayName("BD-ROM Read Support, Class 3 Bitmap")]
		public short Class3BDRomReadSupport { get { return this._Class3BDRomReadSupport; } set { this._Class3BDRomReadSupport = value; } }
	}
}