/*
=========================================================================================================
  Module      : レコメンド設定キャッシュコントローラ(RecommendCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.Recommend;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// レコメンド設定キャッシュコントローラ
	/// </summary>
	public class RecommendCacheController : DataCacheControllerBase<RecommendModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal RecommendCacheController()
			: base(RefreshFileType.Recommend)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.RecommendService.GetAll(Constants.CONST_DEFAULT_SHOP_ID).ToArray();
		}

		/// <summary>
		/// 有効なレコメンド設定取得
		/// </summary>
		/// <returns>有効なレコメンド設定モデル列</returns>
		public RecommendModel[] GetApplicableRecommend()
		{
			// 開始終了時間内、有効フラグがON
			return this.CacheData.Where(recommend =>
				((recommend.DateBegin <= DateTime.Now) && ((recommend.DateEnd == null) || (recommend.DateEnd >= DateTime.Now)))
				&& (recommend.IsValid)).ToArray();
		}
	}
}