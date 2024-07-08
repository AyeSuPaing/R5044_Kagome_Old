/*
=========================================================================================================
  Module      : ペイジェントカードオーソリAPI (PaygentCreditCardAuthApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.Paygent
{
	/// <summary>
	/// ペイジェントカードオーソリAPI
	/// </summary>
	public class PaygentCreditCardAuthApi : PaygentApiHeader
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaygentCreditCardAuthApi() : base(PaygentConstants.PAYGENT_APITYPE_CARD_AUTH)
		{
			this.PaymentAmount = string.Empty;
			this.CardNumber = string.Empty;
			this.CardValidTerm = string.Empty;
			this.CardConfNumber = string.Empty;
			this.PaymentClass = string.Empty;
			this.SplitCount = string.Empty;
			this.Skip3DSecure = string.Empty;
			this.UseType = string.Empty;
			this.HttpAccept = string.Empty;
			this.HttpUserAgent = string.Empty;
			this.TermUrl = string.Empty;
			this.RefTradingId = string.Empty;
			this.StockCardMode = string.Empty;
			this.CustomerId = string.Empty;
			this.CustomerCardId = string.Empty;
			this.SiteId = string.Empty;
			this.CardToken = string.Empty;
			this.SalesMode = string.Empty;
			this.SecurityCodeToken = string.Empty;
			this.SecurityCodeUse = string.Empty;
			this.AuthId = string.Empty;
		}

		///<summary> 決済金額</summary>
		public string PaymentAmount
		{
			get { return this.RequestParams["payment_amount"]; }
			set { this.RequestParams["payment_amount"] = value; }
		}
		///<summary> カード番号</summary>
		public string CardNumber
		{
			get { return this.RequestParams["card_number"]; }
			set { this.RequestParams["card_number"] = value; }
		}
		///<summary> カード有効期限</summary>
		public string CardValidTerm
		{
			get { return this.RequestParams["card_valid_term"]; }
			set { this.RequestParams["card_valid_term"] = value; }
		}
		///<summary> カード確認番号</summary>
		public string CardConfNumber
		{
			get { return this.RequestParams["card_conf_number"]; }
			set { this.RequestParams["card_conf_number"] = value; }
		}
		///<summary> 支払区分</summary>
		public string PaymentClass
		{
			get { return this.RequestParams["payment_class"]; }
			set { this.RequestParams["payment_class"] = value; }
		}
		///<summary> 分割回数</summary>
		public string SplitCount
		{
			get { return this.RequestParams["split_count"]; }
			set { this.RequestParams["split_count"] = value; }
		}
		///<summary> 3Dセキュア不要区分 不要：1</summary>
		public string Skip3DSecure
		{
			get { return this.RequestParams["3dsecure_ryaku"]; }
			set { this.RequestParams["3dsecure_ryaku"] = value; }
		}
		///<summary> 3Dセキュア利用タイプ</summary>
		public string UseType
		{
			get { return this.RequestParams["3dsecure_use_type"]; }
			set { this.RequestParams["3dsecure_use_type"] = value; }
		}
		///<summary> HttpAccept</summary>
		public string HttpAccept
		{
			get { return this.RequestParams["http_accept"]; }
			set { this.RequestParams["http_accept"] = value; }
		}
		///<summary> HttpUserAgent</summary>
		public string HttpUserAgent
		{
			get { return this.RequestParams["http_user_agent"]; }
			set { this.RequestParams["http_user_agent"] = value; }
		}
		///<summary> 3-Dセキュア戻りURL</summary>
		public string TermUrl
		{
			get { return this.RequestParams["term_url"]; }
			set { this.RequestParams["term_url"] = value; }
		}
		///<summary> 参照マーチャント取引ID</summary>
		public string RefTradingId
		{
			get { return this.RequestParams["ref_trading_id"]; }
			set { this.RequestParams["ref_trading_id"] = value; }
		}
		///<summary> カード情報お預かりモード</summary>
		public string StockCardMode
		{
			get { return this.RequestParams["stock_card_mode"]; }
			set { this.RequestParams["stock_card_mode"] = value; }
		}
		///<summary> 顧客ID</summary>
		public string CustomerId
		{
			get { return this.RequestParams["customer_id"]; }
			set { this.RequestParams["customer_id"] = value; }
		}
		///<summary> 顧客カードID</summary>
		public string CustomerCardId
		{
			get { return this.RequestParams["customer_card_id"]; }
			set { this.RequestParams["customer_card_id"] = value; }
		}
		///<summary> サイトID</summary>
		public string SiteId
		{
			get { return this.RequestParams["site_id"]; }
			set { this.RequestParams["site_id"] = value; }
		}
		///<summary> カード情報トークン</summary>
		public string CardToken
		{
			get { return this.RequestParams["card_token"]; }
			set { this.RequestParams["card_token"] = value; }
		}
		///<summary> 同時売上モード</summary>
		public string SalesMode
		{
			get { return this.RequestParams["sales_mode"]; }
			set { this.RequestParams["sales_mode"] = value; }
		}
		///<summary> セキュリティーコードトークン利用</summary>
		public string SecurityCodeToken
		{
			get { return this.RequestParams["security_code_token"]; }
			set { this.RequestParams["security_code_token"] = value; }
		}
		///<summary> セキュリティコード利用</summary>
		public string SecurityCodeUse
		{
			get { return this.RequestParams["security_code_use"]; }
			set { this.RequestParams["security_code_use"] = value; }
		}
		///<summary> 3Dセキュア認証ID</summary>
		public string AuthId
		{
			get { return this.RequestParams["3ds_auth_id"]; }
			set { this.RequestParams["3ds_auth_id"] = value; }
		}
	}
}
