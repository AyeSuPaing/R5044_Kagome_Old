/*
=========================================================================================================
  Module      : 特集ページ情報キャッシュコントローラ (FeaturePageCacheController.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using w2.App.Common.Design;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.FeaturePage;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 特集ページ情報キャッシュコントローラ
	/// </summary>
	public class FeaturePageCacheController : DataCacheControllerBase<FeaturePageModel[]>
	{
		///<summary>特集ページヘッダーバナーID</summary>
		private const string FEATURE_PAGE_DIV_ID_HEADER_BANNER = "header-banner";

		/// <summary>特集ページIMAGEなし画像のパス</summary>
		private static string FEATURE_PAGE_NO_IMAGE_PATH = Constants.PATH_ROOT + Constants.PATH_FEATURE_IMAGE + Constants.PRODUCTIMAGE_NOIMAGE_PC_LL;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeaturePageCacheController()
			: base(RefreshFileType.FeaturePage)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			var featurePages = DomainFacade.Instance.FeaturePageService.GetAllPageWithContents();

			foreach (var page in featurePages)
			{
				// バナー画像
				var pcPath = Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, page.FileDirPath);
				var pcPageText = DesignCommon.GetFileTextAll(Path.Combine(pcPath, page.FileName));
				var pcPageImageSrc = DesignCommon.GetHeaderBannerSrc(
					FEATURE_PAGE_DIV_ID_HEADER_BANNER,
					pcPageText);
				page.PcBannerImageSrc = string.IsNullOrEmpty(pcPageImageSrc) ? FEATURE_PAGE_NO_IMAGE_PATH : pcPageImageSrc;

				var spPath = Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, page.FileDirPath);
				var spPageText = DesignCommon.GetFileTextAll(Path.Combine(spPath, page.FileName));
				var spPageImageSrc = DesignCommon.GetHeaderBannerSrc(
					FEATURE_PAGE_DIV_ID_HEADER_BANNER,
					spPageText);
				page.SpBannerImageSrc = string.IsNullOrEmpty(spPageImageSrc) ? FEATURE_PAGE_NO_IMAGE_PATH : spPageImageSrc;

				// 特集ページパス
				page.FeaturePagePath = Constants.PATH_ROOT + page.FileDirPath.Replace("\\", "/") + page.FileName;
			}
			this.CacheData = featurePages;
		}
	}
}
