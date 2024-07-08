/*
=========================================================================================================
  Module      : Scoring Sale Question Page Input (ScoringSaleQuestionPageInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.ScoringSale;

/// <summary>
/// Scoring sale question page input
/// </summary>
[Serializable]
public class ScoringSaleQuestionPageInput : InputBase<ScoringSaleQuestionPageModel>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoringSaleQuestionPageInput()
	{
		this.QuestionPageItems = new ScoringSaleQuestionPageItemInput[0];
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="model">Scoring sale question page model</param>
	public ScoringSaleQuestionPageInput(ScoringSaleQuestionPageModel model)
		: this()
	{
		this.ScoringSaleId = model.ScoringSaleId;
		this.PageNo = model.PageNo.ToString();
		this.PreviousPageBtnCaption = model.PreviousPageBtnCaption;
		this.NextPageBtnCaption = model.NextPageBtnCaption;
		this.DateCreated = model.DateCreated;
		this.DateChanged = model.DateChanged;
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region +Method
	/// <summary>
	/// Create model
	/// </summary>
	/// <returns>Scoring sale question page model</returns>
	public override ScoringSaleQuestionPageModel CreateModel()
	{
		var model = new ScoringSaleQuestionPageModel
		{
			ScoringSaleId = this.ScoringSaleId,
			PageNo = int.Parse(this.PageNo),
			PreviousPageBtnCaption = this.PreviousPageBtnCaption,
			NextPageBtnCaption = this.NextPageBtnCaption,
			DateCreated = this.DateCreated,
			DateChanged = this.DateChanged,
			LastChanged = this.LastChanged,
		};
		return model;
	}
	#endregion

	#region +Properties
	/// <summary>スコアリング販売ID</summary>
	public string ScoringSaleId
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_SCORING_SALE_ID]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_SCORING_SALE_ID] = value; }
	}
	/// <summary>ページNo</summary>
	public string PageNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_PAGE_NO]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_PAGE_NO] = value; }
	}
	/// <summary>前ページボタン文言</summary>
	public string PreviousPageBtnCaption
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_PREVIOUS_PAGE_BTN_CAPTION]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_PREVIOUS_PAGE_BTN_CAPTION] = value; }
	}
	/// <summary>次ページボタン文言</summary>
	public string NextPageBtnCaption
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_NEXT_PAGE_BTN_CAPTION]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_NEXT_PAGE_BTN_CAPTION] = value; }
	}
	/// <summary>作成日</summary>
	public DateTime DateCreated
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public DateTime DateChanged
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGE_LAST_CHANGED] = value; }
	}
	/// <summary>Question page items</summary>
	public ScoringSaleQuestionPageItemInput[] QuestionPageItems { get; set; }
	#endregion
}
