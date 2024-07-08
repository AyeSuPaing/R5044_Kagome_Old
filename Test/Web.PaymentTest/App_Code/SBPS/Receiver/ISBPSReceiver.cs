using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ISBPSReceiver の概要の説明です
/// </summary>
public interface ISBPSReceiver
{
	/// <summary>
	/// 受信
	/// </summary>
	/// <returns>レスポンスXML</returns>
	string Receive();
}