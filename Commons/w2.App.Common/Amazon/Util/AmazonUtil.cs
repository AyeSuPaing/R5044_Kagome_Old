/*
=========================================================================================================
  Module      : Amazonユーティリティ(AmazonUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Order;
using w2.Common.Web;
using w2.Domain.Payment;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Amazon.Util
{
	/// <summary>
	/// Amazonユーティリティクラス
	/// </summary>
	public class AmazonUtil
	{
		/// <summary>
		/// Amazonアカウント認証
		/// </summary>
		/// <param name="token">トークン</param>
		/// <returns>成功：Amazonモデル失敗：null</returns>
		public static AmazonModel AuthenticationAmazon(string token)
		{
			if (string.IsNullOrEmpty(token)) return null;

			// Amazonアカウント情報取得
			if (Constants.AMAZON_PAYMENT_CV2_ENABLED)
			{
				var buyer = new AmazonCv2ApiFacade().GetBuyer(token);

				// アカウント情報チェック
				if ((buyer == null)
					|| (string.IsNullOrEmpty(buyer.BuyerId))
					|| (string.IsNullOrEmpty(buyer.Name)
					|| (string.IsNullOrEmpty(buyer.Email))))
				{
					return null;
				}

				var amazonModel = new AmazonModel(token, buyer);
				return amazonModel;
			}
			else
			{
				var userInfo = AmazonApiFacade.GetUserInfo(token);

				// アカウント情報チェック
				if ((userInfo == null)
				|| (string.IsNullOrEmpty(userInfo.UserId))
				|| (string.IsNullOrEmpty(userInfo.Name)
				|| (string.IsNullOrEmpty(userInfo.Email))))
				{
					return null;
				}

				// アカウント情報モデル生成
				var amazonModel = new AmazonModel(token, userInfo);
				return amazonModel;
			}
		}

		/// <summary>
		/// ユーザー拡張項目にAmazonユーザーIDをセット
		/// </summary>
		/// <param name="userExtend">ユーザー拡張項目</param>
		/// <param name="w2UserId">ユーザーID</param>
		/// <param name="amazonUserId">AmazonユーザーID</param>
		public static void SetAmazonUserIdForUserExtend(UserExtendModel userExtend, string w2UserId, string amazonUserId)
		{
			userExtend.UserExtendDataValue[Constants.AMAZON_USER_ID_USEREXTEND_COLUMN_NAME] = amazonUserId;
			new UserService().UpdateUserExtend(
				userExtend,
				w2UserId,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);
		}

		/// <summary>
		/// ユーザー拡張項目からAmazonユーザーIDを除去
		/// </summary>
		/// <param name="userExtend">ユーザー拡張項目</param>
		/// <param name="w2UserId">ユーザーID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void RemoveAmazonUserIdFromUserExtend(
			UserExtendModel userExtend,
			string w2UserId,
			UpdateHistoryAction updateHistoryAction)
		{
			userExtend.UserExtendDataValue[Constants.AMAZON_USER_ID_USEREXTEND_COLUMN_NAME] = string.Empty;
			new UserService().UpdateUserExtend(
				userExtend,
				w2UserId,
				Constants.FLG_LASTCHANGED_USER,
				updateHistoryAction);
		}

		/// <summary>
		/// AmazonユーザーIDからユーザー情報取得
		/// </summary>
		/// <param name="amazonUserId">AmazonユーザーID</param>
		/// <returns>ユーザーモデル</returns>
		public static UserModel GetUserByAmazonUserId(string amazonUserId)
		{
			if (Constants.AMAZON_LOGIN_OPTION_ENABLED == false) return null;

			// 列の存在チェック
			var userService = new UserService();
			if (userService.UserExtendColumnExists(Constants.AMAZON_USER_ID_USEREXTEND_COLUMN_NAME) == false) return null;

			// ユーザーID取得
			var user = userService.GetUserByExternalUserId(Constants.AMAZON_USER_ID_USEREXTEND_COLUMN_NAME, amazonUserId);

			return user;
		}

		/// <summary>
		/// ユーザーIDからAmazonユーザーID取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>AmazonユーザーID</returns>
		public static string GetAmazonUserIdByUseId(string userId)
		{
			if (Constants.AMAZON_LOGIN_OPTION_ENABLED == false) return null;

			// 列の存在チェック
			var userService = new UserService();
			if (userService.UserExtendColumnExists(Constants.AMAZON_USER_ID_USEREXTEND_COLUMN_NAME) == false) return null;

			// AmazonユーザーID取得
			var userExtend = userService.GetUserExtend(userId);
			var amazonUserId = userExtend.UserExtendDataValue[Constants.AMAZON_USER_ID_USEREXTEND_COLUMN_NAME];

			return amazonUserId;
		}

		/// <summary>
		/// コールバックURL作成
		/// </summary>
		/// <param name="callbackPath">コールバックパス</param>
		/// <param name="nextUrl">画面遷移先</param>
		/// <returns>コールバックURL</returns>
		public static string CreateCallbackUrl(string callbackPath, string nextUrl = "")
		{
			var callbackUrl = new UrlCreator(Constants.PATH_ROOT + callbackPath);
			if (string.IsNullOrEmpty(nextUrl) == false)
				callbackUrl.AddParam(Constants.REQUEST_KEY_FRONT_NEXT_URL, nextUrl);


			return callbackUrl.CreateUrl();
		}

		/// <summary>
		/// 有効決済種別からAmazonPayを除去
		/// </summary>
		/// <param name="validPayments">有効決済種別</param>
		/// <returns>AmazonPayを除去した有効決済種別</returns>
		public static PaymentModel[] RemoveAmazonPayFromValidPayments(PaymentModel[] validPayments)
		{
			var result = validPayments
				.Where(payment => (OrderCommon.IsAmazonPayment(payment.PaymentId) == false))
				.ToArray();
			return result;
		}
	}
}
