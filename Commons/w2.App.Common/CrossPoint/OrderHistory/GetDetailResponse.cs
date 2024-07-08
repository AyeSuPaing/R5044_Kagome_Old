/*
=========================================================================================================
  Module      : CrossPoint API 伝票詳細取得レスポンス (GetDetailResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.OrderHistory
{
	/// <summary>
	/// CrossPoint API 伝票詳細取得レスポンス
	/// </summary>
	[XmlRoot("GetSlipDetail")]
	public class GetDetailResponse : ResponseBase<OrderHistoryApiResult>
	{
	}
}
