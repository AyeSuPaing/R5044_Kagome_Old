/*
=========================================================================================================
  Module      : 注文拡張項目設定マスタモデル (OrderExtendSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.OrderExtendSetting
{
	/// <summary>
	/// 注文拡張項目設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class OrderExtendSettingModel : ModelBase<OrderExtendSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderExtendSettingModel()
		{
			this.SettingId = "";
			this.SettingName = "";
			this.OutlineKbn = Constants.FLG_ORDEREXTENDSETTING_OUTLINE_TEXT;
			this.Outline = "";
			this.SortOrder = 1;
			this.InputType = "";
			this.InputDefault = "";
			this.InitOnlyFlg = Constants.FLG_ORDEREXTENDSETTING_UPDATABLE;
			this.Validator = "";
			this.DisplayKbn = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderExtendSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderExtendSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザ拡張項目ID</summary>
		public string SettingId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SETTING_ID]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SETTING_ID] = value; }
		}
		/// <summary>名称</summary>
		public string SettingName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SETTING_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SETTING_NAME] = value; }
		}
		/// <summary>概要表示区分</summary>
		public string OutlineKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_OUTLINE_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_OUTLINE_KBN] = value; }
		}
		/// <summary>概要</summary>
		public string Outline
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_OUTLINE]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_OUTLINE] = value; }
		}
		/// <summary>並び順</summary>
		public int SortOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SORT_ORDER]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_SORT_ORDER] = value; }
		}
		/// <summary>入力種別</summary>
		public string InputType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INPUT_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INPUT_TYPE] = value; }
		}
		/// <summary>初期値</summary>
		public string InputDefault
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INPUT_DEFAULT]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INPUT_DEFAULT] = value; }
		}
		/// <summary>登録時のみフラグ</summary>
		public string InitOnlyFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INIT_ONLY_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_INIT_ONLY_FLG] = value; }
		}
		/// <summary>表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DISPLAY_KBN] = value; }
		}
		/// <summary>バリデータテキスト</summary>
		public string Validator
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_VALIDATOR]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_VALIDATOR] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDEREXTENDSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}