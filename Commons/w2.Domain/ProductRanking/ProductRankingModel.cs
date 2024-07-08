/*
=========================================================================================================
  Module      : 商品ランキング設定マスタモデル (ProductRankingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductRanking
{
	/// <summary>
	/// 商品ランキング設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductRankingModel : ModelBase<ProductRankingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductRankingModel()
		{
			this.TotalType = Constants.FLG_PRODUCTRANKING_TOTAL_TYPE_AUTO;
			this.TotalKbn = Constants.FLG_PRODUCTRANKING_TOTAL_KBN_PERIOD;
			this.StockKbn = Constants.FLG_PRODUCTRANKING_STOCK_KBN_INVALID;
			this.ValidFlg = Constants.FLG_PRODUCTRANKING_VALID_FLG_VALID;
			this.BrandKbn = Constants.FLG_PRODUCTRANKING_BRAND_KBN_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductRankingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductRankingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_SHOP_ID] = value; }
		}
		/// <summary>商品ランキングID</summary>
		public string RankingId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_RANKING_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_RANKING_ID] = value; }
		}
		/// <summary>集計タイプ</summary>
		public string TotalType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_TYPE] = value; }
		}
		/// <summary>集計期間区分</summary>
		public string TotalKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_KBN] = value; }
		}
		/// <summary>集計期間(FROM)</summary>
		public DateTime TotalFrom
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_FROM]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_FROM] = value; }
		}
		/// <summary>集計期間(TO)</summary>
		public DateTime TotalTo
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_TO]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_TO] = value; }
		}
		/// <summary>集計日数</summary>
		public string TotalDays
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_DAYS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_TOTAL_DAYS] = value; }
		}
		/// <summary>カテゴリID</summary>
		public string CategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_CATEGORY_ID] = value; }
		}
		/// <summary>説明</summary>
		public string Desc1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_DESC1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_DESC1] = value; }
		}
		/// <summary>在庫切れ商品</summary>
		public string StockKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_STOCK_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_STOCK_KBN] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTRANKING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTRANKING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_LAST_CHANGED] = value; }
		}
		/// <summary>ブランド指定</summary>
		public string BrandKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_BRAND_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_BRAND_KBN] = value; }
		}
		/// <summary>ブランドID</summary>
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_BRAND_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_BRAND_ID] = value; }
		}
		/// <summary>カテゴリ除外ID</summary>
		public string ExcludeCategoryIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKING_EXCLUDE_CATEGORY_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKING_EXCLUDE_CATEGORY_IDS] = value; }
		}
		#endregion
	}
}
