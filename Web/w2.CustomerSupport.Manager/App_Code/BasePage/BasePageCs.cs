/*
=========================================================================================================
  Module      : CS基底ページ(BasePageCs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.Message;
using w2.App.Common.Cs.Reports;
using w2.Common.Net.Mail;
using w2.Common.Util;
using w2.Domain.MailSendLog;
using w2.Domain.User;

/// <summary>
/// BasePageCs の概要の説明です
/// </summary>
public class BasePageCs : BasePage
{
	/// <summary>メールアクション種別・区分対応配列</summary>
	private static KeyValuePair<MailActionType, string>?[] m_mailActionTypeKbnPairs =
	{
		new KeyValuePair<MailActionType, string>(MailActionType.ApprovalCancel, Constants.KBN_MAILACTION_APPROVE_CANCEL),
		new KeyValuePair<MailActionType, string>(MailActionType.ApprovalOK, Constants.KBN_MAILACTION_APPROVE_OK),
		new KeyValuePair<MailActionType, string>(MailActionType.ApprovalNG, Constants.KBN_MAILACTION_APPROVE_NG),
		new KeyValuePair<MailActionType, string>(MailActionType.ApprovalOKSend, Constants.KBN_MAILACTION_APPROVE_OK_SEND),
		new KeyValuePair<MailActionType, string>(MailActionType.SendCancel, Constants.KBN_MAILACTION_SEND_CANCEL),
		new KeyValuePair<MailActionType, string>(MailActionType.SendOK, Constants.KBN_MAILACTION_SEND_OK),
		new KeyValuePair<MailActionType, string>(MailActionType.SendNG, Constants.KBN_MAILACTION_SEND_NG)
	};
	/// <summary>メールアクション種別・文字列対応配列</summary>
	private static Dictionary<MailActionType, string> m_mailActionTypeStringPairs = new Dictionary<MailActionType,string>
	{
		{MailActionType.MailSend, "メール送信"},
		{MailActionType.ApprovalRequest, "承認依頼"},
		{MailActionType.MailSendRequest, "メール送信依頼"},
		{MailActionType.ApprovalCancel, "承認依頼取り下げ"},
		{MailActionType.ApprovalOK, "承認OK"},
		{MailActionType.ApprovalNG, "承認依頼差し戻し"},
		{MailActionType.SendCancel, "送信依頼取り下げ"},
		{MailActionType.SendOK, "送信OK"},
		{MailActionType.SendNG, "送信依頼差し戻し"}
	};

	#region #GetMailActionString メールアクション文字列取得（主に通知メール文章に利用）
	/// <summary>
	/// メールアクション文字列取得（主に通知メール文章に利用）
	/// </summary>
	/// <param name="type">メールアクション種別</param>
	/// <returns>メールアクション文字列</returns>
	protected string GetMailActionString(MailActionType type)
	{
		return m_mailActionTypeStringPairs.ContainsKey(type) ? m_mailActionTypeStringPairs[type] : "";
	}
	#endregion

	#region #GetMailActionTypeFromKbn メールアクション種別取得
	/// <summary>
	/// メールアクション種別取得
	/// </summary>
	/// <param name="mailActionKbn">メールアクション区分</param>
	/// <returns>メールアクション種別</returns>
	public static MailActionType GetMailActionTypeFromKbn(string mailActionKbn)
	{
		var target = m_mailActionTypeKbnPairs.FirstOrDefault(m => m.HasValue && (m.Value.Value == mailActionKbn));
		if (target == null) throw new Exception("不正なメールアクション区分：" + mailActionKbn);

		return target.Value.Key;
	}
	#endregion

	#region #GetMailActionKbnFromType メールアクション区分取得
	/// <summary>
	/// メールアクション区分取得
	/// </summary>
	/// <param name="MailActionType">メールアクション種別</param>
	/// <returns>メールアクション区分</returns>
	public static string GetMailActionKbnFromType(MailActionType type)
	{
		var target = m_mailActionTypeKbnPairs.FirstOrDefault(m => m.HasValue && (m.Value.Key == type));
		if (target == null) throw new Exception("不正なメールアクション種別：" + type.ToString());

		return target.Value.Value;
	}
	#endregion

	#region -RegisterUpdateIncidentAndMessageAll インシデント・メッセージすべて登録更新
	/// <summary>
	/// インシデント・メッセージすべて登録更新
	/// </summary>
	/// <param name="incident">インシデント</param>
	/// <param name="message">メッセージ</param>
	/// <param name="messageMail">メッセージメール</param>
	/// <param name="tempAttachments">メール添付（テンポラリ）</param>
	protected void RegisterUpdateIncidentAndMessageAll(
		CsIncidentModel incident,
		CsMessageModel message,
		CsMessageMailModel messageMail = null,
		CsMessageMailAttachmentModel[] tempAttachments = null)
	{
		// インシデントの問合せ元名称などセット
		if (messageMail != null)
		{
			if (string.IsNullOrEmpty(incident.UserContact)) incident.UserContact = messageMail.MailTo;

			// メール送信時のユーザー紐付け処理
			MailAddress[] addressList = (from m in messageMail.MailTo.Split(',') where (m.Trim() != "") select MailAddress.GetInstance(m.Trim())).ToArray();
			if (addressList.Length > 0)
			{
				var boundUsers = new UserService().GetUsersByMailAddr(addressList[0].AddressRaw);
				if (string.IsNullOrEmpty(incident.UserId) && (boundUsers.Length == 1))
				{
					incident.UserId = boundUsers[0].UserId;
				}
			}
			// 回答にメール本文セット
			message.InquiryTitle = messageMail.MailSubject;
			message.ReplyText = messageMail.MailBody;
		}
		else
		{
			incident.UserName = StringUtility.ToNull(message.UserName1 + message.UserName2);
			incident.UserContact = StringUtility.ToNull(message.UserTel1);
		}

		// インシデント登録／更新
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		string incidentId = incidentService.RegisterUpdateWithSummaryValues(incident);

		// メール登録／更新
		if (messageMail != null)
		{
			var mailId = RegisterUpdateMailAndAttachments(messageMail, tempAttachments);
			messageMail.MailId = mailId;
			message.SendMailId = mailId;
		}

		// メッセージ登録
		message.IncidentId = incidentId;
		message.EX_Mail = messageMail;
		int messageNo = RegisterUpdateMessage(message);

		// 下書き依頼情報は削除
		var service = new CsMessageRequestService(new CsMessageRequestRepository());
		service.DeleteDraftRequestWithItems(message.DeptId, message.IncidentId, message.MessageNo);

		// リクエスト登録
		if (message.EX_Request != null)
		{
			message.EX_Request.IncidentId = incidentId;
			message.EX_Request.MessageNo = messageNo;
			foreach (var item in message.EX_Request.EX_Items)
			{
				item.IncidentId = incidentId;
				item.MessageNo = messageNo;
			}
			var requestService = new CsMessageRequestService(new CsMessageRequestRepository());
			requestService.RegisterWithAllItems(message.EX_Request);
		}

		// 担当変更メール送信
		if (incident.EX_OperatorIdBefore != incident.OperatorId)
		{
			var sender = new NotificationMailSender(this.LoginOperatorShopId);
			sender.SendNotificationMailForIncidentOperatorChanged(
				this.LoginOperatorDeptId,
				incident.IncidentId,
				incident.OperatorId);
		}
	}
	#endregion

	#region -RegisterUpdateMailAndAttachments メッセージメール・添付ファイル登録更新
	/// <summary>
	///　メッセージメール・添付ファイル登録更新
	/// </summary>
	/// <param name="sendMail">送信メールモデル</param>
	/// <param name="tempAttachments">添付ファイル（テンポラリ）</param>
	/// <returns>メッセージメールID</returns>
	private string RegisterUpdateMailAndAttachments(CsMessageMailModel sendMail, CsMessageMailAttachmentModel[] tempAttachments)
	{
		var attachmentList = new List<CsMessageMailAttachmentModel>();
		for (int i = 0; i < tempAttachments.Length; i++)
		{
			var attachment = new CsMessageMailAttachmentModel();
			attachment.DeptId = this.LoginOperatorDeptId;
			attachment.FileNo = i + 1;
			attachment.FileName = tempAttachments[i].FileName;
			attachment.EX_FileNoBefore = tempAttachments[i].FileNo;
			attachment.MailId = tempAttachments[i].MailId;
			attachmentList.Add(attachment);
		}
		sendMail.EX_MailAttachments = attachmentList.ToArray();

		var mailService = new CsMessageMailService(new CsMessageMailRepository());
		var sendMailId = sendMail.MailId;
		if (sendMailId == "")
		{
			sendMailId = mailService.RegisterAll(sendMail, true);
		}
		else
		{
			mailService.UpdateAll(sendMail);
		}

		return sendMailId;
	}
	#endregion

	#region -RegisterUpdateMessage メッセージ情報登録更新
	/// <summary>
	/// メッセージ情報登録更新
	/// </summary>
	/// <param name="message">メッセージモデル</param>
	/// <returns>メッセージNO</returns>
	private int RegisterUpdateMessage(CsMessageModel message)
	{
		int messageNo;
		var service = new CsMessageService(new CsMessageRepository());
		if (message.MessageNo == 0)
		{
			messageNo = service.Register(message);
		}
		else
		{
			service.Update(message);
			messageNo = message.MessageNo;
		}

		return messageNo;
	}
	#endregion
	
	#region #SendNotificationMailForRequest 依頼通知メール送信
	/// <summary>
	/// 依頼通知メール送信
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	/// <param name="sendOperator">送信先オペレータ</param>
	/// <param name="fromOperatorName">依頼元オペレータ名</param>
	/// <param name="mailActionType">メールアクション種別</param>
	/// <param name="comment">依頼文言</param>
	/// <returns>送信成功したか（スキップはtrueとする）</returns>
	protected bool SendNotificationMailForRequest(string incidentId, CsOperatorModel sendOperator, string fromOperatorName, MailActionType mailActionType, string comment)
	{
		// 通知メールNGならスキップ
		if (sendOperator.EX_NotifyInfoFlg == false) return true;

		// URL作成
		string mailActionString = GetMailActionString(mailActionType);
		string nextUrl = Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_TOP_PAGE;
		switch (mailActionType)
		{
			case MailActionType.ApprovalRequest:
				nextUrl += "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + HttpUtility.UrlEncode(TopPageKbn.Approval.ToString());
				break;

			case MailActionType.MailSendRequest:
				nextUrl += "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + HttpUtility.UrlEncode(TopPageKbn.Send.ToString());
				break;

			case MailActionType.ApprovalOK:
			case MailActionType.ApprovalNG:
				nextUrl += "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + HttpUtility.UrlEncode(TopPageKbn.ApprovalResult.ToString());
				break;

			case MailActionType.SendNG:
				nextUrl += "?" + Constants.REQUEST_KEY_CS_TOPPAGE_KBN + "=" + HttpUtility.UrlEncode(TopPageKbn.SendResult.ToString());
				break;
		}

		// メール送信
		var mailSender = new NotificationMailSender(this.LoginOperatorShopId);
		var result = mailSender.SendNotificationMailForRequest(
			incidentId,
			sendOperator.MailAddr,
			fromOperatorName,
			mailActionString,
			comment,
			nextUrl);

		return result;
	}
	#endregion

	#region #ExecReceiveMailBatch メール受信バッチ起動
	/// <summary>
	/// メール受信バッチ起動
	/// </summary>
	/// <returns>取り込み件数</returns>
	protected int ExecReceiveMailBatch()
	{
		var proc = System.Diagnostics.Process.Start(Constants.PHYSICALDIRPATH_CSMAILRECEIVER_EXE);

		while (proc.HasExited == false)
		{
			System.Threading.Thread.Sleep(500);
		}

		return proc.ExitCode;
	}
	#endregion

	#region #SendMail メール送信実行
	/// <summary>
	/// メール送信実行
	/// </summary>
	/// <param name="sendMail">送信メール（送信後メールデータがセットされる）</param>
	/// <param name="mailAttachemnts">添付ファイル</param>
	/// <param name="userId">ユーザID</param>
	/// <param name="mailAddress">メールアドレス</param>
	/// <returns>メール送信成功したか</returns>
	protected bool SendMail(CsMessageMailModel sendMail, CsMessageMailAttachmentModel[] mailAttachemnts, string userId, string mailAddress)
	{
		var result = true;

		MailMessage message = new MailMessage();
		message.To.AddRange(
			(from m in sendMail.MailTo.Split(',') where (m.Trim() != "") select MailAddress.GetInstance(m.Trim())).ToArray());
		message.CC.AddRange(
			(from m in sendMail.MailCc.Split(',') where (m.Trim() != "") select MailAddress.GetInstance(m.Trim())).ToArray());
		message.Bcc.AddRange(
			(from m in sendMail.MailBcc.Split(',') where (m.Trim() != "") select MailAddress.GetInstance(m.Trim())).ToArray());
		message.From = MailAddress.GetInstance(sendMail.MailFrom);
		message.Subject = sendMail.MailSubject;
		message.Body = sendMail.MailBody;
		message.MessageId = sendMail.MessageId;
		message.InReplyTo = sendMail.InReplyTo;
		if (string.IsNullOrEmpty(Constants.ERROR_MAILADDRESS) == false) message.ReturnPath = new MailAddress(Constants.ERROR_MAILADDRESS);
		message.References = sendMail.EX_References;
		// HACK:既存メール送信モジュールがファイル名を渡す仕様なので一度テンポラリに格納してそれを渡しているが直したい
		string tempPath = null;
		if (mailAttachemnts.Length != 0)
		{
			var attachmentTempFilePaths = CreateTempFile(mailAttachemnts, out tempPath);
			message.AttachmentFilePath.AddRange(attachmentTempFilePaths);
		}

		var sender = new SmtpMailSender(
			Constants.SERVER_SMTP,
			Constants.SERVER_SMTP_PORT,
			message);

		if (sender.SendMail(userId, mailAddress) == false)
		{
			AppLogger.WriteError(sender.MailSendException);
			result = false;
		}

			// ユーザーIDが存在する AND CSオプションが有効の場合はメール送信ログ登録
		if ((string.IsNullOrEmpty(userId) == false) && Constants.CS_OPTION_ENABLED)
		{
			var service = new MailSendLogService();
			var model = new MailSendLogModel
			{
				UserId = userId,
				DeptId = "",
				MailtextId = "",
				MailtextName = "",
				MaildistId = "",
				MaildistName = "",
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				MailId = "",
				MailName = "",
				MailFromName = message.From.DisplayName,
				MailFrom = message.From.Address,
				MailTo = string.Join(",", message.To.Select(m => m.Address)),
				MailCc = string.Join(",", message.CC.Select(m => m.Address)),
				MailBcc = string.Join(",", message.Bcc.Select(m => m.Address)),
				MailSubject = message.Subject,
				MailBody = message.Body,
				MailBodyHtml = "",
				ErrorMessage = (this.MailSendException != null) ? this.MailSendException.ToString() : "",
				DateSendMail = DateTime.Now,
				ReadFlg = Constants.FLG_MAILSENDLOG_READ_FLG_UNREAD
			};
			service.Insert(model);
		}

		if (result)
		{
			// 添付フォルダ削除
			if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);

			// メールデータセット
			sendMail.EX_MailDataModel = new CsMessageMailDataModel();
			sendMail.EX_MailDataModel.DeptId = this.LoginOperatorDeptId;
			sendMail.EX_MailDataModel.MailData = sender.GetSmtpClient().MailData.ToString();
		}

		return result;
	}
	#endregion

	#region #DrawTextLineThrough 打消し線記入
	/// <summary>
	/// 打消し線記入
	/// </summary>
	/// <param name="row">行モデル</param>
	/// <returns>結果</returns>
	protected bool DrawTextLineThrough(object row)
	{
		var result = false;
		if (row is ReportMatrixRowModelForCsWorkflow)
		{
			var castRow = (ReportMatrixRowModelForCsWorkflow)row;
			result = ((string.IsNullOrEmpty(castRow.Name) || (castRow.Valid)));
		}

		if (row is ReportRowModel)
		{
			var castRow = (ReportRowModel)row;
			result = ((string.IsNullOrEmpty(castRow.Name) || (castRow.Valid)));
		}
		return result;
	}
	#endregion

	#region -CreateTempFile テンポラリファイル作成
	/// <summary>
	/// テンポラリファイル作成
	/// </summary>
	/// <param name="attachemnts">添付ファイル</param>
	/// <param name="tempPath">テンポラリ作成パス</param>
	/// <returns>テンポラリファイルパスリスト</returns>
	private List<string> CreateTempFile(CsMessageMailAttachmentModel[] attachemnts, out string tempPath)
	{
		tempPath = Path.GetTempPath() + Path.GetRandomFileName() + @"\";
		Directory.CreateDirectory(tempPath);

		var attachmentService = new CsMessageMailAttachmentService(new CsMessageMailAttachmentRepository());

		List<string> list = new List<string>();
		foreach (var attachemnt in attachemnts)
		{
			var model = attachmentService.Get(this.LoginOperatorDeptId, attachemnt.MailId, attachemnt.FileNo);
			string tempFilePath = tempPath + attachemnt.FileName;
			File.WriteAllBytes(tempFilePath, model.FileData);

			list.Add(tempFilePath);
		}
		return list;
	}
	#endregion

	#region +CreateUserDetailUrl ユーザ詳細URL作成
	/// <summary>
	/// ユーザ詳細URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <param name="isEditable">編集可能か</param>
	/// <param name="isPopup">メニュー表示するか</param>
	/// <returns>ユーザ詳細URL</returns>
	public string CreateUserDetailUrl(string userId, bool isEditable = true, bool isPopup = true)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT_EC).Append(isEditable ? Constants.PAGE_MANAGER_USER_CONFIRM : Constants.PAGE_MANAGER_USER_CONFIRM_POPUP);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(userId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);
		url.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(isPopup ? Constants.KBN_WINDOW_POPUP : Constants.KBN_WINDOW_DEFAULT);
		return url.ToString();
	}
	#endregion

	#region +CreateOrderDetailUrl 受注詳細詳細URL作成
	/// <summary>
	/// 受注詳細詳細URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="isPopUpAction">ポップアップさせるかどうか</param>
	/// <param name="reloadParent">更新時に親ウィンドウをリロードするかどうか</param>
	/// <param name="popupParantName">ポップアップ元ページ（遷移元のリロード判定に利用）</param>
	/// <returns>注文情報詳細URL</returns>
	public string CreateOrderDetailUrl(string orderId, bool isPopUpAction, bool reloadParent, string popupParantName)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT_EC).Append(Constants.PAGE_MANAGER_ORDER_CONFIRM);
		url.Append("?").Append(Constants.REQUEST_KEY_ORDER_ID).Append("=").Append(HttpUtility.UrlEncode(orderId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));
		if (isPopUpAction)
		{
			url.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));
		}
		if (reloadParent)
		{
			url.Append("&").Append(Constants.REQUEST_KEY_RELOAD_PARENT_WINDOW).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_RELOAD_PARENT_WINDOW_ON));
		}
		if (string.IsNullOrEmpty(popupParantName) == false)
		{
			url.Append("&").Append(Constants.REQUEST_KEY_MANAGER_POPUP_PARENT_NAME).Append("=").Append(HttpUtility.UrlEncode(popupParantName));
		}
		return url.ToString();
	}
	#endregion

	#region +CreateFixedPurchaseDetailUrl 定期情報詳細URL作成
	/// <summary>
	/// 定期情報詳細URL作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="isPopUpAction">ポップアップさせるかどうか</param>
	/// <param name="reloadParent">更新時に親ウィンドウをリロードするかどうか</param>
	/// <returns>定期情報詳細URL</returns>
	public string CreateFixedPurchaseDetailUrl(string fixedPurchaseId, bool isPopUpAction, bool reloadParent)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT_EC).Append(Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM);
		url.Append("?").Append(Constants.REQUEST_KEY_FIXED_PURCHASE_ID).Append("=").Append(HttpUtility.UrlEncode(fixedPurchaseId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));
		if (isPopUpAction)
		{
			url.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));
		}
		if (reloadParent)
		{
			url.Append("&").Append(Constants.REQUEST_KEY_RELOAD_PARENT_WINDOW).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_RELOAD_PARENT_WINDOW_ON));
		}

		return url.ToString();
	}
	#endregion

	#region -CheckMailAddrInputs メールアドレス入力チェック（カンマ区切り複数可能）
	/// <summary>
	/// メールアドレス入力チェック
	/// </summary>
	/// <param name="mailInputs">メール入力（カンマ区切り複数OK）</param>
	/// <returns>エラーメッセージ</returns>
	public static string CheckMailAddrInputs(string mailInputs)
	{
		var errors = mailInputs.Split(',').Select(input => CheckMailAddrInput(input.Trim())).ToArray();
		return string.Join(" ", errors).Trim();
	}
	#endregion

	#region -CheckMailAddrInput メールアドレス入力チェック
	/// <summary>
	/// メールアドレス入力チェック
	/// </summary>
	/// <param name="mailInputs">メール入力</param>
	/// <returns>エラーメッセージ</returns>
	private static string CheckMailAddrInput(string mailInput)
	{
		Match match = Regex.Match(mailInput.Trim(), "<.*>");
		string mailAddr = (match.Success) ? match.Value.Replace("<", "").Replace(">", "").Trim() : mailInput.Trim();
		var errorMessage = Validator.CheckMailAddressError(string.Format("\"{0}\" ", mailAddr), mailAddr);

		return string.IsNullOrEmpty(errorMessage) ? string.Empty : errorMessage + "<br />";
	}
	#endregion

	#region +PagerEventArgs 内部クラス：ページャイベントクラス
	/// <summary>
	/// 内部クラス：ページャイベントクラス
	/// </summary>
	public class PagerEventArgs : EventArgs
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pageNo"></param>
		public PagerEventArgs(int pageNo)
			: base()
		{
			this.PageNo = pageNo;
		}

		/// <summary>ページNO</summary>
		public int PageNo;
	}
	#endregion

	#region UserSortAction
	/// <summary>
	/// User sort action
	/// </summary>
	/// <param name="userSearch">User search</param>
	/// <param name="sortType">Sort type</param>
	/// <paparam name="sortField">Sort field</paparam>
	/// <param name="currentSortIconPosition">Current sort icon position</param>
	/// <param name="source">Source</param>
	public void SortAction(DataView userSearch, string sortType, string sortField, Label currentSortIconPosition, Repeater source)
	{
		userSearch.Sort = string.Format("{0} {1}", sortField, sortType);
		currentSortIconPosition.Visible = true;
		currentSortIconPosition.Text = (sortType == Constants.FLG_SORT_KBN_ASC) ? Constants.FLG_SORT_SYMBOL_ASC : Constants.FLG_SORT_SYMBOL_DESC;
		source.DataSource = userSearch;
		source.DataBind();
	}
	#endregion

	#region For Sort
	/// <summary>
	/// Get Sort From Sort Kbn
	/// </summary>
	/// <param name="sortKbnOld">Sort Kbn Old</param>
	/// <param name="sortKbnCurrent">Sort Kbn Current</param>
	/// <param name="sortOld">Sort Old</param>
	/// <returns>Sort</returns>
	public static string GetSortFromSortKbn(string sortKbnOld, string sortKbnCurrent, string sortOld)
	{
		if (sortKbnOld != sortKbnCurrent) return Constants.FLG_SORT_KBN_DESC;

		return ((sortOld == Constants.FLG_SORT_KBN_ASC) ? Constants.FLG_SORT_KBN_DESC : Constants.FLG_SORT_KBN_ASC);
	}

	/// <summary>
	/// Change Refine Task Link Sort
	/// </summary>
	/// <param name="labelSort">Label sort</param>
	/// <param name="visible">Visible</param>
	/// <param name="sort">Sort</param>
	public static void ChangeRefineTaskLinkSort(Label labelSort, bool visible, string sort)
	{
		labelSort.Visible = visible;
		labelSort.Text = ((sort == Constants.FLG_SORT_KBN_ASC) ? Constants.FLG_SORT_SYMBOL_ASC : Constants.FLG_SORT_SYMBOL_DESC);
	}
	#endregion

	#region CheckMailTransmissionDisabledString
	/// <summary>
	/// Check mail transmission disabled string
	/// </summary>
	/// <param name="mailContent">Mail content</param>
	/// <returns>Error message</returns>
	public static string CheckMailTransmissionDisabledString(string mailContent)
	{
		var included = Constants.MAIL_TRANSMISSION_DISABLED_STRINGS
			.Select(item => mailContent.Contains(item) ? item : null)
			.Where(item => (item != null))
			.ToArray();
		var errorMessage = included.Any()
			? MessageManager.GetMessages(MessageManager.INPUTCHECK_MAIL_TRANSMISSION_DISABLED_STRING)
				.Replace("@@ 1 @@", string.Join(",", included))
			: string.Empty;
		return errorMessage;
	}
	#endregion

	#region GetDispTextInvalid
	/// <summary>
	/// Get Disp Text Invalid
	/// </summary>
	/// <param name="isValid">Is valid</param>
	/// <returns>Disp Text Invalid</returns>
	public string GetDispTextInvalid(bool isValid)
	{
		var invalidName = isValid
			? string.Format("({0})", ReplaceTag("@@DispText.common_message.invalid@@"))
			: string.Empty;
		return invalidName;
	}
	#endregion

	#region GetDispTextDelete
	/// <summary>
	/// Get Disp Text Delete
	/// </summary>
	/// <param name="assignId">Assign ID</param>
	/// <returns>Disp Text Delete</returns>
	public string GetDispTextDelete(string assignId)
	{
		var deleteName = string.Format(
			"{0}({1})",
			assignId,
			ReplaceTag("@@DispText.common_message.deleted@@"));
		return deleteName;
	}
	#endregion

	#region プロパティ
	/// <summary>メール送信例外</summary>
	public Exception MailSendException { get; private set; }
	#endregion
}

#region 列挙体
/// <summary>トップページ区分</summary>
public enum TopPageKbn
{
	/// <summary>トップ（カテゴリ）</summary>
	Top,
	/// <summary>下書き</summary>
	Draft,
	/// <summary>回答済み</summary>
	Reply,
	/// <summary>承認</summary>
	Approval,
	/// <summary>承認依頼中</summary>
	ApprovalRequest,
	/// <summary>承認結果返却済み</summary>
	ApprovalResult,
	/// <summary>送信代行</summary>
	Send,
	/// <summary>送信依頼中</summary>
	SendRequest,
	/// <summary>送信結果返却済み</summary>
	SendResult,
	/// <summary>検索（インシデント）</summary>
	SearchIncident,
	/// <summary>検索（メッセージ）</summary>
	SearchMessage,
	/// <summary>ゴミ箱（インシデント）</summary>
	TrashIncident,
	/// <summary>ゴミ箱（メッセージ）</summary>
	TrashMessage,
	/// <summary>ゴミ箱（承認/送信代行）</summary>
	TrashRequest,
	/// <summary>ゴミ箱（下書き）</summary>
	TrashDraft
}
/// <summary>メールアクション種別</summary>
public enum MailActionType
{
	/// <summary>メール送信</summary>
	MailSend,
	/// <summary>承認依頼</summary>
	ApprovalRequest,
	/// <summary>メール送信依頼</summary>
	MailSendRequest,
	/// <summary>承認依頼取り下げ</summary>
	ApprovalCancel,
	/// <summary>承認OK</summary>
	ApprovalOK,
	/// <summary>承認OKからの送信</summary>
	ApprovalOKSend,
	/// <summary>承認差し戻し</summary>
	ApprovalNG,
	/// <summary>送信依頼取り下げ</summary>
	SendCancel,
	/// <summary>代理送信OK</summary>
	SendOK,
	/// <summary>送信依頼差し戻し</summary>
	SendNG
}
/// <summary>タスクステータス絞り込みモード</summary>
public enum TaskStatusRefineMode
{
	/// <summary>未完了</summary>
	Uncomplete,
	/// <summary>未対応</summary>
	None,
	/// <summary>対応中</summary>
	Active,
	/// <summary>保留</summary>
	Suspend,
	/// <summary>緊急</summary>
	Urgent,
	/// <summary>完了</summary>
	Complete,
	/// <summary>すべて</summary>
	All
}
/// <summary>タスクターゲットモード</summary>
public enum TaskTargetMode
{
	/// <summary>個人タスク</summary>
	Personal,
	/// <summary>グループタスク</summary>
	Group,
	/// <summary>すべて</summary>
	All,
	/// <summary>未割当</summary>
	Unassign,
	/// <summary>グループタスク(個別グループ、未割当)</summary>
	OneGroup,
}
/// <summary>Sort Incident Kbn</summary>
public enum SortIncidentKbn
{
	/// <summary>インシデントID</summary>
	IncidentId,
	/// <summary>タイトル</summary>
	IncidentTitle,
	/// <summary>問合せ元</summary>
	EX_InquirySource,
	/// <summary>カテゴリ</summary>
	EX_IncidentCategoryName,
	/// <summary>ステータス</summary>
	EX_StatusText,
	/// <summary>担当</summary>
	EX_CsOperatorName,
	/// <summary>受付日時</summary>
	DateLastReceived,
	/// <summary>インシデント最終送受信日時</summary>
	DateMessageLastSendReceived,
	/// <summary>更新日時</summary>
	DateChanged,
	/// <summary>グループ</summary>
	EX_CsGroupName,
}
/// <summary>Sort Message Kbn</summary>
public enum SortMessageKbn
{
	/// <summary>インシデントID</summary>
	IncidentId,
	/// <summary>件名</summary>
	InquiryTitle,
	/// <summary>差出人元</summary>
	EX_Sender,
	/// <summary>対応者/作成者</summary>
	EX_NameOperator,
	/// <summary>メッセージステータス</summary>
	EX_MessageStatusName,
	/// <summary>更新日時</summary>
	DateChanged,
	/// <summary>回答日時</summary>
	InquiryReplyDate,
	/// <summary>依頼日時</summary>
	EX_Request,
	/// <summary>作成日</summary>
	DateCreated
}
/// <summary>Sort User Search Kbn</summary>
public enum SortUserSearchKbn
{
	/// <summary>ユーザーID</summary>
	UserId,
	/// <summary>区分</summary>
	Classification,
	/// <summary>氏名</summary>
	UserName,
	/// <summary>企業名</summary>
	CompanyName,
	/// <summary>部署名</summary>
	CompanyPostName,
	/// <summary>メールアドレス</summary>
	MailAddress,
	/// <summary>電話番号</summary>
	PhoneNumber,
	/// <summary>住所</summary>
	StreetAddress,
	/// <summary>管理レベル</summary>
	ManagementLevel,
	/// <summary>登録日時</summary>
	RegisteredDate
}
/// <summary>Sort User Message Input Kbn</summary>
public enum SortUserMessageInputKbn
{
	/// <summary>ユーザーID</summary>
	UserId,
	/// <summary>氏名</summary>
	UserName,
	/// <summary>企業名</summary>
	CompanyName,
	/// <summary>部署名</summary>
	CompanyPostName,
	/// <summary>メールアドレス</summary>
	MailAddress
}
/// <summary>Sort message kbn</summary>
public enum SortMessageRightKbn
{
	/// <summary>Reply changed date</summary>
	ReplyChangedDate,
	/// <summary>Date send or receive</summary>
	DateSendOrReceive,
	/// <summary>Inquiry title</summary>
	InquiryTitle,
	/// <summary>Operator name</summary>
	OperatorName
}
#endregion
