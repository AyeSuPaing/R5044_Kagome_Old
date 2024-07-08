/*
=========================================================================================================
  Module      : ソフトバンクペイメント設定クラス(PaymentSBPSSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	/// <summary>
	/// SBPS設定
	/// </summary>
	public class PaymentSBPSSetting
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiUrl">API UR</param>
		/// <param name="orderLinkUrl">注文リンクURL</param>
		/// <param name="merchantId">マーチャントID</param>
		/// <param name="serviceId">サービスID</param>
		/// <param name="hashKey">ハッシュキー</param>
		/// <param name="basicAuthenticationId">基本認証ID</param>
		/// <param name="basicAuthenticationPassword">基本認証パスワード</param>
		/// <param name="tripleDESKey">3DES暗号化の暗号化キー</param>
		/// <param name="tripleDESIV">3DES暗号化の初期化ベクトル</param>
		public PaymentSBPSSetting(
			string apiUrl,
			string orderLinkUrl,
			string merchantId,
			string serviceId,
			string hashKey,
			string basicAuthenticationId,
			string basicAuthenticationPassword,
			string tripleDESKey,
			string tripleDESIV) 
		{
			this.ApiUrl = apiUrl;
			this.OrderLinkUrl = orderLinkUrl;
			this.MerchantId = merchantId;
			this.ServiceId = serviceId;
			this.HashKey = hashKey;
			this.BasicAuthenticationId = basicAuthenticationId;
			this.BasicAuthenticationPassword = basicAuthenticationPassword;

			if ((string.IsNullOrEmpty(tripleDESKey) == false)
				&& (string.IsNullOrEmpty(tripleDESIV) == false))
			{
				this.TripleDESKeyAndIV = new KeyValuePair<string, string>(
					tripleDESKey,
					tripleDESIV);
			}
		}

		/// <summary>
		/// デフォルト設定取得
		/// </summary>
		/// <returns></returns>
		internal static PaymentSBPSSetting GetDefaultSetting()
		{
			return new PaymentSBPSSetting(
				Constants.PAYMENT_SETTING_SBPS_API_URL,
				Constants.PAYMENT_SETTING_SBPS_ORDER_LINK_URL,
				Constants.PAYMENT_SETTING_SBPS_MERCHANT_ID,
				Constants.PAYMENT_SETTING_SBPS_SERVICE_ID,
				Constants.PAYMENT_SETTING_SBPS_HASHKEY,
				Constants.PAYMENT_SETTING_SBPS_BASIC_AUTHENTICATION_ID,
				Constants.PAYMENT_SETTING_SBPS_BASIC_AUTHENTICATION_PASSWORD,
				Constants.PAYMENT_SETTING_SBPS_3DES_KEY,
				Constants.PAYMENT_SETTING_SBPS_3DES_IV);
		}

		/// <summary>API URL</summary>
		public string ApiUrl { get; private set; }
		/// <summary>注文リンク URL</summary>
		public string OrderLinkUrl { get; private set; }
		/// <summary>マーチャントID</summary>
		public string MerchantId { get; private set; }
		/// <summary>サービスID</summary>
		public string ServiceId { get; private set; }
		/// <summary>ハッシュキー</summary>
		public string HashKey { get; private set; }
		/// <summary>基本認証ID</summary>
		public string BasicAuthenticationId { get; private set; }
		/// <summary>基本認証パスワード</summary>
		public string BasicAuthenticationPassword { get; private set; }
		/// <summary>3DES暗号化のキー／値（指定無しの場合暗号化しない）</summary>
		public KeyValuePair<string, string>? TripleDESKeyAndIV { get; private set; }
	}
}
