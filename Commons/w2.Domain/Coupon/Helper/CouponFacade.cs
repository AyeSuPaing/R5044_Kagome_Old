/*
=========================================================================================================
  Module      : クーポンに関する複数の業務を統合するクラス (CouponFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.Coupon.Helper
{
	/// <summary>
	/// クーポンに関する複数の業務を統合するクラス
	/// </summary>
	internal class CouponFacade
	{
		#region ~DeleteUserCoupon 指定されたユーザークーポン情報を削除し、ユーザークーポン履歴に登録
		/// <summary>
		/// 指定されたユーザークーポン情報を削除し、ユーザークーポン履歴に登録
		/// </summary>
		/// <param name="deleteCouponInfo">削除ユーザークーポン情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQL接続</param>
		/// <returns>削除件数</returns>
		internal int DeleteUserCoupon(
			UserCouponDetailInfo deleteCouponInfo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			using (var repository = new CouponRepository(accessor))
			{
				// ユーザクーポン情報削除
				var deleteResult = repository.DeleteUserCoupon(deleteCouponInfo);
				if (deleteResult > 0)
				{
					// ユーザクーポン情報履歴登録
					var historyModel = this.CreateUserCouponHistoryModel(
						deleteCouponInfo,
						Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH,
						Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_OPERATOR,
						-1 * (deleteCouponInfo.UserCouponCount ?? 1),
						(deleteCouponInfo.DiscountPrice != null) ? deleteCouponInfo.DiscountPrice.GetValueOrDefault() * -1 : 0);
					repository.InsertUserCouponHistory(historyModel);

					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertForUser(deleteCouponInfo.UserId, lastChanged, accessor);
					}
				}
				return deleteResult;
			}
		}
		#endregion

		#region ~DeleteExpiredUserCoupon 期限切れユーザクーポン削除
		/// <summary>
		/// 期限切れユーザクーポン削除
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQL接続</param>
		/// <returns>削除件数</returns>
		internal int DeleteExpiredUserCoupon(string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			using (var repository = new CouponRepository(accessor))
			{
				int updateCount = 0;
				// 未使用の期限切れユーザクーポン取得
				var expiredCoupons = repository.GetExpiredUserCoupons();

				// 期限切れ利用回数制限クーポンを使用済みに変更
				foreach (var coupon in expiredCoupons.Where(coupon => (coupon.UserCouponCount != null)))
				{
					if (coupon.CouponNo == null) continue;

					var userCoupon = new UserCouponModel
					{
						UserId = coupon.UserId,
						DeptId = coupon.DeptId,
						CouponId = coupon.CouponId,
						CouponNo = (int)coupon.CouponNo,
						UseFlg = Constants.FLG_USERCOUPON_USE_FLG_USE,
						DateChanged = DateTime.Now,
						LastChanged = lastChanged,
					};
					var result = repository.UpdateUserCouponUseFlg(userCoupon);

					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertForUser(coupon.UserId, lastChanged, accessor);
					}
					updateCount += result;
				}

				foreach (var userCooupon in expiredCoupons.Where(coupon => (coupon.UserCouponCount == null)))
				{
					// 期限切れユーザクーポン削除＆履歴インサート
					var memo = "有効期限切れ：" + DateTime.Now.ToString("yyyy/MM/dd");
					var deleteCoupon =
						this.CreateUserCouponHistoryModel(userCooupon,
						Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_EXPIRE,
						Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_BASE,
						-1,
						(userCooupon.DiscountPrice != null) ? userCooupon.DiscountPrice.GetValueOrDefault() : 0,
						memo);
					var result = repository.DeleteExpiredUserCoupon(deleteCoupon);

					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertForUser(userCooupon.UserId, lastChanged, accessor);
					}
					updateCount += result;
				}

				return updateCount;
			}
		}
		#endregion

		#region ~InsertUserCoupon 指定されたユーザークーポン情報を登録し、ユーザークーポン履歴に登録
		/// <summary>
		/// 指定されたユーザークーポン情報を登録し、ユーザークーポン履歴に登録
		/// </summary>
		/// <param name="insertCouponInfo">登録ユーザークーポン情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQL接続</param>
		/// <param name="isPublishByOperator">オペレータが発行するか</param>
		/// <returns>発行件数</returns>
		internal int InsertUserCoupon(
			UserCouponDetailInfo insertCouponInfo,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool isPublishByOperator = true)
		{
			using (var repository = new CouponRepository(accessor))
			{
				// ユーザークーポン情報登録
				var couponModel = this.CreateUserCouponModel(insertCouponInfo);
				var insertResult = repository.InsertUserCoupon(couponModel);

				if (insertResult > 0)
				{
					// ユーザクーポン情報履歴登録
					var historyModel = this.CreateUserCouponHistoryModel(
						insertCouponInfo,
						Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH,
						isPublishByOperator
							? Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_OPERATOR
							: Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_BASE,
						insertCouponInfo.UserCouponCount ?? 1,
						(insertCouponInfo.DiscountPrice != null) ? insertCouponInfo.DiscountPrice.GetValueOrDefault() : 0);
					repository.InsertUserCouponHistory(historyModel);

					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertForUser(insertCouponInfo.UserId, insertCouponInfo.LastChanged, accessor);
					}
				}
				return insertResult;
			}
		}
		#endregion

		#region ~GetCouponTransitionInfo クーポン推移レポート検索して、結果データを統合する
		/// <summary>
		/// クーポン推移レポート検索して、結果データを統合する
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="accessor">SQL接続</param>
		/// <returns>検索結果</returns>
		internal CouponTransitionReportResult[] GetCouponTransitionInfo(CouponTransitionReportCondition condition, SqlAccessor accessor)
		{
			var isDailyReport = (condition.ReportType == ReportType.Day);
			using (var repository = new CouponRepository())
			{
				// 日別・月別のレポート種類によりデータ取得
				var couponTransDetail = repository.SearchCouponTransition(condition);
				var unusedCouponPriceTransDetail = isDailyReport
					? repository.SearchUnusedCouponPriceTransitionDay(condition)
					: repository.SearchUnusedCouponPriceTransitionMonth(condition);
				var unusedCouponCountTransDetail = isDailyReport
					? repository.SearchUnusedCouponCountTransitionDay(condition)
					: repository.SearchUnusedCouponCountTransitionMonth(condition);

				var searchResult = this.MergeCouponTransitionResult(
					condition,
					couponTransDetail,
					unusedCouponPriceTransDetail,
					unusedCouponCountTransDetail);
				return searchResult;
			}
		}
		#endregion

		#region -CreateUserCouponModel ユーザークーポン詳細情報を元に、クーポンモデルを生成
		/// <summary>
		/// ユーザークーポン詳細情報を元に、クーポンモデルを生成
		/// </summary>
		/// <param name="userCouponInfo">ユーザークーポン詳細情報</param>
		/// <returns>ユーザークーポンモデル</returns>
		private UserCouponModel CreateUserCouponModel(UserCouponDetailInfo userCouponInfo)
		{
			var couponModel = new UserCouponModel
			{
				// データ設定
				UserId = userCouponInfo.UserId,
				DeptId = userCouponInfo.DeptId,
				CouponId = userCouponInfo.CouponId,
				CouponNo = userCouponInfo.CouponNo.GetValueOrDefault(),
				OrderId = string.Empty,
				UseFlg = userCouponInfo.UseFlg,
				LastChanged = userCouponInfo.LastChanged,
				UserCouponCount = userCouponInfo.UserCouponCount
			};
			return couponModel;
		}
		#endregion

		#region -CreateUserCouponHistoryModel ユーザークーポン詳細情報を元に、ユーザークーポン履歴モデルを生成
		/// <summary>
		/// ユーザークーポン詳細情報を元に、ユーザークーポン履歴モデルを生成
		/// </summary>
		/// <param name="userCouponInfo">ユーザークーポン詳細情報</param>
		/// <param name="historyKbn">クーポン履歴区分</param>
		/// <param name="actionKbn">操作区分</param>
		/// <param name="couponInc">加算数</param>
		/// <param name="couponPrice">クーポン金額</param>
		/// <param name="memo">メモ</param>
		/// <returns>ユーザークーポン履歴モデル</returns>
		private UserCouponHistoryModel CreateUserCouponHistoryModel(UserCouponDetailInfo userCouponInfo,
			string historyKbn,
			string actionKbn,
			decimal couponInc,
			decimal couponPrice,
			string memo = "")
		{
			var couponHistoryModel = new UserCouponHistoryModel()
			{
				// データ設定
				UserId = userCouponInfo.UserId,
				DeptId = userCouponInfo.DeptId,
				CouponId = userCouponInfo.CouponId,
				CouponNo = userCouponInfo.CouponNo.GetValueOrDefault(),
				CouponCode = userCouponInfo.CouponCode,
				OrderId = StringUtility.ToEmpty(userCouponInfo.OrderId),
				HistoryKbn = historyKbn,
				ActionKbn = actionKbn,
				CouponInc = couponInc,
				Memo = memo,
				CouponPrice = couponPrice,
				LastChanged = userCouponInfo.LastChanged,
				UserCouponCount = userCouponInfo.UserCouponCount
			};
			return couponHistoryModel;
		}
		#endregion

		#region -MergeCouponTransitionResult クーポン推移検索結果マージ
		/// <summary>
		/// クーポン推移検索結果マージ
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="couponTransDetail">クーポン推移情報</param>
		/// <param name="unusedCouponPriceTransDetail">未利用クーポン金額推移情報</param>
		/// <param name="unusedCouponCountTransDetail">未利用クーポン枚数推移情報</param>
		/// <returns>クーポン推移レポート検索結果</returns>
		private CouponTransitionReportResult[] MergeCouponTransitionResult(
			CouponTransitionReportCondition condition,
			CouponTransitionInfo[] couponTransDetail,
			UnusedCouponTransitionInfo[] unusedCouponPriceTransDetail,
			UnusedCouponTransitionInfo[] unusedCouponCountTransDetail)
		{
			var searchResult = new List<CouponTransitionReportResult>();

			//------------------------------------------------------
			// 月の最終日を取得
			//------------------------------------------------------
			int lastIndex = 0;
			// 日毎？
			if (condition.ReportType == ReportType.Day)
			{
				DateTime dateTemp = new DateTime(int.Parse(condition.Year), int.Parse(condition.Month), 1);
				lastIndex = dateTemp.AddMonths(1).AddDays(-1).Day;
			}
			else
			{
				// 月毎？
				lastIndex = 12;
			}

			int countIndex1 = 0;
			int countIndex2 = 0;
			int countIndex3 = 0;
			CouponTransitionReportResult transDetailMerged = null;
			//------------------------------------------------------
			// データ格納
			//------------------------------------------------------
			for (int loopIndex = 1; loopIndex <= lastIndex; loopIndex++)
			{
				transDetailMerged = new CouponTransitionReportResult();

				// データ取得可否判定
				bool dataFlg1 = false;
				if (countIndex1 < couponTransDetail.Length)
				{
					int month = int.Parse(couponTransDetail[countIndex1].TgtMonth);	// ★
					int day = int.Parse(couponTransDetail[countIndex1].TgtDay);	// ★

					// 該当日付データ？
					if (((loopIndex == day) && (condition.ReportType == ReportType.Day))
						|| ((loopIndex == month) && (condition.ReportType != ReportType.Day)))
					{
						dataFlg1 = true;
					}
				}

				// データ取得可否判定2
				bool dataFlg2 = false;
				if (countIndex2 < unusedCouponPriceTransDetail.Length)
				{
					int month = int.Parse(unusedCouponPriceTransDetail[countIndex2].TgtMonth);	// ★
					int day = int.Parse(unusedCouponPriceTransDetail[countIndex2].TgtDay);	// ★

					// 該当日付データ？
					if (((loopIndex == day) && (condition.ReportType == ReportType.Day))
						|| ((loopIndex == month) && (condition.ReportType != ReportType.Day)))
					{
						dataFlg2 = true;
					}
				}

				// データ取得可否判定3
				bool dataFlg3 = false;
				if (countIndex3 < unusedCouponCountTransDetail.Length)
				{
					int month = int.Parse(unusedCouponCountTransDetail[countIndex3].TgtMonth);	// ★
					int day = int.Parse(unusedCouponCountTransDetail[countIndex3].TgtDay);	// ★

					// 該当日付データ？
					if (((loopIndex == day) && (condition.ReportType == ReportType.Day))
						|| ((loopIndex == month) && (condition.ReportType != ReportType.Day)))
					{
						dataFlg3 = true;
					}
				}

				// 日付※日毎? or 月毎？
				transDetailMerged.TimeUnit = loopIndex
					+ (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
						? ((condition.ReportType == ReportType.Day) ? "日" : "月")
						: "");

				// 発行クーポン
				transDetailMerged.AddCoupon = (dataFlg1 == false)
					? "－"
					: this.CreateDispData(couponTransDetail[countIndex1].AddCoupon, DataDisplayKbn.Price, true, condition.CurrencyLocaleId, condition.CurrencyLocaleFormat);
				// 発行枚数
				transDetailMerged.AddCount = (dataFlg1 == false)
					? "－"
					: this.CreateDispData(couponTransDetail[countIndex1].AddCount, DataDisplayKbn.Count, true);
				// 利用クーポン
				transDetailMerged.UseCoupon = (dataFlg1 == false)
					? "－"
					: this.CreateDispData(couponTransDetail[countIndex1].UseCoupon, DataDisplayKbn.Price, true, condition.CurrencyLocaleId, condition.CurrencyLocaleFormat);
				// 利用枚数
				transDetailMerged.UseCount = (dataFlg1 == false)
					? "－"
					: this.CreateDispData(couponTransDetail[countIndex1].UseCount, DataDisplayKbn.Count, true);
				// 有効期限切れクーポン
				transDetailMerged.ExpCoupon = (dataFlg1 == false)
					? "－"
					: this.CreateDispData(couponTransDetail[countIndex1].ExpCoupon, DataDisplayKbn.Price, true, condition.CurrencyLocaleId, condition.CurrencyLocaleFormat);
				// 有効期限切れ枚数
				transDetailMerged.ExpCount = (dataFlg1 == false)
					? "－"
					: this.CreateDispData(couponTransDetail[countIndex1].ExpCount, DataDisplayKbn.Count, true);
				// 利用可能クーポン（未利用クーポン）
				transDetailMerged.UnusedCoupon = (dataFlg2 == false)
					? "－"
					: this.CreateDispData(unusedCouponPriceTransDetail[countIndex2].UnusedPrice, DataDisplayKbn.Price, true, condition.CurrencyLocaleId, condition.CurrencyLocaleFormat);
				// 利用可能枚数（未利用クーポン）
				transDetailMerged.UnusedCount = (dataFlg3 == false)
					? "－"
					: this.CreateDispData(unusedCouponCountTransDetail[countIndex3].UnusedCount, DataDisplayKbn.Count, true);

				// 追加
				searchResult.Add(transDetailMerged);

				countIndex1 += (dataFlg1) ? 1 : 0;
				countIndex2 += (dataFlg2) ? 1 : 0;
				countIndex3 += (dataFlg3) ? 1 : 0;
			}

			return searchResult.ToArray();
		}
		#endregion

		#region -CreateDispData クーポン推移レポートの１カラム分表示データ作成
		/// <summary>
		/// クーポン推移レポートの１カラム分表示データ作成
		/// </summary>
		/// <param name="value">入力データ</param>
		/// <param name="dispKbn">データ表示区分</param>
		/// <param name="insertDataFlg">実データ追加可否</param>
		/// <param name="currencyLocaleId">現在のロケールID</param>
		/// <param name="currencyLocaleFormat">現在のロケールフォーマット</param>
		/// <returns>表示データ</returns>
		private string CreateDispData(
			decimal value,
			DataDisplayKbn dispKbn,
			bool insertDataFlg,
			string currencyLocaleId = "ja-JP",
			string currencyLocaleFormat = "{0:C}")
		{
			if (insertDataFlg)
			{
				// 金額表示の場合
				if (dispKbn == DataDisplayKbn.Price)
				{
					if (value == 0)
					{
						return "(±)" + StringUtility.ToPrice(value, currencyLocaleId, currencyLocaleFormat);
					}
					else if (value < 0)
					{
						return "(-)" + StringUtility.ToPrice(
							(decimal)(value * -1),
							currencyLocaleId,
							currencyLocaleFormat);
					}
					else if (value > 0)
					{
						return "(+)" + StringUtility.ToPrice(value, currencyLocaleId, currencyLocaleFormat);
					}
				}
				else if (dispKbn == DataDisplayKbn.Count)
				{
					// 枚数表示
					return StringUtility.ToNumeric(value) + "枚";
				}
			}

			return "－";
		}
		#endregion
	}
}
