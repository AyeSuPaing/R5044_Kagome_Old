/*
=========================================================================================================
  Module      : 暗号・複合化処理の提供口となるクラス(w2Crypt.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Crypto
{
	public class w2Crypt : ICrypt
	{
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public w2Crypt() 
		{			
		}

		/// <summary>
		/// 文字列暗号化
		/// </summary>
		/// <param name="targetvalue">暗号化対象文字列</param>
		/// <param name="password">暗号化に利用するパスワード</param>
		/// <returns>暗号化文字列。失敗した場合はString.Empty</returns>
		public string Encrypt(string targetvalue,string password)
		{
			RijndaelCryptographer cry = new RijndaelCryptographer();
			string returnval = "";

			try
			{
				returnval = cry.EncryptString(targetvalue, password);
			}
			catch
			{
				//エラー起きても何もしない
			}
			finally
			{
				cry = null;
			}

			return returnval;
		}

		/// <summary>
		/// 文字列複合化
		/// </summary>
		/// <param name="targetvalue">複合化対象文字列</param>
		/// <param name="password">複合化に利用するパスワード</param>
		/// <returns>複合化文字列。失敗した場合はString.Empty</returns>
		public string Decrypt(string targetvalue,string password)
		{
			RijndaelCryptographer cry = new RijndaelCryptographer();
			string returnval = "";

			try
			{
				returnval = cry.DecryptString(targetvalue, password);
			}
			catch
			{
				//エラー起きても何もしない
			}
			finally
			{
				cry = null;
			}			
			
			return returnval;
		}
	}
}
