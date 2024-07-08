/*
=========================================================================================================
  Module      : ソニーペイメントe-SCOTTE 与信削除API(EScottProcess1DeleteApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.EScott.DataSchema;
using w2.App.Common.Order.Payment.EScott.Helper;

namespace w2.App.Common.Order.Payment.EScott
{
	/// <summary>
	/// 与信削除APIクラス
	/// </summary>
	public class EScottProcess1DeleteApi
	{
		/// <summary>リクエストパラメータ</summary>
		private readonly EScottRequest m_requestParameter;

		/// <summary>
		/// トークンから売上計上API生成
		/// </summary>
		/// <param name="transactionId"></param>
		/// <param name="orderPaymentId"></param>
		/// <returns></returns>
		public static EScottProcess1DeleteApi CreateEScottMaster1DeleteApi(string transactionId, string orderPaymentId)
		{
			var api = new EScottProcess1DeleteApi(
				EScottHelper.GetTransactionDate(),
				orderPaymentId,
				string.Empty,
				string.Empty,
				EScottHelper.GetProcessId(transactionId),
				EScottHelper.GetProcessPass(transactionId),
				EScottHelper.GetKaiinId(transactionId),
				EScottHelper.GetKaiinPassFromTransactionId(transactionId));
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
		private EScottProcess1DeleteApi(
			string transactionDate,
			string merchantFree1,
			string merchantFree2,
			string merchantFree3,
			string processId,
			string processPass,
			string kaiinId,
			string kaiinPass)
		{
			m_requestParameter = new EScottRequest(Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MASTERANDPROCESSANDRECOVER_URL);

			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_PASS, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTPASSWORD);
			m_requestParameter.AddRequestParameter(EScottConstants.TRANSACTION_DATE, transactionDate);
			m_requestParameter.AddRequestParameter(EScottConstants.OPERATE_ID, EScottConstants.OPERATOR_ID_PROCESS_1DELETE);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE1, merchantFree1);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE2, merchantFree2);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE3, merchantFree3);
			m_requestParameter.AddRequestParameter(EScottConstants.PROCESS_ID, processId);
			m_requestParameter.AddRequestParameter(EScottConstants.PROCESS_PASS, processPass);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_ID, kaiinId);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_PASS, kaiinPass);
		}

		/// <summary>
		/// リクエスト実行
		/// </summary>
		/// <returns>利用額変更APIレスポンス</returns>
		public Process1DeleteResponse ExecRequest()
		{
			var requestMessage = m_requestParameter.PostHttpRequestWithResponseSplitting();
			var response = new Process1DeleteResponse(requestMessage);
			return response;
		}
	}
}
