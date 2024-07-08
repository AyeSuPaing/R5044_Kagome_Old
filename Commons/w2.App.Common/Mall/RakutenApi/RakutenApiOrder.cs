/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API 受注モデル (RakutenApiOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API 受注モデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_RESPONSE_ORDER_MODEL_LIST)]
	public class RakutenApiOrder
	{
		/// <summary>APIの日付型フォーマット</summary>
		private const string RAKUTEN_PAY_API_DATETIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss+0900";
		/// <summary>APIの日付型フォーマット</summary>
		private const string RAKUTEN_PAY_API_DATE_FORMAT = "yyyy-MM-dd";

		/// <summary>
		/// 日付文字列をDate型に変換
		/// </summary>
		/// <param name="input">対象文字列</param>
		/// <param name="format">日付型フォーマット</param>
		/// <returns></returns>
		private DateTime? ToDate(string input)
		{
			if (input == null) return null;

			DateTime result;
			DateTime.TryParseExact(input, RAKUTEN_PAY_API_DATE_FORMAT, null, DateTimeStyles.AssumeLocal, out result);
			return result;
		}
		/// <summary>
		/// 日付文字列をDateTime型に変換
		/// </summary>
		/// <param name="input">対象文字列</param>
		/// <returns></returns>
		private DateTime? ToDateTime(string input)
		{
			if (input == null) return null;

			DateTime result;
			DateTime.TryParseExact(input, RAKUTEN_PAY_API_DATETIME_FORMAT, null, DateTimeStyles.AssumeLocal, out result);
			return result;
		}
		/// <summary>
		/// 日付文字列をDateTime型に変換
		/// </summary>
		/// <param name="input">対象文字列</param>
		/// <param name="format">日付型フォーマット</param>
		/// <returns></returns>
		private DateTime? ToDateTime(string input, string format)
		{
			if (input == null) return null;

			DateTime result;
			DateTime.TryParseExact(input, format, null, DateTimeStyles.AssumeLocal, out result);
			return result;
		}

		/// <summary>注文番号</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_ORDER_NUMBER)]
		public string OrderNumber { get; set; }
		/// <summary>ステータス</summary>
		/// <remarks>
		/// 100: 注文確認待ち
		/// 200: 楽天処理中
		/// 300: 発送待ち
		/// 400: 変更確定待ち
		/// 500: 発送済
		/// 600: 支払手続き中
		/// 700: 支払手続き済
		/// 800: キャンセル確定待ち
		/// 900: キャンセル確定
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_ORDER_PROGRESS)]
		public int? OrderProgress { get; set; }
		/// <summary>サブステータスID	</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_SUB_STATUS_ID)]
		public string SubStatusId { get; set; }
		/// <summary>サブステータス</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_SUB_STATUS_NAME)]
		public string SubStatusName { get; set; }
		/// <summary>注文日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_ORDER_DATETIME)]
		public string OrderDatetimeString
		{
			get
			{
				return this.OrderDatetime.HasValue
					? this.OrderDatetime.Value.ToString(RAKUTEN_PAY_API_DATETIME_FORMAT)
					: "";
			}
			set { if(string.IsNullOrEmpty(value) == false) this.OrderDatetime = ToDateTime(value); }
		}
		/// <summary>注文日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public DateTime? OrderDatetime { get; set; }
		/// <summary>注文確認日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_SHOP_ORDER_CFM_DATETIME)]
		public string ShopOrderCfmDatetimeString
		{
			get
			{
				return this.ShopOrderCfmDatetime.HasValue
					? this.ShopOrderCfmDatetime.Value.ToString(RAKUTEN_PAY_API_DATETIME_FORMAT)
					: "";
			}
			set { if (string.IsNullOrEmpty(value) == false) this.ShopOrderCfmDatetime = ToDateTime(value); }
		}
		/// <summary>注文確認日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public DateTime? ShopOrderCfmDatetime { get; set; }
		/// <summary>注文確定日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_ORDER_FIX_DATETIME)]
		public string OrderFixDatetimeString
		{
			get
			{
				return this.OrderFixDatetime.HasValue
					? this.OrderFixDatetime.Value.ToString(RAKUTEN_PAY_API_DATETIME_FORMAT)
					: "";
			}
			set { if (string.IsNullOrEmpty(value) == false) this.OrderFixDatetime = ToDateTime(value); }
		}
		/// <summary>注文確定日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public DateTime? OrderFixDatetime { get; set; }
		/// <summary>発送指示日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_SHIPPING_INST_DATETIME)]
		public string ShippingInstDatetimeString
		{
			get
			{
				return this.ShippingInstDatetime.HasValue
					? this.ShippingInstDatetime.Value.ToString(RAKUTEN_PAY_API_DATETIME_FORMAT)
					: "";
			}
			set { if (string.IsNullOrEmpty(value) == false) this.ShippingInstDatetime = ToDateTime(value); }
		}
		/// <summary>発送指示日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public DateTime? ShippingInstDatetime { get; set; }
		/// <summary>発送完了報告日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_SHIPPING_CMPL_RPT_DATETIME)]
		public string ShippingCmplRptDatetimeString
		{
			get
			{
				return this.ShippingCmplRptDatetime.HasValue
					? this.ShippingCmplRptDatetime.Value.ToString(RAKUTEN_PAY_API_DATETIME_FORMAT)
					: "";
			}
			set { if (string.IsNullOrEmpty(value) == false) this.ShippingCmplRptDatetime = ToDateTime(value); }
		}
		/// <summary>発送完了報告日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public DateTime? ShippingCmplRptDatetime { get; set; }
		/// <summary>キャンセル期限日（YYYY-MM-DD）</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_CANCEL_DUE_DATE)]
		public string CancelDueDateString
		{
			get
			{
				return this.CancelDueDate.HasValue
					? this.CancelDueDate.Value.ToString(RAKUTEN_PAY_API_DATETIME_FORMAT)
					: "";
			}
			set { if (string.IsNullOrEmpty(value) == false) this.CancelDueDate = ToDateTime(value); }
		}
		/// <summary>キャンセル期限日（YYYY-MM-DD）</summary>
		public DateTime? CancelDueDate { get; set; }
		/// <summary>お届け日指定（YYYY-MM-DD）</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_DELIVERY_DATE)]
		public DateTime? DeliveryDate { get; set; }
		/// <summary>お届け時間帯</summary>
		/// <remarks>
		/// 0: なし
		/// 1: 午前
		/// 2: 午後
		/// 9: その他
		/// h1h2: h1時-h2時(h1=7～24, h2=07～24)
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_SHIPPING_TERM)]
		public int? ShippingTerm { get; set; }
		/// <summary>コメント</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_REMARKS)]
		public string Remarks { get; set; }
		/// <summary>ギフト配送希望フラグ</summary>
		/// <remarks>
		/// 0: ギフト注文ではない
		/// 1: ギフト注文である
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_GIFT_CHECK_FLAG)]
		public int? GiftCheckFlag { get; set; }
		/// <summary>複数送付先フラグ</summary>
		/// <remarks>
		/// 0: 複数配送先無し
		/// 1: 複数配送先有り
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_SEVERAL_SENDER_FLAG)]
		public int? SeveralSenderFlag { get; set; }
		/// <summary>送付先一致フラグ</summary>
		/// <remarks>
		/// 0: 注文者と送付者の住所が同じではない
		/// 1: 注文が単数で注文者と送付者の住所が同じ
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_EQUAL_SENDER_FLAG)]
		public int? EqualSenderFlag { get; set; }
		/// <summary>離島フラグ</summary>
		/// <remarks>
		/// 0: 送付先に離島が含まれていない
		/// 1: 送付先に離島が含まれている
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_ISOLATED_ISLAND_FLAG)]
		public int? IsolatedIslandFlag { get; set; }
		/// <summary>楽天会員フラグ</summary>
		/// <remarks>
		/// 0: 楽天会員ではない
		/// 1: 楽天会員である
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_RAKUTEN_MEMBER_FLAG)]
		public int? RakutenMemberFlag { get; set; }
		/// <summary>利用端末</summary>
		/// <remarks>
		/// 0: PC (Windows系のスマートフォン、タブレットを含む)
		/// 1: モバイル(docomo) フィーチャーフォン
		/// 2: モバイル(KDDI) フィーチャーフォン
		/// 3: モバイル(Softbank) フィーチャーフォン
		/// 5: モバイル(WILLCOM) フィーチャーフォン
		/// 11: スマートフォン（iPhone系）
		/// 12: スマートフォン（Android系）
		/// 19: スマートフォン（その他）
		/// 21: タブレット（iPad系）
		/// 22: タブレット（Android系）
		/// 29: タブレット（その他）
		/// 99: その他
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_CARRIER_CODE)]
		public int CarrierCode { get; set; }
		/// <summary>メールキャリアコード</summary>
		/// <remarks>
		/// 0: PC ("@i.softbank.jp"を含む)
		/// 1: DoCoMo
		/// 2: au
		/// 3: SoftBank
		/// 5: WILLCOM
		/// 99: その他
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_EMAIL_CARRIER_CODE)]
		public int EmailCarrierCode { get; set; }
		/// <summary>注文種別</summary>
		/// <remarks>
		/// 1: 通常購入
		/// 4: 定期購入
		/// 5: 頒布会
		/// 6: 予約商品
		/// </remarks>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_ORDER_TYPE)]
		public int? OrderType { get; set; }
		/// <summary>申込番号 </summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_RESERVE_NUMBER)]
		public string ReserveNumber { get; set; }
		/// <summary>申込お届け回数 （予約=1、定期購入,頒布会=確定回数）</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_RESERVE_DELIVERY_COUNT)]
		public int? ReserveDeliveryCount { get; set; }
		/// <summary>警告表示タイプ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_CAUTION_DISPLAY_TYPE)]
		public int? CautionDisplayType { get; set; }
		/// <summary>警告表示タイプ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_CAUTION_DISPLAY_DETAIL_TYPE)]
		public int? CautionDisplayDetailType { get; set; }
		/// <summary>楽天確認中フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_RAKUTEN_CONFIRM_FLAG)]
		public int? RakutenConfirmFlag { get; set; }
		/// <summary>商品合計金額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_GOODS_PRICE)]
		public decimal GoodsPrice { get; set; }
		/// <summary>消費税合計</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_GOODS_TAX)]
		public decimal GoodsTax { get; set; }
		/// <summary>送料合計</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_POSTAGE_PRICE)]
		public decimal PostagePrice { get; set; }
		/// <summary>代引料合計</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_DELIVERY_PRICE)]
		public decimal DeliveryPrice { get; set; }
		/// <summary>決済手数料合計</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_PAYMENT_CHARGE)]
		public decimal PaymentCharge { get; set; }
		/// <summary>決済手数料税率</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_PAYMENT_CHARGE_TAX_RATE)]
		public decimal PaymentChargeTaxRate { get; set; }
		/// <summary>合計金額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_TOTAL_PRICE)]
		public decimal TotalPrice { get; set; }
		/// <summary>請求金額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_REQUEST_PRICE)]
		public decimal RequestPrice { get; set; }
		/// <summary>クーポン利用総額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_COUPON_ALL_TOTAL_PRICE)]
		public decimal CouponAllTotalPrice { get; set; }
		/// <summary>店舗発行クーポン利用額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_COUPON_SHOP_PRICE)]
		public decimal CouponShopPrice { get; set; }
		/// <summary>楽天発行クーポン利用額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_COUPON_OTHER_PRICE)]
		public decimal CouponOtherPrice { get; set; }
		/// <summary>店舗負担金合計</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_ADDITIONAL_FEE_OCCUR_AMOUNT_TO_SHOP)]
		public decimal AdditionalFeeOccurAmountToShop { get; set; }
		/// <summary>あす楽希望フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_ASURAKU_FLAG)]
		public int? AsurakuFlag { get; set; }
		/// <summary>医薬品受注フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_DRUG_FLAG)]
		public int? DrugFlag { get; set; }
		/// <summary>楽天スーパーDEAL商品受注フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_DEAL_FLAG)]
		public int? DealFlag { get; set; }
		/// <summary>メンバーシッププログラム受注タイプ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_MEMBERSHIP_TYPE)]
		public int? MembershipType { get; set; }
		/// <summary>ひとことメモ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_MEMO)]
		public string Memo { get; set; }
		/// <summary>担当者</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_OPERATOR)]
		public string Operator { get; set; }
		/// <summary>メール差込文(お客様へのメッセージ)</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_MAIL_PLUG_SENTENCE)]
		public string MailPlugSentence { get; set; }
		/// <summary>購入履歴修正有無フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_MODIFY_FLAG)]
		public int? ModifyFlag { get; set; }
		/// <summary>領収書発行回数</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_RCEIPT_ISSUE_COUNT)]
		public int? ReceiptIssueCount { get; set; }
		/// <summary>領収書発行履歴リスト</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_RCEIPT_ISSUE_HISTORY_LIST)]
		public List<DateTime> ReceiptIssueHistoryList { get; set; }
		/// <summary>消費税再計算フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_IS_TAX_RECALC)]
		public int? IsTaxRecalc { get; set; }
		/// <summary>注文者モデル</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_ORDERER_MODEL)]
		public RakutenApiOrderer OrdererModel { get; set; }
		/// <summary>支払方法モデル</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_SETTLEMENT_MODEL)]
		public RakutenApiSettlement SettlementModel { get; set; }
		/// <summary>配送方法モデル</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_DELIVERY_MODEL)]
		public RakutenApiDelivery DeliveryModel { get; set; }
		/// <summary>ポイントモデル</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_POINT_MODEL)]
		public RakutenApiPoint PointModel { get; set; }
		/// <summary>ラッピングモデル1</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_WRAPPING_MODEL1)]
		public RakutenApiWrapping WrappingModel1 { get; set; }
		/// <summary>ラッピングモデル2</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_WRAPPING_MODEL2)]
		public RakutenApiWrapping WrappingModel2 { get; set; }
		/// <summary>送付先モデルリスト</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_PACKAGE_MODEL_LIST)]
		public RakutenApiPackage[] PackageModelList { get; set; }
		/// <summary>クーポンモデルリスト</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_COUPON_MODEL_LIST)]
		public RakutenApiCoupon[] CouponModelList { get; set; }
		/// <summary>注文者負担金合計</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDERER_MODEL_ADDITIONAL_FEE_OCCUR_AMOUNT_TO_USER)]
		public decimal AdditionalFeeOccurAmountToUser { get; set; }
		/// <summary>税率モデルリスト</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ORDER_MODEL_TAX_SUMMARY_MODEL_LIST)]
		public RakutenApiTaxSummary[] TaxSummaryModelList { get; set; }
	}
}
