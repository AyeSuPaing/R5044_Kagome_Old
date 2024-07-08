/*
=========================================================================================================
  Module      : Delete Response (DeleteResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// CrossPoint API ユーザー削除結果
	/// </summary>
	[XmlRoot("DelMember")]
	public class DeleteResponse : UserResponse
	{
	}
}
