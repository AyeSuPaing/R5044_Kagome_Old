using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSRakutenIdCancelReceiver の概要の説明です
/// </summary>
public class SBPSRakutenIdCancelReceiver : SBPSBaseCancelReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSRakutenIdCancelReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}
}