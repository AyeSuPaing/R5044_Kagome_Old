/*
=========================================================================================================
  Module      : マルチパート解析 (MultipartMessage.cs)
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
    /// マルチパート解析クラス
    /// </summary>
	///**************************************************************************************
    public class MultipartMessage
    {
		/// <summary>
        /// コンストラクタ
        /// </summary>
		/// <param name="strMessageBody">メッセージのデータ本体</param>
		/// <param name="strBoundary">分割文字列</param>
		/// <remarks>マルチパートメッセージを構成する</remarks>
		public MultipartMessage(string strMessageBody, string strBoundary)
		{
			InitMultipartMessage(strMessageBody, strBoundary);
		}

		/// <summary>
        /// メッセージ本体からマルチパートメッセージを構成する
        /// </summary>
		/// <param name="strMessageBody">メッセージのデータ本体</param>
		/// <param name="strBoundary">分割文字列</param>
		private void InitMultipartMessage(string strMessageBody, string strBoundary)
        {
			string[] strSplitters = new string[] { strBoundary };
			string[] strMessages = strMessageBody.Split(strSplitters, StringSplitOptions.None); // マルチパートに分解されたメッセージ群

            // メッセージを構成
			List<MailMessage> lMailMessages = new List<MailMessage>();
			for (int iLoop = 1; iLoop < strMessages.Length; iLoop++)
            {
                try
                {
					lMailMessages.Add(new MailMessage(strMessages[iLoop - 1]));
                }
                catch (Exception)
                {
                    // メッセージを構成する要素に不足がある場合など
                    lMailMessages.Remove(null);
                }
            }

            // 最初のテキストメッセージをbodyとする
			List<MailMessage> lMailMessagesClones = new List<MailMessage>(lMailMessages);
            foreach (MailMessage mailMessage in lMailMessagesClones)
            {
                // content-typeが無い場合 -> そのコンテンツは無効として削除
				if (mailMessage.Header.HeaderParts["content-type"] == null)
                {
					lMailMessages.Remove(mailMessage);
                    continue;
                }

                // Content-Typeからtextコンテンツを探す
				List<string> lContentTypes = (List<string>)(mailMessage.Header.HeaderParts["content-type"]);
				foreach (string strHeaderElement in lContentTypes)
                {
                    int iIsText = strHeaderElement.ToLower().IndexOf("text/");
					if ((0 < iIsText) && (iIsText < strHeaderElement.Length))
                    {
                        // ヒット
						this.Body = mailMessage.Body;
                        break;
                    }
                }
            }
        }

		/// <summary>本文</summary>
		public string Body { get; private set; }
	}
}
