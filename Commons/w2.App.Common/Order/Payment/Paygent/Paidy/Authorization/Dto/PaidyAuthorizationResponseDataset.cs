/*
=========================================================================================================
  Module      : Paygent API Paidy決済情報検証電文 結果モデル(PaidyAuthorizationResponseDataset.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Authorization.Dto
{
	/// <summary>
	/// Paygent API Paidy決済情報検証電文 結果モデル
	/// </summary>
	[Serializable]
	public class PaidyAuthorizationResponseDataset : IPaygentResponse
	{
		/// <summary>処理結果</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_RESULT)]
		public string Result { get; set; }
		/// <summary>レスポンスコード</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CODE)]
		public string ResponseCode { get; set; }
		/// <summary>レスポンス詳細</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_DETAIL)]
		public string ResponseDetail { get; set; }
		/// <summary>決済照会結果</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_RETRIEVE_RESULT)]
		public string RetrieveResult { get; set; }
		/// <summary>決済金額</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_AMOUNT)]
		public string PaymentAmount { get; set; }
		/// <summary>通貨</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CURRENCY)]
		public string Currency { get; set; }
		/// <summary>決済の説明</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_DESCRIPTION)]
		public string Description { get; set; }
		/// <summary>店舗名</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_STORE_NAME)]
		public string StoreName { get; set; }
		/// <summary>購入者_メールアドレス</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_BUYER_EMAIL)]
		public string BuyerEmail { get; set; }
		/// <summary>購入者_氏名(漢字)</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_BUYER_NAME_KANJI)]
		public string BuyerNameKanji { get; set; }
		/// <summary>購入者_氏名(カナ)</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_BUYER_NAME_KANA)]
		public string BuyerNameKana { get; set; }
		/// <summary>購入者_電話番号</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_BUYER_PHONE)]
		public string BuyerPhone { get; set; }
		/// <summary>配送先_郵便番号</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_ZIP)]
		public string ShippingAddressZip { get; set; }
		/// <summary>配送先_都道府県</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_STATE)]
		public string ShippingAddressState { get; set; }
		/// <summary>配送先_市区町村</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_CITY)]
		public string ShippingAddressCity { get; set; }
		/// <summary>配送先_番地</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_LINE2)]
		public string ShippingAddressLine2 { get; set; }
		/// <summary> 配送先_建物名・部屋番号</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_LINE1)]
		public string ShippingAddressLine1 { get; set; }
		/// <summary>注文_消費税</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ORDER_TAX)]
		public string OrderTax { get; set; }
		/// <summary>注文_配送料</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ORDER_SHIPPING)]
		public string OrderShipping { get; set; }
		/// <summary>注文商品_ID</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ORDER_ITEMS_ID)]
		public string OrderItemsId { get; set; }
		/// <summary>注文商品_商品名</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ORDER_ITEMS_TITLE)]
		public string OrderItemsTitle { get; set; }
		/// <summary>商品_説明文</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ORDER_ITEMS_DESCRIPTION)]
		public string OrderItemsDescription { get; set; }
		/// <summary>注文商品_単価</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ORDER_ITEMS_UNIT_PRICE)]
		public string OrderItemsUnitPrice { get; set; }
		/// <summary>注文商品_個数</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ORDER_ITEMS_QUANTITY)]
		public string OrderItemsQuantity { get; set; }
	}
}

