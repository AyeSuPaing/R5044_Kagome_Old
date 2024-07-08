/*
=========================================================================================================
  Module      : シリアルキー一覧ページ処理(ProductList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.Domain.SerialKey;
using w2.App.Common.Product;

public partial class Form_SerialKey_SerialKeyList : SerialKeyPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
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
			// 登録系のセッションをクリア
			//------------------------------------------------------
			Session[Constants.SESSIONPARAM_KEY_SERIALKEY_INFO] = null;

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable searchParam = GetParameters();

			// 検索条件をセッションに保存
			Session[Constants.SESSIONPARAM_KEY_SERIALKEY_SEARCH_INFO] = searchParam;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			Hashtable sqlParam = GetSearchSqlInfo(searchParam);

			//------------------------------------------------------
			// シリアルキー該当件数取得
			//------------------------------------------------------
			// ページング可能総シリアルキー数
			int totalSerialKeyCounts = GetSerialKeyListCount(sqlParam);

			//------------------------------------------------------
			// エラー表示制御&ページャーの表示可能有無を取得
			//------------------------------------------------------
			bool displayPager = GetValidPagerDisplay(totalSerialKeyCounts);

			//------------------------------------------------------
			// シリアルキー一覧情報表示
			//------------------------------------------------------
			SetSerialKeyList(sqlParam, totalSerialKeyCounts);

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (displayPager)
			{
				string nextUrl = CreateSerialKeyListUrlWithoutPageNo(searchParam);
				lbPager1.Text = WebPager.CreateDefaultListPager(totalSerialKeyCounts, this.CurrentPageNo, nextUrl);
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 検索条件：並び順
		ddlSortKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_SERIALKEY,"sort"));

		// ステータス
		ddlStatus.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_SERIALKEY, Constants.FIELD_SERIALKEY_STATUS))
		{
			ddlStatus.Items.Add(li);
		}

		// 有効フラグ
		ddlValidFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_SERIALKEY, Constants.FIELD_SERIALKEY_VALID_FLG))
		{
			ddlValidFlg.Items.Add(li);
		}
	}

	/// <summary>
	/// シリアルキー該当件数取得
	/// </summary>
	/// <param name="sqlParam">検索情報パラメタ</param>
	/// <returns>該当件数</returns>
	private static int GetSerialKeyListCount(Hashtable sqlParam)
	{
		int totalSerialKeyCounts = new SerialKeyService().GetSerialKeyCount(
			(string)sqlParam[Constants.FIELD_SERIALKEY_SERIAL_KEY + Constants.FLG_LIKE_ESCAPED],
			(string)sqlParam[Constants.FIELD_SERIALKEY_PRODUCT_ID + Constants.FLG_LIKE_ESCAPED],
			(string)sqlParam[Constants.FIELD_SERIALKEY_USER_ID + Constants.FLG_LIKE_ESCAPED],
			(string)sqlParam[Constants.FIELD_SERIALKEY_ORDER_ID + Constants.FLG_LIKE_ESCAPED],
			(string)sqlParam[Constants.FIELD_SERIALKEY_STATUS],
			(string)sqlParam[Constants.FIELD_SERIALKEY_VALID_FLG],
			(string)sqlParam["sort_kbn"]);
		return totalSerialKeyCounts;
	}

	/// <summary>
	/// エラー表示制御&ページャーの表示可能有無を取得
	/// </summary>
	/// <param name="totalSerialKeyCounts">シリアルキー該当件数取得</param>
	/// <returns>ページャーの表示可能有無</returns>
	private bool GetValidPagerDisplay(int totalSerialKeyCounts)
	{
		bool displayPager = true;
		StringBuilder errorMessage = new StringBuilder();

		// 上限件数より多い？
		if (totalSerialKeyCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
		{
			errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
			errorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(totalSerialKeyCounts));
			errorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

			displayPager = false;
		}
		// 該当件数なし？
		else if (totalSerialKeyCounts == 0)
		{
			errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
		}
		tdErrorMessage.InnerHtml = errorMessage.ToString();
		trListError.Visible = (errorMessage.ToString().Length != 0);

		return displayPager;
	}

	/// <summary>
	/// シリアルキー一覧情報表示
	/// </summary>
	/// <param name="sqlParam">検索情報</param>
	/// <param name="totalCount">Total count</param>
	private void SetSerialKeyList(Hashtable sqlParam, int totalCount)
	{
		if (trListError.Visible == false)
		{
			var serialKeyData = new SerialKeyService().GetSerialKeyList(
				(string)sqlParam[Constants.FIELD_SERIALKEY_SERIAL_KEY + Constants.FLG_LIKE_ESCAPED],
				(string)sqlParam[Constants.FIELD_SERIALKEY_PRODUCT_ID + Constants.FLG_LIKE_ESCAPED],
				(string)sqlParam[Constants.FIELD_SERIALKEY_USER_ID + Constants.FLG_LIKE_ESCAPED],
				(string)sqlParam[Constants.FIELD_SERIALKEY_ORDER_ID + Constants.FLG_LIKE_ESCAPED],
				(string)sqlParam[Constants.FIELD_SERIALKEY_STATUS],
				(string)sqlParam[Constants.FIELD_SERIALKEY_VALID_FLG],
				(string)sqlParam["sort_kbn"],
				Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1,
				Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo);

			// Redirect to last page when current page no don't have any data
			CheckRedirectToLastPage(
				serialKeyData.Length,
				totalCount,
				CreateSerialKeyListUrlWithoutPageNo(GetSearchInfoFromControl()));

			rList.DataSource = serialKeyData;
			rList.DataBind();
		}
	}

	/// <summary>
	/// シリアルキー一覧パラメタ取得
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters()
	{
		Hashtable paramResult = new Hashtable();
		try
		{
			// シリアルキー
			paramResult.Add(Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY]));
			tbSerialKey.Text = SerialKeyUtility.DecryptSerialKey((string)paramResult[Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY]);

			// 商品ID
			paramResult.Add(Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID]));
			tbProductId.Text = (string)paramResult[Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID];

			// ユーザーID
			paramResult.Add(Constants.REQUEST_KEY_SERIALKEY_USER_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SERIALKEY_USER_ID]));
			tbUserId.Text = (string)paramResult[Constants.REQUEST_KEY_SERIALKEY_USER_ID];

			// 注文ID
			paramResult.Add(Constants.REQUEST_KEY_SERIALKEY_ORDER_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SERIALKEY_ORDER_ID]));
			tbOrderId.Text = (string)paramResult[Constants.REQUEST_KEY_SERIALKEY_ORDER_ID];

			// 状態
			paramResult.Add(Constants.REQUEST_KEY_SERIALKEY_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SERIALKEY_STATUS]));
			ddlStatus.SelectedValue = (string)paramResult[Constants.REQUEST_KEY_SERIALKEY_STATUS];

			// 有効フラグ
			paramResult.Add(Constants.REQUEST_KEY_SERIALKEY_VALID_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SERIALKEY_VALID_FLG]));
			ddlValidFlg.SelectedValue = (string)Request[Constants.REQUEST_KEY_SERIALKEY_VALID_FLG];

			// ソート
			switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_SERIALKEY_LIST_SERIAL_KEY_ASC:
				case Constants.KBN_SORT_SERIALKEY_LIST_SERIAL_KEY_DESC:
				case Constants.KBN_SORT_SERIALKEY_LIST_PRODUCT_ID_ASC:
				case Constants.KBN_SORT_SERIALKEY_LIST_PRODUCT_ID_DESC:
				case Constants.KBN_SORT_SERIALKEY_LIST_USER_ID_ASC:
				case Constants.KBN_SORT_SERIALKEY_LIST_USER_ID_DESC:
				case Constants.KBN_SORT_SERIALKEY_LIST_ORDER_ID_ASC:
				case Constants.KBN_SORT_SERIALKEY_LIST_ORDER_ID_DESC:
				case Constants.KBN_SORT_SERIALKEY_LIST_DATE_CREATED_ASC:
				case Constants.KBN_SORT_SERIALKEY_LIST_DATE_CREATED_DESC:
				case Constants.KBN_SORT_SERIALKEY_LIST_DATE_CHANGED_ASC:
				case Constants.KBN_SORT_SERIALKEY_LIST_DATE_CHANGED_DESC:
					paramResult.Add(Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN]);
					break;
				default:
					paramResult.Add(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_SERIALKEY_LIST_DEFAULT);
					break;
			}
			ddlSortKbn.SelectedValue = (string)paramResult[Constants.REQUEST_KEY_SORT_KBN];

			// ページ番号（ページャ動作時のみもちまわる）
			int pageNo;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) == false)
			{
				pageNo = 1;
			}
			paramResult.Add(Constants.REQUEST_KEY_PAGE_NO, pageNo.ToString());
			this.CurrentPageNo = pageNo;
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return paramResult;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable searchParam)
	{
		Hashtable searchResult = new Hashtable();
		// シリアルキー
		searchResult.Add(Constants.FIELD_SERIALKEY_SERIAL_KEY + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY]));
		// 商品ID
		searchResult.Add(Constants.FIELD_SERIALKEY_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID]));
		// ユーザーID
		searchResult.Add(Constants.FIELD_SERIALKEY_USER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_SERIALKEY_USER_ID]));
		// 注文ID
		searchResult.Add(Constants.FIELD_SERIALKEY_ORDER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_SERIALKEY_ORDER_ID]));
		// 状態
		searchResult.Add(Constants.FIELD_SERIALKEY_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_SERIALKEY_STATUS]));
		// 有効フラグ
		searchResult.Add(Constants.FIELD_SERIALKEY_VALID_FLG, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_SERIALKEY_VALID_FLG]));
		// ソート区分
		searchResult.Add("sort_kbn", (string)searchParam[Constants.REQUEST_KEY_SORT_KBN]);

		return searchResult;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfoFromControl()
	{
		Hashtable searchParam = new Hashtable();
		// シリアルキー
		searchParam.Add(Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY, SerialKeyUtility.EncryptSerialKey(tbSerialKey.Text.Trim()));
		// 商品ID
		searchParam.Add(Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID, tbProductId.Text.Trim());
		// ユーザーID
		searchParam.Add(Constants.REQUEST_KEY_SERIALKEY_USER_ID, tbUserId.Text.Trim());
		// 注文ID
		searchParam.Add(Constants.REQUEST_KEY_SERIALKEY_ORDER_ID, tbOrderId.Text.Trim());
		// 状態
		searchParam.Add(Constants.REQUEST_KEY_SERIALKEY_STATUS, ddlStatus.SelectedValue);
		// 有効フラグ
		searchParam.Add(Constants.REQUEST_KEY_SERIALKEY_VALID_FLG, ddlValidFlg.SelectedValue);
		// ソート区分
		searchParam.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);

		return searchParam;
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// 検索用パラメタ作成し、同じ画面にリダイレクト
		Response.Redirect(CreateSerialKeyListUrlWithoutPageNo(GetSearchInfoFromControl()));
	}

	/// <summary>
	/// 新規ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// セッション初期化
		//------------------------------------------------------
		Session[Constants.SESSIONPARAM_KEY_SERIALKEY_INFO] = null;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(CreateSerialKeyRegistUrl("", "", Constants.ACTION_STATUS_INSERT));
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

	/// <summary>カレントページNO</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_PAGE_NO]; }
		set { ViewState[Constants.REQUEST_KEY_PAGE_NO] = value; }
	}
}

