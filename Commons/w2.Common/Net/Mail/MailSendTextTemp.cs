/*
=========================================================================================================
  Module      : メール送信テキスト一時保存クラス(MailSendTextTemp.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System.Text;

namespace w2.Common.Net.Mail
{
	/// <summary>
	/// メール送信テキスト一時保存クラス
	/// </summary>
	public class MailSendTextTemp
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mailSendText">メール送信内容</param>
		/// <param name="encordedText">エンコード済テキスト</param>
		/// <param name="encodeing">エンコード情報</param>
		public MailSendTextTemp(string mailSendText, string encordedText = null, Encoding encodeing = null)
		{
			this.MailSendText = mailSendText;
			this.EncordedText = encordedText;
			this.Encodeing = encodeing;
		}

		/// <summary>
		/// Sendするテキストを取得
		/// </summary>
		/// <returns>Sendするテキスト</returns>
		public string GetSendText() => this.EncordedText ?? this.MailSendText;

		/// <summary>メール送信テキスト</summary>
		public string MailSendText { get; }
		/// <summary>エンコード済テキスト</summary>
		public string EncordedText { get; }
		/// <summary>エンコード情報（指定しない場合はnull）</summary>
		public Encoding Encodeing { get; }
	}
}
