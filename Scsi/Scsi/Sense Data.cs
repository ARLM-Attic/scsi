using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	/// <summary>Represents drive sense data (state and error information). Although this structure is relatively large, it is not a class because of potential benefits in time-critical situations. You should cache this structure in a local variable and pass it by reference in order to prevent excess overheads.</summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SenseData : IMarshalable, ICloneable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte SENSE_KEY_MASK = 0x0F;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte RESPONSE_CODE_MASK = 0x7F;
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		//private const byte ADDITIONAL_SIZE = 14;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte0;
		public bool Valid { get { return Bits.GetBit(this.byte0, 7); } set { this.byte0 = Bits.SetBit(this.byte0, 7, value); } }
		public ResponseCode ResponseCode
		{
			get { return (ResponseCode)Bits.GetValueMask(this.byte0, 0, RESPONSE_CODE_MASK); }
			set { this.byte0 = Bits.PutValueMask(this.byte0, (byte)value, 0, RESPONSE_CODE_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte obsolete1;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		public bool FileMark { get { return Bits.GetBit(this.byte2, 7); } set { this.byte2 = Bits.SetBit(this.byte2, 7, value); } }
		public bool EndOfMedium { get { return Bits.GetBit(this.byte2, 6); } set { this.byte2 = Bits.SetBit(this.byte2, 6, value); } }
		public bool IncorrectLengthIndicator { get { return Bits.GetBit(this.byte2, 5); } set { this.byte2 = Bits.SetBit(this.byte2, 5, value); } }
		public SenseKey SenseKey
		{
			get { return (SenseKey)Bits.GetValueMask(this.byte2, 0, SENSE_KEY_MASK); }
			set { this.byte2 = Bits.PutValueMask(this.byte2, (byte)value, 0, SENSE_KEY_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _Information;
		public uint Information { get { return Bits.BigEndian(this._Information); } set { this._Information = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _AdditionalSenseLength;
		public byte AdditionalSenseLength { get { return this._AdditionalSenseLength; } set { this._AdditionalSenseLength = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint _CommandSpecificInformation;
		public uint CommandSpecificInformation { get { return Bits.BigEndian(this._CommandSpecificInformation); } set { this._CommandSpecificInformation = Bits.BigEndian(value); } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AdditionalSenseCode _AdditionalSenseCode;
		public AdditionalSenseCode AdditionalSenseCode { get { return this._AdditionalSenseCode; } set { this._AdditionalSenseCode = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AdditionalSenseCodeQualifier _AdditionalSenseCodeQualifier;
		public AdditionalSenseCodeQualifier AdditionalSenseCodeQualifier { get { return this._AdditionalSenseCodeQualifier; } set { this._AdditionalSenseCodeQualifier = value; } }
		public AdditionalSenseInformation AdditionalSenseCodeAndQualifier
		{
			get { return unchecked((AdditionalSenseInformation)((ushort)this.AdditionalSenseCode << 8 | (ushort)this.AdditionalSenseCodeQualifier)); }
			set { unchecked { this.AdditionalSenseCode = (AdditionalSenseCode)(byte)((ushort)value >> 8); this.AdditionalSenseCodeQualifier = (AdditionalSenseCodeQualifier)(byte)value; } }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte _FieldReplaceableUnitCode;
		public byte FieldReplaceableUnitCode { get { return this._FieldReplaceableUnitCode; } set { this._FieldReplaceableUnitCode = value; } }
		[DebuggerDisplay("{GetSenseKeySpecificBytes()}")]
		public SenseKeySpecificBytes SenseKeySpecific;
		//public unsafe fixed byte AditionalSenseBytes[16];

		public object GetSenseKeySpecificBytes()
		{
			if (this.SenseKeySpecific.SenseKeySpecificValid)
			{
				switch (this.SenseKey)
				{
					case SenseKey.NotReady:
						goto case SenseKey.NoSense;
					case SenseKey.NoSense:
						return this.SenseKeySpecific.ProgressIndication;
					case SenseKey.RecoveredError:
						goto case SenseKey.MediumError;
					case SenseKey.MediumError:
						goto case SenseKey.HardwareError;
					case SenseKey.HardwareError:
						return this.SenseKeySpecific.ActualRetryCount;
					case SenseKey.IllegalRequest:
						return this.SenseKeySpecific.FieldPointer;
					case SenseKey.CopyAborted:
						return this.SenseKeySpecific.Segment;
					default:
						return null;
				}
			}
			else
			{
				return null;
			}
		}

		public override string ToString()
		{
			const string SEPARATOR = ", ";
			var sb = new System.Text.StringBuilder();
			sb.AppendFormat("{0} {{", this.GetType().Name);
			sb.AppendFormat("{0}", this.ResponseCode);
			sb.AppendFormat(SEPARATOR + "SK = {0}", this.SenseKey);
			if (this.AdditionalSenseCode != default(AdditionalSenseCode)) { sb.AppendFormat(SEPARATOR + "ASC = {0}", Enum.IsDefined(typeof(AdditionalSenseCode), this.AdditionalSenseCode) ? this.AdditionalSenseCode.ToString() : string.Format("0x{0:X}", this.AdditionalSenseCode)); }
			if (this.AdditionalSenseCodeQualifier != default(AdditionalSenseCodeQualifier)) { sb.AppendFormat(SEPARATOR + "ASQC = 0x{0:X}", this.AdditionalSenseCodeQualifier); }
			if (this.EndOfMedium) { sb.Append(SEPARATOR + "End of Medium"); }
			if (this.FileMark) { sb.Append(SEPARATOR + "File Mark"); }
			if (this.IncorrectLengthIndicator) { sb.Append(SEPARATOR + "Incorrect Length Indicator"); }
			sb.Append("}");
			object senseKeySpecific = this.GetSenseKeySpecificBytes();
			if (senseKeySpecific != null) { sb.AppendFormat(SEPARATOR + "{0}", senseKeySpecific); }
			return sb.ToString();
		}

		public SenseData Clone() { var me = (SenseData)this.MemberwiseClone();return me; }

		private void MarshalFrom(BufferWithSize buffer) { Marshaler.DefaultPtrToStructure(buffer, ref this); }
		private void MarshalTo(BufferWithSize buffer) { Marshaler.DefaultStructureToPtr((object)this, buffer); }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int MarshaledSize { get { return Marshal.SizeOf(this); } }

		void IMarshalable.MarshalFrom(BufferWithSize buffer) { this.MarshalFrom(buffer); }
		void IMarshalable.MarshalTo(BufferWithSize buffer) { this.MarshalTo(buffer); }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int IMarshalable.MarshaledSize { get { return this.MarshaledSize; } }

		object ICloneable.Clone() { return this.Clone(); }
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct SenseKeySpecificBytes
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[FieldOffset(0)]
		private byte byte15;
		public bool SenseKeySpecificValid { get { return Bits.GetBit(this.byte15, 7); } set { this.byte15 = Bits.SetBit(this.byte15, 7, value); } }
		[FieldOffset(0)]
		public FieldPointerBytes FieldPointer;
		[FieldOffset(0)]
		public ActualRetryCountBytes ActualRetryCount;
		[FieldOffset(0)]
		public ProgressIndicationBytes ProgressIndication;
		[FieldOffset(0)]
		public SegmentBytes Segment;

		public override string ToString() { return Internal.GenericToString(this, false); }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct FieldPointerBytes
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte BIT_POINTER_MASK = 0x07;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte15;
		public bool CommandData { get { return Bits.GetBit(this.byte15, 6); } set { this.byte15 = Bits.SetBit(this.byte15, 6, value); } }
		public bool BitPointerValid { get { return Bits.GetBit(this.byte15, 3); } set { this.byte15 = Bits.SetBit(this.byte15, 3, value); } }
		public byte BitPointer
		{
			get { return Bits.GetValueMask(this.byte15, 0, BIT_POINTER_MASK); }
			set { this.byte15 = Bits.PutValueMask(this.byte15, (byte)value, 0, BIT_POINTER_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _FieldPointer;
		public ushort FieldPointer { get { return Bits.BigEndian(this._FieldPointer); } set { this._FieldPointer = Bits.BigEndian(value); } }

		public override string ToString()
		{
			var sb = new System.Text.StringBuilder();
			const string SEPARATOR = ", ";
			sb.AppendFormat("{0} ({1}) {{", this.GetType().Name, this.CommandData ? "Error in CDB" : "Error in buffer");
			sb.AppendFormat("Field Pointer = {0}", this.FieldPointer);
			if (this.BitPointerValid) { sb.AppendFormat(SEPARATOR + "Bit Pointer = {0}", this.BitPointer); }
			sb.Append("}");
			return sb.ToString();
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ActualRetryCountBytes
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte15;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _ActualRetryCount;
		public ushort ActualRetryCount { get { return Bits.BigEndian(this._ActualRetryCount); } set { this._ActualRetryCount = Bits.BigEndian(value); } }

		public override string ToString() { return Internal.GenericToString(this, false); }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ProgressIndicationBytes
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte15;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _ProgressIndication;
		public ushort ProgressIndication { get { return Bits.BigEndian(this._ProgressIndication); } set { this._ProgressIndication = Bits.BigEndian(value); } }
		public float ProgressIndicationFraction { get { return (float)this.ProgressIndication / ProgressIndicationDenominator; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public const int ProgressIndicationDenominator = (int)ushort.MaxValue + 1;

		public override string ToString() { return Internal.GenericToString(this, false); }
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SegmentBytes
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte BIT_POINTER_MASK = 0x07;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte15;
		/// <summary>An <see cref="SegmentDescriptor"/> value of <c>false</c> indicates that the field pointer
		/// is relative to the start of the parameter list. An <see cref="SegmentDescriptor"/> value of <c>true</c>
		/// indicates that the field pointer is relative to the start of the segment descriptor indicated by
		/// the third and fourth bytes of the <see cref="SenseData.CommandSpecificInformation"/> field.
		/// </summary>
		public bool SegmentDescriptor { get { return Bits.GetBit(this.byte15, 5); } set { this.byte15 = Bits.SetBit(this.byte15, 5, value); } }
		public bool BitPointerValid { get { return Bits.GetBit(this.byte15, 3); } set { this.byte15 = Bits.SetBit(this.byte15, 3, value); } }
		public byte BitPointer
		{
			get { return Bits.GetValueMask(this.byte15, 0, BIT_POINTER_MASK); }
			set { this.byte15 = Bits.PutValueMask(this.byte15, (byte)value, 0, BIT_POINTER_MASK); }
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _FieldPointer;
		public ushort FieldPointer { get { return Bits.BigEndian(this._FieldPointer); } set { this._FieldPointer = Bits.BigEndian(value); } }

		public override string ToString() { return Internal.GenericToString(this, false); }
	}

	public enum SenseKey : byte
	{
		NoSense = 0x00,
		RecoveredError = 0x01,
		NotReady = 0x02,
		MediumError = 0x03,
		HardwareError = 0x04,
		IllegalRequest = 0x05,
		UnitAttention = 0x06,
		DataProtect = 0x07,
		BlankCheck = 0x08,
		VendorSpecific = 0x09,
		CopyAborted = 0x0A,
		AbortedCommand = 0x0B,
		VolumeOverflow = 0x0D,
		Miscompare = 0x0E,
	}

	public enum AdditionalSenseCode : byte
	{
		None = 0x00,
		LogicalUnitNotReady = 0x04,
		ReadError = 0x11,
		PositioningError = 0x15,
		RecoveredDataWithoutErrorCorrection = 0x17,
		RecoveredDataWithErrorCorrection = 0x18,
		ParameterListLengthError = 0x1A,
		InvalidCommandOperationCode = 0x20,
		AddressError = 0x21,
		InvalidFunction = 0x22,
		InvalidFieldInCommandDescriptorBlock = 0x24,
		ParameterError = 0x26,
		PowerOnOrResetOrBusDeviceResetOccurred = 0x29,
		ParametersChanged = 0x2A,
		CommandSequenceError = 0x2C,
		InsufficientTimeForOperation = 0x2E,
		MediumError = 0x30,
		MediumNotPresent = 0x3A,
		TargetOperationConditionsChanged = 0x3F,
		OperatorRequestOrStateChangeInput = 0x5A,
		LogException = 0x5B,
	}

	/// <summary>This enumeration should be cast to a <see cref="byte"/> for usage; there are so many values that it is difficult to list all the possibilities.
	/// I have not yet figured out a good way to combine <see cref="SenseKey"/>, <see cref="AdditionalSenseCode"/>, and <see cref="AdditionalSenseCodeQualifier"/> values.
	/// I have thought of combining them into a single structure, but by doing that I lose much of the IDE IntelliSense support.
	/// </summary>
	public enum AdditionalSenseCodeQualifier : byte { None = 0x00, }

	public enum ResponseCode : byte { None = 0x00, CurrentError = 0x70, DeferredError = 0x71, VendorSpecific = 0x7F }

	public enum ScsiStatus : byte
	{
		/// <summary>This status indicates that the device server has successfully completed the task.</summary>
		[Description("This status indicates that the device server has successfully completed the task.")]
		Good = 0x00,
		/// <summary>This status indicates that an CA or ACA condition has occurred. Autosense data may be delivered.</summary>
		CheckCondition = 0x02,
		/// <summary>This status is returned whenever the requested operation specified by an unlinked command is satisfied.</summary>
		ConditionMet = 0x04,
		Busy = 0x08,
		Intermediate = 0x10,
		IntermediateConditionMet = 0x14,
		ReservationConflict = 0x18,
		TaskSetFull = 0x28,
		ACAActive = 0x30,
		TaskAborted = 0x40,
	}
}