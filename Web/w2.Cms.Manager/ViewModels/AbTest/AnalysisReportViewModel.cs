/*
=========================================================================================================
  Module      : 分析レポートモデル(AnalysisReportViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace w2.Cms.Manager.ViewModels.AbTest

{
	/// <summary>
	/// 分析レポートモデル
	/// </summary>
	[Serializable]
	public class AnalysisReportViewModel : ViewModelBase
	{
		private const bool HAS_NUMBER_DEFAULT = true;

		public AnalysisReportViewModel()
		{
			this.HasNumber = HAS_NUMBER_DEFAULT;
			this.TypeItems = new Dictionary<string, AnalysisReportTypeViewModel>();
			this.TypeItemNames = new Dictionary<string, string>();
		}

		/// <summary> ページID </summary>
		public string PageId { get; set; }
		/// <summary> ページ名 </summary>
		public string PageName { get; set; }
		/// <summary> 開始日 </summary>
		public string StartDate { get; set; }
		/// <summary> 終了日 </summary>
		public string EndDate { get; set; }
		/// <summary> 選択分析レポートタイプ名 </summary>
		public string AnalysisReportType { get; set; }
		/// <summary> 数値表示 </summary>
		public bool HasNumber { get; set; }
		/// <summary> 分析レポート種別モデル </summary>
		public Dictionary<string, AnalysisReportTypeViewModel> TypeItems { get; set; }
		/// <summary> 分析レポート種別モデル表示名 </summary>
		public Dictionary<string, string> TypeItemNames { get; set; }
	}

	/// <summary>
	/// 分析レポート種別モデル
	/// </summary>
	[Serializable]
	public class AnalysisReportTypeViewModel : ViewModelBase
	{
		public AnalysisReportTypeViewModel()
		{
			this.DetailItems = new Dictionary<string, AnalysisReportDetailViewModel>();
			this.DetailItemNames = new Dictionary<string, string>();
		}

		public bool IsSelected { get; set; }
		/// <summary> 分析レポート詳細モデル </summary>
		public Dictionary<string, AnalysisReportDetailViewModel> DetailItems { get; set; }
		/// <summary> 分析レポート詳細モデル表示名 </summary>
		public Dictionary<string, string> DetailItemNames { get; set; }
		/// <summary> 分析チャートモデル </summary>
		public AnalysisChartViewModel ReportCharts { get; set; }
	}

	/// <summary>
	/// 分析レポート詳細モデル
	/// </summary>
	[Serializable]
	public class AnalysisReportDetailViewModel : ViewModelBase
	{
		public AnalysisReportDetailViewModel()
		{
			this.DailyItems = new SortedDictionary<string, string>();
		}
		/// <summary> 日割りデータ(key:日付,value:データ) </summary>
		public SortedDictionary<string, string> DailyItems { get; set; }
	}

	/// <summary>
	/// 分析チャートモデル
	/// </summary>
	[Serializable]
	public class AnalysisChartViewModel
	{
		public AnalysisChartViewModel()
		{
			this.Labels = new List<string>();
			this.Datasets = new List<AnalysisChartDetailViewModel>();
		}

		/// <summary> ラベル </summary>
		[JsonProperty("labels")]
		public List<string> Labels { get; set; }
		/// <summary> データセット</summary>
		[JsonProperty("datasets")]
		public List<AnalysisChartDetailViewModel> Datasets { get; set; }
	}

	/// <summary>
	/// 分析チャートデータセットモデル
	/// </summary>
	[Serializable]
	public class AnalysisChartDetailViewModel
	{
		public AnalysisChartDetailViewModel()
		{
			this.ChartData = new List<float>();
		}

		/// <summary> 系列名 </summary>
		[JsonProperty("label")]
		public string Label { get; set; }
		/// <summary> チャートデータ </summary>
		[JsonProperty("data")]
		public List<float> ChartData { get; set; }
		/// <summary> 曲線度（0～1） </summary>
		[JsonProperty("tension")]
		public int Tension { get; set; }
		/// <summary> グラフを塗るか？ </summary>
		[JsonProperty("fill")]
		public bool Fill { get; set; }
		/// <summary> グラフの色 </summary>
		[JsonProperty("backgroundColor")]
		public string BackgroundColor { get; set; }
		/// <summary> グラフ線の色 </summary>
		[JsonProperty("borderColor")]
		public string BorderColor { get; set; }
		/// <summary> グラフ線の太さ </summary>
		[JsonProperty("borderWidth")]
		public int BorderWidth { get; set; }
	}
}
