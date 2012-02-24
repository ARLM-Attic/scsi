using System;
using Helper;

namespace Scsi.Stream
{
	/// <summary>Represents a block device (e.g. a hard disk).</summary>
	public class StreamDevice : ScsiDevice
	{
		/// <summary>Initializes a new instance of the <see cref="StreamDevice"/> class.</summary>
		/// <param name="interface">The pass-through interface to use.</param>
		/// <param name="leaveOpen"><c>true</c> to leave the interface open, or <c>false</c> to dispose along with this object.</param>
		public StreamDevice(IScsiPassThrough @interface, bool leaveOpen) : base(@interface, leaveOpen) { }



	}
}