/*
=========================================================================================================
  Module      : Csv Utility (CsvUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Util;

namespace w2.Commerce.Batch.WmsShippingBatch.Util
{
	/// <summary>
	/// Csv utility
	/// </summary>
	public static class CsvUtility
	{
		/// <summary>
		/// Convert field
		/// </summary>
		/// <param name="data">Value</param>
		/// <param name="setting">The export field setting</param>
		/// <returns>Converted field</returns>
		public static string ConvertField(Hashtable data, ExportFieldSetting setting)
		{
			if (data == null) return string.Empty;

			// Get value of export name setting
			var value = data[setting.ExportName];

			// Convert with format
			if (string.IsNullOrEmpty(setting.ExportFormat) == false)
			{
				value = ConvertWithFormat(value, setting);
			}

			// Set default value
			if (string.IsNullOrEmpty(StringUtility.ToEmpty(value))
				&& (string.IsNullOrEmpty(setting.ExportDefaultValue) == false))
			{
				value = setting.ExportDefaultValue;
			}

			// Convert with max length
			var result = ConvertWithMaxLength(StringUtility.ToEmpty(value), setting);
			return result;
		}

		/// <summary>
		/// Convert with format
		/// </summary>
		/// <param name="value">A value that need to convert</param>
		/// <param name="setting">The export field setting</param>
		/// <returns>A converted value</returns>
		private static string ConvertWithFormat(object value, ExportFieldSetting setting)
		{
			var result = string.Format("{0:" + setting.ExportFormat + "}", value);
			return result;
		}

		/// <summary>
		/// Convert with max length
		/// </summary>
		/// <param name="value">A value that need to convert</param>
		/// <param name="setting">The export field setting</param>
		/// <returns>A converted value</returns>
		private static string ConvertWithMaxLength(string value, ExportFieldSetting setting)
		{
			// Check value and length
			if ((setting.ExportMaxLength.HasValue == false)
				|| (value.Length <= setting.ExportMaxLength.Value))
			{
				return value;
			}

			// Create new value
			var newValue = value.Substring(0, setting.ExportMaxLength.Value);
			return newValue;
		}
	}
}
