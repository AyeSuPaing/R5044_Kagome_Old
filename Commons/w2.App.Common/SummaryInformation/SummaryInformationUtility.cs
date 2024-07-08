/*
=========================================================================================================
  Module      : Summary Information Utility (SummaryInformationUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.SummaryInformation
{
	/// <summary>
	/// Summary information utility
	/// </summary>
	public static class SummaryInformationUtility
	{
		/// <summary>
		/// Calculate percent
		/// </summary>
		/// <param name="current">The current value</param>
		/// <param name="total">The total value</param>
		/// <returns>A calculated percent</returns>
		public static decimal CalculatePercent(decimal current, decimal total)
		{
			var result = (total > 0)
				? Math.Round((current * 100) / total)
				: 0m;
			return result;
		}

		/// <summary>
		/// Get percent from number
		/// </summary>
		/// <param name="current">The current value</param>
		/// <param name="total">The total value</param>
		/// <returns>Percentage</returns>
		public static string GetPercentFromNumber(decimal current, decimal total)
		{
			var percentage = CalculatePercent(current, total);
			return string.Format("{0}%", percentage.ToString("0"));
		}
	}
}
