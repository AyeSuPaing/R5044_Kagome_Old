/*
=========================================================================================================
  Module      : クーポンサービスのインタフェース(ICouponService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;
using w2.Domain.Coupon.Helper;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.Coupon
{
	/// <summary>
	/// クーポンサービスのインタフェース
	/// </summary>
	public interface ICouponService : IService
	{
		/// <summary>
		/// ユーザーが利用可能なクーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="mailAddress">メールアドレス</param>
		/// <param name="usedUserJudgeType">クーポン利用済みユーザー判定方法</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		UserCouponDetailInfo[] GetUserUsableCoupons(
			string deptId,
			string userId,
			string mailAddress,
			string usedUserJudgeType,
			SqlAccessor accessor = null);

		/// <summary>
		/// ユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		UserCouponDetailInfo[] GetUserCoupons(
			string deptId,
			string userId,
			SqlAccessor accessor = null);

		/// <summary>
		/// 有効期限にも関わらず、全てのユーザークーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		UserCouponDetailInfo[] GetAllUserCoupons(string deptId, string userId);

		/// <summary>
		/// 有効期限にも関わらず、全てのユーザークーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		UserCouponDetailInfo[] GetAllUserCoupons(string deptId, string userId, SqlAccessor accessor);

		/// <summary>
		/// ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns>ユーザークーポン情報</returns>
		UserCouponDetailInfo[] GetAllUserCouponsFromCouponCode(string deptId, string userId, string couponCode);

		/// <summary>
		/// ユーザーID・識別ID・クーポンコードを元にユーザークーポン取得(利用不可のものも含む)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <param name="referralCode">Referral code</param>
		/// <returns>ユーザークーポン情報</returns>
		UserCouponDetailInfo[] GetAllUserCouponsFromCouponCodeIncludeUnavailable(
			string deptId,
			string userId,
			string couponCode,
			string referralCode = "");

		/// <summary>
		/// ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">枝番</param>
		/// <returns>ユーザークーポン情報</returns>
		UserCouponDetailInfo GetUserCouponFromCouponNo(string deptId, string userId, string couponId, int couponNo);

		/// <summary>
		/// ユーザーID・識別ID・クーポンID・枝番を元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">枝番</param>
		/// <returns>ユーザークーポン情報一覧</returns>
		UserCouponDetailInfo[] GetAllUserCouponsFromCouponId(string deptId, string userId, string couponId, int couponNo);

		/// <summary>
		/// 識別ID・クーポンIDを元にユーザークーポン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns>ユーザークーポン情報</returns>
		UserCouponDetailInfo GetUserCouponFromCouponId(string deptId, string couponId);

		/// <summary>
		/// ユーザクーポン情報取得(本注文で発行したクーポン)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン情報</returns>
		UserCouponDetailInfo[] GetOrderPublishUserCoupon(string deptId, string userId, string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// 最大ユーザクーポン枝番取得
		/// </summary>
		/// <param name="deptId">ユーザID</param>
		/// <param name="userId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>最大ユーザクーポン枝番</returns>
		int GetUserCouponNo(string deptId, string userId, string couponId, SqlAccessor accessor = null);

		/// <summary>
		/// 利用可能ユーザークーポン数取得
		/// </summary>
		/// <param name="condition">クーポン情報検索条件情報</param>
		/// <returns>件数</returns>
		int GetAllUserUsableCouponsCount(CouponListSearchCondition condition);

		/// <summary>
		/// 利用可能ユーザークーポン取得
		/// </summary>
		/// <param name="condition">クーポン情報検索条件情報</param>
		/// <returns>ユーザクーポン一覧</returns>
		UserCouponDetailInfo[] GetAllUserUsableCoupons(CouponListSearchCondition condition);

		/// <summary>
		/// 指定されたユーザークーポン情報を登録
		/// </summary>
		/// <param name="couponInfo">登録ユーザークーポン情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="isPublishByOperator">オペレータが発行するか</param>
		/// <returns>登録件数</returns>
		int InsertUserCoupon(UserCouponDetailInfo couponInfo, UpdateHistoryAction updateHistoryAction, bool isPublishByOperator = true);

		/// <summary>
		/// 指定されたユーザークーポン情報を登録
		/// </summary>
		/// <param name="couponInfo">登録ユーザークーポン情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="isPublishByOperator">オペレータが発行するか</param>
		/// <returns>登録件数</returns>
		int InsertUserCoupon(UserCouponDetailInfo couponInfo, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, bool isPublishByOperator = true);

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
		bool InsertUserCouponWithOrderId(
			string userId,
			string orderId,
			string deptId,
			string couponId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

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
		bool InsertUserCouponWithOrderId(
			string userId,
			string orderId,
			string deptId,
			string couponId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

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
		bool UpdateUserCouponUseFlg(
			string userId,
			string deptId,
			string couponId,
			int couponNo,
			bool useFlg,
			DateTime dateChanged,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// ユーザクーポン更新(使用済み→未使用)
		/// </summary>
		/// <param name="couponInfo">ユーザークーポン情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateUnUseUserCoupon(UserCouponModel couponInfo, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// ユーザークーポン更新(ユーザークーポン利用可能回数)
		/// </summary>
		/// <param name="couponInfo">ユーザークーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功/失敗</returns>
		bool UpdateUseCouponCountUserCoupon(UserCouponModel couponInfo, SqlAccessor accessor = null);

		/// <summary>
		/// 指定されたユーザークーポン情報を削除
		/// </summary>
		/// <param name="deleteCouponInfo">削除ユーザークーポン情報</param>
		/// <param name="lastChanged">最終更新者（履歴用）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>削除件数</returns>
		int DeleteUserCoupon(
			UserCouponDetailInfo deleteCouponInfo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// ユーザークーポン情報削除(注文キャンセル時)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int DeleteUserCouponByOrderId(
			string userId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 期限切れユーザクーポン削除
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>削除件数</returns>
		int DeleteExpiredUserCoupon(string lastChanged, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 発行可能クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行可能クーポン情報一覧</returns>
		CouponModel[] GetAllPublishCoupons(string deptId, SqlAccessor accessor = null);

		/// <summary>
		/// 発行可能クーポン情報取得(発行期間を考慮しない)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行可能クーポン情報一覧</returns>
		CouponModel[] GetAllPublishCouponsNotPublishDate(string deptId, SqlAccessor accessor = null);

		/// <summary>
		/// クーポンタイプを元に発行可能クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponType">クーポン種別</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行可能クーポン情報一覧</returns>
		CouponModel[] GetPublishCouponsByCouponType(string deptId, string couponType, SqlAccessor accessor = null);

		/// <summary>
		/// クーポンIDを元に発行可能クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行可能クーポン情報</returns>
		CouponModel GetPublishCouponsById(string deptId, string couponId, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザー情報検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果 (0件の場合は0配列)</returns>
		UserListSearchResult[] SearchUserList(BaseCouponSearchCondition condition);

		/// <summary>
		/// ユーザーIDとクーポンIDからユーザークーポン履歴情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークーポン履歴</returns>
		UserCouponHistoryModel[] GetUserCouponHistoryByUserIdAndCouponId(string userId, string couponId, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザー情報検索結果件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果件数</returns>
		int GetCountOfUserListSearch(BaseCouponSearchCondition condition);

		/// <summary>
		/// ユーザークーポン履歴情報検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果 (0件の場合は0配列)</returns>
		UserCouponHistoryListSearchResult[] SearchUserCouponHistory(BaseCouponSearchCondition condition);

		/// <summary>
		/// ユーザークーポン履歴取得（全て）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザークーポン履歴列</returns>
		UserCouponHistoryModel[] GetHistoiresAll(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザークーポン履歴取得（全て）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>ユーザークーポン履歴列</returns>
		UserCouponHistoryModel[] GetHistoiresByOrderId(string userId, string orderId);

		/// <summary>
		/// ユーザークーポン履歴情報を登録
		/// </summary>
		/// <param name="couponHistory">ユーザークーポン履歴情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録件数</returns>
		int InsertUserCouponHistory(UserCouponHistoryModel couponHistory, SqlAccessor accessor = null);

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
		bool InsertUserCouponHistory(
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
			string fixedPurchaseId = "");

		/// <summary>
		/// ユーザークーポン履歴登録(キャンセル向け)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertUserCouponHistoryForCancel(string deptId, string userId, string orderId, string lastChanged, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザークーポン履歴情報を削除
		/// </summary>
		/// <param name="couponHistory">ユーザークーポン履歴情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		int DeleteUserCouponHistory(UserCouponHistoryModel couponHistory, SqlAccessor accessor = null);

		/// <summary>
		/// クーポン情報検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果 (0件の場合は0配列)</returns>
		CouponListSearchResult[] SearchCouponList(CouponListSearchCondition condition);

		/// <summary>
		/// クーポン推移レポート検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns> 検索結果</returns>
		CouponTransitionReportResult[] SearchCouponTransitionReport(CouponTransitionReportCondition condition);

		/// <summary>
		/// 未利用クーポン枚数推移情報登録
		/// </summary>
		/// <param name="model">サマリ分析結果テーブルモデル</param>
		/// <returns> 登録件数</returns>
		int CreateUnusedCouponCountReport(DispSummaryAnalysisModel model);

		/// <summary>
		/// 未利用クーポン金額推移情報登録
		/// </summary>
		/// <param name="model">サマリ分析結果テーブルモデル</param>
		/// <returns> 登録件数</returns>
		int CreateUnusedCouponPriceReport(DispSummaryAnalysisModel model);

		/// <summary>
		/// クーポン情報詳細取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns> クーポン情報</returns>
		CouponModel GetCoupon(string deptId, string couponId);

		/// <summary>
		/// クーポンコードからクーポン情報詳細取得（前方一致・先頭一件）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns> クーポン情報</returns>
		CouponModel[] GetCouponsFromCouponCode(string deptId, string couponCode);

		/// <summary>
		/// クーポン情報取得(クーポンコード完全一致指定)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <returns> クーポン情報</returns>
		CouponModel GetCouponFromCouponCodePerfectMatch(string deptId, string couponCode);

		/// <summary>
		/// クーポン数検索（前方一致）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>クーポン数</returns>
		int GetCouponCount(CouponListSearchCondition condition);

		/// <summary>
		/// 全クーポン数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>クーポン数</returns>
		int GetAllCouponCount(string deptId);

		/// <summary>
		/// 全クーポン情報取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>クーポン情報</returns>
		CouponModel[] GetAllCoupons(string deptId);

		/// <summary>
		/// クーポン情報登録
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <returns> 登録件数 </returns>
		int InsertCoupon(CouponModel model);

		/// <summary>
		/// クーポン情報更新
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <returns> 更新件数 </returns>
		int UpdateCoupon(CouponModel model);

		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		int UpdateCouponCountDown(CouponModel model, SqlAccessor accessor = null);

		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponCode">クーポンンコード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果(true: 更新件数 > 0, false: 更新件数 = 0)</returns>
		bool UpdateCouponCountDown(string deptId, string couponId, string couponCode, string lastChanged, SqlAccessor sqlAccessor);

		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする(現在のクーポン数を考慮しない)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponCode">クーポンンコード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果(true: 更新件数 > 0, false: 更新件数 = 0)</returns>
		bool UpdateCouponCountDownIgnoreCouponCount(
			string deptId,
			string couponId,
			string couponCode,
			string lastChanged,
			SqlAccessor sqlAccessor);

		/// <summary>
		/// クーポン情報、残り利用回数をマイナスする
		/// </summary>
		/// <param name="model">クーポン情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		int UpdateCouponCountDownIgnoreCouponCount(
			CouponModel model,
			SqlAccessor accessor);

		/// <summary>
		/// クーポン情報、残り利用回数をプラスする
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponCode">クーポンンコード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果(true: 更新件数 > 0, false: 更新件数 = 0)</returns>
		bool UpdateCouponCountUp(string deptId, string couponId, string couponCode, string lastChanged, SqlAccessor sqlAccessor);

		/// <summary>
		/// クーポン情報削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <returns> 削除件数 </returns>
		int DeleteCoupon(string deptId, string couponId);

		/// <summary>
		/// 有効期限開始日が指定日時以降・有効期限終了日が指定日時以降のクーポン情報取得
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="expireDateTimeBegin">有効期限開始日時</param>
		/// <param name="expireDateTimeEnd">有効期限終了日時</param>
		/// <returns>クーポン情報</returns>
		UserCouponDetailInfo[] GetAllUserCouponsSpecificExpireDate(string strUserId, string strDeptId, DateTime expireDateTimeBegin, DateTime expireDateTimeEnd);

		/// <summary>
		/// ブラックリスト型クーポンを利用済みか
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <returns>利用済みか</returns>
		bool CheckUsedCoupon(string couponId, string couponUseUser);

		/// <summary>
		/// クーポン利用ユーザー取得
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>クーポン利用ユーザー情報</returns>
		CouponUseUserModel GetCouponUseUser(string couponId, string couponUseUser, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// クーポン利用ユーザー取得
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>クーポン利用ユーザー情報</returns>
		CouponUseUserModel[] GetCouponUseUserByCouponUseUser(string couponUseUser, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// ユーザークーポン履歴Noを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ユーザー履歴No</returns>
		int GetNextHistoryNo(string userId, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// クーポン利用ユーザー検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>ヒット件数</returns>
		int GetCouponUseUserSearchHitCount(CouponUseUserListSearchCondition condition);

		/// <summary>
		/// クーポン利用ユーザー検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果</returns>
		CouponUseUserListSearchResult[] SearchCouponUseUser(CouponUseUserListSearchCondition condition);

		/// <summary>
		/// クーポン利用ユーザー登録
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		int InsertCouponUseUser(CouponUseUserModel couponUseUser, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// クーポン利用ユーザー更新
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		int UpdateCouponUseUser(CouponUseUserModel couponUseUser, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <returns>処理件数</returns>
		int DeleteCouponUseUser(
			string couponId,
			string couponUseUser);

		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		int DeleteCouponUseUser(
			string couponId,
			string couponUseUser,
			SqlAccessor accessor);

		/// <summary>
		/// クーポン利用ユーザー 注文ID更新
		/// </summary>
		/// <param name="couponUseUser">クーポン利用ユーザー情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateCouponUseUserOrderId(CouponUseUserModel couponUseUser, SqlAccessor accessor = null);

		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		int DeleteCouponUseUserByOrderId(string couponId, string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// クーポン利用ユーザー削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		int DeleteCouponUseUserByFixedPurchaseId(
			string couponId,
			string fixedPurchaseId,
			SqlAccessor accessor = null);

		/// <summary>
		/// クーポン発行スケジュールリスト検索
		/// </summary>
		/// <param name="cond">クーポン発行スケジュールリスト検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		CouponScheduleListSearchResult[] CouponScheduleListSearch(CouponScheduleListSearchCondition cond);

		/// <summary>
		/// クーポン発行スケジュール取得
		/// </summary>
		/// <param name="couponScheduleId">クーポンス発行ケジュールID</param>
		/// <returns>モデル</returns>
		CouponScheduleModel GetCouponSchedule(string couponScheduleId);

		/// <summary>
		/// クーポン発行スケジュール登録
		/// </summary>
		/// <param name="model">登録するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>登録した件数</returns>
		int InsertCouponSchedule(CouponScheduleModel model, SqlAccessor accessor = null);

		/// <summary>
		/// クーポン発行スケジュール更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		int UpdateCouponSchedule(CouponScheduleModel model, SqlAccessor accessor = null);

		/// <summary>
		/// クーポン発行スケジュール削除
		/// </summary>
		/// <param name="couponScheduleId">ポイントルールスケジュールID</param>
		/// <returns>削除した件数</returns>
		int DeleteCouponSchedule(string couponScheduleId);

		/// <summary>
		/// クーポン発行スケジュールステータス更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		int UpdateCouponScheduleStatus(CouponScheduleModel model, SqlAccessor accessor = null);

		/// <summary>
		/// クーポン発行スケジュールステータス更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		int UpdateCouponScheduleLastCount(CouponScheduleModel model, SqlAccessor accessor = null);

		/// <summary>
		/// ターゲットリストで使われているか
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>クーポン発行スケジュールモデル</returns>
		CouponScheduleModel[] CheckTargetListUsed(string targetId);

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
		bool ApplyNextShippingUseCouponToFixedPurchase(
			UserCouponDetailInfo coupon,
			string userId,
			string fixedPurchaseId,
			string couponUseUser,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

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
		int UpdateUserCouponForIntegration(
			string userId,
			string userIdOld,
			string couponId,
			string couponNo,
			string couponNoOld,
			SqlAccessor accessor);

		/// <summary>
		/// ユーザークーポン履歴更新(ユーザー統合用)
		/// </summary>
		/// <param name="userId">ユーザーID(新)</param>
		/// <param name="userIdOld">ユーザーID(旧)</param>
		/// <param name="historyNo">履歴No(新)</param>
		/// <param name="historyNoOld">履歴No(旧)</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateUserCouponHistoryForIntegration(
			string userId,
			string userIdOld,
			string historyNo,
			string historyNoOld,
			SqlAccessor accessor);

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
		bool ReturnNextShippingUseCoupon(
			UserCouponDetailInfo coupon,
			string userId,
			string orderId,
			string fixedPurchaseId,
			string lastChanged,
			string historyKbn,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// Search coupons for autosuggest
		/// </summary>
		/// <param name="condition">Condition</param>
		/// <returns>Coupons</returns>
		UserCouponDetailInfo[] SearchCouponsForAutosuggest(CouponListSearchCondition condition);
	}
}