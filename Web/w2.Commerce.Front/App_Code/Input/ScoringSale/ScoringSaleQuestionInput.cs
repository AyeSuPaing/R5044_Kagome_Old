/*
=========================================================================================================
  Module      : Scoring Sale Question Input (ScoringSaleQuestionInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.ScoringSale;

/// <summary>
/// Scoring sale question input
/// </summary>
[Serializable]
public class ScoringSaleQuestionInput : InputBase<ScoringSaleQuestionModel>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoringSaleQuestionInput()
	{
		this.Choices = new ScoringSaleQuestionChoiceInput[0];
		this.Score = new ScoreAxisInput();
		this.QuestionNo = 0;
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale question model</param>
	public ScoringSaleQuestionInput(ScoringSaleQuestionModel model)
		: this()
	{
		this.QuestionId = model.QuestionId;
		this.QuestionTitle = model.QuestionTitle;
		this.ScoreAxisId = model.ScoreAxisId;
		this.AnswerType = model.AnswerType;
		this.QuestionStatement = model.QuestionStatement;
		this.DateCreated = model.DateCreated;
		this.DateChanged = model.DateChanged;
		this.LastChanged = model.LastChanged;
		this.Choices = model.ScoringSaleQuestionChoiceList
			.Select(item => new ScoringSaleQuestionChoiceInput(item))
			.ToArray();
	}
	#endregion

	#region +Method
	/// <summary>
	/// Create model
	/// </summary>
	/// <returns>モデル</returns>
	public override ScoringSaleQuestionModel CreateModel()
	{
		var model = new ScoringSaleQuestionModel
		{
			QuestionId = this.QuestionId,
			QuestionTitle = this.QuestionTitle,
			ScoreAxisId = this.ScoreAxisId,
			AnswerType = this.AnswerType,
			QuestionStatement = this.QuestionStatement,
			DateCreated = this.DateCreated,
			DateChanged = this.DateChanged,
			LastChanged = this.LastChanged,
		};
		return model;
	}
	#endregion

	#region +Properties
	/// <summary>質問ID</summary>
	public string QuestionId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID] = value; }
	}
	/// <summary>質問タイトル</summary>
	public string QuestionTitle
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_TITLE]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_TITLE] = value; }
	}
	/// <summary>スコア軸ID</summary>
	public string ScoreAxisId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_ID] = value; }
	}
	/// <summary>回答タイプ</summary>
	public string AnswerType
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_ANSWER_TYPE]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_ANSWER_TYPE] = value; }
	}
	/// <summary>質問文</summary>
	public string QuestionStatement
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_STATEMENT]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_STATEMENT] = value; }
	}
	/// <summary>作成日</summary>
	public DateTime DateCreated
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public DateTime DateChanged
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_LAST_CHANGED] = value; }
	}
	/// <summary>Choices</summary>
	public ScoringSaleQuestionChoiceInput[] Choices { get; set; }
	/// <summary>Score</summary>
	public ScoreAxisInput Score { get; set; }
	/// <summary>Question no</summary>
	public int QuestionNo { get; set; }
	#endregion
}
