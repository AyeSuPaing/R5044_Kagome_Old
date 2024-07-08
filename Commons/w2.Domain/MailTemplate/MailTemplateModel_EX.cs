/*
=========================================================================================================
  Module      : メールテンプレートマスタモデル (MailTemplateModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.MailTemplate
{
	/// <summary>
	/// メールテンプレートマスタモデル
	/// </summary>
	public partial class MailTemplateModel
	{
		/// <summary>SMS利用フラグ：利用しない</summary>
		public const string SMS_USE_FLG_OFF = "0";
		/// <summary>SMS利用フラグ：利用する</summary>
		public const string SMS_USE_FLG_ON = "1";

		/// <summary>LINE利用フラグ：利用しない</summary>
		public const string LINE_USE_FLG_OFF = "0";
		/// <summary>LINE利用フラグ：利用する</summary>
		public const string LINE_USE_FLG_ON = "1";

		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>自動送信フラグ判定</summary>
		public bool AutoSendFlgCheck
		{
			get { return this.AutoSendFlg == Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_SEND; }
		}
		#endregion
	}
}
