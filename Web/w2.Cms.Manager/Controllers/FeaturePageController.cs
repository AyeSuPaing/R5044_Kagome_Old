/*
=========================================================================================================
  Module      : 特集ページコントローラ (FeaturePageController.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Cms;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeaturePage;
using w2.Cms.Manager.ViewModels.FeaturePage;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.FeaturePage;
using w2.Domain.ProductGroup;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 特集ページコントローラ
	/// </summary>
	public class FeaturePageController : BaseController
	{
		/// <summary>
		/// メイン画面
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult Main()
		{
			var viewModel = this.Service.CreateMainVm();
			return View(viewModel);
		}

		/// <summary>
		/// ページ詳細画面の取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult MainContentDetail(long? pageId)
		{
			if (pageId != null)
			{
				this.TempData.FeaturePageModel = new FeaturePageModel
				{
					FeaturePageId = (long)pageId
				};
			}

			var viewModel = this.Service.CreateFeaturePageDetailVm(pageId);
			if (viewModel == null) return JavaScript("open_page_failed()");
			return PartialView("_MainContentsDetail", viewModel);
		}

		/// <summary>
		/// 一覧の取得
		/// </summary>
		/// <param name="paramModel">Param model</param>
		/// <param name="types">Types</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(FeaturePageListSearchParamModel paramModel, CheckBoxModel[] types)
		{
			PageDesignUtility.DeleteFeaturePreview();
			paramModel.Types = types;
			var viewModel = this.Service.CreateListVm(paramModel);
			return PartialView("_MainContentsList", viewModel);
		}

		/// <summary>
		/// ページ詳細 更新
		/// </summary>
		/// <param name="input">更新内容</param>
		/// <returns>エラーメッセージ</returns>
		[HttpPost]
		public ActionResult UpdateDetailPage(FeaturePageInput input)
		{
			var errorMessage = this.Service.UpdateDetailPage(input);
			return Json(errorMessage, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// ページ順序
		/// </summary>
		/// <param name="pageIds">ページ順序</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult UpdatePageSort(long[] pageIds)
		{
			this.Service.PageSortUpdate(pageIds);
			return null;
		}

		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>エラーメッセージ</returns>
		[HttpPost]
		public ActionResult DeletePage(long pageId)
		{
			var errorMessage = this.Service.DeletePage(pageId);
			return Json(errorMessage, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// ページの複製
		/// </summary>
		/// <param name="pageId">複製元のページID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult MainContentDetailCopy(long pageId)
		{
			var newPageId = this.Service.CreateCopyFile(pageId);
			var viewModel = this.Service.CreateFeaturePageDetailVm(newPageId);
			return PartialView("_MainContentsDetail", viewModel);
		}

		/// <summary>
		/// プレビュー生成 PC用
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult PreviewPc(FeaturePageInput input)
		{
			// プレビュー画面生成 URL返却
			var url = this.Service.Preview(input, DesignCommon.DeviceType.Pc);
			var errorMessage = WebRequestCheck.Send(url);
			var result = string.IsNullOrEmpty(errorMessage)
				? Json(data: new { url }, JsonRequestBehavior.AllowGet)
				: Json(data: new { errorMessage }, JsonRequestBehavior.AllowGet);
			return result;

		}

		/// <summary>
		/// プレビュー生成 SP用
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		public ActionResult PreviewSp(FeaturePageInput input)
		{
			// プレビュー画面生成 URL返却
			var url = this.Service.Preview(input, DesignCommon.DeviceType.Sp);
			var errorMessage = WebRequestCheck.Send(url);
			var result = string.IsNullOrEmpty(errorMessage)
				? Json(data: new { url }, JsonRequestBehavior.AllowGet)
				: Json(data: new { errorMessage }, JsonRequestBehavior.AllowGet);
			return result;
		}

		/// <summary>
		/// 商品グループ以外の商品一覧入力
		/// </summary>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="modelNo">商品一覧モデル識別番号</param>
		/// <param name="list">商品リスト</param>
		/// <returns>アクション結果</returns>
		public override ActionResult ProductInputView(
			string baseName,
			string modelNo,
			ProductViewModel[] list)
		{
			var viewModel = new ProductInputViewModel
			{
				BaseName = baseName,
				ProductList = list,
				DispNum = list.Length
			};

			return PartialView("_ProductListInput", viewModel);
		}

		/// <summary>
		/// 商品グループを選択した場合の商品一覧入力
		/// </summary>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="modelNo">商品一覧モデル識別番号</param>
		/// <param name="list">商品リスト</param>
		/// <param name="group">商品グループ</param>
		/// <returns>アクション結果</returns>
		public override ActionResult ProductInputView(
			string baseName,
			string modelNo,
			ProductViewModel[] list,
			ProductGroupModel group)
		{
			var viewModel = new ProductInputViewModel
			{
				BaseName = baseName,
				ProductList = list,
				GroupId = group.ProductGroupId,
				GroupName = group.ProductGroupName,
				DispNum = list.Length
			};

			return PartialView("_ProductGroupInput", viewModel);
		}

		/// <summary>
		/// 管理用ページタイトル変更
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="title">タイトル名</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult ManagementTitleEdit(long pageId, string title)
		{
			this.Service.ManagementTitleEdit(pageId, title);
			return null;
		}

		/// <summary>
		/// 商品表示順序更新
		/// </summary>
		/// <param name="productIds">商品ID配列</param>
		/// <param name="sortType">並び順</param>
		/// <param name="baseName">バインド時の名前</param>
		/// <returns>アクション結果</returns>
		public ActionResult UpdateProductSort(
			string[] productIds,
			string sortType,
			string baseName)
		{
			var viewModel = this.Service.UpdateProductSort(productIds, sortType, baseName);
			return PartialView("_ProductInput", viewModel);
		}

		/// <summary>
		/// 最上位カテゴリ選択時に紐づく子カテゴリのリストを作成
		/// </summary>
		/// <param name="rootCategoryId">最上位カテゴリID</param>
		/// <returns>子カテゴリリスト</returns>
		public ActionResult CreateChildCategoryList(string rootCategoryId)
		{
			var viewModel = this.Service.CreateChildCategoryList(this.Service.PageId, rootCategoryId);
			return PartialView("_ProductCategory", viewModel);
		}

		/// <summary>
		/// 商品表示順序更新
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>アクション結果</returns>
		public bool IsSettingProducts(FeaturePageInput input)
		{
			return (input.ProductInput != null);
		}

		/// <summary>サービス</summary>
		private FeaturePageWorkerService Service { get { return GetDefaultService<FeaturePageWorkerService>(); } }
	}
}
