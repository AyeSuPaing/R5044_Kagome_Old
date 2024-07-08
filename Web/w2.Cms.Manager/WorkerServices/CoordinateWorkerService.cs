/*
=========================================================================================================
  Module      : コーディネートワーカーサービス(CoordinateWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using w2.App.Common.Order;
using w2.App.Common.Preview;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.Coordinate;
using w2.Cms.Manager.ViewModels.Coordinate;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.ContentsLog.Helper;
using w2.Domain.ContentsTag;
using w2.Domain.Coordinate;
using w2.Domain.Coordinate.Helper;
using w2.Domain.CoordinateCategory;
using w2.Domain.CoordinateCategory.Helper;
using w2.Domain.Product;
using w2.Domain.ProductStock;
using w2.Domain.RealShop;
using w2.Domain.Staff;
using w2.Domain.User;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// コーディネートワーカーサービス
	/// </summary>
	public class CoordinateWorkerService : BaseWorkerService
	{
		/// <summary>コーディネート画像パス</summary>
		public static string m_coordinatePath = @"Contents\Coordinate\";
		/// <summary>コーディネート画像パス</summary>
		public static string m_coordinateTemporaryPath = @"Contents\Temp\";
		/// <summary>コーディネート画像ルート</summary>
		public string m_coordinateRoot = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, m_coordinatePath);
		/// <summary>コーディネート画像ルート</summary>
		public string m_coordinateTemporaryRoot = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, m_coordinateTemporaryPath);
		/// <summary>商品価格で並び替え</summary>
		public const string SORT_PRICE = "1";
		/// <summary>商品IDで並び替え</summary>
		public const string SORT_ID = "2";
		/// <summary>カテゴリIDで並び替え</summary>
		public const string SORT_CATEGORY = "3";
		/// <summary>ブランドIDで並び替え</summary>
		public const string SORT_BRAND = "4";
		/// <summary>商品名で並び替え</summary>
		public const string SORT_NAME = "5";
		/// <summary>表示優先順で並び替え</summary>
		public const string SORT_DISPLAY_PRIORITY = "6";
		/// <summary>販売開始日で並び替え</summary>
		public const string SORT_SELL_FROM = "7";

		/// <summary>
		/// コーディネートビューモデル作成
		/// </summary>
		/// <returns>ビューモデル</returns>
		public CoordinatePageViewModel CoordinatePageVm()
		{
			return new CoordinatePageViewModel();
		}

		/// <summary>
		/// コーディネートビューモデル作成
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(CoordinateParamModel pm)
		{
			var searchCondition = CreateSearchCondtion(pm);
			var count = new CoordinateService().GetSearchHitCount(searchCondition);
			var list = new CoordinateService().Search(searchCondition).ToList();
			if (count == 0)
			{
				return new ListViewModel()
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError,
				};
			}
			list = list.GroupBy(u => u.CoordinateId).Where(u => u.Any()).Distinct().Select(u => u.FirstOrDefault()).ToList();
			// 画像や項目の紐づけ
			list = SetItem(list);

			return new ListViewModel()
			{
				List = list,
				ParamModel = pm
			};
		}

		/// <summary>
		/// 総件数を取得
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns></returns>
		public int GetCoordinateCount(CoordinateParamModel pm)
		{
			var searchCondition = CreateSearchCondtion(pm);
			var count = new CoordinateService().GetSearchHitCount(searchCondition);
			return count;
		}

		/// <summary>
		/// 検索条件を作成
		/// </summary>
		/// <param name="pm">パラムモデル</param>
		/// <returns>検索条件</returns>
		public CoordinateListSearchCondition CreateSearchCondtion(CoordinateParamModel pm)
		{
			var searchCondition = new CoordinateListSearchCondition
			{
				Keyword0 = pm.SearchKeyword,
				Staff = pm.SearchStaff,
				RealShop = pm.SearchRealShop,
				Category = pm.SearchCategory,
				DisplayDateKbn = pm.DisplayDateKbn,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
			};

			return searchCondition;
		}

		/// <summary>
		/// コーディネートカテゴリビューモデル作成
		/// </summary>
		/// <param name="searchCategoryModal">カテゴリモーダル検索値</param>
		/// <returns>ビューモデル</returns>
		public CategoryListViewModel CreateCategoryListVm(string searchCategoryModal)
		{
			var searchCondition = new CoordinateCategoryListSearchCondition
			{
				EndRowNumber = 2147483647,
				CoordinateCategory = searchCategoryModal
			};

			var list = new CoordinateCategoryService().Search(searchCondition).ToList();
			if (list.Count == 0)
			{
				return new CategoryListViewModel()
				{
					ErrorMessage = WebMessages.NoHitListError,
				};
			}

			return new CategoryListViewModel()
			{
				List = list,
			};
		}

		/// <summary>
		/// 項目をセット
		/// </summary>
		/// <param name="list">検索結果</param>
		/// <returns>検索結果</returns>
		public List<CoordinateListSearchResult> SetItem(List<CoordinateListSearchResult> list)
		{
			var coordinateService = new CoordinateService();
			var userService = new UserService();
			var logService = new ContentsLogService();
			foreach (var model in list)
			{
				// 項目の処理
				var items = coordinateService.GetWithChilds(model.CoordinateId, this.SessionWrapper.LoginShopId);
				model.CategoryList = items.CategoryList;
				model.TagList = items.TagList;
				model.ProductList = items.ProductList;
				model.LikeCount = userService.GetUserActivityCountForManager(Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE, model.CoordinateId);
				model.ContentsSummaryData = new ContentsSummaryData();
				var summary = logService.GetContentsSummaryData(
					Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_COORDINATE,
					model.CoordinateId);
				var summaryToday = logService.GetContentsSummaryDataOfToday(
					Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_COORDINATE,
					model.CoordinateId);
				model.ContentsSummaryData.PvCount = ((summary.Any()) ? summary[0].PvCount : 0) + ((summaryToday.Any()) ? summaryToday[0].PvCount : 0);
				model.ContentsSummaryData.CvCount = ((summary.Any()) ? summary[0].CvCount : 0) + ((summaryToday.Any()) ? summaryToday[0].CvCount : 0);
				model.ContentsSummaryData.Price =((summary.Any()) ? summary[0].Price : 0) + ((summaryToday.Any()) ? summaryToday[0].Price : 0);
			}
			return list;
		}

		/// <summary>
		/// コーディネート詳細ビューモデル作成
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>ビューモデル</returns>
		public DetailViewModel CreateDetailVm(string coordinateId)
		{
			InitializeImageFolder(coordinateId);
			return new DetailViewModel(coordinateId, this.SessionWrapper.LoginShopId);
		}

		/// <summary>
		/// 画像フォルダを初期化
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		public void InitializeImageFolder(string coordinateId)
		{
			var sourceDir = Path.Combine(m_coordinateRoot, coordinateId);
			if (Directory.Exists(sourceDir) == false) Directory.CreateDirectory(sourceDir);

			var tempDir = Path.Combine(
				m_coordinateTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_COORDINATE,
				coordinateId);
			if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
			Directory.CreateDirectory(tempDir);
			CopyDirectory(sourceDir, tempDir);
		}

		/// <summary>
		/// 商品の表示順を変更
		/// </summary>
		/// <param name="productIds">商品IDのリスト</param>
		/// <param name="sortType">並び順</param>
		/// <param name="baseName">バインド時の名前</param>
		/// <returns>ビューモデル</returns>
		public ProductSortModel UpdateProductSort(string[] productIds, string sortType, string baseName)
		{
			var service = new ProductService();
			var models = productIds.Select(id => service.Get(this.SessionWrapper.LoginShopId, id));

			switch (sortType)
			{
				case SORT_PRICE:
					models = models.OrderBy(m => m.DisplayPrice);
					break;

				case SORT_ID:
					models = models.OrderBy(m => m.ProductId);
					break;

				case SORT_CATEGORY:
					models = models.OrderBy(m => m.CategoryId1);
					break;

				case SORT_BRAND:
					models = models.OrderBy(m => m.BrandId1);
					break;

				case SORT_NAME:
					models = models.OrderBy(m => m.Name);
					break;

				case SORT_DISPLAY_PRIORITY:
					models = models.OrderBy(m => m.DisplayPriority);
					break;

				case SORT_SELL_FROM:
					models = models.OrderBy(m => m.SellFrom);
					break;
			}

			var list = new List<ProductViewModel>();
			var sortNo = 0;

			foreach (var model in models)
			{
				var pvm = GetVariationProductVm(model, sortNo);
				sortNo++;
				list.Add(pvm);
			}

			var vm = new ProductSortModel
			{
				ProductList = list.ToArray(),
				BaseName = baseName
			};

			return vm;
		}

		/// <summary>
		/// アップロード済画像ビューモデルを作成
		/// </summary>
		/// <param name="coordinateId"></param>
		/// <returns></returns>
		public UploadedImageViewModel CreateUploadedImageVm(string coordinateId)
		{
			var vm = new UploadedImageViewModel(coordinateId, this.SessionWrapper.LoginOperator.OperatorId);
			return vm;
		}

		/// <summary>
		/// カテゴリ入力ビューモデルを作成
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>カテゴリビューモデル</returns>
		public CategoryViewModel CreateCategoryVm(string coordinateId, ActionStatus actionStatus)
		{
			// コピー新規作成時はコピー元のコーディネートIDで取得
			var vm = new CategoryViewModel(
				actionStatus != ActionStatus.CopyInsert ? coordinateId : this.SessionWrapper.CoordinateId,
				this.SessionWrapper.LoginShopId);

			return vm;
		}

		/// <summary>
		/// カテゴリ入力ビューモデルを作成
		/// </summary>
		/// <param name="categories">カテゴリ</param>
		/// <returns>カテゴリビューモデル</returns>
		public CategoryViewModel ChangeCategoryVm(string categories)
		{
			var vm = new CategoryViewModel(categories);
			return vm;
		}

		/// <summary>
		/// ディレクトリをコピーする
		/// </summary>
		/// <param name="sourceDirName">コピーするディレクトリ</param>
		/// <param name="destDirName">コピー先のディレクトリ</param>
		public static void CopyDirectory(string sourceDirName, string destDirName)
		{
			//コピー先のディレクトリがないときは作る
			if (Directory.Exists(destDirName) == false)
			{
				Directory.CreateDirectory(destDirName);
				//属性もコピー
				File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
			}

			//コピー先のディレクトリ名の末尾に"\"をつける
			if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
				destDirName = destDirName + Path.DirectorySeparatorChar;

			//コピー元のディレクトリにあるファイルをコピー
			var files = Directory.GetFiles(sourceDirName);
			foreach (string file in files)
			{
				File.Copy(file, destDirName + Path.GetFileName(file), true);
			}

			//コピー元のディレクトリにあるディレクトリについて、再帰的に呼び出す
			var dirs = Directory.GetDirectories(sourceDirName);
			foreach (var dir in dirs)
			{
				CopyDirectory(dir, destDirName + Path.GetFileName(dir));
			}
		}

		/// <summary>
		/// コピー新規作成
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>ビューモデル</returns>
		public DetailViewModel CopyPage(string coordinateId)
		{
			var vm = CreateDetailVm(coordinateId);
			var newId = Register();
			this.SessionWrapper.CoordinateId = coordinateId;
			InitializeImageFolder(newId);
			CopyImage(coordinateId, newId);

			new DetailViewModel().CoordinateId = newId;
			vm.CoordinateId = newId;
			vm.CoordinateTitle += Constants.COPY_NEW_SUFFIX;
			vm.DisplayKbn = Constants.FLG_COORDINATE_DISPLAY_KBN_DRAFT;
			vm.CoordinateUrl = String.Empty;

			return vm;
		}

		/// <summary>
		/// 画像コピー
		/// </summary>
		/// <param name="copySourceCoordinateId">コピー元コーディネートID</param>
		/// <param name="copyTargetCoordinateId">コピー先コーディネートID</param>
		private void CopyImage(string copySourceCoordinateId, string copyTargetCoordinateId)
		{
			var sourceDir = Path.Combine(m_coordinateRoot, copySourceCoordinateId);
			var target = Path.Combine(
				m_coordinateTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_COORDINATE,
				copyTargetCoordinateId);
			var files = Directory.GetFiles(sourceDir);

			try
			{
				foreach (string file in files)
				{
					var uploadPath = CreateUploadPath(target, copyTargetCoordinateId);
					File.Copy(file, uploadPath);
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <returns>コーディネートID</returns>
		public string Register()
		{
			var newId = NumberingUtility.CreateKeyId(
				this.SessionWrapper.LoginShopId,
				Constants.NUMBER_KEY_CMS_COORDINATE_ID,
				Constants.CONST_COORDINATE_ID_LENGTH);
			var staff = new StaffService().GetByOperatorId(this.SessionWrapper.LoginOperator.OperatorId).FirstOrDefault(s => s.IsModel);
			var staffId = string.Empty;
			var realShopId = string.Empty;
			if (staff != null)
			{
				staffId = staff.StaffId;
				var shop = new RealShopService().Get(staff.RealShopId);
				if (shop != null) realShopId = shop.RealShopId;
			}

			var model = new CoordinateModel()
			{
				CoordinateTitle = "",
				CoordinateId = newId,
				LastChanged = this.SessionWrapper.LoginOperatorName,
				CoordinateUrl = CreateCoordinateDetailUrl(newId),
				StaffId = staffId,
				RealShopId = realShopId
			};

			if (Directory.Exists(m_coordinateRoot) == false) Directory.CreateDirectory(m_coordinateRoot);
			Directory.CreateDirectory(Path.Combine(m_coordinateRoot, newId));

			return model.CoordinateId;
		}

		/// <summary>
		/// 詳細画面更新
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="coordinateModel">コーディネート</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>コーディネートID</returns>
		public string InsertUpdateCoordinate(ActionStatus actionStatus, CoordinateModel coordinateModel, SqlAccessor accessor = null)
		{
			coordinateModel.LastChanged = this.SessionWrapper.LoginOperatorName;
			coordinateModel.CoordinateUrl = CreateCoordinateDetailUrl(coordinateModel.CoordinateId);

			switch (actionStatus)
			{
				case ActionStatus.Insert:
				case ActionStatus.CopyInsert:
					// 登録処理
					new CoordinateService().Insert(coordinateModel, accessor);
					break;

				case ActionStatus.Update:
					// 更新処理
					new CoordinateService().Update(coordinateModel, accessor);
					break;
			}

			// 紐づけ処理
			LinkItem(coordinateModel, accessor);

			// 画像をメインフォルダにコピー(プレビュー時はコピーしない)
			if (accessor == null)
			{
				CopyImagesToMainFolder(coordinateModel);
			}

			return coordinateModel.CoordinateId;
		}

		/// <summary>
		/// 画像をメインフォルダにコピー
		/// </summary>
		/// <param name="coordinateModel">コーディネート</param>
		public void CopyImagesToMainFolder(CoordinateModel coordinateModel)
		{
			var tempDir = Path.Combine(
				m_coordinateTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_COORDINATE,
				coordinateModel.CoordinateId);
			var sourceDir = Path.Combine(m_coordinateRoot, coordinateModel.CoordinateId);
			if (Directory.Exists(tempDir) && Directory.Exists(sourceDir))
			{
				try
				{
					Directory.Delete(sourceDir, true);
					CopyDirectory(tempDir, sourceDir);
					Directory.Delete(tempDir, true);
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					InitializeImageFolder(coordinateModel.CoordinateId);
				}
			}
		}

		/// <summary>
		/// 項目を紐づける
		/// </summary>
		/// <param name="coordinateModel">コーディネート</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void LinkItem(CoordinateModel coordinateModel, SqlAccessor accessor = null)
		{
			var coordinateService = new CoordinateService();
			coordinateService.DeleteItem(coordinateModel.CoordinateId, accessor);

			// タグの処理
			if (coordinateModel.TagList != null)
			{
				var tagService = new ContentsTagService();
				foreach (var tag in coordinateModel.TagList)
				{
					if (string.IsNullOrEmpty(tag.ContentsTagName) == false)
					{
						if (tagService.GetByName(tag.ContentsTagName) == null)
						{
							tagService.Insert(
								new ContentsTagModel()
								{
									ContentsTagName = tag.ContentsTagName
								});
						}

						var item = new CoordinateItemModel
						{
							CoordinateId = coordinateModel.CoordinateId,
							ItemId = tagService.GetByName(tag.ContentsTagName).ContentsTagId.ToString(),
							ItemId2 ="",
							ItemName = tag.ContentsTagName,
							ItemKbn = Constants.FLG_COORDINATE_ITEM_KBN_TAG,
							LastChanged = coordinateModel.LastChanged
						};
						coordinateService.InsertCoordinateItem(item, accessor);
					}
				}
			}

			// カテゴリの処理
			if (coordinateModel.CategoryList != null)
			{
				foreach (var category in coordinateModel.CategoryList)
				{
					if (string.IsNullOrEmpty(category.CoordinateCategoryId) == false)
					{
						var item = new CoordinateItemModel
						{
							CoordinateId = coordinateModel.CoordinateId,
							ItemId = category.CoordinateCategoryId,
							ItemId2 = "",
							ItemName = category.CoordinateCategoryName,
							ItemKbn = Constants.FLG_COORDINATE_ITEM_KBN_CATEGORY,
							LastChanged = coordinateModel.LastChanged
						};
						coordinateService.InsertCoordinateItem(item, accessor);
					}
				}
			}

			// 商品の処理
			if (coordinateModel.ProductList != null)
			{
				foreach (var model in coordinateModel.ProductList)
				{
					if (string.IsNullOrEmpty(model.ProductId)) continue;

					var item = new CoordinateItemModel
					{
						CoordinateId = coordinateModel.CoordinateId,
						ItemId = model.ProductId,
						ItemId2 = (model.UseVariationFlg == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE)
							? model.VariationId
							: model.ProductId,
						ItemName = model.Name,
						ItemKbn = Constants.FLG_COORDINATE_ITEM_KBN_PRODUCT,
						LastChanged = coordinateModel.LastChanged
					};

					coordinateService.InsertCoordinateItem(item, accessor);
				}
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		public void Delete(string coordinateId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				var service = new CoordinateService();
				var userService = new UserService();
				service.DeleteItem(coordinateId, sqlAccessor);
				service.Delete(coordinateId, sqlAccessor);
				userService.DeleteUserActivityForManager(Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE,coordinateId, sqlAccessor);
				var target = Path.Combine(m_coordinateRoot, coordinateId);
				var tempTarget = Path.Combine(m_coordinateTemporaryRoot, Constants.PATH_TEMP_COORDINATE, coordinateId);
				if (Directory.Exists(target)) Directory.Delete(target, true);
				if (Directory.Exists(tempTarget)) Directory.Delete(tempTarget, true);

				sqlAccessor.CommitTransaction();
			}
		}

		/// <summary>
		/// 画像取得
		/// </summary>
		/// <param name="fileName">ファイルネーム</param>
		/// <param name="dir">ディレクトリ</param>
		/// <returns>画像</returns>
		public static string GetImage(string fileName, string dir)
		{
			// 条件判断して画像ファイル名などを決定
			var imageFileUrlPart = File.Exists(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, dir, fileName))
				? fileName
				: Path.Combine("..",Constants.PRODUCTIMAGE_NOIMAGE_PC_LL);

			// 画像パス作成
			var rootPath = Constants.PATH_ROOT_FRONT_PC;
			var pathTmp = Path.Combine(rootPath, dir, imageFileUrlPart);
			var path = rootPath.StartsWith("/")
				? CreateImageCnvUrl(pathTmp, Constants.KBN_IMAGEFORMAT_JPG, 500)
				: pathTmp;
			return path;
		}

		/// <summary>
		/// イメージコンバータURLの作成
		/// </summary>
		/// <param name="imageFileUrl">商品画像URL</param>
		/// <param name="format">拡張子</param>
		/// <param name="width">サイズ</param>
		/// <returns>イメージコンバータ経由で参照可能なURL</returns>
		private static string CreateImageCnvUrl(string imageFileUrl, string format, int width)
		{
			var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_IMAGECONVERTER)
				.AddParam(Constants.REQUEST_KEY_IMGCNV_FILE, imageFileUrl)
				.AddParam(Constants.REQUEST_KEY_IMGCNV_FORMAT, format).AddParam(
					Constants.REQUEST_KEY_IMGCNV_SIZE,
					width.ToString()).CreateUrl();
			return url;
		}

		/// <summary>
		/// 商品画像URL取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>商品画像</returns>
		public static string CreateProductImageUrl(string productId, string variationId = null)
		{
			// 店舗ID
			var shopId = new SessionWrapper().LoginShopId;
			// ファイル名ヘッダ
			var service = new ProductService();
			string imageFileNameHead;
			if(string.IsNullOrEmpty(variationId) == false)
			{
				var variation = service.GetProductVariation(shopId, productId, variationId, string.Empty);
				imageFileNameHead = (variation != null) 
					? variation.VariationImageHead
					: string.Empty;
			}
			else
			{
				var product = service.Get(shopId, productId);
				imageFileNameHead = (product != null)
					? product.ImageHead
					: string.Empty;
			}

			// 画像URL作成
			var imageUrl = new StringBuilder();
			imageUrl.Append(Constants.PATH_ROOT_FRONT_PC).Append(Constants.PATH_PRODUCTIMAGES).Append(shopId)
				.Append("/").Append(imageFileNameHead).Append(Constants.PRODUCTIMAGE_FOOTER_LL);
			if (File.Exists(HttpContext.Current.Server.MapPath(imageUrl.ToString())) == false)
			{
				// 画像無しの場合はNOIMAGE画像
				imageUrl = new StringBuilder();
				imageUrl.Append(Constants.PATH_ROOT_FRONT_PC).Append(Constants.PATH_PRODUCTIMAGES).Append(Constants.PRODUCTIMAGE_NOIMAGE_PC_LL);
			}
		

			return imageUrl.ToString();
		}

		/// <summary>
		/// 画像アップロード
		/// </summary>
		/// <param name="image">画像</param>
		/// <param name="coordinateId">コーディネートID</param>
		public string Upload(HttpPostedFileBase image, string coordinateId)
		{
			if (image == null) return WebMessages.CoordinatePageNotFileError;
			var target = Path.Combine(
				m_coordinateTemporaryRoot, 
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_COORDINATE,
				coordinateId);
			if (Directory.Exists(target) == false) return WebMessages.CoordinatePageNotFileError;
			if (Directory.EnumerateFileSystemEntries(target).Count() >= 100) return WebMessages.CoordinatePageFileMaxError;
			var uploadPath = CreateUploadPath(target, coordinateId);

			try
			{
				image.SaveAs(uploadPath);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// 画像リストからコピー
		/// </summary>
		/// <param name="path">パス</param>
		/// <param name="coordinateId">コーディネートID</param>
		public string CopyFromImageList(string path, string coordinateId)
		{
			var originalPath = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, path);
			if (File.Exists(originalPath) == false) return string.Empty;

			var target = Path.Combine(
				m_coordinateTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_COORDINATE,
				coordinateId);
			if (Directory.Exists(target) == false) return WebMessages.CoordinatePageNotFileError;
			if (Directory.EnumerateFileSystemEntries(target).Count() >= 100) return WebMessages.CoordinatePageFileMaxError;

			var uploadPath = CreateUploadPath(target, coordinateId);

			try
			{
				File.Copy(originalPath, uploadPath);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// アップロードパス名を作成
		/// </summary>
		/// <param name="target"></param>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>エラーメッセージ</returns>
		public string CreateUploadPath(string target, string coordinateId)
		{
			string fileName;
			if (Directory.EnumerateFileSystemEntries(target).Any() == false)
			{
				fileName = Constants.COORDINATEIMAGE_PREFIX + coordinateId + Constants.CONTENTS_IMAGE_FIRST;
			}
			else
			{
				fileName = Constants.COORDINATEIMAGE_PREFIX + coordinateId + (Directory.EnumerateFileSystemEntries(target).Count() + 1) + ".jpg";
			}
			var uploadPath = Path.Combine(target, fileName);

			return uploadPath;
		}

		/// <summary>
		/// 画像削除
		/// </summary>
		/// <param name="imageNo">イメージNo</param>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>エラーメッセージ</returns>
		public string DeleteImage(int imageNo, string coordinateId)
		{
			var targetDir = Path.Combine(
				m_coordinateTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_COORDINATE,
				coordinateId);
			var targetFile = Path.Combine(targetDir, Constants.COORDINATEIMAGE_PREFIX + coordinateId + imageNo + ".jpg");
			var errorMessage = string.Empty;
			if (File.Exists(targetFile))
			{
				try
				{
					File.Delete(targetFile);
					var count = Directory.EnumerateFileSystemEntries(targetDir).Count();
					if (count > 0)
					{
						var loopCount = count - imageNo;
						for (var i = 0; i <= loopCount; i++)
						{
							var beforeFileName = Path.Combine(
								targetDir,
								Constants.COORDINATEIMAGE_PREFIX + coordinateId + (imageNo + i + 1) + ".jpg");
							var afterFileName = Path.Combine(
								targetDir,
								Constants.COORDINATEIMAGE_PREFIX + coordinateId + (imageNo + i) + ".jpg");
							File.Move(beforeFileName, afterFileName);
						}
					}
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					InitializeImageFolder(coordinateId);
					return WebMessages.CoordinatePageDeleteFileError;
				}
			}
			else
			{
				errorMessage = WebMessages.CoordinatePageDeleteFileError;
			}

			return errorMessage;
		}

		/// <summary>
		/// 画像順序を更新
		/// </summary>
		/// <param name="order">順序</param>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>エラーメッセージ</returns>
		public string UpdateImageSort(string[] order, string coordinateId)
		{
			var targetDir = Path.Combine(
				m_coordinateTemporaryRoot,
				this.SessionWrapper.LoginOperator.OperatorId,
				Constants.PATH_TEMP_COORDINATE,
				coordinateId);
			var errorMessage = string.Empty;
			var loopCount = order.Length;

			try
			{
				for (var i = 0; i < loopCount; i++)
				{
					var imageNo = int.Parse(order[i]);
					var beforeFileName = Path.Combine(targetDir, Constants.COORDINATEIMAGE_PREFIX + coordinateId + imageNo + ".jpg");
					var afterFileName = Path.Combine(targetDir, "copy_" + Constants.COORDINATEIMAGE_PREFIX + coordinateId + (i + 1) + ".jpg");
					if (File.Exists(beforeFileName))
					{
						File.Copy(beforeFileName, afterFileName);
					}
				}

				for (var i = 0; i < loopCount; i++)
				{
					var beforeFileName = Path.Combine(targetDir, "copy_" + Constants.COORDINATEIMAGE_PREFIX + coordinateId + (i + 1) + ".jpg");
					var afterFileName = Path.Combine(targetDir, Constants.COORDINATEIMAGE_PREFIX + coordinateId + (i + 1) + ".jpg");
					if (File.Exists(afterFileName))
					{
						File.Delete(afterFileName);
						File.Move(beforeFileName, afterFileName);
					}
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				InitializeImageFolder(coordinateId);
				return WebMessages.CoordinatePageMoveFileError;
			}
			return errorMessage;
		}

		/// <summary>
		/// コーディネート詳細URL作成（フロント）
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>URL</returns>
		public static string CreateCoordinateDetailUrl(string coordinateId)
		{
			// URL作成
			var targetPageUrl = new StringBuilder();
			if (Constants.PATH_ROOT_FRONT_PC.StartsWith("http"))
			{
				targetPageUrl.Append(Constants.PATH_ROOT_FRONT_PC);
			}
			else
			{
				targetPageUrl.Append(Constants.PROTOCOL_HTTPS).Append(Constants.SITE_DOMAIN).Append(Constants.PATH_ROOT_FRONT_PC);
			}

			targetPageUrl.Append(Constants.PAGE_FRONT_COORDINATE_DETAIL);

			var urlCreator = new UrlCreator(targetPageUrl.ToString());
			urlCreator.AddParam(Constants.REQUEST_KEY_COORDINATE_ID, coordinateId);

			return urlCreator.CreateUrl();
		}

		/// <summary>
		/// プレビュー表示検証
		/// </summary>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(CoordinateInput input)
		{
			var message = input.Validate(this.SessionWrapper.LoginShopId);
			return message;
		}

		/// <summary>
		/// プレビュー表示
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <returns>プレビューURL</returns>
		public string Preview(ActionStatus actionStatus, CoordinateInput input)
		{
			var model = input.CreateModel();
			InsertCoordinateDetailPreview(actionStatus, model);
			var urlCreator = new UrlCreator(
				Path.Combine(Constants.PATH_ROOT_FRONT_PC, Constants.PAGE_FRONT_COORDINATE_DETAIL));
			urlCreator.AddParam(Constants.REQUEST_KEY_OPERATOR_ID, this.SessionWrapper.LoginOperator.OperatorId);
			urlCreator.AddParam(Constants.REQUEST_KEY_COORDINATE_ID, model.CoordinateId);
			return urlCreator.CreateUrl();
		}

		/// <summary>
		/// コーディネート詳細プレビュー情報登録
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="model">コーディネート</param>
		protected void InsertCoordinateDetailPreview(ActionStatus actionStatus, CoordinateModel model)
		{
			// プレビュー用データを以下の手順で取得
			// 1.登録・更新処理を実行
			// 2.対象データを取得
			// 3.「1.」の処理をロールバック
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				DataView coordinate;
				// トランザクション開始
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				try
				{
					// コーディネート更新&取得
					coordinate = new CoordinateService().GetForPreview(InsertUpdateCoordinate(actionStatus, model, sqlAccessor), sqlAccessor);
				}
				finally
				{
					// 更新処理ロールバック
					sqlAccessor.RollbackTransaction();
				}

				ProductPreview.InsertPreview(Constants.FLG_PREVIEW_PREVIEW_KBN_COORDINATE_DETAIL, this.SessionWrapper.LoginShopId, (string)coordinate[0][Constants.FIELD_COORDINATE_COORDINATE_ID], "", "", "", coordinate.Table);
			}
		}

		/// <summary>
		/// 商品ビューモデルを取得（バリエーションあり）
		/// </summary>
		/// <param name="model">商品</param>
		/// <param name="sortNo">ソートNo</param>
		/// <returns>商品ビューモデル</returns>
		public ProductViewModel GetVariationProductVm(ProductModel model, int sortNo)
		{
			// 商品バリエーション情報作成
			var productInfo = ProductCommon.GetProductInfoUnuseMemberRankPrice(this.SessionWrapper.LoginShopId, model.ProductId);
			var variationList = new List<SelectListItem>();
			var name = model.Name;
			if ((string)ProductCommon.GetKeyValue(productInfo[0], Constants.FIELD_PRODUCT_USE_VARIATION_FLG) == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE)
			{
				name = GetProductJointName(productInfo[0]);
				if (string.IsNullOrEmpty(model.VariationId)) model.VariationId = (string)productInfo[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
				foreach (DataRowView product in productInfo)
				{
					var selected = (((string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] == model.VariationId));
					variationList.Add(new SelectListItem
					{
						Text = (string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
						Value = (string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
						Selected = selected
					});
					if (selected) name = GetProductJointName(product);
				}
				
			}

			var pvm = new ProductViewModel
			{
				Id = model.ProductId,
				VariationList = variationList,
				VariationId = model.VariationId,
				Name = name,
				ImagePath = CreateProductImageUrl(model.ProductId, model.VariationId),
				Stock = new ProductStockService().GetSum(this.SessionWrapper.LoginShopId, model.ProductId),
				SortNo = (sortNo + 1),
			};
			return pvm;
		}

		/// <summary>
		/// バリエーション名を取得
		/// </summary>
		/// <param name="product">商品</param>
		/// <returns>バリエーション名</returns>
		public string GetProductJointName(DataRowView product)
		{
			var productJointName = (string)product[Constants.FIELD_PRODUCT_NAME] 
				+ ProductCommon.CreateVariationName(
					(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
					(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
					(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]);
			
			return productJointName;
		}

		/// <summary>
		/// エクスポート
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>エクスポートするファイルデータ</returns>
		public MasterExportHelper.ExportFileData Export(CoordinateParamModel pm)
		{
			var masterKbn = pm.DataExportType.Split('-')[1];
			var settingId = int.Parse(pm.DataExportType.Split('-')[0]) - 1;
			var cond = new CoordinateListSearchCondition
			{
				Keyword0 = pm.SearchKeyword,
				Staff = pm.SearchStaff,
				RealShop = pm.SearchRealShop,
				Category = pm.SearchCategory,
				DisplayDateKbn = pm.DisplayDateKbn,
			};

			var fileData = MasterExportHelper.CreateExportData(
				base.SessionWrapper.LoginShopId,
				masterKbn,
				settingId,
				cond);

			return fileData;
		}
	}
}