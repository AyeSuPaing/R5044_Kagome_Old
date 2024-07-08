using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSCreditUserRegistReceiver の概要の説明です
/// </summary>
public class SBPSCreditUserRegistReceiver : SBPSBaseReceiver
{
	public readonly static List<SBPSApiRequestData> Users = new List<SBPSApiRequestData>();

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCreditUserRegistReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// クレジット顧客登録受取
	/// </summary>
	/// <returns>レスポンス文字列</returns>
	public override string Receive()
	{
		return GetResponseXmlBase(true);
	}
}