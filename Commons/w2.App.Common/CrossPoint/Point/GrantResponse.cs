/*
=========================================================================================================
  Module      : Grant Response (GrantResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// CrossPoint API ポイント確定レスポンス
	/// </summary>
	[XmlRoot("FixGrantPoint")]
	public class GrantResponse : ResponseBase<PointApiResult>
	{
	}
}
