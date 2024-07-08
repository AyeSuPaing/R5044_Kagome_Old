/*
=========================================================================================================
  Module      : Scoring Sale Page (ScoringSalePage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Domain;

/// <summary>
/// Scoring sale page
/// </summary>
public class ScoringSalePage : BaseUserControl
{
	/// <summary>Wrapped html generic control error session expired</summary>
	protected WrappedHtmlGenericControl WpErrorSessionExpired { get { return GetWrappedControl<WrappedHtmlGenericControl>("pErrorSessionExpired"); } }

	/// <summary>
	/// Load scoring sale
	/// </summary>
	/// <param name="scoringSaleId">Scoring sale id</param>
	protected void LoadScoringSale(string scoringSaleId)
	{
		if (this.ScoringSale != null) return;

		var scoringSale = DomainFacade.Instance.ScoringSaleService.GetScoringSale(scoringSaleId);
		if (scoringSale == null) return;

		this.ScoringSale = new ScoringSaleInput(scoringSale);
		this.ScoringSaleInputLastChangedTime = this.ScoringSale.DateChanged.ToString();
	}

	/// <summary>
	/// Get question
	/// </summary>
	/// <param name="repeater">Repeater</param>
	/// <param name="question">Question</param>
	/// <returns>Question</returns>
	public ScoringSaleQuestionPageItemInput GetQuestion(
		WrappedRepeater repeater,
		ScoringSaleQuestionPageItemInput question)
	{
		UpdateQuestionChoices(question.Question.Choices);

		var selectedBranchNoList = GetSelectedBranchNoList(repeater, question.Question.AnswerType);
		foreach (var branchNo in selectedBranchNoList)
		{
			var selectedChoice = question.Question.Choices
				.FirstOrDefault(item => (item.BranchNo == branchNo));
			if (selectedChoice != null) selectedChoice.IsChosen = true;
		}
		return question;
	}

	/// <summary>
	/// Get question pulldown
	/// </summary>
	/// <param name="repeater">Repeater</param>
	/// <param name="question">Question</param>
	/// <param name="choices">Choices</param>
	/// <returns>Question pulldown</returns>
	protected ScoringSaleQuestionPageItemInput GetQuestionPulldown(
		ScoringSaleQuestionPageItemInput question,
		ScoringSaleQuestionChoiceInput[] choices)
	{
		UpdateQuestionChoices(question.Question.Choices);

		var pulldownSelectedChoice = GetPulldownSelectedChoice(question, choices);

		var selectedChoice = question.Question.Choices
			.FirstOrDefault(item => (item.BranchNo == pulldownSelectedChoice.Value)
				&& (item.QuestionId == pulldownSelectedChoice.Key));
		if (selectedChoice != null) selectedChoice.IsChosen = true;

		return question;
	}

	/// <summary>
	/// Get pulldown selected choice
	/// </summary>
	/// <param name="question">Question</param>
	/// <param name="choices">Choices</param>
	/// <returns>Pulldown selected choice</returns>
	public KeyValuePair<string, int> GetPulldownSelectedChoice(
		ScoringSaleQuestionPageItemInput question,
		ScoringSaleQuestionChoiceInput[] choices)
	{
		var wddlChoicePullDown = GetWrappedControl<WrappedDropDownList>("ddlQuestionPullDown");
		var choicePullDown = 0;
		int.TryParse(wddlChoicePullDown.SelectedValue, out choicePullDown);
		var chosen = new KeyValuePair<string, int>(question.QuestionId, choicePullDown);

		return chosen;
	}

	/// <summary>
	/// Get selected branch no
	/// </summary>
	/// <param name="repeater">Repeater</param>
	/// <param name="answerType">Answer type</param>
	/// <returns>Branch no array</returns>
	private int[] GetSelectedBranchNoList(
		WrappedRepeater repeater,
		string answerType)
	{
		var branchNo = 0;
		var branchNoList = new List<int>();

		switch (answerType)
		{
			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_TEXT_SINGLE:
				foreach (RepeaterItem item in repeater.Items)
				{
					var wrbChoice = GetWrappedControl<WrappedRadioButtonGroup>(item, "rbgChoiceTextSingle");
					if (wrbChoice.Checked == false) continue;

					var whfBranchNo = GetWrappedControl<WrappedHiddenField>(item, "hfBranchNo");
					int.TryParse(whfBranchNo.Value, out branchNo);
					break;
				}
				return new int[] { branchNo };

			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_TEXT_MULTIPLE:
				foreach (RepeaterItem item in repeater.Items)
				{
					var wcbChoice = GetWrappedControl<WrappedCheckBox>(item, "cbChoiceTextMultiple");
					if (wcbChoice.Checked == false) continue;

					var whfBranchNo = GetWrappedControl<WrappedHiddenField>(item, "hfBranchNo");
					int.TryParse(whfBranchNo.Value, out branchNo);
					branchNoList.Add(branchNo);
				}
				return branchNoList.ToArray();

			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_SINGLE:
				foreach (RepeaterItem item in repeater.Items)
				{
					var wrbChoice = GetWrappedControl<WrappedRadioButtonGroup>(item, "rbgChoiceImageTextSingle");
					if (wrbChoice.Checked == false) continue;

					var whfSingleImageChoiceNo = GetWrappedControl<WrappedHiddenField>(item, "hfBranchNo");
					int.TryParse(whfSingleImageChoiceNo.Value, out branchNo);
					break;
				}
				return new int[] { branchNo };

			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_MULTIPLE:
				foreach (RepeaterItem item in repeater.Items)
				{
					var wcbChoice = GetWrappedControl<WrappedCheckBox>(item, "cbChoiceImageTextMultiple");
					if (wcbChoice.Checked == false) continue;

					var whfQuestionChoiceNo = GetWrappedControl<WrappedHiddenField>(item, "hfBranchNo");
					int.TryParse(whfQuestionChoiceNo.Value, out branchNo);
					branchNoList.Add(branchNo);
				}
				return branchNoList.ToArray();

			case Constants.FLG_SCORINGSALE_QUESTION_TYPE_CHART:
				foreach (RepeaterItem item in repeater.Items)
				{
					var wrbChoice = GetWrappedControl<WrappedRadioButtonGroup>(item, "rbgChoiceChart");
					if (wrbChoice.Checked == false) continue;

					var whfChartChoiceNo = GetWrappedControl<WrappedHiddenField>(item, "hfBranchNo");
					int.TryParse(whfChartChoiceNo.Value, out branchNo);
					break;
				}
				return new int[] { branchNo };

			default: return new int[0];
		}
	}

	/// <summary>
	/// Insert page view contents log
	/// </summary>
	/// <param name="contentId">Content id</param>
	public void InsertPageViewContentsLog(string contentId)
	{
		ContentsLogUtility.InsertPageViewContentsLog(
			Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_SCORINGSALE,
			contentId,
			this.IsSmartPhone);
	}

	/// <summary>
	/// Create preview key
	/// </summary>
	/// <returns>Preview key</returns>
	public Dictionary<string, string> GetParameters()
	{
		var parameters = Request.QueryString.AllKeys.ToDictionary(
			key => key,
			key => Request[key]);
		return parameters;
	}

	/// <summary>
	/// Update question choices
	/// </summary>
	/// <param name="questionChoices">Question choices</param>
	public void UpdateQuestionChoices(ScoringSaleQuestionChoiceInput[] questionChoices)
	{
		foreach (var item in questionChoices)
		{
			item.IsChosen = false;
		}
	}

	/// <summary>
	/// Show error session expired
	/// </summary>
	public void ShowErrorSessionExpired()
	{
		if (string.IsNullOrEmpty(this.ScoringSaleErrorSessionExpired) == false)
		{
			this.WpErrorSessionExpired.Visible = true;
			this.WpErrorSessionExpired.InnerText = this.ScoringSaleErrorSessionExpired;
		}
		else
		{
			this.WpErrorSessionExpired.Visible = false;
			this.WpErrorSessionExpired.InnerText = string.Empty;
		}

		this.ScoringSaleErrorSessionExpired = null;
	}

	/// <summary>
	/// Load scoring sale input last changed time
	/// </summary>
	/// <param name="scoringSaleId">Scoring sale id</param>
	public void LoadScoringSaleInputLastChangedTime(string scoringSaleId)
	{
		if (this.ScoringSaleInputLastChangedTime == null) return;
		if (this.ScoringSale == null) LoadScoringSale(scoringSaleId);

		var scoringSale = DomainFacade.Instance.ScoringSaleService.GetScoringSale(scoringSaleId);

		if (IsScoringSalePageFirst(scoringSale.TopPageUseFlg)
			&& (scoringSale.ScoringSaleId == this.ScoringSale.ScoringSaleId))
		{
			var lastWriteTimeOld = DateTime.Parse(this.ScoringSaleInputLastChangedTime);
			var lastWriteTimeNew = scoringSale.DateChanged;

			var difference = lastWriteTimeNew - lastWriteTimeOld;
			if (difference >= TimeSpan.FromSeconds(1))
			{
				this.ScoringSale = null;
				this.ScoringSaleInputLastChangedTime = scoringSale.DateChanged.ToString();
			}
		}
	}

	/// <summary>
	/// Is scoring sale page first
	/// </summary>
	/// <param name="topPageUseFlg">Top page use flg</param>
	/// <returns>Is scoring sale page first</returns>
	public bool IsScoringSalePageFirst(string topPageUseFlg)
	{
		var isScoringSalePageFirst = (topPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLAG_ON)
			? (this.Request.Url.Segments.Last() == Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE_NAME)
			: ((this.PageNumber == 1)
				&& (this.Request.Url.Segments.Last() == Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE_NAME));

		return isScoringSalePageFirst;
	}

	#region +Properties
	/// <summary>Scoring sale</summary>
	protected ScoringSaleInput ScoringSale
	{
		get { return (ScoringSaleInput)Session["ScoringSaleInput"]; }
		set { Session["ScoringSaleInput"] = value; }
	}
	/// <summary>Whether it is a smartphone</summary>
	public bool IsSmartPhone
	{
		get
		{
			var result = (Constants.SMARTPHONE_OPTION_ENABLED
				&& SmartPhoneUtility.CheckSmartPhone(Request.UserAgent));
			return result;
		}
	}
	/// <summary>Is scoring sale preview</summary>
	public bool IsScoringSalePreview
	{
		get
		{
			return this.Request.RawUrl.Contains(Constants.REQUEST_KEY_PREVIEW_KEY);
		}
	}
	/// <summary>Design type</summary>
	public string DesignType
	{
		get
		{
			var result = SmartPhoneUtility.CheckSmartPhoneSite(Request.Path)
				? Constants.SCORINGSALE_DESIGN_TYPE_SP
				: Constants.SCORINGSALE_DESIGN_TYPE_PC;
			return result;
		}
	}
	/// <summary>Scoring sale input last changed time</summary>
	public string ScoringSaleInputLastChangedTime
	{
		get { return (string)this.Session["ScoringSaleInputLastChangedTime"]; }
		set { this.Session["ScoringSaleInputLastChangedTime"] = value; }
	}
	/// <summary>Scoring sale error session expired</summary>
	public string ScoringSaleErrorSessionExpired
	{
		get { return (string)this.Session["ScoringSaleErrorSessionExpired"]; }
		set { this.Session["ScoringSaleErrorSessionExpired"] = value; }
	}
	/// <summary>Page no</summary>
	public string PageNo
	{
		get { return Request[Constants.REQUEST_KEY_PAGE_NO]; }
	}
	/// <summary>Page number</summary>
	public int PageNumber
	{
		get
		{
			var pageNumber = 1;
			if (int.TryParse(this.PageNo, out pageNumber) == false) return 1;
			return pageNumber;
		}
	}
	#endregion
}
