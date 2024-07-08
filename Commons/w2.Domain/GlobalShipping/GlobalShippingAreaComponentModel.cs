/*
=========================================================================================================
  Module      : 海外配送エリア構成モデル (GlobalShippingAreaComponentModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.GlobalShipping
{
	/// <summary>
	/// 海外配送エリア構成モデル
	/// </summary>
	[Serializable]
	public partial class GlobalShippingAreaComponentModel : ModelBase<GlobalShippingAreaComponentModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public GlobalShippingAreaComponentModel()
		{
			this.GlobalShippingAreaId = "";
			this.CountryIsoCode = "";
			this.ConditionAddr5 = "";
			this.ConditionAddr4 = "";
			this.ConditionAddr3 = "";
			this.ConditionAddr2 = "";
			this.ConditionZip = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalShippingAreaComponentModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalShippingAreaComponentModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>シーケンス</summary>
		public long Seq
		{
			get { return (long)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_SEQ]; }
		}
		/// <summary>地域ID</summary>
		public string GlobalShippingAreaId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_GLOBAL_SHIPPING_AREA_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_GLOBAL_SHIPPING_AREA_ID] = value; }
		}
		/// <summary>ISO国コード</summary>
		public string CountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>住所5条件</summary>
		public string ConditionAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR5]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR5] = value; }
		}
		/// <summary>住所4条件</summary>
		public string ConditionAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR4]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR4] = value; }
		}
		/// <summary>住所3条件</summary>
		public string ConditionAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR3]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR3] = value; }
		}
		/// <summary>住所2条件</summary>
		public string ConditionAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR2]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR2] = value; }
		}
		/// <summary>郵便番号条件</summary>
		public string ConditionZip
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ZIP]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ZIP] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
