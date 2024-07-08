/*
=========================================================================================================
  Module      : Sale Goal Model (SaleGoalModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.SaleGoal
{
	/// <summary>
	/// Sale goal model
	/// </summary>
	[Serializable]
	public partial class SaleGoalModel : ModelBase<SaleGoalModel>
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public SaleGoalModel()
		{
			this.AnnualGoal = 0m;
			this.ApplicableMonth = 1;
			this.MonthlyGoal1 = 0m;
			this.MonthlyGoal2 = 0m;
			this.MonthlyGoal3 = 0m;
			this.MonthlyGoal4 = 0m;
			this.MonthlyGoal5 = 0m;
			this.MonthlyGoal6 = 0m;
			this.MonthlyGoal7 = 0m;
			this.MonthlyGoal8 = 0m;
			this.MonthlyGoal9 = 0m;
			this.MonthlyGoal10 = 0m;
			this.MonthlyGoal11 = 0m;
			this.MonthlyGoal12 = 0m;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Source</param>
		public SaleGoalModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Source</param>
		public SaleGoalModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region +Properties
		/// <summary>Year</summary>
		public string Year
		{
			get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_YEAR]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_YEAR] = value; }
		}
		/// <summary>Annual goal</summary>
		public decimal AnnualGoal
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_ANNUAL_GOAL]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_ANNUAL_GOAL] = value; }
		}
		/// <summary>Applicable month</summary>
		public int ApplicableMonth
		{
			get { return (int)this.DataSource[Constants.FIELD_SALEGOAL_APPLICABLE_MONTH]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_APPLICABLE_MONTH] = value; }
		}
		/// <summary>Monthly goal 1</summary>
		public decimal MonthlyGoal1
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_1]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_1] = value; }
		}
		/// <summary>Monthly goal 2</summary>
		public decimal MonthlyGoal2
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_2]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_2] = value; }
		}
		/// <summary>Monthly goal 3</summary>
		public decimal MonthlyGoal3
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_3]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_3] = value; }
		}
		/// <summary>Monthly goal 4</summary>
		public decimal MonthlyGoal4
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_4]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_4] = value; }
		}
		/// <summary>Monthly goal 5</summary>
		public decimal MonthlyGoal5
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_5]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_5] = value; }
		}
		/// <summary>Monthly goal 6</summary>
		public decimal MonthlyGoal6
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_6]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_6] = value; }
		}
		/// <summary>Monthly goal 7</summary>
		public decimal MonthlyGoal7
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_7]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_7] = value; }
		}
		/// <summary>Monthly goal 8</summary>
		public decimal MonthlyGoal8
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_8]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_8] = value; }
		}
		/// <summary>Monthly goal 9</summary>
		public decimal MonthlyGoal9
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_9]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_9] = value; }
		}
		/// <summary>Monthly goal 10</summary>
		public decimal MonthlyGoal10
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_10]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_10] = value; }
		}
		/// <summary>Monthly goal 11</summary>
		public decimal MonthlyGoal11
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_11]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_11] = value; }
		}
		/// <summary>Monthly goal 12</summary>
		public decimal MonthlyGoal12
		{
			get { return (decimal)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_12]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_12] = value; }
		}
		/// <summary>Last changed</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SALEGOAL_LAST_CHANGED] = value; }
		}
		#endregion
	}
}