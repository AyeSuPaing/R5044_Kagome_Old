/*
=========================================================================================================
  Module      : 更新履歴前後検索のためのヘルパクラス (UpdateHistoryHashCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace w2.Domain.UpdateHistory.Helper
{
	/// <summary>
	/// SHA256ハッシュ作成
	/// </summary>
	internal class UpdateHistoryHashCreator
	{
		/// <summary>
		/// SHA256ハッシュ文字列作成
		/// </summary>
		/// <param name="source">対象</param>
		/// <returns>ハッシュ文字列</returns>
		internal static string CreateSha256HashString(byte[] source)
		{
			var hash = CreateSha256Hash(source);
			var result = BitConverter.ToString(hash).ToLower().Replace("-", "");
			return result;
		}

		/// <summary>
		/// SHA256ハッシュ文字列作成
		/// </summary>
		/// <param name="source">対象</param>
		/// <returns>ハッシュ値</returns>
		private static byte[] CreateSha256Hash(byte[] source)
		{
			using (var sha256 = new System.Security.Cryptography.SHA256CryptoServiceProvider())
			{
				var hash = sha256.ComputeHash(source);
				return hash;
			}
		}
	}
}