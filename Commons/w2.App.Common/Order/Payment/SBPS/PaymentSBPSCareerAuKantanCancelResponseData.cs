/*
=========================================================================================================
  Module      : ソフトバンクペイメント AUかんたん支払い「取消・返金要求処理」レスポンスデータ(PaymentSBPSCareerAuKantanCancelResponseData.cs)
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
	/// ソフトバンクペイメント マルチ決済 AUかんたん支払い「取消・返金要求処理」レスポンスデータ
	/// </summary>
	public class PaymentSBPSCareerAuKantanCancelResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCareerAuKantanCancelResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
