/*
=========================================================================================================
  Module      : 定期購入情報編集入力ページ処理(FixedPurchaseModifyInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Elogit;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Helper;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.Common.Web;
using w2.Domain.CountryLocation;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.TwFixedPurchaseInvoice;
using w2.Domain.TwUserInvoice;
using w2.Domain.User;
using w2.Domain.UserShipping;
using w2.App.Common.Order.Register;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Input;
using w2.Common.Util;
using System.Web.UI;
using w2.App.Common.Order.Payment;

public partial class Form_FixedPurchase_FixedPurchaseModifyInput : FixedPurchasePage
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
			// パラメータ不正?
			if (this.ActionStatus != Constants.ACTION_STATUS_UPDATE)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 定期購入IDパラメータなし?
			var isExsit = (string.IsNullOrEmpty(this.RequestFixedPurchaseId) == false);

			// 定期購入情報取得
			if (isExsit)
			{
				this.FixedPurchaseContainer = new FixedPurchaseService().GetContainer(this.RequestFixedPurchaseId);
				if (this.FixedPurchaseContainer == null) isExsit = false;
			}
			// 定期商品が存在する場合は、配送種別情報取得
			if (this.IsItemsExist && isExsit)
			{
				this.ShopShipping = new ShopShippingService().Get(this.FixedPurchaseContainer.ShopId, this.FixedPurchaseContainer.Shippings[0].Items[0].ShippingType);
				if (this.ShopShipping == null) isExsit = false;
			}

			// Taiwan FixedPurchase Invoice
			if (OrderCommon.DisplayTwInvoiceInfo()
				&& isExsit
				&& (this.RequestActionKbn == ACTION_KBN_INVOICE))
			{
				this.TwFixedPurchaseInvoice = (this.TwModifyInfo == null)
					? new TwFixedPurchaseInvoiceService().GetTaiwanFixedPurchaseInvoice(
						this.RequestFixedPurchaseId,
						this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo)
					: this.TwModifyInfo.CreateModel();

				if (this.TwFixedPurchaseInvoice == null) isExsit = false;
			}

			// データが存在しない場合はエラーページへ遷移
			if (isExsit == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 表示コンポーネント初期化
			InitializeComponents();

			if (this.RequestActionKbn == ACTION_KBN_INVOICE)
			{
				ddlUniformInvoice.SelectedValue = this.TwFixedPurchaseInvoice.TwUniformInvoice;
				ddlCarryType.SelectedValue = this.TwFixedPurchaseInvoice.TwCarryType;

				// Set Visible For Uniform Type
				SetVisibleForUniformOption(ddlUniformInvoice.SelectedValue);

				// Set Visible For Carry Type Option
				SetVisibleForCarryTypeOption(ddlCarryType.SelectedValue);

				DataBindForUniformCarryType(ddlUniformInvoice.SelectedValue);
			}

			// DBから取得したデータを画面にセット
			SetValues();

			// 配送不可エリアエラーがある場合、エラーメッセージを表示する
			if (CheckUnavailableShippingAreaForFixedPurchase())
			{
				trFixedPurchaseShippingErrorMessagesTitle.Visible
					= trFixedPurchaseShippingErrorMessages.Visible = true;
				lbFixedPurchaseShippingErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR);
				return;
			}

			// 楽天クレカかつPayTg非利用の場合、新規クレカ登録領域を非表示にする
			phNewCreditCard.Visible = ((Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) || this.IsUserPayTg);

			if ((this.UserCreditCards != null)
				&& ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
					&& (this.IsUserPayTg == false)))
			{
				if (this.UserCreditCards.Any())
				{
					trRegisteredCreditCard.Visible = rbRegisteredCreditCard.Checked = true;
					trUseNowCreditCard.Visible = rbUseNowCreditCard.Checked = false;
				}
				else
				{
					trUseNowCreditCard.Visible = rbUseNowCreditCard.Checked = true;
				}
			}

			// 支配方法選択イベント実行
			ddlOrderPaymentKbn_SelectedIndexChanged(sender, e);

			// 定期購入配送間隔月/日選択イベント実行
			if (this.RequestActionKbn == ACTION_KBN_PATTERN)
			{
				if (rbFixedPurchaseDays.Checked || rbFixedPurchaseWeekAndDay.Checked) ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(sender, e);
				if (rbFixedPurchaseIntervalDays.Checked) ddlIntervalDays_OnSelectedIndexChanged(sender, e);
				if (rbFixedPurchaseWeekAndDayOfWeek.Checked) ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged(sender, e);
			}

			// トークン決済の場合はクライアント検証をオフ
			DisableCreditInputCustomValidatorForGetCreditToken();

			if (this.IsUserPayTg && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.VeriTrans))
			{
				//PayTg端末状態取得
				GetPayTgDeviceStatus();
			}
		}
		else
		{
			// トークンが入力されていたら入力画面を切り替える
			SwitchDisplayForCreditTokenInput();

			if (this.RequestActionKbn == ACTION_KBN_INVOICE)
			{
				// Set Visible For Uniform Type
				SetVisibleForUniformOption(ddlUniformInvoice.SelectedValue);

				// Set Visible For Carry Type Option
				SetVisibleForCarryTypeOption(ddlCarryType.SelectedValue);
			}

			// 選択していた反映パターンを初期化する
			this.AddressUpdatePattern = null;
		}

		RefreshCreditForm();
	}

	/// <summary>
	/// 詳細へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateFixedPurchaseDetailUrl(this.RequestFixedPurchaseId));
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		// 決済情報変更
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		TwFixedPurchaseInvoiceInput twFixedPurchaseInput = null;
		if (this.RequestActionKbn == ACTION_KBN_PAYMENT)
		{
			var liSelectedOrderPayment = GetSelectedOrderPaymentListItem(Constants.FLG_ORDER_MALL_ID_OWN_SITE);	// 定期注文は自社サイトのみ

			// 支払方法が「クレジットカード」？
			input.OrderPaymentKbn = liSelectedOrderPayment.Value;
			if ((input.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT))
			{
				// クレジットカード選択が「現在利用しているクレジットカードを利用する」？
				if (rbUseNowCreditCard.Checked)
				{
					input.SelectCreditCard = FixedPurchaseInput.SelectCreditCardKbn.UseNow;
					if (OrderCommon.CreditInstallmentsSelectable)
					{
						input.CardInstallmentsCode = ddlUseNowCreditCardInstallments.SelectedValue;
					}
				}
				// クレジットカード選択が「新しいクレジットカードを利用する」？
				else if (rbNewCreditCard.Checked && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten || this.IsUserPayTg))
				{
					input.SelectCreditCard = FixedPurchaseInput.SelectCreditCardKbn.New;

					if (this.CanUseCreditCardNoForm == false) return;

					// トークンが取得できていないときはエラーとして扱う(バグ#3554対策)
					if (OrderCommon.CreditTokenUse && string.IsNullOrEmpty(hfCreditToken.Value))
					{
						spanErrorMessageForCreditCard.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
						spanErrorMessageForCreditCard.Style["display"] = "block";
						return;
					}

					// 入力チェック
					var orderCreditCardInput = GetOrderCreditCardInputForFixedPurchasePage(this.FixedPurchaseContainer.UserId);
					var errorMessage = orderCreditCardInput.Validate() ?? string.Empty;

					// payTGチェック
					if ((errorMessage.Length == 0) && this.IsUserPayTg)
					{
						errorMessage = (this.PayTgResponse == null)
							? string.Empty
							: (string)this.PayTgResponse[VeriTransConst.PAYTG_RESPONSE_ERROR] ?? string.Empty;
					}

					if (errorMessage.Length != 0)
					{
						trFixedPurchasePaymentErrorMessagesTitle.Visible
							= trFixedPurchasePaymentErrorMessages.Visible = true;
						lbFixedPurchasePaymentErrorMessages.Text = errorMessage;
						// 処理を抜ける
						return;
					}

					if (this.IsUserPayTg && (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans))
					{
						orderCreditCardInput.ExpireMonth = (string)this.PayTgResponse[VeriTransConst.PAYTG_CARD_EXPIRE_MONTH];
						orderCreditCardInput.ExpireYear = (string)this.PayTgResponse[VeriTransConst.PAYTG_CARD_EXPIRE_YEAR];
						var cardNumber = (string)this.PayTgResponse[VeriTransConst.PAYTG_CARD_NUMBER];
						orderCreditCardInput.CardNo = cardNumber.Substring(cardNumber.Length - 4);
					}
					else if (this.IsUserPayTg)
					{
						orderCreditCardInput.ExpireMonth = ddlCreditExpireMonth.SelectedValue;
						orderCreditCardInput.ExpireYear = ddlCreditExpireYear.SelectedValue;
						orderCreditCardInput.CompanyCode = this.CreditCardCompanyCodebyPayTg;
						orderCreditCardInput.CreditToken = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.CreditTokenbyPayTg);
					}

					// クレジットカード情報をセット
					input.CreditBranchNo = orderCreditCardInput.CreditBranchNo;
					input.CardInstallmentsCode = orderCreditCardInput.InstallmentsCode;
					input.CreditCardInput = orderCreditCardInput;
				}
				// クレジットカード選択が「登録済みのクレジットカードを利用する」？
				else if (rbRegisteredCreditCard.Checked)
				{
					input.SelectCreditCard = FixedPurchaseInput.SelectCreditCardKbn.Registered;
					input.CreditBranchNo = ddlRegisteredCreditCards.SelectedValue;
					if (OrderCommon.CreditInstallmentsSelectable)
					{
						input.CardInstallmentsCode = dllCreditInstallments2.SelectedValue;
					}
				}
				else
				{
					// 未選択の場合
					trFixedPurchasePaymentErrorMessagesTitle.Visible
						= trFixedPurchasePaymentErrorMessages.Visible = true;
					lbFixedPurchasePaymentErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_CREDIT_CARD_SPECIFY_ERROR);
					// 処理を抜ける
					return;
				}
			}
			else if (input.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
			{
				input.SelectCreditCard = FixedPurchaseInput.SelectCreditCardKbn.New;

				var orderInputCreditCard = new OrderCreditCardInput
				{
					CreditBranchNo = CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW,
					CompanyCode = (OrderCommon.CreditCompanySelectable) ? ddlCreditCardCompany.SelectedValue : "",
					CardNo = "XXXXXXXXXXXXXXXX",
					CardNo1 = "XXXXXXXXXXXXXXXX",
					ExpireMonth = "00",
					ExpireYear = "00",
					AuthorName = "XXXXXXXX",
					SecurityCode = (OrderCommon.CreditSecurityCodeEnable) ? StringUtility.ToHankaku(tbCreditSecurityCode.Text).Trim() : null,
					InstallmentsCode = input.CardInstallmentsCode = dllCreditInstallments.SelectedValue,
					DoRegister = cbRegistCreditCard.Checked,
				};
				orderInputCreditCard.RegisterCardName = (orderInputCreditCard.DoRegister ? tbUserCreditCardName.Text : Constants.CREDITCARD_UNREGIST_FIXEDPURCHASE_DISPLAY_NAME);

				// クレジットカード情報をセット
				input.CreditBranchNo = CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;
				input.CardInstallmentsCode = dllCreditInstallments.SelectedValue;
				input.CreditCardInput = orderInputCreditCard;

				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) return;
				// 入力エラー?
				var errorMessage = orderInputCreditCard.Validate();
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					trFixedPurchasePaymentErrorMessagesTitle.Visible
						= trFixedPurchasePaymentErrorMessages.Visible = true;
					lbFixedPurchasePaymentErrorMessages.Text = errorMessage;
					// 処理を抜ける
					return;
				}
			}
			else if (input.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				{
					var accountEmail = PayPalUtility.Account.GetCooperateAccountEmail(this.FixedPurchaseContainer.UserId);
					if (string.IsNullOrEmpty(accountEmail)) return;
				}
			}
			else if (input.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				var user = new UserService().Get(input.UserId);
				var errorMessage = CheckUsedPaymentFixedPurchaseForTriLinkAfterPay(
					input.OrderPaymentKbn,
					input.Shippings[0].ShippingCountryIsoCode,
					user.AddrCountryIsoCode);
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					trFixedPurchasePaymentErrorMessagesTitle.Visible
						= trFixedPurchasePaymentErrorMessages.Visible = true;
					lbFixedPurchasePaymentErrorMessages.Text = errorMessage;
					// 処理を抜ける
					return;
				}
			}
			else if (string.IsNullOrEmpty(input.OrderPaymentKbn))
			{
				return;
			}
			else if (input.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
			{
				input.SelectCreditCard = FixedPurchaseInput.SelectCreditCardKbn.New;
			}
			else
			{
				input.CreditBranchNo = null;
				input.CardInstallmentsCode = "";
			}
		}
		// 配送先情報変更
		if (this.RequestActionKbn == ACTION_KBN_SHIPPING)
		{
			var isConvenienceStore = false;
			var isInputShipping = false;
			var shipping = input.Shippings[0];
			switch (ddlUserShipping.SelectedValue)
			{
				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE:
					isConvenienceStore = true;
					break;

				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
					isConvenienceStore = false;
					isInputShipping = true;
					break;

				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
					shipping.ShippingName1 = this.User.Name1;
					shipping.ShippingName2 = this.User.Name2;
					shipping.ShippingName = this.User.Name;
					shipping.ShippingNameKana1 = this.User.NameKana1;
					shipping.ShippingNameKana2 = this.User.NameKana2;
					shipping.ShippingNameKana = this.User.NameKana;
					shipping.ShippingZip1 = this.User.Zip1;
					shipping.ShippingZip2 = this.User.Zip2;
					shipping.ShippingZip = this.User.Zip;
					shipping.ShippingAddr1 = this.User.Addr1;
					shipping.ShippingAddr2 = this.User.Addr2;
					shipping.ShippingAddr3 = this.User.Addr3;
					shipping.ShippingAddr4 = this.User.Addr4;
					shipping.ShippingAddr5 = this.User.Addr5;
					shipping.ShippingCountryIsoCode = this.User.AddrCountryIsoCode;
					shipping.ShippingCountryName = this.User.AddrCountryName;
					shipping.ShippingCompanyName = this.User.CompanyName;
					shipping.ShippingCompanyPostName = this.User.CompanyName;
					shipping.ShippingTel1_1 = this.User.Tel1_1;
					shipping.ShippingTel1_2 = this.User.Tel1_2;
					shipping.ShippingTel1_3 = this.User.Tel1_3;
					shipping.ShippingTel1 = this.User.Tel1;
					shipping.ShippingTime = ddlShippingTime.SelectedValue;
					break;

				default:
					isConvenienceStore = (this.UserShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
					if (isConvenienceStore == false)
					{
						var userShippings = new UserShippingService().GetAllOrderByShippingNoDesc(this.FixedPurchaseContainer.UserId).ToArray();
						var selectedShipping = userShippings.FirstOrDefault(item => item.ShippingNo == int.Parse(ddlUserShipping.SelectedValue));
						if (selectedShipping != null)
						{
							shipping.ShippingName1 = selectedShipping.ShippingName1;
							shipping.ShippingName2 = selectedShipping.ShippingName2;
							shipping.ShippingName = selectedShipping.ShippingName;
							shipping.ShippingNameKana1 = selectedShipping.ShippingNameKana1;
							shipping.ShippingNameKana2 = selectedShipping.ShippingNameKana2;
							shipping.ShippingNameKana = selectedShipping.ShippingNameKana;
							shipping.ShippingZip1 = selectedShipping.ShippingZip1;
							shipping.ShippingZip2 = selectedShipping.ShippingZip2;
							shipping.ShippingZip = selectedShipping.ShippingZip;
							shipping.ShippingAddr1 = selectedShipping.ShippingAddr1;
							shipping.ShippingAddr2 = selectedShipping.ShippingAddr2;
							shipping.ShippingAddr3 = selectedShipping.ShippingAddr3;
							shipping.ShippingAddr4 = selectedShipping.ShippingAddr4;
							shipping.ShippingAddr5 = selectedShipping.ShippingAddr5;
							shipping.ShippingCountryIsoCode = selectedShipping.ShippingCountryIsoCode;
							shipping.ShippingCountryName = selectedShipping.ShippingCountryName;
							shipping.ShippingCompanyName = selectedShipping.ShippingCompanyName;
							shipping.ShippingCompanyPostName = selectedShipping.ShippingCompanyPostName;
							shipping.ShippingTel1_1 = selectedShipping.ShippingTel1_1;
							shipping.ShippingTel1_2 = selectedShipping.ShippingTel1_2;
							shipping.ShippingTel1_3 = selectedShipping.ShippingTel1_3;
							shipping.ShippingTel1 = selectedShipping.ShippingTel1;
							shipping.ShippingTime = ddlShippingTime.SelectedValue;
						}
					}
					break;
			}

			input.Shippings[0].ShippingReceivingStoreFlg = (isConvenienceStore
				? Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON
				: Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
			if (isInputShipping)
			{
				shipping.ShippingName1 = DataInputUtility.ConvertToFullWidthBySetting(tbShippingName1.Text.Trim(), this.IsShippingAddrJp);
				shipping.ShippingName2 = DataInputUtility.ConvertToFullWidthBySetting(tbShippingName2.Text.Trim(), this.IsShippingAddrJp);
				shipping.ShippingName = shipping.ShippingName1 + shipping.ShippingName2;
				shipping.ShippingNameKana1 = StringUtility.ToZenkaku(tbShippingNameKana1.Text.Trim());
				shipping.ShippingNameKana2 = StringUtility.ToZenkaku(tbShippingNameKana2.Text.Trim());
				shipping.ShippingNameKana = shipping.ShippingNameKana1 + shipping.ShippingNameKana2;
				shipping.ShippingZip1 = StringUtility.ToHankaku(tbShippingZip1.Text.Trim());
				shipping.ShippingZip2 = StringUtility.ToHankaku(tbShippingZip2.Text.Trim());
				shipping.ShippingZip = shipping.ShippingZip1 + "-" + shipping.ShippingZip2;
				shipping.ShippingAddr1 = ddlShippingAddr1.SelectedValue;
				shipping.ShippingAddr2 = DataInputUtility.ConvertToFullWidthBySetting(tbShippingAddr2.Text.Trim(), this.IsShippingAddrJp);
				shipping.ShippingAddr3 = DataInputUtility.ConvertToFullWidthBySetting(tbShippingAddr3.Text.Trim(), this.IsShippingAddrJp);
				shipping.ShippingAddr4 = DataInputUtility.ConvertToFullWidthBySetting(tbShippingAddr4.Text.Trim(), this.IsShippingAddrJp);
				shipping.ShippingAddr5 = string.Empty;
				shipping.ShippingCountryIsoCode = string.Empty;
				shipping.ShippingCountryName = string.Empty;
				shipping.ShippingCompanyName = DataInputUtility.ConvertToFullWidthBySetting(tbShippingCompanyName.Text.Trim(), this.IsShippingAddrJp);
				shipping.ShippingCompanyPostName = DataInputUtility.ConvertToFullWidthBySetting(tbShippingCompanyPostName.Text.Trim(), this.IsShippingAddrJp);
				shipping.ShippingTel1_1 = StringUtility.ToHankaku(tbShippingTel1.Text.Trim());
				shipping.ShippingTel1_2 = StringUtility.ToHankaku(tbShippingTel2.Text.Trim());
				shipping.ShippingTel1_3 = StringUtility.ToHankaku(tbShippingTel3.Text.Trim());
				shipping.ShippingTel1 = shipping.ShippingTel1_1 + "-" + shipping.ShippingTel1_2 + "-" + shipping.ShippingTel1_3;
				shipping.ShippingTime = ddlShippingTime.SelectedValue;
			}
			shipping.ShippingMethod = ddlShippingMethod.SelectedValue;
			shipping.DeliveryCompanyId = ddlDeliveryCompany.SelectedValue;
			this.DeliveryCompany = this.DeliveryCompanyList.First(i => i.DeliveryCompanyId == ddlDeliveryCompany.SelectedValue);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				if (isConvenienceStore == false)
				{
					shipping.ShippingCountryIsoCode = ddlShippingCountry.SelectedValue;
					shipping.ShippingCountryName = ddlShippingCountry.SelectedItem.Text;

					if (this.IsShippingAddrJp == false)
					{
						shipping.ShippingZip = StringUtility.ToHankaku(tbShippingZipGlobal.Text.Trim());
						shipping.ShippingZip1 = string.Empty;
						shipping.ShippingZip2 = string.Empty;
						shipping.ShippingTel1 = StringUtility.ToHankaku(tbShippingTel1Global.Text.Trim());
						shipping.ShippingTel1_1 = string.Empty;
						shipping.ShippingTel1_2 = string.Empty;
						shipping.ShippingTel1_3 = string.Empty;
						shipping.ShippingNameKana1 = string.Empty;
						shipping.ShippingNameKana2 = string.Empty;
						shipping.ShippingNameKana = string.Empty;
						shipping.ShippingAddr5 = this.IsShippingAddrUs
							? ddlShippingAddr5.SelectedValue
							: DataInputUtility.ConvertToFullWidthBySetting(tbShippingAddr5.Text.Trim(), this.IsShippingAddrJp);
					}
				}

				// 台湾後払いの利用可否を判定する
				if (input.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					var user = new UserService().Get(input.UserId);
					var errorMessage = CheckUsedPaymentFixedPurchaseForTriLinkAfterPay(
						input.OrderPaymentKbn,
						shipping.ShippingCountryIsoCode,
						user.AddrCountryIsoCode);
					if (string.IsNullOrEmpty(errorMessage) == false)
					{
						trFixedPurchaseShippingErrorMessagesTitle.Visible
							= trFixedPurchaseShippingErrorMessages.Visible = true;
						lbFixedPurchaseShippingErrorMessages.Text = errorMessage;
						// 処理を抜ける
						return;
					}
				}
			}

			if (isConvenienceStore)
			{
				input.Shippings[0].ShippingReceivingStoreId = hfCvsShopId.Value;
				input.Shippings[0].ShippingName = hfCvsShopName.Value;
				input.Shippings[0].ShippingAddr4 = hfCvsShopAddress.Value;
				input.Shippings[0].ShippingTel1 = hfCvsShopTel.Value;
				lCvsShopId.Text = WebSanitizer.HtmlEncode(hfCvsShopId.Value);
				lCvsShopName.Text = WebSanitizer.HtmlEncode(hfCvsShopName.Value);
				lCvsShopAddress.Text = WebSanitizer.HtmlEncode(hfCvsShopAddress.Value);
				lCvsShopTel.Text = WebSanitizer.HtmlEncode(hfCvsShopTel.Value);

				if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
				{
					input.Shippings[0].ShippingReceivingStoreType = ddlShippingReceivingStoreType.SelectedValue;
				}

				if (string.IsNullOrEmpty(input.Shippings[0].ShippingReceivingStoreId) || string.IsNullOrEmpty(input.Shippings[0].ShippingName))
				{
					trFixedPurchaseShippingErrorMessagesTitle.Visible
						= trFixedPurchaseShippingErrorMessages.Visible = true;
					lbFixedPurchaseShippingErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CONVENIENCE_STORE_IS_NOT_SELECTED);
					return;
				}

				// Check Id Exists In Xml Store
				if ((OrderCommon.CheckIdExistsInXmlStoreBatchFixedPurchase(input.Shippings[0].ShippingReceivingStoreId) == false)
					&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false))
				{
					trFixedPurchaseShippingErrorMessagesTitle.Visible
						= trFixedPurchaseShippingErrorMessages.Visible = true;
					lbFixedPurchaseShippingErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_CONVENIENCE_STORE_NOT_VALID);
					return;
				}
			}

			// 配送先情報反映対象をセッションにセット
			var patternList = new List<string>();
			foreach (ListItem item in cblUpdatePattern.Items)
			{
				if (item.Selected) patternList.Add(item.Value);
			}
			this.AddressUpdatePattern = patternList.ToArray();

			shipping.UserShippingKbn = ddlUserShipping.SelectedValue;

			// 入力エラー?
			if (isInputShipping && (input.Validate(true) == false))
			{
				trFixedPurchaseShippingErrorMessagesTitle.Visible
					= trFixedPurchaseShippingErrorMessages.Visible = true;
				lbFixedPurchaseShippingErrorMessages.Text = input.ErrorMessage;
				// 処理を抜ける
				return;
			}

			// 配送不可エリアエラーがある場合、エラーメッセージを表示する
			var unavailableShippingZip = GetUnavailableShippingZip(this.ShopShipping.ShippingId, this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId);
			var shippingZip = input.Shippings[0].HyphenlessShippingZip;
			var unavailableShipping = OrderCommon.CheckUnavailableShippingArea(unavailableShippingZip, shippingZip);
			if (unavailableShipping)
			{
				trFixedPurchaseShippingErrorMessagesTitle.Visible
					= trFixedPurchaseShippingErrorMessages.Visible = true;
				lbFixedPurchaseShippingErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR);
				return;
			}
		}
		// 配送パターン変更
		else if (this.RequestActionKbn == ACTION_KBN_PATTERN)
		{
			// Xか月ごとY日
			if ((rbFixedPurchaseDays.Checked) && (rbFixedPurchaseDays.Visible))
			{
				input.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE;
				input.FixedPurchaseSetting1 = ddlMonth.SelectedValue + "," + ddlMonthlyDate.SelectedValue;
				input.FixedPurchaseSetting1_1_1 = ddlMonth.SelectedValue;
				input.FixedPurchaseSetting1_1_2 = ddlMonthlyDate.SelectedValue;
			}
			// Xヶ月ごと第Y Z曜日
			else if ((rbFixedPurchaseWeekAndDay.Checked) && (rbFixedPurchaseWeekAndDay.Visible))
			{
				input.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY;
				input.FixedPurchaseSetting1 = string.Format("{0},{1},{2}",
					ddlIntervalMonths.SelectedValue,
					ddlWeekOfMonth.SelectedValue,
					ddlDayOfWeek.SelectedValue);
				input.FixedPurchaseSetting_IntervalMonths = ddlIntervalMonths.SelectedValue;
				input.FixedPurchaseSetting1_2_1 = ddlWeekOfMonth.SelectedValue;
				input.FixedPurchaseSetting1_2_2 = ddlDayOfWeek.SelectedValue;
			}
			// X日 間隔
			else if ((rbFixedPurchaseIntervalDays.Checked) && (rbFixedPurchaseIntervalDays.Visible))
			{
				input.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS;
				input.FixedPurchaseSetting1 = ddlIntervalDays.SelectedValue;
				input.FixedPurchaseSetting1_3 = ddlIntervalDays.SelectedValue;
			}
			//週間隔・曜日指定
			else if ((rbFixedPurchaseWeekAndDayOfWeek.Checked) && (rbFixedPurchaseWeekAndDayOfWeek.Visible))
			{
				input.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY;
				input.FixedPurchaseSetting1 = string.Format(
						"{0},{1}",
						ddlFixedPurchaseEveryNWeek_Week.SelectedValue,
						ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue);
				input.FixedPurchaseSetting1_4_1 = ddlFixedPurchaseEveryNWeek_Week.SelectedValue;
				input.FixedPurchaseSetting1_4_2 = ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue;
			}

			// 入力チェック
			var errorMessage = "";
			if (input.Validate(false) == false) errorMessage = input.ErrorMessage;

			// 配送日正当性チェック
			errorMessage += ValidateShippingDate(ucNextShippingDate.DateString, ucNextNextShippingDate.DateString);

			// 入力エラー?
			if (errorMessage.Length != 0)
			{
				trFixedPurchasePatternErrorMessagesTitle.Visible
					= trFixedPurchasePatternErrorMessages.Visible = true;
				lbFixedPurchasePatternErrorMessages.Text = errorMessage;
				// 処理を抜ける
				return;
			}

			// 最後の定期購入日から配送日を自動計算するチェックあり?
			var nextShippingDate = DateTime.Parse(input.NextShippingDate);
			var nextNextShippingDate = DateTime.Parse(input.NextNextShippingDate);
			var service = new FixedPurchaseService();
			var calculateMode = service.GetCalculationMode(
				input.FixedPurchaseKbn,
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
			if (cbScheduleAutoComputeFlg.Checked)
			{
				var lastOrder = new OrderService().GetLastFixedPurchaseOrder(input.FixedPurchaseId);
				// 前回注文日（出荷日ではない）をベースに再計算する
				nextShippingDate =
					service.CalculateNextShippingDateFromLastShippedDate(
						input.FixedPurchaseKbn,
						input.FixedPurchaseSetting1,
						lastOrder.ArrivalScheduleDate,
						this.ShopShipping.FixedPurchaseMinimumShippingSpan,
						0);	// キャンセル期限は考慮しない
				nextNextShippingDate =
					service.CalculateFollowingShippingDate(
						input.FixedPurchaseKbn,
						input.FixedPurchaseSetting1,
						nextShippingDate,
						0,
						calculateMode);
			}
			// 日付を指定する場合
			else
			{
				nextShippingDate = DateTime.Parse(ucNextShippingDate.DateString);
				// 次回配送日から自動計算するチェックあり?
				if (cbNextNextShippingDateAutoComputeFlg.Checked)
				{
					// 次回配送日より算出する
					nextNextShippingDate =
						service.CalculateNextShippingDate(
							input.FixedPurchaseKbn,
							input.FixedPurchaseSetting1,
							nextShippingDate,
							this.ShopShipping.FixedPurchaseShippingDaysRequired,
							this.ShopShipping.FixedPurchaseMinimumShippingSpan,
							calculateMode);
				}
				else
				{
					// 選択された日付を登録する
					nextNextShippingDate = DateTime.Parse(ucNextNextShippingDate.DateString);
				}
			}
			input.NextShippingDate = nextShippingDate.ToString();
			input.NextNextShippingDate = nextNextShippingDate.ToString();
		}
		// 商品変更
		else if (this.RequestActionKbn == ACTION_KBN_ITEM)
		{
			// 新商品を取得する
			var newItems = GetItems().ToArray();

			// 新商品の配送種別マスタを取得
			var newShopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(newItems[0].ShippingType);

			// 新商品の配送サービス一覧を取得
			var newCompanyList =
				(input.Shippings[0].ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					? newShopShipping.CompanyListExpress
					: newShopShipping.CompanyListMail;

			// この定期注文の配送サービスが新商品の利用できる配送サービスではない場合、エラーメッセージを表示する
			if (newCompanyList.Any(item => (item.DeliveryCompanyId == input.Shippings[0].DeliveryCompanyId)) == false)
			{
				trFixedPurchaseItemErrorMessagesTitle.Visible = trFixedPurchaseItemErrorMessages.Visible = true;
				lbFixedPurchaseItemErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELIVERY_COMPANY_DIFFERENT_TO_ORDER);

				return;
			}

			// 商品セット
			input.Shippings[0].Items = newItems;

			// 入力エラー?
			if (input.Validate(false, this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false)
			{
				trFixedPurchaseItemErrorMessagesTitle.Visible
					= trFixedPurchaseItemErrorMessages.Visible = true;
				lbFixedPurchaseItemErrorMessages.Text = input.ErrorMessage;

				// 処理を抜ける
				return;
			}

			// 商品付帯情報の入力チェック
			if (input.Shippings[0].Items.Any(fpItem => fpItem.ProductOptionSettingList != null))
			{
				var errorMsg = ValidateProductOptionValue(input.Shippings[0].Items);
				if (string.IsNullOrEmpty(errorMsg) == false)
				{
					trFixedPurchaseItemErrorMessagesTitle.Visible
						= trFixedPurchaseItemErrorMessages.Visible = true;
					lbFixedPurchaseItemErrorMessages.Text = errorMsg;

					// 処理を抜ける
					return;
				}
			}

			var shipping = input.Shippings[0];
			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& (shipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
			{
				foreach (var item in shipping.Items)
				{
					var shopShipping = new ShopShippingService().Get(input.ShopId, item.ShippingType);
					var deliveryCompanyList = (shipping.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
						? shopShipping.CompanyListExpress
						: shopShipping.CompanyListMail;

					if (deliveryCompanyList.Any(
						deliveryCompany => deliveryCompany.DeliveryCompanyId
							== Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID) == false)
					{
						trFixedPurchaseItemErrorMessagesTitle.Visible = trFixedPurchaseItemErrorMessages.Visible = true;
						lbFixedPurchaseItemErrorMessages.Text = WebMessages.GetMessages(
							WebMessages.ERRMSG_MANAGER_PRODUCT_SHIPPING_KBN_DIFFERENT_TO_ORDER);

						return;
					}
				}
			}

			// 頒布会の注文か
			if (this.IsSubscriptionBox)
			{
				// 商品の定期購入価格が設定されない場合は、商品表示価格で計算する
				var amount = input.Shippings[0].Items.Sum(x => (x.FixedPurchasePrice ?? x.Price) * int.Parse(x.ItemQuantity));

				// 頒布会コースの商品合計金額（税込）設定の条件を満たしているかチェック
				var errorMessage = OrderCommon.GetSubscriptionBoxTotalAmountError(
					this.FixedPurchaseContainer.SubscriptionBoxCourseId,
					decimal.Parse(amount.ToPriceString()));
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					trFixedPurchaseItemErrorMessages.Visible = true;
					lbFixedPurchaseItemErrorMessages.Text = errorMessage;
					return;
				}
			}
		}
		else if (this.RequestActionKbn == ACTION_KBN_INVOICE)
		{
			// FixedPurchase Invoice
			twFixedPurchaseInput = GetTwFxiedPurchaseInvoiceInput();
			var parameter = new Hashtable();
			var errorMessage = string.Empty;

			switch (ddlUniformInvoice.SelectedValue)
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
					{
						if (ddlCarryType.Text == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
						{
							parameter.Add(string.Format("{0}_1", Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION), twFixedPurchaseInput.TwCarryTypeOption);
						}
						else if (ddlCarryType.Text == Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE)
						{
							parameter.Add(string.Format("{0}_2", Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION), twFixedPurchaseInput.TwCarryTypeOption);
						}
					}
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
					{
						if (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue))
						{
							parameter.Add(Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1, twFixedPurchaseInput.TwUniformInvoiceOption1);
							parameter.Add(Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2, twFixedPurchaseInput.TwUniformInvoiceOption2);
						}
					}
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
					{
						if (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue))
						{
							parameter.Add(string.Format("{0}_donate", Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1), twFixedPurchaseInput.TwUniformInvoiceOption1);
						}
					}
					break;
			}

			errorMessage = Validator.Validate("OrderRegistInput", parameter);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				trInvoiceErrorMessage.Visible = true;
				lbInvoiceError.Text = errorMessage;

				return;
			}
		}

		// セッションに入力内容を格納し、確認画面へ
		this.ModifyInfo = input;
		this.TwModifyInfo = twFixedPurchaseInput;
		Response.Redirect(CreateFixedPurchaseConfirmUrl());
	}

	/// <summary>
	/// 支払方法選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOrderPaymentKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// ユーザークレジットカード情報・表示制御
		trPaymentKbnCredit.Visible = (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
		// クレジットカード選択イベント実行
		rbCreditCard_OnCheckedChanged(sender, e);

		lbOrderPaymentInfo.Text = "";
		if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
		{
			var accountEmail = PayPalUtility.Account.GetCooperateAccountEmail(this.FixedPurchaseContainer.UserId);
			lbOrderPaymentInfo.Text = (string.IsNullOrEmpty(accountEmail))
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_ISNOT_LINK_ACCOUNT)
				: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_AVAILABLE_ACCOUNT).Replace("@@ 1 @@", accountEmail);
		}
	}

	/// <summary>
	/// クレジットカード選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbCreditCard_OnCheckedChanged(object sender, EventArgs e)
	{
		// クレジットカード選択・表示制御
		trUseNowCreditCard.Visible = rbUseNowCreditCard.Checked;
		tbodyNewCreditCard.Visible = rbNewCreditCard.Checked;
		trRegisteredCreditCard.Visible = rbRegisteredCreditCard.Checked;
		// 登録済みクレジットカード選択イベント実行
		ddlRegisteredCreditCards_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 登録済みクレジットカード選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlRegisteredCreditCards_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// 支払方法が「クレジットカード」？
		if (trPaymentKbnCredit.Visible)
		{
			// クレジットカード選択が「登録済みのクレジットカードを利用する」？
			if (rbRegisteredCreditCard.Checked)
			{
				// ユーザーカード情報表示
				var userCreditCard = this.UserCreditCards.FirstOrDefault(uc => uc.BranchNo == int.Parse(ddlRegisteredCreditCards.SelectedValue));
				if (userCreditCard != null)
				{
					lRegisteredCreditCardLastFourDigit.Text = userCreditCard.LastFourDigit;
					lRegisteredCreditCardExpirationMonth.Text = userCreditCard.ExpirationMonth;
					lRegisteredCreditCardExpirationYear.Text = userCreditCard.ExpirationYear;
					lRegisteredCreditCardAuthorName.Text = userCreditCard.AuthorName;
				}
			}
		}
	}

	/// <summary>
	/// クレジットカード登録する選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbRegistCreditCard_OnCheckedChanged(object sender, System.EventArgs e)
	{
		// クレジットカード登録名・表示制御
		trUserCreditCardName.Visible = cbRegistCreditCard.Checked;
	}

	/// <summary>
	/// 配送方法選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingMethod_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		var isShippingConvenience = (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE);
		RefreshDeliveryCompanyForDisplay(isShippingConvenience);
		ddlDeliveryCompany_SelectedIndexChanged(sender, e);

		var selectedValue = ddlUserShipping.SelectedValue;
		ddlUserShipping.Items.Clear();
		var userShippings = new UserShippingService().GetAllOrderByShippingNoDesc(this.FixedPurchaseContainer.UserId).ToArray();
		BindShippingList(userShippings);

		if (ddlUserShipping.Items.FindByValue(selectedValue) != null)
		{
			ddlUserShipping.SelectedValue = selectedValue;
		}
		else
		{
			ddlUserShippingKbn_SelectedIndexChanged(ddlUserShipping, e);
		}
	}

	/// <summary>
	/// 配送会社選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDeliveryCompany_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		ddlShippingTime.Items.Clear();
		if (string.IsNullOrEmpty(ddlDeliveryCompany.SelectedValue) == false)
		{
			this.DeliveryCompany = this.DeliveryCompanyList.First(i => i.DeliveryCompanyId == ddlDeliveryCompany.SelectedValue);
		}
		var companyDefault = this.DeliveryCompanyList.FirstOrDefault(i => i.DeliveryCompanyId == ddlDeliveryCompany.SelectedValue);
		if (companyDefault != null)
		{
			this.DeliveryCompany = companyDefault;
		}

		ddlShippingTime.Items.Add(new ListItem(ReplaceTag("@@DispText.shipping_time_list.none@@"), ""));
		ddlShippingTime.Items.AddRange(GetShippingTimeList(this.DeliveryCompany).Cast<ListItem>().ToArray());

		foreach (ListItem li in ddlShippingTime.Items)
		{
			li.Selected = (li.Value == this.FixedPurchaseContainer.Shippings[0].ShippingTime);
		}
	}

	/// <summary>
	/// 商品追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddProduct_Click(object sender, EventArgs e)
	{
		// 商品情報取得
		var items = GetItems().ToList();

		// 空データ追加
		var addItem = new FixedPurchaseItemInput()
		{
			FixedPurchaseId = this.FixedPurchaseContainer.FixedPurchaseId,
			FixedPurchaseItemNo = (items.Count + 1).ToString(),
			FixedPurchaseShippingNo = this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo.ToString(),
			ShopId = this.FixedPurchaseContainer.ShopId,
			ProductId = "",
			VariationId = "",
			ItemQuantity = "0",
			Price = 0,
			ItemOrderCount = "0",
			ItemShippedCount = "0"
		};
		items.Add(addItem);

		// Check if there is one item: Don't allow to delete
		this.CanDelete = (items.Count > 1);
		// データバインド
		rItemList.DataSource = items;
		rItemList.DataBind();
	}

	/// <summary>
	/// 再計算ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReCalculate_Click(object sender, EventArgs e)
	{
		// 商品情報取得
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		input.Shippings[0].Items = GetItems().ToArray();

		// 入力エラー?
		trFixedPurchaseItemErrorMessagesTitle.Visible
			= trFixedPurchaseItemErrorMessages.Visible = false;
		lbFixedPurchaseItemErrorMessages.Text = "";

		// 検証
		var errorMessage = string.Concat(input.ErrorMessage, CheckLimitedPayment(input));

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			// エラーメッセージ表示
			trFixedPurchaseItemErrorMessagesTitle.Visible
				= trFixedPurchaseItemErrorMessages.Visible = true;
			lbFixedPurchaseItemErrorMessages.Text = HtmlSanitizer.HtmlEncode(errorMessage);
		}

		var userShippings = new UserShippingService().GetAllOrderByShippingNoDesc(this.FixedPurchaseContainer.UserId).ToArray();
		BindShippingList(userShippings);

		// Check if there is one item: Don't allow to delete
		this.CanDelete = (input.Shippings[0].Items.Length > 1);
		// データバインド
		rItemList.DataSource = input.Shippings[0].Items;
		rItemList.DataBind();
	}

	/// <summary>
	/// 商品情報リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rItemList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// 取得
		if (e.CommandName == "get")
		{
			// 再計算ボタンクリックイベントにて取得処理実行
			btnReCalculate_Click(source, e);
		}
		// 削除
		else if (e.CommandName == "delete")
		{
			// 削除対象を除外したリスト取得
			var fixedPurchaseItemNo = e.Item.ItemIndex + 1;
			var items = GetItems().Where(i => i.FixedPurchaseItemNo != fixedPurchaseItemNo.ToString()).ToArray();
			// 定期購入注文商品枝番を振り直す
			int itemNo = 1;
			foreach (FixedPurchaseItemInput item in items)
			{
				item.FixedPurchaseItemNo = itemNo.ToString();
				itemNo++;
			}

			// Check if there is one item: Don't allow to delete
			this.CanDelete = (items.Length > 1);
			rItemList.DataSource = items;
			rItemList.DataBind();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 編集領域表示制御
		dvFixedPurchasePayment.Visible = (this.RequestActionKbn == ACTION_KBN_PAYMENT);
		dvFixedPurchaseShipping.Visible = (this.RequestActionKbn == ACTION_KBN_SHIPPING);
		dvFixedPurchasePattern.Visible = (this.RequestActionKbn == ACTION_KBN_PATTERN);
		dvFixedPurchaseItemList.Visible = (this.RequestActionKbn == ACTION_KBN_ITEM);
		spanUpdateHistoryConfirmTop.Visible = (this.RequestActionKbn == ACTION_KBN_ITEM);
		dvInvoice.Visible = (this.RequestActionKbn == ACTION_KBN_INVOICE);

		// 決済情報
		if (this.RequestActionKbn == ACTION_KBN_PAYMENT)
		{
			// ユーザ―情報 & ユーザークレジットカード情報取得
			var user = new UserService().Get(this.FixedPurchaseContainer.UserId);
			this.UserCreditCards = UserCreditCard.GetUsable(this.FixedPurchaseContainer.UserId);

			var payments = GetPaymentValidListPermission(
				this.LoginOperatorShopId,
				this.ShopShipping.PaymentSelectionFlg,
				this.ShopShipping.PermittedPaymentIds,
				true);
			if (OrderCommon.CheckPaymentYamatoKaSms(this.FixedPurchaseContainer.OrderPaymentKbn) == false)
			{
				payments = payments.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)).ToArray();
			}
			var input = new FixedPurchaseInput(this.FixedPurchaseContainer);

			var regKey = Constants.PAYMENT_LINEPAY_OPTION_ENABLED
				? user.UserExtend.UserExtendDataValue[Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY]
				: string.Empty;
			foreach (var payment in payments)
			{
				if ((input.Shippings[0].ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
					&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT))
				{
					continue;
				}
				if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
					|| (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					|| (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					|| ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
						&& string.IsNullOrEmpty(regKey))
					|| ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
						&& (input.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))) continue;

				if (((input.Shippings[0].ShippingReceivingStoreFlg != Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED && (IsPaymentAtConvenienceStore(input) == false)))
					&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)) continue;

				if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (OrderCommon.HasPaymentDataAfteeOrAtone(
						this.FixedPurchaseContainer.FixedPurchaseId,
						Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) == false)) continue;
				if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (OrderCommon.HasPaymentDataAfteeOrAtone(
						this.FixedPurchaseContainer.FixedPurchaseId,
						Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) == false)) continue;

				if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& input.Shippings[0].Items.Any(product =>
						(product.DigitalContentsFlg == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID))) continue;

				// 継続課金利用可能な決済の追加制御
				if (OrderCommon.IsUsablePaymentContinuous(payment.PaymentId)
					&& (payment.PaymentId != this.FixedPurchaseContainer.OrderPaymentKbn)) continue;

				// Remove payment boku
				if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (input.OrderPaymentKbn != payment.PaymentId)) continue;

				var item = new ListItem(payment.PaymentName, payment.PaymentId);
				ddlOrderPaymentKbn.Items.Add(item);
			}

			// 現在利用しているクレジットカードを利用する部・表示制御
			divUseNowCreditCard.Visible = (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			// 登録済みのクレジットカードを利用する部・表示制御
			divRegisteredCreditCard.Visible = (OrderCommon.CreditCardRegistable && user.IsMember && this.UserCreditCards.Length > 0);
			// カード会社・表示制御
			if (OrderCommon.CreditCompanySelectable)
			{
				ddlCreditCardCompany.Items.Add(new ListItem("", ""));
				ddlCreditCardCompany.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName));
			}
			ddlRegisteredCreditCards.DataSource = this.UserCreditCards;
			ddlRegisteredCreditCards.DataBind();
			// カード有効期限(月)
			ddlCreditExpireMonth.Items.AddRange(this.CreditExpirationMonthListItems);
			ddlCreditExpireMonth.SelectedValue = DateTime.Now.Month.ToString("00");
			// カード有効期限(年)
			ddlCreditExpireYear.Items.AddRange(this.CreditExpirationYearListItems);
			ddlCreditExpireYear.SelectedValue = DateTime.Now.Year.ToString("00").Substring(2);
			// カード分割支払い・表示制御
			trUseNowCreditCardInstallments.Visible
				= trInstallments.Visible
				= trRegisteredCreditCardInstallments.Visible = OrderCommon.CreditInstallmentsSelectable;
			if (OrderCommon.CreditInstallmentsSelectable)
			{
				ddlUseNowCreditCardInstallments.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName));
				dllCreditInstallments.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName));
				dllCreditInstallments2.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName));
			}
			// クレジットカード登録 / 登録名・表示制御
			trRegistCreditCard.Visible = (OrderCommon.CreditCardRegistable && user.IsMember && (Constants.MAX_NUM_REGIST_CREDITCARD > this.UserCreditCards.Length));
			trUserCreditCardName.Visible = false;
			// セキュリティコード・表示制御
			trUseNowCreditCardSecurityCode.Visible
				= trRegisteredCreditCardSecurityCode.Visible = OrderCommon.CreditSecurityCodeEnable;
			trSecurityCode.Visible =
				((this.IsUserPayTg == false) && (trRegisteredCreditCardSecurityCode.Visible = OrderCommon.CreditSecurityCodeEnable));

			if (this.IsUserPayTg && (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans))
			{
				if (string.IsNullOrEmpty(tbCreditCardNo1.Text)) tbCreditCardNo1.Text = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_CARD_NUMBER;
				ddlCreditExpireMonth.SelectedValue = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_MONTH;
				ddlCreditExpireYear.SelectedValue = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_YEAR;
			}
			DisplayPaymentUserManagementLevel(this.LoginOperatorShopId, user);
			DisplayPaymentOrderOwnerKbn(this.LoginOperatorShopId, user.UserKbn);

			tdCreditNumber.Visible = (this.IsUserPayTg == false);
			trCreditExpire.Visible = (this.IsUserPayTg == false);
			tdGetCardInfo.Visible = this.IsUserPayTg;
		}
		// 配送先情報
		else if (this.RequestActionKbn == ACTION_KBN_SHIPPING)
		{
			// 都道府県
			ddlShippingAddr1.Items.Add("");
			foreach (var prefectures in Constants.STR_PREFECTURES_LIST)
			{
				ddlShippingAddr1.Items.Add(prefectures);
			}
			// 国
			var shippingAvailableCountry = new CountryLocationService().GetShippingAvailableCountry();
			ddlShippingCountry.Items.AddRange(shippingAvailableCountry.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
			// 州
			ddlShippingAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());
			// 配送方法
			ddlShippingMethod.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD));
			// 配送会社
			var companyList = this.FixedPurchaseContainer.Shippings[0].ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS ? this.ShopShipping.CompanyListExpress : this.ShopShipping.CompanyListMail;
			companyList = ((this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				? companyList.Where(company => (company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray()
				: companyList.Where(company => (company.DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray());
			var companyItemList = this.DeliveryCompanyList
				.Where(company => companyList.Any(c => company.DeliveryCompanyId == c.DeliveryCompanyId))
				.Select(company => new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));
			ddlDeliveryCompany.Items.AddRange(companyItemList.ToArray());
			this.DeliveryCompany = this.DeliveryCompanyList.First(i => i.DeliveryCompanyId == this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId);
			// 配送時間帯
			ddlShippingTime.Items.AddRange(GetShippingTimeList(this.DeliveryCompany).Cast<ListItem>().ToArray());
			ddlShippingTime.Items.Insert(0, (new ListItem(ReplaceTag("@@DispText.shipping_time_list.none@@"), "")));

			// 配送先情報反映対象
			ValueText.GetValueItemList(Constants.TABLE_FIXEDPURCHASE, "address_update_pattern")
				.Cast<ListItem>().ToList().ForEach(li => cblUpdatePattern.Items.Add(li));

			var userShippings = new UserShippingService().GetAllOrderByShippingNoDesc(this.FixedPurchaseContainer.UserId).ToArray();
			BindShippingList(userShippings);
		}
		//	配送パターン
		else if (this.RequestActionKbn == ACTION_KBN_PATTERN)
		{
			// 配送パターン・表示制御
			dtMonthlyDate.Visible = ddMonthlyDate.Visible = this.ShopShipping.IsValidFixedPurchaseKbn1Flg;
			dtWeekAndDay.Visible = ddWeekAndDay.Visible = this.ShopShipping.IsValidFixedPurchaseKbn2Flg;
			dtIntervalDays.Visible = ddIntervalDays.Visible = this.ShopShipping.IsValidFixedPurchaseKbn3Flg;
			dtEveryNWeek.Visible = ddEveryNWeek.Visible = this.ShopShipping.IsValidFixedPurchaseKbn4Flg;
			// 第X週ドロップダウン作成
			this.ddlWeekOfMonth.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST));
			// Y曜日ドロップダウン作成
			this.ddlDayOfWeek.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST));
			// X日ドロップダウン作成
			if (string.IsNullOrEmpty(this.ShopShipping.FixedPurchaseKbn1Setting2))
			{
				ddlMonthlyDate.Items.AddRange(ValueText.GetValueItemArray(
					Constants.TABLE_SHOPSHIPPING,
					Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST));
			}
			// 配送スケジュール指定フラグ（デフォルト未チェック）
			this.cbScheduleAutoComputeFlg.Checked = false;
			// 次々回配送日自動計算フラグ（デフォルト未チェック）
			cbNextNextShippingDateAutoComputeFlg.Checked = true;
		}
		//	商品情報
		else if (this.RequestActionKbn == ACTION_KBN_ITEM)
		{
			// 処理なし
		}
		else if (this.RequestActionKbn == ACTION_KBN_INVOICE)
		{
			ddlUniformInvoice.Items.AddRange(
				ValueText.GetValueItemArray(
				Constants.TABLE_TWORDERINVOICE,
				Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE));

			ddlCarryType.Items.AddRange(
				ValueText.GetValueItemArray(
				Constants.TABLE_TWORDERINVOICE,
				Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE));

			if (this.IsMobile)
			{
				tbCarryTypeOption1.Text = (
					((this.IsPersonal)
					&& (this.TwOrderInvoice != null))
						? this.TwOrderInvoice.TwCarryTypeOption
						: string.Empty);
			}

			if (this.IsCertificate)
			{
				tbCarryTypeOption2.Text = (
					((this.IsPersonal)
					&& (this.TwOrderInvoice != null))
						? this.TwOrderInvoice.TwCarryTypeOption
						: string.Empty);
			}

			tbCompanyOption1.Text = (
				((this.IsCompany)
				&& (this.TwOrderInvoice != null))
					? this.TwOrderInvoice.TwUniformInvoiceOption1
					: string.Empty);

			tbCompanyOption2.Text = (
				((this.IsCompany)
				&& (this.TwOrderInvoice != null))
					? this.TwOrderInvoice.TwUniformInvoiceOption2
					: string.Empty);

			tbDonateOption1.Text = (
				((this.IsDonate)
				&& (this.TwOrderInvoice != null))
					? this.TwOrderInvoice.TwUniformInvoiceOption1
					: string.Empty);
		}

		// ポップアップ表示制御（タイトルを非表示へ）
		trTitleFixedpurchaseTop.Visible = trTitleFixedpurchaseMiddle.Visible = trTitleFixedpurchaseBottom.Visible = (this.IsPopUp == false);
	}

	/// <summary>
	/// 画面に値をセット
	/// </summary>
	private void SetValues()
	{
		var input = (this.IsReinput) ? this.ModifyInfo : new FixedPurchaseInput(this.FixedPurchaseContainer);

		// 決済情報
		if (this.RequestActionKbn == ACTION_KBN_PAYMENT)
		{
			ddlOrderPaymentKbn.SelectedValue = input.OrderPaymentKbn;
			// 支払方法が「クレジットカード」？
			if (input.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				// カード選択が「現在利用しているクレジットカードを利用する」？
				if (input.IsSelectCreditCardUseNow)
				{
					if (this.FixedPurchaseContainer.CreditBranchNo.HasValue)
					{
						rbUseNowCreditCard.Checked = true;
						var userCreditCard = UserCreditCard.Get(input.UserId, this.FixedPurchaseContainer.CreditBranchNo.Value);
						lUseNowCreditCardCopmanyName.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName, userCreditCard.CompanyCode));
						lUseNowCreditCardLastFourDigit.Text = WebSanitizer.HtmlEncode(userCreditCard.LastFourDigit);
						lUseNowCreditCardExpirationMonth.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpirationMonth);
						lUseNowCreditCardExpirationYear.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpirationYear);
						lUseNowCreditCardAuthorName.Text = WebSanitizer.HtmlEncode(userCreditCard.AuthorName);
						ddlUseNowCreditCardInstallments.SelectedValue = this.FixedPurchaseContainer.CardInstallmentsCode;
					}
					else
					{
						// カード枝番が保存されていない場合「新しいクレジットカードを利用する」をデフォルト値とする
						divUseNowCreditCard.Visible = false;
						rbUseNowCreditCard.Checked = false;
						rbNewCreditCard.Checked = true;
					}
				}
				// カード選択が「新しいクレジットカードを利用する」？
				else if (input.IsSelectCreditCardNew)
				{
					rbNewCreditCard.Checked = true;
					var inputCreditCard = input.CreditCardInput;
					ddlRegisteredCreditCards.SelectedValue = CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;
					tbCreditCardNo1.Text = this.IsUserPayTg
						? inputCreditCard.CardNo1
						: inputCreditCard.CardNo1.Contains(Constants.CHAR_MASKING_FOR_TOKEN) ? "" : inputCreditCard.CardNo1;
					ddlCreditExpireMonth.SelectedValue = inputCreditCard.ExpireMonth;
					ddlCreditExpireYear.SelectedValue = inputCreditCard.ExpireYear;
					tbCreditAuthorName.Text = inputCreditCard.AuthorName;
					if (OrderCommon.CreditSecurityCodeEnable)
					{
						tbCreditSecurityCode.Text = inputCreditCard.SecurityCode.Replace(Constants.CHAR_MASKING_FOR_TOKEN, "");
					}
					cbRegistCreditCard.Checked = inputCreditCard.DoRegister;
					if (inputCreditCard.RegisterCardName != Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME)
					{
						tbUserCreditCardName.Text = inputCreditCard.RegisterCardName;
					}

					if (this.IsUserPayTg)
					{
						if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
						{
							var payTgResponse = new Hashtable
							{
								{ VeriTransConst.PAYTG_CARD_EXPIRE_MONTH, inputCreditCard.ExpireMonth },
								{ VeriTransConst.PAYTG_CARD_EXPIRE_YEAR, inputCreditCard.ExpireYear },
								{ VeriTransConst.PAYTG_CARD_NUMBER, inputCreditCard.CardNo1 }
							};
							this.PayTgResponse = payTgResponse;
						}
						else
						{
							var payTgResponse = new Hashtable
							{
								{ PayTgConstants.PAYTG_CARD_EXPIRE_MONTH, inputCreditCard.ExpireMonth },
								{ PayTgConstants.PAYTG_CARD_EXPIRE_YEAR, inputCreditCard.ExpireYear },
								{ PayTgConstants.PAYTG_CARD_NUMBER, inputCreditCard.CardNo1 }
							};
							this.PayTgResponse = payTgResponse;
						}
						DisplayCreditInputForm();
					}
				}
				// カード選択が「登録済みのクレジットカードを利用する」？
				else if (input.IsSelectCreditCardRegistered)
				{
					rbRegisteredCreditCard.Checked = true;
					ddlRegisteredCreditCards.SelectedValue = input.CreditBranchNo;
				}
			}
			// 支払方法が「クレジットカード」以外？
			else
			{
				// 「新しいクレジットカードを利用する」をデフォルト値とする
				rbNewCreditCard.Checked = true;
			}

			if (OrderCommon.CreditInstallmentsSelectable)
			{
				ddlUseNowCreditCardInstallments.SelectedValue = input.CardInstallmentsCode;
				dllCreditInstallments.SelectedValue = input.CardInstallmentsCode;
				dllCreditInstallments2.SelectedValue = input.CardInstallmentsCode;
			}
		}
		// 配送先情報
		else if (this.RequestActionKbn == ACTION_KBN_SHIPPING)
		{
			// Bind owner address data
			this.User = new UserService().Get(this.FixedPurchaseContainer.UserId);

			// 定期配送先情報
			var shipping = input.Shippings[0];
			tbShippingName1.Text = shipping.ShippingName1;
			tbShippingName2.Text = shipping.ShippingName2;
			tbShippingNameKana1.Text = shipping.ShippingNameKana1;
			tbShippingNameKana2.Text = shipping.ShippingNameKana2;
			if ((shipping.ShippingZip != "" && (shipping.ShippingZip.Contains("-"))))
			{
				var shippingZip = StringUtility.ToEmpty(shipping.ShippingZip + "-").Split('-');
				tbShippingZip1.Text = shippingZip[0];
				tbShippingZip2.Text = shippingZip[1];
			}
			foreach (ListItem li in ddlShippingAddr1.Items)
			{
				li.Selected = (li.Value == shipping.ShippingAddr1);
			}
			tbShippingAddr2.Text = shipping.ShippingAddr2;
			tbShippingAddr3.Text = shipping.ShippingAddr3;
			tbShippingAddr4.Text = shipping.ShippingAddr4;
			tbShippingCompanyName.Text = shipping.ShippingCompanyName;
			tbShippingCompanyPostName.Text = shipping.ShippingCompanyPostName;

			if (shipping.ShippingTel1.Contains('-'))
			{
				var shippingTel = StringUtility.ToEmpty(shipping.ShippingTel1 + "--").Split('-');
				tbShippingTel1.Text = shippingTel[0];
				tbShippingTel2.Text = shippingTel[1];
				tbShippingTel3.Text = shippingTel[2];
			}

			foreach (ListItem li in ddlShippingMethod.Items)
			{
				li.Selected = (li.Value == shipping.ShippingMethod);

				if (li.Selected)
				{
					ddlShippingMethod_SelectedIndexChanged(null, null);
				}
			}
			foreach (ListItem li in ddlDeliveryCompany.Items)
			{
				li.Selected = (li.Value == shipping.DeliveryCompanyId);
			}
			foreach (ListItem li in ddlShippingTime.Items)
			{
				li.Selected = (li.Value == shipping.ShippingTime);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				ddlShippingCountry.SelectedValue = shipping.ShippingCountryIsoCode;

				if (this.IsShippingAddrJp == false)
				{
					tbShippingZipGlobal.Text = shipping.ShippingZip;
					tbShippingTel1Global.Text = shipping.ShippingTel1;

					if (this.IsShippingAddrUs)
					{
						ddlShippingAddr5.SelectedValue = shipping.ShippingAddr5;
					}
					else
					{
						tbShippingAddr5.Text = shipping.ShippingAddr5;
					}
				}
			}

			// 定期台帳編集確認画面から戻ってきた時に、前に編集画面で選択していた値を設定する
			if ((this.AddressUpdatePattern != null) && (this.AddressUpdatePattern.Length > 0))
			{
				SetSearchCheckBoxValue(cblUpdatePattern, this.AddressUpdatePattern);
			}

			ddlUserShipping.SelectedValue = ((shipping.UserShippingKbn != null)
				? shipping.UserShippingKbn
				: ((shipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
					? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE
					: CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW));
			ddlUserShippingKbn_SelectedIndexChanged(null, null);
		}
		// 配送パターン
		else if (this.RequestActionKbn == ACTION_KBN_PATTERN)
		{
			// Xか月ごとリストボックス作成
			var selectedValue = (((input.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
				|| (input.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY))
				? input.FixedPurchaseSetting1.Split(',')[0]
				: "");
			var fixedPurchaseKbn1Setting2 = (((input.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
				|| (input.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY))
				? input.FixedPurchaseSetting1.Split(',')[1]
				: "");
			ddlMonth.Items.AddRange(
				OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
					this.ShopShipping.FixedPurchaseKbn1Setting,
					selectedValue));
			ddlIntervalMonths.Items.AddRange(
				OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
					this.ShopShipping.FixedPurchaseKbn1Setting,
					selectedValue));

			// X日間隔
			var interval = ((input.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS) ? input.FixedPurchaseSetting1 : "");
			ddlIntervalDays.Items.AddRange(
				OrderCommon.GetKbn3FixedPurchaseIntervalListItems(
					this.ShopShipping.FixedPurchaseKbn3Setting.Replace("(", "").Replace(")", ""),
					interval));

			if (string.IsNullOrEmpty(this.ShopShipping.FixedPurchaseKbn1Setting2) == false)
			{
				this.ddlMonthlyDate.Items.AddRange(
					OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
						this.ShopShipping.FixedPurchaseKbn1Setting2.Replace("(", "").Replace(")", ""),
						fixedPurchaseKbn1Setting2,
						true));
			}

			// X週・X曜日
			ddlFixedPurchaseEveryNWeek_Week.Items.Clear();
			ddlFixedPurchaseEveryNWeek_Week.Items.AddRange(
				OrderCommon.GetKbn4Setting1FixedPurchaseIntervalListItems(
					this.ShopShipping.FixedPurchaseKbn4Setting1
						.Replace("(", string.Empty).Replace(")", string.Empty),
					string.Empty));

			ddlFixedPurchaseEveryNWeek_DayOfWeek.Items.Clear();
			ddlFixedPurchaseEveryNWeek_DayOfWeek.Items.AddRange(
				OrderCommon.GetKbn4Setting2FixedPurchaseIntervalListItems(
					this.ShopShipping.FixedPurchaseKbn4Setting2,
					string.Empty));

			// 配送パターン
			switch (input.FixedPurchaseKbn)
			{
				// Xか月ごとY日
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					rbFixedPurchaseDays.Checked = true;
					var monthAndDays = input.FixedPurchaseSetting1.Split(',');
					ddlMonth.SelectedValue = monthAndDays[0];
					ddlMonthlyDate.SelectedValue = monthAndDays[1];
					break;

				// Xヶ月ごと第YのZ曜日
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					var strSplitedFixedPurchaseSetting1 = StringUtility.ToEmpty(input.FixedPurchaseSetting1).Split(',');
					rbFixedPurchaseWeekAndDay.Checked = true;
					if (strSplitedFixedPurchaseSetting1.Length > 0)
					{
						ddlIntervalMonths.SelectedValue = strSplitedFixedPurchaseSetting1[0];
						ddlWeekOfMonth.SelectedValue = strSplitedFixedPurchaseSetting1[1];
						ddlDayOfWeek.SelectedValue = strSplitedFixedPurchaseSetting1[2];
					}
					break;

				// X日 間隔
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
					rbFixedPurchaseIntervalDays.Checked = true;
					ddlIntervalDays.SelectedValue = input.FixedPurchaseSetting1;
					break;

				// X週 Y曜日
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
					rbFixedPurchaseWeekAndDayOfWeek.Checked = true;
					var weekAndDayOfWeek = input.FixedPurchaseSetting1.Split(',');
					ddlFixedPurchaseEveryNWeek_Week.SelectedValue = weekAndDayOfWeek[0];
					ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue = weekAndDayOfWeek[1];
					break;
			}

			// 配送スケジュール
			var nextShippingDate = DateTime.Parse(input.NextShippingDate);
			ucNextShippingDate.Year = nextShippingDate.Year.ToString();
			ucNextShippingDate.Month = nextShippingDate.Month.ToString("00");
			ucNextShippingDate.Day = nextShippingDate.Day.ToString("00");
			ucNextShippingDate.SelectedDate = nextShippingDate;
			ucNextShippingDate.YearList = DateTimeUtility.GetYearListItem(DateTime.Now.Year, DateTime.Now.Year + 10);
			ucNextShippingDate.DataBind();
			var nextNextShippingDate = DateTime.Parse(input.NextNextShippingDate);
			ucNextNextShippingDate.Year = nextNextShippingDate.Year.ToString();
			ucNextNextShippingDate.Month = nextNextShippingDate.Month.ToString("00");
			ucNextNextShippingDate.Day = nextNextShippingDate.Day.ToString("00");
			ucNextNextShippingDate.SelectedDate = nextNextShippingDate;
			ucNextNextShippingDate.YearList = DateTimeUtility.GetYearListItem(DateTime.Now.Year, DateTime.Now.Year + 10);
			ucNextNextShippingDate.DataBind();
		}
		// 商品情報
		else if (this.RequestActionKbn == ACTION_KBN_ITEM)
		{
			if (this.IsSubscriptionBox)
			{
				var selectedSubscriptionBox = DataCacheControllerFacade
					.GetSubscriptionBoxCacheController()
					.Get(this.FixedPurchaseContainer.SubscriptionBoxCourseId);

				foreach (var item in input.Shippings[0].Items.Select((value, index) => new { value, index }))
				{
					// 頒布会キャンペーン期間かどうか
					var selectedSubscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
						x => (x.ProductId == item.value.ProductId) && (x.VariationId == item.value.VariationId));

					// 次回配送日が頒布会キャンペーン期間かどうか
					if (OrderCommon.IsSubscriptionBoxCampaignPeriodByNextShippingDate(selectedSubscriptionBoxItem, DateTime.Parse(input.NextShippingDate)))
					{
						input.Shippings[0].Items[item.index].FixedPurchasePrice = selectedSubscriptionBoxItem.CampaignPrice.ToPriceDecimal();
					}
				}
			}

			// Check if there is one item: Don't allow to delete
			this.CanDelete = (input.Shippings[0].Items.Length > 1);
			rItemList.DataSource = input.Shippings[0].Items;
			rItemList.DataBind();
		}
		// Invoice
		else if (this.RequestActionKbn == ACTION_KBN_INVOICE)
		{
			ddlUniformInvoice.SelectedValue = this.TwFixedPurchaseInvoice.TwUniformInvoice;

			// Set Visible For Uniform Type
			SetVisibleForUniformOption(ddlUniformInvoice.SelectedValue);

			ddlCarryType.SelectedValue = this.TwFixedPurchaseInvoice.TwCarryType;

			// Set Visible For Carry Type Option
			SetVisibleForCarryTypeOption(ddlCarryType.SelectedItem.Value);

			tbCompanyOption1.Text = (this.IsCompany) ? this.TwFixedPurchaseInvoice.TwUniformInvoiceOption1 : string.Empty;
			tbCompanyOption2.Text = (this.IsCompany) ? this.TwFixedPurchaseInvoice.TwUniformInvoiceOption2 : string.Empty;

			tbCarryTypeOption1.Text = this.IsMobile ? this.TwFixedPurchaseInvoice.TwCarryTypeOption : string.Empty;
			tbCarryTypeOption2.Text = this.IsCertificate ? this.TwFixedPurchaseInvoice.TwCarryTypeOption : string.Empty;

			tbDonateOption1.Text = (this.IsDonate) ? this.TwFixedPurchaseInvoice.TwUniformInvoiceOption1 : string.Empty;
		}

		// 会員ランクIDセット
		this.MemberRankId = this.FixedPurchaseContainer.MemberRankId;
	}

	/// <summary>
	/// 商品情報取得
	/// </summary>
	/// <returns>商品情報入力内容</returns>
	private IEnumerable<FixedPurchaseItemInput> GetItems()
	{
		var itemNo = 1;
		foreach (RepeaterItem ri in rItemList.Items)
		{
			// 入力情報をセット
			var productId = ((TextBox)ri.FindControl("tbProductId")).Text.Trim();
			var variationId = productId + ((TextBox)ri.FindControl("tbVariationId")).Text.Trim();
			var itemQuantity = ((TextBox)ri.FindControl("tbItemQuantity")).Text;
			var itemOrderCount = ((TextBox)ri.FindControl("tbItemOrderCount")).Text;
			var itemShippedCount = ((TextBox)ri.FindControl("tbItemShippedCount")).Text;
			var isInputFormText = ((RadioButton)ri.FindControl("rbProductOptionValueInputFormText")).Checked;
			var productOptionSettingList = (isInputFormText == false)
				? CreateProductOptionSettingList(ri)
				: null;
			var productOptionTexts = isInputFormText
				? ((TextBox)ri.FindControl("tbProductOptionTexts")).Text
				: productOptionSettingList.GetJsonStringFromSelectValues();

			var item = new FixedPurchaseItemInput()
			{
				FixedPurchaseId = this.FixedPurchaseContainer.FixedPurchaseId,
				FixedPurchaseItemNo = itemNo.ToString(),
				FixedPurchaseShippingNo = this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo.ToString(),
				ShopId = this.FixedPurchaseContainer.ShopId,
				ProductId = productId,
				VariationId = variationId,
				ItemQuantity = itemQuantity,
				ItemQuantitySingle = itemQuantity,
				ProductOptionTexts = productOptionTexts,
				Price = 0,
				ItemOrderCount = itemOrderCount,
				ItemShippedCount = itemShippedCount,
				ProductOptionSettingList = productOptionSettingList
			};

			// 商品情報が存在する?
			var product = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, this.MemberRankId);
			if (product.Count != 0)
			{
				// 商品情報をセット
				item.SupplierId = (string)product[0][Constants.FIELD_PRODUCT_SUPPLIER_ID];
				item.Name = (string)product[0][Constants.FIELD_PRODUCT_NAME];
				item.VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
				item.VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
				item.VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
				item.Price = (decimal)product[0][Constants.FIELD_PRODUCTVARIATION_PRICE];
				item.SpecialPrice = product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] : null;
				item.MemberRankPrice = product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] : null;
				item.FixedPurchasePrice = product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] : null;
				item.ShippingType = (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
				item.FixedPurchaseFlg = (string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
				item.ProductLimitedPaymentIds = (string)product[0][Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS];
			}

			// 頒布会注文かどうか
			if (this.IsSubscriptionBox)
			{
				var selectedSubscriptionBox = DataCacheControllerFacade
					.GetSubscriptionBoxCacheController()
					.Get(this.FixedPurchaseContainer.SubscriptionBoxCourseId);

				// 頒布会キャンペーン期間かどうか
				var selectedSubscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
					x => (x.ProductId == productId) && (x.VariationId == variationId));

				// 次回配送日が頒布会キャンペーン期間かどうか
				if (OrderCommon.IsSubscriptionBoxCampaignPeriodByNextShippingDate(selectedSubscriptionBoxItem, (DateTime)this.FixedPurchaseContainer.NextShippingDate))
				{
					item.FixedPurchasePrice = selectedSubscriptionBoxItem.CampaignPrice.ToPriceDecimal();
				}
			}

			itemNo++;

			yield return item;
		}
	}

	/// <summary>
	/// 配送不可エリアエラーがある場合、エラーメッセージを表示する
	/// </summary>
	private bool CheckUnavailableShippingAreaForFixedPurchase()
	{
		var unavailableShippingZip = GetUnavailableShippingZip(this.ShopShipping.ShippingId, this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId);
		var shippingZip = this.FixedPurchaseContainer.Shippings[0].HyphenlessShippingZip;
		var unavailableShipping = OrderCommon.CheckUnavailableShippingArea(unavailableShippingZip, shippingZip);
		return unavailableShipping;
	}

	/// <summary>
	/// 配送サービスの配送不可エリア（郵便番号）を取得
	/// </summary>
	/// <param name="shippingId">配送種別ID</param>
	/// <param name="delivaryCompanyId">配送会社ID</param>
	private static string GetUnavailableShippingZip(string shippingId, string delivaryCompanyId)
	{
		var shopShipping = new ShopShippingService();
		var unavailableShippingZip = shopShipping.GetUnavailableShippingZipFromShippingDelivery(shippingId, delivaryCompanyId);
		return unavailableShippingZip;
	}

	/// <summary>
	/// 配送日正当性チェック
	/// </summary>
	/// <param name="nextShippingDateString">次回配送日</param>
	/// <param name="nextNextShippingDateString">次々回配送日</param>
	/// <returns>エラーメッセージ</returns>
	private string ValidateShippingDate(string nextShippingDateString, string nextNextShippingDateString)
	{
		// 配送日指定をしない場合はそのまま空文字を返す
		if (cbScheduleAutoComputeFlg.Checked) return "";

		var returnMessages = new List<string>();

		// 次回配送日付正当性チェック
		DateTime nextShippingDate;
		if (DateTime.TryParse(nextShippingDateString, out nextShippingDate) == false)
		{
			returnMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_NEXT_SHIPPING_DATE_TYPE_ERROR));
		}

		// 次々回配送日自動計算フラグがオンの場合、ここまでの結果を返す
		if (cbNextNextShippingDateAutoComputeFlg.Checked) return string.Concat(returnMessages);

		// 次々回配送日付正当性チェック
		DateTime nextNextShippingDate;
		if (DateTime.TryParse(nextNextShippingDateString, out nextNextShippingDate) == false)
		{
			returnMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_NEXT_NEXT_SHIPPING_DATE_TYPE_ERROR));
		}

		// 次々回配送日が次回配送日より後かどうか？
		if (returnMessages.Count == 0)
		{
			if (nextShippingDate >= nextNextShippingDate)
			{
				returnMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_SHIPPING_DATE_RANGE_ERROR));
			}
		}

		return string.Concat(returnMessages);
	}

	/// <summary>
	/// クレジットフォームをリフレッシュ
	/// </summary>
	private void RefreshCreditForm()
	{
		divCreditCardNoToken.Visible = (HasCreditToken() == false);
		divCreditCardForTokenAcquired.Visible = HasCreditToken();

		trUserCreditCardName.Visible = cbRegistCreditCard.Checked;
	}

	/// <summary>
	/// （トークン決済向け）カード情報編集リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForToken_Click(object sender, EventArgs e)
	{
		// トークンなどクレジットカード情報削除
		tbCreditCardNo1.Text = "";
		ddlCreditExpireMonth.SelectedIndex = 0;
		ddlCreditExpireYear.SelectedValue = (DateTime.Now.Year - 2000).ToString("00");
		tbCreditAuthorName.Text = "";
		hfCreditToken.Value = "";
	}

	/// <summary>
	/// 月間隔日付指定選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbFixedPurchaseDays_CheckedChanged(object sender, EventArgs e)
	{
		ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 週・曜日指定選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbFixedPurchaseWeekAndDay_CheckedChanged(object sender, EventArgs e)
	{
		ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送日間隔指定選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbFixedPurchaseIntervalDays_CheckedChanged(object sender, EventArgs e)
	{
		ddlIntervalDays_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 週間隔・曜日指定選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbFixedPurchaseEveryNWeek_CheckedChanged(object sender, EventArgs e)
	{
		ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送間隔月ドロップダウン値変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		divAlertMessage.Visible = false;

		if (((rbFixedPurchaseDays.Checked == false)
				&& (rbFixedPurchaseWeekAndDay.Checked == false))
			|| (string.IsNullOrEmpty(ddlMonth.SelectedValue)
				&& string.IsNullOrEmpty(ddlIntervalMonths.SelectedValue))) return;

		// 利用不可配送間隔月情報取得
		var limitedFixedpurchaseValues = new FixedPurchaseInput(this.FixedPurchaseContainer).Shippings[0].Items
			.Where(item => string.IsNullOrEmpty(item.LimitedFixedPurchaseKbn1Setting) == false)
			.Select(item => new Hashtable
				{
					{Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING, item.LimitedFixedPurchaseKbn1Setting}
				})
			.Distinct().ToList();

		// 選択している配送間隔月チェック、アラートメッセージのセット
		CheckIntervalValueAndSetAlertMessage(
			limitedFixedpurchaseValues,
			rbFixedPurchaseDays.Checked ? ddlMonth.SelectedValue : ddlIntervalMonths.SelectedValue,
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING);
	}

	/// <summary>
	/// 配送間隔日ドロップダウン値変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlIntervalDays_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		divAlertMessage.Visible = false;

		if ((rbFixedPurchaseIntervalDays.Checked == false)
			|| string.IsNullOrEmpty(ddlIntervalDays.SelectedValue)) return;

		// 利用不可配送間隔情報取得
		var limitedFixedpurchaseValues = new FixedPurchaseInput(this.FixedPurchaseContainer).Shippings[0].Items
			.Where(item => string.IsNullOrEmpty(item.LimitedFixedPurchaseKbn3Setting) == false)
			.Select(item => new Hashtable
				{
					{Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING, item.LimitedFixedPurchaseKbn3Setting}
				})
			.Distinct().ToList();

		// 選択している配送間隔日チェック、アラートメッセージのセット
		CheckIntervalValueAndSetAlertMessage(
			limitedFixedpurchaseValues,
			ddlIntervalDays.SelectedValue,
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING);
	}

	/// <summary>
	/// 週間隔・曜日指定ドロップダウン値変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		divAlertMessage.Visible = false;

		if ((rbFixedPurchaseWeekAndDayOfWeek.Checked == false)
			|| string.IsNullOrEmpty(ddlFixedPurchaseEveryNWeek_Week.SelectedValue)
			|| string.IsNullOrEmpty(ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue)) return;

		var limitedFixedpurchaseValues = new FixedPurchaseInput(this.FixedPurchaseContainer).Shippings[0].Items
			.Where(item => (string.IsNullOrEmpty(item.LimitedFixedPurchaseKbn4Setting) == false))
			.Select(item => new Hashtable
			{
				{ Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING, item.LimitedFixedPurchaseKbn4Setting },
			})
			.Distinct().ToList();

		// 選択している配送間隔月が規定の値かどうかのチェック
		CheckIntervalValueAndSetAlertMessage(
			limitedFixedpurchaseValues,
			ddlFixedPurchaseEveryNWeek_Week.SelectedValue,
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING);
	}

	/// <summary>
	/// 配送方法自動判定ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetShippingMethod_Click(object sender, EventArgs e)
	{
		ddlShippingMethod.SelectedValue = OrderCommon.GetShippingMethod(this.FixedPurchaseContainer.Shippings[0].Items);
		ddlShippingMethod_SelectedIndexChanged(ddlShippingMethod, e);

		var shopShipping = new ShopShippingService().Get(
			this.LoginOperatorShopId,
			this.FixedPurchaseContainer.Shippings[0].Items[0].ShippingType);
		var companyList = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
			? shopShipping.CompanyListExpress
			: shopShipping.CompanyListMail;
		ddlDeliveryCompany.Items.Clear();

		// メール便配送サービスエスカレーション
		if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED
			&& (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL))
		{
			var deliveryCompanyId = OrderCommon.DeliveryCompanyMailEscalation(this.FixedPurchaseContainer.Shippings[0].Items, companyList);
			if (string.IsNullOrEmpty(deliveryCompanyId))
			{
				ddlShippingMethod.SelectedValue = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
				ddlShippingMethod_SelectedIndexChanged(ddlShippingMethod, e);
				companyList = shopShipping.CompanyListExpress;
			}
			else
			{
				foreach (var item in companyList)
				{
					var company = this.DeliveryCompanyList
						.First(itemCompany => (itemCompany.DeliveryCompanyId == item.DeliveryCompanyId));
					ddlDeliveryCompany.Items.Add(new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));
				}
				ddlDeliveryCompany.SelectedValue = deliveryCompanyId;
				return;
			}
		}

		foreach (var item in companyList)
		{
			var company = this.DeliveryCompanyList
				.First(itemCompany => (itemCompany.DeliveryCompanyId == item.DeliveryCompanyId));
			ddlDeliveryCompany.Items.Add(new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));
		}
		ddlDeliveryCompany.SelectedValue = companyList.First(item => item.IsDefault).DeliveryCompanyId;
	}

	/// <summary>
	/// 選択している配送間隔月・日が規定の値かどうかのチェック
	/// かつアラートメッセージのセット
	/// </summary>
	/// <param name="limitedIntervalValues">利用不可配送間隔情報</param>
	/// <param name="value">選択中の値</param>
	/// <param name="fieldName">チェックする項目名</param>
	private void CheckIntervalValueAndSetAlertMessage(List<Hashtable> limitedIntervalValues, string value, string fieldName)
	{
		// 注意喚起メッセージを設定
		lbFixedPurchasePatternAlertMessages.Text =
			(CheckSpecificIntervalMonthAndDay(limitedIntervalValues, value, fieldName) == false)
			? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_DATE_SPECIFIC_INTERVAL_INVALID)
			: string.Empty;

		divAlertMessage.Visible = (lbFixedPurchasePatternAlertMessages.Text.Length > 0);
	}

	/// <summary>
	/// アドレス帳ドロップダウン作成
	/// </summary>
	/// <param name="userShippings">ユーザー配送先情報</param>
	protected void BindShippingList(UserShippingModel[] userShippings)
	{
		ddlUserShipping.Items.Clear();
		var shopShipping = new ShopShippingService().Get(this.LoginOperatorShopId, this.FixedPurchaseContainer.Shippings[0].Items[0].ShippingType);
		var itemCollection = new ListItemCollection();
		var userShippingAddress = userShippings;
		if ((this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			&& this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE
			&& Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& CheckItemRelateWithServiceConvenienceStore(shopShipping))
		{
			itemCollection.AddRange(userShippingAddress.Where(item => (item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
				.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
			itemCollection.Add(new ListItem(
				ReplaceTag("@@DispText.shipping_addr_kbn_list.convenience_store@@"),
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE));
		}
		else
		{
			itemCollection.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.new@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW));
			itemCollection.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.owner@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER));

			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED && CheckItemRelateWithServiceConvenienceStore(shopShipping))
			{
				itemCollection.AddRange(userShippingAddress.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
				itemCollection.Add(
					new ListItem(
						ReplaceTag("@@DispText.shipping_addr_kbn_list.convenience_store@@"),
						CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE));
			}
			else
			{
				var userShippingNormal = userShippingAddress.Where(item => item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
				itemCollection.AddRange(userShippingNormal.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
			}
		}

		ddlUserShipping.DataSource = itemCollection;
		ddlUserShipping.DataBind();
	}

	/// <summary>
	/// Check Item Relate With Service Convenience Store
	/// </summary>
	/// <param name="shopShipping">Shop Shipping</param>
	/// <returns>True: shipping Kbn is convenience store and relate product </returns>
	protected bool CheckItemRelateWithServiceConvenienceStore(ShopShippingModel shopShipping)
	{
		var serviceShipping = false;
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& (shopShipping != null))
		{
			var deliveryCompanyList = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				? shopShipping.CompanyListExpress
				: shopShipping.CompanyListMail;

			serviceShipping = deliveryCompanyList
				.Select(company => new DeliveryCompanyService().Get(company.DeliveryCompanyId))
				.Any(company => company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID);
		}

		return serviceShipping;
	}

	/// <summary>
	/// クレジットトークン向け  カード情報取得JSスクリプト作成（カート内ではない）
	/// </summary>
	/// <returns>
	/// カード情報取得スクリプト
	/// 文字列を返すのでevalで動的実行すれば連想配列でカード情報がとれます
	/// </returns>
	protected string CreateGetCardInfoJsScriptForCreditToken()
	{
		if (ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return "";

		var script = CreateGetCardInfoJsScriptForCreditTokenInner();
		return script;
	}

	/// <summary>
	/// クレジットトークン取得＆フォームセットJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateGetCreditTokenAndSetToFormJsScript()
	{
		var script = CreateGetCreditTokenAndSetToFormJsScriptInner();
		return script;
	}

	/// <summary>
	/// クレジットトークン向け フォームマスキングJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateMaskFormsForCreditTokenJsScript()
	{
		var maskingScripts = CreateMaskFormsForCreditTokenJsScriptInner();
		return maskingScripts;
	}

	/// <summary>
	/// 注文者の住所国と配送先国から、台湾後払いが利用可能かを判定しエラーメッセージを返す
	/// </summary>
	/// <param name="paymentId">判定する決済種別ID</param>
	/// <param name="shippingCountryIsoCode">配送先国コード</param>
	/// <param name="ownerCountryIsoCode">注文者の住所国コード</param>
	/// <returns>可能:true 不可能:false</returns>
	private string CheckUsedPaymentFixedPurchaseForTriLinkAfterPay(string paymentId, string shippingCountryIsoCode, string ownerCountryIsoCode)
	{
		var errorMessage = (TriLinkAfterPayHelper.CheckUsedPaymentForTriLinkAfterPay(paymentId, shippingCountryIsoCode, ownerCountryIsoCode))
			? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_USBABLE_COUNTRY_ERROR).Replace("@@ 1 @@", "Taiwan") : string.Empty;
		return errorMessage;
	}

	/// <summary>
	/// Get FixedPurchase Input
	/// </summary>
	/// <returns>twFixedPurchaseInvoice</returns>
	private TwFixedPurchaseInvoiceInput GetTwFxiedPurchaseInvoiceInput()
	{
		if (ddlUniformInvoiceOrCarryTypeOption.Visible
			&& (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue) == false))
		{
			var userInvoices = new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.FixedPurchaseContainer.UserId);
			var userInvoice = userInvoices.FirstOrDefault(item => item.TwInvoiceNo == int.Parse(ddlUniformInvoiceOrCarryTypeOption.SelectedValue));
			if (userInvoice != null)
			{
				return new TwFixedPurchaseInvoiceInput
				{
					FixedPurchaseId = this.RequestFixedPurchaseId,
					FixedPurchaseShippingNo = "1",
					TwUniformInvoiceOption2 = userInvoice.TwUniformInvoiceOption2,
					TwUniformInvoiceOption1 = userInvoice.TwUniformInvoiceOption1,
					TwCarryTypeOption = userInvoice.TwCarryTypeOption,
					TwCarryType = userInvoice.TwCarryType,
					TwUniformInvoice = userInvoice.TwUniformInvoice
				};
			}
		}

		var twFixedPurchaseInvoice = new TwFixedPurchaseInvoiceInput
		{
			FixedPurchaseId = this.RequestFixedPurchaseId,
			FixedPurchaseShippingNo = "1",
			TwUniformInvoiceOption2 = (this.IsCompany) ? tbCompanyOption2.Text : string.Empty,
			TwUniformInvoiceOption1 = (this.IsCompany) ? tbCompanyOption1.Text : (this.IsDonate) ? tbDonateOption1.Text : string.Empty,
			TwCarryTypeOption = (this.IsMobile)
				? tbCarryTypeOption1.Text
				: this.IsCertificate
					? tbCarryTypeOption2.Text
					: string.Empty,
			TwCarryType = (this.IsPersonal) ? ddlCarryType.SelectedValue : string.Empty,
			TwUniformInvoice = ddlUniformInvoice.SelectedValue
		};

		return twFixedPurchaseInvoice;
	}

	/// <summary>
	/// Get Uniform Invoice Or Carry Type Option
	/// </summary>
	/// <param name="invoiceCarryType">Uniform Invoice Type</param>
	/// <param name="invoiceCarryType">Invoice Carry Type</param>
	/// <returns>List Item</returns>
	protected ListItemCollection GetUniformInvoiceOrCarryTypeOption(string uniformInvoiceType, string invoiceCarryType)
	{
		var listItem = new ListItemCollection
		{
			new ListItem(ReplaceTag("@@DispText.invoice_carry_type_option.new@@"), string.Empty),
		};

		var userInvoice = (string.IsNullOrEmpty(this.FixedPurchaseContainer.UserId)
			? null
			: new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.FixedPurchaseContainer.UserId));

		if (userInvoice != null)
		{
			if (uniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
			{
				if (uniformInvoiceType != Constants.FLG_ORDER_TW_CARRY_TYPE_NONE)
				{
					listItem.AddRange(userInvoice
						.Where(item => (item.TwUniformInvoice == uniformInvoiceType) && (item.TwCarryType == invoiceCarryType))
						.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
				}
			}
			else
			{
				listItem.AddRange(userInvoice
					.Where(item => (item.TwUniformInvoice == uniformInvoiceType))
					.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
			}
		}

		return listItem;
	}

	/// <summary>
	/// Set Visible For Uniform
	/// </summary>
	protected void SetVisibleForUniformOption(string uniformType)
	{
		var isPersonal = false;
		var isCompany = false;
		var isDonate = false;

		if (string.IsNullOrEmpty(uniformType) == false)
		{
			OrderCommon.CheckUniformType(
				uniformType,
				ref isPersonal,
				ref isDonate,
				ref isCompany);
		}

		this.IsCompany = isCompany;
		this.IsDonate = isDonate;
		this.IsPersonal = isPersonal;
	}

	/// <summary>
	/// Set Visible For Carry Type Option
	/// </summary>
	public void SetVisibleForCarryTypeOption(string carryType)
	{
		// Check Carry Type
		var isMobile = false;
		var isCertificate = false;
		var isNoCarryType = false;

		OrderCommon.CheckCarryType(
			carryType,
			ref isMobile,
			ref isCertificate,
			ref isNoCarryType);

		this.IsMobile = isMobile;
		this.IsCertificate = isCertificate;
		this.IsNoCarryType = isNoCarryType;

		tbCarryTypeOption1.Visible = this.IsMobile;
		tbCarryTypeOption2.Visible = this.IsCertificate;
	}

	/// <summary>
	/// Select Change For DropDown Carry Type
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCarryType_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch (ddlCarryType.SelectedValue)
		{
			case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
			case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
				ddlUniformInvoiceOrCarryTypeOption.Visible = true;
				ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(ddlUniformInvoice.SelectedValue, ddlCarryType.SelectedValue);
				ddlUniformInvoiceOrCarryTypeOption.DataBind();
				break;

			default:
				ddlUniformInvoiceOrCarryTypeOption.Visible = false;
				break;
		}
	}

	/// <summary>
	/// Select Change For DropDown Carry Type Or Uniform
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUniformInvoiceOrCarryTypeOption_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue) == false)
		{
			var userInvoices = new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.FixedPurchaseContainer.UserId);
			var userInvoice = userInvoices.FirstOrDefault(item => item.TwInvoiceNo == int.Parse(ddlUniformInvoiceOrCarryTypeOption.SelectedValue));
			if (this.IsMobile && this.IsPersonal)
			{
				tbCarryTypeOption1.Enabled = false;
				if (userInvoice != null)
				{
					tbCarryTypeOption1.Text = userInvoice.TwCarryTypeOption;
				}
			}
			else if (this.IsCertificate && this.IsPersonal)
			{
				tbCarryTypeOption2.Enabled = false;
				if (userInvoice != null)
				{
					tbCarryTypeOption2.Text = userInvoice.TwCarryTypeOption;
				}
			}
			else if (this.IsCompany)
			{
				tbCompanyOption1.Visible = false;
				lbCompanyOption1.Visible = true;
				tbCompanyOption2.Visible = false;
				lbCompanyOption2.Visible = true;

				if (userInvoice != null)
				{
					lbCompanyOption1.Text = userInvoice.TwUniformInvoiceOption1;
					lbCompanyOption2.Text = userInvoice.TwUniformInvoiceOption2;
				}
			}
			else if (this.IsDonate)
			{
				tbDonateOption1.Visible = false;
				lbDonateOption1.Visible = true;
				if (userInvoice != null)
				{
					lbDonateOption1.Text = userInvoice.TwUniformInvoiceOption1;
				}
			}
		}
		else
		{
			tbCarryTypeOption1.Enabled = true;
			tbCarryTypeOption2.Enabled = true;

			tbCompanyOption1.Enabled = true;
			tbCompanyOption1.Text = string.Empty;
			tbCompanyOption1.Visible = true;
			tbCompanyOption2.Enabled = true;
			tbCompanyOption2.Text = string.Empty;
			tbCompanyOption2.Visible = true;

			lbCompanyOption1.Text = string.Empty;
			lbCompanyOption2.Text = string.Empty;

			tbDonateOption1.Visible = true;
			lbDonateOption1.Text = string.Empty;
		}
	}

	/// <summary>
	/// ddlUniformInvoice Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUniformInvoice_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetVisibleForUniformOption(ddlUniformInvoice.SelectedValue);

		if (this.IsCompany || this.IsDonate)
		{
			ddlUniformInvoiceOrCarryTypeOption.Visible = true;
		}

		DataBindForUniformCarryType(ddlUniformInvoice.SelectedValue);

		if (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue))
		{
			lbDonateOption1.Visible = false;
			tbDonateOption1.Visible = true;

			lbCompanyOption1.Visible = false;
			lbCompanyOption2.Visible = false;
			tbCompanyOption1.Visible = true;
			tbCompanyOption2.Visible = true;

			tbCarryTypeOption1.Enabled = true;
			tbCarryTypeOption2.Enabled = true;
		}
	}

	/// <summary>
	/// Data Bind For Uniform Carry Type
	/// </summary>
	/// <param name="value">Value</param>
	private void DataBindForUniformCarryType(string value)
	{
		switch (value)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
				ddlUniformInvoiceOrCarryTypeOption.Visible =
					(string.IsNullOrEmpty(ddlCarryType.SelectedValue) == false);
				ddlUniformInvoiceOrCarryTypeOption.DataSource =
					GetUniformInvoiceOrCarryTypeOption(
						value,
						ddlCarryType.SelectedValue);
				ddlUniformInvoiceOrCarryTypeOption.DataBind();
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				ddlUniformInvoiceOrCarryTypeOption.Visible = true;
				ddlUniformInvoiceOrCarryTypeOption.DataSource =
					GetUniformInvoiceOrCarryTypeOption(
						value,
						ddlCarryType.SelectedValue);
				ddlUniformInvoiceOrCarryTypeOption.DataBind();
				break;
		}
	}

	/// <summary>
	/// Open Convenience Store Map Ec Pay Click Event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOpenConvenienceStoreMapEcPay_Click(object sender, EventArgs e)
	{
		var shippingReceivingStoreType = ddlShippingReceivingStoreType.SelectedValue;
		if (string.IsNullOrEmpty(shippingReceivingStoreType)) return;

		var script = ECPayUtility.CreateScriptForOpenConvenienceStoreMap(shippingReceivingStoreType);
		System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenConvenienceStoreMap", script, true);
	}

	/// <summary>
	/// Select Change User Shipping
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserShippingKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			LoadShippingReceivingStoreTypeForDisplay();
		}

		trFixedPurchaseShippingErrorMessagesTitle.Visible
			= trFixedPurchaseShippingErrorMessages.Visible = false;
		var input = (this.ModifyInfo == null)
			? new FixedPurchaseInput(this.FixedPurchaseContainer)
			: this.ModifyInfo;
		var shipping = input.Shippings[0];
		this.UserShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
		trShippingTime1.Visible = true;
		var shippingNo = 0;
		var isShippingConvenience = (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE);
		if (int.TryParse(ddlUserShipping.SelectedValue, out shippingNo))
		{
			var userShipping = new UserShippingService().Get(this.FixedPurchaseContainer.UserId, shippingNo);
			this.UserShippingReceivingStoreFlg = ((userShipping == null)
				? Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF
				: userShipping.ShippingReceivingStoreFlg);
			if (userShipping != null)
			{
				if ((userShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
					&& (string.IsNullOrEmpty(userShipping.ShippingReceivingStoreId) == false))
				{
					if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
					{
						ddlShippingReceivingStoreType.SelectedValue = userShipping.ShippingReceivingStoreType;
						ddlShippingReceivingStoreType.Enabled = false;
						btnOpenConvenienceStoreMapEcPay.Visible = false;
					}

					tbodyConvenienceStore.Visible = true;
					tbodyUserShipping.Visible = false;
					buttonConvenienceStoreMapPopup.Visible = (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false);
					trShippingTime1.Visible = false;
					tbSeletedAddress.Visible = false;
					lCvsShopId.Text = WebSanitizer.HtmlEncode(userShipping.ShippingReceivingStoreId);
					lCvsShopName.Text = WebSanitizer.HtmlEncode(userShipping.ShippingName);
					lCvsShopAddress.Text = WebSanitizer.HtmlEncode(userShipping.ShippingAddr4);
					lCvsShopTel.Text = WebSanitizer.HtmlEncode(userShipping.ShippingTel1);
					hfCvsShopId.Value = userShipping.ShippingReceivingStoreId;
					hfCvsShopName.Value = userShipping.ShippingName;
					hfCvsShopAddress.Value = userShipping.ShippingAddr4;
					hfCvsShopTel.Value = userShipping.ShippingTel1;
					hfCvsShopFlg.Value = userShipping.ShippingReceivingStoreFlg;
					isShippingConvenience = true;
				}
				else
				{
					tbodyConvenienceStore.Visible = false;
					tbodyUserShipping.Visible = false;
					buttonConvenienceStoreMapPopup.Visible = false;
					tbSeletedAddress.Visible = true;
					lbSeletedAddressName.Text = userShipping.ShippingName;
					lbSeletedAddressNameKana.Text = userShipping.ShippingNameKana;
					lbSeletedAddressAddr1.Text = userShipping.ShippingAddr1;
					lbSeletedAddressAddr2.Text = userShipping.ShippingAddr2;
					lbSeletedAddressAddr3.Text = userShipping.ShippingAddr3;
					lbSeletedAddressAddr4.Text = userShipping.ShippingAddr4;
					lbSeletedAddressAddr5.Text = userShipping.ShippingAddr5;
					lbSeletedAddressZip.Text = userShipping.ShippingZip;
					lbSeletedAddressAddrCountryName.Text = userShipping.ShippingCountryName;
					lbSeletedAddressCompanyName.Text = userShipping.ShippingCompanyName;
					lbSeletedAddressCompanyPostName.Text = userShipping.ShippingCompanyPostName;
					lbSeletedAddressTel1.Text = userShipping.ShippingTel1;
					ddlShippingReceivingStoreType.Visible = false;
					btnOpenConvenienceStoreMapEcPay.Visible = false;
				}
			}
		}

		else if (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
		{
			tbodyUserShipping.Visible = false;
			trShippingTime1.Visible = false;
			tbodyConvenienceStore.Visible = true;
			ddlShippingReceivingStoreType.Visible = true;
			btnOpenConvenienceStoreMapEcPay.Visible = Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED;
			buttonConvenienceStoreMapPopup.Visible = (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false);
			this.UserShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
			if (shipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			{
				tbSeletedAddress.Visible = false;
				lCvsShopId.Text = WebSanitizer.HtmlEncode(shipping.ShippingReceivingStoreId);
				lCvsShopName.Text = WebSanitizer.HtmlEncode(shipping.ShippingName);
				lCvsShopAddress.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr4);
				lCvsShopTel.Text = WebSanitizer.HtmlEncode(shipping.ShippingTel1);
				hfCvsShopId.Value = shipping.ShippingReceivingStoreId;
				hfCvsShopName.Value = shipping.ShippingName;
				hfCvsShopAddress.Value = shipping.ShippingAddr4;
				hfCvsShopTel.Value = shipping.ShippingTel1;
				hfCvsShopFlg.Value = shipping.ShippingReceivingStoreFlg;
				if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
				{
					ddlShippingReceivingStoreType.SelectedValue = shipping.ShippingReceivingStoreType;
				}
			}
			else
			{
				tbSeletedAddress.Visible = false;
				lCvsShopId.Text = string.Empty;
				lCvsShopName.Text = string.Empty;
				lCvsShopAddress.Text = string.Empty;
				lCvsShopTel.Text = string.Empty;
				hfCvsShopId.Value = string.Empty;
				hfCvsShopName.Value = string.Empty;
				hfCvsShopAddress.Value = string.Empty;
				hfCvsShopTel.Value = string.Empty;
				hfCvsShopFlg.Value = shipping.ShippingReceivingStoreFlg;
			}
		}

		else if (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
		{
			this.UserShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
			tbodyConvenienceStore.Visible = false;
			tbodyUserShipping.Visible = true;
			buttonConvenienceStoreMapPopup.Visible = false;
			tbSeletedAddress.Visible = false;
			ddlShippingReceivingStoreType.Visible = false;
			btnOpenConvenienceStoreMapEcPay.Visible = false;
			tbShippingName1.Text = shipping.ShippingName1;
			tbShippingName2.Text = shipping.ShippingName2;
			tbShippingNameKana1.Text = shipping.ShippingNameKana1;
			tbShippingNameKana2.Text = shipping.ShippingNameKana2;
			if ((shipping.ShippingZip != "" && (shipping.ShippingZip.Contains("-"))))
			{
				var shippingZip = StringUtility.ToEmpty(shipping.ShippingZip + "-").Split('-');
				tbShippingZip1.Text = shippingZip[0];
				tbShippingZip2.Text = shippingZip[1];
			}
			foreach (ListItem li in ddlShippingAddr1.Items)
			{
				li.Selected = (li.Value == shipping.ShippingAddr1);
			}
			tbShippingAddr2.Text = shipping.ShippingAddr2;
			tbShippingAddr3.Text = shipping.ShippingAddr3;
			tbShippingAddr4.Text = shipping.ShippingAddr4;
			tbShippingCompanyName.Text = shipping.ShippingCompanyName;
			tbShippingCompanyPostName.Text = shipping.ShippingCompanyPostName;

			if (shipping.ShippingTel1.Contains('-'))
			{
				var shippingTel = StringUtility.ToEmpty(shipping.ShippingTel1 + "--").Split('-');
				tbShippingTel1.Text = shippingTel[0];
				tbShippingTel2.Text = shippingTel[1];
				tbShippingTel3.Text = shippingTel[2];
			}
			foreach (ListItem li in ddlDeliveryCompany.Items)
			{
				li.Selected = (li.Value == shipping.DeliveryCompanyId);
			}
			foreach (ListItem li in ddlShippingTime.Items)
			{
				li.Selected = (li.Value == shipping.ShippingTime);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				if (ddlShippingCountry.Items.FindByValue(shipping.ShippingCountryIsoCode) != null)
				{
					ddlShippingCountry.SelectedValue = shipping.ShippingCountryIsoCode;
				}

				if (this.IsShippingAddrJp == false)
				{
					tbShippingZipGlobal.Text = shipping.ShippingZip;
					tbShippingTel1Global.Text = shipping.ShippingTel1;

					if (this.IsShippingAddrUs)
					{
						ddlShippingAddr5.SelectedValue = shipping.ShippingAddr5;
					}
					else
					{
						tbShippingAddr5.Text = shipping.ShippingAddr5;
					}
				}
			}

			// 定期台帳編集確認画面から戻ってきた時に、前に編集画面で選択していた値を設定する
			if ((this.AddressUpdatePattern != null) && (this.AddressUpdatePattern.Length > 0))
			{
				SetSearchCheckBoxValue(cblUpdatePattern, this.AddressUpdatePattern);
			}
		}
		else if (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)
		{
			tbSeletedAddress.Visible = true;
			tbodyConvenienceStore.Visible = false;
			tbodyUserShipping.Visible = false;
			buttonConvenienceStoreMapPopup.Visible = false;
			ddlShippingReceivingStoreType.Visible = false;
			btnOpenConvenienceStoreMapEcPay.Visible = false;

			lbSeletedAddressName.Text = this.User.Name;
			lbSeletedAddressNameKana.Text = this.User.NameKana;
			lbSeletedAddressAddr1.Text = this.User.Addr1;
			lbSeletedAddressAddr2.Text = this.User.Addr2;
			lbSeletedAddressAddr3.Text = this.User.Addr3;
			lbSeletedAddressAddr4.Text = this.User.Addr4;
			lbSeletedAddressAddr5.Text = this.User.Addr5;
			lbSeletedAddressZip.Text = this.User.Zip;
			lbSeletedAddressAddrCountryName.Text = this.User.AddrCountryName;
			lbSeletedAddressCompanyName.Text = this.User.CompanyName;
			lbSeletedAddressCompanyPostName.Text = this.User.CompanyPostName;
			lbSeletedAddressTel1.Text = this.User.Tel1;
		}
		ddlDeliveryCompany.Items.Clear();

		var companyList = ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS ? this.ShopShipping.CompanyListExpress : this.ShopShipping.CompanyListMail;
		companyList = (isShippingConvenience
			? companyList.Where(company => (company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray()
			: companyList.Where(company => (company.DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray());
		foreach (var company in companyList)
		{
			var info = this.DeliveryCompanyList.First(i => i.DeliveryCompanyId == company.DeliveryCompanyId);
			ddlDeliveryCompany.Items.Add(new ListItem(info.DeliveryCompanyName, info.DeliveryCompanyId));
		}

		RefreshDeliveryCompanyForDisplay(isShippingConvenience);
		ddlDeliveryCompany_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// カード情報取得ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetCardInfo_Click(object sender, EventArgs e)
	{
		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
		{
			// 入力チェック
			var orderCreditCardInput = GetOrderCreditCardInputForFixedPurchasePage(this.FixedPurchaseContainer.UserId);
			var errorMessage = orderCreditCardInput.Validate(true, WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_VERITRANS_PAYTG_CARD_UNREGISTERD));
			if (errorMessage.Length != 0)
			{
				trFixedPurchasePaymentErrorMessagesTitle.Visible
					= trFixedPurchasePaymentErrorMessages.Visible = true;
				lbFixedPurchasePaymentErrorMessages.Text = errorMessage;
				return;
			}

			hfPayTgSendId.Value = new UserCreditCardCooperationInfoVeritrans(this.FixedPurchaseContainer.UserId).CooperationId1;
			Session[PayTgConstants.PARAM_CUSTOMERID] = hfPayTgSendId.Value;

			// PayTG連携APIのポストデータ作成
			hfPayTgPostData.Value = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
				? string.Empty
				: new PayTgApiForVeriTrans(hfPayTgSendId.Value).CreatePostData();

			var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
				? CreateRegisterCardVeriTransMockUrl(hfPayTgSendId.Value)
				: Constants.PAYMENT_SETTING_PAYTG_REGISTCREDITURL;

			// PayTG連動でカード登録実行
			System.Web.UI.ScriptManager.RegisterStartupScript(
				this,
				GetType(),
				"execRegistration",
				"execCardRegistration(tbCreditAuthorName'" + apiUrl + "');",
				true);
		}
		else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
		{
			// PayTg WebApiで利用するため決済注文IDを採番
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(this.LoginOperatorShopId);

			// PayTG連携APIのポストデータ作成
			hfPayTgPostData.Value = new PayTgApi(paymentOrderId).CreatePostData();

			var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
				? CreateRegisterCardMockUrl()
				: Constants.PAYMENT_SETTING_PAYTG_BASEURL + PayTgConstants.PspShortName.RAKUTEN + PayTgConstants.DealingTypes.URL_CHECKCARD;

			// PayTG連動でカード登録実行
			ScriptManager.RegisterStartupScript(
			this,
			GetType(),
			"execRegistration",
			string.Format("execCardRegistration('{0}');", apiUrl),
			true);
		}
	}

	/// <summary>
	/// PayTG連携のレスポンス処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnProcessPayTgResponse_Click(object sender, EventArgs e)
	{
		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
		{
			var payTg = new PayTgApiForVeriTrans(hfPayTgSendId.Value);
			payTg.ParseResponse(hfPayTgResponse.Value);

			this.IsSuccessfulCardRegistration = false;

			if (payTg.Result.IsSuccess)
			{
				var cardExpire = payTg.Result.Response.CardExpire.Split('/');
				var cardNumber = payTg.Result.Response.CardNumber;
				var payTgResponse = new Hashtable
				{
					{ VeriTransConst.PAYTG_CARD_EXPIRE_MONTH, cardExpire[0] },
					{ VeriTransConst.PAYTG_CARD_EXPIRE_YEAR, cardExpire[1] },
					{ VeriTransConst.PAYTG_CARD_NUMBER, cardNumber },
					{ VeriTransConst.PAYTG_RESPONSE_ERROR, string.Empty },
				};
				this.PayTgResponse = payTgResponse;

				ddlCreditExpireMonth.SelectedValue = (string)this.PayTgResponse[VeriTransConst.PAYTG_CARD_EXPIRE_MONTH];
				ddlCreditExpireYear.SelectedValue = (string)this.PayTgResponse[VeriTransConst.PAYTG_CARD_EXPIRE_YEAR];
				tbCreditCardNo1.Text = cardNumber;

				trFixedPurchasePaymentErrorMessagesTitle.Visible = trFixedPurchasePaymentErrorMessages.Visible = false;
				lbFixedPurchasePaymentErrorMessages.Text = string.Empty;
				this.IsSuccessfulCardRegistration = true;
			}
			else
			{
				var payTgResponse =
					new Hashtable { { VeriTransConst.PAYTG_RESPONSE_ERROR, payTg.Result.ErrorMessages } };
				this.PayTgResponse = payTgResponse;
				trFixedPurchasePaymentErrorMessagesTitle.Visible = trFixedPurchasePaymentErrorMessages.Visible = true;
				lbFixedPurchasePaymentErrorMessages.Text = payTg.Result.ErrorMessages;
				this.IsSuccessfulCardRegistration = false;
			}
		}
		else
		{
			var payTg = new PayTgApi(hfPayTgSendId.Value);
			payTg.ParseResponse(hfPayTgResponse.Value);

			this.IsSuccessfulCardRegistration = false;

			if (payTg.Result.IsSuccess)
			{
				var cardExpireMonth = payTg.Result.Response.McAcntNo1;
				var cardExpireYear = payTg.Result.Response.Expire;
				var lastFourDigit = payTg.Result.Response.Last4;
				var cardNumber = "XXXXXXXXXXXX" + lastFourDigit;
				var companyCode = payTg.Result.Response.AcqName;
				var token = payTg.Result.Response.Token;
				var vResultCode = payTg.Result.Response.VResultCode;
				var errorMsg = payTg.Result.Response.ErrorMsg;

				var payTgResponse = new Hashtable
				{
					{ PayTgConstants.PAYTG_CARD_EXPIRE_MONTH, cardExpireMonth },
					{ PayTgConstants.PAYTG_CARD_EXPIRE_YEAR, cardExpireYear },
					{ PayTgConstants.PARAM_LAST4, lastFourDigit },
					{ PayTgConstants.PAYTG_CARD_NUMBER, cardNumber },
					{ PayTgConstants.PARAM_ACQNAME, companyCode },
					{ PayTgConstants.PARAM_TOKEN, token },
					{ PayTgConstants.PAYTG_RESPONSE_ERROR, string.Empty },
				};
				Session[PayTgConstants.PARAM_TOKEN] = hfPayTgSendId.Value = payTg.Result.Response.Token;
				Session[PayTgConstants.PARAM_ACQNAME] = companyCode;
				this.PayTgResponse = payTgResponse;

				ddlCreditExpireMonth.SelectedValue = cardExpireMonth;
				ddlCreditExpireYear.SelectedValue = cardExpireYear.Substring(cardExpireYear.Length - 2);
				tbCreditCardNo1.Text = cardNumber;

				trFixedPurchasePaymentErrorMessagesTitle.Visible = trFixedPurchasePaymentErrorMessages.Visible = false;
				lbFixedPurchasePaymentErrorMessages.Text = string.Empty;
				this.IsSuccessfulCardRegistration = true;
			}
			else
			{
				// PCサイト向けに優先したいクレジットカードメッセージ
				var cardErrorMessageForPc = string.Empty;

				this.IsSuccessfulCardRegistration = false;
				var resultCode = payTg.Result.Response.VResultCode;

				if (string.IsNullOrEmpty(resultCode))
				{
					// PayTg端末のエラーの場合はエラーメッセージを統一
					resultCode = PayTgConstants.ERRMSG_PAYTG_UNAVAILABLE;
				}
				var creditError = new CreditErrorMessage();
				creditError.SetCreditErrorMessages(Constants.FILE_XML_RAKUTEN_CREDIT_ERROR_MESSAGE);
				var errorList = creditError.GetValueItemArray();
				cardErrorMessageForPc = (errorList.Any(s => s.Value == resultCode))
					? errorList.First(s => (s.Value == resultCode)).Text
					: string.Empty;

				var payTgResponse = new Hashtable { { PayTgConstants.PAYTG_RESPONSE_ERROR, cardErrorMessageForPc } };
				this.PayTgResponse = payTgResponse;
				trFixedPurchasePaymentErrorMessagesTitle.Visible = trFixedPurchasePaymentErrorMessages.Visible = true;
				lbFixedPurchasePaymentErrorMessages.Text = cardErrorMessageForPc;
			}
		}
		DisplayCreditInputForm();
	}

	/// <summary>
	/// クレジットカード登録フォーム表示
	/// </summary>
	private void DisplayCreditInputForm()
	{
		btnGetCreditCardInfo.Enabled = (this.IsSuccessfulCardRegistration == false);
		ddlCreditExpireMonth.Enabled = (this.IsSuccessfulCardRegistration == false);
		ddlCreditExpireYear.Enabled = (this.IsSuccessfulCardRegistration == false);
		tbCreditCardNo1.Enabled = (this.IsSuccessfulCardRegistration == false);
		trCreditExpire.Visible = this.IsSuccessfulCardRegistration;
		tdCreditNumber.Visible = this.IsSuccessfulCardRegistration;
		tdGetCardInfo.Visible = (this.IsSuccessfulCardRegistration == false);
	}

	/// <summary>
	/// Shipping Receiving Store Type Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingReceivingStoreType_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		var input = this.ModifyInfo ?? new FixedPurchaseInput(this.FixedPurchaseContainer);
		lCvsShopId.Text = string.Empty;
		lCvsShopName.Text = string.Empty;
		lCvsShopAddress.Text = string.Empty;
		lCvsShopTel.Text = string.Empty;
		hfCvsShopId.Value = string.Empty;
		hfCvsShopName.Value = string.Empty;
		hfCvsShopAddress.Value = string.Empty;
		hfCvsShopTel.Value = string.Empty;
		hfCvsShopFlg.Value = input.Shippings.First().ShippingReceivingStoreFlg;
	}

	/// <summary>
	/// Load Shipping Receiving Store Type For Display
	/// </summary>
	protected void LoadShippingReceivingStoreTypeForDisplay()
	{
		ddlShippingReceivingStoreType.DataSource = ValueText.GetValueItemList(
			Constants.TABLE_ORDERSHIPPING,
			Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE);
		ddlShippingReceivingStoreType.DataBind();
	}

	/// <summary>
	/// Refresh Delivery Company For Display
	/// </summary>
	/// <param name="isShippingConvenience">Is Shipping Convenience</param>
	protected void RefreshDeliveryCompanyForDisplay(bool isShippingConvenience)
	{
		ddlDeliveryCompany.Items.Clear();
		var companyList = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
			? this.ShopShipping.CompanyListExpress
			: this.ShopShipping.CompanyListMail;
		companyList = isShippingConvenience
			? companyList.Where(company => (company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray()
			: companyList.Where(company => (company.DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray();

		var deliveryCompanies = new List<ListItem>();
		foreach (var company in companyList)
		{
			var info = this.DeliveryCompanyList.First(item => (item.DeliveryCompanyId == company.DeliveryCompanyId));
			deliveryCompanies.Add(new ListItem(info.DeliveryCompanyName, info.DeliveryCompanyId));
		}
		ddlDeliveryCompany.Items.AddRange(
			deliveryCompanies
				.OrderBy(item => this.DeliveryCompanyList.First(company => (company.DeliveryCompanyId == item.Value)).DisplayOrder)
				.ThenBy(item => item.Value)
				.ToArray());
		foreach (ListItem li in ddlDeliveryCompany.Items)
		{
			li.Selected = (li.Value == this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId);
		}
	}

	/// <summary>
	/// Is Payment At Convenience Store
	/// </summary>
	/// <param name="input">Input</param>
	/// <returns>True: Payment at convenience store</returns>
	protected bool IsPaymentAtConvenienceStore(FixedPurchaseInput input)
	{
		var shippingReceivingStoreType = input.Shippings[0].ShippingReceivingStoreType;
		var result = ((shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT)
			|| (shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT_PAYMENT)
			|| (shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT));
		return result;
	}

	/// <summary>
	/// Check Store Id Valid
	/// </summary>
	/// <param name="storeId">Convenience Store Id</param>
	/// <returns>Store is valid or not</returns>
	[WebMethod]
	public static bool CheckStoreIdValid(string storeId)
	{
		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) return true;

		if (string.IsNullOrEmpty(storeId)) return false;

		return OrderCommon.CheckIdExistsInXmlStoreBatchFixedPurchase(storeId);
	}

	/// <summary>
	/// 利用可能な決済種別か？チェック
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="user">ユーザーモデル</param>
	private void DisplayPaymentUserManagementLevel(string shopId, UserModel user)
	{
		var message = (UserService.IsUser(user.UserKbn))
			? GetPaymentUserManagementLevelNotUseMessage(shopId, user.UserManagementLevelId)
			: "";
		lbPaymentUserManagementLevelMessage.Text = message;
	}

	/// <summary>
	/// 注文者区分選択できない決済種別を文言で表示
	/// </summary>
	/// <param name="shopId">ShopID</param>
	/// <param name="userKbn">注文者区分</param>
	private void DisplayPaymentOrderOwnerKbn(string shopId, string userKbn)
	{
		string message = GetPaymentOrderOwnerKbnNotUseMessage(shopId, userKbn);
		lbPaymentOrderOwnerKbnMessage.Text = message;
	}

	/// <summary>
	/// Linkbutton search address from shipping zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromShippingZipGlobal_Click(object sender, EventArgs e)
	{
		BindingAddressByGlobalZipcode(
			this.ShippingAddrCountryIsoCode,
			StringUtility.ToHankaku(tbShippingZipGlobal.Text.Trim()),
			tbShippingAddr2,
			tbShippingAddr3,
			tbShippingAddr4,
			tbShippingAddr5,
			ddlShippingAddr5);
	}

	/// <summary>
	/// 商品付帯情報入力形式選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbProductOptionValueInputForm_OnCheckedChanged(object sender, EventArgs e)
	{
		var rbProductOptionValueInput = (RadioButton)sender;
		foreach (var ri in rItemList.Items.Cast<RepeaterItem>())
		{
			var rbProductOptionValueInputFormText = (RadioButton)ri.FindControl("rbProductOptionValueInputFormText");
			var rbProductOptionValueInputForm = (RadioButton)ri.FindControl("rbProductOptionValueInputForm");

			if (rbProductOptionValueInputFormText.ClientID.Equals(rbProductOptionValueInput.ClientID)
				|| rbProductOptionValueInputForm.ClientID.Equals(rbProductOptionValueInput.ClientID))
			{
				// 簡単入力フォーム → テキスト入力フォーム
				if (rbProductOptionValueInputFormText.Checked)
				{
					var settings = CreateProductOptionSettingList(ri);
					((TextBox)ri.FindControl("tbProductOptionTexts")).Text = settings.GetDisplayProductOptionSettingSelectValues();

					ri.FindControl("dvProductOptionValueInputFormText").Visible = true;
					ri.FindControl("dvProductOptionValueInputForm").Visible = false;
					return;
				}
				// テキスト入力フォーム → 簡単入力フォーム
				else if (rbProductOptionValueInputForm.Checked)
				{
					var productId = ((TextBox)ri.FindControl("tbProductId")).Text;
					var settingList = new ProductOptionSettingList(this.LoginOperatorShopId, productId);
					var text = ((TextBox)ri.FindControl("tbProductOptionTexts")).Text.Trim();
					settingList.SetDefaultValueFromProductOptionTexts(text);

					var rProductOptionValueSettings = ((Repeater)ri.FindControl("rProductOptionValueSettings"));
					rProductOptionValueSettings.DataSource = settingList;
					rProductOptionValueSettings.DataBind();
					ri.FindControl("dvProductOptionValueInputFormText").Visible = false;
					ri.FindControl("dvProductOptionValueInputForm").Visible = true;
					return;
				}
			}
		}
	}

	/// <summary>
	/// 商品付帯情報作成
	/// </summary>
	/// <param name="riItem">付帯情報選択領域のリピータアイテム</param>
	/// <returns>商品付帯情報設定リスト</returns>
	private ProductOptionSettingList CreateProductOptionSettingList(RepeaterItem riItem)
	{
		var productId = ((TextBox)riItem.FindControl("tbProductId")).Text;
		var productIdForOptionSetting = ((HiddenField)riItem.FindControl("hfProductIdForOptionSetting")).Value;
		var settingList = new ProductOptionSettingList(this.LoginOperatorShopId, productId);

		// 商品が変更されていたら選択無しの設定を渡す
		if (productId != productIdForOptionSetting) return settingList;

		// 選択値セット
		foreach (var settingItem in settingList.Items)
		{
			var index = settingList.Items.IndexOf(settingItem);
			var riTarget = ((Repeater)riItem.FindControl("rProductOptionValueSettings")).Items[index];
			settingItem.SelectedSettingValue = GetSelectedProductOptionValue(riTarget);
		}
		return settingList;
	}

	/// <summary>
	/// 選択されている商品付帯情報値を取得する
	/// </summary>
	/// <param name="riItem">付帯情報選択用リピータ</param>
	/// <returns>選択されている付帯情報</returns>
	private string GetSelectedProductOptionValue(RepeaterItem riItem)
	{
		var rCblProductOptionValueSetting = (Repeater)riItem.FindControl("rCblProductOptionValueSetting");
		var ddlProductOptionValueSetting = (DropDownList)riItem.FindControl("ddlProductOptionValueSetting");
		var tbProductOptionValueSetting = (TextBox)riItem.FindControl("tbProductOptionValueSetting");
		if (rCblProductOptionValueSetting.Visible)
		{
			var lSelectedValues = new List<string>();
			foreach (RepeaterItem riCheckBox in rCblProductOptionValueSetting.Items)
			{
				var cbOption = ((CheckBox)(riCheckBox.FindControl("cbProductOptionValueSetting")));
				if (cbOption.Checked == false) continue;

				lSelectedValues.Add(cbOption.Text);
			}
			return string.Join(
				Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE,
				lSelectedValues.ToArray());
		}

		if (ddlProductOptionValueSetting.Visible)
		{
			return ddlProductOptionValueSetting.SelectedValue;
		}

		if (tbProductOptionValueSetting.Visible)
		{
			return tbProductOptionValueSetting.Text;
		}
		return null;
	}

	/// <summary>
	/// 商品付帯情報の入力チェック
	/// </summary>
	/// <param name="fpItemInputs">定期購入商品情報</param>
	/// <returns>エラーメッセージ</returns>
	private string ValidateProductOptionValue(IEnumerable<FixedPurchaseItemInput> fpItemInputs)
	{
		var errorMsg = new StringBuilder();
		var checkKbn = "OptionValueValidate";
		var fpItemInputIndex = 0;
		foreach (var fpItemInput in fpItemInputs)
		{
			if (fpItemInput.ProductOptionSettingList != null)
			{
				foreach (ProductOptionSetting optionSetting in fpItemInput.ProductOptionSettingList)
				{
					var tmpValueName = optionSetting.ValueName;
					optionSetting.ValueName = string.Format(
						ReplaceTag("@@DispText.common_message.location_no@@"),
						(fpItemInputIndex + 1).ToString(),
						optionSetting.ValueName);

					// 付帯情報の入力チェック
					if (optionSetting.IsTextBox)
					{
						var validatorXml = optionSetting.CreateValidatorXml(checkKbn);
						var validateValue = optionSetting.SelectedSettingValue ?? optionSetting.DefaultValue;
						var param = new Hashtable
						{
							{ optionSetting.ValueName, validateValue }
						};
						errorMsg.Append(Validator.Validate(checkKbn, validatorXml.InnerXml, param));
						optionSetting.ValueName = tmpValueName;
					}
					else if (optionSetting.IsCheckBox)
					{
						var optionSettingCheckBoxList =
							optionSetting.SettingValuesListItemCollection.Cast<ListItem>().ToList();
						if (optionSetting.IsNecessary && (optionSettingCheckBoxList.Count(value => value.Selected) == 0))
						{
							errorMsg.Append(
								WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR)
									.Replace("@@ 1 @@", optionSetting.ValueName));
						}
						optionSetting.ValueName = tmpValueName;
					}
					else if (optionSetting.IsSelectMenu)
					{
						if (optionSetting.IsNecessary && (optionSetting.SelectedSettingValue == optionSetting.SettingValues.First()))
						{
							errorMsg.Append(
								WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR)
									.Replace("@@ 1 @@", optionSetting.ValueName));
						}
						optionSetting.ValueName = tmpValueName;
					}
				}
			}
			fpItemInputIndex++;
		}

		return errorMsg.ToString();
	}

	/// <summary>
	/// 定期購入商品情報入力クラスから商品付帯情報を取得
	/// </summary>
	/// <param name="fpItemInput">定期購入商品情報入力</param>
	/// <returns>商品付帯情報リスト</returns>
	protected List<ProductOptionSetting> GetProductOptionValueSettings(FixedPurchaseItemInput fpItemInput)
	{
		// 商品付帯情報リスト取得
		var product = new ProductService().Get(fpItemInput.ShopId, fpItemInput.ProductId);
		if (product == null) return new ProductOptionSettingList().Items;
		var productOptionList = new ProductOptionSettingList(product.ProductOptionSettings);

		// デフォルト値を設定
		productOptionList.SetDefaultValueFromProductOptionTexts(fpItemInput.ProductOptionTexts);
		return productOptionList.Items;
	}

	/// <summary>
	/// Get Search Page Url
	/// </summary>
	/// <returns> Return url page</returns>
	protected string GetSearchPageUrl()
	{
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false))
		{
			var subscriptionBoxFlag = string.Format("'{0}','{1}'", Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_VALID, Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY);
			return new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, Constants.KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG, subscriptionBoxFlag)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID, this.FixedPurchaseContainer.SubscriptionBoxCourseId)
				.AddParam(Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID, HttpUtility.UrlEncode(this.MemberRankId))
				.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, Constants.FLG_PRODUCT_VALID_FLG_VALID)
				.CreateUrl();
		}
		return new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT, "1")
			.AddParam(Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID, HttpUtility.UrlEncode(this.MemberRankId))
			.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, Constants.FLG_PRODUCT_VALID_FLG_VALID)
			.CreateUrl();
	}

	/// <summary>
	/// 決済利用チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string CheckLimitedPayment(FixedPurchaseInput input)
	{
		var prodcts = input.Shippings[0].Items
			.Where(item => (item.CanUsePayment(input.OrderPaymentKbn) == false))
			.Select(item => item.Name)
			.ToArray();
		if (prodcts.Length == 0) return string.Empty;
		var orderPaymentName = new PaymentService().Get(input.ShopId, input.OrderPaymentKbn);
		var displayProduct = string.Empty;
		foreach (var prodctName in prodcts)
		{
			if (string.IsNullOrEmpty(displayProduct))
			{
				displayProduct = "「" + prodctName + "」";
			}
			else
			{
				displayProduct += "、「" + prodctName + "」";
			}
		}
		var errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_LIMITED_PAYMENT_ERROR)
			.Replace("@@ 1 @@", displayProduct)
			.Replace("@@ 2 @@", orderPaymentName.PaymentName);
		return errorMessage;
	}
	/// <summary>
	/// 商品検索URL作成
	/// </summary>
	/// <returns>商品検索URL</returns>
	public string SarchUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
			.AddParam(
				Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN,
				HttpUtility.UrlEncode(Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT))
			.AddParam(Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT, "=1")
			.AddParam(Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID, HttpUtility.UrlEncode(this.MemberRankId))
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID, this.FixedPurchaseContainer.SubscriptionBoxCourseId)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, Constants.FLG_PRODUCT_VALID_FLG_VALID);
		return url.CreateUrl();
	}

	/// <summary>
	/// リピーターイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rItemList_OnDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) return;
		if ((e.Item.ItemType != ListItemType.Item) && (e.Item.ItemType != ListItemType.AlternatingItem)) return;

		var rbProductOptionValueInputFormText = (RadioButton)e.Item.FindControl("rbProductOptionValueInputFormText");
		var rbProductOptionValueInputForm = (RadioButton)e.Item.FindControl("rbProductOptionValueInputForm");
		rbProductOptionValueInputFormText.Checked = false;
		rbProductOptionValueInputForm.Checked = true;
		rbProductOptionValueInputFormText.Visible = false;
		rbProductOptionValueInputForm.Visible = false;

		e.Item.FindControl("dvProductOptionValueInputFormText").Visible = false;
		e.Item.FindControl("dvProductOptionValueInputForm").Visible = true;

		var productId = ((TextBox)e.Item.FindControl("tbProductId")).Text;
		var settingList = new ProductOptionSettingList(this.LoginOperatorShopId, productId);
		var text = ((TextBox)e.Item.FindControl("tbProductOptionTexts")).Text.Trim();
		settingList.SetDefaultValueFromProductOptionTexts(text);

		var rProductOptionValueSettings = ((Repeater)e.Item.FindControl("rProductOptionValueSettings"));
		rProductOptionValueSettings.DataSource = settingList;
		rProductOptionValueSettings.DataBind();
	}

	/// <summary>
	/// PayTg端末状態取得
	/// </summary>
	private void GetPayTgDeviceStatus()
	{
		var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
			? Constants.PATH_ROOT + Constants.PAGE_MANAGER_CHECK_DEVICE_STATUS_MOCK
			: Constants.PAYMENT_SETTING_PAYTG_DEVICE_STATUS_CHECK_URL;

		// PayTG連動でカード登録実行
		ScriptManager.RegisterStartupScript(
			this,
			GetType(),
			"execGetDeviceStatus",
			string.Format("execGetPayTgDeviceStatus('{0}');", apiUrl),
			true);
	}

	#region +プロパティ
	/// <summary>会員ランクID</summary>
	protected string MemberRankId
	{
		get { return (string)ViewState["MemberRankId"]; }
		set { ViewState["MemberRankId"] = value; }
	}
	/// <summary>ユーザークレジットカード情報</summary>
	protected UserCreditCard[] UserCreditCards
	{
		get { return (UserCreditCard[])ViewState["UserCreditCards"]; }
		set { ViewState["UserCreditCards"] = value; }
	}
	/// <summary>時間帯選択できるか</summary>
	protected bool IsValidShippingTimeSetFlg
	{
		get { return ((ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS) && (this.DeliveryCompany.IsValidShippingTimeSetFlg)); }
	}
	/// <summary>配送先の住所が日本か</summary>
	protected bool IsShippingAddrJp
	{
		get { return IsCountryJp(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所がアメリカか</summary>
	protected bool IsShippingAddrUs
	{
		get { return IsCountryUs(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>ユーザーの住所郵便番号が必須か</summary>
	protected bool IsUserAddrZipNecessary
	{
		get { return IsAddrZipcodeNecessary(this.ShippingAddrCountryIsoCode); ; }
	}
	/// <summary>配送先の住所国ISOコード</summary>
	protected string ShippingAddrCountryIsoCode
	{
		get { return ddlShippingCountry.SelectedValue; }
	}
	/// <summary>Can delete</summary>
	protected bool CanDelete { get; set; }
	/// <summary>Is Shipping Taiwan</summary>
	protected bool IsShippingAddrTw
	{
		get { return IsCountryTw(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>User Shipping Receiving Store Flg</summary>
	protected string UserShippingReceivingStoreFlg
	{
		get { return (string)ViewState["UserShippingReceivingStoreFlg"]; }
		set { ViewState["UserShippingReceivingStoreFlg"] = value; }
	}
	/// <summary>User Model</summary>
	protected new UserModel User
	{
		get { return (UserModel)ViewState["User"]; }
		set { ViewState["User"] = value; }
	}
	/// <summary>頒布会注文か</summary>
	protected bool IsSubscriptionBox
	{
		get
		{
			return Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
				&& (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false);
		}
	}
	/// <summary>配送不可エリアかどうか</summary>
	private bool IsUnavailableShippingArea { get; set; }
	/// <summary>PayTgクレジットトークン</summary>
	protected string CreditTokenbyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_CUSTOMERID]; }
		set { this.Session[PayTgConstants.PARAM_CUSTOMERID] = value; }
	}
	/// <summary>PayTgクレジット会社コード</summary>
	protected string CreditCardCompanyCodebyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_ACQNAME]; }
		set { this.Session[PayTgConstants.PARAM_ACQNAME] = value; }
	}
	#endregion
}
