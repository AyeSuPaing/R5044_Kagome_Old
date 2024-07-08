/*
=========================================================================================================
  Module      : コーディネートカテゴリコントローラ(CoordinateCategoryController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ParamModels.CoordinateCategory;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// コーディネートカテゴリコントローラ
	/// </summary>
	public class CoordinateCategoryController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult Main(
			ActionStatus? actionStatus,
			CoordinateCategoryParamModel pm)
		{
			var vm = (actionStatus == null)
				? this.Service.CreateCoordinateCategoryVm(ActionStatus.List, this.TempData.CoordinateCategoryModel, pm)
				: this.Service.CreateCoordinateCategoryVm((ActionStatus)actionStatus, this.TempData.CoordinateCategoryModel, pm);

			return View(vm);
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Main")]
		[Button(ButtonName = "Search")]
		public ActionResult Search(CoordinateCategoryParamModel pm)
		{
			return Main(ActionStatus.List, pm);
		}

		/// <summary>
		/// 親登録画面表示
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Main")]
		[Button(ButtonName = "ParentRegistration")]
		public ActionResult ParentRegistration(CoordinateCategoryParamModel pm)
		{
			pm.CoordinateParentCategoryId = Constants.FLG_COORDINATECATEGORY_ROOT;
			return Register(pm);
		}

		/// <summary>
		/// 登録画面表示
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Main")]
		[Button(ButtonName = "Register")]
		public ActionResult Register(CoordinateCategoryParamModel pm)
		{
			this.TempData.CoordinateCategoryModel = null;
			pm.IsUpdate = false;

			return RedirectToAction(
				"Main",
				new
				{
					ActionStatus = ActionStatus.Insert,
					pm.SearchCoordinateCategoryId,
					pm.SearchCoordinateParentCategoryId,
					pm.PagerNo,
					pm.CoordinateParentCategoryId,
					pm.SelectedIndex,
					pm.IsUpdate,
					pm.PageLayout
				});
		}

		/// <summary>
		/// 編集画面表示
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Main")]
		[Button(ButtonName = "Edit")]
		public ActionResult Edit(CoordinateCategoryParamModel pm)
		{
			pm.IsUpdate = true;

			return RedirectToAction(
				"Main",
				new
				{
					ActionStatus = ActionStatus.Update,
					pm.CoordinateCategoryId,
					pm.SearchCoordinateCategoryId,
					pm.SearchCoordinateParentCategoryId,
					pm.PagerNo,
					pm.SelectedIndex,
					pm.IsUpdate,
					pm.PageLayout
				});
		}

		/// <summary>
		/// 確認アクション
		/// </summary>
		/// <param name="input">インプット</param>
		/// <param name="pm">パラムモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Main")]
		[Button(ButtonName = "Confirm")]
		public ActionResult Confirm(CoordinateCategoryInput input, CoordinateCategoryParamModel pm)
		{
			// カテゴリID入力値と親カテゴリをあわせる
			input.CoordinateCategoryId = (input.CoordinateParentCategoryId ==Constants.FLG_COORDINATECATEGORY_ROOT) 
				? input.FormattedCoordinateCategoryId
				: input.CoordinateParentCategoryId + input.FormattedCoordinateCategoryId;

			this.TempData.CoordinateCategoryModel = input.CreateModel();
			return RedirectToAction(
				"Main",
				new
				{
					ActionStatus = ActionStatus.Confirm,
					pm.SearchCoordinateCategoryId,
					pm.SearchCoordinateParentCategoryId,
					pm.PagerNo,
					pm.SelectedIndex,
					pm.IsUpdate,
					pm.PageLayout
				});
		}

		/// <summary>
		/// 入力内容を確認
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="pm">パラムモデル</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(CoordinateCategoryInput input, CoordinateCategoryParamModel pm)
		{
			// カテゴリID入力値と親カテゴリをあわせる
			input.CoordinateCategoryId = (input.CoordinateParentCategoryId == Constants.FLG_COORDINATECATEGORY_ROOT)
				? input.FormattedCoordinateCategoryId
				: input.CoordinateParentCategoryId + input.FormattedCoordinateCategoryId;

			var errorMessage = input.Validate(pm.IsUpdate);
			return StringUtility.ToEmpty(errorMessage);
		}

		/// <summary>
		/// 登録更新アクション
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Main")]
		[Button(ButtonName = "InsertUpdate")]
		public ActionResult InsertUpdate(CoordinateCategoryParamModel pm, ActionStatus actionStatus)
		{
			var model = this.TempData.CoordinateCategoryModel;
			this.Service.InsertUpdate(model, pm.IsUpdate);

			return RedirectToAction("Main",
				new
				{
					ActionStatus = ActionStatus.Detail,
					pm.SearchCoordinateCategoryId,
					pm.SearchCoordinateParentCategoryId,
					pm.PagerNo,
					pm.SelectedIndex,
					model.CoordinateCategoryId,
					pm.PageLayout
				});
		}

		/// <summary>
		/// 削除アクション
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Main")]
		[Button(ButtonName = "Delete")]
		public ActionResult Delete(CoordinateCategoryParamModel pm)
		{
			this.Service.Delete(pm.CoordinateCategoryId);

			return RedirectToAction("Main",
				new
				{
					ActionStatus = ActionStatus.List,
					pm.SearchCoordinateCategoryId,
					pm.SearchCoordinateParentCategoryId,
					pm.PagerNo,
					pm.SelectedIndex,
					pm.PageLayout
				});
		}

		/// <summary>
		/// エクスポート
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Main")]
		[Button(ButtonName = "Export")]
		public ActionResult Export(CoordinateCategoryParamModel pm)
		{
			var fileData = this.Service.Export(pm);

			// エラーがあればエラー画面へ
			if (string.IsNullOrEmpty(fileData.Error) == false)
			{
				return CreateErrorPageAction(fileData.Error);
			}
			var actionResult = fileData.CreateActionResult();
			return actionResult;
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public string ValidateExport(CoordinateCategoryParamModel pm)
		{
			var str = (string.IsNullOrEmpty(pm.DataExportType))
				? WebMessages.MasterexportSettingSettingIdNotSelected
				: string.Empty;
			return str;
		}

		/// <summary>サービス</summary>
		private CoordinateCategoryWorkerService Service { get { return GetDefaultService<CoordinateCategoryWorkerService>(); } }
	}
}
