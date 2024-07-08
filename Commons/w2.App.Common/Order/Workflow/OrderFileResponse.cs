/*
=========================================================================================================
  Module      :  Order file response(OrderFileResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Order file response
	/// </summary>
	[Serializable]
	public class OrderFileResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="orderValue">Order value</param>
		/// <param name="orderText">Order text</param>
		/// <param name="orderInfo">Order info</param>
		/// <param name="canShipmentEntry">Can shipment entry</param>
		/// <param name="fileNamePattern">File nam pattern</param>
		/// <param name="mailTemplateId">Mail template id</param>
		public OrderFileResponse(
			string orderValue,
			string orderText,
			string orderInfo,
			bool canShipmentEntry,
			string fileNamePattern,
			string mailTemplateId)
		{
			this.OrderValue = orderValue;
			this.OrderText = orderText;
			this.OrderInfo = orderInfo;
			this.CanShipmentEntry = canShipmentEntry;
			this.FileNamePattern = fileNamePattern;
			this.MailTemplateId = mailTemplateId;
		}

		/// <summary>Order value</summary>
		public string OrderValue { get; set; }
		/// <summary>Order text</summary>
		public string OrderText { get; set; }
		/// <summary>Order info</summary>
		public string OrderInfo { get; set; }
		/// <summary>Can shipment entry</summary>
		public bool CanShipmentEntry { get; set; }
		/// <summary>File name pattern</summary>
		public string FileNamePattern { get; set; }
		/// <summary>Mail template id</summary>
		public string MailTemplateId { get; set; }
	}
}
