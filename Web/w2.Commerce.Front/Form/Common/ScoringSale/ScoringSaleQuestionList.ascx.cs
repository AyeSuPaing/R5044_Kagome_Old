/*
=========================================================================================================
  Module      : Scoring Sale Question List (ScoringSaleQuestionList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// Scoring sale question list
/// </summary>
public partial class Form_Common_ScoringSale_ScoringSaleQuestionList : ScoringSalePage
{
	/// <summary>Wrapped repeater question</summary>
	protected WrappedRepeater WrQuestion { get { return GetWrappedControl<WrappedRepeater>("rQuestion"); } }

	/// <summary>First page value</summary>
	private const string CONST_SCORINGSALE_FIRST_PAGE = "1";

	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			DisplayQuestion();
		}
	}

	/// <summary>
	/// Display question
	/// </summary>
	private void DisplayQuestion()
	{
		this.WrQuestion.DataSource = this.QuestionPageItems;
		this.WrQuestion.DataBind();
	}

	/// <summary>
	/// Get questions
	/// </summary>
	/// <returns>Question info</returns>
	public ScoringSaleQuestionPageItemInput[] GetQuestions()
	{
		var questions = new List<ScoringSaleQuestionPageItemInput>();
		foreach (RepeaterItem item in this.WrQuestion.Items)
		{
			var uc = (Form_Common_ScoringSale_ScoringSaleQuestion)item.FindControl("ucScoringSaleQuestion");
			questions.Add(uc.GetQuestion());
		}

		return questions.ToArray();
	}

	#region +Properties
	/// <summary>Question page items</summary>
	public ScoringSaleQuestionPageItemInput[] QuestionPageItems
	{
		get { return (ScoringSaleQuestionPageItemInput[])ViewState["ScoringSaleQuestionList_QuestionPageItems"]; }
		set { ViewState["ScoringSaleQuestionList_QuestionPageItems"] = value; }
	}
	#endregion
}
