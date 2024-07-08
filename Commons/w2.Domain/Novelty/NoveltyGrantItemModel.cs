/*
=========================================================================================================
  Module      : ノベルティ付与アイテムモデル (NoveltyGrantItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Novelty
{
	/// <summary>
	/// ノベルティ付与アイテムモデル
	/// </summary>
	[Serializable]
	public partial class NoveltyGrantItemModel : ModelBase<NoveltyGrantItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NoveltyGrantItemModel()
		{
			this.ConditionNo = 1;
			this.SortNo = 1;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NoveltyGrantItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NoveltyGrantItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_SHOP_ID] = value; }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_NOVELTY_ID] = value; }
		}
		/// <summary>ノベルティ付与条件枝番</summary>
		public int ConditionNo
		{
			get { return (int)this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_CONDITION_NO]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_CONDITION_NO] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_PRODUCT_ID] = value; }
		}
		/// <summary>並び順</summary>
		public int SortNo
		{
			get { return (int)this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_SORT_NO]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_SORT_NO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
