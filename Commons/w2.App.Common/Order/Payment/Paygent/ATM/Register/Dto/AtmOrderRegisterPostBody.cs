/*
=========================================================================================================
  Module      : Atm Order Register Post Body(AtmOrderRegisterPostBody.cs)
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

namespace w2.App.Common.Order.Payment.Paygent.ATM.Register.Dto
{
	/// <summary>
	/// Atm order register post body
	/// </summary>
	[Serializable]
	public class AtmOrderRegisterPostBody : IPaygentPostBody
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="order">注文データ</param>
		public AtmOrderRegisterPostBody(CartObject cart, Hashtable order)
		{
			var sharedParameter = new PaygentSharedApiParameter();
			this.MerchantId = sharedParameter.MerchantId;
			this.ConnectId = sharedParameter.ConnectId;
			this.ConnectPassword = sharedParameter.ConnectPassword;
			this.TelegramKind = PaygentConstants.PAYGENT_TELEGRAM_KIND_ATM_ORDER_REGISTER;
			this.TelegramVersion = sharedParameter.TelegramVersion;
			this.TradingId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
			this.PaymentId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]);
			this.PaymentAmount = Math.Truncate(cart.PriceTotal).ToString();
			this.LastName = StringUtility.TruncateUtf8(StringUtility.ToZenkaku(cart.Owner.Name1), 12);
			this.FirstName = StringUtility.TruncateUtf8(StringUtility.ToZenkaku(cart.Owner.Name2), 12);
			this.LastNameKana = StringUtility.TruncateUtf8(StringUtility.ToHankakuKatakana(StringUtility.ToZenkakuKatakana(cart.Owner.NameKana1)), 12);
			this.FirstNameKana = StringUtility.TruncateUtf8(StringUtility.ToHankakuKatakana(StringUtility.ToZenkakuKatakana(cart.Owner.NameKana2)), 12);
			this.PaymentDetail = Constants.PAYMENT_PAYGENT_ATM_ORDERDETAIL_NAME;
			this.PaymentDetailKana = Constants.PAYMENT_PAYGENT_ATM_ORDERDETAIL_NAME_KANA;
			this.PaymentLimitDate = Constants.PAYMENT_PAYGENT_ATM_PAYMENT_LIMIT_DAY.ToString();
		}

		/// <summary>
		/// リクエストパラメータ生成
		/// </summary>
		/// <returns>リクエストパラメータ</returns>
		public Dictionary<string, string> GenerateKeyValues()
		{
			var parameters = new Dictionary<string, string>
			{
				{
					PaygentConstants.PAYGENT_API_REQUEST_CONTENT_TYPE,
					PaygentConstants.PAYGENT_API_REQUEST_CONTENT_TYPE_FORM_URLENCODED
				},
				{ PaygentConstants.PAYGENT_API_REQUEST_MERCHANT_ID, this.MerchantId },
				{ PaygentConstants.PAYGENT_API_REQUEST_CONNECT_ID, this.ConnectId },
				{ PaygentConstants.PAYGENT_API_REQUEST_CONNECT_PASSWORD, this.ConnectPassword },
				{ PaygentConstants.PAYGENT_API_REQUEST_TELEGRAM_KIND, this.TelegramKind },
				{ PaygentConstants.PAYGENT_API_REQUEST_TELEGRAM_VERSION, this.TelegramVersion },
				{ PaygentConstants.PAYGENT_API_REQUEST_TRADING_ID, this.TradingId },
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_ID, this.PaymentId },
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_AMOUNT, this.PaymentAmount },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_FAMILY_NAME, this.LastName.ToWithinLv2Chars() },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_NAME, this.FirstName.ToWithinLv2Chars() },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_FAMILY_NAME_KANA, this.LastNameKana },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_NAME_KANA, this.FirstNameKana },
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_DETAIL, this.PaymentDetail },
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_DETAIL_KANA, this.PaymentDetailKana },
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_LIMIT_DATE, this.PaymentLimitDate },
			};
			return parameters;
		}

		/// <summary>マーチャントID</summary>
		public string MerchantId { get; set; }
		/// <summary>接続ID</summary>
		public string ConnectId { get; set; }
		/// <summary>接続パスワード</summary>
		public string ConnectPassword { get; set; }
		/// <summary>電文種別ID</summary>
		public string TelegramKind { get; set; }
		/// <summary>電文バージョン番号</summary>
		public string TelegramVersion { get; set; }
		/// <summary>決済ID</summary>
		public string PaymentId { get; set; } = string.Empty;
		/// <summary>マーチャント取引ID</summary>
		public string TradingId { get; set; }
		/// <summary>決済金額</summary>
		public string PaymentAmount { get; private set; }
		/// <summary>利用者名</summary>
		public string FirstName { get; private set; }
		/// <summary>利用者姓</summary>
		public string LastName { get; private set; }
		/// <summary>利用者名カナ</summary>
		public string FirstNameKana { get; private set; }
		/// <summary>利用者姓カナ</summary>
		public string LastNameKana { get; private set; }
		/// <summary>支払期限日</summary>
		public string PaymentLimitDate { get; private set; }
		/// <summary>決済内容</summary>
		public string PaymentDetail { get; private set; }
		/// <summary>決済内容カナ</summary>
		public string PaymentDetailKana { get; private set; }
	}
}
