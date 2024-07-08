/*
=========================================================================================================
  Module      : ソニーペイメントe-SCOTTE 再与信API(EScottProcess1ReAuthApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.EScott.DataSchema;
using w2.App.Common.Order.Payment.EScott.Helper;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.EScott
{
	/// <summary>
	/// 再与信APIクラス
	/// </summary>
	public class EScottProcess1ReAuthApi
	{
		/// <summary>リクエストパラメータ</summary>
		private readonly EScottRequest m_requestParameter;

		/// <summary>
		/// 再与信API生成
		/// </summary>
		/// <param name="order">受注</param>
		/// <returns>再与信API</returns>
		public static EScottProcess1ReAuthApi CreateEScottProcess1ReAuthApi(OrderModel order)
		{
			var api = new EScottProcess1ReAuthApi(
				EScottHelper.GetTransactionDate(),
				order.PaymentOrderId,
				string.Empty,
				string.Empty,
				EScottHelper.GetProcessId(order.CardTranId),
				EScottHelper.GetProcessPass(order.CardTranId),
				EScottHelper.GetKaiinId(order.CardTranId),
				EScottHelper.GetKaiinPassFromTransactionId(order.CardTranId),
				EScottHelper.ToAmount(order.LastBilledAmount));
			return api;
		}

		/// <summary>
		/// 利用額変更APIコンストラクタ
		/// </summary>
		/// <param name="transactionDate">取引日付</param>
		/// <param name="merchantFree1">自由領域1</param>
		/// <param name="merchantFree2">自由領域2</param>
		/// <param name="merchantFree3">自由領域3</param>
		/// <param name="processId">処理番号</param>
		/// <param name="processPass">処理パスワード</param>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="kaiinPass">会員パスワード</param>
		/// <param name="amount">金額</param>
		private EScottProcess1ReAuthApi(
			string transactionDate,
			string merchantFree1,
			string merchantFree2,
			string merchantFree3,
			string processId,
			string processPass,
			string kaiinId,
			string kaiinPass,
			string amount)
		{
			m_requestParameter = new EScottRequest(Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MASTERANDPROCESSANDRECOVER_URL);

			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_PASS, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTPASSWORD);
			m_requestParameter.AddRequestParameter(EScottConstants.TRANSACTION_DATE, transactionDate);
			m_requestParameter.AddRequestParameter(EScottConstants.OPERATE_ID, EScottConstants.OPERATOR_ID_PROCESS_1REAUTH);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE1, merchantFree1);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE2, merchantFree2);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE3, merchantFree3);
			m_requestParameter.AddRequestParameter(EScottConstants.PROCESS_ID, processId);
			m_requestParameter.AddRequestParameter(EScottConstants.PROCESS_PASS, processPass);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_ID, kaiinId);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_PASS, kaiinPass);
			m_requestParameter.AddRequestParameter(EScottConstants.AMOUNT, amount);
		}

		/// <summary>
		/// リクエスト実行
		/// </summary>
		/// <returns>再与信APIレスポンス</returns>
		public Process1ReAuthResponse ExecRequest()
		{
			var requestMessage = m_requestParameter.PostHttpRequestWithResponseSplitting();
			var response = new Process1ReAuthResponse(
				requestMessage,
				m_requestParameter.GetParameter(EScottConstants.KAIIN_ID),
				m_requestParameter.GetParameter(EScottConstants.KAIIN_PASS));
			return response;
		}
	}
}
