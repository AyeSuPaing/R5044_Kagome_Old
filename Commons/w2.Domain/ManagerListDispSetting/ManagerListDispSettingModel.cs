/*
=========================================================================================================
  Module      : 表示設定管理モデル (ManagerListDispSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ManagerListDispSetting
{
	/// <summary>
	/// 表示設定管理モデル
	/// </summary>
	[Serializable]
	public partial class ManagerListDispSettingModel : ModelBase<ManagerListDispSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ManagerListDispSettingModel()
		{
			this.DispFlag = Constants.FLG_MANAGERLISTDISPSETTING_DISP_FLAG_ON;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ManagerListDispSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ManagerListDispSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSTATUSSETTING_SHOP_ID] = value; }
		}
		/// <summary>表示設定先区分</summary>
		public string DispSettingKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DISP_SETTING_KBN]; }
			set { this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DISP_SETTING_KBN] = value; }
		}
		/// <summary>表示項目名</summary>
		public string DispColmunName
		{
			get { return (string)this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DISP_COLUMN_NAME]; }
			set { this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DISP_COLUMN_NAME] = value; }
		}
		/// <summary>表示フラグ</summary>
		public string DispFlag
		{
			get { return (string)this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DISP_FLAG]; }
			set { this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DISP_FLAG] = value; }
		}
		/// <summary>項目の表示順</summary>
		public int DispOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DISP_ORDER]; }
			set { this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DISP_ORDER] = value; }
		}
		/// <summary>セルの幅</summary>
		public int ColmunWidth
		{
			get { return (int)this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_COLMUN_WIDTH]; }
			set { this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_COLMUN_WIDTH] = value; }
		}
		/// <summary>セル内の表示エリア</summary>
		public string ColmunAlign
		{
			get { return (string)this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_COLMUN_ALIGN]; }
			set { this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_COLMUN_ALIGN] = value; }
		}
		/// <summary>最終更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MANAGERLISTDISPSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}