/*
=========================================================================================================
  Module      : 商品グループマスタモデル (ProductGroupModel.cs)
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
	/// 商品グループマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductGroupModel : ModelBase<ProductGroupModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductGroupModel()
		{
			this.EndDate = null;
			this.ValidFlg = Constants.FLG_PRODUCTGROUP_VALID_FLG_VALID;
			this.ProductGroupPageContentsKbn = Constants.FLG_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN_TEXT;
			this.ProductGroupPageContents = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductGroupModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductGroupModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>商品グループID</summary>
		public string ProductGroupId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID] = value; }
		}
		/// <summary>商品グループ名</summary>
		public string ProductGroupName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_NAME] = value; }
		}
		/// <summary>開始日時</summary>
		public DateTime BeginDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTGROUP_BEGIN_DATE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_BEGIN_DATE] = value; }
		}
		/// <summary>終了日時</summary>
		public DateTime? EndDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTGROUP_END_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCTGROUP_END_DATE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_END_DATE] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_VALID_FLG] = value; }
		}
		/// <summary>商品グループページ表示内容HTML区分</summary>
		public string ProductGroupPageContentsKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN] = value; }
		}
		/// <summary>商品グループページ表示内容</summary>
		public string ProductGroupPageContents
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTGROUP_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTGROUP_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
