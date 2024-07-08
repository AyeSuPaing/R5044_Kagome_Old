/*
=========================================================================================================
  Module      : 配送リードタイムマスタモデル (DeliveryLeadTimeModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.DeliveryLeadTime
{
	/// <summary>
	/// 配送リードタイムマスタモデル
	/// </summary>
	[Serializable]
	public partial class DeliveryLeadTimeModel : ModelBase<DeliveryLeadTimeModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DeliveryLeadTimeModel()
		{
			// TODO:定数を利用するよう書き換えてください。
			this.LeadTimeZoneNo = 0;
			this.ShippingLeadTime = 0;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DeliveryLeadTimeModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DeliveryLeadTimeModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_SHOP_ID] = value; }
		}
		/// <summary>配送会社ID</summary>
		public string DeliveryCompanyId
		{
			get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DELIVERY_COMPANY_ID]; }
			set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DELIVERY_COMPANY_ID] = value; }
		}
		/// <summary>リードタイム地帯区分</summary>
		public int LeadTimeZoneNo
		{
			get { return (int)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NO]; }
			set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NO] = value; }
		}
		/// <summary>地帯名</summary>
		public string LeadTimeZoneName
		{
			get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NAME]; }
			set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NAME] = value; }
		}
		/// <summary>郵便番号</summary>
		public string Zip
		{
			get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_ZIP]; }
			set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_ZIP] = value; }
		}
		/// <summary>追加配送リードタイム</summary>
		public int ShippingLeadTime
		{
			get { return (int)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_SHIPPING_LEAD_TIME]; }
			set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_SHIPPING_LEAD_TIME] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LAST_CHANGED] = value; }
		}
		#endregion
	}
}