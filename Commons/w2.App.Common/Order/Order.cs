/*
=========================================================================================================
  Module      : 注文情報クラス(Order.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Extensions;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order
{
	/// <summary>
	/// Order の概要の説明です
	/// </summary>
	[Serializable]
	public class Order: IEnumerable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Order()
		{
			this.ExtendStatuses = new string[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];
			this.ExtendStatusDates = new string[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dvOrder">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <remarks>
		/// 注文情報、注文者情報、配送先情報、注文商品情報、注文クーポン情報を含む情報を引数をして渡す
		/// </remarks>
		public Order(DataView dvOrder, SqlAccessor accessor = null)
			: this()
		{
			var drvOrder = dvOrder[0];

			//-----------------------------------------------------
			// 注文情報設定
			//-----------------------------------------------------
			this.OrderId = (string)drvOrder[Constants.FIELD_ORDER_ORDER_ID];
			this.OrderIdOrg = (string)drvOrder[Constants.FIELD_ORDER_ORDER_ID_ORG];
			this.UserId = (string)drvOrder[Constants.FIELD_ORDER_USER_ID];
			this.UserKbn = drvOrder[Constants.FIELD_USER_USER_KBN] != DBNull.Value ? drvOrder[Constants.FIELD_USER_USER_KBN].ToString() : null;
			this.ShopId = (string)drvOrder[Constants.FIELD_ORDER_SHOP_ID];
			this.SupplierId = (string)drvOrder[Constants.FIELD_ORDER_SUPPLIER_ID];
			this.OrderKbn = (string)drvOrder[Constants.FIELD_ORDER_ORDER_KBN];
			this.MallId = (string)drvOrder[Constants.FIELD_ORDER_MALL_ID];
			this.MallName = StringUtility.ToEmpty(drvOrder[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]);
			this.OrderPaymentKbn = (string)drvOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN];
			this.OrderStatus = (string)drvOrder[Constants.FIELD_ORDER_ORDER_STATUS];
			this.OrderDate = drvOrder[Constants.FIELD_ORDER_ORDER_DATE].ToString();
			this.OrderRecognitionDate = drvOrder[Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE].ToString() : null;
			this.OrderStockReservedStatus = (string)drvOrder[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS];
			this.OrderStockReservedDate = drvOrder[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE].ToString() : null;
			this.OrderShippingDate = drvOrder[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE].ToString() : null;
			this.OrderShippedStatus = (string)drvOrder[Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS];
			this.OrderShippedDate = drvOrder[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE].ToString() : null;
			this.OrderDeliveringDate = drvOrder[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE].ToString() : null;
			this.OrderCancelDate = drvOrder[Constants.FIELD_ORDER_ORDER_CANCEL_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_CANCEL_DATE].ToString() : null;
			this.OrderPaymentStatus = (string)drvOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS];
			this.OrderPaymentDate = drvOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE].ToString() : null;
			this.DemandStatus = (string)drvOrder[Constants.FIELD_ORDER_DEMAND_STATUS];
			this.DemandDate = drvOrder[Constants.FIELD_ORDER_DEMAND_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_DEMAND_DATE].ToString() : null;
			this.OrderReturnExchangeStatus = (string)drvOrder[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS];
			this.OrderReturnExchangeReceiptDate = drvOrder[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE].ToString() : null;
			this.OrderReturnExchangeArrivalDate = drvOrder[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE].ToString() : null;
			this.OrderReturnExchangeCompleteDate = drvOrder[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE].ToString() : null;
			this.OrderRepaymentStatus = (string)drvOrder[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS];
			this.OrderRepaymentDate = drvOrder[Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE].ToString() : null;
			this.OrderItemCount = drvOrder[Constants.FIELD_ORDER_ORDER_ITEM_COUNT].ToString();
			this.OrderProductCount = drvOrder[Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT].ToString();
			this.OrderPriceSubtotal = drvOrder[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL].ToString();
			this.OrderPriceShipping = drvOrder[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING].ToString();
			this.OrderPriceExchange = drvOrder[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE].ToString();
			this.OrderPriceRegulation = drvOrder[Constants.FIELD_ORDER_ORDER_PRICE_REGULATION].ToString();
			this.OrderPriceRepayment = drvOrder[Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT].ToString();
			this.OrderPriceTotal = drvOrder[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL].ToString();
			this.OrderPriceTax = drvOrder[Constants.FIELD_ORDER_ORDER_PRICE_TAX].ToString();
			this.OrderPriceSubtotalTax = drvOrder[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX].ToString();
			this.ShippingTaxRate = drvOrder[Constants.FIELD_ORDER_SHIPPING_TAX_RATE].ToString();
			this.PaymentTaxRate = drvOrder[Constants.FIELD_ORDER_PAYMENT_TAX_RATE].ToString();
			this.OrderPointUse = drvOrder[Constants.FIELD_ORDER_ORDER_POINT_USE].ToString();
			this.OrderPointUseYen = drvOrder[Constants.FIELD_ORDER_ORDER_POINT_USE_YEN].ToString();
			this.OrderPointAdd = drvOrder[Constants.FIELD_ORDER_ORDER_POINT_ADD].ToString();
			this.OrderCouponUse = drvOrder[Constants.FIELD_ORDER_ORDER_COUPON_USE].ToString();
			this.MemberRankId = drvOrder[Constants.FIELD_ORDER_MEMBER_RANK_ID].ToString();
			this.MemberRankName = MemberRankOptionUtility.GetMemberRankName(this.MemberRankId);
			this.MemberRankDiscount = drvOrder[Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE].ToString();
			this.FixedPurchaseMemberDiscountAmount = drvOrder[Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT].ToString();
			this.SetPromotionProductDiscountAmount = drvOrder[Constants.FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT].ToString();
			this.SetPromotionShippingChargeDiscountAmount = drvOrder[Constants.FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT].ToString();
			this.SetPromotionPaymentChargeDiscountAmount = drvOrder[Constants.FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT].ToString();
			this.PaymentName = (string)drvOrder[Constants.FIELD_PAYMENT_PAYMENT_NAME];
			this.CardKbn = (string)drvOrder[Constants.FIELD_ORDER_CARD_KBN];
			this.CardInstallmentsText = (string)drvOrder[Constants.FIELD_ORDER_CARD_INSTRUMENTS];
			this.CardInstallmentsCode = (string)drvOrder[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE];
			this.CardTranId = (string)drvOrder[Constants.FIELD_ORDER_CARD_TRAN_ID];
			this.PaymentOrderId = (string)drvOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];
			this.ShippingId = (string)drvOrder[Constants.FIELD_ORDER_SHIPPING_ID];
			this.ShippingName = (string)drvOrder[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME];
			this.ShippedChangedKbn = (string)drvOrder[Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN];
			this.ReturnExchangeKbn = (string)drvOrder[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN];
			this.ReturnExchangeReasonKbn = (string)drvOrder[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN];
			this.AdvCodeFirst = (string)drvOrder[Constants.FIELD_ORDER_ADVCODE_FIRST];
			this.AdvCodeNew = (string)drvOrder[Constants.FIELD_ORDER_ADVCODE_NEW];
			this.CarrerId = (string)drvOrder[Constants.FIELD_ORDER_CAREER_ID];
			this.MobileUID = (string)drvOrder[Constants.FIELD_ORDER_MOBILE_UID];
			this.RemoteAddr = (string)drvOrder[Constants.FIELD_ORDER_REMOTE_ADDR];
			this.Attribute1 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE1];
			this.Attribute2 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE2];
			this.Attribute3 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE3];
			this.Attribute4 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE4];
			this.Attribute5 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE5];
			this.Attribute6 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE6];
			this.Attribute7 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE7];
			this.Attribute8 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE8];
			this.Attribute9 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE9];
			this.Attribute10 = (string)drvOrder[Constants.FIELD_ORDER_ATTRIBUTE10];
			this.CreditBranchNo = (int?)StringUtility.ToValue(drvOrder[Constants.FIELD_ORDER_CREDIT_BRANCH_NO], null);
			// 注文拡張ステータス１～３０個（利用数分のみ定義する）
			for (int i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
			{
				this.ExtendStatuses[i] = (string)drvOrder[Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + (i + 1).ToString()];
				this.ExtendStatusDates[i] = drvOrder[Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + (i + 1).ToString()] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + (i + 1).ToString()].ToString() : null;
			}
			this.Memo = (string)drvOrder[Constants.FIELD_ORDER_MEMO];
			this.PaymentMemo = (string)drvOrder[Constants.FIELD_ORDER_PAYMENT_MEMO];
			this.ManagementMemo = (string)drvOrder[Constants.FIELD_ORDER_MANAGEMENT_MEMO];
			this.RelationMemo = (string)drvOrder[Constants.FIELD_ORDER_RELATION_MEMO];
			this.ReturnExchangeReasonMemo = (string)drvOrder[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_MEMO];
			this.RegulationMemo = (string)drvOrder[Constants.FIELD_ORDER_REGULATION_MEMO];
			this.RepaymentMemo = (string)drvOrder[Constants.FIELD_ORDER_REPAYMENT_MEMO];
			this.DateCreated = drvOrder[Constants.FIELD_ORDER_DATE_CREATED].ToString();
			this.DateChanged = drvOrder[Constants.FIELD_ORDER_DATE_CHANGED].ToString();
			this.LastChanged = (string)drvOrder[Constants.FIELD_ORDER_LAST_CHANGED];
			this.GiftFlg = (string)drvOrder[Constants.FIELD_ORDER_GIFT_FLG];
			this.PaymentSelectionFlg = (string)drvOrder[Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG];
			this.PermittedPaymentIds = (string)drvOrder[Constants.FIELD_SHOPSHIPPING_PERMITTED_PAYMENT_IDS];

			// 配送料別途見積もり表示する場合(文言はPC用を優先)
			this.ShippingPriceSeparateEstimateFlg = (string)drvOrder[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG];
			this.ShippingPriceSeparateEstimateMessage = ((string)drvOrder[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE] != "") ?
				(string)drvOrder[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE] : (string)drvOrder[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE_MOBILE];

			this.FixedPurchaseId = (string)drvOrder[Constants.FIELD_ORDER_FIXED_PURCHASE_ID];
			this.FixedPurchaseOrderCount =
				(drvOrder[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT] != DBNull.Value) ? (int?)drvOrder[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT] : null;
			this.FixedPurchaseShippedCount =
				(drvOrder[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT] != DBNull.Value) ? (int?)drvOrder[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT] : null;
			this.FixedPurchaseDiscountPrice = drvOrder[Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE].ToString();

			// 最終請求金額、最終利用ポイント数、最終ポイント利用額
			this.LastBilledAmount = drvOrder[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT].ToString();
			this.LastOrderPointUse = drvOrder[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE].ToString();
			this.LastOrderPointUseYen = drvOrder[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN].ToString();

			// 返品交換含む注文の最終請求金額
			// ※返品交換含む関連注文の合計金額(キャンセル済みの交換商品代金を除く)
			var relatedOrders = new OrderService().GetRelatedOrders(this.OrderId, accessor);
			var canceledItem = relatedOrders
				.Where(order => (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED))
				.SelectMany(order => order.Shippings.SelectMany(shipping => shipping.Items)).Where(
					item => item.ItemReturnExchangeKbn == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE);

			var relatedOrderPriceTotal = relatedOrders.Sum(order => order.OrderPriceTotal);
			var canceledItemPriceTotal = canceledItem
				.Where(item => item.IsSubscriptionBoxFixedAmount == false)
				.Sum(item => TaxCalculationUtility.GetPriceTaxIncluded(item.ItemPrice, item.ItemPriceTax));
			this.RelatedOrderLastBilledAmount = (relatedOrderPriceTotal - canceledItemPriceTotal).ToString();

			this.RelatedOrderLastOrderPointUse = relatedOrders.Sum(order => order.OrderPointUse).ToString();
			this.RelatedOrderLastOrderPointUseYen = relatedOrders.Sum(order => order.OrderPointUseYen).ToString();

			// 返品交換含む税率毎価格情報情報を設定
			var orderPriceByTaxRateList = new OrderPriceByTaxRateService().GetRelatedSummaryOrderPriceByTaxRate(this.OrderId);
			foreach (var orderPriceByTaxRate in orderPriceByTaxRateList)
			{
				var canceledPrice = canceledItem
					.Where(
						item => (item.ProductTaxRate == orderPriceByTaxRate.KeyTaxRate)
							&& (item.IsSubscriptionBoxFixedAmount == false))
					.Sum(item => TaxCalculationUtility.GetPriceTaxIncluded(item.ItemPrice, item.ItemPriceTax));
				orderPriceByTaxRate.PriceSubtotalByRate -= canceledPrice;
				orderPriceByTaxRate.PriceTotalByRate -= canceledPrice;
			}
			m_lOrderPriceByTaxRate.AddRange(orderPriceByTaxRateList);


			// 外部決済ステータス, 外部決済エラーメッセージ
			this.ExternalPaymentStatus = (string)drvOrder[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS];
			this.ExternalPaymentErrorMessage = (string)drvOrder[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_ERROR_MESSAGE];
			this.ExternalPaymentAuthDate = drvOrder[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE] != DBNull.Value ? drvOrder[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE].ToString() : null;

			//-----------------------------------------------------
			// 注文者情報設定
			//-----------------------------------------------------
			this.Owner = new OrderOwner(drvOrder);

			//-----------------------------------------------------
			// 配送先情報設定
			//-----------------------------------------------------
			var dvOrderShippings = new OrderService().GetOrderShippingInDataView(this.OrderId);
			foreach (DataRowView drv in dvOrderShippings)
			{
				m_lShippings.Add(new OrderShipping(drv));
			}

			//-----------------------------------------------------
			// 注文商品情報設定
			//-----------------------------------------------------
			foreach (DataRowView drv in dvOrder)
			{
				m_lItems.Add(new OrderItem(drv));
			}

			//-----------------------------------------------------
			// 注文セットプロモーション情報設定
			//-----------------------------------------------------
			var orderSetPromotions = new OrderService().GetOrderSetPromotionInDataView(this.OrderId);
			foreach (DataRowView drv in orderSetPromotions)
			{
				m_lSetPromotions.Add(new OrderSetPromotion(drv.ToHashtable()));
			}

			//-----------------------------------------------------
			// 注文クーポン設定
			//-----------------------------------------------------
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				// 注文クーポンが存在する場合
				if (drvOrder[Constants.FIELD_ORDERCOUPON_COUPON_NO] != DBNull.Value)
				{
					m_lCoupons.Add(new OrderCoupon(drvOrder));
				}
			}
			this.OrderCountOrder = drvOrder[Constants.FIELD_ORDER_ORDER_COUNT_ORDER].ToString();
			this.InvoiceBundleFlg = drvOrder[Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG].ToString();
			this.OnlinePaymentStatus = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS]);
			this.DigitalContentsFlg = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG]);
			this.ExternalPaymentType = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE]);
			this.SubscriptionBoxCourseId = (string)drvOrder[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID];
			if (drvOrder[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT] != DBNull.Value)
			{
				this.SubscriptionBoxFixedAmount = (decimal?)drvOrder[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT];
			}
			this.OrderSubscriptionBoxOrderCount = (int?)drvOrder[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT];
			this.IsOrderCombined = string.IsNullOrEmpty(
				(string)drvOrder[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS]) == false;

			if (this.HasSubscriptionBoxFixedAmountItem && (this.IsReturnExchangeOrder == false))
			{
				SetFixedAmountCourseItemReturned(relatedOrders);
			}
		}

		/// <summary>
		/// IEnumerable.GetEnumerator()の実装
		/// </summary>
		/// <returns>IEnumerator</returns>
		public IEnumerator GetEnumerator()
		{
			return m_lItems.GetEnumerator();
		}

		/// <summary>
		/// 注文者情報更新
		/// </summary>
		/// <param name="orderOwner">注文者情報</param>
		public void UpdateOwner(Hashtable orderOwner)
		{
			//-----------------------------------------------------
			// 注文者情報更新
			//-----------------------------------------------------
			// 更新(更新対象の項目のみ)
			this.Owner.OwnerName = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME];
			this.Owner.OwnerName1 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME1];
			this.Owner.OwnerName2 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME2];
			this.Owner.OwnerNameKana = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA];
			this.Owner.OwnerNameKana1 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1];
			this.Owner.OwnerNameKana2 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2];
			this.Owner.OwnerTel1 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL1];
			this.Owner.OwnerTel2 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL2];
			this.Owner.OwnerMailAddr = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
			this.Owner.OwnerMailAddr2 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2];
			this.Owner.OwnerZip = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_ZIP];
			this.Owner.OwnerAddr1 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR1];
			this.Owner.OwnerAddr2 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR2];
			this.Owner.OwnerAddr3 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR3];
			this.Owner.OwnerAddr4 = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR4];
			this.Owner.OwnerCompanyName = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME];
			this.Owner.OwnerCompanyPostName = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME];
			this.Owner.OwnerSex = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_SEX];
			this.Owner.OwnerName = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME];
			this.Owner.OwnerBirth = (string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH];
			this.Owner.OwnerBirthDisp = (orderOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] != null)
				? DateTimeUtility.ToStringForManager(
					DateTime.Parse((string)orderOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]),
					DateTimeUtility.FormatType.LongDate2Letter)
				: null;

			//-----------------------------------------------------
			// ユーザ情報更新
			//-----------------------------------------------------
			this.Owner.UserMemo = (string)orderOwner[Constants.FIELD_USER_USER_MEMO];
			this.Owner.UserManagementLevelId = (string)orderOwner[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID];
		}

		/// <summary>
		/// 配送先情報更新
		/// </summary>
		/// <param name="orderShippings">配送先情報</param>
		public void UpdateShippings(List<Hashtable> orderShippings)
		{
			//-----------------------------------------------------
			// 配送先情報更新
			//-----------------------------------------------------
			foreach (var htOrderShipping in orderShippings)
			{
				foreach (var os in m_lShippings
					.Where(os => (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO] == os.OrderShippingNo))
				{
					// 配送先枝番が一致したら更新(更新対象の項目のみ)
					os.DeleteTarget = (bool)htOrderShipping[OrderShipping.CONST_DELETE_TARGET_SHIPPING];
					os.ShippingName = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME];
					os.ShippingName1 = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1];
					os.ShippingName2 = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2];
					os.ShippingNameKana = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA];
					os.ShippingNameKana1 = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1];
					os.ShippingNameKana2 = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2];
					os.ShippingTel1 = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1];
					os.ShippingZip = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP];
					os.ShippingAddr1 = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1];
					os.ShippingAddr2 = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2];
					os.ShippingAddr3 = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3];
					os.ShippingAddr4 = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4];
					os.ShippingCompanyName = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME];
					os.ShippingCompanyPostName = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME];
					os.ShippingDate = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE];
					os.ShippingTime = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME];
					os.ShippingTimeMessage = (string)htOrderShipping[OrderShipping.FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE];
					os.ShippingCheckNo = (string)htOrderShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO];
					os.SenderName = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME]);
					os.SenderName1 = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME1]);
					os.SenderName2 = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME2]);
					os.SenderNameKana = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA]);
					os.SenderNameKana1 = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1]);
					os.SenderNameKana2 = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2]);
					os.SenderTel1 = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1]);
					os.SenderZip = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP]);
					os.SenderAddr1 = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1]);
					os.SenderAddr2 = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2]);
					os.SenderAddr3 = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3]);
					os.SenderAddr4 = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4]);
					os.SenderCompanyName = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME]);
					os.SenderCompanyPostName = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME]);
					os.WrappingPaperType = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE]);
					os.WrappingPaperName = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME]);
					os.WrappingBagType = StringUtility.ToEmpty(htOrderShipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE]);
					os.AnotherShippingFlag = IsAnotherShippingFlagValid(this.Owner, os) ? Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID : Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID;
					break;
				}
			}
		}

		/// <summary>
		/// 注文クーポン情報更新
		/// </summary>
		/// <param name="htOrderCoupon">注文クーポン情報</param>
		/// <remarks>注文クーポン情報がない場合は、追加</remarks>
		public void UpdateCoupon(Hashtable htOrderCoupon)
		{
			//-----------------------------------------------------
			// 注文クーポン情報更新
			// ※複数クーポン対応時は注文クーポン枝番でマッチングし、更新する必要あり
			//-----------------------------------------------------
			// 更新(更新対象の項目のみ)
			if (m_lCoupons.Count > 0)
			{
				m_lCoupons[0].DeptId = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_DEPT_ID];
				m_lCoupons[0].CouponId = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_ID];
				m_lCoupons[0].CouponNo = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_NO];
				m_lCoupons[0].CouponType = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_TYPE];
				m_lCoupons[0].CouponDiscountPrice = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE];
				m_lCoupons[0].CouponDiscountRate = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE];
				m_lCoupons[0].CouponCode = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_CODE];
				m_lCoupons[0].CouponDispName = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME];
				m_lCoupons[0].CouponName = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_NAME];
			}
			// 追加
			else
			{
				OrderCoupon ocCoupon = new OrderCoupon();
				ocCoupon.OrderCouponNo = "1";
				ocCoupon.DeptId = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_DEPT_ID];
				ocCoupon.CouponId = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_ID];
				ocCoupon.CouponNo = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_NO];
				ocCoupon.CouponType = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_TYPE];
				ocCoupon.CouponDiscountPrice = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE];
				ocCoupon.CouponDiscountRate = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE];
				ocCoupon.CouponCode = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_CODE];
				ocCoupon.CouponDispName = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME];
				ocCoupon.CouponName = (string)htOrderCoupon[Constants.FIELD_ORDERCOUPON_COUPON_NAME];
				m_lCoupons.Add(ocCoupon);
			}
		}

		/// <summary>
		/// 注文商品情報取得
		/// </summary>
		/// <param name="strOrderItemNo">注文商品枝番</param>
		/// <returns>注文商品情報</returns>
		public OrderItem GetItem(string strOrderItemNo)
		{
			OrderItem oiItem = null;

			foreach (OrderItem oi in m_lItems)
			{
				if (oi.OrderItemNo == strOrderItemNo)
				{
					oiItem = oi;
					break;
				}
			}

			return oiItem;
		}

		/// <summary>
		/// 配送先情報取得
		/// </summary>
		/// <param name="strOrderShippingNo">配送先枝番</param>
		/// <returns>配送先情報</returns>
		public OrderShipping GetShipping(string strOrderShippingNo)
		{
			foreach (OrderShipping os in m_lShippings)
			{
				if (os.OrderShippingNo == strOrderShippingNo)
				{
					return os;
				}
			}

			return null;
		}

		/// <summary>
		/// セットプロモーション名取得
		/// </summary>
		/// <param name="orderSetPromotionNo">注文セットプロモーション枝番</param>
		/// <returns>セットプロモーション名</returns>
		public string GetOrderSetPromotionName(string orderSetPromotionNo)
		{
			if (this.SetPromotions.Count == 0) return "";
			if (orderSetPromotionNo == "") return "";

			return this.SetPromotions.First(sp => sp.OrderSetPromotionNo == orderSetPromotionNo).SetPromotionName;
		}

		/// <summary>
		/// 別出荷フラグをチェック
		/// </summary>
		/// <param name="owner">注文者情報</param>
		/// <param name="shipping">配送先情報</param>
		/// <returns>有効:true</returns>
		private bool IsAnotherShippingFlagValid(OrderOwner owner, OrderShipping shipping)
		{
			return ((StringUtility.ToEmpty(owner.OwnerName1) != (StringUtility.ToEmpty(shipping.ShippingName1)))
					|| (StringUtility.ToEmpty(owner.OwnerName2) != (StringUtility.ToEmpty(shipping.ShippingName2)))
					|| (StringUtility.ToEmpty(owner.OwnerZip) != (StringUtility.ToEmpty(shipping.ShippingZip)))
					|| (StringUtility.ToEmpty(owner.OwnerAddr1) != (StringUtility.ToEmpty(shipping.ShippingAddr1)))
					|| (StringUtility.ToEmpty(owner.OwnerAddr2) != (StringUtility.ToEmpty(shipping.ShippingAddr2)))
					|| (StringUtility.ToEmpty(owner.OwnerAddr3) != (StringUtility.ToEmpty(shipping.ShippingAddr3)))
					|| (StringUtility.ToEmpty(owner.OwnerAddr4) != (StringUtility.ToEmpty(shipping.ShippingAddr4)))
					|| (StringUtility.ToEmpty(owner.OwnerCompanyName) != (StringUtility.ToEmpty(shipping.ShippingCompanyName)))
					|| (StringUtility.ToEmpty(owner.OwnerCompanyPostName) != (StringUtility.ToEmpty(shipping.ShippingCompanyPostName)))
					|| (StringUtility.ToEmpty(owner.OwnerTel1) != (StringUtility.ToEmpty(shipping.ShippingTel1))));
		}

		/// <summary>
		/// CrossPoint連携後の注文情報か確認
		/// </summary>
		/// <param name="date">注文日</param>
		/// <param name="userId">注文者</param>
		/// <returns>CrossPoint連携対象の注文か</returns>
		public static bool CheckLinkedCrossPoint(string date, string userId)
		{
			DateTime orderDate;
			if (DateTime.TryParse(date, out orderDate) == false) return false;

			var result = (CheckDateLinkedCrossPoint(orderDate)
				&& CheckUserIdLinkedCrossPoint(userId));
			return result;
		}

		/// <summary>
		/// CrossPoint連携後の注文情報か確認
		/// </summary>
		/// <param name="date">注文日</param>
		/// <param name="userId">注文者</param>
		/// <returns>CrossPoint連携対象の注文か</returns>
		public static bool CheckLinkedCrossPoint(DateTime? date, string userId)
		{
			if (date == null) return false;

			var result = (CheckDateLinkedCrossPoint((DateTime)date)
				&& CheckUserIdLinkedCrossPoint(userId));
			return result;
		}

		/// <summary>
		/// CrossPoint連携後の注文情報か確認
		/// </summary>
		/// <param name="orderDate">注文日</param>
		/// <returns>注文日がCrossPoint連携後の注文か</returns>
		protected static bool CheckDateLinkedCrossPoint(DateTime orderDate)
		{
			if (Constants.CROSS_POINT_LINK_START_DATETIME == null) return false;

			return (Constants.CROSS_POINT_LINK_START_DATETIME <= orderDate);
		}

		/// <summary>
		/// CrossPoint連携後の注文情報か確認
		/// </summary>
		/// <param name="userId">注文者</param>
		/// <returns>注文者がCrossPoint連携済みか</returns>
		protected static bool CheckUserIdLinkedCrossPoint(string userId)
		{
			var result = new CrossPointUserApiService().Get(userId);
			return (result != null);
		}

		/// <summary>
		/// 対象の頒布会定額コースに含まれる注文商品の個数合計を取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <remarks>返品交換区分が通常の商品を取得する</remarks>
		/// <returns>対象コースに含まれる注文商品の個数合計</returns>
		public int GetFixedAmountCourseItemQuantityTotal(string subscriptionBoxCourseId)
		{
			var result = this.Items
				.Where(
					item => item.IsSubscriptionBoxFixedAmount
						&& (item.SubscriptionBoxCourseId == subscriptionBoxCourseId)
						&& item.IsNotReturnExchange)
				.Sum(item => int.Parse(item.ItemQuantity));
			return result;
		}

		/// <summary>
		/// 頒布会定額コース商品の返品情報をセット
		/// </summary>
		/// <param name="relatedOrders">関連注文情報</param>
		private void SetFixedAmountCourseItemReturned(OrderModel[] relatedOrders)
		{
			// 元注文情報
			var originalOrder = relatedOrders.First(item => item.IsNotReturnExchangeOrder);

			// 注文商品に含まれる頒布会定額コースID配列
			var fixedAmountCourseIds = this.Items
				.Where(item => item.IsSubscriptionBoxFixedAmount)
				.Select(item => item.SubscriptionBoxCourseId)
				.Distinct()
				.ToArray();

			// 返品交換注文ID配列
			var returnExchangeOrderIds = relatedOrders
				.Where(item => item.IsNotReturnExchangeOrder == false)
				.Select(item => item.OrderId)
				.Distinct()
				.ToArray();

			var returnedItemsCount = new Dictionary<string, int>();
			var allReturnedFixedAmountCourseIds = new List<string>();

			foreach (var courseId in fixedAmountCourseIds)
			{
				var originalOrderCount = originalOrder.Items
					.Where(
						item => (item.OrderId == originalOrder.OrderId)
							&& item.IsSubscriptionBoxFixedAmount
							&& (item.SubscriptionBoxCourseId == courseId))
					.Sum(item => item.ItemQuantity);
				if (originalOrderCount == 0) continue;

				var returnedOrderCount = returnExchangeOrderIds
					.Sum(
						orderId => originalOrder.Items
							.Where(
								item => (item.OrderId == orderId)
									&& item.IsReturnItem
									&& item.IsSubscriptionBoxFixedAmount
									&& (item.SubscriptionBoxCourseId == courseId))
							.Sum(item => item.ItemQuantity));

				returnedItemsCount.Add(courseId, returnedOrderCount * -1);
				if (originalOrderCount == returnedItemsCount[courseId]) allReturnedFixedAmountCourseIds.Add(courseId);
			}

			this.ReturnedItemQuantityByFixedAmountCourse = returnedItemsCount;
			this.AllReturnedFixedAmountCourseIds = allReturnedFixedAmountCourseIds.ToArray();
		}

		/// <summary>
		/// 1種類の頒布会定額コースの商品のみが含まれるか
		/// </summary>
		/// <remarks>通常の定額頒布会 or 同コース同梱の定額頒布会であるかを確認</remarks>
		/// <returns>1種類の頒布会定額コースの商品のみが含まれるであればTRUE</returns>
		public bool HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()
		{
			var hasItemsNotFixedAmount = this.Items.Any(item => item.IsSubscriptionBoxFixedAmount == false);
			return (hasItemsNotFixedAmount == false) && (this.ItemSubscriptionBoxFixedAmountCourseIds.Length == 1);
		}

		#region "プロパティ"
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>元注文ID</summary>
		public string OrderIdOrg { get; set; }
		/// <summary>ユーザID</summary>
		public string UserId { get; set; }
		/// <summary>顧客区分</summary>
		public string UserKbn { get; set; }
		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>サプライヤID</summary>
		public string SupplierId { get; set; }
		/// <summary>注文区分</summary>
		public string OrderKbn { get; set; }
		/// <summary>モールID</summary>
		public string MallId { get; set; }
		/// <summary>モール名</summary>
		public string MallName { get; set; }
		/// <summary>支払区分(決済種別ID)</summary>
		public string OrderPaymentKbn { get; set; }
		/// <summary>注文ステータス</summary>
		public string OrderStatus { get; set; }
		/// <summary>注文日時</summary>
		public string OrderDate { get; set; }
		/// <summary>受注承認日時</summary>
		public string OrderRecognitionDate { get; set; }
		/// <summary>在庫引当ステータス</summary>
		public string OrderStockReservedStatus { get; set; }
		/// <summary>在庫引当日時</summary>
		public string OrderStockReservedDate { get; set; }
		/// <summary>出荷手配日時</summary>
		public string OrderShippingDate { get; set; }
		/// <summary>出荷ステータス</summary>
		public string OrderShippedStatus { get; set; }
		/// <summary>出荷完了日時</summary>
		public string OrderShippedDate { get; set; }
		/// <summary>配送完了日時</summary>
		public string OrderDeliveringDate { get; set; }
		/// <summary>キャンセル日時</summary>
		public string OrderCancelDate { get; set; }
		/// <summary>入金ステータス</summary>
		public string OrderPaymentStatus { get; set; }
		/// <summary>入金確認日時</summary>
		public string OrderPaymentDate { get; set; }
		/// <summary>督促ステータス</summary>
		public string DemandStatus { get; set; }
		/// <summary>督促日時</summary>
		public string DemandDate { get; set; }
		/// <summary>拡張ステータスリスト</summary>
		public string[] ExtendStatuses { get; private set; }
		/// <summary>拡張ステータス更新日リスト</summary>
		public string[] ExtendStatusDates { get; private set; }
		/// <summary>返品交換ステータス</summary>
		public string OrderReturnExchangeStatus { get; set; }
		/// <summary>返品交換受付日時</summary>
		public string OrderReturnExchangeReceiptDate { get; set; }
		/// <summary>返品交換商品到着日時</summary>
		public string OrderReturnExchangeArrivalDate { get; set; }
		/// <summary>返品交換完了日時</summary>
		public string OrderReturnExchangeCompleteDate { get; set; }
		/// <summary>返金ステータス</summary>
		public string OrderRepaymentStatus { get; set; }
		/// <summary>返金日時</summary>
		public string OrderRepaymentDate { get; set; }
		/// <summary>注文アイテム数</summary>
		public string OrderItemCount { get; set; }
		/// <summary>注文商品数</summary>
		public string OrderProductCount { get; set; }
		/// <summary>小計(商品代金合計)</summary>
		public string OrderPriceSubtotal { get; set; }
		/// <summary>配送料</summary>
		public string OrderPriceShipping { get; set; }
		/// <summary>代引手数料</summary>
		public string OrderPriceExchange { get; set; }
		/// <summary>調整金額</summary>
		public string OrderPriceRegulation { get; set; }
		/// <summary>返金金額</summary>
		public string OrderPriceRepayment { get; set; }
		/// <summary>支払金額合計</summary>
		public string OrderPriceTotal { get; set; }
		/// <summary>配送料税率</summary>
		public string ShippingTaxRate { get; set; }
		/// <summary>決済手数料税率</summary>
		public string PaymentTaxRate { get; set; }
		/// <summary>税総額</summary>
		public string OrderPriceTax { get; set; }
		/// <summary>商品小計消費税額</summary>
		public string OrderPriceSubtotalTax { get; set; }
		/// <summary>利用ポイント数</summary>
		public string OrderPointUse { get; set; }
		/// <summary>ポイント利用額</summary>
		public string OrderPointUseYen { get; set; }
		/// <summary>付与ポイント</summary>
		public string OrderPointAdd { get; set; }
		/// <summary>クーポン割引額</summary>
		public string OrderCouponUse { get; set; }
		/// <summary>注文時会員ランクID</summary>
		public string MemberRankId { get; set; }
		/// <summary>注文時会員ランク名</summary>
		public string MemberRankName { get; set; }
		/// <summary>会員ランク割引額</summary>
		public string MemberRankDiscount { get; set; }
		/// <summary>定期会員割引額</summary>
		public string FixedPurchaseMemberDiscountAmount { get; set; }
		/// <summary>セットプロモーション割引額(商品割引分)</summary>
		public string SetPromotionProductDiscountAmount { get; set; }
		/// <summary>セットプロモーション割引額(配送料割引分)</summary>
		public string SetPromotionShippingChargeDiscountAmount { get; set; }
		/// <summary>セットプロモーション割引額(決済手数料割引分)</summary>
		public string SetPromotionPaymentChargeDiscountAmount { get; set; }
		/// <summary>決済カード区分</summary>
		public string CardKbn { get; set; }
		/// <summary>決済カード支払回数文言</summary>
		public string CardInstallmentsText { get; set; }
		/// <summary>決済カード支払回数コード</summary>
		public string CardInstallmentsCode { get; set; }
		/// <summary>決済カード取引ID</summary>
		public string CardTranId { get; set; }
		/// <summary>決済注文ID</summary>
		public string PaymentOrderId { get; set; }
		/// <summary>配送種別ID</summary>
		public string ShippingId { get; set; }
		/// <summary>配送種別名</summary>
		public string ShippingName { get; set; }
		/// <summary>出荷後変更区分</summary>
		public string ShippedChangedKbn { get; set; }
		/// <summary>返品交換区分</summary>
		public string ReturnExchangeKbn { get; set; }
		/// <summary>返品交換都合区分</summary>
		public string ReturnExchangeReasonKbn { get; set; }
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId { get; set; }
		/// <summary>初回広告コード</summary>
		public string AdvCodeFirst { get; set; }
		/// <summary>最新広告コード</summary>
		public string AdvCodeNew { get; set; }
		/// <summary>キャリアID</summary>
		public string CarrerId { get; set; }
		/// <summary>モバイルUID</summary>
		public string MobileUID { get; set; }
		/// <summary>リモートIPアドレス</summary>
		public string RemoteAddr { get; set; }
		/// <summary>属性1</summary>
		public string Attribute1 { get; set; }
		/// <summary>属性2</summary>
		public string Attribute2 { get; set; }
		/// <summary>属性3</summary>
		public string Attribute3 { get; set; }
		/// <summary>属性4</summary>
		public string Attribute4 { get; set; }
		/// <summary>属性5</summary>
		public string Attribute5 { get; set; }
		/// <summary>属性6</summary>
		public string Attribute6 { get; set; }
		/// <summary>属性7</summary>
		public string Attribute7 { get; set; }
		/// <summary>属性8</summary>
		public string Attribute8 { get; set; }
		/// <summary>属性9</summary>
		public string Attribute9 { get; set; }
		/// <summary>属性10</summary>
		public string Attribute10 { get; set; }
		/// <summary>注文メモ</summary>
		public string Memo { get; set; }
		/// <summary>決済連携メモ</summary>
		public string PaymentMemo { get; set; }
		/// <summary>管理メモ</summary>
		public string ManagementMemo { get; set; }
		/// <summary>外部連携メモ</summary>
		public string RelationMemo { get; set; }
		/// <summary>返品交換理由メモ</summary>
		public string ReturnExchangeReasonMemo { get; set; }
		/// <summary>調整金額メモ</summary>
		public string RegulationMemo { get; set; }
		/// <summary>返金メモ</summary>
		public string RepaymentMemo { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
		/// <summary>注文者情報</summary>
		public OrderOwner Owner { get; set; }
		/// <summary>配送先情報</summary>
		public OrderShipping Shipping
		{
			get
			{
				if (this.Shippings.Count != 0)
				{
					return this.Shippings[0];
				}
				return null;
			}
		}
		/// <summary>配送先情報リスト</summary>
		public List<OrderShipping> Shippings
		{
			get { return m_lShippings; }
		}
		private List<OrderShipping> m_lShippings = new List<OrderShipping>();
		/// <summary>注文商品情報</summary>
		public List<OrderItem> Items
		{
			get { return m_lItems; }
		}
		private List<OrderItem> m_lItems = new List<OrderItem>();
		/// <summary>注文セットプロモーション情報</summary>
		public List<OrderSetPromotion> SetPromotions
		{
			get { return m_lSetPromotions; }
		}
		private List<OrderSetPromotion> m_lSetPromotions = new List<OrderSetPromotion>();
		/// <summary>税率毎価格情報</summary>
		public List<OrderPriceByTaxRateModel> OrderPriceByTaxRate
		{
			get { return m_lOrderPriceByTaxRate; }
		}
		private readonly List<OrderPriceByTaxRateModel> m_lOrderPriceByTaxRate = new List<OrderPriceByTaxRateModel>();
		/// <summary>クーポン情報</summary>
		public OrderCoupon Coupon
		{
			get
			{
				if (this.Coupons.Count != 0)
				{
					return this.Coupons[0];
				}
				return null;
			}
		}
		/// <summary>クーポン情報リスト</summary>
		public List<OrderCoupon> Coupons
		{
			get { return m_lCoupons; }
		}
		private List<OrderCoupon> m_lCoupons = new List<OrderCoupon>();
		/// <summary>決済種別名</summary>
		public string PaymentName { get; set; }
		/// <summary>ギフト注文フラグ</summary>
		public string GiftFlg { get; set; }
		/// <summary>決済選択の任意利用フラグ</summary>
		public string PaymentSelectionFlg { get; set; }
		/// <summary>決済選択の可能リスト</summary>
		public string PermittedPaymentIds { get; set; }
		/// <summary>配送料別途見積もりフラグ</summary>
		public string ShippingPriceSeparateEstimateFlg { get; set; }
		/// <summary>配送料別途見積もり文言</summary>
		public string ShippingPriceSeparateEstimateMessage { get; set; }
		/// <summary>返品交換注文かどうか</summary>
		public bool IsReturnExchangeOrder { get { return this.ReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN; } }
		/// <summary>すべてのアイテムが在庫戻し済みかどうか</summary>
		public bool IsAllItemStockReturned { get { return Items.All(i => (i.IsAllowReturnStock == false)); } }
		/// <summary>Credit Brand No</summary>
		public int? CreditBranchNo { get; set; }
		/// <summary>定期購入回数(注文時点)</summary>
		public int? FixedPurchaseOrderCount { get; set; }
		/// <summary>定期購入回数(出荷時点)</summary>
		public int? FixedPurchaseShippedCount { get; set; }
		/// <summary>定期購入割引額</summary>
		public string FixedPurchaseDiscountPrice { get; set; }
		/// <summary>最終請求金額</summary>
		public string LastBilledAmount { get; set; }
		/// <summary>最終利用ポイント数</summary>
		public string LastOrderPointUse { get; set; }
		/// <summary>最終ポイント利用額</summary>
		public string LastOrderPointUseYen { get; set; }
		/// <summary>最終請求金額（返品交換含む）</summary>
		public string RelatedOrderLastBilledAmount { get; set; }
		/// <summary>最終利用ポイント数（返品交換含む）</summary>
		public string RelatedOrderLastOrderPointUse { get; set; }
		/// <summary>最終ポイント利用額（返品交換含む）</summary>
		public string RelatedOrderLastOrderPointUseYen { get; set; }
		/// <summary>外部決済ステータス</summary>
		public string ExternalPaymentStatus { get; set; }
		/// <summary>外部決済エラーメッセージ</summary>
		public string ExternalPaymentErrorMessage { get; set; }
		/// <summary>外部決済与信日時</summary>
		public string ExternalPaymentAuthDate { get; set; }
		/// <summary>定期購入注文か</summary>
		public bool IsFixedPurchaseOrder
		{
			get { return string.IsNullOrEmpty(this.FixedPurchaseId) == false; }
		}
		/// <summary>ギフト注文か</summary>
		public bool IsGiftOrder
		{
			get
			{
				return (this.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON);
			}
		}
		/// <summary> 再与信可能の注文サイト区分であるかどうか </summary>
		public bool IsPermitReauthOrderSiteKbn
		{
			get { return (Constants.PAYMENT_REAUTH_ORDER_SITE_KBN.Contains(this.MallId)); }
		}
		/// <summary>購入回数</summary>
		public string OrderCountOrder { get; set; }
		/// <summary>請求書印字フラグ</summary>
		public string InvoiceBundleFlg { get; set; }
		/// <summary>Online Payment Status</summary>
		public string OnlinePaymentStatus { get; set; }
		/// <summary>デジタルコンテンツ購入フラグ</summary>
		public string DigitalContentsFlg { get; set; }
		/// <summary>External Payment Type</summary>
		public string ExternalPaymentType { get; set; }
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>頒布会コース定額価格</summary>
		public decimal? SubscriptionBoxFixedAmount { get; set; }
		/// <summary>頒布会定額コースか</summary>
		public bool IsSubscriptionBoxFixedAmount
		{
			get
			{
				if (string.IsNullOrEmpty(this.SubscriptionBoxCourseId)) return false;
				return ((this.SubscriptionBoxFixedAmount.HasValue) && (this.SubscriptionBoxFixedAmount.Value != 0));
			}
		}
		/// <summary>頒布会購入回数</summary>
		public int? OrderSubscriptionBoxOrderCount { get; set; }
		/// <summary>同梱注文か</summary>
		public bool IsOrderCombined { get; }
		/// <summary>頒布会商品が含まれる同梱注文か</summary>
		public bool IsOrderCombinedWithSubscriptionBoxItem
		{
			get
			{
				if (this.IsOrderCombined == false) return false;

				var result = this.Items.Any(item => item.IsSubscriptionBox);
				return result;
			}
		}
		/// <summary>頒布会定額コース商品が含まれるか</summary>
		public bool HasSubscriptionBoxFixedAmountItem
		{
			get
			{
				var result = this.Items.Any(item => item.IsSubscriptionBoxFixedAmount);
				return result;
			}
		}
		/// <summary>注文商品に含まれる頒布会コースID配列</summary>
		public string[] ItemSubscriptionBoxCourseIds
		{
			get
			{
				var result = this.Items
					.Where(item => item.IsSubscriptionBox)
					.Select(item => item.SubscriptionBoxCourseId)
					.Distinct()
					.ToArray();
				return result;
			}
		}
		/// <summary>注文商品に含まれる頒布会定額コースID配列</summary>
		public string[] ItemSubscriptionBoxFixedAmountCourseIds
		{
			get
			{
				var result = this.Items
					.Where(item => item.IsSubscriptionBoxFixedAmount)
					.Select(item => item.SubscriptionBoxCourseId)
					.Distinct()
					.ToArray();
				return result;
			}
		}
		/// <summary>商品が全て頒布会定額コース商品か</summary>
		public bool IsAllItemsSubscriptionBoxFixedAmount
		{
			get
			{
				var hasItemsNotFixedAmount = this.Items.Any(item => item.IsSubscriptionBoxFixedAmount == false);
				return hasItemsNotFixedAmount == false;
			}
		}
		/// <summary>頒布会定額コースごとの返品済み商品個数</summary>
		public Dictionary<string, int> ReturnedItemQuantityByFixedAmountCourse { get; private set; } = new Dictionary<string, int>();
		/// <summary>全返品済みの定期頒布会コースID</summary>
		public string[] AllReturnedFixedAmountCourseIds { get; private set; } = Array.Empty<string>();
		#endregion
	}
}
