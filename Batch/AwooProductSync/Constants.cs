/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.AwooProductSync
{
	/// <summary>
	/// 定数定義
	/// </summary>
	class Constants : w2.App.Common.Constants
	{
		/// <summary>awoo連携ファイル出力パス</summary>
		public static string AWOO_PRODUCT_SYNC_FILE_PATH = "";
		/// <summary>最大文字数：商品ID</summary>
		public const int AWOO_PRODUCT_SYNC_MAX_LENGTH_PRODUCT_ID = 50;
		/// <summary>最大文字数：タイトル</summary>
		public const int AWOO_PRODUCT_SYNC_MAX_LENGTH_TITLE = 512;
		/// <summary>最大文字数：商品ディスクリプション</summary>
		public const int AWOO_PRODUCT_SYNC_MAX_LENGTH_DESCRIPTION = 65535;
		/// <summary>最大文字数：商品URL</summary>
		public const int AWOO_PRODUCT_SYNC_MAX_LENGTH_LINK = 2048;
		/// <summary>最大文字数：商品画像URL</summary>
		public const int AWOO_PRODUCT_SYNC_MAX_LENGTH_IMAGE_LINK = 2048;
		/// <summary>最大文字数：ブランド</summary>
		public const int AWOO_PRODUCT_SYNC_MAX_LENGTH_BRAND = 64;
		/// <summary>最大文字数：商品カテゴリ</summary>
		public const int AWOO_PRODUCT_SYNC_MAX_LENGTH_PRODUCT_TYPE = 750;
	}
}
