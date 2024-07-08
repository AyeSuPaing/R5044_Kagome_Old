/*
=========================================================================================================
  Module      : 注文詳細API OrderInfoのレスポンスクラス(OrderInfoResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace w2.App.Common.LohacoCreatorWebApi.OrderInfo
{
	/// <summary>
	/// 注文詳細API OrderInfoのレスポンスクラス
	/// </summary>
	[XmlRoot("ResultSet")]
	[Serializable]
	public class OrderInfoResponse : BaseResponse
	{
		#region +OrderInfoResponse コンストラクタ
		/// <summary>
		/// 注文詳細API OrderInfoResponseのデフォルトコンストラクタ
		/// </summary>
		public OrderInfoResponse()
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
			var copy = WebApiHelper.DeepClone(this);

			if ((copy.Result == null) || (copy.Result.OrderInfo == null)) return WebApiHelper.SerializeXml(copy);

			var pay = copy.Result.OrderInfo.Pay;
			if (pay != null)
			{
				if (string.IsNullOrWhiteSpace(pay.BillFirstName) == false) pay.BillFirstName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillLastName) == false) pay.BillLastName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillFirstNameKana) == false) pay.BillFirstNameKana = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillLastNameKana) == false) pay.BillLastNameKana = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillAddress1) == false) pay.BillAddress1 = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillAddress2) == false) pay.BillAddress2 = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillPhoneNumber) == false) pay.BillPhoneNumber = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillMailAddress) == false) pay.BillMailAddress = LohacoConstants.MASK_STRING;
			}

			var ship = copy.Result.OrderInfo.Ship;
			if (ship != null)
			{
				if (string.IsNullOrWhiteSpace(ship.GiftWrapName) == false) ship.GiftWrapName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(ship.ShipFirstName) == false) ship.ShipFirstName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(ship.ShipLastName) == false) ship.ShipLastName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(ship.ShipFirstNameKana) == false) ship.ShipFirstNameKana = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(ship.ShipLastNameKana) == false) ship.ShipLastNameKana = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(ship.ShipAddress1) == false) ship.ShipAddress1 = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(ship.ShipAddress2) == false) ship.ShipAddress2 = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(ship.ShipPhoneNumber) == false) ship.ShipPhoneNumber = LohacoConstants.MASK_STRING;
			}

			return WebApiHelper.SerializeXml(copy);
		}
		#endregion

		#region プロパティ
		/// <summary>合計レスポンス件数</summary>
		[XmlAttribute("totalResultsAvailable")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int TotalResultsAvailable { get; set; }
		/// <summary>返却レスポンス件数</summary>
		[XmlAttribute("totalResultsReturned")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int TotalResultsReturned { get; set; }
		/// <summary>1番目レスポンスの位置</summary>
		[XmlAttribute("firstResultPosition")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int FirstResultPosition { get; set; }
		/// <summary>レスポンス</summary>
		[XmlElement("Result")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public OrderInfoResult Result { get; set; }
		#endregion

		#region OrderInfoResult 内部クラス
		/// <summary>
		/// 注文詳細API結果のクラス
		/// </summary>
		[Serializable]
		public class OrderInfoResult : BaseResult
		{
			#region プロパティ
			/// <summary>注文情報</summary>
			[XmlElement("OrderInfo")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public OrderInfo OrderInfo { get; set; }
			#endregion
		}
		#endregion

		#region OrderInfo  内部クラス
		/// <summary>
		/// 注文詳細クラス
		/// </summary>
		[Serializable]
		public class OrderInfo
		{
			#region Conditional Serialization
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
			public bool ShouldSerizlizeIsSeen() { return this.IsSeen.HasValue; }
			/// <summary>
			/// CancelReasonエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeCancelReason() { return this.CancelReason.HasValue; }
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
			/// SendConfirmTimeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeSendConfirmTime() { return this.SendConfirmTime.HasValue; }
			/// <summary>
			/// SendPayTimeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeSendPayTime() { return this.SendPayTime.HasValue; }
			/// <summary>
			/// PrintSlipTimeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePrintSlipTime() { return this.PrintSlipTime.HasValue; }
			/// <summary>
			/// PrintDeliveryTimeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePrintDeliveryTime() { return this.PrintDeliveryTime.HasValue; }
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
			/// AutoDoneDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeAutoDoneDate() { return this.AutoDoneDate.HasValue; }
			/// <summary>
			/// FirstOrderDoneDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeFirstOrderDoneDate() { return this.FirstOrderDoneDate.HasValue; }
			#endregion

			#region プロパティ
			/// <summary>支払情報</summary>
			[XmlElement("Pay")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Pay Pay { get; set; }
			/// <summary>お届情報</summary>
			[XmlElement("Ship")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Ship Ship { get; set; }
			/// <summary>明細情報</summary>
			[XmlElement("Detail")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Detail Detail { get; set; }
			/// <summary>商品ライン情報</summary>
			[XmlElement("Item")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public List<Item> Item { get; set; }
			/// <summary>セラー情報</summary>
			[XmlElement("Seller")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Seller Seller { get; set; }
			/// <summary>バイヤー情報</summary>
			[XmlElement("Buyer")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Buyer Buyer { get; set; }
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
			/// <summary>キャンセル理由</summary>
			[XmlElement("CancelReason")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.CancelReason? CancelReason { get; set; }
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
			/// <summary>注文確認メール送信時刻(yyy-MM-ddTHH:mm:ss+09:00フォーマット)</summary>
			[XmlElement("SendConfirmTime")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? SendConfirmTime { get; set; }
			/// <summary>支払完了メール送信時刻(yyy-MM-ddTHH:mm:ss+09:00フォーマット)</summary>
			[XmlElement("SendPayTime")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? SendPayTime { get; set; }
			/// <summary>注文伝票出力時刻(yyy-MM-ddTHH:mm:ss+09:00フォーマット)</summary>
			[XmlElement("PrintSlipTime")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? PrintSlipTime { get; set; }
			/// <summary>納品書出力時刻</summary>
			[XmlElement("PrintDeliveryTime")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? PrintDeliveryTime { get; set; }
			/// <summary>バイヤーコメント</summary>
			[XmlElement("BuyerComments", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BuyerComments { get; set; }
			/// <summary>セラーコメント</summary>
			[XmlElement("SellerComments", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string SellerComments { get; set; }
			/// <summary>社内メモ</summary>
			[XmlElement("Notes", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Notes { get; set; }
			/// <summary>更新者</summary>
			[XmlElement("OperationUser", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string OperationUser { get; set; }
			/// <summary>履歴 ID</summary>
			[XmlElement("HistoryId", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string HistoryId { get; set; }
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
			/// <summary>自動完了予定日</summary>
			[XmlElement("AutoDoneDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? AutoDoneDate { get; set; }
			/// <summary>初回完了日</summary>
			[XmlElement("FirstOrderDoneDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? FirstOrderDoneDate { get; set; }
			#endregion
		}
		#endregion

		#region Pay 内部クラス
		/// <summary>
		/// 支払情報クラス
		/// </summary>
		[Serializable]
		public class Pay
		{
			#region Conditional Serialization
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
			/// PayActionTimeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePayActionTime() { return this.PayActionTime.HasValue; }
			/// <summary>
			/// PayDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePayDate() { return this.PayDate.HasValue; }
			/// <summary>
			/// CardPayTypeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeCardPayType() { return this.CardPayType.HasValue; }
			/// <summary>
			/// CardPayCountエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeCardPayCount() { return this.CardPayCount.HasValue; }
			/// <summary>
			/// UseYahooCardエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeUseYahooCard() { return this.UseYahooCard.HasValue; }
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
			#endregion

			#region プロパティ
			/// <summary>入金ステータス</summary>
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
			/// <summary>支払い日時</summary>
			[XmlElement("PayActionTime")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? PayActionTime { get; set; }
			/// <summary>入金日</summary>
			[XmlElement("PayDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? PayDate { get; set; }
			/// <summary>入金処理備考</summary>
			[XmlElement("PayNotes", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string PayNotes { get; set; }
			/// <summary>決済 ID</summary>
			[XmlElement("SettleId", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string SettleId { get; set; }
			/// <summary>カード支払い区分</summary>
			[XmlElement("CardPayType")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.CardPayType? CardPayType { get; set; }
			/// <summary>カード支払回数</summary>
			[XmlElement("CardPayCount")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Int32? CardPayCount { get; set; }
			/// <summary>Yahoo!カード利用有無</summary>
			[XmlElement("UseYahooCard")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? UseYahooCard { get; set; }
			/// <summary>ウォレット利用有無</summary>
			[XmlElement("UseWallet")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? UseWallet { get; set; }
			/// <summary>納品書有無</summary>
			[XmlElement("NeedDetailedSlip")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? NeedDetailedSlip { get; set; }
			/// <summary>ご請求先名前 </summary>
			[XmlElement("BillFirstName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillFirstName { get; set; }
			/// <summary>ご請求先名前カナ</summary>
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			[XmlElement("BillFirstNameKana", IsNullable = false)]
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
			/// <summary>ご請求先都道府県</summary>
			[XmlElement("BillPrefecture", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillPrefecture { get; set; }
			/// <summary>ご請求先市区郡</summary>
			[XmlElement("BillCity", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillCity { get; set; }
			/// <summary>ご請求先住所 1 </summary>
			[XmlElement("BillAddress1", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillAddress1 { get; set; }
			/// <summary>ご請求先住所 2</summary>
			[XmlElement("BillAddress2", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillAddress2 { get; set; }
			/// <summary>ご請求先電話番号</summary>
			[XmlElement("BillPhoneNumber", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillPhoneNumber { get; set; }
			/// <summary>ご請求先メールアドレス</summary>
			[XmlElement("BillMailAddress", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillMailAddress { get; set; }
			#endregion
		}
		#endregion

		#region Ship 内部クラス
		/// <summary>
		/// 配送情報クラス
		/// </summary>
		[Serializable]
		public class Ship
		{
			#region Conditional Serialization
			/// <summary>
			/// ShipStatusエレメントをXML文字列に出力するかどうか判定
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
			/// ShipDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeShipDate() { return this.ShipDate.HasValue; }
			/// <summary>
			/// ArrivalDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeArrivalDate() { return this.ArrivalDate.HasValue; }
			/// <summary>
			/// NeedGiftWrapエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeNeedGiftWrap() { return this.NeedGiftWrap.HasValue; }
			/// <summary>
			/// NeedGiftWrapPaperエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeNeedGiftWrapPaper() { return this.NeedGiftWrapPaper.HasValue; }
			#endregion

			#region プロパティ
			/// <summary>出荷ステータス</summary>
			[XmlElement("ShipStatus")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.ShipStatus? ShipStatus { get; set; }
			/// <summary>配送方法</summary>
			[XmlElement("ShipMethod", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipMethod { get; set; }
			/// <summary>配送方法名称</summary>
			[XmlElement("ShipMethodName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipMethodName { get; set; }
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
			/// <summary>配送会社 URL</summary>
			[XmlElement("ShipUrl", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipUrl { get; set; }
			/// <summary>翌日配送フラグ</summary>
			[XmlElement("ArriveType")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.ArriveType? ArriveType { get; set; }
			/// <summary>宅配 BOX 利用</summary>
			[XmlElement("DeliveryBoxType")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.DeliveryBoxType? DeliveryBoxType { get; set; }
			/// <summary>出荷日</summary>
			[XmlElement("ShipDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? ShipDate { get; set; }
			/// <summary>着荷日</summary>
			[XmlElement("ArrivalDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? ArrivalDate { get; set; }
			/// <summary>ギフト包装有無</summary>
			[XmlElement("NeedGiftWrap")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? NeedGiftWrap { get; set; }
			/// <summary>ギフト包装種類</summary>
			[XmlElement("GiftWrapType", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string GiftWrapType { get; set; }
			/// <summary>ギフトメッセージ</summary>
			[XmlElement("GiftWrapMessage")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string GiftWrapMessage { get; set; }
			/// <summary>のし有無</summary>
			[XmlElement("NeedGiftWrapPaper")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? NeedGiftWrapPaper { get; set; }
			/// <summary>のし種類</summary>
			[XmlElement("GiftWrapPaperType", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string GiftWrapPaperType { get; set; }
			/// <summary>名入れ</summary>
			[XmlElement("GiftWrapName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string GiftWrapName { get; set; }
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
			/// <summary>お届け先郵便番号</summary>
			[XmlElement("ShipZipCode", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipZipCode { get; set; }
			/// <summary>お届け先都道府県</summary>
			[XmlElement("ShipPrefecture", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipPrefecture { get; set; }
			/// <summary>お届け先市区郡</summary>
			[XmlElement("ShipCity", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipCity { get; set; }
			/// <summary>お届け先住所1</summary>
			[XmlElement("ShipAddress1", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipAddress1 { get; set; }
			/// <summary>お届け先住所2</summary>
			[XmlElement("ShipAddress2", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipAddress2 { get; set; }
			/// <summary>お届け先電話番号</summary>
			[XmlElement("ShipPhoneNumber", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipPhoneNumber { get; set; }
			#endregion
		}
		#endregion

		#region Detail 内部クラス
		/// <summary>
		/// 注文詳細情報クラス
		/// </summary>
		[Serializable]
		public class Detail
		{
			#region Conditional Serialization
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
			/// AdjustmentsエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeAdjustments() { return this.Adjustments.HasValue; }
			/// <summary>
			/// UsePointエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeUsePoint() { return this.UsePoint.HasValue; }
			/// <summary>
			/// UsePaypayAmountエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeUsePaypayAmount() { return this.UsePaypayAmount.HasValue; }
			/// <summary>
			/// TotalPriceエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeTotalPrice() { return this.TotalPrice.HasValue; }
			/// <summary>
			/// GetPointFixDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeGetPointFixDate() { return this.GetPointFixDate.HasValue; }
			/// <summary>
			/// IsGetPointFixAllエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsGetPointFixAll() { return this.IsGetPointFixAll.HasValue; }
			#endregion

			#region プロパティ
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
			/// <summary>調整額</summary>
			[XmlElement("Adjustments")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? Adjustments { get; set; }
			/// <summary>利用ポイント数</summary>
			[XmlElement("UsePoint")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? UsePoint { get; set; }
			/// <summary>PayPay利用額</summary>
			[XmlElement("UsePaypayAmount")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? UsePaypayAmount { get; set; }
			/// <summary>合計金額</summary>
			[XmlElement("TotalPrice")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? TotalPrice { get; set; }
			/// <summary>付与ポイント確定日</summary>
			[XmlElement("GetPointFixDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? GetPointFixDate { get; set; }
			/// <summary>全付与ポイント確定有無</summary>
			[XmlElement("IsGetPointFixAll")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsGetPointFixAll { get; set; }
			#endregion
		}
		#endregion

		#region Item 内部クラス
		/// <summary>
		/// 注文商品情報クラス
		/// </summary>
		[Serializable]
		public class Item
		{
			#region Conditional Serialization
			/// <summary>
			/// IsTaxableエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsTaxable() { return this.IsTaxable.HasValue; }
			/// <summary>
			/// UnitStoreCouponDiscountエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeUnitStoreCouponDiscount() { return this.UnitStoreCouponDiscount.HasValue; }
			/// <summary>
			/// ReleaseDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeReleaseDate() { return this.ReleaseDate.HasValue; }
			/// <summary>
			/// IsShippingFreeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsShippingFree() { return this.IsShippingFree.HasValue; }
			/// <summary>
			/// UnitGetPointエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeUnitGetPoint() { return this.UnitGetPoint.HasValue; }
			/// <summary>
			/// AddonGetPointエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeAddonGetPoint() { return this.AddonGetPoint.HasValue; }
			/// <summary>
			/// AddonGetPointRatioエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeAddonGetPointRatio() { return this.AddonGetPointRatio.HasValue; }
			/// <summary>
			/// YjCardGetPointエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeYjCardGetPoint() { return this.YjCardGetPoint.HasValue; }
			/// <summary>
			/// YjCardGetPointRatioエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeYjCardGetPointRatio() { return this.YjCardGetPointRatio.HasValue; }
			/// <summary>
			/// TaxRatioエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeTaxRatio() { return this.TaxRatio.HasValue; }
			#endregion

			#region プロパティ
			/// <summary>商品ラインID</summary>
			[XmlElement("LineId")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public int LineId { get; set; }
			/// <summary>カタログ商品コード</summary>
			[XmlElement("CtgItemCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string CtgItemCd { get; set; }
			/// <summary>商品コード</summary>
			[XmlElement("ItemCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ItemCd { get; set; }
			/// <summary>商品名</summary>
			[XmlElement("Title", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Title { get; set; }
			/// <summary>属性情報</summary>
			[XmlElement("Attribute", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Attribute { get; set; }
			/// <summary>課税対象</summary>
			[XmlElement("IsTaxable")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsTaxable { get; set; }
			/// <summary>JANコード</summary>
			[XmlElement("Jan", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Jan { get; set; }
			/// <summary>ISBNコード</summary>
			[XmlElement("Isbn", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Isbn { get; set; }
			/// <summary>大大カテゴリコード</summary>
			[XmlElement("LlCateCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string LlCateCd { get; set; }
			/// <summary>大カテゴリコード</summary>
			[XmlElement("LCateCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string LCateCd { get; set; }
			/// <summary>小カテゴリコード</summary>
			[XmlElement("SCateCode", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string SCateCode { get; set; }
			/// <summary>商品単価</summary>
			[XmlElement("UnitPrice")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public decimal UnitPrice { get; set; }
			/// <summary>値引き前の単価</summary>
			[XmlElement("OriginUnitPrice")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public decimal OriginUnitPrice { get; set; }
			/// <summary>商品1つあたりのクーポン値引き額</summary>
			[XmlElement("UnitStoreCouponDiscount")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? UnitStoreCouponDiscount { get; set; }
			/// <summary>数量</summary>
			[XmlElement("Quantity")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public int Quantity { get; set; }
			/// <summary>発売日</summary>
			[XmlElement("ReleaseDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? ReleaseDate { get; set; }
			/// <summary>送料無料フラグ</summary>
			[XmlElement("IsShippingFree")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsShippingFree { get; set; }
			/// <summary>単位付与ポイント数</summary>
			[XmlElement("UnitGetPoint")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Single? UnitGetPoint { get; set; }
			/// <summary>アドオン付与ポイント数</summary>
			[XmlElement("AddonGetPoint")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Single? AddonGetPoint { get; set; }
			/// <summary>アドオン付与ポイント倍率</summary>
			[XmlElement("AddonGetPointRatio")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Single? AddonGetPointRatio { get; set; }
			/// <summary>YJカード付与ポイント数</summary>
			[XmlElement("YjCardGetPoint")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Single? YjCardGetPoint { get; set; }
			/// <summary>YJカード付与ポイント倍率</summary>
			[XmlElement("YjCardGetPointRatio")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Single? YjCardGetPointRatio { get; set; }
			/// <summary>消費税率</summary>
			[XmlElement("TaxRatio")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? TaxRatio { get; set; }
			#endregion
		}
		#endregion

		#region Seller 内部クラス
		/// <summary>
		/// セラー情報クラス
		/// </summary>
		[Serializable]
		public class Seller
		{
			#region プロパティ
			/// <summary>セラーID</summary>
			[XmlElement("SellerId", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string SellerId { get; set; }
			#endregion
		}
		#endregion

		#region Buyer 内部クラス
		/// <summary>
		/// バイヤー情報クラス
		/// </summary>
		[Serializable]
		public class Buyer
		{
			#region Conditional Serialization
			/// <summary>
			/// IsLoginエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsLogin() { return this.IsLogin.HasValue; }
			/// <summary>
			/// YjCardStatusエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeYjCardStatus() { return this.YjCardStatus.HasValue; }
			#endregion

			#region プロパティ
			/// <summary>Yahoo! JAPAN IDログイン有無</summary>
			[XmlElement("IsLogin")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsLogin { get; set; }
			/// <summary>顧客コード</summary>
			[XmlElement("CustomerCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string CustomerCd { get; set; }
			/// <summary>FSPライセンスコード</summary>
			[XmlElement("FspLicenseCode", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string FspLicenseCode { get; set; }
			/// <summary>FSPライセンス名</summary>
			[XmlElement("FspLicenseName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string FspLicenseName { get; set; }
			/// <summary>YJカード会員ステータス</summary>
			[XmlElement("YjCardStatus")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.YjCardStatus? YjCardStatus { get; set; }
			#endregion
		}
		#endregion
	}
}
