/*
=========================================================================================================
  Module      : 商品タグマネージャー コントローラー(ProductTagManagerController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using w2.App.Common.DataCacheController;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ProductTagManager;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.Affiliate;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 商品タグマネージャー コントローラー
	/// </summary>
	public class _ProductTagManagerController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(ProductTagManagerListParamModel pm)
		{
			var vm = this.Service.CreateListVm(pm);
			// 検索条件を一時保持
			this.TempData.ProductTagManagerListParamModel = pm;
			return View(vm);
		}

		/// <summary>
		/// 一括更新
		/// </summary>
		/// <param name="modifyInputs">入力内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "Update")]
		public ActionResult Update(ProductTagManagerInput[] modifyInputs)
		{
			var listModels = new List<AffiliateProductTagSettingModel>();
			foreach (var input in modifyInputs)
			{
				if ((input.TagName == input.BeforeTagName) 
					&& (input.TagContent == input.BeforeTagContent)
					&& (input.TagDelimiter == input.BeforeTagDelimiter)) continue;
				listModels.Add(input.CreateModel());
			}
			var vm = this.Service.CreateRegisterVmUpdate(listModels);
			return View(vm);
		}

		/// <summary>
		/// 入力内容を確認
		/// </summary>
		/// <param name="modifyInputs">入力値</param>
		/// <returns>エラーメッセージ</returns>
		public string ValidateUpdate(ProductTagManagerInput[] modifyInputs)
		{
			var errorMessages = new StringBuilder();
			var listModels = new List<AffiliateProductTagSettingModel>();
			if (modifyInputs != null)
			{
				foreach (var input in modifyInputs)
				{
					if ((input.TagName == input.BeforeTagName)
						&& (input.TagContent == input.BeforeTagContent)
						&& (input.TagDelimiter == input.BeforeTagDelimiter)) continue;

					var validateMessages = input.Validate(ActionStatus.Update);
					if (string.IsNullOrEmpty(validateMessages)) listModels.Add(input.CreateModel());
					errorMessages.Append(validateMessages.Replace("@@ 1 @@", input.AffiliateProductTagId));
				}
			}

			// 入力内容に不備がある場合
			if (errorMessages.Length != 0)
			{
				return errorMessages.ToString();
			}

			// 更新対象が存在しない場合
			if (listModels.Count == 0)
			{
				return WebMessages.TagManagerProductTagSelectedError;
			}

			return string.Empty;
		}

		/// <summary>
		/// 入力内容を確認
		/// </summary>
		/// <param name="registerInput">入力値</param>
		/// <returns>エラーメッセージ</returns>
		public string ValidateInsert(ProductTagManagerInput registerInput)
		{
			var errorMessage = registerInput.Validate(ActionStatus.Insert);
			return (string.IsNullOrEmpty(errorMessage) == false) ? errorMessage : string.Empty;
		}

		/// <summary>
		/// 追加
		/// </summary>
		/// <param name="registerInput">入力内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "Insert")]
		public ActionResult Insert(ProductTagManagerInput registerInput)
		{
			var vm = this.Service.CreateRegisterVmInsert(registerInput);
			return View(vm);
		}

		/// <summary>
		/// 続けて処理する
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpGet]
		[ActionName("List")]
		[Button(ButtonName = "Continue")]
		public ActionResult Continue()
		{
			return RedirectToAction("List", this.TempData.ProductTagManagerListParamModel);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="affiliateProductTagId">削除対象の商品タグID</param>
		/// <returns>削除結果 成功:true 失敗:false</returns>
		[HttpPost]
		public bool Delete(string affiliateProductTagId)
		{
			var tempAffiliateProductTagId = 0;
			if (int.TryParse(affiliateProductTagId, out tempAffiliateProductTagId) == false)
			{
				this.TempData.ErrorMessage = WebMessages.SystemError;
				return false;
			}

			var model = new AffiliateTagSettingService().AffiliateProductTagSettingGet(tempAffiliateProductTagId);
			var affiliateTags = DataCacheControllerFacade.GetAffiliateTagSettingCacheController()
				.CacheData
				.Where(a => a.AffiliateProductTagId == tempAffiliateProductTagId)
				.ToArray();
			if ((model == null) || affiliateTags.Length > 0)
			{
				this.TempData.ErrorMessage = WebMessages
					.TagManagerProductTagDeleteError
					.Replace("@@ 1 @@", string.Join(",", affiliateTags.Select(m => m.AffiliateId)));
				return false;
			}
			
			this.Service.Delete(model.AffiliateProductTagId);
			return true;
		}

		/// <summary>サービス</summary>
		private ProductTagManagerWorkerService Service { get { return GetDefaultService<ProductTagManagerWorkerService>(); } }
	}
}
