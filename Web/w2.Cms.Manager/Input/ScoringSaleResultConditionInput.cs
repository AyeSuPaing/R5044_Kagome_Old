/*
=========================================================================================================
  Module      : Scoring Sale Result Condition Input (ScoringSaleResultConditionInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Common.Util;
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
		this.BranchNo = Constants.CONST_SCORINGSALE_BRANCH_NO_DEFAULT;
		this.ConditionBranchNo = Constants.CONST_SCORINGSALE_CONDITION_BRANCH_NO; ;
		this.ScoreAxisAxisNo = string.Empty;
		this.ScoreAxisAxisValueFrom = string.Empty;
		this.ScoreAxisAxisValueTo = string.Empty;
		this.GroupNo = Constants.CONST_SCORINGSALE_GROUP_NO_DEFAULT;
	}
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale result condition model</param>
	public ScoringSaleResultConditionInput(ScoringSaleResultConditionModel model)
		: this()
	{
		this.ScoringSaleId = model.ScoringSaleId;
		this.BranchNo = model.BranchNo.ToString();
		this.ConditionBranchNo = model.ConditionBranchNo.ToString();
		this.ScoreAxisAxisNo = model.ScoreAxisAxisNo.ToString();
		this.ScoreAxisAxisValueFrom = model.ScoreAxisAxisValueFrom.ToString();
		this.ScoreAxisAxisValueTo = model.ScoreAxisAxisValueTo.ToString();
		this.Condition = model.Condition;
		this.GroupNo = model.GroupNo.ToString();
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
			BranchNo = int.Parse(this.BranchNo),
			ConditionBranchNo = int.Parse(this.ConditionBranchNo),
			ScoreAxisAxisNo = int.Parse(this.ScoreAxisAxisNo),
			ScoreAxisAxisValueFrom = int.Parse(this.ScoreAxisAxisValueFrom),
			ScoreAxisAxisValueTo = int.Parse(this.ScoreAxisAxisValueTo),
			Condition = this.Condition,
			GroupNo = int.Parse(this.GroupNo),
		};
		return model;
	}

	/// <summary>
	/// Validate
	/// </summary>
	/// <param name="isRegister">Is register</param>
	/// <returns>Error messages</returns>
	public List<string> Validate(bool isRegister)
	{
		var errorMessageList = Validator.Validate(isRegister
				? "ScoringSaleResultConditionRegister"
				: "ScoringSaleResultConditionModify", this.DataSource)
			.Select(keyValue => keyValue.Value)
			.ToList();
		return errorMessageList;
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
	public string BranchNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_BRANCH_NO] = value; }
	}
	/// <summary>条件枝番</summary>
	public string ConditionBranchNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION_BRANCH_NO] = value; }
	}
	/// <summary>スコア軸番号</summary>
	public string ScoreAxisAxisNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO] = value; }
	}
	/// <summary>スコア軸値(From)</summary>
	public string ScoreAxisAxisValueFrom
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_FROM]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_FROM] = value; }
	}
	/// <summary>スコア軸値(To)</summary>
	public string ScoreAxisAxisValueTo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_TO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_TO] = value; }
	}
	/// <summary>条件</summary>
	public string Condition
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION] = value; }
	}
	/// <summary>グループ番号</summary>
	public string GroupNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_GROUP_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_GROUP_NO] = value; }
	}
	#endregion
}
