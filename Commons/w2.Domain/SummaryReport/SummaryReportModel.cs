/*
=========================================================================================================
  Module      : Summary Report Model (SummaryReportModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.SummaryReport
{
	/// <summary>
	/// Summary report model
	/// </summary>
	[Serializable]
	public partial class SummaryReportModel : ModelBase<SummaryReportModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SummaryReportModel()
		{
			this.PeriodKbn = string.Empty;
			this.DataKbn = string.Empty;
			this.Data = 0;
			this.DateCreated = this.ReportDate = DateTime.Now;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SummaryReportModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SummaryReportModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>Period Kbn</summary>
		public string PeriodKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SUMMARYREPORT_PERIOD_KBN]; }
			set { this.DataSource[Constants.FIELD_SUMMARYREPORT_PERIOD_KBN] = value; }
		}
		/// <summary>Data Kbn</summary>
		public string DataKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SUMMARYREPORT_DATA_KBN]; }
			set { this.DataSource[Constants.FIELD_SUMMARYREPORT_DATA_KBN] = value; }
		}
		/// <summary>Report Date</summary>
		public DateTime ReportDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SUMMARYREPORT_REPORT_DATE]; }
			set { this.DataSource[Constants.FIELD_SUMMARYREPORT_REPORT_DATE] = value; }
		}
		/// <summary>Data</summary>
		public decimal Data
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SUMMARYREPORT_DATA]; }
			set { this.DataSource[Constants.FIELD_SUMMARYREPORT_DATA] = value; }
		}
		/// <summary>Date Created</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SUMMARYREPORT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SUMMARYREPORT_DATE_CREATED] = value; }
		}
		#endregion
	}
}