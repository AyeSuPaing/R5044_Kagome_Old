/*
=========================================================================================================
  Module      : 拠点グループ設定登録ページ処理(CsGroupRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;

public partial class Form_CsGroup_CsGroupRegister : BasePage
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
			// 初期化処理
			Initialize();

			// グループ情報表示
			DisplayGroupInfo();

			// データバインド
			DataBind();
		}
		else
		{
			// 画面入力値を元に編集モデル更新
			this.EditModel.CsGroupName = tbCsGroupName.Text;
			this.EditModel.DisplayOrder = int.Parse(ddlDisplayOrder.SelectedValue);
			this.EditModel.ValidFlg = chkValidFlg.Checked ? Constants.FLG_CSGROUP_VALID_FLG_VALID : Constants.FLG_CSGROUP_VALID_FLG_INVALID;
		}
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	private void Initialize()
	{
		// 表示順ドロップダウン作成
		for (int i = 1; i <= 100; i++)
		{
			ddlDisplayOrder.Items.Add(new ListItem(i.ToString(), i.ToString()));
		}
	}

	/// <summary>
	/// グループ情報表示
	/// </summary>
	private void DisplayGroupInfo()
	{
		// グループリスト表示
		CsGroupService service = new CsGroupService(new CsGroupRepository());
		CsGroupModel[] groupList = service.GetAll(this.LoginOperatorDeptId);
		if (groupList.Length == 0)
		{
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		rList.DataSource = groupList;

		// グループ詳細情報表示
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:	// 新規登録
				DispControlInsert();
				break;

			case Constants.ACTION_STATUS_UPDATE:	// 更新
				DispControlUpdate();
				break;

			case Constants.ACTION_STATUS_COMPLETE:	// 完了
				DispControlComplete();
				break;

			default:								// 未選択
				DispControlNoSelect();
				break;
		}
	}

	/// <summary>
	/// 登録時の表示制御
	/// </summary>
	private void DispControlInsert()
	{
		// 登録画面表示切替
		trNotSelect.Visible = false;
		trEdit.Visible = true;
		btnRegisterBottom.Visible = true;

		// 編集用モデルの初期値を作る
		this.EditModel = new CsGroupModel();
		this.EditModel.DeptId = this.LoginOperatorDeptId;
		this.EditModel.CsGroupId = string.Empty;
		this.EditModel.CsGroupName = string.Empty;
		this.EditModel.DisplayOrder = 1;
		this.EditModel.ValidFlg = Constants.FLG_CSGROUP_VALID_FLG_VALID;
		this.EditModel.LastChanged = this.LoginOperatorName;
	}

	/// <summary>
	/// 編集時の表示制御
	/// </summary>
	private void DispControlUpdate()
	{
		// 編集画面表示切替
		trNotSelect.Visible = false;
		trEdit.Visible = true;
		btnEditBottom.Visible = true;
		btnDeleteBottom.Visible = true;

		// 編集用モデルをDBからとってくる
		CsGroupService service = new CsGroupService(new CsGroupRepository());
		this.EditModel = service.Get(this.LoginOperatorDeptId, this.GroupId);
		if (this.EditModel == null)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 最終更新者をセット
		this.EditModel.LastChanged = this.LoginOperatorName;
	}

	/// <summary>
	/// 完了時の表示制御
	/// </summary>
	private void DispControlComplete()
	{
		trEdit.Visible = true;
		btnEditBottom.Visible = true;
		btnDeleteBottom.Visible = true;
		divErrorMessage.Visible = false;
		divComp.Visible = true;

		// 編集用モデルをDBからとってくる
		CsGroupService service = new CsGroupService(new CsGroupRepository());
		this.EditModel = service.Get(this.LoginOperatorDeptId, this.GroupId);
		if (this.EditModel == null)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 未選択時の表示制御
	/// </summary>
	private void DispControlNoSelect()
	{
		divErrorMessage.Visible = true;
		divComp.Visible = false;
		divNotSelectMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CSGROUP_NO_SELECTED_ERROR);

		// 空のモデルを作る
		this.EditModel = new CsGroupModel();
		this.EditModel.DisplayOrder = 1; // エラーになるので
	}

	/// <summary>
	/// 新規登録ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 登録画面へ遷移
		Response.Redirect(CreateRegisterUrl());
	}

	/// <summary>
	/// 登録するボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegister_Click(object sender, System.EventArgs e)
	{
		// 入力チェック
		Hashtable input = CreateInputHash();
		string errorMessage = Validator.Validate("CsGroupRegister", input);
		if (errorMessage != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 登録処理
		CsGroupService service = new CsGroupService(new CsGroupRepository());
		service.Register(this.EditModel);

		// 完了画面へ遷移
		Response.Redirect(CreateCompleteUrl(this.EditModel.CsGroupId));
	}

	/// <summary>
	/// 更新するボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 入力チェック
		Hashtable input = CreateInputHash();
		string errorMessages = Validator.Validate("CsGroupModify", input);
		if (errorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 更新処理
		CsGroupService service = new CsGroupService(new CsGroupRepository());
		service.Update(this.EditModel);

		// 完了画面へ遷移
		Response.Redirect(CreateCompleteUrl(this.EditModel.CsGroupId));
	}

	/// <summary>
	/// 入力チェック用の入力情報作成
	/// </summary>
	/// <returns>Hashtableデータ</returns>
	private Hashtable CreateInputHash()
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_CSGROUP_DEPT_ID, this.EditModel.DeptId);							// 識別ID
		input.Add(Constants.FIELD_CSGROUP_CS_GROUP_NAME, this.EditModel.CsGroupName);				// グループ名
		input.Add(Constants.FIELD_CSGROUP_DISPLAY_ORDER, this.EditModel.DisplayOrder.ToString());	// 表示順
		input.Add(Constants.FIELD_CSGROUP_VALID_FLG, this.EditModel.ValidFlg);						// 有効フラグ
		return input;
	}

	/// <summary>
	/// 削除するボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 削除
		CsGroupService service = new CsGroupService(new CsGroupRepository());
		service.Delete(this.EditModel.DeptId, this.EditModel.CsGroupId);

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_CSGROUP_REGISTER);
	}

	#region #CreateRegisterUrl 登録URL作成
	/// <summary>
	/// 登録URL作成
	/// </summary>
	/// <returns>CSグループ情報登録画面URL</returns>
	protected string CreateRegisterUrl()
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_CSGROUP_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT;
	}
	#endregion

	#region #CreateEditUrl 編集URL作成
	/// <summary>
	/// 編集URL作成
	/// </summary>
	/// <param name="groupId">CSグループID</param>
	/// <returns>CSグループ情報編集画面URL</returns>
	protected string CreateEditUrl(string groupId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_CSGROUP_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE
			+ "&" + Constants.REQUEST_KEY_CSGROUP_ID + "=" + StringUtility.ToEmpty(groupId);
	}
	#endregion

	#region #CreateCompleteUrl 完了URL作成
	/// <summary>
	/// 完了URL作成
	/// </summary>
	/// <param name="groupId">CSグループID</param>
	/// <returns>CSグループ情報完了画面URL</returns>
	protected string CreateCompleteUrl(string groupId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_CSGROUP_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COMPLETE
			+ "&" + Constants.REQUEST_KEY_CSGROUP_ID + "=" + StringUtility.ToEmpty(groupId);
	}
	#endregion

	#region プロパティ
	/// <summary>
	/// グループIDプロパティ
	/// </summary>
	protected string GroupId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CSGROUP_ID]); }
	}
	/// <summary>
	/// 編集モデルプロパティ
	/// </summary>
	protected CsGroupModel EditModel
	{
		get { return ViewState["EditModel"] == null ? null : (CsGroupModel)ViewState["EditModel"]; }
		set { ViewState["EditModel"] = value; }
	}
	#endregion
}
