/*
=========================================================================================================
  Module      : 楽天受注連携API例外クラス (RakutenApiCoopException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Mall.Rakuten
{
	/// <summary>
	/// 楽天受注連携API例外クラス
	/// </summary>
	public class RakutenApiCoopException : Exception
	{
		public const string ORDER_GET_ERROR = "注文情報の取得に失敗しました。";
		public const string ORDER_GET_PROGRESS_RETRY = "注文情報のステータスが取込対象ではないため保留します。";
		public const string ORDER_GET_PROGRESS_ERROR = "注文情報のステータスが取込対象ではありません。";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		public RakutenApiCoopException(string message)
			: base(message)
		{
			// 処理なし
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="ex">例外エラー</param>
		public RakutenApiCoopException(string message, Exception ex)
			: base(message, ex)
		{
			// 処理なし
		}
	}
}
