/*
=========================================================================================================
  Module      : 注文方法設定入力確認画面(UserShippingConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;
using w2.Domain.UserDefaultOrderSetting;

public partial class Form_User_UserDefaultOrderSettingConfirm : UserPage
{
	# region ラップ済みコントロール宣言
	private WrappedControl WucPaymentDescriptionCvsDef { get { return GetWrappedControl<WrappedControl>("ucPaymentDescriptionCvsDef"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// HTTPS通信チェック（HTTPのとき、注文方法設定入力画面へ）
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_INPUT);

		if (!IsPostBack)
		{
			// デフォルト注文方法情報の表示/非表示設定
			this.WrappedUserDefaultOrderInput.WrTrDefaultPayment.Visible = this.IsSelectedDefaultPaymentOn;
			this.WrappedUserDefaultOrderInput.WrTrDefaultShippingInfo.Visible = this.IsSelectedDefaultShippingOn;
			this.WrappedUserDefaultOrderInput.WrDivUserCreditCardInfo.Visible = (this.IsSelectedDefaultPaymentOn && this.IsSelectedPaymentCreditCard);
			this.WrappedUserDefaultOrderInput.WtrDefaultInvoiceInfo.Visible = this.IsSelectedDefaultInvoiceOn;

			// デフォルト注文方法入力情報を表示する
			DisplayUserDefaultOrderSettingInput();

			// デフォルト注文方法情報がコンビニ後払いの場合は、注意書きを表示
			this.WucPaymentDescriptionCvsDef.Visible = (this.IsSelectedDefaultPaymentOn && this.IsSelectedPaymentCvsDef);

			this.DataBind();
		}
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdate_Click(object sender, EventArgs e)
	{
		// デフォルト注文方法を登録/更新し、注文方法設定完了画面へリダイレクトする
		ExecInsertOrUpdateDefaultOrderSetting();
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_LIST);
	}

	/// <summary>
	/// デフォルト注文方法入力情報を表示する
	/// </summary>
	private void DisplayUserDefaultOrderSettingInput()
	{
		this.WrappedUserDefaultOrderInput.WlHfPaymentId.Value = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.PaymentId);

		// 決済種別名を翻訳
		var paymentName = this.UserDefaultOrderSetting.PaymentName;

		// 既定の支払方法が指定なしの場合は翻訳処理を行わない
		if (Constants.GLOBAL_OPTION_ENABLE && (paymentName != this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp))
		{
			var beforeTranslationPaymentName = DataCacheControllerFacade.GetPaymentCacheController().GetPaymentName(this.UserDefaultOrderSetting.PaymentId);
			paymentName = NameTranslationCommon.GetTranslationName(
				this.UserDefaultOrderSetting.PaymentId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PAYMENT_PAYMENT_NAME,
				beforeTranslationPaymentName);
		}
		this.WrappedUserDefaultOrderInput.WlDefaultPayment.Text = WebSanitizer.HtmlEncode(paymentName);

		if (this.IsSelectedRakutenCvsPre)
		{
			this.WrappedUserDefaultOrderInput.WlCvsTypeName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.RakutenCvsTypeName);
			this.WrappedUserDefaultOrderInput.WhfCvsType.Value = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.RakutenCvsType);
		}

		// Payment method is Zeus or Paygent convenience store prepayment
		if (this.IsSelectedZeusCvsPre || this.IsSelectedPaygentCvsPre)
		{
			this.WrappedUserDefaultOrderInput.WlCvsTypeName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.CvsTypeName);
			this.WrappedUserDefaultOrderInput.WhfCvsType.Value = this.UserDefaultOrderSetting.CvsType;
			this.WrappedUserDefaultOrderInput.WtrCvsType.Visible = true;
		}
		else
		{
			this.WrappedUserDefaultOrderInput.WtrCvsType.Visible = false;
		}

		this.WrappedUserDefaultOrderInput.WlShippigName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingName);
		this.WrappedUserDefaultOrderInput.WlShippingName1.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingName1);
		this.WrappedUserDefaultOrderInput.WlShippingName2.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingName2);
		this.WrappedUserDefaultOrderInput.WlShippingNameKana1.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingNameKana1);
		this.WrappedUserDefaultOrderInput.WlShippingNameKana2.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingNameKana2);
		this.WrappedUserDefaultOrderInput.WlShippingAddr1.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingAddress1);
		this.WrappedUserDefaultOrderInput.WlShippingAddr2.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingAddress2);
		this.WrappedUserDefaultOrderInput.WlShippingAddr3.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingAddress3);
		this.WrappedUserDefaultOrderInput.WlShippingAddr4.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingAddress4);
		this.WrappedUserDefaultOrderInput.WlShippingAddr5.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingAddress5);
		this.WrappedUserDefaultOrderInput.WlShippingCountryName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingCountryName);
		this.WrappedUserDefaultOrderInput.WlShippingCompanyName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingCompanyName);
		this.WrappedUserDefaultOrderInput.WlShippingCompanyPostName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingCompanyPostName);
		this.WrappedUserDefaultOrderInput.WlShippingTel1.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingTel1);
		this.WrappedUserDefaultOrderInput.WlCardDispName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.UserCreditCardCartDispName);
		this.WrappedUserDefaultOrderInput.WlCardCompanyName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.UserCreditCardCompanyName);
		this.WrappedUserDefaultOrderInput.WlLastFourDigit.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.UserCreditCardLastFourDigit);
		this.WrappedUserDefaultOrderInput.WlExpirationMonth.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.UserCreditCardExpirationMonth);
		this.WrappedUserDefaultOrderInput.WlExpirationYear.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.UserCreditCardExpirationYear);
		this.WrappedUserDefaultOrderInput.WlAuthorName.Text = WebSanitizer.HtmlEncode((string)this.UserDefaultOrderSetting.UserCreditCardAuthorName);
		this.WrappedUserDefaultOrderInput.WlShippingZip.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingZip);
		this.WrappedUserDefaultOrderInput.WlShippingZipGlobal.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.ShippingZip);
		this.IsShippingAddrJp = IsCountryJp(this.UserDefaultOrderSetting.ShippingCountryIsoCode);

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			this.WrappedUserDefaultOrderInput.WlInvoiceName.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.InvoiceName);
			this.WrappedUserDefaultOrderInput.WlUniformInvoiceInformation.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.UniformInvoiceInformation);
			this.WrappedUserDefaultOrderInput.WlCarryTypeInformation.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.CarryTypeInformation);
			this.WrappedUserDefaultOrderInput.WlUniformInvoiceTypeOption1.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.UniformInvoiceTypeOption1);
			this.WrappedUserDefaultOrderInput.WlUniformInvoiceTypeOption2.Text = WebSanitizer.HtmlEncode(this.UserDefaultOrderSetting.UniformInvoiceTypeOption2);
		}
	}

	/// <summary>
	/// デフォルト注文方法を登録/更新する。
	/// </summary>
	private void ExecInsertOrUpdateDefaultOrderSetting()
	{
		var paymentId = (this.IsSelectedDefaultPaymentOn) ? this.WrappedUserDefaultOrderInput.WlHfPaymentId.Value : null;
		var shippingNo = (this.IsSelectedDefaultOwnerShippingOn)
			? 0 : (this.IsSelectedDefaultShippingOn && (this.WrappedUserDefaultOrderInput.WlShippigName.Text != null))
			? (int?)int.Parse(this.UserDefaultOrderSetting.ShippingNo) : null;
		var creditCardBranchNo = (this.IsSelectedDefaultPaymentOn && this.IsSelectedPaymentCreditCard)
			? (int?)int.Parse(this.UserDefaultOrderSetting.UserCreditCardBranchNo) : null;

		if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
		{
			var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
			if (userDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				creditCardBranchNo = userDefaultOrderSetting.CreditBranchNo;
			}
			else
			{
				var userCreditCard = PayPalUtility.Payment.RegisterAsUserCreditCard(
					this.LoginUserId,
					SessionManager.PayPalCooperationInfo,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);
				creditCardBranchNo = userCreditCard.BranchNo;
			}
		}
		else if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
		{
			var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
			if (userDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			{
				creditCardBranchNo = userDefaultOrderSetting.CreditBranchNo;
			}
			else
			{
				creditCardBranchNo = new UserCreditCardService().GetMaxBranchNoByUserIdAndCooperationType(
					this.LoginUserId,
					Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_PAIDY);
			}
		}

		var invoiceNo = (this.IsSelectedDefaultInvoiceOn)
			? int.Parse(this.UserDefaultOrderSetting.InvoiceNo)
			: (int?)null;

		var rakutenCvsType = this.UserDefaultOrderSetting.RakutenCvsType;
		// Get Zeus Cvs type
		var zeusCvsType = this.IsSelectedZeusCvsPre
			? this.UserDefaultOrderSetting.CvsType
			: null;
		// Get Paygent Cvs type
		var paygentCvsType = this.IsSelectedPaygentCvsPre
			? this.UserDefaultOrderSetting.CvsType
			: null;

		InsertOrUpdateDefaultOrderSetting(
			paymentId: paymentId,
			creditCardBranchNo: creditCardBranchNo,
			shippingNo: shippingNo,
			invoiceNo: invoiceNo,
			rakutenCvsType: rakutenCvsType,
			zeusCvsType: zeusCvsType,
			paygentCvsType: paygentCvsType);
		this.IsDispCompleteMessageForUserDefaultOrderSetting = true;
	}

	/// <summary>
	/// デフォルト注文方法登録/更新
	/// </summary>
	/// <param name="paymentId">決済種別ID</param>
	/// <param name="creditCardBranchNo">クレジットカード枝番</param>
	/// <param name="shippingNo">配送先枝番</param>
	/// <param name="invoiceNo">Invoice No</param>
	/// <param name="rakutenCvsType">楽天コンビニ前払い支払いコンビニ</param>
	/// <param name="zeusCvsType">Zeusコンビニ前払い支払いコンビニ</param>
	/// <param name="paygentCvsType">Paygentコンビニ前払い支払いコンビニ</param>
	private void InsertOrUpdateDefaultOrderSetting(
		string paymentId,
		int? creditCardBranchNo,
		int? shippingNo,
		int? invoiceNo,
		string rakutenCvsType,
		string zeusCvsType,
		string paygentCvsType)
	{
		new UserDefaultOrderSettingService().InsertOrUpdate(
			userId: this.LoginUserId,
			paymentId: paymentId,
			creditCardBranchNo: creditCardBranchNo,
			shippingNo: shippingNo,
			invoiceNo: invoiceNo,
			rakutenCvsType: rakutenCvsType,
			zeusCvsType: zeusCvsType,
			paygentCvsType: paygentCvsType,
			lastChanged: Constants.FLG_LASTCHANGED_USER);
	}

	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }
	/// <summary>選択中の配送先方法があるかの判定</summary>
	private bool IsSelectedDefaultShippingOn
	{
		get { return this.UserDefaultOrderSetting.IsSelectedDefaultShippingOn; }
	}
	/// <summary>選択中の配送先方法が注文者情報の住所であるかの判定</summary>
	private bool IsSelectedDefaultOwnerShippingOn
	{
		get { return this.UserDefaultOrderSetting.IsSelectedDefaultOwnerShippingOn; }
	}
	/// <summary>選択中の支払方法があるかの判定</summary>
	protected bool IsSelectedDefaultPaymentOn
	{
		get { return this.UserDefaultOrderSetting.IsSelectedDefaultPaymentOn; }
	}
	/// <summary>選択中の支払方法がクレジットカードであるか判定</summary>
	private bool IsSelectedPaymentCreditCard
	{
		get { return this.UserDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT; }
	}
	/// <summary>選択中の支払方法がコンビニ後払いであるか判定</summary>
	protected bool IsSelectedPaymentCvsDef
	{
		get { return this.UserDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF; }
	}
	/// <summary>選択中の支払方法がコンビニ前払いであるか判定</summary>
	protected bool IsSelectedPaymentCvsPre
	{
		get { return this.UserDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE; }
	}
	/// <summary>デフォルト注文方法パラメータ</summary>
	protected UserDefaultOrderSettingInputParameter UserDefaultOrderSetting
	{
		get { return (UserDefaultOrderSettingInputParameter)Session[Constants.SESSION_KEY_PARAM]; }
	}
	/// <summary>ラップ済みデフォルト注文方法設定情報入力</summary>
	private WrappedUserDefaultOrderInputs WrappedUserDefaultOrderInput
	{
		get
		{
			if (m_wrappedUserDefaultOrderInput == null)
			{
				m_wrappedUserDefaultOrderInput = new WrappedUserDefaultOrderInputs(this, null);
			}
			return m_wrappedUserDefaultOrderInput;
		}
		set { m_wrappedUserDefaultOrderInput = value; }
	}
	private WrappedUserDefaultOrderInputs m_wrappedUserDefaultOrderInput = null;
	/// <summary>注文方法の保存完了メッセージを表示するか</summary>
	public bool IsDispCompleteMessageForUserDefaultOrderSetting
	{
		get { return (bool)Session[Constants.SESSION_KEY_IS_DISP_USER_DEFAULT_ORDER_SETTING_COMPLETE_MESSAGE]; }
		set { Session[Constants.SESSION_KEY_IS_DISP_USER_DEFAULT_ORDER_SETTING_COMPLETE_MESSAGE] = value; }
	}
	/// <summary>配送先住所が日本か</summary>
	public bool IsShippingAddrJp
	{
		get { return (bool)(ViewState["IsShippingAddrJp"] ?? false); }
		set { ViewState["IsShippingAddrJp"] = value; }
	}
	/// <summary>Is Selected Default Invoice On</summary>
	private bool IsSelectedDefaultInvoiceOn
	{
		get { return string.IsNullOrEmpty(this.UserDefaultOrderSetting.InvoiceNo) == false; }
	}
	/// <summary>選択中の支払方法が楽天コンビニ前払いであるか判定</summary>
	protected bool IsSelectedRakutenCvsPre
	{
		get
		{
			return (this.WrappedUserDefaultOrderInput.WlHfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
				&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten);
		}
	}
	/// <summary>選択中の支払方法が Zeus コンビニ前払いであるか判定</summary>
	protected bool IsSelectedZeusCvsPre
	{
		get
		{
			return this.IsSelectedPaymentCvsPre && OrderCommon.IsPaymentCvsTypeZeus;
		}
	}
	/// <summary>Check Paygent convenience store prepayment</summary>
	protected bool IsSelectedPaygentCvsPre
	{
		get
		{
			return this.IsSelectedPaymentCvsPre && OrderCommon.IsPaymentCvsTypePaygent;
		}
	}
}
