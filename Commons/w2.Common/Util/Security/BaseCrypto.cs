/*
=========================================================================================================
  Module      : 暗号化/復号化モジュール基底クラス(BaseCrypto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace w2.Common.Util.Security
{
	///**************************************************************************************
	/// <summary>
	/// 暗号化/復号化を行う基底クラス
	/// </summary>
	///**************************************************************************************
	public abstract class BaseCrypto
	{
		/// <summary>文字エンコーディング</summary>
		Encoding m_encoding = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="symmetricAlgorithm">アルゴリズムの暗号サービスプロバイダー</param>
		/// <param name="base64RKey">アルゴリズムの共有キー(BASE64)</param>
		/// <param name="base64RIV">アルゴリズムの初期化ベクター(BASE64)</param>
		/// <param name="encoding">エンコード(省略時UTF-8)</param>
		public BaseCrypto(SymmetricAlgorithm symmetricAlgorithm, byte[] base64RKey, byte[] base64RIV, Encoding encoding = null)
		{
			this.CryptoModule = symmetricAlgorithm;
			this.CryptoModule.Key = base64RKey;
			this.CryptoModule.IV = base64RIV;
			m_encoding = (encoding != null) ? encoding : Encoding.UTF8;
		}

		/// <summary>
		/// 文字列暗号化（文字列のバイト配列を暗号化してBASE64エンコードしたものを返却）
		/// </summary>
		/// <param name="source">対象データ</param>
		/// <returns>暗号化データ</returns>
		public string Encrypt(string source)
		{
			return Convert.ToBase64String(Encrypt(m_encoding.GetBytes(source)));
		}
		/// <summary>
		/// 暗号化（バイト配列を暗号化してBASE64エンコードしたものを返却）
		/// </summary>
		/// <param name="source">対象データ</param>
		/// <returns>暗号化データ</returns>
		private byte[] Encrypt(byte[] source)
		{
			byte[] bDestination = null;
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, this.CryptoModule.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cs.Write(source, 0, source.Length);
				}
				bDestination = ms.ToArray();	// CryptoStreamを閉じてから実行する必要がある
			}

			return bDestination;
		}

		/// <summary>
		/// 文字列復号化（文字列をBASE64文字デコードしたものを復号化したものを返却）
		/// </summary>
		/// <param name="source">対象データ</param>
		/// <returns>復号化データ</returns>
		public string Decrypt(string source)
		{
			return m_encoding.GetString(Decrypt(Convert.FromBase64String(source)));
		}
		/// <summary>
		/// 復号化（文字列をBASE64文字デコードしたものを復号化したものを返却）
		/// </summary>
		/// <param name="bSource">対象データ</param>
		/// <returns>復号化データ</returns>
		private byte[] Decrypt(byte[] bSource)
		{
			byte[] bDestination = null;
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, this.CryptoModule.CreateDecryptor(), CryptoStreamMode.Write))
				{
					cs.Write(bSource, 0, bSource.Length);
				}
				bDestination = ms.ToArray();	// CryptoStreamを閉じてから実行する必要がある
			}

			return bDestination;
		}

		/// <summary>アルゴリズムの暗号サービスプロバイダー</summary>
		public SymmetricAlgorithm CryptoModule { get; private set; }
		/// <summary>暗号化KEY</summary>
		public byte[] Key { get { return this.CryptoModule.Key; } }
		/// <summary>初期化ベクトル</summary>
		public byte[] IV { get { return this.CryptoModule.IV; } }

	}
}
