/*
=========================================================================================================
  Module      : 国ISOコードのマスタテーブルリポジトリ (CountryLocationRepository.cs)
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

namespace w2.Domain.CountryLocation
{
	/// <summary>
	/// 国ISOコードのマスタテーブルリポジトリ
	/// </summary>
	internal class CountryLocationRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "CountryLocation";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal CountryLocationRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal CountryLocationRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="geonameId">リージョンID</param>
		/// <returns>モデル</returns>
		internal CountryLocationModel Get(string geonameId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COUNTRYLOCATION_GEONAME_ID, geonameId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new CountryLocationModel(dv[0]);
		}
		#endregion

		#region ~GetCountryNames 国名リストを取得
		/// <summary>
		/// 国名リストを取得
		/// </summary>
		/// <returns>国名リスト</returns>
		internal CountryLocationModel[] GetCountryNames()
		{
			var ht = new Hashtable { };
			var dv = Get(XML_KEY_NAME, "GetCountryNames", ht);
			return dv.Cast<DataRowView>().Select(drv => new CountryLocationModel(drv)).ToArray();
		}
		#endregion

		#region ~GetShippingAvailableCountry 配送可能な国を取得
		/// <summary>
		/// 配送可能な国を取得
		/// </summary>
		/// <returns>配送可能な国リスト</returns>
		internal CountryLocationModel[] GetShippingAvailableCountry()
		{
			var ht = new Hashtable{};
			var dv = Get(XML_KEY_NAME, "GetShippingAvailableCountry", ht);
			return dv.Cast<DataRowView>().Select(drv => new CountryLocationModel(drv)).ToArray();
		}
		#endregion
	}
}
