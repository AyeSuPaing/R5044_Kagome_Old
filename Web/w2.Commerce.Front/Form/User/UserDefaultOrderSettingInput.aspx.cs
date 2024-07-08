/*
=========================================================================================================
  Module      : 注文方法設定入力画面(UserDefaultOrderSettingInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.CountryLocation;
using w2.Domain.Payment;
using w2.Domain.TwUserInvoice;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserShipping;

public partial class Form_User_UserDefaultOrderSettingInput : UserPage
{
	# region ラップ済みコントロール宣言
	private WrappedControl WucPaymentDescriptionCvsDef { get { return GetWrappedControl<WrappedControl>("ucPaymentDescriptionCvsDef"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			var userShippingList = new UserShippingService().GetAllOrderByShippingNoDesc(this.LoginUserId).ToList();
			var userUsableCreditCardList = new UserCreditCardService().GetUsable(this.LoginUserId).ToList();
			var userInvoiceList = new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.LoginUserId);

			// デフォルト注文方法系のドロップダウンリスト初期化
			InitDropDownList(userShippingList, userUsableCreditCardList, userInvoiceList);
			// デフォルト注文方法系のコントロール初期化
			InitDefaultOrderSettingControl();
			// デフォルト注文方法を表示する
			DisplaySelectedUserShippingAndUserCreditCard(
				this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.Text,
				this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.Text);

			this.HasUserShippingError = false;
			this.DataBind();
		}
		else
		{
			this.WrappedUserDefaultOrderInput.WlDivCreditCardNoErrorMessage.Visible = false;
			this.WucPaymentDescriptionCvsDef.Visible = false;
		}
	}

	#region  イベント
	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbConfirm_Click(object sender, EventArgs e)
	{
		// 既定のお支払方法がクレジットカードで、登録済みクレジットカード情報が存在しない場合、エラーメッセージを表示
		if (this.IsSelectedPaymentCreditCard && this.IsSelectedPaymentCreditCard && (this.IsRegistedUserDefaultCreditCard == false))
		{
			ShowCreditCardNoErrorMessage();
			return;
		}
		// PayPalログインしないとPayPalは利用できない
		if (this.IsSelectedPaymentPayPal && (SessionManager.PayPalCooperationInfo == null))
		{
			SessionManager.MessageForErrorPage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAL_NEEDS_LOGIN_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// 既定の配送先住所の国が配送不可の場合、確認画面へ遷移しない
		if (this.HasUserShippingError) return;

		// 注文方法入力情報をセッションに格納し、注文方法入力確認画面へリダイレクト
		Session[Constants.SESSION_KEY_PARAM] = GetUserDefaultOrderSettingInput();
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_CONFIRM);
	}

	/// <summary>
	/// ドロップダウンチェンジイベント：デフォルト配送先名
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDefaultShipping_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.WrappedUserDefaultOrderInput.WlShippingCountryErrorMessage.Text = string.Empty;

		// 選択中の配送先情報を表示する
		DisplaySelectedUserShippingAndUserCreditCard(
			this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.Text,
			this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.Text);
	}

	/// <summary>
	/// ドロップダウンチェンジイベント：デフォルトお支払方法
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDefaultPayment_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 既定のお支払方法がクレジットカードで、登録済みクレジットカード情報が存在しない場合、エラーメッセージを表示
		if ((this.IsSelectedPaymentCreditCard && this.IsSelectedDefaultPaymentOn && this.IsRegistedUserDefaultCreditCard == false))
		{
			ShowCreditCardNoErrorMessage();
			return;
		}
		else
		{
			// エラーメッセージ初期化
			ResetErrorMessage();
		}

		// 選択中の支払方法情報を表示する
		DisplaySelectedUserShippingAndUserCreditCard(
			this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.Text,
			this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.Text);
	}

	/// <summary>
	/// ドロップダウンチェンジイベント：デフォルトクレジットカード名
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDefaultUserCreditCardName_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 選択中の支払方法情報を表示する
		DisplaySelectedUserShippingAndUserCreditCard(
			this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.Text, this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.Text);
	}
	#endregion

	/// <summary>
	/// デフォルト注文方法系のコントロール初期化
	/// </summary>
	private void InitDefaultOrderSettingControl()
	{
		// コントロール系の表示/非表示設定
		this.WrappedUserDefaultOrderInput.WrTrDefaultShipping.Visible = (this.IsSelectedDefaultShippingOn && this.IsRegistedUserDefaultShipping);
		this.WrappedUserDefaultOrderInput.WrDivUserShippingInfo.Visible = (this.IsSelectedDefaultShippingOn && this.IsRegistedUserDefaultShipping);
		this.WrappedUserDefaultOrderInput.WrTrDefaultPayment.Visible = true;
		this.WrappedUserDefaultOrderInput.WrDivUserCreditCardInfo.Visible
			= (this.IsSelectedPaymentCreditCard && this.IsRegistedUserDefaultCreditCard && this.IsDefaultPaymentCreditCard);
		// 既定のお支払方法、配送先名、クレジットカード登録名を選択する
		this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectItemByValue(this.UserDefaultOrderSetting.PaymentId);
		this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.SelectItemByValue(this.UserDefaultOrderSetting.ShippingNo.ToString());
		this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.SelectItemByValue(this.UserDefaultOrderSetting.UserCreditCardBranchNo.ToString());
		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.SelectItemByValue(this.UserDefaultOrderSetting.InvoiceNo.ToString());
		}
		this.WrappedUserDefaultOrderInput.WtrDefaultInvoiceInfo.Visible = this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.SelectedValue != this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp;

		if (OrderCommon.IsPaymentCvsTypeZeus)
		{
			this.WrappedUserDefaultOrderInput.WddlZeusCvsType.SelectItemByValue(this.UserDefaultOrderSetting.CvsType);
		}

		if (OrderCommon.IsPaymentCvsTypePaygent)
		{
			this.WrappedUserDefaultOrderInput.WddlPaygentCvsType.SelectItemByValue(this.UserDefaultOrderSetting.CvsType);
		}
	}

	/// <summary>
	/// 選択中の配送先情報、クレジットカード登録情報を表示する
	/// </summary>
	/// <param name="selectedShippingName">選択中の配送先方法名</param>
	/// <param name="selectedUserCreditCardName">選択中のクレジットカード登録名</param>
	private void DisplaySelectedUserShippingAndUserCreditCard(string selectedShippingName, string selectedUserCreditCardName)
	{
		var shippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP;

		// 注文者情報の配送先住所の指定の場合、注文者の配送先情報を表示
		if (this.IsSelectedDefaultOwnerShippingOn)
		{
			var user = new UserService().Get(this.LoginUserId);
			DisplayUserDefaultOwnerShippingSetting(user);
			shippingCountryIsoCode = user.AddrCountryIsoCode;
		}
		// 配送先情報が登録されている場合、デフォルト配送先方法を表示
		else if (this.IsSelectedDefaultShippingOn)
		{
			var userShipping = new UserShippingService().Get(this.LoginUserId, int.Parse(this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.SelectedValue));
			DisplayUserDefaultShippingSetting(userShipping);
			shippingCountryIsoCode = userShipping.ShippingCountryIsoCode;
		}
		// 選択中の配送先方法がない場合、非表示
		else if (this.IsSelectedDefaultShippingOn == false)
		{
			this.WrappedUserDefaultOrderInput.WrTrDefaultShippingInfo.Visible = false;
		}

		this.IsShippingAddrJp = IsCountryJp(shippingCountryIsoCode);

		// クレジットカード情報が登録されている場合の表示
		this.WrappedUserDefaultOrderInput.WrDivUserCreditCardInfo.Visible = false;
		if (this.IsSelectedPaymentCreditCard)
		{
			if (this.IsRegistedUserDefaultCreditCard)
			{
				var userCreditCard = new UserCreditCardService().Get(this.LoginUserId, int.Parse(this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.SelectedValue));
				DisplayDefaultCreditCardSetting(userCreditCard);
			}
			// 選択中のクレジットカードのカード枝番が登録済みクレジットカード情報に存在しない場合、デフォルト支払方法を非表示
			else if (this.IsRegistedUserDefaultCreditCard == false)
			{
				this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.Text = this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp;
				this.WrappedUserDefaultOrderInput.WrDivUserCreditCardInfo.Visible = false;
			}
		}
		// PayPalが選択されているときの処理
		this.WrappedUserDefaultOrderInput.WcDivPayPalInfo.Visible = this.IsSelectedPaymentPayPal;

		// 既定の配送先の国が配送不可の場合、エラーメッセージが出す
		this.HasUserShippingError = (ShippingCountryUtil.CheckShippingCountryAvailable(this.ShippingAvailableCountryList, shippingCountryIsoCode) == false);
		this.WrappedUserDefaultOrderInput.WlShippingCountryErrorMessage.Text = this.HasUserShippingError
			? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_COUNTRY_UNAVAILABLE)
			: string.Empty;

		// 支払方法がコンビニ後払いの場合、注意書きを表示
		this.WucPaymentDescriptionCvsDef.Visible = this.IsSelectedPaymentCvsDef;
		var twInvoiceNo = -1;
		if ((this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.SelectedValue != this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp)
			&& int.TryParse(this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.SelectedValue, out twInvoiceNo))
		{
			var userInvoice = new TwUserInvoiceService().Get(
				this.LoginUserId,
				twInvoiceNo);
			if (userInvoice != null)
			{
				DisplayUserDefaultInvoiceSetting(userInvoice);
			}
		}
		else
		{
			this.WrappedUserDefaultOrderInput.WtrDefaultInvoiceInfo.Visible = false;
		}
	}

	/// <summary>
	/// 注文方法入力情報取得
	/// </summary>
	private UserDefaultOrderSettingInputParameter GetUserDefaultOrderSettingInput()
	{
		var userDefaultOrderSettingInputParam = new UserDefaultOrderSettingInputParameter();
		userDefaultOrderSettingInputParam.IsSelectedDefaultOwnerShippingOn = this.IsSelectedDefaultOwnerShippingOn;
		userDefaultOrderSettingInputParam.IsSelectedDefaultShippingOn = this.IsSelectedDefaultShippingOn;
		userDefaultOrderSettingInputParam.IsSelectedDefaultPaymentOn = this.IsSelectedDefaultPaymentOn;
		userDefaultOrderSettingInputParam.PaymentId = this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectedValue;
		if (this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectedItem != null)
		{
			userDefaultOrderSettingInputParam.PaymentName = this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectedItem.ToString();
		}
		userDefaultOrderSettingInputParam.ShippingNo = this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.SelectedValue;
		if (this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.SelectedItem.ToString() != null)
		{
			userDefaultOrderSettingInputParam.ShippingName = this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.SelectedItem.ToString();
		}
		userDefaultOrderSettingInputParam.ShippingName1 = this.WrappedUserDefaultOrderInput.WlShippingName1.Text;
		userDefaultOrderSettingInputParam.ShippingName2 = this.WrappedUserDefaultOrderInput.WlShippingName2.Text;
		userDefaultOrderSettingInputParam.ShippingNameKana1 = this.WrappedUserDefaultOrderInput.WlShippingNameKana1.Text;
		userDefaultOrderSettingInputParam.ShippingNameKana2 = this.WrappedUserDefaultOrderInput.WlShippingNameKana2.Text;
		userDefaultOrderSettingInputParam.ShippingZip =
			IsCountryJp(this.WrappedUserDefaultOrderInput.ShippingCountryIsoCode)
				? this.WrappedUserDefaultOrderInput.WlShippingZip.Text
				: this.WrappedUserDefaultOrderInput.WlShippingZipGlobal.Text;
		userDefaultOrderSettingInputParam.ShippingAddress1 = this.WrappedUserDefaultOrderInput.WlShippingAddr1.Text;
		userDefaultOrderSettingInputParam.ShippingAddress2 = this.WrappedUserDefaultOrderInput.WlShippingAddr2.Text;
		userDefaultOrderSettingInputParam.ShippingAddress3 = this.WrappedUserDefaultOrderInput.WlShippingAddr3.Text;
		userDefaultOrderSettingInputParam.ShippingAddress4 = this.WrappedUserDefaultOrderInput.WlShippingAddr4.Text;
		userDefaultOrderSettingInputParam.ShippingAddress5 = this.WrappedUserDefaultOrderInput.WlShippingAddr5.Text;
		userDefaultOrderSettingInputParam.ShippingCountryName = this.WrappedUserDefaultOrderInput.WlShippingCountryName.Text;
		userDefaultOrderSettingInputParam.ShippingCompanyName = this.WrappedUserDefaultOrderInput.WlShippingCompanyName.Text;
		userDefaultOrderSettingInputParam.ShippingCompanyPostName = this.WrappedUserDefaultOrderInput.WlShippingCompanyPostName.Text;
		userDefaultOrderSettingInputParam.ShippingTel1 = this.WrappedUserDefaultOrderInput.WlShippingTel1.Text;
		userDefaultOrderSettingInputParam.UserCreditCardBranchNo = this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.SelectedValue;
		if (this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.SelectedItem != null)
		{
			userDefaultOrderSettingInputParam.UserCreditCardCartDispName = this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.SelectedItem.ToString();
		}
		userDefaultOrderSettingInputParam.UserCreditCardCompanyName = this.WrappedUserDefaultOrderInput.WlCardCompanyName.Text;
		userDefaultOrderSettingInputParam.UserCreditCardLastFourDigit = this.WrappedUserDefaultOrderInput.WlLastFourDigit.Text;
		userDefaultOrderSettingInputParam.UserCreditCardExpirationMonth = this.WrappedUserDefaultOrderInput.WlExpirationMonth.Text;
		userDefaultOrderSettingInputParam.UserCreditCardExpirationYear = this.WrappedUserDefaultOrderInput.WlExpirationYear.Text;
		userDefaultOrderSettingInputParam.UserCreditCardAuthorName = this.WrappedUserDefaultOrderInput.WlAuthorName.Text;
		userDefaultOrderSettingInputParam.UserCreditCardBranchNo = this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.SelectedValue;
		userDefaultOrderSettingInputParam.ShippingCountryIsoCode = this.WrappedUserDefaultOrderInput.ShippingCountryIsoCode;

		userDefaultOrderSettingInputParam.InvoiceName = this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.SelectedText;
		if (userDefaultOrderSettingInputParam.InvoiceName != this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp)
		{
			userDefaultOrderSettingInputParam.InvoiceNo = this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.SelectedValue;
			userDefaultOrderSettingInputParam.UniformInvoiceInformation = this.WrappedUserDefaultOrderInput.WlUniformInvoiceInformation.Text;
			userDefaultOrderSettingInputParam.CarryTypeInformation = this.WrappedUserDefaultOrderInput.WlCarryTypeInformation.Text;
			userDefaultOrderSettingInputParam.UniformInvoiceTypeOption1 = this.WrappedUserDefaultOrderInput.WlUniformInvoiceTypeOption1.Text;
			userDefaultOrderSettingInputParam.UniformInvoiceTypeOption2 = this.WrappedUserDefaultOrderInput.WlUniformInvoiceTypeOption2.Text;
		}

		if (this.IsSelectedPaymentCvsPre
			&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten))
		{
			userDefaultOrderSettingInputParam.RakutenCvsTypeName = this.WrappedUserDefaultOrderInput.WddlRakutenCvsType.SelectedText;
			userDefaultOrderSettingInputParam.RakutenCvsType = this.WrappedUserDefaultOrderInput.WddlRakutenCvsType.SelectedValue;
		}
		else
		{
			userDefaultOrderSettingInputParam.RakutenCvsTypeName = string.Empty;
			userDefaultOrderSettingInputParam.RakutenCvsType = string.Empty;
		}

		if (this.IsSelectedPaymentCvsPre && OrderCommon.IsPaymentCvsTypeZeus)
		{
			userDefaultOrderSettingInputParam.CvsTypeName = this.WrappedUserDefaultOrderInput.WddlZeusCvsType.SelectedText;
			userDefaultOrderSettingInputParam.CvsType = this.WrappedUserDefaultOrderInput.WddlZeusCvsType.SelectedValue;
		}

		if (this.IsSelectedPaymentCvsPre && OrderCommon.IsPaymentCvsTypePaygent)
		{
			userDefaultOrderSettingInputParam.CvsTypeName = this.WrappedUserDefaultOrderInput.WddlPaygentCvsType.SelectedText;
			userDefaultOrderSettingInputParam.CvsType = this.WrappedUserDefaultOrderInput.WddlPaygentCvsType.SelectedValue;
		}

		return userDefaultOrderSettingInputParam;
	}

	/// <summary>
	/// ドロップダウンリスト初期化
	/// </summary>
	/// <param name="userShippingList">ユーザー配送先情報リスト</param>
	/// <param name="userCreditCardList">ユーザークレジットカード情報リスト</param>
	/// <param name="userInvoiceList">User Invoice Information List</param>
	private void InitDropDownList(List<UserShippingModel> userShippingList, List<UserCreditCardModel> userCreditCardList, TwUserInvoiceModel[] userInvoiceList)
	{
		// 有効な決済情報用
		this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.AddItem(new ListItem(this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp));

		var payments = new PaymentService().GetValidUserDefaultPayment(Constants.CONST_DEFAULT_SHOP_ID).ToList();

		payments.RemoveAll(payment =>
			((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				|| (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
				|| (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)));

		if (Constants.GLOBAL_OPTION_ENABLE
			|| (Constants.PAYMENT_NETBANKING_OPTION_ENABLED == false))
		{
			payments.RemoveAll(payment => payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET);
		}

		// クレジットカート保持が不可の場合は、既定のお支払方法からクレジットカードを削除
		if (Constants.MAX_NUM_REGIST_CREDITCARD == 0)
		{
			payments.RemoveAll(payment => payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
		}

		// 翻訳情報を設定
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			payments = NameTranslationCommon.SetPaymentTranslationData(
				payments.ToArray(),
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId).ToList();
		}

		if (Constants.PAYMENT_BOKU_OPTION_ENABLED == false)
		{
			payments.RemoveAll(payment => (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
		}

		if ((Constants.PAYMENT_ATMOPTION_ENABLED == false)
			|| Constants.GLOBAL_OPTION_ENABLE)
		{
			payments.RemoveAll(payment => (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATM));
		}

		//ユーザー管理レベルとユーザー区分で使用不可な決済方法を除外する
		payments = OrderCommon.CheckPaymentManagementLevelAndOrderOwnerKbnNotUse(payments.ToArray(), this.LoginUserId).ToList();

		payments.ForEach(payment => this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.AddItem(new ListItem(payment.PaymentName, payment.PaymentId)));

		// ユーザ配送先用
		this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.AddItem(new ListItem(this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp));
		this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.AddItem(new ListItem(
			this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingOwnerShippingDisp,
			Constants.FIELD_USER_DEFAULT_ORDER_SETTING_OWNER_SHIPPING_VALUE));

		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
		{
			userShippingList.ForEach(userShipping => this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.AddItem(new ListItem(userShipping.Name, userShipping.ShippingNo.ToString())));
		}
		else
		{
			var userShippingNormal = userShippingList.Where(item => item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
			this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.AddItems(
				userShippingNormal.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
		}

		// 利用可能なユーザクレジットカード用
		userCreditCardList.ForEach(
			usableUserCreditCard => this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.AddItem(new ListItem(usableUserCreditCard.CardDispName, usableUserCreditCard.BranchNo.ToString())));

		// 配送可能な国情報セット
		this.ShippingAvailableCountryList = new CountryLocationService().GetShippingAvailableCountry();

		this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.AddItem(new ListItem(this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp));
		if (userInvoiceList != null)
		{
			this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.Items.AddRange(userInvoiceList
				.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString()))
				.ToArray());
		}

		if (OrderCommon.IsPaymentCvsTypeZeus)
		{
			var zeusCvsTypes = ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.PAYMENT_ZEUS_CVS_TYPE);
			this.WrappedUserDefaultOrderInput.WddlZeusCvsType.Items.AddRange(zeusCvsTypes);
		}

		if (OrderCommon.IsPaymentCvsTypePaygent)
		{
			var paygentCvsTypes = ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.PAYMENT_PAYGENT_CVS_TYPE);
			this.WrappedUserDefaultOrderInput.WddlPaygentCvsType.Items.AddRange(paygentCvsTypes);
		}
	}

	/// <summary>
	/// 注文者情報の配送先情報を表示する
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	private void DisplayUserDefaultOwnerShippingSetting(UserModel user)
	{
		this.WrappedUserDefaultOrderInput.DisplayUserDefaultOwnerShippingSetting(user);
	}

	/// <summary>
	/// デフォルト配送先方法を表示する
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
	private void DisplayDefaultCreditCardSetting(UserCreditCardModel userCreditCard)
	{
		this.WrappedUserDefaultOrderInput.DisplayUserDefaultCreditCardSetting(userCreditCard);
	}

	/// <summary>
	/// エラーメッセージ表示：クレジットカード未登録
	/// </summary>
	private void ShowCreditCardNoErrorMessage()
	{
		this.WrappedUserDefaultOrderInput.ShowCreditCardNoErrorMessage();
	}

	/// <summary>
	/// エラーメッセージ初期化
	/// </summary>
	private void ResetErrorMessage()
	{
		this.WrappedUserDefaultOrderInput.ResetErrorMessage();
	}

	/// <summary>
	/// ペイパル認証完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPayPalAuthComplete_Click(object sender, EventArgs e)
	{
		var payPalScriptsForm = GetWrappedControl<WrappedPayPalPayScriptsFormControl>("ucPaypalScriptsForm");
		SetPaypalInfoToSession(payPalScriptsForm);

		PayPalUtility.Account.UpdateUserExtendForPayPal(
			this.LoginUserId,
			SessionManager.PayPalLoginResult,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// Dropdown List Default Invoice Name
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDefaultInvoice_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplaySelectedUserInvoice();
	}

	/// <summary>
	/// Display the selected invoice information
	/// </summary>
	private void DisplaySelectedUserInvoice()
	{
		if (this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.SelectedValue != this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp)
		{
			var userInvoice = new TwUserInvoiceService().Get(
				this.LoginUserId,
				int.Parse(this.WrappedUserDefaultOrderInput.WddlDefaultInvoice.SelectedValue));
			if (userInvoice != null)
			{
				DisplayUserDefaultInvoiceSetting(userInvoice);
			}
		}
		else
		{
			this.WrappedUserDefaultOrderInput.WtrDefaultInvoiceInfo.Visible = false;
		}
	}

	/// <summary>
	/// Display User Default Invoice Setting
	/// </summary>
	/// <param name="userInvoice">User Invoice</param>
	private void DisplayUserDefaultInvoiceSetting(TwUserInvoiceModel userInvoice)
	{
		this.WrappedUserDefaultOrderInput.DisplayUserDefaultInvoiceSetting(userInvoice);
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
		get { return this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.SelectedItem.ToString() != this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp; }
	}
	/// <summary>選択中の配送先方法が注文者情報の住所であるかの判定</summary>
	private bool IsSelectedDefaultOwnerShippingOn
	{
		get { return this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.SelectedItem.ToString() == this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingOwnerShippingDisp; }
	}
	/// <summary>選択中の支払方法があるかの判定</summary>
	private bool IsSelectedDefaultPaymentOn
	{
		get { return this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectedItem.ToString() != this.WrappedUserDefaultOrderInput.UserDefaultOrderSettingNoneDisp; }
	}
	/// <summary>選択中の支払方法がクレジットカードであるか判定</summary>
	private bool IsSelectedPaymentCreditCard
	{
		get { return (this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT); }
	}
	/// <summary>選択中の支払方法がコンビニ後払いであるか判定</summary>
	protected bool IsSelectedPaymentCvsDef
	{
		get { return (this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF); }
	}
	/// <summary>選択中の支払方法がコンビニ前払いであるか判定</summary>
	protected bool IsSelectedPaymentCvsPre
	{
		get { return (this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE); }
	}
	/// <summary>選択中の支払方法がPayPalであるか判定</summary>
	protected bool IsSelectedPaymentPayPal
	{
		get { return (this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL); }
	}
	/// <summary>選択中の支払方法が後付款(TriLink後払い)であるか判定</summary>
	protected bool IsSelectedPaymentTryLinkAfterPay
	{
		get { return (this.WrappedUserDefaultOrderInput.WrDdlDefaultPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY); }
	}
	/// <summary>ユーザー配送先情報が登録されているか判定</summary>
	private bool IsRegistedUserDefaultShipping
	{
		get { return this.WrappedUserDefaultOrderInput.WrDdlDefaultShipping.Items.Count > 0; }
	}
	/// <summary>ユーザークレジットカード情報が登録されているか判定</summary>
	private bool IsRegistedUserDefaultCreditCard
	{
		get { return this.WrappedUserDefaultOrderInput.WrDdlUserCreditCardName.Items.Count > 0; }
	}
	/// <summary>既定のデフォルト支払方法がクレジットカードであるかの判定</summary>
	private bool IsDefaultPaymentCreditCard
	{
		get { return (this.UserDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT); }
	}
	/// <summary>デフォルト注文方法パラメータ</summary>
	private UserDefaultOrderSettingInputParameter UserDefaultOrderSetting
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
	/// <summary>配送可能な国情報(</summary>
	protected CountryLocationModel[] ShippingAvailableCountryList
	{
		get { return (CountryLocationModel[])Session["shipping_available_country_list"]; }
		set { Session["shipping_available_country_list"] = value; }
	}
	/// <summary>配送先にエラーがあるか</summary>
	protected bool HasUserShippingError
	{
		get { return (bool)ViewState["HasUserShippingError"]; }
		set { ViewState["HasUserShippingError"] = value; }
	}
	/// <summary>配送先住所が日本か</summary>
	public bool IsShippingAddrJp
	{
		get { return (bool)(ViewState["IsShippingAddrJp"] ?? false); }
		set { ViewState["IsShippingAddrJp"] = value; }
	}
	///// <summary>楽天コンビニ支払先</summary>
	protected ListItemCollection RakutenCvsTypeList
	{
		get { return ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.PAYMENT_RAKUTEN_CVS_TYPE); }
	}
}
