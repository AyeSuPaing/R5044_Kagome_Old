/*
=========================================================================================================
  Module      : Get Point History Response (GetPointHistoryResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.PointHistory
{
	/// <summary>
	/// CrossPoint API ポイント履歴取得レスポンス
	/// </summary>
	[XmlRoot("GetPointHisList")]
	public class GetPointHistoryResponse : ResponseBase<PointHistoryApiResult>
	{
	}
}
