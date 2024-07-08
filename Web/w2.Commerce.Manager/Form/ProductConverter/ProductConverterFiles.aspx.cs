/*
=========================================================================================================
  Module      : 商品コンバータ 結果ファイル一覧ページ処理(ProductConverterFiles.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Text;

public partial class Form_ProductConverter_ProductConverterFiles : ProductConverterPage
{
	Hashtable m_htParam = new Hashtable();
	string m_strAdtoId = null;
	string m_strActionStatus = null;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// リクエスト取得＆ビューステート格納
		//------------------------------------------------------
		m_strActionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);
		ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);

		m_strAdtoId = StringUtility.ToEmpty(Request[Constants.FIELD_MALLPRDCNV_ADTO_ID]);
		ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strAdtoId);

		if (!IsPostBack)
		{
			Hashtable htParam = new Hashtable();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			htParam = GetParameters(Request);
			htParam.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID, m_strAdtoId);

			// 不正パラメータが存在した場合
			if ((bool)htParam[Constants.ERROR_REQUEST_PRAMETER])
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			//------------------------------------------------------
			// 出力ファイル一覧
			//------------------------------------------------------
			int iTotalAdtoCounts = 0;	// ページング可能総数
			int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

			// 商品コンバータデータ取得
			DataView dvProductConverterFiles = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "GetProductConverterFiles"))
			{
				htParam.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1);										// 表示開始記事番号
				htParam.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber);										// 表示開始記事番号

				dvProductConverterFiles = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htParam);
			}
			if (dvProductConverterFiles.Count != 0)
			{
				iTotalAdtoCounts = int.Parse(dvProductConverterFiles[0].Row["row_count"].ToString());

				// エラー非表示制御
				trProductConverterListError.Visible = false;
			}
			else
			{
				iTotalAdtoCounts = 0;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

				// 一覧非表示・エラー表示制御
				trProductConverterListError.Visible = true;
			}

			// データソースセット
			rProductConverterList.DataSource = dvProductConverterFiles;

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			string strNextUrl = CreateProductConverterFilesUrl(m_strAdtoId);
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalAdtoCounts, iCurrentPageNumber, strNextUrl);

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind();
		}
	}

	/// <summary>
	/// 出力ファイル一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>出力ファイル一覧遷移URL</returns>
	protected string CreateProductConverterFilesUrl(string strAdtoId)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTCONVERTER_FILES);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_ADTO_ID).Append("=").Append(strAdtoId);

		return sbResult.ToString();
	}
}
