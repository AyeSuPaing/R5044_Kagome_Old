/*
=========================================================================================================
  Module      : 国ISOコードのマスタテーブルモデル (CountryLocationModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.CountryLocation
{
	/// <summary>
	/// 国ISOコードのマスタテーブルモデル
	/// </summary>
	[Serializable]
	public partial class CountryLocationModel : ModelBase<CountryLocationModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CountryLocationModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CountryLocationModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CountryLocationModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>リージョンID</summary>
		public string GeonameId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUNTRYLOCATION_GEONAME_ID]; }
			set { this.DataSource[Constants.FIELD_COUNTRYLOCATION_GEONAME_ID] = value; }
		}
		/// <summary>ISO国コード</summary>
		public string CountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_COUNTRYLOCATION_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_COUNTRYLOCATION_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>国名</summary>
		public string CountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_COUNTRYLOCATION_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_COUNTRYLOCATION_COUNTRY_NAME] = value; }
		}
		/// <summary>最終更新日時</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COUNTRYLOCATION_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COUNTRYLOCATION_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
