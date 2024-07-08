/*
=========================================================================================================
  Module      : 配送会社情報一覧ページ(DeliveryCompany.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using w2.Common.Web;

public partial class Form_DeliveryCompany_DeliveryCompanyList : DeliveryCompanyPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 一覧表示
		ViewDeliveryCompanyList();
	}

	/// <summary>
	/// 一覧表示
	/// </summary>
	private void ViewDeliveryCompanyList()
	{
		// 検索条件取得
		var pageNo = 1;
		if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out pageNo) == false) pageNo = 1;

		// 一覧取得
		var deliveryCompanyList = this.DeliveryCompanyService.Search(Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (pageNo - 1) + 1, Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * pageNo);

		rDeliveryCompanyList.DataSource = deliveryCompanyList;
		rDeliveryCompanyList.DataBind();

		// 件数取得
		var totalDeliveryCompanyCount = this.DeliveryCompanyService.GetAll().Length;

		// ページャー
		var rootUrl = CreateRootUrl();
		lbPager.Text = WebPager.CreateDefaultListPager(totalDeliveryCompanyCount, pageNo, rootUrl);
	}

	/// <summary>
	/// ルートURL作成
	/// </summary>
	/// <returns>ルートURL</returns>
	private string CreateRootUrl()
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_DELIVERY_COMPANY_LIST);

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// 「新規登録」ボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO] = null;

		// 新規登録ページ表示
		Response.Redirect(CreateDeliveryCompanyUrl(string.Empty, Constants.PAGE_MANAGER_DELIVERY_COMPANY_REGISTER, Constants.ACTION_STATUS_INSERT));
	}
}