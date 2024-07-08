/*
=========================================================================================================
  Module      : AmazonPayCv2住所変更遷移画面(AmazonPayCv2ChangeActionRedirect.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.AmazonCv2;
using w2.Common.Web;

/// <summary>
/// AmazonPayCv2住所変更遷移画面
/// </summary>
public partial class Form_OrderHistory_AmazonPayCv2_AmazonPayCv2ChangeActionRedirect : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if ((this.OrderId == null) || (this.ActionStatus == null) || (this.AmazonCheckoutSessionId == null))
		{
			Session.Remove(AmazonCv2Constants.SESSION_KEY_AMAZON_IS_REDIRECTED_CHANGE_PAGE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_DEFAULT);
		}

		this.IsRedirectedChangePage = (Session[AmazonCv2Constants.SESSION_KEY_AMAZON_IS_REDIRECTED_CHANGE_PAGE] != null);

		if (this.IsRedirectedChangePage)
		{
			var url = CreateOrderHistoryPath();

			if (this.ActionStatus == AmazonCv2Constants.AMAZON_ACTION_STATUS_AUTH)
				Session.Remove(AmazonCv2Constants.SESSION_KEY_AMAZON_IS_REDIRECTED_CHANGE_PAGE);

			Response.Redirect(url);
		}
	}

	/// <summary>
	/// 注文履歴遷移パス生成
	/// </summary>
	/// <returns>注文履歴パス</returns>
	private string CreateOrderHistoryPath()
	{
		var callbackPath = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
		.AddParam(
			AmazonCv2Constants.REQUEST_KEY_AMAZON_ACTION_STATUS,
			this.ActionStatus)
		.AddParam(
			AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID,
			this.AmazonCheckoutSessionId)
		.AddParam(
			AmazonCv2Constants.REQUEST_KEY_ORDER_ID,
			this.OrderId)
		.CreateUrl();

		return callbackPath;
	}

	/// <summary>注文ID</summary>
	private string OrderId
	{
		get { return (string)Request[AmazonCv2Constants.REQUEST_KEY_ORDER_ID]; }
	}
	/// <summary>Amazonアクションステータス</summary>
	private string ActionStatus
	{
		get { return (string)Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_ACTION_STATUS]; }
	}
	/// <summary>AmazonCv2チェックアウトセッションID</summary>
	protected string AmazonCheckoutSessionId
	{
		get { return (string)Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID]; }
	}
	/// <summary>Amazon住所変更画面を表示済みか</summary>
	private bool IsRedirectedChangePage
	{
		get { return (bool)Session[AmazonCv2Constants.SESSION_KEY_AMAZON_IS_REDIRECTED_CHANGE_PAGE]; }
		set { Session[AmazonCv2Constants.SESSION_KEY_AMAZON_IS_REDIRECTED_CHANGE_PAGE] = value; }
	}
}