/*
=========================================================================================================
  Module      : Gmo Request Reissue(GmoRequestReissue.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.GMO.Reissue
{
	/// <summary>
	/// Request value of invoice reissue application API
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestReissue : BaseGmoRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public GmoRequestReissue()
			: base()
		{
			this.Transaction = new TransactionElement();
			this.KindInfo = new KindInfoElement();
			this.Buyer = new BuyerElement();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="order">Order information</param>
		public GmoRequestReissue(OrderModel order)
			: base()
		{
			this.Transaction = new TransactionElement
			{
				GmoTransactionId = order.CardTranId
			};

			// Same destination only
			this.KindInfo = new KindInfoElement
			{
				DestinationKind = DestinationKindType.ALSO
			};

			// Lost fixed
			this.Buyer = new BuyerElement(order)
			{
				RequestedReasonCode = ReissueRequestedReasonCode.LOST
			};
		}

		/// <summary>Transaction information</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction { get; set; }
		/// <summary>Update type information</summary>
		[XmlElement("kindInfo")]
		public KindInfoElement KindInfo { get; set; }
		/// <summary>Buyer information</summary>
		[XmlElement("buyer")]
		public BuyerElement Buyer { get; set; }
	}

	#region Transaction information element
	/// <summary>
	/// Transaction information element
	/// </summary>
	public class TransactionElement
	{
		/// <summary>GMO transaction ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId { get; set; }
	}
	#endregion

	#region Update type information element
	/// <summary>
	/// Update type information element
	/// </summary>
	public class KindInfoElement
	{
		/// <summary>Destination classification</summary>
		[XmlElement("destinationKind")]
		public DestinationKindType DestinationKind { get; set; }
	}
	#endregion

	#region Buyer information element
	/// <summary>
	/// Buyer information element
	/// </summary>
	public class BuyerElement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public BuyerElement()
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="order">Order information</param>
		public BuyerElement(OrderModel order)
			: this()
		{
			this.FullName = order.Owner.OwnerName;
			this.FullKanaName = order.Owner.OwnerNameKana;
			this.Tel1 = order.Owner.OwnerTel1;
			this.Tel2 = order.Owner.OwnerTel2;
			this.Email1 = order.Owner.OwnerMailAddr;
			this.Email2 = order.Owner.OwnerMailAddr2;
			this.ZipCode = order.Owner.OwnerZip;
			this.Address = order.Owner.ConcatenateAddressWithoutCountryName();
			this.CompanyName = order.Owner.OwnerCompanyName;
			this.DepartmentName = order.Owner.OwnerCompanyPostName;
		}

		/// <summary>Request reason code</summary>
		[XmlElement("requestedReasonCode")]
		public ReissueRequestedReasonCode RequestedReasonCode { get; set; }
		/// <summary>Other reasons for request</summary>
		[XmlElement("requestedOtherReason")]
		public string RequestedOtherReason { get; set; }
		/// <summary>Name (Kanji)</summary>
		[XmlElement("fullName")]
		public string FullName { get; set; }
		/// <summary>Name (Kana)</summary>
		[XmlElement("fullKanaName")]
		public string FullKanaName { get; set; }
		/// <summary>Phone number 1</summary>
		[XmlElement("tel1")]
		public string Tel1 { get; set; }
		/// <summary>Phone number 2</summary>
		[XmlElement("tel2")]
		public string Tel2 { get; set; }
		/// <summary>Email address 1</summary>
		[XmlElement("email1")]
		public string Email1 { get; set; }
		/// <summary>Email address 2</summary>
		[XmlElement("email2")]
		public string Email2 { get; set; }
		/// <summary>Zip code</summary>
		[XmlElement("zipCode")]
		public string ZipCode { get; set; }
		/// <summary>Address</summary>
		[XmlElement("address")]
		public string Address { get; set; }
		/// <summary>Company name</summary>
		[XmlElement("companyName")]
		public string CompanyName { get; set; }
		/// <summary>Department name</summary>
		[XmlElement("departmentName")]
		public string DepartmentName { get; set; }
	}
	#endregion
}
