/*
=========================================================================================================
  Module      : ユーザーポイント履歴検索のためのヘルパクラス (UserPointHistorySearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Point.Helper
{
	/// <summary>
	/// ユーザーポイント履歴検索のためのヘルパクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	internal class UserPointHistorySearch
	{
		#region ~Search ユーザーポイント履歴検索
		/// <summary>
		/// ユーザーポイント履歴検索
		/// </summary>
		/// <param name="cond">ユーザーポイント履歴検索条件クラス</param>
		/// <returns>検索結果</returns>
		internal UserPointHistorySearchResult[] Search(UserPointHistorySearchCondition cond)
		{
			using (var rep = new PointRepository())
			{
				return rep.SearchUserPointHistory(cond);
			}
		}
		#endregion

		#region ~ユーザーポイント履歴（概要）検索
		/// <summary>
		/// ユーザーポイント履歴（概要）検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果</returns>
		internal UserPointHistorySummarySearchResult[] SearchSummary(UserPointHistorySummarySearchCondition condition)
		{
			using (var repository = new PointRepository())
			{
				var result = repository.SearchUserPointHistorySummary(condition);
				return result;
			}
		}
		#endregion

		#region ~ユーザーポイント履歴（概要）検索数取得
		/// <summary>
		/// ユーザーポイント履歴（概要）検索数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索ヒット数</returns>
		public int GetSearchHitCountUserPointHistorySummary(UserPointHistorySummarySearchCondition condition)
		{
			using (var repository = new PointRepository())
			{
				var result = repository.GetSearchHitCountUserPointHistorySummary(condition);
				return result;
			}
		}
		#endregion
	}

	/// <summary>
	/// ユーザーポイント履歴（概要）の検索条件クラス
	/// </summary>
	public class UserPointHistorySummarySearchCondition : BaseDbMapModel
	{
		#region プロパティ
		/// <summary>ユーザーID</summary>
		[DbMapName("user_id")]
		public string UserId { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		#endregion
	}

	/// <summary>
	/// ユーザーポイント検索条件クラス
	/// </summary>
	[Serializable]
	public class UserPointHistorySearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>ユーザーID</summary>
		[DbMapName("user_id")]
		public string UserId { get; set; }
		/// <summary>ポイント区分</summary>
		[DbMapName("point_kbn")]
		public string PointKbn { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNumber { get; set; }
		#endregion
	}

	/// <summary>
	/// ユーザーポイント履歴（概要）の検索結果クラス
	/// </summary>
	[Serializable]
	public class UserPointHistorySummarySearchResult : UserPointHistoryModel
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserPointHistorySummarySearchResult()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserPointHistorySummarySearchResult(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserPointHistorySummarySearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ(UserPointHistoryModelに実装している以外）
		/// <summary>行番号</summary>
		public long RowNumber
		{
			get { return (long)this.DataSource["row_number"]; }
			set { this.DataSource["row_number"] = value; }
		}
		/// <summary>ポイント合計</summary>
		public decimal PointTotal
		{
			get { return (decimal)this.DataSource["point_total"]; }
			set { this.DataSource["point_total"] = value; }
		}
		/// <summary>ユーザー名</summary>
		public string UserName
		{
			get { return (string)this.DataSource["user_name"]; }
			set { this.DataSource["user_name"] = value; }
		}
		#endregion
	}

	/// <summary>
	/// ユーザーポイントの検索結果クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class UserPointHistorySearchResult : UserPointHistoryModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private UserPointHistorySearchResult()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserPointHistorySearchResult(UserPointHistoryModel model)
			: this(model.DataSource)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserPointHistorySearchResult(DataRowView source)
			: this()
		{
			this.DataSource = source.ToHashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserPointHistorySearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ(UserPointHistoryModelに実装している以外）
		/// <summary>ユーザー名</summary>
		public string UserName
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_NAME]; }
			set { this.DataSource[Constants.FIELD_USER_NAME] = value; }
		}
		/// <summary>有効期限延長符号</summary>
		public string PointExpExtendOperator
		{
			get { return (string)this.DataSource["point_exp_extend_operator"]; }
			set { this.DataSource["point_exp_extend_operator"] = value; }
		}
		/// <summary>有効期限延長年</summary>
		public new string PointExpExtendYear
		{
			get { return (string)this.DataSource["point_exp_extend_year"]; }
			set { this.DataSource["point_exp_extend_year"] = value; }
		}
		/// <summary>有効期限延長月</summary>
		public new string PointExpExtendMonth
		{
			get { return (string)this.DataSource["point_exp_extend_month"]; }
			set { this.DataSource["point_exp_extend_month"] = value; }
		}
		/// <summary>有効期限延長日</summary>
		public string PointExpExtendDay
		{
			get { return (string)this.DataSource["point_exp_extend_day"]; }
			set { this.DataSource["point_exp_extend_day"] = value; }
		}
		/// <summary>検索結果の総合計行数</summary>
		public int RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
			set { this.DataSource["row_count"] = value; }
		}
		#endregion
	}
}
