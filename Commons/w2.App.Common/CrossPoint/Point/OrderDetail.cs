/*
=========================================================================================================
  Module      : Order Detail (OrderDetail.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.CrossPoint.Point
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
			this.Price = 0m;
			this.SalesPrice = 0m;
			this.Quantity = 0;
			this.Tax = 0m;
			this.ItemSalesKbn = Constants.CROSS_POINT_FLG_ITEM_KBN_PRODUCT;
		}

		/// <summary>JANコード</summary>
		public string JanCode { get; set; }
		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>商品コード</summary>
		public string ProductId { get; set; }
		/// <summary>単価(税抜)</summary>
		public decimal Price { get; set; }
		/// <summary>販売単価(税抜)</summary>
		public decimal SalesPrice { get; set; }
		/// <summary>販売数量</summary>
		public int Quantity { get; set; }
		/// <summary>税額</summary>
		public decimal Tax { get; set; }
		/// <summary>商品外売上区分</summary>
		public string ItemSalesKbn { get; set; }
	}
}
