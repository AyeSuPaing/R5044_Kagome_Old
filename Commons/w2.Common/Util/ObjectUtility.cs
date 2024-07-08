/*
=========================================================================================================
  Module      : Object Utility (ObjectUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace w2.Common.Util
{
	/// <summary>
	/// ObjectUtility class
	/// </summary>
	public class ObjectUtility
	{
		/// <summary>
		/// Deeps the copy.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <returns>The new object copy</returns>
		public static T DeepCopy<T>(T source)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, source);
				stream.Position = 0;

				return (T)formatter.Deserialize(stream);
			}
		}

		/// <summary>
		/// Try parse string to boolean
		/// </summary>
		/// <param name="value">The string value</param>
		/// <returns>A boolean value</returns>
		public static bool TryParseBool(string value)
		{
			bool result;
			if (bool.TryParse(value, out result) == false) return false;

			return result;
		}

		/// <summary>
		/// Try parse string to int
		/// </summary>
		/// <param name="value">The string value</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns>An int value</returns>
		public static int TryParseInt(string value, int defaultValue)
		{
			int result;
			if (int.TryParse(value, out result) == false) return defaultValue;

			return result;
		}

		/// <summary>
		/// Try parse string to nullable int
		/// </summary>
		/// <param name="value">The string value</param>
		/// <returns>An nullable int value</returns>
		public static int? TryParseInt(string value)
		{
			int result;
			if (int.TryParse(value, out result) == false) return null;

			return result;
		}

		/// <summary>
		/// Try parse string to decimal
		/// </summary>
		/// <param name="value">The string value</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns>A decimal value</returns>
		public static decimal TryParseDecimal(string value, decimal defaultValue)
		{
			decimal result;
			if (decimal.TryParse(value, out result) == false) return defaultValue;

			return result;
		}

		/// <summary>
		/// Try parse string to nullable decimal
		/// </summary>
		/// <param name="value">The string value</param>
		/// <returns>A nullable decimal value</returns>
		public static decimal? TryParseDecimal(string value)
		{
			decimal result;
			if (decimal.TryParse(value, out result) == false) return null;

			return result;
		}

		/// <summary>
		/// Try parse string to date time
		/// </summary>
		/// <param name="value">The string value</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns>A date time value</returns>
		public static DateTime TryParseDateTime(string value, DateTime defaultValue)
		{
			DateTime result;
			if (DateTime.TryParse(value, out result) == false) return defaultValue;

			return result;
		}

		/// <summary>
		/// Try parse string to nullable date time
		/// </summary>
		/// <param name="value">The string value</param>
		/// <returns>A nullable date time value</returns>
		public static DateTime? TryParseDateTime(string value)
		{
			DateTime result;
			if (DateTime.TryParse(value, out result) == false) return null;

			return result;
		}

		/// <summary>
		/// Try parse exac string to date time
		/// </summary>
		/// <param name="value">The string value</param>
		/// <param name="format">date format</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns>A date time value</returns>
		public static DateTime TryParseExacDateTime(string value, string format, DateTime defaultValue)
		{
			DateTime result;
			if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out result) == false) return defaultValue;

			return result;
		}
	}
}