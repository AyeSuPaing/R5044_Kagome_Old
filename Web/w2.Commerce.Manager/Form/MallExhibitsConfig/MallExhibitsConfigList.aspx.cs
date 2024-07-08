/*
=========================================================================================================
  Module      : モール出品設定リストページ(MallExhibitsConfigList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using w2.App.Common.Option;

public partial class Form_MallExhibitsConfig_MallExhibitsConfigList : ProductPage
{
	// 定数
	protected const string CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG = "exhibits_flg"; // 出品フラグ
	protected const string CONST_SEACH_KEY_CTEGORY_FIELD_NAME = "search_key"; // 検索項目
	protected const string CONST_SEACH_KEY_CTEGORY_FLG = "3"; // カテゴリID

	//=========================================================================================
	// データバインド用変数
	//=========================================================================================
	protected Hashtable m_htMallExhibitsConfigName = new Hashtable();		// モール出品名称見出し用
	protected Hashtable m_htMallExhibitsConfigColumn = new Hashtable();		// モール出品カラム名

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//初期表示
			InitializeComponents();

			// モール出品設定一覧情報表示
			ViewMallExhibitsConfigList();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 検索項目
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLEXHIBITSCONFIG, CONST_SEACH_KEY_CTEGORY_FIELD_NAME))
		{
			if((Constants.PRODUCT_CTEGORY_OPTION_ENABLE == false) && li.Value == CONST_SEACH_KEY_CTEGORY_FLG)
				continue;

			ddlSearchKey.Items.Add(li);
		}
	}

	/// <summary>
	/// モール出品設定一覧情報表示
	/// </summary>
	private void ViewMallExhibitsConfigList()
	{
		//------------------------------------------------------
		// リクエスト情報取得（チェックのみ利用）
		//------------------------------------------------------
		Hashtable htParam = GetParameters(Request);

		//------------------------------------------------------
		// 検索コントロール制御（商品在庫一覧共通処理）
		//------------------------------------------------------
		// ページ番号
		int iCurrentPageNumber = 0;
		if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out iCurrentPageNumber) == false)
		{
			iCurrentPageNumber = 1;
		}
		ViewState[Constants.REQUEST_KEY_PAGE_NO] = iCurrentPageNumber;

		// 文字検索項目
		string strSearchKey = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SEARCH_KEY]);
		foreach (ListItem li in ddlSearchKey.Items)
		{
			li.Selected = (li.Value == strSearchKey);
		}

		string strSearchWord = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SEARCH_WORD]).Trim();
		tbSearchWord.Text = strSearchWord;

		// ソート項目
		string strSortKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]);
		foreach (ListItem li in ddlSortKbn.Items)
		{
			li.Selected = (li.Value == strSortKbn);
		}

		//------------------------------------------------------
		// モール出品設定見出し項目設定
		//------------------------------------------------------
		DataView dvMallCooperationSettings = MallPage.GetMallCooperationSettingAll(this.LoginOperatorShopId);

		int iExhibitsCount = 1;
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG))
		{
			foreach (DataRowView drv in dvMallCooperationSettings)
			{
				if (drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG].ToString() == li.Value)
				{
					m_htMallExhibitsConfigName.Add(CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG + iExhibitsCount.ToString(), (string)drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]);
					m_htMallExhibitsConfigColumn.Add(CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG + iExhibitsCount.ToString(), string.Format("({0})", MallOptionUtility.GetExhibitsConfigField((string)drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG])));
					break;
				}
			}
			iExhibitsCount++;
		}

		//------------------------------------------------------
		// SQL検索パラメータ取得
		//------------------------------------------------------
		Hashtable htSqlParam = new Hashtable();
		htSqlParam.Add("srch_key", strSearchWord != "" ? strSearchKey : "99");			// 検索フィールド
		htSqlParam.Add("srch_word" + "_like_escaped", 
			StringUtility.SqlLikeStringSharpEscape(strSearchWord));						// 検索値
		htSqlParam.Add("sort_kbn", strSortKbn);											// ソート区分
		htSqlParam.Add("bgn_row_num",
			Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1);	// 表示開始記事番号
		htSqlParam.Add("end_row_num",
			Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber);			// 表示開始記事番号

		//------------------------------------------------------
		// モール出品設定一覧（該当件数）
		//------------------------------------------------------
		int iTotalExhibitsCounts = 0;	// ページング可能総商品数
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallExhibitsConfig", "GetMallExhibitsConfigCount"))
		{
			DataView dvExhibitsCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
			if (dvExhibitsCount.Count != 0)
			{
				iTotalExhibitsCounts = (int)dvExhibitsCount[0]["row_count"];
			}
		}

		//------------------------------------------------------
		// エラー表示制御
		//------------------------------------------------------
		bool blDisplayPager = true;
		StringBuilder sbErrorMessage = new StringBuilder();
		// 上限件数より多い？
		if (iTotalExhibitsCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
		{
			sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
			sbErrorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(iTotalExhibitsCounts));
			sbErrorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

			blDisplayPager = false;
		}
		// 該当件数なし？
		else if (iTotalExhibitsCounts == 0)
		{
			sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
		}
		tdListErrorMessage.InnerHtml = sbErrorMessage.ToString();
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19) ? 1 : 0;
		tdListErrorMessage.ColSpan += m_htMallExhibitsConfigName.Contains(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20) ? 1 : 0;
		trListError.Visible = (sbErrorMessage.ToString().Length != 0);

		//------------------------------------------------------
		// モール出品設定一覧情報表示
		//------------------------------------------------------
		if (trListError.Visible == false)
		{
			// モール出品設定一覧情報取得
			DataView dvExhibits = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallExhibitsConfig", "GetMallExhibitsConfigList"))
			{
				dvExhibits = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
			}

			// データソース設定
			rList.DataSource = dvExhibits;
		}

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		if (blDisplayPager)
		{
			string strNextUrl = CreateMallExhibitsConfigListUrl(strSearchKey, strSearchWord, strSortKbn);
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalExhibitsCounts, iCurrentPageNumber, strNextUrl);
		}

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
	}

	/// <summary>
	/// モール出品設定一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">モール出品設定一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	private Hashtable GetParameters(HttpRequest hrRequest)
	{
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
				case Constants.KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_PRODUCT_ID:						// 商品ID
				case Constants.KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_NAME:							// 商品名
				case Constants.KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_NAME_KANA:						// 商品名(フリガナ)
				case Constants.KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_CATEGORY_ID:						// カテゴリID
					strSearchKey = hrRequest[Constants.REQUEST_KEY_SEARCH_KEY].ToString();
					break;
				case "":
					strSearchKey = Constants.KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_DEFAULT;			// 商品IDがデフォルト
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
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_PRODUCT_ID_ASC:				// 商品ID/昇順
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_PRODUCT_ID_DESC:				// 商品ID/降順
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_NAME_ASC:						// 商品名/昇順
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_NAME_DESC:					// 商品名/降順
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_NAME_KANA_ASC:				// 商品フリガナ/昇順
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_NAME_KANA_DESC:				// 商品フリガナ/降順
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_DATE_CREATED_ASC:				// 作成日/昇順
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_DATE_CREATED_DESC:			// 作成日/降順
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_DATE_CHANGED_ASC:				// 更新日/昇順
				case Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_DATE_CHANGED_DESC:			// 更新日/降順
					strSortKbn = hrRequest[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				case "":
					strSortKbn = Constants.KBN_SORT_MALLEXHIBITSCONFIG_LIST_DEFAULT;			// 商品ID/昇順がデフォルト
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

		return htResult;
	}

	/// <summary>
	/// モール出品設定一覧データビューを表示分だけ取得
	/// </summary>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <returns>商品在庫一覧データビュー</returns>
	private DataView GetMallExhibitsListDataView(
		int iPageNumber,
		string strSearchKey,
		string strSearchWord,
		string strSortKbn)
	{
		// 変数宣言
		DataView dvResult = null;

		// 検索値が存在しない場合
		if (strSearchWord == "")
		{
			strSearchKey = "99";	// 未条件
		}

		// ステートメントからカテゴリデータ取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallExhibitsConfig", "GetMallExhibitsConfigList"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add("srch_key", strSearchKey);							// 検索フィールド
			htInput.Add("srch_word", strSearchWord);						// 検索値
			htInput.Add("sort_kbn", strSortKbn);							// ソート区分
			htInput.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNumber - 1) + 1);								// 表示開始記事番号
			htInput.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNumber);								// 表示開始記事番号

			// SQL発行
			DataSet ds = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput);
			dvResult = ds.Tables["Table"].DefaultView;

			// 商品在庫情報をビューステートに保存
			ViewState.Add(Constants.TABLE_MALLEXHIBITSCONFIG, ds);
		}

		return dvResult;
	}

	/// <summary>
	/// モール出品設定一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <returns>モール出品設定一覧遷移URL</returns>
	private string CreateMallExhibitsConfigListUrl(
		string strSearchKey,
		string strSearchWord,
		string strSortKbn)
	{
		StringBuilder sbResult = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_MALL_EXHIBITS_CONFIG_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_SEARCH_KEY).Append("=").Append(strSearchKey);
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SEARCH_WORD).Append("=").Append(HttpUtility.UrlEncode(strSearchWord));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(strSortKbn);

		return sbResult.ToString();
	}

	/// <summary>
	/// モール出品設定一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <param name="iPageNumber">ページ番号</param>
	/// <returns>モール出品設定一覧遷移URL</returns>
	private string CreateMallExhibitsConfigListUrl(
		string strSearchKey,
		string strSearchWord,
		string strSortKbn,
		int iPageNumber)
	{
		StringBuilder sbResult = new StringBuilder(CreateMallExhibitsConfigListUrl(strSearchKey, strSearchWord, strSortKbn));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(iPageNumber.ToString());

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
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		string strUrl = CreateMallExhibitsConfigListUrl(
				ddlSearchKey.SelectedValue,
				tbSearchWord.Text.Trim(),
				ddlSortKbn.SelectedValue,
				1);

		// モール出品設定一覧(参照)へ
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
	/// 更新用全件CSVダウンロードボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnExport_Click(object sender, EventArgs e)
	{
		// 検索情報取得
		Hashtable htParam = new Hashtable();
		htParam.Add("srch_key", tbSearchWord.Text.Trim() != "" ? ddlSearchKey.SelectedValue : "99");
		htParam.Add("srch_word", tbSearchWord.Text);
		htParam.Add("sort_kbn", ddlSortKbn.SelectedValue);
		htParam.Add(Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID, 0);// 共通化処理によりダミー情報作成

		// 店舗ID設定
		htParam.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);

		// マスタ区分設定(複数待つ場合は設定IDも設定する★)
		htParam[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN] = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_MALLEXHIBITSCONFIG;

		Session[Constants.SESSION_KEY_PARAM] = htParam;

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTEREXPORT);
	}

	/// <summary>
	/// 更新用全件CSVアップロードボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnImport_Click(object sender, System.EventArgs e)
	{
		IImportExhibits objImportExhibits = null;
		bool blResult = true;
		StringBuilder sbErrorMessage = new StringBuilder();

		// ファイル指定チェック
		if (blResult && fFile.Value == "")
		{
			blResult = false;
			sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_FILE_UNSELECTED));
		}

		// ファイル存在チェック
		if (blResult && fFile.PostedFile.InputStream.Length == 0)
		{
			blResult = false;
			sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_FILE_UNFIND));
		}

		// 上記チェックでエラーではない場合
		if (blResult)
		{
			//------------------------------------------------------
			// 履歴保存（取込ファイルをlogフォルダ配下にコピーする）
			//------------------------------------------------------
			// ※下記処理は、StreamReaderによってfFile.PostedFile.InputStreamのデータが消える前に行う必要がある

			// ディレクトリがなければ作成
			if (Directory.Exists(Constants.PHYSICALDIRPATH_LOGFILE + @"MallExhibitsConfig\") == false)
			{
				Directory.CreateDirectory(Constants.PHYSICALDIRPATH_LOGFILE + @"MallExhibitsConfig\");
			}

			string[] strFileNames = fFile.Value.Split('\\');
			StringBuilder sbFilePath = new StringBuilder();
			sbFilePath.Append(Constants.PHYSICALDIRPATH_LOGFILE).Append(@"MallExhibitsConfig\");
			sbFilePath.Append(DateTime.Now.ToString("yyyyMMddhhmmss")).Append("_").Append(strFileNames[strFileNames.Length - 1]);
			string strFilePath = sbFilePath.ToString();

			try
			{
				// ファイルアップロード実行
				fFile.PostedFile.SaveAs(strFilePath);
			}
			catch (UnauthorizedAccessException ex)
			{
				// ファイルアップロード権限エラー（ログにも記録）
				AppLogger.WriteError(ex.ToString());
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTERUPLOAD_UPLOAD_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			//------------------------------------------------------
			// CSVファイルの取込実行
			//------------------------------------------------------
			try
			{
				// Zip圧縮
				w2.Common.Util.Archiver.ZipArchiver zaZipArchiver = new w2.Common.Util.Archiver.ZipArchiver();
				zaZipArchiver.CompressFile(
					strFilePath,
					Path.GetDirectoryName(strFilePath),
					Path.GetDirectoryName(strFilePath) + @"\" + Path.GetFileNameWithoutExtension(strFilePath) + ".zip");

				// 取込ファイル削除
				File.Delete(strFilePath);
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn("モール出品設定アップロードファイルのZip圧縮に失敗しました。", ex);
			}

			//------------------------------------------------------
			// CSVファイルの取込実行
			//------------------------------------------------------
			objImportExhibits = new ImportMallExhibitsConfig();

			// モール出品設定ファイル取込
			using (StreamReader sr = new StreamReader(fFile.PostedFile.InputStream, Encoding.GetEncoding("Shift_JIS")))
			{
				blResult = objImportExhibits.ImportExhibits(sr, this.LoginOperatorShopId, this.LoginOperatorName);
			}

			// エラーであればメッセージを連結
			if (blResult == false)
			{
				sbErrorMessage.Append(objImportExhibits.ErrorMessage);
			}
		}

		// 処理成功
		if (blResult)
		{
			lbResultMessage.ForeColor = Color.Empty;
			lbResultMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUCCESSFUL_CAPTURE)
				.Replace("@@ 1 @@", StringUtility.ToNumeric(objImportExhibits.UpdatedCount))
				.Replace("@@ 2 @@", StringUtility.ToNumeric(objImportExhibits.LinesCount));

			//------------------------------------------------------
			// モール出品設定一覧情報表示
			//------------------------------------------------------
			ViewMallExhibitsConfigList();
		}
		// 処理失敗
		else
		{
			// エラーメッセージ出力
			lbResultMessage.ForeColor = Color.Red;
			lbResultMessage.Text = sbErrorMessage.ToString();
		}
	}
}

