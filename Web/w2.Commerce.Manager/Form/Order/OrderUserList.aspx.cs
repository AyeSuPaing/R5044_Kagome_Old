/*
=========================================================================================================
  Module      : 注文登録用ユーザ一覧ページ処理(OrderUserList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class Form_Order_OrderUserList : BasePage
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
			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			int iPageNum = 0;
			if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out iPageNum) == false)
			{
				iPageNum = 1;
			}
			string strUserId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_USER_ID]);
			string strUserKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_USER_KBN]);
			string strUserName = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_NAME]);
			string strUserMailAddr = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_MAIL_ADDR]);
			string strUserNameKana = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_NAME_KANA]);
			string strUserTel1 = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_TEL1]);

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			tbUserID.Text = strUserId;
			tbUserName.Text = strUserName;
			tbUserMailAddr.Text = strUserMailAddr;
			tbUserNameKana.Text = strUserNameKana;
			tbUserTel1.Text = strUserTel1;
			// 顧客区分
			ddlUserKbn.Items.Add(new ListItem("", ""));
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN))
			{
				if ((Constants.CS_OPTION_ENABLED == false) && (li.Value == Constants.FLG_USER_USER_KBN_CS)) continue;	// オプションOFF時はCS区分を追加しない
				// モバイルデータの表示と非表示OFF時はMB_USERとMB_GEST区分を追加しない
				if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
					&& ((li.Value == Constants.FLG_USER_USER_KBN_MOBILE_USER)
						|| (li.Value == Constants.FLG_USER_USER_KBN_MOBILE_GUEST))) continue;
				ddlUserKbn.Items.Add(li);
			}
			ddlUserKbn.SelectedValue = strUserKbn;

			if (this.IsNotSearchDefault) return;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			Hashtable htSqlParam = new Hashtable();
			htSqlParam.Add(Constants.FIELD_USER_USER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strUserId));
			htSqlParam.Add(Constants.FIELD_USER_USER_KBN, strUserKbn);
			htSqlParam.Add(Constants.FIELD_USER_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strUserName));
			htSqlParam.Add(Constants.FIELD_USER_MAIL_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strUserMailAddr));
			htSqlParam.Add(Constants.FIELD_USER_NAME_KANA + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strUserNameKana));
			htSqlParam.Add(Constants.FIELD_USER_TEL1 + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strUserTel1));
			htSqlParam.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNum - 1) + 1);							// 表示開始記事番号
			htSqlParam.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNum);

			//------------------------------------------------------
			// ユーザ該当件数取得
			//------------------------------------------------------
			int iTotalUserCounts = 0;	// ページング可能総商品数
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("OrderRegist", "GetUserCount"))
			{
				DataView dvUserCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
				if (dvUserCount.Count != 0)
				{
					iTotalUserCounts = (int)dvUserCount[0]["row_count"];
				}
			}

			//------------------------------------------------------
			// エラー表示制御
			//------------------------------------------------------
			bool blDisplayPager = true;
			StringBuilder sbErrorMessage = new StringBuilder();
			// 上限件数より多い？
			if (iTotalUserCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				sbErrorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(iTotalUserCounts));
				sbErrorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				blDisplayPager = false;
			}
			// 該当件数なし？
			else if (iTotalUserCounts == 0)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdErrorMessage.InnerHtml = sbErrorMessage.ToString();
			trListError.Visible = (sbErrorMessage.ToString().Length != 0);

			//------------------------------------------------------
			// ユーザ一覧情報表示
			//------------------------------------------------------
			if (trListError.Visible == false)
			{
				// ユーザ一覧情報取得
				DataView dvUser = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("OrderRegist", "GetUserList"))
				{
					sqlStatement.UseLiteralSql = true;
					dvUser = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
				}

				// データバインド
				rList.DataSource = dvUser;
				rList.DataBind();
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (blDisplayPager)
			{
				string strNextUrl = CreateUserListUrl(strUserId, strUserKbn, strUserName, strUserMailAddr, strUserNameKana, strUserTel1);
				lbPager1.Text = WebPager.CreateDefaultListPager(iTotalUserCounts, iPageNum, strNextUrl);
			}
		}
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void UserSearch(object sender, EventArgs e)
	{
		StringBuilder sbSearchUrl = new StringBuilder();
		sbSearchUrl.Append(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_USER_LIST);
		sbSearchUrl.Append("?").Append(Constants.REQUEST_KEY_USER_USER_ID).Append("=").Append(HttpUtility.UrlEncode(tbUserID.Text));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_KBN).Append("=").Append(HttpUtility.UrlEncode(ddlUserKbn.SelectedValue));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_NAME).Append("=").Append(HttpUtility.UrlEncode(tbUserName.Text));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_MAIL_ADDR).Append("=").Append(HttpUtility.UrlEncode(tbUserMailAddr.Text));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_NAME_KANA).Append("=").Append(HttpUtility.UrlEncode(tbUserNameKana.Text));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_TEL1).Append("=").Append(HttpUtility.UrlEncode(tbUserTel1.Text));

		// 同じ画面にリダイレクト
		Response.Redirect(sbSearchUrl.ToString());
	}

	/// <summary>
	/// 一覧ＵＲＬ作成
	/// </summary>
	/// <param name="strUserId">ユーザID</param>
	/// <param name="strUserKbn">顧客区分</param>
	/// <param name="strUserName">氏名</param>
	/// <param name="strUserMailAddr">メールアドレス</param>
	/// <param name="strUserNameKana">氏名かな</param>
	/// <param name="strUserTel1">電話番号1</param>
	/// <returns></returns>
	private string CreateUserListUrl(string strUserId, string strUserKbn, string strUserName, string strUserMailAddr, string strUserNameKana, string strUserTel1)
	{
		StringBuilder sbSearchUrl = new StringBuilder();
		sbSearchUrl.Append(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_USER_LIST);
		sbSearchUrl.Append("?").Append(Constants.REQUEST_KEY_USER_USER_ID).Append("=").Append(HttpUtility.UrlEncode(strUserId));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_USER_KBN).Append("=").Append(HttpUtility.UrlEncode(strUserKbn));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_NAME).Append("=").Append(HttpUtility.UrlEncode(strUserName));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_MAIL_ADDR).Append("=").Append(HttpUtility.UrlEncode(strUserMailAddr));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_NAME_KANA).Append("=").Append(HttpUtility.UrlEncode(strUserNameKana));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_USER_TEL1).Append("=").Append(HttpUtility.UrlEncode(strUserTel1));

		return sbSearchUrl.ToString();
	}

	/// <summary>
	/// メールアドレス取得
	/// </summary>
	/// <param name="drvUser">ユーザー情報</param>
	/// <remarks>
	///	モバイル会員、ゲストの場合はメールアドレス2を返す
	/// 上記以外はメールアドレス1を返す
	/// </remarks>
	protected string DisplayMailAddr(DataRowView drvUser)
	{
		string strResult = null;

		switch ((string)drvUser[Constants.FIELD_USER_USER_KBN])
		{
			// モバイル会員、ゲストの場合はメールアドレス2を設定
			case Constants.FLG_USER_USER_KBN_MOBILE_USER:
			case Constants.FLG_USER_USER_KBN_MOBILE_GUEST:
				strResult = (string)drvUser[Constants.FIELD_USER_MAIL_ADDR2];
				break;
			default:
				strResult = (string)drvUser[Constants.FIELD_USER_MAIL_ADDR];
				break;
		}

		return StringUtility.ToEmpty(strResult);
	}
}
