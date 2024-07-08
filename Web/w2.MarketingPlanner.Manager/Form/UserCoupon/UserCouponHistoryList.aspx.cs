/*
=========================================================================================================
  Module      : ユーザクーポン履歴情報一覧ページ処理(UserCouponHistoryList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.User;

public partial class Form_UserCoupon_UserCouponHistoryList : BasePage
{
	#region #Page_Load ページロード
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
			// ユーザ情報取得
			//------------------------------------------------------
			this.UserId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERID]);
			var userModel = new UserService().Get(this.UserId);
			if (userModel != null)
			{
				// 基本情報設定
				lUserKbn.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, userModel.UserKbn));
				lName.Text = WebSanitizer.HtmlEncode(userModel.Name);
			}
			else
			{
				// 該当データ無しエラー
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}

			//------------------------------------------------------
			// パラメタ取得
			//------------------------------------------------------
			string searchKey = Request[Constants.REQUEST_KEY_SEARCH_KEY];
			string searchWord = Request[Constants.REQUEST_KEY_SEARCH_WORD];
			string sortKbn = Request[Constants.REQUEST_KEY_SORT_KBN];

			if ((searchKey == null) || (searchWord == null) || (sortKbn == null))
			{
				// 初期遷移の場合は、フォームのデフォルト値設定
				searchKey = ddlSearchKey.SelectedValue;
				searchWord = tbSearchWord.Text;
				sortKbn = ddlSortKbn.SelectedValue;
			}
			else
			{
				try
				{
					// 初期遷移でない場合は取得したパラメタをフォームへ設定
					foreach (ListItem li in ddlSearchKey.Items)
					{
						li.Selected = (li.Value == searchKey);
					}
					tbSearchWord.Text = searchWord;
					foreach (ListItem li in ddlSortKbn.Items)
					{
						li.Selected = (li.Value == sortKbn);
					}
				}
				catch
				{
					// コントロール設定エラーの場合
				}
			}

			int currentPage = 1;
			try
			{
				currentPage = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO]);
			}
			catch
			{
				// 変換エラーの場合、１ページ目とする
			}

			//------------------------------------------------------
			// ユーザクーポン履歴情報取得
			//------------------------------------------------------
			int totalCount = 0;
			var searchResult = GetUserCouponHistory(currentPage, searchKey, searchWord, sortKbn, this.UserId);
			if (searchResult != null && searchResult.Length > 0)
			{
				totalCount = searchResult[0].RowCount;
				// エラー非表示制御
				trListError.Visible = false;
				rList.DataSource = searchResult;
			}
			else
			{
				// エラー表示制御
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			string nextUrl = CreateSearchUrl(this.UserId, searchKey, searchWord, sortKbn);
			lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, currentPage, nextUrl);

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind();

		}
	}
	#endregion

	#region -GetUserCouponHistory ユーザクーポン履歴情報取得
	/// <summary>
	/// ユーザクーポン履歴情報取得
	/// </summary>
	/// <param name="pageNumber">ページ番号</param>
	/// <param name="searchKey">検索キー</param>
	/// <param name="searchWord">検索文</param>
	/// <param name="sortKbn">ソート区分</param>
	/// <param name="userId">ユーザID</param>
	/// <returns>ユーザクーポン履歴情報</returns>
	private UserCouponHistoryListSearchResult[] GetUserCouponHistory(int pageNumber, string searchKey, string searchWord, string sortKbn, string userId)
	{
		var searchCond = new BaseCouponSearchCondition()
		{
			UserId = userId,
			DeptId = this.LoginOperatorDeptId,
			BgnRowNumber = (pageNumber - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
			EndRowNumber = pageNumber * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
			SearchKey = searchKey,
			SearchWordLikeEscaped = StringUtility.SqlLikeStringSharpEscape(searchWord),
			SortKbn = sortKbn
		};
		return new CouponService().SearchUserCouponHistory(searchCond);
	}
	#endregion
	
	#region #CreateSearchUrl ユーザクーポン情報遷移URL作成
	/// <summary>
	/// ユーザクーポン情報遷移URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <returns>ユーザクーポン情報遷移URL</returns>
	protected string CreateSearchUrl(string userId)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERCOUPONHISTORYLIST);
		url.Append("?").Append(Constants.REQUEST_KEY_USERID).Append("=").Append(HttpUtility.UrlEncode(userId));
		return url.ToString();
	}
	#endregion
	
	#region #CreateSearchUrl ユーザクーポン情報遷移URL作成
	/// <summary>
	/// ユーザクーポン情報遷移URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <param name="searchKey">検索キー</param>
	/// <param name="searchWord">検索文</param>
	/// <param name="sortKbn">ソート区分</param>
	/// <returns>ユーザクーポン履歴情報URL</returns>
	protected string CreateSearchUrl(string userId, string searchKey, string searchWord, string sortKbn)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERCOUPONHISTORYLIST);
		url.Append("?").Append(Constants.REQUEST_KEY_USERID).Append("=").Append(HttpUtility.UrlEncode(userId));
		url.Append("&").Append(Constants.REQUEST_KEY_SEARCH_KEY).Append("=").Append(HttpUtility.UrlEncode(searchKey));
		url.Append("&").Append(Constants.REQUEST_KEY_SEARCH_WORD).Append("=").Append(HttpUtility.UrlEncode(searchWord));
		url.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode(sortKbn));
		return url.ToString();
	}
	#endregion

	#region #CreateUserListUrl ユーザ情報一覧遷移URL作成
	/// <summary>
	/// ユーザ情報一覧遷移URL作成
	/// </summary>
	/// <returns>ユーザ情報一覧遷移URL</returns>
	protected string CreateUserListUrl()
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERLIST);
		// 検索条件が存在する場合
		if (Session[Constants.SESSIONPARAM_KEY_USER_SEARCH_INFO] != null)
		{
			// 一覧検索条件取得
			Hashtable searchParams = (Hashtable)Session[Constants.SESSIONPARAM_KEY_USER_SEARCH_INFO];
			url.Append("?").Append(Constants.REQUEST_KEY_SEARCH_KEY).Append("=").Append(HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_SEARCH_KEY]));
			url.Append("&").Append(Constants.REQUEST_KEY_SEARCH_WORD).Append("=").Append(HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_SEARCH_WORD]));
			url.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_SORT_KBN]));
			url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(searchParams[Constants.REQUEST_KEY_PAGE_NO])));
		}

		return url.ToString();
	}
	#endregion

	#region #CreateCouponDetailUrl クーポン情報詳細遷移URL作成
	/// <summary>
	/// クーポン情報詳細遷移URL作成
	/// </summary>
	/// <param name="couponId">ユーザID</param>
	/// <returns>クーポン情報詳細遷移URL</returns>
	protected string CreateCouponDetailUrl(string couponId)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_COUPON_CONFIRM);
		url.Append("?").Append(Constants.REQUEST_KEY_COUPON_COUPON_ID).Append("=").Append(HttpUtility.UrlEncode(couponId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return url.ToString();
	}
	#endregion

	#region #CreateUserCouponListUrl ユーザクーポン情報一覧遷移URL作成
	/// <summary>
	/// ユーザクーポン情報一覧遷移URL作成
	/// </summary>
	/// <returns>ユーザクーポン情報一覧遷移URL</returns>
	protected string CreateUserCouponListUrl()
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERCOUPONLIST);
		url.Append("?").Append(Constants.REQUEST_KEY_USERID).Append("=").Append(HttpUtility.UrlEncode(this.UserId));

		return url.ToString();
	}
	#endregion

	#region #DisplayCouponPrice クーポン割引額(クーポン金額)取得
	/// <summary>
	/// クーポン割引額(クーポン金額)取得
	/// </summary>
	/// <param name="historyResultItem">ユーザークーポン履歴一覧検索結果情報</param>
	/// <returns>クーポン割引額</returns>
	/// <remarks>
	/// 履歴区分が「10：利用」の場合のみ金額を表示、それ以外は「-」
	/// </remarks>
	protected string DisplayCouponPrice(UserCouponHistoryListSearchResult historyResultItem)
	{
		// クーポン履歴区分が利用の場合
		if (historyResultItem.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE)
		{
			return StringUtility.ToPrice(historyResultItem.CouponPrice);
		}
		return "－";
	}
	#endregion

	#region #DisplayCouponInc クーポン枚数(加算数)取得
	/// <summary>
	/// クーポン枚数(加算数)取得
	/// </summary>
	/// <param name="historyResultItem">ユーザークーポン履歴一覧検索結果情報</param>
	/// <returns>クーポン枚数</returns>
	protected string DisplayCouponInc(UserCouponHistoryListSearchResult historyResultItem)
	{
		string strResult = StringUtility.ToNumeric(historyResultItem.CouponInc);

		// クーポン履歴区分が利用の場合
		if (historyResultItem.CouponInc < 0)
		{
			strResult = "<span class=\"notice\">" + strResult + "</span>";
		}

		return strResult;
	}
	#endregion

	#region #tbSearchWord_TextChanged 検索値テキストエリアでEnterキー押下
	/// <summary>
	/// 検索値テキストエリアでEnterキー押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>検索処理を行う</remarks>
	protected void tbSearchWord_TextChanged(object sender, System.EventArgs e)
	{
		// 検索
		this.btnSearch_Click(sender, e);
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
		Response.Redirect(
			CreateSearchUrl(
					this.UserId,
					ddlSearchKey.SelectedValue,
					tbSearchWord.Text.Trim(),
					ddlSortKbn.SelectedValue
					)
		);
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザID</summary>
	protected string UserId
	{
		get { return (ViewState[Constants.REQUEST_KEY_USERID] != null) ? (string)ViewState[Constants.REQUEST_KEY_USERID] : ""; }
		set { ViewState[Constants.REQUEST_KEY_USERID] = value; }
	}
	#endregion
}
