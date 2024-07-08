/*
=========================================================================================================
  Module      : Scoring Sale Question(ScoringSaleQuestion.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;

/// <summary>
/// Scoring sale question
/// </summary>
public partial class Form_Common_ScoringSale_ScoringSaleQuestion : ScoringSalePage
{
	/// <summary>
	/// Page Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			switch (this.QuestionPageItem.Question.AnswerType)
			{
				case Constants.FLG_SCORINGSALE_QUESTION_TYPE_TEXT_SINGLE:
					ucQuestionTextSingle.Question = this.QuestionPageItem;
					break;

				case Constants.FLG_SCORINGSALE_QUESTION_TYPE_TEXT_MULTIPLE:
					ucQuestionTextMultiple.Question = this.QuestionPageItem;
					break;

				case Constants.FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_SINGLE:
					ucQuestionImageTextSingle.Question = this.QuestionPageItem;
					break;

				case Constants.FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_MULTIPLE:
					ucQuestionImageTextMultiple.Question = this.QuestionPageItem;
					break;

				case Constants.FLG_SCORINGSALE_QUESTION_TYPE_PULLDOWN:
					ucQuestionPullDown.Question = this.QuestionPageItem;
					break;

				case Constants.FLG_SCORINGSALE_QUESTION_TYPE_CHART:
					ucQuestionChart.Question = this.QuestionPageItem;
					break;
			}
		}
	}

	/// <summary>
	/// Get question
	/// </summary>
	/// <returns>Question info</returns>
	public ScoringSaleQuestionPageItemInput GetQuestion()
	{
		switch (this.QuestionPageItem.Question.AnswerType)
		{
			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_TEXT_SINGLE:
				return ucQuestionTextSingle.GetQuestion();

			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_TEXT_MULTIPLE:
				return ucQuestionTextMultiple.GetQuestion();

			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_SINGLE:
				return ucQuestionImageTextSingle.GetQuestion();

			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_MULTIPLE:
				return ucQuestionImageTextMultiple.GetQuestion();

			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_PULLDOWN:
				return ucQuestionPullDown.GetQuestion();

			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_CHART:
				return ucQuestionChart.GetQuestion();

			default: return null;
		}
	}

	#region +Properties
	/// <summary>Branch no</summary>
	public int BranchNo
	{
		get { return (int)ViewState["ScoringSaleQuestion_BranchNo"]; }
		set { ViewState["ScoringSaleQuestion_BranchNo"] = value; }
	}
	/// <summary>Question page item</summary>
	protected ScoringSaleQuestionPageItemInput QuestionPageItem
	{
		get
		{
			var result = this.ScoringSale.CurrentQuestionPage.QuestionPageItems.FirstOrDefault(item => (item.BranchNo == this.BranchNo));
			return result;
		}
	}
	#endregion
}
