/*
=========================================================================================================
  Module      :ノベルティ設定一覧ページ処理(NoveltyList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.Novelty;

public partial class Form_Novelty_NoveltyList : NoveltyPage
{
	protected const string FIELD_NOVELTY_STATUS = "status";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 初期化
			Initialize();

			// ノベルティ設定一覧表示
			DisplayNoveltyList();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 開催状態
		ddlNoveltyStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlNoveltyStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_NOVELTY, Constants.REQUEST_KEY_NOVELTY_STATUS));
		// 並び順
		ddlSortKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_NOVELTY, Constants.REQUEST_KEY_SORT_KBN));
	}

	/// <summary>
	/// ノベルティ設定一覧表示
	/// </summary>
	private void DisplayNoveltyList()
	{
		// 検索フォームにパラメータをセット
		tbNoveltyId.Text = this.RequestNoveltyId;
		tbNoveltyDisplayName.Text = this.RequestNoveltyDisplayName;
		tbNoveltyName.Text = this.RequestNoveltyName;
		ddlNoveltyStatus.SelectedValue = this.RequestNoveltyStatus;
		ddlSortKbn.SelectedValue = this.RequestSortKbn;

		// ページ番号取得
		int bgnRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1;
		int endRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.RequestPageNum;

		// パラメータセット
		var param = new Hashtable
		{
			{Constants.FIELD_NOVELTY_SHOP_ID, this.LoginOperatorShopId},
			{Constants.FIELD_NOVELTY_NOVELTY_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(this.RequestNoveltyId)},
			{Constants.FIELD_NOVELTY_NOVELTY_DISP_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(this.RequestNoveltyDisplayName)},
			{Constants.FIELD_NOVELTY_NOVELTY_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(this.RequestNoveltyName)},
			{"status", this.RequestNoveltyStatus},
			{"sort_kbn", this.RequestSortKbn},
			{"bgn_row_num", bgnRow},
			{"end_row_num", endRow}
		};

		// ノベルティ設定一覧取得
		var service = new NoveltyService();
		int totalCount = service.GetSearchHitCount(param);
		var models = service.Search(param);
		rList.DataSource = models;
		rList.DataBind();

		// 件数取得、エラー表示制御
		if (totalCount != 0)
		{
			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// ページャ作成
		string nextUrl = CreateNoveltyListUrl(this.RequestNoveltyId, this.RequestNoveltyDisplayName, this.RequestNoveltyName, this.RequestNoveltyStatus, this.RequestSortKbn);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);

		// ノベルティ設定一覧検索情報格納
		this.SearchInfo = new SearchValues(this.RequestNoveltyId, this.RequestNoveltyDisplayName, this.RequestNoveltyName, this.RequestNoveltyStatus, this.RequestSortKbn, this.RequestPageNum);
		this.NoveltyIdListOfDisplayedData = models.Select(m => m.NoveltyId).ToArray();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		var url = CreateNoveltyListUrl(tbNoveltyId.Text, tbNoveltyDisplayName.Text, tbNoveltyName.Text, ddlNoveltyStatus.SelectedValue, ddlSortKbn.SelectedValue);
		Response.Redirect(url);
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		var url = CreateNoveltyRegisterUrl("", Constants.ACTION_STATUS_INSERT);
		Response.Redirect(url);
	}

	/// <summary>
	/// 翻訳データ出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = this.NoveltyIdListOfDisplayedData;
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}

	#region プロパティ
	/// <summary>リクエスト：ノベルティ名（表示用）</summary>
	private string RequestNoveltyDisplayName
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NOVELTY_DISP_NAME]).Trim(); }
	}
	/// <summary>リクエスト：ノベルティ名（管理用）</summary>
	private string RequestNoveltyName
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NOVELTY_NAME]).Trim(); }
	}
	/// <summary>リクエスト：開催状態</summary>
	private string RequestNoveltyStatus
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NOVELTY_STATUS]).Trim(); }
	}
	/// <summary>リクエスト：並び順</summary>
	private string RequestSortKbn
	{
		get
		{
			var sortKbn = Constants.KBN_SORT_NOVELTY_LIST_STATUS;
			switch (Request[Constants.REQUEST_KEY_SORT_KBN])
			{
				case Constants.KBN_SORT_NOVELTY_LIST_STATUS:
				case Constants.KBN_SORT_NOVELTY_LIST_NOVELTY_ID_ASC:
				case Constants.KBN_SORT_NOVELTY_LIST_NOVELTY_ID_DESC:
				case Constants.KBN_SORT_NOVELTY_LIST_NOVELTY_DISP_NAME_ASC:
				case Constants.KBN_SORT_NOVELTY_LIST_NOVELTY_DISP_NAME_DESC:
				case Constants.KBN_SORT_NOVELTY_LIST_NOVELTY_NAME_ASC:
				case Constants.KBN_SORT_NOVELTY_LIST_NOVELTY_NAME_DESC:
				case Constants.KBN_SORT_NOVELTY_LIST_DATE_BEGIN_ASC:
				case Constants.KBN_SORT_NOVELTY_LIST_DATE_BEGIN_DESC:
				case Constants.KBN_SORT_NOVELTY_LIST_DATE_END_ASC:
				case Constants.KBN_SORT_NOVELTY_LIST_DATE_END_DESC:
					sortKbn = Request[Constants.REQUEST_KEY_SORT_KBN];
					break;
			}
			return sortKbn;
		}
	}
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	/// <summary>画面表示されているノベルティIDリスト</summary>
	private string[] NoveltyIdListOfDisplayedData
	{
		get { return (string[])ViewState["noveltyid_list_of_displayed_data"]; }
		set { ViewState["noveltyid_list_of_displayed_data"] = value; }
	}
	#endregion
}