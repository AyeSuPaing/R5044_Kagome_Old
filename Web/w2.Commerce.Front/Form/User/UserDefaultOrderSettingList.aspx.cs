/*
=========================================================================================================
  Module      : 注文方法設定一覧画面(UserDefaultOrderSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.Payment;
using w2.Domain.TwUserInvoice;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.UserShipping;

public partial class Form_User_UserDefaultOrderSettingList : UserPage
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
		if (!IsPostBack)
		{ 
			if (Constants.TWOCLICK_OPTION_ENABLE == false) 
			{ 
				this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR); 
				var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR).CreateUrl(); 
				this.Response.Redirect(url);
			}

			// デフォルト注文方法を表示する
			DisplayUserDefaultOrderSetting();
			// デフォルト注文方法の保存完了メッセージを初期化
			ResetUserDefaultOrderSettingCompleteMessage();
		}
	}

	/// <summary>
	/// 編集するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEdit_Click(object sender, EventArgs e)
	{
		var input = GetUserDefaultOrderSetting();

		// デフォルト注文方法情報をセッションに格納し、注文方法設定入力画面へリダイレクトする
		Session[Constants.SESSION_KEY_PARAM] = input;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_INPUT);
	}

	/// <summary>
	/// デフォルト注文方法を表示する
	/// </summary>
	private void DisplayUserDefaultOrderSetting()
	{
		var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
		// デフォルト注文方法が登録されている場合、デフォルト注文方法を表示する
		if (userDefaultOrderSetting != null)
		{
			DisplayUserDefaultOrderSetting(userDefaultOrderSetting);
		}
		else
		{
			DisplayNoUserDefaultOrderSetting();
		}
	}

	/// <summary>
	/// デフォルト注文方法が未登録時の表示
	/// </summary>
	private void DisplayNoUserDefaultOrderSetting()
	{
		this.WrappedUserDefaultOrderInput.DisplayNoUserDefaultOrderSetting();
	}

	/// <summary>
	/// デフォルト注文方法を表示する
	/// </summary>
	/// <param name="userDefaultOrderSetting">デフォルト注文方法情報</param>
	private void DisplayUserDefaultOrderSetting(UserDefaultOrderSettingModel userDefaultOrderSetting)
	{
		var user = new UserService().Get(this.LoginUserId);
		var userShipping = (userDefaultOrderSetting.UserShippingNo != null)
			? new UserShippingService().Get(this.LoginUserId, (int)userDefaultOrderSetting.UserShippingNo) : null;
		var userCreditCard = (userDefaultOrderSetting.CreditBranchNo != null)
			? new UserCreditCardService().Get(this.LoginUserId, (int)userDefaultOrderSetting.CreditBranchNo) : null;
		var paymentName = (userDefaultOrderSetting.PaymentId != null)
			? DataCacheControllerFacade.GetPaymentCacheController().Get(userDefaultOrderSetting.PaymentId).PaymentName : null;

		// 翻訳情報を設定（既定の支払方法が指定なしの場合は翻訳処理を行わない）
		if (Constants.GLOBAL_OPTION_ENABLE && (paymentName != this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp))
		{
			paymentName = (string.IsNullOrEmpty(paymentName) == false)
				? NameTranslationCommon.GetTranslationName(
					userDefaultOrderSetting.PaymentId,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
					Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PAYMENT_PAYMENT_NAME,
					paymentName)
				: paymentName;
		}

		// デフォルト配送先方法が設定されている場合表示（注文者情報）
		var shippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP;
		if ((user != null) && (userDefaultOrderSetting.UserShippingNo == 0))
		{
			DisplayUserDefaultOwnerShippingSetting(user);
			this.WrappedUserDefaultOrderInput.WlHfShippingNo.Value = userDefaultOrderSetting.UserShippingNo.ToString();
			shippingCountryIsoCode = user.AddrCountryIsoCode;
		}
		// デフォルト配送先方法が設定されている場合表示（アドレス帳の情報）
		else if ((userShipping != null) && userDefaultOrderSetting.UserShippingNo != 0)
		{
			DisplayUserDefaultShippingSetting(userShipping);
			this.WrappedUserDefaultOrderInput.WlHfShippingNo.Value = userDefaultOrderSetting.UserShippingNo.ToString();
			shippingCountryIsoCode = userShipping.ShippingCountryIsoCode;
		}
		else
		{
			DisplayNoUserShipping();
		}

		this.IsShippingAddrJp = IsCountryJp(shippingCountryIsoCode);

		// デフォルト支払方法が設定されていれば表示
		if (userDefaultOrderSetting.PaymentId != null)
		{
			DisplayUserDefaultPaymentName(paymentName);
			this.WrappedUserDefaultOrderInput.WlHfPaymentId.Value = userDefaultOrderSetting.PaymentId.ToString();
		}
		else
		{
			DisplayUserDefaultPaymentName(this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp);
		}

		// デフォルト支払方法がクレジットカード情報の場合表示
		if (userDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			if (userCreditCard.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON)
			{
				DisplayUserDefaultCreditCardSetting(userCreditCard);
				this.WrappedUserDefaultOrderInput.WlHfUserCreditCardBranchNo.Value = userDefaultOrderSetting.CreditBranchNo.ToString();
			}
			// それ以外の場合は、クレジットカード情報を表示せずエラーメッセージ表示
			else
			{
				DisplayNoUserCreditCard();
			}
		}
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// デフォルト支払方法が後付款(TriLink後払い)で、配送先が台湾ではない場合にメッセージ表示
			if ((userDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				&& ((shippingCountryIsoCode != Constants.COUNTRY_ISO_CODE_TW)
					|| (this.WrappedUserDefaultOrderInput.WlShippigName.Text == this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp)))
			{
				DisplayTryLinkAfterPayErrorMessage();
			}
		}

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			var userInvoice = (userDefaultOrderSetting.UserInvoiceNo.HasValue)
				? new TwUserInvoiceService().Get(
					this.LoginUserId,
					userDefaultOrderSetting.UserInvoiceNo.Value)
				: null;
			if (userInvoice != null)
			{
				this.WrappedUserDefaultOrderInput.WhfInvoiceNo.Value = userDefaultOrderSetting.UserInvoiceNo.ToString();
				this.WrappedUserDefaultOrderInput.DisplayUserDefaultInvoiceSetting(userInvoice);
			}
			else
			{
				this.WrappedUserDefaultOrderInput.DisplayNoUserInvoice();
			}
		}

		// 支払い方法が楽天コンビニ前払いの場合
		DisplayUserDefaultRakutenCvsType(userDefaultOrderSetting.RakutenCvsType);

		// 支払い方法がコンビニ前払いの場合
		if (userDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
		{
			var cvsType = OrderCommon.IsPaymentCvsTypeZeus
				? userDefaultOrderSetting.ZeusCvsType
				: (OrderCommon.IsPaymentCvsTypePaygent 
					? userDefaultOrderSetting.PaygentCvsType 
					: null);
			this.WrappedUserDefaultOrderInput.DisplayUserDefaultCvsType(cvsType);
		}
		else
		{
			this.WrappedUserDefaultOrderInput.DisplayNoUserDefaultCvsType();
		}

		// 支払方法がコンビニ後払いの場合、注意書きを表示
		this.WucPaymentDescriptionCvsDef.Visible = (userDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
	}

	/// <summary>
	/// デフォルト支払方法を表示する
	/// </summary>
	/// <param name="paymentName">決済種別名</param>
	private void DisplayUserDefaultPaymentName(string paymentName)
	{
		this.WrappedUserDefaultOrderInput.DisplayUserDefaultPaymentName(paymentName);
	}

	/// <summary>
	/// デフォルト配送方法を表示する（注文者情報）
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	private void DisplayUserDefaultOwnerShippingSetting(UserModel user)
	{
		this.WrappedUserDefaultOrderInput.DisplayUserDefaultOwnerShippingSetting(user);
	}

	/// <summary>
	/// デフォルト配送方法を表示する（アドレス帳情報）
	/// </summary>
	/// <param name="userShipping">ユーザー配送先情報</param>
	private void DisplayUserDefaultShippingSetting(UserShippingModel userShipping)
	{
		this.WrappedUserDefaultOrderInput.DisplayUserDefaultShippingSetting(userShipping);
	}

	/// <summary>
	/// デフォルトクレジットカード情報を表示する
	/// </summary>
	/// <param name="userCreditCard">ユーザークレジットカード情報</param>
	private void DisplayUserDefaultCreditCardSetting(UserCreditCardModel userCreditCard)
	{
		this.WrappedUserDefaultOrderInput.DisplayUserDefaultCreditCardSetting(userCreditCard);
	}

	/// <summary>
	/// デフォルト支払いコンビニを表示する
	/// </summary>
	/// <param name="cvsType">支払いコンビニ</param>
	private void DisplayUserDefaultRakutenCvsType(string cvsType)
	{
		this.WrappedUserDefaultOrderInput.DisplayUserDefaultRakutenCvsType(cvsType);
	}

	/// <summary>
	/// 注文方法設定入力情報取得
	/// </summary>
	/// <returns>注文方法設定入力情報</returns>
	private UserDefaultOrderSettingInputParameter GetUserDefaultOrderSetting()
	{
		var result = new UserDefaultOrderSettingInputParameter
		{
			PaymentId = this.WrappedUserDefaultOrderInput.WlHfPaymentId.Value,
			ShippingNo = this.WrappedUserDefaultOrderInput.WlHfShippingNo.Value,
			UserCreditCardBranchNo = this.WrappedUserDefaultOrderInput.WlHfShippingNo.Value,
			ShippingName = this.WrappedUserDefaultOrderInput.WlShippigName.Text,
			PaymentName = this.WrappedUserDefaultOrderInput.WlDefaultPayment.Text,
			UserCreditCardCartDispName = this.WrappedUserDefaultOrderInput.WlCardDispName.Text,
			InvoiceNo = this.WrappedUserDefaultOrderInput.WhfInvoiceNo.Value,
			InvoiceName = this.WrappedUserDefaultOrderInput.WlInvoiceName.Text,
			CvsType = this.WrappedUserDefaultOrderInput.WhfCvsType.Value,
			CvsTypeName = this.WrappedUserDefaultOrderInput.WlCvsTypeName.Text,
		};
		return result;
	}

	/// <summary>
	/// デフォルト注文方法の保存完了メッセージを初期化
	/// </summary>
	private void ResetUserDefaultOrderSettingCompleteMessage()
	{
		// 注文方法の入力確認画面からの画面遷移でなければ、注文完了メッセージ非表示
		var referrerUrl = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "";
		var orderSettingConfirmUrl =
			(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_CONFIRM);
		if (referrerUrl != orderSettingConfirmUrl)
		{
			this.IsDispCompleteMessageForUserDefaultOrderSetting = false;
		}
	}

	/// <summary>
	/// アドレス帳情報がない場合の表示
	/// </summary>
	private void DisplayNoUserShipping()
	{
		this.WrappedUserDefaultOrderInput.DisplayNoUserShipping();
	}

	/// <summary>
	/// クレジットカード情報がない場合の表示
	/// </summary>
	private void DisplayNoUserCreditCard()
	{
		this.WrappedUserDefaultOrderInput.DisplayNoUserCreditCard();
	}

	/// <summary>
	/// 後付款(TriLink後払い)設定時に配送先が台湾でない場合の表示
	/// </summary>
	private void DisplayTryLinkAfterPayErrorMessage()
	{
		this.WrappedUserDefaultOrderInput.DisplayTryLinkAfterPayErrorMessage();
	}

	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }
	/// <summary>ラップ済みデフォルト注文方法設定情報入力</summary>
	public WrappedUserDefaultOrderInputs WrappedUserDefaultOrderInput
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
}