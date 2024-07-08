/*
=========================================================================================================
  Module      : 決済カード連携サービスのインタフェース(IUserCreditCardService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Common.Sql;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.UserCreditCard
{
	/// <summary>
	/// 決済カード連携サービスのインタフェース
	/// </summary>
	public interface IUserCreditCardService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		UserCreditCardModel Get(string userId, int branchNo, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザーIDで取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		UserCreditCardModel[] GetByUserId(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 連携ID1から取得
		/// </summary>
		/// <param name="cooperationId1">連携ID1</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		UserCreditCardModel GetByCooperationId1(string cooperationId1, SqlAccessor accessor = null);

		/// <summary>
		///  利用可能かユーザークレカ未登録のものを取得（管理画面で利用）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード情報</returns>
		UserCreditCardModel[] GetUsableOrUnregisterd(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 利用可能なもの取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード情報</returns>
		UserCreditCardModel[] GetUsable(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 枝番の最大値取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>枝番の最大値</returns>
		int GetMaxBranchNo(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// ゼウス用の決済連携ID2を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">ブランチNo</param>
		/// <returns>ゼウス用の決済連携ID2</returns>
		string GetCooperationId2ForZeus(string userId, int branchNo);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル（枝番は採番される）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>発行した枝番</returns>
		int Insert(UserCreditCardModel model, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル（枝番は採番される）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>発行した枝番</returns>
		int Insert(UserCreditCardModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>影響を受けた件数</returns>
		bool Modify(
			string userId,
			int branchNo,
			Action<UserCreditCardModel> updateAction,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		bool Modify(
			string userId,
			int branchNo,
			Action<UserCreditCardModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 更新　※モバイル版でのみ利用
		/// </summary>
		/// <param name="model">ユーザークレジットカードモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>影響件数</returns>
		bool Update(UserCreditCardModel model, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 更新　※モバイル版でのみ利用
		/// </summary>
		/// <param name="model">ユーザークレジットカードモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響件数</returns>
		bool Update(UserCreditCardModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// 連携ID更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="cooperationId">連携ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新 成功/失敗</returns>
		bool UpdateCooperationId(
			string userId,
			int branchNo,
			string cooperationId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 表示フラグ更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="dispFlg">表示フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新 成功/失敗</returns>
		bool UpdateDispFlg(
			string userId,
			int branchNo,
			bool dispFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

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
		bool UpdateDispFlg(
			string userId,
			int branchNo,
			bool dispFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>結果</returns>
		bool Delete(
			string userId,
			int branchNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		bool Delete(string userId, int branchNo, SqlAccessor accessor);

		/// <summary>
		/// Get Max Branch No By User Id And Cooperation Type
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="cooperationType">Cooperation Type</param>
		/// <returns>Max Branch No</returns>
		int GetMaxBranchNoByUserIdAndCooperationType(
			string userId,
			string cooperationType);

		/// <summary>
		/// Get User Credit Card Expired For Payment Paidys
		/// </summary>
		/// <param name="paymentPaidyTokenDeleteLimitDay">Payment Paidy Token Delete Limit Day</param>
		/// <param name="cooperationType">Cooperation Type</param>
		/// <param name="maskString">Mask String</param>
		/// <returns>User Credit Card Expired For Payment Paidys</returns>
		List<UserCreditCardModel> GetUserCreditCardExpiredForPaymentPaidys(
			int paymentPaidyTokenDeleteLimitDay,
			string cooperationType,
			string maskString);
	}
}