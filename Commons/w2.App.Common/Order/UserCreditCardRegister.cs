/*
=========================================================================================================
  Module      : ユーザークレジットカード登録クラス(UserCreditCardRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.EScott;
using w2.Common.Sql;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UpdateHistory;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.Zcom.Cancel;
using w2.App.Common.Order.Payment.GMO.Zcom.Direct;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.App.Common.User;
using w2.Common.Helper;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Rakuten.Authorize;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.User.Botchan;
using w2.App.Common.Web;

namespace w2.App.Common.Order
{
	/// <summary>
	/// サイト区分
	/// </summary>
	public enum SiteKbn
	{
		Pc,
		SmartPhone,
		Mobile
	}

	/// <summary>
	/// ユーザークレジットカード登録クラス
	/// </summary>
	public class UserCreditCardRegister
	{
		/// <summary>
		/// 登録（登録向け与信実行）
		/// </summary>
		/// <param name="creditCardInput">クレジットカード入力情報</param>
		/// <param name="siteKbn">サイト区分</param>
		/// <param name="setDispFlg">表示フラグを設定するか（定期用に新規登録するカードは非表示）</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="isOrderFlow">注文フローでの登録か</param>
		/// <returns>成功したか、新規発行枝番、エラーメッセージ</returns>
		public Result Exec(
			UserCreditCardInput creditCardInput,
			SiteKbn siteKbn,
			bool setDispFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null,
			bool isOrderFlow = false)
		{
			var userCreditCard = ExecOnlySave(creditCardInput, lastChanged, UpdateHistoryAction.DoNotInsert);

			// 外部連携をしてカードの有効性をチェックする
			var user = new UserService().Get(creditCardInput.UserId, accessor);
			var userCreditCardModule = new UserCreditCard(userCreditCard);

			var result = ExecExternalPayment(creditCardInput, userCreditCardModule, siteKbn, user, UpdateHistoryAction.DoNotInsert);
			var success = result.Item1;
			var errorMessage = result.Item2;

			// 成功したらペイジェントクレカの場合は連携ID2に顧客カードIDを格納
			if (success && (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent))
			{
				var updateParams = userCreditCard;
				updateParams.CooperationId2 = SessionManager.UserCreditCooperationId2;
				new UserCreditCardService().UpdateCooperationId2(updateParams, accessor);
				// 更新後空にしておく
				SessionManager.UserCreditCooperationId2 = string.Empty;
			}

			// クレジット情報登録(表示フラグを有効に)
			if (success && setDispFlg)
			{
				var updateDispFlgResult = userCreditCardModule.UpdateDispFlg(
					true,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					updateDispFlgResult,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Unknown,
					PaymentFileLogger.PaymentProcessingType.RegistCreditCard,
					string.Format(
						"{0}:{1}",
						PaymentFileLogger.PaymentProcessingType.RegistCreditCard.ToText(),
						updateDispFlgResult ? "" : "(表示更新に失敗)"),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_USER_USER_ID, user.UserId },
						{ Constants.FOR_LOG_KEY_BRANCH_NO, userCreditCard.BranchNo.ToString() }
					});

				//"クレジット情報の登録(表示フラグ更新に失敗)"
				if (updateDispFlgResult == false)
				{
					// 失敗
					success = false;

					// ここにきている時点でAPIのErrorMessageは取ることができないためここで値を入れてあげる
					errorMessage = "クレジット情報の登録(表示フラグ更新に失敗)";
				}
			}

			// 失敗であれば不要なクレジット情報削除、トークンも有効期限切れに
			// HACK:注文フローでの登録処理の場合後続処理に影響するためここでは削除しない
			if ((success == false) && (isOrderFlow == false))
			{
				userCreditCardModule.Delete(lastChanged, UpdateHistoryAction.DoNotInsert);

				if (creditCardInput.CreditToken != null) creditCardInput.CreditToken.SetTokneExpired();

				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Unknown,
					PaymentFileLogger.PaymentProcessingType.RegistCreditCard,
					errorMessage,
					new Dictionary<string, string>
					{
						{ Constants.FIELD_USER_USER_ID, user.UserId },
						{ Constants.FOR_LOG_KEY_BRANCH_NO, userCreditCard.BranchNo.ToString() }
					});
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(creditCardInput.UserId, lastChanged, accessor);
			}

			return new Result
			{
				Success = success,
				BranchNo = userCreditCard.BranchNo,
				ErrorMessage = errorMessage,
			};
		}

		/// <summary>
		/// 登録（PayTg向け処理）
		/// </summary>
		/// <param name="creditCardInput">クレジットカード入力情報</param>
		/// <param name="siteKbn">サイト区分</param>
		/// <param name="setDispFlg">表示フラグを設定するか（定期用に新規登録するカードは非表示）</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか、新規発行枝番、エラーメッセージ</returns>
		public Result ExecForPayTg(
			UserCreditCardInput creditCardInput,
			SiteKbn siteKbn,
			bool setDispFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			var userCreditCard = ExecOnlySaveForPayTg(creditCardInput, lastChanged, UpdateHistoryAction.DoNotInsert);
			var userCreditCardModule = new UserCreditCard(userCreditCard);

			// クレジット情報登録(表示フラグを有効に)
			var updateDispFlgResult = true;
			if (setDispFlg)
			{
				updateDispFlgResult = userCreditCardModule.UpdateDispFlg(
					true,
					lastChanged,
					UpdateHistoryAction.DoNotInsert);

				// 失敗であれば不要なクレジット情報削除
				if (updateDispFlgResult == false)
				{
					userCreditCardModule.Delete(lastChanged, UpdateHistoryAction.DoNotInsert);
				}

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					updateDispFlgResult,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Unknown,
					PaymentFileLogger.PaymentProcessingType.RegistCreditCard,
					string.Format(
						"{0}:{1}",
						PaymentFileLogger.PaymentProcessingType.RegistCreditCard.ToText(),
						updateDispFlgResult ? "" : "(表示更新に失敗)"),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_USER_USER_ID, creditCardInput.UserId },
						{ Constants.FOR_LOG_KEY_BRANCH_NO, userCreditCardModule.BranchNo.ToString() }
					});
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(creditCardInput.UserId, lastChanged);
			}

			var errorMessage = updateDispFlgResult ? string.Empty : "クレジット情報の登録(表示フラグ更新に失敗)";
			return new Result
			{
				Success = updateDispFlgResult,
				BranchNo = userCreditCard.BranchNo,
				ErrorMessage = errorMessage,
			};
		}

		/// <summary>
		/// 保存だけ実行
		/// </summary>
		/// <param name="creditCardInput">クレジットカード入力</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴</param>
		/// <returns>ユーザークレジットカード</returns>
		public UserCreditCardModel ExecOnlySave(
			UserCreditCardInput creditCardInput,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				var userCreditCard = ExecOnlySave(creditCardInput, lastChanged, updateHistoryAction, accessor);
				return userCreditCard;
			}
		}
		/// <summary>
		/// 保存だけ実行
		/// </summary>
		/// <param name="creditCardInput">クレジットカード入力</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード</returns>
		public UserCreditCardModel ExecOnlySave(
			UserCreditCardInput creditCardInput,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// モデル生成・登録（連携IDはトークンに含まれていれば利用、非表示で登録）
			var userCreditCard = creditCardInput.CreateModel(
				(OrderCommon.CreditTokenUse && (creditCardInput.CreditToken != null)) ? creditCardInput.CreditToken.CooperationId1 : "",
				(OrderCommon.CreditTokenUse && (creditCardInput.CreditToken != null)) ? creditCardInput.CreditToken.CooperationId2 : "",
				false,
				lastChanged);
			// ペイジェントクレカの場合は連携ID1にペイジェント側の顧客IDとしてユーザーID+数字5桁を格納
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent)
			{
				userCreditCard.CooperationId = UserCreditCardCooperationInfoPaygent.CreatePaygentMemberId(creditCardInput.UserId);
			}
			var branchNo = new UserCreditCardService().Insert(userCreditCard, UpdateHistoryAction.DoNotInsert, accessor);
			creditCardInput.BranchNo = branchNo.ToString();

			// 連携IDが空であれば作成
			var userCreditCardCooperationInfo =
				UserCreditCardCooperationInfoFacade.CreateForAuth(creditCardInput.UserId, branchNo, accessor);
			if (string.IsNullOrEmpty(userCreditCard.CooperationId)) userCreditCard.CooperationId = userCreditCardCooperationInfo.CooperationId1;
			if (string.IsNullOrEmpty(userCreditCard.CooperationId2)) userCreditCard.CooperationId2 = userCreditCardCooperationInfo.CooperationId2;

			new UserCreditCardService().Modify(
				userCreditCard.UserId,
				userCreditCard.BranchNo,
				model =>
				{
					model.CooperationId = userCreditCard.CooperationId;
					model.CooperationId2 = userCreditCard.CooperationId2;
				},
				UpdateHistoryAction.DoNotInsert,
				accessor);

			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(creditCardInput.UserId, lastChanged, accessor);
			}

			return userCreditCard;
		}

		/// <summary>
		/// 保存だけ実行（payTg向け処理）
		/// </summary>
		/// <param name="creditCardInput">クレジットカード入力</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴</param>
		/// <returns>ユーザークレジットカード</returns>
		public UserCreditCardModel ExecOnlySaveForPayTg(
			UserCreditCardInput creditCardInput,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				var userCreditCard = UserCreditCardModelForPayTg(creditCardInput, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();

				return userCreditCard;
			}
		}
		/// <summary>
		/// 保存だけ実行（payTg向け処理）
		/// </summary>
		/// <param name="creditCardInput">クレジットカード入力</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴</param>
		/// <param name="accessor">SQLサクセス</param>
		/// <returns>ユーザークレジットカード</returns>
		public UserCreditCardModel ExecOnlySaveForPayTg(
			UserCreditCardInput creditCardInput,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var userCreditCard = UserCreditCardModelForPayTg(creditCardInput, lastChanged, updateHistoryAction, accessor);
			return userCreditCard;
		}

		/// <summary>
		/// ユーザークレジットモデル作成（payTg向け処理）
		/// </summary>
		/// <param name="creditCardInput">クレジットカード入力</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">SQLサクセス</param>
		/// <param name="accessor">SQLサクセス</param>
		/// <returns>ユーザークレジットモデル</returns>
		private static UserCreditCardModel UserCreditCardModelForPayTg(
			UserCreditCardInput creditCardInput,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var userCreditCard = creditCardInput.CreateModel(
				(creditCardInput.CreditToken != null) ? creditCardInput.CreditToken.CooperationId1 : "",
				(creditCardInput.CreditToken != null) ? creditCardInput.CreditToken.CooperationId2 : "",
				false,
				lastChanged);

			var branchNo = new UserCreditCardService().Insert(userCreditCard, UpdateHistoryAction.DoNotInsert, accessor);
			creditCardInput.BranchNo = branchNo.ToString();

			new UserCreditCardService().Modify(
				userCreditCard.UserId,
				userCreditCard.BranchNo,
				model =>
				{
					model.CooperationId = userCreditCard.CooperationId;
					model.CooperationId2 = userCreditCard.CooperationId2;
				},
				UpdateHistoryAction.DoNotInsert,
				accessor);

			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(creditCardInput.UserId, lastChanged, accessor);
			}

			return userCreditCard;
		}

		/// <summary>
		/// 外部連携実行
		/// </summary>
		/// <param name="creditCardInput">クレジットカード入力情報</param>
		/// <param name="userCreditCard">ユーザークレジットカードクラス</param>
		/// <param name="siteKbn">サイト区分</param>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか、エラーメッセージ</returns>
		private Tuple<bool, string> ExecExternalPayment(
			UserCreditCardInput creditCardInput,
			UserCreditCard userCreditCard,
			SiteKbn siteKbn,
			UserModel user,
			UpdateHistoryAction updateHistoryAction)
		{
			var mailAddr = ((siteKbn == SiteKbn.Mobile)
				? ((user.MailAddr2 != "") ? user.MailAddr2 : user.MailAddr)
				: ((user.MailAddr != "") ? user.MailAddr : user.MailAddr2));

			var success = false;
			var errorMessage = "";

			// ゼウスクレジットカード？
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
			{
				var result = new ZeusSecureLinkApi().ExecForRegisterCreditCard(
					creditCardInput.CreditToken.Token,
					userCreditCard.CooperationInfo.ZeusTelNo,
					mailAddr,
					userCreditCard.CooperationInfo.ZeusSendId);
				if (result.Success)
				{
					success = true;
				}
				else
				{
					// 発行されたトークンキーは複数回利用は出来ない仕様のため、一度失敗したら有効期限切れとする
					creditCardInput.CreditToken.SetTokneExpired();
					errorMessage = "決済エラー：" + result.ErrorMessage;
				}
			}
			// ソフトバンクペイメント？
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)
			{
				// トークン利用
				if (OrderCommon.CreditTokenUse)
				{
					var paymentSBPS = new PaymentSBPSCreditCustomerRegistWithTokenApi();
					if (paymentSBPS.Exec(
						userCreditCard.CooperationInfo.SBPSCustCode,
						creditCardInput.CreditToken))
					{
						success = true;
					}
					else
					{
						if (paymentSBPS.ResponseData.IsTokenExpired)
						{
							creditCardInput.CreditToken.ExpireDate = DateTime.Now.Date.AddYears(-1);
						}

						errorMessage = "決済エラー：" + LogCreator.CreateErrorMessage(
							paymentSBPS.ResponseData.ResErrCode,
							paymentSBPS.ResponseData.ResErrMessages);
					}
				}
				// それ以外は永久トークン利用
				else
				{
					var paymentSBPS = new PaymentSBPSCreditCustomerRegistWithTokenizedPanApi();
					if (paymentSBPS.Exec(
						userCreditCard.CooperationInfo.SBPSCustCode,
						creditCardInput.CardNo,
						creditCardInput.ExpirationYear,
						creditCardInput.ExpirationMonth))
					{
						success = true;
					}
					else
					{
						errorMessage = "決済エラー：" + LogCreator.CreateErrorMessage(
							paymentSBPS.ResponseData.ResErrCode,
							paymentSBPS.ResponseData.ResErrMessages);
					}
				}
			}
			// ヤマトKWC？
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc)
			{
				var deviceKbn = (siteKbn == SiteKbn.Pc)
					? PaymentYamatoKwcDeviceDiv.Pc
					: (siteKbn == SiteKbn.SmartPhone)
						? PaymentYamatoKwcDeviceDiv.SmartPhone
						: PaymentYamatoKwcDeviceDiv.Mobile;
				var mailAddress = (string.IsNullOrEmpty(user.MailAddr) ? Constants.PAYMENT_SETTING_YAMATO_KWC_DUMMY_MAILADDRESS : user.MailAddr);

				var yamatoApi = new PaymentYamatoKwcCreditAuthApi((creditCardInput.CreditToken.Token != null));
				var paymentOrderId = OrderCommon.CreatePaymentOrderId("0");
				var result = yamatoApi.Exec(
					deviceKbn,
					paymentOrderId,
					Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
					1,
					user.Name,
					user.Tel1,
					mailAddress,
					1,
					new PaymentYamatoKwcCreditOptionServiceParamOptionReg(
						userCreditCard.CompanyCode,
						creditCardInput.CreditToken.Token));

				// ログ格納処理用のdictionaryを生成
				var isExecSuccess = result.Success;
				PaymentFileLogger.WritePaymentLog(
					isExecSuccess,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Yamatokwc,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					isExecSuccess ? "" : "決済エラー：" + result.ErrorInfoForLog,
					new Dictionary<string, string>
					{
						{Constants.FIELD_USER_USER_ID, user.UserId},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId}
					});

				if (result.Success)
				{
					var yamatoCreditCancelApiResponse = new PaymentYamatoKwcCreditCancelApi().Exec(paymentOrderId);
					var isSuccessResponse = yamatoCreditCancelApiResponse.Success;

					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						isSuccessResponse,
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						PaymentFileLogger.PaymentType.Yamatokwc,
						PaymentFileLogger.PaymentProcessingType.OneYenAuthCancel,
						isSuccessResponse ? "" : LogCreator.CreateErrorMessage(
							yamatoCreditCancelApiResponse.ErrorCode,
							yamatoCreditCancelApiResponse.ErrorMessage),
						new Dictionary<string, string>
						{
							{Constants.FIELD_USER_USER_ID, user.UserId},
							{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId}
						});

					success = true;
				}
				else
				{
					errorMessage = "決済エラー：" + LogCreator.CreateErrorMessage(result.ErrorCode, result.ErrorMessage);
				}
			}
			// GMO？
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
			{
				// 会員登録＆カード登録
				var paymentGmo = new PaymentGmoCredit();
				success = paymentGmo.SaveMemberAndCard(
					userCreditCard.CooperationInfo.GMOMemberId,
					user.Name,
					creditCardInput.CreditToken.Token,
					userCreditCard.AuthorName);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					success,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.CooperationIdUpdate,
					success ? "" : "決済エラー：" + paymentGmo.ErrorMessages,
					new Dictionary<string, string>
					{
						{Constants.FIELD_USER_USER_ID, user.UserId},
						{Constants.FOR_LOG_KEY_BRANCH_NO, userCreditCard.BranchNo.ToString()},
						{Constants.FOR_LOG_KEY_FOR_GMO_ACCESS_ID, paymentGmo.AccessId},
						{Constants.FOR_LOG_KEY_FOR_GMO_MEMBER_ID, userCreditCard.CooperationInfo.GMOMemberId}
					});

				if (success)
				{
					// カード登録連番を連携IDに格納できたかどうか。
					var isRegisterToCooperationSuccess = userCreditCard.UpdateCooperationId(
						userCreditCard.CooperationInfo.GMOMemberId,
						Constants.FLG_LASTCHANGED_USER,
						updateHistoryAction);

					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						isRegisterToCooperationSuccess,
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						PaymentFileLogger.PaymentType.Gmo,
						PaymentFileLogger.PaymentProcessingType.CooperationIdUpdate,
						isRegisterToCooperationSuccess ? paymentGmo.ResultMessageCsvText : paymentGmo.ErrorMessages,
						new Dictionary<string, string>
						{
							{Constants.FIELD_USER_USER_ID, user.UserId},
							{Constants.FOR_LOG_KEY_BRANCH_NO, userCreditCard.BranchNo.ToString()},
							{Constants.FOR_LOG_KEY_FOR_GMO_ACCESS_ID, paymentGmo.AccessId},
							{Constants.FOR_LOG_KEY_FOR_GMO_MEMBER_ID, userCreditCard.CooperationInfo.GMOMemberId}
						});

					// カード登録連番を連携IDに格納
					if (isRegisterToCooperationSuccess == false)
					{
						success = false;
					}
				}
				else
				{
					errorMessage = "決済エラー：" + paymentGmo.ErrorMessages;
				}
			}
			// Zcom
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom)
			{
				// 1円決済をして取消し
				var registerAdp = new ZcomDirectRequestUserCreditCardAdapter(creditCardInput)
				{
					PaymentUserId = userCreditCard.CooperationId
				};
				var registerRes = registerAdp.Execute();

				// 実行成功したかどうか
				var isSuccessRegisterAdpExec = registerRes.IsSuccessResult();

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					isSuccessRegisterAdpExec,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Zcom,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					isSuccessRegisterAdpExec ? "" : "決済エラー：" + registerRes.GetResultValue(),
					new Dictionary<string, string>
					{
						{Constants.FIELD_USER_USER_ID, user.UserId},
						{"branch_no", userCreditCard.BranchNo.ToString()},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, registerAdp.PaymentOrderId}
					});

				if (isSuccessRegisterAdpExec)
				{
					// 1円決済うまくいったら取り消す
					var cancelAdp = new ZcomCancelRequestAdapter(registerAdp.PaymentOrderId);
					var cancelRes = cancelAdp.Execute();
					success = true;

					// 実行結果
					var cancelResResult = cancelRes.IsSuccessResult();

					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						cancelResResult,
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						PaymentFileLogger.PaymentType.Zcom,
						PaymentFileLogger.PaymentProcessingType.OneYenAuthCancel,
						cancelResResult ? "" : LogCreator.CreateErrorMessage(
							cancelRes.GetErrorCodeValue(),
							cancelRes.GetErrorDetailValue()),
						new Dictionary<string, string>
						{
							{Constants.FIELD_USER_USER_ID, user.UserId},
							{"branch_no", userCreditCard.BranchNo.ToString()},
							{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, registerAdp.PaymentOrderId}
						});

					if (cancelResResult == false)
					{
						errorMessage = "決済エラー:" + LogCreator.CreateErrorMessage(
							cancelRes.GetErrorCodeValue(),
							cancelRes.GetErrorDetailValue());
					}
				}
				else
				{
					errorMessage = "決済エラー：" + LogCreator.CreateErrorMessage(
						registerRes.GetErrorCodeValue(),
						registerRes.GetErrorDetailValue());
				}
			}
			// e-SCOTT
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott)
			{
				var registerAdp = EScottMember4MemAddApi.CreateEScottMember4MemAddApiByCreditCard(userCreditCard, creditCardInput.CreditToken.Token);
				var registerRes = registerAdp.ExecRequest();

				// 実行成功したかどうか
				success = registerRes.IsSuccess;
				errorMessage = success ? "" : "決済エラー：" + registerRes.ResponseMessage;

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					success,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.EScott,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					errorMessage,
					new Dictionary<string, string>
					{
						{ Constants.FIELD_USER_USER_ID, user.UserId },
						{ "branch_no", userCreditCard.BranchNo.ToString() },
					});
			}
			// ベリトランス
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
			{
				// 会員登録とカード登録
				var accountManager = new AccountManager();
				var response = accountManager.AddCardByUser(userCreditCard.CooperationId, creditCardInput.CreditToken.Token);
				if (response.Mstatus == "success")
				{
					success = true;
				}
				else
				{
					errorMessage = "決済エラー：" + LogCreator.CreateErrorMessage(
						response.VResultCode,
						response.MerrMsg);
				}

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					success,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.VeriTrans,
					PaymentFileLogger.PaymentProcessingType.VeritransMembersAndCardRegistration,
					success ? "" : LogCreator.CreateErrorMessage(
						response.VResultCode,
						response.MerrMsg),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_USERCREDITCARD_COOPERATION_ID, userCreditCard.CooperationId },
						{ Constants.FIELD_USERCREDITCARD_BRANCH_NO, userCreditCard.BranchNo.ToString() }
					});
			}
			// Rakuten payment
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
			{
				var context = HttpContext.Current;
				var ipAddress = (context != null) ? context.Request.ServerVariables["REMOTE_ADDR"] : "";

				var paymentOrderId = OrderCommon.CreatePaymentOrderId("0");
				var rakutenAuthourizeRequest = new RakutenAuthorizeRequest(ipAddress)
				{
					PaymentId = paymentOrderId,
					GrossAmount = 0m,
					CardToken = new CardTokenBase
					{
						Amount = "0",
						CardToken = creditCardInput.CreditToken.Token,
						CvvToken = creditCardInput.CreditCvvToken.Token,
					},
				};
				// Authorize APi
				var apiResponse = RakutenApiFacade.AuthorizeAPI(rakutenAuthourizeRequest);
				success = (apiResponse.ResultType == RakutenConstants.RESULT_TYPE_SUCCESS);
			}
			// ペイジェント
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent)
			{
				// カード登録APIをインスタンス化
				var request = new PaygentCreditCardRegisterApi
				{
					CustomerId = userCreditCard.CooperationId,
					CardToken = creditCardInput.CreditToken.Token,
				};
				// API送信
				var result = PaygentApiFacade.SendRequest(request);
				// 連携処理成功後UpdateするのでSessionに一時格納
				SessionManager.UserCreditCooperationId2 = (string)result[PaygentConstants.CUSTOMER_CARD_ID];
				// 登録結果とエラーメッセージを返す
				success = (string)result[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS;
				errorMessage = (string)result[PaygentConstants.RESPONSE_DETAIL];
			}

			return new Tuple<bool, string>(success, errorMessage);
		}

		/// <summary>
		/// モバイル版でのみ使用、クレジットカード仮登録
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>枝番</returns>
		public Result ExecProvisionalRegistration(string userId, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = ExecProvisionalRegistration(userId, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return result;
			}
		}
		/// <summary>
		/// モバイル版でのみ使用、クレジットカード仮登録
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>枝番</returns>
		public Result ExecProvisionalRegistration(string userId, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var userCreditCardCooperationInfo = UserCreditCardCooperationInfoFacade.CreateForAuth(userId, 0, accessor);	// 枝番使うのはGMOのみだがGMOはモバイル対応していない

			// モデル生成・登録（まずは非表示で登録）
			var userCreditCard = new UserCreditCardModel
			{
				UserId = userId,
				CooperationId = userCreditCardCooperationInfo.CooperationId1,
				CooperationId2 = userCreditCardCooperationInfo.CooperationId2,
				CardDispName = "",
				LastFourDigit = "",
				ExpirationMonth = "",
				ExpirationYear = "",
				AuthorName = "",
				DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
				LastChanged = Constants.FLG_LASTCHANGED_USER,
				CompanyCode = "",
			};

			// 実行
			var branchNo = new UserCreditCardService().Insert(userCreditCard, updateHistoryAction, accessor);
			// 枝番返却
			return new Result
			{
				BranchNo = branchNo,
			};
		}

		/// <summary>
		/// カード情報をロールバック（削除するだけ）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>結果(成功/失敗)</returns>
		public bool RollbackExecProvisionalRegistration(string userId, int branchNo, UpdateHistoryAction updateHistoryAction)
		{
			var userCreditCard = new UserCreditCardModel
			{
				UserId = userId,
				BranchNo = branchNo
			};

			bool result = new UserCreditCard(userCreditCard).Delete(
				Constants.FLG_LASTCHANGED_USER,
				updateHistoryAction);
			return result;
		}

		/// <summary>
		/// モバイル版でのみ使用、クレジットカード情報更新（まだ仮登録）
		/// </summary>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>実行結果</returns>
		public Result ExecProvisionalRegistration2(
			UserCreditCardModel userCreditCard,
			UpdateHistoryAction updateHistoryAction)
		{
			// 現在登録されているクレジットカード情報を取得し、更新しない情報を埋める
			var registeredUserCreditCard = new UserCreditCardService().Get(userCreditCard.UserId, userCreditCard.BranchNo);
			registeredUserCreditCard.LastFourDigit = userCreditCard.LastFourDigit;
			registeredUserCreditCard.ExpirationMonth = userCreditCard.ExpirationMonth;
			registeredUserCreditCard.ExpirationYear =
				userCreditCard.ExpirationYear.Substring(userCreditCard.ExpirationYear.Length - 2, 2);
			registeredUserCreditCard.AuthorName = userCreditCard.AuthorName;
			registeredUserCreditCard.CompanyCode = userCreditCard.CompanyCode;
			registeredUserCreditCard.DispFlg = userCreditCard.DispFlg;

			// 更新実行
			new UserCreditCardService().Update(registeredUserCreditCard, updateHistoryAction);

			return new Result
			{
				Success = true,
			};
		}

		/// <summary>
		/// モバイル版でのみ使用、クレジットカード情報更新（まだ仮登録）
		/// </summary>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>実行結果</returns>
		public Result ExecProvisionalRegistration2(
			UserCreditCardModel userCreditCard,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 現在登録されているクレジットカード情報を取得し、更新しない情報を埋める
			var registeredUserCreditCard = new UserCreditCardService().Get(
				userCreditCard.UserId,
				userCreditCard.BranchNo,
				accessor);
			registeredUserCreditCard.LastFourDigit = userCreditCard.LastFourDigit;
			registeredUserCreditCard.ExpirationMonth = userCreditCard.ExpirationMonth;
			registeredUserCreditCard.ExpirationYear = userCreditCard.ExpirationYear.Substring(userCreditCard.ExpirationYear.Length - 2, 2);
			registeredUserCreditCard.AuthorName = userCreditCard.AuthorName;
			registeredUserCreditCard.CompanyCode = userCreditCard.CompanyCode;
			registeredUserCreditCard.DispFlg = userCreditCard.DispFlg;

			// 更新実行
			new UserCreditCardService().Update(registeredUserCreditCard, updateHistoryAction, accessor);
			return new Result
			{
				Success = true,
			};
		}
		/// <summary>
		/// モバイル版でのみ使用、クレジットカード情報更新（本登録）
		/// </summary>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void ExecRegistrationComplete(UserCreditCardModel userCreditCard, UpdateHistoryAction updateHistoryAction)
		{
			// 現在登録されているクレジットカード情報を取得し、更新しない情報を埋める
			var registeredUserCreditCard = new UserCreditCardService().Get(userCreditCard.UserId, userCreditCard.BranchNo);
			registeredUserCreditCard.CardDispName = userCreditCard.CardDispName;
			registeredUserCreditCard.LastChanged = Constants.FLG_LASTCHANGED_USER;
			registeredUserCreditCard.DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_ON;

			// 更新実行
			new UserCreditCardService().Update(
				registeredUserCreditCard,
				updateHistoryAction);
		}
		/// <summary>カード登録用ペイジェント側顧客ID</summary>
		private static string PaygentCustomerId { get; set; }
	}

	/// <summary>
	/// 結果
	/// </summary>
	public class Result
	{
		/// <summary>成功したか</summary>
		public bool Success { get; set; }
		/// <summary>新規発行枝番</summary>
		public int BranchNo { get; set; }
		/// <summary>（主に決済の）エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}
