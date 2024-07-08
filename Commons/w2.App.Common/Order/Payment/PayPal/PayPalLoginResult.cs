/*
=========================================================================================================
  Module      : ペイパルログイン結果(PayPalLoginResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using Braintree;
using Newtonsoft.Json;
using w2.App.Common.Global;

namespace w2.App.Common.Order.Payment.PayPal
{
	/// <summary>
	/// ペイパルログイン結果
	/// </summary>
	[Serializable]
	public class PayPalLoginResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paypalPayerId">ペイパルPayerId（BrainTree顧客IDとなる）</param>
		/// <param name="nonce">ナンス</param>
		/// <param name="deviceData">デバイスデータ</param>
		/// <param name="shippingAddress">配送先</param>
		public PayPalLoginResult(
			string paypalPayerId,
			string nonce,
			string deviceData,
			string shippingAddress)
		{
			var customer = CreateCustomer(paypalPayerId, nonce, deviceData);
			this.CustomerId = customer.Id;
			this.Nonce = nonce;
			this.DeviceData = deviceData;
			this.ShippingAddressPayPal = JsonConvert.DeserializeObject<PayPalAddress>(shippingAddress);
			this.AddressInfo = new W2AddressInfo(this.ShippingAddressPayPal);
			this.AccountEMail = customer.PayPalAccounts[0].Email;
		}

		/// <summary>
		/// BrainTreeカスタマ作成
		/// </summary>
		/// <param name="paypalPayerId">ペイパルPayerId（BrainTree顧客IDとなる）</param>
		/// <param name="nonce">ナンス</param>
		/// <param name="deviceData">デバイスデータ</param>
		/// <returns>カスタマ</returns>
		private Customer CreateCustomer(
			string paypalPayerId,
			string nonce,
			string deviceData)
		{
			try
			{
				var customer = Constants.PAYMENT_PAYPAL_GATEWAY.Customer.Find(paypalPayerId);
				return customer;
			}
			catch (Braintree.Exceptions.NotFoundException)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL,
					PaymentFileLogger.PaymentType.PayPal,
					PaymentFileLogger.PaymentProcessingType.CreatePaypalCustomer,
					"",
					new Dictionary<string, string>
					{
						{"paypal_payer_id", paypalPayerId} //ペイパルPayerId（BrainTree顧客IDとなる）
					});
				var resultCustomer = Constants.PAYMENT_PAYPAL_GATEWAY.Customer.Create(
					new CustomerRequest
					{
						Id = paypalPayerId,
						PaymentMethodNonce = nonce,
						DeviceData = deviceData
					});
				return resultCustomer.Target;
			}
		}

		/// <summary>BrainTree顧客ID</summary>
		public string CustomerId { get; set; }
		/// <summary>ノンス</summary>
		public string Nonce { get; set; }
		/// <summary>デバイスデータ</summary>
		public string DeviceData { get; set; }
		/// <summary>アカウントEメール</summary>
		public string AccountEMail { get; set; }
		/// <summary>PayPal配送先</summary>
		public PayPalAddress ShippingAddressPayPal { get; set; }
		/// <summary>配送先</summary>
		public W2AddressInfo AddressInfo { get; set; }

		/// <summary>
		/// アドレス情報
		/// </summary>
		[Serializable]
		public class W2AddressInfo
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="paypalAddress">ペイパルアドレス</param>
			internal W2AddressInfo(PayPalAddress paypalAddress)
			{
				var isAddressJp = GlobalAddressUtil.IsCountryJp(paypalAddress.CountryCode);
				this.Name1 = isAddressJp
					? paypalAddress.LastName
					: paypalAddress.FirstName;
				this.Name2 = isAddressJp
					? paypalAddress.FirstName
					: paypalAddress.LastName;
				this.MailAddr = paypalAddress.Email;
				this.ContryIsoCode = paypalAddress.CountryCode;
				var zips = (paypalAddress.PostalCode + "-").Split('-');
				this.Zip1 = zips[0];
				this.Zip2 = zips[1];
				this.Addr1 = isAddressJp ? paypalAddress.State : "";
				this.Addr2 = paypalAddress.City;
				this.Addr3 = paypalAddress.Line1;
				this.Addr4 = paypalAddress.Line2;
				this.Addr5 = (isAddressJp == false) ? "" : paypalAddress.State;
				var tel2 = (paypalAddress.Phone + "--").Split('-');
				this.Tel_1 = tel2[0];
				this.Tel_2 = tel2[1];
				this.Tel_3 = tel2[2];
			}

			/// <summary>氏名</summary>
			public string Name { get; set; }
			/// <summary>氏名1</summary>
			public string Name1 { get; set; }
			/// <summary>氏名2</summary>
			public string Name2 { get; set; }
			/// <summary>メールアドレス</summary>
			public string MailAddr { get; set; }
			/// <summary>国ISOコード</summary>
			public string ContryIsoCode { get; set; }
			/// <summary>郵便番号1</summary>
			public string Zip1 { get; set; }
			/// <summary>郵便番号2</summary>
			public string Zip2 { get; set; }
			/// <summary>住所1</summary>
			public string Addr1 { get; set; }
			/// <summary>住所2</summary>
			public string Addr2 { get; set; }
			/// <summary>住所3</summary>
			public string Addr3 { get; set; }
			/// <summary>住所4</summary>
			public string Addr4 { get; set; }
			/// <summary>住所5</summary>
			public string Addr5 { get; set; }
			/// <summary>電話番号1</summary>
			public string Tel_1 { get; set; }
			/// <summary>電話番号2</summary>
			public string Tel_2 { get; set; }
			/// <summary>電話番号3</summary>
			public string Tel_3 { get; set; }
		}

		/// <summary>
		/// ペイパルアドレスクラス
		/// </summary>
		[Serializable]
		[JsonObject("PayPalAddress")]
		public class PayPalAddress
		{
			[JsonProperty("email")]
			public string Email { get; set; }
			[JsonProperty("firstName")]
			public string FirstName { get; set; }
			[JsonProperty("lastName")]
			public string LastName { get; set; }
			[JsonProperty("phone")]
			public string Phone { get; set; }
			[JsonProperty("recipientName")]
			public string RecipientName { get; set; }
			[JsonProperty("line1")]
			public string Line1 { get; set; }
			[JsonProperty("line2")]
			public string Line2 { get; set; }
			[JsonProperty("city")]
			public string City { get; set; }
			[JsonProperty("state")]
			public string State { get; set; }
			[JsonProperty("postalCode")]
			public string PostalCode { get; set; }
			[JsonProperty("countryCode")]
			public string CountryCode { get; set; }
		}
	}
}
