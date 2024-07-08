using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

/// <summary>
/// SBPSCreditUserReferReceiver の概要の説明です
/// </summary>
public class SBPSCreditUserReferReceiver : SBPSBaseReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCreditUserReferReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// クレジット顧客参照受取
	/// </summary>
	/// <param name="req">リクエスト</param>
	/// <returns>レスポンス文字列</returns>
	public override string Receive()
	{
		// 見つかったばあいはそれを返す。
		var target = SBPSCreditUserRegistReceiver.Users.Find(user => user.CustCode == m_requestData.CustCode);
		if (target != null)
		{
			return GetResponseXml(true, target);
		}

		// 見つからなかった場合も成功で返すことにする
		return GetResponseXml(true, null);
	}

	/// <summary>
	/// 顧客参照レスポンスXML取得
	/// </summary>
	/// <param name="result">結果</param>
	/// <param name="req">リクエスト</param>
	/// <returns>レスポンス文字列</returns>
	private string GetResponseXml(bool result, SBPSApiRequestData req)
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

}