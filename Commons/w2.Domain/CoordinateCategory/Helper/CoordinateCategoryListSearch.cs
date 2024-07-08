/*
=========================================================================================================
  Module      : コーディネートカテゴリ一覧検索条件クラス (CoordinateCategoryListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.CoordinateCategory.Helper
{
	/// <summary>
	/// コーディネートカテゴリ一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class CoordinateCategoryListSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */

		#region プロパティ
		/// <summary>カテゴリID（SQL LIKEエスケープ済） </summary>
		[DbMapName("coordinate_category_id_like_escaped")]
		public string CoordinateCategoryIdLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.CoordinateCategoryId); } }
		/// <summary>カテゴリID</summary>
		[DbMapName("coordinate_category__id")]
		public string CoordinateCategoryId { get; set; }
		/// <summary>親カテゴリID（SQL LIKEエスケープ済） </summary>
		[DbMapName("coordinate_parent_category_id_like_escaped")]
		public string CoordinateParentCategoryIdLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.CoordinateParentCategoryId); } }
		/// <summary>親カテゴリID</summary>
		[DbMapName("coordinate_parent_category_id")]
		public string CoordinateParentCategoryId { get; set; }
		/// <summary>カテゴリ SQL LIKEエスケープ済） </summary>
		[DbMapName("coordinate_category_like_escaped")]
		public string CoordinateCategoryLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.CoordinateCategory); } }
		/// <summary>カテゴリ</summary>
		[DbMapName("coordinate_category")]
		public string CoordinateCategory { get; set; }
		/// <summary>
		/// 開始行番号
		/// </summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>
		/// 終了行番号
		/// </summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		#endregion
	}

	/// <summary>
	///コーディネートモデル一覧検索クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class CoordinateCategoryListSearchResult : CoordinateCategoryModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CoordinateCategoryListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion
	}
}
