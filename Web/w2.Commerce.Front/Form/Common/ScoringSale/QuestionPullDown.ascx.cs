/*
=========================================================================================================
  Module      : Question Pull Down (QuestionPullDown.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.UI.WebControls;

/// <summary>
/// Question pull down
/// </summary>
public partial class Form_Common_ScoringSale_QuestionPullDown : ScoringSalePage
{
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.IsPostBack)
		{
			if (this.Question == null) return;

			this.Choices = this.Question.Question.Choices;
			this.DataBind();
		}
	}

	/// <summary>
	/// Get question
	/// </summary>
	/// <returns>Question</returns>
	public ScoringSaleQuestionPageItemInput GetQuestion()
	{
		return GetQuestionPulldown(this.Question, this.Choices);
	}

	/// <summary>
	/// Get selected value
	/// </summary>
	/// <returns>Array list</returns>
	protected ListItem[] GetQuestionItems()
	{
		if (this.Choices == null) return null;

		if ((this.Question != null)
			&& this.Question.Question.Choices.Length != 0)
		{
			var selectedChoice = this.Question.Question.Choices
				.FirstOrDefault(choice => (choice.IsChosen
					&& (choice.QuestionId == this.Question.QuestionId)));
			if (selectedChoice != null) this.BrandNo = selectedChoice.BranchNo.ToString();
		}

		var items = this.Choices
			.Select(item =>
				new ListItem(item.QuestionChoiceStatement, item.BranchNo.ToString()))
			.ToArray();
		return items;
	}

	#region +Properties
	/// <summary>Question info</summary>
	public ScoringSaleQuestionPageItemInput Question
	{
		get { return (ScoringSaleQuestionPageItemInput)ViewState["QuestionPullDown_Question"]; }
		set { ViewState["QuestionPullDown_Question"] = value; }
	}
	/// <summary>Choices</summary>
	private ScoringSaleQuestionChoiceInput[] Choices
	{
		get { return (ScoringSaleQuestionChoiceInput[])ViewState["QuestionPullDown_Choices"]; }
		set { ViewState["QuestionPullDown_Choices"] = value; }
	}
	/// <summary>BrandNo</summary>
	public string BrandNo
	{
		get { return (string)ViewState["BrandNo"]; }
		set { ViewState["BrandNo"] = value; }
	}
	#endregion
}
