/*
=========================================================================================================
  Module      : DSK後払い 配送サービスコード(PaymentDskDeferredDeliveryServiceCode.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.DSKDeferred.Helper
{
	/// <summary>
	/// DSK後払い 配送サービスコード
	/// </summary>
	public class PaymentDskDeferredDeliveryServiceCode
	{
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
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_SAGAWA;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YAMATO:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_YAMATO;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_NITTSU:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_NITTSU;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SEINO:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_SEINO;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YUSEI:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_YUSEI;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YUPAKE:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_YUPAKE;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SEINOEX:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_SEINOEX;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_FUKUYAMA:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_FUKUYAMA;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_NIIGATA:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_NIIGATA;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_MEITETSU:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_MEITETSU;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SHINMEI:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_SHINMEI;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TOLLEX:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_TOLLEX;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_ECO:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_ECO;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TOKYU:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_TOKYU;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TONAMI:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_TONAMI;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_ESURAIN:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_ESURAIN;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_HARTRANSU:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_HARTRANSU;

				case Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_OTHER:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_OTHER;

				default:
					return DSK_DEFERRED_DELIVERY_SERVICE_CODE_SAGAWA;
			}
		}

		/// <summary>佐川急便</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_SAGAWA = "11";
		/// <summary>ヤマト運輸</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_YAMATO = "12";
		/// <summary>日本通運</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_NITTSU = "13";
		/// <summary>西濃運輸</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_SEINO = "14";
		/// <summary>郵便書留</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_YUSEI = "15";
		/// <summary>ゆうパック</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_YUPAKE = "16";
		/// <summary>セイノースーパーエクスプレス</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_SEINOEX = "17";
		/// <summary>福山通運</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_FUKUYAMA = "18";
		/// <summary>新潟運輸</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_NIIGATA = "20";
		/// <summary>名鉄運輸</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_MEITETSU = "21";
		/// <summary>信州名鉄運輸</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_SHINMEI = "23";
		/// <summary>トールエクスプレスジャパン</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_TOLLEX = "26";
		/// <summary>エコ配</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_ECO = "27";
		/// <summary>翌朝 10 時便</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_TOKYU = "28";
		/// <summary>トナミ運輸</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_TONAMI = "29";
		/// <summary>エスラインギフ</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_ESURAIN = "30";
		/// <summary>ハートランス</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_HARTRANSU = "31";
		/// <summary>その他</summary>
		public const string DSK_DEFERRED_DELIVERY_SERVICE_CODE_OTHER = "99";
	}
}
