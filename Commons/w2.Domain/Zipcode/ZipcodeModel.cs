/*
=========================================================================================================
  Module      : 郵便番号マスタモデル (ZipcodeModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Zipcode
{
	/// <summary>
	/// 郵便番号マスタモデル
	/// </summary>
	[Serializable]
	public partial class ZipcodeModel : ModelBase<ZipcodeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ZipcodeModel()
		{
			this.Flg1 = "0";
			this.Flg2 = "0";
			this.Flg3 = "0";
			this.Flg4 = "0";
			this.Flg5 = "0";
			this.Flg6 = "0";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ZipcodeModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ZipcodeModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>全国地方公共団体コード</summary>
		public string LocalPubCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_LOCAL_PUB_CODE]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_LOCAL_PUB_CODE] = value; }
		}
		/// <summary>(旧)郵便番号</summary>
		public string ZipcodeOld
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_ZIPCODE_OLD]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_ZIPCODE_OLD] = value; }
		}
		/// <summary>郵便番号</summary>
		public string Zipcode
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_ZIPCODE]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_ZIPCODE] = value; }
		}
		/// <summary>都道府県名かな</summary>
		public string PrefectureKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_PREFECTURE_KANA]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_PREFECTURE_KANA] = value; }
		}
		/// <summary>市区町村名かな</summary>
		public string CityKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_CITY_KANA]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_CITY_KANA] = value; }
		}
		/// <summary>町域名かな</summary>
		public string TownKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_TOWN_KANA]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_TOWN_KANA] = value; }
		}
		/// <summary>都道府県名　</summary>
		public string Prefecture
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_PREFECTURE]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_PREFECTURE] = value; }
		}
		/// <summary>市区町村名</summary>
		public string City
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_CITY]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_CITY] = value; }
		}
		/// <summary>町域名</summary>
		public string Town
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_TOWN]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_TOWN] = value; }
		}
		/// <summary>フラグ1</summary>
		public string Flg1
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_FLG1]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_FLG1] = value; }
		}
		/// <summary>フラグ2</summary>
		public string Flg2
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_FLG2]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_FLG2] = value; }
		}
		/// <summary>フラグ3</summary>
		public string Flg3
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_FLG3]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_FLG3] = value; }
		}
		/// <summary>フラグ4</summary>
		public string Flg4
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_FLG4]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_FLG4] = value; }
		}
		/// <summary>フラグ5</summary>
		public string Flg5
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_FLG5]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_FLG5] = value; }
		}
		/// <summary>フラグ6</summary>
		public string Flg6
		{
			get { return (string)this.DataSource[Constants.FIELD_ZIPCODE_FLG6]; }
			set { this.DataSource[Constants.FIELD_ZIPCODE_FLG6] = value; }
		}
		#endregion
	}
}
