/*
=========================================================================================================
  Module      : e-SCOTT Master電文与信API（与信）クラス(EScottMaster1AuthApi.cs)
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
	/// Master電文与信API（与信）クラス
	/// </summary>
	public class EScottMaster1AuthApi
	{
		/// <summary>リクエストパラメータ</summary>
		private readonly EScottRequest m_requestParameter;

		/// <summary>
		/// 会員方式でe-Scott与信APIを生成
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="orderPaymentId"></param>
		/// <returns></returns>
		public static EScottMaster1AuthApi CreateEScottMaster1AuthApi(CartObject cart, string orderPaymentId)
		{
			var api = new EScottMaster1AuthApi(
				EScottHelper.GetTransactionDate(),
				orderPaymentId,
				string.Empty,
				string.Empty,
				EScottHelper.GetKaiinId(cart.Payment.UserCreditCard.CooperationId),
				EScottHelper.GetKaiinPassFromCooperationId(cart.Payment.UserCreditCard.CooperationId),
				EScottHelper.ToPayType(cart.Payment.CreditInstallmentsCode),
				EScottHelper.ToAmount(cart.PriceTotal));
			return api;
		}

		/// <summary>
		/// 他決済からe-Scott与信APIを生成
		/// </summary>
		/// <param name="order">受注</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="creditCard">クレジットカード</param>
		/// <returns>Master電文与信API</returns>
		public static EScottMaster1AuthApi CreateEScottMaster1AuthApiByTokenForReauth(
			OrderModel order,
			string paymentOrderId,
			UserCreditCard creditCard)
		{
			var api = new EScottMaster1AuthApi(
				EScottHelper.GetTransactionDate(),
				paymentOrderId,
				string.Empty,
				string.Empty,
				EScottHelper.GetKaiinId(creditCard.CooperationId),
				EScottHelper.GetKaiinPassFromCooperationId(creditCard.CooperationId),
				EScottHelper.ToPayType(order.CardInstallmentsCode),
				EScottHelper.ToAmount(order.LastBilledAmount));
			return api;
		}

		/// <summary>
		/// 与信APIコンストラクタ
		/// </summary>
		/// <param name="transactionDate">取引日付</param>
		/// <param name="merchantFree1">自由領域1</param>
		/// <param name="merchantFree2">自由領域2</param>
		/// <param name="merchantFree3">自由領域3</param>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="kaiinPass">会員パスワード</param>
		/// <param name="payType">支払い区分</param>
		/// <param name="amount">利用金額</param>
		private EScottMaster1AuthApi(
			string transactionDate,
			string merchantFree1,
			string merchantFree2,
			string merchantFree3,
			string kaiinId,
			string kaiinPass,
			string payType,
			string amount)
		{
			m_requestParameter = new EScottRequest(Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MASTERANDPROCESSANDRECOVER_URL);

			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_PASS, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTPASSWORD);
			m_requestParameter.AddRequestParameter(EScottConstants.TRANSACTION_DATE, transactionDate);
			m_requestParameter.AddRequestParameter(EScottConstants.OPERATE_ID, EScottConstants.OPERATOR_ID_MASTER_1AUTH);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE1, merchantFree1);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE2, merchantFree2);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE3, merchantFree3);
			m_requestParameter.AddRequestParameter(EScottConstants.TENANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TENANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_ID, kaiinId);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_PASS, kaiinPass);
			m_requestParameter.AddRequestParameter(EScottConstants.PAY_TYPE, payType);
			m_requestParameter.AddRequestParameter(EScottConstants.AMOUNT, amount);
		}

		/// <summary>
		/// リクエスト実行
		/// </summary>
		/// <returns>Master電文与信レスポンス</returns>
		public Master1AuthResponse ExecRequest()
		{
			var requestMessage = m_requestParameter.PostHttpRequestWithResponseSplitting();
			var response = new Master1AuthResponse(
				requestMessage,
				m_requestParameter.GetParameter(EScottConstants.KAIIN_ID),
				m_requestParameter.GetParameter(EScottConstants.KAIIN_PASS));
			return response;
		}
	}
}
