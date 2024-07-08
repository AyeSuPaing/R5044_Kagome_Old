/*
=========================================================================================================
  Module      : 再与信アクション（コンビニ後払い与信）クラス(ReauthCvsDefAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.DSKDeferred;
using w2.App.Common.Order.Payment.DSKDeferred.OrderCancel;
using w2.App.Common.Order.Payment.DSKDeferred.OrderRegister;
using w2.Common.Util;
using w2.Domain.Order;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.OrderModifyCancel;
using w2.App.Common.Order.Payment.GMO.OrderRegister;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Transaction;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.Order;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.App.Common.Order.Payment.Score.Cancel;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（コンビニ後払い与信）クラス
	/// </summary>
	public class ReauthCvsDefAction : BaseReauthAction<ReauthCvsDefAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReauthCvsDefAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "コンビニ後払い与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// コンビニ後払い与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			ReauthActionResult result = null;
			var order = reauthActionParams.Order;

			switch (Constants.PAYMENT_CVS_DEF_KBN)
			{
				case Constants.PaymentCvsDef.YamatoKa:
					result = ExecAuthYamatoKa(order);
					break;

				case Constants.PaymentCvsDef.Gmo:
					result = ExecAuthGmo(order);
					break;

				case Constants.PaymentCvsDef.Atodene:
					result = ExecAuthAtodene(order);
					break;

				case Constants.PaymentCvsDef.Dsk:
					result = ExecAuthDSKDeferred(order);
					break;

				case Constants.PaymentCvsDef.Atobaraicom:
					result = ExecAuthAtobaraicom(order);
					break;

				case Constants.PaymentCvsDef.Score:
					result = ExecAuthScore(order);
					break;

				case Constants.PaymentCvsDef.Veritrans:
					result = ExecAuthVeritrans(order);
					break;

				// その他
				default:
					result = new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						apiErrorMessage: "該当のコンビニ後払いは再与信をサポートしていません。");
					break;
			}

			// 取引IDをセット
			if (result.Result)
			{
				order.CardTranId = result.CardTranId;
				order.PaymentOrderId = result.PaymentOrderId;
			}
			return result;
		}

		/// <summary>
		/// ヤマトコンビニ後払い与信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthYamatoKa(OrderModel order)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var cardTranId = OrderCommon.CreatePaymentOrderId(order.ShopId);

			var orderShipping = order.Shippings[0];
			var isSms = OrderCommon.CheckPaymentYamatoKaSms(order.OrderPaymentKbn);
			var api = new PaymentYamatoKaEntryApi();
			if (api.Exec(
				cardTranId,
				DateTime.Now.ToString("yyyyMMdd"),
				PaymentYamatoKaUtility.CreateYamatoKaShipYmd(order),
				order.Owner.OwnerName,
				StringUtility.ToHankakuKatakana(StringUtility.ToZenkakuKatakana(order.Owner.OwnerNameKana)),
				order.Owner.OwnerZip,
				new PaymentYamatoKaAddress(order.Owner.OwnerAddr1, order.Owner.OwnerAddr2, order.Owner.OwnerAddr3, order.Owner.OwnerAddr4),
				order.Owner.OwnerTel1,
				order.Owner.OwnerMailAddr,
				order.LastBilledAmount,
				PaymentYamatoKaUtility.CreateSendDiv(isSms, orderShipping.AnotherShippingFlg),
				PaymentYamatoKaUtility.CreateProductItemList(order.LastBilledAmount),
				orderShipping.ShippingName,
				orderShipping.ShippingZip,
				new PaymentYamatoKaAddress(orderShipping.ShippingAddr1, orderShipping.ShippingAddr2, orderShipping.ShippingAddr3, orderShipping.ShippingAddr4),
				orderShipping.ShippingTel1) == false)
			{
				var message = LogCreator.CreateErrorMessage(api.ResponseData.ErrorCode, api.ResponseData.ErrorMessages);
				if (string.IsNullOrEmpty(api.ResponseData.Result) == false)
				{
					message += string.Format(
						"[審査結果：{0}({1})]",
						api.ResponseData.ResultDescription,
						api.ResponseData.Result);
				}
				return new ReauthActionResult(false, order.OrderId, string.Empty, cardTranIdForLog: cardTranId, apiErrorMessage: message);
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, cardTranId, cardTranId, order.LastBilledAmount),
				cardTranId,
				cardTranId);
		}

		/// <summary>
		/// GMO後払い与信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthGmo(OrderModel order)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			var orderInput = (OrderModel)order.Clone();
			orderInput.PaymentOrderId = paymentOrderId;
			orderInput.DeviceInfo = order.DeviceInfo;

			var request = new GmoRequestOrderRegister(orderInput);
			var facade = new GmoDeferredApiFacade();
			var result = facade.OrderRegister(request);
			if ((result == null)
				|| (result.Result != ResultCode.OK)
				|| (result.TransactionResult.IsResultNg))
			{
				var errorMessage = ((result != null) && (result.Result == ResultCode.NG))
					? string.Join(
						"\t",
						result.Errors.Error.Select(
							err => LogCreator.CreateErrorMessage(err.ErrorCode, err.ErrorMessage)))
					: ((result != null) && result.TransactionResult.IsResultNg)
						? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_GMO_CVSDEFAUTH_ERROR)
						: "";
				return new ReauthActionResult(
					false,
					orderInput.OrderId,
					string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: errorMessage);
			}

			if (result.TransactionResult.IsResultExamination)
			{
				// リアルタイムで与信結果が取得できなかった場合は取り消す
				var cancelRequest = new GmoRequestOrderModifyCancel();
				cancelRequest.KindInfo = new KindInfoElement();
				cancelRequest.KindInfo.UpdateKind = UpdateKindType.OrderCancel;
				cancelRequest.Buyer = new Payment.GMO.OrderModifyCancel.BuyerElement();
				cancelRequest.Buyer.GmoTransactionId = result.TransactionResult.GmoTransactionId;
				cancelRequest.Buyer.ShopTransactionId = result.TransactionResult.ShopTransactionId;
				cancelRequest.Deliveries = new Payment.GMO.OrderModifyCancel.DeliveriesElement();
				cancelRequest.Deliveries.Delivery = new Payment.GMO.OrderModifyCancel.DeliveryElement();
				cancelRequest.Deliveries.Delivery.DeliveryCustomer = new Payment.GMO.OrderModifyCancel.DeliveryCustomerElement();

				var cancelResult = facade.OrderModifyCancel(cancelRequest);
				// 取消しに失敗した場合
				if (cancelResult.Result != ResultCode.OK)
				{
					return new ReauthActionResult(
						false,
						orderInput.OrderId,
						string.Empty,
						cardTranIdForLog: result.TransactionResult.GmoTransactionId,
						apiErrorMessage: "GMO後払い与信保留の取消にて失敗。" + string.Join(
							"\t",
							result.Errors.Error.Select(
								err => LogCreator.CreateErrorMessage(err.ErrorCode, err.ErrorMessage))));
				}

				return new ReauthActionResult(
					false,
					orderInput.OrderId,
					string.Empty,
					cardTranIdForLog: orderInput.CardTranId);
			}
			return new ReauthActionResult(
				true,
				orderInput.OrderId,
				CreatePaymentMemo(orderInput.OrderPaymentKbn, paymentOrderId, result.TransactionResult.GmoTransactionId, orderInput.LastBilledAmount),
				cardTranId: result.TransactionResult.GmoTransactionId,
				paymentOrderId: paymentOrderId);
		}

		/// <summary>
		/// Atodene後払い与信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthAtodene(OrderModel order)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			var orderInput = order.Clone();
			orderInput.PaymentOrderId = paymentOrderId;

			if (string.IsNullOrEmpty(orderInput.OrderIdOrg) == false)
			{
				orderInput = CreateOrderForReturnExchange(orderInput, orderInput.OrderIdOrg);
			}

			var tranAdp = new AtodeneTransactionModelAdapter(orderInput);
			var res = tranAdp.Execute();

			// リアルタイムで与信が取れたもののみOK
			if (res.Result == AtodeneConst.RESULT_OK)
			{
				if ((res.TransactionInfo != null)
					&& (res.TransactionInfo.AutoAuthoriresult == AtodeneConst.AUTO_AUTH_RESULT_OK))
				{
					return new ReauthActionResult(
						true,
						orderInput.OrderId,
						CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, res.TransactionInfo.TransactionId, orderInput.LastBilledAmount),
						cardTranId: res.TransactionInfo.TransactionId,
						paymentOrderId: paymentOrderId);
				}

				if ((res.TransactionInfo != null)
					&& ((res.TransactionInfo.AutoAuthoriresult == AtodeneConst.AUTO_AUTH_RESULT_PROCESSING)
						|| (res.TransactionInfo.AutoAuthoriresult == AtodeneConst.AUTO_AUTH_RESULT_NG)))
				{
					var isAuthProcessing = (res.TransactionInfo.AutoAuthoriresult == AtodeneConst.AUTO_AUTH_RESULT_PROCESSING);

					// 審査保留・NGの場合は取り消す
					var cancelAdp = new AtodeneCancelTransactionAdapter(res.TransactionInfo.TransactionId);
					var cancelRes = cancelAdp.ExecuteCancel();

					// 取消しに失敗した場合
					if (cancelRes.Result != AtodeneConst.RESULT_OK)
					{
						var message = string.Format("Atodene後払い与信{0}の取消にて失敗。", isAuthProcessing ? "保留" : "失敗");
						return new ReauthActionResult(
							false,
							orderInput.OrderId,
							string.Empty,
							cardTranIdForLog: res.TransactionInfo.TransactionId,
							apiErrorMessage: message + string.Join(
								"\t",
								res.Errors.Error.Select(
									err => LogCreator.CreateErrorMessage(err.ErrorCode, err.ErrorMessage))));
					}
				}
			}

			var apiErrors = Enumerable.Empty<string>().ToArray();
			if ((res.Errors != null)
				&& (res.Errors.Error != null)
				&& res.Errors.Error.Any())
			{
				apiErrors = res.Errors.Error
					.Select(err => LogCreator.CreateErrorMessage(err.ErrorCode, err.ErrorMessage))
					.ToArray();
			}

			return new ReauthActionResult(
				false,
				orderInput.OrderId,
				string.Empty,
				cardTranIdForLog: orderInput.CardTranId,
				apiErrorMessage: string.Format(
					"Atodene後払いエラー。自動審査結果：{0}{1}{2}",
					(res.TransactionInfo != null)
						? res.TransactionInfo.AutoAuthoriresult
						: "不明",
					apiErrors.Any()
						? Environment.NewLine
						: string.Empty,
					apiErrors.JoinToString("\t")));
		}

		/// <summary>
		/// DSK後払い与信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>与信結果</returns>
		private ReauthActionResult ExecAuthDSKDeferred(OrderModel order)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			var orderInput = order.Clone();
			orderInput.PaymentOrderId = paymentOrderId;

			if (string.IsNullOrEmpty(orderInput.OrderIdOrg) == false)
			{
				orderInput = CreateOrderForReturnExchange(orderInput, orderInput.OrderIdOrg);
			}

			var registerAdapter = new DskDeferredOrderRegisterModelAdapter(orderInput);
			var response = registerAdapter.Execute();

			if ((response.IsResultOk)
				&& (response.TransactionResult != null)
				&& (response.TransactionResult.AuthorResult == DskDeferredConst.AUTO_AUTH_RESULT_NG))
			{
				var isAuthProcessing = (response.TransactionResult.AuthorResult == DskDeferredConst.AUTO_AUTH_RESULT_HOLD);

				var cancelAdapter = new DskDeferredOrderCancelAdapter(response.TransactionResult.TransactionId, orderInput.PaymentOrderId, orderInput.LastBilledAmount.ToPriceString());
				var cancelResponse = cancelAdapter.Execute();

				if (cancelResponse.IsResultOk == false)
				{
					var message = string.Format("DSK後払い与信{0}の取消にて失敗。", isAuthProcessing ? "保留" : "失敗");
					return new ReauthActionResult(
						false,
						orderInput.OrderId,
						string.Empty,
						cardTranIdForLog: response.TransactionResult.TransactionId,
						apiErrorMessage: message + string.Join(
							"\t",
							response.Errors.Error.Select(
								err => LogCreator.CreateErrorMessage(err.ErrorCode, err.ErrorMessage))));
				}
			}

			if ((response.IsResultOk == false)
				|| ((response.TransactionResult != null)
					&& (response.TransactionResult.AuthorResult == DskDeferredConst.AUTO_AUTH_RESULT_NG)))
			{
				var apiErrors = Enumerable.Empty<string>().ToArray();
				if ((response.Errors != null)
					&& (response.Errors.Error != null)
					&& response.Errors.Error.Any())
				{
					apiErrors = response.Errors.Error
						.Select(err => LogCreator.CreateErrorMessage(err.ErrorCode, err.ErrorMessage))
						.ToArray();
				}

				return new ReauthActionResult(
					false,
					orderInput.OrderId,
					string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: string.Format(
						"DSK後払いエラー。自動審査結果：{0}{1}{2}",
						(response.TransactionResult != null)
							? response.TransactionResult.AuthorResult
							: "不明",
						apiErrors.Any()
							? Environment.NewLine
							: string.Empty,
						apiErrors.JoinToString("\t")));
			}

			var result = new ReauthActionResult(
				true,
				orderInput.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, response.TransactionResult.TransactionId, orderInput.LastBilledAmount),
				cardTranId: response.TransactionResult.TransactionId,
				paymentOrderId: paymentOrderId);

			if ((response.TransactionResult != null) && (response.TransactionResult.AuthorResult == DskDeferredConst.AUTO_AUTH_RESULT_HOLD))
			{
				result.IsAuthResultHold = true;
			}

			return result;
		}

		/// <summary>
		/// 返品交換用に新規注文を作成
		/// </summary>
		/// <param name="newOrder">新規注文</param>
		/// <param name="orderIdOrg">元注文</param>
		/// <returns>新規注文</returns>
		private OrderModel CreateOrderForReturnExchange(OrderModel newOrder, string orderIdOrg)
		{
			var orgOrder = new OrderService().Get(orderIdOrg);
			var returnExchangeOrderItems = new OrderService().GetReturnExchangeOrderItems(orderIdOrg);

			var tmpOrderItems = orgOrder.Items.ToList();

			returnExchangeOrderItems.Select(
				order =>
				{
					if (tmpOrderItems.Any(item => (item.ProductId == order.ProductId) && (item.VariationId == order.VariationId)) == false)
					{
						tmpOrderItems.Add(order);
					}
					else
					{
						tmpOrderItems
							.Where(item => (item.ProductId == order.ProductId) && (item.VariationId == order.VariationId))
							.Select(item => item.ItemQuantity += order.ItemQuantity).ToArray();
					}
					return orgOrder;
				}).ToArray();

			newOrder.Items.Select(
				order =>
				{
					if (tmpOrderItems.Any(item => (item.ProductId == order.ProductId) && (item.VariationId == order.VariationId)) == false)
					{
						tmpOrderItems.Add(order);
					}
					else
					{
						tmpOrderItems
							.Where(item => (item.ProductId == order.ProductId) && (item.VariationId == order.VariationId))
							.Select(item => item.ItemQuantity += order.ItemQuantity).ToArray();
					}
					return orgOrder;
				}).ToArray();

			newOrder.Items = tmpOrderItems.Where(item => item.ItemQuantity > 0).ToArray();
			newOrder.OrderPriceShipping = orgOrder.OrderPriceShipping;
			newOrder.OrderPriceExchange = orgOrder.OrderPriceExchange;
			newOrder.OrderPriceSubtotal = tmpOrderItems.Where(item => item.ItemQuantity > 0).Sum(item => item.ProductPrice * item.ItemQuantity);

			return newOrder;
		}

		/// <summary>
		/// Atobaraicom後払い与信処理
		/// </summary>
		/// <param name="order">Order model</param>
		/// <returns>認証後払いの実行</returns>
		private ReauthActionResult ExecAuthAtobaraicom(OrderModel order)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var orderInput = order.Clone();
			var requestAPI = new AtobaraicomRegistationApi();
			var response = requestAPI.ExecRegistation(null, null, order);

			if (response.IsAuthorizeHold)
			{
				new AtobaraicomCancelationApi().ExecCancel(response.SystemOrderId);
				var result = new ReauthActionResult(
					false,
					orderInput.OrderId,
					string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: string.Format(
						"{0}{1}",
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ATOBARAICOM_CVSDEFAUTH_ERROR),
						string.Join("\t", response.Messages)));
				return result;
			}

			if (string.IsNullOrWhiteSpace(response.OrderId)
				|| (response.IsAuthorizeOK == false))
			{
				return new ReauthActionResult(
					false,
					orderInput.OrderId,
					string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: string.Format("{0}{1}",
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ATOBARAICOM_CVSDEFAUTH_ERROR),
						string.Join("\t",response.Messages)));
			}

			return new ReauthActionResult(
				true,
				orderInput.OrderId,
				CreatePaymentMemo(
					orderInput.OrderPaymentKbn,
					response.SystemOrderId,
					string.Empty,
					orderInput.LastBilledAmount),
				cardTranId: string.Empty,
				paymentOrderId: response.SystemOrderId);
		}

		/// <summary>
		/// Score後払い与信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthScore(OrderModel order)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			var orderInput = order.Clone();
			orderInput.DeviceInfo = order.DeviceInfo;

			if (string.IsNullOrEmpty(orderInput.OrderIdOrg) == false)
			{
				orderInput = CreateOrderForReturnExchange(orderInput, orderInput.OrderIdOrg);
			}

			var facade = new ScoreApiFacade();
			var orderRegisterRequest = new ScoreRequestOrderRegisterModify(orderInput)
			{
				Buyer =
				{
					ShopTransactionId = paymentOrderId,
					NissenTransactionId = string.Empty
				}
			};

			var errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_SCORE_PAYMENT_CHANGE_ERROR);
			var orderRegisterResponse = facade.OrderRegister(orderRegisterRequest);
			if ((orderRegisterResponse == null)
				|| (orderRegisterResponse.Result != ScoreResult.Ok.ToText()))
			{
				return new ReauthActionResult(
					result: false,
					orderId: orderInput.OrderId,
					paymentMemo: string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: errorMessage);
			}

			if (orderRegisterResponse.TransactionResult.IsResultNg)
			{
				var cancelRequest = new ScoreCancelRequest
				{
					Transaction =
					{
						NissenTransactionId = orderRegisterResponse.TransactionResult.NissenTransactionId,
						ShopTransactionId = paymentOrderId,
						BilledAmount = orderRegisterRequest.Buyer.BilledAmount
					}
				};
				var cancelResult = facade.OrderCancel(cancelRequest);

				if (cancelResult.Result != ScoreResult.Ok.ToText())
				{
					var cancelErrorMessage = string.Format(
						"スコア後払い与信失敗の取消にて失敗。注文ID：{0}、取引ID：{1}",
						order.OrderId,
						cancelResult.TransactionResult.NissenTransactionId);
					return new ReauthActionResult(
						false,
						orderInput.OrderId,
						string.Empty,
						cardTranIdForLog: cancelResult.TransactionResult.NissenTransactionId,
						apiErrorMessage: cancelErrorMessage);
				}
				return new ReauthActionResult(
					result: false,
					orderId: orderInput.OrderId,
					paymentMemo: string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: errorMessage);
			}

			var reauthActionResult = new ReauthActionResult(
				result: true,
				orderId: orderInput.OrderId,
				paymentMemo: CreatePaymentMemo(orderInput.OrderPaymentKbn, paymentOrderId, orderRegisterResponse.TransactionResult.NissenTransactionId, orderInput.LastBilledAmount),
				paymentOrderId: paymentOrderId,
				cardTranId: orderRegisterResponse.TransactionResult.NissenTransactionId,
				apiErrorMessage: orderRegisterResponse.TransactionResult.IsResultHold
					? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_SCORE_CVSDEFAUTH_ERROR)
					: string.Empty)
			{
				IsAuthResultHold = orderRegisterResponse.TransactionResult.IsResultHold
			};
			return reauthActionResult;
		}

		/// <summary>
		/// ベリトランス後払い与信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthVeritrans(OrderModel order)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			var orderInput = order.Clone();
			orderInput.PaymentOrderId = paymentOrderId;

			if (string.IsNullOrEmpty(orderInput.OrderIdOrg) == false)
			{
				orderInput = CreateOrderForReturnExchange(orderInput, orderInput.OrderIdOrg);
			}

			var errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_VERITRANS_PAYMENT_CHANGE_ERROR);
			var paymentVeritransCvsDef = new PaymentVeritransCvsDef();
			var orderRegisterResponse = paymentVeritransCvsDef.OrderRegister(orderInput);
			ReauthActionResult reauthActionResult;
			if ((orderRegisterResponse == null)
				|| (orderRegisterResponse.Mstatus != VeriTransConst.RESULT_STATUS_OK))
			{
				reauthActionResult = new ReauthActionResult(
					result: false,
					orderId: orderInput.OrderId,
					paymentMemo: string.Empty,
					cardTranId: orderInput.CardTranId,
					paymentOrderId: orderInput.PaymentOrderId,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: errorMessage);
				return reauthActionResult;
			}

			if ((orderRegisterResponse.AuthorResult == VeriTransConst.VeritransAuthorResult.Ng.ToText())
				|| (orderRegisterResponse.AuthorResult == VeriTransConst.VeritransAuthorResult.Hold.ToText()))
			{
				var cancelResult = paymentVeritransCvsDef.OrderCancel(orderInput.PaymentOrderId);
				if (cancelResult.Mstatus != VeriTransConst.RESULT_STATUS_OK)
				{
					var cancelErrorMessage = string.Format(
						"ベリトランス後払い与信失敗の取消にて失敗。注文ID：{0}、取引ID：{1}",
						order.OrderId,
						cancelResult.CustTxn);
					reauthActionResult = new ReauthActionResult(
						result: false,
						orderId: orderInput.OrderId,
						paymentMemo: string.Empty,
						cardTranId: cancelResult.CustTxn,
						paymentOrderId: orderInput.PaymentOrderId,
						cardTranIdForLog: cancelResult.CustTxn,
						apiErrorMessage: cancelErrorMessage);
					return reauthActionResult;
				}
				reauthActionResult = new ReauthActionResult(
					result: false,
					orderId: orderInput.OrderId,
					paymentMemo: string.Empty,
					cardTranId: orderInput.CardTranId,
					orderInput.PaymentOrderId,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: orderRegisterResponse.AuthorResult == VeriTransConst.VeritransAuthorResult.Hold.ToText()
						? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_VERITRANS_CVSDEFAUTH_ERROR)
						: errorMessage);
				return reauthActionResult;
			}

			reauthActionResult = new ReauthActionResult(
				result: true,
				orderId: orderInput.OrderId,
				paymentMemo: CreatePaymentMemo(
					orderInput.OrderPaymentKbn,
					paymentOrderId,
					orderRegisterResponse.CustTxn,
					orderInput.LastBilledAmount),
				cardTranId: orderRegisterResponse.CustTxn,
				paymentOrderId: paymentOrderId,
				cardTranIdForLog: orderRegisterResponse.CustTxn,
				apiErrorMessage: string.Empty);
			return reauthActionResult;
		}
		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			public ReauthActionParams(OrderModel order, string httpHeader = null)
			{
				this.Order = order;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel Order { get; private set; }
			#endregion
		}
	}
}
