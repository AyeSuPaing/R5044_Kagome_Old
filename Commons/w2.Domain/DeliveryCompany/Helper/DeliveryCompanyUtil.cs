/*
=========================================================================================================
  Module      : 配送会社設定共通処理クラス(DeliveryCompanyUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.DeliveryCompany.Helper
{
	/// <summary>
	/// 配送会社設定ユーティリティ
	/// </summary>
	public class DeliveryCompanyUtil
	{
		/// <summary>
		/// 出荷連携配送会社取得
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="orderPaymentKbn">注文決済区分</param>
		/// <returns>出荷連携配送会社</returns>
		public static string GetDeliveryCompanyType(string deliveryCompanyId, string orderPaymentKbn)
		{
			var deliveryCompany = new DeliveryCompanyService().Get(deliveryCompanyId);

			var result = (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				? deliveryCompany.DeliveryCompanyTypeCreditcard
				: (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					? deliveryCompany.DeliveryCompanyTypePostNpPayment
					: (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
						? deliveryCompany.DeliveryCompanyTypeGmoAtokaraPayment
						: deliveryCompany.DeliveryCompanyTypePostPayment;

			return result;
		}
	}
}
