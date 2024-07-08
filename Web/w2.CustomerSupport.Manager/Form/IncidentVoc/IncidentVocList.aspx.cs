/*
=========================================================================================================
  Module      : VOC区分設定一覧ページ処理(IncidentVocList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Text;
using System.Web;
using w2.App.Common.Cs.IncidentVoc;

public partial class Form_IncidentVoc_IncidentVocList : BasePage
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
			// VOC情報一覧表示
			ViewVocList();
		}
	}

	/// <summary>
	/// VOC情報一覧表示
	/// </summary>
	private void ViewVocList()
	{
		// VOC情報一覧取得
		int bgnRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.PageNo - 1) + 1;
		int endRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.PageNo;
		CsIncidentVocService service = new CsIncidentVocService(new CsIncidentVocRepository());
		CsIncidentVocModel[] models = service.Search(this.LoginOperatorDeptId, bgnRow, endRow);

		// 総件数、エラー表示/非表示制御
		int totalCounts;
		if (models.Length != 0)
		{
			totalCounts = int.Parse(models[0].EX_RowCount.ToString());
			trListError.Visible = false;
		}
		else
		{
			totalCounts = 0;
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// データソースセット
		rList.DataSource = models;
		rList.DataBind();

		// ページャ作成（一覧処理で総件数を取得）
		string nextUrl = Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTVOC_LIST;
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCounts, this.PageNo, nextUrl);
	}

	/// <summary>
	/// データバインド用：VOC情報詳細URL作成
	/// </summary>
	/// <param name="vocId">VOC ID</param>
	/// <returns>VOC情報詳細URL</returns>
	protected string CreateVocDetailUrl(string vocId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTVOC_CONFIRM
			+ "?" + Constants.REQUEST_KEY_VOC_ID + "=" + vocId
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTVOC_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	#region プロパティ
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
