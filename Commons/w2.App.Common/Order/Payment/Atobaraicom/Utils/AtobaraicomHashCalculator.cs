/*
=========================================================================================================
  Module      : 後払いハッシュ計算機 (AtobaraicomHashCalculator.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Security.Cryptography;
using System.Text;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払いハッシュ計算機
	/// </summary>
	public class AtobaraicomHashCalculator
	{
		/// <summary>ハッシュキー</summary>
		private string m_hashKey = null;
		/// <summary>ハッシュエンコーディング</summary>
		private Encoding m_encoding = null;
		/// <summary>チェックサム用バッファ</summary>
		private StringBuilder m_checkSumBuffer = new StringBuilder();

		/// <summary>
		/// 後払いハッシュ計算機
		/// </summary>
		/// <param name="hashKey">ハッシュキー</param>
		/// <param name="encoding">エンコーディング</param>
		internal AtobaraicomHashCalculator(string hashKey, Encoding encoding)
		{
			m_hashKey = hashKey;
			m_encoding = encoding;
		}

		/// <summary>
		/// 追加
		/// </summary>
		/// <param name="source">ソース</param>
		/// <returns>値</returns>
		internal string Add(string source)
		{
			var value = StringUtility.ToEmpty(source).Trim();
			m_checkSumBuffer.Append(value);

			return value;
		}

		/// <summary>
		/// SHA1ハッシュ計算
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>ハッシュ値</returns>
		private string ComputeHashSHA1(string value)
		{
			var calculator = new SHA1Managed();
			var hash = calculator.ComputeHash(m_encoding.GetBytes(value));

			var hashString = new StringBuilder();
			foreach (var item in hash)
			{
				hashString.Append(item.ToString("x2"));
			}
			return hashString.ToString();
		}
	}
}
