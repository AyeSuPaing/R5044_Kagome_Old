/*
=========================================================================================================
  Module      : 出荷集計モデル (DailyOrderShipmentForecastModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.DailyOrderShipmentForecast
{
	/// <summary>
	/// 出荷集計モデル
	/// </summary>
	[Serializable]
	public partial class DailyOrderShipmentForecastModel : ModelBase<DailyOrderShipmentForecastModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DailyOrderShipmentForecastModel()
		{
			this.ShipmentOrderCount = 0;
			this.TotalOrderPriceSubtotal = 0;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DailyOrderShipmentForecastModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DailyOrderShipmentForecastModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>出荷日</summary>
		public DateTime ShipmentDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_DATE]; }
			set { this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_DATE] = value; }
		}
		/// <summary>出荷数</summary>
		public long ShipmentOrderCount
		{
			get { return (long)this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT]; }
			set { this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT] = value; }
		}
		/// <summary>出荷商品金額合計</summary>
		public decimal TotalOrderPriceSubtotal
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL]; }
			set { this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHIPMENTQUANTITY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
