/*
=========================================================================================================
  Module      : GMO後払い 配送サービスコード(PaymentGmoDeliveryServiceCode.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Helper
{
	/// <summary>
	/// GMO後払い 配送サービスコード
	/// </summary>
	public static class PaymentGmoDeliveryServiceCode
	{
		/// <summary>佐川急便</summary>
		public const string GMO_DLIVERY_SERVICE_CODE_SAGAWA = "11";
		/// <summary>ヤマト運輸</summary>
		public const string GMO_DLIVERY_SERVICE_CODE_YAMATO = "12";
		/// <summary>西濃運輸</summary>
		public const string GMO_DLIVERY_SERVICE_CODE_SEINO = "14";
		/// <summary>郵便書留</summary>
		public const string GMO_DLIVERY_SERVICE_CODE_YUSEI = "15";
		/// <summary>ゆうパック</summary>
		public const string GMO_DLIVERY_SERVICE_CODE_YUPAKE = "16";
		/// <summary>福山通運</summary>
		public const string GMO_DLIVERY_SERVICE_CODE_FUKUYAMA = "18";
		/// <summary>エコ配</summary>
		public const string GMO_DLIVERY_SERVICE_CODE_ECO = "27";
		/// <summary>翌朝10時便</summary>
		public const string GMO_DLIVERY_SERVICE_CODE_TOKYU = "28";

		/// <summary>
		/// 配送サービスコード取得
		/// </summary>
		/// <param name="deliveryCompanyType">出荷連携配送会社</param>
		/// <returns>配送サービスコード</returns>
		public static string GetDeliveryServiceCode(string deliveryCompanyType)
		{
			switch (deliveryCompanyType)
			{
				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SAGAWA:
					return GMO_DLIVERY_SERVICE_CODE_SAGAWA;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YAMATO:
					return GMO_DLIVERY_SERVICE_CODE_YAMATO;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SEINO:
					return GMO_DLIVERY_SERVICE_CODE_SEINO;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YUSEI:
					return GMO_DLIVERY_SERVICE_CODE_YUSEI;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YUPAKE:
					return GMO_DLIVERY_SERVICE_CODE_YUPAKE;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_FUKUYAMA:
					return GMO_DLIVERY_SERVICE_CODE_FUKUYAMA;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_ECO:
					return GMO_DLIVERY_SERVICE_CODE_ECO;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TOKYU:
					return GMO_DLIVERY_SERVICE_CODE_TOKYU;

				default:
					return GMO_DLIVERY_SERVICE_CODE_SAGAWA;
			}
		}
	}
}
