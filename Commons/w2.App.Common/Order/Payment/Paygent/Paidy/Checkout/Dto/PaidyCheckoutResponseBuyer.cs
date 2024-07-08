/*
=========================================================================================================
  Module      :Paidy Checkout 購入者情報モデル (PaidyAuthorizationResponseBuyer.cs)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Checkout.Dto
{
	/// <summary>
	/// Paygent API Paidy決済情報検証電文 購入者情報モデル
	/// </summary>
	public class PaidyCheckoutResponseBuyer
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="email">メールアドレス</param>
		/// <param name="name1">氏名(漢字)</param>
		/// <param name="name2">氏名(カタカナ)</param>
		/// <param name="phone">電話番号(ハイフン抜き)</param>
		/// <param name="dob">生年月日(YYY-MM-DD)</param>
		public PaidyCheckoutResponseBuyer(
			string email,
			string name1,
			string name2,
			string phone,
			string dob)
		{
			this.Email = email;
			this.Name1 = name1;
			this.Name2 = name2;
			this.Phone = phone;
			this.Dob = dob;
		}

		/// <summary>メールアドレス</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_EMAIL)]
		public string Email { get; private set; }
		/// <summary>氏名(漢字)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_NAME1)]
		public string Name1 { get; private set; }
		/// <summary>氏名(カタカナ)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_NAME2)]
		public string Name2 { get; private set; }
		/// <summary>電話番号(ハイフン抜き)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_PHONE)]
		public string Phone { get; private set; }
		/// <summary>生年月日(YYYY-MM-DD)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_DOB)]
		public string Dob { get; private set; }
	}
}
