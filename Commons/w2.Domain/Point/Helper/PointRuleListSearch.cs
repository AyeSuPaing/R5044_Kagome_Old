/*
=========================================================================================================
  Module      : ポイントルールリスト検索のためのヘルパクラス (PointRuleListSearch.cs)
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
	/// ポイントルールリスト検索のためのヘルパクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	internal class PointRuleListSearch
	{	
		#region ~PointRuleListSearchResult ポイントルールリスト検索
		/// <summary>
		/// ポイントルールリスト検索
		/// </summary>
		/// <param name="cond">ポイントルールリスト検索条件クラス</param>
		/// <returns>検索結果</returns>
		internal PointRuleListSearchResult[] Search(PointRuleListSearchCondition cond)
		{
			using (var rep = new PointRepository())
			{
				return rep.SearchPointRuleList(cond);
			}
		}
		#endregion
	}

	/// <summary>
	/// ポイントルールリスト検索条件クラス
	/// </summary>
	[Serializable]
	public class PointRuleListSearchCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>
		/// 識別ID
		/// </summary>
		[DbMapName("dept_id")]
		public string DeptId { get; set; }

		/// <summary>
		/// ポイントルール区分
		/// </summary>
		[DbMapName("point_rule_kbn")]
		public string PointRuleKbn { get; set; }

		/// <summary>
		/// 検索キー
		/// 0：ポイント加算区分
		/// 99：条件なし
		/// </summary>
		[DbMapName("srch_key")]
		public int SearchKey { get; set; }

		/// <summary>
		/// 検索ワード
		/// </summary>
		[DbMapName("srch_word")]
		public string SearchWord { get; set; }

		/// <summary>
		/// 並び順区分
		/// 0：優先度/昇順
		/// 1：優先度/降順
		/// </summary>
		[DbMapName("sort_kbn")]
		public int SortKbn { get; set; }

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
	/// ポイントルールリスト検索結果クラス(PointRuleModelを拡張)
	/// </summary>
	[Serializable]
	public class PointRuleListSearchResult : PointRuleModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private PointRuleListSearchResult()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointRuleListSearchResult(PointRuleModel model)
			: this(model.DataSource)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointRuleListSearchResult(DataRowView source)
			: this()
		{
			this.DataSource = source.ToHashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointRuleListSearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ(UserPointHistoryModelに実装している以外）
		/// <summary>キャンペーン有効フラグ</summary>
		public string CampaignValidFlg
		{
			get { return (string)this.DataSource["campaign_valid_flg"]; }
			set { this.DataSource["campaign_valid_flg"] = value; }
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
