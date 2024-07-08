/*
=========================================================================================================
  Module      : 注文拡張ステータス設定リポジトリ (OrderExtendStatusSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.OrderExtendStatusSetting
{
	/// <summary>
	/// 注文拡張ステータス設定リポジトリ
	/// </summary>
	internal class OrderExtendStatusSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "OrderExtendStatusSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal OrderExtendStatusSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal OrderExtendStatusSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetOrderExtendStatusSetting 取得
		/// <summary>
		/// 注文ステータス名称が登録されているデータを取得
		/// </summary>
		/// <returns>モデル配列</returns>
		internal OrderExtendStatusSettingModel[] GetOrderExtendStatusSetting()
		{
			var ht = new Hashtable
			{
				{Constants.FLG_ORDER_EXTEND_STATUS_MAX, Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderExtendStatusSettingList", ht);
			return dv.Count > 0 ? dv.Cast<DataRowView>().Select(drv => new OrderExtendStatusSettingModel(drv)).ToArray() : new OrderExtendStatusSettingModel[0];
		}
		#endregion
	}
}