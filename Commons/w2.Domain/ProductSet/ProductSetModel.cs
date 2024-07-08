/*
=========================================================================================================
  Module      : 商品セットマスタモデル (ProductSetModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductSet
{
	/// <summary>
	/// 商品セットマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductSetModel : ModelBase<ProductSetModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductSetModel()
		{
			this.ShopId = "";
			this.ProductSetId = "";
			this.ProductSetName = "";
			this.ParentMin = null;
			this.ParentMax = null;
			this.ChildMin = null;
			this.ChildMax = null;
			this.Priority = 1;
			this.DescriptionKbn = "0";
			this.Description = "";
			this.DescriptionKbnMobile = "0";
			this.DescriptionMobile = "";
			this.MaxSellQuantity = 1;
			this.ShippingSizeKbn = "";
			this.EditableFlg = "0";
			this.ValidFlg = "1";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductSetModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductSetModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_SHOP_ID] = value; }
		}
		/// <summary>商品セットID</summary>
		public string ProductSetId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID] = value; }
		}
		/// <summary>商品セット名</summary>
		public string ProductSetName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME] = value; }
		}
		/// <summary>親商品数(下限)</summary>
		public int? ParentMin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSET_PARENT_MIN] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSET_PARENT_MIN];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSET_PARENT_MIN] = value; }
		}
		/// <summary>親商品数(上限)</summary>
		public int? ParentMax
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSET_PARENT_MAX] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSET_PARENT_MAX];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSET_PARENT_MAX] = value; }
		}
		/// <summary>子商品数(下限)</summary>
		public int? ChildMin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSET_CHILD_MIN] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSET_CHILD_MIN];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSET_CHILD_MIN] = value; }
		}
		/// <summary>子商品数(上限)</summary>
		public int? ChildMax
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSET_CHILD_MAX] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSET_CHILD_MAX];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSET_CHILD_MAX] = value; }
		}
		/// <summary>適用優先順位</summary>
		public int Priority
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSET_PRIORITY]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_PRIORITY] = value; }
		}
		/// <summary>表示用文言HTML区分</summary>
		public string DescriptionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN] = value; }
		}
		/// <summary>表示用文言</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_DESCRIPTION] = value; }
		}
		/// <summary>モバイル表示用文言HTML区分</summary>
		public string DescriptionKbnMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN_MOBILE] = value; }
		}
		/// <summary>モバイル表示用文言</summary>
		public string DescriptionMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_DESCRIPTION_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_DESCRIPTION_MOBILE] = value; }
		}
		/// <summary>販売可能数</summary>
		public int MaxSellQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSET_MAX_SELL_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_MAX_SELL_QUANTITY] = value; }
		}
		/// <summary>配送サイズ区分</summary>
		public string ShippingSizeKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_SHIPPING_SIZE_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_SHIPPING_SIZE_KBN] = value; }
		}
		/// <summary>編集可能フラグ</summary>
		public string EditableFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_EDITABLE_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_EDITABLE_FLG] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSET_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSET_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSET_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSET_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
