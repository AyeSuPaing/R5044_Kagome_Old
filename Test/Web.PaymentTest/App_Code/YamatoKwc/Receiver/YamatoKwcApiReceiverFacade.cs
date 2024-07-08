/*
=========================================================================================================
  Module      : ヤマトKWC APIレシーバファサード (YamatoKwcApiReceiverFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

/// <summary>
/// ヤマトKWC APIレシーバファサード
/// </summary>
public class YamatoKwcApiReceiverFacade
{
	/// <summary>
	/// 受信
	/// </summary>
	/// <param name="req"></param>
	/// <returns></returns>
	public string Receive(HttpRequest req)
	{
		var xmlString = CreateResponseXmlString(req);
		return xmlString;
	}

	/// <summary>
	/// レスポンスXML文字列作成
	/// </summary>
	/// <param name="req"></param>
	/// <returns></returns>
	private string CreateResponseXmlString(HttpRequest req)
	{
		var success = string.IsNullOrEmpty(req["reserve_1"]);
		var functionDiv = (PaymentYamatoKwcFunctionDiv)Enum.Parse(typeof(PaymentYamatoKwcFunctionDiv), req["function_div"]);

		var xmlString = CreateResponseXml(functionDiv, success);
		return xmlString;
	}

	/// <summary>
	/// レスポンス作成(成功）
	/// </summary>
	/// <param name="functionDiv">機能区分</param>
	/// <param name="result">結果</param>
	/// <returns>レスポンスXML</returns>
	private string CreateResponseXml(PaymentYamatoKwcFunctionDiv functionDiv, bool result)
	{
		var date = DateTime.Now.ToString("yyyyMMdd");
		var datetime = DateTime.Now.ToString("yyyyMMddHHmmss");

		var xdocPath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Xml",
			"YamatoKwc",
			string.Format("YamatoKwcApiRes{0}.xml", functionDiv));

		var xdoc = XDocument.Load(xdocPath);
		var targetElem = xdoc.Root.Elements().First(e => e.Name == (result ? "success" : "failure")).Elements().First();
		
		var resultXmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + "\r\n"
			+ targetElem.ToString()
			.Replace("@@datetime@@", datetime)
			.Replace("@@date@@", date);
		return resultXmlString;
	}
}