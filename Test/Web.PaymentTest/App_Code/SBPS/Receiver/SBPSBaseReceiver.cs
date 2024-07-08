using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

/// <summary>
/// SBPSBaseReceiver の概要の説明です
/// </summary>
public abstract class SBPSBaseReceiver : ISBPSReceiver
{
	protected readonly static string DAMMY_STRING = "DAMMY";

	protected readonly static PaymentSBPSTripleDESCrypto m_tripleDes = new PaymentSBPSTripleDESCrypto(
		Constants.PAYMENT_SETTING_SBPS_3DES_KEY,
		Constants.PAYMENT_SETTING_SBPS_3DES_IV);

	protected SBPSApiRequestData m_requestData = null;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="request">リクエストデータ</param>
	public SBPSBaseReceiver(SBPSApiRequestData requestData)
	{
		m_requestData = requestData;
	}

	/// <summary>
	/// 受信
	/// </summary>
	/// <returns>レスポンスXML</returns>
	public abstract string Receive();

	/// <summary>
	/// ベースのレスポンスXML取得
	/// </summary>
	/// <param name="result">結果</param>
	/// <returns>レスポンス文字列</returns>
	protected string GetResponseXmlBase(bool result)
	{
		XDocument response = new XDocument();
		response.Add(
			new XElement("sps-api-response",
				new XElement("res_result", result ? "OK" : "NG"),
				new XElement("res_sps_transaction_id", DAMMY_STRING + "B66275001ST0100101101000152"),
				new XElement("res_process_date", DateTime.Now.ToString("yyyyMMddHHmmss")),
				new XElement("res_err_code", result ? "" : "10104200"),
				new XElement("res_date", DateTime.Now.ToString("yyyyMMddHHmmss"))
				)
			);

		return response.ToString();
	}

	/// <summary>
	/// 継続課金用のレスポンスXML取得
	/// </summary>
	/// <param name="result">結果</param>
	/// <param name="errorCode">エラーコード</param>
	/// <returns>レスポンス文字列</returns>
	protected string GetResponseXmlForContinuousOrder(bool result, string errorCode = "")
	{
		var response = new XDocument();
		response.Add(
			new XElement("sps-api-response",
				new XElement("res_result", result ? "OK" : "NG"),
				new XElement("res_sps_transaction_id", DAMMY_STRING + "B66275001ST0100101101000152"),
				new XElement("res_tracking_id", m_requestData.TrackingId),
				new XElement("res_process_date", DateTime.Now.ToString("yyyyMMddHHmmss")),
				new XElement("res_err_code", result ? "" : errorCode),
				new XElement("res_date", DateTime.Now.ToString("yyyyMMddHHmmss"))
			)
		);

		return response.ToString();
	}

	/// <summary> 成功のテストパターンか </summary>
	public bool IsSuccessTestPattern
	{
		get
		{
			return ((string.IsNullOrEmpty(m_requestData.TrackingId) == false)
				&& (m_requestData.TrackingId.StartsWith("ngtest") == false));
		}
	}
}