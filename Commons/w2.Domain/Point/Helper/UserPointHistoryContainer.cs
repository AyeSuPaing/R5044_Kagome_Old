/*
=========================================================================================================
  Module      : User Point History Container (UserPointHistoryContainer.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;

namespace w2.Domain.Point.Helper
{
	#region +ユーザーポイント履歴表示用クラス
	/// <summary>
	/// ユーザーポイント履歴表示用クラス
	/// </summary>
	[Serializable]
	public class UserPointHistoryContainer : UserPointHistoryModel
	{
		/// <summary>ポイント発効日</summary>
		public const string POINT_CREATE_DATE = "point_date_created";
		/// <summary>ポイント注文ID</summary>
		public const string POINT_ORDER_ID = "point_order_id";
		/// <summary>ポイント定期購入ID</summary>
		public const string POINT_FIXED_PURCHASE_ID = "point_fixed_purchase_id";
		/// <summary>返品注文数</summary>
		public const string RETURN_ORDER_COUNT = "return_order_count";
		/// <summary>交換注文数</summary>
		public const string EXCHANGE_ORDER_COUNT = "exchange_order_count";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserPointHistoryContainer()
		{
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserPointHistoryContainer(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>ポイント発効日</summary>
		public DateTime PointCreateDate
		{
			get { return (DateTime)this.DataSource[POINT_CREATE_DATE]; }
			set { this.DataSource[POINT_CREATE_DATE] = value; }
		}
		/// <summary>ポイント注文ID</summary>
		public string PointOrderId
		{
			get { return StringUtility.ToEmpty(this.DataSource[POINT_ORDER_ID]); }
			set { this.DataSource[POINT_ORDER_ID] = value; }
		}
		/// <summary>ポイント定期購入ID</summary>
		public string PointFixedPurchaseId
		{
			get { return StringUtility.ToEmpty(this.DataSource[POINT_FIXED_PURCHASE_ID]); }
			set { this.DataSource[POINT_FIXED_PURCHASE_ID] = value; }
		}
		/// <summary>返品注文数</summary>
		public int ReturnOrderCount
		{
			get { return (int)this.DataSource[RETURN_ORDER_COUNT]; }
			set { this.DataSource[RETURN_ORDER_COUNT] = value; }
		}
		/// <summary>交換注文数</summary>
		public int ExchangeOrderCount
		{
			get { return (int)this.DataSource[EXCHANGE_ORDER_COUNT]; }
			set { this.DataSource[EXCHANGE_ORDER_COUNT] = value; }
		}
		/// <summary>出荷手配日時</summary>
		public DateTime? OrderShippingDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE] = value; }
		}
		/// <summary>ポイント数</summary>
		public decimal Point
		{
			get { return (decimal)this.DataSource[Constants.FIELD_USERPOINT_POINT]; }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT] = value; }
		}
		/// <summary>ポイント種別</summary>
		public string UserPointType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_USERPOINT_POINT_TYPE]); }
			set { this.DataSource[Constants.FIELD_USERPOINT_POINT_TYPE] = value; }
		}
		#endregion
	}
	#endregion
}