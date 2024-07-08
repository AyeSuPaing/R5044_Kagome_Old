/*
=========================================================================================================
  Module      : GmoCvsApi(GmoCvsApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using w2.Common.Logger;

/// <summary>
/// Summary description for GMoCvsApi
/// </summary>
public class GmoCvsApi : System.Web.UI.Page
{
	#region +Constants
	protected const string FILE_EXTENSION = ".xml";
	#endregion

	/// <summary>
	/// GMo Cvs Api Constructor
	/// </summary>
	public GmoCvsApi()
	{
	}

	/// <summary>
	/// Get Response String From Xml
	/// </summary>
	/// <param name="xmlFilePath"></param>
	/// <returns></returns>
	protected string GetResponseFromXml(string xmlFilePath)
	{
		var listParam = new List<String>();

		try
		{
			var xdoc = XDocument.Load(xmlFilePath);
			xdoc.DescendantNodes().OfType<XComment>().Remove();

			var responseNode = (XElement)xdoc.Root.FirstNode;

			foreach (XElement elementNode in responseNode.Elements())
			{
				listParam.Add(string.Format("{0}={1}", elementNode.Name, elementNode.Value.Replace("@@datetime@@", DateTime.Now.ToString("yyyyMMddHHmmss"))));
			}
		}
		catch (Exception exception)
		{
			FileLogger.WriteError(exception);

			listParam.Add(string.Format("{0}={1}", "ErrCode", "E01"));
			listParam.Add(string.Format("{0}={1}", "ErrInfo", "Exception"));
		}

		return String.Join("&", listParam);
	}

	/// <summary>
	/// Exec request
	/// </summary>
	/// <param name="requestPage">Request page</param>
	protected void ExecRequest(string requestPage)
	{
		var requestFile = string.Format("{0}{1}", requestPage, FILE_EXTENSION);
		var reader = new StreamReader(Request.InputStream);
		var requestString = HttpUtility.UrlDecode(reader.ReadToEnd(), Encoding.UTF8);
		FileLogger.WriteInfo(String.Format("REQUEST INFO: {0}", requestString));

		var responseFile = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				"Xml",
				"GmoCvs",
				requestFile);

		var responseString = GetResponseFromXml(responseFile);

		Response.ContentType = "text/plain";
		Response.Write(responseString);
	}
}