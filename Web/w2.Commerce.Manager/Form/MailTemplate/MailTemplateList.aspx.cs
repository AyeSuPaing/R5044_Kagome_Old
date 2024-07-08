/*
=========================================================================================================
  Module      : メールテンプレート一覧ページ処理(MailTemplateList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.Domain.MailTemplate;
using System.Collections.Generic;

/// <summary>
/// MailTemplateList の概要の説明です。
/// </summary>
public partial class Form_MailTemplate_MailTemplateList : BasePage
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
			// 編集などした時に残ったセッションをクリアする（新規登録の初期値を正しいするため）
			Session.Remove(Constants.SESSIONPARAM_KEY_MAILTEMPLATE_INFO);
			// メールテンプレート情報一覧表示
			ViewMailTemplateList();
		}
	}

	/// <summary>
	/// メールテンプレート情報一覧表示
	/// </summary>
	private void ViewMailTemplateList()
	{
		// 変数宣言
		Hashtable htParam = new Hashtable();

		//------------------------------------------------------
		// リクエスト情報取得
		//------------------------------------------------------
		htParam = GetParameters(Request);
		// 不正パラメータが存在した場合
		if ((bool)htParam[Constants.ERROR_REQUEST_PRAMETER])
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		string strSearchKey = StringUtility.ToEmpty((string)htParam[Constants.REQUEST_KEY_SEARCH_KEY]);
		string strSearchWord = StringUtility.ToEmpty((string)htParam[Constants.REQUEST_KEY_SEARCH_WORD]);
		string strSortKbn = StringUtility.ToEmpty((string)htParam[Constants.REQUEST_KEY_SORT_KBN]);
		int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

		//------------------------------------------------------
		// 検索コントロール制御（メールテンプレート一覧共通処理）
		//------------------------------------------------------
		SetSearchInfo(strSearchKey, strSearchWord, strSortKbn);

		//------------------------------------------------------
		// メールテンプレート一覧
		//------------------------------------------------------
		var exclusionList = new List<string>();
		if ((Constants.STOCK_ALERT_MAIL_THRESHOLD == 0))
		{
			exclusionList.Add(Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_STOCKALERT);
		}
		if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false)
		{
			exclusionList.Add(Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_FIXEDPURCHASE);
		}

		var searchCondition = CreateSearchCondition(iCurrentPageNumber, strSearchKey, strSearchWord, strSortKbn);
		var service = new MailTemplateService();
		var mailTemplateList = exclusionList.Any()
			? service.SearchExcludeCategory(searchCondition, exclusionList.ToArray())
			: service.Search(searchCondition);

  // 検索条件取得
  var pageNo = 1;
		if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out pageNo) == false) pageNo = 1;

		// 件数取得
		var totalMailTemplateCount = exclusionList.Any()
			? service.GetSearchHitCountExcludeCategory(searchCondition, exclusionList.ToArray())
			: service.GetSearchHitCount(searchCondition);

  // ページャー
  var rootUrl = CreateMailTemplateListUrl(strSearchKey, strSearchWord, strSortKbn);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalMailTemplateCount, pageNo, rootUrl);

		// 自動送信未対応のメールテンプレートはフラグ欄を空欄で表示
		foreach (var mailTemplate in mailTemplateList)
		{
			var autoSendPossible = Constants.AUTOSEND_MAIL_ID_LIST.Any(autoSendMailId => (mailTemplate.MailId == autoSendMailId));
			if (autoSendPossible == false) mailTemplate.AutoSendFlg = "";
		}

		// データソースセット
		rList.DataSource = mailTemplateList;

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();

		// 件数取得、エラー表示制御
		if (totalMailTemplateCount != 0)
		{
			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
	}

	/// <summary>
	/// ルートURL作成
	/// </summary>
	/// <returns>ルートURL</returns>
	private string CreateRootUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST).CreateUrl();
		return url;
	}

	/// <summary>
	/// メールテンプレート一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">商品一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	private static Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();

		int iCurrentPageNumber = 1;
		string strSearchKey = String.Empty;
		string strSearchWord = String.Empty;
		string strSortKbn = String.Empty;
		bool blParamError = false;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// 検索キー
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SEARCH_KEY]))
			{
				case Constants.KBN_SEARCHKEY_MAILTEMPLATE_LIST_MAIL_ID:						// メールテンプレートID
				case Constants.KBN_SEARCHKEY_MAILTEMPLATE_LIST_MAIL_NAME:					// メールテンプレート名
					strSearchKey = hrRequest[Constants.REQUEST_KEY_SEARCH_KEY].ToString();
					break;
				case "":
					strSearchKey = Constants.KBN_SEARCHKEY_MAILTEMPLATE_LIST_DEFAULT;		// メールテンプレートIDがデフォルト
					break;
				default:
					blParamError = true;
					break;
			}
		}
		catch
		{
			blParamError = true;
		}
		// 検索ワード
		try
		{
			strSearchWord = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SEARCH_WORD]);
		}
		catch
		{
			blParamError = true;
		}
		// ソート区分
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_MAILTEMPLATE_LIST_MAIL_ID_ASC:			// メールテンプレートID/昇順
				case Constants.KBN_SORT_MAILTEMPLATE_LIST_MAIL_ID_DESC:			// メールテンプレートID/降順
				case Constants.KBN_SORT_MAILTEMPLATE_LIST_MAIL_NAME_ASC:		// メールテンプレート名/昇順
				case Constants.KBN_SORT_MAILTEMPLATE_LIST_MAIL_NAME_DESC:		// メールテンプレート名/降順
				case Constants.KBN_SORT_MAILTEMPLATE_LIST_DATE_CREATED_ASC:		// 作成日/昇順
				case Constants.KBN_SORT_MAILTEMPLATE_LIST_DATE_CREATED_DESC:	// 作成日/降順
				case Constants.KBN_SORT_MAILTEMPLATE_LIST_DATE_CHANGED_ASC:		// 更新日/昇順
				case Constants.KBN_SORT_MAILTEMPLATE_LIST_DATE_CHANGED_DESC:	// 更新日/降順
					strSortKbn = hrRequest[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				case "":
					strSortKbn = Constants.KBN_SORT_MAILTEMPLATE_LIST_DEFAULT;	// メールテンプレートID/昇順がデフォルト
					break;
				default:
					blParamError = true;
					break;
			}
		}
		catch
		{
			blParamError = true;
		}
		// ページ番号（ページャ動作時のみもちまわる）
		try
		{
			if (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				iCurrentPageNumber = int.Parse(hrRequest[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			blParamError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_KEY, strSearchKey);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_WORD, strSearchWord);
		htResult.Add(Constants.REQUEST_KEY_SORT_KBN, strSortKbn);
		htResult.Add(Constants.ERROR_REQUEST_PRAMETER, blParamError);

		return htResult;
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <param name="iPageNumber">ページ番号</param>
	/// <param name="strSearchKey">検索フィールド</param>
	/// <param name="strSearchWord">検索ワード</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <returns>検索条件</returns>
	private Hashtable CreateSearchCondition(int iPageNumber, string strSearchKey, string strSearchWord, string strSortKbn)
	{
		// 検索値が存在しない場合
		if (strSearchWord == "")
		{
			strSearchKey = "99";	// 未条件
		}

		int iBgn = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNumber - 1) + 1;
		int iEnd = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNumber;

		var param = new Hashtable
		{
			{"srch_key", strSearchKey},	// 検索フィールド
			{"srch_word", strSearchWord},	// 検索値
			{"srch_word_like_escaped", StringUtility.SqlLikeStringSharpEscape(strSearchWord)},	// 検索値（エスケープ）
			{"sort_kbn", strSortKbn},	// ソート区分
			{Constants.FIELD_MAILTEMPLATE_SHOP_ID, this.LoginOperatorShopId},	// 店舗ID
			{"bgn_row_num", iBgn},	// 表示開始記事番号
			{"end_row_num", iEnd},	// 表示開始記事番号
		};
		return param;
	}

	/// <summary>
	/// 検索コントロール制御
	/// </summary>
	/// <remarks>
	/// Request内容を検索コントロールに設定
	/// </remarks>
	private void SetSearchInfo(string strSearchKey, string strSearchWord, string strSortKbn)
	{
		foreach (ListItem li in ddlSearchKey.Items)
		{
			li.Selected = (li.Value == strSearchKey);
		}
		tbSearchWord.Text = strSearchWord;
		foreach (ListItem li in ddlSortKbn.Items)
		{
			li.Selected = (li.Value == strSortKbn);
		}
	}

	/// <summary>
	/// メールテンプレート一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <returns>メールテンプレート一覧遷移URL</returns>
	private string CreateMailTemplateListUrl(string strSearchKey, string strSearchWord, string strSortKbn)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_LIST;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_SEARCH_KEY + "=" + strSearchKey;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_SEARCH_WORD + "=" + HttpUtility.UrlEncode(strSearchWord);
		strResult += "&";
		strResult += Constants.REQUEST_KEY_SORT_KBN + "=" + strSortKbn;

		return strResult;
	}

	/// <summary>
	/// メールテンプレート一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>メールテンプレート一覧遷移URL</returns>
	private string CreateMailTemplateListUrl(string strSearchKey, string strSearchWord, string strSortKbn, int iPageNumber)
	{
		string strResult = CreateMailTemplateListUrl(strSearchKey, strSearchWord, strSortKbn);
		strResult += "&";
		strResult += Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();

		return strResult;
	}

	/// <summary>
	/// データバインド用メールテンプレート詳細URL作成
	/// </summary>
	/// <param name="strMailId">メールテンプレートID</param>
	/// <returns>メールテンプレート詳細URL</returns>
	protected string CreateMailTemplateDetailUrl(string strMailId)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_CONFIRM;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_MAIL_TEMPLATE_ID + "=" + strMailId;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;

		return strResult;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// メールテンプレート一覧へ
		Response.Redirect(
			CreateMailTemplateListUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim(), ddlSortKbn.SelectedValue, 1));
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
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MAIL_TEMPLETE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}
}
