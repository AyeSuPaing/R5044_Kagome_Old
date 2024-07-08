/*
=========================================================================================================
  Module      : クーポンサービスクラス (CouponService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Coupon.Helper;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.Coupon
{
	/// <summary>
	/// クーポンサービスクラス
	/// </summary>
	public class CouponService : ServiceBase, ICouponService
	{
		#region +GetUserUsableCoupons ユーザーが利用可能なクーポン取得
		/// <summary>
		/// ユーザーが利用可能なクーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <param name="usedUserJudgeType">クーポン利用済みユーザー判定方法</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		public UserCouponDetailInfo[] GetUserUsableCoupons(
			string deptId,
			string userId,
			string mailAddress,
			string usedUserJudgeType,
			SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var coupons = repository.GetUserUsableCoupons(deptId, userId, mailAddress, usedUserJudgeType);
				return coupons;
			}
		}
		#endregion

		#region +GetUserCoupons ユーザークーポン取得
		/// <summary>
		/// ユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		public UserCouponDetailInfo[] GetUserCoupons(
			string deptId,
			string userId,
			SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var coupons = repository.GetUserCoupons(deptId, userId);
				return coupons;
			}
		}
		#endregion

		#region +GetAllUserCoupons 有効期限にも関わらず、全てのユーザークーポン情報取得
		/// <summary>
		/// 有効期限にも関わらず、全てのユーザークーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		public UserCouponDetailInfo[] GetAllUserCoupons(string deptId, string userId)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				var coupns = GetAllUserCoupons(deptId, userId, accessor);

				accessor.CommitTransaction();
				return coupns;
			}
		}
		#endregion
		#region +GetAllUserCoupons 有効期限にも関わらず、全てのユーザークーポン情報取得
		/// <summary>
		/// 有効期限にも関わらず、全てのユーザークーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		public UserCouponDetailInfo[] GetAllUserCoupons(string deptId, string userId, SqlAccessor accessor)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var coupons = repository.GetAllUserCoupons(deptId, userId);
				return coupons;
			}
		}
		#endregion

		#region +GetAllUserCouponsFromCouponCode ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得
		/// <summary>
		/// ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns>ユーザークーポン情報</returns>
		public UserCouponDetailInfo[] GetAllUserCouponsFromCouponCode(string deptId, string userId, string couponCode)
		{
			using (var repository = new CouponRepository())
			{
				var coupons = repository.GetAllUserCouponsFromCouponCode(deptId, userId, couponCode);
				return coupons;
			}
		}
		#endregion

		#region +GetAllUserCouponsFromCouponCodeIncludeUnavailable ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得(利用不可のものも含む)
		/// <summary>
		/// ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得(利用不可のものも含む)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <param name="referralCode">Referral code</param>
		/// <returns>ユーザークーポン情報</returns>
		public UserCouponDetailInfo[] GetAllUserCouponsFromCouponCodeIncludeUnavailable(
			string deptId,
			string userId,
			string couponCode,
			string referralCode = "")
		{
			using (var repository = new CouponRepository())
			{
				var coupons = repository.GetAllUserCouponsFromCouponCodeIncludeUnavailable(deptId, userId, couponCode);
				var hasUserIdReferred = DomainFacade.Instance.UserService.GetReferredUserId(userId);

				if (string.IsNullOrEmpty(hasUserIdReferred)
					&& string.IsNullOrEmpty(referralCode))
				{
					coupons = coupons
						.Where(coupon => (coupon.CouponType != Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON))
						.ToArray();
				}

				return coupons;
			}
		}
		#endregion

		#region +GetUserCouponFromCouponNo ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// <summary>
		/// ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">枝番</param>
		/// <returns>ユーザークーポン情報</returns>
		public UserCouponDetailInfo GetUserCouponFromCouponNo(string deptId, string userId, string couponId, int couponNo)
		{
			using (var repository = new CouponRepository())
			{
				var coupon = repository.GetUserCouponFromCouponNo(deptId, userId, couponId, couponNo);
				return coupon;
			}
		}
		#endregion

		#region +GetAllUserCouponsFromCouponId ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// <summary>
		/// ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">枝番</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		public UserCouponDetailInfo[] GetAllUserCouponsFromCouponId(string deptId, string userId, string couponId, int couponNo)
		{
			using (var repository = new CouponRepository())
			{
				var coupons = repository.GetAllUserCouponsFromCouponId(deptId, userId, couponId, couponNo);
				return coupons;
			}
		}
		#endregion

		#region +GetUserCouponFromCouponId 識別ID・クーポンIDを元にユーザークーポン取得
		/// <summary>
		/// 識別ID・クーポンIDを元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns>ユーザークーポン情報</returns>
		public UserCouponDetailInfo GetUserCouponFromCouponId(string deptId, string couponId)
		{
			using (var repository = new CouponRepository())
			{
				var coupon = repository.GetUserCouponFromCouponId(deptId, couponId);
				return coupon;
			}
		}
		#endregion

		#region +GetOrderPublishUserCoupon ユーザクーポン情報取得(本注文で発行したクーポン)
		/// <summary>
		/// ユーザクーポン情報取得(本注文で発行したクーポン)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン情報</returns>
		public UserCouponDetailInfo[] GetOrderPublishUserCoupon(string deptId, string userId, string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var userCoupon = repository.GetOrderPublishUserCoupon(deptId, userId, orderId);
				return userCoupon;
			}
		}
		#endregion

		#region +GetUserCouponNo 最大ユーザクーポン枝番取得
		/// <summary>
		/// 最大ユーザクーポン枝番取得
		/// </summary>
		/// <param name="deptId">ユーザID</param>
		/// <param name="userId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>最大ユーザクーポン枝番</returns>
		public int GetUserCouponNo(string deptId, string userId, string couponId, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var couponNo = repository.GetUserCouponNo(deptId, userId, couponId);
				return couponNo;
			}
		}
		#endregion

		#region +GetAllUserUsableCouponsCount 利用可能ユーザークーポン数取得
		/// <summary>
		/// 利用可能ユーザークーポン数取得
		/// </summary>
		/// <param name="condition">クーポン情報検索条件情報</param>
		/// <returns>件数</returns>
		public int GetAllUserUsableCouponsCount(CouponListSearchCondition condition)
		{
			using (var repository = new CouponRepository())
			{
				var result = repository.GetAllUserUsableCouponsCount(condition);
				return result;
			}
		}
		#endregion

		#region +GetAllUserUsableCoupons 利用可能ユーザークーポン取得
		/// <summary>
		/// 利用可能ユーザークーポン取得
		/// </summary>
		/// <param name="condition">クーポン情報検索条件情報</param>
		/// <returns>ユーザクーポン一覧</returns>
		public UserCouponDetailInfo[] GetAllUserUsableCoupons(CouponListSearchCondition condition)
		{
			using (var repository = new CouponRepository())
			{
				var coupons = repository.GetAllUserUsableCoupons(condition);
				return coupons;
			}
		}
		#endregion

		#region +InsertUserCoupon 指定されたユーザークーポン情報を登録
		/// <summary>
		/// 指定されたユーザークーポン情報を登録
		/// </summary>
		/// <param name="couponInfo">登録ユーザークーポン情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="isPublishByOperator">オペレータが発行するか</param>
		/// <returns>登録件数</returns>
		public int InsertUserCoupon(UserCouponDetailInfo couponInfo, UpdateHistoryAction updateHistoryAction, bool isPublishByOperator = true)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				int result = InsertUserCoupon(couponInfo, updateHistoryAction, accessor, isPublishByOperator);

				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion
		#region +InsertUserCoupon 指定されたユーザークーポン情報を登録
		/// <summary>
		/// 指定されたユーザークーポン情報を登録
		/// </summary>
		/// <param name="couponInfo">登録ユーザークーポン情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="isPublishByOperator">オペレータが発行するか</param>
		/// <returns>登録件数</returns>
		public int InsertUserCoupon(UserCouponDetailInfo couponInfo, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, bool isPublishByOperator = true)
		{
			int result = new CouponFacade().InsertUserCoupon(couponInfo, updateHistoryAction, accessor, isPublishByOperator);
			return result;
		}
		#endregion

		#region +InsertUserCouponWithOrderId ユーザークーポン情報登録（注文ID指定あり）
		/// <summary>
		/// ユーザークーポン情報登録（注文ID指定あり）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>処理結果(true: 登録件数 > 0), false: 登録件数 = 0)</returns>
		public bool InsertUserCouponWithOrderId(
			string userId,
			string orderId,
			string deptId,
			string couponId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = InsertUserCouponWithOrderId(
					userId,
					orderId,
					deptId,
					couponId,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		/// <summary>
		/// ユーザークーポン情報登録（注文ID指定あり）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果(true: 登録件数 > 0), false: 登録件数 = 0)</returns>
		public bool InsertUserCouponWithOrderId(
			string userId,
			string orderId,
			string deptId,
			string couponId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var userCoupon = new UserCouponModel()
			{
				UserId = userId,
				DeptId = deptId,
				CouponId = couponId,
				CouponNo = this.GetUserCouponNo(deptId, userId, couponId, accessor),
				OrderId = orderId,
				LastChanged = lastChanged
			};
			var result = this.InsertUserCouponWithOrderId(userCoupon, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}

			return result > 0;
		}
		#endregion
		#region -InsertUserCouponWithOrderId ユーザークーポン情報登録（注文ID指定あり）
		/// <summary>
		/// ユーザークーポン情報登録（注文ID指定あり）
		/// </summary>
		/// <param name="couponInfo">登録ユーザークーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録件数</returns>
		private int InsertUserCouponWithOrderId(UserCouponModel couponInfo, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				int result = repository.InsertUserCouponWithOrderId(couponInfo);
				return result;
			}
		}
		#endregion

		#region +UpdateUserCouponUseFlg ユーザークーポン利用フラグ更新
		/// <summary>
		/// ユーザークーポン利用フラグ更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">クーポンNO</param>
		/// <param name="useFlg">利用フラグ</param>
		/// <param name="dateChanged">最終更新日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新成功か(true: 更新件数 > 0, false: 更新件数 = 0)</returns>
		public bool UpdateUserCouponUseFlg(
			string userId,
			string deptId,
			string couponId,
			int couponNo,
			bool useFlg,
			DateTime dateChanged,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var updated = UpdateUserCouponUseFlg(userId, deptId, couponId, couponNo, useFlg, dateChanged, lastChanged, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -UpdateUserCouponUseFlg ユーザークーポン利用フラグ更新
		/// <summary>
		/// ユーザークーポン利用フラグ更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">クーポンNO</param>
		/// <param name="useFlg">利用フラグ</param>
		/// <param name="dateChanged">最終更新日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新成功か(true: 更新件数 > 0, false: 更新件数 = 0)</returns>
		private bool UpdateUserCouponUseFlg(string userId, string deptId, string couponId, int couponNo, bool useFlg, DateTime dateChanged, string lastChanged, SqlAccessor accessor)
		{
			var userCoupon = new UserCouponModel()
			{
				UserId = userId,
				DeptId = deptId,
				CouponId = couponId,
				CouponNo = couponNo,
				UseFlg = useFlg ? Constants.FLG_USERCOUPON_USE_FLG_USE : Constants.FLG_USERCOUPON_USE_FLG_UNUSE,
				DateChanged = dateChanged,
				LastChanged = lastChanged
			};
			var result = this.UpdateUserCouponUseFlg(userCoupon, accessor);
			return result > 0;
		}
		/// <summary>
		/// ユーザークーポン利用フラグ更新
		/// </summary>
		/// <param name="couponInfo">ユーザークーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int UpdateUserCouponUseFlg(UserCouponModel couponInfo, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateUserCouponUseFlg(couponInfo);
				return result;
			}
		}
		#endregion

		#region +UpdateUnUseUserCoupon ユーザクーポン更新(使用済み→未使用)
		/// <summary>
		/// ユーザクーポン更新(使用済み→未使用)
		/// </summary>
		/// <param name="couponInfo">ユーザークーポン情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateUnUseUserCoupon(UserCouponModel couponInfo, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// 更新
			var updated = UpdateUnUseUserCoupon(couponInfo);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(couponInfo.UserId, couponInfo.LastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -UpdateUnUseUserCoupon ユーザクーポン更新(使用済み→未使用)
		/// <summary>
		/// ユーザクーポン更新(使用済み→未使用)
		/// </summary>
		/// <param name="couponInfo">ユーザークーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int UpdateUnUseUserCoupon(UserCouponModel couponInfo, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateUnUseUserCoupon(couponInfo);
				return result;
			}
		}
		#endregion

		#region UpdateUseCouponCountUserCoupon ユーザークーポン更新(ユーザークーポン利用可能回数)
		/// <summary>
		/// ユーザークーポン更新(ユーザークーポン利用可能回数)
		/// </summary>
		/// <param name="couponInfo">ユーザークーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功/失敗</returns>
		public bool UpdateUseCouponCountUserCoupon(UserCouponModel couponInfo, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateUseCouponCountUserCoupon(couponInfo);
				return result >= 1;
			}
		}
		#endregion

		#region +DeleteUserCoupon 指定されたユーザークーポン情報を削除
		/// <summary>
		/// 指定されたユーザークーポン情報を削除
		/// </summary>
		/// <param name="deleteCouponInfo">削除ユーザークーポン情報</param>
		/// <param name="lastChanged">最終更新者（履歴用）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>削除件数</returns>
		public int DeleteUserCoupon(
			UserCouponDetailInfo deleteCouponInfo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = DeleteUserCoupon(deleteCouponInfo, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion
		#region -DeleteUserCoupon 指定されたユーザークーポン情報を削除
		/// <summary>
		/// 指定されたユーザークーポン情報を削除
		/// </summary>
		/// <param name="deleteCouponInfo">削除ユーザークーポン情報</param>
		/// <param name="lastChanged">最終更新者（履歴用）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		private int DeleteUserCoupon(
			UserCouponDetailInfo deleteCouponInfo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var result = new CouponFacade().DeleteUserCoupon(deleteCouponInfo, lastChanged, updateHistoryAction, accessor);
			return result;
		}
		#endregion

		#region +DeleteUserCouponByOrderId ユーザークーポン削除(注文キャンセル時)
		/// <summary>
		/// ユーザークーポン情報削除(注文キャンセル時)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteUserCouponByOrderId(
			string userId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 削除
			var updated = DeleteUserCouponByOrderId(userId, orderId, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -DeleteUserCouponByOrderId ユーザークーポン削除(注文キャンセル時)
		/// <summary>
		/// ユーザークーポン情報削除(注文キャンセル時)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		private int DeleteUserCouponByOrderId(string userId, string orderId, SqlAccessor accessor)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.DeleteUserCouponByOrderId(userId, orderId);
				return result;
			}
		}
		#endregion

		#region +DeleteExpiredUserCoupon 期限切れユーザクーポン削除
		/// <summary>
		/// 期限切れユーザクーポン削除
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>削除件数</returns>
		public int DeleteExpiredUserCoupon(string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = new CouponFacade().DeleteExpiredUserCoupon(lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion

		#region +GetAllPublishCoupons 発行可能クーポン情報取得
		/// <summary>
		/// 発行可能クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行可能クーポン情報一覧</returns>
		public CouponModel[] GetAllPublishCoupons(string deptId, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var coupons = repository.GetAllPublishCoupons(deptId);
				return coupons;
			}
		}
		#endregion

		#region +GetAllPublishCouponsNotPublishDate 発行可能クーポン情報取得(発行期間を考慮しない)
		/// <summary>
		/// 発行可能クーポン情報取得(発行期間を考慮しない)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行可能クーポン情報一覧</returns>
		public CouponModel[] GetAllPublishCouponsNotPublishDate(string deptId, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var coupons = repository.GetAllPublishCouponsNotPublishDate(deptId);
				return coupons;
			}
		}
		#endregion

		#region +GetPublishCouponsByCouponType クーポンタイプを元に発行可能クーポン情報取得
		/// <summary>
		/// クーポンタイプを元に発行可能クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponType">クーポン種別</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行可能クーポン情報一覧</returns>
		public CouponModel[] GetPublishCouponsByCouponType(string deptId, string couponType, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var coupons = repository.GetPublishCouponsByCouponType(deptId, couponType);
				return coupons;
			}
		}
		#endregion

		#region +GetPublishCouponsById クーポンIDを元に発行可能クーポン情報取得
		/// <summary>
		/// クーポンIDを元に発行可能クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行可能クーポン情報</returns>
		public CouponModel GetPublishCouponsById(string deptId, string couponId, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var couponInfo = repository.GetPublishCouponsById(deptId, couponId);
				if (couponInfo != null && couponInfo.Length > 0)
				{
					return couponInfo[0];
				}
				return null;
			}
		}
		#endregion

		#region +SearchUserList ユーザー情報検索
		/// <summary>
		/// ユーザー情報検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果 (0件の場合は0配列)</returns>
		public UserListSearchResult[] SearchUserList(BaseCouponSearchCondition condition)
		{
			using (var repository = new CouponRepository())
			{
				var result = repository.SearchUserList(condition);
				return result;
			}
		}
		#endregion

		#region +GetUserCouponHistoryByUserIdAndCouponId ユーザーIDとクーポンIDからユーザークーポン履歴情報取得
		/// <summary>
		/// ユーザーIDとクーポンIDからユーザークーポン履歴情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン履歴</returns>
		public UserCouponHistoryModel[] GetUserCouponHistoryByUserIdAndCouponId(string userId, string couponId, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var userCouponHistory = repository.GetUserCouponHistoryByUserIdAndCouponId(userId, couponId);
				return userCouponHistory;
			}
		}
		#endregion

		#region +GetCountOfUserListSearch ユーザー情報検索結果件数取得
		/// <summary>
		/// ユーザー情報検索結果件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果件数</returns>
		public int GetCountOfUserListSearch(BaseCouponSearchCondition condition)
		{
			using (var repository = new CouponRepository())
			{
				var count = repository.GetCountOfUserListSearch(condition);
				return count;
			}
		}
		#endregion

		#region +SearchUserCouponHistory ユーザークーポン履歴情報検索
		/// <summary>
		/// ユーザークーポン履歴情報検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果 (0件の場合は0配列)</returns>
		public UserCouponHistoryListSearchResult[] SearchUserCouponHistory(BaseCouponSearchCondition condition)
		{
			using (var repository = new CouponRepository())
			{
				var result = repository.SearchUserCouponHistory(condition);
				return result;
			}
		}
		#endregion

		#region +GetHistoiresAll ユーザークーポン履歴取得（全て）
		/// <summary>
		/// ユーザークーポン履歴取得（全て）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザークーポン履歴列</returns>
		public UserCouponHistoryModel[] GetHistoiresAll(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var results = repository.GetHistoiresAll(userId);
				return results;
			}
		}
		#endregion

		#region +GetHistoiresByOrderId ユーザークーポン履歴取得（注文ID指定）
		/// <summary>
		/// ユーザークーポン履歴取得（全て）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>ユーザークーポン履歴列</returns>
		public UserCouponHistoryModel[] GetHistoiresByOrderId(string userId, string orderId)
		{
			return GetHistoiresAll(userId).Where(h => h.OrderId == orderId).ToArray();
		}
		#endregion

		#region +InsertUserCouponHistory ユーザークーポン履歴情報を登録
		/// <summary>
		/// ユーザークーポン履歴情報を登録
		/// </summary>
		/// <param name="couponHistory">ユーザークーポン履歴情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録件数</returns>
		public int InsertUserCouponHistory(UserCouponHistoryModel couponHistory, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.InsertUserCouponHistory(couponHistory);
				return result;
			}
		}
		/// <summary>
		/// ユーザークーポン履歴情報を登録
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <param name="historyKbn">クーポン履歴区分</param>
		/// <param name="actionKbn">操作区分</param>
		/// <param name="couponInc">加算数</param>
		/// <param name="couponPrice">クーポン金額</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>処理結果(true: 登録件数 > 0, false: 登録件数 = 0)</returns>
		public bool InsertUserCouponHistory(
			string userId,
			string orderId,
			string deptId,
			string couponId,
			string couponCode,
			string historyKbn,
			string actionKbn,
			int couponInc,
			decimal couponPrice,
			string lastChanged,
			SqlAccessor sqlAccessor,
			string fixedPurchaseId = "")
		{
			var couponHistory = new UserCouponHistoryModel()
			{
				UserId = userId,
				DeptId = deptId,
				CouponId = couponId,
				CouponCode = couponCode,
				OrderId = orderId,
				HistoryKbn = historyKbn,
				ActionKbn = actionKbn,
				CouponInc = couponInc,
				CouponPrice = couponPrice,
				LastChanged = lastChanged,
				FixedPurchaseId = fixedPurchaseId,
			};

			// ユーザクーポン情報履歴を登録
			var result = this.InsertUserCouponHistory(couponHistory, sqlAccessor);
			return (result > 0);
		}
		#endregion

		#region +InsertUserCouponHistoryForCancel ユーザークーポン履歴登録(キャンセル向け)
		/// <summary>
		/// ユーザークーポン履歴登録(キャンセル向け)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertUserCouponHistoryForCancel(string deptId, string userId, string orderId, string lastChanged, SqlAccessor accessor = null)
		{
			var published = GetOrderPublishUserCoupon(deptId, userId, orderId, accessor);
			if (published == null) return;
			
			published.ToList().ForEach(coupon => InsertUserCouponHistory(
				userId,
				orderId,
				coupon.DeptId,
				coupon.CouponId,
				coupon.CouponCode,
				Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH_CANCEL,
				Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
				-1,
				(coupon.DiscountPrice.GetValueOrDefault(0) * -1),
				lastChanged,
				accessor));
					
		}
		#endregion

		#region +DeleteUserCouponHistory ユーザークーポン履歴情報を削除
		/// <summary>
		/// ユーザークーポン履歴情報を削除
		/// </summary>
		/// <param name="couponHistory">ユーザークーポン履歴情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		public int DeleteUserCouponHistory(UserCouponHistoryModel couponHistory, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.DeleteUserCouponHistory(couponHistory);
				return result;
			}
		}
		#endregion

		#region +SearchCouponList クーポン情報検索
		/// <summary>
		/// クーポン情報検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		public CouponListSearchResult[] SearchCouponList(CouponListSearchCondition condition)
		{
			using (var repository = new CouponRepository())
			{
				var result = repository.SearchCouponList(condition);
				return result;
			}
		}
		#endregion

		#region +SearchCouponTransitionReport クーポン情報検索
		/// <summary>
		/// クーポン推移レポート検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果</returns>
		public CouponTransitionReportResult[] SearchCouponTransitionReport(CouponTransitionReportCondition condition)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var searchResult = new CouponFacade().GetCouponTransitionInfo(condition, accessor);

				accessor.CommitTransaction();
				return searchResult;
			}
		}
		#endregion

		#region +CreateUnusedCouponCountReport 未利用クーポン枚数推移情報登録
		/// <summary>
		/// 未利用クーポン枚数推移情報登録
		/// </summary>
		/// <param name="model">サマリ分析結果テーブルモデル</param>
		/// <returns> 登録件数</returns>
		public int CreateUnusedCouponCountReport(DispSummaryAnalysisModel model)
		{
			using (var repository = new CouponRepository { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var result = repository.InsertUnusedCouponCountTransition(model);
				return result;
			}
		}
		#endregion

		#region +CreateUnusedCouponPriceReport 未利用クーポン金額推移情報登録
		/// <summary>
		/// 未利用クーポン金額推移情報登録
		/// </summary>
		/// <param name="model">サマリ分析結果テーブルモデル</param>
		/// <returns> 登録件数</returns>
		public int CreateUnusedCouponPriceReport(DispSummaryAnalysisModel model)
		{
			using (var repository = new CouponRepository { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var result = repository.InsertUnusedCouponPriceTransition(model);
				return result;
			}
		}
		#endregion

		#region +GetCoupon クーポン情報詳細取得
		/// <summary>
		/// クーポン情報詳細取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns> クーポン情報</returns>
		public CouponModel GetCoupon(string deptId, string couponId)
		{
			using (var repository = new CouponRepository())
			{
				var coupon = repository.GetCoupon(deptId, couponId);
				return coupon;
			}
		}
		#endregion

		#region +GetCouponsFromCouponCode クーポンコードからクーポン情報詳細取得（前方一致・先頭一件）
		/// <summary>
		/// クーポンコードからクーポン情報詳細取得（前方一致・先頭一件）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns> クーポン情報</returns>
		public CouponModel[] GetCouponsFromCouponCode(string deptId, string couponCode)
		{
			using (var repository = new CouponRepository())
			{
				var coupons = repository.GetCouponsFromCouponCode(deptId, couponCode);
				return coupons;
			}
		}
		#endregion

		#region +GetCouponFromCouponCodePerfectMatch クーポンコード完全一致
		/// <summary>
		/// クーポン情報取得(クーポンコード完全一致指定)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns> クーポン情報</returns>
		public CouponModel GetCouponFromCouponCodePerfectMatch(string deptId, string couponCode)
		{
			using (var repository = new CouponRepository())
			{
				var coupon = repository.GetCouponFromCouponCodePerfectMatch(deptId, couponCode);
				return coupon;
			}
		}
		#endregion

		#region +GetCouponCount クーポン数検索（前方一致）
		/// <summary>
		/// クーポン数検索（前方一致）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>クーポン数</returns>
		public int GetCouponCount(CouponListSearchCondition condition)
		{
			using (var repository = new CouponRepository())
			{
				var count = repository.GetCouponCount(condition);
				return count;
			}
		}
		#endregion

		#region +GetAllCouponCount 全クーポン数取得
		/// <summary>
		/// 全クーポン数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>クーポン数</returns>
		public int GetAllCouponCount(string deptId)
		{
			using (var repository = new CouponRepository())
			{
				var count = repository.GetAllCouponCount(deptId);
				return count;
			}
		}
		#endregion

		#region +GetAllCoupons 全クーポン情報取得
		/// <summary>
		/// 全クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>クーポン情報</returns>
		public CouponModel[] GetAllCoupons(string deptId)
		{
			using (var repository = new CouponRepository())
			{
				var coupons = repository.GetAllCoupons(deptId);
				return coupons;
			}
		}
		#endregion

		#region +InsertCoupon クーポン情報登録
		/// <summary>
		/// クーポン情報登録
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <returns> 登録件数 </returns>
		public int InsertCoupon(CouponModel model)
		{
			using (var repository = new CouponRepository())
			{
				var result = repository.InsertCoupon(model);
				return result;
			}
		}
		#endregion

		#region +UpdateCoupon クーポン情報更新
		/// <summary>
		/// クーポン情報更新
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <returns> 更新件数 </returns>
		public int UpdateCoupon(CouponModel model)
		{
			using (var repository = new CouponRepository())
			{
				var result = repository.UpdateCoupon(model);
				return result;
			}
		}
		#endregion

		#region +UpdateCouponCountDown クーポン情報、残り利用回数をマイナスする
		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		public int UpdateCouponCountDown(CouponModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateCouponCountDown(model);
				return result;
			}
		}
		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponCode">クーポンンコード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果(true: 更新件数 > 0, false: 更新件数 = 0)</returns>
		public bool UpdateCouponCountDown(string deptId, string couponId, string couponCode, string lastChanged, SqlAccessor sqlAccessor)
		{
			var coupon = new CouponModel()
			{
				DeptId = deptId,
				CouponId = couponId,
				CouponCode = couponCode,
				LastChanged = lastChanged
			};

			// クーポン情報を更新
			var result = this.UpdateCouponCountDown(coupon, sqlAccessor);
			return (result > 0);
		}
		#endregion

		#region +UpdateCouponCountDownIgnoreCouponCount クーポン情報、残り利用回数をマイナスする(現在のクーポン数を考慮しない)
		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする(現在のクーポン数を考慮しない)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponCode">クーポンンコード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果(true: 更新件数 > 0, false: 更新件数 = 0)</returns>
		public bool UpdateCouponCountDownIgnoreCouponCount(
			string deptId,
			string couponId,
			string couponCode,
			string lastChanged,
			SqlAccessor sqlAccessor)
		{
			var coupon = new CouponModel()
			{
				DeptId = deptId,
				CouponId = couponId,
				CouponCode = couponCode,
				LastChanged = lastChanged
			};

			// クーポン情報を更新
			var result = this.UpdateCouponCountDownIgnoreCouponCount(coupon, sqlAccessor);
			return (result > 0);
		}
		#endregion
		#region +UpdateCouponCountDown クーポン情報、残り利用回数をマイナスする(現在のクーポン数を考慮しない)
		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		public int UpdateCouponCountDownIgnoreCouponCount(
			CouponModel model,
			SqlAccessor accessor)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateCouponCountDownIgnoreCouponCount(model);
				return result;
			}
		}
		#endregion

		#region +UpdateCouponCountUp クーポン情報、残り利用回数をプラスする
		/// <summary>
		/// クーポン情報、残り利用回数をプラスする
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponCode">クーポンンコード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果(true: 更新件数 > 0, false: 更新件数 = 0)</returns>
		public bool UpdateCouponCountUp(string deptId, string couponId, string couponCode, string lastChanged, SqlAccessor sqlAccessor)
		{
			var coupon = new CouponModel()
			{
				DeptId = deptId,
				CouponId = couponId,
				CouponCode = couponCode,
				LastChanged = lastChanged
			};

			// クーポン情報を更新
			var result = this.UpdateCouponCountUp(coupon, sqlAccessor);
			return (result > 0);
		}
		/// <summary>
		/// クーポン情報、残り利用回数をプラスする
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		private int UpdateCouponCountUp(CouponModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateCouponCountUp(model);
				return result;
			}
		}
		#endregion

		#region +DeleteCoupon クーポン情報削除
		/// <summary>
		/// クーポン情報削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns> 削除件数 </returns>
		public int DeleteCoupon(string deptId, string couponId)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new CouponRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var result = repository.DeleteCoupon(deptId, couponId);
				if (result > 0)
				{
					repository.DeleteCouponUseUserByCouponId(couponId);
				}
				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion

		#region +GetAllUserCouponsSpecificExpireDate 有効期限指定ユーザクーポン情報取得
		/// <summary>
		/// 有効期限開始日が指定日時以降・有効期限終了日が指定日時以降のクーポン情報取得
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="expireDateTimeBegin">有効期限開始日時</param>
		/// <param name="expireDateTimeEnd">有効期限終了日時</param>
		/// <returns>クーポン情報</returns>
		public UserCouponDetailInfo[] GetAllUserCouponsSpecificExpireDate(string strUserId, string strDeptId, DateTime expireDateTimeBegin, DateTime expireDateTimeEnd)
		{
			using (var repository = new CouponRepository())
			{
				var result = repository.GetAllUserCouponsSpecificExpireDate(strUserId, strDeptId, expireDateTimeBegin, expireDateTimeEnd);
				return result;
			}
		}
		#endregion

		#region +GetCountCouponUseUser ブラックリスト型クーポンを利用済みか
		/// <summary>
		/// ブラックリスト型クーポンを利用済みか
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <returns>利用済みか</returns>
		public bool CheckUsedCoupon(string couponId, string couponUseUser)
		{
			using (var repository = new CouponRepository())
			{
				var usedCount = repository.GetCountCouponUseUser(couponId, couponUseUser);
				return (usedCount > 0);
			}
		}
		#endregion

		#region +GetCouponUseUser クーポン利用ユーザー取得
		/// <summary>
		/// クーポン利用ユーザー取得
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>クーポン利用ユーザー情報</returns>
		public CouponUseUserModel GetCouponUseUser(string couponId, string couponUseUser, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new CouponRepository(sqlAccessor))
			{
				var result = repository.GetCouponUseUser(couponId, couponUseUser);
				return result;
			}
		}
		#endregion

		#region +GetCouponUseUserByCouponUseUser ユーザー情報に紐づくクーポン利用ユーザー情報取得
		/// <summary>
		/// クーポン利用ユーザー取得
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>クーポン利用ユーザー情報</returns>
		public CouponUseUserModel[] GetCouponUseUserByCouponUseUser(string couponUseUser, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new CouponRepository(sqlAccessor))
			{
				var result = repository.GetCouponUseUserByCouponUseUser(couponUseUser);
				return result;
			}
		}
		#endregion

		#region +GetNextHistoryNo ユーザークーポン履歴Noを取得
		/// <summary>
		/// ユーザークーポン履歴Noを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ユーザー履歴No</returns>
		public int GetNextHistoryNo(string userId, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new CouponRepository(sqlAccessor))
			{
				var result = repository.GetNextHistoryNo(userId);
				return result;
			}
		}
		#endregion

		#region +GetCouponUseUserSearchHitCount クーポン利用ユーザー検索ヒット件数取得
		/// <summary>
		/// クーポン利用ユーザー検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>ヒット件数</returns>
		public int GetCouponUseUserSearchHitCount(CouponUseUserListSearchCondition condition)
		{
			using (var repository = new CouponRepository())
			{
				var count = repository.GetCouponUseUserSearchHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +SearchCouponUseUser クーポン利用ユーザー検索
		/// <summary>
		/// クーポン利用ユーザー検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果</returns>
		public CouponUseUserListSearchResult[] SearchCouponUseUser(CouponUseUserListSearchCondition condition)
		{
			using (var repository = new CouponRepository())
			{
				var searchResult = repository.SearchCouponUseUser(condition);
				return searchResult;
			}
		}
		#endregion

		#region +InsertCouponUseUser クーポン利用ユーザー登録
		/// <summary>
		/// クーポン利用ユーザー登録
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		public int InsertCouponUseUser(CouponUseUserModel couponUseUser, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new CouponRepository(sqlAccessor))
			{
				var result = repository.InsertCouponUseUser(couponUseUser);
				return result;
			}
		}
		#endregion

		#region +UpdateCouponUseUser クーポン利用ユーザー更新
		/// <summary>
		/// クーポン利用ユーザー更新
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		public int UpdateCouponUseUser(CouponUseUserModel couponUseUser, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new CouponRepository(sqlAccessor))
			{
				var result = repository.UpdateCouponUseUser(couponUseUser);
				return result;
			}
		}
		#endregion

		#region +DeleteCouponUseUser クーポン利用ユーザー削除
		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <returns>処理件数</returns>
		public int DeleteCouponUseUser(
			string couponId,
			string couponUseUser)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = DeleteCouponUseUser(couponId, couponUseUser, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region -DeleteCouponUseUser クーポン利用ユーザー削除
		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		public int DeleteCouponUseUser(
			string couponId,
			string couponUseUser,
			SqlAccessor accessor)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.DeleteCouponUseUser(couponId, couponUseUser);
				return result;
			}
		}
		#endregion

		#region ~UpdateCouponUseUserOrderId クーポン利用ユーザー 注文ID更新
		/// <summary>
		/// クーポン利用ユーザー 注文ID更新
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateCouponUseUserOrderId(CouponUseUserModel couponUseUser, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateCouponUseUserOrderId(couponUseUser);
				return result;
			}
		}
		#endregion

		#region +DeleteCouponUseUser クーポン利用ユーザー削除(注文ID指定)
		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		public int DeleteCouponUseUserByOrderId(string couponId, string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.DeleteCouponUseUserByOrderId(couponId, orderId);
				return result;
			}
		}
		#endregion

		#region ~DeleteCouponUseUserByFixedPurchaseId クーポン利用ユーザー削除(定期購入ID指定)
		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		public int DeleteCouponUseUserByFixedPurchaseId(
			string couponId,
			string fixedPurchaseId,
			SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.DeleteCouponUseUserByFixedPurchaseId(couponId, fixedPurchaseId);
				return result;
			}
		}
		#endregion

		#region +CouponScheduleListSearchResult クーポン発行スケジュールリスト検索
		/// <summary>
		/// クーポン発行スケジュールリスト検索
		/// </summary>
		/// <param name="cond">クーポン発行スケジュールリスト検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		public CouponScheduleListSearchResult[] CouponScheduleListSearch(CouponScheduleListSearchCondition cond)
		{
			var search = new CouponScheduleListSearch();
			return search.Search(cond);
		}
		#endregion

		#region +GetCouponSchedule クーポン発行スケジュール取得
		/// <summary>
		/// クーポン発行スケジュール取得
		/// </summary>
		/// <param name="couponScheduleId">クーポンス発行ケジュールID</param>
		/// <returns>モデル</returns>
		public CouponScheduleModel GetCouponSchedule(string couponScheduleId)
		{
			using (var repository = new CouponRepository())
			{
				var model = repository.GetCouponSchedule(couponScheduleId);
				return model;
			}
		}
		#endregion

		#region +GetCouponScheduleCountFromCouponId 識別ID・クーポンIDを元にクーポン発行スケジュール件数取得
		/// <summary>
		/// 識別ID・クーポンIDを元にクーポン発行スケジュール件数取得
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <returns>クーポン発行スケジュール件数</returns>
		public int GetCouponScheduleCountFromCouponId(string couponId)
		{
			using (var repository = new CouponRepository())
			{
				var count = repository.GetCouponScheduleCountFromCouponId(couponId);
				return count;
			}
		}
		#endregion

		#region +InsertCouponSchedule クーポン発行スケジュール登録
		/// <summary>
		/// クーポン発行スケジュール登録
		/// </summary>
		/// <param name="model">登録するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>登録した件数</returns>
		public int InsertCouponSchedule(CouponScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.InsertCouponSchedule(model);

				return result;
			}
		}
		#endregion

		#region +UpdateCouponSchedule クーポン発行スケジュール更新
		/// <summary>
		/// クーポン発行スケジュール更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		public int UpdateCouponSchedule(CouponScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateCouponSchedule(model);

				return result;
			}
		}
		#endregion

		#region +DeleteCouponSchedule クーポン発行スケジュール削除
		/// <summary>
		/// クーポン発行スケジュール削除
		/// </summary>
		/// <param name="couponScheduleId">ポイントルールスケジュールID</param>
		/// <returns>削除した件数</returns>
		public int DeleteCouponSchedule(string couponScheduleId)
		{
			using (var repository = new CouponRepository())
			{
				var result = repository.DeleteCouponSchedule(couponScheduleId);

				return result;
			}
		}
		#endregion

		#region +UpdateCouponScheduleStatus クーポン発行スケジュールステータス更新
		/// <summary>
		/// クーポン発行スケジュールステータス更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		public int UpdateCouponScheduleStatus(CouponScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateCouponScheduleStatus(model);

				return result;
			}
		}
		#endregion

		#region +UpdateCouponScheduleLastCount クーポン発行スケジュール最終付与人数更新
		/// <summary>
		/// クーポン発行スケジュールステータス更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		public int UpdateCouponScheduleLastCount(CouponScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateCouponScheduleLastCount(model);

				return result;
			}
		}
		#endregion

		#region +CheckTargetListUsed ターゲットリストで使われているか
		/// <summary>
		/// ターゲットリストで使われているか
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>クーポン発行スケジュールモデル</returns>
		public CouponScheduleModel[] CheckTargetListUsed(string targetId)
		{
			using (var repository = new CouponRepository())
			{
				var result = repository.CheckTargetListUsed(targetId);

				return result;
			}
		}
		#endregion

		#region +ApplyNextShippingUseCouponToFixedPurchase 定期購入に次回購入利用クーポンの適用
		/// <summary>
		/// 定期購入に次回購入利用クーポンの適用
		/// </summary>
		/// <param name="coupon">クーポン情報</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果（成功：TRUE　失敗：FALSE）</returns>
		public bool ApplyNextShippingUseCouponToFixedPurchase(
			UserCouponDetailInfo coupon,
			string userId,
			string fixedPurchaseId,
			string couponUseUser,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			if (coupon == null) return true;
			var success = true;
			switch (coupon.CouponType)
			{
				// 発行者向け回数制限ありクーポン
				case Constants.FLG_COUPONCOUPON_TYPE_USERREGIST:
				case Constants.FLG_COUPONCOUPON_TYPE_BUY:
				case Constants.FLG_COUPONCOUPON_TYPE_FIRSTBUY:
					success = UpdateUserCouponUseFlg(
						userId,
						coupon.DeptId,
						coupon.CouponId,
						coupon.CouponNo ?? 1,
						true,
						DateTime.Now,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor);
					break;

				// 全員向け回数制限ありクーポン
				case Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING:
					success = UpdateCouponCountDown(
						coupon.DeptId,
						coupon.CouponId,
						coupon.CouponCode,
						lastChanged,
						accessor);
					break;

				// 会員限定回数制限付きクーポン
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER:
					var userCoupon = new UserCouponModel
					{
						UserId = userId,
						DeptId = coupon.DeptId,
						CouponId = coupon.CouponId,
						CouponNo = coupon.CouponNo ?? 1,
						UserCouponCount = coupon.UserCouponCount - 1, // １枚減らす
						LastChanged = lastChanged
					};
					success = UpdateUseCouponCountUserCoupon(userCoupon, accessor);
					break;

				// ブラックリスト型クーポン
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_ALL:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL:
					var couponUseUserModel = new CouponUseUserModel
					{
						CouponId = coupon.CouponId,
						OrderId = "",
						FixedPurchaseId = fixedPurchaseId,
						CouponUseUser = couponUseUser,
						LastChanged = lastChanged
					};
					success = (InsertCouponUseUser(couponUseUserModel, accessor) > 0);
					break;
			}

			if (success)
			{
				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				// ユーザクーポン履歴INSERT処理(クーポン適用)
				success = InsertUserCouponHistory(
					userId,
					"",
					coupon.DeptId,
					coupon.CouponId,
					coupon.CouponCode,
					Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE,
					Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_BASE,
					-1,
					0,
					lastChanged,
					accessor,
					fixedPurchaseId);
			}

			return success;
		}
		#endregion

		#region +UpdateUserCouponForIntegration ユーザークーポン更新(ユーザー統合用)
		/// <summary>
		/// ユーザークーポン履歴更新(ユーザー統合用)
		/// </summary>
		/// <param name="userId">ユーザーID(新)</param>
		/// <param name="userIdOld">ユーザーID(旧)</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">クーポンNo(新)</param>
		/// <param name="couponNoOld">クーポンNo(旧)</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateUserCouponForIntegration(
			string userId,
			string userIdOld,
			string couponId,
			string couponNo,
			string couponNoOld,
			SqlAccessor accessor)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateUserCouponForIntegration(
					userId,
					userIdOld,
					couponId,
					couponNo,
					couponNoOld);
				return result;
			}
		}
		#endregion

		#region +UpdateUserCouponHistoryForIntegration ユーザークーポン履歴更新(ユーザー統合用)
		/// <summary>
		/// ユーザークーポン履歴更新(ユーザー統合用)
		/// </summary>
		/// <param name="userId">ユーザーID(新)</param>
		/// <param name="userIdOld">ユーザーID(旧)</param>
		/// <param name="historyNo">履歴No(新)</param>
		/// <param name="historyNoOld">履歴No(旧)</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateUserCouponHistoryForIntegration(
			string userId,
			string userIdOld,
			string historyNo,
			string historyNoOld,
			SqlAccessor accessor)
		{
			using (var repository = new CouponRepository(accessor))
			{
				var result = repository.UpdateUserCouponHistoryForIntegration(
					userId,
					userIdOld,
					historyNo,
					historyNoOld);
				return result;
			}
		}
		#endregion

		#region +ReturnNextShippingUseCoupon 次回購入利用クーポンを戻す
		/// <summary>
		/// 次回購入利用クーポンを戻す
		/// </summary>
		/// <param name="coupon">クーポン情報</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="historyKbn">クーポン履歴区分</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果（成功：TRUE　失敗：FALSE）</returns>
		public bool ReturnNextShippingUseCoupon(
			UserCouponDetailInfo coupon,
			string userId,
			string orderId,
			string fixedPurchaseId,
			string lastChanged,
			string historyKbn,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			if (coupon == null) return true;
			var success = true;
			switch (coupon.CouponType)
			{
				// 発行者向け回数制限ありクーポン
				case Constants.FLG_COUPONCOUPON_TYPE_USERREGIST:
				case Constants.FLG_COUPONCOUPON_TYPE_BUY:
				case Constants.FLG_COUPONCOUPON_TYPE_FIRSTBUY:
					success = UpdateUserCouponUseFlg(
						userId,
						coupon.DeptId,
						coupon.CouponId,
						coupon.CouponNo ?? 1,
						false,
						DateTime.Now,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor);
					break;

				// 全員向け回数制限ありクーポン
				case Constants.FLG_COUPONCOUPON_TYPE_ALL_LIMIT:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING:
					success = UpdateCouponCountUp(
						coupon.DeptId,
						coupon.CouponId,
						coupon.CouponCode,
						lastChanged,
						accessor);
					break;

				// 会員限定回数制限付きクーポン
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER:
					var userCoupon = new UserCouponModel
					{
						UserId = userId,
						DeptId = coupon.DeptId,
						CouponId = coupon.CouponId,
						CouponNo = coupon.CouponNo.Value,
						UserCouponCount = coupon.UserCouponCount + 1, // １枚増やす
						LastChanged = lastChanged
					};
					success = UpdateUseCouponCountUserCoupon(userCoupon, accessor);
					break;

				// ブラックリスト型クーポン
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_ALL:
				case Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL:
					success = (DeleteCouponUseUserByFixedPurchaseId(coupon.CouponId, fixedPurchaseId, accessor) > 0);
					break;
			}

			if (success)
			{
				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				// ユーザクーポン履歴INSERT処理(クーポン戻す)
				success = InsertUserCouponHistory(
					userId,
					orderId,
					coupon.DeptId,
					coupon.CouponId,
					coupon.CouponCode,
					historyKbn,
					Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_BASE,
					1,
					0,
					lastChanged,
					accessor,
					fixedPurchaseId);
			}

			return success;
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="input">検索条件</param>
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMaster(string sqlFieldNames, string masterKbn, Hashtable input)
		{
			try
			{
				using (var repository = new CouponRepository())
				{
					repository.CheckFieldsForGetMaster(
						input,
						masterKbn,
						new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn(ex);
				return false;
			}
			return true;
		}

		/// <summary>
		///  CSVへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か</returns>
		public bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var accessor = new SqlAccessor())
			using (var repository = new CouponRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				statementName,
				new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames)))
			{
				new MasterExportCsv().Exec(setting, reader, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
			}
			return true;
		}

		/// <summary>
		/// Excelへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="excelTemplateSetting">Excelテンプレート設定</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か（件数エラーの場合は失敗）</returns>
		public bool ExportToExcel(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			ExcelTemplateSetting excelTemplateSetting,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var repository = new CouponRepository())
			{
				var dv = repository.GetMaster(input, statementName, new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				if (dv.Count >= 20000) return false;

				new MasterExportExcel().Exec(setting, excelTemplateSetting, dv, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				return true;
			}
		}

		/// <summary>
		/// マスタ区分で該当するStatementNameを取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>StatementName</returns>
		private string GetStatementNameInfo(string masterKbn)
		{
			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON: // クーポン
					return "GetCouponMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON: // ユーザクーポン
					return "GetUserCouponMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER: // クーポン利用ユーザー情報
					return "GetCouponUseUserForMaster";
			}
			throw new Exception("未対応のマスタ区分：" + masterKbn);
		}
		#endregion

		#region +SearchCouponsForAutosuggest
		/// <summary>
		/// Search coupons for autosuggest
		/// </summary>
		/// <param name="condition">Condition</param>
		/// <returns>Coupons</returns>
		public UserCouponDetailInfo[] SearchCouponsForAutosuggest(CouponListSearchCondition condition)
		{
			try
			{
				using (var repository = new CouponRepository())
				{
					repository.CommandTimeout = Constants.ORDERREGISTINPUT_SUGGEST_QUERY_TIMEOUT;
					var coupons = repository.SearchCouponsForAutosuggest(condition);
					return coupons;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		#endregion
	}
}
