/*
=========================================================================================================
  Module      : 表示設定管理リポジトリ (ManagerListDispSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ManagerListDispSetting
{
	/// <summary>
	/// 表示設定管理リポジトリ
	/// </summary>
	internal class ManagerListDispSettingRepository : RepositoryBase
	{
		private const string XML_KEY_NAME = "ManagerListDispSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ManagerListDispSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal ManagerListDispSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetAllByDispSettingKbn 取得
		/// <summary>
		/// 設定区分先の表示設定を全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="dispSettingKbn">表示設定先区分</param>
		/// <returns>モデル配列</returns>
		internal ManagerListDispSettingModel[] GetAllByDispSettingKbn(string shopId, object dispSettingKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPOPERATOR_SHOP_ID, shopId},
				{Constants.FIELD_MANAGERLISTDISPSETTING_DISP_SETTING_KBN, dispSettingKbn},
			};
			var dv = Get(XML_KEY_NAME, "GetAllByDispSettingKbn", ht);
			return (dv.Count > 0) ? dv.Cast<DataRowView>().Select(drv => new ManagerListDispSettingModel(drv)).ToArray() : new ManagerListDispSettingModel[0];
		}
		#endregion

		#region ~GetForDispSettingItemNotIncludedOrderId 取得
		/// <summary>
		/// 表示設定を取得(注文IDは除く)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="dispSettingKbn">表示設定先区分</param>
		/// <returns>モデル配列</returns>
		internal ManagerListDispSettingModel[] GetForDispSettingItemNotIncludedOrderId(string shopId, object dispSettingKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPOPERATOR_SHOP_ID, shopId},
				{Constants.FIELD_MANAGERLISTDISPSETTING_DISP_SETTING_KBN, dispSettingKbn},
			};
			var dv = Get(XML_KEY_NAME, "GetForDispSettingItemNotIncludedOrderId", ht);
			return (dv.Count > 0) ? dv.Cast<DataRowView>().Select(drv => new ManagerListDispSettingModel(drv)).ToArray() : new ManagerListDispSettingModel[0];
		}
		#endregion

		#region ~UpdateDispSetting 更新
		/// <summary>
		/// 表示設定を更新する
		/// </summary>
		/// <param name="model">モデル</param>
		internal int UpdateDispSetting(ManagerListDispSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateDispSetting", model.DataSource);
			return result;
		}
		#endregion

		#region ~UpdateDispSettingFlagOff 更新
		/// <summary>
		/// 項目の表示フラグをOFFにする
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="dispSettingKbn">表示設定先区分</param>
		/// <param name="dispColmunName">表示項目名</param>
		internal int UpdateDispSettingFlagOff(string shopId, string lastChanged, object dispSettingKbn, string dispColmunName)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPOPERATOR_SHOP_ID, shopId},
				{Constants.FIELD_SHOPOPERATOR_LAST_CHANGED, lastChanged},
				{Constants.FIELD_MANAGERLISTDISPSETTING_DISP_SETTING_KBN, dispSettingKbn},
				{Constants.FIELD_MANAGERLISTDISPSETTING_DISP_COLUMN_NAME, dispColmunName},
			};

			var result = Exec(XML_KEY_NAME, "UpdateDispSettingFlagOff", ht);
			return result;
		}
		#endregion
	}
}