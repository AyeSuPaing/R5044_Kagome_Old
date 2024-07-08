/*
=========================================================================================================
  Module      : Scoring Sale Question Page Item Input (ScoringSaleQuestionPageItemInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.ScoringSale;

/// <summary>
/// Scoring sale question page item input
/// </summary>
[Serializable]
public class ScoringSaleQuestionPageItemInput : InputBase<ScoringSaleQuestionPageItemModel>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoringSaleQuestionPageItemInput()
	{
		this.Question = new ScoringSaleQuestionInput();
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale question page item model</param>
	public ScoringSaleQuestionPageItemInput(ScoringSaleQuestionPageItemModel model)
		: this()
	{
		this.ScoringSaleId = model.ScoringSaleId;
		this.PageNo = model.PageNo.ToString();
		this.BranchNo = model.BranchNo;
		this.QuestionId = model.QuestionId;
		this.Question = new ScoringSaleQuestionInput(model.Question);
	}
	#endregion

	#region +Method
	/// <summary>
	/// Create model
	/// </summary>
	/// <returns>Scoring sale question page item model</returns>
	public override ScoringSaleQuestionPageItemModel CreateModel()
	{
		var model = new ScoringSaleQuestionPageItemModel
		{
			ScoringSaleId = this.ScoringSaleId,
			PageNo = int.Parse(this.PageNo),
			BranchNo = this.BranchNo,
			QuestionId =  this.QuestionId,
		};
		return model;
	}
	#endregion

	#region +Properties
	/// <summary>質問ID</summary>
	public string QuestionId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION_ID] = value; }
	}
	/// <summary>スコアリング販売ID</summary>
	public string ScoringSaleId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID] = value; }
	}
	/// <summary>ページNo</summary>
	public string PageNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_PAGE_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_PAGE_NO] = value; }
	}
	/// <summary>枝番</summary>
	public int BranchNo
	{
		get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_BRANCH_NO] = value; }
	}
	/// <summary>Question</summary>
	public ScoringSaleQuestionInput Question { get; set; }
	#endregion
}
