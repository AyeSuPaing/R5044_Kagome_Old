/*
=========================================================================================================
  Module      : Modify Response (ModifyResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// CrossPoint API 購買ポイント一部取消レスポンス
	/// </summary>
	[XmlRoot("CancelPurchasePointByDetail")]
	public class ModifyResponse : ResponseBase<PointApiResult>
	{
	}
}
