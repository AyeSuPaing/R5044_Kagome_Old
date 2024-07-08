/*
=========================================================================================================
  Module      : パスワード変更コントローラ(OperatorPasswordChangeController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.App.Common;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// パスワード変更コントローラ
	/// </summary>
	public class OperatorPasswordChangeController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult OperatorPasswordChangeInput()
		{
			var vm = this.Service.CreateInputVm();
			return View(vm);
		}

		/// <summary>
		/// 更新アクション
		/// </summary>
		/// <param name="input">パスワード変更入力</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("OperatorPasswordChangeInput")]
		[Button(ButtonName = "Update")]
		public ActionResult Update(OperatorPasswordChangeInput input)
		{
			this.Service.ChangeOperatorPassword(
				input.ShopId,
				input.OperatorId,
				input.Password,
				this.SessionWrapper.LoginOperator.Name);

			return View("Complete");
		}

		/// <summary>
		/// 更新確認
		/// </summary>
		/// <param name="input">パスワード変更入力</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(OperatorPasswordChangeInput input)
		{
			var errorMessage = input.Validate();
			return StringUtility.ToEmpty(errorMessage);
		}

		/// <summary>サービス</summary>
		private OperatorPasswordChangeWorkerService Service { get { return GetDefaultService<OperatorPasswordChangeWorkerService>(); } }
	}
}
