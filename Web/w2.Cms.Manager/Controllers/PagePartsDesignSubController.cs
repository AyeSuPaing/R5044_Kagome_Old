/*
=========================================================================================================
  Module      : ページパーツ管理 サブコントローラ(PagePartsDesignSubController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using System.Web.SessionState;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Cms;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ParamModels.PartsDesign;
using w2.Cms.Manager.ViewModels.Error;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// ページパーツ管理 サブコントローラ（セッション読み取り専用・ページからのAJAX呼び出し用）
	/// </summary>
	/// <remarks>
	/// セッションを読み取り専用とすることで、リクエストのセッション排他を回避（並列実行可能）し、パフォーマンスを向上します。
	/// ページ内の非同期取得などのAJAX通信で利用します。
	/// </remarks>
	[SessionState(SessionStateBehavior.ReadOnly)]
	public class PagePartsDesignSubController : BaseController
	{
		/// <summary>
		/// パーツ検索(PC)
		/// </summary>
		/// <param name="partsParamPc">検索条件</param>
		/// <param name="pcPartsTypes">パーツタイプ</param>
		/// <param name="useType">対象デバイス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult SearchPartsListPc(
			PartsDesignListSearchParamModel partsParamPc,
			CheckBoxModel[] pcPartsTypes,
			string useType)
		{
			return SearchPartsList(partsParamPc, pcPartsTypes, useType);
		}

		/// <summary>
		/// パーツ検索(SP)
		/// </summary>
		/// <param name="partsParamSp">検索条件</param>
		/// <param name="spPartsTypes">パーツタイプ</param>
		/// <param name="useType">対象デバイス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult SearchPartsListSp(
			PartsDesignListSearchParamModel partsParamSp,
			CheckBoxModel[] spPartsTypes,
			string useType)
		{
			return SearchPartsList(partsParamSp, spPartsTypes, useType);
		}

		/// <summary>
		/// パーツ検索
		/// </summary>
		/// <param name="partsParam">検索条件</param>
		/// <param name="partsTypes">パーツタイプ</param>
		/// <param name="useType">対象デバイス</param>
		/// <returns>アクション結果</returns>
		private ActionResult SearchPartsList(
			PartsDesignListSearchParamModel partsParam,
			CheckBoxModel[] partsTypes,
			string useType)
		{
			partsParam.Types = partsTypes;
			var vm = this.Service.CreateGroupListVm(partsParam, Constants.PARTS_IN_PAGEDESIGN_MAX_VIEW_CONTENT_COUNT, useType);
			if (string.IsNullOrEmpty(vm.ErrorMessage) == false)
			{
				var errorVm = new IndexViewModel()
				{
					ErrorMessagesHtmlEncoded = vm.ErrorMessage
				};
				return PartialView("_ErrorMessage", errorVm);
			}
			else
			{
				return PartialView("_LayoutEditPartsList", vm);
			}
		}

		/// <summary>
		/// 他のオペレーターが開いているか判定
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>アクショsン結果</returns>
		[HttpPost]
		public ActionResult CheckOtherOperatorFileOpening(string fileName)
		{
			var message = new ConcurrentEditUtil().CheckOtherOperatorFileOpening(fileName);
			return Json(message, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// オペレーターが開いているということを通知
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult NoticeOperatorOpeningFile(string fileName)
		{
			var message = new ConcurrentEditUtil().NoticeOperatorOpeningFile(fileName);
			var result = Json(message, JsonRequestBehavior.AllowGet);
			return result;
		}

		/// <summary>サービス</summary>
		private PartsDesignWorkerService Service { get { return GetDefaultService<PartsDesignWorkerService>(); } }
	}
}