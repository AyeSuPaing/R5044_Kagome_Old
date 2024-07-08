/*
=========================================================================================================
  Module      : シリアルキーユーティリティクラス (SerialKeyUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using w2.Common.Util.Security;

namespace w2.Domain.SerialKey.Helper
{
	/// <summary>
	/// シリアルキーユーティリティクラス
	/// </summary>
	public class SerialKeyUtility
	{
		/// <summary>
		/// シリアルキー文字列を、Config設定で指定されたパターンに沿ってフォーマットします。
		/// </summary>
		/// <param name="keyString">シリアルキー文字列</param>
		/// <returns>フォーマット済み文字列</returns>
		/// <remarks>置換文字が多すぎる場合、最後の文字置換以降のフォーマットパターンは無視されます。</remarks>
		/// <remarks>置換文字が少なすぎる場合、残ったキー文字列はフォーマットせずに末尾に追加します。</remarks>
		public static string GetFormattedKeyString(string keyString)
		{
			// 書式文字列の組み立て
			var counter = 0;
			var formatString = new StringBuilder();
			foreach (var c in Constants.DIGITAL_CONTENTS_SERIAL_KEY_FORMAT)
			{
				// 置換文字数＞キー文字列長
				if (counter == keyString.Length) break;

				if (c == 'X')
				{
					formatString.Append(keyString[counter]);
					counter++;
				}
				else
				{
					formatString.Append(c);
				}
			}

			// 置換文字数＜キー文字列長
			if (counter < keyString.Length)
			{
				formatString.Append(keyString.Substring(counter));
			}

			return formatString.ToString();
		}

		/// <summary>
		/// シリアルキー復号化
		/// </summary>
		/// <param name="serialKey">シリアルキー</param>
		/// <returns>復号されたシリアルキー</returns>
		public static string DecryptSerialKey(string serialKey)
		{
			if (string.IsNullOrEmpty(serialKey)) return "";

			var crypto = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			return crypto.Decrypt(serialKey);
		}

		/// <summary>
		/// シリアルキー暗号化
		/// </summary>
		/// <param name="serialKey">シリアルキー</param>
		/// <returns>暗号化されたシリアルキー</returns>
		public static string EncryptSerialKey(string serialKey)
		{
			if (string.IsNullOrEmpty(serialKey)) return "";

			var crypto = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			return crypto.Encrypt(serialKey);
		}
	}
}
