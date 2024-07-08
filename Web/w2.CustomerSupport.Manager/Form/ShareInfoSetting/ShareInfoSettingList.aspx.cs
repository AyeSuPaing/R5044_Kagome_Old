/*
=========================================================================================================
  Module      : 共有情報管理一覧ページ処理(ShareInfoSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.ShareInfo;

public partial class Form_ShareInfoSetting_ShareInfoSettingList : ShareInfoSettingPage
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
			// 画面初期化
			Initialize();

			// 共有情報一覧表示
			DisplayShareInfoSettingList();
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
		var service = new CsOperatorService(new CsOperatorRepository());
		var operators = service.GetValidAll(this.LoginOperatorDeptId);
		ddlSenders.Items.Add(new ListItem("", ""));
		ddlSenders.Items.AddRange((from o in operators select new ListItem(o.EX_ShopOperatorName, o.OperatorId)).ToArray());
	}
	#endregion

	#region -DisplayShareInfoSettingList 共有情報一覧表示
	/// <summary>
	/// 共有情報一覧表示
	/// </summary>
	private void DisplayShareInfoSettingList()
	{
		// 検索パラメータセット
		tbInfoText.Text = this.InfoText;
		foreach (ListItem li in ddlInfoKbn.Items) li.Selected = (li.Value == this.InfoKbn);
		foreach (ListItem li in ddlImportance.Items) li.Selected = (li.Value == this.Importance);
		foreach (ListItem li in ddlSenders.Items) li.Selected = (li.Value == this.SenderId);
		foreach (ListItem li in ddlSortKbn.Items) li.Selected = (li.Value == this.SortKbn);

		// 一覧表示
		var service = new CsShareInfoService(new CsShareInfoRepository());
		var models = service.Search(this.LoginOperatorDeptId, this.InfoNo, this.InfoText, this.InfoKbn, this.Importance, this.SenderId, this.SortKbn, this.PageNo);
		rList.DataSource = models;
		rList.DataBind();

		// 件数取得、表示制御
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
		string nextUrl = CreateListUrl(StringUtility.ToEmpty(this.InfoNo), this.InfoText, this.SenderId, this.Importance, this.InfoKbn, this.SortKbn);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.PageNo, nextUrl);
	}
	#endregion

	#region -CreateListUrl 一覧URL作成
	/// <summary>
	/// 一覧URL作成
	/// </summary>
	/// <param name="infoNo">共有情報No</param>
	/// <param name="infoText">共有情報テキスト</param>
	/// <param name="senderId">登録オペレータID</param>
	/// <param name="importance">重要度</param>
	/// <param name="infoKbn">共有情報区分</param>
	/// <param name="sortKbn">ソート区分</param>
	/// <returns>共有情報一覧遷移URL</returns>
	private string CreateListUrl(string infoNo, string infoText, string senderId, string importance, string infoKbn, string sortKbn)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_LIST
			+ "?" + Constants.REQUEST_KEY_INFO_NO + "=" + HttpUtility.UrlEncode(infoNo)
			+ "&" + Constants.REQUEST_KEY_SHAREINFO_TEXT + "=" + HttpUtility.UrlEncode(infoText)
			+ "&" + Constants.REQUEST_KEY_SHAREINFO_SENDER + "=" + HttpUtility.UrlEncode(senderId)
			+ "&" + Constants.REQUEST_KEY_SHAREINFO_IMPORTANCE + "=" + HttpUtility.UrlEncode(importance)
			+ "&" + Constants.REQUEST_KEY_SHAREINFO_KBN + "=" + HttpUtility.UrlEncode(infoKbn)
			+ "&" + Constants.REQUEST_KEY_SORT_KBN + "=" + HttpUtility.UrlEncode(sortKbn);
	}
	#endregion

	#region #CreateDetailUrl 詳細URL作成
	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="infoNo">共有情報No</param>
	/// <returns>共有情報詳細URL</returns>
	protected string CreateDetailUrl(long infoNo)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_CONFIRM
			+ "?" + Constants.REQUEST_KEY_INFO_NO + "=" + infoNo.ToString()
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;
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
		// 共有情報一覧へ
		Response.Redirect(CreateListUrl(tbInfoNo.Text, tbInfoText.Text, ddlSenders.SelectedValue, ddlImportance.SelectedValue, ddlInfoKbn.SelectedValue, ddlSortKbn.SelectedValue));
	}
	#endregion

	#region #btnInsert_Click 新規登録ボタンクリック
	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);		
	}
	#endregion

	#region プロパティ
	/// <summary>共有情報番号</summary>
	protected long? InfoNo
	{
		get
		{
			long infoNo;
			return long.TryParse(Request[Constants.REQUEST_KEY_INFO_NO], out infoNo) ? (long?)infoNo : null;
		}
	}
	/// <summary>共有情報テキスト</summary>
	protected string InfoText
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHAREINFO_TEXT]); }
	}
	/// <summary>区分</summary>
	protected string InfoKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHAREINFO_KBN]); }
	}
	/// <summary>重要度</summary>
	protected string Importance
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHAREINFO_IMPORTANCE]); }
	}
	/// <summary>送信元オペレータ</summary>
	protected string SenderId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHAREINFO_SENDER]); }
	}
	/// <summary>ソート区分</summary>
	protected string SortKbn
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

				default:
					return Constants.KBN_SORT_SHAREINFO_LIST_DEFAULT;
			}
		}
	}
	/// <summary>ページ番号</summary>
	protected int PageNo
	{
		get
		{
			int pageNo;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) ? pageNo : 1;
		}
	}
	#endregion
}
