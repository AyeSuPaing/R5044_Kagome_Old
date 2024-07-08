/*
=========================================================================================================
  Module      : Lpページ商品モデル (LandingPageProductModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// Lpページ商品モデル
	/// </summary>
	[Serializable]
	public partial class LandingPageProductModel : ModelBase<LandingPageProductModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public LandingPageProductModel()
		{
			this.PageId = "";
			this.ShopId = "";
			this.ProductId = "";
			this.VariationId = "";
			this.Quantity = 0;
			this.LastChanged = "";
			this.BranchNo = 0;
			this.VariationSortNumber = 0;
			this.BuyType = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageProductModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public LandingPageProductModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ページID</summary>
		public string PageId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_PAGE_ID] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_VARIATION_ID] = value; }
		}
		/// <summary>個数</summary>
		public int Quantity
		{
			get { return int.Parse(this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_QUANTITY].ToString()); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_QUANTITY] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_LAST_CHANGED] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return int.Parse(this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_BRANCH_NO].ToString()); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_BRANCH_NO] = value; }
		}
		/// <summary>バリエーション順序</summary>
		public int VariationSortNumber
		{
			get { return int.Parse(this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_VARIATION_SORT_NUMBER].ToString()); }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_VARIATION_SORT_NUMBER] = value; }
		}
		/// <summary>配送種別ID</summary>
		public string ShippingId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_SHIPPING_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_SHIPPING_ID] = value; }
		}
		/// <summary>購入タイプ</summary>
		public string BuyType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_BUY_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_BUY_TYPE] = value; }
		}
		#endregion
	}
}
