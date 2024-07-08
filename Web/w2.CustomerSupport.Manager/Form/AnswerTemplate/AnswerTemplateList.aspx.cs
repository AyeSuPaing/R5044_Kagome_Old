/*
=========================================================================================================
  Module      : 回答例文章設定一覧ページ処理(AnswerTemplateList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.AnswerTemplate;
using w2.Common.Web;

public partial class Form_AnswerTemplate_AnswerTemplateList : BasePage
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

			// 回答例情報一覧表示
			DisplayAnswerTemplateList();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 回答例カテゴリドロップダウン作成
		var categoryService = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
		var models = categoryService.GetValidAll(this.LoginOperatorDeptId);
		ddlCategory.Items.Add(new ListItem("　　　　　　　　　　　　　　　", ""));
		ddlCategory.Items.AddRange((from m in models select new ListItem(m.EX_RankedCategoryName, m.CategoryId)).ToArray());

		// 有効フラグドロップダウン作成
		ddlValidFlg.Items.Add(new ListItem("", ""));
		ddlValidFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSANSWERTEMPLATE, Constants.FIELD_CSANSWERTEMPLATE_VALID_FLG));
	}

	/// <summary>
	/// 回答例情報一覧表示
	/// </summary>
	private void DisplayAnswerTemplateList()
	{
		// 検索パラメータセット
		foreach (ListItem li in ddlCategory.Items)
		{
			li.Selected = (li.Value == this.RequestCategoryId);
		}
		tbAnswerTitle.Text = this.RequestTitle;
		tbAnswerText.Text = this.RequestText;
		ddlValidFlg.SelectedValue = this.RequestValidFlg;
		tbAnswerMailTitle.Text = this.RequestMailTitle;

		// 回答例情報一覧取得
		int bgnRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1;
		int endRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.RequestPageNum;
		var service = new CsAnswerTemplateService(new CsAnswerTemplateRepository());
		var models = service.Search(
			this.LoginOperatorDeptId,
			this.RequestCategoryId,
			this.RequestTitle,
			this.RequestMailTitle,
			this.RequestText,
			this.RequestValidFlg,
			bgnRow,
			endRow);
		rList.DataSource = models;
		rList.DataBind();

		// 件数取得、エラー表示制御
		int totalCount;
		if (models.Length != 0)
		{
			totalCount = models[0].EX_SearchCount;
			trListError.Visible = false;
		}
		else
		{
			totalCount = 0;
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// ページャ作成
		var nextUrl = CreateListUrl(
			this.RequestCategoryId,
			this.RequestTitle,
			this.RequestMailTitle,
			this.RequestText,
			this.RequestValidFlg);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);
	}

	/// <summary>
	/// 一覧URL作成
	/// </summary>
	/// <param name="categoryId">回答例カテゴリID</param>
	/// <param name="answerText">回答例タイトル</param>
	/// <param name="answerMailTitle">回答例件名</param>
	/// <param name="answerTitle">回答例本文</param>
	/// <param name="validFlg">有効フラグ</param>
	/// <returns>回答例情報一覧URL</returns>
	private string CreateListUrl(
		string categoryId,
		string answerTitle,
		string answerMailTitle,
		string answerText,
		string validFlg)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT_CS + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_LIST);
		urlCreator
			.AddParam(Constants.REQUEST_KEY_CATEGORY_ID, categoryId)
			.AddParam(Constants.REQUEST_KEY_ANSWERTEMPLATE_TITLE, answerTitle)
			.AddParam(Constants.REQUEST_KEY_ANSWERTEMPLATE_MAIL_TITLE, answerMailTitle)
			.AddParam(Constants.REQUEST_KEY_ANSWERTEMPLATE_TEXT, answerText)
			.AddParam(Constants.REQUEST_KEY_ANSWERTEMPLATE_VALID_FLG, validFlg);
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="answerId">回答例ID</param>
	/// <returns>回答例情報詳細URL</returns>
	protected string CreateDetailUrl(string answerId)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT_CS + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_CONFIRM);
		urlCreator
			.AddParam(Constants.REQUEST_KEY_ANSWER_TEMPLATE_ID, answerId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL);
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// 回答例情報一覧へ
		Response.Redirect(
			CreateListUrl(
				ddlCategory.SelectedValue,
				tbAnswerTitle.Text,
				tbAnswerMailTitle.Text,
				tbAnswerText.Text,
				ddlValidFlg.SelectedValue));
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
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	#region プロパティ
	/// <summary>リクエスト：回答例カテゴリID</summary>
	private string RequestCategoryId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CATEGORY_ID]); }
	}
	/// <summary>リクエスト：回答例タイトル</summary>
	private string RequestTitle
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ANSWERTEMPLATE_TITLE]); }
	}
	/// <summary>リクエスト：回答例件名</summary>
	private string RequestMailTitle
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ANSWERTEMPLATE_MAIL_TITLE]); }
	}
	/// <summary>リクエスト：回答例本文</summary>
	private string RequestText
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ANSWERTEMPLATE_TEXT]); }
	}
	/// <summary>リクエスト：有効フラグ</summary>
	private string RequestValidFlg
	{
		get
		{
			string validFlg = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ANSWERTEMPLATE_VALID_FLG]);
			switch (validFlg)
			{
				case "":												// 指定なし
				case Constants.FLG_CSANSWERTEMPLATE_VALID_FLG_VALID:	// 有効
				case Constants.FLG_CSANSWERTEMPLATE_VALID_FLG_INVALID:	// 無効
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
	#endregion
}
