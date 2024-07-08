/*
=========================================================================================================
  Module      : CPM（顧客ポートフォリオマネジメント）クラスタ数格納クラス(CpmClusterCount.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Extensions;
using w2.Domain.DispSummaryAnalysis;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// CPMクラスタレポート
	/// </summary>
	[Serializable]
	public class CpmClusterReport
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="items">レポートアイテム</param>
		public CpmClusterReport(IEnumerable<CpmClusterReportItem> items)
		{
			this.Items = items.ToList();
		}

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="id">クラスタID</param>
		/// <returns>レポートアイテム</returns>
		public CpmClusterReportItem Get(string id)
		{
			var data = this.Items.FirstOrDefault(d => d.CpmClusterId == id);
			return data;
		}

		/// <summary>
		/// アイテムのパーセンテージを計算
		/// </summary>
		public void CalculateItemPercentage()
		{
			// トータル取得
			var total = this.Items.Sum(ri => ri.Count);
			if (total == 0) total = 1;
			int percentSum = 0;

			// パーセンテージ計算（データがないものは空のものを作成）
			foreach (var repotyItem in this.Items)
			{
				if (repotyItem.Count.HasValue == false) continue;
				var percent = (int)((decimal)repotyItem.Count.Value * 100 / total + (decimal)0.5); // % は小数点四捨五入
				if (this.Items[this.Items.Count - 1] == repotyItem)
				{
					percent = 100 - percentSum; // 最後の要素は残りを割り当て
				}
				repotyItem.Percentage = percent;
				percentSum += percent;
			}
		}

		/// <summary>番号（日次レポートでは日、月次レポートでは月を格納）</summary>
		public int No { get; set; }
		/// <summary>CPMクラスタアイテムリスト</summary>
		public List<CpmClusterReportItem> Items { get; set; }
	}

	/// <summary>
	/// CPMクラスタレポートアイテム
	/// </summary>
	[Serializable]
	public class CpmClusterReportItem : ModelBase<CpmClusterReportItem>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ（空のデータ）
		/// </summary>
		public CpmClusterReportItem()
		{
			this.CpmClusterId = null;
			this.Count = null;
			this.Percentage = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CpmClusterReportItem(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CpmClusterReportItem(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="id">クラスタID</param>
		public CpmClusterReportItem(string id)
			: this()
		{
			this.CpmClusterId = id;
			this.Count = 0;
			this.Percentage = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">クラスタID</param>
		public CpmClusterReportItem(DispSummaryAnalysisModel model)
			: this()
		{
			this.CpmClusterId = model.ValueName;
			this.Count = model.Counts;
			this.Percentage = (int)model.Reserved1;
		}
		#endregion

		/// <summary>クラスタID</summary>
		public string CpmClusterId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ID]; }
			set { this.DataSource[Constants.FIELD_USERATTRIBUTE_CPM_CLUSTER_ID] = value; }
		}

		/// <summary>カウント</summary>
		public long? Count
		{
			get { return (long?)this.DataSource["count"]; }
			set { this.DataSource["count"] = value; }
		}
		/// <summary>割合</summary>
		public int? Percentage
		{
			get { return (int?)this.DataSource["percentage"]; }
			set { this.DataSource["percentage"] = value; }
		}
		/// <summary>伸び数</summary>
		public long? GrowthCount
		{
			get { return (long?)this.DataSource["GrowthCount"]; }
			set { this.DataSource["GrowthCount"] = value; }
		}
		/// <summary>伸び率</summary>
		public int? GrowthRate
		{
			get { return (int?)this.DataSource["GrowthRate"]; }
			set { this.DataSource["GrowthRate"] = value; }
		}
	}
}
