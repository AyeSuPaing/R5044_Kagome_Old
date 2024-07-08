/*
=========================================================================================================
  Module      : Global Zipcode Service (GlobalZipcodeService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.GlobalZipcode
{
	/// <summary>
	/// Global zipcode service
	/// </summary>
	public class GlobalZipcodeService : ServiceBase, IGlobalZipcodeService
	{
		#region +Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="countryIsoCode">Country ISO code</param>
		/// <param name="globalZipcode">Global zipcode</param>
		/// <returns>Global zipcode model</returns>
		public GlobalZipcodeModel Get(string countryIsoCode, string globalZipcode)
		{
			using (var repository = new GlobalZipcodeRepository())
			{
				var model = repository.Get(countryIsoCode, globalZipcode);
				return model;
			}
		}
		#endregion
	}
}
