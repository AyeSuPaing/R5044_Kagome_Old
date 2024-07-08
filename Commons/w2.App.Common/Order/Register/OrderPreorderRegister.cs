/*
=========================================================================================================
  Module      : 仮注文登録処理クラス(OrderRegistPreorder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Flaps;
using w2.App.Common.NextEngine.Helper;
using w2.App.Common.Option;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.NewebPay;
using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Product;
using w2.App.Common.User;
using w2.Common;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.Recommend;
using w2.Domain.SerialKey;
using w2.Domain.ShopShipping;
using w2.Domain.TwOrderInvoice;
using w2.Domain.TwUserInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Register
{
	/// <summary>
	/// 仮注文登録処理クラス
	/// </summary>
	public class OrderPreorderRegister : IOrderPreorderRegister
	{
		/// <summary>
		/// 仮注文登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="registGuestUser">ゲストユーザー登録するか</param>
		/// <param name="isUser">会員か否か</param>
		/// <param name="isFirstCart">最初のカートか</param>
		/// <param name="shippingNo">登録した配送先NO</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="execType">実行タイプ</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功/失敗</returns>
		public bool RegistPreOrder(
			Hashtable order,
			CartObject cart,
			bool registGuestUser,
			bool isUser,
			bool isFirstCart,
			out int shippingNo,
			string lastChanged,
			OrderRegisterBase.ExecTypes execType,
			UpdateHistoryAction updateHistoryAction)
		{
			shippingNo = 0;
			string transactionName = null;

			bool success = true;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				// トランザクション開始
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				try
				{
					//------------------------------------------------------
					// １－０．デッドロックの関係上先に行っておく処理
					//------------------------------------------------------
					// 初回購入かチェックする。
					bool isOrderFirstBuy = false;
					if (Constants.W2MP_POINT_OPTION_ENABLED || Constants.W2MP_COUPON_OPTION_ENABLED)
					{
						isOrderFirstBuy = DomainFacade.Instance.OrderService.CheckOrderFirstBuy(cart.OrderUserId, string.Empty, sqlAccessor);
					}

					var isOrderFirstBuyForPoint = isOrderFirstBuy;
					if (string.IsNullOrEmpty(cart.OrderCombineParentOrderId) == false)
					{
						var orderIdList = new List<string>(cart.OrderCombineParentOrderId.Split(',')).ToArray();
						isOrderFirstBuyForPoint = PointOptionUtility.CheckOrderFirstBuyForOrderCombine(cart.CartUserId, orderIdList, sqlAccessor);
					}

					//------------------------------------------------------
					// １－１．仮注文データ挿入処理
					//------------------------------------------------------
					if (success)
					{
						transactionName = "1-1-1.注文情報INSERT処理";
						InsertOrder(order, cart, isUser, isFirstCart, sqlAccessor, execType, registGuestUser);

						transactionName = "1-1-2.注文者情報INSERT処理";
						InsertOrderOwner(order, cart.Owner, sqlAccessor);

						transactionName = "1-1-3.注文配送先情報INSERT処理";
						InsertOrderShipping(order, cart, sqlAccessor);

						transactionName = "1-1-4.注文商品情報INSERT処理 + シリアルキーRESERVE処理";
						var insertOrderItemResult = InsertOrderItem(order, cart, sqlAccessor);
						if (insertOrderItemResult.Result == false) throw new ReserveSerialKeyException(insertOrderItemResult.ErrorCartProduct);

						transactionName = "1-1-5.注文セットプロモーション情報INSERT処理";
						InsertOrderSetPromotion(order, cart, sqlAccessor);

						transactionName = "1-1-6.税率毎注文情報INSERT処理";
						InsertOrderPriceByTaxRate(order, cart, sqlAccessor);
					}

					//------------------------------------------------------
					// １－２．在庫データ引当処理
					//------------------------------------------------------
					if (success)
					{
						transactionName = "1-2.商品在庫情報UPDATE処理";
						success = OrderCommon.UpdateProductStock(order, cart, true, sqlAccessor);
					}
					if (success)
					{
						transactionName = "1-2a.商品在庫情報UPDATE処理(商品在庫履歴)";
						OrderCommon.InsertProductStockHistory((string)order[Constants.FIELD_ORDER_ORDER_ID], cart, true, sqlAccessor);
					}

					//------------------------------------------------------
					// １－３．ユーザポイント関連処理
					//------------------------------------------------------
					if (Constants.W2MP_POINT_OPTION_ENABLED && isUser)
					{
						// ECで新規注文登録時に会員登録をする場合、クロスポイント側に連携するユーザーが後から作られる関係でエラーになる。
						// そのためこの条件のみ先に連携を行う
						if (Constants.CROSS_POINT_OPTION_ENABLED
							&& registGuestUser
							&& (execType == OrderRegisterBase.ExecTypes.CommerceManager)
							&& (cart.Owner.OwnerKbn == Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_USER))
						{
							var user = CreateUserModel(order, cart);
							// ゲストユーザーの際はメールアドレスを空にする
							user.MailAddr = string.Empty;

							// クロスポイント側にユーザー情報を登録
							var apiResult = new CrossPointUserApiService().Insert(user);
							if (apiResult.IsSuccess == false)
							{
								var errorMessage = apiResult.ErrorCodeList.Contains(
									Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
										? apiResult.ErrorMessage
										: MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

								throw new w2Exception(errorMessage);
							}
						}

						if (success)
						{
							transactionName = "1-3-1.利用ポイントUPDATE処理";
							success = UsePoints(order, cart, isUser, lastChanged, UpdateHistoryAction.DoNotInsert, sqlAccessor);
						}
						if (success)
						{
							transactionName = "1-3-2.購入ポイントINSERT処理";
							PublishPoint(order, cart, isUser, isOrderFirstBuyForPoint, isFirstCart, lastChanged, UpdateHistoryAction.DoNotInsert, sqlAccessor);
						}
					}

					//------------------------------------------------------
					// １－４．クーポン関連処理
					//------------------------------------------------------
					if (Constants.W2MP_COUPON_OPTION_ENABLED)
					{
						// 注文同梱ではなく、定期台帳の次回購入利用クーポンが設定された場合、その設定をリセットとクーポン戻す処理を行う
						if (success
							&& (string.IsNullOrEmpty(cart.OrderCombineParentOrderId))
							&& (cart.FixedPurchase != null)
							&& (order.ContainsKey(Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_ID))
							&& (string.IsNullOrEmpty((string)order[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_ID]) == false))
						{
							var fpService = new FixedPurchaseService();

							// 次回購入利用クーポンをリセット
							transactionName = "1-4-0-1.定期台帳の次回購入利用クーポンリセット処理";
							success = fpService.ResetNextShippingUseCoupon(
								cart.FixedPurchase.FixedPurchaseId,
								(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
								UpdateHistoryAction.Insert,
								sqlAccessor);

							// 定期台帳に適用したクーポンを戻す
							if (success && (cart.FixedPurchase.NextShippingUseCouponDetail != null))
							{
								transactionName = "1-4-0-2.定期台帳の次回購入利用クーポン戻す処理";
								success = new CouponService().ReturnNextShippingUseCoupon(
									cart.FixedPurchase.NextShippingUseCouponDetail,
									cart.OrderUserId,
									(string)order[Constants.FIELD_ORDER_ORDER_ID],
									cart.FixedPurchase.FixedPurchaseId,
									(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
									(cart.Coupon != null)
										? Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_ADJUSTMENT
										: Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE_CANCEL,
									UpdateHistoryAction.DoNotInsert,
									sqlAccessor);
							}
						}

						// クーポン利用処理
						if (success) success = UseCoupons(order, cart, lastChanged, ref transactionName, UpdateHistoryAction.DoNotInsert, sqlAccessor);

						// クーポン発行処理
						if (success)
							success = PublishCoupons(
								cart,
								isUser,
								isOrderFirstBuy,
								lastChanged,
								ref transactionName,
								UpdateHistoryAction.DoNotInsert,
								sqlAccessor);
					}

					//------------------------------------------------------
					// １－５．ゲストユーザ登録処理（何度も実行できてはいけない）
					//------------------------------------------------------
					if (success)
					{
						if (registGuestUser)
						{
							transactionName = "1-5.ゲストユーザINSERT処理";
							InsertGuestUser(order, cart, execType, sqlAccessor);
						}
					}

					// 定期会員フラグ更新処理
					if (success && (Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE == false))
					{
						if (Constants.FIXEDPURCHASE_OPTION_ENABLED && Constants.MEMBER_RANK_OPTION_ENABLED)
						{
							UpdateFixedPurchaseMemberFlg(order, cart, lastChanged, UpdateHistoryAction.DoNotInsert, sqlAccessor);
						}
					}

					//------------------------------------------------------
					// １－６． アドレス帳情報登録処理
					//------------------------------------------------------
					if (success)
					{
						// ログイン済み(会員のみ) AND アドレス帳保存フラグがtrue
						if (isUser)
						{
							transactionName = "1-6.アドレス帳情報INSERT処理";
							foreach (CartShipping shipping in cart.Shippings.FindAll(s => s.UserShippingRegistFlg))
							{
								shippingNo = InsertUserShipping(cart.OrderUserId, shipping, sqlAccessor);
							}

							if (OrderCommon.DisplayTwInvoiceInfo())
							{
								foreach (var shipping in cart.Shippings.FindAll(item => (item.UserInvoiceRegistFlg && item.IsShippingAddrTw)))
								{
									InsertUserInvoice(cart.OrderUserId, lastChanged, shipping, sqlAccessor);
								}
							}
						}
					}

					//------------------------------------------------------
					// １－７． クレジットカード登録処理
					//------------------------------------------------------
					if (success)
					{
						// 入力可能クレジットであればクレジットカード登録
						if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
							&& OrderCommon.CreditCardRegistable)
						{
							// 新規カード？
							if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
							{
								transactionName = "1-7.クレジットカード情報INSERT処理";

								var isZeusLinkPoint = ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus) && Constants.PAYMENT_SETTING_ZEUS_USE_LINKPOINT_ENABLED);
								var cardNo = (cart.Payment.CreditCardNo1 + cart.Payment.CreditCardNo2 + cart.Payment.CreditCardNo3 + cart.Payment.CreditCardNo4);
								if ((string.IsNullOrEmpty(cardNo)) && (isZeusLinkPoint == false))
								{
									throw new CartException(
										CommerceMessages.GetMessages(
											CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
								}
								var lastFourDigit = (cardNo.Length > 4) ? cardNo.Substring(cardNo.Length - 4, 4).Trim() : cardNo;
								// モバイルリンク式向けクレジットカード仮登録 
								if (execType == OrderRegisterBase.ExecTypes.Mobile)
								{
									var result = new UserCreditCardRegister().ExecProvisionalRegistration(cart.OrderUserId, UpdateHistoryAction.DoNotInsert, sqlAccessor);
									cart.Payment.CreditCardBranchNo = result.BranchNo.ToString();
								}
								// それ以外（ゼウスリンク式決済もこちら）
								else
								{
									UserCreditCardModel userCreditCard;
									if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans
										&& (execType == OrderRegisterBase.ExecTypes.CommerceManager))
									{
										userCreditCard = new UserCreditCardRegister().ExecOnlySaveForPayTg(
											new UserCreditCardInput
											{
												UserId = cart.OrderUserId,
												CardDispName = StringUtility.ToEmpty(cart.Payment.UserCreditCardName),
												CardNo = lastFourDigit,
												ExpirationMonth = cart.Payment.CreditExpireMonth,
												ExpirationYear = cart.Payment.CreditExpireYear,
												AuthorName = cart.Payment.CreditAuthorName,
												DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
												LastChanged = lastChanged,
												CompanyCode = cart.Payment.CreditCardCompany,
												CreditToken = cart.Payment.CreditToken,
											},
											lastChanged,
											UpdateHistoryAction.DoNotInsert,
											sqlAccessor);
									}
									// 楽天かつPayTg利用時
									else if (Constants.PAYMENT_SETTING_PAYTG_ENABLED
										&& Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten
										&& (execType == OrderRegisterBase.ExecTypes.CommerceManager))
									{
										userCreditCard = new UserCreditCardRegister().ExecOnlySaveForPayTg(
											new UserCreditCardInput
											{
												UserId = cart.OrderUserId,
												CardDispName = StringUtility.ToEmpty(cart.Payment.UserCreditCardName),
												CardNo = lastFourDigit,
												ExpirationMonth = cart.Payment.CreditExpireMonth,
												ExpirationYear = cart.Payment.CreditExpireYear,
												AuthorName = cart.Payment.CreditAuthorName,
												DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
												LastChanged = lastChanged,
												CompanyCode = cart.Payment.CreditCardCompany,
												CreditToken = cart.Payment.CreditToken,
											},
											lastChanged,
											UpdateHistoryAction.DoNotInsert,
											sqlAccessor);
									}
									// ペイジェントクレカ以外の処理
									else if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Paygent)
									{
										userCreditCard = new UserCreditCardRegister().ExecOnlySave(
											new UserCreditCardInput
											{
												UserId = cart.OrderUserId,
												CardDispName = StringUtility.ToEmpty(cart.Payment.UserCreditCardName),
												CardNo = lastFourDigit,
												ExpirationMonth = cart.Payment.CreditExpireMonth,
												ExpirationYear = cart.Payment.CreditExpireYear,
												AuthorName = cart.Payment.CreditAuthorName,
												DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
												LastChanged = lastChanged,
												CompanyCode = cart.Payment.CreditCardCompany,
												CreditToken = cart.Payment.CreditToken,
											},
											lastChanged,
											UpdateHistoryAction.DoNotInsert,
											sqlAccessor);
									}
									// ペイジェントクレカのみの処理
									else
									{
										var result = new UserCreditCardRegister().Exec(
											new UserCreditCardInput
											{
												UserId = cart.OrderUserId,
												CardDispName = StringUtility.ToEmpty(cart.Payment.UserCreditCardName),
												CardNo = lastFourDigit,
												ExpirationMonth = cart.Payment.CreditExpireMonth,
												ExpirationYear = cart.Payment.CreditExpireYear,
												AuthorName = cart.Payment.CreditAuthorName,
												DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
												LastChanged = lastChanged,
												CompanyCode = cart.Payment.CreditCardCompany,
												CreditToken = cart.Payment.CreditToken,
											},
											SiteKbn.Pc,
											false,
											lastChanged,
											UpdateHistoryAction.Insert,
											sqlAccessor,
											true);
										userCreditCard = new UserCreditCardService().Get(cart.OrderUserId, result.BranchNo, sqlAccessor);
									}
									cart.Payment.UserCreditCard = new UserCreditCard(userCreditCard);
									// 取得したカード情報の連携IDを格納
									order.Add(Constants.FIELD_USERCREDITCARD_COOPERATION_ID, cart.Payment.UserCreditCard.CooperationId);
									order.Add(Constants.FIELD_USERCREDITCARD_COOPERATION_ID2, cart.Payment.UserCreditCard.CooperationId2);
									order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = cart.Payment.UserCreditCard.BranchNo;
								}
							}
							else
							{
								transactionName = "1-7.クレジットカード情報取得処理";
								cart.Payment.UserCreditCard = new UserCreditCard(
									new UserCreditCardService().Get(cart.OrderUserId, int.Parse(cart.Payment.CreditCardBranchNo), sqlAccessor));
								order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = cart.Payment.UserCreditCard.BranchNo;
							}

							// クレジットカード枝番更新
							new OrderService().UpdateCreditBranchNo(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(int)order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO],
								lastChanged,
								UpdateHistoryAction.DoNotInsert,
								sqlAccessor);
						}
						// 仮クレジットカード登録（与信をしない）
						else if ((cart.Payment.PaymentId == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
							&& OrderCommon.CreditCardRegistable)
						{
							transactionName = "1-7.仮クレジットカード情報取得処理";

							cart.Payment.UserCreditCard = new ProvisionalCreditCardProcessor().RegisterUnregisterdCreditCard(
								cart.OrderUserId,
								StringUtility.ToEmpty(cart.Payment.UserCreditCardName),
								Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_ORDER_REGISTER,
								"",
								Constants.FLG_ORDER_ORDER_STATUS_ORDERED,
								lastChanged,
								UpdateHistoryAction.DoNotInsert,
								sqlAccessor);
							order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = cart.Payment.UserCreditCard.BranchNo;
						}
						// PalPalの情報もクレジットカードに格納
						else if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
						{
							transactionName = "1-7. PalPalユーザークレジットカード情報登録処理";

							var userCreditCard =
								(cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
									? PayPalUtility.Payment.RegisterAsUserCreditCard(
										cart.OrderUserId,
										cart.PayPalCooperationInfo,
										lastChanged,
										UpdateHistoryAction.DoNotInsert,
										sqlAccessor)
									: new UserCreditCardService().Get(
										cart.CartUserId,
										int.Parse(cart.Payment.CreditCardBranchNo),
										sqlAccessor);
							cart.Payment.UserCreditCard = new UserCreditCard(userCreditCard);
							order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = cart.Payment.UserCreditCard.BranchNo;
						}
						// Paidyの情報もクレジットカードに格納
						else if (cart.Payment.IsPaymentDirectPaidy)
						{
							transactionName = "1-7. Paidyユーザークレジットカード情報登録処理";

							UserCreditCardModel userCreditCard;
							if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
							{
								userCreditCard = new UserCreditCardService().GetByCooperationId1(cart.Payment.PaidyToken);
								userCreditCard = userCreditCard
									?? PaidyUtility.RegisterAsUserCreditCard(
										cart.OrderUserId,
										cart.Payment.PaidyToken,
										lastChanged,
										UpdateHistoryAction.DoNotInsert,
										sqlAccessor);
							}
							else
							{
								userCreditCard = new UserCreditCardService().Get(
									cart.CartUserId,
									int.Parse(cart.Payment.CreditCardBranchNo),
									sqlAccessor);
							}

							cart.Payment.UserCreditCard = new UserCreditCard(userCreditCard);
							order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = cart.Payment.UserCreditCard.BranchNo;
						}
						// EcPayの情報もクレジットカードに格納
						else if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
							&& (cart.Payment.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT))
						{
							transactionName = "1-7. EcPayユーザークレジットカード情報登録処理";

							UserCreditCardModel userCreditCard;
							if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
							{
								userCreditCard = new UserCreditCardService().GetByCooperationId1(cart.Payment.CreditCardBranchNo);
								userCreditCard = userCreditCard
									?? ECPayUtility.RegisterAsUserCreditCard(
										cart.OrderUserId,
										lastChanged,
										UpdateHistoryAction.DoNotInsert,
										sqlAccessor);
							}
							else
							{
								userCreditCard = new UserCreditCardService().Get(
									cart.CartUserId,
									int.Parse(cart.Payment.CreditCardBranchNo),
									sqlAccessor);
							}

							cart.Payment.UserCreditCard = new UserCreditCard(userCreditCard);
							order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = cart.Payment.UserCreditCard.BranchNo;
						}
						// NewebPayの情報もクレジットカードに格納
						else if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
							&& (cart.Payment.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT))
						{
							transactionName = "1-7. 藍新Payユーザークレジットカード情報登録処理";

							UserCreditCardModel userCreditCard;
							if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
							{
								userCreditCard = new UserCreditCardService().GetByCooperationId1(cart.Payment.CreditCardBranchNo);
								userCreditCard = userCreditCard
									?? NewebPayUtility.RegisterAsUserCreditCard(
										cart.OrderUserId,
										lastChanged,
										UpdateHistoryAction.DoNotInsert,
										sqlAccessor);
							}
							else
							{
								userCreditCard = new UserCreditCardService().Get(
									cart.CartUserId,
									int.Parse(cart.Payment.CreditCardBranchNo),
									sqlAccessor);
							}

							cart.Payment.UserCreditCard = new UserCreditCard(userCreditCard);
							order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = cart.Payment.UserCreditCard.BranchNo;
						}
						else if (cart.Payment.IsPaymentYamatoKaSms)
						{
							transactionName = "1-7. ヤマト後払いSMS認証連携クレジットカード情報登録処理";

							UserCreditCardModel userCreditCard;
							if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
							{
								userCreditCard = new UserCreditCardService().GetByCooperationId1(cart.Payment.CreditCardBranchNo);
								userCreditCard = userCreditCard
									?? RegisterTelNumForYamatoKaSmsAsUserCreditCard(
										cart.OrderUserId,
										cart.Owner.Tel1,
										lastChanged,
										UpdateHistoryAction.DoNotInsert,
										sqlAccessor);
							}
							else
							{
								userCreditCard = new UserCreditCardService().Get(
									cart.CartUserId,
									int.Parse(cart.Payment.CreditCardBranchNo),
									sqlAccessor);
							}

							cart.Payment.UserCreditCard = new UserCreditCard(userCreditCard);
							order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = cart.Payment.UserCreditCard.BranchNo;
						}
					}

					//------------------------------------------------------
					// １－８． 定期購入処理
					//------------------------------------------------------
					if (success && (Constants.FIXEDPURCHASE_OPTION_ENABLED && cart.HasFixedPurchase) || (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && cart.HasSubscriptionBox))
					{
						// 今すぐ注文での場合、もしくは注文同梱で既存の注文台帳を更新する場合スキップ(親注文定期購購入なしでかつ子注文定期購入あり)
						// 注文同梱時、頒布会が含まれる場合は子注文は別台帳扱いになるため登録する（頒布会同コース同士であればスキップ）
						if (((cart.IsOrderCombined == false)
								|| cart.IsRegisterFixedPurchaseWhenOrderCombine)
							&& string.IsNullOrEmpty((string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]))
						{
							transactionName = "1-8-1.定期購入仮登録処理";
							success = RegistPreFixedPurchaseOrder(order, cart, lastChanged, UpdateHistoryAction.Insert, sqlAccessor);
						}

						transactionName = "1-8-2.定期継続分析処理";
						if (success) InsertFixedPurchaseRepeatAnalysis(order, cart, lastChanged, sqlAccessor);
					}

					//------------------------------------------------------
					// １－９． リアルタイム累計購入回数更新処理
					//------------------------------------------------------
					if (success)
					{
						transactionName = "1-9.リアルタイム累計購入回数更新処理";
						success = OrderCommon.UpdateRealTimeOrderCount(order, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ORDER, sqlAccessor);
					}

					// FLAPS注文連携
					if (Constants.FLAPS_OPTION_ENABLE && success)
					{
						transactionName = "1-10.FLAPS注文連携処理";

						var posNoSerNo = new FlapsIntegrationFacade().ProcessOrder(order, cart, sqlAccessor);
						order[Constants.FLAPS_ORDEREXTENDSETTING_ATTRNO_FOR_POSNOSERNO] = posNoSerNo;
						success = (string.IsNullOrEmpty(posNoSerNo) == false);
					}

					//------------------------------------------------------
					// １－Ｘ．成功/失敗時処理
					//------------------------------------------------------
					// 成功時処理
					if (success)
					{
						// 更新履歴登録
						if (updateHistoryAction == UpdateHistoryAction.Insert)
						{
							new UpdateHistoryService().InsertForOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], lastChanged, sqlAccessor);
							new UpdateHistoryService().InsertForUser(cart.OrderUserId, lastChanged, sqlAccessor);
						}

						// トランザクションコミット
						sqlAccessor.CommitTransaction();
					}
					// 失敗時処理
					else
					{
						throw new Exception("仮注文登録処理でエラーが発生しました。");
					}
				}
				// カートエラー
				catch (CartException ex)
				{
					success = false;
					AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage(transactionName, order, cart), ex);
					throw new CartException(ex.Message);
				}
				// その他のエラー？
				catch (Exception ex)
				{
					success = false;
					try
					{
						// トランザクションロールバック
						sqlAccessor.RollbackTransaction();
					}
					catch (Exception ex2)
					{
						AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage(transactionName, order, cart) + ex.ToString(), ex2);
					}

					// FLAPS連携の注文キャンセルAPI処理
					if (Constants.FLAPS_OPTION_ENABLE)
					{
						try
						{
							new FlapsIntegrationFacade().CancelOrder(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								StringUtility.ToEmpty(order[Constants.FLAPS_ORDEREXTENDSETTING_ATTRNO_FOR_POSNOSERNO]));
						}
						catch (Exception flapsEx)
						{
							// FLAPSのキャンセルAPI処理に失敗してもログを残すのみで例外スローしない
							AppLogger.WriteError(
								OrderCommon.CreateOrderFailedLogMessage(transactionName, order, cart) + ex,
								flapsEx);
						}
					}

					// 商品在庫チェック or シリアルキー引当例外の場合
					if ((ex is ProductStockException) || (ex is ReserveSerialKeyException))
					{
						throw;
					}
					AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage(transactionName, order, cart), ex);
				}

				var nextEngineTempOrderSync = NextEngineTempOrderSync.GetNextEngineTempOrderSync(order);
				nextEngineTempOrderSync.SetTempOrderSyncFlg(success);

				return success;
			}
		}

		/// <summary>
		/// ヤマト後払いSMS認証用クレジットカード情報登録
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="ownerTelNum">注文者電話番号</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード</returns>
		public static UserCreditCardModel RegisterTelNumForYamatoKaSmsAsUserCreditCard(
			string userId,
			string ownerTelNum,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			var userCreditCard = new UserCreditCardModel
			{
				UserId = userId,
				CooperationId = ownerTelNum,
				CooperationId2 = string.Empty,
				CardDispName = "YamatoKaSms",
				LastFourDigit = string.Empty,
				ExpirationMonth = string.Empty,
				ExpirationYear = string.Empty,
				AuthorName = string.Empty,
				DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
				LastChanged = lastChanged,
				CompanyCode = string.Empty,
				CooperationType = Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_YAMATOKASMS,
			};

			new UserCreditCardService().Insert(
				userCreditCard,
				updateHistoryAction,
				accessor);
			return userCreditCard;
		}

		/// <summary>
		/// 注文情報のINSERT処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情</param>
		/// <param name="isUser">会員か否か</param>
		/// <param name="isFirstCart">最初のカートか否か</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="execType">注文実行種別</param>
		/// <param name="registGuestUser">ゲストユーザー登録するか</param>
		private void InsertOrder(Hashtable order, CartObject cart, bool isUser, bool isFirstCart, SqlAccessor sqlAccessor, OrderRegisterBase.ExecTypes execType, bool registGuestUser)
		{
			using (SqlStatement sqlStatement = new SqlStatement("Order", "OrderRegist"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]);
				input.Add(Constants.FIELD_ORDER_USER_ID, (string)order[Constants.FIELD_ORDER_USER_ID]);
				input.Add(Constants.FIELD_ORDER_SHOP_ID, cart.ShopId);
				input.Add(Constants.FIELD_ORDER_ORDER_KBN, (string)order[Constants.FIELD_ORDER_ORDER_KBN]);
				input.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, cart.Payment.PaymentId);
				input.Add(Constants.FIELD_ORDER_ORDER_STATUS, (string)order[Constants.FIELD_ORDER_ORDER_STATUS]);
				input.Add(Constants.FIELD_ORDER_ORDER_ITEM_COUNT, cart.Items.Count);
				int productCount = 0;
				foreach (CartProduct cp in cart.Items)
				{
					productCount += cp.Count;
				}
				input.Add(Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT, productCount);
				input.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, cart.PriceSubtotal);
				input.Add(Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING, cart.PriceShipping);					// 配送料
				input.Add(Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE, cart.Payment.PriceExchange);			// 決済手数料
				input.Add(Constants.FIELD_ORDER_ORDER_PRICE_REGULATION, cart.PriceRegulation);				// 調整金額
				input.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, cart.PriceTotal);						// 支払金額合計
				input.Add(Constants.FIELD_ORDER_LAST_BILLED_AMOUNT, cart.PriceTotal);						// 最終請求金額
				decimal discountSetPrice = 0;
				foreach (CartProduct cp in cart.Items)
				{
					if (cp.IsSetItem) discountSetPrice += (cp.PriceOrg - cp.Price) * cp.Count;
				}

				input.Add(Constants.FIELD_ORDER_ORDER_DISCOUNT_SET_PRICE, discountSetPrice);					// セット値引金額

				// 最初のカートではなく、初回購入ポイント付与区分が固定値の場合、カートの初回購入ポイントを0にする
				// （初回購入ポイントが固定値付与の場合、最初のカートのみ初回購入ポイントを割り当てる）
				if ((isFirstCart == false) && (cart.FirstBuyPointKbn == Constants.FLG_POINTRULE_INC_TYPE_NUM))
				{
					cart.FirstBuyPoint = 0;
				}

				if (Constants.MEMBER_RANK_OPTION_ENABLED)
				{
					string rankId = MemberRankOptionUtility.GetMemberRankId((string)order[Constants.FIELD_ORDER_USER_ID]);
					// 管理画面からの注文且つユーザー新規登録有の場合のみ、会員ランクが入っていなければデフォルトの会員ランク設定
					if ((execType == OrderRegisterBase.ExecTypes.CommerceManager)
						&& isUser
						&& registGuestUser
						&& (string.IsNullOrEmpty(rankId)))
					{
						rankId = MemberRankOptionUtility.GetDefaultMemberRank();
					}
					input.Add(Constants.FIELD_ORDER_MEMBER_RANK_ID, rankId);								// 注文時会員ランク
					input.Add(Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE, cart.MemberRankDiscount);	// 会員ランク割引額
					input.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT, cart.FixedPurchaseMemberDiscountAmount);		// 定期会員割引額
				}
				else
				{
					input.Add(Constants.FIELD_ORDER_MEMBER_RANK_ID, "");
					input.Add(Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE, 0);	// 会員ランク割引額
					input.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT, 0);		// 定期会員割引額
				}

				input.Add(Constants.FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT, cart.SetPromotions.ProductDiscountAmount);
				input.Add(Constants.FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT, (cart.SetPromotions.ShippingChargeDiscountAmount));
				input.Add(Constants.FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT, (cart.SetPromotions.PaymentChargeDiscountAmount));

				var isPointUseable = (Constants.W2MP_POINT_OPTION_ENABLED && isUser);
				decimal orderPointUse = isPointUseable ? cart.UsePoint : 0;
				decimal orderPointUseYen = isPointUseable ? cart.UsePointPrice : 0;
				input.Add(Constants.FIELD_ORDER_ORDER_POINT_USE, orderPointUse);
				input.Add(Constants.FIELD_ORDER_ORDER_POINT_USE_YEN, orderPointUseYen);
				input.Add(Constants.FIELD_ORDER_ORDER_POINT_ADD, isPointUseable ? cart.BuyPoint + cart.FirstBuyPoint : 0);
				// 最終利用ポイント数
				input.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE, orderPointUse);
				// 最終ポイント利用額
				input.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN, orderPointUseYen);

				input.Add(Constants.FIELD_ORDER_ORDER_COUPON_USE, cart.UseCouponPrice);						// クーポン割引額

				// 定期購入割引額
				input.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE, (Constants.FIXEDPURCHASE_OPTION_ENABLED) ? cart.FixedPurchaseDiscount : (decimal)0);

				if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					input.Add(Constants.FIELD_ORDER_CARD_KBN, StringUtility.ToEmpty(cart.Payment.CreditCardCompany));
				}
				else
				{
					input.Add(Constants.FIELD_ORDER_CARD_KBN, "");
				}
				if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && OrderCommon.CreditInstallmentsSelectable)
				{
					input.Add(Constants.FIELD_ORDER_CARD_INSTRUMENTS, ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, cart.Payment.CreditInstallmentsCode));
					input.Add(Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE, cart.Payment.CreditInstallmentsCode);
				}
				else if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (cart.Payment.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT))
				{
					input[Constants.FIELD_ORDER_CARD_INSTRUMENTS] = string.Empty;
					input[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE] = (string.IsNullOrEmpty(cart.Payment.NewebPayCreditInstallmentsCode))
						? NewebPayConstants.FLG_CREDIT_CARD_ONCE_TIME
						: cart.Payment.NewebPayCreditInstallmentsCode;
				}
				else
				{
					input.Add(Constants.FIELD_ORDER_CARD_INSTRUMENTS, "");
					input.Add(Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE, "");
				}
				input.Add(Constants.FIELD_ORDER_PAYMENT_ORDER_ID, order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);	// 決済注文ID

				input.Add(Constants.FIELD_ORDER_SHIPPING_ID, cart.ShippingType);							// 配送種別ID

				// 注文メモ
				if (string.IsNullOrEmpty(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_MEMO])))
				{
					input.Add(Constants.FIELD_ORDER_MEMO, cart.GetOrderMemos());
				}
				else
				{
					input.Add(Constants.FIELD_ORDER_MEMO, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_MEMO]));
				}

				input.Add(Constants.FIELD_ORDER_ORDER_DATE, order[Constants.FIELD_ORDER_ORDER_DATE]);	// 注文日時
				input.Add(Constants.FIELD_ORDER_DATE_CREATED, order[Constants.FIELD_ORDER_ORDER_DATE]);	// 作成日
				input.Add(Constants.FIELD_ORDER_DATE_CHANGED, order[Constants.FIELD_ORDER_ORDER_DATE]);	// 更新日

				input[Constants.FIELD_ORDER_CAREER_ID] = order[Constants.FIELD_ORDER_CAREER_ID];		// キャリアID
				input[Constants.FIELD_ORDER_MOBILE_UID] = order[Constants.FIELD_ORDER_MOBILE_UID];		// モバイルUID
				input[Constants.FIELD_ORDER_REMOTE_ADDR] = order[Constants.FIELD_ORDER_REMOTE_ADDR];	// リモートIPアドレス

				// 広告コード関連
				input[Constants.FIELD_ORDER_ADVCODE_FIRST] = order[Constants.FIELD_ORDER_ADVCODE_FIRST];
				input[Constants.FIELD_ORDER_ADVCODE_NEW] = order[Constants.FIELD_ORDER_ADVCODE_NEW];

				// 汎用アフィリエイト関連
				input[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME1] = order[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME1];
				input[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE1] = order[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE1];
				input[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME2] = order[Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME2];
				input[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE2] = order[Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE2];
				input[Constants.FIELD_ORDER_USER_AGENT] = order[Constants.FIELD_ORDER_USER_AGENT];

				// 定期購入ID
				input[Constants.FIELD_ORDER_FIXED_PURCHASE_ID] = order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID];
				// 定期購入回数(注文時点)
				input[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT] = order[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT];

				// 管理メモの設定
				input[Constants.FIELD_ORDER_MANAGEMENT_MEMO] = string.Empty;
				if (order.Contains(Constants.FIELD_ORDER_MANAGEMENT_MEMO))
				{
					input[Constants.FIELD_ORDER_MANAGEMENT_MEMO] = order[Constants.FIELD_ORDER_MANAGEMENT_MEMO];
				}

				// 配送メモ
				input.Add(
					Constants.FIELD_ORDER_SHIPPING_MEMO,
					order.Contains(Constants.FIELD_ORDER_SHIPPING_MEMO)
						? order[Constants.FIELD_ORDER_SHIPPING_MEMO]
						: "");

				// 調整金額メモ
				input.Add(Constants.FIELD_ORDER_REGULATION_MEMO, StringUtility.ToEmpty(cart.RegulationMemo));

				// ギフトフラグ
				input[Constants.FIELD_ORDER_GIFT_FLG] = cart.IsGift ? Constants.FLG_ORDER_GIFT_FLG_ON : Constants.FLG_ORDER_GIFT_FLG_OFF;

				// デジタルコンテンツフラグ
				input[Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG] = cart.IsDigitalContentsOnly ? Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON : Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_OFF;

				// 配送料別途見積もり表示フラグ
				input[Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] = cart.ShippingPriceSeparateEstimateFlg ? Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID : Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID;

				// 税関係
				input[Constants.FIELD_ORDER_ORDER_TAX_INCLUDED_FLG] = TaxCalculationUtility.GetPrescribedOrderTaxIncludedFlag();
				input[Constants.FIELD_ORDER_ORDER_TAX_RATE] = "0";
				input[Constants.FIELD_ORDER_ORDER_TAX_ROUND_TYPE] = Constants.TAX_EXCLUDED_FRACTION_ROUNDING;

				// 最終与信フラグ設定
				input[Constants.FIELD_ORDER_LAST_AUTH_FLG] = Constants.FLG_ORDER_LAST_AUTH_FLG_ON;

				input[Constants.FIELD_ORDER_LAST_CHANGED] = order[Constants.FIELD_ORDER_LAST_CHANGED];

				// 注文同梱元注文Id設定
				input.Add(Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS, string.Empty);
				if (order.Contains(Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS))
				{
					input[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS] = (string)order[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS];
				}

				// 外部連携メモ
				input.Add(Constants.FIELD_ORDER_RELATION_MEMO, string.Empty);
				if (order.Contains(Constants.FIELD_ORDER_RELATION_MEMO))
				{
					input[Constants.FIELD_ORDER_RELATION_MEMO] = (string)order[Constants.FIELD_ORDER_RELATION_MEMO];
				}

				// 注文拡張フラグ39設定
				input.Add(Constants.FIELD_ORDER_EXTEND_STATUS39, Constants.FLG_ORDER_EXTEND_STATUS_OFF);
				if (order.Contains(Constants.FIELD_ORDER_EXTEND_STATUS39))
				{
					input[Constants.FIELD_ORDER_EXTEND_STATUS39] = order[Constants.FIELD_ORDER_EXTEND_STATUS39];
				}

				input.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX, cart.PriceSubtotalTax);							// 小計税額
				input.Add(Constants.FIELD_ORDER_ORDER_PRICE_TAX, cart.PriceTax);							// 税額総計

				input.Add(Constants.FIELD_ORDER_SETTLEMENT_CURRENCY, cart.SettlementCurrency);					// 決済通貨
				input.Add(Constants.FIELD_ORDER_SETTLEMENT_RATE, cart.SettlementRate);							// 決済レート
				input.Add(Constants.FIELD_ORDER_SETTLEMENT_AMOUNT, cart.SettlementAmount);						// 決済金額

				input.Add(Constants.FIELD_ORDER_SHIPPING_TAX_RATE, cart.ShippingTaxRate);	// 配送料税率
				input.Add(Constants.FIELD_ORDER_PAYMENT_TAX_RATE, cart.PaymentTaxRate);	// 決済手数料税率

				// リアルタイム累計購入回数取得
				var user = new UserService().Get((string)order[Constants.FIELD_ORDER_USER_ID], sqlAccessor);
				var orderCount = ((user != null) ? user.OrderCountOrderRealtime : 0);
				input[Constants.FIELD_ORDER_ORDER_COUNT_ORDER] = orderCount + 1;			//購入回数
				// 請求書同梱フラグ
				input.Add(Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG, cart.GetInvoiceBundleFlg());

				// コンバージョン情報を格納
				input.Add(Constants.FIELD_ORDER_INFLOW_CONTENTS_ID, (cart.ContentsLog != null) ? cart.ContentsLog.ContentsId : string.Empty);
				input.Add(Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE, (cart.ContentsLog != null) ? cart.ContentsLog.ContentsType : string.Empty);

				// 領収書情報
				input.Add(Constants.FIELD_ORDER_RECEIPT_FLG, cart.ReceiptFlg ?? Constants.FLG_ORDER_RECEIPT_FLG_OFF);
				input.Add(Constants.FIELD_ORDER_RECEIPT_ADDRESS, cart.ReceiptAddress ?? "");
				input.Add(Constants.FIELD_ORDER_RECEIPT_PROVISO, cart.ReceiptProviso ?? "");
				input[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE] = StringUtility.ToEmpty(cart.Payment.ExternalPaymentType);

				// 頒布会情報
				input[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID] = cart.SubscriptionBoxCourseId;
				input[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT] = cart.SubscriptionBoxFixedAmount;
				input[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT] = (int?)order[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT];

				if (Constants.BOTCHAN_OPTION && cart.IsBotChanOrder)
				{
					input.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE40, DateTime.Now);
					input.Add(Constants.FIELD_ORDER_EXTEND_STATUS40, Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON);
				}

				foreach (var field in Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST)
				{
					if (input.Contains(field) == false)
					{
						input.Add(field, string.Empty);
					}
					input[field] = (cart.OrderExtend.ContainsKey(field))
						? cart.OrderExtend[field].Value
						: string.Empty;
				}

				// 店舗受取ステータス
				input[Constants.FIELD_ORDER_STOREPICKUP_STATUS] =
					cart.Shippings.Any(shipping => string.IsNullOrEmpty(shipping.RealShopId) == false)
						? Constants.FLG_STOREPICKUP_STATUS_PENDING
						: string.Empty;

				// ステートメント実行
				sqlStatement.ExecStatement(sqlAccessor, input);
			}
		}

		/// <summary>
		/// 注文者情報のINSERT処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="owner">注文者情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private void InsertOrderOwner(Hashtable order, CartOwner owner, SqlAccessor sqlAccessor)
		{
			using (SqlStatement sqlStatement = new SqlStatement("Order", "OrderOwnerRegist"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_ORDEROWNER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, owner.OwnerKbn);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME, owner.Name);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME1, owner.Name1);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME2, owner.Name2);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA, owner.NameKana);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1, owner.NameKana1);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2, owner.NameKana2);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR, owner.MailAddr);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2, owner.MailAddr2);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_ZIP, owner.Zip);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR1, owner.Addr1);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR2, owner.Addr2);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR3, owner.Addr3);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR4, owner.Addr4);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR5, owner.Addr5);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE, owner.AddrCountryIsoCode);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME, owner.AddrCountryName);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME, owner.CompanyName); // 企業名
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME, owner.CompanyPostName); // 部署名
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL1, owner.Tel1);
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_SEX, (owner.Sex != null) ? owner.Sex : Constants.FLG_USER_SEX_UNKNOWN);										// 性別
				input.Add(Constants.FIELD_ORDEROWNER_OWNER_BIRTH, owner.Birth);
				input.Add(Constants.FIELD_ORDEROWNER_DATE_CREATED, order[Constants.FIELD_ORDER_ORDER_DATE]);
				input.Add(Constants.FIELD_ORDEROWNER_DATE_CHANGED, order[Constants.FIELD_ORDER_ORDER_DATE]);
				input.Add(Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE, owner.AccessCountryIsoCode);
				input.Add(Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE, owner.DispLanguageCode);
				input.Add(Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID, owner.DispLanguageLocaleId);
				input.Add(Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE, owner.DispCurrencyCode);
				input.Add(Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID, owner.DispCurrencyLocaleId);

				sqlStatement.ExecStatement(sqlAccessor, input);
			}
		}

		/// <summary>
		/// 注文配送先情報のINSERT処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private void InsertOrderShipping(Hashtable order, CartObject cart, SqlAccessor sqlAccessor)
		{
			int index = 1;
			foreach (var shipping in cart.Shippings)
			{
				var shopShipping = new ShopShippingService().Get(cart.ShopId, cart.ShippingType);
				var isShippingMethodMail = cart.Shippings[0].IsMail;
				shipping.ShippingMethod = (cart.Shippings[0].IsMail)
					? OrderCommon.GetShippingMethod(cart.Items)
					: Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;

				if ((shipping.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					&& isShippingMethodMail)
				{
					shipping.DeliveryCompanyId = shopShipping.CompanyListExpress.FirstOrDefault(item => item.IsDefault).DeliveryCompanyId;
				}

				// TODO:出来たら枝番渡さなくていいようにする。（Shippingsをクラス化してSHippingに常に枝番持たせるようにする？）
				InsertOrderShipping(order, shipping, index, sqlAccessor, cart.IsCombineParentOrderHasFixedPurchase);

				if (OrderCommon.DisplayTwInvoiceInfo()
					&& shipping.IsShippingAddrTw)
				{
					// Insert Invoice
					InsertOrderInvoice(
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
						shipping,
						index,
						(DateTime)order[Constants.FIELD_ORDER_ORDER_DATE],
						sqlAccessor);
				}

				index++;
			}
		}
		/// <summary>
		/// 注文配送先情報のINSERT処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="shipping">配送先情報</param>
		/// <param name="orderShippingNo">配送先枝番</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="isCombineParentOrderHasFixedPurchase">注文同梱の親注文は定期注文があるか</param>
		private void InsertOrderShipping(Hashtable order, CartShipping shipping, int orderShippingNo, SqlAccessor sqlAccessor, bool isCombineParentOrderHasFixedPurchase)
		{
			using (SqlStatement sqlStatement = new SqlStatement("Order", "OrderShippingRegist"))
			{
				Hashtable input = new Hashtable();

				// 配送先
				input.Add(Constants.FIELD_ORDERSHIPPING_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]);
				input.Add(Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO, orderShippingNo);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME, shipping.Name);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1, shipping.Name1);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2, shipping.Name2);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA, shipping.NameKana);	// 配送先氏名かな
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1, shipping.NameKana1);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2, shipping.NameKana2);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP, shipping.Zip);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, shipping.Addr1);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2, shipping.Addr2);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3, shipping.Addr3);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4, shipping.Addr4);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5, shipping.Addr5);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME, shipping.ShippingCountryName);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE, shipping.ShippingCountryIsoCode);
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME, shipping.CompanyName);	// 企業名
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME, shipping.CompanyPostName);	// 部署名
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1, shipping.Tel1);

				// 差出人（ギフト用）
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME1, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME2, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ZIP, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_TEL1, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME, string.Empty);
				input.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5, string.Empty);
				if (shipping.CartObject.IsGift)
				{
					input[Constants.FIELD_ORDERSHIPPING_SENDER_NAME] = StringUtility.ToEmpty(shipping.SenderName1 + shipping.SenderName2);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_NAME1] = StringUtility.ToEmpty(shipping.SenderName1);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_NAME2] = StringUtility.ToEmpty(shipping.SenderName2);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA] = StringUtility.ToEmpty(shipping.SenderNameKana1 + shipping.SenderNameKana2);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1] = StringUtility.ToEmpty(shipping.SenderNameKana1);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2] = StringUtility.ToEmpty(shipping.SenderNameKana2);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP] = shipping.SenderZip;
					input[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1] = StringUtility.ToEmpty(shipping.SenderAddr1);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2] = StringUtility.ToEmpty(shipping.SenderAddr2);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3] = StringUtility.ToEmpty(shipping.SenderAddr3);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4] = StringUtility.ToEmpty(shipping.SenderAddr4);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5] = StringUtility.ToEmpty(shipping.SenderAddr5);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME] = StringUtility.ToEmpty(shipping.SenderCompanyName);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME] = StringUtility.ToEmpty(shipping.SenderCompanyPostName);
					input[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1] = shipping.SenderTel1;
					input[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE] = shipping.SenderCountryIsoCode;
					input[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME] = shipping.SenderCountryName;
				}

				// 配送希望日入力可 & 配送希望日指定済みの場合
				if ((shipping.SpecifyShippingDateFlg && (shipping.ShippingDate != DateTime.MinValue))
					|| isCombineParentOrderHasFixedPurchase
					|| (shipping.ShippingDate != null))
				{
					input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, shipping.ShippingDate);
				}
				else
				{
					input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, DBNull.Value);
				}

				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, shipping.ShippingMethod);
				input.Add(Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID, shipping.DeliveryCompanyId);

				// 配送希望時間帯入力可 & 配送希望時間帯指定済み場合
				if (shipping.SpecifyShippingTimeFlg && (shipping.ShippingTime != null))
				{
					input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, shipping.ShippingTime);
				}
				else
				{
					input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, "");
				}

				// のし情報（ギフトのみ）
				if (shipping.WrappingPaperValidFlg)
				{
					input.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE, StringUtility.ToEmpty(shipping.WrappingPaperType));
					input.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME, StringUtility.ToEmpty(shipping.WrappingPaperName));
				}
				else
				{
					input.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE, "");
					input.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME, "");
				}
				// 包装情報（ギフトのみ）
				if (shipping.WrappingBagValidFlg)
				{
					input.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE, StringUtility.ToEmpty(shipping.WrappingBagType));
				}
				else
				{
					input.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE, "");
				}
				input.Add(Constants.FIELD_ORDERSHIPPING_DATE_CREATED, order[Constants.FIELD_ORDER_ORDER_DATE]);
				input.Add(Constants.FIELD_ORDERSHIPPING_DATE_CHANGED, order[Constants.FIELD_ORDER_ORDER_DATE]);
				input.Add(Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG, StringUtility.ToEmpty(shipping.AnotherShippingFlag));
				input.Add(Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE, shipping.ScheduledShippingDate);

				// Insert Convenience Store Addr
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID, StringUtility.ToEmpty(shipping.ConvenienceStoreId));
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG, StringUtility.ToEmpty(shipping.ConvenienceStoreFlg));

				// Insert shipping receiving store type for EcPay
				input.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE, StringUtility.ToEmpty(shipping.ShippingReceivingStoreType));

				// Insert store pickup real shop id
				input[Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID] = StringUtility.ToEmpty(shipping.RealShopId);

				sqlStatement.ExecStatement(sqlAccessor, input);
			}
		}

		/// <summary>
		/// Insert Order Invoice
		/// </summary>
		/// <param name="orderId">OrderId</param>
		/// <param name="shipping">Shipping</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <param name="orderTime">注文日時</param>
		/// <param name="accessor">Accessor</param>
		private void InsertOrderInvoice(
			string orderId,
			CartShipping shipping,
			int orderShippingNo,
			DateTime orderTime,
			SqlAccessor accessor)
		{
			var model = new TwOrderInvoiceModel
			{
				OrderId = orderId,
				OrderShippingNo = orderShippingNo,
				TwUniformInvoice = StringUtility.ToEmpty(shipping.UniformInvoiceType),
				TwUniformInvoiceOption1 = StringUtility.ToEmpty(shipping.UniformInvoiceOption1),
				TwUniformInvoiceOption2 = StringUtility.ToEmpty(shipping.UniformInvoiceOption2),
				TwCarryType = StringUtility.ToEmpty(shipping.CarryType),
				TwCarryTypeOption = StringUtility.ToEmpty(shipping.CarryTypeOptionValue),
				TwInvoiceDate = orderTime
			};

			// Call Api Get Invoice
			model.TwInvoiceNo = string.Empty;
			model.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_NOT_ISSUED;

			new TwOrderInvoiceService().Insert(model, accessor);
		}

		/// <summary>
		/// 注文商品情報のINSERT処理とシリアルキーのRESERVE処理。
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>注文商品登録結果（シリアルキーが不足して引き当てに失敗したときには .Resultにはfalseがセットされる）</returns>
		private InsertOrderItemResult InsertOrderItem(Hashtable order, CartObject cart, SqlAccessor sqlAccessor)
		{
			// 通常注文？
			if (cart.IsGift == false)
			{
				var itemIndex = 1;
				// 通常/セット商品登録(同梱商品を除く)
				foreach (var product in cart.Items.Where(cp => ((cp.QuantitiyUnallocatedToSet != 0) && (cp.IsBundle == false))))
				{
					var count = product.IsSetItem ? product.Count : product.QuantitiyUnallocatedToSet;
					var countSingle = product.IsSetItem ? product.CountSingle : product.QuantitiyUnallocatedToSet;

					InsertOrderItem(
						order,
						product,
						count,
						countSingle,
						itemIndex,
						1,
						null,
						null,
						cart.Shippings[0].IsDutyFree,
						sqlAccessor);
					if (ReserveSerialKey(order, product, product.Count, itemIndex, sqlAccessor) == false)
					{
						return new InsertOrderItemResult(false, product);
					}

					itemIndex++;
				}

				// セットプロモーション商品登録
				foreach (CartSetPromotion setpromotion in cart.SetPromotions)
				{
					var setPromotionItemIndex = 1;
					foreach (var product in setpromotion.Items)
					{
						var quantity = product.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo];

						InsertOrderItem(
							order,
							product,
							quantity,
							quantity,
							itemIndex,
							1,
							setpromotion.CartSetPromotionNo,
							setPromotionItemIndex,
							cart.Shippings[0].IsDutyFree,
							sqlAccessor);
						if (ReserveSerialKey(order, product, product.Count, itemIndex, sqlAccessor) == false)
						{
							return new InsertOrderItemResult(false, product);
						}

						itemIndex++;
						setPromotionItemIndex++;
					}
				}

				// 同梱商品を登録
				foreach (var product in cart.Items.Where(cp => ((cp.QuantitiyUnallocatedToSet != 0) && cp.IsBundle)))
				{
					var count = product.IsSetItem ? product.Count : product.QuantitiyUnallocatedToSet;
					var countSingle = product.IsSetItem ? product.CountSingle : product.QuantitiyUnallocatedToSet;

					InsertOrderItem(
						order,
						product,
						count,
						countSingle,
						itemIndex,
						1,
						null,
						null,
						cart.Shippings[0].IsDutyFree,
						sqlAccessor);
					if (ReserveSerialKey(order, product, product.Count, itemIndex, sqlAccessor) == false)
					{
						return new InsertOrderItemResult(false, product);
					}

					itemIndex++;
				}

			}
			// ギフト注文？
			else
			{
				var productsAllocatedToSetAndShipping = new List<Hashtable>();

				// セットプロモーションあり？
				if (cart.SetPromotions.Items.Count != 0)
				{
					var products = new List<Hashtable>();

					// 配送先商品をばらす
					int shippingIndex = 1;
					foreach (var shipping in cart.Shippings)
					{
						foreach (var productCount in shipping.ProductCounts)
						{
							for (var i = 0; i < productCount.Count; i++)
							{
								var ht = new Hashtable
								{
									{ "product", productCount.Product },
									{ Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, shippingIndex },
									{ "isDutyFree", shipping.IsDutyFree },
								};
								products.Add(ht);
							}
						}

						shippingIndex++;
					}

					foreach (var cartProduct in cart.Items)
					{
						// 対象商品を抽出
						var targetProducts = products.FindAll(cp => (CartProduct)cp["product"] == cartProduct);

						// セットプロモーション情報を追加
						var i = 0;
						for (var j = 0; j < cartProduct.QuantitiyUnallocatedToSet; j++)
						{
							targetProducts[i].Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, "");
							i++;
						}
						foreach (var setpromotionitem in cartProduct.QuantityAllocatedToSet)
						{
							for (var j = 0; j < setpromotionitem.Value; j++)
							{
								targetProducts[i].Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, setpromotionitem.Key.ToString());
								i++;
							}
						}

						// 配送先、セットプロモーションでグループ化
						var groupedTargetProduct = targetProducts.GroupBy(p => p[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO] + "," + p[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]);

						foreach (var product in groupedTargetProduct)
						{
							var ht = new Hashtable
							{
								{ "product", cartProduct },
								{ Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, product.Key.Split(',')[0] },
								{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, product.Key.Split(',')[1] },
								{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, product.ToList().Count },
								{ "isDutyFree", product.First()["isDutyFree"] },
							};
							productsAllocatedToSetAndShipping.Add(ht);
						}
					}
				}
				else
				{
					var shippingIndex = 1;
					foreach (var shipping in cart.Shippings)
					{
						foreach (var productCount in shipping.ProductCounts)
						{
							var ht = new Hashtable
							{
								{ "product", productCount.Product },
								{ Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, shippingIndex },
								{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, "" },
								{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, productCount.Count },
								{ "isDutyFree", shipping.IsDutyFree },
							};
							productsAllocatedToSetAndShipping.Add(ht);
						}

						shippingIndex++;
					}
				}

				var itemIndex = 1;
				// 通常商品登録(同梱商品を除く)
				foreach (var ht in productsAllocatedToSetAndShipping
					.Where(ht => (((string)ht[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == "")
						&& (((CartProduct)ht["product"]).IsBundle == false))))
				{
					InsertOrderItem(
						order,
						(CartProduct)ht["product"],
						(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
						(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
						itemIndex,
						int.Parse(ht[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO].ToString()),
						null,
						null,
						(bool)ht["isDutyFree"],
						sqlAccessor);
					if (ReserveSerialKey(
						order,
						(CartProduct)ht["product"],
						(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
						itemIndex,
						sqlAccessor) == false)
					{
						return new InsertOrderItemResult(false, (CartProduct)ht["product"]);
					}
					itemIndex++;
				}
				// セットプロモーション商品登録
				foreach (CartSetPromotion setpromotion in cart.SetPromotions)
				{
					var setPromotionItemIndex = 1;
					foreach (var ht in productsAllocatedToSetAndShipping.Where(ht => (string)ht[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == setpromotion.CartSetPromotionNo.ToString()))
					{
						InsertOrderItem(
							order,
							(CartProduct)ht["product"],
							(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
							(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
							itemIndex,
							int.Parse(ht[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO].ToString()),
							setpromotion.CartSetPromotionNo,
							setPromotionItemIndex,
							(bool)ht["isDutyFree"],
							sqlAccessor);
						if (ReserveSerialKey(
							order,
							(CartProduct)ht["product"],
							(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
							itemIndex,
							sqlAccessor) == false)
						{
							return new InsertOrderItemResult(false, (CartProduct)ht["product"]);
						}

						itemIndex++;
						setPromotionItemIndex++;
					}
				}
				// 同梱商品登録
				foreach (var ht in productsAllocatedToSetAndShipping
					.Where(ht => (((string)ht[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == "")
						&& ((CartProduct)ht["product"]).IsBundle)))
				{
					InsertOrderItem(
						order,
						(CartProduct)ht["product"],
						(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
						(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
						itemIndex,
						int.Parse(ht[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO].ToString()),
						null,
						null,
						(bool)ht["isDutyFree"],
						sqlAccessor);
					if (ReserveSerialKey(
						order,
						(CartProduct)ht["product"],
						(int)ht[Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
						itemIndex,
						sqlAccessor) == false)
					{
						return new InsertOrderItemResult(false, (CartProduct)ht["product"]);
					}
					itemIndex++;
				}
			}

			return new InsertOrderItemResult(true);
		}
		/// <summary>
		/// 注文商品情報のINSERT処理とシリアルキーのRESERVE処理。
		/// シリアルキーが不足して引き当てに失敗したときには false を返します。
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="product">カート商品</param>
		/// <param name="productCount">商品数量（ギフト／セット考慮）</param>
		/// <param name="productCountSingle">商品数量（ギフト／セット考慮）</param>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <param name="orderShippingNo">注文配送先枝番</param>
		/// <param name="isDutyFree">免税か</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private void InsertOrderItem(
			Hashtable order,
			CartProduct product,
			int productCount,
			int productCountSingle,
			int orderItemNo,
			int orderShippingNo,
			int? orderSetPromotionNo,
			int? orderSetPromotionItemNo,
			bool isDutyFree,
			SqlAccessor sqlAccessor)
		{
			using (SqlStatement sqlStatement = new SqlStatement("Order", "OrderItemRegist"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_ORDERITEM_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]);
				input.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO, orderItemNo);				// 注文商品枝番
				input.Add(Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, orderShippingNo);		// 配送先枝番
				input.Add(Constants.FIELD_ORDERITEM_SHOP_ID, product.ShopId);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, product.ProductId);
				input.Add(Constants.FIELD_ORDERITEM_VARIATION_ID, product.VariationId);
				input.Add(Constants.FIELD_ORDERITEM_SUPPLIER_ID, product.SupplierId);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_NAME, product.ProductJointName);	// 注文商品名（バリエーション名付加）
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_NAME_KANA, product.ProductNameKana);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE, product.Price);				// 商品単価
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG, product.PriceOrg);		// 商品単価（値引き前）
				if (product.IsSetItem)
				{
					input.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_ID, product.ProductSet.ProductSetId);		// 商品セットID
					input.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_NO, product.ProductSet.ProductSetNo);		// 商品セットNo
					input.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT, product.ProductSet.ProductSetCount);	// 商品セット数量
				}
				else
				{
					input.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_ID, "");
					input.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_NO, null);
					input.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT, null);
				}
				input.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY, productCount);				// 注文数（商品数ｘセット数)
				input.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE, productCountSingle);	// 注文数（商品数）
				input.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE, product.PriceIncludedOptionPrice * productCount);					// 明細金額（小計ｘセット数）
				input.Add(
					Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX,
					isDutyFree ? 0 : (product.PriceTax + product.OptionPriceTax) * productCount);					// 税額
				input.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE, product.PriceIncludedOptionPrice * productCountSingle);	// 明細金額（小計）※ギフト考慮して計算している
				input.Add(Constants.FIELD_ORDERITEM_DATE_CREATED, order[Constants.FIELD_ORDER_ORDER_DATE]);
				input.Add(Constants.FIELD_ORDERITEM_DATE_CHANGED, order[Constants.FIELD_ORDER_ORDER_DATE]);
				if (product.ProductOptionSettingList != null)
				{
					input.Add(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS, ProductOptionSettingHelper.GetSelectedOptionSettingForOrderItem(product.ProductOptionSettingList)); // 商品付帯情報
				}
				else
				{
					input.Add(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS, "");
				}
				input.Add(Constants.FIELD_ORDERITEM_BRAND_ID, product.BrandId);
				input.Add(Constants.FIELD_ORDERITEM_DOWNLOAD_URL, product.DownloadUrl);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCTSALE_ID, product.ProductSaleId);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG, product.TaxIncludedFlg);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, product.TaxRate);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE, product.TaxRoundType);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX, product.PricePretax);

				int index = 1;
				product.CooperationId.ForEach(cooperationId => input.Add(Constants.HASH_KEY_COOPERATION_ID + index++, cooperationId));

				input.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, orderSetPromotionNo);
				input.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO, orderSetPromotionItemNo);

				input.Add(Constants.FIELD_ORDERITEM_NOVELTY_ID, product.NoveltyId);
				input.Add(Constants.FIELD_ORDERITEM_RECOMMEND_ID, product.RecommendId);
				input.Add(Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG, ((product.AddCartKbn == Constants.AddCartKbn.FixedPurchase) || product.AddCartKbn == Constants.AddCartKbn.SubscriptionBox)
					? Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON : Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID, product.ProductBundleId);	// 商品同梱ID
				input.Add(Constants.FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE, product.BundleItemDisplayType);	// 商品同梱明細表示フラグ

				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_POINT_KBN, product.PointKbn1);
				input.Add(Constants.FIELD_ORDERITEM_PRODUCT_POINT, product.Point1);
				input.Add(Constants.FIELD_ORDERITEM_LIMITED_PAYMENT_IDS, string.Join(",", product.LimitedPaymentIds));
				input.Add(Constants.FIELD_ORDERITEM_PLURAL_SHIPPING_PRICE_FREE_FLG, product.IsPluralShippingPriceFree);
				input.Add(Constants.FIELD_ORDERITEM_SHIPPING_SIZE_KBN, product.ShippingSizeKbn);
				input.Add(Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE, product.FixedPurchaseDiscountType);
				input.Add(Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE, product.FixedPurchaseDiscountValue);
				input.Add(Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE, (orderSetPromotionNo == null)
					? product.DiscountedPriceUnAllocatedToSet
						: (product.DiscountedPrice.Count > 0)
							? product.DiscountedPrice[int.Parse(StringUtility.ToEmpty(orderSetPromotionNo))]
							: 0);
				input.Add(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID, product.SubscriptionBoxCourseId);
				input.Add(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT, product.SubscriptionBoxFixedAmount);

				sqlStatement.ExecStatement(sqlAccessor, input);
			}
		}

		/// <summary>
		/// 注文セットプロモーション情報登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public static void InsertOrderSetPromotion(Hashtable order, CartObject cart, SqlAccessor sqlAccessor)
		{
			foreach (CartSetPromotion cartSetPromotion in cart.SetPromotions)
			{
				InsertOrderSetPromotion(order, cartSetPromotion, sqlAccessor);
			}
		}
		/// <summary>
		/// 注文セットプロモーション情報登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cartSetPromotion">カートセットプロモーション情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public static void InsertOrderSetPromotion(Hashtable order, CartSetPromotion cartSetPromotion, SqlAccessor sqlAccessor)
		{
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO, cartSetPromotion.CartSetPromotionNo);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID, cartSetPromotion.SetpromotionId);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME, cartSetPromotion.SetpromotionName);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME,
				(((string)order[Constants.FIELD_ORDER_ORDER_KBN] == Constants.FLG_ORDER_ORDER_KBN_MOBILE) && (cartSetPromotion.SetpromotionDispNameMobile != "")) ? cartSetPromotion.SetpromotionDispNameMobile : cartSetPromotion.SetpromotionDispName);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL, cartSetPromotion.UndiscountedProductSubtotal);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG, cartSetPromotion.IsDiscountTypeProductDiscount ? Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON : Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT, cartSetPromotion.ProductDiscountAmount);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG, cartSetPromotion.IsDiscountTypeShippingChargeFree ? Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON : Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT, cartSetPromotion.ShippingChargeDiscountAmount);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG, cartSetPromotion.IsDiscountTypePaymentChargeFree ? Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON : Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF);
			sqlParams.Add(Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT, cartSetPromotion.PaymentChargeDiscountAmount);

			using (SqlStatement sqlStatement = new SqlStatement("Order", "InsertOrderSetPromotion"))
			{
				sqlStatement.ExecStatement(sqlAccessor, sqlParams);
			}
		}

		/// <summary>
		/// 税率毎注文情報情報登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public static void InsertOrderPriceByTaxRate(Hashtable order, CartObject cart, SqlAccessor sqlAccessor)
		{
			var orderPriceByTaxRateService = new OrderPriceByTaxRateService();
			foreach (var cartPriceInfoByTaxRate in cart.PriceInfoByTaxRate)
			{
				var model = cartPriceInfoByTaxRate.CreateModel();
				model.OrderId = (string)order[Constants.FIELD_ORDER_ORDER_ID];
				model.DateCreated = (DateTime)order[Constants.FIELD_ORDER_ORDER_DATE];
				model.DateChanged = (DateTime)order[Constants.FIELD_ORDER_ORDER_DATE];

				orderPriceByTaxRateService.Insert(model, sqlAccessor);
			}
		}

		/// <summary>
		/// シリアルキーRESERVE処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="product">カート商品</param>
		/// <param name="productCount">商品数量（ギフト考慮）</param>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>成功（シリアルキーが足らない場合はfalse）</returns>
		private bool ReserveSerialKey(
			Hashtable order,
			CartProduct product,
			int productCount,
			int orderItemNo,
			SqlAccessor sqlAccessor)
		{
			if (product.IsDigitalContents == false) return true;

			var orderId = (string)order[Constants.FIELD_ORDER_ORDER_ID];
			var userId = (string)order[Constants.FIELD_ORDER_USER_ID];
			var lastChanged = (string)order[Constants.FIELD_ORDER_LAST_CHANGED];

			var serialKeyService = new SerialKeyService();
			var updatedCount = serialKeyService.ReserveSerialKey(
				orderId,
				orderItemNo,
				userId,
				productCount,
				product.ProductId,
				product.VariationId,
				lastChanged,
				sqlAccessor);

			// シリアルキー不足時は false を返してロールバック
			if (updatedCount != productCount)
			{
				AppLogger.WriteError("シリアルキー不足です。（"
					+ "注文ID:" + (string)order[Constants.FIELD_ORDER_ORDER_ID] + " "
					+ "商品ID:" + product.ProductId + " "
					+ "バリエーションID:" + product.VariationId + " "
					+ "個数:" + productCount + "）");
				return false;
			}

			// 引当されたシリアルキーの取得＆格納（メールテンプレート用）
			var serialKeys = serialKeyService.GetSerialKeyFromOrder(orderId, orderItemNo, sqlAccessor);
			product.SerialKeys.Clear(); // 決済失敗等で仮注文戻し時、1注文で複数回実行されるのでクリアが必要
			product.SerialKeys.AddRange(serialKeys.Select(items => items.SerialKey));

			return true;
		}

		/// <summary>
		/// ポイント利用処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isUser">ユーザーか</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		private bool UsePoints(
			Hashtable order,
			CartObject cart,
			bool isUser,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			if ((Constants.W2MP_POINT_OPTION_ENABLED == false) || (isUser == false)) return true;

			var nextShippingUsePoint = order.ContainsKey(Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT)
				? (decimal)order[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT]
				: 0;
			var fixedPurchaseId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
			// 注文が定期受注から生成された、かつ、利用ポイントが設定された
			if ((string.IsNullOrEmpty(fixedPurchaseId) == false) && (nextShippingUsePoint > 0))
			{
				// 次回購入の利用ポイントを生成した注文に適用
				var result = new PointService().ApplyNextShippingUsePointToOrder(
					Constants.CONST_DEFAULT_DEPT_ID,
					(string)order[Constants.FIELD_ORDER_USER_ID],
					fixedPurchaseId,
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					cart.UsePoint,
					nextShippingUsePoint,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor);
				if (result == false) return false;

				// 定期購入情報の利用ポイント数を0にリセット
				result = new FixedPurchaseService().UpdateNextShippingUsePointToFixedPurchase(
					(string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID],
					FixedPurchaseModel.DEFAULT_NEXT_SHIPPING_USE_POINT,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor);
				if (result == false) return false;
			}
			else if (cart.UsePoint > 0)
			{
				// 次回購入の利用ポイントではない場合、従来通りに処理
				var result = (PointOptionUtility.UpdateOrderPointUse(
					cart.OrderUserId,
					cart.OrderId,
					cart.UsePoint,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					cart.CartId,
					sqlAccessor) >= 0);
				if (result == false) return false;
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], lastChanged, sqlAccessor);
			}
			return true;
		}

		/// <summary>
		/// ポイント発行処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isUser">ユーザーか</param>
		/// <param name="isFirstOrder">初回購入か</param>
		/// <param name="isFirstCart">先頭カートか</param>
		/// <<param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		private void PublishPoint(
			Hashtable order,
			CartObject cart,
			bool isUser,
			bool isFirstOrder,
			bool isFirstCart,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			string transactionName = null;
			bool success = true;

			if (Constants.W2MP_POINT_OPTION_ENABLED == false) return;
			if (isUser == false) return;

			if (success)
			{
				transactionName = "1-3-2-1.通常購入ポイントINSERT処理";
				success = (OrderCommon.AddUserPoint(
					order,
					cart,
					Constants.FLG_POINTRULE_POINT_INC_KBN_BUY,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor) >= 0);
			}
			if (success == false) throw new Exception(transactionName + "に失敗しました。");

			if (success)
			{
				// 初回購入ポイントが％で発行されるとき、複数カートでも全てのカートに初回購入ポイントがつく
				bool publishFirstBuyPoint = isFirstOrder;
				if ((cart.FirstBuyPoint != 0)
					&& (cart.FirstBuyPointKbn == Constants.FLG_POINTRULE_INC_TYPE_RATE)
					&& isUser)
				{
					publishFirstBuyPoint = true;
				}

				// 初回購入ポイントあり？（既にある仮注文は初回購入とみなさない）
				if (publishFirstBuyPoint)
				{
					transactionName = "1-3-2-2.初回購入ポイントINSERT処理";
					success = (OrderCommon.AddUserPoint(
						order,
						cart,
						Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY,
						UpdateHistoryAction.DoNotInsert,
						sqlAccessor) >= 0);
				}
			}
			if (success == false) throw new Exception(transactionName + "に失敗しました。");

			if (Constants.INTRODUCTION_COUPON_OPTION_ENABLED
				&& success
				&& (cart.Coupon != null)
				&& (cart.Coupon.CouponType == Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON))
			{
				// Exec give point to user referral
				var userId = DomainFacade.Instance.UserService.GetReferredUserId(cart.CartUserId);

				if (string.IsNullOrEmpty(userId) == false)
				{
					success = ExecGivePointToUserReferral(
						userId,
						order,
						cart,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert,
						sqlAccessor);

					if (success == false) throw new Exception(transactionName + "に失敗しました。");
				}
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(cart.OrderUserId, lastChanged, sqlAccessor);
			}
		}

		/// <summary>
		/// クーポン利用処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="transactionName">トランザクション名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		private bool UseCoupons(
			Hashtable order,
			CartObject cart,
			string lastChanged,
			ref string transactionName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			if (Constants.W2MP_COUPON_OPTION_ENABLED == false) return true;
			if (cart.Coupon == null) return true;

			bool success = true;
			var couponService = new CouponService();

			if (success)
			{
				transactionName = "1-4-1.注文クーポンINSERT処理";
				success = InsertOrderCoupon(order, cart, UpdateHistoryAction.DoNotInsert, sqlAccessor);
			}
			if (success)
			{
				if (cart.Coupon.IsCouponLimit())
				{
					transactionName = "1-4-2-A.利用可能回数制限有りユーザクーポンUPDATE処理（利用済みへ）";
					success = couponService.UpdateUserCouponUseFlg(
						cart.OrderUserId,
						cart.Coupon.DeptId,
						cart.Coupon.CouponId,
						cart.Coupon.CouponNo,
						true,
						(DateTime)order[Constants.FIELD_ORDER_ORDER_DATE],
						(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
						UpdateHistoryAction.DoNotInsert,
						sqlAccessor);
				}
			}
			if (success)
			{
				if (cart.Coupon.IsCouponAllLimit())
				{
					transactionName = "1-4-2-B.回数制限ありクーポンの利用可能回数を減らす";

					// 注文同梱で、注文同梱の親注文でクーポンを利用していた場合は現在のクーポン利用回数を考慮せずに利用回数を減らす
					// 親注文と注文同梱注文で二重にクーポンがマイナスされるが、注文同梱後のキャンセル処理にて注文同梱前のクーポン利用回数が戻される
					if ((string.IsNullOrEmpty(cart.OrderCombineParentOrderId) == false) && (cart.IsCombineParentOrderUseCoupon))
					{
						success = couponService.UpdateCouponCountDownIgnoreCouponCount(
							cart.Coupon.DeptId,
							cart.Coupon.CouponId,
							cart.Coupon.CouponCode,
							lastChanged,
							sqlAccessor);
					}
					else
					{
						success = couponService.UpdateCouponCountDown(
							cart.Coupon.DeptId,
							cart.Coupon.CouponId,
							cart.Coupon.CouponCode,
							lastChanged,
							sqlAccessor);
					}
				}
			}
			if (success)
			{
				if (cart.Coupon.IsCouponLimitedForRegisteredUser())
				{
					transactionName = "1-4-2-D.会員限定回数制限ありクーポンのユーザーに紐づく利用可能回数を減らす";
					success = CouponOptionUtility.UpdateUserCouponCount(
						cart.Coupon.DeptId,
						cart.OrderUserId,
						cart.Coupon.CouponId,
						cart.Coupon.CouponNo,
						sqlAccessor,
						false);
				}
			}
			if (success)
			{
				if (cart.Coupon.IsBlacklistCoupon())
				{
					// 定期台帳の利用クーポンを注文に適用されるかの判定
					var isApplyFromFixedPurchase =
						((StringUtility.ToEmpty(order[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_ID]) != "")
							&& (cart.FixedPurchase != null));
					transactionName = "1-4-2-C.ブラックリスト型クーポン利用済みユーザーINSERT処理(利用済みへ)";
					var couponUseUser = new CouponUseUserModel
					{
						CouponId = cart.Coupon.CouponId,
						OrderId = cart.OrderId,
						CouponUseUser =
							(Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
								== Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
							? cart.Owner.MailAddr
							: cart.OrderUserId,
						FixedPurchaseId = isApplyFromFixedPurchase ? cart.FixedPurchase.FixedPurchaseId : "",
						LastChanged = lastChanged
					};

					// 注文同梱の親・子注文、または注文完了ページでのレコメンドの元注文がブラックリスト型クーポンを使用したかチェック
					var useBlacklistCoupon = false;
					var usedBlacklistCoupon = couponService.GetCouponUseUser(
							couponUseUser.CouponId,
							couponUseUser.CouponUseUser,
							sqlAccessor);
					if (string.IsNullOrEmpty(cart.OrderCombineParentOrderId) == false)
					{
						useBlacklistCoupon = ((usedBlacklistCoupon != null)
							&& (cart.OrderCombineParentOrderId.Split(',').Contains(usedBlacklistCoupon.OrderId)
								|| (usedBlacklistCoupon.OrderId == cart.BeforeRecommendOrderId)));
					}
					else if (cart.Items.Any(item => (string.IsNullOrEmpty(item.RecommendId) == false)))
					{
						var recommend = new RecommendService().Get(
							cart.ShopId,
							cart.Items.First(item => (string.IsNullOrEmpty(item.RecommendId) == false)).RecommendId,
							sqlAccessor);
						useBlacklistCoupon = (recommend.CanDisplayOrderCompletePage
							&& (usedBlacklistCoupon.OrderId == cart.BeforeRecommendOrderId));
					}

					// 注文同梱で親注文が使用したブラックリスト型クーポンの場合は紐づく注文IDを更新する
					success = useBlacklistCoupon
						? (couponService.UpdateCouponUseUserOrderId(couponUseUser, sqlAccessor) > 0)
						: (couponService.InsertCouponUseUser(couponUseUser, sqlAccessor) > 0);
				}
			}

			if (success)
			{
				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(cart.OrderUserId, lastChanged, sqlAccessor);
				}

				transactionName = "1-4-3.ユーザクーポン履歴INSERT処理(クーポン利用)";
				success = couponService.InsertUserCouponHistory(
					cart.OrderUserId,
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					cart.Coupon.DeptId,
					cart.Coupon.CouponId,
					cart.Coupon.CouponCode,
					Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE,
					Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_BASE,
					-1,
					cart.UseCouponPrice,
					lastChanged,
					sqlAccessor);
			}
			return success;
		}

		/// <summary>
		/// クーポン発行
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="isUser">ユーザーか</param>
		/// <param name="isFirstOrder">初回購入か</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="transactionName">トランザクション名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>結果</returns>
		private bool PublishCoupons(
			CartObject cart,
			bool isUser,
			bool isFirstOrder,
			string lastChanged,
			ref string transactionName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			bool success = true;

			if (Constants.W2MP_COUPON_OPTION_ENABLED == false) return true;
			if (isUser == false) return true;

			var couponService = new CouponService();
			transactionName = "1-4-4.購入した会員に発行ユーザクーポンINSERT処理";
			var buyCoupons = couponService.GetPublishCouponsByCouponType(
				Constants.W2MP_DEPT_ID,
				Constants.FLG_COUPONCOUPON_TYPE_BUY,
				sqlAccessor);
			success = PublishCoupons(buyCoupons, cart, lastChanged, UpdateHistoryAction.DoNotInsert, sqlAccessor);
			if (success == false) return false;

			if (isFirstOrder)
			{
				transactionName = "1-4-6.初回購入した会員に発行ユーザクーポンINSERT処理";
				var firstBuyCoupons = couponService.GetPublishCouponsByCouponType(
					Constants.W2MP_DEPT_ID,
					Constants.FLG_COUPONCOUPON_TYPE_FIRSTBUY,
					sqlAccessor);
				success = PublishCoupons(firstBuyCoupons, cart, lastChanged, UpdateHistoryAction.DoNotInsert, sqlAccessor);

				// Introduction coupon
				if (Constants.INTRODUCTION_COUPON_OPTION_ENABLED)
				{
					var userId = DomainFacade.Instance.UserService.GetReferredUserId(cart.CartUserId);

					if (string.IsNullOrEmpty(userId) == false)
					{
						// Publish coupons give to introducer
						var couponGiveToIntroducer = DomainFacade.Instance.CouponService.GetPublishCouponsByCouponType(
							Constants.W2MP_DEPT_ID,
							Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON);

						PublishCoupons(
							couponGiveToIntroducer,
							cart,
							lastChanged,
							UpdateHistoryAction.DoNotInsert,
							sqlAccessor,
							userId);

						// Publish coupons thanks for introduced user
						var couponThanksForIntroducer = DomainFacade.Instance.CouponService.GetPublishCouponsByCouponType(
							Constants.W2MP_DEPT_ID,
							Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON);

						PublishCoupons(
							couponThanksForIntroducer,
							cart,
							lastChanged,
							UpdateHistoryAction.DoNotInsert,
							sqlAccessor);
					}
				}
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(cart.OrderUserId, lastChanged, sqlAccessor);
			}
			return success;
		}
		/// <summary>
		/// クーポン発行
		/// </summary>
		/// <param name="coupons">発行クーポン</param>
		/// <param name="cart">カート</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="userIdOfReferralCode">User id of referral code</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private bool PublishCoupons(
			CouponModel[] coupons,
			CartObject cart,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor,
			string userIdOfReferralCode = "")
		{
			bool success = true;
			var couponService = new CouponService();
			var userId = string.IsNullOrEmpty(userIdOfReferralCode)
				? cart.OrderUserId
				: userIdOfReferralCode;

			foreach (var coupon in coupons)
			{
				success = couponService.InsertUserCouponWithOrderId(
					userId,
					cart.OrderId,
					coupon.DeptId,
					coupon.CouponId,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor);
				if (success == false) break;

				success = couponService.InsertUserCouponHistory(
					userId,
					cart.OrderId,
					coupon.DeptId,
					coupon.CouponId,
					coupon.CouponCode,
					Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH,
					Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_BASE,
					1,
					coupon.DiscountPrice.GetValueOrDefault(),
					lastChanged,
					sqlAccessor);
				if (success == false) break;
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(cart.OrderUserId, lastChanged, sqlAccessor);
			}
			return success;
		}

		/// <summary>
		/// GUESTユーザ情報のINSERT処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="execType">実行タイプ</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private void InsertGuestUser(
			Hashtable order,
			CartObject cart,
			OrderRegisterBase.ExecTypes execType,
			SqlAccessor sqlAccessor)
		{
			var user = CreateUserModel(order, cart);

			// 登録
			new UserService().InsertWithUserExtend(user, user.LastChanged, UpdateHistoryAction.DoNotInsert, sqlAccessor);

			// 【EC】新規注文登録時に会員登録をする場合、ポイント更新処理の前に連携するため処理をスキップ
			if ((Constants.CROSS_POINT_OPTION_ENABLED)
				&& ((execType != OrderRegisterBase.ExecTypes.CommerceManager)
					|| (cart.Owner.OwnerKbn != Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_USER)))
			{
				// ゲストユーザーの際はメールアドレスを空にする
				user.MailAddr = string.Empty;

				// クロスポイント側にユーザー情報を登録
				var apiResult = new CrossPointUserApiService().Insert(user);

				if (apiResult.IsSuccess == false)
				{
					var errorMessage = apiResult.ErrorCodeList.Contains(
						Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
							? apiResult.ErrorMessage
							: MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

					throw new w2Exception(errorMessage);
				}
			}

			// 【EC】新規注文登録で会員登録をする時に発行されるポイント
			if (Constants.W2MP_POINT_OPTION_ENABLED
				&& (execType == OrderRegisterBase.ExecTypes.CommerceManager)
				&& (cart.Owner.OwnerKbn == Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_USER))
			{
				OrderCommon.InsertUserPointAtUserRegistration(user, UpdateHistoryAction.DoNotInsert, sqlAccessor);
			}
		}

		/// <summary>
		/// アドレス帳情報のINSERT処理
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="shipping">配送先情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ユーザー配送先枝番</returns>
		private int InsertUserShipping(string userId, CartShipping shipping, SqlAccessor sqlAccessor)
		{
			var userShipping = shipping.CreateUserShipping(userId);
			var userShippingNo = DomainFacade.Instance.UserShippingService.Insert(
				userShipping,
				"", // 履歴を落とさないので最終更新者はダミー
				UpdateHistoryAction.DoNotInsert,
				sqlAccessor);
			return userShippingNo;
		}

		/// <summary>
		/// 注文クーポン情報のINSERT処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		private bool InsertOrderCoupon(Hashtable order, CartObject cart, UpdateHistoryAction updateHistoryAction, SqlAccessor sqlAccessor)
		{
			decimal? discountPrice = null;
			decimal? discountRate = null;
			switch (cart.Coupon.DiscountKbn)
			{
				case CartCoupon.CouponDiscountKbn.Price:
					discountPrice = cart.Coupon.DiscountPrice;	// クーポン割引額
					break;

				case CartCoupon.CouponDiscountKbn.Rate:
					discountRate = cart.Coupon.DiscountRate;	// クーポン割引率
					break;
			}

			var orderCoupon = new OrderCouponModel()
			{
				OrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),	// 注文ID
				OrderCouponNo = 1,				// 注文クーポン枝番
				DeptId = cart.Coupon.DeptId,	// 識別ID
				CouponId = cart.Coupon.CouponId,	// クーポンID
				CouponNo = cart.Coupon.CouponNo,	// 識別ID
				CouponCode = cart.Coupon.CouponCode,	// クーポンコード
				CouponName = cart.Coupon.CouponName,	// 管理用クーポン名
				CouponDispName = cart.Coupon.CouponDispName,	// 表示用クーポン名
				CouponType = cart.Coupon.CouponType,	// クーポン種別
				CouponDiscountPrice = discountPrice,	// クーポン商品割引額
				CouponDiscountRate = discountRate,		// クーポン商品割引率
				DateCreated = DateTime.Parse(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_DATE])),	// 作成日
				DateChanged = DateTime.Parse(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_DATE])),	// 更新日
				LastChanged = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED])					// 最終更新者
			};

			// 注文クーポン情報登録
			var result = new OrderService().InsertCoupon(orderCoupon, updateHistoryAction, sqlAccessor);
			return (result > 0);
		}

		/// <summary>
		/// 定期会員フラグ更新処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private void UpdateFixedPurchaseMemberFlg(
			Hashtable order,
			CartObject cart,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			var user = new UserService().Get((string)order[Constants.FIELD_ORDER_USER_ID], sqlAccessor);
			if (user == null) return;

			var fixedPurchaseMemberFlg = ((user.IsFixedPurchaseMember || cart.HasFixedPurchase || cart.HasSubscriptionBox)
				? Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON
				: Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF);

			new UserService().UpdateFixedPurchaseMemberFlg(
				(string)order[Constants.FIELD_ORDER_USER_ID],
				fixedPurchaseMemberFlg,
				lastChanged,
				updateHistoryAction,
				sqlAccessor);
		}

		/// <summary>
		/// 定期継続分析登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void InsertFixedPurchaseRepeatAnalysis(Hashtable order, CartObject cart, string lastChanged, SqlAccessor accessor)
		{
			var fixedPurchaseId = (cart.FixedPurchase != null)
				? cart.FixedPurchase.FixedPurchaseId
				: StringUtility.ToEmpty(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
			var registerItems = cart.GetItemsRegisteredFixedPurchase();
			foreach (var item in registerItems)
			{
				var service = new FixedPurchaseRepeatAnalysisService();
				var maxModel = service.GetRepeatAnalysisMaxCountByUserProduct(cart.OrderUserId, item.ProductId, item.VariationId, accessor);
				var model = new FixedPurchaseRepeatAnalysisModel
				{
					OrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
					UserId = cart.OrderUserId,
					ProductId = item.ProductId,
					VariationId = item.VariationId,
					Count = maxModel.Count + 1,
					FixedPurchaseId = fixedPurchaseId,
					Status = Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_ORDER,
					LastChanged = lastChanged,
				};

				service.Insert(model, accessor);
			}
		}

		/// <summary>
		/// 定期購入注文仮登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>アラートメッセージ</returns>
		public static bool RegistPreFixedPurchaseOrder(
			Hashtable order,
			CartObject cart,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var fixedPurchaseId = new FixedPurchaseRegister(order, cart, lastChanged)
				.RegisterAndUpdateFixedPurchaseInfoForOrder(
					Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_TEMP,
					updateHistoryAction,
					accessor);

			if (string.IsNullOrEmpty(fixedPurchaseId)) return false;

			order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID] = fixedPurchaseId;
			order[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID] = cart.SubscriptionBoxCourseId;
			order[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT] = cart.SubscriptionBoxFixedAmount;
			return true;
		}

		/// <summary>
		/// Insert User Invoice
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="shipping">Shipping</param>
		/// <param name="accessor">Accessor</param>
		private void InsertUserInvoice(
			string userId,
			string lastChanged,
			CartShipping shipping,
			SqlAccessor accessor)
		{
			var userInvoice = new TwUserInvoiceModel
			{
				UserId = userId,
				TwInvoiceName = StringUtility.ToEmpty(shipping.InvoiceName),
				TwUniformInvoice = StringUtility.ToEmpty(shipping.UniformInvoiceType),
				TwUniformInvoiceOption1 = StringUtility.ToEmpty(shipping.UniformInvoiceOption1),
				TwUniformInvoiceOption2 = StringUtility.ToEmpty(shipping.UniformInvoiceOption2),
				TwCarryType = StringUtility.ToEmpty(shipping.CarryType),
				TwCarryTypeOption = StringUtility.ToEmpty(shipping.CarryTypeOptionValue)
			};

			var userInvoiceNo = new TwUserInvoiceService().Insert(
				userInvoice,
				lastChanged,
				UpdateHistoryAction.Insert,
				accessor);
		}

		/// <summary>
		/// Exec give point to user referral
		/// </summary>
		/// <param name="userId">User id</param>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>True if any user is give point: False</returns>
		private bool ExecGivePointToUserReferral(
			string userId,
			Hashtable order,
			CartObject cart,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var pointRules = PointOptionUtility.GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_BUY);
			var updated = 0;

			foreach (var pointRule in pointRules)
			{
				var pointRuleIncType = string.Empty;
				var orderPointAdd = PointOptionUtility.GetOrderPointAdd(
					cart,
					Constants.FLG_POINTRULE_POINT_INC_KBN_BUY,
					out pointRuleIncType,
					cart.FixedPurchase,
					accessor: accessor);

				updated += PointOptionUtility.InsertUserPoint(
					userId,
					cart.OrderId,
					pointRule.PointRuleId,
					orderPointAdd,
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
					updateHistoryAction,
					accessor);
			}

			return (updated >= 0);
		}

		/// <summary>
		/// 注文情報とカート情報を利用してユーザー情報を作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <returns>ユーザー情報</returns>
		private UserModel CreateUserModel(Hashtable order, CartObject cart)
		{
			// 入力情報取得
			var user = new UserModel
			{
				UserId = cart.OrderUserId,
				UserKbn = cart.Owner.OwnerKbn,
				Name = cart.Owner.Name,
				Name1 = cart.Owner.Name1,
				Name2 = cart.Owner.Name2,
				NameKana = cart.Owner.NameKana,
				NameKana1 = cart.Owner.NameKana1,
				NameKana2 = cart.Owner.NameKana2,
				MailAddr = cart.Owner.MailAddr,
				MailAddr2 = cart.Owner.MailAddr2,
				Zip = cart.Owner.Zip,
				Zip1 = cart.Owner.Zip1,
				Zip2 = cart.Owner.Zip2,
				Addr1 = cart.Owner.Addr1,
				Addr2 = cart.Owner.Addr2,
				Addr3 = cart.Owner.Addr3,
				Addr4 = cart.Owner.Addr4,
				Addr5 = cart.Owner.Addr5,
				Addr = cart.Owner.ConcatenateAddress(),
				AddrCountryIsoCode = cart.Owner.AddrCountryIsoCode,
				AddrCountryName = cart.Owner.AddrCountryName,
				CompanyName = cart.Owner.CompanyName,
				CompanyPostName = cart.Owner.CompanyPostName,
				Tel1 = cart.Owner.Tel1,
				Tel1_1 = cart.Owner.Tel1_1,
				Tel1_2 = cart.Owner.Tel1_2,
				Tel1_3 = cart.Owner.Tel1_3,
				Tel2 = cart.Owner.Tel2,
				Tel2_1 = cart.Owner.Tel2_1,
				Tel2_2 = cart.Owner.Tel2_2,
				Tel2_3 = cart.Owner.Tel2_3,
				Sex = cart.Owner.Sex,
				Birth = cart.Owner.Birth,
				BirthYear = cart.Owner.BirthYear,
				BirthMonth = cart.Owner.BirthMonth,
				BirthDay = cart.Owner.BirthDay,
				MailFlg = (cart.Owner.MailFlg ? Constants.FLG_USER_MAILFLG_OK : Constants.FLG_USER_MAILFLG_NG),
				CareerId = (string)order[Constants.FIELD_ORDER_CAREER_ID],
				RemoteAddr = (string)order[Constants.FIELD_ORDER_REMOTE_ADDR],
				AdvcodeFirst = (string)order[Constants.FIELD_ORDER_ADVCODE_NEW],
				LastChanged = (string)order[Constants.FIELD_ORDER_LAST_CHANGED],
				AccessCountryIsoCode = cart.Owner.AccessCountryIsoCode,
				DispLanguageCode = cart.Owner.DispLanguageCode,
				DispLanguageLocaleId = cart.Owner.DispLanguageLocaleId,
				DispCurrencyCode = cart.Owner.DispCurrencyCode,
				DispCurrencyLocaleId = cart.Owner.DispCurrencyLocaleId,
			};

			return user;
		}

		/// <summary>
		/// 注文商品登録結果クラス
		/// </summary>
		public class InsertOrderItemResult
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="result">結果（成功：true、失敗：false）</param>
			/// <param name="errorCartProduct">エラーカート商品情報<</param>
			public InsertOrderItemResult(bool result, CartProduct errorCartProduct = null)
			{
				this.Result = result;
				this.ErrorCartProduct = errorCartProduct;
			}
			#endregion

			#region プロパティ
			/// <summary>結果（成功：true、失敗：false）</summary>
			public bool Result { get; private set; }
			/// <summary>エラーカート商品情報</summary>
			public CartProduct ErrorCartProduct { get; private set; }
			#endregion
		}
	}
}
