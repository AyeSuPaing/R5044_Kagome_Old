<%--
=========================================================================================================
  Module      : LINEミニアプリログ出力ハンドラー(LineMiniAppLogExporter.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="LineMiniAppLogExporter" %>
using System.IO;
using System.Web;
using Newtonsoft.Json;
using w2.Common.Logger;

public class LineMiniAppLogExporter : IHttpHandler
{
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		if (context.Request.HttpMethod != "POST") return;

		string json;
		using (var reader = new StreamReader(context.Request.GetBufferedInputStream()))
		{
			json = reader.ReadToEnd();
		}
		if (string.IsNullOrEmpty(json)) return;

		var request = JsonConvert.DeserializeObject<ExportLogInformation>(json);
		FileLogger.Write(request.LogType, request.Message);
	}

	///  <summary>IsReusable</summary>
	public bool IsReusable { get { return false; } }

	/// <summary>
	/// ログ出力リクエストボディ
	/// </summary>
	[JsonObject]
	public class ExportLogInformation
	{
		[JsonProperty("log_type")]
		public string LogType { get; set; }
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}