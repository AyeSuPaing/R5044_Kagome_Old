/*
=========================================================================================================
  Module      : タグマネージャー コントローラー(TagManagerController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using w2.App.Common.Affiliate;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.TagManager;
using w2.Cms.Manager.ViewModels.TagManager;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// タグマネージャー コントローラー
	/// </summary>
	public class TagManagerController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(TagManagerListParamModel pm)
		{
			var vm = this.Service.CreateListVm(pm);
			// 検索条件を一時保持
			this.TempData.TagManagerListParamModel = pm;
			return View(vm);
		}


		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="affiliateId">タグID</param>
		/// <returns></returns>
		public ActionResult Register(ActionStatus actionStatus, string affiliateId)
		{
			var vm = this.Service.CreateRegisterVmDetail(actionStatus, affiliateId);
			return (vm != null) ? View(vm) : CreateErrorPageAction(WebMessages.InconsistencyError);
		}


		/// <summary>
		/// 置換タグの表示更新
		/// </summary>
		/// <param name="input">タグ入力内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult ReplaceTagDisplayUpdate(TagManagerInput input)
		{
			var replaceTagList = input.IsAllPageCheck
				? new ReplaceTagManager().SelectPageReplaceTagList(TagSetting.ACTION_TYPE_ALL)
				: new ReplaceTagManager().SelectPageReplaceTagList(
					input.Pages.Where(m => m.IsCheck).Select(m => m.Path).ToList());

			var partialViewListItem = replaceTagList.Select(
				m => new ListItem
				{
					Value = m,
					Text = ValueText.GetValueText("AffiliateTag", "ReplaseTagDescription", m)
				}).ToArray();
			return PartialView("_ReplaceTag", partialViewListItem);
		}


		/// <summary>
		/// 商品タグIDの表示内容更新
		/// </summary>
		/// <param name="selectedAffiliateProductTagId">選択されている商品タグID</param>
		/// <returns>アクション結果</returns>
		[HttpGet]
		public ActionResult ProductTagDisplayUpdate(string selectedAffiliateProductTagId)
		{
			var model = new ProductTagViewModel(selectedAffiliateProductTagId);
			return PartialView("_ProductTag", model);
		}

		/// <summary>
		/// 入力内容を確認
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="conditionInput">コンディション入力値</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(TagManagerInput input, TagManagerConditionInput conditionInput)
		{
			var errorMessages = this.Service.ErrorCheck(input).ToString();
			return StringUtility.ToEmpty(errorMessages);
		}

		/// <summary>
		/// 削除時バリデート
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="conditionInput">コンディション入力値</param>
		/// <returns></returns>
		public string ValidateForDelete(TagManagerInput input, TagManagerConditionInput conditionInput)
		{
			var errorMessages = new StringBuilder();
			var shopOperators = new ShopOperatorService().GetOperatorListWithTagID();
			if (shopOperators == null) return string.Empty;
			foreach (var shopOperator in shopOperators)
			{
				var affiliateTagIds = shopOperator.UsableAffiliateTagIdsInReport.Split(',');
				foreach (var affiliateTagId in affiliateTagIds)
				{
					if (input.AffiliateId.Equals(affiliateTagId))
					{
						errorMessages.Append(WebMessages.TagManagerSelectCheckedError).Replace("@@ 1 @@", shopOperator.OperatorId);
					}
				}
			}
			return errorMessages.ToString();
		}
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">タグ入力内容</param>
		/// <param name="conditionInput">タグ表示条件 入力内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "Update")]
		public ActionResult Update(TagManagerInput input, TagManagerConditionInput conditionInput)
		{
			var vm = this.Service.CreateRegisterVmUpdate(input, conditionInput);
			return View(vm);
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">タグ入力内容</param>
		/// <param name="conditionInput">タグ表示条件 入力内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "Insert")]
		public ActionResult Insert(TagManagerInput input, TagManagerConditionInput conditionInput)
		{
			var vm = this.Service.CreateRegisterVmInsert(input, conditionInput);
			return View(vm);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="affiliateId">削除対象のタグID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "Delete")]
		public ActionResult Register(string affiliateId)
		{
			this.Service.Delete(affiliateId);
			// 一覧へ戻る
			return BackList();
		}

		/// <summary>
		/// コピー新規登録
		/// </summary>
		/// <param name="affiliateId">コピー対象のタグID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "CopyInsert")]
		public ActionResult CopyInsert(string affiliateId)
		{
			var vm = this.Service.CreateRegisterVmDetail(ActionStatus.Insert, affiliateId);
			return View(vm);
		}

		/// <summary>
		/// 一覧へ戻る
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Register")]
		[Button(ButtonName = "BackList")]
		public ActionResult BackList()
		{
			return RedirectToAction("List", this.TempData.TagManagerListParamModel);
		}

		/// <summary>サービス</summary>
		private TagManagerWorkerService Service { get { return GetDefaultService<TagManagerWorkerService>(); } }
	}
}
