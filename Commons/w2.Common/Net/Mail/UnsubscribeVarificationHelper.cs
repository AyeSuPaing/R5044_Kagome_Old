/*
=========================================================================================================
  Module      : 登録解除検証ヘルパークラス(UnsubscribeVarificationHelper.ss)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Security.Cryptography;
using System.Text;

namespace w2.Common.Net.Mail
{
	/// <summary>
	/// 登録解除検証ヘルパークラス
	/// </summary>
	public class UnsubscribeVarificationHelper
	{
		/// <summary>
		/// ハッシュ化する
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <returns>ハッシュ化済みテキスト</returns>
		public static string Hash(string userId, string mailAddress)
		{
			using (var sha256 = SHA256.Create())
			{
				var bytes = Encoding.UTF8.GetBytes(userId + mailAddress);
				var hashBytes = sha256.ComputeHash(bytes);
				return Convert.ToBase64String(hashBytes);
			}
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="verificationKey">検証キー</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="maillAddress">メールアドレス</param>
		/// <returns>検証結果</returns>
		public static bool Verification(string verificationKey, string userId, string maillAddress)
		{
			var hash = Hash(userId, maillAddress);
			var result = verificationKey == hash;
			return result;
		}
	}
}
