/*
=========================================================================================================
  Module      : Lohaco連携用定数定義クラス(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Commerce.MallBatch.LiaiseLohacoMall
{
	/// <summary>
	/// Lohaco連携用定数定義クラス
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		/// <summary>1回取得注文件数</summary>
		public static int LOHACO_GET_ORDER_COUNT = 50;
		/// <summary>デフォルト配送種別</summary>
		public static string LOHACO_DEFAULT_SHIPPING_ID = "101";
		/// <summary>送受信リクエスト・レスポンスをログに記載するかどうか</summary>
		public static bool WRITE_DEBUG_LOG_ENABLED = false;
		/// <summary>送受信リクエスト・レスポンスをログに記載するかどうか</summary>
		public static bool MASK_PERSONAL_INFO_ENABLED = true;

		/// <summary>クレジットカード支払方法</summary>
		public const string PAY_METHOD_CREDIT_CARD = "paymen_a1";
		/// <summary>氏名1の最大文字列数</summary>
		public const int NAME1_MAX_LENGTH = 20;
		/// <summary>氏名２の最大文字列数</summary>
		public const int NAME2_MAX_LENGTH = 20;
		/// <summary>氏名かな1の最大文字列数</summary>
		public const int NAME_KANA1_MAX_LENGTH = 30;
		/// <summary>氏名かな２の最大文字列数</summary>
		public const int NAME_KANA2_MAX_LENGTH = 30;
		/// <summary>住所２～４の最大文字列数</summary>
		public const int ADDRESS_MAX_LENGTH = 50 * 3;
	}
}
