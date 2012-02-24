using System.Collections.ObjectModel;
using System.Diagnostics;
using Helper;

namespace Scsi.Multimedia
{
	public class FeatureCollection : KeyedCollection<FeatureCode, MultimediaFeature>
	{
		public FeatureCollection() : base() { }

		protected override FeatureCode GetKeyForItem(MultimediaFeature item) { return item.FeatureCode; }

		internal static FeatureCollection FromBuffer(BufferWithSize buffer)
		{
			var header = Marshaler.PtrToStructure<FeatureHeader>(buffer);
			var result = new FeatureCollection();
			int currentOffset = Marshaler.DefaultSizeOf<FeatureHeader>();
			while (currentOffset < header.DataLength + sizeof(uint))
			{
				var currentBuf = buffer.ExtractSegment(currentOffset);
				int additionalLength = MultimediaFeature.ReadAdditionalLength(currentBuf);
				int length = Marshaler.DefaultSizeOf<MultimediaFeature>() + additionalLength;
				currentBuf = currentBuf.ExtractSegment(0, length);
				var feature = MultimediaFeature.FromBuffer(currentBuf);
				result.Add(feature);
				Debug.Assert(length == feature.AdditionalLength + Marshaler.DefaultSizeOf<MultimediaFeature>());
				currentOffset += length;
			}
			return result;
		}
	}
}