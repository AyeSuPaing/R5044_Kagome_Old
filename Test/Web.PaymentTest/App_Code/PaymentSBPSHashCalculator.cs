/*
=========================================================================================================
  Module      : ソフトバンクペイメント ハッシュ計算クラス(PaymentSBPSHashCalculator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using w2.Common.Util;

/// <summary>
/// SBPSハッシュ計算クラス
/// </summary>
public class PaymentSBPSHashCalculator
{
	/// <summary>ハッシュキー</summary>
	private string m_hashKey = null;
	/// <summary>ハッシュエンコーディング</summary>
	private Encoding m_encoding = null;
	/// <summary>チェックサム用バッファ</summary>
	private StringBuilder m_checkSumBuffer = new StringBuilder();

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="hashKey">ハッシュキー</param>
	/// <param name="encoding">ハッシュエンコーディング</param>
	public PaymentSBPSHashCalculator(string hashKey, Encoding encoding)
	{
		m_hashKey = hashKey;
		m_encoding = encoding;
	}

	/// <summary>
	/// チェックサムバッファへ追加しつつ同じものを戻す
	/// </summary>
	/// <param name="source">値</param>
	/// <returns>値（nullは空文字に置換して返す）</returns>
	public string Add(string source)
	{
		string value = StringUtility.ToEmpty(source).Trim();
		m_checkSumBuffer.Append(value);

		return value;
	}

	/// <summary>
	/// バッファをSHA1ハッシュ計算して返す（バッファクリアもする）
	/// </summary>
	/// <returns>ハッシュ値</returns>
	public string ComputeHashSHA1AndClearBuffer()
	{
		string hashString = ComputeHashSHA1(m_checkSumBuffer.ToString() + m_hashKey);

		m_checkSumBuffer.Clear();

		return hashString;
	}
	/// <summary>
	/// SHA1ハッシュ計算
	/// </summary>
	/// <returns>ハッシュ値</returns>
	private string ComputeHashSHA1(string value)
	{
		SHA1Managed calculator = new SHA1Managed();
		byte[] hash = calculator.ComputeHash(m_encoding.GetBytes(value));

		StringBuilder hashString = new StringBuilder();
		foreach (byte b in hash)
		{
			hashString.Append(b.ToString("x2"));
		}
		return hashString.ToString();
	}
}