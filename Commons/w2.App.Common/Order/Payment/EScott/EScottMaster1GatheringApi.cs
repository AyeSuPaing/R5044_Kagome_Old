/*
=========================================================================================================
  Module      : e-SCOTT Master電文与信&売り上げ計上API（与信&売り上げ計上）クラス(EScottMaster1GatheringApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.EScott.DataSchema;
using w2.App.Common.Order.Payment.EScott.Helper;

namespace w2.App.Common.Order.Payment.EScott
{
	/// <summary>
	/// Master電文与信と売り上げ計上API
	/// </summary>
	public class EScottMaster1GatheringApi
	{
		/// <summary>リクエストパラメータ</summary>
		private readonly EScottRequest m_requestParameter;

		/// <summary>
		/// Master電文与信と売り上げ計上API生成
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="installmentsCode">支払回数</param>
		/// <param name="creditCard">クレジットカード</param>
		/// <param name="saleTime">売り上げ請求日</param>
		/// <returns>Master電文与信と売り上げ計上API</returns>
		public static EScottMaster1GatheringApi CreateEScottMaster1GatheringApi(
			string paymentOrderId,
			decimal lastBilledAmount,
			string installmentsCode,
			UserCreditCard creditCard,
			DateTime saleTime)
		{
			var api = new EScottMaster1GatheringApi(
				paymentOrderId,
				string.Empty,
				string.Empty,
				EScottHelper.GetKaiinId(creditCard.CooperationId),
				EScottHelper.GetKaiinPassFromCooperationId(creditCard.CooperationId),
				EScottHelper.ToPayType(installmentsCode),
				EScottHelper.ToAmount(lastBilledAmount),
				EScottHelper.ToEScottDate(saleTime));
			return api;
		}

		/// <summary>
		/// 与信と売上計上APIコンストラクタ
		/// </summary>
		/// <param name="merchantFree1">自由領域1</param>
		/// <param name="merchantFree2">自由領域2</param>
		/// <param name="merchantFree3">自由領域3</param>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="kaiinPass">会員パスワード</param>
		/// <param name="payType">支払い区分</param>
		/// <param name="amount">利用金額</param>
		/// <param name="saleTime">トークン</param>
		private EScottMaster1GatheringApi(
			string merchantFree1,
			string merchantFree2,
			string merchantFree3,
			string kaiinId,
			string kaiinPass,
			string payType,
			string amount,
			string saleTime)
		{
			m_requestParameter = new EScottRequest(Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MASTERANDPROCESSANDRECOVER_URL);

			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_PASS, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTPASSWORD);
			m_requestParameter.AddRequestParameter(EScottConstants.OPERATE_ID, EScottConstants.OPERATOR_ID_MASTER_1GATHERING);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE1, merchantFree1);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE2, merchantFree2);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE3, merchantFree3);
			m_requestParameter.AddRequestParameter(EScottConstants.TENANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TENANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_ID, kaiinId);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_PASS, kaiinPass);
			m_requestParameter.AddRequestParameter(EScottConstants.PAY_TYPE, payType);
			m_requestParameter.AddRequestParameter(EScottConstants.AMOUNT, amount);
			m_requestParameter.AddRequestParameter(EScottConstants.TRANSACTION_DATE, saleTime);
		}

		/// <summary>
		/// リクエスト実行
		/// </summary>
		/// <returns>Master電文与信と売り上げ計上API</returns>
		public Master1GatheringResponse ExecRequest()
		{
			var requestMessage = m_requestParameter.PostHttpRequestWithResponseSplitting();
			var response = new Master1GatheringResponse(
				requestMessage,
				m_requestParameter.GetParameter(EScottConstants.KAIIN_ID),
				m_requestParameter.GetParameter(EScottConstants.KAIIN_PASS));
			return response;
		}
	}
}
