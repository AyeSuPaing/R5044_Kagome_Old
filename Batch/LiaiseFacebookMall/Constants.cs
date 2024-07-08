/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.LiaiseFacebookMall
{
	/// <summary>
	/// 定数定義
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		/// <summary>Facebook catalog api datetime format</summary>
		public const string FACEBOOK_CATALOG_API_DATETIME_FORMAT = "yyyy-MM-dd'T'HH:mm-0300";
		/// <summary>Facebook catalog api method</summary>
		/// <remarks>Value is CREATE or UPDATE</remarks>
		public const string FACEBOOK_CATALOG_API_METHOD = "CREATE";
		/// <summary>Facebook catalog api max request count</summary>
		public const int FACEBOOK_CATALOG_API_MAX_REQUEST_COUNT = 500;
		/// <summary>Facebook catalog api response error log message</summary>
		public const string FACEBOOK_CATALOG_API_RESPONSE_ERROR_LOG_MESSAGE = "システム管理者にお問い合わせください。";
	}
}