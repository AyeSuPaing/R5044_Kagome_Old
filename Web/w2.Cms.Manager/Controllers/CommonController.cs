/*
=========================================================================================================
  Module      : コントローラ基底クラス(BaseController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Translation;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Util;
using w2.Domain.NameTranslationSetting;
using w2.Domain.Product;
using w2.Domain.Product.Helper;
using w2.Domain.ProductCategory;
using w2.Domain.ProductGroup;
using w2.Domain.ProductStock;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// コントローラ基底クラス
	/// </summary>
	[ValidateInput(false)]
	public abstract class CommonController : Controller, IController
	{
		#region -NameTranslationSettingExport 名称翻訳情報CSVエクスポート処理
		/// <summary>
		/// 名称翻訳情報CSVエクスポート処理
		/// </summary>
		/// <returns>結果</returns>
		public ActionResult NameTranslationSettingExport()
		{
			var exportTargetData = GetExportTranslationData();
			var csvFormatters = (exportTargetData != null)
				? exportTargetData.Select(s => new NameTranslationCsvOutputFormatter(s)).ToArray()
				: new NameTranslationCsvOutputFormatter[0];

			var content = (csvFormatters.FirstOrDefault() ?? new NameTranslationCsvOutputFormatter()).OutputCsvHeader()
				+ Environment.NewLine + string.Join(
					Environment.NewLine,
					csvFormatters.Select(csvFormatter => csvFormatter.FormatCsvLine()).ToArray());
			var fileName =
				ValueText.GetValueText(Constants.TABLE_NAMETRANSLATIONSETTING, "export_file_name", string.Empty)
				+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
			return File(Encoding.UTF8.GetBytes(content), "text/csv", fileName);
		}
		#endregion

		#region -GetExportTranslationData エクスポート対象名称翻訳設定データ取得
		/// <summary>
		/// エクスポート対象名称翻訳設定データ取得
		/// </summary>
		/// <returns>エクスポート対象データ</returns>
		private List<NameTranslationSettingModel> GetExportTranslationData()
		{
			var exportTargetData = new List<NameTranslationSettingModel>();
			var translationData = new NameTranslationSettingService().GetTranslationSettingsByListPage(
				this.SessionWrapper.TranslationExportTargetDataKbn,
				this.SessionWrapper.TranslationSearchCondition);
			if (translationData != null) exportTargetData.AddRange(translationData);

			return exportTargetData;
		}
		#endregion

		/// <summary>
		/// 商品IDのリストを受け取って画面に商品情報の一覧をセット
		/// </summary>
		/// <param name="productIds">商品IDのリスト</param>
		/// <param name="modelNo">商品一覧モデル識別番号</param>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="addRootToImg">画像にルートを付けるか</param>
		/// <returns>結果</returns>
		public virtual ActionResult SetProductList(string baseName, string modelNo, string[] productIds, bool addRootToImg =false)
		{
			var service = new ProductService();
			var stockService = new ProductStockService();
			var list = new List<ProductViewModel>();
			var i = 0;

			foreach (var productId in productIds)
			{
				var model = service.Get(this.SessionWrapper.LoginShopId, productId);

				if (model == null) return Json("product_search_error('" + productId + "')", JsonRequestBehavior.AllowGet);

				var pvm = new ProductViewModel
				{
					Id = model.ProductId,
					Name = model.Name,
					ImagePath = CreateImagePath(model.ImageHead, addRootToImg),
					Stock = stockService.GetSum(this.SessionWrapper.LoginShopId, model.ProductId),
					SortNo = ++i,
				};
				list.Add(pvm);
			}

			var vm = ProductInputView(baseName, modelNo, list.ToArray());
			return vm;
		}

		/// <summary>
		/// 画像パスを作成
		/// </summary>
		/// <param name="imageHead">画像ヘッド</param>
		/// <param name="addRootToImg">画像にルートを付けるか</param>
		/// <returns>画像パス</returns>
		public string CreateImagePath(string imageHead,bool addRootToImg)
		{
			string imagePath;
			if (addRootToImg)
			{
				imagePath = string.IsNullOrEmpty(imageHead)
					? string.Format("{0}{1}{2}", Constants.PATH_ROOT_FRONT_PC, Constants.PATH_PRODUCTIMAGES, Constants.PRODUCTIMAGE_NOIMAGE_PC)
					: string.Format(
						"{0}{1}0/{2}{3}",
						Constants.PATH_ROOT_FRONT_PC,
						Constants.PATH_PRODUCTIMAGES,
						imageHead,
						Constants.PRODUCTIMAGE_FOOTER_M);
			}
			else
			{
				imagePath = string.IsNullOrEmpty(imageHead)
					? string.Format("{0}{1}", Constants.PATH_PRODUCTIMAGES, Constants.PRODUCTIMAGE_NOIMAGE_PC)
					: string.Format(
						"{0}0/{1}{2}",
						Constants.PATH_PRODUCTIMAGES,
						imageHead,
						Constants.PRODUCTIMAGE_FOOTER_M);
			}

			return imagePath;
		}

		/// <summary>
		/// 商品入力ビュー
		/// </summary>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="modelNo">商品一覧モデル識別番号</param>
		/// <param name="list">商品ビューモデル</param>
		/// <returns>アクション結果</returns>
		public abstract ActionResult ProductInputView(string baseName, string modelNo, ProductViewModel[] list);

		/// <summary>
		/// 商品グループIDを受け取って画面に商品情報の一覧をセット
		/// </summary>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="modelNo">商品一覧モデル識別番号</param>
		/// <param name="groupId">グループID</param>
		/// <returns>結果</returns>
		public ActionResult SetProductGroup(string baseName, string modelNo, string groupId)
		{
			var service = new ProductGroupService();
			var productService = new ProductService();
			var stockService = new ProductStockService();
			var group = service.Get(groupId);

			var vm = ProductInputView(
				baseName,
				modelNo,
				group.Items.Select(
					item =>
					{
						var models = productService.Get(this.SessionWrapper.LoginShopId, item.MasterId);

						if (models == null) return null;

						return new ProductViewModel
						{
							Name = models.Name,
							Id = models.ProductId,
							ImagePath = string.IsNullOrEmpty(models.ImageHead)
								? string.Format(
									"{0}{1}",
									Constants.PATH_PRODUCTIMAGES,
									Constants.PRODUCTIMAGE_NOIMAGE_PC)
							: string.Format(
								"{0}0/{1}{2}",
								Constants.PATH_PRODUCTIMAGES,
									models.ImageHead,
								Constants.PRODUCTIMAGE_FOOTER_M),
							Stock = stockService.GetSum(this.SessionWrapper.LoginShopId, models.ProductId),
						SortNo = item.ItemNo,
						};
					}).Where(item => item != null).ToArray(), group);
			return vm;
		}

		/// <summary>
		/// 商品入力ビュー
		/// </summary>
		/// <param name="baseName">バインド時の名前</param>
		/// <param name="modelNo">商品一覧モデル識別番号</param>
		/// <param name="list">商品ビューモデル</param>
		/// <param name="group">商品グループモデル</param>
		/// <returns>アクション結果</returns>
		public abstract ActionResult ProductInputView(string baseName, string modelNo, ProductViewModel[] list, ProductGroupModel group);

		/// <summary>
		/// 総件数を取得
		/// </summary>
		/// <param name="paramModel">パラムモデル</param>
		/// <returns>総件数</returns>
		public virtual int GetSearchHitCountOnCms(ProductSearchParamModel paramModel)
		{
			paramModel.BeginRowNumber = (paramModel.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
			paramModel.EndRowNumber = paramModel.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;

			var count = new ProductService().GetSearchHitCountOnCms(paramModel, this.SessionWrapper.LoginShopId);
			return count;
		}

		/// <summary>
		/// 絞り込み検索
		/// </summary>
		/// <param name="paramModel">検索条件</param>
		/// <returns>結果</returns>
		public virtual ActionResult ProductSearch(ProductSearchParamModel paramModel)
		{
			paramModel.BeginRowNumber = (paramModel.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
			paramModel.EndRowNumber = paramModel.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;
			var totalCount = new ProductService().GetSearchHitCountOnCms(paramModel, this.SessionWrapper.LoginShopId);
			var countHtml = paramModel.BeginRowNumber + "-"
				+ ((totalCount > paramModel.EndRowNumber) ? StringUtility.ToNumeric(paramModel.EndRowNumber) : StringUtility.ToNumeric(totalCount))
				+ "/" + StringUtility.ToNumeric(totalCount);

			var productList = new ProductService().SearchOnCms(paramModel, this.SessionWrapper.LoginShopId);
			var service = new ProductCategoryService();
			var vm = productList.Select(
				pl => new ProductSearchResultModel
				{
					ProductName = pl.Name,
					VariationName = "",
					ProductId = pl.ProductId,
					VariationId = pl.ProductId,
					CategoryName = string.IsNullOrEmpty(pl.CategoryId1) == false
						? service.Get(pl.CategoryId1) != null ? service.Get(pl.CategoryId1).Name : string.Empty
						: string.Empty,
					Price = pl.DisplayPrice.ToPriceString(true),
					SellStartDate = (pl.SellFrom).ToString("yyyy/MM/dd"),
					CountHtml = countHtml
				}).ToArray();
			return PartialView("_ProductSearchResult", vm);
		}

		/// <summary>
		/// セッションラッパー更新
		/// </summary>
		/// <param name="sessionWrapper">セッションラッパー</param>
		public abstract void UpdateSessionWrapper(SessionWrapper sessionWrapper);

		/// <summary>セッションラッパー</summary>
		public SessionWrapper SessionWrapper { get; protected set; }
	}
}
