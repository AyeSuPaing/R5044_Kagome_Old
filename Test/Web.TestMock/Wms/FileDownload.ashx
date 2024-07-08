<%--
=========================================================================================================
  Module      : File Download (FileDownload.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="FileDownload" %>
using System.Web;

/// <summary>
/// File download
/// </summary>
public class FileDownload : IHttpHandler
{
	/// <summary>
	/// Process Request
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "application/json; charset=utf-8";

		// Execute
		var data = new WmsApi(context).ExecProcess("FileDownload");

		context.Response.Write(data);
	}

	/// <summary>Is reusable</summary>
	public bool IsReusable { get { return false; } }
}
