/*
=========================================================================================================
  Module      : ソフトバンクペイメント マルチ決済 ソフトバンク・ワイモバイルまとめて支払い「確定要求」レスポンスデータ(PaymentSBPSCareerSoftbankKetaiSaleResponseData.cs)
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
	/// ソフトバンクペイメント マルチ決済 ソフトバンク・ワイモバイルまとめて支払い「確定要求」レスポンスデータ
	/// </summary>
	public class PaymentSBPSCareerSoftbankKetaiSaleResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCareerSoftbankKetaiSaleResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
