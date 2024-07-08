/*
=========================================================================================================
  Module      : スタッフコントローラ(StaffController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.Staff;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// スタッフコントローラ
	/// </summary>
	public class StaffController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(ListParamModel pm)
		{
			var vm = this.Service.CreateListVm(pm);
			return View(vm);
		}

		/// <summary>
		/// 詳細確認画面表示
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="staffId">スタッフID</param>
		/// <param name="pageLayout">レイアウト</param>
		/// <returns>アクション結果</returns>
		public ActionResult Confirm(ActionStatus actionStatus, string staffId, string pageLayout = Constants.LAYOUT_PATH_DEFAULT)
		{
			var vm = this.Service.CreateConfirmVm(actionStatus, staffId, pageLayout,this.TempData);
			return View(vm);
		}

		/// <summary>
		/// 登録編集画面表示
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="staffId">スタッフID（編集の時）</param>
		/// <returns>アクション結果</returns>
		public ActionResult Register(ActionStatus actionStatus, string staffId)
		{
			var vm = this.Service.CreateRegisterVm(actionStatus, staffId);
			return View(vm);
		}

		/// <summary>
		/// 登録編集画面アクション
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="input">入力クラス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult Register(ActionStatus actionStatus, StaffInput input)
		{
			this.TempData.Staff = input.CreateModel();
			return RedirectToAction(
				"Confirm",
				new
				{
					ActionStatus = actionStatus
				});
		}

		/// <summary>
		/// 入力チェック
		/// </summary>
		/// <param name="input">入力クラス</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(StaffInput input)
		{
			var actionStatus = (input.IsInsert) ? ActionStatus.Insert : ActionStatus.Update;
			var errorMessage = input.Validate(actionStatus);
			return StringUtility.ToEmpty(errorMessage);
		}

		/// <summary>
		/// 登録更新アクション
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "InsertUpdate")]
		public ActionResult InsertUpdateOfConfirm(ActionStatus actionStatus)
		{
			this.Service.InsertUpdate(actionStatus, this.TempData.Staff);

			return RedirectToAction("List");
		}

		/// <summary>
		/// 削除アクション
		/// </summary>
		/// <param name="staffId">スタッフID（編集の時）</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "Delete")]
		public ActionResult DeleteOfDetail(string staffId)
		{
			this.Service.Delete(staffId);

			return RedirectToAction("List");
		}

		/// <summary>
		/// 編集アクション
		/// </summary>
		/// <param name="staffId">スタッフID（編集の時）</param>
		/// <returns>アクション結果</returns>
		[ActionName("Confirm")]
		[Button(ButtonName = "Edit")]
		public ActionResult EditOfDetail(string staffId)
		{
			return RedirectToAction(
				"Register",
				new
				{
					ActionStatus = ActionStatus.Update,
					staffId
				});
		}

		/// <summary>
		/// コピー新規登録アクション
		/// </summary>
		/// <param name="staffId">スタッフID（編集の時）</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "CopyInsert")]
		public ActionResult CopyInsertOfDetail(string staffId)
		{
			return RedirectToAction(
				"Register",
				new
				{
					ActionStatus = ActionStatus.Insert,
					staffId
				});
		}

		/// <summary>
		/// ファイル存在チェック
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>true: 存在する false:存在しない</returns>
		[HttpPost]
		public ActionResult CheckFileExist(string fileName)
		{
			var isExist = this.Service.CheckFileExist(fileName);
			return Json(isExist, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// アップロード
		/// </summary>
		/// <param name="files">画像</param>
		/// <param name="staffId"></param>
		/// <returns>アクション結果</returns>
		public ActionResult Upload(HttpPostedFileBase files, string staffId)
		{
			var errorMessage = this.Service.Upload(files, staffId);

			return Content(errorMessage);
		}

		/// <summary>
		/// リアル店舗名をセット
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>オペレータリンク</returns>
		public ActionResult SetOperatorName(string operatorId)
		{
			if (string.IsNullOrEmpty(operatorId)) return Content(string.Empty);
			var op = new ShopOperatorService().Get(this.SessionWrapper.LoginShopId, operatorId);
			var name = (op != null) ? op.Name : "";

			return Content("<a href=\"javascript:open_shop_operator(\'" + operatorId + "\')\">" + name + "</a>");
			
		}

		/// <summary>
		/// エクスポート
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "Export")]
		public ActionResult Export(ListParamModel pm)
		{
			var fileData = this.Service.Export(pm);

			// エラーがあればエラー画面へ
			if (string.IsNullOrEmpty(fileData.Error) == false)
			{
				return CreateErrorPageAction(fileData.Error);
			}
			var actionResult =  fileData.CreateActionResult();
			return actionResult;
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public string ValidateExport(ListParamModel pm)
		{
			var str = (string.IsNullOrEmpty(pm.DataExportType))
				? WebMessages.MasterexportSettingSettingIdNotSelected
				: string.Empty;
			return str;
		}

		/// <summary>サービス</summary>
		private StaffWorkerService Service { get { return GetDefaultService<StaffWorkerService>(); } }
	}
}
