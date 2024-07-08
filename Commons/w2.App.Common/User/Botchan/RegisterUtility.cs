/*
=========================================================================================================
  Module      : Register Utility(Register.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Text;
using w2.App.Common.DataCacheController;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Coupon;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.User.Botchan
{
	/// <summary>
	/// Botchan Utility
	/// </summary>
	public class RegisterUtility
	{
		/// <summary>
		/// Create User
		/// </summary>
		/// <param name="userModel">User Model</param>
		/// <returns>User</returns>
		public static User CreateUser(UserModel userModel)
		{
			var user = new User()
			{
				UserId = userModel.UserId,
				UserKbn = userModel.UserKbn,
				Name = userModel.Name,
				NickName = userModel.NickName,
				MailAddr = userModel.MailAddr,
				Zip = userModel.Zip,
				Addr = userModel.Addr,
				Tel1 = userModel.Tel1,
				Tel2 = userModel.Tel2,
				Tel3 = userModel.Tel3,
				Fax = userModel.Fax,
				Sex = userModel.Sex,
				Birth = userModel.Birth,
				CompanyName = userModel.CompanyName,
				CompanyPostName = userModel.CompanyPostName,
				CompanyExectiveName = userModel.CompanyExectiveName,
				AdvcodeFirst = userModel.AdvcodeFirst,
				Attribute1 = userModel.Attribute1,
				Attribute2 = userModel.Attribute2,
				Attribute3 = userModel.Attribute3,
				Attribute4 = userModel.Attribute4,
				Attribute5 = userModel.Attribute5,
				Attribute6 = userModel.Attribute6,
				Attribute7 = userModel.Attribute7,
				Attribute8 = userModel.Attribute8,
				Attribute9 = userModel.Attribute9,
				Attribute10 = userModel.Attribute10,
				LoginId = userModel.LoginId,
				Password = userModel.Password,
				UserMemo = userModel.UserMemo,
				CareerId = userModel.CareerId,
				MemberRankId = userModel.MemberRankId,
				RecommendUid = userModel.RecommendUid,
				FixedPurchaseMemberFlg = userModel.FixedPurchaseMemberFlg,
				OrderCountOrderRealtime = userModel.OrderCountOrderRealtime,
				OrderCountOld = userModel.OrderCountOld,
			};
			return user;
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
				var publishCoupon = PublishCouponAtRegist(registeredUser, UpdateHistoryAction.DoNotInsert);
				mailData.Add("publish_coupons", publishCoupon);
			}

			// ユーザ拡張項目情報取得
			var controller = DataCacheControllerFacade.GetUserExtendSettingCacheController();
			var userExtendSettings = controller.GetModifyUserExtendSettingList(false, Constants.FLG_USEREXTENDSETTING_DISPLAY_PC);
			mailData[Constants.TABLE_USEREXTENDSETTING] = userExtendSettings;
			mailData[Constants.TABLE_USEREXTEND] = registeredUser.UserExtend;

			// 生年月日の時分秒削除
			mailData[Constants.FIELD_USER_BIRTH]
				= DateTimeUtility.ToStringFromRegion(mailData[Constants.FIELD_USER_BIRTH], DateTimeUtility.FormatType.ShortDate2Letter);

			// 更新履歴挿入
			new UpdateHistoryService().InsertForUser(registeredUser.UserId, Constants.FLG_LASTCHANGED_USER);

			return mailData;
		}

		/// <summary>
		///  会員登録時のポイント処理
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>発行済みポイント</returns>
		private string PointAtRegist(UserModel user, UpdateHistoryAction updateHistoryAction)
		{
			var point = PublishPointAtRegist(user, updateHistoryAction);
			return point;
		}

		/// <summary>
		/// 新規登録時ポイントの発行
		/// </summary>
		/// <param name="user">ユーザ情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>発行済みポイント</returns>
		protected string PublishPointAtRegist(UserModel user, UpdateHistoryAction updateHistoryAction)
		{
			var pointRules = PointOptionUtility.GetPointRulePriorityHigh("02");

			var totalGrantedPoint = 0;
			foreach (var pointRule in pointRules)
			{
				totalGrantedPoint += (int)pointRule.IncNum;
				new PointOptionUtility().InsertUserRegisterUserPoint(user.UserId, pointRule.PointRuleId, user.LastChanged, updateHistoryAction);
			}

			return totalGrantedPoint.ToString();
		}

		/// <summary>
		/// 登録時クーポン発行
		/// </summary>
		/// <param name="user">ユーザ情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>発行済みクーポン情報</returns>
		protected string PublishCouponAtRegist(UserModel user, UpdateHistoryAction updateHistoryAction)
		{
			var success = false;

			// 発行対象のクーポン情報取得
			// ※「新規登録で発行する会員用クーポン」のみ
			var publishCoupon = 1;
			var sbPublishCoupons = new StringBuilder();
			var couponService = new CouponService();

			var coupons = couponService.GetPublishCouponsByCouponType(Constants.W2MP_DEPT_ID, Constants.FLG_COUPONCOUPON_TYPE_USERREGIST);
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
						user.UserId,
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
							user.UserId,
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
							new UpdateHistoryService().InsertForUser(user.UserId, Constants.FLG_LASTCHANGED_USER, sqlAccessor);
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
						AppLogger.WriteError("新規会員登録クーポン発行失敗：[user_id=" + user.UserId + ",dept_id=" + coupon.DeptId + ",coupon_id=" + coupon.CouponId + "]\r\n");
					}
				}
			}

			return sbPublishCoupons.ToString();
		}
	}
}
