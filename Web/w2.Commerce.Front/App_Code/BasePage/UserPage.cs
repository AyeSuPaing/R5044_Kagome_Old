/*
=========================================================================================================
  Module      : ユーザー系基底ページ(UserPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.CrossPoint.User;
using w2.App.Common;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order;
using w2.Common.Util;
using w2.Domain.Coupon;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using System.Linq;

///*********************************************************************************************
/// <summary>
/// ユーザー系基底ページ
/// </summary>
///*********************************************************************************************
public class UserPage : BasePage
{
	/// <summary>重複チェック</summary>
	private const string CHECK_DUPLICATION = "CHECK_DUPLICATION";

	/// <summary>
	///  登録時クーポン発行
	/// </summary>
	/// <param name="user">ユーザ情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="userIdOfReferralCode">User id of referral code</param>
	/// <param name="isRegister">新規登録ユーザーであるかどうか</param>
	/// <returns>発行済みクーポン情報</returns>
	protected string PublishCouponAtRegist(
		UserModel user,
		UpdateHistoryAction updateHistoryAction,
		string userIdOfReferralCode,
		bool isRegister = false)
	{
		var blSuccess = false;

		// 発行対象のクーポン情報取得
		// ※「新規登録で発行する会員用クーポン」のみ
		var publishCoupon = 1;
		var sbPublishCoupons = new StringBuilder();
		var couponService = new CouponService();

		var coupons = couponService.GetPublishCouponsByCouponType(
			Constants.W2MP_DEPT_ID,
			isRegister
				? Constants.FLG_COUPONCOUPON_TYPE_USERREGIST
				: Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_BY_INTRODUCED_PERSON_AFTER_MEMBERSHIP_REGISTRATION);
		
		if (isRegister && (string.IsNullOrEmpty(userIdOfReferralCode) == false))
		{
			var couponPurchaseGiveToIntroducedUser = couponService.GetPublishCouponsByCouponType(
				Constants.W2MP_DEPT_ID, 
				Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON); 
			coupons = coupons.Concat(couponPurchaseGiveToIntroducedUser).ToArray();
		}
		var userId = isRegister
			? user.UserId
			: userIdOfReferralCode;

		foreach (var coupon in coupons)
		{
			// 新規会員登録クーポン発行
			using (var sqlAccessor = new SqlAccessor())
			{
				// トランザクション開始
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				// ユーザクーポン情報登録
				blSuccess = couponService.InsertUserCouponWithOrderId(
					userId,
					"",
					coupon.DeptId,
					coupon.CouponId,
					user.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor);
				if (blSuccess)
				{
					// ユーザクーポン履歴情報登録
					blSuccess = couponService.InsertUserCouponHistory(
						userId,
						"",
						coupon.DeptId,
						coupon.CouponId,
						coupon.CouponCode,
						Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH,
						Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_BASE,
						1,
						coupon.DiscountPrice.GetValueOrDefault(),
						Constants.FLG_LASTCHANGED_USER,
						sqlAccessor);
				}

				if (blSuccess)
				{
					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertForUser(userId, Constants.FLG_LASTCHANGED_USER, sqlAccessor);
					}

					// トランザクションコミット
					sqlAccessor.CommitTransaction();

					sbPublishCoupons.Append("-(").Append(publishCoupon).Append(")--------------------").Append("\r\n");
					sbPublishCoupons.Append(ReplaceTagByLocaleId("@@MailTemplate.coupon_code.name@@", user.DispLanguageLocaleId)).Append(coupon.CouponCode).Append("\r\n");
					sbPublishCoupons.Append(ReplaceTagByLocaleId("@@MailTemplate.coupon_name.name@@", user.DispLanguageLocaleId)).Append(coupon.CouponDispName).Append("\r\n");
					publishCoupon++;
				}
				else
				{
					// トランザクションロールバック
					sqlAccessor.RollbackTransaction();

					// ログ出力(念のため、ログ出力)
					AppLogger.WriteError("新規会員登録クーポン発行失敗：[user_id=" + userId + ",dept_id=" + coupon.DeptId + ",coupon_id=" + coupon.CouponId + "]\r\n");
				}
			}
		}

		return sbPublishCoupons.ToString();
	}

	/// <summary>
	/// 新規登録時ポイントの発行
	/// </summary>
	/// <param name="user">ユーザ情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>発行済みポイント</returns>
	protected string PublishPointAtRegist(UserModel user, UpdateHistoryAction updateHistoryAction)
	{
		var pointRules = PointOptionUtility.GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER);

		var totalGrantedPoint = 0;
		foreach (var pointRule in pointRules)
		{
			totalGrantedPoint += (int)pointRule.IncNum;
			new PointOptionUtility().InsertUserRegisterUserPoint(user.UserId, pointRule.PointRuleId, user.LastChanged, updateHistoryAction);

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				// Update Cross Point api
				CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
					user,
					pointRule.IncNum,
					CrossPointUtility.GetValue(Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID, pointRule.PointIncKbn));
			}
		}

		return totalGrantedPoint.ToString();
	}

	/// <summary>
	///  購入時ポイントの発行
	/// </summary>
	/// <param name="user">ユーザ情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	protected void PublishPointAtOrder(UserModel user, UpdateHistoryAction updateHistoryAction)
	{
		List<CartObject> guestCartList = (List<CartObject>)Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_CART_LIST];

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();

			//------------------------------------------------------
			// ポイント付与
			//------------------------------------------------------
			bool blIsAddFirstBuyPoint = false;
			foreach (CartObject coCart in guestCartList)
			{
				Hashtable htOrder = new Hashtable();
				htOrder[Constants.FIELD_ORDER_USER_ID] = user.UserId;
				htOrder[Constants.FIELD_ORDER_ORDER_ID] = coCart.OrderId;
				htOrder[Constants.FIELD_ORDER_LAST_CHANGED] = user.LastChanged;

				// 購入時ポイント取得
				decimal dPoint = coCart.BuyPoint;

				if (Constants.CROSS_POINT_OPTION_ENABLED == false)
				{
					// 初回購入ポイント追加（加算数の場合は最初の一つ、加算率の場合は全て）
					if ((blIsAddFirstBuyPoint == false)
						|| (coCart.FirstBuyPointKbn == Constants.FLG_POINTRULE_INC_TYPE_RATE))
					{
						blIsAddFirstBuyPoint = true;
						dPoint += coCart.FirstBuyPoint;

						OrderCommon.AddUserPoint(
							htOrder,
							coCart,
							Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY,
							UpdateHistoryAction.DoNotInsert,
							sqlAccessor);
					}

					// 購入時発行ポイント付与
					OrderCommon.AddUserPoint(
						htOrder,
						coCart,
						Constants.FLG_POINTRULE_POINT_INC_KBN_BUY,
						UpdateHistoryAction.DoNotInsert,
						sqlAccessor);
				}
				else
				{
					var point = CrossPointUtility.GetOrderPointAdd(coCart);
					if (point == null) continue;

					dPoint = (point.BaseGrantPoint + point.SpecialGrantPoint);
					var order = new OrderService().Get(coCart.OrderId, sqlAccessor);
					if (order == null) continue;

					// 付与ポイント登録
					var discount = coCart.TotalPriceDiscount;
					var registerInput = new PointApiInput
					{
						MemberId = coCart.OrderUserId,
						OrderDate = order.OrderDate,
						PosNo = w2.App.Common.Constants.CROSS_POINT_POS_NO,
						OrderId = coCart.OrderId,
						PointId = point.PointId,
						BaseGrantPoint = point.BaseGrantPoint,
						SpecialGrantPoint = point.SpecialGrantPoint,
						PriceTotalInTax = (TaxCalculationUtility.GetPriceTaxIncluded(coCart.PriceSubtotal, coCart.PriceSubtotalTax) - discount),
						PriceTotalNoTax = (TaxCalculationUtility.GetPriceTaxExcluded(coCart.PriceSubtotal, coCart.PriceSubtotalTax) - discount),
						UsePoint = coCart.UsePoint,
						Items = CartObject.GetOrderDetails(coCart),
					};
					var result = new CrossPointPointApiService().Register(registerInput.GetParam(PointApiInput.RequestType.Register));
					if (result.IsSuccess == false) continue;
				}

				// 受注データに付与ポイントセット
				var sv = new OrderService();
				sv.AdjustAddPoint(
					coCart.OrderId,
					dPoint,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(coCart.OrderId, Constants.FLG_LASTCHANGED_USER, sqlAccessor);
					new UpdateHistoryService().InsertForUser(user.UserId, Constants.FLG_LASTCHANGED_USER, sqlAccessor);
				}
			}
		}
	}

	/// <summary>
	/// メール送信
	/// </summary>
	/// <param name="mailId">メールテンプレートID</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="mailData">メールデータ</param>
	/// <param name="languageCode">言語コード</param>
	/// <param name="languageLocaleId">言語ロケールID</param>
	protected void SendMail(string mailId, string userId, Hashtable mailData, string languageCode = null, string languageLocaleId = null)
	{
		if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
		{
			if ((string)mailData[Constants.FIELD_USER_MAIL_ADDR] != "")
			{
				using (var msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, true, Constants.MailSendMethod.Auto, languageCode, languageLocaleId, (string)mailData[Constants.FIELD_USER_MAIL_ADDR]))
				{
					msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR].ToString());

					if (msMailSend.SendMail() == false)
					{
						AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
					}
				}
			}

			if ((string)mailData[Constants.FIELD_USER_MAIL_ADDR2] != "")
			{
				using (var msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, false, Constants.MailSendMethod.Auto, userMailAddress: (string)mailData[Constants.FIELD_USER_MAIL_ADDR]))
				{
					msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR2].ToString());

					if (msMailSend.SendMail() == false)
					{
						AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
					}
				}
			}
		}
		else
		{
			using (var msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, true, Constants.MailSendMethod.Auto, languageCode, languageLocaleId, (string)mailData[Constants.FIELD_USER_MAIL_ADDR]))
			{
				msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR].ToString());

				if (msMailSend.SendMail() == false)
				{
					AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
				}
			}
		}
	}

	/// <summary>
	/// CrossPoint メールアドレス重複チェック
	/// </summary>
	/// <param name="userInfo">ユーザー情報</param>
	/// <returns>エラーメッセージ</returns>
	protected string GetErrorMessageDuplicationCrossPointMailAddress(UserInput userInfo)
	{
		var crossPointUserSearchResult = new CrossPointUserApiService().Search(
			new Dictionary<string, string>
			{
				{ w2.App.Common.Constants.CROSS_POINT_PARAM_MEMBER_INFO_PC_MAIL, userInfo.MailAddr },
			});

		if (crossPointUserSearchResult.Result == null) return string.Empty;

		if (crossPointUserSearchResult.ResultStatus.IsSuccess == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
				w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var replaceTagName = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED
			? "@@User.mail_addr.name@@"
			: "@@User.login_id.name@@";

		var errorMessage = Validator.GetErrorMessage(
			CHECK_DUPLICATION,
			Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(replaceTagName, userInfo.AddrCountryIsoCode));

		return errorMessage;
	}
}
