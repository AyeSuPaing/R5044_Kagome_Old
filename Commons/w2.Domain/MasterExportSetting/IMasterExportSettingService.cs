/*
=========================================================================================================
  Module      : 国ISOコードのマスタテーブルサービスのインターフェース (IMasterExportSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.MasterExportSetting
{
	/// <summary>
	/// マスタ出力定義サービスのインターフェース
	/// </summary>
	public interface IMasterExportSettingService : IService
	{
		/// <summary>
		/// 指定マスタのものを全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		MasterExportSettingModel[] GetAllByMaster(string shopId, string masterKbn, SqlAccessor accessor = null);

		/// <summary>
		/// マスタ出力定義設定XMLから指定マスタのHashtable取得(Type)
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>タイプのみ取得</returns>
		Hashtable GetMasterExportSettingTypes(string masterKbn);

		/// <summary>
		/// マスタ区分取得（例外用）
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>マスタ区分</returns>
		/// <remarks>ワークフローの場合マスタ定義は注文と同一情報を利用するためここで変換</remarks>
		string GetMasterKbnException(string masterKbn);

		/// <summary>
		/// 名称が重複しているかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="settingName">設定名</param>
		/// <returns>重複している</returns>
		bool CheckNameDuplication(string shopId, string masterKbn, string settingName);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void Insert(MasterExportSettingModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int Update(MasterExportSettingModel model);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="settingId">設定ID</param>
		void Delete(string shopId, string masterKbn, string settingId);
	}
}
