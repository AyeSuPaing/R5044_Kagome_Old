using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SBPSCreditSalesReceiver の概要の説明です
/// </summary>
public class SBPSCreditSaleReceiver : SBPSBaseReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCreditSaleReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// クレジット売上受取
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

		// 売上金額を超えていたらエラーとする
		if ((m_requestData.AmountSales != null) && (m_requestData.AmountSales > amountAuth.Value))
		{
			return GetResponseXmlBase(false);
		}
		return GetResponseXmlBase(true);
	}
}