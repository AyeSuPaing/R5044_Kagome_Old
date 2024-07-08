/*
=========================================================================================================
  Module      : Yahoo API 公開鍵クラス(YahooApiPubkey.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System.Security.Cryptography;
using System.Text;
using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;

namespace w2.App.Common.Mall.Yahoo
{
	/// <summary>
	/// Yahoo API 公開鍵クラス
	/// </summary>
	public class YahooApiPubkey
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sellerId">セラーID</param>
		/// <param name="pubkey">公開鍵</param>
		public YahooApiPubkey(string sellerId, string pubkey)
		{
			this.SellerId= sellerId;
			this.Pubkey = pubkey;
		}
		
		/// <summary>
		/// 暗号化
		/// </summary>
		/// <returns>暗号化した値</returns>
		public string Encrypt()
		{
			if (string.IsNullOrEmpty(this.Pubkey)) return string.Empty;

			// 認証情報作成(ストアアカウントとunixtimestampを:で結合する)
			var authenticationValue =
				Encoding.UTF8.GetBytes($"{this.SellerId}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");
			var rsaPubkey = ImportPublicKey(this.Pubkey);
			var bytesCypherText = rsaPubkey.Encrypt(authenticationValue, RSAEncryptionPadding.Pkcs1);
			var result = Convert.ToBase64String(bytesCypherText);
			return result;
		}

		/// <summary>
		/// RSA暗号化モジュールに公開鍵を取り込む
		/// </summary>
		/// <param name="pem">公開鍵</param>
		/// <returns>RSA暗号化モジュール</returns>
		private static RSACryptoServiceProvider ImportPublicKey(string pem)
		{
			var pemReader = new PemReader(new StringReader(pem));
			var publicKey = (AsymmetricKeyParameter)(pemReader.ReadObject());
			var rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

			var csp = new RSACryptoServiceProvider();
			csp.ImportParameters(rsaParams);
			return csp;
		}

		/// <summary>セラーID</summary>
		public string SellerId { get; }
		/// <summary>公開鍵</summary>
		public string Pubkey { get; }
	}
}
