/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「決済要求（トークン利用）」API レスポンスデータ(PaymentSBPSCreditAuthResponseData.cs)
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
	/// ソフトバンクペイメント クレジット「決済要求（トークン利用）」API レスポンスデータ
	/// </summary>
	public class PaymentSBPSCreditAuthWithTokenResponseData : PaymentSBPSCreditAuthResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCreditAuthWithTokenResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}

		/// <summary>トークン有効期限切れか</summary>
		public bool IsTokenExpired
		{
			get { return PaymentSBPSUtil.IsCreditTokenExpired(this.ResErrCode); }
		}
	}
}
