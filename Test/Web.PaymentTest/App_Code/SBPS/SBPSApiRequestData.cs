using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Text;
using w2.App.Common.Order;

/// <summary>
/// SBPSApiRequestData の概要の説明です
/// </summary>
public class SBPSApiRequestData
{
	PaymentSBPSTripleDESCrypto m_tripleDES = null;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public SBPSApiRequestData(string xml)
	{
		this.ReceiveXml = XDocument.Parse(xml);

		m_tripleDES = new PaymentSBPSTripleDESCrypto(
			Constants.PAYMENT_SETTING_SBPS_3DES_KEY,
			Constants.PAYMENT_SETTING_SBPS_3DES_IV);
	}

	/// <summary>
	/// カード番号取得
	/// </summary>
	/// <returns>カード番号</returns>
	public string GetCardNo()
	{
		XElement cardNoElement = this.ReceiveXml.Element("sps-api-request").Element("pay_method_info").Element("cc_number");

		return m_tripleDES.GetDecryptedData(cardNoElement.Value);
	}

	/// <summary>受取XML</summary>
	private XDocument ReceiveXml { get; set; }
	/// <summary>機能ID</summary>
	public string FunctionId
	{
		get
		{
			return this.ReceiveXml.Element("sps-api-request").Attribute("id").Value;
		}
	}
	/// <summary>顧客ID</summary>
	public string CustCode
	{
		get
		{
			return this.ReceiveXml.Element("sps-api-request").Element("cust_code").Value;
		}
	}

	/// <summary>カードNO</summary>
	public string CardNo
	{
		get
		{
			return (this.CardNoRaw != null) ? m_tripleDES.GetDecryptedData(this.CardNoRaw) : null;
		}
	}
	/// <summary>カードNO（暗号化）</summary>
	public string CardNoRaw
	{
		get
		{
			var ccNumElem = this.ReceiveXml.Element("sps-api-request").Element("pay_method_info").Element("cc_number");
			return (ccNumElem != null) ? ccNumElem.Value : null;
		}
	}

	/// <summary>カード有効期限</summary>
	public string CardExpiration
	{
		get
		{
			return m_tripleDES.GetDecryptedData(this.CardExpirationRaw);
		}
	}
	/// <summary>カード有効期限（暗号化）</summary>
	public string CardExpirationRaw
	{
		get
		{
			return this.ReceiveXml.Element("sps-api-request").Element("pay_method_info").Element("cc_expiration").Value;
		}
	}

	/// <summary>トラッキングIDカードNO</summary>
	public string TrackingId
	{
		get
		{
			return  this.ReceiveXml.Element("sps-api-request").Element("tracking_id").Value;
		}
	}
	/// <summary>与信金額（与信のみ）</summary>
	public decimal AmountAuth
	{
		get
		{
			return decimal.Parse(this.ReceiveXml.Element("sps-api-request").Element("amount").Value);
		}
	}
	/// <summary>売上金額（売上のみ）</summary>
	public decimal? AmountSales
	{
		get
		{
			if (this.ReceiveXml.Element("sps-api-request").Element("pay_option_manage").Element("amount") == null) return null;

			return decimal.Parse(this.ReceiveXml.Element("sps-api-request").Element("pay_option_manage").Element("amount").Value);
		}
	}
}