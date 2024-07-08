/*
=========================================================================================================
  Module      : LP商品入力モデル(NewsController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using w2.App.Common;
using w2.App.Common.Input;
using w2.Domain.LandingPage;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// LP商品入力モデル
	/// </summary>
	[Serializable]
	public class LandingPageProductInput : InputBase<LandingPageProductModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LandingPageProductInput()
		{
			this.BranchNo = "0";
			this.VariationSortNumber = "0";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public LandingPageProductInput(LandingPageProductModel model)
		{
			this.DataSource = new Hashtable();
			this.ShopId = model.ShopId;
			this.PageId = model.PageId;
			this.ProductId = model.ProductId;
			this.VariationId = model.VariationId;
			this.Quantity = model.Quantity.ToString();
			this.BranchNo = model.BranchNo.ToString();
			this.VariationSortNumber = model.VariationSortNumber.ToString();
			this.BuyType = model.BuyType;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override LandingPageProductModel CreateModel()
		{
			var model = new LandingPageProductModel();
			model.PageId = this.PageId;
			model.ShopId = this.ShopId;
			model.ProductId = this.ProductId;
			model.VariationId = this.VariationId;
			model.Quantity = int.Parse(this.Quantity);
			model.LastChanged = "";
			model.DateChanged = DateTime.Now;
			model.DateCreated = DateTime.Now;
			model.BranchNo = int.Parse(this.BranchNo);
			model.VariationSortNumber = int.Parse(this.VariationSortNumber);
			model.BuyType = (string.IsNullOrEmpty(this.BuyType) == false)
				? this.BuyType
				: LandingPageConst.BUY_TYPE_NORMAL;
			return model;
		}
		#endregion

		/// <summary>ページID</summary>
		public string PageId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_PAGE_ID] = value; }
		}
		/// <summary>枝番</summary>
		public string BranchNo
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_BRANCH_NO] = value; }
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
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_VARIATION_ID] = value; }
		}
		/// <summary>数量</summary>
		public string Quantity
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_QUANTITY] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationSortNumber
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_VARIATION_SORT_NUMBER]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_VARIATION_SORT_NUMBER] = value; }
		}
		/// <summary>購入タイプ</summary>
		public string BuyType
		{
			get { return (string)this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_BUY_TYPE]; }
			set { this.DataSource[Constants.FIELD_LANDINGPAGEPRODUCT_BUY_TYPE] = value; }
		}
	}
}