/*
=========================================================================================================
  Module      : Paypay定数定義 (PaypayConstants.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// Paypay定数定義
	/// </summary>
	public static class PaypayConstants
	{
		/// <summary>リクエストキー：注文ID</summary>
		public const string REQUEST_KEY_ORDERID = "od";
		/// <summary>リクエストキー：コマンド</summary>
		public const string REQUEST_KEY_COMMAND = "command";

		/// <summary>外部リクエストキー：取引ID</summary>
		public const string EXTERNAL_REQUEST_KEY_PAYPAY_ACCESS_ID = "AccessID";
		/// <summary>外部リクエストキー：取引パスワード</summary>
		public const string EXTERNAL_REQUEST_KEY_PAYPAY_TOKEN = "Token";

		/// <summary>リクエストキー：メルペイ受信結果</summary>
		public const string REQUEST_KEY_RECEIVE_RESULT = "r";
		/// <summary>メルペイ受信結果OK</summary>
		public const string FLG_SBPS_EXECUTE_RESULT_OK = "0";
		/// <summary>メルペイ受信結果NG</summary>
		public const string FLG_SBPS_EXECUTE_RESULT_NG = "1";
		/// <summary>メルペイ受信結果OK</summary>
		public const string FLG_GMO_EXECUTE_RESULT_OK = "0";
		/// <summary>メルペイ受信結果NG</summary>
		public const string FLG_GMO_EXECUTE_RESULT_NG = "1";

		/// <summary>決済方法：ペイペイ</summary>
		internal const string FLG_PAYTYPE_PAYPAY = "45";

		/// <summary>PayPay決済ステータス：未決済</summary>
		/// <remarks>
		/// 決済が完了していない状態（お客様のご注文途中の離脱が主な原因）
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_UNPROCESSED = "UNPROCESSED";
		/// <summary>PayPay決済ステータス：要求成功</summary>
		/// <remarks>
		/// 決済準備が整った状態（まだお客様は支払っていません）
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_REQSUCCESS = "REQSUCCESS";
		/// <summary>PayPay決済ステータス：認証処理中</summary>
		/// <remarks>
		/// お客様がPayPayで支払い操作中の状態
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_AUTHPROCESS = "AUTHPROCESS";
		/// <summary>PayPay決済ステータス：仮売上</summary>
		/// <remarks>
		/// 決済が正常に完了し、ご利用枠を確保した状態
		/// （PayPay残高から指定した金額を確保します）
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_AUTH = "AUTH";
		/// <summary>PayPay決済ステータス：実売上</summary>
		/// <remarks>
		/// 売上が確定した状態
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_SALES = "SALES";
		/// <summary>PayPay決済ステータス：即時売上</summary>
		/// <remarks>
		/// 仮売上を行わず、売上を確定した状態
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_CAPTURE = "CAPTURE";
		/// <summary>PayPay決済ステータス：キャンセル</summary>
		/// <remarks>
		/// 仮売上の取引がキャンセルされた状態
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_CANCEL = "CANCEL";
		/// <summary>PayPay決済ステータス：返金</summary>
		/// <remarks>
		/// 売上を確定した取引がキャンセルされた状態
		/// （確定した売上金をPayPay残高に戻します）
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_RETURN = "RETURN";
		/// <summary>PayPay決済ステータス：決済失敗</summary>
		/// <remarks>
		/// お客様による支払い中断や、システムトラブル等の何らかの原因でお支払いに失敗してしまった状態
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_PAYFAIL = "PAYFAIL";
		/// <summary>PayPay決済ステータス：期限切れ</summary>
		/// <remarks>
		/// 認証処理中の状態を一定時間経過した状態
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_EXPIRED = "EXPIRED";
		/// <summary>メルペイ取引ステータス：仮売上受付</summary>
		/// <remarks>
		/// メルペイが仮売上を受付けた状態
		/// </remarks>
		public const string FLG_PAYPAY_STATUS_REQAUTH = "REQAUTH";
		/// <summary>Paypay transaction status：register</summary>
		public const string FLG_PAYPAY_STATUS_REGISTER = "REGISTER";

		/// <summary>エラー情報定数：エンドユーザーによるキャンセル</summary>
		public const string FLG_ERROR_INFORMATION_CANCELED_BY_USER = "MP1000001";

		/// <summary>PayPay決済タイプ：随時決済</summary>
		public const string FLG_PAYPAY_PAYMENT_TYPE = "1";
		/// <summary>PayPayコマンドタイプ：承認</summary>
		public const string FLG_PAYPAY_COMMAND_TYPE_AUTHORIZE = "Authorize";

	}
}
