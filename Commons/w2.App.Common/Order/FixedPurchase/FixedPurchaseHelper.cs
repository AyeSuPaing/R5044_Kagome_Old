/*
=========================================================================================================
  Module      : 定期購入のヘルパークラス (FixedPurchaseHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Mail;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.FixedPurchaseForecast;
using w2.Domain.Point;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Order.FixedPurchase
{
	/// <summary>
	/// 定期購入のヘルパークラス
	/// </summary>
	public class FixedPurchaseHelper
	{
		/// <summary>
		/// <para>定期購入の有効/無効 確認。有効な（退会していない）ユーザの定期注文以外は無効</para>
		/// <para>無効であれば無効化する</para>
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>無効な定期購入</returns>
		public static bool CheckFixedPurchaseIsInvalidAndInvalidateFixedPurchase(FixedPurchaseModel fixedPurchase, string lastChanged)
		{
			var user = new UserService().Get(fixedPurchase.UserId);
			if ((user == null) || user.IsDeleted)
			{
				new FixedPurchaseService().UpdateInvalidate(
					fixedPurchase.FixedPurchaseId,
					lastChanged,
					UpdateHistoryAction.Insert);
				return true;
			}
			return false;
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="fixedPurchaseId">定期注文ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		public static void SendMail(string fixedPurchaseId, string mailId)
		{
			var dataSendMail = new MailTemplateDataCreaterForFixedPurchase(true).GetFixedPurchaseMailDatas(fixedPurchaseId);
			var pcMailAddr = (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
			var mobileMailAddr = (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2];

			var dispLanguageCode = string.Empty;
			var dispLanguageLocaleId = string.Empty;
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var user = new UserService().Get((string)dataSendMail[Constants.FIELD_FIXEDPURCHASE_USER_ID]);
				dispLanguageCode = user.DispLanguageCode;
				dispLanguageLocaleId = user.DispLanguageLocaleId;
			}

			// メールの送信対象がモバイル・PCの両方なら両方に送信、片方ならPCアドレスまたはモバイルアドレスに送信する。
			if ((Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED) || (string.IsNullOrEmpty(pcMailAddr) == false))
			{
				SendMailProcess(dataSendMail, mailId, pcMailAddr, dispLanguageCode, dispLanguageLocaleId);
			}
			if ((Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED) || (string.IsNullOrEmpty(pcMailAddr)))
			{
				SendMailProcess(dataSendMail, mailId, mobileMailAddr);
			}
		}

		/// <summary>
		/// メール送信の内部処理
		/// </summary>
		/// <param name="dataSendMail">メール配信内容のプロパティ群</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="mailAddr">送信メールアドレス</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="langugeLocaleId">言語ロケールID</param>
		private static void SendMailProcess(
			Hashtable dataSendMail,
			string mailId,
			string mailAddr,
			string languageCode = null,
			string langugeLocaleId = null)
		{
			if (string.IsNullOrEmpty(mailAddr)) return;

			// ユーザ対象のメールを送信
			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				mailId,
				(string)dataSendMail[Constants.FIELD_ORDER_USER_ID],
				dataSendMail,
				(bool)dataSendMail["is_pc"],
				Constants.MailSendMethod.Auto,
				languageCode,
				langugeLocaleId,
				StringUtility.ToEmpty(mailAddr)))
			{
				mailSender.AddTo(StringUtility.ToEmpty(mailAddr));
				if (mailSender.SendMail() == false)
				{
					throw new Exception("ユーザ対象のメール送信処理に失敗しました。", mailSender.MailSendException);
				}
			}
		}

		/// <summary>
		/// 利用可能なクーポンチェックおよび取得
		/// </summary>
		/// <param name="couponCode">クーポンコード</param>
		/// <param name="coupons">クーポン情報配列</param>
		/// <param name="fixedPurchaseContainer">定期台帳情報</param>
		/// <param name="cartList">カートリスト情報</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>クーポン情報</returns>
		public static UserCouponDetailInfo CheckAndGetUseCouponForNextShipping(
			string couponCode,
			UserCouponDetailInfo[] coupons,
			FixedPurchaseContainer fixedPurchaseContainer,
			CartObjectList cartList,
			out string errorMessage)
		{
			errorMessage = string.Empty;
			if (coupons.Length == 0)
			{
				errorMessage = CouponOptionUtility.GetErrorMessage(CouponErrorcode.NoCouponError)
					.Replace("@@ 1 @@", couponCode);
				return null;
			}

			// 同じクーポンを指定する場合、エラーメッセージを表示する
			if ((string.IsNullOrEmpty(couponCode) == false)
					&& (coupons[0].CouponId == fixedPurchaseContainer.NextShippingUseCouponId)
					&& ((coupons[0].CouponNo.HasValue == false)
						|| (coupons[0].CouponNo.Value == fixedPurchaseContainer.NextShippingUseCouponNo)))
			{
				errorMessage = CommerceMessages.GetMessages(
					CommerceMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_NO_CHANGE_ERROR);
				return null;
			}

			// クーポン利用可能チェック（回数制限、未使用チェック）
			var couponErrorCode = CouponErrorcode.CouponUsedError;
			foreach (var coupon in coupons)
			{
				// 利用可能なクーポンが１個でも見つかれば次の処理へ
				couponErrorCode = CouponOptionUtility.CheckUseCoupon(
					coupon,
					fixedPurchaseContainer.UserId,
					fixedPurchaseContainer.OwnerMailAddr);
				if (couponErrorCode == CouponErrorcode.NoError)
				{
					break;
				}
			}
			if (couponErrorCode != CouponErrorcode.NoError)
			{
				errorMessage = CouponOptionUtility.GetErrorMessage(couponErrorCode)
					.Replace("@@ 1 @@", couponCode);
				return null;
			}

			// クーポン有効性チェック（カートリストがNULLの場合、空のカートでチェック）
			var cart = (cartList != null) ? cartList.Items[0] : new CartObject("", "", "", "", "", false, false);
			errorMessage = CouponOptionUtility.CheckCouponValidWithCart(cart, coupons[0])
				.Replace("@@ 1 @@", couponCode);

			return string.IsNullOrEmpty(errorMessage) ? coupons.First() : null;
		}

		/// <summary>
		/// 次回購入の利用クーポン変更
		/// </summary>
		/// <param name="fixedPurchaseContainer">定期台帳情報</param>
		/// <param name="inputCoupon">クーポン情報</param>
		/// <param name="isNotResetCoupon">次回購入利用クーポンリセットではないか</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新結果</returns>
		public static bool ChangeNextShippingUseCoupon(
			FixedPurchaseContainer fixedPurchaseContainer,
			UserCouponDetailInfo inputCoupon,
			bool isNotResetCoupon,
			string lastChanged)
		{
			bool result;
			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();
				try
				{
					// 次回購入利用クーポンを更新
					result = new FixedPurchaseService().UpdateNextShippingUseCoupon(
						fixedPurchaseContainer.FixedPurchaseId,
						isNotResetCoupon ? inputCoupon.CouponId : FixedPurchaseModel.DEFAULT_NEXT_SHIPPING_USE_COUPON_ID,
						isNotResetCoupon ? inputCoupon.CouponNo : FixedPurchaseModel.DEFAULT_NEXT_SHIPPING_USE_COUPON_NO,
						lastChanged,
						UpdateHistoryAction.Insert,
						accessor);

					var couponService = new CouponService();
					// 変更したクーポンを適用（利用可能回数を減らす／利用済みにする）
					if (result && (inputCoupon != null))
					{
						result = couponService.ApplyNextShippingUseCouponToFixedPurchase(
							inputCoupon,
							fixedPurchaseContainer.UserId,
							fixedPurchaseContainer.FixedPurchaseId,
							(Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
								? fixedPurchaseContainer.OwnerMailAddr
								: fixedPurchaseContainer.UserId,
							lastChanged,
							UpdateHistoryAction.Insert,
							accessor);
					}

					// 既存の利用クーポンがあれば、戻す処理を行う
					if (result && (fixedPurchaseContainer.NextShippingUseCouponDetail != null))
					{
						result = couponService.ReturnNextShippingUseCoupon(
							fixedPurchaseContainer.NextShippingUseCouponDetail,
							fixedPurchaseContainer.UserId,
							"",
							fixedPurchaseContainer.FixedPurchaseId,
							lastChanged,
							Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE_CANCEL,
							UpdateHistoryAction.Insert,
							accessor);
					}

					if (result)
					{
						// トランザクションコミット
						accessor.CommitTransaction();
					}
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					result = false;
				}
			}

			return result;
		}

		/// <summary>
		/// 継続課金（定期・従量）の解約処理
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="orderPaymentKbn">決済区分</param>
		/// <param name="externalPaymentAgreementId">外部決済契約ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>API実行結果</returns>
		public static bool CancelPaymentContinuous(
			string fixedPurchaseId,
			string orderPaymentKbn,
			string externalPaymentAgreementId,
			string lastChanged,
			out string errorMessage,
			SqlAccessor accessor = null)
		{
			errorMessage = string.Empty;
			bool result;
			switch (orderPaymentKbn)
			{
				// SBPS auかんたん決済
				case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
					result = CancelSbpsAuKantanContinuous(
						externalPaymentAgreementId,
						out errorMessage);
					break;

				// SBPSドコモケータイ支払い
				case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
					result = CancelSbpsDocomoKeitaiContinuous(
						externalPaymentAgreementId,
						out errorMessage);
					break;

				// Paypay
				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
					result = CancelPaypayContinuous(
						fixedPurchaseId,
						externalPaymentAgreementId,
						out errorMessage);
					break;

				default:
					return true;
			}

			if (result == false) return false;

			// 継続課金解約した時には管理用IDを空に変更（更新履歴とともに）
			new FixedPurchaseService().SetExternalPaymentAgreementId(
				fixedPurchaseId,
				string.Empty,
				lastChanged,
				UpdateHistoryAction.Insert,
				accessor,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CONTINUOUS_ORDER_CANCEL);

			return true;
		}

		/// <summary>
		/// SBPS auかんたん決済の継続課金解約処理
		/// </summary>
		/// <param name="trackingId">トラキングID</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>API実行結果</returns>
		private static bool CancelSbpsAuKantanContinuous(
			string trackingId,
			out string errorMessage)
		{
			errorMessage = string.Empty;
			var api = new PaymentSBPSCareerAuKantanContinuousCancelApi();
			var result = api.Exec(trackingId);
			if (result == false) errorMessage = api.ResponseData.ResErrMessages;
			return result;
		}

		/// <summary>
		/// SBPSドコモケータイ支払いの継続課金解約処理
		/// </summary>
		/// <param name="trackingId">トラキングID</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>API実行結果</returns>
		private static bool CancelSbpsDocomoKeitaiContinuous(
			string trackingId,
			out string errorMessage)
		{
			errorMessage = string.Empty;
			var api = new PaymentSBPSCareerDocomoKetaiContinuousCancelApi();
			var result = api.Exec(trackingId);
			if (result == false) errorMessage = api.ResponseData.ResErrMessages;
			return result;
		}

		/// <summary>
		/// Cancel Paypay continuous
		/// </summary>
		/// <param name="fixedPurchaseId">Fixed purchase id</param>
		/// <param name="externalPaymentAgreementId">External payment agreement id</param>
		/// <param name="errorMessage">Error message</param>
		/// <returns>Api execution result</returns>
		private static bool CancelPaypayContinuous(
			string fixedPurchaseId,
			string externalPaymentAgreementId,
			out string errorMessage)
		{
			var fixedPurchase = DomainFacade.Instance.FixedPurchaseService.Get(fixedPurchaseId);
			var result = true;
			errorMessage = string.Empty;
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case Constants.PaymentPayPayKbn.GMO:
					var gmoResponse = new PaypayGmoFacade().AcceptEndPayment(fixedPurchase);
					if (gmoResponse.Result == Results.Failed) errorMessage = gmoResponse.ErrorMessage;
					result = gmoResponse.Result == Results.Success;
					break;

				case Constants.PaymentPayPayKbn.VeriTrans:
					var veritransResult = new PaymentVeritransPaypay().Terminate(fixedPurchaseId);
					if (veritransResult.Mstatus != VeriTransConst.RESULT_STATUS_OK)
					{
						errorMessage = veritransResult.MerrMsg;
						result = false;
					}
					break;

				default:
					result = false;
					break;
			}
			return result;
		}

		/// <summary>
		/// 割引額が次回利用額を越えていないか判定
		/// </summary>
		/// <param name="inputCoupon">ユーザークーポン情報</param>
		/// <param name="newPoint">更新後ポイント数</param>
		/// <param name="priceSubtotalForCampaign">クーポン利用可能額</param>
		/// <returns>結果</returns>
		public static bool CheckDiscountableForNextFixedPurchase(UserCouponDetailInfo inputCoupon, decimal newPoint, decimal priceSubtotalForCampaign)
		{
			if (newPoint == 0) return true;

			var couponDiscountAmount = (inputCoupon.DiscountPrice == null)
				? (priceSubtotalForCampaign * (inputCoupon.DiscountRate / 100))
				: inputCoupon.DiscountPrice;
			var result = priceSubtotalForCampaign >= (newPoint + couponDiscountAmount);

			return result;
		}

		/// <summary>
		/// CrossPointAPIによるユーザーポイントの更新
		/// </summary>
		/// <param name="fixedPurchaseInfo">定期購入情報</param>
		/// <param name="user">ユーザー情報</param>
		/// <returns>エラーメッセージ</returns>
		public static string UpdateCrossPointApiUserPoint(FixedPurchaseContainer fixedPurchaseInfo, UserModel user)
		{
			var pointInfo = new PointService().GetUserPoint(
			fixedPurchaseInfo.UserId,
			cartId: string.Empty)
				.Where(
					userPoint => (userPoint.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE)
						&& (userPoint.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
				.ToArray().FirstOrDefault();

			if ((pointInfo != null)
				&& (pointInfo.PointExp >= DateTime.Now))
			{
				var errorMessage = CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
					user,
					fixedPurchaseInfo.NextShippingUsePoint,
					CrossPointUtility.GetValue(
						Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
						Constants.CROSS_POINT_REASON_KBN_MODIFY));

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					return errorMessage;
				}
			}

			return string.Empty;
		}

		#region +Aggregate 集計実行
		/// <summary>
		/// 集計実行
		/// </summary>
		/// <param name="lastExecDate">最終実行日</param>
		/// <param name="nextShippingCalculationMode">次回配送日計算モード</param>
		/// <param name="taxExcludedFractionRounding">税処理金額端数処理方法</param>
		/// <param name="managementIncludedTaxFlag">税込み管理フラグ</param>
		/// <param name="currencyLocaleId">通貨ロケールID</param>
		/// <param name="currencyDecimalDigits">通貨の小数点以下の有効桁数</param>
		public void Aggregate(
			DateTime lastExecDate,
			NextShippingCalculationMode nextShippingCalculationMode,
			string taxExcludedFractionRounding,
			bool managementIncludedTaxFlag,
			string currencyLocaleId,
			int? currencyDecimalDigits)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var isTotalTarget = (DateTime.Now.ToString("yyyy/MM") != lastExecDate.ToString("yyyy/MM"));

				var count = new FixedPurchaseForecastService().GetCount(accessor);
				if ((count == 0) || isTotalTarget)
				{
					// 初回と月が替わったタイミングで全集計
					TotalAggregate(
						nextShippingCalculationMode,
						taxExcludedFractionRounding,
						managementIncludedTaxFlag,
						currencyLocaleId,
						currencyDecimalDigits,
						accessor);
				}
				else
				{
					DifferenceAggregate(
						nextShippingCalculationMode,
						taxExcludedFractionRounding,
						managementIncludedTaxFlag,
						currencyLocaleId,
						currencyDecimalDigits,
						lastExecDate,
						accessor);
				}
				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +TotalAggregate 全集計
		/// <summary>
		/// 全集計
		/// </summary>
		/// <param name="nextShippingCalculationMode">次回配送日計算モード</param>
		/// <param name="taxExcludedFractionRounding">税処理金額端数処理方法</param>
		/// <param name="managementIncludedTaxFlag">税込み管理フラグ</param>
		/// <param name="currencyLocaleId">通貨ロケールID</param>
		/// <param name="currencyDecimalDigits">通貨の小数点以下の有効桁数</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void TotalAggregate(
			NextShippingCalculationMode nextShippingCalculationMode,
			string taxExcludedFractionRounding,
			bool managementIncludedTaxFlag,
			string currencyLocaleId,
			int? currencyDecimalDigits,
			SqlAccessor accessor)
		{
			new FixedPurchaseForecastService().DeleteAll(accessor);
			var models = new FixedPurchaseService().GetTargetsForForecastAggregate(accessor);
			foreach (var model in models)
			{
				CalculateEachMonthSalesAndStock(
					model,
					true,
					nextShippingCalculationMode,
					taxExcludedFractionRounding,
					managementIncludedTaxFlag,
					currencyLocaleId,
					currencyDecimalDigits,
					accessor);
			}
		}
		#endregion

		#region +DifferenceAggregate 差分集計
		/// <summary>
		/// DifferenceAggregate
		/// </summary>
		/// <param name="nextShippingCalculationMode">次回配送日計算モード</param>
		/// <param name="taxExcludedFractionRounding">税処理金額端数処理方法</param>
		/// <param name="managementIncludedTaxFlag">税込み管理フラグ</param>
		/// <param name="currencyLocaleId">通貨ロケールID</param>
		/// <param name="currencyDecimalDigits">通貨の小数点以下の有効桁数</param>
		/// <param name="lastExecDate">最終実行日</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void DifferenceAggregate(
			NextShippingCalculationMode nextShippingCalculationMode,
			string taxExcludedFractionRounding,
			bool managementIncludedTaxFlag,
			string currencyLocaleId,
			int? currencyDecimalDigits,
			DateTime lastExecDate,
			SqlAccessor accessor)
		{
			var models = new FixedPurchaseService().GetTargetsForForecastAggregate(lastExecDate, accessor);
			foreach (var model in models)
			{
				CalculateEachMonthSalesAndStock(
					model,
					false,
					nextShippingCalculationMode,
					taxExcludedFractionRounding,
					managementIncludedTaxFlag,
					currencyLocaleId,
					currencyDecimalDigits,
					accessor);
			}
		}
		#endregion

		#region -CalculateEachMonthSalesAndStock 各月の金額と個数を集計
		/// <summary>
		/// 各月の金額と個数を集計
		/// </summary>
		/// <param name="model">定期</param>
		/// <param name="isTotalAggregation">全集計か</param>
		/// <param name="nextShippingCalculationMode">全集計か</param>
		/// <param name="taxExcludedFractionRounding">税処理金額端数処理方法</param>
		/// <param name="managementIncludedTaxFlag">税込み管理フラグ</param>
		/// <param name="currencyLocaleId">通貨ロケールID</param>
		/// <param name="currencyDecimalDigits">通貨の小数点以下の有効桁数</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void CalculateEachMonthSalesAndStock(
			FixedPurchaseModel model,
			bool isTotalAggregation,
			NextShippingCalculationMode nextShippingCalculationMode,
			string taxExcludedFractionRounding,
			bool managementIncludedTaxFlag,
			string currencyLocaleId,
			int? currencyDecimalDigits,
			SqlAccessor accessor)
		{
			if (string.IsNullOrEmpty(model.Shippings[0].Items[0].ProductId)) return;

			var userMemberRabkId = new UserService().GetMemberRankId(model.UserId, accessor);
			if (userMemberRabkId == null) return;

			var products = new List<ProductModel>();
			if (model.SubscriptionBoxFixedAmount == null)
			{
				products.AddRange(
					model.Shippings[0].Items
						.Select(
							item => new ProductService().GetProductVariation(
								item.ShopId,
								item.ProductId,
								item.VariationId,
								userMemberRabkId,
								accessor))
						.Where(product => product != null));
			}

			if (products.Count == 0) return;
			var shippingType = new ShopShippingService().GetOnlyModel(Constants.CONST_DEFAULT_SHOP_ID, products[0].ShippingType, accessor);
			var eachMonthDeliveryFrequency =
				new FixedPurchaseForecastService().CalculateToSixMonthEachMonthDeliveryFrequency(
					model.FixedPurchaseKbn,
					model.FixedPurchaseSetting1,
					model.NextShippingDate,
					model.NextNextShippingDate,
					shippingType.FixedPurchaseShippingDaysRequired,
					shippingType.FixedPurchaseMinimumShippingSpan,
					nextShippingCalculationMode,
					DateTime.Now);
			var shippingDate = new FixedPurchaseRepository().GetShippingAll(model.FixedPurchaseId);
			var nextNextScheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
						model.ShopId,
						model.NextNextShippingDate,
						string.Empty,
						shippingDate[0].DeliveryCompanyId,
						shippingDate[0].ShippingCountryIsoCode,
						shippingDate[0].ShippingAddr1,
						shippingDate[0].ShippingZip,
						false);

			var nextScheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
						model.ShopId,
						model.NextShippingDate,
						string.Empty,
						shippingDate[0].DeliveryCompanyId,
						shippingDate[0].ShippingCountryIsoCode,
						shippingDate[0].ShippingAddr1,
						shippingDate[0].ShippingZip,
						false);

			var eachMonthDeliveryFrequencyForShippingDate =
				new FixedPurchaseForecastService().CalculateToSixMonthEachMonthDeliveryFrequency(
					model.FixedPurchaseKbn,
					model.FixedPurchaseSetting1,
					nextScheduledShippingDate,
					nextNextScheduledShippingDate,
					shippingType.FixedPurchaseShippingDaysRequired,
					shippingType.FixedPurchaseMinimumShippingSpan,
					nextShippingCalculationMode,
					DateTime.Now);

			if (isTotalAggregation == false)
			{
				new FixedPurchaseForecastService().DeleteTargeFixedPurechaseId(model.ShopId, model.FixedPurchaseId, accessor);
			}

			for (var i = 0; i < products.Count; i++)
			{
				new FixedPurchaseForecastService().InsertProductVariation(
					model.FixedPurchaseId,
					products[i],
					model.Shippings[0].Items[i].ItemQuantity,
					eachMonthDeliveryFrequency,
					taxExcludedFractionRounding,
					managementIncludedTaxFlag,
					currencyLocaleId,
					currencyDecimalDigits,
					eachMonthDeliveryFrequencyForShippingDate,
					accessor
				);
			}
		}
		#endregion
	}
}
