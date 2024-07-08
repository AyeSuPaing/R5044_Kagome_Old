/*
=========================================================================================================
  Module      : 商品ランキングワーカーサービス(ProductRankingWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ParamModels.ProductRanking;
using w2.Cms.Manager.ViewModels.ProductRanking;
using w2.Domain.ProductCategory;
using w2.Domain.ProductRanking;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 商品ランキングワーカーサービス
	/// </summary>
	public class ProductRankingWorkerService : BaseWorkerService
	{
		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(ProductRankingListParamModel pm)
		{
			var total = new ProductRankingService().GetSearchHitCount(
				this.SessionWrapper.LoginShopId,
				pm.RankingId,
				pm.TotalType,
				pm.ValidFlg);
			if (total == 0)
			{
				return new ListViewModel
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError,
				};
			}

			var list = new ProductRankingService().Search(
				this.SessionWrapper.LoginShopId,
				pm.RankingId,
				pm.TotalType,
				pm.ValidFlg,
				pm.SortKbn,
				(pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);

			var url = this.UrlHelper.Action(
				"List",
				Constants.CONTROLLER_W2CMS_MANAGER_PRODUCT_RANKING,
				new
				{
					RankingId = pm.RankingId,
					TotalType = pm.TotalType,
					ValidFlg = pm.ValidFlg,
					SortKbn = pm.SortKbn,
					PagerNo = pm.PagerNo,
				});
			return new ListViewModel
			{
				ParamModel = pm,
				List = list.Select(
					r => new SearchResultListViewModel(
						r,
						GetDisplayCategoryInfo(r.CategoryId),
						GetDisplayCategoryInfo(r.ExcludeCategoryIds))).ToArray(),
				PagerHtml = WebPager.CreateDefaultListPager(total, pm.PagerNo, url),
			};
		}

		/// <summary>
		/// 登録編集ビューモデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <param name="tempDataManager">データマネージャ</param>
		/// <returns>登録ビューモデル</returns>
		public RegisterViewModel CreateRegisterVm(
			ActionStatus actionStatus,
			string rankingId,
			TempDataManager tempDataManager)
		{
			switch (actionStatus)
			{
				case ActionStatus.Insert:
					var insertModel = tempDataManager.ProductRanking
						?? new ProductRankingService().GetForDisplay(this.SessionWrapper.LoginShopId, rankingId);
					return CreateRegisterViewModel(insertModel, actionStatus);

				case ActionStatus.Update:
					var updateModel = tempDataManager.ProductRanking
						?? new ProductRankingService().GetForDisplay(this.SessionWrapper.LoginShopId, rankingId);
					if (updateModel != null)
					{
						return CreateRegisterViewModel(updateModel, actionStatus);
					}
					else
					{
						throw new Exception("RankingIdが不正です");
					}

				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// 確認詳細ビューモデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <param name="tempDataManager"></param>
		/// <returns>確認ビューモデル</returns>
		public ConfirmViewModel CreateConfirmVm(
			ActionStatus actionStatus,
			string rankingId,
			TempDataManager tempDataManager)
		{
			switch (actionStatus)
			{
				case ActionStatus.Detail:
					var productRanking = new ProductRankingService().GetForDisplay(this.SessionWrapper.LoginShopId, rankingId);
					return new ConfirmViewModel(
						actionStatus,
						productRanking,
						GetDisplayCategoryInfo(productRanking.CategoryId),
						GetDisplayCategoryInfo(productRanking.ExcludeCategoryIds));

				case ActionStatus.Insert:
				case ActionStatus.Update:
					return new ConfirmViewModel(actionStatus, tempDataManager.ProductRanking,
						GetDisplayCategoryInfo(tempDataManager.ProductRanking.CategoryId),
						GetDisplayCategoryInfo(tempDataManager.ProductRanking.ExcludeCategoryIds));

				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="rankingId">商品ランキングID</param>
		public void Delete(string rankingId)
		{
			// 削除
			new ProductRankingService().Delete(this.SessionWrapper.LoginShopId, rankingId);

			// フロント系サイトを最新情報へ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.DisplayProduct).CreateUpdateRefreshFile();
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="tempDataManager">データマネージャ</param>
		/// <returns>ビューモデル</returns>
		public ConfirmViewModel Update(TempDataManager tempDataManager)
		{
			var model = tempDataManager.ProductRanking;
			model.ShopId = this.SessionWrapper.LoginShopId;
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			model.Items.ToList().ForEach(item => item.LastChanged = this.SessionWrapper.LoginOperatorName);

			var service = new ProductRankingService();
			service.Update(model);

			// フロント系サイトを最新情報へ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.DisplayProduct).CreateUpdateRefreshFile();

			return CreateConfirmVm(ActionStatus.Detail,model.RankingId, null);
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">商品ランキング情報</param>
		/// <returns>ビューモデル</returns>
		public ConfirmViewModel Insert(ProductRankingModel model)
		{
			model.ShopId = this.SessionWrapper.LoginShopId;
			model.LastChanged = this.SessionWrapper.LoginOperatorName;

			var service = new ProductRankingService();
			service.Insert(model);

			// フロント系サイトを最新情報へ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.News).CreateUpdateRefreshFile();

			return CreateConfirmVm(ActionStatus.Detail, model.RankingId, null);
		}

		/// <summary>
		/// 表示用カテゴリ情報を取得
		/// </summary>
		/// <param name="categoryIds">カテゴリID（列）</param>
		/// <returns>表示用カテゴリ情報</returns>
		protected string GetDisplayCategoryInfo(string categoryIds)
		{
			var categoryIdList = categoryIds.Replace("\r\n", "\n").Split('\n').Where(s => string.IsNullOrEmpty(s) == false).ToArray();
			var categories = new ProductCategoryService().GetByCategoryIds(categoryIdList);
			var errorMessage = new StringBuilder();

			foreach (string categoryId in categoryIdList)
			{
				if (categories.Any(c => (c.CategoryId == categoryId)) == false)
				{
					errorMessage.Append(WebMessages.ProductRankingProductCategoryUnFound.Replace("@@ 1 @@", categoryId));
				}
			}

			var categoryInfo = (string.IsNullOrEmpty(errorMessage.ToString()))
				? string.Join(
					"\r\n",
					categoryIdList.Select(
						id => string.Format(
							"{0} ( {1} )",
							id,
							categories.First(s => (s.CategoryId == id)).Name)))
				: errorMessage.ToString().Replace("<br />", "\r\n");
			return categoryInfo;
		}

		/// <summary>
		/// 登録用ViewModel作成
		/// </summary>
		/// <param name="productRanking">商品ランキング</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>登録用ビューモデル</returns>
		protected RegisterViewModel CreateRegisterViewModel(ProductRankingModel productRanking, ActionStatus actionStatus)
		{
			if (productRanking == null)
			{
				return new RegisterViewModel(this.SessionWrapper.LoginShopId)
				{
					ActionStatus = actionStatus,
				};
			}

			var viewModel = new RegisterViewModel(
				this.SessionWrapper.LoginShopId,
				productRanking,
				GetDisplayCategoryInfo(productRanking.CategoryId),
				GetDisplayCategoryInfo(productRanking.ExcludeCategoryIds))
			{
				ActionStatus = actionStatus,
			};
			return viewModel;
		}

		/// <summary>
		/// 集計処理
		/// </summary>
		/// <param name="rankingIds">対象候補商品ランキング</param>
		/// <param name="checks">対象商品ランキング</param>
		public void TotalAction(string[] rankingIds, bool[] checks)
		{
			var commandParams = new StringBuilder();
			commandParams.Append(this.SessionWrapper.LoginShopId).Append(" ");

			// バッチに渡すパラメータを作成
			bool noParam = false;
			for (var index = 0; index < checks.Length; index++)
			{
				if (checks[index] == false) continue;

				commandParams.Append(rankingIds[index]).Append(" ");
				noParam = true;
			}

			if (noParam)
			{
				Process.Start(Constants.PHYSICALDIRPATH_CREATEDISPPRODUCT_EXE, commandParams.ToString());
			}
		}
	}
}