/*
=========================================================================================================
  Module      : 注文共通処理クラス 注文キャンセル部分(OrderCommon_OrderCancel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using Amazon.Pay.API.Types;
using AmazonPay.Responses;
using jp.veritrans.tercerog.mdk;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Api;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.NextEngine;
using w2.App.Common.NextEngine.Helper;
using w2.App.Common.NextEngine.Response;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Aftee;
using w2.App.Common.Order.Payment.Atone;
using w2.App.Common.Order.Payment.Boku;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.App.Common.Order.Payment.DSKDeferred.OrderCancel;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.EScott;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.OrderModifyCancel;
using w2.App.Common.Order.Payment.GMO.Zcom.Cancel;
using w2.App.Common.Order.Payment.GMOAtokara;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction;
using w2.App.Common.Order.Payment.LinePay;
using w2.App.Common.Order.Payment.NewebPay;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.Paygent.Paidy.AuthorizationCancellation.Dto;
using w2.App.Common.Order.Payment.Paygent.Paidy.Refund.Dto;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.Cancel;
using w2.App.Common.Order.Payment.TriLinkAfterPay;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Request;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.User;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.ProductStock;
using w2.Domain.SerialKey;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Order
{
	public partial class OrderCommon
	{
		/// <summary>クレジットカード売上確定後キャンセル可能日</summary>
		public enum CancelableDays
		{
			/// <summary>GMO（180日以内）</summary>
			Gmo = 180,
			/// <summary>ヤマトKWC（90日以内）</summary>
			YamatoKwc = 90,
			/// <summary>その他（不明）</summary>
			Other = 0
		}

		/// <summary>
		/// 注文情報キャンセル付随処理（ステータス・外部連携以外更新）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="rollBackRealStock">実在庫をロールバックするか</param>
		/// <param name="productStockHistoryActionStatus">アクションステータス</param>
		/// <param name="loginOperatorDeptId">ログインオペレータ識別ID</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="stockCooperation">在庫連動するか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="updateTwInvoiceStatus">電子発票更新ステータス</param>
		/// <remarks>ユーザーコントロールからの呼び出し対応のためにstatic化する</remarks>
		public static void CancelOrderSubProcess(
			DataRowView order,
			bool rollBackRealStock,
			string productStockHistoryActionStatus,
			string loginOperatorDeptId,
			string loginOperatorName,
			bool stockCooperation,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			string updateTwInvoiceStatus = "")
		{
			var orderModel = new OrderModel(order);
			if (string.IsNullOrEmpty(StringUtility.ToEmpty(order[Constants.FIELD_ORDERCOUPON_COUPON_ID])) == false)
			{
				orderModel.Coupons = new[] { new OrderCouponModel(order) };
			}
			CancelOrderSubProcess(
				orderModel,
				rollBackRealStock,
				productStockHistoryActionStatus,
				loginOperatorDeptId,
				loginOperatorName,
				stockCooperation,
				updateHistoryAction,
				accessor,
				updateTwInvoiceStatus);
		}
		/// <summary>
		/// 注文情報キャンセル付随処理（ステータス・外部連携以外更新）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="rollBackRealStock">実在庫をロールバックするか</param>
		/// <param name="productStockHistoryActionStatus">アクションステータス</param>
		/// <param name="loginOperatorDeptId">ログインオペレータ識別ID</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="stockCooperation">在庫連動するか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="updateTwInvoiceStatus">電子発票更新ステータス</param>
		/// <remarks>ユーザーコントロールからの呼び出し対応のためにstatic化する</remarks>
		public static void CancelOrderSubProcess(
			OrderModel order,
			bool rollBackRealStock,
			string productStockHistoryActionStatus,
			string loginOperatorDeptId,
			string loginOperatorName,
			bool stockCooperation,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			string updateTwInvoiceStatus = "")
		{
			CancelOrderSubProcess(
				order,
				rollBackRealStock,
				productStockHistoryActionStatus,
				loginOperatorDeptId,
				loginOperatorName,
				stockCooperation,
				(string.IsNullOrEmpty(order.CombinedOrgOrderIds) == false),
				updateHistoryAction,
				null,
				accessor,
				updateTwInvoiceStatus);
		}

		/// <summary>
		/// 注文同梱に伴る注文情報キャンセル付随処理（ステータス・外部連携以外更新）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="rollBackRealStock">実在庫をロールバックするか</param>
		/// <param name="productStockHistoryActionStatus">アクションステータス</param>
		/// <param name="loginOperatorDeptId">ログインオペレータ識別ID</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="stockCooperation">在庫連動するか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="orderCombineCoupon">注文同梱で生成された新注文が利用するクーポン</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <remarks>ユーザーコントロールからの呼び出し対応のためにstatic化する</remarks>
		public static void CancelOrderSubProcessForOrderCombine(
			DataRowView order,
			bool rollBackRealStock,
			string productStockHistoryActionStatus,
			string loginOperatorDeptId,
			string loginOperatorName,
			bool stockCooperation,
			UpdateHistoryAction updateHistoryAction,
			CartCoupon orderCombineCoupon,
			SqlAccessor accessor)
		{
			var orderModel = new OrderModel(order);
			if (string.IsNullOrEmpty(StringUtility.ToEmpty(order[Constants.FIELD_ORDERCOUPON_COUPON_ID])) == false)
			{
				orderModel.Coupons = new[] { new OrderCouponModel(order) };
			}
			CancelOrderSubProcessForOrderCombine(
				orderModel,
				rollBackRealStock,
				productStockHistoryActionStatus,
				loginOperatorDeptId,
				loginOperatorName,
				stockCooperation,
				updateHistoryAction,
				orderCombineCoupon,
				accessor);
		}
		/// <summary>
		/// 注文同梱に伴る注文情報キャンセル付随処理（ステータス・外部連携以外更新）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="rollBackRealStock">実在庫をロールバックするか</param>
		/// <param name="productStockHistoryActionStatus">アクションステータス</param>
		/// <param name="loginOperatorDeptId">ログインオペレータ識別ID</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="stockCooperation">在庫連動するか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="orderCombineCoupon">注文同梱で生成された新注文が利用するクーポン</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <remarks>ユーザーコントロールからの呼び出し対応のためにstatic化する</remarks>
		public static void CancelOrderSubProcessForOrderCombine(
			OrderModel order,
			bool rollBackRealStock,
			string productStockHistoryActionStatus,
			string loginOperatorDeptId,
			string loginOperatorName,
			bool stockCooperation,
			UpdateHistoryAction updateHistoryAction,
			CartCoupon orderCombineCoupon,
			SqlAccessor accessor)
		{
			CancelOrderSubProcess(
				order,
				rollBackRealStock,
				productStockHistoryActionStatus,
				loginOperatorDeptId,
				loginOperatorName,
				stockCooperation,
				true,
				updateHistoryAction,
				orderCombineCoupon,
				accessor);
		}

		/// <summary>
		/// 注文情報キャンセル付随処理（ステータス・外部連携以外更新）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="rollBackRealStock">実在庫をロールバックするか</param>
		/// <param name="productStockHistoryActionStatus">アクションステータス</param>
		/// <param name="loginOperatorDeptId">ログインオペレータ識別ID</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="stockCooperation">在庫連動するか</param>
		/// <param name="isOrderCombined">注文同梱しているか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="orderCombineCoupon">注文同梱で生成された新注文が利用するクーポン</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="updateTwInvoiceStatus">電子発票更新ステータス</param>
		/// <remarks>ユーザーコントロールからの呼び出し対応のためにstatic化する</remarks>
		private static void CancelOrderSubProcess(
			OrderModel order,
			bool rollBackRealStock,
			string productStockHistoryActionStatus,
			string loginOperatorDeptId,
			string loginOperatorName,
			bool stockCooperation,
			bool isOrderCombined,
			UpdateHistoryAction updateHistoryAction,
			CartCoupon orderCombineCoupon,
			SqlAccessor accessor,
			string updateTwInvoiceStatus = "")
		{
			// 在庫戻し処理
			if (stockCooperation)
			{
				CancelProductStock(
					order,
					rollBackRealStock,
					productStockHistoryActionStatus,
					((loginOperatorName == Constants.FLG_LASTCHANGED_USER) ? "" : loginOperatorName),
					updateHistoryAction,
					accessor);
			}

			var userService = new UserService();
			var user = userService.Get(order.UserId, accessor);
			if ((user != null) && (user.DelFlg == Constants.FLG_USER_DELFLG_UNDELETED))
			{
				// 利用・付与ポイント戻し処理
				if (Constants.W2MP_POINT_OPTION_ENABLED)
				{
					// ユーザー利用ポイント戻し
					// 注文同梱はポイントの有効期限が切れていてもポイント利用できる仕様のため
					// 注文同梱の場合は有効期限が切れている場合も必ずポイントの戻し処理を行う
					CancelUserPointUse(
						order,
						loginOperatorDeptId,
						loginOperatorName,
						isOrderCombined,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					// ユーザー付与ポイント戻し
					CancelUserPointAdd(
						order,
						loginOperatorDeptId,
						loginOperatorName,
						UpdateHistoryAction.DoNotInsert,
						accessor);
				}

				// クーポン戻し処理
				if (Constants.W2MP_COUPON_OPTION_ENABLED)
				{
					if ((order.Coupons != null) && (order.Coupons.Any()))
					{
						// ユーザー利用クーポン戻し
						CancelUserCouponUse(
							order,
							user,
							loginOperatorName,
							orderCombineCoupon,
							accessor,
							UpdateHistoryAction.DoNotInsert);
					}

					// ユーザー付与クーポン戻し
					CancelUserCouponAdd(order, loginOperatorDeptId, loginOperatorName, UpdateHistoryAction.DoNotInsert, accessor);
				}
			}

			// シリアルキー戻し処理
			if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
			{
				CancelOrderSerialKey(order, loginOperatorName, accessor);
			}

			// 定期購入情報：購入回数(注文基準)戻し処理
			// 定期購入OP有効?
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				// 元注文かつ仮注文以外かつ定期購入注文?
				if (order.IsOriginalOrder && order.IsFixedPurchaseOrder)
				{
					// 注文同梱あり
					if (isOrderCombined)
					{
						new FixedPurchaseService().UpdateForCancelCombinedOrder(
							order.FixedPurchaseId,
							order.OrderId,
							loginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						
						if (string.IsNullOrEmpty(order.CombinedOrgOrderIds) == false)
						{
							// 注文同梱に紐づくすべての定期台帳ID取得
							var combinedFixedPurchaseIds = new List<string>();
							foreach (var combinedOrgOrderId in OrderCombineUtility.GetCombineOrgOrderIds(order.CombinedOrgOrderIds))
							{
								var combinedFixedPurchaseId = new OrderService()
									.GetOrderInfoByOrderId(combinedOrgOrderId, accessor).FixedPurchaseId;
								if (string.IsNullOrEmpty(combinedFixedPurchaseId)) continue;

								combinedFixedPurchaseIds.Add(combinedFixedPurchaseId);
							}

							// 注文同梱（子）に紐づく定期台帳更新
							var isFirstCombinedFixedPurchaseId = true;
							foreach (var combinedFixedPurchaseId in combinedFixedPurchaseIds)
							{
								if (isFirstCombinedFixedPurchaseId && (combinedFixedPurchaseId == order.FixedPurchaseId)) 
								{
									// 初回は同梱注文のキャンセルで紐づいている定期台帳にて実施済みの為、処理しない
									isFirstCombinedFixedPurchaseId = false;
									continue;
								}

								new FixedPurchaseService().UpdateForCancelOrder(
									combinedFixedPurchaseId,
									order.OrderId,
									Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_COUNT_UPDATE,
									loginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor,
									doHistoryCheck: false,
									isCombinedFixedPurchase: true);
							}
						}
					}
					// 注文同梱なし
					else
					{
						new FixedPurchaseService().UpdateForCancelOrder(
							order.FixedPurchaseId,
							order.OrderId,
							loginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							accessor);
					}
				}
			}

			// Cancel Invoice
			var currentOrder = new OrderService().Get(order.OrderId, accessor);
			var shippings = order.Shippings ?? currentOrder.Shippings;
			var service = new TwOrderInvoiceService();
			var twOrderInvoice = service.GetOrderInvoice(
				order.OrderId,
				shippings[0].OrderShippingNo,
				accessor);

			if (twOrderInvoice != null)
			{
				if (Constants.TWINVOICE_ECPAY_ENABLED
					&& (twOrderInvoice.TwInvoiceStatus != Constants.FLG_ORDER_INVOICE_STATUS_CANCEL)
					&& ((updateTwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_REFUND)
						|| (updateTwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_CANCEL)))
				{
					var invoiceEcPayApi = new TwInvoiceEcPayApi();
					twOrderInvoice.TwInvoiceStatus = updateTwInvoiceStatus;
					var isRefund = (twOrderInvoice.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_REFUND);
					if (isRefund)
					{
						twOrderInvoice.TwInvoiceNo = string.Format("{0}-1", twOrderInvoice.TwInvoiceNo);
					}

					// Execute invalid api
					var request = isRefund
						? invoiceEcPayApi.CreateRequestReturnObject(
							TwInvoiceEcPayApi.ExecuteTypes.Allowance,
							currentOrder,
							twOrderInvoice,
							accessor)
						: invoiceEcPayApi.CreateRequestCancelObject(
							TwInvoiceEcPayApi.ExecuteTypes.Invalid,
							order,
							twOrderInvoice,
							accessor);
					var response = invoiceEcPayApi.ReceiveResponseObject(
						isRefund
							? TwInvoiceEcPayApi.ExecuteTypes.Allowance
							: TwInvoiceEcPayApi.ExecuteTypes.Invalid,
						request);

					if (response.IsSuccess == false)
					{
						throw new Exception(response.Message);
					}
					if (isRefund)
					{
						// Update Taiwan order invoice status
						twOrderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED;
						twOrderInvoice.TwInvoiceNo = response.Response.Data.IAAllowNo;
					}
				}

				service.UpdateTwOrderInvoiceForModify(
					twOrderInvoice,
					loginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED
				&& UserService.IsUser(user.UserKbn))
			{
				var result = CrossPointUtility.DeletePurchasePoint(order);

				if (result == false)
				{
					throw new Exception(MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR));
				}

				DomainFacade.Instance.OrderService.UpdateOrderExtendStatus(
					order.OrderId,
					Constants.ORDER_EXTEND_STATUS_NO_CROSSPOINT_GRANTED,
					Constants.FLG_ORDER_EXTEND_STATUS_ON,
					DateTime.Now,
					Constants.FLG_LASTCHANGED_SYSTEM,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// Adjust point and member rank by Cross Point api
				UserUtility.AdjustPointAndMemberRankByCrossPointApi(
					user.UserId,
					accessor,
					UpdateHistoryAction.DoNotInsert);
			}

			// ネクストエンジン連携
			if (Constants.NE_OPTION_ENABLED
				&& Constants.NE_REALATION_TEMP_ORDER
				&& ((order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP) 
					|| (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)))
			{
				try
				{
					var result = UpdateNextEngineOrderForCancel(order.OrderId, accessor);
					if (result.Item2)
					{
						var mailMessage = new StringBuilder();
						mailMessage.AppendLine(NextEngineConstants.ERROR_MESSAGE_FAIL_TMP_CNSL);
						mailMessage.AppendLine(NextEngineConstants.ERROR_MESSAGE_FAIL_TMP_CNSL_FOR_ADMIN);
						mailMessage.AppendLine(string.Format(
							NextEngineConstants.ERROR_MESSAGE_FORMAT_TARGET,
							order.OrderId,
							order.UserId));

						NextEngineApi.MailSend(new Hashtable { { "message", mailMessage.ToString() }, });

						var logMessage = new StringBuilder();
						logMessage.AppendLine(NextEngineConstants.ERROR_MESSAGE_FAIL_TMP_CNSL);
						logMessage.AppendLine(string.Format(NextEngineConstants.ERROR_MESSAGE_FORMAT_TARGET,
							order.OrderId,
							order.UserId));

						FileLogger.WriteError(logMessage.ToString());

						return;
					}

					if (string.IsNullOrEmpty(result.Item1) == false)
					{
						throw new Exception(result.Item1);
					}
				}
				catch (Exception ex)
				{
					throw new Exception(MessageManager.GetMessages(CommerceMessages.ERRMSG_SYSTEM_ERROR), ex);
				}
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(
					order.OrderId,
					loginOperatorName,
					accessor);
			}
		}

		/// <summary>
		/// 在庫戻し処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="rollBackRealStock">実在庫ロールバックするか</param>
		/// <param name="productStockHistoryActionStatus">商品在庫履歴アクションステータス</param>
		/// <param name="loginOperatorName">オペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private static void CancelProductStock(
			OrderModel order,
			bool rollBackRealStock,
			string productStockHistoryActionStatus,
			string loginOperatorName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			var orderItems = order.Items ?? new OrderService().Get(order.OrderId, sqlAccessor).Items;
			var service = new ProductStockService();
			// 論理在庫キャンセル
			service.UpdateProductStockCancel(orderItems, productStockHistoryActionStatus, loginOperatorName, sqlAccessor);

			// 実在庫キャンセル
			if (rollBackRealStock)
			{
				UpdateOrderItemRealStockCanceled(
					order.OrderId,
					order.ShopId,
					loginOperatorName,
					updateHistoryAction,
					sqlAccessor);
			}
		}

		/// <summary>
		/// ユーザー利用ポイント戻し
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="loginOperatorDeptId">オペレータ識別ID</param>
		/// <param name="loginOperatorName">オペレータ名</param>
		/// <param name="shouldRestoreExpiredPoint">期限切れのポイントを戻すか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ポイント戻しされたか</returns>
		private static bool CancelUserPointUse(
			DataRowView order,
			string loginOperatorDeptId,
			string loginOperatorName,
			bool shouldRestoreExpiredPoint,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			return CancelUserPointUse(
				new OrderModel(order),
				loginOperatorDeptId,
				loginOperatorName,
				shouldRestoreExpiredPoint,
				updateHistoryAction,
				sqlAccessor);
		}
		/// <summary>
		/// ユーザー利用ポイント戻し
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="loginOperatorDeptId">オペレータ識別ID</param>
		/// <param name="loginOperatorName">オペレータ名</param>
		/// <param name="shouldRestoreExpiredPoint">期限切れのポイントを戻すか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ポイント戻しされたか</returns>
		private static bool CancelUserPointUse(
			OrderModel order,
			string loginOperatorDeptId,
			string loginOperatorName,
			bool shouldRestoreExpiredPoint,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			// 利用ポイントが0より大きい場合、利用ポイント戻し
			if (order.OrderPointUse > 0)
			{
				var isSuccess = new PointService().CancelUsedPointForBuy(
					order.UserId,
					loginOperatorDeptId,
					order.OrderPointUse,
					order.OrderId,
					loginOperatorName,
					shouldRestoreExpiredPoint,
					updateHistoryAction,
					sqlAccessor);

				if (isSuccess == false)
				{
					throw new Exception("ユーザー利用ポイントの戻しに失敗。更新件数が0件。多重更新の場合は楽観ロックにより利用ポイント戻しの更新件数0件となる。");
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// ユーザー付与ポイント戻し
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="loginOperatorDeptId">オペレータ識別ID</param>
		/// <param name="loginOperatorName">オペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ポイント戻しされたか</returns>
		private static bool CancelUserPointAdd(
			DataRowView order,
			string loginOperatorDeptId,
			string loginOperatorName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			return CancelUserPointAdd(
				new OrderModel(order),
				loginOperatorDeptId,
				loginOperatorName,
				updateHistoryAction,
				sqlAccessor);
		}
		/// <summary>
		/// ユーザー付与ポイント戻し
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="loginOperatorDeptId">オペレータ識別ID</param>
		/// <param name="loginOperatorName">オペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ポイント戻しされたか</returns>
		private static bool CancelUserPointAdd(
			OrderModel order,
			string loginOperatorDeptId,
			string loginOperatorName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			// 付与ポイントが0より大きい場合、付与ポイント戻し
			if (order.OrderPointAdd > 0)
			{
				var sv = new PointService();
				var isSuccess = sv.CancelAddedPointForBuy(
					order.UserId,
					loginOperatorDeptId,
					order.OrderPointAdd,
					order.OrderId,
					loginOperatorName,
					updateHistoryAction,
					sqlAccessor);

				if (isSuccess == false)
				{
					throw new Exception("ユーザー付与ポイント戻しに失敗。更新件数が0件。多重更新の場合は楽観ロックにより付与ポイント戻しの更新件数0件となる。");
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// ユーザー利用クーポン戻し
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="user">ユーザー情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="orderCombineCoupon">注文同梱で生成された新注文が利用するクーポン</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>クーポン戻しされたか</returns>
		private static bool CancelUserCouponUse(
			OrderModel order,
			UserModel user,
			string lastChanged,
			CartCoupon orderCombineCoupon,
			SqlAccessor sqlAccessor,
			UpdateHistoryAction updateHistoryAction)
		{
			var updated = false;
			var couponService = new CouponService();
			// １回利用クーポンの場合
			var coupon = order.Coupons.First();
			if(CouponOptionUtility.IsCouponLimit(coupon.CouponType))
			{
				// 注文同梱で生成された新注文にそのクーポンを利用していた場合、「利用済み→未使用」に更新しない
				if((orderCombineCoupon == null) || (orderCombineCoupon.CouponId != coupon.CouponId))
				{
					updated = couponService.UpdateUserCouponUseFlg(
						order.UserId,
						coupon.DeptId,
						coupon.CouponId,
						coupon.CouponNo,
						false,
						DateTime.Now,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						sqlAccessor);
				}
			}
			// 回数制限クーポンを使った場合、利用回数を戻す
			else if (CouponOptionUtility.IsCouponAllLimit(coupon.CouponType))
			{
				updated = couponService.UpdateCouponCountUp(
					coupon.DeptId,
					coupon.CouponId,
					coupon.CouponCode,
					lastChanged,
					sqlAccessor);
			}
			// ブラックリスト型クーポンを使った場合、利用済み情報を削除
			else if (CouponOptionUtility.IsBlacklistCoupon(coupon.CouponType))
			{
				updated = (couponService.DeleteCouponUseUserByOrderId(
					coupon.CouponId,
					order.OrderId,
					sqlAccessor) > 0);
			}
			// 会員限定回数制限ありクーポンを使った場合、利用回数を戻す
			else if (CouponOptionUtility.IsCouponLimitedForRegisteredUser(coupon.CouponType))
			{
				CouponOptionUtility.UpdateUserCouponCount(
					coupon.DeptId,
					user.UserId,
					coupon.CouponId,
					coupon.CouponNo,
					sqlAccessor,
					true);
			}
			
			// ユーザクーポン履歴登録(利用クーポン)
			couponService.InsertUserCouponHistory(
				order.UserId,
				order.OrderId,
				coupon.DeptId,
				coupon.CouponId,
				coupon.CouponCode,
				Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE_CANCEL,
				Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
				1,
				order.OrderCouponUse*-1,
				lastChanged,
				sqlAccessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(order.OrderId, lastChanged, sqlAccessor);
				new UpdateHistoryService().InsertForUser(order.UserId, lastChanged, sqlAccessor);
			}

			return updated;
		}

		/// <summary>
		/// ユーザー付与クーポン戻し
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>クーポン戻しされたか</returns>
		private static bool CancelUserCouponAdd(
			OrderModel order,
			string deptId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			// ユーザクーポン履歴登録(発行クーポン)
			// ※削除処理前に履歴登録を行う
			InsertUserCouponHistoryForCancel(order, deptId, lastChanged, sqlAccessor);

			// 注文で発行されたクーポンを削除
			return DeleteUserCoupon(order, lastChanged, updateHistoryAction, sqlAccessor);
		}

		/// <summary>
		/// キャンセル向けユーザークーポン履歴挿入
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="loginOperatorDeptId">オペレータ識別ID</param>
		/// <param name="loginOperatorName">オペレータ名</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private static void InsertUserCouponHistoryForCancel(OrderModel order, string loginOperatorDeptId, string loginOperatorName, SqlAccessor sqlAccessor)
		{
			var couponService = new CouponService();
			// 発行クーポン取得
			var publishedCoupons = couponService.GetOrderPublishUserCoupon(
				loginOperatorDeptId,
				order.UserId,
				order.OrderId,
				sqlAccessor);

			// 発行クーポンに対して履歴挿入
			foreach (var coupon in publishedCoupons)
			{
				couponService.InsertUserCouponHistory(
					order.UserId,
					order.OrderId,
					coupon.DeptId,
					coupon.CouponId,
					coupon.CouponCode,
					Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH_CANCEL,
					Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
					-1,
					((coupon.DiscountPrice != null) ? (coupon.DiscountPrice.GetValueOrDefault()) : 0) * -1,
					loginOperatorName,
					sqlAccessor);
			}
		}

		/// <summary>
		/// ユーザークーポン削除
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private static bool DeleteUserCoupon(
			OrderModel order,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var result =
				new CouponService().DeleteUserCouponByOrderId(
					order.UserId,
					order.OrderId,
					lastChanged,
					updateHistoryAction,
					accessor);
			return (result > 0);
		}

		/// <summary>
		/// 注文シリアルキー戻し
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="loginOperatorName">オペレータ名</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>シリアルキー戻しされたか</returns>
		/// <remarks>
		/// @引当済 -> 引当リセット（再引き当て可能にする）
		/// @引渡済 -> キャンセル（使用不可にする）
		/// </remarks>
		private static bool CancelOrderSerialKey(OrderModel order, string loginOperatorName, SqlAccessor sqlAccessor)
		{
			if (order.IsDigitalContents)
			{
				var updated = new SerialKeyService().UpdateByCancelOrder(
					order.OrderId,
					loginOperatorName,
					sqlAccessor);
				return (updated > 0);
			}
			return false;
		}

		/// <summary>
		/// 注文同梱あり定期購入回数(注文基準)キャンセル更新
		/// </summary>
		/// <param name="order">定期情報</param>
		/// <param name="loginOperatorName">オペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private static void UpdateFixedPurchaseCancelCount(OrderModel order, string loginOperatorName, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var orderIds = order.CombinedOrgOrderIds.Split(',');
			var orderCombines = new OrderService().GetCombinedOrders(orderIds);
			foreach (var orderCombine in orderCombines)
			{
				if (string.IsNullOrEmpty(orderCombine.CombinedOrgOrderIds))
				{
					// 通常注文は対象外(通常注文＋定期注文同梱)
					if (string.IsNullOrEmpty(orderCombine.FixedPurchaseId)) continue;

					// 同梱元注文のキャンセル更新
					new FixedPurchaseService().UpdateForCancelOrder(
						orderCombine.FixedPurchaseId,
						orderCombine.OrderId,
						loginOperatorName,
						updateHistoryAction,
						accessor, 
						false);
				}
				else
				{
					UpdateFixedPurchaseCancelCount(orderCombine, loginOperatorName, updateHistoryAction, accessor);
				}
			}
		}

		/// <summary>
		/// ネクストエンジン受注伝票の更新（キャンセル処理）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>tuple1：エラーメッセージ、tuple2：メール送信可否、tuple3:成功か</returns>
		public static Tuple<string, bool, bool> UpdateNextEngineOrderForCancel(string orderId, SqlAccessor accessor)
		{
			var order = new OrderService().Get(orderId, accessor);
			if (order == null) return Tuple.Create(string.Empty, false, false);

			var result = UpdateNextEngineOrderForCancel(order);
			return result;
		}
		/// <summary>
		/// ネクストエンジン受注伝票の更新（キャンセル処理）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>tuple1：エラーメッセージ、tuple2：メール送信可否、tuple3:成功か</returns>
		public static Tuple<string, bool, bool> UpdateNextEngineOrderForCancel(OrderModel order)
		{
			var mailFlg = false;
			var isSuccess = false;
			if (order == null) return Tuple.Create(string.Empty, mailFlg, isSuccess);

			var accessToken = string.Empty;
			var refreshToken = string.Empty;
			var errorMessage = string.Empty;

			var isAccessTokenValid = NextEngineApi.IsExistsToken(out accessToken, out refreshToken);
			if (isAccessTokenValid == false)
			{
				return Tuple.Create(
					Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_NEXTENGINE_INVALID_ACCESS_TOKEN,
					mailFlg,
					isSuccess);
			}

			// ネクストエンジンから受注情報を取得
			var orderIds = new string[] { order.OrderId };
			var searchOrderRsponse = NextEngineApi.CallSearchOrderApi(
				accessToken,
				refreshToken,
				NextEngineConstants.FLG_WAIT_FLG_IMMEDIATELY,
				NextEngineConstants.FIELDS_SEARCH_ORDER_RESPONSE,
				orderIds);

			switch (searchOrderRsponse.Result)
			{
				case NextEngineConstants.FLG_RESULT_SUCCESS:

					try
					{
						var updateOrderResponse = NextEngineApi.CallUpdateOrderApiForCancell(
							accessToken,
							refreshToken,
							NextEngineConstants.FLG_WAIT_FLG_AVOID_FAILURE,
							searchOrderRsponse.Data[0].NEOrderId,
							searchOrderRsponse.Data[0].LastModifiedDate,
							NextEngineOrderModel.GetOrderInfo(order, order.Items[0], order.Shippings[0]));

						isSuccess = updateOrderResponse.Result == NextEngineConstants.FLG_RESULT_SUCCESS;
						if (updateOrderResponse.Result != NextEngineConstants.FLG_RESULT_SUCCESS)
						{
							errorMessage = updateOrderResponse.Message;
						}
						break;
					}
					catch
					{
						// 外でエラーを吐かせるため、ここではエラーを吐かない
						mailFlg = (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED);
						break;
					}

				case NextEngineConstants.FLG_RESULT_ERROR:
				case NextEngineConstants.FLG_RESULT_REDIRECT:
					errorMessage = searchOrderRsponse.Message;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return Tuple.Create(errorMessage, mailFlg, isSuccess);
		}

		/// <summary>
		/// Update next engine order for bulk cancel
		/// </summary>
		/// <param name="orderIds">Order ids</param>
		/// <returns>Failure cancel order ids</returns>
		public static IEnumerable<string> UpdateNextEngineOrderForBulkCancel(IEnumerable<string> orderIds)
		{
			if (orderIds.Any() == false) return orderIds;

			string accessToken, refreshToken;
			if (NextEngineApi.IsExistsToken(out accessToken, out refreshToken) == false) return orderIds;

			var searchOrderResponse = NextEngineApi.CallSearchOrderApi(
				accessToken,
				refreshToken,
				NextEngineConstants.FLG_WAIT_FLG_IMMEDIATELY,
				NextEngineConstants.FIELDS_SEARCH_ORDER_RESPONSE,
				orderIds.ToArray());
			if (searchOrderResponse.Result != NextEngineConstants.FLG_RESULT_SUCCESS) return orderIds;

			var searchOrderIds = searchOrderResponse.Data
				.Select(data => data.OrderId)
				.ToArray();
			if (searchOrderIds.Length == 0) return orderIds;

			var failureCancelOrderIds = orderIds
				.Where(orderId =>
					searchOrderIds.Contains(orderId) == false)
				.ToArray();
			switch (searchOrderResponse.Result)
			{
				case NextEngineConstants.FLG_RESULT_SUCCESS:
					var bulkCancelResponse = NextEngineApi.CallBulkUpdateOrderApi(
						accessToken,
						refreshToken,
						NextEngineConstants.FLG_WAIT_FLG_IMMEDIATELY,
						NextEngineOrderModel.GetOrdersForBulkCancel(searchOrderResponse.Data));

					switch (bulkCancelResponse.Result)
					{
						case NextEngineConstants.FLG_RESULT_SUCCESS:
							return failureCancelOrderIds;

						case NextEngineConstants.FLG_RESULT_ERROR:
							if (bulkCancelResponse.Message == null) return orderIds;

							var failureNextEngineOrders = JsonConvert
								.DeserializeObject<NextEngineBulkUpdateOrderMessageApiResponse[]>(
									bulkCancelResponse.Message.ToString());
							var neOrderApiUpdateFail = failureNextEngineOrders
								.Select(order => order.ReceiveOrderId);
							var failureCancelOrderIdsFromBulkResponse = searchOrderResponse.Data
								.Where(order => neOrderApiUpdateFail.Contains(order.NEOrderId))
								.Select(order => order.OrderId);
							return failureCancelOrderIds.Concat(failureCancelOrderIdsFromBulkResponse);
					}
					break;

				case NextEngineConstants.FLG_RESULT_ERROR:
				case NextEngineConstants.FLG_RESULT_REDIRECT:
					return orderIds;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return orderIds;
		}

		/// <summary>
		/// 外部連携決済キャンセル
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>エラーメッセージ</returns>
		public static string CancelExternalCooperationPayment(
			OrderModel order,
			SqlAccessor accessor = null)
		{
			var result = "";
			switch (order.OrderPaymentKbn)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
					result = CancelPaymentCreditCard(
						order.CardTranId,
						order.OrderPriceTotal,
						order.PaymentOrderId,
						order.OrderId,
						accessor);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
					result = CancelPaymentSoftBankKetaiSBPS(order.CardTranId);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
					result = CancelPaymentDocomoKetaiSBPS(order.CardTranId);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
					result = CancelPaymentAuKantanSBPS(order.CardTranId);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS:
					result = CancelPaymentRecruitSBPS(order.CardTranId);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS:
					result = CancelPaymentPaypalSBPS(order.CardTranId);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS:
					result = CancelPaymentRakutenIdSBPS(order.CardTranId);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
					result = CancelPaymentCvsDef(order);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
					result = CancelPaymentAmazonPay(order.CardTranId, order.OrderPriceTotal, order.OrderId, accessor);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
					result = CancelPaymentAmazonPay(order.CardTranId, order.OrderPriceTotal, order.OrderId, accessor);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
					result = CancelPaymentPayPal(order);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY:
					result = CancelPaymentTriLinkAfterPay(order.CardTranId);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
					if (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct)
					{
						result = CancelPaymentPaidyPay(order, accessor);
					}
					if (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
					{
						result = CancelPaymentPaygentPaidy(order, accessor);
					}
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
					result = CancelAtonePayment(order);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
					result = CancelAfteePayment(order);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
					result = CancelPaymentLinePay(order);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
					result = CancelPaymentNPAfterPay(order);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
					result = CancelPaymentEcPay(order, accessor);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
					result = CancelPaymentNewebPay(order, accessor);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
					result = CancelPaymentPayPay(order, accessor);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU:
					result = CancelPaymentBoku(order);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO:
				case Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE:
					result = CancelPaymentGmoPost(order);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA:
					result = CancelPaymentGmoAtokara(order);
					break;
			}

			return result;
		}

		/// <summary>
		/// カード決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <param name="orderPriceTotal">支払い金額合計</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCard(string cardTranId, decimal orderPriceTotal, string paymentOrderId, string orderId, SqlAccessor accessor = null)
		{
			var result = "";
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Zeus:
					result = CancelPaymentCreditCardZeus(cardTranId);
					break;

				case Constants.PaymentCard.SBPS:
					result = CancelPaymentCreditCardSBPS(cardTranId);
					break;

				case Constants.PaymentCard.YamatoKwc:
					result = CancelPaymentCreditCardYamatoKwc(paymentOrderId);
					break;

				case Constants.PaymentCard.Gmo:
					result = CancelPaymentCreditCardGmo(cardTranId, paymentOrderId, orderId, accessor);
					break;

				case Constants.PaymentCard.Zcom:
					result = CancelPaymentCreditCardZcom(paymentOrderId);
					break;
				
				case Constants.PaymentCard.EScott:
					result = CancelPaymentCreditCardEScott(cardTranId, paymentOrderId);
					break;

				case Constants.PaymentCard.VeriTrans:
					result = CancelPaymentCreditCardVeriTrans(paymentOrderId);
					break;
					
				case Constants.PaymentCard.Rakuten:
					result = CancelPaymentCreditCardRakuten(paymentOrderId);
					break;

				case Constants.PaymentCard.Paygent:
					result = CancelPaymentCreditCardPaygent(orderId, cardTranId, accessor);
					break;
			}

			return result;
		}

		/// <summary>
		/// ゼウス決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCardZeus(string cardTranId)
		{
			var result = new ZeusSecureBatchCancelApi().Exec(cardTranId);
			return result.Success ? "" : result.ErrorMessage;
		}

		/// <summary>
		/// ベリトランス決済キャンセル
		/// </summary>
		/// <param name="orderId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCardVeriTrans(string orderId)
		{
			var result = new PaymentVeritransCredit().Cancel(orderId);
			return (result.Mstatus == VeriTransConst.RESULT_STATUS_OK) ? "" : result.MerrMsg;
		}

		/// <summary>
		/// SBPS決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCardSBPS(string cardTranId)
		{
			// 与信後決済の場合は、キャンセル処理をスキップ
			if (string.IsNullOrEmpty(cardTranId)) return "";

			var api = new PaymentSBPSCreditCancelApi();
			var result = api.Exec(cardTranId);
			return result ? "" : api.ResponseData.ResErrMessages;
		}

		/// <summary>
		/// ヤマトKWC決済キャンセル
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCardYamatoKwc(string paymentOrderId)
		{
			var result = new PaymentYamatoKwcCreditCancelApi().Exec(paymentOrderId);
			return result.Success ? "" : result.ErrorInfoForLog;
		}

		/// <summary>
		/// GMO決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCardGmo(string cardTranId, string paymentOrderId, string orderId, SqlAccessor accessor = null)
		{
			// 与信後決済の場合は、キャンセル処理をスキップ
			if (string.IsNullOrEmpty(cardTranId)) return "";

			var api = new PaymentGmoCredit();
			var result = api.Cancel(paymentOrderId, cardTranId, orderId, accessor);

			return result ? "" : api.ErrorMessages;
		}

		/// <summary>
		/// Zcom決済キャンセル
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCardZcom(string paymentOrderId)
		{
			var adp = new ZcomCancelRequestAdapter(paymentOrderId);
			var result = adp.Execute();

			return result.IsSuccessResult() ? "" : result.GetErrorDetailValue();
		}

		/// <summary>
		/// e-SCOTT決済キャンセル
		/// </summary>
		/// <param name="transactionId">決済取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCardEScott(string transactionId, string paymentOrderId)
		{
			// 与信後決済の場合は、キャンセル処理をスキップ
			if (string.IsNullOrEmpty(transactionId)) return string.Empty;

			var adp = EScottProcess1DeleteApi.CreateEScottMaster1DeleteApi(transactionId, paymentOrderId);
			var result = adp.ExecRequest();

			return result.IsSuccess ? string.Empty : result.ResponseMessage;
		}
		
		/// <summary>
		/// Rakuten決済キャンセル
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCardRakuten(string paymentOrderId)
		{
			var request = new RakutenCancelOrRefundRequest
			{
				PaymentId = paymentOrderId
			};

			// Call Rakuten Cancel Api
			var result = RakutenApiFacade.CancelOrRefund(request);
			var errorMessage = string.Empty;
			if (result.ResultType != RakutenConstants.RESULT_TYPE_SUCCESS)
			{
				var creditError = new CreditErrorMessage();
				creditError.SetCreditErrorMessages(Constants.FILE_XML_RAKUTEN_CREDIT_ERROR_MESSAGE);
				var errorList = creditError.GetValueItemArray();
				errorMessage = (errorList.Any(s => s.Value == result.ErrorCode))
					? errorList.First(s => s.Value == result.ErrorCode).Text
					: string.Empty;
			}
			return errorMessage;
		}

		/// <summary>
		/// ペイジェントクレカ決済キャンセル
		/// </summary>
		///<param name="orderId">受注ID</param>
		/// <param name="cardTranId">決済カード取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCreditCardPaygent(string orderId, string cardTranId, SqlAccessor accessor = null)
		{
			var order = new OrderService().Get(orderId, accessor);
			IDictionary apiResult = null;
			// 外部決済ステータスによって判断
			// オーソリ完了：オーソリキャンセル電文
			if (order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP)
			{
				var sendParams = new PaygentApiHeader(PaygentConstants.PAYGENT_APITYPE_CARD_AUTH_CANCEL);
				sendParams.PaymentId = cardTranId;
				apiResult = PaygentApiFacade.SendRequest(sendParams);
			}
			// 売上確定：売上キャンセル電文
			else if (order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
			{
				var sendParams = new PaygentApiHeader(PaygentConstants.PAYGENT_APITYPE_CARD_REALSALE_CANCEL);
				sendParams.PaymentId = cardTranId;
				apiResult = PaygentApiFacade.SendRequest(sendParams);
			}
			var errorMessage = string.Empty;
			// API連携後ペイジェントのレスポンスが異常終了だったらエラーメッセージを返す
			if ((string)apiResult[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_FAILURE)
			{
				errorMessage = "ペイジェントクレジットキャンセルエラー:" + apiResult[PaygentConstants.RESPONSE_DETAIL];
			}
			return errorMessage;
		}

		/// <summary>
		/// ソフトバンク携帯決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentSoftBankKetaiSBPS(string cardTranId)
		{
			var api = new PaymentSBPSCareerSoftbankKetaiCancelApi();
			var result = api.Exec(cardTranId);
			return result ? "" : api.ResponseData.ResErrMessages;
		}

		/// <summary>
		/// ドコモ携帯決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentDocomoKetaiSBPS(string cardTranId)
		{
			var api = new PaymentSBPSCareerDocomoKetaiCancelApi();
			var result = api.Exec(cardTranId);
			return result == false ? "" : api.ResponseData.ResErrMessages;
		}

		/// <summary>
		/// AUかんたん決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentAuKantanSBPS(string cardTranId)
		{
			var api = new PaymentSBPSCareerAuKantanCancelApi();
			var result = api.Exec(cardTranId);
			return result ? "" : api.ResponseData.ResErrMessages;

		}

		/// <summary>
		/// リクルート決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentRecruitSBPS(string cardTranId)
		{
			var api = new PaymentSBPSRecruitCancelApi();
			var result = api.Exec(cardTranId);
			return result ? "" : api.ResponseData.ResErrMessages;
		}

		/// <summary>
		/// Paypal決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentPaypalSBPS(string cardTranId)
		{
			var api = new PaymentSBPSPaypalCancelApi();
			var result = api.Exec(cardTranId);
			return result ? "" : api.ResponseData.ResErrMessages;
		}

		/// <summary>
		/// 楽天ペイ キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentRakutenIdSBPS(string cardTranId)
		{
			var api = new PaymentSBPSRakutenIdCancelApi();
			var result = api.Exec(cardTranId);
			return result ? "" : api.ResponseData.ResErrMessages;
		}

		/// <summary>
		/// コンビニ後払い決済キャンセル
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCvsDef(OrderModel order)
		{
			var result = "";
			switch (Constants.PAYMENT_CVS_DEF_KBN)
			{
				case Constants.PaymentCvsDef.YamatoKa:
					result = CancelPaymentCvsDefYamatoKa(order.PaymentOrderId);
					break;

				case Constants.PaymentCvsDef.Gmo:
					result = CancelPaymentCvsDefGmo(order.CardTranId);
					break;

				case Constants.PaymentCvsDef.Atodene:
					result = CancelPaymentCvsDefAtodene(order.CardTranId);
					break;

				case Constants.PaymentCvsDef.Dsk:
					result = CancelPaymentCvsDefDsk(order);
					break;

				case Constants.PaymentCvsDef.Atobaraicom:
					result = CancelPaymentCvsDefAtobaraicom(order.PaymentOrderId);
					break;

				case Constants.PaymentCvsDef.Score:
					result = CancelPaymentCvsDefScore(order);
					break;

				case Constants.PaymentCvsDef.Veritrans:
					result = CancelPaymentCvsDefVeritrans(order);
					break;
			}
			return result;
		}

		/// <summary>
		/// PayPal決済キャンセル
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentPayPal(OrderModel order)
		{
			var doRefund = ((Constants.PAYPAL_PAYMENT_METHOD == Constants.PayPalPaymentMethod.AUTH_WITH_SUBMIT)
				|| (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				|| order.IsDigitalContents);
			var cancelResult = PayPalUtility.Payment.VoidOrRefund(
				order.CardTranId,
				doRefund);
			if (cancelResult.IsSuccess()) return "";
			return cancelResult.Message;
		}

		/// <summary>
		/// ヤマト後払い決済キャンセル
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCvsDefYamatoKa(string paymentOrderId)
		{
			var api = new PaymentYamatoKaCancelApi();
			var result = api.Exec(paymentOrderId);
			return result ? "" : api.ResponseData.ErrorMessages;
		}

		/// <summary>
		/// GMO後払い決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCvsDefGmo(string cardTranId)
		{
			var request = new GmoRequestOrderModifyCancel
			{
				KindInfo = new KindInfoElement
				{
					UpdateKind = UpdateKindType.OrderCancel
				},
				Buyer = new BuyerElement
				{
					GmoTransactionId = cardTranId
				}
			};

			var result = new GmoDeferredApiFacade().OrderModifyCancel(request);
			if (result.Result == ResultCode.OK)
			{
				return "";
			}
			else
			{
				var err = string.Join("\r\n", result.Errors.Error.Select(e => e.ErrorCode + "：" + e.ErrorMessage).ToArray());
				return err;
			}
		}

		/// <summary>
		/// Atodene後払いキャンセル
		/// </summary>
		/// <param name="cardTranId">お問い合わせ番号</param>
		/// <returns>
		/// エラーメッセージ
		/// 成功時はEmpty
		/// </returns>
		private static string CancelPaymentCvsDefAtodene(string cardTranId)
		{
			// アダプタ生成してAPIたたく
			var adp = new AtodeneCancelTransactionAdapter(cardTranId);
			var res = adp.ExecuteCancel();

			if (res.Result != AtodeneConst.RESULT_OK)
			{
				if ((res.Errors != null) && (res.Errors.Error != null))
				{
					return string.Join("\r\n", res.Errors.Error.Select(e => string.Format("{0}:{1}", e.ErrorCode, e.ErrorMessage)).ToArray());
				}
			}
			return "";
		}

		/// <summary>
		/// DSK後払いキャンセル
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCvsDefDsk(OrderModel order)
		{
			var adapter = new DskDeferredOrderCancelAdapter(order.CardTranId, order.PaymentOrderId, order.LastBilledAmount.ToPriceString());
			var response = adapter.Execute();

			if ((response.IsResultOk == false) && (response.Errors != null) && (response.Errors.Error != null))
			{
				return string.Join("\r\n", response.Errors.Error.Select(e => string.Format("{0}:{1}", e.ErrorCode, e.ErrorMessage)).ToArray());
			}
			return "";
		}

		/// <summary>
		/// 後付款(TriLink後払い)決済キャンセル
		/// </summary>
		/// <param name="cardTranId">決済取引ID</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentTriLinkAfterPay(string cardTranId)
		{
			var cancelResult = TriLinkAfterPayApiFacade.CancelOrder(
				new TriLinkAfterPayCancelRequest(cardTranId));
			if (cancelResult.ResponseResult) return "";
			return cancelResult.Message;
		}

		/// <summary>
		/// Amazon Payキャンセル
		/// </summary>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="orderPriceTotal">支払い金額合計</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentAmazonPay(string cardTranId, Decimal orderPriceTotal, string orderId, SqlAccessor accessor)
		{
			if (Constants.AMAZON_PAYMENT_CV2_ENABLED)
			{
				return CancelPaymentAmazonPayCv2(cardTranId, orderPriceTotal, orderId, accessor);
			}

			var order = new OrderService().Get(orderId, accessor);
			AbstractResponse response = null;

			// 売上確定済みの場合返金、売上確定前の場合キャンセル処理
			if (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
			{
				response = AmazonApiFacade.Refund(cardTranId, orderPriceTotal, orderId + "_" + DateTime.Now.ToString("HHmmssfff"));
			}
			else if (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE)
			{
				response = AmazonApiFacade.CloseAuthorization(cardTranId);
			}

			if (response.GetSuccess() == false)
			{
				return AmazonApiMessageManager.GetErrorMessage(response.GetErrorCode());
			}

			return "";
		}

		/// <summary>
		/// Amazon Pay(CV2)キャンセル
		/// </summary>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="orderPriceTotal">支払い金額合計</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentAmazonPayCv2(
			string cardTranId,
			Decimal orderPriceTotal,
			string orderId,
			SqlAccessor accessor)
		{
			var order = new OrderService().Get(orderId, accessor);
			var result = CancelAmazonpayCv2(cardTranId, orderPriceTotal, order.OnlinePaymentStatus, new AmazonCv2ApiFacade());

			return result;
		}

		/// <summary>
		/// AmazonPay(CV2)キャンセル
		/// </summary>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="orderPriceTotal">支払い金額合計</param>
		/// <param name="onlinePaymentStatus">オンライン決済状況</param>
		/// <param name="facade">Amazonファサード</param>
		/// <returns>エラーメッセージ</returns>
		public static string CancelAmazonpayCv2(string cardTranId, decimal orderPriceTotal, string onlinePaymentStatus, AmazonCv2ApiFacade facade)
		{
			var response = new AmazonPayResponse();
			var errorMessage = string.Empty;
			// 売上確定済みの場合返金、売上確定前の場合キャンセル処理
			if (onlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
			{
				response = facade.CreateRefund(cardTranId, orderPriceTotal);
			}
			else if (onlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE)
			{
				response = facade.CancelCharge(cardTranId);
			}

			if (response.Success == false)
			{
				errorMessage = AmazonCv2ApiFacade.GetErrorCodeAndMessage(response).Message;
			}

			return errorMessage;
		}

		/// <summary>
		/// Cancel Atone Payment
		/// </summary>
		/// <param name="order">Order Model</param>
		/// <returns>Error Message</returns>
		private static string CancelAtonePayment(OrderModel order)
		{
			// Check transaction status
			var response = AtonePaymentApiFacade.GetPayment(order.CardTranId);
			if ((response.Refunds == null) || (response.Refunds.Length == 0))
			{
				response = AtonePaymentApiFacade.RefundPayment(
					order.CardTranId,
					new AtoneRefundPaymentRequest()
					{
						AmountRefund = CurrencyManager.GetSettlementAmount(
							order.LastBilledAmount,
							order.SettlementRate,
							order.SettlementCurrency).ToString("0"),
						DescriptionRefund = string.Empty,
						RefundReason = "キャンセル"
					});
			}

			if (response.IsSuccess == false)
			{
				var atoneCancelMessages = response.Errors.SelectMany(error => error.Messages).ToArray();
				var result = string.Join(",", atoneCancelMessages);
				return result;
			}
			if (response.IsAuthorizationSuccess == false)
			{
				return response.AuthorizationResultNgReasonMessage;
			}
			return string.Empty;
		}

		/// <summary>
		/// Cancel Aftee Payment
		/// </summary>
		/// <param name="order">Order Model</param>
		/// <returns>Error Message</returns>
		private static string CancelAfteePayment(OrderModel order)
		{
			// Check transaction status
			var response = AfteePaymentApiFacade.GetPayment(order.CardTranId);
			// Not refund when already refund
			if (response.Refunds == null)
			{
				response = AfteePaymentApiFacade.RefundPayment(
					order.CardTranId,
					new AfteeRefundPaymentRequest()
					{
						AmountRefund = CurrencyManager.GetSettlementAmount(
							order.LastBilledAmount,
							order.SettlementRate,
							order.SettlementCurrency).ToString("0"),
						DescriptionRefund = string.Empty,
						RefundReason = "キャンセル"
					});
			}

			if (response.IsSuccess) return string.Empty;
			var afteeCancelMessages = response.Errors.SelectMany(error => error.Messages);
			var result = string.Join(",", afteeCancelMessages);
			return result;
		}

		/// <summary>
		/// Score後払い決済キャンセル
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCvsDefScore(OrderModel order)
		{
			var request = new ScoreCancelRequest
			{
				Transaction =
				{
					ShopTransactionId = order.PaymentOrderId,
					NissenTransactionId = order.CardTranId,
					BilledAmount = order.OrderPriceTotal.ToPriceString()
				}
			};

			var result = new ScoreApiFacade().OrderCancel(request);
			if (result.Result == ScoreResult.Ok.ToText()) return string.Empty;

			var err = string.Join("\r\n", result.Errors.ErrorList.Select(e => $"{e.ErrorCode}：{e.ErrorMessage}").ToArray());
			return err;
		}

		/// <summary>
		/// ベリトランス後払い決済キャンセル
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentCvsDefVeritrans(OrderModel order)
		{
			var result = new PaymentVeritransCvsDef().OrderCancel(order.PaymentOrderId);
			if (result.Mstatus == VeriTransConst.RESULT_STATUS_OK) return string.Empty;

			var error = result.Errors != null
				? string.Join("\r\n", result.Errors.Select(e => $"{e.ErrorCode}：{e.ErrorMessage}").ToArray())
				: string.Format("{0}：{1}", result.VResultCode, result.MerrMsg);
			return error;
		}

		/// <summary>
		/// クレジットカード売上確定後キャンセル可能か？
		/// ※再与信可能かの判断に利用している
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル可：true、キャンセル不可：false</returns>
		public static bool IsCancelableForCreditCardSalesCompleteOrder(OrderModel order)
		{
			// 再与信オプション無効？
			if (Constants.PAYMENT_REAUTH_ENABLED == false) return true;

			// 決済種別がクレジットカードではない？
			if (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return true;

			// 外部決済ステータスが売上確定済みではない？
			if (order.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP) return true;

			// 売上確定後〇日過ぎている場合はキャンセル不可
			var externalPaymentAuthDate = order.ExternalPaymentAuthDate.Value.Date;
			var now = DateTime.Now.Date;
			bool isCancelable;
			switch (Constants.PAYMENT_CARD_KBN)
			{
				// GMO：180日以内？
				case Constants.PaymentCard.Gmo:
					isCancelable = (now <= externalPaymentAuthDate.AddDays((int)CancelableDays.Gmo));
					break;
				// ヤマトKWC：90日以内？
				case Constants.PaymentCard.YamatoKwc:
					isCancelable = (now <= externalPaymentAuthDate.AddDays((int)CancelableDays.YamatoKwc));
					break;
				// その他クレカ：不明
				default:
					isCancelable = true;
					break;
			}

			return isCancelable;
		}

		/// <summary>
		/// Cancel Payment Paidy Pay
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Error message</returns>
		public static string CancelPaymentPaidyPay(
			OrderModel order,
			SqlAccessor accessor)
		{
			var result = PaidyPaymentApiFacade.GetPayment(order.PaymentOrderId);
			if (result.HasError)
			{
				return result.GetApiErrorMessages();
			}

			// Close or refund Paidy payment
			if ((result.Payment.Status != Constants.FLG_PAYMENT_PAIDY_API_STATUS_CLOSED)
				&& (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE))
			{
				result = PaidyPaymentApiFacade.ClosePayment(order.PaymentOrderId);
			}
			else if ((order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				&& (string.IsNullOrEmpty(order.CardTranId) == false))
			{
				result = PaidyPaymentApiFacade.RefundPayment(
					order.CardTranId,
					order.PaymentOrderId,
					order.OrderPriceTotal);

				if (result.HasError == false)
				{
					new OrderService().UpdateCardTranId(
					order.OrderId,
					string.Empty,
					order.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				}
			}

			if (result.HasError)
			{
				return result.GetApiErrorMessages();
			}

			return string.Empty;
		}

		/// <summary>
		/// Cancel Payment LINE Pay
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Result Message</returns>
		private static string CancelPaymentLinePay(OrderModel order)
		{
			var cardTranId = order.CardTranId;
			if (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
			{
				var refundResult = LinePayApiFacade.RefundPayment(
					cardTranId,
					order.SettlementAmount,
					new LinePayApiFacade.LinePayLogInfo(order));
				if (refundResult.IsSuccess == false) return refundResult.ReturnMessage;
			}
			else if (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE)
			{
				var cancelResult = LinePayApiFacade.VoidApiPayment(
					cardTranId,
					new LinePayApiFacade.LinePayLogInfo(order));
				if (cancelResult.IsSuccess == false) return cancelResult.ReturnMessage;
			}

			return string.Empty;
		}

		/// <summary>
		/// Cancel Payment NP After Pay
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Error Message</returns>
		public static string CancelPaymentNPAfterPay(OrderModel order)
		{
			var requestData = NPAfterPayUtility.CreateCancelOrGetPaymentRequestData(order.CardTranId);
			var result = NPAfterPayApiFacade.CancelOrder(requestData);
			if (result.IsSuccess == false)
			{
				var isPC = (order.LastChanged == Constants.FLG_LASTCHANGED_USER);
				var apiErrorMessage = result.GetApiErrorMessage(isPC);
				return apiErrorMessage;
			}
			return string.Empty;
		}

		/// <summary>
		/// Cancel Payment Ec Pay
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Error message</returns>
		public static string CancelPaymentEcPay(
			OrderModel order,
			SqlAccessor accessor)
		{
			var request = ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(order, true);
			var response = ECPayApiFacade.CancelRefundAndCapturePayment(request);
			var result = response.IsSuccess
				? string.Empty
				: string.Format("{0} : {1}", response.ReturnCode, response.ReturnMessage);

			new OrderService().UpdateCardTranId(
				order.OrderId,
				response.TradeNo,
				order.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			return result;
		}

		/// <summary>
		/// Cancel Payment NewebPay
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Error message</returns>
		public static string CancelPaymentNewebPay(
			OrderModel order,
			SqlAccessor accessor)
		{
			var requestCancel = NewebPayUtility.CreateCancelRefundCaptureRequest(order, true);
			var resultCancel = NewebPayApiFacade.SendCancelRefundAndCaptureOrder(requestCancel, true);

			var result = resultCancel.IsSuccess
				? string.Empty
				: string.Format(
					"{0} : {1}",
					resultCancel.Response.Status,
					resultCancel.Response.Message);

			new OrderService().UpdateCardTranId(
				order.OrderId,
				resultCancel.Response.TradeNo,
				order.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			return result;
		}

		/// <summary>
		/// Cancel Payment PayPay
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Error message</returns>
		public static string CancelPaymentPayPay(
			OrderModel order,
			SqlAccessor accessor)
		{
			var errorMessage = string.Empty;
			var isCanceled = false;
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case Constants.PaymentPayPayKbn.SBPS:
					var cancelApi = new PaymentSBPSPaypayCancelApi();
					isCanceled = cancelApi.Exec(
						order.CardTranId,
						order.OrderPriceTotal);

					errorMessage = (isCanceled)
						? string.Empty
						: string.Format(
							"{0} : {1}",
							cancelApi.ResponseData.ResErrCode,
							cancelApi.ResponseData.ResErrMessages);
					break;

				case Constants.PaymentPayPayKbn.GMO:
					var resultCancel = new PaypayGmoFacade().CancelPayment(order);

					isCanceled = (resultCancel.Result == Results.Success);
					errorMessage = (isCanceled)
						? string.Empty
						: string.Format(
							"{0} : {1}",
							resultCancel.Status,
							resultCancel.ErrorMessage);
					break;

				case Constants.PaymentPayPayKbn.VeriTrans:
					var paymentVeritransPaypayApi = new PaymentVeritransPaypay();
					var response = (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
						? (IResponseDto)paymentVeritransPaypayApi.Refund(order.PaymentOrderId, order.LastBilledAmount)
						: (IResponseDto)paymentVeritransPaypayApi.Cancel(order.PaymentOrderId);
					isCanceled = response.Mstatus == VeriTransConst.RESULT_STATUS_OK;
					errorMessage = isCanceled
						? string.Empty
						: LogCreator.CreateErrorMessage(response.VResultCode, response.MerrMsg);
					break;
			}

			PaymentFileLogger.WritePaymentLog(
				isCanceled,
				order.PaymentName ?? string.Empty,
				PaymentFileLogger.PaymentType.PayPay,
				PaymentFileLogger.PaymentProcessingType.CancelPayment,
				LogCreator.CrateMessageWithCardTranId(
					order.CardTranId ?? order.CardTranId,
					string.Empty) + string.Format("\tpaymentName : {0}", order.PaymentName));

			if (isCanceled)
			{
				var updated = DomainFacade.Instance.OrderService.Modify(
				order.OrderId,
				model =>
				{
					model.OnlineDeliveryStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED;
					model.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
				},
				UpdateHistoryAction.DoNotInsert,
				accessor);
			}

			return errorMessage;
		}

		/// <summary>
		/// Atobaraicom後払いキャンセル
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>応答メッセージ</returns>
		private static string CancelPaymentCvsDefAtobaraicom(string paymentOrderId)
		{
			var api = new AtobaraicomCancelationApi();
			var result = api.ExecCancel(paymentOrderId);
			return result ? string.Empty : api.ResponseMessage;
		}

		/// <summary>
		/// Cancel payment boku
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Error message</returns>
		public static string CancelPaymentBoku(OrderModel order)
		{
			var refundId = string.Format("refund{0}", order.CardTranId);
			var refund = new PaymentBokuRefundChargeApi().Exec(
				null,
				order.CardTranId,
				refundId,
				BokuConstants.CONST_BOKU_REASON_CODE_GOOD_WILL,
				null,
				(order.OrderTaxIncludedFlg == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX));

			if (refund == null)
			{
				return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
			}
			else if ((refund.IsSuccess == false)
				|| (refund.RefundStatus == BokuConstants.CONST_BOKU_REFUND_STATUS_FAILED))
			{
				return string.Format(
					"{0} : {1}",
					refund.Result.Status,
					refund.Result.Message);
			}

			if (order.IsFixedPurchaseOrder == false)
			{
				var cancel = new PaymentBokuCancelOptinApi().Exec(order.PaymentOrderId);
				if ((cancel == null) || (cancel.IsSuccess == false))
				{
					return (cancel == null)
						? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR)
						: string.Format(
							"{0} : {1}",
							cancel.Result.Status,
							cancel.Result.Message);
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// GMO掛け払いキャンセル
		/// </summary>
		/// <param name="order">受注情報</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentGmoPost(OrderModel order)
		{
			// Api確認
			var transactionAPi = new GmoTransactionApi();
			var requestGmo = new Payment.GMO.TransactionModifyCancel.GmoRequestTransactionModifyCancel(order);
			requestGmo.KindInfo.UpdateKind = UpdateKindType.OrderCancel;
			var responseGmo = transactionAPi.TransactionModifyCancel(requestGmo);
			// GmoレスポンスがNGの場合エラーメッセージ取得
			return (responseGmo.Result == ResultCode.NG)
				? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_STATUS_UPDATE_NG)
				: string.Empty;
		}

		/// <summary>
		/// GMOアトカラ キャンセル
		/// </summary>
		/// <param name="order">受注情報</param>
		/// <returns>エラーメッセージ</returns>
		private static string CancelPaymentGmoAtokara(OrderModel order)
		{
			var cancelApi = new PaymentGmoAtokaraCancelApi();
			var apiResult = cancelApi.Exec(PaymentGmoAtokaraTypes.UpdateKind.Cancel, order);

			return apiResult == false
				? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_STATUS_UPDATE_NG)
				: string.Empty;
		}

		/// <summary>
		/// Paidy(Paygent)のキャンセル
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQL アクセサー</param>
		/// <returns>Error message</returns>
		public static string CancelPaymentPaygentPaidy(
			OrderModel order,
			SqlAccessor accessor = null)
		{
			PaidyAuthorizationCancellationResult paidyAuthorizationCancellationResult;
			PaidyRefundResult paidyRefundResult;
			if (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE)
			{
				paidyAuthorizationCancellationResult = new PaygentApiFacade().PaidyAuthorizationCancellation(order.CardTranId);

				if (paidyAuthorizationCancellationResult.IsSuccess == false)
				{
					return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CANCEL_PAYMENT_FAILED);
				}
			}
			else if ((order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				&& (string.IsNullOrEmpty(order.CardTranId) == false))
			{
				paidyRefundResult = new PaygentApiFacade().PaidyRefund(
					order.CardTranId,
					order.OrderPriceTotal);

				if (paidyRefundResult.IsSuccess == false)
				{
					return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CANCEL_PAYMENT_FAILED);
				}
			}

			return string.Empty;
		}
	}
}
