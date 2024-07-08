/*
=========================================================================================================
  Module      : AwooAPIリクエスト基底リクエストデータ (AwooApiGetRequestBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Common.Web;

namespace w2.App.Common.Awoo
{
	/// <summary>
	/// AwooAPIリクエスト基底リクエストデータ
	/// </summary>
	public abstract class AwooApiGetRequestBase : IHttpApiRequestData
	{
		/// <summary>保持データ</summary>
		protected IDictionary<string, string> m_data = new Dictionary<string, string>();

		/// <summary>
		/// クエリ文字列生
		/// </summary>
		/// <returns>POST用の文字列</returns>
		public string CreatePostString()
		{
			return string.Join("&", this.m_data.Select(kvp => string.Concat(kvp.Key, "=", HttpUtility.UrlEncode(kvp.Value))).ToArray());
		}
	}
}
