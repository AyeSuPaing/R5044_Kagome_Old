/*
=========================================================================================================
  Module      : SBPS ドコモケータイ払い「継続課金（定期・従量）購入要求処理」レスポンスデータ(PaymentSBPSCareerDocomoKetaiContinuousOrderResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order
{
	/// <summary>
	/// SBPS ドコモケータイ払い「継続課金（定期・従量）購入要求処理」レスポンスデータ
	/// </summary>
	public class PaymentSBPSCareerDocomoKetaiContinuousOrderResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCareerDocomoKetaiContinuousOrderResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
