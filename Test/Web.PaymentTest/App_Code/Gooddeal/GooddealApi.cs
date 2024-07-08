/*
=========================================================================================================
  Module      : Gooddeal Api(GooddealApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Web;
using w2.Common.Helper;

/// <summary>
/// Gooddeal Api
/// </summary>
public class GooddealApi
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="context">The context</param>
	public GooddealApi(HttpContext context)
	{
		this.Context = context;
	}

	/// <summary>
	/// Execute process
	/// </summary>
	/// <param name="elementName">Element name</param>
	/// <returns>Json response</returns>
	public string ExecProcess(string elementName)
	{
		var request = GetRequest();
		if ((request == null)
			|| string.IsNullOrEmpty(request.OrderNo))
		{
			return GetJsonResponseFromFile("Error");
		}

		var orderNo = request.OrderNo;
		var response = GetJsonResponseFromFile(elementName)
			.Replace("@@ orderNo @@", orderNo)
			.Replace("@@ now @@", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
		return response;
	}

	/// <summary>
	/// Get json response from file
	/// </summary>
	/// <param name="elementName">Element name</param>
	/// <returns>A data json</returns>
	private string GetJsonResponseFromFile(string elementName)
	{
		var filePath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Json",
			"Gooddeal",
			elementName + ".json");
		using (var sr = new StreamReader(filePath))
		{
			return sr.ReadToEnd();
		}
	}

	/// <summary>
	/// Get request
	/// </summary>
	/// <returns>A Gooddeal Request</returns>
	private w2.App.Common.Gooddeal.GooddealRequest GetRequest()
	{
		try
		{
			using (var sr = new StreamReader(this.Context.Request.InputStream))
			{
				var bodyRequest = sr.ReadToEnd();
				var request = SerializeHelper.DeserializeJson<w2.App.Common.Gooddeal.GooddealRequest>(bodyRequest);
				return request;
			};
		}
		catch
		{
		}
		return null;
	}

	/// <summary>The context</summary>
	private HttpContext Context { set; get; }
}