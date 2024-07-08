/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 請求金額変更（減額）依頼レスポンスデータクラス(PaymentYamatoKaReduceResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 請求金額変更（減額）依頼レスポンスデータクラス
	/// </summary>
	public class PaymentYamatoKaReduceResponseData : PaymentYamatoKaBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal PaymentYamatoKaReduceResponseData(string responseString)
			: base(responseString)
		{
		}
	}
}
