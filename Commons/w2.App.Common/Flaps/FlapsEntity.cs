/*
=========================================================================================================
  Module      : FLAPSリクエストXMLエンティティクラス (FlapsEntity.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace w2.App.Common.Flaps
{
	/// <summary>
	/// FLAPSリクエストXMLエンティティクラス
	/// </summary>
	public class FlapsEntity
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FlapsEntity()
		{
			this.Is = "54666559";
			this.RequestXml = "";
		}

		/// <summary>
		/// 取得API実行
		/// </summary>
		/// <typeparam name="T">戻り値型</typeparam>
		/// <returns>結果オブジェクト</returns>
		internal T Get<T>() where T : ResultBase
		{
			Serialize();

			var result = this.Request.Invoke(XDocument.Parse(this.RequestXml));
			return (T)Convert.ChangeType(result, typeof(T));
		}

		/// <summary>
		/// シリアライズ (オブジェクトからXML値文字列へ変換)
		/// </summary>
		private void Serialize()
		{
			using (var stringwriter = new StringWriter())
			{
				var xns = new XmlSerializerNamespaces();
				xns.Add("", "");
				var serializer = new XmlSerializer(GetType());
				serializer.Serialize(stringwriter, this, xns);
				this.RequestXml = stringwriter.ToString();
			}
		}

		/// <summary>リクエスト関数</summary>
		[XmlIgnore]
		protected Func<XDocument, ResultBase> Request { get; set; }
		/// <summary>リクエストXML</summary>
		[XmlIgnore]
		private string RequestXml { get; set; }
		/// <summary>コマンドセット名</summary>
		[XmlElement(ElementName = "IS")]
		public string Is { get; set; }
		/// <summary>APIリクエスト操作名</summary>
		[XmlElement(ElementName = "METHOD")]
		public string Method { get; set; }
	}
}
