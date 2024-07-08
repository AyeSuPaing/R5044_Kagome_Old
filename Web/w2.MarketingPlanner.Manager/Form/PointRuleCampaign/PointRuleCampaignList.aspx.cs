/*
=========================================================================================================
  Module      : ポイントキャンペーンルール一覧ページ処理(PointRuleCampaignList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Domain.Point;
using w2.Domain.Point.Helper;

public partial class Form_PointRuleCampaign_PointRuleCampaignList : BasePage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 登録系のセッションをクリア
			MpSessionWrapper.PointRuleInput = null;

			// 画面制御
			InitializeComponents();

			// ポイントキャンペーンール情報一覧表示
			ViewPointRuleCampaignList();
		}
	}
	#endregion

	#region -ViewPointRuleCampaignList ポイントキャンペーンルール情報一覧表示(DataGridにDataView(ポイントキャンペーンルール情報)を設定)
	/// <summary>
	/// ポイントキャンペーンルール情報一覧表示(DataGridにDataView(ポイントキャンペーンルール情報)を設定)
	/// </summary>
	private void ViewPointRuleCampaignList()
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
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		Session[Constants.SESSIONPARAM_KEY_POINTRULE_CAMPAIGN_SEARCH_INFO] = htParam;

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		string strSearchWord = StringUtility.ToEmpty((string)htParam[Constants.REQUEST_KEY_SEARCH_WORD]);
		string strSortKbn = StringUtility.ToEmpty((string)htParam[Constants.REQUEST_KEY_SORT_KBN]);
		int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

		//------------------------------------------------------
		// 検索コントロール制御（一覧共通処理）
		//------------------------------------------------------
		SetSearchInfo(strSearchWord,strSortKbn);

		try
		{
			//------------------------------------------------------
			// ポイントキャンペーンルール情報一覧
			//------------------------------------------------------
			int iTotalPointRuleCounts = 0;	// ページング可能総数
			
			// 検索用のサービスクラス
			var searchSv = new PointService();
			
			// 検索条件組立
			var cond = new PointRuleListSearchCondition();
			cond.DeptId = this.LoginOperatorDeptId;
			
			// ポイントルール区分は「02：キャンペーン」
			cond.PointRuleKbn = Constants.FLG_POINTRULE_POINT_RULE_KBN_CAMPAIGN;
			cond.SearchKey = string.IsNullOrEmpty(strSearchWord) ? 99 : 0;
			cond.SearchWord = strSearchWord;
			cond.SortKbn = int.Parse(strSortKbn);
			cond.BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1;
			cond.EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber;
			
			//検索実行
			var result = searchSv.PointRuleListSearch(cond);
			if (result.Any())
			{
				iTotalPointRuleCounts = result.FirstOrDefault().RowCount;
				// エラー非表示制御
				trListError.Visible = false;
			}
			else
			{
				iTotalPointRuleCounts = 0;
				// エラー表示制御
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}
			rList.DataSource = result
				.Select(item => new WrappedSearchResult(item))
				.ToArray();

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			string strNextUrl = CreatePointRuleCampaignListUrl(strSearchWord, strSortKbn);
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalPointRuleCounts, iCurrentPageNumber, strNextUrl);

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind();
		}
		finally
		{
		}
	}
	#endregion

	#region -GetParameters ポイントキャンペーンルール情報一覧パラメタ取得
	/// <summary>
	/// ポイントキャンペーンルール情報一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">ポイントキャンペーンルール情報一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	private Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();
		string strSearchWord = String.Empty;
		string strSortKbn = String.Empty;
		int iCurrentPageNumber = 1;
		bool blParamError = false;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// ソート区分
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_POINTRULE_LIST_PRIORITY_ASC:				// 優先度/昇順
				case Constants.KBN_SORT_POINTRULE_LIST_PRIORITY_DESC:				// 優先度/降順
					strSortKbn = hrRequest[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				case "":
					strSortKbn = Constants.KBN_SORT_POINTRULE_LIST_DEFAULT;			// 優先度/降順がデフォルト
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
			foreach (ListItem li in ddlPointIncKbn.Items)
			{
				li.Selected = (li.Value == StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SEARCH_WORD]));
			}
			strSearchWord = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SEARCH_WORD]);
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
		htResult.Add(Constants.ERROR_REQUEST_PRAMETER, blParamError);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_WORD, strSearchWord);
		htResult.Add(Constants.REQUEST_KEY_SORT_KBN, strSortKbn);

		return htResult;
	}
	#endregion

	#region -InitializeComponents 表示コンポーネント初期化
	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// ポイント加算区分ドロップダウン作成
		ddlPointIncKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_INC_KBN))
		{
			if ((li.Value == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE)
				|| (li.Value == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT)
				|| (li.Value == Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK)
				|| ((li.Value == Constants.FLG_POINTRULE_POINT_INC_KBN_LOGIN)
					&& (Constants.CROSS_POINT_LOGIN_POINT_ENABLED == false)
					&& Constants.CROSS_POINT_OPTION_ENABLED)) continue;
			ddlPointIncKbn.Items.Add(li);
		}
		ddlPointIncKbn.DataBind();
	}
	#endregion

	#region -SetSearchInfo 検索コントロール制御
	/// <summary>
	/// 検索コントロール制御
	/// </summary>
	/// <remarks>
	/// Request内容を検索コントロールに設定
	/// </remarks>
	private void SetSearchInfo(string strSearchWord, string strSortKbn)
	{
		foreach (ListItem li in ddlPointIncKbn.Items)
		{
			li.Selected = (li.Value == strSearchWord);
		}
		foreach (ListItem li in ddlSortKbn.Items)
		{
			li.Selected = (li.Value == strSortKbn);
		}
	}
	#endregion

	#region -CreatePointRuleCampaignListUrl ポイントキャンペーンルール情報一覧遷移URL作成
	/// <summary>
	/// ポイントキャンペーンルール情報一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <returns>ポイントキャンペーンルール情報一覧遷移URL</returns>
	private string CreatePointRuleCampaignListUrl(string strSearchWord, string strSortKbn)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_LIST;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_SEARCH_WORD + "=" + System.Web.HttpUtility.UrlEncode(strSearchWord);
		strResult += "&";
		strResult += Constants.REQUEST_KEY_SORT_KBN + "=" + strSortKbn;
		return strResult;
	}
	#endregion

	#region -CreatePointRuleCampaignListUrl ポイントキャンペーンルール情報一覧遷移URL作成
	/// <summary>
	/// ポイントキャンペーンルール情報一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <param name="iPageNumber">ページ番号</param>
	/// <returns>ポイントキャンペーンルール情報一覧遷移URL</returns>
	private string CreatePointRuleCampaignListUrl(string strSearchWord,
		string strSortKbn, int iPageNumber)
	{
		string strResult = CreatePointRuleCampaignListUrl(strSearchWord, strSortKbn);
		strResult += "&";
		strResult += Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();

		return strResult;
	}
	#endregion

	#region #CreatePointRuleCampaignDetailUrl データバインド用ポイントキャンペーンルール情報詳細URL作成
	/// <summary>
	/// データバインド用ポイントキャンペーンルール情報詳細URL作成
	/// </summary>
	/// <param name="strPointRuleId">ルールID</param>
	/// <returns>ポイントキャンペーンルール情報詳細URL</returns>
	protected string CreatePointRuleCampaignDetailUrl(string strPointRuleId)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_CONFIRM;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_POINTRULE_ID + "=" + strPointRuleId;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;

		return strResult;
	}
	#endregion

	#region #btnInsertTop_Click 新規登録ボタンクリック
	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertTop_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_REGISTER + "?" + 
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}
	#endregion

	#region #btnSearch_Click 検索ボタンクリック
	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// ポイントキャンペーンルール情報一覧へ
		Response.Redirect(
			CreatePointRuleCampaignListUrl(ddlPointIncKbn.SelectedValue,ddlSortKbn.SelectedValue,1));
	}
	#endregion

	#region 検索結果のラッパークラス
	/// <summary>
	/// 検索結果のラッパークラス
	/// </summary>
	[Serializable]
	protected class WrappedSearchResult : PointRuleListSearchResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result"></param>
		public WrappedSearchResult(PointRuleListSearchResult result)
			: base(result.DataSource)
		{

		}
	}
	#endregion
}