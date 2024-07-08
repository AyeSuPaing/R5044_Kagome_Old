/*
=========================================================================================================
  Module      : SBPS ドコモケータイ支払い継続課金の解約API結果(SBPSCareerDocomoKetaiContinuousCancelReceiver.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

/// <summary>
/// SBPS ドコモケータイ支払い継続課金の解約API結果クラス
/// </summary>
public class SBPSCareerDocomoKetaiContinuousCancelReceiver : SBPSBaseReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCareerDocomoKetaiContinuousCancelReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// 継続課金の解約受取
	/// </summary>
	/// <returns>レスポンス文字列</returns>
	public override string Receive()
	{
		// TrackingIdに応じて、成功・失敗パターンの結果を返す
		return GetResponseXmlForContinuousOrder(this.IsSuccessTestPattern, "401L5000");
	}
}