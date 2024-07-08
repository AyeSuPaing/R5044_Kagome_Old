/*
=========================================================================================================
  Module      : オペレーター管理コントローラ(ShopOperatorController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ShopOperator;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// オペレーター管理コントローラ
	/// </summary>
	public class ShopOperatorController : BaseController
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
		/// <param name="operatorId">オペレータID（編集の時）</param>
		/// <returns>アクション結果</returns>
		public ActionResult Confirm(ActionStatus actionStatus, string operatorId, string pageLayout = Constants.LAYOUT_PATH_DEFAULT)
		{
			if ((this.TempData.ShopOperator == null)
				&& (actionStatus != ActionStatus.Detail)) return RedirectToAction("List");

			var vm = this.Service.CreateConfirmVm(actionStatus, operatorId, pageLayout, this.TempData);
			return (vm != null) ? View(vm) : CreateErrorPageAction(WebMessages.InconsistencyError);
		}

		/// <summary>
		/// 編集アクション
		/// </summary>
		/// <param name="operatorId">オペレータID（編集の時）</param>
		/// <returns>アクション結果</returns>
		[ActionName("Confirm")]
		[Button(ButtonName = "Edit")]
		public ActionResult EditOfDetail(string operatorId)
		{
			return RedirectToAction(
				"Register",
				new
				{
					ActionStatus = ActionStatus.Update,
					operatorId
				});
		}

		/// <summary>
		/// 削除アクション
		/// </summary>
		/// <param name="operatorId">オペレータID（編集の時）</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "Delete")]
		public ActionResult DeleteOfDetail(string operatorId)
		{
			this.Service.Delete(operatorId);

			return RedirectToAction("List");
		}

		/// <summary>
		/// コピー新規登録アクション
		/// </summary>
		/// <param name="operatorId">オペレータID（編集の時）</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "CopyInsert")]
		public ActionResult CopyInsertOfDetail(string operatorId)
		{
			return RedirectToAction(
				"Register",
				new
				{
					ActionStatus = ActionStatus.Insert,
					operatorId
				});
		}

		/// <summary>
		/// 登録更新アクション
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "InsertUpdate")]
		public ActionResult InsertUpdateOfConfirm(ActionStatus actionStatus, string operatorId)
		{
			this.Service.InsertUpdate(actionStatus, this.TempData.ShopOperator);
			this.TempData.ShopOperator = null;

			return RedirectToAction("List");
		}

		/// <summary>
		/// 登録編集画面表示
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="operatorId">オペレータID（編集の時）</param>
		/// <returns>アクション結果</returns>
		public ActionResult Register(ActionStatus actionStatus, string operatorId)
		{
			var vm = this.Service.CreateRegisterVm(actionStatus, operatorId);
			return (vm != null) ? View(vm) : CreateErrorPageAction(WebMessages.InconsistencyError);
		}

		/// <summary>
		/// 登録編集画面アクション
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="input">入力クラス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult Register(ActionStatus actionStatus, ShopOperatorInput input)
		{
			this.TempData.ShopOperator = input.CreateModel();
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
		public string Validate(ShopOperatorInput input)
		{
			var actionStatus = (input.IsInsert) ? ActionStatus.Insert : ActionStatus.Update;
			var errorMessage = input.Validate(actionStatus);
			return StringUtility.ToEmpty(errorMessage);
		}

		/// <summary>サービス</summary>
		private ShopOperatoWorkerService Service { get { return GetDefaultService<ShopOperatoWorkerService>(); } }
	}
}
