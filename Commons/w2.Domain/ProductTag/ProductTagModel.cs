/*
=========================================================================================================
  Module      : 商品タグマスタモデル (ProductTagModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductTag
{
	/// <summary>
	/// 商品タグマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductTagModel : ModelBase<ProductTagModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductTagModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductTagModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductTagModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAG_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAG_PRODUCT_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTTAG_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAG_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTTAG_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAG_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAG_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAG_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
