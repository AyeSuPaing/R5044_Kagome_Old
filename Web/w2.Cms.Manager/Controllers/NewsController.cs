/*
=========================================================================================================
  Module      : お知らせコントローラー(NewsController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.News;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// お知らせコントローラー
	/// </summary>
	public class NewsController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(NewsListParamModel pm)
		{
			pm.Dates = pm.Dates ?? this.TempData.NewsListParamModel.Dates;
			var vm = this.Service.CreateListVm(pm);

			// 検索条件を一時保持
			this.TempData.NewsListParamModel = pm;
			return View(vm);
		}

		/// <summary>
		/// 登録・編集画面
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="newsId">お知らせID</param>
		/// <returns>アクション結果</returns>
		public ActionResult Register(ActionStatus actionStatus, string newsId)
		{
			var vm = this.Service.CreateRegisterVm(actionStatus, newsId);
			return View(vm);
		}

		/// <summary>
		/// [一覧表示] Top表示の更新
		/// </summary>
		/// <param name="newsId">お知らせID</param>
		/// <param name="disp">Top表示の更新内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult DispFlgModifyOfList(string newsId, string disp)
		{
			this.Service.DispFlgModify(newsId, disp);

			return null;
		}

		/// <summary>
		/// 更新アクション
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "Update")]
		public ActionResult UpdateOfRegister(NewsInput input)
		{
			input.SetDatetime();
			var vm = this.Service.Update(input);
			return View(vm);
		}

		/// <summary>
		/// 登録アクション
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "Insert")]
		public ActionResult InsertOfRegister(NewsInput input)
		{
			input.SetDatetime();
			var vm = this.Service.Insert(input);
			return View(vm);
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(NewsInput input)
		{
			input.SetDatetime();
			var actionStatus = (input.IsInsert) ? ActionStatus.Insert : ActionStatus.Update;
			var errorMessage = input.Validate(actionStatus);
			return StringUtility.ToEmpty(errorMessage);
		}

		/// <summary>
		/// 削除アクション
		/// </summary>
		/// <param name="newsId">お知らせId</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "Delete")]
		public ActionResult DeleteOfRegister(string newsId)
		{
			this.Service.Delete(newsId);

			// 一覧へ戻る
			return BackList();
		}

		/// <summary>
		/// コピー新規登録アクション
		/// </summary>
		/// <param name="newsId"></param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "CopyInsert")]
		public ActionResult CopyInsertOfRegister(string newsId)
		{
			var vm = this.Service.CreateRegisterVm(ActionStatus.Insert, newsId);
			return View(vm);
		}

		/// <summary>
		/// 一覧へ戻るアクション
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "BackList")]
		public ActionResult BackList()
		{
			return RedirectToAction("List", this.TempData.NewsListParamModel);
		}

		/// <summary>サービス</summary>
		private NewsWorkerService Service { get { return GetDefaultService<NewsWorkerService>(); } }
	}
}