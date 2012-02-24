using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class FormatDescriptorCDRW : FormatDescriptor
	{
		public FormatDescriptorCDRW() : base() { }

		public FormatDescriptorCDRW(bool session, bool grow, uint formatSize)
			: this()
		{
			this.Session = session;
			this.Grow = grow;
			this.FormatSize = formatSize;
			this.FormatOptionsValid = true;
		}

		public FormatDescriptorCDRW(bool session, bool grow, uint formatSize, bool immediate, bool tryOut, bool disablePrimary, bool disableCertification)
			: this(session, grow, formatSize)
		{
			this.Immediate = immediate;
			this.TryOut = tryOut;
			this.DisablePrimary = disablePrimary;
			this.DisableCertification = disableCertification;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		public bool Session { get { return Bits.GetBit(this.byte0, 7); } set { this.byte0 = Bits.SetBit(this.byte0, 7, value); } }
		public bool Grow { get { return Bits.GetBit(this.byte0, 6); } set { this.byte0 = Bits.SetBit(this.byte0, 6, value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _FormatSize;
		/// <summary>The number of user data blocks. Must be divisible by the <see cref="WriteParametersPage.PacketSize"/> field.</summary>
		[Description("The number of user data blocks. Must be divisible by the PacketSize field.")]
		public uint FormatSize { get { return Bits.BigEndian(this._FormatSize); } set { this._FormatSize = Bits.BigEndian(value); } }


		public override FormatCode FormatCode { get { return FormatCode.CDRW; } }
	}
}