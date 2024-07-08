/*
=========================================================================================================
  Module      : AmazonCv2APIのファサードインターフェース(IAmazonCv2ApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Amazon.Pay.API.WebStore.Charge;

namespace w2.App.Common.AmazonCv2
{
	public interface IAmazonCv2ApiFacade : IService
	{
		/// <summary>
		/// チャージ取得
		/// </summary>
		/// <param name="chargeId">チャージID</param>
		/// <returns>チャージレスポンス</returns>
		ChargeResponse GetCharge(string chargeId);
	}
}
