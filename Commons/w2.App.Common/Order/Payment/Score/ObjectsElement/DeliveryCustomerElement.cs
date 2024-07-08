/*
=========================================================================================================
  Module      : 配送先顧客情報(DeliveryCustomerElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 配送先顧客情報
	/// </summary>
	public class DeliveryCustomerElement
	{
		/// <summary>
		/// コンストラク
		/// </summary>
		public DeliveryCustomerElement()
		{
			this.FullName = string.Empty;
			this.FirstName = string.Empty;
			this.LastName = string.Empty;
			this.FullKanaName = string.Empty;
			this.FirstKanaName = string.Empty;
			this.LastKanaName = string.Empty;
			this.ZipCode = string.Empty;
			this.Address1 = string.Empty;
			this.Address2 = string.Empty;
			this.Address3 = string.Empty;
			this.CompanyName = string.Empty;
			this.DepartmentName = string.Empty;
			this.Tel = string.Empty;
			this.Email = string.Empty;
		}
		/// <summary>
		/// コンストラク
		/// </summary>
		/// <param name="orderShipping">注文配送情報</param>
		public DeliveryCustomerElement(OrderShippingModel orderShipping)
			: this()
		{
			this.Address1 = orderShipping.ShippingAddr1;
			this.Address2 = orderShipping.ShippingAddr2;
			this.Address3 = orderShipping.ShippingAddr3 + orderShipping.ShippingAddr4;
			this.CompanyName = orderShipping.ShippingCompanyName;
			this.DepartmentName = orderShipping.ShippingCompanyPostName;
			this.FullKanaName = orderShipping.ShippingNameKana;
			this.FirstKanaName = orderShipping.ShippingNameKana1;
			this.LastKanaName = orderShipping.ShippingNameKana2;
			this.FullName = orderShipping.ShippingName;
			this.FirstName = orderShipping.ShippingName1;
			this.LastName = orderShipping.ShippingName2;
			this.Tel = orderShipping.ShippingTel1.Replace("-", string.Empty);
			this.Email = null;
			this.ZipCode = orderShipping.ShippingZip;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shipping">配送情報</param>
		public DeliveryCustomerElement(CartShipping shipping)
			: this()
		{
			this.Address1 = shipping.Addr1;
			this.Address2 = shipping.Addr2;
			this.Address3 = shipping.Addr3 + shipping.Addr4;
			this.CompanyName = StringUtility.ToEmpty(shipping.CompanyName);
			this.DepartmentName = StringUtility.ToEmpty(shipping.CompanyPostName);
			this.FullKanaName = shipping.NameKana;
			this.FirstKanaName = shipping.NameKana1;
			this.LastKanaName = shipping.NameKana2;
			this.FullName = shipping.Name;
			this.FirstName = shipping.Name1;
			this.LastName = shipping.Name2;
			this.Tel = shipping.Tel1.Replace("-", string.Empty);
			this.Email = string.Empty;
			this.ZipCode = shipping.Zip;
		}

		/// <summary>氏名（漢字）</summary>
		[XmlElement("fullName")]
		public string FullName { get; set; }
		/// <summary>姓（漢字）</summary>
		[XmlElement("firstName")]
		public string FirstName { get; set; }
		/// <summary>名（漢字）</summary>
		[XmlElement("lastName")]
		public string LastName { get; set; }
		/// <summary>氏名（カナ）</summary>
		[XmlElement("FullKanaName")]
		public string FullKanaName { get; set; }
		/// <summary>姓（カナ）</summary>
		[XmlElement("firstKanaName")]
		public string FirstKanaName { get; set; }
		/// <summary>氏（カナ）</summary>
		[XmlElement("lastKanaName")]
		public string LastKanaName { get; set; }
		/// <summary>郵便番号</summary>
		[XmlElement("zipCode")]
		public string ZipCode { get; set; }
		/// <summary>都道府県</summary>
		[XmlElement("address1")]
		public string Address1 { get; set; }
		/// <summary>市区町村</summary>
		[XmlElement("address2")]
		public string Address2 { get; set; }
		/// <summary>その以降の住所</summary>
		[XmlElement("address3")]
		public string Address3 { get; set; }
		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName { get; set; }
		/// <summary>部署名</summary>
		[XmlElement("departmentName")]
		public string DepartmentName { get; set; }
		/// <summary>電話番号</summary>
		[XmlElement("tel")]
		public string Tel { get; set; }
		/// <summary>メールアドレス</summary>
		[XmlElement("email")]
		public string Email { get; set; }
	}
}
