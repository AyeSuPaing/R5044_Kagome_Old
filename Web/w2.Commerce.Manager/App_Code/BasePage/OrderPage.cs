/*
=========================================================================================================
  Module      : 注文共通ページ(OrderPage.cs)
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
using System.Web.UI.WebControls;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Aftee;
using w2.App.Common.Order.Payment.Atone;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Order.Reauth;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.Point;
using w2.Domain.Product;
using w2.Domain.ProductStock.Helper;
using w2.Domain.ShopOperator;
using w2.Domain.SubscriptionBox;
using w2.Domain.TwFixedPurchaseInvoice;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

/// <summary>
/// OrderPage の概要の説明です
/// </summary>
public partial class OrderPage : ProductPage
{
	/// <summary>実行結果</summary>
	public enum ResultKbn
	{
		/// <summary>更新なし</summary>
		NoUpdate,
		/// <summary>更新OK</summary>
		UpdateOK,
		/// <summary>一部更新</summary>
		UpdatePart,
		/// <summary>更新NG</summary>
		UpdateNG
	}

	// ステートメント定数
	public const string FIELD_ORDER_ORDER_STOCKRESERVED_STATUS_LIST = Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS + "_list";		// 在庫引当ステータス(一覧用)
	public const string FIELD_ORDER_ORDER_SHIPPED_STATUS_LIST = Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS + "_list";					// 出荷ステータス(一覧用)
	public const string FIELD_ORDER_EXTEND_STATUS_LIST = "extend_status_list";															// 拡張ステータス(一覧用)

	// 拡張ステータス定数
	/// <summary>拡張ステータスベース名</summary>
	public const string FIELD_ORDER_EXTEND_STATUS = Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME;
	/// <summary>拡張ステータス更新日時ベース名</summary>
	public const string FIELD_ORDER_EXTEND_STATUS_DATE = Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME;
	/// <summary>調整前用ポイント</summary>
	protected const string CONST_ORDER_ORDER_POINT_USE_BEFORE = Constants.FIELD_ORDER_ORDER_POINT_USE + "_before";

	/// <summary>Payment type</summary>
	protected const string CONST_KEY_PAYMENT_TYPE = Constants.VALUETEXT_PARAM_PAYMENT_TYPE;

	/// <summary>Html control attribute class</summary>
	protected const string CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS = "class";
	/// <summary>Html control attribute class: discount</summary>
	protected const string CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT = "discount";

	/// <summary>Html control attribute style</summary>
	protected const string CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE = "style";
	/// <summary>Html control attribute style: display none</summary>
	protected const string CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE = "display:none";
	/// <summary>Html control attribute style: display empty</summary>
	protected const string CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_EMPTY = "display:''";
	/// <summary>Html control attribute style: scroll vertical</summary>
	protected const string CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_SCROLL_VERTICAL = "scroll-vertical";

	/// <summary>
	/// 受注詳細詳細URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="isPopUpAction">ポップアップさせるかどうか</param>
	/// <param name="reloadsParent">更新時に親ウィンドウをリロードするかどうか</param>
	/// <returns>注文情報詳細URL</returns>
	public string CreateOrderDetailUrl(string orderId, bool isPopUpAction, bool reloadsParent)
	{
		return CreateOrderDetailUrl(orderId, isPopUpAction, reloadsParent, (string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME]);
	}
	/// <summary>
	/// 受注詳細詳細URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="isPopUpAction">ポップアップさせるかどうか</param>
	/// <param name="reloadsParent">表示時に親ウィンドウをリロードするかどうか</param>
	/// <param name="popupParantName">ポップアップ元ページ（遷移元のリロード判定に利用）</param>
	/// <returns>注文情報詳細URL</returns>
	public static string CreateOrderDetailUrl(string orderId, bool isPopUpAction, bool reloadsParent, string popupParantName)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_ORDER_CONFIRM);
		url.Append("?").Append(Constants.REQUEST_KEY_ORDER_ID).Append("=").Append(HttpUtility.UrlEncode(orderId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));
		if (isPopUpAction)
		{
			url.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));
		}
		if (reloadsParent)
		{
			url.Append("&").Append(Constants.REQUEST_KEY_RELOAD_PARENT_WINDOW).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_RELOAD_PARENT_WINDOW_ON));
		}
		if (string.IsNullOrEmpty(popupParantName) == false)
		{
			url.Append("&").Append(Constants.REQUEST_KEY_MANAGER_POPUP_PARENT_NAME).Append("=").Append(HttpUtility.UrlEncode(popupParantName));
		}
		return url.ToString();
	}

	/// <summary>
	/// 配送先情報取得
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="accessor">Sql Accessor</param>
	/// <returns>配送先情報</returns>
	public static DataView GetOrderShippings(string orderId, SqlAccessor accessor = null)
	{
		var orderShippings = new OrderService().GetOrderShippingInDataView(orderId, accessor);
		return orderShippings;
	}

	/// <summary>
	/// 注文商品情報取得
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns></returns>
	public static DataView GetOrderItems(string strOrderId, string strShopId, SqlAccessor sqlAccessor)
	{
		return OrderCommon.GetOrderItems(strOrderId, strShopId, sqlAccessor);
	}

	/// <summary>
	/// 注文セットプロモーション情報取得
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>注文セットプロモーション情報</returns>
	public static DataView GetOrderSetPromotions(string orderId)
	{
		var orderSetPromotion = new OrderService().GetOrderSetPromotionInDataView(orderId);
		return orderSetPromotion;
	}

	/// <summary>
	/// 商品バリエーション情報取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="memberRankId">会員ランクID</param>
	/// <param name="productSaleId">商品セールID（闇市用）</param>
	/// <returns>商品バリエーション</returns>
	public static DataView GetProductVariation(string shopId, string productId, string variationId, string memberRankId = "", string productSaleId = "")
	{
		using (var sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();

			return GetProductVariation(shopId, productId, variationId, memberRankId, productSaleId, sqlAccessor);
		}
	}
	/// <summary>
	/// 商品バリエーション情報取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="memberRankId">会員ランクID</param>
	/// <param name="productSaleId">商品セールID（闇市用）</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>商品バリエーション</returns>
	protected static DataView GetProductVariation(string shopId, string productId, string variationId, string memberRankId, string productSaleId, SqlAccessor sqlAccessor)
	{
		using (var sqlStatement = new SqlStatement("Order", "GetProductVariation"))
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, variationId },
				{ Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID, memberRankId },
				{ Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, productSaleId }
			};

			return sqlStatement.SelectSingleStatement(sqlAccessor, input);
		}
	}

	/// <summary>
	/// 現在有効な価格を取得
	/// </summary>
	/// <param name="productObject">商品情報</param>
	/// <returns> 現在有効な価格</returns>
	/// <remarks>
	/// 価格の優先度は、セール価格>特別価格>通常価格(バリエーション)
	/// </remarks>
	public static decimal GetProductValidityPrice(object productObject)
	{
		var product = (productObject is DataRowView)
			? ((DataRowView)productObject).ToHashtable()
			: (Hashtable)productObject;

		switch (ProductPrice.GetProductPriceType(product, true))
		{
			case ProductPrice.PriceTypes.ClosedMarketPrice:
				return (decimal)product["closed_market_price"];

			case ProductPrice.PriceTypes.MemberRankPrice:
				return (decimal)product[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION];

			case ProductPrice.PriceTypes.TimeSale:
				return (decimal)product[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE];

			case ProductPrice.PriceTypes.Special:
				return (decimal)product[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE];
		}
		return (decimal)product[Constants.FIELD_PRODUCTVARIATION_PRICE];
	}

	/// <summary>
	/// 現在有効な定期購入価格を取得
	/// </summary>
	/// <param name="drvProduct">商品情報</param>
	/// <param name="firstTime">初回有無</param>
	/// <returns> 定期購入価格</returns>
	/// <remarks>
	/// 定期購入価格の優先度は、セール価格>特別価格>通常価格(バリエーション)
	/// </remarks>
	public static decimal GetFixedPurchaseProductValidityPrice(DataRowView drvProduct, bool firstTime)
	{
		switch (ProductPrice.GetFixedPurchaseProductPriceType(drvProduct, true, firstTime))
		{
			case ProductPrice.PriceTypes.FixedPurchaseFirsttimePrice:
				return (decimal)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE];

			case ProductPrice.PriceTypes.FixedPurchasePrice:
				return (decimal)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE];

			case ProductPrice.PriceTypes.MemberRankPrice:
				return (decimal)drvProduct[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION];

			case ProductPrice.PriceTypes.TimeSale:
				return (decimal)drvProduct[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE];

			case ProductPrice.PriceTypes.Special:
				return (decimal)drvProduct[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE];

			default:
				return (decimal)drvProduct[Constants.FIELD_PRODUCTVARIATION_PRICE];
		}
	}

	/// <summary>
	/// 注文情報更新ロック取得処理
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	protected static void GetUpdlockFromOrderTables(string strOrderId, SqlAccessor sqlAccessor)
	{
		OrderCommon.GetUpdlockFromOrderTables(strOrderId, sqlAccessor);
	}

	/// <summary>
	/// 決済種別情報取得取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="paymentSelectionFlg">決済選択の任意利用フラグ</param>
	/// <param name="permittedPaymentIds">許可決定種別ID</param>
	/// <param name="hasFixedPurchase">定期購入あり？</param>
	/// <returns>決済種別情報取得報モデル</returns>
	protected PaymentModel[] GetPaymentValidListPermission(string shopId, string paymentSelectionFlg, string permittedPaymentIds, bool hasFixedPurchase = false)
	{
		var paymentAllList = new PaymentService().GetValidAll(shopId)
			.OrderBy(p => p.DisplayOrder)
			.ThenBy(p => p.PaymentId)
			.ToArray();

		var result = paymentAllList;
		if (paymentSelectionFlg == Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID)
		{
			var permittedPaymentIdList = permittedPaymentIds.Split(',');
			result = result.Where(p => permittedPaymentIdList.Any(pp => pp == p.PaymentId)).ToArray();
		}

		if (hasFixedPurchase)
		{
			result = result.Where(p => Constants.CAN_FIXEDPURCHASE_PAYMENTIDS.Any(pp => pp == p.PaymentId)).ToArray();
		}

		var canNotUsePaymentList = new List<string>();
		if (Constants.AMAZON_PAYMENT_OPTION_ENABLED == false)
		{
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT);
		}

		if ((Constants.AMAZON_PAYMENT_OPTION_ENABLED) == false || (Constants.AMAZON_PAYMENT_CV2_ENABLED == false))
		{
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2);
		}

		if (Constants.PAYMENT_LINEPAY_OPTION_ENABLED == false)
		{
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY);
		}

		if (Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED == false)
		{
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
		}

		if (Constants.PAYMENT_BOKU_OPTION_ENABLED == false)
		{
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU);
		}

		if (Constants.PAYMENT_GMO_POST_ENABLED == false)
		{
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE);
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO);
		}

		if (Constants.GLOBAL_OPTION_ENABLE
			|| (Constants.PAYMENT_NETBANKING_OPTION_ENABLED == false))
		{
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET);
		}

		if ((Constants.PAYMENT_ATMOPTION_ENABLED == false)
			|| Constants.GLOBAL_OPTION_ENABLE)
		{
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_ATM);
		}

		if (Constants.GLOBAL_OPTION_ENABLE
			&& (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent))
		{
			canNotUsePaymentList.Add(Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
		}

		result = result.Where(p => canNotUsePaymentList.Any(pp => (pp == p.PaymentId)) == false).ToArray();

		//基軸通貨で決済を制限
		result = CurrencyManager.RemovePaymentsByKeyCurrency(result);

		return result;
	}

	/// <summary>
	/// Get notice message payment which user management level can not be used
	/// </summary>
	/// <param name="shopId">Shop ID</param>
	/// <param name="userLevelId">User Management Level ID</param>
	/// <returns>Notice message string containts payments names</returns>
	public static string GetPaymentUserManagementLevelNotUseMessage(string shopId, string userLevelId)
	{
		var payments = UserManagementLevelUtility.GetPaymentsUserManagementLevelNotUse(shopId, userLevelId);

		if (payments.Length == 0) return string.Empty;
		var message = string.Join("」「", payments.Select(m => m.PaymentName));

		return string.IsNullOrEmpty(message)
			? string.Empty
			: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_PAYMENT_USER_MANAGERMENT_LEVEL_NOTIFY)
			+ "「" + message + "」";
	}

	/// <summary>
	/// 注文者区分で使用不可決済種別文言を取得
	/// </summary>
	/// <param name="shopId">Shop ID</param>
	/// <param name="userKbn">注文者区分ID</param>
	/// <returns>利用不可決済種別名が入っている文言</returns>
	public static string GetPaymentOrderOwnerKbnNotUseMessage(string shopId, string userKbn)
	{
		var paymentList = new PaymentService().GetValidAll(shopId);
		var allOwnerKbnString = UserService.IsUser(userKbn)
			? Constants.FLG_USER_USER_KBN_ALL_USER
			: Constants.FLG_USER_USER_KBN_ALL_GUEST;

		var invalidPaymentList = new List<PaymentModel>();
		foreach (var validPayment in paymentList)
		{
			// 使用者不可注文者区分内にuserKbnと同じ分類になる全てのユーザー、全てのゲストがあるかチェック
			// ある場合は使用不可決済種別として格納
			var orderOwnerKbnList = validPayment.OrderOwnerKbnNotUse.Split(',');
			if (orderOwnerKbnList.Any(orderOwnerKbn => (orderOwnerKbn == allOwnerKbnString)))
			{
				invalidPaymentList.Add(validPayment);
				continue;
			}

			// ない場合は使用不可注文者にuserKbnと一致する項目があるかチェックし、決済種別を格納
			if (orderOwnerKbnList.Contains(userKbn)) invalidPaymentList.Add(validPayment);
		}

		if (invalidPaymentList.Count == 0) return string.Empty;

		var invalidPaymentName = string.Format("「{0}」", string.Join("」「", invalidPaymentList.Select(m => m.PaymentName)));

		var result = string.Format(
			"{0}{1}",
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_PAYMENT_ORDER_OWNER_KBN_NOTIFY),
			invalidPaymentName);
		return result;
	}

	/// <summary>
	/// 注文情報取得（ユーザーポイント情報含む）
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="strUserId">ユーザーID</param>
	/// <returns>注文情報、ユーザーポイント情報</returns>
	public DataSet GetOrderAndUserPoint(string strOrderId, string strUserId)
	{
		using (var sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();

			return GetOrderAndUserPoint(strOrderId, strUserId, sqlAccessor);
		}
	}
	/// <summary>
	/// 注文情報取得（ユーザーポイント情報含む）
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <param name="sqlAccessor">ユーザID</param>
	/// <returns>注文情報、ユーザーポイント情報</returns>
	public DataSet GetOrderAndUserPoint(string strOrderId, string strUserId, SqlAccessor sqlAccessor)
	{
		using (var sqlStatement = new SqlStatement("Order", "GetOrderAndUserPoint"))
		{
			var htInput = new Hashtable
		{
				{ Constants.FIELD_ORDER_ORDER_ID, strOrderId },
				{ Constants.FIELD_ORDER_USER_ID, strUserId }
			};

			return sqlStatement.SelectStatement(sqlAccessor, htInput);
		}
	}

	/// <summary>
	/// 注文情報オブジェクト取得
	/// </summary>
	/// <param name="strOrderId"></param>
	public Order GetOrderObject(string strOrderId)
	{
		var dvObject = OrderCommon.GetOrder(strOrderId);

		return (dvObject.Count > 0) ? new Order(dvObject) : new Order();
	}

	/// <summary>
	/// 注文商品入力チェック 必須入力チェックなし
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="orderOld">旧注文情報（特にない場合はnull）</param>
	/// <param name="orderItems">注文商品</param>
	/// <param name="itemsRepeater">呼び出し元のリピーター</param>
	/// <param name="isModify">受注情報編集で呼び出されているか</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckOrderItemRegistable(OrderInput order, OrderInput orderOld, OrderItemInput[] orderItems, RepeaterItem itemsRepeater, bool isModify = false)
	{
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();

			return CheckOrderItemRegistable(order, orderOld, orderItems, accessor, itemsRepeater, isModify);
		}
	}

	/// <summary>
	/// 商品付帯情報入力チェック
	/// </summary>
	/// <param name="orderInput">受注商品情報</param>
	/// <param name="productName">商品名</param>
	/// <param name="orderItemIndex">受注インデックス</param>
	/// <returns>エラーメッセージ</returns>
	public string CheckProductOptionValueText(OrderItemInput orderInput, string productName, int orderItemIndex)
	{
		if (orderInput == null) return string.Empty;

		var productOptionNecessaryErrorMessages =
			ValidateProductOptionNecessary(orderInput.ProductOptionSettingsWithSelectedValues, orderItemIndex);

		if (string.IsNullOrEmpty(productOptionNecessaryErrorMessages) == false) return productOptionNecessaryErrorMessages;

		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)
		{
			if (string.IsNullOrEmpty(orderInput.ProductOptionTexts)) return string.Empty;

			// 付帯情報の入力値に"："が含まれていない場合はエラーを返す
			var isNotContainsFullWidthColon = orderInput.ProductOptionTexts.Split('　').Any(x => x.Contains('：') == false);
			var errorMessage = isNotContainsFullWidthColon
				? string.Format(
					"{0}<br />",
					WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_TEXT_SETTING)
						.Replace("@@ 1 @@", productName))
				: string.Empty;
			return errorMessage;
		}
		else
		{
			var errorMessages = ValidateProductOptionPriceFormat(orderInput.ProductOptionSettingsWithSelectedValues, productName);
			return errorMessages;
		}
	}

	/// <summary>
	/// 商品付帯情報のバリデーション
	/// </summary>
	/// <param name="productOptionSelected">商品付帯情報</param>
	/// <param name="productName">商品名</param>
	/// <returns></returns>
	private string ValidateProductOptionPriceFormat(ProductOptionSettingList productOptionSelected, string productName)
	{
		var errorMessages = new ProductPage().CheckProductOptionValue(productOptionSelected);
		if(string.IsNullOrEmpty(errorMessages) == false) this.IsNeedShowConfirm = false;

		return errorMessages;
	}

	/// <summary>
	/// 商品付帯情報必須チェック
	/// </summary>
	/// <param name="productOptionSettingList">商品付帯情報</param>
	/// <param name="orderItemIndex">受注商品インデックス</param>
	/// <returns></returns>
	private string ValidateProductOptionNecessary(ProductOptionSettingList productOptionSettingList, int orderItemIndex)
	{
		var errorMessages = new StringBuilder();
		foreach (var productOptionSetting in productOptionSettingList.Items)
		{
			var tmpValueName = productOptionSetting.ValueName;
			productOptionSetting.ValueName = string.Format(
				ReplaceTag("@@DispText.common_message.location_no@@"),
				(orderItemIndex).ToString(),
				productOptionSetting.ValueName);

			// 付帯情報必須チェック
			if (productOptionSetting.IsTextBox)
			{
				if (productOptionSetting.IsNecessary && string.IsNullOrEmpty(productOptionSetting.SelectedSettingValue))
				{
					errorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NO_OPTIONS_SET_ERROR)
							.Replace("@@ 1 @@", productOptionSetting.ValueName));
				}
				productOptionSetting.ValueName = tmpValueName;
			}
			else if (productOptionSetting.IsCheckBox)
			{
				var productOptionSettingCheckBoxList =
					productOptionSetting.SettingValuesListItemCollection.Cast<ListItem>().ToList();
				if (productOptionSetting.IsNecessary && (productOptionSettingCheckBoxList.Count(value => value.Selected) == 0))
				{
					errorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NO_OPTIONS_SET_ERROR)
							.Replace("@@ 1 @@", productOptionSetting.ValueName));
					productOptionSetting.ValueName = tmpValueName;
				}
			}
			else if (productOptionSetting.IsSelectMenu)
			{
				if (productOptionSetting.IsNecessary && (productOptionSetting.SelectedSettingValue == productOptionSetting.SettingValues.First()))
				{
					errorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NO_OPTIONS_SET_ERROR)
							.Replace("@@ 1 @@", productOptionSetting.ValueName));
					productOptionSetting.ValueName = tmpValueName;
				}
			}
		}

		return errorMessages.ToString();
	}

	/// <summary>
	/// 注文商品入力チェック 必須入力チェックなし
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="orderOld">旧注文情報（特にない場合はnull）</param>
	/// <param name="orderItems">注文商品</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="itemsRepeater">呼び出し元のリピーター</param>
	/// <param name="isModify">受注情報編集で呼び出されているか</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckOrderItemRegistable(OrderInput order, OrderInput orderOld, OrderItemInput[] orderItems, SqlAccessor accessor, RepeaterItem itemsRepeater, bool isModify)
	{
		var errorMessage = new StringBuilder();

		// チェック対象商品がなければ抜ける
		var itemIndex = ((HiddenField)itemsRepeater.FindControl("hfItemIndex")).Value;
		if (orderItems.Any(i => i.ItemIndex == itemIndex) == false) return string.Empty;

		int targetOrderItemIndex = int.Parse(itemIndex);
		var targetOrderItem = orderItems.First(i => i.ItemIndex == itemIndex);

		// 商品情報取得
		var productVariations = new List<DataView>();
		foreach (var orderItem in orderItems)
		{
			var productVariation = GetProductVariation(orderItem.ShopId, orderItem.ProductId, orderItem.VariationId, "", "", accessor);
			productVariations.Add(productVariation);
		}

		// 商品全削除チェック
		var allDeleteItem = true;
		foreach (var orderItem in orderItems)
		{
			// 返品商品の場合は、次の商品へ
			if (orderItem.IsReturnItem) continue;
			// 削除対象の場合は、次の商品へ
			if (orderItem.DeleteTarget) continue;

			allDeleteItem = false;
			break;
		}
		// 商品全削除の場合
		if (allDeleteItem)
		{
			errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERITEM_NO_DATA));
			this.IsNeedShowConfirm = false;
		}

		// 商品配送種別チェック
		int index = -1;
		foreach (var orderItem in orderItems)
		{
			index++;
			// 返品商品の場合は、次の商品へ
			if (orderItem.IsReturnItem) continue;
			// 削除対象の場合は、次の商品へ
			if (orderItem.DeleteTarget) continue;
			// セット商品の場合は、次の商品へ
			if (orderItem.IsProductSet) continue;
			// 自サイト以外の場は、次の商品へ
			if (order.MallId != Constants.FLG_ORDER_MALL_ID_OWN_SITE) continue;

			// 商品情報が存在しない場合は、次の商品へ
			var productVariation = ((DataView)productVariations[index]);
			if (productVariation.Count == 0) continue;

			if (isModify == false)
			{
				// 配送種別が異なる？
				if (order.ShippingId != (string)productVariation[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE])
				{
					errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_SHIPPING_KBN_DIFF));
				}
			}
		}

		// 商品購入チェック
		if ((targetOrderItem.IsReturnItem == false)
			&& (targetOrderItem.DeleteTarget == false))
		{
			var productVariation = (DataView)productVariations[targetOrderItemIndex];
			if (productVariation.Count == 0)
			{
				// 商品情報存在チェック
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", targetOrderItem.ProductName));
			}
			else if ((string)productVariation[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
			{
				// 商品有効性チェック
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID).Replace("@@ 1 @@", targetOrderItem.ProductName));
			}
			else
			{
				// 購入可能ランクチェック
				var buyableRankId = (string)productVariation[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK];
				if (MemberRankOptionUtility.CheckMemberRankPermission(order.MemberRankId, buyableRankId) == false)
				{
					errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_MEMBER_RANK_BUYABLE)
						.Replace("@@ 1 @@", targetOrderItem.ProductName)
						.Replace("@@ 2 @@", buyableRankId));
				}
			}
		}

		// 商品在庫チェック
		if (Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED)
		{
			var errorProductNameList = CheckProductStock(orderOld, orderItems, accessor);
			foreach (var errorProductName in errorProductNameList)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_NO_STOCK).Replace("@@ 1 @@", errorProductName));
			}

			if (errorProductNameList.Count > 0)
			{
				this.IsNeedShowConfirm = false;
			}
		}
		return errorMessage.ToString();
	}

	/// <summary>
	/// 注文商品入力チェック
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="orderOld">旧注文情報（特にない場合はnull）</param>
	/// <param name="orderItems">注文商品</param>
	/// <param name="isModify">受注情報編集で呼び出されているか</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckOrderItem(OrderInput order, OrderInput orderOld, OrderItemInput[] orderItems, bool isModify = false)
	{
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();

			return CheckOrderItem(order, orderOld, orderItems, accessor, isModify);
		}
	}
	/// <summary>
	/// 注文商品入力チェック
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="orderOld">旧注文情報（特にない場合はnull）</param>
	/// <param name="orderItems">注文商品</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="isModify">受注情報編集で呼び出されているか</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckOrderItem(OrderInput order, OrderInput orderOld, OrderItemInput[] orderItems, SqlAccessor accessor, bool isModify)
	{
		var errorMessage = new StringBuilder();

		// 商品情報取得
		var productVariations = new List<DataView>();
		foreach (var orderItem in orderItems)
		{
			var productVariation = GetProductVariation(orderItem.ShopId, orderItem.ProductId, orderItem.VariationId, "", "", accessor);
			productVariations.Add(productVariation);
		}

		// 商品全削除チェック（全て返品 or 削除対象？）
		var allDeleteItem =
			orderItems.Where(i => (i.IsReturnItem == false))
			.All(i => i.DeleteTarget);
		// 商品全削除の場合
		if (allDeleteItem)
		{
			errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERITEM_NO_DATA));
			this.IsNeedShowConfirm = false;
		}

		// 商品配送種別チェック
		int index = -1;
		foreach (var orderItem in orderItems)
		{
			index++;
			// 返品商品の場合は、次の商品へ
			if (orderItem.IsReturnItem) continue;
			// 削除対象の場合は、次の商品へ
			if (orderItem.DeleteTarget) continue;
			// セット商品の場合は、次の商品へ
			if (orderItem.IsProductSet) continue;
			// 自サイト以外の場は、次の商品へ
			if (order.MallId != Constants.FLG_ORDER_MALL_ID_OWN_SITE) continue;

			// 商品情報が存在しない場合は、次の商品へ
			var productVariation = ((DataView)productVariations[index]);
			if (productVariation.Count == 0) continue;

			if (isModify == false)
			{
				// 配送種別が異なる？
				if (order.ShippingId != (string)productVariation[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE])
				{
					errorMessage.Append(WebMessages.GetMessages(
						(Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES.Any(site => site == order.MallId)
							&& (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP))
								? WebMessages.ERRMSG_MANAGER_PRODUCT_SHIPPING_KBN_DIFFERENT_TO_ORDER
								: WebMessages.ERRMSG_MANAGER_PRODUCT_SHIPPING_KBN_DIFF));
				}
			}
			else
			{
				if (order.ShippingId != (string)productVariation[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE])
				{
					Session["shipping_kbn_different_alert"] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_SHIPPING_KBN_DIFFERENT_ALERT);
				}
			}
		}

		// 注文商品入力チェック
		var no = 0;
		string errorMaxLength = MessageManager.GetMessages(MessageManager.INPUTCHECK_LENGTH_MAX).Replace("@@ 1 @@は@@ 2 @@", "");
		foreach (var orderItem in orderItems)
		{
			no++;

			// 返品商品の場合は、次の商品へ
			if (orderItem.IsReturnItem) continue;
			// 削除対象の場合は、次の商品へ
			if (orderItem.DeleteTarget) continue;

			// 定期購入を利用しない場合、または定期購入グラフがオフの場合、定期購入回数入力チェックを通す
			if ((Constants.FIXEDPURCHASE_OPTION_ENABLED　== false)
				|| (orderItem.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF))
			{
				orderItem.DataSource.Remove(w2.App.Common.Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT);
				orderItem.DataSource.Remove(w2.App.Common.Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT);
			}

			// 注文商品入力チェック
			var errorProduct = orderItem.Validate();
			if (((order.MallId == Constants.FLG_USER_MALL_ID_OWN_SITE)
					|| (errorProduct.All(x => (this.OmitCheckForOrderItem.Contains(x.Key) && (x.Value.Contains(errorMaxLength)))) == false))
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))
			{
				this.IsNeedShowConfirm = false;
			}
			errorMessage.Append(CheckProductOptionValueText(orderItem, orderItem.ProductName, no));

				// 「@@ 1 @@」を商品番号で置換し、エラーメッセージを連結
			errorMessage.Append(Validator.ChangeToDisplay(errorProduct)).Replace("@@ 1 @@", no.ToString() + "番目の");
		}

		// 商品チェック
		index = -1;
		foreach (var orderItem in orderItems)
		{
			index++;

			// 返品商品の場合は、次の商品へ
			if (orderItem.IsReturnItem) continue;
			// 削除対象の場合は、次の商品へ
			if (orderItem.DeleteTarget) continue;

			// 商品情報が存在しない場合
			var dvProductVariation = productVariations[index];
			if (dvProductVariation.Count == 0)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", orderItem.ProductName));
				this.IsNeedShowConfirm = false;
			}
			else if ((string)dvProductVariation[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
			{
				// 商品有効性チェック
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID).Replace("@@ 1 @@", orderItem.ProductName));
				this.IsNeedShowConfirm = false;
			}
		}

		// 会員ランク制御チェック
		index = -1;
		foreach (var orderItem in orderItems)
		{
			index++;

			// 返品商品の場合は、次の商品へ
			if (orderItem.IsReturnItem) continue;
			// 削除対象の場合は、次の商品へ
			if (orderItem.DeleteTarget) continue;

			// 商品情報が存在しない場合は、次の商品へ
			var productVariation = productVariations[index];
			if (productVariation.Count == 0) continue;

			// 購入不可会員ランク？
			var buyableRankId = (string)productVariation[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK];
			var blIsBuyableOK = MemberRankOptionUtility.CheckMemberRankPermission(order.MemberRankId, buyableRankId);
			if (blIsBuyableOK == false)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_MEMBER_RANK_BUYABLE).Replace("@@ 1 @@", orderItem.ProductName).Replace("@@ 2 @@", MemberRankOptionUtility.GetMemberRankName(buyableRankId)));
				this.IsNeedShowConfirm = false;
			}
		}

		//★★販売可能数量チェックはアラートで対応

		// 商品在庫チェック
		if (Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED)
		{
			var errorProductNameList = CheckProductStock(orderOld, orderItems, accessor);
			foreach (var errorProductName in errorProductNameList)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_NO_STOCK).Replace("@@ 1 @@", errorProductName));
			}

			if (errorProductNameList.Count > 0)
			{
				this.IsNeedShowConfirm = false;
			}
		}

		return errorMessage.ToString();
	}

	#region "ポイントオプション"
	/// <summary>
	/// ユーザーポイント情報取得(本・仮ポイント)
	/// </summary>
	/// <param name="strUserId">ユーザID</param>
	/// <param name="strOrderId">注文ID</param>
	/// <returns>ユーザーポイント情報取得</returns>
	public static UserPointModel[] GetUserPointByOrderId(string strUserId, string strOrderId)
	{
		var sv = new PointService();
		var model = sv.GetUserPoint(strUserId, string.Empty)
			.Where(userPoint =>
				((userPoint.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
					|| (userPoint.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP && userPoint.Kbn1 == strOrderId)))
			.ToArray();

		return model;
	}

    /// <summary>
    /// ユーザポイント情報追加
    /// </summary>
    /// <param name="order">注文情報</param>
    /// <param name="userPoint">ユーザポイント情報</param>
    /// <param name="adjustPoint">調整ポイント</param>
    /// <param name="updateHistoryAction">更新履歴アクション</param>
    /// <param name="accessor">SQLアクセサ</param>
    /// <returns>処理結果</returns>
    protected bool InsertUserPoint(
        OrderInput order,
        UserPointModel userPoint,
        decimal adjustPoint,
        UpdateHistoryAction updateHistoryAction,
        SqlAccessor accessor)
    {
        var sv = new PointService();
        // ポイントマスタ
        var master = sv.GetPointMaster().FirstOrDefault(x => x.DeptId == Constants.W2MP_DEPT_ID && x.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE);
		var rule = sv.GetPointRule(userPoint.DeptId, userPoint.PointRuleId);
        var model = new UserPointModel();

        model.UserId = order.UserId;
        model.PointKbn = StringUtility.ToEmpty(userPoint.PointKbn);
        model.PointKbnNo = sv.IssuePointKbnNoForUser(order.UserId, accessor);
        model.DeptId = this.LoginOperatorDeptId;
        model.PointRuleId = StringUtility.ToEmpty(userPoint.PointRuleId);
        model.PointRuleKbn = StringUtility.ToEmpty(userPoint.PointRuleKbn);
        model.PointType = userPoint.PointType;
        // 仮ポイントの場合はオペレーター注文変更を格納
        model.PointIncKbn = userPoint.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP ? Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_ORDER : userPoint.PointIncKbn;
        model.Point = adjustPoint;

        // ポイントマスタのポイント有効期限設定によって動作を変える
        // 注文日を元に生成
        var orderDate = DateTime.Parse(order.OrderDate);

        model.PointExp = master.PointExpKbn == Constants.FLG_POINT_POINT_EXP_KBN_VALID
            ? (DateTime?)new DateTime(orderDate.Year, orderDate.Month, orderDate.Day, 23, 59, 59, 997).AddYears(1)
            : null;

        // 仮ポイントの場合は注文IDを設定
        model.Kbn1 = userPoint.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP ? order.OrderId : string.Empty;
        model.Kbn2 = string.Empty;
        model.Kbn3 = string.Empty;
        model.Kbn4 = string.Empty;
        model.Kbn5 = string.Empty;
        model.LastChanged = this.LoginOperatorName;

        // ユーザーポイント登録
        var updated = sv.RegisterUserPoint(model, updateHistoryAction, accessor);

        // 登録件数確認
        if (updated == false) return false;

        // ユーザーのポイント情報
        var userPoints = sv.GetUserPoint(model.UserId, string.Empty, accessor);

        // 履歴情報を作る
        UserPointHistoryModel userPointHistoryModel = new UserPointHistoryModel();
        userPointHistoryModel.UserId = model.UserId;
        userPointHistoryModel.DeptId = model.DeptId;
        userPointHistoryModel.PointRuleId = model.PointRuleId;
        userPointHistoryModel.PointRuleKbn = model.PointRuleKbn;
        userPointHistoryModel.PointKbn = model.PointKbn;
        userPointHistoryModel.PointType = model.PointType;
        userPointHistoryModel.PointIncKbn = model.PointIncKbn;
        userPointHistoryModel.PointInc = model.Point;
		userPointHistoryModel.PointExpExtend = rule.PointExpExtend;

        // 有効期限の最大値を使う
        userPointHistoryModel.UserPointExp = userPoints.Max(x => x.PointExp);

        userPointHistoryModel.Kbn1 = order.OrderId;
        userPointHistoryModel.Kbn2 = string.Empty;
        userPointHistoryModel.Kbn3 = string.Empty;
        userPointHistoryModel.Kbn4 = string.Empty;
        userPointHistoryModel.Kbn5 = string.Empty;
        userPointHistoryModel.Memo = string.Empty;
        userPointHistoryModel.LastChanged = this.LoginOperatorName;

        sv.RegisterHistory(userPointHistoryModel, accessor);

        // 登録できていれば成功
        return true;
    }

	/// <summary>
	/// ユーザポイント情報更新
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="upUserPoint">ユーザポイント情報</param>
	/// <param name="dPoint">ポイント</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	protected bool UpdateUserPoint(
		OrderInput order,
		UserPointModel upUserPoint,
		decimal dPoint,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor sqlAccessor)
	{
		// ユーザーポイント情報の本ポイントが期限切れの場合、ユーザーポイント情報は更新しない
		if (upUserPoint == null)
		{
			// ログを残す
			var errorMessages = string.Format(
				"ご利用頂いているポイントは有効期限が既に切れているため、戻すことができませんでした。注文ID：{0}、失効ポイント：{1}",
				order.OrderId,
				dPoint);
			AppLogger.WriteError(errorMessages);
			// 更新しないことを正とし、処理結果はtrueで返す
			return true;
		}
		var sv = new PointService();

		var rule = sv.GetPointRule(upUserPoint.DeptId, upUserPoint.PointRuleId);

		// 更新用のモデル
		var model = sv.GetUserPoint(order.UserId, string.Empty, sqlAccessor)
			.First(x => (x.PointKbnNo == upUserPoint.PointKbnNo));
		
		// 更新する値
		model.Point += dPoint;
		model.LastChanged = this.LoginOperatorName;

		// 更新
		var cnt = sv.UpdateUserPoint(model, updateHistoryAction, sqlAccessor);
		if (cnt == 0) return false;

		// 履歴情報を作る
		cnt = sv.RegisterHistory(
			new UserPointHistoryModel
			{
				UserId = model.UserId,
				DeptId = model.DeptId,
				PointRuleId = model.PointRuleId,
				PointRuleKbn = model.PointRuleKbn,
				PointKbn = model.PointKbn,
				PointType = model.PointType,
				PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_ORDER,
				PointInc = dPoint,
		        PointExpExtend = rule.PointExpExtend,
				UserPointExp = model.PointExp,
				Kbn1 = order.OrderId,
				Kbn2 = string.Empty,
				Kbn3 = string.Empty,
				Kbn4 = string.Empty,
				Kbn5 = string.Empty,
				Memo = string.Empty,
				LastChanged = this.LoginOperatorName,
				// 復元処理の対象にはしない
				RestoredFlg = Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_RESTORED,
			},
			sqlAccessor);

		// 登録できていれば成功
		return cnt > 0;
	}

	#endregion

	/// <summary>
	/// 注文督促ステータス更新ステートメント取得
	/// </summary>
	/// <param name="strStatus">ステータス値</param>
	/// <returns>ステータス更新ステートメント</returns>
	protected string GetUpdateOrderDemandStatusStatement(string strStatus)
	{
		// 督促なし
		if (strStatus == Constants.FLG_ORDER_DEMAND_STATUS_LEVEL0)
		{
			return "UpdateOrderDemandStatusUnKnown";
		}
		// 督促レベル１～
		else if ((strStatus == Constants.FLG_ORDER_DEMAND_STATUS_LEVEL1)
			|| (strStatus == Constants.FLG_ORDER_DEMAND_STATUS_LEVEL2))
		{
			return "UpdateOrderDemandStatusLevel";
		}

		return null;
	}

	/// <summary>
	/// Create the extend status list from data view.
	/// </summary>
	/// <param name="extendStatusData">The extend status data.</param>
	/// <param name="masterData">The master data.</param>
	/// <returns>The extend status list</returns>
	public static List<ExtendStatus> CreateExtendStatusListFromDataView(DataView extendStatusData, Hashtable masterData)
	{
		var extendStatusList = new List<ExtendStatus>();

		foreach (DataRowView extendRow in extendStatusData)
		{
			string extendNo = extendRow[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString();
			var extendName = (string)extendRow[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME];
			var extendParam = (string)masterData[FIELD_ORDER_EXTEND_STATUS + extendNo];
			var extendDateTime = DateTime.Now;
			if (DateTime.TryParse(StringUtility.ToEmpty(masterData[FIELD_ORDER_EXTEND_STATUS_DATE + extendNo]), out extendDateTime) == false)
			{
				if (DateTime.TryParse(StringUtility.ToEmpty(masterData[FIELD_ORDER_EXTEND_STATUS_DATE + extendNo]), out extendDateTime) == false)
				{
					extendDateTime = DateTime.Now;
				}
			}
			extendStatusList.Add(new ExtendStatus(extendNo, extendName, extendParam, extendDateTime));
		}

		return extendStatusList;
	}

	/// <summary>	
	/// データバインド用注文一覧遷移URL作成
	/// </summary>
	/// <param name="searchParam">検索パラメタ（nullの場合はパラメタなし）</param>
	/// <param name="addPageNo">ページ番号付きのURLを生成するか</param>
	/// <returns>注文一覧遷移URL</returns>
	protected string CreateOrderListUrl(Hashtable searchParam, bool addPageNo)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_LIST);
		if (searchParam == null) return urlCreator.CreateUrl();

		urlCreator.AddParam(Constants.REQUEST_KEY_ORDER_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_USER_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_USER_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_NAME, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_NAME])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR])
			.AddParam(Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_ZIP, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_ZIP])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_ADDR, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_ADDR])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_TEL, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_TEL])
			.AddParam(Constants.REQUEST_KEY_SORT_KBN, (string)searchParam[Constants.REQUEST_KEY_SORT_KBN])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_KBN, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_KBN])
			.AddParam(Constants.REQUEST_KEY_ORDER_CARD_TRAN_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_CARD_TRAN_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_PAYMENT_ORDER_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_PAYMENT_ORDER_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_KBN, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_KBN])
			.AddParam(Constants.REQUEST_KEY_ORDER_DEMAND_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_DEMAND_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_NO, (string)searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_NO])
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_EXTEND_STATUS_NO, (string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_EXTEND_STATUS_NO])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_KBN, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_KBN])
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_NONE, (string)searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_NONE])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_KBN, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_KBN])
			.AddParam(Constants.REQUEST_KEY_ORDER_SEPARATE_ESTIMATES_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_SEPARATE_ESTIMATES_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_STOCKRESERVED_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_STOCKRESERVED_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_SHIPPED_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPED_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_CHECK_NO, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_CHECK_NO])
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPED_CHANGED_KBN, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPED_CHANGED_KBN])
			.AddParam(Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE, (string)searchParam[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE])
			.AddParam(Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_KBN, (string)searchParam[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_KBN])
			.AddParam(Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_REASON_KBN, (string)searchParam[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_REASON_KBN])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_RETURN_EXCHANGE_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_RETURN_EXCHANGE_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_REPAYMENT_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_REPAYMENT_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_MALL_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_MALL_ID])
			.AddParam(Constants.REQUEST_KEY_REATURN_EXCHANGE_REPAYMENT_UPDATE_DATE_STATUS, (string)searchParam[Constants.REQUEST_KEY_REATURN_EXCHANGE_REPAYMENT_UPDATE_DATE_STATUS])
			.AddParam(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_FROM, (string)searchParam[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_FROM])
			.AddParam(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_FROM, (string)searchParam[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_FROM])
			.AddParam(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_TO, (string)searchParam[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_TO])
			.AddParam(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_TO, (string)searchParam[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_TO])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, (string)searchParam[Constants.REQUEST_KEY_PRODUCT_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_NAME, (string)searchParam[Constants.REQUEST_KEY_PRODUCT_NAME])
			.AddParam(Constants.REQUEST_KEY_ORDER_SETPROMOTION_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_SETPROMOTION_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_NOVELTY_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_NOVELTY_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_RECOMMEND_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_RECOMMEND_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_FROM, (string)searchParam[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_FROM])
			.AddParam(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_TO, (string)searchParam[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_TO])
			.AddParam(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_FROM, (string)searchParam[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_FROM])
			.AddParam(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_TO, (string)searchParam[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_TO])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_COUNT_FROM, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_COUNT_FROM])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_COUNT_TO, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_COUNT_TO])
			.AddParam(Constants.REQUEST_KEY_ORDER_MEMO_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_MEMO_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_RELATION_MEMO_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_RELATION_MEMO_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_GIFT_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_GIFT_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_DIGITAL_CONTENTS_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_DIGITAL_CONTENTS_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_COMPANY_NAME, (string)searchParam[Constants.REQUEST_KEY_ORDER_COMPANY_NAME])
			.AddParam(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID, (string)searchParam[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID])
			.AddParam(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE, (string)searchParam[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE])
			.AddParam(Constants.REQUEST_KEY_USER_USER_MEMO, (string)searchParam[Constants.REQUEST_KEY_USER_USER_MEMO])
			.AddParam(Constants.REQUEST_KEY_USER_USER_MEMO_FLG, (string)searchParam[Constants.REQUEST_KEY_USER_USER_MEMO_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_TEXTS, (string)searchParam[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_TEXTS])
			.AddParam(Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_NONE, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_NONE]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_NONE, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_NONE])
			.AddParam(Constants.REQUEST_KEY_ORDER_MEMO, (string)searchParam[Constants.REQUEST_KEY_ORDER_MEMO])
			.AddParam(Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO, (string)searchParam[Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO])
			.AddParam(Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO, (string)searchParam[Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO])
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO])
			.AddParam(Constants.REQUEST_KEY_ORDER_RELATION_MEMO, (string)searchParam[Constants.REQUEST_KEY_ORDER_RELATION_MEMO])
			.AddParam(Constants.REQUEST_KEY_ORDER_ANOTHER_SHIPPING_FLAG, (string)searchParam[Constants.REQUEST_KEY_ORDER_ANOTHER_SHIPPING_FLAG])
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_NAME, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_NAME])
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_NAME_KANA, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_NAME_KANA])
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_ZIP, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_ZIP])
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_ADDR, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_ADDR])
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_TEL1, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_TEL1])
			.AddParam(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID, (string)searchParam[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID])
			.AddParam(Constants.REQUEST_KEY_COUPON_CODE, (string)searchParam[Constants.REQUEST_KEY_COUPON_CODE])
			.AddParam(Constants.REQUEST_KEY_COUPON_NAME, (string)searchParam[Constants.REQUEST_KEY_COUPON_NAME])
			.AddParam(Constants.REQUEST_KEY_ORDER_ADVCODE_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_ADVCODE_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_ADVCODE, (string)searchParam[Constants.REQUEST_KEY_ORDER_ADVCODE])
			.AddParam(Constants.REQUEST_KEY_ORDERITEM_PRODUCT_BUNDLE_ID, (string)searchParam[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_BUNDLE_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTERNAL_ORDER_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_ORDER_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTERNAL_IMPORT_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_IMPORT_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_MALL_LINK_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_MALL_LINK_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_METHOD, (string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_METHOD])
			.AddParam(Constants.REQUEST_KEY_ORDER_RECEIPT_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_RECEIPT_FLG])
			.AddParam(Constants.REQUEST_KEY_ORDER_INVOICE_BUNDLE_FLG, (string)searchParam[Constants.REQUEST_KEY_ORDER_INVOICE_BUNDLE_FLG])
			.AddParam(Constants.REQUEST_KEY_TW_INVOICE_NO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_TW_INVOICE_NO]))
			.AddParam(Constants.REQUEST_KEY_TW_INVOICE_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_TW_INVOICE_STATUS]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_PREFECTURE, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_PREFECTURE]))
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_NAME, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_NAME]))
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_FLG, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_FLG]))
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TYPE, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TYPE]))
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM]))
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS_CODE, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS_CODE]))
			.AddParam(Constants.REQUEST_KEY_ORDER_SHIPPING_CURRENT_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_CURRENT_STATUS]))
			.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, (string)searchParam[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID])
			.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_FROM, (string)searchParam[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_FROM])
			.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_TO, (string)searchParam[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_TO])
			.AddParam(Constants.REQUEST_KEY_STORE_PICKUP_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_STORE_PICKUP_STATUS]));

		// ページ番号付き？
		if (addPageNo)
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_PAGE_NO]));
		}

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// Store pick up order list
	/// </summary>
	/// <param name="searchParam">Search param</param>
	/// <param name="addPageNo">Add page no</param>
	/// <returns>Url search order</returns>
	protected UrlCreator StorePickUpOrderList(Hashtable searchParam, bool addPageNo)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_STOREPICKUP_ORDER_LIST);
		if (searchParam == null) return urlCreator;

		urlCreator.AddParam(Constants.REQUEST_KEY_ORDER_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_ORDER_STATUS])
			.AddParam(Constants.REQUEST_KEY_SORT_KBN, (string)searchParam[Constants.REQUEST_KEY_SORT_KBN])
			.AddParam(Constants.REQUEST_KEY_ORDER_USER_ID, (string)searchParam[Constants.REQUEST_KEY_ORDER_USER_ID])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_NAME, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_NAME])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, (string)searchParam[Constants.REQUEST_KEY_PRODUCT_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_NAME, (string)searchParam[Constants.REQUEST_KEY_PRODUCT_NAME])
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_TEL, (string)searchParam[Constants.REQUEST_KEY_ORDER_OWNER_TEL])
			.AddParam(Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID, (string)searchParam[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID])
			.AddParam(Constants.REQUEST_KEY_REALSHOP_NAME, (string)searchParam[Constants.REQUEST_KEY_REALSHOP_NAME])
			.AddParam(Constants.REQUEST_KEY_ORDER_STOREPICKUP_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_STOREPICKUP_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS, (string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS])
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM, (string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM])
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO, (string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO])
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM, (string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM])
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO, (string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO]);

		// ページ番号付き？
		if (addPageNo)
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_PAGE_NO]));
		}

		return urlCreator;
	}

	/// <summary>
	/// 決済情報取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strPaymentId">決済種別ID</param>
	/// <returns>決済手数料情報</returns>
	/// <remarks>基本的に取得できるはずなのでDataRowView取得としている</remarks>
	public static DataRowView GetPayment(string strShopId, string strPaymentId)
	{
		return OrderCommon.GetPayment(strShopId, strPaymentId);
	}

	/// <summary>
	/// Get products limited payment message
	/// </summary>
	/// <param name="shopId">The current shop Id</param>
	/// <param name="productList">List Of products</param>
	/// <returns> Products have limited payment message </returns>
	protected string GetProductsLimitedPaymentMessage(string shopId, List<KeyValuePair<string, string>> productList)
	{
		if (productList.Count == 0) return string.Empty;

		var productService = new ProductService();

		var productsLimitedPayment = productList
			.Select(product => productService.GetProductVariation(shopId, product.Key, product.Value, string.Empty))
			.Where(product => ((product != null) && (string.IsNullOrEmpty(product.LimitedPaymentIds) == false)));

		var productNameList = new List<string>();
		var paymentnameList = new List<string>();
		foreach (var product in productsLimitedPayment)
		{
			var listLimitedPayments = OrderCommon.GetLimitedPayments(shopId, product.LimitedPaymentIds.Split(','))
				.Select(p => p.PaymentName)
				.ToList();

			if (listLimitedPayments.Count <= 0) continue;

			productNameList.Add(
				ProductCommon.CreateProductJointName(
					product.Name,
					product.VariationName1,
					product.VariationName2,
					product.VariationName3));
			paymentnameList.AddRange(listLimitedPayments);
		}

		var limitedPaymentMessages = ((productNameList.Any() == false) ?
			string.Empty :
			string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_LIMITED_PAYMENT), string.Join("、", productNameList.Distinct()), string.Join("」「", paymentnameList.Distinct())));

		return limitedPaymentMessages;
	}

	/// <summary>
	/// 別途見積りのためのメッセージ取得
	/// </summary>
	/// <param name="dvOrder"></param>
	/// <param name="IsPc"></param>
	/// <rereturns>PC,Mobileサイトに合わせたメッセージ</rereturns>
	private static string GetMessageForSeparateEstimate(DataView dvOrder, bool IsPc)
	{
		var message = (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED
			&& ((string)dvOrder[0][Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] == Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID))
				? IsPc
					? OrderCommon.GetShopShipping((string)dvOrder[0][Constants.FIELD_ORDER_SHOP_ID], (string)dvOrder[0][Constants.FIELD_ORDER_SHIPPING_ID]).ShippingPriceSeparateEstimatesMessage
					: OrderCommon.GetShopShipping((string)dvOrder[0][Constants.FIELD_ORDER_SHOP_ID], (string)dvOrder[0][Constants.FIELD_ORDER_SHIPPING_ID]).ShippingPriceSeparateEstimatesMessageMobile
				: string.Empty;
		return message;
	}

	/// <summary>
	/// 注文IDのセッション格納
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <remarks>ポップアップ時に更新後、親画面リロード後の注文情報に印を付けるために使用する。</remarks>
	protected void SetSessionOrderId(string strOrderId)
	{
		if (this.IsPopUp) Session["updated_order_id"] = strOrderId;
	}

	/// <summary>
	/// 注文IDのセッション取得
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <remarks>注文IDをセッションから取得後、セッション情報を空にする。</remarks>
	protected string GetSessionOrderId()
	{
		string strOrderId = (string)Session["updated_order_id"];
		Session["updated_order_id"] = null;
		return strOrderId;
	}

	/// <summary>
	/// 注文セットプロモーションドロップダウンリスト用ListItemCollection取得
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <returns>注文セットプロモーションドロップダウンリスト用ListItemCollection</returns>
	protected ListItemCollection GetOrderSetPromotionList(OrderInput order)
	{
		var orderSetPromotionList = new ListItemCollection();
		orderSetPromotionList.Add(new ListItem("", ""));

		orderSetPromotionList.AddRange(
			order.SetPromotions.OrderBy(sp => sp.OrderSetpromotionNo)
			.Select(sp =>
				new ListItem(sp.OrderSetpromotionNo + "：" + sp.SetpromotionName, sp.OrderSetpromotionNo)).ToArray()
			);

		return orderSetPromotionList;
	}
	/// <summary>
	/// 注文セットプロモーションドロップダウンリスト用ListItemCollection取得
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <returns>注文セットプロモーションドロップダウンリスト用ListItemCollection</returns>
	protected ListItemCollection GetOrderSetPromotionList(Order order)
	{
		var orderSetPromotionList = new ListItemCollection();
		orderSetPromotionList.Add(new ListItem("", ""));

		orderSetPromotionList.AddRange(
			order.SetPromotions.OrderBy(sp => sp.OrderSetPromotionNo)
			.Select(sp =>
				new ListItem(sp.OrderSetPromotionNo + "：" + sp.SetPromotionName, sp.OrderSetPromotionNo)).ToArray()
			);

		return orderSetPromotionList;
	}

	/// <summary>
	/// Get error payment
	/// </summary>
	/// <param name="shopId">ShopId</param>
	/// <param name="orderPaymentKbn">OrderPaymentKbn</param>
	/// <param name="orderPriceTotal">OrderPriceTotal</param>
	/// <param name="orderPointUseYen">OrderPointUseYen</param>
	/// <returns>Error message</returns>
	protected static string CheckPaymentPriceEnabled(string shopId, string orderPaymentKbn, decimal orderPriceTotal, decimal orderPointUseYen)
	{
		StringBuilder errorMessages = new StringBuilder();

		DataRowView payment = GetPayment(shopId, orderPaymentKbn);
		if (payment == null)
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_DELETED_ERROR));

			return errorMessages.ToString();
		}

		if ((payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN] != DBNull.Value)
			&& ((orderPriceTotal) < (decimal)payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN]))
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_USBABLE_PRICE_ERROR).Replace("@@ 1 @@", payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN].ToPriceString(true) + " 以上"));
		}

		if ((payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX] != DBNull.Value)
			&& (orderPriceTotal > (decimal)payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX]))
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_USBABLE_PRICE_ERROR).Replace("@@ 1 @@", payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX].ToPriceString(true) + " 以下"));
		}

		return errorMessages.ToString();
	}

	/// <summary>
	/// 定期商品チェックと商品定期購入フラグ整合性チェック
	/// </summary>
	/// <param name="fixedPurchaseProductFlg">定期商品フラグ ※注文商品情報</param>
	/// <param name="productVariation">商品情報</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckFixedPurchaseProduct(string fixedPurchaseProductFlg, DataRowView productVariation)
	{
		var result = "";

		var productName = ProductCommon.CreateProductJointName(productVariation);
		var fixedPurchaseFlg = (string)productVariation[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
		var fixedPurchaseProductFlgOn =
			(fixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON);

		if ((fixedPurchaseProductFlgOn == false)
			&& (fixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY))
		{
			result = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_CAN_NOT_BE_SET_NOMAL_ERROR).Replace("@@ 1 @@", productName);
		}
		else if (fixedPurchaseProductFlgOn
			&& (fixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID))
		{
			result = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_CAN_NOT_BE_SET_FIXED_PURCHASE_ERROR).Replace("@@ 1 @@", productName);
		}

		return result;
	}

	/// <summary>
	/// 商品名への定期購入接頭辞追加・削除
	/// </summary>
	/// <param name="productName">商品名</param>
	/// <param name="isFixedPurchaseProduct">作成する名称は定期ありのものか</param>
	/// <param name="isSubscriptionBoxProduct">作成する名称は頒布会ありのものか</param>
	protected static string CreateFixedPurchaseProductName(string productName,
		bool isFixedPurchaseProduct,
		bool isSubscriptionBoxProduct)
	{
		if (string.IsNullOrEmpty(Constants.PRODUCT_FIXED_PURCHASE_STRING)
			|| string.IsNullOrEmpty(Constants.PRODUCT_SUBSCRIPTION_BOX_STRING)) return productName;

		// （定期 OR 頒布会） AND 商品名の先頭に定期/頒布会商品接頭辞が付いていない
		var isContainsFixedPurchaseString = productName.StartsWith(Constants.PRODUCT_FIXED_PURCHASE_STRING);
		var isContainsSubscriptionBoxString = productName.StartsWith(Constants.PRODUCT_SUBSCRIPTION_BOX_STRING);
		if ((isFixedPurchaseProduct && (isContainsFixedPurchaseString == false) && (isSubscriptionBoxProduct == false))
			|| (isSubscriptionBoxProduct && (isContainsSubscriptionBoxString == false)))
		{
			return string.Format("{0}{1}",
				isSubscriptionBoxProduct
					? Constants.PRODUCT_SUBSCRIPTION_BOX_STRING
					: Constants.PRODUCT_FIXED_PURCHASE_STRING,
				productName);
		}
		// 定期商品以外 AND 商品名の先頭に定期商品接頭辞が付いている
		if (((isFixedPurchaseProduct == false) && isContainsFixedPurchaseString)
			|| ((isSubscriptionBoxProduct == false) && isContainsSubscriptionBoxString))
		{
			return productName.Remove(
				0,
				isSubscriptionBoxProduct
					? Constants.PRODUCT_SUBSCRIPTION_BOX_STRING.Length
					: Constants.PRODUCT_FIXED_PURCHASE_STRING.Length);
		}

		return productName;
	}

	/// <summary>
	/// 商品同梱IDの表示値作成
	/// </summary>
	/// <param name="productBundleId">商品同梱ID</param>
	/// <param name="shopOperator">店舗オペレータ</param>
	/// <returns>表示値</returns>
	public static string GetProductBundleIdDisplayValue(string productBundleId, ShopOperatorModel shopOperator)
	{
		if (string.IsNullOrEmpty(productBundleId)) return HttpUtility.HtmlEncode("-");

		// 権限チェック
		if (MenuUtility.HasAuthorityEc(shopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCTBUNDLE_REGISTER) == false)
		{
			return HttpUtility.HtmlEncode(productBundleId);
		}

		var url = CreateProductBundleRegisterPageUrl(productBundleId);

		return string.Format(
			"<a href=\"\" onclick=\"javascript:open_window('{0}','productbundlecontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');\">{1}</a>",
			HttpUtility.HtmlEncode(url),
			HttpUtility.HtmlEncode(productBundleId));
	}

	/// <summary>
	/// 商品同梱設定画面へのリンク生成
	/// </summary>
	/// <param name="productBundleId">商品同梱ID</param>
	/// <returns>リンク</returns>
	private static string CreateProductBundleRegisterPageUrl(string productBundleId)
	{
		var urlCreator =
			new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTBUNDLE_REGISTER)
			.AddParam(Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID, productBundleId)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
				.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// 配送間隔日・月のチェック
	/// </summary>
	/// <param name="items">アイテムハッシュテーブル</param>
	/// <param name="selectedValue">選択中の値</param>
	/// <param name="fieldName">チェックする項目名</param>
	/// <returns>選択している値が規定の配送パターンかどうか</returns>
	protected static bool CheckSpecificIntervalMonthAndDay(List<Hashtable> items, string selectedValue, string fieldName)
	{
		if (items.Count < 1) return true;

		// 利用不可配送間隔日/月を取得
		var limitedInterval = items
			.Where(item => string.IsNullOrEmpty((string)item[fieldName]) == false)
			.Select(item => (string)item[fieldName]).Distinct().ToList();

		var result = limitedInterval.All(
			item => item.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(selectedValue) == false);

		return result;
	}

	/// <summary>
	/// 外部決済連携情報セット（注文分割：返品／交換登録、又は、途中交換注文編集の場合等）
	/// </summary>
	/// <param name="reauth">再与信実行インスタンス</param>
	/// <param name="reauthResult">再与信結果</param>
	/// <param name="orderNew">（返品・交換・編集後の）注文情報</param>
	/// <param name="orderOld">（元注文・直前の）注文情報</param>
	/// <param name="isReturnAllItems">全返品フラグ</param>
	/// <returns>外部決済連携実行したか？</returns>
	public bool SetExternalPaymentInfoReauthSplit(
		ReauthExecuter reauth,
		ReauthResult reauthResult,
		OrderModel orderNew,
		OrderModel orderOld,
		bool isReturnAllItems = false)
	{
		// 外部決済連携をしていない？
		if (reauth.HasAnyAction == false)
		{
			// 返品・交換注文
			// クレジットカード OR ヤマト後払い：未決済
			// 上記以外：連携なし
			if (OrderCommon.CheckCanPaymentReauth(orderNew.OrderPaymentKbn))
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				this.RegisterCardTranId = orderOld.CardTranId;
				this.RegisterPaymentOrderId = orderOld.PaymentOrderId;
			}
			else
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
			}
			return false;
		}

		// 元注文＆返品・交換注文に決済メモセット
		if (string.IsNullOrEmpty(reauthResult.PaymentMemo) == false)
		{
			this.RegisterPaymentMemo = reauthResult.PaymentMemoForReturnExchangeOrder;
			this.UpdatePaymentMemoForOrderOld = reauthResult.PaymentMemoForOrderOld;
		}

		// 再与信・減額・請求書再発行もつ場合に外部決済情報更新
		if (reauth.HasReauthOrReduceOrReprint)
		{
			// 返品・交換注文
			// 決済取引ID、決済注文ID、外部決済ステータス、外部決済与信日時をセット
			this.RegisterCardTranId = reauthResult.CardTranId;
			this.RegisterPaymentOrderId = reauthResult.PaymentOrderId;
			this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			this.RegisterExternalPaymentAuthDate = reauth.GetUpdateReauthDate(
				orderOld.ExternalPaymentAuthDate,
				orderOld.OrderPaymentKbn,
				orderNew.OrderPaymentKbn).ToString();
		}

		// 失敗？
		this.ReturnOrderReauthErrorMessages = null;
		if ((reauthResult.ResultDetail != ReauthResult.ResultDetailTypes.Success)
			&& (orderNew.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
		{
			// 登録完了後にエラーメッセージを表示するため、セッションにセット
			this.ReturnOrderReauthErrorMessages =
				reauthResult.ErrorMessages
				+ "\r\n" + WebMessages.GetMessages(WebMessages.ERRMSG_EXTERNAL_PAYMENT_CANCEL_FAILED);
		}

		// 失敗し、同額で与信を取り直している場合
		if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSameAmount)
		{
			this.UpdatePaymentMemoForOrderOld = reauthResult.PaymentMemo;
			return false;
		}

		// ▽ 返品注文の外部決済ステータスをセット ▽
		// 返品・交換注文がクレジットカード？
		if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN)))
		{
			// 売上確定に失敗？
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
			{
				// 全返品、一部返品：売上確定エラー
				// 交換：与信済み
				this.RegisterExternalPaymentStatus = (orderNew.IsReturnOrder && Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}
			// 元注文がヤマト後払い AND 入金済み？
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
			&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が台湾後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				// 返品注文 全返品：未決済
				this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0)) ? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED : this.RegisterExternalPaymentStatus;
				// 返品注文 一部返品：売上確定済み
				this.RegisterExternalPaymentStatus = reauth.HasSales ? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP : this.RegisterExternalPaymentStatus;
			}
		}
		// 返品・交換注文がヤマト後払い？
		else if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN))
		{
			// 元注文がヤマト後払い AND 入金済み？
			if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が台湾後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				// 返品注文
				// 全返品：未決済、一部返品：与信済み
				this.RegisterExternalPaymentStatus =
					isReturnAllItems && (orderNew.LastBilledAmount == 0)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;
			}
		}
		else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
		{
			if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				this.RegisterExternalPaymentStatus =
					isReturnAllItems && (orderNew.LastBilledAmount == 0)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;
			}
		}
		else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
		{
			if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				this.RegisterExternalPaymentStatus =
					isReturnAllItems && (orderNew.LastBilledAmount == 0)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;
			}
		}
		// 返品・交換注文が台湾後払い
		else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
		{
			// 元注文がヤマト後払い AND 入金済み？
			if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が台湾後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				// 返品注文
				// 全返品：未決済、一部返品：与信済み
				this.RegisterExternalPaymentStatus =
					isReturnAllItems && (orderNew.LastBilledAmount == 0)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: this.RegisterExternalPaymentStatus;
			}
		}
		// 返品・交換注文がAmazonPay？
		else if (OrderCommon.IsAmazonPayment(orderNew.OrderPaymentKbn))
		{
			// 決済取引IDをセット
			this.RegisterCardTranId = reauthResult.CardTranId;

			// 売上確定に失敗？
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
			{
				// 全返品、一部返品：売上確定エラー
				// 交換：与信済み
				this.RegisterExternalPaymentStatus = (orderNew.IsReturnOrder && Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}
			// 元注文がヤマト後払い AND 入金済み？
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が台湾後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				// 返品注文 全返品
				// 最終請求金額が0円：未決済
				// 最終請求金額が１円以上 かつ 返品時自動売上確定：売上確定済み
				// それ以外：前処理のステータスを引き継ぐ
				this.RegisterExternalPaymentStatus =
					(isReturnAllItems && (orderNew.LastBilledAmount == 0))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: Constants.PAYMENT_AMAZON_PAYMENT_RETURN_AUTOSALES_ENABLED
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
							: this.RegisterExternalPaymentStatus;

				// 返品注文 一部返品：売上確定済み
				if (reauth.HasSales)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
				}
			}
		}
		// For return exchange order of paidy pay
		else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
		{
			// 決済取引IDをセット
			this.RegisterCardTranId = reauthResult.CardTranId;

			// 売上確定に失敗？
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
			{
				// 全返品、一部返品：売上確定エラー
				// 交換：与信済み
				this.RegisterExternalPaymentStatus = (orderNew.IsReturnOrder && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}
			// 元注文がヤマト後払い AND 入金済み？
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が台湾後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				// 返品注文 全返品：未決済
				this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;

				// 返品注文 一部返品：売上確定済み
				if (reauth.HasSales)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
				}
			}
		}
		// 返品・交換注文がLINE Pay？
		else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
		{
			// 決済取引IDをセット
			this.RegisterCardTranId = orderNew.CardTranId;

			// 売上確定に失敗？
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
			{
				// 全返品、一部返品：売上確定エラー
				// 交換：与信済み
				this.RegisterExternalPaymentStatus =
					(orderNew.IsReturnOrder && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}
			// 元注文がヤマト後払い AND 入金済み？
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が台湾後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				// 返品注文 全返品：未決済
				this.RegisterExternalPaymentStatus =
					(isReturnAllItems && (orderNew.LastBilledAmount == 0))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: this.RegisterExternalPaymentStatus;
				// 返品注文 一部返品：売上確定済み
				if (reauth.HasSales
					&& orderNew.IsExchangeOrder
					&& (orderNew.LastBilledAmount < orderOld.LastBilledAmount))
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
				}
			}
		}
		// NP After Pay
		else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
		{
			// 元注文がヤマト後払い AND 入金済み？
			if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が台湾後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus =
					(NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(orderOld.ExternalPaymentStatus)
						&& string.IsNullOrEmpty(reauthResult.ApiErrorMessages))
					? orderOld.ExternalPaymentStatus
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				// 返品注文
				// 全返品：未決済、一部返品：与信済み
				this.RegisterExternalPaymentStatus =
					(isReturnAllItems && (orderNew.LastBilledAmount == 0))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: this.RegisterExternalPaymentStatus;
			}
		}
		// For return exchange order of EcPay
		else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
		{
			// 決済取引IDをセット
			this.RegisterCardTranId = reauthResult.CardTranId;

			// 売上確定に失敗？
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
			{
				// 全返品、一部返品：売上確定エラー
				// 交換：与信済み
				this.RegisterExternalPaymentStatus = (orderNew.IsReturnOrder)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}
			// 元注文がヤマト後払い AND 入金済み？
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が台湾後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				// 返品注文 全返品：未決済
				this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;

				// 返品注文 一部返品：売上確定済み
				if (reauth.HasSales)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
				}
			}
		}
		// For return exchange order of Newebpay
		else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
		{
			// 決済取引IDをセット
			this.RegisterCardTranId = reauthResult.CardTranId;

			// 売上確定に失敗？
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
			{
				// 全返品、一部返品：売上確定エラー
				// 交換：与信済み
				this.RegisterExternalPaymentStatus = orderNew.IsReturnOrder
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}
			else
			{
				// 返品注文 全返品：未決済
				this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;

				// 返品注文 一部返品：売上確定済み
				if (reauth.HasSales)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
				}
			}
		}
		// For return exchange order of Paypay
		else if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			&& ((Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO)
				|| (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans)))
		{
			// 決済取引IDをセット
			this.RegisterCardTranId = reauthResult.CardTranId;

			// 売上確定に失敗？
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
			{
				// 全返品、一部返品：売上確定エラー
				// 交換：与信済み
				this.RegisterExternalPaymentStatus = (orderNew.IsReturnOrder)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}
			// 元注文がヤマト後払い AND 入金済み？
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が台湾後払い？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				// 返品注文
				// 全返品、一部返品：未決済
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				// 返品注文 全返品：未決済
				this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;

				// 返品注文 一部返品：売上確定済み
				if (reauth.HasSales)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
				}
			}
		}
		// 返品・交換注文が代引き OR 決済無し
		else
		{
			// 返品・交換注文：連携なし
			this.UpdateExternalPaymentStatusForOrderOld =
				Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
		}

		// ▽ 元注文の外部決済ステータスをセット ▽
		//　元注文がクレジットカード？
		if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& reauth.HasCancel)
		{
			// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
			this.UpdateExternalPaymentStatusForOrderOld =
				((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
					|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
				: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
					: orderOld.ExternalPaymentStatus);
		}
		// 元注文がヤマト後払い？
		else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN))
		{
			// 元注文がヤマト後払い AND 入金済み？
			if (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP)
			{
				// 元注文：入金済み
				this.UpdateExternalPaymentStatusForOrderOld =
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP;
			}
			else if (reauth.HasCancel)
			{
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
				((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
					|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
				: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
					: orderOld.ExternalPaymentStatus);
			}
		}
		else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE))
		{
			if (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP)
			{
				// 元注文：入金済み
				this.UpdateExternalPaymentStatusForOrderOld =
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP;
			}
			else if (reauth.HasCancel)
			{
				this.UpdateExternalPaymentStatusForOrderOld =
				((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
					|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
				: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
					: orderOld.ExternalPaymentStatus);
			}
		}
		// 元注文が台湾後払い
		else if (((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				|| (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT))
			&& (reauth.HasCancel))
		{
			if (reauth.HasCancel)
			{
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
				((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
					|| ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
				: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
					: orderOld.ExternalPaymentStatus);
			}
		}
		// 元注文がAmazonPay？
		else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
			|| (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT))
		{
			// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
			// 返金済みになるのは減額かつSuccessのときのみ
			// 更新しないのはそれ以外
			// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
			this.UpdateExternalPaymentStatusForOrderOld =
			((orderNew.LastBilledAmount <= 0) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
			? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
			: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
				: orderOld.ExternalPaymentStatus);
		}
		// 元注文がPaidy Pay
		else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
		{
			if (reauth.HasCancel)
			{
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
					((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus);
			}
			else
			{
				// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
				// 返金済みになるのは減額かつSuccessのときのみ
				// 更新しないのはそれ以外
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
					((orderNew.LastBilledAmount <= 0)
						&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus);
			}

			this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
			if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
				&& (orderNew.LastBilledAmount == 0))
			{
				this.UpdateOrderStatusForOrderOld = Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDER_CANCELED;
				this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED;
			}
		}
		// Original order is atone翌月払い
		else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
		{
			this.UpdateExternalPaymentStatusForOrderOld = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
			this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;

			// Sales Order After Created Order Return Order Exchange
			if (orderOld.OrderId != orderNew.OrderId)
			{
				var user = new UserService().Get(orderNew.UserId);
				var tokenId = ((user != null)
					? user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]
					: string.Empty);

				if (string.IsNullOrEmpty(tokenId) == false)
				{
					// If Order Not Sales Then Call Sales Api
					var orderApi = AtonePaymentApiFacade.GetPayment(orderNew.CardTranId);
					if (string.IsNullOrEmpty(orderApi.SalesSettledDatetime))
					{
						var capture = AtonePaymentApiFacade.CapturePayment(tokenId, orderNew.CardTranId);
						if (capture.IsSuccess)
						{
							this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
							this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
						}
					}
					else
					{
						this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
						this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					}
				}
			}
		}
		// Original order is aftee翌月払い
		else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
		{
			this.UpdateExternalPaymentStatusForOrderOld = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
			this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;

			// Sales Order After Created Order Return Order Exchange
			if (orderOld.OrderId != orderNew.OrderId)
			{
				var user = new UserService().Get(orderNew.UserId);
				var tokenId = ((user != null)
					? user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID]
					: string.Empty);

				if (string.IsNullOrEmpty(tokenId) == false)
				{
					// If Order Not Sales Then Call Sales Api
					var orderApi = AfteePaymentApiFacade.GetPayment(orderNew.CardTranId);
					if (string.IsNullOrEmpty(orderApi.SalesSettledDatetime))
					{
						var capture = AfteePaymentApiFacade.CapturePayment(tokenId, orderNew.CardTranId);
						if (capture.IsSuccess)
						{
							this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
							this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
						}
					}
					else
					{
						this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
						this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					}
				}
			}
		}
		// 元注文がLINE Pay？
		else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
		{
			if (reauth.HasCancel)
			{
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
					((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
							: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
								? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
								: orderOld.ExternalPaymentStatus);
			}
			else
			{
				// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
				// 返金済みになるのは減額かつSuccessのときのみ
				// 更新しないのはそれ以外
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
					((orderNew.LastBilledAmount <= 0)
						&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
							: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
								? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
								: orderOld.ExternalPaymentStatus);
			}

			if (reauth.HasSales)
			{
				this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
			}
		}
		// 元注文がNP後払い？
		else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			&& reauth.HasCancel)
		{
			this.UpdateExternalPaymentStatusForOrderOld =
				((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
					|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus;
		}
		// 元注文がEcPay
		else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
		{
			if (reauth.HasCancel)
			{
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
					((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus);
			}
			else
			{
				// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
				// 返金済みになるのは減額かつSuccessのときのみ
				// 更新しないのはそれ以外
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
					((orderNew.LastBilledAmount <= 0)
						&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus);
			}

			this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
		}
		// 元注文がNewebPay
		else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
		{
			if (reauth.HasCancel)
			{
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld = ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
					|| (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
						: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
							: orderOld.ExternalPaymentStatus);
			}
			else
			{
				// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
				// 返金済みになるのは減額かつSuccessのときのみ
				// 更新しないのはそれ以外
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld = ((orderNew.LastBilledAmount <= 0)
					&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
						: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
							: orderOld.ExternalPaymentStatus);
			}
		}
		// PayPay
		else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			&& ((Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO)
				|| (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans)))
		{
			if (reauth.HasCancel)
			{
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
					((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus);
			}
			else
			{
				// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
				// 返金済みになるのは減額かつSuccessのときのみ
				// 更新しないのはそれ以外
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
					((orderNew.LastBilledAmount <= 0)
						&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus);
			}

			this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
		}
		// その他（代引き OR 決済無し等）
		else
		{
			// 元注文：連携なし
			this.UpdateExternalPaymentStatusForOrderOld =
				Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
		}
		return true;
	}

	/// <summary>
	/// 外部決済連携情報セット（注文分割：返品／交換登録、又は、途中交換注文編集の場合等）
	/// </summary>
	/// <param name="reauthResult">再与信結果</param>
	public void SetExternalPaymentSession(ReauthResult reauthResult)
	{
		this.ReturnOrderReauthErrorMessages = null;
		if (reauthResult.ResultDetail != ReauthResult.ResultDetailTypes.Success)
		{
			// 登録完了後にエラーメッセージを表示するため、セッションにセット
			this.ReturnOrderReauthErrorMessages =
				reauthResult.ErrorMessages
				+ "\r\n" + WebMessages.GetMessages(WebMessages.ERRMSG_EXTERNAL_PAYMENT_CANCEL_FAILED);
		}
	}

	/// <summary>
	/// 受注情報一覧に表示する項目名のデータを適切なフォーマットに変換
	/// </summary>
	/// <param name="dispColumnName">表示項目名</param>
	/// <param name="dispData">変換前表示データ</param>
	/// <param name="shippingCompanyId">配送会社ID</param>
	/// <returns>変換後の表示データ</returns>
	public static string ConvertItemFormatForDisplayList(string dispColumnName, object dispData, string shippingCompanyId)
	{
		var result = dispData.ToString();

		switch (dispColumnName)
		{
			// サイト
			case Constants.FIELD_ORDER_MALL_ID:
				result = CreateSiteNameOnly(dispData.ToString(), dispColumnName);
				break;

			// ユーザー管理
			case Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID:
				result = UserManagementLevelUtility.GetUserManagementLevelName(dispData.ToString());
				break;

			// 注文ステータス
			case Constants.FIELD_ORDER_ORDER_STATUS:
				result = ValueText.GetValueText(Constants.TABLE_ORDER, dispColumnName, dispData);
				break;

			// 注文日時
			case Constants.FIELD_ORDER_ORDER_DATE:
				result = DateTimeUtility.ToStringForManager(
					dispData,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
				break;

			// 注文区分
			case Constants.FIELD_ORDER_ORDER_KBN:
				result = ValueText.GetValueText(Constants.TABLE_ORDER, dispColumnName, dispData);
				break;

			// 注文者区分
			case Constants.FIELD_ORDEROWNER_OWNER_KBN:
				result = ValueText.GetValueText(Constants.TABLE_ORDEROWNER, dispColumnName, dispData);
				break;

			// 入金ステータス
			case Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS:
				result = ValueText.GetValueText(Constants.TABLE_ORDER, dispColumnName, dispData);
				break;

			// 合計金額
			case Constants.FIELD_ORDER_ORDER_PRICE_TOTAL:
				result = CurrencyManager.ToPrice(dispData);
				break;

			// ギフト
			case Constants.FIELD_ORDER_GIFT_FLG:
				result = ((string)dispData == Constants.FLG_ORDER_GIFT_FLG_ON) ? "*" : "";
				break;

			// 注文メモ、管理メモ、配送メモ
			case Constants.FIELD_ORDER_MEMO:
			case Constants.FIELD_ORDER_MANAGEMENT_MEMO:
			case Constants.FIELD_ORDER_SHIPPING_MEMO:
				if (string.IsNullOrEmpty((string)dispData) == false) result = "*";
				break;

			// デジタルコンテンツ
			case Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG:
				result = ((string)dispData == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID) ? "*" : "";
				break;

			// 引当状況
			case Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS:
				result = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					FIELD_ORDER_ORDER_STOCKRESERVED_STATUS_LIST,
					dispData);
				break;

			// 出荷状況
			case Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS:
				result = ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FLG_ORDER_ORDER_STOCKRESERVED_STATUS_LIST,
					dispData);
				break;

			// 返品交換区分
			case Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN:
				result = ValueText.GetValueText(Constants.TABLE_ORDER, dispColumnName, dispData);
				break;

			// 返品交換受付日
			case Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE:
				result = DateTimeUtility.ToStringForManager(
					dispData,
					DateTimeUtility.FormatType.ShortDate2Letter);
				break;

			// 返品交換ステータス
			case Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS:
				result = ValueText.GetValueText(Constants.TABLE_ORDER, dispColumnName, dispData);
				break;

			// 返品ステータス
			case Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS:
				result = ValueText.GetValueText(Constants.TABLE_ORDER, dispColumnName, dispData);
				break;

			// 出荷予定日、配送希望日
			case Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE:
			case Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE:
				result = GetShippingDateText(dispData.ToString());
				break;

			// 配送希望時間帯
			case Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME:
				result = GetShippingTimeText(shippingCompanyId, dispData.ToString());
				break;

			// 定期購入回数(注文時点)、定期購入回数(出荷時点)、ユーザー購入回数（注文基準）
			case Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT:
			case Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT:
			case Constants.FIELD_ORDER_ORDER_COUNT_ORDER:
				result = (string.IsNullOrEmpty(dispData.ToString()) == false)
					? (string.Format(
						ValueText.GetValueText(Constants.TABLE_ORDER, "purchase_count_unit", "count_unit"),
						dispData))
					: "－";
				break;

			// 督促ステータス
			case Constants.FIELD_ORDER_DEMAND_STATUS:
				result = ValueText.GetValueText(Constants.TABLE_ORDER, dispColumnName, dispData);
				break;

			case Constants.FIELD_ORDER_STOREPICKUP_STATUS:
				result = (string.IsNullOrEmpty(StringUtility.ToEmpty(dispData)) == false)
					? ValueText.GetValueText(Constants.TABLE_ORDER, dispColumnName, dispData)
					: "－";
				break;

			// 拡張ステータス
			default:
				if (dispColumnName.Contains(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME))
				{
					result = ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.FLG_ORDER_EXTEND_STATUS_LIST,
						dispData);
				}
				break;
		}
		return result;
	}

	/// <summary>
	/// Set Can Delete To Order Item
	/// </summary>
	/// <param name="orderItems">Order item</param>
	/// <returns>Order items</returns>
	public static OrderItemInput[] SetCanDeleteToOrderItem(IEnumerable<OrderItemInput> orderItems)
	{
		var listItems = orderItems.ToList();
		var itemSetCount = listItems.Where(item => item.IsProductSet).GroupBy(item => new
			{
				SetId = item.ProductSetId,
				SetNo = item.ProductSetNo
			}).Count();
		var itemCount = listItems.Count(item => (item.IsProductSet == false));
		listItems.ForEach(item => item.CanDelete = ((itemCount + itemSetCount) > 1));

		return listItems.ToArray();
	}

	/// <summary>
	/// 元注文情報（元注文 or 最後の返品注文）更新
	/// </summary>
	/// <param name="orderOld">元注文</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transaction">トランザクション名</param>
	protected void ExecuteUpdateOrderOld(
		OrderModel orderOld,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor accessor)
	{
		// 決済メモ更新
		var orderService = new OrderService();
		if (string.IsNullOrEmpty(this.UpdatePaymentMemoForOrderOld) == false)
		{
			orderService.AddPaymentMemo(
				orderOld.OrderId,
				this.UpdatePaymentMemoForOrderOld,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		// 外部決済ステータス更新
		if (string.IsNullOrEmpty(this.UpdateExternalPaymentStatusForOrderOld) == false)
		{
			orderService.UpdateExternalPaymentInfo(
				orderOld.OrderId,
				this.UpdateExternalPaymentStatusForOrderOld,
				true,
				null,
				"",
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		// For case has update online payment status for order old
		if (string.IsNullOrEmpty(this.UpdateOnlinePaymentStatusForOrderOld) == false)
		{
			orderService.UpdateOnlinePaymentStatus(
				orderOld.OrderId,
				this.UpdateOnlinePaymentStatusForOrderOld,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		// For case has update order status for order old
		if (string.IsNullOrEmpty(this.UpdateOrderStatusForOrderOld) == false)
		{
			orderService.UpdateOrderStatus(
				orderOld.OrderId,
				this.UpdateOrderStatusForOrderOld,
				DateTime.Now,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		// 最終与信フラグ更新（更新履歴とともに？）
		orderService.UpdateLastAuthFlg(
			orderOld.OrderId,
			Constants.FLG_ORDER_LAST_AUTH_FLG_OFF,
			this.LoginOperatorName,
			updateHistoryAction,
			accessor);
	}

	/// <summary>
	/// 商品在庫チェック
	/// </summary>
	/// <param name="orderOld">変更前注文情報 なければnull</param>
	/// <param name="orderItems">注文商品情報</param>
	/// <returns>在庫エラー商品名リスト</returns>
	protected List<string> CheckProductStock(OrderInput orderOld, OrderItemInput[] orderItems)
	{
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			return CheckProductStock(orderOld, orderItems, accessor);
		}
	}
	/// <summary>
	/// 商品在庫チェック
	/// </summary>
	/// <param name="orderOld">変更前注文情報 なければnull</param>
	/// <param name="orderItems">注文商品情報</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>在庫エラー商品名リスト</returns>
	protected List<string> CheckProductStock(OrderInput orderOld, OrderItemInput[] orderItems, SqlAccessor accessor)
	{
		var errorProductNameList = new List<string>();

		// 変更前の在庫管理対象 商品別数量増減リスト作成
		var productStockAdjustListOld = new List<ProductStockAdjustCheck>();
		if (orderOld != null)
		{
			// 変更前の注文商品取得
			var orderItemsOld = orderOld.Shippings.SelectMany(s => s.Items).ToList();
			productStockAdjustListOld = CreateProductStockAdjustList(null, orderItemsOld, true, accessor);
		}

		// 変更前のリストをベースに変更後との商品別数量増減リストを作成
		var productStockAdjustList = CreateProductStockAdjustList(productStockAdjustListOld, orderItems.ToList(), false, accessor);

		foreach (var productStockAdjust in productStockAdjustList)
		{
			var productVariation = GetProductVariation(productStockAdjust.ShopId, productStockAdjust.ProductId, productStockAdjust.VariationId, string.Empty, string.Empty, accessor);

			// 「在庫０以下の場合、表示する。購入不可」以外の場合、在庫チェックをスキップ
			var stockManagementKbn = (string)productVariation[0][Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN];
			if (stockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG) continue;

			// 論理在庫数取得(※在庫情報が存在しない場合は、0とする)
			var stock = (productVariation[0][Constants.FIELD_PRODUCTSTOCK_STOCK] != DBNull.Value)
				? (int)productVariation[0][Constants.FIELD_PRODUCTSTOCK_STOCK]
				: 0;

			// 「購入数(変更後購入数 - 変更前購入数) > 在庫数」の場合、エラーメッセージを追加
			if (productStockAdjust.AdjustmentQuantity > stock)
			{
				errorProductNameList.Add(productStockAdjust.ProductName);
			}
		}

		return errorProductNameList;
	}

	/// <summary>
	/// 在庫管理対象 商品別数量増減リスト作成
	/// </summary>
	/// <param name="baseProductStockAdjustList">ベースとする在庫管理対象 商品別数量増減リスト<</param>
	/// <param name="orderItems">変更前もしくは変更後の商品情報</param>
	/// <param name="isOld">変更前フラグ true：変更前、false:変更後</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>引数の商品情報を追加した在庫管理対象 商品別数量増減リスト</returns>
	protected List<ProductStockAdjustCheck> CreateProductStockAdjustList(
		List<ProductStockAdjustCheck> baseProductStockAdjustList,
		List<OrderItemInput> orderItems,
		bool isOld,
		SqlAccessor accessor)
	{
		// ベースのリストを基にリストを作成
		var productStockAdjustList = (baseProductStockAdjustList == null)
			? new List<ProductStockAdjustCheck>()
			: new List<ProductStockAdjustCheck>(baseProductStockAdjustList);

		foreach (var orderItem in orderItems)
		{
			// 返品商品、引当済み場合はスキップ
			if ((orderItem.IsReturnItem) || (orderItem.IsRealStockReserved)) continue;

			// 削除対象の場合はスキップ(削除対象の場合変更前・後との消込が不要なため)
			if (orderItem.DeleteTarget) continue;

			// 数量が数値ではない場合はスキップ
			// 数量が数値の場合に変更前の場合はマイナス、変更後はプラスの数量とする
			int addItemQuantity;
			if (int.TryParse(orderItem.ItemQuantity, out addItemQuantity) == false) continue;
			addItemQuantity = addItemQuantity * (isOld ? -1 : 1);

			// 商品情報取得（在庫管理方法などを取得したい）
			var productVariation = GetProductVariation(orderItem.ShopId, orderItem.ProductId, orderItem.VariationId, string.Empty, string.Empty, accessor);

			// 商品情報が無い場合は在庫も無いため制御できないためスキップ
			if (productVariation.Count == 0) continue;

			// 在庫管理しない商品の場合はスキップ
			if ((string)productVariation[0][Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED) continue;

			// 在庫更新用リストに存在する場合リストの数量を更新、ない場合リストに追加
			// 入力された値が大文字・小文字の区別なく入力されている可能性があるため大文字に変換して比較
			var productStockAdjust = productStockAdjustList.FirstOrDefault(p =>
				((p.ShopId == orderItem.ShopId) && (p.ProductId.ToUpper() == orderItem.ProductId.ToUpper()) && (p.VariationId.ToUpper() == orderItem.VariationId.ToUpper())));
			if (productStockAdjust != null)
			{
				productStockAdjust.AdjustmentQuantity += addItemQuantity;
			}
			else
			{
				productStockAdjust = new ProductStockAdjustCheck
				{
					ShopId = orderItem.ShopId,
					ProductId = orderItem.ProductId,
					VariationId = orderItem.VariationId,
					ProductName = orderItem.ProductName,
					AdjustmentQuantity = addItemQuantity
				};

				productStockAdjustList.Add(productStockAdjust);
			}
		}

		return productStockAdjustList;
	}

	/// <summary>
	/// 選択注文決済区分入力情報取得（仮クレジットカード考慮）
	/// </summary>
	/// <param name="mallId">モールID</param>
	/// <returns>注文決済区分入力情報</returns>
	protected ListItem GetSelectedOrderPaymentListItem(string mallId)
	{
		var wddlOrderPaymentKbn = GetWrappedControl<WrappedDropDownList>("ddlOrderPaymentKbn");
		if ((this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus)
			&& (mallId == Constants.FLG_ORDER_MALL_ID_OWN_SITE))
		{
			var useNewCreditCard =
				((GetWrappedControl<WrappedDropDownList>("ddlUserCreditCard").SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
					|| GetWrappedControl<WrappedRadioButton>("rbNewCreditCard").Checked);
			if ((wddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && useNewCreditCard)
			{
				var payment = new PaymentService().Get(this.LoginOperatorShopId, Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID);
				return new ListItem(payment.PaymentName, payment.PaymentId);
			}
		}
		return wddlOrderPaymentKbn.SelectedItem;
	}

	/// <summary>
	/// Create Settlement Amount
	/// </summary>
	/// <param name="paymentId">Payment Id</param>
	/// <param name="priceTotal">Price Total</param>
	/// <param name="isVisibleSettlementAmount">Is Visible Settlement Amount</param>
	/// <returns>Settlement Currency Notation</returns>
	protected string CreateSettlementAmount(
		string paymentId,
		decimal priceTotal,
		bool isVisibleSettlementAmount)
	{
		var settlementCurrencyNotation = string.Empty;

		if (isVisibleSettlementAmount)
		{
			var settlementCurrency =
				CurrencyManager.GetSettlementCurrency(paymentId);
			var settlementRate =
				CurrencyManager.GetSettlementRate(settlementCurrency);
			var settlementAmount =
				CurrencyManager.GetSettlementAmount(
					priceTotal,
					settlementRate,
					settlementCurrency);
			settlementCurrencyNotation =
				CurrencyManager.ToSettlementCurrencyNotation(
					settlementAmount,
					settlementCurrency);
		}

		return settlementCurrencyNotation;
	}

	/// <summary>
	/// Check Max Length Url
	/// </summary>
	/// <param name="conditionUrl">Condition Url</param>
	public void CheckMaxLengthUrl(string conditionUrl)
	{
		var url = string.Format("{0}{1}{2}", Constants.PROTOCOL_HTTPS, Constants.SITE_DOMAIN, conditionUrl);
		if (url.Length <= Constants.ORDER_SEARCH_MAX_LENGTH_URL) return;

		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_TOO_MANY_SEARCH_CONDITION);
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>
	/// Show Display Demand Status
	/// </summary>
	/// <param name="paymentId">Payment Id</param>
	/// <returns>True: Show Or False: Hide</returns>
	public bool ShowDisplayDemandStatus(string paymentId)
	{
		switch (paymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
			case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
				return false;

			default:
				return true;
		}
	}

	/// <summary>
	/// 最大注文ID(枝番付き)取得
	/// </summary>
	/// <param name="orderId">Order Id</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>最大注文ID(枝番付き)</returns>
	protected string GetNewOrderReturnId(string orderId, SqlAccessor accessor)
	{
		var newOrderId = new OrderService()
			.GetOrderIdForOrderReturnExchange(orderId, accessor);
		return newOrderId;
	}

	/// <summary>
	/// ポイント戻し処理できるかどうか
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>エラーメッセージ</returns>
	protected static string CheckCanRevokeGrantedUserPoint(OrderModel order, SqlAccessor accessor)
	{
		var errorMessage = new List<string>();
		var orderAddedPoint = order.OrderPointAdd;
		if ((Constants.W2MP_POINT_OPTION_ENABLED == false) || (orderAddedPoint == 0))
		{
			return string.Empty;
		}

		// 付与ポイント情報取得
		var pointService = new PointService();
		var userPoint = pointService.GetUserPoint(order.UserId, string.Empty, accessor);
		var userPointHistory = pointService.GetUserPointHistoryByOrderId(order.UserId, order.OrderId, accessor);

		// 仮ポイントの場合
		if (userPoint.Any(x => (x.OrderId == order.OrderId) && x.IsPointTypeTemp))
		{
			return string.Empty;
		}

		// 利用ポイント復元情報取得
		bool isBeforeMigration;
		var usedUserPointHistory = pointService.GetUserPointHistoriesForRestore(
			order.UserId,
			order.OrderId,
			"",
			out isBeforeMigration,
			accessor);
		var actionExecutedDatetime = DateTime.Now;

		// 注文時に利用ポイント・ユーザー保持ポイント合計を取得
		var sumBaseUserPoint = 0m;
		var sumLimitedTermPoint = 0m;

		// 本ポイント
		var baseUserPointHistory =
			userPointHistory.Where(x => x.IsBasePoint && x.IsPointTypeComp && x.IsAddedPoint).ToArray();
		if (baseUserPointHistory.Any())
		{
			sumBaseUserPoint += usedUserPointHistory
				.Where(x => x.IsBasePoint && x.IsPointTypeComp && (actionExecutedDatetime <= x.UserPointExp))
				.Sum(x => (x.PointInc * -1));
			sumBaseUserPoint += userPoint.Where(x => x.IsBasePoint && x.IsPointTypeComp).Sum(x => x.Point);
		}

		// 期間限定ポイント
		var limitedTermPointHistory = userPointHistory.Where(x => x.IsLimitedTermPoint && x.IsAddedPoint).ToArray();
		if (limitedTermPointHistory.Any())
		{
			sumLimitedTermPoint += usedUserPointHistory
				.Where(x => x.IsLimitedTermPoint && (actionExecutedDatetime <= x.UserPointExp))
				.Sum(x => (x.PointInc * -1));
			sumLimitedTermPoint += userPoint.Where(x => x.IsLimitedTermPoint).Sum(x => x.Point);
		}

		// 本ポイントを戻せるか
		var addedBasePoint = baseUserPointHistory.Sum(x => x.PointInc);
		if (sumBaseUserPoint < addedBasePoint)
		{
			errorMessage.Add(
				string.Format(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_CANCEL_ADD_POINT_ERROR),
					sumBaseUserPoint,
					(addedBasePoint * -1),
					(sumBaseUserPoint - addedBasePoint)).Replace(
					"@@ point_kbn_name @@",
					ValueText.GetValueText(
						Constants.TABLE_USERPOINT,
						Constants.FIELD_USERPOINT_POINT_KBN,
						Constants.FLG_USERPOINT_POINT_KBN_BASE)));
		}

		// 期間限定ポイントを戻せるか
		var addedLimitedTermPoint = limitedTermPointHistory.Sum(x => x.PointInc);
		if (sumLimitedTermPoint < addedLimitedTermPoint)
		{
			errorMessage.Add(
				string.Format(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_CANCEL_ADD_POINT_ERROR),
					sumLimitedTermPoint,
					(addedLimitedTermPoint * -1),
					(sumLimitedTermPoint - addedLimitedTermPoint)).Replace(
					"@@ point_kbn_name @@",
					ValueText.GetValueText(
						Constants.TABLE_USERPOINT,
						Constants.FIELD_USERPOINT_POINT_KBN,
						Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT)));
		}
		return string.Join("<br/>", errorMessage);
	}
	
	/// <summary>
	/// 注文拡張項目の入力内容を設定
	/// </summary>
	/// <param name="rItem">注文拡張項目リピータ</param>
	/// <param name="input">入力ないよいう</param>
	public void SetOrderExtendFromUserExtendObject(Repeater rItem, OrderExtendInput input)
	{
		foreach (RepeaterItem item in rItem.Items)
		{
			var settingId = GetOrderExtendSettingId(item);
			var itemInput = input.OrderExtendItems.FirstOrDefault(m => m.SettingModel.SettingId == settingId) ?? new OrderExtendItemInput();
			switch (GetOrderExtendInputType(item))
			{
				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
					var wtbSelect = GetOrderExtendWrappedInputText(item);
					wtbSelect.Text = itemInput.InputValue;
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					var wddlSelect = GetOrderExtendWrappedDdlSelect(item);
					wddlSelect.Items.Clear();
					OrderExtendCommon.GetListItemForManager(itemInput.SettingModel.InputDefault).ForEach(listitem => wddlSelect.Items.Add(listitem));
					if (wddlSelect.Items.FindByValue(StringUtility.ToEmpty(itemInput.InputValue)) != null)
					{
						wddlSelect.Items.FindByValue(StringUtility.ToEmpty(itemInput.InputValue)).Selected = true;
					}
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
					var wrblSelect = GetOrderExtendWrappedRadioSelect(item);
					wrblSelect.Items.Clear();
					OrderExtendCommon.GetListItemForManager(itemInput.SettingModel.InputDefault).ForEach(listitem => wrblSelect.Items.Add(listitem));
					if (wrblSelect.Items.FindByValue(StringUtility.ToEmpty(itemInput.InputValue)) != null)
					{
						wrblSelect.Items.FindByValue(StringUtility.ToEmpty(itemInput.InputValue)).Selected = true;
					}
					break;

				case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					var wcblSelect = GetOrderExtendWrappedCheckSelect(item);
					wcblSelect.Items.Clear();
					OrderExtendCommon.GetListItemForManager(itemInput.SettingModel.InputDefault).ForEach(listitem => wcblSelect.Items.Add(listitem));
					var selectedItem = (StringUtility.ToEmpty(itemInput.InputValue)).Split(',');
					foreach (ListItem listitem in wcblSelect.Items)
					{
						listitem.Selected = selectedItem.Contains(listitem.Value);
					}
					break;

				default:
					// なにもしない
					break;
			}
		}
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎の拡張項目設定ID取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>拡張項目設定ID</returns>
	private string GetOrderExtendSettingId(RepeaterItem item)
	{
		var whfSettingId = GetWrappedControl<WrappedHiddenField>(item, "hfSettingId");
		return whfSettingId.Value;
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎の入力方法取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>入力方法</returns>
	private string GetOrderExtendInputType(RepeaterItem item)
	{
		var whfInputType = GetWrappedControl<WrappedHiddenField>(item, "hfInputType");
		return whfInputType.Value;
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎のラップ済みテキストボックスコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みテキストボックス</returns>
	private WrappedTextBox GetOrderExtendWrappedInputText(RepeaterItem item)
	{
		return GetWrappedControl<WrappedTextBox>(item, "tbSelect");
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎のラップ済みドロップダウンリストコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みドロップダウンリスト</returns>
	private WrappedDropDownList GetOrderExtendWrappedDdlSelect(RepeaterItem item)
	{
		return GetWrappedControl<WrappedDropDownList>(item, "ddlSelect");
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎のラップ済みラジオボタンリストコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みラジオボタンリスト</returns>
	private WrappedRadioButtonList GetOrderExtendWrappedRadioSelect(RepeaterItem item)
	{
		var wrblSelect = GetWrappedControl<WrappedRadioButtonList>(item, "rblSelect");
		return wrblSelect;
	}

	/// <summary>
	/// 注文拡張項目 リピータ毎のラップ済みチェックボックスリストコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みチェックボックスリスト</returns>
	private WrappedCheckBoxList GetOrderExtendWrappedCheckSelect(RepeaterItem item)
	{
		var wcblSelect = GetWrappedControl<WrappedCheckBoxList>(item, "cblSelect");
		return wcblSelect;
	}
	/// <summary>
	/// 注文拡張項目 エラーメッセージの設定
	/// </summary>
	/// <param name="riCart">カートリピーター</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	public void SetOrderExtendErrMessage(RepeaterItem riCart, Dictionary<string, string> errorMessage)
	{
		var wrOrderExtendInput = GetWrappedControl<WrappedRepeater>(riCart, "rOrderExtendInput");
		SetOrderExtendErrMessage(wrOrderExtendInput, errorMessage);
	}
	/// <summary>
	/// 注文拡張項目 エラーメッセージの設定
	/// </summary>
	/// <param name="rItem">注文項目リピーター</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	public void SetOrderExtendErrMessage(WrappedRepeater rItem, Dictionary<string, string> errorMessage)
	{
		foreach (RepeaterItem item in rItem.Items)
		{
			var settingId = GetOrderExtendSettingId(item);
			if (errorMessage.ContainsKey(settingId) == false) continue;

			var wlbErrMessage = GetWrappedControl<WrappedLabel>(item, "lbErrMessage");
			wlbErrMessage.Text = errorMessage[settingId];
		}
	}

	/// <summary>
	/// 注文拡張項目 入力内容の取得
	/// </summary>
	/// <param name="rItem">注文拡張項目リピーター</param>
	/// <returns>入力内容</returns>
	public Dictionary<string, string> CreateOrderExtendFromInputData(Repeater rItem)
	{
		var result = new Dictionary<string, string>();
		foreach (RepeaterItem item in rItem.Items)
		{
			var value = "";
			switch (GetOrderExtendInputType(item))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					var wtbSelect = GetOrderExtendWrappedInputText(item);
					value = StringUtility.ToEmpty(wtbSelect.Text);
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					var wddlSelect = GetOrderExtendWrappedDdlSelect(item);
					value = (wddlSelect.SelectedItem != null) ? wddlSelect.SelectedItem.Value : "";
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					var wrblSelect = GetOrderExtendWrappedRadioSelect(item);
					value = (wrblSelect.SelectedItem != null) ? wrblSelect.SelectedItem.Value : "";
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					var wcblSelect = GetOrderExtendWrappedCheckSelect(item);
					foreach (ListItem listitem in wcblSelect.Items)
					{
						if (listitem.Selected == false) continue;

						value += (value != "") ? "," : "";
						value += listitem.Value;
					}
					break;

				default:
					// なにもしない
					break;
			}

			var settingId = GetOrderExtendSettingId(item);
			result.Add(settingId, value);
		}
		return result;
	}

	/// <summary>
	/// ドロップダウンリストにアイテム追加
	/// </summary>
	/// <param name="ddl">追加先</param>
	/// <param name="listItems">リストアイテム列</param>
	/// <param name="defaultValue">デフォルト値（指定無し：null）</param>
	protected void AddItemToDropDownList(
		DropDownList ddl,
		IEnumerable<ListItem> listItems,
		string defaultValue)
	{
		foreach (var li in listItems)
		{
			li.Selected = (li.Value == defaultValue);
			ddl.Items.Add(li);
		}
	}

	/// <summary>
	/// ベリトランクレジットカード登録モックUrl作成
	/// </summary>
	/// <param name="sendId">ベリトラン連携用のユニークID</param>
	/// <returns>ベリトランスクレジットカード登録モックUrl</returns>
	protected string CreateRegisterCardVeriTransMockUrl(string sendId)
	{
		var url = new UrlCreator(Constants.PAYMENT_SETTING_PAYTG_MOCK_URL_FOR_VERITRANS4G)
			.AddParam(PayTgConstants.PARAM_CUSTOMERID, sendId)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// PayTgクレジットカード登録モックUrl作成
	/// </summary>
	/// <returns>クレジットカード登録モックUrl</returns>
	protected string CreateRegisterCardMockUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_REGISTER_CARD_MOCK)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// 配送希望日表示文言取得
	/// </summary>
	/// <param name="shippingDate">配送希望日</param>
	/// <returns>配送希望日表示文言</returns>
	protected static string GetShippingDateText(string shippingDate)
	{
		var orderShippingDate = DateTimeUtility.ToStringForManager(
			shippingDate,
			DateTimeUtility.FormatType.LongDateWeekOfDay2Letter,
			ReplaceTag("@@DispText.shipping_date_list.none@@"));
		if (Constants.MALLCOOPERATION_OPTION_ENABLED
			&& (string.IsNullOrEmpty(shippingDate) == false)
			&& shippingDate.StartsWith("1900"))
		{
			//「指定あり（メモ欄参照）」
			return ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_BODY_ORDER_CONFIRM,
				Constants.VALUETEXT_PARAM_SHIPPING_DATE_TEXT,
				Constants.VALUETEXT_PARAM_DESIGNATION);
		}
		else
		{
			return orderShippingDate;
		}
	}

	/// <summary>
	/// 配送希望時間帯表示文言取得
	/// </summary>
	/// <param name="shippingCompanyId">配送会社ID</param>
	/// <param name="shippingTimeId">配送希望時間帯</param>
	/// <returns>配送希望時間帯表示文言</returns>
	protected static string GetShippingTimeText(string shippingCompanyId, string shippingTimeId)
	{
		var deliveryCompany = new DeliveryCompanyService().GetAll().FirstOrDefault(i => i.DeliveryCompanyId == shippingCompanyId);
		var shippingTimeText = string.Empty;
		if (deliveryCompany != null)
		{
			shippingTimeText = deliveryCompany.GetShippingTimeMessage(shippingTimeId);
		}
		return (string.IsNullOrEmpty(shippingTimeText) == false) ? shippingTimeText : ReplaceTag("@@DispText.shipping_time_list.none@@");
	}

	/// <summary>
	/// 頒布会商品種類数エラーメッセージ取得
	/// </summary>
	/// <param name="subsctiptionBoxCourseId">頒布会コースID</param>
	/// <param name="numberOfProducts">商品種類数</param>
	/// <returns>エラーメッセージ</returns>
	public string GetSubscriptionBoxProductOfNumberError(string subsctiptionBoxCourseId, int numberOfProducts)
	{
		var subscriptionBoxProductOfNumberError =
			new SubscriptionBoxService().GetSubscriptionBoxProductOfNumberErrorType(
				subsctiptionBoxCourseId,
				numberOfProducts);
		var message = GetSubscriptionBoxProductOfNumberError(subscriptionBoxProductOfNumberError);
		return message;
	}

	/// <summary>
	/// 頒布会商品種類数エラーメッセージ取得
	/// </summary>
	/// <param name="managementName">表示名</param>
	/// <param name="numberOfProducts">商品種類数</param>
	/// <param name="minimumNumberOfProducts">最低購入種類数</param>
	/// <param name="maximumNumberOfProducts">最大購入種類数</param>
	/// <returns>エラーメッセージ</returns>
	public string GetSubscriptionBoxProductOfNumberError(
		string managementName,
		int numberOfProducts,
		int? minimumNumberOfProducts,
		int? maximumNumberOfProducts
		)
	{
		var subscriptionBoxProductOfNumberError =
			new SubscriptionBoxService().GetSubscriptionBoxProductOfNumberErrorType(
				managementName,
				numberOfProducts,
				minimumNumberOfProducts,
				maximumNumberOfProducts);
		var message = GetSubscriptionBoxProductOfNumberError(subscriptionBoxProductOfNumberError);
		return message;
	}

	/// <summary>
	/// 頒布会商品種類数エラーメッセージ取得
	/// </summary>
	/// <param name="errorType">頒布会商品種類数エラークラス</param>
	/// <returns>エラーメッセージ</returns>
	public string GetSubscriptionBoxProductOfNumberError(SubscriptionBoxProductOfNumberError errorType)
	{
		var message = "";
		switch (errorType.ErrorType)
		{
			case SubscriptionBoxProductOfNumberError.ErrorTypes.MinLimit:
				message = string.IsNullOrEmpty(errorType.MaximumNumberOfProducts)
					? WebMessages.GetMessages(
							WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_MIN_NUMBER_OF_PRODUCTS_ERROR)
						.Replace("@@ 1 @@", errorType.DisplayName)
						.Replace("@@ 2 @@", errorType.MinimumNumberOfProducts)
					: WebMessages.GetMessages(
							WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_MIN_NUMBER_OF_PRODUCTS_ERROR_SETTING_MAX_NUMBER_OF_PRODUCTS)
						.Replace("@@ 1 @@", errorType.DisplayName)
						.Replace("@@ 2 @@", errorType.MinimumNumberOfProducts)
						.Replace("@@ 3 @@", errorType.MaximumNumberOfProducts);
				break;

			case SubscriptionBoxProductOfNumberError.ErrorTypes.MaxLimit:
				message = string.IsNullOrEmpty(errorType.MinimumNumberOfProducts) 
					? WebMessages.GetMessages(
							WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_MAX_NUMBER_OF_PRODUCTS_ERROR)
						.Replace("@@ 1 @@", errorType.DisplayName)
						.Replace("@@ 2 @@", errorType.MaximumNumberOfProducts)
					: WebMessages.GetMessages(
							WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_MAX_NUMBER_OF_PRODUCTS_ERROR_SETTING_MIN_NUMBER_OF_PRODUCTS)
						.Replace("@@ 1 @@", errorType.DisplayName)
						.Replace("@@ 2 @@", errorType.MinimumNumberOfProducts)
						.Replace("@@ 3 @@", errorType.MaximumNumberOfProducts);
				break;
		}
		return message;
	}

	/// <summary>
	/// 頒布会数量エラー取得
	/// </summary>
	/// <param name="quantity">数量</param>
	/// <param name="maxQuantity">最大数量</param>
	/// <param name="minQuantity">最低数量</param>
	/// <param name="displayName">頒布会名</param>
	/// <returns></returns>
	public static string GetSubscriptionBoxQuantityError(int quantity, int? maxQuantity, int? minQuantity, string displayName)
	{
		var message = "";
		if ((minQuantity != null) && (quantity < minQuantity))
		{
			message = (maxQuantity == null)
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PRODUCT_QUANTITY_LACK)
					.Replace("@@ 1 @@", displayName)
					.Replace("@@ 2 @@", minQuantity.ToString())
				: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PRODUCT_QUANTITY_LACK_SETTING_MAX_QUANTITY)
					.Replace("@@ 1 @@", displayName)
					.Replace("@@ 2 @@", minQuantity.ToString())
					.Replace("@@ 3 @@", maxQuantity.ToString());
			return message;
		}

		if ((maxQuantity != null) && (quantity > maxQuantity)) 
		{
			message = (minQuantity == null)
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PRODUCT_QUANTITY_OVER)
					.Replace("@@ 1 @@", displayName)
					.Replace("@@ 2 @@", maxQuantity.ToString())
				: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PRODUCT_QUANTITY_OVER_SETTING_MIN_QUANTITY)
					.Replace("@@ 1 @@", displayName)
					.Replace("@@ 2 @@", minQuantity.ToString())
					.Replace("@@ 3 @@", maxQuantity.ToString());
			return message;
		}
		return message;
	}

	/// <summary>
	/// CrossPoint連携：伝票が存在しないエラーのみか
	/// </summary>
	/// <param name="errors">エラーコードリスト</param>
	/// <returns>伝票が存在しないエラーのみか</returns>
	protected static bool CheckOnlyNoSlipError(w2.App.Common.CrossPoint.Error[] errors)
	{
		if (errors == null) return false;

		var result = CheckOnlyNoSlipError(errors.Select(error => error.Code).ToArray());
		return result;
	}

	/// <summary>
	/// CrossPoint連携：伝票が存在しないエラーのみか
	/// </summary>
	/// <param name="errors">エラーコードリスト</param>
	/// <returns>伝票が存在しないエラーのみか</returns>
	protected static bool CheckOnlyNoSlipError(string[] errors)
	{
		if ((errors == null) || (errors.Length != 1)) return false;

		var noSlipErrorList = new[]
		{
			w2.App.Common.Constants.CROSS_POINT_RESULT_MODIFY_ERROR_NO_SLIP,
			w2.App.Common.Constants.CROSS_POINT_RESULT_DELETE_ERROR_NO_SLIP,
		};

		var result = noSlipErrorList.Contains(errors[0]);
		return result;
	}

	/// <summary>
	/// CrossPointのポイント情報変更
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="errors">エラー一覧</param>
	/// <param name="isExcludeForGrant">ポイント確定対象外か（ポイント確定済み or 伝票削除済み）</param>
	/// <param name="withGrant">ポイント確定を行うか</param>
	/// <returns>実行結果</returns>
	protected static bool UpdatePointByApi(
		OrderModel order,
		out string[] errors,
		out bool isExcludeForGrant,
		bool withGrant = false)
	{
		errors = null;
		isExcludeForGrant = false;

		var discount = order.MemberRankDiscountPrice
			+ order.OrderCouponUse
			+ order.SetpromotionProductDiscountAmount
			+ order.FixedPurchaseDiscountPrice
			+ order.FixedPurchaseMemberDiscountAmount
			- order.OrderPriceRegulation
			- order.ReturnPriceCorrection;
		var priceTaxIncluded = TaxCalculationUtility.GetPriceTaxIncluded(
			order.OrderPriceSubtotal,
			order.OrderPriceSubtotalTax);

		if ((order.OrderPriceSubtotal > discount)
			&& (priceTaxIncluded > discount))
		{
			var input = new PointApiInput
			{
				MemberId = order.UserId,
				OrderDate = order.OrderDate,
				PosNo = w2.App.Common.Constants.CROSS_POINT_POS_NO,
				OrderId = order.OrderId,
				BaseGrantPoint = order.OrderPointAdd,
				SpecialGrantPoint = 0m,
				PriceTotalInTax = (priceTaxIncluded - discount),
				PriceTotalNoTax = TaxCalculationUtility.GetPriceTaxExcluded(order.OrderPriceSubtotal, order.OrderPriceSubtotalTax) - discount,
				UsePoint = order.LastOrderPointUse,
				Items = CartObject.GetOrderDetails(order),
				ReasonId = CrossPointUtility.GetValue(Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID, w2.App.Common.Constants.CROSS_POINT_REASON_KBN_OPERATOR),
			};
			var result = new CrossPointPointApiService().Modify(input.GetParam(PointApiInput.RequestType.Modify));
			if (result.IsSuccess == false)
			{
				errors = result.Error.Select(err => err.Code).ToArray();
				return result.IsSuccess;
			}

			if (withGrant == false) return result.IsSuccess;

			var grantInput = new PointApiInput
			{
				MemberId = order.UserId,
				OrderId = order.OrderId,
			};

			// baseGrantPointが0だと連携先でエラーになるため、処理をスキップする。
			if (input.BaseGrantPoint == 0) return true;

			var grantResult = new CrossPointPointApiService().Grant(grantInput.GetParam(PointApiInput.RequestType.Grant));
			isExcludeForGrant = result.IsSuccess;

			if (grantResult.IsSuccess == false)
			{
				errors = grantResult.Error.Select(err => err.Code).ToArray();
			}
			return grantResult.IsSuccess;
		}
		else
		{
			var input = new PointApiInput
			{
				MemberId = order.UserId,
				OrderDate = order.OrderDate,
				PosNo = w2.App.Common.Constants.CROSS_POINT_POS_NO,
				OrderId = order.OrderId,
			};
			var result = new CrossPointPointApiService().Delete(input.GetParam(PointApiInput.RequestType.Delete));
			isExcludeForGrant = result.IsSuccess;

			if (result.IsSuccess == false) errors = result.Error.Select(err => err.Code).ToArray();
			return result.IsSuccess;
		}
	}

	/// <summary>
	/// Get point minimum purchase price error message
	/// </summary>
	/// <param name="settlementCurrency">Settlement currency</param>
	/// <returns>Point minimum purchase price error message</returns>
	protected string GetPointMinimumPurchasePriceErrorMessage(string settlementCurrency)
	{
		var pointMinimumPurchasePrice = Constants.GLOBAL_OPTION_ENABLE
			? CurrencyManager.ToSettlementCurrencyNotation(
				Constants.POINT_MINIMUM_PURCHASEPRICE,
				settlementCurrency)
			: string.Format("{0}円", Constants.POINT_MINIMUM_PURCHASEPRICE.ToPriceDecimal());
		var errorMessage = string.Format(
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POINT_MINIMUM_PURCHASEPRICE),
			pointMinimumPurchasePrice);

		return errorMessage;
	}

	/// <summary>
	/// 注文一覧データセットを表示分だけ取得
	/// </summary>
	/// <param name="input">パラメタ情報</param>
	/// <param name="pageNumber">表示開始記事番号</param>
	/// <param name="realShopIds">Real shop IDs</param>
	/// <returns>注文一覧データセット</returns>
	public DataView GetOrderListWithUserSymbols(Hashtable input, int pageNumber, string realShopIds = "")
	{
		// 受注一覧情報取得
		var orderList = GetOrderList(input, pageNumber, realShopIds);

		// ユーザーシンボル取得
		var userIds = orderList.Cast<DataRowView>().Select(row => (string)row[Constants.FIELD_USER_USER_ID]).Distinct().ToArray();
		var symbols = GetUserSymbols(userIds);

		// ユーザーシンボルをセット
		foreach (DataRowView row in orderList)
		{
			row.Row[Constants.FIELD_ORDEROWNER_OWNER_NAME] = (string)row.Row[Constants.FIELD_ORDEROWNER_OWNER_NAME]
				+ symbols.Where(symbol => symbol.Key == (string)row[Constants.FIELD_USER_USER_ID]).FirstOrDefault().Value;
		}
		return orderList;
	}

	/// <summary>
	/// 注文一覧データセットを表示分だけ取得
	/// </summary>
	/// <param name="input">パラメタ情報</param>
	/// <param name="pageNumber">表示開始記事番号</param>
	/// <param name="realShopIds">Real shop ids</param>
	/// <returns>注文一覧データセット</returns>
	private DataView GetOrderList(Hashtable input, int pageNumber, string realShopIds = "")
	{
		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement("Order", "GetOrderList"))
		{
			sqlStatement.UseLiteralSql = true;

			input["bgn_row_num"] = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (pageNumber - 1) + 1;
			input["end_row_num"] = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * pageNumber;

			sqlStatement.ReplaceStatement(
				"@@ orderby @@",
				OrderCommon.GetOrderSearchOrderByStringForOrderListAndWorkflow((string)input["sort_kbn"]));
			sqlStatement.ReplaceStatement(
				"@@ multi_order_id @@",
				StringUtility.ToEmpty(input[Constants.PARAM_REPLACEMENT_ORDER_ID_LIKE_ESCAPED])
					.Replace("'", "''")
					.Replace(",", "','"));
			sqlStatement.ReplaceStatement(
				"@@ replacement_order_id_like_escaped @@",
				StringUtility.ToEmpty(input[Constants.PARAM_REPLACEMENT_ORDER_ID_LIKE_ESCAPED])
					.Replace("'", "''"));
			sqlStatement.ReplaceStatement(
				"@@ order_shipping_addr1 @@",
				StringUtility.ToEmpty(input[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]));

			var whereRealShopIds = (string.IsNullOrEmpty(realShopIds) == false)
				? string.Format("AND w2_RealShop.real_shop_id IN ({0})", realShopIds)
				: string.Empty;

			sqlStatement.ReplaceStatement("@@ real_shop_ids_condition @@", whereRealShopIds);

			sqlStatement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(
				sqlStatement.Statement,
				Constants.TABLE_ORDER,
				StringUtility.ToEmpty(input[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));

			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
		}
	}

	/// <summary>永久トークン利用するか</summary>
	protected bool CreditTokenizedPanUse
	{
		get { return OrderCommon.CreditTokenizedPanUse; }
	}
	/// <summary>PayTgを利用するか</summary>
	protected bool IsUserPayTg
	{
		get { return OrderCommon.IsUserPayTg; }
	}
	/// <summary>クレジットカード有効期限(月)</summary>
	protected ListItem[] CreditExpirationMonthListItems
	{
		get
		{
			var items = DateTimeUtility.GetCreditMonthListItem();
			return items;
		}
	}
	/// <summary>クレジットカード有効期限(年)</summary>
	protected ListItem[] CreditExpirationYearListItems
	{
		get
		{
			var items = DateTimeUtility.GetCreditYearListItem((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS) ? "yyyy" : "yy");
			return items;
		}
	}
	/// <summary>編集後注文ID</summary>
	protected string UpdatedOrderId { get; set; }
	/// <summary>Array of omit check for OrderItem</summary>
	protected string OmitCheckForOrderItem
	{
		get { return Constants.FIELD_ORDERITEM_PRODUCT_NAME; }
	}
	/// <summary>Is need show confirm</summary>
	protected bool IsNeedShowConfirm { get; set; }
	/// <summary>返品時再与信エラーメッセージ</summary>
	protected string ReturnOrderReauthErrorMessages
	{
		get { return StringUtility.ToEmpty(Session["ReturnOrderReauthErrorMessages"]); }
		set { Session["ReturnOrderReauthErrorMessages"] = value; }
	}
	/// <summary>メール送信エラーメッセージ</summary>
	protected string SendMailErrorMessage
	{
		get { return SessionManager.SendMailErrorMessage; }
		set { SessionManager.SendMailErrorMessage = value; }
	}
	/// <summary>値登録向け・カード取引ID（登録する場合は値が格納される）</summary>
	protected string RegisterCardTranId { get; set; }
	/// <summary>値登録向け・決済注文ID（登録する場合は値が格納される）</summary>
	protected string RegisterPaymentOrderId { get; set; }
	/// <summary>値登録向け・外部決済与信日時（登録する場合は値が格納される）</summary>
	protected string RegisterExternalPaymentAuthDate { get; set; }
	/// <summary>値登録向け・外部決済ステータス（登録する場合は値が格納される）</summary>
	protected string RegisterExternalPaymentStatus { get; set; }
	/// <summary>値登録向け・オンライン決済ステータス（登録する場合は値が格納される）</summary>
	protected string RegisterOnlinePaymentStatus { get; set; }
	/// <summary>値登録向け・決済連携メモ（登録する場合は値が格納される）</summary>
	protected string RegisterPaymentMemo { get; set; }
	/// <summary>元注文情報値更新向け・外部決済ステータス（更新する場合は値が格納される）</summary>
	protected string UpdateExternalPaymentStatusForOrderOld { get; set; }
	/// <summary>元注文情報値更新向け・決済連携メモ（更新する場合は値が格納される</summary>
	protected string UpdatePaymentMemoForOrderOld { get; set; }
	/// <summary>Update Online Payment Status For Order Old</summary>
	protected string UpdateOnlinePaymentStatusForOrderOld { get; set; }
	/// <summary>Update order status for order old</summary>
	protected string UpdateOrderStatusForOrderOld { get; set; }
	//*****************************************************************************************
	/// <summary>
	/// Extend status class
	/// </summary>
	//*****************************************************************************************
	public class ExtendStatus
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="extendNo">The extend no.</param>
		/// <param name="extendName">Name of the extend.</param>
		/// <param name="extendParam">The extend parameter.</param>
		/// <param name="extendDateTime">The extend date time.</param>
		public ExtendStatus(string extendNo, string extendName, string extendParam, DateTime extendDateTime)
		{
			this.No = extendNo;
			this.Name = extendName;
			this.Param = extendParam;
			this.Time = extendDateTime;
		}

		/// <summary>The extend no</summary>
		public string No { get; private set; }
		/// <summary>Name of the extend</summary>
		public string Name { get; private set; }
		/// <summary>The extend parameter</summary>
		public string Param { get; private set; }
		/// <summary>The extend date time</summary>
		public DateTime Time { get; private set; }
	}
	/// <summary>Taiwan Order Invoice Input</summary>
	protected TwOrderInvoiceModel TwOrderInvoice { get; set; }
	/// <summary>Invoice Uniform: COMPANY</summary>
	protected bool IsCompany { get; set; }
	/// <summary>Invoice Uniform: DONATE</summary>
	protected bool IsDonate { get; set; }
	/// <summary>Invoice Uniform: PERSONAL</summary>
	protected bool IsPersonal { get; set; }
	/// <summary>Invoice Carry Type: EMPTY</summary>
	public bool IsNoCarryType { get; set; }
	/// <summary>Invoice Carry Type: MOBILE</summary>
	public bool IsMobile { get; set; }
	/// <summary>Invoice Carry Type: CERTIFICATE</summary>
	public bool IsCertificate { get; set; }
	/// <summary>Store Value For Dropdown Carry Type Change</summary>
	protected string CarryTypeValue { get; set; }
	/// <summary>Store Value For Dropdown Uniform Change</summary>
	protected string UniformTypeValue { get; set; }
	/// <summary>Taiwan Order Invoice Input</summary>
	protected TwFixedPurchaseInvoiceModel TwFixedPurchaseInvoice { get; set; }
	/// <summary>Taiwan Order Invoice Input</summary>
	protected TwOrderInvoiceInput TwOrderInvoiceInput
	{
		get { return (TwOrderInvoiceInput)ViewState["TwOrderInvoiceInput"]; }
		set { ViewState["TwOrderInvoiceInput"] = value; }
	}
	/// <summary>PayTgカード登録に成功したか</summary>
	protected bool IsSuccessfulCardRegistration
	{
		get 
		{
			if (this.Session["IsSuccessfulCardRegistration"] == null) return false;
			return (bool)this.Session["IsSuccessfulCardRegistration"];
		}
		set { this.Session["IsSuccessfulCardRegistration"] = value; }
	}
	/// <summary>配送先：都道府県の検索できるか？</summary>
	protected bool IsSearchShippingAddr1
	{
		get
		{
			return Constants.SEARCHCONDITION_SHIPPINGADDR1_ENABLED && this.IsShippingCountryAvailableJp;
		}
	}
	/// <summary>現在の付与された通常本ポイント</summary>
	protected string AddedBasePointCompOld
	{
		get { return StringUtility.ToNumeric((string)ViewState["AddedBasePointCompOld"]); }
		set { ViewState["AddedBasePointCompOld"] = value; }
	}
	/// <summary>payTgレスポンス</summary>
	protected Hashtable PayTgResponse
	{
		get { return (Hashtable)ViewState["PayTgResponse"]; }
		set { ViewState["PayTgResponse"] = value; }
	}
	/// <summary>ベリトランス連携会員ID</summary>
	protected string VeriTransAccountId
	{
		get { return (string)this.Session[PayTgConstants.PARAM_CUSTOMERID]; }
		set { this.Session[PayTgConstants.PARAM_CUSTOMERID] = value; }
	}
	/// <summary>ポイント発行されない注文か</summary>
	protected string IsNoPointPublished 
	{
		get { return (string)ViewState["IsNoPointPublished"]; }
		set { ViewState["IsNoPointPublished"] = value; }
	}
}
