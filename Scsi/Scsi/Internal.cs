using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Scsi
{
	public static class Internal
	{
		public static string GenericToString<T>(T value, bool dynamic) { return GenericToString<T>(value, dynamic, ", "); }

		public static string GenericToString<T>(T value, bool dynamic, string separator) { return GenericToString<T>(dynamic ? value.GetType() : typeof(T), ref value, separator); }

		private static string GenericToString<T>(Type type, [In] ref T value, string separator)
		{
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			List<string> strs = new List<string>();
			foreach (var property in properties)
			{
				if (property.CanRead)
				{
					try { strs.Add(string.Format("{0} = {1}", property.Name, property.GetValue(value, null))); }
					catch (InvalidOperationException) { }
				}
			}
			return string.Format("{0} {{{1}}}", type.Name, string.Join(separator, strs.ToArray()));
		}


		public static void StringToBufferAnsi(string str, Helper.BufferWithSize buffer) { for (int i = 0; i < str.Length; i++) { buffer[i] = (byte)str[i]; } }
	}
}