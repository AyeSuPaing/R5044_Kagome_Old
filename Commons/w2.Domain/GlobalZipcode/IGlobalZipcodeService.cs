/*
=========================================================================================================
  Module      : Global Zipcode Service Interface (IGlobalZipcodeService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.GlobalZipcode
{
	/// <summary>
	/// Global zipcode service interface
	/// </summary>
	public interface IGlobalZipcodeService : IService
	{
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="countryIsoCode">Country ISO code</param>
		/// <param name="globalZipcode">Global zipcode</param>
		/// <returns>Global zipcode model</returns>
		GlobalZipcodeModel Get(string countryIsoCode, string globalZipcode);
	}
}
