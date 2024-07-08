/*
=========================================================================================================
  Module      : アクション結果ユニット(注文単位）(ActionResultUnit.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// アクション結果ユニット(注文単位）
	/// </summary>
	[Serializable]
	public class ActionResultUnit
	{
		/// <summary>エラーがあるか</summary>
		public bool HasError
		{
			get
			{
				return ((this.ResultOrderStatusChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultProductRealStockChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultPaymentStatusChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultExternalPaymentAction == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultDemandStatusChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultOrderExtendStatusChange.Any(s => s == OrderCommon.ResultKbn.UpdateNG))
					|| (this.ResultReturnExchangeStatusChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultRepaymentStatusChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultReceiptOutputFlgChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultMailSend == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultScheduledShippingDateAction == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultShippingDateAction == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultFixedPurchaseIsAliveChangeAction == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultFixedPurchasePaymentStatusChangeAction == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultNextShippingDateChangeAction == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultNextNextShippingDateChangeAction == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultFixedPurchaseExtendStatusChange.Any(s => (s == OrderCommon.ResultKbn.UpdateNG)))
					|| (this.ResultOrderReturnChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultOrderInvoiveStatusChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultOrderInvoiveApiChange == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultExternalOrderInfoAction == OrderCommon.ResultKbn.UpdateNG)
					|| (this.ResultStorePickupStatusChange == OrderCommon.ResultKbn.UpdateNG));
			}
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strOrderId"></param>
		public ActionResultUnit(string strOrderId)
		{
			this.OrderId = strOrderId;

			this.ResultOrderExtendStatusChange = new OrderCommon.ResultKbn[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];
			this.ResultFixedPurchaseExtendStatusChange = new OrderCommon.ResultKbn[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];
			this.ErrorMessages = new List<string>();

			Reset(OrderCommon.ResultKbn.UpdateOK, false);
		}

		/// <summary>
		/// 値リセット
		/// </summary>
		/// <param name="resultKbn">セットする値</param>
		/// <param name="hasUpdateInvoiceOK">Has Invoice Update OK</param>
		public void Reset(OrderCommon.ResultKbn resultKbn, bool hasUpdateInvoiceOK)
		{
			this.ResultOrderStatusChange = resultKbn;
			this.ResultProductRealStockChange = resultKbn;
			this.ResultPaymentStatusChange = resultKbn;
			this.ResultExternalPaymentAction = resultKbn;
			this.ResultDemandStatusChange = resultKbn;
			for (var iIndex = 0; iIndex < this.ResultOrderExtendStatusChange.Length; iIndex++)
			{
				this.ResultOrderExtendStatusChange[iIndex] = resultKbn;
			}
			this.ResultReturnExchangeStatusChange = resultKbn;
			this.ResultRepaymentStatusChange = resultKbn;
			this.ResultMailSend = resultKbn;
			this.ResultScheduledShippingDateAction = resultKbn;
			this.ResultShippingDateAction = resultKbn;
			this.ResultFixedPurchaseIsAliveChangeAction = resultKbn;
			this.ResultFixedPurchasePaymentStatusChangeAction = resultKbn;
			this.ResultNextShippingDateChangeAction = resultKbn;
			this.ResultNextNextShippingDateChangeAction = resultKbn;
			for (var index = 0; index < this.ResultFixedPurchaseExtendStatusChange.Length; index++)
			{
				this.ResultFixedPurchaseExtendStatusChange[index] = resultKbn;
			}
			this.ResultOrderReturnChange = resultKbn;
			this.ResultReceiptOutputFlgChange = resultKbn;
			if (hasUpdateInvoiceOK == false)
			{
				this.ResultOrderInvoiveStatusChange = resultKbn;
				this.ResultOrderInvoiveApiChange = resultKbn;
			}
			this.ResultExternalOrderInfoAction = resultKbn;
			this.ResultStorePickupStatusChange = resultKbn;
		}

		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>注文ステータス変更結果</summary>
		public OrderCommon.ResultKbn ResultOrderStatusChange { get; set; }
		/// <summary>在庫変更結果</summary>
		public OrderCommon.ResultKbn ResultProductRealStockChange { get; set; }
		/// <summary>決済ステータス変更結果</summary>
		public OrderCommon.ResultKbn ResultPaymentStatusChange { get; set; }
		/// <summary>外部決済結果</summary>
		public OrderCommon.ResultKbn ResultExternalPaymentAction { get; set; }
		/// <summary>督促ステータス変更結果</summary>
		public OrderCommon.ResultKbn ResultDemandStatusChange { get; set; }
		/// <summary>注文拡張ステータス変更結果</summary>
		public OrderCommon.ResultKbn[] ResultOrderExtendStatusChange { get; private set; }
		/// <summary>返品交換ステータス変更結果</summary>
		public OrderCommon.ResultKbn ResultReturnExchangeStatusChange { get; set; }
		/// <summary>次々回配送日変更結果</summary>
		public OrderCommon.ResultKbn ResultRepaymentStatusChange { get; set; }
		/// <summary>メール送信結果</summary>
		public OrderCommon.ResultKbn ResultMailSend { get; set; }
		/// <summary>出荷予定日変更結果</summary>
		public OrderCommon.ResultKbn ResultScheduledShippingDateAction { get; set; }
		/// <summary>配送希望日変更結果</summary>
		public OrderCommon.ResultKbn ResultShippingDateAction { get; set; }
		/// <summary>定期購入状態変更結果</summary>
		public OrderCommon.ResultKbn ResultFixedPurchaseIsAliveChangeAction { get; set; }
		/// <summary>定期決済ステータス変更結果</summary>
		public OrderCommon.ResultKbn ResultFixedPurchasePaymentStatusChangeAction { get; set; }
		/// <summary>次回配送日変更結果</summary>
		public OrderCommon.ResultKbn ResultNextShippingDateChangeAction { get; set; }
		/// <summary>次々回配送日変更結果</summary>
		public OrderCommon.ResultKbn ResultNextNextShippingDateChangeAction { get; set; }
		/// <summary>配送不可エリア停止変更結果</summary>
		public OrderCommon.ResultKbn ResultFixedPurchaseStopUnavailableShippingAreaChangeAction { get; set; }
		/// <summary>定期拡張ステータス変更結果</summary>
		public OrderCommon.ResultKbn[] ResultFixedPurchaseExtendStatusChange { get; private set; }
		/// <summary>Order Return Change</summary>
		public OrderCommon.ResultKbn ResultOrderReturnChange { get; set; }
		/// <summary>領収書出力フラグ変更結果</summary>
		public OrderCommon.ResultKbn ResultReceiptOutputFlgChange { get; set; }
		/// <summary>Order Invoive Status Change</summary>
		public OrderCommon.ResultKbn ResultOrderInvoiveStatusChange { get; set; }
		/// <summary>Order Invoive Api Change</summary>
		public OrderCommon.ResultKbn ResultOrderInvoiveApiChange { get; set; }
		/// <summary>Result External Order Information Action</summary>
		public OrderCommon.ResultKbn ResultExternalOrderInfoAction { get; set; }
		/// <summary>店舗受取ステータス変更結果</summary>
		public OrderCommon.ResultKbn ResultStorePickupStatusChange { get; set; }
		/// <summary>External Order Information Error Message</summary>
		public string ExternalOrderInfoErrorMessage { get; set; }
		/// <summary>エラーメッセージリスト</summary>
		public List<string> ErrorMessages { get; set; }
	}
}
