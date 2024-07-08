/*
=========================================================================================================
  Module      : 定期解約理由区分設定情報キャッシュコントローラ(FixedPurchaseCancelReasonCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.FixedPurchase;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 定期解約理由区分設定情報キャッシュコントローラ
	/// </summary>
	public class FixedPurchaseCancelReasonCacheController : DataCacheControllerBase<FixedPurchaseCancelReasonModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal FixedPurchaseCancelReasonCacheController()
			: base(RefreshFileType.FixedPurchaseCancelReason)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.FixedPurchaseService.GetCancelReasonAll().ToArray();
		}

		/// <summary>
		/// 定期解約理由区分設定情報取得
		/// </summary>
		/// <param name="cancelReasonId">解約理由区分ID</param>
		public FixedPurchaseCancelReasonModel GetCancelReason(string cancelReasonId)
		{
			var fixedPurchaseCancelReason = this.CacheData.Where(m => m.CancelReasonId == cancelReasonId);
			return fixedPurchaseCancelReason.FirstOrDefault();
		}
	}
}