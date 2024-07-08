<%--
=========================================================================================================
  Module      : File Upload (FileUpload.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="FileUpload" %>
using System.Web;

/// <summary>
/// File upload
/// </summary>
public class FileUpload : IHttpHandler
{
	/// <summary>
	/// Process Request
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "application/json; charset=utf-8";

		// Execute
		var data = new WmsApi(context).ExecProcess("FileUpload");

		context.Response.Write(data);
	}

	/// <summary>Is reusable</summary>
	public bool IsReusable { get { return false; } }
}
