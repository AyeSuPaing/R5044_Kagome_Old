/*
=========================================================================================================
 Module      : サイトマップ設定ワーカーサービス(SitemapWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.Sitemap;
using w2.Cms.Manager.ParamModels.Sitemap;
using w2.Cms.Manager.ViewModels.Sitemap;
using w2.Common.Web;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// サイトマップ設定ワーカーサービス
	/// </summary>
	public class SitemapWorkerService : BaseWorkerService
	{
		/// <summary>1ページの表示件数</summary>
		private const int ITEMS_PER_PAGE = 10;

		/// <summary>
		/// サイトマップ設定ファイルが正常か
		/// (シリアライズできるか)
		/// </summary>
		/// <returns>結果</returns>
		public bool IsValidSitemapSetting()
		{
			return SitemapUtility.TrySerializeSitemapSetting();
		}

		/// <summary>
		/// Main画面用ビューモデル作成
		/// </summary>
		/// <param name="pm">パラメータモデル</param>
		/// <returns>ビューモデル</returns>
		public MainViewModel CreateViewModelForMain(MainParamModel pm)
		{
			var vm = new MainViewModel
			{
				ParamModel = pm,
			};

			int totalHit;
			vm.PageItems = Search(pm, out totalHit);
			vm.PagerHtml = CreatePagerHtml(totalHit, pm.PagerNo, pm);
			vm.Message = vm.PageItems.Any() ? string.Empty : WebMessages.NoHitListError;

			return vm;
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="param">検索条件パラメタ</param>
		/// <param name="total">検索ヒット件数(参照渡し)</param>
		/// <returns>検索結果</returns>
		public SitemapPageItem[] Search(MainParamModel param, out int total)
		{
			var result = SitemapUtility.GetPagesByPageType(param.PageType)
				.Where(
					p => (((param.ForPcPage) && (p.DeviceType == DeviceTypeEnum.Pc || p.DeviceType == DeviceTypeEnum.PcAndSp))
						|| ((param.ForSpPage) && (p.DeviceType == DeviceTypeEnum.Sp || p.DeviceType == DeviceTypeEnum.PcAndSp))))
				.Where(
					p => (string.IsNullOrEmpty(param.Keyword)
						|| p.Title.Contains(param.Keyword)
						|| p.Url.Contains(param.Keyword)))
				.Select(
					(item, index) =>
					{
						item.SequentialId = index;
						return item;
					})
				.ToArray();

			total = result.Length;

			result = result
				.Skip((param.PagerNo - 1) * ITEMS_PER_PAGE)
				.Take(ITEMS_PER_PAGE)
				.ToArray();

			return result;
		}

		/// <summary>
		/// ページャHTML作成
		/// </summary>
		/// <param name="total">総件数</param>
		/// <param name="currentPage">現在のページ番号</param>
		/// <param name="paramModel">パラメータ</param>
		/// <returns>ページャHTML</returns>
		public string CreatePagerHtml(int total, int currentPage, MainParamModel paramModel = null)
		{
			paramModel = paramModel ?? new MainParamModel();

			var pager = WebPager.CreateDefaultListPager(
				total,
				currentPage,
				new UrlCreator(this.UrlHelper.Action("Main", Constants.CONTROLLER_W2CMS_MANAGER_SITEMAP))
					.AddParam("ParamModel.Keyword", paramModel.Keyword)
					.AddParam("ParamModel.ForPcPage", paramModel.ForPcPage.ToString())
					.AddParam("ParamModel.ForSpPage", paramModel.ForSpPage.ToString())
					.AddParam("ParamModel.PageType", ((int)(paramModel.PageType)).ToString())
					.CreateUrl(),
				ITEMS_PER_PAGE);
			return pager;
		}

		/// <summary>
		/// 設定更新
		/// </summary>
		/// <param name="pageItem">ページアイテム</param>
		public void Update(SitemapPageItem[] pageItem)
		{
			SitemapUtility.UpdateSetting(pageItem);
		}
	}
}