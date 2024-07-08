/*
=========================================================================================================
  Module      : 定期購入情報一覧画面(FixedPurchaseList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.App.Common.Web.WrappedContols;
using System.Linq;
using System.Collections.Generic;
using w2.Common.Web;
using System.Web.UI.WebControls;

public partial class Form_FixedPurchase_FixedPurchaseList : FixedPurchasePage
{
	#region ラップ済みコントロール宣言
	WrappedRepeater WrList { get { return GetWrappedControl<WrappedRepeater>("rList"); } }
	WrappedHtmlGenericControl WpInfo { get { return GetWrappedControl<WrappedHtmlGenericControl>("pInfo"); } }
	WrappedHtmlGenericControl WspAlert { get { return GetWrappedControl<WrappedHtmlGenericControl>("spAlert"); } }
	WrappedRepeater WrFixedPurchaseStatusList { get { return GetWrappedControl<WrappedRepeater>("rFixedPurchaseStatusList"); } }
	WrappedLinkButton WlbContinuing { get { return GetWrappedControl<WrappedLinkButton>("lbContinuing"); } }
	WrappedCheckBox WcbFixedPurchaseStatusList { get { return GetWrappedControl<WrappedCheckBox>("cbFixedPurchaseStatusList"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>	
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ログインチェック（ログイン後は定期購入一覧から）
		CheckLoggedIn(Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST);

		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		CheckHttps();

		// 検索条件作成
		var searchCondition = new UserFixedPurchaseListSearchCondition
		{
			UserId = this.LoginUserId,
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.RequestPageNum,
			FixedPurchaseStatusParameter = Request.QueryString[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS]
		};

		if (!IsPostBack)
		{
			// 定期購入絞り込み表セット
			this.WrFixedPurchaseStatusList.DataSource = ValueText
				.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_KBN)
				.Where(status =>
					Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
						? status.Value != ""
						: status.Value != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_COMPLETE)
				.ToArray();
			this.WrFixedPurchaseStatusList.DataBind();
		}

		// 定期購入情報一覧表セット
		var service = new FixedPurchaseService();
		this.WrList.DataSource = service.SearchUserFixedPurchaseExcludeOrderCombineCancel(searchCondition);
		this.WrList.DataBind();

		// 総件数取得
		var totalCount = service.GetCountOfSearchUserFixedPurchaseExcludeOrderCombineCancel(searchCondition);
		// 総件数が0件の場合
		if (totalCount == 0)
		{
			this.WpInfo.Visible = false;
			this.WrList.Visible = false;
			this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDERHISTORY_NO_ITEM);

			// 処理を抜ける
			return;
		}

		// ページャ作成
		var nextUrl = CreateUrl();
		this.PagerHtml = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);
	}

	/// <summary>
	/// 未出荷注文の配送日文言を取得
	/// </summary>
	/// <param name="shippingDateNearest">未出荷注文の配送日付</param>
	/// <returns>未出荷注文の配送日文言</returns>
	protected string GetFixedPurchaseShippingDateNearestValue(DateTime? shippingDateNearest)
	{
		var shippingDateNearestValue = shippingDateNearest.HasValue
			? DateTimeUtility.ToStringFromRegion(shippingDateNearest, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter)
			: ReplaceTag("@@DispText.shipping_date_list.none@@");
		return shippingDateNearestValue;
	}

	/// <summary>
	/// 定期台帳番号に紐づくFixedPurchaseItemContainerリストの取得
	/// </summary>
	/// <param name="fixedPurchaseId">定期台帳番号</param>
	/// <returns>定期台帳番号に紐づくFixedPurchaseItemContainerリスト</returns>
	protected FixedPurchaseItemContainer[] GetFixedPurchaseItemContainerList(string fixedPurchaseId)
	{
		var fixedPurchaseContainer = new FixedPurchaseService().GetContainer(fixedPurchaseId);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			fixedPurchaseContainer = SetProductNameTranslationDataToFixedPurchaseContainer(fixedPurchaseContainer);
		}
		return fixedPurchaseContainer.Shippings[0].Items;
	}

	/// <summary>
	/// 定期一覧URL作成
	/// </summary>
	/// <param name="requestKeyValue">ステータス</param>
	/// <returns>商品一覧URL</returns>
	protected string CreateUrl(string requestKeyValue = null)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST);
		var fixedpurchasestatus = new List<string>();
		
		// 現在のページのパラメータを取得
		if (string.IsNullOrEmpty(this.Request.QueryString[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS]) == false)
		{
			fixedpurchasestatus = new List<string>(Request.QueryString[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS].Split(','));
			foreach (var status in fixedpurchasestatus.Where(status => status != requestKeyValue))
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, status);
			}
		}

		if ((fixedpurchasestatus.Contains(requestKeyValue) == false) && (string.IsNullOrEmpty(requestKeyValue) == false))
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, requestKeyValue);
		}
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// URLに指定のパラメータがあるかどうか判定
	/// </summary>
	/// <param name="requestKeyValue">ステータス</param>
	/// <returns>ある場合TRUE</returns>
	protected bool CheckParam(string requestKeyValue)
	{
		if (string.IsNullOrEmpty(this.Request.QueryString[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS])) return false;

		var result = this.Request.QueryString[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS].Split(',').Contains(requestKeyValue);
		return result;
	}

	/// <summary>
	/// 継続中のみ表示URL作成
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbContinuing_OnClick(object sender, EventArgs e)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_NORMAL)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_NOSTOCK)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_FAILED)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_SUSPEND);
		Response.Redirect(urlCreator.CreateUrl());
	}

	/// <summary>
	/// チェックボックス変更時のリダイレクト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbFixedPurchaseStatusList_OnCheckedChanged(object sender, EventArgs e)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST);
		var kvp = ValueText.GetValueKvpArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_KBN);
		foreach (RepeaterItem ri in this.WrFixedPurchaseStatusList.Items)
		{
			var cb = (CheckBox)ri.FindControl("cbFixedPurchaseStatusList");
			kvp.Where(text => cb.Checked && (text.Value == cb.Text))
				.Select(select => select.Key).ToList()
				.ForEach(param => urlCreator.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, param));
		}
		this.Response.Redirect(urlCreator.CreateUrl());
	}

	#region プロパティ
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	/// <summary>ページャーHTML</summary>
	protected string PagerHtml
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>アラートメッセージ</summary>
	protected string AlertMessage
	{
		get { return (string)ViewState["AlertMessage"]; }
		private set { ViewState["AlertMessage"] = value; }
	}
	#endregion
}
