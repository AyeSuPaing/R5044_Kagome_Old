/*
=========================================================================================================
  Module      : ヤマトKWC 配送サービスコード(PaymentYamatoKwcDeliveryServiceCode.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order.Payment.YamatoKwc.Helper
{
	/// <summary>
	/// ヤマトKWC 配送サービスコード
	/// </summary>
	public static class PaymentYamatoKwcDeliveryServiceCode
	{
		/// <summary>ヤマト</summary>
		public const string YAMATO_KWC_DLIVERY_SERVICE_CODE_YAMATO = "00";
		/// <summary>その他</summary>
		public const string YAMATO_KWC_DLIVERY_SERVICE_CODE_OTHER = "99";

		/// <summary>
		/// 配送サービスコードチェック
		/// </summary>
		/// <param name="deliveryServiceCode">配送サービスコード</param>
		/// <returns>ヤマトかどうか</returns>
		public static bool CheckDeliveryServiceCodeYamato(string deliveryServiceCode)
		{
			return (deliveryServiceCode == YAMATO_KWC_DLIVERY_SERVICE_CODE_YAMATO);
		}

		/// <summary>
		/// 配送サービスコード取得
		/// </summary>
		/// <param name="deliveryCompanyType">出荷連携配送会社</param>
		/// <returns>配送サービスコード</returns>
		public static string GetDeliveryServiceCode(string deliveryCompanyType)
		{
			switch (deliveryCompanyType)
			{
				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YAMATO:
					return YAMATO_KWC_DLIVERY_SERVICE_CODE_YAMATO;

				default:
					return YAMATO_KWC_DLIVERY_SERVICE_CODE_OTHER;
			}
		}
	}
}
