/*
=========================================================================================================
  Module      : CS顧客検索画面処理(UserSearch.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.Domain.ExternalLink;
using w2.App.Common.Cs.Message;
using w2.App.Common.Cs.UserHistory;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Manager;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.MailSendLog;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.Domain.DmShippingHistory;
using w2.Domain.SubscriptionBox;
using w2.Common.Web;
using System.Net;

public partial class Form_Message_UserSearch : BasePageCs
{
	#region 定数・列挙体
	/// <summary>顧客一覧表示件数</summary>
	private const int USER_LIST_COUNT_MAX = 100;

	/// <summary>ユーザー履歴モード種別</summary>
	protected enum UserHistoryModeType
	{
		/// <summary>全て</summary>
		All,
		/// <summary>注文</summary>
		Order,
		/// <summary>定期情報</summary>
		FixedPurchase,
		/// <summary>メッセージ</summary>
		Message,
		/// <summary>メール送信ログ</summary>
		MailSendLog,
		/// <summary>DM発送履歴</summary>
		DmShippingHistory,
		/// <summary>定期情報(頒布会)</summary>
		SubscriptionBox
	}

	/// <summary>ユーザーサブ情報表示モード種別</summary>
	protected enum UserSubInfoDispModeType
	{
		/// <summary>ユーザーリレーション</summary>
		UserRelation,
		/// <summary>ユーザー拡張項目</summary>
		UserExtends,
		/// <summary>ユーザー属性</summary>
		UserAttribute,
	}
	#endregion

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
			// パラメタがあれば検索実行
			if ((string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USER_ID])) == false
				|| (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_CUSTOMER_TELNO]) == false))
			{
				tbSearchUserId.Text = Request[Constants.REQUEST_KEY_USER_ID];
				tbSearchTel.Text = Request[Constants.REQUEST_KEY_CUSTOMER_TELNO];
				btnSearchUser_Click(sender, e);
			}
		}
	}
	#endregion

	#region #btnSearchUser_Click 顧客検索ボタンクリック
	/// <summary>
	/// 顧客検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearchUser_Click(object sender, EventArgs e)
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_USER_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchUserName.Text.Trim()));
		input.Add(Constants.FIELD_USER_TEL1 + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchTel.Text.Trim()));
		input.Add(Constants.FIELD_USER_MAIL_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchMailAddr.Text.Trim()));
		input.Add(Constants.FIELD_USER_USER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchUserId.Text.Trim()));
		input.Add(Constants.FIELD_ORDER_ORDER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchOrderId.Text.Trim()));

		// ユーザー件数取得・表示
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("CsUserSearch", "SearchUserCount"))
		{
			this.SearchCount = (int)sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input)[0][0];
		}
		lUserSearchCount.Text = string.Format(
			ReplaceTag("@@DispText.common_message.unit_of_quantity@@"),
			StringUtility.ToNumeric(this.SearchCount));
		if (this.SearchCount > USER_LIST_COUNT_MAX)
		{
			lUserSearchCount.Text += WebMessages.GetMessages(WebMessages.MSG_MANAGER_USER_LIST_COUNT_MAX_DISPLAYED)
				.Replace("@@ 1 @@", USER_LIST_COUNT_MAX.ToString());
		}


		// ユーザー一覧表示
		string userId;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("CsUserSearch", "SearchUser"))
		{
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ count_max @@", USER_LIST_COUNT_MAX.ToString());

			var users = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			userId = (users.Count == 1) ? (string)users[0][Constants.FIELD_USER_USER_ID] : string.Empty;

			// User search list
			this.UserSearch = users.Table;
			foreach (DataRow row in this.UserSearch.Rows)
			{
				row[Constants.FIELD_USER_USER_KBN] = ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, row[Constants.FIELD_USER_USER_KBN]);
			}

			rUserSearchResult.DataSource = this.UserSearch;
		}
		rUserSearchResult.DataBind();

		// 一件ヒットの場合は先頭選択
		if (this.SearchCount == 1)
		{
			// 選択
			ScriptManager.RegisterStartupScript(this, this.GetType(), "MyAction", "select_user_list_first('" + userId + "');", true);
		}

		hfSelectedUserId.Value = "";
		divUserList.Visible = true;
		divUserDetail.Visible = false;
		divUserHistory.Visible = false;

		// Set default sort
		SetDefaultSort();
	}
	#endregion

	#region #lbSelectUser_Click ユーザー選択リンククリック
	/// <summary>
	/// ユーザー選択リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSelectUser_Click(object sender, EventArgs e)
	{
		// 選択ユーザーIDが取れない場合は顧客情報欄はクリア
		if (string.IsNullOrEmpty(hfSelectedUserId.Value))
		{
			divUserDetail.Visible = false;
			divUserHistory.Visible = false;
			return;
		}

		var user = new UserService().Get(hfSelectedUserId.Value);

		// 基本情報
		lUserId1.Text =
			lUserId2.Text = WebSanitizer.HtmlEncode(user.UserId);
		lSiteName.Text = WebSanitizer.HtmlEncode(CreateSiteNameForDetail(user.MallId, user.GetMallName()));
		lUserKbn.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, user.UserKbn));
		lMemberRank.Text = WebSanitizer.HtmlEncode(MemberRankOptionUtility.GetMemberRankName(user.MemberRankId));
		lUserNameKana.Text = WebSanitizer.HtmlEncode(user.NameKana);
		lUserName.Text = WebSanitizer.HtmlEncode(user.Name);
		if (user.DelFlg == Constants.FLG_USER_DELFLG_UNDELETED)
		{
			var zipcodeUtil = new ZipcodeSearchUtility(user.Zip1 + user.Zip2);
			lUserAddrKana.Text = WebSanitizer.HtmlEncode(zipcodeUtil.PrefectureKana + " " + zipcodeUtil.CityKana + " " + zipcodeUtil.TownKana);
		}
		else
		{
			lUserAddrKana.Text = "";
		}
		lUserAddr.Text = WebSanitizer.HtmlEncode(user.Addr);
		lUserMails.Text = WebSanitizer.HtmlEncodeChangeToBr(CombineWithNewLine(user.MailAddr, user.MailAddr2));
		ucErrorPoint.SetMailAddr(user.MailAddr);
		lUserTel.Text = string.Format("{0}<br/>{1}", WebSanitizer.HtmlEncode(user.Tel1), WebSanitizer.HtmlEncode(user.Tel2));
		lUserMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(user.UserMemo);
		lUserManagementLevel.Text = WebSanitizer.HtmlEncode(UserManagementLevelUtility.GetUserManagementLevelName(user.UserManagementLevelId));
		if (user.Birth != null)
		{
			int age = DateTime.Today.Year - user.Birth.Value.Year;
			if ((DateTime.Today.Month * 100 + DateTime.Today.Day) < (user.Birth.Value.Month * 100 + user.Birth.Value.Day)) age--; // 誕生日以前であれば1マイナス
			lUserBirth.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					user.Birth,
					DateTimeUtility.FormatType.ShortDate2Letter));
			lUserAge.Text = string.Format(
				string.Format("({0})", ReplaceTag("@@DispText.common_message.unit_of_age@@")),
				age);
		}
		else
		{
			lUserBirth.Text = "";
			lUserAge.Text = "-";
		}

		// Get list of payment that user level not use
		var payments = UserManagementLevelUtility.GetPaymentsUserManagementLevelNotUse(this.LoginOperatorShopId, user.UserManagementLevelId);
		lPaymentUserManagementLevelNotUse.Text = (UserService.IsUser(user.UserKbn) && payments.Length != 0) ? string.Join(", ", payments.Select(m => m.PaymentName)) : "";

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			lAccessCountryIsoCode.Text = user.AccessCountryIsoCode;
			lDispLanguageCode.Text = user.DispLanguageCode;
			lDispLanguageLocalId.Text = GlobalConfigUtil.LanguageLocaleIdDisplayFormat(user.DispLanguageLocaleId);
			lDispCurrencyCode.Text = user.DispCurrencyCode;
			lDispCurrencyLocalId.Text = GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(user.DispCurrencyLocaleId);
		}

		// ユーザーリレーション情報
		lUserDateCreated.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				user.DateCreated,
				DateTimeUtility.FormatType.ShortDateHourMinute2Letter));
		lUserAdvCodeFirst.Text = WebSanitizer.HtmlEncode(user.AdvcodeFirst);
		lUserMailFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, user.MailFlg));
		// ユーザー注文情報
		var userOrder = new UserOrderInfo(hfSelectedUserId.Value);
		lUserFirstOrderDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				userOrder.FirstOrderDate,
				DateTimeUtility.FormatType.ShortDate2Letter,
				"-"));
		lUserLastOrderDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				userOrder.LastOrderDate,
				DateTimeUtility.FormatType.ShortDate2Letter,
				"-"));
		lUserLastOrderPrice.Text = WebSanitizer.HtmlEncode(userOrder.LastOrderPrice.HasValue ? userOrder.LastOrderPrice.Value.ToPriceString(true) : "-");
		lUserOrderCount.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(userOrder.OrderCount));
		lUserOrderPriceYearTotal.Text = WebSanitizer.HtmlEncode(userOrder.OrderPriceYearTotal.ToPriceString(true));
		if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
		{
			var userFixedPurchase = new UserOrderFixedPurshaseInfo(hfSelectedUserId.Value);
			lUserFirstOrderDateFixedPurchase.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					userFixedPurchase.FirstOrderDate,
					DateTimeUtility.FormatType.ShortDate2Letter,
					"-"));
			lUserLastOrderDateFixedPurchase.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					userFixedPurchase.LastOrderDate,
					DateTimeUtility.FormatType.ShortDate2Letter,
					"-"));
			lUserLastOrderPriceFixedPurchase.Text = WebSanitizer.HtmlEncode(userFixedPurchase.LastOrderPrice.HasValue ? userFixedPurchase.LastOrderPrice.Value.ToPriceString(true) : "-");
			lUserOrderCountFixedPurchase.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(userFixedPurchase.OrderCount));
			lUserOrderPriceYearTotalFixedPurchase.Text = WebSanitizer.HtmlEncode(userFixedPurchase.OrderPriceYearTotal.ToPriceString(true));
		}

		lUserWithdrawaledDate.Text = WebSanitizer.HtmlEncode(
			(user.DelFlg == Constants.FLG_USER_DELFLG_DELETED)
				? DateTimeUtility.ToStringForManager(
					user.DateChanged,
					DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
					"-")
				: "-");

		// ユーザー拡張項目
		var userExtendDispValues = new List<KeyValuePair<string, string>>();
		var userExtendSettings = new UserExtendSettingList(this.LoginOperatorId);
		var userExtend = user.UserExtend;
		if (userExtend != null)
		{
			foreach (var setting in userExtendSettings.Items.Where(item =>
				item.DisplayKbn.Contains(Constants.FLG_USEREXTENDSETTING_DISPLAY_CS) && userExtend.UserExtendDataText.ContainsKey(item.SettingId)))
			{
				userExtendDispValues.Add(
						new KeyValuePair<string, string>(setting.SettingName, (string)userExtend.UserExtendDataText[setting.SettingId]));
			}
			while ((userExtendDispValues.Count % 4) != 0)
			{
				userExtendDispValues.Add(new KeyValuePair<string, string>());
			}
			rUserExtendList.DataSource = userExtendDispValues;
			rUserExtendList.DataBind();
		}

		// ユーザー属性
		var attributeModel = new UserService().GetUserAttribute(hfSelectedUserId.Value);
		if (attributeModel != null)
		{
			lFirstOrderDate.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					attributeModel.FirstOrderDate,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
					"-"));
			lSecondOrderDate.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					attributeModel.SecondOrderDate,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
					"-"));
			lLastOrderDate.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					attributeModel.LastOrderDate,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
					"-"));
			lEnrollmentDays.Text = WebSanitizer.HtmlEncode(attributeModel.EnrollmentDays.HasValue ? StringUtility.ToNumeric(attributeModel.EnrollmentDays) : "-");
			lAwayDays.Text = WebSanitizer.HtmlEncode(attributeModel.AwayDays.HasValue ? StringUtility.ToNumeric(attributeModel.AwayDays) : "-");
			lOrderAmountOrderAll.Text = WebSanitizer.HtmlEncode(attributeModel.OrderAmountOrderAll.ToPriceString(true));
			lOrderAmountOrderFp.Text = WebSanitizer.HtmlEncode(attributeModel.OrderAmountOrderFp.ToPriceString(true));
			lOrderCountOrderAll.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderCountOrderAll));
			lOrderCountOrderFp.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderCountOrderFp));
			lOrderAmountShipAll.Text = WebSanitizer.HtmlEncode(attributeModel.OrderAmountShipAll.ToPriceString(true));
			lOrderAmountShipFp.Text = WebSanitizer.HtmlEncode(attributeModel.OrderAmountShipFp.ToPriceString(true));
			lOrderCountShipAll.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderCountShipAll));
			lOrderCountShipFp.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderCountShipFp));
			if (Constants.CPM_OPTION_ENABLED)
			{
				lCpmCluster.Text = (string.IsNullOrEmpty(attributeModel.CpmClusterId) == false)
					? string.Format(
					// 「{0}客」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_USER_SEARCH,
							Constants.VALUETEXT_PARAM_USER,
							Constants.VALUETEXT_PARAM_CUSTOMER),
						WebSanitizer.HtmlEncode(attributeModel.GetCpmClusterName(Constants.CPM_CLUSTER_SETTINGS)))
					: string.Empty;
				lCpmClusterBefore.Text = (string.IsNullOrEmpty(attributeModel.CpmClusterIdBefore) == false)
					? string.Format(
						"{0}",
						WebSanitizer.HtmlEncode(string.Format(
					// 「（以前：{0}客）」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_SEARCH,
								Constants.VALUETEXT_PARAM_USER,
								Constants.VALUETEXT_PARAM_GUEST),
							attributeModel.GetCpmClusterNameBefore(Constants.CPM_CLUSTER_SETTINGS))))
					: string.Empty;
				lCpmClusterChangedDate.Text = ((string.IsNullOrEmpty(attributeModel.CpmClusterIdBefore) == false) && (attributeModel.CpmClusterChangedDate.HasValue))
					? WebSanitizer.HtmlEncode(
						DateTimeUtility.ToStringForManager(
							attributeModel.CpmClusterChangedDate,
							DateTimeUtility.FormatType.ShortDate2Letter))
					: string.Empty;
			}
		}

		lAttributeCalculateDate.Text = WebSanitizer.HtmlEncode(
			(attributeModel != null)
				? DateTimeUtility.ToStringForManager(
					attributeModel.DateChanged,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)
				// 「未集計」
				: ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_SEARCH,
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_UNAGGREGATED));
		divUserDetailAttributeInfoInner.Visible = (attributeModel != null);
		divUserDetailAttributeNoInfoInner.Visible = (attributeModel == null);

		divUserDetail.Visible = true;
		divUserHistory.Visible = true;

		SetLoadHistoryFlg();
		DisplayExternalLinkButton();
	}
	#endregion

	/// <summary>
	/// リンク表示
	/// </summary>
	private void DisplayExternalLinkButton()
	{
		// 外部リンク情報一覧取得
		int bgnRow = Constants.CONST_DISP_EXTERNAL_LINK_BUTTON * (this.RequestPageNum - 1) + 1;
		int endRow = Constants.CONST_DISP_EXTERNAL_LINK_BUTTON * this.RequestPageNum;
		var service = new CsExternalLinkService();

		// リンクボタン
		var models = service.Search(
			this.LoginOperatorDeptId,
			this.RequestLinkId,
			this.RequestTitle,
			Constants.FLG_CSEXTERNALLINK_VALID_FLG_VALID,
			bgnRow,
			endRow);

		// リンクボタン一覧表示
		rExternalLinkList.DataSource = models;
		rExternalLinkList.DataBind();

		// 余分リンクボタン
		var extraModels = service.GetValidFlg(
			this.LoginOperatorDeptId,
			Constants.FLG_CSEXTERNALLINK_VALID_FLG_VALID);

		// 余分リンクボタン一覧表示
		rExtraExternalLinkList.DataSource = extraModels;
		rExtraExternalLinkList.DataBind();

		// 件数取得、エラー表示制御
		if (extraModels.Length > Constants.CONST_DISP_EXTERNAL_LINK_BUTTON)
		{
			divExterExternalLink.Visible = true;
		}
	}

	#region #lbDispUserRelation_Click ユーザーリレーション情報表示リンクボタンクリック
	/// <summary>
	/// ユーザーリレーション情報表示リンクボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDispUserRelation_Click(object sender, EventArgs e)
	{
		this.UserSubInfoDispMode = UserSubInfoDispModeType.UserRelation;
	}
	#endregion

	#region #lbDispUserExtends_Click ユーザー拡張項目情報表示リンクボタンクリック
	/// <summary>
	/// ユーザー拡張項目情報表示リンクボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDispUserExtends_Click(object sender, EventArgs e)
	{
		this.UserSubInfoDispMode = UserSubInfoDispModeType.UserExtends;
	}
	#endregion

	#region #lbDispUserAttribute_Click ユーザー属性情報表示リンクボタンクリック
	/// <summary>
	/// ユーザー属性情報表示リンクボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDispUserAttribute_Click(object sender, EventArgs e)
	{
		this.UserSubInfoDispMode = UserSubInfoDispModeType.UserAttribute;
	}
	#endregion

	#region #lbDispUserHistoryAll_Click ユーザー履歴（全体）表示
	/// <summary>
	/// ユーザー履歴（全体）表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDispUserHistoryAll_Click(object sender, EventArgs e)
	{
		this.UserHistoryDispMode = UserHistoryModeType.All;
		SetLoadHistoryFlg();
	}
	#endregion

	#region #lbDispUserHistoryOrder_Click ユーザー履歴（注文）表示
	/// <summary>
	/// ユーザー履歴（注文）表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDispUserHistoryOrder_Click(object sender, EventArgs e)
	{
		this.UserHistoryDispMode = UserHistoryModeType.Order;
		SetLoadHistoryFlg();
	}
	#endregion

	#region #lbDispUserHistoryFixedPurchase_Click ユーザー履歴（定期情報）表示
	/// <summary>
	/// ユーザー履歴（定期情報）表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDispUserHistoryFixedPurchase_Click(object sender, EventArgs e)
	{
		this.UserHistoryDispMode = UserHistoryModeType.FixedPurchase;
		SetLoadHistoryFlg();
	}
	#endregion

	#region #lbDispUserHistoryMessage_Click ユーザー履歴（メッセージ）表示
	/// <summary>
	/// ユーザー履歴（メッセージ）表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDispUserHistoryMessage_Click(object sender, EventArgs e)
	{
		this.UserHistoryDispMode = UserHistoryModeType.Message;
		SetLoadHistoryFlg();
	}
	#endregion

	#region #lbDispMailSendLog_Click ユーザー履歴（メール送信ログ）表示
	/// <summary>
	/// ユーザー履歴（メール送信ログ）表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDispMailSendLog_Click(object sender, EventArgs e)
	{
		this.UserHistoryDispMode = UserHistoryModeType.MailSendLog;
		SetLoadHistoryFlg();
	}
	#endregion

	#region #lbDispDmShippingHistory_Click ユーザー履歴（DM発送履歴）表示
	/// <summary>
	/// ユーザー履歴（DM発送履歴）表示
	/// </summary>
	protected void lbDispDmShippingHistory_Click(object sender, EventArgs e)
	{
		this.UserHistoryDispMode = UserHistoryModeType.DmShippingHistory;
		SetLoadHistoryFlg();
	}
	#endregion

	#region #lbLoadHistories_Click ユーザー履歴をロード
	/// <summary>
	/// ユーザー履歴をロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbLoadHistories_Click(object sender, EventArgs e)
	{
		var list = GetUserHistory(hfSelectedUserId.Value, this.UserHistoryDispMode).ToList();
		rUserHistoryInfoAll.DataSource = list;
		rUserHistoryInfoAll.DataBind();

		lUserHistoryCount.Text = StringUtility.ToNumeric(rUserHistoryInfoAll.Items.Count);

		divHistoryHead.Visible = true;
		divHistoryBody.Visible = true;
		imgLoading.Visible = false;

		hfLoadHistoryFlg.Value = "0";	// 履歴読み込みフラグを落とす
	}
	#endregion

	#region SetLoadHistoryFlg 履歴読み込みフラグを立てる
	/// <summary>
	/// 履歴読み込みフラグを立てる
	/// </summary>
	private void SetLoadHistoryFlg()
	{
		divHistoryHead.Visible = false;
		divHistoryBody.Visible = false;
		imgLoading.Visible = true;
		hfLoadHistoryFlg.Value = "1";	// 履歴読み込みフラグを立てる
	}
	#endregion

	#region CombineWithNewLine 改行で結合する
	/// <summary>
	/// 改行で結合する
	/// </summary>
	/// <param name="src"></param>
	/// <returns></returns>
	private string CombineWithNewLine(params object[] src)
	{
		return string.Join("\r\n", src).Trim();
	}
	#endregion

	#region GetUserHistoryユーザー履歴取得
	/// <summary>
	/// ユーザー履歴取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="historyMode">履歴モード</param>
	/// <returns>履歴一覧</returns>
	private UserHistoryBase[] GetUserHistory(string userId, UserHistoryModeType historyMode)
	{
		UserHistoryBase[] histories = null;
		switch (historyMode)
		{
			case UserHistoryModeType.All:
				histories = (Constants.DM_SHIPPING_HISTORY_OPTION_ENABLED
					&& (Constants.DMSHIPPINGHISTORY_DISPLAY_METHOD == Constants.DMSHIPPINGHISTORY_DISPLAY_METHOD_ALL))
					? GetUserHistoryOrders(userId)
						.Concat(GetUserHistoryMessages(userId))
						.Concat(GetUserHistoryFixedPurchases(userId))
						.Concat(GetUserHistoryMailSendLog(userId))
						.Concat(GetUserHistoryDmShippingHistory(userId))
						.ToArray()
					: GetUserHistoryOrders(userId)
						.Concat(GetUserHistoryMessages(userId))
						.Concat(GetUserHistoryFixedPurchases(userId))
						.Concat(GetUserHistoryMailSendLog(userId))
						.ToArray();
				break;

			case UserHistoryModeType.Order:
				histories = GetUserHistoryOrders(userId);
				break;

			case UserHistoryModeType.FixedPurchase:
				histories = GetUserHistoryFixedPurchases(userId);
				break;

			case UserHistoryModeType.Message:
				histories = GetUserHistoryMessages(userId);
				break;

			case UserHistoryModeType.MailSendLog:
				histories = GetUserHistoryMailSendLog(userId);
				break;

			case UserHistoryModeType.DmShippingHistory:
				histories = GetUserHistoryDmShippingHistory(userId);
				break;
		}
		histories = histories.OrderByDescending(item => item.DateTime).ToArray();
		foreach (var history in histories)
		{
			if ((history is UserHistoryOrder)
				&& MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM))
			{
				history.Url = SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateOrderDetailUrl((string)history.DataSource[Constants.FIELD_ORDER_ORDER_ID], true, false, Constants.PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH));
			}
			else if ((history is UserHistoryFixedPurchase)
				&& MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM))
			{
				history.Url = SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateFixedPurchaseDetailUrl((string)history.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID], true, false));
			}
			else if (history is UserHistoryMessage)
			{
				CsMessageModel model = new CsMessageModel(history.DataSource);
				history.Url = string.Format("{0}?{1}={2}&{3}={4}&{5}={6}&{7}={8}&",
					Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MESSAGE_INPUT,
					Constants.REQUEST_KEY_MESSAGE_MEDIA_MODE,
					HttpUtility.UrlEncode(Constants.KBN_MESSAGE_MEDIA_MODE_TEL),
					Constants.REQUEST_KEY_MESSAGE_EDIT_MODE,
					HttpUtility.UrlEncode(Constants.KBN_MESSAGE_EDIT_MODE_NEW),
					Constants.REQUEST_KEY_INCIDENT_ID,
					HttpUtility.UrlEncode(model.IncidentId),
					Constants.REQUEST_KEY_MESSAGE_NO,
					model.MessageNo);
			}
			else if (history is UserHistoryMailSendLog)
			{
				var mailSendLog = ((UserHistoryMailSendLog)history).MailSendLog;
				history.Url = string.Format("{0}?{1}={2}",
					Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MAILSENDLOG_CONFIRM,
					Constants.REQUEST_KEY_MAILSENDLOG_LOG_NO,
					HttpUtility.UrlEncode(mailSendLog.LogNo.ToString()));
			}
			else if ((history is UserHistoryDmShippingHistory)
				&& MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM))
			{
				history.Url = SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateUserDetailUrl(userId));
			}
		}
		return histories;
	}
	#endregion

	#region GetUserHistoryOrders ユーザー履歴（注文）取得
	/// <summary>
	/// ユーザー履歴（注文）取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>履歴一覧</returns>
	private UserHistoryBase[] GetUserHistoryOrders(string userId)
	{
		var service = new UserHistoryOrderService(new UserHistoryOrderRepository());
		return service.GetList(userId);
	}
	#endregion

	#region GetUserHistoryFixedPurchases ユーザー履歴（定期情報）取得
	/// <summary>
	/// ユーザー履歴（定期情報）取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>履歴一覧</returns>
	private UserHistoryBase[] GetUserHistoryFixedPurchases(string userId)
	{
		var searchCondition = new UserFixedPurchaseListSearchCondition()
		{
			UserId = userId
		};
		return new FixedPurchaseService().SearchUserFixedPurchase(searchCondition).Select(f => new UserHistoryFixedPurchase(f)).ToArray();
	}
	#endregion

	#region -GetUserHistoryMessages ユーザー履歴（メッセージ）取得
	/// <summary>
	/// ユーザー履歴（メッセージ）取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>履歴一覧</returns>
	private UserHistoryBase[] GetUserHistoryMessages(string userId)
	{
		var service = new UserHistoryMessageService(new UserHistoryMessageRepository());
		return service.GetList(userId).ToArray();
	}
	#endregion

	#region -GetUserHistoryMailSendLog ユーザー履歴（メール送信ログ）取得
	/// <summary>
	/// ユーザー履歴（メール送信ログ）取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>履歴一覧</returns>
	private UserHistoryBase[] GetUserHistoryMailSendLog(string userId)
	{
		return new MailSendLogService().GetMailSendLogByUserId(userId).Select(m => new UserHistoryMailSendLog(m)).ToArray();
	}
	#endregion

	#region -GetUserHistoryDmShippingHistory ユーザー履歴（DM発送履歴）取得
	/// <summary>
	/// ユーザー履歴（DM発送履歴）取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>履歴一覧</returns>
	private UserHistoryBase[] GetUserHistoryDmShippingHistory(string userId)
	{
		var histories = DmShippingHistoryService
			.GetDmShippingHistoryByUserId(userId)
			.Select(model => new UserHistoryDmShippingHistory(model))
			.ToArray();
		return histories;
	}

	#endregion

	/// <summary>
	/// ユーザーマスタ/拡張項目マスタの項目取得
	/// </summary>
	/// <param name="linkurl">リンクパス</param>
	/// <param name="userId">ユーザーID</param>
	/// <returns>ユーザー情報</returns>
	protected string GetReplacedLink(string linkurl, string userId)
	{
		// ユーザ項目取得
		var userModel = new UserService().Get(userId);
		// ユーザ拡張項目マスタ設定取得
		var extendModelSetting = new UserService().GetUserExtendSettingArray();
		// URLに含まれる置換タグを取り出す
		var inReplaceTag = Regex.Matches(linkurl, "@@.[a-zA-Z0-9_]+.@@")
			.Cast<Match>()
			.Select(x => x.Value)
			.ToArray();
		// URLを一次変数へ格納
		var url = linkurl;

		// URLに置換タグが含まれているか
		if (inReplaceTag.Any())
		{
			// URLに含まれる置換タグの数だけループ
			foreach (var item in inReplaceTag)
			{
				// ユーザ項目マスタのデータ取得
				var tempUm = userModel.DataSource[item.Replace("@", string.Empty).Trim()];
				// ユーザ拡張項目マスタのタグ取得
				var tagEm = extendModelSetting.FirstOrDefault(x => x.SettingId == item.Replace("@", string.Empty).Trim());
				// ユーザ拡張項目マスタのデータ取得
				var tempEm = (tagEm != null) ? userModel.UserExtend.DataSource[tagEm.SettingId] : null;

				// 置換文字列設定
				var replaced = (tempUm != null)
					? tempUm.ToString()
					: (tempEm != null)
						? tempEm.ToString()
						: string.Empty;
				// リンクの置換タグを置き換え
				url = Regex.Replace(url, item, replaced);
			}
		}
		// Urlが正しいか確認
		var correctUrl = CorrectUrlConfirm(url);
		return correctUrl;
	}

	/// <summary>
	/// URLが正しいか確認
	/// </summary>
	/// <param name="url">URLパス</param>
	/// <returns>URL</returns>
	private string CorrectUrlConfirm(string url)
	{
		// URL形式
		var isUrl = Regex.IsMatch(url, @"^s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+$");
		// URLが正しくない場合
		if ((isUrl == false) || string.IsNullOrEmpty(url)) return ErrorUrl(WebMessages.ERRMSG_MANAGER_URL_FORMAT_ERROR);
		
		try
		{
			var request = WebRequest.Create(url);
			request.GetResponse().Close();
		}
		catch (WebException ex)
		{
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				var response = (HttpWebResponse)ex.Response;
				if ((int)response.StatusCode == 404 || (int)response.StatusCode == 410) return ErrorUrl(WebMessages.ERRMSG_404_ERROR);
			}
		}
		catch (UriFormatException)
		{
			return url;
		}
		return url;
	}

	/// <summary>
	/// 履歴コンテンツ文字列取得
	/// </summary>
	/// <param name="history">履歴</param>
	/// <returns>コンテンツ文字列</returns>
	protected string GetHistoryContetns(UserHistoryBase history)
	{
		if (history is UserHistoryOrder)
		{
			var hist = (UserHistoryOrder)history;
			var localId = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId;
			var orderPriceTotal = StringUtility.ToPrice(
				StringUtility.ToNumeric(hist.OrderPriceTotal.ToPriceString()),
				localId,
				GlobalConfigUtil.GetCurrencyLocaleFormat(localId));
			return string.Format(
				"[{0}][{1}][{2}] {3} （{4}{5}）{6}",
				hist.OrderId,
				ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, hist.OrderStatus),
				ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
					hist.OrderPaymentStatus),
				hist.Items[0].ItemProductName,
				((hist.OrderItemCount > 1)
					? string.Format(
						// 「他{0}明細」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_USER_SEARCH,
							Constants.VALUETEXT_PARAM_USER,
							Constants.VALUETEXT_PARAM_DETAILS),
						(hist.OrderItemCount - 1))
					: string.Empty),
				string.Format(
					// 「合計 {0}」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_USER_SEARCH,
						Constants.VALUETEXT_PARAM_USER,
						Constants.VALUETEXT_PARAM_TOTAL),
					orderPriceTotal),
				(Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE)
					? string.Format(
						// 「出荷予定日 {0}」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_USER_SEARCH,
							Constants.VALUETEXT_PARAM_USER,
							Constants.VALUETEXT_PARAM_SCHEDULED_SHIPPING_DATE),
						hist.Shippings[0].OrderShippingScheduleShippingDate.HasValue
							? DateTimeUtility.ToStringForManager(
								hist.Shippings[0].OrderShippingScheduleShippingDate.Value,
								DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime)
							// 「指定なし」
							: ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_SEARCH,
								Constants.VALUETEXT_PARAM_USER,
								Constants.VALUETEXT_PARAM_UNSPECIFIED))
					: string.Empty);
		}
		else if (history is UserHistoryFixedPurchase)
		{
			var hist = (UserHistoryFixedPurchase)history;
			return string.Format(
				// 「[{0}][{1}][{2}][購入回数:{3}回][最終{4}, 次回{5}] {6}{7}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_SEARCH,
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_NUMBER_OF_PURCHASES_FINAL_NEXT_TIME),
				hist.FixedPurchase.FixedPurchaseId,
				hist.FixedPurchase.FixedPurchaseStatusText,
				OrderCommon.CreateFixedPurchaseSettingMessage(hist.FixedPurchase),
				hist.FixedPurchase.OrderCount,
				hist.FixedPurchase.LastOrderDate.HasValue ? hist.FixedPurchase.LastOrderDate.Value.ToString("yyyy/MM/dd") : "",
				hist.FixedPurchase.NextShippingDate.HasValue ? hist.FixedPurchase.NextShippingDate.Value.ToString("yyyy/MM/dd") : "",
				hist.FixedPurchase.Shippings[0].Items[0].Name,
				(hist.FixedPurchase.Shippings[0].Items.Length > 1)
					? string.Format(
					// 「（他{0}商品）」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_USER_SEARCH,
							Constants.VALUETEXT_PARAM_USER,
							Constants.VALUETEXT_PARAM_OTHER_PRODUCTS),
						(hist.FixedPurchase.Shippings[0].Items.Length - 1))
					: String.Empty);
		}
		else if (history is UserHistoryMessage)
		{
			var hist = (UserHistoryMessage)history;
			return string.Format(
				// 「[{0}][{1}/{2}] {3} （作成者:{4}）」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_SEARCH,
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_AUTHOR),
				hist.IncidentId,
				ValueText.GetValueText(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_MEDIA_KBN, hist.MediaKbn),
				ValueText.GetValueText(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_DIRECTION_KBN, hist.DirectionKbn),
				hist.InquiryTitle,
				string.IsNullOrEmpty(hist.OperatorName) ? "-" : hist.OperatorName);
		}
		else if (history is UserHistoryMailSendLog)
		{
			var hist = (UserHistoryMailSendLog)history;
			return string.Format(
			"{0}",
			hist.MailSendLog.MailSubject);
		}
		else if (history is UserHistoryDmShippingHistory)
		{
			var hist = (UserHistoryDmShippingHistory)history;
			return string.Format(
				// 「[DMコード：{0}][DM名：{1}][有効期間：{2}]」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_SEARCH,
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_DM_CODE_NAME_VALIDITY_PERIOD),
				hist.DmShippingHistory.DmCode,
				hist.DmShippingHistory.DmName,
				hist.DmShippingHistory.ValidDate);
		}

		return string.Empty;
	}

	/// <summary>
	/// 履歴アイコン取得
	/// </summary>
	/// <param name="history">履歴</param>
	/// <returns>メッセージアイコン/受注アイコン</returns>
	protected string GetHistoryIcon(UserHistoryBase history)
	{
		var imgTag = "<img src=\"../../Images/Cs/{0}\" alt=\"{1}\" />";
		if (history is UserHistoryOrder)
		{
			return string.Format(imgTag, "icon_order.png", "order");
		}
		else if (history is UserHistoryFixedPurchase)
		{
			return string.Format(imgTag, "icon_fixed_purchase.png", "fixedpurchase");
		}
		else if (history is UserHistoryMessage)
		{
			return string.Format(imgTag, ((UserHistoryMessage)history).GetMessageIcon(), "mail");
		}
		else if (history is UserHistoryMailSendLog)
		{
			var hist = (UserHistoryMailSendLog)history;
			return string.Format(imgTag, "icon_mail_out.png", "mailsendlog")
				+ (hist.MailSendLog.HasError ? " " + string.Format(imgTag, "icon_error.png", "error") : "");
		}
		return "";
	}

	/// <summary>
	/// 既読か
	/// </summary>
	/// <param name="history">履歴</param>
	/// <returns>既読/未読</returns>
	protected bool IsCheckedReadFlg(UserHistoryBase history)
	{
		if (history is UserHistoryMailSendLog)
		{
			return (((UserHistoryMailSendLog)history).MailSendLog.ReadFlg == Constants.FLG_MAILSENDLOG_READ_FLG_READ);
		}
		return false;
	}

	/// <summary>
	/// メール送信履歴か
	/// </summary>
	/// <param name="history">履歴</param>
	/// <returns>メール送信履歴か</returns>
	protected bool IsMailSendLog(UserHistoryBase history)
	{
		return (history is UserHistoryMailSendLog);
	}

	/// <summary>
	/// 履歴詳細
	/// </summary>
	/// <param name="history">ユーザー履歴</param>
	/// <returns>履歴詳細文字列</returns>
	protected string GetHistoryDetail(UserHistoryBase history)
	{
		if (history is UserHistoryOrder)
		{
			var hist = (UserHistoryOrder)history;
			List<string> details = new List<string>();
			foreach (var child in hist.Items)
			{
				details.Add(string.Format("{0} x {1}", child.ItemProductName, child.ItemQuantity));
			}
			return string.Join("\r\n", details);
		}
		else if (history is UserHistoryFixedPurchase)
		{
			var hist = (UserHistoryFixedPurchase)history;
			List<string> details = new List<string>();
			foreach (var child in hist.FixedPurchase.Shippings[0].Items)
			{
				details.Add(string.Format("{0} x {1}", child.Name, child.ItemQuantity));
			}
			return string.Join("\r\n", details);
		}
		return "";
	}

	/// <summary>
	/// 既読フラグ取得
	/// </summary>
	/// <param name="history">履歴情報</param>
	/// <returns>既読/未読</returns>
	protected string GetReadFlg(UserHistoryBase history)
	{
		if (history is UserHistoryMailSendLog)
		{
			return ValueText.GetValueText(
				Constants.TABLE_MAILSENDLOG,
				Constants.FIELD_MAILSENDLOG_READ_FLG,
				((UserHistoryMailSendLog)history).MailSendLog.ReadFlg);
		}
		return "";
	}

	#region SortUserInUserSearch
	/// <summary>
	/// User sort action
	/// </summary>
	/// <param name="sender">sender</param>
	/// <param name="e">event</param>
	protected void lbUserSearchSort_Click(object sender, EventArgs e)
	{
		var sortKbn = (SortUserSearchKbn)Enum.Parse(typeof(SortUserSearchKbn), ((LinkButton)sender).CommandArgument);
		this.SortType = ((this.SortKbnCurrent == sortKbn) && (this.SortType == Constants.FLG_SORT_KBN_ASC)) ? Constants.FLG_SORT_KBN_DESC : Constants.FLG_SORT_KBN_ASC;
		this.SortKbnCurrent = sortKbn;
		var userSearch = new DataView(this.UserSearch);
		RefreshCurrentSortSymbols();

		switch (this.SortKbnCurrent)
		{
			case SortUserSearchKbn.Classification:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_USER_KBN, this.lClassificationIconSort, this.rUserSearchResult);
				break;

			case SortUserSearchKbn.UserName:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_NAME_KANA, this.lUserNameIconSort, this.rUserSearchResult);
				break;

			case SortUserSearchKbn.CompanyName:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_COMPANY_NAME, this.lCompanyNameIconSort, this.rUserSearchResult);
				break;

			case SortUserSearchKbn.CompanyPostName:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_COMPANY_POST_NAME, this.lCompanyPostNameIconSort, this.rUserSearchResult);
				break;

			case SortUserSearchKbn.MailAddress:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_MAIL_ADDR, this.lMailAddressIconSort, this.rUserSearchResult);
				break;

			case SortUserSearchKbn.PhoneNumber:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_TEL1, this.lPhoneNumberIconSort, this.rUserSearchResult);
				break;

			case SortUserSearchKbn.StreetAddress:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_ADDR, this.lStreetAddressIconSort, this.rUserSearchResult);
				break;

			case SortUserSearchKbn.ManagementLevel:
				SortAction(userSearch, this.SortType, Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME, this.lManagementLevelIconSort, this.rUserSearchResult);
				break;

			case SortUserSearchKbn.RegisteredDate:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_DATE_CREATED, this.lRegisteredDateIconSort, this.rUserSearchResult);
				break;

			default:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_USER_ID, this.lUserIdIconSort, this.rUserSearchResult);
				break;
		}
	}

	/// <summary>
	/// Refresh current sort symbols
	/// </summary>
	private void RefreshCurrentSortSymbols()
	{
		this.lUserIdIconSort.Text
			= this.lClassificationIconSort.Text
			= this.lUserNameIconSort.Text
			= this.lCompanyNameIconSort.Text
			= this.lCompanyPostNameIconSort.Text
			= this.lMailAddressIconSort.Text
			= this.lPhoneNumberIconSort.Text
			= this.lManagementLevelIconSort.Text
			= this.lStreetAddressIconSort.Text
			= this.lRegisteredDateIconSort.Text = string.Empty;
	}

	/// <summary>
	/// Set default sort
	/// </summary>
	private void SetDefaultSort()
	{
		RefreshCurrentSortSymbols();
		this.SortKbnCurrent = SortUserSearchKbn.UserId;
		this.SortType = Constants.FLG_SORT_KBN_ASC;
		this.lUserIdIconSort.Text = Constants.FLG_SORT_SYMBOL_ASC;
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザーサブ情報表示モード</summary>
	protected UserSubInfoDispModeType UserSubInfoDispMode
	{
		get { return (UserSubInfoDispModeType)(ViewState["UserDispSubMode"] ?? UserSubInfoDispModeType.UserRelation); }
		set { ViewState["UserDispSubMode"] = value; }
	}
	/// <summary>ユーザー履歴表示モード</summary>
	protected UserHistoryModeType UserHistoryDispMode
	{
		get { return (UserHistoryModeType)(ViewState["UserHistoryDispMode"] ?? UserHistoryModeType.All); }
		set { ViewState["UserHistoryDispMode"] = value; }
	}
	/// <summary>User Search</summary>
	private DataTable UserSearch
	{
		get { return (DataTable)ViewState["UserSearch"]; }
		set { ViewState["UserSearch"] = value; }
	}
	/// <summary>Sort Kbn</summary>
	protected SortUserSearchKbn SortKbnCurrent
	{
		get { return (SortUserSearchKbn)ViewState["sortKbnCurrent"]; }
		set { ViewState["sortKbnCurrent"] = value; }
	}
	/// <summary>Sort order status</summary>
	protected string SortType
	{
		get { return (string)ViewState["SortType"]; }
		set { ViewState["SortType"] = value; }
	}
	/// <summary>電話番号をパラメータ付与するか</summary>
	protected bool IsAddParamTelNo
	{
		get
		{
			return (((this.SearchCount == 0) && (string.IsNullOrEmpty(tbSearchTel.Text) == false))
				&& (StringUtility.ToHankaku(tbSearchTel.Text).Replace("-", "").All(char.IsDigit)));
		}
	}
	/// <summary>検索結果件数</summary>
	protected int SearchCount { get; set; }
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	/// <summary>外部リンクID</summary>
	private string RequestLinkId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_LINK_ID]); }
	}
	/// <summary>リクエスト：外部リンク設定用タイトル</summary>
	private string RequestTitle
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_TITLE]); }
	}
	/// <summary>エラーページ</summary>
	private static string ErrorUrl(string message)
	{
		return new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_MANAGER_ERRORKBN, HttpUtility.UrlEncode(message))
				.CreateUrl();
	}
	#endregion

	#region 内部クラス +UserOrderInfo ユーザー注文情報
	/// <summary>
	/// ユーザー注文情報
	/// </summary>
	private class UserOrderInfo
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		public UserOrderInfo(string userId)
		{
			CreateInfo(userId);
		}

		/// <summary>
		/// 情報作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		private void CreateInfo(string userId)
		{
			DataSet relationInfo = GetInfo(userId);

			int counter = 0;
			this.FirstOrderDate = (relationInfo.Tables[counter].Rows.Count > 0) ? (DateTime?)relationInfo.Tables[counter].Rows[0][0] : null;
			counter++;

			this.LastOrderDate = (relationInfo.Tables[counter].Rows.Count > 0) ? (DateTime?)relationInfo.Tables[counter].Rows[0][0] : null;
			this.LastOrderPrice = (relationInfo.Tables[counter].Rows.Count > 0) ? (decimal?)relationInfo.Tables[counter].Rows[0][1] : null;
			counter++;

			this.OrderCount = (int)relationInfo.Tables[counter].Rows[0][0];
			counter++;

			this.OrderPriceYearTotal = (decimal)relationInfo.Tables[counter].Rows[0][0];
			counter++;
		}

		/// <summary>
		/// 情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>購入情報データセット（初回購入系、最終購入系、購入回数、年間購買累計金額）</returns>
		protected virtual DataSet GetInfo(string userId)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("CsUserSearch", "GetUserOrderInfo"))
			{
				var input = new Hashtable();
				input.Add(Constants.FIELD_USER_USER_ID, userId);

				return sqlStatement.SelectStatement(sqlAccessor, input);
			}
		}

		/// <summary>初回購入日</summary>
		public DateTime? FirstOrderDate { get; private set; }
		/// <summary>最終購入日</summary>
		public DateTime? LastOrderDate { get; private set; }
		/// <summary>最終購入金額</summary>
		public decimal? LastOrderPrice { get; private set; }
		/// <summary>購入回数</summary>
		public int OrderCount { get; private set; }
		/// <summary>年間累計購買金額</summary>
		public decimal OrderPriceYearTotal { get; private set; }
	}
	#endregion

	#region 内部クラス +UserOrderFixedPurshaseInfo ユーザー注文情報（定期）
	/// <summary>
	/// ユーザー注文情報（定期）
	/// </summary>
	private class UserOrderFixedPurshaseInfo : UserOrderInfo
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		public UserOrderFixedPurshaseInfo(string userId)
			: base(userId)
		{
		}

		/// <summary>
		/// 情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>定期系データセット（初回購入系、最終購入系、購入回数、年間購買累計金額）</returns>
		protected override DataSet GetInfo(string userId)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("CsUserSearch", "GetUserOrderFixedPurchaseInfo"))
			{
				var input = new Hashtable();
				input.Add(Constants.FIELD_USER_USER_ID, userId);

				return sqlStatement.SelectStatement(sqlAccessor, input);
			}
		}
	}
	#endregion

	/// <summary>
	/// 頒布会注文か（あるいは頒布会定期）？
	/// </summary>
	/// <param name="history">履歴情報</param>
	/// <returns>結果</returns>
	protected bool IsSubscriptionBoxOrder(UserHistoryBase history)
	{
		var condition = new Hashtable();
		if ((history as UserHistoryOrder) != null)
		{
			condition.Add(Constants.FIELD_ORDER_ORDER_ID, ((UserHistoryOrder)history).OrderId);
		}
		else if ((history as UserHistoryFixedPurchase) != null)
		{
			condition.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ID, ((UserHistoryFixedPurchase)history).FixedPurchase.Shippings[0].FixedPurchaseId);
		}
		else
		{
			return false;
		}

		var result = new SubscriptionBoxService().IsSubscriptionBoxOrderOrFixedPurchase(condition);
		return result;
	}
}
