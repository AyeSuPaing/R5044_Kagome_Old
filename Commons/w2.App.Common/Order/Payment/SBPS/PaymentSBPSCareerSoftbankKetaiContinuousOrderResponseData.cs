/*
=========================================================================================================
  Module      : SBPS ソフトバンクまとめて支払い（B）「継続課金（定期・従量）購入要求処理」レスポンスデータ(PaymentSBPSCareerSoftbankKetaiContinuousOrderResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order
{
	/// <summary>
	/// SBPS ソフトバンクまとめて支払い（B）「継続課金（定期・従量）購入要求処理」レスポンスデータ
	/// </summary>
	public class PaymentSBPSCareerSoftbankKetaiContinuousOrderResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCareerSoftbankKetaiContinuousOrderResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
