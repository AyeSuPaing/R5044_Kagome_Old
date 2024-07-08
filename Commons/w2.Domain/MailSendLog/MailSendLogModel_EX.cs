/*
=========================================================================================================
  Module      : メール送信ログモデル (MailSendLogModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.MailSendLog
{
	/// <summary>
	/// メール送信ログモデル
	/// </summary>
	public partial class MailSendLogModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>メールテンプレートでのメール？</summary>
		public bool IsMailTemplate
		{
			get { return (string.IsNullOrEmpty(this.MailId) == false); }
		}
		/// <summary>エラーメッセージあり？</summary>
		public bool HasError
		{
			get { return (string.IsNullOrEmpty(this.ErrorMessage) == false); }
		}
		#endregion
	}
}
