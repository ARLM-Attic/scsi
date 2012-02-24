using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Helper;

namespace Scsi
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ControlModePage : ModePage
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE2_OFFSET = Marshal.OffsetOf(typeof(ControlModePage), "byte2");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE3_OFFSET = Marshal.OffsetOf(typeof(ControlModePage), "byte3");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE4_OFFSET = Marshal.OffsetOf(typeof(ControlModePage), "byte4");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BYTE5_OFFSET = Marshal.OffsetOf(typeof(ControlModePage), "byte5");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr READY_AER_HOLDOFF_PERIOD_OFFSET = Marshal.OffsetOf(typeof(ControlModePage), "_ReadyAerHoldoffPeriod");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr BUSY_TIMEOUT_PERIOD_OFFSET = Marshal.OffsetOf(typeof(ControlModePage), "_BusyTimeoutPeriod");
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly IntPtr EXTENDED_SELF_TEST_COMPLETION_TIME_OFFSET = Marshal.OffsetOf(typeof(ControlModePage), "_ExtendedSelfTestCompletionTime");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte TASK_SET_TYPE_MASK = 0xE0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte QUEUE_ERROR_MASK = 0x06;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte QUEUE_ALGORITHM_MODIIFER_MASK = 0xF0;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte UNIT_ATTENTION_INTERLOCKS_CONTROL_MASK = 0x30;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const byte AUTO_LOAD_MODE_MASK = 0x07;

		public ControlModePage() : base(ModePageCode.Control) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte2;
		public bool ReportLogExceptionCondition { get { return Bits.GetBit(this.byte2, 0); } set { this.byte2 = Bits.SetBit(this.byte2, 0, value); } }
		public bool GlobalLoggingTargetSaveDisable { get { return Bits.GetBit(this.byte2, 1); } set { this.byte2 = Bits.SetBit(this.byte2, 1, value); } }
		public bool DescriptorFormatSenseData { get { return Bits.GetBit(this.byte2, 2); } set { this.byte2 = Bits.SetBit(this.byte2, 2, value); } }
		public bool TaskManagementFunctionsOnly { get { return Bits.GetBit(this.byte2, 4); } set { this.byte2 = Bits.SetBit(this.byte2, 4, value); } }
		public TaskSetType TaskSetType { get { return (TaskSetType)Bits.GetValueMask(this.byte2, 5, TASK_SET_TYPE_MASK); } set { this.byte2 = Bits.PutValueMask(this.byte2, (byte)value, 5, TASK_SET_TYPE_MASK); } }
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte3;
		[Obsolete]
		public bool DisableQueueing { get { return Bits.GetBit(this.byte3, 0); } set { this.byte3 = Bits.SetBit(this.byte3, 0, value); } }
		public QueueErrorManagement QueueErrorManagement { get { return (QueueErrorManagement)Bits.GetValueMask(this.byte3, 1, QUEUE_ERROR_MASK); } set { this.byte3 = Bits.PutValueMask(this.byte3, (byte)value, 1, QUEUE_ERROR_MASK); } }
		public QueueAlgorithmModifier QueueAlgorithmModifier { get { return (QueueAlgorithmModifier)Bits.GetValueMask(this.byte3, 4, QUEUE_ALGORITHM_MODIIFER_MASK); } set { this.byte3 = Bits.PutValueMask(this.byte3, (byte)value, 4, QUEUE_ALGORITHM_MODIIFER_MASK); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte4;
		[Obsolete]
		public bool ErrorAsynchronousEventReportingPermission { get { return Bits.GetBit(this.byte4, 0); } set { this.byte4 = Bits.SetBit(this.byte4, 0, value); } }
		[Obsolete]
		public bool UnitAttentionAsynchronousEventReportingPermission { get { return Bits.GetBit(this.byte4, 1); } set { this.byte4 = Bits.SetBit(this.byte4, 1, value); } }
		[Obsolete]
		public bool ReadyAsynchronousEventReportingPermission { get { return Bits.GetBit(this.byte4, 2); } set { this.byte4 = Bits.SetBit(this.byte4, 2, value); } }
		public bool SoftwareWriteProtect { get { return Bits.GetBit(this.byte4, 3); } set { this.byte4 = Bits.SetBit(this.byte4, 3, value); } }
		public UnitAttentionInterlocksControl UnitAttentionInterlocksControl { get { return (UnitAttentionInterlocksControl)Bits.GetValueMask(this.byte4, 4, UNIT_ATTENTION_INTERLOCKS_CONTROL_MASK); } set { this.byte4 = Bits.PutValueMask(this.byte4, (byte)value, 4, UNIT_ATTENTION_INTERLOCKS_CONTROL_MASK); } }
		public bool ReportACheck { get { return Bits.GetBit(this.byte4, 6); } set { this.byte4 = Bits.SetBit(this.byte4, 6, value); } }
		[Obsolete]
		public bool TaskAbortedOld { get { return Bits.GetBit(this.byte4, 7); } set { this.byte4 = Bits.SetBit(this.byte4, 7, value); } }
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte byte5;
		public AutoLoadMode AutoLoadMode { get { return (AutoLoadMode)Bits.GetValueMask(this.byte5, 4, AUTO_LOAD_MODE_MASK); } set { this.byte5 = Bits.PutValueMask(this.byte5, (byte)value, 4, AUTO_LOAD_MODE_MASK); } }
		public bool TaskAbortedStatus { get { return Bits.GetBit(this.byte5, 6); } set { this.byte5 = Bits.SetBit(this.byte5, 6, value); } }
		public bool ApplicationTagOwner { get { return Bits.GetBit(this.byte5, 7); } set { this.byte5 = Bits.SetBit(this.byte5, 7, value); } }
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _ReadyAerHoldoffPeriod;
		public ushort ReadyAerHoldoffPeriod { get { return Bits.BigEndian(this._ReadyAerHoldoffPeriod); } set { this._ReadyAerHoldoffPeriod = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _BusyTimeoutPeriod;
		public ushort BusyTimeoutPeriod { get { return Bits.BigEndian(this._BusyTimeoutPeriod); } set { this._BusyTimeoutPeriod = Bits.BigEndian(value); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ushort _ExtendedSelfTestCompletionTime;
		public ushort ExtendedSelfTestCompletionTime { get { return Bits.BigEndian(this._ExtendedSelfTestCompletionTime); } set { this._ExtendedSelfTestCompletionTime = Bits.BigEndian(value); } }
		
		protected override void MarshalFrom(BufferWithSize buffer)
		{
			base.MarshalFrom(buffer);
			this.byte2 = buffer.Read<byte>(BYTE2_OFFSET);
			this.byte3 = buffer.Read<byte>(BYTE3_OFFSET);
			this.byte4 = buffer.Read<byte>(BYTE4_OFFSET);
			this.byte5 = buffer.Read<byte>(BYTE5_OFFSET);
			this._ReadyAerHoldoffPeriod = buffer.Read<byte>(READY_AER_HOLDOFF_PERIOD_OFFSET);
			this._BusyTimeoutPeriod = buffer.Read<byte>(BUSY_TIMEOUT_PERIOD_OFFSET);
			this._ExtendedSelfTestCompletionTime = buffer.Read<byte>(EXTENDED_SELF_TEST_COMPLETION_TIME_OFFSET);
		}

		protected override void MarshalTo(BufferWithSize buffer)
		{
			base.MarshalTo(buffer);
			buffer.Write(this.byte2, BYTE2_OFFSET);
			buffer.Write(this.byte3, BYTE3_OFFSET);
			buffer.Write(this.byte4, BYTE4_OFFSET);
			buffer.Write(this.byte5, BYTE5_OFFSET);
			buffer.Write(this._ReadyAerHoldoffPeriod, READY_AER_HOLDOFF_PERIOD_OFFSET);
			buffer.Write(this._BusyTimeoutPeriod, BUSY_TIMEOUT_PERIOD_OFFSET);
			buffer.Write(this._ExtendedSelfTestCompletionTime, EXTENDED_SELF_TEST_COMPLETION_TIME_OFFSET);
		}

		protected override int MarshaledSize { get { return Marshaler.DefaultSizeOf<ControlModePage>(); } }
	}


	public enum TaskSetType : byte
	{
		SharedNexusTasks = 0,
		SeparateNexusTasks = 1,
	}

	public enum QueueErrorManagement : byte
	{
		ResumeTasks = 0,
		AbortAllTasks = 1,
		AbortAffectedTasks = 3,
	}

	public enum QueueAlgorithmModifier : byte
	{
		RestrictedOrdering = 1,
		UnrestrictedOrderingAllowed = 2,
	}

	public enum UnitAttentionInterlocksControl : byte
	{
		ClearNoUnitAttention = 0,
		NoClearNoUnitAttention = 2,
		NoClearUnitAttention = 3,
	}

	public enum AutoLoadMode : byte
	{
		LoadMediumForFullAccess = 0,
		LoadMediumForAuxiliaryAccess = 1,
		NoMediumLoad = 2,
	}
}