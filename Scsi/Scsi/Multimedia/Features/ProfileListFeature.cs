using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi.Multimedia
{
	/// <summary>This feature identifies profiles supported by the drive.</summary>
	[Description("This feature identifies profiles supported by the drive.")]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ProfileListFeature : MultimediaFeature
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr PROFILE_DESCRIPTORS_OFFSET = Marshal.OffsetOf(typeof(ProfileListFeature), "_ProfileDescriptors");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ScsiCommandCode[] __SupportedOperations = Sort(new ScsiCommandCode[] { });

		public override FeatureSupportKind GetSupport(ScsiCommand command)
		{
			return Array.BinarySearch<ScsiCommandCode>(__SupportedOperations, command.OpCode) >= 0 ? FeatureSupportKind.Mandatory : FeatureSupportKind.None;
		}

		public ProfileListFeature() : base(FeatureCode.ProfileList) { }

		[DisplayName("Current Disc Type")]
		public MultimediaProfile CurrentProfile { get { foreach (var item in this.ProfileDescriptors) { if (item.IsCurrentProfile) { return item.Profile; } } return 0; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		private ProfileDescriptor[] _ProfileDescriptors;
		[Browsable(false)]
		[DisplayName("Supported Disc Types")]
		public ProfileDescriptor[] ProfileDescriptors
		{
			get { return this._ProfileDescriptors; }
			set
			{
				this._ProfileDescriptors = value;
				this.AdditionalLength = (byte)(value == null ? 0 : Marshaler.DefaultSizeOf<ProfileDescriptor>() * value.Length);
			}
		}
		[DisplayName("Supported Disc Types"), Obsolete("This is only for debugging or viewing, not programmatic usage.")]
		public MultimediaProfileFlags SupportedProfiles
		{
			get
			{
				MultimediaProfileFlags flags = 0;
				foreach (var item in this.ProfileDescriptors) { flags |= (MultimediaProfileFlags)Enum.Parse(typeof(MultimediaProfileFlags), item.Profile.ToString()); }
				return flags;
			}
			set { Trace.WriteLine("SupportedProfiles should NOT be set!"); }
		}

		protected override void MarshalFrom(BufferWithSize buffer)
		{
			Marshaler.DefaultPtrToStructure(buffer, this);
			var count = this.AdditionalLength / Marshaler.DefaultSizeOf<ProfileDescriptor>();
			this._ProfileDescriptors = new ProfileDescriptor[count];
			unsafe
			{
				var descriptors = buffer.ExtractSegment(PROFILE_DESCRIPTORS_OFFSET);
				for (int i = 0; i < count; i++)
				{
					this._ProfileDescriptors[i] = descriptors.Read<ProfileDescriptor>(i * sizeof(ProfileDescriptor));
				}
			}
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			Marshaler.DefaultStructureToPtr((object)this, buffer);
			if (this._ProfileDescriptors != null)
			{
				unsafe
				{
					var descriptors = buffer.ExtractSegment(PROFILE_DESCRIPTORS_OFFSET);
					for (int i = 0; i < this._ProfileDescriptors.Length; i++)
					{
						descriptors.Write(this._ProfileDescriptors[i], i * sizeof(ProfileDescriptor));
					}
				}
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct ProfileDescriptor : IMarshalable
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private MultimediaProfile _Profile;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[DisplayName("Profile Name")]
			public MultimediaProfile Profile { get { return Bits.BigEndian(this._Profile); } set { this._Profile = Bits.BigEndian(value); } }
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private byte byte2;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[DisplayName("Current Profile")]
			public bool IsCurrentProfile { get { return Bits.GetBit(this.byte2, 0); } set { this.byte2 = Bits.SetBit(this.byte2, 0, value); } }
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private byte byte3;

			public void MarshalFrom(BufferWithSize buffer) { this = buffer.Read<ProfileDescriptor>(); }
			public void MarshalTo(BufferWithSize buffer) { buffer.Write(this); }
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[Browsable(false)]
			public int MarshaledSize { get { return Marshaler.DefaultSizeOf<ProfileDescriptor>(); } }

			public override string ToString() { return string.Format("ProfileDescriptor {{Profile = {0}, Current = {1}}}", this.Profile, this.IsCurrentProfile); }
		}

		private static string GetCDDriveType(IEnumerable<ProfileDescriptor> profilesSupported)
		{
			bool rom = false, r = false, rw = false;

			foreach (var profile in profilesSupported)
			{
				switch (profile.Profile)
				{
					case MultimediaProfile.CDROM:
						rom |= true;
						break;
					case MultimediaProfile.CDR:
						r |= true;
						break;
					case MultimediaProfile.CDRW:
						rw |= true;
						break;
				}
			}

			if (rw) { return "CD-RW"; }
			else if (r) { return "CD-R"; }
			else if (rom) { return "CD-ROM"; }
			else { return null; }
		}

		private static string GetDvdDriveType(IEnumerable<ProfileDescriptor> profilesSupported)
		{
			bool rom = false, minusR = false, minusRW = false, plusR = false, plusRW = false, minusRDL = false, minusRWDL = false, plusRDL = false, plusRWDL = false, ram = false;

			foreach (var profile in profilesSupported)
			{
				switch (profile.Profile)
				{
					case MultimediaProfile.DvdRom:
						rom |= true;
						break;
					case MultimediaProfile.DvdMinusRSequentialRecording:
						minusR |= true;
						break;
					case MultimediaProfile.DvdMinusRDualLayerSequentialRecording:
					case MultimediaProfile.DvdMinusRDualLayerJumpRecording:
						minusRDL |= true;
						break;
					case MultimediaProfile.DvdMinusRWRestrictedOverwrite:
					case MultimediaProfile.DvdMinusRWSequentialRecording:
						minusRW |= true;
						break;
					case MultimediaProfile.DvdMinusRWDualLayer:
						minusRWDL |= true;
						break;
					case MultimediaProfile.DvdPlusR:
						plusR |= true;
						break;
					case MultimediaProfile.DvdPlusRDualLayer:
						plusRDL |= true;
						break;
					case MultimediaProfile.DvdPlusRW:
						plusRW |= true;
						break;
					case MultimediaProfile.DvdPlusRWDualLayer:
						plusRWDL |= true;
						break;
					case MultimediaProfile.DvdRam:
						ram = true;
						break;
				}
			}

			var types = new List<string>();
			char c;
			if (minusR | minusRDL | minusRW | minusRWDL) { if (plusR | plusRDL | plusRW | plusRWDL) { c = '±'; } else { c = '-'; } }
			else { if (plusR | plusRDL | plusRW | plusRWDL) { c = '-'; } else { return null; } }

			if ((minusRWDL | plusRWDL) | ((minusRW | plusRW) & (minusRDL | plusRDL))) { types.Add(string.Format("DVD{0}RW DL", c)); }
			else if (minusRW | plusRW) { types.Add(string.Format("DVD{0}RW", c)); }
			else if (minusRDL | plusRDL) { types.Add(string.Format("DVD{0}R DL", c)); }
			else if (minusR | plusR) { types.Add(string.Format("DVD{0}R", c)); }

			if (ram) { types.Add("DVD-RAM"); }

			return types.Count > 0 ? string.Join("/", types.ToArray()) : null;
		}

		private static string GetBDDriveType(IEnumerable<ProfileDescriptor> profilesSupported)
		{
			bool rom = false, r = false, re = false;

			foreach (var profile in profilesSupported)
			{
				switch (profile.Profile)
				{
					case MultimediaProfile.BDROM:
						rom = true;
						break;
					case MultimediaProfile.BDRSequentialRecording:
						r |= true;
						break;
					case MultimediaProfile.BDRE:
					case MultimediaProfile.BDRERandomRecording:
						re |= true;
						break;
				}
			}

			if (re) { return "BD-RE"; }
			else if (r) { return "BD-R"; }
			else if (rom) { return "BD-ROM"; }
			else { return null; }
		}

		private static string GetHddvdDriveType(IEnumerable<ProfileDescriptor> profilesSupported)
		{
			bool rom = false, minusR = false, minusRW = false, minusRDL = false, minusRWDL = false, ram = false;

			foreach (var profile in profilesSupported)
			{
				switch (profile.Profile)
				{
					case MultimediaProfile.HDDvdRom:
						rom |= true;
						break;
					case MultimediaProfile.HDDvdR:
						minusR |= true;
						break;
					case MultimediaProfile.HDDvdRam:
						ram |= true;
						break;
					case MultimediaProfile.HDDvdRW:
						minusRW |= true;
						break;
					case MultimediaProfile.HDDvdRDualLayer:
						minusRDL |= true;
						break;
					case MultimediaProfile.HDDvdRWDualLayer:
						minusRWDL |= true;
						break;
				}
			}

			var types = new List<string>();

			if (minusRWDL) { types.Add("HD DVD-RW DL"); }
			else if (minusRDL) { types.Add("HD DVD-R DL"); }
			else if (minusRW) { types.Add("HD DVD-RW"); }
			else if (minusR) { types.Add("HD DVD-R"); }

			if (ram) { types.Add("HD DVD-RAM"); }

			return types.Count > 0 ? string.Join("/", types.ToArray()) : null;
		}

		public string GetCDDriveType() { return GetCDDriveType(this.ProfileDescriptors); }
		public string GetDvdDriveType() { return GetDvdDriveType(this.ProfileDescriptors); }
		public string GetBDDriveType() { return GetBDDriveType(this.ProfileDescriptors); }
		public string GetHddvdDriveType() { return GetHddvdDriveType(this.ProfileDescriptors); }
		public string GetDriveType() { return JoinIgnoreNulls("/", new string[] { this.GetCDDriveType(), this.GetDvdDriveType(), this.GetHddvdDriveType(), this.GetBDDriveType() }); }
		private static string JoinIgnoreNulls(string separator, string[] strings) { var result = new List<string>(); for (int i = 0; i < strings.Length; i++) { if (strings[i] != null) { result.Add(strings[i]); } } return string.Join(separator, result.ToArray()); }
	}
}