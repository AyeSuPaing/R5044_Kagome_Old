/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace AmazonMarketPlaceWebService
{
	/// <summary>
	/// 定数定義
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		/// <summary>アプリケーションバージョン(MWS連携に使用)</summary>
		public const string APPLICATION_VERSION = "1.0";
		/// <summary>Amazonドキュメントバージョン</summary>
		public const string AMAZON_FEED_DOCUMENT_VERSION = "1.0";
		/// <summary>出荷通知フィードメッセージタイプ</summary>
		public const string AMAZON_FEED_MESSAGE_TYPE_FULFILLMENT = "OrderFulfillment";
		///// <summary>在庫フィードメッセージタイプ</summary>
		public const string AMAZON_FEED_MESSAGE_TYPE_INVENTORY = "Inventory";
		///// <summary>オペレーションタイプ（更新）</summary>
		public const string AMAZON_OPERATION_TYPE_UPDATE = "Update";
		///// <summary>FeedProcessingStatus(リクエスト受付)</summary>
		public const string FEED_PROCESSING_STATUS_SUBMITTED = "_SUBMITTED_";

		/// <summary>初回取込時に何日前からの更新注文を取り込むか</summary>
		public static int GET_AMAZON_ORDERS_UPDATE_BEFORE_DAYS = 0;
		/// <summary>SubmitFeedリクエスト上限数</summary>
		public const int AMAZON_FEED_SUBMIT_MAXCOUNT = 100;
		/// <summary>SubmitFeed結果：リクエスト受付完了</summary>
		public const string AMAZON_FEED_SUBMIT_RESULT_SUBMITTED = "_SUBMITTED_";
		/// <summary>SubmitFeed結果：リクエスト処理完了</summary>
		public const string AMAZON_FEED_SUBMIT_RESULT_DONE = "_DONE_";

		/// <summary>FeedSubmissionResult実行結果出力一時ファイル名</summary>
		public const string XML_FILE_NAME_TMP_FEEDSUBMISSION_RESULT = "tmp_FeedSubmissionResult.xml";
	}
}
