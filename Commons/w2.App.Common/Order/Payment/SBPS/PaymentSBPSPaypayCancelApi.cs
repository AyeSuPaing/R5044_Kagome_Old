/*
=========================================================================================================
  Module      : Payment SBPS Paypay Cancel Api (PaymentSBPSPaypayCancelApi.cs)
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
	/// Payment SBPS Paypay cancel api
	/// </summary>
	public class PaymentSBPSPaypayCancelApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentSBPSPaypayCancelApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">SBPS settings</param>
		public PaymentSBPSPaypayCancelApi(PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSPaypayCancelResponseData(settings))
		{
		}

		/// <summary>
		/// Cancel cooperation execution
		/// </summary>
		/// <param name="trackingId">Tracking id</param>
		/// <param name="amount">Total amount</param>
		/// <returns>Execution result</returns>
		public bool Exec(string trackingId, decimal amount)
		{
			var requestXml = CreateCancelXml(trackingId, amount);
			return Post(requestXml);
		}

		/// <summary>
		/// Api cancel cooperation xml creation
		/// </summary>
		/// <param name="trackingId">Tracking id</param>
		/// <param name="amount">Total amount</param>
		/// <returns>Cancel xml</returns>
		private XDocument CreateCancelXml(string trackingId, decimal amount)
		{
			var document = new XDocument(new XDeclaration("1.0", "Shift_JIS", string.Empty));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "ST02-00303-311"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					new XElement("tracking_id", this.HashCalculator.Add(trackingId)),
					new XElement("pay_option_manage", new XElement("amount", this.HashCalculator.Add(amount.ToPriceString()))),
					new XElement("encrypted_flg", this.HashCalculator.Add("1")),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())));
			return document;
		}

		/// <summary>Response data</summary>
		public PaymentSBPSPaypayCancelResponseData ResponseData
		{
			get { return (PaymentSBPSPaypayCancelResponseData)this.ResponseDataInner; }
		}
	}
}
