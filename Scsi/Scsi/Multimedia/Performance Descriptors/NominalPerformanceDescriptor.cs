﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class NominalPerformanceDescriptor : PerformanceDescriptor
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _StartLogicalBlockAddress;
		/// <summary>The first logical block address of the extent described by this descriptor.</summary>
		public uint StartLogicalBlockAddress { get { return Bits.BigEndian(this._StartLogicalBlockAddress); } set { this._StartLogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _StartPerformance;
		/// <summary>The nominal Drive performance at the Start LBA, in increments of <c>1000</c> (NOT <c>1024</c>) bytes per second.</summary>
		public uint StartPerformance { get { return Bits.BigEndian(this._StartPerformance); } set { this._StartPerformance = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _EndLogicalBlockAddress;
		/// <summary>The last logical block address of the extent described by this descriptor.</summary>
		public uint EndLogicalBlockAddress { get { return Bits.BigEndian(this._EndLogicalBlockAddress); } set { this._EndLogicalBlockAddress = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _EndPerformance;
		/// <summary>The nominal Drive performance at the End LBA, in increments of <c>1000</c> (NOT <c>1024</c>) bytes per second.</summary>
		public uint EndPerformance { get { return Bits.BigEndian(this._EndPerformance); } set { this._EndPerformance = Bits.BigEndian(value); } }
	}
}