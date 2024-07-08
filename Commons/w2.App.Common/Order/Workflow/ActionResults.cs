/*
=========================================================================================================
  Module      : アクションリザルト(ActionResults.cs)
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
	/// アクション結果（セッションに格納され完了ページへ表示されます）
	/// </summary>
	[Serializable]
	public class ActionResults
	{
		/// <summary>ワークフロー名</summary>
		public string WorkflowName { get; private set; }
		/// <summary>結果一覧</summary>
		public List<ActionResultUnit> Results { get; private set; }
		/// <summary>注文ステータス更新を結果表示するか</summary>
		public bool IsDisplayUpdateOrderStatusResult { get; private set; }
		/// <summary>商品実在庫更新を結果表示するか</summary>
		public bool IsDisplayUpdateProductRealStockResult { get; private set; }
		/// <summary>入金ステータス更新を結果表示するか</summary>
		public bool IsDisplayUpdatePaymentStatusResult { get; private set; }
		/// <summary>外部決済連携を結果表示するか</summary>
		public bool IsDisplayExecExternalPaymentActionResult { get; private set; }
		/// <summary>督促ステータス更新を結果表示するか</summary>
		public bool IsDisplayUpdateDemandStatusResult { get; private set; }
		/// <summary>注文返品ステータス更新を結果表示するか</summary>
		public bool IsDisplayUpdateReturnExchangeStatusResult { get; private set; }
		/// <summary>注文返金ステータス更新を結果表示するか</summary>
		public bool IsDisplayUpdateRepaymentStatusResult { get; private set; }
		/// <summary>定期購入状況更新を結果表示するか</summary>
		public bool IsDisplayUpdateFixedPurchaseIsAliveResult { get; private set; }
		/// <summary>定期決済ステータス更新を結果表示するか</summary>
		public bool IsDisplayUpdateFixedPurchasePaymentStatusResult { get; private set; }
		/// <summary>次回配送日更新を結果表示するか</summary>
		public bool IsDisplayUpdateNextShippingDateResult { get; private set; }
		/// <summary>次々回配送日更新を結果表示するか</summary>
		public bool IsDisplayUpdateNextNextShippingDateResult { get; private set; }
		/// <summary>配送不可エリア変更を結果表示するか</summary>
		public bool IsDisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult { get; private set; }
		/// <summary>注文拡張ステータス更新を結果表示するか</summary>
		public bool[] IsDisplayUpdateOrderExtendStatusStatementResults { get; private set; }
		/// <summary>定期拡張ステータス更新を結果表示するか</summary>
		public bool[] IsDisplayUpdateFixedPurchaseExtendStatusStatementResults { get; private set; }
		/// <summary>メール送信を結果表示するか</summary>
		public bool IsDisplayMailSendResults { get; private set; }
		/// <summary>表示カラム数</summary>
		public int ResultDisplayColumns { get; private set; }
		/// <summary>出荷予定日更新を結果表示するか</summary>
		public bool DisplayUpdateScheduledShippingDateStatusResult { get; set; }
		/// <summary>配送希望日更新を結果表示するか</summary>
		public bool DisplayUpdateShippingDateStatusResult { get; set; }
		/// <summary>返品更新を結果表示するか</summary>
		public bool DisplayUpdateOrderReturnResult { get; set; }
		/// <summary>発票ステータス更新を結果表示するか</summary>
		public bool DisplayUpdateOrderInvoiceStatusResult { get; private set; }
		/// <summary>電子発票連携更新を結果表示するか</summary>
		public bool DisplayUpdateOrderInvoiceApiResult { get; private set; }
		/// <summary>Display Execute External Order Information Action Result</summary>
		public bool DisplayExecExternalOrderInfoActionResult { get; private set; }
		/// <summary>発票ステータス更新を結果表示するか</summary>
		public bool DisplayUpdateStorePickupStatusResult { get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="workflowName">ワークフロー名</param>
		/// <param name="lResults">結果一覧</param>
		/// <param name="isDisplayUpdateOrderStatusResult">注文ステータス更新を結果表示するか</param>
		/// <param name="isDisplayUpdateProductRealStockResult">商品実在庫更新を結果表示するか</param>
		/// <param name="isDisplayUpdatePaymentStatusResult">入金ステータス更新を結果表示するか</param>
		/// <param name="isDisplayExecExternalPaymentActionResult">外部決済連携を結果表示するか</param>
		/// <param name="isDisplayUpdateDemandStatusResult">督促ステータス更新を結果表示するか</param>
		/// <param name="isDisplayUpdateReturnExchangeStatusResult">注文返品ステータス更新を結果表示するか</param>
		/// <param name="isDisplayUpdateRepaymentStatusResult">注文返金ステータス更新を結果表示するか</param>
		/// <param name="isDisplayUpdateOrderExtendStatusStatementResults">注文拡張ステータス更新を結果表示するか</param>
		/// <param name="isDisplayMailSendResults">メール送信を結果表示するか</param>
		/// <param name="isDisplayUpdateScheduledShippingDateStatusResult">出荷予定日更新を結果表示するか</param>
		/// <param name="isDisplayUpdateShippingDateStatusResult">配送希望日更新を結果表示するか</param>
		/// <param name="isDisplayUpdateFixedPurchaseIsAliveResult">定期購入状況更新を結果表示するか</param>
		/// <param name="isDisplayUpdateFixedPurchasePaymentStatusResult">定期決済ステータス更新を結果表示するか</param>
		/// <param name="isDisplayUpdateNextShippingDateResult">次回配送日更新を結果表示するか</param>
		/// <param name="isDisplayUpdateNextNextShippingDateResult">次々回配送日更新を結果表示するか</param>
		/// <param name="isDisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult">配送不可エリア変更を結果表示するか</param>
		/// <param name="isDisplayUpdateFixedPurchaseExtendStatusStatementResults">定期拡張ステータス更新を結果表示するか</param>
		/// <param name="displayUpdateOrderReturnResult">返品更新を結果表示するか</param>
		/// <param name="displayUpdateOrderInvoiceStatusResult">発票ステータス更新を結果表示するか</param>
		/// <param name="displayUpdateOrderInvoiceApiResult">電子発票連携更新を結果表示するか</param>
		/// <param name="displayExecExternalOrderInfoActionResult">Display Execute External Order Information Action Result</param>
		/// <param name="displayUpdateStorePickupStatusResult">店舗受取ステータス更新を結果表示するか</param>
		public ActionResults(
			string workflowName,
			List<ActionResultUnit> lResults,
			bool isDisplayUpdateOrderStatusResult,
			bool isDisplayUpdateProductRealStockResult,
			bool isDisplayUpdatePaymentStatusResult,
			bool isDisplayExecExternalPaymentActionResult,
			bool isDisplayUpdateDemandStatusResult,
			bool isDisplayUpdateReturnExchangeStatusResult,
			bool isDisplayUpdateRepaymentStatusResult,
			bool[] isDisplayUpdateOrderExtendStatusStatementResults,
			bool isDisplayMailSendResults,
			bool isDisplayUpdateScheduledShippingDateStatusResult,
			bool isDisplayUpdateShippingDateStatusResult,
			bool isDisplayUpdateFixedPurchaseIsAliveResult,
			bool isDisplayUpdateFixedPurchasePaymentStatusResult,
			bool isDisplayUpdateNextShippingDateResult,
			bool isDisplayUpdateNextNextShippingDateResult,
			bool isDisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult,
			bool[] isDisplayUpdateFixedPurchaseExtendStatusStatementResults,
			bool displayUpdateOrderReturnResult,
			bool displayUpdateOrderInvoiceStatusResult,
			bool displayUpdateOrderInvoiceApiResult,
			bool displayExecExternalOrderInfoActionResult,
			bool displayUpdateStorePickupStatusResult)
		{
			this.WorkflowName = workflowName;
			this.Results = lResults;
			this.IsDisplayUpdateOrderStatusResult = isDisplayUpdateOrderStatusResult;
			this.IsDisplayUpdateProductRealStockResult = isDisplayUpdateProductRealStockResult;
			this.IsDisplayUpdatePaymentStatusResult = isDisplayUpdatePaymentStatusResult;
			this.IsDisplayExecExternalPaymentActionResult = isDisplayExecExternalPaymentActionResult;
			this.IsDisplayUpdateDemandStatusResult = isDisplayUpdateDemandStatusResult;
			this.IsDisplayUpdateReturnExchangeStatusResult = isDisplayUpdateReturnExchangeStatusResult;
			this.IsDisplayUpdateRepaymentStatusResult = isDisplayUpdateRepaymentStatusResult;
			this.IsDisplayUpdateOrderExtendStatusStatementResults = isDisplayUpdateOrderExtendStatusStatementResults;
			this.IsDisplayMailSendResults = isDisplayMailSendResults;
			this.DisplayUpdateScheduledShippingDateStatusResult = isDisplayUpdateScheduledShippingDateStatusResult;
			this.DisplayUpdateShippingDateStatusResult = isDisplayUpdateShippingDateStatusResult;
			this.IsDisplayUpdateFixedPurchaseIsAliveResult = isDisplayUpdateFixedPurchaseIsAliveResult;
			this.IsDisplayUpdateFixedPurchasePaymentStatusResult = isDisplayUpdateFixedPurchasePaymentStatusResult;
			this.IsDisplayUpdateNextShippingDateResult = isDisplayUpdateNextShippingDateResult;
			this.IsDisplayUpdateNextNextShippingDateResult = isDisplayUpdateNextNextShippingDateResult;
			this.IsDisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult = isDisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult;
			this.IsDisplayUpdateFixedPurchaseExtendStatusStatementResults = isDisplayUpdateFixedPurchaseExtendStatusStatementResults;
			this.DisplayUpdateOrderReturnResult = displayUpdateOrderReturnResult;
			this.DisplayUpdateStorePickupStatusResult = displayUpdateStorePickupStatusResult;

			// Action Result: Invoice Api
			this.DisplayUpdateOrderInvoiceApiResult = displayUpdateOrderInvoiceApiResult;

			// Action Result: Invoice Status
			this.DisplayUpdateOrderInvoiceStatusResult = displayUpdateOrderInvoiceStatusResult;

			this.DisplayExecExternalOrderInfoActionResult = displayExecExternalOrderInfoActionResult;

			this.ResultDisplayColumns += (this.IsDisplayUpdateOrderStatusResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayUpdateProductRealStockResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayUpdatePaymentStatusResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayExecExternalPaymentActionResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayUpdateDemandStatusResult ? 1 : 0);
			foreach (var blResult in this.IsDisplayUpdateOrderExtendStatusStatementResults)
			{
				this.ResultDisplayColumns += (blResult ? 1 : 0);
			}
			this.ResultDisplayColumns += (this.IsDisplayUpdateReturnExchangeStatusResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayUpdateRepaymentStatusResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayMailSendResults ? 1 : 0);
			this.ResultDisplayColumns += (this.DisplayUpdateScheduledShippingDateStatusResult ? 1 : 0);
			this.ResultDisplayColumns += (this.DisplayUpdateShippingDateStatusResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayUpdateFixedPurchaseIsAliveResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayUpdateFixedPurchasePaymentStatusResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayUpdateNextShippingDateResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayUpdateNextNextShippingDateResult ? 1 : 0);
			this.ResultDisplayColumns += (this.IsDisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult ? 1 : 0);
			this.ResultDisplayColumns
				+= this.IsDisplayUpdateFixedPurchaseExtendStatusStatementResults.Sum(result => (result ? 1 : 0));
			this.ResultDisplayColumns += (this.DisplayUpdateOrderReturnResult ? 1 : 0);
			this.ResultDisplayColumns += (this.DisplayUpdateOrderInvoiceApiResult ? 1 : 0);
			this.ResultDisplayColumns += (this.DisplayUpdateOrderInvoiceStatusResult ? 1 : 0);
			this.ResultDisplayColumns += (this.DisplayUpdateStorePickupStatusResult ? 1 : 0);
		}
	}
}
