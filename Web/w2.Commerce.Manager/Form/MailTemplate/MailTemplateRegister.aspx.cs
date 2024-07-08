/*
=========================================================================================================
  Module      : メールテンプレート登録ページ処理(MailTemplateRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.ShopMessage;
using Input.MailTemplate;
using w2.App.Common.Mail;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Domain.GlobalSMS;
using w2.Domain.MailTemplate;
using w2.Domain.MessagingAppContents;

/// <summary>
/// MailTemplateModify の概要の説明です。
/// </summary>
public partial class Form_MailTemplate_MailTemplateRegister : BasePage
{
	protected MailTemplateInput m_input = new MailTemplateInput();
	protected GlobalSMSTemplateInput[] m_smsinput = new GlobalSMSTemplateInput[] { };

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
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus);

			//------------------------------------------------------
			// 処理区分チェック
			//------------------------------------------------------
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");

				// グローバル設定登録時以外は、「指定しない」をドロップダウンリストに表示する
				if (actionStatus != Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
				{
					wddlLanguageCode.Items.Add(
						new ListItem(ValueText.GetValueText(Constants.TABLE_MAILTEMPLATE, Constants.FIELD_MAILTEMPLATE_LANGUAGE_CODE, string.Empty)
							, string.Format("{0}/{1}", string.Empty, string.Empty)));
				}
				
				wddlLanguageCode.Items.AddRange(
					Constants.GLOBAL_CONFIGS.GlobalSettings.Languages
						.Select(c => new ListItem(string.Format("{0}({1})", c.Code, c.LocaleId), c.Code + "/" + c.LocaleId)).ToArray());
			}

			rbCheckHtml.DataSource = ValueText.GetValueItemList(
				Constants.TABLE_MAILTEMPLATE,
				Constants.FIELD_MAILTEMPLATE_SEND_HTML_FLG);
			this.SendHtmlFlg = Constants.FLG_MAILTEMPLATE_SEND_HTML_FLG_NOTSEND;
			rbCheckHtml.DataBind();

			// 新規？
			if ((actionStatus == Constants.ACTION_STATUS_INSERT))
			{
				// 処理無し
				trRegister.Visible = true;
				trAutoSendFlg.Visible = false;

				if (Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO] != null)
				{
					m_input = (MailTemplateInput)Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO];
					this.MailTextHtml = m_input.MailTextHtml;
					this.SendHtmlFlg = string.IsNullOrEmpty(m_input.SendHtmlFlg)
						? Constants.FLG_MAILTEMPLATE_SEND_HTML_FLG_NOTSEND
						: m_input.SendHtmlFlg;
					DataBind();
				}

				SetDropDawnListValue();
				ddlSelectTemplateTag.DataBind();
				this.MailId = ddlSelectTemplateTag.SelectedValue;

				if (actionStatus == Constants.ACTION_STATUS_INSERT) tblMobileSetting.Visible = true;
				if (Constants.GLOBAL_SMS_OPTION_ENABLED)
				{
					this.rSmsCarrier.DataSource = ValueText.GetValueItemArray("sms_carrier", "carrier_id").Select(
						x => new GlobalSMSTemplateModel()
						{
							PhoneCarrier = x.Value,
							SmsText = ""
						}).ToArray();

					this.rSmsCarrier.DataBind();
				}

				if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) LineDirectContentsDataBind(true);

				rReplacrmentTagList.DataSource = GetReplacementTag();
				rReplacrmentTagList.DataBind();
			}
			// コピー新規・編集？
			else if ((actionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| (actionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (actionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
			{
				// セッションよりメールテンプレート情報取得
				m_input = (MailTemplateInput)Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO];
				ViewState.Add(Constants.FIELD_MAILTEMPLATE_MAIL_ID, m_input.MailId);
				ViewState.Add(Constants.FIELD_MAILTEMPLATE_MAIL_CATEGORY, m_input.MailCategory);
				this.MailTextHtml = m_input.MailTextHtml;
				this.SendHtmlFlg = string.IsNullOrEmpty(m_input.SendHtmlFlg)
					? Constants.FLG_MAILTEMPLATE_SEND_HTML_FLG_NOTSEND
					: m_input.SendHtmlFlg;

				m_smsinput = (GlobalSMSTemplateInput[])Session[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO];

				m_input.MailFrom = MailSendUtility.AdjustShopMessageTagWithHavingSpace(m_input.MailFrom);
				m_input.MailBcc = MailSendUtility.AdjustShopMessageTagWithHavingSpace(m_input.MailBcc);

				if (actionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				{
					m_input.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_NOTSEND; ;
					trRegister.Visible = true;
					trAutoSendFlg.Visible = false;
					tblMobileSetting.Visible = true;

					if (m_input.MailId != null)
					{
						SessionManager.BeforeCopyMailId = m_input.MailId;
					}
					else
					{
						m_input.MailId = SessionManager.BeforeCopyMailId;
					}

					if (MailReplacementTag.HasInnternalElements(m_input.MailId) == false)
					{
						SetDropDawnListValue();
						this.MailId = ddlSelectTemplateTag.SelectedValue;
					}
				}
				else if ((actionStatus == Constants.ACTION_STATUS_UPDATE) || (actionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
				{
					trEdit.Visible = true;
					tblMobileSetting.Visible = true;

					// 自動送信フラグの表示
					trAutoSendFlg.Visible = (AutoSendPossibleCheck(m_input.MailId)) ? true : false ;

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");
						wddlLanguageCode.SelectItemByValue(string.Format("{0}/{1}", m_input.LanguageCode, m_input.LanguageLocaleId));

						// w2_MailTemplateテーブルに登録されている設定の場合のみ、モバイルの設定が可能
						if (m_input.MailTemplateSettingTableName != Constants.TABLE_MAILTEMPLATE) tblMobileSetting.Visible = false;

						ddlLanguageCode.Enabled = (actionStatus != Constants.ACTION_STATUS_UPDATE);

						if (actionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
						{
							trMailId.Visible = true;
							lMailName.Visible = true;
							tbMailName.Visible = false;
						}
						else
						{
							tbMailName.Visible = (m_input.MailTemplateSettingTableName == Constants.TABLE_MAILTEMPLATE);
							lMailName.Visible = (m_input.MailTemplateSettingTableName == Constants.TABLE_MAILTEMPLATEGLOBAL);
						}
					}

					if (MailReplacementTag.HasInnternalElements(m_input.MailId) == false)
					{
						SetDropDawnListValue();
					}

				}
				if (m_input.SmsUseFlg == MailTemplateModel.SMS_USE_FLG_ON)
				{
					this.rSmsCarrier.Visible = true;
				}

				if (m_smsinput != null)
				{
					this.rSmsCarrier.DataSource = m_smsinput.Select(x => x.CreateModel()).ToArray();
				}

				if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) LineDirectContentsDataBind(false);

				rReplacrmentTagList.DataSource = GetReplacementTag();

				// データバインド
				DataBind();

				rblAutoSendFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_MAILTEMPLATE, Constants.FIELD_MAILTEMPLATE_AUTO_SEND_FLG));
				rblAutoSendFlg.SelectedValue = (string)m_input.AutoSendFlg;

				rblSendLineFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_MAILTEMPLATE, Constants.FIELD_MAILTEMPLATE_LINE_USE_FLG));
				rblSendLineFlg.SelectedValue = (string)m_input.LineUseFlg;

				// ViewStateにメールテンプレート情報を格納
				ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO] = m_input;
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
		else
		{
			if ((this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (this.ActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
			{
				m_input = (MailTemplateInput)Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO];
			}
			this.MailId = ddlSelectTemplateTag.SelectedValue;
		}

		hfTagList.Value = ReplacementTagForString();
		this.IsSelectedLanguageCode = GetIsSelectedLanguageCode(this.ActionStatus);
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
	/// メールの用途変更時に使用可能置換タグを更新する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSelectTemplateTag_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.MailId = ddlSelectTemplateTag.SelectedItem.Value;
		rReplacrmentTagList.DataSource = GetReplacementTag();
		rReplacrmentTagList.DataBind();
		var mailCategory = "";
		switch (ddlSelectTemplateTag.SelectedIndex)
		{
			case Constants.KBN_EMAIL_USAGE_CATEGORY_ORDER:
				mailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_ORDER;
				break;

			case Constants.KBN_EMAIL_USAGE_CATEGORY_FIXEDPURCHASE:
				mailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_FIXEDPURCHASE;
				break;

			case Constants.KBN_EMAIL_USAGE_CATEGORY_USER:
				mailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_USER;
				break;

			case Constants.KBN_EMAIL_USAGE_CATEGORY_TOREALSHOP:
				mailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_TOREALSHOP;
				break;

			case Constants.KBN_EMAIL_USAGE_CATEGORY_FROMREALSHOP:
				mailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_FROMREALSHOP;
				break;

			default:
				mailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM;
				break;
		}

		ViewState[Constants.FIELD_MAILTEMPLATE_MAIL_CATEGORY] = mailCategory;
		this.SelectTemplateTagIndex = ddlSelectTemplateTag.SelectedIndex;
	}

	/// <summary>
	/// 使用可能な置換タグを取得
	/// </summary>
	/// <returns>使用可能な置換タグ一覧</returns>
	protected KeyValuePair<string, string>[] GetReplacementTag()
	{
		var mailId = UsageForEmailIdConversion(m_input.MailId);
		var replacementTagArray = ShopMessageUtil.GetMailTagArrayByShopMassage();
		var siteRootPathTagArray = new MailTemplateSiteRootPath().GetSiteRootPathMailTagArray();

		var defaultReplacementTagArray = MailReplacementTag.GetAvailableMailTemplateTagArrayByMailId(mailId)
			.ToDictionary(key => key.Text, value => value.Value);
		replacementTagArray = defaultReplacementTagArray.Concat(replacementTagArray).ToArray();
		replacementTagArray = replacementTagArray.Concat(siteRootPathTagArray).ToArray();

		return replacementTagArray.ToArray();
	}

	/// <summary>
	/// 使用可能な置換タグをカンマ区切りの文字列にする
	/// </summary>
	/// <returns>使用可能な置換タグ文字列</returns>
	protected string ReplacementTagForString()
	{
		var replacementTag = string.Format(
			"{0},{1},{2}",
			MailReplacementTag.GetMailTemplateTagArrayAllByMailId(UsageForEmailIdConversion(m_input.MailId)),
			ShopMessageUtil.GetMailTagByShopMessage(),
			new MailTemplateSiteRootPath().SiteRootPathMailTag);

		return replacementTag;
	}

	/// <summary>
	/// カスタムメールの場合は使用用途に応じたメールIDに変換
	/// </summary>
	/// <param name="mailId">編集中メールテンプレートのメールID</param>
	/// <returns>使用用途に応じたメールID</returns>
	private string UsageForEmailIdConversion(string mailId)
	{
		var result = (MailReplacementTag.HasInnternalElements(mailId) == false) ? this.MailId : m_input.MailId;
		return result;
	}

	/// <summary>
	/// ドロップダウンリストにメールの使用用途をセット
	/// </summary>
	protected void SetDropDawnListValue()
	{
		ddlSelectTemplateTag.DataSource = ValueText.GetValueKvpArray(Constants.VALUETEXT_PARAM_MAIL_TEMPLATE_SETTING_LIST, Constants.VALUETEXT_PARAM_USE_APPLICATIONS);
		ddlSelectTemplateTag.DataTextField = "Value";
		ddlSelectTemplateTag.DataValueField = "Key";

		if (m_input != null)
		{
			switch (m_input.MailCategory)
			{
				case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_ORDER:
					this.SelectTemplateTagIndex = Constants.KBN_EMAIL_USAGE_CATEGORY_ORDER;
					break;

				case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_FIXEDPURCHASE:
					this.SelectTemplateTagIndex = Constants.KBN_EMAIL_USAGE_CATEGORY_FIXEDPURCHASE;
					break;

				case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_USER:
					this.SelectTemplateTagIndex = Constants.KBN_EMAIL_USAGE_CATEGORY_USER;
					break;

				case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_TOREALSHOP:
					this.SelectTemplateTagIndex = Constants.KBN_EMAIL_USAGE_CATEGORY_TOREALSHOP;
					break;

				case Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_FROMREALSHOP:
					this.SelectTemplateTagIndex = Constants.KBN_EMAIL_USAGE_CATEGORY_FROMREALSHOP;
					break;
			}

			ddlSelectTemplateTag.SelectedIndex = this.SelectTemplateTagIndex;

			this.MailId = ValueText.GetValueKvpArray(
				Constants.VALUETEXT_PARAM_MAIL_TEMPLATE_SETTING_LIST,
				Constants.VALUETEXT_PARAM_USE_APPLICATIONS)[this.SelectTemplateTagIndex].Key;
		}
		else
		{
			ddlSelectTemplateTag.SelectedIndex = 0;
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		// 入力情報取得
		var inputData = GetInputData();

		//------------------------------------------------------
		// 処理ステータス
		//------------------------------------------------------
		var validator = String.Empty;
		// 新規・コピー新規
		if (((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT) ||
			((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT) ||
			((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
		{
			validator = "MailTemplateRegist";
		}
		// 変更
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			validator = "MailTemplateModify";
		}

		// 入力チェック＆重複チェック
		var errorMessages = inputData.Validate(validator);

		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
		{
			inputData.MailName = ((MailTemplateInput)ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO]).MailName;
		}

		if (errorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// SMS
		var smsInputs = GetSmsInput(inputData);
		// SMSのバリデート
		var smsInputErrors = smsInputs.Select(x => x.Validate()).ToArray();
		if (smsInputErrors.Any(m => m != ""))
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.Join("<br />",smsInputErrors.Where(m => m != "").ToArray());
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		
		// LINE
		if (inputData.LineUseFlg == MailTemplateModel.LINE_USE_FLG_ON)
		{
			SetLineContentsInput();
			var lineInputErrors = this.LineDirectSendMessageContents.Select(input => input.Validate()).ToArray();
			if (lineInputErrors.Any(msg => string.IsNullOrEmpty(msg) == false))
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = string.Join(
					Environment.NewLine,
					lineInputErrors.Where(m => (string.IsNullOrEmpty(m) == false)).ToArray());
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		inputData.MailFrom = tbMailFrom.Text.TrimAllSpaces();
		inputData.MailBcc = tbMailBcc.Text.TrimAllSpaces();

		// パラメタをセッションへ格納
		Session[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO] = inputData;
		Session[Constants.SESSIONPARAM_KEY_SMSTEMPLATE_INFO] = smsInputs;
		SetLineContentsInput();
		
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

		// メールテンプレート情報確認ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_CONFIRM +
			"?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}

	/// <summary>
	/// SMSテンプレート入力情報取得
	/// </summary>
	/// <param name="inputData">メールテンプレート入力情報</param>
	/// <returns>SMSテンプレート入力情報</returns>
	private GlobalSMSTemplateInput[] GetSmsInput(MailTemplateInput inputData)
	{
		var rtn = new List<GlobalSMSTemplateInput>();

		if (Constants.GLOBAL_SMS_OPTION_ENABLED && inputData.SmsUseFlg == MailTemplateModel.SMS_USE_FLG_ON)
		{
			// リピーターぐるぐる回す
			foreach (RepeaterItem ri in this.rSmsCarrier.Items)
			{
				var txtSmsBody = (TextBox)ri.FindControl("tbSmsText");
				var hCarrier = (HiddenField)ri.FindControl("hCarrier");

				var smsInput = new GlobalSMSTemplateInput();
				smsInput.ShopId = inputData.ShopId;
				smsInput.MailId = inputData.MailId;
				smsInput.PhoneCarrier = hCarrier.Value;
				smsInput.SmsText = txtSmsBody.Text;
				smsInput.LastChanged = base.LoginOperatorName;
				rtn.Add(smsInput);
			}
		}
		return rtn.ToArray();
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
			rLineDirectContents.Visible = (m_input.LineUseFlg == MailTemplateModel.LINE_USE_FLG_ON);
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
					MasterKbn = MessagingAppContentsModel.MASTER_KBN_MAILTEMPLATE,
					MasterId = (string)ViewState[Constants.FIELD_MAILTEMPLATE_MAIL_ID],
					MessagingAppKbn = MessagingAppContentsModel.MESSAGING_APP_KBN_LINE,
					BranchNo = (ri.ItemIndex + 1).ToString(),
					MediaType = MessagingAppContentsModel.MEDIA_TYPE_TEXT,
					Contents = tb.Text.Trim(),
					LastChanged = this.LoginOperatorName
				};
			}).ToList();
	}

	/// <summary>
	/// 入力情報取得
	/// </summary>
	/// <returns>入力情報</returns>
	private MailTemplateInput GetInputData()
	{
		var input = new MailTemplateInput();

		//------------------------------------------------------
		// 処理ステータス
		//------------------------------------------------------
		// 新規・コピー新規
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT ||
			(string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT)
		{
			switch (ddlSelectTemplateTag.SelectedIndex)
			{
				case Constants.KBN_EMAIL_USAGE_CATEGORY_ORDER:
					input.MailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_ORDER;
					break;

				case Constants.KBN_EMAIL_USAGE_CATEGORY_FIXEDPURCHASE:
					input.MailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_FIXEDPURCHASE;
					break;

				case Constants.KBN_EMAIL_USAGE_CATEGORY_USER:
					input.MailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_USER;
					break;

				case Constants.KBN_EMAIL_USAGE_CATEGORY_TOREALSHOP:
					input.MailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_TOREALSHOP;
					break;

				case Constants.KBN_EMAIL_USAGE_CATEGORY_FROMREALSHOP:
					input.MailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_FROMREALSHOP;
					break;

				default:
					input.MailCategory = Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM;
					break;
			}
		}
		// 変更・グローバル設定登録
		else if (((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
			|| ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
		{
			input.MailId = (string)ViewState[Constants.FIELD_MAILTEMPLATE_MAIL_ID];
			input.MailCategory = (string)ViewState[Constants.FIELD_MAILTEMPLATE_MAIL_CATEGORY];
			input.MailTemplateSettingTableName = ((MailTemplateInput)ViewState[Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO]).MailTemplateSettingTableName;
		}

		input.ShopId = this.LoginOperatorShopId;								// 店舗ID

		// グローバル設定登録時は、メールテンプレート名を後から格納する(メールテンプレート名の重複チェックがはしらないようにするため)
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] != Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
		{
			input.MailName = tbMailName.Text;
		}
		
		input.MailFromName = tbMailFromName.Text;								// メールFROM名
		input.MailTo = tbMailTo.Text.TrimAllSpaces();			// 送信先メールアドレス
		input.MailCc = tbMailCc.Text.TrimAllSpaces();			// Ccメールアドレス
		input.MailSubject = tbMailSubject.Text;									// メール件名
		input.MailBody = tbMailBody.Text;										// メール本文
		input.MailSubjectMobile = tbMailSubjectMobile.Text;						// メール件名(モバイル)
		input.MailBodyMobile = tbMailBodyMobile.Text;							// メール本文(モバイル)
		input.LastChanged = this.LoginOperatorName;								// 最終更新者
		input.AutoSendFlg = rblAutoSendFlg.Text;								// 自動送信フラグ
		input.LineUseFlg = rblSendLineFlg.Text;
		input.LanguageCode = string.Empty;
		input.LanguageLocaleId = string.Empty;
		input.MailTextHtml = tbHtmlBody.Text;
		input.SendHtmlFlg = rbCheckHtml.SelectedValue;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var ddlLanguageCodeSplitValue = ddlLanguageCode.SelectedItem.Value.Split('/');
			input.LanguageCode = ddlLanguageCodeSplitValue[0];
			input.LanguageLocaleId = ddlLanguageCodeSplitValue[1];
		}

		// ショップメッセージタグ置換してから送信元メールアドレスをセット
		var replacedMailFrom = ShopMessageUtil.ConvertShopMessage(tbMailFrom.Text, input.LanguageCode, input.LanguageLocaleId, false);
		input.MailFrom = replacedMailFrom.TrimAllSpaces();
		// ショップメッセージタグ置換してからBccメールアドレスをセット
		var replacedMailBcc = ShopMessageUtil.ConvertShopMessage(tbMailBcc.Text, input.LanguageCode, input.LanguageLocaleId, false);
		input.MailBcc = replacedMailBcc.TrimAllSpaces();

		input.SmsUseFlg = (Constants.GLOBAL_SMS_OPTION_ENABLED)
			? this.chksms.Checked ? MailTemplateModel.SMS_USE_FLG_ON : MailTemplateModel.SMS_USE_FLG_OFF
			: MailTemplateModel.SMS_USE_FLG_OFF;

		input.LineUseFlg = (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED)
			? this.chkline.Checked
				? MailTemplateModel.LINE_USE_FLG_ON
				: MailTemplateModel.LINE_USE_FLG_OFF
			: ((Constants.REPEATLINE_OPTION_ENABLED == Constants.RepeatLineOption.CooperationAndMessaging) && (this.rblSendLineFlg.Text == MailTemplateModel.LINE_USE_FLG_ON))
				? MailTemplateModel.LINE_USE_FLG_ON
				: MailTemplateModel.LINE_USE_FLG_OFF;

		return input;
	}

	/// <summary>
	/// SMS送信チェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void chksms_CheckedChanged(object sender, EventArgs e)
	{
		this.rSmsCarrier.Visible = this.chksms.Checked;
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
	/// Check html change
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbCheckHtml_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.SendHtmlFlg = rbCheckHtml.SelectedValue;

		if (this.SendHtmlFlg == Constants.FLG_MAILTEMPLATE_SEND_HTML_FLG_NOTSEND)
		{
			this.MailTextHtml = tbHtmlBody.Text;
		}

		tbHtmlBody.Text = this.MailTextHtml;
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

	/// <summary>メールID</summary>
	protected string MailId { get; set; }
	/// <summary>テンプレートタグのインデックス</summary>
	protected int SelectTemplateTagIndex { get; set; }
	/// <summary>Mail text html</summary>
	protected string MailTextHtml
	{
		get { return (string)ViewState["MailTextHtml"]; }
		set { ViewState["MailTextHtml"] = value; }
	}
	/// <summary>Send html flg</summary>
	protected string SendHtmlFlg
	{
		get { return (string)ViewState["SendHtmlFlg"]; }
		set { ViewState["SendHtmlFlg"] = value; }
	}
	/// <summary>Is send html flg on</summary>
	protected bool IsSendHtmlFlgOn
	{
		get { return (this.SendHtmlFlg == Constants.FLG_MAILTEMPLATE_SEND_HTML_FLG_SEND); }
	}
}
