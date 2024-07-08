/*
=========================================================================================================
  Module      : 商品セット情報一覧ページ処理(ProductSetList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class Form_ProductSet_ProductSetList : BasePage
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
			//------------------------------------------------------
			// リクエスト取得
			//------------------------------------------------------
			string strProductSetId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SET_ID]);
			string strProductSetName = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SET_NAME]);
			string strSortKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]);
			int iPageNo = 1;
			if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out iPageNo) == false)
			{
				iPageNo = 1;
			}

			//------------------------------------------------------
			// 一覧取得
			//------------------------------------------------------
			DataView dvList = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "GetProductSetList"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTSET_SHOP_ID, this.LoginOperatorShopId);
				htInput.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strProductSetId));
				htInput.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strProductSetName));
				htInput.Add("sort_kbn", strSortKbn);
				htInput.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNo - 1) + 1);
				htInput.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNo);

				dvList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// 画面表示
			//------------------------------------------------------
			int iTotalUserCounts = 0;
			if (dvList.Count != 0)
			{
				trListError.Visible = false;
				iTotalUserCounts = (int)dvList[0].Row["row_count"];
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// 一覧セット
			rList.DataSource = dvList;
			rList.DataBind();

			this.ProductSetIdListOfDisplayedData = dvList.Cast<DataRowView>()
				.Select(drv => (string)drv[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID]).ToArray();

			// 検索ボックスセット
			tbProductSetId.Text = strProductSetId;
			tbProductSetName.Text = strProductSetName;
			foreach (ListItem li in ddlSortKbn.Items)
			{
				li.Selected = (li.Value == strSortKbn);
			}

			// ページャセット
			string strNextUrl = CreateListUrl();
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalUserCounts, iPageNo, strNextUrl);
		}
	}

	/// <summary>
	/// 一覧URL作成(ページャ用）
	/// </summary>
	/// <returns></returns>
	protected string CreateListUrl()
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSET_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_PRODUCT_SET_ID).Append("=").Append(HttpUtility.UrlEncode(tbProductSetId.Text.Trim()));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_SET_NAME).Append("=").Append(HttpUtility.UrlEncode(tbProductSetName.Text.Trim()));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode(ddlSortKbn.SelectedValue));

		return sbResult.ToString();
	}

	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="strProductSetId"></param>
	/// <returns></returns>
	protected string CreateDetailUrl(string strProductSetId)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSET_CONFIRM);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_SET_ID).Append("=").Append(HttpUtility.UrlEncode(strProductSetId));

		return sbResult.ToString();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateListUrl());
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSET_REGISTER);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_INSERT));

		Response.Redirect(sbResult.ToString());
	}

	/// <summary>
	/// 翻訳データ出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = this.ProductSetIdListOfDisplayedData;
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}

	/// <summary>画面表示されている商品セットIDリスト</summary>
	private string[] ProductSetIdListOfDisplayedData
	{
		get { return (string[])ViewState["productsetid_list_of_displayed_data"]; }
		set { ViewState["productsetid_list_of_displayed_data"] = value; }
	}
}
