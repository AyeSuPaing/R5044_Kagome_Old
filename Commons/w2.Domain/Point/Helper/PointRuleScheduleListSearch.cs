/*
=========================================================================================================
  Module      : ポイントルールスケジュールリスト検索のためのヘルパクラス (PointRuleScheduleListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
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
	/// ポイントルールスケジュールリスト検索のためのヘルパクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	internal class PointRuleScheduleListSearch
	{
		#region ~PointRuleScheduleListSearchResult ポイントルールスケジュールリスト検索
		/// <summary>
		/// ポイントルールスケジュールリスト検索
		/// </summary>
		/// <param name="cond">ポイントルールスケジュールリスト検索条件クラス</param>
		/// <returns>検索結果</returns>
		internal PointRuleScheduleListSearchResult[] Search(PointRuleScheduleListSearchCondition cond)
		{
			using (var rep = new PointRepository())
			{
				return rep.SearchPointRuleScheduleList(cond);
			}
		}
		#endregion
	}

	/// <summary>
	/// ポイントルールスケジュールリスト検索条件クラス
	/// </summary>
	[Serializable]
	public class PointRuleScheduleListSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>
		/// ポイントルールスケジュールID
		/// </summary>
		[DbMapName("point_rule_schedule_id")]
		public string PointRuleScheduleId { get; set; }

		/// <summary>
		/// ポイントルールスケジュール名
		/// </summary>
		[DbMapName("point_rule_schedule_name")]
		public string PointRuleScheduleName { get; set; }

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
	/// ポイントルールスケジュールリスト検索結果クラス(PointRuleScheduleModelを拡張)
	/// </summary>
	[Serializable]
	public class PointRuleScheduleListSearchResult : PointRuleScheduleModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private PointRuleScheduleListSearchResult()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointRuleScheduleListSearchResult(PointRuleModel model)
			: this(model.DataSource)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointRuleScheduleListSearchResult(DataRowView source)
			: this()
		{
			this.DataSource = source.ToHashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointRuleScheduleListSearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>検索結果の総合計行数</summary>
		public int RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
			set { this.DataSource["row_count"] = value; }
		}
		#endregion
	}
}
