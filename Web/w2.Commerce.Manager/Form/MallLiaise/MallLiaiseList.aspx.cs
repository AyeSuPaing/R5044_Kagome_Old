/*
=========================================================================================================
  Module      : モール連携 設定一覧ページ処理(MallLiaiseList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Text;

public partial class Form_MailLiaise_MallLiaiseList : MallPage
{
	// ページャ関連
	int m_iPageNumber = 1;
	const int PAGE_MAX_ROW = 100;

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
			// モール区分を設定
			//------------------------------------------------------
			ddlMallKbn.Items.Clear();
			foreach (ListItem mallKbn in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, "mall_kbn_search"))
			{
				// Remove item of Facebook catalog api cooperation setting when option setting off
				if ((Constants.FACEBOOK_CATALOG_API_COOPERATION_OPTION_ENABLED == false)
					&& (mallKbn.Value == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_FACEBOOK)) continue;

				ddlMallKbn.Items.Add(mallKbn);
			}

			m_iPageNumber = 1;

			//------------------------------------------------------
			// 初期検索
			//------------------------------------------------------
			SearchMallList();
		}
	}

	/// <summary>
	/// モール設定情報一覧取得
	/// </summary>
	private void SearchMallList()
	{
		//------------------------------------------------------
		// リストを設定
		//------------------------------------------------------
		List<Hashtable> lMallLists = new List<Hashtable>();
		DataView dvMallList = GetMallList();
		foreach (DataRowView drvMallList in dvMallList)
		{
			Hashtable htRow = new Hashtable();
			htRow.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, drvMallList[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]);
			htRow.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME, drvMallList[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]);

			ListItem li = ddlMallKbn.Items.FindByValue((string)drvMallList[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN]);
			if (li != null)
			{
				htRow["mall_kbn_view"] = li.Text;
			}
			var mallExhibitsConfig = StringUtility.ToEmpty(drvMallList[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG]);
			htRow.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN, drvMallList[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN]);
			htRow.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG,
				(string.IsNullOrEmpty(mallExhibitsConfig) == false)
					? ValueText.GetValueText(
						Constants.TABLE_MALLCOOPERATIONSETTING,
						Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG,
						drvMallList[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG].ToString())
					//「全ての商品を出品」
					: ValueText.GetValueText(
						Constants.TABLE_MALLCOOPERATIONSETTING,
						Constants.VALUETEXT_PARAM_TEXT_MALL_COOPERATION_SETTING,
						Constants.VALUETEXT_PARAM_ALL_PRODUCT));
			htRow.Add(Constants.FIELD_MALLCOOPERATIONSETTING_VALID_FLG, (((string)drvMallList[Constants.FIELD_MALLCOOPERATIONSETTING_VALID_FLG] == "0") ? "－" : "○"));

			lMallLists.Add(htRow);
		}

		rMallCooperationSettingList.DataSource = lMallLists;
		rMallCooperationSettingList.DataBind();

		// 件数取得、エラー表示制御
		if (dvMallList.Count != 0)
		{
			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string strNextUrl = CreateProductConverterListUrl(m_iPageNumber);
		lbPager1.Text = (dvMallList.Count > 0) ? WebPager.CreateDefaultListPager((int)dvMallList[0]["row_count"], m_iPageNumber, strNextUrl) : WebPager.CreateDefaultListPager(0, m_iPageNumber, strNextUrl);
	}

	/// <summary>
	/// モール設定情報一覧取得
	/// </summary>
	private DataView GetMallList()
	{
		// モール設定情報を取得する
		DataView dvResult = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "GetMallCooperationSettingList"))
		{
			Hashtable htSearch = new Hashtable();
			htSearch.Add(Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, this.LoginOperatorShopId);
			htSearch.Add("bgn_row_num", PAGE_MAX_ROW * (m_iPageNumber - 1));
			htSearch.Add("end_row_num", PAGE_MAX_ROW * m_iPageNumber);
			htSearch.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbMallId.Text));
			htSearch.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbMallName.Text));
			htSearch.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN, ddlMallKbn.SelectedItem.Value);

			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSearch);
		}

		// ソートする
		dvResult.Sort = ddlSort.SelectedValue;

		return dvResult;
	}

	/// <summary>
	/// 新規登録 ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_MALL_CONFIG);
		sbUrl.Append("?");
		sbUrl.Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_INSERT);

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 検索 ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// モール連携設定を検索する
		SearchMallList();
	}
}
