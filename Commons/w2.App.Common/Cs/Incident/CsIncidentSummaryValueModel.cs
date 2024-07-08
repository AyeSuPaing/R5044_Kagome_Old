/*
=========================================================================================================
  Module      : インシデント集計区分値モデル(CsIncidentSummaryValueModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Incident
{
	/// <summary>
	/// インシデント集計区分値モデル
	/// </summary>
	[Serializable]
	public partial class CsIncidentSummaryValueModel : ModelBase<CsIncidentSummaryValueModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsIncidentSummaryValueModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">値</param>
		public CsIncidentSummaryValueModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">値</param>
		public CsIncidentSummaryValueModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_DEPT_ID] = value; }
		}
		/// <summary>インシデントID</summary>
		public string IncidentId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_INCIDENT_ID]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_INCIDENT_ID] = value; }
		}
		/// <summary>集計区分番号</summary>
		public int SummaryNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_SUMMARY_NO]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_SUMMARY_NO] = value; }
		}
		/// <summary>集計区分値</summary>
		public string Value
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_VALUE]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_VALUE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSINCIDENTSUMMARYVALUE_LAST_CHANGED] = value; }
		}
		#endregion

	}
}
