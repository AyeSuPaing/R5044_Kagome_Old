/*
=========================================================================================================
  Module      : メール受信クラス(MailReceiver.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.Message;
using w2.App.Common.MailAssignSetting;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Util;
using w2.Domain.User;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	/// <summary>
	/// メール受信クラス
	/// </summary>
	public class MailReceiver
	{
		/// <summary>Mail receiver type</summary>
		public enum MailReceiverType
		{
			Mail,
			Gmail,
			Exchange
		}

		/// <summary>取り込みエラーメッセージのMailFrom表示文字列</summary>
		private const string ERROR_MESSAGE_MAIL_FROM = "SYSTEM";
		/// <summary>取り込みエラーメッセージのMailTo表示文字列</summary>
		private const string ERROR_MESSAGE_MAIL_TO = "ALL-OPERATORS";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MailReceiver()
		{
			// 振り分け設定取得
			var service = new CsMailAssignSettingService(new CsMailAssignSettingRepository());
			this.AssignSettings = service.GetAllWithItems(Constants.CONST_DEFAULT_DEPT_ID);

			// リポジトリサービス初期化
			this.IncidentService = new CsIncidentService(new CsIncidentRepository());
			this.MessageService = new CsMessageService(new CsMessageRepository());
			this.MessageMailService = new CsMessageMailService(new CsMessageMailRepository());

			this.Type = MailReceiverType.Mail;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="popSetting">POPアカウント情報</param>
		public MailReceiver(Pop3Client.PopAccountSetting popSetting)
			: this()
		{
			// POP設定セット
			this.PopSetting = popSetting;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="gmailSetting">Gmail Setting</param>
		public MailReceiver(GmailAccountSetting gmailSetting)
			: this()
		{
			this.GmailAccountSetting = gmailSetting;
			this.Type = MailReceiverType.Gmail;

			// POP設定セット
			this.PopSetting = new Pop3Client.PopAccountSetting(
				Constants.SERVER_POP,
				Constants.SERVER_POP_PORT,
				Constants.SERVER_POP_AUTH_USER_NAME,
				Constants.SERVER_POP_AUTH_PASSOWRD,
				Constants.SERVER_POP_TYPE);
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="exchangeAccountSetting">Exchange account setting</param>
		public MailReceiver(ExchangeAccountSetting exchangeAccountSetting)
			: this()
		{
			this.ExchangeAccountSetting = exchangeAccountSetting;
			this.Type = MailReceiverType.Exchange;

			// POP設定セット
			this.PopSetting = new Pop3Client.PopAccountSetting(
				Constants.SERVER_POP,
				Constants.SERVER_POP_PORT,
				Constants.SERVER_POP_AUTH_USER_NAME,
				Constants.SERVER_POP_AUTH_PASSOWRD,
				Constants.SERVER_POP_TYPE);
		}
		#endregion

		#region +ReceiveAndImport メール受信＆取込
		/// <summary>
		/// メール受信＆取込
		/// </summary>
		/// <returns>取り込み件数</returns>
		public int ReceiveAndImport()
		{
			IList<Pop3Message> mails;

			// 削除しないで受信
			using (var pop = new Pop3Client(this.PopSetting))
			{
				pop.Connect();
				mails = pop.GetMessages(false);
			}

			// 取込
			var deleteList = new HashSet<Pop3Message>();
			foreach (Pop3Message mail in mails)
			{
				// デコードできなかった場合は空文字を格納（振り分けルール判定のため）
				if (mail.BodyDecoded == null) mail.BodyDecoded = "";

				// Reply-ToがあればFrom書き換え
				if (mail.ReplyTo != null) mail.From = mail.ReplyTo;

				// メッセージIDがなければセット
				if (mail.MessageId == null) mail.SetMessageId(MessageIdGenerator.Generate());

				bool success = ImportMail(mail);

				// 成功したら削除リストに追加
				if (success) deleteList.Add(mail);
			}

			// 取り込み済みを削除
			using (var pop = new Pop3Client(this.PopSetting))
			{
				pop.Connect();
				pop.DeleteMessages(deleteList);
			}

			return deleteList.Count;
		}
		#endregion

		#region +ImportMail メール取込
		/// <summary>
		/// メール取込
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns>取り込み件数</returns>
		public int ImportMail(string filePath)
		{
			// ファイル名チェック
			if (Path.GetFileName(filePath).StartsWith(Constants.PROJECT_NO) == false)
			{
				FileLogger.WriteError("対象プロジェクトのメールではありません。");
				return 0;
			}

			// ファイル読み込み
			var src = File.ReadAllText(filePath, Encoding.GetEncoding("Shift_JIS"));

			// メール解析
			var mail = new Pop3Message(src);

			// デコードできなかった場合は空文字を格納（振り分けルール判定のため）
			if (mail.BodyDecoded == null) mail.BodyDecoded = "";

			// Reply-ToがあればFrom書き換え
			if (mail.ReplyTo != null) mail.From = mail.ReplyTo;

			// メッセージIDがなければセット
			if (mail.MessageId == null) mail.SetMessageId(MessageIdGenerator.Generate());

			var success = ImportMail(mail);

			return (success ? 1 : 0);
		}
		#endregion

		#region -ImportMail メール取込
		/// <summary>
		/// メール取込
		/// </summary>
		/// <param name="mail">メール</param>
		/// <returns>取り込み成功したかどうか</returns>
		public bool ImportMail(Pop3Message mail)
		{
			// メール解析で失敗したときはnullが入ってくる
			if (mail == null)
			{
				// ログ出力
				AppLogger.WriteInfo(string.Format("  [Account:{0}][失敗] メール本体の解析に失敗しました、ログを確認してください。",
					this.UserIdOrEmail));

				// オペレータ通知用にメッセージとして登録
				var subject = "[SYSTEM] メール受信失敗";
				var body = string.Format("メール本体の解析に失敗しました。\r\nアカウント: {0}\r\n\r\nシステム管理者に連絡してください。",
					this.UserIdOrEmail);
				RegisterErrorIncidentMessage(subject, body);

				return false;
			}

			try
			{

				// メール内容がnullの場合取込スキップする
				if ((mail.From == null) && (mail.To == null) && (mail.BodyRaw == null))
				{
					AppLogger.WriteInfo(string.Format("  [Account:{0}]メール内容が存在しないため、取込をスキップしました",
						this.UserIdOrEmail));

					return false;
				}

				// 紐付くユーザーを取得
				var user = new UserService().GetUserByMailAddr(mail.From.AddressRaw);

				// 紐付くインシデントを取得（In-Reply-To、Referencesを判定）
				CsIncidentModel incident = null;

				// InReplyToが空の場合に紐づけをすると、予期しないものにつくので新しいインシデントとする
				if (string.IsNullOrEmpty(mail.InReplyTo) == false)
				{
					CsIncidentModel[] incidents = this.IncidentService.GetByMessageId(Constants.CONST_DEFAULT_DEPT_ID, mail.InReplyTo);
					if (incidents.Length > 0) incident = incidents[0];
					if (incident == null)
					{
						mail.References.Where(r => r != mail.InReplyTo && string.IsNullOrEmpty(r) == false).ToList().ForEach(r =>
						{
							incidents = this.IncidentService.GetByMessageId(Constants.CONST_DEFAULT_DEPT_ID, r);
							if (incidents.Length > 0) incident = incidents[0];
						});
					}
				}

				// メール振分け適用
				MailAssign(mail, incident, user);

				return true;
			}
			catch (Exception ex)
			{
				// ログ出力
				AppLogger.WriteError(ex);
				string errorString = string.Format("  [Account:{0}][失敗] メール取り込みに失敗しました、ログを確認してください。(Message-ID:{1}, From:{2}, Subject:{3})",
					this.UserIdOrEmail,
					mail.MessageId,
					mail.From.Address,
					mail.Subject);
				Console.WriteLine(errorString);
				AppLogger.WriteInfo(errorString);

				// オペレータ通知用にメッセージとして登録
				string subject = "[SYSTEM] メール取り込み失敗";
				string body = string.Format("メール取り込みに失敗しました。\r\nアカウント: {0}\r\nMessage-ID:{1}\r\nFrom:{2}\r\nSubject:{3}\r\n\r\nシステム管理者に連絡してください。",
					this.UserIdOrEmail,
					mail.MessageId,
					mail.From.Address,
					mail.Subject);
				RegisterErrorIncidentMessage(subject, body);

				return false;
			}
		}
		#endregion

		#region -RegisterErrorIncidentMessage 取り込みエラーとしてインシデント/メッセージ登録
		/// <summary>
		/// 取り込みエラーとしてインシデント/メッセージ登録
		/// </summary>
		/// <param name="subject">タイトル文字列</param>
		/// <param name="body">本文文字列</param>
		private void RegisterErrorIncidentMessage(string subject, string body)
		{
			var lastChanged = "batch";

			// インシデント登録
			var incident = CsIncidentService.CreateDefaultModel();
			incident.IncidentTitle = subject;
			incident.LastChanged = lastChanged;
			incident.IncidentId = this.IncidentService.RegisterUpdateWithSummaryValues(incident);

			// メッセージメール登録
			var message = CsMessageMailService.CreateDefaultReceiveModel();
			message.MailFrom = ERROR_MESSAGE_MAIL_FROM;
			message.MailTo = ERROR_MESSAGE_MAIL_TO;
			message.MailSubject = subject;
			message.MailBody = body;
			message.LastChanged = lastChanged;
			var mailId = this.MessageMailService.RegisterAll(message, false);

			// メッセージ登録
			var messageMail = CsMessageService.CreateDefaultReceivedMailModel();
			messageMail.IncidentId = incident.IncidentId;
			messageMail.ReceiveMailId = mailId;
			messageMail.LastChanged = lastChanged;
			this.MessageService.Register(messageMail);
		}
		#endregion

		#region -MailAssign メール振分け＆登録
		/// <summary>
		/// メール振分け＆登録
		/// </summary>
		/// <param name="mail">メール</param>
		/// <param name="boundIncident">紐付いたインシデント</param>
		/// <param name="boundUser">紐付いたユーザ</param>
		private void MailAssign(Pop3Message mail, CsIncidentModel boundIncident, UserModel boundUser)
		{
			// メッセージメール登録
			var mailModel = CreateMessageMailModel(mail);
			var receiveMailId = this.MessageMailService.RegisterAll(mailModel, false);

			// 適用する振分け設定を作成
			var applySetting = CreateMailAssignSetting(mail, boundIncident);

			// インシデント登録
			var targetIncident = CreateTargetIncident(boundIncident, applySetting);
			targetIncident.LastChanged = "batch";
			if (boundIncident == null)
			{
				// 新規インシデント登録時
				targetIncident.IncidentTitle = mail.Subject;
				targetIncident.UserName = mail.From.DisplayName;
				targetIncident.UserContact = mail.From.AddressRaw;
				targetIncident.UserId = (boundUser == null) ? "" : boundUser.UserId;
				if (applySetting.Trash == Constants.FLG_CSMAILASSIGNSETTING_TRASH_VALID) targetIncident.ValidFlg = Constants.FLG_CSINCIDENT_VALID_FLG_INVALID;
			}
			else
			{
				// 既存インシデント更新時
				targetIncident.UserId = boundIncident.UserId;
				if (string.IsNullOrEmpty(targetIncident.UserId) && (boundUser != null))
				{
					targetIncident.UserId = boundUser.UserId;
				}
				// ステータスが「対応済み」の場合は「未対応」に更新
				if (targetIncident.Status == Constants.FLG_CSINCIDENT_STATUS_COMPLETE)
				{
					targetIncident.Status = Constants.FLG_CSINCIDENT_STATUS_NONE;
				}
				// 最終受信日時更新
				this.IncidentService.UpdateLastReceivedDate(targetIncident);
			}
			var incidentId = this.IncidentService.RegisterUpdateWithSummaryValues(targetIncident);

			// メッセージ登録
			var incident = this.IncidentService.Get(Constants.CONST_DEFAULT_DEPT_ID, incidentId);
			var message = CreateMessage(mail, mailModel, incident, receiveMailId);
			message.ValidFlg = (applySetting.Trash == Constants.FLG_CSMAILASSIGNSETTING_TRASH_VALID)
				? Constants.FLG_CSMESSAGE_VALID_FLG_INVALID
				: Constants.FLG_CSMESSAGE_VALID_FLG_VALID;
			var messageId = this.MessageService.Register(message);

			// ログ出力
			Console.WriteLine("IncidentId={0}, MessageId={1}, MailId={2}, UserId={3}", incident.IncidentId, messageId, receiveMailId, targetIncident.UserId);
			AppLogger.WriteInfo(string.Format("  [Account:{0}][成功] IncidentId={1}{2}, MessageId={3,-2}, MailId={4}, UserId={5}",
				this.UserIdOrEmail,
				incident.IncidentId,
				((boundIncident == null) ? "(NEW) " : "(BIND)"),
				messageId,
				receiveMailId,
				incident.UserId));
		}
		#endregion

		#region -SendAutoResponseMail オートレスポンスメールを送信
		/// <summary>
		/// オートレスポンスメールを送信
		/// </summary>
		/// <param name="mail">受信メール</param>
		/// <param name="applySetting">メール振り分け設定</param>
		private void SendAutoResponseMail(Pop3Message mail, CsMailAssignSettingModel applySetting)
		{
			var message = new MailMessage();

			if (Regex.IsMatch(mail.From.Address, @"^[a-zA-Z0-9_+-]+(\.[a-zA-Z0-9_+-]+)*@([a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9]*\.)+[a-zA-Z]{2,}$"))
			{
				message.To.Add(new MailAddress(mail.From.Address, mail.From.DisplayName));
				message.CC.AddRange(
					(from m in applySetting.AutoResponseCc.Split(',') where (m.Trim() != "") select MailAddress.GetInstance(m.Trim())).ToArray());
				message.Bcc.AddRange(
					(from m in applySetting.AutoResponseBcc.Split(',') where (m.Trim() != "") select MailAddress.GetInstance(m.Trim())).ToArray());
				message.From = MailAddress.GetInstance(applySetting.AutoResponseFrom);
				message.Subject = applySetting.AutoResponseSubject;
				message.Body = applySetting.AutoResponseBody;
			}
			else
			{
				FileLogger.WriteInfo(
					string.Format(
						"  [Account:{0}][失敗] オートレスポンス時の送信先メールアドレスが不正です。",
					mail.From.Address));
				return;
			}

			var sender = new SmtpMailSender(
				Constants.SERVER_SMTP,
				Constants.SERVER_SMTP_PORT,
				message);

			if (sender.SendMail() == false)
			{
				Console.WriteLine(sender.MailSendException.ToString());
				AppLogger.WriteError(sender.MailSendException);
			}
		}
		#endregion

		#region -CreateMailAssignSetting メール振分け設定作成
		/// <summary>
		/// メール振分け設定作成
		/// </summary>
		/// <param name="mail">メール文章</param>
		/// <param name="boundIncident">紐付くインシデント</param>
		/// <returns>メール振分け設定</returns>
		private CsMailAssignSettingModel CreateMailAssignSetting(Pop3Message mail, CsIncidentModel boundIncident)
		{
			CsMailAssignSettingModel applySetting = null;
			foreach (CsMailAssignSettingModel setting in this.AssignSettings)
			{
				var isMatch = false;

				// 振分け条件の判定
				if (setting.EX_IsMatchOnBind)
				{
					if (boundIncident != null) isMatch = true;
				}
				else if (IsMatchAssignSetting(setting, mail) || setting.EX_IsMatchAnything)
				{
					isMatch = true;
				}

				// 振分けアクション作成
				if (isMatch)
				{
					if (applySetting == null)
					{
						applySetting = (CsMailAssignSettingModel)setting.Clone();
					}
					else
					{
						applySetting.Merge(setting);
					}
				}

				// 振分け停止、オートレスポンスの適用
				if (isMatch)
				{
					// メール送信
					if (setting.AutoResponse == Constants.FLG_CSMAILASSIGNSETTING_AUTO_RESPONSE_VALID)
					{
						SendAutoResponseMail(mail, setting);
					}

					// 振分け停止
					if (setting.StopFiltering == Constants.FLG_CSMAILASSIGNSETTING_STOP_FILTERING_VALID) break;
				}
			}
			return applySetting;
		}
		#endregion

		#region -IsMatchAssignSetting 該当のメールが、メール振り分け条件のどれかにマッチするかを判定
		/// <summary>
		/// 該当のメールが、メール振り分け条件のどれかにマッチするかを判定します。
		/// </summary>
		/// <param name="setting">メール振り分け設定</param>
		/// <param name="mail">受信メール</param>
		/// <returns>マッチすればtrue</returns>
		private static bool IsMatchAssignSetting(CsMailAssignSettingModel setting, Pop3Message mail)
		{
			var isMatchList = new List<bool>();

			// 個々の振り分け条件を判定
			foreach (var settingDetail in setting.Items)
			{
				var result = IsMatchAssignSettingItem(settingDetail, mail);
				isMatchList.Add(result);
			}

			// この振り分け条件を適用するかどうか確定
			var isAnd = (setting.LogicalOperator == Constants.FLG_CSMAILASSIGNSETTING_LOGICAL_OPERATOR_AND);	// かつ
			var isOr = (setting.LogicalOperator == Constants.FLG_CSMAILASSIGNSETTING_LOGICAL_OPERATOR_OR);		// または

			if (isAnd) return (isMatchList.Contains(false) == false);
			if (isOr) return isMatchList.Contains(true);

			throw new Exception();
		}
		#endregion

		#region -IsMatchAssignSettingItem 該当のメールが、個々のメール振り分け条件にマッチするかどうかを判定
		/// <summary>
		/// 該当のメールが、個々のメール振り分け条件にマッチするかどうかを判定します。
		/// </summary>
		/// <param name="detail">メール振り分け条件詳細</param>
		/// <param name="mail">受信メール</param>
		/// <returns>マッチすればtrue</returns>
		private static bool IsMatchAssignSettingItem(CsMailAssignSettingItemModel detail, Pop3Message mail)
		{
			var value = detail.MatchingValue;
			var ignoreCase = (detail.IgnoreCase == Constants.FLG_CSMAILASSIGNSETTINGITEM_IGNORE_CASE_VALID);
			bool isContains;
			switch (detail.MatchingTarget)
			{
				case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_SUBJECT:
					isContains = StringExtention.Contains(mail.Subject, value, ignoreCase);
					break;

				case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_BODY:
					isContains = StringExtention.Contains(mail.BodyDecoded, value, ignoreCase);
					break;

				case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_HEADER:
					isContains = false;
					foreach (string key in mail.Header.Values.Keys)
					{
						foreach (string v in mail.Header.Values.GetValues(key))
						{
							if (StringExtention.Contains(v, value, ignoreCase)) isContains = true;
						}
					}
					break;

				case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_FROM:
					isContains = StringExtention.Contains(mail.From.AddressRaw, value, ignoreCase);
					break;

				case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_TOCC:
					isContains = false;
					foreach (MailAddress addr in mail.To)
					{
						isContains |= StringExtention.Contains(addr.AddressRaw, value, ignoreCase);
					}
					foreach (MailAddress addr in mail.Cc)
					{
						isContains |= StringExtention.Contains(addr.AddressRaw, value, ignoreCase);
					}
					break;

				case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_TO:
					isContains = false;
					foreach (MailAddress addr in mail.To)
					{
						isContains |= StringExtention.Contains(addr.AddressRaw, value, ignoreCase);
					}
					break;

				case Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_CC:
					isContains = false;
					foreach (MailAddress addr in mail.Cc)
					{
						isContains |= StringExtention.Contains(addr.AddressRaw, value, ignoreCase);
					}
					break;

				default:
					throw new Exception("不正な振り分け項目です。");
			}

			return isContains ^ (detail.MatchingType == Constants.FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE_NO_INCLUDE);
		}
		#endregion

		#region -CreateMessageMailModel  取り込んだ受信メールをもとに、MessageMailModel を作成
		/// <summary>
		/// 取り込んだ受信メールをもとに、MessageMailModel を作成します。
		/// </summary>
		/// <param name="mail">受信メール</param>
		/// <returns>メッセージメールモデル</returns>
		private CsMessageMailModel CreateMessageMailModel(Pop3Message mail)
		{
			var messageMail = CsMessageMailService.CreateDefaultReceiveModel();
			messageMail.MailFrom = MailAddress.GetMailAddrString(mail.From.DisplayName, mail.From.AddressRaw);
			messageMail.MailTo = string.Join(", ", mail.To.Select(a => MailAddress.GetMailAddrString(a.DisplayName, a.AddressRaw)).ToArray());
			messageMail.MailCc = string.Join(", ", mail.Cc.Select(a => MailAddress.GetMailAddrString(a.DisplayName, a.AddressRaw)).ToArray());
			messageMail.MailSubject = mail.Subject ?? "";
			messageMail.MailBody = mail.BodyDecoded;
			messageMail.ReceiveDatetime = mail.Date;
			messageMail.MessageId = mail.MessageId ?? "";
			messageMail.InReplyTo = mail.InReplyTo ?? "";
			messageMail.LastChanged = "batch";

			// メールデータの格納
			CsMessageMailDataModel mailData = new CsMessageMailDataModel();
			mailData.DeptId = messageMail.DeptId;
			mailData.MailData = mail.Source;
			messageMail.EX_MailDataModel = mailData;

			// メール添付ファイルの格納
			var fileNo = 1;
			string multipartMailBody = null;
			var attachments = new List<CsMessageMailAttachmentModel>();

			// 1階層目の通常パート群, 1階層目のマルチパートの下位パート の順番で平坦化パートリストを作成
			var normalParts = mail.Attachment.Where(a => a.IsMultipart == false);
			var multipartChildParts = mail.Attachment.Where(a => a.IsMultipart).SelectMany(a => a.Attachment);
			var targetParts = normalParts.Concat(multipartChildParts).ToArray();

			foreach (Pop3Message part in targetParts)
			{
				// HTMLメールを考慮し、マルチパート内に添付ファイルでないテキスト文章があれば抽出（のちに本文と置き換え）
				if ((multipartMailBody == null)
					&& part.ContentType.MediaType.ToLower().StartsWith("text/")
					&& (part.IsAttachmentFile == false))
				{
					multipartMailBody = part.BodyDecoded;
					continue;
				}
				CsMessageMailAttachmentModel attachment = new CsMessageMailAttachmentModel();
				attachment.DeptId = messageMail.DeptId;
				attachment.FileNo = fileNo++;
				attachment.FileName = (part.ContentType.Name == null) ? null : System.Text.Encoding.Unicode.GetString(Convert.FromBase64String(part.ContentType.Name));
				attachment.FileData = part.BodyBytes;

				// ContentType.Nameがない場合はContentDispositionからファイル名を取得する。
				// それでも取得できなかった場合は連番ファイル名を振る。
				// 送信先不明などで返される multipart/report 等の取り込み時に発生します。
				if (attachment.FileName == "")
				{
					if (part.Header.ContentDisposition.FileName != null)
					{
						attachment.FileName = part.Header.ContentDisposition.FileName;
					}
					else
					{
						string fileName = "file_" + attachment.FileNo;
						attachment.FileName = fileName + ((part.ContentType.MediaType.ToLower().StartsWith("text/htm")) ? ".html" : ".txt");
					}
				}

				attachments.Add(attachment);
			}
			messageMail.MailBody = multipartMailBody ?? messageMail.MailBody;
			messageMail.EX_MailAttachments = attachments.ToArray();

			return messageMail;
		}
		#endregion

		#region -CreateTargetIncident 登録/更新用の振分け設定適用済みインシデントを作成します。
		/// <summary>
		/// 登録/更新用の振分け設定適用済みインシデントを作成します。
		/// </summary>
		/// <param name="incident">更新対象インシデント、登録時はnull</param>
		/// <param name="setting">メール振分け設定</param>
		/// <returns>インシデント</returns>
		private CsIncidentModel CreateTargetIncident(CsIncidentModel incident, CsMailAssignSettingModel setting)
		{
			var model = incident ?? CsIncidentService.CreateDefaultModel();

			if (setting.AssignIncidentCategoryId != "") model.IncidentCategoryId = setting.AssignIncidentCategoryId;
			if (setting.AssignStatus != "") model.Status = setting.AssignStatus;
			if (setting.AssignImportance != "") model.Importance = setting.AssignImportance;
			if (setting.AssignCsGroupId != "") model.CsGroupId = (setting.AssignCsGroupId == Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_GROUP_ID_CLEAR) ? "" : setting.AssignCsGroupId;
			if (setting.AssignOperatorId != "") model.OperatorId = (setting.AssignOperatorId == Constants.FLG_CSMAILASSIGNSETTING_ASSIGN_OPERATOR_ID_CLEAR) ? "" : setting.AssignOperatorId;

			return model;
		}
		#endregion

		#region -CreateMessage 新規登録用の CsMessageModel を作成
		/// <summary>
		/// 新規登録用の CsMessageModel を作成します。
		/// </summary>
		/// <param name="mail">受信メール</param>
		/// <param name="mailModel">メッセージメール</param>
		/// <param name="incident">インシデント</param>
		/// <param name="mailId">受信メール識別ID</param>
		/// <returns>メッセージ</returns>
		private CsMessageModel CreateMessage(Pop3Message mail, CsMessageMailModel mailModel, CsIncidentModel incident, string mailId)
		{
			var model = CsMessageService.CreateDefaultReceivedMailModel();
			model.IncidentId = incident.IncidentId;
			model.ReceiveMailId = mailId;
			model.OperatorId = "";
			model.InquiryReplyDate = mail.Date;
			model.UserName1 = mail.From.DisplayName;
			model.UserMailAddr = mail.From.AddressRaw;
			model.InquiryTitle = mail.Subject;
			model.InquiryText = mailModel.MailBody;
			model.LastChanged = "batch";
			return model;
		}
		#endregion

		#region プロパティ
		/// <summary>POPアカウント情報</summary>
		private Pop3Client.PopAccountSetting PopSetting { get; set; }
		/// <summary>受信メール振り分け設定リスト</summary>
		private CsMailAssignSettingModel[] AssignSettings;
		/// <summary>インシデントサービス</summary>
		private CsIncidentService IncidentService;
		/// <summary>メッセージサービス</summary>
		private CsMessageService MessageService;
		/// <summary>メッセージメールサービス</summary>
		private CsMessageMailService MessageMailService;
		/// <summary>Gmail Account Setting</summary>
		public GmailAccountSetting GmailAccountSetting { get; set; }
		/// <summary>Mail receiver type</summary>
		public MailReceiverType Type { get; set; }

		/// <summary>User ID or email</summary>
		public string UserIdOrEmail
		{
			get
			{
				var result = string.Empty;
				switch (Type)
				{
					case MailReceiverType.Mail:
						result = this.PopSetting.UserId;
						break;

					case MailReceiverType.Gmail:
						result = this.GmailAccountSetting.Account;
						break;

					case MailReceiverType.Exchange:
						result = this.ExchangeAccountSetting.Account;
						break;
				}

				return result;
			}
		}
		/// <summary>Exchange account setting</summary>
		public ExchangeAccountSetting ExchangeAccountSetting { get; set; }
		#endregion
	}
}
