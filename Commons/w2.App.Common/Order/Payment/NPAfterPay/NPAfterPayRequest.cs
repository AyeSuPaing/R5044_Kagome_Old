/*
=========================================================================================================
  Module      : NP After Pay Request(NPAfterPayRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.NPAfterPay
{
	/// <summary>
	/// NP After Pay Request
	/// </summary>
	public class NPAfterPayRequest
	{
		/// <summary>Transactions</summary>
		[JsonProperty(PropertyName = "transactions")]
		public Transaction[] Transactions { get; set; }
	}

	/// <summary>
	/// Transaction
	/// </summary>
	public class Transaction
	{
		/// <summary>NP取引ID</summary>
		[JsonProperty(PropertyName = "np_transaction_id")]
		public string NpTransactionId { get; set; }
		/// <summary>運送会社コード</summary>
		[JsonProperty(PropertyName = "pd_company_code")]
		public string PdCompanyCode { get; set; }
		/// <summary>配送伝票番号</summary>
		[JsonProperty(PropertyName = "slip_no")]
		public string SlipNo { get; set; }
		/// <summary>請求書発行日</summary>
		[JsonProperty(PropertyName = "bill_issued_date")]
		public string BillIssuedDate { get; set; }
		/// <summary>加盟店取引ID</summary>
		[JsonProperty(PropertyName = "shop_transaction_id")]
		public string ShopTransactionId { get; set; }
		/// <summary>加盟店注文日</summary>
		[JsonProperty(PropertyName = "shop_order_date")]
		public string ShopOrderDate { get; set; }
		/// <summary>決済方法</summary>
		[JsonProperty(PropertyName = "settlement_type")]
		public string SettlementType { get; set; }
		/// <summary>顧客請求金額</summary>
		[JsonProperty(PropertyName = "billed_amount")]
		public int? BilledAmount { get; set; }
		/// <summary>購入者</summary>
		[JsonProperty(PropertyName = "customer")]
		public Customer Customer { get; set; }
		/// <summary>サービス提供先（配送先）</summary>
		[JsonProperty(PropertyName = "dest_customer")]
		public DestCustomer DestCustomer { get; set; }
		/// <summary>商品情報</summary>
		[JsonProperty(PropertyName = "goods")]
		public Goods[] Goods { get; set; }
	}

	/// <summary>
	/// Customer
	/// </summary>
	[JsonObject("customer")]
	public class Customer
	{
		/// <summary>氏名</summary>
		[JsonProperty(PropertyName = "customer_name")]
		public string CustomerName { get; set; }
		/// <summary>氏名（カナ）</summary>
		[JsonProperty(PropertyName = "customer_name_kana")]
		public string CustomerNameKana { get; set; }
		/// <summary>会社名</summary>
		[JsonProperty(PropertyName = "company_name")]
		public string CompanyName { get; set; }
		/// <summary>部署名</summary>
		[JsonProperty(PropertyName = "department_name")]
		public string DepartmentName { get; set; }
		/// <summary>郵便番号</summary>
		[JsonProperty(PropertyName = "zip_code")]
		public string ZipCode { get; set; }
		/// <summary>住所</summary>
		[JsonProperty(PropertyName = "address")]
		public string Address { get; set; }
		/// <summary>電話番号</summary>
		[JsonProperty(PropertyName = "tel")]
		public string Tel { get; set; }
		/// <summary>メールアドレス</summary>
		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }
	}

	/// <summary>
	/// Dest Customer
	/// </summary>
	[JsonObject("dest_customer")]
	public class DestCustomer
	{
		/// <summary>氏名</summary>
		[JsonProperty(PropertyName = "customer_name")]
		public string CustomerName { get; set; }
		/// <summary>氏名（カナ）</summary>
		[JsonProperty(PropertyName = "customer_name_kana")]
		public string CustomerNameKana { get; set; }
		/// <summary>会社名</summary>
		[JsonProperty(PropertyName = "company_name")]
		public string CompanyName { get; set; }
		/// <summary>部署名</summary>
		[JsonProperty(PropertyName = "department_name")]
		public string DepartmentName { get; set; }
		/// <summary>郵便番号</summary>
		[JsonProperty(PropertyName = "zip_code")]
		public string ZipCode { get; set; }
		/// <summary>住所</summary>
		[JsonProperty(PropertyName = "address")]
		public string Address { get; set; }
		/// <summary>電話番号</summary>
		[JsonProperty(PropertyName = "tel")]
		public string Tel { get; set; }
	}

	/// <summary>
	/// Goods
	/// </summary>
	public class Goods
	{
		/// <summary>商品バリエーションID</summary>
		public string VariationId { get; set; }
		/// <summary>商品名</summary>
		[JsonProperty(PropertyName = "goods_name")]
		public string GoodName { get; set; }
		/// <summary>商品単価</summary>
		[JsonProperty(PropertyName = "goods_price")]
		public int GoodPrice { get; set; }
		/// <summary>個数</summary>
		[JsonProperty(PropertyName = "quantity")]
		public int Quantity { get; set; }
	}
}
