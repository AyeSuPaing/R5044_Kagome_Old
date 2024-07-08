using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSPaypalCancelReceiver の概要の説明です
/// </summary>
public class SBPSPaypalCancelReceiver : SBPSBaseCancelReceiver
{
    /// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSPaypalCancelReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}     
}
