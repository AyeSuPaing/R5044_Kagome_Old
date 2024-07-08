/*
=========================================================================================================
  Module      : 注文検索API OrderListのレスポンスクラス(OrderListResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace w2.App.Common.LohacoCreatorWebApi.OrderList
{
	/// <summary>
	/// 注文検索API OrderListのレスポンスクラス
	/// </summary>
	[XmlRoot("Result")]
	[Serializable]
	public class OrderListResponse : BaseResponse
	{
		#region +OrderListResponse コンストラクタ
		/// <summary>
		/// 注文検索API OrderListのデフォルトコンストラクタ
		/// </summary>
		public OrderListResponse()
		{
		}
		#endregion

		#region +WriteDebugLogWithMaskedPersonalInfo デバッグログ（個人情報がマスクされる状態）の出力
		/// <summary>
		/// デバッグログ（個人情報がマスクされる状態）の出力
		/// </summary>
		/// <returns>デバッグログ（個人情報がマスクされる状態）内容</returns>
		public override string WriteDebugLogWithMaskedPersonalInfo()
		{
			// 個人情報をマスク用コピーオブジェクトの作成
			var copy = WebApiHelper.DeepClone(this);

			if ((copy == null)
				|| (copy.SearchInfo == null)
				|| (copy.SearchInfo.OrderInfo == null)
				|| (copy.SearchInfo.OrderInfo.Count == 0))
			{
				return WebApiHelper.SerializeXml(copy);
			}

			foreach (var info in copy.SearchInfo.OrderInfo)
			{
				if (info == null) continue;

				if (string.IsNullOrWhiteSpace(info.BillFirstName) == false) info.BillFirstName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(info.BillLastName) == false) info.BillLastName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(info.BillFirstNameKana) == false) info.BillFirstNameKana = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(info.BillLastNameKana) == false) info.BillLastNameKana = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(info.ShipFirstName) == false) info.ShipFirstName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(info.ShipLastName) == false) info.ShipLastName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(info.ShipFirstNameKana) == false) info.ShipFirstNameKana = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(info.ShipLastNameKana) == false) info.ShipLastNameKana = LohacoConstants.MASK_STRING;
			}

			return WebApiHelper.SerializeXml(copy);
		}
		#endregion

		#region プロパティ
		/// <summary>検索結果</summary>
		[XmlElement("Search")]
		public Search SearchInfo { get; set; }
		/// <summary>取得成否（OK/NG）</summary>
		[XmlElement("Status")]
		public LohacoConstants.Status Status { get; set; }
		/// <summary>エラー情報</summary>
		[XmlElement("Error")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Error OrderListError { get; set; }
		#endregion

		#region Search 内部クラス
		/// <summary>
		/// 注文検索API 検索結果クラス
		/// </summary>
		[Serializable]
		public class Search
		{
			#region プロパティ
			/// <summary>全件数</summary>
			[XmlElement("TotalCount")]
			public int TotalCount { get; set; }
			/// <summary>注文情報</summary>
			[XmlElement("OrderInfo")]
			public List<OrderListOrderInfo> OrderInfo { get; set; }
			#endregion
		}
		#endregion

		#region OrderInfo 内部クラス
		/// <summary>
		/// 注文検索API 注文情報クラス
		/// </summary>
		[Serializable]
		public class OrderListOrderInfo
		{
			#region Conditional Serialization
			/// <summary>
			/// IndexエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIndex() { return this.Index.HasValue; }
			/// <summary>
			/// VersionエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeVersion() { return this.Version.HasValue; }
			/// <summary>
			/// DeviceTypeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeDeviceType() { return this.DeviceType.HasValue; }
			/// <summary>
			/// IsSeenエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsSeen() { return this.IsSeen.HasValue; }
			/// <summary>
			/// IsRoyaltyエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsRoyalty() { return this.IsRoyalty.HasValue; }
			/// <summary>
			/// IsRoyaltyFixエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsRoyaltyFix() { return this.IsRoyaltyFix.HasValue; }
			/// <summary>
			/// IsSellerエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsSeller() { return this.IsSeller.HasValue; }
			/// <summary>
			/// OrderTimeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeOrderTime() { return this.OrderTime.HasValue; }
			/// <summary>
			/// LastUpdateTimeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeLastUpdateTime() { return this.LastUpdateTime.HasValue; }
			/// <summary>
			/// OrderStatusエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeOrderStatus() { return this.OrderStatus.HasValue; }
			/// <summary>
			/// RoyaltyFixTimeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeRoyaltyFixTime() { return this.RoyaltyFixTime.HasValue; }
			/// <summary>
			/// PrintSlipFlagエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePrintSlipFlag() { return this.PrintSlipFlag.HasValue; }
			/// <summary>
			/// BuyerCommentsFlagエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeBuyerCommentsFlag() { return this.BuyerCommentsFlag.HasValue; }
			/// <summary>
			/// PayStatusエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePayStatus() { return this.PayStatus.HasValue; }
			/// <summary>
			/// SettleStatusエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeSettleStatus() { return this.SettleStatus.HasValue; }
			/// <summary>
			/// PayDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePayDate() { return this.PayDate.HasValue; }
			/// <summary>
			/// UseWalletエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeUseWallet() { return this.UseWallet.HasValue; }
			/// <summary>
			/// NeedDetailedSlipエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeNeedDetailedSlip() { return this.NeedDetailedSlip.HasValue; }
			/// <summary>
			/// ShipStatusレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeShipStatus() { return this.ShipStatus.HasValue; }
			/// <summary>
			/// ShipRequestDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeShipRequestDate() { return this.ShipRequestDate.HasValue; }
			/// <summary>
			/// ArriveTypeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeArriveType() { return this.ArriveType.HasValue; }
			/// <summary>
			/// DeliveryBoxTypeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeDeliveryBoxType() { return this.DeliveryBoxType.HasValue; }
			/// <summary>
			/// NeedGiftWrapエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeNeedGiftWrap() { return this.NeedGiftWrap.HasValue; }
			/// <summary>
			/// NeedGiftWrapMessageエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeNeedGiftWrapMessage() { return this.NeedGiftWrapMessage.HasValue; }
			/// <summary>
			/// NeedGiftWrapPaperエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeNeedGiftWrapPaper() { return this.NeedGiftWrapPaper.HasValue; }
			/// <summary>
			/// PayChargeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePayCharge() { return this.PayCharge.HasValue; }
			/// <summary>
			/// ShipChargeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeShipCharge() { return this.ShipCharge.HasValue; }
			/// <summary>
			/// GiftWrapChargeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeGiftWrapCharge() { return this.GiftWrapCharge.HasValue; }
			/// <summary>
			/// DiscountエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeDiscount() { return this.Discount.HasValue; }
			/// <summary>
			/// TotalMallCouponDiscountエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeTotalMallCouponDiscount() { return this.TotalMallCouponDiscount.HasValue; }
			/// <summary>
			/// StoreCouponDiscountエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeStoreCouponDiscount() { return this.StoreCouponDiscount.HasValue; }
			/// <summary>
			/// UsePointエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeUsePoint() { return this.UsePoint.HasValue; }
			/// <summary>
			/// TotalPriceエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeTotalPrice() { return this.TotalPrice.HasValue; }
			/// <summary>
			/// RefundTotalPriceエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeRefundTotalPrice() { return this.RefundTotalPrice.HasValue; }
			/// <summary>
			/// IsGetPointFixAllエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsGetPointFixAll() { return this.IsGetPointFixAll.HasValue; }
			/// <summary>
			/// IsLoginエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsLogin() { return this.IsLogin.HasValue; }
			/// <summary>
			/// ArrivalDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeArrivalDate() { return this.ArrivalDate.HasValue; }
			/// <summary>
			/// CouponTypeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeCouponType() { return this.CouponType.HasValue; }
			/// <summary>
			/// StoreCouponTypeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeStoreCouponType() { return this.StoreCouponType.HasValue; }
			/// <summary>
			/// FirstOrderDoneDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeFirstOrderDoneDate() { return this.FirstOrderDoneDate.HasValue; }
			#endregion

			#region プロパティ
			/// <summary>行番号</summary>
			[XmlElement("Index")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Int32? Index { get; set; }
			/// <summary>注文 ID</summary>
			[XmlElement("OrderId", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string OrderId { get; set; }
			/// <summary>Version</summary>
			[XmlElement("Version")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Int32? Version { get; set; }
			/// <summary>デバイス種別</summary>
			[XmlElement("DeviceType")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.DeviceType? DeviceType { get; set; }
			/// <summary>閲覧済みフラグ</summary>
			[XmlElement("IsSeen")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsSeen { get; set; }
			/// <summary>課金フラグ</summary>
			[XmlElement("IsRoyalty")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsRoyalty { get; set; }
			/// <summary>課金確定フラグ</summary>
			[XmlElement("IsRoyaltyFix")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsRoyaltyFix { get; set; }
			/// <summary>管理者注文フラグ</summary>
			[XmlElement("IsSeller")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsSeller { get; set; }
			/// <summary>注文日時</summary>
			[XmlElement("OrderTime")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? OrderTime { get; set; }
			/// <summary>最終更新日時</summary>
			[XmlElement("LastUpdateTime")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? LastUpdateTime { get; set; }
			/// <summary>注文ステータス</summary>
			[XmlElement("OrderStatus")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.OrderStatus? OrderStatus { get; set; }
			/// <summary>課金確定日時</summary>
			[XmlElement("RoyaltyFixTime")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? RoyaltyFixTime { get; set; }
			/// <summary>注文伝票出力有無</summary>
			[XmlElement("PrintSlipFlag")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? PrintSlipFlag { get; set; }
			/// <summary>バイヤーコメント有無</summary>
			[XmlElement("BuyerCommentsFlag")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? BuyerCommentsFlag { get; set; }
			/// <summary>入金ステータス </summary>
			[XmlElement("PayStatus")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.PayStatus? PayStatus { get; set; }
			/// <summary>決済ステータス</summary>
			[XmlElement("SettleStatus")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.SettleStatus? SettleStatus { get; set; }
			/// <summary>支払い方法</summary>
			[XmlElement("PayMethod", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string PayMethod { get; set; }
			/// <summary>支払い方法名称</summary>
			[XmlElement("PayMethodName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string PayMethodName { get; set; }
			/// <summary>入金日</summary>
			[XmlElement("PayDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? PayDate { get; set; }
			/// <summary>決済 ID</summary>
			[XmlElement("SettleId", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string SettleId { get; set; }
			/// <summary>ウォレット利用有無</summary>
			[XmlElement("UseWallet")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? UseWallet { get; set; }
			/// <summary>納品書有無</summary>
			[XmlElement("NeedDetailedSlip")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? NeedDetailedSlip { get; set; }
			/// <summary>ご請求先名前</summary>
			[XmlElement("BillFirstName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillFirstName { get; set; }
			/// <summary>ご請求先名前カナ</summary>
			[XmlElement("BillFirstNameKana", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillFirstNameKana { get; set; }
			/// <summary>ご請求先名字</summary>
			[XmlElement("BillLastName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillLastName { get; set; }
			/// <summary>ご請求先名字カナ</summary>
			[XmlElement("BillLastNameKana", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillLastNameKana { get; set; }
			/// <summary>ご請求先郵便番号</summary>
			[XmlElement("BillZipCode", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillZipCode { get; set; }
			/// <summary>ご請求先都道府県 </summary>
			[XmlElement("BillPrefecture", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillPrefecture { get; set; }
			/// <summary>出荷ステータス</summary>
			[XmlElement("ShipStatus")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.ShipStatus? ShipStatus { get; set; }
			/// <summary>配送方法</summary>
			[XmlElement("ShipMethod", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipMethod { get; set; }
			/// <summary>配送希望日</summary>
			[XmlElement("ShipRequestDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? ShipRequestDate { get; set; }
			/// <summary>配送希望時間</summary>
			[XmlElement("ShipRequestTime", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipRequestTime { get; set; }
			/// <summary>配送メモ</summary>
			[XmlElement("ShipNotes", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipNotes { get; set; }
			/// <summary>配送伝票番号１</summary>
			[XmlElement("ShipInvoiceNumber1", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipInvoiceNumber1 { get; set; }
			/// <summary>配送伝票番号 2</summary>
			[XmlElement("ShipInvoiceNumber2", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipInvoiceNumber2 { get; set; }
			/// <summary>翌日配送フラグ</summary>
			[XmlElement("ArriveType")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.ArriveType? ArriveType { get; set; }
			/// <summary>宅配 BOX 利用</summary>
			[XmlElement("DeliveryBoxType")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.DeliveryBoxType? DeliveryBoxType { get; set; }
			/// <summary>出荷日(yyyyMMddフォーマット)</summary>
			[XmlElement("ShipDate", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipDate { get; set; }
			/// <summary>ギフト包装有無</summary>
			[XmlElement("NeedGiftWrap")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? NeedGiftWrap { get; set; }
			/// <summary>ギフトメッセージ有無</summary>
			[XmlElement("NeedGiftWrapMessage")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? NeedGiftWrapMessage { get; set; }
			/// <summary>のし有無</summary>
			[XmlElement("NeedGiftWrapPaper")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? NeedGiftWrapPaper { get; set; }
			/// <summary>お届け先名前</summary>
			[XmlElement("ShipFirstName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipFirstName { get; set; }
			/// <summary>お届け先名前カナ</summary>
			[XmlElement("ShipFirstNameKana", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipFirstNameKana { get; set; }
			/// <summary>お届け先名字</summary>
			[XmlElement("ShipLastName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipLastName { get; set; }
			/// <summary>お届け先名字カナ</summary>
			[XmlElement("ShipLastNameKana", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipLastNameKana { get; set; }
			/// <summary>お届け先都道府県</summary>
			[XmlElement("ShipPrefecture", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipPrefecture { get; set; }
			/// <summary>手数料</summary>
			[XmlElement("PayCharge")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? PayCharge { get; set; }
			/// <summary>送料</summary>
			[XmlElement("ShipCharge")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? ShipCharge { get; set; }
			/// <summary>ギフト包装料</summary>
			[XmlElement("GiftWrapCharge")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? GiftWrapCharge { get; set; }
			/// <summary>値引き</summary>
			[XmlElement("Discount")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? Discount { get; set; }
			/// <summary>モールクーポン値引き額</summary>
			[XmlElement("TotalMallCouponDiscount")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? TotalMallCouponDiscount { get; set; }
			/// <summary>ストアクーポン値引き額</summary>
			[XmlElement("StoreCouponDiscount")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? StoreCouponDiscount { get; set; }
			/// <summary>利用ポイント数</summary>
			[XmlElement("UsePoint")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? UsePoint { get; set; }
			/// <summary>合計金額</summary>
			[XmlElement("TotalPrice")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? TotalPrice { get; set; }
			/// <summary>返金合計金額</summary>
			[XmlElement("RefundTotalPrice")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? RefundTotalPrice { get; set; }
			/// <summary>全付与ポイント確定有無</summary>
			[XmlElement("IsGetPointFixAll")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsGetPointFixAll { get; set; }
			/// <summary>セラーID</summary>
			[XmlElement("SellerId", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string SellerId { get; set; }
			/// <summary>Yahoo! JAPAN IDログイン有無</summary>
			[XmlElement("IsLogin")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsLogin { get; set; }
			/// <summary>顧客コード</summary>
			[XmlElement("CustomerCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string CustomerCd { get; set; }
			/// <summary>配送会社 URL</summary>
			[XmlElement("ShipUrl", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipUrl { get; set; }
			/// <summary>配送方法名称</summary>
			[XmlElement("ShipMethodName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipMethodName { get; set; }
			/// <summary>着荷日</summary>
			[XmlElement("ArrivalDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? ArrivalDate { get; set; }
			/// <summary>クーポン種別</summary>
			[XmlElement("CouponType")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.CouponType? CouponType { get; set; }
			/// <summary>使用したモールクーポン情報</summary>
			[XmlElement("CouponCampaignCode", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string CouponCampaignCode { get; set; }
			/// <summary>ストアクーポン種別</summary>
			[XmlElement("StoreCouponType")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.StoreCouponType? StoreCouponType { get; set; }
			/// <summary>ストアクーポンコード</summary>
			[XmlElement("StoreCouponCode", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string StoreCouponCode { get; set; }
			/// <summary>ストアクーポン名</summary>
			[XmlElement("StoreCouponName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string StoreCouponName { get; set; }
			/// <summary>初回完了日</summary>
			[XmlElement("FirstOrderDoneDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? FirstOrderDoneDate { get; set; }
			/// <summary>取得情報値（商品情報） </summary>
			[XmlElement("Item")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public OrderListItem Item { get; set; }
			#endregion
		}
		#endregion

		#region OrderListItem 内部クラス
		/// <summary>
		/// 注文検索API 注文商品情報クラス
		/// </summary>
		[Serializable]
		public class OrderListItem
		{
			#region プロパティ
			/// <summary>注文（最長）発売日(yyyy-MM-ddフォーマット)</summary>
			[XmlElement("ReleaseDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? ReleaseDate { get; set; }
			#endregion
		}
		#endregion
	}
}
