/*
=========================================================================================================
  Module      : 定期会員フラグ無効（ミルキー停止）バッチ処理(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Sql;
using w2.Domain.FixedPurchase;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace MilkyStop
{
	/// <summary>
	/// ミルキー停止処理（※定期会員フラグOFFにする）
	/// </summary>
	public class Program
	{
		/// <summary>対象日付</summary>
		private const string TARGET_DATE = "target_date";

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		/// <param name="args">コマンドライン引数</param>
		static void Main(string[] args)
		{
			try
			{
				// 初期化
				Initialize();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				// 定期会員フラグOFFにする
				SetFixedPurchaseMemberOff();

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				SendMail(
					"【失敗】 " + Constants.MAIL_TITLE,
					"定期会員フラグ無効バッチの実行が失敗されました。\r\n" + ex.Message);
			}
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private static void Initialize()
		{
			try
			{
				// SQL接続文字列設定
				Constants.STRING_SQL_CONNECTION = Properties.Settings.Default.SqlConnection;
				// SqlStatementXml格納ディレクトリ物理パス
				Constants.PHYSICALDIRPATH_SQL_STATEMENT = AppDomain.CurrentDomain.BaseDirectory + Constants.DIRPATH_XML_STATEMENTS;

				// アプリケーション設定読み込み
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;
				Constants.PHYSICALDIRPATH_LOGFILE = Properties.Settings.Default.Directory_LogFilePath;
				SetServerSmtp();

				// アプリケーション固有の設定
				Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE = Properties.Settings.Default.ConfigFileDirPath;
				Constants.MAIL_TITLE = Properties.Settings.Default.MailTitle;
				Constants.MAIL_FROM = Properties.Settings.Default.MailFrom;
				Constants.MAIL_TO = Properties.Settings.Default.MailTo;
				Constants.MAIL_CC = Properties.Settings.Default.MailCc;
				Constants.MAIL_BCC = Properties.Settings.Default.MailBcc;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("設定値の読み込みに失敗しました。\r\n" + ex);
			}
		}

		/// <summary>
		/// SMTPサーバ設定
		/// </summary>
		private static void SetServerSmtp()
		{
			// SMTPサーバ設定
			// 配列順内容：Server,Port,AuthType,PopServer,PopPort,PopType,UserName,Password
			var smtpSettings = Properties.Settings.Default.Server_Smtp_Settings.Split(',');
			Constants.SERVER_SMTP = smtpSettings[0];
			Constants.SERVER_SMTP_PORT = int.Parse(smtpSettings[1]);
			foreach (Enum e in Enum.GetValues(typeof(w2.Common.SmtpAuthType)))
			{
				if (e.ToString().ToUpper() != smtpSettings[2].ToUpper()) continue;
				Constants.SERVER_SMTP_AUTH_TYPE = (w2.Common.SmtpAuthType)e;
				break;
			}
			switch (Constants.SERVER_SMTP_AUTH_TYPE)
			{
				case w2.Common.SmtpAuthType.PopBeforeSmtp:
				{
					Constants.SERVER_SMTP_AUTH_POP_SERVER = smtpSettings[3];
					Constants.SERVER_SMTP_AUTH_POP_PORT = smtpSettings[4];
					foreach (Enum e in Enum.GetValues(typeof(w2.Common.PopType)))
					{
						if (e.ToString().ToUpper() != smtpSettings[5].ToUpper()) continue;
						Constants.SERVER_SMTP_AUTH_POP_TYPE = (w2.Common.PopType)e;
						break;
					}
					Constants.SERVER_SMTP_AUTH_USER_NAME = smtpSettings[6];
					Constants.SERVER_SMTP_AUTH_PASSOWRD = smtpSettings[7];
					break;
				}
				case w2.Common.SmtpAuthType.SmtpAuth:
					Constants.SERVER_SMTP_AUTH_USER_NAME = smtpSettings[6];
					Constants.SERVER_SMTP_AUTH_PASSOWRD = smtpSettings[7];
					break;
			}
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="title">メールタイトル</param>
		/// <param name="body">メール本文</param>
		private static void SendMail(string title, string body)
		{
			using (var sender = new SmtpMailSender(Constants.SERVER_SMTP))
			{
				var config = Properties.Settings.Default;
				sender.SetFrom(config.MailFrom);
				foreach (var to in config.MailTo) sender.AddTo(to);
				foreach (var cc in config.MailCc) sender.AddTo(cc);
				foreach (var bcc in config.MailBcc) sender.AddTo(bcc);
				sender.SetSubject(title);
				sender.SetBody(body);

				if (sender.SendMail()) return;

				// メール送信失敗
				throw new Exception("メールの送信に失敗しました。");
			}
		}

		/// <summary>
		/// 結果メールの本文作成
		/// </summary>
		/// <param name="changedUserIds">変更されたユーザーIDリスト</param>
		/// <param name="cancelledFixedPurchaseIds">キャンセルされた定期台帳IDリスト</param>
		/// <returns>結果メールの本文</returns>
		private static string CreateResultMessage(List<string> changedUserIds, List<string> cancelledFixedPurchaseIds)
		{
			var body = new StringBuilder();
			// 定期会員フラグ更新された場合
			if (changedUserIds.Count > 0)
			{
				body.Append("■下記の会員に対して、定期会員フラグを無効にしました。\r\n");
				body.AppendFormat("変更件数：{0}\r\n", changedUserIds.Count);
				body.Append(string.Join("\r\n", changedUserIds));
			}
			// キャリア決済での定期台帳がキャンセルされた場合
			if (cancelledFixedPurchaseIds.Count > 0)
			{
				if (body.Length > 0) body.Append("\r\n");
				body.Append("■下記の定期台帳に対して、キャンセルしました。\r\n");
				body.AppendFormat("キャンセル件数：{0}\r\n", cancelledFixedPurchaseIds.Count);
				body.Append(string.Join("\r\n", cancelledFixedPurchaseIds));
			}
			// 変更対象がない場合
			if (body.Length == 0)
			{
				body.Append("定期会員フラグを無効にする対象がありません。");
			}

			return body.ToString();
		}

		/// <summary>
		/// 定期会員フラグOFFにする
		/// </summary>
		private static void SetFixedPurchaseMemberOff()
		{
			// 定期会員フラグOFFを行う
			var changedUserIds = new List<string>();
			var service = new UserService();
			GetTargetUserIds().ForEach(
				id =>
				{
					var success = service.UpdateFixedPurchaseMemberFlg(
						id,
						Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF,
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.Insert);
					if (success) changedUserIds.Add(id);
				});

			// キャリア決済での定期台帳をキャンセル
			var cancelledFixedPurchaseIds = new List<string>();
			var fpService = new FixedPurchaseService();
			GetFixedPurchaseCancelTarget().ForEach(
				fp =>
				{
					var success = fpService.CancelFixedPurchase(
						fpService.GetContainer(fp.FixedPurchaseId),
						Constants.FLG_LASTCHANGED_BATCH,
						fp.ShopId,
						true,
						UpdateHistoryAction.Insert);
					if (success) cancelledFixedPurchaseIds.Add(fp.FixedPurchaseId);
				});

			// 管理者へのメール送信
			SendMail(
				"【成功】 " + Constants.MAIL_TITLE,
				CreateResultMessage(changedUserIds, cancelledFixedPurchaseIds));
		}

		/// <summary>
		/// キャンセルするための定期台帳取得
		/// </summary>
		/// <returns>定期台帳モデルの配列</returns>
		private static List<FixedPurchaseModel> GetFixedPurchaseCancelTarget()
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("FixedPurchaseMember", "GetFixedPurchaseCancelTarget"))
			{
				var param = new Hashtable
				{
					{ TARGET_DATE, DateTime.Now.Date }
				};
				var data = statement.SelectSingleStatementWithOC(accessor, param);
				var models = data.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToList();
				return models;
			}
		}

		/// <summary>
		/// 定期会員フラグOFFにする対象のユーザーID取得
		/// </summary>
		/// <returns>ユーザーIDの配列</returns>
		private static List<string> GetTargetUserIds()
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("FixedPurchaseMember", "GetTargetUserIds"))
			{
				var param = new Hashtable
				{
					{ TARGET_DATE, DateTime.Now.Date.AddDays(-365) }
				};
				var data = statement.SelectSingleStatementWithOC(accessor, param);
				if (data.Count == 0) return new List<string>();
				var userIds = data.Cast<DataRowView>()
					.Select(x => (string)x[Constants.FIELD_USER_USER_ID])
					.ToList();
				return userIds;
			}
		}
	}
}
