/*
=========================================================================================================
  Module      : 配送国 ユーティリティクラス(ShippingCountryUtility.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Domain.CountryLocation;

namespace w2.App.Common.Global
{
	/// <summary>
	/// 配送国 ユーティリティクラス
	/// </summary>
	public class ShippingCountryUtil
	{
		/// <summary>
		/// 配送可能な国を取得し、配送先の国が配送可能かをチェック
		/// </summary>
		/// <param name="shippingCountryIsoCode">配送先のISO国コード</param>
		/// <returns>true：配送可能, false：配送不可</returns>
		public static bool GetShippingCountryAvailableListAndCheck(string shippingCountryIsoCode)
		{
			var shippingAvailableCountryList = new CountryLocationService().GetShippingAvailableCountry();
			return CheckShippingCountryAvailable(shippingAvailableCountryList, shippingCountryIsoCode);
		}

		/// <summary>
		/// 配送先の国が配送可能かをチェック
		/// </summary>
		/// <param name="shippingAvailableCountryList">配送可能な国リスト</param>
		/// <param name="shippingCountryIsoCode">配送先のISO国コード</param>
		/// <returns>true：配送可能, false：配送不可</returns>
		public static bool CheckShippingCountryAvailable(CountryLocationModel[] shippingAvailableCountryList, string shippingCountryIsoCode)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return true;
			if (shippingAvailableCountryList == null) return false;

			var isShippingAvailableCountry = shippingAvailableCountryList.Any(c => (c.CountryIsoCode == shippingCountryIsoCode));
			return isShippingAvailableCountry;
		}
	}
}
