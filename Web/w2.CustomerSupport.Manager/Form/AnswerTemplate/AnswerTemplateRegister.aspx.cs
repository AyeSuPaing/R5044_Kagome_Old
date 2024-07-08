/*
=========================================================================================================
  Module      : 回答例文章設定登録ページ処理(AnswerTemplateRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.AnswerTemplate;

public partial class Form_AnswerTemplate_AnswerTemplateRegister : BasePage
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

			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			// 回答例情報表示
			DisplayAnswerTemplate();

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 表示制御
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー新規
				trRegister.Visible = true;
				break;

			case Constants.ACTION_STATUS_UPDATE:		// 編集
				trEdit.Visible = true;
				break;
		}

		// 回答例カテゴリドロップダウン作成
		var categoryService = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
		var models = categoryService.GetValidAll(this.LoginOperatorDeptId);
		ddlAnswerCategoryId.Items.Add(new ListItem("　　　　　　　　　　　　　　　", ""));
		ddlAnswerCategoryId.Items.AddRange(models.Select(model => new ListItem(model.EX_RankedCategoryName, model.CategoryId)).ToArray());

		// 表示順ドロップダウン作成
		for (int i = 1; i <= 100; i++)
		{
			ddlDisplayOrder.Items.Add(new ListItem(i.ToString(), i.ToString()));
		}
	}

	/// <summary>
	/// 回答例情報表示
	/// </summary>
	private void DisplayAnswerTemplate()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				break;

			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				// データ取得/設定
				CsAnswerTemplateModel answerTemplateInfo = (CsAnswerTemplateModel)Session[Constants.SESSION_KEY_ANSWERTEMPLATE_INFO];
				foreach (ListItem item in ddlAnswerCategoryId.Items)
				{
					// カテゴリプルダウンのデフォルト選択値を設定（DBから取得したカテゴリIDが存在しない可能性もあるので、ループで回す）
					item.Selected = (item.Value == answerTemplateInfo.AnswerCategoryId);
				}
				tbAnswerTitle.Text = answerTemplateInfo.AnswerTitle;												// 回答例タイトル
				tbAnswerMailTitle.Text = answerTemplateInfo.AnswerMailTitle;										// 回答例件名
				tbAnswerText.Text = answerTemplateInfo.AnswerText;													// 回答例本文
				ddlDisplayOrder.SelectedValue = answerTemplateInfo.DisplayOrder.ToString();							// 表示順
				cbValidFlg.Checked = (answerTemplateInfo.ValidFlg == Constants.FLG_CSANSWERTEMPLATE_VALID_FLG_VALID);	// 有効フラグ
				// ViewStateに格納
				this.AnswerId = answerTemplateInfo.AnswerId;
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		Hashtable inputData = CheckInputData();

		// モデル変換前処理
		inputData[Constants.FIELD_CSANSWERTEMPLATE_DISPLAY_ORDER] = int.Parse((string)inputData[Constants.FIELD_CSANSWERTEMPLATE_DISPLAY_ORDER]);	// 表示順
		inputData[Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_NAME] = ddlAnswerCategoryId.SelectedItem.Text;	// 回答例カテゴリ名称（確認画面用）

		// 必要なデータをセッションへ格納
		Session[Constants.SESSION_KEY_ANSWERTEMPLATE_INFO] = new CsAnswerTemplateModel(inputData);	// 入力データ
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;								// 処理区分

		// 回答例確認ページへ遷移			
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_CONFIRM
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + this.ActionStatus);		
	}

	/// <summary>
	/// 入力情報チェック
	/// </summary>
	/// <returns>入力情報</returns>
	private Hashtable CheckInputData()
	{
		Hashtable inputData = new Hashtable();
		string validator = "";

		// 処理区分に応じてValidator選択
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー新規
				validator = "CsAnswerTemplateRegister";
				break;

			case Constants.ACTION_STATUS_UPDATE:		// 変更
				validator = "CsAnswerTemplateModify";
				inputData.Add(Constants.FIELD_CSANSWERTEMPLATE_ANSWER_ID, this.AnswerId);	// 回答例ID
				break;
		}

		// 登録/更新データ格納
		inputData.Add(Constants.FIELD_CSANSWERTEMPLATE_DEPT_ID, this.LoginOperatorDeptId);				// 識別ID
		inputData.Add(Constants.FIELD_CSANSWERTEMPLATE_ANSWER_CATEGORY_ID, ddlAnswerCategoryId.Text);	// 回答例カテゴリID
		inputData.Add(Constants.FIELD_CSANSWERTEMPLATE_ANSWER_TITLE, tbAnswerTitle.Text);				// 回答例タイトル
		inputData.Add(Constants.FIELD_CSANSWERTEMPLATE_ANSWER_MAIL_TITLE, tbAnswerMailTitle.Text);		// 回答例件名
		inputData.Add(Constants.FIELD_CSANSWERTEMPLATE_ANSWER_TEXT, tbAnswerText.Text);					// 回答例本文
		inputData.Add(Constants.FIELD_CSANSWERTEMPLATE_DISPLAY_ORDER, ddlDisplayOrder.SelectedValue);	// 表示順
		inputData.Add(Constants.FIELD_CSANSWERTEMPLATE_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_CSANSWERTEMPLATE_VALID_FLG_VALID : Constants.FLG_CSANSWERTEMPLATE_VALID_FLG_INVALID);	// 有効フラグ
		inputData.Add(Constants.FIELD_CSANSWERTEMPLATE_LAST_CHANGED, this.LoginOperatorName);			// 最終更新者

		// 入力チェック＆重複チェック
		string strErrorMessages = Validator.Validate(validator, inputData);
		if (strErrorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		return inputData;
	}

	#region プロパティ
	/// <summary>回答例ID</summary>
	private string AnswerId
	{
		get { return (string)ViewState["AnswerId"]; }
		set { ViewState["AnswerId"] = value; }
	}
	#endregion
}
