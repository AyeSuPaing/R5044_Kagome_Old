/*
=========================================================================================================
  Module      : コマンド基底クラス(CommandBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Common.Logger;

namespace w2.Commerce.Batch.UpdateShipmentForReturnOrder.Commands
{
	/// <summary>
	/// コマンド基底クラス
	/// </summary>
	public abstract class CommandBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary
		protected CommandBase()
		{
			this.ErrorMessages = new List<string>();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 転送処理実行
		/// </summary>
		public void Execute()
		{
			this.BeginDate = DateTime.Now;

			// 継承クラスの処理
			this.Exec();

			// 終了メール送信
			this.EndDate = DateTime.Now;
			this.EndProcess();
		}

		/// <summary>
		/// 継承クラスの処理
		/// </summary>
		protected abstract void Exec();

		/// <summary>
		/// 終了処理
		/// </summary>
		protected void EndProcess()
		{
			// 完了ログ書き込み
			WriteInfoLog();

			// エラーがあったらエラーメール送信
			if (this.ErrorMessages.Count > 0)
			{
				SendErrorMail();
			}
		}

		/// <summary>
		/// 完了プロパティセット
		/// </summary>
		/// <param name="successCount">成功数</param>
		/// <param name="errorCount">失敗数</param>
		protected void SetFinishProperties(int successCount, int errorCount)
		{
			this.Messages = CreateMessage(successCount, errorCount);
			this.SuccessCount = successCount;
			this.ErrorCount = errorCount;
		}

		/// <summary>
		/// エラーメール送信
		/// </summary>
		private void SendErrorMail()
		{
			var messages = string.Format("「{0}」処理でエラーが発生しました。\r\n", this.Action)
				+ "\r\n"
				+ this.Messages;
			new MailSender().SendMail("エラー", messages);
		}

		/// <summary>
		/// 完了ログ書き込み（メール送信用テンポラリログにも追記）
		/// </summary>
		private void WriteInfoLog()
		{
			var message = string.Format("{0}　時間：{1}（成功：{2}件、失敗：{3}件）",
				(this.Action + " 完了").PadRight(25, '　'),
				(this.EndDate - this.BeginDate),
				this.SuccessCount.ToString().PadLeft(3, ' '),
				this.ErrorCount.ToString().PadLeft(3, ' '));
			FileLogger.WriteInfo(message);
		}

		/// <summary>
		/// メッセージ作成
		/// </summary>
		/// <param name="successCount">成功数</param>
		/// <param name="errorCount">失敗数</param>
		/// <returns>メッセージ</returns>
		public string CreateMessage(int successCount, int errorCount)
		{
			var messages = "■データ（処理件数）\r\n"
				+ string.Format("成功：{0}件\r\n", successCount)
				+ string.Format("失敗：{0}件\r\n", errorCount);
			if (this.ErrorMessages.Count != 0)
			{
				messages += "以下のエラーが発生しました\r\n" 
					+ string.Join("\r\n", this.ErrorMessages);
			}
			return messages;
		}

		/// <summary>
		/// エラーメッセージ作成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>エラーメッセージ</returns>
		public string CreateErrorMessage(string orderId, string errorMessage)
		{
			return string.Format("注文ID : {0}, エラー内容 : {1}", orderId, errorMessage);
		}

		#endregion

		#region プロパティ
		/// <summary>処理開始日時</summary>
		private DateTime? BeginDate { get; set; }
		/// <summary>処理終了日時</summary>
		private DateTime? EndDate { get; set; }
		/// <summary>処理</summary>
		public string Action { get; set; }
		/// <summary>メッセージ</summary>
		public string Messages { get; set; }
		/// <summary>成功数</summary>
		public int SuccessCount { get; set; }
		/// <summary>失敗数</summary>
		public int ErrorCount { get; set; }
		/// <summary>エラーメッセージ</summary>
		public List<string> ErrorMessages { get; set; }
		#endregion
	}
}