using System.Runtime.InteropServices;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class MountRainierRewritableFullFormatDescriptor : FormatDescriptorOther
	{
		public MountRainierRewritableFullFormatDescriptor() : base(FormatType.MountRainierRewritableFormat) { this.NumberOfBlocks = ~0U; }
		public MountRainierRewritableFullFormatDescriptor(MountRainierRewritableFormatOption options) : this() { this.FormatOptions = options; }
		public MountRainierRewritableFormatOption FormatOptions { get { return (MountRainierRewritableFormatOption)this.TypeDependentParameter; } set { this.TypeDependentParameter = (uint)value; } }
	}

	public enum MountRainierRewritableFormatOption : int
	{
		NewFormat = 0x00000000,
		RestartFormat = 0x00000001,
	}
}