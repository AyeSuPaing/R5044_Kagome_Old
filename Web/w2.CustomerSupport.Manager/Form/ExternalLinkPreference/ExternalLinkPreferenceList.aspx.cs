/*
=========================================================================================================
  Module      : 外部リンク設定リストページ(ExternalLinkPreferecneList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI.WebControls;
using w2.Domain.ExternalLink;
using w2.Common.Web;

public partial class Form_ExternalLinkPerference_ExternalLinkPreferecneList : BasePage
{

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

			// 外部リンク情報一覧表示
			Display();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 有効フラグドロップダウン作成
		ddlValidFlg.Items.Add(new ListItem("", ""));
		ddlValidFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSEXTERNALLINK, Constants.FIELD_CSEXTERNALLINK_VALID_FLG));
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	private void Display()
	{
		// 検索パラメータセット
		tbExternalLinkTitle.Text = this.RequestTitle;
		ddlValidFlg.SelectedValue = this.RequestValidFlg;

		// 外部リンク情報一覧取得
		var bgnRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1;
		var endRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum);
		var service = new CsExternalLinkService();
		var models = service.Search(
			this.LoginOperatorDeptId,
			this.RequestLinkId,
			this.RequestTitle,
			this.RequestValidFlg,
			bgnRow,
			endRow);
		rList.DataSource = models;
		rList.DataBind();

		// 件数取得、エラー表示制御
		int totalCount;
		if (models.Length != 0)
		{
			totalCount = models[0].SearchCount;
			trListError.Visible = false;
		}
		else
		{
			totalCount = 0;
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// ページ作成
		var nextUrl = CreateExternalLinkUrl(
			Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_LIST,
			this.RequestTitle,
			this.RequestValidFlg);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);
	}

    /// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="externalLinkId">リンクID</param>
	/// <returns>外部リンク情報詳細URL</returns>
	protected string CreateDetailUrl(string externalLinkId)
	{
		var url = CreateExternalLinkUrl(Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_CONFIRM, externalLinkId);
		return url;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// 外部リンク情報一覧へ
		var url = CreateExternalLinkUrl(
			Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_LIST,
			tbExternalLinkTitle.Text,
			ddlValidFlg.SelectedValue);
		Response.Redirect(url);
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 新規登録画面へ
		Response.Redirect(CreateExternalLinkUrl(Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_REGISTER));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="buttonActionStatus">ボタンステータス</param>
	/// <param name="externalInfo">リンク情報</param>
	/// <param name="validFlg">フラグ</param>
	/// <returns></returns>
	private string CreateExternalLinkUrl(string buttonActionStatus, string externalInfo = null, string validFlg = null)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + buttonActionStatus);
		switch (buttonActionStatus)
		{
			case Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_REGISTER:
				// 処理区分をセッションへ格納
				Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

				// 新規登録画面へ
				url.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT);
				break;

			case Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_LIST:
				// 外部リンク情報一覧URL
				url.AddParam(Constants.REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_TITLE, externalInfo)
					.AddParam(Constants.REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_VALID_FLG, validFlg);
				break;

			case Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_CONFIRM:
				// 外部リンク情報詳細URL
				url.AddParam(Constants.REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_LINK_ID, externalInfo)
					.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL);
				break;
		}
		return url.CreateUrl();
	}

	#region プロパティ
	/// <summary>リクエスト：外部リンク設定用タイトル</summary>
	private string RequestTitle
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_TITLE]); }
	}
	/// <summary>リクエスト：有効フラグ</summary>
	private string RequestValidFlg
	{
		get
		{
			string validFlg = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_VALID_FLG]);
			switch (validFlg)
			{
				case "":												// 指定なし
				case Constants.FLG_CSEXTERNALLINK_VALID_FLG_VALID:		// 有効
				case Constants.FLG_CSEXTERNALLINK_VALID_FLG_INVALID:	// 無効
					return validFlg;
				default:
					return "";
			}
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
	/// <summary>外部リンクID</summary>
	private string RequestLinkId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_LINK_ID]); }
	}
	#endregion
}
