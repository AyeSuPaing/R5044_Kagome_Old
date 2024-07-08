/*
=========================================================================================================
  Module      : 項目メモ設定サービス (FieldMemoSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.FieldMemoSetting
{
	/// <summary>
	/// 項目メモ設定サービス
	/// </summary>
	public class FieldMemoSettingService : ServiceBase
	{
		#region ~GetList 対象テーブル名で全取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public FieldMemoSettingModel[] GetList(string tableName, SqlAccessor accessor = null)
		{
			using (var repository = new FieldMemoSettingRepository(accessor))
			{
				var models = repository.GetList(tableName);
				return models;
			}
		}
		#endregion

		#region +GetFieldMemoSettingListClip 項目メモ一覧取得処理 指定文字数以上は省略
		/// <summary>
		/// 項目メモ一覧取得処理 指定文字数以上は省略
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="clipLength">省略した際の最大表示文字数</param>
		/// <returns>項目メモ一覧データ</returns>
		public FieldMemoSettingModel[] GetFieldMemoSettingListClip(string tableName, SqlAccessor accessor = null, int clipLength = 15)
		{
			var memoList = GetList(tableName, accessor);

			var result = memoList.Select(
				m =>
				{
					if (m.Memo.Length <= clipLength) return m;

					m.Memo = m.Memo.Substring(0, clipLength) + "...";

					if (m.Memo.Contains("\r\n"))
					{
						m.Memo = m.Memo.Substring(0, m.Memo.IndexOf("\r\n")) + "...";
					}
					return m;

				}).ToArray();

			return result;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public FieldMemoSettingModel Get(string tableName, string fieldName, SqlAccessor accessor = null)
		{
			using (var repository = new FieldMemoSettingRepository(accessor))
			{
				var model = repository.Get(tableName, fieldName);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(FieldMemoSettingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FieldMemoSettingRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(FieldMemoSettingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FieldMemoSettingRepository(accessor))
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
		/// <param name="tableName">テーブル名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(string tableName, string fieldName, SqlAccessor accessor = null)
		{
			using (var repository = new FieldMemoSettingRepository(accessor))
			{
				var result = repository.Delete(tableName, fieldName);
				return result;
			}
		}
		#endregion
	}
}