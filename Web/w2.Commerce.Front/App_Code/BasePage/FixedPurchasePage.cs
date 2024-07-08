/*
=========================================================================================================
  Module      : 定期系基底ページ(FixedPurchasePage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Helper;
using w2.App.Common.Order.Register;
using w2.App.Common.Product;
using w2.Common.Logger;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.Payment;
using w2.Domain.ShopShipping;
using w2.Domain.SubscriptionBox;
using w2.Domain.TwFixedPurchaseInvoice;
using w2.Domain.TwUserInvoice;
using w2.Domain.User;

/// <summary>
/// FixedPurchasePage の概要の説明です
/// </summary>
public class FixedPurchasePage : HistoryPage
{
	#region +メソッド
	/// <summary>
	/// 定期購入情報セット
	/// </summary>
	protected void SetFixedPurchaseInfo()
	{
		var isExsit = true;

		// 定期購入IDパラメータなし?
		if (string.IsNullOrEmpty(this.RequestFixedPurchaseId))
		{
			isExsit = false;
			FileLogger.WriteError("該当定期情報の定期購入IDパラメータはNullです。");
		}

		// 定期購入情報取得
		if (isExsit)
		{
			var service = new FixedPurchaseService();
			this.FixedPurchaseContainer = service.GetContainer(this.RequestFixedPurchaseId);
			if ((this.FixedPurchaseContainer == null) || (this.FixedPurchaseContainer.UserId != this.LoginUserId))
			{
				isExsit = false;
				FileLogger.WriteError("該当定期情報を取得できません。");
			}

			// 定期商品1つも存在していなければ、エラーページに遷移する
			if (this.FixedPurchaseContainer.Shippings[0].Items.Any() == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXEDPURCHASE_NO_ITEMS);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// 解約理由取得
			this.CancelReason = new FixedPurchaseCancelReasonModel();
			if ((this.FixedPurchaseContainer != null) && (this.FixedPurchaseContainer.CancelReasonId != ""))
			{
				this.CancelReason = service.GetCancelReason(this.FixedPurchaseContainer.CancelReasonId);
			}
		}
		// 配送種別情報取得
		if (isExsit)
		{
			this.ShopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(this.FixedPurchaseContainer.Shippings[0].Items[0].ShippingType);
			if (this.ShopShipping == null)
			{
				isExsit = false;
				FileLogger.WriteError("該当定期情報の配送種別は存在しません。");
			}
		}
		// 配送種別情報取得
		if (isExsit)
		{
			this.DeliveryCompany
				= new DeliveryCompanyService().Get(this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId);
			if (this.DeliveryCompany == null)
			{
				isExsit = false;
				FileLogger.WriteError("該当定期情報の配送サービス情報は存在しません。");
			}
			this.SelectedDeliveryCompany = this.SelectedDeliveryCompany ?? this.DeliveryCompany;
		}

		// 決済種別情報取得
		if (isExsit)
		{
			this.FixedPurchaseContainer.OrderPaymentKbn
				= OrderCommon.ConvertAmazonPaymentId(this.FixedPurchaseContainer.OrderPaymentKbn);
			this.Payment = DataCacheControllerFacade.GetPaymentCacheController().Get(this.FixedPurchaseContainer.OrderPaymentKbn);
			if (this.Payment == null)
			{
				isExsit = false;
				FileLogger.WriteError("該当定期情報の決済種別は存在しません。");
			}
		}

		// データが存在しない場合はエラーページへ遷移
		if (isExsit == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXEDPURCHASE_UNDISP);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// 定期購入再開可能？
		// ※継続課金を利用できる決済である定期購入には定期購入解約してから再開できないようにする。
		this.CanResumeFixedPurchase = ((this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus
				&& (OrderCommon.IsCancelablePaymentContinuousByApi(this.FixedPurchaseContainer.OrderPaymentKbn) == false))
			|| this.FixedPurchaseContainer.IsSuspendFixedPurchaseStatus);

		// 定期購入キャンセル可能?
		// 定期購入休止可能?
		this.CanCancelFixedPurchase = this.CanSuspendFixedPurchase =
			((this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false)
				&& (this.FixedPurchaseContainer.IsSuspendFixedPurchaseStatus == false));

		//現在利用しているユーザクレジットカード情報のセット
		if (this.FixedPurchaseContainer.UseUserCreditCard)
		{
			this.UserCreditCardInfo = UserCreditCard.Get(this.FixedPurchaseContainer.UserId, this.FixedPurchaseContainer.CreditBranchNo.Value);
		}
		//ユーザの持っている利用可能なクレジットカード群
		this.UserCreditCardsUsable = UserCreditCard.GetUsable(this.LoginUserId);

		// 利用可能な支払い方法選択肢をセット
		this.ValidPayments = LoadPaymentValidList(CreateSimpleCartListForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId).Items[0]);

		if (((this.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF)
			&& (Constants.PAYMENT_SMS_DEF_KBN == Constants.PaymentSmsDef.YamatoKa)))
		{
			this.ValidPayments = this.ValidPayments
				.Select(payments => payments.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)).ToArray())
				.ToList();
		}

		// 翻訳情報設定
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			this.FixedPurchaseContainer = SetProductNameTranslationDataToFixedPurchaseContainer(this.FixedPurchaseContainer);
			this.Payment.PaymentName = NameTranslationCommon.GetTranslationName(
				this.Payment.PaymentId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PAYMENT_PAYMENT_NAME,
				this.Payment.PaymentName);

			var cancelReason = DataCacheControllerFacade.GetFixedPurchaseCancelReasonCacheController().GetCancelReason(this.CancelReason.CancelReasonId);
			this.CancelReason.CancelReasonName = (cancelReason != null)
				? NameTranslationCommon.GetTranslationName(
					this.CancelReason.CancelReasonId,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON,
					Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME,
					cancelReason.CancelReasonName)
				: string.Empty;

			// 次回購入利用クーポン情報があれば、翻訳クーポン名を設定
			if (this.FixedPurchaseContainer.NextShippingUseCouponDetail != null)
			{
				this.FixedPurchaseContainer.NextShippingUseCouponDetail = NameTranslationCommon.SetCouponTranslationData(
					new[] { this.FixedPurchaseContainer.NextShippingUseCouponDetail },
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId)[0];
			}

			// For case taiwan invoice enable
			if (OrderCommon.DisplayTwInvoiceInfo())
			{
				this.TwFixedPurchaseInvoiceModel = new TwFixedPurchaseInvoiceService().GetTaiwanFixedPurchaseInvoice(
					this.FixedPurchaseContainer.FixedPurchaseId,
					this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo);

				this.TwUserInvoiceList = new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.FixedPurchaseContainer.UserId);
			}
		}
	}

	/// <summary>
	/// カート情報リストの作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <returns>カート情報リスト</returns>
	protected CartObjectList CreateSimpleCartListForFixedPurchase(string fixedPurchaseId)
	{
		var fixedPurchase = new FixedPurchaseService().Get(fixedPurchaseId);
		var user = new UserService().Get(this.LoginUserId);
		if (user == null) return null;

		var orderRegister = new OrderRegisterFixedPurchaseInner(user.IsMember, Constants.FLG_LASTCHANGED_USER, false, fixedPurchaseId, null);
		var cartObject = new OrderRegisterFixedPurchase(Constants.FLG_LASTCHANGED_USER, false, false, null)
			.CreateCartList(fixedPurchase, user, orderRegister);
		return cartObject;
	}

	/// <summary>
	/// 利用可能な支払い方法リストの取得
	/// </summary>
	/// <param name="cartObject">注文情報より作成したカート情報</param>
	/// <returns>有効決済種別</returns>
	protected List<PaymentModel[]> LoadPaymentValidList(CartObject cartObject)
	{
		var validPayments = new List<PaymentModel[]>();
		if (cartObject == null) return validPayments;

		// 配送先で再計算
		cartObject.Calculate(false, isShippingChanged: true);

		// 決済種別情報セット（配送種別で決済種別を限定しているのであれば、有効の決済種別のみ取得）
		var validPaymentList = OrderCommon.GetValidPaymentList(
			cartObject,
			this.LoginUserId,
			isMultiCart: this.CartList.IsMultiCart);

		//Cart情報より支払い方法の制限時メッセージのセット
		this.DispLimitedPaymentMessages = new Hashtable();
		this.DispLimitedPaymentMessages[0] = OrderCommon.GetLimitedPaymentsMessagesForCart(cartObject, validPaymentList);

		// 支払い方法の取得
		var paymentsUnlimit = OrderCommon.GetPaymentsUnLimitByProduct(cartObject, validPaymentList);

		var regKey = Constants.PAYMENT_LINEPAY_OPTION_ENABLED
			? new UserService().GetUserExtend(this.LoginUserId).UserExtendDataValue[Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY]
			: string.Empty;

		var paymentIdsCanChange = OrderCommon.GetPaymentIdsCanChange(
			cartObject.Payment.PaymentId,
			Constants.SETTING_CAN_CHANGE_FIXEDPURCHASE_PAYMENT_IDS,
			Constants.FIXEDPURCHASE_PAYMENT_IDS_PRIORITY_OPTION_ENABLED);

		// 決済種別の制限
		validPayments.Add(
			paymentsUnlimit.Where(
				payment =>
				{
					switch (payment.PaymentId)
					{
						case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE:
							var shippingReceivingStoreType = this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreType;
							if ((this.UseShippingReceivingStore == false)
								|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
									&& (ECPayUtility.GetIsCollection(shippingReceivingStoreType)
										== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF)))
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
							// 支払い方法が後払いでかつ、決済エラーの場合は選択肢より除外
							if ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
								&& (this.FixedPurchaseContainer.PaymentStatus == Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR))
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
							if ((Constants.AMAZON_PAYMENT_OPTION_ENABLED == false) || Constants.AMAZON_PAYMENT_CV2_ENABLED)
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
							if ((Constants.AMAZON_PAYMENT_OPTION_ENABLED == false) || (Constants.AMAZON_PAYMENT_CV2_ENABLED == false))
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY:
							// 支払い方法が後払いでかつ、決済エラーの場合は選択肢より除外
							// または、注文者の住所か配送先が台湾以外の場合、選択肢より除外
							if (((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
									&& (this.FixedPurchaseContainer.PaymentStatus == Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR))
								|| (TriLinkAfterPayHelper.CheckUsedPaymentForTriLinkAfterPay(
									payment.PaymentId,
									this.FixedPurchaseShippingContainer.ShippingCountryIsoCode,
									cartObject.Owner.AddrCountryIsoCode)))
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT:
							//メール便の場合は除外
							if (this.FixedPurchaseShippingContainer.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
							if (Constants.PAYMENT_PAIDY_OPTION_ENABLED == false)
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
							var hasPaymentDataAtone = OrderCommon.HasPaymentDataAfteeOrAtone(
								this.FixedPurchaseContainer.FixedPurchaseId,
								Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
							if ((Constants.PAYMENT_ATONEOPTION_ENABLED == false)
								|| (hasPaymentDataAtone == false)) return false;
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
							var isHavePaymentDataAftee = OrderCommon.HasPaymentDataAfteeOrAtone(
								this.FixedPurchaseContainer.FixedPurchaseId,
								Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
							if ((Constants.PAYMENT_AFTEEOPTION_ENABLED == false)
								|| (isHavePaymentDataAftee == false)) return false;
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
							if ((Constants.PAYMENT_LINEPAY_OPTION_ENABLED == false)
								|| string.IsNullOrEmpty(regKey))
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF:
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
							if (this.FixedPurchaseContainer.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU:
							if (Constants.PAYMENT_BOKU_OPTION_ENABLED == false) return false;
							break;

						default:
							// 定期：マイページから変更可能な決済種別IDの追加設定に含まれていた場合利用可能にする
							return Constants.SETTING_PARTICULAR_USABLE_FIXEDPURCHASE_PAYMENT_IDS_WHEN_CHANGE_ADDITIONAL_SETTING.Contains(payment.PaymentId);
					}
					return true;
				}).Where(payment => IsValidModifyFront(payment.PaymentId, paymentIdsCanChange))
					.OrderBy(item => item.DisplayOrder)
					.ToArray());
		return validPayments;
	}

	/// <summary>
	/// 定期購入情報解約入力画面URL作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="nextPageKbn">遷移画面区分</param>
	/// <returns>定期購入情報解約入力画面URL</returns>
	protected string CreateFixedPurchaseCancelReasonInputUrl(string fixedPurchaseId, string nextPageKbn)
	{
		return this.CreateFixedPurchaseCancelReasonUrl(
			Constants.PAGE_FRONT_FIXED_PURCHASE_CANCEL_INPUT,
			fixedPurchaseId,
			nextPageKbn);
	}
	/// <summary>
	/// 定期購入情報解約確認画面URL作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="nextPageKbn">遷移画面区分</param>
	/// <returns>定期購入情報解約確認画面URL</returns>
	protected string CreateFixedPurchaseCancelReasonConfirmUrl(string fixedPurchaseId, string nextPageKbn)
	{
		return this.CreateFixedPurchaseCancelReasonUrl(
			Constants.PAGE_FRONT_FIXED_PURCHASE_CANCEL_CONFIRM,
			fixedPurchaseId,
			nextPageKbn);
	}
	/// <summary>
	/// 定期購入情報解約完了画面URL作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="nextPageKbn">遷移画面区分</param>
	/// <returns>定期購入情報解約完了画面URL</returns>
	protected string CreateFixedPurchaseCancelReasonCompleteUrl(string fixedPurchaseId, string nextPageKbn)
	{
		return this.CreateFixedPurchaseCancelReasonUrl(
			Constants.PAGE_FRONT_FIXED_PURCHASE_CANCEL_COMPLETE,
			fixedPurchaseId,
			nextPageKbn);
	}
	/// <summary>
	/// 定期購入情報解約画面URL作成
	/// </summary>
	/// <param name="page">画面</param>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="nextPageKbn">遷移画面区分</param>
	/// <returns>定期購入情報解約画面URL</returns>
	private string CreateFixedPurchaseCancelReasonUrl(string page, string fixedPurchaseId, string nextPageKbn)
	{
		var result = new StringBuilder();

		result.Append(Constants.PATH_ROOT).Append(page);
		result.Append("?").Append(Constants.REQUEST_KEY_FIXED_PURCHASE_ID).Append("=").Append(HttpUtility.UrlEncode(fixedPurchaseId));
		result.Append("&" + Constants.REQUEST_KEY_FIXED_PURCHASE_NEXT_PAGE_KBN + "=" + HttpUtility.UrlEncode(nextPageKbn));

		return result.ToString();
	}

	/// <summary>
	/// 商品名翻訳情報を定期購入情報に設定
	/// </summary>
	/// <param name="container">定期購入情報</param>
	/// <returns>定期購入情報</returns>
	protected FixedPurchaseContainer SetProductNameTranslationDataToFixedPurchaseContainer(FixedPurchaseContainer container)
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT,
			MasterId1List = container.Shippings[0].Items.Select(item => item.ProductId).ToList(),
			LanguageCode = RegionManager.GetInstance().Region.LanguageCode,
			LanguageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId,
		};
		var translationSettings =
			new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);

		if (translationSettings.Any() == false) return container;

		container.Shippings[0].Items = container.Shippings[0].Items.Select(
			item => SetProductNameTranslationDataToFixedPurchaseContainer(item, translationSettings)).ToArray();
		return container;
	}
	/// <summary>
	/// 商品名翻訳情報を定期購入情報に設定
	/// </summary>
	/// <param name="item">定期購入情報(商品)</param>
	/// <param name="translationSettings">翻訳設定情報</param>
	/// <returns>定期購入情報(商品)</returns>
	private FixedPurchaseItemContainer SetProductNameTranslationDataToFixedPurchaseContainer(FixedPurchaseItemContainer item, NameTranslationSettingModel[] translationSettings)
	{
		var productNameTranslationSetting = translationSettings.FirstOrDefault(
			setting => (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_NAME)
				&& (setting.MasterId1 == item.ProductId));

		item.Name = (productNameTranslationSetting != null)
			? productNameTranslationSetting.AfterTranslationalName
			: item.Name;
		return item;
	}

	/// <summary>
	/// 頒布会コース名を取得
	/// </summary>
	/// <returns>頒布会コース名</returns>
	protected string GetSubscriptionBoxDisplayName()
	{
		if (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId)) return string.Empty;
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.FixedPurchaseContainer.SubscriptionBoxCourseId);
		return subscriptionBox.DisplayName;
	}

	/// <summary>
	/// オプション価格を表示するかどうか
	/// </summary>
	/// <returns>オプション価格表示非表示</returns>
	protected bool IsDisplayOptionPrice()
	{
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) return false;

		var fpItemInputs = new FixedPurchaseInput(this.FixedPurchaseContainer);
		var result = fpItemInputs.Shippings[0].Items
			.Any(fixedPurchaseInput => (fixedPurchaseInput.OptionPrice != 0)
				|| fixedPurchaseInput.ProductOptionSettingsWithSelectedValues.HasOptionPrice);
		return result;
	}
	#endregion

	#region +プロパティ
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示
	/// <summary>リクエスト：定期購入ID</summary>
	protected string RequestFixedPurchaseId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXED_PURCHASE_ID]).Trim(); }
	}
	/// <summary>定期購入情報</summary>
	protected FixedPurchaseContainer FixedPurchaseContainer
	{
		get { return (FixedPurchaseContainer)ViewState["FixedPurchase"]; }
		set { ViewState["FixedPurchase"] = value; }
	}
	/// <summary>定期購入配送先情報</summary>
	protected FixedPurchaseShippingContainer FixedPurchaseShippingContainer
	{
		get { return this.FixedPurchaseContainer.Shippings[0]; }
	}
	/// <summary>ユーザの持っている利用可能なクレジットカード群</summary>
	protected UserCreditCard[] UserCreditCardsUsable
	{
		get { return (UserCreditCard[])ViewState["UserCreditCardsUsable"]; }
		set { ViewState["UserCreditCardsUsable"] = value; }
	}
	/// <summary>現在利用しているユーザクレジットカード情報</summary>
	protected UserCreditCard UserCreditCardInfo
	{
		get { return (UserCreditCard)ViewState["UserCreditCardInfo"]; }
		set { ViewState["UserCreditCardInfo"] = value; }
	}
	/// <summary>配送種別情報</summary>
	protected ShopShippingModel ShopShipping
	{
		get { return (ShopShippingModel)ViewState["ShopShipping"]; }
		set { ViewState["ShopShipping"] = value; }
	}
	/// <summary>配送会社情報</summary>
	protected DeliveryCompanyModel DeliveryCompany
	{
		get { return (DeliveryCompanyModel)ViewState["DeliveryCompany"]; }
		set { ViewState["DeliveryCompany"] = value; }
	}
	/// <summary>選択中の配送会社情報</summary>
	protected DeliveryCompanyModel SelectedDeliveryCompany
	{
		get { return (DeliveryCompanyModel)ViewState["SelectedDeliveryCompany"]; }
		set { ViewState["SelectedDeliveryCompany"] = value; }
	}
	/// <summary>決済種別情報</summary>
	protected PaymentModel Payment
	{
		get { return (PaymentModel)ViewState["Payment"]; }
		set { ViewState["Payment"] = value; }
	}
	/// <summary>定期解約理由区分設定</summary>
	protected FixedPurchaseCancelReasonModel CancelReason
	{
		get { return (FixedPurchaseCancelReasonModel)ViewState["CancelReason"]; }
		set { ViewState["CancelReason"] = value; }
	}
	/// <summary>定期購入キャンセル可能かどうか</summary>
	protected bool CanCancelFixedPurchase
	{
		get { return (bool)ViewState["CanCancelFixedPurchase"]; }
		set { ViewState["CanCancelFixedPurchase"] = value; }
	}
	/// <summary>定期購入情報解約理由入力情報</summary>
	protected FixedPurchaseCancelReasonInput CancelReasonInput
	{
		get { return (FixedPurchaseCancelReasonInput)Session["CancelReasonInput"]; }
		set { Session["CancelReasonInput"] = value; }
	}
	/// <summary>定期購入情報休止理由入力情報</summary>
	protected FixedPurchaseSuspendReasonInput SuspendReasonInput
	{
		get { return (FixedPurchaseSuspendReasonInput)Session["SuspendReasonInput"]; }
		set { Session["SuspendReasonInput"] = value; }
	}
	/// <summary>支払い方法の制限時メッセージ</summary>
	protected Hashtable DispLimitedPaymentMessages
	{
		get { return (Hashtable)Session["DispLimitedPaymentMessages"]; }
		set { Session["DispLimitedPaymentMessages"] = value; }
	}
	/// <summary>定期購入休止可能かどうか</summary>
	protected bool CanSuspendFixedPurchase
	{
		get { return (bool)ViewState["CanSuspendFixedPurchase"]; }
		set { ViewState["CanSuspendFixedPurchase"] = value; }
	}
	/// <summary>定期購入再開可能かどうか</summary>
	protected bool CanResumeFixedPurchase
	{
		get { return (bool)ViewState["CanResumeFixedPurchase"]; }
		set { ViewState["CanResumeFixedPurchase"] = value; }
	}
	/// <summary>定期キャンセルか</summary>
	protected bool IsCancel
	{
		get { return (this.FixedPurchaseNextPageKbn == Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_CANCEL); }
	}
	/// <summary>定期休止か</summary>
	protected bool IsSuspend
	{
		get { return (this.FixedPurchaseNextPageKbn == Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_SUSPEND); }
	}
	/// <summary>定期遷移区分</summary>
	protected string FixedPurchaseNextPageKbn
	{
		get { return Request[Constants.REQUEST_KEY_FIXED_PURCHASE_NEXT_PAGE_KBN]; }
	}
	/// <summary>定期購入解約可能かどうか</summary>
	protected int FixedPurchaseCancelableCount
	{
		get { return ViewState["FixedPurchaseCancelableCount"] == null ? 0 : (int)ViewState["FixedPurchaseCancelableCount"];}
		set { ViewState["FixedPurchaseCancelableCount"] = value; }
	}
	/// <summary>定期購入解約可能かどうか</summary>
	protected bool IsCancelable
	{
		get { return (this.FixedPurchaseContainer.ShippedCount >= this.FixedPurchaseCancelableCount); }
	}
	/// <summary>Taiwan Fixed Purchase Invoice Model</summary>
	protected TwFixedPurchaseInvoiceModel TwFixedPurchaseInvoiceModel
	{
		get { return (TwFixedPurchaseInvoiceModel)ViewState["TwFixedPurchaseInvoiceModel"]; }
		set { ViewState["TwFixedPurchaseInvoiceModel"] = value; }
	}
	/// <summary>Taiwan User Invoice List</summary>
	protected TwUserInvoiceModel[] TwUserInvoiceList
	{
		get { return (TwUserInvoiceModel[])ViewState["TwUserInvoiceList"]; }
		set { ViewState["TwUserInvoiceList"] = value; }
	}
	/// <summary> Use Shipping Address </summary>
	protected bool UseShippingAddress
	{
		get
		{
			return (this.FixedPurchaseShippingContainer.ShippingReceivingStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
		}
	}
	/// <summary> Use Shipping Receiving Store </summary>
	protected bool UseShippingReceivingStore
	{
		get
		{
			return (this.FixedPurchaseShippingContainer.ShippingReceivingStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
		}
	}
	/// <summary>Is Display Input Order Payment Kbn</summary>
	protected bool IsDisplayInputOrderPaymentKbn
	{
		get
		{
			var result =
				(((this.FixedPurchaseContainer.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
					|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)
					|| (ECPayUtility.GetIsCollection(this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreType)
							== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF))
					&& (OrderCommon.IsOnlyCancelablePaymentContinuousByManual(this.FixedPurchaseContainer.OrderPaymentKbn) == false));
			return result;
		}
	}
	#endregion
}
