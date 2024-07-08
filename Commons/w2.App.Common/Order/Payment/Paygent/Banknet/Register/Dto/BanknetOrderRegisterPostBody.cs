/*
=========================================================================================================
  Module      : Paygent Banknet Order Register Post Body(BanknetOrderRegisterPostBody.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.App.Common.Order.Payment.Paygent.Foundation.SharedDto;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.Paygent.Banknet.Register.Dto
{
	/// <summary>
	/// Paygent Banknet order register post body
	/// </summary>
	[Serializable]
	public class BanknetOrderRegisterPostBody : IPaygentPostBody
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="order">注文データ</param>
		public BanknetOrderRegisterPostBody(CartObject cart, Hashtable order)
		{
			var sharedParameter = new PaygentSharedApiParameter();
			this.MerchantId = sharedParameter.MerchantId;
			this.ConnectId = sharedParameter.ConnectId;
			this.ConnectPassword = sharedParameter.ConnectPassword;
			this.TelegramKind = PaygentConstants.PAYGENT_TELEGRAM_KIND_BANKNET_ORDER_REGISTER;
			this.TelegramVersion = sharedParameter.TelegramVersion;
			this.TradingId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
			this.PaymentAmount = Math.Truncate(cart.PriceTotal).ToString();
			this.ClaimKana = Constants.PAYMENT_PAYGENT_BANKNET_ORDERDETAIL_NAME_KANA;
			this.ClaimKanji = Constants.PAYMENT_PAYGENT_BANKNET_ORDERDETAIL_NAME;
			this.LastName = StringUtility.TruncateUtf8(StringUtility.ToZenkaku(cart.Owner.Name1), 12);
			this.FirstName = StringUtility.TruncateUtf8(StringUtility.ToZenkaku(cart.Owner.Name2), 12);
			this.LastNameKana = StringUtility.TruncateUtf8(StringUtility.ToHankakuKatakana(StringUtility.ToZenkakuKatakana(cart.Owner.NameKana1)), 12);
			this.FirstNameKana = StringUtility.TruncateUtf8(StringUtility.ToHankakuKatakana(StringUtility.ToZenkakuKatakana(cart.Owner.NameKana2)), 12);
			this.AspPaymentTerm = $"1{Constants.PAYMENT_PAYGENT_BANKNET_PAYMENT_LIMIT_DAY:00}0000";
		}

		/// <summary>
		/// リクエストパラメータ生成
		/// </summary>
		/// <returns>リクエストパラメータ</returns>
		public Dictionary<string, string> GenerateKeyValues()
		{
			var parameters = new Dictionary<string, string>
			{
				{ PaygentConstants.PAYGENT_API_REQUEST_MERCHANT_ID, this.MerchantId },
				{ PaygentConstants.PAYGENT_API_REQUEST_CONNECT_ID, this.ConnectId },
				{ PaygentConstants.PAYGENT_API_REQUEST_CONNECT_PASSWORD, this.ConnectPassword },
				{ PaygentConstants.PAYGENT_API_REQUEST_TELEGRAM_KIND, this.TelegramKind },
				{ PaygentConstants.PAYGENT_API_REQUEST_TELEGRAM_VERSION, this.TelegramVersion },
				{ PaygentConstants.PAYGENT_API_REQUEST_TRADING_ID, this.TradingId },
				{ PaygentConstants.PAYGENT_API_REQUEST_AMOUNT, this.PaymentAmount },
				{ PaygentConstants.PAYGENT_API_REQUEST_CLAIM_KANA, this.ClaimKana },
				{ PaygentConstants.PAYGENT_API_REQUEST_CLAIM_KANJI, this.ClaimKanji },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_FAMILY_NAME, this.LastName.ToWithinLv2Chars() },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_NAME, this.FirstName.ToWithinLv2Chars() },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_FAMILY_NAME_KANA, this.LastNameKana },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_NAME_KANA, this.FirstNameKana },
				{ PaygentConstants.PAYGENT_API_REQUEST_ASP_PAYMENT_TERM, this.AspPaymentTerm },
			};
			return parameters;
		}

		/// <summary>マーチャントID</summary>
		private string MerchantId { get; set; }
		/// <summary>接続ID</summary>
		private string ConnectId { get; set; }
		/// <summary>接続パスワード</summary>
		private string ConnectPassword { get; set; }
		/// <summary>電文種別ID</summary>
		private string TelegramKind { get; set; }
		/// <summary>電文バージョン番号</summary>
		private string TelegramVersion { get; set; }
		/// <summary>マーチャント取引ID</summary>
		private string TradingId { get; set; }
		/// <summary>決済金額</summary>
		public string PaymentAmount { get; private set; }
		/// <summary>請求内容カナ</summary>
		public string ClaimKana { get; private set; }
		/// <summary>請求内容漢字</summary>
		public string ClaimKanji { get; private set; }
		/// <summary>利用者名</summary>
		public string FirstName { get; private set; }
		/// <summary>利用者姓</summary>
		public string LastName { get; private set; }
		/// <summary>利用者名カナ</summary>
		public string FirstNameKana { get; private set; }
		/// <summary>利用者姓カナ</summary>
		public string LastNameKana { get; private set; }
		/// <summary>支払期間</summary>
		public string AspPaymentTerm { get; private set; }
	}
}
