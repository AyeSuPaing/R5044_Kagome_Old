/*
=========================================================================================================
  Module      : 配送先項目要素(DeliveriesElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 配送先項目要素
	/// </summary>
	public class DeliveryElements
	{
		/// <summary>コンストラクタ</summary>
		public DeliveryElements()
		{
			this.Delivery = new DeliveryElement[] { };
		}

		/// <summary>配送先情報</summary>
		[XmlElement("delivery")]
		public DeliveryElement[] Delivery { get; set; }
	}

	#region DeliveryElement 配送先情報要素
	/// <summary>
	/// 配送先情報要素
	/// </summary>
	public class DeliveryElement
	{
		/// <summary>コンストラクタ</summary>
		public DeliveryElement()
		{
			this.DeliveryCustomer = new DeliveryCustomerElement();
			this.Details = new DetailsElement();
		}

		/// <summary>配送先顧客情報</summary>
		[XmlElement("deliveryCustomer")]
		public DeliveryCustomerElement DeliveryCustomer { get; set; }
		/// <summary>明細項目</summary>
		[XmlElement("details")]
		public DetailsElement Details { get; set; }
	}
	#endregion
}