/*
=========================================================================================================
  Module      : Payment Boku Charge Request(PaymentBokuChargeRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.Boku.Request
{
	/// <summary>
	/// Payment Boku Charge Request
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "charge-request", IsNullable = false, Namespace = "")]
	public class PaymentBokuChargeRequest : PaymentBokuBaseRequest
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuChargeRequest()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuChargeRequest(PaymentBokuSetting setting)
			: base(setting)
		{
			this.TimeOut = new TimeOutElement();
			this.Subscription = new SubscriptionElement();
			this.TimeOut.After = setting.Timeout;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Set value to request
		/// </summary>
		/// <param name="currency">Currency</param>
		/// <param name="merchantData">Merchant data</param>
		/// <param name="merchantItemDescription">Merchant item description</param>
		/// <param name="merchantTransactionId">Merchant transaction id</param>
		/// <param name="optinId">Optin id</param>
		/// <param name="totalAmount">Total amount</param>
		/// <param name="isIncludeTax">Is include tax</param>
		/// <param name="consumerIpAddress">Consumer ip address</param>
		/// <param name="isSubscription">Is subscription</param>
		/// <param name="renewal">Renewal</param>
		/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
		/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
		public void SetValue(
			string currency,
			string merchantData,
			string merchantItemDescription,
			string merchantTransactionId,
			string optinId,
			string totalAmount,
			bool isIncludeTax,
			string consumerIpAddress,
			bool isSubscription,
			bool renewal,
			string fixedPurchaseKbn,
			string fixedPurchaseSetting)
		{
			this.Currency = currency;
			this.MerchantData = StringUtility.ToNull(merchantData);
			this.MerchantItemDescription = merchantItemDescription;
			this.MerchantTransactionId = merchantTransactionId;
			this.OptinId = optinId;
			this.TotalAmount = totalAmount;
			this.TaxMode = (isIncludeTax) ? BokuConstants.CONST_BOKU_TAX_MODE_INCTAX : BokuConstants.CONST_BOKU_TAX_MODE_EXTAX;
			this.ConsumerIpAddress = (consumerIpAddress != "::1") ? consumerIpAddress : null;
			this.Subscription.IsSubscription = isSubscription;
			if (isSubscription)
			{
				this.Subscription.Renewal = renewal;
				SetPeriod(fixedPurchaseKbn, fixedPurchaseSetting);
			}
		}

		/// <summary>
		/// Set period
		/// </summary>
		/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
		/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
		private void SetPeriod(string fixedPurchaseKbn, string fixedPurchaseSetting)
		{
			var unit = BokuConstants.CONST_BOKU_CHARGE_SUBCRIPTION_PERIOD_UNIT_DAY;
			var count = 0;

			// 配送周期をAmazonAPi用に変換
			switch (fixedPurchaseKbn)
			{
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					unit = BokuConstants.CONST_BOKU_CHARGE_SUBCRIPTION_PERIOD_UNIT_MONTH;
					int.TryParse(fixedPurchaseSetting.Split(',')[0], out count);
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					unit = BokuConstants.CONST_BOKU_CHARGE_SUBCRIPTION_PERIOD_UNIT_WEEK;
					int.TryParse(fixedPurchaseSetting.Split(',')[0], out count);
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
					unit = BokuConstants.CONST_BOKU_CHARGE_SUBCRIPTION_PERIOD_UNIT_DAY;
					int.TryParse(fixedPurchaseSetting, out count);
					break;
			}

			this.Subscription.Period.Unit = unit;
			this.Subscription.Period.Count = count;
		}
		#endregion

		#region Properties
		/// <summary>Optin id</summary>
		[XmlElement("optin-id")]
		public string OptinId { get; set; }
		/// <summary>Currency</summary>
		[XmlElement("currency")]
		public string Currency { get; set; }
		/// <summary>Merchant data</summary>
		[XmlElement("merchant-data")]
		public string MerchantData { get; set; }
		/// <summary>Merchant item description</summary>
		[XmlElement("merchant-item-description")]
		public string MerchantItemDescription { get; set; }
		/// <summary>Merchant transaction id</summary>
		[XmlElement("merchant-transaction-id")]
		public string MerchantTransactionId { get; set; }
		/// <summary>Total amount</summary>
		[XmlElement("total-amount")]
		public string TotalAmount { get; set; }
		/// <summary>Tax mode</summary>
		[XmlElement("tax-mode")]
		public string TaxMode { get; set; }
		/// <summary>Consumer ip address</summary>
		[XmlElement("consumer-ip-address")]
		public string ConsumerIpAddress { get; set; }
		/// <summary>Time out</summary>
		[XmlElement("timeout")]
		public TimeOutElement TimeOut { get; set; }
		/// <summary>Subscription</summary>
		[XmlElement("subcription")]
		public SubscriptionElement Subscription { get; set; }
		#endregion
	}

	/// <summary>
	/// Time Out Element
	/// </summary>
	public class TimeOutElement
	{
		/// <summary>After</summary>
		[XmlAttribute("after")]
		public string After { get; set; }
	}

	/// <summary>
	/// Subscription Element
	/// </summary>
	public class SubscriptionElement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SubscriptionElement()
		{
			this.Period = new PeriodElement();
		}

		/// <summary>Is subscription</summary>
		[XmlAttribute("is-subcription")]
		public bool IsSubscription { get; set; }
		/// <summary>Period</summary>
		[XmlElement("period")]
		public PeriodElement Period { get; set; }
		/// <summary>Renewal</summary>
		[XmlElement("renewal")]
		public bool Renewal { get; set; }
	}

	/// <summary>
	/// Period Element
	/// </summary>
	public class PeriodElement
	{
		/// <summary>Unit</summary>
		[XmlAttribute("unit")]
		public string Unit { get; set; }
		/// <summary>Count</summary>
		[XmlAttribute("count")]
		public int Count { get; set; }
	}
}
