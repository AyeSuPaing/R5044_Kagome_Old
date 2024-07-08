/*
=========================================================================================================
  Module      : LPコントローラー(LandingPageController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using w2.App.Common.LandingPage;
using w2.App.Common.LandingPage.LandingPageDesignData;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.LandingPaeg;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.LandingPage;
using w2.Domain.Product.Helper;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// LPコントローラー
	/// </summary>
	public class LandingPageController : BaseController
	{
		/// <summary>
		/// メイン
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult Main()
		{
			return View();
		}

		/// <summary>
		/// 一覧
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		public ActionResult List(LandingPageListParamModel pm, string pageId)
		{
			var vm = this.Service.GetListView(pm);
			if (string.IsNullOrEmpty(pageId) == false)
			{
				vm.OpenDetailPageId = pageId;
			}
			return View(vm);
		}

		/// <summary>
		/// デザイナー
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="pageId">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>アクション結果</returns>
		public ActionResult Designer(ActionStatus actionStatus, string pageId, string designType)
		{
			var vm = this.Service.GetDesignerView(pageId, designType);
			return View(vm);
		}

		/// <summary>
		/// デフォルトブロックJSON生成（隠し機能）
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		[ActionName("CreateDefBlockJson")]
		public ActionResult CreateDefBlockJson(string pageId)
		{
			var data = this.Service.GetBlocksJson(pageId);
			var fi = new FileContentResult(Encoding.UTF8.GetBytes(data), "text/plain") { FileDownloadName = "formlp-blocks-default.json" };
			return fi;
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="pageId">削除対象のページID</param>
		/// <returns>アクション結果</returns>
		public ActionResult Delete(string pageId)
		{
			var message = this.Service.DeleteLpData(pageId);
			var data = new Dictionary<string, string>
			{
				{ "result", string.IsNullOrEmpty(message) ? "ok" : "ng" },
				{ "msg", message },
				{ "id", pageId }
			};
			return Json(data);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>アクション結果</returns>
		public ActionResult Update(LandingPageInput input)
		{
			Dictionary<string, string> data;
			var errorMsg = this.Service.UpdateLpData(input);
			if (string.IsNullOrEmpty(errorMsg) == false)
			{
				data = new Dictionary<string, string>
				{
					{ "result", "ng" },
					{"msg",errorMsg},
					{"id",input.PageId}
				};
			}
			else
			{
				data = new Dictionary<string, string>
				{
					{"result", "ok" },
					{"msg",""},
					{"id",input.PageId}
				};
			}
			return Json(data);
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>アクション結果</returns>
		public ActionResult Register(LandingPageInput input)
		{
			Dictionary<string, string> data;
			var errorMsg = this.Service.RegisterLpData(input);
			if (string.IsNullOrEmpty(errorMsg) == false)
			{
				data = new Dictionary<string, string>
				{
					{ "result", "ng" },
					{"msg",errorMsg},
					{"id",input.PageId}
				};
			}
			else
			{
				data = new Dictionary<string, string>
				{
					{"result", "ok" },
					{"msg",""},
					{"id",input.PageId}
				};
			}
			return Json(data);
		}

		/// <summary>
		/// Lp詳細ビュー取得
		/// </summary>
		/// <param name="pageId">対象のページID</param>
		/// <returns>アクション結果</returns>
		public ActionResult GetLpDetailViewModel(string pageId)
		{
			var data = this.Service.GetLpDetailViewModel(pageId);
			return Json(data);
		}

		/// <summary>
		/// LPドロップダウンリスト取得
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult GetLpDefaultDropDownList()
		{
			var data = this.Service.GetLpDefaultDropDownList();
			return Json(data);
		}

		/// <summary>
		/// Lp商品ビュー取得
		/// </summary>
		/// <param name="pageId">対象のページID</param>
		/// <returns>アクション結果</returns>
		public ActionResult GetLpProductViewModel(string pageId)
		{
			var data = this.Service.GetLpProductViewModel(pageId);
			return Json(data);
		}

		/// <summary>
		/// デザイン更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>アクション結果</returns>
		public ActionResult UpdateDesign(PageDesignInput input)
		{
			Dictionary<string, string> data;
			var errorMsg = this.Service.UpdateLpDesign(input);
			if (string.IsNullOrEmpty(errorMsg) == false)
			{
				data = new Dictionary<string, string>
				{
					{ "result", "ng" },
					{"msg",errorMsg},
					{"id",input.PageId}
				};
			}
			else
			{
				data = new Dictionary<string, string>
				{
					{"result", "ok" },
					{"msg",""},
					{"id",input.PageId}
				};
			}
			return Json(data);
		}

		/// <summary>
		/// 画像リスト取得
		/// </summary>
		/// <param name="groupId">対象グループID</param>
		/// <param name="keyWord">キーワード</param>
		/// <returns>アクション結果</returns>
		public ActionResult GetImageList(string groupId, string keyWord)
		{
			var result = this.Service.GetImageList(groupId, keyWord);
			return Json(result);
		}

		/// <summary>
		/// 画像グループリスト取得
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult GetImageGroupListItem()
		{
			var result = this.Service.GetImageGroupListItems();
			return Json(result);
		}

		/// <summary>
		/// ファイルアップロード
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult FileUpload()
		{
			for (int i = 0; i < Request.Files.Count; i++)
			{
				var file = Request.Files[i];

				if (file != null)
				{
					var fileName = Path.GetFileName(file.FileName);

					if (fileName != null)
					{
						var path = Path.Combine(this.Server.MapPath("~/Junk/"), fileName);
						file.SaveAs(path);
					}
				}
			}

			var data = new Dictionary<string, string>
			{
				{"result", "ok" },
				{"msg",""},
				{"id",""}
			};
			return Json(data);
		}

		/// <summary>
		/// 総件数を取得
		/// </summary>
		/// <param name="paramModel">検索条件</param>
		/// <returns>結果</returns>
		public override int GetSearchHitCountOnCms(ProductSearchParamModel paramModel)
		{
			var count = this.Service.GetSearchHitCountOnCms(paramModel);
			return count;
		}

		/// <summary>
		/// 絞り込み検索
		/// </summary>
		/// <param name="paramModel">検索条件</param>
		/// <returns>結果</returns>
		public override ActionResult ProductSearch(ProductSearchParamModel paramModel)
		{
			var vm = this.Service.ProductSearch(paramModel);
			return PartialView("_ProductVariationSearchResult", vm);
		}

		/// <summary>
		/// 商品バリエーションセット
		/// </summary>
		/// <param name="products">商品情報</param>
		/// <returns>アクション結果</returns>
		public ActionResult SetProductVariation(ProductSearchResultModel[] products)
		{
			var data = this.Service.GetLpProducts(products);
			return Json(data);
		}

		/// <summary>
		/// ページリストビュー取得
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult GetPageListViewModel(LandingPageListParamModel pm)
		{
			var vm = this.Service.GetListView(pm);
			return Json(vm);
		}
		
		/// <summary>
		/// プレビューファイル生成
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>アクション結果</returns>
		public ActionResult CreatePreviewFileListPage(LandingPageInput input, string designType)
		{
			var previewKey = this.Service.CreatePreviewFile(input, designType);
			var data = new Dictionary<string, string>
			{
				{"previewKey", previewKey }
			};
			return Json(data);
		}

		/// <summary>
		/// プレビューファイル生成
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>アクション結果</returns>
		public ActionResult CreatePreviewFileDesignerPage(PageDesignInput input, string designType)
		{
			var previewKey = this.Service.CreatePreviewFile(input, designType);
			var data = new Dictionary<string, string>
			{
				{"previewKey", previewKey }
			};
			return Json(data);
		}

		/// <summary>
		/// プレビュー
		/// </summary>
		/// <param name="previewKey">プレビュー用キー</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>アクション結果</returns>
		public ActionResult Preview(string previewKey, string designType)
		{
			var vm = this.Service.CreatePreviewVm(previewKey, designType);
			return View(vm);
		}

		/// <summary>
		/// メンテナンスツールを有効化
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult EnableToMaintenaceTool()
		{
			LpDesignHelper.EnableToMaintenaceTool();
			var data = new Dictionary<string, string>
			{
				{"result", "ok" }
			};
			return Json(data);
		}

		/// <summary>
		/// 全デザインファイル再作成
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult RecreateAllFile()
		{
			LpDesignHelper.RecreateAllFile();
			var data = new Dictionary<string, string>
			{
				{"result", "ok" }
			};
			return Json(data);
		}

		/// <summary>
		/// ページがあるか
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		public ActionResult ExistPage(string pageId)
		{
			var data = new Dictionary<string, string>();
			var result = new LandingPageService().Get(pageId) != null;
			data.Add("result", result ? "true" : "false");
			return Json(data);
		}

		/// <summary>
		/// 画像フォームをセット
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult SetImageForm()
		{
			return PartialView("_ImageForm", new ImageInput());
		}

		/// <summary>
		/// アップロード済み画像をセット
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		public ActionResult SetUploadedImage(string pageId)
		{
			var vm = this.Service.CreateUploadedImageVm(pageId);
			return PartialView("_UploadedImage", vm);
		}

		/// <summary>
		/// アップロード
		/// </summary>
		/// <param name="files">画像</param>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		public ActionResult Upload(HttpPostedFileBase files, string pageId)
		{
			var errorMessage = this.Service.Upload(files, pageId);

			return Content(errorMessage);
		}

		/// <summary>
		/// 画像を削除
		/// </summary>
		/// <param name="imageNo">イメージNo</param>
		/// <param name="pageId">ページID</param>
		/// <returns>アクション結果</returns>
		public ActionResult DeleteImage(int imageNo, string pageId)
		{
			var errorMessage = this.Service.DeleteImage(imageNo, pageId);
			return Content(errorMessage);
		}

		/// <summary>
		/// 画像リストからコピー
		/// </summary>
		/// <param name="path">パス</param>
		/// <param name="pageId">コーディネートID</param>
		/// <returns>アクション結果</returns>
		public ActionResult CopyFromImageList(string path, string pageId)
		{
			var errorMessage = this.Service.CopyFromImageList(path, pageId);

			return Content(errorMessage);
		}

		/// <summary>
		/// LP新規作成状態取得
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult GetLpRegisterState()
		{
			var vm = this.Service.GetLpRegisterState();
			return Json(vm);
		}

		/// <summary>
		/// ABテストアイテム件数取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>件数</returns>
		public int GetCountInAbTestItemByPageId(string pageId)
		{
			var count = this.Service.GetCountInAbTestItemByPageId(pageId);
			return count;
		}

		/// <summary>サービス</summary>
		private LandingPageWorkerService Service { get { return GetDefaultService<LandingPageWorkerService>(); } }
	}
}