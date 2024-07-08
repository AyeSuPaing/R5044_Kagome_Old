/*
=========================================================================================================
  Module      : オペレータ権限設定確認ページ処理(OperatorAuthorityConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Text;
using w2.App.Common.Cs.CsOperator;

public partial class Form_OperatorAuthority_OperatorAuthorityConfirm : BasePage
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
			// 画面初期化
			Initialize();

			// オペレータ権限情報表示
			DisplayOperatorAuthorityInfo();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_DETAIL:	// 詳細表示
				btnEditTop.Visible = true;
				btnEditBottom.Visible = true;

				// 削除ボタン表示
				btnDeleteTop.Visible = true;
				btnDeleteBottom.Visible = true;

				break;

			case Constants.ACTION_STATUS_INSERT:	// 新規登録
				btnInsertTop.Visible = true;
				btnInsertBottom.Visible = true;
				break;

			case Constants.ACTION_STATUS_UPDATE:	// 編集確認
				btnUpdateTop.Visible = true;
				btnUpdateBottom.Visible = true;
				break;
		}
	}

	/// <summary>
	/// オペレータ権限情報表示
	/// </summary>
	private void DisplayOperatorAuthorityInfo()
	{
		// 画面設定処理
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				this.OperatorAuthorityInfo = (CsOperatorAuthorityModel)Session[Constants.SESSION_KEY_OPERATORAUTHORITY_INFO];
				break;

			case Constants.ACTION_STATUS_DETAIL:
				var service = new CsOperatorAuthorityService(new CsOperatorAuthorityRepository());
				this.OperatorAuthorityInfo = service.Get(this.LoginOperatorDeptId, Request[Constants.REQUEST_KEY_OPERATOR_AUTHORITY_ID]);
				if (this.OperatorAuthorityInfo == null)
				{
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				break;

			default:
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		this.OperatorAuthorityInfo.OperatorAuthorityId = NumberingUtility.CreateNewNumber(this.LoginOperatorShopId, Constants.NUMBER_KEY_CS_OPERATOR_AUTHORITY_ID).ToString().PadLeft(Constants.CONST_CS_OPERATOR_AUTHORITY_ID_LENGTH, '0');
		this.OperatorAuthorityInfo.DeptId = this.LoginOperatorDeptId;
		this.OperatorAuthorityInfo.LastChanged = this.LoginOperatorName;

		var service = new CsOperatorAuthorityService(new CsOperatorAuthorityRepository());
		service.Register(this.OperatorAuthorityInfo);

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_LIST);
	}


	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		this.OperatorAuthorityInfo.DeptId = this.LoginOperatorDeptId;
		this.OperatorAuthorityInfo.LastChanged = this.LoginOperatorName;

		var service = new CsOperatorAuthorityService(new CsOperatorAuthorityRepository());
		service.Update(this.OperatorAuthorityInfo);


		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_LIST);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// CSオペレータ権限情報に設定されていないかチェック
		DataView csOperatorAuthorityData = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("CsOperatorAuthority", "CheckCsOperatorAuthorityUsed"))
		{
			Hashtable input = new Hashtable
			{
				{Constants.FIELD_CSOPERATOR_DEPT_ID, this.LoginOperatorDeptId},
				{Constants.FIELD_CSOPERATOR_OPERATOR_AUTHORITY_ID, this.OperatorAuthorityInfo.OperatorAuthorityId},
			};
			csOperatorAuthorityData = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
		}

		// 設定されていたら削除させない、エラーページへ
		if (csOperatorAuthorityData.Count != 0)
		{
			StringBuilder errMsg = new StringBuilder();
			errMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CSOPERATORAUTHORITY_DELETE_IMPOSSIBLE_ERROR));
			Session[Constants.SESSION_KEY_ERROR_MSG] = errMsg.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		var service = new CsOperatorAuthorityService(new CsOperatorAuthorityRepository());
		service.Delete(this.LoginOperatorDeptId, this.OperatorAuthorityInfo.OperatorAuthorityId);

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_LIST);
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 処理区分・パラメタをセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;
		Session[Constants.SESSION_KEY_OPERATORAUTHORITY_INFO] = this.OperatorAuthorityInfo;

		// 新規登録画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}

	#region プロパティ
	/// <summary>オペレータ権限情報</summary>
	protected CsOperatorAuthorityModel OperatorAuthorityInfo
	{
		get { return (CsOperatorAuthorityModel)ViewState[Constants.SESSION_KEY_OPERATORAUTHORITY_INFO]; }
		private set { ViewState[Constants.SESSION_KEY_OPERATORAUTHORITY_INFO] = value; }
	}
	#endregion
}
