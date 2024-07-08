/*
=========================================================================================================
  Module      : AdvCode Media Type List Search Condition (AdvCodeMediaTypeListSearchCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Linq;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.AdvCode.Helper
{
	/// <summary>
	/// AdvCode Media Type List Search Condition
	/// </summary>
	[Serializable]
	public class AdvCodeMediaTypeListSearchCondition : BaseDbMapModel
	{
		/// <summary>
		/// 閲覧可能な広告媒体区分を配列で取得
		/// </summary>
		/// <returns>閲覧可能な広告媒体区分配列</returns>
		public string[] GetUsableMediaTypeIdsArray()
		{
			if (string.IsNullOrEmpty(this.UsableMediaTypeIds)) return new string[0];

			var usableMediaTypes = this.UsableMediaTypeIds.Split(',')
				.Where(mediaType => (string.IsNullOrEmpty(mediaType) == false))
				.Distinct()
				.ToArray();
			return usableMediaTypes;
		}

		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>区分ID</summary>
		public string AdvcodeMediaTypeId { get; set; }
		/// <summary>区分ID（SQL LIKEエスケープ済）</summary>
		[DbMapName("advcode_media_type_id_like_escaped")]
		public string AdvcodeMediaTypeIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.AdvcodeMediaTypeId); }
		}
		/// <summary>媒体区分名</summary>
		public string AdvcodeMediaTypeName { get; set; }
		/// <summary>媒体区分名（SQL LIKEエスケープ済）</summary>
		[DbMapName("advcode_media_type_name_like_escaped")]
		public string AdvcodeMediaTypeNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.AdvcodeMediaTypeName); }
		}
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		/// <summary>ソート区分</summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }
		/// <summary>閲覧可能な広告媒体区分</summary>
		[DbMapName("usable_media_type_ids")]
		public string UsableMediaTypeIds { get; set; }
		#endregion
	}

	/// <summary>
	/// AdvCode Media Type List Search Result
	/// </summary>
	[Serializable]
	public class AdvCodeMediaTypeListSearchResult : AdvCodeMediaTypeModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AdvCodeMediaTypeListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ(Modelに実装している以外）
		#endregion
	}
}
