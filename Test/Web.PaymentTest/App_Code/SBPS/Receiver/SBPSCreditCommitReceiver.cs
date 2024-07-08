using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSCreditCommitReceiver の概要の説明です
/// </summary>
public class SBPSCreditCommitReceiver : SBPSBaseReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCreditCommitReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// クレジットコミット受取
	/// </summary>
	/// <returns>レスポンス文字列</returns>
	public override string Receive()
	{
		if (m_requestData.TrackingId.StartsWith(DAMMY_STRING))
		{
			return GetResponseXmlBase(true);
		}
		else
		{
			return GetResponseXmlBase(false);
		}
	}


}