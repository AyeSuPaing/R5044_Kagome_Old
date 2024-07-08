/*
=========================================================================================================
  Module      : レコメンド設定一覧検索のためのヘルパクラス (RecommendListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Recommend.Helper
{
	#region +レコメンド設定一覧検索条件クラス
	/// <summary>
	/// レコメンド設定一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class RecommendListSearchCondition : BaseDbMapModel
	{
		/// <summary>店舗ID</summary>
		[DbMapName("shop_id")]
		public string ShopId { get; set; }
		/// <summary>レコメンド設定ID</summary>
		public string RecommendId { get; set; }
		/// <summary>レコメンド設定ID（SQL LIKEエスケープ済）</summary>
		[DbMapName("recommend_id_like_escaped")]
		public string RecommendIdLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.RecommendId); }
		}
		/// <summary>レコメンド名（管理用）</summary>
		public string RecommendName { get; set; }
		/// <summary>レコメンド名（管理用）（SQL LIKEエスケープ済）</summary>
		[DbMapName("recommend_name_like_escaped")]
		public string RecommendNameLikeEscaped
		{
			get { return StringUtility.SqlLikeStringSharpEscape(this.RecommendName); }
		}
		/// <summary>レコメンド区分</summary>
		[DbMapName("recommend_kbn")]
		public string RecommendKbn { get; set; }
		/// <summary>開催状況</summary>
		[DbMapName("status")]
		public string Status { get; set; }
		/// <summary>
		/// 並び順区分
		/// 0：開催状態順
		/// 1：レコメンドID/昇順
		/// 2：レコメンドID/降順
		/// 3：レコメンド名(管理用)/昇順
		/// 4：レコメンド名(管理用)/降順
		/// 5：適用優先順/昇順
		/// 6：適用優先順/降順
		/// 7：開始日時/昇順
		/// 8：開始日時/降順
		/// 9：終了日時/昇順
		/// 10：終了日時/降順
		/// </summary>
		[DbMapName("sort_kbn")]
		public string SortKbn { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		/// <summary>
		/// レコメンドレポート 並び順区分
		/// 0：開催状態順
		/// 1：レコメンド名(管理用)/昇順
		/// 2：レコメンド名(管理用)/降順
		/// 3：PV数/昇順
		/// 4：PV数/降順
		/// 5：CV数/昇順
		/// 6：CV数/降順
		/// 7：CV率(%)/昇順
		/// 8：CV率(%)/降順
		/// </summary>
		[DbMapName("report_sort_kbn")]
		public string ReportSortKbn { get; set; }
		/// <summary>開始時間</summary>
		[DbMapName("date_bgn")]
		public DateTime BeginDate { get; set; }
		/// <summary>終了時間</summary>
		[DbMapName("date_end")]
		public DateTime EndDate { get; set; }
		/// <summary>有効フラグ</summary>
		[DbMapName("valid_flg")]
		public string ValidFlg { get; set; }
	}
	#endregion

	#region +レコメンド設定入一覧検索結果クラス
	/// <summary>
	/// レコメンド設定一覧検索結果クラス
	/// ※RecommendModelを拡張
	/// </summary>
	[Serializable]
	public class RecommendListSearchResult : RecommendModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		#endregion
	}
	#endregion
}