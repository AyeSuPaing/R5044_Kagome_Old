/*
=========================================================================================================
  Module      : ユーザ拡張項目設定マスタモデル (UserExtendSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.UserExtendSetting
{
	/// <summary>
	/// ユーザ拡張項目設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class UserExtendSettingModel : ModelBase<UserExtendSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserExtendSettingModel()
		{
			this.SettingId = string.Empty;
			this.SettingName = string.Empty;
			this.OutlineKbn = Constants.FLG_USEREXTENDSETTING_OUTLINE_TEXT;
			this.Outline = string.Empty;
			this.SortOrder = 0;
			this.InputType = Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT;
			this.InputDefault = string.Empty;
			this.InitOnlyFlg = Constants.FLG_USEREXTENDSETTING_UPDATABLE;
			this.DisplayKbn = string.Empty;
			this.LastChanged = string.Empty;
			this.IsRegisted = false;
			this.DeleteFlg = false;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserExtendSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserExtendSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザ拡張項目ID</summary>
		public string SettingId
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_SETTING_ID]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_SETTING_ID] = value; }
		}
		/// <summary>名称</summary>
		public string SettingName
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_SETTING_NAME]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_SETTING_NAME] = value; }
		}
		/// <summary>ユーザ拡張項目概要表示区分</summary>
		public string OutlineKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE_KBN]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE_KBN] = value; }
		}
		/// <summary>ユーザ拡張項目概要</summary>
		public string Outline
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_OUTLINE] = value; }
		}
		/// <summary>並び順</summary>
		public int SortOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_USEREXTENDSETTING_SORT_ORDER]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_SORT_ORDER] = value; }
		}
		/// <summary>入力種別</summary>
		public string InputType
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_INPUT_TYPE]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_INPUT_TYPE] = value; }
		}
		/// <summary>初期値</summary>
		public string InputDefault
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_INPUT_DEFAULT]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_INPUT_DEFAULT] = value; }
		}
		/// <summary>登録時のみフラグ</summary>
		public string InitOnlyFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_INIT_ONLY_FLG]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_INIT_ONLY_FLG] = value; }
		}
		/// <summary>表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_DISPLAY_KBN] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USEREXTENDSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USEREXTENDSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREXTENDSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USEREXTENDSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
