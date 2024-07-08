/*
=========================================================================================================
  Module      : 特集エリアコントローラ(FeatureAreaController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Web.Mvc;
using w2.App.Common.Design;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeatureArea;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 特集エリアコントローラ
	/// </summary>
	[ValidateInput(false)]
	public class FeatureAreaController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult Main(FeatureAreaListParamModel pm)
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
		public ActionResult GetList(FeatureAreaListParamModel pm)
		{
			var vm = this.Service.CreateListVm(pm);
			return PartialView("_FeatureAreaList", vm);
		}

		/// <summary>
		/// 詳細画面
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult FeatureAreaDetail(string areaId, ActionStatus actionStatus)
		{
			var vm = this.Service.CreateDetailVm(areaId);
			if (actionStatus == ActionStatus.CopyInsert) vm.Input.AreaName += Constants.COPY_NEW_SUFFIX;
			vm.ActionStatus = actionStatus;
			return PartialView("_FeatureAreaDetail", vm);
		}

		/// <summary>
		/// 詳細画面更新
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="input">特集エリア</param>
		/// <param name="bannerInputs">特集エリアバナー</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult UpdateDetailPage(ActionStatus actionStatus, FeatureAreaInput input, FeatureAreaBannerInput[] bannerInputs)
		{
			input.BannerInput = bannerInputs ?? new FeatureAreaBannerInput[0];

			var newId = string.Empty;
			switch (actionStatus)
			{
				case ActionStatus.Insert:
				case ActionStatus.CopyInsert:
					// IDを自動採番
					newId = Common.Util.NumberingUtility.CreateKeyId(
						this.SessionWrapper.LoginShopId,
						Constants.NUMBER_KEY_CMS_FEATURE_AREA_ID,
						Constants.CONST_FEATURE_AREA_ID_LENGTH);
					input.AreaId = newId;
					break;

				case ActionStatus.Update:
					newId = input.AreaId;
					break;
			}

			var errorMessage = this.Service.InsertUpdateFeatureArea(input, actionStatus);

			var objList = new List<object>
			{
				newId,
				errorMessage
			};

			return Json(new { data = objList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 特集エリア削除
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult DeleteDetailPage(string areaId)
		{
			var errorMessage = this.Service.DeleteFeatureArea(areaId);

			// フロント系サイトを最新情報へ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.FeatureAreaBanner).CreateUpdateRefreshFile();
			return Json(errorMessage, JsonRequestBehavior.AllowGet); ;
		}

		/// <summary>
		/// 詳細画面
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult FeatureAreaDetailContents(string areaId)
		{
			var vm = this.Service.CreateDetailVm(areaId);
			return PartialView("_FeatureAreaDetail", vm);
		}

		/// <summary>
		/// プレビュー生成 PC用
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <param name="bannerInputs">バナー入力内容</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult Preview(FeatureAreaInput input, FeatureAreaBannerInput[] bannerInputs)
		{
			input.BannerInput = bannerInputs;
			var url = PreviewUrl(input);
			return Json(url, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// プレビュー用URL作成
		/// </summary>
		/// <param name="input">プレビューする特集エリア情報</param>
		/// <param name="deviceType">プレビューするデバイスタイプ</param>
		/// <param name="isThumb">サムネイル出力用か</param>
		/// <param name="typeInput">プレビュー用特集エリアタイプ</param>
		/// <returns>プレビュー用URL</returns>
		public string PreviewUrl(FeatureAreaInput input,
			DesignCommon.DeviceType deviceType = DesignCommon.DeviceType.Pc,
			bool isThumb = false,
			FeatureAreaTypeInput typeInput = null)
		{
			return this.Service.SorBannerAndPreview(input, deviceType, isThumb, typeInput);
		}

		/// <summary>
		/// 更新用コンテンツタグ置換
		/// </summary>
		/// <param name="fileTextAll">テンプレートファイル内容</param>
		/// <param name="areaId">エリアID</param>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>置換後文字列</returns>
		public string ReplaceContentsTagForUpdate(
			string fileTextAll,
			string areaId,
			string areaTypeId,
			DesignCommon.DeviceType deviceType)
		{
			return this.Service.ReplaceContentsTagForUpdate(
				fileTextAll,
				areaId,
				areaTypeId,
				deviceType);
		}

		/// <summary>サービス</summary>
		private FeatureAreaWorkerService Service { get { return GetDefaultService<FeatureAreaWorkerService>(); } }
	}
}
