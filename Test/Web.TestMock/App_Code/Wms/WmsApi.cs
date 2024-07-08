/*
=========================================================================================================
  Module      : Wms Api (WmsApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Web;
using w2.App.Common.Elogit;
using w2.Common.Util;

/// <summary>
/// Wms api
/// </summary>
public class WmsApi
{
	/// <summary>Constants key folder name Csv</summary>
	private const string CONST_KEY_FOLDER_NAME_CSV = "Csv";
	/// <summary>Constants key folder name Json</summary>
	private const string CONST_KEY_FOLDER_NAME_JSON = "Json";
	/// <summary>Constants key folder name Wms</summary>
	private const string CONST_KEY_FOLDER_NAME_WMS = "Wms";

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="context">Http context</param>
	public WmsApi(HttpContext context)
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
		var fileName = string.Format(
			"{0}{1}",
			elementName,
			GetParamsRequest().SyoriKbn);
		var response = GetJsonResponseFromFile(fileName);
		return response;
	}

	/// <summary>
	/// Get json response from file
	/// </summary>
	/// <param name="elementName">Element name</param>
	/// <returns>A data json</returns>
	private string GetJsonResponseFromFile(string elementName)
	{
		var downloadFilePath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			CONST_KEY_FOLDER_NAME_JSON,
			CONST_KEY_FOLDER_NAME_WMS,
			elementName + ".json");
		var data = File.ReadAllText(downloadFilePath);

		// If the file data is empty
		if (string.IsNullOrEmpty(data))
		{
			var filePath = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				CONST_KEY_FOLDER_NAME_JSON,
				CONST_KEY_FOLDER_NAME_WMS,
				"FileDownload1.json");
			var responseFileCreate = JsonConvert.DeserializeObject<ElogitResponse>(File.ReadAllText(filePath));

			// Reset download file path
			downloadFilePath = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				CONST_KEY_FOLDER_NAME_CSV,
				CONST_KEY_FOLDER_NAME_WMS,
				responseFileCreate.IfHistoryKey + ".csv");
		}

		// Encoder acquisition
		byte[] csvByteStream = null;
		using (var csvFileStream = new FileStream(downloadFilePath, FileMode.Open, FileAccess.Read))
		{
			csvByteStream = new byte[csvFileStream.Length];
			csvFileStream.Read(csvByteStream, 0, csvByteStream.Length);
		}

		var encoding = StringUtility.GetCode(csvByteStream);
		if (encoding == null) throw new Exception("ファイルの読取に失敗しました。[Encoding Error.]");

		// Read the response
		using (var streamReader = new StreamReader(downloadFilePath, encoding))
		{
			return streamReader.ReadToEnd();
		}
	}

	/// <summary>
	/// Get params request
	/// </summary>
	/// <returns>Base elogit request</returns>
	private BaseElogitRequest GetParamsRequest()
	{
		try
		{
			var requestForm = HttpUtility.ParseQueryString(this.Context.Request.Form.ToString());
			var json = JsonConvert.SerializeObject(requestForm.Cast<string>()
				.ToDictionary(item => item, item => requestForm[item]));
			var request = JsonConvert.DeserializeObject<BaseElogitRequest>(json);
			return request;
		}
		catch
		{
			return null;
		}
	}

	/// <summary>The context</summary>
	private HttpContext Context { set; get; }
}
