/*
=========================================================================================================
  Module      : 警告メール送信クラス(WarningMailSender.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.IO;
using w2.App.Common;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.Message;
using w2.CustomerSupport.Batch.CsWarningMailSender.MailSendModules;

namespace w2.CustomerSupport.Batch.CsWarningMailSender
{
	/// <summary>
	/// 警告メール送信クラス
	/// </summary>
	class WarningMailSender
	{
		#region 定数・メンバ変数
		/// <summary>送信済みファイル</summary>
		private const string MAIIL_SENT_FILENAME = "MailSent";
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public WarningMailSender()
		{
		}
		#endregion

		#region +Send 送信
		/// <summary>
		/// 送信
		/// </summary>
		public void Send()
		{
			string sentFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MAIIL_SENT_FILENAME);

			if (CheckMailSentFile(sentFilePath) == false) return;

			if (Constants.WARNING_NO_ASSIGN)
			{
				// 管理者向けメール送信
				var adminWarningMailSender = new AdminWarningMailSender();
				adminWarningMailSender.SendToTarget();
			}

			if (Constants.WARNING_NO_ACTION)
			{
				var operatorWarningMailSender = new OperatorWarningMailSender();

				// オペレータ(本人)向けメール送信
				operatorWarningMailSender.SendToTargetForOperator();

				// オペレータ(グループ)向けメール送信
				operatorWarningMailSender.SendToTargetForGroup();
			}

			// メール送信ファイル更新
			UpdateMailSentFile(sentFilePath);
		}
		#endregion

		#region -CheckMailSent メール送信ファイルチェック
		/// <summary>
		/// メール送信ファイルチェック
		/// </summary>
		/// <param name="filePath">送信済みファイルパス</param>
		/// <returns>送信OKか</returns>
		private bool CheckMailSentFile(string filePath)
		{
			// 初回は実行OK
			if (File.Exists(filePath) == false) return true;

			// 送信時間（毎日X時）をこえていたら実行OK
			return (File.GetLastWriteTime(filePath).Date < DateTime.Now.Date)
			       || ((File.GetLastWriteTime(filePath).Date == DateTime.Now.Date) && (DateTime.Now.Hour >= Constants.MAIL_SEND_TIME));
		}
		#endregion

		#region -UpdateMailSentFile メール送信済みファイルを更新します。
		/// <summary>
		/// メール送信済みファイルを更新します。
		/// </summary>
		/// <param name="filePath">送信済みファイルパス</param>
		private void UpdateMailSentFile(string filePath)
		{
			if (File.Exists(filePath) == false)
			{
				File.CreateText(filePath);
			}
			else
			{
				File.SetLastWriteTime(filePath, DateTime.Now);
			}
		}
		#endregion
	}
}
