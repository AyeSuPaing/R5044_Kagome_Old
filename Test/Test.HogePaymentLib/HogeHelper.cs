using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Test.HogePaymentLib
{
	public static class HogeHelper
	{
		/// <summary>
		/// デシリアライズ(XML ⇒ オブジェクト)
		/// </summary>
		/// <typeparam name="T">変換するオブジェクトの型</typeparam>
		/// <param name="xml">対象とするXMLの文字列</param>
		/// <returns>変換したオブジェクト</returns>
		public static T Deserialize<T>(string xml)
		{
			XmlReader reader = XmlReader.Create(new StringReader(xml), null);
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(reader);
		}

		/// <summary>
		/// シリアライズ （オブジェクト ⇒ XML）
		/// </summary>
		/// <param name="target">変換するオブジェクト</param>
		/// <returns>変換したXML</returns>
		public static string Serialize(object target)
		{
			using (MemoryStream stream = new MemoryStream())
			using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
			using (StreamReader reader = new StreamReader(stream))
			{
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				new XmlSerializer(target.GetType()).Serialize(writer, target, ns);
				stream.Seek(0, SeekOrigin.Begin);
				return reader.ReadToEnd();
			}
		}
	}
}
