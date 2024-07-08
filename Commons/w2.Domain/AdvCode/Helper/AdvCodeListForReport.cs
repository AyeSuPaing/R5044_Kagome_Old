/*
=========================================================================================================
  Module      : Adv code list for report (AdvCodeListForReport.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Domain.AdvCode.Helper
{
	/// <summary>
	/// Adv code list for report
	/// </summary>
	[Serializable]
	public class AdvCodeListForReport : AdvCodeModel
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AdvCodeListForReport()
		{
			this.AdvcodeMediaTypeName = string.Empty;
			this.TotalOrderAmount = 0m;
			this.TotalOrderCount = 0;
			this.OrderAmountRate = 0m;
			this.OrderCountRate = 0m;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AdvCodeListForReport(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AdvCodeListForReport(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>媒体区分名</summary>
		public string AdvcodeMediaTypeName
		{
			get
			{
				return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME]);
			}
			set { this.DataSource[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME] = value; }
		}
		/// <summary>Total order amount</summary>
		public decimal TotalOrderAmount
		{
			get { return (decimal)StringUtility.ToValue(this.DataSource["total_order_amount"], 0m); }
			set { this.DataSource["total_order_amount"] = value; }
		}
		/// <summary>Total order count</summary>
		public int TotalOrderCount
		{
			get { return (int)StringUtility.ToValue(this.DataSource["total_order_count"], 0); }
			set { this.DataSource["total_order_count"] = value; }
		}
		/// <summary>Order amount rate</summary>
		public decimal OrderAmountRate
		{
			get { return (decimal)StringUtility.ToValue(this.DataSource["order_amount_rate"], 0m); }
			set { this.DataSource["order_amount_rate"] = value; }
		}
		/// <summary>Order count rate</summary>
		public decimal OrderCountRate
		{
			get { return (decimal)StringUtility.ToValue(this.DataSource["order_count_rate"], 0m); }
			set { this.DataSource["order_count_rate"] = value; }
		}
		#endregion
	}
}