using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSCareerDocomoKetaiSaleReceiver の概要の説明です
/// </summary>
public class SBPSCareerDocomoKetaiSaleReceiver : SBPSBaseReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCareerDocomoKetaiSaleReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// 売上受取
	/// </summary>
	/// <returns>レスポンス文字列</returns>
	public override string Receive()
	{
		decimal? amountAuth = null;
		if (SBPSCreditAuthReceiver.TrackingIds.ContainsKey(m_requestData.TrackingId))	// 今のところ排他は考慮無し
		{
			amountAuth = SBPSCreditAuthReceiver.TrackingIds[m_requestData.TrackingId];
		}

		// 見つからなかった場合も成功で返すことにする
		if (amountAuth == null) return GetResponseXmlBase(true);

		return GetResponseXmlBase(true);
	}
}