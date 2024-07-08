/*
=========================================================================================================
  Module      : メール文章解析（マルチパート対応） (MailMessage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.MallBatch.MailOrderGetter.MailAnalyze
{
	///**************************************************************************************
    /// <summary>
    /// メール文章解析クラス
    /// </summary>
	///**************************************************************************************
    public class MailMessage
    {
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailMessage()
		{
			// プロパティ初期化
			this.Header = new MailHeader();
			this.MultipartMessage = null;
		}
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="strMailMessage">メール文章</param>
		/// <remarks>メールを構成する</remarks>
		public MailMessage(string strMailMessageText) : this()
		{
			InitMailMessage(strMailMessageText);
		}

        /// <summary>
        /// メール受信内容から可読なメールを構成します
        /// </summary>
        /// <param name="strMailMessage">メール文章</param>
		private void InitMailMessage(string strMailMessageText)
        {
			this.MailMessageText = strMailMessageText;

            // ヘッダと本文を分離
			int iStart = strMailMessageText.IndexOf("\r\n\r\n"); // 改行二個が最初に連続している位置

            // ヘッダ
			this.Header = new MailHeader(strMailMessageText.Substring(0, iStart));

            // 本文
			if (this.Header.MultiPart)
            {
                // マルチパート
				this.MultipartMessage = new MultipartMessage(strMailMessageText.Substring(iStart + 2), this.Header.MultiPartBoundary);
				this.Body = this.MultipartMessage.Body;
            }
            else
            {
                // 非マルチパート
				this.Body = strMailMessageText.Substring(iStart + 2);
            }
        }

        /// <summary>メール全体のテキスト</summary>
		public string MailMessageText { get; private set; }

		/// <summary>メールのヘッダを表します</summary>
		public MailHeader Header { set; get; }

		/// <summary>マルチパートメッセージの場合、添付ファイルなど</summary>
		public MultipartMessage MultipartMessage { set; private get; }

		/// <summary>メールの本文</summary>
		public string Body { set; get; }
    }
}
