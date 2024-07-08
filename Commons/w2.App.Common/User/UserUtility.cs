/*
=========================================================================================================
  Module      : ユーザーユーティリティクラス(UserUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Line.Util;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.User.SocialLogin;
using w2.Common.Sql;
using w2.Domain;
using w2.Domain.AdvCode;
using w2.Domain.MemberRank;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserManagementLevel;

namespace w2.App.Common.User
{
	/// <summary>
	/// ユーザーユーティリティ
	/// </summary>
	public class UserUtility
	{
		/// <summary>
		/// 退会処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastchanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="errorMessage">Error message</param>
		public static void Withdrawal(
			string userId,
			string lastchanged,
			out string errorMessage,
			UpdateHistoryAction updateHistoryAction)
		{
			var result = false;
			var userService = new UserService();
			errorMessage = string.Empty;
			var user = userService.Get(userId);

			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				result = userService.UserWithdrawal(
				userId,
				lastchanged,
					UpdateHistoryAction.Insert,
					accessor);

				if (Constants.CROSS_POINT_OPTION_ENABLED
					&& result)
				{
					var afterWithdrawalUserInfo = userService.Get(userId, accessor);
					afterWithdrawalUserInfo.Zip = string.Empty;
					afterWithdrawalUserInfo.Zip1 = string.Empty;
					afterWithdrawalUserInfo.Zip2 = string.Empty;

					var withdrawalResult = new CrossPointUserApiService().Withdraw(afterWithdrawalUserInfo);
					if (withdrawalResult.IsSuccess == false)
					{
						errorMessage = string.Format(
							"{0}{1}{2}",
							CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SYSTEM_ERROR),
							Environment.NewLine,
							withdrawalResult.ErrorCodeList);
						return;
					}
				}

				// トランザクションコミット
				accessor.CommitTransaction();
			}
			if (result)
			{
				// 連携退会
				if (Constants.USER_COOPERATION_ENABLED)
				{
					var userCooperationPlugin = new UserCooperationPlugin(Constants.FLG_LASTCHANGED_USER);
					userCooperationPlugin.Withdrawal(user);
				}

				// ソーシャルプラス側ユーザ削除
				if (Constants.SOCIAL_LOGIN_ENABLED)
				{
					var sldu = new SocialLoginDeleteUser();
					sldu.Exec(Constants.SOCIAL_LOGIN_API_KEY, null, userId);
				}

				// ユーザー拡張項目からLINEユーザーID削除
				if (Line.Constants.LINE_DIRECT_OPTION_ENABLED || Constants.SOCIAL_LOGIN_ENABLED)
				{
					var userExtend = new UserService().GetUserExtend(userId); 
					LineUtil.RemoveLineUserIdFromUserExtend(userExtend, userId, UpdateHistoryAction.DoNotInsert);
				}

				// ユーザー拡張項目からAmazonユーザーID削除
				if (Constants.AMAZON_LOGIN_OPTION_ENABLED || Constants.AMAZON_PAYMENT_OPTION_ENABLED)
				{
					var userExtend = new UserService().GetUserExtend(userId);
					AmazonUtil.RemoveAmazonUserIdFromUserExtend(userExtend, userId, UpdateHistoryAction.DoNotInsert);
				}

				// PayPalユーザー削除
				if (Constants.PAYPAL_LOGINPAYMENT_ENABLED)
				{
					userService.ModifyUserExtend(
						userId,
						model =>
						{
							model.UserExtendDataValue[Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID] = string.Empty;
							model.UserExtendDataValue[Constants.PAYPAL_USEREXTEND_COLUMNNAME_COOPERATION_INFOS] = string.Empty;
						},
						UpdateHistoryAction.DoNotInsert);
				}

				// 楽天コネクト削除
				if (Constants.RAKUTEN_LOGIN_ENABLED)
				{
					userService.ModifyUserExtend(
						userId,
						model =>
						{
							model.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_OPEN_ID] = string.Empty;
						},
						UpdateHistoryAction.DoNotInsert);
				}

				// 入荷メール情報削除
				using (var accessor = new SqlAccessor())
				using (var statement = new SqlStatement(
					"UserProductArrivalMail",
					"DeleteUserProductArrivalMailFromUserId"))
				{
					var ht = new Hashtable
					{
						{ Constants.FIELD_USER_USER_ID, userId },
					};
					statement.ExecStatementWithOC(accessor, ht);
				}

				// 更新履歴登録
				new UpdateHistoryService().InsertForUser(userId, lastchanged);
			}
		}

		/// <summary>
		/// 広告コードより補正情報（会員ランク、ユーザー管理レベル）を取得する
		/// </summary>
		/// <param name="advCode">広告コード</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		public static void GetCorrectUserInfoByAdvCode(
			string advCode,
			out string memberRankId,
			out string userManagementLevelId)
		{
			memberRankId = null;
			userManagementLevelId = null;
			if (string.IsNullOrEmpty(advCode) == false)
			{
				var advCodeInfo = new AdvCodeService().GetAdvCodeFromAdvertisementCode(advCode);
				if (advCodeInfo != null)
				{
					var memberRank = DomainFacade.Instance.MemberRankService.GetMemberRankList()
						.FirstOrDefault(model => model.MemberRankId == advCodeInfo.MemberRankIdGrantedAtAccountRegistration && model.IsValid);
					memberRankId = (memberRank == null)
						? memberRankId
						: memberRank.MemberRankId;

					var userManagementLevel = DomainFacade.Instance.UserManagementLevelService.GetAllList()
						.FirstOrDefault(model => model.UserManagementLevelId == advCodeInfo.UserManagementLevelIdGrantedAtAccountRegistration);
					userManagementLevelId = (userManagementLevel == null)
						? userManagementLevelId
						: userManagementLevel.UserManagementLevelId;
				}
			}
		}

		/// <summary>
		/// 広告コードよりユーザー情報を補正する
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		public static void CorrectUserByAdvCode(UserModel user)
		{
			if (string.IsNullOrEmpty(user.AdvcodeFirst) == false)
			{
				string memberRankId;
				string userManagementLevelId;
				GetCorrectUserInfoByAdvCode(user.AdvcodeFirst, out memberRankId, out userManagementLevelId);

				user.MemberRankId = memberRankId ?? user.MemberRankId;
				user.UserManagementLevelId = userManagementLevelId ?? user.UserManagementLevelId;
			}

		}

		/// <summary>
		/// Adjust point and member rank by Cross Point api
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		/// <remarks>ユーザーIDを指定した際、最新を取得後に更新を行う。ユーザーモデルを指定した際、そのユーザーモデルを基に更新を行う。</remarks>
		public static void AdjustPointAndMemberRankByCrossPointApi(
			string userId,
			SqlAccessor accessor = null,
			UpdateHistoryAction updateHistoryAction = UpdateHistoryAction.Insert)
		{
			AdjustPointAndMemberRankByCrossPointApi(
				new UserService().Get(userId),
				accessor,
				updateHistoryAction
			);
		}
		/// <summary>
		/// Adjust point and member rank by Cross Point api
		/// </summary>
		/// <param name="user">User model</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		/// <remarks>ユーザーIDを指定した際、最新を取得後に更新を行う。ユーザーモデルを指定した際、そのユーザーモデルを基に更新を行う。</remarks>
		public static void AdjustPointAndMemberRankByCrossPointApi(
			UserModel user,
			SqlAccessor accessor = null,
			UpdateHistoryAction updateHistoryAction = UpdateHistoryAction.Insert)
		{
			if ((user == null)
				|| (user.UserKbn == Constants.FLG_USER_USER_KBN_ALL_GUEST)
				|| (user.UserKbn == Constants.FLG_USER_USER_KBN_PC_GUEST)
				|| (user.UserKbn == Constants.FLG_USER_USER_KBN_MOBILE_GUEST)
				|| (user.UserKbn == Constants.FLG_USER_USER_KBN_SMARTPHONE_GUEST)
				|| (user.UserKbn == Constants.FLG_USER_USER_KBN_OFFLINE_GUEST))
			{
				return;
			}

			var userExtend = new UserService().GetUserExtend(user.UserId, accessor);
			user.UserExtend = userExtend;

			var crossPointUser = new CrossPointUserApiService().Get(user.UserId);

			if ((crossPointUser == null) || (user.IsMember == false))
			{
				return;
			}

			var zip = new ZipCode(crossPointUser.Postcode);
			var tel1 = new Tel(crossPointUser.Tel);
			var tel2 = new Tel(crossPointUser.MbTel);
			var birthday = new Func<DateTime?>(() =>
				{
					DateTime birth;
					if (DateTime.TryParse(crossPointUser.Birthday, out birth) == false) return null;
					return birth;
				})();

			user.Name1 = crossPointUser.LastName;
			user.Name2 = crossPointUser.FirstName;
			user.Name = crossPointUser.Name;
			user.NameKana1 = crossPointUser.LastNamePhonetic;
			user.NameKana2 = crossPointUser.FirstNamePhonetic;
			user.NameKana = crossPointUser.NamePhonetic;
			user.UserExtend.UserExtendDataValue.CrossPointAddShopName = crossPointUser.AdmissionShopName;
			user.MemberRankId = crossPointUser.MemberRankId;
			user.Sex = GetSystemGender(crossPointUser.Sex);
			user.Birth = birthday.HasValue ? birthday : user.Birth;
			user.BirthDay = birthday.HasValue ? birthday.Value.Day.ToString() : user.BirthDay;
			user.BirthMonth = birthday.HasValue ? birthday.Value.Month.ToString() : user.BirthMonth;
			user.BirthYear = birthday.HasValue ? birthday.Value.Year.ToString() : user.BirthYear;
			user.Zip = crossPointUser.Postcode;
			user.Zip1 = zip.Zip1;
			user.Zip2 = zip.Zip2;
			user.Addr1 = crossPointUser.PrefName;
			user.Addr2 = crossPointUser.City;
			user.Addr3 = string.Format(
				"{0}{1}",
				crossPointUser.Town,
				crossPointUser.Address);
			user.Addr4 = crossPointUser.Building;
			user.Tel1 = crossPointUser.Tel;
			user.Tel1_1 = tel1.Tel1;
			user.Tel1_2 = tel1.Tel2;
			user.Tel1_3 = tel1.Tel3;
			user.Tel2 = crossPointUser.MbTel;
			user.Tel2_1 = tel2.Tel1;
			user.Tel2_2 = tel2.Tel2;
			user.Tel2_3 = tel2.Tel3;
			user.MailAddr = crossPointUser.PcMail;
			user.MailAddr2 = crossPointUser.MbMail;
			user.MailFlg = (crossPointUser.EmailDmUnnecessaryFlg == Constants.CROSS_POINT_FLG_USER_MAIL_FLG_OK)
				? Constants.FLG_USER_MAILFLG_OK
				: Constants.FLG_USER_MAILFLG_NG;
			user.UserExtend.UserExtendDataValue.CrossPointShopCardNo = crossPointUser.RealShopCardNo;
			user.UserExtend.UserExtendDataValue.CrossPointShopCardPin = crossPointUser.PinCode;
			user.LastChanged = Constants.FLG_LASTCHANGED_CROSSPOINT;

			DomainFacade.Instance.UserService.UpdateWithUserExtend(
				user,
				updateHistoryAction,
				accessor);

			AdjustPointByCrossPoint(crossPointUser, user.UserId, accessor);

			if (HttpContext.Current != null)
			{
				HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_USER_MEMBER_RANK_ID] = crossPointUser.MemberRankId;
			}
		}

		/// <summary>
		/// CROSSPOINT連携して、調整対象ユーザーのポイント数調整
		/// </summary>
		/// <param name="userIds">調整対象ユーザーID一覧</param>
		/// <param name="accessor">アクセサー</param>
		public void AdjustPointByCrossPointApi(string[] userIds, SqlAccessor accessor = null)
		{
			foreach (var userId in userIds)
			{
				AdjustPointByCrossPointApi(userId, accessor);
			}
		}

		/// <summary>
		/// CROSSPOINT連携して、調整対象ユーザーのポイント数調整
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサー</param>
		public static void AdjustPointByCrossPointApi(string userId, SqlAccessor accessor = null)
		{
			var crossPointUser = new CrossPointUserApiService().Get(userId);
			if (crossPointUser == null) return;
			AdjustPointByCrossPoint(crossPointUser, userId, accessor);
		}

		/// <summary>
		/// ユーザーのポイント数調整
		/// </summary>
		/// <param name="crossPointUser">crossPointユーザーモデル</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサー</param>
		private static void AdjustPointByCrossPoint(UserApiResult crossPointUser, string userId, SqlAccessor accessor = null)
		{
			// 有効ポイントが数値変換できる場合だけポイント更新を行う
			decimal effectivePoint;
			if (decimal.TryParse(crossPointUser.EffectivePoint, out effectivePoint))
			{
				CrossPointUtility.AdjustPointByCrossPoint(
					userId,
					effectivePoint,
					crossPointUser.ExceptInvalidDate,
					Constants.FLG_LASTCHANGED_USER,
					accessor);
			}
		}

		/// <summary>
		/// Check card no and card pin
		/// </summary>
		/// <param name="cardNo">Card no</param>
		/// <param name="cardPin">Card pin</param>
		/// <param name="hasCard">Has card</param>
		/// <returns>True: if card id valid, otherwise: false</returns>
		public static bool CheckCardNoAndCardPin(string cardNo, string cardPin, out bool hasCard)
		{
			hasCard = false;
			if ((string.IsNullOrEmpty(cardNo) == false)
				&& (string.IsNullOrEmpty(cardPin) == false))
			{
				return hasCard = true;
			}

			if (string.IsNullOrEmpty(cardNo)
				&& string.IsNullOrEmpty(cardPin))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Get System Gender
		/// </summary>
		/// <param name="sex">性別</param>
		/// <returns>Gender</returns>
		public static string GetSystemGender(string sex)
		{
			switch (sex)
			{
				case Constants.CROSS_POINT_FLG_USER_SEX_FEMALE:
				case Constants.CROSS_POINT_BATCH_USER_SEX_FEMALE:
					return Constants.FLG_USER_SEX_FEMALE;

				case Constants.CROSS_POINT_FLG_USER_SEX_MALE:
				case Constants.CROSS_POINT_BATCH_USER_SEX_MALE:
					return Constants.FLG_USER_SEX_MALE;

				default:
					return Constants.FLG_USER_SEX_UNKNOWN;
			}
		}

		/// <summary>
		/// CrossPoint メールアドレス重複チェック
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>メールアドレスは重複しているか</returns>
		public bool IsDuplicationCrossPointMailAddress(string mailAddr)
		{
			var crossPointUserSearchResult = new CrossPointUserApiService().Search(
				new Dictionary<string, string>
				{
					{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_PC_MAIL, mailAddr },
				});
			return (crossPointUserSearchResult.TotalResult != 0);
		}

		/// <summary>
		/// CrossPoint メールアドレスのネットショップ会員IDを取得
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>ネットショップ会員Id</returns>
		public string GetMemberIdByCrossPointMailAddress(string mailAddr)
		{
			var crossPointUserSearchResult = new CrossPointUserApiService().Search(
				new Dictionary<string, string>
				{
					{ Constants.CROSS_POINT_PARAM_MEMBER_INFO_PC_MAIL, mailAddr },
				});
			var memberId = crossPointUserSearchResult
				.Result
				.Select(user => user.NetShopMemberId)
				.FirstOrDefault();
			return memberId;
		}

		/// <summary>
		/// CROSSPOINTメールアドレス重複チェック
		/// </summary>
		/// <param name="shopCardNo">会員カード番号</param>
		/// <param name="shopCardPin">PINコード</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="isRegister">true:登録/false:更新</param>
		/// <returns>成功:空/失敗:エラーメッセージ定数</returns>
		public string CheckCrossPointDuplicationMailAddress(
			string shopCardNo,
			string shopCardPin,
			string mailAddr,
			string userId,
			bool isRegister)
		{
			var isDuplicationMailAddress = new UserUtility().IsDuplicationCrossPointMailAddress(mailAddr);
			if (isRegister && string.IsNullOrEmpty(shopCardNo)
				&& string.IsNullOrEmpty(shopCardPin) && isDuplicationMailAddress)
			{
				return w2.App.Common.CommerceMessages.ERRMSG_CROSS_POINT_REGISTER_MAILADDRESS;
			}

			if ((isRegister == false) && string.IsNullOrEmpty(shopCardPin) && isDuplicationMailAddress)
			{
				var netShopMemberId = new UserUtility().GetMemberIdByCrossPointMailAddress(mailAddr);
				if (netShopMemberId != userId)
				{
					return w2.App.Common.CommerceMessages.ERRMSG_CROSS_POINT_REGISTER_MAILADDRESS;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// CROSSPOINT会員カード番号・PIN入力チェック
		/// </summary>
		/// <param name="shopCardNo">会員カード番号</param>
		/// <param name="shopCardPin">PINコード</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="isRegister">true:登録/false:更新</param>
		/// <returns>成功:空/失敗:エラーメッセージ定数</returns>
		public string CheckShopCardNoAndPinCode(string shopCardNo, string shopCardPin, string userId, bool isRegister)
		{
			bool hasCard;
			var cardValid = CheckCardNoAndCardPin(shopCardNo, shopCardPin, out hasCard);

			if (cardValid == false)
			{
				if ((isRegister && (string.IsNullOrEmpty(shopCardNo) == false))
					|| (string.IsNullOrEmpty(shopCardPin) == false))
				{
					return CommerceMessages.ERRMSG_FRONT_POINT_CARD_REGISTER_ERROR;
				}
			}
			if (hasCard == false) return string.Empty;

			// 該当する会員が取得できない場合はエラーを返す
			var userList = new CrossPointUserApiService()
				.GetUserByCardNoAndCardPinCodeExcludingWithdrawal(shopCardNo, shopCardPin);
			if (userList.TotalResult == 0)
			{
				return CommerceMessages.ERRMSG_FRONT_USER_NOT_EXIST;
			}

			// 既に店舗会員が登録されている際はエラーを返す
			var netShopMemberId = userList.Result
				.Select(user => user.NetShopMemberId)
				.FirstOrDefault(id => (string.IsNullOrEmpty(id) == false));
			if ((string.IsNullOrEmpty(netShopMemberId) == false) && (userId != netShopMemberId))
			{
				return CommerceMessages.ERRMSG_CROSS_POINT_REGISTER_STORE_MEMBER_FAILED;
			}
			return string.Empty;
		}
	}
}
