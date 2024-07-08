/*
=========================================================================================================
  Module      : 公開範囲設定 コントローラ(_ReleaseRangeSettingController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 公開範囲設定 コントローラ
	/// </summary>
	public class _ReleaseRangeSettingController : BaseController
	{
		/// <summary>
		/// ターゲットリストの取得
		/// </summary>
		/// <param name="keyword">検索キーワード</param>
		/// <returns>アクション結果</returns>
		public ActionResult TargetList(string keyword)
		{
			var models = this.Service.SearchTargetListModel(keyword);
			return PartialView("_ReleaseRangeSettingTargetList", models);
		}

		/// <summary>サービス</summary>
		private ReleaseRangeSettingWorkerService Service { get { return GetDefaultService<ReleaseRangeSettingWorkerService>(); } }
	}
}