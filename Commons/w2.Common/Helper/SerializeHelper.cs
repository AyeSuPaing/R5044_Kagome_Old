/*
=========================================================================================================
  Module      : シリアライズのヘルパ(SerializeHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace w2.Common.Helper
{
	/// <summary>
	/// シリアライズのヘルパ
	/// </summary>
	public static class SerializeHelper
	{
		#region +Deserialize デシリアライズ(XML ⇒ オブジェクト)
		/// <summary>
		/// デシリアライズ(XML ⇒ オブジェクト)
		/// </summary>
		/// <typeparam name="T">変換するオブジェクトの型</typeparam>
		/// <param name="xml">対象とするXMLの文字列</param>
		/// <returns>変換したオブジェクト</returns>
		public static T Deserialize<T>(string xml)
		{
			using (var reader = XmlReader.Create(new StringReader(xml), null))
			{
				var serializer = new XmlSerializer(typeof(T));
				return (T)serializer.Deserialize(reader);
			}
		}
		#endregion

		#region +Serialize シリアライズ （オブジェクト ⇒ XML）
		/// <summary>
		/// シリアライズ （オブジェクト ⇒ XML）
		/// </summary>
		/// <param name="target">変換するオブジェクト</param>
		/// <returns>変換したXML</returns>
		public static string Serialize(object target)
		{
			using (var stream = new MemoryStream())
			using (var writer = new StreamWriter(stream, Encoding.UTF8))
			using (var reader = new StreamReader(stream))
			{
				var ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				new XmlSerializer(target.GetType()).Serialize(writer, target, ns);
				stream.Seek(0, SeekOrigin.Begin);
				return reader.ReadToEnd();
			}
		}
		#endregion

		#region +DeserializeFromXmlFile XMLファイルからデシリアライズ
		/// <summary>
		/// XMLファイルからデシリアライズ
		/// </summary>
		/// <typeparam name="T">デシリアライズする型</typeparam>
		/// <param name="filePath">ファイルパス</param>
		/// <returns>オブジェクト</returns>
		public static T DeserializeFromXmlFile<T>(string filePath)
		{
			using (var fileStream = new FileStream(filePath, FileMode.Open))
			{
				var obj = (T)(new XmlSerializer(typeof(T))).Deserialize(fileStream);
				return obj;
			}
		}
		#endregion

		#region SerializeToXmlFile シリアライズしてXMLファイルに保存
		/// <summary>
		/// シリアライズしてXMLファイルに保存
		/// </summary>
		/// <typeparam name="T">シリアライズする型</typeparam>
		/// <param name="obj">オブジェクト</param>
		/// <param name="filePath">ファイルパス</param>
		public static void SerializeToXmlFile<T>(T obj, string filePath)
		{
			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				(new XmlSerializer(typeof(T))).Serialize(fileStream, obj);
			}
		}
		#endregion

		#region +DeserializeJson デシリアライズ(JSON ⇒ オブジェクト)
		/// <summary>
		/// デシリアライズ(JSON ⇒ オブジェクト)
		/// </summary>
		/// <typeparam name="T">変換するオブジェクトの型</typeparam>
		/// <param name="json">対象とするJSONの文字列</param>
		/// <returns>変換したオブジェクト</returns>
		public static T DeserializeJson<T>(string json)
		{
			var serializer = new JavaScriptSerializer();
			return serializer.Deserialize<T>(json);
		}

		/// <summary>
		/// デシリアライズ(JSON ⇒ オブジェクト)
		/// JSONに変換出来なかった場合はNULLを返す
		/// </summary>
		/// <typeparam name="T">ジェネリッククラス</typeparam>
		/// <param name="json">変換対象文字列</param>
		/// <returns>JSONオブジェクト</returns>
		public static T GetDeserializeJson<T>(string json) where T : class
		{
			var jsonObj = new object();
			try
			{
				jsonObj = DeserializeJson<T>(json);
			}
			catch (Exception e)
			{
				return null;
			}

			return (T)jsonObj;
		}
		#endregion

		#region +SerializeJson シリアライズ （オブジェクト ⇒ JSON）
		/// <summary>
		/// シリアライズ （オブジェクト ⇒ JSON）
		/// </summary>
		/// <param name="target">変換するオブジェクト</param>
		/// <returns>変換したJSON</returns>
		public static string SerializeJson(object target)
		{
			return new JavaScriptSerializer().Serialize(target);
		}
		#endregion
	}
}