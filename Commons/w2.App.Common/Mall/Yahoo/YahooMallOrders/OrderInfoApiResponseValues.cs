/*
=========================================================================================================
  Module      : YAHOO API 注文詳細APIレスポンス値列挙体 (OrderInfoApiResponseValues.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Helper.Attribute;

namespace w2.App.Common.Mall.Yahoo.YahooMallOrders
{
	/// <summary>
	/// デバイス種別
	/// </summary>
	internal enum DeviceType
	{
		/// <summary>PC</summary>
		[EnumTextName("1")]
		Pc,
		/// <summary>モバイル</summary>
		[EnumTextName("2")]
		Mb,
		/// <summary>スマートフォン</summary>
		[EnumTextName("3")]
		Sp,
		/// <summary>タブレット</summary>
		[EnumTextName("4")]
		Tablet,
	}

	/// <summary>
	/// Yahooモール注文ステータス
	/// </summary>
	internal enum OrderStatus
	{
		/// <summary>予約</summary>
		[EnumTextName("1")]
		Reserved,
		/// <summary>処理中</summary>
		[EnumTextName("2")]
		Processing,
		/// <summary>保留</summary>
		[EnumTextName("3")]
		Suspended,
		/// <summary>キャンセル</summary>
		[EnumTextName("4")]
		Canceled,
		/// <summary>完了</summary>
		[EnumTextName("5")]
		Completed,
	}
}
