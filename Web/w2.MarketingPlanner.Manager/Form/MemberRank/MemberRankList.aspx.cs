/*
=========================================================================================================
  Module      : 会員ランク設定一覧ページ処理(MemberRankList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using w2.App.Common.RefreshFileManager;

public partial class Form_MemberRank_MemberRankList : MemberRankPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack)
		{
			Hashtable htParam = new Hashtable();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			htParam = GetParameters(Request);
			// 不正パラメータが存在した場合
			if ((bool)htParam[Constants.ERROR_REQUEST_PRAMETER])
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
			int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

			//------------------------------------------------------
			// 会員ランク設定一覧表示
			//------------------------------------------------------
			int iTotalMemberRankCounts = 0;	// ページング可能総数
			// 会員ランク情報取得
			DataView dvMemberRank = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MemberRank", "GetMemberRankListByRowIndex"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1);
				htInput.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber);

				dvMemberRank = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			if (dvMemberRank.Count != 0)
			{
				iTotalMemberRankCounts = int.Parse(dvMemberRank[0].Row["row_count"].ToString());
				// エラー非表示制御
				trListError.Visible = false;
				
				// デフォルト会員ランク設定取得
				DataView dvDefaultMemberRank = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("MemberRank", "GetDefaultMemberRank"))
				{
					dvDefaultMemberRank = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
				}

				if (dvDefaultMemberRank.Count != 0)
				{
					// デフォルト会員ランクのIDをプロパティに設定
					DefaultRankId = (string)dvDefaultMemberRank[0][Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID];
				}
			}
			else
			{
				iTotalMemberRankCounts = 0;
				// エラー表示制御
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// データバインド
			rList.DataSource = dvMemberRank;
			rList.DataBind();

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			string strNextUrl = Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_LIST;
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalMemberRankCounts, iCurrentPageNumber, strNextUrl);
		}
    }

	/// <summary>
	/// 会員ランク設定一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">会員ランク設定一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	private static Hashtable GetParameters(HttpRequest hrRequest)
	{
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

	/// <summary>
	/// データバインド用会員ランク設定詳細URL作成
	/// </summary>
	/// <param name="strRankId">ランクID</param>
	/// <returns>会員ランク設定詳細URL</returns>
	protected string CreateMemberRankDetailUrl(string strRankId)
	{
		StringBuilder sbUrl = new StringBuilder();

		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MEMBERRANK_ID).Append("=").Append(HttpUtility.UrlEncode(strRankId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbUrl.ToString();
	}

	/// <summary>
	/// 新規ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// デフォルト設定更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDefaultUpdate_Click(object sender, EventArgs e)
	{
		// 選択されたデフォルト会員ランクを取得
		string strDefaultRankId = Request["rbDefaultRank"];

		// デフォルト会員ランクを変更（既存のデフォルト設定ランクを非デフォルトに、選択されたランクをデフォルトに設定）
		if (DefaultRankId != strDefaultRankId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MemberRank", "ChangeDefaultMemberRank"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID + "_old", DefaultRankId);		// 既存のデフォルト設定ランク
				htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID + "_new", strDefaultRankId);	// 新規のデフォルト設定ランク

				int iUpdate = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		// 各サイトの会員ランク情報更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.MemberRank).CreateUpdateRefreshFile();

		// 一覧に戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_LIST);
	}

	/// <summary>
	/// 翻訳データ出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_OnClick(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = new string[0];
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}
	
	/// <summary>デフォルト設定会員ランクID</summary>
	public string DefaultRankId
	{
		get { return (string)ViewState["DefaultRankId"]; }
		set { ViewState["DefaultRankId"] = value; }
	}
}