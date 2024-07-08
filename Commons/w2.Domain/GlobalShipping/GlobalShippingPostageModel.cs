/*
=========================================================================================================
  Module      : 海外配送料金表モデル (GlobalShippingPostageModel.cs)
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
	/// 海外配送料金表モデル
	/// </summary>
	[Serializable]
	public partial class GlobalShippingPostageModel : ModelBase<GlobalShippingPostageModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public GlobalShippingPostageModel()
		{
			this.WeightGramGreaterThanOrEqualTo = 0;
			this.WeightGramLessThan = 0;
			this.GlobalShippingPostage = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalShippingPostageModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public GlobalShippingPostageModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>シーケンス</summary>
		public long Seq
		{
			get { return (long)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SEQ]; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHOP_ID] = value; }
		}
		/// <summary>配送種別ID</summary>
		public string ShippingId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHIPPING_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHIPPING_ID] = value; }
		}
		/// <summary>配送会社ID</summary>
		public string DeliveryCompanyId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_DELIVERY_COMPANY_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_DELIVERY_COMPANY_ID] = value; }
		}
		/// <summary>地域ID</summary>
		public string GlobalShippingAreaId
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_AREA_ID]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_AREA_ID] = value; }
		}
		/// <summary>重量（～g以上）</summary>
		public long WeightGramGreaterThanOrEqualTo
		{
			get { return (long)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_GREATER_THAN_OR_EQUAL_TO]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_GREATER_THAN_OR_EQUAL_TO] = value; }
		}
		/// <summary>重量（～g以下）</summary>
		public long WeightGramLessThan
		{
			get { return (long)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_LESS_THAN]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_LESS_THAN] = value; }
		}
		/// <summary>送料</summary>
		public decimal GlobalShippingPostage
		{
			get { return (decimal)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_POSTAGE]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_POSTAGE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGPOSTAGE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
