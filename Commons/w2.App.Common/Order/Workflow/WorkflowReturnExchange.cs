/*
=========================================================================================================
  Module      : WorkflowReturnExchange(ReturnExchange.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Api;
using w2.App.Common.Option;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// WorkflowReturnExchange
	/// </summary>
	public class WorkflowReturnExchange
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="loginOperatorDeptId">ログインオペレーターデプトID</param>
		public WorkflowReturnExchange(string loginOperatorName, string loginOperatorDeptId)
		{
			this.LoginOperatorName = loginOperatorName;
			this.LoginOperatorDeptId = loginOperatorDeptId;
		}

		/// <summary>
		/// Create Return Order New
		/// </summary>
		/// <param name="returnOrder">Return order</param>
		/// <param name="orderModelOld">Order model old</param>
		/// <param name="returnReasonKbn">Return reason kbn</param>
		/// <param name="returnReasonMemo">Return reason memo</param>
		/// <param name="cart">Cart</param>
		/// <param name="registerCardTranId">値登録向け・カード取引ID（登録する場合に値を格納する）</param>
		/// <param name="registerPaymentOrderId">値登録向け・決済注文ID（登録する場合に値を格納する）</param>
		/// <param name="registerExternalPaymentStatus">値登録向け・外部決済与信日時（登録する場合に値を格納する）</param>
		/// <param name="registerExternalPaymentAuthDate">値登録向け・外部決済ステータス（登録する場合に値を格納する）</param>
		/// <param name="registerOnlinePaymentStatus">値登録向け・オンライン決済ステータス（登録する場合に値を格納する）</param>
		/// <param name="registerPaymentMemo">値登録向け・決済連携メモ（登録する場合に値を格納する）</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Order new</returns>
		public Hashtable CreateReturnOrderNew(
			Order returnOrder,
			OrderModel orderModelOld,
			string returnReasonKbn,
			string returnReasonMemo,
			CartObject cart,
			string registerCardTranId,
			string registerPaymentOrderId,
			string registerExternalPaymentStatus,
			string registerExternalPaymentAuthDate,
			string registerOnlinePaymentStatus,
			string registerPaymentMemo,
			SqlAccessor accessor)
		{
			var order = CreateOrderInfo(orderModelOld, returnOrder, cart);
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				ProcessForReturnAddPointTmpAndComp(order, orderModelOld);
			}

			// 最大注文ID(枝番付き)取得＆注文ID(上書き)
			var orderId = new OrderService()
				.GetOrderIdForOrderReturnExchange(orderModelOld.OrderId, accessor);
			order[Constants.FIELD_ORDER_ORDER_ID] = orderId;
			order[Constants.FIELD_ORDER_ORDER_ID_ORG] = orderModelOld.OrderId;
			order[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN] = returnReasonKbn;
			order[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_MEMO] = returnReasonMemo;
			order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID] = orderModelOld.FixedPurchaseId;

			// Get order input
			new OrderExtend(
				registerCardTranId,
				registerPaymentOrderId,
				registerExternalPaymentStatus,
				registerExternalPaymentAuthDate,
				registerOnlinePaymentStatus,
				registerPaymentMemo)
				.GetOrderInputForReturnExchange(order, returnOrder, orderModelOld, true, this.LoginOperatorName, accessor);

			var returnAndExchangeItems = GetListReturnOrderItemInfo(returnOrder);

			var pointPriceByTaxRate = OrderCommon.CalculateAdjustmentPointPriceByTaxRate(orderModelOld.LastOrderPointUse * -1,
				returnAndExchangeItems,
			cart);

			var priceCorrectionByTaxRate = OrderCommon.CalculatePriceCorrectionByTaxRate(
				cart,
				returnOrder.OrderPriceByTaxRate,
				pointPriceByTaxRate,
				returnAndExchangeItems,
				returnOrder.ItemSubscriptionBoxFixedAmountCourseIds);

			foreach (var correctionPriceInfo in priceCorrectionByTaxRate)
			{
				var shippingPriceRegulation = (decimal.Parse(returnOrder.ShippingTaxRate) == correctionPriceInfo.KeyTaxRate)
					? decimal.Parse(returnOrder.OrderPriceShipping) * -1
					: 0m;

				var paymentPriceRegulation = (decimal.Parse(returnOrder.PaymentTaxRate) == correctionPriceInfo.KeyTaxRate)
					? decimal.Parse(returnOrder.OrderPriceExchange) * -1
					: 0m;

				correctionPriceInfo.ReturnPriceCorrectionByRate += shippingPriceRegulation + paymentPriceRegulation;
			}

			var orderPriceInfoByTaxRate = OrderCommon.CalculateReturnPriceInfoByTaxRate(
				GetListReturnOrderItemInfo(returnOrder),
				priceCorrectionByTaxRate,
				pointPriceByTaxRate,
				returnOrder.ItemSubscriptionBoxFixedAmountCourseIds);

			var priceTax = orderPriceInfoByTaxRate.Sum(priceByRate => priceByRate.TaxPriceByRate);
			order[Constants.FIELD_ORDER_ORDER_PRICE_TAX] = priceTax;
			order[Constants.TABLE_ORDERPRICEBYTAXRATE] = orderPriceInfoByTaxRate;
			return order;
		}

		/// <summary>
		/// Execute Regist Return Order
		/// </summary>
		/// <param name="orderNew">Order new</param>
		/// <param name="returnOrder">Return order</param>
		/// <param name="orderModelOld">Order model old</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql Accessor</param>
		/// <returns>Return order</returns>
		public void ExecuteRegistReturnOrder(
			Hashtable orderNew,
			Order returnOrder,
			OrderModel orderModelOld,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			List<ReturnOrderItem> exchangeOrderItems = null;
			var listReturnOrderItem = GetListReturnOrderItemInfo(returnOrder);

			// Execute regist order
			OrderCommon.OrderRegisterForReturnExchange(
				orderNew,
				returnOrder,
				UpdateHistoryAction.DoNotInsert,
				this.LoginOperatorName,
				accessor);

			// Execute regist owner
			OrderCommon.RegistOrderOwnerForReturnExchange(
				orderNew,
				UpdateHistoryAction.DoNotInsert,
				this.LoginOperatorName,
				accessor);

			// Execute regist order shipping and item
			ExecuteRegistOrderShippingAndItem(
				orderNew,
				returnOrder,
				orderModelOld,
				listReturnOrderItem,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// Execute regist order set promotion
			OrderCommon.RegistOrderSetPromotionForReturnExchange(
				orderNew,
				returnOrder,
				listReturnOrderItem,
				exchangeOrderItems,
				UpdateHistoryAction.DoNotInsert,
				this.LoginOperatorName,
				accessor);

			// 税率毎価格情報保存
			OrderCommon.ExecuteRegistOrderPriceInfoByTaxRate(
				orderNew,
				accessor);

			// Execute update shipped changed kbn
			OrderCommon.UpdateShippedChangedKbnForReturnExchange(
				returnOrder.OrderId,
				UpdateHistoryAction.DoNotInsert,
				this.LoginOperatorName,
				accessor);

			// Execute update coupon
			OrderCommon.UpdateCouponForReturnExchange(
				orderNew,
				returnOrder,
				this.LoginOperatorName,
				accessor);

			var pointErrorMessag = string.Empty;
			// Execute update point
			OrderCommon.UpdatePointForReturnExchange(
				orderNew,
				returnOrder.OrderId,
				returnOrder.UserId,
				UpdateHistoryAction.DoNotInsert,
				this.LoginOperatorName,
				accessor,
				out pointErrorMessag);

			// Update store pickup status return
			if (string.IsNullOrEmpty(orderModelOld.Shippings[0].StorePickupRealShopId) == false)
			{
				var storePickupInput = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, orderNew[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_LAST_CHANGED, this.LoginOperatorName },
					{ Constants.FIELD_ORDER_STOREPICKUP_STATUS, Constants.FLG_STOREPICKUP_STATUS_RETURNED },
					{ Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE, orderModelOld.StorePickupReturnDate }
				};
				DomainFacade.Instance.OrderService.UpdateStorePickupStatus(storePickupInput, accessor);
			}

			if (string.IsNullOrEmpty(pointErrorMessag) == false)
			{
				throw new Exception(pointErrorMessag);
			}

			// Update Related Order
			OrderCommon.UpdateRelatedOrder(returnOrder.OrderId, orderNew, UpdateHistoryAction.Insert, this.LoginOperatorName, accessor);

			// Update Order Return Status
			var result = new OrderService().Get(StringUtility.ToEmpty(orderNew[Constants.FIELD_ORDER_ORDER_ID]), accessor);
			UpdateStatusCompleteForOrderReturn(result, updateHistoryAction, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				var updateHistoryService = new UpdateHistoryService();
				updateHistoryService.InsertForOrder(result.OrderId, this.LoginOperatorName, accessor);
				updateHistoryService.InsertForUser(StringUtility.ToEmpty(orderNew[Constants.FIELD_ORDER_USER_ID]), this.LoginOperatorName, accessor);
			}
		}

		/// <summary>
		/// Execute regist the taiwan order invoice ec pay
		/// </summary>
		/// <param name="orderNew">The new order</param>
		/// <param name="orderModelOld">The old order</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>The result kbn of workflow</returns>
		public OrderCommon.ResultKbn ExecuteRegistTwOrderInvoiceEcPay(
			Hashtable orderNew,
			OrderModel orderModelOld,
			SqlAccessor accessor)
		{
			var invoiceEcPayApi = new TwInvoiceEcPayApi();
			var invoiceService = new TwOrderInvoiceService();
			var orderInvoiceOld = invoiceService.GetOrderInvoice(
				orderModelOld.OrderId,
				orderModelOld.Shippings.FirstOrDefault().OrderShippingNo);

			if (orderInvoiceOld == null)
			{
				return OrderCommon.ResultKbn.UpdateNG;
			}

			var orderModelNew = new OrderModel(orderNew);
			var orderInvoiceNew = orderInvoiceOld.Clone();
			orderInvoiceNew.OrderId = orderModelNew.OrderId;

			var beginDate = orderInvoiceOld.TwInvoiceDate ?? DateTime.Now;
			if (invoiceEcPayApi.IsSamePeriod(beginDate, DateTime.Now))
			{
				// Execute invalid api
				var request = invoiceEcPayApi.CreateRequestReturnObject(
					TwInvoiceEcPayApi.ExecuteTypes.Invalid,
					orderModelNew,
					orderInvoiceOld,
					accessor);

				var response = invoiceEcPayApi.ReceiveResponseObject(
					TwInvoiceEcPayApi.ExecuteTypes.Invalid,
					request);

				if (response.IsSuccess == false)
				{
					return OrderCommon.ResultKbn.UpdateNG;
				}

				// Update Taiwan old order invoice status
				invoiceService.UpdateTwOrderInvoiceStatus(
					orderModelOld.OrderId,
					orderModelOld.Shippings.FirstOrDefault().OrderShippingNo,
					Constants.FLG_ORDER_INVOICE_STATUS_CANCEL,
					orderInvoiceOld.TwInvoiceNo,
					accessor);

				// Update Taiwan new order invoice for modify
				orderInvoiceNew.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_CANCEL;
				orderInvoiceNew.TwInvoiceNo = orderInvoiceOld.TwInvoiceNo;
				orderInvoiceNew.TwInvoiceDate = null;
			}
			else
			{
				// Execute allowance api
				var request = invoiceEcPayApi.CreateRequestReturnObject(
					TwInvoiceEcPayApi.ExecuteTypes.Allowance,
					orderModelNew,
					orderInvoiceOld,
					accessor);

				var response = invoiceEcPayApi.ReceiveResponseObject(
					TwInvoiceEcPayApi.ExecuteTypes.Allowance,
					request);

				if (response.IsSuccess == false)
				{
					return OrderCommon.ResultKbn.UpdateNG;
				}

				// Update Taiwan old order invoice status
				invoiceService.UpdateTwOrderInvoiceStatus(
					orderModelOld.OrderId,
					orderModelOld.Shippings.FirstOrDefault().OrderShippingNo,
					Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED,
					orderInvoiceOld.TwInvoiceNo,
					accessor);

				// Update Taiwan new order invoice status
				orderInvoiceNew.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED;
				orderInvoiceNew.TwInvoiceNo = response.Response.Data.IAAllowNo;
			}
			invoiceService.Insert(orderInvoiceNew, accessor);
			return OrderCommon.ResultKbn.UpdateOK;
		}

		/// <summary>
		/// Get List Return Order Item Info
		/// </summary>
		/// <param name="returnOrder">Return Order</param>
		/// <returns>List Return Order Item Info</returns>
		private List<ReturnOrderItem> GetListReturnOrderItemInfo(Order returnOrder)
		{
			var result = returnOrder.Items.Select(item =>
			{
				var itemShipping =
					returnOrder.Shippings.First(shipping => shipping.OrderShippingNo == item.OrderShippingNo);
				return new ReturnOrderItem
				{
					ShopId = returnOrder.ShopId,
					ProductId = item.ProductId,
					VId = item.VId,
					SupplierId = returnOrder.SupplierId,
					ProductName = item.ProductName,
					ProductNameKana = item.ProductNameKana,
					ProductPrice = decimal.Parse(item.ProductPrice),
					ItemPriceTax = (-decimal.Parse(item.ItemPriceTax)),
					ItemQuantity = (-int.Parse(item.ItemQuantity)),
					ProductSaleId = item.ProductSaleId,
					NoveltyId = item.NoveltyId,
					RecommendId = item.RecommendId,
					BrandId = item.BrandId,
					DownloadUrl = item.DownloadUrl,
					CooperationIds = item.CooperationId.ToArray(),
					ItemReturnExchangeKbn = item.ItemReturnExchangeKbn,
					ProductOptionValue = item.ProductOptionSettingSelectedTexts,
					OrderShippingNo = item.OrderShippingNo,
					ShippingName = itemShipping.ShippingName,
					ShippingNameKana = itemShipping.ShippingNameKana,
					ShippingTel1 = itemShipping.ShippingTel1,
					ShippingZip = itemShipping.ShippingZip,
					ShippingAddr1 = itemShipping.ShippingAddr1,
					ShippingAddr2 = itemShipping.ShippingAddr2,
					ShippingAddr3 = itemShipping.ShippingAddr3,
					ShippingAddr4 = itemShipping.ShippingAddr4,
					ShippingCompanyName = itemShipping.ShippingCompanyName,
					ShippingCompanyPostName = itemShipping.ShippingCompanyPostName,
					ProductPricePretax = decimal.Parse(item.ProductPricePretax),
					ProductTaxIncludedFlg = item.ProductTaxIncludedFlg,
					ProductTaxRate = decimal.Parse(item.ProductTaxRate),
					ProductTaxRoundType = item.ProductTaxRoundType,
					OrderSetPromotionNo = item.OrderSetPromotionNo,
					FixedPurchaseProductFlg = item.FixedPurchaseProductFlg,
					ProductBundleId = item.ProductBundleId,
					BundleItemDisplayType = item.BundleItemDisplayType,
					ShippingAddr5 = itemShipping.ShippingAddr5,
					ShippingCountryIsoCode = itemShipping.ShippingCountryIsoCode,
					ShippingCountryName = itemShipping.ShippingCountryName,
					DiscountedPrice = decimal.Parse(item.DiscountedPrice) * (-1),
					SubscriptionBoxCourseId = item.SubscriptionBoxCourseId,
					SubscriptionBoxFixedAmount = item.SubscriptionBoxFixedAmount,
				};
			}).ToList();

			return result;
		}

		/// <summary>
		/// Get Return Order
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Order Or Null</returns>
		public Order GetReturnOrder(string orderId, SqlAccessor accessor)
		{
			var order = new OrderService().GetOrderForOrderReturnExchangeInDataView(orderId, accessor);
			var result = order.Cast<DataRowView>();

			// Check if the order has returned before => Error message
			var isAllOrderItemNotReturn = result
				.All(item => (StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN]) == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN));
			if (isAllOrderItemNotReturn == false)
			{
				var productId =
					result.First(item => (StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN]) == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN))
						[Constants.FIELD_ORDERITEM_PRODUCT_ID];

				AppLogger.WriteError(string.Format("注文ID={0}/返品済み:{1}", orderId, productId));

				return null;
			}

			// Check if the order is a temporary order => Error message
			var isOrderTemp = result.Any(item => (StringUtility.ToEmpty(item[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]) == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID));
			if (isOrderTemp)
			{
				AppLogger.WriteError(string.Format("注文ID={0}/仮クレジットカード決済種別", orderId));

				return null;
			}

			// Check if the order is canceled => Error message
			var isCancelOrder = result.Any(item => (StringUtility.ToEmpty(item[Constants.FIELD_ORDER_ORDER_STATUS]) == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED));
			if (isCancelOrder)
			{
				AppLogger.WriteError(string.Format("注文ID={0}/キャンセル", orderId));

				return null;
			}

			if (order.Count == 0) return null;

			var returnOrder = new Order(order, accessor);
			returnOrder.Items.ForEach(item => item.ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN);
			return returnOrder;
		}

		/// <summary>
		/// Execute Regist Order Shipping And Order Item
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="returnOrder">Return order</param>
		/// <param name="orderOld">Order old</param>
		/// <param name="listReturnOrderItem">List return order item</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Accessor</param>
		private void ExecuteRegistOrderShippingAndItem(
			Hashtable order,
			Order returnOrder,
			OrderModel orderOld,
			List<ReturnOrderItem> listReturnOrderItem,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var orderItemNo = 1;
			var orderShippingNumbers = new HashSet<int>();	// ギフト用（配送番号）
			var orderSetPromotionItemNoList = returnOrder.SetPromotions.ToDictionary(osp => osp.OrderSetPromotionNo, osp => 1);

			foreach (var item in listReturnOrderItem)
			{
				if (orderShippingNumbers.Add(int.Parse(StringUtility.ToEmpty(item.OrderShippingNo))))
				{
					OrderCommon.RegistOrderShippingForReturnExchange(order, orderOld, item.OrderShippingNo, accessor);
				}

				// 注文商品情報登録
				OrderCommon.RegistReturnOrderItemForReturnExchange(
					order,
					orderItemNo,
					item.OrderShippingNo,
					orderSetPromotionItemNoList,
					item,
					this.LoginOperatorName,
					accessor);

				orderItemNo++;
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
					this.LoginOperatorName,
					accessor);
			}
		}

		/// <summary>
		/// Process For Return Add Point Temp And Complete
		/// </summary>
		/// <param name="orderNew">Order new</param>
		/// <param name="orderOld">Order old</param>
		private void ProcessForReturnAddPointTmpAndComp(Hashtable orderNew, OrderModel orderOld)
		{
			// Get list user point add temp
			var userPointTemps = OrderCommon.GetUserPointTemp(orderOld.UserId, orderOld.OrderId);

			// For process return add point temp
			var returnAddPointTmpList = new List<List<Hashtable>>();
			var returnPointsTemp = new List<Hashtable>();
			foreach (var item in userPointTemps)
			{
				var returnAddPointTmp = new Hashtable
				{
					{ Constants.FIELD_USERPOINT_USER_ID, orderOld.UserId },
					{ Constants.FIELD_ORDER_ORDER_ID, orderOld.OrderId },
					{ Constants.FIELD_USERPOINT_POINT_KBN_NO, item.PointKbnNo },
					{ Constants.FIELD_USERPOINT_POINT_INC_KBN, item.PointIncKbn },
					{ OrderCommon.CONST_ORDER_POINT_ADD_ADJUSTMENT, -1*(item.Point) }
				};

				returnPointsTemp.Add(returnAddPointTmp);
			}
			returnAddPointTmpList.Add(returnPointsTemp);
			orderNew[OrderCommon.CONST_ORDER_POINT_ADD_TEMP] = returnAddPointTmpList;

			// For process return add point complete
			if (orderOld.OrderPointAdd != 0)
			{
				var userPointHistoryModels = new PointService().GetUserPointHistories(orderOld.UserId);

				var orderBasePointAddComp = PointOptionUtility.GetOrderPointAdd(
					userPointHistoryModels,
					Constants.FLG_USERPOINT_POINT_KBN_BASE,
					orderOld.OrderId);
				var returnBasePointComp = new Hashtable
				{
					{ Constants.FIELD_USERPOINT_USER_ID, orderOld.UserId },
					{ Constants.FIELD_ORDER_ORDER_ID, orderOld.OrderId },
					{ Constants.FIELD_USERPOINT_POINT, 0m },
					{ OrderCommon.CONST_ORDER_POINT_ADD_ADJUSTMENT, -1*orderBasePointAddComp }
				};

				var orderLimitPointAddComp = PointOptionUtility.GetOrderPointAdd(
					userPointHistoryModels,
					Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT,
					orderOld.OrderId);
				var returnLimitPointComp = new Hashtable
				{
					{ Constants.FIELD_USERPOINT_USER_ID, orderOld.UserId },
					{ Constants.FIELD_ORDER_ORDER_ID, orderOld.OrderId },
					{ Constants.FIELD_USERPOINT_POINT, 0m },
					{ OrderCommon.CONST_ORDER_POINT_ADD_ADJUSTMENT, -1*orderLimitPointAddComp }
				};

				orderNew[OrderCommon.CONST_ORDER_BASE_POINT_ADD_COMP] = returnBasePointComp;
				orderNew[OrderCommon.CONST_ORDER_LIMIT_POINT_ADD_COMP] = returnLimitPointComp;
			}
			else
			{
				orderNew[OrderCommon.CONST_ORDER_BASE_POINT_ADD_COMP] = null;
				orderNew[OrderCommon.CONST_ORDER_LIMIT_POINT_ADD_COMP] = null;
			}

			orderNew[OrderCommon.CONST_ORDER_ORDER_POINT_USE_ADJUSTMENT] = ((orderOld.LastOrderPointUse > 0) ? (orderOld.LastOrderPointUse * -1) : 0);
		}

		/// <summary>
		/// Create Order Info
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="returnOrder">Return order</param>
		/// <param name="cart">Cart</param>
		/// <returns>Order Info</returns>
		private Hashtable CreateOrderInfo(
			OrderModel order,
			Order returnOrder,
			CartObject cart)
		{
			var orderPriceSubtotalTax = order.OrderPriceSubtotalTax * -1;
			var orderPriceTotal = order.LastBilledAmount;
			var orderInfo = new Hashtable()
			{
				{ Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, order.OrderPaymentKbn },
				{ Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS, Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_CONFIRM },
				{ Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE, DateTime.Now },
				{ Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS, Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_RECEIPT },
				{ Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE, DateTime.Now.ToShortDateString() },
				{ Constants.FIELD_ORDER_CREDIT_BRANCH_NO, order.CreditBranchNo },
				{ Constants.FIELD_ORDER_CARD_INSTRUMENTS, order.CardInstruments },
				{ Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE, order.CardInstallmentsCode },
				{ Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, (order.OrderPriceSubtotal * -1) },
				{ Constants.FIELD_ORDER_LAST_AUTH_FLG, Constants.FLG_ORDER_LAST_AUTH_FLG_OFF },
				{ Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN },
				{ Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, (orderPriceTotal * -1) },
				{ Constants.FIELD_ORDER_ORDER_PRICE_REGULATION, 0m },
				{ Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX, orderPriceSubtotalTax },
				{ Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT, orderPriceTotal },
				{ Constants.FIELD_ORDER_REGULATION_MEMO, string.Empty },
				{ Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO, cart.Shippings[0].ShippingNo },
				{ Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE, order.ExternalPaymentType },
				{ Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID, order.SubscriptionBoxCourseId },
				{ Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT, order.SubscriptionBoxFixedAmount },
				{ Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT, order.OrderSubscriptionBoxOrderCount },
				{ Constants.FIELD_ORDER_STOREPICKUP_STATUS, order.StorePickupStatus },
				{ Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE, order.StorePickupReturnDate },
			};

			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				if ((returnOrder.Coupon != null)
					&& (returnOrder.Coupon.IsCouponLimit
					|| returnOrder.Coupon.IsCouponAllLimit
					|| returnOrder.Coupon.IsBlacklistCoupon))
				{
					var orderCoupon = OrderCommon.GetOrderCoupon(returnOrder);
					if (orderCoupon != null)
					{
						orderInfo[Constants.TABLE_ORDERCOUPON] = orderCoupon;
					}
				}
			}

			// Return Global Owner Address
			OrderCommon.ReturnGlobalOwnerAddress(orderInfo, returnOrder);

			return orderInfo;
		}

		/// <summary>
		/// Create Cart
		/// </summary>
		/// <param name="returnOrder">Return Order</param>
		/// <param name="user">User</param>
		/// <returns>Cart object</returns>
		public CartObject CreateCart(Order returnOrder, UserModel user)
		{
			// カートオブジェクト作成
			var cart = new CartObject(
				returnOrder.UserId,
				returnOrder.OrderKbn,
				returnOrder.ShopId,
				returnOrder.ShippingId,
				false,
				false)
			{
				IsReturnCart = true,
				MallId = returnOrder.MallId,
			};


			// カート再計算
			OrderCommon.CalculateCartForReturnExchange(cart, returnOrder, user);

			return cart;
		}

		/// <summary>
		/// Update Status Complete For Order Return
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Accessor</param>
		private void UpdateStatusCompleteForOrderReturn(OrderModel order, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// Update Order Return Status
			UpdateOrderReturnStatus(order, updateHistoryAction, accessor);

			// Update Order Repayment Status
			UpdateOrderRepaymentStatus(order, updateHistoryAction, accessor);
		}

		/// <summary>
		/// Update Order Return Status
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Accessor</param>
		private void UpdateOrderReturnStatus(OrderModel order, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var actions = new List<string>()
			{
				Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS,
				Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE
			};

			// Get data input action
			var actionInput = new Hashtable
			{
				{ Constants.TABLE_ORDER, actions }
			};

			var history = new OrderHistory
			{
				OrderId = order.OrderId,
				Action = OrderHistory.ActionType.EcOrderWorkflow,
				OpearatorName = this.LoginOperatorName,
				Accessor = accessor,
				UpdateAction = actionInput
			};

			// Begin write history
			history.HistoryBegin();

			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
				{ "update_date", DateTime.Now.ToShortDateString() },
				{ Constants.FIELD_ORDER_USER_ID, order.UserId },
				{ Constants.FIELD_ORDER_LAST_CHANGED, this.LoginOperatorName },
				{ Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS, Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_RECEIPT }
			};

			new OrderService().UpdateOrderReturnExchangeStatusReceipt(input, accessor);

			if (order.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN)
			{
				// 定期購入OP有効?
				if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
				{
					var orderId = order.OrderId;
					var orderIdOrg = order.OrderIdOrg;

					// 注文（返品交換含む）取得
					var service = new OrderService();
					var relatedOrders = service.GetRelatedOrders(orderIdOrg, accessor);

					// 元注文取得
					var orderOrg = relatedOrders.First(item => item.IsOriginalOrder);

					// 定期購入注文?
					if (string.IsNullOrEmpty(orderOrg.FixedPurchaseId) == false)
					{
						// 本注文の返品交換ステータスを「返品交換完了」にセット
						var orderReturn = relatedOrders.First(item => item.OrderId == orderId);
						orderReturn.OrderReturnExchangeStatus = Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE;

						// 注文商品が全て返品されているか？
						if (service.InspectReturnAllItems(relatedOrders, accessor))
						{
							// 定期購入：注文返品更新
							new FixedPurchaseService().UpdateForReturnOrder(
								orderOrg.FixedPurchaseId,
								orderOrg.OrderId,
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert,
								accessor);
						}
					}
				}
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(order.OrderId, this.LoginOperatorName, accessor);
			}

			// Write history complete
			history.HistoryComplete();
		}

		/// <summary>
		/// Update Order Repayment Status
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Accessor</param>
		private void UpdateOrderRepaymentStatus(OrderModel order, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var actions = new List<string>()
			{
				Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS,
				Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE
			};

			// Get data input action
			var actionInput = new Hashtable
			{
				{ Constants.TABLE_ORDER, actions }
			};

			var history = new OrderHistory
			{
				OrderId = order.OrderId,
				Action = OrderHistory.ActionType.EcOrderWorkflow,
				OpearatorName = this.LoginOperatorName,
				Accessor = accessor,
				UpdateAction = actionInput
			};

			// Begin write history
			history.HistoryBegin();

			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
				{ "update_date", DateTime.Now.ToShortDateString() },
				{ Constants.FIELD_ORDER_USER_ID, order.UserId },
				{ Constants.FIELD_ORDER_LAST_CHANGED, this.LoginOperatorName },
				{ Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS, Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_CONFIRM }
			};

			new OrderService().UpdateOrderRepaymentStatusConfrim(input, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(order.OrderId, this.LoginOperatorName, accessor);
			}

			// Write history complete
			history.HistoryComplete();
		}

		/// <summary>
		/// Had Order Return
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>True: If had order return</returns>
		public bool HadOrderReturn(string orderId, SqlAccessor accessor)
		{
			var order = new OrderService().GetForOrderReturnInDataView(orderId, Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN, accessor);
			return ((int)order[0][0] > 0);
		}
		/// <summary>ログインオペレーター名</summary>
		private string LoginOperatorName { get; set; }
		/// <summary>ログインオペレーターデプトID</summary>
		private string LoginOperatorDeptId { get; set; }
	}
}
