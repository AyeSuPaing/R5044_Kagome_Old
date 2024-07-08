/*
=========================================================================================================
  Module      : スタッフ一覧検索条件クラス (StaffListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Staff.Helper
{
	/// <summary>
	/// スタッフ一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class StaffListSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */

		#region プロパティ
		/// <summary>スタッフID（SQL LIKEエスケープ済） </summary>
		[DbMapName("staff_id_like_escaped")]
		public string StaffIdLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.StaffId); } }
		/// <summary>スタッフID</summary>
		[DbMapName("staff_id")]
		public string StaffId { get; set; }
		/// <summary>スタッフ名（SQL LIKEエスケープ済） </summary>
		[DbMapName("staff_name_like_escaped")]
		public string StaffNameLikeEscaped { get { return StringUtility.SqlLikeStringSharpEscape(this.StaffName); } }
		/// <summary>スタッフ名</summary>
		[DbMapName("staff_name")]
		public string StaffName { get; set; }
		/// <summary>身長下限</summary>
		[DbMapName("height_lower_limit")]
		public string HeightLowerLimit { get; set; }
		/// <summary>身長上限</summary>
		[DbMapName("height_upper_limit")]
		public string HeightUpperLimit { get; set; }
		/// <summary>有効フラグ</summary>
		[DbMapName("valid_flg")]
		public string ValidFlg { get; set; }
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
	///スタッフ一覧検索クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class StaffListSearchResult : StaffModel
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public StaffListSearchResult()
		{
			this.StaffId = "";
			this.StaffName = "";
			this.StaffProfile = "";
			this.StaffHeight = 0;
			this.StaffInstagramId = "";
			this.StaffSex = Constants.FLG_STAFF_SEX_UNKNOWN;
			this.ModelFlg = Constants.FLG_STAFF_MODEL_FLG_VALID;
			this.OperatorId = "";
			this.RealShopId = "";
			this.ValidFlg = Constants.FLG_STAFF_VALID_FLG_VALID;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public StaffListSearchResult(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public StaffListSearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion
	}
}
