/*
=========================================================================================================
  Module      : 注文メモ設定一覧ページ処理(OrderMemoList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_OrderMemoInfo_OrderMemoList : BasePage
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
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponent();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			int iPage;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out iPage) == false)
			{
				iPage = 1;
			}
			this.CurrentPageNumber = iPage;
			ViewState["CurrentPageNumber"] = this.CurrentPageNumber;

			//------------------------------------------------------
			// 検索情報保持
			//------------------------------------------------------
			Hashtable htParam = new Hashtable();
			htParam.Add(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_ORDER_MEMO_ID]));
			// 検索条件復元用
			htParam.Add(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMO_ID]));
			htParam.Add(Constants.FIELD_ORDERMEMOSETTING_DISPLAY_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMO_DISP_KBN]));
			htParam.Add(Constants.FIELD_ORDERMEMOSETTING_VALID_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMO_VALID_FLG]));
			htParam.Add(Constants.REQUEST_KEY_PAGE_NO, this.CurrentPageNumber);

			//------------------------------------------------------
			// 一覧表示
			//------------------------------------------------------
			int iTotal = GetAffiliateTagList(htParam);

			//------------------------------------------------------
			// ページャ作成
			//------------------------------------------------------
			string strNextUrl = CreateListUrl(htParam);
			lbPager.Text = WebPager.CreateDefaultListPager(iTotal, this.CurrentPageNumber, strNextUrl);
		}
		else
		{
			this.CurrentPageNumber = (int)ViewState["CurrentPageNumber"];
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		// 表示区分
		ddlDispKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERMEMOSETTING, Constants.FIELD_ORDERMEMOSETTING_DISPLAY_KBN))
		{
			ddlDispKbn.Items.Add(li);
		}
		ddlDispKbn.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMO_DISP_KBN]);

		// 有効フラグドロップダウンにセット
		ddlValidFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERMEMOSETTING, Constants.FIELD_ORDERMEMOSETTING_VALID_FLG))
		{
			ddlValidFlg.Items.Add(li);
		}
		ddlValidFlg.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMO_VALID_FLG]);

		// 注文メモID
		tbOrderMemoId.Text = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMO_ID]);
	}

	/// <summary>
	/// 一覧表示
	/// </summary>
	/// <param name="htSearchInfo">検索条件</param>
	/// <returns>総件数</returns>
	private int GetAffiliateTagList(Hashtable htSearchInfo)
	{
		int iTotal = 0;

		//------------------------------------------------------
		// 一覧データ取得
		//------------------------------------------------------
		DataView dvOrderMemo = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("OrderMemoSetting", "GetOrderMemoSettingList"))
		{
			htSearchInfo.Add("bgn_row_num", (this.CurrentPageNumber - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1);
			htSearchInfo.Add("end_row_num", this.CurrentPageNumber * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);

			dvOrderMemo = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSearchInfo);
		}

		//------------------------------------------------------
		// 画面へセット
		//------------------------------------------------------
		if (dvOrderMemo.Count != 0)
		{
			rOrderMemoList.DataSource = dvOrderMemo;
			rOrderMemoList.DataBind();

			this.OrderMemoIdListOfDisplayedData = dvOrderMemo.Cast<DataRowView>()
				.Select(drv => (string)drv[Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID]).ToArray();

			// エラー表示制御
			trListError.Visible = false;
			iTotal = int.Parse(dvOrderMemo[0]["row_count"].ToString());
		}
		else
		{
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

			this.OrderMemoIdListOfDisplayedData = new string[0];
		}

		return iTotal;
	}

	/// <summary>
	/// 一覧遷移URL作成
	/// </summary>
	/// <param name="iPageNumber">表示開始ページ番号/param>
	/// <param name="htSearchInfo">検索情報</param>
	/// <returns>注文メモ情報一覧URL</returns>
	private string CreateListUrl(int iPageNumber, Hashtable htSearchInfo)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(CreateListUrl(htSearchInfo));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_PAGE_NO);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode(iPageNumber.ToString()));

		return sbResult.ToString();
	}
	/// <summary>
	/// 一覧URL作成処理
	/// </summary>
	/// <param name="htSearchInfo">検索情報</param>
	/// <returns>アフィリエイトタグ設定一覧URL</returns>
	private string CreateListUrl(Hashtable htSearchInfo)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT);
		sbResult.Append(Constants.PAGE_MANAGER_ORDER_MEMO_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_ORDER_MEMO_ID).Append("=").Append(HttpUtility.UrlEncode((string)htSearchInfo[Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ORDER_MEMO_DISP_KBN).Append("=").Append(HttpUtility.UrlEncode((string)htSearchInfo[Constants.FIELD_ORDERMEMOSETTING_DISPLAY_KBN]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ORDER_MEMO_VALID_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htSearchInfo[Constants.FIELD_ORDERMEMOSETTING_VALID_FLG]));

		return sbResult.ToString();
	}

	/// <summary>
	/// 詳細URL作成処理
	/// </summary>
	/// <param name="strOrderMemoId">注文メモID</param>
	/// <returns>注文メモID登録／編集URL</returns>
	protected string CreateDetailUrl(string strOrderMemoId)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_ORDER_MEMO_CONFIRM);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_ORDER_MEMO_ID).Append("=").Append(HttpUtility.UrlEncode(strOrderMemoId));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));

		return sbResult.ToString();
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MEMO_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// ページ番号のみ1ページ目に戻し、検索値をセットしてリダイレクト
		Hashtable htParam = new Hashtable();
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID, tbOrderMemoId.Text);
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_DISPLAY_KBN, ddlDispKbn.SelectedValue);
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_VALID_FLG, ddlValidFlg.SelectedValue);

		Response.Redirect(CreateListUrl(1, htParam));
	}

	/// <summary>
	/// 翻訳設定情報出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = this.OrderMemoIdListOfDisplayedData;
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}

	/// <summary>カレントページ番号</summary>
	public int CurrentPageNumber { get; set; }
	/// <summary>画面表示されている注文メモ設定IDリスト</summary>
	private string[] OrderMemoIdListOfDisplayedData
	{
		get { return (string[])ViewState["ordermemoid_list_of_displayed_data"]; }
		set { ViewState["ordermemoid_list_of_displayed_data"] = value; }
	}
}
