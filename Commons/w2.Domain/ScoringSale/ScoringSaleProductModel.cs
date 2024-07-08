/*
=========================================================================================================
  Module      : ScoringSaleProductモデル (ScoringSaleProductModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// ScoringSaleProductモデル
	/// </summary>
	[Serializable]
	public partial class ScoringSaleProductModel : ModelBase<ScoringSaleProductModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ScoringSaleProductModel()
		{
			this.Quantity = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleProductModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleProductModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>スコアリング販売ID</summary>
		public string ScoringSaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_SCORING_SALE_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_SCORING_SALE_ID] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_BRANCH_NO] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_VARIATION_ID] = value; }
		}
		/// <summary>個数</summary>
		public int Quantity
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEPRODUCT_QUANTITY] = value; }
		}
		#endregion
	}
}
