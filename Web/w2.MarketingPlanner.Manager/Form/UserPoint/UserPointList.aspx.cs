/*
=========================================================================================================
  Module      : ユーザポイント一覧ページ処理(UserPointList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common;
using w2.App.Common.User;
using w2.Common.Web;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.UpdateHistory.Helper;

public partial class Form_UserPoint_UserPointList : BasePage
{

	//=========================================================================================
	// 処理結果定数
	//=========================================================================================
	private const string RESULT_USERPOINT = "userpoint";					// ユーザーポイント情報
	private const string RESULT_UPDATE_POINT_RESULT = "update_stock_result";		// ポイント
	private const string RESULT_UPDATE_EXP_DATETIME_RESULT = "update_stock_alert_result";	// 有効期限

	//=========================================================================================
	// データバインド用変数
	//=========================================================================================
	protected Hashtable m_htUpdatePointResult = new Hashtable();			// ポイント更新結果データバインド用
	protected Hashtable m_htUpdateExpDateTimeResult = new Hashtable();		// 有効期限更新結果データバインド用

	protected System.Web.UI.WebControls.Button btnPoingUpdateBottom;

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable htParam = GetParameters(Request);
			// 不正パラメータが存在した場合
			if ((bool)htParam[Constants.ERROR_REQUEST_PRAMETER])
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}

			//------------------------------------------------------
			// 検索ドロップダウンリスト制御
			// ユーザ：「法人（企業名、部署名）」利用設定で切り替える
			//------------------------------------------------------
			if (Constants.DISPLAY_CORPORATION_ENABLED)
			{
				// 検索ドロップダウン作成("企業名", "部署名"あり)
				foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USERPOINT, "search_key"))
				{
					ddlSearchKey.Items.Add(li);
				}
			}
			else
			{
				// 検索ドロップダウン作成("企業名", "部署名"除外)
				foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USERPOINT, "search_key"))
				{
					if ((li.Value != Constants.KBN_SEARCHKEY_USERPOINT_LIST_COMPANY_NAME) && (li.Value != Constants.KBN_SEARCHKEY_USERPOINT_LIST_COMPANY_POST_NAME))
					{
						ddlSearchKey.Items.Add(li);
					}
				}
			}

			// ポイント区分 ラジオボタンリスト作成
			foreach (var item in ValueText.GetValueItemArray(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_KBN))
			{
				if (Constants.CROSS_POINT_OPTION_ENABLED
					&& (item.Value != Constants.FLG_USERPOINT_POINT_KBN_BASE))
				{
					continue;
				}

				rblPointKbn.Items.Add(item);
			}

			//------------------------------------------------------
			// 検索情報取得
			//------------------------------------------------------
			string strSearchKey = StringUtility.ToEmpty((string)htParam[Constants.REQUEST_KEY_SEARCH_KEY]);
			string strSearchWord = StringUtility.ToEmpty((string)htParam[Constants.REQUEST_KEY_SEARCH_WORD]);
			string strSortKbn = StringUtility.ToEmpty((string)htParam[Constants.REQUEST_KEY_SORT_KBN]);
			int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];
			string strPointKbn = Constants.CROSS_POINT_OPTION_ENABLED
				? Constants.FLG_USERPOINT_POINT_KBN_BASE
				: StringUtility.ToEmpty((string)htParam[Constants.REQUEST_KEY_POINT_KBN]);

			//------------------------------------------------------
			// 検索コントロール制御（一覧共通処理）
			//------------------------------------------------------
			foreach (ListItem li in ddlSearchKey.Items)
			{
				li.Selected = (li.Value == strSearchKey);
			}
			tbSearchWord.Text = strSearchWord;
			foreach (ListItem li in ddlSortKbn.Items)
			{
				li.Selected = (li.Value == strSortKbn);
			}
			foreach (ListItem li in rblPointKbn.Items)
			{
				li.Selected = (li.Value == strPointKbn);
			}

			if (this.IsNotSearchDefault) return;

			//------------------------------------------------------
			// 検索情報保持(その他ページの「一覧へ戻る」ボタン実行時に利用)
			//------------------------------------------------------
			Session[Constants.SESSIONPARAM_KEY_USERPOINT_SEARCH_INFO] = htParam;

			//------------------------------------------------------
			// 完了表示の場合
			//------------------------------------------------------
			if ((string)htParam[Constants.REQUEST_KEY_DISPLAY_KBN] == Constants.KBN_USERPOINT_DISPLAY_COMPLETE)
			{
				// 処理結果取得
				Hashtable htUpdatePoint = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				m_htUpdatePointResult = (Hashtable)htUpdatePoint[RESULT_UPDATE_POINT_RESULT];
				m_htUpdateExpDateTimeResult = (Hashtable)htUpdatePoint[RESULT_UPDATE_EXP_DATETIME_RESULT];

				// データセット
				rComplete.DataSource = (ArrayList)htUpdatePoint[RESULT_USERPOINT];
				rComplete.DataBind();

				divPointEdit.Visible = false;
				divPointComplete.Visible = true;
				trList.Visible = false;

				// 検索処理が行われた場合は、編集表示へ遷移させる
				ViewState[Constants.REQUEST_KEY_DISPLAY_KBN] = Constants.KBN_USERPOINT_DISPLAY_EDIT;

				return;		// 処理を抜ける
			}

			// 検索条件を生成
			var cond = new UserPointSearchCondition
			{
				SearchKey = int.Parse(string.IsNullOrEmpty(strSearchWord) ? "99" : strSearchKey),
				SearchWordLikeEscaped = StringUtility.SqlLikeStringSharpEscape(strSearchWord),
				SortKbn = int.Parse(strSortKbn),
				DeptId = this.LoginOperatorDeptId,
				SelectPointKbn = strPointKbn,
				BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1,
				EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber
			};

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				cond.DelFlg = Constants.FLG_USER_DELFLG_UNDELETED;
			}

			int iTotalPointCounts = 0;	// ページング可能総商品数
			var sv = new PointService();
			// 検索（件数だけ）
			iTotalPointCounts = sv.GetCountOfUserPointListSearch(cond);

			//------------------------------------------------------
			// 表示制御
			//------------------------------------------------------
			bool blDisplayPager = true;
			StringBuilder sbErrorMessage = new StringBuilder();
			// 上限件数より多い？
			if (iTotalPointCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				sbErrorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(iTotalPointCounts));
				sbErrorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				blDisplayPager = false;
			}
			// 該当件数なし？
			else if (iTotalPointCounts == 0)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdErrorMessage.InnerHtml = sbErrorMessage.ToString();
			trListError.Visible = (sbErrorMessage.ToString().Length != 0);
			// 更新できないようにする
			btnPointUpdateTop.Visible = btnPointUpdateBottom.Visible = (trListError.Visible == false);

			if (trListError.Visible == false)
			{
				// 検索
				var res = sv.UserPointListSearch(cond);
				var result = res.Select(re => new WrappedSearchResult(re)).ToArray();
				ViewState.Add(Constants.TABLE_USERPOINT, result);
				rEdit.DataSource = result;
				rEdit.DataBind();
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (blDisplayPager)
			{
				string strNextUrl = CreateUserPointListUrl(strSearchKey, strSearchWord, strSortKbn, strPointKbn, (string)htParam[Constants.REQUEST_KEY_DISPLAY_KBN]);
				lbPager1.Text = WebPager.CreateDefaultListPager(iTotalPointCounts, iCurrentPageNumber, strNextUrl);
			}

			//------------------------------------------------------
			// エンターキーでのSubmitを無効とする領域を設定する
			// ※RepeaterがBindされていないと正常に動作しない
			//------------------------------------------------------
			// KeyEventをキャンセルするスクリプトを設定
			new InnerTextBoxList(divPointEdit).SetKeyPressEventCancelEnterKey();
		}
	}
	#endregion

	#region #IsHaveUserPointHistory ユーザーポイント情報履歴があるかどうか
	/// <summary>
	/// ユーザーポイント情報履歴があるかどうか
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>TRUE：ある FALSE：無し</returns>
	protected bool HaveUserPointHistory(string userId)
	{
		var userPointHistory = new PointService().GetUserPointHistories(userId).Count();
		return (userPointHistory != 0);
	}
	#endregion

	#region -GetParameters ユーザーポイント情報一覧パラメタ取得
	/// <summary>
	/// ユーザーポイント情報一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">ユーザーポイント情報一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	private static Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();
		int iCurrentPageNumber = 1;
		string strSearchKey = String.Empty;
		string strSearchWord = String.Empty;
		string strSortKbn = String.Empty;
		string strPointKbn = String.Empty;
		string strDisplayKbn = String.Empty;
		bool blParamError = false;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// 表示区分
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_DISPLAY_KBN]))
			{
				case Constants.KBN_USERPOINT_DISPLAY_EDIT:						// 編集表示
				case Constants.KBN_USERPOINT_DISPLAY_COMPLETE:					// 完了表示
					strDisplayKbn = hrRequest[Constants.REQUEST_KEY_DISPLAY_KBN].ToString();
					break;
				case "":
					strDisplayKbn = Constants.KBN_USERPOINT_DISPLAY_DEFAULT;	// 編集表示がデフォルト
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
				case Constants.KBN_SEARCHKEY_USERPOINT_LIST_USER_ID:						// ユーザーID
				case Constants.KBN_SEARCHKEY_USERPOINT_LIST_NAME:							// 氏名
				case Constants.KBN_SEARCHKEY_USERPOINT_LIST_NAME_KANA:						// フリガナ
				case Constants.KBN_SEARCHKEY_USERPOINT_LIST_TEL1:							// 電話番号
				case Constants.KBN_SEARCHKEY_USERPOINT_LIST_MAIL_ADDR:						// メールアドレス
				case Constants.KBN_SEARCHKEY_USERPOINT_LIST_ZIP1:							// 郵便番号
				case Constants.KBN_SEARCHKEY_USERPOINT_LIST_ADDR:							// 住所
				case Constants.KBN_SEARCHKEY_USERPOINT_LIST_COMPANY_NAME:                   // 企業名
				case Constants.KBN_SEARCHKEY_USERPOINT_LIST_COMPANY_POST_NAME:              // 部署名
				case "99":																	// 未指定
					strSearchKey = hrRequest[Constants.REQUEST_KEY_SEARCH_KEY].ToString();
					break;
				case "":
					strSearchKey = Constants.KBN_SEARCHKEY_USERPOINT_LIST_DEFAULT;			// ユーザーIDがデフォルト
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
				case Constants.KBN_SORT_USERPOINT_LIST_POINT_ASC:				// ポイント/昇順
				case Constants.KBN_SORT_USERPOINT_LIST_POINT_DESC:				// ポイント/降順
				case Constants.KBN_SORT_USERPOINT_LIST_EXP_DATETIME_ASC:		// 有効期限/昇順
				case Constants.KBN_SORT_USERPOINT_LIST_EXP_DATETIME_DESC:		// 有効期限/降順
				case Constants.KBN_SORT_USERPOINT_LIST_USER_ID_ASC:				// ユーザID/昇順
				case Constants.KBN_SORT_USERPOINT_LIST_USER_ID_DESC:			// ユーザID/降順

					strSortKbn = hrRequest[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				case "":
					strSortKbn = Constants.KBN_SORT_USERPOINT_LIST_DEFAULT;		// ポイント/降順がデフォルト
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

		// ポイント区分
		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_POINT_KBN]))
			{

				case Constants.KBN_USERPOINT_LIST_ALL:
				case Constants.KBN_USERPOINT_LIST_BASE:
				case Constants.KBN_USERPOINT_LIST_LIMITED_TERM_POINT:
					strPointKbn = hrRequest[Constants.REQUEST_KEY_POINT_KBN];
					break;

				// 空の場合デフォルト値を設定する
				case "":
					strPointKbn = Constants.KBN_USERPOINT_LIST_DEFAULT;
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
		// パラメタ取得
		//------------------------------------------------------
		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_KEY, strSearchKey);
		htResult.Add(Constants.REQUEST_KEY_SEARCH_WORD, strSearchWord);
		htResult.Add(Constants.REQUEST_KEY_SORT_KBN, strSortKbn);
		htResult.Add(Constants.ERROR_REQUEST_PRAMETER, blParamError);
		htResult.Add(Constants.REQUEST_KEY_POINT_KBN, strPointKbn);
		htResult.Add(Constants.REQUEST_KEY_DISPLAY_KBN, strDisplayKbn);

		return htResult;
	}
	#endregion

	#region -CreateUserPointListUrl ユーザポイント一覧遷移URL作成
	/// <summary>
	/// ユーザポイント一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <param name="strPointKbn">ポイント区分</param>
	/// <param name="strDisplayKbn">表示区分</param>
	/// <returns>ユーザポイント一覧遷移URL</returns>
	private string CreateUserPointListUrl(string strSearchKey, string strSearchWord, string strSortKbn, string strPointKbn, string strDisplayKbn)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USERPOINT_LIST)
			.AddParam(Constants.REQUEST_KEY_SEARCH_KEY, strSearchKey)
			.AddParam(Constants.REQUEST_KEY_SEARCH_WORD, strSearchWord)
			.AddParam(Constants.REQUEST_KEY_SORT_KBN, strSortKbn)
			.AddParam(Constants.REQUEST_KEY_POINT_KBN, strPointKbn)
			.AddParam(Constants.REQUEST_KEY_DISPLAY_KBN, strDisplayKbn)
			.CreateUrl();

		return url;
	}
	#endregion

	#region -CreateUserPointListUrl ユーザポイント一覧遷移URL作成
	/// <summary>
	/// ユーザポイント一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <param name="strPointKbn">ポイント区分</param>
	/// <param name="strDisplayKbn">表示区分</param>
	/// <param name="iPageNumber">ページ番号</param>
	/// <returns>ユーザポイント一覧遷移URL</returns>
	private string CreateUserPointListUrl(string strSearchKey, string strSearchWord,
		string strSortKbn, string strPointKbn, string strDisplayKbn, int iPageNumber)
	{
		var url = new UrlCreator(CreateUserPointListUrl(strSearchKey, strSearchWord, strSortKbn, strPointKbn, strDisplayKbn))
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, iPageNumber.ToString())
			.CreateUrl();

		return url;
	}
	#endregion

	#region #CreateUserPointHistoryListUrl データバインド用ユーザーポイント履歴情報一覧URL作成
	/// <summary>
	/// データバインド用ユーザーポイント履歴情報一覧URL作成
	/// </summary>
	/// <param name="strUserId">ユーザーID</param>
	/// <returns>データバインド用ユーザーポイント履歴情報一覧URL</returns>
	protected string CreateUserPointHistoryListUrl(string strUserId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USERPOINTHISTORY_LIST)
			.AddParam(Constants.REQUEST_KEY_USERID, strUserId)
			.CreateUrl();

		return url;
	}
	#endregion

	#region #btnSearch_Click 検索ボタンクリック
	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// ユーザポイント情報一覧へ
		Response.Redirect(
			CreateUserPointListUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim(), ddlSortKbn.SelectedValue, rblPointKbn.SelectedValue, (string)ViewState[Constants.REQUEST_KEY_DISPLAY_KBN], 1));
	}
	#endregion

	#region #btnPointUpdateTop_Click このページの一括更新クリック
	/// <summary>
	/// このページの一括更新クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnPointUpdateTop_Click(object sender, System.EventArgs e)
	{
		StringBuilder sbErrorMessages = new StringBuilder();
		ArrayList alUpdatePoint = new ArrayList();
		Hashtable htUpdatePoint = new Hashtable();
		Hashtable htUpdateExpDateTime = new Hashtable();
		string strNo = String.Empty;
		string strPointOperator = String.Empty;
		string strPoint = String.Empty;
		string strExpDateTimeOperator = String.Empty;
		string addExpireMonthsVal = String.Empty;
		string addExpireDaysVal = String.Empty;
		int iPoint = 0;
		int addExpireMonths = 0; //有効期限の延長（ヵ月）
		int addExpireDays = 0; //有効期限の延長（ヵ日）
		string strUserId = String.Empty;
		string strPointKbn = String.Empty;
		int iPointKbnNo = 0;
		bool blUpdatePoint = false;
		bool blUpdateExpDateTime = false;
		bool blSelected = false;

		// ユーザーポイント情報取得
		WrappedSearchResult[] searchResult = ((WrappedSearchResult[])ViewState[Constants.TABLE_USERPOINT]);

		// 入力チェック
		for (int iLoop = 0; iLoop < searchResult.Length; iLoop++)
		{
			// コントロール選択(tbPoint1 OR tbPoint2 etc)
			strNo = ((iLoop + 1) % 2) == 1 ? "1" : "2";

			// ポイント
			strPointOperator = ((DropDownList)(rEdit.Items[iLoop].FindControl("ddlPointOperator" + strNo))).SelectedValue;	// ポイント（＋－）
			strPoint = ((TextBox)(rEdit.Items[iLoop].FindControl("tbPoint" + strNo))).Text;									// ポイント

			if (Constants.CROSS_POINT_OPTION_ENABLED == false)
			{
				// 有効期限（＋－）
				strExpDateTimeOperator = ((DropDownList)(rEdit.Items[iLoop].FindControl("ddlExpDateTimeOperator" + strNo))).SelectedValue;
				// 有効期限（ヵ日）
				addExpireMonthsVal = ((TextBox)(rEdit.Items[iLoop].FindControl("tbMonthOfExpDateTime" + strNo))).Text;
				// 有効期限（ヵ日）
				addExpireDaysVal = ((TextBox)(rEdit.Items[iLoop].FindControl("tbExpDateTime" + strNo))).Text;
			}

			// 入力値をハッシュテーブルに格納
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_USERPOINT_POINT + "_operator", strPointOperator);				// ポイント（＋－）
			htInput.Add(Constants.FIELD_USERPOINT_POINT, strPoint);										// ポイント

			if (Constants.CROSS_POINT_OPTION_ENABLED == false)
			{
				// 有効期限（＋－）
				htInput.Add(Constants.FIELD_USERPOINT_POINT_EXP + "_operator", strExpDateTimeOperator);
				// 有効期限（ヵ月）
				htInput.Add(Constants.FIELD_USERPOINT_POINT_EXP + "_months", addExpireMonthsVal);
				htInput.Add(Constants.FIELD_USERPOINT_POINT_EXP + "_days", addExpireDaysVal);
			}

			// 入力チェック
			sbErrorMessages.Append(Validator.Validate("UserPoint", htInput).Replace("@@ 1 @@", searchResult[iLoop].Name));
			if (sbErrorMessages.Length != 0) continue;

			// ポイント区分
			var pointKbn = (((HiddenField)(rEdit.Items[iLoop].FindControl("hfPointKbn" + strNo))).Value);

			var pointInc = decimal.Parse(strPoint);
			var pointOperation = pointInc * ((strPointOperator == "1") ? -1 : 1);
			var isNegativePoint = ((searchResult[iLoop].RealPoint + pointOperation) < 0);
			if (isNegativePoint == false) continue;

			// CROSS POINT連携の場合、保持ポイントはマイナスにできない
			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				sbErrorMessages.Append(
					CommerceMessages.GetMessages(
						CommerceMessages.ERRMSG_MANAGER_CROSSPOINT_NEGATIVE_POINT_UNACCEPTABLE));
			}

			// 期間限定ポイントはマイナスにできない
			if (pointKbn == Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT)
			{
				sbErrorMessages.Append(
					WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_USERPOINT_NEGATIVE_POINT_UNACCEPTABLE));
			}
		}

		// エラーページへ
		if (sbErrorMessages.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// ポイント,有効期限変更
		for (int iLoop = 0; iLoop < searchResult.Length; iLoop++)
		{
			// コントロール選択(tbPoint1 OR tbPoint2 etc)
			strNo = ((iLoop + 1) % 2) == 1 ? "1" : "2";

			// ユーザーID、ポイント区分、枝番
			strUserId = searchResult[iLoop].UserId;
			strPointKbn = searchResult[iLoop].PointKbn;
			iPointKbnNo = searchResult[iLoop].PointKbnNo;

			// ポイント
			strPointOperator = ((DropDownList)(rEdit.Items[iLoop].FindControl("ddlPointOperator" + strNo))).SelectedValue;	// ポイント（＋－）
			iPoint = int.Parse(((TextBox)(rEdit.Items[iLoop].FindControl("tbPoint" + strNo))).Text);						// ポイント
			iPoint = strPointOperator == "0" ? iPoint : -iPoint;															// ＋－付与

			if (Constants.CROSS_POINT_OPTION_ENABLED == false)
			{
				// 有効期限
				// 有効期限（＋－）
				strExpDateTimeOperator = ((DropDownList)(rEdit.Items[iLoop].FindControl("ddlExpDateTimeOperator" + strNo))).SelectedValue;
				// 有効期限（ヵ月）
				addExpireMonths = int.Parse(((TextBox)(rEdit.Items[iLoop].FindControl("tbMonthOfExpDateTime" + strNo))).Text);
				// ＋－付与（ヵ月）
				addExpireMonths = (strExpDateTimeOperator == "0") ? addExpireMonths : -addExpireMonths;
				// 有効期限（ヵ日）
				addExpireDays = int.Parse(((TextBox)(rEdit.Items[iLoop].FindControl("tbExpDateTime" + strNo))).Text);
				addExpireDays = (strExpDateTimeOperator == "0") ? addExpireDays : -addExpireDays;
			}

			// ポイント枝番
			var pointKbnNo = int.Parse((((HiddenField)(rEdit.Items[iLoop].FindControl("hfPointKbnNo" + strNo))).Value));
			// ポイント区分
			var pointKbn = (((HiddenField)(rEdit.Items[iLoop].FindControl("hfPointKbn" + strNo))).Value);

			// 更新対象
			blUpdatePoint = iPoint != 0;
			if (Constants.CROSS_POINT_OPTION_ENABLED == false)
			{
				blUpdateExpDateTime = ((addExpireDays != 0) || (addExpireMonths != 0));
			}

			// 完了表示用に格納
			// null値は処理対象外
			if (blUpdatePoint == false)
			{
				htUpdatePoint.Add(strUserId + pointKbn + pointKbnNo, null);
			}
			else
			{
				htUpdatePoint.Add(strUserId + pointKbn + pointKbnNo, true);
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED
				|| blUpdateExpDateTime == false)
			{
				htUpdateExpDateTime.Add(strUserId + pointKbn + pointKbnNo, null);
			}
			else
			{
				htUpdateExpDateTime.Add(strUserId + pointKbn + pointKbnNo, true);
			}

			// ポイントが変更されていた場合
			if (blUpdatePoint)
			{
				var sv = new PointService();

				// CrossPointにポイント変更を連携
				if (Constants.CROSS_POINT_OPTION_ENABLED)
				{
					var updateInput = new PointApiInput
					{
						MemberId = strUserId,
						UpdatePoint = iPoint,
						ReasonId = CrossPointUtility.GetValue(Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID, w2.App.Common.Constants.CROSS_POINT_REASON_KBN_OPERATOR),
					};

					var result = new CrossPointPointApiService().UpdatePoint(updateInput.GetParam(PointApiInput.RequestType.Update));
					if (result.IsSuccess == false)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
					}
				}

				// ポイント操作内容を組み立てる
				var operation = new PointOperationContents
				{
					DeptId = this.LoginOperatorDeptId,
					UserId = strUserId,
					PointKbn = pointKbn,
					PointKbnNo = pointKbnNo,
					AddPoint = iPoint,
					AddExpMonths = Constants.CROSS_POINT_OPTION_ENABLED ? 0 : addExpireMonths,
					AddExpDays = Constants.CROSS_POINT_OPTION_ENABLED ? 0 : addExpireDays,
					OperatorName = this.LoginOperatorName
				};

				// ポイント操作実施（更新履歴とともに）
				int iUpdated = sv.PointOperation(operation, UpdateHistoryAction.Insert);
				// ポイント操作件数が0件の場合
				if (iUpdated == 0)
				{
					blUpdatePoint = false;
					if (Constants.CROSS_POINT_OPTION_ENABLED == false)
					{
						blUpdateExpDateTime = false;
					}
				}

				// 完了表示用に格納
				alUpdatePoint.Add(searchResult[iLoop].DataSource); // ユーザーポイント情報
				// ポイント更新対象の場合
				if (htUpdatePoint[strUserId + strPointKbn + iPointKbnNo.ToString()] != null)
				{
					htUpdatePoint[strUserId + strPointKbn + iPointKbnNo.ToString()] = blUpdatePoint;				// ポイント更新結果
				}
				// 有効期限更新対象の場合
				if (htUpdateExpDateTime[strUserId + strPointKbn] != null)
				{
					// 有効期限更新結果
					htUpdateExpDateTime[strUserId + strPointKbn + iPointKbnNo.ToString()] = Constants.CROSS_POINT_OPTION_ENABLED
						? false
						: blUpdateExpDateTime;
				}
				blSelected = true;
			}
		}

		// 更新対象が無い場合
		if (blSelected == false)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERPOINT_TARGET_NO_SELECTED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// 更新したユーザーポイント情報を格納
		Hashtable htParam = new Hashtable();
		htParam.Add(RESULT_USERPOINT, alUpdatePoint);						// ユーザーポイント情報
		htParam.Add(RESULT_UPDATE_POINT_RESULT, htUpdatePoint);				// ポイント更新結果
		htParam.Add(RESULT_UPDATE_EXP_DATETIME_RESULT, htUpdateExpDateTime);	// 有効期限数更新結果
		Session[Constants.SESSION_KEY_PARAM] = htParam;

		// ユーザーポイント情報完了ページへ
		Response.Redirect(
			CreateUserPointListUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim(),
			ddlSortKbn.SelectedValue, rblPointKbn.SelectedValue, Constants.KBN_USERPOINT_DISPLAY_COMPLETE, 1));
	}
	#endregion

	#region #btRedirectEditTop_Click 続けて編集を行うボタンクリック
	/// <summary>
	/// 続けて編集を行うボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btRedirectEditTop_Click(object sender, System.EventArgs e)
	{
		// ユーザポイント編集表示
		Response.Redirect(
			CreateUserPointListUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text.Trim(),
			ddlSortKbn.SelectedValue, rblPointKbn.SelectedValue, Constants.KBN_USERPOINT_DISPLAY_EDIT, 1));
	}
	#endregion

	#region #CreateSearchParams ユーザーポイント情報出力用の検索ハッシュテーブル生成
	/// <summary>
	/// ユーザーポイント情報出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>ユーザーポイント情報出力ユーザコントロールのイベントに割り当てて使う</remarks>
	private Hashtable CreateSearchParams()
	{
		var srchKey = StringUtility.ToEmpty((string)Request[Constants.REQUEST_KEY_SEARCH_KEY]) != ""
			? (string)Request[Constants.REQUEST_KEY_SEARCH_KEY]
			: Constants.KBN_SEARCHKEY_USERPOINT_LIST_DEFAULT;

		var sortKbn = StringUtility.ToEmpty((string)Request[Constants.REQUEST_KEY_SORT_KBN]) != ""
			? (string)Request[Constants.REQUEST_KEY_SORT_KBN]
			: Constants.KBN_SORT_USERPOINT_LIST_DEFAULT;

		var pointKbn = StringUtility.ToEmpty((string)Request[Constants.REQUEST_KEY_POINT_KBN]) != ""
			? (string)Request[Constants.REQUEST_KEY_POINT_KBN]
			: Constants.KBN_USERPOINT_LIST_DEFAULT;

		return new Hashtable()
		{
			{"srch_key", srchKey },				// 検索フィールド
			{"srch_word_like_escaped", StringUtility.ToEmpty((string)Request[Constants.REQUEST_KEY_SEARCH_WORD])},	// 検索値
			{"sort_kbn", sortKbn},					// ソート区分
			{"select_point_kbn", pointKbn},		// ポイント区分
		};
	}
	#endregion

	#region 受注詳細URL作成
	/// <summary>
	/// 受注詳細URL作成
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateOrderDetailUrl(string strOrderId)
	{
		return CreateOrderDetailUrl(
			strOrderId,
			true,
			false,
			null);
	}
	#endregion

	#region 検索結果のラッパークラス
	/// <summary>
	/// 検索結果のラッパークラス
	/// </summary>
	[Serializable]
	protected class WrappedSearchResult : UserPointSearchResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result"></param>
		public WrappedSearchResult(UserPointSearchResult result)
			: base(result.DataSource)
		{

		}

		/// <summary>本ポイントがあるかどうか</summary>
		public bool ExistsRealPoint { get { return base.RealPoint != null; } }

		/// <summary>仮ポイントがあるかどうか</summary>
		public bool ExistsTempPoint { get { return base.TempPoint != null; } }
	}
	#endregion
}
