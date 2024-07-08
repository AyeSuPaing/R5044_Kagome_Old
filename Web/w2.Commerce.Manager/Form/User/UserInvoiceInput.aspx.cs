/*
=========================================================================================================
  Module      : User Invoice Input Screen Processing (UserInvoiceInput.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.TwUserInvoice;

public partial class Form_User_UserInvoiceInput : ProductPage
{
	# region ラップ済みコントロール宣言
	protected WrappedTextBox WtbInvoiceName { get { return GetWrappedControl<WrappedTextBox>("tbInvoiceName"); } }
	protected WrappedDropDownList WddlTwUniformInvoice { get { return GetWrappedControl<WrappedDropDownList>("ddlTwUniformInvoice"); } }
	protected WrappedDropDownList WddlTwCarryType { get { return GetWrappedControl<WrappedDropDownList>("ddlTwCarryType"); } }
	protected WrappedHtmlGenericControl WtrCarryType { get { return GetWrappedControl<WrappedHtmlGenericControl>("trCarryType"); } }
	protected WrappedTextBox WtbCarryTypeOption_8 { get { return GetWrappedControl<WrappedTextBox>("tbCarryTypeOption_8"); } }
	protected WrappedTextBox WtbCarryTypeOption_16 { get { return GetWrappedControl<WrappedTextBox>("tbCarryTypeOption_16"); } }
	protected WrappedHtmlGenericControl WtrUniformInvoiceOption1 { get { return GetWrappedControl<WrappedHtmlGenericControl>("trUniformInvoiceOption1"); } }
	protected WrappedHtmlGenericControl WtrUniformInvoiceOption2 { get { return GetWrappedControl<WrappedHtmlGenericControl>("trUniformInvoiceOption2"); } }
	protected WrappedTextBox WtbUniformInvoiceOption1_3 { get { return GetWrappedControl<WrappedTextBox>("tbUniformInvoiceOption1_3"); } }
	protected WrappedTextBox WtbUniformInvoiceOption1_8 { get { return GetWrappedControl<WrappedTextBox>("tbUniformInvoiceOption1_8"); } }
	protected WrappedTextBox WtbUniformInvoiceOption2 { get { return GetWrappedControl<WrappedTextBox>("tbUniformInvoiceOption2"); } }
	protected WrappedHtmlGenericControl WcompanyTittle { get { return GetWrappedControl<WrappedHtmlGenericControl>("companyTittle"); } }
	protected WrappedHtmlGenericControl WdonateTittle { get { return GetWrappedControl<WrappedHtmlGenericControl>("donateTittle"); } }
	protected WrappedHtmlGenericControl WdivCarryTypeOption { get { return GetWrappedControl<WrappedHtmlGenericControl>("divCarryTypeOption"); } }
	protected WrappedHtmlGenericControl WdivTwCarryTypeOption_8 { get { return GetWrappedControl<WrappedHtmlGenericControl>("divTwCarryTypeOption_8"); } }
	protected WrappedHtmlGenericControl WdivTwCarryTypeOption_16 { get { return GetWrappedControl<WrappedHtmlGenericControl>("divTwCarryTypeOption_16"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			InitComponents(this.ActionStatus);

			var twUserInvoice = GetTwUserInvoiceForSet();
			Session[Constants.SESSION_KEY_PARAM_FOR_USER_INVOICE_INPUT] = null;
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;
			// ユーザー配送先情報項目セット
			if (twUserInvoice != null)
			{
				this.WtbInvoiceName.Text = twUserInvoice.TwInvoiceName;
				this.WddlTwUniformInvoice.SelectItemByValue(twUserInvoice.TwUniformInvoice);
				this.WddlTwCarryType.SelectItemByValue(twUserInvoice.TwCarryType);
				this.WtbCarryTypeOption_8.Text = this.WtbCarryTypeOption_16.Text = twUserInvoice.TwCarryTypeOption;
				this.WtbUniformInvoiceOption1_3.Text = this.WtbUniformInvoiceOption1_8.Text = twUserInvoice.TwUniformInvoiceOption1;
				this.WtbUniformInvoiceOption2.Text = twUserInvoice.TwUniformInvoiceOption2;
			}

			ddlTwUniformInvoice_SelectedIndexChanged(sender, e);
			ddlTwCarryType_SelectedIndexChanged(sender, e);
		}
	}

	/// <summary>
	/// Get TwUser Invoice For Set
	/// </summary>
	private TwUserInvoiceModel GetTwUserInvoiceForSet()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_UPDATE:
				// ユーザー配送先情報取得
				var sessionTwUserInvoice = (TwUserInvoiceModel)this.Session[Constants.SESSION_KEY_PARAM_FOR_USER_INVOICE_INPUT];
				if ((sessionTwUserInvoice == null) || (this.Session[Constants.SESSION_KEY_PARAM_FOR_BACK] == null)
					|| (sessionTwUserInvoice.TwInvoiceNo.ToString() != StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_INVOICE_NO])))
				{
					var userShipping = new TwUserInvoiceService().Get(this.UserId, this.InvoiceNo);

					return userShipping;
				}
				else
				{
					return sessionTwUserInvoice;
				}

			case Constants.ACTION_STATUS_INSERT:
				if ((this.Session[Constants.SESSION_KEY_PARAM_FOR_USER_INVOICE_INPUT] != null) && (this.Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null))
				{
					var twUserInvoice = (TwUserInvoiceModel)this.Session[Constants.SESSION_KEY_PARAM_FOR_USER_INVOICE_INPUT];

					return twUserInvoice;
				}
				break;
		}

		return null;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	private void InitComponents(string actionStatus)
	{
		if (actionStatus == Constants.ACTION_STATUS_INSERT)
		{
			pRegistInfo.Visible = true;
		}
		else if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			pModifyInfo.Visible = true;
		}

		this.WddlTwUniformInvoice.Items.Add(new ListItem(string.Empty, string.Empty));
		this.WddlTwUniformInvoice.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_TWUSERINVOICE, Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE));
		this.WddlTwCarryType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_TWUSERINVOICE, Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE));
	}

	/// <summary>
	/// User Invoice Display
	/// </summary>
	/// <param name="twUserInvoice">Tw User Invoice</param>
	private void DisplayTwUserInvoice(TwUserInvoiceInput twUserInvoice)
	{
		this.WtbInvoiceName.Text = twUserInvoice.TwInvoiceName;
		this.WddlTwUniformInvoice.SelectItemByValue(twUserInvoice.TwUniformInvoice);
		this.WddlTwCarryType.SelectItemByValue(twUserInvoice.TwCarryType);
		this.WtbCarryTypeOption_8.Text = this.WtbCarryTypeOption_16.Text = twUserInvoice.TwCarryTypeOption;
		this.WtbUniformInvoiceOption1_3.Text = this.WtbUniformInvoiceOption1_8.Text = twUserInvoice.TwUniformInvoiceOption1;
		this.WtbUniformInvoiceOption2.Text = twUserInvoice.TwUniformInvoiceOption2;
	}

	/// <summary>
	/// Get User Invoice Information
	/// </summary>
	/// <returns>User Invoice Information</returns>
	private TwUserInvoiceInput GetTwUserInvoiceInput()
	{
		var userInvoiceInput = new TwUserInvoiceInput()
		{
			UserId = this.UserId,
			TwInvoiceNo = this.InvoiceNo.ToString(),
			TwInvoiceName = this.WtbInvoiceName.Text,
			TwUniformInvoice = this.WddlTwUniformInvoice.SelectedValue,
			TwCarryType = this.WddlTwCarryType.SelectedValue
		};

		switch (this.WddlTwUniformInvoice.SelectedValue)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
				this.WtbUniformInvoiceOption1_3.Text = string.Empty;
				this.WtbUniformInvoiceOption1_8.Text = string.Empty;
				userInvoiceInput.TwCarryType = this.WddlTwCarryType.SelectedValue;

				switch (this.WddlTwCarryType.SelectedValue)
				{
					case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
						userInvoiceInput.TwCarryTypeOption = this.WtbCarryTypeOption_8.Text;
						break;

					case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
						userInvoiceInput.TwCarryTypeOption = this.WtbCarryTypeOption_16.Text;
						break;
				}
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				userInvoiceInput.TwUniformInvoiceOption1 = this.WtbUniformInvoiceOption1_8.Text;
				userInvoiceInput.TwUniformInvoiceOption2 = this.WtbUniformInvoiceOption2.Text;
				this.WtbCarryTypeOption_8.Text = string.Empty;
				this.WtbCarryTypeOption_16.Text = string.Empty;
				userInvoiceInput.TwCarryType = string.Empty;
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				userInvoiceInput.TwUniformInvoiceOption1 = this.WtbUniformInvoiceOption1_3.Text;
				this.WtbCarryTypeOption_8.Text = string.Empty;
				this.WtbCarryTypeOption_16.Text = string.Empty;
				userInvoiceInput.TwCarryType = string.Empty;
				break;
		}

		return userInvoiceInput;
	}

	/// <summary>
	/// Dropdown List Tw Carry Type Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlTwCarryType_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch (this.WddlTwCarryType.SelectedValue)
		{
			case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
				this.WdivTwCarryTypeOption_8.Visible = true;
				this.WdivTwCarryTypeOption_16.Visible = false;
				this.WdivCarryTypeOption.Visible = true;
				break;

			case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
				this.WdivTwCarryTypeOption_16.Visible = true;
				this.WdivTwCarryTypeOption_8.Visible = false;
				this.WdivCarryTypeOption.Visible = true;
				break;

			default:
				this.WdivTwCarryTypeOption_8.Visible = false;
				this.WdivTwCarryTypeOption_16.Visible = false;
				this.WdivCarryTypeOption.Visible = false;
				break;
		}
	}

	/// <summary>
	/// Dropdown List Tw Uniform Invoice Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlTwUniformInvoice_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch (WddlTwUniformInvoice.SelectedValue)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
				this.WtrCarryType.Visible = true;
				this.WtrUniformInvoiceOption1.Visible = false;
				this.WtrUniformInvoiceOption2.Visible = false;
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				this.WtrCarryType.Visible = false;
				this.WtrUniformInvoiceOption1.Visible = true;
				this.WtrUniformInvoiceOption2.Visible = true;
				this.WtbUniformInvoiceOption1_3.Visible = false;
				this.WtbUniformInvoiceOption1_8.Visible = true;
				this.WcompanyTittle.Visible = true;
				this.WdonateTittle.Visible = false;
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				this.WtrCarryType.Visible = false;
				this.WtrUniformInvoiceOption1.Visible = true;
				this.WtrUniformInvoiceOption2.Visible = false;
				this.WtbUniformInvoiceOption1_3.Visible = true;
				this.WtbUniformInvoiceOption1_8.Visible = false;
				this.WcompanyTittle.Visible = false;
				this.WdonateTittle.Visible = true;
				break;

			default:
				this.WtrCarryType.Visible = false;
				this.WtrUniformInvoiceOption1.Visible = false;
				this.WtrUniformInvoiceOption2.Visible = false;
				break;
		}
	}

	/// <summary>
	/// 入力データをチェックし、確認ページに移動します
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnComfirm_Click(object sender, EventArgs e)
	{
		// 入力値格納
		var twUserInvoiceInput = GetTwUserInvoiceInput();

		// 入力チェック
		var errorMessages = twUserInvoiceInput.Validate();

		if ((this.WddlTwUniformInvoice.SelectedValue == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
			&& (errorMessages == string.Empty))
		{
			// Validate carry type
			Regex regex = null;
			switch (this.WddlTwCarryType.SelectedValue)
			{
				case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
					regex = new Regex(Constants.REGEX_MOBILE_CARRY_TYPE_OPTION_8);
					if (regex.IsMatch(this.WtbCarryTypeOption_8.Text.Trim()) == false)
					{
						lbErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MOBILE_CARRY_TYPE_OPTION_8);
						divErrorMessage.Visible = true;

						return;
					}
					break;

				case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
					regex = new Regex(Constants.REGEX_CERTIFICATE_CARRY_TYPE_OPTION_16);
					if (regex.IsMatch(this.WtbCarryTypeOption_16.Text.Trim()) == false)
					{
						lbErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_CERTIFICATE_CARRY_TYPE_OPTION_16);
						divErrorMessage.Visible = true;

						return;
					}
					break;
			}
		}

		if (string.IsNullOrEmpty(errorMessages) ==  false)
		{
			lbErrorMessage.Text = errorMessages;
			divErrorMessage.Visible = true;

			return;
		}
		
		var model = twUserInvoiceInput.CreateModel();
		Session[Constants.SESSION_KEY_PARAM_FOR_USER_INVOICE_INPUT] = model;
	
		// アドレス帳確認画面遷移
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_INVOICE_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_USER_ID, this.UserId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// データをクリア
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReset_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_INVOICE_INPUT)
			.AddParam(Constants.REQUEST_KEY_INVOICE_NO, this.InvoiceNo.ToString());
		if (this.UserId != null) url.AddParam(Constants.REQUEST_KEY_USER_ID, this.UserId);
		if (this.ActionStatus != null) url.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus);

		if (this.InvoiceNo != 0) Session[Constants.SESSION_KEY_PARAM_FOR_USER_INVOICE_INPUT] = null;

		Response.Redirect(url.CreateUrl());
	}

	/// <summary>電子発票管理枝番</summary>
	private int InvoiceNo
	{
		get
		{
			var invoiceNo = 0;
			int.TryParse(Request[Constants.REQUEST_KEY_INVOICE_NO], out invoiceNo);

			return invoiceNo;
		}
	}
	/// <summary>（リクエストから取得できる）ユーザーID</summary>
	protected string UserId
	{
		get { return Request[Constants.REQUEST_KEY_USER_ID]; }
	}
	/// <summary>User Invoice Input</summary>
	protected TwUserInvoiceInput TwUserInvoice
	{
		get { return (TwUserInvoiceInput)Session[Constants.SESSION_KEY_PARAM]; }
		set { Session[Constants.SESSION_KEY_PARAM] = value; }
	}
}