/*
=========================================================================================================
  Module      : コーディネートカテゴリワーカーサービス(CoordinateCategoryWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ParamModels.CoordinateCategory;
using w2.Cms.Manager.ViewModels.CoordinateCategory;
using w2.Domain.CoordinateCategory;
using w2.Domain.CoordinateCategory.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// コーディネートカテゴリワーカーサービス
	/// </summary>
	public class CoordinateCategoryWorkerService : BaseWorkerService
	{
		/// <summary>
		/// コーディネートカテゴリビューモデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="categoryModel">カテゴリーモデル</param>
		/// <param name="pm">パラムモデル</param>
		/// <returns>ビューモデル</returns>
		public CoordinateCategoryViewModel CreateCoordinateCategoryVm(
			ActionStatus actionStatus,
			CoordinateCategoryModel categoryModel,
			CoordinateCategoryParamModel pm)
		{
				var searchCondition = new CoordinateCategoryListSearchCondition
				{
					CoordinateCategoryId = pm.SearchCoordinateCategoryId,
					CoordinateParentCategoryId = pm.SearchCoordinateParentCategoryId,
					BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
					EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
				};
				var total = new CoordinateCategoryService().GetSearchHitCount(searchCondition);
				var list = new CoordinateCategoryService().Search(searchCondition);
				if (list.Length == 0)
				{
					return new CoordinateCategoryViewModel(actionStatus, categoryModel, pm)
					{
						ErrorMessage = WebMessages.NoHitListError,
					};
				}

				var url = this.UrlHelper.Action(
					"Main",
					Constants.CONTROLLER_W2CMS_MANAGER_COORDINATE_CATEGORY,
					new
					{
						pm.SearchCoordinateCategoryId,
						pm.SearchCoordinateParentCategoryId,
						pm.PageLayout
					});
				return new CoordinateCategoryViewModel(actionStatus, categoryModel, pm)
				{
					List = list,
					PagerHtml = WebPager.CreateDefaultListPager(total, pm.PagerNo, url),
				};
		}

		/// <summary>
		/// 登録更新
		/// </summary>
		/// <param name="categoryModel">カテゴリー</param>
		/// <param name="isUpdate">更新か</param>
		public void InsertUpdate(CoordinateCategoryModel categoryModel, bool isUpdate)
		{
			categoryModel.LastChanged = this.SessionWrapper.LoginOperatorName;
			var service = new CoordinateCategoryService();
			if (isUpdate)
			{
				service.Update(categoryModel);
			}
			else
			{
				service.Insert(categoryModel);
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="coordinateCategoryId">コーディネートカテゴリID</param>
		public void Delete(string coordinateCategoryId)
		{
			new CoordinateCategoryService().Delete(coordinateCategoryId);
		}

		/// <summary>
		/// エクスポート
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>エクスポートするファイルデータ</returns>
		public MasterExportHelper.ExportFileData Export(CoordinateCategoryParamModel pm)
		{
			var masterKbn = pm.DataExportType.Split('-')[1];
			var settingId = int.Parse(pm.DataExportType.Split('-')[0]) - 1;
			var cond = new CoordinateCategoryListSearchCondition
			{
				CoordinateCategoryId = pm.SearchCoordinateCategoryId,
				CoordinateParentCategoryId = pm.SearchCoordinateParentCategoryId,
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