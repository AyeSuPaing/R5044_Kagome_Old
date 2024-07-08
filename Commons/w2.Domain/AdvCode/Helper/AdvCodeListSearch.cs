/*
=========================================================================================================
  Module      : AdvCode List Search Condition (AdvCodeListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.AdvCode.Helper
{
	/// <summary>
	/// AdvCode List Search Condition
	/// </summary>
	[Serializable]
	public class AdvCodeListSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>識別ID</summary>
		[DbMapName("dept_id")]
		public string DeptId { get; set; }
		/// <summary>広告コード</summary>
		public string AdvertisementCode { get; set; }
		/// <summary>広告コード（SQL LIKEエスケープ済）</summary>
		[DbMapName("advertisement_code_like_escaped")]
		public string AdvertisementCodeLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.AdvertisementCode); }
		}
		/// <summary>媒体名</summary>
		public string MediaName { get; set; }
		/// <summary>媒体名（SQL LIKEエスケープ済）</summary>
		[DbMapName("media_name_like_escaped")]
		public string MediaNameeLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.MediaName); }
		}
		/// <summary>広告媒体区分</summary>
		[DbMapName("advcode_media_type_id")]
		public string AdvcodeMediaTypeId { get; set; }
		/// <summary>有効フラグ</summary>
		[DbMapName("valid_flg")]
		public string ValidFlg { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		/// <summary>ソート区分</summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }
		/// <summary>会員ランクID</summary>
		[DbMapName("member_rank_id")]
		public string MemberRankId { get; set; }
		/// <summary>ユーザー管理レベルID</summary>
		[DbMapName("user_management_level_id")]
		public string UserManagementLevelId { get; set; }
		#endregion
	}

	/// <summary>
	///AdvCode List Search Result
	/// </summary>
	[Serializable]
	public class AdvCodeListSearchResult : AdvCodeModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AdvCodeListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ(Modelに実装している以外）
		/// <summary>>媒体区分名</summary>
		public string AdvcodeMediaTypeName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME]); }
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME] = value; }
		}
		#endregion
	}
}
