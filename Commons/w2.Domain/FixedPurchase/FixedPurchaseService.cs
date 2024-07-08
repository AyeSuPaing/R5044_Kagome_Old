/*
=========================================================================================================
  Module      : 定期購入情報サービス (FixedPurchaseService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.Order;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.Point;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.TwFixedPurchaseInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入情報サービス
	/// </summary>
	public class FixedPurchaseService : ServiceBase, IFixedPurchaseService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public FixedPurchaseModel Get(string fixedPurchaseId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var model = repository.Get(fixedPurchaseId);
				if (model == null) return null;

				// 配送先&商品をセット
				this.SetChildModel(model, repository);

				return model;
			}
		}
		#endregion

		#region +GetUpdLock 更新ロック取得
		/// <summary>
		/// 更新ロック取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期購入ID</returns>
		public string GetUpdLock(string fixedPurchaseId, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var result = repository.GetUpdLock(fixedPurchaseId);
				return result;
			}
		}
		#endregion

		#region +GetFixedPurchasesByUserId ユーザーIDから定期購入情報取得
		/// <summary>
		/// ユーザーIDから定期購入情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデルリスト</returns>
		public FixedPurchaseModel[] GetFixedPurchasesByUserId(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var models = repository.GetFixedPurchasesByUserId(userId);
				foreach (var model in models)
				{
					// 配送先&商品をセット
					this.SetChildModel(model, repository);
				}
				return models;
			}
		}
		#endregion

		#region +GetFixedPurchasesByProductId 商品IDから定期購入情報取得
		/// <summary>
		/// 商品IDから定期購入情報取得
		/// </summary>
		/// <param name="productId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデルリスト</returns>
		public FixedPurchaseModel[] GetFixedPurchasesByProductId(string productId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var models = repository.GetFixedPurchasesByProductId(productId);
				foreach (var model in models)
				{
					// 配送先&商品をセット
					SetChildModel(model, repository);
				}
				return models;
			}
		}
		#endregion

		#region +GetTargetsForCreateOrder 注文対象の定期購入取得
		/// <summary>
		/// 注文対象の定期購入取得
		/// </summary>
		/// <returns>モデル列</returns>
		public FixedPurchaseModel[] GetTargetsForCreateOrder()
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var models = repository.GetTargetsForCreateOrder();
				var targets = models.Where(f => f.OrderPaymentKbn != Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID).ToArray();
				foreach (var model in targets)
				{
					// 配送先&商品をセット
					this.SetChildModel(model, repository);
				}
				return targets;
			}
		}
		#endregion

		#region +GetOrdersForLine
		/// <summary>
		/// 定期台帳取得_LINE連携
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="offset">開始位置</param>
		/// <param name="limit">最大件数</param>
		/// <param name="updateAt">取得時間範囲</param>
		/// <returns>定期データリスト</returns>
		public FixedPurchaseModel[] GetFixedPurchasesForLine(
			string userId,
			int offset,
			int limit,
			DateTime updateAt)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var result = repository.GetFixedPurchasesForLine(
					userId,
					offset,
					limit,
					updateAt);
				return result;
			}
		}
		#endregion

		#region +GetContainerWorking 稼働している定期情報すべて取得（日次出荷予測レポート用）
		/// <summary>
		/// 稼働している定期情報すべて取得（日次出荷予測レポート用）
		/// </summary>
		/// <returns>定期データリスト</returns>
		public DataView GetContainerWorking()
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var dv = repository.GetContainerWorking();
				return dv;
			}
		}
		#endregion

		#region 変更期限案内メール送信対象の定期購入取得(全件）
		/// <summary>
		/// 変更期限案内メール送信対象の定期購入取得(全件）
		/// </summary>
		/// <returns>モデル列</returns>
		public FixedPurchaseModel[] GetTargetsForSendChangeDeadlineMail()
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var models = repository.GetTargetsForSendChangeDeadlineMail();
				var targets = models.Where(f => f.OrderPaymentKbn != Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID).ToArray();
				foreach (var model in targets)
				{
					// 配送先と商品をセットする
					this.SetChildModel(model, repository);
				}
				return targets;
			}
		}
		#endregion

		#region +定期売上集計対象の定期取得
		/// <summary>
		/// 定期売上集計対象の定期取得(前回実行日からの差分取得)
		/// </summary>
		/// <param name="lastExecDateTime">最終実行日</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期一覧</returns>
		public FixedPurchaseModel[] GetTargetsForForecastAggregate(DateTime lastExecDateTime, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var models = repository.GetTargetsForForecastAggregate(lastExecDateTime);
				foreach (var model in models)
				{
					SetChildModel(model, repository, true);
				}
				return models;
			}
		}
		/// <summary>
		/// 定期売上集計対象の定期取得(全件)
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期一覧</returns>
		public FixedPurchaseModel[] GetTargetsForForecastAggregate(SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var models = repository.GetTargetsForForecastAggregate();
				foreach (var model in models)
				{
					SetChildModel(model, repository, true);
				}
				return models;
			}
		}
		#endregion

		#region +SetChildModel 子モデルをセット(配送先・商品)
		/// <summary>
		/// 子モデルをセット(配送先・商品)
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="repository">リポジトリ</param>
		private void SetChildModel(FixedPurchaseModel model, FixedPurchaseRepository repository)
		{
			SetChildModel(model, repository, false);
		}

		/// <summary>
		/// 子モデルをセット(配送先・商品)
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="repository">リポジトリ</param>
		/// <param name="isForFixedPurchaseForecast">定期売上予測による実行かどうか</param>
		private void SetChildModel(FixedPurchaseModel model, FixedPurchaseRepository repository, bool isForFixedPurchaseForecast)
		{
			// 配送先
			model.Shippings = repository.GetShippingAll(model.FixedPurchaseId);
			// 商品
			var items = repository.GetItemAll(model.FixedPurchaseId);
			if (items == null) return;

			foreach (var shipping in model.Shippings)
			{
				var itemsMatchingFixedPurchaseShippingNo = items
					.Where(item => shipping.MatchesFixedPurchaseShippingNo(item.FixedPurchaseShippingNo))
					.ToArray();

				// 定期売上予測による実行の場合、同一商品ID、バリエーションIDで付帯情報だけが異なる商品を同一商品として扱う。
				// e.g. 以下の場合、以下の1種類の商品が5個購入されるとみなす。
				// 商品ID: A, 商品バリエーションID: X, 付帯情報: L, 個数: 2
				// 商品ID: A, 商品バリエーションID: X, 付帯情報: M, 個数: 3
				shipping.Items = (isForFixedPurchaseForecast == false)
					? itemsMatchingFixedPurchaseShippingNo
					: itemsMatchingFixedPurchaseShippingNo
						.GroupBy(item => item.VariationId)
						.Select(
							group =>
							{
								var firstGroup = group.First();
								firstGroup.ItemQuantity = group.Sum(item => item.ItemQuantity);
								firstGroup.ItemQuantitySingle = group.Sum(item => item.ItemQuantitySingle);
								return firstGroup;
							})
						.ToArray();
			}

			// 次回購入利用クーポン情報を取得
			if (string.IsNullOrEmpty(model.NextShippingUseCouponId) == false)
			{
				model.NextShippingUseCouponDetail = new CouponService().GetAllUserCouponsFromCouponId(
					model.ShopId,
					model.UserId,
					model.NextShippingUseCouponId,
					model.NextShippingUseCouponNo).FirstOrDefault();
			}

			// Get Taiwan Fixed Purchase Invoice
			model.Invoice = new TwFixedPurchaseInvoiceService().GetTaiwanFixedPurchaseInvoice(
				model.FixedPurchaseId,
				model.Shippings[0].FixedPurchaseShippingNo,
				repository.Accessor);
		}
		#endregion

		#region -Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="historyModel">履歴モデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void Insert(
			FixedPurchaseModel model,
			FixedPurchaseHistoryModel historyModel,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				// 登録
				repository.Insert(model);
				foreach (var shipping in model.Shippings)
				{
					// 配送先
					repository.InsertShipping(shipping);
					foreach (var item in shipping.Items)
					{
						// 商品
						repository.InsertItem(item);

						// 定期継続分析
						var repeatAnalysisModel = new FixedPurchaseRepeatAnalysisModel
						{
							UserId = model.UserId,
							ProductId = item.ProductId,
							VariationId = item.VariationId,
							Count = 0,
							OrderId = "",
							FixedPurchaseId = item.FixedPurchaseId,
							Status = Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_EXISTS,
							LastChanged = model.LastChanged,
						};
						new FixedPurchaseRepeatAnalysisService().Insert(repeatAnalysisModel, accessor);
					}
				}

				// 履歴あり?
				if (historyModel != null)
				{
					// 定期購入注文履歴No取得
					historyModel.FixedPurchaseHistoryNo = repository.GetHistoryCount(model.FixedPurchaseId) + 1;
					// 履歴登録
					repository.InsertHistory(historyModel);
				}

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForFixedPurchase(model.FixedPurchaseId, model.LastChanged, accessor);
				}
			}
		}
		#endregion

		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="historyModel">履歴モデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="isUpdateFixedPurchaseItem">商品情報を更新するか</param>
		/// <returns>影響を受けた件数</returns>
		private int Update(
			FixedPurchaseModel model,
			FixedPurchaseHistoryModel historyModel,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool isUpdateFixedPurchaseItem = true)
		{
			var isFixedPurchaseRepeatAnalysisRegister = ((historyModel != null)
				&& ((historyModel.FixedPurchaseHistoryKbn == Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_TEMP)
					|| (historyModel.FixedPurchaseHistoryKbn == Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_RESUME)
					|| (historyModel.FixedPurchaseHistoryKbn == Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ITEMUPDATE)
					|| (historyModel.FixedPurchaseHistoryKbn == Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ITEMUPDATE_FOR_COMBINE)));
			var result = 0;
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				// 更新
				result = repository.Update(model);
				foreach (var shipping in model.Shippings)
				{
					// 配送先
					repository.UpdateShipping(shipping);
					// 商品（DELETE => INSERT）
					if(isUpdateFixedPurchaseItem) repository.DeleteItemsAll(shipping.FixedPurchaseId);

					if (isFixedPurchaseRepeatAnalysisRegister)
					{
						new FixedPurchaseRepeatAnalysisService()
							.FallOutFixedPurchaseAllItem(
								model.UserId,
								shipping.FixedPurchaseId,
								model.LastChanged,
								accessor);
					}

					foreach (var item in shipping.Items)
					{
						// 商品
						if (isUpdateFixedPurchaseItem)
						{
							item.DateCreated = model.DateCreated;
							repository.InsertItemForUpdate(item);
						}

						if (isFixedPurchaseRepeatAnalysisRegister)
						{
							new FixedPurchaseRepeatAnalysisService()
								.RegistFixedpurchaseItem(
									model.UserId,
									item.ProductId,
									item.VariationId,
									shipping.FixedPurchaseId,
									model.LastChanged,
									accessor);
						}
					}
				}

				// 履歴あり?
				if (historyModel != null)
				{
					// 定期購入注文履歴No取得
					historyModel.FixedPurchaseHistoryNo = repository.GetHistoryCount(model.FixedPurchaseId) + 1;
					// 履歴登録
					repository.InsertHistory(historyModel);
				}

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForFixedPurchase(model.FixedPurchaseId, model.LastChanged, accessor);
				}
			}
			return result;
		}
		#endregion

		#region +UpdatePaymentStatus 決済ステータス更新
		/// <summary>
		/// 決済ステータス更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="paymentStatus">決済ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdatePaymentStatus(
			string fixedPurchaseId,
			string paymentStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdatePaymentStatus(fixedPurchaseId, paymentStatus, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +UpdatePaymentStatus 決済ステータス更新
		/// <summary>
		/// 決済ステータス更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="paymentStatus">決済ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdatePaymentStatus(
			string fixedPurchaseId,
			string paymentStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				fixedPurchaseId,
				"",
				(model, historyModel) =>
				{
					model.PaymentStatus = paymentStatus;
					model.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return updated;
		}
		#endregion

		#region +UpdateUserIdForIntegration ユーザーID更新（ユーザー統合向け）
		/// <summary>
		/// ユーザーID更新（ユーザー統合向け）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public void UpdateUserIdForIntegration(
			string fixedPurchaseId,
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			UpdateUserId(fixedPurchaseId, userId, lastChanged, updateHistoryAction, accessor);
		}
		#endregion
		#region -UpdateUserId ユーザーID更新
		/// <summary>
		/// ユーザーID更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		private void UpdateUserId(
			string fixedPurchaseId,
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			Modify(
				fixedPurchaseId,
				"",
				(model, historyModel) =>
				{
					model.UserId = userId;
					model.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateNextShippingUseCoupon 次回購入の利用クーポン更新
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
		public bool UpdateNextShippingUseCoupon(
			string fixedPurchaseId,
			string couponId,
			int? couponNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var result = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_NEXTSHIPPINGUSECOUPON_UPDATE,
				(model, historyModel) =>
				{
					model.NextShippingUseCouponId = couponId;
					model.NextShippingUseCouponNo = couponNo ?? FixedPurchaseModel.DEFAULT_NEXT_SHIPPING_USE_COUPON_NO;
					model.LastChanged = lastChanged;
					historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return (result > 0);
		}
		#endregion

		#region ResetNextShippingUseCoupon 次回購入の利用クーポンリセット
		/// <summary>
		/// 次回購入の利用クーポンリセット
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新結果</returns>
		public bool ResetNextShippingUseCoupon(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var result = UpdateNextShippingUseCoupon(
				fixedPurchaseId,
				FixedPurchaseModel.DEFAULT_NEXT_SHIPPING_USE_COUPON_ID,
				FixedPurchaseModel.DEFAULT_NEXT_SHIPPING_USE_COUPON_NO,
				lastChanged,
				updateHistoryAction,
				accessor);
			return result;
		}
		#endregion

		#region +UpdateReceiptInfo 領収書情報更新
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
		public int UpdateReceiptInfo(
			string fixedPurchaseId,
			string receiptFlg,
			string receiptAddress,
			string receiptProviso,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateReceiptInfo(
					fixedPurchaseId,
					receiptFlg,
					receiptAddress,
					receiptProviso,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +UpdateReceiptInfo 領収書情報更新
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
		public int UpdateReceiptInfo(
			string fixedPurchaseId,
			string receiptFlg,
			string receiptAddress,
			string receiptProviso,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_RECEIPT_UPDATE,
				(model, historyModel) =>
				{
					model.ReceiptFlg = receiptFlg;
					model.ReceiptAddress = receiptAddress;
					model.ReceiptProviso = receiptProviso;
					model.LastChanged = lastChanged;
					historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return updated;
		}
		#endregion

		#region +解約理由関連
		/// <summary>
		/// 解約理由取得
		/// </summary>
		/// <param name="cancelReasonId">解約理由区分ID</param>
		/// <returns>モデル</returns>
		public FixedPurchaseCancelReasonModel GetCancelReason(string cancelReasonId)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				return repository.GetCancelReason(cancelReasonId);
			}
		}

		/// <summary>
		/// 解約理由取得（全て）
		/// </summary>
		/// <returns>モデル列</returns>
		public FixedPurchaseCancelReasonModel[] GetCancelReasonAll()
		{
			using (var repository = new FixedPurchaseRepository())
			{
				return repository.GetCancelReasonAll();
			}
		}

		/// <summary>
		/// 解約理由取得（表示範囲が「EC管理」のみ）
		/// </summary>
		/// <returns></returns>
		public FixedPurchaseCancelReasonModel[] GetCancelReasonForEC()
		{
			return this.GetCancelReasonAll().Where(cr => cr.IsValidDisplayKbnEc).ToArray();
		}

		/// <summary>
		/// 解約理由取得（表示範囲が「PC/スマフォ」のみ）
		/// </summary>
		/// <returns></returns>
		public FixedPurchaseCancelReasonModel[] GetCancelReasonForPC()
		{
			return this.GetCancelReasonAll().Where(cr => cr.IsValidDisplayKbnPc).ToArray();
		}

		/// <summary>
		/// 解約理由取得（定期購入情報で利用されている全て）
		/// </summary>
		/// <returns>モデル列</returns>
		public FixedPurchaseCancelReasonModel[] GetUsedCancelReasonAll()
		{
			using (var repository = new FixedPurchaseRepository())
			{
				return repository.GetUsedCancelReasonAll();
			}
		}

		/// <summary>
		/// 解約理由登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int InsertCancelReason(FixedPurchaseCancelReasonModel model, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				return repository.InsertCancelReason(model);
			}
		}

		/// <summary>
		/// 解約理由削除（全て）
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteCancelReasonAll(SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				return repository.DeleteCancelReasonAll();
			}
		}
		#endregion

		#region +GetContainer 取得（表示及びメール送信用）
		/// <summary>
		/// 取得（表示及びメール送信用）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="isSendMail">メール送信用か</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public FixedPurchaseContainer GetContainer(string fixedPurchaseId, bool isSendMail = false, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var container = repository.GetContainer(fixedPurchaseId, isSendMail);

				// 次回購入利用クーポン情報を取得
				if ((container != null) && (string.IsNullOrEmpty(container.NextShippingUseCouponId) == false))
				{
					container.NextShippingUseCouponDetail = new CouponService().GetAllUserCouponsFromCouponId(
						container.ShopId,
						container.UserId,
						container.NextShippingUseCouponId,
						container.NextShippingUseCouponNo).FirstOrDefault();
				}

				return container;
			}
		}
		#endregion

		#region 定期購入操作関連

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">定期モデル</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseStatus">定期購入ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Register(
			FixedPurchaseModel model,
			string orderId,
			string fixedPurchaseStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 最終更新者をセット
			model.LastChanged = lastChanged;
			// 定期購入ステータスをセット
			model.FixedPurchaseStatus = fixedPurchaseStatus;

			// ステータスが通常の場合更新区分をを成功にセット
			var fixedPurchaseHistoryKbn = (fixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL)
				? Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS
				: Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_TEMP;

			// 履歴モデル作成
			var historyModel = CreateHistoryModel(
				model,
				fixedPurchaseHistoryKbn);
			historyModel.OrderId = orderId;
			historyModel.UpdateOrderCount = 1;
			historyModel.UpdateOrderCountResult = 1;

			// 登録
			Insert(model, historyModel, updateHistoryAction, accessor);
		}
		#endregion

		#region +Modify 更新
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
		public int Modify(
			string fixedPurchaseId,
			string fixedPurchaseHistoryKbn,
			Action<FixedPurchaseModel, FixedPurchaseHistoryModel> updateAction,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string externalPaymentCooperationLog = "",
			bool isUpdateFixedPurchaseItem = true)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Modify(
					fixedPurchaseId,
					fixedPurchaseHistoryKbn,
					updateAction,
					lastChanged,
					updateHistoryAction,
					accessor,
					externalPaymentCooperationLog,
					isUpdateFixedPurchaseItem);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +Modify 更新
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
		public int Modify(
			string fixedPurchaseId,
			string fixedPurchaseHistoryKbn,
			Action<FixedPurchaseModel, FixedPurchaseHistoryModel> updateAction,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			string externalPaymentCooperationLog = "",
			bool isUpdateFixedPurchaseItem = true)
		{
			// 最新データ取得
			var model = this.Get(fixedPurchaseId, accessor);

			// 履歴モデル作成
			var historyModel = CreateHistoryModel(model, fixedPurchaseHistoryKbn);

			if (historyModel != null) historyModel.ExternalPaymentCooperationLog = externalPaymentCooperationLog;

			// モデル内容更新
			updateAction(model, historyModel);

			// 更新
			var updated = Update(model, historyModel, updateHistoryAction, accessor, isUpdateFixedPurchaseItem);
			return updated;
		}
		#endregion

		#region +UpdateOrderCount 購入回数(注文基準)更新
		/// <summary>
		/// 購入回数(注文基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderCount">購入回数(注文基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateOrderCount(
			string fixedPurchaseId,
			int orderCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_COUNT_UPDATE,
				(model, historyModel) =>
				{
					// 購入回数(注文基準)更新、購入回数(注文基準)更新結果をセット
					historyModel.UpdateOrderCount = (orderCount - model.OrderCount);
					historyModel.UpdateOrderCountResult = orderCount;

					// 購入回数(注文基準)をセット
					model.OrderCount = orderCount;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateShippedCount 購入回数(出荷基準)更新
		/// <summary>
		/// 購入回数(出荷基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="shippedCount">購入回数(出荷基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateShippedCount(
			string fixedPurchaseId,
			int shippedCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SHIPPED_COUNT_UPDATE,
				(model, historyModel) =>
				{
					// 購入回数(出荷基準)更新、購入回数(出荷基準)更新結果をセット
					historyModel.UpdateShippedCount = (shippedCount - model.ShippedCount);
					historyModel.UpdateShippedCountResult = shippedCount;

					// 購入回数(出荷基準)をセット
					model.ShippedCount = shippedCount;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateExtendStatus 拡張ステータス更新
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
		public int UpdateExtendStatus(
			string fixedPurchaseId,
			int extendStatusNo,
			string extendStatus,
			DateTime updateDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateExtendStatus(
					fixedPurchaseId,
					extendStatusNo,
					extendStatus,
					updateDate,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
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
		public int UpdateExtendStatus(
			string fixedPurchaseId,
			int extendStatusNo,
			string extendStatus,
			DateTime updateDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var result = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_EXTENDSTATUS_UPDATE,
				(model, historyModel) =>
				{
					// 拡張ステータスをセット
					model.ExtendStatus[extendStatusNo - 1].Value = extendStatus;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;

					// 拡張ステータス更新日をセット
					model.ExtendStatus[extendStatusNo - 1].ExtendStatusDate = updateDate;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return result;
		}

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
		public void UpdateOrderPayment(
			string fixedPurchaseId,
			string orderPaymentKbn,
			int? creditBranchNo,
			string cardInstallmentsCode,
			string externalPaymentAgreementId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				UpdateOrderPayment(
					fixedPurchaseId,
					orderPaymentKbn,
					creditBranchNo,
					cardInstallmentsCode,
					externalPaymentAgreementId,
					lastChanged,
					updateHistoryAction,
					accessor);
			}
		}
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
		public void UpdateOrderPayment(
			string fixedPurchaseId,
			string orderPaymentKbn,
			int? creditBranchNo,
			string cardInstallmentsCode,
			string externalPaymentAgreementId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDERPAYMENT_UPDATE,
				(model, historyModel) =>
				{
					// 支払区分、クレジットカード枝番、カード支払い回数コードをセット
					model.OrderPaymentKbn = orderPaymentKbn;
					model.CreditBranchNo = creditBranchNo;
					model.CardInstallmentsCode = cardInstallmentsCode;
					// 外部支払い契約IDをセット
					model.ExternalPaymentAgreementId = externalPaymentAgreementId;
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
					//決済ステータスに「通常」をセット
					model.PaymentStatus = Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateShipping 配送先更新
		/// <summary>
		/// 配送先更新
		/// </summary>
		/// <param name="shipping">定期配送先モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateShipping(
			FixedPurchaseShippingModel shipping,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateShipping(
					shipping,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		/// <summary>
		/// 配送先更新
		/// </summary>
		/// <param name="shipping">定期配送先モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateShipping(
			FixedPurchaseShippingModel shipping,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			Modify(
				shipping.FixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SHIPPINGUPDATE,
				(model, historyModel) =>
				{
					// 配送先をセット
					model.Shippings[0].ShippingName1 = shipping.ShippingName1;
					model.Shippings[0].ShippingName2 = shipping.ShippingName2;
					model.Shippings[0].ShippingName = shipping.ShippingName;
					model.Shippings[0].ShippingNameKana1 = shipping.ShippingNameKana1;
					model.Shippings[0].ShippingNameKana2 = shipping.ShippingNameKana2;
					model.Shippings[0].ShippingNameKana = shipping.ShippingNameKana;
					model.Shippings[0].ShippingZip = shipping.ShippingZip;
					model.Shippings[0].ShippingAddr1 = shipping.ShippingAddr1;
					model.Shippings[0].ShippingAddr2 = shipping.ShippingAddr2;
					model.Shippings[0].ShippingAddr3 = shipping.ShippingAddr3;
					model.Shippings[0].ShippingAddr4 = shipping.ShippingAddr4;
					model.Shippings[0].ShippingCompanyName = shipping.ShippingCompanyName;
					model.Shippings[0].ShippingCompanyPostName = shipping.ShippingCompanyPostName;
					model.Shippings[0].ShippingTel1 = shipping.ShippingTel1;
					model.Shippings[0].ShippingTime = shipping.ShippingTime;
					model.Shippings[0].ShippingMethod = shipping.ShippingMethod;
					model.Shippings[0].DeliveryCompanyId = shipping.DeliveryCompanyId;
					model.Shippings[0].ShippingCountryIsoCode = shipping.ShippingCountryIsoCode;
					model.Shippings[0].ShippingCountryName = shipping.ShippingCountryName;
					model.Shippings[0].ShippingAddr5 = shipping.ShippingAddr5;
					model.Shippings[0].ShippingReceivingStoreId = shipping.ShippingReceivingStoreId;
					model.Shippings[0].ShippingReceivingStoreFlg = shipping.ShippingReceivingStoreFlg;
					model.Shippings[0].ShippingReceivingStoreType = shipping.ShippingReceivingStoreType;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdatePattern 配送パターン更新
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
		public void UpdatePattern(
			string fixedPurchaseId,
			string fixedPurchaseKbn,
			string fixedPurchaseSetting1,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_PATTERNUPDATE,
				(model, historyModel) =>
				{
					// 配送パターンをセット
					model.FixedPurchaseKbn = fixedPurchaseKbn;
					model.FixedPurchaseSetting1 = fixedPurchaseSetting1;
					model.NextShippingDate = nextShippingDate;
					model.NextNextShippingDate = nextNextShippingDate;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdatePattern 次回配送日/次々回配送日更新
		/// <summary>
		/// 次回配送日/次々回配送日更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateShippingDate(
			string fixedPurchaseId,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				UpdateShippingDate(
					fixedPurchaseId,
					nextShippingDate,
					nextNextShippingDate,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
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
		public bool UpdateShippingDate(
			string fixedPurchaseId,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SHIPING_DATE_UPDATE,
				(model, historyModel) =>
				{
					// 配送パターンをセット
					if (nextShippingDate != null) model.NextShippingDate = nextShippingDate;
					if (nextNextShippingDate != null) model.NextNextShippingDate = nextNextShippingDate;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);

			return (updated > 0);
		}
		#endregion

		#region +UpdateItems 商品更新
		/// <summary>
		/// 商品更新
		/// </summary>
		/// <param name="items">定期商品モデル列</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateItems(FixedPurchaseItemModel[] items, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			if ((items != null) && (items.Length > 0))
			{
				Modify(
					items[0].FixedPurchaseId,
					Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ITEMUPDATE,
					(model, historyModel) =>
					{
						// 商品をセット
						model.Shippings[0].Items = items;

						// 最終更新者をセット
						model.LastChanged = historyModel.LastChanged = lastChanged;
					},
					lastChanged,
					updateHistoryAction);
			}
		}
		#endregion

		#region +ApplyNextShippingUsePointChange 次回購入の利用ポイントの更新を適用
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
		public bool UpdateItemsForOrderCombine(
			FixedPurchaseItemModel[] items,
			string childFixedPurchaseIds,
			bool shippingMethodChangeToExpress,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updateCount = this.Modify(
				items[0].FixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ITEMUPDATE_FOR_COMBINE,
				(model, historyModel) =>
				{
					// 商品をセット
					model.Shippings[0].Items = items;

					// 配送方法を宅配便へ変更
					if (shippingMethodChangeToExpress) model.Shippings[0].ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			if (updateCount == 0) return false;

			// 同梱元定期購入ID更新(子定期購入IDがない場合(定期台帳と定期カートの同梱)はスキップ)
			if (string.IsNullOrEmpty(childFixedPurchaseIds) == false)
			{
				updateCount = this.Modify(
					items[0].FixedPurchaseId,
					null,
					(combinedModel, combinedHistoryModel) =>
					{
						// 同梱元定期購入ID
						combinedModel.CombinedOrgFixedpurchaseIds = string.IsNullOrEmpty(combinedModel.CombinedOrgFixedpurchaseIds)
							? childFixedPurchaseIds
							: combinedModel.CombinedOrgFixedpurchaseIds + "," + childFixedPurchaseIds;

						// 最終更新者をセット
						combinedModel.LastChanged = lastChanged;
					},
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}
			return (updateCount > 0);
		}

		/// <summary>
		/// 商品更新
		/// </summary>
		/// <param name="items">定期商品モデル列</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="historyKbn">定期購入履歴区分</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		private void UpdateItems(
			FixedPurchaseItemModel[] items,
			string lastChanged,
			string historyKbn,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				this.Modify(
					items[0].FixedPurchaseId,
					historyKbn,
					(model, historyModel) =>
					{
						// 商品をセット
						model.Shippings[0].Items = items;

						// 最終更新者をセット
						model.LastChanged = historyModel.LastChanged = lastChanged;
					},
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// 次回購入の利用ポイントの更新を適用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="newUsePoint">更新後の次回購入利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したかどうか</returns>
		public bool ApplyNextShippingUsePointChange(
			string deptId,
			FixedPurchaseContainer fixedPurchase,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 利用ポイント数の更新
				var result = UpdateNextShippingUsePointToFixedPurchase(
					fixedPurchase.FixedPurchaseId,
					newUsePoint.ToString(),
					lastChanged,
					updateHistoryAction,
					accessor);
				if (result == false) return false;

				// ユーザポイントの更新
				result = new PointService().ApplyNextShippingUsePointToUserPoint(
					deptId,
					fixedPurchase.FixedPurchaseId,
					fixedPurchase.UserId,
					fixedPurchase.NextShippingUsePoint,
					newUsePoint,
					lastChanged,
					updateHistoryAction,
					accessor);
				if (result == false) return false;

				accessor.CommitTransaction();
				return true;
			}
		}
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
		public bool ApplyNextShippingUsePointChange(
			string deptId,
			FixedPurchaseModel fixedPurchase,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 利用ポイント数の更新
			var result = UpdateNextShippingUsePointToFixedPurchase(
				fixedPurchase.FixedPurchaseId,
				newUsePoint.ToString(),
				lastChanged,
				updateHistoryAction,
				accessor);
			if (result == false) return false;

			// ユーザポイントの更新
			result = new PointService().ApplyNextShippingUsePointToUserPoint(
				deptId,
				fixedPurchase.FixedPurchaseId,
				fixedPurchase.UserId,
				0,
				newUsePoint,
				lastChanged,
				updateHistoryAction,
				accessor);
			return result;
		}

		/// <summary>
		/// 次回購入の利用ポイント全ポイント継続利用の更新を適用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="newUsePoint">更新後の次回購入利用ポイント</param>
		/// <param name="newUseAllPointFlg">次回購入の利用ポイントの全適用フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したかどうか</returns>
		public bool ApplyNextShippingUseAllPointChange(
			string deptId,
			FixedPurchaseContainer fixedPurchase,
			decimal newUsePoint,
			string newUseAllPointFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 利用ポイント数の更新
				var result = UpdateNextShippingUseAllPointToFixedPurchase(
					fixedPurchase.FixedPurchaseId,
					newUsePoint.ToString(),
					newUseAllPointFlg,
					lastChanged,
					updateHistoryAction,
					accessor);
				if (result == false) return false;

				// ユーザポイントの更新
				result = new PointService().ApplyNextShippingUsePointToUserPoint(
					deptId,
					fixedPurchase.FixedPurchaseId,
					fixedPurchase.UserId,
					fixedPurchase.NextShippingUsePoint,
					newUsePoint,
					lastChanged,
					updateHistoryAction,
					accessor);
				if (result == false) return false;

				accessor.CommitTransaction();
				return true;
			}
		}
		#endregion

		#region +CancelFixedPurchase 定期購入情報の解約（解約理由なし）と次回購入利用ポイントの戻し
		/// <summary>
		/// 定期購入情報の解約（解約理由なし）と次回購入利用ポイントの戻し
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="isPointOptionOn">ポイントOPが有効かどうか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		public bool CancelFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			string lastChanged,
			string deptId,
			bool isPointOptionOn,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = CancelFixedPurchase(
					fixedPurchase,
					lastChanged,
					deptId,
					isPointOptionOn,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +CancelFixedPurchase 定期購入情報の解約（解約理由なし）と次回購入利用ポイントの戻し
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
		public bool CancelFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			string lastChanged,
			string deptId,
			bool isPointOptionOn,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			return CancelFixedPurchase(
				fixedPurchase,
				string.Empty,
				string.Empty,
				lastChanged,
				deptId,
				isPointOptionOn,
				updateHistoryAction,
				accessor);
		}
		#endregion
		#region +CancelFixedPurchase 定期購入情報の解約（解約理由付き）と次回購入利用ポイントの戻し
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
		public bool CancelFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			string cancelReasonId,
			string cancelMemo,
			string lastChanged,
			string deptId,
			bool isPointOptionOn,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = CancelFixedPurchase(
					fixedPurchase,
					cancelReasonId,
					cancelMemo,
					lastChanged,
					deptId,
					isPointOptionOn,
					updateHistoryAction,
					accessor);
				if (result) accessor.CommitTransaction();
				return result;
			}
		}
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
		public bool CancelFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			string cancelReasonId,
			string cancelMemo,
			string lastChanged,
			string deptId,
			bool isPointOptionOn,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 定期購入情報を解約
			var result = CancelInner(
				fixedPurchase.FixedPurchaseId,
				cancelReasonId,
				cancelMemo,
				lastChanged,
				updateHistoryAction,
				accessor);
			if (result == false) return false;

			// 定期継続分析
			foreach (var item in fixedPurchase.Shippings[0].Items)
			{
				new FixedPurchaseRepeatAnalysisService().FallOutFixedpurchaseItem(fixedPurchase.UserId, item.ProductId, item.VariationId, fixedPurchase.FixedPurchaseId, lastChanged, accessor);
			}

			// 次回購入の利用ポイントをユーザポイントに戻す
			if (isPointOptionOn && (fixedPurchase.NextShippingUsePoint > 0))
			{
				result = new PointService().ReturnNextShippingUsePointToUserPoint(
					deptId,
					fixedPurchase.UserId,
					fixedPurchase.FixedPurchaseId,
					fixedPurchase.NextShippingUsePoint,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				if (result == false) return false;

				// 定期購入の利用ポイント数を0に設定
				result = UpdateNextShippingUsePointToFixedPurchase(
					fixedPurchase.FixedPurchaseId,
					FixedPurchaseModel.DEFAULT_NEXT_SHIPPING_USE_POINT,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					// 下部で履歴を落とす
					accessor);
				if (result == false) return false;
			}

			// 次回購入の利用クーポンを戻す
			if (string.IsNullOrEmpty(fixedPurchase.NextShippingUseCouponId) == false)
			{
				// 次回購入利用クーポン情報をリセット
				result = ResetNextShippingUseCoupon(
					fixedPurchase.FixedPurchaseId,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				if (result == false) return false;

				// 設定したクーポンを戻す
				if (fixedPurchase.NextShippingUseCouponDetail != null)
				{
					result = new CouponService().ReturnNextShippingUseCoupon(
						fixedPurchase.NextShippingUseCouponDetail,
						fixedPurchase.UserId,
						"",
						fixedPurchase.FixedPurchaseId,
						lastChanged,
						Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE_CANCEL,
						UpdateHistoryAction.DoNotInsert,
						accessor);
					if (result == false) return false;
				}
			}

			// 生きてる定期購入情報が存在しない場合、ユーザマスタ及びセッション変数の定期会員フラグをOFFに更新する
			if (HasActiveFixedPurchaseInfo(fixedPurchase.UserId, accessor) == false)
			{
				new UserService().UpdateFixedPurchaseMemberFlg(
					fixedPurchase.UserId,
					Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF,
					lastChanged,
					UpdateHistoryAction.DoNotInsert);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(fixedPurchase.UserId, lastChanged, accessor);
				new UpdateHistoryService().InsertForFixedPurchase(fixedPurchase.FixedPurchaseId, lastChanged, accessor);
			}
			return true;
		}
		#endregion

		#region -CancelInner キャンセル（定期マスタをキャンセルするだけ）
		/// <summary>
		/// キャンセル（定期マスタをキャンセルするだけ）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="cancelReasonId">解約理由区分ID</param>
		/// <param name="cancelMemo">解約メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		private bool CancelInner(
			string fixedPurchaseId,
			string cancelReasonId,
			string cancelMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CANCEL,
				(model, historyModel) =>
				{
					// 解約理由区分ID、解約メモ、定期購入ステータスに「キャンセル」をセット
					model.CancelReasonId = cancelReasonId;
					model.CancelMemo = cancelMemo;
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL;
					// 休止から解約する時、定期再開予定日、休止理由を空にする
					model.ResumeDate = null;
					model.SuspendReason = string.Empty;

					// Set data for cancel date
					model.CancelDate = DateTime.Now;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return (updated > 0);
		}
		#endregion

		#region +Complete 頒布会完了
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
		public bool Complete(
			string fixedPurchaseId,
			string lastChanged,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_COMPLETE,
				(model, historyModel) =>
				{
					// 定期購入ステータスに「完了」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_COMPLETE;
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
					// 次回配送日・次々回配送日をセット
					model.NextShippingDate = nextShippingDate ?? model.NextShippingDate;
					model.NextNextShippingDate = nextNextShippingDate ?? model.NextNextShippingDate;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return (updated > 0);
		}
		#endregion

		#region +Resume 定期購入再開
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
		public bool Resume(
			string fixedPurchaseId,
			string userId,
			string lastChanged,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Resume(
					fixedPurchaseId,
					userId,
					lastChanged,
					nextShippingDate,
					nextNextShippingDate,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
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
		public bool Resume(
			string fixedPurchaseId,
			string userId,
			string lastChanged,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var result = ResumeInner(
				fixedPurchaseId,
				lastChanged,
				nextShippingDate,
				nextNextShippingDate,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			if (result == false) return false;

			// 生きてる定期購入情報が存在する場合、ユーザマスタ及びセッション変数の定期会員フラグをONに更新する
			if (HasActiveFixedPurchaseInfo(userId, accessor))
			{
				new UserService().UpdateFixedPurchaseMemberFlg(
					userId,
					Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				new UpdateHistoryService().InsertForFixedPurchase(fixedPurchaseId, lastChanged, accessor);
			}

			return true;
		}
		#endregion

		#region -ResumeInner 定期再開
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
		public bool ResumeInner(
			string fixedPurchaseId,
			string lastChanged,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_RESUME,
				(model, historyModel) =>
				{
					// 定期購入ステータスに「通常」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL;
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
					// 次回配送日・次々回配送日をセット
					model.NextShippingDate = nextShippingDate ?? model.NextShippingDate;
					model.NextNextShippingDate = nextNextShippingDate ?? model.NextNextShippingDate;

					// Set data for restart date
					model.RestartDate = DateTime.Now;
					// 休止理由・定期再開予定日
					model.SuspendReason = string.Empty;
					model.ResumeDate = null;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return (updated > 0);
		}
		#endregion
		#region +Resume 定期購入再開
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
		public bool Resume(
			string fixedPurchaseId,
			string lastChanged,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_RESUME,
				(model, historyModel) =>
				{
					// 定期購入ステータスに「通常」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL;
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
					// 次回配送日・次々回配送日をセット
					model.NextShippingDate = nextShippingDate ?? model.NextShippingDate;
					model.NextNextShippingDate = nextNextShippingDate ?? model.NextNextShippingDate;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);

			return (updated > 0);
		}
		#endregion

		#region +UpdateFixedPurchaseStatusTempToNormal 定期購入ステータスを仮登録から通常に更新
		/// <summary>
		/// 定期購入ステータスを仮登録から通常に更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateFixedPurchaseStatusTempToNormal(
			string orderId,
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateFixedPurchaseStatusTempToNormal(
					orderId,
					fixedPurchaseId,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		/// <summary>
		/// 定期購入ステータスを仮登録から通常に更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateFixedPurchaseStatusTempToNormal(
			string orderId,
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			//仮登録ステータスか確認
			if (CheckTemporaryRegistration(fixedPurchaseId, accessor) == false) return;

			// 注文成功のログ
			UpdateForFirstSuccessOrder(
				fixedPurchaseId,
				orderId,
				lastChanged,
				updateHistoryAction,
				accessor);

			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_REGISTER_SUCCESS,
				(model, historyModel) =>
				{
					// 定期購入ステータスに「通常」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL;
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
					historyModel.OrderId = orderId;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateFixedPurchaseStatusToStopUnavailableShippingArea 定期購入ステータスを配送不可エリア停止に更新
		/// <summary>
		/// 定期購入ステータスを配送不可エリア停止に更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateFixedPurchaseStatusToStopUnavailableShippingArea(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var fixedPurchase = Get(fixedPurchaseId, accessor);
			// すでに定期購入ステータスが「配送不可エリア停止」ならスキップ
			if (fixedPurchase.FixedPurchaseStatus
				== Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA) return;

			// 配送先住所が配送不可エリアかチェック
			var unavailableShipping = CheckUnavailableShipping(fixedPurchaseId, accessor);
			if (unavailableShipping)
			{
				UpdateForFailedUnavailableShippingArea(fixedPurchaseId, lastChanged, updateHistoryAction, accessor);
			}
		}
		#endregion

		#region +UpdateFixedPurchaseStatusStopUnavailableShippingAreaToNormal 定期購入ステータスを配送不可エリア停止から通常に更新
		/// <summary>
		/// 定期購入ステータスを配送不可エリア停止から通常に更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateFixedPurchaseStatusStopUnavailableShippingAreaToNormal(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			// 定期購入ステータスが「配送不可エリア停止」かチェック
			var fixedPurchase = Get(fixedPurchaseId, accessor);
			if (fixedPurchase.FixedPurchaseStatus
				!= Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA) return;

			// 配送先住所が配送不可エリアかチェック
			var unavailableShipping = CheckUnavailableShipping(fixedPurchaseId, accessor);
			if (unavailableShipping) return;

			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_REGISTER_SUCCESS,
				(model, historyModel) =>
				{
					// 定期購入ステータスをセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL;
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +CancelTemporaryRegistrationFixedPurchase 仮登録の定期購入をキャンセルする
		/// <summary>
		/// 仮登録の定期購入をキャンセルする
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="orderId">注文ID</param>
		public void CancelTemporaryRegistrationFixedPurchase(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string orderId = null)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				CancelTemporaryRegistrationFixedPurchase(
					fixedPurchaseId,
					lastChanged,
					updateHistoryAction,
					accessor,
					orderId);

				accessor.CommitTransaction();
			}
		}
		/// <summary>
		/// 仮登録の定期購入をキャンセルする
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="orderId">注文ID</param>
		public void CancelTemporaryRegistrationFixedPurchase(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			string orderId = null)
		{
			//仮登録ステータスか確認
			if (CheckTemporaryRegistration(fixedPurchaseId, accessor) == false) return;

			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CANCEL_TEMP,
				(model, historyModel) =>
				{
					// 定期購入ステータスに「仮登録キャンセル」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL_TEMP;
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
					if (string.IsNullOrEmpty(orderId) == false) historyModel.OrderId = orderId;

					// Set data for cancel date
					model.CancelDate = DateTime.Now;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +CheckTemporaryRegistration 仮登録ステータスか確認する
		/// <summary>
		/// 仮登録ステータスか確認する
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public bool CheckTemporaryRegistration(string fixedPurchaseId, SqlAccessor accessor = null)
		{
			if (string.IsNullOrEmpty(fixedPurchaseId)) return false;

			var status = Get(fixedPurchaseId, accessor).FixedPurchaseStatus;
			return (status == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_TEMP);
		}
		#endregion

		#region -CheckUnavailableShipping 配送不可エリアか確認する
		/// <summary>
		/// 配送不可エリアか確認する
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>配送不可エリアかどうか</returns>
		private bool CheckUnavailableShipping(string fixedPurchaseId, SqlAccessor accessor = null)
		{
			if (string.IsNullOrEmpty(fixedPurchaseId)) return false;

			var fixedPurchase = new OrderService().GetFirstFixedPurchaseOrder(fixedPurchaseId);
			var unavailableShippingZip = new ShopShippingService().GetUnavailableShippingZipFromShippingDelivery(
				 fixedPurchase.ShippingId,
				 fixedPurchase.Shippings[0].DeliveryCompanyId);
			var shippingZip = Get(fixedPurchaseId, accessor).Shippings[0].HyphenlessShippingZip;

			return unavailableShippingZip.Contains(shippingZip);
		}
		#endregion

		#region +SkipOrder スキップ
		/// <summary>
		/// スキップ
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="shopShipping">配送種別情報</param>
		/// <param name="calculationMode">次回配送日計算モード</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void SkipOrder(
			string fixedPurchaseId,
			string lastChanged,
			ShopShippingModel shopShipping,
			NextShippingCalculationMode calculationMode,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SKIPSHIPPINGDATE,
				(model, historyModel) =>
				{
					// 次回配送日に次々回配送日をセット
					model.NextShippingDate = model.NextNextShippingDate;
					// 次々回配送日をセット
					var calculateMode = GetCalculationMode(model.FixedPurchaseKbn ,calculationMode);
					model.NextNextShippingDate = this.CalculateFollowingShippingDate(
						model.FixedPurchaseKbn,
						model.FixedPurchaseSetting1,
						model.NextNextShippingDate.Value,
						shopShipping.FixedPurchaseMinimumShippingSpan,
						calculateMode);

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;

					// Increase Skipped Count
					model.SkippedCount++;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateFixedPurchaseManagementMemo 管理メモ更新
		/// <summary>
		/// 管理メモ更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseManagementMemo">管理メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <remarks>履歴登録なし</remarks>
		public void UpdateFixedPurchaseManagementMemo(
			string fixedPurchaseId,
			string fixedPurchaseManagementMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				"",
				(model, historyModel) =>
				{
					// 管理メモをセット
					model.FixedPurchaseManagementMemo = fixedPurchaseManagementMemo;

					// 最終更新者をセット
					model.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateShippingMemo 配送メモ更新
		/// <summary>
		/// 配送メモ更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="shippingMemo">配送メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateShippingMemo(
			string fixedPurchaseId,
			string shippingMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				"",
				(model, historyModel) =>
				{
					// 配送メモをセット
					model.ShippingMemo = shippingMemo;
					// 最終更新者をセット
					model.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateFixedPurchaseCancelReason 解約理由更新
		/// <summary>
		/// 解約理由更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="cancelReasonId">解約理由区分ID</param>
		/// <param name="cancelMemo">解約メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateFixedPurchaseCancelReason(
			string fixedPurchaseId,
			string cancelReasonId,
			string cancelMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CANCELREASON_UPDATE,
				(model, historyModel) =>
				{
					// 解約理由区分ID、解約メモをセット
					model.CancelReasonId = cancelReasonId;
					model.CancelMemo = cancelMemo;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateForAuthSuccess クレジットカード決済与信成功更新
		/// <summary>
		/// クレジットカード決済与信成功更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="creditBranchNo">クレジットカード枝番</param>
		/// <param name="cardInstallmentsCode">カード支払い回数コード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		public void UpdateForAuthSuccess(
			string fixedPurchaseId,
			int creditBranchNo,
			string cardInstallmentsCode,
			string lastChanged,
			string externalPaymentCooperationLog,
			UpdateHistoryAction updateHistoryAction
			)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_AUTH_SUCCESS,
				(model, historyModel) =>
				{
					// 決済ステータスに「通常」、クレジットカード枝番、カード支払い回数コードをセット
					model.PaymentStatus = Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL;
					model.CreditBranchNo = creditBranchNo;
					model.CardInstallmentsCode = cardInstallmentsCode;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				externalPaymentCooperationLog,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateForAuthSuccess クレジットカード登録失敗向け更新
		/// <summary>
		/// クレジットカード登録失敗向け更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		public void UpdateForCreditRegisterFail(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string externalPaymentCooperationLog = "")
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CREDIT_REGISTER_FAIL,
				(model, historyModel) =>
				{
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				externalPaymentCooperationLog,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateForProvisionalCreditCardRegisterd 仮クレジットカード登録向け更新
		/// <summary>
		/// 仮クレジットカード登録向け更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateForProvisionalCreditCardRegisterd(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_AUTH_SUCCESS,
				(model, historyModel) =>
				{
					model.PaymentStatus = Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL;
					model.OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateInvalidate 無効に更新
		/// <summary>
		/// 無効に更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <remarks>履歴登録なし</remarks>
		public void UpdateInvalidate(string fixedPurchaseId, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				"",
				(model, historyModel) =>
				{
					// 有効フラグに「無効」をセット
					model.ValidFlg = Constants.FLG_FIXEDPURCHASE_VALID_FLG_INVALID;
					model.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region UpdateForFirstSuccessOrder 初回注文登録成功更新
		/// <summary>
		/// 初回注文登録成功更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateForFirstSuccessOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				(model, historyModel) =>
				{
					// 注文IDセット
					historyModel.OrderId = orderId;
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateForSuccessOrder 注文登録成功更新
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
		public void UpdateForSuccessOrder(
			string fixedPurchaseId,
			DateTime nextShippingDate,
			DateTime? nextNextShippingDate,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				(model, historyModel) =>
				{
					// 注文ID、購入回数(注文基準)更新、購入回数(注文基準)更新結果をセット
					historyModel.OrderId = orderId;
					historyModel.UpdateOrderCount = 1;
					historyModel.UpdateOrderCountResult = model.OrderCount + 1;

					// 最終購入日、購入回数(注文基準) + 1、定期購入ステータスを「通常」、決済ステータスを「通常」、次回配送日、次々回配送日をセット
					model.LastOrderDate = DateTime.Now;
					model.OrderCount = model.OrderCount + 1;
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL;
					model.PaymentStatus = Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL;
					model.NextShippingDate = nextShippingDate;
					model.NextNextShippingDate = nextNextShippingDate;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// 注文登録失敗更新
		/// </summary>
		/// <param name="fixedPurchaseId"></param>
		/// <param name="lastChanged"></param>
		/// <param name="updateHistoryAction"></param>
		/// <param name="accessor"></param>
		public void UpdateForFailedOrder(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED,
				(model, historyModel) =>
				{
					// 定期購入ステータスを「その他エラー停止」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_FAILED;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateForSuccessOrder 注文登録成功更新
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
		public void UpdateForSuccessOrder(
			string fixedPurchaseId,
			DateTime nextShippingDate,
			DateTime? nextNextShippingDate,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			FixedPurchaseItemModel[] fixedPurchaseItemModel,
			SqlAccessor accessor)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS,
				(model, historyModel) =>
				{
					// 注文ID、購入回数(注文基準)更新、購入回数(注文基準)更新結果をセット
					historyModel.OrderId = orderId;
					historyModel.UpdateOrderCount = 1;
					historyModel.UpdateOrderCountResult = model.OrderCount + 1;

					// 最終購入日、購入回数(注文基準) + 1、定期購入ステータスを「通常」、決済ステータスを「通常」、次回配送日、次々回配送日をセット
					model.LastOrderDate = DateTime.Now;
					model.OrderCount = model.OrderCount + 1;
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL;
					model.PaymentStatus = Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL;
					model.NextShippingDate = nextShippingDate;
					model.NextNextShippingDate = nextNextShippingDate;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;

					model.Shippings[0].Items = fixedPurchaseItemModel;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateForFailedOrder 注文登録失敗更新
		/// <summary>
		/// 注文登録失敗更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateForFailedOrder(string fixedPurchaseId, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED,
				(model, historyModel) =>
				{
					// 定期購入ステータスを「その他エラー停止」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_FAILED;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateForFailedPayment 決済エラー停止
		/// <summary>
		/// 決済エラー停止
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		public void UpdateForFailedPayment(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string externalPaymentCooperationLog = "")
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT,
				(model, historyModel) =>
				{
					// 定期購入ステータスを「決済エラー停止」、決済ステータスを「決済失敗」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED;
					model.PaymentStatus = Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				externalPaymentCooperationLog,
				false);
		}
		#endregion

		#region +UpdateForFailedNoStock 在庫切れ停止
		/// <summary>
		/// 在庫切れ停止
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateForFailedNoStock(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_NOSTOCK,
				(model, historyModel) =>
				{
					// 定期購入ステータスを「在庫切れ停止」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateForFailedUnavailableShippingArea 配送不可エリアエラー停止
		/// <summary>
		/// 配送不可エリアエラー停止
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateForFailedUnavailableShippingArea(
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_UNAVAILABLE_SHIPPING_AREA,
				(model, historyModel) =>
				{
					// 定期購入ステータスを「配送不可エリア停止」をセット
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateForCancelOrder 注文キャンセル更新
		/// <summary>
		/// 注文キャンセル更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="doHistoryCheck">購入履歴チェック必要かどうか</param>
		public void UpdateForCancelOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool doHistoryCheck = true)
		{
			UpdateForCancelOrder(
				fixedPurchaseId,
				orderId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_CANCEL,
				lastChanged,
				updateHistoryAction,
				accessor,
				doHistoryCheck
			);
		}
		#endregion
		#region +UpdateForCancelOrder 注文同梱に伴う注文キャンセル更新
		/// <summary>
		/// 注文同梱に伴う注文キャンセル更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="doHistoryCheck">購入履歴チェック必要かどうか</param>
		public void UpdateForCancelCombinedOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool doHistoryCheck = true)
		{
			UpdateForCancelOrder(
				fixedPurchaseId,
				orderId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CANCEL_FOR_COMBINE,
				lastChanged,
				updateHistoryAction,
				accessor,
				doHistoryCheck);
		}
		#endregion
		#region -UpdateForCancelOrder 注文キャンセル更新
		/// <summary>
		/// 注文キャンセル更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="historyKbn">定期履歴区分</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="doHistoryCheck">購入履歴チェック必要かどうか</param>
		/// <param name="isCombinedFixedPurchase">注文同梱の子定期台帳かどうか</param>
		public void UpdateForCancelOrder(
			string fixedPurchaseId,
			string orderId,
			string historyKbn,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool doHistoryCheck,
			bool isCombinedFixedPurchase = false)
		{
			if (doHistoryCheck)
			{
				// 定期購入履歴情報から既にキャンセルが行われている場合は、処理を行わない
				var searchCondition = new FixedPurchaseHistoryListSearchCondition
				{
					FixedPurchaseId = fixedPurchaseId
				};
				var historyModels = this.SearchFixedPurchaseHistory(searchCondition, accessor);
				if (historyModels.Any(history => (history.OrderId == orderId) && (history.FixedPurchaseHistoryKbn == historyKbn)))
				{
					return;
				}
			}

			// 定期購入継続分析削除
			new FixedPurchaseRepeatAnalysisService().DeleteByOrder(orderId, lastChanged, accessor);

			// 定期台帳の商品を取得
			var fixedPurchaseItems = GetAllItem(fixedPurchaseId, accessor);

			// 定期台帳にあるキャンセル対象の商品の商品購入回数(注文基準) - 1
			// キャンセル対象の注文商品を取得
			var orderItemsTargetCancel = new OrderService().Get(orderId, accessor).Shippings[0].Items;
			foreach (var fixedPurchaseItem in fixedPurchaseItems
				.Where(item => orderItemsTargetCancel
					.Any(product => (product.ProductId == item.ProductId)
						&& (product.VariationId == item.VariationId))))
			{
				if (fixedPurchaseItem.ItemOrderCount <= 0) continue;
				fixedPurchaseItem.ItemOrderCount--;
			}

			var historyStatus = isCombinedFixedPurchase
				? Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_COUNT_UPDATE
				: Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_CANCEL;

			this.Modify(
				fixedPurchaseId,
				historyStatus,
				(model, historyModel) =>
				{
					// 注文ID、購入回数(注文基準)更新、購入回数(注文基準)更新結果をセット
					historyModel.OrderId = isCombinedFixedPurchase ? string.Empty : orderId;
					historyModel.UpdateOrderCount = -1;
					historyModel.UpdateOrderCountResult = model.OrderCount - 1;

					// 購入回数(注文基準) - 1
					model.OrderCount = model.OrderCount - 1;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;

					// 商品購入回数(注文基準)セット
					model.Shippings[0].Items = fixedPurchaseItems;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateForShippedOrder 注文出荷完了更新
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
		public int UpdateForShippedOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool doHistoryCheck = true)
		{
			if (doHistoryCheck)
			{
				// 定期購入履歴情報から既に注文出荷完了が行われている場合は、処理を行わない
				var searchCondition = new FixedPurchaseHistoryListSearchCondition
				{
					FixedPurchaseId = fixedPurchaseId
				};
				var historyModels = this.SearchFixedPurchaseHistory(searchCondition, accessor);
				if (historyModels.Any(
					history => 
						(history.OrderId == orderId)
						&& (history.FixedPurchaseHistoryKbn == Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_SHIPPED)))
				{
					return 0;
				}
			}

			// 定期台帳にある出荷対象の商品の商品購入回数(注文基準) + 1
			// 出荷対象の注文商品を取得
			var orderItemsTargetShipping = new OrderService().Get(orderId, accessor).Shippings[0].Items;
			// 定期台帳の商品を取得
			var fixedPurchaseItems = GetAllItem(fixedPurchaseId, accessor);
			foreach (var item in fixedPurchaseItems
				.Where(item => orderItemsTargetShipping
					.Any(product => (product.ProductId == item.ProductId)
						&& (product.VariationId == item.VariationId))))
			{
				item.ItemShippedCount++;
			}

			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_SHIPPED,
				(model, historyModel) =>
				{
					// 注文ID、購入回数(出荷基準)更新、購入回数(出荷基準)更新結果をセット
					historyModel.OrderId = orderId;
					historyModel.UpdateShippedCount = 1;
					historyModel.UpdateShippedCountResult = model.ShippedCount + 1;

					// 購入回数(出荷基準) + 1
					model.ShippedCount = model.ShippedCount + 1;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;

					// 商品購入回数(注文基準)セット
					model.Shippings[0].Items = fixedPurchaseItems;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateForReturnOrder 注文返品更新
		/// <summary>
		/// 注文返品更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateForReturnOrder(
			string fixedPurchaseId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 定期購入履歴情報から既に注文返品が行われている場合は、処理を行わない
			var searchCondition = new FixedPurchaseHistoryListSearchCondition
				{
					FixedPurchaseId = fixedPurchaseId
				};
			var historyModels = this.SearchFixedPurchaseHistory(searchCondition, accessor);
			if (historyModels.Any(history => (history.OrderId == orderId) && (history.IsReturned))) return;

			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_RETURN,
				(model, historyModel) =>
				{
					// 注文ID、購入回数(注文基準)更新、購入回数(注文基準)更新、購入回数(出荷基準)更新、購入回数(出荷基準)更新結果をセット
					historyModel.OrderId = orderId;
					historyModel.UpdateOrderCount = -1;
					historyModel.UpdateOrderCountResult = model.OrderCount - 1;
					historyModel.UpdateShippedCount = -1;
					historyModel.UpdateShippedCountResult = model.ShippedCount - 1;

					// 購入回数(注文基準) - 1、購入回数(出荷基準) - 1
					model.OrderCount = model.OrderCount - 1;
					model.ShippedCount = model.ShippedCount - 1;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateNextShippingUsePointToFixedPurchase
		/// <summary>
		/// 定期購入情報に利用ポイント数の更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="nextShipingUsePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		public bool UpdateNextShippingUsePointToFixedPurchase(
			string fixedPurchaseId,
			string nextShipingUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_NEXTSHIPINGUSEPOINT_UPDATE,
				(model, historyModel) =>
				{
					// 次回定期購入の利用ポイントをセット
					model.NextShippingUsePoint = decimal.Parse(nextShipingUsePoint);
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return (updated > 0);
		}
		#endregion

		#region +UpdateNextShippingUsePointToFixedPurchase
		/// <summary>
		/// 定期購入情報に次回購入の利用ポイントの全適用の更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="nextShipingUsePoint">利用ポイント数</param>
		/// <param name="nextShippingUseAllPointFlg">次回購入の利用ポイントの全適用フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		public bool UpdateNextShippingUseAllPointToFixedPurchase(
			string fixedPurchaseId,
			string nextShipingUsePoint,
			string nextShippingUseAllPointFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_NEXTSHIPINGUSEPOINT_UPDATE,
				(model, historyModel) =>
				{
					// 次回定期購入の利用ポイントをセット
					model.NextShippingUsePoint = decimal.Parse(nextShipingUsePoint);
					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
					// 次回購入の利用ポイントの全適用フラグ
					model.UseAllPointFlg = nextShippingUseAllPointFlg;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem: false);
			return (updated > 0);
		}
		#endregion

		#region -CreateHistoryModel 履歴モデル作成
		/// <summary>
		/// 履歴モデル作成
		/// </summary>
		/// <param name="model">定期モデル</param>
		/// <param name="fixedPurchaseHistoryKbn">定期購入履歴区分</param>
		/// <returns>履歴モデル</returns>
		private FixedPurchaseHistoryModel CreateHistoryModel(FixedPurchaseModel model, string fixedPurchaseHistoryKbn)
		{
			// 定期購入履歴区分の指定がない場合はnullを返す
			if (string.IsNullOrEmpty(fixedPurchaseHistoryKbn)) return null;

			return new FixedPurchaseHistoryModel
			{
				FixedPurchaseId = model.FixedPurchaseId,
				FixedPurchaseHistoryKbn = fixedPurchaseHistoryKbn,
				UserId = model.UserId,
				OrderId = "",
				LastChanged = model.LastChanged,
				UpdateOrderCount = null,
				UpdateShippedCount = null,
				UpdateOrderCountResult = null,
				UpdateShippedCountResult = null,
				ExternalPaymentCooperationLog = ""
			};
		}
		#endregion

		#region +SuspendFixedPurchase 定期購入情報の休止
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
		public bool SuspendFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			DateTime? resumeDate,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string suspendReason,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			var success = true;

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				success = SuspendFixedPurchase(
					fixedPurchase,
					resumeDate,
					nextShippingDate,
					nextNextShippingDate,
					suspendReason,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}

			return success;
		}
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
		public bool SuspendFixedPurchase(
			FixedPurchaseContainer fixedPurchase,
			DateTime? resumeDate,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string suspendReason,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 定期購入情報を休止
			var result = SuspendInner(
				fixedPurchase.FixedPurchaseId,
				resumeDate,
				nextShippingDate,
				nextNextShippingDate,
				suspendReason,
				lastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			if (result == false) return false;

			// 生きてる定期購入情報が存在しない場合、ユーザマスタ及びセッション変数の定期会員フラグをOFFに更新する
			if (HasActiveFixedPurchaseInfo(fixedPurchase.UserId, accessor) == false)
			{
				new UserService().UpdateFixedPurchaseMemberFlg(
					fixedPurchase.UserId,
					Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF,
					lastChanged,
					UpdateHistoryAction.DoNotInsert);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(fixedPurchase.UserId, lastChanged, accessor);
				new UpdateHistoryService().InsertForFixedPurchase(fixedPurchase.FixedPurchaseId, lastChanged, accessor);
			}

			return true;
		}
		#endregion

		#region -SuspendInner 休止
		/// <summary>
		/// 休止
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="resumeDate">定期再開予定日</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="suspendReason">休止理由</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		private bool SuspendInner(
			string fixedPurchaseId,
			DateTime? resumeDate,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			string suspendReason,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SUSPEND,
				(model, historyModel) =>
				{
					model.ResumeDate = resumeDate;
					model.NextShippingDate = nextShippingDate ?? model.NextShippingDate;	// nullの場合は更新しない
					model.NextNextShippingDate = nextNextShippingDate ?? model.NextNextShippingDate;	// nullの場合は更新しない
					model.SuspendReason = suspendReason;
					model.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_SUSPEND;
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return (updated > 0);
		}
		#endregion

		#region +UpdateFixedPurchaseSuspendReason 休止理由更新
		/// <summary>
		/// 休止理由更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="resumeDate">定期再開予定日</param>
		/// <param name="suspendReason">休止理由</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateFixedPurchaseSuspendReason(
			string fixedPurchaseId,
			DateTime? resumeDate,
			string suspendReason,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SUSPEND_REASON_UPDATE,
				(model, historyModel) =>
				{
					model.ResumeDate = resumeDate;
					model.SuspendReason = suspendReason;
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#region +UpdateFixedPurchaseMemo
		/// <summary>
		/// Update Fixed Purchase Memo
		/// </summary>
		/// <param name="fixedPurchaseId">Fixed Purchase Id</param>
		/// <param name="fixedPurchaseMemo">Fixed Purchase Memo</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		public void UpdateFixedPurchaseMemo(
			string fixedPurchaseId,
			string fixedPurchaseMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			Modify(
				fixedPurchaseId,
				string.Empty,
				(model, historyModel) =>
				{
					model.Memo = fixedPurchaseMemo;
					model.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				isUpdateFixedPurchaseItem:false);
		}
		#endregion

		#endregion // 定期購入操作関連

		#region +定期購入一覧検索関連
		/// <summary>
		/// 検索ヒット件数取得（定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <param name="replaces">クエリ置換内容</param>
		/// <returns>モデル</returns>
		public int GetCountOfSearchFixedPurchase(FixedPurchaseListSearchCondition searchCondition, KeyValuePair<string, string>[] replaces = null)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var count = repository.GetCountOfSearchFixedPurchase(searchCondition, replaces);
				return count;
			}
		}

		/// <summary>
		/// 検索（定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <param name="replaces">クエリ置換内容</param>
		/// <returns>モデル列</returns>
		public FixedPurchaseListSearchResult[] SearchFixedPurchase(FixedPurchaseListSearchCondition searchCondition, KeyValuePair<string, string>[] replaces = null)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				return repository.SearchFixedPurchase(searchCondition, replaces);
			}
		}
		#endregion

		#region +定期履歴一覧検索関連
		/// <summary>
		/// 検索ヒット件数取得（定期購入履歴一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>モデル</returns>
		public int GetCountOfSearchFixedPurchaseHistory(FixedPurchaseHistoryListSearchCondition searchCondition)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var count = repository.GetCountOfSearchFixedPurchaseHistory(searchCondition);
				return count;
			}
		}

		/// <summary>
		/// 検索（定期購入履歴一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		public FixedPurchaseHistoryListSearchResult[] SearchFixedPurchaseHistory(FixedPurchaseHistoryListSearchCondition searchCondition, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				return repository.SearchFixedPurchaseHistory(searchCondition);
			}
		}
		#endregion

		#region +ユーザ定期購入一覧検索関連
		/// <summary>
		/// 検索ヒット件数取得（ユーザ定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>件数</returns>
		public int GetCountOfSearchUserFixedPurchase(UserFixedPurchaseListSearchCondition searchCondition)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var count = repository.GetCountOfSearchUserFixedPurchase(searchCondition);
				return count;
			}
		}

		/// <summary>
		/// 検索（ユーザ定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>モデル列</returns>
		public UserFixedPurchaseListSearchResult[] SearchUserFixedPurchase(UserFixedPurchaseListSearchCondition searchCondition)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				return repository.SearchUserFixedPurchase(searchCondition);
			}
		}

		/// <summary>
		/// 検索ヒット件数取得（ユーザ定期購入一覧）注文同梱でのキャンセル除く
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>件数</returns>
		public int GetCountOfSearchUserFixedPurchaseExcludeOrderCombineCancel(UserFixedPurchaseListSearchCondition searchCondition)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var count = repository.GetCountOfSearchUserFixedPurchaseExcludeOrderCombineCancel(searchCondition);
				return count;
			}
		}

		/// <summary>
		/// 検索（ユーザ定期購入一覧）注文同梱でのキャンセル除く
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>モデル列</returns>
		public UserFixedPurchaseListSearchResult[] SearchUserFixedPurchaseExcludeOrderCombineCancel(UserFixedPurchaseListSearchCondition searchCondition)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				return repository.SearchUserFixedPurchaseExcludeOrderCombineCancel(searchCondition);
			}
		}
		#endregion

		#region +定期購入配送日計算関連
		/// <summary>
		/// 配送希望日と配送所要日数を元に、初回配送予定日を計算します。
		/// </summary>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="daysRequired">配送所要日数</param>
		/// <returns>初回配送予定日</returns>
		public DateTime CalculateFirstShippingDate(DateTime? shippingDate, int daysRequired)
		{
			var service = new FixedPurchaseShippingDateCalculator();
			return service.CalculateFirstShippingDate(shippingDate, daysRequired);
		}

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
		public DateTime CalculateNextShippingDate(string fpKbn, string fpSetting, DateTime? shippingDate, int daysRequired, int minSpan, NextShippingCalculationMode calculationMode)
		{
			var service = new FixedPurchaseShippingDateCalculator();
			return service.CalculateNextShippingDate(fpKbn, fpSetting, shippingDate, daysRequired, minSpan, calculationMode);
		}

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
		public DateTime CalculateNextNextShippingDate(string fpKbn, string fpSetting, DateTime? shippingDate, int daysRequired, int minSpan, NextShippingCalculationMode calculationMode)
		{
			var service = new FixedPurchaseShippingDateCalculator();
			return service.CalculateNextNextShippingDate(fpKbn, fpSetting, shippingDate, daysRequired, minSpan, calculationMode);
		}

		/// <summary>
		/// 指定された基準日を起点とし、次サイクルの配送日を計算して返却します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="calculationMode">配送日計算モード</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <returns>次サイクルの配送日</returns>
		public DateTime CalculateFollowingShippingDate(string fpKbn, string fpSetting, DateTime baseDate, int minSpan, NextShippingCalculationMode calculationMode)
		{
			var service = new FixedPurchaseShippingDateCalculator();
			return service.CalculateFollowingShippingDate(fpKbn, fpSetting, baseDate, minSpan, calculationMode);
		}

		/// <summary>
		/// Calculate first shipping date
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="calculationMode">配送日計算モード</param>
		/// <returns>次サイクルの配送日</returns>
		public DateTime CalculateFirstShippingDate(
			string fpKbn,
			string fpSetting,
			DateTime baseDate,
			int minSpan,
			NextShippingCalculationMode calculationMode)
		{
			var service = new FixedPurchaseShippingDateCalculator();
			return service.CalculateFirstShippingDate(fpKbn, fpSetting, baseDate, minSpan, calculationMode);
		}

		/// <summary>
		/// 最終購入注文の配送日を元にして、キャンセル可能な最短の次回配送日を計算して返却します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="lastShippedDate">最終購入注文の配送日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="daysCancel">何日前までキャンセル可能か</param>
		/// <returns>次回配送日</returns>
		public DateTime CalculateNextShippingDateFromLastShippedDate(string fpKbn, string fpSetting, DateTime lastShippedDate, int minSpan, int daysCancel)
		{
			var service = new FixedPurchaseShippingDateCalculator();
			return service.CalculateNextShippingDateFromLastShippedDate(fpKbn, fpSetting, lastShippedDate, minSpan, daysCancel);
		}

		/// <summary>
		/// Calculate first shipping date option 2
		/// </summary>
		/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
		/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
		/// <param name="firstShippingDate">First shipping date</param>
		/// <param name="minSpan">Minimum shipping span</param>
		/// <param name="calculationMode">Calculation mode</param>
		/// <returns>A first shipping date option 2</returns>
		public DateTime CalculateFirstShippingDateOption2(
			string fixedPurchaseKbn,
			string fixedPurchaseSetting,
			DateTime firstShippingDate,
			int minSpan,
			NextShippingCalculationMode calculationMode)
		{
			var service = new FixedPurchaseShippingDateCalculator();
			var result = service.CalculateFirstShippingDateOption2(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				firstShippingDate,
				minSpan,
				calculationMode);
			return result;
		}
		#endregion

		#region +GetFixedPurchaseHistoryListForMailTemplate 定期購入履歴取得(未出荷の受注情報を持つもののみ抽出)
		/// <summary>
		/// 定期購入履歴取得(未出荷の受注情報を持つもののみ抽出)
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>モデル列</returns>
		public static FixedPurchaseHistoryModel[] GetFixedPurchaseHistoryListForMailTemplate(string fixedPurchaseId)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var models = repository.GetFixedPurchaseHistoryListForMailTemplate(fixedPurchaseId);
				return models;
			}
		}
		#endregion

		#region +GetSubcriptionBoxId
		/// <summary>
		/// GetSubcriptionBoxId from Order
		/// </summary>
		/// <param name="fixedPurchaseId"> fixed purchase id</param>
		/// <returns>return Subcription Box Id</returns>
		public string GetSubcriptionBoxId(string fixedPurchaseId)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				return repository.GetSubcriptionBoxId(fixedPurchaseId);
			}
		}
		#endregion

		#region +HasActiveFixedPurchaseInfo 生きている定期購入情報が存在するかの判定
		/// <summary>
		/// 生きている定期購入情報が存在するかの判定
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true：存在する、false：存在しない</returns>
		public bool HasActiveFixedPurchaseInfo(string userId, SqlAccessor accessor = null)
		{
			var fixedPurchases = GetFixedPurchasesByUserId(userId, accessor);
			return (fixedPurchases.Any(fixedPurchase
				=> ((fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL)
						&& (fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_SUSPEND)
						&& (fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL_TEMP))));
		}
		/// <summary>
		/// 生きている定期購入情報が存在するかの判定
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="exclusionFixedPurchaseIds">除外の定期購入IDリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true：存在する、false：存在しない</returns>
		public bool HasActiveFixedPurchaseInfo(string userId, List<string> exclusionFixedPurchaseIds, SqlAccessor accessor = null)
		{
			var fixedPurchases = GetFixedPurchasesByUserId(userId, accessor);
			var result = fixedPurchases
				.Where(fp => exclusionFixedPurchaseIds.Contains(fp.FixedPurchaseId) == false)
				.Any(fp => (fp.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL));
			return result;
		}
		#endregion

		#region +HasActiveFixedPurchaseInfoByProductId 商品ごとの生きている定期購入情報が存在するかの判定
		/// <summary>
		/// 商品ごとの生きている定期購入情報が存在するかの判定
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>true：存在する、false：存在しない</returns>
		public bool HasActiveFixedPurchaseInfoByProductId(string productId, SqlAccessor accessor = null)
		{
			var fixedPurchases = GetFixedPurchasesByProductId(productId, accessor);
			return (fixedPurchases.Any(fixedPurchase
				=> ((fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL)
					&& (fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_SUSPEND)
					&& (fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL_TEMP))));
		}
		#endregion

		#region +SearchFixedPurchaseBatchMailTmpLogs 定期購入メール一時ログ検索
		/// <summary>
		/// 定期購入メール一時ログ検索
		/// </summary>
		/// <param name="actionMasterId">スケジュール実行ID</param>
		/// <returns>送信メール一覧</returns>
		public FixedPurchaseBatchMailTmpLogModel[] SearchFixedPurchaseBatchMailTmpLogs(string actionMasterId)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var results = repository.SearchFixedPurchaseBatchMailTmpLogs(actionMasterId);
				return results;
			}
		}
		#endregion

		#region +InsertFixedPurchaseBatchMailTmpLog 定期購入メール一時ログ登録
		/// <summary>
		/// 定期購入メール一時ログ登録
		/// </summary>
		/// <param name="model">定期購入メール一時ログモデル</param>
		public void InsertFixedPurchaseBatchMailTmpLog(FixedPurchaseBatchMailTmpLogModel model)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				repository.InsertFixedPurchaseBatchMailTmpLog(model);
			}
		}
		#endregion

		#region +DeleteFixedPurchaseBatchMailTmpLog 定期購入メール一時ログ削除
		/// <summary>
		/// 定期購入メール一時ログ削除
		/// </summary>
		/// <param name="tmpLogId">削除対象のtmp_log_id</param>
		public int DeleteFixedPurchaseBatchMailTmpLog(int tmpLogId)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var result = repository.DeleteFixedPurchaseBatchMailTmpLog(tmpLogId);
				return result;
			}
		}
		#endregion

		#region +RegistOrderHistoryForOrderCombine 定期購入 注文同梱履歴登録
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
		public bool RegistHistoryForOrderCombine(
			string fixedPurchaseId,
			string orderId,
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool isCountOrder = false)
		{
			// 定期台帳の商品を取得し、商品購入回数を+1 (元注文をキャンセルした際に-1されるため)
			var fixedPurchaseItems = GetAllItem(fixedPurchaseId, accessor);

			if (isCountOrder)
			{
				foreach (var item in fixedPurchaseItems)
				{
					item.ItemOrderCount++;
				}
			}

			var count = this.Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS_FOR_COMBINE,
				(model, historyModel) =>
				{
					historyModel.UpdateOrderCount = 1;
					historyModel.UpdateOrderCountResult = model.OrderCount + 1;
					historyModel.OrderId = orderId;
					historyModel.LastChanged = lastChanged;

					model.OrderCount++;
					model.LastChanged = lastChanged;
					model.UserId = userId;
					model.Shippings[0].Items = fixedPurchaseItems;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
			return (count > 0);
		}
		#endregion

		#region +GetCombinableParentFixedPurchaseWithCondition 定期購入同梱可能な親定期購入情報取得
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
		public FixedPurchaseModel[] GetCombinableParentFixedPurchaseWithCondition(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombineFixedPurchasePaymentStatus,
			string userId,
			string userName,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			int startRowNum,
			int endRowNum)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var models = repository.GetCombinableParentFixedPurchaseWithCondition(
					allowCombineFixedPurchaseStatus,
					allowCombineFixedPurchasePaymentStatus,
					userId,
					userName,
					nextShipDateFrom,
					nextShipDateTo,
					startRowNum,
					endRowNum);

				foreach (var model in models)
				{
					// 配送先&商品をセット
					this.SetChildModel(model, repository);
				}

				return models;
			}
		}
		#endregion

		#region +GetCombinableParentFixedPurchaseWithConditionCount 定期購入同梱可能な親定期購入件数取得
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
		public int GetCombinableParentFixedPurchaseWithConditionCount(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombineFixedPurchasePaymentStatus,
			string userId,
			string userName,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var count = repository.GetCombinableParentFixedPurchaseWithConditionCount(
					allowCombineFixedPurchaseStatus,
					allowCombineFixedPurchasePaymentStatus,
					userId,
					userName,
					nextShipDateFrom,
					nextShipDateTo);

				return count;
			}
		}
		#endregion

		#region +GetCombinableFixedPurchase 定期購入同梱可能な定期購入情報取得
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
		public FixedPurchaseModel[] GetCombinableFixedPurchase(
			string shopId,
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombinePaymentStatus,
			string userId,
			string shippingType,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			string parentPaymentKbn)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var models = repository.GetCombinableFixedPurchase(
					allowCombineFixedPurchaseStatus,
					allowCombinePaymentStatus,
					userId,
					shippingType,
					nextShipDateFrom,
					nextShipDateTo,
					parentPaymentKbn);

				foreach (var model in models)
				{
					// 配送先&商品をセット
					this.SetChildModel(model, repository);
				}

				// 親注文の決済種別が利用不可の商品を含む注文と、定期台帳と紐づく商品が削除済みである注文を除外する
				var combinableFps = new List<FixedPurchaseModel>();
				foreach (var model in models)
				{
					var isCombinable = true;
					foreach (var item in model.Shippings[0].Items)
					{
						var product = new ProductService().GetProductVariation(shopId, item.ProductId, item.VariationId, "");
						if ((product == null) || product.LimitedPaymentIds.Contains(parentPaymentKbn))
						{
							isCombinable = false;
							break;
						}
					}
					if (isCombinable) combinableFps.Add(model);
				}

				return combinableFps.ToArray();
			}
		}
		#endregion

		#region +GetCombinableFixedPurchaseCount 定期購入同梱可能な定期購入件数取得
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
		public int GetCombinableFixedPurchaseCount(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombinePaymentStatus,
			string userId,
			string shippingType,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			string parentPaymentKbn)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var count = repository.GetCombinableFixedPurchaseCount(
					allowCombineFixedPurchaseStatus,
					allowCombinePaymentStatus,
					userId,
					shippingType,
					nextShipDateFrom,
					nextShipDateTo,
					parentPaymentKbn);

				return count;
			}
		}
		#endregion

		#region +UpdateItemOrderCount 商品購入回数(注文基準)更新
		/// <summary>
		/// 商品購入回数(注文基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="itemOrderCount">商品購入回数(注文基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateItemOrderCount(
			string fixedPurchaseId,
			string variationId,
			int itemOrderCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			// 定期購入商品情報を取得
			var fixedPurchaseItems = GetAllItem(fixedPurchaseId);
			foreach (var item in fixedPurchaseItems.Where(i => i.VariationId == variationId))
			{
				item.ItemOrderCount = itemOrderCount;
			}
			UpdateItems(
				fixedPurchaseItems,
				lastChanged,
				updateHistoryAction);
		}
		#endregion

		#region +UpdateItemShippedCount 商品購入回数(出荷基準)更新
		/// <summary>
		/// 商品購入回数(出荷基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="itemShippedCount">商品購入回数(出荷基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateItemShippedCount(
			string fixedPurchaseId,
			string variationId,
			int itemShippedCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			// 定期購入商品情報を取得
			var fixedPurchaseItems = GetAllItem(fixedPurchaseId);
			foreach (var item in fixedPurchaseItems.Where(i => i.VariationId == variationId))
			{
				item.ItemShippedCount = itemShippedCount;
			}
			UpdateItems(
				fixedPurchaseItems,
				lastChanged,
				updateHistoryAction);
		}
		#endregion

		#region +UpdateItemOrderCountWhenOrdering 注文時の商品購入回数(注文基準)更新
		/// <summary>
		/// 注文時の商品購入回数(注文基準)更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="itemOrderCount">商品購入回数(注文基準)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateItemOrderCountWhenOrdering(
			string fixedPurchaseId,
			string variationId,
			int itemOrderCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			UpdateItemOrderCount(
				fixedPurchaseId,
				variationId,
				itemOrderCount + 1,
				lastChanged,
				updateHistoryAction);
		}
		#endregion

		#region +DeletePrefixedPurchase 仮登録の定期台帳を削除
		/// <summary>
		/// 仮登録の定期台帳と履歴を削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public bool DeletePrefixedPurchaseAndHistory(string fixedPurchaseId, SqlAccessor accessor = null)
		{
			//仮登録ステータスか確認
			if (CheckTemporaryRegistration(fixedPurchaseId, accessor) == false) return true;

			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var result = ((repository.Delete(fixedPurchaseId) > 0)
					&& (repository.DeleteItemsAll(fixedPurchaseId) > 0)
					&& (repository.DeleteShipping(fixedPurchaseId) > 0)
					&& (repository.DeleteHistory(fixedPurchaseId) > 0)
					&& (new FixedPurchaseRepeatAnalysisService().DeleteByFixedPurchaseId(fixedPurchaseId, accessor) > 0));
				return result;
			}
		}
		#endregion

		/// <summary>
		/// 定期購入履歴削除
		/// </summary>
		/// <param name="orderId">定期注文に紐づく注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>結果</returns>
		public int DeleteHistoryByOrderId(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var result = repository.DeleteHistoryByOrderId(orderId);
				return result;
			}
		}

		#region +CheckDeliveryCompanyFixedPurchaseItems 配送会社、配送種別、配送方法に紐づく定期台帳の商品数取得
		/// <summary>
		/// 配送会社、配送種別、配送方法に紐づく定期台帳の商品があるか判定
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingMethod">配送方法</param>
		/// <returns>有無</returns>
		public bool CheckDeliveryCompanyFixedPurchaseItems(string deliveryCompanyId, string shippingId, string shippingMethod)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var count = repository.GetCountOfDeliveryCompanyFixedPurchaseItems(deliveryCompanyId, shippingId, shippingMethod);
				return (count > 0);
			}
		}
		#endregion

		#region +GetTargetsForResume 再開対象の定期購入取得
		/// <summary>
		/// 再開対象の定期購入取得
		/// </summary>
		/// <returns>再開対象の定期購入情報</returns>
		public FixedPurchaseModel[] GetTargetsForResume()
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var targets = repository.GetTargetsForResume();
				return targets;
			}
		}
		#endregion

		#region +GetFixedPurchaseHistory 外部決済連携ログ取得(定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログの取得)
		/// <summary>
		/// 外部決済連携ログ取得(定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログの取得)
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログ</returns>
		public FixedPurchaseHistoryModel[] GetFixedPurchaseHistory(
			string fixedPurchaseId,
			SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var fixedPurchaseHistories =
					repository.GetFixedPurchaseHistory(fixedPurchaseId);
				return fixedPurchaseHistories;
			}
		}
		#endregion

		#region +GetDetail 外部決済連携ログ取得(定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログの取得)
		/// <summary>
		/// 外部決済連携ログ取得(定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログの取得)
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseHistoryNo">定期購入注文履歴NO</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログ</returns>
		public string GetDetailExternalPaymentCooperationLog(
			string fixedPurchaseId,
			string fixedPurchaseHistoryNo,
			SqlAccessor accessor = null)
		{
			long castFixedPurchaseHistoryNo;
			if (long.TryParse(fixedPurchaseHistoryNo, out castFixedPurchaseHistoryNo))
			{
				var fixedPurchaseHistories = GetFixedPurchaseHistory(fixedPurchaseId, accessor);
				var externalPaymentCooperationLog =
					fixedPurchaseHistories.Where(
							fixedPurchaseHistory =>
								fixedPurchaseHistory.FixedPurchaseHistoryNo == castFixedPurchaseHistoryNo).ToArray()[0]
						.ExternalPaymentCooperationLog;
				return externalPaymentCooperationLog;
			}

			return "";
		}
		#endregion

		#region +GetOrderByOrderIdAndCouponUseUser 定期購入IDとクーポン利用ユーザー(メールアドレスorユーザーID)から定期購入情報が取得
		/// <summary>
		/// 定期購入IDとクーポン利用ユーザー(メールアドレスorユーザーID)から定期購入情報が取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="usedUserJudgeType">利用済みユーザー判定方法</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期購入情報</returns>
		public FixedPurchaseModel GetFixedPurchaseByFixedPurchaseIdAndCouponUseUser(
			string fixedPurchaseId,
			string couponUseUser,
			string usedUserJudgeType,
			SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var model = repository.GetFixedPurchaseByFixedPurchaseIdAndCouponUseUser(
					fixedPurchaseId,
					couponUseUser,
					usedUserJudgeType);
				return model;
			}
		}
		#endregion

		#region +UpdateSkippedCount
		/// <summary>
		/// Clear Skipped Count
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int ClearSkippedCount(
			string fixedPurchaseId,
			SqlAccessor accessor = null)
		{
			var result = 0;
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				// 更新
				result = repository.ClearSkippedCount(fixedPurchaseId);
			}

			return result;
		}
		#endregion

		#region +UpdateSubScriptionBoxOrderCount
		/// <summary>
		/// Update SubScriptionBox Order Count
		/// </summary>
		/// <param name="fixedPurchaseId"> Fixed pruchase id</param>
		/// <param name="subscriptionBoxOrderCount">Ordering number of times with SubScription Box</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセサー</param>
		/// <returns>Number of updated rows</returns>
		public int UpdateSubScriptionBoxOrderCount(
			string fixedPurchaseId,
			int subscriptionBoxOrderCount,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var count = repository.UpdateSubScriptionBoxOrderCount(fixedPurchaseId, subscriptionBoxOrderCount);
				return count;
			}
		}
		#endregion

		/// <summary>
		/// 外部支払契約ID設定
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="externalPaymentAgreementId">外部支払契約ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="fixedPurchaseHistoryKbn">定期購入履歴区分</param>
		/// <returns>影響を受けた件数</returns>
		public int SetExternalPaymentAgreementId(
			string fixedPurchaseId,
			string externalPaymentAgreementId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null,
			string fixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_AUTH_SUCCESS)
		{
			// 更新
			var result = Modify(
				fixedPurchaseId,
				fixedPurchaseHistoryKbn,
				(model, historyModel) =>
				{
					model.ExternalPaymentAgreementId = externalPaymentAgreementId;
					historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return result;
		}

		#region +UpdateOrderExtend 注文拡張項目の更新
		/// <summary>
		/// 注文拡張項目の更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期ID</param>
		/// <param name="lastChange">最終更新者</param>
		/// <param name="values">変更内容</param>
		/// <param name="updateHistoryAction">更新履歴</param>
		/// <param name="accessor">Sqlアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateOrderExtend(
			string fixedPurchaseId,
			string lastChange,
			Dictionary<string, string> values,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			if (values.Count == 0) return 0;

			var result = Modify(
				fixedPurchaseId,
				string.Empty,
				(model, historyModel) =>
				{
					foreach (var value in values.Where(value => model.DataSource.ContainsKey(value.Key)))
					{
						model.DataSource[value.Key] = value.Value;
					}
					model.LastChanged = lastChange;
				},
				lastChange,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return result;
		}
		#endregion

		#region +GetOrderCountByFixedPurchaseWorkflowSetting
		/// <summary>
		/// Get order count by fixed purchase workflow setting
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Order count</returns>
		public int GetOrderCountByFixedPurchaseWorkflowSetting(Hashtable searchParam)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var orderCount = repository.GetOrderCountByFixedPurchaseWorkflowSetting(searchParam);
				return orderCount;
			}
		}
		#endregion

		#region +UpdateExtendStatus
		/// <summary>
		/// 拡張ステータス更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="extendStatusNo">拡張ステータスNo</param>
		/// <param name="extendStatus">拡張ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		public int UpdateExtendStatus(
			string fixedPurchaseId,
			int extendStatusNo,
			string extendStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateExtendStatus(
					fixedPurchaseId,
					extendStatusNo,
					extendStatus,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}

		/// <summary>
		/// 拡張ステータス更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="extendStatusNo">拡張ステータスNo</param>
		/// <param name="extendStatus">拡張ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public int UpdateExtendStatus(
			string fixedPurchaseId,
			int extendStatusNo,
			string extendStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var result = Modify(
				fixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_EXTENDSTATUS_UPDATE,
				(model, historyModel) =>
				{
					// 拡張ステータスをセット
					model.ExtendStatus[extendStatusNo - 1].Value = extendStatus;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor,
				isUpdateFixedPurchaseItem:false);
			return result;
		}
		#endregion

		#region +GetCountOrderFixedPurchase
		/// <summary>
		/// Get countorder fixed purchase
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Fixed purchase order quantity</returns>
		public int GetCountOrderFixedPurchase(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var count = repository.GetCountOrderFixedPurchase(orderId);
				return count;
			}
		}
		#endregion

		#region +マスタ出力
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
			using (var repository = new FixedPurchaseRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				statementName,
				new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames),
				new KeyValuePair<string, string>("@@ order_extend_field_name @@",
					string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, StringUtility.ToEmpty(input[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]))),
				new KeyValuePair<string, string>("@@ where @@",
					StringUtility.ToEmpty(input["@@ where @@"]))))
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
			using (var repository = new FixedPurchaseRepository())
			{
				var dv = repository.GetMaster(
					input,
					statementName, new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames),
					new KeyValuePair<string, string>("@@ order_extend_field_name @@",
						string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, StringUtility.ToEmpty(input[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]))),
					new KeyValuePair<string, string>("@@ where @@",
						StringUtility.ToEmpty(input["@@ where @@"])));
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
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE: // 定期購入マスタ
					return "GetFixedPurchaseMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM: // 定期購入商品マスタ
					return "GetFixedPurchaseItemMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE_WORKFLOW:// 定期購入（ワークフロー）
					return "GetFixedPurchaseWorkflowMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM_WORKFLOW:// 定期購入商品（ワークフロー）
					return "GetFixedPurchaseItemWorkflowMaster";
			}
			throw new Exception("未対応のマスタ区分：" + masterKbn);
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="shopId">ショップID</param>
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMaster(string sqlFieldNames, string masterKbn, string shopId)
		{
			try
			{
				using (var repository = new FixedPurchaseRepository())
				{
					repository.CheckFieldsForGetMaster(
						new Hashtable { { Constants.FIELD_FIXEDPURCHASE_SHOP_ID, shopId } },
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
		#endregion

		#region +UpdateForReturnOrderItem 注文返品更新(商品単位)
		/// <summary>
		/// 注文返品更新(商品単位)
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="isChangeStatusOrderfromComplete">Is change status order from complete</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateForReturnOrderItem(
			string fixedPurchaseId,
			string orderId,
			string variationId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			bool isChangeStatusOrderFromComplete = false)
		{
			// 定期購入履歴情報から既に注文返品が行われている場合は、処理を行わない
			var searchCondition = new FixedPurchaseHistoryListSearchCondition
			{
				FixedPurchaseId = fixedPurchaseId
			};
			var historyModels = this.SearchFixedPurchaseHistory(searchCondition, accessor);
			if (historyModels
				.Any(history => ((history.OrderId == orderId)
					&& (history.IsReturned)))) return;

			// 定期購入商品情報を取得
			var fixedPurchaseItems = GetAllItem(fixedPurchaseId);

			foreach (var item in fixedPurchaseItems.Where(i => i.VariationId == variationId))
			{
				if (isChangeStatusOrderFromComplete)
				{
					item.ItemOrderCount++;
					item.ItemShippedCount++;
				}
				else
				{
					item.ItemOrderCount--;
					item.ItemShippedCount--;
				}
			}
			UpdateItems(
				fixedPurchaseItems,
				lastChanged,
				updateHistoryAction);
		}
		#endregion

		#region +GetAllItem 取得
		/// <summary>
		/// 定期台帳商品取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public FixedPurchaseItemModel[] GetAllItem(string fixedPurchaseId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var model = repository.GetItemAll(fixedPurchaseId);
				return model;
			}
		}
		#endregion

		#region +GetFixedPurchaseWorkflowListNoPagination
		/// <summary>
		/// Get fixed purchase workflow list no pagination
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Dataview of fixed purchase list</returns>
		public DataView GetFixedPurchaseWorkflowListNoPagination(Hashtable searchParam)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var results = repository.GetFixedPurchaseWorkflowListNoPagination(searchParam);
				return results;
			}
		}
		#endregion

		#region +GetFixedPurchaseWorkflowList
		/// <summary>
		/// Get fixed purchase workflow list
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <param name="pageNumber">Pager number</param>
		/// <returns>Dataview of fixed purchase list</returns>
		public DataView GetFixedPurchaseWorkflowList(Hashtable searchParam, int pageNumber)
		{
			using (var repository = new FixedPurchaseRepository())
			{
				var results = repository.GetFixedPurchaseWorkflowList(searchParam, pageNumber);
				return results;
			}
		}
		#endregion

		/// <summary>
		/// 次回配送日計算モードを取得
		/// </summary>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="defaultCalculationMode">デフォルト計算モード（Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE）</param>
		/// <returns>次回配送日計算モード</returns>
		public NextShippingCalculationMode GetCalculationMode(
			string fixedPurchaseKbn,
			NextShippingCalculationMode defaultCalculationMode)
		{
			var calculateMode = ((fixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
				|| (fixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY)
				|| (fixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS))
				? defaultCalculationMode
				: NextShippingCalculationMode.EveryNWeek;

			return calculateMode;
		}

		#region +DeleteAllItems 削除
		/// <summary>
		/// 定期台帳の商品のみ削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		public int DeleteAllItems(string fixedPurchaseId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				var result = repository.DeleteItemsAll(fixedPurchaseId);
				return result;
			}
		}
		#endregion

		#region +InsertSingleItem 商品の登録
		/// <summary>
		/// 定期台帳の商品のみ登録
		/// </summary>
		/// <param name="item">商品</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertSingleItem(FixedPurchaseItemModel item, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepository(accessor))
			{
				repository.InsertItem(item);
			}
		}
		#endregion

		/// <summary>
		/// 頒布会次回配送商品の更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="isPointOptionOn">ポイントオプションON/OFF</param>
		/// <param name="getNextProductsResult">次回配送商品取得結果</param>
		/// <param name="updateHistoryAction">履歴更新アクション</param>
		/// <returns>結果</returns>
		public bool UpdateNextDeliveryForSubscriptionBox(
			string fixedPurchaseId,
			string lastChanged,
			bool isPointOptionOn,
			SubscriptionBoxGetNextProductsResult getNextProductsResult,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateNextDeliveryForSubscriptionBox(
					fixedPurchaseId,
					lastChanged,
					isPointOptionOn,
					getNextProductsResult,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}

			return true;
		}
		/// <summary>
		/// 頒布会次回配送商品の更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="isPointOptionOn">ポイントオプションON/OFF</param>
		/// <param name="getNextProductsResult">次回配送商品取得結果</param>
		/// <param name="updateHistoryAction">履歴更新アクション</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>結果</returns>
		public bool UpdateNextDeliveryForSubscriptionBox(
			string fixedPurchaseId,
			string lastChanged,
			bool isPointOptionOn,
			SubscriptionBoxGetNextProductsResult getNextProductsResult,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			if (string.IsNullOrEmpty(fixedPurchaseId))
			{
				return false;
			}

			var fixedPurchaseContainer = GetContainer(
				fixedPurchaseId,
				false,
				accessor);

			switch (getNextProductsResult.Result)
			{
				case SubscriptionBoxGetNextProductsResult.ResultTypes.Success:
						DeleteAllItems(fixedPurchaseId, accessor);

						foreach (var item in getNextProductsResult.NextProducts)
						{
							InsertSingleItem(item, accessor);
						}

						//成功の場合拡張ステータスを更新(44.頒布会の次回商品取得一部失敗フラグ)
						UpdateExtendStatus(
							fixedPurchaseContainer.FixedPurchaseId,
							44,
							"0",
							DateTime.Now,
							lastChanged,
							updateHistoryAction,
							accessor);

						UpdateSubScriptionBoxOrderCount(
							fixedPurchaseContainer.FixedPurchaseId,
							getNextProductsResult.NextCount,
							updateHistoryAction,
							accessor);
						break;

				case SubscriptionBoxGetNextProductsResult.ResultTypes.Cancel:
					CancelFixedPurchase(
						fixedPurchaseContainer,
						string.Empty,
						string.Empty,
						lastChanged,
						Constants.CONST_DEFAULT_DEPT_ID,
						isPointOptionOn,
						updateHistoryAction,
						accessor);
					break;

				case SubscriptionBoxGetNextProductsResult.ResultTypes.Fail:
					UpdateForFailedOrder(
						fixedPurchaseContainer.FixedPurchaseId,
						lastChanged,
						updateHistoryAction,
						accessor);
					break;

				case SubscriptionBoxGetNextProductsResult.ResultTypes.PartialFailure:
					DeleteAllItems(fixedPurchaseId, accessor);

					foreach (var item in getNextProductsResult.NextProducts)
					{
						InsertSingleItem(item, accessor);
					}

					//一部商品が更新できなかった場合拡張ステータスを更新(44.頒布会の次回商品取得一部失敗フラグ)
					UpdateExtendStatus(
						fixedPurchaseContainer.FixedPurchaseId,
						44,
						"1",
						DateTime.Now,
						lastChanged,
						updateHistoryAction,
						accessor);

					UpdateSubScriptionBoxOrderCount(
						fixedPurchaseContainer.FixedPurchaseId,
						getNextProductsResult.NextCount,
						updateHistoryAction,
						accessor);
					break;

			}
			return true;
		}
	}
}
