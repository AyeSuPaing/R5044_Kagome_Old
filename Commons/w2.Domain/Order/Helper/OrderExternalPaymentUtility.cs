/*
=========================================================================================================
  Module      : 外部決済ユーティリティ(OrderExternalPaymentUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Order.Helper
{
	/// <summary>
	/// OrderExternalPaymentUtility の概要の説明です
	/// </summary>
	public static class OrderExternalPaymentUtility
	{
		/// <summary>
		/// 決済連携メモ取得
		/// </summary>
		/// <param name="beforePaymentMemo">既存の決済連携メモ</param>
		/// <param name="newPaymentMemo">新規の決済連携メモ</param>
		/// <returns>決済連携メモ</returns>
		public static string SetExternalPaymentMemo(string beforePaymentMemo, string newPaymentMemo)
		{
			var result = string.IsNullOrEmpty(beforePaymentMemo)
				? newPaymentMemo
				: string.Join(Environment.NewLine, beforePaymentMemo, newPaymentMemo);
			return result.Trim();
		}
		/// <summary>
		/// 決済連携メモ取得
		/// </summary>
		/// <param name="beforePaymentMemo">既存の決済連携メモ</param>
		/// <param name="newPaymentMemo">新規の決済連携メモ</param>
		/// <param name="isOrderCombine">同梱か</param>
		/// <returns>決済連携メモ</returns>
		public static string SetExternalPaymentMemo(string beforePaymentMemo, string newPaymentMemo, bool isOrderCombine)
		{
			var result = (isOrderCombine == false)
				? newPaymentMemo
				: string.Join(Environment.NewLine, beforePaymentMemo, newPaymentMemo);
			return result.Trim();
		}
	}
}
