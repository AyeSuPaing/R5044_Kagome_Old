/*
=========================================================================================================
  Module      : Product list for report (ProductListForReport.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Domain.Product.Helper
{
	/// <summary>
	/// Product list for report
	/// </summary>
	[Serializable]
	public class ProductListForReport : ProductModel
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductListForReport()
		{
			this.TotalOrderAmount = 0m;
			this.TotalProductCount = 0;
			this.TotalProducVariationtCount = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductListForReport(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductListForReport(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>Total order amount</summary>
		public decimal TotalOrderAmount
		{
			get { return (decimal)StringUtility.ToValue(this.DataSource["total_order_amount"], 0m); }
			set { this.DataSource["total_order_amount"] = value; }
		}
		/// <summary>Total product count</summary>
		public int TotalProductCount
		{
			get { return (int)StringUtility.ToValue(this.DataSource["total_product_count"], 0); }
			set { this.DataSource["total_product_count"] = value; }
		}
		/// <summary>Total product variation count</summary>
		public int TotalProducVariationtCount
		{
			get { return (int)StringUtility.ToValue(this.DataSource["total_product_variation_count"], 0); }
			set { this.DataSource["total_product_variation_count"] = value; }
		}
		#endregion
	}
}
