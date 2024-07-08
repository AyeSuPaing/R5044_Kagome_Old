/*
=========================================================================================================
  Module      : ユーザ情報一覧処理(UserList.aspx.cs)
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

public partial class Form_UserCoupon_UserList : BasePage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 検索ドロップダウンリスト制御
			// ユーザ：「法人（企業名、部署名）」利用設定で切り替える
			//------------------------------------------------------
			if (Constants.DISPLAY_CORPORATION_ENABLED)
			{
				// 検索ドロップダウン作成("企業名", "部署名"あり)
				foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USERCOUPON, "search_key"))
				{
					ddlSearchKey.Items.Add(li);
				}
			}
			else
			{
				// 検索ドロップダウン作成("企業名", "部署名"除外)
				foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USERCOUPON, "search_key"))
				{
					if ((li.Value != Constants.KBN_SEARCHKEY_USERCOUPON_LIST_COMPANY_NAME) && (li.Value != Constants.KBN_SEARCHKEY_USERCOUPON_LIST_COMPANY_POST_NAME))
					{
						ddlSearchKey.Items.Add(li);
					}
				}
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

			if (this.IsNotSearchDefault) return;

			//------------------------------------------------------
			// 検索情報保持(その他ページの「一覧へ戻る」ボタン実行時に利用)
			//------------------------------------------------------
			Hashtable searchParams = new Hashtable();
			searchParams.Add(Constants.REQUEST_KEY_SEARCH_KEY, searchKey);
			searchParams.Add(Constants.REQUEST_KEY_SEARCH_WORD, searchWord);
			searchParams.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);
			searchParams.Add(Constants.REQUEST_KEY_PAGE_NO, currentPage);
			Session[Constants.SESSIONPARAM_KEY_USER_SEARCH_INFO] = searchParams;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			var searchCond = new BaseCouponSearchCondition()
			{
				BgnRowNumber = (currentPage - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
				PageSize = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
				SearchKey = searchKey,
				SearchWordLikeEscaped = StringUtility.SqlLikeStringSharpEscape(searchWord),
				SortKbn = sortKbn,
				DeptId = this.LoginOperatorDeptId,
				UsedUserJudgeType = Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
			};

			//------------------------------------------------------
			// ユーザ一覧（該当件数）
			//------------------------------------------------------
			var couponService = new CouponService();

			// ページング可能総商品数
			var totalUserCounts = couponService.GetCountOfUserListSearch(searchCond);

			//------------------------------------------------------
			// エラー表示制御
			//------------------------------------------------------
			bool displayPagerFlg = true;
			StringBuilder errorMessageBuilder = new StringBuilder();
			// 上限件数より多い？
			if (totalUserCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				errorMessageBuilder.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				errorMessageBuilder.Replace("@@ 1 @@", StringUtility.ToNumeric(totalUserCounts));
				errorMessageBuilder.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				displayPagerFlg = false;
			}
			// 該当件数なし？
			else if (totalUserCounts == 0)
			{
				errorMessageBuilder.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdErrorMessage.InnerHtml = errorMessageBuilder.ToString();
			trListError.Visible = (errorMessageBuilder.ToString().Length != 0);

			//------------------------------------------------------
			// ユーザ一覧情報表示
			//------------------------------------------------------
			if (trListError.Visible == false)
			{
				// ユーザ一覧情報取得
				var userInfo = couponService.SearchUserList(searchCond);

				// データバインド
				rList.DataSource = userInfo;
				rList.DataBind();
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (displayPagerFlg)
			{
				string strNextUrl = CreateSearchUrl(searchKey, searchWord, sortKbn);
				lbPager1.Text = WebPager.CreateDefaultListPager(totalUserCounts, currentPage, strNextUrl);
			}

			//------------------------------------------------------
			// 元ページ情報をセッション格納
			//------------------------------------------------------
			Session[Constants.SESSION_KEY_ORIGIN_PAGE] = Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERLIST;
		}
	}
	#endregion

	#region -CreateSearchUrl ユーザ情報一覧遷移URL作成
	/// <summary>
	/// ユーザ情報一覧遷移URL作成
	/// </summary>
	/// <param name="searchKey">検索キー</param>
	/// <param name="searchWord">検索値</param>
	/// <param name="sortKbn">ソート区分</param>
	/// <returns>ユーザ一覧遷移URL</returns>
	private string CreateSearchUrl(string searchKey, string searchWord, string sortKbn)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERLIST);
		url.Append("?").Append(Constants.REQUEST_KEY_SEARCH_KEY).Append("=").Append(HttpUtility.UrlEncode(searchKey));
		url.Append("&").Append(Constants.REQUEST_KEY_SEARCH_WORD).Append("=").Append(HttpUtility.UrlEncode(searchWord));
		url.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode(sortKbn));

		return url.ToString();
	}
	#endregion

	#region #CreateUserCouponListUrl ユーザクーポン情報一覧遷移URL作成
	/// <summary>
	/// ユーザクーポン情報一覧遷移URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <returns>ユーザクーポン情報一覧遷移URL</returns>
	protected string CreateUserCouponListUrl(string userId)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERCOUPONLIST);
		url.Append("?").Append(Constants.REQUEST_KEY_USERID).Append("=").Append(HttpUtility.UrlEncode(userId));

		return url.ToString();
	}
	#endregion

	#region #CreateUserCouponHistoryListUrl ユーザクーポン履歴情報一覧遷移URL作成
	/// <summary>
	/// ユーザクーポン履歴情報一覧遷移URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <returns>ユーザクーポン履歴情報一覧遷移URL</returns>
	protected string CreateUserCouponHistoryListUrl(string userId)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERCOUPON_USERCOUPONHISTORYLIST);
		url.Append("?").Append(Constants.REQUEST_KEY_USERID).Append("=").Append(HttpUtility.UrlEncode(userId));

		return url.ToString();
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
					ddlSearchKey.SelectedValue,
					tbSearchWord.Text.Trim(),
					ddlSortKbn.SelectedValue
					)
		);
	}
	#endregion

	#region #CreateSearchParams ユーザクーポンマスタ出力用の検索ハッシュテーブル生成
	/// <summary>
	/// ユーザクーポンマスタ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>ユーザクーポンマスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	private Hashtable CreateSearchParams()
	{
		var searchCond = new BaseCouponSearchCondition()
		{
			SearchKey = Request[Constants.REQUEST_KEY_SEARCH_KEY],
			SearchWordLikeEscaped = StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_SEARCH_WORD]),
			SortKbn = Request[Constants.REQUEST_KEY_SORT_KBN],
			DeptId = this.LoginOperatorDeptId
		};
		return searchCond.CreateHashtableParams();
	}
	#endregion
}
