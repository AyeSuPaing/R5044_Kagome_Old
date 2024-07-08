/*
=========================================================================================================
  Module      : LINE連携の定期台帳 (LineFixedPurchase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using Newtonsoft.Json;

/// <summary>
/// LINE連携の定期台帳
/// </summary>
[Serializable]
public class LineFixedPurchase
{
	/// <summary>定期購入ID</summary>
	[JsonProperty("fixed_purchase_id")]
	public string FixedPurchaseId { get; set; }
	/// <summary>定期購入区分</summary>
	[JsonProperty("fixed_purchase_kbn")]
	public string FixedPurchaseKbn { get; set; }
	/// <summary>定期購入設定１</summary>
	[JsonProperty("fixed_purchase_setting1")]
	public string FixedPurchaseSetting1 { get; set; }
	/// <summary>定期購入ステータス</summary>
	[JsonProperty("fixed_purchase_status")]
	public string FixedPurchaseStatus { get; set; }
	/// <summary>決済ステータス</summary>
	[JsonProperty("payment_status")]
	public string PaymentStatus { get; set; }
	/// <summary>最終購入日</summary>
	[JsonProperty("last_order_date")]
	public string LastOrderDate { get; set; }
	/// <summary>購入回数(注文基準)</summary>
	[JsonProperty("order_count")]
	public int OrderCount { get; set; }
	/// <summary>ユーザID</summary>
	[JsonProperty("user_id")]
	public string UserId { get; set; }
	/// <summary>店舗ID</summary>
	[JsonProperty("shop_id")]
	public string ShopId { get; set; }
	/// <summary>注文区分</summary>
	[JsonProperty("order_kbn")]
	public string OrderKbn { get; set; }
	/// <summary>支払区分</summary>
	[JsonProperty("order_payment_kbn")]
	public string OrderPaymentKbn { get; set; }
	/// <summary>定期購入開始日時</summary>
	[JsonProperty("fixed_purchase_date_bgn")]
	public string FixedPurchaseDateBgn { get; set; }
	/// <summary>有効フラグ</summary>
	[JsonProperty("valid_flg")]
	public string ValidFlg { get; set; }
	/// <summary>作成日</summary>
	[JsonProperty("date_created")]
	public string DateCreated { get; set; }
	/// <summary>更新日</summary>
	[JsonProperty("date_changed")]
	public string DateChanged { get; set; }
	/// <summary>最終更新者</summary>
	[JsonProperty("last_changed")]
	public string LastChanged { get; set; }
	/// <summary>クレジットカード枝番</summary>
	[JsonProperty("credit_branch_no")]
	public int? CreditBranchNo { get; set; }
	/// <summary>次回配送日</summary>
	[JsonProperty("next_shipping_date")]
	public string NextShippingDate { get; set; }
	/// <summary>次々回配送日</summary>
	[JsonProperty("next_next_shipping_date")]
	public string NextNextShippingDate { get; set; }
	/// <summary>定期購入管理メモ</summary>
	[JsonProperty("fixed_purchase_management_memo")]
	public string FixedPurchaseManagementMemo { get; set; }
	/// <summary>カード支払い回数コード</summary>
	[JsonProperty("card_installments_code")]
	public string CardInstallmentsCode { get; set; }
	/// <summary>購入回数(出荷基準)</summary>
	[JsonProperty("shipped_count")]
	public int ShippedCount { get; set; }
	/// <summary>解約理由区分ID</summary>
	[JsonProperty("cancel_reason_id")]
	public string CancelReasonId { get; set; }
	/// <summary>解約メモ</summary>
	[JsonProperty("cancel_memo")]
	public string CancelMemo { get; set; }
	/// <summary>次回購入の利用ポイント数</summary>
	[JsonProperty("next_shipping_use_point")]
	public decimal NextShippingUsePoint { get; set; }
	/// <summary>定期購入同梱元定期購入ID</summary>
	[JsonProperty("combined_org_fixedpurchase_ids")]
	public string CombinedOrgFixedpurchaseIds { get; set; }
	/// <summary>メモ</summary>
	[JsonProperty("memo")]
	public string Memo { get; set; }
	/// <summary>解約日</summary>
	[JsonProperty("cancel_date")]
	public string CancelDate { get; set; }
	/// <summary>再開日</summary>
	[JsonProperty("restart_date")]
	public string RestartDate { get; set; }
	/// <summary>外部支払契約ID</summary>
	[JsonProperty("external_payment_agreement_id")]
	public string ExternalPaymentAgreementId { get; set; }
	/// <summary>定期再開予定日</summary>
	[JsonProperty("resume_date")]
	public string ResumeDate { get; set; }
	/// <summary>休止理由</summary>
	[JsonProperty("suspend_reason")]
	public string SuspendReason { get; set; }
	/// <summary>配送メモ</summary>
	[JsonProperty("shipping_memo")]
	public string ShippingMemo { get; set; }
	/// <summary>次回購入の利用クーポンID</summary>
	[JsonProperty("next_shipping_use_coupon_id")]
	public string NextShippingUseCouponId { get; set; }
	/// <summary>次回購入の利用クーポン枝番</summary>
	[JsonProperty("next_shipping_use_coupon_no")]
	public int NextShippingUseCouponNo { get; set; }
	/// <summary>領収書希望フラグ</summary>
	[JsonProperty("receipt_flg")]
	public string ReceiptFlg { get; set; }
	/// <summary>宛名</summary>
	[JsonProperty("receipt_address")]
	public string ReceiptAddress { get; set; }
	/// <summary>但し書き</summary>
	[JsonProperty("receipt_proviso")]
	public string ReceiptProviso { get; set; }
}
