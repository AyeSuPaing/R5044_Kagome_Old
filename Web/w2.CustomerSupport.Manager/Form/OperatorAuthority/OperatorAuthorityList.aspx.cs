/*
=========================================================================================================
  Module      : オペレータ権限設定一覧ページ処理(OperatorAuthorityList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using w2.App.Common.Cs.CsOperator;

public partial class Form_OperatorAuthority_OperatorAuthorityList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 一覧取得
			var service = new CsOperatorAuthorityService(new CsOperatorAuthorityRepository());
			var results = service.GetAll(this.LoginOperatorDeptId);
			operatorAuthorityList.DataSource = results;
			operatorAuthorityList.DataBind();

			// エラー表示制御
			if (results.Length != 0)
			{
				trListError.Visible = false;
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}
		}
	}

	/// <summary>
	/// 詳細画面URL作成
	/// </summary>
	/// <param name="operatorAuthorityId">オペレータ権限ID</param>
	/// <returns>詳細画面URL</returns>
	protected string CreateDetailUrl(string operatorAuthorityId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_CONFIRM
			+ "?" + Constants.REQUEST_KEY_OPERATOR_AUTHORITY_ID + "=" + HttpUtility.UrlEncode(operatorAuthorityId)
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNew_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}
}
