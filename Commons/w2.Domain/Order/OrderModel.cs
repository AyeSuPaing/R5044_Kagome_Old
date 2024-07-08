/*
=========================================================================================================
  Module      : 注文情報モデル (OrderModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文情報モデル
	/// </summary>
	[Serializable]
	public partial class OrderModel : ModelBase<OrderModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderModel()
		{
			this.OrderId = "";
			this.OrderIdOrg = "";
			this.OrderGroupId = "";
			this.OrderNo = "";
			this.BundleChildOrderIds = "";
			this.BundleParentOrderId = "";
			this.BundleOrderBak = "";
			this.UserId = "";
			this.ShopId = "";
			this.SupplierId = "";
			this.OrderKbn = Constants.FLG_ORDER_ORDER_KBN_PC;
			this.MallId = Constants.FLG_ORDER_MALL_ID_OWN_SITE;
			this.OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT;
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
			this.OrderPaymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
			this.OrderPaymentDate = null;
			this.DemandStatus = Constants.FLG_ORDER_DEMAND_STATUS_LEVEL0;
			this.DemandDate = null;
			this.OrderReturnExchangeStatus = Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_UNKNOWN;
			this.OrderReturnExchangeReceiptDate = null;
			this.OrderReturnExchangeArrivalDate = null;
			this.OrderReturnExchangeCompleteDate = null;
			this.OrderRepaymentStatus = Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_UNKNOWN;
			this.OrderRepaymentDate = null;
			this.OrderItemCount = 0;
			this.OrderProductCount = 0;
			this.OrderPriceSubtotal = 0;
			this.OrderPricePack = 0;
			this.OrderPriceTax = 0;
			this.OrderPriceShipping = 0;
			this.OrderPriceExchange = 0;
			this.OrderPriceRegulation = 0;
			this.OrderPriceRepayment = 0;
			this.OrderPriceTotal = 0;
			this.OrderDiscountSetPrice = 0;
			this.OrderPointUse = 0;
			this.OrderPointUseYen = 0;
			this.OrderPointAdd = 0;
			this.OrderPointRate = 0;
			this.OrderCouponUse = 0;
			this.CardKbn = "";
			this.CardInstruments = "";
			this.CardTranId = "";
			this.ShippingId = "";
			this.FixedPurchaseId = "";
			this.AdvcodeFirst = "";
			this.AdvcodeNew = "";
			this.InflowContentsType = string.Empty;
			this.InflowContentsId = string.Empty;
			this.ShippedChangedKbn = Constants.FLG_ORDER_SHIPPED_CHANGED_KBN_UNKNOWN;
			this.ReturnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN;
			this.ReturnExchangeReasonKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_UNKNOWN;
			this.Attribute1 = "";
			this.Attribute2 = "";
			this.Attribute3 = "";
			this.Attribute4 = "";
			this.Attribute5 = "";
			this.Attribute6 = "";
			this.Attribute7 = "";
			this.Attribute8 = "";
			this.Attribute9 = "";
			this.Attribute10 = "";
			this.ExtendStatus1 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate1 = null;
			this.ExtendStatus2 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate2 = null;
			this.ExtendStatus3 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate3 = null;
			this.ExtendStatus4 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate4 = null;
			this.ExtendStatus5 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate5 = null;
			this.ExtendStatus6 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate6 = null;
			this.ExtendStatus7 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate7 = null;
			this.ExtendStatus8 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate8 = null;
			this.ExtendStatus9 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate9 = null;
			this.ExtendStatus10 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate10 = null;
			this.CareerId = "";
			this.MobileUid = "";
			this.RemoteAddr = "";
			this.Memo = "";
			this.WrappingMemo = "";
			this.PaymentMemo = "";
			this.ManagementMemo = "";
			this.RelationMemo = "";
			this.ReturnExchangeReasonMemo = "";
			this.RegulationMemo = "";
			this.RepaymentMemo = "";
			this.DelFlg = "0";
			this.LastChanged = "";
			this.MemberRankDiscountPrice = 0;
			this.MemberRankId = "";
			this.CreditBranchNo = null;
			this.AffiliateSessionName1 = "";
			this.AffiliateSessionValue1 = "";
			this.AffiliateSessionName2 = "";
			this.AffiliateSessionValue2 = "";
			this.UserAgent = "";
			this.GiftFlg = Constants.FLG_ORDER_GIFT_FLG_OFF;
			this.DigitalContentsFlg = Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_OFF;
			this.Card_3dsecureTranId = "";
			this.Card_3dsecureAuthUrl = "";
			this.Card_3dsecureAuthKey = "";
			this.ShippingPriceSeparateEstimatesFlg = Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID;
			this.OrderTaxIncludedFlg = Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX;
			this.OrderTaxRate = 0;
			this.OrderTaxRoundType = Constants.FLG_ORDER_ORDER_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_DOWN;
			this.ExtendStatus11 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate11 = null;
			this.ExtendStatus12 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate12 = null;
			this.ExtendStatus13 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate13 = null;
			this.ExtendStatus14 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate14 = null;
			this.ExtendStatus15 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate15 = null;
			this.ExtendStatus16 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate16 = null;
			this.ExtendStatus17 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate17 = null;
			this.ExtendStatus18 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate18 = null;
			this.ExtendStatus19 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate19 = null;
			this.ExtendStatus20 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate20 = null;
			this.ExtendStatus21 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate21 = null;
			this.ExtendStatus22 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate22 = null;
			this.ExtendStatus23 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate23 = null;
			this.ExtendStatus24 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate24 = null;
			this.ExtendStatus25 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate25 = null;
			this.ExtendStatus26 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate26 = null;
			this.ExtendStatus27 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate27 = null;
			this.ExtendStatus28 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate28 = null;
			this.ExtendStatus29 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate29 = null;
			this.ExtendStatus30 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate30 = null;
			this.CardInstallmentsCode = "";
			this.SetpromotionProductDiscountAmount = 0;
			this.SetpromotionShippingChargeDiscountAmount = 0;
			this.SetpromotionPaymentChargeDiscountAmount = 0;
			this.OnlinePaymentStatus = "";
			this.FixedPurchaseOrderCount = null;
			this.FixedPurchaseShippedCount = null;
			this.FixedPurchaseDiscountPrice = 0;
			this.PaymentOrderId = "";
			this.FixedPurchaseMemberDiscountAmount = 0;
			this.CombinedOrgOrderIds = "";
			this.LastBilledAmount = 0;
			this.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
			this.ExternalPaymentErrorMessage = "";
			this.ExternalPaymentAuthDate = null;
			this.ExtendStatus31 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate31 = null;
			this.ExtendStatus32 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate32 = null;
			this.ExtendStatus33 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate33 = null;
			this.ExtendStatus34 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate34 = null;
			this.ExtendStatus35 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate35 = null;
			this.ExtendStatus36 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate36 = null;
			this.ExtendStatus37 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate37 = null;
			this.ExtendStatus38 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate38 = null;
			this.ExtendStatus39 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate39 = null;
			this.ExtendStatus40 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate40 = null;
			this.LastOrderPointUse = 0;
			this.LastOrderPointUseYen = 0;
			this.ExternalOrderId = "";
			this.ExternalImportStatus = "";
			this.LastAuthFlg = string.Empty;
			this.MallLinkStatus = Constants.FLG_ORDER_MALL_LINK_STATUS_NONE;
			this.FixedPurchaseKbn = "";
			this.FixedPurchaseSetting1 = "";
			this.OrderPriceSubtotalTax = 0;
			this.SettlementCurrency = "JPY";
			this.SettlementRate = 1;
			this.SettlementAmount = 0;
			this.ShippingMemo = "";
			this.ShippingTaxRate = 0;
			this.PaymentTaxRate = 0;
			this.InvoiceBundleFlg = Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			this.ReceiptFlg = Constants.FLG_ORDER_RECEIPT_FLG_OFF;
			this.ReceiptOutputFlg = Constants.FLG_ORDER_RECEIPT_OUTPUT_FLG_OFF;
			this.ReceiptAddress = "";
			this.ReceiptProviso = "";
			this.DeliveryTranId = string.Empty;
			this.OnlineDeliveryStatus = string.Empty;
			this.ExternalPaymentType = string.Empty;
			this.LogiCooperationStatus = string.Empty;
			this.ExtendStatus41 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate41 = null;
			this.ExtendStatus42 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate42 = null;
			this.ExtendStatus43 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate43 = null;
			this.ExtendStatus44 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate44 = null;
			this.ExtendStatus45 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate45 = null;
			this.ExtendStatus46 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate46 = null;
			this.ExtendStatus47 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate47 = null;
			this.ExtendStatus48 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate48 = null;
			this.ExtendStatus49 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate49 = null;
			this.ExtendStatus50 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			this.ExtendStatusDate50 = null;
			this.CardTranPass = string.Empty;
			this.SubscriptionBoxCourseId = "";
			this.SubscriptionBoxFixedAmount = null;
			this.OrderSubscriptionBoxOrderCount = 0;
			this.IsUpdateBokuPaymentFromMyPage = false;
			this.StorePickupStatus = string.Empty;
			this.StorePickupStoreArrivedDate = null;
			this.StorePickupDeliveredCompleteDate = null;
			this.StorePickupReturnDate = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		[UpdateDataAttribute(1, "order_id")]
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_ID] = value; }
		}
		/// <summary>元注文ID</summary>
		[UpdateDataAttribute(2, "order_id_org")]
		public string OrderIdOrg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_ID_ORG]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_ID_ORG] = value; }
		}
		/// <summary>注文グループID</summary>
		[UpdateDataAttribute(3, "order_group_id")]
		public string OrderGroupId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_GROUP_ID] = value; }
		}
		/// <summary>注文番号</summary>
		[UpdateDataAttribute(4, "order_no")]
		public string OrderNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_NO]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_NO] = value; }
		}
		/// <summary>同梱子注文ID列</summary>
		[UpdateDataAttribute(5, "bundle_child_order_ids")]
		public string BundleChildOrderIds
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_BUNDLE_CHILD_ORDER_IDS]; }
			set { this.DataSource[Constants.FIELD_ORDER_BUNDLE_CHILD_ORDER_IDS] = value; }
		}
		/// <summary>同梱親注文ID</summary>
		[UpdateDataAttribute(6, "bundle_parent_order_id")]
		public string BundleParentOrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_BUNDLE_PARENT_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_BUNDLE_PARENT_ORDER_ID] = value; }
		}
		/// <summary>同梱済注文XML</summary>
		[UpdateDataAttribute(7, "bundle_order_bak")]
		public string BundleOrderBak
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_BUNDLE_ORDER_BAK]; }
			set { this.DataSource[Constants.FIELD_ORDER_BUNDLE_ORDER_BAK] = value; }
		}
		/// <summary>ユーザID</summary>
		[UpdateDataAttribute(8, "user_id")]
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_USER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_USER_ID] = value; }
		}
		/// <summary>店舗ID</summary>
		[UpdateDataAttribute(9, "shop_id")]
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHOP_ID] = value; }
		}
		/// <summary>サプライヤID</summary>
		[UpdateDataAttribute(10, "supplier_id")]
		public string SupplierId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SUPPLIER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_SUPPLIER_ID] = value; }
		}
		/// <summary>注文区分</summary>
		[UpdateDataAttribute(11, "order_kbn")]
		public string OrderKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_KBN] = value; }
		}
		/// <summary>モールID</summary>
		[UpdateDataAttribute(12, "mall_id")]
		public string MallId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MALL_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_MALL_ID] = value; }
		}
		/// <summary>支払区分</summary>
		[UpdateDataAttribute(13, "order_payment_kbn")]
		public string OrderPaymentKbn
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] = value; }
		}
		/// <summary>注文ステータス</summary>
		[UpdateDataAttribute(14, "order_status")]
		public string OrderStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_STATUS] = value; }
		}
		/// <summary>注文日時</summary>
		[UpdateDataAttribute(15, "order_date")]
		public DateTime? OrderDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_DATE] = value; }
		}
		/// <summary>受注承認日時</summary>
		[UpdateDataAttribute(16, "order_recognition_date")]
		public DateTime? OrderRecognitionDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE] = value; }
		}
		/// <summary>在庫引当ステータス</summary>
		[UpdateDataAttribute(17, "order_stockreserved_status")]
		public string OrderStockreservedStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS] = value; }
		}
		/// <summary>在庫引当日時</summary>
		[UpdateDataAttribute(18, "order_stockreserved_date")]
		public DateTime? OrderStockreservedDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE] = value; }
		}
		/// <summary>出荷手配日時</summary>
		[UpdateDataAttribute(19, "order_shipping_date")]
		public DateTime? OrderShippingDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE] = value; }
		}
		/// <summary>出荷ステータス</summary>
		[UpdateDataAttribute(20, "order_shipped_status")]
		public string OrderShippedStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS] = value; }
		}
		/// <summary>出荷完了日時</summary>
		[UpdateDataAttribute(21, "order_shipped_date")]
		public DateTime? OrderShippedDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE] = value; }
		}
		/// <summary>配送完了日時</summary>
		[UpdateDataAttribute(22, "order_delivering_date")]
		public DateTime? OrderDeliveringDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE] = value; }
		}
		/// <summary>キャンセル日時</summary>
		[UpdateDataAttribute(23, "order_cancel_date")]
		public DateTime? OrderCancelDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_CANCEL_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_CANCEL_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_CANCEL_DATE] = value; }
		}
		/// <summary>返品日時</summary>
		[UpdateDataAttribute(24, "order_return_date")]
		public DateTime? OrderReturnDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_DATE] = value; }
		}
		/// <summary>入金ステータス</summary>
		[UpdateDataAttribute(25, "order_payment_status")]
		public string OrderPaymentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = value; }
		}
		/// <summary>入金確認日時</summary>
		[UpdateDataAttribute(26, "order_payment_date")]
		public DateTime? OrderPaymentDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE] = value; }
		}
		/// <summary>督促ステータス</summary>
		[UpdateDataAttribute(27, "demand_status")]
		public string DemandStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DEMAND_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_DEMAND_STATUS] = value; }
		}
		/// <summary>督促日時</summary>
		[UpdateDataAttribute(28, "demand_date")]
		public DateTime? DemandDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_DEMAND_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_DEMAND_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_DEMAND_DATE] = value; }
		}
		/// <summary>返品交換ステータス</summary>
		[UpdateDataAttribute(29, "order_return_exchange_status")]
		public string OrderReturnExchangeStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS] = value; }
		}
		/// <summary>返品交換受付日時</summary>
		[UpdateDataAttribute(30, "order_return_exchange_receipt_date")]
		public DateTime? OrderReturnExchangeReceiptDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE] = value; }
		}
		/// <summary>返品交換商品到着日時</summary>
		[UpdateDataAttribute(31, "order_return_exchange_arrival_date")]
		public DateTime? OrderReturnExchangeArrivalDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE] = value; }
		}
		/// <summary>返品交換完了日時</summary>
		[UpdateDataAttribute(32, "order_return_exchange_complete_date")]
		public DateTime? OrderReturnExchangeCompleteDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE] = value; }
		}
		/// <summary>返金ステータス</summary>
		[UpdateDataAttribute(33, "order_repayment_status")]
		public string OrderRepaymentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS] = value; }
		}
		/// <summary>返金日時</summary>
		[UpdateDataAttribute(34, "order_repayment_date")]
		public DateTime? OrderRepaymentDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE] = value; }
		}
		/// <summary>注文アイテム数</summary>
		[UpdateDataAttribute(35, "order_item_count")]
		public int OrderItemCount
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDER_ORDER_ITEM_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_ITEM_COUNT] = value; }
		}
		/// <summary>注文商品数</summary>
		[UpdateDataAttribute(36, "order_product_count")]
		public int OrderProductCount
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT] = value; }
		}
		/// <summary>小計</summary>
		[UpdateDataAttribute(37, "order_price_subtotal")]
		public decimal OrderPriceSubtotal
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL] = value; }
		}
		/// <summary>荷造金額</summary>
		[UpdateDataAttribute(38, "order_price_pack")]
		public decimal OrderPricePack
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_PACK]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_PACK] = value; }
		}
		/// <summary>税金合計</summary>
		[UpdateDataAttribute(39, "order_price_tax")]
		public decimal OrderPriceTax
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TAX] = value; }
		}
		/// <summary>配送料</summary>
		[UpdateDataAttribute(40, "order_price_shipping")]
		public decimal OrderPriceShipping
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING] = value; }
		}
		/// <summary>代引手数料</summary>
		[UpdateDataAttribute(41, "order_price_exchange")]
		public decimal OrderPriceExchange
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE] = value; }
		}
		/// <summary>調整金額</summary>
		[UpdateDataAttribute(42, "order_price_regulation")]
		public decimal OrderPriceRegulation
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_REGULATION]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_REGULATION] = value; }
		}
		/// <summary>返金金額</summary>
		[UpdateDataAttribute(43, "order_price_repayment")]
		public decimal OrderPriceRepayment
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT] = value; }
		}
		/// <summary>支払金額合計</summary>
		[UpdateDataAttribute(44, "order_price_total")]
		public decimal OrderPriceTotal
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL] = value; }
		}
		/// <summary>セット値引金額</summary>
		[UpdateDataAttribute(45, "order_discount_set_price")]
		public decimal OrderDiscountSetPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_DISCOUNT_SET_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_DISCOUNT_SET_PRICE] = value; }
		}
		/// <summary>利用ポイント数</summary>
		[UpdateDataAttribute(46, "order_point_use")]
		public decimal OrderPointUse
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE] = value; }
		}
		/// <summary>ポイント利用額</summary>
		[UpdateDataAttribute(47, "order_point_use_yen")]
		public decimal OrderPointUseYen
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE_YEN]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_USE_YEN] = value; }
		}
		/// <summary>付与ポイント</summary>
		[UpdateDataAttribute(48, "order_point_add")]
		public decimal OrderPointAdd
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_ADD]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_ADD] = value; }
		}
		/// <summary>ポイント調整率</summary>
		[UpdateDataAttribute(49, "order_point_rate")]
		public decimal OrderPointRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_POINT_RATE] = value; }
		}
		/// <summary>クーポン割引額</summary>
		[UpdateDataAttribute(50, "order_coupon_use")]
		public decimal OrderCouponUse
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_COUPON_USE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_COUPON_USE] = value; }
		}
		/// <summary>決済カード区分</summary>
		[UpdateDataAttribute(51, "card_kbn")]
		public string CardKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_KBN] = value; }
		}
		/// <summary>決済カード支払回数文言</summary>
		[UpdateDataAttribute(52, "card_instruments")]
		public string CardInstruments
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_INSTRUMENTS]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_INSTRUMENTS] = value; }
		}
		/// <summary>決済カード取引ID</summary>
		[UpdateDataAttribute(53, "card_tran_id")]
		public string CardTranId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_TRAN_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_TRAN_ID] = value; }
		}
		/// <summary>配送種別ID</summary>
		[UpdateDataAttribute(54, "shipping_id")]
		public string ShippingId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHIPPING_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPING_ID] = value; }
		}
		/// <summary>定期購入ID</summary>
		[UpdateDataAttribute(55, "fixed_purchase_id")]
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_ID] = value; }
		}
		/// <summary>初回広告コード</summary>
		[UpdateDataAttribute(56, "advcode_first")]
		public string AdvcodeFirst
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ADVCODE_FIRST]; }
			set { this.DataSource[Constants.FIELD_ORDER_ADVCODE_FIRST] = value; }
		}
		/// <summary>最新広告コード</summary>
		[UpdateDataAttribute(57, "advcode_new")]
		public string AdvcodeNew
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ADVCODE_NEW]; }
			set { this.DataSource[Constants.FIELD_ORDER_ADVCODE_NEW] = value; }
		}
		/// <summary>出荷後変更区分</summary>
		[UpdateDataAttribute(58, "shipped_changed_kbn")]
		public string ShippedChangedKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN] = value; }
		}
		/// <summary>返品交換区分</summary>
		[UpdateDataAttribute(59, "return_exchange_kbn")]
		public string ReturnExchangeKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN] = value; }
		}
		/// <summary>返品交換都合区分</summary>
		[UpdateDataAttribute(60, "return_exchange_reason_kbn")]
		public string ReturnExchangeReasonKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN] = value; }
		}
		/// <summary>注文拡張項目1</summary>
		[UpdateDataAttribute(61, "attribute1")]
		public string Attribute1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE1]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE1] = value; }
		}
		/// <summary>注文拡張項目2</summary>
		[UpdateDataAttribute(62, "attribute2")]
		public string Attribute2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE2]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE2] = value; }
		}
		/// <summary>注文拡張項目3</summary>
		[UpdateDataAttribute(63, "attribute3")]
		public string Attribute3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE3]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE3] = value; }
		}
		/// <summary>注文拡張項目4</summary>
		[UpdateDataAttribute(64, "attribute4")]
		public string Attribute4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE4]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE4] = value; }
		}
		/// <summary>注文拡張項目5</summary>
		[UpdateDataAttribute(65, "attribute5")]
		public string Attribute5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE5]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE5] = value; }
		}
		/// <summary>注文拡張項目6</summary>
		[UpdateDataAttribute(66, "attribute6")]
		public string Attribute6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE6]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE6] = value; }
		}
		/// <summary>注文拡張項目7</summary>
		[UpdateDataAttribute(67, "attribute7")]
		public string Attribute7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE7]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE7] = value; }
		}
		/// <summary>注文拡張項目8</summary>
		[UpdateDataAttribute(68, "attribute8")]
		public string Attribute8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE8]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE8] = value; }
		}
		/// <summary>注文拡張項目9</summary>
		[UpdateDataAttribute(69, "attribute9")]
		public string Attribute9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE9]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE9] = value; }
		}
		/// <summary>注文拡張項目10</summary>
		[UpdateDataAttribute(70, "attribute10")]
		public string Attribute10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE10]; }
			set { this.DataSource[Constants.FIELD_ORDER_ATTRIBUTE10] = value; }
		}
		/// <summary>拡張ステータス１</summary>
		[UpdateDataAttribute(71, "extend_status1")]
		public string ExtendStatus1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS1]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS1] = value; }
		}
		/// <summary>拡張ステータス更新日時１</summary>
		[UpdateDataAttribute(72, "extend_status_date1")]
		public DateTime? ExtendStatusDate1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE1] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE1];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE1] = value; }
		}
		/// <summary>拡張ステータス２</summary>
		[UpdateDataAttribute(73, "extend_status2")]
		public string ExtendStatus2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS2]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS2] = value; }
		}
		/// <summary>拡張ステータス更新日時２</summary>
		[UpdateDataAttribute(74, "extend_status_date2")]
		public DateTime? ExtendStatusDate2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE2] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE2];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE2] = value; }
		}
		/// <summary>拡張ステータス３</summary>
		[UpdateDataAttribute(75, "extend_status3")]
		public string ExtendStatus3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS3]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS3] = value; }
		}
		/// <summary>拡張ステータス更新日時３</summary>
		[UpdateDataAttribute(76, "extend_status_date3")]
		public DateTime? ExtendStatusDate3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE3] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE3];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE3] = value; }
		}
		/// <summary>拡張ステータス４</summary>
		[UpdateDataAttribute(77, "extend_status4")]
		public string ExtendStatus4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS4]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS4] = value; }
		}
		/// <summary>拡張ステータス更新日時４</summary>
		[UpdateDataAttribute(78, "extend_status_date4")]
		public DateTime? ExtendStatusDate4
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE4] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE4];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE4] = value; }
		}
		/// <summary>拡張ステータス５</summary>
		[UpdateDataAttribute(79, "extend_status5")]
		public string ExtendStatus5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS5]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS5] = value; }
		}
		/// <summary>拡張ステータス更新日時５</summary>
		[UpdateDataAttribute(80, "extend_status_date5")]
		public DateTime? ExtendStatusDate5
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE5] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE5];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE5] = value; }
		}
		/// <summary>拡張ステータス６</summary>
		[UpdateDataAttribute(81, "extend_status6")]
		public string ExtendStatus6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS6]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS6] = value; }
		}
		/// <summary>拡張ステータス更新日時６</summary>
		[UpdateDataAttribute(82, "extend_status_date6")]
		public DateTime? ExtendStatusDate6
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE6] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE6];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE6] = value; }
		}
		/// <summary>拡張ステータス７</summary>
		[UpdateDataAttribute(83, "extend_status7")]
		public string ExtendStatus7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS7]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS7] = value; }
		}
		/// <summary>拡張ステータス更新日時７</summary>
		[UpdateDataAttribute(84, "extend_status_date7")]
		public DateTime? ExtendStatusDate7
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE7] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE7];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE7] = value; }
		}
		/// <summary>拡張ステータス８</summary>
		[UpdateDataAttribute(85, "extend_status8")]
		public string ExtendStatus8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS8]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS8] = value; }
		}
		/// <summary>拡張ステータス更新日時８</summary>
		[UpdateDataAttribute(86, "extend_status_date8")]
		public DateTime? ExtendStatusDate8
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE8] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE8];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE8] = value; }
		}
		/// <summary>拡張ステータス９</summary>
		[UpdateDataAttribute(87, "extend_status9")]
		public string ExtendStatus9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS9]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS9] = value; }
		}
		/// <summary>拡張ステータス更新日時９</summary>
		[UpdateDataAttribute(88, "extend_status_date9")]
		public DateTime? ExtendStatusDate9
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE9] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE9];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE9] = value; }
		}
		/// <summary>拡張ステータス１０</summary>
		[UpdateDataAttribute(89, "extend_status10")]
		public string ExtendStatus10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS10]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS10] = value; }
		}
		/// <summary>拡張ステータス更新日時１０</summary>
		[UpdateDataAttribute(90, "extend_status_date10")]
		public DateTime? ExtendStatusDate10
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE10] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE10];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE10] = value; }
		}
		/// <summary>キャリアID</summary>
		[UpdateDataAttribute(91, "career_id")]
		public string CareerId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CAREER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_CAREER_ID] = value; }
		}
		/// <summary>モバイルUID</summary>
		[UpdateDataAttribute(92, "mobile_uid")]
		public string MobileUid
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MOBILE_UID]; }
			set { this.DataSource[Constants.FIELD_ORDER_MOBILE_UID] = value; }
		}
		/// <summary>リモートIPアドレス</summary>
		[UpdateDataAttribute(93, "remote_addr")]
		public string RemoteAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_REMOTE_ADDR]; }
			set { this.DataSource[Constants.FIELD_ORDER_REMOTE_ADDR] = value; }
		}
		/// <summary>メモ</summary>
		[UpdateDataAttribute(94, "memo")]
		public string Memo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_MEMO] = value; }
		}
		/// <summary>熨斗メモ</summary>
		[UpdateDataAttribute(95, "wrapping_memo")]
		public string WrappingMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_WRAPPING_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_WRAPPING_MEMO] = value; }
		}
		/// <summary>決済連携メモ</summary>
		[UpdateDataAttribute(96, "payment_memo")]
		public string PaymentMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_PAYMENT_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_PAYMENT_MEMO] = value; }
		}
		/// <summary>管理メモ</summary>
		[UpdateDataAttribute(97, "management_memo")]
		public string ManagementMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MANAGEMENT_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_MANAGEMENT_MEMO] = value; }
		}
		/// <summary>外部連携メモ</summary>
		[UpdateDataAttribute(98, "relation_memo")]
		public string RelationMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RELATION_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_RELATION_MEMO] = value; }
		}
		/// <summary>返品交換理由メモ</summary>
		[UpdateDataAttribute(99, "return_exchange_reason_memo")]
		public string ReturnExchangeReasonMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_MEMO] = value; }
		}
		/// <summary>調整金額メモ</summary>
		[UpdateDataAttribute(100, "regulation_memo")]
		public string RegulationMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_REGULATION_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_REGULATION_MEMO] = value; }
		}
		/// <summary>返金メモ</summary>
		[UpdateDataAttribute(101, "repayment_memo")]
		public string RepaymentMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_REPAYMENT_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_REPAYMENT_MEMO] = value; }
		}
		/// <summary>削除フラグ</summary>
		[UpdateDataAttribute(102, "del_flg")]
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateDataAttribute(103, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDER_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateDataAttribute(104, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDER_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDER_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		[UpdateDataAttribute(105, "last_changed")]
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_CHANGED] = value; }
		}
		/// <summary>会員ランク割引額</summary>
		[UpdateDataAttribute(106, "member_rank_discount_price")]
		public decimal MemberRankDiscountPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE] = value; }
		}
		/// <summary>注文時会員ランク</summary>
		[UpdateDataAttribute(107, "member_rank_id")]
		public string MemberRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MEMBER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_MEMBER_RANK_ID] = value; }
		}
		/// <summary>クレジットカード枝番</summary>
		[UpdateDataAttribute(108, "credit_branch_no")]
		public int? CreditBranchNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDER_CREDIT_BRANCH_NO];
			}
			set { this.DataSource[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = value; }
		}
		/// <summary>アフィリエイトセッション変数名1</summary>
		[UpdateDataAttribute(109, "affiliate_session_name1")]
		public string AffiliateSessionName1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME1]; }
			set { this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME1] = value; }
		}
		/// <summary>アフィリエイトセッション値1</summary>
		[UpdateDataAttribute(110, "affiliate_session_value1")]
		public string AffiliateSessionValue1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE1]; }
			set { this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE1] = value; }
		}
		/// <summary>アフィリエイトセッション変数名2</summary>
		[UpdateDataAttribute(111, "affiliate_session_name2")]
		public string AffiliateSessionName2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME2]; }
			set { this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME2] = value; }
		}
		/// <summary>アフィリエイトセッション値2</summary>
		[UpdateDataAttribute(112, "affiliate_session_value2")]
		public string AffiliateSessionValue2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE2]; }
			set { this.DataSource[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE2] = value; }
		}
		/// <summary>ユーザーエージェント</summary>
		[UpdateDataAttribute(113, "user_agent")]
		public string UserAgent
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_USER_AGENT]; }
			set { this.DataSource[Constants.FIELD_ORDER_USER_AGENT] = value; }
		}
		/// <summary>ギフト購入フラグ</summary>
		[UpdateDataAttribute(114, "gift_flg")]
		public string GiftFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_GIFT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_GIFT_FLG] = value; }
		}
		/// <summary>デジタルコンテンツ購入フラグ</summary>
		[UpdateDataAttribute(115, "digital_contents_flg")]
		public string DigitalContentsFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG] = value; }
		}
		/// <summary>3DセキュアトランザクションID</summary>
		[UpdateDataAttribute(116, "card_3dsecure_tran_id")]
		public string Card_3dsecureTranId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_TRAN_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_TRAN_ID] = value; }
		}
		/// <summary>3Dセキュア認証URL</summary>
		[UpdateDataAttribute(117, "card_3dsecure_auth_url")]
		public string Card_3dsecureAuthUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL] = value; }
		}
		/// <summary>3Dセキュア認証キー</summary>
		[UpdateDataAttribute(118, "card_3dsecure_auth_key")]
		public string Card_3dsecureAuthKey
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_KEY]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_KEY] = value; }
		}
		/// <summary>配送料の別見積もりの利用フラグ</summary>
		[UpdateDataAttribute(119, "shipping_price_separate_estimates_flg")]
		public string ShippingPriceSeparateEstimatesFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] = value; }
		}
		/// <summary>税込フラグ</summary>
		[UpdateDataAttribute(120, "order_tax_included_flg")]
		public string OrderTaxIncludedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_INCLUDED_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_INCLUDED_FLG] = value; }
		}
		/// <summary>税率</summary>
		/// <remarks>使用しない</remarks>  
		[UpdateDataAttribute(121, "order_tax_rate")]
		public decimal OrderTaxRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_RATE] = value; }
		}
		/// <summary>税計算方法</summary>
		[UpdateDataAttribute(122, "order_tax_round_type")]
		public string OrderTaxRoundType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_ROUND_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_TAX_ROUND_TYPE] = value; }
		}
		/// <summary>拡張ステータス11</summary>
		[UpdateDataAttribute(123, "extend_status11")]
		public string ExtendStatus11
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS11]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS11] = value; }
		}
		/// <summary>拡張ステータス更新日時11</summary>
		[UpdateDataAttribute(124, "extend_status_date11")]
		public DateTime? ExtendStatusDate11
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE11] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE11];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE11] = value; }
		}
		/// <summary>拡張ステータス12</summary>
		[UpdateDataAttribute(125, "extend_status12")]
		public string ExtendStatus12
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS12]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS12] = value; }
		}
		/// <summary>拡張ステータス更新日時12</summary>
		[UpdateDataAttribute(126, "extend_status_date12")]
		public DateTime? ExtendStatusDate12
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE12] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE12];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE12] = value; }
		}
		/// <summary>拡張ステータス13</summary>
		[UpdateDataAttribute(127, "extend_status13")]
		public string ExtendStatus13
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS13]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS13] = value; }
		}
		/// <summary>拡張ステータス更新日時13</summary>
		[UpdateDataAttribute(128, "extend_status_date13")]
		public DateTime? ExtendStatusDate13
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE13] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE13];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE13] = value; }
		}
		/// <summary>拡張ステータス14</summary>
		[UpdateDataAttribute(129, "extend_status14")]
		public string ExtendStatus14
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS14]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS14] = value; }
		}
		/// <summary>拡張ステータス更新日時14</summary>
		[UpdateDataAttribute(130, "extend_status_date14")]
		public DateTime? ExtendStatusDate14
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE14] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE14];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE14] = value; }
		}
		/// <summary>拡張ステータス15</summary>
		[UpdateDataAttribute(131, "extend_status15")]
		public string ExtendStatus15
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS15]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS15] = value; }
		}
		/// <summary>拡張ステータス更新日時15</summary>
		[UpdateDataAttribute(132, "extend_status_date15")]
		public DateTime? ExtendStatusDate15
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE15] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE15];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE15] = value; }
		}
		/// <summary>拡張ステータス16</summary>
		[UpdateDataAttribute(133, "extend_status16")]
		public string ExtendStatus16
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS16]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS16] = value; }
		}
		/// <summary>拡張ステータス更新日時16</summary>
		[UpdateDataAttribute(134, "extend_status_date16")]
		public DateTime? ExtendStatusDate16
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE16] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE16];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE16] = value; }
		}
		/// <summary>拡張ステータス17</summary>
		[UpdateDataAttribute(135, "extend_status17")]
		public string ExtendStatus17
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS17]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS17] = value; }
		}
		/// <summary>拡張ステータス更新日時17</summary>
		[UpdateDataAttribute(136, "extend_status_date17")]
		public DateTime? ExtendStatusDate17
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE17] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE17];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE17] = value; }
		}
		/// <summary>拡張ステータス18</summary>
		[UpdateDataAttribute(137, "extend_status18")]
		public string ExtendStatus18
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS18]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS18] = value; }
		}
		/// <summary>拡張ステータス更新日時18</summary>
		[UpdateDataAttribute(138, "extend_status_date18")]
		public DateTime? ExtendStatusDate18
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE18] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE18];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE18] = value; }
		}
		/// <summary>拡張ステータス19</summary>
		[UpdateDataAttribute(139, "extend_status19")]
		public string ExtendStatus19
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS19]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS19] = value; }
		}
		/// <summary>拡張ステータス更新日時19</summary>
		[UpdateDataAttribute(140, "extend_status_date19")]
		public DateTime? ExtendStatusDate19
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE19] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE19];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE19] = value; }
		}
		/// <summary>拡張ステータス20</summary>
		[UpdateDataAttribute(141, "extend_status20")]
		public string ExtendStatus20
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS20]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS20] = value; }
		}
		/// <summary>拡張ステータス更新日時20</summary>
		[UpdateDataAttribute(142, "extend_status_date20")]
		public DateTime? ExtendStatusDate20
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE20] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE20];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE20] = value; }
		}
		/// <summary>拡張ステータス21</summary>
		[UpdateDataAttribute(143, "extend_status21")]
		public string ExtendStatus21
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS21]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS21] = value; }
		}
		/// <summary>拡張ステータス更新日時21</summary>
		[UpdateDataAttribute(144, "extend_status_date21")]
		public DateTime? ExtendStatusDate21
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE21] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE21];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE21] = value; }
		}
		/// <summary>拡張ステータス22</summary>
		[UpdateDataAttribute(145, "extend_status22")]
		public string ExtendStatus22
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS22]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS22] = value; }
		}
		/// <summary>拡張ステータス更新日時22</summary>
		[UpdateDataAttribute(146, "extend_status_date22")]
		public DateTime? ExtendStatusDate22
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE22] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE22];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE22] = value; }
		}
		/// <summary>拡張ステータス23</summary>
		[UpdateDataAttribute(147, "extend_status23")]
		public string ExtendStatus23
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS23]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS23] = value; }
		}
		/// <summary>拡張ステータス更新日時23</summary>
		[UpdateDataAttribute(148, "extend_status_date23")]
		public DateTime? ExtendStatusDate23
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE23] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE23];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE23] = value; }
		}
		/// <summary>拡張ステータス24</summary>
		[UpdateDataAttribute(149, "extend_status24")]
		public string ExtendStatus24
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS24]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS24] = value; }
		}
		/// <summary>拡張ステータス更新日時24</summary>
		[UpdateDataAttribute(150, "extend_status_date24")]
		public DateTime? ExtendStatusDate24
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE24] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE24];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE24] = value; }
		}
		/// <summary>拡張ステータス25</summary>
		[UpdateDataAttribute(151, "extend_status25")]
		public string ExtendStatus25
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS25]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS25] = value; }
		}
		/// <summary>拡張ステータス更新日時25</summary>
		[UpdateDataAttribute(152, "extend_status_date25")]
		public DateTime? ExtendStatusDate25
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE25] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE25];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE25] = value; }
		}
		/// <summary>拡張ステータス26</summary>
		[UpdateDataAttribute(153, "extend_status26")]
		public string ExtendStatus26
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS26]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS26] = value; }
		}
		/// <summary>拡張ステータス更新日時26</summary>
		[UpdateDataAttribute(154, "extend_status_date26")]
		public DateTime? ExtendStatusDate26
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE26] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE26];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE26] = value; }
		}
		/// <summary>拡張ステータス27</summary>
		[UpdateDataAttribute(155, "extend_status27")]
		public string ExtendStatus27
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS27]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS27] = value; }
		}
		/// <summary>拡張ステータス更新日時27</summary>
		[UpdateDataAttribute(156, "extend_status_date27")]
		public DateTime? ExtendStatusDate27
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE27] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE27];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE27] = value; }
		}
		/// <summary>拡張ステータス28</summary>
		[UpdateDataAttribute(157, "extend_status28")]
		public string ExtendStatus28
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS28]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS28] = value; }
		}
		/// <summary>拡張ステータス更新日時28</summary>
		[UpdateDataAttribute(158, "extend_status_date28")]
		public DateTime? ExtendStatusDate28
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE28] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE28];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE28] = value; }
		}
		/// <summary>拡張ステータス29</summary>
		[UpdateDataAttribute(159, "extend_status29")]
		public string ExtendStatus29
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS29]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS29] = value; }
		}
		/// <summary>拡張ステータス更新日時29</summary>
		[UpdateDataAttribute(160, "extend_status_date29")]
		public DateTime? ExtendStatusDate29
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE29] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE29];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE29] = value; }
		}
		/// <summary>拡張ステータス30</summary>
		[UpdateDataAttribute(161, "extend_status30")]
		public string ExtendStatus30
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS30]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS30] = value; }
		}
		/// <summary>拡張ステータス更新日時30</summary>
		[UpdateDataAttribute(162, "extend_status_date30")]
		public DateTime? ExtendStatusDate30
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE30] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE30];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE30] = value; }
		}
		/// <summary>カード支払い回数コード</summary>
		[UpdateDataAttribute(163, "card_installments_code")]
		public string CardInstallmentsCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE] = value; }
		}
		/// <summary>セットプロモーション商品割引額</summary>
		[UpdateDataAttribute(164, "setpromotion_product_discount_amount")]
		public decimal SetpromotionProductDiscountAmount
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>セットプロモーション配送料割引額</summary>
		[UpdateDataAttribute(165, "setpromotion_shipping_charge_discount_amount")]
		public decimal SetpromotionShippingChargeDiscountAmount
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>セットプロモーション決済手数料割引額</summary>
		[UpdateDataAttribute(166, "setpromotion_payment_charge_discount_amount")]
		public decimal SetpromotionPaymentChargeDiscountAmount
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>オンライン決済ステータス</summary>
		[UpdateDataAttribute(167, "online_payment_status")]
		public string OnlinePaymentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS] = value; }
		}
		/// <summary>定期購入回数(注文時点)</summary>
		[UpdateDataAttribute(168, "fixed_purchase_order_count")]
		public int? FixedPurchaseOrderCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT];
			}
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT] = value; }
		}
		/// <summary>定期購入回数(出荷時点)</summary>
		[UpdateDataAttribute(169, "fixed_purchase_shipped_count")]
		public int? FixedPurchaseShippedCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT];
			}
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT] = value; }
		}
		/// <summary>定期購入割引額</summary>
		[UpdateDataAttribute(170, "fixed_purchase_discount_price")]
		public decimal FixedPurchaseDiscountPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE] = value; }
		}
		/// <summary>決済注文ID</summary>
		[UpdateDataAttribute(171, "payment_order_id")]
		public string PaymentOrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = value; }
		}
		/// <summary>定期会員割引額</summary>
		[UpdateDataAttribute(172, "fixed_purchase_member_discount_amount")]
		public decimal FixedPurchaseMemberDiscountAmount
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>注文同梱元注文ID</summary>
		[UpdateDataAttribute(173, "combined_org_order_ids")]
		public string CombinedOrgOrderIds
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS]; }
			set { this.DataSource[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS] = value; }
		}
		/// <summary>最終請求金額</summary>
		[UpdateDataAttribute(174, "last_billed_amount")]
		public decimal LastBilledAmount
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT] = value; }
		}
		/// <summary>外部決済ステータス</summary>
		[UpdateDataAttribute(175, "external_payment_status")]
		public string ExternalPaymentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = value; }
		}
		/// <summary>外部決済エラーメッセージ</summary>
		[UpdateDataAttribute(176, "external_payment_error_message")]
		public string ExternalPaymentErrorMessage
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_ERROR_MESSAGE]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_ERROR_MESSAGE] = value; }
		}
		/// <summary>外部決済与信日時</summary>
		[UpdateDataAttribute(177, "external_payment_auth_date")]
		public DateTime? ExternalPaymentAuthDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE] = value; }
		}
		/// <summary>拡張ステータス31</summary>
		[UpdateDataAttribute(178, "extend_status31")]
		public string ExtendStatus31
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS31]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS31] = value; }
		}
		/// <summary>拡張ステータス更新日時31</summary>
		[UpdateDataAttribute(179, "extend_status_date31")]
		public DateTime? ExtendStatusDate31
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE31] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE31];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE31] = value; }
		}
		/// <summary>拡張ステータス32</summary>
		[UpdateDataAttribute(180, "extend_status32")]
		public string ExtendStatus32
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS32]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS32] = value; }
		}
		/// <summary>拡張ステータス更新日時32</summary>
		[UpdateDataAttribute(181, "extend_status_date32")]
		public DateTime? ExtendStatusDate32
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE32] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE32];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE32] = value; }
		}
		/// <summary>拡張ステータス33</summary>
		[UpdateDataAttribute(182, "extend_status33")]
		public string ExtendStatus33
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS33]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS33] = value; }
		}
		/// <summary>拡張ステータス更新日時33</summary>
		[UpdateDataAttribute(183, "extend_status_date33")]
		public DateTime? ExtendStatusDate33
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE33] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE33];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE33] = value; }
		}
		/// <summary>拡張ステータス34</summary>
		[UpdateDataAttribute(184, "extend_status34")]
		public string ExtendStatus34
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS34]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS34] = value; }
		}
		/// <summary>拡張ステータス更新日時34</summary>
		[UpdateDataAttribute(185, "extend_status_date34")]
		public DateTime? ExtendStatusDate34
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE34] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE34];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE34] = value; }
		}
		/// <summary>拡張ステータス35</summary>
		[UpdateDataAttribute(186, "extend_status35")]
		public string ExtendStatus35
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS35]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS35] = value; }
		}
		/// <summary>拡張ステータス更新日時35</summary>
		[UpdateDataAttribute(187, "extend_status_date35")]
		public DateTime? ExtendStatusDate35
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE35] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE35];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE35] = value; }
		}
		/// <summary>拡張ステータス36</summary>
		[UpdateDataAttribute(188, "extend_status36")]
		public string ExtendStatus36
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS36]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS36] = value; }
		}
		/// <summary>拡張ステータス更新日時36</summary>
		[UpdateDataAttribute(189, "extend_status_date36")]
		public DateTime? ExtendStatusDate36
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE36] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE36];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE36] = value; }
		}
		/// <summary>拡張ステータス37</summary>
		[UpdateDataAttribute(190, "extend_status37")]
		public string ExtendStatus37
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS37]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS37] = value; }
		}
		/// <summary>拡張ステータス更新日時37</summary>
		[UpdateDataAttribute(191, "extend_status_date37")]
		public DateTime? ExtendStatusDate37
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE37] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE37];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE37] = value; }
		}
		/// <summary>拡張ステータス38</summary>
		[UpdateDataAttribute(192, "extend_status38")]
		public string ExtendStatus38
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS38]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS38] = value; }
		}
		/// <summary>拡張ステータス更新日時38</summary>
		[UpdateDataAttribute(193, "extend_status_date38")]
		public DateTime? ExtendStatusDate38
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE38] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE38];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE38] = value; }
		}
		/// <summary>拡張ステータス39</summary>
		[UpdateDataAttribute(194, "extend_status39")]
		public string ExtendStatus39
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS39]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS39] = value; }
		}
		/// <summary>拡張ステータス更新日時39</summary>
		[UpdateDataAttribute(195, "extend_status_date39")]
		public DateTime? ExtendStatusDate39
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE39] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE39];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE39] = value; }
		}
		/// <summary>拡張ステータス40</summary>
		[UpdateDataAttribute(196, "extend_status40")]
		public string ExtendStatus40
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS40]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS40] = value; }
		}
		/// <summary>拡張ステータス更新日時40</summary>
		[UpdateDataAttribute(197, "extend_status_date40")]
		public DateTime? ExtendStatusDate40
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE40] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE40];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE40] = value; }
		}
		/// <summary>最終利用ポイント数</summary>
		[UpdateDataAttribute(198, "last_order_point_use")]
		public decimal LastOrderPointUse
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE] = value; }
		}
		/// <summary>最終ポイント利用額</summary>
		[UpdateDataAttribute(199, "last_order_point_use_yen")]
		public decimal LastOrderPointUseYen
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN]; }
			set { this.DataSource[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN] = value; }
		}
		/// <summary>外部連携受注ID</summary>
		[UpdateDataAttribute(200, "external_order_id")]
		public string ExternalOrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_ORDER_ID] = value; }
		}
		/// <summary>外部連携取込ステータス</summary>
		[UpdateDataAttribute(201, "external_import_status")]
		public string ExternalImportStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS] = value; }
		}
		/// <summary>最終与信フラグ</summary>
		[UpdateDataAttribute(202, "last_auth_flg")]
		public string LastAuthFlg
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_LAST_AUTH_FLG] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_ORDER_LAST_AUTH_FLG];
			}
			set { this.DataSource[Constants.FIELD_ORDER_LAST_AUTH_FLG] = value; }
		}
		/// <summary>モール連携ステータス</summary>
		[UpdateDataAttribute(203, "mall_link_status")]
		public string MallLinkStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_MALL_LINK_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_MALL_LINK_STATUS] = value; }
		}
		/// <summary>商品小計税額</summary>
		[UpdateDataAttribute(204, "order_price_subtotal_tax")]
		public decimal OrderPriceSubtotalTax
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX] = value; }
		}
		/// <summary>決済手数料税額</summary>
		/// V5.13より使用しない
		[UpdateDataAttribute(205, "order_price_exchange_tax")]
		public decimal OrderPriceExchangeTax
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX] = value; }
		}
		/// <summary>配送料税額</summary>
		/// V5.13より使用しない
		[UpdateDataAttribute(206, "order_price_shipping_tax")]
		public decimal OrderPriceShippingTax
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX] = value; }
		}
		/// <summary>決済通貨</summary>
		[UpdateDataAttribute(207, "settlement_currency")]
		public string SettlementCurrency
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY] = value; }
		}
		/// <summary>決済レート</summary>
		[UpdateDataAttribute(208, "settlement_rate")]
		public decimal SettlementRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_RATE] = value; }
		}
		/// <summary>決済金額</summary>
		[UpdateDataAttribute(209, "settlement_amount")]
		public decimal SettlementAmount
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT] = value; }
		}
		/// <summary>配送メモ</summary>
		[UpdateDataAttribute(210, "shipping_memo")]
		public string ShippingMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SHIPPING_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPING_MEMO] = value; }
		}
		/// <summary>外部連携決済ログ</summary>
		[UpdateDataAttribute(211, "external_payment_cooperation_log")]
		public string ExternalPaymentCooperationLog
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG] = value; }
		}
		/// <summary>配送料税率</summary>
		[UpdateDataAttribute(212, "shipping_tax_rate")]
		public decimal ShippingTaxRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_SHIPPING_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_SHIPPING_TAX_RATE] = value; }
		}
		/// <summary>決済手数料税率</summary>
		[UpdateDataAttribute(213, "payment_tax_rate")]
		public decimal PaymentTaxRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDER_PAYMENT_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDER_PAYMENT_TAX_RATE] = value; }
		}
		/// <summary>購入回数（注文時点）</summary>
		[UpdateData(214, "order_count_order")]
		public int? OrderCountOrder
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_COUNT_ORDER] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDER_ORDER_COUNT_ORDER];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_COUNT_ORDER] = value; }
		}
		/// <summary>請求書同梱フラグ</summary>
		[UpdateDataAttribute(215, "invoice_bundle_flg")]
		public string InvoiceBundleFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG] = value; }
		}
		/// <summary>流入コンテンツタイプ</summary>
		[UpdateDataAttribute(216, "inflow_contents_type")]
		public string InflowContentsType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE] = value; }
		}
		/// <summary>流入コンテンツID</summary>
		[UpdateDataAttribute(217, "inflow_contents_id")]
		public string InflowContentsId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_INFLOW_CONTENTS_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_INFLOW_CONTENTS_ID] = value; }
		}
		/// <summary>領収書希望フラグ</summary>
		[UpdateDataAttribute(218, "receipt_flg")]
		public string ReceiptFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_FLG] = value; }
		}
		/// <summary>領収書出力フラグ</summary>
		[UpdateDataAttribute(219, "receipt_output_flg")]
		public string ReceiptOutputFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG] = value; }
		}
		/// <summary>宛名</summary>
		[UpdateDataAttribute(220, "receipt_address")]
		public string ReceiptAddress
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_ADDRESS]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_ADDRESS] = value; }
		}
		/// <summary>但し書き</summary>
		[UpdateDataAttribute(221, "receipt_proviso")]
		public string ReceiptProviso
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_PROVISO]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_PROVISO] = value; }
		}
		/// <summary>物流取引ID</summary>
		[UpdateDataAttribute(222, "delivery_tran_id")]
		public string DeliveryTranId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_DELIVERY_TRAN_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_DELIVERY_TRAN_ID] = value; }
		}
		/// <summary>オンライン配信状況</summary>
		[UpdateDataAttribute(223, "online_delivery_status")]
		public string OnlineDeliveryStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_ONLINE_DELIVERY_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_ONLINE_DELIVERY_STATUS] = value; }
		}
		/// <summary>外部決済タイプ</summary>
		[UpdateDataAttribute(224, "external_payment_type")]
		public string ExternalPaymentType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE] = value; }
		}
		/// <summary>物流連携ステータス</summary>
		[UpdateDataAttribute(225, "logi_cooperation_status")]
		public string LogiCooperationStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_LOGI_COOPERATION_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_LOGI_COOPERATION_STATUS] = value; }
		}
		/// <summary>拡張ステータス41</summary>
		[UpdateDataAttribute(226, "extend_status41")]
		public string ExtendStatus41
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS41]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS41] = value; }
		}
		/// <summary>拡張ステータス更新日時41</summary>
		[UpdateDataAttribute(227, "extend_status_date41")]
		public DateTime? ExtendStatusDate41
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE41] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE41];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE41] = value; }
		}
		/// <summary>拡張ステータス42</summary>
		[UpdateDataAttribute(228, "extend_status42")]
		public string ExtendStatus42
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS42]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS42] = value; }
		}
		/// <summary>拡張ステータス更新日時42</summary>
		[UpdateDataAttribute(229, "extend_status_date42")]
		public DateTime? ExtendStatusDate42
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE42] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE42];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE42] = value; }
		}
		/// <summary>拡張ステータス43</summary>
		[UpdateDataAttribute(230, "extend_status43")]
		public string ExtendStatus43
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS43]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS43] = value; }
		}
		/// <summary>拡張ステータス更新日時43</summary>
		[UpdateDataAttribute(231, "extend_status_date43")]
		public DateTime? ExtendStatusDate43
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE43] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE43];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE43] = value; }
		}
		/// <summary>拡張ステータス44</summary>
		[UpdateDataAttribute(232, "extend_status44")]
		public string ExtendStatus44
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS44]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS44] = value; }
		}
		/// <summary>拡張ステータス更新日時44</summary>
		[UpdateDataAttribute(233, "extend_status_date44")]
		public DateTime? ExtendStatusDate44
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE44] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE44];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE44] = value; }
		}
		/// <summary>拡張ステータス45</summary>
		[UpdateDataAttribute(234, "extend_status45")]
		public string ExtendStatus45
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS45]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS45] = value; }
		}
		/// <summary>拡張ステータス更新日時45</summary>
		[UpdateDataAttribute(235, "extend_status_date45")]
		public DateTime? ExtendStatusDate45
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE45] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE45];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE45] = value; }
		}
		/// <summary>拡張ステータス46</summary>
		[UpdateDataAttribute(236, "extend_status46")]
		public string ExtendStatus46
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS46]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS46] = value; }
		}
		/// <summary>拡張ステータス更新日時46</summary>
		[UpdateDataAttribute(237, "extend_status_date46")]
		public DateTime? ExtendStatusDate46
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE46] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE46];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE46] = value; }
		}
		/// <summary>拡張ステータス47</summary>
		[UpdateDataAttribute(238, "extend_status47")]
		public string ExtendStatus47
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS47]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS47] = value; }
		}
		/// <summary>拡張ステータス更新日時47</summary>
		[UpdateDataAttribute(239, "extend_status_date47")]
		public DateTime? ExtendStatusDate47
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE47] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE47];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE47] = value; }
		}
		/// <summary>拡張ステータス48</summary>
		[UpdateDataAttribute(240, "extend_status48")]
		public string ExtendStatus48
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS48]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS48] = value; }
		}
		/// <summary>拡張ステータス更新日時48</summary>
		[UpdateDataAttribute(241, "extend_status_date48")]
		public DateTime? ExtendStatusDate48
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE48] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE48];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE48] = value; }
		}
		/// <summary>拡張ステータス49</summary>
		[UpdateDataAttribute(242, "extend_status49")]
		public string ExtendStatus49
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS49]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS49] = value; }
		}
		/// <summary>拡張ステータス更新日時49</summary>
		[UpdateDataAttribute(243, "extend_status_date49")]
		public DateTime? ExtendStatusDate49
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE49] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE49];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE49] = value; }
		}
		/// <summary>拡張ステータス50</summary>
		[UpdateDataAttribute(244, "extend_status50")]
		public string ExtendStatus50
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS50]; }
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS50] = value; }
		}
		/// <summary>拡張ステータス更新日時50</summary>
		[UpdateDataAttribute(245, "extend_status_date50")]
		public DateTime? ExtendStatusDate50
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE50] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE50];
			}
			set { this.DataSource[Constants.FIELD_ORDER_EXTEND_STATUS_DATE50] = value; }
		}
		/// <summary>決済カード取引PASS</summary>
		[UpdateDataAttribute(246, "card_tran_pass")]
		public string CardTranPass
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_CARD_TRAN_PASS]; }
			set { this.DataSource[Constants.FIELD_ORDER_CARD_TRAN_PASS] = value; }
		}
		/// <summary>頒布会コースID</summary>
		[UpdateDataAttribute(246, "subscription_box_course_id")]
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>頒布会コース定額価格</summary>
		[UpdateDataAttribute(247, "subscription_box_fixed_amount")]
		public decimal? SubscriptionBoxFixedAmount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT];
			}
			set { this.DataSource[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT] = value; }
		}
		/// <summary>頒布会購入回数</summary>
		[UpdateDataAttribute(248, "order_subscription_box_order_count")]
		public int? OrderSubscriptionBoxOrderCount
		{
			get { return (int?)this.DataSource[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT] = value; }
		}
		/// <summary>店舗受取ステータス</summary>
		[UpdateDataAttribute(249, "storepickup_status")]
		public string StorePickupStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STATUS] = value; }
		}
		/// <summary>店舗到着日時</summary>
		[UpdateDataAttribute(250, "storepickup_store_arrived_date")]
		public DateTime? StorePickupStoreArrivedDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE] = value; }
		}
		/// <summary>引渡し日時</summary>
		[UpdateDataAttribute(251, "storepickup_delivered_complete_date")]
		public DateTime? StorePickupDeliveredCompleteDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE] = value; }
		}
		/// <summary>返送日時</summary>
		[UpdateDataAttribute(252, "storepickup_return_date")]
		public DateTime? StorePickupReturnDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE] = value; }
		}
		#endregion
	}
}
