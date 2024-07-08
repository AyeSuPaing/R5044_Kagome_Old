/*
=========================================================================================================
 Module      : ポイント処理ヘルパ(PointOperationHelper.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.Point.Helper
{
	/// <summary>
	/// ポイント処理ヘルパ
	/// </summary>
	internal static class PointOperationHelper
	{
		/// <summary>
		/// 履歴情報からユーザーポイント作成
		/// </summary>
		/// <param name="history">ユーザーポイント履歴</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="forceToBasePoint">強制的に通常ポイントとするか？</param>
		/// <returns>ユーザーポイント</returns>
		internal static UserPointModel CreateUserPointFromHistory(
			UserPointHistoryModel history,
			string lastChanged,
			bool forceToBasePoint)
		{
			// 履歴をもとにユーザーポイント作成
			var userPoint = new UserPointModel
			{
				UserId = history.UserId,
				LastChanged = lastChanged,
				DateChanged = DateTime.Now,
				DateCreated = DateTime.Now,
				PointRuleId = history.PointRuleId,
				PointRuleKbn = history.PointRuleKbn,
				PointKbn = history.PointKbn,
				PointType = history.PointType,
				PointIncKbn = history.PointIncKbn,
				EffectiveDate = history.EffectiveDate,
				PointExp = history.UserPointExp,
				Point = (history.PointInc * -1)
			};

			if (userPoint.IsBasePoint || forceToBasePoint)
			{
				userPoint.PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE;
				userPoint.EffectiveDate = null;
				userPoint.PointIncKbn = string.Empty;
				userPoint.PointRuleKbn = string.Empty;
				userPoint.PointRuleId = string.Empty;
				userPoint.OrderId = string.Empty;
			}

			if ((userPoint.IsBasePoint == false) && forceToBasePoint)
			{
				userPoint.PointExp = UserPointModel.GetDefaultExp();
			}

			return userPoint;
		}

		/// <summary>
		/// 復元処理用にポイント履歴取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="isBeforeMigration">VUP前の履歴であったか？</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザーポイント履歴</returns>
		internal static UserPointHistoryModel[] GetUserPointHistoriesForRestore(
			string userId,
			string orderId,
			string lastChanged,
			out bool isBeforeMigration,
			SqlAccessor accessor)
		{
			var service = new PointService();

			// 履歴を取得する
			var histories = service.GetUserPointHistoryByOrderId(userId, orderId, accessor)
				.Where(h => (h.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
				.ToArray();

			// NA指定されている履歴有り＆NOT RESTOREDの履歴無し→V5.13前と判定
			isBeforeMigration = histories
				.Any(h => (h.IsRestoredFlgAvailable == false))
					&& (histories.Any(h => h.IsRestored) == false);

			// V5.13以前のポイント履歴への対応
			if (isBeforeMigration)
			{
				histories = CreateDummyHistoryForBeforeMigration(userId, orderId, lastChanged, accessor);
			}

			histories = histories
				.Where(h => (h.IsRestored == false))
				.ToArray();

			return histories;
		}

		/// <summary>
		/// 注文IDからダミーの履歴情報作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザーポイント履歴</returns>
		/// <remarks>
		/// 期間限定ポイントに対応する前と後でポイント履歴の持ち方が変わる。
		/// 5.13以前の注文を正常に処理するため、ダミーのポイント履歴を作成する。
		/// </remarks>
		private static UserPointHistoryModel[] CreateDummyHistoryForBeforeMigration(
			string userId,
			string orderId,
			string lastChanged,
			SqlAccessor accessor)
		{
			var pointService = new PointService();

			var historiesBeforeMigration = pointService.GetUserPointHistoryByOrderId(userId, orderId, accessor)
				.Where(
					x => (x.PointType == Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP)
						&& (x.RestoredFlg == Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NA)
						&& (x.PointIncKbn != Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_ADJUST_USE_POINT_FROM_FIXED_PURCHASE_TO_ORDER))
				.ToArray();

			var historiesBeforeMigrationFirst = historiesBeforeMigration.FirstOrDefault();

			// ダミーの履歴を作成
			var dummyHistory = new UserPointHistoryModel
			{
				UserId = userId,
				DeptId = Constants.CONST_DEFAULT_DEPT_ID,
				PointKbn = Constants.FLG_USERPOINTHISTORY_POINT_KBN_BASE,
				PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
				// ポイント利用、利用ポイント調整の合算値が最終利用ポイントになる
				PointInc = historiesBeforeMigration.Sum(x => x.PointInc),
				UserPointExp = historiesBeforeMigration.Max(x => x.UserPointExp),
				PointExpExtend = UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING,
				Kbn1 = orderId,
				Memo = string.Empty,
				LastChanged = lastChanged,
				EffectiveDate = null,
				RestoredFlg = Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NOT_RESTORED,
				FixedPurchaseId = (historiesBeforeMigrationFirst != null)
					? historiesBeforeMigrationFirst.FixedPurchaseId
					: string.Empty
			};

			var histories = new[] { dummyHistory };
			return histories;
		}
	}
}
