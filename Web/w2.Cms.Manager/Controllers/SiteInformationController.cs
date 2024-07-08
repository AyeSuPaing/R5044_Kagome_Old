/*
=========================================================================================================
  Module      : サイト情報コントローラ(SiteInformationController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using Castle.Core.Internal;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// サイト情報コントローラ
	/// </summary>
	public class SiteInformationController : BaseController
	{
		/// <summary>
		/// 編集画面
		/// </summary>
		/// <returns>結果</returns>
		public ActionResult Modify()
		{
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.SessionWrapper.TranslationSearchCondition = new string[0];
				this.SessionWrapper.TranslationExportTargetDataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION;
			}

			var vm = this.Service.CreateModifyVm();
			return View(vm);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">サイト情報入力</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult Update(SiteInformationInput input)
		{
			var errorMessage = this.Service.UpdateShopMessageXml(input);

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return Json(errorMessage, JsonRequestBehavior.AllowGet);
			}
			return Json("", JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="nodeName">ノード名</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult Delete(string nodeName)
		{
			var errorMessage = this.Service.DeleteShopMessageXml(nodeName);
			if (errorMessage.IsNullOrEmpty() == false)
			{
				return Json(errorMessage, JsonRequestBehavior.AllowGet);
			}
			return Json("", JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 追加
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult Add()
		{
			var errorMessage = this.Service.AddShopMessageXml();
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return Json(errorMessage, JsonRequestBehavior.AllowGet);
			}
			return Json("", JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// ステータスを確認
		/// </summary>
		/// <returns>ステータス</returns>
		public ActionResult CheckStatus()
		{
			var message = this.Service.CheckStatus();
			if (string.IsNullOrEmpty(message) == false)
			{
				return Json(message, JsonRequestBehavior.AllowGet);
			}
			return Json("", JsonRequestBehavior.AllowGet);
		}

		/// <summary>サービス</summary>
		private SiteInformationWorkerService Service { get { return GetDefaultService<SiteInformationWorkerService>(); } }
	}
}
