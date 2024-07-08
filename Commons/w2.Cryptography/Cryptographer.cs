/*
=========================================================================================================
  Module      : 暗号化/複合化モジュール(Cryptographer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace w2.Cryptography
{
	/// <summary>
	/// 暗号化・複合化モジュール
	/// </summary>
	public class Cryptographer
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="key">共有キー</param>
		/// <param name="vector">初期化ベクター</param>
		public Cryptographer(string key, string iv)
		{
			// ラインダールを設定
			RijndaelManaged rijndael = new RijndaelManaged();
			rijndael.Key = Convert.FromBase64String(key);
			rijndael.IV = Convert.FromBase64String(iv);

			// プロパティ初期化
			this.Encryptor = rijndael.CreateEncryptor();
			this.Decryptor = rijndael.CreateDecryptor();
		}

		/// <summary>
		/// 暗号化処理
		/// </summary>
		/// <param name="source">文字列</param>
		/// <returns>暗号化された文字列</returns>
		public string Encrypt(string source)
		{
			Encoding utf8 = Encoding.UTF8;
			byte[] encryptedSource = Transform(utf8.GetBytes(source), this.Encryptor);
			return Convert.ToBase64String(encryptedSource);
		}

		/// <summary>
		/// 複合化処理
		/// </summary>
		/// <param name="source">文字列</param>
		/// <returns>複合化された文字列</returns>
		public string Decrypt(string source)
		{
			Encoding utf8 = Encoding.UTF8;
			byte[] encryptedSource = Transform(Convert.FromBase64String(source), this.Decryptor);
			return utf8.GetString(encryptedSource);
		}

		/// <summary>
		/// 暗号化・複合化処理
		/// </summary>
		/// <param name="source">対象データ</param>
		/// <param name="transform">暗号変換情報</param>
		/// <returns>変換後情報</returns>
		private byte[] Transform(byte[] source, ICryptoTransform transform)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
				{
					cs.Write(source, 0, source.Length);
				}
				return ms.ToArray(); // CryptStreamを閉じてからでないとbyte配列が途中で途切れる可能性がある
			}
		}

		/// <summary>暗号化オブジェクト</summary>
		private ICryptoTransform Encryptor { get; set; }
		/// <summary>復号化オブジェクト</summary>
		private ICryptoTransform Decryptor { get; set; }
	}
}
