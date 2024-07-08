/*
=========================================================================================================
  Module      : 店舗受取注文一覧画面(StorePickUpOrderList.aspx.cs)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.ManagerListDispSetting;
using w2.Domain.OperatorAuthority;

/// <summary>
/// Store pick up order list
/// </summary>
public partial class Form_StorePickUp_StorePickUpOrderList : OrderPage
{
	/// <summary>Default search rowspan</summary>
	protected const int DEFAULT_SEARCH_ROWSPAN = 32;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitializeComponents();

			// リクエスト情報取得
			var htParam = GetParametersAndSetToControl();

			// 検索情報保持(編集で利用)
			Session[Constants.SESSIONPARAM_KEY_ORDER_SEARCH_INFO] = htParam;

			var currentPageNo = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

			// 注文件数取得
			var searchSqlInfo = GetSearchSqlInfo(htParam);
			var realShopIds = string.Empty;
			if (this.OperatorAuthoritys != null)
			{
				realShopIds = string.Format(
					"'{0}'",
					string.Join("','", this.OperatorAuthoritys.Select(item => item.ConditionValue).ToArray()));
			}

			var totalOrderCounts = DomainFacade.Instance.OrderService.GetOrderStorePickUpCount(searchSqlInfo, realShopIds);

			if (totalOrderCounts > 0)
			{
				// 表示件数に基づいてページャ数を計算する
				if ((totalOrderCounts % Constants.CONST_DISP_CONTENTS_DEFAULT_LIST == 0)
					&& (totalOrderCounts / Constants.CONST_DISP_CONTENTS_DEFAULT_LIST < currentPageNo))
				{
					currentPageNo = totalOrderCounts / Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;
				}

				// 検索条件に係る項目の編集によりページが減った場合の対応
				if (totalOrderCounts / Constants.CONST_DISP_CONTENTS_DEFAULT_LIST < currentPageNo)
				{
					currentPageNo = (totalOrderCounts / Constants.CONST_DISP_CONTENTS_DEFAULT_LIST) + 1;
				}

				// 検索情報取得
				var sqlParams = new Hashtable
				{
					{ Constants.FIELD_TARGETLIST_TARGET_TYPE, Constants.FLG_TARGETLIST_TARGET_TYPE_ORDER_LIST },
					{ Constants.TABLE_USER, searchSqlInfo }
				};

				Session[Constants.SESSION_KEY_PARAM + "EC"] = sqlParams;
				btnImportTargetList.Enabled = true;
			}
			else
			{
				btnImportTargetList.Enabled = false;
			}

			// 注文件数による表示制御
			trListError.Visible = false;

			// データ総数が定数以上の場合警告文を表示、以下の場合は条件を絞って注文一覧を表示
			if (totalOrderCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				trListError.Visible = true;

				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST);
				tdErrorMessage.InnerHtml = tdErrorMessage.InnerHtml.Replace(
					"@@ 1 @@",
					StringUtility.ToNumeric(totalOrderCounts));
				tdErrorMessage.InnerHtml = tdErrorMessage.InnerHtml.Replace(
					"@@ 2 @@",
					StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				return;
			}

			// 表示件数が０の場合、メッセージを表示して抜ける
			if (totalOrderCounts == 0)
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

				return;
			}

			if (string.IsNullOrEmpty((string)searchSqlInfo[Constants.FIELD_ORDER_STOREPICKUP_STATUS]))
			{
				searchSqlInfo[Constants.FIELD_ORDER_STOREPICKUP_STATUS] = "1";
			}
			var orders = GetOrderListWithUserSymbols(searchSqlInfo, currentPageNo, realShopIds);

			rList.DataSource = orders;
			rList.DataBind();

			// Data source set of fixed table
			rTableFixColumn.DataSource = orders;
			rTableFixColumn.DataBind();

			// 表示設定の確認
			BindDispSetting();

			// その他表示制御
			// ページャ作成（一覧処理で総件数を取得）
			string strNextUrl = StorePickUpOrderList(htParam, false).CreateUrl();
			lbPager1.Text = WebPager.CreateDefaultListPager(totalOrderCounts, currentPageNo, strNextUrl);
		}
	}

	/// <summary>
	/// ヘッダーに表示する項目名を適切なフォーマットに変換する
	/// </summary>
	/// <param name="dispColumnName">表示項目名</param>
	/// <returns>表示項目名</returns>
	protected string ConvertDispColumnNameFormat(string dispColumnName)
	{
		var resultColumnName = string.Empty;

		// 拡張ステータスに"拡張ステータス"をつける
		if (dispColumnName.Contains(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME))
		{
			// 翻訳するため、"拡張ステータス"をValueTextから取り出している。
			var extendS = ValueText.GetValueText(
				Constants.TABLE_MANAGERLISTDISPSETTING,
				Constants.FIELD_MANAGERLISTDISPSETTING_DISP_COLUMN_NAME,
				Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME);

			var numberReplaceExtendNumber = dispColumnName.Replace(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME, string.Empty);
			foreach (var dispExtendStatus in this.DisplayExtendStatusList)
			{
				if ((dispExtendStatus.Value == numberReplaceExtendNumber) == false) continue;

				resultColumnName = string.Format(
					"{0}{1}：\r\n{2}",
					extendS,
					dispExtendStatus.Value,
					dispExtendStatus.Key.Replace(numberReplaceExtendNumber + "：", string.Empty));

				break;
			}

			return resultColumnName;
		}

		resultColumnName = ValueText.GetValueText(
			Constants.TABLE_MANAGERLISTDISPSETTING,
			Constants.FIELD_MANAGERLISTDISPSETTING_DISP_COLUMN_NAME,
			dispColumnName);

		return resultColumnName;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 並び順
		ddlSortKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER,
			Constants.REQUEST_KEY_SORT_KBN));

		// 浸取店铺
		ddlStorepickupStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlStorepickupStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER,
			Constants.FIELD_ORDER_STOREPICKUP_STATUS));

		// 注文ステータス
		ddlOrderStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER,
			Constants.FIELD_ORDER_ORDER_STATUS))
		{
			// 実在庫管理なしの場合は在庫引き当て済みを追加したくない。ステータス不明についても。
			if ((Constants.REALSTOCK_OPTION_ENABLED
					|| (li.Value != Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED))
				&& (li.Value != Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN))
			{
				ddlOrderStatus.Items.Add(li);
			}
		}

		// ステータス更新日ドロップダウン作成
		ddlOrderUpdateDateStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, "update_status"))
		{
			// 実在庫管理なしの場合は在庫引き当て済みを追加したくない
			if (Constants.REALSTOCK_OPTION_ENABLED
				|| (li.Value != Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE))
			{
				ddlOrderUpdateDateStatus.Items.Add(li);
			}
		}

		var orderExtendStatusSettingList = GetOrderExtendStatusSettingList(this.LoginOperatorShopId);
		foreach (DataRowView orderExtendStatusSetting in orderExtendStatusSettingList)
		{
			var orderExtendStatusListItem = new ListItem(
				string.Format("{0}:{1}",
					StringUtility.ToEmpty(orderExtendStatusSetting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO]),
					StringUtility.ToEmpty(orderExtendStatusSetting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME])),
				StringUtility.ToEmpty(orderExtendStatusSetting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO]));

			this.DisplayExtendStatusList.Add(orderExtendStatusListItem.Text, orderExtendStatusListItem.Value);
		}
	}

	/// <summary>
	/// パラメタを取得してコントロールにセットする
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParametersAndSetToControl()
	{
		var result = new Hashtable();
		try
		{
			// 注文ID
			result.Add(Constants.REQUEST_KEY_ORDER_ID,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ID]));
			tbOrderId.Text = Request[Constants.REQUEST_KEY_ORDER_ID];

			// 注文ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_STATUS,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_STATUS]));
			ddlOrderStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_STATUS];

			// ユーザーID
			result.Add(Constants.REQUEST_KEY_ORDER_USER_ID,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_USER_ID]));
			tbUserId.Text = Request[Constants.REQUEST_KEY_ORDER_USER_ID];

			// 注文者名
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_NAME]));
			tbOwnerName.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_NAME];

			// 注文者名（かな）
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA]));
			tbOwnerNameKana.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA];

			// メールアドレス
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR]));
			tbOwnerMailAddr.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR];

			// 商品ID
			result.Add(Constants.REQUEST_KEY_PRODUCT_ID,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ID]));
			tbProductId.Text = Request[Constants.REQUEST_KEY_PRODUCT_ID];

			// 商品名
			result.Add(Constants.REQUEST_KEY_PRODUCT_NAME,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_NAME]));
			tbProductName.Text = Request[Constants.REQUEST_KEY_PRODUCT_NAME];

			// 注文者電話番号
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_TEL,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_TEL]));
			tbOwnerTel.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_TEL];

			// 受取店舗ID
			result.Add(Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID]));
			tbRealShopId.Text = Request[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID];

			// 受取店舗
			result.Add(Constants.REQUEST_KEY_REALSHOP_NAME,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REALSHOP_NAME]));
			tbRealShopName.Text = Request[Constants.REQUEST_KEY_REALSHOP_NAME];

			// 受取店铺
			result.Add(Constants.REQUEST_KEY_ORDER_STOREPICKUP_STATUS,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_STOREPICKUP_STATUS]));
			ddlStorepickupStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_STOREPICKUP_STATUS];

			// ソート
			string sortKbn = null;
			switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_ORDER_ID_ASC:
				case Constants.KBN_SORT_ORDER_ID_DESC:
				case Constants.KBN_SORT_ORDER_DATE_ASC:
				case Constants.KBN_SORT_ORDER_DATE_DESC:
				case Constants.KBN_SORT_ORDER_DATE_CREATED_ASC:
				case Constants.KBN_SORT_ORDER_DATE_CREATED_DESC:
				case Constants.KBN_SORT_ORDER_DATE_CHANGED_ASC:
				case Constants.KBN_SORT_ORDER_DATE_CHANGED_DESC:
					sortKbn = Request[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				default:
					sortKbn = Constants.KBN_SORT_ORDER_DATE_DEFAULT;
					break;
			}
			result.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);
			ddlSortKbn.SelectedValue = sortKbn;

			// ページ番号
			int currentPageNumber;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out currentPageNumber) == false)
			{
				currentPageNumber = 1;
			}
			result.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNumber);

			// 初回表示の場合は設定から検索項目を絞り込む
			if (this.IsFirstView)
			{
				if (string.IsNullOrEmpty(Constants.ORDERLIST_FIRSTVIEW_ORDERSTATUS) == false)
				{
					// ステータス更新日・ステータス
					result[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS] = Constants.ORDERLIST_FIRSTVIEW_ORDERSTATUS;
				}

				if (string.IsNullOrEmpty(Constants.ORDERLIST_FIRSTVIEW_ABSTIMESPAN) == false)
				{
					// ステータス更新日
					var timeSpan = RelativeCalendar.FromText(Constants.ORDERLIST_FIRSTVIEW_ABSTIMESPAN);
					ucOrderUpdateDatePeriod.SetPeriodDate(timeSpan.BeginTime,
						DateTimeUtility.GetEndTimeOfDay(timeSpan.EndTime));
				}
			}
			else
			{
				// ステータス更新日・ステータス
				result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS,
					StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS]));
			}
			ddlOrderUpdateDateStatus.SelectedValue = (string)result[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS];

			// Set request key order update date from date to
			result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO]));

			// Set request key order update time from time to
			result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO]));

			var orderUpdateDateFrom = string.Format(
				"{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM])
					.Replace(":", string.Empty));

			var orderUpdateDateTo = string.Format(
				"{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO])
					.Replace(":", string.Empty));

			ucOrderUpdateDatePeriod.SetPeriodDate(
				orderUpdateDateFrom,
				orderUpdateDateTo);

			// 返品を除外
			result.Add(Constants.EXCLUDE_RETURN_FLG, Constants.FLG_ON);
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return result;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	protected Hashtable GetSearchInfoFromControl()
	{
		Hashtable result = new Hashtable
		{
			{ Constants.REQUEST_KEY_ORDER_ID, tbOrderId.Text.Trim() },
			{ Constants.REQUEST_KEY_ORDER_USER_ID, tbUserId.Text.Trim() },
			{ Constants.REQUEST_KEY_ORDER_OWNER_NAME, tbOwnerName.Text.Trim() },
			{ Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA, tbOwnerNameKana.Text.Trim() },
			{ Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR, tbOwnerMailAddr.Text.Trim() },
			{ Constants.REQUEST_KEY_ORDER_OWNER_TEL, tbOwnerTel.Text.Trim() },
			{ Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID, tbRealShopId.Text.Trim() },
			{ Constants.REQUEST_KEY_REALSHOP_NAME, tbRealShopName.Text.Trim() },
			{ Constants.REQUEST_KEY_PRODUCT_ID, tbProductId.Text.Trim() },
			{ Constants.REQUEST_KEY_PRODUCT_NAME, tbProductName.Text.Trim() },
			{ Constants.REQUEST_KEY_ORDER_STOREPICKUP_STATUS, ddlStorepickupStatus.SelectedValue },
			{ Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue },
			{ Constants.REQUEST_KEY_ORDER_ORDER_STATUS, ddlOrderStatus.SelectedValue },
			{ Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS, ddlOrderUpdateDateStatus.SelectedValue },
			{ Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM, ucOrderUpdateDatePeriod.HfStartDate.Value },
			{ Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO, ucOrderUpdateDatePeriod.HfEndDate.Value },
			{ Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM, ucOrderUpdateDatePeriod.HfStartTime.Value },
			{ Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO, ucOrderUpdateDatePeriod.HfEndTime.Value },
		};

		return result;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="searchParam">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable searchParam)
	{
		var orderDateFromDefault = DateTime.Parse("1900/01/01");
		var orderDateToDefault = DateTime.Parse("2999/01/01");

		var result = new Hashtable();

		// 店舗ID
		result.Add(Constants.FIELD_PRODUCT_SHOP_ID,
			this.LoginOperatorShopId);
		// 注文ID
		result.Add(Constants.FIELD_ORDER_ORDER_ID + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_ID]));
		// 注文ステータス
		result.Add(Constants.FIELD_ORDER_ORDER_STATUS,
			StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_STATUS]));
		// ユーザーID
		result.Add(Constants.FIELD_ORDER_USER_ID + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_USER_ID]));
		// 注文者名
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_NAME]));
		// 注文者名（かな）
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA]));
		// メールアドレス
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR]));
		// 商品ID
		result.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_PRODUCT_ID]));
		// 商品名
		result.Add(Constants.FIELD_ORDERITEM_PRODUCT_NAME + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_PRODUCT_NAME]));
		// 注文者電話番号
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_TEL]));
		// 受取店舗ID
		result.Add(Constants.FIELD_REALSHOP_REAL_SHOP_ID + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID]));
		// 受取店舗
		result.Add(Constants.FIELD_REALSHOP_NAME + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_REALSHOP_NAME]));
		// 浸取店铺
		result.Add(Constants.FIELD_ORDER_STOREPICKUP_STATUS,
			StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_STOREPICKUP_STATUS]));
		// ソート区分
		result.Add("sort_kbn",
			StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_SORT_KBN]));
		// ステータス更新日・ステータス
		result.Add(Constants.FIELD_ORDER_ORDER_DATE + "_status",
			StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_STATUS]));
		// ステータス更新日初期化
		result.Add(Constants.FIELD_ORDER_ORDER_DATE + "_from", orderDateFromDefault);
		result.Add(Constants.FIELD_ORDER_ORDER_DATE + "_to", orderDateToDefault);
		result.Add(Constants.FIELD_ORDER_ORDER_DATE + "_use_index_hint", null);
		result.Add(Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_ORDER_SHIPPING_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_SHIPPING_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_ORDER_SHIPPED_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_SHIPPED_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_ORDER_DELIVERING_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_DELIVERING_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_ORDER_CANCEL_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_CANCEL_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_DEMAND_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_DEMAND_DATE + "_to", null);
		if (ddlOrderUpdateDateStatus.SelectedValue.Length != 0)
		{
			// ステータス更新日(From)
			var dateFrom = string.Format(
				"{0} {1}",
				(string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM],
				(string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM]);

			if (Validator.IsDate((string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM]))
			{
				result[ddlOrderUpdateDateStatus.SelectedValue + "_from"] = DateTime.Parse(dateFrom);
			}

			// ステータス更新日(To)
			var dateTo = string.Format(
				"{0} {1}",
				(string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO],
				(string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO]);

			if (Validator.IsDate((string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO]))
			{
				result[ddlOrderUpdateDateStatus.SelectedValue + "_to"] = DateTime.Parse(dateTo).AddSeconds(1);
			}
		}

		if ((result[Constants.FIELD_ORDER_ORDER_DATE + "_from"] != null)
			|| (result[Constants.FIELD_ORDER_ORDER_DATE + "_to"] != null))
		{
			result[Constants.FIELD_ORDER_ORDER_DATE + "_use_index_hint"] = "1";
		}

		// デフォルトのままの場合、注文日を対象から外す
		result[Constants.FIELD_ORDER_ORDER_DATE + "_from_to"] =
			(((DateTime)result[Constants.FIELD_ORDER_ORDER_DATE + "_from"] == orderDateFromDefault)
				&& ((DateTime)result[Constants.FIELD_ORDER_ORDER_DATE + "_to"] == orderDateToDefault))
			? null
			: "1";

		return result;
	}

	/// <summary>
	/// 受注情報一覧の表示設定を確認して、受注情報一覧に表示する項目のみを設定されている表示順のリストを作成する
	/// </summary>
	private void BindDispSetting()
	{
		var dispOrders = new ManagerListDispSettingService()
			.GetForDispSettingItemNotIncludedOrderId(this.LoginOperatorShopId,
				Constants.FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERSTOREPICKUP);

		rManagerListDispSetting.DataSource = dispOrders;
		rManagerListDispSetting.DataBind();
	}

	/// <summary>
	/// データバインド用注文情報詳細URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>注文情報詳細URL</returns>
	protected string StorePickUpOrderDetailUrl(string orderId)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_STOREPICKUP_ORDER_DETAIL)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, HttpUtility.UrlEncode(orderId))
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// データバインド用注文一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="pageNo">表示開始記事番号</param>
	/// <returns>注文一覧遷移URL</returns>
	protected string StorePickUpOrderList(Hashtable htSearch, int pageNo)
	{
		return StorePickUpOrderList(htSearch, false)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, pageNo.ToString())
			.CreateUrl();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		var url = StorePickUpOrderList(GetSearchInfoFromControl(), 1);

		// Check Max Length Url
		CheckMaxLengthUrl(url);

		Response.Redirect(url);
	}

	/// <summary>
	/// Calculate search rowspan
	/// </summary>
	/// <returns>Number of rowspan</returns>
	protected int CalculateSearchRowSpan()
	{
		var result = DEFAULT_SEARCH_ROWSPAN
			+ ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 1 : 0)
			+ (Constants.W2MP_AFFILIATE_OPTION_ENABLED ? 1 : 0)
			+ (Constants.FIXEDPURCHASE_OPTION_ENABLED ? 1 : 0)
			+ (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 1 : 0)
			+ (Constants.URERU_AD_IMPORT_ENABLED ? 1 : 0)
			+ (Constants.RECEIPT_OPTION_ENABLED ? 1 : 0)
			+ (Constants.DISPLAY_CORPORATION_ENABLED ? 1 : 0)
			+ (OrderCommon.CanDisplayInvoiceBundle() ? 1 : 0)
			+ (Constants.ORDER_EXTEND_OPTION_ENABLED ? 1 : 0);

		return result;
	}

	/// <summary>
	/// 受注情報一覧表示で連携OPを確認する
	/// </summary>
	/// <param name="dispColumnName"></param>
	/// <returns>True if the linked OP in the store pick up order information list display, otherwise false</returns>
	protected bool IsOptionCooperation(string dispColumnName)
	{
		switch (dispColumnName)
		{
			case Constants.FIELD_USER_MALL_ID:
				if ((Constants.MALLCOOPERATION_OPTION_ENABLED == false)
					|| (Constants.URERU_AD_IMPORT_ENABLED == false)) return false;
				break;

			case Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS:
			case Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS:
				if (Constants.REALSTOCK_OPTION_ENABLED == false) return false;
				break;

			case Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG:
				if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED == false) return false;
				break;

			case Constants.FIELD_ORDER_GIFT_FLG:
				if (Constants.GIFTORDER_OPTION_ENABLED == false) return false;
				break;
		}

		return true;
	}

	#region プロパティ
	/// <summary>注文拡張ステータス一覧</summary>
	protected Dictionary<string, string> DisplayExtendStatusList
	{
		get
		{
			if (ViewState[Constants.FLG_DISPLAY_EXTEND_STATUS_LIST_VIEW] == null)
			{
				ViewState[Constants.FLG_DISPLAY_EXTEND_STATUS_LIST_VIEW] = new Dictionary<string, string>();
			}

			return (Dictionary<string, string>)ViewState[Constants.FLG_DISPLAY_EXTEND_STATUS_LIST_VIEW];
		}
	}
	/// <summary>初回表示</summary>
	protected bool IsFirstView
	{
		get { return (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_FIRSTVIEW]) == false); }
	}
	/// <summary>項目表示設定</summary>
	protected ManagerListDispSettingModel[] ColumnDisplaySettings
	{
		get
		{
			if (ViewState[Constants.FLG_COLUMNDISPLAY_SETTINGS_VIEW] == null)
			{
				ViewState[Constants.FLG_COLUMNDISPLAY_SETTINGS_VIEW] = new ManagerListDispSettingService()
					.GetAllByDispSettingKbn(
						this.LoginOperatorShopId,
						Constants.FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERSTOREPICKUP)
					.OrderBy(x => x.DispOrder)
					.ToArray();
			}

			return (ManagerListDispSettingModel[])ViewState[Constants.FLG_COLUMNDISPLAY_SETTINGS_VIEW];
		}
	}
	/// <summary>項目表示設定</summary>
	protected List<OperatorAuthorityModel> OperatorAuthoritys
	{
		get
		{
			return DomainFacade.Instance.OperatorAuthorityService.Get(
				this.LoginOperatorShopId,
				this.LoginOperatorId);
		}
	}
	#endregion
}
