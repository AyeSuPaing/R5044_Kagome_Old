/*
=========================================================================================================
  Module      : 取引修正・キャンセルのリクエスト値(GmoRequestOrderModifyCancel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.OrderModifyCancel
{
	/// <summary>
	///  取引修正・キャンセルのリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestOrderModifyCancel : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestOrderModifyCancel()
			: base()
		{
			this.Buyer = new BuyerElement();
			this.Deliveries = new DeliveriesElement();
		}

		/// <summary>更新種別情報</summary>
		[XmlElement("kindInfo")]
		public KindInfoElement KindInfo;

		/// <summary>購入者情報</summary>
		[XmlElement("buyer")]
		public BuyerElement Buyer;

		/// <summary>配送先項目</summary>
		[XmlElement("deliveries")]
		public DeliveriesElement Deliveries;
	}

	#region KindInfoElement 更新種別情報要素
	/// <summary>
	/// 更新種別情報要素
	/// </summary>
	public class KindInfoElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public KindInfoElement()
		{
			this.UpdateKind = UpdateKindType.OrderModify;
		}

		/// <summary>取引更新種別</summary>
		[XmlElement("updateKind")]
		public UpdateKindType UpdateKind;

	}
	#endregion

	#region BuyerElement 購入者情報要素
	/// <summary>
	/// 購入者情報要素
	/// </summary>
	public class BuyerElement
	{
		/// <summary>コンストラクタ</summary>
		public BuyerElement()
		{
			this.GmoTransactionId = "";
			this.ShopTransactionId = "";
			this.ShopOrderDate = "";
			this.FullName = "";
			this.FullKanaName = "";
			this.ZipCode = "";
			this.Address = "";
			this.CompanyName = "";
			this.DepartmentName = "";
			this.Tel1 = "";
			this.Tel2 = "";
			this.Email1 = "";
			this.Email2 = "";
			this.BilledAmount = 0;
			this.GmoExtend1 = "";
			this.PaymentType = GmoPaymentType.IncludeService;
		}

		/// <summary>GMO 取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;

		/// <summary>加盟店注文日</summary>
		[XmlElement("shopOrderDate")]
		public string ShopOrderDate;

		/// <summary>氏名（漢字）</summary>
		[XmlElement("fullName")]
		public string FullName;

		/// <summary>氏名（カナ）</summary>
		[XmlElement("FullKanaName")]
		public string FullKanaName;

		/// <summary>郵便番号</summary>
		[XmlElement("zipCode")]
		public string ZipCode;

		/// <summary>住所</summary>
		[XmlElement("address")]
		public string Address;

		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName;

		/// <summary>部署名</summary>
		[XmlElement("departmentName")]
		public string DepartmentName;

		/// <summary>電話番号１</summary>
		[XmlElement("tel1")]
		public string Tel1;

		/// <summary>電話番号２</summary>
		[XmlElement("tel2")]
		public string Tel2;

		/// <summary>メールアドレス１</summary>
		[XmlElement("email1")]
		public string Email1;

		/// <summary>メールアドレス２</summary>
		[XmlElement("email2")]
		public string Email2;

		/// <summary>顧客請求額</summary>
		[XmlElement("billedAmount")]
		public decimal BilledAmount;

		/// <summary>GMO拡張項目１</summary>
		[XmlElement("gmoExtend1")]
		public string GmoExtend1;

		/// <summary>決済種別</summary>
		[XmlElement("paymentType")]
		public GmoPaymentType PaymentType;
	}
	#endregion

	#region DeliveriesElement 配送先項目要素
	/// <summary>
	/// 配送先項目要素
	/// </summary>
	public class DeliveriesElement
	{
		/// <summary>コンストラクタ</summary>
		public DeliveriesElement()
		{
			this.Delivery = new DeliveryElement();
		}

		/// <summary>配送先情報</summary>
		[XmlElement("delivery")]
		public DeliveryElement Delivery;
	}
	#endregion

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
		public DeliveryCustomerElement DeliveryCustomer;

		/// <summary>明細項目</summary>
		[XmlElement("details")]
		public DetailsElement Details;
	}
	#endregion

	#region DeliveryCustomerElement 配送先顧客情報
	/// <summary>
	/// 配送先顧客情報
	/// </summary>
	public class DeliveryCustomerElement
	{
		/// <summary>コンストラクタ</summary>
		public DeliveryCustomerElement()
		{
			this.FullName = "";
			this.FullKanaName = "";
			this.ZipCode = "";
			this.Address = "";
			this.CompanyName = "";
			this.DepartmentName = "";
			this.Tel = "";
		}

		/// <summary>氏名（漢字）</summary>
		[XmlElement("fullName")]
		public string FullName;

		/// <summary>氏名（カナ）</summary>
		[XmlElement("fullKanaName")]
		public string FullKanaName;

		/// <summary>郵便番号</summary>
		[XmlElement("zipCode")]
		public string ZipCode;

		/// <summary>住所</summary>
		[XmlElement("address")]
		public string Address;

		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName;

		/// <summary>部署名</summary>
		[XmlElement("departmentName")]
		public string DepartmentName;

		/// <summary>電話番号</summary>
		[XmlElement("tel")]
		public string Tel;
	}
	#endregion

	#region DetailsElement 明細項目要素
	/// <summary>
	/// 明細項目要素
	/// </summary>
	public class DetailsElement
	{
		/// <summary>コンストラクタ</summary>
		public DetailsElement()
		{
			this.Detail = new DetailElement[] { new DetailElement() };
		}

		/// <summary>明細詳細情報</summary>
		[XmlElement("detail")]
		public DetailElement[] Detail;
	}
	#endregion

	#region DetailElement 明細詳細情報要素
	/// <summary>
	/// 明細詳細情報要素
	/// </summary>
	public class DetailElement
	{
		/// <summary>コンストラクタ</summary>
		public DetailElement()
		{
			this.DetailName = "";
			this.DetailPrice = 0;
			this.DetailQuantity = 0;
			this.GmoExtend2 = "";
			this.GmoExtend3 = "";
			this.GmoExtend4 = "";
		}

		/// <summary>明細名</summary>
		[XmlElement("detailName")]
		public string DetailName;

		/// <summary>単価</summary>
		[XmlElement("detailPrice")]
		public decimal DetailPrice;

		/// <summary>数量</summary>
		[XmlElement("detailQuantity")]
		public decimal DetailQuantity;

		/// <summary>GMO拡張項目2</summary>
		[XmlElement("gmoExtend2")]
		public string GmoExtend2;

		/// <summary>GMO拡張項目3</summary>
		[XmlElement("gmoExtend3")]
		public string GmoExtend3;

		/// <summary>GMO拡張項目4</summary>
		[XmlElement("gmoExtend4")]
		public string GmoExtend4;

	}
	#endregion
}
