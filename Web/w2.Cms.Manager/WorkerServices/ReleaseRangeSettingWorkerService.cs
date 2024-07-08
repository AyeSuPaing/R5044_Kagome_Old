/*
=========================================================================================================
  Module      : 公開範囲設定 ワーカーサービス(ReleaseRangeSettingWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common;
using w2.Domain.TargetList;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 公開範囲設定　ワーカーサービス
	/// </summary>
	public class ReleaseRangeSettingWorkerService : BaseWorkerService
	{
		/// <summary>
		/// 公開範囲設定 検索結果の表示モデル
		/// </summary>
		/// <param name="keyword">検索キーワード</param>
		/// <returns>検索結果の表示モデル</returns>
		public TargetListModel[] SearchTargetListModel(string keyword)
		{
			var result = new TargetListService().GetAll(this.SessionWrapper.LoginShopId)
				.Where(t => (t.ValidFlg == Constants.FLG_TARGETLIST_VALID_FLG_VALID)
					&& (t.DelFlg == Constants.FLG_TARGETLIST_DEL_FLG_INVALID)
					&& (t.TargetId.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
						|| t.TargetName.Contains(keyword)))
				.ToArray();
			return result;
		}
	}
}