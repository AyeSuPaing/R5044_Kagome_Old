/*
=========================================================================================================
  Module      : 印字データ明細項目情報要素(DetailListElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 明細項目要素
	/// </summary>
	public class DetailListElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DetailListElement()
		{
			this.GoodsDetail = new[] { new GoodsDetailElement() };
		}

		/// <summary>明細詳細情報</summary>
		[XmlElement("goodsDetail")]
		public GoodsDetailElement[] GoodsDetail;
	}

	/// <summary>
	/// 明細詳細情報要素
	/// </summary>
	public class GoodsDetailElement
	{
		/// <summary>明細名</summary>
		[XmlElement("goodsName")]
		public string GoodsName;
		/// <summary>単価</summary>
		[XmlElement("goodsPrice")]
		public string GoodsPrice;
		/// <summary>数量</summary>
		[XmlElement("goodsNum")]
		public string GoodsNum;
	}
}
