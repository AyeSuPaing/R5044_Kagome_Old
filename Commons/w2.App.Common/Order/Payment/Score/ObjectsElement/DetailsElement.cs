/*
=========================================================================================================
  Module      : 明細項目要素(DetailsElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.App.Common.Extensions.Currency;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 明細項目要素
	/// </summary>
	public class DetailsElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DetailsElement()
		{
			this.Detail = new[] { new DetailElement() };
		}

		/// <summary>明細詳細情報</summary>
		[XmlElement("detail")]
		public DetailElement[] Detail { get; set; }
	}

	#region DetailElement 明細詳細情報要素
	/// <summary>
	/// 明細詳細情報要素
	/// </summary>
	public class DetailElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DetailElement()
		{
			this.DetailId = string.Empty;
			this.DetailName = string.Empty;
			this.DetailPrice = "0";
			this.DetailQuantity = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="detailName">詳細名</param>
		/// <param name="detailPrice">単価</param>
		/// <param name="detailQuantity">注文数</param>
		public DetailElement(string detailName, decimal detailPrice, int detailQuantity = 1)
			: this()
		{
			this.DetailName = detailName;
			this.DetailPrice = detailPrice.ToPriceString();
			this.DetailQuantity = detailQuantity;
		}

		/// <summary>明細ID</summary>
		[XmlElement("detailId")]
		public string DetailId { get; set; }
		/// <summary>明細名</summary>
		[XmlElement("detailName")]
		public string DetailName { get; set; }
		/// <summary>単価</summary>
		[XmlElement("detailPrice")]
		public string DetailPrice { get; set; }
		/// <summary>数量</summary>
		[XmlElement("detailQuantity")]
		public decimal DetailQuantity { get; set; }
	}
	#endregion
}
