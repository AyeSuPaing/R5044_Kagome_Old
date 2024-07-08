/*
=========================================================================================================
  Module      : Sale Goal Input (SaleGoalInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Domain.SaleGoal;

/// <summary>
/// Sale goal input
/// </summary>
public class SaleGoalInput : InputBase<SaleGoalModel>
{
	#region +Constructor
	/// <summary>
	/// Constructor
	/// </summary>
	public SaleGoalInput()
	{
	}
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Sale goal model</param>
	public SaleGoalInput(SaleGoalModel model)
		: this()
	{
		this.Year = model.Year;
		this.AnnualGoal = model.AnnualGoal.ToString();
		this.ApplicableMonth = model.ApplicableMonth.ToString();
		this.MonthlyGoal1 = model.MonthlyGoal1.ToString();
		this.MonthlyGoal2 = model.MonthlyGoal2.ToString();
		this.MonthlyGoal3 = model.MonthlyGoal3.ToString();
		this.MonthlyGoal4 = model.MonthlyGoal4.ToString();
		this.MonthlyGoal5 = model.MonthlyGoal5.ToString();
		this.MonthlyGoal6 = model.MonthlyGoal6.ToString();
		this.MonthlyGoal7 = model.MonthlyGoal7.ToString();
		this.MonthlyGoal8 = model.MonthlyGoal8.ToString();
		this.MonthlyGoal9 = model.MonthlyGoal9.ToString();
		this.MonthlyGoal10 = model.MonthlyGoal10.ToString();
		this.MonthlyGoal11 = model.MonthlyGoal11.ToString();
		this.MonthlyGoal12 = model.MonthlyGoal12.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region +Method
	/// <summary>
	/// Create model
	/// </summary>
	/// <returns>モデル</returns>
	public override SaleGoalModel CreateModel()
	{
		var model = new SaleGoalModel
		{
			Year = this.Year,
			AnnualGoal = decimal.Parse(this.AnnualGoal),
			ApplicableMonth = int.Parse(this.ApplicableMonth),
			MonthlyGoal1 = decimal.Parse(this.MonthlyGoal1),
			MonthlyGoal2 = decimal.Parse(this.MonthlyGoal2),
			MonthlyGoal3 = decimal.Parse(this.MonthlyGoal3),
			MonthlyGoal4 = decimal.Parse(this.MonthlyGoal4),
			MonthlyGoal5 = decimal.Parse(this.MonthlyGoal5),
			MonthlyGoal6 = decimal.Parse(this.MonthlyGoal6),
			MonthlyGoal7 = decimal.Parse(this.MonthlyGoal7),
			MonthlyGoal8 = decimal.Parse(this.MonthlyGoal8),
			MonthlyGoal9 = decimal.Parse(this.MonthlyGoal9),
			MonthlyGoal10 = decimal.Parse(this.MonthlyGoal10),
			MonthlyGoal11 = decimal.Parse(this.MonthlyGoal11),
			MonthlyGoal12 = decimal.Parse(this.MonthlyGoal12),
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// Check validate
	/// </summary>
	/// <returns>Error message</returns>
	public string Validate()
	{
		var errorMessage = Validator.Validate("SaleGoal", this.DataSource);
		return errorMessage;
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
	public string AnnualGoal
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_ANNUAL_GOAL]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_ANNUAL_GOAL] = value; }
	}
	/// <summary>Applicable month</summary>
	public string ApplicableMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_APPLICABLE_MONTH]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_APPLICABLE_MONTH] = value; }
	}
	/// <summary>Monthly goal 1</summary>
	public string MonthlyGoal1
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_1]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_1] = value; }
	}
	/// <summary>Monthly goal 2</summary>
	public string MonthlyGoal2
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_2]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_2] = value; }
	}
	/// <summary>Monthly goal 3</summary>
	public string MonthlyGoal3
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_3]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_3] = value; }
	}
	/// <summary>Monthly goal 4</summary>
	public string MonthlyGoal4
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_4]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_4] = value; }
	}
	/// <summary>Monthly goal 5</summary>
	public string MonthlyGoal5
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_5]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_5] = value; }
	}
	/// <summary>Monthly goal 6</summary>
	public string MonthlyGoal6
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_6]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_6] = value; }
	}
	/// <summary>Monthly goal 7</summary>
	public string MonthlyGoal7
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_7]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_7] = value; }
	}
	/// <summary>Monthly goal 8</summary>
	public string MonthlyGoal8
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_8]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_8] = value; }
	}
	/// <summary>Monthly goal 9</summary>
	public string MonthlyGoal9
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_9]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_9] = value; }
	}
	/// <summary>Monthly goal 10</summary>
	public string MonthlyGoal10
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_10]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_10] = value; }
	}
	/// <summary>Monthly goal 11</summary>
	public string MonthlyGoal11
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_11]; }
		set { this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_11] = value; }
	}
	/// <summary>Monthly goal 12</summary>
	public string MonthlyGoal12
	{
		get { return (string)this.DataSource[Constants.FIELD_SALEGOAL_MONTHLY_GOAL_12]; }
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