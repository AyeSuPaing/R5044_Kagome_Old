/*
=========================================================================================================
  Module      : Amazonペイメント画面(OrderAmazonInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using Amazon.Pay.API.WebStore.CheckoutSession;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common;
using w2.Common.Util;
using w2.Domain.Payment;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// Amazonペイメント画面
/// </summary>
public partial class Form_Order_OrderAmazonInput : OrderCartPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済みコントロール宣言
	protected WrappedHtmlGenericControl WhgcConstraintErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "constraintErrorMessage"); } }
	protected WrappedHtmlGenericControl WdvUserRegisterRegulationVisible { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "dvUserRegisterRegulationVisible"); } }
	protected WrappedHiddenField WhfAmazonOrderRefID { get { return GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfAmazonOrderRefID"); } }
	protected WrappedHiddenField WhfAmazonBillingAgreementId { get { return GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfAmazonBillingAgreementId"); } }
	protected WrappedCheckBox WcbOwnerMailFlg { get { return GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbOwnerMailFlg"); } }
	protected WrappedCheckBox WcbOwnerMailFlg2 { get { return GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbOwnerMailFlg2"); } }
	protected WrappedCheckBox WcbShipToOwnerAddress { get { return GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbShipToOwnerAddress"); } }
	protected WrappedCheckBox WcbUserRegisterForExternalSettlement { get { return GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbUserRegisterForExternalSettlement", false); } }
	protected WrappedCheckBox WcbUserRegister { get { return GetWrappedControl<WrappedCheckBox>(this.FirstRpeaterItem, "cbUserRegister", false); } }
	protected WrappedUpdatePanel WupUserRegistRegulationForAmazonPay { get { return GetWrappedControl<WrappedUpdatePanel>(this.FirstRpeaterItem, "upUserRegistRegulationForAmazonPay"); } }

	/// <summary>Wrapped text box owner tel1 1</summary>
	protected WrappedTextBox WtbOwnerTel1_1 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerTel1_1"); } }
	/// <summary>Wrapped text box owner tel1 2</summary>
	protected WrappedTextBox WtbOwnerTel1_2 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerTel1_2"); } }
	/// <summary>Wrapped text box owner tel1 3</summary>
	protected WrappedTextBox WtbOwnerTel1_3 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerTel1_3"); } }
	/// <summary>Wrapped text box owner tel1</summary>
	protected WrappedTextBox WtbOwnerTel1 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1"); } }
	/// <summary>Wrapped label authentication status</summary>
	protected WrappedLabel WlbAuthenticationStatus { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationStatus"); } }
	/// <summary>Wrapped label authentication message</summary>
	protected WrappedLabel WlbAuthenticationMessage { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationMessage"); } }
	/// <summary>Wrapped link button get authentication code</summary>
	protected WrappedLinkButton WlbGetAuthenticationCode { get { return GetWrappedControl<WrappedLinkButton>(this.FirstRpeaterItem, "lbGetAuthenticationCode"); } }
	#endregion

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			if (this.CartList.Items[0].Items.Count == 0) return;

			InitPage(e);

			this.AmazonCheckoutSession = new CheckoutSessionResponse();
			return;
		}

		// 画面遷移の正当性チェック
		if (!IsPostBack)
		{
			CheckOrderUrlSession();
		}

		// 注文同梱されている場合、カート情報を同梱後のカートに変更する
		if (this.IsOrderCombined)
		{
			this.CartList = SessionManager.OrderCombineCartList;
		}

		// カートチェック
		CheckCartData();

		// Amazon Payが利用できるかチェック
		if (this.CanUseAmazonPayment() == false)
		{
			Response.Redirect(Constants.PATH_ROOT);
		}

		if (Constants.AMAZON_PAYMENT_CV2_ENABLED)
		{
			if (string.IsNullOrEmpty(this.AmazonCheckoutSessionId))	Response.Redirect(Constants.PATH_ROOT);

			this.AmazonCheckoutSession = new AmazonCv2ApiFacade().GetCheckoutSession(this.AmazonCheckoutSessionId);

			var shippingAddress = this.AmazonCheckoutSession.ShippingAddress;
			var billingAddress = this.AmazonCheckoutSession.BillingAddress;

			this.CartList.Items[0].Shippings[0].Zip1 = shippingAddress.PostalCode.Substring(0, 3);
			this.CartList.Items[0].Shippings[0].Zip2 = shippingAddress.PostalCode.Substring(4);
			this.CartList.Items[0].Shippings[0].Addr1 = shippingAddress.StateOrRegion;
			this.CartList.Items[0].Shippings[0].Addr2 = StringUtility.ToEmpty(shippingAddress.AddressLine1);
			this.CartList.Items[0].Shippings[0].Addr3 = StringUtility.ToEmpty(shippingAddress.AddressLine2);
			this.CartList.Items[0].Shippings[0].Addr4 = StringUtility.ToEmpty(shippingAddress.AddressLine3);
			this.CartList.Items[0].Shippings[0].ShippingCountryIsoCode = shippingAddress.CountryCode;
			this.CartList.Items[0].Shippings[0].Tel1 = shippingAddress.PhoneNumber;
			this.CartList.Items[0].Shippings[0].IsSameShippingAsCart1 = false;
			this.CartList.Items[0].Calculate(false);

			if (Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED)
			{
				var relationMemo = Constants.CONST_RELATIONMEMO_AMAZON_PAY + "\r\n" +
					"注文者氏名：" + this.AmazonModel.Name + "\r\n" +
					"メールアドレス：" + this.AmazonModel.Email;
				this.CartList.Items[0].RelationMemo = relationMemo;
			}

			// 注文者入力情報と配送先入力情報を検証
			var errorMessages = new StringBuilder();
			var shippingInput = new AmazonAddressInput(shippingAddress, this.AmazonCheckoutSession.Buyer.Email);
			var billingInput = new AmazonAddressInput(billingAddress, this.AmazonCheckoutSession.Buyer.Email);
			errorMessages.AppendLine(shippingInput.Validate());
			if (Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED) errorMessages.AppendLine(billingInput.Validate());

			var errorMessage = errorMessages.ToString().Trim();
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
			}

			var shippingModel = AmazonAddressParser.Parse(shippingInput);
			var billingModel = AmazonAddressParser.Parse(billingInput);
			Session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS] = shippingModel;

			// 未ログインの場合は注文者情報を設定
			// 請求先住所取得オプションがTRUEの場合は、追加で「注文確認画面へ遷移したことない」場合に注文者情報を設定
			if ((this.IsLoggedIn == false)
				&& Constants.AMAZON_PAYMENT_CV2_ENABLED
				&& (SessionManager.IsMovedOnOrderConfirm == false))
			{
				var owner = new CartOwner
				{
					Name = this.AmazonModel.Name,
					Name1 = this.AmazonModel.GetName1(),
					Name2 = this.AmazonModel.GetName2(),
					MailAddr = this.AmazonModel.Email,
					MailFlg = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.mail_flg.default@@") == Constants.FLG_USER_MAILFLG_OK)
				};

				if (Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED)
				{
					owner.Name = shippingModel.Name;
					owner.Name1 = shippingModel.Name1;
					owner.Name2 = shippingModel.Name2;
					owner.Zip1 = shippingModel.Zip1;
					owner.Zip2 = shippingModel.Zip2;
					owner.Addr1 = shippingModel.Addr1;
					owner.Addr2 = shippingModel.Addr2;
					owner.Addr3 = shippingModel.Addr3;
					owner.Addr4 = shippingModel.Addr4;
					owner.Tel1_1 = shippingModel.Tel1;
					owner.Tel1_2 = shippingModel.Tel2;
					owner.Tel1_3 = shippingModel.Tel3;
				}

				if (Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED)
				{
					owner.Name = billingModel.Name;
					owner.Name1 = billingModel.Name1;
					owner.Name2 = billingModel.Name2;
					owner.Zip1 = billingModel.Zip1;
					owner.Zip2 = billingModel.Zip2;
					owner.Addr1 = billingModel.Addr1;
					owner.Addr2 = billingModel.Addr2;
					owner.Addr3 = billingModel.Addr3;
					owner.Addr4 = billingModel.Addr4;
					owner.AddrCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE
						? Constants.COUNTRY_ISO_CODE_JP
						: string.Empty;
					owner.Tel1_1 = billingModel.Tel1;
					owner.Tel1_2 = billingModel.Tel2;
					owner.Tel1_3 = billingModel.Tel3;
				}

				this.CartList.SetOwner(owner);

				var ownerModel = new AmazonAddressModel
				{
					Name = this.AmazonModel.Name,
					Name1 = this.AmazonModel.GetName1(),
					Name2 = this.AmazonModel.GetName2(),
					NameKana = Constants.PAYMENT_AMAZON_NAMEKANA1 + Constants.PAYMENT_AMAZON_NAMEKANA2,
					NameKana1 = Constants.PAYMENT_AMAZON_NAMEKANA1,
					NameKana2 = Constants.PAYMENT_AMAZON_NAMEKANA2,
					MailAddr = this.AmazonModel.Email,
				};

				if (Constants.AMAZON_PAYMENT_CV2_ENABLED &&
					Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED)
				{
					ownerModel.Name = shippingModel.Name;
					ownerModel.Name1 = shippingModel.Name1;
					ownerModel.Name2 = shippingModel.Name2;
					ownerModel.Zip1 = shippingModel.Zip1;
					ownerModel.Zip2 = shippingModel.Zip2;
					ownerModel.Addr1 = shippingModel.Addr1;
					ownerModel.Addr2 = shippingModel.Addr2;
					ownerModel.Addr3 = shippingModel.Addr3;
					ownerModel.Addr4 = shippingModel.Addr4;
					ownerModel.Tel1 = shippingModel.Tel1;
					ownerModel.Tel2 = shippingModel.Tel2;
					ownerModel.Tel3 = shippingModel.Tel3;
				}
				Session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS] = ownerModel;
			}
		}
		else
		{
			var amazonAddress = (AmazonAddressModel)Session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS];
			if (Session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS] != null)
			{
				this.CartList.Items[0].Shippings[0].Zip1 = amazonAddress.Zip1;
				this.CartList.Items[0].Shippings[0].Zip2 = amazonAddress.Zip2;
				this.CartList.Items[0].Shippings[0].Addr1 = amazonAddress.Addr1;
				this.CartList.Items[0].Shippings[0].Addr2 = amazonAddress.Addr2;
				this.CartList.Items[0].Shippings[0].Addr3 = amazonAddress.Addr3;
				this.CartList.Items[0].Shippings[0].Addr4 = amazonAddress.Addr4;
				this.CartList.Items[0].Shippings[0].ShippingCountryIsoCode = amazonAddress.CountryCode;
				this.CartList.Items[0].Calculate(false);
			}

			// Amazonアカウント認証
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			// 制約エラーがあるか確認
			if (AmazonApiFacade
					.SetBillingAgreementDetails(GetWrappedControl<WrappedHiddenField>(this.FirstRpeaterItem, "hfAmazonBillingAgreementId").Value)
					.GetConstraintIdList().Count == 0)
			{
				GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "constraintErrorMessage").InnerHtml = "";
			}
			if (amazonModel == null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_INVALID_TOKEN_FOR_AMAZON);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		// Load shipping method
		this.Process.CreateShippingMethodListOnDataBind();

		if (!IsPostBack)
		{
			// 初期処理
			this.InitPage(e);

			// 領収書情報
			if (Constants.RECEIPT_OPTION_ENABLED)
			{
				// 領収書希望の選択肢変更イベント
				var wddlReceiptFlg = GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlReceiptFlg");
				if (wddlReceiptFlg.HasInnerControl)
				{
					ddlReceiptFlg_OnSelectedIndexChanged(wddlReceiptFlg, EventArgs.Empty);
				}
			}

			if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
			{
				var telNew = WtbOwnerTel1_1.HasInnerControl
					? string.Format(
						"{0}{1}{2}",
						this.WtbOwnerTel1_1.Text,
						this.WtbOwnerTel1_2,
						this.WtbOwnerTel1_3.Text)
					: this.WtbOwnerTel1.Text;

				if (telNew == this.CartList.Owner.Tel1)
				{
					this.HasAuthenticationCode = true;
					this.AuthenticationCode = string.Empty;
					this.WlbGetAuthenticationCode.Enabled = false;
				}

				if (string.IsNullOrEmpty(this.CartList.AuthenticationCode) == false)
				{
					var wtbAuthenticationCode = GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbAuthenticationCode");
					var wlbGetAuthenticationCode = GetWrappedControl<WrappedLinkButton>(this.FirstRpeaterItem, "lbGetAuthenticationCode");

					this.HasAuthenticationCode = this.CartList.HasAuthenticationCode;
					wtbAuthenticationCode.Text
						= this.AuthenticationCode
						= this.CartList.AuthenticationCode;
					wtbAuthenticationCode.Enabled
						= wlbGetAuthenticationCode.Enabled
						= false;
				}
			}
		}

		if (CheckUnavailableShippingAreaForAmazonInput())
		{
			var wpShippingZipError = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "pShippingZipError");
			wpShippingZipError.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ZIPCODE_UNAVAILABLE_SHIPPING_AREA);
		}

		SessionManager.IsAmazonPayGotRecurringConsent = this.CartList.HasFixedPurchase;

		// カスタムバリデータ属性値更新
		UpdateAttributeValueForCustomValidator();
	}
	#endregion

	#region #lbNext_Click 次へボタンクリック
	/// <summary>
	/// 次へボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNext_Click(object sender, EventArgs e)
	{
		this.RegisterUser = null;

		if (lbNext_Click_OrderShipping_AmazonOwner(sender, e) == false) return;
		if (lbNext_Click_OrderShipping_AmazonShipping(sender, e) == false) return;
		if (lbNext_Click_OrderShipping_AmazonOthers(sender, e) == false) return;

		// 配送希望日についてのエラーがあれば、メッセージ表示
		if (CheckAndDisplayAmazonShippingDateErrorMessage()) return;

		// 決済情報セット
		var payment = new PaymentService().Get(
			this.CartList.Items[0].ShopId,
			OrderCommon.GetAmazonPayPaymentId());
		this.CartList.Items[0].Payment.UpdateCartPayment(
			payment.PaymentId,
			payment.PaymentName,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			null,
			false,
			string.Empty);

		this.CartList.Items[0].CalculateWithCartShipping();
		this.CartList.Items[0].AmazonOrderReferenceId =
			Constants.AMAZON_PAYMENT_CV2_ENABLED
				? this.CartList.Items[0].HasFixedPurchase
					? string.Empty
					: this.AmazonCheckoutSession.ChargePermissionId
				: this.WhfAmazonOrderRefID.Value;
		this.CartList.Items[0].ExternalPaymentAgreementId =
			Constants.AMAZON_PAYMENT_CV2_ENABLED
				? string.Empty
				: this.WhfAmazonBillingAgreementId.Value;

		// 領収書情報をカートにセット※エラーがある場合、TRUEで返す
		if (Constants.RECEIPT_OPTION_ENABLED && SetReceipt()) return;

		// 配送先が配送不可エリアかどうか
		if (CheckUnavailableShippingAreaForAmazonInput())
		{
			var wpShippingZipError = GetWrappedControl<WrappedHtmlGenericControl>(this.FirstRpeaterItem, "pShippingZipError");
			wpShippingZipError.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR);
			return;
		}

		if (this.AmazonPayRegisterVisible
			&& ((this.WcbUserRegisterForExternalSettlement.Visible && this.WcbUserRegisterForExternalSettlement.Checked)
				|| (this.WcbUserRegister.Visible && this.WcbUserRegister.Checked)))
		{
			SessionManager.IsAmazonPayRegisterForOrder = true;
			this.Process.IsVisible_UserPassword = false;

			var loggedUser = Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED == false
				? this.Process.UserRegisterAndNextUrlForAmazonPay()
				: this.Process.UserRegisterForAmazonPayByOrderOwner();

			// クロスポイント側にユーザー情報を登録
			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				var apiResult = new CrossPointUserApiService().Insert(loggedUser);

				if (apiResult.IsSuccess == false)
				{
					var errorMessage =
						apiResult.ErrorCodeList.Contains(Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
							? apiResult.ErrorMessage
							: MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
					throw new w2Exception(errorMessage);
				}

				if (Constants.W2MP_POINT_OPTION_ENABLED)
				{
					new UserRegister().PointAtRegist(loggedUser, UpdateHistoryAction.DoNotInsert);
				}
			}

			// 確認画面へ遷移するURL
			var nextUrl = this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_CONFIRM;

			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_CONFIRM;

			// ログインIDをCookieから削除 ※クッキーのログインIDが他者の可能性があるため
			UserCookieManager.CreateCookieForLoginId("", false);

			// ログイン成功アクション実行
			ExecLoginSuccessActionAndGoNextInner(loggedUser, nextUrl, UpdateHistoryAction.Insert);
		}
		SessionManager.IsMovedOnOrderConfirm = true;

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_CONFIRM;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_CONFIRM);
	}
	#endregion

	#region -InitPage 初期処理
	/// <summary>
	/// 初期処理
	/// </summary>
	/// <param name="e">イベント</param>
	private void InitPage(EventArgs e)
	{
		this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);

		// カート情報セット
		base.CreateOrderOwner();
		base.CreateOrderShipping();
		base.CreateOrderMemo();
		base.CreateCartPayment();
		base.CreateOrderExtend();

		// 配送情報入力画面初期処理
		InitComponentsOrderShipping();

		// 配送種別情報取得後＆画面データバインド前
		CreateFixedPurchaseSettings();

		this.DataBind();

		this.CartList.CartListShippingMethodUserUnSelected();

		// 配送方法選択
		this.WrCartList.Items
			.Cast<RepeaterItem>()
			.ToList().ForEach(riCart => SelectShippingMethod(riCart, this.CartList.Items[riCart.ItemIndex]));

		// 配送情報入力画面初期処理（共通）
		InitComponentsDispOrderShipping(e);

		// 配送サービス選択初期化
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			SelectDeliveryCompany(riCart, this.CartList.Items[riCart.ItemIndex]);
		}

		// 注文同梱されている場合、親注文情報で各コントロールを初期化
		if (this.IsOrderCombined)
		{
			this.ddlShippingMethod.SelectedValue = this.CartList.Items[0].GetShipping().ShippingMethod;
			this.ddlShippingDate.SelectedValue = this.CartList.Items[0].GetShipping().GetShippingDate();
			this.ddlShippingTime.SelectedValue = this.CartList.Items[0].GetShipping().GetShippingTime();
		}

		SessionManager.IsAmazonPayRegisterForOrder = false;
	}
	#endregion

	/// <summary>
	/// 注文情報取得
	/// </summary>
	/// <param name="orderReferenceIdOrBillingAgreementId">注文リファレンスIDor支払い契約ID</param>
	/// <param name="orderType">注文種別</param>
	/// <param name="addressType">住所種別</param>
	/// <returns>エラーメッセージ</returns>
	[WebMethod]
	public static string GetAmazonAddress(string orderReferenceIdOrBillingAgreementId, string orderType, string addressType)
	{
		// トークン取得
		var session = HttpContext.Current.Session;
		var amazonModel = (AmazonModel)session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
		var token = amazonModel.Token;

		// 注文種別、住所種別
		AmazonConstants.OrderType eOrderType;
		AmazonConstants.AddressType eAddressType;
		var isValidOrderType = Enum.TryParse<AmazonConstants.OrderType>(orderType, out eOrderType);
		var isValidAddressType = Enum.TryParse<AmazonConstants.AddressType>(addressType, out eAddressType);
		var errorMessage = string.Empty;
		if (string.IsNullOrEmpty(orderReferenceIdOrBillingAgreementId)
			|| string.IsNullOrEmpty(token)
			|| (isValidOrderType == false)
			|| (isValidAddressType == false))
		{
			errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_INVALID_NAME_FOR_AMAZON_ADDRESS_WIDGET);
			return JsonConvert.SerializeObject(new { Error = errorMessage });
		}

		// ウィジェットから住所情報取得
		AmazonAddressInput input = null;
		if (eOrderType == AmazonConstants.OrderType.OneTime)
		{
			var res = AmazonApiFacade.GetOrderReferenceDetails(orderReferenceIdOrBillingAgreementId, token);
			input = new AmazonAddressInput(res);
		}
		else
		{
			var res = AmazonApiFacade.GetBillingAgreementDetails(orderReferenceIdOrBillingAgreementId, token);
			input = new AmazonAddressInput(res);
		}

		// 入力チェック
		errorMessage = input.Validate();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			// エラーメッセージ保持
			if (eAddressType == AmazonConstants.AddressType.Owner) session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS_ERROR_MSG] = errorMessage;
			if (eAddressType == AmazonConstants.AddressType.Shipping) session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS_ERROR_MSG] = errorMessage;
			return JsonConvert.SerializeObject(new { Error = errorMessage });
		}

		var oldModel = (AmazonAddressModel)session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS];
		// モデル生成
		var model = AmazonAddressParser.Parse(input);
		if (eAddressType == AmazonConstants.AddressType.Owner)
		{
			session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS] = model;
			session[AmazonConstants.SESSION_KEY_AMAZON_OWNER_ADDRESS_ERROR_MSG] = string.Empty;
		}
		if (eAddressType == AmazonConstants.AddressType.Shipping)
		{
			session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS] = model;
			session[AmazonConstants.SESSION_KEY_AMAZON_SHIPPING_ADDRESS_ERROR_MSG] = string.Empty;
		}

		session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS] = model;

		if ((oldModel == null)
			|| (model.Addr != oldModel.Addr)
			|| (model.Addr1 != oldModel.Addr1)
			|| (model.Addr2 != oldModel.Addr2)
			|| (model.Addr3 != oldModel.Addr3)
			|| (model.Addr4 != oldModel.Addr4))
		{
			return JsonConvert.SerializeObject(
				new
				{
					RequestPostBack = string.Empty
				});
		}

		return JsonConvert.SerializeObject(new { Error = string.Empty });
	}

	/// <summary>
	/// カスタムバリデータの属性値を変更する（EFOオプションONのとき、カスタムバリデータを無効化する）
	/// </summary>
	public void UpdateAttributeValueForCustomValidator()
	{
		// EFOオプションチェック（有効な場合、カスタムバリデータを無効化）
		if (this.IsEfoOptionEnabled) SetCustomValidatorControlsForEfoOption();

		// AmazonPayの請求先住所取得オプションの対象の場合、一部カスタムバリデータを無効化
		if (IsTargetToExtendedAmazonAddressManagerOption()) this.Process.SetCustomValidatorControlsForExtendedAmazonAddressManagerOption(this.WrCartList);

		// デフォルトのカスタムバリデータコントロール情報を設定
		SetCustomValidatorControlInformationList(this);
	}

	/// <summary>
	/// EFOオプション時のカスタムバリデータコントロールを設定
	/// </summary>
	private void SetCustomValidatorControlsForEfoOption()
	{
		var searchTag = new List<string>
		{
			"cvOwnerName1",
			"cvOwnerName2",
			"cvOwnerName1",
			"cvOwnerName2",
			"cvOwnerNameKana1",
			"cvOwnerNameKana2",
			"cvOwnerBirth",
			"cvOwnerSex",
			"cvOwnerMailAddr",
			"cvOwnerMailAddrForCheck",
			"cvOwnerMailAddrConf",
			"cvOwnerMailAddr2",
			"cvOwnerMailAddr2Conf",
			"cvOwnerZip1",
			"cvOwnerZip2",
			"cvOwnerAddr1",
			"cvOwnerAddr2",
			"cvOwnerAddr3",
			"cvOwnerTel1_1",
			"cvOwnerTel1_2",
			"cvOwnerTel1_3",
			"cvOwnerTel2_1",
			"cvOwnerTel2_2",
			"cvOwnerTel2_3",
		};
		var repeaterItem = this.WrCartList.Items.Cast<RepeaterItem>().First();
		var customValidatorControls = searchTag
			.Select(target => GetWrappedControl<WrappedCustomValidator>(repeaterItem, target))
			.ToList();

		var searchRepTag = new List<string>
		{
			"cvFixedPurchaseMonth",
			"cvFixedPurchaseMonthlyDate",
			"cvFixedPurchaseWeekOfMonth",
			"cvFixedPurchaseDayOfWeek",
			"cvFixedPurchaseIntervalDays",
		};
		customValidatorControls.AddRange(
			this.WrCartList.Items
				.Cast<RepeaterItem>()
				.ToList()
				.SelectMany(
					rpItem => searchRepTag.Select(tag => GetWrappedControl<WrappedCustomValidator>(rpItem, tag))));

		SetDisableAndHideCustomValidatorControlInformationList(customValidatorControls);
	}

	/// <summary>
	/// 配送先が配送不可エリアに指定されているかチェック
	/// </summary>
	/// <returns>配送不可エリアに指定されているかどうか</returns>
	private bool CheckUnavailableShippingAreaForAmazonInput()
	{
		// 配送先不可エラーメッセージを表示
		var shippingZip = this.AmazonCheckoutSession.ShippingAddress.PostalCode.Replace("-", "");
		return OrderCommon.CheckUnavailableShippingArea(this.UnavailableShippingZip, shippingZip);
	}

	/// <summary>
	/// 「AmazonPayで会員登録する」チェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUserRegisterForExternalSettlement_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.cbUserRegisterForExternalSettlement_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// Link button get authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbGetAuthenticationCode_Click(object sender, EventArgs e)
	{
		this.Process.lbGetAuthenticationCode_Click(sender, e);
	}

	/// <summary>
	/// Link button check authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCheckAuthenticationCode_Click(object sender, EventArgs e)
	{
		this.Process.lbCheckAuthenticationCode_Click(sender, e);
	}

	/// <summary>
	/// 「会員登録する」チェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUserRegister_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.WdvUserRegisterRegulationVisible.Visible = this.WcbUserRegister.Checked;
		this.WupUserRegistRegulationForAmazonPay.Update();
	}

	/// <summary>
	/// 次の画面へ遷移するボタンのメッセージを変更するか
	/// </summary>
	/// <returns>結果</returns>
	protected bool IsChangeMessageForNextButton()
	{
		if (IsTargetToExtendedAmazonAddressManagerOption() == false) return false;

		var userRegisterForExternalSettlementEnabled = WcbUserRegisterForExternalSettlement.Visible
			&& this.WcbUserRegisterForExternalSettlement.Checked;
		var userRegisterEnabled = WcbUserRegister.Visible
			&& this.WcbUserRegister.Checked;
		return userRegisterForExternalSettlementEnabled || userRegisterEnabled;
	}

	/// <summary>AmazonCv2チェックアウトセッション</summary>
	protected CheckoutSessionResponse AmazonCheckoutSession { get; set; }
	/// <summary>AmazonCv2チェックアウトセッションID</summary>
	protected string AmazonCheckoutSessionId
	{
		get { return (string)Session[AmazonCv2Constants.SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID]; }
	}
	/// <summary>アマゾンモデル</summary>
	protected AmazonModel AmazonModel
	{
		get { return (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL]; }
	}
	/// <summary>AmazonPayカウントで会員登録しているか</summary>
	public bool IsUserRegistedForAmazon
	{
		get
		{
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			if (amazonModel == null) return false;
			return (AmazonUtil.GetUserByAmazonUserId(amazonModel.UserId) != null);
		}
	}
	/// <summary>ログインしているAmazonPayユーザーと同じメールアドレスを持つユーザーが存在するか</summary>
	public bool ExistsUserWithSameAmazonEmailAddress
	{
		get
		{
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			if (amazonModel == null) return false;
			var user = new UserInput
			{
				UserId = amazonModel.UserId,
				LoginId = amazonModel.Email
			};
			// DB重複チェック
			var result = (user.ValidateDuplication(UserInput.EnumUserInputValidationKbn.UserRegist).Count > 0);
			return result;
		}
	}
	/// <summary>ログインしているAmazonPayユーザーと同じメールアドレスを持つユーザーが存在するか</summary>
	public bool AmazonPayRegisterVisible
	{
		get { return this.IsAmazonLoggedIn && (this.IsUserRegistedForAmazon == false) && (this.ExistsUserWithSameAmazonEmailAddress == false); }
	}
	/// <summary>注文者情報の入力欄を表示するかどうか</summary>
	protected bool IsOrderOwnerInputEnabled
	{
		get { return this.IsLoggedIn || Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED; }
	}
}
