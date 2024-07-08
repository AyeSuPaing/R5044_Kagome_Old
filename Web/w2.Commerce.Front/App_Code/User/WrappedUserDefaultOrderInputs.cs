/*
=========================================================================================================
  Module      : ラップ済みデフォルト注文方法設定情報入力クラス(WrappedUserDefaultOrderInputs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Global;
using w2.App.Common.Order;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.TwUserInvoice;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserShipping;

/// <summary>
/// ラップ済みデフォルト注文方法設定情報入力クラス
/// </summary>
public class WrappedUserDefaultOrderInputs
{
	/// <summary>ベースページ</summary>
	private readonly CommonPage m_commonPage = null;
	/// <summary>親コントロール</summary>
	private readonly Control m_cParent = null;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="basePage">ベースページ</param>
	/// <param name="riParent">親コントロール（カート内であれば決済リピータアイテム、それ以外はContentPlaceHolder）</param>
	public WrappedUserDefaultOrderInputs(CommonPage basePage, RepeaterItem riParent = null)
	{
		m_commonPage = basePage;
		m_cParent = riParent
			?? ((basePage.Master != null)
				? (basePage.Master.FindControl("ContentPlaceHolder1") ?? basePage.Master.FindControl("ContentPlaceHolderBody"))
				: basePage);
	}

	/// <summary>
	/// デフォルト支払方法を表示する
	/// </summary>
	/// <param name="paymentName">決済種別名</param>
	public void DisplayUserDefaultPaymentName(string paymentName)
	{
		this.WrTrDefaultPayment.Visible = true;
		this.WlDefaultPayment.Text = WebSanitizer.HtmlEncode(paymentName);
	}

	/// <summary>
	/// デフォルト配送方法を表示する（注文者情報）
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	public void DisplayUserDefaultOwnerShippingSetting(UserModel user)
	{
		this.WrDivUserShippingInfo.Visible = true;
		this.WrTrDefaultShippingInfo.Visible = true;
		this.WlShippigName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSettingOwnerShippingDisp);
		this.WlShippingName1.Text = WebSanitizer.HtmlEncode(user.Name1);
		this.WlShippingName2.Text = WebSanitizer.HtmlEncode(user.Name2);
		this.WlShippingNameKana1.Text = WebSanitizer.HtmlEncode(user.NameKana1);
		this.WlShippingNameKana2.Text = WebSanitizer.HtmlEncode(user.NameKana2);
		this.WlShippingAddr1.Text = WebSanitizer.HtmlEncode(user.Addr1);
		this.WlShippingAddr2.Text = WebSanitizer.HtmlEncode(user.Addr2);
		this.WlShippingAddr3.Text = WebSanitizer.HtmlEncode(user.Addr3);
		this.WlShippingAddr4.Text = WebSanitizer.HtmlEncode(user.Addr4);
		this.WlShippingAddr5.Text = WebSanitizer.HtmlEncode(user.Addr5);
		this.WlShippingCountryName.Text = WebSanitizer.HtmlEncode(user.AddrCountryName);
		this.WlShippingCompanyName.Text = WebSanitizer.HtmlEncode(user.CompanyName);
		this.WlShippingCompanyPostName.Text = WebSanitizer.HtmlEncode(user.CompanyPostName);
		this.WlShippingTel1.Text = WebSanitizer.HtmlEncode(user.Tel1);

		if (GlobalAddressUtil.IsCountryJp(user.AddrCountryIsoCode))
		{
			this.WlShippingZip.Text = WebSanitizer.HtmlEncode(user.Zip);
			this.WlShippingZipGlobal.Text = string.Empty;
		}
		else
		{
			this.WlShippingZip.Text = string.Empty;
			this.WlShippingZipGlobal.Text = WebSanitizer.HtmlEncode(user.Zip);
		}
		this.ShippingCountryIsoCode = user.AddrCountryIsoCode;
	}

	/// <summary>
	/// デフォルト配送方法を表示する（アドレス帳情報）
	/// </summary>
	/// <param name="userShipping">ユーザー配送先情報</param>
	public void DisplayUserDefaultShippingSetting(UserShippingModel userShipping)
	{
		this.WrDivUserShippingInfo.Visible = true;
		this.WrTrDefaultShippingInfo.Visible = true;
		this.WlShippigName.Text = WebSanitizer.HtmlEncode(userShipping.Name);
		this.WlShippingName1.Text = WebSanitizer.HtmlEncode(userShipping.ShippingName1);
		this.WlShippingName2.Text = WebSanitizer.HtmlEncode(userShipping.ShippingName2);
		this.WlShippingNameKana1.Text = WebSanitizer.HtmlEncode(userShipping.ShippingNameKana1);
		this.WlShippingNameKana2.Text = WebSanitizer.HtmlEncode(userShipping.ShippingNameKana2);
		this.WlShippingAddr1.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr1);
		this.WlShippingAddr2.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr2);
		this.WlShippingAddr3.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr3);
		this.WlShippingAddr4.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr4);
		this.WlShippingAddr5.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr5);
		this.WlShippingCountryName.Text = WebSanitizer.HtmlEncode(userShipping.ShippingCountryName);
		this.WlShippingCompanyName.Text = WebSanitizer.HtmlEncode(userShipping.ShippingCompanyName);
		this.WlShippingCompanyPostName.Text = WebSanitizer.HtmlEncode(userShipping.ShippingCompanyPostName);
		this.WlShippingTel1.Text = WebSanitizer.HtmlEncode(userShipping.ShippingTel1);

		if (GlobalAddressUtil.IsCountryJp(userShipping.ShippingCountryIsoCode))
		{
			this.WlShippingZip.Text = WebSanitizer.HtmlEncode(userShipping.ShippingZip);
			this.WlShippingZipGlobal.Text = string.Empty;
		}
		else
		{
			this.WlShippingZip.Text = string.Empty;
			this.WlShippingZipGlobal.Text = WebSanitizer.HtmlEncode(userShipping.ShippingZip);
		}
		this.ShippingCountryIsoCode = userShipping.ShippingCountryIsoCode;
	}

	/// <summary>
	/// デフォルトクレジットカード情報を表示する
	/// </summary>
	/// <param name="userCreditCard">ユーザークレジットカード情報</param>
	public void DisplayUserDefaultCreditCardSetting(UserCreditCardModel userCreditCard)
	{
		this.WrDivUserCreditCardInfo.Visible = true;
		this.WrDivUserCreditCardInfo.Visible = true;
		this.WlCardDispName.Text = WebSanitizer.HtmlEncode(userCreditCard.CardDispName);
		this.WlCardCompanyName.Text = WebSanitizer.HtmlEncode(OrderCommon.GetCreditCardCompanyName(userCreditCard.CompanyCode));
		this.WlLastFourDigit.Text = WebSanitizer.HtmlEncode(userCreditCard.LastFourDigit);
		this.WlExpirationMonth.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpirationMonth);
		this.WlExpirationYear.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpirationYear);
		this.WlAuthorName.Text = WebSanitizer.HtmlEncode(userCreditCard.AuthorName);
	}

	/// <summary>
	/// デフォルト楽天前払い支払いコンビニを表示する
	/// </summary>
	/// <param name="cvsType">楽天前払い支払いコンビニ</param>
	public void DisplayUserDefaultRakutenCvsType(string cvsType)
	{
		if (string.IsNullOrEmpty(cvsType))
		{
			this.WtrCvsType.Visible = false;
			return;
		}

		var cvsTypeList = ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.PAYMENT_RAKUTEN_CVS_TYPE).Cast<ListItem>().ToArray();
		var cvsTypeName = (cvsTypeList.First(item => (item.Value == cvsType))).Text;

		this.WlCvsTypeName.Text = cvsTypeName;
		this.WhfCvsType.Value = WebSanitizer.HtmlEncode(cvsType);
	}

	/// <summary>
	/// デフォルト注文方法が未登録の表示
	/// </summary>
	public void DisplayNoUserDefaultOrderSetting()
	{
		this.WlShippigName.Text = this.UserDefaultOrderSettingNoneDisp;
		this.WlDefaultPayment.Text = this.UserDefaultOrderSettingNoneDisp;
		this.WlInvoiceName.Text = this.UserDefaultOrderSettingNoneDisp;
		this.WrTrDefaultShippingInfo.Visible = false;
		this.WrDivUserCreditCardInfo.Visible = false;
		this.WtrDefaultInvoiceInfo.Visible = false;
		this.WtrCvsType.Visible = false;
	}

	/// <summary>
	/// アドレス帳情報がない場合の表示
	/// </summary>
	public void DisplayNoUserShipping()
	{
		this.WrTrDefaultShippingInfo.Visible = false;
		this.WlShippigName.Text = this.UserDefaultOrderSettingNoneDisp;
	}

	/// <summary>
	/// Display No User Invoice
	/// </summary>
	public void DisplayNoUserInvoice()
	{
		this.WtrDefaultInvoiceInfo.Visible = false;
		this.WlInvoiceName.Text = this.UserDefaultOrderSettingNoneDisp;
	}

	/// <summary>
	/// クレジットカード情報がない場合の表示
	/// </summary>
	public void DisplayNoUserCreditCard()
	{
		this.WrDivUserCreditCardInfo.Visible = false;
		this.WlDivCreditCardNoErrorMessage.Visible = true;
		this.WlCreditCardNoErrorMessage.Text
			= WebSanitizer.HtmlEncode(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_CREDIT_CARD));
	}

	/// <summary>
	/// 後付款(TriLink後払い)設定時に配送先が台湾でない場合の表示
	/// </summary>
	public void DisplayTryLinkAfterPayErrorMessage()
	{
		this.WlTryLinkAfterPayErrorMessage.Text
			= WebSanitizer.HtmlEncode(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY));
	}

	/// <summary>
	/// エラーメッセージ表示：クレジットカード未登録
	/// </summary>
	public void ShowCreditCardNoErrorMessage()
	{
		this.WlDivCreditCardNoErrorMessage.Visible = true;
		this.WlCreditCardNoErrorMessage.Text
			= WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USERCREDITCARD_NO_CARD));
		this.WlRegistCreditCardErrorMessage.Text
			= WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_DEFAULT_ORDER_SETTING_USERCREDITCARD_NO_CARD));
	}

	/// <summary>
	/// エラーメッセージ初期化
	/// </summary>
	public void ResetErrorMessage()
	{
		this.WlCreditCardNoErrorMessage.Text = string.Empty;
		this.WlRegistCreditCardErrorMessage.Text = string.Empty;
		this.WlTryLinkAfterPayErrorMessage.Text = string.Empty;
	}

	/// <summary>
	/// Display User Default Invoice Setting
	/// </summary>
	/// <param name="userInvoice">User Invoice Information</param>
	public void DisplayUserDefaultInvoiceSetting(TwUserInvoiceModel userInvoice)
	{
		this.WtrDefaultInvoiceInfo.Visible = true;
		this.WlInvoiceName.Text = WebSanitizer.HtmlEncode(StringUtility.ToEmpty(userInvoice.TwInvoiceName));
		this.WlUniformInvoiceInformation.Text = WebSanitizer.HtmlEncode(string.Format("{0}：{1}",
			CommonPage.ReplaceTag("@@TwInvoice.uniform_invoice.name@@"),
			ValueText.GetValueText(Constants.TABLE_TWUSERINVOICE, Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE, userInvoice.TwUniformInvoice)));
		this.WlUniformInvoiceTypeOption1.Text = string.Empty;
		this.WlUniformInvoiceTypeOption2.Text = string.Empty;
		this.WlCarryTypeInformation.Text = string.Empty;

		switch (userInvoice.TwUniformInvoice)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				this.WlCarryTypeInformation.Visible = false;
				this.WlUniformInvoiceTypeOption1.Visible = true;
				this.WlUniformInvoiceTypeOption2.Visible = true;

				this.WlUniformInvoiceTypeOption1.Text = WebSanitizer.HtmlEncode(string.Format("{0}：{1}",
					CommonPage.ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@"),
					userInvoice.TwUniformInvoiceOption1));
				this.WlUniformInvoiceTypeOption2.Text = WebSanitizer.HtmlEncode(string.Format("{0}：{1}",
					CommonPage.ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@"),
					userInvoice.TwUniformInvoiceOption2));
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				this.WlCarryTypeInformation.Visible = false;
				this.WlUniformInvoiceTypeOption1.Visible = true;
				this.WlUniformInvoiceTypeOption2.Visible = false;

				this.WlUniformInvoiceTypeOption1.Text = WebSanitizer.HtmlEncode(string.Format("{0}：{1}",
					CommonPage.ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@"), userInvoice.TwUniformInvoiceOption1));
				break;

			default:
				this.WlCarryTypeInformation.Visible = true;
				this.WlUniformInvoiceTypeOption1.Visible = false;
				this.WlUniformInvoiceTypeOption2.Visible = false;
				this.WlCarryTypeInformation.Text = WebSanitizer.HtmlEncode(string.Format("{0}：{1}",
					CommonPage.ReplaceTag("@@TwInvoice.carry_type.name@@"),
					GetInformationInvoice(
						userInvoice.TwCarryType,
						userInvoice.TwCarryTypeOption)));
				break;
		}
	}

	/// <summary>
	/// <summary>
	/// Get Information Invoice
	/// </summary>
	/// <param name="carryType">Carry Type</param>
	/// <param name="carryTypeOption">Carry Type Option</param>
	/// <returns>Information Invoice</returns>
	protected string GetInformationInvoice(string carryType, string carryTypeOption)
	{
		var result = string.IsNullOrEmpty(carryType)
			? ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, carryType)
			: string.Format("{0}：{1}",
				ValueText.GetValueText(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, carryType),
				carryTypeOption);

		return result;
	}

	/// <summary>
	/// 前払い支払いコンビニ情報がない場合の表示
	/// </summary>
	public void DisplayNoUserDefaultCvsType()
	{
		this.WtrCvsType.Visible = false;
		this.WlCvsTypeName.Text = string.Empty;
		this.WhfCvsType.Value = string.Empty;
	}

	/// <summary>
	/// デフォルト 前払い支払いコンビニを表示する
	/// </summary>
	/// <param name="cvsType">前払い支払いコンビニ</param>
	public void DisplayUserDefaultCvsType(string cvsType)
	{
		if (string.IsNullOrEmpty(cvsType)
			|| ((OrderCommon.IsPaymentCvsTypeZeus == false)
				&& (OrderCommon.IsPaymentCvsTypePaygent == false)))
		{
			DisplayNoUserDefaultCvsType();
			return;
		}

		var cvsTypeName = ValueText.GetValueText(
			Constants.TABLE_ORDER,
			OrderCommon.CvsCodeValueTextFieldName,
			cvsType);
		this.WlCvsTypeName.Text = WebSanitizer.HtmlEncode(cvsTypeName);
		this.WhfCvsType.Value = cvsType;
		this.WtrCvsType.Visible = true;
	}

	/// <summary>配送先名</summary>
	public WrappedLiteral WlShippigName
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingName"); }
	}
	/// <summary>配送先氏名1</summary>
	public WrappedLiteral WlShippingName1
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingName1"); }
	}
	/// <summary>配送先氏名2</summary>
	public WrappedLiteral WlShippingName2
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingName2"); }
	}
	/// <summary>配送先氏名かな1</summary>
	public WrappedLiteral WlShippingNameKana1
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingNameKana1"); }
	}
	/// <summary>配送先氏名かな2</summary>
	public WrappedLiteral WlShippingNameKana2
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingNameKana2"); }
	}
	/// <summary>配送先郵便番号</summary>
	public WrappedLiteral WlShippingZip
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingZip"); }
	}
	/// <summary>配送先住所1</summary>
	public WrappedLiteral WlShippingAddr1
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingAddr1"); }
	}
	/// <summary>配送先住所2</summary>
	public WrappedLiteral WlShippingAddr2
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingAddr2"); }
	}
	/// <summary>配送先住所3</summary>
	public WrappedLiteral WlShippingAddr3
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingAddr3"); }
	}
	/// <summary>配送先住所4</summary>
	public WrappedLiteral WlShippingAddr4
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingAddr4"); }
	}
	/// <summary>配送先住所5</summary>
	public WrappedLiteral WlShippingAddr5
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingAddr5"); }
	}
	/// <summary>配送先国</summary>
	public WrappedLiteral WlShippingCountryName
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingCountryName"); }
	}
	/// <summary>配送先郵便番号(海外)</summary>
	public WrappedLiteral WlShippingZipGlobal
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingZipGlobal"); }
	}
	/// <summary>配送先企業名</summary>
	public WrappedLiteral WlShippingCompanyName
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingCompanyName"); }
	}
	/// <summary>配送先部署名</summary>
	public WrappedLiteral WlShippingCompanyPostName
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingCompanyPostName"); }
	}
	/// <summary>配送先電話番号1</summary>
	public WrappedLiteral WlShippingTel1
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingTel1"); }
	}
	/// <summary>クレジットカード登録名</summary>
	public WrappedLiteral WlCardDispName
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lCardDispName"); }
	}
	/// <summary>カード会社名</summary>
	public WrappedLiteral WlCardCompanyName
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lCardCompanyName"); }
	}
	/// <summary>カード番号下4桁</summary>
	public WrappedLiteral WlLastFourDigit
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lLastFourDigit"); }
	}
	/// <summary>有効期限（月）</summary>
	public WrappedLiteral WlExpirationMonth
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lExpirationMonth"); }
	}
	/// <summary>有効期限（年）</summary>
	public WrappedLiteral WlExpirationYear
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lExpirationYear"); }
	}
	/// <summary>カード名義</summary>
	public WrappedLiteral WlAuthorName
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lAuthorName"); }
	}
	/// <summary>デフォルト支払方法</summary>
	public WrappedLiteral WlDefaultPayment
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lDefaultPayment"); }
	}
	/// <summary>デフォルト支払方法ID</summary>
	public WrappedLiteral WhfDefaultPaymentId
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "hfDefaultPaymentId"); }
	}
	/// <summary>クレジットカード未登録エラーメッセージ</summary>
	public WrappedLiteral WlCreditCardNoErrorMessage
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lCreditCardNoErrorMessage"); }
	}
	/// <summary>後付款(TriLink後払い)エラーメッセージ</summary>
	public WrappedLiteral WlTryLinkAfterPayErrorMessage
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lTryLinkAfterPayErrorMessage"); }
	}
	/// <summary>クレジットカード登録必須メッセージ<</summary>
	public WrappedLiteral WlRegistCreditCardErrorMessage
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lRegistCreditCardErrorMessage"); }
	}
	/// <summary>配送先情報</summary>
	public WrappedControl WrDivUserShippingInfo
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "dvUserShippingInfo"); }
	}
	/// <summary>クレジットカード情報</summary>
	public WrappedControl WrDivUserCreditCardInfo
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "dvUserCreditCardInfo"); }
	}
	/// <summary>PayPal情報</summary>
	public WrappedControl WcDivPayPalInfo
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "divPayPalInfo"); }
	}
	/// <summary>デフォルト配送先名</summary>
	public WrappedControl WrTrDefaultShippingName
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "trDefaultShippingName"); }
	}
	/// <summary>デフォルト配送先情報</summary>
	public WrappedControl WrTrDefaultShippingInfo
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "trDefaultShippingInfo"); }
	}
	/// <summary>デフォルトクレジットカード登録名</summary>
	public WrappedControl WrTrDefaultCardDispName
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "trDefaultCardDispName"); }
	}
	/// <summary>マイページクレジットカード登録リンク</summary>
	public WrappedControl WlDivCreditCardNoErrorMessage
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "dvCreditCardNoErrorMessageLink"); }
	}
	/// <summary>デフォルト配送先方法</summary>
	public WrappedControl WrTrDefaultShipping
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "trDefaultShipping"); }
	}
	/// <summary>デフォルト支払方法</summary>
	public WrappedControl WrTrDefaultPayment
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "trDefaultPayment"); }
	}
	/// <summary>デフォルト配送先ドロップダウンリスト</summary>
	public WrappedDropDownList WrDdlDefaultShipping
	{
		get { return m_commonPage.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlDefaultShipping"); }
	}
	/// <summary>デフォルト支払方法ドロップダウンリスト</summary>
	public WrappedDropDownList WrDdlDefaultPayment
	{
		get { return m_commonPage.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlDefaultPayment"); }
	}
	/// <summary>デフォルトクレジットカード登録名ドロップダウンリスト</summary>
	public WrappedDropDownList WrDdlUserCreditCardName
	{
		get { return m_commonPage.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlDefaultUserCreditCardName"); }
	}
	/// <summary>決済種別ID</summary>
	public WrappedHiddenField WlHfPaymentId
	{
		get { return m_commonPage.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfPaymentId"); }
	}
	/// <summary>配送先枝番</summary>
	public WrappedHiddenField WlHfShippingNo
	{
		get { return m_commonPage.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfShippingNo"); }
	}
	/// <summary>クレジットカード枝番</summary>
	public WrappedHiddenField WlHfUserCreditCardBranchNo
	{
		get { return m_commonPage.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfUserCreditCardBranchNo"); }
	}
	/// <summary>配送先ISOコード</summary>
	public string ShippingCountryIsoCode { get; set; }
	/// <summary>配送不可の国エラーメッセージ</summary>
	public WrappedLiteral WlShippingCountryErrorMessage
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lShippingCountryErrorMessage"); }
	}
	/// <summary>デフォルト注文方法指定表示用文言：注文者情報の住所</summary>
	public string UserDefaultOrderSettingOwnerShippingDisp
	{
		get
		{
			return ValueText.GetValueText(
				Constants.TABLE_USERDEFAULTORDERSETTING,
				Constants.FIELD_USER_DEFAULT_ORDER_SETTING_OWNER_SHIPPING,
				Constants.FIELD_USER_DEFAULT_ORDER_SETTING_OWNER_SHIPPING_VALUE);
		}
	}
	/// <summary>デフォルト注文方法指定表示用文言：指定なし</summary>
	public string UserDefaultOrderSettingNoneDisp
	{
		get
		{
			return ValueText.GetValueText(
				Constants.TABLE_USERDEFAULTORDERSETTING,
				Constants.FIELD_USER_DEFAULT_ORDER_SETTING_NONE,
				Constants.FIELD_USER_DEFAULT_ORDER_SETTING_NONE_VALUE);
		}
	}
	/// <summary>Default Invoice Information</summary>
	public WrappedControl WtrDefaultInvoiceInfo
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "trDefaultInvoiceInfo"); }
	}
	/// <summary>Invoice Name</summary>
	public WrappedLiteral WlInvoiceName
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lInvoiceName"); }
	}
	/// <summary>Invoice Branch Number</summary>
	public WrappedHiddenField WhfInvoiceNo
	{
		get { return m_commonPage.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfInvoiceNo"); }
	}
	/// <summary>Default Invoice Dropdown List</summary>
	public WrappedDropDownList WddlDefaultInvoice
	{
		get { return m_commonPage.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlDefaultInvoice"); }
	}
	/// <summary>Uniform Invoice</summary>
	public WrappedLiteral WlUniformInvoiceInformation
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lUniformInvoiceInformation"); }
	}
	/// <summary>Carry Type</summary>
	public WrappedLiteral WlCarryTypeInformation
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lCarryTypeInformation"); }
	}
	/// <summary>Uniform Invoice Type Option 1</summary>
	public WrappedLiteral WlUniformInvoiceTypeOption1
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lUniformInvoiceTypeOption1"); }
	}
	/// <summary>Uniform Invoice Type Option 2</summary>
	public WrappedLiteral WlUniformInvoiceTypeOption2
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lUniformInvoiceTypeOption2"); }
	}
	/// <summary>デフォルト支払いコンビニ</summary>
	public WrappedControl WtrCvsType
	{
		get { return m_commonPage.GetWrappedControl<WrappedControl>(m_cParent, "trCvsType"); }
	}
	/// <summary>楽天コンビニ前払い支払いコンビニドロップダウンリスト</summary>
	public WrappedDropDownList WddlRakutenCvsType
	{
		get { return m_commonPage.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlRakutenCvsType"); }
	}
	/// <summary>コンビニ前払い支払いコンビニ名称</summary>
	public WrappedLiteral WlCvsTypeName
	{
		get { return m_commonPage.GetWrappedControl<WrappedLiteral>(m_cParent, "lCvsTypeName"); }
	}
	/// <summary>コンビニ前払い支払いコンビニ</summary>
	public WrappedHiddenField WhfCvsType
	{
		get { return m_commonPage.GetWrappedControl<WrappedHiddenField>(m_cParent, "hfCvsType"); }
	}
	/// <summary>Zeus コンビニ前払い支払いコンビニドロップダウンリスト</summary>
	public WrappedDropDownList WddlZeusCvsType
	{
		get { return m_commonPage.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlZeusCvsType"); }
	}
	/// <summary>Paygent コンビニ前払い支払いコンビニドロップダウンリスト</summary>
	public WrappedDropDownList WddlPaygentCvsType
	{
		get { return m_commonPage.GetWrappedControl<WrappedDropDownList>(m_cParent, "ddlPaygentCvsType"); }
	}
}
