/*
=========================================================================================================
  Module      : 定数定義 (Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.MallBatch.MailOrderGetter
{
	///**************************************************************************************
	/// <summary>
	/// 定数定義
	/// </summary>
	///**************************************************************************************
	public class Constants : w2.App.Common.Constants
	{
		public static string PRODUCT_KEY;								// 0:product_id 1:cooperation_id1
		public static string PRODUCT_KEY_PRODUCT_ID = "0";
		public static string PRODUCT_KEY_COOPERATION_ID1 = "1";

		public static string SHOP_ID = null;							// ショップID
		public static string DISP_ERROR_MESSAGE = null;					// 画面表示用エラーメッセージ内容
		public static string DISP_ERROR_KBN = null;						// 画面表示用エラー区分
		public static string PATH_ACTIVE = null;						// 「アクティブ」フォルダ
		public static string PATH_ERROR = null;							// 「エラー」フォルダ
		public static string PATH_STOCK_ERROR = null;					// 「在庫レコードなし」フォルダ
		public static string PATH_SUCCESS = null;						// 「取り込み完了」フォルダ
		public static string PATH_UNKNOWN = null;						// 「不明」フォルダ
		public static string PATH_RETRY = null;							// 「リトライ」フォルダ

		// 決済区分名称（注文メールから決済名称を取り込むための定数）
		public static string ORDER_PAYMENT_KBN_NAME_CREDIT = "クレジットカード";
		public static string ORDER_PAYMENT_KBN_NAME_CASH_ON_DELIVERY = "代金引換";
		public static string ORDER_PAYMENT_KBN_NAME_BANK_TRANSFER_CASH_BEFORE_DELIVERY = "銀行振込（前払い）";
		public static string ORDER_PAYMENT_KBN_NAME_BANK_TRANSFER_DEFERRED_PAYMENT = "銀行振込（後払い）";
		public static string ORDER_PAYMENT_KBN_NAME_CONVENIENCE_STORE_CASH_BEFORE_DELIVERY = "コンビニ（前払い）";
		public static string ORDER_PAYMENT_KBN_NAME_CONVENIENCE_STORE_DEFERRED_PAYMENT = "コンビニ（後払い）";
		public static string ORDER_PAYMENT_KBN_NAME_POST_TRANSFER_CASH_BEFORE_DELIVERY = "郵便振込（前払い）";
		public static string ORDER_PAYMENT_KBN_NAME_POST_TRANSFER_DEFERRED_PAYMENT = "郵便振込（後払い）";
		public static string ORDER_PAYMENT_KBN_NAME_NON_PAYMENT = "決済なし";

		// Addr1 of order rakuten oversea 
		public static string ORDER_ADDR1_RAKUTEN_OVERSEA = "東京都";

		public const string BATCH_USER = "batch";
		public static string TAX_ROUNDTYPE = null;
		public static bool RAKUTEN_RESERVATION_ORDER_MAIL_ENABLED = false;
		public static bool RAKUTEN_OVERSEA_ORDER_MAIL_ENABLED = false;
		public static string RAKUTEN_OVERSEAS_ORDER_EXTEND_STATUS_FIELD = "";
		public static string RAKUTEN_SHIPPING_TOMORROW_ORDER_EXTEND_STATUS_FIELD = "";
		// モール連携デフォルト配送種別
		public static string MALL_DEFAULT_SHIPPING_ID = "101";

		/// <summary>楽天受注メール: 注文通知メール件名</summary>
		public static string RAKUTEN_ORDER_MAIL_SUBJECT = "";
		/// <summary>楽天受注メール: 注文通知メール件名(RakutenGlobalMarket)</summary>
		public static string RAKUTEN_GLOBAL_MARKET_ORDER_MAIL_SUBJECT = "";
		/// <summary>楽天受注メール: 予約購入の受注確定時メール件名</summary>
		public static string RAKUTEN_RESERVATION_ORDER_MAIL_SUBJECT = "";
		/// <summary>楽天受注メール: 注文メール内の受注番号を含む行の正規表現</summary>
		public static string RAKUTEN_REGULAR_EXPRESSION_FOR_LINE = "";
		/// <summary>楽天受注メール: 注文メール内の受注番号の正規表現</summary>
		public static string RAKUTEN_REGULAR_EXPRESSION_FOR_ORDER_NO = "";
		/// <summary>楽天受注API: 税込別フラグ(税込)</summary>
		public const string RAKUTEN_API_INCLUDE_TAX_FLG_ON = "1";
		/// <summary>楽天受注API: 送料込別フラグ(送料込もしくは送料無料)</summary>
		public const string RAKUTEN_API_INCLUDE_POSTAGE_FLG_ON = "1";
		/// <summary>楽天受注API: クレジットカード支払い回数(3回払い)</summary>
		public const string RAKUTEN_API_CARD_INSTALLMENT_DESC_3 = "103";
		/// <summary>楽天受注API: クレジットカード支払い回数(5回払い)</summary>
		public const string RAKUTEN_API_CARD_INSTALLMENT_DESC_5 = "105";
		/// <summary>楽天受注API: クレジットカード支払い回数(6回払い)</summary>
		public const string RAKUTEN_API_CARD_INSTALLMENT_DESC_6 = "106";
		/// <summary>楽天受注API: クレジットカード支払い回数(10回払い)</summary>
		public const string RAKUTEN_API_CARD_INSTALLMENT_DESC_10 = "110";
		/// <summary>楽天受注API: クレジットカード支払い回数(12回払い)</summary>
		public const string RAKUTEN_API_CARD_INSTALLMENT_DESC_12 = "112";
		/// <summary>楽天受注API: クレジットカード支払い回数(15回払い)</summary>
		public const string RAKUTEN_API_CARD_INSTALLMENT_DESC_15 = "115";
		/// <summary>楽天受注API: クレジットカード支払い回数(18回払い)</summary>
		public const string RAKUTEN_API_CARD_INSTALLMENT_DESC_18 = "118";
		/// <summary>楽天受注API: クレジットカード支払い回数(20回払い)</summary>
		public const string RAKUTEN_API_CARD_INSTALLMENT_DESC_20 = "120";
		/// <summary>楽天受注API: クレジットカード支払い回数(24回払い)</summary>
		public const string RAKUTEN_API_CARD_INSTALLMENT_DESC_24 = "124";
		/// <summary>楽天受注API: お届け時間帯(指定なし)</summary>
		public const string RAKUTEN_API_SHIPPING_TERM_NONE = "0";
		/// <summary>楽天受注API: お届け時間帯(午前)</summary>
		public const string RAKUTEN_API_SHIPPING_TERM_AM = "1";
		/// <summary>楽天受注API: お届け時間帯(午後)</summary>
		public const string RAKUTEN_API_SHIPPING_TERM_PM = "2";
		/// <summary>楽天受注API: お届け時間帯(その他)</summary>
		public const string RAKUTEN_API_SHIPPING_TERM_OTHER = "9";
		/// <summary>楽天受注API: 楽天APIの利用端末</summary>
		public enum RakutenApiCarrierCode
		{
			/// <summary>Windows系のスマートフォン、タブレットを含む。</summary>
			Pc = 0,
			/// <summary>モバイル(docomo) フィーチャーフォン</summary>
			MobileDocomo = 1,
			/// <summary>モバイル(KDDI) フィーチャーフォン</summary>
			MobileKddi = 2,
			/// <summary>モバイル(Softbank) フィーチャーフォン</summary>
			MobileSoftbank = 3,
			/// <summary>モバイル(WILLCOM) フィーチャーフォン</summary>
			MobileWillcom = 5,
			/// <summary>スマートフォン（iPhone系）</summary>
			SmartphoneIphone = 11,
			/// <summary>スマートフォン（Android系）</summary>
			SmartphoneAndroid = 12,
			/// <summary>スマートフォン（その他）</summary>
			SmartphoneOther = 19,
			/// <summary>タブレット（iPad系）</summary>
			TabletIpad = 21,
			/// <summary>タブレット（Android系）</summary>
			TabletAndroid = 22,
			/// <summary>タブレット（その他）</summary>
			TabletOther = 29,
			/// <summary>その他　不明な場合も含む</summary>
			Other = 99,
		}
		/// <summary>楽天受注API: 予約購入の種別</summary>
		public const int RAKUTEN_API_RESERVE_ORDER_TYPE = 6;
		/// <summary>楽天受注API: あす楽フラグ有効の値</summary>
		public const string RAKUTEN_API_ASURAKU_FLG_ON = "1";
		/// <summary>実行モード</summary>
		public enum ExecTypes
		{
			/// <summary>アクティブ</summary>
			Active,
			/// <summary>リトライ</summary>
			Retry
		}
	}
}
