/*
=========================================================================================================
  Module      : 特集エリアタイプマスタモデル (FeatureAreaTypeModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FeatureArea
{
	/// <summary>
	/// 特集エリアタイプマスタモデル
	/// </summary>
	[Serializable]
	public partial class FeatureAreaTypeModel : ModelBase<FeatureAreaTypeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FeatureAreaTypeModel()
		{
			this.AreaTypeId = "";
			this.AreaTypeName = "";
			this.ActionType = "";
			this.InternalMemo = "";
			this.PcStartTag = "";
			this.PcRepeatTag = "";
			this.PcEndTag = "";
			this.PcScriptTag = "";
			this.SpStartTag = "";
			this.SpRepeatTag = "";
			this.SpEndTag = "";
			this.SpScriptTag = "";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaTypeModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureAreaTypeModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>特集エリアタイプID</summary>
		public string AreaTypeId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_AREA_TYPE_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_AREA_TYPE_ID] = value; }
		}
		/// <summary>特集エリアタイプ名</summary>
		public string AreaTypeName
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_AREA_TYPE_NAME]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_AREA_TYPE_NAME] = value; }
		}
		/// <summary>動作タイプ</summary>
		public string ActionType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_ACTION_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_ACTION_TYPE] = value; }
		}
		/// <summary>内部用メモ</summary>
		public string InternalMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_INTERNAL_MEMO]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_INTERNAL_MEMO] = value; }
		}
		/// <summary>PC開始タグ</summary>
		public string PcStartTag
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_PC_START_TAG]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_PC_START_TAG] = value; }
		}
		/// <summary>PC繰り返しタグ</summary>
		public string PcRepeatTag
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_PC_REPEAT_TAG]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_PC_REPEAT_TAG] = value; }
		}
		/// <summary>PC終了タグ</summary>
		public string PcEndTag
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_PC_END_TAG]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_PC_END_TAG] = value; }
		}
		/// <summary>PCスクリプトタグ</summary>
		public string PcScriptTag
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_PC_SCRIPT_TAG]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_PC_SCRIPT_TAG] = value; }
		}
		/// <summary>SP開始タグ</summary>
		public string SpStartTag
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_SP_START_TAG]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_SP_START_TAG] = value; }
		}
		/// <summary>SP繰り返しタグ</summary>
		public string SpRepeatTag
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_SP_REPEAT_TAG]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_SP_REPEAT_TAG] = value; }
		}
		/// <summary>SP終了タグ</summary>
		public string SpEndTag
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_SP_END_TAG]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_SP_END_TAG] = value; }
		}
		/// <summary>SPスクリプトタグ</summary>
		public string SpScriptTag
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_SP_SCRIPT_TAG]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_SP_SCRIPT_TAG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREAREATYPE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREAREATYPE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
