/*
=========================================================================================================
  Module      : 発送情報要素(PdRequestElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 発送情報要素
	/// </summary>
	/// <remarks>PdはScore社独自の名称でよくわからない謎の略語です</remarks>
	public class PdRequestElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PdRequestElement()
		{
			this.Pdcompanycode = string.Empty;
			this.Slipno = string.Empty;
			this.Address1 = string.Empty;
			this.Address2 = string.Empty;
			this.Address3 = string.Empty;
		}

		/// <summary>運送会社コード</summary>
		[XmlElement("pdcompanycode")]
		public string Pdcompanycode { get; set; }
		/// <summary>配送伝票番号</summary>
		[XmlElement("slipno")]
		public string Slipno { get; set; }
		/// <summary>都道府県</summary>
		[XmlElement("address1")]
		public string Address1 { get; set; }
		/// <summary>市区町村</summary>
		[XmlElement("address2")]
		public string Address2 { get; set; }
		/// <summary>その以降の住所</summary>
		[XmlElement("address3")]
		public string Address3 { get; set; }
	}
}
