/*
=========================================================================================================
  Module      : 注文拡張項目設定リポジトリ (OrderExtendSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.OrderExtendSetting
{
	/// <summary>
	/// 注文拡張項目設定リポジトリ
	/// </summary>
	internal class OrderExtendSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "OrderExtendSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal OrderExtendSettingRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal OrderExtendSettingRepository(SqlAccessor accessor) : base(accessor)
		{
		}
		#endregion

		#region ~GetAll 全取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		internal OrderExtendSettingModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			return dv.Cast<DataRowView>().Select(drv => new OrderExtendSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <returns>モデル</returns>
		internal OrderExtendSettingModel Get(string settingId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDEREXTENDSETTING_SETTING_ID, settingId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new OrderExtendSettingModel(dv[0]);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(OrderExtendSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion
	}
}