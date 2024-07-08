/*
=========================================================================================================
  Module      : Rijndael(AES)暗号化/復号化モジュール(RijndaelCrypto.cs)
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
	 
	    // 復号化
		string strDecrypted = rc.Decrypt(strEncrypted);
		// string strDecrypted = Encoding.UTF8.GetString(rc.Decrypt(bEncrypted));
	*/

	///**************************************************************************************
	/// <summary>
	/// ラインダール暗号化/復号化を行う
	/// </summary>
	///**************************************************************************************
	public class RijndaelCrypto : BaseCrypto
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="base64RKey">アルゴリズムの共有キー(BASE64)</param>
		/// <param name="base64RIV">アルゴリズムの初期化ベクター(BASE64)</param>
		/// <param name="encoding">エンコード(省略時UTF-8)</param>
		public RijndaelCrypto(string base64RKey, string base64RIV, Encoding encoding = null)
			: this(Convert.FromBase64String(base64RKey), Convert.FromBase64String(base64RIV), encoding)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="base64RKey">アルゴリズムの共有キー(BASE64)</param>
		/// <param name="base64RIV">アルゴリズムの初期化ベクター(BASE64)</param>
		/// <param name="encoding">エンコード(省略時UTF-8)</param>
		public RijndaelCrypto(byte[] rKey, byte[] rIV, Encoding encoding = null)
			: base(new RijndaelManaged(), rKey, rIV, encoding)
		{
		}
	}
}
