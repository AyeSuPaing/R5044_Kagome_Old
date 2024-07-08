/*
=========================================================================================================
  Module      : Paygent API POSTリクエストボディ インタ－フェース(IPaygentPostBody.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.App.Common.Order.Payment.Paygent.Foundation
{
	/// <summary>
	/// POSTリクエストボディ インタ－フェース
	/// </summary>
	public interface IPaygentPostBody
	{
		/// <summary>
		/// リクエストパラメータの生成
		/// </summary>
		/// <returns>リクエストパラメーター</returns>
		Dictionary<string, string> GenerateKeyValues();
	}
}
