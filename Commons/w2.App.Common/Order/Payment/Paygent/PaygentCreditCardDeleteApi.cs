/*
=========================================================================================================
  Module      : ペイジェントカード情報削除API (PaygentCreditCardDeleteApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.Paygent
{
	/// <summary>
	/// ペイジェントカード情報削除API
	/// </summary>
	public class PaygentCreditCardDeleteApi : PaygentApiHeader
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>	
		/// <param name="apiType">電文種別ID</param>
		public PaygentCreditCardDeleteApi() : base(PaygentConstants.PAYGENT_APITYPE_CARD_DELETE)
		{
			this.CustomerId = string.Empty;
			this.CustomerCardId = string.Empty;
			this.SiteId = string.Empty;
		}

		///<summary> 顧客ID </summary>
		public string CustomerId
		{
			get { return this.RequestParams["customer_id"]; }
			set { this.RequestParams["customer_id"] = value; }
		}
		///<summary> 顧客カードID </summary>
		public string CustomerCardId
		{
			get { return this.RequestParams["customer_card_id"]; }
			set { this.RequestParams["customer_card_id"] = value; }
		}
		///<summary> サイトID </summary>
		public string SiteId
		{
			get { return this.RequestParams["site_id"]; }
			set { this.RequestParams["site_id"] = value; }
		}
	}
}
