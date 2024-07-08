/*
=========================================================================================================
  Module      : CrossPoint API ユーザー更新結果 (UpdateResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// CrossPoint API ユーザー更新結果
	/// </summary>
	[XmlRoot("UpdMember")]
	public class UpdateResponse : UserResponse
	{
	}
}
