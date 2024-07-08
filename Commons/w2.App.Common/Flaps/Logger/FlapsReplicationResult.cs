/*
=========================================================================================================
  Module      : FLAPS同期処理結果クラス (FlapsReplicationResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.Common.Logger;

namespace w2.App.Common.Flaps.Logger
{
	/// <summary>
	/// FLAPS同期処理結果
	/// </summary>
	public class FlapsReplicationResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="replicationType">同期対象のデータの種類</param>
		public FlapsReplicationResult(string replicationType)
		{
			switch (replicationType)
			{
				case Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT:
					this.ReplicationDataType = "Product Replication";
					break;

				case Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCTSTOCK:
					this.ReplicationDataType = "Product Stock Replication";
					break;

				default:
					this.ReplicationDataType = replicationType;
					break;
			}
		}

		/// <summary>
		/// 成功件数計上
		/// </summary>
		public void CountupSuccess()
		{
			this.Total++;
			this.Success++;
		}

		/// <summary>
		/// 失敗件数計上
		/// </summary>
		public void CountupFailure()
		{
			this.Total++;
			this.Failure++;
		}

		/// <summary>
		/// 管理者へメール送信
		/// </summary>
		public void NotifyOnEmail()
		{
			using (var mail = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_FLAPS_INTEGRATION,
				string.Empty,
				new Hashtable(this.MailTemplateTags)))
			{
				if (mail.SendMail() == false)
				{
					FileLogger.WriteError(mail.MailSendException);
				}
			}
		}

		/// <summary>
		/// ログを出力
		/// </summary>
		public void WriteResult()
		{
			var lines = new []
			{
				CreateLogLine("同期データ", this.MailTemplateTags["replication_data_type"]),
				CreateLogLine("処理総件数", this.MailTemplateTags["total"]),
				CreateLogLine("成功件数", this.MailTemplateTags["success"]),
				CreateLogLine("失敗件数", this.MailTemplateTags["failure"]),
				CreateLogLine("成功率", this.MailTemplateTags["success_rate"]),
			};
			var msg = string.Join(Environment.NewLine, lines);
			FileLogger.WriteInfo(msg);
			Console.WriteLine(msg);
		}

		/// <summary>
		/// 結果一行分の文字列を作成
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">値</param>
		/// <returns>結果1行分文字列</returns>
		private string CreateLogLine(string key, string value)
		{
			return string.Format("{0}: {1}", key, value);
		}

		/// <summary>同期処理の種類</summary>
		private string ReplicationDataType { get; set; }
		/// <summary>処理総件数</summary>
		public int Total { get; private set; }
		/// <summary>成功処理件数</summary>
		public int Success { get; private set; }
		/// <summary>失敗処理件数</summary>
		public int Failure { get; private set; }
		/// <summary>成功率</summary>
		private string SuccessRate
		{
			get { return ((Convert.ToDecimal(this.Success) / Convert.ToDecimal(this.Total)) * 100) + "%"; }
		}
		/// <summary>メールテンプレートタグ</summary>
		private Dictionary<string, string> MailTemplateTags
		{
			get
			{
				return new Dictionary<string, string>
				{
					{ "replication_data_type", this.ReplicationDataType },
					{ "total", this.Total.ToString() },
					{ "success", this.Success.ToString() },
					{ "failure", this.Failure.ToString() },
					{ "success_rate", this.SuccessRate },
				};
			}
		}
	}
}
