/*
=========================================================================================================
  Module      : Payment SBPS Paypay Sale Api (PaymentSBPSPaypaySaleApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;

namespace w2.App.Common.Order
{
	/// <summary>
	/// Payment SBPS Paypay sale api
	/// </summary>
	public class PaymentSBPSPaypaySaleApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentSBPSPaypaySaleApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">SBPS settings</param>
		public PaymentSBPSPaypaySaleApi(PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSPaypaySaleResponseData(settings))
		{
		}

		/// <summary>
		/// Sales cooperation execution
		/// </summary>
		/// <param name="trackingId">Tracking id</param>
		/// <param name="amount">Total amount</param>
		/// <returns>Execution result</returns>
		public bool Exec(string trackingId, decimal amount)
		{
			var requestXml = CreateSaleXml(trackingId, amount);
			return Post(requestXml);
		}

		/// <summary>
		/// Api sales cooperation xml creation
		/// </summary>
		/// <param name="trackingId">Tracking id</param>
		/// <param name="amount">Amount</param>
		/// <returns>Sale xml</returns>
		private XDocument CreateSaleXml(string trackingId, decimal amount)
		{
			var document = new XDocument(new XDeclaration("1.0", "Shift_JIS", string.Empty));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "ST02-00201-311"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					new XElement("tracking_id", this.HashCalculator.Add(trackingId)),
					new XElement("pay_option_manage", new XElement("amount", this.HashCalculator.Add(amount.ToPriceString()))),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())));
			return document;
		}

		/// <summary>Response data</summary>
		public PaymentSBPSPaypaySaleResponseData ResponseData
		{
			get { return (PaymentSBPSPaypaySaleResponseData)this.ResponseDataInner; }
		}
	}
}
