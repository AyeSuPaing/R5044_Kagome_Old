/*
=========================================================================================================
  Module      : ユーザー配送先情報サービス (UserShippingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserDefaultOrderSetting;

namespace w2.Domain.UserShipping
{
	/// <summary>
	/// ユーザー配送先情報サービス
	/// </summary>
	public class UserShippingService : ServiceBase, IUserShippingService
	{
		#region +Search 検索（一覧）
		/// <summary>
		/// 検索（一覧）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>モデル配列</returns>
		public UserShippingModel[] Search(string userId, int beginRowNum, int endRowNum)
		{
			using (var repository = new UserShippingRepository())
			{
				var models = repository.Search(userId, beginRowNum, endRowNum);
				return models;
			}
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(string userId, int beginRowNum, int endRowNum)
		{
			using (var repository = new UserShippingRepository())
			{
				var count = repository.GetSearchHitCount(userId, beginRowNum, endRowNum);
				return count;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="shippingNo">配送先枝番</param>
		/// <returns>モデル</returns>
		public UserShippingModel Get(string userId, int shippingNo)
		{
			using (var repository = new UserShippingRepository())
			{
				var model = repository.Get(userId, shippingNo);
				return model;
			}
		}
		#endregion

		#region +GetAll 取得（全て）
		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		public UserShippingModel[] GetAllOrderByShippingNoDesc(string userId, SqlAccessor accessor = null)
		{
			using (var repository = ((accessor == null) ? new UserShippingRepository() : new UserShippingRepository(accessor)))
			{
				var models = repository.GetAll(userId).OrderByDescending(m => m.ShippingNo).ToArray();
				return models;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>登録されたユーザー配送先枝番</returns>
		public int Insert(UserShippingModel model, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction(IsolationLevel.Serializable);

				var shippingNo = Insert(model, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return shippingNo;
			}
		}
		#endregion
		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録されたユーザー配送先枝番</returns>
		public int Insert(
			UserShippingModel model,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 登録
			var shippingNo = Insert(model, accessor);

			// 更新日と最終更新者を更新
			UpdateUserDateChangedAndLastChanged(model.UserId, lastChanged, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, lastChanged, accessor);
			}
			return shippingNo;
		}
		#endregion
		#region -Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録されたユーザー配送先枝番</returns>
		private int Insert(UserShippingModel model, SqlAccessor accessor)
		{
			using (var repository = new UserShippingRepository(accessor))
			{
				var shippingNo = repository.GetNewShippingNo(model.UserId);
				model.ShippingNo = shippingNo;

				repository.Insert(model);

				return shippingNo;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新されたか</returns>
		public bool Update(UserShippingModel model, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				var updated = Update(model, accessor);

				// 更新日と最終更新者を更新
				UpdateUserDateChangedAndLastChanged(model.UserId, lastChanged, accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(model.UserId, lastChanged, accessor);
				}

				accessor.CommitTransaction();
				return (updated > 0);
			}

		}
		#endregion
		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int Update(UserShippingModel model, SqlAccessor accessor)
		{
			using (var repository = new UserShippingRepository(accessor))
			{
				var updated = repository.Update(model);
				return updated;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="shippingNo">配送先枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="twoClickFlg">2クリック決済フラグ</param>
		/// <returns>更新されたか</returns>
		public bool Delete(string userId, int shippingNo, string lastChanged, UpdateHistoryAction updateHistoryAction, bool twoClickFlg)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新日と最終更新者を更新
				UpdateUserDateChangedAndLastChanged(userId, lastChanged, accessor);

				var updated = Delete(userId, shippingNo, lastChanged, updateHistoryAction, accessor);

				// デフォルト配送先に設定されていた場合は同時に削除する。
				var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(userId, accessor);
				if ((userDefaultOrderSetting != null)
					&& (shippingNo == userDefaultOrderSetting.UserShippingNo))
				{
					userDefaultOrderSetting.UserShippingNo = null;
					new UserDefaultOrderSettingService().Update(userDefaultOrderSetting, accessor);
				}

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="shippingNo">配送先枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新されたか</returns>
		public bool Delete(
			string userId,
			int shippingNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var updated = Delete(userId, shippingNo, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
			return (updated > 0);
		}
		#endregion
		#region -Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="shippingNo">配送先枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数う</returns>
		private int Delete(string userId, int shippingNo, SqlAccessor accessor)
		{
			using (var repository = new UserShippingRepository(accessor))
			{
				var updated = repository.Delete(userId, shippingNo);
				return updated;
			}
		}
		#endregion

		/// <summary>
		/// 更新日と最終更新者を更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void UpdateUserDateChangedAndLastChanged(
			string userId,
			string lastChanged,
			SqlAccessor accessor)
		{
			new UserService().Modify(
				userId,
				userModel =>
				{
					userModel.LastChanged = lastChanged;
				},
				UpdateHistoryAction.Insert,
				accessor);
		}
	}
}
