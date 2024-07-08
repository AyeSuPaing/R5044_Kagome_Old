/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API 支払方法モデル (RakutenApiSettlement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API 支払方法モデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_ORDER_MODEL_SETTLEMENT_MODEL)]
	public class RakutenApiSettlement
	{
		/// <summary>支払方法コード</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SETTLEMENT_MODEL_SETTLEMENT_METHOD_CODE)]
		public int? SettlementMethodCode { get; set; }
		/// <summary>支払方法名</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SETTLEMENT_MODEL_SETTLEMENT_METHOD)]
		public string SettlementMethod { get; set; }
		/// <summary>楽天市場の共通決済手段フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SETTLEMENT_MODEL_RPAY_SETTLEMENT_FLAG)]
		public int? RpaySettlementFlag { get; set; }
		/// <summary>クレジットカード種類</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_NAME)]
		public string CardName { get; set; }
		/// <summary>クレジットカード番号</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_NUMBER)]
		public string CardNumber { get; set; }
		/// <summary>クレジットカード名義人</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_OWNER)]
		public string CardOwner { get; set; }
		/// <summary>クレジットカード有効期限</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_YM)]
		public string CardYm { get; set; }
		/// <summary>クレジットカード支払い方法</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_PAY_TYPE)]
		public int? CardPayType { get; set; }
		/// <summary>クレジットカード支払い回数</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_INSTALLMENT_DESC)]
		public string CardInstallmentDesc { get; set; }
	}
}
