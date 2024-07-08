/*
=========================================================================================================
  Module      : 3DES 暗号化/復号化モジュール(TripleDESCrypto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace w2.Common.Util.Security
{
	/*
	[ 3DES：Triple DES ]
	 　IBM社が開発した暗号方式。
	   同社が開発した秘密鍵型の暗号方式である「DES」を三重に適用するようにした方式のこと。
	   コンピュータの性能向上に伴ってDES暗号を解読される危険性が高まったため、
	   同じ方式を三重にかけることにより、強度を高めた。
	   特定の文章を鍵Aで暗号化し、その結果を鍵Bで復号し、さらにその結果を鍵C(鍵Aで行なう場合もある)で暗号化している。
	   第2段階を復号とすることによって、3つの鍵をすべて同一にした場合にDESと同じ結果が得られるようになっている。
	 
	  [利用例 (暗号化キーを指定する場合)]
		string strTest = "Triple DES 暗号化テスト";
		byte[] TDESKey = Convert.FromBase64String("b4Rrc5S6Erq0vMBcQng3NBJc/Kz/Qtgo");
		byte[] TDESIV = Convert.FromBase64String("O4ipn2Oxwc8=");
		w2.Common.Util.Security.TripleDESCrypto tdes = new w2.Common.Util.Security.TripleDESCrypto(TDESKey, TDESIV);

	    // 暗号化
	    string strEncrypted = tdes.Encrypt(strTest);
	    // byte[] bEncrypted = tdes.Encrypt(Encoding.UTF8.GetBytes(strTest));

		// 復号化
		string strDecrypted = tdes.Decrypt(strEncrypted);
		// string strDecrypted = Encoding.UTF8.GetString(tdes.Decrypt(bEncrypted));
	*/

	///**************************************************************************************
	/// <summary>
	/// 3DES暗号化/復号化を行う
	/// </summary>
	///**************************************************************************************
	public class TripleDESCrypto : BaseCrypto
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="base64TDESKey">アルゴリズムの共有キー(BASE64)</param>
		/// <param name="base64TDESIV">アルゴリズムの初期化ベクター(BASE64)</param>
		/// <param name="encoding">エンコード(省略時UTF-8)</param>
		public TripleDESCrypto(string base64TDESKey, string base64TDESIV, Encoding encoding = null)
			: this(Convert.FromBase64String(base64TDESKey), Convert.FromBase64String(base64TDESIV), encoding)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="TDESKey">アルゴリズムの共有キー</param>
		/// <param name="TDESIV">アルゴリズムの初期化ベクター</param>
		/// <param name="encoding">エンコード(省略時UTF-8)</param>
		public TripleDESCrypto(byte[] TDESKey, byte[] TDESIV, Encoding encoding = null)
			: base(new TripleDESCryptoServiceProvider(), TDESKey, TDESIV, encoding)
		{
		}
	}
}
