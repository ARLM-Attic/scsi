using System;
using System.ComponentModel;
using System.Diagnostics;
using Helper;
namespace Ata
{
	/// <summary>Represents a generic ATA device. This class should not be instantiated directly unless no subclasses exist that are appropriate.</summary>
	[Description("Represents a generic ATA device. This class should not be instantiated directly unless no subclasses exist that are appropriate.")]
	public class AtaDevice : IDisposable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _LogicalSectorSize;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int _Is48LBA = -1; //Boolean... but we need 3 states
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int _DmaSupported = -1; //Boolean... but we need 3 states
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool leaveOpen;

		/// <summary>Initializes a new instance of the <see cref="AtaDevice"/> class.</summary>
		/// <param name="interface">The pass-through interface to use.</param>
		/// <param name="leaveOpen"><c>true</c> to leave the interface open, or <c>false</c> to dispose along with this object.</param>
		public AtaDevice(IAtaPassThrough @interface, bool leaveOpen) { this.leaveOpen = leaveOpen; this.Interface = @interface; }

		public byte? AdvancedPowerManagementLevel { get { var id = this.IdentifyDevice(); return id.AdvancedPowerManagementFeatureSetEnabled ? id.AdvancedPowerManagementLevelCurrent : (byte?)null; } set { if (value != null) { this.SetFeatures(0x05, value.Value, 0); } else { this.SetFeatures(0x85, 0, 0); } } }

		public byte? AutomaticAcousticManagementLevel { get { var id = this.IdentifyDevice(); return id.AutomaticAcousticManagementFeatureSetEnabled ? id.AutomaticAcousticManagementLevelCurrent : (byte?)null; } set { if (value != null) { this.SetFeatures(0x42, value.Value, 0); } else { this.SetFeatures(0xC2, 0, 0); } } }

		public void DeviceReset() { var task = new AtaTaskFile(AtaCommand.DeviceReset); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		public void Dispose() { this.Dispose(true); GC.SuppressFinalize(this); }

		public bool DmaSupported { get { if (this._DmaSupported != -1) { this._DmaSupported = this.IdentifyDevice().DmaSupported ? 1 : 0; } return this._DmaSupported != 0; } }

		protected virtual void Dispose(bool disposing) { if (disposing) { try { if (!this.leaveOpen) { this.Interface.Dispose(); } } finally { this.Interface = null; } } }

		protected void ExecuteCommand28(ref AtaTaskFile task, BufferWithSize buffer, AtaFlags flags, bool checkError) { flags &= ~AtaFlags.Command48Bit; var high = new AtaTaskFile(); this.ExecuteCommandCore(ref task, ref high, buffer, flags, checkError); }

		protected void ExecuteCommand48(ref AtaTaskFile low, ref AtaTaskFile high, BufferWithSize buffer, AtaFlags flags, bool checkError) { flags |= AtaFlags.Command48Bit; this.ExecuteCommandCore(ref low, ref high, buffer, flags, checkError); }

		protected virtual void ExecuteCommandCore(ref AtaTaskFile low, ref AtaTaskFile high, BufferWithSize buffer, AtaFlags flags, bool checkError)
		{
			this.Interface.ExecuteCommand(ref low, ref high, buffer.Address, buffer.LengthU32, this.TimeoutSeconds, flags);
			if (checkError)
			{
				if (low.Error != AtaError.None) { throw AtaException.CreateException(low.Error); }
				//if (high.Error != AtaError.None) { throw AtaException.CreateException(high.Error); }
			}
		}

		public void FlushCache() { var task = new AtaTaskFile(AtaCommand.FlushCache); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		public void FlushCacheExt() { AtaTaskFile low = new AtaTaskFile(AtaCommand.FlushCache), high = low; this.ExecuteCommand48(ref low, ref high, BufferWithSize.Zero, AtaFlags.Command48Bit, true); }

		public byte? FreeFallSensitivityLevel { get { var id = this.IdentifyDevice(); return id.FreeFallControlFeatureSetEnabled ? id.FreeFallControlSensitivityLevelCurrent : (byte?)null; } set { if (value != null) { this.SetFeatures(0x41, value.Value, 0); } else { this.SetFeatures(0xC1, 0, 0); } } }

		public DeviceIdentifier IdentifyDevice()
		{
			var task = new AtaTaskFile(AtaCommand.IdentifyDevice);
			var id = new DeviceIdentifier();
			BufferWithSize buffer;
			unsafe { buffer = new BufferWithSize((IntPtr)(&id), Marshaler.DefaultSizeOf<DeviceIdentifier>()); }
			this.ExecuteCommand28(ref task, buffer, AtaFlags.ReceiveData, true);
			return id;
		}

		[Obsolete("Do not use. This is only for viewing the data in the debugger, because manipulating the returned data has no effect. Use the appropriate Get and Set methods instead.", true)]
		private DeviceIdentifier Identifier { get { return this.IdentifyDevice(); } }

		public void Idle() { var task = new AtaTaskFile(0, 0, 0, 0, AtaCommand.Idle, 0); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		public void IdleImmediate() { var task = new AtaTaskFile(0, 0, 0, 0, AtaCommand.IdleImmediate, 0); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		public IAtaPassThrough Interface { get; private set; }

		public uint LogicalSectorSize { get { if (this._LogicalSectorSize == 0) { var id = this.IdentifyDevice(); this._LogicalSectorSize = id.LogicalSectorSize; } return this._LogicalSectorSize; } }

		/// <summary>The highest addressable logical block address. Note that this is one less than the number of logical blocks on the disk.</summary>
		public int NativeMaximumAddress { get { var task = new AtaTaskFile(0, 0, 0, 1 << 6, AtaCommand.ReadNativeMaxAddress, 0); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.NoMultiple, true); return checked((int)task.LogicalBlockAddress); } }

		/// <summary>The highest addressable logical block address, in 48-bit form. Note that this is one less than the number of logical blocks on the disk.</summary>
		public long NativeMaximumAddressExt { get { var low = new AtaTaskFile(0, 0, 0, 1 << 6, AtaCommand.ReadNativeMaxAddressExt, 0); var high = new AtaTaskFile(0, 0, 0, 1 << 6, AtaCommand.ReadNativeMaxAddressExt, 0); this.ExecuteCommand48(ref low, ref high, BufferWithSize.Zero, AtaFlags.Command48Bit, true); return low.LogicalBlockAddress; } }

		public bool NonVolatileCacheEnabled { get { return this.IdentifyDevice().NonVolatileCacheFeatureSetEnabled; } set { if (value) { this.NonVolatileCacheCommand(0x0015, 0, 0, BufferWithSize.Zero, AtaFlags.None); } else { this.NonVolatileCacheCommand(0x0016, 0, 0, BufferWithSize.Zero, AtaFlags.None); } } }

		public byte[] ReadDma(int logicalBlockAddress, byte sectorCount) { var data = new byte[this.LogicalSectorSize * sectorCount]; this.ReadDma(logicalBlockAddress, sectorCount, data, 0); return data; }

		/// <param name="sectorCount">The number of sectors to read. IMPORTANT NOTE: If zero, then this value specifies <c>0x100</c> sectors.</param>
		public void ReadDma(int logicalBlockAddress, byte sectorCount, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.ReadDma(logicalBlockAddress, sectorCount, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		/// <param name="sectorCount">The number of sectors to read. IMPORTANT NOTE: If zero, then this value specifies <c>0x100</c> sectors.</param>
		public void ReadDma(int logicalBlockAddress, byte sectorCount, BufferWithSize buffer)
		{
			var sectorsToRead = (uint)(sectorCount == 0 ? 256 : sectorCount);
			if (sectorsToRead * this.LogicalSectorSize > buffer.LengthU32) { throw new ArgumentOutOfRangeException("buffer", buffer, "Buffer was too small."); }
			var task = new AtaTaskFile(0, sectorCount, checked((uint)logicalBlockAddress), 1 << 6, AtaCommand.ReadDma, 0);
			this.ExecuteCommand28(ref task, buffer, AtaFlags.ReceiveData | AtaFlags.UseDma, true);
		}

		public byte[] ReadDmaExt(long logicalBlockAddress, ushort sectorCount) { var data = new byte[this.LogicalSectorSize * sectorCount]; this.ReadDmaExt(logicalBlockAddress, sectorCount, data, 0); return data; }

		/// <param name="sectorCount">The number of sectors to read. IMPORTANT NOTE: If zero, then this value specifies <c>0x10000</c> sectors.</param>
		public void ReadDmaExt(long logicalBlockAddress, ushort sectorCount, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.ReadDmaExt(logicalBlockAddress, sectorCount, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		/// <param name="sectorCount">The number of sectors to read. IMPORTANT NOTE: If zero, then this value specifies <c>0x10000</c> sectors.</param>
		public void ReadDmaExt(long logicalBlockAddress, ushort sectorCount, BufferWithSize buffer)
		{
			var sectorsToRead = (uint)(sectorCount == 0 ? 256 : sectorCount);
			if (sectorsToRead * this.LogicalSectorSize > buffer.LengthU32) { throw new ArgumentOutOfRangeException("buffer", buffer, "Buffer was too small."); }
			var low = new AtaTaskFile(0, unchecked((byte)sectorCount), unchecked((uint)logicalBlockAddress & 0x00FFFFFFU), 1 << 6, AtaCommand.ReadDmaExt, 0);
			var high = new AtaTaskFile(0, checked((byte)(sectorCount >> 8)), checked((uint)(logicalBlockAddress >> 24)), 1 << 6, AtaCommand.ReadDmaExt, 0);
			this.ExecuteCommand48(ref low, ref high, buffer, AtaFlags.ReceiveData | AtaFlags.UseDma | AtaFlags.Command48Bit, true);
		}

		public bool ReadLookAhead { get { return this.IdentifyDevice().ReadLookAheadEnabled; } set { this.SetFeatures(value ? (byte)0xAA : (byte)0x55, 0, 0); } }

		public byte[] ReadSectors(int logicalBlockAddress, byte sectorCount) { var data = new byte[this.LogicalSectorSize * sectorCount]; this.ReadSectors(logicalBlockAddress, sectorCount, data, 0); return data; }

		/// <param name="sectorCount">The number of sectors to read. IMPORTANT NOTE: If zero, then this value specifies <c>0x100</c> sectors.</param>
		public void ReadSectors(int logicalBlockAddress, byte sectorCount, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.ReadSectors(logicalBlockAddress, sectorCount, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		/// <param name="sectorCount">The number of sectors to read. IMPORTANT NOTE: If zero, then this value specifies <c>0x100</c> sectors.</param>
		public void ReadSectors(int logicalBlockAddress, byte sectorCount, BufferWithSize buffer)
		{
			var sectorsToRead = (uint)(sectorCount == 0 ? 256 : sectorCount);
			if (sectorsToRead * this.LogicalSectorSize > buffer.LengthU32) { throw new ArgumentOutOfRangeException("buffer", buffer, "Buffer was too small."); }
			var task = new AtaTaskFile(0, sectorCount, checked((uint)logicalBlockAddress), 1 << 6, AtaCommand.ReadSectors, 0);
			this.ExecuteCommand28(ref task, buffer, AtaFlags.ReceiveData, true);
		}

		public byte[] ReadSectorsExt(long logicalBlockAddress, ushort sectorCount) { var data = new byte[this.LogicalSectorSize * sectorCount]; this.ReadSectorsExt(logicalBlockAddress, sectorCount, data, 0); return data; }

		/// <param name="sectorCount">The number of sectors to read. IMPORTANT NOTE: If zero, then this value specifies <c>0x10000</c> sectors.</param>
		public void ReadSectorsExt(long logicalBlockAddress, ushort sectorCount, byte[] buffer, int bufferOffset) { unsafe { fixed (byte* pBuffer = &buffer[bufferOffset]) { this.ReadSectorsExt(logicalBlockAddress, sectorCount, new BufferWithSize(pBuffer, buffer.Length - bufferOffset)); } } }

		/// <param name="sectorCount">The number of sectors to read. IMPORTANT NOTE: If zero, then this value specifies <c>0x10000</c> sectors.</param>
		public void ReadSectorsExt(long logicalBlockAddress, ushort sectorCount, BufferWithSize buffer)
		{
			var sectorsToRead = (uint)(sectorCount == 0 ? 256 : sectorCount);
			if (sectorsToRead * this.LogicalSectorSize > buffer.LengthU32) { throw new ArgumentOutOfRangeException("buffer", buffer, "Buffer was too small."); }
			var low = new AtaTaskFile(0, unchecked((byte)sectorCount), unchecked((uint)logicalBlockAddress & 0x00FFFFFFU), 1 << 6, AtaCommand.ReadSectorsExt, 0);
			var high = new AtaTaskFile(0, checked((byte)(sectorCount >> 8)), checked((uint)(logicalBlockAddress >> 24)), 1 << 6, AtaCommand.ReadSectorsExt, 0);
			this.ExecuteCommand48(ref low, ref high, buffer, AtaFlags.ReceiveData | AtaFlags.Command48Bit, true);
		}

		private void SetFeatures(byte feature, byte count, int logicalBlockAddress) { var task = new AtaTaskFile(feature, count, checked((uint)logicalBlockAddress), 0, AtaCommand.SetFeatures, 0); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		private void NonVolatileCacheCommand(byte feature, byte count, int logicalBlockAddress, BufferWithSize buffer, AtaFlags flags) { var task = new AtaTaskFile(feature, count, checked((uint)logicalBlockAddress), 0, AtaCommand.NonVolatileCache, 0); this.ExecuteCommand28(ref task, buffer, flags, true); }

		public void Sleep() { var task = new AtaTaskFile(0, 0, 0, 0, AtaCommand.Sleep, 0); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		public void SmartDisableOperations() { var task = new AtaTaskFile(0xD9, 0, 0xC24F << 8, 0, AtaCommand.Smart, 0); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		public void SmartEnableOperations() { var task = new AtaTaskFile(0xD8, 0, 0xC24F << 8, 0, AtaCommand.Smart, 0); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		public void Standby() { var task = new AtaTaskFile(0, 0, 0, 0, AtaCommand.Standby, 0); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		public void StandbyImmediate() { var task = new AtaTaskFile(0, 0, 0, 0, AtaCommand.StandbyImmediate, 0); this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true); }

		public bool Supports48BitLogicalBlockAddressing { get { if (this._Is48LBA != -1) { this._Is48LBA = this.IdentifyDevice().Command48BitAddressFeatureSetEnabled ? 1 : 0; } return this._Is48LBA != 0; } }

		public SmartData SmartReadData()
		{
			var task = new AtaTaskFile(0xD0, 0, 0xC24F << 8, 0, AtaCommand.Smart, 0);
			SmartData data;
			unsafe { this.ExecuteCommand28(ref task, new BufferWithSize((IntPtr)(&data), sizeof(SmartData)), AtaFlags.ReceiveData, true); }
			return data;
		}

		/// <returns>Whether or not the threshold was exceeded.</returns>
		public bool SmartReturnStatus(byte log)
		{
			var task = new AtaTaskFile(0xDA, 0, (0xC24FU << 8) | log, 0, AtaCommand.Smart, 0);
			this.ExecuteCommand28(ref task, BufferWithSize.Zero, AtaFlags.None, true);
			ushort val = (ushort)((task.LogicalBlockAddress & 0xFFFF00) >> 8);
			if (val == 0x2CF4) { return true; }
			else if (val == 0xC24F) { return false; }
			else { throw new InvalidOperationException(); }
		}

		public virtual uint TimeoutSeconds { get { return 10; } }

		public bool VolatileWriteCacheEnabled { get { return this.IdentifyDevice().VolatileWriteCacheEnabled; } set { this.SetFeatures(value ? (byte)0x02 : (byte)0x82, 0, 0); } }
	}
}