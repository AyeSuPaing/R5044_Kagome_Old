/*
=========================================================================================================
  Module      : メール送信ユーティリティ (MailSendUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Logger;
using w2.App.Common;

namespace w2.Commerce.Batch.ExternalOrderImport
{
	/// <summary>
	/// メール送信ユーティリティ
	/// </summary>
	public class MailSendUtil
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mailTemplateId">メールテンプレートID</param>
		public MailSendUtil(string mailTemplateId)
		{
			this.MailTemplateId = mailTemplateId;
		}

		/// <summary>
		/// オペレーター向けメール送信
		/// </summary>
		public void SendMailForOperator()
		{
			var input = new Hashtable
			{
				{ Constants.EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_RESULT, GetResultString() },
				{ Constants.EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_TIME_BEGIN, this.BeginTime.ToString("yyyy/MM/dd HH:mm:ss") },
				{ Constants.EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_TIME_END, this.EndTime.ToString("yyyy/MM/dd HH:mm:ss") },
				{ Constants.EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_SUCCESS_COUNT, this.SuccessCount },
				{ Constants.EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_FAILURE_COUNT, this.FailureCount },
				{ Constants.EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_EXECUTE_COUNT, this.ExecuteCount },
				{ Constants.EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_SKIP_COUNT, this.SkipCount },
				{ Constants.EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_APP_NAME, this.AppName },
				{ Constants.EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_MESSAGE, this.Message }
			};

			using (var mailUtil = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, this.MailTemplateId, string.Empty, input, true, App.Common.Constants.MailSendMethod.Auto))
			{
				if (mailUtil.SendMail() == false)
				{
					FileLogger.WriteError(mailUtil.MailSendException);
				}
			}
		}

		/// <summary>
		/// 実行結果取得
		/// </summary>
		/// <returns>実行結果</returns>
		private string GetResultString()
		{
			if ((this.ExecuteCount - this.SkipCount) == 0) return "成功（警告）";
			var result = (this.FailureCount > 0)
				? "失敗"
				: "成功";
			return result;
		}

		#region プロパティ
		/// <summary>処理名</summary>
		public string AppName { get; set; }
		/// <summary>メールテンプレートID</summary>
		public string MailTemplateId { get; set; }
		/// <summary>メッセージ</summary>
		public StringBuilder Message { get; set; }
		/// <summary>処理結果</summary>
		public string Result { get; set; }
		/// <summary>処理開始時間</summary>
		public DateTime BeginTime { get; set; }
		/// <summary>処理終了時間</summary>
		public DateTime EndTime { get; set; }
		/// <summary>処理総件数</summary>
		public int ExecuteCount { get; set; }
		/// <summary>成功件数</summary>
		public int SuccessCount { get; set; }
		/// <summary>処理スキップ件数</summary>
		public int SkipCount { get; set; }
		/// <summary>エラー件数</summary>
		public int FailureCount { get; set; }
		#endregion
	}
}
