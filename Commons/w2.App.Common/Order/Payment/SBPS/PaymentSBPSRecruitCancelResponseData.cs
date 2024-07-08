/*
=========================================================================================================
  Module      : ソフトバンクペイメント リクルートかんたん支払い「取消・返要求処理」レスポンスデータ(PaymentSBPSRecruitCancelResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチ決済 リクルートかんたん支払い「取消・返要求処理」APIクラス
	/// </summary>
	public class PaymentSBPSRecruitCancelResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSRecruitCancelResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}
	}
}
