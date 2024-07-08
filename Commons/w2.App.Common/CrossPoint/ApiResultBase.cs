/*
=========================================================================================================
  Module      : CrossPoint API 結果取得 基底モデル (ApiResultBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using w2.Common.Util;

namespace w2.App.Common.CrossPoint
{
	/// <summary>
	/// 結果取得 基底モデル
	/// </summary>
	public abstract class ApiResultBase
	{
		/// <summary>
		/// Xmlエレメントの値を取得
		/// </summary>
		/// <param name="elements">Xml要素</param>
		/// <param name="name">要素名</param>
		/// <returns>Xmlエレメント</returns>
		protected string GetElementValue(XmlElement[] elements, string name)
		{
			var result = elements
				.Where(element => (element.Name == name))
				.Select(element => (element.FirstChild != null) ? element.FirstChild.Value : string.Empty)
				.FirstOrDefault();
			return StringUtility.ToEmpty(result);
		}

		/// <summary>連番</summary>
		[XmlAttribute("No")]
		public int No { get; set; }
		/// <summary>その他</summary>
		[XmlAnyElement]
		public XmlElement[] OtherElements { get; set; }
	}
}
