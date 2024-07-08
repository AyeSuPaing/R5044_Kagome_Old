/*
=========================================================================================================
  Module      : 更新履歴情報情報確認ページ処理(UpdateHistoryConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class Form_UpdateHistory_UpdateHistoryConfirm : UpdateHistoryPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 初期化
			Initialize();

			// 検索結果セット
			SetUpdateHistoryList();

			// 更新履歴検索一覧表示
			DisplaySearchUpdateHistoryList(this.UpdateHistoryList);

			// 詳細表示
			DisplayUpdateHistoryConfirm();
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		var updateHistoryKbns = cblMaster.Items.Cast<ListItem>().Where(li => li.Selected).Select(li => li.Value);
		var url = CreateUpdateHistoryConfirmUrl(string.Join(",", updateHistoryKbns), this.RequestUserId, tbMasterId.Text);
		Response.Redirect(url);
	}

	/// <summary>
	/// 詳細表示チェックボックスクリック（更新履歴検索）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbDisplayDetailUpdateHistory_CheckedChanged(object sender, EventArgs e)
	{
		// 更新履歴情報（一覧詳細用）取得
		var updateHistoryBeforeAndAfterList = new List<UpdateHistoryInput>();
		if (this.UpdateHistoryBeforeAndAfterList != null)
		{
			updateHistoryBeforeAndAfterList = this.UpdateHistoryBeforeAndAfterList.ToList();
		}

		// 選択中の更新履歴情報追加
		var addUpdateHistoryNoList = new List<long>();
		foreach (RepeaterItem item in rSearchUpdateHistoryList.Items)
		{
			var updateHistoryNo = long.Parse(((HiddenField)item.FindControl("hfUpdateHhistoryNo")).Value);
			// 選択されたものをリストに格納
			if (((CheckBox)item.FindControl("cbDisplayDetailUpdateHistory")).Checked)
			{
				if (updateHistoryBeforeAndAfterList.Any(uh => uh.UpdateHistoryNo == updateHistoryNo) == false)
				{
					var condition = new UpdateHistoryBeforeAndAfterSearchCondition
					{
						UpdateHistoryNo = updateHistoryNo,
						UpdateHistoryKbn = ((HiddenField)item.FindControl("hfUpdateHistoryKbn")).Value,
						UserId = ((HiddenField)item.FindControl("hfUserId")).Value,
						MasterId = ((HiddenField)item.FindControl("hfMasterId")).Value
					};
					var searchResult = new UpdateHistoryService().BeforeAfterSearch(condition);
					var updateHistory = new UpdateHistoryInput(searchResult);
					updateHistoryBeforeAndAfterList.Add(ChangeDateItems(updateHistory));
				}
				addUpdateHistoryNoList.Add(updateHistoryNo);
			}
		}

		// 未選択の更新履歴情報削除
		foreach (var updateHistoryNo in updateHistoryBeforeAndAfterList.Select(uh => uh.UpdateHistoryNo).ToArray())
		{
			if (addUpdateHistoryNoList.Contains(updateHistoryNo) == false)
			{
				updateHistoryBeforeAndAfterList
					.Remove(updateHistoryBeforeAndAfterList.First(uh => uh.UpdateHistoryNo == updateHistoryNo));
			}
		}

		// 更新履歴詳細（一覧詳細用）表示
		this.UpdateHistoryBeforeAndAfterList = updateHistoryBeforeAndAfterList.OrderByDescending(uh => uh.UpdateHistoryNo).ToArray();
		if (this.UpdateHistoryBeforeAndAfterList.Length != 0)
		{
			rDetailUpdateHistoryList.Visible = true;
			trListError.Visible = false;
			rDetailUpdateHistoryList.DataSource = this.UpdateHistoryBeforeAndAfterList;
			rDetailUpdateHistoryList.DataBind();
		}
		else
		{
			rDetailUpdateHistoryList.Visible = false;
			trListError.Visible = true;
		}

		// 詳細表示が1件の場合、各マスタ情報を表示する
		this.DisplayUpdateHistory = new UpdateHistoryInput();
		if (this.UpdateHistoryBeforeAndAfterList.Length == 1)
		{
			if (this.UpdateHistoryBeforeAndAfterList[0].UpdateHistoryKbn == Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_ORDER)
			{
				this.DisplayUpdateHistory = this.UpdateHistoryBeforeAndAfterList[0];
			}
			if (this.UpdateHistoryBeforeAndAfterList[0].UpdateHistoryKbn == Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_FIXEDPURCHASE)
			{
				this.DisplayUpdateHistory = this.UpdateHistoryBeforeAndAfterList[0];
			}
			DisplayUpdateHistoryConfirm();
		}
	}

	/// <summary>
	/// 全表示・差分表示切替
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rDetailUpdateHistoryList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var updateHistoryBeforeAndAfterList = this.UpdateHistoryBeforeAndAfterList.ToList();
		foreach (var updateHistory in updateHistoryBeforeAndAfterList)
		{
			if (updateHistory.UpdateHistoryNo == long.Parse(e.CommandArgument.ToString()))
			{
				updateHistory.IsOpenAllField = (updateHistory.IsOpenAllField == false);
			}
		}
		this.UpdateHistoryBeforeAndAfterList = updateHistoryBeforeAndAfterList.OrderByDescending(uh => uh.UpdateHistoryNo).ToArray();
		rDetailUpdateHistoryList.DataSource = this.UpdateHistoryBeforeAndAfterList;
		rDetailUpdateHistoryList.DataBind();
	}

	/// <summary>
	/// 更新履歴詳細のパスワード表示制御
	/// </summary>
	/// <param name="item">更新前後の履歴詳細</param>
	/// <returns>更新前後の履歴詳細</returns>
	protected BeforeAndAfterUpdateData DisplayValue(BeforeAndAfterUpdateData item)
	{
		var menuPath = Constants.PATH_ROOT + Constants.MENU_PATH_LARGE_USER;
		if ((item.FieldName == Constants.FIELD_USER_PASSWORD) && (MenuUtility.HasAuthority(this.LoginOperatorMenu, menuPath, Constants.KBN_MENU_FUNCTION_USER_PASSWORD_DISPLAY) == false))
		{
			item.Before = StringUtility.ChangeToAster(item.Before);
			item.After = StringUtility.ChangeToAster(item.After);
		}
		return item;
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// マスタ
		cblMaster.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_UPDATEHISTORY, Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_KBN));
	}

	/// <summary>
	/// 詳細表示
	/// </summary>
	private void DisplayUpdateHistoryConfirm()
	{
		// ユーザー情報セット
		var user = new UserService().Get(this.RequestUserId);
		if (user != null)
		{
			lUserId.Text = WebSanitizer.HtmlEncode(user.UserId);
			this.UserDetailUrl = CreateUserDetailUrl(user.UserId);
			lName.Text = WebSanitizer.HtmlEncode(user.Name);
			lNameKana.Text = WebSanitizer.HtmlEncode(user.NameKana);
			lZip.Text = WebSanitizer.HtmlEncode("〒" + user.Zip);
			lAddr.Text = WebSanitizer.HtmlEncode(user.Addr);
			lTel1.Text = WebSanitizer.HtmlEncode((user.Tel1 != "") ? user.Tel1 : "-");
			lTel2.Text = WebSanitizer.HtmlEncode((user.Tel2 != "") ? user.Tel2 : "-");
			lMailAddr.Text = WebSanitizer.HtmlEncode((user.MailAddr != "") ? user.MailAddr : "-");
			lMailAddr2.Text = WebSanitizer.HtmlEncode((user.MailAddr2 != "") ? user.MailAddr2 : "-");

			if (IsCountryJp(user.AddrCountryIsoCode) == false)
			{
				lZip.Text = string.Empty;
				var addr = string.Format("{0} {1} {2} {3} {4} {5}"
					, user.Addr2
					, user.Addr3
					, user.Addr4
					, user.Addr5
					, user.Zip
					, user.AddrCountryName);
				lAddr.Text = WebSanitizer.HtmlEncode(addr);
			}
		}

		// 注文情報セット
		if (this.IsUpdateHistoryKbnOrder)
		{
			var order = new OrderService().Get(this.DisplayUpdateHistory.MasterId);
			if (order != null)
			{
				lOrderId.Text = WebSanitizer.HtmlEncode(order.OrderId);
				lSubscriptionBoxCourseIdOrder.Text = WebSanitizer.HtmlEncode(order.SubscriptionBoxCourseId);
				this.OrderDetailUrl = CreateOrderDetailUrl(order.OrderId);
				lOrderKbn.Text = WebSanitizer.HtmlEncode(order.OrderKbnText);
				lOwnerKbn.Text = WebSanitizer.HtmlEncode(order.Owner.OwnerKbnText);
				lOrderDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						order.OrderDate,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lOrderStatus.Text = WebSanitizer.HtmlEncode(order.OrderStatusText);
				lOrderPaymentDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						order.OrderPaymentDate,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
						"-"));
				lOrderPaymentStatus.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, order.OrderPaymentStatus));
				lOrderPriceTotal.Text = WebSanitizer.HtmlEncode(order.OrderPriceTotal.ToPriceString(true));

				var shipping = new ShopShippingService().Get(order.ShopId, order.ShippingId);
				lShippingId.Text =
					WebSanitizer.HtmlEncode(
						string.Format("[{0}]{1}",
							order.ShippingId,
							shipping != null ? shipping.ShopShippingName : "-"
						)
					);
				var payment = new PaymentService().Get(order.ShopId, order.OrderPaymentKbn);
				if (payment != null)
				{
					var paymentName = payment.PaymentName
						+ string.Format("({0}払い)",
						ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, order.CardInstallmentsCode));
					lOrderPaymentKbn.Text = WebSanitizer.HtmlEncode(paymentName);
				}
				lSubscriptionBoxCourseIdOrder.Text = WebSanitizer.HtmlEncode(order.SubscriptionBoxCourseId);
			}
		}

		// 定期情報セット
		if (this.IsUpdateHistoryKbnFixedPurchase)
		{
			var fixedPurchase = new FixedPurchaseService().Get(this.DisplayUpdateHistory.MasterId);
			if (fixedPurchase != null)
			{
				lFixedPurchaseId.Text = WebSanitizer.HtmlEncode(fixedPurchase.FixedPurchaseId);
				this.FixedPurchaseDetailUrl = FixedPurchasePage.CreateFixedPurchaseDetailUrl(fixedPurchase.FixedPurchaseId, true);
				lFixedPurchaseSetting1.Text = WebSanitizer.HtmlEncode(OrderCommon.CreateFixedPurchaseSettingMessage(fixedPurchase));
				lFixedPurchaseDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						fixedPurchase.FixedPurchaseDateBgn,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lFixedPurchaseLastOrderDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						fixedPurchase.LastOrderDate,
						DateTimeUtility.FormatType.ShortDate2Letter));
				lFixedPurchaseOrderCount.Text = WebSanitizer.HtmlEncode(fixedPurchase.OrderCount);
				lFixedPurchaseShippedCount.Text = WebSanitizer.HtmlEncode(fixedPurchase.ShippedCount);
				lFixedPurchaseStatus.Text = WebSanitizer.HtmlEncode(fixedPurchase.FixedPurchaseStatusText);
				lPaymentStatus.Text = WebSanitizer.HtmlEncode(fixedPurchase.PaymentStatusText);
				lNextShippingDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						fixedPurchase.NextShippingDate,
						DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));
				lNextNextShippingDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						fixedPurchase.NextNextShippingDate,
						DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));
				lSubscriptionBoxCourseId.Text = WebSanitizer.HtmlEncode(fixedPurchase.SubscriptionBoxCourseId);
			}
		}
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <returns>検索条件</returns>
	private UpdateHistoryListSearchCondition CreateSearchCondition()
	{
		var updateHistoryKbn = this.RequestUpdateHistoryKbn.Split(',');
		return new UpdateHistoryListSearchCondition
		{
			UpdateHistoryKbn1 = updateHistoryKbn.Length >= 1 ? updateHistoryKbn[0] : "",
			UpdateHistoryKbn2 = updateHistoryKbn.Length >= 2 ? updateHistoryKbn[1] : "",
			UpdateHistoryKbn3 = updateHistoryKbn.Length >= 3 ? updateHistoryKbn[2] : "",
			UserId = this.RequestUserId,
			MasterId = this.RequestMasterId
		};
	}

	/// <summary>
	/// 検索結果セット
	/// </summary>
	private void SetUpdateHistoryList()
	{
		// 検索フォームにパラメータをセット
		var updateHistoryKbn = this.RequestUpdateHistoryKbn.Split(',');
		foreach (ListItem item in cblMaster.Items)
		{
			if (updateHistoryKbn.Contains(item.Value))
			{
				item.Selected = true;
			}
		}
		tbMasterId.Text = this.RequestMasterId;

		// 検索結果セット
		var searchCondition = CreateSearchCondition();
		var result = new UpdateHistoryService().Search(searchCondition);
		this.UpdateHistoryList = result.Select(r => new UpdateHistorySearchInput(r)).ToArray();
	}

	/// <summary>
	/// 更新履歴検索一覧表示
	/// </summary>
	private void DisplaySearchUpdateHistoryList(UpdateHistorySearchInput[] searchUpdateHistoryList)
	{
		if (searchUpdateHistoryList.Length != 0)
		{
			trDisplayUpdateHistoryListError.Visible = false;
		}
		else
		{
			trDisplayUpdateHistoryListError.Visible = true;
		}
		this.DisplayUpdateHistory = new UpdateHistoryInput();
		rSearchUpdateHistoryList.DataSource = searchUpdateHistoryList;
		rSearchUpdateHistoryList.DataBind();
	}

	/// <summary>
	/// ユーザ詳細URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <returns>ユーザ詳細URL</returns>
	public static string CreateUserDetailUrl(string userId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CONFIRM_POPUP)
		.AddParam(Constants.REQUEST_KEY_USER_ID, userId)
		.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
		.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP).CreateUrl();
		return url;
	}

	/// <summary>
	/// 受注詳細URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateOrderDetailUrl(string orderId)
	{
		return OrderPage.CreateOrderDetailUrl(
			orderId,
			true,
			false,
			null);
	}

	/// <summary>
	/// 日付項目変換
	/// </summary>
	/// <param name="data">更新履歴情報</param>
	/// <returns>グローバル対応のための各日付項目が変更された更新履歴情報</returns>
	private UpdateHistoryInput ChangeDateItems(UpdateHistoryInput data)
	{
		// グローバル対応なしの時は日付変換変換を行わない
		if (Constants.GLOBAL_OPTION_ENABLE == false) return data;

		// 各日付項目を変換する
		foreach (var item in data.BeforeAndAfterUpdateDataList)
		{
			var isNotDateType = false;
			var formatType = DateTimeUtility.FormatType.ShortDate2Letter;
			switch (item.FieldName)
			{
				case Constants.FIELD_USER_BIRTH:
					formatType = DateTimeUtility.FormatType.LongDate2Letter;
					break;

				case Constants.FIELD_ORDER_ORDER_DATE:
				case Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE:
				case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN:
				case Constants.FIELD_FIXEDPURCHASE_DATE_CREATED:
				case Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED:
					formatType = DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter;
					break;

				case Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE:
				case Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE:
				case Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE:
				case Constants.FIELD_ORDER_ORDER_SHIPPED_DATE:
				case Constants.FIELD_ORDER_ORDER_DELIVERING_DATE:
				case Constants.FIELD_ORDER_ORDER_RETURN_DATE:
				case Constants.FIELD_ORDER_ORDER_PAYMENT_DATE:
				case Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE:
				case Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE:
				case Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE:
				case Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE:
				case Constants.FIELD_ORDER_ORDER_CANCEL_DATE:
				case Constants.FIELD_ORDER_DEMAND_DATE:
				case Constants.FIELD_USERPOINT_POINT_EXP:
				case Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE:
				case Constants.FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE:
				case Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE:
					formatType = DateTimeUtility.FormatType.ShortDate2Letter;
					break;

				case Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE:
				case Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE:
					formatType = DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter;
					break;

				case Constants.FIELD_ORDER_STOREPICKUP_STATUS:
					item.Before = ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.FIELD_ORDER_STOREPICKUP_STATUS,
						item.Before);
					item.After = ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.FIELD_ORDER_STOREPICKUP_STATUS,
						item.After);
					isNotDateType = true;
					break;

				default:
					isNotDateType = true;
					break;
			}
			if (isNotDateType) continue;
			// 日付変換
			item.Before = DateTimeUtility.ToStringForManager(item.Before, formatType);
			item.After = DateTimeUtility.ToStringForManager(item.After, formatType);
		}
		return data;
	}

	#region プロパティ
	/// <summary>リクエスト：表示選択</summary>
	protected int? RequestDisplayUpdateHistory
	{
		get
		{
			if (Request.Form["rbDisplayUpdateHistory"] == null) return null;
			return int.Parse(Request.Form["rbDisplayUpdateHistory"]);
		}
	}
	/// <summary>
	/// 更新履歴検索の選択中の更新履歴
	/// </summary>
	protected UpdateHistoryInput DisplayUpdateHistory
	{
		get { return (UpdateHistoryInput)ViewState["DisplayUpdateHistory"]; }
		set { ViewState["DisplayUpdateHistory"] = value; }
	}
	/// <summary>更新履歴区分が注文情報？</summary>
	protected bool IsUpdateHistoryKbnOrder
	{
		get { return (this.DisplayUpdateHistory != null) && (this.DisplayUpdateHistory.UpdateHistoryKbn == Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_ORDER); }
	}
	/// <summary>更新履歴区分が定期購入情報？</summary>
	protected bool IsUpdateHistoryKbnFixedPurchase
	{
		get { return (this.DisplayUpdateHistory != null) && (this.DisplayUpdateHistory.UpdateHistoryKbn == Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_FIXEDPURCHASE); }
	}
	/// <summary>検索結果（更新履歴情報）</summary>
	protected UpdateHistorySearchInput[] UpdateHistoryList
	{
		get { return (UpdateHistorySearchInput[])ViewState["UpdateHistoryList"]; }
		set { ViewState["UpdateHistoryList"] = value; }
	}
	/// <summary>更新履歴情報（一覧詳細用）</summary>
	protected UpdateHistoryInput[] UpdateHistoryBeforeAndAfterList
	{
		get { return (UpdateHistoryInput[])ViewState["UpdateHistoryBeforeAndAfterList"]; }
		set { ViewState["UpdateHistoryBeforeAndAfterList"] = value; }
	}
	/// <summary>ユーザー詳細URL</summary>
	protected string UserDetailUrl
	{
		get { return (string)ViewState["UserDetailUrl"]; }
		set { ViewState["UserDetailUrl"] = value; }
	}
	/// <summary>注文詳細URL</summary>
	protected string OrderDetailUrl
	{
		get { return (string)ViewState["OrderDetailUrl"]; }
		set { ViewState["OrderDetailUrl"] = value; }
	}
	/// <summary>定期購入詳細URL</summary>
	protected string FixedPurchaseDetailUrl
	{
		get { return (string)ViewState["FixedPurchaseDetailUrl"]; }
		set { ViewState["FixedPurchaseDetailUrl"] = value; }
	}


	#endregion
}
