/*
=========================================================================================================
  Module      : Amazon用定数定義クラス(AmazonCv2Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.AmazonCv2
{
	/// <summary>
	/// Amazon(CV2)用定数定義クラス
	/// </summary>
	public class AmazonCv2Constants
	{
		/// <summary>AmazonチェックアウトセッションID</summary>
		public const string REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID = "amazonCheckoutSessionId";
		/// <summary>Amazonバイヤートークン</summary>
		public const string REQUEST_KEY_AMAZON_BUYER_TOKEN = "BuyerToken";
		/// <summary>アマゾンアクションステータス</summary>
		public const string REQUEST_KEY_AMAZON_ACTION_STATUS = "amazonAction";
		/// <summary>注文ID</summary>
		public const string REQUEST_KEY_ORDER_ID = "odid";
		/// <summary>定期ID</summary>
		public const string REQUEST_KEY_FIXED_PURCHASE_ID = "fxpchsid";
		/// <summary>アマゾンアクションステータス：チェックアウトセッション作成</summary>
		public const string AMAZON_ACTION_STATUS_CREATE_SESSION = "createSession";
		/// <summary>アマゾンアクションステータス：オーソリ</summary>
		public const string AMAZON_ACTION_STATUS_AUTH = "auth";
		/// <summary>AmazonチェックアウトセッションID</summary>
		public const string SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID = "amazonCheckoutSessionId";
		/// <summary>Amazonリダイレクト済み判定用セッションキー</summary>
		public const string SESSION_KEY_AMAZON_IS_REDIRECTED_CHANGE_PAGE = "amazonIsRedirectedChangePage";
		/// <summary>チャージオブジェクトステータス：売上確定済み</summary>
		public const string FLG_CHARGE_STATUS_CAPTURED = "Captured";
		/// <summary>返金処理エラーステータスコード：返金額超過</summary>
		public const int FLG_REFUND_ERROR_STATUS_TRANSACTION_AMOUNT_EXCEEDED = 400;
	}
}