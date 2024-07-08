/*
=========================================================================================================
  Module      : ロハコ連携用定数定義クラス(LohacoConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
namespace w2.App.Common.LohacoCreatorWebApi
{
	/// <summary>
	/// ロハコ連携用定数定義クラス
	/// </summary>
	public class LohacoConstants
	{
		/// <summary>（リクエスト・レスポンス）JSONのコンテンツタイプ</summary>
		public static string CONTENT_TYPE_JSON = "application/json";
		/// <summary>（リクエスト・レスポンス）Xmlのコンテンツタイプ</summary>
		public static string CONTENT_TYPE_XML = "application/xml";
		/// <summary>（リクエスト）UrlEncodedのコンテンツタイプ</summary>
		public static string CONTENT_TYPE_FORM_URL_ENCODED = "application/x-www-form-urlencoded";
		/// <summary>（リクエスト）フォームデータのコンテンツタイプ</summary>
		public static string CONTENT_TYPE_MULTIPART_FORM_DATA = "multipart/form-data";
		/// <summary>X-Service-Provider-Idリクエストヘッダー</summary>
		public static string REQUEST_HEADER_PROVIDER_ID = "X-Service-Provider-Id";
		/// <summary>X-Api-Signatureリクエストヘッダー</summary>
		public static string REQUEST_HEADER_SIGNATURE = "X-Api-Signature";
		/// <summary>注文情報のソート順（注文日の昇順）</summary>
		public static string SORT_ORDER_TIME_ASCENDING = "+order_time";
		/// <summary>注文情報のソート順（注文日の降順）</summary>
		public static string SORT_ORDER_TIME_DESCENDING = "-order_time";
		/// <summary>個人情報でマスクされる文字列</summary>
		public static string MASK_STRING = "****";
		/// <summary>リクエストのタイムアウト値（ミリ秒単位）</summary>
		public static int API_REQUEST_TIME_OUT = 50000;

		#region 列挙体
		/// <summary>エラーコード</summary>
		public enum ErrorCode
		{
			// ■共通エラーコード
			/// <summary>公開鍵の有効期限が切れている</summary>
			[XmlEnum(Name = "px-01001")]
			Px01001,
			/// <summary>署名認証に失敗した</summary>
			[XmlEnum(Name = "px-01002")]
			Px01002,
			/// <summary>存在しないアクセスポイントへのアクセス</summary>
			[XmlEnum(Name = "px-01003")]
			Px01003,
			/// <summary>許可されていないメソッドタイプのリクエスト</summary>
			[XmlEnum(Name = "px-01004")]
			Px01004,
			/// <summary>公開鍵が登録されていない</summary>
			[XmlEnum(Name = "px-01005")]
			Px01005,
			/// <summary>ヘッダとリクエストパラメータのセラーID が一致しない</summary>
			[XmlEnum(Name = "px-01006")]
			Px01006,
			/// <summary>XML のパースに失敗した</summary>
			[XmlEnum(Name = "px-01007")]
			Px01007,
			/// <summary>許可されていない IP からのアクセス</summary>
			[XmlEnum(Name = "px-01008")]
			Px01008,
			// ■在庫管理API（stockEdit）
			/// <summary>リクエストパラメータエラー</summary>
			[XmlEnum(Name = "st01001")]
			St01001,
			/// <summary>1 リクエストに設定できる商品数を超えている</summary>
			[XmlEnum(Name = "st01002")]
			St01002,
			/// <summary>在庫数の指定に誤りがある</summary>
			[XmlEnum(Name = "st01003")]
			St01003,
			/// <summary>セラーID が不正</summary>
			[XmlEnum(Name = "st01004")]
			St01004,
			/// <summary>データがない</summary>
			[XmlEnum(Name = "st01006")]
			St01006,
			/// <summary>在庫数が設定可能な範囲を超過している</summary>
			[XmlEnum(Name = "st01007")]
			St01007,
			/// <summary>在庫数を超えて注文できない</summary>
			[XmlEnum(Name = "st01009")]
			St01009,
			/// <summary>データベースエラーが発生した</summary>
			[XmlEnum(Name = "st01998")]
			St01998,
			/// <summary>システムエラーが発生した</summary>
			[XmlEnum(Name = "st01999")]
			St01999,
			// ■在庫検索API（stockSearch）
			/// <summary>リクエストパラメータエラー</summary>
			[XmlEnum(Name = "st02001")]
			St02001,
			/// <summary>1 リクエストに設定できる商品数を超えている</summary>
			[XmlEnum(Name = "st02002")]
			St02002,
			/// <summary>セラーID が不正</summary>
			[XmlEnum(Name = "st02004")]
			St02004,
			/// <summary>データベースエラーが発生した</summary>
			[XmlEnum(Name = "st02998")]
			St02998,
			/// <summary>システムエラーが発生しました</summary>
			[XmlEnum(Name = "st02999")]
			St02999,
			// ■注文検索API（orderList/orderPublicList）
			// ■注文詳細API（orderInfo/orderPublicInfo）
			/// <summary>リクエストパラメータエラー</summary>
			[XmlEnum(Name = "od00001")]
			Od00001,
			/// <summary>該当注文がない</summary>
			[XmlEnum(Name = "od00003")]
			Od00003,
			/// <summary>取得可能項目以外の項目指定エラー</summary>
			[XmlEnum(Name = "od60001")]
			Od60001,
			/// <summary>内部エラー。連続して発生する場合はお問い合わせください。 </summary>
			[XmlEnum(Name = "od00004")]
			Od00004,
			/// <summary>取得可能項目以外の項目指定エラー</summary>
			[XmlEnum(Name = "od50001")]
			Od50001,
		}

		/// <summary>ステータス</summary>
		public enum Status
		{
			/// <summary>成功</summary>
			[XmlEnum(Name = "OK")]
			OK,
			/// <summary>失敗</summary>
			[XmlEnum(Name = "NG")]
			NG,
		}

		/// <summary>在庫管理・在庫検索API StockEdit・StockSearchのステータス</summary>
		public enum StockStatus
		{
			/// <summary>正常</summary>
			[XmlEnum(Name = "1")]
			Success,
			/// <summary>警告</summary>
			[XmlEnum(Name = "2")]
			Warning,
			/// <summary>エラー</summary>
			[XmlEnum(Name = "3")]
			Error,
		}

		/// <summary>リクエストのコンテンツタイプ</summary>
		public enum RequestContentType
		{
			/// <summary>リクエストはXMLデータのみ</summary>
			Xml,
			/// <summary>リクエストは、a=b&c=dのようなパラメータ形式</summary>
			FormUrlEncoded,
			/// <summary>リクエストはマルチパートデータのアップロード形式</summary>
			FormData,
		}

		/// <summary>レスポンスのコンテンツタイプ</summary>
		public enum ResponseContentType
		{
			/// <summary>レスポンスはXMLデータ</summary>
			Xml,
			/// <summary>レスポンスはJsonデータ</summary>
			Json,
		}

		/// <summary>注文Field</summary>
		public enum OrderField
		{
			/// <summary>注文ID</summary>
			OrderId,
			/// <summary>バージョン</summary>
			Version,
			/// <summary>デバイス種類</summary>
			DeviceType,
			/// <summary>閲覧済みフラグ</summary>
			IsSeen,
			/// <summary>キャンセル理由</summary>
			CancelReason,
			/// <summary>課金フラグ</summary>
			IsRoyalty,
			/// <summary>課金確定フラグ</summary>
			IsRoyaltyFix,
			/// <summary>管理者注文フラグ</summary>
			IsSeller,
			/// <summary>注文日時</summary>
			OrderTime,
			/// <summary>最終更新日時</summary>
			LastUpdateTime,
			/// <summary>注文ステータス</summary>
			OrderStatus,
			/// <summary>課金確定日時</summary>
			RoyaltyFixTime,
			/// <summary>注文確認メール送信時刻</summary>
			SendConfirmTime,
			/// <summary>支払完了メール送信時刻</summary>
			SendPayTime,
			/// <summary>注文伝票出力時刻</summary>
			PrintSlipTime,
			/// <summary>納品書出力時刻</summary>
			PrintDeliveryTime,
			/// <summary>バイヤーコメント</summary>
			BuyerComments,
			/// <summary>セラーコメント</summary>
			SellerComments,
			/// <summary>社内メモ</summary>
			Notes,
			/// <summary>更新者</summary>
			OperationUser,
			/// <summary>履歴 ID</summary>
			HistoryId,
			/// <summary>クーポン種別</summary>
			CouponType,
			/// <summary>使用したモールクーポン情報</summary>
			CouponCampaignCode,
			/// <summary>ストアクーポン種別</summary>
			StoreCouponType,
			/// <summary>ストアクーポンコード</summary>
			StoreCouponCode,
			/// <summary>ストアクーポン名</summary>
			StoreCouponName,
			/// <summary>自動完了予定日</summary>
			AutoDoneDate,
			/// <summary>初回完了日</summary>
			FirstOrderDoneDate,
			/// <summary>入金ステータス</summary>
			PayStatus,
			/// <summary>決済ステータス</summary>
			SettleStatus,
			/// <summary>支払い方法</summary>
			PayMethod,
			/// <summary>支払い方法名称</summary>
			PayMethodName,
			/// <summary>支払い日時</summary>
			PayActionTime,
			/// <summary>入金日</summary>
			PayDate,
			/// <summary>入金処理備考</summary>
			PayNotes,
			/// <summary>決済 ID</summary>
			SettleId,
			/// <summary>カード支払い区分</summary>
			CardPayType,
			/// <summary>カード支払回数</summary>
			CardPayCount,
			/// <summary>Yahoo!カード利用有無</summary>
			UseYahooCard,
			/// <summary>ウォレット利用有無</summary>
			UseWallet,
			/// <summary>納品書有無</summary>
			NeedDetailedSlip,
			/// <summary>ご請求先名前</summary>
			BillFirstName,
			/// <summary>ご請求先名前カナ</summary>
			BillFirstNameKana,
			/// <summary>ご請求先名字</summary>
			BillLastName,
			/// <summary>ご請求先名字カナ</summary>
			BillLastNameKana,
			/// <summary>ご請求先郵便番号</summary>
			BillZipCode,
			/// <summary>ご請求先都道府県</summary>
			BillPrefecture,
			/// <summary>ご請求先市区郡</summary>
			BillCity,
			/// <summary>ご請求先住所 1</summary>
			BillAddress1,
			/// <summary>ご請求先住所 2</summary>
			BillAddress2,
			/// <summary>ご請求先電話番号</summary>
			BillPhoneNumber,
			/// <summary>ご請求先メールアドレス</summary>
			BillMailAddress,
			/// <summary>出荷ステータス</summary>
			ShipStatus,
			/// <summary>配送方法</summary>
			ShipMethod,
			/// <summary>配送方法名称</summary>
			ShipMethodName,
			/// <summary>配送希望日</summary>
			ShipRequestDate,
			/// <summary>配送希望時間</summary>
			ShipRequestTime,
			/// <summary>配送メモ</summary>
			ShipNotes,
			/// <summary>配送伝票番号１</summary>
			ShipInvoiceNumber1,
			/// <summary>配送伝票番号 2</summary>
			ShipInvoiceNumber2,
			/// <summary>配送会社URL</summary>
			ShipUrl,
			/// <summary>翌日配送フラグ</summary>
			ArriveType,
			/// <summary>宅配 BOX 利用</summary>
			DeliveryBoxType,
			/// <summary>出荷日</summary>
			ShipDate,
			/// <summary>着荷日</summary>
			ArrivalDate,
			/// <summary>ギフト包装有無</summary>
			NeedGiftWrap,
			/// <summary>ギフト包装種類</summary>
			GiftWrapType,
			/// <summary>ギフトメッセージ</summary>
			GiftWrapMessage,
			/// <summary>のし有無</summary>
			NeedGiftWrapPaper,
			/// <summary>のし種類</summary>
			GiftWrapPaperType,
			/// <summary>名入れ</summary>
			GiftWrapName,
			/// <summary>お届け先名前</summary>
			ShipFirstName,
			/// <summary>お届け先名前カナ</summary>
			ShipFirstNameKana,
			/// <summary>お届け先名字</summary>
			ShipLastName,
			/// <summary>お届け先名字カナ</summary>
			ShipLastNameKana,
			/// <summary>お届け先郵便番号</summary>
			ShipZipCode,
			/// <summary>お届け先都道府県</summary>
			ShipPrefecture,
			/// <summary>お届け先市区郡</summary>
			ShipCity,
			/// <summary>お届け先住所 1</summary>
			ShipAddress1,
			/// <summary>お届け先住所 2</summary>
			ShipAddress2,
			/// <summary>お届け先電話番号</summary>
			ShipPhoneNumber,
			/// <summary>手数料</summary>
			PayCharge,
			/// <summary>送料</summary>
			ShipCharge,
			/// <summary>ギフト包装料</summary>
			GiftWrapCharge,
			/// <summary>値引き</summary>
			Discount,
			/// <summary>モールクーポン値引き額</summary>
			TotalMallCouponDiscount,
			/// <summary>ストアクーポン値引き額</summary>
			StoreCouponDiscount,
			/// <summary>調整額</summary>
			Adjustments,
			/// <summary>利用ポイント数</summary>
			UsePoint,
			/// <summary>合計金額</summary>
			TotalPrice,
			/// <summary>付与ポイント確定日</summary>
			GetPointFixDate,
			/// <summary>全付与ポイント確定有無</summary>
			IsGetPointFixAll,
			/// <summary>PayPay利用額</summary>
			UsePaypayAmount,
			/// <summary>商品ラインID</summary>
			LineId,
			/// <summary>カタログ商品コード</summary>
			CtgItemCd,
			/// <summary>商品コード</summary>
			ItemCd,
			/// <summary>商品名</summary>
			Title,
			/// <summary>属性情報</summary>
			Attribute,
			/// <summary>課税対象</summary>
			IsTaxable,
			/// <summary>JAN コード </summary>
			Jan,
			/// <summary>ISBN コード</summary>
			Isbn,
			/// <summary>大大カテゴリコード</summary>
			LlCateCd,
			/// <summary>大カテゴリコード</summary>
			LCateCd,
			/// <summary>小カテゴリコード</summary>
			SCateCode,
			/// <summary>商品単価</summary>
			UnitPrice,
			/// <summary>値引き前の単価</summary>
			OriginUnitPrice,
			/// <summary>商品 1 つあたりのクーポン値引き額</summary>
			UnitStoreCouponDiscount,
			/// <summary>数量</summary>
			Quantity,
			/// <summary>発売日</summary>
			ReleaseDate,
			/// <summary>送料無料フラグ</summary>
			IsShippingFree,
			/// <summary>単位付与ポイント数</summary>
			UnitGetPoint,
			/// <summary>アドオン付与ポイント数</summary>
			AddonGetPoint,
			/// <summary>アドオン付与ポイント倍率</summary>
			AddonGetPointRatio,
			/// <summary>YJ カード付与ポイント数 </summary>
			YjCardGetPoint,
			/// <summary>YJ カード付与ポイント倍率</summary>
			YjCardGetPointRatio,
			/// <summary>消費税率</summary>
			TaxRatio,
			/// <summary>セラーID</summary>
			SellerId,
			/// <summary>Yahoo! JAPAN ID ログイン有無</summary>
			IsLogin,
			/// <summary>顧客コード</summary>
			CustomerCd,
			/// <summary>FSP ライセンスコード</summary>
			FspLicenseCode,
			/// <summary>FSP ライセンス名</summary>
			FspLicenseName,
			/// <summary>YJ カード会員ステータス</summary>
			YjCardStatus,
		}

		/// <summary>デバイス種類</summary>
		public enum DeviceType
		{
			/// <summary>PC</summary>
			[XmlEnum("0")]
			Pc = 0,
			/// <summary>SP</summary>
			[XmlEnum("1")]
			Sp = 1,
			/// <summary>アプリ</summary>
			[XmlEnum("2")]
			App = 2,
			/// <summary>YahooShoppingアプリ</summary>
			[XmlEnum("3")]
			YahooShoppingApp = 3,
		}

		/// <summary>キャンセル理由</summary>
		public enum CancelReason
		{
			// ■注文者都合
			/// <summary>キャンセル</summary>
			[XmlEnum("100")]
			Cancel = 100,
			/// <summary>返品</summary>
			[XmlEnum("110")]
			Return = 110,
			/// <summary>未入金</summary>
			[XmlEnum("120")]
			NotPayment = 120,
			/// <summary>住所不明</summary>
			[XmlEnum("130")]
			AddressInvalid = 130,
			/// <summary>受け取り拒否</summary>
			[XmlEnum("140")]
			ReceiveRefuse = 140,
			/// <summary>連絡不通</summary>
			[XmlEnum("150")]
			ContactNotThrough = 150,
			/// <summary>重複注文</summary>
			[XmlEnum("160")]
			DuplicateOrder = 160,
			/// <summary>決済審査不可</summary>
			[XmlEnum("170")]
			PaymentNg = 170,
			/// <summary>その他</summary>
			[XmlEnum("180")]
			OwnerOther = 180,
			// ■ストア都合
			/// <summary>決済方法都合</summary>
			[XmlEnum("200")]
			PaymentMethod = 200,
			/// <summary>欠品</summary>
			[XmlEnum("210")]
			OutOfStock = 210,
			/// <summary>販売中止</summary>
			[XmlEnum("220")]
			NotSell = 220,
			/// <summary>その他（nullや指定がない場合は「230」が選択される）</summary>
			[XmlEnum("230")]
			StoreOther = 230,
		}

		/// <summary>参照可否フラグ</summary>
		public enum PermissionType
		{
			/// <summary>セラー、バイヤー参照可</summary>
			[XmlEnum("0")]
			BothOk = 0,
			/// <summary>セラーのみ参照可</summary>
			[XmlEnum("1")]
			SellerOnlyOk = 1,
			/// <summary>バイヤーのみ参照可 </summary>
			[XmlEnum("2")]
			BuyerOnlyOk = 2,
			/// <summary>セラー、バイヤー参照不可</summary>
			[XmlEnum("3")]
			BothNg = 3,
		}

		/// <summary>注文ステータス</summary>
		public enum OrderStatus
		{
			/// <summary>予約中</summary>
			[XmlEnum("1")]
			Reserving = 1,
			/// <summary>処理中</summary>
			[XmlEnum("2")]
			Processing = 2,
			/// <summary>保留</summary>
			[XmlEnum("3")]
			Pending = 3,
			/// <summary>キャンセル</summary>
			[XmlEnum("4")]
			Cancelled = 4,
			/// <summary> 完了</summary>
			[XmlEnum("5")]
			Completed = 5,
		}

		/// <summary>クーポン種別</summary>
		public enum CouponType
		{
			/// <summary>クーポンなし</summary>
			[XmlEnum("0")]
			NoCoupon = 0,
			/// <summary>モールクーポン</summary>
			[XmlEnum("1")]
			MallCoupon = 1,
			/// <summary>ストアクーポン</summary>
			[XmlEnum("2")]
			StoreCoupon = 2,
		}

		/// <summary>ストアクーポン種別</summary>
		public enum StoreCouponType
		{
			/// <summary>定額値値引き</summary>
			[XmlEnum("1")]
			FixedAmount = 1,
			/// <summary>定率値値引き</summary>
			[XmlEnum("2")]
			FixedPercentage = 2,
			/// <summary>送料無料</summary>
			[XmlEnum("3")]
			FreeShipping = 3,
		}

		/// <summary>入金ステータス</summary>
		public enum PayStatus
		{
			/// <summary>未入金・入金待ち</summary>
			[XmlEnum("0")]
			NotPayment = 0,
			/// <summary>入金済</summary>
			[XmlEnum("1")]
			Paid = 1,
		}

		/// <summary>決済ステータス</summary>
		public enum SettleStatus
		{
			/// <summary>決済申込</summary>
			[XmlEnum("1")]
			PaymentApply = 1,
			/// <summary>支払待ち</summary>
			[XmlEnum("2")]
			PaymentWaiting = 2,
			/// <summary>支払完了</summary>
			[XmlEnum("3")]
			PaymentCompleted = 3,
			/// <summary>入金待ち</summary>
			[XmlEnum("4")]
			DepositWaiting = 4,
			/// <summary>決済完了</summary>
			[XmlEnum("5")]
			DepositCompleted = 5,
			/// <summary>キャンセル</summary>
			[XmlEnum("6")]
			Cancelled = 6,
			/// <summary>返金</summary>
			[XmlEnum("7")]
			Refund = 7,
			/// <summary>有効期限切れ</summary>
			[XmlEnum("8")]
			Expired = 8,
			/// <summary>決済申込中</summary>
			[XmlEnum("9")]
			PaymentApplying = 9,
			/// <summary>オーソリエラー</summary>
			[XmlEnum("10")]
			AuthorError = 10,
			/// <summary>売上取消</summary>
			[XmlEnum("11")]
			CreditCancelled = 11,
		}

		/// <summary>カード支払い区分</summary>
		public enum CardPayType
		{
			/// <summary>一括払い</summary>
			[XmlEnum("1")]
			FullPayment = 1,
			/// <summary>ボーナス一括払い</summary>
			[XmlEnum("2")]
			BonusPayment = 2,
			/// <summary>リボ払い</summary>
			[XmlEnum("3")]
			RevolvingPayment = 3,
			/// <summary>分割払い</summary>
			[XmlEnum("4")]
			DividedPayment = 4,
		}

		/// <summary>出荷ステータス</summary>
		public enum ShipStatus
		{
			/// <summary>出荷不可</summary>
			[XmlEnum("0")]
			UnableToShip = 0,
			/// <summary>出荷可</summary>
			[XmlEnum("1")]
			AbleToShip = 1,
			/// <summary> 出荷処理中</summary>
			[XmlEnum("2")]
			Shipping = 2,
			/// <summary>出荷完了</summary>
			[XmlEnum("3")]
			Shipped = 3,
			/// <summary>着荷完了</summary>
			[XmlEnum("4")]
			Delivered = 4,
		}

		/// <summary>翌日配送フラグ</summary>
		public enum ArriveType
		{
			/// <summary>非対応</summary>
			[XmlEnum("0")]
			NotSupport = 0,
			/// <summary>即日配送注文</summary>
			[XmlEnum("1")]
			SameDayDelivery = 1,
			/// <summary>翌日配送注文</summary>
			[XmlEnum("2")]
			NextDayDelivery = 2,
		}

		/// <summary>宅配 BOX 利用</summary>
		public enum DeliveryBoxType
		{
			/// <summary>宅配 BOX 指定なし</summary>
			[XmlEnum("0")]
			NotSpecifyMailBox = 0,
			/// <summary>宅配 BOX 不可</summary>
			[XmlEnum("1")]
			NotAllowMailBox = 1,
			/// <summary>宅配 BOX 希望</summary>
			[XmlEnum("2")]
			WantMailBox = 2,
		}

		/// <summary>YJ カード会員ステータス</summary>
		public enum YjCardStatus
		{
			/// <summary>非会員・申込可能</summary>
			[XmlEnum("0")]
			Guest = 0,
			/// <summary>申込済・審査中</summary>
			[XmlEnum("1")]
			Inspecting = 1,
			/// <summary>申込済・発送中</summary>
			[XmlEnum("2")]
			Shipping = 2,
			/// <summary>申込済・カード到着済み</summary>
			[XmlEnum("3")]
			Delivered = 3,
			/// <summary>無効中</summary>
			[XmlEnum("4")]
			Invalid = 4,
			/// <summary>審査 NG</summary>
			[XmlEnum("5")]
			InspectFailed = 5,
			/// <summary>退会</summary>
			[XmlEnum("6")]
			Unsubscribed = 6,
		}

		/// <summary>配送希望日無し</summary>
		public enum SpecifyShippingDateType
		{
			/// <summary>配送希望日無しを含む</summary>
			[XmlEnum("0")]
			IncludeUnspecifyShippingDate = 0,
			/// <summary>配送希望日無しを含まない</summary>
			[XmlEnum("1")]
			NotIncludeUnspecifyShippingDate = 1,
			/// <summary>配送希望無しのみ</summary>
			[XmlEnum("2")]
			OnlyUnspecifyShippingDate = 2,
		}

		/// <summary>返金の要不要や返金の状態を表す区分。APIで更新できるのは1：必要から2：返金済みへの更新のみ</summary>
		public enum RefundStatus
		{
			/// <summary>不要</summary>
			[XmlEnum("0")]
			NotNeedRefund = 0,
			/// <summary>必要</summary>
			[XmlEnum("1")]
			NeedRefund = 1,
			/// <summary>返金済み</summary>
			[XmlEnum("2")]
			RefundDone = 2,
		}

		/// <summary>キャンセル種別</summary>
		public enum CancelType
		{
			/// <summary>バイヤーからのキャンセル</summary>
			[XmlEnum("1")]
			BuyerCancel = 1,
			/// <summary>ストアからのキャンセル</summary>
			[XmlEnum("2")]
			StoreCancel = 2,
			/// <summary>注文完了時のエラー発生によるキャンセル(ロールバック)</summary>
			[XmlEnum("3")]
			RollbackCancel = 3,
		}
		#endregion
	}
}
