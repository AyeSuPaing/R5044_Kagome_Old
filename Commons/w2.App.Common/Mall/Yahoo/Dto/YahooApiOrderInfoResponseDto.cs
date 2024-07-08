/*
=========================================================================================================
  Module      : YAHOO API  注文詳細API リクエストDTO クラス(YahooApiOrderInfoResponseDto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Reflection;
using System.Xml.Serialization;

namespace w2.App.Common.Mall.Yahoo.Dto
{
	/// <summary>
	/// YAHOO API  注文詳細API リクエストDTO クラス
	/// </summary>
	[Serializable]
	[XmlRoot("ResultSet")]
	public class YahooApiOrderInfoResponseDto
	{
		/// <summary>OrderInfoクラスのプロパティ</summary>
		private static PropertyInfo[] s_orderInfoProperties = typeof(OrderInfo).GetProperties();
		/// <summary>Payクラスのプロパティ</summary>
		private static PropertyInfo[] s_payProperties = typeof(Pay).GetProperties();
		/// <summary>Shipクラスのプロパティ</summary>
		private static PropertyInfo[] s_shipProperties = typeof(Ship).GetProperties();
		/// <summary>Detailクラスのプロパティ</summary>
		private static PropertyInfo[] s_detailProperties = typeof(Detail).GetProperties();
		/// <summary>Sellerクラスのプロパティ</summary>
		private static PropertyInfo[] s_sellerProperties = typeof(Seller).GetProperties();
		/// <summary>Buyerクラスのプロパティ</summary>
		private static PropertyInfo[] s_buyerProperties = typeof(Buyer).GetProperties();

		/// <summary>結果</summary>
		[XmlElement("Result")]
		public Result Result { get; set; }
		/// <summary>該当件数の総個数です。(常に0か1)</summary>
		[XmlAttribute("totalResultsAvailable")]
		public string TotalResultsAvailable { get; set; } = "";
		/// <summary>返却され、かつマッチした件数</summary>
		[XmlAttribute("totalResultsReturned")]
		public string TotalResultsReturned { get; set; } = "";
		/// <summary>データの取得開始位置です。(常に0か1)</summary>
		[XmlAttribute("firstResultPosition")]
		public string FirstResultPosition { get; set; } = "";

		/// <summary>
		/// OrderInfoクラスのプロパティを取得
		/// </summary>
		/// <returns>OrderInfoクラスのプロパティ</returns>
		public static PropertyInfo[] GetOrderInfoProperties() => s_orderInfoProperties;

		/// <summary>
		/// Payクラスのプロパティを取得
		/// </summary>
		/// <returns>Payクラスのプロパティ</returns>
		public static PropertyInfo[] GetPayProperties() => s_payProperties;
		
		/// <summary>
		/// Shipクラスのプロパティを取得
		/// </summary>
		/// <returns>Shipクラスのプロパティ</returns>
		public static PropertyInfo[] GetShipProperties() => s_shipProperties;
		
		/// <summary>
		/// Detailクラスのプロパティを取得
		/// </summary>
		/// <returns>Detailクラスのプロパティ</returns>
		public static PropertyInfo[] GetDetailProperties() => s_detailProperties;
		
		/// <summary>
		/// Sellerクラスのプロパティを取得
		/// </summary>
		/// <returns>Sellerクラスのプロパティ</returns>
		public static PropertyInfo[] GetSellerProperties() => s_sellerProperties;
		
		/// <summary>
		/// Buyerクラスのプロパティを取得
		/// </summary>
		/// <returns>Buyerクラスのプロパティ</returns>
		public static PropertyInfo[] GetBuyerProperties() => s_buyerProperties;
	}

	/// <summary>
	/// 結果
	/// </summary>
	[Serializable]
	public class Result
	{
		/// <summary>取得成否(OK/NG)</summary>
		[XmlElement("Status")]
		public string Status { get; set; } = "";
		/// <summary>注文情報</summary>
		[XmlElement("OrderInfo")]
		public OrderInfo OrderInfo { get; set; }
		/// <summary>警告情報</summary>
		[XmlElement("Warning")]
		public Warning Warning { get; set; }
	}

	/// <summary>
	/// 注文情報
	/// </summary>
	[Serializable]
	public class OrderInfo
	{
		/// <summary>注文ID</summary>
		[XmlElement("OrderId")]
		public string OrderId { get; set; } = "";
		/// <summary>バージョン</summary>
		[XmlElement("Version")]
		public string Version { get; set; } = "";
		/// <summary>分割元注文ID</summary>
		[XmlElement("ParentOrderId")]
		public string ParentOrderId { get; set; } = "";
		/// <summary>分割後注文ID</summary>
		[XmlElement("ChildOrderId")]
		public string ChildOrderId { get; set; } = "";
		/// <summary>デバイス種別</summary>
		[XmlElement("DeviceType")]
		public string DeviceType { get; set; } = "";
		/// <summary>携帯キャリア名</summary>
		[XmlElement("MobileCarrierName")]
		public string MobileCarrierName { get; set; } = "";
		/// <summary>注文有効フラグ</summary>
		[XmlElement("IsActive")]
		public string IsActive { get; set; } = "";
		/// <summary>閲覧済みフラグ</summary>
		[XmlElement("IsSeen")]
		public string IsSeen { get; set; } = "";
		/// <summary>分割フラグ</summary>
		[XmlElement("IsSplit")]
		public string IsSplit { get; set; } = "";
		/// <summary>キャンセル理由</summary>
		[XmlElement("CancelReason")]
		public string CancelReason { get; set; } = "";
		/// <summary>キャンセル理由詳細</summary>
		[XmlElement("CancelReasonDetail")]
		public string CancelReasonDetail { get; set; } = "";
		/// <summary>ロイヤルティフラグ</summary>
		[XmlElement("IsRoyalty")]
		public string IsRoyalty { get; set; } = "";
		/// <summary>ロイヤルティ確定フラグ</summary>
		[XmlElement("IsRoyaltyFix")]
		public string IsRoyaltyFix { get; set; } = "";
		/// <summary>管理者注文フラグ</summary>
		[XmlElement("IsSeller")]
		public string IsSeller { get; set; } = "";
		/// <summary>アフィリエイトフラグ</summary>
		[XmlElement("IsAffiliate")]
		public string IsAffiliate { get; set; } = "";
		/// <summary>評価フラグ（Buyer⇒Seller）</summary>
		[XmlElement("IsRatingB2s")]
		public string IsRatingB2S { get; set; } = "";
		/// <summary>SNLオプトイン</summary>
		[XmlElement("NeedSnl")]
		public string NeedSnl { get; set; } = "";
		/// <summary>注文日時</summary>
		[XmlElement("OrderTime")]
		public string OrderTime { get; set; } = "";
		/// <summary>最終更新日時</summary>
		[XmlElement("LastUpdateTime")]
		public string LastUpdateTime { get; set; } = "";
		/// <summary>いたずらフラグ</summary>
		[XmlElement("Suspect")]
		public string Suspect { get; set; } = "";
		/// <summary>いたずらメッセージ</summary>
		[XmlElement("SuspectMessage")]
		public string SuspectMessage { get; set; } = "";
		/// <summary>注文ステータス</summary>
		[XmlElement("OrderStatus")]
		public string OrderStatus { get; set; } = "";
		/// <summary>ストアステータス</summary>
		[XmlElement("StoreStatus")]
		public string StoreStatus { get; set; } = "";
		/// <summary>ロイヤルティ確定日時</summary>
		[XmlElement("RoyaltyFixTime")]
		public string RoyaltyFixTime { get; set; } = "";
		/// <summary>注文確認メール送信時刻</summary>
		[XmlElement("SendConfirmTime")]
		public string SendConfirmTime { get; set; } = "";
		/// <summary>支払完了メール送信時刻</summary>
		[XmlElement("SendPayTime")]
		public string SendPayTime { get; set; } = "";
		/// <summary>注文伝票出力時刻</summary>
		[XmlElement("PrintSlipTime")]
		public string PrintSlipTime { get; set; } = "";
		/// <summary>納品書出力時刻</summary>
		[XmlElement("PrintDeliveryTime")]
		public string PrintDeliveryTime { get; set; } = "";
		/// <summary>請求書出力時刻</summary>
		[XmlElement("PrintBillTime")]
		public string PrintBillTime { get; set; } = "";
		/// <summary>バイヤーコメント</summary>
		[XmlElement("BuyerComments")]
		public string BuyerComments { get; set; } = "";
		/// <summary>セラーコメント</summary>
		[XmlElement("SellerComments")]
		public string SellerComments { get; set; } = "";
		/// <summary>ストア内メモ</summary>
		[XmlElement("Notes")]
		public string Notes { get; set; } = "";
		/// <summary>更新者</summary>
		[XmlElement("OperationUser")]
		public string OperationUser { get; set; } = "";
		/// <summary>参照元URL（リファラー）</summary>
		[XmlElement("Referer")]
		public string Referer { get; set; } = "";
		/// <summary>入力ポイント</summary>
		[XmlElement("EntryPoint")]
		public string EntryPoint { get; set; } = "";
		/// <summary>調査用リンク</summary>
		[XmlElement("Clink")]
		public string Clink { get; set; } = "";
		/// <summary>履歴ID</summary>
		[XmlElement("HistoryId")]
		public string HistoryId { get; set; } = "";
		/// <summary>クーポン利用ID</summary>
		[XmlElement("UsageId")]
		public string UsageId { get; set; } = "";
		/// <summary>使用したクーポン情報</summary>
		[XmlElement("UseCouponData")]
		public string UseCouponData { get; set; } = "";
		/// <summary>クーポン合計値引き額</summary>
		[XmlElement("TotalCouponDiscount")]
		public string TotalCouponDiscount { get; set; } = "";
		/// <summary>送料無料クーポン利用有無</summary>
		[XmlElement("ShippingCouponFlg")]
		public string ShippingCouponFlg { get; set; } = "";
		/// <summary>送料無料クーポンを適用したときの送料の値引き額</summary>
		[XmlElement("ShippingCouponDiscount")]
		public string ShippingCouponDiscount { get; set; } = "";
		/// <summary>後付与ポイント内訳</summary>
		[XmlElement("CampaignPoints")]
		public string CampaignPoints { get; set; } = "";
		/// <summary>複数配送注文フラグ</summary>
		[XmlElement("IsMultiShip")]
		public string IsMultiShip { get; set; } = "";
		/// <summary>複数配送注文ID</summary>
		[XmlElement("MultiShipId")]
		public string MultiShipId { get; set; } = "";
		/// <summary>読み取り専用</summary>
		[XmlElement("IsReadOnly")]
		public string IsReadOnly { get; set; } = "";
		/// <summary>第1類医薬品フラグ</summary>
		[XmlElement("IsFirstClassDrugIncludes")]
		public string IsFirstClassDrugIncludes { get; set; } = "";
		/// <summary>第1類医薬品承諾フラグ</summary>
		[XmlElement("IsFirstClassDrugAgreement")]
		public string IsFirstClassDrugAgreement { get; set; } = "";
		/// <summary>無料プレゼント(ウェルカムギフト) 含有フラグ</summary>
		[XmlElement("IsWelcomeGiftIncludes")]
		public string IsWelcomeGiftIncludes { get; set; } = "";
		/// <summary>ヤマト連携ステータス</summary>
		[XmlElement("YamatoCoopStatus")]
		public string YamatoCoopStatus { get; set; } = "";
		/// <summary>不正保留ステータス</summary>
		[XmlElement("FraudHoldStatus")]
		public string FraudHoldStatus { get; set; } = "";
		/// <summary>orderList公開日時</summary>
		[XmlElement("PublicationTime")]
		public string PublicationTime { get; set; } = "";
		/// <summary>ヤフオク!併売フラグ</summary>
		[XmlElement("IsYahooAuctionOrder")]
		public string IsYahooAuctionOrder { get; set; } = "";
		/// <summary>ヤフオク!管理番号</summary>
		[XmlElement("YahooAuctionMerchantId")]
		public string YahooAuctionMerchantId { get; set; } = "";
		/// <summary>オークションID</summary>
		[XmlElement("YahooAuctionId")]
		public string YahooAuctionId { get; set; } = "";
		/// <summary>ヤフオク!購入後決済フラグ</summary>
		[XmlElement("IsYahooAuctionDeferred")]
		public string IsYahooAuctionDeferred { get; set; } = "";
		/// <summary>ヤフオク!カテゴリ種別</summary>
		[XmlElement("YahooAuctionCategoryType")]
		public string YahooAuctionCategoryType { get; set; } = "";
		/// <summary>ヤフオク!落札種別</summary>
		[XmlElement("YahooAuctionBidType")]
		public string YahooAuctionBidType { get; set; } = "";
		/// <summary>ヤフオク!同梱タイプ</summary>
		[XmlElement("YahooAuctionBundleType")]
		public string YahooAuctionBundleType { get; set; } = "";
		/// <summary>優良店判定</summary>
		[XmlElement("GoodStoreStatus")]
		public string GoodStoreStatus { get; set; } = "";
		/// <summary>注文時点の優良店特典適応状態</summary>
		[XmlElement("CurrentGoodStoreBenefitApply")]
		public string CurrentGoodStoreBenefitApply { get; set; } = "";
		/// <summary>注文時点のプラン適応状況</summary>
		[XmlElement("CurrentPromoPkgApply")]
		public string CurrentPromoPkgApply { get; set; } = "";
		/// <summary>LINEギフト注文ID</summary>
		[XmlElement("LineGiftOrderId")]
		public string LineGiftOrderId { get; set; } = "";
		/// <summary>LINEギフト注文フラグ</summary>
		[XmlElement("IsLineGiftOrder")]
		public string IsLineGiftOrder { get; set; } = "";
		/// <summary>決済情報</summary>
		[XmlElement("Pay")]
		public Pay Pay { get; set; }
		/// <summary>配送情報</summary>
		[XmlElement("Ship")]
		public Ship Ship { get; set; }
		///// <summary>商品</summary> // デシリアライズするときに最大サイズを超えるため、コメントアウトする
		//[XmlElement("Item")] 
		//public Item Item { get; set; } = "";
		/// <summary>詳細</summary>
		[XmlElement("Detail")]
		public Detail Detail { get; set; }
		/// <summary>セラー</summary>
		[XmlElement("Seller")]
		public Seller Seller { get; set; }
		/// <summary>バイヤー</summary>
		[XmlElement("Buyer")]
		public Buyer Buyer { get; set; }
	}

	/// <summary>
	/// 決済
	/// </summary>
	[Serializable]
	public class Pay
	{
		/// <summary>入金ステータス</summary>
		[XmlElement("PayStatus")]
		public string PayStatus { get; set; } = "";
		/// <summary>決済ステータス</summary>
		[XmlElement("SettleStatus")]
		public string SettleStatus { get; set; } = "";
		/// <summary>支払い分類</summary>
		[XmlElement("PayType")]
		public string PayType { get; set; } = "";
		/// <summary>支払い種別</summary>
		[XmlElement("PayKind")]
		public string PayKind { get; set; } = "";
		/// <summary>支払い方法</summary>
		[XmlElement("PayMethod")]
		public string PayMethod { get; set; } = "";
		/// <summary>支払い方法名称</summary>
		[XmlElement("PayMethodName")]
		public string PayMethodName { get; set; } = "";
		/// <summary>ストア負担決済手数料</summary>
		[XmlElement("SellerHandlingCharge")]
		public string SellerHandlingCharge { get; set; } = "";
		/// <summary>支払い日時</summary>
		[XmlElement("PayActionTime")]
		public string PayActionTime { get; set; } = "";
		/// <summary>入金日</summary>
		[XmlElement("PayDate")]
		public string PayDate { get; set; } = "";
		/// <summary>入金処理備考</summary>
		[XmlElement("PayNotes")]
		public string PayNotes { get; set; } = "";
		/// <summary>決済ID</summary>
		[XmlElement("SettleId")]
		public string SettleId { get; set; } = "";
		/// <summary>カード種別</summary>
		[XmlElement("CardBrand")]
		public string CardBrand { get; set; } = "";
		/// <summary>クレジットカード番号</summary>
		[XmlElement("CardNumber")]
		public string CardNumber { get; set; } = "";
		/// <summary>カード番号下４けた</summary>
		[XmlElement("CardNumberLast4")]
		public string CardNumberLast4 { get; set; } = "";
		/// <summary>カード有効期限（年）</summary>
		[XmlElement("CardExpireYear")]
		public string CardExpireYear { get; set; } = "";
		/// <summary>カード有効期限（月）</summary>
		[XmlElement("CardExpireMonth")]
		public string CardExpireMonth { get; set; } = "";
		/// <summary>カード支払い区分</summary>
		[XmlElement("CardPayType")]
		public string CardPayType { get; set; } = "";
		/// <summary>カード名義人姓名（独自カード用）</summary>
		[XmlElement("CardHolderName")]
		public string CardHolderName { get; set; } = "";
		/// <summary>カード支払回数</summary>
		[XmlElement("CardPayCount")]
		public string CardPayCount { get; set; } = "";
		/// <summary>カード生年月日</summary>
		[XmlElement("CardBirthDay")]
		public string CardBirthDay { get; set; } = "";
		/// <summary>Yahoo! JAPAN JCBカード利用有無</summary>
		[XmlElement("UseYahooCard")]
		public string UseYahooCard { get; set; } = "";
		/// <summary>ウォレット利用有無</summary>
		[XmlElement("UseWallet")]
		public string UseWallet { get; set; } = "";
		/// <summary>請求書有無</summary>
		[XmlElement("NeedBillSlip")]
		public string NeedBillSlip { get; set; } = "";
		/// <summary>明細書有無</summary>
		[XmlElement("NeedDetailedSlip")]
		public string NeedDetailedSlip { get; set; } = "";
		/// <summary>領収書有無</summary>
		[XmlElement("NeedReceipt")]
		public string NeedReceipt { get; set; } = "";
		/// <summary>年齢確認フィールド名</summary>
		[XmlElement("AgeConfirmField")]
		public string AgeConfirmField { get; set; } = "";
		/// <summary>年齢確認入力値</summary>
		[XmlElement("AgeConfirmValue")]
		public string AgeConfirmValue { get; set; } = "";
		/// <summary>年齢確認チェック有無</summary>
		[XmlElement("AgeConfirmCheck")]
		public string AgeConfirmCheck { get; set; } = "";
		/// <summary>ご請求先住所引用元</summary>
		[XmlElement("BillAddressFrom")]
		public string BillAddressFrom { get; set; } = "";
		/// <summary>ご請求先名前</summary>
		[XmlElement("BillFirstName")]
		public string BillFirstName { get; set; } = "";
		/// <summary>ご請求先名前カナ</summary>
		[XmlElement("BillFirstNameKana")]
		public string BillFirstNameKana { get; set; } = "";
		/// <summary>ご請求先名字</summary>
		[XmlElement("BillLastName")]
		public string BillLastName { get; set; } = "";
		/// <summary>ご請求先名字カナ</summary>
		[XmlElement("BillLastNameKana")]
		public string BillLastNameKana { get; set; } = "";
		/// <summary>ご請求先郵便番号</summary>
		[XmlElement("BillZipCode")]
		public string BillZipCode { get; set; } = "";
		/// <summary>ご請求先都道府県</summary>
		[XmlElement("BillPrefecture")]
		public string BillPrefecture { get; set; } = "";
		/// <summary>ご請求先都道府県フリガナ</summary>
		[XmlElement("BillPrefectureKana")]
		public string BillPrefectureKana { get; set; } = "";
		/// <summary>ご請求先市区郡</summary>
		[XmlElement("BillCity")]
		public string BillCity { get; set; } = "";
		/// <summary>ご請求先市区郡フリガナ</summary>
		[XmlElement("BillCityKana")]
		public string BillCityKana { get; set; } = "";
		/// <summary>ご請求先住所1</summary>
		[XmlElement("BillAddress1")]
		public string BillAddress1 { get; set; } = "";
		/// <summary>ご請求先住所1フリガナ</summary>
		[XmlElement("BillAddress1Kana")]
		public string BillAddress1Kana { get; set; } = "";
		/// <summary>ご請求先住所2</summary>
		[XmlElement("BillAddress2")]
		public string BillAddress2 { get; set; } = "";
		/// <summary>ご請求先住所2フリガナ</summary>
		[XmlElement("BillAddress2Kana")]
		public string BillAddress2Kana { get; set; } = "";
		/// <summary>ご請求先電話番号</summary>
		[XmlElement("BillPhoneNumber")]
		public string BillPhoneNumber { get; set; } = "";
		/// <summary>ご請求先電話番号（緊急）</summary>
		[XmlElement("BillEmgPhoneNumber")]
		public string BillEmgPhoneNumber { get; set; } = "";
		/// <summary>ご請求先メールアドレス</summary>
		[XmlElement("BillMailAddress")]
		public string BillMailAddress { get; set; } = "";
		/// <summary>ご請求先所属1フィールド名</summary>
		[XmlElement("BillSection1Field")]
		public string BillSection1Field { get; set; } = "";
		/// <summary>ご請求先所属1入力情報</summary>
		[XmlElement("BillSection1Value")]
		public string BillSection1Value { get; set; } = "";
		/// <summary>ご請求先所属2フィールド名</summary>
		[XmlElement("BillSection2Field")]
		public string BillSection2Field { get; set; } = "";
		/// <summary>ご請求先所属2入力情報</summary>
		[XmlElement("BillSection2Value")]
		public string BillSection2Value { get; set; } = "";
		/// <summary>支払番号</summary>
		[XmlElement("PayNo")]
		public string PayNo { get; set; } = "";
		/// <summary>支払番号発行日時</summary>
		[XmlElement("PayNoIssueDate")]
		public string PayNoIssueDate { get; set; } = "";
		/// <summary>確認番号</summary>
		[XmlElement("ConfirmNumber")]
		public string ConfirmNumber { get; set; } = "";
		/// <summary>支払期限日時</summary>
		[XmlElement("PaymentTerm")]
		public string PaymentTerm { get; set; } = "";
		/// <summary>ApplePay利用有無</summary>
		[XmlElement("IsApplePay")]
		public string IsApplePay { get; set; } = "";
		/// <summary>LINEギフト支払い方法名称</summary>
		[XmlElement("LineGiftPayMethodName")]
		public string LineGiftPayMethodName { get; set; } = "";
		/// <summary>併用支払い分類</summary>
		[XmlElement("CombinedPayType")]
		public string CombinedPayType { get; set; } = "";
		/// <summary>併用お支払い方法種別	</summary>
		[XmlElement("CombinedPayKind")]
		public string CombinedPayKind { get; set; } = "";
		/// <summary>併用お支払い方法	</summary>
		[XmlElement("CombinedPayMethod")]
		public string CombinedPayMethod { get; set; } = "";
		/// <summary>併用お支払い方法名称</summary>
		[XmlElement("CombinedPayMethodName")]
		public string CombinedPayMethodName { get; set; } = "";
	}

	/// <summary>
	/// 配送
	/// </summary>
	[Serializable]
	public class Ship
	{
		/// <summary>出荷ステータス</summary>
		[XmlElement("ShipStatus")]
		public string ShipStatus { get; set; } = "";
		/// <summary>配送方法</summary>
		[XmlElement("ShipMethod")]
		public string ShipMethod { get; set; } = "";
		/// <summary>配送方法名称</summary>
		[XmlElement("ShipMethodName")]
		public string ShipMethodName { get; set; } = "";
		/// <summary>配送希望日</summary>
		[XmlElement("ShipRequestDate")]
		public string ShipRequestDate { get; set; } = "";
		/// <summary>配送希望時間</summary>
		[XmlElement("ShipRequestTime")]
		public string ShipRequestTime { get; set; } = "";
		/// <summary>配送メモ</summary>
		[XmlElement("ShipNotes")]
		public string ShipNotes { get; set; } = "";
		/// <summary>配送会社</summary>
		[XmlElement("ShipCompanyCode")]
		public string ShipCompanyCode { get; set; } = "";
		/// <summary>受取店舗コード</summary>
		[XmlElement("ReceiveShopCode")]
		public string ReceiveShopCode { get; set; } = "";
		/// <summary>配送伝票番号１</summary>
		[XmlElement("ShipInvoiceNumber1")]
		public string ShipInvoiceNumber1 { get; set; } = "";
		/// <summary>配送伝票番号2</summary>
		[XmlElement("ShipInvoiceNumber2")]
		public string ShipInvoiceNumber2 { get; set; } = "";
		/// <summary>伝票番号なし理由</summary>
		[XmlElement("ShipInvoiceNumberEmptyReason")]
		public string ShipInvoiceNumberEmptyReason { get; set; } = "";
		/// <summary>配送会社URL</summary>
		[XmlElement("ShipUrl")]
		public string ShipUrl { get; set; } = "";
		/// <summary>きょうつく、あすつく</summary>
		[XmlElement("ArriveType")]
		public string ArriveType { get; set; } = "";
		/// <summary>出荷日</summary>
		[XmlElement("ShipDate")]
		public string ShipDate { get; set; } = "";
		/// <summary>着荷日</summary>
		[XmlElement("ArrivalDate")]
		public string ArrivalDate { get; set; } = "";
		/// <summary>送料着払いフラグ</summary>
		[XmlElement("IsCashOnDelivery")]
		public string IsCashOnDelivery { get; set; } = "";
		/// <summary>ギフト包装有無</summary>
		[XmlElement("NeedGiftWrap")]
		public string NeedGiftWrap { get; set; } = "";
		/// <summary>ギフト包装コード</summary>
		[XmlElement("GiftWrapCode")]
		public string GiftWrapCode { get; set; } = "";
		/// <summary>ギフト包装種類</summary>
		[XmlElement("GiftWrapType")]
		public string GiftWrapType { get; set; } = "";
		/// <summary>ギフトメッセージ</summary>
		[XmlElement("GiftWrapMessage")]
		public string GiftWrapMessage { get; set; } = "";
		/// <summary>のし有無</summary>
		[XmlElement("NeedGiftWrapPaper")]
		public string NeedGiftWrapPaper { get; set; } = "";
		/// <summary>のし種類</summary>
		[XmlElement("GiftWrapPaperType")]
		public string GiftWrapPaperType { get; set; } = "";
		/// <summary>名入れ</summary>
		[XmlElement("GiftWrapName")]
		public string GiftWrapName { get; set; } = "";
		/// <summary>オプションフィールドキー情報（フィールド名）</summary>
		[XmlElement("Option1Field")]
		public string Option1Field { get; set; } = "";
		/// <summary>オプションフィールドキー情報（設定）</summary>
		[XmlElement("Option1Type")]
		public string Option1Type { get; set; } = "";
		/// <summary>オプションフィールド入力内容</summary>
		[XmlElement("Option1Value")]
		public string Option1Value { get; set; } = "";
		/// <summary>オプションフィールドキー情報（フィールド名）</summary>
		[XmlElement("Option2Field")]
		public string Option2Field { get; set; } = "";
		/// <summary>オプションフィールドキー情報（設定）</summary>
		[XmlElement("Option2Type")]
		public string Option2Type { get; set; } = "";
		/// <summary>オプションフィールド入力内容</summary>
		[XmlElement("Option2Value")]
		public string Option2Value { get; set; } = "";
		/// <summary>お届け先名前</summary>
		[XmlElement("ShipFirstName")]
		public string ShipFirstName { get; set; } = "";
		/// <summary>お届け先名前カナ</summary>
		[XmlElement("ShipFirstNameKana")]
		public string ShipFirstNameKana { get; set; } = "";
		/// <summary>お届け先名字</summary>
		[XmlElement("ShipLastName")]
		public string ShipLastName { get; set; } = "";
		/// <summary>お届け先名字カナ</summary>
		[XmlElement("ShipLastNameKana")]
		public string ShipLastNameKana { get; set; } = "";
		/// <summary>お届け先郵便番号</summary>
		[XmlElement("ShipZipCode")]
		public string ShipZipCode { get; set; } = "";
		/// <summary>お届け先都道府県</summary>
		[XmlElement("ShipPrefecture")]
		public string ShipPrefecture { get; set; } = "";
		/// <summary>お届け先都道府県カナ</summary>
		[XmlElement("ShipPrefectureKana")]
		public string ShipPrefectureKana { get; set; } = "";
		/// <summary>お届け先市区郡</summary>
		[XmlElement("ShipCity")]
		public string ShipCity { get; set; } = "";
		/// <summary>お届け先市区郡フリガナ</summary>
		[XmlElement("ShipCityKana")]
		public string ShipCityKana { get; set; } = "";
		/// <summary>お届け先住所1</summary>
		[XmlElement("ShipAddress1")]
		public string ShipAddress1 { get; set; } = "";
		/// <summary>お届け先住所1フリガナ</summary>
		[XmlElement("ShipAddress1Kana")]
		public string ShipAddress1Kana { get; set; } = "";
		/// <summary>お届け先住所2</summary>
		[XmlElement("ShipAddress2")]
		public string ShipAddress2 { get; set; } = "";
		/// <summary>お届け先住所2フリガナ</summary>
		[XmlElement("ShipAddress2Kana")]
		public string ShipAddress2Kana { get; set; } = "";
		/// <summary>お届け先電話番号</summary>
		[XmlElement("ShipPhoneNumber")]
		public string ShipPhoneNumber { get; set; } = "";
		/// <summary>お届け先緊急連絡先</summary>
		[XmlElement("ShipEmgPhoneNumber")]
		public string ShipEmgPhoneNumber { get; set; } = "";
		/// <summary>お届け先所属1フィールド名</summary>
		[XmlElement("ShipSection1Field")]
		public string ShipSection1Field { get; set; } = "";
		/// <summary>お届け先所属1値</summary>
		[XmlElement("ShipSection1Value")]
		public string ShipSection1Value { get; set; } = "";
		/// <summary>お届け先所属2フィールド名</summary>
		[XmlElement("ShipSection2Field")]
		public string ShipSection2Field { get; set; } = "";
		/// <summary>お届け先所属2値</summary>
		[XmlElement("ShipSection2Value")]
		public string ShipSection2Value { get; set; } = "";
		/// <summary>自宅外配送受取種別</summary>
		[XmlElement("ReceiveSatelliteType")]
		public string ReceiveSatelliteType { get; set; } = "";
		/// <summary>自宅外配送受取決済手段</summary>
		[XmlElement("ReceiveSatelliteSettleMethod")]
		public string ReceiveSatelliteSettleMethod { get; set; } = "";
		/// <summary>自宅外配送受取手段</summary>
		[XmlElement("ReceiveSatelliteMethod")]
		public string ReceiveSatelliteMethod { get; set; } = "";
		/// <summary>自宅外配送受取会社名</summary>
		[XmlElement("ReceiveSatelliteCompanyName")]
		public string ReceiveSatelliteCompanyName { get; set; } = "";
		/// <summary>自宅外配送受取店舗コード</summary>
		[XmlElement("ReceiveSatelliteShopCode")]
		public string ReceiveSatelliteShopCode { get; set; } = "";
		/// <summary>自宅外配送受取店舗名</summary>
		[XmlElement("ReceiveSatelliteShopName")]
		public string ReceiveSatelliteShopName { get; set; } = "";
		/// <summary>自宅外配送指定配送種類</summary>
		[XmlElement("ReceiveSatelliteShipKind")]
		public string ReceiveSatelliteShipKind { get; set; } = "";
		/// <summary>自宅外配送通販業者コード</summary>
		[XmlElement("ReceiveSatelliteYahooCode")]
		public string ReceiveSatelliteYahooCode { get; set; } = "";
		/// <summary>自宅外配送受取認証番号</summary>
		[XmlElement("ReceiveSatelliteCertificationNumber")]
		public string ReceiveSatelliteCertificationNumber { get; set; } = "";
		/// <summary>集荷日</summary>
		[XmlElement("CollectionDate")]
		public string CollectionDate { get; set; } = "";
		/// <summary>代引き用消費税額</summary>
		[XmlElement("CashOnDeliveryTax")]
		public string CashOnDeliveryTax { get; set; } = "";
		/// <summary>出荷個口数</summary>
		[XmlElement("NumberUnitsShipped")]
		public string NumberUnitsShipped { get; set; } = "";
		/// <summary>配送希望時間帯番号</summary>
		[XmlElement("ShipRequestTimeZoneCode")]
		public string ShipRequestTimeZoneCode { get; set; } = "";
		/// <summary>出荷指示区分</summary>
		[XmlElement("ShipInstructType")]
		public string ShipInstructType { get; set; } = "";
		/// <summary>出荷指示ステータス</summary>
		[XmlElement("ShipInstructStatus")]
		public string ShipInstructStatus { get; set; } = "";
		/// <summary>店頭注文種別</summary>
		[XmlElement("ReceiveShopType")]
		public string ReceiveShopType { get; set; } = "";
		/// <summary>配送元店頭名</summary>
		[XmlElement("ReceiveShopName")]
		public string ReceiveShopName { get; set; } = "";
		/// <summary>優良配送フラグ</summary>
		[XmlElement("ExcellentDelivery")]
		public string ExcellentDelivery { get; set; } = "";
		/// <summary>EAZY注文フラグ</summary>
		[XmlElement("IsEazy")]
		public string IsEazy { get; set; } = "";
		/// <summary>EAZYコード</summary>
		[XmlElement("EazyDeliveryCode")]
		public string EazyDeliveryCode { get; set; } = "";
		/// <summary>EAZY受け取り場所名</summary>
		[XmlElement("EazyDeliveryName")]
		public string EazyDeliveryName { get; set; } = "";
		/// <summary>定期購入フラグ</summary>
		[XmlElement("IsSubscription")]
		public string IsSubscription { get; set; } = "";
		/// <summary>定期購入親ID</summary>
		[XmlElement("SubscriptionId")]
		public string SubscriptionId { get; set; } = "";
		/// <summary>定期購入継続回数</summary>
		[XmlElement("SubscriptionContinueCount")]
		public string SubscriptionContinueCount { get; set; } = "";
		/// <summary>配送サイクル</summary>
		[XmlElement("SubscriptionCycleType")]
		public string SubscriptionCycleType { get; set; } = "";
		/// <summary>配送サイクル</summary>
		[XmlElement("SubscriptionCycleDate")]
		public string SubscriptionCycleDate { get; set; } = "";
		/// <summary>LINEギフト出荷可能フラグ</summary>
		[XmlElement("IsLineGiftShippable")]
		public string IsLineGiftShippable { get; set; } = "";
		/// <summary>発送期限</summary>
		[XmlElement("ShippingDeadline")]
		public string ShippingDeadline { get; set; } = "";
		/// <summary>利用ギフト券情報</summary>
		[XmlElement("UseGiftCardData")]
		public string UseGiftCardData { get; set; } = "";
	}

	/// <summary>
	/// 明細
	/// </summary>
	[Serializable]
	public class Detail
	{
		/// <summary>手数料</summary>
		[XmlElement("PayCharge")]
		public string PayCharge { get; set; } = "";
		/// <summary>送料</summary>
		[XmlElement("ShipCharge")]
		public string ShipCharge { get; set; } = "";
		/// <summary>ギフト包装料</summary>
		[XmlElement("GiftWrapCharge")]
		public string GiftWrapCharge { get; set; } = "";
		/// <summary>値引き</summary>
		[XmlElement("Discount")]
		public string Discount { get; set; } = "";
		/// <summary>調整額</summary>
		[XmlElement("Adjustments")]
		public string Adjustments { get; set; } = "";
		/// <summary>決済金額</summary>
		[XmlElement("SettleAmount")]
		public string SettleAmount { get; set; } = "";
		/// <summary>利用ポイント合計</summary>
		[XmlElement("UsePoint")]
		public string UsePoint { get; set; } = "";
		/// <summary>ギフト券利用額</summary>
		[XmlElement("GiftCardDiscount")]
		public string GiftCardDiscount { get; set; } = "";
		/// <summary>合計金額</summary>
		[XmlElement("TotalPrice")]
		public string TotalPrice { get; set; } = "";
		/// <summary>入金金額</summary>
		[XmlElement("SettlePayAmount")]
		public string SettlePayAmount { get; set; } = "";
		/// <summary>消費税率</summary>
		[XmlElement("TaxRatio")]
		public string TaxRatio { get; set; } = "";
		/// <summary>利用ポイント確定日</summary>
		[XmlElement("UsePointFixDate")]
		public string UsePointFixDate { get; set; } = "";
		/// <summary>利用ポイント確定有無</summary>
		[XmlElement("IsUsePointFix")]
		public string IsUsePointFix { get; set; } = "";
		/// <summary>全付与ポイント確定有無</summary>
		[XmlElement("IsGetPointFixAll")]
		public string IsGetPointFixAll { get; set; } = "";
		/// <summary>モールクーポン値引き額</summary>
		[XmlElement("TotalMallCouponDiscount")]
		public string TotalMallCouponDiscount { get; set; } = "";
		/// <summary>全付与ストアPayPayボーナス確定フラグ</summary>
		[XmlElement("IsGetStoreBonusFixAll")]
		public string IsGetStoreBonusFixAll { get; set; } = "";
		/// <summary>支払い金額</summary>
		[XmlElement("PayMethodAmount")]
		public string PayMethodAmount { get; set; } = "";
		/// <summary>併用支払い金額</summary>
		[XmlElement("CombinedPayMethodAmount")]
		public string CombinedPayMethodAmount { get; set; } = "";
	}

	/// <summary>
	/// 商品
	/// </summary>
	[Serializable]
	public class Item
	{
		/// <summary>LINEギフト手数料</summary>
		[XmlElement("LineGiftCharge")]
		public string LineGiftCharge { get; set; } = "";
		/// <summary>商品ラインID</summary>
		[XmlElement("LineId")]
		public string LineId { get; set; } = "";
		/// <summary>商品ID</summary>
		[XmlElement("ItemId")]
		public string ItemId { get; set; } = "";
		/// <summary>商品名</summary>
		[XmlElement("Title")]
		public string Title { get; set; } = "";
		/// <summary>商品サブコード</summary>
		[XmlElement("SubCode")]
		public string SubCode { get; set; } = "";
		/// <summary>商品サブコードオプション</summary>
		[XmlElement("SubCodeOption")]
		public string SubCodeOption { get; set; } = "";
		/// <summary>商品オプション</summary>
		[XmlElement("ItemOption")]
		public string ItemOption { get; set; } = "";
		/// <summary>インスクリプション</summary>
		[XmlElement("Inscription")]
		public string Inscription { get; set; } = "";
		/// <summary>中古フラグ</summary>
		[XmlElement("IsUsed")]
		public string IsUsed { get; set; } = "";
		/// <summary>商品画像ID</summary>
		[XmlElement("ImageId")]
		public string ImageId { get; set; } = "";
		/// <summary>課税対象</summary>
		[XmlElement("IsTaxable")]
		public string IsTaxable { get; set; } = "";
		/// <summary>消費税率</summary>
		[XmlElement("ItemTaxRatio")]
		public string ItemTaxRatio { get; set; } = "";
		/// <summary>重量</summary>
		[XmlElement("Weight")]
		public string Weight { get; set; } = "";
		/// <summary>サイズ</summary>
		[XmlElement("Size")]
		public string Size { get; set; } = "";
		/// <summary>JANコード</summary>
		[XmlElement("Jan")]
		public string Jan { get; set; } = "";
		/// <summary>製品コード</summary>
		[XmlElement("ProductId")]
		public string ProductId { get; set; } = "";
		/// <summary>プロダクトカテゴリID</summary>
		[XmlElement("CategoryId")]
		public string CategoryId { get; set; } = "";
		/// <summary>アフィリエイト料率</summary>
		[XmlElement("AffiliateRatio")]
		public string AffiliateRatio { get; set; } = "";
		/// <summary>商品単価</summary>
		[XmlElement("UnitPrice")]
		public string UnitPrice { get; set; } = "";
		/// <summary>商品税抜単価</summary>
		[XmlElement("NonTaxUnitPrice")]
		public string NonTaxUnitPrice { get; set; } = "";
		/// <summary>数量</summary>
		[XmlElement("Quantity")]
		public string Quantity { get; set; } = "";
		/// <summary>ポイント対象数量</summary>
		[XmlElement("PointAvailQuantity")]
		public string PointAvailQuantity { get; set; } = "";
		/// <summary>発売日</summary>
		[XmlElement("ReleaseDate")]
		public string ReleaseDate { get; set; } = "";
		/// <summary>送料無料フラグ</summary>
		[XmlElement("IsShippingFree")]
		public string IsShippingFree { get; set; } = "";
		/// <summary>返品可否フラグ</summary>
		[XmlElement("IsReturnAccept")]
		public string IsReturnAccept { get; set; } = "";
		/// <summary>商品発送元都道府県</summary>
		[XmlElement("LocationPrefecture")]
		public string LocationPrefecture { get; set; } = "";
		/// <summary>商品発送元市区町村</summary>
		[XmlElement("LocationCity")]
		public string LocationCity { get; set; } = "";
		/// <summary>商品レビュー有無フラグ</summary>
		[XmlElement("HaveReview")]
		public string HaveReview { get; set; } = "";
		/// <summary>キャンペーン対象フラグ</summary>
		[XmlElement("IsCampaign")]
		public string IsCampaign { get; set; } = "";
		/// <summary>商品別ポイントコード</summary>
		[XmlElement("PointFspCode")]
		public string PointFspCode { get; set; } = "";
		/// <summary>付与ポイント倍率（Yahoo!JAPAN負担）</summary>
		[XmlElement("PointRatioY")]
		public string PointRatioY { get; set; } = "";
		/// <summary>付与ポイント倍率（ストア負担）</summary>
		[XmlElement("PointRatioSeller")]
		public string PointRatioSeller { get; set; } = "";
		/// <summary>単位付与ポイント数</summary>
		[XmlElement("UnitGetPoint")]
		public string UnitGetPoint { get; set; } = "";
		/// <summary>付与ポイント確定フラグ</summary>
		[XmlElement("IsGetPointFix")]
		public string IsGetPointFix { get; set; } = "";
		/// <summary>付与ポイント確定日</summary>
		[XmlElement("GetPointFixDate")]
		public string GetPointFixDate { get; set; } = "";
		/// <summary>ストアクーポン</summary>
		[XmlElement("CouponData")]
		public string CouponData { get; set; } = "";
		/// <summary>クーポンの値引き額</summary>
		[XmlElement("CouponDiscount")]
		public string CouponDiscount { get; set; } = "";
		/// <summary>クーポン適用枚数</summary>
		[XmlElement("CouponUseNum")]
		public string CouponUseNum { get; set; } = "";
		/// <summary>値引き前の単価</summary>
		[XmlElement("OriginalPrice")]
		public string OriginalPrice { get; set; } = "";
		/// <summary>ライン分割前の数量</summary>
		[XmlElement("OriginalNum")]
		public string OriginalNum { get; set; } = "";
		/// <summary>発送日テキスト</summary>
		[XmlElement("LeadTimeText")]
		public string LeadTimeText { get; set; } = "";
		/// <summary>発送日スタート</summary>
		[XmlElement("LeadTimeStart")]
		public string LeadTimeStart { get; set; } = "";
		/// <summary>発送日エンド</summary>
		[XmlElement("LeadTimeEnd")]
		public string LeadTimeEnd { get; set; } = "";
		/// <summary>価格種別</summary>
		[XmlElement("PriceType")]
		public string PriceType { get; set; } = "";
		/// <summary>梱包バーコード情報</summary>
		[XmlElement("PickAndDeliveryCode")]
		public string PickAndDeliveryCode { get; set; } = "";
		/// <summary>荷扱い情報</summary>
		[XmlElement("PickAndDeliveryTransportRuleType")]
		public string PickAndDeliveryTransportRuleType { get; set; } = "";
		/// <summary>配送不可理由</summary>
		[XmlElement("YamatoUndeliverableReason")]
		public string YamatoUndeliverableReason { get; set; } = "";
		/// <summary>付与ストアPayPayボーナス倍率（Seller負担）</summary>
		[XmlElement("StoreBonusRatioSeller")]
		public string StoreBonusRatioSeller { get; set; } = "";
		/// <summary>単位付与ストアPayPayボーナス数</summary>
		[XmlElement("UnitGetStoreBonus")]
		public string UnitGetStoreBonus { get; set; } = "";
		/// <summary>付与ストアPayPayボーナス確定フラグ</summary>
		[XmlElement("IsGetStoreBonusFix")]
		public string IsGetStoreBonusFix { get; set; } = "";
		/// <summary>付与ストアPayPayボーナス確定日</summary>
		[XmlElement("GetStoreBonusFixDate")]
		public string GetStoreBonusFixDate { get; set; } = "";
		/// <summary>オークションID</summary>
		[XmlElement("ItemYahooAucId")]
		public string ItemYahooAucId { get; set; } = "";
		/// <summary>ヤフオク!管理番号</summary>
		[XmlElement("ItemYahooAucMerchantId")]
		public string ItemYahooAucMerchantId { get; set; } = "";
	}

	/// <summary>
	/// セラー
	/// </summary>
	[Serializable]
	public class Seller
	{
		/// <summary>セラーID</summary>
		[XmlElement("SellerId")]
		public string SellerId { get; set; } = "";
		/// <summary>LINEギフトアカウント（LINEギフトショップID）</summary>
		[XmlElement("LineGiftAccount")]
		public string LineGiftAccount { get; set; } = "";
	}

	/// <summary>
	/// バイヤー
	/// </summary>
	[Serializable]
	public class Buyer
	{
		/// <summary>Yahoo! JAPAN IDログイン有無</summary>
		[XmlElement("IsLogin")]
		public string IsLogin { get; set; } = "";
		/// <summary>FSPライセンスコード</summary>
		[XmlElement("FspLicenseCode")]
		public string FspLicenseCode { get; set; } = "";
		/// <summary>FSPライセンス名</summary>
		[XmlElement("FspLicenseName")]
		public string FspLicenseName { get; set; } = "";
		/// <summary>ゲストユニークキー（vwoidc）</summary>
		[XmlElement("GuestAuthId")]
		public string GuestAuthId { get; set; } = "";
	}

	/// <summary>
	/// 警告
	/// </summary>
	[Serializable]
	public class Warning
	{
		/// <summary>警告コード</summary>
		[XmlElement("Code")]
		public string Code { get; set; } = "";
		/// <summary>警告メッセージ</summary>
		[XmlElement("Message")]
		public string Message { get; set; } = "";
		/// <summary>詳細</summary>
		[XmlElement("Detail")]
		public string Detail { get; set; } = "";
	}
}
