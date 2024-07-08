/*
=========================================================================================================
  Module      : メール配信文章確認ページ処理(MailDistTextConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Input.MailDist;
using w2.App.Common.MailDist;
using w2.App.Common.Web;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.GlobalSMS;
using w2.Domain.MailDist;
using w2.Domain.MessagingAppContents;

public partial class Form_MailDistText_MailDistTextConfirm : DecomeBasePage
{
	const string TAG_MAIL_CLICK_URL_BGN = "[[TAG_MAIL_CLICK_URL_BGN]]";
	const string TAG_MAIL_CLICK_URL_END = "[[TAG_MAIL_CLICK_URL_END]]";

	public string m_strActionStatus = null;
	string m_strMailTextId = null;

	List<string> m_lUrls = new List<string>();

	/// <summary>SMS入力情報</summary>
	protected GlobalSMSDistTextInput[] m_smsInputs = new GlobalSMSDistTextInput[] { };

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

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(m_strActionStatus);

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			switch (m_strActionStatus)
			{
				// 登録・確認の確認画面
				case Constants.ACTION_STATUS_INSERT:
				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT:

					Hashtable htMailDistText = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

					// メール文章ID取得＆ビューステート格納
					m_strMailTextId = (string)htMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID];
					ViewState[Constants.REQUEST_KEY_MAILTEXT_ID] = m_strMailTextId;
				
					// コントロールにメール文章反映
					DispMailText(htMailDistText);

					m_smsInputs = ((GlobalSMSDistTextInput[])Session[Constants.SESSIONPARAM_KEY_SMSDISTTEXT_INFO]);
					rSmsCarrier.DataSource = m_smsInputs
						.Select(x => x.CreateModel())
						.ToArray();

					rSmsCarrier.Visible =
						(StringUtility.ToEmpty(htMailDistText[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG])
							== MailDistTextModel.SMS_USE_FLG_ON);
					rSmsCarrier.DataBind();

					// LINE配信内容バインド
					rLineDirectContents.DataSource = this.LineDirectSendMessageContents
						.Select(input => input.CreateModel())
						.ToArray();
					rLineDirectContents.Visible =
						(StringUtility.ToEmpty(htMailDistText[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]) == MailDistTextModel.LINE_USE_FLG_ON);
					rLineDirectContents.DataBind();

					break;

				// 詳細画面
				case Constants.ACTION_STATUS_DETAIL:

					// メール文章ID取得＆ビューステート格納
					m_strMailTextId = Request[Constants.REQUEST_KEY_MAILTEXT_ID];
					ViewState[Constants.REQUEST_KEY_MAILTEXT_ID] = m_strMailTextId;

					DataView dvMailDistText = null;
					DataView dvMailClick = null;

					if (Constants.GLOBAL_OPTION_ENABLE == false)
					{
						using (SqlAccessor sqlAccessor = new SqlAccessor())
						{
							dvMailDistText = GetMailDistText(m_strMailTextId, sqlAccessor);
							dvMailClick = GetMailClick(m_strMailTextId, sqlAccessor);
						}
					}
					else
					{
						var requestLanguageCode = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE] ?? string.Empty;
						var requestLanguageLocaleId = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID] ?? string.Empty;

						if ((string.IsNullOrEmpty(requestLanguageCode) == false)
							&& (string.IsNullOrEmpty(requestLanguageLocaleId) == false))
						{
							dvMailDistText = new MailDistService().GetTextDataViewByLanguageCode(
								this.LoginOperatorDeptId,
								m_strMailTextId,
								requestLanguageCode,
								requestLanguageLocaleId);
						}
						else
						{
							dvMailDistText = GetMailDistText(m_strMailTextId, new SqlAccessor());
						}
						this.MailDistTextSettingTableName = Constants.TABLE_MAILDISTTEXT;

						if (dvMailDistText.Count == 0)
						{
							dvMailDistText = new MailDistService().GetTextGlobalSettingDataView(
								this.LoginOperatorDeptId,
								m_strMailTextId,
								requestLanguageCode,
								requestLanguageLocaleId);
							this.MailDistTextSettingTableName = Constants.TABLE_MAILDISTTEXTGLOBAL;
						}
						dvMailClick = GetMailClick(m_strMailTextId, new SqlAccessor());
					}

					// 該当データが有りの場合
					if (dvMailDistText.Count != 0)
					{
						DataRowView drvMailDistText = dvMailDistText[0];
						Hashtable htInput = new Hashtable();
						foreach (DataColumn dc in drvMailDistText.Row.Table.Columns)
						{
							htInput[dc.ColumnName] = StringUtility.ToEmpty(drvMailDistText[dc.ColumnName]);
						}

						// コントロールにメール文章反映
						DispMailText(htInput);

						// SMS情報
						if (Constants.GLOBAL_SMS_OPTION_ENABLED)
						{
							// SMS情報
							var smsSv = new GlobalSMSService();
							var smsTeplates = smsSv.GetSmsDistTexts(this.LoginOperatorDeptId, m_strMailTextId);
							var carrier = ValueText.GetValueItemArray("sms_carrier", "carrier_id");

							var smsData = carrier.Select(
								x =>
								{
									return smsTeplates.FirstOrDefault(t => t.PhoneCarrier == x.Value)
										?? new GlobalSMSDistTextModel()
										{
											PhoneCarrier = x.Value,
											DateChanged = DateTime.Now,
											DateCreated = DateTime.Now,
											SmsText = ""
										};
								}).ToArray();
							m_smsInputs = smsData.Select(x => new GlobalSMSDistTextInput(x)).ToArray();
							this.rSmsCarrier.DataSource = smsData;
							this.rSmsCarrier.DataBind();
						}

						// LINE情報
						if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED)
						{
							var msgAppContents = new MessagingAppContentsService().GetAllContentsEachMessagingAppKbn(
								this.LoginOperatorDeptId,
								MessagingAppContentsModel.MASTER_KBN_MAILDISTTEXT,
								m_strMailTextId,
								MessagingAppContentsModel.MESSAGING_APP_KBN_LINE);

							this.LineDirectSendMessageContents = msgAppContents.Select(x => new MessagingAppContentsInput(x)).ToList();
							rLineDirectContents.DataSource = msgAppContents;
							rLineDirectContents.Visible =
								(StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]) == MailDistTextModel.LINE_USE_FLG_ON);
							rLineDirectContents.DataBind();
						}
					}
					else
					{
						// 該当データ無しの場合、エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
					}
					break;

				default:
					// 該当データ無しの場合、エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
					break;
			}
		}
		else
		{
			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
			m_strMailTextId = (string)ViewState[Constants.REQUEST_KEY_MAILTEXT_ID];
		}
	}

	/// <summary>
	/// メール文章取得
	/// </summary>
	/// <param name="mailtextId">メール文章ID</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>メール文章</returns>
	private DataView GetMailDistText(string mailtextId, SqlAccessor accessor = null)
	{
		using (SqlStatement sqlStatement = new SqlStatement("MailDistText", "GetMailDistText"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILDISTTEXT_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, m_strMailTextId);

			var dvMailDistText = sqlStatement.SelectSingleStatementWithOC(accessor, htInput);
			return dvMailDistText;
		}
	}

	/// <summary>
	/// メールクリック設定取得
	/// </summary>
	/// <param name="mailtextId">メール文章ID</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>メールクリック設定</returns>
	private DataView GetMailClick(string mailtextId, SqlAccessor accessor = null)
	{
		using (SqlStatement sqlStatement = new SqlStatement("MailClick", "GetMailClickAll"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILCLICK_MAILTEXT_ID, mailtextId);

			var dvMailClick = sqlStatement.SelectSingleStatementWithOC(accessor, htInput);
			return dvMailClick;
		}
	}

	/// <summary>
	/// コントロールにメール文章の内容を反映する
	/// </summary>
	/// <param name="htMailDistText"></param>
	private void DispMailText(Hashtable htMailDistText)
	{
		lMailtextId.Text = WebSanitizer.HtmlEncode(htMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]);
		lMailtextName.Text = WebSanitizer.HtmlEncode(htMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME]);
		lMailFromName.Text = WebSanitizer.HtmlEncode(htMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_FROM_NAME]);
		lMailFrom.Text = WebSanitizer.HtmlEncode(htMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_FROM]);
		lMailtextSubject.Text = WebSanitizer.HtmlEncode(htMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT]);
		
		var mailTextDecome = string.Empty;
		if ((Constants.GLOBAL_OPTION_ENABLE == false)
			|| (this.MailDistTextSettingTableName == Constants.TABLE_MAILDISTTEXT))
		{
			lMailtextSubjectMobile.Text = ReplaceEmojiTagToHtml((string)htMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE]);
			dvMailtextSubjectMobile.Visible = (string.IsNullOrEmpty(lMailtextSubjectMobile.Text) == false);
			lMailtextMobile.Text = ReplaceMailClickUrlTagToHtml(StringUtility.ChangeToBrTag(ReplaceEmojiTagHtmlEncodedToHtml(WebSanitizer.HtmlEncode(SetUrlText((string)htMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE], Constants.FLG_MAILCLICK_PCMOBILE_KBN_MOBILE, false)))));
			dvMailtextMobile.Visible = (string.IsNullOrEmpty(lMailtextMobile.Text) == false);
			tblMobileMailInfo.Visible = dvMailtextMobile.Visible;
		}

		lMailtextBody.Text = ReplaceMailClickUrlTagToHtml(StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(SetUrlText((string)htMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY], Constants.FLG_MAILCLICK_PCMOBILE_KBN_PC, false))));
		var mailTextHtml = ReplaceMailClickUrlTagToHtml(SetUrlText((string)htMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML], Constants.FLG_MAILCLICK_PCMOBILE_KBN_PC, true));
		this.HtmlForPreviewList = new List<string> { mailTextHtml, mailTextDecome };

		lMailtextDateCreated.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				htMailDistText[Constants.FIELD_MAILDISTTEXT_DATE_CREATED],
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lMailtextDateChanged.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				htMailDistText[Constants.FIELD_MAILDISTTEXT_DATE_CHANGED],
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lMailtextLastChanged.Text = WebSanitizer.HtmlEncode(htMailDistText[Constants.FIELD_MAILDISTTEXT_LAST_CHANGED]);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var languageCode = (string)htMailDistText[Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE];
			var languageLocaleId = (string)htMailDistText[Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID];
			lLanguageCode.Text = (string.IsNullOrEmpty(languageCode) == false)
				? WebSanitizer.HtmlEncode(string.Format("{0}({1})", languageCode, languageLocaleId))
				: ValueText.GetValueText(Constants.TABLE_MAILDISTTEXT, Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE, string.Empty);	// 「指定しない」を表示

			InitializeLanguageCodeList(htMailDistText);
		}

		this.IsSelectedLanguageCode =
			GetIsSelectedLanguageCode((string)htMailDistText[Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE]);

		// SMS
		lsmsUseFlg.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTTEXT,
			Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG,
			StringUtility.ToEmpty(htMailDistText[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG]));
		rSmsCarrier.Visible = (StringUtility.ToEmpty(htMailDistText[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG]) == MailDistTextModel.SMS_USE_FLG_ON);

		// LINE
		llineDirectUseFlg.Text = ValueText.GetValueText(
			Constants.TABLE_MAILDISTTEXT,
			Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG,
			string.IsNullOrEmpty(StringUtility.ToEmpty(htMailDistText[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]))
				? MailDistTextModel.LINE_USE_FLG_OFF
				: StringUtility.ToEmpty(htMailDistText[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]));
		rLineDirectContents.Visible = (StringUtility.ToEmpty(htMailDistText[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]) == MailDistTextModel.LINE_USE_FLG_ON);
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 項目設定
		trMailtextId.Visible = (strActionStatus == Constants.ACTION_STATUS_DETAIL) || (strActionStatus == Constants.ACTION_STATUS_UPDATE);

		btnEditTop.Visible = btnEditBottom.Visible = (strActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = (strActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnMailClickSettingTop.Visible = btnMailClickSettingBottom.Visible = (strActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnDeleteTop.Visible = btnDeleteBottom.Visible = (strActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnInsertTop.Visible = btnInsertBottom.Visible
			= (strActionStatus == Constants.ACTION_STATUS_INSERT)
				|| (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| (strActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT);
		btnUpdateTop.Visible = btnUpdateBottom.Visible = (strActionStatus == Constants.ACTION_STATUS_UPDATE);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlLanguageCode.Visible = (strActionStatus == Constants.ACTION_STATUS_DETAIL);
			btnInsertGlobalTop.Visible = btnInsertGlobalBottom.Visible = (strActionStatus == Constants.ACTION_STATUS_DETAIL);
			divMobileSetting.Visible = (strActionStatus != Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT);
		}
	}

	/// <summary>
	/// 本文データ画面セット（メールクリック設定用チェックボックス設定）
	/// </summary>
	/// <param name="strMailText"></param>
	/// <param name="strPCMobileKbn"></param>
	/// <param name="blIsHtml"></param>
	private string SetUrlText(string strMailText, string strPCMobileKbn, bool blIsHtml)
	{
		StringBuilder sbNewBody = new StringBuilder();

		//------------------------------------------------------
		// メールクリック定義取得
		//------------------------------------------------------
		DataView dvMailClick = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailClick", "GetMailClick"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILCLICK_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILCLICK_MAILTEXT_ID, m_strMailTextId);
			htInput.Add(Constants.FIELD_MAILCLICK_PCMOBILE_KBN, strPCMobileKbn);

			dvMailClick = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		string strUrlPattern = MailDistTextUtility.GetPatturnUrl(blIsHtml);
		string strSeparatePattern = MailDistTextUtility.GetSeparatePattern(blIsHtml);

		foreach (string strMailTextLine in MailDistTextUtility.CreateMailTextLines(strMailText, strSeparatePattern))
		{
			string strTemp = strMailTextLine;

			Match mAnchorUrl = Regex.Match(strMailTextLine, strUrlPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			if (mAnchorUrl.Success)
			{
				string strAnchorUrl = mAnchorUrl.Value;
				string strUrl = strAnchorUrl;
				if (blIsHtml)
				{
					Match m = Regex.Match(strUrl, MailDistTextUtility.PATTURN_URL_TEXT);
					if (m.Success)
					{
						strUrl = m.Value;
					}
				}

				foreach (DataRowView drvMailClick in dvMailClick)
				{
					if (strUrl == (string)drvMailClick[Constants.FIELD_MAILCLICK_MAILCLICK_URL])
					{
						strTemp = strTemp.Replace(strAnchorUrl, TAG_MAIL_CLICK_URL_BGN + strAnchorUrl + TAG_MAIL_CLICK_URL_END);
						break;
					}
				}
			}
			sbNewBody.Append(strTemp);
		}

		return sbNewBody.ToString();
	}

	/// <summary>
	/// 角括弧タグをHTMLタグに変換する
	/// </summary>
	/// <param name="strText">文字列</param>
	/// <returns>角括弧タグをHTML分に置換した文字列</returns>
	/// <remarks>Text文章のHtmlエンコードを直前で行いたいので、Html変換用のタグを置換する</remarks>
	protected string ReplaceMailClickUrlTagToHtml(string strText)
	{
		return strText.Replace(TAG_MAIL_CLICK_URL_BGN, "<span style='color:Blue; font-weight:bold'>[</span>").Replace(TAG_MAIL_CLICK_URL_END, "<span style='color:Blue; font-weight:bold'>]</span>");
	}

	/// <summary>
	/// デコメ・テキストのサイズの総計を得る
	/// </summary>
	/// <param name="strText"></param>
	/// <returns></returns>
	protected int CalculateDecomeTextHtmlLength(string strText, string strDecome)
	{
		string strReplacedText = null;

		// タグを置換
		strReplacedText = strText + ReplaceImageTag(strDecome, "<img src='xx'>");
		strReplacedText = ReplaceEmojiTag(strReplacedText, "■");

		// 半角は1バイトとして計算したいので、SJISでのバイト数を得る
		return Encoding.GetEncoding("Shift-JIS").GetByteCount(strReplacedText);
	}

	/// <summary>
	/// デコメ画像ファイルサイズの総計を得る
	/// </summary>
	/// <param name="strSrcText"></param>
	/// <returns></returns>
	protected long CalculateDecomeImageFileLength(string strSrcText)
	{
		List<string> lFiles = new List<string>();
		long lDecomeImageFileLength = 0;

		ForEachTag(strSrcText, TAG_DECOME_IMAGE_HEAD, TAG_DECOME_IMAGE_FOOT, (string strMatch) =>
		{
			string strFilePath = Constants.MARKETINGPLANNER_DECOME_MOBILEHTMLMAIL_DIRPATH + strMatch.Split(':')[1].Replace(TAG_DECOME_IMAGE_FOOT, "");
			
			// ファイルが存在し、かつ重複していなければ、ファイルサイズを加算
			if ((lFiles.Contains(strFilePath) == false) && (File.Exists(strFilePath)))
			{
				lFiles.Add(strFilePath);
				lDecomeImageFileLength += new FileInfo(strFilePath).Length;
			}
		});

		return lDecomeImageFileLength;
	}

	/// <summary>
	/// 編集ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MAILTEXT_ID).Append("=").Append(Server.UrlEncode(m_strMailTextId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var selectedValueSplit = ddlLanguageCode.SelectedValue.Split('/');
			var languageCode = selectedValueSplit[0];
			var languageLocaleId = selectedValueSplit[1];

			sbUrl.Append("&").Append(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE).Append("=").Append(languageCode);
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID).Append("=").Append(languageLocaleId);
		}
		Response.Redirect(sbUrl.ToString());
	}
	
	/// <summary>
	/// 更新ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		Hashtable htInput = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

		if ((Constants.GLOBAL_OPTION_ENABLE == false) || ((string)htInput[Constants.SETTING_TABLE_NAME] == Constants.TABLE_MAILDISTTEXT))
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MailDistText", "UpdateMailDistText"))
			{
				int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}
		else
		{
			var input = new MailDistTextInput();
			input.DataSource = htInput;
			new MailDistService().UpdateTextGlobalSetting(input.CreateGlobalModel());
		}

		// 他言語のメールテンプレートを修正する時は、下の「SMS」と「LINE」情報を更新しない
		this.IsSelectedLanguageCode = GetIsSelectedLanguageCode(
			StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE]));
		if (this.IsSelectedLanguageCode)
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_LIST);
		}

		if (Constants.GLOBAL_SMS_OPTION_ENABLED && (StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG]) == MailDistTextModel.SMS_USE_FLG_ON))
		{
			var smsTemplate = ((GlobalSMSDistTextInput[])Session[Constants.SESSIONPARAM_KEY_SMSDISTTEXT_INFO])
				.Select(
					x =>
					{
						x.MailtextId = StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]);
						x.DateChanged = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
						x.DateCreated = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
						x.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
						return x.CreateModel();
					}).ToArray();

			var sv = new GlobalSMSService();

			foreach (var smsModel in smsTemplate)
			{
				sv.UpsertSMSDistText(smsModel);
			}
		}
		else if (Constants.GLOBAL_SMS_OPTION_ENABLED
			&& (StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG]) == MailDistTextModel.SMS_USE_FLG_OFF))
		{
			var sv = new GlobalSMSService();
			sv.DeleteSmsDistTexts(this.LoginOperatorDeptId, StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]));
		}

		// LINE
		if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED
			&& (StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]) == MailDistTextModel.LINE_USE_FLG_ON))
		{
			var msgAppContents = this.LineDirectSendMessageContents
				.Select(
					(x, index) =>
					{
						x.BranchNo = (index + 1).ToString();
						x.MasterId = StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]);
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
		else if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED
			&& (StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]) == MailDistTextModel.LINE_USE_FLG_OFF))
		{
			new MessagingAppContentsService().DeleteAllContentsEachMasterId(
				this.LoginOperatorDeptId,
				MessagingAppContentsModel.MASTER_KBN_MAILDISTTEXT,
				StringUtility.ToEmpty(htInput[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]));
		}

		Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_LIST );
	}

	/// <summary>
	/// 登録ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		Hashtable input = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

		if ((Constants.GLOBAL_OPTION_ENABLE == false)
			|| (m_strActionStatus != Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
		{
			// Create new mail dist text ID
			input[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID] = NumberingUtility.CreateKeyId(
				this.LoginOperatorShopId,
				Constants.NUMBER_KEY_MP_MAILDISTTEXT_ID,
				10);

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MailDistText", "InsertMailDistText"))
			{
				int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
		}
		else
		{
			var mailDistTextinput = new MailDistTextInput();
			mailDistTextinput.DataSource = input;
			new MailDistService().InsertTextGlobalSetting(mailDistTextinput.CreateGlobalModel());
		}

		// SMS情報
		if (Constants.GLOBAL_SMS_OPTION_ENABLED
			&& (StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG]) == MailDistTextModel.SMS_USE_FLG_ON))
		{
			var smsTemplate = ((GlobalSMSDistTextInput[])Session[Constants.SESSIONPARAM_KEY_SMSDISTTEXT_INFO])
				.Select(
					x =>
					{
						x.MailtextId = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]);
						x.DateChanged = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
						x.DateCreated = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
						x.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
						return x.CreateModel();
					}).ToArray();

			var sv = new GlobalSMSService();

			foreach (var smsModel in smsTemplate)
			{
				sv.UpsertSMSDistText(smsModel);
			}
		}

		// LINE情報
		if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED
			&& (StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]) == MailDistTextModel.LINE_USE_FLG_ON))
		{
			var msgAppContents = this.LineDirectSendMessageContents
				.Select(
					x =>
					{
						x.MasterId = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]);
						return x.CreateModel();
					}).ToArray();

			var sv = new MessagingAppContentsService();
			foreach (var smcModel in msgAppContents)
			{
				sv.UpsertContents(smcModel);
			}
		}

		Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_LIST );
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// メール配信設定に設定されていないかチェック
		//------------------------------------------------------
		DataView dvMailDistSettings = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "CheckMailDistTextUsed"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILDISTTEXT_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, m_strMailTextId);

			dvMailDistSettings = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
		}

		//------------------------------------------------------
		// 設定されていたら削除させない。エラーページへ。
		//------------------------------------------------------
		if (dvMailDistSettings.Count != 0)
		{
			// エラーメッセージに、設定されているメール配信設定を表示させる
			StringBuilder sbErrMsg = new StringBuilder();
			sbErrMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MAILDISTTEXT_USED));
			foreach (DataRowView drvMailDistSetting in dvMailDistSettings)
			{
				sbErrMsg.Append((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]).Append(" : ");
				sbErrMsg.Append((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME]).Append("<br />");
			}

			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrMsg.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 設定されていなければ削除フラグを立てる
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailDistText", "UpdateMailDistTextDelFlg"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILDISTTEXT_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, m_strMailTextId);

			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			new MailDistService().DeleteTextGlobalSetting(this.LoginOperatorDeptId, m_strMailTextId);
		}

		if (Constants.GLOBAL_SMS_OPTION_ENABLED)
		{
			var sv = new GlobalSMSService();
			sv.DeleteSmsDistTexts(this.LoginOperatorDeptId, m_strMailTextId);
		}

		if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED)
		{
			new MessagingAppContentsService().DeleteAllContentsEachMasterId(
				this.LoginOperatorDeptId,
				MessagingAppContentsModel.MASTER_KBN_MAILDISTTEXT,
				m_strMailTextId);
		}

		Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_LIST );
	}

	/// <summary>
	/// コピー新規登録クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_REGISTER);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_MAILTEXT_ID).Append("=").Append(HttpUtility.UrlEncode(m_strMailTextId));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_COPY_INSERT);

		Response.Redirect(sbResult.ToString());
	}

	/// <summary>
	/// 言語コードリスト初期化
	/// </summary>
	private void InitializeLanguageCodeList(Hashtable htMailDistText)
	{
		var settings =
			new MailDistService().GetMailDistTextContainGlobalSetting(this.LoginOperatorDeptId, m_strMailTextId);

		// 「言語コード/言語ロケールID」の形式でドロップダウンリストを生成する
		var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");
		wddlLanguageCode.AddItems(settings.OrderBy(setting => setting.LanguageCode)
			.Select(setting =>
				new ListItem(
					(string.IsNullOrEmpty(setting.LanguageCode) == false)
						? string.Format("{0}({1})", setting.LanguageCode, setting.LanguageLocaleId)
						: ValueText.GetValueText(Constants.TABLE_MAILDISTTEXT, Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE, string.Empty)	// 「指定しない」を表示
					, string.Format("{0}/{1}", setting.LanguageCode, setting.LanguageLocaleId))).ToArray());

		var languageCode = (string)htMailDistText[Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE] ?? string.Empty;
		var languageLocaleId = (string)htMailDistText[Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID] ?? string.Empty;
		wddlLanguageCode.SelectItemByValue(string.Format("{0}/{1}", languageCode, languageLocaleId));
	}

	/// <summary>
	/// 言語コードドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlLanguageCode_SelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedItem = ((DropDownList)sender).SelectedItem;
		var selectedValueSplit = selectedItem.Value.Split('/');

		var languageCode = selectedValueSplit[0];
		var languageLocaleId = selectedValueSplit[1];

		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_MAILTEXT_ID, m_strMailTextId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL);

		if ((string.IsNullOrEmpty(languageCode) == false) && (string.IsNullOrEmpty(languageLocaleId) == false))
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE, languageCode)
				.AddParam(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID, languageLocaleId);
		}

		Response.Redirect(urlCreator.CreateUrl());
	}

	/// <summary>
	/// 他言語コードで登録するクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertGlobal_Click(object sender, EventArgs e)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_REGISTER)
			.AddParam(Constants.REQUEST_KEY_MAILTEXT_ID, m_strMailTextId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT);
		Response.Redirect(urlCreator.CreateUrl());
	}

	/// <summary>
	/// メールクリック設定リンク取得
	/// </summary>
	/// <returns>メールクリック設定リンク</returns>
	protected string GetMailClickPopupUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_MAILCLICK).AddParam(
			Constants.REQUEST_KEY_MAILTEXT_ID,
			m_strMailTextId).CreateUrl();

		return url;
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

	/// <summary>メール配信文章設定テーブル名(w2_MailDistText/w2_MailDistTextGlobal)</summary>
	protected string MailDistTextSettingTableName
	{
		get { return (string)ViewState[Constants.SETTING_TABLE_NAME]; }
		set { ViewState[Constants.SETTING_TABLE_NAME] = value; }
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