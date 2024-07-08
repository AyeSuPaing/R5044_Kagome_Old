/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 出荷情報依頼レスポンスデータクラス(PaymentYamatoKaShipmentEntryResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
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
	/// ヤマト決済(後払い) 出荷情報依頼レスポンスデータクラス
	/// </summary>
	public class PaymentYamatoKaShipmentEntryResponseData : PaymentYamatoKaBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal PaymentYamatoKaShipmentEntryResponseData(string responseString)
			: base(responseString)
		{
		}
	}
}
