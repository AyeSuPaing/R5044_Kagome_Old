/*
=========================================================================================================
  Module      : 会員登録APIファサード(RegisterApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Botchan;
using w2.App.Common.Global.Region;
using w2.App.Common.Option;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.User.Helper;

namespace w2.App.Common.User.Botchan
{
	/// <summary>
	/// 会員登録APIファサード
	/// </summary>
	public class RegisterApiFacade
	{
		/// <summary>
		/// 会員登録APIファサード
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="errorTypeList">エラーリスト</param>
		/// <param name="memo">メモー</param>
		/// <returns>会員登録レスポンス</returns>
		public RegisterResponse CreateResponseByRegisterUser(
			RegisterRequest request,
			ref List<BotchanMessageManager.MessagesCode> errorTypeList,
			ref string memo)
		{
			var userObject = request.UserRegisterObject;

			var userId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);

			var zipList = userObject.Zip.Split('-');
			var telList = userObject.Tel1.Split('-');
			var birthList = userObject.Birth.Split('/');

			var user = new UserModel
			{
				UserId = userId,
				Name = userObject.Name1 + userObject.Name2,
				Name1 = userObject.Name1,
				Name2 = userObject.Name2,
				NameKana = userObject.NameKana1 + userObject.NameKana2,
				NameKana1 = userObject.NameKana1,
				NameKana2 = userObject.NameKana2,
				UserKbn = userObject.UserKbn,
				Birth = (userObject.Birth != null) ? DateTime.Parse(userObject.Birth) : (DateTime?)null,
				BirthYear = (birthList.Length > 0) ? birthList[0] : string.Empty,
				BirthMonth = (userObject.Birth != null)
					? ((birthList.Length > 1)
						? DateTime.Parse(userObject.Birth).Month.ToString()
						: string.Empty)
					: string.Empty,
				BirthDay = (userObject.Birth != null)
					? ((birthList.Length > 2)
						? DateTime.Parse(userObject.Birth).Day.ToString()
						: string.Empty)
					: string.Empty,
				Sex = userObject.Sex,
				MailAddr = userObject.MailAddr,
				LoginId = userObject.MailAddr,
				Zip = userObject.Zip,
				Zip1 = (zipList.Length > 0) ? zipList[0] : string.Empty,
				Zip2 = (zipList.Length > 1) ? zipList[1] : string.Empty,
				Addr = AddressHelper.ConcatenateAddressWithoutCountryName(
					userObject.Addr1,
					userObject.Addr2,
					userObject.Addr3,
					userObject.Addr4),
				Addr1 = userObject.Addr1,
				Addr2 = userObject.Addr2,
				Addr3 = userObject.Addr3,
				Addr4 = userObject.Addr4,
				Addr5 = string.Empty,
				AddrCountryIsoCode = string.Empty,
				AddrCountryName = string.Empty,
				Tel1 = userObject.Tel1,
				Tel1_1 = (telList.Length > 0) ? telList[0] : string.Empty,
				Tel1_2 = (telList.Length > 1) ? telList[1] : string.Empty,
				Tel1_3 = (telList.Length > 2) ? telList[2] : string.Empty,
				PasswordDecrypted = userObject.Password,
				RemoteAddr = request.UserIpAddress,
				MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank(),
				LastChanged = Constants.FLG_LASTCHANGED_BOTCHAN,
				MailFlg = string.IsNullOrEmpty(userObject.MailFlg)
					? Constants.FLG_USER_MAILFLG_UNKNOWN
					: userObject.MailFlg,
			};

			new UserService().InsertWithUserExtend(
				user,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);

			RegionManager.GetInstance().UpdateUserRegion(new RegionModel(), userId);

			LoginAccountLockManager.GetInstance().CancelAccountLock(
				request.UserIpAddress,
				user.LoginId,
				user.PasswordDecrypted);

			var mailData = new RegisterUtility().ExecProcessOnUserRegistered(user, UpdateHistoryAction.DoNotInsert);
			new BotChanUtility().SendMail(
				Constants.CONST_MAIL_ID_USER_REGIST,
				user.UserId,
				mailData);

			var response = new RegisterResponse
			{
				Result = true,
				Message = Constants.BOTCHAN_API_SUCCESS_MESSAGE,
				MessageId = Constants.BOTCHAN_API_SUCCESS_MESSAGE_ID,
				Data = new RegisterResponseData
				{
					UserId = userId,
				}
			};

			return response;
		}
	}
}
