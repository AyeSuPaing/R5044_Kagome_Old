/*
=========================================================================================================
  Module      : Xmlクラス拡張モジュール(XmlExtensions.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace w2.Common.Extensions
{
	///**************************************************************************************
	/// <summary>
	/// Xml関連クラスに拡張メソッドを追加する
	/// </summary>
	///**************************************************************************************
	public static class XmlExtensions
	{
		/// <summary>
		/// XDocumentオブジェクトをXmlDocumentオブジェクトに変換
		/// </summary>
		/// <returns>変換後オブジェクト</returns>
		public static XmlDocument ToXmlDocument(this XDocument xdoc)
		{
			var xmlDocument = new XmlDocument();
			using (var xmlReader = xdoc.CreateReader())
			{
				xmlDocument.Load(xmlReader);
			}
			return xmlDocument;
		}

		/// <summary>
		/// XmlDocumentオブジェクトをXDocumentオブジェクトに変換
		/// 注意：ルートノード外のコメントが消えます。保存時には注意
		/// </summary>
		/// <returns>変換後オブジェクト</returns>
		public static XDocument ToXDocument(this XmlDocument xdoc)
		{
			using (var nodeReader = new XmlNodeReader(xdoc))
			{
				nodeReader.MoveToContent();
				return XDocument.Load(nodeReader);
			}
		}
	}
}
