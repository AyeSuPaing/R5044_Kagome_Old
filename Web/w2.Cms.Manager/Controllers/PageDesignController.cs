/*
=========================================================================================================
  Module      : ページ管理 コントローラ(PageDesignController.cs)
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
using w2.Cms.Manager.ParamModels.PageDesign;
using w2.Cms.Manager.ViewModels.Error;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.PageDesign;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// ページ管理コントローラ
	/// </summary>
	public class PageDesignController : BaseController
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
		/// ページ詳細画面の取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult MainContentDetail(long pageId)
		{
			if (new PageDesignService().GetPage(pageId) == null)
			{
				return JavaScript("open_page_failed()");
			}
			var vm = this.Service.CreatePageDetailVm(pageId);
			vm.ActionStatus = ActionStatus.Update;
			return PartialView("_MainContentDetail", vm);
		}

		/// <summary>
		/// ページの新規登録画面の取得
		/// </summary>
		/// <param name="pageId">複製元のページID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult MainContentDetailRegister()
		{
			var vm = this.Service.CreatePageDetailRegisterVm();
			vm.ActionStatus = ActionStatus.Insert;
			return PartialView("_MainContentDetail", vm);
		}

		/// <summary>
		/// ページの複製登録画面の取得
		/// </summary>
		/// <param name="pageId">複製元のページID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult MainContentDetailCopy(long pageId)
		{
			var vm = this.Service.CreatePageDetailRegisterVm(pageId);
			vm.ActionStatus = ActionStatus.CopyInsert;
			return PartialView("_MainContentDetail", vm);
		}

		/// <summary>
		/// ページの復元
		/// </summary>
		/// <param name="pageId">復元のページID</param>
		/// <param name="fileName">ファイル名</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult RestoreMainContentDetail(long pageId, string fileName)
		{
			var vm = this.Service.CreatePageDetailVm(pageId, fileName);
			vm.ActionStatus = ActionStatus.Update;
			return PartialView("_MainContentDetail", vm);
		}

		/// <summary>
		/// グループ一覧の取得
		/// </summary>
		/// <param name="paramModel">検索パラメータ</param>
		/// <param name="types">チェックボックス入力内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult GroupList(PageDesignListSearchParamModel paramModel, CheckBoxModel[] types)
		{
			PageDesignUtility.DeletePreviewFile();
			paramModel.Types = types;
			var vm = this.Service.CreateGroupListVm(paramModel);
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
		/// ページのグループ移動
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult GroupMove(long pageId)
		{
			var vm = this.Service.CreateGroupMoveViewModel(pageId);
			return PartialView("_ModalEditGroup", vm); ;
		}

		/// <summary>
		/// ページ詳細 更新
		/// </summary>
		/// <param name="input">更新内容</param>
		/// <returns>エラーメッセージ</returns>
		[HttpPost]
		public ActionResult UpdateDetailPage(PageDesignPageInput input)
		{
			var errorMessage = this.Service.UpdatePage(input);
			return Json(errorMessage, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// ページ詳細 登録
		/// </summary>
		/// <param name="input">更新内容</param>
		/// <returns>エラーメッセージ</returns>
		[HttpPost]
		public ActionResult RegisterDetailPage(PageDesignPageInput input)
		{
			input.IsRegister = true;
			var data = new
			{
				errorMessage = this.Service.UpdatePage(input),
				pageId = input.PageId
			};
			return Json(data, JsonRequestBehavior.AllowGet);
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
		/// グループ削除
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns>アクション結果 なし</returns>
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
		/// ページ順序
		/// </summary>
		/// <param name="groupId">対象グループID</param>
		/// <param name="pageIds">ページ順序</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult UpdatePageSort(long groupId, long[] pageIds)
		{
			this.Service.PageSortUpdate(groupId, pageIds);
			return null;
		}

		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>エラーメッセージ</returns>
		[HttpPost]
		public ActionResult DeletePage(long pageId)
		{
			var errorMessage = this.Service.DeletePage(pageId);
			return Json(errorMessage, JsonRequestBehavior.AllowGet); ;
		}

		/// <summary>
		/// 管理用ページタイトル変更
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="title">タイトル名</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult ManagementTitleEdit(long pageId, string title)
		{
			this.Service.ManagementTitleEdit(pageId, title);
			return null;
		}

		/// <summary>
		/// プレビュー生成 PC用
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult PreviewPc(PageDesignPageInput input)
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
		public ActionResult PreviewSp(PageDesignPageInput input)
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
		/// ページ自動バックアップリスト取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>ページバックアップリスト</returns>
		[HttpGet]
		public ActionResult GetPageBackupList(long pageId)
		{
			var pageBackupList = this.Service.GetPageBackupList(pageId);
			return Json(pageBackupList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// ページ・パーツダウンロード 全デバイス対象
		/// </summary>
		/// <returns>レスポンス Zipファイル</returns>
		public ActionResult DownloadAll()
		{
			this.Service.Download(this.Response);
			return null;
		}

		/// <summary>
		/// ページ・パーツダウンロード PC対象
		/// </summary>
		/// <returns>レスポンス Zipファイル</returns>
		public ActionResult DownloadPc()
		{
			this.Service.Download(this.Response, DesignCommon.DeviceType.Pc);
			return null;
		}

		/// <summary>
		/// ページ・パーツダウンロード SP対象
		/// </summary>
		/// <returns>レスポンス Zipファイル</returns>
		public ActionResult DownloadSp()
		{
			this.Service.Download(this.Response, DesignCommon.DeviceType.Sp);
			return null;
		}

		/// <summary>
		/// ページのグループ移動
		/// </summary>
		/// <param name="groupId">移動先グループID</param>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult GroupMoveEdit(long groupId, long pageId)
		{
			this.Service.GroupMoveEdit(groupId, pageId);
			return null;
		}

		/// <summary>
		/// 整合性バッチの起動
		/// </summary>
		/// <returns>アクション結果 なし</returns>
		[HttpGet]
		public ActionResult PageDesignConsistencyAction()
		{
			this.Service.PageDesignConsistencyAction();
			return null;
		}

		/// <summary>サービス</summary>
		private PageDesignWorkerService Service { get { return GetDefaultService<PageDesignWorkerService>(); } }
	}
}
