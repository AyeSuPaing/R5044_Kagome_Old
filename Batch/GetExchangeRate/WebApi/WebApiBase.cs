/*
=========================================================================================================
  Module      : WebAPIベースクラス(WebApiBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace w2.Commerce.GetExchangeRate.WebApi
{
	/// <summary>
	/// WebAPIベースクラス
	/// </summary>
	internal class WebApiBase
	{
		/// <summary>
		/// GETメソッド
		/// </summary>
		/// <param name="url">URL</param>
		/// <returns>レスポンス(文字列)</returns>
		internal string Get(Uri url)
		{
			using (var client = new HttpClient())
			{
				var response = client.GetStringAsync(url).Result;
				return response;
			}
		}
	}
}