/*
=========================================================================================================
  Module      : コーディネートページコントローラ(CoordinatePageController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.Coordinate;
using w2.Cms.Manager.ViewModels.Coordinate;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Web;
using w2.Domain.Coordinate;
using w2.Domain.Product;
using w2.Domain.RealShop;
using w2.Domain.Staff;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// コーディネートページコントローラ
	/// </summary>
	public class CoordinatePageController : BaseController
	{
		/// <summary>
		/// コーディネートページ表示
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult Main()
		{
			var vm = this.Service.CoordinatePageVm();
			return View(vm);
		}

		/// <summary>
		/// 一覧の取得
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult List(CoordinateParamModel pm)
		{
			var vm = this.Service.CreateListVm(pm);
			return PartialView("List", vm);
		}

		/// <summary>
		/// カテゴリ一覧の取得
		/// </summary>
		/// <param name="searchCategoryModal">カテゴリモーダルを検索</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult CategoryList(string searchCategoryModal)
		{
			var vm = this.Service.CreateCategoryListVm(searchCategoryModal);
			return PartialView("CategoryList", vm);
		}

		/// <summary>
		/// ページ詳細画面の取得
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult Detail(string coordinateId)
		{
			var vm = this.Service.CreateDetailVm(coordinateId);
			if (this.ModelState.IsValid) this.ModelState.Clear();
			return PartialView("Detail", vm);
		}

		/// <summary>
		/// 商品一覧入力
		/// </summary>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="modelNo">商品一覧モデル識別番号</param>
		/// <param name="list">商品リスト</param>
		/// <returns>アクション結果</returns>
		public override ActionResult ProductInputView(string baseName, string modelNo, ProductViewModel[] list)
		{
			var vm = new ProductInputModel
			{
				BaseName = baseName,
				ProductList = list,
			};

			return PartialView("ProductListInput", vm);
		}

		/// <summary>
		/// アップロード済み画像をセット
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>アクション結果</returns>
		public ActionResult SetUploadedImage(string coordinateId)
		{
			var vm = this.Service.CreateUploadedImageVm(coordinateId);
			return PartialView("UploadedImage", vm);
		}

		/// <summary>
		/// 画像フォームをセット
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult SetImageForm()
		{
			var input = new CoordinateInput();
			return PartialView("ImageForm", input.Image);
		}

		/// <summary>
		/// カテゴリ入力をセット
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>アクション結果</returns>
		public ActionResult SetCategoryInput(string coordinateId, ActionStatus actionStatus)
		{
			var vm = this.Service.CreateCategoryVm(coordinateId, actionStatus);
			return PartialView("CategoryInput", vm);
		}

		/// <summary>
		/// カテゴリ入力を変更
		/// </summary>
		/// <param name="categories">カテゴリ</param>
		/// <returns>アクション結果</returns>
		public ActionResult ChangeCategoryInput(string categories)
		{
			var vm = this.Service.ChangeCategoryVm(categories);
			return PartialView("CategoryInput", vm);
		}

		/// <summary>
		/// 商品表示順序更新
		/// </summary>
		/// <param name="productIds">商品ID配列</param>
		/// <param name="sortType">並び順</param>
		/// <param name="baseName">バインド時の名前</param>
		/// <returns>アクション結果</returns>
		public ActionResult UpdateProductSort(string[] productIds, string sortType, string baseName)
		{
			var vm = this.Service.UpdateProductSort(productIds, sortType, baseName);

			return PartialView("ProductInput", vm);
		}

		/// <summary>
		/// ページ詳細 更新
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="input">更新内容</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult UpdateDetailPage(ActionStatus actionStatus, CoordinateInput input)
		{
			var errorMessage = input.Validate(this.SessionWrapper.LoginShopId);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return Json(errorMessage, JsonRequestBehavior.AllowGet);
			}
			this.Service.InsertUpdateCoordinate(actionStatus, input.CreateModel());

			return Json(errorMessage, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 新規登録
		/// </summary>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Register()
		{
			if (this.ModelState.IsValid) this.ModelState.Clear();
			return Detail(this.Service.Register());
		}

		/// <summary>
		/// コピー新規作成
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult CopyPage(string coordinateId, ActionStatus actionStatus)
		{
			if (this.ModelState.IsValid) this.ModelState.Clear();
			var vm = this.Service.CopyPage(coordinateId);
			vm.ActionStatus = actionStatus;

			return PartialView("Detail", vm);
		}

		/// <summary>
		/// 削除アクション
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>アクション結果</returns>
		public ActionResult Delete(string coordinateId)
		{
			this.Service.Delete(coordinateId);
			return null;
		}

		/// <summary>
		/// アップロード
		/// </summary>
		/// <param name="files">画像</param>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>アクション結果</returns>
		public ActionResult Upload(HttpPostedFileBase files, string coordinateId)
		{
			var errorMessage =this.Service.Upload(files, coordinateId);

			return Content(errorMessage);
		}

		/// <summary>
		/// 画像リストからコピー
		/// </summary>
		/// <param name="path">パス</param>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>アクション結果</returns>
		public ActionResult CopyFromImageList(string path, string coordinateId)
		{
			var errorMessage = this.Service.CopyFromImageList(path, coordinateId);

			return Content(errorMessage);
		}

		/// <summary>
		/// プレビュー表示検証
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <returns>エラーメッセージ</returns>
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Validate(CoordinateInput input)
		{
			var message = this.Service.Validate(input);
			return Json(message, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// プレビュー生成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="input">入力内容</param>
		/// <returns>プレビューURL</returns>
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Preview(ActionStatus actionStatus, CoordinateInput input)
		{
			// プレビュー画面生成 URL返却
			var url = this.Service.Preview(actionStatus, input);
			return Json(url, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// リアル店舗IDをセット
		/// </summary>
		/// <param name="staffId">スタッフID</param>
		/// <returns>リアルショップID</returns>
		public ActionResult SetRealShopId(string staffId)
		{
			if (string.IsNullOrEmpty(staffId)) return Content(string.Empty);
			var staff = new StaffService().Get(staffId);
			return Content((staff != null) ? staff.RealShopId : "");
		}

		/// <summary>
		/// スタッフ名をセット
		/// </summary>
		/// <param name="staffId">スタッフID</param>
		/// <returns>スタッフ名</returns>
		public ActionResult SetStaffName(string staffId)
		{
			if (string.IsNullOrEmpty(staffId)) return Content(string.Empty);
			var staff = new StaffService().Get(staffId);
			var name = ((staff != null) ? HtmlSanitizer.HtmlEncode(staff.StaffName) : "");

			return Content("<a href=\"javascript:open_staff(\'" + staffId + "\')\">" + name + "</a>");
		}

		/// <summary>
		/// リアル店舗名をセット
		/// </summary>
		/// <param name="realShopId">リアル店舗ID</param>
		/// <returns>リアル店舗Html</returns>
		public ActionResult SetRealShopName(string realShopId)
		{
			if (string.IsNullOrEmpty(realShopId)) return Content(string.Empty);
			var realShop= new RealShopService().Get(realShopId);
			var name = ((realShop != null) ? realShop.Name : "");

			return Content("<a href=\"javascript:open_real_shop(\'" + realShopId + "\')\">" + name + "</a>");
			
		}

		/// <summary>
		/// 画像を削除
		/// </summary>
		/// <param name="imageNo">イメージNo</param>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>アクション結果</returns>
		public ActionResult DeleteImage(int imageNo, string coordinateId)
		{
			var errorMessage = this.Service.DeleteImage(imageNo, coordinateId);
			return Content(errorMessage);
		}

		/// <summary>
		/// 画像の順番を変更
		/// </summary>
		/// <param name="order">順番</param>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns></returns>
		public ActionResult UpdateImageSort(string[] order, string coordinateId)
		{
			var errorMessage = this.Service.UpdateImageSort(order, coordinateId);
			return Content(errorMessage);
		}

		/// <summary>
		/// 商品IDのリストを受け取って画面に商品情報の一覧をセット
		/// </summary>
		/// <param name="productIds">商品IDのリスト</param>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="variationIds">バリエーションID</param>
		/// <returns>結果</returns>
		public ActionResult SetVariationProductList(string baseName, string[] productIds, string[] variationIds = null)
		{
			var list = new List<ProductViewModel>();
			var sortNo = 0;
			var errorProductIdList = new List<string>();
			foreach (var productId in productIds)
			{
				var model = new ProductService().Get(this.SessionWrapper.LoginShopId, productId);
				if (model == null) errorProductIdList.Add(productId);
				if (errorProductIdList.Count != 0) continue;
				model.VariationId = (variationIds != null && string.IsNullOrEmpty(variationIds[sortNo]) == false) ? variationIds[sortNo] : string.Empty;
				var pvm =this.Service.GetVariationProductVm(model, sortNo);
				sortNo++;
				list.Add(pvm);
			}

			if (errorProductIdList.Count != 0)
			{
				var errorString = string.Join(",", errorProductIdList);
				return Json("product_search_error('" + errorString + "')", JsonRequestBehavior.AllowGet);
			}

			var vm = ProductInputView(baseName, "0", list.ToArray());
			return vm;
		}

		/// <summary>
		/// バリエーションの変更
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>結果</returns>
		public ActionResult ChangeVariation(CoordinateInput input)
		{
			var productList = input.ProductInput[0].ProductList;
			var separatedProductIds ="";
			var separatedVariationIds = "";
			var comma = ',';
			foreach (var product in productList)
			{
				separatedProductIds += product.Id + comma;
				separatedVariationIds += product.VariationId + comma;
			}

			var productIds = separatedProductIds.Substring(0, (separatedProductIds.Length-1)).Split(comma);
			var variationIds = separatedVariationIds.Substring(0, (separatedVariationIds.Length-1)).Split(comma);

			return SetVariationProductList("input.ProductInput[0]", productIds, variationIds);
		}

		/// <summary>
		/// 総件数の取得
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult GetCoordinateCount(CoordinateParamModel pm)
		{
			var total = this.Service.GetCoordinateCount(pm);
			return Content(total.ToString());
		}

		/// <summary>
		/// エクスポート
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Main")]
		[Button(ButtonName = "Export")]
		public ActionResult Export(CoordinateParamModel pm)
		{
			var fileData = this.Service.Export(pm);

			// エラーがあればエラー画面へ
			if (string.IsNullOrEmpty(fileData.Error) == false)
			{
				return CreateErrorPageAction(fileData.Error);
			}
			var actionResult = fileData.CreateActionResult();
			return actionResult;
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public string ValidateExport(CoordinateParamModel pm)
		{
			var str = (string.IsNullOrEmpty(pm.DataExportType))
				? WebMessages.MasterexportSettingSettingIdNotSelected
				: string.Empty;
			return str;
		}

		/// <summary>サービス</summary>
		private CoordinateWorkerService Service { get { return GetDefaultService<CoordinateWorkerService>(); } }
	}
}
