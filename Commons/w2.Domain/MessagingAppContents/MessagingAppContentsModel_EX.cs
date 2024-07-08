/*
=========================================================================================================
  Module      : メッセージアプリ向けコンテンツモデルクラス(MessagingAppContentsModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.MessagingAppContents
{
	/// <summary>
	/// メッセージアプリ向けコンテンツモデルクラス
	/// </summary>
	public partial class MessagingAppContentsModel
	{
		/// <summary> マスタ区分（MailTemplate） </summary>
		public const string MASTER_KBN_MAILTEMPLATE = "MailTemplate";
		/// <summary> マスタ区分（MailDistText） </summary>
		public const string MASTER_KBN_MAILDISTTEXT = "MailDistText";
		/// <summary> メッセージアプリ区分（LINE） </summary>
		public const string MESSAGING_APP_KBN_LINE = "LINE";
		/// <summary> メディアタイプ（TEXT） </summary>
		public const string MEDIA_TYPE_TEXT = "TEXT";

		/// <summary>
		/// メディアタイプが 'TEXT' か？
		/// </summary>
		public bool IsText
		{
			get { return (this.MediaType == MEDIA_TYPE_TEXT); }
		}
	}
}
