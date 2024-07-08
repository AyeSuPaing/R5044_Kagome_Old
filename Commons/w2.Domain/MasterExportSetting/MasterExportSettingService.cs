/*
=========================================================================================================
  Module      : マスタ出力定義サービス (MasterExportSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using w2.Common.Sql;

namespace w2.Domain.MasterExportSetting
{
	/// <summary>
	/// マスタ出力定義サービス
	/// </summary>
	public class MasterExportSettingService : ServiceBase, IMasterExportSettingService
	{
		#region +GetAllByMaster 指定マスタのものを全て取得
		/// <summary>
		/// 指定マスタのものを全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public MasterExportSettingModel[] GetAllByMaster(string shopId, string masterKbn, SqlAccessor accessor = null)
		{
			using (var repository = new MasterExportSettingRepository(accessor))
			{
				var models = repository.GetAllByMaster(shopId, masterKbn);
				return models;
			}
		}
		#endregion

		/// <summary>
		/// マスタ出力定義設定XMLから指定マスタのHashtable取得(Type)
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>タイプのみ取得</returns>
		public Hashtable GetMasterExportSettingTypes(string masterKbn)
		{
			var temp = GetFieldElements(GetMasterKbnException(masterKbn))
				.ToDictionary(
					key => key.Attribute(Constants.MASTEREXPORTSETTING_XML_NAME).Value,
					value => (value.Attribute(Constants.MASTEREXPORTSETTING_XML_TYPE) != null)
						? value.Attribute(Constants.MASTEREXPORTSETTING_XML_TYPE).Value
						: "");

			return new Hashtable(temp);
		}

		/// <summary>
		/// マスタ出力定義設定XMLから指定マスタのノード取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>マスタ出力セッティングファイル(Fieldノード要素)</returns>
		private IEnumerable<XElement> GetFieldElements(string masterKbn)
		{
			var result = XElement
				.Load(AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MASTEREXPORTSETTING_SETTING)
				.Elements(masterKbn)
				.Elements("Field")
				.Distinct();
			return result;
		}

		/// <summary>
		/// マスタ区分取得（例外用）
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>マスタ区分</returns>
		/// <remarks>ワークフローの場合マスタ定義は注文と同一情報を利用するためここで変換</remarks>
		public string GetMasterKbnException(string masterKbn)
		{
			switch (masterKbn)
			{
				// 注文マスタ表示（ワークフロー）の場合
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW:
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER;
					break;

				// 注文商品マスタ表示（ワークフロー）の場合
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM_WORKFLOW:
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM;
					break;

				// 注文セットプロモーションマスタ（ワークフロー）の場合
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION_WORKFLOW:
					masterKbn = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION;
					break;
			}
			return masterKbn;
		}

		#region +CheckNameDpulication 名称が重複しているかチェック
		/// <summary>
		/// 名称が重複しているかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="settingName">設定名</param>
		/// <returns>重複している</returns>
		public bool CheckNameDuplication(string shopId, string masterKbn, string settingName)
		{
			using (var repository = new MasterExportSettingRepository())
			{
				var count = repository.GetCountByMasterKbnAndName(shopId, masterKbn, settingName);
				return (count > 0);
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(MasterExportSettingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new MasterExportSettingRepository(accessor))
			{
				var settingCount = repository.GetAllByMaster(model.ShopId, model.MasterKbn).Length;
				model.SettingId = (settingCount + 1).ToString("000");

				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(MasterExportSettingModel model)
		{
			using (var repository = new MasterExportSettingRepository())
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="settingId">設定ID</param>
		public void Delete(string shopId, string masterKbn, string settingId)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new MasterExportSettingRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				repository.Delete(shopId, masterKbn, settingId);
				repository.UpdateSettingIdForDelete(shopId, masterKbn, settingId);

				accessor.CommitTransaction();
			}
		}
		#endregion
	}
}