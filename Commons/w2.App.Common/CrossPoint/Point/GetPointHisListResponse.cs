/*
=========================================================================================================
  Module      : Get Point His List Response (GetPointHisListResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.CrossPoint.Point;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// Cross point api get point history list response
	/// </summary>
	[XmlRoot("GetPointHistoryList")]
	public class GetPointHisListResponse : ResponseBase<PointApiResult>
	{
	}
}
