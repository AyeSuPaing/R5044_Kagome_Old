using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Text;
using w2.App.Common.Order;
using w2.Common.Util.Security;

/// <summary>
/// PaymentSBPSCvsPaymentRevc_ReqApi の概要の説明です
/// </summary>
public class PaymentSBPSCvsPaymentRevc_ReqApi : w2.App.Common.Order.PaymentSBPSBaseApi
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="settings">SBPS設定</param>
	public PaymentSBPSCvsPaymentRevc_ReqApi(PaymentSBPSSetting settings)
		: base(settings, null)
	{
		m_tripleDES = new PaymentSBPSTripleDESCrypto(
			Constants.PAYMENT_SETTING_SBPS_3DES_KEY,
			Constants.PAYMENT_SETTING_SBPS_3DES_IV);
		this.HashCalculator = new PaymentSBPSHashCalculator(Constants.PAYMENT_SETTING_SBPS_HASHKEY, Encoding.GetEncoding("Shift_JIS"));
	}

	protected new PaymentSBPSTripleDESCrypto m_tripleDES = null;

	/// <summary>SBPS設定</summary>
	protected new PaymentSBPSHashCalculator HashCalculator { get; private set; }


	/// <summary>
	/// 実行
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="trackingId)">トラッキングID</param>
	/// <returns>実行結果</returns>
	public XDocument Exec(string orderId, string trackingId)
	{
		// XML作成
		XDocument requestXml = CreateAuthXml(orderId, trackingId);

		// 実行
		return PostHttpRequest(requestXml);
	}

	/// <summary>
	/// XML作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="trackingId)">トラッキングID</param>
	/// <returns>XML</returns>
	private XDocument CreateAuthXml(string orderId, string  trackingId)
	{
		XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
		document.Add(
			new XElement("sps-api-request",
				new XAttribute("id", "NT01-00103-701"),
				new XElement("merchant_id", this.HashCalculator.Add(Constants.PAYMENT_SETTING_SBPS_MERCHANT_ID)),
				new XElement("service_id", this.HashCalculator.Add(Constants.PAYMENT_SETTING_SBPS_SERVICE_ID)),
				new XElement("order_id", this.HashCalculator.Add(orderId)),
				new XElement("sps_transaction_id", this.HashCalculator.Add("")),
				new XElement("tracking_id", this.HashCalculator.Add(trackingId)),
				new XElement("rec_datetime", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
				new XElement("pay_method_info",
					new XElement("rec_type", GetEncryptedData(this.HashCalculator.Add("1"))),			// 速報
					new XElement("rec_amount", GetEncryptedData(this.HashCalculator.Add("500"))),		// 入金
					new XElement("rec_amount_total", GetEncryptedData(this.HashCalculator.Add("500"))),	// 累計入金
					new XElement("res_mail", GetEncryptedData(this.HashCalculator.Add("ochiai@w2solution.co.jp"))),	// メールアドレス
					new XElement("rec_extra", GetEncryptedData(this.HashCalculator.Add("")))			// 備考
				),
				new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
				new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())
			)
		);
		return document;
	}
}