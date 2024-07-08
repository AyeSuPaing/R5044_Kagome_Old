/*
=========================================================================================================
  Module      : 注文内容変更API OrderChangeのリクエストクラス(OrderChangeRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace w2.App.Common.LohacoCreatorWebApi.OrderChange
{
	/// <summary>
	/// 注文内容変更API OrderChangeのリクエストクラス
	/// </summary>
	[Serializable]
	public class OrderChangeRequest : BaseRequest
	{
		#region +OrderChangeRequest コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderChangeRequest()
			: base()
		{
			this.Uri = "https://public.lohaco.jp/lohacoApi/v1/orderChange";
			this.Accept = LohacoConstants.ResponseContentType.Xml;
			this.ContentType = LohacoConstants.RequestContentType.Xml;
		}
		/// <summary>
		/// パラメータ付きのコンストラクタ
		/// </summary>
		/// <param name="providerId">ストアアカウント</param>
		public OrderChangeRequest(string providerId)
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

			if ((copy == null) || (copy.Order == null)) return WebApiHelper.SerializeXml(copy);

			// 注文者の個人情報のマスク
			if (copy.Order.Pay != null)
			{
				var pay = copy.Order.Pay;
				if (string.IsNullOrWhiteSpace(pay.BillFirstName) == false) pay.BillFirstName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillLastName) == false) pay.BillLastName = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillFirstNameKana) == false) pay.BillFirstNameKana = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillLastNameKana) == false) pay.BillLastNameKana = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillAddress1) == false) pay.BillAddress1 = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillAddress2) == false) pay.BillAddress2 = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillPhoneNumber) == false) pay.BillPhoneNumber = LohacoConstants.MASK_STRING;
				if (string.IsNullOrWhiteSpace(pay.BillMailAddress) == false) pay.BillMailAddress = LohacoConstants.MASK_STRING;
			}

			// 配送先の個人情報のマスク
			if (copy.Order.Ship != null)
			{
				var ship = copy.Order.Ship;
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
		/// <summary>更新対象指定情報</summary>
		[XmlElement("Target")]
		public ChangeTarget Target { get; set; }
		/// <summary>更新情報</summary>
		[XmlElement("Order")]
		public ChangeOrder Order { get; set; }
		/// <summary>ストアアカウント</summary>
		[XmlElement("SellerId")]
		public string SellerId { get; set; }
		#endregion

		#region ChangeTarget 内部クラス
		/// <summary>
		/// 注文内容変更API 更新対象指定情報クラス
		/// </summary>
		[Serializable]
		public class ChangeTarget
		{
			#region プロパティ
			/// <summary>注文ID（必須）</summary>
			[XmlElement("OrderId")]
			public string OrderId { get; set; }
			/// <summary>更新者名（指定しない場合は system が入る）</summary>
			[XmlElement("OperationUser", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string OperationUser { get; set; }
			#endregion
		}
		#endregion

		#region ChangeOrder 内部クラス
		/// <summary>
		/// 注文内容変更API 更新情報クラス
		/// </summary>
		[Serializable]
		public class ChangeOrder
		{
			#region Conditional Serialization
			/// <summary>
			/// IsSeenエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeIsSeen() { return this.IsSeen.HasValue; }
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
			/// RefundStatusエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeRefundStatus() { return this.RefundStatus.HasValue; }
			#endregion

			#region プロパティ
			/// <summary>閲覧済みフラグ</summary>
			[XmlElement("IsSeen")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? IsSeen { get; set; }
			/// <summary>支払い方法</summary>
			[XmlElement("PayMethod", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string PayMethod { get; set; }
			/// <summary>支払い方法名称</summary>
			[XmlElement("PayMethodName", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string PayMethodName { get; set; }
			/// <summary>注文伝票出力時刻(yyyyMMddHHmmssフォーマット)</summary>
			[XmlElement("PrintSlipTime")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? PrintSlipTime { get; set; }
			/// <summary>納品書出力時刻(yyyyMMddHHmmssフォーマット)</summary>
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
			/// <summary>返金ステータス</summary>
			[XmlElement("RefundStatus")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public LohacoConstants.RefundStatus? RefundStatus { get; set; }
			/// <summary>支払情報</summary>
			[XmlElement("Pay")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public ChangePay Pay { get; set; }
			/// <summary>配送情報</summary>
			[XmlElement("Ship")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public ChangeShip Ship { get; set; }
			/// <summary>明細情報</summary>
			[XmlElement("Detail")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public ChangeDetail Detail { get; set; }
			/// <summary>商品情報</summary>
			[XmlElement("Item")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public ChangeItem Item { get; set; }
			#endregion
		}
		#endregion

		#region ChangePay 内部クラス
		/// <summary>
		/// 注文内容変更API 更新請求情報クラス
		/// </summary>
		[Serializable]
		public class ChangePay
		{
			#region Conditional Serialization
			/// <summary>
			/// PayStatusエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePayStatus() { return this.PayStatus.HasValue; }
			/// <summary>
			/// PayDateエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializePayDate() { return this.PayDate.HasValue; }
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
			/// <summary>入金日(yyyyMMddフォーマット)</summary>
			[XmlElement("PayDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? PayDate { get; set; }
			/// <summary>入金処理備考</summary>
			[XmlElement("PayNotes", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string PayNotes { get; set; }
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

		#region ChangeShip 内部クラス
		/// <summary>
		/// 注文内容変更API 更新配送情報クラス
		/// </summary>
		[Serializable]
		public class ChangeShip
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
			/// NeedGiftWrapエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeNeedGiftWrap() { return this.NeedGiftWrap.HasValue; }
			/// <summary>
			/// NeedGiftWrapPaperエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeNeedGiftWrapPaper() { return this.NeedGiftWrapPaper.HasValue; }
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
			/// <summary>ギフト包装有無</summary>
			[XmlElement("NeedGiftWrap")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Boolean? NeedGiftWrap { get; set; }
			/// <summary>ギフト包装種類</summary>
			[XmlElement("GiftWrapType", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string GiftWrapType { get; set; }
			/// <summary>ギフトメッセージ</summary>
			[XmlElement("GiftWrapMessage", IsNullable = false)]
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
			/// <summary>出荷日</summary>
			[XmlElement("ShipDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? ShipDate { get; set; }
			/// <summary>着荷日</summary>
			[XmlElement("ArrivalDate")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime? ArrivalDate { get; set; }
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

		#region ChangeDetail 内部クラス
		/// <summary>
		/// 注文内容変更API 更新明細情報クラス
		/// </summary>
		[Serializable]
		public class ChangeDetail
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
			/// AdjustmentsエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeAdjustments() { return this.Adjustments.HasValue; }
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
			/// <summary>調整額</summary>
			[XmlElement("Adjustments")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Decimal? Adjustments { get; set; }
			#endregion
		}
		#endregion

		#region ChangeItem 内部クラス
		/// <summary>
		/// 注文内容変更API 更新商品情報クラス
		/// </summary>
		[Serializable]
		public class ChangeItem
		{
			#region Conditional Serialization
			/// <summary>
			/// LineIdエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeLineId() { return this.LineId.HasValue; }
			/// <summary>
			/// QuantityエレメントをXML文字列に出力するかどうか判定
			/// </summary>
			/// <returns>true：null以外の場合出力、false：nullの場合出力しない</returns>
			public bool ShouldSerializeQuantity() { return this.Quantity.HasValue; }
			#endregion

			#region プロパティ
			/// <summary>商品ラインID（商品情報を更新する際は必須）</summary>
			[XmlElement("LineId")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Int32? LineId { get; set; }
			/// <summary>数量</summary>
			[XmlElement("Quantity")]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Int32? Quantity { get; set; }
			#endregion
		}
		#endregion
	}
}
