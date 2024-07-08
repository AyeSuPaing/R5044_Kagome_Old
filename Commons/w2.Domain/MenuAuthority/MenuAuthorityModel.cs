/*
=========================================================================================================
  Module      : メニュー権限管理マスタモデル (MenuAuthorityModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MenuAuthority
{
	/// <summary>
	/// メニュー権限管理マスタモデル
	/// </summary>
	[Serializable]
	public partial class MenuAuthorityModel : ModelBase<MenuAuthorityModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MenuAuthorityModel()
		{
			this.ShopId = "";
			this.PkgKbn = "";
			this.MenuAuthorityLevel = 0;
			this.MenuAuthorityName = "";
			this.MenuPath = "";
			this.FunctionLevel = 0;
			this.DefaultDispFlg = Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_OFF;
			this.DelFlg = "0";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MenuAuthorityModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MenuAuthorityModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_MENUAUTHORITY_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_SHOP_ID] = value; }
		}
		/// <summary>パッケージ区分</summary>
		public string PkgKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MENUAUTHORITY_PKG_KBN]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_PKG_KBN] = value; }
		}
		/// <summary>表示レベル</summary>
		public int MenuAuthorityLevel
		{
			get { return (int)this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_LEVEL]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_LEVEL] = value; }
		}
		/// <summary>メニュー権限名</summary>
		public string MenuAuthorityName
		{
			get { return (string)this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME] = value; }
		}
		/// <summary>メニューパス</summary>
		public string MenuPath
		{
			get { return (string)this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_PATH]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_MENU_PATH] = value; }
		}
		/// <summary>機能レベル</summary>
		public int FunctionLevel
		{
			get { return (int)this.DataSource[Constants.FIELD_MENUAUTHORITY_FUNCTION_LEVEL]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_FUNCTION_LEVEL] = value; }
		}
		/// <summary>デフォルト表示フラグ</summary>
		public string DefaultDispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MENUAUTHORITY_DEFAULT_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_DEFAULT_DISP_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MENUAUTHORITY_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MENUAUTHORITY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MENUAUTHORITY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MENUAUTHORITY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MENUAUTHORITY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
