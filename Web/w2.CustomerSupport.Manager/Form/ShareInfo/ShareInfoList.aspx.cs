/*
=========================================================================================================
  Module      : 共有情報一覧ページ処理(ShareInfoList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.Common.Extensions;
using w2.App.Common.Cs.ShareInfo;
using w2.App.Common.Cs.CsOperator;

public partial class Form_ShareInfo_ShareInfoList : BasePage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 初期化
			Initialize();

			// レイアウト上、マスタページの閉じるボタンを表示しない
			((Form_Common_PopupPage)Master).HideCloseButton = true;

			// 共有情報一覧表示
			DisplayShareInfoList();
		}
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 区分ドロップダウン作成
		ddlInfoKbn.Items.Add(new ListItem("", ""));
		ddlInfoKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSSHAREINFO, Constants.FIELD_CSSHAREINFO_INFO_KBN));

		// 重要度ドロップダウン作成
		ddlImportance.Items.Add(new ListItem("", ""));
		ddlImportance.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSSHAREINFO, Constants.FIELD_CSSHAREINFO_INFO_IMPORTANCE));

		// 送信元ドロップダウン作成
		var operatorService = new CsOperatorService(new CsOperatorRepository());
		var operators = operatorService.GetValidAll(this.LoginOperatorDeptId);
		ddlSender.Items.Add(new ListItem("", ""));
		ddlSender.Items.AddRange((from o in operators select new ListItem(o.EX_ShopOperatorName, o.OperatorId)).ToArray());
	}
	#endregion

	#region -DisplayShareInfoList 共有情報一覧表示
	/// <summary>
	/// 共有情報一覧表示
	/// </summary>
	private void DisplayShareInfoList()
	{
		// 検索パラメータセット
		tbInfoText.Text = this.InfoText;
		foreach (ListItem li in ddlInfoKbn.Items) li.Selected = (li.Value == this.InfoKbn);
		foreach (ListItem li in ddlImportance.Items) li.Selected = (li.Value == this.Importance);
		foreach (ListItem li in ddlSender.Items) li.Selected = (li.Value == this.SenderId);
		foreach (ListItem li in rblReadKbn.Items) li.Selected = (li.Value == this.ReadKbn);
		foreach (ListItem li in ddlSortKbn.Items) li.Selected = (li.Value == this.SortKbn);

		// 共有情報一覧取得・表示
		var service = new CsShareInfoReadService(new CsShareInfoReadRepository());
		string readFlg = (this.ReadKbn == Constants.KBN_READ_SHAREINFO_LIST_READING || this.ReadKbn == Constants.KBN_READ_SHAREINFO_LIST_UNREAD) ? Constants.FLG_CSSHAREINFOREAD_READ_FLG_UNREAD : "";
		string pinnedFlg = (this.ReadKbn == Constants.KBN_READ_SHAREINFO_LIST_READING) ? Constants.FLG_CSSHAREINFOREAD_PINNED_FLG_PINNED : (this.ReadKbn == Constants.KBN_READ_SHAREINFO_LIST_UNREAD) ? "-" : "";
		var models = service.SearchWithShareInfo(this.LoginOperatorDeptId, this.LoginOperatorId, this.InfoText, this.SenderId, this.Importance, this.InfoKbn, readFlg, pinnedFlg, this.SortKbn, this.PageNo);
		rList.DataSource = models;
		rList.DataBind();

		// 件数取得、エラー表示制御
		int totalCount;
		if (models.Length != 0)
		{
			totalCount = models[0].EX_RowCount;
		}
		else
		{
			totalCount = 0;
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// ページャ作成
		string nextUrl = CreateListUrl(this.InfoText, this.SenderId, this.Importance, this.InfoKbn, this.ReadKbn, this.SortKbn);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.PageNo, nextUrl);
	}
	#endregion

	#region -CreateListUrl 一覧URL作成
	/// <summary>
	/// 一覧URL作成
	/// </summary>
	/// <param name="infoText">共有情報テキスト</param>
	/// <param name="senderId">送信元オペレータID</param>
	/// <param name="importance">重要度</param>
	/// <param name="infoKbn">区分</param>
	/// <param name="readedFlg">既読フラグ</param>
	/// <param name="sortKbn">ソート区分</param>
	/// <returns>一覧URL</returns>
	private static string CreateListUrl(string infoText, string senderId, string importance, string infoKbn, string readedFlg, string sortKbn)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFO_LIST
			+ "?" + Constants.REQUEST_KEY_SHAREINFO_TEXT + "=" + HttpUtility.UrlEncode(infoText)
			+ "&" + Constants.REQUEST_KEY_SHAREINFO_SENDER + "=" + HttpUtility.UrlEncode(senderId)
			+ "&" + Constants.REQUEST_KEY_SHAREINFO_IMPORTANCE + "=" + HttpUtility.UrlEncode(importance)
			+ "&" + Constants.REQUEST_KEY_SHAREINFO_KBN + "=" + HttpUtility.UrlEncode(infoKbn)
			+ "&" + Constants.REQUEST_KEY_SHAREINFO_READKBN + "=" + HttpUtility.UrlEncode(readedFlg)
			+ "&" + Constants.REQUEST_KEY_SORT_KBN + "=" + HttpUtility.UrlEncode(sortKbn);
	}
	#endregion

	#region #CreateDetailUrl 詳細URL作成
	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="infoNo">共有情報NO</param>
	/// <returns>共有情報詳細URL</returns>
	protected string CreateDetailUrl(long infoNo)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFO_CONFIRM
			+ "?" + Constants.REQUEST_KEY_INFO_NO + "=" + infoNo.ToString()
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE;
	}
	#endregion

	#region #btnSearch_Click 検索ボタンクリック
	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		Response.Redirect(CreateListUrl(tbInfoText.Text, ddlSender.SelectedValue, ddlImportance.SelectedValue, ddlInfoKbn.SelectedValue, rblReadKbn.SelectedValue, ddlSortKbn.SelectedValue));
	}
	#endregion

	#region プロパティ
	/// <summary>共有テキスト</summary>
	private string InfoText
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHAREINFO_TEXT]); }
	}
	/// <summary>区分</summary>
	private string InfoKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHAREINFO_KBN]); }
	}
	/// <summary>重要度</summary>
	private string Importance
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHAREINFO_IMPORTANCE]); }
	}
	/// <summary>送信元オペレータ</summary>
	private string SenderId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHAREINFO_SENDER]); }
	}
	/// <summary>既読区分</summary>
	private string ReadKbn
	{
		get
		{
			string readKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHAREINFO_READKBN]);
			switch (readKbn)
			{
				case Constants.KBN_READ_SHAREINFO_LIST_READING:	// 未確認/ピン止め
				case Constants.KBN_READ_SHAREINFO_LIST_UNREAD:	// 未確認
				case Constants.KBN_READ_SHAREINFO_LIST_ALL:		// 全て表示
					return readKbn;

				case "":
				default:
					return Constants.KBN_READ_SHAREINFO_LIST_DEFAULT;
			}
		}
	}
	/// <summary>ソート区分</summary>
	private string SortKbn
	{
		get
		{
			string sortKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]);
			switch (sortKbn)
			{
				case Constants.KBN_SORT_SHAREINFO_LIST_DATE_CREATED_ASC:				// 作成日/昇順
				case Constants.KBN_SORT_SHAREINFO_LIST_DATE_CREATED_DESC:				// 作成日/降順
				case Constants.KBN_SORT_SHAREINFO_LIST_IMPORTANCE_ASC:					// 重要度/昇順
				case Constants.KBN_SORT_SHAREINFO_LIST_IMPORTANCE_DESC:					// 重要度/降順
					return sortKbn;

				case "":
				default:
					return Constants.KBN_SORT_SHAREINFO_LIST_DEFAULT;
			}
		}
	}
	/// <summary>ページ番号</summary>
	private int PageNo
	{
		get
		{
			int pageNo;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) ? pageNo : 1;
		}
	}
	#endregion
}
