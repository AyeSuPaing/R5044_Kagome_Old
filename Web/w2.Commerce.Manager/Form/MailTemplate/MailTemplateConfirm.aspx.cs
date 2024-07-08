/*
=========================================================================================================
  Module      : メールテンプレート完了ページ処理(MailTemplateConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Input.MailTemplate;
using w2.App.Common;
using w2.App.Common.ShopMessage;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.GlobalSMS;
using w2.Domain.MailTemplate;
using w2.Domain.MessagingAppContents;

/// <summary>
/// MailTemplateConfirm の概要の説明です。
/// </summary>
public partial class Form_MailTemplate_MailTemplateConfirm : BasePage
{
	protected MailTemplateInput m_input = new MailTemplateInput();
	/// <summary>smsテンプレート入力クラス</summary>
	protected GlobalSMSTemplateInput[] m_smsInputs = new GlobalSMSTemplateInput[] {};

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			var actionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			var mailId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAIL_TEMPLATE_ID]);
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(actionStatus, mailId);

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 更新確認？
			// 登録・コピー登録・更新画面確認？
			if ((actionStatus == Constants.ACTION_STATUS_INSERT)
				|| (actionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| (actionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (actionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
			{
				//------------------------------------------------------
				// 処理区分チェック
				//------------------------------------------------------
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

				if (SessionManager.Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO] == null)
				{
					// 一覧画面へ戻る
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST);
				}

				m_input = (MailTemplateInput)Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO];
				m_smsInputs = ((GlobalSMSTemplateInput[])Session[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO]);

				// SMSテンプレバインド
				rSmsCarrier.DataSource = m_smsInputs
					.Select(x => x.CreateModel())
					.ToArray();

				// LINE配信内容バインド
				rLineDirectContents.DataSource = this.LineDirectSendMessageContents
					.Select(input => input.CreateModel())
					.ToArray();

				this.IsSelectedLanguageCode = GetIsSelectedLanguageCode(m_input.LanguageCode);

				// 自動送信非対応のメールテンプレートは非表示
				if (AutoSendPossibleCheck(m_input.MailId) == false) trAutoSendFlg.Visible = false;
			}
			// 詳細表示？
			else if (actionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				if (Constants.GLOBAL_OPTION_ENABLE == false)
				{
					this.MailTemplateData = new MailTemplateService().Get(this.LoginOperatorShopId, mailId);
				}
				else
				{
					var requestLanguageCode = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE] ?? string.Empty;
					var requestLanguageLocaleId = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID] ?? string.Empty;

					if ((string.IsNullOrEmpty(requestLanguageCode) == false)
						&& (string.IsNullOrEmpty(requestLanguageLocaleId) == false))
					{
						this.MailTemplateData = new MailTemplateService().GetByLanguageCode(
							this.LoginOperatorShopId,
							mailId,
							requestLanguageCode,
							requestLanguageLocaleId);
					}
					else
					{
						this.MailTemplateData = new MailTemplateService().Get(this.LoginOperatorShopId, mailId);
					}
					this.MailTemplateSettingTableName = Constants.TABLE_MAILTEMPLATE;

					if (this.MailTemplateData == null)
					{
						this.MailTemplateData = new MailTemplateService().GetGlobalSetting(
							this.LoginOperatorShopId, mailId, requestLanguageCode, requestLanguageLocaleId);
						this.MailTemplateSettingTableName = Constants.TABLE_MAILTEMPLATEGLOBAL;
					}
				}

				// 該当データが有りの場合
				if (this.MailTemplateData != null)
				{
					// Hashtabe格納
					m_input = new MailTemplateInput(this.MailTemplateData);
					m_input.MailTemplateSettingTableName = this.MailTemplateSettingTableName;

					// 自動送信非対応のメールテンプレートは非表示
					if (AutoSendPossibleCheck(m_input.MailId) == false) trAutoSendFlg.Visible = false;

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						InitializeLanguageCodeList();
					}
				}
				// 該当データ無しの場合
				else
				{
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] =
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				this.IsSelectedLanguageCode = GetIsSelectedLanguageCode(
					(this.MailTemplateData != null) ? this.MailTemplateData.LanguageCode : string.Empty);

				// SMS情報
				if (Constants.GLOBAL_SMS_OPTION_ENABLED)
				{
					// SMS情報
					var smsSv = new GlobalSMSService();
					var smsTeplates = smsSv.GetSmsTemplates(this.LoginOperatorShopId, mailId);
					var carrier = ValueText.GetValueItemArray("sms_carrier", "carrier_id");

					var smsData = carrier.Select(
						x =>
						{
							return smsTeplates.FirstOrDefault(t => t.PhoneCarrier == x.Value)
								??  new GlobalSMSTemplateModel()
								{
									PhoneCarrier = x.Value,
									SmsText = ""
								};
						}).ToArray();
					m_smsInputs = smsData.Select(x => new GlobalSMSTemplateInput(x)).ToArray();
					this.rSmsCarrier.DataSource = smsData;
				}

				// LINE情報
				if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED)
				{
					var msgAppContentsSv = new MessagingAppContentsService();
					var msgAppContents = msgAppContentsSv.GetAllContentsEachMessagingAppKbn(
						this.LoginOperatorShopId,
						MessagingAppContentsModel.MASTER_KBN_MAILTEMPLATE,
						mailId,
						MessagingAppContentsModel.MESSAGING_APP_KBN_LINE);

					this.LineDirectSendMessageContents = msgAppContents.Select(x => new MessagingAppContentsInput(x)).ToList();
					rLineDirectContents.DataSource = msgAppContents;
					rLineDirectContents.DataBind();
				}
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 送信元表示
			this.EncodedDisplayMailFrom = ReplaceShopMessageTagForDisplay(
				MailSendUtility.AdjustShopMessageTagWithHavingSpace(m_input.MailFrom),
				true,
				false);

			// 送信元名表示
			this.EncodedDisplaySenderName = ReplaceShopMessageTagForDisplay(m_input.MailFromName,
				false,
				false);

			// Bcc表示
			this.EncodedDisplayMailBcc = ReplaceShopMessageTagForDisplay(
				MailSendUtility.AdjustShopMessageTagWithHavingSpace(m_input.MailBcc),
				true,
				true);

			// エラーメッセージ表示
			CheckAndDisplayErrorMessages();

			// VIewStateにも格納しておく
			ViewState.Add(Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO, m_input);
			ViewState.Add(Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO, m_smsInputs);
			ViewState.Add(Constants.SESSIONPARAM_KEY_LINETEMPLATE_INFO, this.LineDirectSendMessageContents);

			// データバインド
			DataBind();

			this.HtmlForPreviewList = new List<string>
			{
				m_input.MailTextHtml.Replace("<@@", "&lt;@@").Replace("</@@", "&lt;/@@").Replace("@@>", "@@&gt;")
			};
		}
	}

	/// <summary>
	/// 表示のために埋め込みタグを置換
	/// </summary>
	/// <param name="input">メールテンプレート項目の入力値</param>
	/// <param name="deleteSpace">空白を除くか</param>
	/// <param name="isMultiLineToolTips">複数行のツールチップか</param>
	/// <returns>メールテンプレート項目の出力値</returns>
	private string ReplaceShopMessageTagForDisplay(string input, bool deleteSpace, bool isMultiLineToolTips)
	{
		var result = WebSanitizer.HtmlEncode(input);
		var matchShopMessageTagResults = Regex.Matches(result, ShopMessageUtil.FORMAT_SHOP_MESSAGE_TAG_WITH_SPACE);
		foreach (Match m in matchShopMessageTagResults)
		{
			var replacedValue = ShopMessageUtil.ConvertShopMessage(
				m.Value,
				m_input.LanguageCode,
				m_input.LanguageLocaleId,
				false);

			if (Regex.IsMatch(replacedValue, ShopMessageUtil.FORMAT_SHOP_MESSAGE_TAG_WITH_SPACE) == false)
			{
				result = result.Replace(m.Value, CreateUnderLineWithTitle(replacedValue, deleteSpace, m.Value, isMultiLineToolTips));
			}
		}

		return result;
	}

	/// <summary>
	/// 下線とツールチップを追加
	/// </summary>
	/// <param name="replacedValue">置換された値</param>
	/// <param name="deleteSpace">空白を除くか</param>
	/// <param name="shopMessageTag">ショップメッセージタグ</param>
	/// <param name="isMultiLineToolTips">複数行のツールチップか</param>
	/// <returns>下線とツールチップが追加された値</returns>
	private string CreateUnderLineWithTitle(string replacedValue, bool deleteSpace, string shopMessageTag, bool isMultiLineToolTips)
	{
		replacedValue = deleteSpace ? replacedValue.TrimAllSpaces() : replacedValue;
		var result = string.Format(
			"<u title = \"{0}\">{1}</u>",
			isMultiLineToolTips ? (string.Join("\n", replacedValue.Split(','))) : replacedValue,
			shopMessageTag);
		return result;
	}

	/// <summary>
	/// 埋め込みタグを置換
	/// </summary>
	/// <param name="input">置換前の値</param>
	/// <returns>置換後の値</returns>
	private string ReplaceShopMessageTag(string input)
	{
		var result = ShopMessageUtil.ConvertShopMessage(
			MailSendUtility.AdjustShopMessageTagWithHavingSpace(input),
			m_input.LanguageCode,
			m_input.LanguageLocaleId,
			false);
		return result;
	}

	/// <summary>
	/// 置換できない埋め込みタグを確認
	/// </summary>
	/// <param name="input">置換後の値</param>
	/// <returns>置換できない埋め込みタグが存在するか</returns>
	private bool CheckShopMessageTagRemain(string input)
	{
		var result = Regex.IsMatch(input, ShopMessageUtil.FORMAT_SHOP_MESSAGE_TAG_WITH_SPACE);
		return result;
	}

	/// <summary>
	/// メールアドレス形式と送信元名を確認
	/// </summary>
	/// <param name="value">確認項目の値</param>
	/// <param name="hasCheckMail">メールアドレス形式検証必要か</param>
	/// <param name="isBcc">BCCメールアドレスか</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckMailAddressAndSenderMailName(string value, bool hasCheckMail = true, bool isBcc = false)
	{
		string errorMessage;
		// 置換できない埋め込みタグを確認
		if (CheckShopMessageTagRemain(value))
		{
			errorMessage = HtmlSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MAILTEMPLATE_SHOP_MESSAGE_TAG_NOT_EXIST));
			return errorMessage;
		}

		if (hasCheckMail == false) return string.Empty;

		value = value.TrimAllSpaces();
		errorMessage = isBcc ? Validator.CheckMailAddrInputs("", value) : Validator.CheckMailAddrInput("", value);

		errorMessage = (((isBcc == false) && string.IsNullOrEmpty(value)) || (string.IsNullOrEmpty(errorMessage) == false))
			? HtmlSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MAILTEMPLATE_NOT_MAIL_ADDRESS_FORMAT))
			: string.Empty;
		return errorMessage;
	}

	/// <summary>
	/// 検証結果の確認とエラーメッセージを表示
	/// </summary>
	private void CheckAndDisplayErrorMessages()
	{
		// 送信元メールアドレスの検証とエラーメッセージの表示
		lbMailFromErrorMessage.Text = CheckMailAddressAndSenderMailName(ReplaceShopMessageTag(m_input.MailFrom));
		lbMailFromErrorMessage.Visible = (string.IsNullOrEmpty(lbMailFromErrorMessage.Text) == false);

		// 送信元名の検証とエラーメッセージの表示
		lbMailFromNameErrorMessage.Text = CheckMailAddressAndSenderMailName(
			ReplaceShopMessageTag(m_input.MailFromName),
			false);
		lbMailFromNameErrorMessage.Visible = (string.IsNullOrEmpty(lbMailFromNameErrorMessage.Text) == false);

		// BCCメールアドレスの検証とエラーメッセージの表示
		lbMailBccErrorMessage.Text = CheckMailAddressAndSenderMailName(
			ReplaceShopMessageTag(m_input.MailBcc),
			isBcc: true);
		lbMailBccErrorMessage.Visible = (string.IsNullOrEmpty(lbMailBccErrorMessage.Text) == false);
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	/// <param name="mailId">メールテンプレートID</param>
	private void InitializeComponents(string actionStatus, string mailId)
	{
		// 新規・コピー新規・グローバル設定登録？
		if ((actionStatus == Constants.ACTION_STATUS_INSERT) ||
			(actionStatus == Constants.ACTION_STATUS_COPY_INSERT) ||
			(actionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
		{
			btnGoBackTop.Visible = true;
			btnGoBackBottom.Visible = true;
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trConfirm.Visible = true;

			if (actionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
			{
				trMailId.Visible = true;
				trMobileSetting.Visible = false;
			}
		}
		// 更新？
		else if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnGoBackTop.Visible = true;
			btnGoBackBottom.Visible = true;
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trConfirm.Visible = true;
			trMailId.Visible = true;
		}
		// 詳細
		else if (actionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnBackToListTop.Visible = true;
			btnBackToListBottom.Visible = true;
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnCopyInsertTop.Visible = true;
			btnCopyInsertBottom.Visible = true;
			trMailId.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetail.Visible = true;

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				btnInsertGlobalTop.Visible = true;
				btnInsertGlobalBottom.Visible = true;
				ddlLanguageCode.Visible = true;
			}

			// システム固定のメールテンプレート（IDの頭文字が"0" OR "1"）の場合、削除ボタンを非表示にする
			bool blDeletable = true;
			if (mailId.StartsWith("0") || mailId.StartsWith("1"))
			{
				blDeletable = false;
			}
			btnDeleteTop.Visible = blDeletable;
			btnDeleteBottom.Visible = blDeletable;
		}
	}

	/// <summary>
	/// メールテンプレートの自動送信可能か確認
	/// </summary>
	/// <param name="mailId">メールテンプレートID</param>
	/// <returns>自動送信可能か判断</returns>
	private bool AutoSendPossibleCheck(string mailId)
	{
		var autoSendPossible = Constants.AUTOSEND_MAIL_ID_LIST.Any(autoSendMailId => (mailId == autoSendMailId));
		return autoSendPossible;
	}

	/// <summary>
	/// 指定メールが予約メールIDかチェック
	/// </summary>
	/// <param name="mailId">メールテンプレートID</param>
	/// <returns>ユーザー任意メールテンプレートID:true システム用メールテンプレートID：false</returns>
	private bool IsUnReservedMailId(string mailId)
	{
		var result = mailId.Substring(0, 1) == Constants.CONST_MAIL_ID_INITIAL_UNRESERVED;

		return result;
	}

	#region #btnBack_Click 一覧へ戻るボタンクリック
	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackToListTop_Click(object sender, System.EventArgs e)
	{
		// メールテンプレート設定一覧へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST);
	}
	#endregion

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// メールテンプレート情報をそのままセッションへセット
		Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO] = ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO];
		Session[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO] = ViewState[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO];
		this.LineDirectSendMessageContents = (List<MessagingAppContentsInput>)ViewState[Constants.SESSIONPARAM_KEY_LINETEMPLATE_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}


	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// メールテンプレート情報をそのままセッションへセット
		Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO] = ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO];
		// SMSテンプレート
		Session[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO] = ViewState[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO];
		// メッセージ送信内容テンプレート
		this.LineDirectSendMessageContents = (List<MessagingAppContentsInput>)ViewState[Constants.SESSIONPARAM_KEY_LINETEMPLATE_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}


	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// メールテンプレート情報取得
		var input = (MailTemplateInput)ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO];

		// Is Used Mail Id
		var isUsedMailId = CheckUsedMailId(input.MailId);

		// ユーザー任意のメールテンプレートIDの場合
		if ((IsUnReservedMailId(input.MailId)) && (isUsedMailId == false))
		{
			// 削除
			var service = new MailTemplateService();
			service.Delete(this.LoginOperatorShopId, input.MailId);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				service.DeleteGlobalSetting(this.LoginOperatorShopId, input.MailId);
			}

			if (Constants.GLOBAL_SMS_OPTION_ENABLED)
			{
				var sv = new GlobalSMSService();
				sv.DeleteSmsTemplates(this.LoginOperatorShopId, input.MailId);
			}

			if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED)
			{
				new MessagingAppContentsService().DeleteAllContentsEachMasterId(
					this.LoginOperatorShopId,
					MessagingAppContentsModel.MASTER_KBN_MAILTEMPLATE,
					input.MailId);
		}
		}
		else
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(isUsedMailId
				? WebMessages.ERRMSG_MANAGER_MAILTEMPLATE_MAIL_ID_USED
				: WebMessages.ERRMSG_MANAGER_MAILTEMPLATE_RESERVED_MAIL_ID_ERROR);

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST);
	}

	/// <summary>
	/// 指定言語だけ削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteGlobal_Click(object sender, System.EventArgs e)
	{
		// メールテンプレート情報取得
		var model = ((MailTemplateInput)ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO]).CreateModel();

		// Is Used Mail Id
		var isUsedMailId = CheckUsedMailId(model.MailId);

		// ユーザー任意のメールテンプレートIDの場合
		if ((IsUnReservedMailId(model.MailId)) && (isUsedMailId == false))
		{
			// 削除
			var service = new MailTemplateService();
			service.DeleteSpecifiedLanguageSetting(model);
		}
		else
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(isUsedMailId
				? WebMessages.ERRMSG_MANAGER_MAILTEMPLATE_MAIL_ID_USED
				: WebMessages.ERRMSG_MANAGER_MAILTEMPLATE_RESERVED_MAIL_ID_ERROR);

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		SessionManager.BeforeCopyMailId = null;
		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 頭文字が9(ユーザー作成可能メールテンプレートID)の新規メールテンプレートID取得
		var newMailTemplateId = string.Format("9{0}", NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_MAILTEMPLATE_ID, 7));
		var mailTemplate = ((MailTemplateInput)ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO]).CreateModel();

		// 登録
		var actionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
		if ((actionStatus == Constants.ACTION_STATUS_INSERT) || (actionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			mailTemplate.MailId = newMailTemplateId;
			if (mailTemplate.AutoSendFlg == "") mailTemplate.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_NOTSEND;

			new MailTemplateService().Insert(mailTemplate);
		}
		else
		{
			// グローバル設定登録
			RegisterGlobalSetting(mailTemplate);
		}

		if (Constants.GLOBAL_SMS_OPTION_ENABLED && (mailTemplate.SmsUseFlg == MailTemplateModel.SMS_USE_FLG_ON))
		{
			var smsTemplate = ((GlobalSMSTemplateInput[])ViewState[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO])
				.Select(
					x =>
					{
						x.MailId = newMailTemplateId;
						return x.CreateModel();
					}).ToArray();

			var sv = new GlobalSMSService();

			foreach (var smsModel in smsTemplate)
			{
				// UPSERT
				sv.UpsertSMSTemplate(smsModel);
			}
		}

		// LINE情報
		if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && (mailTemplate.LineUseFlg == MailTemplateModel.LINE_USE_FLG_ON))
		{
			var msgAppContents = ((List<MessagingAppContentsInput>)ViewState[Constants.SESSIONPARAM_KEY_LINETEMPLATE_INFO])
				.Select(
					x =>
					{
						x.MasterId = newMailTemplateId;
						return x.CreateModel();
					}).ToArray();

			var sv = new MessagingAppContentsService();
			foreach (var smcModel in msgAppContents)
			{
				sv.UpsertContents(smcModel);
			}
		}

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST);
	}


	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 更新
		var mailTemplate = ((MailTemplateInput)ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO]).CreateModel();

		if ((Constants.GLOBAL_OPTION_ENABLE == false)
			|| (((MailTemplateInput)ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO]).MailTemplateSettingTableName == Constants.TABLE_MAILTEMPLATE))
		{
			new MailTemplateService().Update(mailTemplate);
		}
		else
		{
			new MailTemplateService().UpdateGlobalSetting(mailTemplate);
		}

		// 他言語のメールテンプレートを修正する時は、下の「SMS」と「LINE」情報を更新しない
		this.IsSelectedLanguageCode = GetIsSelectedLanguageCode(mailTemplate.LanguageCode);
		if (this.IsSelectedLanguageCode)
		{
			// 一覧画面へ戻る
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST);
		}

		if (Constants.GLOBAL_SMS_OPTION_ENABLED && (mailTemplate.SmsUseFlg == MailTemplateModel.SMS_USE_FLG_ON))
		{
			var smsTemplate = ((GlobalSMSTemplateInput[])ViewState[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO])
				.Select(
					x =>
					{
						x.MailId = mailTemplate.MailId;
						return x.CreateModel();
					}).ToArray();

			var sv = new GlobalSMSService();

			foreach (var smsModel in smsTemplate)
			{
				// UPSERT
				sv.UpsertSMSTemplate(smsModel);
			}
		}
		else if (Constants.GLOBAL_SMS_OPTION_ENABLED && (mailTemplate.SmsUseFlg == MailTemplateModel.SMS_USE_FLG_OFF))
		{
			var sv = new GlobalSMSService();
			sv.DeleteSmsTemplates(this.LoginOperatorShopId, mailTemplate.MailId);
		}

		if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && (mailTemplate.LineUseFlg == MailTemplateModel.LINE_USE_FLG_ON))
		{
			var msgAppContents = ((List<MessagingAppContentsInput>)ViewState[Constants.SESSIONPARAM_KEY_LINETEMPLATE_INFO])
				.Select(
					x =>
					{
						x.MasterId = mailTemplate.MailId;
						return x.CreateModel();
					}).ToArray();

			var sv = new MessagingAppContentsService();
			foreach (var smcModel in msgAppContents)
			{
				sv.UpsertContents(smcModel);
			}

			// 登録されている不要な送信内容を削除
			if (msgAppContents.Length < w2.App.Common.Line.Constants.LINE_DIRECT_MAX_MESSAGE_COUNT)
			{
				for (var i = (msgAppContents.Length + 1); i <= w2.App.Common.Line.Constants.LINE_DIRECT_MAX_MESSAGE_COUNT; i++)
				{
					sv.DeleteContentsEachBranchNo(
						msgAppContents[0].DeptId,
						msgAppContents[0].MasterKbn,
						msgAppContents[0].MasterId,
						msgAppContents[0].MessagingAppKbn,
						i.ToString());
				}
			}
		}
		else if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && (mailTemplate.LineUseFlg == MailTemplateModel.LINE_USE_FLG_OFF))
		{
			new MessagingAppContentsService().DeleteAllContentsEachMasterId(
				this.LoginOperatorShopId,
				MessagingAppContentsModel.MASTER_KBN_MAILTEMPLATE,
				mailTemplate.MailId);
		}

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST);
	}

	/// <summary>
	/// Check Exist Mail TemplateId used at OrderWorkflow
	/// </summary>
	/// <param name="mailTemplateId">Mail Template Id</param>
	/// <returns>true: exist mail template false: Not exist mail template</returns>
	private bool CheckUsedMailId(string mailTemplateId)
	{
		// Order Extend Status Change
		var orderExtendStatusChange = new StringBuilder();
		foreach(DataRowView row in GetOrderExtendStatusSettingList(this.LoginOperatorShopId))
		{
			orderExtendStatusChange.Append(string.Format("OR  w2_OrderWorkflowSetting.cassette_order_extend_status_change{0} like '%{1}%'",
				StringUtility.ToEmpty(row[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO]),
				mailTemplateId));
		}

		// Check Used Mail Id
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("OrderWorkflowSetting", "CheckUsedMailId"))
		{
			var input = new Hashtable();
			input.Add(Constants.FIELD_MAILTEMPLATE_SHOP_ID, this.LoginOperatorShopId);
			input.Add(Constants.FIELD_MAILTEMPLATE_MAIL_ID, mailTemplateId);

			statement.Statement = statement.Statement.Replace("@@ cassette_order_extend_status_change @@", orderExtendStatusChange.ToString());

			return (statement.SelectSingleStatementWithOC(accessor, input).Count > 0);
		}
	}

	/// <summary>
	/// 他言語コードで登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertGlobal_Click(object sender, EventArgs e)
	{
		// 大元のメールテンプレート情報を取得してセッションへセット
		var mailTemplateData = new MailTemplateService().Get(this.LoginOperatorShopId, this.MailTemplateData.MailId);
		var input = new MailTemplateInput(mailTemplateData);
		Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO] = input;
		// SMSテンプレート
		Session[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO] = ViewState[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT;

		// グローバル設定登録画面へ
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT);
		Response.Redirect(urlCreator.CreateUrl());
	}

	/// <summary>
	/// グローバル設定登録
	/// </summary>
	/// <param name="mailTemplate">メールテンプレートモデル</param>
	private void RegisterGlobalSetting(MailTemplateModel mailTemplate)
	{
		if (mailTemplate.AutoSendFlg == "") mailTemplate.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_NOTSEND;
		new MailTemplateService().InsertGlobalSetting(mailTemplate);
	}

	/// <summary>
	/// 言語コードリスト初期化
	/// </summary>
	private void InitializeLanguageCodeList()
	{
		var settings =
			new MailTemplateService().GetMailTemplateContainGlobalSetting(this.MailTemplateData.ShopId, this.MailTemplateData.MailId);

		// 「言語コード/言語ロケールID」の形式でドロップダウンリストを生成する
		var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");
		wddlLanguageCode.AddItems(settings.OrderBy(setting => setting.LanguageCode)
			.Select(setting =>
				new ListItem(
					(string.IsNullOrEmpty(setting.LanguageCode) == false)
						? string.Format("{0}({1})", setting.LanguageCode, setting.LanguageLocaleId)
						: ValueText.GetValueText(Constants.TABLE_MAILTEMPLATE, Constants.FIELD_MAILTEMPLATE_LANGUAGE_CODE, string.Empty)	// 「指定しない」を表示
					, string.Format("{0}/{1}", setting.LanguageCode, setting.LanguageLocaleId))).ToArray());

		var languageCode = this.MailTemplateData.LanguageCode ?? string.Empty;
		var languageLocaleId = this.MailTemplateData.LanguageLocaleId ?? string.Empty;
		wddlLanguageCode.SelectItemByValue(string.Format("{0}/{1}", languageCode, languageLocaleId));
	}

	/// <summary>
	/// 言語コードドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlLanguageCode_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedItem = ((DropDownList)sender).SelectedItem;
		var selectedValueSplit = selectedItem.Value.Split('/');

		var languageCode = selectedValueSplit[0];
		var languageLocaleId = selectedValueSplit[1];

		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_MAIL_TEMPLATE_ID, this.MailTemplateData.MailId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL);

		if ((string.IsNullOrEmpty(languageCode) == false) && (string.IsNullOrEmpty(languageLocaleId) == false))
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE, languageCode)
				.AddParam(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID, languageLocaleId);
		}
		Response.Redirect(urlCreator.CreateUrl());
	}

	/// <summary>
	/// グローバル設定か
	/// </summary>
	/// <param name="model">メールテンプレートモデル</param>
	protected bool isGlobalSetting(MailTemplateModel model)
	{
		var globalSetting = new MailTemplateService().GetGlobalSetting(model.ShopId, model.MailId, model.LanguageCode, model.LanguageLocaleId);

		return (globalSetting != null);
	}

	/// <summary>
	/// メールの使用用途取得
	/// </summary>
	/// <returns>メールの使用用途</returns>
	protected string GetEmailUsageCategory()
	{
		var index = 0;
		switch (m_input.MailCategory)
		{
			case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_ORDER:
				index = Constants.KBN_EMAIL_USAGE_CATEGORY_ORDER;
				break;

			case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_FIXEDPURCHASE:
				index = Constants.KBN_EMAIL_USAGE_CATEGORY_FIXEDPURCHASE;
				break;

			case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_USER:
				index = Constants.KBN_EMAIL_USAGE_CATEGORY_USER;
				break;

			case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_TOREALSHOP:
				index = Constants.KBN_EMAIL_USAGE_CATEGORY_TOREALSHOP;
				break;

			case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_FROMREALSHOP:
				index = Constants.KBN_EMAIL_USAGE_CATEGORY_FROMREALSHOP;
				break;
		}
		return ValueText.GetValueKvpArray(
			Constants.VALUETEXT_PARAM_MAIL_TEMPLATE_SETTING_LIST,
			Constants.VALUETEXT_PARAM_USE_APPLICATIONS)[index].Value;
	}

	/// <summary>
	/// グローバル言語コードが指定されているか判断フラグを取得
	/// </summary>
	/// <param name="languageCode">言語コード</param>
	/// <returns>グローバル言語コードが指定されているか</returns>
	private bool GetIsSelectedLanguageCode(string languageCode)
	{
		var result = (Constants.GLOBAL_OPTION_ENABLE && (string.IsNullOrEmpty(languageCode) == false));
		return result;
	}

	/// <summary>メールテンプレートデータ</summary>
	private MailTemplateModel MailTemplateData
	{
		get { return (MailTemplateModel)ViewState["mail_template_data"]; }
		set { ViewState["mail_template_data"] = value; }
	}
	/// <summary>メールテンプレート設定テーブル名(w2_MailTemplate/w2_MailTemplateGlobal)</summary>
	protected string MailTemplateSettingTableName
	{
		get { return (string)ViewState[Constants.SETTING_TABLE_NAME]; }
		set { ViewState[Constants.SETTING_TABLE_NAME] = value; }
	}
	/// <summary>グローバル言語コードが指定されているかの判断フラグ</summary>
	protected bool IsSelectedLanguageCode { get; set; }

	/// <summary>LINEフォーム表示フラグ</summary>
	protected bool LineDispFlg
	{
		get
		{
			return ((m_input.MailId == Constants.CONST_MAIL_ID_ORDER_COMPLETE)
				&& (this.IsSelectedLanguageCode == false));
		}
	}

	/// <summary> LINE直接連携送信内容 </summary>
	protected List<MessagingAppContentsInput> LineDirectSendMessageContents
	{
		get
		{
			if (SessionManager.Session[Constants.SESSION_KEY_MAILTEMPLATE_LINE_CONTENTS] == null)
			{
				SessionManager.Session[Constants.SESSION_KEY_MAILTEMPLATE_LINE_CONTENTS] = new List<MessagingAppContentsInput>();
			}
			return (List<MessagingAppContentsInput>)SessionManager.Session[Constants.SESSION_KEY_MAILTEMPLATE_LINE_CONTENTS];
		}
		set { SessionManager.Session[Constants.SESSION_KEY_MAILTEMPLATE_LINE_CONTENTS] = value; }
	}
	/// <summary>メールFrom表示用</summary>
	protected string EncodedDisplayMailFrom { get; set; }
	/// <summary>送信元名表示用</summary>
	protected string EncodedDisplaySenderName { get; set; }
	/// <summary>メールBcc表示用</summary>
	protected string EncodedDisplayMailBcc { get; set; }
}
