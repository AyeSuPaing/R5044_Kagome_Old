/*
=========================================================================================================
  Module      : 国ISOコードのマスタテーブルサービス (CountryLocationService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;

namespace w2.Domain.CountryLocation
{
	/// <summary>
	/// 国ISOコードのマスタテーブルサービス
	/// </summary>
	public class CountryLocationService : ServiceBase, ICountryLocationService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="geonameId">リージョンID</param>
		/// <returns>モデル</returns>
		public CountryLocationModel Get(string geonameId)
		{
			using (var repository = new CountryLocationRepository())
			{
				var model = repository.Get(geonameId);
				return model;
			}
		}
		#endregion

		#region +GetCountryNames 国名リストを取得
		/// <summary>
		/// 配送可能な国を取得
		/// </summary>
		/// <returns>国名リスト</returns>
		public CountryLocationModel[] GetCountryNames()
		{
			using (var repository = new CountryLocationRepository())
			{
				var result = repository.GetCountryNames();
				return result;
			}
		}
		#endregion

		#region +GetShippingAvailableCountry 配送可能な国を取得
		/// <summary>
		/// 配送可能な国を取得
		/// </summary>
		/// <returns>配送可能な国リスト</returns>
		public CountryLocationModel[] GetShippingAvailableCountry()
		{
			using (var repository = new CountryLocationRepository())
			{
				var result = repository.GetShippingAvailableCountry();
				return result;
			}
		}
		#endregion
	}
}
