/*
=========================================================================================================
  Module      : Login Utility(LoginUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using w2.Domain.Coupon.Helper;
using w2.Domain.Point;
using w2.Domain.TempDatas;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.UserExtendSetting;
using w2.Domain.UserShipping;

namespace w2.App.Common.User.Botchan
{
	/// <summary>
	/// Botchan Utility
	/// </summary>
	public class LoginUtility
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
		/// Create User Attribute
		/// </summary>
		/// <param name="userAttributeModel">User Attribute Model</param>
		/// <returns>User Attribute</returns>
		public static UserAttribute CreateUserAttribute(UserAttributeModel userAttributeModel)
		{
			if (userAttributeModel == null) return new UserAttribute();
			var userAttribute = new UserAttribute()
			{
				FirstOrderDate = userAttributeModel.FirstOrderDate,
				SecondOrderDate = userAttributeModel.SecondOrderDate,
				LastOrderDate = userAttributeModel.LastOrderDate,
				CpmClusterAttribute1 = userAttributeModel.CpmClusterAttribute1,
				CpmClusterAttribute2 = userAttributeModel.CpmClusterAttribute2,
				CpmClusterAttribute1Before = userAttributeModel.CpmClusterAttribute1Before,
				CpmClusterAttribute2Before = userAttributeModel.CpmClusterAttribute2Before,
			};
			return userAttribute;
		}

		/// <summary>
		/// Create User Extend Settings
		/// </summary>
		/// <param name="userExtendSettingModels">User Extend Setting Models</param>
		/// <returns>User Extend Setting Array</returns>
		public static UserExtendSetting[] CreateUserExtendSettings(UserExtendSettingModel[] userExtendSettingModels)
		{
			var listUserExtendSetting = new List<UserExtendSetting>();
			foreach (var userExtendSettingModel in userExtendSettingModels)
			{
				listUserExtendSetting.Add(CreateUserExtendSetting(userExtendSettingModel));
			}
			return listUserExtendSetting.ToArray();
		}

		/// <summary>
		/// Create User Extend Setting
		/// </summary>
		/// <param name="userExtendSettingModel">User Extend Setting Model</param>
		/// <returns>User Extend Setting</returns>
		public static UserExtendSetting CreateUserExtendSetting(UserExtendSettingModel userExtendSettingModel)
		{
			var userExtend = new UserExtendSetting()
			{
				SettingId = userExtendSettingModel.SettingId,
				SettingName = userExtendSettingModel.SettingName,
				OutlineKbn = userExtendSettingModel.OutlineKbn,
				Outline = userExtendSettingModel.Outline,
			};
			return userExtend;
		}

		/// <summary>
		/// Create User Credit Cards
		/// </summary>
		/// <param name="userCreditCardModels">User Credit Card Model</param>
		/// <returns>User Credit Card</returns>
		public static UserCreditCard[] CreateUserCreditCards(UserCreditCardModel[] userCreditCardModels)
		{
			var listUserCreditCard = new List<UserCreditCard>();
			if (userCreditCardModels == null) return listUserCreditCard.ToArray();
			foreach (var userCreditCardModel in userCreditCardModels)
			{
				listUserCreditCard.Add(CreateUserCreditCard(userCreditCardModel));
			}
			return listUserCreditCard.ToArray();
		}

		/// <summary>
		/// Create User Credit Card
		/// </summary>
		/// <param name="userCreditCardModel">User Credit Card Model</param>
		/// <returns>User Credit Card</returns>
		private static UserCreditCard CreateUserCreditCard(UserCreditCardModel userCreditCardModel)
		{
			var userCreditCard = new UserCreditCard()
			{
				BranchNo = userCreditCardModel.BranchNo,
				CooperationId = userCreditCardModel.CooperationId,
				CardDispName = userCreditCardModel.CardDispName,
				LastFourDigit = userCreditCardModel.LastFourDigit,
				ExpirationMonth = userCreditCardModel.ExpirationMonth,
				ExpirationYear = userCreditCardModel.ExpirationYear,
				AuthorName = userCreditCardModel.AuthorName,
				CompanyCode = userCreditCardModel.CompanyCode,
				CooperationType = userCreditCardModel.CooperationType,
			};
			return userCreditCard;
		}

		/// <summary>
		/// Create User Shippings
		/// </summary>
		/// <param name="userShippingModels">User Shipping Model</param>
		/// <param name="user">User model</param>
		/// <returns>User Shipping</returns>
		public static UserShipping[] CreateUserShippings(
			UserShippingModel[] userShippingModels,
			UserModel user = null)
		{
			var listUserShipping = new List<UserShipping>();

			if ((userShippingModels.Length == 0) && (user != null))
			{
				var userShipping = new UserShipping
				{
					ShippingNo = 0,
					ShippingName1 = user.Name1,
					ShippingName2 = user.Name2,
					ShippingNameKana1 = user.NameKana1,
					ShippingNameKana2 = user.NameKana2,
					ShippingZip = user.Zip,
					ShippingAddr1 = user.Addr1,
					ShippingAddr2 = user.Addr2,
					ShippingAddr3 = user.Addr3,
					ShippingAddr4 = user.Addr4,
					ShippingTel1 = user.Tel1,
				};
				listUserShipping.Add(userShipping);
			}

			foreach (var userShippingModel in userShippingModels)
			{
				listUserShipping.Add(CreateUserShipping(userShippingModel));
			}
			return listUserShipping.ToArray();
		}

		/// <summary>
		/// Create User Shipping
		/// </summary>
		/// <param name="userShippingModel">User Shipping Model</param>
		/// <returns>User Shipping</returns>
		public static UserShipping CreateUserShipping(UserShippingModel userShippingModel)
		{
			var userShipping = new UserShipping()
			{
				ShippingNo = userShippingModel.ShippingNo,
				Name = userShippingModel.Name,
				ShippingName1 = userShippingModel.ShippingName1,
				ShippingName2 = userShippingModel.ShippingName2,
				ShippingNameKana1 = userShippingModel.ShippingNameKana1,
				ShippingNameKana2 = userShippingModel.ShippingNameKana2,
				ShippingZip = userShippingModel.ShippingZip,
				ShippingAddr1 = userShippingModel.ShippingAddr1,
				ShippingAddr2 = userShippingModel.ShippingAddr2,
				ShippingAddr3 = userShippingModel.ShippingAddr3,
				ShippingAddr4 = userShippingModel.ShippingAddr4,
				ShippingTel1 = userShippingModel.ShippingTel1,
				ShippingCompanyName = userShippingModel.ShippingCompanyName,
				ShippingCompanyPostName = userShippingModel.ShippingCompanyPostName,
			};
			return userShipping;
		}

		/// <summary>
		/// Create User Default Order Setting
		/// </summary>
		/// <param name="userDefaultOrderSettingModel">User Default Order Setting Model</param>
		/// <returns>User Default Order Setting</returns>
		public static UserDefaultOrderSetting CreateUserDefaultOrderSetting(UserDefaultOrderSettingModel userDefaultOrderSettingModel)
		{
			if (userDefaultOrderSettingModel == null) return new UserDefaultOrderSetting();
			var UserDefaultOrderSetting = new UserDefaultOrderSetting()
			{
				PaymentId = userDefaultOrderSettingModel.PaymentId,
				CreditBranchNo = userDefaultOrderSettingModel.CreditBranchNo,
				UserShippingNo = userDefaultOrderSettingModel.UserShippingNo,
			};
			return UserDefaultOrderSetting;
		}

		/// <summary>
		/// Create User Points
		/// </summary>
		/// <param name="userPointModels">User Point Model</param>
		/// <returns>User Point</returns>
		public static UserPoint[] CreateUserPoints(UserPointModel[] userPointModels)
		{
			var listUserPoint = new List<UserPoint>();
			foreach (var userPointModel in userPointModels)
			{
				listUserPoint.Add(CreateUserPoint(userPointModel));
			}
			return listUserPoint.ToArray();
		}

		/// <summary>
		/// Create User Point
		/// </summary>
		/// <param name="userPointModel">User Point Model</param>
		/// <returns>User Point</returns>
		private static UserPoint CreateUserPoint(UserPointModel userPointModel)
		{
			var userPoint = new UserPoint()
			{
				PointKbn = userPointModel.PointKbn,
				PointKbnNo = userPointModel.PointKbnNo,
				DeptId = userPointModel.DeptId,
				PointRuleId = userPointModel.PointRuleId,
				PointRuleKbn = userPointModel.PointRuleKbn,
				PointType = userPointModel.PointType,
				PointIncKbn = userPointModel.PointIncKbn,
				Point = userPointModel.Point,
				PointExp = userPointModel.PointExp,
			};
			return userPoint;
		}

		/// <summary>
		/// Create User Coupons
		/// </summary>
		/// <param name="userCouponDetailInfos">User Coupon Detail Info</param>
		/// <returns>User Coupon</returns>
		public static UserCoupon[] CreateUserCoupons(UserCouponDetailInfo[] userCouponDetailInfos)
		{
			var listUserCoupon = new List<UserCoupon>();
			foreach (var userCouponDetailInfo in userCouponDetailInfos)
			{
				listUserCoupon.Add(CreateUserCoupon(userCouponDetailInfo));
			}
			return listUserCoupon.ToArray();
		}

		/// <summary>
		/// Create User Coupon
		/// </summary>
		/// <param name="userCouponDetailInfo">User Coupon Detail Info</param>
		/// <returns>User Coupon</returns>
		private static UserCoupon CreateUserCoupon(UserCouponDetailInfo userCouponDetailInfo)
		{
			var userCoupon = new UserCoupon()
			{
				DeptId = userCouponDetailInfo.DeptId,
				CouponId = userCouponDetailInfo.CouponId,
				CouponNo = userCouponDetailInfo.CouponNo,
				OrderId = userCouponDetailInfo.OrderId,
				UseFlg = userCouponDetailInfo.UseFlg,
				UserCouponCount = userCouponDetailInfo.UserCouponCount,
			};
			return userCoupon;
		}

		/// <summary>
		/// Check User
		/// </summary>
		/// <param name="userKbn">User Kbn</param>
		/// <returns>True: If user can login, Otherwise: False</returns>
		public static bool CheckUser(string userKbn)
		{
			switch (userKbn)
			{
				case Constants.FLG_USER_USER_KBN_PC_USER:
				case Constants.FLG_USER_USER_KBN_SMARTPHONE_USER:
				case Constants.FLG_USER_USER_KBN_OFFLINE_USER:
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Create Hash SHA256
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>Key bytes for hash</returns>
		public static string CreateHashSHA256(string data)
		{
			var keyBytesForHash = Encoding.UTF8.GetBytes(data);
			using (var sha256 = new System.Security.Cryptography.SHA256CryptoServiceProvider())
			{
				var hash = sha256.ComputeHash(keyBytesForHash);
				var result = BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
				return result;
			}
		}

		/// <summary>
		/// Get User
		/// </summary>
		/// <param name="mailAddr">Mail Address</param>
		/// <returns>User</returns>
		public static UserModel GetUser(string mailAddr)
		{
			var user = new UserService().GetUserByMailAddr(mailAddr);
			return user;
		}

		/// <summary>
		/// Is Login Success
		/// </summary>
		/// <param name="userModel">User Model</param>
		/// <param name="userIpAddress">User Ip Address</param>
		/// <param name="authText">Auth Text</param>
		/// <returns>Is Login Success</returns>
		public static bool IsLoginSuccess(
			UserModel userModel,
			string userIpAddress,
			string authText)
		{
			var secret = string.Format(
				"{0}{1}{2}",
				userModel.MailAddr,
				userModel.PasswordDecrypted,
				Constants.SECRET_KEY_API_BOTCHAN);
			var hashKey = CreateHashSHA256(secret);

			if ((authText == hashKey)
				&& LoginUtility.CheckUser(userModel.UserKbn)) return true;

			UpdateLockPossibleTrialLoginCount(userIpAddress, userModel.LoginId);
			return false;
		}

		/// <summary>
		/// Is User Lock
		/// </summary>
		/// <param name="userModel">User Model</param>
		/// <param name="userIpAddress">User Ip Address</param>
		/// <returns>Is User Lock</returns>
		public static bool IsUserLock(UserModel userModel, string userIpAddress)
		{
			var isLock = IsAccountLocked(
				userIpAddress,
				userModel.LoginId);
			return (isLock == false);
		}

		/// <summary>
		/// アカウントロックデータのログイン試行可能回数更新
		/// </summary>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="loginId">ログインID</param>
		public static void UpdateLockPossibleTrialLoginCount(string ipAddress, string loginId)
		{
			// TempDataキーの設定（「IP＋ログインID」）
			var loginErrorInfoKeyForLoginId = CreateErrorInfoKeyForLoginId(ipAddress, loginId);

			// ログインエラー情報取得
			var loginErrorInfos = GetLoginErrorInfos(loginErrorInfoKeyForLoginId);
			var loginErrorInfoForLoginId = loginErrorInfos.Item1;

			// IP＋ログインIDでエラーログイン情報が存在した場合、ログイン試行可能回数を１減らす
			new TempDatasService().Save(
				TempDatasService.TempType.LoginErrorInfoLoginId,
				loginErrorInfoKeyForLoginId,
				(loginErrorInfoForLoginId == null)
					? (Constants.POSSIBLE_BOTCHAN_LOGIN_ERROR_COUNT - 1)
					: ((int)loginErrorInfoForLoginId.TempDataDeserialized - 1));
		}

		/// <summary>
		/// ログインID向けログインエラー情報アカウントロックデータキー作成
		/// </summary>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="loginId">ログインID</param>
		/// <returns>アカウントロックデータキー</returns>
		private static string CreateErrorInfoKeyForLoginId(string ipAddress, string loginId)
		{
			var result = string.Format(
				"{0}{1}{2}",
				ipAddress,
				Constants.ACOUNT_LOCK_KEY_LOGINERRORINFO_MIDDLE_STRING_LOGIN_ID,
				loginId);
			return result;
		}

		/// <summary>
		/// ログインエラー情報取得
		/// </summary>
		/// <param name="ipAndLoginId">IP＋ログインIDキー</param>
		/// <returns>ログインエラー情報（IP+ログインID）</returns>
		private static Tuple<TempDatasModel> GetLoginErrorInfos(string ipAndLoginId)
		{
			return new Tuple<TempDatasModel>(
				new TempDatasService().Resotre(
					TempDatasService.TempType.LoginErrorInfoLoginId,
					ipAndLoginId,
					Constants.POSSIBLE_BOTCHAN_LOGIN_LOCK_MINUTES));
		}

		/// <summary>
		/// ログインアカウントロックチェック
		/// </summary>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="loginId">ログインID</param>
		/// <returns>アカウントロックフラグ</returns>
		public static bool IsAccountLocked(string ipAddress, string loginId)
		{
			// アカウントロックデータキーの設定（「IP＋ログインID」）
			var loginErrorInfoKeyForLoginId = CreateErrorInfoKeyForLoginId(ipAddress, loginId);
			var loginErrorInfos = GetLoginErrorInfos(loginErrorInfoKeyForLoginId);
			var loginErrorInfoForLoginId = loginErrorInfos.Item1;

			// 「IP＋ログインID」のどちらかでログイン試行可能回数が0の場合はアカウントロック中
			return ((loginErrorInfoForLoginId != null)
				&& ((int)loginErrorInfoForLoginId.TempDataDeserialized <= 0));
		}
	}
}
