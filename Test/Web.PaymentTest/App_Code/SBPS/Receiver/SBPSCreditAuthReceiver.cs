using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

/// <summary>
/// SBPSCreditAuthReceiver の概要の説明です
/// </summary>
public class SBPSCreditAuthReceiver : SBPSBaseReceiver
{
	public readonly static Dictionary<string, decimal> TrackingIds = new Dictionary<string, decimal>();　// 今のところ排他は考慮無し

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCreditAuthReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// クレジット与信受取
	/// </summary>
	/// <returns>レスポンス文字列</returns>
	public override string Receive()
	{
		switch (m_requestData.CardNo)
		{
			case "0000000000000000":
			case "4123450131003312x":
				return GetResponseXml(false, "");

			default:
				string trackingId = DAMMY_STRING + DateTime.Now.ToString("MMddHHmmss");

				if (SBPSCreditAuthReceiver.TrackingIds.ContainsKey(trackingId) == false) SBPSCreditAuthReceiver.TrackingIds.Add(trackingId, m_requestData.AmountAuth);

				return GetResponseXml(true, trackingId);
		}
	}

	/// <summary>
	/// クレジット与信向けレスポンス取得
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
					new XElement("cc_company_code", "cZlhiRE1310="),
					new XElement("recognized_no", "y7UBc+7xI8E=")
				),
				new XElement("res_process_date", DateTime.Now.ToString("yyyyMMddHHmmss")),
				new XElement("res_err_code", result ? "" : "10165200"),	// "65": // G12：カード使用不可
				new XElement("res_date", DateTime.Now.ToString("yyyyMMddHHmmss"))
				)
			);

		return response.ToString();
	}
}