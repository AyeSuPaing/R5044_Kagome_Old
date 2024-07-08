using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSCareerAuKantanCancelReceiver の概要の説明です
/// </summary>
public class SBPSRecruitCancelReceiver : SBPSBaseCancelReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSRecruitCancelReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}
}