/*
=========================================================================================================
  Module      : マスタ出力定義リポジトリ (MasterExportSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MasterExportSetting
{
	/// <summary>
	/// マスタ出力定義リポジトリ
	/// </summary>
	public class MasterExportSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MasterExportSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MasterExportSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MasterExportSettingRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region +GetAllByMaster 指定マスタのものを全て取得
		/// <summary>
		/// 指定マスタのものを全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>モデル</returns>
		public MasterExportSettingModel[] GetAllByMaster(string shopId, string masterKbn)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, masterKbn },
			};
			var dv = Get(XML_KEY_NAME, "GetAllByMaster", ht);
			var result = dv.Cast<DataRowView>().Select(drv => new MasterExportSettingModel(drv)).ToArray();
			return result;
		}
		#endregion

		#region +GetCountByMasterKbnAndName マスタ区分と名称が同じものの件数を取得
		/// <summary>
		/// マスタ区分と名称が同じものの件数を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="settingName">設定名</param>
		/// <returns>モデル</returns>
		public int GetCountByMasterKbnAndName(string shopId, string masterKbn, string settingName)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, masterKbn },
				{ Constants.FIELD_MASTEREXPORTSETTING_SETTING_NAME, settingName },
			};
			var dv = Get(XML_KEY_NAME, "GetCountByMasterKbnAndName", ht);
			var result = (int)dv[0][0];
			return result;
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(MasterExportSettingModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(MasterExportSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="settingId">設定ID</param>
		public int Delete(string shopId, string masterKbn, string settingId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, masterKbn },
				{ Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID, settingId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +UpdateSettingIdForDelete 削除時設定ID更新
		/// <summary>
		/// 削除時設定ID更新
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="settingId">設定ID</param>
		public int UpdateSettingIdForDelete(string shopId, string masterKbn, string settingId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, masterKbn },
				{ Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID, settingId },
			};
			var result = Exec(XML_KEY_NAME, "UpdateSettingIdForDelete", ht);
			return result;
		}
		#endregion
	}
}