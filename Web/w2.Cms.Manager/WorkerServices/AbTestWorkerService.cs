/*
=========================================================================================================
  Module      : ABテストワーカーサービス(AbTestWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.AbTest;
using w2.Cms.Manager.ViewModels.AbTest;
using w2.Common.Util;
using w2.Domain.AbTest;
using w2.Domain.ContentsLog;
using w2.Domain.ContentsSummaryAnalysis;
using w2.Domain.LandingPage;
using w2.Domain.LandingPage.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// ABテストワーカーサービス
	/// </summary>
	public class AbTestWorkerService : BaseWorkerService
	{
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="param">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public AbTestListViewModel GetListView(AbTestListParamModel param)
		{
			var models = new AbTestService().Search(param.SearchWord);
			var result = new AbTestListViewModel
			{
				ActionStatus = ActionStatus.List,
				Items = models.Select(CreateDetailView).ToArray(),
				ParamModel = param
			};

			return result;
		}

		/// <summary>
		/// 詳細ビュー生成
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>詳細ビュー</returns>
		private AbTestListViewModel.AbTestListItemDetailViewModel CreateDetailView(AbTestModel model)
		{
			var dateChanged = DateTimeUtility.ToStringForManager(
				model.DateChanged,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter).Split(new[] { " " }, StringSplitOptions.None);

			var contentsIds = model.Items.Select(item => model.AbTestId + "-" + item.PageId).ToArray();
			var summary = new ContentsLogService().GetContentsSummaryData(Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_ABTEST, contentsIds);
			var summaryToday = new ContentsLogService().GetContentsSummaryDataOfToday(Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_ABTEST, contentsIds);
			var result = new AbTestListViewModel.AbTestListItemDetailViewModel
			{
				AbTestId = model.AbTestId,
				AbTestTitle = model.AbTestTitle,
				PublicStatus = model.PublicStatus,
				ViewCount = (summary.Any() ? summary.Sum(s => s.PvCount) : 0)
					+ (summaryToday.Any() ? summaryToday.Sum(s => s.PvCount) : 0),
				DateChanged1 = dateChanged[0],
				DateChanged2 = dateChanged[1],
				Items = model.Items.Select(
					item =>
					{
						var listItem = new AbTestListViewModel.AbTestListItem();
						var page = new LandingPageService().Get(item.PageId);
						listItem.PageId = item.PageId;
						listItem.PageTitle = page.ManagementTitle;
						listItem.DistributionRate = item.DistributionRate;
						return listItem;
					}).ToArray(),
			};

			return result;
		}

		/// <summary>
		/// ABテスト詳細View取得
		/// </summary>
		/// <param name="id">取得対象のID</param>
		/// <returns>ABテスト詳細View</returns>
		public AbTestDetailViewModel GetAbTestDetailViewModel(string id)
		{
			var model = new AbTestService().Get(id);
			var result = new AbTestDetailViewModel
			{
				AbTestId = model.AbTestId,
				AbTestTitle = model.AbTestTitle,
				PublicStatus = model.PublicStatus,
				RangeStartDate = model.PublicStartDatetime.ToString("yyyy/MM/dd"),
				RangeStartTime = model.PublicStartDatetime.ToString("HH:mm"),
				RangeEndDate = model.PublicEndDatetime.HasValue
					? model.PublicEndDatetime.Value.ToString("yyyy/MM/dd")
					: "",
				RangeEndTime = model.PublicEndDatetime.HasValue
					? model.PublicEndDatetime.Value.ToString("HH:mm")
					: ""
			};

			return result;
		}

		/// <summary>
		/// ABテストアイテムビュー取得
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <return>ABテストアイテムビュー</return>
		public AbTestItemViewModel GetAbTestItemViewModel(string abTestId)
		{
			var itemModels = new AbTestService().GetAllItemByAbTestId(abTestId);
			var result = new AbTestItemViewModel
			{
				AbTestId = abTestId,
				Items = itemModels.Select(
					itemModel =>
					{
						var page = new LandingPageService().Get(itemModel.PageId);
						var item = new AbTestItemViewModel.Item
						{
							PageId = page.PageId,
							PageTitle = page.ManagementTitle,
							PageUrl = page.PageFileName,
							DistributionRate = itemModel.DistributionRate,
							DateCreated = page.DateCreated.ToString()
						};
						return item;
					}).ToArray()
			};

			return result;
		}

		/// <summary>
		/// ABテストアイテムビューを生成
		/// </summary>
		/// <param name="items">LPページモデル</param>
		///// <return>ABテストアイテムビュー</return>
		public AbTestItemViewModel GenerateAbTestItemViewModel(LandingPageSearchResultModel[] items)
		{
			var result = new AbTestItemViewModel();
			var list = new List<AbTestItemViewModel.Item>();
			foreach (var item in items)
			{
				var pageId = item.PageId;
				var model = new LandingPageService().Get(pageId);
				var page = new AbTestItemViewModel.Item
				{
					PageId = model.PageId,
					PageTitle = model.ManagementTitle,
				};

				list.Add(page);
			}

			result.Items = list;
			return result;
		}

		/// <summary>
		/// ID、名前検索ヒット件数取得
		/// </summary>
		/// <param name="paramModel">パラムモデル</param>
		/// <returns>総件数</returns>
		public int GetCountOfSearchByIdAndName(LandingPageSearchParamModel paramModel)
		{
			var count = new LandingPageService().GetCountOfSearchByParamModel(paramModel);
			return count;
		}

		/// <summary>
		/// LP検索
		/// </summary>
		/// <param name="paramModel">パラメタモデル</param>
		/// <returns>検索結果</returns>
		public LandingPageSearchResultModel[] LandingPageSearch(LandingPageSearchParamModel paramModel)
		{
			paramModel.OffsetNumber = (paramModel.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;
			paramModel.FetchNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;

			var totalCount = new LandingPageService().GetCountOfSearchByParamModel(paramModel);
			var beginRowNumber = paramModel.OffsetNumber + 1;
			var endRowNumber = paramModel.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;
			var countHtml = beginRowNumber
				+ "-"
				+ ((totalCount > endRowNumber) ? StringUtility.ToNumeric(endRowNumber) : StringUtility.ToNumeric(totalCount))
				+ "/" + StringUtility.ToNumeric(totalCount);

			var landingPageList = new LandingPageService().SearchByParamModel(paramModel);
			var result = landingPageList.Select(
				designModel => new LandingPageSearchResultModel()
				{
					PageId = designModel.PageId,
					PageTitle = designModel.ManagementTitle,
					PageUrl = designModel.PageFileName,
					DateCreated = designModel.DateCreated.ToString(),
					CountHtml = countHtml
				}).ToArray();
			return result;
		}

		/// <summary>
		/// ABテスト登録
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns> チェックメッセージ/空</returns>
		/// <remarks>デザインはデフォルトのものを登録</remarks>
		public string RegisterAbTestData(AbTestInput input)
		{
			var errorMessage = input.Validate(true);
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;

			new AbTestService().Insert(model);
			input.AbTestId = model.AbTestId;

			this.SessionWrapper.LandingPageId = string.Empty;

			return string.Empty;
		}

		/// <summary>
		/// ABテスト更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns> チェックメッセージ/空</returns>
		public string UpdateAbTestData(AbTestInput input)
		{
			var errorMessage = input.Validate(false);
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;

			new AbTestService().Update(model);

			return string.Empty;
		}

		/// <summary>
		/// ABテスト削除
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>影響を受けたABテストの件数</returns>
		public int Delete(string abTestId)
		{
			var result = new AbTestService().Delete(abTestId);

			return result;
		}

		/// <summary>
		/// ABテスト分析レポートモデル取得
		/// </summary>
		/// <param name="abtestId">ABテストID</param>
		/// <param name="startDate">開始日</param>
		/// <param name="endDate">終了日</param>
		/// <returns>ABテスト分析レポートビューモデル</returns>
		public AnalysisReportViewModel GetForAbTestAnalysisReport(string abtestId, DateTime? startDate = null, DateTime? endDate = null)
		{
			var abTestService = new AbTestService();

			var abTestModel = abTestService.Get(abtestId);
			if (abTestModel == null) return null;
			var abtestItemModels = abTestService.GetAllItemByAbTestId(abtestId);

			var adjustedStartDate = DateTime.Now;
			var adjustedEndDate = DateTime.Now;
			SetStartDateAndEndDate(abTestModel, startDate, endDate, ref adjustedStartDate, ref adjustedEndDate);

			var contentsIds = abtestItemModels.Select(model => model.AbTestId + "-" + model.PageId).ToArray();
			var contentsLogService = new ContentsLogService();
			var contentsSummaryAnalysisModels = contentsLogService.GetForAnalysisReport(adjustedStartDate, adjustedEndDate, Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_ABTEST, contentsIds);
			if (adjustedEndDate >= DateTime.Today)
			{
				var contentsSummaryAnalysisModelsToday = contentsLogService.GetForAnalysisReportOfToday(Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_ABTEST, contentsIds);
				concatSummaryAnalysisModel(ref contentsSummaryAnalysisModels, contentsSummaryAnalysisModelsToday);
			}

			var landingPageService = new LandingPageService();
			var lpPages = abtestItemModels.ToDictionary(
				model => model.PageId,
				model => landingPageService.Get(model.PageId).ManagementTitle);

			var analysisReportViewModel = new AnalysisReportViewModel
			{
				PageId = abtestId,
				PageName = abTestModel.AbTestTitle,
				StartDate = adjustedStartDate.ToString("yyyy/MM/dd"),
				EndDate = adjustedEndDate.ToString("yyyy/MM/dd"),
				AnalysisReportType = Constants.FLG_CONTENTSLOG_REPORT_TYPE_PV,
				TypeItemNames = new Dictionary<string, string>()
				{
					{ Constants.FLG_CONTENTSLOG_REPORT_TYPE_PV, "ページビュー数" },
					{ Constants.FLG_CONTENTSLOG_REPORT_TYPE_ODREXCCV, "コンバージョン数（注文実行時点）" },
					{ Constants.FLG_CONTENTSLOG_REPORT_TYPE_ODRCOMPCV, "コンバージョン数（注文完了時点）" },
					{ Constants.FLG_CONTENTSLOG_REPORT_TYPE_ODREXCCV + "R", "CVR（注文実行時点）" },
					{ Constants.FLG_CONTENTSLOG_REPORT_TYPE_ODRCOMPCV + "R", "CVR（注文完了時点）" }
				}
			};

			SetTypeItems(ref analysisReportViewModel, lpPages, analysisReportViewModel.TypeItemNames.Keys);
			SetDailyItems(abtestId,ref analysisReportViewModel,contentsSummaryAnalysisModels);
			FillUpDailyItems(ref analysisReportViewModel, adjustedStartDate, adjustedEndDate);
			SetChartData(ref analysisReportViewModel);

			return analysisReportViewModel;
		}

		/// <summary>
		/// 対象期間設定
		/// </summary>
		/// <param name="abTestModel">ABテストモデル</param>
		/// <param name="startDate">指定開始日</param>
		/// <param name="endDate">指定終了日</param>
		/// <param name="adjustedStartDate">開始日</param>
		/// <param name="adjustedEndDate">終了日</param>
		private void SetStartDateAndEndDate(AbTestModel abTestModel, DateTime? startDate, DateTime? endDate, ref DateTime adjustedStartDate, ref DateTime adjustedEndDate)
		{
			var publicStartDate = abTestModel.PublicStartDatetime.Date;
			var publicEndDate = (abTestModel.PublicEndDatetime != null)
				? abTestModel.PublicEndDatetime.Value.Date
				: DateTime.Today;

			if ((startDate == null) && (endDate == null))
			{
				// 公開終了日と今日を比較してより早い日付を設定
				adjustedEndDate = (publicEndDate < DateTime.Today)
					? publicEndDate
					: DateTime.Today;
				// 終了日から１か月前と登録されている開始日を比較して
				// 終了日に近い日付を設定（開始日が未来の時は終了日から1か月前）
				adjustedStartDate = ((adjustedEndDate.Date.AddMonths(-1) < publicStartDate)
					&& (publicStartDate <= DateTime.Today))
					? publicStartDate
					: adjustedEndDate.Date.AddMonths(-1);
				return;
			}
			if ((startDate != null) && (endDate == null))
			{
				// "指定開始日"を基準にレポート表示期間を設定
				adjustedStartDate = startDate.Value.Date;
				if (DateTime.Today <= adjustedStartDate)
				{
					adjustedEndDate = adjustedStartDate;
					return;
				}

				if (publicEndDate < DateTime.Today)
				{
					adjustedEndDate = (adjustedStartDate.AddMonths(1) < publicEndDate)
						? adjustedStartDate.AddMonths(1)
						: publicEndDate;
				}
				else
				{
					adjustedEndDate = (adjustedStartDate.AddMonths(1) < DateTime.Today)
						? adjustedStartDate.AddMonths(1)
						: DateTime.Today;
				}
				return;
			}
			if (startDate == null)
			{
				// "指定終了日"を基準にレポート表示期間を設定
				adjustedEndDate = endDate.Value.Date;
				adjustedStartDate = ((adjustedEndDate.AddMonths(-1) < publicStartDate)
					&& (publicStartDate <= adjustedEndDate))
					? publicStartDate
					: adjustedEndDate.AddMonths(-1);
				return;
			}

			// "指定開始日""指定終了日"を基準にレポート表示期間を設定
			adjustedStartDate = startDate.Value.Date;
			if (adjustedStartDate <= endDate.Value.Date)
			{
				adjustedEndDate = endDate.Value.Date;
				return;
			}
			if (DateTime.Today < adjustedStartDate)
			{
				adjustedEndDate = adjustedStartDate;
				return;
			}
			adjustedEndDate = (publicEndDate < DateTime.Today)
				? publicEndDate
				: DateTime.Today;
		}

		/// <summary>
		/// コンテンツ解析モデル結合
		/// </summary>
		/// <param name="contentsSummaryAnalysisModels">コンテンツ解析モデル</param>
		/// <param name="contentsSummaryAnalysisModelsToday">コンテンツ解析モデル（本日分）</param>
		private void concatSummaryAnalysisModel(
			ref ContentsSummaryAnalysisModel[] contentsSummaryAnalysisModels,
			ContentsSummaryAnalysisModel[] contentsSummaryAnalysisModelsToday)
		{
			Array.Resize(ref contentsSummaryAnalysisModels, contentsSummaryAnalysisModels.Length + contentsSummaryAnalysisModelsToday.Length);
			contentsSummaryAnalysisModelsToday.CopyTo(contentsSummaryAnalysisModels, contentsSummaryAnalysisModels.Length - contentsSummaryAnalysisModelsToday.Length);
		}

		/// <summary>
		/// 分析レポート種別モデル設定
		/// </summary>
		/// <param name="analysisReportViewModel">分析レポートビューモデル</param>
		/// <param name="lpPages">Lpページリスト</param>
		/// <param name="reportTypes">レポート種別リスト</param>
		private void SetTypeItems(ref AnalysisReportViewModel analysisReportViewModel, Dictionary<string,string> lpPages, IEnumerable<string> reportTypes)
		{
			foreach (var type in reportTypes)
			{
				var analysisReportTypeViewModel = new AnalysisReportTypeViewModel
				{
					IsSelected = (type == Constants.FLG_CONTENTSLOG_REPORT_TYPE_PV),
					DetailItemNames = lpPages
				};
				analysisReportViewModel.TypeItems.Add(type, analysisReportTypeViewModel);

				SetDetailItems(ref analysisReportTypeViewModel, lpPages.Keys);
			}
		}

		/// <summary>
		/// 分析レポート詳細モデル設定
		/// </summary>
		/// <param name="analysisReportTypeViewModel">分析レポートビューモデル</param>
		/// <param name="lpPageIds">LpページIDリスト</param>
		private void SetDetailItems(ref AnalysisReportTypeViewModel analysisReportTypeViewModel, IEnumerable<string> lpPageIds)
		{
			foreach (var lpPageId in lpPageIds)
			{
				analysisReportTypeViewModel.DetailItems.Add(lpPageId, new AnalysisReportDetailViewModel());
			}
		}

		/// <summary>
		/// 分析レポート日毎データ設定
		/// </summary>
		/// <param name="abtestId">ABテストID</param>
		/// <param name="analysisReportViewModel">分析レポートビューモデル</param>
		/// <param name="contentsSummaryAnalysisModels">コンテンツ解析モデル</param>
		private void SetDailyItems(string abtestId,
			ref AnalysisReportViewModel analysisReportViewModel,
			ContentsSummaryAnalysisModel[] contentsSummaryAnalysisModels)
		{
			foreach (var model in contentsSummaryAnalysisModels)
			{
				if ((model.ReportType == Constants.FLG_CONTENTSLOG_REPORT_TYPE_ODREXCCV)
					|| (model.ReportType == Constants.FLG_CONTENTSLOG_REPORT_TYPE_ODRCOMPCV))
				{
					long pv;
					//ODREXCCV,ODRCOMPCVならODREXCCVR,ODRCOMPCVRを計算、挿入する。
					if (TryGetPv(contentsSummaryAnalysisModels, model.Date, out pv))
					{
						var cvr = (double)(model.Count) * 100 / pv;
						var rateDetailModel = analysisReportViewModel.TypeItems[model.ReportType + "R"]
							.DetailItems[model.ContentsId.Substring(abtestId.Length + 1)];
						rateDetailModel.DailyItems[model.Date.ToString()] = cvr.ToString("##.00");
					}
				}

				var detailModel = analysisReportViewModel.TypeItems[model.ReportType]
					.DetailItems[model.ContentsId.Substring(abtestId.Length + 1)];
				detailModel.DailyItems[model.Date.ToString()] = model.Count.ToString();
			}
		}

		/// <summary>
		/// 分析レポートの日毎データの空きを「0」で埋める
		/// </summary>
		/// <param name="analysisReportViewModel">分析レポートビューモデル</param>
		/// <param name="startDate">開始日</param>
		/// <param name="endDate">終了日</param>
		private void FillUpDailyItems(ref AnalysisReportViewModel analysisReportViewModel,DateTime startDate,DateTime endDate)
		{
			for (var day = startDate; day <= endDate; day = day.AddDays(1))
			{
				foreach (var typeItem in analysisReportViewModel.TypeItems)
				{
					var analysisReportTypeViewModel = typeItem.Value;
					foreach (var detailItem in analysisReportTypeViewModel.DetailItems)
					{
						var analysisReportDetailViewModel = detailItem.Value;
						if(analysisReportDetailViewModel.DailyItems.ContainsKey(day.ToString()) == false)
							analysisReportDetailViewModel.DailyItems.Add(day.ToString(), "0");
					}
				}
			}
		}

		/// <summary>
		/// 分析チャート用データ設定
		/// </summary>
		/// <param name="analysisReportViewModel">分析レポートモデル</param>
		private void SetChartData(ref AnalysisReportViewModel analysisReportViewModel)
		{
			// 分析レポート詳細毎にラベルの色を作成
			var rgbList = GenerateRgbList(analysisReportViewModel.TypeItems.FirstOrDefault().Value.DetailItems.Count);

			// 分析チャート用データ設定
			foreach (var typeItem in analysisReportViewModel.TypeItems)
			{
				var analysisReportTypeViewModel = typeItem.Value;
				var reportChart = new AnalysisChartViewModel();

				// チャートのラベル作成（日付）
				var labels = analysisReportTypeViewModel.DetailItems.FirstOrDefault().Value.DailyItems.Select(
					dailyItem =>
					{
						var date = DateTime.Parse(dailyItem.Key);
						return date.ToString("M/d");
					});

				foreach (var detailItem in analysisReportTypeViewModel.DetailItems.Select((item, index) => new { item, index }))
				{
					var detail = detailItem.item;
					var index = detailItem.index;
					var chartData = new List<float>();
					foreach (var dailyItem in detail.Value.DailyItems)
					{
						// 日付ごとのデータ格納
						float dailyValue;
						float.TryParse(dailyItem.Value, out dailyValue);
						chartData.Add(dailyValue);
					}

					// チャート用パラメータ格納
					var color = rgbList[index];
					reportChart.Datasets.Add(new AnalysisChartDetailViewModel()
					{
						Label = analysisReportTypeViewModel.DetailItemNames[detail.Key],
						ChartData = chartData,
						Tension = 0,
						Fill = false,
						BackgroundColor = string.Format("rgba({0},{1},{2},0.2)", color[0], color[1], color[2]),
						BorderColor = string.Format("rgba({0},{1},{2},1)", color[0], color[1], color[2]),
						BorderWidth = 2
					});
				}
				reportChart.Labels = labels.ToList();

				analysisReportTypeViewModel.ReportCharts = reportChart;
			}
		}

		/// <summary>
		/// PV数を取得
		/// </summary>
		/// <param name="models">モデル</param>
		/// <param name="date">対象日</param>
		/// <param name="pv">取得したPV数</param>
		/// <returns>成否</returns>
		private bool TryGetPv(IEnumerable<ContentsSummaryAnalysisModel> models, DateTime? date, out long pv)
		{
			var pvModel = models.FirstOrDefault(model => (model.ReportType == Constants.FLG_CONTENTSLOG_REPORT_TYPE_PV) && (model.Date == date));
			if ((pvModel == null) || (pvModel.Count == 0))
			{
				pv = 0;
				return false;
			}
			pv = pvModel.Count;
			return true;
		}

		/// <summary>
		/// RGBのリスト生成
		/// </summary>
		/// <param name="number">色数</param>
		/// <returns>RGBリスト</returns>
		private List<int[]> GenerateRgbList(int number)
		{
			const int CIRCLE = 360;
			const int MAX_COLOR_STRENGTH = 255;
			const int SATURATION = 150;
			const int BRIGHTNESS = 170;

			var rgbList = new List<int[]>();
			for (var numberOfColor = 0; numberOfColor < number; numberOfColor++)
			{
				var hue = (CIRCLE * numberOfColor) / number;
				var rgb = new int[3];

				for (var primeColorNumber = 0; primeColorNumber < rgb.Length; primeColorNumber++)
				{
					//色相の値から各原色の強さを割り出す計算
					if ((0 <= hue) && (hue <= CIRCLE/2))
					{
						rgb[primeColorNumber] = MAX_COLOR_STRENGTH - MAX_COLOR_STRENGTH / 60 * (hue - 60);
					}
					else
					{
						rgb[primeColorNumber] = MAX_COLOR_STRENGTH / 60 * (hue - 240);
					}
					rgb[primeColorNumber] = BringIntoRange(rgb[primeColorNumber], MAX_COLOR_STRENGTH);

					//明度、彩度を適応
					rgb[primeColorNumber] =
						(MAX_COLOR_STRENGTH / 2 + (rgb[primeColorNumber] - MAX_COLOR_STRENGTH / 2) * SATURATION
							/ MAX_COLOR_STRENGTH) + BRIGHTNESS * 2 - MAX_COLOR_STRENGTH;

					rgb[primeColorNumber] = BringIntoRange(rgb[primeColorNumber], MAX_COLOR_STRENGTH);

					//Red,Blue,Greenの切り替え
					hue += CIRCLE/3;

					//切り替えによってhue(色相)が色相環を一周以上回ったらマイナス一周する
					if (hue > CIRCLE) hue -= CIRCLE;
				}
				rgbList.Add(rgb);
			}

			return rgbList;
		}

		/// <summary>
		/// 数値を範囲に収める
		/// </summary>
		/// <param name="target">対象の値</param>
		/// <param name="max">最大値</param>
		/// <param name="min">最小値</param>
		/// <returns>切り揃えた値</returns>
		private int BringIntoRange(int target, int max, int min = 0)
		{
			if (target < min) target = 0;
			else if (target > max) target = max;
			return target;
		}
	}
}
