/*
=========================================================================================================
  Module      : かんたん会員登録設定マスタモデル (UserEasyRegisterSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.UserEasyRegisterSetting
{
	/// <summary>
	/// かんたん会員登録設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class UserEasyRegisterSettingModel : ModelBase<UserEasyRegisterSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserEasyRegisterSettingModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserEasyRegisterSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserEasyRegisterSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>項目ID</summary>
		public string ItemId
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_ITEM_ID]; }
			set { this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_ITEM_ID] = value; }
		}
		/// <summary>表示フラグ</summary>
		public string DisplayFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_DISPLAY_FLG]; }
			set { this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_DISPLAY_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USEREASYREGISTERSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
