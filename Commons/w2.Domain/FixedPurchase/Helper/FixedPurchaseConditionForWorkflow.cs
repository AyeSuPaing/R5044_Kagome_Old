/*
=========================================================================================================
  Module      : Fixed purchase condition for workflow(FixedPurchaseConditionForWorkflow.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Util;
using w2.Domain.Helper;

namespace w2.Domain.FixedPurchase.Helper
{
	/// <summary>
	/// Fixed purchase condition for workflow
	/// </summary>
	public class FixedPurchaseConditionForWorkflow : BaseDbMapModel
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public FixedPurchaseConditionForWorkflow()
		{
			this.DataSource = new Hashtable();
			this.MemoFlg = string.Empty;
			this.Memo = string.Empty;
			this.ShippingMemoFlg = string.Empty;
			this.ShippingMemo = string.Empty;
			this.ReceiptFlg = string.Empty;
			this.FixedPurchaseId= string.Empty;
			this.ProductId = string.Empty;
			this.ProductName = string.Empty;
			this.UserId = string.Empty;
			this.FixedPurchaseStatus = string.Empty;
			this.FixedPurchasePaymentStatus = string.Empty;
			this.OrderedCountFrom = string.Empty;
			this.OrderedCountTo = string.Empty;
			this.ShippedCountFrom = string.Empty;
			this.ShippedCountTo = string.Empty;
			this.DateCreatedFrom = string.Empty;
			this.DateCreatedTo = string.Empty;
			this.DateChangedFrom = string.Empty;
			this.DateChangedTo = string.Empty;
			this.DateLastOrderedFrom = string.Empty;
			this.DateLastOrderedTo = string.Empty;
			this.DateBgnFrom = string.Empty;
			this.DateBgnTo = string.Empty;
			this.DateNextShippingFrom = string.Empty;
			this.DateNextShippingTo = string.Empty;
			this.DateNextNextShippingFrom = string.Empty;
			this.DateNextNextShippingTo = string.Empty;
			this.FixedPurchaseExtendStatusNo = string.Empty;
			this.FixedPurchaseExtendStatus = string.Empty;
			this.FixedPurchaseExtendStatusNoUpdateDate = string.Empty;
			this.FixedPurchaseExtendStatusForm = string.Empty;
			this.FixedPurchaseExtendStatusTo = string.Empty;
			this.IsSelectedByWorkflow = false;
			this.OrderExtendName = string.Empty;
			this.OrderExtendFlg = string.Empty;
			this.OrderExtendType = string.Empty;
			this.OrderExtendLikeEscaped = string.Empty;
			this.SubscriptionBoxCourseId = string.Empty;
			this.SubscriptionBoxOrderCountFrom = string.Empty;
			this.SubscriptionBoxOrderCountTo = string.Empty;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Source</param>
		public FixedPurchaseConditionForWorkflow(Hashtable source)
		{
			this.DataSource = source;
			this.IsSelectedByWorkflow = true;
		}
		#endregion

		#region Properties
		/// <summary>Memo flag</summary>
		public string MemoFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG] = value; }
		}
		/// <summary>Memo</summary>
		public string Memo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO] = value; }
		}
		/// <summary>Shipping memo flag</summary>
		public string ShippingMemoFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG] = value; }
		}
		/// <summary>Shipping memo</summary>
		public string ShippingMemo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO] = value; }
		}
		/// <summary>Receipt flag</summary>
		public string ReceiptFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_RECEIPT_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_RECEIPT_FLG] = value; }
		}
		/// <summary>Fixed purchase id</summary>
		public string FixedPurchaseId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_ID] = value; }
		}
		/// <summary>Product id</summary>
		public string ProductId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PRODUCT_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PRODUCT_ID] = value; }
		}
		/// <summary>Product name</summary>
		public string ProductName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PRODUCT_NAME]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PRODUCT_NAME] = value; }
		}
		/// <summary>User id</summary>
		public string UserId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_USER_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_USER_ID] = value; }
		}
		/// <summary>Fixed purchase status</summary>
		public string FixedPurchaseStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_STAUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_STAUS] = value; }
		}
		/// <summary>Fixed purchase payment status</summary>
		public string FixedPurchasePaymentStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PAYMENT_STAUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PAYMENT_STAUS] = value; }
		}
		/// <summary>Ordered count from</summary>
		public string OrderedCountFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDERED_COUNT_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDERED_COUNT_FROM] = value; }
		}
		/// <summary>Ordered count to</summary>
		public string OrderedCountTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDERED_COUNT_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDERED_COUNT_TO] = value; }
		}
		/// <summary>Shipped count from</summary>
		public string ShippedCountFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPED_COUNT_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPED_COUNT_FROM] = value; }
		}
		/// <summary>Shipped count to</summary>
		public string ShippedCountTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPED_COUNT_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPED_COUNT_TO] = value; }
		}
		/// <summary>Date created from</summary>
		public string DateCreatedFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CREATED_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CREATED_FROM] = value; }
		}
		/// <summary>Date created to</summary>
		public string DateCreatedTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CREATED_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CREATED_TO] = value; }
		}
		/// <summary>Date changed from</summary>
		public string DateChangedFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CHANGED_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CHANGED_FROM] = value; }
		}
		/// <summary>Date changed to</summary>
		public string DateChangedTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CHANGED_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CHANGED_TO] = value; }
		}
		/// <summary>Date last ordered from</summary>
		public string DateLastOrderedFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_LAST_ORDERED_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_LAST_ORDERED_FROM] = value; }
		}
		/// <summary>Date last ordered to</summary>
		public string DateLastOrderedTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_LAST_ORDERED_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_LAST_ORDERED_TO] = value; }
		}
		/// <summary>Date begin from</summary>
		public string DateBgnFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_BGN_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_BGN_FROM] = value; }
		}
		/// <summary>Date begin to</summary>
		public string DateBgnTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_BGN_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_BGN_TO] = value; }
		}
		/// <summary>Date next shipping from</summary>
		public string DateNextShippingFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_SHIPPING_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_SHIPPING_FROM] = value; }
		}
		/// <summary>Date next shipping to</summary>
		public string DateNextShippingTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_SHIPPING_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_SHIPPING_TO] = value; }
		}
		/// <summary>Date next next shipping from</summary>
		public string DateNextNextShippingFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_NEXT_SHIPPING_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_NEXT_SHIPPING_FROM] = value; }
		}
		/// <summary>Date next next shipping to</summary>
		public string DateNextNextShippingTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_NEXT_SHIPPING_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_NEXT_SHIPPING_TO] = value; }
		}
		/// <summary>Fixed purchase extend status no</summary>
		public string FixedPurchaseExtendStatusNo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO] = value; }
		}
		/// <summary>Fixed purchase extend status</summary>
		public string FixedPurchaseExtendStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS] = value; }
		}
		/// <summary>Fixed purchase extend status no update date</summary>
		public string FixedPurchaseExtendStatusNoUpdateDate
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO_UPDATE_DATE]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO_UPDATE_DATE] = value; }
		}
		/// <summary>Fixed purchase extend status from</summary>
		public string FixedPurchaseExtendStatusForm
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_FROM] = value; }
		}
		/// <summary>Fixed purchase extend status to</summary>
		public string FixedPurchaseExtendStatusTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_TO] = value; }
		}
		/// <summary>ワークフローを選択しているか</summary>
		public bool IsSelectedByWorkflow { get; set; }
		/// <summary>注文拡張項目 項目名</summary>
		public string OrderExtendName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME] = value; }
		}
		/// <summary>注文拡張項目 入力有無</summary>
		public string OrderExtendFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG] = value; }
		}
		/// <summary>注文拡張項目 入力方式</summary>
		public string OrderExtendType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TYPE]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TYPE] = value; }
		}
		/// <summary>注文拡張項目 入力内容</summary>
		public string OrderExtendLikeEscaped
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT] = value; }
		}
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COURSE_ID]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>頒布会購入回数FROM</summary>
		public string SubscriptionBoxOrderCountFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_FROM]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_FROM] = value; }
		}
		/// <summary>頒布会購入回数TO</summary>
		public string SubscriptionBoxOrderCountTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_TO]); }
			set { this.DataSource[Constants.REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_TO] = value; }
		}
		/// <summary>Data Source</summary>
		public Hashtable DataSource { get; set; }
		#endregion
	}
}
