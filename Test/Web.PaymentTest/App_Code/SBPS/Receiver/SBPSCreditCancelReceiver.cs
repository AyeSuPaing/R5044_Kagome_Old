using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSCreditCancelReceiver の概要の説明です
/// </summary>
public class SBPSCreditCancelReceiver : SBPSBaseCancelReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCreditCancelReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}
}