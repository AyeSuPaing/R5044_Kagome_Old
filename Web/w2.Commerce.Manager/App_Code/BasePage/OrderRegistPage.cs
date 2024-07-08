/*
=========================================================================================================
  Module      : 注文登録共通ページ(OrderRegistPage.cs)
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using w2.App.Common.Order;
using w2.Domain.Order;
using w2.Domain.SerialKey;

/// <summary>
/// OrderRegistPage の概要の説明です
/// </summary>
public class OrderRegistPage : OrderPage
{
	/// <summary>
	/// 注文商品チェック
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckOrderItemsAndPrices(CartObject cart)
	{
		StringBuilder errorMessages = new StringBuilder();

		// 商品情報取得
		List<DataView> productVariations = GetItemProductVariationForCheck(cart.Items);

		// 商品明細数チェック
		if (productVariations.Count == 0) errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERITEM_NO_DATA));

		// 同一商品有無チェック
		errorMessages.Append(CheckSameItems(cart.Items));

		// 定期購入チェック
		errorMessages.Append(CheckFixedPurchaseFlg(cart.Items, productVariations));

		// エラーでなければ、配送種別等の混在チェック
		if ((errorMessages.Length == 0) && (cart.Items.Count > 1))
		{
			errorMessages.Append(CheckProductTypeMixed(cart.Items, productVariations));
		}

		// エラーでない かつ受注管理時の在庫連動ありであれば在庫チェック
		if ((errorMessages.Length == 0) && (Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED))
		{
			if (string.IsNullOrEmpty(cart.OrderCombineParentOrderId))
			{
				errorMessages.Append(CheckProductStock(cart.Items, productVariations));
			}
			else
			{
				errorMessages.Append(CheckProductStock(cart.Items, productVariations, cart.OrderCombineParentOrderId.Split(',')));
			}
		}

		// エラーでなければ、デジタルコンテンツのキー存在チェック
		if (errorMessages.Length == 0)
		{
			errorMessages.Append(CheckSerialKeyExists(cart.Items, productVariations));
		}

		// 0円以上チェック
		if (cart.PriceTotal < 0) errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_TOTAL_PRICE_IS_MINUS));

		return errorMessages.ToString();
	}

	/// <summary>
	/// 注文商品の商品バリエーション情報取得
	/// </summary>
	/// <param name="items">注文商品</param>
	/// <returns>商品バリエーション情報</returns>
	private List<DataView> GetItemProductVariationForCheck(List<CartProduct> items)
	{
		var productVariations = new List<DataView>();
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			foreach (var item in items)
			{
				productVariations.Add(GetProductVariation(item.ShopId, item.ProductId, item.VariationId, "", "", sqlAccessor));
			}
		}
		return productVariations;
	}

	/// <summary>
	/// 同一商品チェック
	/// </summary>
	/// <param name="items">注文商品</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckSameItems(List<CartProduct> items)
	{
		var errorMessages = new StringBuilder();

		var temp = new Hashtable();
		foreach (var item in items)
		{
			var key = string.Format(
				"{0}***{1}***{2}***{3}***{4}",
				item.ProductId,
				item.VariationId,
				item.IsFixedPurchase,
				item.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues(),
				item.SubscriptionBoxCourseId);
			if (temp.Contains(key))
			{
				errorMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERITEM_DUPLICATION_ERROR)
						.Replace("@@ 1 @@", item.ProductId)
						.Replace("@@ 2 @@", item.VariationId));
				break;
			}
			temp.Add(key, "");
		}
		return errorMessages.ToString();
	}

	/// <summary>
	/// 定期購入フラグチェック
	/// </summary>
	/// <param name="cartItems">注文商品</param>
	/// <param name="productVariations">注文商品の商品バリエーション情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckFixedPurchaseFlg(List<CartProduct> cartItems, List<DataView> productVariations)
	{
		var errorMessages = new StringBuilder();
		// 定期購入出来ない者がチェックされていないか？
		foreach (var item in cartItems.FindAll(i => i.IsFixedPurchase && (i.IsOrderCombine == false)))
		{
			DataView variation = ((DataView)productVariations[cartItems.IndexOf(item)]);
			if ((variation.Count != 0)
				&& ((string)variation[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID))
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_FIXED_PURCHASE_DISABLE).Replace("@@ 1 @@", item.ProductJointName));
			}
		}

		// 定期購入しかできないものがチェック外されていないか？
		foreach (var item in cartItems.FindAll(i => (i.IsFixedPurchase == false)))
		{
			DataView variation = ((DataView)productVariations[cartItems.IndexOf(item)]);
			if ((variation.Count != 0)
				&& ((string)variation[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY))
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_FIXED_PURCHASE_ONLY).Replace("@@ 1 @@", item.ProductJointName));
			}
		}

		return errorMessages.ToString();
	}

	/// <summary>
	/// 商品タイプ混在チェック
	/// </summary>
	/// <param name="items">注文商品</param>
	/// <param name="productVariations">注文商品の商品バリエーション情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckProductTypeMixed(List<CartProduct> items, List<DataView> productVariations)
	{
		var errorMessages = new StringBuilder();

		// 配送種別が異なるときはエラー格納
		string shippingType = null;
		foreach (var item in items)
		{
			DataView dvProductVariation = ((DataView)productVariations[items.IndexOf(item)]);
			if (shippingType == null)
			{
				shippingType = (string)dvProductVariation[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
				continue;
			}

			if (shippingType != (string)dvProductVariation[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE])
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_SHIPPING_KBN_DIFF));
				break;
			}
		}

		// 定期商品が混在するときはエラー格納
		bool fixedPurchase = false;
		foreach (var item in items)
		{
			// 注文同梱の場合、定期商品の混在をチェックしない
			if (item.IsOrderCombine) break;

			if (items.IndexOf(item) == 0)
			{
				fixedPurchase = item.IsFixedPurchase;
				continue;
			}

			if ((Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION) && (fixedPurchase != item.IsFixedPurchase))
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_FIXED_PURCHASE_SEPARATION));
				break;
			}
		}

		// デジタルコンテンツが混在するときはエラー格納
		HashSet<string> digitalContensFlgs = new HashSet<string>();
		foreach (var item in items)
		{
			DataView variation = ((DataView)productVariations[items.IndexOf(item)]);
			digitalContensFlgs.Add((string)variation[0][Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG]);
		}
		if (digitalContensFlgs.Count > 1) // 混在あり
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DIGITAL_CONTENTS_SEPARATION));
		}

		return errorMessages.ToString();
	}

	/// <summary>
	/// 商品在庫チェック
	/// </summary>
	/// <param name="items">注文商品</param>
	/// <param name="productVariations">注文商品の商品バリエーション情報</param>
	/// <param name="combinedParentOrderIds">注文同梱対象 注文ID ※注文同梱画面から遷移の場合のみ設定あり</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckProductStock(List<CartProduct> items, List<DataView> productVariations, string[] combinedParentOrderIds = null)
	{
		var errorMessages = new StringBuilder();
		var combinedOrderList = new List<OrderModel>();
		if (combinedParentOrderIds != null)
		{
			combinedOrderList = combinedParentOrderIds.Select(id => new OrderService().Get(id)).ToList();
		}

		foreach (var item in items)
		{
			DataView variation = ((DataView)productVariations[items.IndexOf(item)]);
			if (variation.Count == 0) continue;

			switch ((string)variation[0][Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN])
			{
				// 「在庫管理無し」
				case Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED:
				// 「在庫0以下の場合でも購入可能」
				case Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYOK:
					break;

				// 「在庫0以下の場合購入不可」
				case Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG:
				case Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPNG_BUYNG:
					int stock = (variation[0][Constants.FIELD_PRODUCTSTOCK_STOCK] != DBNull.Value) ? (int)variation[0][Constants.FIELD_PRODUCTSTOCK_STOCK] : 0;

					// 注文同梱されている場合、親注文の注文個数を取得 ※新規注文情報登録画面での同梱の場合
					var allocatedCount = 0;
					if (item.IsOrderCombine && (item.OrderCombineOrgOrderId != null))
					{
						var parentOrder = new OrderService().Get(item.OrderCombineOrgOrderId);
						allocatedCount = parentOrder.Shippings[0].Items
							.Where(parentItem => (parentItem.VariationId == (string)variation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]))
							.Sum(parentItem => (parentItem.ItemQuantity));
					}
					// 注文同梱での同梱の場合
					else if (combinedOrderList.Count > 0)
					{
						foreach (var order in combinedOrderList)
						{
							allocatedCount += order.Shippings[0].Items
								.Where(orderItem => (orderItem.VariationId == (string)variation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]))
								.Sum(orderItem => (orderItem.ItemQuantity));
						}
					}

					// 親注文分の注文個数(在庫割り当て済み)を商品個数から除外してチェック
					if ((item.Count - allocatedCount) > stock)	// エラーチェック通過してるのでQuantityは数値で間違いない
					{
						errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_NO_STOCK).Replace("@@ 1 @@", item.ProductJointName));
					}
					break;
			}
		}
		return errorMessages.ToString();
	}

	/// <summary>
	/// デジタルコンテンツ キー存在チェック
	/// </summary>
	/// <param name="items">注文商品</param>
	/// <param name="productVariations">注文商品の商品バリエーション情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckSerialKeyExists(List<CartProduct> items, List<DataView> productVariations)
	{
		var errorMessages = new StringBuilder();

		foreach (var item in items)
		{
			DataView variation = ((DataView)productVariations[items.IndexOf(item)]);
			if ((string)variation[0][Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID) continue;

			var serialKeys = new SerialKeyService().CheckForReserveSerialKey(item.Count, item.ProductId, item.VariationId);
			
			// シリアルキー不足時
			if (serialKeys.Length != item.Count)
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DIGITAL_CONTENTS_NO_KEYS).Replace("@@ 1 @@", item.ProductName));
			}
		}
		return errorMessages.ToString();
	}

	/// <summary>
	/// Get css class for status
	/// </summary>
	/// <param name="status">Status</param>
	/// <param name="type">Type of status</param>
	/// <returns>Css class</returns>
	protected string GetCssClassForStatus(string status, string type)
	{
		var isCautionClass = "is-caution";
		var isActiveClass = "is-active";
		switch (type)
		{
			case Constants.FIELD_ORDER_ORDER_STATUS:
				switch (status)
				{
					case Constants.FLG_ORDER_ORDER_STATUS_ORDERED:
					case Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED:
					case Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED:
					case Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED:
					case Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED:
					case Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED:
					case Constants.FLG_ORDER_ORDER_STATUS_TEMP:
						return isActiveClass;
				}
				break;

			case Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS:
				if (status == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM)
					return isCautionClass;
				break;

			case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS:
				switch (status)
				{
					case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL:
					case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL_TEMP:
						return isActiveClass;
				}
				break;

			case Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS:
				if (status == Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR)
					return isCautionClass;
				break;
		}
		return string.Empty;
	}

	/// <summary>
	/// Add attributes for control display
	/// </summary>
	/// <param name="control">Control</param>
	/// <param name="keyName">Key Name</param>
	/// <param name="keyValue">Key Value</param>
	/// <param name="isAdded">Is Added</param>
	protected void AddAttributesForControlDisplay(
		Control control,
		string keyName,
		string keyValue,
		bool isAdded)
	{
		if (control is HtmlControl)
		{
			((HtmlControl)control).Attributes.Add(
				keyName,
				isAdded ? keyValue : string.Empty);
		}
	}

	/// <summary>Product column span number</summary>
	protected int ProductColSpanNumber
	{
		get
		{
			var colSpan = 2;
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED) colSpan++;
			if (Constants.PRODUCT_SALE_OPTION_ENABLED) colSpan++;
			return colSpan;
		}
	}
}
