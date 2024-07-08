/*
=========================================================================================================
  Module      : アフィリエイトタグ情報キャッシュコントローラ(AffiliateTagSettingCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.Affiliate;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// アフィリエイトタグ情報キャッシュコントローラ
	/// </summary>
	public class AffiliateTagSettingCacheController : DataCacheControllerBase<AffiliateTagSettingModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AffiliateTagSettingCacheController() : base(RefreshFileType.AffiliateTagSetting)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.AffiliateTagSettingService.GetAllIncludeConditionModels();
		}

		/// <summary>
		/// アフィリエイトタグで有効なもの全取得
		/// </summary>
		/// <returns>アフィリエイトタグモデルリスト</returns>
		public AffiliateTagSettingModel[] GetAffiliateTagSettingModels()
		{
			return this.CacheData.Where(
				i => (i.ValidFlg == Constants.FLG_AFFILIATETAGSETTING_VALID_FLG_VALID)
					&& (i.AffiliateKbn != Constants.FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_MOBILE)).ToArray();
		}
	}
}