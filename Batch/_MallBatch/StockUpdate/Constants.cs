/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.MallBatch.StockUpdate
{
	///**************************************************************************************
	/// <summary>
	///  定数定義
	/// </summary>
	///**************************************************************************************
	class Constants : w2.App.Common.Constants
	{
		public static string MKADV_OUTPUT_DIR = null;
		public static string TMP_SAVE_TO = null;
		public static int YAHOO_ADD_MINUTE = 0;
		public static string USE_VARIATION = "UseVariation";
		
		public static string PROGRAMS_MKADV = null;
		public static bool YAHOO_APIFLG = false;
		public static string YAHOO_ATTESTATIONKEY = null;
		public static string YAHOO_GETURL = null;
		public static string YAHOO_STOREID = null;
		public static int YAHOO_TSADDMIN = 0;
		public static string YAHOO_UPDATEURL = null;

		/// <summary>出品者SKU紐づけ項目(バリエーションなし)</summary>
		public static string SELLERSKU_FOR_NO_VARIATION = "";
		/// <summary>出品者SKU紐づけ項目(バリエーションあり)</summary>
		public static string SELLERSKU_FOR_USE_VARIATION = "";
		/// <summary>出荷作業日数の連携に使用する項目(バリエーションなし)</summary>
		public static string FULFILLMENTLATENCY_FOR_NO_VARIATION ="";
		/// <summary>出荷作業日数の連携に使用する項目(バリエーションあり)</summary>
		public static string FULFILLMENTLATENCY_FOR_USE_VARIATION = "";
		/// <summary>デフォルト出荷作業日数</summary>
		public static int FULFILLMENTLATENCY_DEFAULT = 0;
		///// <summary>FeedProcessingStatus(リクエスト受付)</summary>
		public const string FEED_PROCESSING_STATUS_SUBMITTED = "_SUBMITTED_";
		/// <summary>送受信リクエスト・レスポンスをログに記載するかどうか</summary>
		public static bool WRITE_DEBUG_LOG_ENABLED = false;
		/// <summary>送受信リクエスト・レスポンスをログに記載するかどうか</summary>
		public static bool MASK_PERSONAL_INFO_ENABLED = true;

		/// <summary>＆mall:定数:w2側 在庫データ保持ディレクトリパス</summary>
		public static string ANDMALL_CLIENT_TEMP_DIR_PATH = "";
		/// <summary>＆mall:定数:w2側 在庫データファイルのバックアップ日数</summary>
		public static int ANDMALL_CLIENT_TEMP_FILE_BACKUP_DAY = 10;
		/// <summary>＆mall:定数:＆モール側 在庫データ保持ディレクトリパス</summary>
		public static string ANDMALL_SERVER_DIR_PATH = "";
	}
}
