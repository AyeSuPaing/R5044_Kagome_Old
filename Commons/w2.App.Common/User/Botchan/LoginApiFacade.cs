/*
=========================================================================================================
  Module      : Login Api Facade(LoginApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using w2.App.Common.Botchan;
using w2.Common.Util;
using w2.Domain.Coupon;
using w2.Domain.Point;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.UserShipping;

namespace w2.App.Common.User.Botchan
{
	/// <summary>
	/// Login Api Facade
	/// </summary>
	public class LoginApiFacade
	{
		/// <summary>
		/// ログインレスポンス作成
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="errorTypeList">エラーリスト</param>
		/// <param name="memo">メモー</param>
		/// <returns>ログインレスポンス</returns>
		public LoginResponse CreateResponseByUser(
			LoginRequest request,
			ref List<BotchanMessageManager.MessagesCode> errorTypeList,
			ref string memo)
		{
			var user = LoginUtility.GetUser(request.MailAddress);
			if (user == null)
			{
				errorTypeList.Add(BotchanMessageManager.MessagesCode.USER_NOT_EXISTS);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.USER_NOT_EXISTS.ToString())
					.Replace("@@ 1 @@", request.MailAddress);
				return null;
			}

			if (LoginUtility.IsUserLock(user, request.UserIpAddress) == false)
			{
				errorTypeList.Add(BotchanMessageManager.MessagesCode.FRONT_USER_LOGIN_ACCOUNT_LOCK);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_USER_LOGIN_ACCOUNT_LOCK.ToString());
				return null;
			}

			if (LoginUtility.IsLoginSuccess(user, request.UserIpAddress, request.AuthText) == false)
			{
				errorTypeList.Add(BotchanMessageManager.MessagesCode.FRONT_USER_LOGIN_IN_MAILADDR_ERROR);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_USER_LOGIN_IN_MAILADDR_ERROR.ToString());
				return null;
			}

			var userService = new UserService();
			var userAttribute = userService.GetUserAttribute(user.UserId);
			var userExtendSetting = userService.GetUserExtendSettingArray();
			var userCreditCards = new UserCreditCardService().GetUsable(user.UserId);
			var userShipping = new UserShippingService().GetAllOrderByShippingNoDesc(user.UserId);
			var userDefaultOrderSetting = Constants.TWOCLICK_OPTION_ENABLE ? new UserDefaultOrderSettingService().Get(user.UserId) : null;
			var userPoint = new PointService().GetUserPoint(user.UserId, string.Empty);
			var userCoupon = new CouponService().GetUserCoupons(Constants.CONST_DEFAULT_DEPT_ID, user.UserId);

			var response = new LoginResponse
			{
				Result = true,
				Message = Constants.BOTCHAN_API_SUCCESS_MESSAGE,
				MessageId = Constants.BOTCHAN_API_SUCCESS_MESSAGE_ID,
				Data = new Data
				{
					User = LoginUtility.CreateUser(user),
					UserAttribute = LoginUtility.CreateUserAttribute(userAttribute),
					UserExtendSetting = LoginUtility.CreateUserExtendSettings(userExtendSetting),
					UserCreditCard = LoginUtility.CreateUserCreditCards(userCreditCards),
					UserShipping = LoginUtility.CreateUserShippings(userShipping, user),
					UserDefaultOrderSetting = LoginUtility.CreateUserDefaultOrderSetting(userDefaultOrderSetting),
					UserPoint = LoginUtility.CreateUserPoints(userPoint),
					UserCoupon = LoginUtility.CreateUserCoupons(userCoupon),
				}
			};

			return response;
		}
	}
}
