/*
=========================================================================================================
  Module      : クーポンリポジトリクラス (CouponRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.Coupon.Helper;

namespace w2.Domain.Coupon
{
	/// <summary>
	/// クーポンリポジトリクラス
	/// </summary>
	internal class CouponRepository : RepositoryBase
	{
		/// <summary>影響を受けた件数</summary>
		private const string XML_KEY_NAME = "Coupon";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal CouponRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// <param name="accessor">SQLアクセサー</param>
		/// </summary>
		internal CouponRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetUserUsableCoupons ユーザーが利用可能なクーポン取得
		/// <summary>
		/// ユーザーが利用可能なクーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <param name="usedUserJudgeType">クーポン利用済みユーザー判定方法</param>
		/// <returns>ユーザークーポンリスト</returns>
		internal UserCouponDetailInfo[] GetUserUsableCoupons(
			string deptId,
			string userId,
			string mailAddress,
			string usedUserJudgeType)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPON_DEPT_ID, deptId },
				{ Constants.FIELD_USERCOUPON_USER_ID, userId },
				{ Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER, mailAddress },
				{ Constants.FLG_COUPONUSEUSER_USED_USER_JUDGE_TYPE, usedUserJudgeType }
			};
			var dv = base.Get(XML_KEY_NAME, "GetUserUsableCoupons", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();
		}
		#endregion

		#region ~GetUserCoupons ユーザークーポン取得
		/// <summary>
		/// ユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <returns>ユーザークーポンリスト</returns>
		internal UserCouponDetailInfo[] GetUserCoupons(
			string deptId,
			string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPON_DEPT_ID, deptId },
				{ Constants.FIELD_USERCOUPON_USER_ID, userId },
			};
			var dv = base.Get(XML_KEY_NAME, "GetUserCoupons", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();
		}
		#endregion

		#region ~GetAllUserCoupons 有効期限にも関わらず、全てのユーザークーポン情報取得
		/// <summary>
		/// 有効期限にも関わらず、全てのユーザークーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		internal UserCouponDetailInfo[] GetAllUserCoupons(string deptId, string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPON_DEPT_ID, deptId },
				{ Constants.FIELD_USERCOUPON_USER_ID, userId }
			};
			var dv = base.Get(XML_KEY_NAME, "GetAllUserCoupons", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();
		}
		#endregion

		#region ~GetAllUserCouponsFromCouponCode ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得
		/// <summary>
		/// ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		internal UserCouponDetailInfo[] GetAllUserCouponsFromCouponCode(string deptId, string userId, string couponCode)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCOUPON_DEPT_ID, deptId},
				{Constants.FIELD_USERCOUPON_USER_ID, userId},
				{Constants.FIELD_COUPON_COUPON_CODE, couponCode}
			};
			var dv = base.Get(XML_KEY_NAME, "GetAllUserCouponsFromCouponCode", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();
		}
		#endregion

		#region ~GetAllUserCouponsFromCouponCodeIncludeUnavailable ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得(利用不可のものも含む)
		/// <summary>
		/// ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得(利用不可のものも含む)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		internal UserCouponDetailInfo[] GetAllUserCouponsFromCouponCodeIncludeUnavailable(string deptId, string userId, string couponCode)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCOUPON_DEPT_ID, deptId},
				{Constants.FIELD_USERCOUPON_USER_ID, userId},
				{Constants.FIELD_COUPON_COUPON_CODE, couponCode}
			};
			var dv = base.Get(XML_KEY_NAME, "GetAllUserCouponsFromCouponCodeIncludeUnavailable", ht);
			var coupons = dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();

			return coupons;
		}
		#endregion

		#region ~GetUserCouponFromCouponNo ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// <summary>
		/// ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">枝番</param>
		/// <returns>ユーザークーポン情報</returns>
		internal UserCouponDetailInfo GetUserCouponFromCouponNo(string deptId, string userId, string couponId, int couponNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCOUPON_DEPT_ID, deptId},
				{Constants.FIELD_USERCOUPON_USER_ID, userId},
				{Constants.FIELD_USERCOUPON_COUPON_ID, couponId},
				{Constants.FIELD_USERCOUPON_COUPON_NO, couponNo}
			};
			var dv = base.Get(XML_KEY_NAME, "GetUserCouponFromCouponNo", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).FirstOrDefault();
		}
		#endregion

		#region ~GetAllUserCouponsFromCouponId ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// <summary>
		/// ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">枝番</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		internal UserCouponDetailInfo[] GetAllUserCouponsFromCouponId(string deptId, string userId, string couponId, int couponNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCOUPON_DEPT_ID, deptId},
				{Constants.FIELD_USERCOUPON_USER_ID, userId},
				{Constants.FIELD_USERCOUPON_COUPON_ID, couponId},
				{Constants.FIELD_USERCOUPON_COUPON_NO, couponNo}
			};
			var dv = base.Get(XML_KEY_NAME, "GetAllUserCouponsFromCouponId", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();
		}
		#endregion

		#region ~GetUserCouponFromCouponId 識別ID・クーポンIDを元にユーザークーポン取得
		/// <summary>
		/// 識別ID・クーポンIDを元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns>ユーザークーポン情報</returns>
		internal UserCouponDetailInfo GetUserCouponFromCouponId(string deptId, string couponId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPON_DEPT_ID, deptId },
				{ Constants.FIELD_USERCOUPON_COUPON_ID, couponId }
			};
			var dv = base.Get(XML_KEY_NAME, "GetUserCouponFromCouponId", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).FirstOrDefault();
		}
		#endregion

		#region ~GetOrderPublishUserCoupon ユーザクーポン情報取得(本注文で発行したクーポン)
		/// <summary>
		/// ユーザクーポン情報取得(本注文で発行したクーポン)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>ユーザークーポン情報</returns>
		internal UserCouponDetailInfo[] GetOrderPublishUserCoupon(string deptId, string userId, string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPON_DEPT_ID, deptId },
				{ Constants.FIELD_USERCOUPON_USER_ID, userId },
				{ Constants.FIELD_USERCOUPON_ORDER_ID, orderId }
			};
			var dv = base.Get(XML_KEY_NAME, "GetOrderPublishUserCoupon", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();
		}
		#endregion

		#region ~GetUserCouponNo 最大ユーザクーポン枝番取得
		/// <summary>
		/// 最大ユーザクーポン枝番取得
		/// </summary>
		/// <param name="deptId">ユーザID</param>
		/// <param name="userId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns>最大ユーザクーポン枝番</returns>
		internal int GetUserCouponNo(string deptId, string userId, string couponId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPON_DEPT_ID, deptId },
				{ Constants.FIELD_USERCOUPON_USER_ID, userId },
				{ Constants.FIELD_USERCOUPON_COUPON_ID, couponId }
			};
			var dv = base.Get(XML_KEY_NAME, "GetUserCouponNo", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetExpiredUserCoupons 期限切れユーザクーポン取得
		/// <summary>
		/// 期限切れユーザクーポン取得
		/// </summary>
		/// <returns>期限切れユーザクーポン一覧</returns>
		internal UserCouponDetailInfo[] GetExpiredUserCoupons()
		{
			var dv = base.Get(XML_KEY_NAME, "GetExpiredUserCoupons");
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();
		}
		#endregion

		#region ~GetAllUserUsableCouponsCount 利用可能ユーザークーポン数取得
		/// <summary>
		/// 利用可能ユーザークーポン数取得
		/// </summary>
		/// <param name="condition">クーポン情報検索条件情報</param>
		/// <returns>件数</returns>
		internal int GetAllUserUsableCouponsCount(CouponListSearchCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetAllUserUsableCouponsCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetAllUserUsableCoupons 利用可能ユーザークーポン取得
		/// <summary>
		/// 利用可能ユーザークーポン取得
		/// </summary>
		/// <param name="condition">クーポン情報検索条件情報</param>
		/// <returns>ユーザクーポン一覧</returns>
		internal UserCouponDetailInfo[] GetAllUserUsableCoupons(CouponListSearchCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetAllUserUsableCoupons", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();
		}
		#endregion

		#region ~InsertUserCoupon ユーザークーポン情報登録
		/// <summary>
		/// ユーザークーポン情報登録
		/// </summary>
		/// <param name="model">ユーザークーポンモデル</param>
		/// <returns>件数</returns>
		internal int InsertUserCoupon(UserCouponModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertUserCoupon", model.DataSource);
			return result;
		}
		#endregion

		#region ~InsertUserCouponWithOrderId ユーザークーポン情報登録（注文ID指定あり）
		/// <summary>
		/// ユーザークーポン情報登録（注文ID指定あり）
		/// </summary>
		/// <param name="model">ユーザークーポンモデル</param>
		/// <returns>件数</returns>
		internal int InsertUserCouponWithOrderId(UserCouponModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertUserCouponWithOrderId", model.DataSource);
			return result;
		}
		#endregion

		#region ~UpdateUserCouponUseFlg ユーザクーポン情報更新（回数制限有りのクーポンの場合のみ）
		/// <summary>
		/// ユーザクーポン情報更新（回数制限有りのクーポンの場合のみ）
		/// </summary>
		/// <param name="model">ユーザークーポンモデル</param>
		/// <returns>件数</returns>
		internal int UpdateUserCouponUseFlg(UserCouponModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateUserCouponUseFlg", model.DataSource);
			return result;
		}
		#endregion

		#region ~UpdateUnUseUserCoupon ユーザクーポン更新(使用済み→未使用)
		/// <summary>
		/// ユーザクーポン更新(使用済み→未使用)
		/// </summary>
		/// <param name="model">ユーザークーポンモデル</param>
		/// <returns>件数</returns>
		internal int UpdateUnUseUserCoupon(UserCouponModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateUnUseUserCoupon", model.DataSource);
			return result;
		}
		#endregion

		#region UpdateUseCouponCountUserCoupon ユーザークーポン更新(ユーザークーポン利用可能回数)
		/// <summary>
		/// ユーザークーポン更新(ユーザークーポン利用可能回数)
		/// </summary>
		/// <param name="model">ユーザークーポンモデル</param>
		/// <returns>件数</returns>
		internal int UpdateUseCouponCountUserCoupon(UserCouponModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateUseCouponCountUserCoupon", model.DataSource);
			return result;
		}
		#endregion

		#region ~DeleteUserCoupon ユーザークーポン削除
		/// <summary>
		/// ユーザークーポン情報削除
		/// </summary>
		/// <param name="deleteCouponInfo">ユーザークーポン情報</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteUserCoupon(UserCouponDetailInfo deleteCouponInfo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCOUPON_DEPT_ID, deleteCouponInfo.DeptId},
				{Constants.FIELD_USERCOUPON_USER_ID, deleteCouponInfo.UserId},
				{Constants.FIELD_USERCOUPON_COUPON_ID, deleteCouponInfo.CouponId},
				{Constants.FIELD_USERCOUPON_COUPON_NO, deleteCouponInfo.CouponNo}
			};
			var result = Exec(XML_KEY_NAME, "DeleteUserCoupon", ht);
			return result;
		}
		#endregion

		#region ~DeleteUserCouponByOrderId ユーザークーポン削除(注文キャンセル時)
		/// <summary>
		/// ユーザークーポン情報削除(注文キャンセル時)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteUserCouponByOrderId(string userId, string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCOUPON_USER_ID, userId},
				{Constants.FIELD_USERCOUPON_ORDER_ID, orderId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteUserCouponByOrderId", ht);
			return result;
		}
		#endregion

		#region ~DeleteExpiredUserCoupon 期限切れユーザクーポン削除
		/// <summary>
		/// 期限切れユーザクーポン削除
		/// </summary>
		/// <param name="model">ユーザクーポン履歴テーブルモデル</param>
		/// <returns>削除件数</returns>
		internal int DeleteExpiredUserCoupon(UserCouponHistoryModel model)
		{
			return base.Exec(XML_KEY_NAME, "DeleteExpiredUserCoupon", model.DataSource);
		}
		#endregion

		#region ~GetAllPublishCoupons 発行可能クーポン情報取得
		/// <summary>
		/// 発行可能クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>発行可能クーポン情報一覧</returns>
		internal CouponModel[] GetAllPublishCoupons(string deptId)
		{
			var ht = new Hashtable { { Constants.FIELD_COUPON_DEPT_ID, deptId } };
			var dv = base.Get(XML_KEY_NAME, "GetAllPublishCoupons", ht);
			return dv.Cast<DataRowView>().Select(drv => new CouponModel(drv)).ToArray();
		}
		#endregion

		#region ~GetAllPublishCouponsNotPublishDate 発行可能クーポン情報取得(発行期間を考慮しない)
		/// <summary>
		/// 発行可能クーポン情報取得(発行期間を考慮しない)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>発行可能クーポン情報一覧</returns>
		internal CouponModel[] GetAllPublishCouponsNotPublishDate(string deptId)
		{
			var ht = new Hashtable { { Constants.FIELD_COUPON_DEPT_ID, deptId } };
			var dv = base.Get(XML_KEY_NAME, "GetAllPublishCouponsNotPublishDate", ht);
			return dv.Cast<DataRowView>().Select(drv => new CouponModel(drv)).ToArray();
		}
		#endregion

		#region ~GetPublishCouponsByCouponType クーポンタイプを元に発行可能クーポン情報取得
		/// <summary>
		/// クーポンタイプを元に発行可能クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponType">クーポン種別</param>
		/// <returns>発行可能クーポン情報一覧</returns>
		internal CouponModel[] GetPublishCouponsByCouponType(string deptId, string couponType)
		{
			var ht = new Hashtable
			{ 
				{ Constants.FIELD_COUPON_DEPT_ID, deptId },
				{ Constants.FIELD_COUPON_COUPON_TYPE, couponType }
			};
			var dv = base.Get(XML_KEY_NAME, "GetPublishCouponsByCouponType", ht);
			return dv.Cast<DataRowView>().Select(drv => new CouponModel(drv)).ToArray();
		}
		#endregion

		#region ~GetPublishCouponsById クーポンIDを元に発行可能クーポン情報取得
		/// <summary>
		/// クーポンIDを元に発行可能クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns>発行可能クーポン情報</returns>
		internal CouponModel[] GetPublishCouponsById(string deptId, string couponId)
		{
			var ht = new Hashtable
			{ 
				{ Constants.FIELD_COUPON_DEPT_ID, deptId },
				{ Constants.FIELD_COUPON_COUPON_ID, couponId }
			};
			var dv = base.Get(XML_KEY_NAME, "GetPublishCouponsCouponId", ht);
			return dv.Cast<DataRowView>().Select(drv => new CouponModel(drv)).ToArray();
		}
		#endregion

		#region ~SearchUserList ユーザー情報検索
		/// <summary>
		/// ユーザー情報検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		internal UserListSearchResult[] SearchUserList(BaseCouponSearchCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetUserList", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UserListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetUserCouponHistoryByUserIdAndCouponId ユーザーIDとクーポンIDからユーザークーポン履歴情報取得
		/// <summary>
		/// ユーザーIDとクーポンIDからユーザークーポン履歴情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns>検索結果</returns>
		internal UserCouponHistoryModel[] GetUserCouponHistoryByUserIdAndCouponId(string userId, string couponId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPONHISTORY_USER_ID, userId },
				{ Constants.FIELD_USERCOUPONHISTORY_COUPON_ID, couponId }
			};
			var dv = base.Get(XML_KEY_NAME, "GetUserCouponHistoryByUserIdAndCouponId", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponHistoryModel(drv)).ToArray();
		}
		#endregion

		#region ~GetCountOfUserListSearch ユーザー情報検索結果件数取得
		/// <summary>
		/// ユーザー情報検索結果件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果件数</returns>
		internal int GetCountOfUserListSearch(BaseCouponSearchCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetUserCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~SearchUserCouponHistory ユーザークーポン履歴情報検索
		/// <summary>
		/// ユーザークーポン履歴情報検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		internal UserCouponHistoryListSearchResult[] SearchUserCouponHistory(BaseCouponSearchCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetUserCouponHistory", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UserCouponHistoryListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetHistoiresAll ユーザークーポン履歴取得（全て）
		/// <summary>
		/// ユーザークーポン履歴取得（全て）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザークーポン履歴列</returns>
		internal UserCouponHistoryModel[] GetHistoiresAll(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCOUPONHISTORY_USER_ID, userId}
			};
			var dv = Get(XML_KEY_NAME, "GetHistoiresAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponHistoryModel(drv)).OrderBy(h => h.HistoryNo).ToArray();
		}
		#endregion

		#region ~InsertUserCouponHistory ユーザークーポン履歴登録
		/// <summary>
		/// ユーザークーポン履歴登録
		/// </summary>
		/// <param name="model">ユーザクーポン履歴テーブルモデル</param>
		/// <returns>件数</returns>
		internal int InsertUserCouponHistory(UserCouponHistoryModel model)
		{
			// 定期購入ID値はNULLを避けるため
			if (string.IsNullOrEmpty(model.FixedPurchaseId)) model.FixedPurchaseId = string.Empty;

			var result = Exec(XML_KEY_NAME, "InsertUserCouponHistory", model.DataSource);
			return result;
		}
		#endregion

		#region ~DeleteUserCouponHistory ユーザークーポン履歴削除
		/// <summary>
		/// ユーザークーポン履歴削除
		/// </summary>
		/// <param name="model">ユーザクーポン履歴テーブルモデル</param>
		/// <returns>件数</returns>
		internal int DeleteUserCouponHistory(UserCouponHistoryModel model)
		{
			var result = Exec(XML_KEY_NAME, "DeleteUserCouponHistory", model.DataSource);
			return result;
		}
		#endregion

		#region ~SearchCouponList クーポン情報検索
		/// <summary>
		/// クーポン情報検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		internal CouponListSearchResult[] SearchCouponList(CouponListSearchCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetCouponList", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new CouponListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~SearchCouponTransition クーポン推移情報検索(日別・月別の共通)
		/// <summary>
		/// クーポン推移情報検索(日別・月別の共通)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		internal CouponTransitionInfo[] SearchCouponTransition(CouponTransitionReportCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetCouponTransition", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new CouponTransitionInfo(drv)).ToArray();
		}
		#endregion

		#region ~SearchUnusedCouponPriceTransitionDay 未利用クーポン金額推移情報検索(日別)
		/// <summary>
		/// 未利用クーポン金額推移情報検索(日別)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		internal UnusedCouponTransitionInfo[] SearchUnusedCouponPriceTransitionDay(CouponTransitionReportCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetUnusedCouponPriceTransitionDay", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UnusedCouponTransitionInfo(drv)).ToArray();
		}
		#endregion

		#region ~SearchUnusedCouponPriceTransitionMonth 未利用クーポン金額推移情報検索(月別)
		/// <summary>
		/// 未利用クーポン金額推移情報検索(月別)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		internal UnusedCouponTransitionInfo[] SearchUnusedCouponPriceTransitionMonth(CouponTransitionReportCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetUnusedCouponPriceTransitionMonth", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UnusedCouponTransitionInfo(drv)).ToArray();
		}
		#endregion

		#region ~SearchUnusedCouponCountTransitionDay 未利用クーポン枚数推移情報検索(日別)
		/// <summary>
		///  未利用クーポン枚数推移情報検索(日別)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		internal UnusedCouponTransitionInfo[] SearchUnusedCouponCountTransitionDay(CouponTransitionReportCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetUnusedCouponCountTransitionDay", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UnusedCouponTransitionInfo(drv)).ToArray();
		}
		#endregion

		#region ~SearchUnusedCouponCountTransitionMonth 未利用クーポン枚数推移情報検索(月別)
		/// <summary>
		/// 未利用クーポン枚数推移情報検索(月別)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		internal UnusedCouponTransitionInfo[] SearchUnusedCouponCountTransitionMonth(CouponTransitionReportCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetUnusedCouponCountTransitionMonth", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UnusedCouponTransitionInfo(drv)).ToArray();
		}
		#endregion

		#region ~InsertUnusedCouponCountTransition 未利用クーポン枚数推移情報登録
		/// <summary>
		/// 未利用クーポン枚数推移情報登録
		/// </summary>
		/// <param name="model">サマリ分析結果テーブルモデル</param>
		/// <returns> 登録件数</returns>
		internal int InsertUnusedCouponCountTransition(DispSummaryAnalysisModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertUnusedCouponCountTransition", model.DataSource);
		}
		#endregion

		#region ~InsertUnusedCouponPriceTransition 未利用クーポン金額推移情報登録
		/// <summary>
		/// 未利用クーポン金額推移情報検索(月別)
		/// </summary>
		/// <param name="model">サマリ分析結果テーブルモデル</param>
		/// <returns> 登録結果</returns>
		internal int InsertUnusedCouponPriceTransition(DispSummaryAnalysisModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertUnusedCouponPriceTransition", model.DataSource);
		}
		#endregion

		#region ~GetCoupon クーポン情報詳細取得
		/// <summary>
		/// クーポン情報詳細取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns>クーポン情報</returns>
		internal CouponModel GetCoupon(string deptId, string couponId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPON_DEPT_ID, deptId },
				{ Constants.FIELD_COUPON_COUPON_ID, couponId }
			};
			var dv = base.Get(XML_KEY_NAME, "GetCoupon", ht);
			return dv.Cast<DataRowView>().Select(drv => new CouponModel(drv)).FirstOrDefault();
		}
		#endregion

		#region ~GetCouponsFromCouponCode クーポンコードからクーポン情報詳細取得（前方一致・先頭一件）
		/// <summary>
		/// クーポンコードからクーポン情報詳細取得（前方一致・先頭一件）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns>クーポン情報</returns>
		internal CouponModel[] GetCouponsFromCouponCode(string deptId, string couponCode)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPON_DEPT_ID, deptId },
				{ Constants.FIELD_COUPON_COUPON_CODE + "_like_escaped", couponCode}
			};
			var dv = base.Get(XML_KEY_NAME, "GetCouponsFromCouponCode", ht);
			return dv.Cast<DataRowView>().Select(drv => new CouponModel(drv)).ToArray();
		}
		#endregion

		#region ~GetCouponFromCouponCodePerfectMatch クーポンコード完全一致
		/// <summary>
		/// クーポン情報取得(クーポンコード完全一致指定)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns>クーポン情報</returns>
		internal CouponModel GetCouponFromCouponCodePerfectMatch(string deptId, string couponCode)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPON_DEPT_ID, deptId },
				{ Constants.FIELD_COUPON_COUPON_CODE, couponCode}
			};
			var dv = base.Get(XML_KEY_NAME, "GetCouponFromCouponCodePerfectMatch", ht);
			var couponModel = (dv.Count > 0) ? new CouponModel(dv[0]) : null;
			return couponModel;
		}
		#endregion

		#region ~GetCouponCount クーポン数検索（前方一致）
		/// <summary>
		/// クーポン数検索（前方一致）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>クーポン数</returns>
		internal int GetCouponCount(CouponListSearchCondition condition)
		{
			var dv = base.Get(XML_KEY_NAME, "GetCouponCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetAllCouponCount 全クーポン数取得
		/// <summary>
		/// 全クーポン数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>クーポン数</returns>
		internal int GetAllCouponCount(string deptId)
		{
			var dv = base.Get(XML_KEY_NAME, "GetAllCouponCount", new Hashtable() { { Constants.FIELD_COUPON_DEPT_ID, deptId } });
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetAllCoupons 全クーポン情報取得
		/// <summary>
		/// 全クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>クーポン情報</returns>
		internal CouponModel[] GetAllCoupons(string deptId)
		{
			var dv = base.Get(XML_KEY_NAME, "GetAllCoupons", new Hashtable { { Constants.FIELD_COUPON_DEPT_ID, deptId } });
			return dv.Cast<DataRowView>().Select(drv => new CouponModel(drv)).ToArray();
		}
		#endregion

		#region ~InsertCoupon クーポン情報登録
		/// <summary>
		/// クーポン情報を登録
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <returns>登録件数</returns>
		internal int InsertCoupon(CouponModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertCoupon", model.DataSource);
		}
		#endregion

		#region ~UpdateCoupon クーポン情報更新
		/// <summary>
		/// クーポン情報を更新
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <returns>更新件数</returns>
		internal int UpdateCoupon(CouponModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateCoupon", model.DataSource);
		}
		#endregion

		#region ~UpdateCouponCountDown クーポン情報、残り利用回数をマイナスする
		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <returns>更新件数</returns>
		internal int UpdateCouponCountDown(CouponModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateCouponCountDown", model.DataSource);
		}
		#endregion

		#region ~UpdateCouponCountDownIgnoreCouponCount クーポン情報、残り利用回数をマイナスする(現在のクーポン数を考慮しない)
		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする(現在のクーポン数を考慮しない)
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <returns>更新件数</returns>
		internal int UpdateCouponCountDownIgnoreCouponCount(CouponModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateCouponCountDownIgnoreCouponCount", model.DataSource);
		}
		#endregion

		#region ~UpdateCouponCountUp クーポン情報、残り利用回数をプラスする
		/// <summary>
		/// クーポン情報、残り利用回数をプラスする
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <returns>更新件数</returns>
		internal int UpdateCouponCountUp(CouponModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateCouponCountUp", model.DataSource);
		}
		#endregion

		#region ~DeleteCoupon クーポン情報削除
		/// <summary>
		/// クーポン情報を削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns>削除件数</returns>
		internal int DeleteCoupon(string deptId, string couponId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPON_DEPT_ID, deptId },
				{ Constants.FIELD_COUPON_COUPON_ID, couponId }
			};
			return base.Exec(XML_KEY_NAME, "DeleteCoupon", ht);
		}
		#endregion

		#region +GetAllUserCouponsSpecificExpireDate 有効期限指定ユーザクーポン情報取得
		/// <summary>
		/// 有効期限開始日が指定日時以降・有効期限終了日が指定日時以降のクーポン取得情報取得
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="expireDateTimeBegin">有効期限開始日時</param>
		/// <param name="expireDateTimeEnd">有効期限終了日時</param>
		/// <returns>クーポン情報</returns>
		public UserCouponDetailInfo[] GetAllUserCouponsSpecificExpireDate(string strUserId, string strDeptId, DateTime expireDateTimeBegin, DateTime expireDateTimeEnd)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCOUPON_USER_ID, strUserId},
				{Constants.FIELD_USERCOUPON_DEPT_ID, strDeptId},
				{"expire_bgn", expireDateTimeBegin},
				{"expire_end", expireDateTimeEnd}
			};

			var dv = base.Get(XML_KEY_NAME, "GetAllUserCouponsSpecificExpireDate", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCouponDetailInfo(drv)).ToArray();

		}
		#endregion

		#region ~GetCountCouponUseUser クーポン利用ユーザー件数取得
		/// <summary>
		/// クーポン利用ユーザー件数取得
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <returns>利用済みか</returns>
		internal int GetCountCouponUseUser(string couponId, string couponUseUser)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPONUSEUSER_COUPON_ID, couponId },
				{ Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER, couponUseUser }
			};
			var dv = Get(XML_KEY_NAME, "GetCountCouponUseUser", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetCouponUseUser クーポン利用ユーザー取得
		/// <summary>
		/// クーポン利用ユーザー取得
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <returns>クーポン利用ユーザー情報</returns>
		internal CouponUseUserModel GetCouponUseUser(string couponId, string couponUseUser)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPONUSEUSER_COUPON_ID, couponId },
				{ Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER, couponUseUser }
			};
			var dv = Get(XML_KEY_NAME, "GetCouponUseUser", ht);
			return dv.Cast<DataRowView>().Select(drv => new CouponUseUserModel(drv)).FirstOrDefault();
		}
		#endregion

		#region ~GetCouponUseUserByCouponUseUser ユーザー情報に紐づくクーポン利用ユーザー情報取得
		/// <summary>
		/// クーポン利用ユーザー取得
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <returns>クーポン利用ユーザー情報</returns>
		internal CouponUseUserModel[] GetCouponUseUserByCouponUseUser(string couponUseUser)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER, couponUseUser }
			};
			var dv = Get(XML_KEY_NAME, "GetCouponUseUserByCouponUseUser", ht);
			return dv.Cast<DataRowView>().Select(drv => new CouponUseUserModel(drv)).ToArray();
		}
		#endregion

		#region ~GetNextHistoryNo ユーザークーポン履歴Noを取得
		/// <summary>
		/// ユーザークーポン履歴Noを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザー履歴No</returns>
		internal int GetNextHistoryNo(string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPONHISTORY_USER_ID, userId }
			};
			var dv = Get(XML_KEY_NAME, "GetNextHistoryNo", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetCouponUseUserSearchHitCount クーポン利用ユーザー検索ヒット件数取得
		/// <summary>
		/// クーポン利用ユーザー検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>ヒット件数</returns>
		internal int GetCouponUseUserSearchHitCount(CouponUseUserListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetCouponUseUserSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~SearchCouponUseUser クーポン利用ユーザー検索
		/// <summary>
		/// クーポン利用ユーザー検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果</returns>
		internal CouponUseUserListSearchResult[] SearchCouponUseUser(CouponUseUserListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "SearchCouponUseUser", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new CouponUseUserListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~InsertCouponUseUser クーポン利用ユーザー登録
		/// <summary>
		/// クーポン利用ユーザー登録
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		/// <returns>登録件数</returns>
		internal int InsertCouponUseUser(CouponUseUserModel couponUseUser)
		{
			var result = Exec(XML_KEY_NAME, "InsertCouponUseUser", couponUseUser.DataSource);
			return result;
		}
		#endregion

		#region ~UpdateCouponUseUSer クーポン利用ユーザー更新
		/// <summary>
		/// クーポン利用ユーザー更新
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		/// <returns>更新件数</returns>
		internal int UpdateCouponUseUser(CouponUseUserModel couponUseUser)
		{
			var result = Exec(XML_KEY_NAME, "UpdateCouponUseUser", couponUseUser.DataSource);
			return result;
		}
		#endregion

		#region ~DeleteCouponUseUser クーポン利用ユーザー削除
		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <returns>処理件数</returns>
		internal int DeleteCouponUseUser(string couponId, string couponUseUser)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPONUSEUSER_COUPON_ID, couponId },
				{ Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER, couponUseUser }
			};
			var result = Exec(XML_KEY_NAME, "DeleteCouponUseUser", ht);
			return result;
		}
		#endregion

		#region ~DeleteCouponUseUserByCouponId クーポンIDからクーポン利用ユーザーを削除
		/// <summary>
		/// クーポンIDからクーポン利用ユーザー
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <returns>処理件数</returns>
		internal int DeleteCouponUseUserByCouponId(string couponId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPONUSEUSER_COUPON_ID, couponId }
			};
			var result = Exec(XML_KEY_NAME, "DeleteCouponUseUserByCouponId", ht);
			return result;
		}
		#endregion

		#region ~UpdateCouponUseUserOrderId クーポン利用ユーザー 注文ID更新
		/// <summary>
		/// クーポン利用ユーザー 注文ID更新
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		/// <returns>更新件数</returns>
		internal int UpdateCouponUseUserOrderId(CouponUseUserModel couponUseUser)
		{
			var result = Exec(XML_KEY_NAME, "UpdateCouponUseUserOrderId", couponUseUser.DataSource);
			return result;
		}
		#endregion

		#region ~DeleteCouponUseUserByOrderId クーポン利用ユーザー削除(注文ID指定)
		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>処理件数</returns>
		internal int DeleteCouponUseUserByOrderId(string couponId, string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPONUSEUSER_COUPON_ID, couponId },
				{ Constants.FIELD_COUPONUSEUSER_ORDER_ID, orderId }
			};
			var result = Exec(XML_KEY_NAME, "DeleteCouponUseUserByOrderId", ht);
			return result;
		}
		#endregion

		#region ~DeleteCouponUseUserByFixedPurchaseId クーポン利用ユーザー削除(定期購入ID指定)
		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>処理件数</returns>
		internal int DeleteCouponUseUserByFixedPurchaseId(string couponId, string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_COUPONUSEUSER_COUPON_ID, couponId },
				{ Constants.FIELD_COUPONUSEUSER_FIXED_PURCHASE_ID, fixedPurchaseId }
			};
			var result = Exec(XML_KEY_NAME, "DeleteCouponUseUserByFixedPurchaseId", ht);
			return result;
		}
		#endregion

		#region ~SearchCouponScheduleList クーポン発行スケジュールリスト検索
		/// <summary>
		/// クーポン発行スケジュールリスト検索
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>クーポン発行スケジュールリスト</returns>
		internal CouponScheduleListSearchResult[] SearchCouponScheduleList(CouponScheduleListSearchCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "SearchCouponSchedule", cond.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new CouponScheduleListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetCouponSchedule クーポン発行スケジュール取得
		/// <summary>
		/// クーポン発行スケジュール取得
		/// </summary>
		/// <param name="couponId">クーポン発行スケジュールID</param>
		/// <returns>クーポン発行スケジュール</returns>
		internal CouponScheduleModel GetCouponSchedule(string couponId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COUPONSCHEDULE_COUPON_SCHEDULE_ID, couponId},
			};
			var dv = base.Get(XML_KEY_NAME, "GetCouponSchedule", ht);
			return new CouponScheduleModel(dv.Cast<DataRowView>().FirstOrDefault());
		}
		#endregion

		#region ~GetCouponScheduleCountFromCouponId 識別ID・クーポンIDを元にクーポン発行スケジュール件数取得
		/// <summary>
		/// 識別ID・クーポンIDを元にクーポン発行スケジュール件数取得
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <returns>クーポン発行スケジュール件数</returns>
		internal int GetCouponScheduleCountFromCouponId(string couponId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPON_COUPON_ID, couponId }
			};
			var dv = Get(XML_KEY_NAME, "GetCouponScheduleCountFromCouponId", ht);
			var result = (int)dv[0][0];
			return result;
		}
		#endregion

		#region ~InsertCouponSchedule クーポン発行スケジュール登録
		/// <summary>
		/// クーポン発行スケジュール登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int InsertCouponSchedule(CouponScheduleModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertCouponSchedule", model.DataSource);
		}
		#endregion

		#region ~UpdateCouponSchedule クーポン発行スケジュール更新
		/// <summary>
		/// クーポン発行スケジュール更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>更新件数</returns>
		internal int UpdateCouponSchedule(CouponScheduleModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateCouponSchedule", model.DataSource);
		}
		#endregion

		#region ~DeleteCouponSchedule クーポン発行スケジュール削除
		/// <summary>
		/// クーポン発行スケジュール削除
		/// </summary>
		/// <param name="couponId">クーポン発行スケジュールID</param>
		/// <returns>削除件数</returns>
		internal int DeleteCouponSchedule(string couponId)
		{
			return base.Exec(XML_KEY_NAME, "DeleteCouponSchedule", new Hashtable { { Constants.FIELD_COUPONSCHEDULE_COUPON_SCHEDULE_ID, couponId } });
		}
		#endregion

		#region ~UpdateCouponScheduleStatus クーポン発行スケジュールステータス更新
		/// <summary>
		/// クーポン発行スケジュールステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>更新件数</returns>
		internal int UpdateCouponScheduleStatus(CouponScheduleModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateCouponScheduleStatus", model.DataSource);
		}
		#endregion

		#region ~UpdateCouponScheduleLastCount クーポン発行スケジュール最終付与人数更新
		/// <summary>
		/// クーポン発行スケジュールステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>更新件数</returns>
		internal int UpdateCouponScheduleLastCount(CouponScheduleModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateCouponScheduleLastCount", model.DataSource);
		}
		#endregion

		#region ~UpdateUserCouponForIntegration ユーザークーポン更新(ユーザー統合用)
		/// <summary>
		/// ユーザークーポン更新(ユーザー統合用)
		/// </summary>
		/// <param name="userId">ユーザーID(新)</param>
		/// <param name="userIdOld">ユーザーID(旧)</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">クーポンNo(新)</param>
		/// <param name="couponNoOld">クーポンNo(旧)</param>
		/// <returns>更新件数</returns>
		internal int UpdateUserCouponForIntegration(
			string userId,
			string userIdOld,
			string couponId,
			string couponNo,
			string couponNoOld)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPON_USER_ID, userId },
				{ Constants.FIELD_USERCOUPON_USER_ID + "_old", userIdOld },
				{ Constants.FIELD_USERCOUPON_COUPON_ID, couponId },
				{ Constants.FIELD_USERCOUPON_COUPON_NO, couponNo },
				{ Constants.FIELD_USERCOUPON_COUPON_NO + "_old", couponNoOld },
			};
			return base.Exec(XML_KEY_NAME, "UpdateUserCouponForIntegration", ht);
		}
		#endregion

		#region ~UpdateUserCouponHistoryForIntegration ユーザークーポン履歴更新(ユーザー統合用)
		/// <summary>
		/// ユーザークーポン履歴更新(ユーザー統合用)
		/// </summary>
		/// <param name="userId">ユーザーID(新)</param>
		/// <param name="userIdOld">ユーザーID(旧)</param>
		/// <param name="historyNo">履歴No(新)</param>
		/// <param name="historyNoOld">履歴No(旧)</param>
		/// <returns>更新件数</returns>
		internal int UpdateUserCouponHistoryForIntegration(
			string userId,
			string userIdOld,
			string historyNo,
			string historyNoOld)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPONHISTORY_USER_ID, userId },
				{ Constants.FIELD_USERCOUPONHISTORY_USER_ID + "_old", userIdOld },
				{ Constants.FIELD_USERCOUPONHISTORY_HISTORY_NO, historyNo },
				{ Constants.FIELD_USERCOUPONHISTORY_HISTORY_NO + "_old", historyNoOld },
			};
			return base.Exec(XML_KEY_NAME, "UpdateUserCouponHistoryForIntegration", ht);
		}
		#endregion

		#region +CheckTargetListUsed ターゲットリストで使われているか
		/// <summary>
		/// クーポン発行スケジュールステータス更新
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>クーポン発行スケジュール</returns>
		public CouponScheduleModel[] CheckTargetListUsed(string targetId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_TARGET_ID, targetId},
			};
			var dv = base.Get(XML_KEY_NAME, "CheckTargetListUsed", ht);
			return dv.Cast<DataRowView>().Select(drv => new CouponScheduleModel(drv)).ToArray();
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMaster(Hashtable input, string masterKbn, params KeyValuePair<string, string>[] replaces)
		{
			var statement = string.Empty;
			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON: // クーポンマスタ
					statement = "CheckCouponFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON:	// ユーザクーポンマスタ
					statement = "CheckUserCouponFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER:	// クーポン利用ユーザー情報
					statement = "CheckCouponUseUserFields";
					break;
			}
			Get(XML_KEY_NAME, statement, input, replaces: replaces);
		}

		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, statementName, input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, statementName, input, replaces: replaces);
			return dv;
		}
		#endregion

		#region ~SearchCouponsForAutosuggest
		/// <summary>
		/// Search coupons for autosuggest
		/// </summary>
		/// <param name="condition">Condition</param>
		/// <returns>Coupons</returns>
		internal UserCouponDetailInfo[] SearchCouponsForAutosuggest(CouponListSearchCondition condition)
		{
			var dv = Get(
				XML_KEY_NAME,
				"SearchCouponsForAutosuggest",
				condition.CreateHashtableParams());
			var coupons = dv.Cast<DataRowView>()
				.Select(drv => new UserCouponDetailInfo(drv))
				.ToArray();
			return coupons;
		}
		#endregion
	}
}
