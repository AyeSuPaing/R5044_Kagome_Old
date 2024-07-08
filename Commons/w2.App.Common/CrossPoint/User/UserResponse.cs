/*
=========================================================================================================
  Module      : CrossPoint API ユーザー基底レスポンス (UserResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Helper;

namespace w2.App.Common.CrossPoint.User
{
	/// <summary>
	/// CrossPoint API ユーザー基底レスポンス
	/// </summary>
	public class UserResponse : ResponseBase<UserApiResult>
	{
		/// <summary>
		/// 結果を取得
		/// </summary>
		/// <typeparam name="TResponse">レスポンス</typeparam>
		/// <param name="response">レスポンスデータ</param>
		/// <returns>結果セット</returns>
		public override ResultSet<UserApiResult> GetResultSet<TResponse>(string response)
		{
			this.ResultSet = SerializeHelper.Deserialize<TResponse>(response).ResultSet;
			this.ResultSet.XmlResponse = response;
			if (this.ResultSet.TotalResult == 0) return this.ResultSet;

			// 動的なパラメータ名への対応
			foreach (var result in this.ResultSet.Result)
			{
				if (result.OtherElements == null) continue;

				var netShopMemberId = string.Format(
					"{0}{1}",
					Constants.CROSS_POINT_ELEMENT_MEMBER_INFO_NET_SHOP_MEMBER_ID,
					Constants.CROSS_POINT_AUTH_SHOP_CODE);

				result.NetShopMemberId = GetElementsByName(result.OtherElements, netShopMemberId);
			}

			return this.ResultSet;
		}
	}
}
