/*
=========================================================================================================
  Module      : 決済結果 (PaypayGmoResult.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// 決済結果
	/// </summary>
	[Flags]
	public enum Results
	{
		/// <summary>なし</summary>
		None = (int)default(Results),
		/// <summary>成功</summary>
		Success = 1,
		/// <summary>失敗</summary>
		Failed = 1 << 1,
		/// <summary>リダイレクトが必要</summary>
		RedirectionIsRequired = 1 << 2,
		/// <summary>仮注文ロールバックが必要</summary>
		PreOrderRollbackIsRequired = 1 << 3,
	}

	/// <summary>
	/// 決済ステータス
	/// </summary>
	public enum Statuses
	{
		/// <summary>処理されず</summary>
		Unprocessed = (int)default(Statuses),
		/// <summary>与信済み（残高確保済み）</summary>
		Auth,
		/// <summary>売上確定済み</summary>
		Captured,
		/// <summary>キャンセル済み</summary>
		Canceled,
		/// <summary>期限切れ</summary>
		Expired,
		/// <summary>Return</summary>
		Return,
		/// <summary>Register</summary>
		Register,
		/// <summary>不明</summary>
		Unknown,
	}

	/// <summary>
	/// 決済結果
	/// </summary>
	public class PaypayGmoResult
	{
		/// <summary>結果</summary>
		public Results Result { get; set; }
		/// <summary>決済処理ステータス</summary>
		public Statuses Status { get; set; }
		/// <summary>リダイレクトが必要な場合のリダイレクトURL</summary>
		public string RedirectUrl { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>PayPayトラッキングID</summary>
		public string PaypayTrackingId { get; set; }
	}
}
