/*
=========================================================================================================
  Module      : ソフトバンクペイメント WEBコンビニ「決済要求処理」APIクラス(PaymentSBPSCvsOrderApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント WEBコンビニ「決済要求処理」APIクラス
	/// </summary>
	public class PaymentSBPSCvsOrderApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="messageXml">メッセージXML</param>
		public PaymentSBPSCvsOrderApi(XDocument messageXml)
			: this(PaymentSBPSSetting.GetDefaultSetting(), messageXml)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		/// <param name="messageXml">メッセージXML</param>
		public PaymentSBPSCvsOrderApi(
			PaymentSBPSSetting settings,
			XDocument messageXml)
			: base(settings, new PaymentSBPSCvsOrderResponseData(settings, messageXml))
		{
		}

		/// <summary>
		/// 決済要求
		/// </summary>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="itemId">商品検索用ID</param>
		/// <param name="itemName">商品表示名</param>
		/// <param name="productItems">商品情報</param>
		/// <param name="amount">注文金額合計</param>
		/// <param name="lastName">顧客姓</param>
		/// <param name="firstName">顧客名</param>
		/// <param name="lastNameKana">顧客姓カナ</param>
		/// <param name="firstNameKana">顧客名カナ</param>
		/// <param name="firstZip">郵便番号1</param>
		/// <param name="secondZip">郵便番号2</param>
		/// <param name="add1">都道府県</param>
		/// <param name="add2">市区町村、番地</param>
		/// <param name="add3">マンション・ビル</param>
		/// <param name="tel">電話番号</param>
		/// <param name="mail">e-mail</param>
		/// <param name="seiyakuDate">制約日(年月日)</param>
		/// <param name="webCvsType">Web コンビニタイプ</param>
		/// <param name="billDate">支払期限(年月日)</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string custCode,
			string orderId,
			string itemId,
			string itemName,
			List<ProductItem> productItems,
			decimal amount,
			string lastName,
			string firstName,
			string lastNameKana,
			string firstNameKana,
			string firstZip,
			string secondZip,
			string add1,
			string add2,
			string add3,
			string tel,
			string mail,
			DateTime seiyakuDate,
			PaymentSBPSTypes.WebCvsTypes webCvsType,
			DateTime? billDate)
		{
			this.ResponseData.Amount = amount;
			this.ResponseData.WebCvsType = webCvsType;

			// XML作成
			XDocument requestXml = CreateOrderXml(
			custCode,
			orderId,
			itemId,
			itemName,
			productItems,
			amount,
			lastName,
			firstName,
			lastNameKana,
			firstNameKana,
			firstZip,
			secondZip,
			add1,
			add2,
			add3,
			tel,
			mail,
			seiyakuDate,
			webCvsType,
			billDate);

			// 実行
			return Post(requestXml);
		}

		/// <summary>
		/// 決済要求XML作成
		/// </summary>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="itemId">商品検索用ID</param>
		/// <param name="itemName">商品表示名</param>
		/// <param name="productItems">商品情報</param>
		/// <param name="amount">注文金額合計</param>
		/// <param name="lastName">顧客姓</param>
		/// <param name="firstName">顧客名</param>
		/// <param name="lastNameKana">顧客姓カナ</param>
		/// <param name="firstNameKana">顧客名カナ</param>
		/// <param name="firstZip">郵便番号1</param>
		/// <param name="secondZip">郵便番号2</param>
		/// <param name="add1">都道府県</param>
		/// <param name="add2">市区町村、番地</param>
		/// <param name="add3">マンション・ビル</param>
		/// <param name="tel">電話番号</param>
		/// <param name="mail">-mail</param>
		/// <param name="seiyakuDate">制約日(年月日)</param>
		/// <param name="webCvsType">Web コンビニタイプ</param>
		/// <param name="billDate">支払期限(年月日)</param>
		/// <returns>決済要求XML</returns>
		private XDocument CreateOrderXml(
			string custCode,
			string orderId,
			string itemId,
			string itemName,
			List<ProductItem> productItems,
			decimal amount,
			string lastName,
			string firstName,
			string lastNameKana,
			string firstNameKana,
			string firstZip,
			string secondZip,
			string add1,
			string add2,
			string add3,
			string tel,
			string mail,
			DateTime seiyakuDate,
			PaymentSBPSTypes.WebCvsTypes webCvsType,
			DateTime? billDate)
		{
			// XML作成
			XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "ST01-00101-701"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					new XElement("cust_code", this.HashCalculator.Add(custCode)),
					new XElement("order_id", this.HashCalculator.Add(orderId)),
					new XElement("item_id", this.HashCalculator.Add(itemId)),	// ここで設定することによってソフトバンクペイメントで検索が可能
					new XElement("item_name", EncodeMultibyteToBase64(this.HashCalculator.Add(itemName))),
				//new XElement("tax", this.HashCalculator.Add("")),
					new XElement("amount", this.HashCalculator.Add(amount.ToPriceString())),
				//new XElement("free1", this.HashCalculator.Add("")),
				//new XElement("free2", this.HashCalculator.Add("")),
				//new XElement("free3", this.HashCalculator.Add("")),
				//new XElement("order_rowno", this.HashCalculator.Add("")),	// 請求番号枝番。再入力時にインクリメントされて送られてくる。2 重購入要求を検知するために設定。
					CreateProductItemNode(productItems),
					new XElement("pay_method_info",
						new XElement("issue_type", GetEncryptedData(this.HashCalculator.Add("0"))),	// WEBコンビニでは0固定
						new XElement("last_name", GetEncryptedData(this.HashCalculator.Add(lastName))),
						new XElement("first_name", GetEncryptedData(this.HashCalculator.Add(firstName))),
						new XElement("last_name_kana", GetEncryptedData(this.HashCalculator.Add(StringUtility.ToZenkakuKatakana(lastNameKana)))),
						new XElement("first_name_kana", GetEncryptedData(this.HashCalculator.Add(StringUtility.ToZenkakuKatakana(firstNameKana)))),
						new XElement("first_zip", GetEncryptedData(this.HashCalculator.Add(StringUtility.ToHankaku(firstZip)))),
						new XElement("second_zip", GetEncryptedData(this.HashCalculator.Add(StringUtility.ToHankaku(secondZip)))),
						new XElement("add1", GetEncryptedData(this.HashCalculator.Add(StringUtility.ToZenkaku(add1)))),
						new XElement("add2", GetEncryptedData(this.HashCalculator.Add(StringUtility.ToZenkaku(add2)))),
						new XElement("add3", GetEncryptedData(this.HashCalculator.Add(StringUtility.ToZenkaku(add3)))),
						new XElement("tel", GetEncryptedData(this.HashCalculator.Add(StringUtility.ToHankaku(tel.Replace("-", ""))))),
						new XElement("mail", GetEncryptedData(this.HashCalculator.Add(StringUtility.ToHankaku(mail)))),
						new XElement("seiyakudate", GetEncryptedData(this.HashCalculator.Add(seiyakuDate.ToString("yyyyMMdd")))),
						new XElement("webcvstype", GetEncryptedData(this.HashCalculator.Add(PaymentSBPSUtil.ConvertWebCvsTypeToCode(webCvsType)))),
						new XElement("bill_date", GetEncryptedData(this.HashCalculator.Add(billDate.HasValue ? billDate.Value.ToString("yyyyMMdd") : "")))),
					new XElement("encrypted_flg", this.HashCalculator.Add("1")),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
				//new XElement("limit_second", this.HashCalculator.Add("")),	// リクエスト時の許容時間（省略の場合は事前設定値を適用）
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
			));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCvsOrderResponseData ResponseData { get { return (PaymentSBPSCvsOrderResponseData)this.ResponseDataInner; } }
	}
}
