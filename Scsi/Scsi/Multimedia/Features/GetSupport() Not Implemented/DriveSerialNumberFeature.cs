using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies a drive that has a unique serial number. The vendor ID, model ID, and serial number is able to uniquely identify a drive that has this feature.</summary>
	[Description("This feature identifies a drive that has a unique serial number.\r\nThe vendor ID, model ID, and serial number is able to uniquely identify a drive that has this feature.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class DriveSerialNumberFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr SERIAL_NUMBER_OFFSET = Marshal.OffsetOf(typeof(DriveSerialNumberFeature), "_SerialNumber");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public DriveSerialNumberFeature() : base(FeatureCode.DriveSerialNumber) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private byte[] _SerialNumber;
		[Browsable(false)]
		public byte[] SerialNumber
		{
			get { return this._SerialNumber; }
			set
			{
				this._SerialNumber = value;
				this.AdditionalLength = (byte)(1 * (value == null ? 0 : value.Length));
			}
		}
		[DisplayName("Drive Serial Number")]
		public string StringSerialNumber { get { return System.BitConverter.ToString(this._SerialNumber); } }

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this._SerialNumber = new byte[this.AdditionalLength / 1];
			buffer.CopyTo(SERIAL_NUMBER_OFFSET, this._SerialNumber, IntPtr.Zero, (IntPtr)this._SerialNumber.Length);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.CopyFrom(SERIAL_NUMBER_OFFSET, this._SerialNumber, IntPtr.Zero, (IntPtr)this._SerialNumber.Length);
		}
	}
}