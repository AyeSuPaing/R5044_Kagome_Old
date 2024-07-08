using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

/// <summary>
/// YamatoKaApiReceiverFacade の概要の説明です
/// </summary>
public class YamatoKaApiReceiverFacade
{
	public YamatoKaApiReceiverFacade()
	{
		//
		// TODO: コンストラクター ロジックをここに追加します
		//
	}

	/// <summary>
	/// 受信
	/// </summary>
	/// <param name="req">リクエスト</param>
	/// <param name="apiType">Apiタイプ</param>
	/// <returns>返信文字列</returns>
	public string Receive(HttpRequest req, string apiType)
	{
		var xmlString = CreateResponseXmlString(apiType);
		return xmlString;
	}

	/// <summary>
	/// レスポンスXML文字列作成
	/// </summary>
	/// <param name="apiType">Apiタイプ</param>
	/// <returns>レスポンスXML文字列</returns>
	private string CreateResponseXmlString(string apiType)
	{
		var xmlString = CreateResponseXml(apiType, true);
		return xmlString;
	}

	/// <summary>
	/// レスポンス作成(成功）
	/// </summary>
	/// <param name="apiType">Apiタイプ</param>
	/// <param name="result">結果</param>
	/// <returns>レスポンスXML</returns>
	private string CreateResponseXml(string apiType, bool result)
	{
		var date = DateTime.Now.ToString("yyyyMMdd");
		var datetime = DateTime.Now.ToString("yyyyMMddHHmmss");

		var xdocPath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Xml",
			"YamatoKa",
			string.Format("YamatoKaApiRes{0}.xml", apiType));

		var xdoc = XDocument.Load(xdocPath);
		var targetElem = xdoc.Root.Elements().First(e => e.Name == (result ? "success" : "failure")).Elements().First();

		var resultXmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + "\r\n"
			+ targetElem.ToString()
			.Replace("@@datetime@@", datetime)
			.Replace("@@date@@", date);
		return resultXmlString;
	}
}