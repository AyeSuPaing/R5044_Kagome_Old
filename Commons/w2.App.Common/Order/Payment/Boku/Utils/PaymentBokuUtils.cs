/*
=========================================================================================================
  Module      : Payment Boku Utils(PaymentBokuUtils.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace w2.App.Common.Order.Payment.Boku.Utils
{
	/// <summary>
	/// Payment Boku Utils
	/// </summary>
	public class PaymentBokuUtils
	{
		/// <summary>
		/// Create hash string
		/// </summary>
		/// <param name="source">Source</param>
		/// <returns>Hash as string</returns>
		public static string CreateHashString(string source)
		{
			var hash = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(source));

			var hashString = string.Join("", hash.Select(b => b.ToString("x2")));
			return hashString;
		}

		/// <summary>
		/// Create signature
		/// </summary>
		/// <param name="authenticationKey">Authentication key</param>
		/// <param name="source">Source</param>
		/// <returns>Signature</returns>
		public static string CreateSignature(string authenticationKey, string source)
		{
			var encoding = Encoding.UTF8;
			using (var hmacsha256 = new HMACSHA256(encoding.GetBytes(authenticationKey)))
			{
				var hash = hmacsha256.ComputeHash(encoding.GetBytes(source));
				var hashString = string.Join("", hash.Select(b => b.ToString("x2")));
				return hashString;
			}
		}

		/// <summary>
		/// Convert to unix timestamp
		/// </summary>
		/// <param name="date">Date</param>
		/// <returns>Unix timestamp</returns>
		public static long ConvertToUnixTimestamp(DateTime date)
		{
			var unixTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			return unixTimestamp;
		}
	}
}
