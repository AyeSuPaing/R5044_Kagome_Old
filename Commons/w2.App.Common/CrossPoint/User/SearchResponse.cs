/*
=========================================================================================================
  Module      : CrossPoint API ユーザー検索結果取得 (SearchResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// CrossPoint API ユーザー検索結果取得
	/// </summary>
	[XmlRoot("GetMemberListWithPinCd")]
	public class SearchResponse : UserResponse
	{
	}
}
