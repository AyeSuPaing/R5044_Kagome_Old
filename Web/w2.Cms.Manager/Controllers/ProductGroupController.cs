/*
=========================================================================================================
  Module      : 商品グループコントローラー(ProductGroupController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ProductGroup;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 商品グループコントローラー
	/// </summary>
	public class ProductGroupController : BaseController
	{
		/// <summary>ページレイアウト</summary>
		public static string m_pageLayout = Constants.LAYOUT_PATH_DEFAULT;

		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(ProductGroupListParamModel pm)
		{
			pm.Dates = pm.Dates ?? this.TempData.ProductGroupListParamModel.Dates;
			var vm = this.Service.CreateListVm(pm);

			// 検索条件持っておく
			this.TempData.ProductGroupListParamModel = pm;
			return View(vm);
		}

		/// <summary>
		/// 更新・登録
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="productGroupId">商品グループID</param>
		/// <param name="pageLayout">ページレイアウト</param>
		/// <returns>アクション結果</returns>
		public ActionResult Edit(ActionStatus actionStatus, string productGroupId, string pageLayout = Constants.LAYOUT_PATH_DEFAULT)
		{
			m_pageLayout = pageLayout;

			var vm = this.Service.CreateEditVm(actionStatus, productGroupId, pageLayout);
			return View(vm);
		}

		/// <summary>
		/// プレビュー
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult Preview()
		{
			var vm = this.Service.CreatePreviewVm();
			return View(vm);
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="pm">商品グループリストパラメタ</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "Search")]
		public ActionResult Search(ProductGroupListParamModel pm)
		{
			var vm = this.Service.CreateListVm(pm);
			this.TempData.ProductGroupListParamModel = pm;
			return View(vm);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Edit")]
		[Button(ButtonName = "Update")]
		public ActionResult Update(ProductGroupInput input)
		{
			// 登録して更新画面
			var vm = this.Service.Update(input);
			return View(vm);
		}

		/// <summary>
		/// 確認
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string Validate(ProductGroupInput input)
		{
			var error = input.Validate(input.ValidationKbn);
			return StringUtility.ToEmpty(error);
		}

		/// <summary>
		/// 一覧へ戻る
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Edit")]
		[Button(ButtonName = "BackList")]
		public ActionResult Back()
		{
			return RedirectToAction("List", this.TempData.ProductGroupListParamModel);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Edit")]
		[Button(ButtonName = "Delete")]
		public ActionResult BacDelete(ProductGroupInput input)
		{
			// 削除して一覧へ
			this.Service.Delete(input);
			return RedirectToAction("List", this.TempData.ProductGroupListParamModel);
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Edit")]
		[Button(ButtonName = "Register")]
		public ActionResult Register(ProductGroupInput input)
		{
			// 登録して編集画面
			var vm = this.Service.Register(input);
			return View(vm);
		}

		/// <summary>
		/// コピー新規登録
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Edit")]
		[Button(ButtonName = "CopyRegister")]
		public ActionResult CopyRegister(ProductGroupInput input)
		{
			var vm = this.Service.CreateEditVm(ActionStatus.CopyInsert, input.ProductGroupId, m_pageLayout);
			return View(vm);
		}

		/// <summary>
		/// プレビューファイル生成
		/// </summary>
		/// <param name="contents">プレビューコンテンツ</param>
		/// <param name="htmlkbn">HTML区分</param>
		[HttpPost]
		public void CreatePreviewFile(string contents, string htmlkbn)
		{
			this.Service.CreatePreviewFile(contents, htmlkbn);
		}

		/// <summary>サービス</summary>
		private ProductGroupWorkerService Service { get { return GetDefaultService<ProductGroupWorkerService>(); } }
	}
}