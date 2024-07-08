/*
=========================================================================================================
  Module      : LPプレビューページ処理(LpPreview.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Web;
using w2.App.Common.LandingPage;
using w2.Domain.LandingPage;

/// <summary>
/// LpPreview の概要の説明です
/// </summary>
public abstract partial class Landing_formlp_LpPreview : OrderLandingFormLp
{
	/// <summary>商品追加エラー</summary>
	protected List<string> m_addProductError = new List<string>();

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public new void Page_Init(Object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			SetLpModel();
		}
		base.Page_Init(sender, e);

		Initialize(this.LpModel);

		Session[this.LadingCartProductSetSelectSessionKey] = null;
	}

	/// <summary>
	/// 初期化（外部から呼び出し）
	/// </summary>
	/// <param name="landingPageDesign">ランディングページデザイン</param>
	protected void Initialize(LandingPageDesignModel landingPageDesign)
	{
		this.PageDesignInput = CreatePageDesign(landingPageDesign);
		DisplayControlPage(landingPageDesign);

		ucInputForm.LandingPageDesignModel = landingPageDesign;
	}

	/// <summary>
	/// Lpモデルを格納
	/// </summary>
	protected void SetLpModel()
	{
		var previewkey = Request.QueryString["previewkey"];
		var pageDesiginType = SmartPhoneUtility.CheckSmartPhoneSite(HttpContext.Current.Request.Path)
			? LandingPageConst.PAGE_DESIGN_TYPE_SP
			: LandingPageConst.PAGE_DESIGN_TYPE_PC;

		if (string.IsNullOrEmpty(previewkey))
		{
			this.LpModel = null;
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
		if (this.PreviewKey != previewkey)
		{
			this.PreviewKey = previewkey;
			this.LpModel = LpDesignHelper.GetPreviewModel(previewkey, pageDesiginType);
			LpDesignHelper.DeletePreviewFile(previewkey, pageDesiginType);
		}
		if (this.LpModel == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>LPモデル</summary>
	public LandingPageDesignModel LpModel
	{
		get { return (LandingPageDesignModel)Session[this.LpModelSessionKey]; }
		set { Session[this.LpModelSessionKey] = value; }
	}
	/// <summary>LPプレビューキー</summary>
	public string PreviewKey
	{
		get { return StringUtility.ToEmpty(Session["lp_page_pareview_key"]); }
		set { Session["lp_page_pareview_key"] = value; }
	}
	/// <summary>商品追加エラー</summary>
	protected abstract string LpModelSessionKey { get; }
}