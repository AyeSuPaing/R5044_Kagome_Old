/*
=========================================================================================================
  Module      : 定期購入情報サービスのインタフェース(IFixedPurchaseService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入情報サービスのインタフェース
	/// </summary>
	public interface IFixedPurchaseService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		FixedPurchaseModel Get(string fixedPurchaseId, SqlAccessor accessor = null);

		/// <summary>
		/// 更新ロック取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期購入ID</returns>
		string GetUpdLock(string fixedPurchaseId, SqlAccessor accessor);

		/// <summary>
		/// ユーザーIDから定期購入情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデルリスト</returns>
		FixedPurchaseModel[] GetFixedPurchasesByUserId(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 注文対象の定期購入取得
		/// </summary>
		/// <returns>モデル列</returns>
		FixedPurchaseModel[] GetTargetsForCreateOrder();

		/// <summary>
		/// 定期台帳取得_LINE連携
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="offset">開始位置</param>
		/// <param name="limit">最大件数</param>
		/// <param name="updateAt">取得時間範囲</param>
		/// <returns>定期データリスト</returns>
		FixedPurchaseModel[] GetFixedPurchasesForLine(
			string userId,
			int offset,
			int limit,
			DateTime updateAt);

		/// <summary>
		/// 変更期限案内メール送信対象の定期購入取得(全件）
		/// </summary>
		/// <returns>モデル列</returns>
		FixedPurchaseModel[] GetTargetsForSendChangeDeadlineMail();

		/// <summary>
		/// 決済ステータス更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="paymentStatus">決済ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>影響を受けた件数</returns>
		int UpdatePaymentStatus(
			string fixedPurchaseId,
			string paymentStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 決済ステータス更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="paymentStatus">決済ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int UpdatePaymentStatus(
			string fixedPurchaseId,
			string paymentStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// ユーザーID更新（ユーザー統合向け）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		void UpdateUserIdForIntegration(
			string fixedPurchaseId,
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 次回購入の利用クーポン更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">クーポン番号</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新結果</returns>
		bool UpdateNextShippingUseCoupon(
			string fixedPurchaseId,
			string couponId,
			int? couponNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 次回購入の利用クーポンリセット
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新結果</returns>
		bool ResetNextShippingUseCoupon(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 領収書情報更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="receiptFlg">領収書希望フラグ</param>
		/// <param name="receiptAddress">宛名</param>
		/// <param name="receiptProviso">但し書き</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		int UpdateReceiptInfo(
			string fixedPurchaseId,
			string receiptFlg,
			string receiptAddress,
			string receiptProviso,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 領収書情報更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="receiptFlg">領収書希望フラグ</param>
		/// <param name="receiptAddress">宛名</param>
		/// <param name="receiptProviso">但し書き</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateReceiptInfo(
			string fixedPurchaseId,
			string receiptFlg,
			string receiptAddress,
			string receiptProviso,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 解約理由取得
		/// </summary>
		/// <param name="cancelReasonId">解約理由区分ID</param>
		/// <returns>モデル</returns>
		FixedPurchaseCancelReasonModel GetCancelReason(string cancelReasonId);

		/// <summary>
		/// 解約理由取得（全て）
		/// </summary>
		/// <returns>モデル列</returns>
		FixedPurchaseCancelReasonModel[] GetCancelReasonAll();

		/// <summary>
		/// 解約理由取得（表示範囲が「EC管理」のみ）
		/// </summary>
		/// <returns></returns>
		FixedPurchaseCancelReasonModel[] GetCancelReasonForEC();

		/// <summary>
		/// 解約理由取得（表示範囲が「PC/スマフォ」のみ）
		/// </summary>
		/// <returns></returns>
		FixedPurchaseCancelReasonModel[] GetCancelReasonForPC();

		/// <summary>
		/// 解約理由取得（定期購入情報で利用されている全て）
		/// </summary>
		/// <returns>モデル列</returns>
		FixedPurchaseCancelReasonModel[] GetUsedCancelReasonAll();

		/// <summary>
		/// 解約理由登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		int InsertCancelReason(FixedPurchaseCancelReasonModel model, SqlAccessor accessor);

		/// <summary>
		/// 解約理由削除（全て）
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int DeleteCancelReasonAll(SqlAccessor accessor);

		/// <summary>
		/// 取得（表示及びメール送信用）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="isSendMail">メール送信用か</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		FixedPurchaseContainer GetContainer(string fixedPurchaseId, bool isSendMail = false, SqlAccessor accessor = null);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">定期モデル</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseStatus">定期購入ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void Register(
			FixedPurchaseModel model,
			string orderId,
			string fixedPurchaseStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseHistoryKbn">定期購入履歴区分</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		/// <param name="isUpdateFixedPurchaseItem">商品情報を更新するか</param>
		/// <returns>影響を受けた件数</returns>
		int Modify(
			string fixedPurchaseId,
			string fixedPurchaseHistoryKbn,
			Action<FixedPurchaseModel, FixedPurchaseHistoryModel> updateAction,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string externalPaymentCooperationLog = "",
			bool isUpdateFixedPurchaseItem = true);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseHistoryKbn">定期購入履歴区分</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		/// <param name="isUpdateFixedPurchaseItem">商品情報を更新するか</param>
		/// <returns>影響を受けた件数</returns>
		int Modify(
			string fixedPurchaseId,
			string fixedPurchaseHistoryKbn,
			Action<FixedPurchaseModel, FixedPurchaseHistoryModel> updateAction,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			string externalPaymentCooperationLog = "",
			bool isUpdateFixedPurchaseItem = true);

		/// <summary>
		/// 購入回数(注文基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderCount">購入回数(注文基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateOrderCount(
			string fixedPurchaseId,
			int orderCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 購入回数(出荷基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="shippedCount">購入回数(出荷基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateShippedCount(
			string fixedPurchaseId,
			int shippedCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 拡張ステータス更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="extendStatusNo">拡張ステータスNo</param>
		/// <param name="extendStatus">拡張ステータス</param>
		/// <param name="updateDate">拡張ステータス更新日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		int UpdateExtendStatus(
			string fixedPurchaseId,
			int extendStatusNo,
			string extendStatus,
			DateTime updateDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 拡張ステータス更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="extendStatusNo">拡張ステータスNo</param>
		/// <param name="extendStatus">拡張ステータス</param>
		/// <param name="updateDate">拡張ステータス更新日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		int UpdateExtendStatus(
			string fixedPurchaseId,
			int extendStatusNo,
			string extendStatus,
			DateTime updateDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 支払方法更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderPaymentKbn">支払区分</param>
		/// <param name="creditBranchNo">クレジットカード枝番</param>
		/// <param name="cardInstallmentsCode">カード支払い回数コード</param>
		/// <param name="externalPaymentAgreementId">外部支払い契約ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateOrderPayment(
			string fixedPurchaseId,
			string orderPaymentKbn,
			int? creditBranchNo,
			string cardInstallmentsCode,
			string externalPaymentAgreementId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 支払方法更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderPaymentKbn">支払区分</param>
		/// <param name="creditBranchNo">クレジットカード枝番</param>
		/// <param name="cardInstallmentsCode">カード支払い回数コード</param>
		/// <param name="externalPaymentAgreementId">外部支払い契約ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateOrderPayment(
			string fixedPurchaseId,
			string orderPaymentKbn,
			int? creditBranchNo,
			string cardInstallmentsCode,
			string externalPaymentAgreementId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 配送先更新
		/// </summary>
		/// <param name="shipping">定期配送先モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateShipping(
			FixedPurchaseShippingModel shipping,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 配送先更新
		/// </summary>
		/// <param name="shipping">定期配送先モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateShipping(
			FixedPurchaseShippingModel shipping,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 配送パターン更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="fixedPurchaseSetting1">定期購入設定１</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdatePattern(
			string fixedPurchaseId,
			string fixedPurchaseKbn,
			string fixedPurchaseSetting1,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 次回配送日/次々回配送日更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateShippingDate(
			string fixedPurchaseId,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 次回配送日/次々回配送日更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新成功したか</returns>
		bool UpdateShippingDate(
			string fixedPurchaseId,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 商品更新
		/// </summary>
		/// <param name="items">定期商品モデル列</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateItems(FixedPurchaseItemModel[] items, string lastChanged, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 商品更新(注文同梱用)
		/// </summary>
		/// <param name="items">定期商品モデル列</param>
		/// <param name="childFixedPurchaseIds">子定期購入ID</param>
		/// <param name="shippingMethodChangeToExpress">配送方法 宅配便への変更有無</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したかどうか</returns>
		bool UpdateItemsForOrderCombine(
			FixedPurchaseItemModel[] items,
			string childFixedPurchaseIds,
			bool shippingMethodChangeToExpress,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 次回購入の利用ポイントの更新を適用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="newUsePoint">更新後の次回購入利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したかどうか</returns>
		bool ApplyNextShippingUsePointChange(
			string deptId,
			FixedPurchaseContainer fixedPurchase,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 次回購入の利用ポイントの更新を適用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="newUsePoint">更新後の次回購入利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したかどうか</returns>
		bool ApplyNextShippingUsePointChange(
			string deptId,
			FixedPurchaseModel fixedPurchase,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入情報の解約（解約理由なし）と次回購入利用ポイントの戻し
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="isPointOptionOn">ポイントOPが有効かどうか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		bool CancelFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			string lastChanged,
			string deptId,
			bool isPointOptionOn,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 定期購入情報の解約（解約理由なし）と次回購入利用ポイントの戻し
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="isPointOptionOn">ポイントOPが有効かどうか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		bool CancelFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			string lastChanged,
			string deptId,
			bool isPointOptionOn,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入情報の解約（解約理由付き）と次回購入利用ポイントの戻し
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="cancelReasonId">解約理由ID</param>
		/// <param name="cancelMemo">解約メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="isPointOptionOn">ポイントOPが有効かどうか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		bool CancelFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			string cancelReasonId,
			string cancelMemo,
			string lastChanged,
			string deptId,
			bool isPointOptionOn,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 定期購入情報の解約（解約理由付き）と次回購入利用ポイントの戻し
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="cancelReasonId">解約理由ID</param>
		/// <param name="cancelMemo">解約メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="isPointOptionOn">ポイントOPが有効かどうか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		bool CancelFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			string cancelReasonId,
			string cancelMemo,
			string lastChanged,
			string deptId,
			bool isPointOptionOn,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入再開
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="nextShippingDate">次回配送日（更新しない場合null）</param>
		/// <param name="nextNextShippingDate">次々回配送日（更新しない場合null）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新成功したか</returns>
		bool Resume(
			string fixedPurchaseId,
			string userId,
			string lastChanged,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 定期購入再開
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="nextShippingDate">次回配送日（更新しない場合null）</param>
		/// <param name="nextNextShippingDate">次々回配送日（更新しない場合null）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新成功したか</returns>
		bool Resume(
			string fixedPurchaseId,
			string userId,
			string lastChanged,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入再開
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="nextShippingDate">次回配送日（更新しない場合null）</param>
		/// <param name="nextNextShippingDate">次々回配送日（更新しない場合null）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新成功したか</returns>
		bool ResumeInner(
			string fixedPurchaseId,
			string lastChanged,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入再開
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="nextShippingDate">次回配送日（更新しない場合null）</param>
		/// <param name="nextNextShippingDate">次々回配送日（更新しない場合null）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新成功したか</returns>
		bool Resume(
			string fixedPurchaseId,
			string lastChanged,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入ステータスを仮登録から通常に更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateFixedPurchaseStatusTempToNormal(
			string orderId,
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 定期購入ステータスを仮登録から通常に更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateFixedPurchaseStatusTempToNormal(
			string orderId,
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 仮登録の定期購入をキャンセルする
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="orderId">注文ID</param>
		void CancelTemporaryRegistrationFixedPurchase(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string orderId = null);

		/// <summary>
		/// 仮登録の定期購入をキャンセルする
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="orderId">注文ID</param>
		void CancelTemporaryRegistrationFixedPurchase(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			string orderId = null);

		/// <summary>
		/// 仮登録ステータスか確認する
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		bool CheckTemporaryRegistration(string fixedPurchaseId, SqlAccessor accessor = null);

		/// <summary>
		/// スキップ
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="shopShipping">配送種別情報</param>
		/// <param name="calculationMode">次回配送日計算モード</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void SkipOrder(
			string fixedPurchaseId,
			string lastChanged,
			ShopShippingModel shopShipping,
			NextShippingCalculationMode calculationMode,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 管理メモ更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseManagementMemo">管理メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <remarks>履歴登録なし</remarks>
		void UpdateFixedPurchaseManagementMemo(
			string fixedPurchaseId,
			string fixedPurchaseManagementMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 配送メモ更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="shippingMemo">配送メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateShippingMemo(
			string fixedPurchaseId,
			string shippingMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 解約理由更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="cancelReasonId">解約理由区分ID</param>
		/// <param name="cancelMemo">解約メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateFixedPurchaseCancelReason(
			string fixedPurchaseId,
			string cancelReasonId,
			string cancelMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// クレジットカード決済与信成功更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="creditBranchNo">クレジットカード枝番</param>
		/// <param name="cardInstallmentsCode">カード支払い回数コード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		void UpdateForAuthSuccess(
			string fixedPurchaseId,
			int creditBranchNo,
			string cardInstallmentsCode,
			string lastChanged,
			string externalPaymentCooperationLog,
			UpdateHistoryAction updateHistoryAction
		);

		/// <summary>
		/// クレジットカード登録失敗向け更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		void UpdateForCreditRegisterFail(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string externalPaymentCooperationLog = "");

		/// <summary>
		/// 仮クレジットカード登録向け更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateForProvisionalCreditCardRegisterd(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 無効に更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <remarks>履歴登録なし</remarks>
		void UpdateInvalidate(string fixedPurchaseId, string lastChanged, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 初回注文登録成功更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateForFirstSuccessOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文登録成功更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateForSuccessOrder(
			string fixedPurchaseId,
			DateTime nextShippingDate,
			DateTime? nextNextShippingDate,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文登録失敗更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateForFailedOrder(string fixedPurchaseId, string lastChanged, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 決済エラー停止
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		void UpdateForFailedPayment(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string externalPaymentCooperationLog = "");

		/// <summary>
		/// 在庫切れ停止
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateForFailedNoStock(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 配送不可エリアエラー停止
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateForFailedUnavailableShippingArea(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文キャンセル更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="doHistoryCheck">購入履歴チェック必要かどうか</param>
		void UpdateForCancelOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool doHistoryCheck = true);

		/// <summary>
		/// 注文同梱に伴う注文キャンセル更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="doHistoryCheck">購入履歴チェック必要かどうか</param>
		void UpdateForCancelCombinedOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool doHistoryCheck = true);

		/// <summary>
		/// 注文出荷完了更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="doHistoryCheck">購入履歴チェック必要かどうか</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateForShippedOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool doHistoryCheck = true);

		/// <summary>
		/// 注文返品更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateForReturnOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入情報に利用ポイント数の更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="nextShipingUsePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		bool UpdateNextShippingUsePointToFixedPurchase(
			string fixedPurchaseId,
			string nextShipingUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入情報の休止
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="resumeDate">定期再開予定日</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="suspendReason">休止理由</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		bool SuspendFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			DateTime? resumeDate,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string suspendReason,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 定期購入情報の休止
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="resumeDate">定期再開予定日</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="suspendReason">休止理由</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		bool SuspendFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			DateTime? resumeDate,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string suspendReason,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 休止理由更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="resumeDate">定期再開予定日</param>
		/// <param name="suspendReason">休止理由</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateFixedPurchaseSuspendReason(
			string fixedPurchaseId,
			DateTime? resumeDate,
			string suspendReason,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// Update Fixed Purchase Memo
		/// </summary>
		/// <param name="fixedPurchaseId">Fixed Purchase Id</param>
		/// <param name="fixedPurchaseMemo">Fixed Purchase Memo</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		void UpdateFixedPurchaseMemo(
			string fixedPurchaseId,
			string fixedPurchaseMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 検索ヒット件数取得（定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <param name="replaces">クエリ置換内容</param>
		/// <returns>モデル</returns>
		int GetCountOfSearchFixedPurchase(FixedPurchaseListSearchCondition searchCondition, KeyValuePair<string, string>[] replaces = null);

		/// <summary>
		/// 検索（定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <param name="replaces">クエリ置換内容</param>
		/// <returns>モデル列</returns>
		FixedPurchaseListSearchResult[] SearchFixedPurchase(FixedPurchaseListSearchCondition searchCondition, KeyValuePair<string, string>[] replaces = null);

		/// <summary>
		/// 検索ヒット件数取得（定期購入履歴一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>モデル</returns>
		int GetCountOfSearchFixedPurchaseHistory(FixedPurchaseHistoryListSearchCondition searchCondition);

		/// <summary>
		/// 検索（定期購入履歴一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		FixedPurchaseHistoryListSearchResult[] SearchFixedPurchaseHistory(FixedPurchaseHistoryListSearchCondition searchCondition, SqlAccessor accessor = null);

		/// <summary>
		/// 検索ヒット件数取得（ユーザ定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>件数</returns>
		int GetCountOfSearchUserFixedPurchase(UserFixedPurchaseListSearchCondition searchCondition);

		/// <summary>
		/// 検索（ユーザ定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>モデル列</returns>
		UserFixedPurchaseListSearchResult[] SearchUserFixedPurchase(UserFixedPurchaseListSearchCondition searchCondition);

		/// <summary>
		/// 検索ヒット件数取得（ユーザ定期購入一覧）注文同梱でのキャンセル除く
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>件数</returns>
		int GetCountOfSearchUserFixedPurchaseExcludeOrderCombineCancel(UserFixedPurchaseListSearchCondition searchCondition);

		/// <summary>
		/// 検索（ユーザ定期購入一覧）注文同梱でのキャンセル除く
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>モデル列</returns>
		UserFixedPurchaseListSearchResult[] SearchUserFixedPurchaseExcludeOrderCombineCancel(UserFixedPurchaseListSearchCondition searchCondition);

		/// <summary>
		/// 配送希望日と配送所要日数を元に、初回配送予定日を計算します。
		/// </summary>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="daysRequired">配送所要日数</param>
		/// <returns>初回配送予定日</returns>
		DateTime CalculateFirstShippingDate(DateTime? shippingDate, int daysRequired);

		/// <summary>
		/// 指定された定期購入配送パターンを元に、次回配送日を計算します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="daysRequired">配送所要日数</param>
		/// <param name="calculationMode">次回配送日計算モード</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <returns>次回配送日</returns>
		DateTime CalculateNextShippingDate(string fpKbn, string fpSetting, DateTime? shippingDate, int daysRequired, int minSpan, NextShippingCalculationMode calculationMode);

		/// <summary>
		/// 指定された定期購入配送パターンを元に、次々回配送日を計算します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="daysRequired">配送所要日数</param>
		/// <param name="calculationMode">次回配送日計算モード</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <returns>次々回配送日</returns>
		DateTime CalculateNextNextShippingDate(string fpKbn, string fpSetting, DateTime? shippingDate, int daysRequired, int minSpan, NextShippingCalculationMode calculationMode);

		/// <summary>
		/// 指定された基準日を起点とし、次サイクルの配送日を計算して返却します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="calculationMode">配送日計算モード</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <returns>次サイクルの配送日</returns>
		DateTime CalculateFollowingShippingDate(string fpKbn, string fpSetting, DateTime baseDate, int minSpan, NextShippingCalculationMode calculationMode);

		/// <summary>
		/// Calculate first shipping date
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="calculationMode">配送日計算モード</param>
		/// <returns>次サイクルの配送日</returns>
		DateTime CalculateFirstShippingDate(
			string fpKbn,
			string fpSetting,
			DateTime baseDate,
			int minSpan,
			NextShippingCalculationMode calculationMode);

		/// <summary>
		/// 最終購入注文の配送日を元にして、キャンセル可能な最短の次回配送日を計算して返却します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="lastShippedDate">最終購入注文の配送日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="daysCancel">何日前までキャンセル可能か</param>
		/// <returns>次回配送日</returns>
		DateTime CalculateNextShippingDateFromLastShippedDate(string fpKbn, string fpSetting, DateTime lastShippedDate, int minSpan, int daysCancel);

		/// <summary>
		/// Calculate first shipping date option 2
		/// </summary>
		/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
		/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
		/// <param name="firstShippingDate">First shipping date</param>
		/// <param name="minSpan">Minimum shipping span</param>
		/// <param name="calculationMode">Calculation mode</param>
		/// <returns>A first shipping date option 2</returns>
		DateTime CalculateFirstShippingDateOption2(
			string fixedPurchaseKbn,
			string fixedPurchaseSetting,
			DateTime firstShippingDate,
			int minSpan,
			NextShippingCalculationMode calculationMode);

		/// <summary>
		/// 生きている定期購入情報が存在するかの判定
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true：存在する、false：存在しない</returns>
		bool HasActiveFixedPurchaseInfo(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 生きている定期購入情報が存在するかの判定
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="exclusionFixedPurchaseIds">除外の定期購入IDリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true：存在する、false：存在しない</returns>
		bool HasActiveFixedPurchaseInfo(string userId, List<string> exclusionFixedPurchaseIds, SqlAccessor accessor = null);

		/// <summary>
		/// 定期購入メール一時ログ検索
		/// </summary>
		/// <param name="actionMasterId">スケジュール実行ID</param>
		/// <returns>送信メール一覧</returns>
		FixedPurchaseBatchMailTmpLogModel[] SearchFixedPurchaseBatchMailTmpLogs(string actionMasterId);

		/// <summary>
		/// 定期購入メール一時ログ登録
		/// </summary>
		/// <param name="model">定期購入メール一時ログモデル</param>
		void InsertFixedPurchaseBatchMailTmpLog(FixedPurchaseBatchMailTmpLogModel model);

		/// <summary>
		/// 定期購入メール一時ログ削除
		/// </summary>
		/// <param name="tmpLogId">削除対象のtmp_log_id</param>
		int DeleteFixedPurchaseBatchMailTmpLog(int tmpLogId);

		/// <summary>
		/// 定期購入 注文同梱履歴登録
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="isCountOrder">Is count order</param>
		/// <returns>履歴登録成否 登録成功：TRUE、登録失敗：FALSE</returns>
		bool RegistHistoryForOrderCombine(
			string fixedPurchaseId,
			string orderId,
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool isCountOrder = false);

		/// <summary>
		/// 定期購入同梱可能な親定期購入情報取得
		/// </summary>
		/// <param name="allowCombineFixedPurchaseStatus">定期購入同梱可能な定期購入ステータス</param>
		/// <param name="allowCombineFixedPurchasePaymentStatus">定期購入同梱可能な決済ステータス</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー氏名</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <returns>モデル</returns>
		FixedPurchaseModel[] GetCombinableParentFixedPurchaseWithCondition(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombineFixedPurchasePaymentStatus,
			string userId,
			string userName,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			int startRowNum,
			int endRowNum);

		/// <summary>
		/// 定期購入同梱可能な親定期購入件数取得
		/// </summary>
		/// <param name="allowCombineFixedPurchaseStatus">定期購入同梱可能な定期購入ステータス</param>
		/// <param name="allowCombineFixedPurchasePaymentStatus">定期購入同梱可能な決済ステータス</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー氏名</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <returns>件数</returns>
		int GetCombinableParentFixedPurchaseWithConditionCount(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombineFixedPurchasePaymentStatus,
			string userId,
			string userName,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo);

		/// <summary>
		/// 定期購入同梱可能な定期購入情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="allowCombineFixedPurchaseStatus">定期購入同梱可能な定期購入ステータス</param>
		/// <param name="allowCombinePaymentStatus">定期購入同梱可能な決済ステータス</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <param name="parentPaymentKbn">親注文の支払区分</param>
		/// <returns>モデル</returns>
		FixedPurchaseModel[] GetCombinableFixedPurchase(
			string shopId,
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombinePaymentStatus,
			string userId,
			string shippingType,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			string parentPaymentKbn);

		/// <summary>
		/// 定期購入同梱可能な定期購入件数取得
		/// </summary>
		/// <param name="allowCombineFixedPurchaseStatus">定期購入同梱可能な定期購入ステータス</param>
		/// <param name="allowCombinePaymentStatus">定期購入同梱可能な決済ステータス</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <param name="parentPaymentKbn">親注文の支払区分</param>
		/// <returns>件数</returns>
		int GetCombinableFixedPurchaseCount(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombinePaymentStatus,
			string userId,
			string shippingType,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			string parentPaymentKbn);

		/// <summary>
		/// 仮登録の定期台帳と履歴を削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		bool DeletePrefixedPurchaseAndHistory(string fixedPurchaseId, SqlAccessor accessor = null);

		/// <summary>
		/// 配送会社、配送種別、配送方法に紐づく定期台帳の商品があるか判定
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingMethod">配送方法</param>
		/// <returns>有無</returns>
		bool CheckDeliveryCompanyFixedPurchaseItems(string deliveryCompanyId, string shippingId, string shippingMethod);

		/// <summary>
		/// 再開対象の定期購入取得
		/// </summary>
		/// <returns>再開対象の定期購入情報</returns>
		FixedPurchaseModel[] GetTargetsForResume();

		/// <summary>
		/// 外部決済連携ログ取得(定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログの取得)
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログ</returns>
		FixedPurchaseHistoryModel[] GetFixedPurchaseHistory(
			string fixedPurchaseId,
			SqlAccessor accessor = null);

		/// <summary>
		/// 外部決済連携ログ取得(定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログの取得)
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseHistoryNo">定期購入注文履歴NO</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログ</returns>
		string GetDetailExternalPaymentCooperationLog(
			string fixedPurchaseId,
			string fixedPurchaseHistoryNo,
			SqlAccessor accessor = null);

		/// <summary>
		/// 定期購入IDとクーポン利用ユーザー(メールアドレスorユーザーID)から定期購入情報が取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="usedUserJudgeType">利用済みユーザー判定方法</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期購入情報</returns>
		FixedPurchaseModel GetFixedPurchaseByFixedPurchaseIdAndCouponUseUser(
			string fixedPurchaseId,
			string couponUseUser,
			string usedUserJudgeType,
			SqlAccessor accessor = null);

		/// <summary>
		/// Clear Skipped Count
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int ClearSkippedCount(
			string fixedPurchaseId,
			SqlAccessor accessor = null);

		/// <summary>
		/// Update SubScriptionBox Order Count
		/// </summary>
		/// <param name="fixedPurchaseId"> Fixed pruchase id</param>
		/// <param name="subscriptionBoxOrderCount">Ordering number of times with SubScription Box</param>
		/// <param name="updateHistoryAction">履歴更新アクション</param>
		/// <param name="accessor">アクセサー</param>
		/// <returns>Number of updated rows</returns>
		int UpdateSubScriptionBoxOrderCount(
			string fixedPurchaseId,
			int subscriptionBoxOrderCount,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);
			
		/// <summary>
		/// GetSubcriptionBoxId from Order
		/// </summary>
		/// <param name="fixedPurchaseId"></param>
		/// <returns></returns>
		string GetSubcriptionBoxId(string fixedPurchaseId);
		
		/// <summary>
		/// 外部支払契約ID設定
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="externalPaymentAgreementId">外部支払契約ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="fixedPurchaseHistoryKbn">Fixed purchase type</param>
		/// <returns></returns>
		int SetExternalPaymentAgreementId(
			string fixedPurchaseId,
			string externalPaymentAgreementId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null,
			string fixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_AUTH_SUCCESS);

		/// <summary>
		/// 注文登録成功更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="fixedPurchaseItemModel">定期購入商品情報モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateForSuccessOrder(
			string fixedPurchaseId,
			DateTime nextShippingDate,
			DateTime? nextNextShippingDate,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			FixedPurchaseItemModel[] fixedPurchaseItemModel,
			SqlAccessor accessor);

		/// <summary>
		/// 商品購入回数(注文基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="itemOrderCount">商品購入回数(注文基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateItemOrderCount(
			string fixedPurchaseId,
			string variationId,
			int itemOrderCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 商品購入回数(出荷基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="itemShippedCount">商品購入回数(出荷基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateItemShippedCount(
			string fixedPurchaseId,
			string variationId,
			int itemShippedCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 注文時の商品購入回数(注文基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="itemOrderCount">商品購入回数(注文基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void UpdateItemOrderCountWhenOrdering(
			string fixedPurchaseId,
			string variationId,
			int itemOrderCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 注文返品更新(商品単位)
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="isChangeStatusOrderFromComplete">Is change status order from complete</param>
		void UpdateForReturnOrderItem(
			string fixedPurchaseId,
			string orderId,
			string variationId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool isChangeStatusOrderFromComplete = false);

		/// <summary>
		/// 定期台帳商品取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		FixedPurchaseItemModel[] GetAllItem(string fixedPurchaseId, SqlAccessor accessor = null);

		/// <summary>
		/// Get order count by fixed purchase workflow setting
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Order count</returns>
		int GetOrderCountByFixedPurchaseWorkflowSetting(Hashtable searchParam);

		/// <summary>
		/// Get fixed purchase workflow list no pagination
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Dataview of fixed purchase list</returns>
		DataView GetFixedPurchaseWorkflowListNoPagination(Hashtable searchParam);

		/// <summary>
		/// Get fixed purchase workflow list
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <param name="pageNumber">Pager number</param>
		/// <returns>Dataview of fixed purchase list</returns>
		DataView GetFixedPurchaseWorkflowList(Hashtable searchParam, int pageNumber);

		/// <summary>
		/// 次回配送日計算モードを取得
		/// </summary>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="defaultCalculationMode">デフォルト計算モード（Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE）</param>
		/// <returns>次回配送日計算モード</returns>
		NextShippingCalculationMode GetCalculationMode(
			string fixedPurchaseKbn,
			NextShippingCalculationMode defaultCalculationMode);
	}
}
