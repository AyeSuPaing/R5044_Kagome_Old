/*
=========================================================================================================
  Module      : Elogit Constants (ElogitConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Elogit
{
	/// <summary>
	/// Elogit constants
	/// </summary>
	public class ElogitConstants
	{
		/// <summary>Syori kbn file create instructions</summary>
		public const string SYORI_KBN_FILE_CREATE_INSTRUCTIONS = "1";
		/// <summary>Syori kbn get status if history</summary>
		public const string SYORI_KBN_GET_STATUS_IF_HISTORY = "2";
		/// <summary>Syori kbn file download</summary>
		public const string SYORI_KBN_FILE_DOWNLOAD = "3";
		/// <summary>Api mode</summary>
		public const string API_MODE = "TRUE";
		/// <summary>IF category cd upload</summary>	
		public const string IF_CATEGORY_CD_UPLOAD = "SO";
		/// <summary>IF category cd download</summary>
		public const string IF_CATEGORY_CD_DOWNLOAD = "WB";
		/// <summary>Target type</summary>
		public const string TARGET_TYPE = "Owner";
		/// <summary>Where condition</summary>
		public const string WHERE_CONDITION = "1,@@ shiping_start_date @@,@@ shipping_end_date @@";
		/// <summary>Response message processing</summary>
		public const string RESPONSE_MESSAGE_PROCESSING = "処理中";
		/// <summary>Key response message</summary>
		public const string KEY_RESPONSE_MESSAGE = "response_message";
		/// <summary>Key target order id</summary>
		public const string KEY_TARGET_ORDER_ID = "target_order_id";
		/// <summary>Key downloaded success</summary>
		public const string KEY_DOWNLOADED_SUCCESS = "downloaded_success";
		/// <summary>Key file download linkage fail</summary>
		public const string KEY_FILE_DOWNLOAD_LINKAGE_FAIL = "file_download_linkage_fail";
		/// <summary>Key file upload linkage</summary>
		public const string KEY_FILE_UPLOAD_LINKAGE = "file_upload_linkage";
		/// <summary>Key if history status failed</summary>
		public const string KEY_IF_HISTORY_STATUS_FAILED = "if_history_status_failed";
		/// <summary>Key download fail</summary>
		public const string KEY_DOWNLOAD_FAIL = "download_fail";
		/// <summary>Key log text</summary>
		public const string KEY_LOG_TEXT = "log_text";
		/// <summary>Key IF history key</summary>
		public const string KEY_IF_HISTORY_KEY = "if_history_key";
		/// <summary>Key upload success</summary>
		public const string KEY_UPLOAD_SUCCESS = "upload_success";
		/// <summary>Key file upload linkage fail</summary>
		public const string KEY_FILE_UPLOAD_LINKAGE_FAIL = "file_upload_linkage_fail";
	}
}
