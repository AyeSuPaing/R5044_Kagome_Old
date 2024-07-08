/*
=========================================================================================================
  Module      : RecustomerAPIリクエスト基底リクエストデータ (RecustomerApiPostRequestBase.cs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using w2.Common.Web;

namespace w2.App.Common.Recustomer
{
	/// <summary>
	/// RecustomerAPIリクエスト基底リクエストデータ
	/// </summary>
	public abstract class RecustomerApiPostRequestBase : IHttpApiRequestData
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
