/*
=========================================================================================================
  Module      : 商品一覧表示設定マスタモデル (ProductListDispSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductListDispSetting
{
	/// <summary>
	/// 商品一覧表示設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductListDispSettingModel : ModelBase<ProductListDispSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductListDispSettingModel()
		{
			this.DispEnable = Constants.FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_ON;
			this.DefaultDispFlg = Constants.FLG_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG_OFF;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductListDispSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductListDispSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>設定ID</summary>
		public string SettingId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_ID] = value; }
		}
		/// <summary>表示名</summary>
		public string SettingName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME] = value; }
		}
		/// <summary>表示／非表示</summary>
		public string DispEnable
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_ENABLE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_ENABLE] = value; }
		}
		/// <summary>表示順</summary>
		public int DispNo
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_NO]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_NO] = value; }
		}
		/// <summary>デフォルト表示フラグ</summary>
		public string DefaultDispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG] = value; }
		}
		/// <summary>説明</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DESCRIPTION] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>設定区分</summary>
		public string SettingKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN] = value; }
		}
		/// <summary>表示件数</summary>
		public int? DispCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_COUNT];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_COUNT] = value; }
		}
		#endregion
	}
}
