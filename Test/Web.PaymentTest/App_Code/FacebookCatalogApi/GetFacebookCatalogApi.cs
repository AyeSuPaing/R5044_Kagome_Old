/*
=========================================================================================================
  Module      : Get Facebook Catalog Api (GetFacebookCatalogApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using w2.App.Common.Facebook;
using w2.Common.Logger;
using w2.Common.Util;

/// <summary>
/// Get Facebook Catalog Api
/// </summary>
public class GetFacebookCatalogApi
{
	/// <summary>
	/// Process request context (POST)
	/// </summary>
	/// <param name="context">Context</param>
	public void ProcessRequestContext(HttpContext context)
	{
		this.Context = context;
		var responces = CreatedResponse();
		ResultResponse(responces);
		return;
	}

	/// <summary>
	/// Result response (application/json)
	/// </summary>
	/// <param name="facebookCatalogApi">Facebook catalog api</param>
	private void ResultResponse(FacebookCatalogApi facebookCatalogApi)
	{
		try
		{
			this.Context.Response.ContentType = "application/json";
			var facebookResponse = new FacebookCatalogResponseApi();

			if (facebookCatalogApi.ResultStatus == "OK")
			{
				facebookResponse.Handles = facebookCatalogApi.Handles.ToArray();
			}
			else
			{
				facebookResponse.Error = new FacebookCatalogErrorApi
				{
					Message = facebookCatalogApi.Error.Message,
					Type = facebookCatalogApi.Error.Type,
					Code = ObjectUtility.TryParseInt(facebookCatalogApi.Error.Code, 0),
					FbtraceId = facebookCatalogApi.Error.FbtraceId
				};
			}

			var response = JsonConvert.SerializeObject(facebookResponse, Formatting.Indented);
			this.Context.Response.Write(response);
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
		}
	}

	/// <summary>
	/// Created response
	/// </summary>
	/// <returns>Response</returns>
	private FacebookCatalogApi CreatedResponse()
	{
		var facebookApi = new FacebookCatalogApi();
		try
		{
			var path = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				"Xml",
				"Facebook",
				"FacebookCatalog.xml");
			using (var fs = new FileStream(path, FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(FacebookCatalogApi));
				facebookApi = (FacebookCatalogApi)serializer.Deserialize(fs);
			}
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
		}

		return facebookApi;
	}

	/// <summary>HTTP context</summary>
	private HttpContext Context { get; set; }
}