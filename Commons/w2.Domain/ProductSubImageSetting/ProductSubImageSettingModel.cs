/*
=========================================================================================================
  Module      : 商品サブ画像設定マスタモデル(ProductSubImageSettingModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductSubImageSetting
{
	/// <summary>
	/// 商品サブ画像設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductSubImageSettingModel : ModelBase<ProductSubImageSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductSubImageSettingModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductSubImageSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductSubImageSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_SHOP_ID] = value; }
		}
		/// <summary>商品サブ画像ID</summary>
		public string ProductSubImageId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_ID] = value; }
		}
		/// <summary>商品サブ画像番号</summary>
		public int? ProductSubImageNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO] = value; }
		}
		/// <summary>商品サブ画像名称</summary>
		public string ProductSubImageName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME] = value; }
		}
		/// <summary>商品サブ画像説明</summary>
		public string ProductSubImageDiscription
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_DISCRIPTION] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSUBIMAGESETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
