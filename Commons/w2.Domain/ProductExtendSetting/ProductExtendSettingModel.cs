/*
=========================================================================================================
  Module      : Product Extend Setting Model (ProductExtendSettingModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductExtendSetting
{
	/// <summary>
	/// Product Extend Setting Model
	/// </summary>
	[Serializable]
	public class ProductExtendSettingModel : ModelBase<ProductExtendSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductExtendSettingModel()
		{
			this.ShopId = Constants.CONST_DEFAULT_SHOP_ID;
			this.ExtendNo = 1;
			this.Name = string.Empty;
			this.DelFlg = Constants.FLG_PRODUCTEXTENDSETTING_DEL_FLG_OFF;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductExtendSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductExtendSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_SHOP_ID] = value; }
		}
		/// <summary>拡張項目番号</summary>
		public int ExtendNo
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]; }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO] = value; }
		}
		/// <summary>拡張項目名称</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME] = value; }
		}
		/// <summary>拡張項目説明</summary>
		public string Discription
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_DISCRIPTION] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_DEL_FLG] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTENDSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}