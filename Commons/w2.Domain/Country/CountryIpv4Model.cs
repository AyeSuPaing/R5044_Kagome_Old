/*
=========================================================================================================
  Module      : リージョン判定IP範囲テーブルモデル (CountryIpv4Model.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.CountryIpv4
{
	/// <summary>
	/// リージョン判定IP範囲テーブルモデル
	/// </summary>
	[Serializable]
	public partial class CountryIpv4Model : ModelBase<CountryIpv4Model>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CountryIpv4Model()
		{
			this.IpNumeric = 0;
			this.IpBroadcastNumeric = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CountryIpv4Model(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CountryIpv4Model(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ネットワークアドレス_数値</summary>
		public int IpNumeric
		{
			get { return (int)this.DataSource[Constants.FIELD_COUNTRYIPV4_IP_NUMERIC]; }
			set { this.DataSource[Constants.FIELD_COUNTRYIPV4_IP_NUMERIC] = value; }
		}
		/// <summary>ブロードキャスト_数値</summary>
		public int IpBroadcastNumeric
		{
			get { return (int)this.DataSource[Constants.FIELD_COUNTRYIPV4_IP_BROADCAST_NUMERIC]; }
			set { this.DataSource[Constants.FIELD_COUNTRYIPV4_IP_BROADCAST_NUMERIC] = value; }
		}
		/// <summary>ネットワークアドレス</summary>
		public string Ip
		{
			get { return (string)this.DataSource[Constants.FIELD_COUNTRYIPV4_IP]; }
			set { this.DataSource[Constants.FIELD_COUNTRYIPV4_IP] = value; }
		}
		/// <summary>ブロードキャスト</summary>
		public string IpBroadcast
		{
			get { return (string)this.DataSource[Constants.FIELD_COUNTRYIPV4_IP_BROADCAST]; }
			set { this.DataSource[Constants.FIELD_COUNTRYIPV4_IP_BROADCAST] = value; }
		}
		/// <summary>リージョンID</summary>
		public string GeonameId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUNTRYIPV4_GEONAME_ID]; }
			set { this.DataSource[Constants.FIELD_COUNTRYIPV4_GEONAME_ID] = value; }
		}
		/// <summary>最終更新日時</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COUNTRYIPV4_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COUNTRYIPV4_DATE_CHANGED] = value; }
		}
		#endregion
	}
}
