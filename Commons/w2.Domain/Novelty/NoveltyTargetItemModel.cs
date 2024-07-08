/*
=========================================================================================================
  Module      : ノベルティ対象アイテムモデル (NoveltyTargetItemModel.cs)
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
	/// ノベルティ対象アイテムモデル
	/// </summary>
	[Serializable]
	public partial class NoveltyTargetItemModel : ModelBase<NoveltyTargetItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NoveltyTargetItemModel()
		{
			this.NoveltyTargetItemNo = 1;
			this.NoveltyTargetItemType = Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_ALL;
			this.NoveltyTargetItemTypeSortNo = 1;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NoveltyTargetItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NoveltyTargetItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_SHOP_ID] = value; }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_ID] = value; }
		}
		/// <summary>ノベルティ対象アイテム枝番</summary>
		public int NoveltyTargetItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_NO] = value; }
		}
		/// <summary>ノベルティ対象アイテム種別</summary>
		public string NoveltyTargetItemType
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE] = value; }
		}
		/// <summary>ノベルティ対象アイテム値</summary>
		public string NoveltyTargetItemValue
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_VALUE]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_VALUE] = value; }
		}
		/// <summary>ノベルティ対象アイテム並び順</summary>
		public int NoveltyTargetItemTypeSortNo
		{
			get { return (int)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE_SORT_NO]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE_SORT_NO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
