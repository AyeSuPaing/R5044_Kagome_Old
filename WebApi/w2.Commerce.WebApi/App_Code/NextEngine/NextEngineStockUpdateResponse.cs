/*
=========================================================================================================
  Module      : ネクストエンジン在庫連携API レスポンス (NextEngineStockUpdateResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;

/// <summary>
/// ネクストエンジン在庫連携API レスポンス
/// </summary>
public class NextEngineStockUpdateResponse
{
	/// <summary>処理結果</summary>
	public enum Processed
	{
		/// <summary>成功</summary>
		[EnumTextName("0")]
		Success,
		/// <summary>クライエントエラー</summary>
		[EnumTextName("-2")]
		ClientError,
		/// <summary>システムエラー</summary>
		[EnumTextName("-3")]
		SystemError,
	}

	/// <summary>レスポンス文字コード</summary>
	public static Encoding ENCODING = Encoding.GetEncoding("EUC-JP");

	/// <summary>
	/// レスポンス結果の取得
	/// </summary>
	/// <param name="request">リクエスト</param>
	/// <param name="processed">処理結果</param>
	/// <returns></returns>
	public string GetResponseText(NextEngineStockUpdateRequest request, Processed processed)
	{
		var result = "";
		var shoppingUpdateStock = new ShoppingUpdateStock()
		{
			Version = "1.0",
			ResultSet = new ResultSet()
			{
				TotalResult = "1",
				Request = new Request()
				{
					Argument = new[]
					{
						new Argument()
						{
							Name = NextEngineStockUpdateRequest.NextEngineStockUpdateRequestQueryParam.StoreAccount
								.ToText(),
							Value = request.StoreAccount
						},
						new Argument()
						{
							Name = NextEngineStockUpdateRequest.NextEngineStockUpdateRequestQueryParam.Code.ToText(),
							Value = request.Code
						},
						new Argument()
						{
							Name = NextEngineStockUpdateRequest.NextEngineStockUpdateRequestQueryParam.Stock.ToText(),
							Value = request.Stock
						},
						new Argument()
						{
							Name = NextEngineStockUpdateRequest.NextEngineStockUpdateRequestQueryParam.Ts.ToText(),
							Value = request.Ts
						},
						new Argument()
						{
							Name = NextEngineStockUpdateRequest.NextEngineStockUpdateRequestQueryParam.Sig.ToText(),
							Value = request.Sig
						},
					}
				},
				Result = new Result()
				{
					No = "1",
					Processed = processed.ToText()
				}
			}
		};
		using (var writer = new EucJpStringWriter())
		{
			var serializer = new XmlSerializer(typeof(ShoppingUpdateStock));
			var ns = new XmlSerializerNamespaces();
			ns.Add(string.Empty, string.Empty);
			serializer.Serialize(writer, shoppingUpdateStock, ns);
			result = writer.ToString();
		}
		return result;
	}

	/// <summary>
	/// EUC-JP型 TextWriterクラス
	/// </summary>
	private class EucJpStringWriter : StringWriter
	{
		/// <summary>
		/// 文字コード
		/// </summary>
		public override Encoding Encoding
		{
			get { return ENCODING; }
		}
	}

	/// <summary>
	/// レスポンスXML ShoppingUpdateStock
	/// </summary>
	[XmlRoot("ShoppingUpdateStock")]
	public class ShoppingUpdateStock
	{
		/// <summary>version</summary>
		[XmlAttribute("version")]
		public string Version { get; set; }
		// <summary>ResultSet</summary>
		[XmlElement("ResultSet")]
		public ResultSet ResultSet { get; set; }
	}

	/// <summary>
	/// レスポンスXML ResultSet
	/// </summary>
	[Serializable]
	public class ResultSet
	{
		// <summary>TotalResult</summary>
		[XmlAttribute("TotalResult")]
		public string TotalResult { get; set; }
		// <summary>Request</summary>
		[XmlElement("Request")]
		public Request Request { get; set; }
		// <summary>Result</summary>
		[XmlElement("Result")]
		public Result Result { get; set; }
	}

	/// <summary>
	/// レスポンスXML Request
	/// </summary>
	[Serializable]
	public class Request
	{
		// <summary>Argument</summary>
		[XmlElementAttribute("Argument")]
		public Argument[] Argument { get; set; }
	}

	/// <summary>
	/// レスポンスXML Argument
	/// </summary>
	[Serializable]
	public class Argument
	{
		// <summary>Name</summary>
		[XmlAttribute("Name")]
		public string Name { get; set; }
		// <summary>Value</summary>
		[XmlAttribute("Value")]
		public string Value { get; set; }
	}

	/// <summary>
	/// レスポンスXML Result
	/// </summary>
	[Serializable]
	public class Result
	{
		// <summary>No</summary>
		[XmlAttribute("No")]
		public string No { get; set; }
		// <summary>Processed</summary>
		[XmlElement("Processed")]
		public string Processed { get; set; }
	}
}