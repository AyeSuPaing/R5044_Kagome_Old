/*
=========================================================================================================
  Module      : 商品タグマネージャー ワーカーサービス(ProductTagManagerWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ProductTagManager;
using w2.Cms.Manager.ViewModels.ProductTagManager;
using w2.Domain.Affiliate;
using w2.Domain.Affiliate.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 商品タグマネージャー ワーカーサービス
	/// </summary>
	public class ProductTagManagerWorkerService : BaseWorkerService
	{
		/// <summary>検索一覧 1ページあたりの表示件数</summary>
		private const int LIST_DISPLAY_RESULT_NUM = 5;

		/// <summary>
		/// 検索ListViewモデル生成
		/// </summary>
		/// <param name="pm">検索パラメタモデル</param>
		/// <returns>Viewモデル</returns>
		public ProductTagManagerListViewModel CreateListVm(ProductTagManagerListParamModel pm)
		{
			var searchCondition = new AffiliateProductTagSettingListSearchCondition
			{
				AffiliateProductTagName = pm.AffiliateProductTagName,
				BeginRowNumber = (pm.PagerNo - 1) * LIST_DISPLAY_RESULT_NUM + 1,
				EndRowNumber = pm.PagerNo * LIST_DISPLAY_RESULT_NUM
			};

			var total = new AffiliateTagSettingService().AffiliateProductTagSettingGetSearchHitCount(searchCondition);
			var list = new AffiliateTagSettingService().AffiliateProductTagSettingSearch(searchCondition);
			if (list.Length == 0)
			{
				return new ProductTagManagerListViewModel
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError
				};
			}

			return new ProductTagManagerListViewModel
			{
				ParamModel = pm,
				ModifyInputs =
					list.Select(r => new ProductTagManagerInput(r)).ToArray(),
				PagerHtml = WebPager.CreateDefaultListPager(
					total,
					pm.PagerNo,
					this.UrlHelper.Action("List", Constants.CONTROLLER_W2CMS_MANAGER_PRODUCT_TAG_MANAGER, pm),
					LIST_DISPLAY_RESULT_NUM)
			};
		}

		/// <summary>
		/// 登録Viewモデル生成
		/// </summary>
		/// <param name="input">登録 入力内容</param>
		/// <returns>Viewモデル</returns>
		public ProductTagManagerListViewModel CreateRegisterVmInsert(ProductTagManagerInput input)
		{
			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			new AffiliateTagSettingService().AffiliateProductTagSettingInsert(model);

			// アフィリエイトタグ設定のキャッシュ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.AffiliateTagSetting).CreateUpdateRefreshFile();

			return new ProductTagManagerListViewModel
			{
				ActionStatus = ActionStatus.Update,
				ConformModel = new[] { model }
			};
		}

		/// <summary>
		/// 更新Viewモデル生成
		/// </summary>
		/// <param name="models">一括更新 入力内容</param>
		/// <returns>Viewモデル</returns>
		public ProductTagManagerListViewModel CreateRegisterVmUpdate(List<AffiliateProductTagSettingModel> models)
		{
			foreach (var model in models)
			{
				model.LastChanged = this.SessionWrapper.LoginOperatorName;
				new AffiliateTagSettingService().AffiliateProductTagSettingUpdate(model);
			}

			// アフィリエイトタグ設定のキャッシュ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.AffiliateTagSetting).CreateUpdateRefreshFile();

			return new ProductTagManagerListViewModel
			{
				ActionStatus = ActionStatus.Update,
				ConformModel = models.ToArray()
			};
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="affiliateProductTagId">商品タグID</param>
		public void Delete(int affiliateProductTagId)
		{
			new AffiliateTagSettingService().AffiliateProductTagSettingDelete(affiliateProductTagId);
		}
	}
}