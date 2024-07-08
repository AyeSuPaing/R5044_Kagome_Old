/*
=========================================================================================================
  Module      : メニュー権限コントローラ(MenuAuthorityController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// メニュー権限コントローラ
	/// </summary>
	public class MenuAuthorityController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult MenuAuthorityList()
		{
			var vm = this.Service.CreateListVm();
			return View(vm);
		}

		/// <summary>
		/// 詳細確認画面表示
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="menuLevel">メニュー権限レベル</param>
		/// <returns>アクション結果</returns>
		public ActionResult Confirm(ActionStatus actionStatus, int? menuLevel)
		{
			var vm = this.Service.CreateConfirmVm(actionStatus, menuLevel, this.TempData);
			return (vm != null) ? View(vm) : CreateErrorPageAction(WebMessages.InconsistencyError);
		}

		/// <summary>
		/// 登録編集画面表示
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="menuAuthorityLevel">メニュー権限レベル</param>
		/// <returns>アクション結果</returns>
		public ActionResult Register(ActionStatus actionStatus, int? menuAuthorityLevel)
		{
			var vm = this.Service.CreateRegisterVm(actionStatus, menuAuthorityLevel);
			return (vm != null) ? View(vm) : CreateErrorPageAction(WebMessages.InconsistencyError);
		}

		/// <summary>
		/// 編集アクション
		/// </summary>
		/// <returns>アクション結果</returns>
		[ActionName("Confirm")]
		[Button(ButtonName = "Edit")]
		public ActionResult EditOfDetail(int menuAuthorityLevel)
		{
			return RedirectToAction(
				"Register",
				new
				{
					ActionStatus = ActionStatus.Update,
					menuAuthorityLevel
				});
		}

		/// <summary>
		/// 削除アクション
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "Delete")]
		public ActionResult DeleteOfDetail(int menuAuthorityLevel)
		{
			if (this.Service.CheckMenuAuthorityUsed(menuAuthorityLevel))
			{
				return CreateErrorPageAction(WebMessages.MenuauthorityDeleteImpossibleError);
			}

			this.Service.Delete(menuAuthorityLevel);

			return RedirectToAction("MenuAuthorityList");
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
			this.Service.InsertUpdate(actionStatus, this.TempData.MenuAuthority);

			return RedirectToAction("MenuAuthorityList");
		}

		/// <summary>
		/// 登録編集画面アクション
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="input">メニュー権限設定入力</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult Register(ActionStatus actionStatus, MenuAuthorityInput input)
		{
			this.TempData.MenuAuthority = input.CreateModels(
				actionStatus,
				this.SessionWrapper.LoginShopId,
				this.SessionWrapper.LoginOperatorName);
			return RedirectToAction(
				"Confirm",
				new
				{
					ActionStatus = actionStatus,
				});
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="input">メニュー権限設定入力</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public string Validate(MenuAuthorityInput input)
		{
			var actionStatus = (input.IsInsert) ? ActionStatus.Insert : ActionStatus.Update;
			var errorMessage = input.Validate(actionStatus, this.SessionWrapper.LoginShopId);
			return StringUtility.ToEmpty(errorMessage);
		}

		/// <summary>サービス</summary>
		private MenuAuthorityWorkerService Service { get { return GetDefaultService<MenuAuthorityWorkerService>(); } }
	}
}
