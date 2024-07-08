﻿/*
=========================================================================================================
  Module      : ソフトバンクペイメント ドコモケータイ払い「売上要求処理」レスポンスデータ(PaymentSBPSCareerDocomoKetaiSaleResponseData.cs)
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
	/// ソフトバンクペイメント マルチ決済 ドコモケータイ払い「売上要求処理」レスポンスデータ
	/// </summary>
	public class PaymentSBPSCareerDocomoKetaiSaleResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCareerDocomoKetaiSaleResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
