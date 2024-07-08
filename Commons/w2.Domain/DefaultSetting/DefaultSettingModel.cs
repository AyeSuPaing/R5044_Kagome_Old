/*
=========================================================================================================
  Module      : Default Setting Model (DefaultSettingModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.DefaultSetting
{
	/// <summary>
	/// Default setting model
	/// </summary>
	public class DefaultSettingModel : ModelBase<DefaultSettingModel>
	{
		#region +Constructor
		/// <summary>
		/// Constructor default
		/// </summary>
		public DefaultSettingModel()
		{
			this.ShopId = Constants.CONST_DEFAULT_SHOP_ID;
			this.Classification = string.Empty;
			this.InitData = string.Empty;
			this.DateCreated = DateTime.Now;
			this.DateChanged = DateTime.Now;
			this.LastChanged = string.Empty;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Source</param>
		public DefaultSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Source</param>
		public DefaultSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region +Properties
		/// <summary>Shop id</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_DEFAULTSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_DEFAULTSETTING_SHOP_ID] = value; }
		}
		/// <summary>Classification</summary>
		public string Classification
		{
			get { return (string)this.DataSource[Constants.FIELD_DEFAULTSETTING_CLASSIFICATION]; }
			set { this.DataSource[Constants.FIELD_DEFAULTSETTING_CLASSIFICATION] = value; }
		}
		/// <summary>Init data</summary>
		public string InitData
		{
			get { return (string)this.DataSource[Constants.FIELD_DEFAULTSETTING_INIT_DATA]; }
			set { this.DataSource[Constants.FIELD_DEFAULTSETTING_INIT_DATA] = value; }
		}
		/// <summary>Date created</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DEFAULTSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_DEFAULTSETTING_DATE_CREATED] = value; }
		}
		/// <summary>Date changed</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DEFAULTSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_DEFAULTSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>Last changed</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_DEFAULTSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_DEFAULTSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
