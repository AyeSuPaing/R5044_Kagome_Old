/*
=========================================================================================================
  Module      : Get Response (GetResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// CrossPoint API 発行ポイント取得レスポンス
	/// </summary>
	[XmlRoot("GetGrantPointByDetail")]
	public class GetResponse : ResponseBase<PointApiResult>
	{
	}
}
