/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) リクエストデータ(Shipment)生成クラス(PaymentTriLinkAfterPayRequestShipmentData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) リクエストデータ(Shipment)生成クラス
	/// </summary>
	[JsonObject]
	public class PaymentTriLinkAfterPayRequestShipmentData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shipping">配送情報</param>
		public PaymentTriLinkAfterPayRequestShipmentData(CartShipping shipping) 
			: this(
				shipping.CompanyName,
				shipping.CompanyPostName,
				shipping.Name,
				shipping.Zip,
				shipping.Addr2 + shipping.Addr3 + shipping.Addr4,
				shipping.Tel1)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shipping">配送情報</param>
		public PaymentTriLinkAfterPayRequestShipmentData(OrderShippingModel shipping) 
			: this(
				shipping.ShippingCompanyName,
				shipping.ShippingCompanyPostName,
				shipping.ShippingName,
				shipping.ShippingZip,
				shipping.ShippingAddr2 + shipping.ShippingAddr3 + shipping.ShippingAddr4,
				shipping.ShippingTel1)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="companyName">配送先会社名</param>
		/// <param name="departmentName">配送先部署名</param>
		/// <param name="name">配送先指名</param>
		/// <param name="zipCode">配送先郵便番号</param>
		/// <param name="address">配送先住所</param>
		/// <param name="tel">配送先電話番号</param>
		public PaymentTriLinkAfterPayRequestShipmentData(
			string companyName, 
			string departmentName,
			string name,
			string zipCode,
			string address,
			string tel)
		{
			this.ShipmentType = TriLinkAfterPayConstants.FLG_TW_AFTERPAY_SHIPMENT_TYPE;
			this.CompanyName = companyName;
			this.DepartmentName = departmentName;
			this.CvsCode = "";
			this.CvsStoreCode = "";
			this.CvsStoreName = "";
			this.Name = name;
			this.ZipCode = zipCode;
			this.Address = address;
			this.Tel = tel;
		}
		#endregion

		#region プロパティ
		/// <summary>配送方法</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_SHIPMENT_TYPE)]
		public string ShipmentType { get; set; }
		/// <summary>配送先会社名</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_COMPANY_NAME)]
		public string CompanyName { get; set; }
		/// <summary>配送先部署名</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_DEPARTMENT_NAME)]
		public string DepartmentName { get; set; }
		/// <summary>コンビニコード</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_CVS_CODE)]
		public string CvsCode { get; set; }
		/// <summary>コンビニ店舗コード</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_CVS_STORE_CODE)]
		public string CvsStoreCode { get; set; }
		/// <summary>コンビニ店舗名</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_CVS_STORE_NAME)]
		public string CvsStoreName { get; set; }
		/// <summary>配送先氏名</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_NAME)]
		public string Name { get; set; }
		/// <summary>配送先郵便番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ZIP_CODE)]
		public string ZipCode { get; set; }
		/// <summary>配送先住所</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ADDRESS)]
		public string Address { get; set; }
		/// <summary>配送先電話番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_TEL)]
		public string Tel { get; set; }
		#endregion
	}
}
