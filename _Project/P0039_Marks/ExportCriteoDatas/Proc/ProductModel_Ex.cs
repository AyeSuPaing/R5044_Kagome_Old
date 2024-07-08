/*
=========================================================================================================
  Module      : 商品モデル拡張(ProductModel_Ex.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Util;

namespace w2.Commerce.Batch.ExportCriteoDatas.Proc
{
	/// <summary>
	/// 商品モデル
	/// </summary>
	public partial class ProductModel
	{
		#region プロパティ
		/// <summary>カテゴリID1名称</summary>
		public string CategoryName1
		{
			get { return StringUtility.ToEmpty(this.DataSource["category_name1"]); }
			set { this.DataSource["category_name1"] = value; }
		}
		/// <summary>カテゴリID2名称</summary>
		public string CategoryName2
		{
			get { return StringUtility.ToEmpty(this.DataSource["category_name2"]); }
			set { this.DataSource["category_name2"] = value; }
		}
		/// <summary>カテゴリID3名称</summary>
		public string CategoryName3
		{
			get { return StringUtility.ToEmpty(this.DataSource["category_name3"]); }
			set { this.DataSource["category_name3"] = value; }
		}
		/// <summary>バナー表示可否</summary>
		public string Recommendable
		{
			get { return StringUtility.ToEmpty(this.DataSource["recommendable"]); }
			set { this.DataSource["recommendable"] = value; }
		}
		/// <summary>在庫の有無</summary>
		public string Instock
		{
			get { return StringUtility.ToEmpty(this.DataSource["instock"]); }
			set { this.DataSource["instock"] = value; }
		}
		#endregion
	}
}
