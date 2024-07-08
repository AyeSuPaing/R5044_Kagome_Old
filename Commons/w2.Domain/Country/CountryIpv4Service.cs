/*
=========================================================================================================
  Module      : リージョン判定IP範囲テーブルサービス (CountryIpv4Service.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;

namespace w2.Domain.CountryIpv4
{
	/// <summary>
	/// リージョン判定IP範囲テーブルサービス
	/// </summary>
	public class CountryIpv4Service : ServiceBase
	{
		#region +GetByIpNumeric IP(数値)より取得
		/// <summary>
		/// IP(数値)より取得
		/// </summary>
		/// <param name="ipNumeric">ネットワークアドレス_数値</param>
		/// <returns>モデル</returns>
		public CountryIpv4Model GetByIpNumeric(int ipNumeric)
		{
			using (var repository = new CountryIpv4Repository())
			{
				var model = repository.GetByIpNumeric(ipNumeric);
				return model;
			}
		}
		#endregion
	}
}
