/*
=========================================================================================================
  Module      : 回答例カテゴリモデルのパーシャルクラス(CsAnswerTemplateCategoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.App.Common.Cs.AnswerTemplate
{
	/// <summary>
	/// 回答例カテゴリモデルのパーシャルクラス
	/// 自動生成されないものはこちらで定義
	/// </summary>
	public partial class CsAnswerTemplateCategoryModel : ModelBase<CsAnswerTemplateCategoryModel>
	{
		/// <summary>
		/// 拡張項目_総件数
		/// </summary>
		public int EX_SearchCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
		/// <summary>
		/// 拡張項目_ランク付けカテゴリ名
		/// </summary>
		public string EX_RankedCategoryName
		{
			get { return (string)this.DataSource["ranked_category_name"]; }
		}
		/// <summary>
		/// 拡張項目_ランク付けカテゴリID
		/// </summary>
		public string EX_RankedCategoryId
		{
			get { return (string)this.DataSource["ranked_category_id"]; }
		}
		/// <summary>
		/// 拡張項目_ランク考慮した有効フラグ
		/// </summary>
		public string EX_RankedValidFlg
		{
			get { return (string)this.DataSource["ranked_valid_flg"]; }
		}
		/// <summary>
		/// 拡張項目_有効フラグ表示文字列
		/// </summary>
		public string EX_ValidFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_CSANSWERTEMPLATECATEGORY, Constants.FIELD_CSANSWERTEMPLATECATEGORY_VALID_FLG, this.ValidFlg);
			}
		}
		/// <summary>
		/// 拡張項目_親カテゴリ名称
		/// </summary>
		public string EX_ParentCategoryName
		{
			get { return StringUtility.ToEmpty(this.DataSource["EX_ParentCategoryName"]); }
			set { this.DataSource["EX_ParentCategoryName"] = value; }
		}
	}
}
