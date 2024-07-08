/*
=========================================================================================================
 Module      : サイトマップ設定コントローラ(SitemapController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.Sitemap;
using w2.Cms.Manager.ParamModels.Sitemap;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>n
	/// XMLサイトマップ設定コントローラ
	/// </summary>
	public class SitemapController : BaseController
	{
		/// <summary>
		/// メイン画面アクション
		/// </summary>
		/// <returns>アクションリザルト</returns>
		[HttpGet]
		public ActionResult Main(MainParamModel paramModel, int pno = 1)
		{
			// サイトマップ設定ファイルのチェック
			if (this.Service.IsValidSitemapSetting() == false)
			{
				return CreateErrorPageAction(WebMessages.SitemapSettingSerializationFailed);
			}

			paramModel.PagerNo = pno;
			var vm = this.Service.CreateViewModelForMain(paramModel);
			return View("Main", vm);
		}

		/// <summary>
		/// 設定更新アクション
		/// </summary>
		/// <param name="pageItem">ページアイテム</param>
		/// <returns>メッセージ</returns>
		[HttpPost]
		public ActionResult Update(SitemapPageItem[] pageItem)
		{
			this.Service.Update(pageItem);
			return Json(string.Empty);
		}

		/// <summary>サービス</summary>
		private SitemapWorkerService Service { get { return GetDefaultService<SitemapWorkerService>(); } }
	}
}