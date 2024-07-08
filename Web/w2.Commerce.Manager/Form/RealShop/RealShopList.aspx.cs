/*
=========================================================================================================
  Module      : リアル店舗情報一覧ページ処理(RealShopList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.RealShop;

public partial class Form_RealShop_RealShopList : RealShopPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable param = GetParameters();
			// 不正パラメータが存在した場合
			if ((bool)param[Constants.ERROR_REQUEST_PRAMETER])
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			//------------------------------------------------------
			// 検索情報保持(編集で利用)
			//------------------------------------------------------
			Session[Constants.SESSIONPARAM_KEY_REALSHOP_SEARCH_INFO] = param;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			Hashtable sqlParam = GetSearchSqlInfo(param);
			int currentPageNumber = (int)param[Constants.REQUEST_KEY_PAGE_NO];

			//------------------------------------------------------
			// リアル店舗該当件数取得
			//------------------------------------------------------
			int totalRealShopCounts = 0;	// ページング可能総商品数
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("RealShop", "GetRealShopCount"))
			{
				DataView realShopCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParam);
				if (realShopCount.Count != 0)
				{
					totalRealShopCounts = (int)realShopCount[0]["row_count"];
				}
			}

			//------------------------------------------------------
			// エラー表示制御
			//------------------------------------------------------
			bool displayPager = true;
			StringBuilder errorMessage = new StringBuilder();
			// 上限件数より多い？
			if (totalRealShopCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				errorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(totalRealShopCounts));
				errorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				displayPager = false;
			}
			// 該当件数なし？
			else if (totalRealShopCounts == 0)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdErrorMessage.InnerHtml = errorMessage.ToString();
			trListError.Visible = (errorMessage.ToString().Length != 0);

			//------------------------------------------------------
			// リアル店舗一覧情報表示
			//------------------------------------------------------
			if (trListError.Visible == false)
			{
				// リアル店舗一覧情報取得
				DataView realShopList = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("RealShop", "GetRealShopList"))
				{
					sqlParam.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (currentPageNumber - 1) + 1);
					sqlParam.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * currentPageNumber);

					realShopList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParam);
				}

				// データバインド
				rList.DataSource = realShopList;
				rList.DataBind();
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (displayPager)
			{
				lbPager1.Text = WebPager.CreateDefaultListPager(totalRealShopCounts, currentPageNumber, CreateRealShopListUrl(param));
			}

			// Bind real shop area data
			var areas = DataCacheControllerFacade
				.GetRealShopAreaCacheController()
				.GetRealShopAreaList();

			rArea.DataSource = areas;
			rArea.DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 有効フラグ
		dllValidFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_REALSHOP, Constants.FIELD_REALSHOP_VALID_FLG))
		{
			dllValidFlg.Items.Add(li);
		}

		// 並び順
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_REALSHOP, Constants.REQUEST_KEY_SORT_KBN))
		{
			ddlSortKbn.Items.Add(li);
			if (li.Value == Constants.KBN_SORT_REALSHOP_LIST_DEFAULT) li.Selected = true;
		}

		// リアル店舗名選択用
		var realShopAll = RealShop.GetRealShopAll();
		rShopIdAutoComplete.DataSource = realShopAll;
		rShopIdAutoComplete.DataBind();
		rShopNameAutoComplete.DataSource = realShopAll;
		rShopNameAutoComplete.DataBind();
	}

	/// <summary>
	/// リアル店舗一覧パラメタ取得
	/// </summary>
	/// <param name="Request"> リアル店舗一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	protected Hashtable GetParameters()
	{
		Hashtable result = new Hashtable();
		int currentPageNumber = 1;
		bool paramError = false;
		try
		{
			// リアル店舗ID
			result.Add(Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID]));
			tbRealShopId.Text = Request[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID];
			// リアル店舗者名
			result.Add(Constants.REQUEST_KEY_REALSHOP_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_NAME]));
			tbRealShopName.Text = Request[Constants.REQUEST_KEY_REALSHOP_NAME];
			// リアル店舗者名(かな)
			result.Add(Constants.REQUEST_KEY_REALSHOP_NAME_KANA, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_NAME_KANA]));
			tbRealShopNameKana.Text = Request[Constants.REQUEST_KEY_REALSHOP_NAME_KANA];
			// メールアドレス
			result.Add(Constants.REQUEST_KEY_REALSHOP_MAIL_ADDR, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_MAIL_ADDR]));
			tbMailAddr.Text = Request[Constants.REQUEST_KEY_REALSHOP_MAIL_ADDR];
			// 電話番号
			result.Add(Constants.REQUEST_KEY_REALSHOP_TEL, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_TEL]));
			tbTel.Text = Request[Constants.REQUEST_KEY_REALSHOP_TEL];
			// 郵便番号
			result.Add(Constants.REQUEST_KEY_REALSHOP_ZIP, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_ZIP]));
			tbZip.Text = Request[Constants.REQUEST_KEY_REALSHOP_ZIP];
			// 住所
			result.Add(Constants.REQUEST_KEY_REALSHOP_ADDR, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_ADDR]));
			tbAddr.Text = Request[Constants.REQUEST_KEY_REALSHOP_ADDR];
			// FAX
			result.Add(Constants.REQUEST_KEY_REALSHOP_FAX, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_FAX]));
			tbFax.Text = Request[Constants.REQUEST_KEY_REALSHOP_FAX];
			// 有効フラグ
			result.Add(Constants.REQUEST_KEY_REALSHOP_VALID_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_VALID_FLG]));
			dllValidFlg.Text = Request[Constants.REQUEST_KEY_REALSHOP_VALID_FLG];
			// ソート区分
			string sortKbn = null;
			switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_REALSHOP_LIST_DISPLAY_ORDER_ASC:				// 表示順/昇順
				case Constants.KBN_SORT_REALSHOP_LIST_DISPLAY_ORDER_DESC:				// 表示順/降順
				case Constants.KBN_SORT_REALSHOP_LIST_REAL_SHOP_ID_ASC:					// リアル店舗ID/昇順
				case Constants.KBN_SORT_REALSHOP_LIST_REAL_SHOP_ID_DESC:				// リアル店舗ID/降順
				case Constants.KBN_SORT_REALSHOP_LIST_REAL_SHOP_NAME_ASC:			// 氏名/昇順
				case Constants.KBN_SORT_REALSHOP_LIST_REAL_SHOP_NAME_DESC:			// 氏名/降順
				case Constants.KBN_SORT_REALSHOP_LIST_REAL_SHOP_NAME_KANA_ASC:		// 氏名(かな)/昇順
				case Constants.KBN_SORT_REALSHOP_LIST_REAL_SHOP_NAME_KANA_DESC:		// 氏名(かな)/降順
					sortKbn = Request[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				case "":
					sortKbn = Constants.KBN_SORT_REALSHOP_LIST_DEFAULT;		//表示順/昇順がデフォルト
					break;
				default:
					paramError = true;
					break;
			}
			result.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);
			ddlSortKbn.SelectedValue = sortKbn;
			// ページ番号（ページャ動作時のみもちまわる）
			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				currentPageNumber = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			paramError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		result.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNumber);
		result.Add(Constants.ERROR_REQUEST_PRAMETER, paramError);

		return result;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateRealShopListUrl(GetSearchInfo()));
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="search">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable search)
	{
		// 変数宣言
		Hashtable result = new Hashtable();
		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		// リアル店舗ID
		result.Add(Constants.FIELD_REALSHOP_REAL_SHOP_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID]));
		// リアル店舗者名
		result.Add(Constants.FIELD_REALSHOP_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOP_NAME]));
		// リアル店舗者名（かな）
		result.Add(Constants.FIELD_REALSHOP_NAME_KANA + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOP_NAME_KANA]));
		// 電話番号
		result.Add(Constants.FIELD_REALSHOP_TEL + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOP_TEL]));
		// メールアドレス
		result.Add(Constants.FIELD_REALSHOP_MAIL_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOP_MAIL_ADDR]));
		// 郵便番号
		result.Add(Constants.FIELD_REALSHOP_ZIP + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOP_ZIP]));
		// 住所
		result.Add(Constants.FIELD_REALSHOP_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOP_ADDR]));
		// FAX
		result.Add(Constants.FIELD_REALSHOP_FAX + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(search[Constants.REQUEST_KEY_REALSHOP_FAX]));
		// 並び順
		result.Add(Constants.REQUEST_KEY_SORT_KBN + "_kbn", StringUtility.ToEmpty(search[Constants.REQUEST_KEY_SORT_KBN]));
		// 有効フラグ
		result.Add(Constants.FIELD_REALSHOP_VALID_FLG, StringUtility.ToEmpty(search[Constants.REQUEST_KEY_REALSHOP_VALID_FLG]));
		return result;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfo()
	{
		// 変数宣言
		Hashtable searchParams = new Hashtable();
		searchParams.Add(Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID, tbRealShopId.Text.Trim());		// リアル店舗ID
		searchParams.Add(Constants.REQUEST_KEY_REALSHOP_NAME, tbRealShopName.Text.Trim());				// リアル店舗者名
		searchParams.Add(Constants.REQUEST_KEY_REALSHOP_NAME_KANA, tbRealShopNameKana.Text.Trim());		// リアル店舗者名（かな）
		searchParams.Add(Constants.REQUEST_KEY_REALSHOP_TEL, tbTel.Text.Trim());						// 電話番号
		searchParams.Add(Constants.REQUEST_KEY_REALSHOP_MAIL_ADDR, tbMailAddr.Text.Trim());				// メールアドレス
		searchParams.Add(Constants.REQUEST_KEY_REALSHOP_ZIP, tbZip.Text.Trim());						// 郵便番号
		searchParams.Add(Constants.REQUEST_KEY_REALSHOP_ADDR, tbAddr.Text.Trim());						// 住所
		searchParams.Add(Constants.REQUEST_KEY_REALSHOP_FAX, tbFax.Text.Trim());						// FAX
		searchParams.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);						// ソート区分
		searchParams.Add(Constants.REQUEST_KEY_REALSHOP_VALID_FLG, dllValidFlg.SelectedValue);			// 有効フラグ
		return searchParams;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// 新規登録画面へ
		Session[Constants.SESSION_KEY_PARAM] = null;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_REALSHOP_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return GetSearchSqlInfo(GetParameters());
	}
}