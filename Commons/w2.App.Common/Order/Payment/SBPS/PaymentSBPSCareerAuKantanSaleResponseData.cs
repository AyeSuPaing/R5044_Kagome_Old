/*
=========================================================================================================
  Module      : ソフトバンクペイメント AUかんたん支払い「売上要求処理」レスポンスデータ(PaymentSBPSCareerAuKantanSaleResponseData.cs)
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
	/// ソフトバンクペイメント マルチ決済 AUかんたん支払い「売上要求処理」レスポンスデータ
	/// </summary>
	public class PaymentSBPSCareerAuKantanSaleResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCareerAuKantanSaleResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
