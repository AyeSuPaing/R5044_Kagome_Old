/*
=========================================================================================================
  Module      :Paidy Checkout 配送先住所 (PaidyAuthorizationResponseShippingAddress.cs)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Checkout.Dto
{
	/// <summary>
	/// Paygent API Paidy決済情報検証電文 配送先住所
	/// </summary>
	public class PaidyCheckoutResponseShippingAddress
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="line1">建物名・部屋番号</param>
		/// <param name="line2">番地</param>
		/// <param name="city">市区町村</param>
		/// <param name="state">都道府県</param>
		/// <param name="zip">郵便番号(NNN-NNNN形式)</param>
		public PaidyCheckoutResponseShippingAddress(
			string line1,
			string line2,
			string city,
			string state,
			string zip)
		{
			this.Line1 = line1;
			this.Line2 = line2;
			this.City = city;
			this.State = state;
			this.Zip = zip;
		}

		/// <summary>建物名・部屋番号</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_LINE1)]
		public string Line1 { get; private set; }
		/// <summary>番地</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_LINE2)]
		public string Line2 { get; private set; }
		/// <summary>市区町村</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_CITY)]
		public string City { get; private set; }
		/// <summary>都道府県</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_STATE)]
		public string State { get; private set; }
		/// <summary>郵便番号(NNN-NNNN形式)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ZIP)]
		public string Zip { get; private set; }
	}
}
