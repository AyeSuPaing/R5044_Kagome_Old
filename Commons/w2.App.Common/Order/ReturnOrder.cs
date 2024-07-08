/*
=========================================================================================================
  Module      : 返品交換注文情報クラス(ReturnOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.User;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 返品交換注文情報クラス
	/// </summary>
	[Serializable]
	public class ReturnOrder
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="returnExchangeOrder">返品交換対象の注文情報（関連注文の返品商品含む）</param>
		public ReturnOrder(Order returnExchangeOrder)
		{
			this.OrderId = returnExchangeOrder.OrderId;
			this.OrderStatus = returnExchangeOrder.OrderStatus;
			this.SubscriptionBoxCourseId = returnExchangeOrder.SubscriptionBoxCourseId;
			this.SubscriptionBoxFixedAmount = returnExchangeOrder.SubscriptionBoxFixedAmount;
			this.OrderSubscriptionBoxOrderCount = returnExchangeOrder.OrderSubscriptionBoxOrderCount;

			// 返品可能商品リスト作成
			SetReturnOrderItems(returnExchangeOrder);

			// ポイント情報セット ※DataBind時にエラーが発生しないようPointOptionの利用可否に関係なく実行する
			SetOrderPoint(returnExchangeOrder);

			// 注文合計情報セット
			SetOrderSummaryInfo(returnExchangeOrder);

			// 税率毎価格情報セット
			SetOrderPriceByTaxRate(returnExchangeOrder);
		}

		/// <summary>
		/// 返品可能商品リスト作成（商品情報プロパティをセット）
		/// </summary>
		/// <param name="returnExchangeOrder">返品交換対象の注文情報（関連注文の返品商品含む）</param>
		private void SetReturnOrderItems(Order returnExchangeOrder)
		{
			// 返品可能商品を格納するリスト
			var returnItems = new List<ReturnOrderItem>();
			// 配送先番号一覧を格納するハッシュセット 
			var shippingNumbers = new HashSet<string>();

			// 表示対象の注文IDの返品可能商品を格納
			foreach (OrderItem orderItem in returnExchangeOrder)
			{
				if (orderItem.OrderId == this.OrderId)
				{
					// 注文数分ループ(注文数がマイナスの場合は、プラスに変更)
					foreach (var i in Enumerable.Range(0, Math.Abs(int.Parse(orderItem.ItemQuantity))))
					{
						var returnableItem = new ReturnOrderItem();
						returnableItem.ShopId = orderItem.ShopId;
						returnableItem.ReturnStatus =
							(orderItem.ItemReturnExchangeKbn == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN)
							? Constants.FLG_ORDER_RETURN_STATUS_RETURN_EXCHANGED
							: Constants.FLG_ORDER_RETURN_STATUS_RETURN_UNTARGET;

						returnableItem.ProductId = orderItem.ProductId;
						returnableItem.VId = orderItem.VId;
						returnableItem.ProductName = orderItem.ProductName;

						if (orderItem.IsSubscriptionBoxFixedAmount == false)
						{
							returnableItem.ProductPrice =
								decimal.TryParse(orderItem.ProductPrice, out var productPrice)
									? productPrice
									: 0m;
							returnableItem.ItemPriceTax =
								decimal.TryParse(orderItem.ItemPriceTax, out var itemPriceTax)
									? itemPriceTax
									: 0m;
							returnableItem.ProductPricePretax =
								decimal.TryParse(orderItem.ProductPricePretax, out var productPricePretax)
									? productPricePretax
									: 0m;

							// 按分計算してから追加
							returnableItem.DiscountedPrice =
								decimal.TryParse(orderItem.DiscountedPrice, out var discountedPrice)
									? CaliculateDiscountedPriceFracution(
										discountedPrice,
										i,
										int.Parse(orderItem.ItemQuantity))
									: 0m;
						}
						else
						{
							returnableItem.ProductPrice = 0m;
							returnableItem.ItemPriceTax = 0m;
							returnableItem.ProductPricePretax = 0m;
							returnableItem.DiscountedPrice = 0m;
						}

						returnableItem.ItemQuantity =
							(orderItem.ItemReturnExchangeKbn == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN)
							? -1
							: 1;

						returnableItem.ProductSaleId = orderItem.ProductSaleId;
						returnableItem.NoveltyId = orderItem.NoveltyId;
						returnableItem.RecommendId = orderItem.RecommendId;
						returnableItem.ProductOptionValue = orderItem.ProductOptionSettingSelectedTexts;
						returnableItem.ProductTaxIncludedFlg = orderItem.ProductTaxIncludedFlg;

						decimal productTaxRate;
						decimal.TryParse(orderItem.ProductTaxRate, out productTaxRate);
						returnableItem.ProductTaxRate = productTaxRate;
						returnableItem.ProductTaxRoundType = orderItem.ProductTaxRoundType;
						returnableItem.BrandId = orderItem.BrandId;
						returnableItem.DownloadUrl = orderItem.DownloadUrl;
						returnableItem.ProductBundleId = orderItem.ProductBundleId;
						returnableItem.BundleItemDisplayType = orderItem.BundleItemDisplayType;
						returnableItem.CooperationId1 = orderItem.CooperationId[0];
						returnableItem.CooperationId2 = orderItem.CooperationId[1];
						returnableItem.CooperationId3 = orderItem.CooperationId[2];
						returnableItem.CooperationId4 = orderItem.CooperationId[3];
						returnableItem.CooperationId5 = orderItem.CooperationId[4];
						returnableItem.CooperationId6 = orderItem.CooperationId[5];
						returnableItem.CooperationId7 = orderItem.CooperationId[6];
						returnableItem.CooperationId8 = orderItem.CooperationId[7];
						returnableItem.CooperationId9 = orderItem.CooperationId[8];
						returnableItem.CooperationId10 = orderItem.CooperationId[9];
						returnableItem.OrderSetPromotionNo = orderItem.OrderSetPromotionNo;
						returnableItem.FixedPurchaseProductFlg = orderItem.FixedPurchaseProductFlg;
						returnableItem.OrderShippingNo = orderItem.OrderShippingNo;

						var shipping = returnExchangeOrder.GetShipping(orderItem.OrderShippingNo);
						returnableItem.ShippingName = shipping.ShippingName;
						returnableItem.ShippingName1 = shipping.ShippingName1;
						returnableItem.ShippingName2 = shipping.ShippingName2;
						returnableItem.ShippingNameKana = shipping.ShippingNameKana;
						returnableItem.ShippingNameKana1 = shipping.ShippingNameKana1;
						returnableItem.ShippingNameKana2 = shipping.ShippingNameKana2;
						returnableItem.ShippingCountryIsoCode = shipping.ShippingCountryIsoCode;
						returnableItem.ShippingCountryName = shipping.ShippingCountryName;
						returnableItem.ShippingZip = shipping.ShippingZip;
						returnableItem.ShippingAddr1 = shipping.ShippingAddr1;
						returnableItem.ShippingAddr2 = shipping.ShippingAddr2;
						returnableItem.ShippingAddr3 = shipping.ShippingAddr3;
						returnableItem.ShippingAddr4 = shipping.ShippingAddr4;
						returnableItem.ShippingAddr5 = shipping.ShippingAddr5;
						returnableItem.ShippingCompanyName = shipping.ShippingCompanyName;
						returnableItem.ShippingCompanyPostName = shipping.ShippingCompanyPostName;
						returnableItem.ShippingTel1 = shipping.ShippingTel1;
						returnableItem.ShippingCheckNo = shipping.ShippingCheckNo;
						returnableItem.SenderName = shipping.SenderName;
						returnableItem.SenderNameKana = shipping.SenderNameKana;
						returnableItem.SenderCountryIsoCode = shipping.SenderCountryIsoCode;
						returnableItem.SenderCountryName = shipping.SenderCountryName;
						returnableItem.SenderZip = shipping.SenderZip;
						returnableItem.SenderAddr1 = shipping.SenderAddr1;
						returnableItem.SenderAddr2 = shipping.SenderAddr2;
						returnableItem.SenderAddr3 = shipping.SenderAddr3;
						returnableItem.SenderAddr4 = shipping.SenderAddr4;
						returnableItem.SenderAddr5 = shipping.SenderAddr5;
						returnableItem.SenderCompanyName = shipping.SenderCompanyName;
						returnableItem.SenderCompanyPostName = shipping.SenderCompanyPostName;
						returnableItem.SenderTel1 = shipping.SenderTel1;
						returnableItem.WrappingPaperType = shipping.WrappingPaperType;
						returnableItem.WrappingPaperName = shipping.WrappingPaperName;
						returnableItem.WrappingBagType = shipping.WrappingBagType;
						returnableItem.SubscriptionBoxCourseId = orderItem.SubscriptionBoxCourseId;
						returnableItem.SubscriptionBoxFixedAmount = orderItem.SubscriptionBoxFixedAmount;

						returnItems.Add(returnableItem);
					}
				}
			}

			// 作成した返品可能商品リストについて、返品済みかどうか判断する
			foreach (OrderItem oi in returnExchangeOrder)
			{
				// 表示対象外の注文IDについて処理する(必ず返品商品)
				if (oi.OrderId == this.OrderId) continue;

				// 注文数分ループ(注文数がマイナスの場合は、プラスに変更)
				foreach (var i in Enumerable.Range(0, Math.Abs(int.Parse(oi.ItemQuantity))))
				{
					// 既にリストにある商品と比較して、返品商品か判断する
					foreach (var item in returnItems)
					{
						// 返品対象外・交換元ではない場合
						if (item.ReturnStatus != Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE
							&& item.ReturnStatus != Constants.FLG_ORDER_RETURN_STATUS_RETURN_EXCHANGED)
						{
							// ショップID、商品ID、バリエーションID、商品単価が一致する場合、返品済み商品とする。
							// なおかつ、配送先番号が一致する場合、返品済み商品とする。（ギフト対応）
							if ((oi.ShopId == item.ShopId)
								&& (oi.ProductId == item.ProductId)
								&& ((oi.VId == item.VId)
								&& (decimal.Parse(oi.ProductPrice) == item.ProductPrice)
								&& (oi.OrderShippingNo == item.OrderShippingNo)))
							{
								item.ReturnStatus = Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE;
								break;
							}
						}
					}
				}
			}

			// プロパティにセット
			this.Items = returnItems;

			foreach (var item in returnItems)
			{
				if (shippingNumbers.Add(item.OrderShippingNo))
				{
					this.Shippings.Add(item);
				}
			}
		}

		/// <summary>
		/// ポイント情報セット(付与ポイント情報プロパティ、関連する表示可否プロパティをセット）
		/// </summary>
		/// <param name="returnExchangeOrder">返品交換対象の注文情報（関連注文の返品商品含む）</param>
		private void SetOrderPoint(Order returnExchangeOrder)
		{
			// 表示初期化
			this.VisibleOrderPointAddTemp = false;
			this.VisibleOrderPointAddComp = false;
			this.VisibleOrderPointAdd = true;
			this.VisibleOrderPointUse = true;

			// ユーザポイント情報取得
			var userPoint = GetUserPointByOrderId(returnExchangeOrder.UserId, returnExchangeOrder.OrderId);
			var userPointList = new UserPointList(userPoint);

			// 仮ポイント情報セット
			this.UserPointTemp = new UserPointList(userPointList.UserPointTemp);

			// ユーザポイント(仮ポイント)が存在する場合は仮ポイント情報を表示
			if (this.UserPointTemp.Items.Count > 0)
			{
				this.VisibleOrderPointAddTemp = true;
			}
			// 仮ポイントが存在せず、付与ポイントが0でなければ本ポイント移行済み
			else if (returnExchangeOrder.OrderPointAdd != "0")
			{
				// 本ポイント情報を表示
				this.VisibleOrderPointAddComp = true;

				var userHistoryModels = new PointService().GetUserPointHistories(returnExchangeOrder.UserId);
				// ユーザーポイント履歴から通常 本ポイントを取得しセット
				this.OrderBasePointAddComp = PointOptionUtility.GetOrderPointAdd(
					userHistoryModels,
					Constants.FLG_USERPOINT_POINT_KBN_BASE,
					returnExchangeOrder.OrderId);

				// ユーザーポイント履歴から期間限定 本ポイントを取得しセット
				this.OrderLimitPointAddComp = PointOptionUtility.GetOrderPointAdd(
					userHistoryModels,
					Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT,
					returnExchangeOrder.OrderId);
			}
			// ポイントが付与されていない？
			else
			{
				this.VisibleOrderPointAdd = false;
			}

			// 利用ポイント
			var user = new UserService().Get(returnExchangeOrder.UserId);
			this.VisibleOrderPointUse = user.IsMember;
			this.LastOrderPointUse = decimal.Parse(returnExchangeOrder.RelatedOrderLastOrderPointUse);

		}

		/// <summary>
		/// 注文合計情報セット(注文合計情報プロパティをセット）
		/// </summary>
		/// <param name="returnExchangeOrder">返品交換対象の注文情報（関連注文の返品商品含む）</param>
		private void SetOrderSummaryInfo(Order returnExchangeOrder)
		{
			var dict = new Dictionary<string, string>
			{
				{ "OrderPriceSubtotal", returnExchangeOrder.OrderPriceSubtotal },
				{ "OrderPriceShipping", returnExchangeOrder.OrderPriceShipping },
				{ "MemberRankDiscount", (-decimal.Parse(returnExchangeOrder.MemberRankDiscount)).ToString() },
				{ "FixedPurchaseMemberDiscountAmount", (-decimal.Parse(returnExchangeOrder.FixedPurchaseMemberDiscountAmount)).ToString() },
				{ "OrderPointUseYen", (-Math.Abs(decimal.Parse(returnExchangeOrder.OrderPointUseYen))).ToString() },
				{ "OrderCouponUse", (-Math.Abs(decimal.Parse(returnExchangeOrder.OrderCouponUse))).ToString() },
				{ "OrderPriceExchange", returnExchangeOrder.OrderPriceExchange },
				{ "OrderPriceTotal", returnExchangeOrder.OrderPriceTotal },
				{ "OrderPriceTax", decimal.Parse(returnExchangeOrder.OrderPriceTax).ToString() },
				{ "OrderPriceSubtotalTax", decimal.Parse(returnExchangeOrder.OrderPriceSubtotalTax).ToString() },
				{ "ShippingTaxRate", decimal.Parse(returnExchangeOrder.ShippingTaxRate).ToString() },
				{ "PaymentTaxRate", decimal.Parse(returnExchangeOrder.PaymentTaxRate).ToString() },
				{ "OrderPriceRegulation", returnExchangeOrder.OrderPriceRegulation }, // 調整金額は交換済み注文のみで使用
				{ "FixedPurchaseDiscountPrice", (-decimal.Parse(returnExchangeOrder.FixedPurchaseDiscountPrice)).ToString() },
			};

			//プロパティにセット
			this.OrderSummaryInfo = dict;
		}

		/// <summary>
		/// 税率毎価格情報セット
		/// </summary>
		/// <param name="returnExchangeOrder">返品交換対象の注文情報（関連注文の返品商品含む）</param>
		private void SetOrderPriceByTaxRate(Order returnExchangeOrder)
		{
			this.OrderPriceByTaxRate = returnExchangeOrder.OrderPriceByTaxRate;
		}

		/// <summary>
		/// ユーザーポイント情報取得(本・仮ポイント)
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="strOrderId">注文ID</param>
		/// <returns>ユーザーポイント情報取得</returns>
		private UserPointModel[] GetUserPointByOrderId(string strUserId, string strOrderId)
		{
			var sv = new PointService();
			var model = sv.GetUserPoint(strUserId, string.Empty)
				.Where(x => ((x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP) || (x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP && x.Kbn1 == strOrderId)))
				.ToArray();
			return model;
		}

		/// <summary>
		/// 割引後商品価格を１商品毎に按分計算
		/// </summary>
		/// <param name="discountedPrice">割引後商品価格</param>
		/// <param name="prodoctCount">現在の商品</param>
		/// <param name="itemQuantity">商品数</param>
		/// <returns>１商品毎の割引後価格</returns>
		private decimal CaliculateDiscountedPriceFracution(decimal discountedPrice, int prodoctCount, int itemQuantity)
		{
			if (itemQuantity == 1) return discountedPrice;
			var price = RoundingCalculationUtility.GetRoundCartDiscountPrice(discountedPrice, itemQuantity);

			// 一商品あたりの割引後価格が割り切れる、または現在の商品が最後でなければ重み付けしていない価格を返す。
			if ((discountedPrice == (price * itemQuantity)) || ((prodoctCount + 1) != itemQuantity))
			{
				return price;
			}

			// 商品数が１以上の時は最後の商品に重み付け
			var weightproductprice = price + (discountedPrice - (price * itemQuantity));
			return weightproductprice;
		}

		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>注文ステータス</summary>
		public string OrderStatus { get; set; }
		/// <summary>商品情報</summary>
		public List<ReturnOrderItem> Items { get; set; }
		/// <summary>配送先情報</summary>
		public List<ReturnOrderItem> Shippings = new List<ReturnOrderItem>();
		/// <summary>税率毎価格情報</summary>
		public List<OrderPriceByTaxRateModel> OrderPriceByTaxRate { get; set; }
		/// <summary>付与ポイント情報(仮ポイント)</summary>
		public UserPointList UserPointTemp { get; set; }
		/// <summary>付与ポイント情報（仮ポイント）表示可否</summary>
		public bool VisibleOrderPointAddTemp { get; set; }
		/// <summary>付与通常ポイント情報（本ポイント）</summary>
		public decimal OrderBasePointAddComp { get; set; }
		/// <summary>付与期間限定ポイント情報（本ポイント）</summary>
		public decimal OrderLimitPointAddComp { get; set; }
		/// <summary>付与ポイント情報（本ポイント）表示可否</summary>
		public bool VisibleOrderPointAddComp { get; set; }
		/// <summary>付与ポイント表示可否</summary>
		public bool VisibleOrderPointAdd { get; set; }
		/// <summary>最終利用ポイント数</summary>
		public decimal LastOrderPointUse { get; set; }
		/// <summary>付与ポイント表示可否</summary>
		public bool VisibleOrderPointUse { get; set; }
		/// <summary>注文合計情報</summary>
		public Dictionary<string, string> OrderSummaryInfo { get; set; }
		/// <summary>表示可否(交換済み注文)</summary>
		public bool VisibleExchangedOrder
		{
			get
			{
				return ((this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
					|| (this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP));
			}
		}
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>頒布会コース定額価格</summary>
		public decimal? SubscriptionBoxFixedAmount { get; set; }
		/// <summary>頒布会定額コースか</summary>
		public bool IsSunscriptionBoxFixedAmount
		{
			get
			{
				if (string.IsNullOrEmpty(this.SubscriptionBoxCourseId)) return false;
				return (this.SubscriptionBoxFixedAmount.HasValue) && (this.SubscriptionBoxFixedAmount.Value != 0);
			}
		}
		/// <summary>頒布会購入数</summary>
		public int? OrderSubscriptionBoxOrderCount { get; set; }
	}
}
