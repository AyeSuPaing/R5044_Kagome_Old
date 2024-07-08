/*
=========================================================================================================
  Module      : メールアドレス解析 (MailAddress.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.Commerce.MallBatch.MailOrderGetter.MailAnalyze
{
	///**************************************************************************************
    /// <summary>
    /// メールアドレス解析クラス
    /// </summary>
	///**************************************************************************************
    public class MailAddress
    {
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailAddress()
		{
			// プロパティ初期化
			this.MailAddresses = new List<string>();
		}
        /// <summary>
        /// コンストラクタ
        /// </summary>
		/// <param name="strAddress">メールアドレス文字列</param>
		/// <remarks>アドレス構造を解析・構築する</remarks>
		public MailAddress(string strAddress) : this()
		{
			InitMailAddress(strAddress);
		}

        /// <summary>
        /// メールアドレス文字列を受け取り、アドレス構造を解析・構築する
        /// </summary>
		/// <param name="strAddress">メールアドレス文字列</param>
		/// <remarks>※警告 このメソッドはRFCに準拠しない仮実装</remarks>
        private void InitMailAddress(string strAddress)
        {
            // セミコロン、カンマで区切る
			string[] strMailAddressesFull = strAddress.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // <から>を抽出
			List<string> lMailAddresses = new List<string>();
            foreach (string strMailAddress in strMailAddressesFull)
            {
				int iStart = strMailAddress.IndexOf('<');
				int iEnd = strMailAddress.IndexOf('>');
				if ((iStart < iEnd) && (0 <= iStart) && (iStart <= strMailAddress.Length))
                {
					lMailAddresses.Add(strMailAddress.Substring(iStart + 1, iEnd - iStart - 1).Trim().Replace("mailto:", ""));
                }
                else
                {
					lMailAddresses.Add(strMailAddress.Trim().Replace("mailto:", ""));
                }
            }
            
            // 配列に格納
			this.MailAddresses = lMailAddresses;
        }

		/// <summary>メールアドレス</summary>
		public List<string> MailAddresses { get; private set; }
	}
}
