/*
=========================================================================================================
  Module      : 商品ランキングコントローラ(ProductRankingController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ProductRanking;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.DispProductInfo;
using w2.Domain.Product;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 商品ランキングコントローラ
	/// </summary>
	public class ProductRankingController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(ProductRankingListParamModel pm)
		{
			var vm = this.Service.CreateListVm(pm);
			this.TempData.ProductRankingListParamModel = pm;
			return View(vm);
		}

		/// <summary>
		/// 詳細確認画面表示
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="rankingId">ランキングID（編集の時）</param>
		/// <returns>アクション結果</returns>
		public ActionResult Confirm(ActionStatus actionStatus, string rankingId)
		{
			var vm = this.Service.CreateConfirmVm(actionStatus, rankingId, this.TempData);
			return View(vm);
		}

		/// <summary>
		/// 編集画面
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>アクション結果</returns>
		public ActionResult Register(ActionStatus actionStatus, string rankingId)
		{
			var vm = this.Service.CreateRegisterVm(actionStatus, rankingId, this.TempData);
			return View(vm);
		}

		/// <summary>
		/// 登録画面
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>アクション結果</returns>
		[ActionName("Register")]
		[Button(ButtonName = "Regist")]
		public ActionResult Regist(ActionStatus actionStatus, string rankingId)
		{
			this.TempData.ProductRanking = null;
			var vm = this.Service.CreateRegisterVm(actionStatus, rankingId, this.TempData);
			return View(vm);
		}

		/// <summary>
		/// 登録編集画面アクション
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="input">入力クラス</param>
		/// <param name="productRankingItems">商品ランキング</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "ToConfirm")]
		public ActionResult Register(ActionStatus actionStatus, ProductRankingInput input, ProductRankingItemInput[] productRankingItems)
		{
			var productRankingList = productRankingItems
				.Where(item => (string.IsNullOrEmpty(item.ProductId) == false))
				.ToList();

			productRankingList.ToList().ForEach(item =>
			{
				item.LastChanged = this.SessionWrapper.LoginOperatorName;
				item.RankingId = input.RankingId;
			});
			input.ProductRankingItems = productRankingList.ToArray();
			input.LastChanged = this.SessionWrapper.LoginOperatorName;
			
			var errorMessage = input.Validate(actionStatus);
			
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return CreateErrorPageAction(errorMessage);
			}

			this.TempData.ProductRanking = input.CreateModel(this.SessionWrapper.LoginShopId);
			return RedirectToAction(
				"Confirm",
				new
				{
					ActionStatus = actionStatus
				});
		}

		/// <summary>
		/// 戻るアクション
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>アクション結果</returns>
		[ActionName("Register")]
		[Button(ButtonName = "Back")]
		public ActionResult Back(ActionStatus actionStatus, string rankingId)
		{
			if (actionStatus == ActionStatus.Update)
			{
				return RedirectToAction(
					"Confirm",
					new
					{
						ActionStatus = ActionStatus.Detail,
						rankingId = rankingId,
					});
			}
			return BackList();
		}

		/// <summary>
		/// 集計実行アクション
		/// </summary>
		/// <param name="rankingIds">商品ランキングID</param>
		/// <param name="checks">集計対象フラグ</param>
		/// <returns>アクション結果</returns>
		[ActionName("Register")]
		[Button(ButtonName = "Total")]
		public ActionResult Total(string[] rankingIds, bool[] checks)
		{
			this.Service.TotalAction(rankingIds, checks);
			return BackList();
		}

		/// <summary>
		/// 登録アクション
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "Insert")]
		public ActionResult InsertOfRegister()
		{
			var vm = this.Service.Insert(this.TempData.ProductRanking);
			return View(vm);
		}

		/// <summary>
		/// コピー新規登録アクション
		/// </summary>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "CopyInsert")]
		public ActionResult CopyInsertOfRegister(string rankingId)
		{
			this.TempData.ProductRanking = null;
			return RedirectToAction(
				"Register",
				new
				{
					ActionStatus = ActionStatus.Insert,
					rankingId
				});
		}

		/// <summary>
		/// 更新アクション
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "Update")]
		public ActionResult Update()
		{
			var vm = this.Service.Update(this.TempData);
			return View(vm);
		}

		/// <summary>
		/// 編集アクション
		/// </summary>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>アクション結果</returns>
		[ActionName("Confirm")]
		[Button(ButtonName = "Edit")]
		public ActionResult EditOfDetail(string rankingId)
		{
			this.TempData.ProductRanking = null;
			return RedirectToAction(
				"Register",
				new
				{
					ActionStatus = ActionStatus.Update,
					rankingId
				});
		}

		/// <summary>
		/// 削除アクション
		/// </summary>
		/// <param name="rankingId">商品ランキングId</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "Delete")]
		public ActionResult DeleteOfRegister(string rankingId)
		{
			this.Service.Delete(rankingId);

			// 一覧へ戻る
			return BackList();
		}

		/// <summary>
		/// 一覧へ戻るアクション
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "BackList")]
		public ActionResult BackList()
		{
			return RedirectToAction("List", this.TempData.ProductRankingListParamModel);
		}

		/// <summary>
		/// 戻るアクション
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Confirm")]
		[Button(ButtonName = "BackEdit")]
		public ActionResult BackEdit(ActionStatus actionStatus)
		{
			return RedirectToAction(
				"Register",
				new
				{
					ButtonName = "Regist",
					ActionStatus = actionStatus,
					rankingId = this.TempData.ProductRanking.RankingId,
					ProductRankingModel = this.TempData.ProductRanking,
				});
		}

		/// <summary>サービス</summary>
		private ProductRankingWorkerService Service { get { return GetDefaultService<ProductRankingWorkerService>(); } }
	}
}
