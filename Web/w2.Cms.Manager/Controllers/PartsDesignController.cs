/*
=========================================================================================================
  Module      : パーツ管理 コントローラ(PartsDesignController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Cms;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.PartsDesign;
using w2.Cms.Manager.ViewModels.Error;
using w2.Cms.Manager.ViewModels.PartsDesign;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// パーツ管理 コントローラ
	/// </summary>
	public class PartsDesignController : BaseController
	{
		/// <summary>
		/// メイン画面
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult Main()
		{
			var vm = this.Service.CreateMainVm();
			return View(vm);
		}

		/// <summary>
		/// パーツ詳細画面の取得
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult MainContentDetail(long partsId)
		{
			if (new PartsDesignService().GetParts(partsId) == null)
			{
				return JavaScript("open_page_failed()");
			}
			var vm = this.Service.CreatePartsDetailVm(partsId);
			vm.ActionStatus = ActionStatus.Update;
			return PartialView("_MainContentDetail", vm);
		}

		/// <summary>
		/// パーツの新規登録画面の取得
		/// </summary>
		/// <param name="selectStandardParts">選択テンプレート</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult MainContentDetailRegister(string selectStandardParts)
		{
			var vm = this.Service.CreatePartsDetailRegisterVm(selectStandardParts);
			vm.ActionStatus = ActionStatus.Insert;
			return PartialView("_MainContentDetail", vm);
		}

		/// <summary>
		/// パーツの複製画面の取得
		/// </summary>
		/// <param name="partsId">複製元のページID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult MainContentDetailCopy(long partsId)
		{
			var vm = this.Service.CreatePartsDetailRegisterVm(partsId, "");
			vm.ActionStatus = ActionStatus.CopyInsert;
			return PartialView("_MainContentDetail", vm);
		}

		/// <summary>
		/// 管理用パーツタイトル変更
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="title">タイトル名</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult ManagementTitleEdit(long partsId, string title)
		{
			this.Service.ManagementTitleEdit(partsId, title);
			return null;
		}

		/// <summary>
		/// グループ一覧の取得
		/// </summary>
		/// <param name="paramModel">検索パラメータ</param>
		/// <param name="types">チェックボックス入力内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult GroupList(PartsDesignListSearchParamModel paramModel, CheckBoxModel[] types)
		{
			PageDesignUtility.DeletePreviewFile();
			paramModel.Types = types;
			var vm = this.Service.CreateGroupListVm(paramModel, Constants.PAGE_PARTS_MAX_VIEW_CONTENT_COUNT);

			if (string.IsNullOrEmpty(vm.ErrorMessage) == false)
			{
				var errorVm = new IndexViewModel()
				{
					ErrorMessagesHtmlEncoded = vm.ErrorMessage
				};
				return PartialView("_ErrorMessage", errorVm);
			}
			else
			{
				return PartialView("_MainContentList", vm);
			}
		}

		/// <summary>
		/// グループ追加
		/// </summary>
		/// <returns>グループID</returns>
		[HttpPost]
		public ActionResult GroupAdd(string name)
		{
			var groupId = this.Service.GroupAdd(name);
			return Json(groupId, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// グループ名変更
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="name">グループ名</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult GroupNameEdit(long groupId, string name)
		{
			this.Service.GroupNameEdit(groupId, name);
			return null;
		}

		/// <summary>
		/// ページのグループ移動
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult GroupMove(long partsId)
		{
			var vm = this.Service.CreateGroupMoveViewModel(partsId);
			return PartialView("_ModalEditGroup", vm);
			;
		}

		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		[HttpPost]
		public ActionResult GroupDelete(long groupId)
		{
			this.Service.GroupDelete(groupId);
			return null;
		}

		/// <summary>
		/// グループ順序更新
		/// </summary>
		/// <param name="groupIds">グループ順序配列</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult UpdateGroupSort(long[] groupIds)
		{
			this.Service.GroupSortUpdate(groupIds);
			return null;
		}

		/// <summary>
		/// パーツ順序
		/// </summary>
		/// <param name="groupId">対象グループID</param>
		/// <param name="partsIds">パーツ順序</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult UpdatePartsSort(long groupId, long[] partsIds)
		{
			this.Service.PartsSortUpdate(groupId, partsIds);
			return null;
		}

		/// <summary>
		/// パーツ詳細 更新
		/// </summary>
		/// <param name="input">更新内容</param>
		/// <returns>エラーメッセージ</returns>
		[HttpPost]
		public ActionResult UpdateDetailParts(PartsDesignPartsInput input)
		{
			var errorMessage = this.Service.UpdateParts(input);
			return Json(errorMessage, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// パーツ詳細 登録
		/// </summary>
		/// <param name="input">更新内容</param>
		/// <returns>エラーメッセージ</returns>
		[HttpPost]
		public ActionResult RegisterDetailParts(PartsDesignPartsInput input)
		{
			var data = new
			{
				errorMessage = this.Service.UpdateParts(input),
				partsId = input.PartsId
			};
			return Json(data, JsonRequestBehavior.AllowGet);
		}


		/// <summary>
		/// パーツ削除
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>エラーメッセージ</returns>
		[HttpPost]
		public ActionResult DeleteParts(long partsId)
		{
			var errorMessage = this.Service.DeleteParts(partsId);
			return Json(errorMessage, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// プレビュー生成 PC用
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult PreviewPc(PartsDesignPartsInput input)
		{
			// プレビュー画面生成 URL返却
			var url = this.Service.Preview(input, DesignCommon.DeviceType.Pc);
			var errorMessage = WebRequestCheck.Send(url);
			var result = string.IsNullOrEmpty(errorMessage)
				? Json(data: new { url }, JsonRequestBehavior.AllowGet)
				: Json(data: new { errorMessage }, JsonRequestBehavior.AllowGet);
			return result;
			
		}

		/// <summary>
		/// プレビュー生成 SP用
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult PreviewSp(PartsDesignPartsInput input)
		{
			// プレビュー画面生成 URL返却
			var url = this.Service.Preview(input, DesignCommon.DeviceType.Sp);
			var errorMessage = WebRequestCheck.Send(url);
			var result = string.IsNullOrEmpty(errorMessage)
				? Json(data: new { url }, JsonRequestBehavior.AllowGet)
				: Json(data: new { errorMessage }, JsonRequestBehavior.AllowGet);
			return result;
		}

		/// <summary>
		/// パーツの復元
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="fileName">ファイル名</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult RestoreMainContentDetail(long partsId, string fileName)
		{
			var vm = this.Service.CreatePartsDetailVm(partsId, fileName);
			return PartialView("_MainContentDetail", vm);
		}

		/// <summary>
		/// ページ自動バックアップリスト取得
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>ページバックアップリスト</returns>
		[HttpGet]
		public ActionResult GetPartsBackupList(long partsId)
		{
			var pageBackupList = this.Service.GetPageBackupList(partsId);
			return Json(pageBackupList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// パーツのグループ移動
		/// </summary>
		/// <param name="groupId">移動先グループ</param>
		/// <param name="partsId">移動するパーツID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult GroupMoveEdit(long groupId, long partsId)
		{
			this.Service.GroupMoveEdit(groupId, partsId);
			return null;
		}

		#region レイアウト編集内にて利用
		/// <summary>
		/// パーツ表示
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="defaultDisplayDevice">デフォルト表示デバイス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult ModalPartsEdit(long partsId, string defaultDisplayDevice = "PC")
		{
			var vm = this.Service.CreatePartsDetailVm(partsId);
			vm.EditPageStatus = PartsDetailViewModel.EditPage.PageDesign;
			vm.DefaultDisplayDevice = PartsDetailViewModel.DisplayDevice.PC;
			if (defaultDisplayDevice == "SP")
			{
				vm.DefaultDisplayDevice = PartsDetailViewModel.DisplayDevice.SP;
			}
			vm.ActionStatus = ActionStatus.Update;
			return PartialView("_LayoutEditModalPartsEdit", vm);
		}

		/// <summary>
		/// プレビュー生成 PC用
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult PreviewPartsPcByPartsId(long partsId)
		{
			// プレビュー画面生成 URL返却
			var url = this.Service.Preview(partsId, DesignCommon.DeviceType.Pc);
			return Json(url, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// プレビュー生成 SP用
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult PreviewPartsSpByPartsId(long partsId)
		{
			// プレビュー画面生成 URL返却
			var url = this.Service.Preview(partsId, DesignCommon.DeviceType.Sp);
			return Json(url, JsonRequestBehavior.AllowGet);
		}
		#endregion

		/// <summary>サービス</summary>
		private PartsDesignWorkerService Service { get { return GetDefaultService<PartsDesignWorkerService>(); } }
	}
}
