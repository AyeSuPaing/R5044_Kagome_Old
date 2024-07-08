/*
=========================================================================================================
  Module      : メール送信ページ処理(MailSend.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Mail;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.MailTemplate;

public partial class Form_MailSend : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// メール送信用データ取得
			//------------------------------------------------------
			var dataSendMail = (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID])
				? new MailTemplateDataCreaterForOrder(true).GetOrderMailDatas(Request[Constants.REQUEST_KEY_ORDER_ID])
				: new MailTemplateDataCreaterForFixedPurchase(true).GetFixedPurchaseMailDatas(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID]));

			//------------------------------------------------------
			// 各入力項目に注文メール情報を設定
			//------------------------------------------------------
			// 注文情報が存在する場合
			var blExist = (dataSendMail.Count != 0);
			if (blExist)
			{
				try
				{
					string languageCode = null;
					string languageLocaleId = null;

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						// 注文者、定期台帳に設定されている言語コードを初期表示する
						languageCode = (string)dataSendMail[Constants.FIELD_USER_DISP_LANGUAGE_CODE];
						languageLocaleId = (string)dataSendMail[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID];

						// ドロップダウンリストで言語コードを変更した場合は、変更した言語コードでテンプレートを取得する
						if (Request.QueryString.AllKeys.Any(key => key == Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE)
							&& Request.QueryString.AllKeys.Any(key => key == Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID))
						{
							languageCode = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE];
							languageLocaleId = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID];
						}
					}

					// メールテンプレートのスクリプトを置換後、各入力項目に値を設定
					using (MailSendUtility msMailSend = new MailSendUtility(
						this.LoginOperatorShopId,
						Request[Constants.REQUEST_KEY_MAIL_TEMPLATE_ID],
						(string)dataSendMail[Constants.FIELD_ORDER_USER_ID],
						dataSendMail,
						(bool)dataSendMail["is_pc"],
						Constants.MailSendMethod.Manual,
						languageCode,
						languageLocaleId,
						(string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]))
					{
						lMailName.Text = lMailNameConfirm.Text = WebSanitizer.HtmlEncode(msMailSend.TmpName);
						tbMailFrom.Text = string.IsNullOrEmpty(msMailSend.Message.From.DisplayName) 
							? msMailSend.TmpFrom 
							: string.Format("{0}<{1}>", msMailSend.Message.From.DisplayName, msMailSend.TmpFrom);
						tbMailTo.Text = ((string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] != "" ? (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] : (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]);
						tbMailCc.Text = msMailSend.TmpCC;
						tbMailBcc.Text = msMailSend.TmpBcc;
						tbMailSubject.Text = msMailSend.Subject;
						tbMailBody.Text = msMailSend.Body;
						tbMailBodyHtml.Text = msMailSend.BodyHtml;
						tbSmsText.Text = (Constants.GLOBAL_SMS_OPTION_ENABLED && msMailSend.UseSms) ? msMailSend.TmpSmsText : "";
						tbSmsText.Enabled = (Constants.GLOBAL_SMS_OPTION_ENABLED && msMailSend.UseSms);
						SetLineTextFromMailSendUtility(msMailSend);

						// メールテンプレート情報セット
						this.UserId = (string)dataSendMail[Constants.FIELD_ORDER_USER_ID];
						this.MailTemplateId = Request[Constants.REQUEST_KEY_MAIL_TEMPLATE_ID];
						this.MailTemplateName = msMailSend.TmpName;
						this.MallId = msMailSend.MallId;
						this.UseSms = msMailSend.UseSms;
						this.UseLine = msMailSend.UseLine;
						this.UserMailAddr = (((string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] != "")
							? (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]
							: (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]);
						this.CanSendHtml = msMailSend.CanSendHtml;

						// Enabled textbox html
						tbMailBodyHtml.Enabled = this.CanSendHtml;
					}

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						InitializeLanguageCodeList(languageCode, languageLocaleId);
						this.OrderId = (string)Request[Constants.REQUEST_KEY_ORDER_ID];
						this.FixedPurchaseId = (string)Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID];
					}
				}
				catch
				{
					// メールテンプレートが存在しない場合、ここに遷移
					blExist = false;
				}
			}

			//------------------------------------------------------
			// 画面表示制御
			//------------------------------------------------------
			divEdit.Visible = blExist;
			divNoData.Visible = (blExist == false);
		}
		else
		{
			// メッセージ初期化
			divMessages.Visible = false;
			lbMessages.Text = "";
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();
		htInput.Add(Constants.FIELD_MAILTEMPLATE_MAIL_FROM, tbMailFrom.Text);
		htInput.Add(Constants.FIELD_MAILTEMPLATE_MAIL_TO, tbMailTo.Text);
		htInput.Add(Constants.FIELD_MAILTEMPLATE_MAIL_CC, tbMailCc.Text);
		htInput.Add(Constants.FIELD_MAILTEMPLATE_MAIL_BCC, tbMailBcc.Text);
		htInput.Add(Constants.FIELD_MAILTEMPLATE_MAIL_SUBJECT, tbMailSubject.Text);

		StringBuilder sbMessages = new StringBuilder(Validator.Validate("MailSendRegist", htInput));

		string errorMessage;

		// 送信元メールアドレス形式チェック
		if (((string)htInput[Constants.FIELD_MAILTEMPLATE_MAIL_FROM]).Trim() != "")
		{
			errorMessage = Validator.CheckMailAddrInput(
				ReplaceTag("@@DispText.mail_send.MailFrom@@"),
				(string)htInput[Constants.FIELD_MAILTEMPLATE_MAIL_FROM]);
			if (errorMessage != "") sbMessages.Append(errorMessage + "<br />");
		}

		// 送信先メールアドレス形式チェック(カンマ区切りで入力できる)
		if (((string)htInput[Constants.FIELD_MAILTEMPLATE_MAIL_TO]).Trim() != "")
		{
			errorMessage = Validator.CheckMailAddrInputs(
				ReplaceTag("@@DispText.mail_send.MailTo@@"),
				(string)htInput[Constants.FIELD_MAILTEMPLATE_MAIL_TO]);
			if (errorMessage != "") sbMessages.Append(errorMessage + "<br />");
		}

		// Ccメールアドレス形式チェック(カンマ区切りで入力できる)
		if (((string)htInput[Constants.FIELD_MAILTEMPLATE_MAIL_CC]).Trim() != "")
		{
			errorMessage = Validator.CheckMailAddrInputs(
				ReplaceTag("@@DispText.mail_send.MailCc@@"),
				(string)htInput[Constants.FIELD_MAILTEMPLATE_MAIL_CC]);
			if (errorMessage != "") sbMessages.Append(errorMessage + "<br />");
		}

		// Bccメールアドレス形式チェック(カンマ区切りで入力できる)
		if (((string)htInput[Constants.FIELD_MAILTEMPLATE_MAIL_BCC]).Trim() != "")
		{
			errorMessage = Validator.CheckMailAddrInputs(
				ReplaceTag("@@DispText.mail_send.MailBcc@@"),
				(string)htInput[Constants.FIELD_MAILTEMPLATE_MAIL_BCC]);
			if (errorMessage != "") sbMessages.Append(errorMessage + "<br />");
		}

		//------------------------------------------------------
		// メッセージ表示制御
		//------------------------------------------------------
		divMessages.Visible = (sbMessages.Length != 0);
		lbMessages.Text = sbMessages.ToString();

		//------------------------------------------------------
		// 画面表示制御
		//------------------------------------------------------
		if (sbMessages.Length == 0)
		{
			// 確認画面表示用に値を設定
			lMailFrom.Text = WebSanitizer.HtmlEncode(tbMailFrom.Text);
			lMailTo.Text = WebSanitizer.HtmlEncode(tbMailTo.Text);
			lMailCc.Text = WebSanitizer.HtmlEncode(tbMailCc.Text);
			lMailBcc.Text = WebSanitizer.HtmlEncode(tbMailBcc.Text);
			lMailSubject.Text = WebSanitizer.HtmlEncode(tbMailSubject.Text);
			lMailMailBody.Text = StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(tbMailBody.Text));
			lSmsText.Text = StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(tbSmsText.Text));
			rLineDirectContents.DataSource = GetLineContentsModify().Select(text => StringUtility.ChangeToBrTag(text)).ToArray();
			rLineDirectContents.DataBind();

			// 確認画面表示
			divEdit.Visible = false;
			divConfirm.Visible = true;

			// メール送信可能フラグをtrueに設定（※確認画面での２重送信防止のため）
			this.MailSendEnableFlg = true;

			this.HtmlForPreviewList = new List<string>
			{
				tbMailBodyHtml.Text.Replace("<@@", "&lt;@@").Replace("</@@", "&lt;/@@").Replace("@@>", "@@&gt;")
			};
		}
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 画面表示制御
		//------------------------------------------------------
		// 編集画面表示
		divEdit.Visible = true;
		divConfirm.Visible = false;
	}

	/// <summary>
	/// メール送信ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendMail_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// メール送信
		//------------------------------------------------------
		bool blSuccess = true;
		StringBuilder sbMessages = new StringBuilder();
		// メール送信可能？
		if (this.MailSendEnableFlg)
		{
			using (MailSendUtility smsMailSend = new MailSendUtility(Constants.MailSendMethod.Manual))
			{
				// 各項目に入力値を設定
				Match match = Regex.Match(tbMailFrom.Text.Trim(), "<.*>");
				string nameFrom = match.Success ? tbMailFrom.Text.Replace(match.Value, "") : string.Empty;
				string emailAddress = match.Success ? match.Value.Replace("<", "").Replace(">", "") : tbMailFrom.Text.Trim();
				smsMailSend.SetFrom(emailAddress, nameFrom);
				foreach (string strMailAddr in tbMailTo.Text.Split(','))
				{
					smsMailSend.AddTo(strMailAddr.Trim());
				}
				foreach (string strMailAddr in tbMailCc.Text.Split(','))
				{
					smsMailSend.AddCC(strMailAddr.Trim());
				}
				foreach (string strMailAddr in tbMailBcc.Text.Split(','))
				{
					smsMailSend.AddBcc(strMailAddr.Trim());
				}
				smsMailSend.SetSubject(tbMailSubject.Text);
				smsMailSend.SetBody(tbMailBody.Text);
				smsMailSend.SetBodyHtml(tbMailBodyHtml.Text);
				smsMailSend.TmpSmsText = tbSmsText.Text.Trim();
				smsMailSend.TmpLineText = GetLineContentsModify();

				// メールテンプレート情報セット
				smsMailSend.UserId = this.UserId;
				smsMailSend.ShopId = this.LoginOperatorShopId;
				smsMailSend.MailId = this.MailTemplateId;
				smsMailSend.TmpName = this.MailTemplateName;
				smsMailSend.MallId = this.MallId;
				smsMailSend.UseSms = this.UseSms;
				smsMailSend.UseLine = this.UseLine;
				smsMailSend.CanSendHtml = this.CanSendHtml;

				var result = CheckSendUser();

				// メール送信
				blSuccess = smsMailSend.SendMail(result);
			}

			if (blSuccess)
			{
				sbMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SEND_EMAIL_WITH_CONTENT));
			}
			else
			{
				sbMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ERROR_SEND_EMAIL));
			}
		}

		//------------------------------------------------------
		// メッセージ表示制御
		//------------------------------------------------------
		divMessages.Visible = (sbMessages.Length != 0);
		lbMessages.Text = sbMessages.ToString();
		lbMessages.ForeColor = blSuccess ? System.Drawing.Color.Empty : System.Drawing.Color.Red; // メール送信成功時は、黒字表示

		//------------------------------------------------------
		// 画面表示制御
		//------------------------------------------------------
		if (blSuccess)
		{
			// 戻るボタン、メール送信ボタンを非表示
			btnBackTop.Visible = btnBackBottom.Visible = false;
			btnSendMailTop.Visible = btnSendMailBottom.Visible = false;
			// 閉じるボタンを表示
			btnCloseTop.Visible = true;

			// ２重送信防止のため、メール送信可能フラグをfalseに設定
			// ※確認するボタン実行時にメール送信可能フラグをtrueに設定しています
			this.MailSendEnableFlg = false;
		}
	}

	/// <summary>
	/// 言語コードドロップダウンリスト変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlLanguageCode_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedItem = ((DropDownList)sender).SelectedItem;
		var selectedValueSplit = selectedItem.Value.Split('/');

		var languageCode = selectedValueSplit[0];
		var languageLocaleId = selectedValueSplit[1];

		var requestTableName = (string.IsNullOrEmpty(this.FixedPurchaseId))
			? Constants.REQUEST_KEY_ORDER_ID
			: Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID;

		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_SEND)
			.AddParam(requestTableName, string.IsNullOrEmpty(this.FixedPurchaseId) ? this.OrderId : this.FixedPurchaseId)
			.AddParam(Constants.REQUEST_KEY_MAIL_TEMPLATE_ID, this.MailTemplateId)
			.AddParam(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE, languageCode)
			.AddParam(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID, languageLocaleId);

		Response.Redirect(urlCreator.CreateUrl());
	}

	/// <summary>
	/// 言語コードリスト初期化
	/// </summary>
	/// <param name="languageCode">言語コード</param>
	/// <param name="languageLocaleId">言語ロケールID</param>
	protected void InitializeLanguageCodeList(string languageCode, string languageLocaleId)
	{
		var settings =
			new MailTemplateService().GetMailTemplateContainGlobalSetting(this.LoginOperatorShopId, this.MailTemplateId);

		// 「言語コード/言語ロケールID」の形式でドロップダウンリストを生成する
		var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");
		wddlLanguageCode.AddItems(settings.OrderBy(setting => setting.LanguageCode)
			.Select(setting =>
				new ListItem(
					(string.IsNullOrEmpty(setting.LanguageCode) == false)
						? string.Format("{0}({1})", setting.LanguageCode, setting.LanguageLocaleId)
						: ValueText.GetValueText(Constants.TABLE_MAILTEMPLATE, Constants.FIELD_MAILTEMPLATE_LANGUAGE_CODE, string.Empty)	// 「指定しない」を表示
					, string.Format("{0}/{1}", setting.LanguageCode, setting.LanguageLocaleId))).ToArray());

		wddlLanguageCode.SelectItemByValue(string.Format("{0}/{1}", languageCode, languageLocaleId));
	}

	/// <summary>
	/// メール送信ユーティリティからLINE送信内容を設定
	/// </summary>
	/// <param name="mailSendUtil">メール送信ユーティリティ</param>
	private void SetLineTextFromMailSendUtility(MailSendUtility mailSendUtil)
	{
		rLineDirectContentsModify.DataSource = (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && mailSendUtil.UseLine)
			? mailSendUtil.TmpLineText
			: new[] { string.Empty };
		rLineDirectContentsModify.DataBind();

		foreach (var ri in rLineDirectContentsModify.Items.Cast<RepeaterItem>())
		{
			var tb = (TextBox)ri.FindControl("tbLineText");
			tb.Enabled = (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && mailSendUtil.UseLine);
		}
	}

	/// <summary>
	/// LINE配信内容をstring配列に変換して取得
	/// </summary>
	/// <returns>LINE配信内容</returns>
	private string[] GetLineContentsModify()
	{
		return rLineDirectContentsModify.Items.Cast<RepeaterItem>().Select(
			ri =>
			{
				var tb = (TextBox)ri.FindControl("tbLineText");
				return tb.Text.Trim();
			}).ToArray();
	}

	/// <summary>
	/// 送信先に該当ユーザーが含まれているか
	/// </summary>
	/// <returns>含まれる:true、含まれない:false</returns>
	private bool CheckSendUser()
	{
		if ((this.UserMailAddr == tbMailTo.Text)
			|| (this.UserMailAddr == tbMailBcc.Text)
			|| (this.UserMailAddr == tbMailCc.Text))
		{
			return true;
		}
		return false;
	}

	#region プロパティ
	/// <summary>ユーザーID</summary>
	public string UserId
	{
		get { return (string)ViewState["UserId"]; }
		set { ViewState["UserId"] = value; }
	}
	/// <summary>メールテンプレートID</summary>
	public string MailTemplateId
	{
		get { return (string)ViewState["MailTemplateId"]; }
		set { ViewState["MailTemplateId"] = value; }
	}
	/// <summary>メールテンプレート名</summary>
	public string MailTemplateName
	{
		get { return (string)ViewState["MailTemplateName"]; }
		set { ViewState["MailTemplateName"] = value; }
	}
	/// <summary>モールID</summary>
	public string MallId
	{
		get { return (string)ViewState["MallId"]; }
		set { ViewState["MallId"] = value; }
	}
	/// <summary>メール送信可能フラグ</summary>
	public bool MailSendEnableFlg
	{
		get { return (bool)Session["mail_send_enable_flg"]; }
		set { Session["mail_send_enable_flg"] = value; }
	}
	/// <summary>注文ID</summary>
	public string OrderId
	{
		get { return (string)ViewState[Constants.FIELD_ORDER_ORDER_ID]; }
		set { ViewState[Constants.FIELD_ORDER_ORDER_ID] = value; }
	}
	/// <summary>定期購入ID</summary>
	public string FixedPurchaseId
	{
		get { return (string)ViewState[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]; }
		set { ViewState[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID] = value; }
	}
	/// <summary>SMS利用か否か</summary>
	public bool UseSms
	{
		get { return (bool)ViewState["UseSms"]; }
		set { ViewState["UseSms"] = value; }
	}
	/// <summary>LINE利用か否か</summary>
	public bool UseLine
	{
		get { return (bool)ViewState[Constants.FIELD_MAILTEMPLATE_LINE_USE_FLG]; }
		set { ViewState[Constants.FIELD_MAILTEMPLATE_LINE_USE_FLG] = value; }
	}
	/// <summary>メールアドレス</summary>
	public string UserMailAddr
	{
		get { return (string)ViewState["UserMailAddr"]; }
		set { ViewState["UserMailAddr"] = value; }
	}
	/// <summary>Can send html</summary>
	public bool CanSendHtml
	{
		get { return (bool)(ViewState["CanSendHtml"] ?? false); }
		set { ViewState["CanSendHtml"] = value; }
	}
	#endregion
}
