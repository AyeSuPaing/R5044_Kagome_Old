/*
=========================================================================================================
  Module      : 仮クレジットカード処理実行クラス(ProvisionalCreditCardProcessor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.EScott;
using w2.App.Common.Order.Payment.EScott.DataSchema;
using w2.Common.Sql;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.Receiver;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Order.Reauth.Actions;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.Common.Logger;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 仮クレジットカード処理実行クラス
	/// </summary>
	public class ProvisionalCreditCardProcessor
	{
		/// <summary>
		/// 未登録クレジットカード登録（通信しない）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="cardDispName">カード表示名</param>
		/// <param name="registerActionKbn">登録タイプ</param>
		/// <param name="registerTargetId">登録対象ID</param>
		/// <param name="beforeOrderStatus">更新前ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>ユーザークレジットカード</returns>
		public UserCreditCard RegisterUnregisterdCreditCard(
			string userId,
			string cardDispName,
			string registerActionKbn,
			string registerTargetId,
			string beforeOrderStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = RegisterUnregisterdCreditCard(
					userId,
					cardDispName,
					registerActionKbn,
					registerTargetId,
					beforeOrderStatus,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		/// <summary>
		/// 未登録クレジットカード登録（通信しない）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="cardDispName">カード表示名</param>
		/// <param name="registerActionKbn">登録アクション区分</param>
		/// <param name="registerTargetId">登録対象ID</param>
		/// <param name="beforeOrderStatus">更新前ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード</returns>
		public UserCreditCard RegisterUnregisterdCreditCard(
			string userId,
			string cardDispName,
			string registerActionKbn,
			string registerTargetId,
			string beforeOrderStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 新規作成
			var branchNo = new UserCreditCardService().Insert(
				new UserCreditCardModel
				{
					UserId = userId,
					CooperationId = "",
					CardDispName = cardDispName,
					DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
					CompanyCode = "",
					LastFourDigit = "XXXX",
					ExpirationYear = "XX",
					ExpirationMonth = "XX",
					AuthorName = "XXXXXXXX",
					LastChanged = lastChanged,
					CooperationId2 = "",
					RegisterActionKbn = registerActionKbn,
					RegisterStatus = Constants.FLG_USERCREDITCARD_REGISTER_STATUS_UNREGISTERED,
					RegisterTargetId = registerTargetId,
					BeforeOrderStatus = beforeOrderStatus,
				},
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 連携ID更新（ヤマトだけトークン取得時のものを設定）
			var userCreditCardCooperationInfo = UserCreditCardCooperationInfoFacade.CreateForCreateProvisionalCreditCard(
				userId,
				branchNo,
				accessor);
			new UserCreditCardService().Modify(
				userId,
				branchNo,
				model =>
				{
					model.CooperationId = userCreditCardCooperationInfo.CooperationId1;
					model.CooperationId2 = userCreditCardCooperationInfo.CooperationId2;
				},
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
			var userCreditCard = UserCreditCard.Get(userId, branchNo, accessor);
			return userCreditCard;
		}

		/// <summary>
		/// POSTからのクレジットカード登録
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>ユーザークレジットカード</returns>
		public bool UpdateProvisionalCreditCardRegisterd(HttpContext context, string lastChanged)
		{
			// 旧ユーザークレジットカード、更新向けユーザークレジットカード取得
			Tuple<UserCreditCardModel, UserCreditCardModel> userCreditCards;
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Gmo:
					userCreditCards = CreateUserCreditCardForUpdateGmo(context, lastChanged);
					break;

				case Constants.PaymentCard.YamatoKwc:
					userCreditCards = CreateUserCreditCardForUpdateYamatoKwc(context, lastChanged);
					break;

				case Constants.PaymentCard.EScott:
					userCreditCards = CreateUserCreditCardForUpdateEScott(context, lastChanged);
					break;

				default:
					throw new Exception("未対応のカード区分：" + Constants.PAYMENT_CARD_KBN);
			}
			if (userCreditCards == null) return false;

			// クレジットカード情報更新
			new UserCreditCardService().Update(userCreditCards.Item2, UpdateHistoryAction.Insert);

			// 定期変更の場合は仮クレジットカードを本クレジットカードに戻す
			if (userCreditCards.Item1.IsRegisterActionKbnFixedPurchaseModify)
			{
				new FixedPurchaseService()
					.UpdateForProvisionalCreditCardRegisterd(
						userCreditCards.Item1.RegisterTargetId,
						lastChanged,
						UpdateHistoryAction.Insert);
			}
			return true;
		}

		/// <summary>
		/// POSTからの更新向けユーザークレジットカード作成（GMO）
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>旧ユーザークレジットカード、更新向けユーザークレジットカード</returns>
		private Tuple<UserCreditCardModel, UserCreditCardModel> CreateUserCreditCardForUpdateGmo(HttpContext context, string lastChanged)
		{
			var response = new PaymentGmoCreditCardRegisterResponse(context);
			if (response.SiteId != Constants.PAYMENT_SETTING_GMO_SITE_ID) return null;

			// 名義人を取得したいがためにAPI接続
			var paymentGmo = new PaymentGmoCredit();
			if (paymentGmo.SearchCard(response.MemberId) == false)
			{
				throw new Exception("GMO会員IDからユーザークレジットカードが検索できませんでした：" + response.MemberId);
			}
			var authorName = paymentGmo.Result.Parameters["HolderName"];

			// 更新用ユーザークレジットカードモデル作成
			var userCreditCardOld = new UserCreditCardService().GetByCooperationId1(response.MemberId);
			var dispFlg = ((string.IsNullOrEmpty(userCreditCardOld.CardDispName) == false)
				&& (userCreditCardOld.CardDispName != Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME)
				&& (userCreditCardOld.CardDispName != Constants.CREDITCARD_UNREGIST_FIXEDPURCHASE_DISPLAY_NAME));
			var lastFourDigit = (response.CardNo.Length < 4)
				? response.CardNo
				: response.CardNo.Substring(response.CardNo.Length - 4, 4);
			// 登録
			var userCreditCardNew = CreateUserCreditCardNewForRegister(
				"",	// GMOはカード会社なし
				lastFourDigit,
				response.Expire.Substring(2, 2),
				response.Expire.Substring(0, 2),
				authorName,
				dispFlg,
				userCreditCardOld.CardDispName,	// 定期の場合は先に登録してある可能性
				lastChanged,
				userCreditCardOld);

			var userCreditCards = response.IsRegistable()
				? new Tuple<UserCreditCardModel, UserCreditCardModel>(userCreditCardOld, userCreditCardNew) 
				: new Tuple<UserCreditCardModel, UserCreditCardModel>(userCreditCardOld, userCreditCardOld);
			return userCreditCards;
		}

		/// <summary>
		/// POSTからの更新向けユーザークレジットカード作成（YamatoKwc）
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>旧ユーザークレジットカード、更新向けユーザークレジットカード</returns>
		private Tuple<UserCreditCardModel, UserCreditCardModel> CreateUserCreditCardForUpdateYamatoKwc(HttpContext context, string lastChanged)
		{
			var response = new PaymentYamatoKwcCreditCardRegisterResponse(context);
			if (response.TraderCode != Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE) return null;

			// 旧ユーザークレジットカード取得
			var tradeInfo = new PaymentYamatoKwcTradeInfoApi().Exec(response.OrderNo);
			if (tradeInfo.ResultDatas.Count == 0)
			{
				throw new Exception("受付番号から取引情報が取得できませんでした：" + response.OrderNo);
			}
			var memberId = tradeInfo.ResultDatas[0].MemberId;
			var userCreditCardOld = new UserCreditCardService().GetByCooperationId1(memberId);
			if (userCreditCardOld == null)
			{
				throw new Exception("カード保有者IＤからユーザークレジットカードが検索できませんでした：" + memberId);
			}

			// 1円与信削除（消せなくても先に進む）
			var paymentOrderId = response.OrderNo;
			var yamatoCreditCancelApiResponse = new PaymentYamatoKwcCreditCancelApi().Exec(paymentOrderId);

			var execYamatoCreditCancelApiResult = yamatoCreditCancelApiResponse.Success;
			PaymentFileLogger.WritePaymentLog(
				execYamatoCreditCancelApiResult,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Yamatokwc,
				PaymentFileLogger.PaymentProcessingType.OneYenAuthCancel,
				execYamatoCreditCancelApiResult
					? ""
					: LogCreator.CreateErrorMessage(
						yamatoCreditCancelApiResponse.ErrorCode,
						yamatoCreditCancelApiResponse.ErrorMessage),
				new Dictionary<string, string>
				{
					{Constants.FIELD_USER_USER_ID, userCreditCardOld.UserId},
					{Constants.FIELD_ORDER_PAYMENT_ORDER_ID,paymentOrderId},
				});

			if (yamatoCreditCancelApiResponse.Success == false)
			{
				AppLogger.WriteError(string.Format(
					"クレジットカード登録時の1円与信キャンセル失敗（user_id={0} 決済注文番号={1} エラーコード={2}）",
					userCreditCardOld.UserId,
					paymentOrderId,
					yamatoCreditCancelApiResponse.ErrorCode));
			}

			// 更新用ユーザークレジットカードモデル作成
			var dispFlg = ((string.IsNullOrEmpty(userCreditCardOld.CardDispName) == false)
				&& (userCreditCardOld.CardDispName != Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME)
				&& (userCreditCardOld.CardDispName != Constants.CREDITCARD_UNREGIST_FIXEDPURCHASE_DISPLAY_NAME));
			// 登録
			var yamatoCreditInfo = new PaymentYamatoKwcCreditInfoGetApi().Exec(memberId, userCreditCardOld.CooperationId2);
			var userCreditCardNew = CreateUserCreditCardNewForRegister(
				yamatoCreditInfo.CardDatas[0].CardCodeApi,
				yamatoCreditInfo.CardDatas[0].MaskingCardNo.Replace("*", ""),
				yamatoCreditInfo.CardDatas[0].CardExp.Substring(0, 2),
				yamatoCreditInfo.CardDatas[0].CardExp.Substring(2, 2),
				yamatoCreditInfo.CardDatas[0].CardOwner,
				dispFlg,
				userCreditCardOld.CardDispName,	// 定期の場合は先に登録してある可能性
				lastChanged,
				userCreditCardOld);
			return new Tuple<UserCreditCardModel, UserCreditCardModel>(userCreditCardOld, userCreditCardNew);
		}

		/// <summary>
		/// POSTからの更新向けユーザークレジットカード作成（e-SCOTT）
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>旧ユーザークレジットカード、更新向けユーザークレジットカード</returns>
		private Tuple<UserCreditCardModel, UserCreditCardModel> CreateUserCreditCardForUpdateEScott(HttpContext context, string lastChanged)
		{
			var request = context.Request.Form.ToString();
			var memberRegisterResponse = new Member4MemAddPost(request);
			if (memberRegisterResponse.TenantId != Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TENANTID) return null;

			var kaiinId = memberRegisterResponse.KaiinId;
			var memberRefApi = EScottMember4MemRefApi.CreateEScottMember4MemRefApi(kaiinId);
			var memberRefApiResult = memberRefApi.ExecRequest();
			if (memberRefApiResult.IsSuccess == false)
			{
				var error = string.Format(
					"会員IDから会員情報が取得できませんでした。会員ID : {0}, エラーコード : {1}, メッセージ : {2}",
					kaiinId,
					memberRefApiResult.ResponseCd,
					memberRefApiResult.ResponseMessage);
				throw new Exception(error);
			}

			var userCreditCardOld = new UserCreditCardService().GetByCooperationId1(kaiinId);
			if (userCreditCardOld == null)
			{
				throw new Exception("会員IDからユーザークレジットカードが検索できませんでした会員ID : " + kaiinId);
			}

			// 更新用ユーザークレジットカードモデル作成
			var dispFlg = ((string.IsNullOrEmpty(userCreditCardOld.CardDispName) == false)
				&& (userCreditCardOld.CardDispName != Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME)
				&& (userCreditCardOld.CardDispName != Constants.CREDITCARD_UNREGIST_FIXEDPURCHASE_DISPLAY_NAME));
			// 登録
			var cardExpMm = memberRefApiResult.CardExp.Substring(2);
			var cardExpYy = memberRefApiResult.CardExp.Substring(0, 2);
			var userCreditCardNew = CreateUserCreditCardNewForRegister(
				string.Empty,
				memberRefApiResult.CardNo.Substring(memberRefApiResult.CardNo.Length - 4),
				cardExpMm,
				cardExpYy,
				string.Empty,
				dispFlg,
				userCreditCardOld.CardDispName,	// 定期の場合は先に登録してある可能性
				lastChanged,
				userCreditCardOld);
			return new Tuple<UserCreditCardModel, UserCreditCardModel>(userCreditCardOld, userCreditCardNew);
		}

		/// <summary>
		/// ZEUS向け仮クレジットカード登録処理
		/// </summary>
		/// <param name="userCreditCardOld">旧ユーザークレジットカード</param>
		/// <param name="lastFourDigit">カード番号下4桁</param>
		/// <param name="expirationMonth">カード有効期限(月)</param>
		/// <param name="expirationYear">カード有効期限(年)</param>
		/// <param name="authorName">カード名義人</param>
		/// <param name="dispFlg">登録フラグ</param>
		/// <param name="cardDispName">クレジットカード登録名</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateProvisionalCreditCardRegisterdForZeus(
			UserCreditCardModel userCreditCardOld,
			string lastFourDigit,
			string expirationMonth,
			string expirationYear,
			string authorName,
			bool dispFlg,
			string cardDispName,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			var userCreditCardNew = CreateUserCreditCardNewForRegister(
				"",
				lastFourDigit,
				expirationMonth,
				expirationYear,
				authorName,
				dispFlg,
				cardDispName,
				lastChanged,
				userCreditCardOld);
			// クレジットカード情報更新
			new UserCreditCardService().Update(userCreditCardNew, updateHistoryAction);
		}

		/// <summary>
		/// 登録向け新規ユーザークレジットカード作成
		/// </summary>
		/// <param name="cardCompanyCode">カード会社コード</param>
		/// <param name="lastFourDigit">カード番号下4桁</param>
		/// <param name="expirationMonth">カード有効期限(月)</param>
		/// <param name="expirationYear">カード有効期限(年)</param>
		/// <param name="authorName">カード名義人</param>
		/// <param name="dispFlg">登録フラグ</param>
		/// <param name="cardDispName">クレジットカード登録名</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="userCreditCardOld">旧クレジットカード</param>
		/// <returns>登録向け新規ユーザークレジットカード</returns>
		private UserCreditCardModel CreateUserCreditCardNewForRegister(
			string cardCompanyCode,
			string lastFourDigit,
			string expirationMonth,
			string expirationYear,
			string authorName,
			bool dispFlg,
			string cardDispName,
			string lastChanged,
			UserCreditCardModel userCreditCardOld)
		{
			var userCreditCardNew = userCreditCardOld.Clone();
			userCreditCardNew.CompanyCode = cardCompanyCode;
			userCreditCardNew.LastFourDigit = lastFourDigit;
			userCreditCardNew.ExpirationMonth = expirationMonth;
			userCreditCardNew.ExpirationYear = expirationYear;
			userCreditCardNew.AuthorName = authorName;
			// 注文系以外は登録アクション区分を空にする
			userCreditCardNew.DispFlg = dispFlg
				? Constants.FLG_USERCREDITCARD_DISP_FLG_ON
				: Constants.FLG_USERCREDITCARD_DISP_FLG_OFF;
			userCreditCardNew.CardDispName = cardDispName;
			userCreditCardNew.RegisterActionKbn = (userCreditCardOld.IsRegisterActionKbnOrderSomething)
				? userCreditCardOld.RegisterActionKbn
				: "";
			// 注文クレカ登録時のみ未与信（与信待ち）にする
			userCreditCardNew.RegisterStatus = userCreditCardOld.IsRegisterActionKbnOrderSomething
				? Constants.FLG_USERCREDITCARD_REGISTER_STATUS_UNAUTHED
				: Constants.FLG_USERCREDITCARD_REGISTER_STATUS_NORMAL;
			// 登録ができたらIDクリアを行う
			userCreditCardNew.RegisterTargetId = "";
			userCreditCardNew.LastChanged = lastChanged;
			return userCreditCardNew;
		}

		/// <summary>
		/// 与信を実行し、注文ステータスを更新する
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cardBranchNo">クレジットカード枝番</param>
		/// <param name="cardCompanyCode">カード会社コード</param>
		/// <param name="cardInstallmentsName">支払回数文言</param>
		/// <param name="cardInstallmentsCode">支払回数コード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>エラーメッセージ</returns>
		public string AuthAndUpdateOrderStatus(
			string orderId,
			int cardBranchNo,
			string cardCompanyCode,
			string cardInstallmentsCode,
			string cardInstallmentsName,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			// 注文情報取得
			var orderOld = new OrderService().Get(orderId);
			if (orderOld.OrderPaymentKbn != Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
			{
				return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_NOT_EXISTED);
			}

			// 再与信を利用してクレジットカード与信実行
			var orderForReauth = orderOld.Clone();
			orderForReauth.CardKbn = cardCompanyCode;
			orderForReauth.CardInstallmentsCode = cardInstallmentsCode;
			orderForReauth.CardInstruments = cardInstallmentsName;
			var reauthExecuter = new ReauthExecuter(
				new ReauthCreditCardAction(new ReauthCreditCardAction.ReauthActionParams(orderForReauth)),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()));
			var reauthResult = reauthExecuter.Execute();

			// ユーザークレジットカードの登録ステータス・before注文ステータス更新
			var userCreditCardOld = new UserCreditCardService().Get(orderOld.UserId, cardBranchNo);
			new UserCreditCardService().Modify(
				userCreditCardOld.UserId,
				userCreditCardOld.BranchNo,
				userCreditCard =>
				{
					if (reauthResult.AuthSuccess)
					{
						userCreditCard.RegisterActionKbn = Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_NORMAL;
						userCreditCard.RegisterStatus = Constants.FLG_USERCREDITCARD_REGISTER_STATUS_NORMAL;
						userCreditCard.BeforeOrderStatus = "";
					}
					else
					{
						userCreditCard.RegisterStatus = Constants.FLG_USERCREDITCARD_REGISTER_STATUS_AUTHERROR;
					}
					userCreditCard.LastChanged = lastChanged;
				},
				updateHistoryAction);

			// 仮クレジットカード向け注文ステータス変更
			if (reauthResult.AuthSuccess)
			{
				// 注文完了時シリアルキー更新（出荷完了に更新。デジコンの場合変更や返品はありえないのでここが実行されるのは注文済みのときのみ。）
				var isDigitalcontents = (orderForReauth.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON);
				var paymentStatusCompleteFlg = (isDigitalcontents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
					? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
					: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
				var successPaymentStatus = paymentStatusCompleteFlg
					? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
					: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;

				// 注文更新（注文ステータス、クレジットカード情報）
				new OrderService().Modify(
					orderId,
					orderTmp =>
					{
						orderTmp.OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
						orderTmp.OrderPaymentStatus = successPaymentStatus;
						if (successPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
						{
							orderTmp.OrderPaymentDate = DateTime.Now;
						}
						orderTmp.OrderStatus = userCreditCardOld.BeforeOrderStatus;
						orderTmp.CardKbn = cardCompanyCode;
						orderTmp.CardInstallmentsCode = cardInstallmentsCode;
						orderTmp.CardInstruments = cardInstallmentsName;
						orderTmp.PaymentOrderId = reauthResult.PaymentOrderId;
						orderTmp.CardTranId = reauthResult.CardTranId;
						orderTmp.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						orderTmp.ExternalPaymentAuthDate = DateTime.Now;
						orderTmp.PaymentMemo =
							(orderTmp.PaymentMemo + "\r\n"
								+ OrderCommon.CreateOrderPaymentMemoForAuth(
									orderId,
									reauthResult.PaymentOrderId,
									Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
									reauthResult.CardTranId,
									CurrencyManager.GetSendingAmount(
										orderOld.LastBilledAmount,
										orderOld.SettlementAmount,
										orderOld.SettlementCurrency))).Trim();
						orderTmp.LastChanged = lastChanged;
					},
					UpdateHistoryAction.DoNotInsert);

				// シリアルキー出荷処理（注文ステータスも出荷にする）
				if (isDigitalcontents && (successPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE))
				{
					OrderCommon.DeliverSerialKeyForOrderComplete(orderId, lastChanged);
				}

				// 注文に紐づく定期も仮クレジットカードであれば更新
				if (orderOld.IsFixedPurchaseOrder)
				{
					var fixedPurchaseService = new FixedPurchaseService();
					var fixedPurchase = fixedPurchaseService.Get(orderOld.FixedPurchaseId);
					if (fixedPurchase.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
					{
						fixedPurchaseService.UpdateOrderPayment(
							orderOld.FixedPurchaseId,
							Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
							cardBranchNo,
							cardInstallmentsCode,
							fixedPurchase.ExternalPaymentAgreementId,
							lastChanged,
							UpdateHistoryAction.DoNotInsert);
					}
				}
			}

			// 更新履歴作成
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(orderId, lastChanged);
			}

			// 外部決済連携ログ格納処理
			OrderCommon.AppendExternalPaymentCooperationLog(
				reauthResult.AuthSuccess,
				orderId,
				reauthResult.AuthSuccess
					? LogCreator.CreateMessage(orderId, orderForReauth.PaymentOrderId)
					: reauthResult.ApiErrorMessages,
				lastChanged,
				UpdateHistoryAction.Insert);

			if (reauthResult.AuthSuccess == false)
			{
				if (OrderCommon.NeedsRegisterProvisionalCreditCardCardKbnZeus)
				{
					return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
				}

				if (OrderCommon.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus)
				{
					return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR_FOR_PROVISIONAL_CREDITCARD_AND_URGE_REREGISTRATION);
				}

				throw new Exception("未対応：" + Constants.PAYMENT_CARD_KBN);
			}
			return "";
		}

		/// <summary>
		/// 与信結果クラス
		/// </summary>
		public class AuthResult
		{
			/// <summary>結果</summary>
			public bool Result { get; set; }
			/// <summary>決済注文ID</summary>
			public string PaymentOrderId { get; set; }
			/// <summary>取引ID</summary>
			public string CardTranId { get; set; }
			/// <summary>エラーメッセージ</summary>
			public string ErrorMessage { get; set; }
		}
	}
}
