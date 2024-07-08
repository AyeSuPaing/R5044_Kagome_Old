/*
=========================================================================================================
  Module      : HttpApiのレスポンス値インタフェース(IHttpApiRequestData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Common.Web
{
	/// <summary>
	/// HttpApiのリクエスト値インタフェース
	/// </summary>
	public interface IHttpApiRequestData
	{
		/// <summary>
		/// ポスト文字列生成
		/// </summary>
		/// <returns>ポスト文字列</returns>
		string CreatePostString();
	}
}
