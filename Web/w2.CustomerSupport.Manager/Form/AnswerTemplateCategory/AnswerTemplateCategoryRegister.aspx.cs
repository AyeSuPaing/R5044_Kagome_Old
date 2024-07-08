/*
=========================================================================================================
  Module      : 回答例カテゴリ登録ページ処理(AnswerTemplateCategoryRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.AnswerTemplate;

public partial class Form_AnswerTemplateCategory_AnswerTemplateCategoryRegister : BasePage
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
		switch (this.ActionStatus)		// 処理ステータス
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
		// 表示コントロール制御
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:	// 登録画面
				trNotSelect.Visible = false;
				trEdit.Visible = true;
				tdParentCategoryRegisterHead.Visible = true;
				tdParentCategoryRegister.Visible = true;
				btnAnswerTemplateCategoryRegister.Visible = true;
				cbValidFlg.Checked = true;	// 有効フラグにデフォルトチェック
				break;

			case Constants.ACTION_STATUS_UPDATE:	// 編集画面
				trNotSelect.Visible = false;
				trEdit.Visible = true;
				tdParentCategoryRegisterHead.Visible = true;
				tdParentCategoryRegister.Visible = true;
				btnAnswerTemplateCategoryModify.Visible = true;
				btnAnswerTemplateCategoryDelete.Visible = true;
				break;

			case Constants.ACTION_STATUS_COMPLETE:	// 完了画面
				trEdit.Visible = true;
				tdParentCategoryRegisterHead.Visible = true;
				tdParentCategoryRegister.Visible = true;
				btnAnswerTemplateCategoryModify.Visible = true;
				btnAnswerTemplateCategoryDelete.Visible = true;
				divErrorMessage.Visible = false;
				divComp.Visible = true;
				break;

			default:								// 未選択
				divErrorMessage.Visible = true;
				divComp.Visible = false;
				divNotSelectMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ANSWER_TEMPLATE_CATEGORY_NO_SELECTED_ERROR);
				break;
		}

		// 回答例カテゴリドロップダウン作成
		var service = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
		var models = service.GetAll(this.LoginOperatorDeptId);
		ddlParentCategory.Items.Add(new ListItem("", ""));
		ddlParentCategory.Items.AddRange(models.Select(m => new ListItem(m.EX_RankedCategoryName, m.CategoryId)).ToArray());

		// 表示順ドロップダウン作成
		ddlDisplayOrder.Items.AddRange(Enumerable.Range(1, 100).Select(i => new ListItem(i.ToString(), i.ToString())).ToArray());
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	private void Display()
	{
		// 回答例カテゴリ情報取得
		var categoryService = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
		var categoryList = categoryService.SearchByCategoryId(this.LoginOperatorDeptId, "");

		// 件数取得、エラー表示/非表示制御
		if (categoryList.Length != 0)
		{
			this.TotalCategoryCount = categoryList[0].EX_SearchCount;
			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		rList.DataSource = categoryList;
		rList.DataBind();
		
		// 回答例カテゴリ情報詳細表示
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_UPDATE:	// 編集
			case Constants.ACTION_STATUS_COMPLETE:	// 完了

				// 回答例カテゴリIDをセット
				if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				{
					this.CategoryId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CATEGORY_ID]);
				}
				else if (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)
				{
					this.CategoryId = ((CsAnswerTemplateCategoryModel)Session[Constants.SESSION_KEY_ANSWERTEMPLATECATEGORY_INFO]).CategoryId;
				}

				// データ取得
				var service = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
				this.CategoryInfo = service.Get(this.LoginOperatorDeptId, CategoryId);
				if (this.CategoryInfo == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// 回答例カテゴリ情報データバインド
				trEdit.DataBind();
				break;
		}
	}

	/// <summary>
	/// 編集画面URL作成
	/// </summary>
	/// <param name="categoryId">回答例カテゴリID</param>
	/// <returns>回答例カテゴリ編集画面URL</returns>
	protected string CreateEditUrl(string categoryId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATECATEGORY_REGISTER
			+ "?" + Constants.REQUEST_KEY_CATEGORY_ID + "=" + categoryId
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAnswerTemplateCategoryInsert_Click(object sender, EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 登録画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATECATEGORY_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAnswerTemplateCategoryRegister_Click(object sender, System.EventArgs e)
	{
		// 登録情報作成
		CsAnswerTemplateCategoryModel inputCategory = CheckInput();
		inputCategory.DeptId = this.LoginOperatorDeptId;					// 識別ID
		inputCategory.LastChanged = this.LoginOperatorName;					// 最終更新者

		// 登録
		var service = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
		var categoryId = service.Register(inputCategory);

		// 完了画面用に格納
		Session[Constants.SESSION_KEY_ANSWERTEMPLATECATEGORY_INFO] = inputCategory;
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;

		// 完了画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATECATEGORY_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COMPLETE);
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAnswerTemplateCategoryModify_Click(object sender, EventArgs e)
	{
		// 更新情報作成
		CsAnswerTemplateCategoryModel updateCateogyr = CheckInput();
		updateCateogyr.DeptId = this.LoginOperatorDeptId;					// 識別ID
		updateCateogyr.LastChanged = this.LoginOperatorName;				// 最終更新者
		updateCateogyr.CategoryId = this.CategoryId;						// 更新対象回答例カテゴリID

		// 無限ループチェック
		if (CheckValidParentCategory(updateCateogyr) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ANSWER_TEMPLATE_CATEGORY_NO_UPDATE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 更新
		var service = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
		service.Update(updateCateogyr);

		// 完了画面用に格納
		Session[Constants.SESSION_KEY_ANSWERTEMPLATECATEGORY_INFO] = updateCateogyr;
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;

		// 完了画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATECATEGORY_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COMPLETE);
	}

	/// <summary>
	/// 入力情報チェック
	/// </summary>
	/// <returns>入力情報にもとづく回答例カテゴリモデル</returns>
	private CsAnswerTemplateCategoryModel CheckInput()
	{
		// 入力データ格納
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_PARENT_CATEGORY_ID, ddlParentCategory.SelectedValue);	// 親ID
		input.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_NAME, tbDivisionName.Text);						// 回答例カテゴリ名
		input.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_DISPLAY_ORDER, ddlDisplayOrder.SelectedValue);			// 表示順
		input.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_CSANSWERTEMPLATECATEGORY_VALID_FLG_VALID : Constants.FLG_CSANSWERTEMPLATECATEGORY_VALID_FLG_INVALID);						// 有効フラグ

		// 処理区分に応じたValidator選択
		string validator;
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				validator = "CsAnswerTemplateCategoryRegister";
				break;

			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COMPLETE:
				validator = "CsAnswerTemplateCategoryModify";
				break;

			default:
				throw new Exception("予期せぬ ActionStatus が指定されました： " + this.ActionStatus);
		}

		// 入力チェック
		string errorMsg = Validator.Validate(validator, input);
		if (errorMsg != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMsg;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// モデルに変換
		CsAnswerTemplateCategoryModel model = new CsAnswerTemplateCategoryModel(input);
		model.DisplayOrder = int.Parse((string)input[Constants.FIELD_CSANSWERTEMPLATECATEGORY_DISPLAY_ORDER]);	// 表示順を型変換
		model.EX_ParentCategoryName = ddlParentCategory.SelectedItem.Text;										// 親カテゴリ名を格納（完了画面用）

		return model;
	}

	/// <summary>
	/// 回答例カテゴリの更新が行えるかチェック
	/// 親カテゴリに自分およびその子孫を指定している場合は更新できないようにする
	/// </summary>
	/// <param name="categoryModel">回答例カテゴリモデル</param>
	/// <returns>true：更新可、false：更新不可</returns>
	private bool CheckValidParentCategory(CsAnswerTemplateCategoryModel categoryModel)
	{
		// 親カテゴリ指定なしであれば更新可能
		if (categoryModel.ParentCategoryId == "") return true;

		// ランク付きカテゴリIDを取得
		var service = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
		var category = service.SearchByCategoryId(this.LoginOperatorDeptId, categoryModel.CategoryId);
		var rankedCategoryId = category[0].EX_RankedCategoryId;

		// 親カテゴリのランク付きカテゴリIDを取得
		var parent = service.SearchByCategoryId(this.LoginOperatorDeptId, categoryModel.ParentCategoryId);
		var parentRankedCategoryId = parent[0].EX_RankedCategoryId;

		// 親子関係の判定
		return (parentRankedCategoryId.StartsWith(rankedCategoryId) == false);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAnswerTemplateCategoryDelete_Click(object sender, EventArgs e)
	{
		// 回答例カテゴリ情報削除チェック
		var service = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
		if (service.HasChildCategories(this.LoginOperatorDeptId, this.CategoryId))
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ANSWER_TEMPLATE_CATEGORY_NO_DELETE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 削除
		service.Delete(this.LoginOperatorDeptId, this.CategoryId);

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATECATEGORY_REGISTER);
	}

	#region プロパティ
	/// <summary>回答例カテゴリID</summary>
	protected string CategoryId
	{
		get { return (string)ViewState[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_CATEGORY_ID]; }
		set { ViewState[Constants.FIELD_CSANSWERTEMPLATE_ANSWER_CATEGORY_ID] = value; }
	}
	/// <summary>総カテゴリ件数</summary>
	protected int TotalCategoryCount { get; private set; }
	/// <summary>データバインド用モデル</summary>
	protected CsAnswerTemplateCategoryModel CategoryInfo;
	/// <summary>データバインド用モデル（完了画面用）</summary>
	protected CsAnswerTemplateCategoryModel CategoryInfoComp;
	#endregion
}
