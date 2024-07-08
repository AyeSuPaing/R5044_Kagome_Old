/*
=========================================================================================================
  Module      : リージョン判定IP範囲テーブルリポジトリ (CountryIpv4Repository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.CountryIpv4
{
	/// <summary>
	/// リージョン判定IP範囲テーブルリポジトリ
	/// </summary>
	internal class CountryIpv4Repository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "CountryIpv4";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal CountryIpv4Repository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal CountryIpv4Repository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetByIpNumeric IP(数値)より取得
		/// <summary>
		/// IP(数値)より取得
		/// </summary>
		/// <param name="ipNumeric">ネットワークアドレス_数値</param>
		/// <returns>モデル</returns>
		internal CountryIpv4Model GetByIpNumeric(int ipNumeric)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COUNTRYIPV4_IP_NUMERIC, ipNumeric},
			};
			var dv = Get(XML_KEY_NAME, "GetByIpNumeric", ht);
			if (dv.Count == 0) return null;
			return new CountryIpv4Model(dv[0]);
		}
		#endregion
	}
}
