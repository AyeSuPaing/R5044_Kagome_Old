/*
=========================================================================================================
  Module      : e-SCOTT 会員無効API（会員無効）クラス(EScottMember4MemInvalApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.EScott.DataSchema;
using w2.App.Common.Order.Payment.EScott.Helper;

namespace w2.App.Common.Order.Payment.EScott
{
	/// <summary>
	/// 会員無効API（会員無効）クラス
	/// </summary>
	public class EScottMember4MemInvalApi
	{
		/// <summary>リクエストパラメータ</summary>
		private readonly EScottRequest m_requestParameter;

		/// <summary>
		/// 会員無効API生成
		/// </summary>
		/// <param name="cooperationId">連携ID</param>
		/// <returns>会員無効API</returns>
		public static EScottMember4MemInvalApi CreateEScottEScottMember4MemInvalApi(string cooperationId)
		{
			var api = new EScottMember4MemInvalApi(
				EScottHelper.GetTransactionDate(),
				string.Empty,
				string.Empty,
				string.Empty,
				EScottHelper.GetKaiinId(cooperationId),
				EScottHelper.GetKaiinPassFromCooperationId(cooperationId));
			return api;
		}

		/// <summary>
		/// 会員無効APIコンストラクタ
		/// </summary>
		/// <param name="tranzactionDate">取引日付</param>
		/// <param name="merchantFree1">自由領域1</param>
		/// <param name="merchantFree2">自由領域2</param>
		/// <param name="merchantFree3">自由領域3</param>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="kaiinPass">会員パスワード</param>
		private EScottMember4MemInvalApi(
			string tranzactionDate,
			string merchantFree1,
			string merchantFree2,
			string merchantFree3,
			string kaiinId,
			string kaiinPass)
		{
			m_requestParameter = new EScottRequest(Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBERREGISTER_URL);

			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_PASS, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTPASSWORD);
			m_requestParameter.AddRequestParameter(EScottConstants.TRANSACTION_DATE, tranzactionDate);
			m_requestParameter.AddRequestParameter(EScottConstants.OPERATE_ID, EScottConstants.OPERATOR_ID_MEMBER_4MEMINVAL);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE1, merchantFree1);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE2, merchantFree2);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE3, merchantFree3);
			m_requestParameter.AddRequestParameter(EScottConstants.TENANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TENANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_ID, kaiinId);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_PASS, kaiinPass);
		}

		/// <summary>
		/// リクエスト実行
		/// </summary>
		/// <returns>会員無効APIレスポンス</returns>
		public Member4MemInvalResponse ExecRequest()
		{
			var requestMessage = m_requestParameter.PostHttpRequestWithResponseSplitting();
			var response = new Member4MemInvalResponse(requestMessage);
			return response;
		}

	}
}
