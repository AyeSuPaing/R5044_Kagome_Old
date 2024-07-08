/*
=========================================================================================================
  Module      : Facebook Catalog Api (FacebookCatalogApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Facebook Catalog Api Class
/// </summary>
[XmlRoot("root")]
public class FacebookCatalogApi
{
	/// <summary>
	/// Create model
	/// </summary>
	/// <param name="filePath">File path</param>
	/// <returns>Facebook api model</returns>
	public FacebookCatalogApi CreateModel(string filePath)
	{
		var facebookCatalogApi = new FacebookCatalogApi();
		using (FileStream file = new FileStream(filePath, FileMode.Open))
		{
			var serializer = new XmlSerializer(typeof(FacebookCatalogApi));
			facebookCatalogApi = (FacebookCatalogApi)serializer.Deserialize(file);
		}

		return facebookCatalogApi;
	}

	/// <summary>Result status</summary>
	[XmlElement("resultstatus")]
	public string ResultStatus { get; set; }
	/// <summary>Handles</summary>
	[XmlArray("handles")]
	[XmlArrayItem("handle")]
	public List<string> Handles { get; set; }
	/// <summary>Error</summary>
	[XmlElement("error")]
	public FacebookCatalogApiError Error { get; set; }

	/// <summary>
	/// Facebook Catalog Api Error
	/// </summary>
	[Serializable]
	public class FacebookCatalogApiError
	{
		/// <summary>Message</summary>
		[XmlElement("message")]
		public string Message { get; set; }
		/// <summary>Type</summary>
		[XmlElement("type")]
		public string Type { get; set; }
		/// <summary>Code</summary>
		[XmlElement("code")]
		public string Code { get; set; }
		/// <summary>Fbtrace id</summary>
		[XmlElement("fbtrace_id")]
		public string FbtraceId { get; set; }
	}
}