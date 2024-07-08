/*
=========================================================================================================
  Module      : 商品グループアイテムマスタモデル (ProductGroupItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductGroup
{
	/// <summary>
	/// 商品グループアイテムマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductGroupItemModel : ModelBase<ProductGroupItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductGroupItemModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductGroupItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductGroupItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>商品グループID</summary>
		public string ProductGroupId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_PRODUCT_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_PRODUCT_GROUP_ID] = value; }
		}
		/// <summary>商品グループアイテム枝番</summary>
		public int ItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_ITEM_NO] = value; }
		}
		/// <summary>アイテムタイプ</summary>
		public string ItemType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_ITEM_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_ITEM_TYPE] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_SHOP_ID] = value; }
		}
		/// <summary>マスタID</summary>
		public string MasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_MASTER_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUPITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
