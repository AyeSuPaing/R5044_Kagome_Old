/*
=========================================================================================================
  Module      : 注文決済登録クラス(OrderPaymentRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using Amazon.Pay.API.WebStore.Charge;
using jp.veritrans.tercerog.mdk.dto;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using w2.App.Common.Amazon;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Aftee;
using w2.App.Common.Order.Payment.Atobaraicom;
using w2.App.Common.Order.Payment.Atone;
using w2.App.Common.Order.Payment.DSKDeferred;
using w2.App.Common.Order.Payment.DSKDeferred.OrderCancel;
using w2.App.Common.Order.Payment.DSKDeferred.OrderRegister;
using w2.App.Common.Order.Payment.EScott;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.OrderModifyCancel;
using w2.App.Common.Order.Payment.GMO.OrderRegister;
using w2.App.Common.Order.Payment.GMO.TransactionRegister;
using w2.App.Common.Order.Payment.GMO.Zcom;
using w2.App.Common.Order.Payment.GMO.Zcom.Direct;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Transaction;
using w2.App.Common.Order.Payment.LinePay;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Rakuten.Authorize;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.Cancel;
using w2.App.Common.Order.Payment.Score.Order;
using w2.App.Common.Order.Payment.TriLinkAfterPay;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Request;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.App.Common.Order.Payment.Zeus;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.App.Common.Order.Payment.Paygent;
using w2.Common.Sql;

namespace w2.App.Common.Order.Register
{
	/// <summary>
	/// 注文決済登録クラス
	/// </summary>
	internal class OrderPaymentRegister
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="properties">注文登録プロパティ</param>
		internal OrderPaymentRegister(OrderRegisterProperties properties)
		{
			this.Properties = properties;
			this.IsAuthResultHold = false;
		}

		#region 決済処理
		/// <summary>
		/// 決済実行
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="needsRollback">ロールバック必要か（決済中の例外はロールバックしない）</param>
		/// <param name="isExternalPayment">外部連携あり？</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		internal bool ExecOrderPayment(
			Hashtable order,
			CartObject cart,
			out bool needsRollback,
			out bool isExternalPayment,
			UpdateHistoryAction updateHistoryAction)
		{
			bool result = true;
			needsRollback = false;
			isExternalPayment = false;

			//------------------------------------------------------
			// ２．外部連携決済処理
			//		・決済失敗時は、仮注文情報を戻す
			//		・例外発生時は、仮注文を「のこす」
			//		・仮注文戻し処理で失敗した場合は、仮注文を「のこす」
			//------------------------------------------------------
			bool paymentStatusCompleteFlg = (cart.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
				? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
				: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
			string successPaymentStatus = paymentStatusCompleteFlg
				? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
				: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;

			try
			{
				var addExternalPaymentMemo = false;
				switch (cart.Payment.PaymentId)
				{
					// カード決済処理
					case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
						result = ExecPaymentCard(order, cart, successPaymentStatus, updateHistoryAction);
						isExternalPayment = true;
						addExternalPaymentMemo = result && (OrderCommon.IsCreditPaymentAffterAuth(cart.HasDigitalContents) == false);
						break;

					// コンビニ決済処理(前払い)
					case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE:
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.CommerceManager) return true; // 管理画面注文登録は未対応
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch) return true; // 定期購入は未対応

						result = ExecPaymentCvs(order, cart);

						if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten)
						{
							isExternalPayment = true;
							addExternalPaymentMemo = result;
						}
						break;

					// コンビニ決済処理(後払い)
					case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
						result = ExecPaymentCvsDef(order, cart);
						isExternalPayment = true;
						addExternalPaymentMemo = result;
						break;

					// 後付款(TriLink後払い)
					case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY:
						result = ((this.OrderExecType == OrderRegisterBase.ExecTypes.CommerceManager)
							|| (this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch))
								? ExecTriLinkAfterPayForCommerce(order, cart)
								: ExecTriLinkAfterPayForFront(order, cart);
						isExternalPayment = true;
						if (result)
						{
							order[Constants.FIELD_ORDER_PAYMENT_MEMO] = OrderCommon.CreateOrderPaymentMemoForAuth(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
								cart.Payment.PaymentId,
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								cart.SendingAmount);
						}
						break;

					// 銀行振込決済
					case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE:
					case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF:
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.CommerceManager) return true; // 管理画面注文登録は未対応
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch) return true; // 定期購入は未対応
						break;

					// Amazonペイメント
					case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
						result = ExecAmazonPayment(order, cart);
						isExternalPayment = true;
						if (result)
						{
							order[Constants.FIELD_ORDER_PAYMENT_MEMO] = OrderCommon.CreateOrderPaymentMemoForAuth(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
								cart.Payment.PaymentId,
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								cart.PriceTotal);
						}
						break;

					// Amazonペイメント(CV2)定期子注文作成、通常注文および初回注文はOrderAmazonCompleteで処理
					case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
						result = ExecAmazonPaymentCv2(order, cart);
						isExternalPayment = true;
						if (result)
						{
							order[Constants.FIELD_ORDER_PAYMENT_MEMO] = OrderCommon.CreateOrderPaymentMemoForAuth(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
								cart.Payment.PaymentId,
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								cart.PriceTotal);
						}
						break;

					// Paypal決済
					case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
						result = ExecPaypalPayment(order, cart);
						isExternalPayment = true;
						if (result)
						{
							order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
						}
						addExternalPaymentMemo = result;
						break;

					// 「決済なし」のときは入金ステータス格納
					case Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT:
						if (order.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
						{
							order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE;
						}
						else
						{
							order.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE);
						}
						break;

					// Paidy翌月払い
					case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
						result = ExecPaidyPayment(order, cart);
						addExternalPaymentMemo = result;
						isExternalPayment = true;
						if (result)
						{
							order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
						}
						break;

					// Atone Payment
					case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
						if (cart.IsNotUpdateInformationForPaymentAtoneOrAftee)
						{
							order[Constants.FIELD_ORDER_CARD_TRAN_ID] = cart.Payment.CardTranId;
							addExternalPaymentMemo = true;
							isExternalPayment = true;
							if (cart.IsDigitalContentsOnly) AtonePaymentApiFacade.CapturePayment(cart.TokenAtone, cart.Payment.CardTranId);
							break;
						}

						// Case Order Now
						if (string.IsNullOrEmpty(cart.TokenAtone))
						{
							var user = new UserService().Get(cart.CartUserId);
							if (user != null)
							{
								cart.TokenAtone =
									user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID];
							}
						}

						var requestAtone =
							JsonConvert.DeserializeObject<AtoneCreatePaymentRequest>(OrderCommon.CreateDataForAuthorizingAtoneAftee(cart, true));
						if ((this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch)
							&& (string.IsNullOrEmpty(cart.Payment.CardTranId) == false))
						{
							requestAtone.Data.RelatedTransaction = cart.Payment.CardTranId;
						}
						else if (cart.HasFixedPurchase
							&& string.IsNullOrEmpty(cart.Payment.CardTranId))
						{
							var lastOrder =
								new OrderService().GetFirstFixedPurchaseOrder(cart.FixedPurchase.FixedPurchaseId);
							if (lastOrder != null)
							{
								cart.Payment.CardTranId = lastOrder.CardTranId;
							}
							requestAtone.Data.RelatedTransaction = cart.Payment.CardTranId;
						}
						else if ((cart.IsNotUpdateInformationForPaymentAtoneOrAftee == false)
							&& (this.OrderExecType != OrderRegisterBase.ExecTypes.CommerceManager))
						{
							requestAtone.Data.UpdatedTransactions = new[] { cart.Payment.CardTranId };
						}

						requestAtone.Data.AuthenticationToken = cart.TokenAtone;
						var responseAtone = AtonePaymentApiFacade.CreatePayment(requestAtone);
						result = (responseAtone.IsSuccess && responseAtone.IsAuthorizationSuccess);
						// Update card tran id & create payment memo
						if (result && string.IsNullOrEmpty(responseAtone.TranId) == false)
						{
							order[Constants.FIELD_ORDER_CARD_TRAN_ID] = responseAtone.TranId;
							addExternalPaymentMemo = true;
							isExternalPayment = true;
						}
						else
						{
							var errorMessage = string.Join(
								",",
								responseAtone.Errors.Select(item => string.Format("{0}", item.Messages[0])));
							var errorCode = string.Join(
								",",
								responseAtone.Errors.Select(item => string.Format("{0}", item.Code)));
							if (responseAtone.IsAuthorizationSuccess == false)
							{
								errorMessage += "," + responseAtone.AuthorizationResultNgReasonMessage;
								errorCode += ",AuthorizationResult:" + responseAtone.AuthorizationResultNgReason;
							}

							this.ApiErrorMessage = LogCreator.CreateErrorMessage(errorCode, errorMessage);
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
						if (cart.IsNotUpdateInformationForPaymentAtoneOrAftee)
						{
							order[Constants.FIELD_ORDER_CARD_TRAN_ID] = cart.Payment.CardTranId;
							addExternalPaymentMemo = true;
							isExternalPayment = true;
							if (cart.IsDigitalContentsOnly) AfteePaymentApiFacade.CapturePayment(cart.TokenAtone, cart.Payment.CardTranId);
							break;
						}

						// Case Order Now
						if (string.IsNullOrEmpty(cart.TokenAftee))
						{
							var user = new UserService().Get(cart.CartUserId);
							if (user != null)
							{
								cart.TokenAftee =
									user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID];
							}
						}

						var requestAftee =
							JsonConvert.DeserializeObject<AfteeCreatePaymentRequest>(OrderCommon.CreateDataForAuthorizingAtoneAftee(cart, false));
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch)
						{
							var lastOrder =
								new OrderService().GetFirstFixedPurchaseOrder(cart.FixedPurchase.FixedPurchaseId);
							if (lastOrder != null)
							{
								cart.Payment.CardTranId = lastOrder.CardTranId;
							}
							requestAftee.Data.RelatedTransaction = cart.Payment.CardTranId;
							requestAftee.Data.UpdatedTransactions = string.Empty;
						}
						else
						{
							requestAftee.Data.UpdatedTransactions = cart.Payment.CardTranId;
						}

						if ((cart.IsNotUpdateInformationForPaymentAtoneOrAftee == false)
							&& (this.OrderExecType == OrderRegisterBase.ExecTypes.CommerceManager))
						{
							requestAftee.Data.UpdatedTransactions = cart.OrderCombineParentTranId;
						}

						requestAftee.Data.AuthenticationToken = cart.TokenAftee;
						var responseAftee = AfteePaymentApiFacade.CreatePayment(requestAftee);
						isExternalPayment = true;
						result = responseAftee.IsSuccess;
						// Update cart tran id & create payment memo
						if (result && (string.IsNullOrEmpty(responseAftee.TranId) == false))
						{
							order[Constants.FIELD_ORDER_CARD_TRAN_ID] = responseAftee.TranId;
							addExternalPaymentMemo = true;
							isExternalPayment = true;
						}
						else
						{
							var errorMessage = string.Join(
								",",
								responseAftee.Errors.Select(item => string.Format("{0}", item.Messages[0])));
							var errorCode = string.Join(
								",",
								responseAftee.Errors.Select(item => string.Format("{0}", item.Code)));
							this.ApiErrorMessage = LogCreator.CreateErrorMessage(errorCode, errorMessage);
						}
						break;

					// LINE Pay
					case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
						result = ExecLinePayment(order, cart);
						addExternalPaymentMemo = result;
						isExternalPayment = true;
						if (result)
						{
							order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
						}
						break;

					// NP後払い
					case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
						result = ExecNPAfterPay(order, cart);
						if (result)
						{
							order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
						}
						addExternalPaymentMemo = result;
						isExternalPayment = result;
						break;

					// EcPay
					case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
						result = true;
						isExternalPayment = true;
						addExternalPaymentMemo = false;
						break;

					// DSKコンビニ後払い
					case Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF:
						result = ExecPaymentDskDef(order, cart);
						break;

					// NewebPay
					case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
						result = true;
						isExternalPayment = true;
						addExternalPaymentMemo = false;
						break;

					// ソフトバンク・ワイモバイルまとめて支払い(SBPS)
					case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
						result = ExecSoftbankKetaiPayment(order, cart);
						isExternalPayment = true;
						addExternalPaymentMemo = result;
						break;

					// auかんたん決済(SBPS)
					case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
						result = ExecAuKantanPayment(order, cart);
						isExternalPayment = true;
						addExternalPaymentMemo = result;
						break;

					// ドコモケータイ払い(SBPS)
					case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
						result = ExecDocomoKetaiPayment(order, cart);
						isExternalPayment = true;
						addExternalPaymentMemo = result;
						break;

					// Paypay
					case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
						if ((this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch)
							|| (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc))
						{
							switch (Constants.PAYMENT_PAYPAY_KBN)
							{
								case Constants.PaymentPayPayKbn.GMO:
									var response = ExecPaypayPayment(order, cart);
									result = (response.Result == Results.Success);
									if (result
										&& (string.IsNullOrEmpty(response.PaypayTrackingId) == false))
									{
										order[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS] = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
									}
									isExternalPayment = true;
									addExternalPaymentMemo = result;
									break;

								case Constants.PaymentPayPayKbn.VeriTrans:
									result = ExecVeriTransPaypayPayment(order, cart);
									isExternalPayment = true;
									addExternalPaymentMemo = result;
									break;
							}
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU:
						order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
						break;

					// GMO: Pay as you go
					case Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO:
						result = ExecPaymentGmoKb(order, cart);
						isExternalPayment = true;
						addExternalPaymentMemo = result;
						break;

					// GMO: Frame Guarantee
					case Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE:
						result = ExecPaymentGmoKb(order, cart);
						isExternalPayment = true;
						addExternalPaymentMemo = result;
						break;

					// GMOアトカラ
					case Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA:
						isExternalPayment = true;
						break;

					// Banknet
					case Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET:
						isExternalPayment = true;
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.CommerceManager) return true;
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch) return true;

						result = ExecPaymentBanknet(order, cart);
						break;

					// ATM Paygent
					case Constants.FLG_PAYMENT_PAYMENT_ID_ATM:
						isExternalPayment = true;
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.CommerceManager) return true;
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch) return true;

						result = ExecPaymentAtm(order, cart);
						break;
				}

				// 決済連携メモ追記
				if (addExternalPaymentMemo)
				{
					var addPaymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
						(string)order[Constants.FIELD_ORDER_ORDER_ID],
						(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
						cart.Payment.PaymentId,
						(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
						cart.SendingAmount);

					if (this.IsAuthResultHold
						&& ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
						&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)))
					{
						addPaymentMemo += "（与信中）";
					}

					order[Constants.FIELD_ORDER_PAYMENT_MEMO] =
						(order.Contains(Constants.FIELD_ORDER_PAYMENT_MEMO)
							&& (string.IsNullOrEmpty((string)order[Constants.FIELD_ORDER_PAYMENT_MEMO]) == false))
							? (string)order[Constants.FIELD_ORDER_PAYMENT_MEMO] + "\r\n\r\n" + addPaymentMemo
							: addPaymentMemo;
				}

				// 仮クレジットカード向け処理（定数でないのでswitch内に持っていけない）
				if (cart.Payment.PaymentId == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
				{
					if (order.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
					{
						order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
					}
					else
					{
						order.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM);
					}
				}

				// ２－Ｘ．失敗時処理（仮注文ロールバック）
				//		・失敗があれば処理をロールバック（＝仮注文のこす）し、例外をとばす
				// ペイジェントPaidyであれば、この処理を通る前に与信取得を行っているため、ロールバックしない
				if ((result == false) && (PaygentUtility.CheckIsPaidyPaygentPayment(cart.Payment.PaymentId) == false))
				{
					needsRollback = true;
				}

				// ２－Ｓ．決済連携メモ作成処理
				if (result) CreatePaymentMemo(order);
			}
			// 例外時
			catch (Exception ex)
			{
				result = false;

				// 決済自体が完了してしまっていることも考えられるので仮注文データを「のこす」
				// また、ロールバック処理でエラーの場合（仮注文を戻せなかった）も
				// データの不整合を防ぐために仮注文データを「のこす」

				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR));

				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart, ""), ex);
			}

			return result;
		}

		/// <summary>
		/// カード決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCard(
			Hashtable order,
			CartObject cart,
			string successPaymentStatus,
			UpdateHistoryAction updateHistoryAction)
		{
			switch (Constants.PAYMENT_CARD_KBN)
			{
				// ２－１－Ｂ．カード・ゼウス決算処理
				case Constants.PaymentCard.Zeus:
					return ExecPaymentCardZeus(order, cart, successPaymentStatus);

				// ２－１－Ｆ．カード・GMO決算処理
				case Constants.PaymentCard.Gmo:
					return ExecPaymentCardGmo(order, cart, successPaymentStatus, updateHistoryAction);

				// ２－１－Ｇ．カード・SBPSクレジット決算処理
				case Constants.PaymentCard.SBPS:
					return ExecPaymentCardSBPS(order, cart, successPaymentStatus, updateHistoryAction); // 通常のAPI決済

				// ２－１－Ｈ．カード・ヤマトKWCクレジット決算処理
				case Constants.PaymentCard.YamatoKwc:
					return ExecPaymentCardYamatoKwc(order, cart, successPaymentStatus, updateHistoryAction);

				// ２－１－Ｉ．カード・Zcompayment決済処理
				case Constants.PaymentCard.Zcom:
					return ExecPaymentCardZcom(order, cart, successPaymentStatus, updateHistoryAction);

				// ２－１－Ｉ．カード・e-SCOTT決済処理
				case Constants.PaymentCard.EScott:
					return ExecPaymentCardEScott(order, cart, successPaymentStatus, updateHistoryAction);

				// ２－１－J．カード・ベリトランス決済処理
				case Constants.PaymentCard.VeriTrans:
					return ExecPaymentCardVeritrans(order, cart, successPaymentStatus);

				// カード・RAKUTENクレジット決算処理
				case Constants.PaymentCard.Rakuten:
					return ExecPaymentCardRakuten(order, cart, successPaymentStatus, updateHistoryAction);

				// カード・ペイジェント決済処理
				case Constants.PaymentCard.Paygent:
					return ExecPaymentCardPaygent(order, cart, successPaymentStatus, updateHistoryAction);
			}
			return true; // 決済連動させない場合はここに来る
		}

		/// <summary>
		/// コンビニ決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCvs(Hashtable order, CartObject cart)
		{
			switch (Constants.PAYMENT_CVS_KBN)
			{
				// ２－２－Ｃ．コンビニ・電算システム決済処理
				case Constants.PaymentCvs.Dsk:
					return ExecPaymentCvsDsk(order, cart);

				// ２－２－Ｅ．コンビニ・SBPS決済処理
				case Constants.PaymentCvs.SBPS:
					return ExecPaymentCvsSBPS(order, cart);

				// ２－２－Ｆ．コンビニ・ヤマトKWC決済処理
				case Constants.PaymentCvs.YamatoKwc:
					return ExecPaymentCvsYamatoKwc(order, cart);

				// ２－２－Ｇ．コンビニ・GMO
				case Constants.PaymentCvs.Gmo:
					return ExecPaymentCvsGmo(order, cart);

				// ２－２－Ｈ．コンビニ・RAKUTEN
				case Constants.PaymentCvs.Rakuten:
					return ExecPaymentCvsRakuten(order, cart);

				// ２－２－Ｈ．コンビニ・ZEUS
				case Constants.PaymentCvs.Zeus:
					return ExecPaymentCvsZeus(order, cart);

				// ２－２－Ｉ．コンビニ・Paygent
				case Constants.PaymentCvs.Paygent:
					return ExecPaymentCvsPaygent(order, cart);
			}
			return true;	// 決済連動させない場合はここに来る
		}

		/// <summary>
		/// ゼウスクレジット決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCardZeus(Hashtable order, CartObject cart, string successPaymentStatus)
		{
			this.TransactionName = "2-1-B.ゼウスクレジット決済処理";

			var successAuth = false;
			var isTokenExpired = false;
			var errorTypeCode = "";

			// ZEUS決済方法取得
			Constants.PaymentCreditCardPaymentMethod? paymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && cart.HasDigitalContents)
				? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS
				: Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD;

			// 3Dセキュア利用向け実行
			if ((this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
				&& Constants.PAYMENT_SETTING_ZEUS_3DSECURE)
			{
				var payment = new ZeusCreditFor3DSecure();
				successAuth = payment.SecureApiAuth(order, cart);

				// 処理成功かつゼウスオーダーIDがセットされていない場合（=要3Dセキュア認証）、次カートへ
				if ((successAuth) && (payment.ZeusOrderId == null))
				{
					this.ZeusCard3DSecurePaymentOrders.Add(order);
					return true;
				}
				if (successAuth)
				{
					if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
					{
						order[Constants.FIELD_ORDER_CARD_TRAN_ID] = payment.ZeusOrderId;
					}
					else
					{
						order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, payment.ZeusOrderId);
					}
				}
				else
				{
					this.ApiErrorMessage = payment.ErrorMessage;
					errorTypeCode = payment.ErrorTypeCode;
				}
			}
			// SecureLink実行
			else
			{
				var result = new ZeusSecureLinkApi().Exec(
					(cart.Payment.CreditToken != null) ? cart.Payment.CreditToken.Token : null,	// トークン無い場合は既存カード利用
					(paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH) ? 0 : cart.SendingAmount,
					cart.Payment.UserCreditCard.CooperationInfo.ZeusTelNo,
					(cart.Owner.MailAddr != "") ? cart.Owner.MailAddr : cart.Owner.MailAddr2,
					cart.Payment.UserCreditCard.CooperationInfo.ZeusSendId,
					cart.Payment.CreditInstallmentsCode);
				successAuth = result.Success;
				if (successAuth)
				{
					if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
					{
						order[Constants.FIELD_ORDER_CARD_TRAN_ID] = result.ZeusOrderId;
					}
					else
					{
						order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, result.ZeusOrderId);
					}
				}
				else
				{
					this.ApiErrorMessage = result.ErrorMessage;
					errorTypeCode = result.ErrorTypeCode;
				}
			}

			if (successAuth)
			{
				if (order.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
				{
					order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, successPaymentStatus);
				}
				if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = "";
				}
			}
			else
			{
				// 発行されたトークンキーは複数回利用は出来ない仕様のため、一度失敗したら有効期限切れとする
				if (cart.Payment.CreditToken != null)
				{
					cart.Payment.CreditToken.SetTokneExpired();
					isTokenExpired = true;
				}

				// エラー詳細表示
				var cardErrorMessageForPc = "";	// PCサイト向けに優先したいクレジットカードメッセージ
				if (Constants.CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED)
				{
					// PC決済のみカード会社からのエラーコードをメッセージとして追加
					if (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
					{
						var creditError = new CreditErrorMessage();
						creditError.SetCreditErrorMessages(Constants.FILE_XML_ZEUS_CREDIT_ERROR_MESSAGE);
						var errorList = creditError.GetValueItemArray();
						if (errorList.Any(s => s.Value == errorTypeCode))
						{
							cardErrorMessageForPc = errorList.First(s => s.Value == errorTypeCode).Text;
							this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR));
							this.ErrorMessages.Add(cardErrorMessageForPc);
						}
					}
				}
				if (string.IsNullOrEmpty(cardErrorMessageForPc))
				{
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
					if (isTokenExpired) this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CREDIT_TOKEN_EXPIRED));
				}
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart, ""));
			}

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				successAuth,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Zeus,
				PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
				successAuth ? "" : OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart, this.ApiErrorMessage),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
					{Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]}
				});
			return successAuth;
		}

		/// <summary>
		/// GMOクレジット決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCardGmo(
			Hashtable order,
			CartObject cart,
			string successPaymentStatus,
			UpdateHistoryAction updateHistoryAction)
		{
			this.TransactionName = "2-1-F.GMOクレジット決済処理";
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			//------------------------------------------------------
			// 取引登録
			//------------------------------------------------------
			var paymentGMO = new PaymentGmoCredit();
			// HACK:BotchanがFront扱いするので戻りURLがない場合はEC扱いにする
			var execType = (Constants.PAYMENT_SETTING_GMO_3DSECURE && (string.IsNullOrEmpty((string)order[PaymentGmo.RETURN_URL])))
				? OrderRegisterBase.ExecTypes.CommerceManager
				: this.OrderExecType;
			if (paymentGMO.EntryTran(paymentOrderId, cart.SendingAmount, execType) == false)
			{
				this.ApiErrorMessage = paymentGMO.ErrorMessages;
				this.ErrorMessages.Add(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_GMO_CARDAUTH_ERROR_ENTRYTRAN));
				this.ErrorMessages.Add(paymentGMO.ErrorMessages);
				AppLogger.WriteInfo(
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						string.Join("\r\n", this.ErrorMessages)));
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						string.Join("\r\n", this.ErrorMessages)),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
						{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] },
						{ Constants.FIELD_USER_USER_ID, cart.CartUserId },
						{ Constants.FIELD_ORDER_CREDIT_BRANCH_NO, cart.Payment.UserCreditCard.BranchNo.ToString() }
					});
				return false;
			}
			else
			{
				// 成功時ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					true,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
					"",
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
						{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] },
						{ Constants.FIELD_USER_USER_ID, cart.CartUserId },
						{ Constants.FIELD_ORDER_CREDIT_BRANCH_NO, cart.Payment.UserCreditCard.BranchNo.ToString() }
					});
			}

			// 決済受付番号として取引IDと取引パスワードを格納
			if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
			{
				order[Constants.FIELD_ORDER_CARD_TRAN_ID] = paymentGMO.AccessId + " " + paymentGMO.AccessPass;
			}
			else
			{
				order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, paymentGMO.AccessId + " " + paymentGMO.AccessPass);
			}

			// 決済注文IDを格納
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			//------------------------------------------------------
			// 登録済みカードが存在するか？
			//------------------------------------------------------
			// 登録済みカード利用 or 定期購入？
			bool useCard = false;
			if ((this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch)
				|| (cart.Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
			{
				// 登録済みカードが存在しない？
				if (paymentGMO.SearchCard(cart.Payment.UserCreditCard.CooperationInfo.GMOMemberId) == false)
				{
					this.ErrorMessages.Add("登録済みカード情報がありません。");
					this.ErrorMessages.Add(paymentGMO.ErrorMessages);
					this.ApiErrorMessage = paymentGMO.ErrorMessages;
					AppLogger.WriteInfo(
						OrderCommon.CreateOrderFailedLogMessage(
							this.TransactionName,
							order,
							cart,
							string.Join("\r\n", this.ErrorMessages)));
					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						false,
						cart.Payment.PaymentId,
						PaymentFileLogger.PaymentType.Gmo,
						PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
						OrderCommon.CreateOrderFailedLogMessage(
							this.TransactionName,
							order,
							cart,
							string.Join(",", this.ErrorMessages)),
						new Dictionary<string, string>
						{
							{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
							{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
							{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] },
							{ Constants.FIELD_USER_USER_ID, cart.CartUserId },
							{ Constants.FIELD_ORDER_CREDIT_BRANCH_NO, cart.Payment.UserCreditCard.BranchNo.ToString() }
						});
					return false;
				}
				else
				{
					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						true,
						cart.Payment.PaymentId,
						PaymentFileLogger.PaymentType.Gmo,
						PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
						"",
						new Dictionary<string, string>
						{
							{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
							{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
							{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] },
							{ Constants.FIELD_USER_USER_ID, cart.CartUserId },
							{ Constants.FIELD_ORDER_CREDIT_BRANCH_NO, cart.Payment.UserCreditCard.BranchNo.ToString() }
						});
				}

				useCard = true;
			}

			//------------------------------------------------------
			// 決済実行
			//------------------------------------------------------
			// 登録済みカード利用？
			bool result = false;
			if (useCard)
			{
				result = paymentGMO.ExecTranUseCard(
					paymentOrderId,
					(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
					cart.Payment.UserCreditCard.CooperationInfo.GMOMemberId,
					cart.Payment.CreditInstallmentsCode,
					cart.Payment.CreditSecurityCode,
					(string)order[PaymentGmo.RETURN_URL]);
			}
			// カード入力？
			else
			{
				result = paymentGMO.ExecTran(
					paymentOrderId,
					cart.Payment.CreditInstallmentsCode,
					cart.Payment.CreditToken.Token,
					(string)order[PaymentGmo.RETURN_URL]);
			}

			if (result == false)
			{
				// エラー詳細表示
				var cardErrorMessageForPc = ""; // PCサイト向けに優先したいクレジットカードメッセージ
				if (Constants.CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED)
				{
					// PC決済のみカード会社からのエラーコードをメッセージとして追加
					if (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
					{
						var creditError = new CreditErrorMessage();
						creditError.SetCreditErrorMessages(Constants.FILE_XML_GMO_CREDIT_ERROR_MESSAGE);
						var errorList = creditError.GetValueItemArray();
						if (errorList.Any(s => s.Value == paymentGMO.ErrorTypeCode))
						{
							cardErrorMessageForPc = errorList.First(s => s.Value == paymentGMO.ErrorTypeCode).Text;
							this.ErrorMessages.Add(
								CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR));
							this.ErrorMessages.Add(cardErrorMessageForPc);
						}
					}
				}

				if (string.IsNullOrEmpty(cardErrorMessageForPc)) this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));

				this.ApiErrorMessage = paymentGMO.ErrorMessages;

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						string.Join(",", this.ErrorMessages)),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
						{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] },
						{ Constants.FIELD_USER_USER_ID, cart.CartUserId },
						{ Constants.FIELD_ORDER_CREDIT_BRANCH_NO, cart.Payment.UserCreditCard.BranchNo.ToString() }
					});

				AppLogger.WriteInfo(
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						string.Join("\r\n", this.ErrorMessages)));
				return false;
			}
			else
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					true,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
					"",
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
						{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] },
						{ Constants.FIELD_USER_USER_ID, cart.CartUserId },
						{ Constants.FIELD_ORDER_CREDIT_BRANCH_NO, cart.Payment.UserCreditCard.BranchNo.ToString() }
					});
			}

			// 3Dセキュア2.0利用向け対応
			if (Constants.PAYMENT_SETTING_GMO_3DSECURE
				&& (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
				&& paymentGMO.IsTds2
				&& (string.IsNullOrEmpty(paymentGMO.RedirectUrl) == false))
			{
				order[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL] = paymentGMO.RedirectUrl;
				this.GmoCard3DSecurePaymentOrders.Add(order);
				// レスポンスの値を保存
				new OrderService().Update3DSecureInfo(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
					paymentGMO.RedirectUrl,
					"",
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);

				FileLogger.Write(
					"GmoTran",
					string.Format(
						"\tGMO３Dセキュア決済: 決済注文ID:{0} 取引ID 取引PASS:{1}",
						paymentOrderId,
						(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]));
			}
			// 3Dセキュア1.0
			else if (Constants.PAYMENT_SETTING_GMO_3DSECURE
				&& (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
				&& paymentGMO.IsTds1
				&& (string.IsNullOrEmpty(paymentGMO.Md) == false)
				&& (string.IsNullOrEmpty(paymentGMO.AcsUrl) == false)
				&& (string.IsNullOrEmpty(paymentGMO.PaReq) == false))
			{
				this.GmoCard3DSecurePaymentOrders.Add(order);
				// レスポンスの値を保存
				new OrderService().Update3DSecureInfo(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
					paymentGMO.AcsUrl,
					paymentGMO.PaReq,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);

				FileLogger.Write(
					"GmoTran",
					string.Format(
						"\tGMO３Dセキュア決済: 決済注文ID:{0} 取引ID 取引PASS:{1}",
						paymentOrderId,
						(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]));
			}
			else
			{
				// カード登録する場合、GMO側のカード登録＆w2側の連携ID更新を行う
				if ((this.OrderExecType != OrderRegisterBase.ExecTypes.FixedPurchaseBatch) && (useCard == false))
				{
					// 会員登録＆カード登録
					var registered = paymentGMO.SaveMemberAndTraedCard(
					cart.Payment.UserCreditCard.CooperationInfo.GMOMemberId,
					cart.Owner.Name,
					paymentOrderId,
					cart.Payment.CreditAuthorName);
					if (registered)
					{
						// カード登録連番を連携IDに格納
						var updated = cart.Payment.UserCreditCard.UpdateCooperationId(
						cart.Payment.UserCreditCard.CooperationInfo.GMOMemberId,
						Constants.FLG_LASTCHANGED_USER,
							UpdateHistoryAction.DoNotInsert);
						if (updated == false)
						{
							AppLogger.WriteError(
								string.Format(
									"クレジット情報の登録(連携ID更新に失敗: UserId = {0} BranchNo= {1}",
									cart.CartUserId,
									cart.Payment.UserCreditCard.BranchNo));
						}
						else if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
						{
							// カード登録を選択した場合、表示フラグを立つ
							cart.Payment.UserCreditCard.UpdateDispFlg(
								cart.Payment.UserCreditCardRegistFlg,
								Constants.FLG_LASTCHANGED_USER,
								updateHistoryAction);
						}
					}
					else
					{
						// 登録だけ失敗した場合はカードレコードだけ削除しておく（決済自体は成功させる）
						cart.Payment.UserCreditCard.Delete(updateHistoryAction);
						this.ApiErrorMessage = paymentGMO.ErrorMessages;
					}
				}
				
				// 入金ステータスを格納
				if (order.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
				{
					order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, successPaymentStatus);
				}
			}
			return true;
		}

		/// <summary>
		/// Rakuten exec payment card
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <param name="successPaymentStatus">SuccessPaymentStatus</param>
		/// <param name="updateHistoryAction">UpdateHistoryAction</param>
		/// <returns>Result exec</returns>
		private bool ExecPaymentCardRakuten(
			Hashtable order,
			CartObject cart,
			string successPaymentStatus,
			UpdateHistoryAction updateHistoryAction)
		{
			this.TransactionName = "2-1-G.Rakutenクレジット決済処理";
			var cardErrorMessageForPc = string.Empty;	// PCサイト向けに優先したいクレジットカードメッセージ
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			var context = HttpContext.Current;
			var ipAddress = (context != null) ? context.Request.ServerVariables["REMOTE_ADDR"] : "";

			var rakutenAuthourizeRequest = new RakutenAuthorizeRequest(ipAddress)
			{
				PaymentId = paymentOrderId,
				GrossAmount = cart.PriceTotal,
				CardToken = new CardTokenBase
				{
					Amount = cart.PriceTotal.ToString("0"),
					CardToken = cart.Payment.UserCreditCard.CooperationId,
					WithThreeDSecure = false,
					CvvToken = StringUtility.ToNull(cart.Payment.RakutenCvvToken),
				},
			};

			switch (cart.Payment.CreditInstallmentsCode)
			{
				// 一括
				case RakutenConstants.DealingTypes.ONCE:
					break;

				// リボ払い
				case RakutenConstants.DealingTypes.REVO:
					rakutenAuthourizeRequest.CardToken.WithRevolving = true;
					break;

				// ボーナス一括
				case RakutenConstants.DealingTypes.BONUS1:
					rakutenAuthourizeRequest.CardToken.WithBonus = true;
					break;

				case "":
					break;

				// 支払い回数
				default:
					rakutenAuthourizeRequest.CardToken.Installments = cart.Payment.CreditInstallmentsCode;
					break;

			}

			var apiResponse = RakutenApiFacade.AuthorizeAPI(rakutenAuthourizeRequest);
			var paymentSuccess = true;

			if (apiResponse.ResultType != RakutenConstants.RESULT_TYPE_SUCCESS)
			{
				paymentSuccess = false;
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(
					apiResponse.ErrorCode,
					apiResponse.ErrorMessage);
			}

			if (paymentSuccess == false)
			{
				// エラーコードをログに落とす暫定対応
				FileLogger.Write("Rakuten", this.ApiErrorMessage, true);

				// エラー詳細表示
				if (Constants.CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED
					&& (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc))
				{
					var creditError = new CreditErrorMessage();
					creditError.SetCreditErrorMessages(Constants.FILE_XML_RAKUTEN_CREDIT_ERROR_MESSAGE);
					var errorList = creditError.GetValueItemArray();
					cardErrorMessageForPc = (errorList.Any(s => s.Value == apiResponse.ErrorCode))
						? errorList.First(s => (s.Value == apiResponse.ErrorCode)).Text
						: string.Empty;
				}
			}

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				paymentSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Sbps,
				PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
				paymentSuccess
					? string.Empty
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						cardErrorMessageForPc),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
					{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] }
				});

			if (paymentSuccess == false)
			{
				AppLogger.WriteInfo(
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						cardErrorMessageForPc));

				if (string.IsNullOrEmpty(cardErrorMessageForPc))
				{
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
				}
				else
				{
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR));
					this.ErrorMessages.Add(cardErrorMessageForPc);
				}
				if (apiResponse.ResultType == RakutenConstants.RESULT_TYPE_PENDING)
				{
					throw new Exception(string.Format("決済処理でpendingが発生しました。"));
				}
				return false;
			}

			order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = apiResponse.AgencyRequestId;
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				// カード登録を選択した場合、表示フラグを立てる
				cart.Payment.UserCreditCard.UpdateDispFlg(
					cart.Payment.UserCreditCardRegistFlg,
					Constants.FLG_LASTCHANGED_USER,
					updateHistoryAction);
			}
			return true;
		}

		/// <summary>
		/// SBPSクレジット決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCardSBPS(
			Hashtable order,
			CartObject cart,
			string successPaymentStatus,
			UpdateHistoryAction updateHistoryAction)
		{
			this.TransactionName = "2-1-G.SBPSクレジット決済処理";
			var paymentSuccess = true;
			var isTokenExpired = false;
			var cardErrorMessageForPc = "";	// PCサイト向けに優先したいクレジットカードメッセージ
			StringBuilder paymentErrorMessage = new StringBuilder();

			string trackingId = null;
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			// SBPSクレジット決済方法設定を取得
			var paymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && cart.HasDigitalContents)
				? Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS
				: Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD;

			// 通常決済？
			if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
			{
				PaymentSBPSCreditAuthResponseData response = null;
				// 登録カード利用 or 定期購入 の時は顧客IDで決済
				if ((this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch)
					|| (cart.Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
				{
					var api = new PaymentSBPSCreditAuthApi();
					paymentSuccess = api.Exec(
						cart.Payment.UserCreditCard.CooperationInfo.SBPSCustCode,
						paymentOrderId,
						Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
						Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
						new List<PaymentSBPSBase.ProductItem>(),
						cart.SendingAmount,
						PaymentSBPSUtil.GetCreditDivideInfo(cart.Payment.CreditInstallmentsCode));
					response = api.ResponseData;

					if (paymentSuccess == false)
					{
						this.ApiErrorMessage = LogCreator.CreateErrorMessage(
							api.ResponseData.ResErrCode,
							api.ResponseData.ResErrMessages);
					}
				}
				// トークンを利用して決済
				else if (OrderCommon.CreditTokenUse)
				{
					var api = new PaymentSBPSCreditAuthWithTokenApi();
					paymentSuccess = api.Exec(
						cart.Payment.UserCreditCard.CooperationInfo.SBPSCustCode,
						paymentOrderId,
						Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
						Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
						new List<PaymentSBPSBase.ProductItem>(),
						cart.SendingAmount,
						cart.Payment.CreditToken,
						PaymentSBPSUtil.GetCreditDivideInfo(cart.Payment.CreditInstallmentsCode),
						true);

					if (paymentSuccess == false)
					{
						this.ApiErrorMessage = LogCreator.CreateErrorMessage(
							api.ResponseData.ResErrCode,
							api.ResponseData.ResErrMessages);
					}

					if (api.ResponseData.IsTokenExpired)
					{
						cart.Payment.CreditToken.SetTokneExpired();
						isTokenExpired = true;
					}
					response = api.ResponseData;
				}
				// それ以外は永久トークンを利用
				else
				{
					var api = new PaymentSBPSCreditAuthWithTokenizedPanApi();
					paymentSuccess = api.Exec(
						cart.Payment.UserCreditCard.CooperationInfo.SBPSCustCode,
						paymentOrderId,
						Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
						Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
						new List<PaymentSBPSBase.ProductItem>(),
						cart.SendingAmount,
						cart.Payment.CreditCardNo,
						cart.Payment.CreditExpireYear,
						cart.Payment.CreditExpireMonth,
						PaymentSBPSUtil.GetCreditDivideInfo(cart.Payment.CreditInstallmentsCode),
						true);
					response = api.ResponseData;

					if (paymentSuccess == false)
					{
						this.ApiErrorMessage = LogCreator.CreateErrorMessage(
							api.ResponseData.ResErrCode,
							api.ResponseData.ResErrMessages);

						paymentErrorMessage.Append(response.ResErrMessages);
					}
				}

				// SBPSクレジット「コミット」処理
				if (paymentSuccess)
				{
					var commitApi = new PaymentSBPSCreditCommitApi();
					paymentSuccess = commitApi.Exec(response.ResTrackingId);
					if (paymentSuccess == false)
					{
						paymentErrorMessage.Append(commitApi.ResponseData.ResErrMessages);
						this.ApiErrorMessage = LogCreator.CreateErrorMessage(
							commitApi.ResponseData.ResErrCode,
							commitApi.ResponseData.ResErrMessages);
					}
				}

				// 全て成功ならトラッキングID取得
				if (paymentSuccess) trackingId = response.ResTrackingId;

				if (paymentSuccess == false)
				{
					// エラーコードをログに落とす暫定対応
					FileLogger.Write("SBPS", this.ApiErrorMessage, true);

					// エラー詳細表示
					if (Constants.CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED)
					{
						// PC決済のみカード会社からのエラーコードをメッセージとして追加
						if (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
						{
							var creditError = new CreditErrorMessage();
							creditError.SetCreditErrorMessages(Constants.FILE_XML_SBPS_CREDIT_ERROR_MESSAGE);
							var errorList = creditError.GetValueItemArray();
							cardErrorMessageForPc = (errorList.Any(s => s.Value == response.ErrorTypeCode))
								? errorList.First(s => s.Value == response.ErrorTypeCode).Text
								: "";
						}
					}
				}
			}
			// 与信後決済？
			else if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
			{
				// 新規カード利用のみ
				if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					// SBPSクレジット「顧客登録」実行（トークン利用）
					if (OrderCommon.CreditTokenUse)
					{
						var custRegist = new PaymentSBPSCreditCustomerRegistWithTokenApi();
						paymentSuccess = custRegist.Exec(
							cart.Payment.UserCreditCard.CooperationInfo.SBPSCustCode,
							cart.Payment.CreditToken);
						if (paymentSuccess == false)
						{
							this.ApiErrorMessage = LogCreator.CreateErrorMessage(
								custRegist.ResponseData.ResErrCode,
								custRegist.ResponseData.ResErrMessages);
							paymentErrorMessage.Append(custRegist.ResponseData.ResErrMessages);
						}
					}
					// SBPSクレジット「顧客登録」実行（永久トークン利用）
					else
					{
						var custRegist = new PaymentSBPSCreditCustomerRegistWithTokenizedPanApi();
						paymentSuccess = custRegist.Exec(
							cart.Payment.UserCreditCard.CooperationInfo.SBPSCustCode,
							cart.Payment.CreditCardNo,
							cart.Payment.CreditExpireYear,
							cart.Payment.CreditExpireMonth);
						if (paymentSuccess == false)
						{
							paymentErrorMessage.Append(custRegist.ResponseData.ResErrMessages);
							this.ApiErrorMessage = LogCreator.CreateErrorMessage(
								custRegist.ResponseData.ResErrCode,
								custRegist.ResponseData.ResErrMessages);
						}
					}
				}
			}

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				paymentSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Sbps,
				PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
				paymentSuccess
					? ""
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						paymentErrorMessage.ToString()),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
					{Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]}
				});

			if (paymentSuccess)
			{
				if (order.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
				{
					order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, successPaymentStatus);
				}
				if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(trackingId);
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, StringUtility.ToEmpty(trackingId));
				}
				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

				if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					// カード登録を選択した場合、表示フラグを立てる
					cart.Payment.UserCreditCard.UpdateDispFlg(
						cart.Payment.UserCreditCardRegistFlg,
						Constants.FLG_LASTCHANGED_USER,
						updateHistoryAction);
				}
				return true;
			}
			else
			{
				AppLogger.WriteInfo(
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						paymentErrorMessage.ToString()));

				if (isTokenExpired)
				{
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CREDIT_TOKEN_EXPIRED));
				}
				else
				{
					if (string.IsNullOrEmpty(cardErrorMessageForPc))
					{
						this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
					}
					else
					{
						this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR));
						this.ErrorMessages.Add(cardErrorMessageForPc);
					}
				}
				return false;
			}
		}

		/// <summary>
		/// ヤマトKWCクレジット決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCardYamatoKwc(Hashtable order, CartObject cart, string successPaymentStatus, UpdateHistoryAction updateHistoryAction)
		{
			this.TransactionName = "2-1-H.ヤマトKWCクレジット決済処理";
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			var mailAddress = (string.IsNullOrEmpty(cart.Owner.MailAddr) ? Constants.PAYMENT_SETTING_YAMATO_KWC_DUMMY_MAILADDRESS : cart.Owner.MailAddr);
			var param = GetPaymentYamatoKwcCreditOptionParam(cart);

			if (param == null)
			{
				this.Properties.IsExpiredYamatoKwcCredit = true;
				return false;
			}
			// 与信実行
			var responseData = new PaymentYamatoKwcCreditAuthApi((cart.Payment.CreditToken != null)).Exec(
				GetYamatoKwcDeviceDiv((string)order[Constants.FIELD_ORDER_ORDER_KBN]),
				paymentOrderId,
				Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
				cart.SendingAmount,
				cart.Owner.Name,
				cart.Owner.Tel1,
				mailAddress,
				string.IsNullOrEmpty(cart.Payment.CreditInstallmentsCode) ? 1 : int.Parse(cart.Payment.CreditInstallmentsCode),
				GetPaymentYamatoKwcCreditOptionParam(cart),
				(string)order[Constants.FIELD_ORDER_ORDER_ID]);

			// 失敗ならエラーログ出力して抜ける
			if (responseData.Success == false)
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(responseData.ErrorCode, responseData.ErrorMessage);
				// 発行されたトークンキーは複数回利用は出来ない仕様のため、一度失敗したら有効期限切れとする
				if (cart.Payment.CreditToken != null)
				{
					cart.Payment.CreditToken.SetTokneExpired();
				}

				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					order,
					cart,
					responseData.ErrorInfoForLog
						+ (string.IsNullOrEmpty(responseData.CreditErrorCode)
							? ""
							: string.Format("（{0}:{1}）", responseData.CreditErrorCode, responseData.CreditErrorMessage))));

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Yamatokwc,
					PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						responseData.ErrorInfoForLog + (string.IsNullOrEmpty(responseData.CreditErrorCode)
							? ""
							: LogCreator.CreateErrorMessage(
								responseData.CreditErrorCode,
								responseData.CreditErrorMessage))),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{
							Constants.FIELD_ORDER_PAYMENT_ORDER_ID,
							(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]
						},
						{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] }
					});

				// エラー詳細表示
				var cardErrorMessageForPc = "";	// PCサイト向けに優先したいクレジットカードメッセージ
				if (Constants.CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED)
				{
					// PC決済のみカード会社からのエラーコードをメッセージとして追加
					if (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
					{
						var creditError = new CreditErrorMessage();
						creditError.SetCreditErrorMessages(Constants.FILE_XML_YAMATOKWC_CREDIT_ERROR_MESSAGE);
						var errorList = creditError.GetValueItemArray();
						if (errorList.Any(s => s.Value == responseData.CreditErrorCode))
						{
							cardErrorMessageForPc = errorList.First(s => s.Value == responseData.CreditErrorCode).Text;
							this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR));
							this.ErrorMessages.Add(cardErrorMessageForPc);
						}
					}
				}
				if (string.IsNullOrEmpty(cardErrorMessageForPc)) this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));

				return false;
			}
			else
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					true,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Yamatokwc,
					PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
					"",
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
						{Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]}
					});
			}

			order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(responseData.CrdCResCd);
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			if (Constants.PAYMENT_SETTING_YAMATO_KWC_3DSECURE
				&& (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
				&& (string.IsNullOrEmpty(responseData.ThreeDAuthHtml) == false))
			{
				order[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL] = responseData.ThreeDAuthHtml;
				order[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_KEY] = responseData.ThreeDToken;

				this.YamatoCard3DSecurePaymentOrders.Add(order);

				// レスポンスの値を保存
				new OrderService().Update3DSecureInfo(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
					responseData.ThreeDAuthHtml,
					responseData.ThreeDToken,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);

				FileLogger.Write(
					"YamatoTran",
					string.Format(
						"\tヤマトKWC ３Dセキュア決済: 決済注文ID:{0} 取引ID 取引PASS:{1}",
						paymentOrderId,
						(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]));
			}

			if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				// カード登録を選択した場合、表示フラグを立つ
				cart.Payment.UserCreditCard.UpdateDispFlg(
					cart.Payment.UserCreditCardRegistFlg,
					Constants.FLG_LASTCHANGED_USER,
					updateHistoryAction);
			}
			return true;
		}

		/// <summary>
		/// ヤマトKWCクレジットオプションサービスパラメタ
		/// </summary>
		/// <param name="cart">カート</param>
		/// <returns>パラメタ</returns>
		private PaymentYamatoKwcCreditOptionServiceParamBase GetPaymentYamatoKwcCreditOptionParam(CartObject cart)
		{
			// 定期購入 OR 登録カード利用の時はオプションサービス利用
			if ((this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch)
				|| (cart.Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
			{
				var memberId = cart.Payment.UserCreditCard.CooperationInfo.YamatoKwcMemberId;
				var authenticationKey = cart.Payment.UserCreditCard.CooperationInfo.YamatoKwcAuthenticationKey;
				var creditInfoResult = new PaymentYamatoKwcCreditInfoGetApi().Exec(memberId, authenticationKey);
				if (creditInfoResult.Success)
				{
					if (creditInfoResult.CardDatas.Count > 0)
					{
						return new PaymentYamatoKwcCreditOptionServiceParamOptionRep(
							memberId, authenticationKey, creditInfoResult.CardDatas[0], cart.Payment.CreditSecurityCode);
					}

					return null;
				}
				else
				{
					var orderModel = new OrderService().Get(cart.OrderId);
					if (orderModel != null)
					{
						// 外部決済連携ログ格納処理
						var apiErrorMessage = LogCreator.CreateErrorMessage(
							creditInfoResult.ErrorCode,
							creditInfoResult.ErrorMessage);

						OrderCommon.AppendExternalPaymentCooperationLog(
							false,
							cart.OrderId,
							apiErrorMessage,
							orderModel.LastChanged,
							UpdateHistoryAction.Insert);
					}
					else
					{
						OrderCommon.AppendExternalPaymentCooperationLog(
							true,
							cart.OrderId,
							LogCreator.CreateMessage(cart.OrderId, ""),
							"",
							UpdateHistoryAction.Insert);
					}
				}

				throw new Exception(string.Format("カード情報が取得に失敗しました。（{0}）：{1},{2}", creditInfoResult.ErrorInfoForLog, memberId, authenticationKey));
			}
			// 登録カード利用なし AND カード登録する 時はカードを登録する
			if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				return new PaymentYamatoKwcCreditOptionServiceParamOptionReg(
					cart.Payment.CreditCardCompany,
					cart.Payment.CreditToken.Token);
			}
			// 通常決済(オプション利用無し）・・・だったが、再与信対応のため登録しておく
			return new PaymentYamatoKwcCreditOptionServiceParamOptionReg(
				cart.Payment.CreditCardCompany,
				cart.Payment.CreditToken.Token);
		}

		/// <summary>
		/// Zcom決済
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カード</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>
		/// TRUE：成功
		/// FALSE：失敗
		/// </returns>
		private bool ExecPaymentCardZcom(
			Hashtable order,
			CartObject cart,
			string successPaymentStatus,
			UpdateHistoryAction updateHistoryAction)
		{
			this.TransactionName = "2-1-I.Zcompaymentクレジット決済処理";
			var adp = new ZcomDirectRequestCartAdapter(cart);

			// 実行
			var response = adp.Execute();

			var executeResult = response.IsSuccessResult();

			if ((response.GetResultValue() == ZcomConst.RESULT_CODE_REDIRECT_URL)
				&& (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
				&& string.IsNullOrEmpty(cart.OrderCombineParentOrderId)
				&& string.IsNullOrEmpty(cart.BeforeRecommendOrderId))
			{
				order[ZcomConst.PARAM_ACCESS_URL] = HttpUtility.UrlDecode(response.GetAccessUrlValue());
				order[ZcomConst.PARAM_MODE] = HttpUtility.UrlDecode(response.GetModeValue());
				order[ZcomConst.PARAM_TRANS_CODE_HASH] = HttpUtility.UrlDecode(response.GetTransCodeHashValue());
				order[ZcomConst.PARAM_PAYMENT_CODE] = HttpUtility.UrlDecode(response.GetPaymentCodeValue());
				order[ZcomConst.PARAM_TRANS_CODE] = StringUtility.ToEmpty(response.GetTransCodeValue());
				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = adp.PaymentOrderId;

				this.ZcomCard3DSecurePaymentOrders.Add(order);
				return true;
			}

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				executeResult,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Zcom,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				executeResult
					? ""
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						string.Format(
							"（{0}:{1}）",
							response.GetErrorCodeValue(),
							HttpUtility.UrlDecode(response.GetErrorDetailValue()))),
				new Dictionary<string, string>
			{
					{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
				});

			if (executeResult == false)
			{
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					order,
					cart,
					string.Format("（{0}:{1}）", response.GetErrorCodeValue(), HttpUtility.UrlDecode(response.GetErrorDetailValue()))));
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(
					response.GetErrorCodeValue(),
					response.GetErrorDetailValue());
				return false;
			}

			order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(response.GetTransCodeValue());
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = adp.PaymentOrderId;

			if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				// 連携IDセット
				cart.Payment.UserCreditCard.UpdateCooperationId(
					adp.PaymentUserId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);

				// カード登録を選択した場合、表示フラグを立つ
				cart.Payment.UserCreditCard.UpdateDispFlg(
					cart.Payment.UserCreditCardRegistFlg,
					Constants.FLG_LASTCHANGED_USER,
					updateHistoryAction);
			}

			return true;
		}

		/// <summary>
		/// e-SCOTT決済
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カード</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>
		/// TRUE：成功
		/// FALSE：失敗
		/// </returns>
		private bool ExecPaymentCardEScott(
			Hashtable order,
			CartObject cart,
			string successPaymentStatus,
			UpdateHistoryAction updateHistoryAction)
		{
			this.TransactionName = "2-1-I.e-SCOTTクレジット決済処理";
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			var paymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && cart.HasDigitalContents)
				? Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD_FORDIGITALCONTENTS
				: Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD;
			var cardTranId = string.Empty;

			if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				var adpMember = EScottMember4MemAddApi.CreateEScottMember4MemAddApiByCart(cart);
				var memberResult = adpMember.ExecRequest();
				// 失敗ならエラーログ出力して抜ける
				if (memberResult.IsSuccess == false)
				{
					AddErrorMessageByEScott(memberResult.ResponseCd, memberResult.ResponseMessage, cart, order);
					return false;
				}
				WriteSuccessLogByEScott(cart, order);
			}

			if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
			{
				var adp = EScottMaster1AuthApi.CreateEScottMaster1AuthApi(cart, paymentOrderId);

				// 実行
				var responseData = adp.ExecRequest();

				// 失敗ならエラーログ出力して抜ける
				if (responseData.IsSuccess == false)
				{
					AddErrorMessageByEScott(responseData.ResponseCd, responseData.ResponseMessage, cart, order);
					return false;
				}
				WriteSuccessLogByEScott(cart, order);
				cardTranId = responseData.CardTranId;
			}

			order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(cardTranId);
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				// カード登録を選択した場合、表示フラグを立つ
				cart.Payment.UserCreditCard.UpdateDispFlg(
					cart.Payment.UserCreditCardRegistFlg,
					Constants.FLG_LASTCHANGED_USER,
					updateHistoryAction);
			}
			return true;
		}

		/// <summary>
		/// e-SCOTT向けエラーメッセージ追加
		/// </summary>
		/// <param name="errorCode">エラーコード</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <param name="cart">カート</param>
		/// <param name="order">受注</param>
		private void AddErrorMessageByEScott(string errorCode, string errorMessage, CartObject cart, Hashtable order)
		{
			this.ApiErrorMessage = LogCreator.CreateErrorMessage(errorCode, errorMessage);
			// 発行されたトークンキーは複数回利用は出来ない仕様のため、一度失敗したら有効期限切れとする
			if (cart.Payment.CreditToken != null)
			{
				cart.Payment.CreditToken.SetTokneExpired();
			}

			AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(
				this.TransactionName,
				order,
				cart,
				errorCode));

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.EScott,
				PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
				OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					order,
					cart,
					this.ApiErrorMessage),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{
						Constants.FIELD_ORDER_PAYMENT_ORDER_ID,
						(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]
					},
					{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] }
				});

			// エラー詳細表示
			var cardErrorMessageForPc = string.Empty;	// PCサイト向けに優先したいクレジットカードメッセージ
			if (Constants.CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED)
			{
				if (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
				{
					var creditError = new CreditErrorMessage();
					creditError.SetCreditErrorMessages(Constants.FILE_XML_ESCOTT_CREDIT_ERROR_MESSAGE);
					var errorList = creditError.GetValueItemArray();
					if (errorList.Any(s => s.Value == errorCode))
					{
						cardErrorMessageForPc = errorList.First(s => s.Value == errorCode).Text;
						this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR));
						this.ErrorMessages.Add(cardErrorMessageForPc);
					}
				}
			}
			if (string.IsNullOrEmpty(cardErrorMessageForPc)) this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
		}

		/// <summary>
		/// e-SCOTT向け成功ログ書き込み
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="order">受注</param>
		private void WriteSuccessLogByEScott(CartObject cart, Hashtable order)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				true,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.EScott,
				PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
				string.Empty,
				new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
						{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] }
					});
		}

		/// <summary>
		/// ベリトランスクレジット決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCardVeritrans(Hashtable order, CartObject cart, string successPaymentStatus)
		{
			this.TransactionName = "2-1-A.ベリトランスクレジット決済処理";
			var authSuccess = false;

			if ((this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
				&& (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				&& Constants.PAYMENT_VERITRANS4G_CREDIT_3DSECURE)
			{
				var paymentVeritrans3ds = new PaymentVeritransCredit3DS();
				var authResult3DS = paymentVeritrans3ds.Auth(order, cart);

				if (authResult3DS.Mstatus != VeriTransConst.RESULT_STATUS_OK)
				{
					SetVeriTransErrorMessage(authResult3DS.VResultCode);
					AppLogger.WriteInfo(
						OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart) + authResult3DS.MerrMsg);
					return false;
				}

				AddOrSetHastTable(order, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, successPaymentStatus);
				AddOrSetHastTable(order, Constants.FIELD_ORDER_CARD_TRAN_ID, authResult3DS.CustTxn);
				AddOrSetHastTable(order, VeriTransConst.RESPONSE_CONTENTS, authResult3DS.ResResponseContents);
				this.VeriTrans3DSecurePaymentOrders.Add(order);
				return true;
			}

			var paymentVeritrans = new PaymentVeritransCredit();
			if ((this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch)
				|| (this.OrderExecType == OrderRegisterBase.ExecTypes.CommerceManager)
				|| (cart.Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
			{
				var userCreditCard = new UserCreditCard(new UserCreditCardService().Get(
					(string)order[Constants.FIELD_ORDER_USER_ID],
					(int)order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]));

				// つくーるから会員IDが受け取れないため取り込んだ定期の子注文生成は再与信APIを使用する
				if ((userCreditCard.CooperationId == string.Empty) && (userCreditCard.CooperationId2 != string.Empty))
				{
					// 与信取得用の決済注文IDを取得する為(キャンセルは与信取得できているため含む)
					// ここで利用する決済注文IDはベリトランスにおける会員IDとして利用
					var orders = new OrderService().GetAllOrderRelateFixedPurchase((string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
					var previousOrder = orders.OrderByDescending(o => o.OrderDate)
						.First(o => o.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP
									&& o.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED);

					order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT] = cart.PriceTotal;
					order[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT] = cart.PriceTotal;
					order[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY] = previousOrder.SettlementCurrency;
					var orderModel = new OrderModel(order);

					//HACK:再与信を利用する都合上、orderModel.PaymentOrderIdに過去の決済注文IDを一時的にセットする
					var newPaymentOrderId = (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]; // 注文フローで作成されている決済注文ID
					orderModel.PaymentOrderId = previousOrder.PaymentOrderId;

					var responseReAuthorizeDto = paymentVeritrans.ReAuthorize(orderModel, userCreditCard, newPaymentOrderId);
					authSuccess = (responseReAuthorizeDto.Mstatus == VeriTransConst.RESULT_STATUS_OK);
					
					// orderModel.PaymentOrderIdを、与信取得したIDに変更する。orderModelに入った内容で更新がかかるため。
					orderModel.PaymentOrderId = newPaymentOrderId;

					return authSuccess;
				}
				else
				{
					var cardInfo = paymentVeritrans.GetCardInfo(cart.Payment.UserCreditCard.CooperationId);
					if (cardInfo.Mstatus != VeriTransConst.RESULT_STATUS_OK)
					{
						this.ErrorMessages.Add("登録済みカード情報がありません。");
						this.ErrorMessages.Add(cardInfo.MerrMsg);
						this.ApiErrorMessage = cardInfo.MerrMsg;
						AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(
							this.TransactionName,
							order,
							cart,
							string.Join("\r\n", this.ErrorMessages)));
						PaymentFileLogger.WritePaymentLog(
							false,
							cart.Payment.PaymentId,
							PaymentFileLogger.PaymentType.VeriTrans,
							PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
							OrderCommon.CreateOrderFailedLogMessage(
								this.TransactionName,
								order,
								cart,
								string.Join(",", this.ErrorMessages)),
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
								{Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]},
								{Constants.FIELD_USER_USER_ID, cart.CartUserId},
								{Constants.FIELD_ORDER_CREDIT_BRANCH_NO, cart.Payment.UserCreditCard.BranchNo.ToString()}
							});

						return false;
					}
				}

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					true,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.VeriTrans,
					PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
					"",
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
						{Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]},
						{Constants.FIELD_USER_USER_ID, cart.CartUserId},
						{Constants.FIELD_ORDER_CREDIT_BRANCH_NO, cart.Payment.UserCreditCard.BranchNo.ToString()}
					});

				var resultPayNowInfo = paymentVeritrans.UsePayNowId(order, cart);
				AddOrSetHastTable(order, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, successPaymentStatus);
				AddOrSetHastTable(order, Constants.FIELD_ORDER_CARD_TRAN_ID, resultPayNowInfo.CustTxn);
				authSuccess = AuthComplete(order, cart, successPaymentStatus, resultPayNowInfo);
				return authSuccess;
			}

			var responseAuthDto = paymentVeritrans.Auth(order, cart);
			authSuccess = AuthComplete(order, cart, successPaymentStatus, responseAuthDto);
			return authSuccess;
		}

		/// <summary>
		/// PAYGENTクレジット決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCardPaygent(Hashtable order, CartObject cart, string successPaymentStatus, UpdateHistoryAction updateHistoryAction)
		{
			this.TransactionName = "2-1-J.PAYGENTクレジット決済処理";

			var authParams = new PaygentCreditCardAuthApi();
			authParams.PaymentAmount = ((int)cart.PriceTotal).ToString();

			//3DSecure利用なし
			authParams.UseType = "";
			authParams.Skip3DSecure = "1";
			
			// 支払回数が1回：PaymentClass = 10, SplitCount = ""
			// 支払回数が1回以外：PaymentClass = 61, SplitCount = "paygent_installments"
			// 一括・分割払い以外は対応しない
			if (cart.Payment.CreditInstallmentsCode == "1")
			{
				authParams.PaymentClass = PaygentConstants.PAYGENT_PAYMENT_CLASS_FULL;
				authParams.SplitCount = "";
			}
			else
			{
				authParams.PaymentClass = PaygentConstants.PAYGENT_PAYMENT_CLASS_INSTALLMENTS;
				authParams.SplitCount = cart.Payment.CreditInstallmentsCode;
			}
			var selectPaymentMethod = Constants.DIGITAL_CONTENTS_OPTION_ENABLED && cart.HasDigitalContents
				? Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS
				: Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD;
			authParams.SalesMode = selectPaymentMethod == Constants.PaygentCreditCardPaymentMethod.Auth ? "0" : "1";
			
			// ペイジェントの顧客カードIDがあれば利用
			if (string.IsNullOrEmpty(cart.Payment.UserCreditCard.CooperationId2) == false)
			{
				authParams.StockCardMode = "1";
				authParams.CustomerId = cart.Payment.UserCreditCard.CooperationId;
				authParams.CustomerCardId = cart.Payment.UserCreditCard.CooperationId2;
				authParams.CardToken = string.Empty;
			}
			else
			{
				authParams.StockCardMode = "0";
				authParams.CardToken = cart.Payment.CreditToken.Token;
			}
			// 与信取得
			var result = PaygentApiFacade.SendRequest(authParams);

			// 与信取得に成功していたら各種ステータスを更新
			if ((string)result[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS)
			{
				// card_tran_idにペイジェント側payment_idを格納
				if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = (string)result["payment_id"];
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, (string)result["payment_id"]);
				}
				
				// ステータス更新処理
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();
					var orderService = new OrderService();

					var updated = orderService.UpdatePaygentOrder((string)order[Constants.FIELD_ORDER_ORDER_ID],
					"",
					(string)result["payment_id"],
					accessor);
					// 即時売上時のステータス更新
					PaygentUtility.UpdatePaymentStatus((string)order[Constants.FIELD_ORDER_ORDER_ID], accessor);
					accessor.CommitTransaction();
					
				}
			}
			else
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
			}
			return (string)result[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="order"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		private static void AddOrSetHastTable(Hashtable order, string key, string value)
		{
			if (order.ContainsKey(key))
			{
				order[key] = value;
			}
			else
			{
				order.Add(key, value);
			}
		}

		/// <summary>
		/// 与信済み後結果判定
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="successPaymentStatus">成功時の入金ステータス</param>
		/// <param name="authResult">与信レスポンス</param>
		/// <returns>与信結果</returns>
		private bool AuthComplete(
			Hashtable order,
			CartObject cart,
			string successPaymentStatus,
			CardAuthorizeResponseDto authResult)
		{
			if (authResult.Mstatus == VeriTransConst.RESULT_STATUS_OK)
			{
				if (order.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
				{
					order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, successPaymentStatus);
				}

				if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = authResult.CustTxn;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, authResult.CustTxn);
				}

				if (authResult.PayNowIdResponse == null) return true;
				if (authResult.PayNowIdResponse.Status == VeriTransConst.RESULT_STATUS_OK) return true;

				if (Constants.CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED)
				{
					var temp = authResult.VResultCode;
				}

				SetVeriTransErrorMessage(authResult.VResultCode);
				AppLogger.WriteInfo(
					OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart) + authResult.MerrMsg);
				return false;
			}
			else
			{
				SetVeriTransErrorMessage(authResult.VResultCode);
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart) + authResult.MerrMsg);
				return false;
			}
		}

		/// <summary>
		/// ベリトランスエラーメッセージ設定
		/// </summary>
		/// <param name="vResultCode">処理結果コード</param>
		private void SetVeriTransErrorMessage(string vResultCode)
		{
			var cardErrorMessageForPc = string.Empty;

			if (Constants.CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED
				&& (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc))
			{
				var errorCode = vResultCode.Substring(0, 4);
				var creditError = new CreditErrorMessage();
				creditError.SetCreditErrorMessages(Constants.FILE_XML_VERITRANS_CREDIT_ERROR_MESSAGE);
				var errorList = creditError.GetValueItemArray();

				if (errorList.Any(s => s.Value == errorCode))
				{
					cardErrorMessageForPc = errorList.First(s => s.Value == errorCode).Text;
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR));
					this.ErrorMessages.Add(cardErrorMessageForPc);
				}
			}

			if (string.IsNullOrEmpty(cardErrorMessageForPc))
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
			}
		}

		/// <summary>
		/// 電算システムコンビニ決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCvsDsk(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-2-C.電算システムコンビニ決済処理";

			PaymentDskCvs pdc = (PaymentDskCvs)cart.Payment.PaymentObject;

			var authResult = pdc.Auth(order, cart, true);
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				authResult,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Dsk,
				PaymentFileLogger.PaymentProcessingType.CvsPaymentProcessing,
				authResult ? "" : pdc.ResultMessageText,
				new Dictionary<string, string>
			{
					{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
				});

			if (authResult)
			{
				// 注文管理ID格納
				if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = pdc.ResultParam[PaymentDskCvs.RESULT_RESULT];
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, pdc.ResultParam[PaymentDskCvs.RESULT_RESULT]);
				}
				order.Add("payment_message_html", pdc.ResultMessageHtml);
				order.Add("payment_message_text", pdc.ResultMessageText);
				return true;
			}
			else
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSAUTH_ERROR));

				this.ApiErrorMessage = pdc.ResultMessageText;
				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart) + "エラー：" + pdc.ResultParam[PaymentDskCvs.RESULT_MESSAGE]);
				return false;
			}
		}

		/// <summary>
		/// SBPSコンビニ決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCvsSBPS(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-2-E.SBPSコンビニ決済処理";

			PaymentSBPSCvsOrderApi orderApi = new PaymentSBPSCvsOrderApi(
				XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + Constants.PATH_XML_CVS_SBPS_DSK));

			var execResult = orderApi.Exec(
				PaymentSBPSUtil.CreateCustCode(cart.CartUserId),
				cart.OrderId,
				Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
				Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
				new List<PaymentSBPSBase.ProductItem>(),
				cart.PriceTotal,
				cart.Owner.Name1,
				cart.Owner.Name2,
				cart.Owner.NameKana1,
				cart.Owner.NameKana2,
				cart.Owner.Zip1,
				cart.Owner.Zip2,
				cart.Owner.Addr1,
				cart.Owner.Addr2 + cart.Owner.Addr3,
				cart.Owner.Addr4,
				cart.Owner.Tel1_1 + cart.Owner.Tel1_2 + cart.Owner.Tel1_3,
				string.IsNullOrEmpty(cart.Owner.MailAddr) ? cart.Owner.MailAddr2 : cart.Owner.MailAddr, // PCメール優先
				DateTime.Now,
				cart.Payment.SBPSWebCvsType,
				Constants.PAYMENT_SETTING_SBPS_CVS_PAYMENT_LIMIT_DAY.HasValue
					? (DateTime?)DateTime.Now.AddDays(Constants.PAYMENT_SETTING_SBPS_CVS_PAYMENT_LIMIT_DAY.Value)
					: null);

			PaymentFileLogger.WritePaymentLog(
				execResult,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Sbps,
				PaymentFileLogger.PaymentProcessingType.CvsPaymentProcessing,
				execResult ? "" : LogCreator.CreateErrorMessage(
					orderApi.ResponseData.ResErrCode,
					orderApi.ResponseData.ResErrMessages),
				new Dictionary<string, string>
			{
					{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
				});

			if (execResult)
			{
				// 決済受付番号
				if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = orderApi.ResponseData.ResTrackingId;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, orderApi.ResponseData.ResTrackingId);
				}
				order.Add("payment_message_html", orderApi.ResponseData.GetCvsPaymentMessage(true, cart.Owner.DispLanguageLocaleId));
				order.Add("payment_message_text", orderApi.ResponseData.GetCvsPaymentMessage(false, cart.Owner.DispLanguageLocaleId));

				return true;
			}
			else
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(
					orderApi.ResponseData.ResErrCode,
					orderApi.ResponseData.ResErrMessages);

				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSAUTH_ERROR).Replace("@@ 1 @@", orderApi.ResponseData.ResErrMessages));
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
				return false;
			}
		}

		/// <summary>
		/// ヤマトKWCコンビニ決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCvsYamatoKwc(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-2-F.ヤマトKWCコンビニ決済処理";

			// 区分判定
			var deviceDiv = GetYamatoKwcDeviceDiv((string)order[Constants.FIELD_ORDER_ORDER_KBN]);

			// 各種コンビニ決済実行
			switch (cart.Payment.YamatoKwcCvsType)
			{
				case YamatoKwcFunctionDivCvs.B01:
					return ExecPaymentCvsYamatoKwcSevenEleven(order, cart, deviceDiv);

				case YamatoKwcFunctionDivCvs.B02:
					return ExecPaymentCvsYamatoKwcFamilyMart(order, cart, deviceDiv);

				case YamatoKwcFunctionDivCvs.B03:
				case YamatoKwcFunctionDivCvs.B04:
				case YamatoKwcFunctionDivCvs.B05:
				case YamatoKwcFunctionDivCvs.B06:
					return ExecPaymentCvsYamatoKwcEtc(order, cart, deviceDiv);

				default:
					throw new Exception("未対応のコンビニタイプ：" + cart.Payment.YamatoKwcCvsType);
			}
		}

		/// <summary>
		/// Gmo convenience exec
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCvsGmo(Hashtable order, CartObject cart)
		{
			if (this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch) return true;

			this.TransactionName = "２－２－Ｇ．コンビニ・GMO";

			// Get order id
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			// Create access convenience
			var paymentGMO = new PaymentGmoCvs();

			var entryTranResult = paymentGMO.EntryTran(paymentOrderId, cart.PriceTotal);

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				entryTranResult,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.CvsPaymentProcessing,
				entryTranResult ? "" : OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart, paymentGMO.ErrorMessages),
				new Dictionary<string, string>
			{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
				});

			if (entryTranResult == false)
			{
				this.ApiErrorMessage = paymentGMO.ErrorMessages;
				// Error log
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_GMO_CVSPREAUTH_ERROR));
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
				return false;
			}

			// Create payment convenience
			var result = paymentGMO.ExecTran(
				paymentOrderId,
				cart.Payment.GmoCvsType,
				cart.Owner.Name,
				cart.Owner.NameKana,
				cart.Owner.Tel1.Replace("-", string.Empty));

			PaymentFileLogger.WritePaymentLog(
				result,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.CvsPaymentProcessing,
				result
					? ""
					: OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart, this.ApiErrorMessage),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
				});

			if (result)
			{
				// Add data
				order[Constants.FIELD_ORDER_CARD_TRAN_ID] = string.Format("{0} {1}", paymentGMO.AccessId, paymentGMO.AccessPass);
				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

				var message = paymentGMO.CreateResultMessage(
					cart.Payment.GmoCvsType,
					cart.PriceTotal.ToString(),
					cart.Owner.DispLanguageLocaleId);
				order["payment_message_html"] = message.Item1;
				order["payment_message_text"] = message.Item2;
			}
			else
			{
				this.ApiErrorMessage = paymentGMO.ErrorMessages;
				// Error log
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_GMO_CVSPREAUTH_ERROR));
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
			}

			return result;
		}

		/// <summary>
		/// ヤマトKWCコンビニ決済処理（セブンイレブン）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="deviceDiv">デバイス区分</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCvsYamatoKwcSevenEleven(Hashtable order, CartObject cart, PaymentYamatoKwcDeviceDiv deviceDiv)
		{
			var mailAddress = (string.IsNullOrEmpty(cart.Owner.MailAddr) ? Constants.PAYMENT_SETTING_YAMATO_KWC_DUMMY_MAILADDRESS : cart.Owner.MailAddr);
			var result = new PaymentYamatoKwcCvs1AuthApi().Exec(
				deviceDiv,
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
				cart.PriceTotal,
				cart.Owner.Name,
				mailAddress);

			var isExecSuccess = result.Success;

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				isExecSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Yamatokwc,
				PaymentFileLogger.PaymentProcessingType.CvsPaymentProcessingBySevenEleven,
				isExecSuccess
					? ""
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						LogCreator.CreateErrorMessage(result.ErrorCode, result.ErrorMessage)),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
				});

			if (result.Success)
			{
				var messages = result.GetCvsMessage(
					XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.PATH_XML_CVS_YAMATOKWC)),
					cart.PriceTotal,
					cart.Owner.DispLanguageLocaleId);
				order.Add("payment_message_html", messages.Item1);
				order.Add("payment_message_text", messages.Item2);

				return true;
			}
			else
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(result.ErrorCode, result.ErrorMessage);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSAUTH_ERROR2).Replace("@@ 1 @@", result.ErrorInfoForLog));
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
				return false;
			}
		}

		/// <summary>
		/// ヤマトKWCコンビニ決済処理（ファミリーマート）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="deviceDiv">デバイス区分</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCvsYamatoKwcFamilyMart(Hashtable order, CartObject cart, PaymentYamatoKwcDeviceDiv deviceDiv)
		{
			var mailAddress = (string.IsNullOrEmpty(cart.Owner.MailAddr) ? Constants.PAYMENT_SETTING_YAMATO_KWC_DUMMY_MAILADDRESS : cart.Owner.MailAddr);
			var result = new PaymentYamatoKwcCvs2AuthApi().Exec(
				deviceDiv,
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
				cart.PriceTotal,
				cart.Owner.Name,
				StringUtility.ToZenkakuKatakana(cart.Owner.NameKana),
				cart.Owner.Tel1,
				mailAddress);

			// ログ格納処理
			var execResult = result.Success;
			PaymentFileLogger.WritePaymentLog(
				execResult,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Yamatokwc,
				PaymentFileLogger.PaymentProcessingType.CvsPaymentProcessingByFamilyMart,
				execResult ? "" : LogCreator.CreateErrorMessage(result.ErrorCode, result.ErrorMessage),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
				});

			if (result.Success)
			{
				var messages = result.GetCvsMessage(
					XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.PATH_XML_CVS_YAMATOKWC)),
					cart.PriceTotal,
					cart.Owner.DispLanguageLocaleId);
				order.Add("payment_message_html", messages.Item1);
				order.Add("payment_message_text", messages.Item2);
				return true;
			}
			else
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(result.ErrorCode, result.ErrorMessage);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSAUTH_ERROR2).Replace("@@ 1 @@", result.ErrorInfoForLog));
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
				return false;
			}
		}

		/// <summary>
		/// ヤマトKWCコンビニ決済処理（ローソン、サークルＫサンクス、ミニストップ、セイコーマート）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="deviceDiv">デバイス区分</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentCvsYamatoKwcEtc(Hashtable order, CartObject cart, PaymentYamatoKwcDeviceDiv deviceDiv)
		{
			var mailAddress = (string.IsNullOrEmpty(cart.Owner.MailAddr) ? Constants.PAYMENT_SETTING_YAMATO_KWC_DUMMY_MAILADDRESS : cart.Owner.MailAddr);
			var result = new PaymentYamatoKwcCvs3AuthApi(cart.Payment.YamatoKwcCvsType).Exec(
				deviceDiv,
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
				cart.PriceTotal,
				cart.Owner.Name,
				cart.Owner.Tel1,
				mailAddress);

			var execResult = result.Success;

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				execResult,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Yamatokwc,
				PaymentFileLogger.PaymentProcessingType.CvsPaymentProcessingByOtherCvs,
				execResult
					? ""
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						LogCreator.CreateErrorMessage(result.ErrorCode, result.ErrorMessage)),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
				});

			if (result.Success)
			{
				var messages = result.GetCvsMessage(
					XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.PATH_XML_CVS_YAMATOKWC)),
					cart.PriceTotal,
					cart.Owner.DispLanguageLocaleId);
				order.Add("payment_message_html", messages.Item1);
				order.Add("payment_message_text", messages.Item2);
				return true;
			}
			else
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(result.ErrorCode, result.ErrorMessage);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSAUTH_ERROR2).Replace("@@ 1 @@", result.ErrorInfoForLog));
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
				return false;
			}
		}

		/// <summary>
		/// ヤマトKWC デバイス区分取得
		/// </summary>
		/// <param name="orderKbn">注文区分</param>
		/// <returns>デバイス区分</returns>
		public PaymentYamatoKwcDeviceDiv GetYamatoKwcDeviceDiv(string orderKbn)
		{
			switch (orderKbn)
			{
				case Constants.FLG_ORDER_ORDER_KBN_MOBILE:
					return PaymentYamatoKwcDeviceDiv.Mobile;

				case Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE:
					return PaymentYamatoKwcDeviceDiv.SmartPhone;

				default:
					return PaymentYamatoKwcDeviceDiv.Pc;
			}
		}

		/// <summary>
		/// コンビニ後払い決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したか</returns>
		internal bool ExecPaymentCvsDef(Hashtable order, CartObject cart)
		{
			switch (Constants.PAYMENT_CVS_DEF_KBN)
			{
				// ２－５－Ａ．クロネコ後払い決済処理
				case Constants.PaymentCvsDef.YamatoKa:
					return ExecPaymentCvsDefYamato(order, cart);

				// ２－５－Ｂ．GMO後払い決済処理
				case Constants.PaymentCvsDef.Gmo:
					return ExecPaymentCvsDefGmo(order, cart);

				// ２－５－Ｃ．Atodene後払い決済処理
				case Constants.PaymentCvsDef.Atodene:
					return ExecPaymentCvsDefAtodene(order, cart);

				// ２－５－Ｄ．DSK後払い決済処理
				case Constants.PaymentCvsDef.Dsk:
					return ExecPaymentCvsDefDsk(order, cart);

				// ２－５－D．Atobaraicom後払い決済処理
				case Constants.PaymentCvsDef.Atobaraicom:
					return ExcePaymentCvsDefAtobaraicom(order, cart);

				// ２－５－E．Score後払い決済処理
				case Constants.PaymentCvsDef.Score:
					return ExecPaymentCvsDefScore(order, cart);

				// ２－５－F．ベリトランス後払い決済処理
				case Constants.PaymentCvsDef.Veritrans:
					return ExecPaymentCvsDefVeritrans(order, cart);
			}
			return true;	// 決済連動させない場合はここに来る
		}

		/// <summary>
		/// NP後払い決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したか</returns>
		internal bool ExecPaymentNpAfterPay(Hashtable order, CartObject cart)
		{
			return ExecNPAfterPay(order, cart);
		}

		/// <summary>
		/// クロネコ後払い決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したか</returns>
		protected bool ExecPaymentCvsDefYamato(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-5-A.クロネコ後払い決済処理";

			// 配送先
			var shipping = cart.GetShipping();

			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			var isSms = OrderCommon.CheckPaymentYamatoKaSms((string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);

			var entryApi = new PaymentYamatoKaEntryApi();
			var entryApiExecResult = (entryApi.Exec(
				paymentOrderId,
				DateTime.Now.ToString("yyyyMMdd"),
				PaymentYamatoKaUtility.CreateYamatoKaShipYmd(cart),
				cart.Owner.Name,
				StringUtility.ToHankakuKatakana(
					StringUtility.ToZenkakuKatakana(cart.Owner.NameKana)), // 全角カナにしてから半角カナへ変換
				cart.Owner.Zip,
				new PaymentYamatoKaAddress(cart.Owner.Addr1, cart.Owner.Addr2, cart.Owner.Addr3, cart.Owner.Addr4),
				cart.Owner.Tel1,
				cart.Owner.MailAddr,
				cart.PriceTotal,
				PaymentYamatoKaUtility.CreateSendDiv(isSms, shipping.AnotherShippingFlag),
				PaymentYamatoKaUtility.CreateProductItemList(order, cart),
				shipping.Name,
				shipping.Zip,
				new PaymentYamatoKaAddress(shipping.Addr1, shipping.Addr2, shipping.Addr3, shipping.Addr4),
				shipping.Tel1));

			if (entryApiExecResult == false)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = string.Format(
					"{0}:{1} ({2}) ",
					StringUtility.ToEmpty(entryApi.ResponseData.RequestDate),
					StringUtility.ToEmpty(entryApi.ResponseData.ErrorMessages),
					StringUtility.ToEmpty(entryApi.ResponseData.ErrorCode));
				this.ApiErrorMessage += (string.IsNullOrEmpty(entryApi.ResponseData.Result) == false)
					? string.Format(
						"\t[審査結果：{0}({1})]",
						entryApi.ResponseData.ResultDescription,
						entryApi.ResponseData.Result)
					: string.Empty;

				PaymentFileLogger.WritePaymentLog(
					false,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.KuronekoPostPay,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					this.ApiErrorMessage,
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
					});
				return false;
			}
			else
			{
				PaymentFileLogger.WritePaymentLog(
					true,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.KuronekoPostPay,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					"",
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
					});
			}

			// 成功したら注文情報に格納
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = paymentOrderId;
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			return true;
		}

		/// <summary>
		/// Gmo後払い決済処理
		/// </summary>
		/// <param name="order">注文ハッシュ</param>
		/// <param name="cart">カート</param>
		/// <returns>True;成功、False：失敗</returns>
		protected bool ExecPaymentCvsDefGmo(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-5-B.GMO後払い決済処理";

			var facade = new GmoDeferredApiFacade();
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			var request = new GmoRequestOrderRegister(cart);
			request.Buyer.ShopTransactionId = paymentOrderId;

			// HTTPヘッダ情報
			if (Constants.PAYMENT_SETTING_GMO_DEFERRED_ENABLE_HTTPHEADERS_POST
				&& order.ContainsKey("gmo_http_headers"))
			{
				request.HttpInfo.HttpHeader = StringUtility.ToEmpty(order["gmo_http_headers"]);
			}

			var result = facade.OrderRegister(request);

			if (result == null)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_GMO_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = string.Format("{0}：決済実施結果が取得できませんでした。", request.Buyer.ShopOrderDate);
				return false;
			}

			if (result.Result != ResultCode.OK)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_GMO_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = string.Format(
					"{0}:{1} ({2}) [審査結果:{3}]",
					request.Buyer.ShopOrderDate,
					result.Errors.Error[0].ErrorMessage,
					result.Errors.Error[0].ErrorCode,
					result.TransactionResult.AuthorResult);
				return false;
			}

			if (result.TransactionResult.IsResultNg)
			{
				// 与信NG
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_GMO_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = string.Format(
					"{0}: [審査結果:{1}]",
					request.Buyer.ShopOrderDate,
					result.TransactionResult.AuthorResult);
				return false;
			}
			else if (result.TransactionResult.IsResultExamination)
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

				// 取消しに失敗した場合は仮注文を残させるためにExceptionを投げる
				if (cancelResult.Result != ResultCode.OK)
				{
					throw new Exception(string.Format("GMO後払い与信保留の取消にて失敗。注文ID：{0}、取引ID：{1}", result.TransactionResult.ShopTransactionId, result.TransactionResult.GmoTransactionId));
				}

				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_GMO_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = string.Format(
					"{0}:[審査結果:{1}]",
					request.Buyer.ShopOrderDate,
					result.TransactionResult.AuthorResult);
				return false;
			}

			// Gmo取引IDを格納
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(result.TransactionResult.GmoTransactionId);
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			return true;
		}

		/// <summary>
		/// Atodene後払い決済処理
		/// </summary>
		/// <param name="order">受注情報</param>
		/// <param name="cart">カート</param>
		/// <returns>
		/// 処理結果
		/// True：成功
		/// False：失敗
		/// </returns>
		protected bool ExecPaymentCvsDefAtodene(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-5-C.Atodene後払い決済処理";
			var adp = new AtodeneTransactionCartAdapter(cart);
			var result = adp.Execute();

			if (result == null)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ATODENE_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = "決済実施結果が取得できませんでした。";
				return false;
			}

			if (result.Result != AtodeneConst.RESULT_OK)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ATODENE_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = string.Format(
					"{0} ({1}) [Atodene取引登録失敗]",
					result.Errors != null && result.Errors.Error != null && result.Errors.Error.Length > 0 ? result.Errors.Error[0].ErrorMessage : "",
					result.Errors != null && result.Errors.Error != null && result.Errors.Error.Length > 0 ? result.Errors.Error[0].ErrorCode : "");
				return false;
			}

			// 審査NG、または審査中についての処理
			if ((result.TransactionInfo.AutoAuthoriresult == AtodeneConst.AUTO_AUTH_RESULT_NG)
				|| (result.TransactionInfo.AutoAuthoriresult == AtodeneConst.AUTO_AUTH_RESULT_PROCESSING))
			{
				var isAuthProcessing = (result.TransactionInfo.AutoAuthoriresult == AtodeneConst.AUTO_AUTH_RESULT_PROCESSING);

				// リアルタイムで与信結果が取得できなかった場合は取り消す
				var cancelAdp = new AtodeneCancelTransactionAdapter(result.TransactionInfo.TransactionId);
				var cancelResult = cancelAdp.ExecuteCancel();

				// 取消しに失敗した場合は仮注文を残させるためにExceptionを投げる
				if (cancelResult.Result != AtodeneConst.RESULT_OK)
				{
					throw new Exception(
						string.Format(
							"Atodene後払い与信{0}の取消にて失敗。注文ID：{1}、取引ID：{2}",
							isAuthProcessing ? "保留" : "失敗",
							cart.OrderId,
							result.TransactionInfo.TransactionId));
				}

				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ATODENE_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = isAuthProcessing ? "[審査結果:審査中]" : "[審査結果：NG]";
				return false;
			}

			// Atodene問い合わせIDを格納
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(result.TransactionInfo.TransactionId);
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = StringUtility.ToEmpty(result.TransactionInfo.ShopOrderId);

			return true;
		}

		/// <summary>
		/// DSK後払い決済処理
		/// </summary>
		/// <param name="order">注文ハッシュ</param>
		/// <param name="cart">カート</param>
		/// <returns>True;成功、False：失敗</returns>
		protected bool ExecPaymentCvsDefDsk(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-5-D.DSK後払い決済処理";

			var adapter = new DskDeferredOrderRegisterCartAdapter(cart);
			var result = adapter.Execute();

			if (result == null)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = "決済実施結果が取得できませんでした。";
				return false;
			}

			if (result.IsResultOk == false)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = string.Format(
					"{0} ({1}) [DSK後払い取引登録失敗]",
					result.Errors.Error[0].ErrorMessage,
					result.Errors.Error[0].ErrorCode);
				return false;
			}

			if (result.TransactionResult.AuthorResult == DskDeferredConst.AUTO_AUTH_RESULT_NG)
			{
				var cancelAdapter = new DskDeferredOrderCancelAdapter(result.TransactionResult.TransactionId,
					adapter.Request.Buyer.ShopTransactionId, adapter.Request.Buyer.BilledAmount.ToPriceString());
				var cancelResult = cancelAdapter.Execute();

				if (cancelResult.Result != DskDeferredConst.RESULT_OK)
				{
					throw new Exception(
						string.Format(
							"DSK後払い与信失敗の取消にて失敗。注文ID：{0}、取引ID：{1}",
							cart.OrderId,
							result.TransactionResult.TransactionId));
				}

				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = "[審査結果：NG]";
				return false;
			}

			if (result.TransactionResult.AuthorResult == DskDeferredConst.AUTO_AUTH_RESULT_HOLD)
			{
				this.IsAuthResultHold = true;
			}

			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(result.TransactionResult.TransactionId);
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = StringUtility.ToEmpty(adapter.Request.Buyer.ShopTransactionId);

			return true;
		}

		/// <summary>
		/// Amazonペイメント
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したかどうか</returns>
		protected bool ExecAmazonPayment(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-6-A.Amazonペイメント決済処理";

			var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);

			// カートに定期台帳情報がセットされている場合定期台帳からの注文生成とみなす
			var amazonBillingAgreementId = (cart.FixedPurchase == null) ? cart.ExternalPaymentAgreementId : cart.FixedPurchase.ExternalPaymentAgreementId;
			var amazonOrderReferenceId = cart.AmazonOrderReferenceId;

			if (string.IsNullOrEmpty(amazonBillingAgreementId) == false)
			{
				if (this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
				{
					// 支払契約を設定
					var sbad = AmazonApiFacade.SetBillingAgreementDetails(amazonBillingAgreementId);

					var isSbadSuccess = sbad.GetSuccess() && (sbad.GetConstraintIdList().Any() == false);

					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						isSbadSuccess,
						cart.Payment.PaymentId,
						PaymentFileLogger.PaymentType.Amazon,
						PaymentFileLogger.PaymentProcessingType.PaymentContractSetting,
						isSbadSuccess ? "" : OrderCommon.CreateOrderFailedLogMessage(
							this.TransactionName,
							order,
							cart,
							LogCreator.CreateErrorMessage(sbad.GetErrorCode(), sbad.GetErrorMessage())),
						new Dictionary<string, string>
						{
							{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
							{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
							{Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId}
						});

					if (isSbadSuccess == false)
					{
						this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
						this.ApiErrorMessage = LogCreator.CreateErrorMessage(sbad.GetErrorCode(), sbad.GetErrorCode());
						AppLogger.WriteInfo(
							OrderCommon.CreateOrderFailedLogMessage(
								this.TransactionName,
								order,
								cart,
								LogCreator.CreateErrorMessage(sbad.GetErrorCode(), sbad.GetErrorMessage())));
						return false;
					}
				}

				// 支払契約の承認
				var cba = AmazonApiFacade.ConfirmBillingAgreement(amazonBillingAgreementId);
				var isCbaSuccess = cba.GetSuccess();

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					isCbaSuccess,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Amazon,
					PaymentFileLogger.PaymentProcessingType.PaymentContractApproval,
					isCbaSuccess ? "" : OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						LogCreator.CreateErrorMessage(cba.GetErrorCode(), cba.GetErrorMessage())),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
						{Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId}
					});

				if (isCbaSuccess == false)
				{
					this.ApiErrorMessage = LogCreator.CreateErrorMessage(cba.GetErrorCode(), cba.GetErrorMessage());
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
					AppLogger.WriteInfo(
						OrderCommon.CreateOrderFailedLogMessage(
							this.TransactionName,
							order,
							cart,
							LogCreator.CreateErrorMessage(cba.GetErrorCode(), cba.GetErrorMessage())));
					return false;
				}

				// 注文生成
				var corfi = AmazonApiFacade.CreateOrderReferenceForId(amazonBillingAgreementId, cart.PriceTotal, orderId);
				var isCorfiSuccess = corfi.GetSuccess() && (corfi.GetConstraintIdList().Any() == false);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					isCorfiSuccess,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Amazon,
					PaymentFileLogger.PaymentProcessingType.OrderGeneration,
					isCorfiSuccess ? "" : OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						LogCreator.CreateErrorMessage(corfi.GetErrorCode(), corfi.GetErrorMessage())),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
						{Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId}
					});

				if (isCorfiSuccess == false)
				{
					this.ApiErrorMessage = LogCreator.CreateErrorMessage(corfi.GetErrorCode(), corfi.GetErrorMessage());
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
					return false;
				}
				amazonOrderReferenceId = corfi.GetAmazonOrderReferenceId();
			}
			else
			{
				// 注文情報を設定
				var set = AmazonApiFacade.SetOrderReferenceDetails(amazonOrderReferenceId, cart.PriceTotal, orderId);
				var isSetSuccess = set.GetSuccess() && (set.GetConstraintIdList().Any() == false);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					isSetSuccess,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Amazon,
					PaymentFileLogger.PaymentProcessingType.OrderInfoSetting,
					isSetSuccess ? "" : OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						LogCreator.CreateErrorMessage(set.GetErrorCode(), set.GetErrorMessage())),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
						{Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId}
					});

				if (isSetSuccess == false)
				{
					this.ApiErrorMessage = LogCreator.CreateErrorMessage(set.GetErrorCode(), set.GetErrorMessage());
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
					return false;
				}
			}

			// 注文情報の承認
			var con = AmazonApiFacade.ConfirmOrderReference(amazonOrderReferenceId);
			var isConSuccess = con.GetSuccess();

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				isConSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Amazon,
				PaymentFileLogger.PaymentProcessingType.OrderInfoApproval,
				isConSuccess
					? ""
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						LogCreator.CreateErrorMessage(con.GetErrorCode(), con.GetErrorMessage())),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
					{Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId}
				});

			if (isConSuccess == false)
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(con.GetErrorCode(), con.GetErrorMessage());
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
				return false;
			}

			// オーソリ処理
			var aut = AmazonApiFacade.Authorize(
				amazonOrderReferenceId,
				cart.PriceTotal,
				orderId + "_" + DateTime.Now.ToString("HHmmssfff"),
				Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW);

			var isAutSuccess = aut.GetSuccess();
			var isAutStateDeclined = (aut.GetAuthorizationState() == AmazonConstants.State.Declined.ToString());
			var errorMessage = (string.IsNullOrEmpty(aut.GetErrorMessage()) == false)
				? LogCreator.CreateErrorMessage(aut.GetErrorCode(), aut.GetErrorMessage())
				: "何らかの理由により与信が正常に終了しませんでした。";

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				(isAutSuccess && (isAutStateDeclined == false)),
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Amazon,
				PaymentFileLogger.PaymentProcessingType.OthoriProcessing,
				(isAutSuccess && (isAutStateDeclined == false))
					? ""
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						errorMessage),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
					{ Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId }
				});

			if (isAutStateDeclined)
			{
				var cor = AmazonApiFacade.CancelOrderReference(
					amazonOrderReferenceId,
					string.Format("State : {0}", AmazonConstants.State.Declined.ToString()));
				var isCorSuccess = cor.GetSuccess();
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					isCorSuccess,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Amazon,
					PaymentFileLogger.PaymentProcessingType.OrderCancel,
					isCorSuccess
						? ""
						: OrderCommon.CreateOrderFailedLogMessage(
							this.TransactionName,
							order,
							cart,
							LogCreator.CreateErrorMessage(cor.GetErrorCode(), cor.GetErrorMessage())),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
						{Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId}
					});

				this.ApiErrorMessage = errorMessage;
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
				return false;
			}

			if (isAutSuccess == false)
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(aut.GetErrorCode(), aut.GetErrorMessage());
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
				return false;
			}

			// 即時決済の場合
			if (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW)
			{
				var autResult = (aut.GetCaptureIdList().Count != 0);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					autResult,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Amazon,
					PaymentFileLogger.PaymentProcessingType.ImmediateSettlement,
					autResult
						? ""
						: OrderCommon.CreateOrderFailedLogMessage(
							this.TransactionName,
							order,
							cart,
							"何らかの理由により与信後の即時売上が正常に終了しませんでした。\t" + LogCreator.CreateErrorMessage(
								aut.GetErrorCode(),
								aut.GetErrorMessage())),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]},
						{Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId}
					});

				if (autResult == false)
				{
					this.ApiErrorMessage = LogCreator.CreateErrorMessage(aut.GetErrorCode(), aut.GetErrorMessage());
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
					return false;
				}

				if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = aut.GetCaptureIdList()[0];
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, aut.GetCaptureIdList()[0]);
				}
				if (order.ContainsKey(Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS))
				{
					order[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS] = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS, Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED);
				}
				if (order.ContainsKey(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS))
				{
					order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP);
				}
			}
			// 即時決済でない場合
			else
			{
				if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = aut.GetAuthorizationId();
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, aut.GetAuthorizationId());
				}
			}

			var orderPaymentStatus = Constants.PAYMENT_AMAZON_PAYMENTSTATUSCOMPLETE
				? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
				: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
			// 入金ステータスを格納
			if (order.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
			{
				order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = orderPaymentStatus;
			}
			else
			{
				order.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, orderPaymentStatus);
			}

			// Amazon決済IDをセット
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = amazonOrderReferenceId;

			return true;
		}

		/// <summary>
		/// Paypal決済
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したかどうか</returns>
		protected bool ExecPaypalPayment(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-7-A.Paypal決済処理";

			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			var deviceData = ((this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
					&& (cart.PayPalCooperationInfo != null))
				? cart.PayPalCooperationInfo.DeviceData
				: string.Empty;

			var result = PayPalUtility.Payment.PayWithCustomerId(
				paymentOrderId,
				cart.Payment.UserCreditCard.CooperationInfo.PayPalCustomerId,
				deviceData,
				cart.PriceTotal,
				cart.HasDigitalContents || (Constants.PAYPAL_PAYMENT_METHOD == Constants.PayPalPaymentMethod.AUTH_WITH_SUBMIT),
				new PayPalUtility.AddressRequestWrapper(cart));
			var isSuccess = result.IsSuccess();	// デバッグで変更できるようにいったん変数に格納しておく

			PaymentFileLogger.WritePaymentLog(
				isSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.PayPal,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				isSuccess
					? ""
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						String.Format("PayPal処理に失敗しました。：" + result.Message + " " + result.Errors)),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
					{
						Constants.FOR_LOG_KEY_FOR_PAYPAL_CUSTOMER_ID,
						cart.Payment.UserCreditCard.CooperationInfo.PayPalCustomerId
					}
				});

			if (isSuccess)
			{
				// 決済取引IDにamazon決済IDをセット
				order[Constants.FIELD_ORDER_CARD_TRAN_ID] = result.Target.Id;
			}
			else
			{
				this.ApiErrorMessage = string.Join("\t", result.Errors);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR));
				return false;
			}
			return true;
		}

		/// <summary>
		/// Amazonペイメント(CV2)定期子注文作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したかどうか</returns>
		protected bool ExecAmazonPaymentCv2(Hashtable order, CartObject cart)
		{
			// 以下定期の子注文作成用、通常注文および定期初回注文はOrderAmazonCompleteで決済処理実施
			this.TransactionName = "2-6-B.Amazonペイメント(CV2)決済処理";

			var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);

			var amazonChargePermissionId = (cart.FixedPurchase != null)
				? cart.FixedPurchase.ExternalPaymentAgreementId
				: string.Empty;

			var chargeResponse = new ChargeResponse();
			var isSuccess = false;
			var error = new AmazonResponseError();

			if (string.IsNullOrEmpty(amazonChargePermissionId) == false)
			{
				var facade = new AmazonCv2ApiFacade();
				facade.UpdateChargePermissionOrderId(amazonChargePermissionId, orderId);
				chargeResponse = facade.CreateCharge(amazonChargePermissionId, cart.PriceTotal, orderId);
				isSuccess = chargeResponse.Success;
				error = AmazonCv2ApiFacade.GetErrorCodeAndMessage(chargeResponse);
			}

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				isSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.AmazonCv2,
				PaymentFileLogger.PaymentProcessingType.OrderGeneration,
				isSuccess
					? string.Empty
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						LogCreator.CreateErrorMessage(error.ReasonCode, error.Message)),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, amazonChargePermissionId },
					{ Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId }
				});

			if (isSuccess == false)
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(error.ReasonCode, error.Message);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CHANGE_TO_ANOTHER_PAYMENT_FOR_AUTH_ERROR));
				return false;
			}

			// 即時決済の場合
			if (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW)
			{
				var chargeResult = (chargeResponse.StatusDetails.State == AmazonCv2Constants.FLG_CHARGE_STATUS_CAPTURED);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					chargeResult,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Amazon,
					PaymentFileLogger.PaymentProcessingType.ImmediateSettlement,
					chargeResult
						? ""
						: OrderCommon.CreateOrderFailedLogMessage(
							this.TransactionName,
							order,
							cart,
							"何らかの理由により与信後の即時売上が正常に終了しませんでした。\t" + LogCreator.CreateErrorMessage(
								error.ReasonCode,
								error.Message)),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, amazonChargePermissionId },
						{ Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId }
					});

				order[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS] = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
				order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
			}

			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = chargeResponse.ChargeId;

			// 入金ステータスを格納
			order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] =
				Constants.PAYMENT_AMAZON_PAYMENTSTATUSCOMPLETE
					? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
					: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
			// Amazon決済IDをセット
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = amazonChargePermissionId;

			return true;
		}

		/// <summary>
		/// 後付款(TriLink後払い)決済処理 [フロント用]
		/// </summary>
		/// <param name="order">注文ハッシュ</param>
		/// <param name="cart">カート</param>
		/// <returns>True;成功、False：失敗</returns>
		protected bool ExecTriLinkAfterPayForFront(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-8-A.後付款(TriLink後払い)決済処理 フロント";

			// アクセストークン発行
			var triLinkAfterPayAccessToken = TriLinkAfterPayApiFacade.AccessToken(new TriLinkAfterPayAccessTokenRequest());
			if (string.IsNullOrEmpty(triLinkAfterPayAccessToken)) return false;

			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			// 注文審査
			var authResponse = TriLinkAfterPayApiFacade.Authorization(
				new TriLinkAfterPayAuthRequest(cart, triLinkAfterPayAccessToken));
			if (authResponse.ResponseResult == false)
			{
				var errorCode = string.Empty;
				var errorMessage = string.Empty;
				if (authResponse.IsHttpStatusCodeBadRequest)
				{
					errorCode = authResponse.Errors.FirstOrDefault().ErrorCode;
					errorMessage = authResponse.Errors.FirstOrDefault().Message;
				}
				else if (authResponse.IsHttpStatusCodeUnauthorized)
				{
					errorCode = authResponse.ErrorCode;
					errorMessage = authResponse.Message;
				}

				this.ApiErrorMessage = LogCreator.CreateErrorMessage(errorCode, errorMessage);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR));

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.TriLink,
					PaymentFileLogger.PaymentProcessingType.OrderReviewForFornt,
					this.ApiErrorMessage,
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, paymentOrderId},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
					});
				return false;
			}

			// 注文確定依頼
			var commitResponse = TriLinkAfterPayApiFacade.Commit(
				new TriLinkAfterPayCommitRequest(authResponse.AcceptNumber, paymentOrderId));
			if (commitResponse.IsHttpStatusCodeOK == false)
			{
				var errorCode = string.Empty;
				var errorMessage = string.Empty;
				if (commitResponse.IsHttpStatusCodeBadRequest)
				{
					errorCode = authResponse.Errors.FirstOrDefault().ErrorCode;
					errorMessage = authResponse.Errors.FirstOrDefault().Message;
				}
				else if (commitResponse.IsHttpStatusCodeUnauthorized)
				{
					errorCode = authResponse.ErrorCode;
					errorMessage = authResponse.Message;
				}

				this.ApiErrorMessage = LogCreator.CreateErrorMessage(errorCode, errorMessage);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR));

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.TriLink,
					PaymentFileLogger.PaymentProcessingType.OrderConfirmationRequestForFront,
					this.ApiErrorMessage,
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, paymentOrderId},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
					});
				return false;
			}
			else
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.TriLink,
					PaymentFileLogger.PaymentProcessingType.OrderConfirmationRequestForFront,
					"",
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, paymentOrderId},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
					});
			}

			// 後付款(TriLink後払い)決済注文IDを格納
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			return true;
		}

		/// <summary>
		/// 後付款(TriLink後払い)決済処理 [管理画面・バッチ用]
		/// </summary>
		/// <param name="order">注文ハッシュ</param>
		/// <param name="cart">カート</param>
		/// <returns>True;成功、False：失敗</returns>
		protected bool ExecTriLinkAfterPayForCommerce(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-8-B.後付款(TriLink後払い)決済処理 管理画面";

			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			// 注文審査
			var authRequest = new TriLinkAfterPayRegisterRequest(cart, paymentOrderId);
			var authResponse = TriLinkAfterPayApiFacade.RegisterOrder(authRequest);
			if ((authResponse.IsHttpStatusCodeCreated)
				&& (authResponse.Authorization.Result == TriLinkAfterPayConstants.FLG_TW_AFTERPAY_AUTH_OK))
			{
				// 後付款(TriLink後払い)決済注文IDを格納
				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;
				// 後付款(TriLink後払い)決済取引IDを格納
				order[Constants.FIELD_ORDER_CARD_TRAN_ID] = authResponse.OrderCode;
			}
			else
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(authResponse.ErrorCode, authResponse.Message);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR));
				return false;
			}

			return true;
		}

		/// <summary>
		/// LINE Pay ペイメント（定期）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したかどうか</returns>
		protected bool ExecLinePayment(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-7-A.LINEPay決済処理";

			var service = new UserService();
			var success = true;
			var userExtend = service.GetUserExtend((string)order[Constants.FIELD_ORDER_USER_ID]);

			// Call API if pre-approved LINE Pay payment accept
			if (cart.IsPreApprovedLinePayPayment == false) return false;

			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			var regKey = ((userExtend != null)
				? (string)userExtend.DataSource[Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY]
				: string.Empty);
			
			var response = LinePayApiFacade.PreapprovedPayment(
				regKey,
				string.Join(",", cart.Items.Select(product => product.ProductJointName)),
				cart.SettlementAmount,
				cart.SettlementCurrency,
				paymentOrderId,
				Constants.PAYMENT_LINEPAY_PAYMENTCAPTURENOW,
				new LinePayApiFacade.LinePayLogInfo((string)order[Constants.FIELD_ORDER_ORDER_ID], paymentOrderId, ""));
			if (response.IsSuccess == false)
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(response.ReturnCode, response.ReturnMessage);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR));
				success = false;
			}

			if (success == false) return false;

			// Set data and update regkey for user extend
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = response.Info.TransactionId;
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			return true;
		}

		/// <summary>
		/// Exec NP After Pay
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>True;成功、False：失敗</returns>
		protected bool ExecNPAfterPay(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-9-A.NPAfterPay決済処理";
			// Create payment data for new order
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			var requestRegist = NPAfterPayUtility.CreateOrderRequestData(cart, order, paymentOrderId);
			var resultRegist = NPAfterPayApiFacade.RegistOrder(requestRegist);
			var isOrderExecTypePc = ((this.OrderExecType == OrderRegisterBase.ExecTypes.Pc)
				|| ((this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch)
					 && (this.LastChanged == Constants.FLG_LASTCHANGED_USER)));
			var success = false;
			var formattedErrorMessage = string.Empty;
			if (resultRegist.IsSuccess)
			{
				if (resultRegist.IsAuthoriReviewOk)
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = resultRegist.GetNPTransactionId();
					order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;
					success = true;
				}
				else
				{
					var apiAuthoriErrorMessage = resultRegist.GetApiErrorMessage(isOrderExecTypePc);
					if (resultRegist.IsAuthoriReviewPending)
					{
						apiAuthoriErrorMessage = apiAuthoriErrorMessage
							.Replace("@@ 1 @@", cart.Owner.ConcatenateAddressWithoutCountryName())
							.Replace("@@ 2 @@", cart.Shippings.First().ConcatenateAddressWithoutCountryName());
					}
					var errorMessageForDisplay = (isOrderExecTypePc
							&& resultRegist.IsAuthoriBeforeReviewOrReviewInProcess)
						? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR)
						: apiAuthoriErrorMessage;
					this.ErrorMessages.Add(errorMessageForDisplay);
					var requestCancel = NPAfterPayUtility.CreateCancelOrGetPaymentRequestData(resultRegist.GetNPTransactionId());
					var resultCancel = NPAfterPayApiFacade.CancelOrder(requestCancel);
					if (resultCancel.IsSuccess == false)
					{
						formattedErrorMessage = resultCancel.GetApiErrorMessage(isOrderExecTypePc);
						var apiErrorMessageTemp = string.Format("{0}\r\n{1}",
							apiAuthoriErrorMessage,
							formattedErrorMessage);
						this.ApiErrorMessage = apiErrorMessageTemp;
					}
					else
					{
						this.ApiErrorMessage = apiAuthoriErrorMessage;
					}
				}
			}
			else
			{
				formattedErrorMessage = resultRegist.GetApiErrorMessage(isOrderExecTypePc);
				this.ApiErrorMessage = formattedErrorMessage;
				this.ErrorMessages.Add(formattedErrorMessage);
			}

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				success,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.NpAfterPay,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				success
					? string.Empty
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						this.ApiErrorMessage),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]) }
				});
			return success;
		}
		#endregion

		/// <summary>
		/// 決済連携メモ作成
		/// </summary>
		/// <param name="order"></param>
		private void CreatePaymentMemo(Hashtable order)
		{
			this.TransactionName = "2-S.決済連携メモ作成処理";

			// 管理画面入力などですでにある場合は追記する
			StringBuilder paymentMemo = new StringBuilder();
			if (order.ContainsKey(Constants.FIELD_ORDER_PAYMENT_MEMO))
			{
				paymentMemo.Append((string)order[Constants.FIELD_ORDER_PAYMENT_MEMO] + "\r\n\r\n");
			}

			// Append payment memo by payment type
			if (order.ContainsKey("payment_message_text"))
			{
				switch ((string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN])
				{
					case Constants.FLG_PAYMENT_PAYMENT_ID_ATM:
						paymentMemo.Append("[ATM決済情報]\r\n");
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET:
						paymentMemo.Append("[ネットバンキング決済情報]\r\n");
						break;

					default:
						paymentMemo.Append("[コンビニ決済情報]\r\n");
						break;
				}
				paymentMemo.Append((string)order["payment_message_text"]);
			}

			order[Constants.FIELD_ORDER_PAYMENT_MEMO] = paymentMemo.ToString().Trim();
		}

		/// <summary>
		/// Paidyペイメント
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したかどうか</returns>
		protected bool ExecPaidyPayment(Hashtable order, CartObject cart)
		{
			switch (Constants.PAYMENT_PAIDY_KBN)
			{
				case Constants.PaymentPaidyKbn.Direct:
					return ExecPaidyDirectPayment(order, cart);

				case Constants.PaymentPaidyKbn.Paygent:
					return ExecPaidyPaygentPayment(order, cart);
			}
			return true;
		}

		/// <summary>
		/// Execute paidy direct payment
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>True if success, otherwise false</returns>
		protected bool ExecPaidyDirectPayment(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-7-A.Paidy決済処理";

			// Create new Paidy payment for new order
			var paymentObject = PaidyUtility.CreatePaidyPaymentObject(cart, order);
			var response = PaidyPaymentApiFacade.CreateTokenPayment(paymentObject);

			var success = ((response.HasError == false) && (paymentObject.Amount == response.Payment.Amount));
			var errorMessage = string.Empty;

			if (success)
			{
				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = response.Payment.Id;
			}
			else
			{
				this.ApiErrorMessage = response.GetApiErrorMessages();
				errorMessage = OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					order,
					cart,
					this.ApiErrorMessage);

				this.ErrorMessages.Add(CommerceMessages.GetMessages(
					response.IsRejectedPayment
						? CommerceMessages.ERRMSG_PAIDY_AUTHORIZE_ERROR
						: CommerceMessages.ERRMSG_FRONT_AUTH_ERROR + "<br />" + response.GetApiErrorCodeAndMessages()));

				cart.Payment.IsRejectedPayment = response.IsRejectedPayment;
				cart.Payment.PaidyToken = String.Empty;
			}

			if (string.IsNullOrEmpty(errorMessage) == false) FileLogger.Write("PaidyError", errorMessage);

			var idDictionary = new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) },
			};

			PaymentFileLogger.WritePaymentLog(
				success,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.PaygentEng,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				errorMessage,
				idDictionary);

			return success;
		}

		/// <summary>
		/// Execute paidy paygent payment
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>True if success, otherwise false</returns>
		protected bool ExecPaidyPaygentPayment(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-7-B.Paidy決済処理";
			var paidyPaymentId = cart.Payment.CardTranId;
			var paidyAuthorizationResult = new PaygentApiFacade().PaidyAuthorize(paidyPaymentId);
			var success = paidyAuthorizationResult.IsSuccess;
			var errorMessage = string.Empty;

			if (success)
			{
				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = cart.Payment.CardTranId;
			}
			else
			{
				errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED);
				this.ApiErrorMessage = errorMessage;
				this.ErrorMessages.Add(errorMessage);
				FileLogger.Write("PaidyError", errorMessage);
			}

			var dictionary = new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) },
				{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]) },
			};

			PaymentFileLogger.WritePaymentLog(
				success,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Paidy,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				errorMessage,
				dictionary);

			return success;
		}

		/// <summary>
		/// Execute Payment DskDef
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		private bool ExecPaymentDskDef(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-8. DSKコンビニ後払い";
			var orderId = (string)order[Constants.FIELD_ORDER_ORDER_ID];

			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = (orderId.Length >= 10)
				? orderId.Substring(orderId.Length - 10)
				: orderId.PadRight(10, '0');

			PaymentFileLogger.WritePaymentLog(
				true,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Dsk,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				"",
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
				});
			return true;
		}

		/// <summary>
		/// ソフトバンク・ワイモバイルまとめて支払い(SBPS)（継続課金（定期・従量））
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したかどうか</returns>
		protected bool ExecSoftbankKetaiPayment(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-9.ソフトバンク・ワイモバイルまとめて支払い(SBPS)（継続課金（定期・従量））";
			var result = ExecPaymentContinuousOrder(order, cart, Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM);
			return result;
		}

		/// <summary>
		/// auかんたん決済(SBPS)（継続課金（定期・従量））
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したかどうか</returns>
		protected bool ExecAuKantanPayment(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-10.auかんたん決済(SBPS)（継続課金（定期・従量））";
			var result = ExecPaymentContinuousOrder(order, cart, Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM);
			return result;
		}

		/// <summary>
		/// ドコモケータイ払い(SBPS)（継続課金（定期・従量））
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したかどうか</returns>
		protected bool ExecDocomoKetaiPayment(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-11.ドコモケータイ払い(SBPS)（継続課金（定期・従量））";
			var result = ExecPaymentContinuousOrder(order, cart, Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE);
			return result;
		}

		/// <summary>
		/// 継続課金の購入要求処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="orderPaymentStatus">入金ステータス</param>
		/// <returns>実行結果</returns>
		private bool ExecPaymentContinuousOrder(Hashtable order, CartObject cart, string orderPaymentStatus)
		{
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			var trackingId = (cart.FixedPurchase != null)
				? cart.FixedPurchase.ExternalPaymentAgreementId
				: string.Empty;
			var custCode = PaymentSBPSUtil.CreateCustCode(cart.OrderUserId);
			var result = true;
			var resTrackingId = "";
			var errorMessage = "";
			this.ApiErrorMessage = "";

			switch (cart.Payment.PaymentId)
			{
				// SBPS ソフトバンク・ワイモバイルまとめて支払い
				case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
					result = ExecutePaymentSoftbankKetaiApi(
						trackingId,
						custCode,
						paymentOrderId,
						cart.SendingAmount,
						out resTrackingId,
						out errorMessage);
					break;

				// SBPS auかんたん決済
				case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
					result = ExecutePaymentAuKantanApi(
						trackingId,
						custCode,
						paymentOrderId,
						cart.SendingAmount,
						out resTrackingId,
						out errorMessage);
					break;

				// SBPSドコモケータイ支払い
				case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
					result = ExecutePaymentDocomoKetaiApi(
						trackingId,
						custCode,
						paymentOrderId,
						cart.SendingAmount,
						out resTrackingId,
						out errorMessage);
					break;
			}

			if (result == false)
			{
				this.ApiErrorMessage = errorMessage;
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR));
				return false;
			}

			// 決済トラキングIDを格納
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = resTrackingId;
			// 決済注文IDを格納
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;
			// 入金ステータスを格納
			order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = orderPaymentStatus;

			return true;
		}

		/// <summary>
		/// SBPS auかんたん決済での継続課金購入要求
		/// </summary>
		/// <param name="trackingId">処理対象トラキングID</param>
		/// <param name="custCode">顧客ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="amount">注文金額</param>
		/// <param name="resTrackingId">処理トラキングID</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>実行結果</returns>
		private bool ExecutePaymentAuKantanApi(
			string trackingId,
			string custCode,
			string paymentOrderId,
			decimal amount,
			out string resTrackingId,
			out string errorMessage)
		{
			errorMessage = "";
			var api = new PaymentSBPSCareerAuKantanContinuousOrderApi();
			var result = api.Exec(
				trackingId,
				custCode,
				paymentOrderId,
				Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
				Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
				amount);
			if (result == false)
			{
				errorMessage = LogCreator.CreateErrorMessage(
					api.ResponseData.ResErrCode,
					api.ResponseData.ResErrMessages);
			}
			resTrackingId = api.ResponseData.ResTrackingId;

			return result;
		}

		/// <summary>
		/// SBPS ドコモケータイ支払いでの継続課金購入要求
		/// </summary>
		/// <param name="trackingId">処理対象トラキングID</param>
		/// <param name="custCode">顧客ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="amount">注文金額</param>
		/// <param name="resTrackingId">処理トラキングID</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>実行結果</returns>
		private bool ExecutePaymentDocomoKetaiApi(
			string trackingId,
			string custCode,
			string paymentOrderId,
			decimal amount,
			out string resTrackingId,
			out string errorMessage)
		{
			errorMessage = "";
			var api = new PaymentSBPSCareerDocomoKetaiContinuousOrderApi();
			var result = api.Exec(
				trackingId,
				custCode,
				paymentOrderId,
				Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
				Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
				amount);
			if (result == false)
			{
				errorMessage = LogCreator.CreateErrorMessage(
					api.ResponseData.ResErrCode,
					api.ResponseData.ResErrMessages);
			}
			resTrackingId = api.ResponseData.ResTrackingId;

			return result;
		}

		/// <summary>
		/// SBPS ソフトバンク・ワイモバイルまとめて支払いでの継続課金購入要求
		/// </summary>
		/// <param name="trackingId">処理対象トラキングID</param>
		/// <param name="custCode">顧客ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="amount">注文金額</param>
		/// <param name="resTrackingId">処理トラキングID</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>実行結果</returns>
		private bool ExecutePaymentSoftbankKetaiApi(
			string trackingId,
			string custCode,
			string paymentOrderId,
			decimal amount,
			out string resTrackingId,
			out string errorMessage)
		{
			errorMessage = "";
			var api = new PaymentSBPSCareerSoftbankKetaiContinuousOrderApi();
			var result = api.Exec(
				trackingId,
				custCode,
				paymentOrderId,
				Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
				Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
				amount);
			if (result == false)
			{
				errorMessage = LogCreator.CreateErrorMessage(
					api.ResponseData.ResErrCode,
					api.ResponseData.ResErrMessages);
			}
			resTrackingId = api.ResponseData.ResTrackingId;

			return result;
		}

		/// <summary>
		/// Exec paypay payment
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>Result paypay GMO</returns>
		protected PaypayGmoResult ExecPaypayPayment(Hashtable order, CartObject cart)
		{
			var orderModel = new OrderModel(order);
			orderModel.PaymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			orderModel.OrderPriceTotal = cart.PriceTotal;
			var result = new PaypayGmoFacade().ExecPayment(cart, orderModel, true);

			if (string.IsNullOrEmpty(result.ErrorMessage) == false)
			{
				this.ApiErrorMessage = result.ErrorMessage;
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR));
			}
			return result;
		}

		/// <summary>
		/// Atobaraicom後払い決済処理
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="cart">カートオブジェクト</param>
		/// <returns>ブール</returns>
		protected bool ExcePaymentCvsDefAtobaraicom(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-5-D.";
			var api = new AtobaraicomRegistationApi();
			var entryApiExecResult = api.ExecRegistation(order, cart);

			if ((string.IsNullOrWhiteSpace(entryApiExecResult.OrderId) == false)
				&& entryApiExecResult.IsAuthorizeOK
				&& entryApiExecResult.IsSuccess)
			{
				PaymentFileLogger.WritePaymentLog(
					true,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Atobaraicom,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					entryApiExecResult.Messages,
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[entryApiExecResult.OrderId] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[entryApiExecResult.SystemOrderId] },
					});
				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = entryApiExecResult.SystemOrderId;
			}
			else if ((string.IsNullOrWhiteSpace(entryApiExecResult.OrderId) == false)
				&& entryApiExecResult.IsAuthorizeHold
				&& (this.OrderExecType == OrderRegisterBase.ExecTypes.FixedPurchaseBatch))
			{
				PaymentFileLogger.WritePaymentLog(
					true,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Atobaraicom,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					string.Format("{0}（与信中）", entryApiExecResult.Messages),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[entryApiExecResult.OrderId] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[entryApiExecResult.SystemOrderId] },
					});
				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = entryApiExecResult.SystemOrderId;
				this.IsAuthResultHold = true;
			}
			else if ((string.IsNullOrWhiteSpace(entryApiExecResult.OrderId) == false)
					&& entryApiExecResult.IsAuthorizeHold)
			{
				var result = new AtobaraicomCancelationApi().ExecCancel(entryApiExecResult.SystemOrderId);

				if (result)
				{
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ATOBARAICOM_CVSDEFAUTH_ERROR));
					PaymentFileLogger.WritePaymentLog(
						false,
						cart.Payment.PaymentId,
						PaymentFileLogger.PaymentType.Atobaraicom,
						PaymentFileLogger.PaymentProcessingType.ExecPayment,
						AtobaraicomConstants.ATOBARAICOM_API_AUTH_RESULT_STATUS_CANCEL,
						new Dictionary<string, string>
						{
							{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
							{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
						});
				}
				else
				{
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SYSTEM_ERROR));
				}

				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = entryApiExecResult.SystemOrderId;
				return false;
			}
			else
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ATOBARAICOM_CVSDEFAUTH_ERROR));
				this.ApiErrorMessage = api.Messages;
				PaymentFileLogger.WritePaymentLog(
					false,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.Atobaraicom,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					this.ApiErrorMessage,
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] },
					});
				return false;
			}
			return true;
		}

		/// <summary>
		/// Score後払い決済処理
		/// </summary>
		/// <param name="order">注文ハッシュ</param>
		/// <param name="cart">カート</param>
		/// <returns>True;成功、False：失敗</returns>
		protected bool ExecPaymentCvsDefScore(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-5-E.Score後払い決済処理";

			var facade = new ScoreApiFacade();
			var orderRegisterRequest = new ScoreRequestOrderRegisterModify(cart);

			// HTTPヘッダ情報
			if (order.ContainsKey("score_http_headers"))
			{
				orderRegisterRequest.HttpInfo.HttpHeader = StringUtility.ToEmpty(order["score_http_headers"]);
			}

			var orderRegisterResponse = facade.OrderRegister(orderRegisterRequest);
			if (orderRegisterResponse == null)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_SCORE_PAYMENT_NG_ERROR));
				this.ApiErrorMessage = "決済実施結果が取得できませんでした。";
				return false;
			}

			if (orderRegisterResponse.Result != ScoreResult.Ok.ToText())
			{
				// 与信NG
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_SCORE_PAYMENT_NG_ERROR));
				this.ApiErrorMessage = string.Join(
					"\r\n",
					orderRegisterResponse.Errors.ErrorList.Select(e => $"{e.ErrorCode}：{e.ErrorMessage}").ToArray());
				return false;
			}

			if (orderRegisterResponse.TransactionResult.IsResultNg)
			{
				var cancelRequest = new ScoreCancelRequest
				{
					Transaction =
					{
						NissenTransactionId = orderRegisterResponse.TransactionResult.NissenTransactionId,
						ShopTransactionId = cart.OrderId,
						BilledAmount = orderRegisterRequest.Buyer.BilledAmount
					}
				};
				var cancelResult = facade.OrderCancel(cancelRequest);

				if (cancelResult.Result != ScoreResult.Ok.ToText())
				{
					throw new Exception(
						string.Format(
							"スコア後払い与信失敗の取消にて失敗。注文ID：{0}、取引ID：{1}",
							cart.OrderId,
							orderRegisterResponse.TransactionResult.NissenTransactionId));
				}

				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_SCORE_PAYMENT_NG_ERROR));
				this.ApiErrorMessage = $"[審査結果：{orderRegisterResponse.TransactionResult.AuthorResult}]";
				return false;
			}

			if (orderRegisterResponse.TransactionResult.IsResultHold)
			{
				this.IsAuthResultHold = true;
				order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
			}

			// Score取引IDを格納
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = StringUtility.ToEmpty(cart.OrderId);
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(orderRegisterResponse.TransactionResult.NissenTransactionId);
			return true;
		}

		/// <summary>
		/// ベリトランス後払い決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>True;成功、False：失敗</returns>
		protected bool ExecPaymentCvsDefVeritrans(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-5-E.Veritrans後払い決済処理";

			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			var paymentVeritransCvsDef = new PaymentVeritransCvsDef();

			var orderRegisterResponse = paymentVeritransCvsDef.OrderRegister(cart, paymentOrderId);
			if (orderRegisterResponse == null)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_VERITRANS_PAYMENT_NG_ERROR));
				this.ApiErrorMessage = "決済実施結果が取得できませんでした。";
				return false;
			}

			if (orderRegisterResponse.Mstatus != VeriTransConst.RESULT_STATUS_OK)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_VERITRANS_PAYMENT_NG_ERROR));
				this.ApiErrorMessage = orderRegisterResponse.Errors != null
					? string.Join(
						"\r\n",
						orderRegisterResponse.Errors.Select(e => $"{e.ErrorCode}：{e.ErrorMessage}").ToArray())
					: string.Format("{0}：{1}", orderRegisterResponse.VResultCode, orderRegisterResponse.MerrMsg);

				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentType.VeriTrans,
					PaymentFileLogger.PaymentProcessingType.ApiError,
					LogCreator.CreateErrorMessage(orderRegisterResponse.VResultCode, orderRegisterResponse.MerrMsg));
				return false;
			}

			if (orderRegisterResponse.AuthorResult == VeriTransConst.VeritransAuthorResult.Ng.ToText())
			{
				var cancelResult = paymentVeritransCvsDef.OrderCancel(paymentOrderId);

				if (cancelResult.Mstatus == VeriTransConst.RESULT_STATUS_OK)
				{
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_VERITRANS_PAYMENT_NG_ERROR));
					this.ApiErrorMessage = $"[審査結果：{orderRegisterResponse.AuthorResult}]";
					PaymentFileLogger.WritePaymentLog(
						false,
						Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
						PaymentFileLogger.PaymentType.VeriTrans,
						PaymentFileLogger.PaymentProcessingType.ApiError,
						LogCreator.CreateErrorMessage(orderRegisterResponse.VResultCode, orderRegisterResponse.MerrMsg));
					return false;
				}

				throw new Exception(
					string.Format(
						"ベリトランス後払い与信失敗の取消にて失敗。注文ID：{0}、取引ID：{1}",
						cart.OrderId,
						orderRegisterResponse.OrderId));
			}

			if (orderRegisterResponse.AuthorResult == VeriTransConst.VeritransAuthorResult.Hold.ToText())
			{
				this.IsAuthResultHold = true;
				order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
			}

			PaymentFileLogger.WritePaymentLog(
				true,
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
				PaymentFileLogger.PaymentType.VeriTrans,
				PaymentFileLogger.PaymentProcessingType.ApiRequestEnd,
				LogCreator.CreateRequestMessageWithResult(
					orderRegisterResponse.GetType().Name,
					string.Empty,
					$"{PaymentFileLogger.PaymentType.VeriTrans}後払い",
					orderRegisterResponse.AuthorResult));

			// ベリトランス取引IDを格納
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = StringUtility.ToEmpty(orderRegisterResponse.OrderId);
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(orderRegisterResponse.CustTxn);
			return true;
		}

		/// <summary>
		/// GmoKb Payment
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>成功したか</returns>
		private bool ExecPaymentGmoKb(Hashtable order, CartObject cart)
		{
			this.TransactionName = "GmoKb Payment";

			var facade = new GmoTransactionApi();
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			var request = new GmoRequestTransactionRegister(cart);
			request.Buyer.ShopTransactionId = paymentOrderId;

			// HTTPヘッダ情報
			if (Constants.PAYMENT_SETTING_GMO_DEFERRED_ENABLE_HTTPHEADERS_POST
				&& order.ContainsKey("gmo_http_headers"))
			{
				request.HttpInfo.HttpHeader = StringUtility.ToEmpty(order["gmo_http_headers"]);
			}

			var result = facade.TransactionRegister(request);

			if (result == null)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_GMO_KB_PAYMENT_ALERT));
				this.ApiErrorMessage = string.Format("{0}：決済実施結果が取得できませんでした。", request.Buyer.ShopOrderDate);
				return false;
			}

			if (result.IsResultNg)
			{
				// 与信NG
				var errorMess = string.Format(
					"{0}:{1} ({2}) [審査結果:{3}]",
					request.Buyer.ShopOrderDate,
					result.Errors.Error[0].ErrorMessage,
					result.Errors.Error[0].ErrCode,
					result.Result);
				this.ErrorMessages.Add(errorMess);
				this.ApiErrorMessage = errorMess;
				return false;
			}

			if (result.IsNG || result.IsAlert)
			{
				// NG/Alert: Order data is not created
				var errorMess = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_GMO_KB_PAYMENT_ALERT);
				this.ErrorMessages.Add(errorMess);
				this.ApiErrorMessage = errorMess;
				return false;
			}

			this.GmoTransactionResult = result.TransactionResult.AuthorResult;
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(result.TransactionResult.GmoTransactionId);
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;

			if (result.IsInReview)
			{
				// In review: Order data is created
				order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = Constants.FLG_ORDER_CREDIT_STATUS_INREVIEW;
				return true;
			}
			if (result.IsDepositWaiting)
			{
				// Deposit Waiting: Order data is created
				order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = Constants.FLG_ORDER_CREDIT_STATUS_DEPOSIT_WAITING;
				return true;
			}

			// OK: Order data is created, Deposit status: deposited
			order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE;
			order[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE] = DateTime.Now;
			order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = StringUtility.ToEmpty(result.TransactionResult.AuthorResult);
			return true;
		}

		/// <summary>
		/// Exec payment cvs rakuten
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>True: Execute payment cvs rakuten is success, otherwise: false</returns>
		private bool ExecPaymentCvsRakuten(Hashtable order, CartObject cart)
		{
			this.TransactionName = "2-2-H.Rakutenコンビニ決済処理";

			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			var orderModel = new OrderModel(order)
			{
				Owner = cart.Owner.CreateModel(cart.OrderId),
				LastBilledAmount = cart.PriceTotal,
				SettlementAmount = cart.SettlementAmount,
				SettlementCurrency = cart.SettlementCurrency,
			};

			var rakutenAuthorizeRequest = RakutenApiUtility.CreateRakutenCvsAuthorizeRequest(orderModel, cart.Payment.RakutenCvsType);
			rakutenAuthorizeRequest.PaymentId = paymentOrderId;

			RakutenApiUtility.WriteLogRakutenRequest(rakutenAuthorizeRequest);
			var authorizeApiResponse = RakutenApiFacade.AuthorizeCvs(rakutenAuthorizeRequest);
			var paymentSuccess = (authorizeApiResponse.ResultType == RakutenConstants.RESULT_TYPE_SUCCESS);
			var externalPaymentCooperationLog = paymentSuccess
				? string.Empty
				: OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					order,
					cart);

			PaymentFileLogger.WritePaymentLog(
				paymentSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Rakuten,
				PaymentFileLogger.PaymentProcessingType.Rakuten3DSecureAuthResultSend,
				externalPaymentCooperationLog,
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]) },
					{ Constants.FIELD_ORDER_CARD_TRAN_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]) },
				});

			if (paymentSuccess == false)
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(
					authorizeApiResponse.ErrorCode,
					authorizeApiResponse.ErrorMessage);

				FileLogger.Write("RakutenCvs", this.ApiErrorMessage, true);

				var createOrderFailedLogMessage = OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					order,
					cart);

				AppLogger.WriteInfo(createOrderFailedLogMessage);

				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSAUTH_ERROR));
				if (authorizeApiResponse.ResultType == RakutenConstants.RESULT_TYPE_PENDING)
				{
					throw new Exception("決済処理でpendingが発生しました。");
				}

				return false;
			}

			var reference = (authorizeApiResponse.Reference != null && Constants.PAYMENT_RAKUTEN_CVS_MOCK_OPTION_ENABLED == false)
				? authorizeApiResponse.Reference.RakutenCardResult.CvsInfoList[0].Reference
				: string.Empty;

			var message = RakutenApiUtility.CreateResultMessage(
				cart.Payment.RakutenCvsType,
				reference,
				rakutenAuthorizeRequest.GrossAmount.ToPriceString(),
				rakutenAuthorizeRequest.CvsPayment.ExpirationDate,
				cart.Owner.DispLanguageLocaleId);

			order["payment_message_html"] = message.Item1;
			order["payment_message_text"] = message.Item2;

			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = authorizeApiResponse.AgencyRequestId;
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;
			order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			return true;
		}

		/// <summary>
		/// Execute payment CVS ZEUS
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>True: if execute payment is success</returns>
		private bool ExecPaymentCvsZeus(Hashtable order, CartObject cart)
		{
			this.TransactionName = "２－２－Ｈ．コンビニ・ZEUS";

			// Execute ZEUS secure link CVS payment
			var zeusCvsPayment = (PaymentZeusCvs)cart.Payment.PaymentObject;
			var zeusCvsResult = zeusCvsPayment.Exec(
				cart.SettlementAmount,
				StringUtility.ToZenkakuKatakana(cart.Owner.NameKana),
				cart.Owner.Tel1,
				cart.Owner.MailAddr,
				cart.OrderId);

			if (zeusCvsResult.IsSuccess)
			{
				// Save payment transaction id to order information
				order[Constants.FIELD_ORDER_CARD_TRAN_ID] = zeusCvsResult.ZeusOrderId;

				// Create payment message and save message to order information
				var paymentMessages = zeusCvsResult.CreateResultMessage(
					zeusCvsPayment.ConveniType,
					cart.SettlementAmount,
					cart.Owner.DispLanguageLocaleId);
				order["payment_message_html"] = paymentMessages.Html;
				order["payment_message_text"] = paymentMessages.Text;
			}
			else
			{
				// Get API error message
				var errorCodeMessage = zeusCvsResult.GetErrorCodeMessage();
				this.ApiErrorMessage = string.IsNullOrEmpty(errorCodeMessage)
					? zeusCvsResult.ErrorMessage
					: LogCreator.CreateErrorMessage(
						zeusCvsResult.ErrorCode,
						errorCodeMessage);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ZEUS_PAYMENT_ERROR));

				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
			}

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				zeusCvsResult.IsSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.Zeus,
				PaymentFileLogger.PaymentProcessingType.CvsPaymentProcessing,
				zeusCvsResult.IsSuccess
					? string.Empty
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						this.ApiErrorMessage),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] },
				});

			return zeusCvsResult.IsSuccess;
		}

		/// <summary>
		/// ベリトランスPayPay決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <returns>成功ならTRUE</returns>
		protected bool ExecVeriTransPaypayPayment(Hashtable order, CartObject cart)
		{
			this.TransactionName = "ベリトランスPayPay決済処理";
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			var originalOrderId = (string)(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID] ?? cart.FixedPurchase.FixedPurchaseId);

			var reauthResult = new PaymentVeritransPaypay().ReAuthorize(
				originalOrderId,
				paymentOrderId,
				cart.PriceTotal);
			if (reauthResult.Mstatus != VeriTransConst.RESULT_STATUS_OK)
			{
				this.ApiErrorMessage = LogCreator.CreateErrorMessage(reauthResult.VResultCode, reauthResult.MerrMsg);
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_REAUTH_ALERT));
				PaymentFileLogger.WritePaymentLog(
					false,
					cart.Payment.PaymentId,
					PaymentFileLogger.PaymentType.PayPay,
					PaymentFileLogger.PaymentProcessingType.ReAuthExec,
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						this.ApiErrorMessage),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId },
						{ Constants.FIELD_USER_USER_ID, cart.OrderUserId }
					});
				return false;
			}

			AddOrSetHastTable(order, Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId);
			AddOrSetHastTable(order, Constants.FIELD_ORDER_CARD_TRAN_ID, reauthResult.PaypayOrderId);
			PaymentFileLogger.WritePaymentLog(
				true,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.PayPay,
				PaymentFileLogger.PaymentProcessingType.ReAuthExec,
				string.Empty,
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId },
					{ Constants.FIELD_USER_USER_ID, cart.OrderUserId },
				});
			return true;
		}

		/// <summary>
		/// Execute payment CVS paygent
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>True: if execute payment is success</returns>
		private bool ExecPaymentCvsPaygent(Hashtable order, CartObject cart)
		{
			this.TransactionName = "２－２－Ｉ．コンビニ・Paygent";

			// Execute paygent CVS payment
			var paygentCvsPayment = new PaygentApiFacade();
			var paygentCvsResult = paygentCvsPayment.RegisterOrder(cart, order);

			if (paygentCvsResult.IsSuccess)
			{
				// Create payment message and save message to order information
				var (html, text) = paygentCvsResult.CreateResultMessage(cart);

				order[Constants.FIELD_ORDER_CARD_TRAN_ID] = paygentCvsResult.Response.PaymentId;
				order[Constants.PAYMENT_MESSAGE_HTML] = html;
				order[Constants.PAYMENT_MESSAGE_TEXT] = text;
			}
			else
			{
				// Get API error message
				this.ApiErrorMessage = paygentCvsResult.GetErrorMessage();
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PAYGENT_PAYMENT_ERROR));

				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
			}

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				paygentCvsResult.IsSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.PaygentEng,
				PaymentFileLogger.PaymentProcessingType.CvsPaygentPaymentProcessing,
				paygentCvsResult.IsSuccess
					? string.Empty
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						order,
						cart,
						this.ApiErrorMessage),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
				});

			return paygentCvsResult.IsSuccess;
		}

		/// <summary>
		/// Execute payment Atm
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>Execute result</returns>
		private bool ExecPaymentAtm(Hashtable order, CartObject cart)
		{
			switch (Constants.PAYMENT_ATM_KBN)
			{
				case Constants.PaymentATMKbn.Paygent:
					return ExecPaymentATMPaygent(order, cart);
			}

			return true;
		}

		/// <summary>
		/// Execute payment Banknet
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>Execute result</returns>
		private bool ExecPaymentBanknet(Hashtable order, CartObject cart)
		{
			switch (Constants.PAYMENT_NETBANKING_KBN)
			{
				case Constants.PaymentBanknetKbn.Paygent:
					return ExecPaymentBanknetPaygent(order, cart);
			}

			return true;
		}

		/// <summary>
		/// Execute payment Banknet Paygent
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>Execute result</returns>
		private bool ExecPaymentBanknetPaygent(Hashtable order, CartObject cart)
		{
			this.TransactionName = "Paygent Banknet";
			var paygentBanknetResult = new PaygentApiFacade().BanknetRegist(cart, order);
			if (paygentBanknetResult.IsSuccess)
			{
				// Create payment message and save message to order information
				var (html, text) = paygentBanknetResult.CreateResultMessage(cart);

				order[Constants.PAYMENT_MESSAGE_HTML] = html;
				order[Constants.PAYMENT_MESSAGE_TEXT] = text;
			}
			else
			{
				// Get API error message
				this.ApiErrorMessage = paygentBanknetResult.GetErrorMessage();
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PAYGENT_PAYMENT_ERROR));

				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
			}

			return paygentBanknetResult.IsSuccess;
		}

		/// <summary>
		/// Execute payment ATM paygent
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="cart">Cart</param>
		/// <returns>True: if execute payment is success, otherwise false</returns>
		private bool ExecPaymentATMPaygent(Hashtable order, CartObject cart)
		{
			this.TransactionName = PaymentFileLogger.PaymentProcessingType.AtmPayment.ToText();

			// Execute paygent ATM payment
			var paygentPayment = new PaygentApiFacade();
			var paygentResult = paygentPayment.PaygentAtmRegister(cart, order);

			if (paygentResult.IsSuccess)
			{
				// Create payment message and save message to order information
				var (html, text) = paygentResult.CreateResultMessage(cart);
				order[Constants.FIELD_ORDER_CARD_TRAN_ID] = paygentResult.Response.PaymentId;
				order[Constants.PAYMENT_MESSAGE_HTML] = html;
				order[Constants.PAYMENT_MESSAGE_TEXT] = text;
			}
			else
			{
				// Get API error message
				this.ApiErrorMessage = paygentResult.GetErrorMessage();
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PAYGENT_PAYMENT_ERROR));
				AppLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart));
			}

			var externalPaymentLog = string.Format(
				"{0:yyyy/mm/dd HH:mm:ss} [{1}] 決済取引ID：{2}・{3}円 ATM決済申込 最終更新者：{4}",
				DateTime.Now,
				paygentResult.IsSuccess
					? "成功"
					: "失敗",
				order[Constants.FIELD_ORDER_CARD_TRAN_ID],
				cart.PriceTotal.ToPriceString(),
				this.LastChanged);
			order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG] = externalPaymentLog;

			return paygentResult.IsSuccess;
		}

		/// <summary>トランザクション名</summary>
		private string TransactionName { get; set; }
		/// <summary>注文登録プロパティ</summary>
		private OrderRegisterProperties Properties { get; set; }
		/// <summary>注文実行種別</summary>
		private OrderRegisterBase.ExecTypes OrderExecType { get { return this.Properties.OrderExecType; } }
		/// <summary>ユーザーか</summary>
		protected bool IsUser { get { return this.Properties.IsUser; } }
		/// <summary>エラーメッセージ</summary>
		public List<string> ErrorMessages { get { return this.Properties.ErrorMessages; } }
		/// <summary>外部連携取込用エラーメッセージ</summary>
		public string ApiErrorMessage { get; private set; }
		/// <summary>アラートメッセージ</summary>
		public List<string> AlertMessages { get { return this.Properties.AlertMessages; } }
		/// <summary>ゼウス3Dセキュア注文</summary>
		public List<Hashtable> ZeusCard3DSecurePaymentOrders { get { return this.Properties.ZeusCard3DSecurePaymentOrders; } }
		/// <summary>成功注文</summary>
		public List<Hashtable> SuccessOrders { get { return this.Properties.SuccessOrders; } }
		/// <summary>成功カート</summary>
		public List<CartObject> SuccessCarts { get { return this.Properties.SuccessCarts; } }
		/// <summary>DB更新時の最終更新者</summary>
		public string LastChanged { get { return this.Properties.LastChanged; } }
		/// <summary>Zcom card 3DSecure payment orders</summary>
		public List<Hashtable> ZcomCard3DSecurePaymentOrders { get { return this.Properties.ZcomCard3DSecurePaymentOrders; } }
		/// <summary>ベリトランス3Dセキュア注文情報</summary>
		public List<Hashtable> VeriTrans3DSecurePaymentOrders { get { return this.Properties.VeriTrans3DSecurePaymentOrders; } }
		/// <summary>与信結果がHOLDか(現在はコンビニ後払い「DSK」のみ利用)</summary>
		public bool IsAuthResultHold { get; private set; }
		/// <summary>Rakuten 3D Secure Payment Orders</summary>
		public List<Hashtable> RakutenCard3DSecurePaymentOrders { get { return this.Properties.RakutenCard3DSecurePaymentOrders; } }
		/// <summary>GMO3Dセキュア注文</summary>
		public List<Hashtable> GmoCard3DSecurePaymentOrders { get { return this.Properties.GmoCard3DSecurePaymentOrders; } }
		/// <summary>GMO承認結果</summary>
		public string GmoTransactionResult { get; set; }
		/// <summary>YamatoKWC3Dセキュア注文</summary>
		public List<Hashtable> YamatoCard3DSecurePaymentOrders { get { return this.Properties.YamatoCard3DSecurePaymentOrders; } }
	}
}

