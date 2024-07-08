/*
=========================================================================================================
  Module      : ユーザークレジットカードヘルパ(UserCreditCardHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ユーザークレジットカードヘルパ
	/// </summary>
	public class UserCreditCardHelper
	{
		/// <summary>
		/// クレジットカード情報文字列(HTML)作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <returns>クレジットカード情報文字列(HTML)</returns>
		public static string CreateCreditCardInfoHtml(string userId, int branchNo)
		{
			var userCreditCard = new UserCreditCardService().Get(userId, branchNo);
			var result = CreateCreditCardInfoHtml(userCreditCard);
			return result;
		}
		/// <summary>
		/// クレジットカード情報文字列(HTML)作成
		/// </summary>
		/// <param name="userCreditCard">ユーザークレジットカード情報</param>
		/// <param name="dispCardDispName">カード登録名表示するか</param>
		/// <returns>クレジットカード情報文字列(HTML)</returns>
		public static string CreateCreditCardInfoHtml(UserCreditCardModel userCreditCard, bool dispCardDispName = true)
		{
			var result = HtmlSanitizer.HtmlEncodeChangeToBr(CreateCreditCardInfo(userCreditCard, dispCardDispName));
			return result;
		}
		/// <summary>
		/// 永久トークン向けクレジットカード情報文字列(HTML)作成
		/// </summary>
		/// <param name="userCreditCard">ユーザークレジットカード情報</param>
		/// <param name="dispCardDispName">カード登録名表示するか</param>
		/// <returns>クレジットカード情報文字列(HTML)</returns>
		public static string CreateCreditCardInfoHtmlForTokenaizedPan(UserCreditCardModel userCreditCard, bool dispCardDispName = true)
		{
			var result = HtmlSanitizer.HtmlEncodeChangeToBr(CreateCreditCardInfo(userCreditCard, dispCardDispName, true));
			return result;
		}

		/// <summary>
		/// クレジットカード情報文字列作成
		/// </summary>
		/// <param name="userCreditCard">ユーザークレジットカード情報</param>
		/// <param name="dispCardDispName">カード登録明照寺するか</param>
		/// <param name="forTokenaizedPan">永久トークン向けか</param>
		/// <returns>クレジットカード情報文字列(HTML)</returns>
		public static string CreateCreditCardInfo(UserCreditCardModel userCreditCard, bool dispCardDispName = true, bool forTokenaizedPan = false)
		{
			var format = string.Empty;
			if (dispCardDispName && userCreditCard.IsDisp && (string.IsNullOrEmpty(userCreditCard.CardDispName) == false))
			{
				format += CreateUserCreditCardNameDispFormat() + "\r\n";
			}
			if (OrderCommon.CreditCompanySelectable && (string.IsNullOrEmpty(userCreditCard.CompanyCode) == false))
			{
				format += "カード会社 ： {1} \r\n";
			}
			format += forTokenaizedPan
				? "永久トークン ： {2} \r\n"
				: "カード番号 ： {2} \r\n";
			format += "有効期限 ： {3}/{4} (月/年) \r\n";

			// e-SCOTTかつカード名義が空の場合、カード名義人を非表示にする
			if ((OrderCommon.IsPaymentCardTypeEScott == false)
				|| (string.IsNullOrEmpty(userCreditCard.AuthorName) == false))
			{
				var cardName = string.IsNullOrEmpty(userCreditCard.AuthorName)
					? string.Empty
					: userCreditCard.AuthorName;
				format += string.Format("カード名義人 ： {0} \r\n", cardName);
			}

			if ((string.IsNullOrEmpty(userCreditCard.CooperationId) == false)
				|| (string.IsNullOrEmpty(userCreditCard.CooperationId2) == false))
			{
				var cooperationInfo = new UserCreditCard.UserCardCooperationInfo(userCreditCard);
				if (OrderCommon.IsPaymentCardTypeGmo)
				{
					format += string.Format("(GMO会員ID : {0})\r\n", cooperationInfo.GMOMemberId);
				}
			}

			var result = string.Empty;
			if ((userCreditCard.CardDispName != null)
				&& (userCreditCard.ExpirationMonth != null)
				&& (userCreditCard.ExpirationYear != null)
				&& (userCreditCard.AuthorName != null)
				&& (userCreditCard.CompanyCode != null)
				&& (userCreditCard.LastFourDigit != null))
			{
				result =
					string.Format(
						format,
						userCreditCard.CardDispName,
						ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName, userCreditCard.CompanyCode),
						(forTokenaizedPan ? "" : "************") + userCreditCard.LastFourDigit,
						userCreditCard.ExpirationMonth,
						userCreditCard.ExpirationYear);
			}
			return result;
		}

		/// <summary>
		/// クレジットカード登録名表示フォーマット作成
		/// </summary>
		/// <returns>フォーマット</returns>
		public static string CreateUserCreditCardNameDispFormat()
		{
			return "カード登録名 ： {0}";
		}

		/// <summary>
		/// クレジットカード表示文字列（「************1234」）取得
		/// </summary>
		/// <param name="cardNo">クレジットカード番号(全体)</param>
		/// <returns>クレジットカード表示文字列</returns>
		public static string CreateCreditCardNoDispString(string cardNo)
		{
			string cardNoTmp = "    " + cardNo;
			return cardNoTmp.Substring(cardNoTmp.Length - 4, 4);
		}
	}
}
