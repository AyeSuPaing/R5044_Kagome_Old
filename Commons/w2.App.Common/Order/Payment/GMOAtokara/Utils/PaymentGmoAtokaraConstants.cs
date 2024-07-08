/*
=========================================================================================================
  Module      : GMOアトカラ 定数クラス(PaymentGmoAtokaraHashCalculator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMOAtokara.Utils
{
	/// <summary>
	///  GMOアトカラ 定数クラス
	/// </summary>
	public class PaymentGmoAtokaraConstants
	{
		/// <summary>処理結果：成功</summary>
		public const string RESULT_OK = "OK";
		/// <summary>処理結果：失敗</summary>
		public const string RESULT_NG = "NG";

		/// <summary>審査結果：成功</summary>
		public const string AUTHORRESULT_OK = "OK";
		/// <summary>審査結果：失敗</summary>
		public const string AUTHORRESULT_NG = "NG";
		/// <summary>審査結果：審査中</summary>
		public const string AUTHORRESULT_REVIEW = "審査中";
		/// <summary>審査結果：保留</summary>
		public const string AUTHORRESULT_HOLD = "保留";

		/// <summary>取引種別：締め取引</summary>
		public const string TRANSACTIONTYPE_CLOSING = "1";
	}
}
