/*
=========================================================================================================
  Module      : DSK後払い注文情報登録リクエスト(DSKDeferredOrderRegisterRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.DSKDeferred.OrderRegister
{
	/// <summary>
	/// DSK後払い注文情報登録リクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false)]
	public class DskDeferredOrderRegisterRequest : BaseDskDeferredRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DskDeferredOrderRegisterRequest()
		{
			this.HttpInfo = new HttpInfoElement();
			this.Buyer = new BuyerElement();
			this.Deliveries = new DeliveriesElement();
		}

		/// <summary>端末情報</summary>
		[XmlElement("httpInfo")]
		public HttpInfoElement HttpInfo;
		/// <summary>購入者情報</summary>
		[XmlElement("buyer")]
		public BuyerElement Buyer;
		/// <summary>配送先項目</summary>
		[XmlElement("deliveries")]
		public DeliveriesElement Deliveries;
	}

	/// <summary>
	/// 端末情報要素
	/// </summary>
	public class HttpInfoElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HttpInfoElement()
		{
			this.HttpHeader = "";
			this.DeviceInfo = "";
		}

		/// <summary>HTTPヘッダ情報</summary>
		[XmlElement("httpHeader")]
		public string HttpHeader;
		/// <summary>デバイス情報</summary>
		[XmlElement("deviceInfo")]
		public string DeviceInfo;
	}

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
			this.ShopTransactionId = "";
			this.ShopOrderDate = "";
			this.FullName = "";
			this.FirstName = "";
			this.LastName = "";
			this.FullKanaName = "";
			this.FirstKanaName = "";
			this.LastKanaName = "";
			this.Tel = "";
			this.Mobile = "";
			this.Email = "";
			this.mobileEmail = "";
			this.ZipCode = "";
			this.Address1 = "";
			this.Address2 = "";
			this.Address3 = "";
			this.CompanyName = "";
			this.DepartmentName = "";
			this.BilledAmount = "";
		}

		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;
		/// <summary>加盟店注文日</summary>
		[XmlElement("shopOrderDate")]
		public string ShopOrderDate;
		/// <summary>フルネーム（漢字）</summary>
		[XmlElement("fullName")]
		public string FullName;
		/// <summary>姓（漢字）</summary>
		[XmlElement("firstName")]
		public string FirstName;
		/// <summary>名（漢字）</summary>
		[XmlElement("lastName")]
		public string LastName;
		/// <summary>フルネーム（カナ）</summary>
		[XmlElement("fullKanaName")]
		public string FullKanaName;
		/// <summary>姓（カナ）</summary>
		[XmlElement("firstKanaName")]
		public string FirstKanaName;
		/// <summary>名（カナ）</summary>
		[XmlElement("lastKanaName")]
		public string LastKanaName;
		/// <summary>電話番号</summary>
		[XmlElement("tel")]
		public string Tel;
		/// <summary>携帯電話番号</summary>
		[XmlElement("mobile")]
		public string Mobile;
		/// <summary>メールアドレス</summary>
		[XmlElement("email")]
		public string Email;
		/// <summary>携帯メールアドレス</summary>
		[XmlElement("mobileEmail")]
		public string mobileEmail;
		/// <summary>郵便番号</summary>
		[XmlElement("zipCode")]
		public string ZipCode;
		/// <summary>都道府県</summary>
		[XmlElement("address1")]
		public string Address1;
		/// <summary>市区町村</summary>
		[XmlElement("address2")]
		public string Address2;
		/// <summary>それ以降の住所</summary>
		[XmlElement("address3")]
		public string Address3;
		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName;
		/// <summary>部署名</summary>
		[XmlElement("departmentName")]
		public string DepartmentName;
		/// <summary>顧客請求額</summary>
		[XmlElement("billedAmount")]
		public string BilledAmount;
		/// <summary>決済種別</summary>
		[XmlElement("paymentType")]
		public DSKDeferredPaymentType PaymentType;
	}

	/// <summary>
	/// 配送先項目要素
	/// </summary>
	public class DeliveriesElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DeliveriesElement()
		{
			this.Delivery = new DeliveryElement[] { new DeliveryElement() };
		}

		/// <summary>配送先情報</summary>
		[XmlElement("delivery")]
		public DeliveryElement[] Delivery;
	}

	/// <summary>
	/// 配送先情報要素
	/// </summary>
	public class DeliveryElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DeliveryElement()
		{
			this.DeliveryCustomer = new DeliveryCustomerElement();
			this.Details = new DetailsElement();
		}

		/// <summary>配送先顧客情報</summary>
		[XmlElement("deliveryCustomer")]
		public DeliveryCustomerElement DeliveryCustomer;
		/// <summary>明細項目</summary>
		[XmlElement("details")]
		public DetailsElement Details;
	}

	/// <summary>
	/// 配送先顧客情報
	/// </summary>
	public class DeliveryCustomerElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DeliveryCustomerElement()
		{
			this.FullName = "";
			this.FirstName = "";
			this.LastName = "";
			this.FullKanaName = "";
			this.FirstKanaName = "";
			this.LastKanaName = "";
			this.ZipCode = "";
			this.Address1 = "";
			this.Address2 = "";
			this.Address3 = "";
			this.CompanyName = "";
			this.DepartmentName = "";
			this.Tel = "";
			this.Email = "";
		}

		/// <summary>フルネーム（漢字）</summary>
		[XmlElement("fullName")]
		public string FullName;
		/// <summary>姓（漢字）</summary>
		[XmlElement("firstName")]
		public string FirstName;
		/// <summary>名（漢字）</summary>
		[XmlElement("lastName")]
		public string LastName;
		/// <summary>フルネーム（カナ）</summary>
		[XmlElement("fullKanaName")]
		public string FullKanaName;
		/// <summary>姓（カナ）</summary>
		[XmlElement("firstKanaName")]
		public string FirstKanaName;
		/// <summary>名（カナ）</summary>
		[XmlElement("lastKanaName")]
		public string LastKanaName;
		/// <summary>郵便番号</summary>
		[XmlElement("zipCode")]
		public string ZipCode;
		/// <summary>都道府県</summary>
		[XmlElement("address1")]
		public string Address1;
		/// <summary>市区町村</summary>
		[XmlElement("address2")]
		public string Address2;
		/// <summary>それ以降の住所</summary>
		[XmlElement("address3")]
		public string Address3;
		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName;
		/// <summary>部署名</summary>
		[XmlElement("departmentName")]
		public string DepartmentName;
		/// <summary>電話番号</summary>
		[XmlElement("tel")]
		public string Tel;
		/// <summary>メールアドレス</summary>
		[XmlElement("email")]
		public string Email;
	}

	/// <summary>
	/// 明細項目要素
	/// </summary>
	public class DetailsElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DetailsElement()
		{
			this.Detail = new DetailElement[] { new DetailElement() };
		}

		/// <summary>明細詳細情報</summary>
		[XmlElement("detail")]
		public DetailElement[] Detail;
	}

	/// <summary>
	/// 明細詳細情報要素
	/// </summary>
	public class DetailElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DetailElement()
		{
			this.DetailName = "";
			this.DetailPrice = "";
			this.DetailQuantity = "";
		}

		/// <summary>明細コード</summary>
		[XmlElement("detailId")]
		public string DetailId;
		/// <summary>明細名</summary>
		[XmlElement("detailName")]
		public string DetailName;
		/// <summary>単価</summary>
		[XmlElement("detailPrice")]
		public string DetailPrice;
		/// <summary>数量</summary>
		[XmlElement("detailQuantity")]
		public string DetailQuantity;
	}
}
