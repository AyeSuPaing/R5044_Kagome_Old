/*
=========================================================================================================
  Module      : 表示設定管理サービスのインターフェース (IManagerListDispSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ManagerListDispSetting
{
	/// <summary>
	/// 表示設定管理サービスのインターフェース
	/// </summary>
	public interface IManagerListDispSettingService : IService
	{
		/// <summary>
		/// 設定区分先の表示設定を全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="dispSettingKbn">表示設定先区分</param>
		/// <returns>モデル配列</returns>
		ManagerListDispSettingModel[] GetAllByDispSettingKbn(string shopId, object dispSettingKbn);

		/// <summary>
		/// 画面に表示するための表示設定、注文IDを除いたデータを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="dispSettingKbn">表示設定先区分</param>
		/// <returns>モデル配列</returns>
		ManagerListDispSettingModel[] GetForDispSettingItemNotIncludedOrderId(
			string shopId,
			object dispSettingKbn);

		/// <summary>
		/// 表示区分先の設定情報を更新する
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		int UpdateDispSetting(ManagerListDispSettingModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 項目の表示フラグをOFFにする
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="dispSettingKbn">表示設定先区分</param>
		/// <param name="dispColmunName">表示項目名</param>
		/// /// <param name="accessor">SQLアクセサ</param>
		int UpdateDispSettingFlagOff(
			string shopId,
			string lastChanged,
			object dispSettingKbn,
			string dispColmunName,
			SqlAccessor accessor = null);
	}
}
