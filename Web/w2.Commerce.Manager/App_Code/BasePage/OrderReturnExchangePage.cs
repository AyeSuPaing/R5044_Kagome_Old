/*
=========================================================================================================
  Module      : 注文返品交換共通ページ(OrderReturnExchangePage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Order;
using w2.Domain.Order;
using w2.Domain.SubscriptionBox;

/// <summary>
/// OrderReturnExchangePage の概要の説明です
/// </summary>
public class OrderReturnExchangePage : OrderPage
{
	//------------------------------------------------------
	// 定数
	//------------------------------------------------------

	protected const string FIELD_ORDER_ORDER_PAYMENT_KBN_TEXT = Constants.FIELD_ORDER_ORDER_PAYMENT_KBN + "_text";
	protected const string FIELD_ORDER_ORDER_PRICE_SUBTOTAL_RETURN = Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL + "_return";
	protected const string FIELD_ORDER_ORDER_PRICE_SUBTOTAL_EXCHANGE = Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL + "_exchange";
	protected const string FIELD_ORDER_ORDER_PRICE_TAX_RETURN = Constants.FIELD_ORDER_ORDER_PRICE_TAX + "_return";
	protected const string FIELD_ORDER_ORDER_PRICE_TAX_EXCHANGE = Constants.FIELD_ORDER_ORDER_PRICE_TAX + "_exchange";

	// ポイントOP関連
	/// <summary>調整後付与ポイント（仮ポイント）</summary>
	protected const string CONST_ORDER_POINT_ADD_TEMP = Constants.FIELD_ORDER_ORDER_POINT_ADD + "_temp";
	/// <summary>調整後付与 通常ポイント（本ポイント）</summary>
	protected const string CONST_ORDER_BASE_POINT_ADD_COMP = Constants.FIELD_ORDER_ORDER_POINT_ADD + "_base_comp";
	/// <summary>調整後付与 期間限定ポイント（本ポイント）</summary>
	protected const string CONST_ORDER_LIMIT_POINT_ADD_COMP = Constants.FIELD_ORDER_ORDER_POINT_ADD + "_limit_comp";
	/// <summary>調整付与ポイント（仮 or 本ポイント）</summary>
	protected const string CONST_ORDER_POINT_ADD_ADJUSTMENT = Constants.FIELD_ORDER_ORDER_POINT_ADD + "_adjustment";
	/// <summary>調整後利用ポイント</summary>
	protected const string CONST_ORDER_ORDER_POINT_USE_ADJUSTMENT = Constants.FIELD_ORDER_ORDER_POINT_USE + "_adjustment";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 元注文情報取得(返品変更商品情報も含む)
		//------------------------------------------------------
		// 子注文の返品商品も含んだ情報を作成する
		var order = new OrderService().GetOrderForOrderReturnExchangeInDataView(this.OrderIdOrg);
		// 元注文が見つからない場合はエラーとする
		if (order.Count == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 元注文情報にセット商品が含まれていた場合はエラーとする（現状セット商品未対応）
		foreach (DataRowView drvOrderItem in order)
		{
			if ((string)drvOrderItem[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID] != "")
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RETURNEXCHNAGE_SET_PRODUCT_DATA);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
		// プロパティへセット
		this.ReturnExchangeOrderOrg = new Order(order);
	}

	/// <summary>
	/// 返品商品チェック
	/// </summary>
	/// <param name="lReturnOrderItems">返品商品情報</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckReturnOrderItem(List<ReturnOrderItem> lReturnOrderItems)
	{
		StringBuilder sbResult = new StringBuilder();

		//------------------------------------------------------
		// 商品数チェック
		//------------------------------------------------------
		if (lReturnOrderItems.Count == 0)
		{
			sbResult.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RETURNEXCHNAGE_RETURN_ORDERITEM_NO_DATA));
		}

		//------------------------------------------------------
		// 商品存在チェック
		//------------------------------------------------------
		foreach (ReturnOrderItem roii in lReturnOrderItems)
		{
			DataView productVariation = GetProductVariation(roii.ShopId, roii.ProductId, roii.VariationId);
			if (productVariation.Count == 0)
			{
				sbResult.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", roii.ProductName));
			}
		}

		return sbResult.ToString();
	}

	/// <summary>
	/// 返品商品チェック（ギフト交換用）
	/// </summary>
	/// <param name="lReturnOrderItems">返品商品情報</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckReturnOrderItemForGift(List<ReturnOrderItem> lReturnOrderItems)
	{
		StringBuilder sbResult = new StringBuilder();
		HashSet<string> hsOrderShippngNumbers = new HashSet<string>();

		//------------------------------------------------------
		// 返品商品が複数の配送先にまたがっている場合にはエラー
		//------------------------------------------------------
		foreach (ReturnOrderItem roii in lReturnOrderItems)
		{
			hsOrderShippngNumbers.Add(roii.OrderShippingNo);
		}
		if (hsOrderShippngNumbers.Count >= 2)
		{
			sbResult.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RETURNEXCHNAGE_EXCHANGE_TO_MULTI_SHIPPINGS));
		}

		return sbResult.ToString();
	}

	/// <summary>
	/// 交換商品チェック
	/// </summary>
	/// <param name="lExchangeOrderItems">交換商品情報</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckExchangeOrderItem(List<ReturnOrderItem> lExchangeOrderItems)
	{
		StringBuilder sbResult = new StringBuilder();

		//------------------------------------------------------
		// 商品情報取得
		//------------------------------------------------------
		List<DataView> lProductVariations = new List<DataView>(
			lExchangeOrderItems.Select(item => GetProductVariation(item.ShopId, item.ProductId, item.VariationId)));

		//------------------------------------------------------
		// 商品数チェック
		//------------------------------------------------------
		if (lExchangeOrderItems.Count == 0)
		{
			sbResult.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RETURNEXCHNAGE_EXCHANGE_ORDERITEM_NO_DATA));
		}

		//------------------------------------------------------
		// 同一商品有無チェック
		//------------------------------------------------------
		Hashtable htTemp = new Hashtable();
		foreach (var roii in lExchangeOrderItems)
		{
			string strKey = roii.ProductId + "***" + roii.VId;
			if (htTemp.Contains(strKey))
			{
				sbResult.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERITEM_DUPLICATION_ERROR).Replace("@@ 1 @@", roii.ProductId).Replace("@@ 2 @@", roii.VId));
				break;
			}
			htTemp.Add(strKey, "");
		}

		//------------------------------------------------------
		// 商品存在チェック
		//------------------------------------------------------
		foreach (var roii in lExchangeOrderItems)
		{
			DataView dvProductVariation = ((DataView)lProductVariations[lExchangeOrderItems.IndexOf(roii)]);
			if (dvProductVariation.Count == 0)
			{
				sbResult.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", roii.ProductName));
			}
		}

		//------------------------------------------------------
		// 商品配送種別チェック
		// ※元注文と異なる配送種別の商品が投入された場合はエラーとする
		//------------------------------------------------------
		if (lExchangeOrderItems.Count > 0)
		{
			foreach (var roii in lExchangeOrderItems)
			{
				DataView dvProductVariation = ((DataView)lProductVariations[lExchangeOrderItems.IndexOf(roii)]);
				if (dvProductVariation.Count != 0)
				{
					// 元注文の配送種別と異なる商品が登録された場合
					if (this.ReturnExchangeOrderOrg.ShippingId != (string)dvProductVariation[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE])
					{
						sbResult.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RETURNEXCHNAGE_ORDER_SHIPPING_KBN_DIFF).Replace("@@ 1 @@", roii.ProductName));
					}
				}
			}
		}

		//------------------------------------------------------
		// エラーではない場合 && 受注管理時の在庫連動あり
		//------------------------------------------------------
		if ((sbResult.Length == 0) && (Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED))
		{
			//------------------------------------------------------
			// 商品在庫チェック
			//------------------------------------------------------
			foreach (var roii in lExchangeOrderItems)
			{
				DataView dvProductVariation = ((DataView)lProductVariations[lExchangeOrderItems.IndexOf(roii)]);
				if (dvProductVariation.Count != 0)
				{
					// 在庫区分で場合分け
					switch ((string)dvProductVariation[0][Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN])
					{
						// 「在庫管理無し」
						case Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED:
						// 「在庫0以下の場合でも購入可能」
						case Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYOK:
							break;

						// 「在庫0以下の場合購入不可」
						case Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG:
							int iStock = (dvProductVariation[0][Constants.FIELD_PRODUCTSTOCK_STOCK] != DBNull.Value) ? (int)dvProductVariation[0][Constants.FIELD_PRODUCTSTOCK_STOCK] : 0;
							if (roii.ItemQuantity > iStock) // エラーチェック通過してるのでQuantityは数値で間違いない
							{
								sbResult.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_NO_STOCK).Replace("@@ 1 @@", roii.ProductName));
							}
							break;
					}
				}
			}
		}

		return sbResult.ToString();
	}

	/// <summary>
	/// 調整後付与ポイントチェック
	/// </summary>
	/// <param name="pointIncKbnString">ポイント加算区分（文字列）</param>
	/// <param name="orderPointAddString">調整後付与ポイント（文字列）</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckOrderPointAdd(string pointIncKbnString, string orderPointAddString)
	{
		// 入力チェック
		var input = new Hashtable
		{
			{Constants.FIELD_ORDER_ORDER_POINT_ADD, orderPointAddString}
		};
		var pointErrorMessage = Validator.Validate("OrderReturnExchangePoint", input);
		if (pointErrorMessage.Length != 0) return pointErrorMessage.Replace("@@ 1 @@", pointIncKbnString);

		// 調整後ポイントがマイナスになる場合はエラー
		if (decimal.Parse(orderPointAddString) < 0)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RETURNEXCHNAGE_RETURN_ADD_POINT);
		}

		return string.Empty;
	}

	/// <summary>返品対象注文ID</summary>
	protected string OrderIdOrg
	{
		get { return Request[Constants.REQUEST_KEY_ORDER_ID]; }
	}
	/// <summary>元注文（＋関連注文商品）情報
	/// 商品情報プロパティには、子注文の返品商品も含まれ、
	///	その商品の注文IDは子注文の注文IDとなっている
	/// </summary>
	protected Order ReturnExchangeOrderOrg { get; set; }
	/// <summary>頒布会コースID</summary>
	protected string SubscriptionBoxCourseId
	{
		get { return ReturnExchangeOrderOrg.SubscriptionBoxCourseId; } // FIXME: Shouldn't be use FIELD constant value.
	}
	/// <summary>頒布会定額コースか</summary>
	protected bool IsSubscriptionBoxFixedAmount
	{
		get
		{
			if (string.IsNullOrEmpty(this.SubscriptionBoxCourseId)) return false;
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.SubscriptionBoxCourseId);
			var result = (subscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE);
			return result;
		}
	}
	/// <summary>注文商品の頒布会コースIDエリアを表示するか</summary>
	protected bool DisplayItemSubscriptionBoxCourseIdArea
	{
		get
		{
			return Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
				&& this.ReturnExchangeOrderOrg.IsOrderCombinedWithSubscriptionBoxItem
				&& (this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false);
		}
	}
}
