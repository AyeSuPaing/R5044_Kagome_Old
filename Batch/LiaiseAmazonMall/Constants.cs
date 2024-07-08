/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.LiaiseAmazonMall
{
	/// <summary>
	/// 定数定義
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		/// <summary>出品者SKU紐づけ項目(バリエーションなし)</summary>
		public static string SELLERSKU_FOR_NO_VARIATION = "";
		/// <summary>出品者SKU紐づけ項目(バリエーションあり)</summary>
		public static string SELLERSKU_FOR_USE_VARIATION = "";
		/// <summary>新規ユーザ登録 顧客区分</summary>
		public static string LIAISE_AMAZON_MALL_DEFAULT_USER_KBN = "";
		/// <summary>デフォルト配送種別</summary>
		public static string LIAISE_AMAZON_MALL_DEFAULT_SHIPPING_ID = "";
		/// <summary>顧客区分：会員</summary>
		public const string USER_KBN_USER = "USER";
		/// <summary>顧客区分：ゲスト</summary>
		public const string USER_KBN_GUEST = "GUEST";

		/// <summary>FeedSubmissionResult実行結果：ProcessingSummaryノード</summary>
		public const string FEEDSUBMISSION_RESULT_XML_NODE_PROCESSING_SUMMARY = "ProcessingSummary";
		/// <summary>FeedSubmissionResult実行結果：MessagesProcessedノード</summary>
		public const string FEEDSUBMISSION_RESULT_XML_NODE_MESSAGES_PROCESSED = "MessagesProcessed";
		/// <summary>FeedSubmissionResult実行結果：MessagesSuccessfulノード</summary>
		public const string FEEDSUBMISSION_RESULT_XML_NODE_MESSAGES_SUCCESSFUL = "MessagesSuccessful";
		/// <summary>FeedSubmissionResult実行結果：MessagesWithErrorノード</summary>
		public const string FEEDSUBMISSION_RESULT_XML_NODE_MESSAGES_WITH_ERROR = "MessagesWithError";
		/// <summary>FeedSubmissionResult実行結果：MessagesWithWarningノード</summary>
		public const string FEEDSUBMISSION_RESULT_XML_NODE_MESSAGES_WITH_WARNING = "MessagesWithWarning";

		/// <summary>Amazon支払方法：Amazonポイント</summary>
		public const string AMAZON_PAYMENTMETHOD_POINTSACCOUNT = "PointsAccount";
		/// <summary>Amazon支払方法：ギフトカード</summary>
		public const string AMAZON_PAYMENTMETHOD_GIFTCARD = "GC";
		/// <summary>Amazon消費税端数処理方法</summary>
		public static string TAX_ROUNDTYPE = Constants.FLG_ORDERITEM_PRODUCT_TAX_ROUND_TYPE_ROUNDOFF;
	}
}
