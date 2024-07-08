/*
=========================================================================================================
  Module      : 商品カテゴリ登録ページ処理(ProductCategoryRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Linq;
using w2.App.Common.Product;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_ProductCategory_ProductCategoryRegister : BasePage
{
	protected Hashtable m_htParam = new Hashtable();

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
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			if (strActionStatus != null)
			{
				// 詳細？
				if ((strActionStatus == Constants.ACTION_STATUS_DETAIL)
					|| (strActionStatus == Constants.ACTION_STATUS_COMPLETE))
				{
					// 商品カテゴリID取得
					string strProductCategoryId =
						Request[Constants.REQUEST_KEY_PRODUCTCATEGORY_ID];
					// 詳細画面表示
					ViewProductCategoryDetail(strProductCategoryId);

					// 子カテゴリが存在する場合
					if (HasChildProductCategory(strProductCategoryId))
					{
						// 削除時ダイアログメッセージ変更
						btnDeleteBottom.Attributes["onclick"] = string.Format(
							"return confirm('{0}');",
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CONFIRM_DELETE_CATEGORY_AT_SAME_TIME));
					}

					// 選択カテゴリID保持
					Session[Constants.SESSION_KEY_PRODUCTCATEGORY_CURRENT_CATEGORY_ID] = strProductCategoryId;
				}
				else
				{
					//------------------------------------------------------
					// 処理区分チェック
					//------------------------------------------------------
					CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

					// 最上位カテゴリ登録？
					if (strActionStatus == Constants.ACTION_STATUS_PARENT_INSERT)
					{
						// 戻るボタンにより遷移して来た場合、セッションより商品カテゴリ情報取得
						CheckFromBackbutton((Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO]);
					}
					// 子カテゴリ登録？
					else if (strActionStatus == Constants.ACTION_STATUS_CHILD_INSERT)
					{
						// セッションより商品カテゴリ情報取得
						Hashtable htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO];
						// 親カテゴリIDのみ格納
						m_htParam.Add(Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID,
							htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]);
						// 戻るボタンにより遷移して来た場合、セッションより商品カテゴリ情報取得
						CheckFromBackbutton(htParam);
					}
					// 編集？
					else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
					{
						// セッションより商品カテゴリ情報取得
						m_htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO];
						// 編集前のカテゴリIDをビューステートに格納(入力チェック用)
						ViewState.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_old",
							(m_htParam.Contains(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_old") ?
								m_htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_old"] : m_htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]));
					}
					// 確認
					else if (strActionStatus == Constants.ACTION_STATUS_CONFIRM)
					{
						// セッションより商品カテゴリ情報取得
						m_htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO];
						// ビューステートに格納
						ViewState.Add(Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO, m_htParam);
					}
				}

			}
			// 一覧画面表示
			ViewProductCategoryList();

			// データバインド
			DataBind();

			//------------------------------------------------------
			// 許可ブランドID設定
			//------------------------------------------------------
			string strPermitBrandIds = (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS];

			// 許可ブランドIDが空
			if (strPermitBrandIds == "")
			{
				// 全てを選択
				cbPermitBrand.Checked = true;
			}
			// 許可ブランド設定あり
			else
			{
				foreach (ListItem li in cblPermitBrandIds.Items)
				{
					// カンマ区切り検索
					strPermitBrandIds += ",";
					if (strPermitBrandIds.IndexOf(li.Value + ",") != -1)
					{
						li.Selected = true;
					}
				}
			}

			// 会員ランク表示制御設定
			string strMemberRankId = (string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID];
			foreach (ListItem li in ddlDisplayMemberRank.Items)
			{
				if (li.Value == strMemberRankId)
				{
					li.Selected = true;
				}
			}

			//------------------------------------------------------
			// エンターキーでのSubmitを無効とする領域を設定する
			// ※RepeaterがBindされていないと正常に動作しない
			//------------------------------------------------------
			// KeyEventをキャンセルするスクリプトを設定
			new InnerTextBoxList(trEdit).SetKeyPressEventCancelEnterKey();
		}

		// 許可ブランドIDの表示設定
		cblPermitBrandIds.Enabled = !cbPermitBrand.Checked;

		//------------------------------------------------------
		// 権限制御（CSVダウンロード）
		//------------------------------------------------------
		SetExportAuthority();
	}


	/// <summary>
	/// コンポーネントの初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 変数宣言
		SqlAccessor wsqlAccessor = null;
		SqlStatement wsqlStatement = null;
		DataView dv = null;
		ListItem liTemp = null;

		//------------------------------------------------------
		// 表示コントロール
		//------------------------------------------------------
		// 最上位カテゴリ登録・子カテゴリ登録？
		if (strActionStatus == Constants.ACTION_STATUS_PARENT_INSERT ||
			strActionStatus == Constants.ACTION_STATUS_CHILD_INSERT)
		{
			// 登録画面表示
			trDetail.Visible = false;
			trNotSelect.Visible = false;
			trEdit.Visible = true;
			imgRegister.Visible = true;
		}
		// 編集？
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			// 編集画面
			trDetail.Visible = false;
			trNotSelect.Visible = false;
			trEdit.Visible = true;
			imgEdit.Visible = true;
		}
		// 確認？
		else if (strActionStatus == Constants.ACTION_STATUS_CONFIRM)
		{
			// 確認画面表示
			trDetail.Visible = false;
			trNotSelect.Visible = false;
			trConfirm.Visible = true;
			imgConfirm.Visible = true;
		}
		// 登録完了?
		else if (strActionStatus == Constants.ACTION_STATUS_COMPLETE)
		{
			// 詳細画面
			trDetail.Visible = true;
			divComp.Visible = true;
			lMessage.Text = (string)Session[Constants.SESSION_KEY_ERROR_MSG];
			trNotSelect.Visible = false;
			imgDetail.Visible = true;
		}
		// 詳細
		else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			// 詳細画面
			trDetail.Visible = true;
			divComp.Visible = false;
			trNotSelect.Visible = false;
			//btnDeleteBottom.Attributes["onclick"] = "return confirm('子カテゴリが存在します。同時に削除してもよろしいですか？');";
			imgDetail.Visible = true;
		}
		// 未選択時画面
		else
		{
			imgDetail.Visible = true;
			divNotSelectMessage.InnerHtml =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTCATEGORY_NO_SELECTED_ERROR);
		}

		// 子カテゴリの並び順にドロップダウン設定
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCTCATEGORY, Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN))
		{
			ddlChildCategorySortKbn.Items.Add(li);
		}

		try
		{
			// パラメタ設定
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, this.LoginOperatorShopId);

			// ステートメント取得（usingを用いてdisposeを自動的に呼び出す）
			using (wsqlAccessor = new SqlAccessor())
			using (wsqlStatement = new SqlStatement("ProductCategory", "GetProductCategoryFirst"))
			{
				// ステートメント実行
				dv = wsqlStatement.SelectStatementWithOC(wsqlAccessor, htInput).Tables["Table"].DefaultView;
			}

			// 第一階層商品カテゴリを検索カテゴリドロップダウンリストに設定
			for (int iCount = 0; iCount <= dv.Count - 1; iCount++)
			{
				liTemp = new ListItem();
				liTemp.Value = StringUtility.ToEmpty(
					dv[iCount][Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID].ToString());
				liTemp.Text = StringUtility.ToEmpty(
					dv[iCount][Constants.FIELD_PRODUCTCATEGORY_NAME].ToString());

				ddlSearchKey.Items.Add(liTemp);
			}
		}
		finally
		{
			wsqlAccessor = null;
			wsqlStatement = null;
			dv = null;
			liTemp = null;
		}

		// ブランドID
		foreach (DataRowView drv in ProductBrandUtility.GetProductBrandList())
		{
			cblPermitBrandIds.Items.Add(new ListItem((string)drv[Constants.FIELD_PRODUCTBRAND_BRAND_ID], (string)drv[Constants.FIELD_PRODUCTBRAND_BRAND_ID]));
		}

		// 会員ランク
		DataView dvMemberRank = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatetment = new SqlStatement("MemberRank", "GetMemberRankList"))
		{
			dvMemberRank = sqlStatetment.SelectSingleStatementWithOC(sqlAccessor);
		}
		ddlDisplayMemberRank.Items.Add("");
		if (dvMemberRank.Count > 0)
		{
			dvMemberRank.Sort = Constants.FIELD_MEMBERRANK_MEMBER_RANK_ORDER;
			foreach (DataRowView drv in dvMemberRank)
			{
				ddlDisplayMemberRank.Items.Add(new ListItem((string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME], (string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID]));
			}
		}
	}


	/// <summary>
	/// 商品カテゴリ情報一覧表示(DataGridにDataView(商品カテゴリ情報)を設定)
	/// </summary>
	private void ViewProductCategoryList()
	{
		// 変数宣言
		int iCurrentPageNumber = 1;

		// ページ番号（ページャ動作時のみもちまわる）
		try
		{
			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) == "")
			{
			}
			else
			{
				iCurrentPageNumber = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			// 不正ページを指定された場合
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 検索コントロール制御（商品一覧共通処理）
		//------------------------------------------------------
		SetSearchInfo();

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		string strSearchKey = StringUtility.ToEmpty(ddlSearchKey.SelectedValue);
		string strSearchWord = StringUtility.ToEmpty(tbSearchWord.Text);

		//------------------------------------------------------
		// 商品カテゴリ一覧
		//------------------------------------------------------
		int iTotalProductCategoryCounts = 0;	// ページング可能総商品数
		// 商品カテゴリデータ取得
		DataView dvCategory = GetProductCategoryListDataView(
			iCurrentPageNumber, strSearchKey, strSearchWord);
		if (dvCategory.Count != 0)
		{
			iTotalProductCategoryCounts = int.Parse(dvCategory[0].Row["row_count"].ToString());
			// エラー非表示制御
			trListError.Visible = false;
		}
		else
		{
			iTotalProductCategoryCounts = 0;
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

		}
		// データソースセット
		rList.DataSource = dvCategory;

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string strNextUrl = CreateProductCategoryRegisterUrl(strSearchKey, strSearchWord);
		lbPager1.Text = WebPager.CreateDefaultListPager(iTotalProductCategoryCounts, iCurrentPageNumber, strNextUrl);

		//------------------------------------------------------
		// カレントページをビューステート格納
		//------------------------------------------------------
		ViewState.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber.ToString());

		this.ProductCategoryIdListOfDisplayedData = dvCategory.Cast<DataRowView>()
			.Select(drv => (string)drv[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]).ToArray();
	}


	/// <summary>
	/// 商品一覧データビューを表示分だけ取得
	/// </summary>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <param name="strParentCategoryId">親カテゴリID</param>
	/// <param name="strProductCategoryId">カテゴリID</param>
	/// <returns>商品一覧データビュー</returns>
	private DataView GetProductCategoryListDataView(int iPageNumber, string strParentCategoryId, string strProductCategoryId)
	{
		// 検索情報取得
		Hashtable htInput = GetSearchInfo(strParentCategoryId, strProductCategoryId);
		htInput.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNumber - 1) + 1);		// 表示開始記事番号
		htInput.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNumber);				// 表示開始記事番号

		// ステートメントからカテゴリデータ取得
		DataView dvResult = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductCategory", "GetProductCategoryList"))
		{
			dvResult = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput).Tables["Table"].DefaultView;
		}

		return dvResult;
	}

	/// <summary>
	/// 検索情報取得
	/// </summary>
	/// <param name="strParentCategoryId">親カテゴリID</param>
	/// <param name="strProductCategoryId">カテゴリID</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfo(string strParentCategoryId, string strProductCategoryId)
	{
		string strSearchKey = String.Empty;
		string strInputCategoryId = String.Empty;

		//------------------------------------------------------
		// 初期化
		// strSearchKey:[0]親カテゴリ検索、[1]カテゴリID検索、[2]親カテゴリ、カテゴリID検索、[99]条件無し
		//------------------------------------------------------
		strSearchKey = "99";
		// 親カテゴリ && カテゴリIDが存在する場合
		if (strParentCategoryId != "" && strProductCategoryId != "")
		{
			// 親カテゴリ以外？
			if (strParentCategoryId != strProductCategoryId)
			{
				strSearchKey = "2";
			}
			else
			{
				strSearchKey = "0";
				strProductCategoryId = strParentCategoryId;
			}
		}
		// 第一階層商品カテゴリが存在する場合
		else if (strParentCategoryId != "")
		{
			strSearchKey = "0";
			strProductCategoryId = strParentCategoryId;
		}
		// カテゴリIDが存在する場合
		else if (strProductCategoryId != "")
		{
			strSearchKey = "1";
		}

		// 検索情報取得
		Hashtable htParam = new Hashtable();
		htParam.Add("srch_key", strSearchKey);													// 検索フィールド
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID, strParentCategoryId);	// 親カテゴリID
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strProductCategoryId));			// カテゴリID
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, this.LoginOperatorShopId);		// 店舗ID
		htParam.Add("root_category_sort_kbn", Constants.ROOT_CATEGORY_SORT_KBN);

		return htParam;
	}

	/// <summary>
	/// 商品詳細情報取得
	/// </summary>
	/// <param name="strProductCategoryId">商品カテゴリID</param>
	private void ViewProductCategoryDetail(string strProductCategoryId)
	{
		DataRow dr = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatements = new SqlStatement("ProductCategory", "GetProductCategory"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);
			htInput.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID, strProductCategoryId);

			DataSet ds = sqlStatements.SelectStatementWithOC(sqlAccessor, htInput);
			// 該当データが有りの場合
			if (ds.Tables["Table"].DefaultView.Count != 0)
			{
				dr = ds.Tables["Table"].Rows[0];
			}
			// 該当データ無しの場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);

			}
		}

		// Hashtabe格納
		foreach (DataColumn dc in dr.Table.Columns)
		{
			m_htParam.Add(dc.ColumnName, dr[dc.ColumnName]);
		}

		// 商品カテゴリ情報をビューステートに格納
		ViewState.Add(Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO, m_htParam);

		// 翻訳情報
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var searchCondition = new NameTranslationSettingSearchCondition()
			{
				DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY,
				MasterId1 = strProductCategoryId,
				MasterId2 = string.Empty,
				MasterId3 = string.Empty,
			};
			this.CategoryTranslationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
		}
	}

	/// <summary>
	/// 子カテゴリ有無取得
	/// </summary>
	/// <param name="strProductCategoryId">カテゴリID</param>
	/// <returns>子カテゴリ有無　有：true 無：false</returns>
	private bool HasChildProductCategory(string strProductCategoryId)
	{
		// 変数宣言
		bool blResult = false;

		// ステートメントからカテゴリデータ取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductCategory", "GetChildProductCategory"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID, strProductCategoryId);		// カテゴリID
			htInput.Add(Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, this.LoginOperatorShopId);	// 店舗ID

			// SQL発行
			DataSet ds = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput);

			// 子カテゴリが存在する場合
			if (ds.Tables["Table"].DefaultView.Count > 0)
			{
				blResult = true;
			}
		}
		return blResult;
	}

	/// <summary>
	/// 検索コントロール制御
	/// </summary>
	/// <remarks>
	/// Request内容を検索コントロールに設定
	/// </remarks>
	private void SetSearchInfo()
	{
		try
		{
			ddlSearchKey.SelectedValue = (string)Request[Constants.REQUEST_KEY_SEARCH_KEY];
		}
		catch
		{
			// 不正カテゴリIDを指定された場合
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		tbSearchWord.Text = (string)Request[Constants.REQUEST_KEY_SEARCH_WORD];
	}

	/// <summary>
	/// 商品カテゴリ遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <returns>商品カテゴリ遷移URL作成</returns>
	private string CreateProductCategoryRegisterUrl(string strSearchKey, string strSearchWord)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTCATEGORY_REGISTER;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_SEARCH_KEY + "=" + strSearchKey;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_SEARCH_WORD + "=" + HttpUtility.UrlEncode(strSearchWord);

		return strResult;
	}

	/// <summary>
	/// 商品カテゴリ遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// 1<param name="iPageNumber">ページ番号</param>
	/// <returns>商品カテゴリ遷移URL作成</returns>
	private string CreateProductCategoryRegisterUrl(string strSearchKey, string strSearchWord, int iPageNumber)
	{
		string strResult = CreateProductCategoryRegisterUrl(strSearchKey, strSearchWord);
		strResult += "&";
		strResult += Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();

		return strResult;
	}

	/// <summary>
	/// データバインド用商品カテゴリ詳細URL作成
	/// </summary>
	/// <param name="strProductCategoryId">商品カテゴリID</param>
	/// <returns>商品カテゴリ詳細URL作成</returns>
	protected string CreateProductCategoryDetailUrl(string strProductCategoryId)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTCATEGORY_REGISTER;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_SEARCH_KEY + "=" + ddlSearchKey.SelectedValue;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_SEARCH_WORD + "=" + HttpUtility.UrlEncode(tbSearchWord.Text.Trim());
		strResult += "&";
		strResult += Constants.REQUEST_KEY_PRODUCTCATEGORY_ID + "=" + strProductCategoryId;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_PAGE_NO + "=" + (string)ViewState[Constants.REQUEST_KEY_PAGE_NO];

		return strResult;
	}

	/// <summary>
	/// 検索イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// 選択カテゴリIDクリア
		Session[Constants.SESSION_KEY_PRODUCTCATEGORY_CURRENT_CATEGORY_ID] = null;

		// 一覧画面へ遷移
		Response.Redirect(
			CreateProductCategoryRegisterUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim()));
	}

	/// <summary>
	/// 編集するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCategoryEdit_Click(object sender, System.EventArgs e)
	{
		// 選択商品カテゴリ情報取得
		Hashtable htParam = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO];

		// 選択されていない場合
		if (htParam == null)
		{
			//エラーページへ遷移
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTCATEGORY_NO_SELECTED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 選択商品カテゴリ情報をパラメタに格納
		Session[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO] = htParam;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ遷移
		Response.Redirect(
			CreateProductCategoryRegisterUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim()) +
			"&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE +
			"&" + Constants.REQUEST_KEY_PAGE_NO + "=" + (string)ViewState[Constants.REQUEST_KEY_PAGE_NO]);
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, System.EventArgs e)
	{
		switch ((string)Session[Constants.SESSION_KEY_ACTION_STATUS])
		{
			//------------------------------------------------------
			// 登録・編集画面表示時、前ページへ遷移する
			//------------------------------------------------------
			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_PARENT_INSERT:
			case Constants.ACTION_STATUS_CHILD_INSERT:
				if ((string)Session[Constants.SESSION_KEY_PRODUCTCATEGORY_CURRENT_CATEGORY_ID] == null)
				{
					// 商品カテゴリページTOPへリダイレクト（初期表示）
					Response.Redirect(CreateProductCategoryRegisterUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim()));
				}
				else
				{
					// 詳細画面へリダイレクト
					Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_DETAIL;
					Response.Redirect(CreateProductCategoryDetailUrl((string)Session[Constants.SESSION_KEY_PRODUCTCATEGORY_CURRENT_CATEGORY_ID]));
				}
				break;

			//------------------------------------------------------
			// 確認画面表示時、前ページへ遷移する
			//------------------------------------------------------
			case Constants.ACTION_STATUS_CONFIRM:
				// 値調整
				Hashtable htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO];
				htParam[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID] = htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_parent"];
				htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_input"] = htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_child"];
				htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID] = htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_parent"];
				htParam["back_flg"] = 1;	// 「戻る」ボタン使用フラグ

				// セッション情報格納
				Session[Constants.SESSION_KEY_ACTION_STATUS] = (string)htParam[Constants.REQUEST_KEY_ACTION_STATUS];
				Session[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO] = htParam;

				// 編集画面へリダイレクト
				StringBuilder sbUrl = new StringBuilder();
				sbUrl.Append(CreateProductCategoryRegisterUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim()));
				sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append((string)htParam[Constants.REQUEST_KEY_ACTION_STATUS]);
				sbUrl.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append((string)ViewState[Constants.REQUEST_KEY_PAGE_NO]);
				Response.Redirect(sbUrl.ToString());
				break;
		}
	}

	/// <summary>
	/// 最上位カテゴリの登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCategoryRootInsert_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_PARENT_INSERT;

		// 編集画面へ遷移
		Response.Redirect(
			CreateProductCategoryRegisterUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim()) +
			"&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_PARENT_INSERT +
			"&" + Constants.REQUEST_KEY_PAGE_NO + "=" + (string)ViewState[Constants.REQUEST_KEY_PAGE_NO]);
	}

	/// <summary>
	/// 子カテゴリの登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCategoryChildInsert_Click(object sender, System.EventArgs e)
	{
		// 選択商品カテゴリ情報取得
		Hashtable htParam = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO];

		// 選択されていない場合
		if (htParam == null)
		{
			//エラーページへ遷移
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTCATEGORY_NO_SELECTED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 11階層目以降を作ろうとした場合
		if (((string)htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]).Length
			>= Constants.CONST_CATEGORY_ID_LENGTH * Constants.CONST_CATEGORY_DEPTH)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTCATEGORY_DEPTH_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 選択情報をパラメタに格納
		Session[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO] = htParam;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_CHILD_INSERT;

		// 編集画面へ遷移
		Response.Redirect(
			CreateProductCategoryRegisterUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim()) +
			"&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_CHILD_INSERT +
			"&" + Constants.REQUEST_KEY_PAGE_NO + "=" + (string)ViewState[Constants.REQUEST_KEY_PAGE_NO]);
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCategoryConfirm_Click(object sender, System.EventArgs e)
	{
		// 変数宣言
		string strValidator = String.Empty;

		// 表示順
		if (string.IsNullOrEmpty(tbDisplayOrder.Text))
		{
			tbDisplayOrder.Text = "1";
		}

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		Hashtable htParam = new Hashtable();

		htParam.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);					// 店舗ID
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_length"
			, tbCategoryId.Text);																	// 商品カテゴリID(Lengthのみチェック)
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID,
			tbParentCategoryId.Text + tbCategoryId.Text);											// 商品カテゴリID(親商品カテゴリも含む)
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_old",
			ViewState[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_old"]);						// 商品カテゴリID(旧)
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID,
			tbParentCategoryId.Text);																// 親商品カテゴリID
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_NAME, tbName.Text);								// カテゴリ名
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_NAME_KANA, tbNameKana.Text);											// カテゴリ名 (フリガナ)
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_DISPLAY_ORDER, tbDisplayOrder.Text);									// 表示順
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN, ddlChildCategorySortKbn.SelectedValue);		// 子カテゴリの並び順
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_SEO_KEYWORDS, tbSeoKeywords.Text);				// SEOキーワード
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_CANONICAL_TEXT, tbCanonicalText.Text);			// カノニカルタグ用テキスト
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_URL, tbUrl.Text);								// カテゴリページURL
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG, cbUseRecommendFlg.Checked ?
			Constants.FLG_PRODUCTCATEGORY_USE_RECOMMEND_FLG_VALID : Constants.FLG_PRODUCTCATEGORY_USE_RECOMMEND_FLG_INVALID);	// 外部レコメンド利用フラグ
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_VALID_FLG, cbValidFlg.Checked ?
			Constants.FLG_PRODUCTCATEGORY_VALID_FLG_VALID : Constants.FLG_PRODUCTCATEGORY_VALID_FLG_INVALID);		// 有効フラグ
		htParam.Add(Constants.REQUEST_KEY_ACTION_STATUS,
			this.ActionStatus);								// 処理方法格納
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_LAST_CHANGED,
			this.LoginOperatorName);							// 最終更新者

		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_parent", tbParentCategoryId.Text); // 親カテゴリID保持（「戻る」ボタン使用時）
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_child", tbCategoryId.Text); //子カテゴリID保持（「戻る」ボタン使用時）

		//------------------------------------------------------
		// ブランド情報格納
		//------------------------------------------------------
		StringBuilder sbBrandIds = new StringBuilder();

		// 全てを選択していないなら
		if (cbPermitBrand.Checked == false)
		{
			// 選択されたIDをカンマ区切りで連結
			foreach (ListItem li in cblPermitBrandIds.Items)
			{
				if (li.Selected)
				{
					if (sbBrandIds.ToString() != "")
					{
						sbBrandIds.Append(",");
					}
					sbBrandIds.Append(li.Value);
				}
			}
		}
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS, sbBrandIds.ToString());

		//------------------------------------------------------
		// 会員ランク表示制御情報格納
		//------------------------------------------------------
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID, ddlDisplayMemberRank.SelectedValue);
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG, cbLowerMemberCanDisplayTreeFlg.Checked ?
			Constants.FLG_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG_VALID : Constants.FLG_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG_INVALID);

		// 定期会員限定フラグ
		htParam.Add(Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG, cbOnlyFixedPurchaseMemberFlg.Checked ?
			Constants.FLG_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG_VALID : Constants.FLG_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG_INVALID);

		// 登録？
		if (this.ActionStatus == Constants.ACTION_STATUS_CHILD_INSERT ||
			this.ActionStatus == Constants.ACTION_STATUS_PARENT_INSERT)
		{
			strValidator = "ProductCategoryInsert";
		}
		// 更新？
		else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			strValidator = "ProductCategoryUpdate";
		}

		// パラメタをセッションに格納
		Session[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO] = htParam;

		// 入力チェック＆重複チェック
		string strErrorMessages = Validator.Validate(strValidator, htParam);
		if (strErrorMessages != "")
		{
			htParam["back_flg"] = 1;	// 「戻る」ボタン使用フラグ
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_CONFIRM;

		// 確認画面へ遷移
		Response.Redirect(
			CreateProductCategoryRegisterUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim()) +
			"&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_CONFIRM +
			"&" + Constants.REQUEST_KEY_PAGE_NO + "=" + (string)ViewState[Constants.REQUEST_KEY_PAGE_NO]);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCategoryRegister_Click(object sender, System.EventArgs e)
	{
		// 変数宣言
		var validator = string.Empty;
		var statementKey = string.Empty;

		// 選択カテゴリIDクリア
		Session[Constants.SESSION_KEY_PRODUCTCATEGORY_CURRENT_CATEGORY_ID] = null;

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		var htInput = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO];

		// 登録？
		if ((string)htInput[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_CHILD_INSERT
			|| (string)htInput[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_PARENT_INSERT)
		{
			validator = "ProductCategoryInsert";
			statementKey = "InsertProductCategory";
		}
		// 更新？
		else if ((string)htInput[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			validator = "ProductCategoryUpdate";
			statementKey = "UpdateProductCategory";
		}

		// 入力チェック＆重複チェック
		var strErrorMessages = Validator.Validate(validator, htInput);
		if (strErrorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 商品カテゴリ登録・変更
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			// 商品カテゴリ登録・変更
			using (var statement = new SqlStatement("ProductCategory", statementKey))
			{
				statement.ExecStatement(accessor, htInput);
			}

			// カテゴリ更新の場合、他のテーブルも整合性を保つ
			if (statementKey == "UpdateProductCategory")
			{
				using (var rankingStatement = new SqlStatement("ProductRanking", "UpdateCategoryIds"))
				{
					rankingStatement.ExecStatement(accessor, htInput);
				}

				using (var productStatement = new SqlStatement("Product", "UpdateProductCategoryIds"))
				{
					productStatement.ExecStatement(accessor, htInput);
				}
			}

			accessor.CommitTransaction();
		}

		// セッションに登録・変更完了メッセージ格納
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTCATEGORY_REGIST_UPDATE_SUCCESS);

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;

		// 検索情報作成
		if (string.IsNullOrEmpty((string)htInput[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID]))
		{
			// 商品カテゴリが選択された場合
			if (ddlSearchKey.SelectedIndex != 0)
			{
				var item = new ListItem(
					(string)htInput[Constants.FIELD_PRODUCTCATEGORY_NAME],
					(string)htInput[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]);

				var pos = ddlSearchKey.SelectedIndex;
				ddlSearchKey.Items.RemoveAt(pos);
				ddlSearchKey.Items.Insert(pos, item);
				ddlSearchKey.SelectedIndex = pos;
			}

			// カテゴリIDが入力された場合
			if (string.IsNullOrEmpty(tbSearchWord.Text.Trim()) == false)
			{
				tbSearchWord.Text = (string)htInput[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID];
			}
		}

		// 商品カテゴリ初期表示
		Response.Redirect(
			CreateProductCategoryRegisterUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim()) +
			"&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COMPLETE +
			"&" + Constants.REQUEST_KEY_PRODUCTCATEGORY_ID + "=" + (string)htInput[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID] +
			"&" + Constants.REQUEST_KEY_PAGE_NO + "=" + (string)ViewState[Constants.REQUEST_KEY_PAGE_NO]);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>検索処理を行う</remarks>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 変数宣言
		SqlAccessor sqlAccessor = null;
		SqlStatement SqlStatement = null;

		// 選択カテゴリIDクリア
		Session[Constants.SESSION_KEY_PRODUCTCATEGORY_CURRENT_CATEGORY_ID] = null;

		// 選択商品カテゴリ情報取得
		Hashtable htParam = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO];

		// 選択されていない場合
		if (htParam == null)
		{
			//エラーページへ遷移
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTCATEGORY_NO_SELECTED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 商品カテゴリ削除
		using (sqlAccessor = new SqlAccessor())
		using (SqlStatement = new SqlStatement("ProductCategory", "DeleteProductCategory"))
		{
			// 商品カテゴリ削除
			int iUpdated = SqlStatement.ExecStatementWithOC(sqlAccessor, htParam);
		}

		// 商品カテゴリ初期表示
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTCATEGORY_REGISTER);
	}

	/// <summary>
	/// 商品一覧ページURL出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportProductListUrl_Click(object sender, EventArgs e)
	{
		uMasterDownload.ExportProductListUrl(GetSearchInfo(StringUtility.ToEmpty(
			ddlSearchKey.SelectedValue),
			StringUtility.ToEmpty(tbSearchWord.Text.Trim())));
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return GetSearchInfo(StringUtility.ToEmpty(
			(string)Request[Constants.REQUEST_KEY_SEARCH_KEY]),
			StringUtility.ToEmpty((string)Request[Constants.REQUEST_KEY_SEARCH_WORD]));
	}

	/// <summary>
	/// 権限制御（CSVダウンロード）
	/// </summary>
	private void SetExportAuthority()
	{
		lbExportProductListUrl.Visible = MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCTCAT_PRODUCTLIST_URL_DL);
	}

	/// <summary>
	/// 「戻る」ボタンからの遷移を判断
	/// </summary>
	/// <param name="htParam">カテゴリ情報</param>
	private void CheckFromBackbutton(Hashtable htParam)
	{
		if (htParam is Hashtable)
		{
			if (htParam.Contains("back_flg"))
			{
				htParam.Remove("back_flg");
				m_htParam = htParam;
				m_htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_input"] = htParam[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_child"];
			}
		}
	}

	/// <summary>
	/// 外部レコメンド連携時のアイテムログ作成
	/// </summary>
	/// <param name="strMasterId">マスタID</param>
	private void RecommendItemCoop(string strMasterId)
	{
		// 外部レコメンド連携用の商品ログを作成
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("RecommendCoopUpdateLog", "InsertRecommendItemLog"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_RECOMMENDCOOPUPDATELOG_SHOP_ID, this.LoginOperatorShopId);
			htInput.Add(Constants.FIELD_RECOMMENDCOOPUPDATELOG_MASTER_KBN, Constants.FLG_RECOMMENDCOOPUPDATELOG_MASTER_KBN_PRODUCTCATEGORY);	// 商品カテゴリマスタ
			htInput.Add(Constants.FIELD_RECOMMENDCOOPUPDATELOG_MASTER_ID, strMasterId);															// マスタID(商品カテゴリID)
			htInput.Add(Constants.FIELD_RECOMMENDCOOPUPDATELOG_LAST_CHANGED, this.LoginOperatorName);											// 最終更新者

			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}
	}

	/// <summary>
	/// 会員ランク名取得処理
	/// </summary>
	/// <param name="strMemberRankId">会員ランクID</param>
	/// <returns>会員ランク名</returns>
	/// <remarks>画面内ドロップダウンから取得（非表示でも可）</remarks>
	protected string GetMemberRankName(string strMemberRankId)
	{
		var result = ReplaceTag("@@DispText.common_message.unspecified@@");
		if (strMemberRankId != "")
		{
			foreach (ListItem li in ddlDisplayMemberRank.Items)
			{
				if (li.Value == strMemberRankId)
				{
					result = li.Text;
					break;
				}
			}
		}
		return result;
	}

	/// <summary>
	/// 翻訳データ出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = this.ProductCategoryIdListOfDisplayedData;
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}

	/// <summary>商品カテゴリ翻訳設定情報</summary>
	protected NameTranslationSettingModel[] CategoryTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["category_translation_data"]; }
		set { ViewState["category_translation_data"] = value; }
	}
	/// <summary>画面表示されているカテゴリIDリスト</summary>
	private string[] ProductCategoryIdListOfDisplayedData
	{
		get { return (string[])ViewState["productcategoryid_list_of_displayed_data"]; }
		set { ViewState["productcategoryid_list_of_displayed_data"] = value; }
	}
	/// <summary>親カテゴリが存在するか</summary>
	protected bool IsParentCategory
	{
		get { return (string.IsNullOrEmpty((string)m_htParam[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID]) == false); }
	}
}
