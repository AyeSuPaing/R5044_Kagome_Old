using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSBaseCancelReceiver の概要の説明です
/// </summary>
public class SBPSBaseCancelReceiver : SBPSBaseReceiver
{
	public readonly static Dictionary<string, decimal> TrackingIds = SBPSCreditAuthReceiver.TrackingIds;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="request">リクエストデータ</param>
	public SBPSBaseCancelReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// キャンセル受取
	/// </summary>
	/// <param name="req">リクエスト</param>
	/// <returns>レスポンス文字列</returns>
	public override string Receive()
	{
		decimal? amountAuth = null;
		if (SBPSBaseCancelReceiver.TrackingIds.ContainsKey(m_requestData.TrackingId))
		{
			amountAuth = SBPSBaseCancelReceiver.TrackingIds[m_requestData.TrackingId];
		}

		if (amountAuth != null)
		{
			SBPSCreditAuthReceiver.TrackingIds.Remove(m_requestData.TrackingId);
			return GetResponseXmlBase(true);
		}

		// 単体テスト用
		if (m_requestData.TrackingId.StartsWith("ngtest"))
		{
			return GetResponseXmlBase(false);
		}

		// 見つからなかった場合も成功で返すことにする
		return GetResponseXmlBase(true);
	}
}