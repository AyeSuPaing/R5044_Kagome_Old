/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 請求書印字情報取得レスポンスデータクラス(PaymentYamatoKaGetInfoResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 請求書印字情報取得レスポンスデータクラス
	/// </summary>
	public class PaymentYamatoKaGetInfoResponseData : PaymentYamatoKaBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal PaymentYamatoKaGetInfoResponseData(string responseString)
			: base(responseString)
		{
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public override void LoadXml(XDocument responseXml)
		{
			this.ProductItem = new PaymentYamatoKaProductItem[10];

			// 基底クラスのメソッド呼び出し
			base.LoadXml(responseXml);

			// 固有の値をセット (後払い) 請求書印字情報取得
			foreach (XElement element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "postCode":
						this.PostCode = element.Value;
						break;

					case "address1":
						this.Address1 = element.Value;
						break;

					case "address2":
						this.Address2 = element.Value;
						break;

					case "name":
						this.Name = element.Value;
						break;

					case "claimDate":
						this.ClaimDate = DateTime.ParseExact(
							element.Value,
							"yyyyMMdd",
							System.Globalization.DateTimeFormatInfo.InvariantInfo,
							System.Globalization.DateTimeStyles.None);
						break;

					case "buyStore":
						this.BuyStore = element.Value;
						break;

					case "hpAddress":
						this.HpAddress = element.Value;
						break;

					case "telNum":
						this.TelNum = element.Value;
						break;

					case "email":
						this.Email = element.Value;
						break;

					case "orderDate":
						this.OrderDate = DateTime.ParseExact(
							element.Value,
							"yyyyMMdd",
							System.Globalization.DateTimeFormatInfo.InvariantInfo,
							System.Globalization.DateTimeStyles.None);
						break;

					case "claimSum":
						this.ClaimSum = decimal.Parse(element.Value);
						break;

					case "maturity":
						this.Maturity = DateTime.ParseExact(
							element.Value,
							"yyyyMMdd",
							System.Globalization.DateTimeFormatInfo.InvariantInfo,
							System.Globalization.DateTimeStyles.None);
						break;

					case "accountNo1":
						this.AccountNo1 = element.Value;
						break;

					case "accountNo2":
						this.AccountNo2 = element.Value;
						break;

					case "memberName":
						this.MemberName = element.Value;
						break;

					case "recipient":
						this.Recipient = element.Value;
						break;

					case "buyerName":
						this.BuyerName = element.Value;
						break;

					case "buyName":
						this.BuyName = element.Value;
						break;

					case "inquiryNo":
						this.InquiryNo = element.Value;
						break;

					case "upperOcrCode":
						this.UpperOcrCode = element.Value;
						break;

					case "lowerOcrCode":
						this.LowerOcrCode = element.Value;
						break;

					case "barCode":
						this.BarCode = element.Value;
						break;

					case "billPostCode":
						this.BillPostCode = element.Value;
						break;
				}

				if (element.Name.ToString().StartsWith("itemName")
					|| element.Name.ToString().StartsWith("itemCount")
					|| element.Name.ToString().StartsWith("unitPrice")
					|| element.Name.ToString().StartsWith("subTotal"))
				{
					var index = int.Parse(element.Name.ToString().Replace("itemName", string.Empty).Replace("itemCount", string.Empty).Replace("unitPrice", string.Empty).Replace("subTotal", string.Empty)) - 1;

					if (this.ProductItem[index] == null) this.ProductItem[index] = new PaymentYamatoKaProductItem();

					if (element.Name.ToString().StartsWith("itemName"))
					{
						this.ProductItem[index].ItemName = element.Value;
					}
					else if (element.Name.ToString().StartsWith("itemCount"))
					{
						int itemCount;
						if (int.TryParse(element.Value, out itemCount)) this.ProductItem[index].ItemCount = itemCount;
					}
					else if (element.Name.ToString().StartsWith("unitPrice"))
					{
						int unitPrice;
						if (int.TryParse(element.Value, out unitPrice)) this.ProductItem[index].UnitPrice = unitPrice;
					}
					else if (element.Name.ToString().StartsWith("subTotal"))
					{
						decimal subTotal;
						if (decimal.TryParse(element.Value, out subTotal)) this.ProductItem[index].SubTotal = subTotal;
					}
				}
			}
		}

		/// <summary>郵便番号</summary>
		public string PostCode { get; private set; }
		/// <summary>住所１</summary>
		public string Address1 { get; private set; }
		/// <summary>住所２</summary>
		public string Address2 { get; private set; }
		/// <summary>氏名</summary>
		public string Name { get; private set; }
		/// <summary>請求書発行日</summary>
		public DateTime ClaimDate { get; private set; }
		/// <summary>購入店名</summary>
		public string BuyStore { get; private set; }
		/// <summary>お問合せHPアドレス</summary>
		public string HpAddress { get; private set; }
		/// <summary>お問合せ電話番号</summary>
		public string TelNum { get; private set; }
		/// <summary>お問合せメールアドレス</summary>
		public string Email { get; private set; }
		/// <summary>受注日</summary>
		public DateTime OrderDate { get; private set; }
		/// <summary>請求金額</summary>
		public decimal ClaimSum { get; private set; }
		/// <summary>支払い期限日</summary>
		public DateTime Maturity { get; private set; }
		/// <summary>購入商品</summary>
		public PaymentYamatoKaProductItem[] ProductItem { get; private set; }
		/// <summary>口座番号１</summary>
		public string AccountNo1 { get; private set; }
		/// <summary>口座番号２</summary>
		public string AccountNo2 { get; private set; }
		/// <summary>加入者名</summary>
		public string MemberName { get; private set; }
		/// <summary>受取人</summary>
		public string Recipient { get; private set; }
		/// <summary>払込人</summary>
		public string BuyerName { get; private set; }
		/// <summary>購入店名</summary>
		public string BuyName { get; private set; }
		/// <summary>お問合せ番号</summary>
		public string InquiryNo { get; private set; }
		/// <summary>OCRコード(上段)</summary>
		public string UpperOcrCode { get; private set; }
		/// <summary>OCRコード(下段)</summary>
		public string LowerOcrCode { get; private set; }
		/// <summary>バーコード情報</summary>
		public string BarCode { get; private set; }
		/// <summary>フリー項目</summary>
		public string BillPostCode { get; private set; }
	}
}
