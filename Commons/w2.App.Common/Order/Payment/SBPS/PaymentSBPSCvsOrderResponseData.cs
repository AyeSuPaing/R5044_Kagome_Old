/*
=========================================================================================================
  Module      : ソフトバンクペイメント WEBコンビニ「決済要求処理」API レスポンスデータ(PaymentSBPSCvsOrderResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using w2.App.Common.Util;
using w2.App.Common.Extensions.Currency;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント WEBコンビニ「決済要求処理」API レスポンスデータ
	/// </summary>
	public class PaymentSBPSCvsOrderResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		/// <param name="messageXml">メッセージXML</param>
		internal PaymentSBPSCvsOrderResponseData(PaymentSBPSSetting settings, XDocument messageXml)
			: base(settings)
		{
			m_messageXml = messageXml;
		}

		/// <summary>
		/// レスポンスをプロパティへ格納(クレジット用にオーバーライド）
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public override void LoadXml(XDocument responseXml)
		{
			// 基底クラスのメソッド呼び出し
			base.LoadXml(responseXml);

			// コンビニ固有の値をセット
			foreach (XElement element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "res_pay_method_info":
						foreach (XElement paymentElement in element.Elements())
						{
							switch (paymentElement.Name.ToString())
							{
								case "invoice_no":
									this.InvoiceNo = GetDecryptedData(paymentElement.Value);
									break;

								case "bill_date":
									DateTime tmpBillDate;
									if (DateTime.TryParseExact(GetDecryptedData(paymentElement.Value), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out tmpBillDate))
									{
										this.BillDate = tmpBillDate;
									}
									break;

								case "cvs_pay_data1":
									this.CvsPayData1 = GetDecryptedData(paymentElement.Value);
									break;

								case "cvs_pay_data2":
									this.CvsPayData2 = GetDecryptedData(paymentElement.Value);
									break;
							}
						}
						break;
				}
			}

			// コンビニ支払いメッセージ作成
			CreateCvsPaymentMessages();
		}

		/// <summary>
		/// コンビニ支払いメッセージ作成
		/// </summary>
		private void CreateCvsPaymentMessages()
		{
			var messages = new List<StringBuilder>
			{
				new StringBuilder(m_messageXml.Root.Element(this.WebCvsType.ToString()).Element(@"Html").Value),
				new StringBuilder(m_messageXml.Root.Element(this.WebCvsType.ToString()).Element(@"Text").Value)
			};

			string[] cvsPayDatas1 = (this.CvsPayData1 + "-").Split('-');

			messages.ForEach(message =>
				{
					message.Replace("@@ CvsPayData1 @@", this.CvsPayData1);
					message.Replace("@@ CvsPayData1-1 @@", cvsPayDatas1[0]);
					message.Replace("@@ CvsPayData1-2 @@", cvsPayDatas1[1]);
					message.Replace("@@ CvsPayData2 @@", this.CvsPayData2);
					message.Replace("@@ Amount @@", StringUtility.ToNumeric(this.Amount.ToPriceString()));
				});

			this.MessageHtml = messages[0].ToString();
			this.MessageText = messages[1].ToString();
		}

		/// <summary>
		/// コンビニ支払いメッセージ取得
		/// </summary>
		/// <param name="isHtml">HTMLメッセージか</param>
		/// <param name="languageLocaleId">注文者の言語ロケールID</param>
		/// <returns>置換済のコンビニ支払いメッセージ</returns>
		public string GetCvsPaymentMessage(bool isHtml, string languageLocaleId)
		{
			var message = (isHtml ? this.MessageHtml : this.MessageText).Replace(
				"@@ BillDate @@",
				DateTimeUtility.ToString(
					this.BillDate,
					DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter,
					languageLocaleId));
			return message;
		}

		/// <summary>請求書番号</summary>
		public string InvoiceNo { get; private set; }
		/// <summary>払込期限</summary>
		public DateTime BillDate { get; private set; }
		/// <summary>コンビニ用 払出情報１</summary>
		public string CvsPayData1 { get; private set; }
		/// <summary>コンビニ用 払出情報2</summary>
		public string CvsPayData2 { get; private set; }

		/// <summary>メッセージ</summary>
		private XDocument m_messageXml { get; set; }
		/// <summary>Webコンビニタイプ（メッセージ生成用。決済時にセット）</summary>
		public PaymentSBPSTypes.WebCvsTypes WebCvsType { get; set; }
		/// <summary>合計金額（メッセージ生成用。決済時にセット）</summary>
		public decimal Amount { get; set; }
		/// <summary>メッセージHTML</summary>
		private string MessageHtml { get; set; }
		/// <summary>メッセージTEXT</summary>
		private string MessageText { get; set; }
	}
}
