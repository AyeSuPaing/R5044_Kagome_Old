/*
=========================================================================================================
  Module      : 商品ランキングアイテム設定マスタモデル (ProductRankingItemModel.cs)
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
	/// 商品ランキングアイテム設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductRankingItemModel : ModelBase<ProductRankingItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductRankingItemModel()
		{
			this.FixationFlg = Constants.FLG_PRODUCTRANKINGITEM_FIXATION_FLG_OFF;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductRankingItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductRankingItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ランキングID</summary>
		public string RankingId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_RANKING_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_RANKING_ID] = value; }
		}
		/// <summary>ランキング</summary>
		public int Rank
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_RANK]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_RANK] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_PRODUCT_ID] = value; }
		}
		/// <summary>固定フラグ</summary>
		public string FixationFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_FIXATION_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_FIXATION_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTRANKINGITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
