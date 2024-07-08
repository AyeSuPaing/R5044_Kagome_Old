/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「コミット」API レスポンスデータ(PaymentSBPSCreditCommitResponseData.cs)
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
	/// ソフトバンクペイメント クレジット「コミット」API レスポンスデータ
	/// </summary>
	public class PaymentSBPSCreditCommitResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCreditCommitResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
