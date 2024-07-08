/*
=========================================================================================================
  Module      : CrossPoint API ポイント登録レスポンス (RegisterResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.Point
{
	/// <summary>
	/// CrossPoint API ポイント登録レスポンス
	/// </summary>
	[XmlRoot("UpdPurchasePointByDetail")]
	public class RegisterResponse : ResponseBase<PointApiResult>
	{
	}
}
