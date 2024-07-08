/*
=========================================================================================================
  Module      : Product Default Setting Model (ProductDefaultSettingModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductDefaultSetting
{
	/// <summary>
	/// Product default setting model
	/// </summary>
	[Serializable]
	public partial class ProductDefaultSettingModel : ModelBase<ProductDefaultSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductDefaultSettingModel()
		{
			this.ShopId = string.Empty;
			this.InitData = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductDefaultSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductDefaultSettingModel(Hashtable source)
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTDEFAULTSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SHOP_ID] = value; }
		}
		/// <summary>初期設定</summary>
		public string InitData
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTDEFAULTSETTING_INIT_DATA]; }
			set { this.DataSource[Constants.FIELD_PRODUCTDEFAULTSETTING_INIT_DATA] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTDEFAULTSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTDEFAULTSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTDEFAULTSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
