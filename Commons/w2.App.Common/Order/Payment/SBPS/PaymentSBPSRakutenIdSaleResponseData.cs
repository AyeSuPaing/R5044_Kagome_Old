/*
=========================================================================================================
  Module      : ソフトバンクペイメント 楽天ペイ「売上要求処理」レスポンスデータ(PaymentSBPSRakutenIdSaleResponseData.cs)
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
	/// ソフトバンクペイメント マルチ決済 楽天ペイ「売上要求処理」レスポンスデータ
	/// </summary>
	public class PaymentSBPSRakutenIdSaleResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSRakutenIdSaleResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
