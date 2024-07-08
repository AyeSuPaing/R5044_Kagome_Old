/*
=========================================================================================================
  Module      : ペイジェントAPI共通ヘッダ (PaygentApiHeader.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.App.Common.Order.Payment.Paygent
{
	/// <summary>
	/// ペイジェントAPI共通ヘッダ
	/// </summary>
	public class PaygentApiHeader
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiType">電文種別ID</param>
		public PaygentApiHeader(string apiType)
		{
			this.RequestParams = new Dictionary<string, string>();
			this.MerchantId = Constants.PAYMENT_PAYGENT_MERCHANTID;
			this.ConnectId = Constants.PAYMENT_PAYGENT_CONNECTID;
			this.ConnectPassword = Constants.PAYMENT_PAYGENT_CONNECTIDPASSWORD;
			this.ApiType = apiType;
			this.ApiVersion = Constants.PAYMENT_PAYGENT_API_VERSION;
			this.TradingId = string.Empty;
			this.PaymentId = string.Empty;
		}

		/// <summary> リクエストパラメータ</summary>
		public Dictionary<string, string> RequestParams { get; set; }
		/// <summary> マーチャントID</summary>
		protected string MerchantId
		{
			get { return this.RequestParams["merchant_id"]; }
			set { this.RequestParams["merchant_id"] = value; }
		}
		/// <summary> 接続ID</summary>
		protected string ConnectId
		{
			get { return this.RequestParams["connect_id"]; }
			set { this.RequestParams["connect_id"] = value; }
		}
		/// <summary> 接続パスワード</summary>
		protected string ConnectPassword
		{
			get { return this.RequestParams["connect_password"]; }
			set { this.RequestParams["connect_password"] = value; }
		}
		/// <summary> 電文種別ID</summary>
		public string ApiType
		{
			get { return this.RequestParams["telegram_kind"]; }
			private set { this.RequestParams["telegram_kind"] = value; }
		}
		/// <summary> 電文バージョン番号</summary>
		protected string ApiVersion
		{
			get { return this.RequestParams["telegram_version"]; }
			private set { this.RequestParams["telegram_version"] = value; }
		}
		/// <summary> マーチャント取引ID</summary>
		public string TradingId
		{
			get { return this.RequestParams["tranding_id"]; }
			set { this.RequestParams["tranding_id"] = value; }
		}
		/// <summary> 決済ID</summary>
		public string PaymentId
		{
			get { return this.RequestParams["payment_id"]; }
			set { this.RequestParams["payment_id"] = value; }
		}
	}
}
