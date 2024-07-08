/*
=========================================================================================================
  Module      : Order Detail (OrderDetail.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.OrderHistory
{
	/// <summary>
	/// 購買明細モデル
	/// </summary>
	public class OrderDetail
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderDetail()
		{
			this.JanCode = string.Empty;
			this.ProductName = string.Empty;
			this.ProductId = string.Empty;
			this.SalesPriceInTax = string.Empty;
			this.SalesPriceNoTax = string.Empty;
			this.Quantity = string.Empty;
			this.ColorCode = string.Empty;
			this.SizeCode = string.Empty;
		}

		/// <summary>JANコード</summary>
		[XmlElement("JanCd")]
		public string JanCode { get; set; }
		/// <summary>商品名</summary>
		[XmlElement("ItemName")]
		public string ProductName { get; set; }
		/// <summary>商品コード</summary>
		[XmlElement("ItemCd")]
		public string ProductId { get; set; }
		/// <summary>販売単価(税込)</summary>
		[XmlElement("UnitPriceInTax")]
		public string SalesPriceInTax { get; set; }
		/// <summary>販売単価(税抜)</summary>
		[XmlElement("UnitPriceNoTax")]
		public string SalesPriceNoTax { get; set; }
		/// <summary>販売数量</summary>
		[XmlElement("SalesNum")]
		public string Quantity { get; set; }
		/// <summary>カラーコード</summary>
		[XmlElement("ColorCd")]
		public string ColorCode { get; set; }
		/// <summary>サイズコード</summary>
		[XmlElement("SizeCd")]
		public string SizeCode { get; set; }
	}
}
