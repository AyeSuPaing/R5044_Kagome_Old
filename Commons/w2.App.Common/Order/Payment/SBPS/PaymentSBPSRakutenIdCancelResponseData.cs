/*
=========================================================================================================
  Module      : ソフトバンクペイメント 楽天ペイ「取消・返要求処理」レスポンスデータ(PaymentSBPSRakutenIdCancelResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチ決済 楽天ペイ「取消・返要求処理」APIクラス
	/// </summary>
	public class PaymentSBPSRakutenIdCancelResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSRakutenIdCancelResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
