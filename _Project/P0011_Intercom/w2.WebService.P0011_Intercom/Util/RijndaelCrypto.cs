/*
=========================================================================================================
  Module      : ラインダール暗号化用クラス(RijndaelCrypto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : ラインダールでの暗号・複合化処理を司るクラス。
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;


namespace w2.Plugin.P0011_Intercom.WebService.Util
{
			/*
	[ Rijndael：ラインダール ]
	 ベルギーの数学者Joan Daemen氏とVincent Rijmen氏によって開発された秘密鍵暗号化アルゴリズム。
	 2000年10月にアメリカ政府標準技術局(NIST)によって次世代の暗号化標準AES(Advanced Encryption Standard)に選定された。
	 それまでの暗号化方式の標準はIBM社が開発したDESだったが、
	 コンピュータの高性能化や理論の発展により相対的に脆弱になってきたためこれに代わる次世代の暗号化標準として採用された。
	 鍵をブロックとして分割して暗号化する方式を採っており、
	 鍵とブロックの長さはそれぞれ128ビット、192ビット、256ビットの中から指定できる。
	 また、暗号化中のビットの変換をFeistel構造という一般的なもので行なうのではなく、
	 3つのそれぞれ違った変換を用いるなどの特徴もあり、暗号強度と速度の双方に優れた暗号化アルゴリズムとして高く評価されている。
	 
	  [利用例 (暗号化キーを指定する場合)]
		string strTest = "Rijndael 暗号化テスト";
		byte[] RKey = Convert.FromBase64String("rCY3Tw9H/bFnDfldd1U/mTUrFIyJGjb+hMRRke7jnE0=");
		byte[] RIV = Convert.FromBase64String("+SSGl11F20otrmofpwf+1g==");
		w2.Common.Util.Security.RijndaelCrypto rc = new w2.Common.Util.Security.RijndaelCrypto(RKey, RIV);

	    // 暗号化
	    strint strEncrypted = rc.Encrypt(strTest);
	    // byte[] bEncrypted = rc.Encrypt(Encoding.UTF8.GetBytes(strTest));
	 
	    // 複合化
		string strDecrypted = rc.Decrypt(strEncrypted);
		// string strDecrypted = Encoding.UTF8.GetString(rc.Decrypt(bEncrypted));
	*/

		///**************************************************************************************
		/// <summary>
		/// ラインダール暗号化/複合化を行う
		/// </summary>
		///**************************************************************************************
		public class RijndaelCrypto
		{
			RijndaelManaged m_rmRijndaelManaged = new RijndaelManaged();

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public RijndaelCrypto()
			{
				m_rmRijndaelManaged.GenerateKey();
				m_rmRijndaelManaged.GenerateIV();
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="bRKey">Key</param>
			/// <param name="bRIV">IV</param>
			public RijndaelCrypto(byte[] bRKey, byte[] bRIV)
			{
				m_rmRijndaelManaged.Key = bRKey;
				m_rmRijndaelManaged.IV = bRIV;
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="strBase64RKey">Key</param>
			/// <param name="strBase64RIV">IV</param>
			public RijndaelCrypto(string strBase64RKey, string strBase64RIV)
			{
				m_rmRijndaelManaged.Key = Convert.FromBase64String(strBase64RKey);
				m_rmRijndaelManaged.IV = Convert.FromBase64String(strBase64RIV);
			}

			/// <summary>
			/// 文字列暗号化（UTF8のバイト配列を暗号化してBASE64エンコードしたものを返却）
			/// </summary>
			/// <param name="strSource">対象データ</param>
			/// <returns>暗号化データ</returns>
			public string Encrypt(string strSource)
			{
				return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(strSource)));
			}
			/// <summary>
			/// 暗号化
			/// </summary>
			/// <param name="bSource">対象データ</param>
			/// <returns>暗号化データ</returns>
			public byte[] Encrypt(byte[] bSource)
			{
				byte[] bDestination = null;
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, m_rmRijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(bSource, 0, bSource.Length);
					}
					bDestination = ms.ToArray();	// CryptoStreamを閉じてから実行する必要がある
				}

				return bDestination;
			}

			/// <summary>
			/// 文字列複合化（BASE64文字デコードしたものを複合化UTF8文字列化したものを返却）
			/// </summary>
			/// <param name="strSource">対象データ</param>
			/// <returns>複合化データ</returns>
			public string Decrypt(string strSource)
			{
				return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(strSource)));
			}
			/// <summary>
			/// 複合化
			/// </summary>
			/// <param name="bSource">対象データ</param>
			/// <returns>複合化データ</returns>
			public byte[] Decrypt(byte[] bSource)
			{
				byte[] bDestination = null;
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, m_rmRijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(bSource, 0, bSource.Length);
					}
					bDestination = ms.ToArray();	// CryptoStreamを閉じてから実行する必要がある
				}

				return bDestination;
			}

			/// <summary>RijndaelのKEY</summary>
			public byte[] RijndaelKey
			{
				get { return m_rmRijndaelManaged.Key; }
				set { m_rmRijndaelManaged.Key = value; }
			}
			/// <summary>RijndaelのKEY(BASE64文字列)</summary>
			public string RijndaelKeyString
			{
				get { return Convert.ToBase64String(m_rmRijndaelManaged.Key); }
				set { m_rmRijndaelManaged.Key = Convert.FromBase64String(value); }
			}
			/// <summary>RijndaelのIV</summary>
			public byte[] RijndaelIV
			{
				get { return m_rmRijndaelManaged.IV; }
				set { m_rmRijndaelManaged.IV = value; }
			}
			/// <summary>RijndaelのIV(BASE64文字列)</summary>
			public string RijndaelIVString
			{
				get { return Convert.ToBase64String(m_rmRijndaelManaged.IV); }
				set { m_rmRijndaelManaged.IV = Convert.FromBase64String(value); }
			}
		}


}