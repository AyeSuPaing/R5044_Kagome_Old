/*
=========================================================================================================
  Module      : SBPS auかんたん決済継続課金の解約API結果(SBPSCareerAuKantanContinuousCancelReceiver.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

/// <summary>
/// SBPS auかんたん決済継続課金の解約API結果クラス
/// </summary>
public class SBPSCareerAuKantanContinuousCancelReceiver : SBPSBaseReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCareerAuKantanContinuousCancelReceiver(SBPSApiRequestData requestData)
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
		return GetResponseXmlForContinuousOrder(this.IsSuccessTestPattern, "402L5000");
	}
}