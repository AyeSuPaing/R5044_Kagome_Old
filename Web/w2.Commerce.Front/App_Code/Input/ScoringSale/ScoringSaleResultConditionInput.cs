/*
=========================================================================================================
  Module      : Scoring Sale Result Condition Input (ScoringSaleResultConditionInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Domain.ScoringSale;

/// <summary>
/// Scoring sale result condition input
/// </summary>
public class ScoringSaleResultConditionInput : InputBase<ScoringSaleResultConditionModel>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoringSaleResultConditionInput()
	{
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale result condition model</param>
	public ScoringSaleResultConditionInput(ScoringSaleResultConditionModel model)
		: this()
	{
		this.ScoringSaleId = model.ScoringSaleId;
		this.BranchNo = model.BranchNo;
		this.ConditionBranchNo = model.ConditionBranchNo;
		this.ScoreAxisAxisNo = model.ScoreAxisAxisNo;
		this.ScoreAxisAxisValueFrom = model.ScoreAxisAxisValueFrom;
		this.ScoreAxisAxisValueTo = model.ScoreAxisAxisValueTo;
		this.Condition = model.Condition;
		this.GroupNo = model.GroupNo;
	}
	#endregion

	#region +Method
	/// <summary>
	/// Create model
	/// </summary>
	/// <returns>Scoring sale result condition model</returns>
	public override ScoringSaleResultConditionModel CreateModel()
	{
		var model = new ScoringSaleResultConditionModel
		{
			ScoringSaleId = this.ScoringSaleId,
			BranchNo = this.BranchNo,
			ConditionBranchNo = this.ConditionBranchNo,
			ScoreAxisAxisNo = this.ScoreAxisAxisNo,
			ScoreAxisAxisValueFrom = this.ScoreAxisAxisValueFrom,
			ScoreAxisAxisValueTo = this.ScoreAxisAxisValueTo,
			Condition = this.Condition,
			GroupNo = this.GroupNo,
		};
		return model;
	}
	#endregion

	#region +Properties
	/// <summary>スコアリング販売ID</summary>
	public string ScoringSaleId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORING_SALE_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORING_SALE_ID] = value; }
	}
	/// <summary>枝番</summary>
	public int BranchNo
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_BRANCH_NO] = value; }
	}
	/// <summary>条件枝番</summary>
	public int ConditionBranchNo
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION_BRANCH_NO] = value; }
	}
	/// <summary>スコア軸番号</summary>
	public int ScoreAxisAxisNo
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO] = value; }
	}
	/// <summary>スコア軸加算値(From)</summary>
	public int ScoreAxisAxisValueFrom
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_FROM]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_FROM] = value; }
	}
	/// <summary>スコア軸加算値(To)</summary>
	public int ScoreAxisAxisValueTo
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_TO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_TO] = value; }
	}
	/// <summary>条件</summary>
	public string Condition
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION] = value; }
	}
	/// <summary>グループ番号</summary>
	public int GroupNo
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_GROUP_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_GROUP_NO] = value; }
	}
	#endregion
}
