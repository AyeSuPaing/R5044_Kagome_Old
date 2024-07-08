/*
=========================================================================================================
  Module      : Delete Response (DeleteResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// CrossPoint API 購買ポイント取消レスポンス
	/// </summary>
	[XmlRoot("DelPurchasePoint")]
	public class DeleteResponse : ResponseBase<PointApiResult>
	{
	}
}
