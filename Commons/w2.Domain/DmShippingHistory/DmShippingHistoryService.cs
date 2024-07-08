/*
=========================================================================================================
  Module      : DM発送履歴サービス (DmShippingHistoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
namespace w2.Domain.DmShippingHistory
{
	/// <summary>
	/// DM発送履歴サービス
	/// </summary>
	public class DmShippingHistoryService : ServiceBase
	{
		#region +GetDmShippingHistoryByUserId DM発送履歴取得
		/// <summary>
		/// ユーザーIDからDM発送履歴取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public static DmShippingHistoryModel[] GetDmShippingHistoryByUserId(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new DmShippingHistoryRepository(accessor))
			{
				var models = repository.GetDmShippingHistoryByUserId(userId);
				return models;
			}
		}
		#endregion

		#region +GetDmShippingHistoriesByUserIdForUserIntegration DM発送履歴取得（ユーザー統合用）
		/// <summary>
		/// ユーザーIDからDM発送履歴取得（ユーザー統合用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public DmShippingHistoryModel[] GetDmShippingHistoriesByUserIdForUserIntegration(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new DmShippingHistoryRepository(accessor))
			{
				var models = repository.GetDmShippingHistoryByUserId(userId);
				return models;
			}
		}
		#endregion

		#region +UpdateForUserIntegration ユーザー統合用
		/// <summary>
		/// ユーザー統合用UPDATE
		/// </summary>
		/// <param name="dmShippingHistory">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateForUserIntegration(DmShippingHistoryModel dmShippingHistory, SqlAccessor accessor = null)
		{
			using (var repository = new DmShippingHistoryRepository(accessor))
			{
				var result = repository.UpdateForUserIntegration(dmShippingHistory);
				return result;
			}
		}
		#endregion
	}
}
