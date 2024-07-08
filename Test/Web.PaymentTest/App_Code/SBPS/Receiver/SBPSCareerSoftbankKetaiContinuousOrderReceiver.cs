/*
=========================================================================================================
  Module      : SBPS ソフトバンクまとめて支払い（B）継続課金の購入API結果(SBPSCareerSoftbankKetaiContinuousOrderReceiver.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

/// <summary>
/// SBPS ソフトバンクまとめて支払い（B）継続課金の購入API結果クラス
/// </summary>
public class SBPSCareerSoftbankKetaiContinuousOrderReceiver : SBPSBaseReceiver
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="requestData">リクエストデータ</param>
	public SBPSCareerSoftbankKetaiContinuousOrderReceiver(SBPSApiRequestData requestData)
		: base(requestData)
	{
	}

	/// <summary>
	/// 継続課金の購入受取
	/// </summary>
	/// <returns>レスポンス文字列</returns>
	public override string Receive()
	{
		// TrackingIdに応じて、成功・失敗パターンの結果を返す
		return GetResponseXmlForContinuousOrder(this.IsSuccessTestPattern, "405L0000");
	}
}