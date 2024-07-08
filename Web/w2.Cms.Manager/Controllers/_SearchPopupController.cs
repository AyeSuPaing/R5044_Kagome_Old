/*
=========================================================================================================
  Module      : 検索ポップアップ コントローラー(SearchPopupController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.ParamModels.SearchPopup;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 検索ポップアップ コントローラー
	/// </summary>
	public class _SearchPopupController : BaseController
	{
		/// <summary>
		/// 広告媒体区分一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult AdCodeMediaTypeSearchList(AdCodeMediaTypeSearchListParamModel pm)
		{
			var vm = this.Service.CreateAdCodeMediaTypeSearchListVm(pm);
			return View(vm);
		}

		/// <summary>
		/// 広告コード一覧表示
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult AdCodeSearchList(AdCodeSearchListParamModel pm)
		{
			var vm = this.Service.CreateAdCodeSearchListVm(pm);
			return View(vm);
		}

		/// <summary>サービス</summary>
		private SearchPopupWorkerService Service { get { return GetDefaultService<SearchPopupWorkerService>(); } }
	}
}
