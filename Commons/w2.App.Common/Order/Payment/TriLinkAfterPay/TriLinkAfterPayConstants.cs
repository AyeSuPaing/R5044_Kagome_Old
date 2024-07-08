/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 定数定義クラス(TriLinkAfterPayConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.TriLinkAfterPay
{
	/// <summary>
	/// 後付款(TriLink後払い) 定数定義クラス
	/// </summary>
	public class TriLinkAfterPayConstants
	{
		/// <summary>アクセストークンセッションキー</summary>
		public const string SESSION_KEY_TRILINK_AFTERPAY_ACCESS_TOKEN = "w2Front_skey_trilink_afterpay_access_token";

		// APIフィールド名
		/// <summary>後付款(TriLink後払い)：APIフィールド名 購入者注文日</summary>
		public const string TW_AFTERPAY_FIELD_BUYER_ORDER_DATE = "buyerOrderDate";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 注文者</summary>
		public const string TW_AFTERPAY_FIELD_CUSTOMER = "customer";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 配送先</summary>
		public const string TW_AFTERPAY_FIELD_SHIPMENT = "shipment";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 請求金額</summary>
		public const string TW_AFTERPAY_FIELD_BILLING_AMOUNT = "billingAmount";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 購入商品</summary>
		public const string TW_AFTERPAY_FIELD_ITEMS = "items";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 審査結果</summary>
		public const string TW_AFTERPAY_FIELD_AUTHORIZATION = "authorization";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 会社名</summary>
		public const string TW_AFTERPAY_FIELD_COMPANY_NAME = "companyName";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 部署名</summary>
		public const string TW_AFTERPAY_FIELD_DEPARTMENT_NAME = "departmentName";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 名</summary>
		public const string TW_AFTERPAY_FIELD_NAME = "name";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 郵便番号</summary>
		public const string TW_AFTERPAY_FIELD_ZIP_CODE = "zipCode";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 住所</summary>
		public const string TW_AFTERPAY_FIELD_ADDRESS = "address";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 携帯番号</summary>
		public const string TW_AFTERPAY_FIELD_MOBILE = "mobile";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 固定電話番号</summary>
		public const string TW_AFTERPAY_FIELD_LAND_LINE = "landLine";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 メールアドレス</summary>
		public const string TW_AFTERPAY_FIELD_EMAIL = "email";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 身分証番号</summary>
		public const string TW_AFTERPAY_FIELD_ID = "id";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 性別</summary>
		public const string TW_AFTERPAY_FIELD_SEX = "sex";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 誕生日</summary>
		public const string TW_AFTERPAY_FIELD_BIRTHDAY = "birthday";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 配送方法</summary>
		public const string TW_AFTERPAY_FIELD_SHIPMENT_TYPE = "shipmentType";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 配送運送会社コード</summary>
		public const string TW_AFTERPAY_FIELD_DELIVERY_COMPANY_CODE = "deliveryCompanyCode";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 配送伝票番号</summary>
		public const string TW_AFTERPAY_FIELD_SLIP_NUMBER = "slipNumber";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 コンビニコード</summary>
		public const string TW_AFTERPAY_FIELD_CVS_CODE = "cvsCode";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 コンビニ店舗コード</summary>
		public const string TW_AFTERPAY_FIELD_CVS_STORE_CODE = "cvsStoreCode";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 コンビニ店舗名</summary>
		public const string TW_AFTERPAY_FIELD_CVS_STORE_NAME = "cvsStoreName";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 電話番号</summary>
		public const string TW_AFTERPAY_FIELD_TEL = "tel";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 商品単価</summary>
		public const string TW_AFTERPAY_FIELD_ITEMS_PRICE = "price";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 個数</summary>
		public const string TW_AFTERPAY_FIELD_ITEMS_QUANTITY = "quantity";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 レスポンスコード</summary>
		public const string TW_AFTERPAY_FIELD_CODE = "code";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 メッセージ</summary>
		public const string TW_AFTERPAY_FIELD_MESSAGE = "message";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 エラー情報</summary>
		public const string TW_AFTERPAY_FIELD_ERRORS = "errors";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 エラーコード</summary>
		public const string TW_AFTERPAY_FIELD_ERROR_CODE = "errorCode";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 問合せ番号</summary>
		public const string TW_AFTERPAY_FIELD_ORDER_CODE = "orderCode";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 理由コード</summary>
		public const string TW_AFTERPAY_FIELD_REASON = "reason";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 対象フィールド</summary>
		public const string TW_AFTERPAY_FIELD_FIELDS = "fields";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 承認番号</summary>
		public const string TW_AFTERPAY_FIELD_ACCEPT_NUMBER = "acceptNumber";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 加盟店注文番号</summary>
		public const string TW_AFTERPAY_FIELD_STORE_ORDER_CODE = "storeOrderCode";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 結果</summary>
		public const string TW_AFTERPAY_FIELD_RESULT = "result";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 シークレットキー</summary>
		public const string TW_AFTERPAY_FIELD_SECRET_KEY = "secretKey";
		/// <summary>後付款(TriLink後払い)：APIフィールド名 アクセストークン</summary>
		public const string TW_AFTERPAY_FIELD_ACCESS_TOKEN = "accessToken";
		/// <summary>性別</summary>
		public const string FLG_TW_AFTERPAY_SEX_MALE = "1";
		public const string FLG_TW_AFTERPAY_SEX_FEMALE = "2";
		/// <summary>配送方法(01：通常宅配 02：コンビニ受取) ※越境サービスの場合、コンビニ受取は指定不可</summary>
		public const string FLG_TW_AFTERPAY_SHIPMENT_TYPE = "01";
		/// <summary>審査結果(OK)</summary>
		public const string FLG_TW_AFTERPAY_AUTH_OK = "02";
		/// <summary>審査結果(NG)</summary>
		public const string FLG_TW_AFTERPAY_AUTH_NG = "03";
	}
}
