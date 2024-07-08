/*
=========================================================================================================
  Module      : HttpApiのレスポンス値インタフェース(IHttpApiResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Common.Web
{
	/// <summary>
	/// HttpApiのレスポンス値インタフェース
	/// </summary>
	public interface IHttpApiResponseData
	{
		/// <summary>
		/// レスポンス文字列生成
		/// </summary>
		/// <returns>レスポンス文字列</returns>
		string CreateResponseString();
	}
}
