/*
=========================================================================================================
  Module      : 回答例モデル(CsAnswerTemplateModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.AnswerTemplate
{
	/// <summary>
	/// 回答例モデル
	/// </summary>
	[Serializable]
	public partial class CsAnswerTemplateModel : ModelBase<CsAnswerTemplateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsAnswerTemplateModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">情報</param>
		public CsAnswerTemplateModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">情報</param>
		public CsAnswerTemplateModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DEPT_ID] = value; }
		}
		/// <summary>回答例ID</summary>
		public string AnswerId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_ID]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_ID] = value; }
		}
		/// <summary>回答例カテゴリID</summary>
		public string AnswerCategoryId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_CATEGORY_ID]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_CATEGORY_ID] = value; }
		}
		/// <summary>回答例タイトル</summary>
		public string AnswerTitle
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_TITLE]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_TITLE] = value; }
		}
		/// <summary>回答例本文</summary>
		public string AnswerText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_TEXT]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_TEXT] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_LAST_CHANGED] = value; }
		}
		/// <summary>件名</summary>
		public string AnswerMailTitle
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_MAIL_TITLE]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_MAIL_TITLE] = value; }
		}
		#endregion
	}
}
