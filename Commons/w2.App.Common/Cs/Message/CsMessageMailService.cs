/*
=========================================================================================================
  Module      : メッセージメールサービス(CsMessageMailService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージメールサービス
	/// </summary>
	public class CsMessageMailService
	{
		/// <summary>レポジトリ</summary>
		private CsMessageMailRepository Repository;

		#region +コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsMessageMailService(CsMessageMailRepository repository)
		{
			this.Repository = repository;
		}

		#endregion

		#region +Get メール情報取得
		/// <summary>
		/// メール情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <returns>メール情報</returns>
		public CsMessageMailModel Get(string deptId, string mailId)
		{
			var dv = this.Repository.Get(deptId, mailId);
			if (dv.Count == 0) return null;
			var mail = new CsMessageMailModel(dv[0]);
			return mail;
		}
		#endregion

		#region +GetWithAttachment メール・添付情報取得
		/// <summary>
		/// メール・添付情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <returns>メール情報</returns>
		public CsMessageMailModel GetWithAttachment(string deptId, string mailId)
		{
			// メールデータ取得 & 添付メールデータ取得
			var dv = this.Repository.GetWithAttachment(deptId, mailId);
			if (dv.Count == 0) return null;

			var csMessageMail = new CsMessageMailModel(dv[0]);
			csMessageMail.EX_MailAttachments = dv[0].ToHashtable()["file_no"].ToString() != ""
				? dv.Cast<DataRowView>().Select(drv => new CsMessageMailAttachmentModel(drv)).ToArray()
				: new DataView().Cast<DataRowView>().Select(drv => new CsMessageMailAttachmentModel(drv)).ToArray();
			return csMessageMail;
		}
		#endregion

		#region +RegisterAll 全て（メール、メールデータ、添付ファイル）登録
		/// <summary>
		/// 全て（メール、メールデータ、添付ファイル）登録
		/// </summary>
		/// <param name="mailModel">メールモデル</param>
		/// <param name="isAttachmentPreregistered">添付ファイル仮登録済みか</param>
		/// <returns>メールID</returns>
		public string RegisterAll(CsMessageMailModel mailModel, bool isAttachmentPreregistered)
		{
			// メールID取得
			string mailId = NumberingUtility.CreateKeyId(mailModel.DeptId, Constants.NUMBER_KEY_CS_MESSAGE_MAIL_ID, 10);

			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// メール登録
				mailModel.MailId = mailId;
				this.Repository.Register(mailModel.DataSource, accessor);

				// メールデータ登録 ※承認依頼などのときはまだメールを作成してないので登録できない
				if (mailModel.EX_MailDataModel != null)
				{
					mailModel.EX_MailDataModel.MailId = mailId;
					var mailDataService = new CsMessageMailDataService(new CsMessageMailDataRepository());
					mailDataService.Register(mailModel.EX_MailDataModel, accessor);
				}

				// メール添付ファイル登録
				var mailAttachmentService = new CsMessageMailAttachmentService(new CsMessageMailAttachmentRepository());
				foreach (var attachment in mailModel.EX_MailAttachments)
				{
					attachment.MailId = mailId;
					if (isAttachmentPreregistered)
					{
						mailAttachmentService.UpdateTempIdToFormalId(attachment, accessor);
					}
					else
					{
						mailAttachmentService.Register(attachment, accessor);
					}
				}

				accessor.CommitTransaction();

				return mailId;
			}
		}
		#endregion

		#region +CreateDefaultReceiveModel 空の受信用メッセージメールモデル作成
		/// <summary>
		/// 空の受信用メッセージメールモデル作成
		/// </summary>
		/// <returns>メッセージメールモデル</returns>
		public static CsMessageMailModel CreateDefaultReceiveModel()
		{
			var model = new CsMessageMailModel();
			model.DeptId = Constants.CONST_DEFAULT_DEPT_ID;
			model.MailKbn = Constants.FLG_CSMESSAGEMAIL_MAIL_KBN_RECEIVE;
			model.MailFrom = "";
			model.MailTo = "";
			model.MailCc = "";
			model.MailSubject = "";
			model.MailBody = "";
			model.ReceiveDatetime = DateTime.Now;
			model.MessageId = "";
			model.InReplyTo = "";
			model.LastChanged = "";
			return model;
		}
		#endregion

		#region +UpdateAll 全て（メール、メールデータ、添付ファイル）更新
		/// <summary>
		/// 全て（メール、メールデータ、添付ファイル）更新
		/// </summary>
		/// <param name="mailModel">メールモデル</param>
		public void UpdateAll(CsMessageMailModel mailModel)
		{
			// トランザクション開始
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// メール更新
				this.Repository.Update(mailModel.DataSource, accessor);

				// メールデータ登録 ※承認依頼などのときはまだメールを作成してないので登録できない
				if (mailModel.EX_MailDataModel != null)
				{
					mailModel.EX_MailDataModel.MailId = mailModel.MailId;
					var mailDataService = new CsMessageMailDataService(new CsMessageMailDataRepository());
					mailDataService.Update(mailModel.EX_MailDataModel, accessor);
				}

				// メール添付ファイル登録
				var mailAttachmentService = new CsMessageMailAttachmentService(new CsMessageMailAttachmentRepository());
				foreach (var attachment in mailModel.EX_MailAttachments)
				{
					if (attachment.MailId == "")
					{
						attachment.MailId = mailModel.MailId;
						mailAttachmentService.UpdateTempIdToFormalId(attachment, accessor);
					}
				}

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +DeleteWithAtachmentAndDatas 添付とメールデータ含めて削除
		/// <summary>
		/// 添付とメールデータ含めて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteWithAtachmentAndDatas(string deptId, string mailId, SqlAccessor accessor)
		{
			// 削除
			this.Repository.Delete(deptId, mailId, accessor);

			// 添付すべて削除
			var messageMailAttachmentService = new CsMessageMailAttachmentService(new CsMessageMailAttachmentRepository());
			messageMailAttachmentService.DeleteAll(deptId, mailId, accessor);

			// メールデータ削除
			var messageMailDataService = new CsMessageMailDataService(new CsMessageMailDataRepository());
			messageMailDataService.Delete(deptId, mailId, accessor);

		}
		#endregion
	}
}
