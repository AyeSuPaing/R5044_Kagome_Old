/*
=========================================================================================================
  Module      : CrossPoint API ユーザー登録結果 (InsertResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// CrossPoint API ユーザー登録結果
	/// </summary>
	[XmlRoot("InsMember")]
	public class InsertResponse : UserResponse
	{
	}
}
