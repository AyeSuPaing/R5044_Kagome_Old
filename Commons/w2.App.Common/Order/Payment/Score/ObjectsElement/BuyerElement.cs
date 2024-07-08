/*
=========================================================================================================
  Module      : 購入者情報要素(BuyerElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Xml.Serialization;
using w2.App.Common.Extensions.Currency;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 購入者情報要素
	/// </summary>
	public class BuyerElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BuyerElement()
		{
			this.NissenTransactionId = string.Empty;
			this.ShopTransactionId = string.Empty;
			this.ShopOrderDate = string.Empty;
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
			this.Mobile = string.Empty;
			this.Email = string.Empty;
			this.MobileEmail = string.Empty;
			this.BilledAmount = "0";
			this.PaymentType = ScorePaymentType.IncludeService;
		}
		/// <summary>
		/// コンストラク
		/// </summary>
		/// <param name="cart">カート情報</param>
		public BuyerElement(CartObject cart)
			: this()
		{
			this.Address1 = cart.Owner.Addr1;
			this.Address2 = cart.Owner.Addr2;
			this.Address3 = cart.Owner.Addr3 + cart.Owner.Addr4;
			this.BilledAmount = cart.PriceTotal.ToPriceString();
			this.CompanyName = StringUtility.ToEmpty(cart.Owner.CompanyName);
			this.DepartmentName = StringUtility.ToEmpty(cart.Owner.CompanyPostName);
			this.Email = StringUtility.ToEmpty(cart.Owner.MailAddr);
			this.MobileEmail = cart.Owner.MailAddr2;
			this.FullKanaName = cart.Owner.NameKana;
			this.FirstKanaName = cart.Owner.NameKana1;
			this.LastKanaName = cart.Owner.NameKana2;
			this.FullName = cart.Owner.Name;
			this.FirstName = cart.Owner.Name1;
			this.LastName = cart.Owner.Name2;
			this.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			this.ShopTransactionId = cart.OrderId;
			this.Tel = cart.Owner.Tel1.Replace("-", string.Empty);
			this.Mobile = cart.Owner.Tel2.Replace("-", string.Empty);
			this.ZipCode = cart.Owner.Zip;
			this.PaymentType = cart.GetInvoiceBundleFlg() == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				? ScorePaymentType.IncludeService
				: ScorePaymentType.SeparateService;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public BuyerElement(OrderModel order)
			: this()
		{
			this.NissenTransactionId = order.CardTranId;
			this.Address1 = order.Owner.OwnerAddr1;
			this.Address2 = order.Owner.OwnerAddr2;
			this.Address3 = order.Owner.OwnerAddr3 + order.Owner.OwnerAddr4;
			this.BilledAmount = order.LastBilledAmount.ToPriceString();
			this.CompanyName = order.Owner.OwnerCompanyName;
			this.DepartmentName = order.Owner.OwnerCompanyPostName;
			this.Email = order.Owner.OwnerMailAddr;
			this.MobileEmail = order.Owner.OwnerMailAddr2;
			this.FirstName = order.Owner.OwnerName1;
			this.LastName = order.Owner.OwnerName2;
			this.FullName = order.Owner.OwnerName;
			this.FullKanaName = order.Owner.OwnerNameKana;
			this.FirstKanaName = order.Owner.OwnerNameKana1;
			this.LastKanaName = order.Owner.OwnerNameKana2;
			this.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			this.ShopTransactionId = order.PaymentOrderId;
			this.Tel = order.Owner.OwnerTel1.Replace("-", string.Empty);
			this.Mobile = order.Owner.OwnerTel2.Replace("-", string.Empty);
			this.ZipCode = order.Owner.OwnerZip;
			this.PaymentType = order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				? ScorePaymentType.IncludeService
				: ScorePaymentType.SeparateService;
		}

		/// <summary>スコア注文ID </summary>
		[XmlElement("nissenTransactionId")]
		public string NissenTransactionId { get; set; }
		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId { get; set; }
		/// <summary>加盟店注文日</summary>
		[XmlElement("shopOrderDate")]
		public string ShopOrderDate { get; set; }
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
		/// <summary>携帯電話番号</summary>
		[XmlElement("mobile")]
		public string Mobile { get; set; }
		/// <summary>メールアドレス</summary>
		[XmlElement("email")]
		public string Email { get; set; }
		/// <summary>携帯メールアドレス</summary>
		[XmlElement("mobileEmail")]
		public string MobileEmail { get; set; }
		/// <summary>顧客請求額</summary>
		[XmlElement("billedAmount")]
		public string BilledAmount { get; set; }
		/// <summary>決済種別</summary>
		[XmlElement("paymentType")]
		public ScorePaymentType PaymentType { get; set; }
	}
}
