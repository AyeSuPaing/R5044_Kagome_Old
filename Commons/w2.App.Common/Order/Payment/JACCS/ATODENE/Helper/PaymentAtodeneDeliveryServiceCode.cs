/*
=========================================================================================================
  Module      : Atodene後払い 配送サービスコード(PaymentAtodeneDeliveryServiceCode.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.Helper
{
	/// <summary>
	/// Atodene後払い 配送サービスコード
	/// </summary>
	public static class PaymentAtodeneDeliveryServiceCode
	{
		/// <summary>佐川急便</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_SAGAWA = "11";
		/// <summary>ヤマト運輸</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_YAMATO = "12";
		/// <summary>日本通運</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_NITTSU = "13";
		/// <summary>西濃運輸</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_SEINO = "14";
		/// <summary>郵便書留</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_YUSEI = "15";
		/// <summary>ゆうパック</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_YUPAKE = "16";
		/// <summary>福山通運</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_FUKUYAMA = "18";
		/// <summary>新潟運輸</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_NIIGATA = "20";
		/// <summary>名鉄運輸</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_MEITETSU = "21";
		/// <summary>信州名鉄運輸</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_SHINMEI = "23";
		/// <summary>トールエクスプレス</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_TOLLEX = "26";
		/// <summary>エコ配</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_ECO = "27";
		/// <summary>翌朝10時便</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_TOKYU = "28";
		/// <summary>トナミ運輸</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_TONAMI = "29";
		/// <summary>セイノースーパーエクスプレス</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_SEINOEX = "30";
		/// <summary>大川配送サービス</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_OKAWA = "31";
		/// <summary>プラスサービス</summary>
		public const string ATODENE_DELIVERY_SERVICE_CODE_PLUS = "32";

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
					return ATODENE_DELIVERY_SERVICE_CODE_SAGAWA;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YAMATO:
					return ATODENE_DELIVERY_SERVICE_CODE_YAMATO;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_NITTSU:
					return ATODENE_DELIVERY_SERVICE_CODE_NITTSU;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SEINO:
					return ATODENE_DELIVERY_SERVICE_CODE_SEINO;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YUSEI:
					return ATODENE_DELIVERY_SERVICE_CODE_YUSEI;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YUPAKE:
					return ATODENE_DELIVERY_SERVICE_CODE_YUPAKE;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_FUKUYAMA:
					return ATODENE_DELIVERY_SERVICE_CODE_FUKUYAMA;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_NIIGATA:
					return ATODENE_DELIVERY_SERVICE_CODE_NIIGATA;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_MEITETSU:
					return ATODENE_DELIVERY_SERVICE_CODE_MEITETSU;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SHINMEI:
					return ATODENE_DELIVERY_SERVICE_CODE_SHINMEI;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TOLLEX:
					return ATODENE_DELIVERY_SERVICE_CODE_TOLLEX;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_ECO:
					return ATODENE_DELIVERY_SERVICE_CODE_ECO;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TOKYU:
					return ATODENE_DELIVERY_SERVICE_CODE_TOKYU;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TONAMI:
					return ATODENE_DELIVERY_SERVICE_CODE_TONAMI;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SEINOEX:
					return ATODENE_DELIVERY_SERVICE_CODE_SEINOEX;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_OKAWA:
					return ATODENE_DELIVERY_SERVICE_CODE_OKAWA;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_PLUS:
					return ATODENE_DELIVERY_SERVICE_CODE_PLUS;

				default:
					return ATODENE_DELIVERY_SERVICE_CODE_SAGAWA;
			}
		}
	}
}