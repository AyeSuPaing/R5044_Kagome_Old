/*
=========================================================================================================
  Module      : Get Response (GetResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// CrossPoint API ユーザー取得レスポンス
	/// </summary>
	[XmlRoot("GetMemberInfoWithPinCd")]
	public class GetResponse : UserResponse
	{
	}
}
