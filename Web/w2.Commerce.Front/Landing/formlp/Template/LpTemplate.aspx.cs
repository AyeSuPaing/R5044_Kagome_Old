/*
=========================================================================================================
  Module      : LPテンプレートページ処理(LpTemplate.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Web.Services;
using w2.App.Common.Util;
using w2.Domain.LandingPage;

/// <summary>
/// LPテンプレートページ処理
/// </summary>
public partial class Landing_formlp_Template_LpTemplate : OrderLandingFormLp
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンテンツログ出力
			ContentsLogUtility.InsertPageViewContentsLog(
				Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_LANDINGCART,
				this.LpPageDesign.PageId,
				this.IsSmartPhone);
		}

		SessionManager.IsAmazonPayGotRecurringConsent = this.CartList.HasFixedPurchase;
	}

	/// <summary>
	/// 初期化（外部から呼び出し）
	/// </summary>
	/// <param name="pageId">ページID</param>
	/// <returns>ランディングページデザイン</returns>
	protected void Initialize(string pageId)
	{
		this.CartList.LandingCartPageId = pageId;

		this.LpPageDesign = new LandingPageService().GetPageDataWithDesign(
			pageId,
			(this.IsSmartPhone && SmartPhoneUtility.SmartPhoneSiteSettings.Any(setting => setting.SmartPhonePageEnabled))
				? LandingPageConst.PAGE_DESIGN_TYPE_SP
				: LandingPageConst.PAGE_DESIGN_TYPE_PC);
		PageCheckProc(this.LpPageDesign);
		this.PageDesignInput = CreatePageDesign(this.LpPageDesign);
		DisplayControlPage(this.LpPageDesign);

		ucInputForm.LandingPageDesignModel = this.LpPageDesign;
	}

	/// <summary>
	/// ページチェック処理
	/// </summary>
	/// <param name="landingPageDesign">ランディングページデザイン</param>
	private void PageCheckProc(LandingPageDesignModel landingPageDesign)
	{
		// チェックしたりとかをここでやる
		if (landingPageDesign == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		if (((this.Request.UserHostAddress != Constants.ALLOWED_IP_ADDRESS_FOR_WEBCAPTURE)
			&& (landingPageDesign.DisplayCheck() == false))
			|| ((Constants.CART_LIST_LP_OPTION == false)
				&& (landingPageDesign.IsCartListLp)))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>
	/// 注文情報取得
	/// </summary>
	/// <param name="orderReferenceIdOrBillingAgreementId">注文リファレンスIDor支払い契約ID</param>
	/// <param name="orderType">注文種別</param>
	/// <param name="addressType">住所種別</param>
	/// <returns>エラーメッセージ</returns>
	[WebMethod]
	public static string GetAmazonAddress(string orderReferenceIdOrBillingAgreementId, string orderType, string addressType)
	{
		return OrderLandingInputProcess.GetAmazonAddress(
			orderReferenceIdOrBillingAgreementId,
			orderType,
			addressType);
	}
}