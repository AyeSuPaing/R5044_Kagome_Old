/*
=========================================================================================================
  Module      : Atodene定数(AtodeneConst.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.JACCS.ATODENE
{
	/// <summary>
	/// Atodene定数
	/// </summary>
	public static class AtodeneConst
	{
		/// <summary>配送完了実績フラグ：スキップ</summary>
		public const string EXTEND1_SHIPING_COMP_FLG_SKIP = "00";
		/// <summary>配送完了実績フラグ：実績あり</summary>
		public const string EXTEND1_SHIPING_COMP_FLG_YES = "01";
		/// <summary>配送完了実績フラグ：実績なし</summary>
		public const string EXTEND1_SHIPING_COMP_FLG_NO = "02";

		/// <summary>請求書サービス：別送</summary>
		public const string INVOICE_SEND_SERVICE_FLG_SEPARATE = "2";
		/// <summary>請求書サービス：同梱</summary>
		public const string INVOICE_SEND_SERVICE_FLG_INCLUDE = "3";

		/// <summary>出荷報告種別：出荷報告</summary>
		public const string DELIVERY_TYPE_SHIPMENT = "1";
		/// <summary>出荷報告種別：出荷報告変更</summary>
		public const string DELIVERY_TYPE_CHANGE = "2";
		/// <summary>出荷報告種別：出荷報告取消し</summary>
		public const string DELIVERY_TYPE_CANCEL = "3";

		/// <summary>更新種別フラグ：キャンセル</summary>
		public const string UPDATE_TYPE_FLAG_CANCEL = "1";
		/// <summary>更新種別フラグ：取引変更</summary>
		public const string UPDATE_TYPE_FLAG_CHANGE = "2";

		/// <summary>処理結果：OK</summary>
		public const string RESULT_OK = "OK";
		/// <summary>処理結果：NG</summary>
		public const string RESULT_NG = "NG";

		/// <summary>自動審査結果：OK</summary>
		public const string AUTO_AUTH_RESULT_OK = "OK";
		/// <summary>自動審査結果：NG</summary>
		public const string AUTO_AUTH_RESULT_NG = "NG";
		/// <summary>自動審査結果：審査中</summary>
		public const string AUTO_AUTH_RESULT_PROCESSING = "審査中";

		/// <summary>アトディーネ連携 商品名：商品名</summary>
		public const string FLG_COOPERATION_PRODUCTNAME_IS_PRODUCTNAME = "0";
		/// <summary>アトディーネ連携 商品名：商品名 + 商品名(カナ)</summary>
		public const string FLG_COOPERATION_PRODUCTNAME_IS_PRODUCTNAME_AND_KANA = "1";

		/// <summary>連携行数上限</summary>
		public const int MAXIMUM_NUMBER_OF_COOPERATION_ROWS = 15;
		/// <summary>商品項目連携行数上限</summary>
		public const int MAXIMUM_NUMBER_OF_PRODUCT_ITEM_COOPERATION_ROWS = 12;
	}
}
