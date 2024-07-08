/*
=========================================================================================================
  Module      : Global Zipcode Repository (GlobalZipcodeRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.GlobalZipcode
{
	/// <summary>
	/// Global zipcode repository
	/// </summary>
	public class GlobalZipcodeRepository : RepositoryBase
	{
		/// <summary>XML key name</summary>
		private const string XML_KEY_NAME = "GlobalZipcode";

		#region +Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		public GlobalZipcodeRepository()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="accessor">Sql accessor</param>
		public GlobalZipcodeRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="countryIsoCode">Country ISO code</param>
		/// <param name="globalZipcode">Global zipcode</param>
		/// <returns>Global zipcode model</returns>
		public GlobalZipcodeModel Get(string countryIsoCode, string globalZipcode)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_GLOBALZIPCODE_COUNTRY_ISO_CODE, countryIsoCode },
				{ Constants.FIELD_GLOBALZIPCODE_ZIPCODE, globalZipcode },
			};
			var result = Get(XML_KEY_NAME, "Get", input);

			if (result.Count == 0) return null;

			return new GlobalZipcodeModel(result[0]);
		}
		#endregion
	}
}
