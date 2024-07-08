/*
=========================================================================================================
  Module      : ノベルティキャッシュコントローラ(NoveltyCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.Novelty;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// ノベルティキャッシュコントローラ
	/// </summary>
	public class NoveltyCacheController : DataCacheControllerBase<NoveltyModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal NoveltyCacheController()
			: base(RefreshFileType.Novelty)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.NoveltyService.GetAll(Constants.CONST_DEFAULT_SHOP_ID).ToArray();
		}

		/// <summary>
		/// 有効なノベルティ設定取得
		/// </summary>
		/// <returns>有効なノベルティ設定モデル列</returns>
		public NoveltyModel[] GetApplicableNovelty()
		{
			// 開始終了時間内、有効フラグがON
			return this.CacheData.Where(novelty =>
				((novelty.DateBegin <= DateTime.Now) && ((novelty.DateEnd == null) || (novelty.DateEnd >= DateTime.Now)))
				&& (novelty.IsValid)).ToArray();
		}
	}
}