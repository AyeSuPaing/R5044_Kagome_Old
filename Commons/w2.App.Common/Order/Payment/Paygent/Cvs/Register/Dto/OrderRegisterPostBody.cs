/*
=========================================================================================================
  Module      : Paygent CVS Order Register Post Body(OrderRegisterPostBody.cs)
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

namespace w2.App.Common.Order.Payment.Paygent.Cvs.Register.Dto
{
	/// <summary>
	/// Paygent CVS order register post body
	/// </summary>
	[Serializable]
	public class OrderRegisterPostBody : IPaygentPostBody
	{
		/// <summary>電文種別ID：ｺﾝﾋﾞﾆ決済(番号方式)</summary>
		private const string TELEGRAM_KIND = "030";
		/// <summary>支払種別：前払い</summary>
		private const string SALES_TYPE_PREPAYMENT = "1";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="order">注文データ</param>
		public OrderRegisterPostBody(CartObject cart, Hashtable order)
		{
			var sharedParameter = new PaygentSharedApiParameter();
			this.MerchantId = sharedParameter.MerchantId;
			this.ConnectId = sharedParameter.ConnectId;
			this.ConnectPassword = sharedParameter.ConnectPassword;
			this.TelegramKind = TELEGRAM_KIND;
			this.TelegramVersion = sharedParameter.TelegramVersion;
			this.TradingId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
			this.PaymentAmount = Math.Truncate(cart.PriceTotal).ToString();
			this.CvsCompanyId = cart.Payment.GetPaygentCvsType() ?? string.Empty;
			this.LastName = StringUtility.ToZenkaku(cart.Owner.Name1);
			this.FirstName = StringUtility.ToZenkaku(cart.Owner.Name2);
			this.LastNameKana = StringUtility.ToHankakuKatakana(StringUtility.ToZenkakuKatakana(cart.Owner.NameKana1));
			this.FirstNameKana = StringUtility.ToHankakuKatakana(StringUtility.ToZenkakuKatakana(cart.Owner.NameKana2));
			this.Tel = cart.Owner.Tel1.Replace("-", string.Empty);
			this.PaymentLimitDate = Constants.PAYMENT_PAYGENT_CVS_PAYMENT_LIMIT_DAY.ToString();
			this.SalesType = SALES_TYPE_PREPAYMENT;
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
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_ID, this.PaymentId },
				{ PaygentConstants.PAYGENT_API_REQUEST_TRADING_ID, this.TradingId },
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_AMOUNT, this.PaymentAmount },
				{ PaygentConstants.PAYGENT_API_REQUEST_CVS_TYPE, this.CvsType },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_FAMILY_NAME, this.LastName.ToWithinLv2Chars() },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_NAME, this.FirstName.ToWithinLv2Chars() },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_FAMILY_NAME_KANA, this.LastNameKana },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_NAME_KANA, this.FirstNameKana },
				{ PaygentConstants.PAYGENT_API_REQUEST_CUSTOMER_TEL, this.Tel },
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_LIMIT_DATE, this.PaymentLimitDate },
				{ PaygentConstants.PAYGENT_API_REQUEST_CVS_COMPANY_ID, this.CvsCompanyId },
				{ PaygentConstants.PAYGENT_API_REQUEST_SALES_TYPE, this.SalesType },
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
		/// <summary>決済ID</summary>
		private string PaymentId { get; set; } = string.Empty;
		/// <summary>マーチャント取引ID</summary>
		private string TradingId { get; set; }
		/// <summary>決済金額</summary>
		public string PaymentAmount { get; private set; }
		/// <summary>CVSタイプ</summary>
		public string CvsType { get; private set; } = string.Empty;
		/// <summary>利用者名</summary>
		public string FirstName { get; private set; }
		/// <summary>利用者姓</summary>
		public string LastName { get; private set; }
		/// <summary>利用者名カナ</summary>
		public string FirstNameKana { get; private set; }
		/// <summary>利用者姓カナ</summary>
		public string LastNameKana { get; private set; }
		/// <summary>利用者電話番号</summary>
		public string Tel { get; private set; }
		/// <summary>支払期限日</summary>
		public string PaymentLimitDate { get; private set; }
		/// <summary>申込コンビニ企業CD</summary>
		public string CvsCompanyId { get; private set; }
		/// <summary>支払種別</summary>
		public string SalesType { get; private set; }
	}
}
