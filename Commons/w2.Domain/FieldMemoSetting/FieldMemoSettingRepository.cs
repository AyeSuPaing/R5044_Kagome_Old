/*
=========================================================================================================
  Module      : 項目メモ設定リポジトリ (FieldMemoSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.FieldMemoSetting
{
	/// <summary>
	/// 項目メモ設定リポジトリ
	/// </summary>
	internal class FieldMemoSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "FieldMemoSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal FieldMemoSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal FieldMemoSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetList 対象テーブル名で全取得
		/// <summary>
		/// 対象テーブル名で全取得
		/// </summary>
		/// <returns>対象テーブル名の全モデル</returns>
		internal FieldMemoSettingModel[] GetList(string tableName)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIELDMEMOSETTING_TABLE_NAME, tableName}
			};
			var dv = Get(XML_KEY_NAME, "GetList", ht);
			var result = dv.Cast<DataRowView>().Select(drv => new FieldMemoSettingModel(drv)).ToArray();
			return result;
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>モデル</returns>
		internal FieldMemoSettingModel Get(string tableName, string fieldName)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIELDMEMOSETTING_TABLE_NAME, tableName},
				{Constants.FIELD_FIELDMEMOSETTING_FIELD_NAME, fieldName},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			var result = (dv.Count == 0) ? null : new FieldMemoSettingModel(dv[0]);
			return result;
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(FieldMemoSettingModel model)
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
		internal int Update(FieldMemoSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="tableName">テーブル名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(string tableName, string fieldName)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIELDMEMOSETTING_TABLE_NAME, tableName},
				{Constants.FIELD_FIELDMEMOSETTING_FIELD_NAME, fieldName},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
	}
}
