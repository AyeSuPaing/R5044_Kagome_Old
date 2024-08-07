﻿/*
=========================================================================================================
  Module      : ラインダール暗号化用クラス(RijndaelCryptographer.cs)
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


namespace w2.Crypto
{
	///**************************************************************************************
	/// <summary>
	/// ラインダール暗号化/複合化を行う
	/// </summary>
	///**************************************************************************************
	internal class RijndaelCryptographer
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal RijndaelCryptographer()
		{
			
		}


		/// <summary>
		/// 文字列を暗号化する
		/// </summary>
		/// <param name="sourceString">暗号化する文字列</param>
		/// <param name="password">暗号化に使用するパスワード</param>
		/// <returns>暗号化された文字列</returns>
		internal string EncryptString(string sourceString, string password)
		{
			//RijndaelManagedオブジェクトを作成
			System.Security.Cryptography.RijndaelManaged rijndael =
				new System.Security.Cryptography.RijndaelManaged();

			//パスワードから共有キーと初期化ベクタを作成
			byte[] key, iv;
			GenerateKeyFromPassword(
				password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
			rijndael.Key = key;
			rijndael.IV = iv;

			//文字列をバイト型配列に変換する
			byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(sourceString);

			//対称暗号化オブジェクトの作成
			System.Security.Cryptography.ICryptoTransform encryptor =
				rijndael.CreateEncryptor();
			//バイト型配列を暗号化する
			//復号化に失敗すると例外CryptographicExceptionが発生
			byte[] encBytes = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
			//閉じる
			encryptor.Dispose();

			//バイト型配列を文字列に変換して返す
			return System.Convert.ToBase64String(encBytes);
		}

		/// <summary>
		/// 暗号化された文字列を復号化する
		/// </summary>
		/// <param name="sourceString">暗号化された文字列</param>
		/// <param name="password">暗号化に使用したパスワード</param>
		/// <returns>復号化された文字列</returns>
		internal string DecryptString(string sourceString, string password)
		{
			//RijndaelManagedオブジェクトを作成
			System.Security.Cryptography.RijndaelManaged rijndael =
				new System.Security.Cryptography.RijndaelManaged();

			//パスワードから共有キーと初期化ベクタを作成
			byte[] key, iv;
			GenerateKeyFromPassword(
				password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
			rijndael.Key = key;
			rijndael.IV = iv;

			//文字列をバイト型配列に戻す
			byte[] strBytes = System.Convert.FromBase64String(sourceString);

			//対称暗号化オブジェクトの作成
			System.Security.Cryptography.ICryptoTransform decryptor =
				rijndael.CreateDecryptor();
			//バイト型配列を復号化する
			byte[] decBytes = decryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
			//閉じる
			decryptor.Dispose();

			//バイト型配列を文字列に戻して返す
			return System.Text.Encoding.UTF8.GetString(decBytes);
		}

		/// <summary>
		/// パスワードから共有キーと初期化ベクタを生成する
		/// </summary>
		/// <param name="password">基になるパスワード</param>
		/// <param name="keySize">共有キーのサイズ（ビット）</param>
		/// <param name="key">作成された共有キー</param>
		/// <param name="blockSize">初期化ベクタのサイズ（ビット）</param>
		/// <param name="iv">作成された初期化ベクタ</param>
		private void GenerateKeyFromPassword(string password,
			int keySize, out byte[] key, int blockSize, out byte[] iv)
		{

			//ソルトからキーとベクター生成

			//Rfc2898DeriveBytesオブジェクトを生成、利用
			byte[] salt = System.Text.Encoding.UTF8.GetBytes("9f3325dd691a41658d610d5725a2f9c6");
			System.Security.Cryptography.Rfc2898DeriveBytes deriveBytes =
				new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);

			//反復処理回数を100回
			deriveBytes.IterationCount = 100;

			//共有キーと初期化ベクタを生成する
			//キーとベクターは公開しないで隠す！！！
			key = deriveBytes.GetBytes(keySize / 8);
			iv = deriveBytes.GetBytes(blockSize / 8);
		}

	}


}