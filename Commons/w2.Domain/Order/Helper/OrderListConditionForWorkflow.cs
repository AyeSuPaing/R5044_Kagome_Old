/*
=========================================================================================================
  Module      : ワークフローで使う受注リスト検索条件(OrderListConditionForWorkflow.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Order.Helper
{
	/// <summary>
	/// ワークフローで使う受注リスト検索条件
	/// </summary>
	public class OrderListConditionForWorkflow : BaseDbMapModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderListConditionForWorkflow()
		{
			this.DataSource = new Hashtable();
			this.MemoFlg = "";
			this.Memo = "";
			this.PaymentMemoFlg = "";
			this.PaymentMemo = "";
			this.ManagementMemoFlg = "";
			this.ManagementMemo = "";
			this.ShippingMemoFlg = "";
			this.ShippingMemo = "";
			this.RelationMemoFlg = "";
			this.RelationMemo = "";
			this.UserMemoFlg = "";
			this.UserMemo = "";
			this.ProductOptionFlg = "";
			this.ProductOptionTexts = "";
			this.ProductId = "";
			this.ProductName = "";
			this.OrderId = "";
			this.UserId = "";
			this.SetpromotionId = "";
			this.NoveltyId = "";
			this.RecommendId = "";
			this.IsSelectedByWorkflow = false;
			this.OrderUpdateDateFrom = "";
			this.OrderUpdateDateTo = "";
			this.ExternalPaymentStatus = "";
			this.ExternalPaymentAuthDateFrom = "";
			this.ExternalPaymentAuthDateTo = "";
			this.IsExternalPaymentAuthDateNone = false;
			this.ShippingDateFrom = "";
			this.ShippingDateTo = "";
			this.IsShippingDate = false;
			this.ScheduledShippingDateFrom = "";
			this.ScheduledShippingDateTo = "";
			this.IsScheduledShippingDate = false;
			this.ReceiptFlg = "";
			this.InvoiceBundleFlg = "";
			this.TwInvoiceStatus = string.Empty;
			this.AnotherShippingFlag = string.Empty;
			this.ShippingStatus = string.Empty;
			this.OrderUpdateDateStatus = string.Empty;
			this.PaymentOrderId = string.Empty;
			this.CardTranId = string.Empty;
			this.ExtendStatusNo = string.Empty;
			this.ExtendStatus = string.Empty;
			this.ExtendStatusDateFrom = string.Empty;
			this.ExtendStatusDateTo = string.Empty;
			this.UpdateDateExtendStatus = string.Empty;
			this.ShippingPrefectures = string.Empty;
			this.OrderExtendName = string.Empty;
			this.OrderExtendFlg = string.Empty;
			this.OrderExtendType = string.Empty;
			this.OrderExtendLikeEscaped = string.Empty;
			this.ShippingStatusCode = string.Empty;
			this.ShippingCurrentStatus = string.Empty;
			this.SubscriptionBoxCourseId = string.Empty;
			this.SubscriptionBoxOrderCountFrom = string.Empty;
			this.SubscriptionBoxOrderCountTo = string.Empty;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Source</param>
		public OrderListConditionForWorkflow(Hashtable source)
		{
			this.DataSource = source;
			this.IsSelectedByWorkflow = true;
		}

		/// <summary>メモフラグ></summary>
		[DbMapName("memo_flg")]
		public string MemoFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MEMO_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MEMO_FLG] = value; }
		}
		/// <summary>メモ</summary>
		[DbMapName("memo")]
		public string Memo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MEMO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MEMO] = value; }
		}
		/// <summary>ペイメントメモフラグ</summary>
		[DbMapName("payment_memo_flg")]
		public string PaymentMemoFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PAYMENT_MEMO_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PAYMENT_MEMO_FLG] = value; }
		}
		/// <summary>ペイメントメモ</summary>
		[DbMapName("payment_memo")]
		public string PaymentMemo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PAYMENT_MEMO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PAYMENT_MEMO] = value; }
		}
		/// <summary>マネジメントメモフラグ</summary>
		[DbMapName("management_memo_flg")]
		public string ManagementMemoFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG] = value; }
		}
		/// <summary>マネジメントメモ</summary>
		[DbMapName("management_memo")]
		public string ManagementMemo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO] = value; }
		}
		/// <summary>配送メモフラグ</summary>
		[DbMapName("shipping_memo_flg")]
		public string ShippingMemoFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG] = value; }
		}
		/// <summary>配送メモ</summary>
		[DbMapName("shipping_memo")]
		public string ShippingMemo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO] = value; }
		}
		/// <summary>関連メモフラグ</summary>
		[DbMapName("relation_memo_flg")]
		public string RelationMemoFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RELATION_MEMO_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RELATION_MEMO_FLG] = value; }
		}
		/// <summary>関連メモ</summary>
		[DbMapName("relation_memo")]
		public string RelationMemo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RELATION_MEMO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RELATION_MEMO] = value; }
		}
		/// <summary>ユーザーメモフラグ</summary>
		[DbMapName("user_memo_flg")]
		public string UserMemoFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_MEMO_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_MEMO_FLG] = value; }
		}
		/// <summary>ユーザーメモ</summary>
		[DbMapName("user_memo")]
		public string UserMemo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_MEMO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_MEMO] = value; }
		}
		/// <summary>商品設定フラグ</summary>
		[DbMapName("product_option_flg")]
		public string ProductOptionFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_OPTION_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_OPTION_FLG] = value; }
		}
		/// <summary>商品設定</summary>
		[DbMapName("product_option_texts")]
		public string ProductOptionTexts
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_OPTION_TEXTS]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_OPTION_TEXTS] = value; }
		}
		/// <summary>商品ID</summary>
		[DbMapName("product_id")]
		public string ProductId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_ID] = value; }
		}
		/// <summary>商品名</summary>
		[DbMapName("product_name")]
		public string ProductName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_NAME]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_NAME] = value; }
		}
		/// <summary>受注ID</summary>
		[DbMapName("order_id")]
		public string OrderId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_ID] = value; }
		}
		/// <summary>ユーザーID</summary>
		[DbMapName("user_id")]
		public string UserId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_ID] = value; }
		}
		/// <summary>セットプロモーションID</summary>
		[DbMapName("setpromotion_id")]
		public string SetpromotionId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SETPROMOTION_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SETPROMOTION_ID] = value; }
		}
		/// <summary>ノベルティID</summary>
		[DbMapName("novelty_id")]
		public string NoveltyId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NOVELTY_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NOVELTY_ID] = value; }
		}
		/// <summary>レコメンドID</summary>
		[DbMapName("recommend_id")]
		public string RecommendId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RECOMMEND_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RECOMMEND_ID] = value; }
		}
		/// <summary>領収書希望フラグ</summary>
		[DbMapName("receipt_flg")]
		public string ReceiptFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RECEIPT_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RECEIPT_FLG] = value; }
		}
		/// <summary>請求書同梱フラグ</summary>
		[DbMapName("invoice_bundle_flg")]
		public string InvoiceBundleFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_INVOICE_BUNDLE_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_INVOICE_BUNDLE_FLG] = value; }
		}
		/// <summary>Tw invoice status</summary>
		[DbMapName("tw_invoice_status")]
		public string TwInvoiceStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_TW_INVOICE_STATUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_TW_INVOICE_STATUS] = value; }
		}
		/// <summary>ワークフローを選択しているか</summary>
		public bool IsSelectedByWorkflow { get; set; }
		/// <summary>注文ステータス更新日From</summary>
		public string OrderUpdateDateFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_FROM] = value; }
		}
		/// <summary>注文ステータス更新日To</summary>
		public string OrderUpdateDateTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_TO] = value; }
		}
		/// <summary>外部支払い状況</summary>
		public string ExternalPaymentStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_STATUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_STATUS] = value; }
		}
		/// <summary>外部支払い状況From</summary>
		public string ExternalPaymentAuthDateFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_FROM] = value; }
		}
		/// <summary>外部支払い状況To</summary>
		public string ExternalPaymentAuthDateTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_TO] = value; }
		}
		/// <summary>外部支払い状況がないか</summary>
		public bool IsExternalPaymentAuthDateNone
		{
			get { return Boolean.Parse(StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_NONE])); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_NONE] = value; }
		}
		/// <summary>発送日From</summary>
		public string ShippingDateFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_FROM] = value; }
		}
		/// <summary>発送日To</summary>
		public string ShippingDateTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_TO] = value; }
		}
		/// <summary>発送日があるか</summary>
		public bool IsShippingDate
		{
			get { return Boolean.Parse(StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE])); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE] = value; }
		}
		/// <summary>発送日スケジュールFrom</summary>
		public string ScheduledShippingDateFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_FROM] = value; }
		}
		/// <summary>発送日スケジュールTo</summary>
		public string ScheduledShippingDateTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_TO] = value; }
		}
		/// <summary>発送日スケジュールがあるか</summary>
		public bool IsScheduledShippingDate
		{
			get { return Boolean.Parse(StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE])); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE] = value; }
		}
		/// <summary>別出荷フラグ</summary>
		public string AnotherShippingFlag
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ANOTHER_SHIPPING_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ANOTHER_SHIPPING_FLG] = value; }
		}
		/// <summary>配送状態</summary>
		public string ShippingStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_STATUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_STATUS] = value; }
		}
		/// <summary>注文ステータス</summary>
		public string OrderUpdateDateStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_STATUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_STATUS] = value; }
		}
		/// <summary>決済注文ID</summary>
		[DbMapName("payment_order_id")]
		public string PaymentOrderId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_PAYMENT_ORDER_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_PAYMENT_ORDER_ID] = value; }
		}
		/// <summary>決済取引ID</summary>
		[DbMapName("card_tran_id")]
		public string CardTranId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_CARD_TRAN_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_CARD_TRAN_ID] = value; }
		}
		/// <summary>拡張ステータスNo</summary>
		public string ExtendStatusNo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_NO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_NO] = value; }
		}
		/// <summary>拡張ステータス</summary>
		public string ExtendStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS] = value; }
		}
		/// <summary>拡張ステータス更新日ステータスNo</summary>
		public string UpdateDateExtendStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_UPDATE_DATE_EXTEND_STATUS_NO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_UPDATE_DATE_EXTEND_STATUS_NO] = value; }
		}
		/// <summary>拡張ステータス更新日From</summary>
		public string ExtendStatusDateFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_FROM] = value; }
		}
		/// <summary>拡張ステータス更新日To</summary>
		public string ExtendStatusDateTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_TO] = value; }
		}
		/// <summary>配送先 都道府県</summary>
		public string ShippingPrefectures
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_PREFECTURE]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_PREFECTURE] = value; }
		}
		/// <summary>市区町村</summary>
		public string ShippingCity
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_CITY]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_CITY] = value; }
		}
		/// <summary>注文拡張項目 項目名</summary>
		[DbMapName("order_extend_name")]
		public string OrderExtendName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME] = value; }
		}
		/// <summary>注文拡張項目 入力有無</summary>
		[DbMapName("order_extend_flg")]
		public string OrderExtendFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG] = value; }
		}
		/// <summary>注文拡張項目 入力方式</summary>
		[DbMapName("order_extend_type")]
		public string OrderExtendType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TYPE]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TYPE] = value; }
		}
		/// <summary>注文拡張項目 入力内容</summary>
		[DbMapName("order_extend_like_escaped")]
		public string OrderExtendLikeEscaped
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT] = value; }
		}
		/// <summary>完了状態コード</summary>
		public string ShippingStatusCode
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_STATUS_CODE]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_STATUS_CODE] = value; }
		}
		/// <summary>現在の状態</summary>
		public string ShippingCurrentStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_CURRENT_STATUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_CURRENT_STATUS] = value; }
		}
		/// <summary>頒布会コースID</summary>
		[DbMapName("subscription_box_course_id")]
		public string SubscriptionBoxCourseId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COURSE_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>頒布会購入回数FROM</summary>
		public string SubscriptionBoxOrderCountFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COUNT_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_FROM] = value; }
		}
		/// <summary>頒布会購入回数TO</summary>
		public string SubscriptionBoxOrderCountTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COUNT_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_TO] = value; }
		}
		/// <summary>店舗受取ステータス</summary>
		[DbMapName("owsps")]
		public string StorePickupStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_STORE_PICKUP_STATUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_STORE_PICKUP_STATUS] = value; }
		}
		/// <summary>Data source </summary>
		public Hashtable DataSource { get; set; }
	}
}
