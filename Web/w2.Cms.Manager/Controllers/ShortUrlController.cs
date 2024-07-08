/*
=========================================================================================================
  Module      : ショートURLコントローラー(NewsController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Text;
using System.Web.Mvc;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ShortUrl;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;
using w2.Domain.ShortUrl;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// ショートURLコントローラー
	/// </summary>
	public class ShortUrlController : BaseController
	{
		/// <summary>
		/// 一覧
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(ShortUrlListParamModel pm)
		{
			var vm = this.Service.Search(pm);
			return View(vm);
		}

		/// <summary>
		/// エクスポート
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "Export")]
		public ActionResult Export(ShortUrlListParamModel pm)
		{
			var fileData = this.Service.Export(pm);

			// エラーがあればエラー画面へ
			if (string.IsNullOrEmpty(fileData.Error) == false)
			{
				return CreateErrorPageAction(fileData.Error);
			}
			return fileData.CreateActionResult();
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public string ValidateExport(ShortUrlListParamModel pm)
		{
			return (string.IsNullOrEmpty(pm.DataExportType)) ? WebMessages.MasterexportSettingSettingIdNotSelected : string.Empty;
		}

		/// <summary>
		/// 編集へ
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "ToEdit")]
		public ActionResult ToEdit(ShortUrlListParamModel pm)
		{
			var vm = this.Service.ToEditMode(pm);
			return View(vm);
		}

		/// <summary>
		/// 一覧へ
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "ToList")]
		public ActionResult ToList(ShortUrlListParamModel pm)
		{
			var vm = this.Service.ToListMode(pm);
			return View(vm);
		}

		/// <summary>
		/// 一括更新
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <param name="bt">更新対象の入力データ</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "BulkUpdate")]
		public ActionResult BulkUpdate(ShortUrlListParamModel pm, ShortUrlInput[] bt)
		{
			var vm = this.Service.BulkUpdate(pm, bt);
			return View(vm);
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <param name="bt">更新対象の入力データ</param>
		/// <returns>アクション結果</returns>
		public string ValidateUpdate(ShortUrlListParamModel pm, ShortUrlInput[] bt)
		{
			// 変更があったもの
			var targets = bt.Where(i => i.IsUrlChanged()).ToArray();
			if (targets.Length == 0) return WebMessages.ShorturlTargetNoSelectedError;

			// チェック
			var checkModels = new ShortUrlService().GetAll(this.SessionWrapper.LoginShopId);
			var error = new StringBuilder();
			foreach (var target in targets)
			{
				target.ShopId = base.SessionWrapper.LoginShopId;
				error.Append(target.Validate(false, checkModels));
			}

			return StringUtility.ToEmpty(error);
		}

		/// <summary>
		/// 続ける
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "ToContinue")]
		public ActionResult ToContinue(ShortUrlListParamModel pm)
		{
			var vm = this.Service.ToContinue(pm);
			return View(vm);
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <param name="ri">登録対象の入力データ</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "Register")]
		public ActionResult Register(ShortUrlListParamModel pm, ShortUrlInput ri)
		{
			var vm = this.Service.Register(pm, ri);
			return View(vm);
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <param name="ri">更新対象の入力データ</param>
		/// <returns>アクション結果</returns>
		public string ValidateInsert(ShortUrlListParamModel pm, ShortUrlInput ri)
		{
			// チェック
			ri.ShopId = base.SessionWrapper.LoginShopId;
			var error = ri.Validate(true, new ShortUrlService().GetAll(this.SessionWrapper.LoginShopId));
			return StringUtility.ToEmpty(error);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("List")]
		[Button(ButtonName = "Delete")]
		public ActionResult Delete(ShortUrlListParamModel pm)
		{
			var vm = this.Service.Delete(pm);
			return View(vm);
		}

		/// <summary>サービス</summary>
		private ShortUrlWorkerService Service { get { return GetDefaultService<ShortUrlWorkerService>(); } }
	}
}