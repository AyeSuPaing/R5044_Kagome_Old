/*
=========================================================================================================
  Module      : ユーザポイント履歴一覧処理(UserPointHistoryList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using w2.App.Common.User;
using w2.Common.Web;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.User;

public partial class Form_UserPoint_UserPointHistoryList : BasePage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// ユーザーポイント履歴情報一覧表示
			ViewUserPointHistoryList();
		}
	}
	#endregion

	#region -ViewUserPointHistoryList ユーザーポイント履歴情報一覧表示(DataGridにDataView(ユーザーポイント履歴情報)を設定)
	/// <summary>
	/// ユーザーポイント履歴情報一覧表示(DataGridにDataView(ユーザーポイント履歴情報)を設定)
	/// </summary>
	private void ViewUserPointHistoryList()
	{
		// 変数宣言
		Hashtable htParam = new Hashtable();
		//------------------------------------------------------
		// リクエスト情報取得
		//------------------------------------------------------
		htParam = GetParameters(Request);
		// 不正パラメータが存在した場合
		if ((bool)htParam[Constants.ERROR_REQUEST_PRAMETER])
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------	
		var currentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];
		var userId = Request[Constants.REQUEST_KEY_USERID];

		// ユーザー情報取得
		var user = new UserService().Get(userId);
		if (user == null)
		{
			// 該当無しの場合、エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}
		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			// Adjust point and member rank by Cross Point api
			UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
		}

		//------------------------------------------------------
		// ユーザーポイント情報取得
		//------------------------------------------------------
		// ユーザーポイント情報取得
		var pointService = new PointService();
		var userPoints = pointService.GetUserPoint(userId, string.Empty);

		// 該当データが有りの場合
		if (userPoints.Any())
		{
			// 通常本ポイント、通常仮ポイント、期間限定本ポイント、期間限定仮ポイントの順番
			// 同区分、同種別は枝番でソート
			rUserPointList.DataSource = userPoints
				.OrderBy(p => p.PointKbn)
				.ThenByDescending(p => p.PointType)
				.ThenBy(p => p.PointKbnNo)
				.ToArray();
		}
		else
		{
			trUserPointDetail.Visible = false;
		}

		var firstUsablePoint = (userPoints != null)
			? userPoints.FirstOrDefault(item => item.IsUsableForOrder)
			: null;
		this.Data = new SummaryForDisplay
		{
			UserId = userId,
			UserName = user.Name,
			PointTotal = (userPoints != null) ? userPoints.Sum(p => p.Point) : 0m,
			TempPointTotal = (userPoints != null)
				? userPoints
					.Where(p => p.IsPointTypeTemp)
					.Sum(p => p.Point)
				: 0m,
			PointUsableForOrder = (userPoints != null)
				? userPoints
					.Where(p => p.IsUsableForOrder)
					.Sum(p => p.Point)
				: 0m,
			PointExp = (firstUsablePoint != null) ? firstUsablePoint.PointExp : null
		};

		//------------------------------------------------------
		// ユーザーポイント履歴取得
		//------------------------------------------------------
		// ユーザーポイント履歴検索
		var condition = new UserPointHistorySummarySearchCondition
		{
			UserId = userId,
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (currentPageNumber - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * currentPageNumber
		};

		// 履歴検索数取得
		var pointHistoriesCount = pointService.GetSearchHitCountUserPointHistorySummary(condition);

		// 履歴検索
		var pointHistories = pointService.SearchUserPointHistorySummary(condition).ToList();
		if (pointHistories.Count > 0)
		{
			trListError.Visible = false;
			rList.DataSource = pointHistories.Select(r => new PointHistoryForDisplay(r));
		}
		else
		{
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		var nextUrl = CreateUserPointHistoryListUrl(userId);
		lbPager1.Text = WebPager.CreateDefaultListPager(pointHistoriesCount, currentPageNumber, nextUrl);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
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
		bool blParamError = false;

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
	#endregion

	#region -CreateUserPointHistoryListUrl ユーザポイント履歴一覧遷移URL作成
	/// <summary>
	/// ユーザポイント履歴一覧遷移URL作成
	/// </summary>
	/// <param name="strUserId">ルールID</param>
	/// <returns>ユーザポイント履歴一覧遷移URL</returns>
	private string CreateUserPointHistoryListUrl(string strUserId)
	{
		var url = new UrlCreator(Path.Combine(Constants.PATH_ROOT, Constants.PAGE_W2MP_MANAGER_USERPOINTHISTORY_LIST))
			.AddParam(Constants.REQUEST_KEY_USERID, strUserId)
			.CreateUrl();

		return url;
	}
	#endregion

	#region #CreateUserPointListUrl ユーザポイント情報一覧遷移URL作成
	/// <summary>
	/// ユーザポイント情報一覧遷移URL作成
	/// </summary>
	/// <returns>ユーザポイント情報一覧遷移URL</returns>
	protected string CreateUserPointListUrl()
	{
		System.Text.StringBuilder sbResult = new System.Text.StringBuilder();

		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_USERPOINT_LIST);
		// 検索条件が存在する場合
		if (Session[Constants.SESSIONPARAM_KEY_USERPOINT_SEARCH_INFO] != null)
		{
			// 一覧検索条件取得
			Hashtable htSearch = (Hashtable)Session[Constants.SESSIONPARAM_KEY_USERPOINT_SEARCH_INFO];
			sbResult.Append("?");
			sbResult.Append(Constants.REQUEST_KEY_SEARCH_KEY).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_SEARCH_KEY]));
			sbResult.Append("&");
			sbResult.Append(Constants.REQUEST_KEY_SEARCH_WORD).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_SEARCH_WORD]));
			sbResult.Append("&");
			sbResult.Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_SORT_KBN]));
			sbResult.Append("&");
			sbResult.Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_PAGE_NO])));
		}

		return sbResult.ToString();
	}
	#endregion

	/// <summary>
	/// 履歴詳細URL作成
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="historyGroupNo">履歴グループ番号</param>
	/// <returns>履歴詳細URL</returns>
	protected string CreatePointHistoryDetailUrl(string userId, string historyGroupNo)
	{
		var url = new UrlCreator(Path.Combine(Constants.PATH_ROOT, Constants.PAGE_W2MP_MANAGER_USERPOINTHISTORY_DETAIL))
			.AddParam(Constants.REQUEST_KEY_USERID, userId)
			.AddParam(Constants.REQUEST_KEY_USERPOINTHISTORY_HISTORY_GROUP_NO, historyGroupNo)
			.CreateUrl();

		return url;
	}

	/// <summary>画面表示データ（基本情報）</summary>
	protected SummaryForDisplay Data { get; set; }

	#region 画面表示用のクラス
	/// <summary>
	/// 画面表示用のクラス（基本情報）
	/// </summary>
	[Serializable]
	protected class SummaryForDisplay
	{
		/// <summary>ユーザーID</summary>
		public string UserId { get; set; }
		/// <summary>ユーザー名</summary>
		public string UserName { get; set; }
		/// <summary>保有ポイント合計</summary>
		public decimal PointTotal { get; set; }
		/// <summary>仮ポイント合計</summary>
		public decimal TempPointTotal { get; set; }
		/// <summary>注文利用可能ポイント合計</summary>
		public decimal PointUsableForOrder { get; set; }
		/// <summary>保有ポイント有効期限</summary>
		public DateTime? PointExp { get; set; }
	}

	/// <summary>
	/// 画面表示用のクラス（履歴一覧）
	/// </summary>
	[Serializable]
	protected class PointHistoryForDisplay
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="searchResult">検索結果</param>
		public PointHistoryForDisplay(UserPointHistorySummarySearchResult searchResult)
		{
			this.RowNumber = searchResult.RowNumber.ToString();
			this.UserId = searchResult.UserId;
			this.DateCreated = DateTimeUtility.ToStringForManager(
				searchResult.DateCreated,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			this.HistoryGroupNo = searchResult.HistoryGroupNo.ToString();
			this.PointIncKbn = string.Format(
				"{0} {1}",
				searchResult.ShopName,
				ValueText.GetValueText(
				Constants.TABLE_USERPOINTHISTORY,
				Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN,
				searchResult.PointIncKbn)).Trim();
			this.PointType = ValueText.GetValueText(
				Constants.TABLE_USERPOINTHISTORY,
				Constants.FIELD_USERPOINTHISTORY_POINT_TYPE,
				searchResult.PointType);
			this.PointTotal = StringUtility.ToNumericWithPlusSign(searchResult.PointTotal);
			this.OrderId = searchResult.OrderId;
			this.FixedPurchaseId = searchResult.FixedPurchaseId;
		}

		/// <summary>履歴番号</summary>
		public string RowNumber { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>ユーザーID</summary>
		public string UserId { get; set; }
		/// <summary>履歴グループ番号</summary>
		public string HistoryGroupNo { get; set; }
		/// <summary>ポイント加算区分</summary>
		public string PointIncKbn { get; set; }
		/// <summary>ポイント種別</summary>
		public string PointType { get; set; }
		/// <summary>ポイント数</summary>
		public string PointTotal { get; set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId { get; set; }
	}
	#endregion
}