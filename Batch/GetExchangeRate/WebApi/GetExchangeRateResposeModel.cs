/*
=========================================================================================================
  Module      : GetExchangeRateRespose モデルクラス(GetExchangeRateResposeModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.Commerce.GetExchangeRate.WebApi
{
	/// <summary>
	/// ExchangeRate モデルクラス
	/// </summary>
	public class GetExchangeRateResposeModel
	{
		/// <summary>成功/失敗</summary>
		public bool Success { get; set; }
		/// <summary>通貨コード（元）</summary>
		public string Source { get; set; }
		/// <summary>為替レートデータ</summary>
		public Dictionary<string, string> Quotes { get; set; }
	}
}
