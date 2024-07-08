/*
=========================================================================================================
  Module      : ポイント基本ルール一覧ページ処理(PointRuleList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.Domain.Point;
using w2.Domain.Point.Helper;

public partial class Form_PointRule_PointRuleList : BasePage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		ClearBrowserCache();

		if (!IsPostBack)
		{
			// 登録系のセッションをクリア
			Session[MpSessionWrapper.SESSION_KEY_POINTRULE_INPUT] = null;

			// ポイント基本ルール情報一覧表示
			ViewPointRuleList();
		}
	}
	#endregion

	#region -ViewPointRuleList ポイント基本ルール情報一覧表示(DataGridにDataView(ポイント基本ルール情報)を設定)
	/// <summary>
	/// ポイント基本ルール情報一覧表示(DataGridにDataView(ポイント基本ルール情報)を設定)
	/// </summary>
	private void ViewPointRuleList()
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
		int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

		//------------------------------------------------------
		// ポイント基本ルール情報一覧
		//------------------------------------------------------
		int iTotalPointRuleCounts = 0;	// ページング可能総数
		
		// 検索用のサービスクラス
		var searchSv = new PointService();
		
		// 検索条件組立
		var cond = new PointRuleListSearchCondition();
		cond.DeptId = this.LoginOperatorDeptId;
		// ポイントルール区分は「01：基本」
		cond.PointRuleKbn = Constants.FLG_POINTRULE_POINT_RULE_KBN_BASE;
		cond.SearchKey = 99;
		cond.SearchWord = "";
		cond.SortKbn = int.Parse(Constants.KBN_SEARCHKEY_USERPOINT_LIST_DEFAULT);
		cond.BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1;
		cond.EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber;

		//検索実行
		var result = (Constants.POINTRULE_OPTION_CLICKPOINT_ENABLED)
			? searchSv.PointRuleListSearch(cond)
			: searchSv.PointRuleListSearch(cond)
				.Where(prl => prl.PointIncKbn != Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK);
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
			tdErrorMessage.InnerHtml =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

		}
		// データソースセット
		rList.DataSource = result
			.Select(item => new WrappedSearchResult(item))
			.ToArray();

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string strNextUrl = Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_LIST;
		lbPager1.Text = WebPager.CreateDefaultListPager(iTotalPointRuleCounts, iCurrentPageNumber, strNextUrl);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
	}
	#endregion

	#region -GetParameters ポイント基本ルール情報一覧パラメタ取得
	/// <summary>
	/// ポイント基本ルール情報一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">ポイント基本ルール情報一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	private static Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();

		int iCurrentPageNumber = 1;
		bool blParamError = false;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
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

		return htResult;
	}
	#endregion

	#region #CreatePointRuleDetailUrl データバインド用ポイント基本ルール情報詳細URL作成
	/// <summary>
	/// データバインド用ポイント基本ルール情報詳細URL作成
	/// </summary>
	/// <param name="strPointRuleId">ルールID</param>
	/// <returns>ポイント基本ルール情報詳細URL</returns>
	protected string CreatePointRuleDetailUrl(string strPointRuleId)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CONFIRM;
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
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_REGISTER + "?" + 
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
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