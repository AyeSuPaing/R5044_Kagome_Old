/*
=========================================================================================================
  Module      : 定期購入同梱クラス (FixedPurchaseCombine.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order.FixedPurchaseCombine;
using w2.App.Common.Util;
using w2.App.Common.Web.WebCustomControl;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// 定期購入同梱クラス
/// </summary>
public partial class Form_OrderCombine_FixedPurchaseCombine : OrderPage
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
			this.FixedPurchaseCombineTargetsID = string.Empty;
			this.FixedPurchaseCombineParentID = string.Empty;

			SetNextShippingDateDropDownList();

			var nextShippingDateFrom = string.Format(
				"{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_TIME_FROM])
					.Replace(":", string.Empty));

			var nextShippingDateTo = string.Format(
				"{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_TIME_TO])
					.Replace(":", string.Empty));

			ucNextShippingDatePeriod.SetPeriodDate(nextShippingDateFrom, nextShippingDateTo);

			tbUserId.Text = Request[Constants.REQUEST_KEY_USER_ID];
			tbUserName.Text = Request[Constants.REQUEST_KEY_USER_NAME];

			hgcErrorMessageRow.Visible = false;
			hgcCompleteMessageRow.Visible = false;

			ParentFixedPurchaseBind();
		}
	}

	/// <summary>
	/// 親定期台帳バインド
	/// </summary>
	private void ParentFixedPurchaseBind()
	{
		var startRowNum = (Constants.CONST_DISP_LIST_CONTENTS_COUNT_FIXEDPURCHASECOMBINE * (this.CurrentPageNo - 1)) + 1;
		var endRowNum = Constants.CONST_DISP_LIST_CONTENTS_COUNT_FIXEDPURCHASECOMBINE * this.CurrentPageNo;

		var userId = StringUtility.ToEmpty(tbUserId.Text).Trim();
		var userName = StringUtility.ToEmpty(tbUserName.Text).Trim();

		// 末日補正処理
		DateTime nextShipDateFrom;
		if (DateTime.TryParse(ucNextShippingDatePeriod.HfStartDate.Value, out nextShipDateFrom))
		{
			if (DateTimeUtility.IsLastDayOfMonth(
				nextShipDateFrom.Year,
				nextShipDateFrom.Month,
				nextShipDateFrom.Day))
			{
				var nextShippingLastOfDay = DateTimeUtility.GetLastDayOfMonth(
					nextShipDateFrom.Year,
					nextShipDateFrom.Month);
				ucNextShippingDatePeriod.SetStartDate(nextShipDateFrom.AddDays(nextShippingLastOfDay));
			}
			nextShipDateFrom = DateTime.Parse(ucNextShippingDatePeriod.StartDateTimeString);
		}

		// 末日補正処理
		DateTime nextShipDateTo;
		if (DateTime.TryParse(ucNextShippingDatePeriod.HfEndDate.Value, out nextShipDateTo))
		{
			if (DateTimeUtility.IsLastDayOfMonth(
				nextShipDateTo.Year,
				nextShipDateTo.Month,
				nextShipDateTo.Day))
			{
				var nextShippingLastOfDay = DateTimeUtility.GetLastDayOfMonth(
					nextShipDateTo.Year,
					nextShipDateTo.Month);
				ucNextShippingDatePeriod.SetEndDate(nextShipDateTo.AddDays(nextShippingLastOfDay));
			}
			nextShipDateTo = DateTime.Parse(ucNextShippingDatePeriod.EndDateTimeString);
		}

		rFixedPurchaseCombineParentFixedPurchase.DataSource = FixedPurchaseCombineUtility.GetCombinableParentFixedPurchaseWithCondition(userId, userName, nextShipDateFrom, nextShipDateTo, startRowNum, endRowNum);
		rFixedPurchaseCombineParentFixedPurchase.DataBind();

		var parentOrderCount = FixedPurchaseCombineUtility.GetCombinableParentFixedPurchaseWithConditionCount(userId, userName, nextShipDateFrom, nextShipDateTo);
		var pageUrl = CreatePageUrl();
		lbPager.Text = WebPager.CreateDefaultListPager(parentOrderCount, this.CurrentPageNo, pageUrl, Constants.CONST_DISP_LIST_CONTENTS_COUNT_FIXEDPURCHASECOMBINE);

		if (parentOrderCount == 0)
		{
			hgcErrorMessageRow.Visible = true;
			lErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERCOMBINE_NO_HIT_LIST);
		}
	}

	/// <summary>
	/// ページURL作成
	/// </summary>
	/// <returns>URL</returns>
	private string CreatePageUrl()
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXEDPURCHASECOMBINE_FIXEDPURCHASE_COMBINE);
		urlCreator.AddParam(Constants.REQUEST_KEY_USER_ID, tbUserId.Text);
		urlCreator.AddParam(Constants.REQUEST_KEY_USER_NAME, tbUserName.Text);
		urlCreator.AddParam(
			Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_FROM,
			ucNextShippingDatePeriod.HfStartDate.Value);
		urlCreator.AddParam(
			Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_TO,
			ucNextShippingDatePeriod.HfEndDate.Value);
		urlCreator.AddParam(
			Constants.REQUEST_KEY_FIXEDPURCHASE_TIME_FROM,
			ucNextShippingDatePeriod.HfStartTime.Value);
		urlCreator.AddParam(
			Constants.REQUEST_KEY_FIXEDPURCHASE_TIME_TO,
			ucNextShippingDatePeriod.HfEndTime.Value);
		var url = urlCreator.CreateUrl();

		return url;
	}

	/// <summary>
	/// ドロップリストに合致する値があれば選択
	/// </summary>
	/// <param name="ddl">ドロップダウンリスト</param>
	/// <param name="value">選択値</param>
	private void SelectList(DropDownList ddl, string value)
	{
		if (ddl.Items.Cast<ListItem>().Any(li => li.Value == value)) ddl.SelectedValue = value;
	}

	/// <summary>
	/// 親指定選択変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbgSelectedParentFixedPurchase_CheckedChanged(object sender, EventArgs e)
	{
		hgcCompleteMessageRow.Visible = false;

		var selectedParentFpId = ((HiddenField)((RepeaterItem)((RadioButtonGroup)sender).Parent).FindControl("hfFixedPurchaseCombineParentFpId")).Value;
		hfSelectedFixedPurchaseCombineParentFpId.Value = selectedParentFpId;

		ParentFixedPurchaseBind();

		var rChildFp = (Repeater)((RepeaterItem)((RadioButtonGroup)sender).Parent).FindControl("rFixedPurchaseCombineChildFixedPurchase");
		rChildFp.DataBind();
	}

	/// <summary>
	/// 親定期購入が選択されているか判定
	/// </summary>
	/// <param name="parentFixedPurchaseId">親定期購入ID</param>
	/// <returns>判定結果 親注文が選択されている場合TRUE、選択されていない場合FALSE</returns>
	protected bool IsParentFixedPurchaseSelect(string parentFixedPurchaseId)
	{
		return (parentFixedPurchaseId == hfSelectedFixedPurchaseCombineParentFpId.Value);
	}

	/// <summary>
	/// 同梱実行ボタンクリック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void bCreateCombinedOrder_Click(object sender, EventArgs e)
	{
		var selectedParentFpId = hfSelectedFixedPurchaseCombineParentFpId.Value;
		var selectedChildFpIds = (string)Request["FixedPurchaseCombineChildFixedPurchase"];

		if (string.IsNullOrEmpty(selectedParentFpId) || string.IsNullOrEmpty(selectedChildFpIds))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXEDPURCHASECOMBINE_NOSELECT);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 同梱（更新履歴とともに）
		var result = FixedPurchaseCombineUtility.CombineFixedPurchase(
			selectedParentFpId,
			selectedChildFpIds.Split(','),
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		if (result == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERCOMBINE_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		this.FixedPurchaseCombineParentID = selectedParentFpId;
		this.FixedPurchaseCombineTargetsID = selectedChildFpIds.Replace(",", ", ");
		hgcCompleteMessageRow.Visible = true;
		hfSelectedFixedPurchaseCombineParentFpId.Value = string.Empty;

		ParentFixedPurchaseBind();
	}

	/// <summary>
	/// 次回配送日ドロップダウンリストセット
	/// </summary>
	private void SetNextShippingDateDropDownList()
	{
		var dateInterval = DateTime.Now.AddDays(Constants.CONST_DISP_FIXEDPURCHASECOMBINE_INITIAL_NEXT_SHIPPING_DATE_INTERVAL);
		// 初期値セット
		ucNextShippingDatePeriod.SetPeriodDate(DateTime.Now, dateInterval);
	}

	/// <summary>
	/// 検索ボタンクリック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// 次回配送日を入力していない場合、エラーメッセージを表示
		if ((string.IsNullOrEmpty(ucNextShippingDatePeriod.HfStartDate.Value))
			|| (string.IsNullOrEmpty(ucNextShippingDatePeriod.HfEndDate.Value))
			|| (string.IsNullOrEmpty(ucNextShippingDatePeriod.HfStartTime.Value))
			|| (string.IsNullOrEmpty(ucNextShippingDatePeriod.HfEndTime.Value)))
		{
			hgcErrorMessageRow.Visible = true;
			lErrorMessage.Text = WebMessages.GetMessages(WebMessages.INPUTCHECK_NECESSARY)
				.Replace("@@ 1 @@", ReplaceTag("@@DispText.fixed_purchase.FixedPurchaseNextShippingDate@@"));
			return;
		}
		hgcErrorMessageRow.Visible = false;

		Response.Redirect(CreatePageUrl());
	}

	/// <summary>
	/// 定期購入同梱対象の定期購入情報取得
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <returns>定期購入情報</returns>
	protected FixedPurchaseModel[] GetCombinableChildFixedPurchases(string fixedPurchaseId, string userId)
	{
		var nextShipDateFrom = DateTime.Parse(ucNextShippingDatePeriod.StartDateTimeString);

		var nextShipDateTo = DateTime.Parse(ucNextShippingDatePeriod.EndDateTimeString);

		var models = FixedPurchaseCombineUtility.GetCombinableChildFixedPurchases(fixedPurchaseId, userId, nextShipDateFrom ,nextShipDateTo);

		return models;
	}

	#region プロパティ
	/// <summary>定期購入同梱 対象定期購入ID</summary>
	protected string FixedPurchaseCombineTargetsID { get; set; }
	/// <summary>定期購入同梱 親定期購入ID</summary>
	protected string FixedPurchaseCombineParentID
	{
		get { return ((string)ViewState["ParentFixedPurchaseId"]); }
		set { ViewState["ParentFixedPurchaseId"] = value; }
	}
	/// <summary>同梱対象ID</summary>
	protected string[] InitialTargetFixedPurchaseIds
	{
		get { return (string[])(ViewState["InitialTargetFixedPurchaseIds"] ?? new string[0]); }
		set { ViewState["InitialTargetFixedPurchaseIds"] = value; }
	}
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