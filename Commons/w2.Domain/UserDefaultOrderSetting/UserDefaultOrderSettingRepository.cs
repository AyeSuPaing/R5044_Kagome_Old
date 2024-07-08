/*
=========================================================================================================
  Module      : デフォルト注文方法リポジトリ (UserDefaultOrderSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.UserDefaultOrderSetting
{
	/// <summary>
	/// デフォルト注文方法リポジトリ
	/// </summary>
	internal class UserDefaultOrderSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "UserDefaultOrderSetting";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal UserDefaultOrderSettingRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		internal UserDefaultOrderSettingModel Get(string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERDEFAULTORDERSETTING_USER_ID, userId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new UserDefaultOrderSettingModel(dv[0]);
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(UserDefaultOrderSettingModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(UserDefaultOrderSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion
	}
}
