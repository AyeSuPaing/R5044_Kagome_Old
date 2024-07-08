/*
=========================================================================================================
  Module      : ポイントサービスのインタフェース(IPointService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;
using w2.Domain.Point.Helper;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.Point
{
	/// <summary>
	/// ポイントサービスのインタフェース
	/// </summary>
	public interface IPointService : IService
	{
		/// <summary>
		/// w2ポイントマスタ取得
		/// </summary>
		/// <returns>
		/// メモリに貯めてるものを取得。メモリになければDBから取得
		/// </returns>
		PointModel[] GetPointMaster();

		/// <summary>
		/// ユーザーのポイント情報を取得
		/// </summary>
		/// <param name="userId">取得条件となるユーザーID</param>
		/// <param name="cartId">カートID（後から増えたため、アクセサの後ろ）</param>
		/// <param name="accessor">
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// </param>
		/// <returns>
		/// ユーザーのポイント情報。
		/// ポイントの情報が取れなければ0配列
		/// </returns>
		UserPointModel[] GetUserPoint(string userId, string cartId, SqlAccessor accessor = null);

		/// <summary>
		/// 注文に利用可能ポイントな合計ポイントを取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns></returns>
		decimal GetUserPointUsableTotal(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザーのポイント情報をヘルパクラスでラップして取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ポイント情報</returns>
		UserPointList GetUserPointList(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザーポイント登録
		/// </summary>
		/// <param name="model">ユーザーポイントモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		bool RegisterUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// ユーザーポイント登録
		/// </summary>
		/// <param name="model">ユーザーポイントモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>成功したか</returns>
		bool RegisterUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// ユーザーポイント更新
		/// </summary>
		/// <param name="model">更新するユーザーポイントモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザーポイント削除
		/// </summary>
		/// <param name="model">更新するユーザーポイントモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int DeleteUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor = null);

		/// <summary>
		/// ポイントルールを取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="pointRuleId">取得対象のポイントルールID</param>
		/// <returns>
		/// 取得できなかった場合はNull
		/// </returns>
		PointRuleModel GetPointRule(string deptId, string pointRuleId);

		/// <summary>
		/// 全ポイントルール取得
		/// </summary>
		PointRuleModel[] GetAllPointRules();

		/// <summary>
		/// ポイントルール登録
		/// </summary>
		/// <param name="model">登録するModel</param>
		/// <returns>登録した件数</returns>
		int RegisterPointRule(PointRuleModel model);

		/// <summary>
		/// UpdatePointRule
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <returns>更新↓件数</returns>
		int UpdatePointRule(PointRuleModel model);

		/// <summary>
		/// UpdatePointRule
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <returns>削除した件数</returns>
		int DeletePointRule(PointRuleModel model);

		/// <summary>
		/// ポイントルールリスト検索
		/// </summary>
		/// <param name="cond">ポイントルールリスト検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		PointRuleListSearchResult[] PointRuleListSearch(PointRuleListSearchCondition cond);

		/// <summary>
		/// ポイント推移レポート検索
		/// </summary>
		/// <param name="cond">ポイント推移レポート検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		PointTransitionReportResult[] PointTransitionReportSearch(PointTransitionReportCondition cond);

		/// <summary>
		/// ユーザーポイント履歴（概要）検索数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索ヒット数</returns>
		int GetSearchHitCountUserPointHistorySummary(UserPointHistorySummarySearchCondition condition);

		/// <summary>
		/// ユーザーポイント履歴（概要）検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果</returns>
		UserPointHistorySummarySearchResult[] SearchUserPointHistorySummary(UserPointHistorySummarySearchCondition condition);

		/// <summary>
		/// ユーザーポイント履歴検索
		/// </summary>
		/// <param name="cond">ユーザーポイント履歴検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		UserPointHistorySearchResult[] UserPointHistorySearch(UserPointHistorySearchCondition cond);

		/// <summary>
		/// ユーザーポイント検索
		/// </summary>
		/// <param name="cond">ユーザーポイント検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		UserPointSearchResult[] UserPointListSearch(UserPointSearchCondition cond);

		/// <summary>
		/// ユーザーポイント検索件数取得
		/// </summary>
		/// <param name="cond">ユーザーポイント検索条件クラス</param>
		/// <returns>検索件数</returns>
		int GetCountOfUserPointListSearch(UserPointSearchCondition cond);

		/// <summary>
		/// ユーザーポイント履歴取得
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>取得した履歴</returns>
		UserPointHistoryModel[] GetUserPointHistories(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 注文IDに紐づく、まだ戻し処理が行われていない（消費されている状態）の
		/// ユーザーポイント履歴を取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザーポイント履歴</returns>
		/// <remarks>V5.13以前のユーザーポイント履歴は含まれない</remarks>
		UserPointHistoryModel[] GetUserPointHistoriesNotRestoredByOrderId(string userId, string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// 指定注文IDのユーザーポイント履歴削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="kbn1">区分値1（注文ID）</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <param name="lastChanged">最終更新者（履歴用）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		int DeleteUserPointHistoryByOrderId(
			string userId,
			string kbn1,
			string pointKbn,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// ユーザーポイント履歴登録
		/// </summary>
		/// <param name="model">ユーザーポイント履歴モデル</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>登録件数</returns>
		int RegisterHistory(UserPointHistoryModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 本ポイント移行対象の仮ポイントを取得
		/// </summary>
		/// <param name="daysFromShippingForBasePoint">出荷後何日で本ポイントへ移行するか(通常ポイント)</param>
		/// <param name="daysFromShippingForLimitedTermPoint">出荷後何日で本ポイントへ移行するか(期間限定ポイント)</param>
		/// <returns></returns>
		UserPointModel[] GetTargetUserTempPointToReal(int daysFromShippingForBasePoint, int daysFromShippingForLimitedTermPoint);

		/// <summary>
		/// 有効期限切れポイント取得
		/// </summary>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		/// <returns></returns>
		UserPointModel[] GetExpiredUserPoints(SqlAccessor sqlAccessor);

		/// <summary>
		/// 有効期限が切れたポイント削除
		/// </summary>
		/// <param name="model">ユーザーポイント履歴モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="isRegistHistory">履歴を登録するか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>削除件数</returns>
		int DeleteExpiredPoint(
			UserPointModel model,
			string lastChanged,
			bool isRegistHistory,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// ユーザポイント更新ロック取得(ポイントバッチ利用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		void GetUpdLockForUserPoint(string userId, SqlAccessor sqlAccessor);

		/// <summary>
		/// ユーザポイント履歴更新ロック取得(ポイントバッチ利用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		void GetUpdLockForUserPointHistory(string userId, SqlAccessor sqlAccessor);

		/// <summary>
		/// 優先度の高いポイントキャンペーン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="currentDateTime">対象とする現在日</param>
		/// <returns>優先度の高いポイントキャンペーン</returns>
		PointRuleModel[] GetHightPriorityCampaignRule(string deptId, DateTime currentDateTime);

		/// <summary>
		/// 基本ルール取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="currentDateTime">対象とする現在日</param>
		/// <returns>基本ルール</returns>
		PointRuleModel[] GetBasicRule(string deptId, DateTime currentDateTime);

		/// <summary>
		/// ポイントルールスケジュールリスト検索
		/// </summary>
		/// <param name="cond">ポイントルールスケジュールリスト検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		PointRuleScheduleListSearchResult[] PointRuleScheduleListSearch(PointRuleScheduleListSearchCondition cond);

		/// <summary>
		/// ポイントルールスケジュール取得
		/// </summary>
		/// <param name="pointRuleScheduleId">ポイントルールスケジュールID</param>
		/// <returns>モデル</returns>
		PointRuleScheduleModel GetPointRuleSchedule(string pointRuleScheduleId);

		/// <summary>
		/// Get Point Rule Schedule By Point Rule Id
		/// </summary>
		/// <param name="pointRuleId">Point Rule Id</param>
		/// <returns>Point Rule Schedules</returns>
		PointRuleScheduleModel[] GetPointRuleScheduleByPointRuleId(string pointRuleId);

		/// <summary>
		/// ポイントルールスケジュール登録
		/// </summary>
		/// <param name="model">登録するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>登録した件数</returns>
		int InsertPointRuleSchedule(PointRuleScheduleModel model, SqlAccessor accessor = null);

		/// <summary>
		/// ポイントルールスケジュール更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		int UpdatePointRuleSchedule(PointRuleScheduleModel model, SqlAccessor accessor = null);

		/// <summary>
		/// ポイントルールスケジュール削除
		/// </summary>
		/// <param name="pointRuleScheduleId">ポイントルールスケジュールID</param>
		/// <returns>削除した件数</returns>
		int DeletePointRuleSchedule(string pointRuleScheduleId);

		/// <summary>
		/// ポイントルールスケジュールステータス更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		int UpdatePointRuleScheduleStatus(PointRuleScheduleModel model, SqlAccessor accessor = null);

		/// <summary>
		/// オペレータによるポイント調整を実施
		/// </summary>
		/// <param name="pointOperationContents">ポイント操作内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>ポイント操作件数</returns>
		int PointOperation(
			PointOperationContents pointOperationContents,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 仮ポイントから本ポイントへ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		int TempToRealPoint(
			string userId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 仮ポイントから本ポイントへ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int TempToRealPoint(
			string userId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 仮ポイントから本ポイントへ
		/// </summary>
		/// <param name="tempPoint">仮ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		int TempToRealPoint(UserPointModel tempPoint, string lastChanged, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 仮ポイントから本ポイントへ
		/// </summary>
		/// <param name="tempPoint">仮ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		int TempToRealPoint(
			UserPointModel tempPoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 購入時のポイント利用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="cartId">カートID</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		int UsePointForBuy(
			string deptId,
			string userId,
			string orderId,
			decimal usePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string cartId = "");

		/// <summary>
		/// 購入時のポイント利用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="cartId">カートID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		int UsePointForBuy(
			string deptId,
			string userId,
			string orderId,
			decimal usePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string cartId = "",
			SqlAccessor accessor = null);

		/// <summary>
		/// 利用ポイントの戻し
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="usedPoint">戻す利用したポイント数</param>
		/// <param name="orderId">ポイント戻しの原因となったキャンセル注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="shouldRestoreExpiredPoint">期限切れのポイントを戻すか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>True:更新成功または更新対象なし False:楽観ロックNG</returns>
		/// <remarks>
		/// アクセサの指定がない場合はメソッド内でトランザクション完結
		/// </remarks>
		/// <returns>成功したか</returns>
		bool CancelUsedPointForBuy(
			string userId,
			string deptId,
			decimal usedPoint,
			string orderId,
			string lastChanged,
			bool shouldRestoreExpiredPoint,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// ユーザーポイントUpSert
		/// </summary>
		/// <param name="model">Updsert内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>UpSert件数</returns>
		int UpsertUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// 付与ポイントの戻し
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="addedPoint">戻す付与したポイント数（本ポイント又は仮ポイントから減算するポイント数）</param>
		/// <param name="orderId">ポイント戻しの原因となったキャンセル注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>True:更新成功または更新対象なし False:楽観ロックNG</returns>
		/// <remarks>
		/// 購入時ポイントがすでに本ポイントに移行している場合は本ポイントより減算
		/// まだ仮ポイント状態の場合は仮ポイントを削除する
		/// アクセサの指定がない場合はメソッド内でトランザクション完結
		/// </remarks>
		bool CancelAddedPointForBuy(
			string userId,
			string deptId,
			decimal addedPoint,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 利用ポイントを再計算
		/// </summary>
		/// <returns>更新件数 ０は楽観ロックによる更新失敗</returns>
		int RecalcOrderUsePoint(
			string userId,
			string oldOrderId,
			string newOrderId,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 利用ポイント再計算
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="oldOrderId">復元のキーになる注文ID</param>
		/// <param name="newOrderId">消費処理で使用する注文ID</param>
		/// <param name="newUsePoint">利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響件数</returns>
		int RecalcOrderUsePoint(
			string userId,
			string oldOrderId,
			string newOrderId,
			decimal newUsePoint,
			string lastChanged,
			SqlAccessor accessor);

		/// <summary>
		/// 利用ポイントを再計算(返品交換用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="oldOrderId">復元のキーになる注文ID</param>
		/// <param name="newOrderId">消費処理で使用する注文ID</param>
		/// <param name="newUsePoint">利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響件数</returns>
		/// <remarks>消費されたポイントを全て通常本ポイントとして加算する。</remarks>
		int RecalcOrderUsePointForReturnExchange(
			string userId,
			string oldOrderId,
			string newOrderId,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 利用ポイント再計算(返品交換用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="oldOrderId">復元のキーになる注文ID</param>
		/// <param name="newOrderId">消費処理で使用する注文ID</param>
		/// <param name="newUsePoint">利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響件数</returns>
		/// <remarks>消費されたポイントを全て通常本ポイントとして加算する。</remarks>
		int RecalcOrderUsePointForReturnExchange(
			string userId,
			string oldOrderId,
			string newOrderId,
			decimal newUsePoint,
			string lastChanged,
			SqlAccessor accessor);

		/// <summary>
		/// 購入失敗時のポイントロールバック
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">失敗した注文ID</param>
		/// <param name="orderPointUse">ロールバックする利用ポイント数</param>
		/// <param name="orderPointAdd">ロールバックする付与ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>履歴削除件数</returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		int RollbackUserPointForBuyFailure(
			string userId,
			string orderId,
			decimal orderPointUse,
			decimal orderPointAdd,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 購入失敗時のポイントロールバック
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">失敗した注文ID</param>
		/// <param name="orderPointUse">ロールバックする利用ポイント数</param>
		/// <param name="orderPointAdd">ロールバックする付与ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor"></param>
		/// <returns>履歴削除件数</returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		int RollbackUserPointForBuyFailure(
			string userId,
			string orderId,
			decimal orderPointUse,
			decimal orderPointAdd,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// ポイントルールを基にしてポイント発行
		/// </summary>
		/// <param name="rule">ポイントルール</param>
		/// <param name="userId">発行するユーザーID</param>
		/// <param name="orderId">発行元となった注文ID</param>
		/// <param name="issuePoint">発行するポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="productId">発行元になった商品ID</param>
		/// <returns>登録件数</returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		int IssuePointByRule(
			PointRuleModel rule,
			string userId,
			string orderId,
			decimal issuePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string productId = "");

		/// <summary>
		/// ポイントルールを基にしてポイント発行
		/// </summary>
		/// <param name="rule">ポイントルール</param>
		/// <param name="userId">発行するユーザーID</param>
		/// <param name="orderId">発行元となった注文ID</param>
		/// <param name="issuePoint">発行するポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="productId">発行元になった商品ID</param>
		/// <returns>登録件数</returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		int IssuePointByRule(
			PointRuleModel rule,
			string userId,
			string orderId,
			decimal issuePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			string productId ="");

		/// <summary>
		/// 返品交換時の付与ポイント調整（仮ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pointKbnNo">ポイント区分No</param>
		/// <param name="orderId">交換先の注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="adjustPoint">調整ポイント数</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		int AdjustOrderPointAddForPointTemp(
			string deptId,
			string userId,
			int pointKbnNo,
			string orderId,
			decimal adjustPoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 返品交換時の通常 付与ポイント調整（本ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">返品元の注文ID</param>
		/// <param name="adjustBasePoint">通常 調整ポイント数</param>
		/// <param name="errorMessageFormat">エラー時 メッセージフォーマット</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しない場合はメソッド内でトランザクション完結)</param>
		/// <param name="baseUpdateCount">out 更新レコード数</param>
		/// <returns>エラー時 エラーメッセージ内容</returns>
		string AdjustOrderBasePointAddForPointComp(
			string deptId,
			string userId,
			string orderId,
			decimal adjustBasePoint,
			string errorMessageFormat,
			string lastChanged,
			SqlAccessor sqlAccessor,
			out int baseUpdateCount);

		/// <summary>
		/// 返品交換時の期間限定 付与ポイント調整（本ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">返品元の注文ID</param>
		/// <param name="adjustLimitPoint">期間限定 調整ポイント数</param>
		/// <param name="errorMessageFormat">エラー時 メッセージフォーマット</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しない場合はメソッド内でトランザクション完結)</param>
		/// <param name="limitUpdateCount">out 更新レコード数</param>
		/// <returns>エラー時 エラーメッセージ</returns>
		string AdjustOrderLimitPointAddForPointComp(
			string deptId,
			string userId,
			string orderId,
			decimal adjustLimitPoint,
			string errorMessageFormat,
			string lastChanged,
			SqlAccessor sqlAccessor,
			out int limitUpdateCount);

		/// <summary>
		/// 注文で付与されたポイントを取り消す
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="addedPoint">付与したポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <param name="memo">メモ（履歴に記載）</param>
		/// <returns>影響件数</returns>
		/// <remarks>
		/// 仮ポイントと期間限定ポイントを物理削除する
		/// 取消合計が付与ポイントを満たさない場合は通常本ポイントから減算を行う
		/// </remarks>
		int RevokeGrantedUserPointByOrderId(
			string userId,
			string orderId,
			decimal addedPoint,
			string lastChanged,
			SqlAccessor accessor,
			string memo = "");

		/// <summary>
		/// ユーザー統合時にポイントを代表ユーザーに統合する
		/// </summary>
		/// <param name="contents">ポイント統合内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLパラメータ</param>
		/// <returns>ユーザーポイント履歴枝番</returns>
		/// <remarks>仮・本ポイントが移行されます</remarks>
		int ExecutePointIntegration(
			PointIntegrationContents contents,
			UpdateHistoryAction updateHistoryAction,
			string lastChanged,
			SqlAccessor accessor);

		/// <summary>
		/// ユーザー統合時に代表ユーザーに統合したポイントを元に戻す
		/// </summary>
		/// <param name="contents">ポイント統合キャンセル内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLパラメータ</param>
		/// <remarks>
		/// 仮・本ポイントが戻されます。
		/// ※仮ポイントを代表ユーザーに統合し、代表ユーザー側で本ポイントに移行されたポイントは戻りません。
		/// </remarks>
		int CancelPointIntegration(
			PointIntegrationCancelContents contents,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 利用可能金額から利用可能ポイント数に変換
		/// </summary>
		/// <param name="useablePrice">最大利用可能金額</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <returns>利用可能ポイント数</returns>
		decimal GetUseablePointFromPrice(decimal useablePrice, string pointKbn);

		/// <summary>
		/// 注文ポイント利用額取得
		/// </summary>
		/// <param name="orderPointUse">利用ポイント</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <returns>注文ポイント利用額</returns>
		decimal GetOrderPointUsePrice(decimal orderPointUse, string pointKbn);

		/// <summary>
		/// ユーザポイントに次回購入の利用ポイントを適用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="oldUsePoint">（更新前）利用ポイント数</param>
		/// <param name="newUsePoint">（更新後）利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		bool ApplyNextShippingUsePointToUserPoint(
			string deptId,
			string fixedPurchaseId,
			string userId,
			decimal oldUsePoint,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入解約時、利用ポイント数をユーザポイントに戻す
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>成功したか</returns>
		bool ReturnNextShippingUsePointToUserPoint(
			string deptId,
			string userId,
			string fixedPurchaseId,
			decimal usePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 設定された次回購入の利用ポイント履歴を生成した注文に適用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderUsePoint">注文の利用ポイント</param>
		/// <param name="nextShippingUsePoint">次回購入の利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		bool ApplyNextShippingUsePointToOrder(
			string deptId,
			string userId,
			string fixedPurchaseId,
			string orderId,
			decimal orderUsePoint,
			decimal nextShippingUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 利用ポイント変更時の購入時付与仮ポイント調整
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="orderId">対象注文ID</param>
		/// <param name="isFirstBuy">初回購入の注文か</param>
		/// <param name="pointFirstBuy">初回購入発行ポイント数</param>
		/// <param name="pointOrder">購入時発行ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>処理件数</returns>
		int AdjustOrderPointAddForPointTempAtUsePointChange(
			string userId,
			string orderId,
			bool isFirstBuy,
			decimal pointFirstBuy,
			decimal pointOrder,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// ターゲットリストで使われているか
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>ポイントルールスケジュールモデル</returns>
		PointRuleScheduleModel[] CheckTargetListUsed(string targetId);

		/// <summary>
		/// 返品交換時の利用ポイント調整（本ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">返品元の注文ID</param>
		/// <param name="adjustPoint">調整ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ(指定しない場合はメソッド内でトランザクション完結)</param>
		/// <returns>更新件数</returns>
		int AdjustOrderPointUseForPointComp(
			string deptId,
			string userId,
			string orderId,
			decimal adjustPoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文IDでユーザーポイント履歴取得
		/// </summary>
		/// <param name="userId">ユーザーIS</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザーポイント履歴</returns>
		UserPointHistoryModel[] GetUserPointHistoryByOrderId(
			string userId,
			string orderId,
			SqlAccessor accessor = null);

		/// <summary>
		/// 履歴グループ番号で取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="historyGroupNo">履歴グループ番号</param>
		/// <returns>ユーザーポイント履歴モデル</returns>
		UserPointHistoryModel[] GetHistoriesByGroupNo(string userId, int historyGroupNo);

		/// <summary>
		/// ポイント枝番を採番
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>枝番</returns>
		int IssuePointKbnNoForUser(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 復元処理用にポイント履歴取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="isBeforeMigration">VUP前の履歴であったか？</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザーポイント履歴</returns>
		UserPointHistoryModel[] GetUserPointHistoriesForRestore(
			string userId,
			string orderId,
			string lastChanged,
			out bool isBeforeMigration,
			SqlAccessor accessor);

		/// <summary>
		/// Adjust point by Cross Point
		/// </summary>
		/// <param name="model">User point model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of cases affected</returns>
		int AdjustPointByCrossPoint(UserPointModel model, SqlAccessor accessor);

		/// <summary>
		/// Issue point by Cross Point
		/// </summary>
		/// <param name="model">User point</param>
		/// <param name="accessor">Sql accessor</param>
		/// <param name="isRegister">Is register</param>
		/// <returns>Number of cases affected</returns>
		int IssuePointByCrossPoint(
			UserPointModel model,
			SqlAccessor accessor,
			bool isRegister = true);

		/// <summary>
		/// ユーザーポイント履歴取得（Front表示用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザーポイント履歴情報</returns>
		UserPointHistoryContainer[] GetUserPointHistoriesOnFront(string userId);
	}
}
