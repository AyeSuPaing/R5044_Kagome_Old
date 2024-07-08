/*
=========================================================================================================
  Module      : グローバルアドレスユーティリティ(GlobalAddressUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.DataCacheController;
using w2.Domain.CountryLocation;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Domain.GlobalZipcode;
using System.Web.UI.WebControls;

namespace w2.App.Common.Global
{
	/// <summary>
	/// グローバルアドレスユーティリティ
	/// </summary>
	public class GlobalAddressUtil
	{
		/// <summary>
		/// 国ISOコードをすべて取得
		/// </summary>
		/// <returns>国ISOコードリスト</returns>
		public static CountryLocationModel[] GetCountriesAll()
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return new CountryLocationModel[0];
			return DataCacheControllerFacade.GetCountryLocationCacheController().CacheData.Countries;
		}

		/// <summary>
		/// 国名取得
		/// </summary>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <returns>国名</returns>
		public static string GetCountryName(string countryIsoCode)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return "";
			if (DataCacheControllerFacade.GetCountryLocationCacheController().CacheData.IsoCodeToNames
				.ContainsKey(countryIsoCode))
			{
				var countryName = DataCacheControllerFacade.GetCountryLocationCacheController().CacheData.IsoCodeToNames[countryIsoCode];
				return countryName;
			}
			return "";
		}

		/// <summary>
		/// 国が日本かどうか
		/// </summary>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <returns>日本か</returns>
		public static bool IsCountryJp(string countryIsoCode)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return true;
			return (countryIsoCode == Constants.COUNTRY_ISO_CODE_JP);
		}

		/// <summary>
		/// 国がアメリカかどうか
		/// </summary>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <returns>アメリカか</returns>
		public static bool IsCountryUs(string countryIsoCode)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return false;
			return (countryIsoCode == Constants.COUNTRY_ISO_CODE_US);
		}

		/// <summary>
		/// 国が台湾かどうか
		/// </summary>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <returns>台湾か</returns>
		public static bool IsCountryTw(string countryIsoCode)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return false;
			return (countryIsoCode == Constants.COUNTRY_ISO_CODE_TW);
		}

		/// <summary>
		/// 郵便番号が必須かどうか
		/// </summary>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <returns>必須か</returns>
		public static bool IsAddrZipcodeNecessary(string countryIsoCode)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return true;
			var isZipNecessary = (Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
				"@@User.zip.globalAddress.necessary@@",
				"",
				countryIsoCode) == "1");
			return isZipNecessary;
		}

		/// <summary>
		/// 住所3が必須かどうか
		/// </summary>
		/// <param name="ownerAddrCountryIsoCode">国ISOコード</param>
		/// <returns>必須か</returns>
		public static bool IsAddress3Necessary(string ownerAddrCountryIsoCode)
		{
			var isAddr3Necessary =
				(IsCountryJp(ownerAddrCountryIsoCode)
					|| IsCountryTw(ownerAddrCountryIsoCode));
			return isAddr3Necessary;
		}

		/// <summary>
		/// 台湾地域ドロップダウンリスト生成
		/// </summary>
		/// <param name="wddlUserAddr3">台湾の住所３</param>
		/// <param name="wtbUserZipGlobal">台湾の郵便番号</param>
		/// <param name="userAddr3">台湾の住所３の元となる地域リスト</param>
		public static void BindingDdlUserAddr3(
			WrappedDropDownList wddlUserAddr3,
			WrappedTextBox wtbUserZipGlobal,
			object userAddr3)
		{
			wddlUserAddr3.DataSource = userAddr3;
			wddlUserAddr3.DataBind();
			wtbUserZipGlobal.Text = (string.IsNullOrEmpty(wddlUserAddr3.SelectedText))
				? string.Empty
				: wddlUserAddr3.SelectedValue.Split('|')[0];
		}

		/// <summary>
		/// Binding address by global zipcode
		/// </summary>
		/// <param name="countryIsoCode">Country ISO Code</param>
		/// <param name="globalZipCode">Global zipcode</param>
		/// <param name="wtbAddr2">Wrapped textbox address 2</param>
		/// <param name="wtbAddr4">Wrapped textbox address 4</param>
		/// <param name="wtbAddr5">Wrapped textbox address 5</param>
		/// <param name="wddlShippingAddr2">Wrapped dropdownlist Taiwan prefecture/city</param>
		/// <param name="wddlShippingAddr3">Wrapped dropdownlist Taiwan town</param>
		/// <param name="wddlShippingAddr5">Wrapped dropdownlist US state</param>
		public static void BindingAddressByGlobalZipcode(
			string countryIsoCode,
			string globalZipCode,
			WrappedTextBox wtbAddr2,
			WrappedTextBox wtbAddr4,
			WrappedTextBox wtbAddr5,
			WrappedDropDownList wddlShippingAddr2,
			WrappedDropDownList wddlShippingAddr3,
			WrappedDropDownList wddlShippingAddr5)
		{
			if (globalZipCode.Length < 3) return;

			var result = new GlobalZipcodeService().Get(
				countryIsoCode,
				StringUtility.ReplaceDelimiter(globalZipCode));
			if (result == null) return;

			// Is country Taiwan
			if (IsCountryTw(countryIsoCode))
			{
				ResetSelectedForWrappedDropDownList(wddlShippingAddr2, result.City);

				var selectedValue = wddlShippingAddr2.Items.FindByText(result.City);
				if (selectedValue != null)
				{
					wddlShippingAddr3.Items.Clear();
					wddlShippingAddr3.DataSource = Constants.TW_DISTRICT_DICT[result.City];
					wddlShippingAddr3.DataBind();
					ResetSelectedForWrappedDropDownList(wddlShippingAddr3, result.Address);

					wtbAddr4.Text = result.Province;
				}
				return;
			}

			// Is country US
			if (IsCountryUs(countryIsoCode))
			{
				wtbAddr2.Text = result.Address;
				wtbAddr4.Text = result.City;
				ResetSelectedForWrappedDropDownList(wddlShippingAddr5, result.Province);
				return;
			}

			// Set address information
			wtbAddr2.Text = result.Address;
			wtbAddr4.Text = result.City;
			wtbAddr5.Text = result.Province;
		}

		/// <summary>
		/// Reset selected for wrapped dropdownlist
		/// </summary>
		/// <param name="wrappedDropDownList">Wrapped dropdownlist</param>
		/// <param name="value">Value</param>
		private static void ResetSelectedForWrappedDropDownList(WrappedDropDownList wrappedDropDownList, string value)
		{
			var selectedValue = wrappedDropDownList.Items.FindByText(value);
			if (selectedValue == null) return;

			// Unselect all items
			foreach (ListItem item in wrappedDropDownList.Items)
			{
				item.Selected = false;
			}

			selectedValue.Selected = true;
		}
	}
}
