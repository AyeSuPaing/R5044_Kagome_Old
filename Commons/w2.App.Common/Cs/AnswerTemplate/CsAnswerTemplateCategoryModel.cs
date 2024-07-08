/*
=========================================================================================================
  Module      : 回答例カテゴリモデル(CsAnswerTemplateCategoryModel.cs)
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
	/// 回答例カテゴリモデル
	/// </summary>
	[Serializable]
	public partial class CsAnswerTemplateCategoryModel : ModelBase<CsAnswerTemplateCategoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsAnswerTemplateCategoryModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">情報</param>
		public CsAnswerTemplateCategoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">情報</param>
		public CsAnswerTemplateCategoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DEPT_ID] = value; }
		}
		/// <summary>回答例カテゴリID</summary>
		public string CategoryId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_ID]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_ID] = value; }
		}
		/// <summary>回答例親カテゴリID</summary>
		public string ParentCategoryId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_PARENT_CATEGORY_ID]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_PARENT_CATEGORY_ID] = value; }
		}
		/// <summary>回答例カテゴリ名</summary>
		public string CategoryName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_NAME]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_NAME] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSANSWERTEMPLATECATEGORY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
