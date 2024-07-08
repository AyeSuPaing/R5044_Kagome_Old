/*
=========================================================================================================
  Module      : 決済カード連携サービス (UserCreditCardService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.UserCreditCard
{
	/// <summary>
	/// 決済カード連携サービス
	/// </summary>
	public class UserCreditCardService : ServiceBase, IUserCreditCardService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserCreditCardModel Get(string userId, int branchNo, SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var model = repository.Get(userId, branchNo);
				return model;
			}
		}
		#endregion

		#region +GetByUserId ユーザーIDで取得
		/// <summary>
		/// ユーザーIDで取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserCreditCardModel[] GetByUserId(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var model = repository.GetByUserId(userId);
				return model;
			}
		}
		#endregion

		#region +GetByCooperationId1 連携ID1から取得
		/// <summary>
		/// 連携ID1から取得
		/// </summary>
		/// <param name="cooperationId1">連携ID1</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserCreditCardModel GetByCooperationId1(string cooperationId1, SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var model = repository.GetByCooperationId1(cooperationId1);
				return model;
			}
		}
		#endregion

		#region +GetUsableOrUnregisterd  利用可能かユーザークレカ未登録のものを取得（管理画面で利用）
		/// <summary>
		///  利用可能かユーザークレカ未登録のものを取得（管理画面で利用）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード情報</returns>
		public UserCreditCardModel[] GetUsableOrUnregisterd(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var models = repository.GetUsableOrUnregisterd(userId);
				return models;
			}
		}
		#endregion

		#region +GetUsableOrByBranchno 利用可能か枝番から取得（管理画面で利用）
		/// <summary>
		/// 利用可能か枝番から取得（管理画面で利用）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード情報</returns>
		public UserCreditCardModel[] GetUsableOrByBranchno(string userId, int branchNo, SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var models = repository.GetUsableOrByBranchno(userId, branchNo);
				return models;
			}
		}
		#endregion

		#region +GetUsable 利用可能なもの取得
		/// <summary>
		/// 利用可能なもの取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード情報</returns>
		public UserCreditCardModel[] GetUsable(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var models = repository.GetUsable(userId);
				return models;
			}
		}
		#endregion
		
		#region +GetMaxBranchNo 枝番の最大値取得
		/// <summary>
		/// 枝番の最大値取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>枝番の最大値</returns>
		public int GetMaxBranchNo(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var maxBranchNo = repository.GetMaxBranchNo(userId);
				return maxBranchNo;
			}
		}
		#endregion

		#region +GetCooperationId2ForZeus ゼウス用の決済連携ID2を取得
		/// <summary>
		/// ゼウス用の決済連携ID2を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">ブランチNo</param>
		/// <returns>ゼウス用の決済連携ID2</returns>
		public string GetCooperationId2ForZeus(string userId, int branchNo)
		{
			var cooperationId2 = userId + branchNo.ToString().PadLeft(5, '0');
			return cooperationId2;
		}
		#endregion

		#region +GetDeleteMemberByEScott e-SCOTTの削除対象会員のクレカ取得
		/// <summary>
		/// e-SCOTTの削除対象会員のクレカ取得
		/// </summary>
		/// <param name="userIdList">ユーザーIDリスト</param>
		/// <param name="dateFrom">日付(From)</param>
		/// <param name="dateTo">日付(To)</param>
		/// <param name="accessor">SQLアクセサ</param>
		public UserCreditCardModel[] GetDeleteMemberByEScott(
			List<string> userIdList,
			DateTime dateFrom,
			DateTime dateTo,
			SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var result = repository.GetDeleteMemberByEScott(userIdList, dateFrom, dateTo);
				return result;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル（枝番は採番される）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>発行した枝番</returns>
		public int Insert(UserCreditCardModel model, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var branchNo = Insert(model, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return branchNo;
			}
		}
		#endregion
		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル（枝番は採番される）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行した枝番</returns>
		public int Insert(UserCreditCardModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// 登録
			var branchNo = Insert(model, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, model.LastChanged, accessor);
			}
			return branchNo;
		}
		#endregion
		#region -Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル（枝番は採番される）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行した枝番</returns>
		private int Insert(UserCreditCardModel model, SqlAccessor accessor)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				// SBPSで4桁で来ても2桁で保存する
				if (model.ExpirationYear.Length == 4) model.ExpirationYear = model.ExpirationYear.Substring(2, 2);
				
				var maxBranchNo = repository.GetMaxBranchNo(model.UserId);
				model.BranchNo = maxBranchNo + 1;

				repository.Insert(model);

				return model.BranchNo;
			}
		}
		#endregion

		#region +Modify 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>影響を受けた件数</returns>
		public bool Modify(
			string userId,
			int branchNo,
			Action<UserCreditCardModel> updateAction,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Modify(userId, branchNo, updateAction, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +Modify 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public bool Modify(
			string userId,
			int branchNo,
			Action<UserCreditCardModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 最新データ取得
			var useCcredtCard = Get(userId, branchNo, accessor);

			// モデル内容更新
			updateAction(useCcredtCard);

			// 更新
			var updated = Update(useCcredtCard, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, useCcredtCard.LastChanged, accessor);
			}
			return updated;
		}
		#endregion

		#region +Update 更新　※モバイル版でのみ利用
		/// <summary>
		/// 更新　※モバイル版でのみ利用
		/// </summary>
		/// <param name="model">ユーザークレジットカードモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>影響件数</returns>
		public bool Update(UserCreditCardModel model, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Update(model, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +Update 更新　※モバイル版でのみ利用
		/// <summary>
		/// 更新　※モバイル版でのみ利用
		/// </summary>
		/// <param name="model">ユーザークレジットカードモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響件数</returns>
		public bool Update(UserCreditCardModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// 更新
			var updated = Update(model, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, model.LastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">ユーザークレジットカードモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響件数</returns>
		private bool Update(UserCreditCardModel model, SqlAccessor accessor)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var updated = repository.Update(model);
				return (updated > 0);
			}
		}
		#endregion

		#region +UpdateCooperationId2 連携ID2のみ更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">ユーザークレジットカードモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新成功か</returns>
		public bool UpdateCooperationId2(UserCreditCardModel model, SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var updated = repository.UpdateCooperationId2(model);
				return (updated > 0);
			}
		}
		#endregion

		#region +UpdateCard カード更新
		/// <summary>
		/// クレジットカード更新
		/// </summary>
		/// <param name="model">ユーザークレジットカード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>影響件数</returns>
		public bool UpdateCardDisplayName(UserCreditCardModel model, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new UserCreditCardRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				var updated= repository.UpdateCardDisplayName(model);

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

		#region +UpdateCooperationId 連携ID更新
		/// <summary>
		/// 連携ID更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="cooperationId">連携ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新 成功/失敗</returns>
		public bool UpdateCooperationId(
			string userId,
			int branchNo,
			string cooperationId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				var updated = UpdateCooperationId(userId, branchNo, cooperationId, lastChanged, accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region -UpdateCooperationId 連携ID更新
		/// <summary>
		/// 連携ID更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="cooperationId">連携ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新 成功/失敗</returns>
		private bool UpdateCooperationId(
			string userId,
			int branchNo,
			string cooperationId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var model = repository.Get(userId, branchNo);
				model.CooperationId = cooperationId;
				model.LastChanged = lastChanged;

				var updated = repository.Update(model);

				return (updated > 0);
			}
		}
		#endregion

		#region +UpdateCooperationId2ForEScottDeleteMember e-SCOTT会員削除に伴う連携ID2更新
		/// <summary>
		/// e-SCOTT会員削除に伴う連携ID2更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="cooperationId">連携ID1</param>
		/// <param name="cooperationId2">連携ID2</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新 成功/失敗</returns>
		public bool UpdateCooperationId2ForEScottDeleteMember(
			string userId,
			string cooperationId,
			string cooperationId2,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				var updated = UpdateCooperationId2ForEScottDeleteMember(
					userId,
					cooperationId,
					cooperationId2,
					lastChanged,
					accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region -UpdateCooperationId2ForEScottDeleteMember e-SCOTT会員削除に伴う連携ID2更新
		/// <summary>
		/// e-SCOTT会員削除に伴う連携ID2更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="cooperationId">連携ID1</param>
		/// <param name="cooperationId2">連携ID2</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新 成功/失敗</returns>
		private bool UpdateCooperationId2ForEScottDeleteMember(
			string userId,
			string cooperationId,
			string cooperationId2,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var updated = 0;

				var modelList = repository.GetByUserIdAndCooperationId(userId, cooperationId);
				foreach (var model in modelList)
				{
					model.CooperationId2 = cooperationId2;
					model.LastChanged = lastChanged;

					updated = repository.Update(model);
				}

				return (updated > 0);
			}
		}
		#endregion

		#region +UpdateDispFlg 表示フラグ更新
		/// <summary>
		/// 表示フラグ更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="dispFlg">表示フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新 成功/失敗</returns>
		public bool UpdateDispFlg(
			string userId,
			int branchNo,
			bool dispFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = UpdateDispFlg(userId, branchNo, dispFlg, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion
		#region +UpdateDispFlg 表示フラグ更新	※履歴は落とさないため、呼び出し元で履歴を落としてください。
		/// <summary>
		/// 表示フラグ更新	※履歴は落とさないため、呼び出し元で履歴を落としてください。
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="dispFlg">表示フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新 成功/失敗</returns>
		public bool UpdateDispFlg(
			string userId,
			int branchNo,
			bool dispFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				// 更新
				var model = repository.Get(userId, branchNo);
				model.DispFlg = dispFlg ? Constants.FLG_USERCREDITCARD_DISP_FLG_ON : Constants.FLG_USERCREDITCARD_DISP_FLG_OFF;
				model.LastChanged = lastChanged;
				var result = (repository.Update(model) > 0);

				// 更新履歴登録
				if (result && (updateHistoryAction == UpdateHistoryAction.Insert))
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>結果</returns>
		public bool Delete(
			string userId,
			int branchNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 削除
				var deleted = Delete(userId, branchNo, accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				accessor.CommitTransaction();
				return deleted;
			}
		}
		#endregion
		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		public bool Delete(string userId, int branchNo, SqlAccessor accessor)
		{
			using (var repository = new UserCreditCardRepository(accessor))
			{
				var result = repository.Delete(userId, branchNo);
				return result > 0;
			}
		}
		#endregion

		#region GetMaxBranchNoByUserIdAndCooperationType
		/// <summary>
		/// Get Max Branch No By User Id And Cooperation Type
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="cooperationType">Cooperation Type</param>
		/// <returns>Max Branch No</returns>
		public int GetMaxBranchNoByUserIdAndCooperationType(
			string userId,
			string cooperationType)
		{
			using (var repository = new UserCreditCardRepository())
			{
				var result = repository.GetMaxBranchNoByUserIdAndCooperationType(
					userId,
					cooperationType);

				return result;
			}
		}
		#endregion

		#region +GetUserCreditCardExpiredForPaymentPaidys
		/// <summary>
		/// Get User Credit Card Expired For Payment Paidys
		/// </summary>
		/// <param name="paymentPaidyTokenDeleteLimitDay">Payment Paidy Token Delete Limit Day</param>
		/// <param name="cooperationType">Cooperation Type</param>
		/// <param name="maskString">Mask String</param>
		/// <returns>User Credit Card Expired For Payment Paidys</returns>
		public List<UserCreditCardModel> GetUserCreditCardExpiredForPaymentPaidys(
			int paymentPaidyTokenDeleteLimitDay,
			string cooperationType,
			string maskString)
		{
			using (var repository = new UserCreditCardRepository())
			{
				var result = repository.GetUserCreditCardExpiredForPaymentPaidys(
					paymentPaidyTokenDeleteLimitDay,
					cooperationType,
					maskString);

				return result;
			}
		}
		#endregion
	}
}
