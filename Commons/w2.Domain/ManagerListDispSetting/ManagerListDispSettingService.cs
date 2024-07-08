/*
=========================================================================================================
  Module      : 表示設定管理サービス (ManagerListDispSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ManagerListDispSetting
{
	/// <summary>
	/// 表示設定管理サービス
	/// </summary>
	public class ManagerListDispSettingService : ServiceBase, IManagerListDispSettingService
	{
		#region +GetAllByDispSettingKbn 取得
		/// <summary>
		/// 設定区分先の表示設定を全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="dispSettingKbn">表示設定先区分</param>
		/// <returns>モデル配列</returns>
		public ManagerListDispSettingModel[] GetAllByDispSettingKbn(string shopId, object dispSettingKbn)
		{
			using (var repository = new ManagerListDispSettingRepository())
			{
				var models = repository.GetAllByDispSettingKbn(shopId, dispSettingKbn);
				return models;
			}
		}
		#endregion

		#region +GetForDispSettingItemNotIncludedOrderId 取得
		/// <summary>
		/// 画面に表示するための表示設定、注文IDを除いたデータを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="dispSettingKbn">表示設定先区分</param>
		/// <returns>モデル配列</returns>
		public ManagerListDispSettingModel[] GetForDispSettingItemNotIncludedOrderId(string shopId, object dispSettingKbn)
		{
			using (var repository = new ManagerListDispSettingRepository())
			{
				var models = repository.GetForDispSettingItemNotIncludedOrderId(shopId, dispSettingKbn);
				return models;
			}
		}
		#endregion

		#region +UpdateDispSetting 更新
		/// <summary>
		/// 表示区分先の設定情報を更新する
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int UpdateDispSetting(ManagerListDispSettingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ManagerListDispSettingRepository(accessor))
			{
				var result = repository.UpdateDispSetting(model);
				return result;
			}
		}
		#endregion

		#region +UpdateDispSettingFlagOff 更新
		/// <summary>
		/// 項目の表示フラグをOFFにする
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="dispSettingKbn">表示設定先区分</param>
		/// <param name="dispColmunName">表示項目名</param>
		/// /// <param name="accessor">SQLアクセサ</param>
		public int UpdateDispSettingFlagOff(string shopId, string lastChanged, object dispSettingKbn, string dispColmunName, SqlAccessor accessor = null)
		{
			using (var repository = new ManagerListDispSettingRepository(accessor))
			{
				var result = repository.UpdateDispSettingFlagOff(shopId, lastChanged, dispSettingKbn, dispColmunName);
				return result;
			}
		}
		#endregion
	}
}