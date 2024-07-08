/*
=========================================================================================================
  Module      : ペイジェントカード情報設定API (PaygentCreditCardRegister.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.Paygent
{
	/// <summary>
	/// ペイジェントカード情報設定API
	/// </summary>
	public class PaygentCreditCardRegisterApi : PaygentApiHeader
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiType">電文種別ID</param>
		public PaygentCreditCardRegisterApi() : base(PaygentConstants.PAYGENT_APITYPE_CARD_REGISTER)
		{
			this.CustomerId = string.Empty;
			this.CardBrand = string.Empty;
			this.CardholderName = string.Empty;
			this.AddInfo1 = string.Empty;
			this.AddInfo2 = string.Empty;
			this.AddInfo3 = string.Empty;
			this.AddInfo4 = string.Empty;
			this.SiteId = string.Empty;
			this.ValidCheckFlg = "1";
			this.CardToken = string.Empty;
			this.SecurityCodeUse = null;
		}

		///<summary> 顧客ID</summary>
		public string CustomerId
		{
			get { return this.RequestParams["customer_id"]; }
			set { this.RequestParams["customer_id"] = value; }
		}
		///<summary> カードブランド</summary>
		public string CardBrand
		{
			get { return this.RequestParams["card_brand"]; }
			set { this.RequestParams["card_brand"] = value; }
		}
		///<summary> カード名義人</summary>
		public string CardholderName
		{
			get { return this.RequestParams["cardholder_name"]; }
			set { this.RequestParams["cardholder_name"] = value; }
		}
		///<summary> 補足情報1</summary>
		public string AddInfo1
		{
			get { return this.RequestParams["add_info1"]; }
			set { this.RequestParams["add_info1"] = value; }
		}
		///<summary> 補足情報2</summary>
		public string AddInfo2
		{
			get { return this.RequestParams["add_info2"]; }
			set { this.RequestParams["add_info2"] = value; }
		}
		///<summary> 補足情報3</summary>
		public string AddInfo3
		{
			get { return this.RequestParams["add_info3"]; }
			set { this.RequestParams["add_info3"] = value; }
		}
		///<summary> 補足情報4</summary>
		public string AddInfo4
		{
			get { return this.RequestParams["add_info4"]; }
			set { this.RequestParams["add_info4"] = value; }
		}
		///<summary> サイトID</summary>
		public string SiteId
		{
			get { return this.RequestParams["site_id"]; }
			set { this.RequestParams["site_id"] = value; }
		}
		///<summary> 有効性チェックフラグ 1を設定</summary>
		public string ValidCheckFlg
		{
			get { return this.RequestParams["valid_check_flg"]; }
			private set { this.RequestParams["valid_check_flg"] = value; }
		}
		///<summary> カード情報トークン</summary>
		public string CardToken
		{
			get { return this.RequestParams["card_token"]; }
			set { this.RequestParams["card_token"] = value; }
		}
		///<summary> セキュリティコード利用 null,0で利用しない、もしくは任意</summary>
		public string SecurityCodeUse
		{
			get { return this.RequestParams["security_code_use"]; }
			set { this.RequestParams["security_code_use"] = value; }
		}
	}
}
