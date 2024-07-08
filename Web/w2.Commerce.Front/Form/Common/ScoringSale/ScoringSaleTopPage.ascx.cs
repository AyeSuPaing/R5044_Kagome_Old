/*
=========================================================================================================
  Module      : Scoring Sale Top Page (ScoringSaleTopPage.ascx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using w2.App.Common.ScoringSale;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;

/// <summary>
/// Scoring sale top page
/// </summary>
public partial class Form_Common_ScoringSale_ScoringSaleTopPage : ScoringSalePage
{
	#region +WrappedControlDeclaration
	/// <summary>Wrapped link button start</summary>
	protected WrappedLinkButton WlbStart { get { return GetWrappedControl<WrappedLinkButton>("lbStart"); } }
	#endregion

	/// <summary>Scoring sale first page</summary>
	private const string SCORINGSALE_FIRST_PAGE = "1";

	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var parameters = GetParameters();
		this.ScoringSaleId = parameters[Constants.REQUEST_KEY_SCORINGSALE_ID];

		if (!IsPostBack)
		{
			ShowErrorSessionExpired();

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

			// Check scoring sale id
			if (string.IsNullOrEmpty(this.ScoringSaleId))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NO_PARAM);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			LoadScoringSaleInputLastChangedTime(this.ScoringSaleId);

			if (this.ScoringSale == null)
			{
				LoadScoringSale(this.ScoringSaleId);
			}
			else
			{
				if (this.ScoringSale.ScoringSaleId != this.ScoringSaleId)
				{
					this.ScoringSale = null;
					LoadScoringSale(this.ScoringSaleId);
				}
			}
			this.TopPageTitle = this.ScoringSale.TopPageTitle;
			this.TopPageSubTitle = this.ScoringSale.TopPageSubTitle;
			this.TopPageBody = this.ScoringSale.TopPageBody;
			this.TopPageImgPath = (string.IsNullOrEmpty(this.ScoringSale.TopPageImgPath) == false)
				? Path.Combine(
					Constants.PATH_ROOT_FRONT_PC,
					this.ScoringSale.TopPageImgPath)
				: string.Empty;
			this.WlbStart.InnerControl.Text = this.ScoringSale.TopPageBtnCaption;

			if (this.IsScoringSalePreview == false)
			{
				// Content log output
				var contentId = string.Format("{0}_1", this.ScoringSale.ScoringSaleId);
				this.InsertPageViewContentsLog(contentId);
			}

			this.DataBind();
		}
	}

	/// <summary>
	/// Link button start click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbStart_Click(object sender, System.EventArgs e)
	{
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
			var topUrl = new UrlCreator(questionPreviewUrl);

			var parameters = GetParameters();
			foreach (var item in parameters)
			{
				topUrl.AddParam(item.Key, item.Value);
			}
			topUrl.AddParam(Constants.REQUEST_KEY_PAGE_NO, SCORINGSALE_FIRST_PAGE);

			Response.Redirect(topUrl.CreateUrl());
		}

		if (this.ScoringSale == null) LoadScoringSale(this.ScoringSaleId);

		var urlPath = string.Format(
			"{0}{1}",
			Constants.PATH_ROOT,
			Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE);
		var firstPageUrl = new UrlCreator(urlPath)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, SCORINGSALE_FIRST_PAGE)
			.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, this.ScoringSaleId)
			.CreateUrl();

		Response.Redirect(firstPageUrl);
	}

	#region +Properties
	/// <summary> Top page title </summary>
	public string TopPageTitle { get; set; }
	/// <summary> Top page sub title </summary>
	public string TopPageSubTitle { get; set; }
	/// <summary> Top page body </summary>
	public string TopPageBody { get; set; }
	/// <summary> Scoring sale id </summary>
	public string ScoringSaleId { get; set; }
	/// <summary>Top page img path</summary>
	public string TopPageImgPath { get; set; }
	#endregion
}
