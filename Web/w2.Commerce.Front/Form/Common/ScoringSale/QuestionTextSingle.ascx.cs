﻿/*
=========================================================================================================
  Module      : Question Text Single (QuestionTextSingle.ascx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// Question text single
/// </summary>
public partial class Form_Common_ScoringSale_QuestionTextSingle : ScoringSalePage
{
	/// <summary>Wrapped repeater scoring sale question choice</summary>
	protected WrappedRepeater WrScoringSaleQuestionChoice { get { return GetWrappedControl<WrappedRepeater>("rScoringSaleQuestionChoice"); } }

	/// <summary>
	/// Page Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (this.Question != null)
			{
				this.WrScoringSaleQuestionChoice.DataSource = this.Question.Question.Choices;
				this.WrScoringSaleQuestionChoice.DataBind();
			}
		}
	}

	/// <summary>
	/// Get question
	/// </summary>
	/// <returns>Question</returns>
	public ScoringSaleQuestionPageItemInput GetQuestion()
	{
		return GetQuestion(this.WrScoringSaleQuestionChoice, this.Question);
	}

	#region +Properties
	/// <summary>Question info</summary>
	public ScoringSaleQuestionPageItemInput Question
	{
		get { return (ScoringSaleQuestionPageItemInput)ViewState["QuestionTextSingle_Question"]; }
		set { ViewState["QuestionTextSingle_Question"] = value; }
	}
	#endregion
}
