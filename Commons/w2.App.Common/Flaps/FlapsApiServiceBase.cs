/*
=========================================================================================================
  Module      : FLAPS API サービス基底クラス(FlapsApiServiceBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Linq;
using Newtonsoft.Json;

namespace w2.App.Common.Flaps
{
	/// <summary>
	/// サービス基底クラス
	/// </summary>
	public class FlapsApiServiceBase
	{
		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <param name="requestXml">リクエスト文</param>
		/// <param name="apiName">API名</param>
		/// <returns>レスポンスデータ</returns>
		protected static TResult GetResult<TResult>(XDocument requestXml, string apiName)
			where TResult : ResultBase, new()
		{
			var response = new FlapsApiRequest().GetResponse(requestXml, apiName);
			var result = JsonConvert.DeserializeObject<TResult>(response) ?? new TResult();

			return result;
		}
	}
}
