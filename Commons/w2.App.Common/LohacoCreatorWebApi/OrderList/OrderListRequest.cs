/*
=========================================================================================================
  Module      : 注文検索API OrderListのリクエストクラス(OrderListRequest.cs)
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
	/// 注文検索API OrderListのリクエストクラス
	/// </summary>
	[Serializable]
	public class OrderListRequest : BaseRequest
	{
		#region +OrderListRequest コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderListRequest()
			: base()
		{
			this.Uri = "https://public.lohaco.jp/lohacoApi/v1/orderList";
			this.Accept = LohacoConstants.ResponseContentType.Xml;
			this.ContentType = LohacoConstants.RequestContentType.Xml;
		}
		/// <summary>
		/// スタアアカウントパラメータ付きのコンストラクタ
		/// </summary>
		/// <param name="providerId">ストアアカウント</param>
		public OrderListRequest(string providerId)
			: this()
		{
			this.SellerId = providerId;
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

			if ((copy.Search == null) || (copy.Search.Condition == null)) return WebApiHelper.SerializeXml(this);

			var condition = copy.Search.Condition;
			if (string.IsNullOrWhiteSpace(condition.BillFirstName) == false) condition.BillFirstName = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.BillLastName) == false) condition.BillLastName = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.BillFirstNameKana) == false) condition.BillFirstNameKana = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.BillLastNameKana) == false) condition.BillLastNameKana = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.BillPhoneNumber) == false) condition.BillPhoneNumber = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.BillMailAddress) == false) condition.BillMailAddress = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.ShipFirstName) == false) condition.ShipFirstName = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.ShipLastName) == false) condition.ShipLastName = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.ShipFirstNameKana) == false) condition.ShipFirstNameKana = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.ShipLastNameKana) == false) condition.ShipLastNameKana = LohacoConstants.MASK_STRING;
			if (string.IsNullOrWhiteSpace(condition.ShipPhoneNumber) == false) condition.ShipPhoneNumber = LohacoConstants.MASK_STRING;

			return WebApiHelper.SerializeXml(copy);
		}
		#endregion

		#region プロパティ
		/// <summary>検索対象注文</summary>
		[XmlElement("Search")]
		public SearchTarget Search { get; set; }
		/// <summary>ストアアカウント</summary>
		[XmlElement("SellerId")]
		public string SellerId { get; set; }
		#endregion

		#region SearchTarget 内部クラス
		/// <summary>
		/// 検索対象注文クラス
		/// </summary>
		[Serializable]
		public class SearchTarget
		{
			#region プロパティ
			/// <summary>最大取得件数（デフォルト：10）</summary>
			[XmlElement("Result")]
			public int Result { get; set; }
			/// <summary>取得開始件数（デフォルト：1）</summary>
			[XmlElement("Start")]
			public int Start { get; set; }
			/// <summary>ソート順</summary>
			[XmlElement("Sort")]
			public string Sort { get; set; }
			/// <summary>レスポンスに含めるFieldをカンマ区切りで指定する</summary>
			[XmlElement("Field")]
			public string Field { get { return string.Join(",", this.Fields.Select(p => Enum.GetName(typeof(LohacoConstants.OrderField), p))); } set { } }
			/// <summary>注文Field一覧</summary>
			[XmlIgnore]
			[JsonIgnore]
			public List<LohacoConstants.OrderField> Fields { get; set; }
			/// <summary>検索条件</summary>
			[XmlElement("Condition")]
			public Condition Condition { get; set; }
			#endregion
		}
		#endregion

		#region Condition 内部クラス
		/// <summary>
		/// 検索条件クラス
		/// </summary>
		[Serializable]
		public class Condition
		{
			#region Conditional Serialization
			/// <summary>
			/// IsActiveエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsActive() { return this.IsActive.HasValue; }
			/// <summary>
			/// IsSeenエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsSeen() { return this.IsSeen.HasValue; }
			/// <summary>
			/// IsPermissionエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsPermission() { return this.IsPermission.HasValue; }
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
			/// OrderStatusエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeOrderStatus() { return this.OrderStatus.HasValue; }
			/// <summary>
			/// PrintSlipFlagエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePrintSlipFlag() { return this.PrintSlipFlag.HasValue; }
			/// <summary>
			/// PrintDeliveryFlagエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePrintDeliveryFlag() { return this.PrintDeliveryFlag.HasValue; }
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
			/// NeedDetailedSlipエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeNeedDetailedSlip() { return this.NeedDetailedSlip.HasValue; }
			/// <summary>
			/// ShipStatusエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeShipStatus() { return this.ShipStatus.HasValue; }
			/// <summary>
			/// ShipRequestDateToエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeShipRequestDateTo() { return this.ShipRequestDateTo.HasValue; }
			/// <summary>
			/// ArriveTypeエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeArriveType() { return this.ArriveType.HasValue; }
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
			/// IsGetPointFixAllエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsGetPointFixAll() { return this.IsGetPointFixAll.HasValue; }
			/// <summary>
			/// IsLoginエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsLogin() { return this.IsLogin.HasValue; }
			#endregion

			#region プロパティ
			/// <summary>注文ID</summary>
			[XmlElement("OrderId", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string OrderId { get; set; }
			/// <summary>デバイス情報</summary>
			[XmlElement("DeviceType", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string DeviceType { get; set; }
			/// <summary>注文有効フラグ</summary>
			[XmlElement("IsActive ")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsActive { get; set; }
			/// <summary>閲覧済みフラグ</summary>
			[XmlElement("IsSeen")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsSeen { get; set; }
			/// <summary>参照可否フラグ</summary>
			[XmlElement("IsPermission")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.PermissionType? IsPermission { get; set; }
			/// <summary>課金フラグ</summary>
			[XmlElement("IsRoyalty")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsRoyalty { get; set; }
			/// <summary>課金確定フラグ</summary>
			[XmlElement("IsRoyaltyFix")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsRoyaltyFix { get; set; }
			/// <summary>注文日時(yyyyMMddHHmmssフォーマット)</summary>
			[XmlElement("OrderTime", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string OrderTime { get; set; }
			/// <summary>注文日時（From）(yyyyMMddHHmmssフォーマット)</summary>
			[XmlElement("OrderTimeFrom", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string OrderTimeFrom { get; set; }
			/// <summary>注文日時（To） (yyyyMMddHHmmssフォーマット)</summary>
			[XmlElement("OrderTimeTo", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string OrderTimeTo { get; set; }
			/// <summary>発売日（From）(yyyyMMddフォーマット)</summary>
			[XmlElement("ReleaseDateFrom", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ReleaseDateFrom { get; set; }
			/// <summary>発売日（To）(yyyyMMddフォーマット)</summary>
			[XmlElement("ReleaseDateTo", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ReleaseDateTo { get; set; }
			/// <summary>注文ステータス</summary>
			[XmlElement("OrderStatus")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.OrderStatus? OrderStatus { get; set; }
			/// <summary>注文伝票出力有無</summary>
			[XmlElement("PrintSlipFlag")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? PrintSlipFlag { get; set; }
			/// <summary>納品書出力有無</summary>
			[XmlElement("PrintDeliveryFlag")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? PrintDeliveryFlag { get; set; }
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
			/// <summary>ご請求先電話番号</summary>
			[XmlElement("BillPhoneNumber", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillPhoneNumber { get; set; }
			/// <summary>ご請求先メールアドレス</summary>
			[XmlElement("BillMailAddress", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string BillMailAddress { get; set; }
			/// <summary>出荷ステータス</summary>
			[XmlElement("ShipStatus")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.ShipStatus? ShipStatus { get; set; }
			/// <summary>配送方法</summary>
			[XmlElement("ShipMethod", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipMethod { get; set; }
			/// <summary>配送希望日（From）(yyyyMMddフォーマット)</summary>
			[XmlElement("ShipRequestDateFrom", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipRequestDateFrom { get; set; }
			/// <summary>配送希望日（To）(yyyyMMddフォーマット)</summary>
			[XmlElement("ShipRequestDateTo")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.SpecifyShippingDateType? ShipRequestDateTo { get; set; }
			/// <summary>配送伝票番号</summary>
			[XmlElement("ShipInvoiceNumber", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipInvoiceNumber { get; set; }
			/// <summary>翌日配送フラグ</summary>
			[XmlElement("ArriveType")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.ArriveType? ArriveType { get; set; }
			/// <summary>出荷日(yyyyMMddフォーマット)</summary>
			[XmlElement("ShipDate", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipDate { get; set; }
			/// <summary>出荷日（From）(yyyyMMddフォーマット)</summary>
			[XmlElement("ShipDateFrom", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipDateFrom { get; set; }
			/// <summary>出荷日（To）(yyyyMMddフォーマット)</summary>
			[XmlElement("ShipDateTo", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipDateTo { get; set; }
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
			/// <summary>お届け先郵便番号</summary>
			[XmlElement("ShipZipCode", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipZipCode { get; set; }
			/// <summary>お届け先都道府県</summary>
			[XmlElement("ShipPrefecture", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipPrefecture { get; set; }
			/// <summary>お届け先電話番号</summary>
			[XmlElement("ShipPhoneNumber", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ShipPhoneNumber { get; set; }
			/// <summary>全付与ポイント確定有無</summary>
			[XmlElement("IsGetPointFixAll")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsGetPointFixAll { get; set; }
			/// <summary>商品コード</summary>
			[XmlElement("ItemCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ItemCd { get; set; }
			/// <summary>商品名</summary>
			[XmlElement("Title", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Title { get; set; }
			/// <summary>付与ポイント確定日（From）(yyyyMMddフォーマット)</summary>
			[XmlElement("GetPointFixDateFrom", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string GetPointFixDateFrom { get; set; }
			/// <summary>付与ポイント確定日（To） (yyyyMMddフォーマット)</summary>
			[XmlElement("GetPointFixDateTo", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string GetPointFixDateTo { get; set; }
			/// <summary>ストアアカウント（必須） </summary>
			[XmlElement("SellerId", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string SellerId { get; set; }
			/// <summary>Yahoo! JAPAN IDログイン有無</summary>
			[XmlElement("IsLogin")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsLogin { get; set; }
			/// <summary>顧客コード </summary>
			[XmlElement("CustomerCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string CustomerCd { get; set; }
			#endregion
		}
		#endregion
	}
}
