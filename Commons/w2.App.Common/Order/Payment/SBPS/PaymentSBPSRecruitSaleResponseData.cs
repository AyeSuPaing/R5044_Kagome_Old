/*
=========================================================================================================
  Module      : ソフトバンクペイメント リクルートかんたん支払い「売上要求処理」レスポンスデータ(PaymentSBPSRecruitSaleResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチ決済 リクルートかんたん支払い「売上要求処理」レスポンスデータ
	/// </summary>
	public class PaymentSBPSRecruitSaleResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSRecruitSaleResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
