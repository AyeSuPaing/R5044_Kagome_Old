/*
=========================================================================================================
  Module      : インシデントカテゴリマスタモデルのパーシャルクラス(CsIncidentCategoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.IncidentCategory
{
	/// <summary>
	/// インシデントカテゴリマスタモデルのパーシャルクラス
	/// </summary>
	public partial class CsIncidentCategoryModel : ModelBase<CsIncidentCategoryModel>
	{
		/// <summary>
		/// 総件数
		/// </summary>
		public int EX_RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
		/// <summary>
		/// 有効フラグ表示文字列
		/// </summary>
		public string EX_ValidFlgText
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSINCIDENTCATEGORY, Constants.FIELD_CSINCIDENTCATEGORY_VALID_FLG, this.ValidFlg); }
		}

		/// <summary>インシデントカテゴリ名（階層で字下げを行う）</summary>
		public string EX_CategoryNameForDropdown
		{
			get { return new string('　', this.EX_RankNo -1) + this.CategoryName; }
		}
		/// <summary>ランクNO（階層の深さ）</summary>
		public int EX_RankNo
		{
			get { return (int)this.DataSource["rank_no"]; }
		}
		/// <summary>ランク考慮したカテゴリID</summary>
		public string EX_RankedCategoryId
		{
			get { return (string)this.DataSource["ranked_category_id"]; }
		}
		/// <summary>親のカテゴリID絶対パス</summary>
		public string EX_ParentCategoryPath
		{
			get { return (string)this.DataSource["parent_category_path"]; }
		}
		/// <summary>親階層を考慮した有効フラグ</summary>
		public string EX_RankedValidFlg
		{
			get { return (string)this.DataSource["ranked_valid_flg"]; }
		}
		/// <summary>親カテゴリ名称</summary>
		public string EX_ParentCategoryName
		{
			get { return StringUtility.ToEmpty(this.DataSource["EX_ParentCategoryName"]); }
			set { this.DataSource["EX_ParentCategoryName"] = value; }
		}
		/// <summary>親カテゴリがすべて有効か（インシデントカテゴリ管理一覧で利用）</summary>
		public bool EX_IsParentValid
		{
			get { return StringUtility.ToEmpty(this.EX_RankedValidFlg) == Constants.FLG_CSINCIDENTCATEGORY_VALID_FLG_VALID; }
		}

	}
}
