/*
=========================================================================================================
  Module      : ソフトバンクペイメント PayPal「取消・返金要求処理」レスポンスデータ(PaymentSBPSPaypalCancelResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチ決済 PayPal「取消・返金要求処理」レスポンスデータ
	/// </summary>
	public class PaymentSBPSPaypalCancelResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSPaypalCancelResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
