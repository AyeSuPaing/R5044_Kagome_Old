/*
=========================================================================================================
  Module      : トークン取得向け決済情報取得処理(UserCreditCardCooperationInfoFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Sql;
using w2.Domain.User;

namespace w2.App.Common.Order.UserCreditCardCooperationInfos
{
	/// <summary>
	/// トークン取得向け決済情報取得処理
	/// </summary>
	public class UserCreditCardCooperationInfoFacade
	{
		/// <summary>
		/// トークン取得用に作成
		/// </summary>
		/// <returns>トークン取得向け決済情報</returns>
		public static UserCreditCardCooperationInfoBase CreateForGetToken()
		{
			// ヤマト決済ではトークン作成時にすでに連携IDは必要
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.YamatoKwc:
					return new UserCreditCardCooperationInfoYamato();

				case Constants.PaymentCard.Gmo:
				case Constants.PaymentCard.SBPS:
				case Constants.PaymentCard.Zeus:
				case Constants.PaymentCard.EScott:
				case Constants.PaymentCard.VeriTrans:
				default:
					return new UserCreditCardCooperationInfoDummy();
			}
		}

		/// <summary>
		/// 与信向けに作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">カード枝番。GMOには必要</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>トークン取得向け決済情報</returns>
		public static UserCreditCardCooperationInfoBase CreateForAuth(
			string userId,
			int branchNo,
			SqlAccessor accessor = null)
		{
			// ゼウス・SBPSはモバイルでも利用するため、こちらにないといけない
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Zeus:
					var user = new UserService().Get(userId, accessor);
					var telNo = user.Tel1_1 + user.Tel1_2 + user.Tel1_3;
					return new UserCreditCardCooperationInfoZeus(
						string.IsNullOrEmpty(telNo) ? user.Tel1.Replace("-", "") : telNo);

				case Constants.PaymentCard.SBPS:
					return new UserCreditCardCooperationInfoSbps(userId);

				case Constants.PaymentCard.Gmo:
					return new UserCreditCardCooperationInfoGmo(userId, accessor);

				case Constants.PaymentCard.Zcom:
					return new UserCreditCardCooperationInfoZcom(userId);

				case Constants.PaymentCard.EScott:
					return new UserCreditCardCooperationInfoEScott(userId, branchNo.ToString());

				case Constants.PaymentCard.VeriTrans:
					return new UserCreditCardCooperationInfoVeritrans(userId);

				case Constants.PaymentCard.Paygent:
					return new UserCreditCardCooperationInfoPaygent(userId, accessor);

				case Constants.PaymentCard.YamatoKwc:
				default:
					return new UserCreditCardCooperationInfoDummy();
			}
		}

		/// <summary>
		/// 仮クレジットカード作成向けに作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="cardBranchNo">カード枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>トークン取得向け決済情報</returns>
		public static UserCreditCardCooperationInfoBase CreateForCreateProvisionalCreditCard(
			string userId,
			int cardBranchNo,
			SqlAccessor accessor = null)
		{
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Gmo:
				case Constants.PaymentCard.Zeus:
				case Constants.PaymentCard.EScott:
				case Constants.PaymentCard.VeriTrans:
					return CreateForAuth(userId, cardBranchNo, accessor);

				case Constants.PaymentCard.YamatoKwc:
					return CreateForGetToken();

				case Constants.PaymentCard.SBPS:
				default:
					return new UserCreditCardCooperationInfoDummy();
			}
		}
	}
}
