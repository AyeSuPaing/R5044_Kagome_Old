/*
=========================================================================================================
  Module      : e-SCOTT 会員登録API（会員登録）クラス(EScottMember4MemAddApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.EScott.DataSchema;
using w2.App.Common.Order.Payment.EScott.Helper;

namespace w2.App.Common.Order.Payment.EScott
{
	/// <summary>
	/// 会員登録API（会員登録）クラス
	/// </summary>
	public class EScottMember4MemAddApi
	{
		/// <summary>リクエストパラメータ</summary>
		private readonly EScottRequest m_requestParameter;

		/// <summary>
		/// クレジットカードから会員登録API生成
		/// </summary>
		/// <param name="creditCard">クレジットカード</param>
		/// <param name="token">トークン</param>
		/// <returns>会員登録API</returns>
		public static EScottMember4MemAddApi CreateEScottMember4MemAddApiByCreditCard(
			UserCreditCard creditCard,
			string token)
		{
			var api = new EScottMember4MemAddApi(
				EScottHelper.GetTransactionDate(),
				string.Empty,
				string.Empty,
				string.Empty,
				EScottHelper.GetKaiinId(creditCard.CooperationId),
				EScottHelper.GetKaiinPassFromCooperationId(creditCard.CooperationId),
				token);
			return api;
		}

		/// <summary>
		/// カートから会員登録API生成
		/// </summary>
		/// <param name="cart">カート</param>
		/// <returns>会員登録API</returns>
		public static EScottMember4MemAddApi CreateEScottMember4MemAddApiByCart(CartObject cart)
		{
			var api = new EScottMember4MemAddApi(
				EScottHelper.GetTransactionDate(),
				string.Empty,
				string.Empty,
				string.Empty,
				EScottHelper.GetKaiinId(cart.Payment.UserCreditCard.CooperationId),
				EScottHelper.GetKaiinPassFromCooperationId(cart.Payment.UserCreditCard.CooperationId),
				cart.Payment.CreditToken.ToString());
			return api;
		}

		/// <summary>
		/// 会員IDから会員登録API生成
		/// </summary>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="kaiinPass">会員パスワード</param>
		/// <param name="token">トークン</param>
		/// <returns>会員登録API</returns>
		public static EScottMember4MemAddApi CreateEScottMember4MemAddApiByKaiinId(
			string kaiinId,
			string kaiinPass,
			string token)
		{
			var api = new EScottMember4MemAddApi(
				EScottHelper.GetTransactionDate(),
				string.Empty,
				string.Empty,
				string.Empty,
				kaiinId,
				kaiinPass,
				token);
			return api;
		}

		/// <summary>
		/// 会員登録APIコンストラクタ
		/// </summary>
		/// <param name="tranzactionDate">取引日付</param>
		/// <param name="merchantFree1">自由領域1</param>
		/// <param name="merchantFree2">自由領域2</param>
		/// <param name="merchantFree3">自由領域3</param>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="kaiinPass">会員パスワード</param>
		/// <param name="token">トークン</param>
		private EScottMember4MemAddApi(
			string tranzactionDate,
			string merchantFree1,
			string merchantFree2,
			string merchantFree3,
			string kaiinId,
			string kaiinPass,
			string token)
		{
			m_requestParameter = new EScottRequest(Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBERREGISTER_URL);

			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_PASS, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTPASSWORD);
			m_requestParameter.AddRequestParameter(EScottConstants.TRANSACTION_DATE, tranzactionDate);
			m_requestParameter.AddRequestParameter(EScottConstants.OPERATE_ID, EScottConstants.OPERATOR_ID_MEMBER_4MEMADD);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE1, merchantFree1);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE2, merchantFree2);
			m_requestParameter.AddRequestParameter(EScottConstants.MERCHANT_FREE3, merchantFree3);
			m_requestParameter.AddRequestParameter(EScottConstants.TENANT_ID, Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TENANTID);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_ID, kaiinId);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_PASS, kaiinPass);
			m_requestParameter.AddRequestParameter(EScottConstants.TOKEN, token);
			m_requestParameter.AddRequestParameter(EScottConstants.KAIIN_ID_AUTO_RIYO_FLG, EScottConstants.FLG_KAIIN_ID_AUTO_RIYO_FLG_OFF);
		}

		/// <summary>
		/// リクエスト実行
		/// </summary>
		/// <returns>会員登録APIレスポンス</returns>
		public Member4MemAddResponse ExecRequest()
		{
			var requestMessage = m_requestParameter.PostHttpRequestWithResponseSplitting();
			var response = new Member4MemAddResponse(requestMessage);
			return response;
		}
	}
}
