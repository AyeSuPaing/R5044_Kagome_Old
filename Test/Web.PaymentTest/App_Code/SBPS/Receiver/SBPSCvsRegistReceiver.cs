using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

/// <summary>
/// SBPSCvsRegistReceiver の概要の説明です
/// </summary>
public class SBPSCvsRegistReceiver : SBPSBaseReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCvsRegistReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// コンビニ決済受取
	/// </summary>
	/// <returns>レスポンス文字列</returns>
	public override string Receive()
	{
		return GetResponseXml(true, DAMMY_STRING + DateTime.Now.ToString("MMddHHmmss"));
	}

	/// <summary>
	/// コンビニ登録向けレスポンス取得
	/// </summary>
	/// <param name="result">結果</param>
	/// <param name="trackingId">トラッキングID</param>
	/// <returns>レスポンス文字列</returns>
	private string GetResponseXml(bool result, string trackingId)
	{
		XDocument response = new XDocument();
		response.Add(
			new XElement("sps-api-response",
				new XElement("res_result", result ? "OK" : "NG"),
				new XElement("res_sps_transaction_id", DAMMY_STRING + "B66275001ST0100101101000152"),
				new XElement("res_tracking_id", trackingId),
				new XElement("res_pay_method_info",
					new XElement("invoice_no", m_tripleDes.GetEncryptedData("123456789")),
					new XElement("bill_date", m_tripleDes.GetEncryptedData("20120101")),
					new XElement("cvs_pay_data1", m_tripleDes.GetEncryptedData("info1")),
					new XElement("cvs_pay_data2", m_tripleDes.GetEncryptedData("info2"))
				),
				new XElement("res_process_date", DateTime.Now.ToString("yyyyMMddHHmmss")),
				new XElement("res_err_code", result ? "" : "10104200"),
				new XElement("res_date", DateTime.Now.ToString("yyyyMMddHHmmss"))
				)
			);

		return response.ToString();
	}
}