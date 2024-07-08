/*
=========================================================================================================
  Module      : ヤマトKWC 出荷情報取消APIレスポンスデータ(PaymentYamatoKwcShipmentCancelResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC 出荷情報取消APIレスポンスデータ
	/// </summary>
	public class PaymentYamatoKwcShipmentCancelResponseData : PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKwcShipmentCancelResponseData(string responseString)
			: base(responseString)
		{
		}
	}
}
