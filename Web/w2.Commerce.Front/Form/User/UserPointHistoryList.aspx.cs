/*
=========================================================================================================
  Module      : ポイント履歴一覧画面(UserPointHistoryList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using w2.App.Common.CrossPoint.PointHistory;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;

public partial class Form_User_UserPointHistoryList : BasePage
{
	/// <summary>ポイント発効日</summary>
	public const string POINT_CREATE_DATE = "point_date_created";
	/// <summary>ポイント注文ID</summary>
	public const string POINT_ORDER_ID = "order_id";
	/// <summary>ポイント定期購入ID</summary>
	public const string POINT_FIXED_PURCHASE_ID = "point_fixed_purchase_id";
	/// <summary>返品を含むか</summary>
	public const string CONTAINS_RETURN_ORDER = "ContainsReturnOrder";
	/// <summary>交換を含むか</summary>
	public const string CONTAINS_EXCHANGE_ORDER = "ContainsExchangeOrder";
	/// <summary>返品／交換注文を含まない数</summary>
	private const int RETURN_EXCHANGE_ORDER_COUNT = 0;

	#region ラップ済みコントロール宣言
	WrappedRepeater WrList { get { return GetWrappedControl<WrappedRepeater>("rList"); } }
	WrappedHtmlGenericControl WpInfo { get { return GetWrappedControl<WrappedHtmlGenericControl>("pInfo"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			UserUtility.AdjustPointByCrossPointApi(this.LoginUserId);
		}
		this.LoginUserPoint = PointOptionUtility.GetUserPoint(this.LoginUserId);

		// ページング可能項目数
		var iTotalCounts = 0;

		// パラメタ取得
		// カレントページ番号
		var iCurrentPageNumber = 1;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out iCurrentPageNumber) == false)
		{
			iCurrentPageNumber = 1;
		}

		// 一覧取得
		var dvList = GetMyPagePointHistoryList(
			this.LoginUserId,
			Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1,
			Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber);

		// 総件数取得
		iTotalCounts = (dvList.Count != 0) ? int.Parse(dvList[0]["row_count"].ToString()) : 0;
		if (iTotalCounts == 0)
		{
			this.WpInfo.Visible = false;
			this.WrList.Visible = false;
			this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_POINT_HISTORY_UNDISP);
		}

		// ページャ作成（一覧取得処理で総件数を取得している必要あり）
		var strNextUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_USERPOINTHISTORY_LIST;
		this.PagerHtml = WebPager.CreateDefaultListPager(iTotalCounts, iCurrentPageNumber, strNextUrl);

		// データバインド
		this.WrList.DataSource = dvList;
		this.WrList.DataBind();
	}

	/// <summary>
	/// ユーザーポイント履歴一覧取得
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <param name="bgnNum">取得開始番号</param>
	/// <param name="endNum">取得終了番号</param>
	/// <returns>マイページポイント履歴一覧のDataView</returns>
	protected DataView GetMyPagePointHistoryList(string userId, int bgnNum, int endNum)
	{
		// 既存に影響を与えないため、CrossPointOPがONの場合DataViewへマッピングを行う
		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			// CROSS POINT側から全件取得
			var pointHistoriesFromCrossPoint = new CrossPointPointHistoryApiService().GetAllPointHistories(this.LoginUserId);

			// CROSS POINT側で処理が失敗した場合はエラーページへリダイレクト
			if (pointHistoriesFromCrossPoint == null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
					w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// 実店舗（ShopNameが「直営EC」以外）かつ、ポイント数が0でない履歴データのみ抽出
			var realShopPointHistoryItems = pointHistoriesFromCrossPoint
				.Where(item => ((item.ShopName != Constants.CROSS_POINT_EC_SHOP_NAME) && (item.Point != 0)))
				.ToArray();

			// 実店舗側とCrossPoint側の履歴を合わせ、並び変えを行う
			var dv = CrossPointUtility.GetUserPointHistoryList(userId, realShopPointHistoryItems);
			dv.Sort = "point_date_created DESC, history_no DESC";

			// ページングを考慮し、表示分だけ取得
			var result = (dv.Count != 0)
				? dv.ToTable().AsEnumerable().Skip(bgnNum - 1).Take(endNum).CopyToDataTable().AsDataView()
				: new DataView();

			return result;
		}

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("UserPoint", "GetPointHistoryList"))
		{
			var input = new Hashtable()
			{
				{ Constants.FIELD_USERPOINT_USER_ID, userId },
				{ "bgn_row_num", bgnNum },
				{ "end_row_num", endNum }
			};
			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
		}
	}

	/// <summary>
	/// 仮ポイントから本ポイントへの移行日が存在するか
	/// </summary>
	/// <param name="pointHistoryDataItem">ポイント履歴情報</param>
	/// <returns>True:移行日あり,False:移行日なし</returns>
	protected bool ExistTempToUsablePointDate(object pointHistoryDataItem)
	{
		if (Constants.ORDER_POINT_BATCH_CHANGE_TEMP_TO_COMP_ENABLED == false) return false;

		var pointType = (string)GetKeyValue(pointHistoryDataItem, Constants.FIELD_USERPOINT_POINT_TYPE);
		if (pointType != Constants.FLG_USERPOINT_POINT_TYPE_TEMP) return false;

		var strDate = StringUtility.ToNull(GetKeyValue(pointHistoryDataItem, Constants.FIELD_ORDER_ORDER_SHIPPED_DATE));
		if (string.IsNullOrEmpty(strDate)) return false;

		return true;
	}

	/// <summary>
	/// 仮ポイントから本ポイントへの移行日付取得
	/// </summary>
	/// <param name="pointHistoryDataItem">ポイント履歴情報</param>
	/// <returns>仮ポイントから本ポイントへの移行日付</returns>
	protected DateTime? GetTempToUsablePointDate(object pointHistoryDataItem)
	{
		var shippedDate = StringUtility.ToNull(GetKeyValue(pointHistoryDataItem, Constants.FIELD_ORDER_ORDER_SHIPPED_DATE));
		if (string.IsNullOrEmpty(shippedDate)) return null;

		return DateTime.Parse(shippedDate)
			.AddDays(
				(string)GetKeyValue(pointHistoryDataItem, Constants.FIELD_USERPOINT_POINT_KBN) == Constants.FLG_USERPOINT_POINT_KBN_BASE
					? Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_DAYS
					: Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_LIMITED_TERM_POINT_DAYS);
	}

	/// <summary>
	/// 仮ポイントから本ポイントへの以降日数取得
	/// </summary>
	/// <param name="pointHistoryDataItem">ポイント履歴情報</param>
	/// <returns>仮ポイントから本ポイントへの移行日付</returns>
	protected int GetTempToCompDays(object pointHistoryDataItem)
	{
		switch ((string)GetKeyValue(pointHistoryDataItem, Constants.FIELD_USERPOINT_POINT_KBN))
		{
			case Constants.FLG_USERPOINT_POINT_KBN_BASE:
				return Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_DAYS;

			case Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT:
				return Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_LIMITED_TERM_POINT_DAYS;

			default: //ここに来るパターンはおかしいけど
				return 0;
		}
	}

	/// <summary>
	/// 返品交換の文言出力
	/// </summary>
	/// <param name="pointHistoryDataItem">ポイント履歴情報</param>
	/// <param name="returnMessage">返品時の文言</param>
	/// <param name="exchangeMessage">交換時の文言</param>
	/// <param name="bothMessage">返品・交換どちらも含む場合の文言</param>
	/// <returns>返品か交換の文言</returns>
	protected string GetReturnExchangeMessage(
		object pointHistoryDataItem,
		string returnMessage,
		string exchangeMessage,
		string bothMessage)
	{
		var containsReturn = (GetKeyValue(pointHistoryDataItem, (Constants.CROSS_POINT_OPTION_ENABLED) ? "return_order_count" : CONTAINS_RETURN_ORDER).ToString() != "0");
		var containsExchange = (GetKeyValue(pointHistoryDataItem, (Constants.CROSS_POINT_OPTION_ENABLED) ? "exchange_order_count" : CONTAINS_EXCHANGE_ORDER).ToString() != "0");

		if (containsReturn && containsExchange)
		{
			return bothMessage;
		}
		else if (containsReturn)
		{
			return returnMessage;
		}
		else if (containsExchange)
		{
			return exchangeMessage;
		}
		else
		{
			return string.Empty;
		}
	}

	/// <summary>
	/// ポイント変更内容を取得
	/// </summary>
	/// <param name="pointHistoryDataItem">ポイント履歴情報</param>
	/// <returns>ポイント変更内容</returns>
	protected string GetPointIncText(object pointHistoryDataItem)
	{
		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			var shopName = (string)GetKeyValue(pointHistoryDataItem, Constants.CROSS_POINT_SETTING_SHOP_NAME);
			if (shopName != Constants.CROSS_POINT_EC_SHOP_NAME)
			{
				var pointChangeReason = (string)GetKeyValue(pointHistoryDataItem, Constants.CROSS_POINT_SETTING_POINT_CHANGE_REASON);
				return pointChangeReason;
			}
		}

		var pointIncKbn = (string)GetKeyValue(pointHistoryDataItem, Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN);
		var pointIncText = ValueText.GetValueText(
			Constants.TABLE_USERPOINTHISTORY,
			Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN,
			pointIncKbn);
		return pointIncText;
	}

	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示
	/// <summary>ページャーHTML</summary>
	protected string PagerHtml
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>アラートメッセージ</summary>
	protected string AlertMessage
	{
		get { return (string)ViewState["AlertMessage"]; }
		private set { ViewState["AlertMessage"] = value; }
	}
}
