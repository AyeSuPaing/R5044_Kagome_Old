/*
=========================================================================================================
  Module      : AwooAPIリクエスト基底リクエストデータ (AwooApiPostRequestBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using w2.Common.Web;

namespace w2.App.Common.Awoo
{
	/// <summary>
	/// AwooAPIリクエスト基底リクエストデータ
	/// </summary>
	public abstract class AwooApiPostRequestBase : IHttpApiRequestData
	{
		/// <summary>
		/// POST用Jsonデータ生成
		/// </summary>
		/// <returns>POST用の文字列</returns>
		public string CreatePostString()
		{
			return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
		}
	}
}
