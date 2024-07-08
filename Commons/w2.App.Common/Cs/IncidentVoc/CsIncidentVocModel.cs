/*
=========================================================================================================
  Module      : インシデントVOCモデル(CsIncidentVocModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.IncidentVoc
{
	/// <summary>
	/// インシデントVOCモデル
	/// </summary>
	[Serializable]
	public partial class CsIncidentVocModel : ModelBase<CsIncidentVocModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsIncidentVocModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">インシデントVOC情報</param>
		public CsIncidentVocModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">インシデントVOC情報</param>
		public CsIncidentVocModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTVOC_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_DEPT_ID] = value; }
		}
		/// <summary>VOC ID</summary>
		public string VocId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTVOC_VOC_ID]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_VOC_ID] = value; }
		}
		/// <summary>表示文言</summary>
		public string VocText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTVOC_VOC_TEXT]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_VOC_TEXT] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_CSINCIDENTVOC_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTVOC_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTVOC_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSINCIDENTVOC_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSINCIDENTVOC_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTVOC_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTVOC_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
