/*
=========================================================================================================
  Module      : SBPS ドコモケータイ払い「継続課金（定期・従量）解約要求処理」レスポンスデータ(PaymentSBPSCareerDocomoKetaiContinuousCancelResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order
{
	/// <summary>
	/// SBPS ドコモケータイ払い「継続課金（定期・従量）解約要求処理」レスポンスデータ
	/// </summary>
	public class PaymentSBPSCareerDocomoKetaiContinuousCancelResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCareerDocomoKetaiContinuousCancelResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}