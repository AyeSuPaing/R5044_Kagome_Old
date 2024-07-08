/*
=========================================================================================================
  Module      : PaypayAPIインボーカ (PaypayGmoApiInvoker.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using w2.App.Common.Order.Payment.Paypay.Request;
using w2.App.Common.Order.Payment.Paypay.Response;
using w2.Common.Logger;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// PaypayAPIインボーカ
	/// </summary>
	public class PaypayGmoApiInvoker
	{
		/// <summary>
		/// 取引登録
		/// </summary>
		/// <param name="req">取引登録リクエスト</param>
		/// <param name="isFixedPurchase">Is fixed purchase</param>
		/// <returns>取引登録結果</returns>
		public EntryTranResponse EntryTran(EntryTranRequest req, bool isFixedPurchase = false)
		{
			var result = Call<EntryTranResponse>(
				ApiType.Json,
				isFixedPurchase
					? Constants.PAYMENT_PAYPAY_ENTRY_TRAN_ACCEPT_API
					: Constants.PAYMENT_PAYPAY_ENTRY_TRAN_API,
				req);

			return result;
		}

		/// <summary>
		/// 決済実行
		/// </summary>
		/// <param name="req">決済実行リクエスト</param>
		/// <param name="isFixedPurchase">Is fixed purchase</param>
		/// <returns>決済実行結果</returns>
		public ExecTranResponse ExecTran(ExecTranRequest req, bool isFixedPurchase = false)
		{
			var result = Call<ExecTranResponse>(
				ApiType.Json,
				isFixedPurchase
					? Constants.PAYMENT_PAYPAY_EXEC_ACCEPT_API
					: Constants.PAYMENT_PAYPAY_EXEC_API,
				req);
			return result;
		}

		/// <summary>
		/// 実売上
		/// </summary>
		/// <param name="req">実売上リクエスト</param>
		/// <returns>実売上結果</returns>
		public SalesPaymentResponse SalesPayment(SalesPaymentRequest req)
		{
			var result = Call<SalesPaymentResponse>(
				ApiType.Json,
				Constants.PAYMENT_PAYPAY_SALES_API,
				req);
			return result;
		}

		/// <summary>
		/// キャンセル
		/// </summary>
		/// <param name="req">キャンセルリクエスト</param>
		/// <returns>キャンセル結果</returns>
		public CancelReturnPaymentResponse CancelReturnPayment(CancelReturnPaymentRequest req)
		{
			var result = Call<CancelReturnPaymentResponse>(
				ApiType.Json,
				Constants.PAYMENT_PAYPAY_CANCEL_RETURN_API,
				req);
			return result;
		}

		/// <summary>
		/// 取引照会
		/// </summary>
		/// <param name="req">取引照会リクエスト</param>
		/// <returns>取引照会結果</returns>
		public SearchTradeResponse SearchTrade(SearchTradeRequest req)
		{
			var result = Call<SearchTradeResponse>(
				ApiType.IdPass,
				Constants.PAYMENT_PAYPAY_SEARCH_TRADE_MULTI_API,
				req);
			return result;
		}

		/// <summary>
		/// Accept end payment
		/// </summary>
		/// <param name="request">Accept end payment request</param>
		/// <returns>Accept end payment response</returns>
		public AcceptEndPaymentResponse AcceptEndPayment(AcceptEndPaymentRequest request)
		{
			var result = Call<AcceptEndPaymentResponse>(
				ApiType.Json,
				Constants.PAYMENT_PAYPAY_EXEC_ACCEPT_END_API,
				request);
			return result;
		}

		/// <summary>
		/// APIコール
		/// </summary>
		/// <returns>結果</returns>
		protected T Call<T>(
			ApiType apiType,
			string url,
			IHttpApiRequestData data)
			where T : IHttpApiResponseData
		{
			var result = CreateApiConnector(apiType).Post<T>(url, data);
			return result;
		}

		/// <summary>
		/// APIコネクタ作成
		/// </summary>
		/// <param name="apiType">API種別</param>
		/// <returns>APIコネクタ</returns>
		private ApiConnector CreateApiConnector(ApiType apiType)
		{
			switch (apiType)
			{
				case ApiType.Json:
					return new PaypayJsonApiConnector
					{
						LogAction = LogAction
					};

				case ApiType.IdPass:
					return new PaypayIdPassApiConnector
					{
						LogAction = LogAction
					};

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// ログ書き込み
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="request">リクエスト</param>
		/// <param name="response">レスポンス</param>
		/// <param name="statusCode">ステータスコード</param>
		private void LogAction(string url, string request, string response, HttpStatusCode statusCode)
		{
			FileLogger.Write(
				"Paypay",
				string.Format(
					"url:{0} status:{1:D3} request:{2} response:{3}",
					url,
					(int)statusCode,
					request,
					response));
		}
	}
}
