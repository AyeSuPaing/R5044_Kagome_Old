/*
=========================================================================================================
  Module      : CrossPoint API ユーザー結合結果 (MergeResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// CrossPoint API ユーザー結合結果
	/// </summary>
	[XmlRoot("MergeMemberInfo")]
	public class MergeResponse : UserResponse
	{
	}
}
