/*
=========================================================================================================
  Module      : オペレータ情報一覧ページ処理(OperatorList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.MailFrom;

public partial class Form_Operator_OperatorList : BasePage
{
	private const string NO_AUTHORITY_VALUE = "-1";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 画面初期化
			Initialize();

			// リクエスト取得
			Hashtable paramData = GetParameters(Request);
			if ((bool)paramData[Constants.ERROR_REQUEST_PRAMETER])
			{
				// 不正ページを指定された場合、エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 一覧表示
			DisplayList(paramData);
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{

		// メニュー権限ドロップダウン
		ddlMenuAccess.Items.Add(new ListItem("", ""));
		foreach (ListItem li in MenuUtility.CreateMenuAuthorityList(this.LoginOperatorShopId, this.LoginOperatorMenuAccessLevel))
		{
			if (li.Value == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER)
			{
				// "アクセス権限なし" のValueを入れ替え
				ddlMenuAccess.Items.Add(new ListItem(li.Text, NO_AUTHORITY_VALUE));
			}
			else
			{
				ddlMenuAccess.Items.Add(li);
			}
		}

		// 有効フラグ
		ddlValid.Items.Add(new ListItem("", ""));
		ddlValid.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_SHOPOPERATOR, Constants.FIELD_SHOPOPERATOR_VALID_FLG));
	}

	/// <summary>
	/// オペレータ一覧パラメタ取得
	/// </summary>
	/// <param name="request">HttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters(HttpRequest request)
	{
		Hashtable result = new Hashtable();
		int currentPageNo = 1;
		bool hasParamError = false;

		try
		{
			// オペレータID
			result.Add(Constants.REQUEST_KEY_OPERATOR_ID, StringUtility.ToEmpty(request[Constants.REQUEST_KEY_OPERATOR_ID]));
			tbOperatorId.Text = request[Constants.REQUEST_KEY_OPERATOR_ID];
			// オペレータ名
			result.Add(Constants.REQUEST_KEY_OPERATOR_NAME, StringUtility.ToEmpty(request[Constants.REQUEST_KEY_OPERATOR_NAME]));
			tbOperatorName.Text = request[Constants.REQUEST_KEY_OPERATOR_NAME];
			// メニュー権限
			result.Add(Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL, StringUtility.ToEmpty(request[Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL]));
			ddlMenuAccess.SelectedValue = request[Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL];
			// 有効フラグ
			result.Add(Constants.REQUEST_KEY_OPERATOR_VALID_FLG, StringUtility.ToEmpty(request[Constants.REQUEST_KEY_OPERATOR_VALID_FLG]));
			ddlValid.SelectedValue = request[Constants.REQUEST_KEY_OPERATOR_VALID_FLG];
			// メールアドレス
			result.Add(Constants.REQUEST_KEY_OPERATOR_MAIL_ADDR, StringUtility.ToEmpty(request[Constants.REQUEST_KEY_OPERATOR_MAIL_ADDR]));
			tbOperatorMailAddress.Text = request[Constants.REQUEST_KEY_OPERATOR_MAIL_ADDR];
			// CSオペレータか否か
			result.Add(Constants.REQUEST_KEY_OPERATOR_IS_CS_OPERATOR, StringUtility.ToEmpty(request[Constants.REQUEST_KEY_OPERATOR_IS_CS_OPERATOR]));
			ddlIsCsOperator.SelectedValue = request[Constants.REQUEST_KEY_OPERATOR_IS_CS_OPERATOR];
			// ソート区分
			string sortKbn = null;
			switch (StringUtility.ToEmpty(request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_OPERATOR_LIST_ID_ASC:					// オペレータID/昇順
				case Constants.KBN_SORT_OPERATOR_LIST_ID_DESC:					// オペレータID/降順
				case Constants.KBN_SORT_OPERATOR_LIST_NAME_ASC:					// オペレータ名/昇順
				case Constants.KBN_SORT_OPERATOR_LIST_NAME_DESC:				// オペレータ名/降順
				case Constants.KBN_SORT_OPERATOR_LIST_DISPLAY_ORDER_ASC:		// CSオペレータ表示順/昇順
				case Constants.KBN_SORT_OPERATOR_LIST_DISPLAY_ORDER_DESC:		// CSオペレータ表示順/降順
					sortKbn = request[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;

				case "":
					// 通常はオペレータID/昇順、CSモードであれば表示順/昇順
					sortKbn = Constants.KBN_SORT_OPERATOR_LIST_DISPLAY_ORDER_ASC;
					break;

				default:
					hasParamError = true;
					break;
			}
			result.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);
			ddlSortKbn.SelectedValue = sortKbn;

			// ページ番号
			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) == "")
			{
				currentPageNo = 1;
			}
			else
			{
				currentPageNo = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO].ToString());
			}
		}
		catch
		{
			hasParamError = true;
		}

		// パラメタ取得
		result.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNo);
		result.Add(Constants.ERROR_REQUEST_PRAMETER, hasParamError);

		return result;
	}

	/// <summary>
	/// 一覧表示
	/// </summary>
	/// <param name="paramData">リクエストパラメータデータ</param>
	private void DisplayList(Hashtable paramData)
	{
		int pageNo = (int)paramData[Constants.REQUEST_KEY_PAGE_NO];
		int bgnRow = (pageNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
		int endRow = pageNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;

		// オペレータ一覧を取得
		DataView dvOeratorList;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopOperator", "GetOperatorList"))
		{
			Hashtable htInput = GetSearchSqlInfo(paramData);
			htInput.Add(Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId);
			htInput.Add(Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG, this.LoginOperatorMenuAccessLevel);
			htInput.Add("bgn_row_num", bgnRow);
			htInput.Add("end_row_num", endRow);

			dvOeratorList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		// メール送信元一覧を取得
		CsMailFromService service = new CsMailFromService(new CsMailFromRepository());
		CsMailFromModel[] mailFromModels = service.GetValidAll(this.LoginOperatorDeptId);

		// オペレータ一覧をセット
		if (dvOeratorList.Count != 0)
		{
			// ページャ設定
			int iTotal = int.Parse(dvOeratorList[0]["row_count"].ToString());
			string nextUrl = CreateListUrl(tbOperatorId.Text.Trim(), tbOperatorName.Text.Trim(), ddlMenuAccess.SelectedValue,
				ddlValid.SelectedValue, tbOperatorMailAddress.Text.Trim(), ddlIsCsOperator.SelectedValue, ddlSortKbn.SelectedValue);
			lbPager.Text = WebPager.CreateDefaultListPager(iTotal, pageNo, nextUrl);

			// データソースセット＆データバインド
			List<CsOperatorModel> list = new List<CsOperatorModel>();
			foreach (DataRowView drvOperator in dvOeratorList)
			{
				CsOperatorModel model = new CsOperatorModel(drvOperator);
				foreach (var mailFrom in mailFromModels)
				{
					if (model.MailFromId == mailFrom.MailFromId) model.EX_MailFromDisplayName = mailFrom.EX_DisplayAddress;
				}
				list.Add(model);
			}
			rOperatorList.DataSource = list;
			DataBind();
		}
		else
		{
			// リストが取得できなかったとき
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="input">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable input)
	{
		Hashtable result = new Hashtable();
	
		// オペレータID
		result.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(input[Constants.REQUEST_KEY_OPERATOR_ID]));
		// オペレータ名
		result.Add(Constants.FIELD_SHOPOPERATOR_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(input[Constants.REQUEST_KEY_OPERATOR_NAME]));
		// メニュー権限
		switch(StringUtility.ToEmpty(input[Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL]))
		{
			// 検索条件としない場合
			case "":
				result.Add("condition_menu_access_level", null);
				break;
			// 権限なしを条件とする場合
			case NO_AUTHORITY_VALUE:
				result.Add("condition_menu_access_level", "999999");
				break;
			// 権限なし以外を条件とする場合
			default:
				result.Add("condition_menu_access_level", StringUtility.ToEmpty(input[Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL]));
				break;
		}
		// 有効フラグ
		result.Add(Constants.FIELD_SHOPOPERATOR_VALID_FLG, StringUtility.SqlLikeStringSharpEscape(input[Constants.REQUEST_KEY_OPERATOR_VALID_FLG]));
		// メールアドレス
		result.Add(Constants.FIELD_CSOPERATOR_MAIL_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(input[Constants.REQUEST_KEY_OPERATOR_MAIL_ADDR]));
		// CSオペレータか否か
		result.Add("is_cs_operator", StringUtility.ToEmpty(input[Constants.REQUEST_KEY_OPERATOR_IS_CS_OPERATOR]));
		// ソート区分
		result.Add("sort_kbn", StringUtility.ToEmpty(input[Constants.REQUEST_KEY_SORT_KBN]));

		return result;
	}

	/// <summary>
	/// 一覧画面URL作成
	/// </summary>
	/// <param name="operatorId">オペレータID</param>
	/// <param name="operatorName">オペレータ名</param>
	/// <param name="menuAccessLevel">メニュー権限</param>
	/// <param name="validFlg">有効フラグ</param>
	/// <param name="operatorMailAddress">メールアドレス</param>
	/// <param name="csOperatorflg">CSオペレータフラグ</param>
	/// <param name="sortKbn">並び順</param>
	/// <returns>一覧遷移URL</returns>
	private string CreateListUrl(string operatorId, string operatorName, string menuAccessLevel, string validFlg, string operatorMailAddress, string csOperatorflg, string sortKbn)
	{
		StringBuilder builder = new StringBuilder();
		builder.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_OPERATOR_LIST);
		builder.Append("?").Append(Constants.REQUEST_KEY_OPERATOR_ID).Append("=").Append(HttpUtility.UrlEncode(operatorId));
		builder.Append("&").Append(Constants.REQUEST_KEY_OPERATOR_NAME).Append("=").Append(HttpUtility.UrlEncode(operatorName));
		builder.Append("&").Append(Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL).Append("=").Append(HttpUtility.UrlEncode(menuAccessLevel));
		builder.Append("&").Append(Constants.REQUEST_KEY_OPERATOR_VALID_FLG).Append("=").Append(HttpUtility.UrlEncode(validFlg));
		builder.Append("&").Append(Constants.REQUEST_KEY_OPERATOR_MAIL_ADDR).Append("=").Append(HttpUtility.UrlEncode(operatorMailAddress));
		builder.Append("&").Append(Constants.REQUEST_KEY_OPERATOR_IS_CS_OPERATOR).Append("=").Append(HttpUtility.UrlEncode(csOperatorflg));
		builder.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode(sortKbn));
		return builder.ToString();
	}

	/// <summary>
	/// データバインド用：詳細画面URL作成
	/// </summary>
	/// <param name="operatorId">オペレータID</param>
	/// <returns>詳細画面URL</returns>
	protected string CreateDetailUrl(string operatorId)
	{
		StringBuilder builder = new StringBuilder();
		builder.Append(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_CONFIRM);
		builder.Append("?").Append(Constants.REQUEST_KEY_OPERATOR_ID).Append("=").Append(HttpUtility.UrlEncode(operatorId));
		builder.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);
		return builder.ToString();
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNew_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// セッションに残っているオペレータ情報をクリア
		Session[Constants.SESSION_KEY_OPERATOR_INFO] = null;

		// 新規登録画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// 一覧へ
		Response.Redirect(CreateListUrl(
			tbOperatorId.Text.Trim(),			// オペレータID
			tbOperatorName.Text.Trim(),			// オペレータ名
			ddlMenuAccess.SelectedValue,		// メニュー権限
			ddlValid.SelectedValue,				// 有効フラグ
			tbOperatorMailAddress.Text.Trim(),	// オペレータメールアドレス
			ddlIsCsOperator.SelectedValue,		// CSオペレータか否か
			ddlSortKbn.SelectedValue)			// ソート区分
		);
	}

	/// <summary>
	/// メニュー権限3名称取得
	/// </summary>
	/// <param name="csOperator">CSオペレータ</param>
	protected string GetMenuAccessLevel3Name(CsOperatorModel csOperator)
	{
		if (csOperator.EX_MenuAccessLevel3 == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER) return Constants.STRING_SUPERUSER_NAME;
		if (csOperator.EX_MenuAccessLevel3 == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER) return Constants.STRING_UNACCESSABLEUSER_NAME;

		return csOperator.EX_MenuAuthorityName;
	}
}
