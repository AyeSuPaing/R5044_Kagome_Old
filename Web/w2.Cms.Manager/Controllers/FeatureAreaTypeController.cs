/*
=========================================================================================================
  Module      : 特集エリアタイプコントローラ(FeatureAreaTypeController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeatureAreaType;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 特集エリアタイプコントローラ
	/// </summary>
	[ValidateInput(false)]
	public class FeatureAreaTypeController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult Main(FeatureAreaTypeListParamModel pm)
		{
			var vm = this.Service.CreateListVm(pm);
			return View(vm);
		}

		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult GetList(FeatureAreaTypeListParamModel pm)
		{
			var vm = this.Service.CreateListVm(pm);
			return PartialView("_FeatureAreaTypeList",vm);
		}

		/// <summary>
		/// 詳細画面
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <param name="isUpdate">更新か</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult FeatureAreaTypeDetail(string areaTypeId, bool isUpdate = false)
		{
			var vm = this.Service.CreateDetailVm(areaTypeId);
			vm.ActionStatus = isUpdate ? ActionStatus.Update : ActionStatus.Insert;
			return PartialView("_FeatureAreaTypeDetail", vm);
		}

		/// <summary>
		/// 詳細画面更新
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="input">特集エリアタイプ</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult UpdateDetailPage(ActionStatus actionStatus, FeatureAreaTypeInput input)
		{
			var errorMessage = input.Validate(actionStatus == ActionStatus.Update);
			if (string.IsNullOrEmpty(errorMessage) == false) return Json(errorMessage, JsonRequestBehavior.AllowGet);

			if (actionStatus == ActionStatus.Insert) this.Service.InsertFeatureAreaType(input);
			if (actionStatus == ActionStatus.Update) this.Service.UpdateFeatureAreaType(input);

			return null;
		}

		/// <summary>
		/// 詳細画面更新
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult DeleteDetailPage(string areaTypeId)
		{
			var errorMessage = this.Service.DeleteFeatureAreaType(areaTypeId);
			return string.IsNullOrEmpty(errorMessage) ? null : Json(errorMessage, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 詳細画面
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		///	<param name="isPc">対象はPCか</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult FeatureAreaTypeDetailContents(string areaTypeId, bool isPc)
		{
			var vm = this.Service.CreateDetailVm(areaTypeId);
			vm.IsPc = isPc;
			return PartialView("_FeatureAreaTypeDetailContents", vm);
		}

		/// <summary>
		/// プレビュー生成 PC用
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult PreviewPc(FeatureAreaTypeInput input)
		{
			// プレビュー画面生成 URL返却
			var url = this.Service.Preview(input, DesignCommon.DeviceType.Pc);
			return Json(url, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// プレビュー生成 PC用
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult PreviewSp(FeatureAreaTypeInput input)
		{
			// プレビュー画面生成 URL返却
			var url = this.Service.Preview(input, DesignCommon.DeviceType.Sp);
			return Json(url, JsonRequestBehavior.AllowGet);
		}

		/// <summary>サービス</summary>
		private FeatureAreaTypeWorkerService Service { get { return GetDefaultService<FeatureAreaTypeWorkerService>(); } }
	}
}
