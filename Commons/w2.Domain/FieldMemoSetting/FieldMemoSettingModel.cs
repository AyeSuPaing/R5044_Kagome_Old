/*
=========================================================================================================
  Module      : 項目メモ設定マスタモデル (FieldMemoSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FieldMemoSetting
{
	/// <summary>
	/// 項目メモ設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class FieldMemoSettingModel : ModelBase<FieldMemoSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FieldMemoSettingModel()
		{
			this.TableName = "";
			this.FieldName = "";
			this.Memo = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FieldMemoSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FieldMemoSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>テーブル名</summary>
		public string TableName
		{
			get { return (string)this.DataSource[Constants.FIELD_FIELDMEMOSETTING_TABLE_NAME]; }
			set { this.DataSource[Constants.FIELD_FIELDMEMOSETTING_TABLE_NAME] = value; }
		}
		/// <summary>フィールド名</summary>
		public string FieldName
		{
			get { return (string)this.DataSource[Constants.FIELD_FIELDMEMOSETTING_FIELD_NAME]; }
			set { this.DataSource[Constants.FIELD_FIELDMEMOSETTING_FIELD_NAME] = value; }
		}
		/// <summary>メモ</summary>
		public string Memo
		{
			get { return (string)this.DataSource[Constants.FIELD_FIELDMEMOSETTING_MEMO]; }
			set { this.DataSource[Constants.FIELD_FIELDMEMOSETTING_MEMO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIELDMEMOSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIELDMEMOSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIELDMEMOSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIELDMEMOSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FIELDMEMOSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIELDMEMOSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
