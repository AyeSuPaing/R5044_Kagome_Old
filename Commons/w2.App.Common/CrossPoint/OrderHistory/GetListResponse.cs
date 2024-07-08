/*
=========================================================================================================
  Module      : CrossPoint API 伝票リスト取得レスポンス (GetListResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.OrderHistory
{
	/// <summary>
	/// CrossPoint API 伝票リスト取得レスポンス
	/// </summary>
	[XmlRoot("GetSlipList")]
	public class GetListResponse : ResponseBase<OrderHistoryApiResult>
	{
	}
}
