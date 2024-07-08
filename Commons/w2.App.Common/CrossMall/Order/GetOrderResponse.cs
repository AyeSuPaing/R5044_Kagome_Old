/*
=========================================================================================================
  Module      : 注文取得するレスポンスクラス (GetOrderResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossMall.Order
{
	/// <summary>
	/// 注文取得するレスポンスクラス
	/// </summary>
	[XmlRoot("GetOrder")]
	public class GetOrderResponse : CrossMallResponseBase<GetOrderResult>
	{
	}
}
