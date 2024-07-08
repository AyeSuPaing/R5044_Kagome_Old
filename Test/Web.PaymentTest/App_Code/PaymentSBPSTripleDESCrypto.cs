/*
=========================================================================================================
  Module      : ソフトバンクペイメント 3DESクラス(PaymentSBPS3DESTripleDESCrypto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Util.Security;

	/// <summary>
	/// SBPS 3DESクラス
	/// </summary>
public class PaymentSBPSTripleDESCrypto
{
	/// <summary>3DESオブジェクト</summary>
	private TripleDESCrypto m_tripleDES = null;
	/// <summary>エンコード</summary>
	private Encoding m_encoding = Encoding.GetEncoding("Shift_JIS");

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="tripleDESKey">3DES暗号化キー</param>
	/// <param name="tripleDESIV">3DES初期化ベクトル</param>
	public PaymentSBPSTripleDESCrypto(
		string tripleDESKey,
		string tripleDESIV)
	{
		m_tripleDES = new TripleDESCrypto(
			m_encoding.GetBytes(tripleDESKey),
			m_encoding.GetBytes(tripleDESIV),
			m_encoding);

		m_tripleDES.CryptoModule.Padding = System.Security.Cryptography.PaddingMode.None;
	}

	/// <summary>
	/// 暗号化データ取得
	/// </summary>
	/// <param name="source">対象</param>
	/// <returns>暗号化データ</returns>
	public string GetEncryptedData(string source)
	{
		int byteCount = m_encoding.GetByteCount(source);
		int totalLengh = source.Length + ((byteCount % 8 != 0) ? (8 - (byteCount % 8)) : 0);
		return m_tripleDES.Encrypt(source.PadRight(totalLengh, ' '));
	}

	/// <summary>
	/// 複合化データ取得
	/// </summary>
	/// <param name="source">対象</param>
	/// <returns>複合化データ</returns>
	public string GetDecryptedData(string source)
	{
		return m_tripleDES.Decrypt(source).TrimEnd();
	}
}

