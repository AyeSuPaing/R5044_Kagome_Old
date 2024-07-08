/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「決済要求（永久トークン利用）」API レスポンスデータ(PaymentSBPSCreditAuthWithTokenizedPanResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
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
	/// ソフトバンクペイメント クレジット「決済要求（永久トークン利用）」API レスポンスデータ
	/// </summary>
	public class PaymentSBPSCreditAuthWithTokenizedPanResponseData : PaymentSBPSCreditAuthResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCreditAuthWithTokenizedPanResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
