/*
=========================================================================================================
  Module      : 注文取込クラス(OrderImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.ProductStock;
using w2.Domain.ProductStockHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.MallCooperation;
using w2.Commerce.Batch.LiaiseAmazonMall.Amazon;
using w2.Commerce.Batch.LiaiseAmazonMall.Util;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.Domain.ProductTaxCategory;
using OrderItem = MarketplaceWebServiceOrders.Model.OrderItem;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Import
{
	/// <summary>
	/// 注文取込クラス
	/// </summary>
	public partial class OrderImport : ImportBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="amazonOrder">Amazon注文情報</param>
		/// <param name="orderItem">Amazon注文商品情報</param>
		/// <param name="mallId">モールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public OrderImport(AmazonOrderModel amazonOrder, string mallId, SqlAccessor accessor)
			: base(amazonOrder, mallId, accessor)
		{
		}
		#endregion

		#region +Import 取込
		/// <summary>
		/// 取込
		/// </summary>
		public override void Import()
		{
			// ユーザID設定
			this.AmazonOrder.UserId = SetUserId();

			// 注文情報作成(取込済み注文をキャンセルする場合は、必要項目のみ更新する)
			this.Order = ((this.AmazonOrder.Order.OrderStatus == Constants.AMAZON_MALL_ORDER_STATUS_CANCELED) && this.IsImportedOrder)
				? CreateCancelUpdateData()
				: CreateImportData();

			// DB登録
			var orderService = new OrderService();
			if (this.IsImportedOrder == false)
			{
				orderService.InsertOrder(this.Order, UpdateHistoryAction.Insert, this.Accessor);
			}
			else
			{
				orderService.UpdateForModify(this.Order, Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.Insert, this.Accessor);
				orderService.UpdateOwnerForModify(this.Order.Owner, Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.Insert, this.Accessor);
				orderService.UpdateShippingForModify(this.Order.Shippings, Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.Insert, this.Accessor);
				orderService.UpdateItemForModify(this.Order.Items, Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.Insert, this.Accessor);
			}

			// 在庫情報更新、在庫履歴登録(新規注文、またはキャンセル注文の場合)
			if ((this.IsImportedOrder == false) || (this.IsImportedOrder && this.AmazonOrder.Order.OrderStatus == Constants.AMAZON_MALL_ORDER_STATUS_CANCELED))
			{
				foreach (var orderItem in this.Order.Items)
				{
					// 在庫管理しない商品の場合はスキップ
					if (orderItem.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED) continue;

					var stockUpdateResult = UpdateProductStock(orderItem);
					InsertProductHistory(orderItem);

					// 新規注文取込時にマイナス在庫が発生した場合はワーニング出力
					if ((stockUpdateResult < 0) && (this.AmazonOrder.Order.OrderStatus != Constants.AMAZON_MALL_ORDER_STATUS_CANCELED))
					{
						WarnMinusStock(orderItem, stockUpdateResult);
					}
				}
			}
		}
		#endregion

		#region -CreateImportData 取込データ作成
		/// <summary>
		/// 取込データ作成
		/// </summary>
		/// <returns>注文情報モデル</returns>
		private OrderModel CreateImportData()
		{
			var amazonShippingPrice = this.AmazonOrder.OrderItem.Sum(
				item => (item.IsSetShippingPrice()) ? Convert.ToDecimal(item.ShippingPrice.Amount) : 0);
			var amazonPaymentPrice = this.AmazonOrder.OrderItem.Sum(
				item => (item.IsSetCODFee()) ? Convert.ToDecimal(item.CODFee.Amount) : 0);
			var orderItems = CreateOrderItems();
			var shipping = new[] { CreateOrderShipping() };
			shipping.First().Items = orderItems;
			// リアルタイム累計購入回数取得
			var user = new UserService().Get(this.AmazonOrder.UserId);
			var orderCount = ((user == null) ? 0 : user.OrderCountOrderRealtime);
			var order = new OrderModel
			{
				OrderId = this.AmazonOrder.Order.AmazonOrderId,
				UserId = this.AmazonOrder.UserId,
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				OrderKbn = Constants.FLG_ORDER_ORDER_KBN_PC,
				MallId = this.MallId,
				OrderPaymentKbn = ConvertOrderPaymentKbn(),
				OrderStatus = SetOrderStatus(),
				OrderDate = this.AmazonOrder.Order.PurchaseDate,
				OrderItemCount = this.AmazonOrder.OrderItem.Count,
				OrderProductCount = (int)this.AmazonOrder.OrderItem.Sum(item => (item.IsSetQuantityOrdered()) ? item.QuantityOrdered : 0),
				OrderPriceShipping = amazonShippingPrice,
				OrderPriceExchange = amazonPaymentPrice,
				OrderPriceRegulation = SetOrderPriceRegulation(),
				OrderPriceTotal = SetOrderPriceTotal(),
				CardKbn = string.Empty,
				CardTranId = string.Empty,
				CardInstruments = string.Empty,
				CardInstallmentsCode = string.Empty,
				ShippingId = Constants.LIAISE_AMAZON_MALL_DEFAULT_SHIPPING_ID,
				FixedPurchaseId = string.Empty,
				AdvcodeFirst = string.Empty,
				AdvcodeNew = string.Empty,
				CareerId = string.Empty,
				MobileUid = string.Empty,
				RemoteAddr = string.Empty,
				Memo = (this.IsImportedOrder) ? this.AmazonOrder.ImportedOrder.Memo : string.Empty,
				WrappingMemo = (this.IsImportedOrder) ? this.AmazonOrder.ImportedOrder.WrappingMemo : string.Empty,
				PaymentMemo = (this.IsImportedOrder) ? this.AmazonOrder.ImportedOrder.PaymentMemo : string.Empty,
				ManagementMemo = (this.IsImportedOrder) ? this.AmazonOrder.ImportedOrder.ManagementMemo : string.Empty,
				RelationMemo = SetRelationMemo(),
				ReturnExchangeReasonMemo = string.Empty,
				RegulationMemo = SetRegulationMemo(),
				RepaymentMemo = string.Empty,
				DateCreated = (this.IsImportedOrder) ? this.AmazonOrder.ImportedOrder.DateCreated : DateTime.Now,
				DateChanged = DateTime.Now,
				LastChanged = Constants.FLG_LASTCHANGED_BATCH,
				MemberRankId = (this.IsImportedOrder) ? this.AmazonOrder.ImportedOrder.MemberRankId : this.AmazonOrder.DefaultMemberRankId,
				GiftFlg = (this.AmazonOrder.OrderItem.Where(a => a.IsSetGiftWrapLevel()).Count() > 0)
					? Constants.FLG_ORDER_GIFT_FLG_ON
					: Constants.FLG_ORDER_GIFT_FLG_OFF,
				PaymentOrderId = string.Empty,
				CombinedOrgOrderIds = string.Empty,
				LastBilledAmount = SetOrderPriceTotal(),
				ExternalPaymentErrorMessage = string.Empty,
				ExternalOrderId = string.Empty,
				ExternalImportStatus = (this.IsImportedOrder) ? this.AmazonOrder.ImportedOrder.ExternalImportStatus : Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_SUCCESS,
				LastAuthFlg = null,
				MallLinkStatus = SetMallLinkStatus(),
				OrderTaxIncludedFlg = TaxCalculationUtility.GetPrescribedOrderTaxIncludedFlag(),
				OrderTaxRoundType = Constants.TAX_ROUNDTYPE,
				Items = orderItems,
				Owner = CreateOrderOwner(),
				Shippings = shipping,
				ShippingTaxRate = Constants.CONST_SHIPPING_TAXRATE,
				PaymentTaxRate = Constants.CONST_PAYMENT_TAXRATE,
				OrderCountOrder = orderCount + 1,
			};
			// 商品価格、税額は商品情報を取得してから再計算する
			order.OrderPriceSubtotal = order.Items.Sum(item => item.ItemPrice);
			order.OrderPriceSubtotalTax = order.Items.Sum(item => item.ItemPriceTax);
			order.OrderPriceByTaxRates = OrderCommon.CreateOrderPriceByTaxRate(order);
			order.OrderPriceTax = order.OrderPriceByTaxRates.Sum(priceByTaxRate => priceByTaxRate.TaxPriceByRate);

			return order;
		}
		#endregion

		#region -CreateOrderOwner 注文者情報作成
		/// <summary>
		/// 注文者情報作成
		/// </summary>
		/// <returns>注文者情報</returns>
		private OrderOwnerModel CreateOrderOwner()
		{
			var owner = new OrderOwnerModel
			{
				OrderId = this.AmazonOrder.Order.AmazonOrderId,
				OwnerKbn = SetOwnerKbn(),
				OwnerName = this.AmazonOrder.Order.IsSetBuyerName() ? this.AmazonOrder.Order.BuyerName : string.Empty,
				OwnerNameKana = string.Empty,
				OwnerMailAddr = this.AmazonOrder.Order.IsSetBuyerEmail() ? this.AmazonOrder.Order.BuyerEmail : string.Empty,
				OwnerZip = string.Empty,
				OwnerAddr1 = string.Empty,
				OwnerAddr2 = string.Empty,
				OwnerAddr3 = string.Empty,
				OwnerAddr4 = string.Empty,
				OwnerTel1 = string.Empty,
				OwnerTel2 = string.Empty,
				OwnerTel3 = string.Empty,
				OwnerFax = string.Empty,
				OwnerCompanyName = string.Empty,
				OwnerCompanyPostName = string.Empty,
				OwnerCompanyExectiveName = string.Empty,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				OwnerName1 = string.Empty,
				OwnerName2 = string.Empty,
				OwnerNameKana1 = string.Empty,
				OwnerNameKana2 = string.Empty,
				OwnerMailAddr2 = string.Empty,
			};
			return owner;
		}
		#endregion

		#region -CreateOrderShipping 配送先情報作成
		/// <summary>
		/// 配送先情報作成
		/// </summary>
		/// <returns>配送先情報</returns>
		private OrderShippingModel CreateOrderShipping()
		{
			var shipping = new OrderShippingModel
			{
				OrderId = this.AmazonOrder.Order.AmazonOrderId,
				OrderShippingNo = 1,
				ShippingName = 
					(this.AmazonOrder.Order.IsSetShippingAddress() && this.AmazonOrder.Order.ShippingAddress.IsSetName())
					? this.AmazonOrder.Order.ShippingAddress.Name : string.Empty,
				ShippingNameKana = string.Empty,
				ShippingZip = 
					(this.AmazonOrder.Order.IsSetShippingAddress() && this.AmazonOrder.Order.ShippingAddress.IsSetPostalCode())
					? this.AmazonOrder.Order.ShippingAddress.PostalCode : string.Empty,
				ShippingAddr1 = 
					(this.AmazonOrder.Order.IsSetShippingAddress() && this.AmazonOrder.Order.ShippingAddress.IsSetStateOrRegion())
					? this.AmazonOrder.Order.ShippingAddress.StateOrRegion : string.Empty,
				ShippingAddr2 = 
					(this.AmazonOrder.Order.IsSetShippingAddress() && this.AmazonOrder.Order.ShippingAddress.IsSetAddressLine1())
					? this.AmazonOrder.Order.ShippingAddress.AddressLine1 : string.Empty,
				ShippingAddr3 =
					(this.AmazonOrder.Order.IsSetShippingAddress() && this.AmazonOrder.Order.ShippingAddress.IsSetAddressLine2())
					? this.AmazonOrder.Order.ShippingAddress.AddressLine2 : string.Empty,
				ShippingAddr4 =
					(this.AmazonOrder.Order.IsSetShippingAddress() && this.AmazonOrder.Order.ShippingAddress.IsSetAddressLine3())
					? this.AmazonOrder.Order.ShippingAddress.AddressLine3 : string.Empty,
				ShippingTel1 = 
					(this.AmazonOrder.Order.IsSetShippingAddress() && this.AmazonOrder.Order.ShippingAddress.IsSetPhone())
					? this.AmazonOrder.Order.ShippingAddress.Phone : string.Empty,
				ShippingTel2 = string.Empty,
				ShippingTel3 = string.Empty,
				ShippingFax = string.Empty,
				ShippingDate = this.AmazonOrder.Order.IsSetLatestDeliveryDate() ? this.AmazonOrder.Order.LatestDeliveryDate : (DateTime?)null,
				ShippingTime = SetShippingTime(),
				ShippingCheckNo = string.Empty,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				ShippingName1 = string.Empty,
				ShippingName2 = string.Empty,
				ShippingNameKana1 = string.Empty,
				ShippingNameKana2 = string.Empty,
				SenderName = string.Empty,
				SenderName1 = string.Empty,
				SenderName2 = string.Empty,
				SenderNameKana = string.Empty,
				SenderNameKana1 = string.Empty,
				SenderNameKana2 = string.Empty,
				SenderZip = string.Empty,
				SenderAddr1 = string.Empty,
				SenderAddr2 = string.Empty,
				SenderAddr3 = string.Empty,
				SenderAddr4 = string.Empty,
				SenderTel1 = string.Empty,
				WrappingPaperType = string.Empty,
				WrappingPaperName = string.Empty,
				WrappingBagType = string.Empty,
				ShippingCompanyName = string.Empty,
				ShippingCompanyPostName = string.Empty,
				SenderCompanyName = string.Empty,
				SenderCompanyPostName = string.Empty,
				AnotherShippingFlg = Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID,
				ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS,
				DeliveryCompanyId = this.AmazonOrder.DefaultDeliveryCompanyId
			};
			return shipping;
		}
		#endregion

		#region -CreateOrderItems 注文商品情報リスト作成
		/// <summary>
		/// 注文商品情報リスト作成
		/// </summary>
		/// <returns>注文商品情報リスト</returns>
		private OrderItemModel[] CreateOrderItems()
		{
			var orderItemList = this.AmazonOrder.OrderItem.Select((item, index) => CreateOrderItem(item, index + 1)).ToArray();
			return orderItemList;
		}
		#endregion

		#region -CreateOrderItem
		/// <summary>
		/// 注文商品情報作成
		/// </summary>
		/// <param name="amazonOrderItem">Amazon注文商品情報</param>
		/// <param name="itemNo">注文商品枝番</param>
		/// <returns>注文商品情報</returns>
		private OrderItemModel CreateOrderItem(OrderItem amazonOrderItem, int itemNo)
		{
			// Amazon出品商品情報取得
			var product = GetProductByAmazonSKU(amazonOrderItem.SellerSKU);

			// 商品税率カテゴリマスタモデル取得
			var productTaxCategory = new ProductTaxCategoryService().Get(product.TaxCategoryId);

			// キャンセル注文の場合は、取込済みの注文情報を再INSERTする（キャンセル注文の場合は、注文数量や単価が設定されていないため）
			if (this.IsImportedOrder && (this.AmazonOrder.Order.OrderStatus == Constants.AMAZON_MALL_ORDER_STATUS_CANCELED))
			{
				var importedOrderItem = this.AmazonOrder.ImportedOrder.Items.FirstOrDefault(item => item.OrderItemNo == itemNo);
				importedOrderItem.StockManagementKbn = product.StockManagementKbn;
				return importedOrderItem;
			}
			var amazonProductPrice = (amazonOrderItem.IsSetItemPrice() && amazonOrderItem.ItemPrice.IsSetAmount())
				? ConvertUtility.ConvertStringToDecimal(amazonOrderItem.ItemPrice.Amount) / amazonOrderItem.QuantityOrdered
				: 0;
			var productTaxPrice = TaxCalculationUtility.GetTaxPrice(
				amazonProductPrice,
				productTaxCategory.TaxRate,
				Constants.TAX_ROUNDTYPE,
				true);
			var itemPrice = ((amazonOrderItem.IsSetItemPrice() && amazonOrderItem.ItemPrice.IsSetAmount())
				? ConvertUtility.ConvertStringToDecimal(amazonOrderItem.ItemPrice.Amount)
				: 0);
			var itemPriceTax = productTaxPrice * amazonOrderItem.QuantityOrdered;

			var orderItem = new OrderItemModel
			{
				OrderId = this.AmazonOrder.Order.AmazonOrderId,
				OrderItemNo = itemNo,
				OrderShippingNo = 1,
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				ProductId = product.ProductId,
				VariationId = product.VariationId,
				SupplierId = string.Empty,
				ProductName = amazonOrderItem.IsSetTitle() ? amazonOrderItem.Title : string.Empty,
				ProductNameKana = string.Empty,
				ProductPrice = TaxCalculationUtility.GetPrescribedPrice(amazonProductPrice, productTaxPrice, true),
				ProductPriceOrg = TaxCalculationUtility.GetPrescribedPrice(amazonProductPrice, productTaxPrice, true),
				ProductPricePretax = amazonProductPrice,
				ItemQuantity = amazonOrderItem.IsSetQuantityOrdered() ? (int)amazonOrderItem.QuantityOrdered : 0,
				ItemQuantitySingle = amazonOrderItem.IsSetQuantityOrdered() ? (int)amazonOrderItem.QuantityOrdered : 0,
				ItemPrice = TaxCalculationUtility.GetPrescribedPrice(itemPrice, itemPriceTax, true),
				ItemPriceSingle = TaxCalculationUtility.GetPrescribedPrice(itemPrice, itemPriceTax, true),
				ItemPriceTax = itemPriceTax,
				ProductSetId = string.Empty,
				ItemMemo = string.Empty,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				ProductOptionTexts = string.Empty,
				BrandId = string.Empty,
				DownloadUrl = string.Empty,
				ProductsaleId = string.Empty,
				// 商品連携IDは後から設定
				CooperationId1 = string.Empty,
				CooperationId2 = string.Empty,
				CooperationId3 = string.Empty,
				CooperationId4 = string.Empty,
				CooperationId5 = string.Empty,
				CooperationId6 = string.Empty,
				CooperationId7 = string.Empty,
				CooperationId8 = string.Empty,
				CooperationId9 = string.Empty,
				CooperationId10 = string.Empty,
				NoveltyId = string.Empty,
				RecommendId = string.Empty,
				ProductBundleId = string.Empty,
				ColumnForMallOrder = amazonOrderItem.IsSetOrderItemId() ? amazonOrderItem.OrderItemId : string.Empty,
				GiftWrappingId = amazonOrderItem.IsSetGiftWrapLevel() ? amazonOrderItem.GiftWrapLevel : string.Empty,
				GiftMessage = amazonOrderItem.IsSetGiftMessageText() ? amazonOrderItem.GiftMessageText : string.Empty,
				ProductTaxRate = productTaxCategory.TaxRate,
				ProductTaxRoundType = Constants.TAX_ROUNDTYPE,
				ProductTaxIncludedFlg = TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag()
			};
			// 商品連携ID設定
			orderItem = SetOrderItemCooperationId(orderItem, product, amazonOrderItem);
			
			// 在庫数反映処理判定用に保持する
			orderItem.StockManagementKbn = product.StockManagementKbn;
			return orderItem;
		}
		#endregion

		#region -CreateOrderPriceByTaxRate 税率毎価格情報作成
		/// <summary>
		/// 税率毎価格情報作成
		/// </summary>
		/// <param name="order">受注情報</param>
		/// <returns>税率毎価格情報</returns>
		private static OrderPriceByTaxRateModel[] CreateOrderPriceByTaxRate(OrderModel order)
		{
			var stackedDiscountAmount = 0m;
			var priceTotal = order.Items.Sum(item => item.ProductPricePretax * item.ItemQuantity);
			// 調整金額適用対象金額取得
			var shippingPrice = order.OrderPriceShipping;
			var paymentPrice = order.OrderPriceExchange;
			priceTotal += paymentPrice;
			priceTotal += shippingPrice;

			var itemInfo = new List<Hashtable>();
			if (priceTotal != 0)
			{
				itemInfo.AddRange(order.Items.Select(
					item => new Hashtable
					{
						{ "itemPriceRegulation", Math.Floor((item.ProductPricePretax * item.ItemQuantity) / priceTotal * order.OrderPriceRegulation) },
						{ "item", item}
					}));
				stackedDiscountAmount = itemInfo.Sum(item => (decimal)item["itemPriceRegulation"]);
			}
			var shippingRegulationPrice = Math.Floor(shippingPrice / priceTotal * order.OrderPriceRegulation);
			stackedDiscountAmount += shippingRegulationPrice;

			var paymentRegulationPrice = Math.Floor(paymentPrice / priceTotal * order.OrderPriceRegulation);
			stackedDiscountAmount += paymentRegulationPrice;

			var fractionAmount = order.OrderPriceRegulation - stackedDiscountAmount;
			
			if (fractionAmount != 0)
			{
				var weightItem = itemInfo.FirstOrDefault(
					item => (((OrderItemModel)item["item"]).ProductPricePretax
						* ((OrderItemModel)item["item"]).ItemQuantity) > 0);
				if (weightItem != null)
				{
					weightItem["itemPriceRegulation"] = (decimal)weightItem["itemPriceRegulation"] + fractionAmount;
				}
				else if (shippingPrice != 0)
				{
					shippingRegulationPrice += fractionAmount;
				}
				else
				{
					paymentRegulationPrice += fractionAmount;
				}
			}

			var priceInfo = new List<Hashtable>();
			// 税率毎の購入金額を算出する
			priceInfo.AddRange(itemInfo
				.Select(item => new Hashtable
				{
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , ((OrderItemModel)item["item"]).ProductTaxRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 
						(((OrderItemModel)item["item"]).ProductPricePretax * ((OrderItemModel)item["item"]).ItemQuantity) + (decimal)item["itemPriceRegulation"] },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
				}).ToList());

			priceInfo.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , order.ShippingTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , shippingPrice + shippingRegulationPrice },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
			});
			priceInfo.Add(new Hashtable
			{ 
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , order.PaymentTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , paymentPrice + paymentRegulationPrice },
			});

			var groupedItem = priceInfo.GroupBy(item => new
			{
				taxRate = (decimal)item[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]
			});
			var priceByTaxRate = groupedItem.Select(
				item => new OrderPriceByTaxRateModel
				{
					KeyTaxRate = item.Key.taxRate,
					PriceSubtotalByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]),
					PriceShippingByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]),
					PricePaymentByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]),
					PriceTotalByRate = item.Sum(itemKey =>
						((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
					TaxPriceByRate = TaxCalculationUtility.GetTaxPrice(item.Sum(itemKey =>
							((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
						(decimal)item.Key.taxRate,
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
						true)
				}).ToArray();
			foreach (var orderPriceByTaxRateModel in priceByTaxRate)
			{
				orderPriceByTaxRateModel.OrderId = order.OrderId;
			}
			return priceByTaxRate;
		}
		#endregion

		#region -UpdateProductStock 在庫数更新
		/// <summary>
		/// 在庫数更新
		/// </summary>
		/// <param name="orderItem">注文商品情報</param>
		/// <returns>更新後在庫数</returns>
		private int UpdateProductStock(OrderItemModel orderItem)
		{
			var itemQuantity = (this.AmazonOrder.Order.OrderStatus == Constants.AMAZON_MALL_ORDER_STATUS_CANCELED)
				? orderItem.ItemQuantity * (-1)
				: orderItem.ItemQuantity;
			var result = new ProductStockService().UpdateProductStockAndGetStock(
				Constants.CONST_DEFAULT_SHOP_ID,
				orderItem.ProductId,
				orderItem.VariationId,
				itemQuantity,
				Constants.FLG_LASTCHANGED_BATCH,
				this.Accessor);
			return result.Stock;
		}
		#endregion

		#region -InsertProductHistory 商品在庫履歴登録
		/// <summary>
		/// 商品在庫履歴登録
		/// </summary>
		/// <param name="orderItem">注文商品情報</param>
		private void InsertProductHistory(OrderItemModel orderItem)
		{
			var model = new ProductStockHistoryModel
			{
				OrderId = orderItem.OrderId,
				ShopId = orderItem.ShopId,
				ProductId = orderItem.ProductId,
				VariationId = orderItem.VariationId,
				ActionStatus = (this.AmazonOrder.Order.OrderStatus != Constants.AMAZON_MALL_ORDER_STATUS_CANCELED)
					? Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER
					: Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL,
				AddStock = (this.AmazonOrder.Order.OrderStatus != Constants.AMAZON_MALL_ORDER_STATUS_CANCELED)
					? orderItem.ItemQuantity * (-1)
					: orderItem.ItemQuantity,
				UpdateMemo = string.Empty,
				DateCreated = DateTime.Now,
				LastChanged = Constants.FLG_LASTCHANGED_BATCH
			};
			new ProductStockHistoryService().Insert(model);
		}
		#endregion

		#region -WarnMinusStock マイナス在庫警告出力
		/// <summary>
		/// マイナス在庫警告出力
		/// </summary>
		/// <param name="orderItem">マイナス在庫になった注文商品情報</param>
		private void WarnMinusStock(OrderItemModel orderItem, int stock)
		{
			// 注文情報のステータスと管理メモにワーニング出力
			var warningMessage = string.Format(
				"Amazonからの受注情報取込処理時({0})、「商品ID：{1}」で在庫数が0未満({2})になりました。外部連携ステータス'異常'で取り込みました。",
				DateTime.Now,
				orderItem.ProductId,
				stock);
			this.Order.ExternalImportStatus = Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_ERROR;
			this.Order.ManagementMemo = string.IsNullOrEmpty(this.Order.ManagementMemo) ? warningMessage : string.Concat(this.Order.ManagementMemo, "\r\n", warningMessage);
			new OrderService().UpdateForModify(this.Order, Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.Insert, this.Accessor);

			// モール監視ログに出力
			new MallWatchingLogManager().Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
				this.MallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
				string.Format("注文ID：{0}[{1}]", this.Order.OrderId, warningMessage));
		}
		#endregion


		/// <summary>
		/// キャンセル更新データ作成
		/// </summary>
		/// <returns>注文情報モデル</returns>
		private OrderModel CreateCancelUpdateData()
		{
			var order = this.AmazonOrder.ImportedOrder;
			order.OrderStatus = SetOrderStatus();
			order.OrderCancelDate = DateTime.Now;
			order.Items = CreateOrderItems();
			order.Owner = this.AmazonOrder.ImportedOrder.Owner;
			order.Shippings = this.AmazonOrder.ImportedOrder.Shippings;

			// ユーザーリアルタイム累計購入回数処理
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, order.UserId},
				{Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, new UserService().Get(order.UserId).OrderCountOrderRealtime}
			};
			OrderCommon.UpdateRealTimeOrderCount(ht, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_CANCEL);

			return order;
		}

		#region プロパティ
		/// <summary>注文情報</summary>
		private OrderModel Order { get; set; }
		/// <summary>取込済み注文情報か</summary>
		private bool IsImportedOrder
		{
			get
			{
				return (this.AmazonOrder.ImportedOrder != null);
			}
		}
		#endregion
	}
}
