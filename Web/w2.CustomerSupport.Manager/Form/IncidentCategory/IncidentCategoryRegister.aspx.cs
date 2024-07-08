/*
=========================================================================================================
  Module      : インシデントカテゴリ登録ページ処理(IncidentCategoryRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.IncidentCategory;
using System.Text;

public partial class Form_IncidentCategory_IncidentCategoryRegister : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if(!IsPostBack)
		{
			// パラメータチェック
			CheckParameters();

			// 初期化
			Initialize();

			// 画面表示
			Display();
		}
	}

	/// <summary>
	/// パラメータチェック
	/// </summary>
	private void CheckParameters()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 登録
			case Constants.ACTION_STATUS_COMPLETE:		// 完了
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				break;

			case Constants.ACTION_STATUS_UPDATE:		// 編集
			case null:									// 未選択
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:	// 登録
				trNotSelect.Visible = false;
				trEdit.Visible = true;
				tdParentCategoryRegisterHead.Visible = true;
				tdParentCategoryRegister.Visible = true;
				btnIncidentCategoryRegister.Visible = true;
				cbValidFlg.Checked = true;	// 有効フラグにデフォルトチェック
				break;

			case Constants.ACTION_STATUS_UPDATE:	// 編集
				trNotSelect.Visible = false;
				trEdit.Visible = true;
				tdParentCategoryRegisterHead.Visible = true;
				tdParentCategoryRegister.Visible = true;
				btnIncidentCategoryModify.Visible = true;
				btnIncidentCategoryDelete.Visible = true;
				break;

			case Constants.ACTION_STATUS_COMPLETE:	// 完了
				trEdit.Visible = true;
				tdParentCategoryRegisterHead.Visible = true;
				tdParentCategoryRegister.Visible = true;
				btnIncidentCategoryModify.Visible = true;
				btnIncidentCategoryDelete.Visible = true;
				divErrorMessage.Visible = false;
				divComp.Visible = true;
				break;

			case null:
				divErrorMessage.Visible = true;
				divComp.Visible = false;
				divNotSelectMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_SELECTED_ERROR);
				break;
		}

		// インシデントカテゴリドロップダウン作成
		var service = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		var categories = service.GetAll(this.LoginOperatorDeptId);
		ddlParentCategory.Items.Add(new ListItem("", ""));
		ddlParentCategory.Items.AddRange(categories.Select(p => new ListItem(p.EX_CategoryNameForDropdown, p.CategoryId)).ToArray());

		// 表示順ドロップダウン作成
		for (int i = 1; i <= 100; i++)
		{
			ddlDisplayOrder.Items.Add(new ListItem(i.ToString(), i.ToString()));
		}
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	private void Display()
	{
		// インシデントカテゴリ情報取得
		CsIncidentCategoryService incidentCategoryService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		CsIncidentCategoryModel[] models = incidentCategoryService.SearchByCategoryId(this.LoginOperatorDeptId, "");

		// インシデントカテゴリ情報一覧表示
		if (models.Length != 0)
		{
			this.TotalCategoryCount = models[0].EX_RowCount;
			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		rList.DataSource = models;
		rList.DataBind();

		/// インシデントカテゴリ情報詳細表示
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_UPDATE:	// 処理区分あり：編集
			case Constants.ACTION_STATUS_COMPLETE:	// 処理区分あり：完了

				// カテゴリIDをセット
				if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				{
					this.CategoryId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CATEGORY_ID]);
				}
				else if (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)
				{
					this.CategoryId = ((CsIncidentCategoryModel)Session[Constants.SESSION_KEY_INCIDENTCATEGORY_INFO]).CategoryId;
				}

				// インシデントカテゴリマスタから取得
				var service = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
				this.CategoryInfo = service.Get(this.LoginOperatorDeptId, this.CategoryId);

				// 該当データなしの場合、エラーページへ
				if (this.CategoryInfo == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// カテゴリ情報データバインド
				trEdit.DataBind();
				break;
		}
	}

	/// <summary>
	/// 編集URL作成
	/// </summary>
	/// <param name="categoryId">インシデントカテゴリID</param>
	/// <returns>インシデントカテゴリ編集URL</returns>
	protected string CreateEditUrl(string categoryId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTCATEGORY_REGISTER
			+ "?" + Constants.REQUEST_KEY_CATEGORY_ID + "=" + categoryId
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnIncidentCategoryInsert_Click(object sender, EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 登録画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTCATEGORY_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnIncidentCategoryRegister_Click(object sender, EventArgs e)
	{
		// 入力情報チェック
		CsIncidentCategoryModel inputData = CheckInputData();

		// インシデントカテゴリID取得・情報登録
		CsIncidentCategoryService incidentCategoryService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		incidentCategoryService.Register(inputData);

		// 完了画面用に格納
		Session[Constants.SESSION_KEY_INCIDENTCATEGORY_INFO] = inputData;
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;

		// 完了画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTCATEGORY_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COMPLETE);
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnIncidentCategoryModify_Click(object sender, EventArgs e)
	{
		// 入力情報チェック
		CsIncidentCategoryModel inputData = CheckInputData();

		// 無限ループチェック
		inputData.CategoryId = this.CategoryId;	// インシデントカテゴリID
		if (CheckValidParentCategory(inputData) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_UPDATE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// インシデントカテゴリ情報更新
		CsIncidentCategoryService incidentCategoryService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		incidentCategoryService.Update(inputData);

		// 完了画面用に格納
		Session[Constants.SESSION_KEY_INCIDENTCATEGORY_INFO] = inputData;
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;

		// 完了画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTCATEGORY_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COMPLETE);
	}

	/// <summary>
	/// 入力情報チェック
	/// </summary>
	/// <returns>入力情報</returns>
	private CsIncidentCategoryModel CheckInputData()
	{
		// パラメタ格納
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_CSINCIDENTCATEGORY_PARENT_CATEGORY_ID, ddlParentCategory.SelectedValue);		// 親ID
		input.Add(Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_NAME, tbDivisionName.Text);						// インシデントカテゴリ名
		input.Add(Constants.FIELD_CSINCIDENTCATEGORY_DEPT_ID, this.LoginOperatorDeptId);						// 識別ID
		input.Add(Constants.FIELD_CSINCIDENTCATEGORY_DISPLAY_ORDER, ddlDisplayOrder.SelectedValue);				// 表示順
		input.Add(Constants.FIELD_CSINCIDENTCATEGORY_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_CSINCIDENTCATEGORY_VALID_FLG_VALID : Constants.FLG_CSINCIDENTCATEGORY_VALID_FLG_INVALID);	// 有効フラグ
		input.Add(Constants.FIELD_CSINCIDENTCATEGORY_LAST_CHANGED, this.LoginOperatorName);						// 最終更新者

		// 処理区分に応じてValidator選択
		string validator;
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				validator = "CsIncidentCategoryRegister";
				break;

			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COMPLETE:
				validator = "CsIncidentCategoryModify";
					break;

			default:
				throw new Exception("予期せぬ ActionStatus が指定されました： " + this.ActionStatus);
		}

		// 入力チェック
		string errorMessages = Validator.Validate(validator, input);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// モデルに変換
		CsIncidentCategoryModel model = new CsIncidentCategoryModel(input);
		model.DisplayOrder = int.Parse((string)input[Constants.FIELD_CSINCIDENTCATEGORY_DISPLAY_ORDER]);	// 表示順を型変換
		model.EX_ParentCategoryName = ddlParentCategory.SelectedItem.Text;									// 親カテゴリ名（完了画面用）

		return model;
	}
	
	/// <summary>
	/// インシデントカテゴリの更新が行えるかチェック
	/// 親カテゴリに自分およびその子孫を指定している場合は更新できないようにする
	/// </summary>
	/// <param name="incidentInfo">インシデントカテゴリ更新データ</param>
	/// <returns>true：更新可、false：更新不可</returns>
	private bool CheckValidParentCategory(CsIncidentCategoryModel incidentInfo)
	{
		// 親カテゴリ指定なしであれば更新可能
		if (incidentInfo.EX_ParentCategoryName == "") return true;

		// 現在のランク付きカテゴリIDを取得
		CsIncidentCategoryService incidentCategoryService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		CsIncidentCategoryModel[] modelsCurrent = incidentCategoryService.SearchByCategoryId(this.LoginOperatorDeptId, incidentInfo.CategoryId);
		string rankedCategoryId = modelsCurrent[0].EX_RankedCategoryId;

		// 指定された親カテゴリのランク付きカテゴリIDを取得
		CsIncidentCategoryModel[] modelsParent = incidentCategoryService.SearchByCategoryId(this.LoginOperatorDeptId, incidentInfo.ParentCategoryId);
		string parentRankedCategoryId = modelsParent[0].EX_RankedCategoryId;

		// 親子関係の判定
		return (parentRankedCategoryId.StartsWith(rankedCategoryId) == false);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnIncidentCategoryDelete_Click(object sender, EventArgs e)
	{
		// インシデントカテゴリ情報削除チェック
		CsIncidentCategoryService incidentCategoryService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		if (incidentCategoryService.HasIncident(this.LoginOperatorDeptId, this.CategoryId))
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_DELETE_BY_INCIDENT);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		if (incidentCategoryService.HasChildCategory(this.LoginOperatorDeptId, this.CategoryId))
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_DELETE_BY_CHILD);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// インシデントカテゴリ情報削除
		incidentCategoryService.Delete(this.LoginOperatorDeptId, this.CategoryId);

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTCATEGORY_REGISTER);
	}

	#region プロパティ
	/// <summary>インシデントカテゴリ総件数</summary>
	protected int TotalCategoryCount { get; private set; }
	/// <summary>カテゴリID</summary>
	protected string CategoryId
	{
		get { return (string)ViewState[Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_ID]; }
		set { ViewState[Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_ID] = value; }
	}
	/// <summary>データバインド用モデル</summary>
	protected CsIncidentCategoryModel CategoryInfo { get; private set; }
	/// <summary>データバインド用モデル（完了画面用）</summary>
	protected CsIncidentCategoryModel CategoryInfoComp { get; private set; }
	#endregion
}
