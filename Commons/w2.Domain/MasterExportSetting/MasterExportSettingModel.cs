/*
=========================================================================================================
  Module      : マスタ出力定義モデル (MasterExportSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MasterExportSetting
{
	/// <summary>
	/// マスタ出力定義モデル
	/// </summary>
	[Serializable]
	public partial class MasterExportSettingModel : ModelBase<MasterExportSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MasterExportSettingModel()
		{
			this.ExportFileType = Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MasterExportSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MasterExportSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID] = value; }
		}
		/// <summary>マスタ区分</summary>
		public string MasterKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN]; }
			set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN] = value; }
		}
		/// <summary>設定ID</summary>
		public string SettingId
		{
			get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID]; }
			set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID] = value; }
		}
		/// <summary>設定名</summary>
		public string SettingName
		{
			get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SETTING_NAME]; }
			set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_SETTING_NAME] = value; }
		}
		/// <summary>フィールド列</summary>
		public string Fields
		{
			get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_FIELDS]; }
			set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_FIELDS] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>出力ファイル形式</summary>
		public string ExportFileType
		{
			get { return (string)this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE]; }
			set { this.DataSource[Constants.FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE] = value; }
		}
		#endregion
	}
}