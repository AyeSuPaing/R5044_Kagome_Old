/*
=========================================================================================================
  Module      : Global Zipcode Model (GlobalZipcodeModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.GlobalZipcode
{
	/// <summary>
	/// Global zipcode model
	/// </summary>
	[Serializable]
	public partial class GlobalZipcodeModel : ModelBase<GlobalZipcodeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GlobalZipcodeModel()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalZipcodeModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalZipcodeModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>Country ISO code</summary>
		public string CountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALZIPCODE_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_GLOBALZIPCODE_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>Zipcode</summary>
		public string Zipcode
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALZIPCODE_ZIPCODE]; }
			set { this.DataSource[Constants.FIELD_GLOBALZIPCODE_ZIPCODE] = value; }
		}
		/// <summary>Country</summary>
		public string Country
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALZIPCODE_COUNTRY]; }
			set { this.DataSource[Constants.FIELD_GLOBALZIPCODE_COUNTRY] = value; }
		}
		/// <summary>Province</summary>
		public string Province
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALZIPCODE_PROVINCE]; }
			set { this.DataSource[Constants.FIELD_GLOBALZIPCODE_PROVINCE] = value; }
		}
		/// <summary>City</summary>
		public string City
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALZIPCODE_CITY]; }
			set { this.DataSource[Constants.FIELD_GLOBALZIPCODE_CITY] = value; }
		}
		/// <summary>Address</summary>
		public string Address
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALZIPCODE_ADDRESS]; }
			set { this.DataSource[Constants.FIELD_GLOBALZIPCODE_ADDRESS] = value; }
		}
		#endregion
	}
}
