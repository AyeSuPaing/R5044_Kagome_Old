/*
=========================================================================================================
  Module      : 注文同梱ページクラス(OrderCombine.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Web.WebCustomControl;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.ShopShipping;
using w2.Domain.DeliveryCompany;

/// <summary>
/// 注文同梱ページクラス
/// </summary>
public partial class Form_OrderCombine_OrderCombine : OrderPage
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
			tbUserId.Text = Request[Constants.REQUEST_KEY_USER_ID];
			tbUserName.Text = Request[Constants.REQUEST_KEY_USER_NAME];

			var userId = StringUtility.ToEmpty(tbUserId.Text).Trim();
			var userName = StringUtility.ToEmpty(tbUserName.Text).Trim();
			var parentOrderCount = OrderCombineUtility.GetCombinableParentOrderWithConditionCount(userId, userName);
			var pageUrl = CreatePageUrl();
			lbPager.Text = WebPager.CreateDefaultListPager(parentOrderCount, this.CurrentPageNo, pageUrl, Constants.CONST_DISP_LIST_CONTENTS_COUNT_ORDERCOMBINE);

			ParendOrderBind(userId, userName);
			hgcErrorMessageRow.Visible = false;

			if (parentOrderCount == 0)
			{
				hgcErrorMessageRow.Visible = true;
				lErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERCOMBINE_NO_HIT_LIST);
			}
		}
	}

	/// <summary>
	/// 親注文データバインド
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="userName">氏名</param>
	private void ParendOrderBind(string userId, string userName)
	{
		var startRowNum = (Constants.CONST_DISP_LIST_CONTENTS_COUNT_ORDERCOMBINE * (this.CurrentPageNo - 1)) + 1;
		var endRowNum = Constants.CONST_DISP_LIST_CONTENTS_COUNT_ORDERCOMBINE * this.CurrentPageNo;
		rOrderCombineParentOrder.DataSource = OrderCombineUtility.GetCombinableParentOrderWithCondition(
			this.LoginOperatorShopId,
			userId,
			userName,
			startRowNum,
			endRowNum);
		rOrderCombineParentOrder.DataBind();
	}

	/// <summary>
	/// ページURL作成
	/// </summary>
	/// <returns>URL</returns>
	private string CreatePageUrl()
	{
		var url = Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERCOMBINE_ORDER_COMBINE
			+ "?" + Constants.REQUEST_KEY_USER_ID + "=" + HttpUtility.UrlEncode(tbUserId.Text)
			+ "&" + Constants.REQUEST_KEY_USER_NAME + "=" + HttpUtility.UrlEncode(tbUserName.Text);

		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERCOMBINE_ORDER_COMBINE)
			.AddParam(Constants.REQUEST_KEY_USER_ID, tbUserId.Text)
			.AddParam(Constants.REQUEST_KEY_USER_NAME, tbUserName.Text);

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// 親指定選択変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbgSelectedOrderCombineParentOrder_CheckedChanged(object sender, EventArgs e)
	{
		var selectedParentOrderId = ((HiddenField)((RepeaterItem)((RadioButtonGroup)sender).Parent).FindControl("hfOrderCombineParentOrderId")).Value;
		hfSelectedOrderCombineParentOrderId.Value = selectedParentOrderId;

		var userId = StringUtility.ToEmpty(tbUserId.Text).Trim();
		var userName = StringUtility.ToEmpty(tbUserName.Text).Trim();
		ParendOrderBind(userId, userName);

		var rChildOrder = (Repeater)((RepeaterItem)((RadioButtonGroup)sender).Parent).FindControl("rOrderCombineChildOrder");
		rChildOrder.DataBind();
	}

	/// <summary>
	/// 親注文が選択されているか判定
	/// </summary>
	/// <param name="parentOrderId">親注文ID</param>
	/// <returns>判定結果 親注文が選択されている場合TRUE、選択されていない場合FALSE</returns>
	protected bool IsParentOrderSelect(string parentOrderId)
	{
		return (parentOrderId == hfSelectedOrderCombineParentOrderId.Value);
	}

	/// <summary>
	/// 同梱注文作成ボタンクリック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void bCreateCombinedOrder_Click(object sender, EventArgs e)
	{
		var selectedParentOrderId = hfSelectedOrderCombineParentOrderId.Value;
		var selectedChildOrderIds = (string)Request["OrderCombineChildOrder"];

		if (string.IsNullOrEmpty(selectedParentOrderId) || string.IsNullOrEmpty(selectedChildOrderIds))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERCOMBINE_NOSELECT);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		Response.Redirect(Constants.PATH_ROOT
			+ Constants.PAGE_MANAGER_ORDER_REGIST_INPUT
			+ string.Format("?{0}={1}&{2}={3}&{4}={5}",
				Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_ORDERCOMBINE,
				Constants.REQUEST_KEY_ORDERCOMBINE_PARENT_ORDER_ID, selectedParentOrderId,
				Constants.REQUEST_KEY_ORDERCOMBINE_CHILD_ORDER_IDs, selectedChildOrderIds));
	}

	/// <summary>
	/// 検索ボタンクリック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void bSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreatePageUrl());
	}

	/// <summary>
	/// 配送希望時間帯文言取得
	/// </summary>
	/// <param name="shippingCompanyId">配送業者ID</param>
	/// <param name="shippingTimeId">配送希望時間帯ID</param>
	/// <returns>配送希望時間帯文言</returns>
	protected string GetShippingTimeMessage(string shippingCompanyId, string shippingTimeId)
	{
		var shippingCompany = new DeliveryCompanyService().Get(shippingCompanyId);
		var timeMessage = (shippingCompany != null) ? shippingCompany.GetShippingTimeMessage(shippingTimeId) : "";
			return (string.IsNullOrEmpty(timeMessage) == false) ? timeMessage : ReplaceTag("@@DispText.shipping_time_list.none@@");
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="strProductId">商品ID</param>
	/// <returns>商品詳細URL</returns>
	protected new string CreateProductDetailUrl(string strProductId)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_CONFIRM);
		urlCreator.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, strProductId);
		urlCreator.AddParam("action_status", "detail");
		var url = urlCreator.CreateUrl();

		return url;
	}

	/// <summary>
	/// 表示用定期注文回数・出荷回数取得
	/// </summary>
	/// <param name="order">注文</param>
	/// <returns>定期注文回数・出荷回数文字列</returns>
	protected string GetFixedPurchaseOrderCountAndShippedCount(OrderModel order)
	{
		if (string.IsNullOrEmpty(order.FixedPurchaseId)) return "― ｜ ―";

		var orderCount = string.Format(ReplaceTag("@@DispText.common_message.times@@"), (order.FixedPurchaseOrderCount ?? 0));
		var shippedCount = string.Format(ReplaceTag("@@DispText.common_message.times@@"), (order.FixedPurchaseShippedCount ?? 0));
		var orderCountAndShippedCount = string.Format("{0} ｜ {1}", orderCount, shippedCount);

		return orderCountAndShippedCount;
	}

	#region プロパティ
	/// <summary>同梱対象ID</summary>
	protected string[] CombineTargetsID { get; set; }
	/// <summary>同梱後の新しいID</summary>
	protected string NewID { get; set; }
	/// <summary>カレントページ番号</summary>
	private int CurrentPageNo
	{
		get
		{
			int pageNo;
			if ((int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) == false) || (pageNo < 1))
			{
				pageNo = 1;
			}
			return pageNo;
		}
	}
	#endregion

}