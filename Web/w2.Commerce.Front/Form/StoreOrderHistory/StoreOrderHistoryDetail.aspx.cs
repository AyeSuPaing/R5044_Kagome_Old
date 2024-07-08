/*
=========================================================================================================
  Module      : 店舗注文購入詳細 (StoreOrderHistoryDetail.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.CrossPoint.OrderHistory;

public partial class Form_StoreOrderHistory_StoreOrderHistoryDetail : BasePage
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
			Display();
		}
	}

	/// <summary>
	/// 表示
	/// </summary>
	private void Display()
	{
		var orderId = StringUtility.ToEmpty(Request[w2.App.Common.Constants.CROSS_POINT_PARAM_POINT_SLIP_NO]);
		var orders = new CrossPointOrderHistoryApiService().GetDetail(orderId);
		
		if ((Constants.CROSS_POINT_OPTION_ENABLED == false)
			|| (orders == null)
			|| (orders.NetShopMemberId != this.LoginUserId))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = Constants.CROSS_POINT_OPTION_ENABLED
				? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDERHISTORY_UNDISP)
				: WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		this.ShopOrder = orders;
		DataBind();
	}

	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }
	/// <summary>マイページメニューに表示するか</summary>
	public override bool DispMyPageMenu { get { return true; } }
	/// <summary>店舗注文</summary>
	public OrderHistoryApiResult ShopOrder { get; set; }
}