/*
=========================================================================================================
  Module      : 注文取り込み(新規注文)クラス(ImportActionNewOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.CrossPoint.Helper;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order.Import.OrderImport.Entity;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Atobaraicom.Shipping;
using w2.App.Common.Order.Register;
using w2.App.Common.User;
using w2.Common;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.Order;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Order.Import.OrderImport.ImportAction
{
	/// <summary>
	/// 注文取り込み(新規注文)
	/// </summary>
	public class ImportActionNewOrder : ImportActionBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderData">取り込み注文データ</param>
		public ImportActionNewOrder(OrderData orderData)
			: base(orderData)
		{
		}

		/// <summary>
		/// 取り込み処理
		/// </summary>
		public override void Import()
		{
			// 商品同梱
			if (this.IsApplyProductBundle)
			{
				AddProductBundle();
			}

			SetDefaultValue();

			// 取り込み対象ではないデータの場合
			if (CheckImportableData() == false)
			{
				return;
			}

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				try
				{
					// ユーザー登録
					if (this.ImportData.UserOrg == null)
					{
						new UserService().Insert(this.ImportData.User, UpdateHistoryAction.DoNotInsert, accessor);
						// 新規登録の場合はクロスポイント側にも登録
						if (Constants.CROSS_POINT_OPTION_ENABLED)
						{
							// クロスポイント側にユーザー情報を登録
							var apiResult = new CrossPointUserApiService().Insert(this.ImportData.User);

							if (apiResult.IsSuccess == false)
							{
								var errorMessage = apiResult.ErrorCodeList.Contains(
										Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
									? apiResult.ErrorMessage
									: MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

								throw new w2Exception(errorMessage);
							}
						}
					}

					// 注文登録
					var orderService = new OrderService();
					orderService.InsertOrder(this.ImportData.Order, UpdateHistoryAction.DoNotInsert, accessor);

					// 在庫更新
					ExecProductStock(accessor);

					// ポイント
					if (Constants.W2MP_POINT_OPTION_ENABLED)
					{
						ExecUserPoint(UpdateHistoryAction.DoNotInsert, accessor);
					}

					// 定期台帳登録
					if (this.ImportData.IsRegistFixedPurchase)
					{
						RegistFixedPurchase(UpdateHistoryAction.DoNotInsert, accessor);
						this.FixedPurchaseRegistCount++;
					}
					else if (string.IsNullOrEmpty(this.ImportData.FixedPurchaseId) == false)
					{
						JoinOrderWithFixedPurchase(UpdateHistoryAction.DoNotInsert, accessor);
					}

					// クーポン登録
					if ((Constants.W2MP_COUPON_OPTION_ENABLED)
						&& (string.IsNullOrEmpty(this.ImportData.Coupon.CouponCode) == false))
					{
						var order = new OrderService();
						order.InsertCoupon(this.ImportData.Coupon, UpdateHistoryAction.DoNotInsert, accessor);
					}

					// ユーザーリアルタイム累計購入回数更新
					new UserService().UpdateRealTimeOrderCount(
						this.ImportData.Order.UserId,
						(int)this.ImportData.Order.OrderCountOrder,
						accessor);

					// 履歴
					new UpdateHistoryService().InsertAllForOrder(this.ImportData.Order.OrderId, this.ImportData.Order.LastChanged, accessor);

					// いったんコミット
					accessor.CommitTransaction();

					// 外部決済連携
					ExecExternalPayment(UpdateHistoryAction.Insert);

					// 定期台帳ステータスを通常に更新
					new FixedPurchaseService()
						.UpdateFixedPurchaseStatusTempToNormal(
							this.ImportData.Order.OrderId,
							this.ImportData.Order.FixedPurchaseId,
							this.ImportData.Order.LastChanged,
							UpdateHistoryAction.Insert);

					this.OrderItemImportCount += this.ImportData.CsvOrderData.Count;

				}
				catch (Exception ex)
				{
					AppLogger.WriteError(ex);
					try
					{
						accessor.RollbackTransaction();
					}
					catch (Exception ex2)
					{
						AppLogger.WriteError(ex.ToString(), ex2);
					}
					if (ex is ProductStockException)
					{
						this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_PRODUCTSTOCK_NO_FIND));
					}
					else
					{
						this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_UNEXPECTED_ERROR));
					}
				}
			}
		}

		/// <summary>
		/// 商品同梱追加
		/// </summary>
		private void AddProductBundle()
		{
			var targetCouponCode = this.ImportData.Coupon.CouponCode;
			if (Constants.W2MP_COUPON_OPTION_ENABLED
				&& (string.IsNullOrEmpty(targetCouponCode) == false))
			{
				this.ImportData.Cart.Coupon = new CartCoupon()
				{
					CouponCode = targetCouponCode,
				};
			}

			using (var productBundler = new ProductBundler(
				new List<CartObject>
				{
					this.ImportData.Cart
				},
				this.ImportData.UserId,
				this.ImportData.Order.AdvcodeFirst,
				this.ImportData.Order.AdvcodeNew))
			{
				var itemIndex = this.ImportData.Order.Items.Length;
				// 同梱物が追加されている
				foreach (var cartProduct in this.ImportData.Cart.Items.Where(
					p => (string.IsNullOrEmpty(p.ProductBundleId) == false)))
				{
					var orderItem = cartProduct.CreateOrderItem(
						this.ImportData.Order,
						cartProduct.Count,
						cartProduct.CountSingle,
						1,
						++itemIndex,
						null,
						null);

					this.ImportData.Order.Items = this.ImportData.Order.Items.Concat(new[] { orderItem }).ToArray();
				}

				// Create cart product bundle for update stock process
				this.ProductBundleCartForUpdateStock = this.ImportData.Cart.Copy();
			}
		}

		/// <summary>
		/// 初期値設定
		/// </summary>
		protected override void SetDefaultValue()
		{
			this.ImportData.Order.MallId = Constants.FLG_ORDER_MALL_ID_OWN_SITE;
			this.ImportData.Order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP;
			this.ImportData.Order.OrderItemCount = this.ImportData.Order.Items.Count();
			this.ImportData.Order.OrderProductCount = this.ImportData.Order.Items.Sum(item => item.ItemQuantity);
		}

		/// <summary>
		/// 取り込み可能かチェック
		/// </summary>
		/// <returns>取り込み可能か？</returns>
		private bool CheckImportableData()
		{

			switch (this.ImportData.Order.OrderPaymentKbn)
			{
				//case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT: // クレカ 対象外
				case Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT: // 代引き
				case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY: // 後付款(TriLink後払い)
				case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE: // 銀振（前払い）
				case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF: // 銀振（後払い）
				case Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT: // 決済なし
				case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY: // NP後払い
				case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF: // コンビニ（後払い）
					return true;

				case Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF: // ヤマト後払いSMS認証連携
					this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DISABLE_PAYMENT)
						.Replace("@@ 1 @@", this.ImportData.Order.OrderPaymentKbn));
					return false;
				default:
					this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DISABLE_PAYMENT)
						.Replace("@@ 1 @@", this.ImportData.Order.OrderPaymentKbn));
					return false;
			}
		}

		/// <summary>
		/// ポイント登録
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセッサ</param>
		private void ExecUserPoint(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var order = this.ImportData.Order;
			var cart = this.ImportData.Cart;
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
				{ Constants.FIELD_ORDER_LAST_CHANGED, order.LastChanged },
				{ Constants.FIELD_ORDER_ORDER_POINT_ADD, order.OrderPointAdd }
			};

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				ht.Add(Constants.FIELD_ORDER_ORDER_DATE, this.ImportData.Order.OrderDate);
			}

			var success = (OrderCommon.AddUserPoint(
				ht,
				cart,
				Constants.FLG_POINTRULE_POINT_INC_KBN_BUY,
				updateHistoryAction,
				accessor) >= 0);

			if (Constants.CROSS_POINT_OPTION_ENABLED == false) return;

			var user = DomainFacade.Instance.UserService.Get(cart.OrderUserId, accessor);
			UserUtility.AdjustPointAndMemberRankByCrossPointApi(user, accessor);
		}

		/// <summary>
		/// 在庫引き当て
		/// </summary>
		/// <param name="accessor">アクセッサ</param>
		private void ExecProductStock(SqlAccessor accessor)
		{
			var order = this.ImportData.Order;
			var cart = this.IsApplyProductBundle
				? this.ProductBundleCartForUpdateStock
				: this.ImportData.Cart;
			var ht = new Hashtable 
			{
				{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
				{ Constants.FIELD_ORDER_LAST_CHANGED, order.LastChanged },
			};

			var success = OrderCommon.UpdateProductStock(ht, cart, true, accessor);

			if (success)
			{
				OrderCommon.InsertProductStockHistory((string)ht[Constants.FIELD_ORDER_ORDER_ID], cart, true, accessor);
			}
		}

		/// <summary>
		/// 定期台帳登録処理
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション種別</param>
		/// <param name="accessor">Sqlアクセサー</param>
		private void RegistFixedPurchase(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			this.ImportData.Order.DataSource.Add(
				Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_ORDER_COUNT,
				this.ImportData.SubscriptionBoxOrderCount);
			var fixedPurchaseId = new FixedPurchaseRegister(
				this.ImportData.Order.DataSource,
				this.ImportData.Cart,
				this.ImportData.Order.LastChanged)
			.RegisterAndUpdateFixedPurchaseInfoForImportOrderFile(
				this.ImportData.FixedPurchaseId,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_TEMP,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			if ((Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE == false)
				&& Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				var fixedPurchaseMemberFlg = ((this.ImportData.Cart.IsFixedPurchaseMember || this.ImportData.Cart.HasFixedPurchase)
					? Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON
					: Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF);

				new UserService().UpdateFixedPurchaseMemberFlg(
					this.ImportData.Order.UserId,
					fixedPurchaseMemberFlg,
					this.ImportData.Order.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(
					this.ImportData.Order.OrderId,
					this.ImportData.Order.LastChanged,
					accessor);
			}
		}

		/// <summary>
		/// 注文を定期台帳の子注文として紐づける
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション種別</param>
		/// <param name="accessor">sqlアクセサー</param>
		private void JoinOrderWithFixedPurchase(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var fixedPurchaseService = new FixedPurchaseService();
			var fixedPurchase = fixedPurchaseService.Get(this.ImportData.FixedPurchaseId, accessor);
			var shopShippingService = new ShopShippingService();
			var shopShipping = shopShippingService.Get(
				fixedPurchase.ShopId,
				this.ImportData.Order.ShippingId,
				accessor);
			var orderService = new OrderService();

			// 定期購入ID + 定期購入回数(注文時点)更新
			orderService.UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
				this.ImportData.Order.OrderId,
				fixedPurchase.FixedPurchaseId,
				fixedPurchase.OrderCount + 1,
				this.ImportData.Order.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 注文拡張ステータス更新
			orderService.UpdateOrderExtendStatusForCreateFixedPurchaseOrder(
				this.ImportData.Order.OrderId,
				fixedPurchase,
				this.ImportData.Order.LastChanged,
				Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 注文登録成功更新
			fixedPurchaseService.UpdateForSuccessOrder(
				fixedPurchase.FixedPurchaseId,
				fixedPurchase.NextShippingDate ?? fixedPurchaseService.CalculateNextShippingDate(fixedPurchase.FixedPurchaseKbn,
					fixedPurchase.FixedPurchaseSetting1,
					this.ImportData.Order.OrderShippingDate ?? DateTime.Now.AddDays(shopShipping.FixedPurchaseShippingDaysRequired),
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan,
					NextShippingCalculationMode.Earliest),
				fixedPurchase.NextNextShippingDate,
				this.ImportData.Order.OrderId,
				this.ImportData.Order.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 定期購入継続分析
			new FixedPurchaseRepeatAnalysisService()
				.UpdateRepeatAnalysisFixedPurchaseIdByOrderId(
					this.ImportData.Order.OrderId,
					fixedPurchase.FixedPurchaseId,
					this.ImportData.Order.LastChanged,
					accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(
					this.ImportData.Order.OrderId,
					this.ImportData.Order.LastChanged,
					accessor);
			}
		}

		/// <summary>
		/// 外部決済連携
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		private void ExecExternalPayment(UpdateHistoryAction updateHistoryAction)
		{
			var user = this.ImportData.User;
			var order = this.ImportData.Order;
			var cart = this.ImportData.Cart;
			var ht = new Hashtable 
			{
				{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, order.PaymentOrderId },
				{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
				{ Constants.FIELD_ORDER_USER_ID, order.UserId },
				{ Constants.FIELD_ORDER_PAYMENT_MEMO, order.PaymentMemo },
				{ Constants.FIELD_ORDER_CARD_TRAN_ID, order.CardTranId },
				{ Constants.FIELD_ORDER_LAST_CHANGED, order.LastChanged },
				{ Constants.FIELD_ORDER_FIXED_PURCHASE_ID, this.ImportData.FixedPurchaseId },
			};

			bool needsRollback = false;
			bool isExternalPayment;
			bool success = true;

			// 外部決済で与信がHOLD時に利用(現在はコンビニ後払い(DSK)のみ利用)
			var isExternalPaymentAuthResultHold = false;

			// クレカは連携行かない
			if (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				var properties = new OrderRegisterProperties(OrderRegisterBase.ExecTypes.CommerceManager, user.IsMember, "");
				try
				{
					// 決済実行
					var paymentRegister = new OrderPaymentRegister(properties);
					success = paymentRegister.ExecOrderPayment(
						ht,
						cart,
						out needsRollback,
						out isExternalPayment,
						UpdateHistoryAction.DoNotInsert);
					isExternalPaymentAuthResultHold = paymentRegister.IsAuthResultHold;
				}
				catch (Exception ex)
				{
					// 仮注文で残す
					success = false;
					AppLogger.WriteError(ex);
				}
			}

			if (needsRollback)
			{
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_EXTERNAL_PAYMENT_ERROR));

				// リアルタイム累計購入回数更新
				ht.Add(Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, (new UserService().Get(order.UserId).OrderCountOrderRealtime - 1));
				OrderCommon.RollbackPreOrder(ht, cart, false, 0, this.ImportData.User.IsMember, UpdateHistoryAction.Insert);
			}

			if (success)
			{
				// ステータス更新
				OrderCommon.UpdateForOrderComplete(ht, cart, true, updateHistoryAction, isExternalPaymentAuthResultHold);

				// Exec real sale cvs def
				ExecRealSaleCvsDef(order.OrderId);
			}
			else if (needsRollback == false)
			{
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_EXTERNAL_PAYMENT_ERROR));
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_TEMPORARY_ORDER_ERROR));
			}

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				null,
				order.PaymentName ?? "",
				PaymentFileLogger.PaymentType.Unknown,
				PaymentFileLogger.PaymentProcessingType.OrderCaptureForNewOrder,
				this.ErrorMessage.ToString(),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, order.OrderId},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, order.PaymentOrderId},
					{Constants.FIELD_ORDER_CARD_TRAN_ID, order.CardTranId}
				});
		}

		/// <summary>
		/// Execute real sale cvs def
		/// </summary>
		/// <param name="orderId">Order id</param>
		private void ExecRealSaleCvsDef(string orderId)
		{
			var order = new OrderService().Get(orderId);

			if ((Constants.PAYMENT_CVS_DEF_KBN != Constants.PaymentCvsDef.Atobaraicom)
				|| (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				return;
			}

			var deliveryCompany = new DeliveryCompanyService().Get(order.Shippings[0].DeliveryCompanyId);
			var deliveryId = (deliveryCompany != null)
				? deliveryCompany.DeliveryCompanyTypePostPayment
				: string.Empty;
			var atobaraicomShippingResponse = new AtobaraicomShippingRegistrationApi().ExecShippingRegistration(
				order.PaymentOrderId,
				order.Shippings[0].ShippingCheckNo,
				deliveryId,
				order.InvoiceBundleFlg);

			if (atobaraicomShippingResponse.IsSuccess == false)
			{
				this.ErrorMessage.AppendLine(atobaraicomShippingResponse.ApiMessages);
			}
		}

		/// <summary>商品同梱適用する？</summary>
		private bool IsApplyProductBundle
		{
			get { return (StringUtility.ToEmpty(this.ImportData.CsvOrderData[0]["apply_product_bundle"]) == "1"); }
		}
		/// <summary>Product bundle cart for update stock</summary>
		private CartObject ProductBundleCartForUpdateStock { get; set; }
	}
}
