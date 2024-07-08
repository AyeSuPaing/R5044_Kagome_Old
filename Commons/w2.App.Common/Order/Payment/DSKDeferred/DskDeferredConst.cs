/*
=========================================================================================================
  Module      : DSK後払い定数(DskDeferredConst.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.DSKDeferred
{
	/// <summary>決済種別</summary>
	public enum DSKDeferredPaymentType
	{
		/// <summary>別送サービス</summary>
		[XmlEnum(Name = "2")]
		SeparateService,
		/// <summary>同梱サービス</summary>
		[XmlEnum(Name = "3")]
		IncludeService
	}

	/// <summary>
	/// DSK後払い定数
	/// </summary>
	public static class DskDeferredConst
	{
		/// <summary>連携行数上限</summary>
		public const int MAXIMUM_NUMBER_OF_COOPERATION_ROWS = 100;
		/// <summary>商品項目行数上限</summary>
		public const int MAXIMUM_NUMBER_OF_PRODUCT_ITEM_ROWS = 97;

		/// <summary>処理結果：OK</summary>
		public const string RESULT_OK = "OK";
		/// <summary>処理結果：NG</summary>
		public const string RESULT_NG = "NG";

		/// <summary>自動審査結果：OK</summary>
		public const string AUTO_AUTH_RESULT_OK = "OK";
		/// <summary>自動審査結果：NG</summary>
		public const string AUTO_AUTH_RESULT_NG = "NG";
		/// <summary>自動審査結果：審査中</summary>
		public const string AUTO_AUTH_RESULT_HOLD = "HOLD";

		/// <summary>与信結果取得API審査結果：OK</summary>
		public const string GET_AUTH_RESULT_AUTH_RESULT_OK = "OK";
		/// <summary>与信結果取得API審査結果：NG</summary>
		public const string GET_AUTH_RESULT_AUTH_RESULT_NG = "NG";
		/// <summary>与信結果取得API審査結果：保留</summary>
		public const string GET_AUTH_RESULT_AUTH_RESULT_PENDING = "保留";
		/// <summary>与信結果取得API審査結果：HOLD</summary>
		public const string GET_AUTH_RESULT_AUTH_RESULT_HOLD = "HOLD";
	}
}
