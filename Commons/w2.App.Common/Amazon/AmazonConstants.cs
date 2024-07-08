/*
=========================================================================================================
  Module      : Amazon用定数定義クラス(AmazonConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Amazon
{
	/// <summary>
	/// Amazon用定数定義クラス
	/// </summary>
	public class AmazonConstants
	{
		/// <summary>Amazonトークンリクエストキー</summary>
		public const string REQUEST_KEY_AMAZON_TOKEN = "access_token";
		/// <summary>Amazonステートリクエストキー</summary>
		public const string REQUEST_KEY_AMAZON_STATE = "state";
		/// <summary>Amazonモデルセッションキー</summary>
		public const string SESSION_KEY_AMAZON_MODEL = "w2Front_skey_amazon_model";
		/// <summary>Amazonアドレス帳情報セッションキー</summary>
		public const string SESSION_KEY_AMAZON_ADDRESS = "w2Front_skey_amazon_address";
		/// <summary>Amazon注文者アドレス帳情報セッションキー</summary>
		public const string SESSION_KEY_AMAZON_OWNER_ADDRESS = "w2Front_skey_amazon_owner_address";
		/// <summary>Amazon配送先アドレス帳情報セッションキー</summary>
		public const string SESSION_KEY_AMAZON_SHIPPING_ADDRESS = "w2Front_skey_amazon_shipping_address";
		/// <summary>Amazon決済デスクリプター</summary>
		public const string SESSION_KEY_AMAZON_PAYMENT_DESCRIPTOR = "w2Front_skey_amazon_payment_discriptor";
		/// <summary>Amazonアドレス帳ウィジェットエラーメッセージセッションキー</summary>
		public const string SESSION_KEY_AMAZON_ADDRESS_ERROR_MSG = "w2Front_skey_amazon_address_error_message";
		/// <summary>Amazon注文者アドレス帳ウィジェットエラーメッセージセッションキー</summary>
		public const string SESSION_KEY_AMAZON_OWNER_ADDRESS_ERROR_MSG = "w2Front_skey_amazon_owner_address_error_message";
		/// <summary>Amazon配送先アドレス帳ウィジェットエラーメッセージセッションキー</summary>
		public const string SESSION_KEY_AMAZON_SHIPPING_ADDRESS_ERROR_MSG = "w2Front_skey_amazon_shipping_address_error_message";

		/// <summary>
		/// 住所種別
		/// </summary>
		public enum AddressType
		{
			/// <summary>注文者情報</summary>
			Owner,
			/// <summary>配送先情報</summary>
			Shipping,
		}

		/// <summary>
		/// 注文種別
		/// </summary>
		public enum OrderType
		{
			/// <summary>ワンタイム</summary>
			OneTime,
			/// <summary>Auto Pay</summary>
			AutoPay,
		}

		/// <summary>
		/// オブジェクト状態
		/// </summary>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201752820"/>
		public enum State
		{
			/// <summary>Pending</summary>
			Pending,
			/// <summary>Open</summary>
			Open,
			/// <summary>Declined</summary>
			Declined,
			/// <summary>Closed</summary>
			Closed,
			/// <summary>Completed</summary>
			Completed,
		}
	}
}