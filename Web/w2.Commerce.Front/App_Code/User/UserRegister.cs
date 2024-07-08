/*
=========================================================================================================
  Module      : ユーザイベント引数クラス(UserRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Order;
using w2.Domain.User;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.DataCacheController;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.User.SocialLogin;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;
using w2.App.Common.Web.Page;

namespace w2.App.Common.User
{
	/// <summary>
	/// UserRegister の概要の説明です
	/// </summary>
	public class UserRegister
	{
		/// <summary>
		///デフォルトコンストラクタ
		/// </summary>
		public UserRegister()
		{
			this.IsAfterGuestOrder = false;
			this.GuestCartList = null;
			this.SocialLogin = null;
			this.AmazonModel = null;
			this.IsRakutenIdConnectUserRegister = false;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isRakutenIdConnectUserRegister">楽天IDConnect会員登録か</param>
		public UserRegister(bool isRakutenIdConnectUserRegister)
			: this()
		{
			this.IsRakutenIdConnectUserRegister = isRakutenIdConnectUserRegister;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isAfterGuestOrder">ゲスト購入後の会員登録か</param>
		/// <param name="guestCartList">ゲストカートリスト</param>
		/// <param name="socialLogin">ソーシャルログインモデル</param>
		/// <param name="amazon">アマゾンモデル</param>
		/// <param name="isRakutenIdConnectUserRegister">楽天IDConnect会員登録か</param>
		public UserRegister(bool isAfterGuestOrder,
			List<CartObject> guestCartList,
			SocialLoginModel socialLogin,
			AmazonModel amazon,
			bool isRakutenIdConnectUserRegister = false)
			: this()
		{
			this.IsAfterGuestOrder = isAfterGuestOrder;
			this.GuestCartList = guestCartList;
			this.SocialLogin = socialLogin;
			this.AmazonModel = amazon;
			this.IsRakutenIdConnectUserRegister = isRakutenIdConnectUserRegister;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="socialLogin">ソーシャルログイン ログインモデル</param>
		public UserRegister(SocialLoginModel socialLogin)
			: this()
		{
			this.SocialLogin = socialLogin;
		}

		/// <summary>
		/// ユーザ登録時処理実行（ゲスト購入後含む）
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="registedUser">登録済ユーザ情報</param>
		public void ExecProcessOnUserRegisted(UserModel registedUser, UpdateHistoryAction updateHistoryAction)
		{
			var mailData = ExecProcessOnUserRegistered(registedUser, updateHistoryAction);

			SendMail(
				Constants.CONST_MAIL_ID_USER_REGIST,
				registedUser.UserId,
				mailData,
				registedUser.DispLanguageCode,
				registedUser.DispLanguageLocaleId);
		}

		/// <summary>
		/// ユーザ登録時処理実行
		/// </summary>
		/// <param name="registeredUser">登録済ユーザ情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>送信メール内容</returns>
		public Hashtable ExecProcessOnUserRegistered(UserModel registeredUser, UpdateHistoryAction updateHistoryAction)
		{
			var mailData = (Hashtable)registeredUser.DataSource.Clone();

			// 複合化されたパスワードに書き換え
			mailData[Constants.FIELD_USER_PASSWORD] = registeredUser.PasswordDecrypted;

			// ポイントOPがONならポイント発行
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				var publishPoint = PointAtRegist(registeredUser, UpdateHistoryAction.DoNotInsert);
				mailData.Add(Constants.FIELD_USERPOINT_POINT, publishPoint);
			}

			// クーポンOPがONならクーポン発行
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				// 紹介した人のデータ取得
				var introducer = DomainFacade.Instance.UserService.GetUserByReferralCode(SessionManager.ReferralCode, null);
				var publishCoupon = PublishCouponAtRegist(registeredUser, UpdateHistoryAction.DoNotInsert, (introducer != null) ? introducer.UserId : "", true);
				mailData.Add("publish_coupons", publishCoupon);

				// Introduction coupon
				if (Constants.INTRODUCTION_COUPON_OPTION_ENABLED
					&& (string.IsNullOrEmpty(SessionManager.ReferralCode) == false))
				{
					if (introducer != null)
					{
						// Publish coupons give to introducer
						var publishCouponAtRegist = PublishCouponAtRegist(
							registeredUser,
							UpdateHistoryAction.DoNotInsert,
							introducer.UserId);
					}
				}
			}

			// ユーザ拡張項目情報取得
			var controller = DataCacheControllerFacade.GetUserExtendSettingCacheController();
			var userExtendSettings = controller.GetModifyUserExtendSettingList(false, Constants.FLG_USEREXTENDSETTING_DISPLAY_PC);
			mailData[Constants.TABLE_USEREXTENDSETTING] = userExtendSettings;
			mailData[Constants.TABLE_USEREXTEND] = registeredUser.UserExtend;

			// 生年月日の時分秒削除
			mailData[Constants.FIELD_USER_BIRTH]
				= DateTimeUtility.ToStringFromRegion(mailData[Constants.FIELD_USER_BIRTH], DateTimeUtility.FormatType.ShortDate2Letter);

			// ソーシャルログインID紐付け
			if (Constants.SOCIAL_LOGIN_ENABLED)
			{
				var socialLogin = this.SocialLogin;
				if (socialLogin != null)
				{
					var authUser = new SocialLoginMap();
					socialLogin.W2UserId = registeredUser.UserId;
					authUser.Exec(Constants.SOCIAL_LOGIN_API_KEY, socialLogin.SPlusUserId, socialLogin.W2UserId, false);

					// ソーシャルプロバイダID保存
					SetSocialProviders(registeredUser.UserId, registeredUser.UserExtend);
					new UserService().UpdateUserExtend(
						registeredUser.UserExtend,
						registeredUser.UserId,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert);
				}
			}
			// AmazonユーザーID紐づけ
			if (Constants.AMAZON_LOGIN_OPTION_ENABLED && this.IsAmazonLoggedIn)
			{
				var amazonModel = this.AmazonModel;
				AmazonUtil.SetAmazonUserIdForUserExtend(registeredUser.UserExtend, registeredUser.UserId, amazonModel.UserId);
			}
			// PayPal連携情報紐付け
			if (SessionManager.PayPalLoginResult != null)
			{
				PayPalUtility.Account.UpdateUserExtendForPayPal(
					registeredUser.UserId,
					SessionManager.PayPalLoginResult,
					UpdateHistoryAction.DoNotInsert);
			}
			// 楽天IDConnect連携紐づけ
			if (Constants.RAKUTEN_LOGIN_ENABLED && this.IsRakutenIdConnectUserRegister)
			{
				registeredUser.UserExtend.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_OPEN_ID] =
					SessionManager.RakutenIdConnectActionInfo.RakutenIdConnectUserInfoResponseData.OpenIdId;

				registeredUser.UserExtend.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_REGISTER_USER] =
					Constants.FLG_USER_RAKUTEN_ID_CONNECT_REGISTER_USER_ON;

				new UserService().UpdateUserExtend(
					registeredUser.UserExtend,
					registeredUser.UserId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);

				SessionManager.RakutenIdConnectActionInfo = null;
				SessionManager.IsRakutenIdConnectLoggedIn = true;
			}

			// Update referred user id
			var userInfo = DomainFacade.Instance.UserService.Get(registeredUser.UserId);
			var referredUser = DomainFacade.Instance.UserService.GetUserByReferralCode(SessionManager.ReferralCode, null);
			var referredUserId = (referredUser == null) ? registeredUser.UserId : referredUser.UserId;

			DomainFacade.Instance.UserService.UpdateReferredUserId(
				userInfo.UserId,
				referredUserId,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);

			// 更新履歴挿入
			new UpdateHistoryService().InsertForUser(registeredUser.UserId, Constants.FLG_LASTCHANGED_USER);

			return mailData;
		}

		/// <summary>
		/// 連携済みソーシャルプロバイダ認証情報セット
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userExtend">ユーザー拡張項目</param>
		/// <returns>ソーシャルプロバイダ認証情報</returns>
		private void SetSocialProviders(string userId, UserExtendModel userExtend)
		{
			if (Constants.SOCIAL_LOGIN_ENABLED == false) return;

			var socialLogin = this.SocialLogin;
			if (socialLogin == null) return;

			SocialLoginUtil.SetSocialProviderInfo(socialLogin.SPlusUserId, userId, userExtend);
		}

		/// <summary>
		///  会員登録時のポイント処理
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>発行済みポイント</returns>
		public string PointAtRegist(UserModel user, UpdateHistoryAction updateHistoryAction)
		{
			var point = PublishPointAtRegist(user, updateHistoryAction);

			// ゲスト購入直後の登録の場合、ゲスト購入時の購入時ポイントを発行する
			// ここで発行したポイントはどこにも通知しない
			if (this.IsAfterGuestOrder)
			{
				PublishPointAtOrder(user, updateHistoryAction);
			}

			return point;
		}

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
			var success = false;

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

			if (this.IsAfterGuestOrder
				&& (string.IsNullOrEmpty(userIdOfReferralCode) == false))
			{
				var couponPurchaseGiveToIntroducer = couponService.GetPublishCouponsByCouponType(
					Constants.W2MP_DEPT_ID,
					Constants.FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON);
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
					success = couponService.InsertUserCouponWithOrderId(
						userId,
						"",
						coupon.DeptId,
						coupon.CouponId,
						user.LastChanged,
						UpdateHistoryAction.DoNotInsert,
						sqlAccessor);
					if (success)
					{
						// ユーザクーポン履歴情報登録
						success = couponService.InsertUserCouponHistory(
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

					if (success)
					{
						// 更新履歴登録
						if (updateHistoryAction == UpdateHistoryAction.Insert)
						{
							new UpdateHistoryService().InsertForUser(
								userId,
								Constants.FLG_LASTCHANGED_USER,
								sqlAccessor);
						}

						// トランザクションコミット
						sqlAccessor.CommitTransaction();

						sbPublishCoupons.Append("-(").Append(publishCoupon).Append(")--------------------").Append("\r\n");
						sbPublishCoupons.Append(CommonPage.ReplaceTagByLocaleId("@@MailTemplate.coupon_code.name@@", user.DispLanguageLocaleId)).Append(coupon.CouponCode).Append("\r\n");
						sbPublishCoupons.Append(CommonPage.ReplaceTagByLocaleId("@@MailTemplate.coupon_name.name@@", user.DispLanguageLocaleId)).Append(coupon.CouponDispName).Append("\r\n");
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
			var pointRules = PointOptionUtility.GetPointRulePriorityHigh(global::Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER);

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
						CrossPointUtility.GetValue(
							Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
							pointRule.PointIncKbn));
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
			var guestCartList = this.GuestCartList;

			using (var sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				//------------------------------------------------------
				// ポイント付与
				//------------------------------------------------------
				var isAddFirstBuyPoint = false;
				foreach (var cart in guestCartList)
				{
					var order = new Hashtable
					{
						{ Constants.FIELD_ORDER_USER_ID, user.UserId },
						{ Constants.FIELD_ORDER_ORDER_ID, cart.OrderId },
						{ Constants.FIELD_ORDER_LAST_CHANGED, user.LastChanged },
					};

					// ゲスト購入時クロスポイント側に受注日時を連携する必要がある
					if (Constants.CROSS_POINT_OPTION_ENABLED)
					{
						var orderModel = new OrderService().Get(cart.OrderId);
						order.Add(Constants.FIELD_ORDER_ORDER_DATE, orderModel.OrderDate);
					}

					// 購入時ポイント取得
					var point = cart.BuyPoint;

					// 初回購入ポイント追加（加算数の場合は最初の一つ、加算率の場合は全て）
					if ((isAddFirstBuyPoint == false) || (cart.FirstBuyPointKbn == Constants.FLG_POINTRULE_INC_TYPE_RATE))
					{
						isAddFirstBuyPoint = true;
						point += cart.FirstBuyPoint;

						OrderCommon.AddUserPoint(
							order,
							cart,
							Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY,
							UpdateHistoryAction.DoNotInsert,
							sqlAccessor);
					}

					// 購入時発行ポイント付与
					OrderCommon.AddUserPoint(
						order,
						cart,
						Constants.FLG_POINTRULE_POINT_INC_KBN_BUY,
						UpdateHistoryAction.DoNotInsert,
						sqlAccessor);

					// 受注データに付与ポイントセット
					var sv = new OrderService();
					sv.AdjustAddPoint(
						cart.OrderId,
						point,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert,
						sqlAccessor);

					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertForOrder(cart.OrderId, Constants.FLG_LASTCHANGED_USER, sqlAccessor);
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
					using (var msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, true, Constants.MailSendMethod.Auto, languageCode, languageLocaleId, mailData[Constants.FIELD_USER_MAIL_ADDR].ToString()))
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
					using (var msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, false, Constants.MailSendMethod.Auto, userMailAddress: mailData[Constants.FIELD_USER_MAIL_ADDR].ToString()))
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
				using (var msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, true, Constants.MailSendMethod.Auto, languageCode, languageLocaleId, mailData[Constants.FIELD_USER_MAIL_ADDR].ToString()))
				{
					msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR].ToString());

					if (msMailSend.SendMail() == false)
					{
						AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
					}
				}
			}
		}

		/// <summary>ゲスト購入後の会員登録か</summary>
		public bool IsAfterGuestOrder { get; private set; }
		/// <summary>ゲストカートリスト</summary>
		public List<CartObject> GuestCartList { get; private set; }
		/// <summary>ソーシャルログイン</summary>
		public SocialLoginModel SocialLogin { get; private set; }
		/// <summary>Amazonモデル</summary>
		public AmazonModel AmazonModel { get; private set; }
		/// <summary>Amazonログイン状態</summary>
		public bool IsAmazonLoggedIn { get { return (this.AmazonModel != null); } }
		/// <summary>楽天IDConnect会員登録か</summary>
		public bool IsRakutenIdConnectUserRegister { get; private set; }
	}
}
