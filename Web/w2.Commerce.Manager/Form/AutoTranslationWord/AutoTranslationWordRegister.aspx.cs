/*
=========================================================================================================
  Module      : 自動翻訳設定登録ページ(AutoTranslationWordRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common.Global.Translation;
using w2.App.Common.RefreshFileManager;
using w2.Domain.AutoTranslationWord;

/// <summary>
/// 自動翻訳設定登録ページ
/// </summary>
public partial class Form_AutoTranslationWord_AutoTranslationWordRegister : AutoTranslationWordPage
{
	/// <summary>セッションキー:登録/更新完了メッセージ</summary>
	const string SESSION_KEY_DISP_COMP_MESSAGE = "dispcompmessage";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// リクエスト取得＆ビューステート格納
			var m_actionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);

			// コンポーネント初期化
			InitializeComponents(m_actionStatus);

			// 登録/更新完了メッセージ表示制御
			if (Session[SESSION_KEY_DISP_COMP_MESSAGE] != null)
			{
				Session[SESSION_KEY_DISP_COMP_MESSAGE] = null;

				divComp.Visible = true;
			}
			else
			{
				divComp.Visible = false;
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string m_actionStatus)
	{
		// 表示非表示制御
		switch (m_actionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:

				// 項目表示制御
				trRegistTitle.Visible = true;
				trEditTitle.Visible = false;
				tbdyDispUpdateInfo.Visible = false;

				divComp.Visible = false;
				divDispInsert.Visible = true;

				// ボタン表示制御
				btnInsert.Visible = true;
				btnUpdate.Visible = false;
				break;

			case Constants.ACTION_STATUS_UPDATE:

				// 項目表示制御
				trRegistTitle.Visible = false;
				trEditTitle.Visible = true;
				tbdyDispUpdateInfo.Visible = true;

				divComp.Visible = false;
				divDispUpdate.Visible = true;

				// ボタン表示制御
				btnInsert.Visible = false;
				btnUpdate.Visible = true;

				DisplayAutoTranslationWordModel();

				break;
		}
	}

	/// <summary>
	/// 自動翻訳モデルを表示
	/// </summary>
	protected void DisplayAutoTranslationWordModel()
	{
		var model = new AutoTranslationWordService().Get(this.RequestWordHashKey, this.RequestLanguageCode);
		lLanguageCode.Text = WebSanitizer.HtmlEncode(model.LanguageCode);
		lWordBefore.Text = WebSanitizer.HtmlEncodeChangeToBr(model.WordBefore);
		hfWordBefore.Value = model.WordBefore;
		tbWordAfter.Text = model.WordAfter;
		cbClearFlg.Checked = Convert.ToBoolean(Convert.ToInt16(model.ClearFlg));
		lDateUsed.Text = WebSanitizer.HtmlEncode(model.DateUsed);
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToList_Click(object sender, EventArgs e)
	{
		// 一覧へリダイレクト
		Response.Redirect(this.SearchInfo.CreateAutoTranslationWordListUrl());
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		var languageCode = tbLanguageCode.Text.Trim();
		var wordHashKey = TranslationManager.WordHash(tbWordBefore.Text);

		var model = new AutoTranslationWordModel
		{
			WordHashKey = wordHashKey,
			LanguageCode = languageCode,
			WordBefore = tbWordBefore.Text,
			WordAfter = tbWordAfter.Text,
			ClearFlg = Convert.ToInt32(cbClearFlg.Checked).ToString(),
			LastChanged = this.LoginOperatorName,
		};

		var errorMessages = CheckInputAutoTranslationWord(model);
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		new AutoTranslationWordService().Insert(model);

		// リフレファイル更新(キャッシュの再生成)
		RefreshFileManagerProvider.GetInstance(RefreshFileType.AutoTranslationWord).CreateUpdateRefreshFile();

		// 完了メッセージ表示準備＆画面遷移
		Session[SESSION_KEY_DISP_COMP_MESSAGE] = "1";

		Response.Redirect(CreateDetailUrl(wordHashKey, languageCode));
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		var model = new AutoTranslationWordModel
		{
			WordHashKey = this.RequestWordHashKey,
			LanguageCode = this.RequestLanguageCode,
			WordBefore = hfWordBefore.Value,
			WordAfter = tbWordAfter.Text,
			ClearFlg = Convert.ToInt32(cbClearFlg.Checked).ToString(),
			LastChanged = this.LoginOperatorName,
		};

		new AutoTranslationWordService().Update(model);

		// リフレファイル更新(キャッシュの再生成)
		RefreshFileManagerProvider.GetInstance(RefreshFileType.AutoTranslationWord).CreateUpdateRefreshFile();

		// 完了メッセージ表示準備＆画面遷移
		Session[SESSION_KEY_DISP_COMP_MESSAGE] = "1";

		Response.Redirect(CreateDetailUrl(this.RequestWordHashKey, this.RequestLanguageCode));
	}

	/// <summary>
	/// 自動翻訳 入力チェック
	/// </summary>
	/// <param name="model">自動翻訳モデル</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckInputAutoTranslationWord(AutoTranslationWordModel model)
	{
		// 入力チェック
		var errorMessages = new StringBuilder();
		errorMessages.Append(Validator.Validate("AutoTranslationWordRegister", model.DataSource));

		return errorMessages.ToString();
	}

	/// <summary>リクエスト：ワードハッシュキー</summary>
	private string RequestWordHashKey
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_AUTOTRANSLATION_WORD_HASH]).Trim(); }
	}
	/// <summary>リクエスト：言語コード</summary>
	private string RequestLanguageCode
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_AUTOTRANSLATION_LANGUAGE_CODE]).Trim(); }
	}
}