/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「顧客情報 登録」API レスポンスデータ(PaymentSBPSCreditCustomerRegistResponseData.cs)
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
	/// ソフトバンクペイメント クレジット「顧客情報 登録」API レスポンスデータ
	/// </summary>
	public class PaymentSBPSCreditCustomerRegistResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCreditCustomerRegistResponseData(PaymentSBPSSetting settings)
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
