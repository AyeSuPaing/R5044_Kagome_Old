/*
=========================================================================================================
  Module      :ABテストコントローラー(AbTestController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using w2.App.Common.LandingPage;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.AbTest;
using w2.Cms.Manager.ViewModels.AbTest;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;
using w2.Domain.AbTest;
using w2.Domain.LandingPage.Helper;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// ABテストコントローラー
	/// </summary>
	public class AbTestController : BaseController
	{
		/// <summary>
		/// メイン
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult Main()
		{
			return View();
		}

		/// <summary>
		/// 一覧
		/// </summary>
		/// <param name="param">パラメタモデル</param>
		/// <param name="abtestId">ABテストID</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(AbTestListParamModel param, string abtestId)
		{
			var viewModel = this.Service.GetListView(param);
			if (string.IsNullOrEmpty(abtestId) == false)
			{
				viewModel.OpenDetailAbTestId = abtestId;
			}
			return View(viewModel);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="abTestId">削除対象のABテストID</param>
		/// <returns>アクション結果</returns>
		public ActionResult Delete(string abTestId)
		{
			var number = this.Service.Delete(abTestId);
			var data = new Dictionary<string, string>
			{
				{ "result", (number > 0) ? "ok" : "ng" },
				{ "id", abTestId }
			};
			return Json(data);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>アクション結果</returns>
		public ActionResult Update(AbTestInput input)
		{
			Dictionary<string, string> data;
			var errorMsg = this.Service.UpdateAbTestData(input);
			if (string.IsNullOrEmpty(errorMsg) == false)
			{
				data = new Dictionary<string, string>
				{
					{ "result", "ng" },
					{ "msg", errorMsg },
					{ "id", input.AbTestId }
				};
				return Json(data);
			}

			data = new Dictionary<string, string>
				{
					{ "result", "ok" },
					{ "msg", "" },
					{ "id", input.AbTestId }
				};
			return Json(data);
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>アクション結果</returns>
		public ActionResult Register(AbTestInput input)
		{
			Dictionary<string, string> data;
			var errorMsg = this.Service.RegisterAbTestData(input);
			if (string.IsNullOrEmpty(errorMsg) == false)
			{
				data = new Dictionary<string, string>
				{
					{ "result", "ng" },
					{ "msg", errorMsg },
					{ "id", input.AbTestId }
				};
				return Json(data);
			}

			data = new Dictionary<string, string>
			{
				{ "result", "ok" },
				{ "msg", "" },
				{ "id", input.AbTestId }
			};
			return Json(data);
		}

		/// <summary>
		/// ABテスト詳細ビュー取得
		/// </summary>
		/// <param name="abTestId">対象のABテストID</param>
		/// <returns>アクション結果</returns>
		public ActionResult GetAbTestDetailViewModel(string abTestId)
		{
			var data = this.Service.GetAbTestDetailViewModel(abTestId);
			return Json(data);
		}

		/// <summary>
		/// ABテスト詳細デフォルトビュー取得
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult GetAbTestDefaultDetailViewModel()
		{
			return Json(new AbTestDetailViewModel());
		}

		/// <summary>
		/// ABテストアイテムビュー取得
		/// </summary>
		/// <param name="abTestId">対象のABテストID</param>
		/// <returns>アクション結果</returns>
		public ActionResult GetAbTestItemViewModel(string abTestId)
		{
			var data = this.Service.GetAbTestItemViewModel(abTestId);
			return Json(data);
		}

		/// <summary>
		/// ABテストアイテムビューを生成
		/// </summary>
		/// <param name="items">LPページモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult GenerateAbTestItemViewModel(LandingPageSearchResultModel[] items)
		{
			var data = this.Service.GenerateAbTestItemViewModel(items);
			return Json(data);
		}

		/// <summary>
		/// ID、名前検索ヒット件数取得
		/// </summary>
		/// <param name="paramModel">検索条件</param>
		/// <returns>結果</returns>
		public int GetCountOfSearchByIdAndName(LandingPageSearchParamModel paramModel)
		{
			var count = this.Service.GetCountOfSearchByIdAndName(paramModel);
			return count;
		}

		/// <summary>
		/// 絞り込み検索
		/// </summary>
		/// <param name="paramModel">検索条件</param>
		/// <returns>結果</returns>
		public ActionResult LandingPageSearch(LandingPageSearchParamModel paramModel)
		{
			var vm = this.Service.LandingPageSearch(paramModel);
			return PartialView("LandingPageSearchResult", vm);
		}

		/// <summary>
		/// ページリストビュー取得
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult GetAbTestListViewModel(AbTestListParamModel pm)
		{
			var vm = this.Service.GetListView(pm);
			return Json(vm);
		}

		/// <summary>
		/// メンテナンスツールを有効化
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult EnableToMaintenanceTool()
		{
			LpDesignHelper.EnableToMaintenaceTool();
			var data = new Dictionary<string, string>
			{
				{ "result", "ok" }
			};
			return Json(data);
		}

		/// <summary>
		/// ページがあるか
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		public ActionResult ExistPage(string pageId)
		{
			var flag = (new AbTestService().Get(pageId) != null);
			var data = new Dictionary<string, string>
			{
				{ "result", flag ? "true" : "false" }
			};
			return Json(data);
		}

		/// <summary>
		/// ABテスト分析レポート表示(初期表示)
		/// </summary>
		/// <param name="abtestId">ABテストID</param>
		/// <returns>アクション結果</returns>
		public ActionResult GetDefaultAnalysisActionResult(string abtestId)
		{
			var data = new AbTestWorkerService().GetForAbTestAnalysisReport(abtestId);
			return PartialView("AbTestAnalysisReportModal", data);
		}

		/// <summary>
		/// ABテスト分析レポートモデルをJSONで取得
		/// </summary>
		/// <param name="abtestId">ABテストID</param>
		/// <param name="startDate">開始日</param>
		/// <param name="endDate">終了日</param>
		/// <returns></returns>
		public ActionResult GetAnalysisActionResult(string abtestId, string startDate, string endDate)
		{
			DateTime start;
			DateTime end;
			var targetStartDate = DateTime.TryParse(startDate, out start) ? (DateTime?)start : null;
			var targetEndDate = DateTime.TryParse(endDate, out end) ? (DateTime?)end : null;
			var viewModel = new AbTestWorkerService().GetForAbTestAnalysisReport(abtestId, targetStartDate, targetEndDate);
			var serializedVm = JsonConvert.SerializeObject(viewModel);
			var json = new ContentResult { Content = serializedVm, ContentType = "application/json" };
			return json;
		}

		/// <summary>
		/// CSVダウンロード
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <param name="strStartDate">開始日</param>
		/// <param name="strEndDate">終了日</param>
		public void DownloadCsv(string abTestId,string strStartDate,string strEndDate)
		{
			var startDate = DateTime.Parse(strStartDate.TrimEnd('/'));
			var endDate = DateTime.Parse(strEndDate.TrimEnd('/'));

			var model = new AbTestWorkerService().GetForAbTestAnalysisReport(abTestId, startDate, endDate);

			var fileName = "AnalysisReportList_" + model.PageId + "_" + startDate.ToString("yyyyMMdd") + "_" + endDate.ToString("yyyyMMdd");

			var sbRecords = new StringBuilder();

			sbRecords.Append(CreateRecordCsv(new List<string> { "ABテストID:","", model.PageId }));
			sbRecords.Append(CreateRecordCsv(new List<string> { "ABテスト管理名:","", model.PageName }));
			sbRecords.Append(CreateRecordCsv(new List<string> { "表示期間:","",startDate.ToString("yyyy年M月d日") + " ～ " + endDate.ToString("yyyy年M月d日") }));
			sbRecords.Append(CreateRecordCsv(new List<string>()));

			foreach (var typeItem in model.TypeItems)
			{
				sbRecords.Append(
					CreateRecordCsv(
						new List<string>
						{
							model.TypeItemNames[typeItem.Key]
						}));

				var headerList = new List<string>
				{
					string.Empty
				};
				for (var day = startDate; day <= endDate; day = day.AddDays(1))
				{
					headerList.Add(string.Format("{0}/{1}", day.Month, day.Day));
				}

				sbRecords.Append(CreateRecordCsv(headerList));

				foreach (var detailItem in typeItem.Value.DetailItems)
				{
					var dailyList = new List<string>
					{
						typeItem.Value.DetailItemNames[detailItem.Key]
					};

					foreach (var dailyItem in detailItem.Value.DailyItems)
					{
						dailyList.Add(dailyItem.Value);
					}
					sbRecords.Append(CreateRecordCsv(dailyList));

				}
				sbRecords.Append(CreateRecordCsv(new List<string>()));
			}

			OutPutFileCsv(fileName, sbRecords.ToString());
		}

		/// <summary>
		/// CSVレコード作成
		/// </summary>
		/// <param name="lParams">値リスト</param>
		private string CreateRecordCsv(IEnumerable<string> lParams)
		{
			return string.Join(",", lParams.Select(str => StringUtility.EscapeCsvColumn(str))) + Environment.NewLine;
		}

		/// <summary>
		/// CSVファイル情報出力
		/// </summary>
		/// <param name="strFileName">ファイル名</param>
		/// <param name="strOutPutInfo">出力内容</param>
		private void OutPutFileCsv(string strFileName, string strOutPutInfo)
		{
			Response.ContentEncoding = Encoding.GetEncoding("Shift_JIS");
			Response.ContentType = "application/csv";
			Response.AppendHeader("Content-Disposition", "attachment; filename=" + strFileName + ".csv");
			Response.Write(strOutPutInfo);
			Response.End();
		}

		/// <summary>サービス</summary>
		private AbTestWorkerService Service
		{
			get { return GetDefaultService<AbTestWorkerService>(); }
		}
	}
}
