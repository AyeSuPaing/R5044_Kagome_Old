/*
=========================================================================================================
  Module      : Scoring Sale Question Choice Input (ScoringSaleQuestionChoiceInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.ScoringSale;

/// <summary>
/// Scoring sale question choice input
/// </summary>
[Serializable]
public class ScoringSaleQuestionChoiceInput : InputBase<ScoringSaleQuestionChoiceModel>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoringSaleQuestionChoiceInput()
	{
		this.IsChosen = false;
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale question choice model</param>
	public ScoringSaleQuestionChoiceInput(ScoringSaleQuestionChoiceModel model)
		: this()
	{
		this.QuestionId = model.QuestionId;
		this.BranchNo = model.BranchNo;
		this.QuestionChoiceStatement = model.QuestionChoiceStatement;
		this.QuestionChoiceStatementImgPath = model.QuestionChoiceStatementImgPath;
		this.AxisAdditional1 = model.AxisAdditional1;
		this.AxisAdditional2 = model.AxisAdditional2;
		this.AxisAdditional3 = model.AxisAdditional3;
		this.AxisAdditional4 = model.AxisAdditional4;
		this.AxisAdditional5 = model.AxisAdditional5;
		this.AxisAdditional6 = model.AxisAdditional6;
		this.AxisAdditional7 = model.AxisAdditional7;
		this.AxisAdditional8 = model.AxisAdditional8;
		this.AxisAdditional9 = model.AxisAdditional9;
		this.AxisAdditional10 = model.AxisAdditional10;
		this.AxisAdditional11 = model.AxisAdditional11;
		this.AxisAdditional12 = model.AxisAdditional12;
		this.AxisAdditional13 = model.AxisAdditional13;
		this.AxisAdditional14 = model.AxisAdditional14;
		this.AxisAdditional15 = model.AxisAdditional15;
	}
	#endregion

	#region +Method
	/// <summary>
	/// Create model
	/// </summary>
	/// <returns>Scoring sale question choice model</returns>
	public override ScoringSaleQuestionChoiceModel CreateModel()
	{
		var model = new ScoringSaleQuestionChoiceModel
		{
			QuestionId = this.QuestionId,
			BranchNo = this.BranchNo,
			QuestionChoiceStatement = this.QuestionChoiceStatement,
			QuestionChoiceStatementImgPath = this.QuestionChoiceStatementImgPath,
			AxisAdditional1 = this.AxisAdditional1,
			AxisAdditional2 = this.AxisAdditional2,
			AxisAdditional3 = this.AxisAdditional3,
			AxisAdditional4 = this.AxisAdditional4,
			AxisAdditional5 = this.AxisAdditional5,
			AxisAdditional6 = this.AxisAdditional6,
			AxisAdditional7 = this.AxisAdditional7,
			AxisAdditional8 = this.AxisAdditional8,
			AxisAdditional9 = this.AxisAdditional9,
			AxisAdditional10 = this.AxisAdditional10,
			AxisAdditional11 = this.AxisAdditional11,
			AxisAdditional12 = this.AxisAdditional12,
			AxisAdditional13 = this.AxisAdditional13,
			AxisAdditional14 = this.AxisAdditional14,
			AxisAdditional15 = this.AxisAdditional15,
		};
		return model;
	}
	#endregion

	#region +Properties
	/// <summary>質問ID</summary>
	public string QuestionId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_ID] = value; }
	}
	/// <summary>枝番</summary>
	public int BranchNo
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_BRANCH_NO] = value; }
	}
	/// <summary>選択肢文</summary>
	public string QuestionChoiceStatement
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT] = value; }
	}
	/// <summary>選択肢画像</summary>
	public string QuestionChoiceStatementImgPath
	{
		get
		{
			var result = (this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH] != DBNull.Value)
				? (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH]
				: null;
			return result;
		}
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH] = value; }
	}
	/// <summary>軸加算値１</summary>
	public int AxisAdditional1
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL1]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL1] = value; }
	}
	/// <summary>軸加算値２</summary>
	public int AxisAdditional2
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL2]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL2] = value; }
	}
	/// <summary>軸加算値３</summary>
	public int AxisAdditional3
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL3]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL3] = value; }
	}
	/// <summary>軸加算値４</summary>
	public int AxisAdditional4
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL4]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL4] = value; }
	}
	/// <summary>軸加算値５</summary>
	public int AxisAdditional5
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL5]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL5] = value; }
	}
	/// <summary>軸加算値６</summary>
	public int AxisAdditional6
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL6]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL6] = value; }
	}
	/// <summary>軸加算値７</summary>
	public int AxisAdditional7
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL7]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL7] = value; }
	}
	/// <summary>軸加算値８</summary>
	public int AxisAdditional8
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL8]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL8] = value; }
	}
	/// <summary>軸加算値９</summary>
	public int AxisAdditional9
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL9]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL9] = value; }
	}
	/// <summary>軸加算値１０</summary>
	public int AxisAdditional10
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL10]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL10] = value; }
	}
	/// <summary>軸加算値１１</summary>
	public int AxisAdditional11
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL11]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL11] = value; }
	}
	/// <summary>軸加算値１２</summary>
	public int AxisAdditional12
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL12]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL12] = value; }
	}
	/// <summary>軸加算値１３</summary>
	public int AxisAdditional13
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL13]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL13] = value; }
	}
	/// <summary>軸加算値１４</summary>
	public int AxisAdditional14
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL14]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL14] = value; }
	}
	/// <summary>軸加算値１５</summary>
	public int AxisAdditional15
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL15]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL15] = value; }
	}
	/// <summary>Is chosen</summary>
	public bool IsChosen { get; set; }
	#endregion
}
