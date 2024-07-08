/*
=========================================================================================================
  Module      : 注文情報入力クラス (OrderInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Input.Order.Helper;
using w2.App.Common.Option;
using w2.Common.Util;
using w2.Domain.TwOrderInvoice;
using w2.Domain.Order;
using w2.Domain;
using w2.Domain.ProductTaxCategory;

namespace w2.App.Common.Input.Order
{
	/// <summary>
	/// 注文情報入力クラス
	/// </summary>
	[Serializable]
	public class OrderInput : InputBase<OrderModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderInput()
		{
			this.OrderId = string.Empty;
			this.OrderIdOrg = string.Empty;
			this.OrderGroupId = string.Empty;
			this.OrderNo = string.Empty;
			this.UserId = string.Empty;
			this.ShopId = Constants.CONST_DEFAULT_SHOP_ID;
			this.SupplierId = string.Empty;
			this.OrderKbn = Constants.FLG_ORDER_ORDER_KBN_PC;
			this.MallId = string.Empty;
			this.MallName = string.Empty;
			this.MallKbn = string.Empty;
			this.OrderPaymentKbn = string.Empty;
			this.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP;
			this.OrderDate = null;
			this.OrderRecognitionDate = null;
			this.OrderStockreservedStatus = Constants.FLG_ORDER_ORDER_STOCKRESERVED_STATUS_UNKNOWN;
			this.OrderStockreservedDate = null;
			this.OrderShippingDate = null;
			this.OrderShippedStatus = Constants.FLG_ORDER_ORDER_SHIPPED_STATUS_UNKNOWN;
			this.OrderShippedDate = null;
			this.OrderDeliveringDate = null;
			this.OrderCancelDate = null;
			this.OrderReturnDate = null;
			this.OrderPaymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_UNKNOWN;
			this.OrderPaymentDate = null;
			this.DemandStatus = Constants.FLG_ORDER_DEMAND_STATUS_UNKNOWN;
			this.DemandDate = null;
			this.OrderReturnExchangeStatus = Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_UNKNOWN;
			this.OrderReturnExchangeReceiptDate = null;
			this.OrderReturnExchangeArrivalDate = null;
			this.OrderReturnExchangeCompleteDate = null;
			this.OrderRepaymentStatus = Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_UNKNOWN;
			this.OrderRepaymentDate = null;
			this.OrderItemCount = "0";
			this.OrderProductCount = "0";
			this.OrderPriceSubtotal = "0";
			this.OrderPricePack = "0";
			this.OrderPriceTax = "0";
			this.OrderPriceShipping = "0";
			this.OrderPriceExchange = "0";
			this.OrderPriceRegulation = "0";
			this.OrderPriceRepayment = "0";
			this.OrderPriceTotal = this.OldOrderPriceTotal = "0";
			this.OrderDiscountSetPrice = "0";
			this.OrderPointUse = this.OldOrderPointUse = "0";
			this.OrderPointUseYen = "0";
			this.OrderPointAdd = "0";
			this.OrderPointRate = "0";
			this.OrderCouponUse = "0";
			this.CardKbn = string.Empty;
			this.CardInstruments = string.Empty;
			this.CardTranId = string.Empty;
			this.ShippingId = string.Empty;
			this.FixedPurchaseId = string.Empty;
			this.AdvcodeFirst = string.Empty;
			this.AdvcodeNew = string.Empty;
			this.ShippedChangedKbn = Constants.FLG_ORDER_SHIPPED_CHANGED_KBN_NOCHANAGED;
			this.ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN;
			this.ReturnExchangeReasonKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_UNKNOWN;
			this.Attribute1 = string.Empty;
			this.Attribute2 = string.Empty;
			this.Attribute3 = string.Empty;
			this.Attribute4 = string.Empty;
			this.Attribute5 = string.Empty;
			this.Attribute6 = string.Empty;
			this.Attribute7 = string.Empty;
			this.Attribute8 = string.Empty;
			this.Attribute9 = string.Empty;
			this.Attribute10 = string.Empty;
			this.ExtendStatus1 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate1 = null;
			this.ExtendStatus2 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate2 = null;
			this.ExtendStatus3 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate3 = null;
			this.ExtendStatus4 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate4 = null;
			this.ExtendStatus5 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate5 = null;
			this.ExtendStatus6 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			//test
			this.ExtendStatusDate6 = null;
			this.ExtendStatus7 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate7 = null;
			this.ExtendStatus8 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate8 = null;
			this.ExtendStatus9 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate9 = null;
			this.ExtendStatus10 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate10 = null;
			this.CareerId = string.Empty;
			this.MobileUid = string.Empty;
			this.RemoteAddr = string.Empty;
			this.Memo = string.Empty;
			this.WrappingMemo = string.Empty;
			this.PaymentMemo = string.Empty;
			this.ManagementMemo = string.Empty;
			this.ShippingMemo = "";
			this.RelationMemo = string.Empty;
			this.ReturnExchangeReasonMemo = string.Empty;
			this.DelFlg = "0";
			this.DateCreated = DateTime.Now.ToString();
			this.DateChanged = DateTime.Now.ToString();
			this.LastChanged = string.Empty;
			this.MemberRankDiscountPrice = "0";
			this.MemberRankId = string.Empty;
			this.CreditBranchNo = null;
			this.AffiliateSessionName1 = string.Empty;
			this.AffiliateSessionValue1 = string.Empty;
			this.AffiliateSessionName2 = string.Empty;
			this.AffiliateSessionValue2 = string.Empty;
			this.UserAgent = string.Empty;
			this.GiftFlg = Constants.FLG_ORDER_GIFT_FLG_OFF;
			this.DigitalContentsFlg = Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_OFF;
			this.Card_3dsecureTranId = string.Empty;
			this.Card_3dsecureAuthUrl = string.Empty;
			this.Card_3dsecureAuthKey = string.Empty;
			this.ShippingPriceSeparateEstimatesFlg = Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID;
			this.OrderTaxIncludedFlg = Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX;
			this.OrderTaxRate = "0"; // 使用しない
			this.OrderTaxRoundType = Constants.FLG_ORDER_ORDER_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_DOWN;
			this.ExtendStatus11 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate11 = null;
			this.ExtendStatus12 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate12 = null;
			this.ExtendStatus13 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate13 = null;
			this.ExtendStatus14 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate14 = null;
			this.ExtendStatus15 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate15 = null;
			this.ExtendStatus16 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate16 = null;
			this.ExtendStatus17 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate17 = null;
			this.ExtendStatus18 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate18 = null;
			this.ExtendStatus19 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate19 = null;
			this.ExtendStatus20 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate20 = null;
			this.ExtendStatus21 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate21 = null;
			this.ExtendStatus22 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate22 = null;
			this.ExtendStatus23 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate23 = null;
			this.ExtendStatus24 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate24 = null;
			this.ExtendStatus25 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate25 = null;
			this.ExtendStatus26 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate26 = null;
			this.ExtendStatus27 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate27 = null;
			this.ExtendStatus28 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate28 = null;
			this.ExtendStatus29 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate29 = null;
			this.ExtendStatus30 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate30 = null;
			this.ExtendStatus31 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate31 = null;
			this.ExtendStatus32 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate32 = null;
			this.ExtendStatus33 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate33 = null;
			this.ExtendStatus34 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate34 = null;
			this.ExtendStatus35 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate35 = null;
			this.ExtendStatus36 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate36 = null;
			this.ExtendStatus37 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate37 = null;
			this.ExtendStatus38 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate38 = null;
			this.ExtendStatus39 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate39 = null;
			this.ExtendStatus40 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate40 = null;
			this.CardInstallmentsCode = string.Empty;
			this.SetpromotionProductDiscountAmount = "0";
			this.SetpromotionShippingChargeDiscountAmount = "0";
			this.SetpromotionPaymentChargeDiscountAmount = "0";
			this.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE;
			this.FixedPurchaseOrderCount = null;
			this.FixedPurchaseShippedCount = null;
			this.FixedPurchaseDiscountPrice = "0";
			this.PaymentOrderId = string.Empty;
			this.FixedPurchaseMemberDiscountAmount = "0";
			this.LastBilledAmount = this.OldLastBilledAmount = "0";
			this.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
			this.ExternalPaymentAuthDate = null;
			this.ExternalPaymentErrorMessage = string.Empty;
			this.LastOrderPointUse = this.OldLastOrderPointUse = "0";
			this.LastOrderPointUseYen = "0";
			this.ExternalImportStatus = string.Empty;
			this.ExternalOrderId = string.Empty;
			this.LastAuthFlg = Constants.FLG_ORDER_LAST_AUTH_FLG_OFF;
			this.MallLinkStatus = string.Empty;
			this.OrderPriceSubtotalTax = "0";
			this.OrderPriceExchangeTax = "0";
			this.OrderPriceShippingTax = "0";
			this.ExternalPaymentCooperationLog = "";
			this.ShippingTaxRate = "0";
			this.PaymentTaxRate = "0";
			this.InvoiceBundleFlg = Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			this.InflowContentsId = string.Empty;
			this.InflowContentsType = string.Empty;
			this.ReceiptFlg = Constants.FLG_ORDER_RECEIPT_FLG_OFF;
			this.ReceiptOutputFlg = Constants.FLG_ORDER_RECEIPT_OUTPUT_FLG_OFF;
			this.ReceiptAddress = "";
			this.ReceiptProviso = "";
			this.DeliveryTranId = string.Empty;
			this.OnlineDeliveryStatus = string.Empty;
			this.ExternalPaymentType = string.Empty;
			this.LogiCooperationStatus = string.Empty;
			this.Shippings = new OrderShippingInput[0];
			this.SetPromotions = new OrderSetPromotionInput[0];
			this.ExtendStatus41 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate41 = null;
			this.ExtendStatus42 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate42 = null;
			this.ExtendStatus43 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate43 = null;
			this.ExtendStatus44 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate44 = null;
			this.ExtendStatus45 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate45 = null;
			this.ExtendStatus46 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate46 = null;
			this.ExtendStatus47 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate47 = null;
			this.ExtendStatus48 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate48 = null;
			this.ExtendStatus49 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate49 = null;
			this.ExtendStatus50 = Constants.FLG_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate50 = null;
			this.CardTranPass = string.Empty;
			this.StorePickupStatus = string.Empty;
			this.StorePickupStoreArrivedDate = null;
			this.StorePickupDeliveredCompleteDate = null;
			this.StorePickupReturnDate = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public OrderInput(OrderModel model)
			: this()
		{
			this.OrderId = model.OrderId;
			this.OrderIdOrg = model.OrderIdOrg;
			this.OrderGroupId = model.OrderGroupId;
			this.OrderNo = model.OrderNo;
			this.UserId = model.UserId;
			this.ShopId = model.ShopId;
			this.SupplierId = model.SupplierId;
			this.OrderKbn = model.OrderKbn;
			this.MallId = model.MallId;
			this.MallName = string.Empty;
			this.MallKbn = string.Empty;
			if (Constants.MALLCOOPERATION_OPTION_ENABLED)
			{
				var mallCooperationSetting = DomainFacade.Instance.MallCooperationSettingService.Get(this.ShopId, this.MallId);
				if (mallCooperationSetting != null)
				{
					this.MallName = mallCooperationSetting.MallName;
					this.MallKbn = mallCooperationSetting.MallKbn;
				}
			}
			this.OrderPaymentKbn = model.OrderPaymentKbn;
			var payment = DomainFacade.Instance.PaymentService.Get(this.ShopId, this.OrderPaymentKbn);
			if (payment != null)
			{
				this.PaymentName = payment.PaymentName;
			}
			this.OrderStatus = model.OrderStatus;
			this.OrderDate = (model.OrderDate != null) ? model.OrderDate.ToString() : null;
			this.OrderRecognitionDate = (model.OrderRecognitionDate != null) ? model.OrderRecognitionDate.ToString() : null;
			this.OrderStockreservedStatus = model.OrderStockreservedStatus;
			this.OrderStockreservedDate = (model.OrderStockreservedDate != null) ? model.OrderStockreservedDate.ToString() : null;
			this.OrderShippingDate = (model.OrderShippingDate != null) ? model.OrderShippingDate.ToString() : null;
			this.OrderShippedStatus = model.OrderShippedStatus;
			this.OrderShippedDate = (model.OrderShippedDate != null) ? model.OrderShippedDate.ToString() : null;
			this.OrderDeliveringDate = (model.OrderDeliveringDate != null) ? model.OrderDeliveringDate.ToString() : null;
			this.OrderCancelDate = (model.OrderCancelDate != null) ? model.OrderCancelDate.ToString() : null;
			this.OrderReturnDate = (model.OrderReturnDate != null) ? model.OrderReturnDate.ToString() : null;
			this.OrderPaymentStatus = model.OrderPaymentStatus;
			this.OrderPaymentDate = (model.OrderPaymentDate != null) ? model.OrderPaymentDate.ToString() : null;
			this.DemandStatus = model.DemandStatus;
			this.DemandDate = (model.DemandDate != null) ? model.DemandDate.ToString() : null;
			this.OrderReturnExchangeStatus = model.OrderReturnExchangeStatus;
			this.OrderReturnExchangeReceiptDate = (model.OrderReturnExchangeReceiptDate != null) ? model.OrderReturnExchangeReceiptDate.ToString() : null;
			this.OrderReturnExchangeArrivalDate = (model.OrderReturnExchangeArrivalDate != null) ? model.OrderReturnExchangeArrivalDate.ToString() : null;
			this.OrderReturnExchangeCompleteDate = (model.OrderReturnExchangeCompleteDate != null) ? model.OrderReturnExchangeCompleteDate.ToString() : null;
			this.OrderRepaymentStatus = model.OrderRepaymentStatus;
			this.OrderRepaymentDate = (model.OrderRepaymentDate != null) ? model.OrderRepaymentDate.ToString() : null;
			this.OrderItemCount = model.OrderItemCount.ToString();
			this.OrderProductCount = model.OrderProductCount.ToString();
			this.OrderPriceSubtotal = model.OrderPriceSubtotal.ToString();
			this.OrderPricePack = model.OrderPricePack.ToString();
			this.OrderPriceTax = model.OrderPriceTax.ToString();
			this.OrderPriceShipping = model.OrderPriceShipping.ToString();
			this.OrderPriceExchange = model.OrderPriceExchange.ToString();
			this.OrderPriceRegulation = model.OrderPriceRegulation.ToString();
			this.OrderPriceRepayment = model.OrderPriceRepayment.ToString();
			this.OrderPriceTotal = this.OldOrderPriceTotal = model.OrderPriceTotal.ToString();
			this.OrderDiscountSetPrice = model.OrderDiscountSetPrice.ToString();
			this.OrderPointUse = this.OldOrderPointUse = model.OrderPointUse.ToString();
			this.OrderPointUseYen = model.OrderPointUseYen.ToString();
			this.OrderPointAdd = model.OrderPointAdd.ToString();
			this.OrderPointRate = model.OrderPointRate.ToString();
			this.OrderCouponUse = model.OrderCouponUse.ToString();
			this.CardKbn = model.CardKbn;
			this.CardInstruments = model.CardInstruments;
			this.CardTranId = model.CardTranId;
			this.ShippingId = model.ShippingId;
			this.FixedPurchaseId = model.FixedPurchaseId;
			this.AdvcodeFirst = model.AdvcodeFirst;
			this.AdvcodeNew = model.AdvcodeNew;
			this.ShippedChangedKbn = model.ShippedChangedKbn;
			this.ReturnExchangeKbn = model.ReturnExchangeKbn;
			this.ReturnExchangeReasonKbn = model.ReturnExchangeReasonKbn;
			this.Attribute1 = model.Attribute1;
			this.Attribute2 = model.Attribute2;
			this.Attribute3 = model.Attribute3;
			this.Attribute4 = model.Attribute4;
			this.Attribute5 = model.Attribute5;
			this.Attribute6 = model.Attribute6;
			this.Attribute7 = model.Attribute7;
			this.Attribute8 = model.Attribute8;
			this.Attribute9 = model.Attribute9;
			this.Attribute10 = model.Attribute10;
			this.ExtendStatus1 = model.ExtendStatus1;
			this.ExtendStatusDate1 = (model.ExtendStatusDate1 != null) ? model.ExtendStatusDate1.ToString() : null;
			this.ExtendStatus2 = model.ExtendStatus2;
			this.ExtendStatusDate2 = (model.ExtendStatusDate2 != null) ? model.ExtendStatusDate2.ToString() : null;
			this.ExtendStatus3 = model.ExtendStatus3;
			this.ExtendStatusDate3 = (model.ExtendStatusDate3 != null) ? model.ExtendStatusDate3.ToString() : null;
			this.ExtendStatus4 = model.ExtendStatus4;
			this.ExtendStatusDate4 = (model.ExtendStatusDate4 != null) ? model.ExtendStatusDate4.ToString() : null;
			this.ExtendStatus5 = model.ExtendStatus5;
			this.ExtendStatusDate5 = (model.ExtendStatusDate5 != null) ? model.ExtendStatusDate5.ToString() : null;
			this.ExtendStatus6 = model.ExtendStatus6;
			this.ExtendStatusDate6 = (model.ExtendStatusDate6 != null) ? model.ExtendStatusDate6.ToString() : null;
			this.ExtendStatus7 = model.ExtendStatus7;
			this.ExtendStatusDate7 = (model.ExtendStatusDate7 != null) ? model.ExtendStatusDate7.ToString() : null;
			this.ExtendStatus8 = model.ExtendStatus8;
			this.ExtendStatusDate8 = (model.ExtendStatusDate8 != null) ? model.ExtendStatusDate8.ToString() : null;
			this.ExtendStatus9 = model.ExtendStatus9;
			this.ExtendStatusDate9 = (model.ExtendStatusDate9 != null) ? model.ExtendStatusDate9.ToString() : null;
			this.ExtendStatus10 = model.ExtendStatus10;
			this.ExtendStatusDate10 = (model.ExtendStatusDate10 != null) ? model.ExtendStatusDate10.ToString() : null;
			this.CareerId = model.CareerId;
			this.MobileUid = model.MobileUid;
			this.RemoteAddr = model.RemoteAddr;
			this.Memo = model.Memo;
			this.WrappingMemo = model.WrappingMemo;
			this.PaymentMemo = model.PaymentMemo;
			this.ManagementMemo = model.ManagementMemo;
			this.RelationMemo = model.RelationMemo;
			this.ReturnExchangeReasonMemo = model.ReturnExchangeReasonMemo;
			this.RegulationMemo = model.RegulationMemo;
			this.RepaymentMemo = model.RepaymentMemo;
			this.DelFlg = model.DelFlg;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
			this.MemberRankDiscountPrice = model.MemberRankDiscountPrice.ToString();
			this.MemberRankId = model.MemberRankId;
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				this.MemberRankName = MemberRankOptionUtility.GetMemberRankName(this.MemberRankId);
			}
			this.CreditBranchNo = (model.CreditBranchNo != null) ? model.CreditBranchNo.ToString() : null;
			this.AffiliateSessionName1 = model.AffiliateSessionName1;
			this.AffiliateSessionValue1 = model.AffiliateSessionValue1;
			this.AffiliateSessionName2 = model.AffiliateSessionName2;
			this.AffiliateSessionValue2 = model.AffiliateSessionValue2;
			this.UserAgent = model.UserAgent;
			this.GiftFlg = model.GiftFlg;
			this.DigitalContentsFlg = model.DigitalContentsFlg;
			this.Card_3dsecureTranId = model.Card_3dsecureTranId;
			this.Card_3dsecureAuthUrl = model.Card_3dsecureAuthUrl;
			this.Card_3dsecureAuthKey = model.Card_3dsecureAuthKey;
			this.ShippingPriceSeparateEstimatesFlg = model.ShippingPriceSeparateEstimatesFlg;
			this.OrderTaxIncludedFlg = model.OrderTaxIncludedFlg;
			this.OrderTaxRate = model.OrderTaxRate.ToString();
			this.OrderTaxRoundType = model.OrderTaxRoundType;
			this.ExtendStatus11 = model.ExtendStatus11;
			this.ExtendStatusDate11 = (model.ExtendStatusDate11 != null) ? model.ExtendStatusDate11.ToString() : null;
			this.ExtendStatus12 = model.ExtendStatus12;
			this.ExtendStatusDate12 = (model.ExtendStatusDate12 != null) ? model.ExtendStatusDate12.ToString() : null;
			this.ExtendStatus13 = model.ExtendStatus13;
			this.ExtendStatusDate13 = (model.ExtendStatusDate13 != null) ? model.ExtendStatusDate13.ToString() : null;
			this.ExtendStatus14 = model.ExtendStatus14;
			this.ExtendStatusDate14 = (model.ExtendStatusDate14 != null) ? model.ExtendStatusDate14.ToString() : null;
			this.ExtendStatus15 = model.ExtendStatus15;
			this.ExtendStatusDate15 = (model.ExtendStatusDate15 != null) ? model.ExtendStatusDate15.ToString() : null;
			this.ExtendStatus16 = model.ExtendStatus16;
			this.ExtendStatusDate16 = (model.ExtendStatusDate16 != null) ? model.ExtendStatusDate16.ToString() : null;
			this.ExtendStatus17 = model.ExtendStatus17;
			this.ExtendStatusDate17 = (model.ExtendStatusDate17 != null) ? model.ExtendStatusDate17.ToString() : null;
			this.ExtendStatus18 = model.ExtendStatus18;
			this.ExtendStatusDate18 = (model.ExtendStatusDate18 != null) ? model.ExtendStatusDate18.ToString() : null;
			this.ExtendStatus19 = model.ExtendStatus19;
			this.ExtendStatusDate19 = (model.ExtendStatusDate19 != null) ? model.ExtendStatusDate19.ToString() : null;
			this.ExtendStatus20 = model.ExtendStatus20;
			this.ExtendStatusDate20 = (model.ExtendStatusDate20 != null) ? model.ExtendStatusDate20.ToString() : null;
			this.ExtendStatus21 = model.ExtendStatus21;
			this.ExtendStatusDate21 = (model.ExtendStatusDate21 != null) ? model.ExtendStatusDate21.ToString() : null;
			this.ExtendStatus22 = model.ExtendStatus22;
			this.ExtendStatusDate22 = (model.ExtendStatusDate22 != null) ? model.ExtendStatusDate22.ToString() : null;
			this.ExtendStatus23 = model.ExtendStatus23;
			this.ExtendStatusDate23 = (model.ExtendStatusDate23 != null) ? model.ExtendStatusDate23.ToString() : null;
			this.ExtendStatus24 = model.ExtendStatus24;
			this.ExtendStatusDate24 = (model.ExtendStatusDate24 != null) ? model.ExtendStatusDate24.ToString() : null;
			this.ExtendStatus25 = model.ExtendStatus25;
			this.ExtendStatusDate25 = (model.ExtendStatusDate25 != null) ? model.ExtendStatusDate25.ToString() : null;
			this.ExtendStatus26 = model.ExtendStatus26;
			this.ExtendStatusDate26 = (model.ExtendStatusDate26 != null) ? model.ExtendStatusDate26.ToString() : null;
			this.ExtendStatus27 = model.ExtendStatus27;
			this.ExtendStatusDate27 = (model.ExtendStatusDate27 != null) ? model.ExtendStatusDate27.ToString() : null;
			this.ExtendStatus28 = model.ExtendStatus28;
			this.ExtendStatusDate28 = (model.ExtendStatusDate28 != null) ? model.ExtendStatusDate28.ToString() : null;
			this.ExtendStatus29 = model.ExtendStatus29;
			this.ExtendStatusDate29 = (model.ExtendStatusDate29 != null) ? model.ExtendStatusDate29.ToString() : null;
			this.ExtendStatus30 = model.ExtendStatus30;
			this.ExtendStatusDate30 = (model.ExtendStatusDate30 != null) ? model.ExtendStatusDate30.ToString() : null;
			this.ExtendStatus31 = model.ExtendStatus31;
			this.ExtendStatusDate31 = (model.ExtendStatusDate31 != null) ? model.ExtendStatusDate31.ToString() : null;
			this.ExtendStatus32 = model.ExtendStatus32;
			this.ExtendStatusDate32 = (model.ExtendStatusDate32 != null) ? model.ExtendStatusDate32.ToString() : null;
			this.ExtendStatus33 = model.ExtendStatus33;
			this.ExtendStatusDate33 = (model.ExtendStatusDate33 != null) ? model.ExtendStatusDate33.ToString() : null;
			this.ExtendStatus34 = model.ExtendStatus34;
			this.ExtendStatusDate34 = (model.ExtendStatusDate34 != null) ? model.ExtendStatusDate34.ToString() : null;
			this.ExtendStatus35 = model.ExtendStatus35;
			this.ExtendStatusDate35 = (model.ExtendStatusDate35 != null) ? model.ExtendStatusDate35.ToString() : null;
			this.ExtendStatus36 = model.ExtendStatus36;
			this.ExtendStatusDate36 = (model.ExtendStatusDate36 != null) ? model.ExtendStatusDate36.ToString() : null;
			this.ExtendStatus37 = model.ExtendStatus37;
			this.ExtendStatusDate37 = (model.ExtendStatusDate37 != null) ? model.ExtendStatusDate37.ToString() : null;
			this.ExtendStatus38 = model.ExtendStatus38;
			this.ExtendStatusDate38 = (model.ExtendStatusDate38 != null) ? model.ExtendStatusDate38.ToString() : null;
			this.ExtendStatus39 = model.ExtendStatus39;
			this.ExtendStatusDate39 = (model.ExtendStatusDate39 != null) ? model.ExtendStatusDate39.ToString() : null;
			this.ExtendStatus40 = model.ExtendStatus40;
			this.ExtendStatusDate40 = (model.ExtendStatusDate40 != null) ? model.ExtendStatusDate40.ToString() : null;
			this.CardInstallmentsCode = model.CardInstallmentsCode;
			this.SetpromotionProductDiscountAmount = model.SetpromotionProductDiscountAmount.ToString();
			this.SetpromotionShippingChargeDiscountAmount = model.SetpromotionShippingChargeDiscountAmount.ToString();
			this.SetpromotionPaymentChargeDiscountAmount = model.SetpromotionPaymentChargeDiscountAmount.ToString();
			this.OnlinePaymentStatus = model.OnlinePaymentStatus;
			this.FixedPurchaseOrderCount = (model.FixedPurchaseOrderCount != null) ? model.FixedPurchaseOrderCount.ToString() : null;
			this.FixedPurchaseShippedCount = (model.FixedPurchaseShippedCount != null) ? model.FixedPurchaseShippedCount.ToString() : null;
			this.FixedPurchaseDiscountPrice = model.FixedPurchaseDiscountPrice.ToString();
			this.PaymentOrderId = model.PaymentOrderId;
			this.FixedPurchaseMemberDiscountAmount = model.FixedPurchaseMemberDiscountAmount.ToString();
			// 注文者
			this.Owner = new OrderOwnerInput(model.Owner);
			// 配送先リスト
			this.Shippings = model.Shippings.Select(shipping => new OrderShippingInput(shipping)).ToArray();
			// 税率毎価格情報リスト
			this.OrderPriceByTaxRates = model.OrderPriceByTaxRates.Select(orderPriceByTaxRate =>
					new OrderPriceByTaxRateInput(orderPriceByTaxRate))
				.ToArray();
			// セットプロモーションリスト
			this.SetPromotions = model.SetPromotions.Select(setPromotion => new OrderSetPromotionInput(setPromotion)).ToArray();
			// クーポンリスト
			this.Coupons = model.Coupons.Select(coupon => new OrderCouponInput(coupon)).ToArray();
			// 最終請求金額
			this.LastBilledAmount = this.OldLastBilledAmount = model.LastBilledAmount.ToString();
			// 外部決済ステータス
			this.ExternalPaymentStatus = StringUtility.ToEmpty(model.ExternalPaymentStatus);
			this.ExternalPaymentAuthDate = (model.ExternalPaymentAuthDate != null) ? model.ExternalPaymentAuthDate.ToString() : null;
			this.ExternalPaymentErrorMessage = model.ExternalPaymentErrorMessage;
			// 最終利用ポイント、最終ポイント利用額
			this.LastOrderPointUse = this.OldLastOrderPointUse = model.LastOrderPointUse.ToString();
			this.LastOrderPointUseYen = model.LastOrderPointUseYen.ToString();
			// 外部連携取込ステータス
			this.ExternalOrderId = model.ExternalOrderId;
			this.ExternalImportStatus = model.ExternalImportStatus;

			this.CombinedOrgOrderIds = model.CombinedOrgOrderIds;
			// 最終与信フラグ
			this.LastAuthFlg = model.LastAuthFlg;
			// モール連携ステータス
			this.MallLinkStatus = model.MallLinkStatus;
			this.OrderPriceSubtotalTax = model.OrderPriceSubtotalTax.ToString();
			this.SettlementCurrency = model.SettlementCurrency.ToString();
			this.SettlementRate = model.SettlementRate.ToString();
			this.SettlementAmount = model.SettlementAmount.ToString();
			// 配送メモ
			this.ShippingMemo = model.ShippingMemo;
			// 外部連携決済エラーログ
			this.ExternalPaymentCooperationLog = model.ExternalPaymentCooperationLog;
			this.ShippingTaxRate = model.ShippingTaxRate.ToString();
			this.PaymentTaxRate = model.PaymentTaxRate.ToString();
			// 購入回数
			this.OrderCountOrder = (model.OrderCountOrder != null) ? model.OrderCountOrder.ToString() : null;
			this.InvoiceBundleFlg = model.InvoiceBundleFlg;
			// コンバージョン
			this.InflowContentsId = model.InflowContentsId;
			this.InflowContentsType = model.InflowContentsType;
			// 領収書希望フラグ
			this.ReceiptFlg = model.ReceiptFlg;
			// 領収書出力フラグ
			this.ReceiptOutputFlg = model.ReceiptOutputFlg;
			// 領収書の宛名
			this.ReceiptAddress = model.ReceiptAddress;
			// 領収書の但し書き
			this.ReceiptProviso = model.ReceiptProviso;
			this.DeliveryTranId = model.DeliveryTranId;
			this.OnlineDeliveryStatus = model.OnlineDeliveryStatus;
			// 外部決済タイプ
			this.ExternalPaymentType = model.ExternalPaymentType;
			this.LogiCooperationStatus = model.LogiCooperationStatus;
			// Extend status 41 ~ 50
			this.ExtendStatus41 = model.ExtendStatus41;
			this.ExtendStatusDate41 = (model.ExtendStatusDate41 != null) ? model.ExtendStatusDate41.ToString() : null;
			this.ExtendStatus42 = model.ExtendStatus42;
			this.ExtendStatusDate42 = (model.ExtendStatusDate42 != null) ? model.ExtendStatusDate42.ToString() : null;
			this.ExtendStatus43 = model.ExtendStatus43;
			this.ExtendStatusDate43 = (model.ExtendStatusDate43 != null) ? model.ExtendStatusDate43.ToString() : null;
			this.ExtendStatus44 = model.ExtendStatus44;
			this.ExtendStatusDate44 = (model.ExtendStatusDate44 != null) ? model.ExtendStatusDate44.ToString() : null;
			this.ExtendStatus45 = model.ExtendStatus45;
			this.ExtendStatusDate45 = (model.ExtendStatusDate45 != null) ? model.ExtendStatusDate45.ToString() : null;
			this.ExtendStatus46 = model.ExtendStatus46;
			this.ExtendStatusDate46 = (model.ExtendStatusDate46 != null) ? model.ExtendStatusDate46.ToString() : null;
			this.ExtendStatus47 = model.ExtendStatus47;
			this.ExtendStatusDate47 = (model.ExtendStatusDate47 != null) ? model.ExtendStatusDate47.ToString() : null;
			this.ExtendStatus48 = model.ExtendStatus48;
			this.ExtendStatusDate48 = (model.ExtendStatusDate48 != null) ? model.ExtendStatusDate48.ToString() : null;
			this.ExtendStatus49 = model.ExtendStatus49;
			this.ExtendStatusDate49 = (model.ExtendStatusDate49 != null) ? model.ExtendStatusDate49.ToString() : null;
			this.ExtendStatus50 = model.ExtendStatus50;
			this.ExtendStatusDate50 = (model.ExtendStatusDate50 != null) ? model.ExtendStatusDate50.ToString() : null;
			this.CardTranPass = model.CardTranPass;
			this.SubscriptionBoxCourseId = model.SubscriptionBoxCourseId;
			this.SubscriptionBoxFixedAmount = (model.SubscriptionBoxFixedAmount.HasValue)
				? StringUtility.ToEmpty(model.SubscriptionBoxFixedAmount.Value)
				: string.Empty;
			this.OrderSubscriptionBoxOrderCount = (model.OrderSubscriptionBoxOrderCount.ToString());
			this.StorePickupStatus = model.StorePickupStatus;
			this.StorePickupStoreArrivedDate =
				(model.StorePickupStoreArrivedDate != null)
					? model.StorePickupStoreArrivedDate.ToString()
					: null;
			this.StorePickupDeliveredCompleteDate =
				(model.StorePickupDeliveredCompleteDate != null)
					? model.StorePickupDeliveredCompleteDate.ToString()
					: null;
			this.StorePickupReturnDate =
				(model.StorePickupReturnDate != null)
					? model.StorePickupReturnDate.ToString()
					: null;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override OrderModel CreateModel()
		{
			int parsedOrderSubscriptionBoxOrderCount;
			var model = new OrderModel
			{
				OrderId = this.OrderId,
				OrderIdOrg = this.OrderIdOrg,
				OrderGroupId = this.OrderGroupId,
				OrderNo = this.OrderNo,
				UserId = this.UserId,
				ShopId = this.ShopId,
				SupplierId = this.SupplierId,
				OrderKbn = this.OrderKbn,
				MallId = this.MallId,
				OrderPaymentKbn = this.OrderPaymentKbn,
				OrderStatus = this.OrderStatus,
				OrderDate = (this.OrderDate != null) ? DateTime.Parse(this.OrderDate) : (DateTime?)null,
				OrderRecognitionDate = (this.OrderRecognitionDate != null) ? DateTime.Parse(this.OrderRecognitionDate) : (DateTime?)null,
				OrderStockreservedStatus = this.OrderStockreservedStatus,
				OrderStockreservedDate = (this.OrderStockreservedDate != null) ? DateTime.Parse(this.OrderStockreservedDate) : (DateTime?)null,
				OrderShippingDate = (this.OrderShippingDate != null) ? DateTime.Parse(this.OrderShippingDate) : (DateTime?)null,
				OrderShippedStatus = this.OrderShippedStatus,
				OrderShippedDate = (this.OrderShippedDate != null) ? DateTime.Parse(this.OrderShippedDate) : (DateTime?)null,
				OrderDeliveringDate = (this.OrderDeliveringDate != null) ? DateTime.Parse(this.OrderDeliveringDate) : (DateTime?)null,
				OrderCancelDate = (this.OrderCancelDate != null) ? DateTime.Parse(this.OrderCancelDate) : (DateTime?)null,
				OrderReturnDate = (this.OrderReturnDate != null) ? DateTime.Parse(this.OrderReturnDate) : (DateTime?)null,
				OrderPaymentStatus = this.OrderPaymentStatus,
				OrderPaymentDate = (this.OrderPaymentDate != null) ? DateTime.Parse(this.OrderPaymentDate) : (DateTime?)null,
				DemandStatus = this.DemandStatus,
				DemandDate = (this.DemandDate != null) ? DateTime.Parse(this.DemandDate) : (DateTime?)null,
				OrderReturnExchangeStatus = this.OrderReturnExchangeStatus,
				OrderReturnExchangeReceiptDate = (this.OrderReturnExchangeReceiptDate != null) ? DateTime.Parse(this.OrderReturnExchangeReceiptDate) : (DateTime?)null,
				OrderReturnExchangeArrivalDate = (this.OrderReturnExchangeArrivalDate != null) ? DateTime.Parse(this.OrderReturnExchangeArrivalDate) : (DateTime?)null,
				OrderReturnExchangeCompleteDate = (this.OrderReturnExchangeCompleteDate != null) ? DateTime.Parse(this.OrderReturnExchangeCompleteDate) : (DateTime?)null,
				OrderRepaymentStatus = this.OrderRepaymentStatus,
				OrderRepaymentDate = (this.OrderRepaymentDate != null) ? DateTime.Parse(this.OrderRepaymentDate) : (DateTime?)null,
				OrderItemCount = int.Parse(this.OrderItemCount),
				OrderProductCount = int.Parse(this.OrderProductCount),
				OrderPriceSubtotal = decimal.Parse(this.OrderPriceSubtotal),
				OrderPricePack = decimal.Parse(this.OrderPricePack),
				OrderPriceTax = decimal.Parse(this.OrderPriceTax),
				OrderPriceSubtotalTax = decimal.Parse(this.OrderPriceSubtotalTax),
				ShippingTaxRate = decimal.Parse(this.ShippingTaxRate),
				PaymentTaxRate = decimal.Parse(this.PaymentTaxRate),
				OrderPriceShipping = decimal.Parse(this.OrderPriceShipping),
				OrderPriceExchange = decimal.Parse(this.OrderPriceExchange),
				OrderPriceRegulation = decimal.Parse(this.OrderPriceRegulation),
				OrderPriceRepayment = decimal.Parse(this.OrderPriceRepayment),
				OrderPriceTotal = decimal.Parse(this.OrderPriceTotal),
				OrderDiscountSetPrice = decimal.Parse(this.OrderDiscountSetPrice),
				OrderPointUse = decimal.Parse(this.OrderPointUse),
				OrderPointUseYen = decimal.Parse(this.OrderPointUseYen),
				OrderPointAdd = decimal.Parse(this.OrderPointAdd),
				OrderPointRate = decimal.Parse(this.OrderPointRate),
				OrderCouponUse = decimal.Parse(this.OrderCouponUse),
				CardKbn = this.CardKbn,
				CardInstruments = this.CardInstruments,
				CardTranId = this.CardTranId,
				ShippingId = this.ShippingId,
				FixedPurchaseId = this.FixedPurchaseId,
				AdvcodeFirst = this.AdvcodeFirst,
				AdvcodeNew = this.AdvcodeNew,
				InflowContentsType = this.InflowContentsType,
				InflowContentsId = this.InflowContentsId,
				ShippedChangedKbn = this.ShippedChangedKbn,
				ReturnExchangeKbn = this.ReturnExchangeKbn,
				ReturnExchangeReasonKbn = this.ReturnExchangeReasonKbn,
				Attribute1 = this.Attribute1,
				Attribute2 = this.Attribute2,
				Attribute3 = this.Attribute3,
				Attribute4 = this.Attribute4,
				Attribute5 = this.Attribute5,
				Attribute6 = this.Attribute6,
				Attribute7 = this.Attribute7,
				Attribute8 = this.Attribute8,
				Attribute9 = this.Attribute9,
				Attribute10 = this.Attribute10,
				ExtendStatus1 = this.ExtendStatus1,
				ExtendStatusDate1 = (this.ExtendStatusDate1 != null) ? DateTime.Parse(this.ExtendStatusDate1) : (DateTime?)null,
				ExtendStatus2 = this.ExtendStatus2,
				ExtendStatusDate2 = (this.ExtendStatusDate2 != null) ? DateTime.Parse(this.ExtendStatusDate2) : (DateTime?)null,
				ExtendStatus3 = this.ExtendStatus3,
				ExtendStatusDate3 = (this.ExtendStatusDate3 != null) ? DateTime.Parse(this.ExtendStatusDate3) : (DateTime?)null,
				ExtendStatus4 = this.ExtendStatus4,
				ExtendStatusDate4 = (this.ExtendStatusDate4 != null) ? DateTime.Parse(this.ExtendStatusDate4) : (DateTime?)null,
				ExtendStatus5 = this.ExtendStatus5,
				ExtendStatusDate5 = (this.ExtendStatusDate5 != null) ? DateTime.Parse(this.ExtendStatusDate5) : (DateTime?)null,
				ExtendStatus6 = this.ExtendStatus6,
				ExtendStatusDate6 = (this.ExtendStatusDate6 != null) ? DateTime.Parse(this.ExtendStatusDate6) : (DateTime?)null,
				ExtendStatus7 = this.ExtendStatus7,
				ExtendStatusDate7 = (this.ExtendStatusDate7 != null) ? DateTime.Parse(this.ExtendStatusDate7) : (DateTime?)null,
				ExtendStatus8 = this.ExtendStatus8,
				ExtendStatusDate8 = (this.ExtendStatusDate8 != null) ? DateTime.Parse(this.ExtendStatusDate8) : (DateTime?)null,
				ExtendStatus9 = this.ExtendStatus9,
				ExtendStatusDate9 = (this.ExtendStatusDate9 != null) ? DateTime.Parse(this.ExtendStatusDate9) : (DateTime?)null,
				ExtendStatus10 = this.ExtendStatus10,
				ExtendStatusDate10 = (this.ExtendStatusDate10 != null) ? DateTime.Parse(this.ExtendStatusDate10) : (DateTime?)null,
				CareerId = this.CareerId,
				MobileUid = this.MobileUid,
				RemoteAddr = this.RemoteAddr,
				Memo = this.Memo,
				WrappingMemo = this.WrappingMemo,
				PaymentMemo = this.PaymentMemo,
				ManagementMemo = this.ManagementMemo,
				ShippingMemo = this.ShippingMemo,
				RelationMemo = this.RelationMemo,
				ReturnExchangeReasonMemo = this.ReturnExchangeReasonMemo,
				RegulationMemo = this.RegulationMemo,
				RepaymentMemo = this.RepaymentMemo,
				DelFlg = this.DelFlg,
				DateCreated = DateTime.Parse(this.DateCreated),
				DateChanged = DateTime.Parse(this.DateChanged),
				LastChanged = this.LastChanged,
				MemberRankDiscountPrice = decimal.Parse(this.MemberRankDiscountPrice),
				MemberRankId = this.MemberRankId,
				CreditBranchNo = (this.CreditBranchNo != null) ? int.Parse(this.CreditBranchNo) : (int?)null,
				AffiliateSessionName1 = this.AffiliateSessionName1,
				AffiliateSessionValue1 = this.AffiliateSessionValue1,
				AffiliateSessionName2 = this.AffiliateSessionName2,
				AffiliateSessionValue2 = this.AffiliateSessionValue2,
				UserAgent = this.UserAgent,
				GiftFlg = this.GiftFlg,
				DigitalContentsFlg = this.DigitalContentsFlg,
				Card_3dsecureTranId = this.Card_3dsecureTranId,
				Card_3dsecureAuthUrl = this.Card_3dsecureAuthUrl,
				Card_3dsecureAuthKey = this.Card_3dsecureAuthKey,
				ShippingPriceSeparateEstimatesFlg = this.ShippingPriceSeparateEstimatesFlg,
				OrderTaxIncludedFlg = this.OrderTaxIncludedFlg,
				OrderTaxRate = decimal.Parse(this.OrderTaxRate),
				OrderTaxRoundType = this.OrderTaxRoundType,
				ExtendStatus11 = this.ExtendStatus11,
				ExtendStatusDate11 = (this.ExtendStatusDate11 != null) ? DateTime.Parse(this.ExtendStatusDate11) : (DateTime?)null,
				ExtendStatus12 = this.ExtendStatus12,
				ExtendStatusDate12 = (this.ExtendStatusDate12 != null) ? DateTime.Parse(this.ExtendStatusDate12) : (DateTime?)null,
				ExtendStatus13 = this.ExtendStatus13,
				ExtendStatusDate13 = (this.ExtendStatusDate13 != null) ? DateTime.Parse(this.ExtendStatusDate13) : (DateTime?)null,
				ExtendStatus14 = this.ExtendStatus14,
				ExtendStatusDate14 = (this.ExtendStatusDate14 != null) ? DateTime.Parse(this.ExtendStatusDate14) : (DateTime?)null,
				ExtendStatus15 = this.ExtendStatus15,
				ExtendStatusDate15 = (this.ExtendStatusDate15 != null) ? DateTime.Parse(this.ExtendStatusDate15) : (DateTime?)null,
				ExtendStatus16 = this.ExtendStatus16,
				ExtendStatusDate16 = (this.ExtendStatusDate16 != null) ? DateTime.Parse(this.ExtendStatusDate16) : (DateTime?)null,
				ExtendStatus17 = this.ExtendStatus17,
				ExtendStatusDate17 = (this.ExtendStatusDate17 != null) ? DateTime.Parse(this.ExtendStatusDate17) : (DateTime?)null,
				ExtendStatus18 = this.ExtendStatus18,
				ExtendStatusDate18 = (this.ExtendStatusDate18 != null) ? DateTime.Parse(this.ExtendStatusDate18) : (DateTime?)null,
				ExtendStatus19 = this.ExtendStatus19,
				ExtendStatusDate19 = (this.ExtendStatusDate19 != null) ? DateTime.Parse(this.ExtendStatusDate19) : (DateTime?)null,
				ExtendStatus20 = this.ExtendStatus20,
				ExtendStatusDate20 = (this.ExtendStatusDate20 != null) ? DateTime.Parse(this.ExtendStatusDate20) : (DateTime?)null,
				ExtendStatus21 = this.ExtendStatus21,
				ExtendStatusDate21 = (this.ExtendStatusDate21 != null) ? DateTime.Parse(this.ExtendStatusDate21) : (DateTime?)null,
				ExtendStatus22 = this.ExtendStatus22,
				ExtendStatusDate22 = (this.ExtendStatusDate22 != null) ? DateTime.Parse(this.ExtendStatusDate22) : (DateTime?)null,
				ExtendStatus23 = this.ExtendStatus23,
				ExtendStatusDate23 = (this.ExtendStatusDate23 != null) ? DateTime.Parse(this.ExtendStatusDate23) : (DateTime?)null,
				ExtendStatus24 = this.ExtendStatus24,
				ExtendStatusDate24 = (this.ExtendStatusDate24 != null) ? DateTime.Parse(this.ExtendStatusDate24) : (DateTime?)null,
				ExtendStatus25 = this.ExtendStatus25,
				ExtendStatusDate25 = (this.ExtendStatusDate25 != null) ? DateTime.Parse(this.ExtendStatusDate25) : (DateTime?)null,
				ExtendStatus26 = this.ExtendStatus26,
				ExtendStatusDate26 = (this.ExtendStatusDate26 != null) ? DateTime.Parse(this.ExtendStatusDate26) : (DateTime?)null,
				ExtendStatus27 = this.ExtendStatus27,
				ExtendStatusDate27 = (this.ExtendStatusDate27 != null) ? DateTime.Parse(this.ExtendStatusDate27) : (DateTime?)null,
				ExtendStatus28 = this.ExtendStatus28,
				ExtendStatusDate28 = (this.ExtendStatusDate28 != null) ? DateTime.Parse(this.ExtendStatusDate28) : (DateTime?)null,
				ExtendStatus29 = this.ExtendStatus29,
				ExtendStatusDate29 = (this.ExtendStatusDate29 != null) ? DateTime.Parse(this.ExtendStatusDate29) : (DateTime?)null,
				ExtendStatus30 = this.ExtendStatus30,
				ExtendStatusDate30 = (this.ExtendStatusDate30 != null) ? DateTime.Parse(this.ExtendStatusDate30) : (DateTime?)null,
				ExtendStatus31 = this.ExtendStatus31,
				ExtendStatusDate31 = (this.ExtendStatusDate31 != null) ? DateTime.Parse(this.ExtendStatusDate31) : (DateTime?)null,
				ExtendStatus32 = this.ExtendStatus32,
				ExtendStatusDate32 = (this.ExtendStatusDate32 != null) ? DateTime.Parse(this.ExtendStatusDate32) : (DateTime?)null,
				ExtendStatus33 = this.ExtendStatus33,
				ExtendStatusDate33 = (this.ExtendStatusDate33 != null) ? DateTime.Parse(this.ExtendStatusDate33) : (DateTime?)null,
				ExtendStatus34 = this.ExtendStatus34,
				ExtendStatusDate34 = (this.ExtendStatusDate34 != null) ? DateTime.Parse(this.ExtendStatusDate34) : (DateTime?)null,
				ExtendStatus35 = this.ExtendStatus35,
				ExtendStatusDate35 = (this.ExtendStatusDate35 != null) ? DateTime.Parse(this.ExtendStatusDate35) : (DateTime?)null,
				ExtendStatus36 = this.ExtendStatus36,
				ExtendStatusDate36 = (this.ExtendStatusDate36 != null) ? DateTime.Parse(this.ExtendStatusDate36) : (DateTime?)null,
				ExtendStatus37 = this.ExtendStatus37,
				ExtendStatusDate37 = (this.ExtendStatusDate37 != null) ? DateTime.Parse(this.ExtendStatusDate37) : (DateTime?)null,
				ExtendStatus38 = this.ExtendStatus38,
				ExtendStatusDate38 = (this.ExtendStatusDate38 != null) ? DateTime.Parse(this.ExtendStatusDate38) : (DateTime?)null,
				ExtendStatus39 = this.ExtendStatus39,
				ExtendStatusDate39 = (this.ExtendStatusDate39 != null) ? DateTime.Parse(this.ExtendStatusDate39) : (DateTime?)null,
				ExtendStatus40 = this.ExtendStatus40,
				ExtendStatusDate40 = (this.ExtendStatusDate40 != null) ? DateTime.Parse(this.ExtendStatusDate40) : (DateTime?)null,
				CardInstallmentsCode = this.CardInstallmentsCode,
				SetpromotionProductDiscountAmount = decimal.Parse(this.SetpromotionProductDiscountAmount),
				SetpromotionShippingChargeDiscountAmount = decimal.Parse(this.SetpromotionShippingChargeDiscountAmount),
				SetpromotionPaymentChargeDiscountAmount = decimal.Parse(this.SetpromotionPaymentChargeDiscountAmount),
				OnlinePaymentStatus = this.OnlinePaymentStatus,
				FixedPurchaseOrderCount = (this.FixedPurchaseOrderCount != null) ? int.Parse(this.FixedPurchaseOrderCount) : (int?)null,
				FixedPurchaseShippedCount = (this.FixedPurchaseShippedCount != null) ? int.Parse(this.FixedPurchaseShippedCount) : (int?)null,
				FixedPurchaseDiscountPrice = decimal.Parse(this.FixedPurchaseDiscountPrice),
				PaymentOrderId = this.PaymentOrderId,
				FixedPurchaseMemberDiscountAmount = decimal.Parse(this.FixedPurchaseMemberDiscountAmount),
				LastBilledAmount = decimal.Parse(this.LastBilledAmount),
				ExternalPaymentStatus = this.ExternalPaymentStatus,
				ExternalPaymentAuthDate = (this.ExternalPaymentAuthDate != null) ? DateTime.Parse(this.ExternalPaymentAuthDate) : (DateTime?)null,
				ExternalPaymentErrorMessage = this.ExternalPaymentErrorMessage,
				LastOrderPointUse = decimal.Parse(this.LastOrderPointUse),
				LastOrderPointUseYen = decimal.Parse(this.LastOrderPointUseYen),
				SettlementCurrency = this.SettlementCurrency,
				SettlementRate = decimal.Parse(this.SettlementRate),
				SettlementAmount = decimal.Parse(this.SettlementAmount),
				InvoiceBundleFlg = this.InvoiceBundleFlg,
				// 最終与信フラグ
				LastAuthFlg = this.LastAuthFlg,
				ExternalOrderId = this.ExternalOrderId,
				ExternalImportStatus = this.ExternalImportStatus,
				MallLinkStatus = this.MallLinkStatus,
				// 注文者
				Owner = this.Owner.CreateModel(),
				// 注文配送先
				Shippings = this.Shippings.Select(orderShipping => orderShipping.CreateModel()).ToArray(),
				// 税率毎価格情報
				OrderPriceByTaxRates = this.OrderPriceByTaxRates.Select(orderPriceByTaxRate => orderPriceByTaxRate.CreateModel()).ToArray(),
				// 注文クーポン
				Coupons = this.Coupons.Select(orderCoupon => orderCoupon.CreateModel()).ToArray(),
				// 注文セットプロモーション
				SetPromotions = this.SetPromotions.Select(orderSetPromotion => orderSetPromotion.CreateModel()).ToArray(),
				ReceiptFlg = this.ReceiptFlg,
				ReceiptOutputFlg = this.ReceiptOutputFlg,
				ReceiptAddress = this.ReceiptAddress,
				ReceiptProviso = this.ReceiptProviso,
				DeliveryTranId = StringUtility.ToEmpty(this.DeliveryTranId),
				OnlineDeliveryStatus = StringUtility.ToEmpty(this.OnlineDeliveryStatus),
				ExternalPaymentType = this.ExternalPaymentType,
				LogiCooperationStatus = StringUtility.ToEmpty(this.LogiCooperationStatus),
				CombinedOrgOrderIds = this.CombinedOrgOrderIds,
				// Extend status 41 ~ 50
				ExtendStatus41 = this.ExtendStatus41,
				ExtendStatusDate41 = (this.ExtendStatusDate41 != null) ? DateTime.Parse(this.ExtendStatusDate41) : (DateTime?)null,
				ExtendStatus42 = this.ExtendStatus42,
				ExtendStatusDate42 = (this.ExtendStatusDate42 != null) ? DateTime.Parse(this.ExtendStatusDate42) : (DateTime?)null,
				ExtendStatus43 = this.ExtendStatus43,
				ExtendStatusDate43 = (this.ExtendStatusDate43 != null) ? DateTime.Parse(this.ExtendStatusDate43) : (DateTime?)null,
				ExtendStatus44 = this.ExtendStatus44,
				ExtendStatusDate44 = (this.ExtendStatusDate44 != null) ? DateTime.Parse(this.ExtendStatusDate44) : (DateTime?)null,
				ExtendStatus45 = this.ExtendStatus45,
				ExtendStatusDate45 = (this.ExtendStatusDate45 != null) ? DateTime.Parse(this.ExtendStatusDate45) : (DateTime?)null,
				ExtendStatus46 = this.ExtendStatus46,
				ExtendStatusDate46 = (this.ExtendStatusDate46 != null) ? DateTime.Parse(this.ExtendStatusDate46) : (DateTime?)null,
				ExtendStatus47 = this.ExtendStatus47,
				ExtendStatusDate47 = (this.ExtendStatusDate47 != null) ? DateTime.Parse(this.ExtendStatusDate47) : (DateTime?)null,
				ExtendStatus48 = this.ExtendStatus48,
				ExtendStatusDate48 = (this.ExtendStatusDate48 != null) ? DateTime.Parse(this.ExtendStatusDate48) : (DateTime?)null,
				ExtendStatus49 = this.ExtendStatus49,
				ExtendStatusDate49 = (this.ExtendStatusDate49 != null) ? DateTime.Parse(this.ExtendStatusDate49) : (DateTime?)null,
				ExtendStatus50 = this.ExtendStatus50,
				ExtendStatusDate50 = (this.ExtendStatusDate50 != null) ? DateTime.Parse(this.ExtendStatusDate50) : (DateTime?)null,
				CardTranPass = this.CardTranPass,
				SubscriptionBoxCourseId = this.SubscriptionBoxCourseId,
				SubscriptionBoxFixedAmount = string.IsNullOrEmpty(this.SubscriptionBoxFixedAmount)
					? (decimal?)null
					: decimal.Parse(this.SubscriptionBoxFixedAmount),
				OrderSubscriptionBoxOrderCount =
					int.TryParse(this.OrderSubscriptionBoxOrderCount, out parsedOrderSubscriptionBoxOrderCount)
						? parsedOrderSubscriptionBoxOrderCount
						: 0,
				StorePickupStatus = this.StorePickupStatus,
				StorePickupStoreArrivedDate =
					(string.IsNullOrEmpty(this.StorePickupStoreArrivedDate) == false)
						? DateTime.Parse(this.StorePickupStoreArrivedDate)
						: (DateTime?)null,
				StorePickupDeliveredCompleteDate =
					(string.IsNullOrEmpty(this.StorePickupDeliveredCompleteDate) == false)
						? DateTime.Parse(this.StorePickupDeliveredCompleteDate)
						: (DateTime?)null,
				StorePickupReturnDate =
					(string.IsNullOrEmpty(this.StorePickupReturnDate) == false)
						? DateTime.Parse(this.StorePickupReturnDate)
						: (DateTime?)null,
			};

			if (Constants.ORDER_EXTEND_OPTION_ENABLED)
			{
				foreach (var field in Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST)
				{
					if (this.DataSource.ContainsKey(field)
					    && (this.OrderExtendInput != null)
					    && this.OrderExtendInput.ContainsKey(field))
					{
						this.DataSource[field] = this.OrderExtendInput[field];
					}
				}
			}

			return model;
		}

		/// <summary>
		/// 検証（注文）
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string ValidateForOrder()
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, this.OrderPaymentKbn },
			};
			var errorMessages = Validator.Validate("OrderModifyInput", input);
			return string.Join("<br />", errorMessages.Select(kvp => kvp.Value));
		}

		/// <summary>
		/// 検証（配送料金、決済手数料、セットプロモーション割引額）
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string ValidateForOrderPrice()
		{
			var errorMessages = Validator.Validate("OrderItemModifyInput", this.DataSource);
			var result = string.Join("<br />", errorMessages.Select(kvp => kvp.Value));
			result += string.Join(string.Empty, this.SetPromotions.Select(sp => sp.Validate()));
			return result;
		}

		/// <summary>
		/// 検証（注文クーポン）
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string ValidateForOrderCoupon()
		{
			if (this.Coupon == null) return string.Empty;
			var errorMessages = Validator.Validate("OrderCouponModifyInput", this.Coupon.DataSource);
			return string.Join("<br />", errorMessages.Select(kvp => kvp.Value));
		}

		/// <summary>
		/// 検証（注文アフィリエイト）
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string ValidateForOrderAffiliate()
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ORDER_ADVCODE_FIRST, this.AdvcodeFirst },
				{Constants.FIELD_ORDER_ADVCODE_NEW, this.AdvcodeNew }
			};
			var errorMessages = Validator.Validate("OrderModifyInput", input);
			return string.Join("<br />", errorMessages.Select(kvp => kvp.Value));
		}

		/// <summary>
		/// 別出荷フラグをチェック
		/// </summary>
		/// <param name="shipping">配送先情報</param>
		/// <returns>有効:true</returns>
		public bool IsAnotherShippingFlagValid(OrderShippingInput shipping)
		{
			var orderOwner = this.Owner;
			return ((StringUtility.ToEmpty(orderOwner.OwnerName1) != (StringUtility.ToEmpty(shipping.ShippingName1)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerName2) != (StringUtility.ToEmpty(shipping.ShippingName2)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerZip) != (StringUtility.ToEmpty(shipping.ShippingZip)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerAddr1) != (StringUtility.ToEmpty(shipping.ShippingAddr1)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerAddr2) != (StringUtility.ToEmpty(shipping.ShippingAddr2)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerAddr3) != (StringUtility.ToEmpty(shipping.ShippingAddr3)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerAddr4) != (StringUtility.ToEmpty(shipping.ShippingAddr4)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerAddr5) != (StringUtility.ToEmpty(shipping.ShippingAddr5)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerAddrCountryIsoCode) != (StringUtility.ToEmpty(shipping.ShippingCountryIsoCode)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerCompanyName) != (StringUtility.ToEmpty(shipping.ShippingCompanyName)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerCompanyPostName) != (StringUtility.ToEmpty(shipping.ShippingCompanyPostName)))
				|| (StringUtility.ToEmpty(orderOwner.OwnerTel1) != (StringUtility.ToEmpty(shipping.ShippingTel1))));
		}

		/// <summary>
		/// セットプロモーション名取得
		/// </summary>
		/// <param name="orderSetPromotionNo">注文セットプロモーション枝番</param>
		/// <returns>セットプロモーション名</returns>
		public string GetOrderSetPromotionName(string orderSetPromotionNo)
		{
			var setPromotion = this.SetPromotions.FirstOrDefault(sp => (sp.OrderSetpromotionNo == orderSetPromotionNo));
			if (setPromotion == null) return null;

			return setPromotion.SetpromotionName;
		}

		/// <summary>
		/// 注文配送先情報追加
		/// </summary>
		public void AddShipping()
		{
			var orderShippingNo = this.Shippings.Count() + 1;
			var orderShipping = new OrderShippingInput
			{
				OrderId = this.OrderId,
				OrderShippingNo = orderShippingNo.ToString(),
				ShippingCountryIsoCode = this.Shippings.First().ShippingCountryIsoCode,
				ShippingCountryName = this.Shippings.First().ShippingCountryName,
				SenderCountryIsoCode = this.Shippings.First().SenderCountryIsoCode,
				SenderCountryName = this.Shippings.First().SenderCountryName,
				DeliveryCompanyId = this.Shippings.First().DeliveryCompanyId
			};
			var orderShippings = new List<OrderShippingInput>();
			orderShippings.AddRange(this.Shippings);
			orderShippings.Add(orderShipping);
			this.Shippings = orderShippings.ToArray();
			this.AddItem(orderShippingNo.ToString());
		}

		/// <summary>
		/// 注文配送先情報削除
		/// </summary>
		/// <param name="deleteTargetOrderShippingNo">削除対象の配送先枝番</param>
		public void DeleteShipping(string deleteTargetOrderShippingNo)
		{
			// 削除対象を除外した注文配送先情報取得
			var orderShippings = this.Shippings.Where(s => s.OrderShippingNo != deleteTargetOrderShippingNo).ToArray();

			// 配送先枝番の書換
			var orderShippingNo = 0;
			foreach (var orderShipping in orderShippings)
			{
				orderShippingNo++;
				orderShipping.OrderShippingNo = orderShippingNo.ToString();
				foreach (var orderItem in orderShipping.Items)
				{
					orderItem.OrderShippingNo = orderShippingNo.ToString();
				}
			}
			this.Shippings = orderShippings;
		}

		/// <summary>
		/// 注文商品情報追加
		/// </summary>
		public void AddItem(string orderShippingNo)
		{
			var orderShippings = this.Shippings;
			var orderShipping = orderShippings.First(os => os.OrderShippingNo == orderShippingNo);

			var orderItem = new OrderItemInput()
			{
				OrderId = this.OrderId,
				OrderShippingNo = orderShippingNo,
				OrderItemNo = (this.Shippings.SelectMany(s => s.Items).Count() + 1).ToString(),
				ItemIndex = (orderShipping.Items.Length).ToString(),
			};
			var orderItems = new List<OrderItemInput>();
			orderItems.AddRange(orderShipping.Items);
			orderItems.Add(orderItem);
			orderShipping.Items = OrderInputHelper.SetCanDeletePropertyToOrderItemInput(orderItems);		
		}

		/// <summary>
		/// 注文商品情報削除
		/// </summary>
		/// <param name="orderShippingNo">配送先枝番</param>
		/// <param name="itemIndex">対象のアイテムインデックス</param>
		public void RemoveItem(string orderShippingNo, int itemIndex)
		{
			if (this.Shippings.Any() == false) return;
			var orderShippings = this.Shippings;
			var orderShipping = orderShippings.First(os => os.OrderShippingNo == orderShippingNo);

			var orderItems = new List<OrderItemInput>();
			orderItems.AddRange(orderShipping.Items);
			orderItems.RemoveAt(itemIndex);
			orderShipping.Items = OrderInputHelper.SetCanDeletePropertyToOrderItemInput(orderItems);
		}

		/// <summary>
		/// 更新（※更新対象の項目のみ）
		/// </summary>
		/// <param name="order">注文情報</param>
		public void Update(OrderInput order)
		{
			// 注文
			this.UpdateOrder(order);
			// 注文者
			this.UpdateOrderOwner(order);
			// 注文配送先
			this.UpdateOrderShippings(order);
			// 税率毎価格情報
			this.OrderPriceByTaxRates = order.OrderPriceByTaxRates;
			// 注文セットプロモーション
			this.UpdateSetPromotion(order);
			// 注文クーポン
			this.UpdateOrderCoupon(order);

			// 金額再計算
			Recalculate();
		}

		/// <summary>
		/// 注文情報更新
		/// </summary>
		/// <param name="order">注文情報</param>
		private void UpdateOrder(OrderInput order)
		{
			this.PaymentName = order.PaymentName;
			this.OrderPaymentKbn = order.OrderPaymentKbn;
			this.CardTranId = order.CardTranId;
			this.PaymentOrderId = order.PaymentOrderId;
			this.OrderPriceShipping = order.OrderPriceShipping;
			this.OrderPriceExchange = order.OrderPriceExchange;
			this.OrderPriceRegulation = order.OrderPriceRegulation;
			this.OrderPriceTax = order.OrderPriceTax;
			this.Memo = order.Memo;
			this.PaymentMemo = order.PaymentMemo;
			this.ManagementMemo = order.ManagementMemo;
			this.ShippingMemo = order.ShippingMemo;
			this.RelationMemo = order.RelationMemo;
			this.RegulationMemo = order.RegulationMemo;
			this.CardInstruments = order.CardInstruments;
			this.CardInstallmentsCode = order.CardInstallmentsCode;
			this.SetpromotionProductDiscountAmount = order.SetpromotionProductDiscountAmount;
			this.SetpromotionShippingChargeDiscountAmount = order.SetpromotionShippingChargeDiscountAmount;
			this.SetpromotionPaymentChargeDiscountAmount = order.SetpromotionPaymentChargeDiscountAmount;
			// ポイントOP有効？
			this.OrderPointAdd = "0";
			this.OrderPointUse = "0";
			this.OrderPointUseYen = "0";
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				this.OrderPointAdd = order.OrderPointAdd;
				this.OrderPointUse = order.OrderPointUse;
				this.OrderPointUseYen = order.OrderPointUseYen;
				this.LastOrderPointUse = order.OrderPointUse;
				this.LastOrderPointUseYen = order.OrderPointUseYen;
				this.OldLastOrderPointUse = order.OldLastOrderPointUse;
			}
			// クーポンOP有効？
			this.OrderCouponUse = "0";
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				this.OrderCouponUse = order.OrderCouponUse;
			}
			// 会員ランクOP有効？
			this.MemberRankDiscountPrice = "0";
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				this.MemberRankDiscountPrice = order.MemberRankDiscountPrice;
			}
			// 定期購入OP有効？
			this.FixedPurchaseDiscountPrice = "0";
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				this.FixedPurchaseDiscountPrice = order.FixedPurchaseDiscountPrice;
			}
			// 定期購入OPかつ会員ランクOP有効?
			this.FixedPurchaseMemberDiscountAmount = "0";
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED && Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				this.FixedPurchaseMemberDiscountAmount = order.FixedPurchaseMemberDiscountAmount;
			}
			this.ShippingPriceSeparateEstimatesFlg = order.ShippingPriceSeparateEstimatesFlg;
			this.OrderKbn = order.OrderKbn;
			this.ReturnExchangeKbn = order.ReturnExchangeKbn;
			this.ReturnExchangeReasonKbn = order.ReturnExchangeReasonKbn;
			this.ReturnExchangeReasonMemo = order.ReturnExchangeReasonMemo;
			this.RepaymentMemo = order.RepaymentMemo;
			this.AdvcodeFirst = order.AdvcodeFirst;
			this.AdvcodeNew = order.AdvcodeNew;
			this.InflowContentsType = order.InflowContentsType;
			this.InflowContentsId = order.InflowContentsId;
			this.OldLastBilledAmount = order.OldLastBilledAmount;
			this.ShippingId = order.ShippingId;
			this.OrderPriceTax = order.OrderPriceTax;
			this.OrderPriceSubtotalTax = order.OrderPriceSubtotalTax;
			this.SettlementCurrency = order.SettlementCurrency;
			this.SettlementRate = order.SettlementRate;
			this.SettlementAmount = order.SettlementAmount;
			this.ExternalOrderId = order.ExternalOrderId;
			this.ShippingTaxRate = order.ShippingTaxRate;
			this.PaymentTaxRate = order.PaymentTaxRate;
			this.InvoiceBundleFlg = order.InvoiceBundleFlg;
			this.ReceiptFlg = order.ReceiptFlg;
			this.ReceiptOutputFlg = order.ReceiptOutputFlg;
			this.ReceiptAddress = order.ReceiptAddress;
			this.ReceiptProviso = order.ReceiptProviso;
			this.ExternalPaymentType = ((order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY) 
				|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
				? order.ExternalPaymentType
				: string.Empty;
			this.LogiCooperationStatus = order.LogiCooperationStatus;
			this.StorePickupStatus = order.StorePickupStatus;
			this.StorePickupStoreArrivedDate = order.StorePickupStoreArrivedDate;
			this.StorePickupDeliveredCompleteDate = order.StorePickupDeliveredCompleteDate;
			this.StorePickupReturnDate = order.StorePickupReturnDate;

			if (Constants.ORDER_EXTEND_OPTION_ENABLED)
			{
				this.OrderExtendInput = order.OrderExtendInput;
			}
		}

		/// <summary>
		/// 注文者情報更新
		/// </summary>
		/// <param name="order">注文情報</param>
		private void UpdateOrderOwner(OrderInput order)
		{
			var orderOwner = order.Owner;
			this.Owner.OrderId = this.OrderId;
			this.Owner.OwnerKbn = orderOwner.OwnerKbn;
			this.Owner.OwnerName = orderOwner.OwnerName1 + orderOwner.OwnerName2;
			this.Owner.OwnerName1 = orderOwner.OwnerName1;
			this.Owner.OwnerName2 = orderOwner.OwnerName2;
			this.Owner.OwnerNameKana = orderOwner.OwnerNameKana1 + orderOwner.OwnerNameKana2;
			this.Owner.OwnerNameKana1 = orderOwner.OwnerNameKana1;
			this.Owner.OwnerNameKana2 = orderOwner.OwnerNameKana2;
			this.Owner.OwnerMailAddr = orderOwner.OwnerMailAddr;
			this.Owner.OwnerMailAddr2 = orderOwner.OwnerMailAddr2;
			this.Owner.OwnerAddrCountryIsoCode = orderOwner.OwnerAddrCountryIsoCode;
			this.Owner.OwnerAddrCountryName = orderOwner.OwnerAddrCountryName;
			this.Owner.OwnerZip = orderOwner.IsAddrJp
				? string.Join("-", orderOwner.OwnerZip1, orderOwner.OwnerZip2)
				: orderOwner.OwnerZip;
			this.Owner.OwnerZip1 = orderOwner.OwnerZip1;
			this.Owner.OwnerZip2 = orderOwner.OwnerZip2;
			this.Owner.OwnerAddr1 = orderOwner.OwnerAddr1;
			this.Owner.OwnerAddr2 = orderOwner.OwnerAddr2;
			this.Owner.OwnerAddr3 = orderOwner.OwnerAddr3;
			this.Owner.OwnerAddr4 = orderOwner.OwnerAddr4;
			this.Owner.OwnerAddr5 = orderOwner.OwnerAddr5;
			this.Owner.OwnerTel1 = orderOwner.IsAddrJp
				? string.Join("-", orderOwner.OwnerTel1_1, orderOwner.OwnerTel1_2, orderOwner.OwnerTel1_3)
				: orderOwner.OwnerTel1;
			this.Owner.OwnerTel1_1 = orderOwner.OwnerTel1_1;
			this.Owner.OwnerTel1_2 = orderOwner.OwnerTel1_2;
			this.Owner.OwnerTel1_3 = orderOwner.OwnerTel1_3;
			this.Owner.OwnerSex = orderOwner.OwnerSex;
			this.Owner.OwnerBirth = orderOwner.OwnerBirth;
			this.Owner.OwnerCompanyName = orderOwner.OwnerCompanyName;
			this.Owner.OwnerCompanyPostName = orderOwner.OwnerCompanyPostName;
			// ユーザー
			this.Owner.UserMemo = orderOwner.UserMemo;
			this.Owner.UserManagementLevelId = orderOwner.UserManagementLevelId;
			this.Owner.AccessCountryIsoCode = orderOwner.AccessCountryIsoCode;
			this.Owner.DispLanguageCode = orderOwner.DispLanguageCode;
			this.Owner.DispLanguageLocaleId = orderOwner.DispLanguageLocaleId;
			this.Owner.DispCurrencyCode = orderOwner.DispCurrencyCode;
			this.Owner.DispCurrencyLocaleId = orderOwner.DispCurrencyLocaleId;
		}

		/// <summary>
		/// 注文配送先情報更新
		/// </summary>
		/// <param name="order">注文情報</param>
		public void UpdateOrderShippings(OrderInput order)
		{
			foreach (var orderShipping in order.Shippings)
			{
				if (orderShipping.ShippingReceivingStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
				{
					orderShipping.ShippingName = orderShipping.ShippingName1 + orderShipping.ShippingName2;
					orderShipping.ShippingNameKana = orderShipping.ShippingNameKana1 + orderShipping.ShippingNameKana2;
					orderShipping.SenderName = orderShipping.SenderName1 + orderShipping.SenderName2;
					orderShipping.SenderNameKana = orderShipping.SenderNameKana1 + orderShipping.SenderNameKana2;

					if (orderShipping.IsShippingAddrJp)
					{
						orderShipping.ShippingTel1 = string.Join("-", orderShipping.ShippingTel1_1, orderShipping.ShippingTel1_2, orderShipping.ShippingTel1_3);
						orderShipping.ShippingZip = string.Join("-", orderShipping.ShippingZip1, orderShipping.ShippingZip2);
						orderShipping.SenderTel1 = string.Join("-", orderShipping.SenderTel1_1, orderShipping.SenderTel1_2, orderShipping.SenderTel1_3);
						orderShipping.SenderZip = string.Join("-", orderShipping.SenderZip1, orderShipping.SenderZip2);
					}
				}

				// 別送フラグ更新
				orderShipping.AnotherShippingFlg =
					(orderShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						? Constants.FLG_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG
						: (string.IsNullOrEmpty(orderShipping.StorePickupRealShopId) == false)
							? Constants.FLG_ORDERSHIPPING_SHIPPING_STORE_PICKUP_FLG
							: this.IsAnotherShippingFlagValid(orderShipping)
								? Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID
								: Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID;

				// 注文商品更新
				UpdateOrderItems(orderShipping.Items, orderShipping);
			}
			this.Shippings = order.Shippings;

		}

		/// <summary>
		/// 注文商品情報更新
		/// </summary>
		/// <param name="orderItems">注文商品情報</param>
		/// <param name="orderShipping">注文配送先情報</param>
		private void UpdateOrderItems(OrderItemInput[] orderItems, OrderShippingInput orderShipping)
		{
			foreach (var orderItem in orderItems)
			{
				// 返品商品以外？
				if (orderItem.IsReturnItem == false)
				{
					// ※通常注文の場合は、返品交換区分を「指定無し」として登録し、交換注文の場合は、返品交換区分を「交換」として登録する
					orderItem.ItemReturnExchangeKbn =
						this.IsNotReturnExchangeOrder
						? Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN
						: Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE;
				}
			}
			orderShipping.Items = OrderInputHelper.SetCanDeletePropertyToOrderItemInput(orderItems);
		}

		/// <summary>
		/// 注文セットプロモーション情報更新
		/// </summary>
		/// <param name="order">注文情報</param>
		private void UpdateSetPromotion(OrderInput order)
		{
			this.SetPromotions = order.SetPromotions;
		}

		/// <summary>
		/// 注文クーポン情報更新
		/// </summary>
		/// <param name="order">注文情報</param>
		private void UpdateOrderCoupon(OrderInput order)
		{
			this.Coupons = new OrderCouponInput[0];
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				var orderCoupon = order.Coupon;
				var coupon = new OrderCouponInput
				{
					OrderId = this.OrderId,
					OrderCouponNo = orderCoupon.OrderCouponNo,
					DeptId = orderCoupon.DeptId,
					CouponId = orderCoupon.CouponId,
					CouponNo = orderCoupon.CouponNo,
					CouponCode = orderCoupon.CouponCode,
					CouponName = orderCoupon.CouponName,
					CouponDispName = orderCoupon.CouponDispName,
					CouponType = orderCoupon.CouponType,
					CouponDiscountPrice = orderCoupon.CouponDiscountPrice,
					CouponDiscountRate = orderCoupon.CouponDiscountRate,
					LastChanged = orderCoupon.LastChanged
				};
				this.Coupons = new[] { coupon };
			}
		}

		/// <summary>
		/// 金額再計算
		/// </summary>
		private void Recalculate()
		{
			// 注文商品情報に絡む項目
			{
				var orderItems = new List<OrderItemInput>();
				int orderProductCount = 0;
				decimal orderPriceSubTotal = 0;
				foreach (var orderItem in this.Shippings.SelectMany(s => s.Items))
				{
					// 削除対象の場合は次へ
					if (orderItem.DeleteTarget) continue;

					// 返品商品以外の場合
					// ※返品商品は注文アイテム数、注文商品数にカウントしない
					if (orderItem.IsReturnItem == false)
					{
						// 注文アイテム数…商品種類数合計（複数商品の個数含まない）
						if (orderItems.Any(i =>
							(i.ShopId == orderItem.ShopId)
							&& (i.ProductId == orderItem.ProductId)
							&& (i.VariationId == orderItem.VariationId)) == false)
						{
							orderItems.Add(orderItem);
						}

						// 注文商品数…商品数総合計（複数商品の個数含む）
						int itemQuantity = 0;
						if (int.TryParse(orderItem.ItemQuantity, out itemQuantity))
						{
							orderProductCount += itemQuantity;
						}
					}

					// 頒布会定額コース商品以外を加算
					if (orderItem.IsSubscriptionBoxFixedAmount == false)
					{
						orderPriceSubTotal += decimal.Parse(orderItem.ItemPrice);
					}
				}

				// 頒布会定額コース商品のみで計算
				var subscriptionBoxFixedAmounts = this.Shippings
					.SelectMany(shipping => shipping.Items)
					.Where(item => item.IsSubscriptionBoxFixedAmount)
					.GroupBy(item => item.SubscriptionBoxCourseId)
					.Sum(item => decimal.Parse(item.First().SubscriptionBoxFixedAmount));

				// 頒布会定額コース商品分を加算
				orderPriceSubTotal += subscriptionBoxFixedAmounts;

				// 注文商品関連項目
				this.OrderItemCount = orderItems.Count.ToString();
				this.OrderProductCount = orderProductCount.ToString();
				this.OrderPriceSubtotal = orderPriceSubTotal.ToString();
			}

			// 金額合計
			var orderPriceTotal = RecalculatePriceTotal();
			this.OrderPriceTotal = orderPriceTotal.ToString();
			// HACK:inputクラスで最終請求金額を計算しているのは微妙。
			var lastBuildAmount = (this.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)
				? orderPriceTotal
				: decimal.Parse(this.OldLastBilledAmount) + orderPriceTotal - decimal.Parse(this.OldOrderPriceTotal);
			this.LastBilledAmount = lastBuildAmount.ToString();
			// HACK:inputクラスで最終利用ポイントを計算しているのは微妙。
			var lastOrderPointUse = (this.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)
				? decimal.Parse(this.OrderPointUse)
				: decimal.Parse(this.OldLastOrderPointUse) + decimal.Parse(this.OrderPointUse) - decimal.Parse(this.OldOrderPointUse);
			this.LastOrderPointUse = lastOrderPointUse.ToString();
			this.LastOrderPointUseYen = PointOptionUtility.GetOrderPointUsePriceDecimal(Constants.CONST_DEFAULT_DEPT_ID, lastOrderPointUse).ToString();
		}

		/// <summary>
		/// 注文合計金額の再計算
		/// </summary>
		/// <returns>注文合計金額</returns>
		public decimal RecalculatePriceTotal()
		{
			// 金額合計
			var orderPriceTotal = 0m;

			// 注文セットプロモーション関連項目
			this.SetpromotionProductDiscountAmount =
				this.SetPromotions.Where(sp => sp.IsDiscountTypeProductDiscount).Sum(sp => decimal.Parse(sp.ProductDiscountAmount)).ToString();
			this.SetpromotionShippingChargeDiscountAmount =
				this.SetPromotions.Where(sp => sp.IsDiscountTypeShippingChargeFree).Sum(sp => decimal.Parse(sp.ShippingChargeDiscountAmount)).ToString();
			this.SetpromotionPaymentChargeDiscountAmount =
				this.SetPromotions.Where(sp => sp.IsDiscountTypePaymentChargeFree).Sum(sp => decimal.Parse(sp.PaymentChargeDiscountAmount)).ToString();

			// 金額合計
			var priceSubtotal = CalculateItemSubtotalExcludeDeleteTarget();
			var priceSubtotalTax = CalculateItemSubtotalTaxExcludeDeleteTarget();

			orderPriceTotal = TaxCalculationUtility.GetPriceTaxIncluded(priceSubtotal, priceSubtotalTax)
				+ decimal.Parse(this.OrderPriceShipping)
				+ decimal.Parse(this.OrderPriceExchange)
				+ decimal.Parse(this.OrderPriceRegulation)
				+ this.OrderPriceByTaxRates
					.Sum(orderPriceByTaxRate => decimal.Parse(orderPriceByTaxRate.PriceCorrectionByRate))
				- decimal.Parse(this.MemberRankDiscountPrice)
				- decimal.Parse(this.OrderCouponUse)
				- decimal.Parse(this.OrderPointUseYen)
				- decimal.Parse(this.SetpromotionProductDiscountAmount)
				- decimal.Parse(this.SetpromotionShippingChargeDiscountAmount)
				- decimal.Parse(this.SetpromotionPaymentChargeDiscountAmount)
				- decimal.Parse(this.FixedPurchaseMemberDiscountAmount)
				- decimal.Parse(this.FixedPurchaseDiscountPrice);
			return orderPriceTotal;
		}

		/// <summary>
		/// Get purchase price total
		/// </summary>
		/// <returns>Purchase price total</returns>
		public decimal GetPurchasePriceTotal()
		{
			var orderPriceSubtotal = decimal.Parse(this.OrderPriceSubtotal);
			var memberRankDiscountPrice = decimal.Parse(this.MemberRankDiscountPrice);
			var fixedPurchaseDiscountPrice = decimal.Parse(this.FixedPurchaseDiscountPrice);
			var fixedPurchaseMemberDiscountAmount = decimal.Parse(this.FixedPurchaseMemberDiscountAmount);
			var setpromotionProductDiscountAmount = decimal.Parse(this.SetpromotionProductDiscountAmount);
			var couponDiscountPrice = ((this.Coupon != null)
					&& (string.IsNullOrEmpty(this.Coupon.CouponDiscountPrice) == false))
				? decimal.Parse(this.Coupon.CouponDiscountPrice)
				: 0m;

			var totalDiscount = memberRankDiscountPrice
				+ couponDiscountPrice
				+ fixedPurchaseDiscountPrice
				+ fixedPurchaseMemberDiscountAmount
				+ setpromotionProductDiscountAmount;

			var result = (orderPriceSubtotal - totalDiscount);
			return result;
		}

		/// <summary>
		/// 商品金額合計を計算する（削除対象を除く）
		/// </summary>
		/// <returns>商品金額合計</returns>
		public decimal CalculateItemSubtotalExcludeDeleteTarget()
		{
			var orderItems = this.Shippings.SelectMany(shipping => shipping.Items).ToArray();

			// 頒布会定額コース商品を除いた分
			var fixedAmountExcluded = orderItems
				.Where(item => (item.DeleteTarget == false) && (item.IsSubscriptionBoxFixedAmount == false))
				.Sum(item => decimal.Parse(item.ItemPrice));

			// 頒布会定額コース商品のみ
			var fixedAmountSubtotal = orderItems
				.Where(item => (item.DeleteTarget == false) && item.IsSubscriptionBoxFixedAmount)
				.GroupBy(item => item.SubscriptionBoxCourseId)
				.Sum(item => decimal.Parse(item.First().SubscriptionBoxFixedAmount));

			var total = fixedAmountExcluded + fixedAmountSubtotal;
			return total;
		}

		/// <summary>
		/// 商品金額合計税額を計算する（削除対象を除く）
		/// </summary>
		/// <returns>商品金額合計税額</returns>
		public decimal CalculateItemSubtotalTaxExcludeDeleteTarget()
		{
			var orderItems = this.Shippings.SelectMany(shipping => shipping.Items).ToArray();

			// 頒布会定額コース商品を除いた分
			var subtotalTax = orderItems
				.Where(item => (item.DeleteTarget == false) && (item.IsSubscriptionBoxFixedAmount == false))
				.Sum(item => decimal.Parse(item.ItemPriceTax));

			// 頒布会定額コース商品のみ
			if (this.HasSubscriptionBoxFixedAmountItem)
			{
				var itemsGroupByfixedAmountCourse = orderItems
					.Where(item => (item.DeleteTarget == false) && item.IsSubscriptionBoxFixedAmount)
					.GroupBy(item => item.SubscriptionBoxCourseId);
				foreach (var fixedAmountCourseGroup in itemsGroupByfixedAmountCourse)
				{
					var subscriptionBox = DataCacheControllerFacade.GetSubscriptionBoxCacheController()
						.Get(fixedAmountCourseGroup.Key);
					var taxRate = new ProductTaxCategoryService().Get(subscriptionBox.TaxCategoryId).TaxRate;
					var fixedAmount =
						decimal.TryParse(fixedAmountCourseGroup.First().SubscriptionBoxFixedAmount, out var parsedFixedAmount)
							? parsedFixedAmount
							: 0;
					var taxPrice = TaxCalculationUtility.GetTaxPrice(
						fixedAmount,
						taxRate,
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
					subtotalTax += taxPrice;
				}
			}

			return subtotalTax;
		}

		/// <summary>
		/// 1種類の頒布会定額コースの商品のみが含まれるか
		/// </summary>
		/// <remarks>通常の定額頒布会 or 同コース同梱の定額頒布会であるかを確認</remarks>
		/// <returns>1種類であればTRUE</returns>
		public bool HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()
		{
			// 注文同梱でない場合、カートが頒布会定額コースならTRUE
			if ((this.IsOrderCombined == false) && this.IsSubscriptionBoxFixedAmount) return true;

			// 注文同梱であれば、定額頒布会以外が含まれていないかつ同コース頒布会での同梱ならTRUE
			var fixedAmountCourseCount = this.Shippings
				.SelectMany(shipping => shipping.Items)
				.Where(item => item.IsSubscriptionBoxFixedAmount)
				.GroupBy(item => item.SubscriptionBoxCourseId)
				.Count();
			return (fixedAmountCourseCount == 1) && this.IsAllItemsSubscriptionBoxFixedAmount;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_ID] = value; }
		}
		/// <summary>元注文ID</summary>
		public string OrderIdOrg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_ID_ORG]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_ID_ORG] = value; }
		}
		/// <summary>注文グループID</summary>
		public string OrderGroupId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_GROUP_ID] = value; }
		}
		/// <summary>注文番号</summary>
		public string OrderNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_NO]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_NO] = value; }
		}
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_USER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_USER_ID] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHOP_ID] = value; }
		}
		/// <summary>サプライヤID</summary>
		public string SupplierId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SUPPLIER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_SUPPLIER_ID] = value; }
		}
		/// <summary>注文区分</summary>
		public string OrderKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_KBN] = value; }
		}
		/// <summary>モールID</summary>
		public string MallId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MALL_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_MALL_ID] = value; }
		}
		/// <summary>モール名</summary>
		public string MallName
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME] = value; }
		}
		/// <summary>支払区分</summary>
		public string OrderPaymentKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] = value; }
		}
		/// <summary>決済種別名</summary>
		public string PaymentName
		{
			get { return (string)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME]; }
			set { this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME] = value; }
		}
		/// <summary>注文ステータス</summary>
		public string OrderStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_STATUS] = value; }
		}
		/// <summary>注文日時</summary>
		public string OrderDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_DATE] = value; }
		}
		/// <summary>受注承認日時</summary>
		public string OrderRecognitionDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE] = value; }
		}
		/// <summary>在庫引当ステータス</summary>
		public string OrderStockreservedStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS] = value; }
		}
		/// <summary>在庫引当日時</summary>
		public string OrderStockreservedDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE] = value; }
		}
		/// <summary>出荷手配日時</summary>
		public string OrderShippingDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE] = value; }
		}
		/// <summary>出荷ステータス</summary>
		public string OrderShippedStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS] = value; }
		}
		/// <summary>出荷完了日時</summary>
		public string OrderShippedDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE] = value; }
		}
		/// <summary>配送完了日時</summary>
		public string OrderDeliveringDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE] = value; }
		}
		/// <summary>キャンセル日時</summary>
		public string OrderCancelDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_CANCEL_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_CANCEL_DATE] = value; }
		}
		/// <summary>返品日時</summary>
		public string OrderReturnDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_DATE] = value; }
		}
		/// <summary>入金ステータス</summary>
		public string OrderPaymentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = value; }
		}
		/// <summary>入金確認日時</summary>
		public string OrderPaymentDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE] = value; }
		}
		/// <summary>督促ステータス</summary>
		public string DemandStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DEMAND_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_DEMAND_STATUS] = value; }
		}
		/// <summary>督促日時</summary>
		public string DemandDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DEMAND_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_DEMAND_DATE] = value; }
		}
		/// <summary>返品交換ステータス</summary>
		public string OrderReturnExchangeStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS] = value; }
		}
		/// <summary>返品交換受付日時</summary>
		public string OrderReturnExchangeReceiptDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE] = value; }
		}
		/// <summary>返品交換商品到着日時</summary>
		public string OrderReturnExchangeArrivalDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE] = value; }
		}
		/// <summary>返品交換完了日時</summary>
		public string OrderReturnExchangeCompleteDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE] = value; }
		}
		/// <summary>返金ステータス</summary>
		public string OrderRepaymentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS] = value; }
		}
		/// <summary>返金日時</summary>
		public string OrderRepaymentDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE] = value; }
		}
		/// <summary>注文アイテム数</summary>
		public string OrderItemCount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_ITEM_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_ITEM_COUNT] = value; }
		}
		/// <summary>注文商品数</summary>
		public string OrderProductCount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT] = value; }
		}
		/// <summary>小計</summary>
		public string OrderPriceSubtotal
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL] = value; }
		}
		/// <summary>荷造金額</summary>
		public string OrderPricePack
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_PACK]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_PACK] = value; }
		}
		/// <summary>税金合計</summary>
		public string OrderPriceTax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TAX] = value; }
		}
		/// <summary>配送料</summary>
		public string OrderPriceShipping
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING] = value; }
		}
		/// <summary>代引手数料</summary>
		public string OrderPriceExchange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE] = value; }
		}
		/// <summary>調整金額</summary>
		public string OrderPriceRegulation
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_REGULATION]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_REGULATION] = value; }
		}
		/// <summary>返金金額</summary>
		public string OrderPriceRepayment
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT] = value; }
		}
		/// <summary>支払金額合計</summary>
		public string OrderPriceTotal
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL] = value; }
		}
		/// <summary>変更前支払金額合計</summary>
		public string OldOrderPriceTotal
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL + "_old"]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL + "_old"] = value; }
		}
		/// <summary>セット値引金額</summary>
		public string OrderDiscountSetPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_DISCOUNT_SET_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_DISCOUNT_SET_PRICE] = value; }
		}
		/// <summary>利用ポイント数</summary>
		public string OrderPointUse
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE] = value; }
		}
		/// <summary>変更前の利用ポイント数</summary>
		public string OldOrderPointUse
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE + "_old"]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE + "_old"] = value; }
		}
		/// <summary>ポイント利用額</summary>
		public string OrderPointUseYen
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE_YEN]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE_YEN] = value; }
		}
		/// <summary>付与ポイント</summary>
		public string OrderPointAdd
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_ADD]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_ADD] = value; }
		}
		/// <summary>ポイント調整率</summary>
		public string OrderPointRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_RATE] = value; }
		}
		/// <summary>クーポン割引額</summary>
		public string OrderCouponUse
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_COUPON_USE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_COUPON_USE] = value; }
		}
		/// <summary>決済カード区分</summary>
		public string CardKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_KBN] = value; }
		}
		/// <summary>決済カード支払回数文言</summary>
		public string CardInstruments
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_INSTRUMENTS]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_INSTRUMENTS] = value; }
		}
		/// <summary>決済カード取引ID</summary>
		public string CardTranId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_TRAN_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_TRAN_ID] = value; }
		}
		/// <summary>配送種別ID</summary>
		public string ShippingId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHIPPING_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPING_ID] = value; }
		}
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_ID] = value; }
		}
		/// <summary>初回広告コード</summary>
		public string AdvcodeFirst
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ADVCODE_FIRST]; }
			set { this.DataSource[Constants.FIELD_ORDER_ADVCODE_FIRST] = value; }
		}
		/// <summary>最新広告コード</summary>
		public string AdvcodeNew
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ADVCODE_NEW]; }
			set { this.DataSource[Constants.FIELD_ORDER_ADVCODE_NEW] = value; }
		}
		/// <summary>出荷後変更区分</summary>
		public string ShippedChangedKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN] = value; }
		}
		/// <summary>返品交換区分</summary>
		public string ReturnExchangeKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN] = value; }
		}
		/// <summary>返品交換都合区分</summary>
		public string ReturnExchangeReasonKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN] = value; }
		}
		/// <summary>属性1</summary>
		public string Attribute1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE1]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE1] = value; }
		}
		/// <summary>属性2</summary>
		public string Attribute2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE2]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE2] = value; }
		}
		/// <summary>属性3</summary>
		public string Attribute3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE3]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE3] = value; }
		}
		/// <summary>属性4</summary>
		public string Attribute4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE4]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE4] = value; }
		}
		/// <summary>属性5</summary>
		public string Attribute5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE5]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE5] = value; }
		}
		/// <summary>属性6</summary>
		public string Attribute6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE6]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE6] = value; }
		}
		/// <summary>属性7</summary>
		public string Attribute7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE7]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE7] = value; }
		}
		/// <summary>属性8</summary>
		public string Attribute8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE8]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE8] = value; }
		}
		/// <summary>属性9</summary>
		public string Attribute9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE9]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE9] = value; }
		}
		/// <summary>属性10</summary>
		public string Attribute10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE10]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE10] = value; }
		}
		/// <summary>拡張ステータス１</summary>
		public string ExtendStatus1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS1]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS1] = value; }
		}
		/// <summary>拡張ステータス更新日時１</summary>
		public string ExtendStatusDate1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE1]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE1] = value; }
		}
		/// <summary>拡張ステータス２</summary>
		public string ExtendStatus2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS2]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS2] = value; }
		}
		/// <summary>拡張ステータス更新日時２</summary>
		public string ExtendStatusDate2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE2]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE2] = value; }
		}
		/// <summary>拡張ステータス３</summary>
		public string ExtendStatus3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS3]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS3] = value; }
		}
		/// <summary>拡張ステータス更新日時３</summary>
		public string ExtendStatusDate3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE3]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE3] = value; }
		}
		/// <summary>拡張ステータス４</summary>
		public string ExtendStatus4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS4]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS4] = value; }
		}
		/// <summary>拡張ステータス更新日時４</summary>
		public string ExtendStatusDate4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE4]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE4] = value; }
		}
		/// <summary>拡張ステータス５</summary>
		public string ExtendStatus5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS5]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS5] = value; }
		}
		/// <summary>拡張ステータス更新日時５</summary>
		public string ExtendStatusDate5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE5]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE5] = value; }
		}
		/// <summary>拡張ステータス６</summary>
		public string ExtendStatus6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS6]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS6] = value; }
		}
		/// <summary>拡張ステータス更新日時６</summary>
		public string ExtendStatusDate6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE6]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE6] = value; }
		}
		/// <summary>拡張ステータス７</summary>
		public string ExtendStatus7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS7]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS7] = value; }
		}
		/// <summary>拡張ステータス更新日時７</summary>
		public string ExtendStatusDate7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE7]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE7] = value; }
		}
		/// <summary>拡張ステータス８</summary>
		public string ExtendStatus8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS8]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS8] = value; }
		}
		/// <summary>拡張ステータス更新日時８</summary>
		public string ExtendStatusDate8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE8]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE8] = value; }
		}
		/// <summary>拡張ステータス９</summary>
		public string ExtendStatus9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS9]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS9] = value; }
		}
		/// <summary>拡張ステータス更新日時９</summary>
		public string ExtendStatusDate9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE9]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE9] = value; }
		}
		/// <summary>拡張ステータス１０</summary>
		public string ExtendStatus10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS10]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS10] = value; }
		}
		/// <summary>拡張ステータス更新日時１０</summary>
		public string ExtendStatusDate10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE10]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE10] = value; }
		}
		/// <summary>キャリアID</summary>
		public string CareerId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CAREER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_CAREER_ID] = value; }
		}
		/// <summary>モバイルUID</summary>
		public string MobileUid
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MOBILE_UID]; }
			set { this.DataSource[Constants.FIELD_ORDER_MOBILE_UID] = value; }
		}
		/// <summary>リモートIPアドレス</summary>
		public string RemoteAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_REMOTE_ADDR]; }
			set { this.DataSource[Constants.FIELD_ORDER_REMOTE_ADDR] = value; }
		}
		/// <summary>メモ</summary>
		public string Memo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_MEMO] = value; }
		}
		/// <summary>熨斗メモ</summary>
		public string WrappingMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_WRAPPING_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_WRAPPING_MEMO] = value; }
		}
		/// <summary>決済連携メモ</summary>
		public string PaymentMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_PAYMENT_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_PAYMENT_MEMO] = value; }
		}
		/// <summary>管理メモ</summary>
		public string ManagementMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MANAGEMENT_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_MANAGEMENT_MEMO] = value; }
		}
		/// <summary>配送メモ</summary>
		public string ShippingMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHIPPING_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPING_MEMO] = value; }
		}
		/// <summary>外部連携メモ</summary>
		public string RelationMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RELATION_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_RELATION_MEMO] = value; }
		}
		/// <summary>返品交換理由メモ</summary>
		public string ReturnExchangeReasonMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_MEMO] = value; }
		}
		/// <summary>調整金額メモ</summary>
		public string RegulationMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_REGULATION_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_REGULATION_MEMO] = value; }
		}
		/// <summary>返金メモ</summary>
		public string RepaymentMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_REPAYMENT_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_REPAYMENT_MEMO] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDER_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDER_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_CHANGED] = value; }
		}
		/// <summary>会員ランク割引額</summary>
		public string MemberRankDiscountPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE] = value; }
		}
		/// <summary>注文時会員ランク</summary>
		public string MemberRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MEMBER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_MEMBER_RANK_ID] = value; }
		}
		/// <summary>会員ランク名</summary>
		public string MemberRankName
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME] = value; }
		}
		/// <summary>クレジットカード枝番</summary>
		public string CreditBranchNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = value; }
		}
		/// <summary>アフィリエイトセッション変数名1</summary>
		public string AffiliateSessionName1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME1]; }
			set { this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME1] = value; }
		}
		/// <summary>アフィリエイトセッション値1</summary>
		public string AffiliateSessionValue1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE1]; }
			set { this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE1] = value; }
		}
		/// <summary>アフィリエイトセッション変数名2</summary>
		public string AffiliateSessionName2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME2]; }
			set { this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME2] = value; }
		}
		/// <summary>アフィリエイトセッション値2</summary>
		public string AffiliateSessionValue2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE2]; }
			set { this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE2] = value; }
		}
		/// <summary>ユーザーエージェント</summary>
		public string UserAgent
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_USER_AGENT]; }
			set { this.DataSource[Constants.FIELD_ORDER_USER_AGENT] = value; }
		}
		/// <summary>ギフト購入フラグ</summary>
		public string GiftFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_GIFT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_GIFT_FLG] = value; }
		}
		/// <summary>デジタルコンテンツ購入フラグ</summary>
		public string DigitalContentsFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG] = value; }
		}
		/// <summary>3DセキュアトランザクションID</summary>
		public string Card_3dsecureTranId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_TRAN_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_TRAN_ID] = value; }
		}
		/// <summary>3Dセキュア認証URL</summary>
		public string Card_3dsecureAuthUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL] = value; }
		}
		/// <summary>3Dセキュア認証キー</summary>
		public string Card_3dsecureAuthKey
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_KEY]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_KEY] = value; }
		}
		/// <summary>配送料の別見積もりの利用フラグ</summary>
		public string ShippingPriceSeparateEstimatesFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] = value; }
		}
		/// <summary>税込フラグ</summary>
		public string OrderTaxIncludedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_INCLUDED_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_INCLUDED_FLG] = value; }
		}
		/// <summary>税率</summary>
		public string OrderTaxRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_RATE] = value; }
		}
		/// <summary>税計算方法</summary>
		public string OrderTaxRoundType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_ROUND_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_ROUND_TYPE] = value; }
		}
		/// <summary>拡張ステータス11</summary>
		public string ExtendStatus11
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS11]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS11] = value; }
		}
		/// <summary>拡張ステータス更新日時11</summary>
		public string ExtendStatusDate11
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE11]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE11] = value; }
		}
		/// <summary>拡張ステータス12</summary>
		public string ExtendStatus12
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS12]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS12] = value; }
		}
		/// <summary>拡張ステータス更新日時12</summary>
		public string ExtendStatusDate12
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE12]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE12] = value; }
		}
		/// <summary>拡張ステータス13</summary>
		public string ExtendStatus13
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS13]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS13] = value; }
		}
		/// <summary>拡張ステータス更新日時13</summary>
		public string ExtendStatusDate13
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE13]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE13] = value; }
		}
		/// <summary>拡張ステータス14</summary>
		public string ExtendStatus14
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS14]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS14] = value; }
		}
		/// <summary>拡張ステータス更新日時14</summary>
		public string ExtendStatusDate14
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE14]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE14] = value; }
		}
		/// <summary>拡張ステータス15</summary>
		public string ExtendStatus15
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS15]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS15] = value; }
		}
		/// <summary>拡張ステータス更新日時15</summary>
		public string ExtendStatusDate15
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE15]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE15] = value; }
		}
		/// <summary>拡張ステータス16</summary>
		public string ExtendStatus16
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS16]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS16] = value; }
		}
		/// <summary>拡張ステータス更新日時16</summary>
		public string ExtendStatusDate16
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE16]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE16] = value; }
		}
		/// <summary>拡張ステータス17</summary>
		public string ExtendStatus17
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS17]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS17] = value; }
		}
		/// <summary>拡張ステータス更新日時17</summary>
		public string ExtendStatusDate17
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE17]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE17] = value; }
		}
		/// <summary>拡張ステータス18</summary>
		public string ExtendStatus18
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS18]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS18] = value; }
		}
		/// <summary>拡張ステータス更新日時18</summary>
		public string ExtendStatusDate18
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE18]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE18] = value; }
		}
		/// <summary>拡張ステータス19</summary>
		public string ExtendStatus19
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS19]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS19] = value; }
		}
		/// <summary>拡張ステータス更新日時19</summary>
		public string ExtendStatusDate19
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE19]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE19] = value; }
		}
		/// <summary>拡張ステータス20</summary>
		public string ExtendStatus20
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS20]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS20] = value; }
		}
		/// <summary>拡張ステータス更新日時20</summary>
		public string ExtendStatusDate20
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE20]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE20] = value; }
		}
		/// <summary>拡張ステータス21</summary>
		public string ExtendStatus21
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS21]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS21] = value; }
		}
		/// <summary>拡張ステータス更新日時21</summary>
		public string ExtendStatusDate21
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE21]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE21] = value; }
		}
		/// <summary>拡張ステータス22</summary>
		public string ExtendStatus22
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS22]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS22] = value; }
		}
		/// <summary>拡張ステータス更新日時22</summary>
		public string ExtendStatusDate22
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE22]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE22] = value; }
		}
		/// <summary>拡張ステータス23</summary>
		public string ExtendStatus23
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS23]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS23] = value; }
		}
		/// <summary>拡張ステータス更新日時23</summary>
		public string ExtendStatusDate23
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE23]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE23] = value; }
		}
		/// <summary>拡張ステータス24</summary>
		public string ExtendStatus24
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS24]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS24] = value; }
		}
		/// <summary>拡張ステータス更新日時24</summary>
		public string ExtendStatusDate24
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE24]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE24] = value; }
		}
		/// <summary>拡張ステータス25</summary>
		public string ExtendStatus25
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS25]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS25] = value; }
		}
		/// <summary>拡張ステータス更新日時25</summary>
		public string ExtendStatusDate25
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE25]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE25] = value; }
		}
		/// <summary>拡張ステータス26</summary>
		public string ExtendStatus26
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS26]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS26] = value; }
		}
		/// <summary>拡張ステータス更新日時26</summary>
		public string ExtendStatusDate26
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE26]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE26] = value; }
		}
		/// <summary>拡張ステータス27</summary>
		public string ExtendStatus27
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS27]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS27] = value; }
		}
		/// <summary>拡張ステータス更新日時27</summary>
		public string ExtendStatusDate27
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE27]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE27] = value; }
		}
		/// <summary>拡張ステータス28</summary>
		public string ExtendStatus28
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS28]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS28] = value; }
		}
		/// <summary>拡張ステータス更新日時28</summary>
		public string ExtendStatusDate28
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE28]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE28] = value; }
		}
		/// <summary>拡張ステータス29</summary>
		public string ExtendStatus29
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS29]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS29] = value; }
		}
		/// <summary>拡張ステータス更新日時29</summary>
		public string ExtendStatusDate29
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE29]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE29] = value; }
		}
		/// <summary>拡張ステータス30</summary>
		public string ExtendStatus30
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS30]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS30] = value; }
		}
		/// <summary>拡張ステータス更新日時30</summary>
		public string ExtendStatusDate30
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE30]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE30] = value; }
		}
		/// <summary>拡張ステータス31</summary>
		public string ExtendStatus31
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS31]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS31] = value; }
		}
		/// <summary>拡張ステータス更新日時31</summary>
		public string ExtendStatusDate31
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE31]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE31] = value; }
		}
		/// <summary>拡張ステータス32</summary>
		public string ExtendStatus32
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS32]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS32] = value; }
		}
		/// <summary>拡張ステータス更新日時32</summary>
		public string ExtendStatusDate32
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE32]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE32] = value; }
		}
		/// <summary>拡張ステータス33</summary>
		public string ExtendStatus33
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS33]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS33] = value; }
		}
		/// <summary>拡張ステータス更新日時33</summary>
		public string ExtendStatusDate33
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE33]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE33] = value; }
		}
		/// <summary>拡張ステータス34</summary>
		public string ExtendStatus34
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS34]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS34] = value; }
		}
		/// <summary>拡張ステータス更新日時34</summary>
		public string ExtendStatusDate34
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE34]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE34] = value; }
		}
		/// <summary>拡張ステータス35</summary>
		public string ExtendStatus35
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS35]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS35] = value; }
		}
		/// <summary>拡張ステータス更新日時35</summary>
		public string ExtendStatusDate35
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE35]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE35] = value; }
		}
		/// <summary>拡張ステータス36</summary>
		public string ExtendStatus36
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS36]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS36] = value; }
		}
		/// <summary>拡張ステータス更新日時36</summary>
		public string ExtendStatusDate36
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE36]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE36] = value; }
		}
		/// <summary>拡張ステータス37</summary>
		public string ExtendStatus37
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS37]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS37] = value; }
		}
		/// <summary>拡張ステータス更新日時37</summary>
		public string ExtendStatusDate37
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE37]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE37] = value; }
		}
		/// <summary>拡張ステータス38</summary>
		public string ExtendStatus38
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS38]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS38] = value; }
		}
		/// <summary>拡張ステータス更新日時38</summary>
		public string ExtendStatusDate38
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE38]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE38] = value; }
		}
		/// <summary>拡張ステータス39</summary>
		public string ExtendStatus39
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS39]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS39] = value; }
		}
		/// <summary>拡張ステータス更新日時39</summary>
		public string ExtendStatusDate39
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE39]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE39] = value; }
		}
		/// <summary>拡張ステータス40</summary>
		public string ExtendStatus40
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS40]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS40] = value; }
		}
		/// <summary>拡張ステータス更新日時40</summary>
		public string ExtendStatusDate40
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE40]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE40] = value; }
		}
		/// <summary>カード支払い回数コード</summary>
		public string CardInstallmentsCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE] = value; }
		}
		/// <summary>セットプロモーション商品割引額</summary>
		public string SetpromotionProductDiscountAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>セットプロモーション配送料割引額</summary>
		public string SetpromotionShippingChargeDiscountAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>セットプロモーション決済手数料割引額</summary>
		public string SetpromotionPaymentChargeDiscountAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>オンライン決済ステータス</summary>
		public string OnlinePaymentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS] = value; }
		}
		/// <summary>定期購入回数(注文時点)</summary>
		public string FixedPurchaseOrderCount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT] = value; }
		}
		/// <summary>定期購入回数(出荷時点)</summary>
		public string FixedPurchaseShippedCount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT] = value; }
		}
		/// <summary>定期購入割引額</summary>
		public string FixedPurchaseDiscountPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE] = value; }
		}
		/// <summary>決済注文ID</summary>
		public string PaymentOrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = value; }
		}
		/// <summary>定期会員割引額</summary>
		public string FixedPurchaseMemberDiscountAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>外部決済ステータス</summary>
		public string ExternalPaymentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = value; }
		}
		/// <summary>外部決済エラーメッセージ</summary>
		public string ExternalPaymentErrorMessage
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_ERROR_MESSAGE]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_ERROR_MESSAGE] = value; }
		}
		/// <summary>外部決済与信日時</summary>
		public string ExternalPaymentAuthDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE] = value; }
		}
		/// <summary>最終請求金額</summary>
		public string LastBilledAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT] = value; }
		}
		/// <summary>変更前の最終請求金額</summary>
		public string OldLastBilledAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT + "_old"]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT + "_old"] = value; }
		}
		/// <summary>最終利用ポイント数</summary>
		public string LastOrderPointUse
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE] = value; }
		}
		/// <summary>変更前の最終利用ポイント数</summary>
		public string OldLastOrderPointUse
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE + "_old"]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE + "_old"] = value; }
		}
		/// <summary>最終ポイント利用額</summary>
		public string LastOrderPointUseYen
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN] = value; }
		}
		/// <summary>外部連携受注ID</summary>
		public string ExternalOrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_ORDER_ID] = value; }
		}
		/// <summary>外部連携取込ステータス</summary>
		public string ExternalImportStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS] = value; }
		}
		/// <summary>最終与信フラグ</summary>
		public string LastAuthFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LAST_AUTH_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_AUTH_FLG] = value; }
		}
		/// <summary>決済通貨</summary>
		public string SettlementCurrency
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY] = value; }
		}
		/// <summary>決済レート</summary>
		public string SettlementRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_RATE] = value; }
		}
		/// <summary>決済金額</summary>
		public string SettlementAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT] = value; }
		}
		/// <summary>注文者情報</summary>
		public OrderOwnerInput Owner
		{
			get { return (OrderOwnerInput)this.DataSource["Owner"]; }
			set { this.DataSource["Owner"] = value; }
		}
		/// <summary>配送先リスト</summary>
		public OrderShippingInput[] Shippings
		{
			get { return (OrderShippingInput[])this.DataSource["Shippings"]; }
			set { this.DataSource["Shippings"] = value; }
		}
		/// <summary>税率毎金額情報リスト</summary>
		public OrderPriceByTaxRateInput[] OrderPriceByTaxRates
		{
			get { return (OrderPriceByTaxRateInput[])this.DataSource["OrderPriceByTaxRates"]; }
			set { this.DataSource["OrderPriceByTaxRates"] = value; }
		}
		/// <summary>デジタルコンテンツ？</summary>
		public bool IsDigitalContents
		{
			get { return (this.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON); }
		}
		/// <summary>クーポン</summary>
		public OrderCouponInput Coupon
		{
			get
			{
				if (this.Coupons.Length == 0)
				{
					return null;
				}
				return this.Coupons[0];
			}
		}
		/// <summary>クーポンリスト</summary>
		public OrderCouponInput[] Coupons
		{
			get { return (OrderCouponInput[])this.DataSource["Coupons"]; }
			set { this.DataSource["Coupons"] = value; }
		}
		/// <summary>セットプロモーションリスト</summary>
		public OrderSetPromotionInput[] SetPromotions
		{
			get { return (OrderSetPromotionInput[])this.DataSource["SetPromotions"]; }
			set { this.DataSource["SetPromotions"] = value; }
		}
		/// <summary>
		/// 拡張ステータス
		/// </summary>
		public ExtendStatusData ExtendStatus
		{
			get
			{
				// 既にデータが存在する場合、返却
				if (m_extendStatus != null) return m_extendStatus;
				m_extendStatus = new ExtendStatusData(this.DataSource);
				return m_extendStatus;
			}
		}
		private ExtendStatusData m_extendStatus = null;
		/// <summary>配送料の別見積もりが有効？</summary>
		public bool IsValidShippingPriceSeparateEstimatesFlg
		{
			get
			{
				return Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED
					& this.ShippingPriceSeparateEstimatesFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID;
			}
		}
		/// <summary>通常注文（返品交換注文ではない）か</summary>
		public bool IsNotReturnExchangeOrder
		{
			get
			{
				return (this.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN);
			}
		}
		/// <summary>返品注文（通常・交換注文ではない）か</summary>
		public bool IsReturnOrder
		{
			get
			{
				return (this.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN);
			}
		}
		/// <summary>交換注文（通常・返品注文ではない）か</summary>
		public bool IsExchangeOrder
		{
			get
			{
				return (this.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE);
			}
		}
		/// <summary>セット商品が存在するか</summary>
		public bool HasSetProduct
		{
			get
			{
				return (this.Shippings.SelectMany(s => s.Items).Any(i => i.IsProductSet));
			}
		}
		/// <summary>注文同梱元注文ID</summary>
		public string CombinedOrgOrderIds
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS]; }
			set { this.DataSource[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS] = value; }
		}
		/// <summary>定期購入注文か</summary>
		public bool IsFixedPurchaseOrder
		{
			get { return string.IsNullOrEmpty(this.FixedPurchaseId) == false; }
		}
		/// <summary>キャンセル注文か</summary>
		public bool IsCancelOrder
		{
			get
			{
				return ((this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
					|| (this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED));
			}
		}
		/// <summary>ギフト注文か</summary>
		public bool IsGiftOrder
		{
			get
			{
				return (this.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON);
			}
		}
		/// <summary>実在庫引当済み商品が存在するか</summary>
		public bool HasRealStockReservedProduct
		{
			get
			{
				return (this.Shippings.SelectMany(s => s.Items).Any(i => i.IsRealStockReserved));
			}
		}
		/// <summary> 再与信可能の注文サイト区分であるかどうか </summary>
		public bool IsPermitReauthOrderSiteKbn
		{
			get { return (Constants.PAYMENT_REAUTH_ORDER_SITE_KBN.Contains(this.MallId)); }
		}
		/// <summary>最終与信フラグがONかどうか</summary>
		public bool IsLastAuthFlgON
		{
			get
			{
				return (this.LastAuthFlg == Constants.FLG_ORDER_LAST_AUTH_FLG_ON);
			}
		}
		/// <summary>モール区分</summary>
		public string MallKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN] = value; }
		}
		/// <summary>モール連携ステータス</summary>
		public string MallLinkStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MALL_LINK_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_MALL_LINK_STATUS] = value; }
		}
		/// <summary>商品小計税額</summary>
		public string OrderPriceSubtotalTax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX] = value; }
		}
		/// <summary>決済手数料税額</summary>
		/// 使用しない
		public string OrderPriceExchangeTax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX] = value; }
		}
		/// <summary>配送料税額</summary>
		/// 使用しない
		public string OrderPriceShippingTax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX] = value; }
		}
		/// <summary>外部連携決済エラーログ</summary>
		public string ExternalPaymentCooperationLog
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG] = value; }
		}
		/// <summary>配送料税率</summary>
		public string ShippingTaxRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHIPPING_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPING_TAX_RATE] = value; }
		}
		/// <summary>決済手数料税率</summary>
		public string PaymentTaxRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_PAYMENT_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_PAYMENT_TAX_RATE] = value; }
		}
		/// <summary>購入回数</summary>
		public string OrderCountOrder
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_COUNT_ORDER]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_COUNT_ORDER] = value; }
		}
		/// <summary>請求書同梱フラグ</summary>
		public string InvoiceBundleFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG] = value; }
		}
		/// <summary>流入コンテンツID</summary>
		public string InflowContentsId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_INFLOW_CONTENTS_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_INFLOW_CONTENTS_ID] = value; }
		}
		/// <summary>流入コンテンツタイプ</summary>
		public string InflowContentsType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE] = value; }
		}
		/// <summary>領収書希望フラグ</summary>
		public string ReceiptFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_FLG] = value; }
		}
		/// <summary>領収書出力フラグ</summary>
		public string ReceiptOutputFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG] = value; }
		}
		/// <summary>宛名</summary>
		public string ReceiptAddress
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_ADDRESS]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_ADDRESS] = value; }
		}
		/// <summary>但し書き</summary>
		public string ReceiptProviso
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_PROVISO]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_PROVISO] = value; }
		}
		/// <summary>Taiwan Order Invoice Model</summary>
		public TwOrderInvoiceModel TwOrderInvoiceModel
		{
			get { return (TwOrderInvoiceModel)this.DataSource["TwOrderInvoiceModel"]; }
			set { this.DataSource["TwOrderInvoiceModel"] = value; }
		}
		/// <summary>物流取引ID</summary>
		public string DeliveryTranId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DELIVERY_TRAN_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_DELIVERY_TRAN_ID] = value; }
		}
		/// <summary>オンライン配信状況</summary>
		public string OnlineDeliveryStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ONLINE_DELIVERY_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ONLINE_DELIVERY_STATUS] = value; }
		}
		/// <summary>外部決済タイプ</summary>
		public string ExternalPaymentType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE] = value; }
		}
		/// <summary>物流連携ステータス</summary>
		public string LogiCooperationStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LOGI_COOPERATION_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_LOGI_COOPERATION_STATUS] = value; }
		}
		/// <summary>Can request cvs def invoice reissue</summary>
		public bool CanRequestCvsDefInvoiceReissue
		{
			get
			{
				return ((this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo));
			}
		}
		/// <summary>Is already shipped</summary>
		public bool IsAlreadyShipped
		{
			get
			{
				return ((this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
					|| (this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP));
			}
		}
		/// <summary>Is order payment status complete</summary>
		public bool IsOrderPaymentStatusComplete
		{
			get { return (this.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE); }
		}
		/// <summary>拡張ステータス41</summary>
		public string ExtendStatus41
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS41]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS41] = value; }
		}
		/// <summary>拡張ステータス更新日時41</summary>
		public string ExtendStatusDate41
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE41]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE41] = value; }
		}
		/// <summary>拡張ステータス42</summary>
		public string ExtendStatus42
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS42]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS42] = value; }
		}
		/// <summary>拡張ステータス更新日時42</summary>
		public string ExtendStatusDate42
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE42]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE42] = value; }
		}
		/// <summary>拡張ステータス43</summary>
		public string ExtendStatus43
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS43]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS43] = value; }
		}
		/// <summary>拡張ステータス更新日時43</summary>
		public string ExtendStatusDate43
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE43]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE43] = value; }
		}
		/// <summary>拡張ステータス44</summary>
		public string ExtendStatus44
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS44]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS44] = value; }
		}
		/// <summary>拡張ステータス更新日時44</summary>
		public string ExtendStatusDate44
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE44]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE44] = value; }
		}
		/// <summary>拡張ステータス45</summary>
		public string ExtendStatus45
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS45]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS45] = value; }
		}
		/// <summary>拡張ステータス更新日時45</summary>
		public string ExtendStatusDate45
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE45]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE45] = value; }
		}
		/// <summary>拡張ステータス46</summary>
		public string ExtendStatus46
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS46]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS46] = value; }
		}
		/// <summary>拡張ステータス更新日時46</summary>
		public string ExtendStatusDate46
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE46]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE46] = value; }
		}
		/// <summary>拡張ステータス47</summary>
		public string ExtendStatus47
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS47]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS47] = value; }
		}
		/// <summary>拡張ステータス更新日時47</summary>
		public string ExtendStatusDate47
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE47]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE47] = value; }
		}
		/// <summary>拡張ステータス48</summary>
		public string ExtendStatus48
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS48]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS48] = value; }
		}
		/// <summary>拡張ステータス更新日時48</summary>
		public string ExtendStatusDate48
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE48]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE48] = value; }
		}
		/// <summary>拡張ステータス49</summary>
		public string ExtendStatus49
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS49]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS49] = value; }
		}
		/// <summary>拡張ステータス更新日時49</summary>
		public string ExtendStatusDate49
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE49]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE49] = value; }
		}
		/// <summary>拡張ステータス50</summary>
		public string ExtendStatus50
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS50]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS50] = value; }
		}
		/// <summary>拡張ステータス更新日時50</summary>
		public string ExtendStatusDate50
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE50]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE50] = value; }
		}
		/// <summary>決済カード取引PASS</summary>
		public string CardTranPass
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_TRAN_PASS]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_TRAN_PASS] = value; }
		}
		/// <summary>注文者情報</summary>
		public Dictionary<string, string> OrderExtendInput
		{
			get { return (Dictionary<string, string>)this.DataSource["OrderExtend"]; }
			set { this.DataSource["OrderExtend"] = value; }
		}
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>頒布会コース定額価格</summary>
		public string SubscriptionBoxFixedAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT] = value; }
		}
		/// <summary>頒布会定額コースか</summary>
		public bool IsSubscriptionBoxFixedAmount
		{
			get { return (string.IsNullOrEmpty(this.SubscriptionBoxFixedAmount) == false); }
		}
		/// <summary>頒布会購入回数</summary>
		public string OrderSubscriptionBoxOrderCount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT] = value; }
		}
		/// <summary>同梱注文か</summary>
		public bool IsOrderCombined => string.IsNullOrEmpty(this.CombinedOrgOrderIds) == false;
		/// <summary>頒布会定額コース商品が含まれるか</summary>
		public bool HasSubscriptionBoxFixedAmountItem
		{
			get
			{
				var result = this.Shippings
					.SelectMany(shipping => shipping.Items)
					.Any(item => item.IsSubscriptionBoxFixedAmount);
				return result;
			}
		}
		/// <summary>注文商品が全て頒布会定額コース商品か</summary>
		public bool IsAllItemsSubscriptionBoxFixedAmount
		{
			get
			{
				// 頒布会定額コースではない商品が無ければTRUE
				var hasItemsNotFixedAmount = this.Shippings
					.SelectMany(shipping => shipping.Items)
					.Any(item => item.IsSubscriptionBoxFixedAmount == false);
				return hasItemsNotFixedAmount == false;
			}
		}
		/// <summary>頒布会商品が含まれる同梱注文か</summary>
		public bool IsOrderCombinedWithSubscriptionBoxItem
		{
			get
			{
				if (this.IsOrderCombined == false) return false;

				var result = this.Shippings
					.SelectMany(shipping => shipping.Items)
					.Any(item => item.IsSubscriptionBox);
				return result;
			}
		}
		/// <summary>注文に含まれる頒布会コースID配列</summary>
		public string[] ItemSubscriptionBoxCourseIds
		{
			get
			{
				var result = this.Shippings
					.SelectMany(shipping => shipping.Items)
					.Where(item => item.IsSubscriptionBox)
					.Select(item => item.SubscriptionBoxCourseId)
					.Distinct()
					.ToArray();
				return result;
			}
		}
		/// <summary>店舗受取ステータス</summary>
		public string StorePickupStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STATUS] = value; }
		}
		/// <summary>店舗到着日</summary>
		public string StorePickupStoreArrivedDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE] = value; }
		}
		/// <summary>引渡し日</summary>
		public string StorePickupDeliveredCompleteDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE] = value; }
		}
		/// <summary>返送日</summary>
		public string StorePickupReturnDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE] = value; }
		}
		/// <summary>クーポンがあるか</summary>
		public bool HasCoupon
		{
			get
			{
				var result = this.Coupons != null
					&& this.Coupons.Any()
					&& (string.IsNullOrEmpty(this.Coupons[0].CouponCode) == false);
				return result;
			}
		}
		#endregion

		#region 拡張ステータスデータクラス
		/// <summary>
		/// 拡張ステータスデータクラス
		/// </summary>
		[Serializable]
		public class ExtendStatusData
		{
			/// <summary>拡張ステータスデータ格納用</summary>
			private List<ExtendStatusDataInner> m_inners = new List<ExtendStatusDataInner>();

			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="source">ソース</param>
			public ExtendStatusData(Hashtable source)
			{
				this.DataSource = source;
			}
			#endregion

			#region インデクサ
			/// <summary>
			/// インデクサ
			/// </summary>
			/// <param name="index">インデックス</param>
			/// <returns>拡張ステータスデータインナー</returns>
			public ExtendStatusDataInner this[int index]
			{
				get
				{
					// 注文拡張ステータスNo範囲外の場合はエラー
					var extendStatusNo = index + 1;
					if (extendStatusNo < 1 || extendStatusNo > Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX)
					{
						throw new ApplicationException(string.Format("拡張ステータスNo範囲外のためエラーが発生しました。（No.{0}）", extendStatusNo));
					}
					// 既にデータが存在する場合、返却
					var inner = m_inners.FirstOrDefault(i => i.ExtendStatusNo == extendStatusNo);
					if (inner != null) return inner;
					inner = new ExtendStatusDataInner(this.DataSource, extendStatusNo);
					m_inners.Add(inner);
					return inner;
				}
			}
			#endregion

			#region プロパティ
			/// <summary>ソース</summary>
			private Hashtable DataSource { get; set; }
			#endregion
		}
		#endregion

		#region 拡張ステータスデータインナークラス
		/// <summary>
		/// 拡張ステータスデータインナークラス
		/// </summary>
		[Serializable]
		public class ExtendStatusDataInner
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="source">ソース</param>
			/// <param name="extendStatusNo">注文拡張ステータスNo</param>
			public ExtendStatusDataInner(Hashtable source, int extendStatusNo)
			{
				this.DataSource = source;
				this.ExtendStatusNo = extendStatusNo;
			}
			#endregion

			#region プロパティ
			/// <summary>注文拡張ステータスNo</summary>
			public int ExtendStatusNo { get; set; }
			/// <summary>拡張ステータス値</summary>
			public string Value
			{
				get { return (string)this.DataSource[this.ValueKey]; }
				set { this.DataSource[this.ValueKey] = value; }
			}
			/// <summary>拡張ステータス値キー</summary>
			private string ValueKey { get { return string.Format(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + "{0}", this.ExtendStatusNo.ToString()); } }
			/// <summary>拡張ステータス更新日時</summary>
			public DateTime? Date
			{
				get
				{
					if (this.DataSource[this.DateKey] == null) return null;
					return DateTime.Parse(this.DataSource[this.DateKey].ToString());
				}
				set { this.DataSource[this.DateKey] = value; }
			}
			/// <summary>拡張ステータス更新日時キー</summary>
			private string DateKey { get { return string.Format(Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + "{0}", this.ExtendStatusNo.ToString()); } }
			/// <summary>ソース</summary>
			private Hashtable DataSource { get; set; }
			#endregion
		}
		#endregion
	}
}

