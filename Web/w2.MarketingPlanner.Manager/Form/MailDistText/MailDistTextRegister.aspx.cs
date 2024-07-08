/*
=========================================================================================================
  Module      : メール配信文章登録ページ処理(MailDistTextRegister.aspx.cs)
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
using System.Web.UI.WebControls;
using Input.MailDist;
using w2.App.Common.Web;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Domain.GlobalSMS;
using w2.Domain.MailDist;
using w2.Domain.MessagingAppContents;

public partial class Form_MailDistText_MailDistTextRegister : DecomeBasePage
{
	public string m_strActionStatus = null;
	string m_strMailTextId = null;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			m_strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState[Constants.REQUEST_KEY_ACTION_STATUS] = m_strActionStatus;

			m_strMailTextId = Request[Constants.REQUEST_KEY_MAILTEXT_ID];
			ViewState[Constants.REQUEST_KEY_MAILTEXT_ID] = m_strMailTextId;

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(m_strActionStatus);

			//------------------------------------------------------
			// 表示用値設定処理Constants.ACTION_STATUS_COPY_INSERT
			//------------------------------------------------------
			// 新規？
			switch (m_strActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:

					this.rSmsCarrier.DataSource = ValueText.GetValueItemArray("sms_carrier", "carrier_id").Select(
						x => new GlobalSMSDistTextModel()
						{
							PhoneCarrier = x.Value,
							SmsText = ""
						}).ToArray();

					this.rSmsCarrier.DataBind();

					if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) LineDirectContentsDataBind(true);
					break;

				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT:

					DataView dvMailDistText = null;
					
					if ((Constants.GLOBAL_OPTION_ENABLE == false || (m_strActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)))
					{
						dvMailDistText = GetMailDistText();
					}
					else
					{
						this.LanguageCode = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE];
						var languageLocaleId = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID];

						if ((string.IsNullOrEmpty(this.LanguageCode) == false)
							&& (string.IsNullOrEmpty(languageLocaleId) == false))
						{
							dvMailDistText = new MailDistService().GetTextDataViewByLanguageCode(
								this.LoginOperatorDeptId,
								m_strMailTextId,
								this.LanguageCode,
								languageLocaleId);
							this.MailDistTextSettingTableName = Constants.TABLE_MAILDISTTEXT;

							if (dvMailDistText.Count == 0)
							{
								dvMailDistText = new MailDistService().GetTextGlobalSettingDataView(
									this.LoginOperatorDeptId,
									m_strMailTextId,
									this.LanguageCode,
									languageLocaleId);
								this.MailDistTextSettingTableName = Constants.TABLE_MAILDISTTEXTGLOBAL;
							}
						}
						else
						{
							dvMailDistText = GetMailDistText();
							this.MailDistTextSettingTableName = Constants.TABLE_MAILDISTTEXT;
						}
					}

					// 該当データが有りの場合
					if (dvMailDistText.Count != 0)
					{
						if (m_strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
						{
							m_strMailTextId = "";
							ViewState[Constants.REQUEST_KEY_MAILTEXT_ID] = m_strMailTextId;
						}

						DataRowView drvMailDistText = dvMailDistText[0];
						lMailtextId.Text = WebSanitizer.HtmlEncode(m_strMailTextId);
						tbMailtextName.Text = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME];
						lMailtextName.Text =
							WebSanitizer.HtmlEncode(drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME]);
						tbMailFromName.Text = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_FROM_NAME];
						tbMailFrom.Text = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_FROM];
						tbMailtextSubject.Text = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT];
						tbMailtextBody.Text = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY];
						tbMailtextHtml.Text = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML];
						chksms.Checked = (StringUtility.ToEmpty(drvMailDistText[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG]) == MailDistTextModel.SMS_USE_FLG_ON);
						chkline.Checked = (StringUtility.ToEmpty(drvMailDistText.ToHashtable()[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]) == MailDistTextModel.LINE_USE_FLG_ON);

						foreach (ListItem li in rblSendHtmlFlg.Items)
						{
							if (tbMailtextHtml.Text.Length == 0)
							{
								li.Selected = (li.Value == Constants.FLG_MAILDISTTEXT_SENDHTML_FLG_INVALID);
							}
							else
							{
								li.Selected = (li.Value == Constants.FLG_MAILDISTTEXT_SENDHTML_FLG_VALID);
							}
						}

						if ((Constants.GLOBAL_OPTION_ENABLE == false) || (this.MailDistTextSettingTableName == Constants.TABLE_MAILDISTTEXT))
						{
							tbMailtextSubjectMobile.Text = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE];
							tbMailtextMobile.Text = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE];
						}

						if (Constants.GLOBAL_OPTION_ENABLE)
						{
							var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");
							this.LanguageCode = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE];
							var languageLocaleId = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID];
							wddlLanguageCode.SelectItemByValue(string.Format("{0}/{1}", this.LanguageCode, languageLocaleId));

							divMobileSetting.Visible = (this.MailDistTextSettingTableName == Constants.TABLE_MAILDISTTEXT);
						}

						// SMS情報
						if (Constants.GLOBAL_SMS_OPTION_ENABLED)
						{
							// SMS情報
							var smsSv = new GlobalSMSService();
							var smsTeplates = smsSv.GetSmsDistTexts(this.LoginOperatorDeptId, StringUtility.ToEmpty(dvMailDistText[0][Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_ID]));
							var carrier = ValueText.GetValueItemArray("sms_carrier", "carrier_id");

							var smsData = carrier.Select(
								x =>
								{
									return smsTeplates.FirstOrDefault(t => t.PhoneCarrier == x.Value)
										?? new GlobalSMSDistTextModel()
										{
											PhoneCarrier = x.Value,
											SmsText = ""
										};
								}).ToArray();
							this.rSmsCarrier.DataSource = smsData;
							if (chksms.Checked)
							{
								this.rSmsCarrier.Visible = true;
							}
							this.rSmsCarrier.DataBind();
						}

						// LINE情報
						if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) LineDirectContentsDataBind(false);
					}
					else
					{
						// 該当データ無しの場合、エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
					}

					// メール文章HTML表示制御
					DispMailDistTextHtmlForm();

					break;
			}

			DisplayControlMailTextName(m_strActionStatus);
		}
		else
		{
			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
			m_strMailTextId = (string)ViewState[Constants.REQUEST_KEY_MAILTEXT_ID];
		}

		this.IsSelectedLanguageCode = GetIsSelectedLanguageCode(m_strActionStatus);
	}

	/// <summary>
	/// メール配信文章取得
	/// </summary>
	/// <returns></returns>
	private DataView GetMailDistText()
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailDistText", "GetMailDistText"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILDISTTEXT_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, m_strMailTextId);
			var dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			return dv;
		}
	}

	/// <summary>
	/// メール文章HTML表示制御
	/// </summary>
	private void DispMailDistTextHtmlForm()
	{
		// HTMLメールテキストボックスの表示をオン・オフ
		if (rblSendHtmlFlg.SelectedValue == Constants.FLG_MAILDISTTEXT_SENDHTML_FLG_VALID)
		{
			spMailtextHtml.Style.Add("display", "inline");
		}
		else
		{
			spMailtextHtml.Style.Add("display", "none");
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		//------------------------------------------------------
		// 画面制御
		//------------------------------------------------------
		// ラジオボタン設定
		rblSendHtmlFlg.Items[0].Value = Constants.FLG_MAILDISTTEXT_SENDHTML_FLG_VALID;
		rblSendHtmlFlg.Items[1].Value = Constants.FLG_MAILDISTTEXT_SENDHTML_FLG_INVALID;

		// 項目設定
		trMailtextId.Visible = ((strActionStatus == Constants.ACTION_STATUS_UPDATE) || (strActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT));

		// 新規登録 or コピー新規登録？？
		if ((strActionStatus == Constants.ACTION_STATUS_INSERT)
			|| (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			spMailtextHtml.Style.Add("display", "none");	// デフォルト無効
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");

			// グローバル設定登録時以外は、「指定しない」をドロップダウンリストに表示する
			if (strActionStatus != Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
			{
				wddlLanguageCode.Items.Add(new ListItem(
					ValueText.GetValueText(Constants.TABLE_MAILDISTTEXT, Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE, string.Empty),
					string.Format("{0}/{1}", string.Empty, string.Empty)));
			}
			wddlLanguageCode.Items.AddRange(
				Constants.GLOBAL_CONFIGS.GlobalSettings.Languages
					.Select(c => new ListItem(string.Format("{0}({1})", c.Code, c.LocaleId), c.Code + "/" + c.LocaleId)).ToArray());

			// 更新時には、言語コード変更不可
			ddlLanguageCode.Enabled = (strActionStatus != Constants.ACTION_STATUS_UPDATE);

			// グローバル設定登録時はモバイル設定不可
			divMobileSetting.Visible = (strActionStatus != Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT);
		}
	}

	/// <summary>
	/// 確認するボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		Hashtable htInput = new Hashtable();
		htInput.Add(Constants.FIELD_MAILDISTTEXT_DEPT_ID, this.LoginOperatorDeptId);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, m_strMailTextId);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME, tbMailtextName.Text);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT, tbMailtextSubject.Text);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE, tbMailtextSubjectMobile.Text);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE + "_for_length_check", ReplaceEmojiTag(tbMailtextSubjectMobile.Text, "■"));	// 絵文字を1文字として文字数をチェックする
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY, tbMailtextBody.Text);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_SENDHTML_FLG, rblSendHtmlFlg.SelectedValue);
		if (rblSendHtmlFlg.SelectedValue == Constants.FLG_MAILDISTTEXT_SENDHTML_FLG_VALID)
		{
			htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML, tbMailtextHtml.Text);
		}
		else
		{
			htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML, "");
		}
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE, tbMailtextMobile.Text);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAIL_FROM_NAME, tbMailFromName.Text);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAIL_FROM, tbMailFrom.Text.Replace(" ", "").Replace("　", ""));
		htInput.Add(Constants.FIELD_MAILDISTTEXT_LAST_CHANGED, this.LoginOperatorName);

		var languageCode = string.Empty;
		var languageLocaleId = string.Empty;
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var ddlLanguageCodeSplitValue = ddlLanguageCode.SelectedItem.Value.Split('/');
			languageCode = ddlLanguageCodeSplitValue[0];
			languageLocaleId = ddlLanguageCodeSplitValue[1];
		}
		htInput.Add(Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE, languageCode);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID, languageLocaleId);
		htInput.Add(Constants.SETTING_TABLE_NAME, this.MailDistTextSettingTableName);

		// SMS
		htInput.Add(Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG,
			(Constants.GLOBAL_SMS_OPTION_ENABLED)
				? this.chksms.Checked ? MailDistTextModel.SMS_USE_FLG_ON : MailDistTextModel.SMS_USE_FLG_OFF
				: MailDistTextModel.SMS_USE_FLG_OFF);
		
		// LINE
		htInput.Add(Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG,
			(w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED)
				? chkline.Checked ? MailDistTextModel.LINE_USE_FLG_ON : MailDistTextModel.LINE_USE_FLG_OFF
				: MailDistTextModel.LINE_USE_FLG_OFF);

		// 入力チェック
		string strErrorMessages = CheackInputData(htInput);

		// エラーページへ遷移？
		if (strErrorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// 入力HashTableをラップしちゃう
		var model = new MailDistTextModel(htInput);

		// SMS
		var smsInputs = GetSmsInput(model);

		// SMSのバリデート
		var smsInputErrors = smsInputs.Select(x => x.Validate()).ToArray();
		if (smsInputErrors.Any(m => m != ""))
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.Join("<br />", smsInputErrors.Where(m => m != "").ToArray());
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// LINE
		if (model.LineUseFlg == MailDistTextModel.LINE_USE_FLG_ON)
		{
			SetLineContentsInput();
			var lineInputErrors = this.LineDirectSendMessageContents.Select(input => input.Validate()).ToArray();
			if (lineInputErrors.Any(msg => string.IsNullOrEmpty(msg) == false))
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = string.Join(
					Environment.NewLine,
					lineInputErrors.Where(msg => string.IsNullOrEmpty(msg) == false).ToArray());
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
		// セッションへパラメタセット
		Session[Constants.SESSION_KEY_PARAM] = htInput;
		Session[Constants.SESSIONPARAM_KEY_SMSDISTTEXT_INFO] = smsInputs;
		SetLineContentsInput();

		// 確認画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_CONFIRM + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + m_strActionStatus);
	}

	/// <summary>
	/// SMS配信文章入力情報取得
	/// </summary>
	/// <param name="inputData">メール配信文章入力情報</param>
	/// <returns>SMS配信文章入力情報</returns>
	private GlobalSMSDistTextInput[] GetSmsInput(MailDistTextModel inputData)
	{
		var rtn = new List<GlobalSMSDistTextInput>();

		if (Constants.GLOBAL_SMS_OPTION_ENABLED && inputData.SmsUseFlg == MailDistTextModel.SMS_USE_FLG_ON)
		{
			// リピーターぐるぐる回す
			foreach (RepeaterItem ri in this.rSmsCarrier.Items)
			{
				var txtSmsBody = (TextBox)ri.FindControl("tbSmsText");
				var hCarrier = (HiddenField)ri.FindControl("hCarrier");

				var smsInput = new GlobalSMSDistTextInput();
				smsInput.DeptId = inputData.DeptId;
				smsInput.MailtextId = inputData.MailtextId;
				smsInput.PhoneCarrier = hCarrier.Value;
				smsInput.SmsText = txtSmsBody.Text;
				smsInput.LastChanged = base.LoginOperatorName;
				rtn.Add(smsInput);
			}
		}
		return rtn.ToArray();
	}

	/// <summary>
	/// 入力値チェック
	/// </summary>
	/// <param name="htInput">入力値</param>
	private string CheackInputData(Hashtable htInput)
	{
		StringBuilder sbResult = new StringBuilder();

		//------------------------------------------------------
		// 入力値チェック（必須チェック、文字数チェックなど）
		//------------------------------------------------------
		// 新規登録？
		if ((m_strActionStatus == Constants.ACTION_STATUS_INSERT)
			|| (m_strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
			|| (m_strActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
		{
			sbResult.Append(Validator.Validate("MailDistTextRegist", htInput));
		}
		// 編集確認？
		else if (m_strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			sbResult.Append(Validator.Validate("MailDistTextModify", htInput));
		}

		//------------------------------------------------------
		// タイトル、本文が入力されていることを確認する
		//------------------------------------------------------
		string strSubject = StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT]); // メールタイトル
		string strBody = StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY]); // メール本文
		string strBodyHtml = StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML]); // HTMLメール本文
		string strSubjectMobile = StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE]); // メールタイトル（モバイル）
		string strBodyMobile = StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE]); // メール本文(モバイル）
		string strBodyDecome = StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_DECOME]); // デコメ本文

		// タイトルが存在しているかチェック
		if ((strSubject != "") || (strSubjectMobile != ""))
		{
			// PCのタイトルが存在している場合
			if (strSubject != "")
			{
				// 本文チェック（ＰＣ）
				sbResult.Append(CheackBody(strBody, strBodyHtml, WebMessages.ERRMSG_MANAGER_MAILDISTTEST_NO_BODY_ERROR));

				// タイトルなし本文チェック（モバイル）
				sbResult.Append(
					CheackBodyWithoutSubject(
						strSubjectMobile,
						strBodyMobile,
						strBodyDecome,
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MAIL_DIST_TEXT_NOT_TITLE_TEXT_MOBILE)));
			}

			// モバイルのタイトルが存在している場合
			if (strSubjectMobile != "")
			{
				// 本文チェック（モバイル）
				sbResult.Append(CheackBody(strBodyMobile, strBodyDecome, WebMessages.ERRMSG_MANAGER_MAILDISTTEST_NO_BODY_ERROR_MOBILE));

				// タイトルなし本文チェック（ＰＣ）
				sbResult.Append(
					CheackBodyWithoutSubject(
						strSubject,
						strBody,
						strBodyHtml,
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MAIL_DIST_TEXT_NOT_TITLE_TEXT_PC)));
			}
		}
		else
		{
			// タイトルが入力されていなければエラー
			sbResult.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MAILDISTTEST_NO_TITLE_ERROR).Replace("@@ 1 @@", ""));
		}

		if (Constants.GLOBAL_OPTION_ENABLE && m_strActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
		{
			// 言語コード重複登録チェック
			var mailtextId = (string)htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID];
			var languageCode = (string)htInput[Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE];
			var languageLocaleId = (string)htInput[Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID];

			var result = new MailDistService().CheckLanguageCodeDupulication(
				this.LoginOperatorDeptId,
				mailtextId,
				languageCode,
				languageLocaleId);

			if (result == false)
			{
				sbResult.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION)
					.Replace("@@ 1 @@", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_LANGUAGE_CODE)));
			}
		}
		return sbResult.ToString();
	}

	/// <summary>
	/// メール本文チェック
	/// </summary>
	/// <param name="strBody">本文</param>
	/// <param name="strBodyHtml">HTML本文（モバイルの場合は、デコメ本文）</param>
	/// <param name="strMessageKey">メッセージキー</param>
	private string CheackBody(string strBody, string strBodyHtml, string strMessageKey)
	{
		// 本文、HTML本文どちらも存在しない場合はエラー
		if ((strBody == "") && (strBodyHtml == ""))
		{
			return WebMessages.GetMessages(strMessageKey);
		}

		return "";
	}

	/// <summary>
	/// タイトルなし本文チェック
	/// </summary>
	/// <param name="strSubject">タイトル</param>
	/// <param name="strBody">本文</param>
	/// <param name="strBodyHtml">HTML本文（モバイルの場合は、デコメ本文）</param>
	/// <param name="strErrorMsg">追加で表示するエラーメッセージ</param>
	private string CheackBodyWithoutSubject(string strSubject, string strBody, string strBodyHtml, string strErrorMsg)
	{
		// タイトルが存在せず、本文が入力されている場合はエラー
		if ((strSubject == "") 
			&& ((strBody != "") || (strBodyHtml != "")))
		{
			return  WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MAILDISTTEST_NO_TITLE_ERROR).Replace("@@ 1 @@", strErrorMsg);
		}

		return "";
	}

	/// <summary>
	/// ラジオボタン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblSendHtmlFlg_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// メール文章HTML表示制御
		DispMailDistTextHtmlForm();
	}

	/// <summary>
	/// メール文章名表示制御
	/// </summary>
	/// <param name="actionStatus">実行アクション</param>
	/// <remarks>グローバルOP：ONの場合のみ、メール文章名の表示制御を行う</remarks>
	private void DisplayControlMailTextName(string actionStatus)
	{
		if (Constants.GLOBAL_OPTION_ENABLE == false) return; ;

		if (actionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
		{
			lMailtextName.Visible = true;
			tbMailtextName.Visible = false;
		}
		else if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			lMailtextName.Visible = (this.MailDistTextSettingTableName != Constants.TABLE_MAILDISTTEXT);
			tbMailtextName.Visible = (this.MailDistTextSettingTableName == Constants.TABLE_MAILDISTTEXT);
		}
	}

	/// <summary>
	/// LINE送信内容リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	public void rLineDirectContents_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var targetIndex = int.Parse((string)e.CommandArgument);
		switch (e.CommandName)
		{
			case "add_line_content":
				EditLineContent(true, targetIndex + 1);
				break;

			case "delete_line_content":
				EditLineContent(false, targetIndex);
				break;
		}
	}

	/// <summary>
	/// LINE送信内容編集
	/// </summary>
	/// <param name="isAdd">追加か？</param>
	/// <param name="editIndex">編集位置</param>
	private void EditLineContent(bool isAdd, int editIndex)
	{
		SetLineContentsInput();

		if (isAdd)
		{
			// 最大5つまで
			if ((this.LineDirectSendMessageContents.Count >= w2.App.Common.Line.Constants.LINE_DIRECT_MAX_MESSAGE_COUNT)
				|| (editIndex >= w2.App.Common.Line.Constants.LINE_DIRECT_MAX_MESSAGE_COUNT)
				|| (editIndex < 0)) return;

			var smcInput = new MessagingAppContentsInput()
			{
				BranchNo = (editIndex + 1).ToString(),
				Contents = "",
			};

			if (this.LineDirectSendMessageContents.Count > editIndex)
			{
				this.LineDirectSendMessageContents.Insert(editIndex, smcInput);
			}
			else
			{
				this.LineDirectSendMessageContents.Add(smcInput);
			}
		}
		else
		{
			if ((this.LineDirectSendMessageContents.Count <= 1)
				|| (this.LineDirectSendMessageContents.Count <= editIndex)) return;

			this.LineDirectSendMessageContents.RemoveAt(editIndex);
		}

		rLineDirectContents.DataSource = this.LineDirectSendMessageContents.Select(input => input.CreateModel()).ToArray();
		rLineDirectContents.DataBind();
		SetLineContentsInput();
	}

	/// <summary>
	/// LINE配信内容をリピーターにバインド
	/// </summary>
	/// <param name="isInsert">新規作成か？</param>
	private void LineDirectContentsDataBind(bool isInsert)
	{
		var bindData = new[]
		{
			new MessagingAppContentsInput
			{
				BranchNo = "0",
				Contents = "",
			}
		};

		if (isInsert == false)
		{
			rLineDirectContents.Visible = chkline.Checked;
			if (this.LineDirectSendMessageContents.Count > 0)
			{
				bindData = this.LineDirectSendMessageContents.ToArray();
			}
		}

		rLineDirectContents.DataSource = bindData.Select(input => input.CreateModel()).ToArray();
		rLineDirectContents.DataBind();
		SetLineContentsInput();
	}

	/// <summary>
	/// LINE送信内容Session保存
	/// </summary>
	private void SetLineContentsInput()
	{
		if (rLineDirectContents.Items.Count == 0) return;

		this.LineDirectSendMessageContents = rLineDirectContents.Items.Cast<RepeaterItem>().Select(
			ri =>
			{
				var tb = (TextBox)ri.FindControl("tbLineText");
				return new MessagingAppContentsInput
				{
					DeptId = this.LoginOperatorShopId,
					MasterKbn = MessagingAppContentsModel.MASTER_KBN_MAILDISTTEXT,
					MasterId = (string)ViewState[Constants.REQUEST_KEY_MAILTEXT_ID],
					MessagingAppKbn = MessagingAppContentsModel.MESSAGING_APP_KBN_LINE,
					BranchNo = (ri.ItemIndex + 1).ToString(),
					MediaType = MessagingAppContentsModel.MEDIA_TYPE_TEXT,
					Contents = tb.Text.Trim(),
					LastChanged = this.LoginOperatorName
				};
			}).ToList();
	}

	/// <summary>
	/// SMS利用チェックボックス変更
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void chksms_CheckedChanged(object sender, EventArgs e)
	{
		this.rSmsCarrier.Visible = this.chksms.Checked;
	}

	/// <summary>
	/// グローバル言語コードが指定されているか判断フラグを取得
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>グローバル言語コードが指定されているか</returns>
	private bool GetIsSelectedLanguageCode(string actionStatus)
	{
		if (Constants.GLOBAL_OPTION_ENABLE == false) return false;

		switch (actionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");
				return (wddlLanguageCode.SelectedIndex != 0);

			case Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT:
				return true;
		}

		return true;
	}

	/// <summary>
	/// LINE送信チェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void chkline_CheckedChanged(object sender, EventArgs e)
	{
		rLineDirectContents.Visible = chkline.Checked;
	}

	/// <summary>メール配信文章設定テーブル名(w2_MailDistText/w2_MailDistTextGlobal)</summary>
	protected string MailDistTextSettingTableName
	{
		get { return (string)ViewState[Constants.SETTING_TABLE_NAME]; }
		set { ViewState[Constants.SETTING_TABLE_NAME] = value; }
	}
	/// <summary>言語コード</summary>
	protected string LanguageCode
	{
		get { return (string)ViewState[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE]; }
		set { ViewState[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE] = value; }
	}
	/// <summary>グローバル言語コードが指定されているかの判断フラグ</summary>
	protected bool IsSelectedLanguageCode { get; set; }
	/// <summary> LINE送信内容 </summary>
	protected List<MessagingAppContentsInput> LineDirectSendMessageContents
	{
		get
		{
			if (SessionManager.Session[Constants.SESSIONPARAM_KEY_LINEDISTTEXT_INFO] == null)
			{
				SessionManager.Session[Constants.SESSIONPARAM_KEY_LINEDISTTEXT_INFO] = new List<MessagingAppContentsInput>();
			}
			return (List<MessagingAppContentsInput>)SessionManager.Session[Constants.SESSIONPARAM_KEY_LINEDISTTEXT_INFO];
		}
		set { SessionManager.Session[Constants.SESSIONPARAM_KEY_LINEDISTTEXT_INFO] = value; }
	}
}