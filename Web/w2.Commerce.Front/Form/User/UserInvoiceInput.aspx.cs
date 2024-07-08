/*
=========================================================================================================
  Module      : User Invoice Input Screen Processing (UserInvoiceInput.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.TwUserInvoice;

public partial class Form_User_UserInvoiceInput : ProductPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }

	# region ラップ済みコントロール宣言
	protected WrappedHtmlGenericControl WpRegistInfo { get { return GetWrappedControl<WrappedHtmlGenericControl>("pRegistInfo"); } }
	protected WrappedHtmlGenericControl WpModifyInfo { get { return GetWrappedControl<WrappedHtmlGenericControl>("pModifyInfo"); } }
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
	/// Page Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_LIST);

		if (!IsPostBack)
		{
			InitComponents();

			// User Invoice Information Item Set
			var twUserInvoice = GetTwUserInvoiceForDisplay();
			if (twUserInvoice != null)
			{
				DisplayTwUserInvoice(twUserInvoice);

				this.WpRegistInfo.Visible = (this.InvoiceNo == 0);
				this.WpModifyInfo.Visible = (this.InvoiceNo != 0);
			}
			else
			{
				this.WpRegistInfo.Visible = true;
			}
			ddlTwUniformInvoice_SelectedIndexChanged(sender, e);
			ddlTwCarryType_SelectedIndexChanged(sender, e);
		}
	}

	/// <summary>
	/// Init Components
	/// </summary>
	private void InitComponents()
	{
		this.WddlTwUniformInvoice.Items.Add(new ListItem(string.Empty, string.Empty));
		this.WddlTwUniformInvoice.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_TWUSERINVOICE, Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE));
		this.WddlTwCarryType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_TWUSERINVOICE, Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE));
	}

	/// <summary>
	/// Get User Invoice Information For Display
	/// </summary>
	/// <returns>User Invoice Information</returns>
	private TwUserInvoiceInput GetTwUserInvoiceForDisplay()
	{
		// Target page information is set when the return button on the user invoice edit screen is display
		if (this.SessionParamTargetPage == Constants.PAGE_FRONT_USER_INVOICE_INPUT)
		{
			// If there is target page information the user invoice information exists in the session
			var twUserInvoice = this.TwUserInvoice;

			// Clear target page information
			this.SessionParamTargetPage = null;

			return twUserInvoice;
		}
		
		// If session information does not exits, get value from DB with the parameter value
		// (If the edit button is click from the user invoice list, the invoice no is included in the parameter
		if (this.InvoiceNo != 0)
		{
			var twUserInvoice = new TwUserInvoiceService().Get(this.LoginUserId, this.InvoiceNo);
			if (twUserInvoice != null)
			{
				return new TwUserInvoiceInput(twUserInvoice);
			}
		}

		return null;
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
	/// User Invoice Input Screen URL Creation
	/// </summary>
	/// <returns>URL</returns>
	protected string CreateTwUserInputUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_INPUT)
			.AddParam(Constants.REQUEST_KEY_SHIPPING_NO, this.InvoiceNo.ToString())
			.CreateUrl();

		return url;
	}

	/// <summary>
	/// 確認するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbConfirm_Click(object sender, EventArgs e)
	{
		// 入力値格納
		var twUserInvoiceInput = GetTwUserInvoiceInput();

		// 入力チェック
		var errorMessages = twUserInvoiceInput.Validate();

		if ((this.WddlTwUniformInvoice.SelectedValue == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
			&& (errorMessages.Count == 0))
		{
			// Validate carry type
			Regex regex = null;
			switch (this.WddlTwCarryType.SelectedValue)
			{
				case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
					regex = new Regex(Constants.REGEX_MOBILE_CARRY_TYPE_OPTION_8);
					if (regex.IsMatch(this.WtbCarryTypeOption_8.Text.Trim()) == false)
					{
						errorMessages[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_8"]
							= WebMessages.GetMessages(WebMessages.ERRMSG_MOBILE_CARRY_TYPE_OPTION_8);
					}
					break;

				case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
					regex = new Regex(Constants.REGEX_CERTIFICATE_CARRY_TYPE_OPTION_16);
					if (regex.IsMatch(this.WtbCarryTypeOption_16.Text.Trim()) == false)
					{
						errorMessages[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_16"]
							= WebMessages.GetMessages(WebMessages.ERRMSG_CERTIFICATE_CARRY_TYPE_OPTION_16);
					}
					break;
			}
		}

		if (errorMessages.Count != 0)
		{
			var customValidators = new List<CustomValidator>();
			CreateCustomValidators(this, customValidators);

			SetControlViewsForError(this.ValidationGroup, errorMessages, customValidators);

			return;
		}

		this.TwUserInvoice = twUserInvoiceInput;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_CONFIRM);
	}

	/// <summary>
	/// Dropdown List Tw Uniform Invoice Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlTwUniformInvoice_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch(WddlTwUniformInvoice.SelectedValue)
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
	/// Dropdown List Tw Carry Type Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlTwCarryType_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch(this.WddlTwCarryType.SelectedValue)
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
	/// Get User Invoice Information
	/// </summary>
	/// <returns>User Invoice Information</returns>
	private TwUserInvoiceInput GetTwUserInvoiceInput()
	{
		var userInvoiceInput = new TwUserInvoiceInput()
		{
			UserId = this.LoginUserId,
			TwInvoiceNo = this.InvoiceNo.ToString(),
			TwInvoiceName = this.WtbInvoiceName.Text,
			TwUniformInvoice = this.WddlTwUniformInvoice.SelectedValue
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
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				userInvoiceInput.TwUniformInvoiceOption1 = this.WtbUniformInvoiceOption1_3.Text;
				this.WtbCarryTypeOption_8.Text = string.Empty;
				this.WtbCarryTypeOption_16.Text = string.Empty;
				break;
		}

		return userInvoiceInput;
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
	/// <summary>User Invoice Input</summary>
	protected TwUserInvoiceInput TwUserInvoice
	{
		get { return (TwUserInvoiceInput)Session[Constants.SESSION_KEY_PARAM]; }
		set { Session[Constants.SESSION_KEY_PARAM] = value; }
	}
	/// <summary>バリデーショングループ</summary>
	public string ValidationGroup
	{
		get { return "TwUserInvoice"; }
	}
}