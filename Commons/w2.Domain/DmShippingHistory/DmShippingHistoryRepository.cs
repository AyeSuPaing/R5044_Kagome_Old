/*
=========================================================================================================
  Module      : DM発送履歴リポジトリ (DmShippingHistoryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;
namespace w2.Domain.DmShippingHistory
{
	/// <summary>
	/// DM発送履歴リポジトリ
	/// </summary>
	public class DmShippingHistoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "DmShippingHistory";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DmShippingHistoryRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetDmShippingHistoryByUserId ユーザーIDからDM発送履歴情報取得
		/// <summary>
		/// ユーザーIDからDM発送履歴情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>DM発送履歴モデル</returns>
		public DmShippingHistoryModel[] GetDmShippingHistoryByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_DMSHIPPINGHISTORY_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetDmShippingHistoryByUserId", ht);
			if (dv.Count == 0) return new DmShippingHistoryModel[0];
			var models = dv.Cast<DataRowView>().Select(drv => new DmShippingHistoryModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region ~UpdateForUserIntegration ユーザーIDからDM発送履歴情報取得
		/// <summary>
		/// ユーザー統合用UPDATE
		/// </summary>
		/// <param name="dmShippingHistory">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateForUserIntegration(DmShippingHistoryModel dmShippingHistory)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_DMSHIPPINGHISTORY_USER_ID, dmShippingHistory.UserId},
				{Constants.FIELD_DMSHIPPINGHISTORY_DM_CODE, dmShippingHistory.DmCode},
				{"representative_user_id", dmShippingHistory.RepresentativeUserId}
			};
			var result = Exec(XML_KEY_NAME, "UpdateForUserIntegration", ht);
			return result;
		}
		#endregion
	}
}
