﻿using System.Runtime.InteropServices;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class FullFormatDescriptor : FormatDescriptorOther
	{
		public FullFormatDescriptor() : base(FormatType.FullFormat) { }
		public FullFormatDescriptor(uint blockLength) : this() { this.BlockLength = blockLength; }
		public uint BlockLength { get { return this.TypeDependentParameter; } set { this.TypeDependentParameter = value; } }
	}
}