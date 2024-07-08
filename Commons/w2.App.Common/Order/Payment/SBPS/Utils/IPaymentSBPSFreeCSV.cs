/*
=========================================================================================================
  Module      : ソフトバンクペイメント マルチ決済「フリー項目」文字列作成インターフェース(IPaymentSBPSFreeCSV.cs)
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
	/// ソフトバンクペイメント マルチ決済「フリー項目」文字列作成インターフェース
	/// </summary>
	public interface IPaymentSBPSFreeCSV
	{
		/// <summary>
		/// 「フリー項目」文字列取得
		/// </summary>
		/// <returns>「フリー項目」文字列</returns>
		string GetFreeCsvString();
	}
}
