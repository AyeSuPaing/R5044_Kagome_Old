/*
=========================================================================================================
  Module      : 国ISOコードのマスタテーブルサービスのインターフェース (ICountryLocationService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Domain.CountryLocation
{
	/// <summary>
	/// 国ISOコードのマスタテーブルサービスのインターフェース
	/// </summary>
	public interface ICountryLocationService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="geonameId">リージョンID</param>
		/// <returns>モデル</returns>
		CountryLocationModel Get(string geonameId);

		/// <summary>
		/// 配送可能な国を取得
		/// </summary>
		/// <returns>国名リスト</returns>
		CountryLocationModel[] GetCountryNames();

		/// <summary>
		/// 配送可能な国を取得
		/// </summary>
		/// <returns>配送可能な国リスト</returns>
		CountryLocationModel[] GetShippingAvailableCountry();
	}
}