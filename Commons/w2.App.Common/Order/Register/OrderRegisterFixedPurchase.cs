/*
=========================================================================================================
  Module      : 定期購入向け注文登録クラス(OrderRegisterFixedPurchase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Web;
using System.Web.UI;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order.Cart;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Boku;
using w2.App.Common.Order.Payment.Boku.Response;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.YamatoKa;
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.MailTemplate;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.SubscriptionBox;

namespace w2.App.Common.Order.Register
{
	/// <summary>
	/// 定期購入向け注文登録クラス
	/// </summary>
	public class OrderRegisterFixedPurchase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="canSendFixedPurchaseMailToUser">定期注文登録時にユーザーに注文メール＆定期購入エラーメール送信するか</param>
		/// <param name="canUpdateShippingDate">次回配送日・次々回配送日を更新するか</param>
		/// <param name="fixedPurchaseMailSendTiming">注文完了メールの送信タイミング</param>
		public OrderRegisterFixedPurchase(string lastChanged, bool canSendFixedPurchaseMailToUser, bool canUpdateShippingDate, FixedPurchaseMailSendTiming fixedPurchaseMailSendTiming)
		{
			this.LastChanged = lastChanged;
			this.CanSendFixedPurchaseMailToUser = canSendFixedPurchaseMailToUser;
			this.CanUpdateShippingDate = canUpdateShippingDate;
			this.FixedPurchaseMailSendTiming = fixedPurchaseMailSendTiming;
		}
		#endregion

		#region +定期購入注文登録
		/// <summary>
		/// 定期購入注文登録（今すぐ注文用）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		public ResultMessages ExecByOrderNow(string fixedPurchaseId)
		{
			var fixedPurchase = new FixedPurchaseService().Get(fixedPurchaseId);
			if(OrderCommon.CheckPaymentYamatoKaSms(fixedPurchase.OrderPaymentKbn))fixedPurchase.OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF;
			return this.Exec(
				fixedPurchaseList: new[] { fixedPurchase },
				senderType: Constants.EnabledOrderCompleteEmailSenderType.Manager);
		}

		/// <summary>
		/// 定期購入注文登録（バッチ用）
		/// </summary>
		/// <remarks>対象定期購入情報から定期注文登録</remarks>
		public ResultMessages ExecByBatch()
		{
			this.IsSendMailConvenienceStoreChange = true;
			return this.Exec(
				fixedPurchaseList: new FixedPurchaseService().GetTargetsForCreateOrder(),
				senderType: Constants.EnabledOrderCompleteEmailSenderType.Batch);
		}

		/// <summary>
		/// 定期購入注文登録（マイページ今すぐ注文用）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="devideInfo">デバイス情報</param>
		/// <returns>定期購入向け注文登録インスタンス</returns>
		public OrderRegisterFixedPurchaseInner ExecByOrderNowFromMyPage(string fixedPurchaseId, string devideInfo = null)
		{
			// 定期購入台帳情報取得
			var fixedPurchase = new FixedPurchaseService().Get(fixedPurchaseId);
			fixedPurchase.DeviceInfo = devideInfo;

			// ユーザーが存在しない or 退会済みの場合は定期購入情報を無効
			var user = new UserService().Get(fixedPurchase.UserId);
			if ((user == null) || (user.IsDeleted))
			{
				new FixedPurchaseService().UpdateInvalidate(fixedPurchaseId, this.LastChanged, UpdateHistoryAction.Insert);
				// 定期購入向け注文登録インスタンス作成
				var orderRegisterFail = new OrderRegisterFixedPurchaseInner(
					true,
					this.LastChanged,
					this.CanSendFixedPurchaseMailToUser,
					fixedPurchaseId,
					this.FixedPurchaseMailSendTiming,
					Constants.EnabledOrderCompleteEmailSenderType.Front);
				orderRegisterFail.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_USER_NOT_EXISTS).Replace("@@ 1 @@", fixedPurchase.UserId));
				return orderRegisterFail;
			}
			// 配送会社が有効でない場合は注文を作成しない
			if (CheckFixdPurchaseCompanyId(fixedPurchase, user) == false)
			{
				var orderRegisterFail = new OrderRegisterFixedPurchaseInner(
					true,
					this.LastChanged,
					this.CanSendFixedPurchaseMailToUser,
					fixedPurchaseId,
					this.FixedPurchaseMailSendTiming,
					Constants.EnabledOrderCompleteEmailSenderType.Front);
				orderRegisterFail.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FIXEDPURCHASEORDER_ERROR));
				return orderRegisterFail;
			}

			// Check can execute by order now
			if (CanExecByOrderNow(fixedPurchase))
			{
				var orderRegisterFail = new OrderRegisterFixedPurchaseInner(
					true,
					this.LastChanged,
					this.CanSendFixedPurchaseMailToUser,
					fixedPurchaseId,
					this.FixedPurchaseMailSendTiming,
					Constants.EnabledOrderCompleteEmailSenderType.Front);
				return orderRegisterFail;
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
					user,
					fixedPurchase.NextShippingUsePoint,
					CrossPointUtility.GetValue(
						Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
						Constants.CROSS_POINT_REASON_KBN_POINT_RULE));

				UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
			}

			// 注文登録
			var orderRegister = RegisterOrder(fixedPurchase, user);

			return orderRegister;
		}

		/// <summary>
		/// 注文登録
		/// </summary>
		/// <param name="fixedPurchase">定期購入台帳情報</param>
		/// <param name="user">ユーザー情報</param>
		/// <returns>定期購入向け注文登録インスタンス</returns>
		private OrderRegisterFixedPurchaseInner RegisterOrder(FixedPurchaseModel fixedPurchase, UserModel user)
		{
			// 定期購入向け注文登録インスタンス作成
			var orderRegister = new OrderRegisterFixedPurchaseInner(
				true,
				this.LastChanged,
				this.CanSendFixedPurchaseMailToUser,
				fixedPurchase.FixedPurchaseId,
				this.FixedPurchaseMailSendTiming,
				Constants.EnabledOrderCompleteEmailSenderType.Front);

			// カートリスト作成
			var cartList = CreateCartList(fixedPurchase, user, orderRegister);

			if (cartList.Items.Any() == false)
			{
				// 定期購入失敗更新
				new FixedPurchaseService().UpdateForFailedOrder(
					fixedPurchase.FixedPurchaseId,
					this.LastChanged,
					UpdateHistoryAction.Insert); // この時点で更新履歴作成
				return orderRegister;
			}

			cartList.SetShippingMethod(fixedPurchase);

			// 配送先が配送不可エリアの場合
			var unavailableShipping = cartList.CheckUnavailableShippingArea(cartList);
			if (unavailableShipping)
			{
				// 定期購入ステータスに「配送不可エリア停止」をセット
				orderRegister.FixedPurchaseStatus =
					Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA;

				new FixedPurchaseService().UpdateForFailedUnavailableShippingArea(
					fixedPurchase.FixedPurchaseId,
					this.LastChanged,
					UpdateHistoryAction.Insert);

				orderRegister.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR));
				return orderRegister;
			}

			// 商品同梱処理
			using (var productBundler = new ProductBundler(
				cartList.Items,
				user.UserId,
				user.AdvcodeFirst,
				string.Empty))
			{
				foreach (CartObject cart in productBundler.CartList)
				{
					cart.DeviceInfo = fixedPurchase.DeviceInfo;

					// 注文情報作成
					var orderId = OrderCommon.CreateOrderId(cart.ShopId);
					var order = CreateOrderInfo(fixedPurchase, cart, orderId, user.AdvcodeFirst);

					// 注文情報チェック
					var success = CheckForOrder(order, cart, orderRegister, true);

					// 注文者情報と配送先情報が異なる場合、別出荷フラグを更新
					cart.UpdateAnotherShippingFlag();

					if ((cart.Shippings[0].ConvenienceStoreFlg
							== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						&& (OrderCommon.CheckIdExistsInXmlStoreBatchFixedPurchase(cart.Shippings[0].ConvenienceStoreId) == false)
						&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false))
					{
						SendMailConvenienceStoreChangeToUser(cart);

						// 定期購入ステータスに「その他エラー停止」をセット
						orderRegister.FixedPurchaseStatus =
							Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_FAILED;
						new FixedPurchaseService().UpdateForFailedOrder(
							fixedPurchase.FixedPurchaseId,
							this.LastChanged,
							UpdateHistoryAction.Insert);

						orderRegister.ErrorMessages
							.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CONVENIENCE_STORE_NOT_VALID));

						continue;
					}

					var orderExtendItem = OrderExtendCommon.ConvertOrderExtend(fixedPurchase);
					cart.UpdateOrderExtend(orderExtendItem);

					// LINE Pay
					if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					{
						// Use LINE Pay Pre-Approved API
						cart.IsPreApprovedLinePayPayment = true;
					}

					// 注文登録
					if (success)
					{
						foreach (var shipping in cart.Shippings)
						{
							shipping.CalculateScheduledShippingDate(
								fixedPurchase.ShopId,
								shipping.ShippingCountryIsoCode,
								shipping.IsTaiwanCountryShippingEnable
									? shipping.Addr2
									: shipping.Addr1,
								shipping.Zip);
						}

						var result = orderRegister.Exec(order, cart, false, (cartList.Items.IndexOf(cart) == 0));
						success = ((result == OrderRegisterBase.ResultTypes.Success)
							|| (result == OrderRegisterBase.ResultTypes.Skip));
					}

					if (success) OrderCommon.UpdateOrderExtendStatus(cart, fixedPurchase);

					// 定期購入ステータス更新
					if (success == false)
					{
						UpdateFixedPurchaseStatus(fixedPurchase.FixedPurchaseId, orderRegister.FixedPurchaseStatus, UpdateHistoryAction.Insert, orderRegister.ApiErrorMessage); // この時点で更新履歴作成
					}
				}

				foreach (var yamatoOrder in orderRegister.YamatoKaSmsOrders)
				{
					var cart = productBundler.CartList.Find(item => (item.OrderId == yamatoOrder.Key));
					var response = ExecutePaymentYamatoKaSms(
						yamatoOrder.Value,
						cart);
					var value = PaymentYamatoKaEntryResponseData.ResultValue.Unavailable;
					Enum.TryParse(response.Result, out value);

					if (value == PaymentYamatoKaEntryResponseData.ResultValue.Available)
					{
						OrderComplete(orderRegister, yamatoOrder.Value, cart);
					}
					else if (value == PaymentYamatoKaEntryResponseData.ResultValue.UnderExamination)
					{
						using (var accessor = new SqlAccessor())
						{
							accessor.OpenConnection();
							accessor.BeginTransaction();

							new OrderService().UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
								(string)yamatoOrder.Value[Constants.FIELD_ORDER_ORDER_ID],
								fixedPurchase.FixedPurchaseId,
								fixedPurchase.OrderCount,
								this.LastChanged,
								UpdateHistoryAction.DoNotInsert,
								accessor);

							accessor.CommitTransaction();
						}
					}
					else
					{
						orderRegister.ErrorMessages.Add(response.ErrorMessages);
						new FixedPurchaseService().UpdateForFailedPayment(
							fixedPurchase.FixedPurchaseId,
							this.LastChanged,
							UpdateHistoryAction.Insert,
							response.ErrorMessages);
					}
				}

				foreach (var bokuOrder in orderRegister.BokuPaymentOrders)
				{
					var cart = productBundler.CartList.Find(item => (item.OrderId == bokuOrder.Key));
					var order = bokuOrder.Value;
					var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
					var paymentMessage = string.Empty;
					var charge = ExecutePaymentBoku(order, cart, out paymentMessage);

					if (charge == null)
					{
						OrderCommon.AppendExternalPaymentCooperationLog(
							false,
							orderId,
							LogCreator.CreateMessage(
								orderId,
								StringUtility.ToEmpty(order[Constants.FIELD_FIXEDPURCHASE_EXTERNAL_PAYMENT_AGREEMENT_ID])),
							Constants.FLG_LASTCHANGED_USER,
							UpdateHistoryAction.DoNotInsert);

						orderRegister.ErrorMessages.Add(paymentMessage);
						new FixedPurchaseService().UpdateForFailedPayment(
							fixedPurchase.FixedPurchaseId,
							this.LastChanged,
							UpdateHistoryAction.Insert,
							paymentMessage);
					}
					else
					{
						OrderCommon.AppendExternalPaymentCooperationLog(
							true,
							orderId,
							LogCreator.CreateMessage(
								orderId,
								StringUtility.ToEmpty(order[Constants.FIELD_FIXEDPURCHASE_EXTERNAL_PAYMENT_AGREEMENT_ID])),
							Constants.FLG_LASTCHANGED_USER,
							UpdateHistoryAction.DoNotInsert);

						var paymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
							orderId,
							charge.OptinId,
							StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]),
							charge.ChargeId,
							cart.PriceTotal);
						order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = charge.OptinId;
						order[Constants.FIELD_ORDER_CARD_TRAN_ID] = charge.ChargeId;
						order[Constants.FIELD_ORDER_PAYMENT_MEMO] = paymentMemo;
						OrderComplete(orderRegister, order, cart);
					}
				}

				foreach (var successOrder in orderRegister.SuccessOrders)
				{
					orderRegister.YamatoKaSmsOrders.Remove(
						orderRegister.YamatoKaSmsOrders.Find(
							o => o.Key == (string)successOrder[Constants.FIELD_ORDER_ORDER_ID]));

					orderRegister.BokuPaymentOrders.Remove(
						orderRegister.BokuPaymentOrders.Find(
							o => (o.Key == StringUtility.ToEmpty(successOrder[Constants.FIELD_ORDER_ORDER_ID]))));
				}
			}

			return orderRegister;
		}

		/// <summary>
		/// ヤマト後払いSMS認証決済連携
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>レスポンス</returns>
		public PaymentYamatoKaEntryResponseData ExecutePaymentYamatoKaSms(Hashtable order, CartObject cart)
		{
			var paymentOrderId = (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];
			var shipping = cart.GetShipping();
			var isSms = OrderCommon.CheckPaymentYamatoKaSms((string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);
			var api = new PaymentYamatoKaEntryApi();
			api.Exec(
				paymentOrderId,
				DateTime.Now.ToString("yyyyMMdd"),
				PaymentYamatoKaUtility.CreateYamatoKaShipYmd(cart),
				cart.Owner.Name,
				StringUtility.ToHankakuKatakana(
					StringUtility.ToZenkakuKatakana(cart.Owner.NameKana)), // 全角カナにしてから半角カナへ変換
				cart.Owner.Zip,
				new PaymentYamatoKaAddress(cart.Owner.Addr1, cart.Owner.Addr2, cart.Owner.Addr3, cart.Owner.Addr4),
				cart.Payment.UserCreditCard.CooperationId,
				cart.Owner.MailAddr,
				cart.PriceTotal,
				PaymentYamatoKaUtility.CreateSendDiv(isSms, shipping.AnotherShippingFlag, true),
				PaymentYamatoKaUtility.CreateProductItemList(order, cart),
				shipping.Name,
				shipping.Zip,
				new PaymentYamatoKaAddress(shipping.Addr1, shipping.Addr2, shipping.Addr3, shipping.Addr4),
				shipping.Tel1);

			return api.ResponseData;
		}

		/// <summary>
		/// Execute payment boku
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <param name="errorMessage">Error message</param>
		/// <returns>Boku charge response</returns>
		public PaymentBokuChargeResponse ExecutePaymentBoku(Hashtable order, CartObject cart, out string errorMessage)
		{
			errorMessage = string.Empty;
			var hasError = false;
			var optinId = StringUtility.ToEmpty(order[Constants.FIELD_FIXEDPURCHASE_EXTERNAL_PAYMENT_AGREEMENT_ID]);
			var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
			var productNames = string.Join(
				",",
				cart.Items.Select(item => item.ProductName));
			var remoteAddr = DomainFacade.Instance.UserService.Get(cart.CartUserId).RemoteAddr;
			var charge = new PaymentBokuChargeApi().Exec(
				cart.SettlementCurrency,
				string.Empty,
				productNames,
				orderId,
				optinId,
				cart.PriceTotal.ToString(),
				(TaxCalculationUtility.GetPrescribedOrderTaxIncludedFlag() == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX),
				remoteAddr,
				true,
				true,
				StringUtility.ToEmpty(order[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]),
				StringUtility.ToEmpty(order[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]));

			if (charge == null)
			{
				hasError = true;
				errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
			}
			else if ((charge.IsSuccess == false)
				|| (charge.ChargeStatus != BokuConstants.CONST_BOKU_CHARGE_STATUS_SUCCESS))
			{
				errorMessage = (charge.ChargeStatus == BokuConstants.CONST_BOKU_CHARGE_STATUS_IN_PROGRESS)
					? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_BOKU_PAYMENT_PROCESS_TIME_OUT)
					: charge.Result.Message;
				hasError = true;
			}

			return hasError ? null : charge;
		}

		/// <summary>
		/// 注文完了
		/// </summary>
		/// <param name="orderRegister">注文登録</param>
		/// <param name="order">注文</param>
		/// <param name="cart">カート</param>
		/// <returns>注文が完了したか</returns>
		private bool OrderComplete(OrderRegisterFixedPurchaseInner orderRegister, Hashtable order, CartObject cart)
		{
			// ３．注文確定処理
			// ・ここを正常通過すれば何があっても注文完了。
			var success = orderRegister.UpdateForOrderComplete(
				order,
				cart,
				true,
				UpdateHistoryAction.DoNotInsert);
			if (success == false) return false;

			// 定期中温スキップ回数の更新
			new FixedPurchaseService().ClearSkippedCount((string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
			
			// ４．後処理
			// 注文完了時の処理
			var alertMessage = orderRegister.OrderCompleteProcesses(order, cart, UpdateHistoryAction.DoNotInsert);
			if (string.IsNullOrEmpty(alertMessage) == false) orderRegister.AlertMessages.Add(alertMessage);

			// 注文完了後の処理（更新履歴とともに）
			orderRegister.AfterOrderCompleteProcesses(order, cart, UpdateHistoryAction.Insert);

			// 成功した注文のカートはすぐ削除したいが、ループの中なのであとで削除する
			orderRegister.SuccessOrders.Add(order);
			orderRegister.SuccessCarts.Add(cart);

			// 注文完了フラグを立てる
			cart.IsOrderDone = true;

			// 定期会員になる条件に、定期注文が入金済みになったかの条件を含める場合、入金ステータスに応じて定期会員フラグの更新を行う
			if (success && Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE)
			{
				success = OrderRegisterBase.UpdateFixedPurchaseMemberFlgBySetting(
					order,
					cart,
					this.LastChanged,
					UpdateHistoryAction.DoNotInsert);
				if (success == false) return false;
			}

			// 更新履歴挿入
			new UpdateHistoryService().InsertAllForOrder(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]), this.LastChanged);

			return true;
		}

		/// <summary>
		/// 定期台帳配送会社有効チェック
		/// </summary>
		/// <param name="fixedPurchase">定期購入台帳情報</param>
		/// <param name="user">ユーザー情報</param>
		/// <returns>有効無効</returns>
		private bool CheckFixdPurchaseCompanyId(FixedPurchaseModel fixedPurchase, UserModel user)
		{
			// 商品情報セット
			var fixedPurchaseShipping = fixedPurchase.Shippings[0];
			var deliveryCompanyId = fixedPurchaseShipping.DeliveryCompanyId;
			var shippingMethod = fixedPurchaseShipping.ShippingMethod;
			foreach (var item in fixedPurchaseShipping.Items)
			{
				// 商品情報取得
				var product = ProductCommon.GetProductVariationInfo(
					item.ShopId,
					item.ProductId,
					item.VariationId,
					user.MemberRankId);
				if (product.Count == 0) continue;

				// 定期台帳に紐づく配送会社が配送種別に存在しているか確認
				var shippingType = (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
				var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(shippingType);
				var deliveryCompanyList = (shippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					? shopShipping.CompanyListExpress
					: shopShipping.CompanyListMail;
				if (deliveryCompanyList.Any(list => list.DeliveryCompanyId == deliveryCompanyId) == false) return false;
			}
			return true;
		}

		/// <summary>
		/// 定期購入台帳ステータスを更新
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseStatus">定期購入ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
		private void UpdateFixedPurchaseStatus(
			string fixedPurchaseId,
			string fixedPurchaseStatus,
			UpdateHistoryAction updateHistoryAction,
			string externalPaymentCooperationLog = "")
		{
			var fixedPurchaseService = new FixedPurchaseService();
			switch (fixedPurchaseStatus)
			{
				// 在庫切れ?
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK:
					fixedPurchaseService.UpdateForFailedNoStock(fixedPurchaseId, this.LastChanged, updateHistoryAction);
					break;

				// 決済失敗?
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED:
					fixedPurchaseService.UpdateForFailedPayment(
						fixedPurchaseId, 
						this.LastChanged, 
						updateHistoryAction, 
						externalPaymentCooperationLog ?? String.Empty);
					break;

				// 配送不可エリアエラー?
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA:
					fixedPurchaseService.UpdateForFailedUnavailableShippingArea(fixedPurchaseId, this.LastChanged, updateHistoryAction);
					break;

				// その他エラー?
				default:
					fixedPurchaseService.UpdateForFailedOrder(fixedPurchaseId, this.LastChanged, updateHistoryAction);
					break;
			}
		}

		/// <summary>
		/// 定期購入注文登録
		/// </summary>
		/// <param name="fixedPurchaseList">定期購入情報リスト</param>
		/// <param name="senderType">送信者種別</param>
		/// <param name="isUpdate">Is Update True Or False</param>
		private ResultMessages Exec(
			FixedPurchaseModel[] fixedPurchaseList,
			Constants.EnabledOrderCompleteEmailSenderType senderType,
			bool isUpdate = false)
		{
			var resultMessages = new ResultMessages();

			var successOrderTotal = 0;
			var alertOrderTotal = 0;
			var errorOrderTotal = 0;
			var reportMailBody = new StringBuilder();

			// 定期購入情報ループ開始
			var fixedPurchaseService = new FixedPurchaseService();
			var userService = new UserService();

			foreach (var fixedPurchase in fixedPurchaseList)
			{
				if ((Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false)
					&& (string.IsNullOrEmpty(fixedPurchase.SubscriptionBoxCourseId) == false))
					continue;

				Console.WriteLine(@"定期購入ID:" + fixedPurchase.FixedPurchaseId);

				// ユーザーが存在しない or 退会済みの場合は定期購入情報を無効
				// 次の定期購入情報へ
				if (FixedPurchaseHelper.CheckFixedPurchaseIsInvalidAndInvalidateFixedPurchase(fixedPurchase, this.LastChanged)) continue;

				// 仮クレジットカードであればスキップ
				if (fixedPurchase.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
				{
					continue;
				}

				// Check can execute by order now
				if (CanExecByOrderNow(fixedPurchase))
				{
					continue;
				}

				// 定期購入向け注文登録インスタンス作成
				var user = userService.Get(fixedPurchase.UserId);
				var orderRegister = new OrderRegisterFixedPurchaseInner(
					user.IsMember,
					this.LastChanged,
					this.CanSendFixedPurchaseMailToUser,
					fixedPurchase.FixedPurchaseId,
					this.FixedPurchaseMailSendTiming,
					senderType);

				// 頒布会コースに紐づく定期台帳だが、対象の頒布会が削除されていた場合は定期購入ステータスをその他エラーにして注文を生成しない
				var existSubscriptionBox = CartObject.CheckExistFixedPurchaseLinkSubscriptionBox(
					fixedPurchase.SubscriptionBoxCourseId,
					fixedPurchase.FixedPurchaseId,
					this.LastChanged);

				// 定期台帳に設定されている商品が、紐づいている頒布会コース内の商品に設定されていない場合は定期購入ステータスをその他エラーにして注文を生成しない
				var isValidFixedPurchaseItem = new SubscriptionBoxService().CheckAllItemInSubscriptionBoxItem(
					fixedPurchase.SubscriptionBoxCourseId,
					fixedPurchase.FixedPurchaseId,
					this.LastChanged);

				if ((existSubscriptionBox == false) || (isValidFixedPurchaseItem == false))
				{
					var msg = new List<string> { CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FIXED_PURCHASE_LINK_SUBSCRIPTION_BOX_NOT_EXIST) };
					reportMailBody.Append(CreateFixedPurchaseReportMailForOperator(fixedPurchase, user, msg, reportMailBody));

					errorOrderTotal++;
					continue;
				}

				// カートリスト作成
				CartObjectList cartList = null;
				try
				{
					cartList = CreateCartList(fixedPurchase, user, orderRegister);
				}
				catch (Exception e)
				{
					// カートリスト作成でエラーになった場合
					fixedPurchaseService.UpdateForFailedOrder(
						fixedPurchase.FixedPurchaseId,
						this.LastChanged,
						UpdateHistoryAction.Insert);  // この時点で更新履歴作成

					var msg = new List<string> { e.Message };
					reportMailBody.Append(CreateFixedPurchaseReportMailForOperator(fixedPurchase, user, msg, reportMailBody));

					errorOrderTotal++;
					continue;
				}
				// 配送会社が有効か確認する
				var checkFlag = CheckFixdPurchaseCompanyId(fixedPurchase, user);

				if (cartList == null) continue;

				if ((cartList.Items.Any() == false) || (checkFlag == false))
				{
					// Update For Failed Order
					fixedPurchaseService.UpdateForFailedOrder(
						fixedPurchase.FixedPurchaseId,
						this.LastChanged,
						UpdateHistoryAction.Insert);  // この時点で更新履歴作成

					if (checkFlag == false)
					{
						orderRegister.AlertMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FIXEDPURCHASEORDER_ERROR));
					}

					// Add mail body for operator
					reportMailBody.Append(CreateFixedPurchaseReportMailForOperator(fixedPurchase, user, orderRegister.AlertMessages, reportMailBody));

					errorOrderTotal++;
					continue;
				}

				// 配送方法に変更があった場合だけ更新
				if (isUpdate && (fixedPurchase.Shippings[0].ShippingMethod != cartList.Items[0].Shippings[0].ShippingMethod))
				{
					fixedPurchase.Shippings[0].ShippingMethod = cartList.Items[0].Shippings[0].ShippingMethod;
					fixedPurchaseService.UpdateShipping(
						fixedPurchase.Shippings[0],
						this.LastChanged,
						UpdateHistoryAction.Insert);
				}

				// 商品同梱処理
				using (var productBundler = new ProductBundler(
					cartList.Items,
					user.UserId,
					user.AdvcodeFirst,
					string.Empty))
				{
					foreach (CartObject cart in productBundler.CartList)
					{
						// タスクスケジュールの作成
						if (this.CanSendFixedPurchaseMailToUser)
						{
							this.FixedPurchaseMailSendTiming.SettingTaskSchedule(Constants.FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_MAIL);
						}

						// 商品同梱後のカートを対象に再計算を行う
						cart.CalculatePointFamily();

						if ((cart.Shippings[0].ConvenienceStoreFlg
								== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
							&& (OrderCommon.CheckIdExistsInXmlStoreBatchFixedPurchase(cart.Shippings[0].ConvenienceStoreId) == false)
							&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false))
						{
							if (this.IsSendMailConvenienceStoreChange)
							{
								SendMailConvenienceStoreChangeToUser(cart);
							}

							// 定期購入ステータスに「その他エラー停止」をセット
							orderRegister.FixedPurchaseStatus =
								Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_FAILED;
							fixedPurchaseService.UpdateForFailedOrder(
								fixedPurchase.FixedPurchaseId,
								this.LastChanged,
								UpdateHistoryAction.Insert);

							resultMessages.ErrorMessages
								.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CONVENIENCE_STORE_NOT_VALID));

							continue;
						}

						// 注文情報作成
						var orderId = OrderCommon.CreateOrderId(cart.ShopId);

						var order = CreateOrderInfo(
							fixedPurchase,
							cart,
							orderId,
							user.AdvcodeFirst);

						// 注文情報チェック
						var success = CheckForOrder(order, cart, orderRegister);

						// 注文者情報と配送先情報が異なる場合、別出荷フラグを更新
						cart.UpdateAnotherShippingFlag();

						var orderExtendItem = OrderExtendCommon.ConvertOrderExtend(fixedPurchase);
						cart.UpdateOrderExtend(orderExtendItem);

						// LINE Pay
						if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
						{
							// Use LINE Pay Pre-Approved API
							cart.IsPreApprovedLinePayPayment = true;
						}

						// 注文登録
						if (success)
						{
							foreach (var shipping in cart.Shippings)
							{
								shipping.CalculateScheduledShippingDate(
									fixedPurchase.ShopId,
									shipping.ShippingCountryIsoCode,
									shipping.IsTaiwanCountryShippingEnable
										? shipping.Addr2
										: shipping.Addr1,
									shipping.Zip);
							}

							if (Constants.CROSS_POINT_OPTION_ENABLED
								&& (fixedPurchase.NextShippingUsePoint > 0))
							{
								CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
									user,
									fixedPurchase.NextShippingUsePoint,
									CrossPointUtility.GetValue(
										Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
										Constants.CROSS_POINT_REASON_KBN_POINT_RULE));
							}

							var orderResult = orderRegister.Exec(order, cart, false, (cartList.Items.IndexOf(cart) == 0));
							success = ((orderResult == OrderRegisterBase.ResultTypes.Success)
								|| (orderResult == OrderRegisterBase.ResultTypes.Skip));

							if (Constants.CROSS_POINT_OPTION_ENABLED)
							{
								// Update user info
								UserUtility.AdjustPointAndMemberRankByCrossPointApi(user.UserId);
							}
						}

						if (success)
						{
							Console.WriteLine(@"  -> 成功:" + orderId);
						}
						else
						{
							switch (orderRegister.FixedPurchaseStatus)
							{
								// 在庫切れ?
								case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK:
									fixedPurchaseService.UpdateForFailedNoStock(
										fixedPurchase.FixedPurchaseId,
										this.LastChanged,
										UpdateHistoryAction.Insert);
									Console.WriteLine(@"  -> 失敗（在庫切れ）:" + orderId);
									break;

								// 決済失敗?
								case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED:
									fixedPurchaseService.UpdateForFailedPayment(
										fixedPurchase.FixedPurchaseId,
										this.LastChanged,
										UpdateHistoryAction.Insert,
										orderRegister.ApiErrorMessage ?? "決済失敗");
									Console.WriteLine(@"  -> 失敗（決済失敗）:" + orderId);
									break;

								// 配送不可エリアエラー?
								case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA:
									fixedPurchaseService.UpdateForFailedUnavailableShippingArea(
										fixedPurchase.FixedPurchaseId,
										this.LastChanged,
										UpdateHistoryAction.Insert);
									Console.WriteLine(@"  -> 失敗（配送不可エリア）:" + orderId);
									break;

								// その他エラー?
								default:
									// 定期購入ステータスに「その他エラー停止」をセット
									orderRegister.FixedPurchaseStatus =
										Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_FAILED;
									fixedPurchaseService.UpdateForFailedOrder(
										fixedPurchase.FixedPurchaseId,
										this.LastChanged,
										UpdateHistoryAction.Insert);
									Console.WriteLine(@"  -> 失敗（その他エラー）:" + orderId);
									break;
							}
						}

						// ユーザー向け：定期購入エラーメール送信
						// ユーザーエラーメール送信するか決定（前回も今回も在庫無しエラーならメールを送らない）
						var sendErrorMailToUser = ((success == false)
							&& ((fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK)
								|| (orderRegister.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK)));
						if (sendErrorMailToUser)
						{
							if (this.FixedPurchaseMailSendTiming.TimeZoneStatus == FixedPurchaseMailSendTiming.TimeZoneStatusEnum.Ok)
							{
								var alertMessage = SendFixedPurchaseMailToUser(
									fixedPurchase,
									order,
									cart,
									orderRegister.FixedPurchaseStatus);
								if (alertMessage != "") orderRegister.AlertMessages.Add(alertMessage);
							}
							else
							{
								if (this.CanSendFixedPurchaseMailToUser) this.FixedPurchaseMailSendTiming.InsertFixedPurchaseBatchMailTmpLog(fixedPurchase.FixedPurchaseId, Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ERROR);
							}
						}

						if (orderRegister.AlertMessages.Count > 0)
						{
							alertOrderTotal++;
							resultMessages.AlertMessages.AddRange(orderRegister.AlertMessages);
						}
						if (orderRegister.ErrorMessages.Count > 0)
						{
							errorOrderTotal++;
							resultMessages.ErrorMessages.AddRange(orderRegister.ErrorMessages);
						}

						// 管理者向け：定期購入報告メール文章作成
						reportMailBody.Append(
							CreteFixedPurchaseReportMailForOperator(
								fixedPurchase,
								cart,
								orderRegister.ErrorMessages,
								resultMessages.AlertMessages,
								orderRegister.ApiErrorMessage,
								(cartList.Items.IndexOf(cart) == 0),
								(fixedPurchaseList[fixedPurchaseList.Length - 1] == fixedPurchase),
								success,
								reportMailBody));

						successOrderTotal += orderRegister.SuccessOrders.Count;

						if (success)
						{
							if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED)
							{
								OrderCommon.UpdateOrderExtendStatusByDeliveryCompanyMailEscalation(
									cart,
									fixedPurchase,
									this.DeliveryCompanyMailEscalationOrderExtendNo);
							}
							else
							{
								OrderCommon.UpdateOrderExtendStatus(cart, fixedPurchase);
							}
						}
					}

					foreach (var yamatoOrder in orderRegister.YamatoKaSmsOrders)
					{
						var cart = productBundler.CartList.Find(item => (item.OrderId == yamatoOrder.Key));
						var response = ExecutePaymentYamatoKaSms(
							yamatoOrder.Value,
							cart);
						var value = PaymentYamatoKaEntryResponseData.ResultValue.Unavailable;
						Enum.TryParse(response.Result, out value);

						if (value == PaymentYamatoKaEntryResponseData.ResultValue.Available)
						{
							OrderComplete(orderRegister, yamatoOrder.Value, cart);
						}
						else if (value == PaymentYamatoKaEntryResponseData.ResultValue.UnderExamination)
						{
							using (var accessor = new SqlAccessor())
							{
								accessor.OpenConnection();
								accessor.BeginTransaction();

								new OrderService().UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
									(string)yamatoOrder.Value[Constants.FIELD_ORDER_ORDER_ID],
									fixedPurchase.FixedPurchaseId,
									fixedPurchase.OrderCount,
									this.LastChanged,
									UpdateHistoryAction.DoNotInsert,
									accessor);

								accessor.CommitTransaction();
							}
						}
						else
						{
							orderRegister.ErrorMessages.Add(response.ErrorMessages);
							fixedPurchaseService.UpdateForFailedPayment(
								fixedPurchase.FixedPurchaseId,
								this.LastChanged,
								UpdateHistoryAction.Insert,
								orderRegister.ApiErrorMessage ?? "決済失敗");
							Console.WriteLine(@"  -> 失敗（決済失敗）:" + yamatoOrder.Key);
						}
					}

					foreach (var bokuOrder in orderRegister.BokuPaymentOrders)
					{
						var cart = productBundler.CartList.Find(item => (item.OrderId == bokuOrder.Key));
						var order = bokuOrder.Value;
						var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
						var paymentMessage = string.Empty;
						var charge = ExecutePaymentBoku(order, cart, out paymentMessage);

						if (charge == null)
						{
							OrderCommon.AppendExternalPaymentCooperationLog(
								false,
								orderId,
								LogCreator.CreateMessage(
									orderId,
									StringUtility.ToEmpty(order[Constants.FIELD_FIXEDPURCHASE_EXTERNAL_PAYMENT_AGREEMENT_ID])),
								Constants.FLG_LASTCHANGED_USER,
								UpdateHistoryAction.DoNotInsert);

							orderRegister.ErrorMessages.Add(paymentMessage);
							new FixedPurchaseService().UpdateForFailedPayment(
								fixedPurchase.FixedPurchaseId,
								this.LastChanged,
								UpdateHistoryAction.Insert,
								paymentMessage);
						}
						else
						{
							OrderCommon.AppendExternalPaymentCooperationLog(
								true,
								orderId,
								LogCreator.CreateMessage(
									orderId,
									StringUtility.ToEmpty(order[Constants.FIELD_FIXEDPURCHASE_EXTERNAL_PAYMENT_AGREEMENT_ID])),
								Constants.FLG_LASTCHANGED_USER,
								UpdateHistoryAction.DoNotInsert);

							var paymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
								orderId,
								charge.OptinId,
								StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]),
								charge.ChargeId,
								cart.PriceTotal);
							order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = charge.OptinId;
							order[Constants.FIELD_ORDER_CARD_TRAN_ID] = charge.ChargeId;
							order[Constants.FIELD_ORDER_PAYMENT_MEMO] = paymentMemo;
							OrderComplete(orderRegister, order, cart);
						}
					}

					foreach (var successOrder in orderRegister.SuccessOrders)
					{
						orderRegister.YamatoKaSmsOrders.Remove(
							orderRegister.YamatoKaSmsOrders.Find(
								o => o.Key == (string)successOrder[Constants.FIELD_ORDER_ORDER_ID]));
					}
				}
			}

			// 管理者向け：定期購入報告メール送信
			SendFixedPurchaseMailToOperator(successOrderTotal, alertOrderTotal, errorOrderTotal, reportMailBody.ToString());

			return resultMessages;
		}

		/// <summary>
		/// ヤマト後払いシステムからSMS認証結果受信時処理
		/// </summary>
		/// <param name="data">SMS認証結果通知データ</param>
		public void YamatoKaSmsNoticeRecievedProcess(PaymentYamatoKaSmsResultData data)
		{
			var fixedPurchaseService = new FixedPurchaseService();
			var order = new OrderService().GetOrderByPaymentOrderId(data.PaymentOrderId);
			var fixedPurchase = fixedPurchaseService.Get(order.FixedPurchaseId);
			var cart = CartObject.CreateCartByOrder(order);
			var orderRegister = new OrderRegisterFixedPurchase(
				Constants.FLG_LASTCHANGED_SYSTEM,
				true,
				false,
				this.FixedPurchaseMailSendTiming);
			order.CreditBranchNo = order.CreditBranchNo;

			if (order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP) return;

			if (data.Result != PaymentYamatoKaEntryResponseData.ResultValue.Available)
			{
				fixedPurchaseService.UpdateForFailedPayment(
					fixedPurchase.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_SYSTEM,
					UpdateHistoryAction.Insert);
				orderRegister.SendFixedPurchaseMailToUser(
					fixedPurchase,
					order.DataSource,
					cart,
					Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED);

				return;
			}

			OrderComplete(
				new OrderRegisterFixedPurchaseInner(
					true,
					this.LastChanged,
					this.CanSendFixedPurchaseMailToUser,
					fixedPurchase.FixedPurchaseId,
					this.FixedPurchaseMailSendTiming),
				order.DataSource,
				cart);
		}
		#endregion

		#region -カートリスト作成関連
		/// <summary>
		/// カートリスト作成
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="user">ユーザ情報</param>
		/// <param name="orderRegister">定期購入注文登録</param>
		/// <returns>カートリスト</returns>
		public CartObjectList CreateCartList(FixedPurchaseModel fixedPurchase, UserModel user, OrderRegisterFixedPurchaseInner orderRegister)
		{
			// カートリスト作成
			var cartList = new CartObjectList(user.UserId, fixedPurchase.OrderKbn, false, fixedPurchase.FixedPurchaseId, fixedPurchase:fixedPurchase);
			var cartKbn = string.IsNullOrEmpty(fixedPurchase.SubscriptionBoxCourseId)
				? Constants.AddCartKbn.FixedPurchase
				: Constants.AddCartKbn.SubscriptionBox;

			// 商品情報セット
			var fixedPurchaseShipping = fixedPurchase.Shippings[0];
			foreach (var item in fixedPurchaseShipping.Items)
			{
				// 商品情報取得
				var product = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, user.MemberRankId);
				if (product.Count != 0)
				{
					// 商品セールID取得
					var productSaleId = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]);

					// 既存の定期購入商品に商品付帯情報が存在する場合、商品付帯情報を作成してカート投入
					// HACK:無理やり情報設定しているため、受注格納時やメール送信時にDDL/CheckBoxで加工切り分けする場合は改修が必要
					var productOptionTexts = ProductOptionSettingHelper.GetDisplayProductOptionTexts(item.ProductOptionTexts);
					if (string.IsNullOrEmpty(productOptionTexts) == false)
					{
						ProductOptionSettingList posl;
						if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED)
						{
							// 定期購入商品の付帯情報をデフォルト値としてセットする
							posl = new ProductOptionSettingList(item.ShopId, item.ProductId);
							posl.SetDefaultValueFromProductOptionTexts(productOptionTexts);

							// 付帯情報のデフォルト値設定後にデフォルト値と定期購入商品の付帯情報が一致していなければ追加
							if (posl.GetDisplayProductOptionSettingSelectValues() != productOptionTexts)
							{
								// NOTE:定期購入商品の付帯情報がデフォルト値としてセット出来れば新たに追加しない
								// セット出来ないときは形式無視の可能性があり定期購入時の付帯情報を引き継ぐ必要がある為、新たに追加する
								var pos = new ProductOptionSetting();
								pos.FixedPurchaseSelectedSettingValue = pos.SelectedSettingValue = productOptionTexts;
								posl.Add(pos);
							}
						}
						// 付帯価格OPがついていなかったら定期作成時点の付帯情報を利用する
						else
						{
							posl = new ProductOptionSettingList();
							var pos = new ProductOptionSetting();
							pos.FixedPurchaseSelectedSettingValue = pos.SelectedSettingValue = productOptionTexts;
							posl.Add(pos);
						}

						cartList.AddProduct(
							product[0],
							cartKbn,
							productSaleId,
							item.ItemQuantity,
							posl,
							null,
							user.DispLanguageCode,
							user.DispLanguageLocaleId,
							fixedPurchase.SubscriptionBoxCourseId,
							this.LastChanged,
							fixedPurchase.NextShippingDate.Value);
					}
					else
					{
						cartList.AddProduct(
							product[0],
							cartKbn,
							productSaleId,
							item.ItemQuantity,
							new ProductOptionSettingList(),
							null,
							user.DispLanguageCode,
							user.DispLanguageLocaleId,
							fixedPurchase.SubscriptionBoxCourseId,
							this.LastChanged,
							fixedPurchase.NextShippingDate.Value);
					}
				}
				else
				{
					// 商品が見つからない場合はカートへ追加しない
					orderRegister.AlertMessages.Add(OrderCommon.GetErrorMessage(OrderErrorcode.ProductDeleted, item.VariationId));
				}
			}

			if (cartList.Items.Any() == false) return cartList;

			// 注文の利用ポイントを再調整
			if (Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE
				&& (fixedPurchase.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON))
			{
				if (Constants.W2MP_POINT_OPTION_ENABLED
					&& user.IsMember
					&& Constants.FIXEDPURCHASE_OPTION_ENABLED
					&& Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE
					&& (fixedPurchase.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON))
				{
					AdjustNextShippingUseAllPoint(cartList, fixedPurchase);
				}
			}
			else
			{
				if (Constants.MP_OPTION_ENABLED && user.IsMember && (fixedPurchase.NextShippingUsePoint > 0))
				{
					AdjustNextShippingUsePoint(cartList, fixedPurchase.NextShippingUsePoint);
				}
			}

			// 定期台帳で設定したクーポンを注文に適用
			if (Constants.W2MP_COUPON_OPTION_ENABLED) SetCoupon(cartList, fixedPurchase);

			// 注文者情報セット
			cartList.SetOwner(
				new CartOwner(
					user.UserKbn,
					user.Name,
					user.Name1,
					user.Name2,
					user.NameKana,
					user.NameKana1,
					user.NameKana2,
					user.MailAddr,
					user.MailAddr2,
					user.Zip,
					user.Zip1,
					user.Zip2,
					user.Addr1,
					user.Addr2,
					user.Addr3,
					user.Addr4,
					user.Addr5,
					user.AddrCountryIsoCode,
					user.AddrCountryName,
					user.CompanyName,
					user.CompanyPostName,
					user.Tel1,
					user.Tel1_1,
					user.Tel1_2,
					user.Tel1_3,
					user.Tel2,
					user.Tel2_1,
					user.Tel2_2,
					user.Tel2_3,
					user.IsSendMail,
					user.Sex,
					user.Birth,
					user.AccessCountryIsoCode,
					user.DispLanguageCode,
					user.DispLanguageLocaleId,
					user.DispCurrencyCode,
					user.DispCurrencyLocaleId)
			);

			if (fixedPurchase.Shippings[0].IsMail)
			{
				using (var productBundler = new ProductBundler(
					cartList.Items,
					user.UserId,
					user.AdvcodeFirst,
					string.Empty))
				{
					foreach (var cart in productBundler.CartList)
					{
						// メール便配送サービスエスカレーション機能
						if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED)
						{
							this.DeliveryCompanyMailEscalationOrderExtendNo = string.Empty;
							var deliveryCompanyId = OrderCommon.DeliveryCompanyMailEscalation(
								cart.Items,
								DataCacheControllerFacade.GetShopShippingCacheController().Get(cart.ShippingType).CompanyListMail);
							if (string.IsNullOrEmpty(deliveryCompanyId))
							{
								fixedPurchase.Shippings[0].ShippingMethod =
									Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
								this.DeliveryCompanyMailEscalationOrderExtendNo = Constants.MAIL_ESCALATION_TO_EXPRESS_ORDEREXTENDNO;
							}
							else if (fixedPurchase.Shippings[0].DeliveryCompanyId != deliveryCompanyId)
							{
								fixedPurchase.Shippings[0].DeliveryCompanyId = deliveryCompanyId;
								this.DeliveryCompanyMailEscalationOrderExtendNo = Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ORDEREXTENDNO;
							}
						}
						// メール便で配送可能か確認し、メール便で配送不可の場合宅配便に変更
						// ※配送方法で配送料金が変わることはないため金額の再計算は行わない
						if ((fixedPurchase.Shippings[0].IsMail == false)
							|| (OrderCommon.IsAvailableShippingKbnMail(cart.Items) == false))
						{
							var defaultShopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(cart.ShippingType);
							var defaultExpressDeliveryCompany = new DeliveryCompanyService().Get(
								defaultShopShipping.GetDefaultDeliveryCompany(true).DeliveryCompanyId);
							cart.Shippings[0].UpdateShippingMethod(
								Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS,
								defaultExpressDeliveryCompany.DeliveryCompanyId);
							fixedPurchase.Shippings[0].ShippingMethod =
								Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;

							// 配送方法変更後に有効な配送サービスが設定されていない場合は
							// デフォルトの配送サービスに変更する
							var isValidCompanyId = CheckFixdPurchaseCompanyId(fixedPurchase, user);
							if (isValidCompanyId == false)
							{
								fixedPurchase.Shippings[0].DeliveryCompanyId =
									defaultExpressDeliveryCompany.DeliveryCompanyId;
							}
						}
					}
				}
			}

			// 配送種別取得
			var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(cartList.Items[0].ShippingType);
			var deliveryCompany = new DeliveryCompanyService().Get(fixedPurchase.Shippings[0].DeliveryCompanyId);
			// 配送先情報セット
			var cartShipping = new CartShipping(cartList.Items[0]);
			cartShipping.ShippingMethod = fixedPurchase.Shippings[0].ShippingMethod;
			cartShipping.DeliveryCompanyId = fixedPurchase.Shippings[0].DeliveryCompanyId;
			// 配送先情報：住所
			cartShipping.UpdateShippingAddr(fixedPurchaseShipping);
			cartShipping.ConvenienceStoreId = fixedPurchaseShipping.ShippingReceivingStoreId;
			cartShipping.ConvenienceStoreFlg = fixedPurchaseShipping.ShippingReceivingStoreFlg;
			cartShipping.ShippingReceivingStoreType = fixedPurchaseShipping.ShippingReceivingStoreType;
			// 配送先情報：配送日時
			DateTime? shippingDate = null;
			if (this.CanUpdateShippingDate
				&& (this.LastChanged == Constants.FLG_LASTCHANGED_BATCH)
				&& (cartShipping.IsExpress
				|| (cartShipping.IsMail
					&& Constants.MAIL_SHIPPINGDATE_INCLUDE_LEADTIME)))
			{
				shippingDate =
					fixedPurchase.NextShippingDate.Value > DateTime.Today ? fixedPurchase.NextShippingDate.Value : (DateTime?)null;
			}
			cartShipping.UpdateShippingDateTime(
				true,
				true,
				shippingDate,
				fixedPurchaseShipping.ShippingTime,
				deliveryCompany.GetShippingTimeMessage(fixedPurchaseShipping.ShippingTime));
			// 配送先情報：定期購入情報
			cartShipping.UpdateFixedPurchaseSetting(
				fixedPurchase.FixedPurchaseKbn,
				fixedPurchase.FixedPurchaseSetting1,
				shopShipping.FixedPurchaseShippingDaysRequired,
				shopShipping.FixedPurchaseMinimumShippingSpan);
			// 配送先情報：定期購入次回/次々回配送日
			var nextShippingDate = fixedPurchase.NextShippingDate.Value;
			var nextNextShippingDate = fixedPurchase.NextNextShippingDate.Value;
			if (this.CanUpdateShippingDate)
			{
				nextShippingDate = GetNextShippingDate(fixedPurchase, shopShipping);
				nextNextShippingDate = GetNextNextShippingDate(fixedPurchase, shopShipping);
			}
			cartShipping.UpdateNextShippingDates(nextShippingDate, nextNextShippingDate);
			// 配送方法＆配送会社
			cartShipping.UpdateShippingMethod(fixedPurchaseShipping.ShippingMethod, fixedPurchaseShipping.DeliveryCompanyId);
			foreach (CartObject cart in cartList)
			{
				cart.SetShippingAddressAndShippingDateTime(cartShipping);
				cart.FixedPurchase = fixedPurchase;
				cart.Calculate(false, isShippingChanged: true);
			}
			// Set shipping method for cart
			cartList.SetShippingMethod(fixedPurchase);

			// For case taiwan invoice enable and country shipping address is taiwan
			if (OrderCommon.DisplayTwInvoiceInfo()
				&& cartShipping.IsShippingAddrTw
				&& (fixedPurchase.Invoice != null))
			{
				cartShipping.UpdateInvoice(
					fixedPurchase.Invoice.TwUniformInvoice,
					fixedPurchase.Invoice.TwUniformInvoiceOption1,
					fixedPurchase.Invoice.TwUniformInvoiceOption2,
					fixedPurchase.Invoice.TwCarryType,
					fixedPurchase.Invoice.TwCarryTypeOption);
			}

			// 支払方法セット
			var payment = DataCacheControllerFacade.GetPaymentCacheController().Get(fixedPurchase.OrderPaymentKbn);
			var cartPayment = new CartPayment();
			cartPayment.UpdateCartPayment(
				payment.PaymentId,
				payment.PaymentName,
				StringUtility.ToEmpty(fixedPurchase.CreditBranchNo),
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				fixedPurchase.CardInstallmentsCode,
				"",
				"",
				null,
				cartPayment.IsSamePaymentAsCart1,
				string.Empty);
			if (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				cartList.PayPalCooperationInfo = new PayPalCooperationInfo(
					UserCreditCard.Get(fixedPurchase.UserId, fixedPurchase.CreditBranchNo.Value));
			}
			foreach (CartObject co in cartList)
			{
				// 利用ポイントで0円になる注文の決済種別を強制的に「決済なし」に変更
				co.Payment = (co.PriceCartTotalWithoutPaymentPrice == 0)
					? GetNoPaymentMethod(fixedPurchase.ShopId, cartPayment.IsSamePaymentAsCart1)
					: cartPayment;
			}

			// 注文メモ・注文拡張項目に空文字セット(エラー回避)
			foreach (CartObject cart in cartList)
			{
				cart.OrderMemos = new List<CartOrderMemo>();
				cart.OrderExtend = new Dictionary<string, CartOrderExtendItem>();
			}

			// カート再計算（配送先で送料計算）
			foreach (CartObject cart in cartList)
			{
				// 決済手数料設定
				// 決済手数料は商品合計金額から計算する（ポイント等で割引された金額から計算してはいけない）
				cart.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
					cart.ShopId,
					cart.Payment.PaymentId,
					cart.PriceSubtotal,
					cart.PriceCartTotalWithoutPaymentPrice);

				// カート配送先情報で再計算
				cart.Calculate(false, isPaymentChanged: true);
			}

			//決済金額決定
			foreach (CartObject co in cartList)
			{
				co.SettlementCurrency = CurrencyManager.GetSettlementCurrency(co.Payment.PaymentId);
				co.SettlementRate = CurrencyManager.GetSettlementRate(co.SettlementCurrency);
				co.SettlementAmount = CurrencyManager.GetSettlementAmount(
					co.PriceTotal,
					co.SettlementRate,
					co.SettlementCurrency);
			}

			// 領収書情報設定
			if (Constants.RECEIPT_OPTION_ENABLED)
			{
				cartList.Items.ForEach(
					cart =>
					{
						cart.ReceiptFlg = fixedPurchase.ReceiptFlg;
						cart.ReceiptAddress = fixedPurchase.ReceiptAddress;
						cart.ReceiptProviso = fixedPurchase.ReceiptProviso;
					});
			}

			return cartList;
		}

		/// <summary>
		/// 注文の利用ポイント数を再調整
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="nextShippingUsePoint">次回購入の利用ポイント</param>
		private void AdjustNextShippingUsePoint(CartObjectList cartList, decimal nextShippingUsePoint)
		{
			var cart = cartList.Items[0];
			if (cart != null) // 削除時に取得できないことがある
			{
				var service = new PointService();
				// 利用ポイント数がと注文時の利用可能ポイント数を比較して、小さい値を注文の利用ポイントに設定
				var useablePoint = service.GetUseablePointFromPrice(cart.PointUsablePrice, Constants.FLG_POINT_POINT_KBN_BASE);
				useablePoint = Math.Min(useablePoint, nextShippingUsePoint);
				var pointUsePrice = Math.Min(cart.PointUsablePrice, service.GetOrderPointUsePrice(useablePoint, Constants.FLG_POINT_POINT_KBN_BASE));
				cart.SetUsePoint(useablePoint, pointUsePrice);
				cart.CalculateWithCartShipping();
			}
		}

		/// <summary>
		/// 注文の利用ポイント数を再調整(利用可能ポイントの定期継続利用）
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="fixedPurchase">定期購入モデル</param>
		private void AdjustNextShippingUseAllPoint(CartObjectList cartList, FixedPurchaseModel fixedPurchase)
		{
			var cart = cartList.Items[0];
			if (cart != null)
			{
				var service = new PointService();
				// 利用ポイント数が注文時の利用可能ポイント数を比較して、利用可能なポイント分を利用
				var useablePoint = service.GetUseablePointFromPrice(cart.PointUsablePrice, Constants.FLG_POINT_POINT_KBN_BASE);
				var pointUsePrice = service.GetOrderPointUsePrice(useablePoint, Constants.FLG_POINT_POINT_KBN_BASE);
				var userPoint = PointOptionUtility.GetUserPoint(fixedPurchase.UserId, Constants.FLG_USERPOINT_POINT_KBN_BASE);
				if (userPoint.PointUsable < 0)
				{
					cart.SetUsePoint(0,0);
				}
				else
				{
					if (useablePoint <= userPoint.PointUsable)
					{
						cart.SetUsePoint(useablePoint, pointUsePrice);
					}
					else
					{
						cart.SetUsePoint(userPoint.PointUsable, userPoint.PointUsable);
					}
				}
				cart.CalculateWithCartShipping();
			}
		}

		/// <summary>
		/// 「決済なし」決済種別の取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="isSamePaymentAsCart1">カート１と同じ決済種別かどうか</param>
		/// <returns>「決済なし」の決済種別</returns>
		private CartPayment GetNoPaymentMethod(string shopId, bool isSamePaymentAsCart1)
		{
			var noPayment = DataCacheControllerFacade.GetPaymentCacheController().Get(Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			var noPaymentMethod = new CartPayment();
			noPaymentMethod.UpdateCartPayment(
				noPayment.PaymentId,
				noPayment.PaymentName,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				null,
				isSamePaymentAsCart1,
				string.Empty);
			return noPaymentMethod;
		}

		/// <summary>
		/// 注文前チェック
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="orderRegister">定期購入注文登録</param>
		/// <param name="isOrderNowFromMyPage">マイページ今すぐ注文か</param>
		/// <returns>チェックＯＫか</returns>
		private bool CheckForOrder(Hashtable order, CartObject cart, OrderRegisterFixedPurchaseInner orderRegister, bool isOrderNowFromMyPage = false)
		{
			// 決済種別が継続課金（定期・従量）の対応外のキャリア決済はエラー
			if (OrderExternalPaymentMemoHelper.IsPaymentCareer(cart.Payment.PaymentId)
				&& (OrderCommon.IsUsablePaymentContinuous(cart.Payment.PaymentId) == false))
			{
				orderRegister.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FIXED_PURCHASE_PAYMENT_CAREER_ERROR));
				orderRegister.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED;
				orderRegister.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT;
				// エラー処理
				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage("００．チェック_キャリア決済NG", order, cart) + string.Join(",", orderRegister.ErrorMessages));
				return false;
			}

			// Check Shipping Country Iso Code For Paidy Payment
			if (Constants.GLOBAL_OPTION_ENABLE
				&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				&& cart.Shippings.Any(shipping => (shipping.IsShippingAddrJp == false)))
			{
				orderRegister.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PAIDY_COUNTRY_SHIPPING_NOT_JAPAN_ERROR));
				orderRegister.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED;
				orderRegister.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT;

				// エラー処理
				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage("００．チェック_PAIDY決済NG", order, cart) + string.Join(",", orderRegister.ErrorMessages));
				return false;
			}

			// Check Shipping Country Iso Code For Np After Pay
			if (Constants.GLOBAL_OPTION_ENABLE
				&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				&& ((cart.Owner.IsAddrJp == false)
					|| cart.Shippings.Any(shipping => (shipping.IsShippingAddrJp == false))))
			{
				orderRegister.ErrorMessages.Add(NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3));
				orderRegister.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED;
				orderRegister.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT;

				// エラー処理
				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage("００．チェック_NP後払い決済NG", order, cart) + string.Join(",", orderRegister.ErrorMessages));
				return false;
			}

			// 決済種別金額範囲チェック
			string message;
			var success = OrderCommon.ValidatePaymentPriceRange(cart, cart.Payment.PaymentId, out message);
			if (success == false)
			{
				orderRegister.ErrorMessages.Add(message);
				orderRegister.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED;
				orderRegister.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT;
				// エラー処理
				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage("００．チェック_決済種別金額範囲NG", order, cart) + string.Join(",", orderRegister.ErrorMessages));
				return false;
			}

			// 在庫チェック
			foreach (CartProduct cp in cart)
			{
				if (OrderCommon.CheckProductStockBuyable(cp) == false)
				{
					success = false;
					orderRegister.ErrorMessages.Add(isOrderNowFromMyPage
						? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCT_NO_STOCK_ORDER_NOW_FROM_MYPAGE)
							.Replace("@@ 1 @@", cp.ProductJointName)
						: OrderCommon.GetErrorMessage(OrderErrorcode.ProductNoStock, cp.ProductJointName));
				}
			}

			// エラー処理
			if (success == false)
			{
				orderRegister.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK;
				orderRegister.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_NOSTOCK;
				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage("００．チェック_在庫NG", order, cart) + string.Join(",", orderRegister.ErrorMessages));
				return false;
			}

			// 配送不可エリアチェック
			var unavailableShipping = CheckUnavailableShippingAreaForFixedPurchaseBatch(cart);
			// 定期購入ステータスが「配送不可エリアエラー」かつ今回の注文が配送不可エリアエラー出ない場合、定期購入ステータスを「通常」に更新
			var replace = UpdateFixedPurchaseStatusByShippingArea(orderRegister, unavailableShipping);
			if (replace) return true;

			// 配送不可エリアエラーの場合、定期購入ステータスを「配送不可エリアエラー」にする
			if (unavailableShipping)
			{
				orderRegister.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA;
				orderRegister.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_UNAVAILABLE_SHIPPING_AREA;
				orderRegister.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR));
				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage("００．チェック_配送不可NG", order, cart) + string.Join(",", orderRegister.ErrorMessages));
				return false;
			}

			return true;
		}

		/// <summary>
		/// 配送不可エリアチェック（定期バッチ用）
		/// </summary>
		/// <param name="cart">カート</param>
		/// <returns>配送不可エリアかどうか</returns>
		private static bool CheckUnavailableShippingAreaForFixedPurchaseBatch(CartObject cart)
		{
			// 配送不可エリアチェック
			var unavailableShippingZip = DomainFacade.Instance.ShopShippingService.GetUnavailableShippingZipFromShippingDelivery(
				cart.ShippingType,
				cart.Shippings[0].DeliveryCompanyId);
			var unavailableShipping =
				OrderCommon.CheckUnavailableShippingArea(
					unavailableShippingZip,
					cart.Shippings[0].HyphenlessZip);

			return unavailableShipping;
		}

		/// <summary>
		/// 定期購入ステータスが「配送不可エリアエラー」かつ今回の注文が配送不可エリアエラー出ない場合、定期購入ステータスを「通常」に更新
		/// </summary>
		/// <param name="orderRegister">定期購入注文登録</param>
		/// <param name="unavailableShipping">配送不可郵便番号</param>
		/// <returns>定期購入ステータスが「配送不可エリアエラー」かつ今回の注文が配送不可エリアエラーかどうか</returns>
		private bool UpdateFixedPurchaseStatusByShippingArea(OrderRegisterFixedPurchaseInner orderRegister, bool unavailableShipping)
		{
			if ((orderRegister.FixedPurchasePaymentStatus
				== Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_UNAVAILABLE_SHIPPING_AREA)
				&& (unavailableShipping == false))
			{
				orderRegister.FixedPurchasePaymentStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL;
				orderRegister.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS;
				return true;
			}

			return false;
		}

		/// <summary>
		/// 注文情報作成
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="advcodeFirst">初回広告コード</param>
		/// <returns>注文情報</returns>
		private Hashtable CreateOrderInfo(FixedPurchaseModel fixedPurchase, CartObject cart, string orderId, string advcodeFirst)
		{
			var order = OrderCommon.CreateOrderInfo(
				cart,
				orderId,
				fixedPurchase.UserId,
				"99bc",	// バッチはとりあえずこの値
				"",
				"",
				advcodeFirst,
				"",
				this.LastChanged);

			// 定期購入ID、定期購入管理メモ、配送メモ格納
			order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID] = fixedPurchase.FixedPurchaseId;
			order[Constants.FIELD_ORDER_MANAGEMENT_MEMO] = fixedPurchase.FixedPurchaseManagementMemo;
			order[Constants.FIELD_ORDER_SHIPPING_MEMO] = fixedPurchase.ShippingMemo;
			// メール送信などのため、次回購入の利用ポイント格納
			order[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT] = fixedPurchase.NextShippingUsePoint;
			// Memo
			order[Constants.FIELD_ORDER_MEMO] = fixedPurchase.Memo;
			// 購入回数
			var orderCount = new UserService().Get(fixedPurchase.UserId).OrderCountOrderRealtime;
			order.Add(Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, orderCount);
			// 次回購入利用クーポンID
			order[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_ID] = fixedPurchase.NextShippingUseCouponId;
			// 頒布会購入回数
			order[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT] = fixedPurchase.SubscriptionBoxOrderCount;

			// Set order remote address
			if (HttpContext.Current != null)
			{
				order[Constants.FIELD_ORDER_REMOTE_ADDR] = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
			}
			order[Constants.FIELD_FIXEDPURCHASE_EXTERNAL_PAYMENT_AGREEMENT_ID] = fixedPurchase.ExternalPaymentAgreementId;
			order[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN] = fixedPurchase.FixedPurchaseKbn;
			order[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1] = fixedPurchase.FixedPurchaseSetting1;
			return order;
		}

		/// <summary>
		/// カートにクーポン設定
		/// </summary>
		/// <param name="cartList">カートリスト情報</param>
		/// <param name="fixedPurchase">定期購入モデル</param>
		private void SetCoupon(CartObjectList cartList, FixedPurchaseModel fixedPurchase)
		{
			var cart = cartList.Items[0];
			// 削除時に取得できないことがある
			if ((cart == null) || (fixedPurchase.NextShippingUseCouponDetail == null)) return;

			// クーポン有効性チェック
			var errorMessage = CouponOptionUtility.CheckCouponValidWithCart(
				cart,
				fixedPurchase.NextShippingUseCouponDetail);
			if (string.IsNullOrEmpty(errorMessage) == false) return;

			// 有効な場合はクーポン情報をカート情報に設定
			cart.Coupon = new CartCoupon(fixedPurchase.NextShippingUseCouponDetail);
			cart.SelectedCoupon = fixedPurchase.NextShippingUseCouponDetail.CouponCode;
		}
		#endregion

		#region -配送日計算関連
		/// <summary>
		/// 定期購入次回配送日の取得
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="shopShipping">配送種別情報</param>
		/// <remarks>未来かどうかで、次々回→次回にスライド or 再計算 を切り替え</remarks>
		/// <returns>日付</returns>
		private DateTime GetNextShippingDate(FixedPurchaseModel fixedPurchase, ShopShippingModel shopShipping)
		{
			DateTime dateTime;
			if (CheckFutureShippingDate(fixedPurchase.NextNextShippingDate.Value))
			{
				dateTime = fixedPurchase.NextNextShippingDate.Value;
			}
			else
			{
				var service = new FixedPurchaseService();
				var calculateMode = service.GetCalculationMode(
					fixedPurchase.FixedPurchaseKbn,
					Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
				dateTime =
					service
						.CalculateFollowingShippingDate(
							fixedPurchase.FixedPurchaseKbn,
							fixedPurchase.FixedPurchaseSetting1,
							DateTime.Now,
							shopShipping.FixedPurchaseMinimumShippingSpan,
							calculateMode);
			}
			return dateTime;
		}

		/// <summary>
		/// 定期購入次々回配送日の取得
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="shopShipping">配送種別情報</param>
		/// <remarks>未来かどうかで、次々回→次回にスライド or 再計算 を切り替え</remarks>
		/// <returns>日付</returns>
		private DateTime GetNextNextShippingDate(FixedPurchaseModel fixedPurchase, ShopShippingModel shopShipping)
		{
			DateTime dateTime;
			var service = new FixedPurchaseService();
			var calculateMode = service.GetCalculationMode(
				fixedPurchase.FixedPurchaseKbn,
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
			if (CheckFutureShippingDate(fixedPurchase.NextNextShippingDate.Value))
			{
				dateTime =
					service.CalculateFollowingShippingDate(
						fixedPurchase.FixedPurchaseKbn,
						fixedPurchase.FixedPurchaseSetting1,
						fixedPurchase.NextNextShippingDate.Value,
						shopShipping.FixedPurchaseMinimumShippingSpan,
						calculateMode);
			}
			else
			{
				dateTime =
					service.CalculateNextNextShippingDate(
						fixedPurchase.FixedPurchaseKbn,
						fixedPurchase.FixedPurchaseSetting1,
						DateTime.Now,
						shopShipping.FixedPurchaseShippingDaysRequired,
						shopShipping.FixedPurchaseMinimumShippingSpan,
						calculateMode);
			}
			return dateTime;
		}

		/// <summary>
		/// 配送日が未来か？
		/// </summary>
		/// <param name="shippingDate">配送日</param>
		/// <returns>true:未来</returns>
		private bool CheckFutureShippingDate(DateTime shippingDate)
		{
			return shippingDate > DateTime.Today;
		}
		#endregion

		#region -メール関連
		/// <summary>
		/// 定期購入エラーメールの送信（ユーザ向け）
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="fixedPurchaseStatus">定期購入ステータス</param>
		/// <returns>アラートメッセージ</returns>
		public string SendFixedPurchaseMailToUser(
			FixedPurchaseModel fixedPurchase,
			Hashtable order,
			CartObject cart,
			string fixedPurchaseStatus)
		{
			if (this.CanSendFixedPurchaseMailToUser == false) return "";

			var errorLogMessage = OrderCommon.CreateOrderSuccessAlertLogMessage("定期購入エラーメールの送信（ユーザ向け）", order, cart);

			var input = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchase.FixedPurchaseId },
				{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, fixedPurchaseStatus },
				{ Constants.TAG_FIXED_PURCHASE_MEMO, fixedPurchase.Memo },
				{ Constants.FIELD_ORDEROWNER_OWNER_NAME, cart.Owner.Name },
				{ Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA, cart.Owner.NameKana }
			};

			var sendToPc = false;
			var sendToMobile = false;
			if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
			{
				if (cart.Owner.MailAddr != "") sendToPc = true;
				if (cart.Owner.MailAddr2 != "") sendToMobile = true;
			}
			else
			{
				if (cart.Owner.MailAddr != "") sendToPc = true;
				else if (cart.Owner.MailAddr2 != "") sendToMobile = true;
			}

			var result = true;
			if (sendToPc) result &= SendReportMailToUser(cart.CartUserId, cart.Owner.MailAddr, input, errorLogMessage, cart.Owner.DispLanguageCode, cart.Owner.DispLanguageLocaleId);
			if (sendToMobile) result &= SendReportMailToUser(cart.CartUserId, cart.Owner.MailAddr2, input, errorLogMessage);

			if (result == false) return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_MAILSEND_ALERT);

			return "";
		}

		/// <summary>
		/// ユーザー向けレポートメール送信処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="input">入力パラメタ</param>
		/// <param name="logMessageForError">エラーログ用メッセージ</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>成功したか</returns>
		private bool SendReportMailToUser(string userId, string mailAddr, Hashtable input, string logMessageForError, string languageCode = null, string languageLocaleId = null)
		{
			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_FIXEDPURCHASE_FOR_USER,
				userId,
				input,
				true,
				Constants.MailSendMethod.Auto,
				languageCode,
				languageLocaleId,
				StringUtility.ToEmpty(mailAddr)))
			{
				mailSender.AddTo(StringUtility.ToEmpty(mailAddr));
				if (mailSender.SendMail() == false)
				{
					AppLogger.WriteError(logMessageForError, mailSender.MailSendException);
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Send Mail Convenience Store Change To User
		/// </summary>
		/// <param name="cart">Cart Object</param>
		private void SendMailConvenienceStoreChangeToUser(CartObject cart)
		{
			var service = new MailTemplateService();
			var mailTemplate = service.Get(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_CONVENIENCE_STORE_FOR_USER);
			if (mailTemplate == null) return;

			switch (Constants.RECEIVINGSTORE_TWPELICAN_MAILSENDMETHOD)
			{
				case "1":
					mailTemplate.SmsUseFlg = MailTemplateModel.SMS_USE_FLG_ON;
					mailTemplate.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_NOTSEND;
					break;

				case "2":
					mailTemplate.SmsUseFlg = MailTemplateModel.SMS_USE_FLG_OFF;
					mailTemplate.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_SEND;
					break;

				case "3":
					mailTemplate.SmsUseFlg = MailTemplateModel.SMS_USE_FLG_ON;
					mailTemplate.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_SEND;
					break;
			}
			service.Update(mailTemplate);
			var input = new Hashtable
			{
				{Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME, cart.Shippings[0].Name},
				{Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4, cart.Shippings[0].Addr4},
				{Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1, cart.Shippings[0].Tel1},
				{Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID, cart.Shippings[0].ConvenienceStoreId}
			};

			// Send mail
			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_CONVENIENCE_STORE_FOR_USER,
				cart.CartUserId,
				input,
				true,
				Constants.MailSendMethod.Auto,
				cart.Owner.DispLanguageCode,
				cart.Owner.DispLanguageLocaleId,
				StringUtility.ToEmpty(cart.Owner.MailAddr)))
			{
				mailSender.AddTo(StringUtility.ToEmpty(cart.Owner.MailAddr));
				mailSender.SendMail();
			}
		}

		/// <summary>
		/// 管理者向け定期購入報告メール文章作成処理
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="cart">カート</param>
		/// <param name="errorMessages">エラーメッセージ</param>
		/// <param name="fixedPurchaseAlertMessages">アラートメッセージ</param>
		/// <param name="fixedPurchaseApiErrorMessage">APIエラーメッセージ</param>
		/// <param name="isFirstCart">最初のカートか</param>
		/// <param name="isLastTarget">最後の定期購入か</param>
		/// <param name="success">購入成功か</param>
		/// <returns>メール文章</returns>
		private string CreteFixedPurchaseReportMailForOperator(
			FixedPurchaseModel fixedPurchase,
			CartObject cart,
			List<string> errorMessages,
			List<string> fixedPurchaseAlertMessages,
			string fixedPurchaseApiErrorMessage,
			bool isFirstCart,
			bool isLastTarget,
			bool success,
			StringBuilder currentMessage)
		{
			var body = new StringBuilder();

			if ((currentMessage.Length > 0) && (currentMessage.ToString().EndsWith("---\n") == false))
			{
				body.Append("---\n");
			}

			if (isFirstCart)
			{
				body.Append("■定期購入ID : ").Append(fixedPurchase.FixedPurchaseId).Append("\n");
				body.Append("  ユーザー   : ").Append(cart.Owner.Name).Append("[").Append(cart.OrderUserId).Append("]\n");
			}
			if (success)
			{
				body.Append("→注文成功（注文ID：").Append(cart.OrderId).Append("）\n");
			}
			else
			{
				body.Append("→注文失敗\n");
				errorMessages
					.Where(message => (string.IsNullOrEmpty(message) == false))
					.ToList()
					.ForEach(messages => body.Append("  ").Append(messages).Append("\n"));
				if (string.IsNullOrEmpty(fixedPurchaseApiErrorMessage) == false)
				{
					body.Append("APIエラー : ").Append(fixedPurchaseApiErrorMessage + "\n");
				}
			}
			if (isLastTarget)
			{
				var error = fixedPurchaseAlertMessages.Distinct().ToList();

				if (error.Any()) body.Append("\n");

				error.ForEach(messages => body.Append("★").Append(messages).Append("\n"));
				body.Append("---\n");
			}
			return body.ToString();
		}

		/// <summary>
		/// Create fixed purchase report mail for operator
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="user">ユーザ情報</param>
		/// <param name="fixedPurchaseAlertMessages">アラートメッセージ</param>
		/// <param name="currentMessage">StringBuilder</param>
		private string CreateFixedPurchaseReportMailForOperator(FixedPurchaseModel fixedPurchase, UserModel user, List<string> fixedPurchaseAlertMessages, StringBuilder currentMessage)
		{
			var body = new StringBuilder();

			if ((currentMessage.Length > 0) && (currentMessage.ToString().EndsWith("---\n") == false))
			{
				body.Append("---\n");
			}

			body.Append("■定期購入ID : ").Append(fixedPurchase.FixedPurchaseId).Append("\n");
			body.Append("  ユーザー   : ").Append(user.Name).Append("[").Append(user.UserId).Append("]\n");

			var alertMessages = fixedPurchaseAlertMessages.Distinct().ToList();

			if (alertMessages.Any()) body.Append("\n");

			alertMessages.ForEach(messages => body.Append("★").Append(messages).Append("\n"));
			body.Append("---\n");

			return body.ToString();
		}

		/// <summary>
		/// 管理者向け定期購入報告メール送信処理
		/// </summary>
		/// <param name="successCount">成功件数</param>
		/// <param name="alertCount">警告件数</param>
		/// <param name="errorCount">失敗件数</param>
		/// <param name="contents">内容詳細</param>
		private void SendFixedPurchaseMailToOperator(int successCount, int alertCount, int errorCount, string contents)
		{
			var body = new StringBuilder();
			body.Append("\n");
			body.Append("成功").Append(StringUtility.ToNumeric(successCount).PadLeft(3, ' ')).Append("件\n");
			body.Append("警告").Append(StringUtility.ToNumeric(alertCount).PadLeft(3, ' ')).Append("件\n");
			body.Append("失敗").Append(StringUtility.ToNumeric(errorCount).PadLeft(3, ' ')).Append("件\n");
			body.Append("\n");
			body.Append(this.CanSendFixedPurchaseMailToUser ? "ユーザーにメールを送信しました。" : "ユーザーへはメールを送信していません。");
			body.Append("\n\n");

			//メールを送信する設定でかつ、スケジュールの日付がデフォルト「0001/01/01 00:00:00」ではない場合に表示
			if (this.CanSendFixedPurchaseMailToUser && (this.FixedPurchaseMailSendTiming.ScheduleDateTime.CompareTo(new DateTime()) != 0))
			{
				body.Append("下記の注文ついて "
					+ DateTimeUtility.ToStringForManager(
						this.FixedPurchaseMailSendTiming.ScheduleDateTime,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)
					+ "の時間より順次メール送信いたします。\n");
			}

			body.Append(contents);

			var mailSender = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_FIXEDPURCHASE_FOR_OPERATOR, "", new Hashtable(), true, Constants.MailSendMethod.Auto);
			mailSender.SetBody(body.ToString());
			mailSender.SetSubject(mailSender.Subject + "[" + DateTime.Now.ToString("MM/dd") + "]");
			if (mailSender.SendMail() == false)
			{
				FileLogger.WriteError(body.ToString(), mailSender.MailSendException);
			}
		}
		#endregion

		#region CanExecByOrderNow
		/// <summary>
		/// Can execute by order now
		/// </summary>
		/// <param name="fixedPurchase">Fixed purchase model</param>
		/// <returns>True or False</returns>
		public bool CanExecByOrderNow(FixedPurchaseModel fixedPurchase)
		{
			var result = false;
			// Load configuration file
			var mainElement = XElement.Load(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + Constants.FILE_XML_ORDERFILEIMPORT_SETTING);
			var serviceNode =
				from serviceNodes in mainElement.Elements("OrderFile")
				where (serviceNodes.Element("Value").Value == Constants.KBN_ORDERFILE_IMPORT_ORDER_SECOND_TIME_NON_DEPOSIT)
				select new
				{
					importFileSetting = serviceNodes.Elements("ImportFileSetting")
						.ToDictionary(node => node.Attribute("key").Value, node => node.Attribute("value").Value)
				};

			if (serviceNode.FirstOrDefault() == null) return result;

			// Import file basic settings
			var importFileSettings = serviceNode.FirstOrDefault().importFileSetting;
			var extendStatusShippedPending = importFileSettings["ExtendStatusShippedPending"];
			result = ((string.IsNullOrEmpty(extendStatusShippedPending) == false)
				&& (fixedPurchase.ExtendStatus[int.Parse(extendStatusShippedPending) - 1].Value
					== Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_ON));
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>定期注文登録時にユーザーに注文メール＆定期購入エラーメール送信するか</summary>
		public bool CanSendFixedPurchaseMailToUser { get; set; }
		/// <summary>次回配送日・次々回配送日を更新するか</summary>
		public bool CanUpdateShippingDate { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
		/// <summary>定期注文メール送信タイミング管理</summary>
		public FixedPurchaseMailSendTiming FixedPurchaseMailSendTiming { get; set; }
		/// <summary>Is Send Mail Convenience Store Change</summary>
		public bool IsSendMailConvenienceStoreChange { get; set; }
		/// <summary>注文拡張ステータス番号（メール便配送サービスエスカレーション機能用）</summary>
		public string DeliveryCompanyMailEscalationOrderExtendNo { get; set; }
		#endregion
	}

	/// <summary>
	/// 定期購入注文登録結果メッセージ
	/// </summary>
	public class ResultMessages
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ResultMessages()
		{
			this.ErrorMessages = new List<string>();
			this.AlertMessages = new List<string>();
		}
		#endregion

		#region プロパティ
		/// <summary>エラーメッセージ</summary>
		public List<string> ErrorMessages { get; set; }
		/// <summary>アラートメッセージ</summary>
		public List<string> AlertMessages { get; set; }
		#endregion
	}

	/// <summary>
	/// 定期購入向け注文登録インナークラス
	/// ※OrderRegisterBaseを継承しメソッドを実装
	/// </summary>
	public class OrderRegisterFixedPurchaseInner : OrderRegisterBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isUser">ユーザーか（ポイント付与判断など）</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="canSendFixedPurchaseMailToUser">定期注文登録時にユーザーに注文メール＆定期購入エラーメール送信するか</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseMailSendTiming">注文完了メールの送信タイミング管理</param>
		/// <param name="senderType">管理者向け注文完了メール送信者種別</param>
		/// <remarks>
		/// senderTypeは定期今すぐ注文のときのみ利用する
		/// 今すぐ注文を実行すると、注文実行種別（ExecTypes）が全てFixedPurchaseBatchになってしまい
		/// 管理者向け注文完了メール送信の判別がフロント・管理・バッチでつかなくなってしまうため指定する必要がある
		/// </remarks>
		public OrderRegisterFixedPurchaseInner(
			bool isUser,
			string lastChanged,
			bool canSendFixedPurchaseMailToUser,
			string fixedPurchaseId,
			FixedPurchaseMailSendTiming fixedPurchaseMailSendTiming,
			Constants.EnabledOrderCompleteEmailSenderType senderType = Constants.EnabledOrderCompleteEmailSenderType.Batch)
			: base(ExecTypes.FixedPurchaseBatch, isUser, lastChanged, senderType)
		{
			this.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL;
			this.FixedPurchasePaymentStatus = Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL;
			this.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS;
			this.CanSendFixedPurchaseMailToUser = canSendFixedPurchaseMailToUser;
			this.FixedPurchaseId = fixedPurchaseId;
			this.FixedPurchaseMailSendTiming = fixedPurchaseMailSendTiming;
			this.YamatoKaSmsOrders = new List<KeyValuePair<string, Hashtable>>();
			this.BokuPaymentOrders = new List<KeyValuePair<string, Hashtable>>();
		}
		#endregion

		#region #OrderRegisterBaseオーバーライドメソッド関連
		/// <summary>
		/// 外部決済かどうかチェック
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>外部決済か</returns>
		protected override bool CheckExternalPayment(Hashtable order, CartObject cart)
		{
			if (cart.Payment.IsPaymentYamatoKaSms)
			{
				this.YamatoKaSmsOrders.Add(new KeyValuePair<string, Hashtable>(cart.OrderId, order));
				return true;
			}

			if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
			{
				this.BokuPaymentOrders.Add(new KeyValuePair<string, Hashtable>(cart.OrderId, order));
				return true;
			}
			return false;
		}

		/// <summary>
		/// 外部決済で決済注文ID発行が必要かどうか
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>true: 発行必要、false: 発行不要</returns>
		protected override bool IsPaymentOrderIdIssue(string paymentId)
		{
			if (OrderCommon.CheckPaymentYamatoKaSms(paymentId)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// 決済失敗時処理
		/// </summary>
		protected override void PaymentFailedProcess()
		{
			// 定期購入ステータスに「決済エラー停止」セット
			this.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED;
			// 決済ステータスに「決済失敗」セット
			this.FixedPurchasePaymentStatus = Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR;
			// 定期購入履歴区分に「決済エラー」セット
			this.FixedPurchaseHistoryKbn = Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT;
		}

		/// <summary>
		/// 注文完了時の処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>アラート文言</returns>
		public override string OrderCompleteProcesses(Hashtable order, CartObject cart, UpdateHistoryAction updateHistoryAction)
		{
			// HACK：注文リファクタリング時に定期サービスに実装する
			this.TransactionName = "4-1.定期購入情報更新処理";
			var fixedPurchaseService = new FixedPurchaseService();
			var orderService = new OrderService();
			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 定期購入ID + 定期購入回数(注文時点)更新
				var fixedPurchaseId = (string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID];
				var orderId = (string)order[Constants.FIELD_ORDER_ORDER_ID];
				var fixedPurchase = fixedPurchaseService.Get(fixedPurchaseId, accessor);
				orderService.UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
					orderId,
					fixedPurchase.FixedPurchaseId,
					fixedPurchase.OrderCount + 1,
					this.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 定期商品購入回数(注文時点)更新
				orderService.UpdateFixedPerchaseItemOrderCountWhenOrdering(
					orderId,
					fixedPurchase.Shippings[0].Items,
					this.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 注文拡張ステータス更新
				orderService.UpdateOrderExtendStatusForCreateFixedPurchaseOrder(
					orderId,
					fixedPurchase,
					this.LastChanged,
					Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 注文登録成功更新
				fixedPurchaseService.UpdateForSuccessOrder(
					fixedPurchase.FixedPurchaseId,
					cart.GetShipping().NextShippingDate,
					cart.GetShipping().NextNextShippingDate,
					orderId,
					this.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					fixedPurchase.Shippings[0].Items,
					accessor);

				// 定期購入継続分析
				new FixedPurchaseRepeatAnalysisService()
					.UpdateRepeatAnalysisFixedPurchaseIdByOrderId(
						orderId,
						fixedPurchase.FixedPurchaseId,
					this.LastChanged,
					accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(orderId, this.LastChanged, accessor);
					new UpdateHistoryService().InsertForFixedPurchase(fixedPurchaseId, this.LastChanged, accessor);
				}

				// トランザクション確定
				accessor.CommitTransaction();

				if (this.FixedPurchaseMailSendTiming.TimeZoneStatus == FixedPurchaseMailSendTiming.TimeZoneStatusEnum.Ok)
				{
					this.TransactionName = "4-2.メール送信処理";
					SendOrderMails(order, cart, this.IsUser, false, this.CanSendFixedPurchaseMailToUser);

					if ((Constants.REPEATLINE_OPTION_ENABLED == Constants.RepeatLineOption.CooperationAndMessaging)
						&& this.CanSendFixedPurchaseMailToUser)
					{
						// LINEを送信
						this.TransactionName = "4-3.LINE連携処理";
						var sendLineMessageFlg = MailSendUtility.GetMailTemplateInfo(
							w2.Domain.Constants.CONST_DEFAULT_SHOP_ID,
							w2.App.Common.Constants.CONST_MAIL_ID_ORDER_COMPLETE).LineUseFlg;
						if (sendLineMessageFlg == MailTemplateModel.LINE_USE_FLG_ON) SendOrderCompleteToLine(order, cart);
					}
				}
				else
				{
					if (this.CanSendFixedPurchaseMailToUser) this.FixedPurchaseMailSendTiming.InsertFixedPurchaseBatchMailTmpLog((string)order[Constants.FIELD_ORDER_ORDER_ID], Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ORDER);
				}
			}

			return "";
		}

		/// <summary>
		/// 注文完了後の処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public override void AfterOrderCompleteProcesses(
			Hashtable order,
			CartObject cart,
			UpdateHistoryAction updateHistoryAction)
		{
			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				// 更新履歴登録
				new UpdateHistoryService().InsertForOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], this.LastChanged);
			}

			// 頒布会の次回配送商品を変更
			UpdateNextSubscriptionBoxProduct(cart, order);
		}

		/// <summary>
		/// 注文完了スキップ時の処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		public override void SkipOrderCompleteProcesses(Hashtable order, CartObject cart)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId { get; set; }
		/// <summary>定期購入ステータス</summary>
		public string FixedPurchaseStatus { get; set; }
		/// <summary>定期購入決済ステータス</summary>
		public string FixedPurchasePaymentStatus { get; set; }
		/// <summary>定期購入履歴区分</summary>
		public string FixedPurchaseHistoryKbn { get; set; }
		/// <summary>定期注文登録時にユーザーに注文メール＆定期購入エラーメール送信するか</summary>
		public bool CanSendFixedPurchaseMailToUser { get; set; }
		/// <summary>定期注文メール送信タイミング管理</summary>
		public FixedPurchaseMailSendTiming FixedPurchaseMailSendTiming { get; set; }
		/// <summary>ヤマト後払いSMS認証連携決済注文</summary>
		public List<KeyValuePair<string, Hashtable>> YamatoKaSmsOrders { set; get; }
		/// <summary>Boku payment orders</summary>
		public List<KeyValuePair<string, Hashtable>> BokuPaymentOrders { set; get; }
		#endregion
	}
}
