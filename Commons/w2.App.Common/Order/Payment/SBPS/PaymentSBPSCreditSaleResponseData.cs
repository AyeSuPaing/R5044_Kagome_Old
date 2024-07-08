/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「API売上連携」 レスポンスデータ(PaymentSBPSCreditSaleApiResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント クレジット「API売上連携」 レスポンスデータ
	/// </summary>
	public class PaymentSBPSCreditSaleResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCreditSaleResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
