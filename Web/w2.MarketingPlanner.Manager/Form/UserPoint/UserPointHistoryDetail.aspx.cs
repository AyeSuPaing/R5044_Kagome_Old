/*
=========================================================================================================
 Module      : ユーザポイント履歴詳細処理(UserPointHistoryDetail.aspx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Linq;
using w2.App.Common.User;
using w2.Domain.Point;
using w2.Domain.User;

/// <summary>
/// ユーザーポイント詳細ページ
/// </summary>
public partial class Form_UserPoint_UserPointHistoryDetail : BasePage
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
			// パラメータチェック
			if (IsValidParameters() == false)
			{
				RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR));
			}

			// データ取得 存在チェック
			var histories = new PointService().GetHistoriesByGroupNo(this.UserId, int.Parse(this.HistoryGroupNo));
			var user = new UserService().Get(this.UserId);
			if ((histories.Any() == false) || (user == null))
			{
				RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL));
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				// Adjust point and member rank by Cross Point api
				UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
			}

			// 表示用クラスに変換してデータバインド
			this.Summary = new SummaryForDisplay(histories, user);
			rHistoryList.DataSource = histories.Select(h => new ListItemForDisplay(h));
			DataBind();
		}
	}

	/// <summary>
	/// パラメータ検証
	/// </summary>
	/// <returns>正当なパラメータであればTRUE</returns>
	private bool IsValidParameters()
	{
		var result = true;
		result &= (Validator.IsNullEmpty(this.UserId) == false);
		result &= (Validator.IsNullEmpty(this.HistoryGroupNo) == false);
		result &= Validator.IsHalfwidthNumber(this.HistoryGroupNo);
		result &= Validator.IsHalfwidthAlphNum(this.UserId);

		return result;
	}

	/// <summary>
	/// エラーページ遷移
	/// </summary>
	/// <param name="message">エラーメッセージ</param>
	private void RedirectToErrorPage(string message)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = message;
		Response.Redirect(Path.Combine(Constants.PATH_ROOT, Constants.PAGE_W2MP_MANAGER_ERROR));
	}

	/// <summary>ユーザーID</summary>
	protected string UserId
	{
		get { return Request[Constants.REQUEST_KEY_USERID]; }
	}
	/// <summary>履歴グループ番号</summary>
	protected string HistoryGroupNo
	{
		get { return Request[Constants.REQUEST_KEY_USERPOINTHISTORY_HISTORY_GROUP_NO]; }
	}
	/// <summary>サマリ表示データ</summary>
	protected SummaryForDisplay Summary
	{
		get { return (SummaryForDisplay)ViewState["Summary"]; }
		set { ViewState["Summary"] = value; }
	}

	/// <summary>
	/// サマリ表示データ
	/// </summary>
	[Serializable]
	protected class SummaryForDisplay
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="histories">ユーザーポイント履歴配列</param>
		/// <param name="user">ユーザー</param>
		public SummaryForDisplay(UserPointHistoryModel[] histories, UserModel user)
		{
			this.UserName = user.Name;
			this.PointIncKbn = ValueText.GetValueText(
				Constants.TABLE_USERPOINTHISTORY,
				Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN,
				histories.First().PointIncKbn);
			this.PointTotal = StringUtility.ToNumericWithPlusSign(histories.Sum(r => r.PointInc));
			this.OrderId = histories.First().OrderId;
			this.FixedPurchaseId = histories.First().FixedPurchaseId;
			this.DateCreated = DateTimeUtility.ToStringForManager(
				histories.Max(r => r.DateCreated),
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			this.LastChanged = histories.First().LastChanged;
		}

		/// <summary>ユーザー名</summary>
		public string UserName { get; set; }
		/// <summary>ポイント数</summary>
		public string PointTotal { get; set; }
		/// <summary>ポイント加算区分</summary>
		public string PointIncKbn { get; set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新者</summary>
		public string LastChanged { get; set; }
	}

	/// <summary>
	/// 一覧表示用クラス
	/// </summary>
	[Serializable]
	protected class ListItemForDisplay
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="history">ユーザーポイント履歴</param>
		public ListItemForDisplay(UserPointHistoryModel history)
		{
			this.PointKbn = ValueText.GetValueText(
				Constants.TABLE_USERPOINTHISTORY,
				Constants.FIELD_USERPOINTHISTORY_POINT_KBN,
				history.PointKbn);
			this.PointRuleKbn = ValueText.GetValueText(
				Constants.TABLE_USERPOINTHISTORY,
				Constants.FIELD_USERPOINTHISTORY_POINT_RULE_KBN,
				history.PointRuleKbn);
			this.PointInc = StringUtility.ToNumericWithPlusSign(history.PointInc);
			this.PointExpExtendSign = history.PointExpExtendSign;
			this.PointExpExtendYear = history.PointExpExtendYear.ToString();
			this.PointExpExtendMonth = history.PointExpExtendMonth.ToString();
			this.EffectiveDate = DateTimeUtility.ToStringForManager(
				history.EffectiveDate,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
				null);
			this.PointExp = DateTimeUtility.ToStringForManager(
				history.UserPointExp,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
				null);

			if (string.IsNullOrEmpty(this.PointRuleKbn)) this.PointRuleKbn = null;

			// フラグ
			this.ShouldShowPointExpExtend = (string.IsNullOrEmpty(history.PointExpExtend) == false);
			this.ShouldShowPointExpExtendYear = (history.PointExpExtendYear >= 1);
		}

		/// <summary>ポイント区分</summary>
		public string PointKbn { get; set; }
		/// <summary>ポイントルール区分</summary>
		public string PointRuleKbn { get; set; }
		/// <summary>ポイント加算区分</summary>
		public string PointInc { get; set; }
		/// <summary>期限延長(記号)</summary>
		public string PointExpExtendSign { get; set; }
		/// <summary>期限延長(年)</summary>
		public string PointExpExtendYear { get; set; }
		/// <summary>期限延長(日)</summary>
		public string PointExpExtendMonth { get; set; }
		/// <summary>ポイント有効期限</summary>
		public string PointExp { get; set; }
		/// <summary>利用可能開始日</summary>
		public string EffectiveDate { get; set; }
		/// <summary>期限延長の表示要否</summary>
		public bool ShouldShowPointExpExtend { get; set; }
		/// <summary>期限延長（年）の表示要否</summary>
		public bool ShouldShowPointExpExtendYear { get; set; }
	}
}