/*
=========================================================================================================
  Module      : Scoring Sale Question Page (ScoringSaleQuestionPage.ascx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Linq;
using w2.App.Common.ScoringSale;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain;

/// <summary>
/// Scoring sale question page
/// </summary>
public partial class Form_Common_ScoringSale_ScoringSaleQuestionPage : ScoringSalePage
{
	/// <summary>Wrapped link button back</summary>
	protected WrappedLinkButton WlbBack { get { return GetWrappedControl<WrappedLinkButton>("lbBack"); } }
	/// <summary>Wrapped link button next</summary>
	protected WrappedLinkButton WlbNext { get { return GetWrappedControl<WrappedLinkButton>("lbNext"); } }
	/// <summary>Wrapped link button page no</summary>
	protected WrappedLiteral WlPageNo { get { return GetWrappedControl<WrappedLiteral>("lPageNo"); } }

	/// <summary>Scoring sale first page</summary>
	private const string SCORINGSALE_FIRST_PAGE = "1";

	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			ShowErrorSessionExpired();
			CheckScoringSale();

			// Load pages
			LoadPages();
			LoadCurrentPageItems();
			SetCssForNextButton();
			ucScoringSaleQuestionList.QuestionPageItems = this.CurrentPage.QuestionPageItems;

			this.WlbBack.InnerControl.Visible =
				(((this.PageNumber == 1)
						&& this.ScoringSale.IsUseTopPage)
					|| (this.PageNumber > 1));
			this.WlPageNo.Text = string.Format(
				"{0}<span class='slash'>/</span>{1}",
				this.ScoringSale.CurrentQuestionPage.PageNo,
				this.ScoringSale.QuestionPages.Length);
			this.WlbNext.InnerControl.Text = this.CurrentPage.NextPageBtnCaption;
			this.WlbBack.InnerControl.Text = this.CurrentPage.PreviousPageBtnCaption;

			if (this.IsScoringSalePreview == false)
			{
				// Content log output
				var contentId = string.Format(
					"{0}_{1}",
					this.ScoringSale.ScoringSaleId,
					this.ScoringSale.IsUseTopPage
						? this.PageNumber + 1
						: this.PageNumber);

				InsertPageViewContentsLog(contentId);
			}
		}
	}

	/// <summary>
	/// Link button back click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		GoToPageSessionExpired();
		if (this.PageNumber > 1) GoToQuestionPage(this.PageNumber - 1);

		var parameters = GetParameters();

		if (this.IsScoringSalePreview)
		{
			var smartPhone = (this.DesignType == Constants.SCORINGSALE_DESIGN_TYPE_SP)
				? "SmartPhone/"
				: string.Empty;
			var topPreviewUrl = string.Format(
				"{0}{1}{2}",
				Constants.PATH_ROOT,
				smartPhone,
				Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE);

			var previewUrl = new UrlCreator(topPreviewUrl)
				.AddParam(Constants.REQUEST_KEY_PREVIEW_KEY, parameters[Constants.REQUEST_KEY_PREVIEW_KEY])
				.AddParam(Constants.REQUEST_KEY_RESET, parameters[Constants.REQUEST_KEY_RESET])
				.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, parameters[Constants.REQUEST_KEY_SCORINGSALE_ID])
				.CreateUrl();

			Response.Redirect(previewUrl);
		}

		// Go to top page
		var topUrl = string.Format(
			"{0}{1}",
			Constants.PATH_ROOT,
			Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE);

		var url = new UrlCreator(topUrl)
			.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, parameters[Constants.REQUEST_KEY_SCORINGSALE_ID])
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>
	/// Link button next click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNext_Click(object sender, EventArgs e)
	{
		GoToPageSessionExpired();
		var parameters = GetParameters();

		// Set choosen
		SetChoosen();

		// Calculate total score
		CalculateTotalScore();

		if (this.PageNumber < this.ScoringSale.QuestionPages.Length) GoToQuestionPage(this.PageNumber + 1);

		if (this.IsScoringSalePreview)
		{
			var smartPhone = (this.DesignType == Constants.SCORINGSALE_DESIGN_TYPE_SP)
				? "SmartPhone/"
				: string.Empty;
			var urlPath = string.Format(
				"{0}{1}{2}",
				Constants.PATH_ROOT,
				smartPhone,
				Constants.PAGE_FRONT_SCORINGSALE_RESULT_PAGE);

			var urlPageResult = new UrlCreator(urlPath)
				.AddParam(Constants.REQUEST_KEY_PREVIEW_KEY, parameters[Constants.REQUEST_KEY_PREVIEW_KEY])
				.AddParam(Constants.REQUEST_KEY_RESET, parameters[Constants.REQUEST_KEY_RESET])
				.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, parameters[Constants.REQUEST_KEY_SCORINGSALE_ID])
				.CreateUrl();
			Response.Redirect(urlPageResult);
		}

		// Redirect to Result page
		var resultUrl = string.Format(
			"{0}{1}",
			Constants.PATH_ROOT,
			Constants.PAGE_FRONT_SCORINGSALE_RESULT_PAGE);

		var url = new UrlCreator(resultUrl)
			.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, parameters[Constants.REQUEST_KEY_SCORINGSALE_ID])
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>
	/// Set choosen
	/// </summary>
	protected void SetChoosen()
	{
		// Get questions
		var questions = ucScoringSaleQuestionList.GetQuestions();
		this.CurrentPage.QuestionPageItems = questions;
	}

	/// <summary>
	/// Check scoring sale
	/// </summary>
	private void CheckScoringSale()
	{
		var parameters = GetParameters();
		this.ScoringSaleId = this.Request.Params[Constants.REQUEST_KEY_SCORINGSALE_ID];

		// Check scoring sale preview
		if (this.IsScoringSalePreview)
		{
			var paramPreviewKey = parameters[Constants.REQUEST_KEY_PREVIEW_KEY];
			var scoringSale = ScoringSaleDesignHelper.GetPreviewModel(
				paramPreviewKey,
				(this.DesignType == Constants.SCORINGSALE_DESIGN_TYPE_SP)
					? Constants.SCORINGSALE_DESIGN_TYPE_SP
					: Constants.SCORINGSALE_DESIGN_TYPE_PC);

			this.ScoringSaleId = scoringSale.ScoringSaleId;
		}

		CheckErrorSessionExpired();

		LoadScoringSaleInputLastChangedTime(this.ScoringSaleId);

		// Check data question page 1
		if (this.PageNumber != 1) return;

		if ((this.ScoringSale == null)
			&& (this.PageNumber == 1))
		{
			LoadScoringSale(this.ScoringSaleId);
		}

		if (this.ScoringSale != null)
		{
			if (this.ScoringSale.ScoringSaleId != this.ScoringSaleId)
			{
				this.ScoringSale = null;
				LoadScoringSale(this.ScoringSaleId);
			}
		}
	}

	/// <summary>
	/// Load pages
	/// </summary>
	private void LoadPages()
	{
		if (this.ScoringSale.QuestionPages.Length != 0) return;

		this.ScoringSale.QuestionPages = DomainFacade.Instance.ScoringSaleService.GetScoringSaleQuestionPageName(this.ScoringSale.ScoringSaleId)
			.Select(item => new ScoringSaleQuestionPageInput(item))
			.ToArray();
	}

	/// <summary>
	/// Load current page items
	/// </summary>
	private void LoadCurrentPageItems()
	{
		// Check valid page number
		if (this.PageNumber > this.ScoringSale.QuestionPages.Length)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NO_PARAM);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		this.ScoringSale.LoadCurrentQuestionPage(this.PageNumber.ToString());
		if (this.CurrentPage.QuestionPageItems.Length != 0) return;

		var service = DomainFacade.Instance.ScoringSaleService;
		var pageItems = service.GetScoringSaleQuestionPageItems(
			this.CurrentPage.ScoringSaleId,
			int.Parse(this.CurrentPage.PageNo));

		foreach (var pageItem in pageItems)
		{
			pageItem.Question = service.GetScoringSaleQuestion(pageItem.QuestionId);
			pageItem.Question.ScoringSaleQuestionChoiceList = service
				.GetScoringSaleQuestionChoices(pageItem.QuestionId)
				.ToList();
		}

		var pageItemInputs = pageItems
			.Select(pageItem => new ScoringSaleQuestionPageItemInput(pageItem))
			.OrderBy(item => item.BranchNo)
			.ToArray();
		var questionCount = this.ScoringSale.QuestionPages.Sum(page => page.QuestionPageItems.Length);

		foreach (var item in pageItemInputs)
		{
			questionCount++;
			item.Question.QuestionNo = questionCount;
		}

		this.CurrentPage.QuestionPageItems = pageItemInputs;
	}

	/// <summary>
	/// Calculate total score
	/// </summary>
	private void CalculateTotalScore()
	{
		var totalScore = new ScoreAxisInput();
		var questionPages = this.ScoringSale.QuestionPages.SelectMany(item => item.QuestionPageItems).ToArray();

		foreach (var pageItem in questionPages)
		{
			totalScore.AddScores(pageItem.Question.Choices.Where(choice => choice.IsChosen).ToArray());
		}

		this.ScoringSale.TotalScore = totalScore;
	}

	/// <summary>
	/// Go to question page
	/// </summary>
	/// <param name="pageNo">Page no</param>
	private void GoToQuestionPage(int pageNo)
	{
		var parameters = GetParameters();

		if (this.IsScoringSalePreview)
		{
			var smartPhone = (this.DesignType == Constants.SCORINGSALE_DESIGN_TYPE_SP)
				? "SmartPhone/"
				: string.Empty;
			var questionPreviewUrl = string.Format(
				"{0}{1}{2}",
				Constants.PATH_ROOT,
				smartPhone,
				Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE);

			var previewUrl = new UrlCreator(questionPreviewUrl)
				.AddParam(Constants.REQUEST_KEY_PREVIEW_KEY, parameters[Constants.REQUEST_KEY_PREVIEW_KEY])
				.AddParam(Constants.REQUEST_KEY_RESET, parameters[Constants.REQUEST_KEY_RESET])
				.AddParam(Constants.REQUEST_KEY_PAGE_NO, pageNo.ToString())
				.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, parameters[Constants.REQUEST_KEY_SCORINGSALE_ID])
				.CreateUrl();

			Response.Redirect(previewUrl);
		}

		var topUrl = string.Format(
			"{0}{1}",
			Constants.PATH_ROOT,
			Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE);

		var url = new UrlCreator(topUrl)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, pageNo.ToString())
			.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, parameters[Constants.REQUEST_KEY_SCORINGSALE_ID])
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>
	/// Set css for next button
	/// </summary>
	public void SetCssForNextButton()
	{
		var lastQuestionPage = this.CurrentPage.QuestionPageItems.LastOrDefault();
		var isDisable = ((lastQuestionPage != null)
			&& (lastQuestionPage.Question.AnswerType != Constants.FLG_SCORINGSALE_QUESTION_TYPE_PULLDOWN));

		this.WlbNext.CssClass = isDisable
			? "scoringsale_btn _go disable"
			: "scoringsale_btn _go";
	}

	/// <summary>
	/// Go to page session expired
	/// </summary>
	protected void GoToPageSessionExpired()
	{
		var scoringSale = DomainFacade.Instance.ScoringSaleService.GetScoringSale(this.ScoringSaleId);

		if ((this.ScoringSale == null)
			&& ((this.PageNumber != 1)
				|| ((this.PageNumber == 1)
					&& (scoringSale.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLAG_ON))))
		{
			this.ScoringSaleId = this.Request.Params[Constants.REQUEST_KEY_SCORINGSALE_ID];
			var page = (scoringSale.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLAG_ON)
				? Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE
				: Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE;
			var urlPath = Path.Combine(
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC,
				page);

			var url = new UrlCreator(urlPath)
				.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, scoringSale.ScoringSaleId)
				.CreateUrl();
			Response.Redirect(url);
		}
		else
		{
			CheckScoringSale();
			LoadPages();
			LoadCurrentPageItems();
		}
	}

	/// <summary>
	/// Check error session expired
	/// </summary>
	public void CheckErrorSessionExpired()
	{
		var scoringSale = DomainFacade.Instance.ScoringSaleService.GetScoringSale(this.ScoringSaleId);

		if (this.ScoringSale != null) return;

		if ((this.PageNumber != 1)
			|| ((this.PageNumber == 1)
				&& (scoringSale.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLAG_ON)))
		{
			this.ScoringSaleErrorSessionExpired = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SCORINGSALE_SESSION_EXPIRED);
			GoToPageSessionExpired();
		}
	}

	#region +Properties
	/// <summary>Current page</summary>
	protected ScoringSaleQuestionPageInput CurrentPage
	{
		get { return this.ScoringSale.CurrentQuestionPage; }
	}
	/// <summary>Scoring sale id</summary>
	public string ScoringSaleId { get; set; }
	#endregion
}
