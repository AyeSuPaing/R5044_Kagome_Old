/*
=========================================================================================================
  Module      : ユーザー情報のためのヘルパクラス (AddressHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Domain.User.Helper
{
	/// <summary>
	/// ユーザー情報のためのヘルパクラス
	/// </summary>
	public class AddressHelper
	{
		/// <summary>
		/// 住所項目を結合（国名は含めない）
		/// </summary>
		/// <param name="address1">住所情報１</param>
		/// <param name="address2">住所情報２</param>
		/// <param name="address3">住所情報３</param>
		/// <param name="address4">住所情報４</param>
		/// <returns>結合した住所</returns>
		public static string ConcatenateAddressWithoutCountryName(
			string address1,
			string address2,
			string address3,
			string address4)
		{
			var address = string.Format("{0}{1}{2} {3}", address1, address2, address3, address4).Trim();
			return address;
		}

		// <summary>
		/// 住所と国名の連結
		/// </summary>
		/// <param name="address1">住所情報１</param>
		/// <param name="address2">住所情報２</param>
		/// <param name="address3">住所情報３</param>
		/// <param name="address4">住所情報４</param>
		/// <param name="countryName">住所国名</param>
		/// <returns>結合した住所</returns>
		public static string ConcatenateAddressWithCountryName(
			string address1,
			string address2,
			string address3,
			string address4,
			string countryName)
		{
			var address = string.Format("{0}{1}{2} {3}{4}", address1, address2, address3, address4, countryName).Trim();
			return address;
		}
	}
}