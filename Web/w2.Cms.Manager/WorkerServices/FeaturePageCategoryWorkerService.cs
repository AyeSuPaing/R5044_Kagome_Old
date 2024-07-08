/*
=========================================================================================================
  Module      : 特集ページワーカーサービス(FeaturePageCategoryWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Linq;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ParamModels.FeaturePageCategory;
using w2.Cms.Manager.ViewModels.FeaturePageCategory;
using w2.Domain.FeaturePageCategory;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 特集ページカテゴリワーカーサービス
	/// </summary>
	public class FeaturePageCategoryWorkerService : BaseWorkerService
	{
		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(FeaturePageCategoryListParamModel pm)
		{
			var total = new FeaturePageCategoryService().GetSearchHitCount(this.SessionWrapper.LoginShopId, pm.SortCategoryId, pm.SortKbn);
			if (total == 0)
			{
				return new ListViewModel
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError,
				};
			}

			var list = new FeaturePageCategoryService().Search(
				this.SessionWrapper.LoginShopId,
				pm.SortCategoryId,
				pm.SortKbn,
				(pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);

			var url = this.UrlHelper.Action(
				"List",
				Constants.CONTROLLER_W2CMS_MANAGER_FEATURE_PAGE_CATEGORY);
			return new ListViewModel
			{
				List = list
					.Select(r => new SearchResultViewModel(r))
					.ToArray(),
				ParamModel = pm,
				PagerHtml = WebPager.CreateDefaultListPager(total, pm.PagerNo, url),
			};
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">特集ページカテゴリ情報</param>
		/// <returns>ビューモデル</returns>
		public void Insert(Hashtable model)
		{
			new FeaturePageCategoryService().Insert(model);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="ht">特集ページカテゴリ情報</param>
		/// <returns>ビューモデル</returns>
		public void Update(Hashtable ht)
		{
			new FeaturePageCategoryService().Update(ht);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="ht">特集ページカテゴリ情報</param>
		public void Delete(Hashtable ht)
		{
			new FeaturePageCategoryService().Delete(ht);
		}
	}
}