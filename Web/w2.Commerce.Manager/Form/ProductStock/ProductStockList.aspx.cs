/*
=========================================================================================================
  Module      : 商品在庫一覧ページ処理(ProductStockList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Stock;

public partial class Form_ProductStock_ProductStockList : ProductPage
{
	//=========================================================================================
	// 処理結果定数
	//=========================================================================================
	private const string RESULT_PRRODUCTSTOCK = "productstock";				// 商品在庫情報
	private const string RESULT_UPDATE_STOCK_RESULT = "update_stock_result";		// 論理在庫数更新結果
	private const string RESULT_UPDATE_REALSTOCK_RESULT = "update_realstock_result";	// 実在庫数更新結果
	private const string RESULT_UPDATE_REALSTOCK_B_RESULT = "update_realstock_b_result";	// 実在庫数B更新結果
	private const string RESULT_UPDATE_REALSTOCK_C_RESULT = "update_realstock_c_result";	// 実在庫数C更新結果
	private const string RESULT_UPDATE_REALSTOCK_RERSERVED_RESULT = "update_realstock_reserved_result";	// 引当済実在庫数更新結果
	private const string RESULT_UPDATE_STOCKALERT_RESULT = "update_stock_alert_result";	// 安全在庫数更新結果

	//=========================================================================================
	// データバインド用変数
	//=========================================================================================
	protected Hashtable m_htUpdateStockResult = new Hashtable();			// 論理在庫数更新結果データバインド用
	protected Hashtable m_htUpdateRealStockResult = new Hashtable();		// 実在庫数更新結果データバインド用
	protected Hashtable m_htUpdateRealStockBResult = new Hashtable();		// 実在庫数B更新結果データバインド用
	protected Hashtable m_htUpdateRealStockCResult = new Hashtable();		// 実在庫数C更新結果データバインド用
	protected Hashtable m_htUpdateRealStockReservedResult = new Hashtable();// 引当済実在庫数更新結果データバインド用
	protected Hashtable m_htUpdateStockAlertResult = new Hashtable();		// 安全在庫数更新結果データバインド用

	// 在庫数検索カウント用
	private int m_iSearchStockCount = 0;

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
			// 在庫数検索項目
			//------------------------------------------------------
			if (Constants.REALSTOCK_OPTION_ENABLED != true)
			{
				// 実在庫オプションが有効じゃない場合、以下のアイテムを削除
				ddlSearchStockKey.Items.Remove(ddlSearchStockKey.Items.FindByValue("realstock"));
				ddlSearchStockKey.Items.Remove(ddlSearchStockKey.Items.FindByValue("realstock_b"));
				ddlSearchStockKey.Items.Remove(ddlSearchStockKey.Items.FindByValue("realstock_c"));
			}

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable htParam = GetParameters(Request);

			if (this.IsNotSearchDefault) return;

			//------------------------------------------------------
			// 検索コントロール制御（商品在庫一覧共通処理）
			//------------------------------------------------------
			// ページ番号
			int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];
			ViewState[Constants.REQUEST_KEY_PAGE_NO] = iCurrentPageNumber;

			// 在庫アラート区分
			foreach (ListItem li in rlStockAlert.Items)
			{
				li.Selected = (li.Value == (string)htParam[Constants.REQUEST_KEY_STOCK_ALERT_KBN]);
			}

			// 在庫数検索項目
			foreach (ListItem li in ddlSearchStockKey.Items)
			{
				li.Selected = (li.Value == (string)htParam[Constants.REQUEST_KEY_SEARCH_STOCK_KEY]);
			}
			m_iSearchStockCount = (int)htParam[Constants.REQUEST_KEY_SEARCH_STOCK_COUNT];
			tbSearchStockCount.Text = m_iSearchStockCount.ToString();
			foreach (ListItem li in ddlSearchProductCountType.Items)
			{
				li.Selected = (li.Value == (string)htParam[Constants.REQUEST_KEY_SEARCH_STOCK_COUNT_TYPE]);
			}

			// 文字検索項目
			foreach (ListItem li in ddlSearchKey.Items)
			{
				li.Selected = (li.Value == (string)htParam[Constants.REQUEST_KEY_SEARCH_KEY]);
			}

			// 検索ワード
			tbSearchWord.Text = (string)htParam[Constants.REQUEST_KEY_SEARCH_WORD];

			// ソート項目
			foreach (ListItem li in ddlSortKbn.Items)
			{
				li.Selected = (li.Value == (string)htParam[Constants.REQUEST_KEY_SORT_KBN]);
			}

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			Hashtable htSqlParam = new Hashtable();
			htSqlParam.Add("srch_key", tbSearchWord.Text != "" ? ddlSearchKey.SelectedValue : "99");	// 検索フィールド
			htSqlParam.Add("srch_word_like_escaped",
				StringUtility.SqlLikeStringSharpEscape(tbSearchWord.Text));								// 検索値
			htSqlParam.Add("srch_stock_key", ddlSearchStockKey.SelectedValue);							// 在庫数検索キー
			htSqlParam.Add("srch_stock_count", tbSearchStockCount.Text);								// 在庫数検索数
			htSqlParam.Add("srch_stock_count_type", ddlSearchProductCountType.SelectedValue);			// 在庫数検索タイプ
			htSqlParam.Add("stock_alert_kbn", rlStockAlert.SelectedValue);								// 商品在庫区分
			htSqlParam.Add("sort_kbn", ddlSortKbn.SelectedValue);										// ソート区分
			htSqlParam.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, this.LoginOperatorShopId);				// 店舗ID
			htSqlParam.Add("bgn_row_num",
				Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1);				// 表示開始記事番号
			htSqlParam.Add("end_row_num",
				Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber);						// 表示終了記事番号

			//------------------------------------------------------
			// 商品在庫該当件数取得
			//------------------------------------------------------
			int iTotalProductStockCounts = 0;	// ページング可能総商品数
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "GetProductStockCount"))
			{
				DataView dvProductStockCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
				if (dvProductStockCount.Count != 0)
				{
					iTotalProductStockCounts = (int)dvProductStockCount[0]["row_count"];
				}
			}

			//------------------------------------------------------
			// エラー表示制御
			//------------------------------------------------------
			bool blDisplayPager = true;
			StringBuilder sbErrorMessage = new StringBuilder();
			// 上限件数より多い？
			if (iTotalProductStockCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				sbErrorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(iTotalProductStockCounts));
				sbErrorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				blDisplayPager = false;
			}
			// 該当件数なし？
			else if (iTotalProductStockCounts == 0)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdListErrorMessage.InnerHtml = tdListEditErrorMessage.InnerHtml = sbErrorMessage.ToString();
			trListError.Visible = trListEditError.Visible = (sbErrorMessage.ToString().Length != 0);

			//------------------------------------------------------
			// 商品在庫一覧表示
			//------------------------------------------------------
			DataView dvProductStock = null;
			if (trListError.Visible == false)
			{
				// 商品在庫一覧情報取得
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "GetProductStockList"))
				{
					DataSet dsProductStock = sqlStatement.SelectStatementWithOC(sqlAccessor, htSqlParam);
					dvProductStock = dsProductStock.Tables["Table"].DefaultView;

					// 商品在庫情報をビューステートに保存
					ViewState.Add(Constants.TABLE_PRODUCTSTOCK, dsProductStock);
				}
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (blDisplayPager)
			{
				// ページャ作成（一覧処理で総件数を取得）
				string strNextUrl = CreateProductStockListUrl(
					ddlSearchKey.SelectedValue,
					tbSearchWord.Text,
					ddlSearchStockKey.SelectedValue,
					m_iSearchStockCount,
					ddlSearchProductCountType.SelectedValue,
					ddlSortKbn.SelectedValue,
					rlStockAlert.SelectedValue,
					(string)htParam[Constants.REQUEST_KEY_DISPLAY_KBN]);

				lbPager1.Text = WebPager.CreateDefaultListPagerWithLimitNumberPage(iTotalProductStockCounts, iCurrentPageNumber, strNextUrl, 1);
			}

			// 一覧表示
			if ((string)htParam[Constants.REQUEST_KEY_DISPLAY_KBN] == Constants.KBN_PRODUCTSTOCK_DISPLAY_LIST)
			{
				tdDispList.Visible = true;
				tdDispEditList.Visible = false;
				// 編集できないようにする
				btnEditTop.Visible = btnEditBottom.Visible = (trListError.Visible == false);
				btnListTop.Visible = false;
				btnListBottom.Visible = false;
				btnStockUpdateTop.Visible = false;
				btnStockUpdateBottom.Visible = false;
				divStockEdit.Visible = true;
				divStockComplete.Visible = false;
				trList.Visible = true;
				trEdit.Visible = false;

				rList.DataSource = dvProductStock;
				rList.DataBind();

				rTableFixColumn.DataSource = dvProductStock;
				rTableFixColumn.DataBind();
			}
			// 編集表示
			else if ((string)htParam[Constants.REQUEST_KEY_DISPLAY_KBN] == Constants.KBN_PRODUCTSTOCK_DISPLAY_EDIT)
			{
				tdDispList.Visible = false;
				tdDispEditList.Visible = true;
				btnEditTop.Visible = false;
				btnEditBottom.Visible = false;
				btnListTop.Visible = true;
				btnListBottom.Visible = true;
				// 更新できないようにする
				btnStockUpdateTop.Visible = btnStockUpdateBottom.Visible = (trListEditError.Visible == false);
				divStockEdit.Visible = true;
				divStockComplete.Visible = false;
				trList.Visible = false;
				trEdit.Visible = true;

				rEdit.DataSource = dvProductStock;
				rEdit.DataBind();

				rEditTableFixColumn.DataSource = dvProductStock;
				rEditTableFixColumn.DataBind();
			}
			// 完了表示
			else if ((string)htParam[Constants.REQUEST_KEY_DISPLAY_KBN] == Constants.KBN_PRODUCTSTOCK_DISPLAY_COMPLETE)
			{
				// 処理結果取得
				Hashtable htUpdateStock = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				m_htUpdateStockResult = (Hashtable)htUpdateStock[RESULT_UPDATE_STOCK_RESULT];
				m_htUpdateRealStockResult = (Hashtable)htUpdateStock[RESULT_UPDATE_REALSTOCK_RESULT];
				m_htUpdateRealStockBResult = (Hashtable)htUpdateStock[RESULT_UPDATE_REALSTOCK_B_RESULT];
				m_htUpdateRealStockCResult = (Hashtable)htUpdateStock[RESULT_UPDATE_REALSTOCK_C_RESULT];
				m_htUpdateRealStockReservedResult = (Hashtable)htUpdateStock[RESULT_UPDATE_REALSTOCK_RERSERVED_RESULT];
				m_htUpdateStockAlertResult = (Hashtable)htUpdateStock[RESULT_UPDATE_STOCKALERT_RESULT];

				divStockEdit.Visible = false;
				divStockComplete.Visible = true;
				trList.Visible = false;
				trEdit.Visible = false;

				// 検索処理が行われた場合は、編集表示へ遷移させる
				htParam[Constants.REQUEST_KEY_DISPLAY_KBN] = Constants.KBN_PRODUCTSTOCK_DISPLAY_EDIT;

				// データセット
				rComplete.DataSource = (ArrayList)htUpdateStock[RESULT_PRRODUCTSTOCK];
				rComplete.DataBind();
			}

			// ポップアップ表示制御（タイトルを非表示へ）
			trTitleProductTop.Visible = trTitleProductMiddle.Visible = trTitleProductBottom.Visible = (this.IsPopUp == false);

			// 表示区分をビューステートに保存(検索イベント用)
			ViewState[Constants.REQUEST_KEY_DISPLAY_KBN] = htParam[Constants.REQUEST_KEY_DISPLAY_KBN];

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind();

			//------------------------------------------------------
			// エンターキーでのSubmitを無効とする領域を設定する
			// ※RepeaterがBindされていないと正常に動作しない
			//------------------------------------------------------
			// KeyEventをキャンセルするスクリプトを設定
			new InnerTextBoxList(tdDispEditList).SetKeyPressEventCancelEnterKey();
		}
		else
		{
			if (int.TryParse(StringUtility.ToEmpty(tbSearchStockCount.Text), out m_iSearchStockCount) == false)
			{
				m_iSearchStockCount = 0;
			}
		}
	}

	/// <summary>
	/// 商品在庫一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">商品在庫一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	private Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable htResult = new Hashtable();

		int iCurrentPageNumber = 1;
		string strSearchKey = "";
		string strSearchWord = "";
		string strSortKbn = "";
		string strStockAlertKbn = "";
		string strDisplayKbn = "";
		string strSearchStockKey = "";
		int iSearchStockCount = 0;
		string strSearchStockCountType = "";
		bool blParamError = false;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// 表示区分
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_DISPLAY_KBN]))
			{
				case Constants.KBN_PRODUCTSTOCK_DISPLAY_LIST:						// 一覧表示
				case Constants.KBN_PRODUCTSTOCK_DISPLAY_EDIT:						// 編集表示
				case Constants.KBN_PRODUCTSTOCK_DISPLAY_COMPLETE:					// 完了表示
					strDisplayKbn = hrRequest[Constants.REQUEST_KEY_DISPLAY_KBN].ToString();
					break;
				case "":
					strDisplayKbn = Constants.KBN_PRODUCTSTOCK_DISPLAY_DEFAULT;		// 一覧表示がデフォルト
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
		// 検索キー
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SEARCH_KEY]))
			{
				case Constants.KBN_SEARCHKEY_PRODUCTSTOCK_LIST_PRODUCT_ID:						// 商品ID
				case Constants.KBN_SEARCHKEY_PRODUCTSTOCK_LIST_NAME:							// 商品名
				case Constants.KBN_SEARCHKEY_PRODUCTSTOCK_LIST_NAME_KANA:						// 商品名(フリガナ)
				case Constants.KBN_SEARCHKEY_PRODUCTSTOCK_LIST_CATEGORY_ID:						// カテゴリID
					strSearchKey = hrRequest[Constants.REQUEST_KEY_SEARCH_KEY].ToString();
					break;
				case "":
					strSearchKey = Constants.KBN_SEARCHKEY_PRODUCTSTOCK_LIST_DEFAULT;			// 商品IDがデフォルト
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
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_PRODUCT_ID_ASC:				// 商品ID/昇順
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_PRODUCT_ID_DESC:				// 商品ID/降順
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_NAME_ASC:						// 商品名/昇順
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_NAME_DESC:					// 商品名/降順
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_NAME_KANA_ASC:				// 商品フリガナ/昇順
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_NAME_KANA_DESC:				// 商品フリガナ/降順
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_DATE_CREATED_ASC:				// 作成日/昇順
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_DATE_CREATED_DESC:			// 作成日/降順
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_DATE_CHANGED_ASC:				// 更新日/昇順
				case Constants.KBN_SORT_PRODUCTSTOCK_LIST_DATE_CHANGED_DESC:			// 更新日/降順
					strSortKbn = hrRequest[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				case "":
					strSortKbn = Constants.KBN_SORT_PRODUCTSTOCK_LIST_DEFAULT;			// 商品ID/昇順がデフォルト
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

		// 商品在庫区分
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_STOCK_ALERT_KBN]))
			{
				case Constants.KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_ALL:						// 在庫管理商品全て
				case Constants.KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_ALERT:						// 安全在庫アラート
				case Constants.KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_SUSPENSION:					// 在庫切れ販売停止
					strStockAlertKbn = hrRequest[Constants.REQUEST_KEY_STOCK_ALERT_KBN].ToString();
					break;
				case "":
					strStockAlertKbn = Constants.KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_DEFAULT;	// 商品ID/昇順がデフォルト
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

		// 在庫数検索項目
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SEARCH_STOCK_KEY]))
			{
				case Constants.KBN_PRODUCTSTOCK_SEARCH_STOCK_KEY_STOCK:
				case Constants.KBN_PRODUCTSTOCK_SEARCH_STOCK_KEY_REALSTOCK:
				case Constants.KBN_PRODUCTSTOCK_SEARCH_STOCK_KEY_REALSTOCK_B:
				case Constants.KBN_PRODUCTSTOCK_SEARCH_STOCK_KEY_REALSTOCK_C:
					strSearchStockKey = hrRequest[Constants.REQUEST_KEY_SEARCH_STOCK_KEY].ToString();
					break;
				case "":
					strSearchStockKey = "";	// 空文字がデフォルト
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
		if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SEARCH_STOCK_COUNT]), out iSearchStockCount) == false)
		{
			iSearchStockCount = 0;
		}
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SEARCH_STOCK_COUNT_TYPE]))
			{
				case Constants.KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_EQUAL:
				case Constants.KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_OVER:
				case Constants.KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_UNDER:
					strSearchStockCountType = hrRequest[Constants.REQUEST_KEY_SEARCH_STOCK_COUNT_TYPE].ToString();
					break;
				case "":
					strSearchStockCountType = Constants.KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_DEFAULT;	// 等しいがデフォルト
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

		//------------------------------------------------------
		// 不正パラメータが存在した場合エラーページへ
		//------------------------------------------------------
		if (blParamError)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_KEY, strSearchKey);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_WORD, strSearchWord);
		htResult.Add(Constants.REQUEST_KEY_SORT_KBN, strSortKbn);
		htResult.Add(Constants.REQUEST_KEY_STOCK_ALERT_KBN, strStockAlertKbn);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_STOCK_KEY, strSearchStockKey);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_STOCK_COUNT, iSearchStockCount);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_STOCK_COUNT_TYPE, strSearchStockCountType);
		htResult.Add(Constants.REQUEST_KEY_DISPLAY_KBN, strDisplayKbn);

		return htResult;
	}

	/// <summary>
	/// 商品在庫一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <param name="strStockAlertKbn">商品在庫区分</param>
	/// <param name="strDisplayKbn">表示区分</param>
	/// <returns>商品在庫一覧遷移URL</returns>
	private string CreateProductStockListUrl(
		string strSearchKey,
		string strSearchWord,
		string strSearchStockKey,
		int iSearchStockCount,
		string strSearchStockCountType,
		string strSortKbn,
		string strStockAlertKbn,
		string strDisplayKbn)
	{
		StringBuilder sbResult = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSTOCK_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_SEARCH_KEY).Append("=").Append(strSearchKey);
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SEARCH_WORD).Append("=").Append(HttpUtility.UrlEncode(strSearchWord));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SEARCH_STOCK_KEY).Append("=").Append(strSearchStockKey);
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SEARCH_STOCK_COUNT).Append("=").Append(iSearchStockCount.ToString());
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SEARCH_STOCK_COUNT_TYPE).Append("=").Append(strSearchStockCountType);
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(strSortKbn);
		sbResult.Append("&").Append(Constants.REQUEST_KEY_STOCK_ALERT_KBN).Append("=").Append(strStockAlertKbn);
		sbResult.Append("&").Append(Constants.REQUEST_KEY_DISPLAY_KBN).Append("=").Append(strDisplayKbn);

		return sbResult.ToString();
	}

	/// <summary>
	/// 商品在庫一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <param name="strStockAlertKbn">商品在庫区分</param>
	/// <param name="strDisplayKbn">表示区分</param>
	/// <param name="iPageNumber">ページ番号</param>
	/// <returns>商品在庫一覧遷移URL</returns>
	private string CreateProductStockListUrl(
		string strSearchKey,
		string strSearchWord,
		string strSearchStockKey,
		int iSearchStockCount,
		string strSearchStockCountType,
		string strSortKbn,
		string strStockAlertKbn,
		string strDisplayKbn,
		int iPageNumber)
	{
		StringBuilder sbResult = new StringBuilder(CreateProductStockListUrl(strSearchKey, strSearchWord, strSearchStockKey, iSearchStockCount, strSearchStockCountType, strSortKbn, strStockAlertKbn, strDisplayKbn));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(iPageNumber.ToString());

		return sbResult.ToString();
	}

	/// <summary>
	/// データバインド用商品在庫履歴一覧URL作成
	/// </summary>
	/// <param name="strProductId">商品ID</param>
	/// <param name="strVariationId">バリエーションID</param>
	/// <returns></returns>
	protected string CreateProductStockHistoryList(string strProductId, string strVariationId)
	{
		System.Text.StringBuilder sbResult = new System.Text.StringBuilder();

		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSTOCKHISTORY_LIST);
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(strProductId));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_VARIATION_ID).Append("=").Append(HttpUtility.UrlEncode(strVariationId));

		return sbResult.ToString();

	}

	/// <summary>
	/// 行データハッシュテーブル取得
	/// </summary>
	/// <param name="dv">データビュー</param>
	/// <param name="iRow">行番号</param>
	/// <returns>行データ</returns>
	private Hashtable GetRowInfoHashtable(DataView dv, int iRow)
	{
		Hashtable htResult = new Hashtable();

		// データが存在する場合
		if (dv.Count != 0)
		{
			DataRow dr = dv[iRow].Row;	// 指定行データ取得

			// Hashtabe格納
			foreach (DataColumn dc in dr.Table.Columns)
			{
				htResult.Add(dc.ColumnName, dr[dc.ColumnName]);
			}
		}

		return htResult;
	}

	/// <summary>
	/// 更新結果表示
	/// </summary>
	/// <param name="objResult">結果(null/true/false)</param>
	/// <returns>結果文字列</returns>
	protected string DisplayResult(object objResult)
	{
		string strResult = null;

		if (objResult == null)
		{
			strResult = "－";
		}
		else if ((bool)objResult == true)
		{
			strResult = "○";
		}
		else if ((bool)objResult == false)
		{
			strResult = "×";
		}

		return strResult;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		string strUrl = CreateProductStockListUrl(
				ddlSearchKey.SelectedValue,
				tbSearchWord.Text.Trim(),
				ddlSearchStockKey.SelectedValue,
				m_iSearchStockCount,
				ddlSearchProductCountType.SelectedValue,
				ddlSortKbn.SelectedValue,
				rlStockAlert.SelectedValue,
				(string)ViewState[Constants.REQUEST_KEY_DISPLAY_KBN],
				1);

		// 商品在庫一覧(参照)へ
		Response.Redirect(strUrl);
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		string strUrl = CreateProductStockListUrl(
			ddlSearchKey.SelectedValue,
			tbSearchWord.Text.Trim(),
			ddlSearchStockKey.SelectedValue,
			m_iSearchStockCount,
			ddlSearchProductCountType.SelectedValue,
			ddlSortKbn.SelectedValue,
			rlStockAlert.SelectedValue,
			Constants.KBN_PRODUCTSTOCK_DISPLAY_EDIT,
			(int)ViewState[Constants.REQUEST_KEY_PAGE_NO]);

		// 商品在庫一覧(編集)へ
		Response.Redirect(strUrl);
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnList_Click(object sender, System.EventArgs e)
	{
		string strUrl = CreateProductStockListUrl(
			ddlSearchKey.SelectedValue,
			tbSearchWord.Text.Trim(),
			ddlSearchStockKey.SelectedValue,
			m_iSearchStockCount,
			ddlSearchProductCountType.SelectedValue,
			ddlSortKbn.SelectedValue,
			rlStockAlert.SelectedValue,
			Constants.KBN_PRODUCTSTOCK_DISPLAY_LIST,
			(int)ViewState[Constants.REQUEST_KEY_PAGE_NO]);

		// 商品在庫一覧(参照)へ
		Response.Redirect(strUrl);
	}

	/// <summary>
	/// このページの一括更新クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnStockUpdate_Click(object sender, System.EventArgs e)
	{
		StringBuilder sbErrorMessages = new StringBuilder();
		ArrayList alUpdateStock = new ArrayList();
		Hashtable htUpdateRealStock = new Hashtable();
		Hashtable htUpdateRealStockB = new Hashtable();
		Hashtable htUpdateRealStockC = new Hashtable();
		Hashtable htUpdateRealStockReserved = new Hashtable();
		Hashtable htUpdateStock = new Hashtable();
		Hashtable htUpdateStockAlert = new Hashtable();
		string strStockAdd = String.Empty;
		string strRealStockAdd = String.Empty;
		string strRealStockAddB = String.Empty;
		string strRealStockAddC = String.Empty;
		string strRealStockReservedAdd = String.Empty;
		string strStockAlertBefore = String.Empty;
		string strStockAlertAfter = String.Empty;
		string strProductId = String.Empty;
		string stVId = String.Empty;
		string strVariationId = String.Empty;
		bool blUpdateRealStock = false;
		bool blUpdateRealStockB = false;
		bool blUpdateRealStockC = false;
		bool blUpdateRealStockReserved = false;
		bool blUpdateStock = false;
		bool blUpdateStockAlert = false;
		bool blSelected = false;

		// 商品在庫情報取得
		DataView dvStock = ((DataSet)ViewState[Constants.TABLE_PRODUCTSTOCK]).Tables["Table"].DefaultView;

		// 入力チェック
		for (int iLoop = 0; iLoop < dvStock.Count; iLoop++)
		{
			// 論理在庫(追加数（＋）)
			strStockAdd = ((TextBox)(rEdit.Items[iLoop].FindControl("tbStock"))).Text;

			// 実在庫が有効な場合
			if (Constants.REALSTOCK_OPTION_ENABLED)
			{
				// 実在庫(追加数（＋）)
				strRealStockAdd = ((TextBox)(rEdit.Items[iLoop].FindControl("tbRealStock"))).Text;
				strRealStockAddB = ((TextBox)(rEdit.Items[iLoop].FindControl("tbRealStockB"))).Text;
				strRealStockAddC = ((TextBox)(rEdit.Items[iLoop].FindControl("tbRealStockC"))).Text;

				// 引当済実在庫(追加数（＋）)
				strRealStockReservedAdd = ((TextBox)(rEdit.Items[iLoop].FindControl("tbRealStockReserved"))).Text;
			}

			// 安全基準値（変更後）
			strStockAlertAfter = ((TextBox)(rEdit.Items[iLoop].FindControl("tbStockAlert"))).Text;

			// 入力値をハッシュテーブルに格納
			Hashtable htInput = new Hashtable();

			htInput.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, strStockAdd);									// 論理在庫(追加在庫数)
			// 実在庫が有効な場合
			if (Constants.REALSTOCK_OPTION_ENABLED)
			{
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, strRealStockAdd);						// 実在庫(追加在庫数)
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, strRealStockAddB);					// 実在庫B品(追加在庫数)
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, strRealStockAddC);					// 実在庫C品(追加在庫数)
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, strRealStockReservedAdd);		// 引当済み実在庫(追加在庫数)
			}
			else
			{
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, "0");									// 実在庫(追加在庫数)
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, "0");									// 実在庫B品(追加在庫数)
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, "0");									// 実在庫C品(追加在庫数)
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, "0");							// 引当済み実在庫(追加在庫数)
			}
			htInput.Add(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT, strStockAlertAfter);						// 変更後安全基準値

			// 入力チェック
			string strProductName = (string)dvStock[iLoop][Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID] + CreateProductAndVariationName(dvStock[iLoop]);
			sbErrorMessages.Append(Validator.Validate("ProductStock", htInput).Replace("@@ 1 @@", strProductName));
		}

		if (sbErrorMessages.Length != 0)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 在庫数,安全基準値変更
		for (int iLoop = 0; iLoop < dvStock.Count; iLoop++)
		{
			// 商品ID
			strProductId = (string)dvStock[iLoop][Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID];

			// バリエーションID
			stVId = (string)dvStock[iLoop][Constants.FIELD_PRODUCTVARIATION_V_ID];
			strVariationId = strProductId + stVId;

			// 論理在庫(追加数（＋）)
			strStockAdd = ((TextBox)(rEdit.Items[iLoop].FindControl("tbStock"))).Text;

			// 実在庫が有効な場合
			if (Constants.REALSTOCK_OPTION_ENABLED)
			{
				// 実在庫(追加数（＋）)
				strRealStockAdd = ((TextBox)(rEdit.Items[iLoop].FindControl("tbRealStock"))).Text;
				strRealStockAddB = ((TextBox)(rEdit.Items[iLoop].FindControl("tbRealStockB"))).Text;
				strRealStockAddC = ((TextBox)(rEdit.Items[iLoop].FindControl("tbRealStockC"))).Text;

				// 引当済実在庫(追加数（＋）)
				strRealStockReservedAdd = ((TextBox)(rEdit.Items[iLoop].FindControl("tbRealStockReserved"))).Text;
			}
			else
			{
				// 実在庫(追加数（＋）)
				strRealStockAdd = "0";
				strRealStockAddB = "0";
				strRealStockAddC = "0";

				// 引当済実在庫(追加数（＋）)
				strRealStockReservedAdd = "0";
			}

			// 安全基準値（変更前）
			strStockAlertBefore = dvStock[iLoop][Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT].ToString();

			// 安全基準値（変更後）
			strStockAlertAfter = ((TextBox)(rEdit.Items[iLoop].FindControl("tbStockAlert"))).Text;

			// 更新対象
			blUpdateStock = int.Parse(strStockAdd) != 0;
			blUpdateRealStock = int.Parse(strRealStockAdd) != 0;
			blUpdateRealStockB = int.Parse(strRealStockAddB) != 0;
			blUpdateRealStockC = int.Parse(strRealStockAddC) != 0;
			blUpdateRealStockReserved = int.Parse(strRealStockReservedAdd) != 0;

			blUpdateStockAlert = int.Parse(strStockAlertBefore) != int.Parse(strStockAlertAfter);

			// 完了表示用に格納
			// null値は処理対象外
			htUpdateStock.Add(strProductId + strVariationId, blUpdateStock ? (object)true : null);
			htUpdateRealStock.Add(strProductId + strVariationId, blUpdateRealStock ? (object)true : null);
			htUpdateRealStockB.Add(strProductId + strVariationId, blUpdateRealStockB ? (object)true : null);
			htUpdateRealStockC.Add(strProductId + strVariationId, blUpdateRealStockC ? (object)true : null);
			htUpdateRealStockReserved.Add(strProductId + strVariationId, blUpdateRealStockReserved ? (object)true : null);
			htUpdateStockAlert.Add(strProductId + strVariationId, blUpdateStockAlert ? (object)true : null);

			// 論理在庫数,実在庫数,引当済実在個数,安全基準値が変更されていた場合
			if (blUpdateStock || blUpdateRealStock || blUpdateRealStockB || blUpdateRealStockC || blUpdateRealStockReserved || blUpdateStockAlert)
			{
				// 入力値をハッシュテーブルに格納
				Hashtable htInput = new Hashtable();
				// 商品在庫情報
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, this.LoginOperatorShopId);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, strProductId);
				htInput.Add(Constants.FIELD_PRODUCTVARIATION_V_ID, stVId);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, strVariationId);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID + Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, strProductId + strVariationId);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, strStockAdd);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, strRealStockAdd);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, strRealStockAddB);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, strRealStockAddC);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, strRealStockReservedAdd);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT, strStockAlertAfter);
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, this.LoginOperatorName);

				// 商品在庫履歴情報
				if (blUpdateStock || blUpdateRealStock || blUpdateRealStockB || blUpdateRealStockC || blUpdateRealStockReserved)
				{
					htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, "");
					htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_OPERATION);	// 在庫管理操作
					htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, strStockAdd);
					htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, strRealStockAdd);
					htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, strRealStockAddB);
					htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, strRealStockAddC);
					htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, strRealStockReservedAdd);
					htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, ((TextBox)(rEdit.Items[iLoop].FindControl("tbUpdateMemo"))).Text);
				}

				StockCommon stockCommon  = new StockCommon();
				stockCommon.UpdateProductStock(
					(string)htInput[Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID],
					(string)htInput[Constants.FIELD_PRODUCTSTOCK_SHOP_ID],
					(string)htInput[Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID],
					(string)htInput[Constants.FIELD_PRODUCTSTOCK_VARIATION_ID],
					(string)htInput[Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED],
					Convert.ToInt32(htInput[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT]),
					Convert.ToInt32(htInput[Constants.FIELD_PRODUCTSTOCK_STOCK]),
					Convert.ToInt32(htInput[Constants.FIELD_PRODUCTSTOCK_REALSTOCK]),
					Convert.ToInt32(htInput[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B]),
					Convert.ToInt32(htInput[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C]),
					Convert.ToInt32(htInput[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED]),
					(string)htInput[Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS],
					(string)htInput[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO],
					(blUpdateStock || blUpdateRealStock || blUpdateRealStockB || blUpdateRealStockC || blUpdateRealStockReserved));

				// 完了表示用に格納
				alUpdateStock.Add(htInput); // 商品在庫情報

				// 論理在庫数更新対象の場合
				if (htUpdateStock[strProductId + strVariationId] != null)
				{
					htUpdateStock[strProductId + strVariationId] = blUpdateStock;		                // 論理在庫(在庫数更新結果)
				}
				// 実在庫数更新対象の場合
				if (htUpdateRealStock[strProductId + strVariationId] != null)
				{
					htUpdateRealStock[strProductId + strVariationId] = blUpdateRealStock;		        // 実在庫(在庫数更新結果)
				}
				// 実在庫数B品更新対象の場合
				if (htUpdateRealStockB[strProductId + strVariationId] != null)
				{
					htUpdateRealStockB[strProductId + strVariationId] = blUpdateRealStockB;		        // 実在庫B(在庫数更新結果)
				}
				// 実在庫数C品更新対象の場合
				if (htUpdateRealStockC[strProductId + strVariationId] != null)
				{
					htUpdateRealStockC[strProductId + strVariationId] = blUpdateRealStockC;		        // 実在庫C(在庫数更新結果)
				}
				// 引当済実在庫数更新対象の場合
				if (htUpdateRealStockReserved[strProductId + strVariationId] != null)
				{
					htUpdateRealStockReserved[strProductId + strVariationId] = blUpdateRealStockReserved;// 実在庫(在庫数更新結果)
				}
				// 安全基準値更新対象の場合
				if (htUpdateStockAlert[strProductId + strVariationId] != null)
				{
					htUpdateStockAlert[strProductId + strVariationId] = blUpdateStockAlert;		    // 安全基準値更新結果
				}

				blSelected = true;
			}
		}

		// 更新対象が無い場合
		if (blSelected == false)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTSTOCK_TARGET_NO_SELECTED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 更新した商品在庫情報を格納
		Hashtable htParam = new Hashtable();
		htParam.Add(RESULT_PRRODUCTSTOCK, alUpdateStock);					// 商品在庫情報
		htParam.Add(RESULT_UPDATE_STOCK_RESULT, htUpdateStock);				// 論理在庫数更新結果
		htParam.Add(RESULT_UPDATE_REALSTOCK_RESULT, htUpdateRealStock);		// 実在庫数更新結果
		htParam.Add(RESULT_UPDATE_REALSTOCK_B_RESULT, htUpdateRealStockB);	// 実在庫数B更新結果
		htParam.Add(RESULT_UPDATE_REALSTOCK_C_RESULT, htUpdateRealStockC);	// 実在庫数C更新結果
		htParam.Add(RESULT_UPDATE_REALSTOCK_RERSERVED_RESULT,
			htUpdateRealStockReserved);										// 引当済実在庫数更新結果
		htParam.Add(RESULT_UPDATE_STOCKALERT_RESULT, htUpdateStockAlert);	// 安全在庫数更新結果
		Session[Constants.SESSION_KEY_PARAM] = htParam;

		// 在庫情報完了ページへ
		string strUrl = CreateProductStockListUrl(
				ddlSearchKey.SelectedValue,
				tbSearchWord.Text.Trim(),
				ddlSearchStockKey.SelectedValue,
				m_iSearchStockCount,
				ddlSearchProductCountType.SelectedValue,
				ddlSortKbn.SelectedValue,
				rlStockAlert.SelectedValue,
				Constants.KBN_PRODUCTSTOCK_DISPLAY_COMPLETE,
				1);

		Response.Redirect(strUrl);
	}

	/// <summary>
	/// 検索値テキストエリアでEnetrキー押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>検索処理を行う</remarks>
	protected void tbSearchWord_TextChanged(object sender, System.EventArgs e)
	{
		// 検索
		this.btnSearch_Click(sender, e);
	}

	/// <summary>
	/// 続けて編集を行うボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btRedirectEdit_Click(object sender, System.EventArgs e)
	{
		string strUrl = CreateProductStockListUrl(
				ddlSearchKey.SelectedValue,
				tbSearchWord.Text.Trim(),
				ddlSearchStockKey.SelectedValue,
				m_iSearchStockCount,
				ddlSearchProductCountType.SelectedValue,
				ddlSortKbn.SelectedValue,
				rlStockAlert.SelectedValue,
				Constants.KBN_PRODUCTSTOCK_DISPLAY_EDIT,
				1);

		// 在庫編集表示
		Response.Redirect(strUrl);
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		Hashtable htParam = new Hashtable();
		htParam.Add("srch_key", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SEARCH_WORD]) != ""
			? StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SEARCH_KEY])
			: Constants.KBN_SEARCHKEY_PRODUCTSTOCK_LIST_DEFAULT);
		htParam.Add("srch_word_like_escaped", StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_SEARCH_WORD]));
		htParam.Add("sort_kbn", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]) != ""
			? StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN])
			: Constants.KBN_SORT_PRODUCTSTOCK_LIST_DEFAULT);
		htParam.Add("srch_stock_key", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SEARCH_STOCK_KEY]));
		htParam.Add("srch_stock_count", m_iSearchStockCount);
		htParam.Add("srch_stock_count_type", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SEARCH_STOCK_COUNT_TYPE]) != ""
			? StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SEARCH_STOCK_COUNT_TYPE])
			: Constants.KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_DEFAULT);
		htParam.Add("stock_alert_kbn", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_STOCK_ALERT_KBN]) != ""
			? StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_STOCK_ALERT_KBN])
			: Constants.KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_DEFAULT);
		htParam.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);

		return htParam;
	}

	/// <summary>プロパティ：表示区分</summary>
	protected string DisplayKbn
	{
		get { return (string)ViewState[Constants.REQUEST_KEY_DISPLAY_KBN]; }
	}
}

