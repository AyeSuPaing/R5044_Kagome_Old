/*
=========================================================================================================
  Module      : e-SCOTT 会員登録情報POSTAPI（会員登録情報POST）クラス(EScottMemberInfoPostApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.EScott.DataSchema;
using w2.App.Common.Order.Payment.EScott.Helper;

namespace w2.App.Common.Order.Payment.EScott
{
	/// <summary>
	/// 会員登録情報POSTAPI（会員登録情報POST）クラス
	/// </summary>
	public class EScottMemberInfoPostApi
	{
		/// <summary>リクエストパラメータ</summary>
		private readonly EScottRequest m_requestParameter;

		/// <summary>
		/// 会員参照API生成
		/// </summary>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="url">POST先URL</param>
		/// <returns>会員参照API</returns>
		public static EScottMemberInfoPostApi CreateEScottEScottMemberInfoPostApi(string kaiinId, string url)
		{
			var api = new EScottMemberInfoPostApi(
				EScottHelper.GetTransactionDate(),
				string.Empty,
				string.Empty,
				string.Empty,
				kaiinId,
				url);
			return api;
		}

		/// <summary>
		/// 会員参照APIコンストラクタ
		/// </summary>
		/// <param name="tranzactionDate">取引日付</param>
		/// <param name="merchantFree1">自由領域1</param>
		/// <param name="merchantFree2">自由領域2</param>
		/// <param name="merchantFree3">自由領域3</param>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="url">POST先URL</param>
		private EScottMemberInfoPostApi(
			string tranzactionDate,
			string merchantFree1,
			string merchantFree2,
			string merchantFree3,
			string kaiinId,
			string url)
		{
			m_requestParameter = new EScottRequest(url);

			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.TRANSACTION_DATE, tranzactionDate);
			m_requestParameter.AddRequestParameter(EScottConstants.OPERATE_ID, EScottConstants.OPERATOR_ID_MEMBER_4MEMADD);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE1, merchantFree1);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE2, merchantFree2);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE3, merchantFree3);
			m_requestParameter.AddRequestParameter(EScottConstants.RESPONSE_CD, EScottConstants.REQUEST_APPROVED);
			m_requestParameter.AddRequestParameter(EScottConstants.TENANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TENANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_ID, kaiinId);
		}

		/// <summary>
		/// リクエスト実行
		/// </summary>
		/// <returns>会員参照APIレスポンス</returns>
		public string ExecRequest()
		{
			var responseMessage = m_requestParameter.PostHttpRequestWithoutResponseSplitting();
			return responseMessage;
		}
	}
}
