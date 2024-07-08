/*
=========================================================================================================
  Module      : Question (Question.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using w2.App.Common.ScoringSale;
using w2.Domain;
using w2.Domain.ScoringSale;

/// <summary>
/// Question Template
/// </summary>
public partial class Form_ScoringSale_Question : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(Object sender, EventArgs e)
	{
		var scoringSaleId = this.Request.Params[Constants.REQUEST_KEY_SCORINGSALE_ID];
		Initialize(scoringSaleId);
	}

	/// <summary>
	/// Initialize
	/// </summary>
	/// <param name="scoringSaleId">Scoring sale id</param>
	protected void Initialize(string scoringSaleId)
	{
		var model = DomainFacade.Instance.ScoringSaleService.GetScoringSale(scoringSaleId);
		PageCheck(model);
	}

	/// <summary>
	/// Page check
	/// </summary>
	/// <param name="model">Scoring sale model</param>
	private void PageCheck(ScoringSaleModel model)
	{
		if (model == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		if (this.Request.RawUrl.Contains(Constants.REQUEST_KEY_PREVIEW_KEY))
		{
			var previewKey = StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PREVIEW_KEY]);
			var designType = SmartPhoneUtility.CheckSmartPhoneSite(Request.Path)
				? Constants.SCORINGSALE_DESIGN_TYPE_SP
				: Constants.SCORINGSALE_DESIGN_TYPE_PC;
			var pathDirectory = ScoringSaleDesignHelper.GetPreviewDirectory(designType);
			var filePath = ScoringSaleDesignHelper.GetPreviewFilePath(previewKey, designType);

			if (File.Exists(filePath))
			{
				return;
			}
		}

		if ((this.Request.UserHostAddress != Constants.ALLOWED_IP_ADDRESS_FOR_WEBCAPTURE)
			&& (model.IsPublicStatus == false))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}
}
