/*
=========================================================================================================
  Module      : 注文情報一覧ページ処理(OrderList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.DataExporters;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Workflow;
using w2.App.Common.OrderExtend;
using w2.App.Common.Pdf.PdfCreater;
using w2.App.Common.SendMail;
using w2.App.Common.Util;
using w2.Common.Web;
using w2.Domain.ManagerListDispSetting;
using w2.Domain.UserManagementLevel;

public partial class Form_Order_OrderList : OrderPage
{
	/// <summary>ユーザーシンボルフィールド名</summary>
	protected const string FIELD_USER_SYMBOL = "symbol";

	// Flag order scheduled shipping none
	private const string FLG_ORDER_SCHEDULED_SHIPPING_NONE_OFF = "0";
	private const string FLG_ORDER_SCHEDULED_SHIPPING_NONE_ON = "1";

	/// <summary>Default search rowspan</summary>
	protected const int DEFAULT_SEARCH_ROWSPAN = 32;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable htParam = GetParametersAndSetToControl();

			if (this.IsNotSearchDefault) return;

			// 検索情報保持(編集で利用)
			Session[Constants.SESSIONPARAM_KEY_ORDER_SEARCH_INFO] = htParam;

			int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

			//------------------------------------------------------
			// 注文件数取得
			//------------------------------------------------------
			var searchSqlInfo = GetSearchSqlInfo(htParam, true);
			var totalOrderCounts = GetOrderCounts(searchSqlInfo);
			Session[Constants.SESSIONPARAM_KEY_ORDER_TOTAL_COUNT] = totalOrderCounts;
			if (totalOrderCounts > 0)
			{
				// 表示件数に基づいてページャ数を計算する
				if ((totalOrderCounts % Constants.CONST_DISP_CONTENTS_DEFAULT_LIST == 0)
					&& (totalOrderCounts / Constants.CONST_DISP_CONTENTS_DEFAULT_LIST < iCurrentPageNumber))
				{
					iCurrentPageNumber = totalOrderCounts / Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;
				}
				// 検索条件に係る項目の編集によりページが減った場合の対応
				if (totalOrderCounts / Constants.CONST_DISP_CONTENTS_DEFAULT_LIST < iCurrentPageNumber)
				{
					iCurrentPageNumber = (totalOrderCounts / Constants.CONST_DISP_CONTENTS_DEFAULT_LIST) + 1;
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

			// 領収書対応の場合、検索条件を合わせて、領収書希望ありの受注件数を取得
			int totalHasReceiptOrderCounts = 0;
			if (Constants.RECEIPT_OPTION_ENABLED)
			{
				switch (StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_ORDER_RECEIPT_FLG]))
				{
					case Constants.FLG_ORDER_RECEIPT_FLG_ON:
						totalHasReceiptOrderCounts = totalOrderCounts;
						break;

					case Constants.FLG_ORDER_RECEIPT_FLG_OFF:
						totalHasReceiptOrderCounts = 0;
						break;

					default:
						var hasReceiptOrderSearchParam = (Hashtable)searchSqlInfo.Clone();
						totalHasReceiptOrderCounts = GetOrderCounts(hasReceiptOrderSearchParam, true);
						break;
				}
			}

			//------------------------------------------------------
			// 注文件数による表示制御
			//------------------------------------------------------
			trListError.Visible = false;

			// PDFダウンロードリンク切り替え
			// ピッキングリストについては件数取得にコストがかかるため非同期のみ
			lbPdfOutput.Visible = (totalOrderCounts <= Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD);
			lbPdfOutputUnsync.Visible = (totalOrderCounts > Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD);
			lbPdfOutputOrderStatement.Visible = (totalOrderCounts <= Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD);
			lbPdfOutputOrderStatementUnsync.Visible = (totalOrderCounts > Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD);
			lbTotalPickingListOutputUnsync.Visible = Constants.PDF_OUTPUT_PICKINGLIST_ENABLED;
			lbPdfOutputReceipt.Visible = (Constants.RECEIPT_OPTION_ENABLED
				&& (totalHasReceiptOrderCounts <= Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD));
			lbPdfOutputReceipt.Enabled = (totalHasReceiptOrderCounts > 0);
			lbPdfOutputReceiptUnsync.Visible = (Constants.RECEIPT_OPTION_ENABLED
				&& (totalHasReceiptOrderCounts > Constants.CONST_COUNT_PDF_DIRECTDOUNLOAD));

			// データ総数が定数以上の場合警告文を表示、以下の場合は条件を絞って注文一覧を表示
			if (totalOrderCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				trListError.Visible = true;

				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST);
				tdErrorMessage.InnerHtml = tdErrorMessage.InnerHtml.Replace("@@ 1 @@", StringUtility.ToNumeric(totalOrderCounts));
				tdErrorMessage.InnerHtml = tdErrorMessage.InnerHtml.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				return;	// 一定数以上であればここで抜ける
			}

			// 表示件数が０の場合、メッセージを表示して抜ける
			if (totalOrderCounts == 0)
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

				return;
			}

			//------------------------------------------------------
			// 注文一覧取得・データバインド
			//------------------------------------------------------
			var orders = GetOrderListWithUserSymbols(GetSearchSqlInfo(htParam, true), iCurrentPageNumber);

			rList.DataSource = orders;
			rList.DataBind();

			// Data source set of fixed table
			rTableFixColumn.DataSource = orders;
			rTableFixColumn.DataBind();

			// 表示設定の確認
			BindDispSetting();

			//------------------------------------------------------
			// 基幹システム連携用データのダウンロードアンカーテキストを取得・データバインド
			//------------------------------------------------------
			rDownloadAnchorTextList.DataSource = DataExporterCreater.GetDownloadAnchorTextForCommerceManager(Constants.PROJECT_NO, Constants.ExportKbn.OrderList);
			rDownloadAnchorTextList.DataBind();

			//------------------------------------------------------
			// その他表示制御
			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			string strNextUrl = CreateOrderListUrl(htParam, false);
			lbPager1.Text = WebPager.CreateDefaultListPager(totalOrderCounts, iCurrentPageNumber, strNextUrl);

			// 権限制御（ダウンロードリンク表示制御）
			lbPdfOutput.Visible &= MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_PDF_OUTPUT_DL);
			lbPdfOutputUnsync.Visible &= MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_PDF_OUTPUT_DL);
			lbTotalPickingListOutputUnsync.Visible &= Constants.PDF_OUTPUT_PICKINGLIST_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_PICKING_LIST_DL);
			rShippingLabelExport.Visible = MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_FILE_EXPORT_DL);
			lbPdfOutputOrderStatement.Visible &= MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_STATEMENT_DL);
			lbPdfOutputOrderStatementUnsync.Visible &= MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_STATEMENT_DL);
			lbPdfOutputReceipt.Visible &= MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_RECEIPT_DL);
			lbPdfOutputReceiptUnsync.Visible &= MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_RECEIPT_DL);

			//------------------------------------------------------
			// ダウンロード待機ポップアップ設定
			//------------------------------------------------------
			// ダウンロードページパス
			StringBuilder sbInvoicePath = new StringBuilder();
			sbInvoicePath.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_DOWNLOAD_WAIT);
			sbInvoicePath.Append("?").Append("targaturl").Append("=").Append(Server.UrlEncode(Constants.PATH_ROOT + Constants.PATH_CONTENTS + "Invoice/"));
			sbInvoicePath.Append("&").Append("sid").Append("=").Append(Server.UrlEncode(Session.SessionID));

			// 納品書
			int iFileNums = 1;
			int iOrderCounts = 1;
			if (totalOrderCounts > OrderInvoiceCreater.CONST_OUTPUT_ORDER_MAX_COUNT)
			{
				iFileNums = (int)System.Math.Ceiling(decimal.Parse(totalOrderCounts.ToString()) / decimal.Parse(OrderInvoiceCreater.CONST_OUTPUT_ORDER_MAX_COUNT.ToString()));
				iOrderCounts = OrderInvoiceCreater.CONST_OUTPUT_ORDER_MAX_COUNT;
			}
			else
			{
				iOrderCounts = totalOrderCounts;
			}
			lbPdfOutputUnsync.OnClientClick = "if (window.confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PDF_OUTPUT_UNSYNC_CONFIRM)
					.Replace("@@ 1 @@", iOrderCounts.ToString())
					.Replace("@@ 2 @@", iFileNums.ToString())
				+ "')){ open_window('"
				+ sbInvoicePath.ToString()
				+ "','pdfoutput','width=850,height=415,top=120,left=320,status=NO,scrollbars=yes');}else{ return false; }";
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				lbPdfOutput.OnClientClick = "if (window.confirm('"
					+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PDF_OUTPUT_UNSYNC_CONFIRM_GLOBAL)
					+ "')){ open_window('" + sbInvoicePath
					+ "','pdfoutput','width=850,height=415,top=120,left=320,status=NO,scrollbars=yes');}else{ return false; }";
			}

			// 受注明細書
			int iOrderStatementFileNums = 1;
			int iOrderStatementOrderCounts = 1;
			if (totalOrderCounts > OrderStatementCreater.CONST_OUTPUT_ORDER_MAX_COUNT)
			{
				iOrderStatementFileNums = (int)System.Math.Ceiling(decimal.Parse(totalOrderCounts.ToString()) / decimal.Parse(OrderStatementCreater.CONST_OUTPUT_ORDER_MAX_COUNT.ToString()));
				iOrderStatementOrderCounts = OrderStatementCreater.CONST_OUTPUT_ORDER_MAX_COUNT;
			}
			else
			{
				iOrderStatementOrderCounts = totalOrderCounts;
			}
			lbPdfOutputOrderStatementUnsync.OnClientClick = "if (window.confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PDF_OUTPUT_ORDER_STATEMENT_UNSYNC)
					.Replace("@@ 1 @@", iOrderStatementOrderCounts.ToString())
					.Replace("@@ 2 @@", iOrderStatementFileNums.ToString())
				+ "')){ open_window('"
				+ sbInvoicePath.ToString()
				+ "','pdfoutput','width=850,height=415,top=120,left=320,status=NO,scrollbars=yes');}else{ return false; }";

			// トータルピッキングリスト
			lbTotalPickingListOutputUnsync.OnClientClick = "if (window.confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_TOTAL_PICKING_LIST_CREATED_UNSYNC)
				+ "')){ open_window('"
				+ sbInvoicePath.ToString()
				+ "','pdfoutput','width=800,height=415,top=120,left=320,status=NO,scrollbars=yes');}else{ return false; }";

			// 領収書
			if (Constants.RECEIPT_OPTION_ENABLED)
			{
				var receiptFileNums = (totalHasReceiptOrderCounts > ReceiptCreater.CONST_OUTPUT_ORDER_MAX_COUNT)
					? (int)Math.Ceiling((1m * totalHasReceiptOrderCounts) / ReceiptCreater.CONST_OUTPUT_ORDER_MAX_COUNT)
					: 1;
				var receiptOrderCounts = (totalHasReceiptOrderCounts > ReceiptCreater.CONST_OUTPUT_ORDER_MAX_COUNT)
					? ReceiptCreater.CONST_OUTPUT_ORDER_MAX_COUNT
					: totalHasReceiptOrderCounts;
				lbPdfOutputReceiptUnsync.OnClientClick = "if (window.confirm('"
					+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PDF_OUTPUT_RECEIPT_UNSYNC)
						.Replace("@@ 1 @@", receiptOrderCounts.ToString())
						.Replace("@@ 2 @@", receiptFileNums.ToString())
					+ "')){ open_window('"
					+ sbInvoicePath
					+ "','pdfoutput','width=850,height=415,top=120,left=320,status=NO,scrollbars=yes');}else{ return false; }";
			}
		}
	}

	/// <summary>
	/// 受注情報一覧の表示設定を確認して、受注情報一覧に表示する項目のみを設定されている表示順のリストを作成する
	/// </summary>
	private void BindDispSetting()
	{
		var dispOrders = new ManagerListDispSettingService()
			.GetForDispSettingItemNotIncludedOrderId(this.LoginOperatorShopId, Constants.FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERLIST);

		// Remove column store pickup status if condition not meet
		if (Constants.STORE_PICKUP_OPTION_ENABLED == false)
		{
			dispOrders = dispOrders
				.Where(ds => ds.DispColmunName != Constants.FIELD_ORDER_STOREPICKUP_STATUS)
				.ToArray();
		}

		rManagerListDispSetting.DataSource = dispOrders;
		rManagerListDispSetting.DataBind();
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
				resultColumnName = string.Format("{0}{1}：\r\n{2}", extendS, dispExtendStatus.Value, dispExtendStatus.Key.Replace(numberReplaceExtendNumber + "：", string.Empty));
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
		// 注文区分
		ddlOrderKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN))
		{
			// モバイルデータの表示OPがOFFの場合は注文区分がモバイル注文を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_ORDER_ORDER_KBN_MOBILE)) continue;
			ddlOrderKbn.Items.Add(li);
		}
		// 注文者区分
		ddlOwnerKbn.Items.Add(new ListItem("", ""));
		ddlOwnerKbn.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDEROWNER, Constants.VALUE_TEXT_KEY_ORDEROWNER_USER_KBN_ALL));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_KBN))
		{
			// モバイルデータの表示と非表示OFF時はMB_USER(モバイル会員一般)とMB_GEST(モバイルゲスト)区分を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& ((li.Value == Constants.FLG_ORDEROWNER_OWNER_KBN_MOBILE_USER)
					|| (li.Value == Constants.FLG_ORDEROWNER_OWNER_KBN_MOBILE_GUEST))) continue;
			ddlOwnerKbn.Items.Add(li);
		}
		// 並び順
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.REQUEST_KEY_SORT_KBN))
		{
			ddlSortKbn.Items.Add(li);
		}
		// 注文ステータス
		ddlOrderStatus.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS))
		{
			// 実在庫管理なしの場合は在庫引き当て済みを追加したくない。ステータス不明についても。
			if ((Constants.REALSTOCK_OPTION_ENABLED || (li.Value != Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED))
				&& (li.Value != Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN))
			{
				ddlOrderStatus.Items.Add(li);
			}
		}
		// 入金ステータス
		ddlOrderPaymentStatus.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
		{
			if (li.Value != Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_UNKNOWN)
			{
				ddlOrderPaymentStatus.Items.Add(li);
			}
		}
		// ステータス更新日ドロップダウン作成
		ddlOrderUpdateDateStatus.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, "update_status"))
		{
			// 実在庫管理なしの場合は在庫引き当て済みを追加したくない
			if (Constants.REALSTOCK_OPTION_ENABLED || (li.Value != Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE))
			{
				ddlOrderUpdateDateStatus.Items.Add(li);
			}
		}

		if (this.IsFirstView)
		{
			if (string.IsNullOrEmpty(Constants.ORDERLIST_FIRSTVIEW_ORDERSTATUS) == false)
			{
				// ステータス更新日・ステータス
				ddlOrderUpdateDateStatus.SelectedValue = Constants.ORDERLIST_FIRSTVIEW_ORDERSTATUS;
			}

			if (string.IsNullOrEmpty(Constants.ORDERLIST_FIRSTVIEW_ABSTIMESPAN) == false)
			{
				// ステータス更新日
				var timeSpan = RelativeCalendar.FromText(Constants.ORDERLIST_FIRSTVIEW_ABSTIMESPAN);
				ucOrderUpdateDatePeriod.SetPeriodDate(timeSpan.BeginTime, timeSpan.EndTime);
			}
		}
		else
		{
			ddlOrderUpdateDateStatus.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS]);
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
			ucOrderUpdateDatePeriod.SetPeriodDate(orderUpdateDateFrom, orderUpdateDateTo);
		}
		
		// 拡張ステータス：一覧部分
		DataView orderExtendStatusSettingList = GetOrderExtendStatusSettingList(this.LoginOperatorShopId);

		// 拡張ステータス：検索フォーム
		ddlOrderExtendStatusName.Items.Add(new ListItem("", ""));
		foreach (DataRowView orderExtendStatusSetting in orderExtendStatusSettingList)
		{
			ListItem li = new ListItem(
				orderExtendStatusSetting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString() + "：" + (string)orderExtendStatusSetting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME],
				orderExtendStatusSetting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString());

			ddlOrderExtendStatusName.Items.Add(li);
			this.DisplayExtendStatusList.Add(li.Text, li.Value);
		}
		ddlOrderExtendStatus.Items.Add(new ListItem("", ""));
		ddlOrderExtendStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, string.Format("{0}2", Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME)));
		trOrderExtendStatus.Visible = (this.DisplayExtendStatusList.Count > 0);
		// /拡張ステータス更新日：検索フォーム
		ddlOrderUpdateDateExtendStatus.Items.Add(new ListItem("", ""));
		foreach (DataRowView orderUpdataExtendStatusSetting in orderExtendStatusSettingList)
		{
			ListItem li = new ListItem(
				orderUpdataExtendStatusSetting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString()
				+ "："
				+ (string)orderUpdataExtendStatusSetting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME],
				orderUpdataExtendStatusSetting[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString());

			ddlOrderUpdateDateExtendStatus.Items.Add(li);
		}
		// 督促ステータス（「指定無し」以外を追加）
		if (Constants.DEMAND_OPTION_ENABLE)
		{
			tdDemandStatus.Visible = true;
			tdDemandStatusList.Visible = true;
			ddlDemandStatus.Items.Add(new ListItem("", ""));
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_DEMAND_STATUS))
			{
				if (li.Value != Constants.FLG_ORDER_DEMAND_STATUS_UNKNOWN)
				{
					ddlDemandStatus.Items.Add(li);
				}
			}
		}

		// 決済種別
		ddlOrderPaymentKbn.Items.Add(new ListItem("", ""));
		foreach (DataRowView payment in GetPaymentValidList(this.LoginOperatorShopId))
		{
			ddlOrderPaymentKbn.Items.Add(
				new ListItem(
					AbbreviateString((string)payment[Constants.FIELD_PAYMENT_PAYMENT_NAME], 12),
					(string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]));
		}
		// 外部決済ステータスリスト作成
		ddlExternalPaymentStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlExternalPaymentStatus.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS));
		// 配送区分
		ddlOrderShippingKbn.Items.Add(new ListItem("", ""));
		foreach (DataRowView shipping in GetShopShippingsAll(this.LoginOperatorShopId))
		{
			ddlOrderShippingKbn.Items.Add(
				new ListItem(
					AbbreviateString((string)shipping[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME], 8),
					(string)shipping[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]));
		}
		// 配送料の別途見積もりフラグ
		ddlSeparateEstimatesFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG))
		{
			ddlSeparateEstimatesFlg.Items.Add(li);
		}
		// 在庫引当ステータス
		ddlOrderStockReservedStatus.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS))
		{
			ddlOrderStockReservedStatus.Items.Add(li);
		}
		// 出荷ステータス
		ddlOrderShippedStatus.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS))
		{
			ddlOrderShippedStatus.Items.Add(li);
		}
		// 出荷後変更区分
		ddlShippedChangedKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN))
		{
			// 「指定無し」以外の場合に追加
			if (li.Value != Constants.FLG_ORDER_SHIPPED_CHANGED_KBN_UNKNOWN)
			{
				ddlShippedChangedKbn.Items.Add(li);
			}
		}
		// 返品交換注文
		cbReturnExchange.Checked = false;
		// 返品交換区分（「指定無し」以外を追加）
		ddlReturnExchangeKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN))
		{
			if (li.Value != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)
			{
				ddlReturnExchangeKbn.Items.Add(li);
			}
		}
		// 返品交換都合区分（「指定無し」以外を追加）
		ddlReturnExchangeReasonKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN))
		{
			if (li.Value != Constants.FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_UNKNOWN)
			{
				ddlReturnExchangeReasonKbn.Items.Add(li);
			}
		}
		// 返品交換ステータス（「指定無し」以外を追加）
		ddlOrderReturnExchangeStatus.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS))
		{
			if (li.Value != Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_UNKNOWN)
			{
				ddlOrderReturnExchangeStatus.Items.Add(li);
			}
		}
		// 返金ステータス（「指定無し」以外を追加）
		ddlOrderRepaymentStatus.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS))
		{
			if (li.Value != Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_UNKNOWN)
			{
				ddlOrderRepaymentStatus.Items.Add(li);
			}
		}
		// サイト
		ddlSiteName.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlSiteName.Items.Add(
			new ListItem(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SITENAME,
					Constants.VALUETEXT_PARAM_OWNSITENAME,
					Constants.FLG_USER_MALL_ID_OWN_SITE),
				Constants.FLG_USER_MALL_ID_OWN_SITE));
		foreach (var siteName in Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES)
		{
			ddlSiteName.Items.Add(
				new ListItem(
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_SITENAME,
						Constants.VALUETEXT_PARAM_OWNSITENAME,
						siteName),
					siteName));
		}
		if (Constants.MALLCOOPERATION_OPTION_ENABLED)
		{
			foreach (DataRowView mallCooperationSetting in GetMallCooperationSettingList(this.LoginOperatorShopId))
			{
				ddlSiteName.Items.Add(
					new ListItem(
						CreateSiteNameForList(
							(string)mallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID],
							(string)mallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]),
							(string)mallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]));
			}
		}
		if (Constants.GLOBAL_OPTION_ENABLE || (Constants.URERU_AD_IMPORT_ENABLED == false))
		{
			// 「つくーる」削除
			var ddlItem = ddlSiteName.Items.FindByValue(Constants.FLG_USER_MALL_ID_URERU_AD);
			ddlSiteName.Items.Remove(ddlItem);
		}

		// 定期注文
		if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
		{
			ddlFixedPurchase.Items.Add(new ListItem("", ""));
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, "fixedpurchase"))
			{
				ddlFixedPurchase.Items.Add(li);
			}
		}
		// 注文メモ
		ddlOrderMemoFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_MEMO))
		{
			ddlOrderMemoFlg.Items.Add(li);
		}
		// 管理メモ
		ddlOrderManagementMemoFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_MANAGEMENT_MEMO))
		{
			ddlOrderManagementMemoFlg.Items.Add(li);
		}
		// 配送メモ
		ddlShippingMemoFlg.Items.Add(new ListItem("", ""));
		ddlShippingMemoFlg.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_SHIPPING_MEMO));
		// 決済連携メモ
		ddlOrderPaymentMemoFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_PAYMENT_MEMO))
		{
			ddlOrderPaymentMemoFlg.Items.Add(li);
		}
		// 外部連携メモ
		ddlOrderRelationMemoFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RELATION_MEMO))
		{
			ddlOrderRelationMemoFlg.Items.Add(li);
		}
		// 商品付帯情報
		ddlProductOptionFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERITEM, Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))
		{
			ddlProductOptionFlg.Items.Add(li);
		}
		// ギフト購入
		ddlGiftFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_GIFT_FLG))
		{
			ddlGiftFlg.Items.Add(li);
		}
		// デジタルコンテンツ商品
		ddlDigitalContentsFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG))
		{
			ddlDigitalContentsFlg.Items.Add(li);
		}
		// 会員ランク
		ddlMemberRankId.Items.Add(new ListItem("", ""));
		foreach (var memberRank in MemberRankOptionUtility.GetMemberRankList())
		{
			ddlMemberRankId.Items.Add(
				new ListItem(
					AbbreviateString(memberRank.MemberRankName, 8),
					memberRank.MemberRankId));
		}
		// ユーザー管理レベルID
		ddlUserManagementLevelId.Items.Add(new ListItem("", ""));
		// ユーザー管理レベルドロップダウン作成
		var models = new UserManagementLevelService().GetAllList()
			.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId))
			.ToArray();
		ddlUserManagementLevelId.Items.AddRange(models);
		// 別出荷フラグ
		ddlAnotherShippingFlag.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG))
		{
			if ((Constants.STORE_PICKUP_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_ORDERSHIPPING_SHIPPING_STORE_PICKUP_FLG)) continue;

			ddlAnotherShippingFlag.Items.Add(li);
		}
		// 請求書同梱フラグ
		ddlInvoiceBundleFlg.Items.Add(new ListItem("", ""));
		ddlInvoiceBundleFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG));
		// User Memo Flag
		ddlUserMemoFlg.Items.Add(new ListItem("", ""));
		ddlUserMemoFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO));
		// Order AdvCode Flag
		ddlAdvCode.Items.Add(new ListItem("", ""));
		ddlAdvCode.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderSearchParam.KEY_ORDER_ADVCODE));
		// Setting default view
		divDisplayMessage.Visible = this.IsNotSearchDefault;
		divDisplayList.Visible = (divDisplayMessage.Visible == false);
		// 送り状ダウンロードリンク表示
		rShippingLabelExport.DataSource = (new OrderFileExportShippingLabel()).GetShippingLabelExportSettingList();
		rShippingLabelExport.DataBind();
		// 外部連携取込ステータス
		ddlExternalImportStatus.Items.Add(new ListItem("", ""));
		ddlExternalImportStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS));

		// モール連携ステータス
		ddlMallLinkStatus.Items.Add(new ListItem("", ""));
		ddlMallLinkStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_MALL_LINK_STATUS));

		// 配送希望日 デフォルトを含むかのチェックボックス文言
		cbOrderShippingNone.Text = string.Format(ReplaceTag("@@DispText.common_message.including@@"),
			ReplaceTag("@@DispText.shipping_date_list.none@@"));

		// Setting text for check box order schecduled shipping none
		cbOrderScheduledShippingNone.Text = string.Format(ReplaceTag("@@DispText.common_message.including@@"),
			ReplaceTag("@@DispText.shipping_date_list.none@@"));

		// 配送方法
		ddlShippingMethod.Items.Add(new ListItem("", ""));
		ddlShippingMethod.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD));

		// 領収書希望フラグ
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			ddlReceiptFlg.Items.Add(new ListItem("", ""));
			ddlReceiptFlg.Items.AddRange(
				ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG));
		}

		if (Constants.STORE_PICKUP_OPTION_ENABLED)
		{
			ddlStorePickupStatus.Items.Add(new ListItem(string.Empty, string.Empty));
			ddlStorePickupStatus.Items.AddRange(
				ValueText.GetValueItemArray(Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_STOREPICKUP_STATUS));
		}

		// Invoice Status
		ddlInvoiceStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlInvoiceStatus.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS));

		// 配送状態
		ddlShippingStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlShippingStatus.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS));

		// 配送先 都道府県
		foreach (var prefecture in Constants.STR_PREFECTURES_LIST)
		{
			cblShippingPrefectures.Items.Add(new ListItem(prefecture, prefecture));
		}

		// 注文拡張項目の設定
		ddlOrderExtendName.Items.Add(new ListItem("", ""));
		ddlOrderExtendName.Items.AddRange(DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels.Select(ues => new ListItem(ues.SettingName, ues.SettingId)).ToArray());
		ddlOrderExtendFlg.Items.Add(new ListItem("", ""));
		ddlOrderExtendFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, "order_extend_flg"));

		// 完了状態コード
		ddlShippingStatusCode.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlShippingStatusCode.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE));

		// 現在の状態
		ddlShippingCurrentStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlShippingCurrentStatus.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS));
	}

	/// <summary>
	/// 注文拡張項目選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOrderExtend_SelectedIndexChanged(object sender, EventArgs e)
	{
		InitializeOrderExtendText("", "");
	}

	/// <summary>
	/// 注文拡張項目テキストの設定
	/// </summary>
	/// <param name="defaultFlg">ありなしフラグ</param>
	/// <param name="defaultText">検索値</param>
	private void InitializeOrderExtendText(string defaultFlg, string defaultText)
	{
		ddlOrderExtendText.Items.Clear();
		tbOrderExtendText.Text = "";
		ddlOrderExtendFlg.Visible = false;
		tbOrderExtendText.Visible = false;
		ddlOrderExtendText.Visible = false;
		ddlOrderExtendFlg.SelectedValue = defaultFlg;
		if (string.IsNullOrEmpty(ddlOrderExtendName.SelectedValue)) return;

		ddlOrderExtendFlg.Visible = true;

		var orderExtendSettingModel =
			DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels.FirstOrDefault(
				o => (o.SettingId == ddlOrderExtendName.SelectedValue));

		if (orderExtendSettingModel == null) return;

		if (orderExtendSettingModel.InputType == Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT)
		{
			tbOrderExtendText.Visible = true;
			tbOrderExtendText.Text = defaultText;
		}
		else
		{
			ddlOrderExtendText.Visible = true;
			ddlOrderExtendText.Items.Add(new ListItem("", ""));
			AddItemToDropDownList(
				ddlOrderExtendText,
				OrderExtendCommon.GetListItemForManager(orderExtendSettingModel.InputDefault),
				defaultText);
		}
	}

	/// <summary>
	/// パラメタを取得してコントロールにセットする
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParametersAndSetToControl()
	{
		Hashtable result = new Hashtable();
		try
		{
			// 注文ID
			result.Add(Constants.REQUEST_KEY_ORDER_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ID]));
			tbOrderId.Text = Request[Constants.REQUEST_KEY_ORDER_ID];

			// ユーザーID
			result.Add(Constants.REQUEST_KEY_ORDER_USER_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_USER_ID]));
			tbUserId.Text = Request[Constants.REQUEST_KEY_ORDER_USER_ID];

			// 注文者名
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_NAME]));
			tbOwnerName.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_NAME];

			// 注文者名（かな）
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA]));
			tbOwnerNameKana.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA];

			// メールアドレス
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR]));
			tbOwnerMailAddr.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR];

			// 会員ランク
			result.Add(Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID]));
			ddlMemberRankId.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID];

			// 注文者郵便番号
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_ZIP, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_ZIP]));
			tbOwnerZip.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_ZIP];

			// 注文者住所
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_ADDR, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_ADDR]));
			tbOwnerAddr.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_ADDR];

			// 注文者電話番号
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_TEL, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_TEL]));
			tbOwnerTel.Text = Request[Constants.REQUEST_KEY_ORDER_OWNER_TEL];

			// 注文区分
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_KBN]));
			ddlOrderKbn.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_KBN];

			// 注文者区分
			result.Add(Constants.REQUEST_KEY_ORDER_OWNER_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_OWNER_KBN]));
			ddlOwnerKbn.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_OWNER_KBN];

			// 注文ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_STATUS]));
			ddlOrderStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_STATUS];

			// 入金ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_STATUS]));
			ddlOrderPaymentStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_STATUS];

			// 督促ステータス
			if (Constants.DEMAND_OPTION_ENABLE)
			{
				result.Add(Constants.REQUEST_KEY_ORDER_DEMAND_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_DEMAND_STATUS]));
				ddlDemandStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_DEMAND_STATUS];
			}

			// 拡張ステータス枝番
			result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_NO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_NO]));
			ddlOrderExtendStatusName.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_NO];

			// 拡張ステータス
			result.Add(
				Constants.REQUEST_KEY_ORDER_EXTEND_STATUS,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS]));
			ddlOrderExtendStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS];

			// 拡張ステータス更新日・ステータス
			result.Add(
				Constants.REQUEST_KEY_ORDER_UPDATE_DATE_EXTEND_STATUS_NO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_EXTEND_STATUS_NO]));
			ddlOrderUpdateDateExtendStatus.SelectedValue =
				Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_EXTEND_STATUS_NO];

			// 決済種別
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_KBN]));
			ddlOrderPaymentKbn.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_KBN];

			// 決済ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_STATUS]));
			ddlExternalPaymentStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_STATUS];

			// 値なしを含む ※初期表示時はチェックあり
			var externalPaymentAuthDateNone =
				StringUtility.ToValue(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_NONE], "1").ToString();
			result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_NONE, externalPaymentAuthDateNone);
			cbExternalPaymentAuthDateNone.Checked = (externalPaymentAuthDateNone == "1");

			// 配送区分
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_KBN]));
			ddlOrderShippingKbn.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_KBN];

			// 配送料の別途見積もりフラグ
			result.Add(Constants.REQUEST_KEY_ORDER_SEPARATE_ESTIMATES_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SEPARATE_ESTIMATES_FLG]));
			ddlSeparateEstimatesFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_SEPARATE_ESTIMATES_FLG];

			// 在庫引当ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_STOCKRESERVED_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_STOCKRESERVED_STATUS]));
			ddlOrderStockReservedStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_STOCKRESERVED_STATUS];

			// 出荷ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_SHIPPED_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPED_STATUS]));
			ddlOrderShippedStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPED_STATUS];

			// 決済注文ID
			result.Add(Constants.REQUEST_KEY_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_PAYMENT_ORDER_ID]));
			tbPaymentOrderId.Text = Request[Constants.REQUEST_KEY_ORDER_PAYMENT_ORDER_ID];

			// 決済取引ID
			result.Add(Constants.REQUEST_KEY_ORDER_CARD_TRAN_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_CARD_TRAN_ID]));
			tbCardTranId.Text = Request[Constants.REQUEST_KEY_ORDER_CARD_TRAN_ID];

			// 配送伝票番号
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_CHECK_NO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_CHECK_NO]));
			tbShippingCheckNo.Text = Request[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_CHECK_NO];

			// 出荷後変更区分
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPED_CHANGED_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPED_CHANGED_KBN]));
			ddlShippedChangedKbn.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_SHIPPED_CHANGED_KBN];

			// 返品交換注文
			result.Add(Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE, StringUtility.ToValue(Request[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE], "0"));
			cbReturnExchange.Checked = Request[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE] == "1";
			// 返品交換区分
			result.Add(Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_KBN]));
			ddlReturnExchangeKbn.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_KBN];
			// 返品交換都合区分
			result.Add(Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_REASON_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_REASON_KBN]));
			ddlReturnExchangeReasonKbn.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_REASON_KBN];
			// 返品交換ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_RETURN_EXCHANGE_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_RETURN_EXCHANGE_STATUS]));
			ddlOrderReturnExchangeStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_RETURN_EXCHANGE_STATUS];
			// 返金ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_REPAYMENT_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_REPAYMENT_STATUS]));
			ddlOrderRepaymentStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ORDER_REPAYMENT_STATUS];
			// 返品交換返金更新日・ステータス
			result.Add(Constants.REQUEST_KEY_REATURN_EXCHANGE_REPAYMENT_UPDATE_DATE_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_REATURN_EXCHANGE_REPAYMENT_UPDATE_DATE_STATUS]));
			ddlReturnExchangeRepaymentUpdateDateStatus.SelectedValue = Request[Constants.REQUEST_KEY_REATURN_EXCHANGE_REPAYMENT_UPDATE_DATE_STATUS];

			result.Add(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_FROM]));
			result.Add(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_TO]));

			result.Add(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_FROM]));
			result.Add(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_TO]));

			var returnExchangeRepaymentUpdateDateStatusFrom = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_FROM])
					.Replace(":", string.Empty));

			var returnExchangeRepaymentUpdateDateStatusTo = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_TO])
					.Replace(":", string.Empty));

			ucReturnExchangeRepaymentUpdateDateStatus.SetPeriodDate(
				returnExchangeRepaymentUpdateDateStatusFrom,
				returnExchangeRepaymentUpdateDateStatusTo);

			// サイト
			result.Add(Constants.REQUEST_KEY_ORDER_MALL_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MALL_ID]));
			ddlSiteName.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_MALL_ID];

			// 商品ID
			result.Add(Constants.REQUEST_KEY_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ID]));
			tbProductId.Text = Request[Constants.REQUEST_KEY_PRODUCT_ID];

			// 商品名
			result.Add(Constants.REQUEST_KEY_PRODUCT_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_NAME]));
			tbProductName.Text = Request[Constants.REQUEST_KEY_PRODUCT_NAME];

			// セットプロモーションID
			result.Add(Constants.REQUEST_KEY_ORDER_SETPROMOTION_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SETPROMOTION_ID]));
			tbSetPromotionId.Text = Request[Constants.REQUEST_KEY_ORDER_SETPROMOTION_ID];

			// ノベルティID
			result.Add(Constants.REQUEST_KEY_ORDER_NOVELTY_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_NOVELTY_ID]));
			tbNoveltyId.Text = Request[Constants.REQUEST_KEY_ORDER_NOVELTY_ID];

			// レコメンドID
			result.Add(Constants.REQUEST_KEY_ORDER_RECOMMEND_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_RECOMMEND_ID]));
			tbRecommendId.Text = Request[Constants.REQUEST_KEY_ORDER_RECOMMEND_ID];

			// 定期購買注文
			result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ID]));
			ddlFixedPurchase.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ID];

			// 定期購入回数(注文時点)
			var orderCount = 0;
			if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_FROM], out orderCount))
			{
				result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_FROM, orderCount.ToString());
				tbOrderCountFrom.Text = orderCount.ToString();
			}
			if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_TO], out orderCount))
			{
				result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_TO, orderCount.ToString());
				tbOrderCountTo.Text = orderCount.ToString();
			}

			// 定期購入回数(出荷時点)
			var shippedCount = 0;
			if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_FROM], out shippedCount))
			{
				result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_FROM, shippedCount.ToString());
				tbShippedCountFrom.Text = shippedCount.ToString();
			}
			if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_TO], out shippedCount))
			{
				result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_TO, shippedCount.ToString());
				tbShippedCountTo.Text = shippedCount.ToString();
			}

			// 購入回数(注文時点)
			var userOrderCount = 0;
			if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_ORDER_COUNT_FROM], out userOrderCount))
			{
				result.Add(Constants.REQUEST_KEY_ORDER_ORDER_COUNT_FROM, userOrderCount.ToString());
				tbOrderTotalCountFrom.Text = userOrderCount.ToString();
			}
			if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_ORDER_COUNT_TO], out userOrderCount))
			{
				result.Add(Constants.REQUEST_KEY_ORDER_ORDER_COUNT_TO, userOrderCount.ToString());
				tbOrderTotalCountTo.Text = userOrderCount.ToString();
			}

			// 注文メモ
			result.Add(Constants.REQUEST_KEY_ORDER_MEMO_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMO_FLG]));
			ddlOrderMemoFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_MEMO_FLG];

			// 管理メモ
			result.Add(Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO_FLG]));
			ddlOrderManagementMemoFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO_FLG];

			// 配送メモありなし
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO_FLG]));
			ddlShippingMemoFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO_FLG];

			// 決済連携メモ
			result.Add(Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO_FLG]));
			ddlOrderPaymentMemoFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO_FLG];

			// 外部連携メモ
			result.Add(Constants.REQUEST_KEY_ORDER_RELATION_MEMO_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_RELATION_MEMO_FLG]));
			ddlOrderRelationMemoFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_RELATION_MEMO_FLG];

			// ギフト購入フラグ
			result.Add(Constants.REQUEST_KEY_ORDER_GIFT_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_GIFT_FLG]));
			ddlGiftFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_GIFT_FLG];

			// デジタルコンテンツ商品フラグ
			result.Add(Constants.REQUEST_KEY_ORDER_DIGITAL_CONTENTS_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_DIGITAL_CONTENTS_FLG]));
			ddlDigitalContentsFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_DIGITAL_CONTENTS_FLG];

			// 企業名
			result.Add(Constants.REQUEST_KEY_ORDER_COMPANY_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_COMPANY_NAME]));
			tbCompanyName.Text = Request[Constants.REQUEST_KEY_ORDER_COMPANY_NAME];

			// ユーザー管理レベルID
			result.Add(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID]));
			ddlUserManagementLevelId.SelectedValue = Request[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID];
			// 選択したユーザー管理レベルを除いて検索するか、しないか
			result.Add(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE]));
			cbUserManagementLevelExclude.Checked = (Request[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE] == Constants.FLG_USER_USER_MANAGEMENT_LEVEL_EXCLUDE_FLG_ON);

			// 指定なしを含む
			result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_NONE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_NONE]));
			cbOrderScheduledShippingNone.Checked = (Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_NONE] == "1");

			// 指定なしを含む ※初期表示時はチェックあり
			var shippingDateNone =
				StringUtility.ToValue(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_NONE], "1").ToString();
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_NONE, shippingDateNone);
			cbOrderShippingNone.Checked = (shippingDateNone == "1");

			// 注文メモ
			result.Add(Constants.REQUEST_KEY_ORDER_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMO]));
			tbMemo.Text = Request[Constants.REQUEST_KEY_ORDER_MEMO];

			// 決済連携メモ
			result.Add(Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO]));
			tbPaymentMemo.Text = Request[Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO];

			// 管理メモ
			result.Add(Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO]));
			tbManagementMemo.Text = Request[Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO];

			// 配送メモ
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO]));
			tbShippingMemo.Text = Request[Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO];

			// 外部連携メモ
			result.Add(Constants.REQUEST_KEY_ORDER_RELATION_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_RELATION_MEMO]));
			tbRelationMemo.Text = Request[Constants.REQUEST_KEY_ORDER_RELATION_MEMO];

			// 商品付帯情報フラグ
			result.Add(Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_FLG]));
			ddlProductOptionFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_FLG];

			// 商品付帯情報
			result.Add(Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_TEXTS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_TEXTS]));
			tbProductOption.Text = Request[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_TEXTS];

			// 別出荷フラグ
			result.Add(Constants.REQUEST_KEY_ORDER_ANOTHER_SHIPPING_FLAG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ANOTHER_SHIPPING_FLAG]));
			ddlAnotherShippingFlag.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ANOTHER_SHIPPING_FLAG]);

			// 請求書同梱フラグ
			result.Add(Constants.REQUEST_KEY_ORDER_INVOICE_BUNDLE_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_INVOICE_BUNDLE_FLG]));
			ddlInvoiceBundleFlg.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_INVOICE_BUNDLE_FLG]);

			// 配送者名
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_NAME]));
			tbShippingName.Text = Request[Constants.REQUEST_KEY_ORDER_SHIPPING_NAME];

			// 配送者名（かな）
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_NAME_KANA, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_NAME_KANA]));
			tbShippingNameKana.Text = Request[Constants.REQUEST_KEY_ORDER_SHIPPING_NAME_KANA];

			// 配送先郵便番号
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_ZIP, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_ZIP]));
			tbShippingZip.Text = Request[Constants.REQUEST_KEY_ORDER_SHIPPING_ZIP];

			// 配送先住所
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_ADDR, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_ADDR]));
			tbShippingAddr.Text = Request[Constants.REQUEST_KEY_ORDER_SHIPPING_ADDR];

			// 配送者電話番号
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_TEL1, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_TEL1]));
			tbShippingTel1.Text = Request[Constants.REQUEST_KEY_ORDER_SHIPPING_TEL1];

			// 商品セールID
			result.Add(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID]));
			if (Constants.PRODUCT_SALE_OPTION_ENABLED)
			{
				tbProductSaleId.Text = Request[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID];
			}

			// クーポンコード
			result.Add(Constants.REQUEST_KEY_COUPON_CODE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPON_CODE]));
			tbCouponCode.Text = Request[Constants.REQUEST_KEY_COUPON_CODE];

			// クーポンコード（管理用名称）
			result.Add(Constants.REQUEST_KEY_COUPON_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPON_NAME]));
			tbCouponName.Text = Request[Constants.REQUEST_KEY_COUPON_NAME];

			// User Memo Flag
			result.Add(Constants.REQUEST_KEY_USER_USER_MEMO_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_USER_MEMO_FLG]));
			ddlUserMemoFlg.SelectedValue = Request[Constants.REQUEST_KEY_USER_USER_MEMO_FLG];

			// User Memo
			result.Add(Constants.REQUEST_KEY_USER_USER_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_USER_MEMO]));
			tbUserMemo.Text = Request[Constants.REQUEST_KEY_USER_USER_MEMO];

			// 領収書希望フラグ
			result.Add(
				Constants.REQUEST_KEY_ORDER_RECEIPT_FLG,
				Constants.RECEIPT_OPTION_ENABLED
					? StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_RECEIPT_FLG])
					: string.Empty);
			if (Constants.RECEIPT_OPTION_ENABLED)
			{
				ddlReceiptFlg.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_RECEIPT_FLG];
			}

			if (Constants.STORE_PICKUP_OPTION_ENABLED)
			{
				result[Constants.REQUEST_KEY_STORE_PICKUP_STATUS] = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_STORE_PICKUP_STATUS]);
				ddlStorePickupStatus.SelectedValue = Request[Constants.REQUEST_KEY_STORE_PICKUP_STATUS];
			}

			// ソート
			string sortKbn = null;
			switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_ORDER_ID_ASC:					// 注文ID/昇順
				case Constants.KBN_SORT_ORDER_ID_DESC:					// 注文ID/降順
				case Constants.KBN_SORT_ORDER_DATE_ASC:					// 注文日/昇順
				case Constants.KBN_SORT_ORDER_DATE_DESC:				// 注文日/降順
				case Constants.KBN_SORT_ORDER_DATE_CREATED_ASC:			// 作成日/昇順
				case Constants.KBN_SORT_ORDER_DATE_CREATED_DESC:		// 作成日/降順
				case Constants.KBN_SORT_ORDER_DATE_CHANGED_ASC:			// 更新日/昇順
				case Constants.KBN_SORT_ORDER_DATE_CHANGED_DESC:		// 更新日/降順
					sortKbn = Request[Constants.REQUEST_KEY_SORT_KBN].ToString();
					break;
				default:
					sortKbn = Constants.KBN_SORT_ORDER_DATE_DEFAULT;		// 注文日/降順がデフォルト
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

			// AdvCode Flag
			result.Add(Constants.REQUEST_KEY_ORDER_ADVCODE_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ADVCODE_FLG]));
			ddlAdvCode.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_ADVCODE_FLG];

			// AdvCode
			result.Add(Constants.REQUEST_KEY_ORDER_ADVCODE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ADVCODE]));
			tbAdvCode.Text = Request[Constants.REQUEST_KEY_ORDER_ADVCODE];

			// 商品同梱ID
			result.Add(Constants.REQUEST_KEY_ORDERITEM_PRODUCT_BUNDLE_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_BUNDLE_ID]));
			if (Constants.PRODUCTBUNDLE_OPTION_ENABLED)
			{
				tbProductBundleId.Text = Request[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_BUNDLE_ID];
			}

			// 外部連携取込ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_ORDER_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_ORDER_ID]));
			result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_IMPORT_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_IMPORT_STATUS]));
			if (Constants.URERU_AD_IMPORT_ENABLED)
			{
				tbExternalOrderId.Text = Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_ORDER_ID];
				ddlExternalImportStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_IMPORT_STATUS];
			}

			// モール連携ステータス
			result.Add(Constants.REQUEST_KEY_ORDER_MALL_LINK_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MALL_LINK_STATUS]));
			ddlMallLinkStatus.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_MALL_LINK_STATUS];

			// 配送方法
			result.Add(
				Constants.REQUEST_KEY_ORDER_SHIPPING_METHOD,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_METHOD]));
			ddlShippingMethod.SelectedValue = Request[Constants.REQUEST_KEY_ORDER_SHIPPING_METHOD];

			// Taiwan Order Invoice
			result.Add(Constants.REQUEST_KEY_TW_INVOICE_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TW_INVOICE_STATUS]));
			result.Add(Constants.REQUEST_KEY_TW_INVOICE_NO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TW_INVOICE_NO]));
			if (OrderCommon.DisplayTwInvoiceInfo())
			{

				ddlInvoiceStatus.SelectedValue = Request[Constants.REQUEST_KEY_TW_INVOICE_STATUS];
				tbInvoiceNo.Text = Request[Constants.REQUEST_KEY_TW_INVOICE_NO];
			}

			// 配送状態
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS]));
			ddlShippingStatus.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS]);

			// 配送先 都道府県
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_PREFECTURE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_PREFECTURE]));
			SetSearchCheckBoxValue(cblShippingPrefectures, PrefectureUtility.GetPrefectures(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_PREFECTURE])));

			ddlOrderExtendName.SelectedValue =
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_NAME]);
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_NAME]));
			InitializeOrderExtendText(
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_FLG]),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT]));
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_FLG]));
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TYPE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TYPE]));
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT]));

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

			var orderUpdateDateFrom = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM])
					.Replace(":", string.Empty));

			var orderUpdateDateTo = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO])
					.Replace(":", string.Empty));

			ucOrderUpdateDatePeriod.SetPeriodDate(
				orderUpdateDateFrom,
				orderUpdateDateTo);

			// 初回表示の場合は設定から検索項目を絞り込む
			if (this.IsFirstView)
			{
				if (string.IsNullOrEmpty(Constants.ORDERLIST_FIRSTVIEW_ORDERSTATUS) == false)
				{
					// ステータス更新日・ステータス
					result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS, Constants.ORDERLIST_FIRSTVIEW_ORDERSTATUS);
				}

				if (string.IsNullOrEmpty(Constants.ORDERLIST_FIRSTVIEW_ABSTIMESPAN) == false)
				{
					// ステータス更新日
					var timeSpan = RelativeCalendar.FromText(Constants.ORDERLIST_FIRSTVIEW_ABSTIMESPAN);
					result[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM] = timeSpan.BeginTime.Date.ToString("yyyy/MM/dd");
					result[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM] = timeSpan.BeginTime.TimeOfDay.ToString(@"hh\:mm\:ss");
					result[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO] = timeSpan.EndTime.Date.ToString("yyyy/MM/dd");
					result[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO] = new TimeSpan(23, 59, 59).ToString(@"hh\:mm\:ss");
					ucOrderUpdateDatePeriod.SetPeriodDate(timeSpan.BeginTime, timeSpan.EndTime);
				}
			}
			else
			{
				// ステータス更新日・ステータス
				result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS]));
			}
			ddlOrderUpdateDateStatus.SelectedValue = (string)result[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS];

			// Set request key order extend status update date from date to
			result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_TO]));

			// Set request key order extend status update time from time to
			result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_TO]));

			var externalStatusUpdateDateFrom = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_FROM])
					.Replace(":", string.Empty));

			var externalStatusUpdateDateTo = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_TO])
					.Replace(":", string.Empty));

			ucExtendStatusUpdateDatePeriod.SetPeriodDate(
				externalStatusUpdateDateFrom,
				externalStatusUpdateDateTo);

			// Set request key order external payment auth date from date to
			result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_TO]));

			// Set request key order external payment auth time from time to
			result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_TO]));

			var externalPaymentAuthDateFrom = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_FROM])
					.Replace(":", string.Empty));

			var externalPaymentAuthDateTo = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_TO])
					.Replace(":", string.Empty));

			ucExternalPaymentAuthDatePeriod.SetPeriodDate(
				externalPaymentAuthDateFrom,
				externalPaymentAuthDateTo);

			// Set request order schedule shipping date from date to
			result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_TO]));

			// Set request order schedule shipping time from time to
			result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_TO]));

			var orderScheduledShippingDateFrom = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_FROM])
					.Replace(":", string.Empty));

			var orderScheduledShippingDateTo = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_TO])
					.Replace(":", string.Empty));

			ucOrderScheduledShippingDatePeriod.SetPeriodDate(
				orderScheduledShippingDateFrom,
				orderScheduledShippingDateTo);

			// Set request order shipping date from date to
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_TO]));

			// Set request order shipping time from time to
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_FROM]));
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_TO]));

			var orderShippingDateFrom = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_FROM])
					.Replace(":", string.Empty));

			var orderShippingDateTo = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_TO])
					.Replace(":", string.Empty));

			ucOrderShippingDatePeriod.SetPeriodDate(orderShippingDateFrom, orderShippingDateTo);

			// 完了状態コード
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS_CODE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS_CODE]));
			ddlShippingStatusCode.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS_CODE]);

			// 現在の状態
			result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_CURRENT_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_CURRENT_STATUS]));
			ddlShippingCurrentStatus.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_CURRENT_STATUS]);
			// 頒布会コースID
			result.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID]));
			tbSubscriptionBoxCourseId.Text = Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID];
			// 頒布会コース購入回数
			var subscriptionBoxOrderCount = 0;
			if (int.TryParse(Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_FROM], out subscriptionBoxOrderCount))
			{
				result.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_FROM, subscriptionBoxOrderCount.ToString());
				tbSubscriptionBoxOrderCountFrom.Text = subscriptionBoxOrderCount.ToString();
			}
			if (int.TryParse(Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_TO], out subscriptionBoxOrderCount))
			{
				result.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_TO, subscriptionBoxOrderCount.ToString());
				tbSubscriptionBoxOrderCountTo.Text = subscriptionBoxOrderCount.ToString();
			}
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
		Hashtable result = new Hashtable();

		result.Add(Constants.REQUEST_KEY_ORDER_ID, tbOrderId.Text.Trim());												// 注文ID
		result.Add(Constants.REQUEST_KEY_ORDER_USER_ID, tbUserId.Text.Trim());	// ユーザーID
		result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME, tbOwnerName.Text.Trim());										// 注文者名
		result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA, tbOwnerNameKana.Text.Trim());	// 注文者名（かな）
		result.Add(Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR, tbOwnerMailAddr.Text.Trim());										// メールアドレス
		result.Add(Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID, ddlMemberRankId.SelectedValue);	// 会員ランク
		result.Add(Constants.REQUEST_KEY_ORDER_OWNER_ZIP, tbOwnerZip.Text.Trim());	// 注文者郵便番号
		result.Add(Constants.REQUEST_KEY_ORDER_OWNER_ADDR, tbOwnerAddr.Text.Trim());	// 注文者住所
		result.Add(Constants.REQUEST_KEY_ORDER_OWNER_TEL, tbOwnerTel.Text.Trim());	// 注文者電話番号
		result.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);												// ソート
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_KBN, ddlOrderKbn.SelectedValue);										// 注文区分
		result.Add(Constants.REQUEST_KEY_ORDER_OWNER_KBN, ddlOwnerKbn.SelectedValue);										// 注文者区分
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_STATUS, ddlOrderStatus.SelectedValue);								// 注文ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS, ddlOrderUpdateDateStatus.SelectedValue);				// ステータス更新日・ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_STATUS, ddlOrderPaymentStatus.SelectedValue);				// 入金ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_DEMAND_STATUS, ddlDemandStatus.SelectedValue);								// 督促ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_NO, ddlOrderExtendStatusName.SelectedValue);					// 拡張ステータス枝番
		result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS, ddlOrderExtendStatus.SelectedValue);						// 拡張ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_EXTEND_STATUS_NO, ddlOrderUpdateDateExtendStatus.SelectedValue);	// 拡張ステータス更新日・ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_KBN, ddlOrderPaymentKbn.SelectedValue);						// 決済区分
		result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_STATUS, ddlExternalPaymentStatus.SelectedValue);		// 外部決済ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_NONE, (cbExternalPaymentAuthDateNone.Checked) ? "1" : "");		// 最終与信日指定無し
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_KBN, ddlOrderShippingKbn.SelectedValue);					// 配送区分
		result.Add(Constants.REQUEST_KEY_ORDER_SEPARATE_ESTIMATES_FLG, ddlSeparateEstimatesFlg.SelectedValue); // 配送料の別見積もりフラグ
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_STOCKRESERVED_STATUS, ddlOrderStockReservedStatus.SelectedValue);	// 在庫引当ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_SHIPPED_STATUS, ddlOrderShippedStatus.SelectedValue);				// 出荷ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_CHECK_NO, tbShippingCheckNo.Text.Trim());					// 配送伝票番号
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPED_CHANGED_KBN, ddlShippedChangedKbn.SelectedValue);					// 出荷後変更区分
		result.Add(Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE, cbReturnExchange.Checked ? "1" : "0");	// 返品交換注文
		result.Add(Constants.REQUEST_KEY_ORDER_MALL_ID, ddlSiteName.SelectedValue);										// サイト
		result.Add(Constants.REQUEST_KEY_PRODUCT_ID, tbProductId.Text.Trim());											// 商品ID
		result.Add(Constants.REQUEST_KEY_PRODUCT_NAME, tbProductName.Text.Trim());										// 商品名
		result.Add(Constants.REQUEST_KEY_ORDER_SETPROMOTION_ID, tbSetPromotionId.Text.Trim());							// セットプロモーションID
		result.Add(Constants.REQUEST_KEY_ORDER_NOVELTY_ID, tbNoveltyId.Text.Trim());							// ノベルティID
		result.Add(Constants.REQUEST_KEY_ORDER_RECOMMEND_ID, tbRecommendId.Text.Trim());						// レコメンドID
		result.Add(Constants.REQUEST_KEY_ORDER_PAYMENT_ORDER_ID, tbPaymentOrderId.Text.Trim()); // 決済注文ID
		result.Add(Constants.REQUEST_KEY_ORDER_CARD_TRAN_ID, tbCardTranId.Text.Trim()); // 決済取引ID
		result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ID, ddlFixedPurchase.SelectedValue);							// 定期購買注文
		result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_FROM, tbOrderCountFrom.Text);		// 定期購入回数(注文時点)From
		result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_TO, tbOrderCountTo.Text);				// 定期購入回数(注文時点)To
		result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_FROM, tbShippedCountFrom.Text);	// 定期購入回数(出荷時点)From
		result.Add(Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_TO, tbShippedCountTo.Text);			// 定期購入回数(出荷時点)To
		result.Add(Constants.REQUEST_KEY_ORDER_MEMO_FLG, ddlOrderMemoFlg.SelectedValue);									// 注文メモ
		result.Add(Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO_FLG, ddlOrderManagementMemoFlg.SelectedValue);				// 管理メモ
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO_FLG, ddlShippingMemoFlg.SelectedValue);						// 配送メモ
		result.Add(Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO_FLG, ddlOrderPaymentMemoFlg.SelectedValue);					// 決済連携メモ
		result.Add(Constants.REQUEST_KEY_ORDER_RELATION_MEMO_FLG, ddlOrderRelationMemoFlg.SelectedValue);					// 外部連携メモ
		result.Add(Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_FLG, ddlProductOptionFlg.SelectedValue);					// 商品付帯情報フラグ
		result.Add(Constants.REQUEST_KEY_ORDER_GIFT_FLG, ddlGiftFlg.SelectedValue);					// ギフト購入フラグ
		result.Add(Constants.REQUEST_KEY_ORDER_DIGITAL_CONTENTS_FLG, ddlDigitalContentsFlg.SelectedValue); // デジタルコンテンツ商品フラグ
		result.Add(Constants.REQUEST_KEY_ORDER_COMPANY_NAME, tbCompanyName.Text.Trim()); // 企業名
		result.Add(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID, ddlUserManagementLevelId.SelectedValue); // 管理ランクID
		result.Add(Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE,
			(cbUserManagementLevelExclude.Checked) ? Constants.FLG_USER_USER_MANAGEMENT_LEVEL_EXCLUDE_FLG_ON : Constants.FLG_USER_USER_MANAGEMENT_LEVEL_EXCLUDE_FLG_OFF); // 選択したユーザー管理レベルを除いて検索するか、しないか

		// 以下の項目は返品交換注文にチェックされている場合のみ、条件に加える
		result.Add(Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_KBN, cbReturnExchange.Checked ? ddlReturnExchangeKbn.SelectedValue : "");					// 返品交換区分
		result.Add(Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_REASON_KBN, cbReturnExchange.Checked ? ddlReturnExchangeReasonKbn.SelectedValue : "");		// 返品交換都合区分
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_RETURN_EXCHANGE_STATUS, cbReturnExchange.Checked ? ddlOrderReturnExchangeStatus.SelectedValue : "");	// 返品交換ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_REPAYMENT_STATUS, cbReturnExchange.Checked ? ddlOrderRepaymentStatus.SelectedValue : "");			// 返金ステータス
		result.Add(Constants.REQUEST_KEY_REATURN_EXCHANGE_REPAYMENT_UPDATE_DATE_STATUS, cbReturnExchange.Checked ? ddlReturnExchangeRepaymentUpdateDateStatus.SelectedValue : "");				// 返品交換返金更新日・ステータス

		result.Add(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_FROM, cbReturnExchange.Checked ? ucReturnExchangeRepaymentUpdateDateStatus.HfStartDate.Value : string.Empty);		// Returns, Exchanges, Refunds, Update Date (From)
		result.Add(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_FROM, cbReturnExchange.Checked ? ucReturnExchangeRepaymentUpdateDateStatus.HfStartTime.Value : string.Empty);	// Returns, Exchanges, Refunds, Update Time (From)

		result.Add(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_TO, cbReturnExchange.Checked ? ucReturnExchangeRepaymentUpdateDateStatus.HfEndDate.Value : string.Empty);			// Returns, Exchanges, Refunds, Update Date (To)
		result.Add(Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_TO, cbReturnExchange.Checked ? ucReturnExchangeRepaymentUpdateDateStatus.HfEndTime.Value : string.Empty);		// Returns, Exchanges, Refunds, Update Time (To)

		result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_NONE, (cbOrderScheduledShippingNone.Checked) ? FLG_ORDER_SCHEDULED_SHIPPING_NONE_ON : FLG_ORDER_SCHEDULED_SHIPPING_NONE_OFF);	// 出荷予定日・デフォルトを含む
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_NONE, (cbOrderShippingNone.Checked) ? "1" : "");					// 配送希望日・デフォルトを含む

		result.Add(Constants.REQUEST_KEY_ORDER_MEMO, tbMemo.Text.Trim());						// 注文メモ
		result.Add(Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO, tbPaymentMemo.Text.Trim());		// 決済連携メモ
		result.Add(Constants.REQUEST_KEY_ORDER_RELATION_MEMO, tbRelationMemo.Text.Trim());		// 外部連携メモ
		result.Add(Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO, tbManagementMemo.Text.Trim());	// 管理メモ
		result.Add(Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_TEXTS, GetProductOptionSearchText());	// 商品付帯情報
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO, tbShippingMemo.Text.Trim());		// 配送メモ
		result.Add(Constants.REQUEST_KEY_ORDER_ANOTHER_SHIPPING_FLAG, ddlAnotherShippingFlag.SelectedValue);			// 別出荷フラグ

		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_NAME, tbShippingName.Text.Trim());			// 配送者名
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_NAME_KANA, tbShippingNameKana.Text.Trim());	// 配送者名（かな）
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_ZIP, tbShippingZip.Text.Trim());				// 配送先郵便番号
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_ADDR, tbShippingAddr.Text.Trim());			// 配送先住所
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_TEL1, tbShippingTel1.Text.Trim());			// 配送者電話番号
		result.Add(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID, Constants.PRODUCT_SALE_OPTION_ENABLED ? tbProductSaleId.Text.Trim() : string.Empty);	// 商品セールID
		result.Add(Constants.REQUEST_KEY_COUPON_CODE, tbCouponCode.Text.Trim());				// クーポンコード
		result.Add(Constants.REQUEST_KEY_COUPON_NAME, tbCouponName.Text.Trim());				// クーポンコード（管理用名称）
		result.Add(Constants.REQUEST_KEY_USER_USER_MEMO_FLG, ddlUserMemoFlg.SelectedValue);		// Order User Memo Flag
		result.Add(Constants.REQUEST_KEY_USER_USER_MEMO, tbUserMemo.Text.Trim());				// Order User Memo
		result.Add(Constants.REQUEST_KEY_ORDER_ADVCODE_FLG, ddlAdvCode.SelectedValue);			// Order AdvCode Flag
		result.Add(Constants.REQUEST_KEY_ORDER_ADVCODE, tbAdvCode.Text.Trim());					// Order AdvCode
		result.Add(Constants.REQUEST_KEY_ORDERITEM_PRODUCT_BUNDLE_ID, tbProductBundleId.Text.Trim());	// 商品同梱ID
		result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_ORDER_ID, tbExternalOrderId.Text.Trim());	// 外部連携受注ID
		result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_IMPORT_STATUS, ddlExternalImportStatus.SelectedValue);	// 外部連携取込ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_MALL_LINK_STATUS, ddlMallLinkStatus.SelectedValue);	// モール連携ステータス
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_METHOD, ddlShippingMethod.SelectedValue);	// 配送方法

		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_COUNT_FROM, tbOrderTotalCountFrom.Text);		// 購入回数(注文時点)From
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_COUNT_TO, tbOrderTotalCountTo.Text);				// 購入回数(注文時点)To
		result.Add(Constants.REQUEST_KEY_ORDER_RECEIPT_FLG, Constants.RECEIPT_OPTION_ENABLED ? ddlReceiptFlg.SelectedValue : string.Empty); // 領収書希望フラグ

		result.Add(Constants.REQUEST_KEY_ORDER_INVOICE_BUNDLE_FLG, ddlInvoiceBundleFlg.SelectedValue);	// 請求書同梱フラグ
		result.Add(Constants.REQUEST_KEY_TW_INVOICE_NO, OrderCommon.DisplayTwInvoiceInfo() ? tbInvoiceNo.Text.Trim() : string.Empty);					// TwInvoice No
		result.Add(Constants.REQUEST_KEY_TW_INVOICE_STATUS, OrderCommon.DisplayTwInvoiceInfo() ? ddlInvoiceStatus.SelectedValue : string.Empty);		// TwInvoice Status
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS, ddlShippingStatus.SelectedValue);	// 配送状態
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_PREFECTURE, PrefectureUtility.GetHashString(
			cblShippingPrefectures.Items
				.Cast<ListItem>()
				.Where(li => li.Selected)
				.Select(li => li.Value)
				.ToArray()));	// 配送先都道府県

		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_NAME, ddlOrderExtendName.SelectedValue);                               // ユーザー拡張項目
		result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_FLG, ddlOrderExtendFlg.SelectedValue);
		if (string.IsNullOrEmpty(ddlOrderExtendName.SelectedValue))
		{
			result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT, "");
		}
		else
		{
			var orderExtendSettingModel =
				DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels.FirstOrDefault(
					userExtend => (userExtend.SettingId == ddlOrderExtendName.SelectedValue));

			if (orderExtendSettingModel != null)
			{
				result.Add(
					Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT,
					(orderExtendSettingModel.IsInputTypeText)
						? tbOrderExtendText.Text.Trim()
						: ddlOrderExtendText.SelectedValue);
				result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TYPE, orderExtendSettingModel.InputType);
			}
			else
			{
				result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT, string.Empty);
				result.Add(Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TYPE, string.Empty);
			}
		}
		// 完了状態コード
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS_CODE, ddlShippingStatusCode.SelectedValue);
		// 現在の状態
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_CURRENT_STATUS, ddlShippingCurrentStatus.SelectedValue);

		// Set request key order update date from date to
		result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM,
			ucOrderUpdateDatePeriod.HfStartDate.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO,
			ucOrderUpdateDatePeriod.HfEndDate.Value);

		// Set request key order update update time from time to
		result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM,
			ucOrderUpdateDatePeriod.HfStartTime.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO, "23:59:59");

		// Set request key order extend status update date from date to
		result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_FROM,
			ucExtendStatusUpdateDatePeriod.HfStartDate.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_TO,
			ucExtendStatusUpdateDatePeriod.HfEndDate.Value);

		// Set request key order extend status update time from time to
		result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_FROM,
			ucExtendStatusUpdateDatePeriod.HfStartTime.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_TO,
			ucExtendStatusUpdateDatePeriod.HfEndTime.Value);

		// Set request key order external payment auth date from date to
		result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_FROM,
			ucExternalPaymentAuthDatePeriod.HfStartDate.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_TO,
			ucExternalPaymentAuthDatePeriod.HfEndDate.Value);

		// Set request key order external payment auth time from time to
		result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_FROM,
			ucExternalPaymentAuthDatePeriod.HfStartTime.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_TO,
			ucExternalPaymentAuthDatePeriod.HfEndTime.Value);

		// Set request order schedule shipping date from date to
		result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_FROM,
			ucOrderScheduledShippingDatePeriod.HfStartDate.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_TO,
			ucOrderScheduledShippingDatePeriod.HfEndDate.Value);

		// Set request order schedule shipping time from time to
		result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_FROM,
			ucOrderScheduledShippingDatePeriod.HfStartTime.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_TO,
			ucOrderScheduledShippingDatePeriod.HfEndTime.Value);

		// Set request order shipping date from date to
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_FROM,
			ucOrderShippingDatePeriod.HfStartDate.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_TO,
			ucOrderShippingDatePeriod.HfEndDate.Value);

		// Set request order shipping time from time to
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_FROM,
			ucOrderShippingDatePeriod.HfStartTime.Value);
		result.Add(Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_TO,
			ucOrderShippingDatePeriod.HfEndTime.Value);

		// 頒布会コースID
		result.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, tbSubscriptionBoxCourseId.Text.Trim());
		// 頒布会購入回数
		result.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_FROM, tbSubscriptionBoxOrderCountFrom.Text.Trim());
		result.Add(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_TO, tbSubscriptionBoxOrderCountTo.Text.Trim());

		// Store pickup status
		result[Constants.REQUEST_KEY_STORE_PICKUP_STATUS] = (Constants.STORE_PICKUP_OPTION_ENABLED)
			? ddlStorePickupStatus.SelectedValue
			: string.Empty;
		return result;
	}

	/// <summary>
	/// 入力された商品付帯情報の検索文字列を取得する
	/// </summary>
	/// <returns>商品付帯情報の検索文字列</returns>
	/// <remarks>
	/// ・入力された商品付帯情報の検索文字列の内、
	/// 区切り文字や価格部分の文字や数字を差し引いて検索文字列を返す<br/>
	/// ※区切り文字や価格部分の内容で検索出来ないようにする為
	/// </remarks>
	private string GetProductOptionSearchText()
	{
		var result = Regex.Replace(tbProductOption.Text.Trim(), @"[\d-]", string.Empty);
		result = result
			.Replace("(", string.Empty)
			.Replace("（", string.Empty)
			.Replace(")", string.Empty)
			.Replace("）", string.Empty)
			.Replace("+", string.Empty)
			.Replace("＋", string.Empty)
			.Replace(@"\", string.Empty)
			.Replace("¥", string.Empty)
			.Replace("￥", string.Empty)
			.Replace(",", string.Empty)
			.Replace("：", string.Empty)
			.Replace("　", string.Empty);
		return result;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="searchParam">検索情報</param>
	/// <param name="isOrderIdReplace">受注IDを置換するかどうか</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable searchParam, bool isOrderIdReplace = false)
	{
		var orderDateFromDefault = DateTime.Parse("1900/01/01");
		var orderDateToDefault = DateTime.Parse("2999/01/01");

		Hashtable result = new Hashtable();

		// 店舗ID
		result.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);
		// 注文ID
		var orderIdKey = isOrderIdReplace ? Constants.PARAM_REPLACEMENT_ORDER_ID_LIKE_ESCAPED : (Constants.FIELD_ORDER_ORDER_ID + "_like_escaped");
		result.Add(orderIdKey, StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_ID]));
		// ユーザーID
		result.Add(Constants.FIELD_ORDER_USER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_USER_ID]));
		// 注文者名
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_NAME]));
		// 注文者名（かな）
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA]));
		// メールアドレス
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_MAIL_ADDR]));
		// 会員ランク
		result.Add(Constants.FIELD_ORDER_MEMBER_RANK_ID, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID]));
		// 注文者郵便番号
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_ZIP + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_ZIP]));
		// 注文者住所
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_ADDR]));
		// 注文者電話番号
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_TEL]));
		// 注文区分
		result.Add(Constants.FIELD_ORDER_ORDER_KBN, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_KBN]));
		// 注文者区分
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_OWNER_KBN]));
		// 注文ステータス
		result.Add(Constants.FIELD_ORDER_ORDER_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_STATUS]));
		// ステータス更新日・ステータス
		result.Add(Constants.FIELD_ORDER_ORDER_DATE + "_status", StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_STATUS]));
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
			var dateFrom = string.Format("{0} {1}",
				(string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM],
				(string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM]);
			if (Validator.IsDate((string)searchParam[Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM]))
			{
				result[ddlOrderUpdateDateStatus.SelectedValue + "_from"] = DateTime.Parse(dateFrom);
			}
			// ステータス更新日(To)
			var dateTo = string.Format("{0} {1}",
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
		result[Constants.FIELD_ORDER_ORDER_DATE + "_from_to"]
			= (((DateTime)result[Constants.FIELD_ORDER_ORDER_DATE + "_from"] == orderDateFromDefault)
			   && ((DateTime)result[Constants.FIELD_ORDER_ORDER_DATE + "_to"] == orderDateToDefault))
				? null
				: "1";

		// 入金ステータス
		result.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_STATUS]));
		// 督促ステータス
		result.Add(Constants.FIELD_ORDER_DEMAND_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_DEMAND_STATUS]));
		// 拡張ステータス/拡張ステータス更新日を初期化１～４０
		// SqlStatementパラメタ数に合わせ、DB定義の最大値まで埋めておく
		for (int i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
		{
			result.Add(
				Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i.ToString(),
				(StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_NO]) == i.ToString()
					? StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS])
					: ""));

			result.Add(
				string.Format("{0}{1}_from", Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME, i.ToString()),
				null);
			result.Add(
				string.Format("{0}{1}_to", Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME, i.ToString()),
				null);
		}
		if (ddlOrderUpdateDateExtendStatus.SelectedValue.Length != 0)
		{
			// 拡張ステータス更新日(From)
			var dateFrom = string.Format("{0} {1}",
				(string)searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_FROM],
				(string)searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_FROM]);
			if (Validator.IsDate((string)searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_FROM]))
			{
				result[string.Format(
					"{0}{1}_from",
					Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME,
					ddlOrderUpdateDateExtendStatus.SelectedValue)] = DateTime.Parse(dateFrom);
			}
			// 拡張ステータス更新日(To)
			var dateTo = string.Format("{0} {1}",
				(string)searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_TO],
				(string)searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_TO]);
			if (Validator.IsDate((string)searchParam[Constants.REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_TO]))
			{
				result[string.Format(
					"{0}{1}_to",
					Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME,
					ddlOrderUpdateDateExtendStatus.SelectedValue)] =
				DateTime.Parse(dateTo).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
			}
		}
		// 決済種別
		result.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_KBN]));
		// 外部決済ステータス
		result.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_STATUS]));
		// 最終与信日
		result.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE + "_from", null);
		var externalPaymetDateFrom = string.Format("{0} {1}",
			(string)searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_FROM],
			(string)searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_FROM]);
		if (Validator.IsDate(externalPaymetDateFrom))
		{
			result[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE + "_from"] = DateTime.Parse(externalPaymetDateFrom);
		}
		result.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE + "_to", null);
		var externalPaymetDateTo = string.Format("{0} {1}",
			(string)searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_TO],
			(string)searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_TO]);
		if (Validator.IsDate(externalPaymetDateTo))
		{
			result[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE + "_to"] = DateTime.Parse(externalPaymetDateTo).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
		}
		// 最終与信日指定なし
		result.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE + "_none", StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_NONE]));
		// 配送区分
		result.Add(Constants.FIELD_ORDER_SHIPPING_ID, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_KBN]));
		// 配送料の別途見積もりフラグ
		result.Add(Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SEPARATE_ESTIMATES_FLG]));
		// 在庫引当ステータス
		result.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_STOCKRESERVED_STATUS]));
		// 出荷ステータス
		result.Add(Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPED_STATUS]));
		// 配送伝票番号
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_SHIPPING_CHECK_NO]));
		// 出荷後変更区分
		result.Add(Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPED_CHANGED_KBN]));
		// 返品交換注文
		result.Add("return_exchange_flg", searchParam[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE]);
		result.Add("return_exchange_flg_on", ((string)result["return_exchange_flg"] == "1") ? "1" : null);
		result.Add("return_exchange_flg_off", ((string)result["return_exchange_flg"] == "0") ? "1" : null);
		// 返品交換区分
		result.Add(Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_KBN]));
		// 返品交換都合区分
		result.Add(Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_RETURN_EXCHANGE_REASON_KBN]));
		// 返品交換ステータス
		result.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_RETURN_EXCHANGE_STATUS]));
		// 返金ステータス
		result.Add(Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ORDER_REPAYMENT_STATUS]));
		// 返品交換返金更新日初期化
		result.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE + "_to", null);
		result.Add(Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE + "_from", null);
		result.Add(Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE + "_to", null);
		if (ddlReturnExchangeRepaymentUpdateDateStatus.SelectedValue.Length != 0)
		{
			// 返品交換返金更新日(From)
			var dateFrom = (string)searchParam[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_FROM];
			if (Validator.IsDate(dateFrom))
			{
				result[ddlReturnExchangeRepaymentUpdateDateStatus.SelectedValue + "_from"] = DateTime.Parse(dateFrom);
			}
			// 返品交換返金更新日(To)
			var dateTo = (string)searchParam[Constants.REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_TO];
			if (Validator.IsDate(dateTo))
			{
				result[ddlReturnExchangeRepaymentUpdateDateStatus.SelectedValue + "_to"] = DateTime.Parse(dateTo).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
			}
		}
		// サイト
		result.Add(Constants.FIELD_ORDER_MALL_ID, searchParam[Constants.REQUEST_KEY_ORDER_MALL_ID]);
		// 商品ID
		result.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_PRODUCT_ID]));
		// 商品名
		result.Add(Constants.FIELD_ORDERITEM_PRODUCT_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_PRODUCT_NAME]));
		// セットプロモーションID
		result.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SETPROMOTION_ID]));
		// ノベルティID
		result.Add(Constants.FIELD_ORDERITEM_NOVELTY_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_NOVELTY_ID]));
		// レコメンドID
		result.Add(Constants.FIELD_ORDERITEM_RECOMMEND_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_RECOMMEND_ID]));
		// 決済注文ID
		result.Add(Constants.FIELD_ORDER_PAYMENT_ORDER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_PAYMENT_ORDER_ID]));
		// 決済取引ID
		result.Add(Constants.FIELD_ORDER_CARD_TRAN_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_CARD_TRAN_ID]));
		// 定期購買注文
		result.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ID, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ID]));
		// 定期購入回数(注文時点)
		var orderCount = 0;
		result[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT + "_from"] = null;
		if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_FROM], out orderCount))
		{
			result[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT + "_from"] = orderCount;
		}
		result[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT + "_to"] = null;
		if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_TO], out orderCount))
		{
			result[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT + "_to"] = orderCount;
		}
		// 定期購入回数(出荷時点)
		var shippedCount = 0;
		result[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT + "_from"] = null;
		if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_FROM], out shippedCount))
		{
			result[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT + "_from"] = shippedCount;
		}
		result[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT + "_to"] = null;
		if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_TO], out shippedCount))
		{
			result[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT + "_to"] = shippedCount;
		}
		// 購入回数(注文時点)
		var userOrderCount = 0;
		result[Constants.FIELD_ORDER_ORDER_COUNT_ORDER + "_from"] = null;
		if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_ORDER_COUNT_FROM], out userOrderCount))
		{
			result[Constants.FIELD_ORDER_ORDER_COUNT_ORDER + "_from"] = userOrderCount;
		}
		result[Constants.FIELD_ORDER_ORDER_COUNT_ORDER + "_to"] = null;
		if (int.TryParse(Request[Constants.REQUEST_KEY_ORDER_ORDER_COUNT_TO], out userOrderCount))
		{
			result[Constants.FIELD_ORDER_ORDER_COUNT_ORDER + "_to"] = userOrderCount;
		}
		// 注文メモ
		result.Add(Constants.FIELD_ORDER_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MEMO_FLG]));
		// 管理メモ
		result.Add(Constants.FIELD_ORDER_MANAGEMENT_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO_FLG]));
		// 配送メモありなし
		result.Add(Constants.FIELD_ORDER_SHIPPING_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO_FLG]));
		// 決済連携メモ
		result.Add(Constants.FIELD_ORDER_PAYMENT_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO_FLG]));
		// 外部連携メモ
		result.Add(Constants.FIELD_ORDER_RELATION_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_RELATION_MEMO_FLG]));
		// 商品付帯情報フラグ
		result.Add(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_FLG]));
		// ギフト購入フラグ
		result.Add(Constants.FIELD_ORDER_GIFT_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_GIFT_FLG]));
		// デジタルコンテンツ商品フラグ
		result.Add(Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_DIGITAL_CONTENTS_FLG]));
		// 注文者の企業名
		result.Add(Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_COMPANY_NAME]));
		// ソート区分
		result.Add("sort_kbn", StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_SORT_KBN]));
		// ユーザー管理レベルID
		result.Add(Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID]));
		// 選択したユーザー管理レベルを除いて検索するか、しないか
		result.Add("user_management_level_exclude", StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE]));

		// 出荷予定日(From)
		result.Add(string.Format("{0}_from", Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE), null);
		var scheduledShippingDateFrom = string.Format("{0} {1}",
			(string)searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_FROM],
			(string)searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_FROM]);
		if (Validator.IsDate(scheduledShippingDateFrom))
		{
			result[string.Format("{0}_from", Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE)] = DateTime.Parse(scheduledShippingDateFrom);
			result[string.Format("{0}_fromto", Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE)] = DateTime.Parse(scheduledShippingDateFrom);
		}

		// 出荷予定日(To)
		result.Add(string.Format("{0}_to", Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE), null);
		var scheduledShippingDateTo = string.Format("{0} {1}",
			(string)searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_TO],
			(string)searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_TO]);
		if (Validator.IsDate(scheduledShippingDateTo))
		{
			result[string.Format("{0}_to", Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE)] =
				DateTime.Parse(scheduledShippingDateTo).AddSeconds(1);
			result[string.Format("{0}_fromto", Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE)] =
				DateTime.Parse(scheduledShippingDateTo).AddSeconds(1);
		}

		// 出荷予定日(デフォルトを含む)
		result.Add(Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE + "_none", StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_NONE]));

		// 配送希望日(From)
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_from", null);
		var shippingDateFrom = string.Format("{0} {1}",
			(string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_FROM],
			(string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_FROM]);
		var shippingDateSpecified = false;
		if (Validator.IsDate(shippingDateFrom))
		{
			result[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_from"] = DateTime.Parse(shippingDateFrom);
			result[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_fromto"] = DateTime.Parse(shippingDateFrom);
			shippingDateSpecified = true;
		}
		// 配送希望日(To)
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_to", null);
		var shippingDateTo = string.Format("{0} {1}",
			(string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_TO],
			(string)searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_TIME_TO]);
		if (Validator.IsDate(shippingDateTo))
		{
			result[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_to"] =
				DateTime.Parse(shippingDateTo).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
			result[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_fromto"] =
				DateTime.Parse(shippingDateTo).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
			shippingDateSpecified = true;
		}
		// 配送希望日(指定なし含む)
		var shippingDateNone = StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_NONE]) == "1";
		var shippingDateNoneUnchecked = (shippingDateSpecified == false) && (shippingDateNone == false) ? "1" : "";
		var shippingDateNoneChecked = (shippingDateSpecified && shippingDateNone) ? "1" : "";
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_none_unchecked", shippingDateNoneUnchecked);
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_none_checked", shippingDateNoneChecked);
		// 配送希望日(デフォルトを含む)
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_none", StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_SHIPPING_DATE_NONE]));
		// 注文メモ
		result.Add(Constants.FIELD_ORDER_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_ORDER_MEMO]));
		// 管理メモ
		result.Add(Constants.FIELD_ORDER_MANAGEMENT_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_ORDER_MANAGEMENT_MEMO]));
		// 配送メモ
		result.Add(Constants.FIELD_ORDER_SHIPPING_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_MEMO]));
		// 決済連携メモ
		result.Add(Constants.FIELD_ORDER_PAYMENT_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_ORDER_PAYMENT_MEMO]));
		// 外部連携メモ
		result.Add(Constants.FIELD_ORDER_RELATION_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_ORDER_RELATION_MEMO]));
		// 商品付帯情報フラグ
		result.Add(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_TEXTS]));
		// 別出荷フラグ
		result.Add(Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ANOTHER_SHIPPING_FLAG]));
		// 配送者名
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME + "_like_escaped", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_NAME]));
		// 配送者名（かな）
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA + "_like_escaped", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_NAME_KANA]));
		// 配送先郵便番号
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_like_escaped", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_ZIP]));
		// 配送先住所
		result.Add("shipping_addr_like_escaped", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_ADDR]));
		// 配送者電話番号
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_like_escaped", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_TEL1]));
		// 商品セールID
		result.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID + "_like_escaped", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID]));
		// クーポンコード
		result.Add(Constants.FIELD_ORDERCOUPON_COUPON_CODE + "_like_escaped", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPON_CODE]));
		// クーポンコード（管理用名称）
		result.Add(Constants.FIELD_ORDERCOUPON_COUPON_NAME + "_like_escaped", StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_COUPON_NAME]));
		// Order User Memo Flag
		result.Add(Constants.FIELD_USER_USER_MEMO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_USER_MEMO_FLG]));
		// Order User Memo
		result.Add(Constants.FIELD_USER_USER_MEMO + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_USER_USER_MEMO]));
		// AdvCode Flag
		result.Add(OrderSearchParam.KEY_ORDER_ADVCODE, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_ADVCODE_FLG]));
		// AdvCode
		result.Add(OrderSearchParam.KEY_ORDER_ADVCODE + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_ADVCODE]));
		// 商品同梱ID
		result.Add(Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDERITEM_PRODUCT_BUNDLE_ID]));
		// 外部連携受注ID
		result.Add(Constants.FIELD_ORDER_EXTERNAL_ORDER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_ORDER_EXTERNAL_ORDER_ID]));
		// 外部連携取込ステータス
		result.Add(Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_EXTERNAL_IMPORT_STATUS]));
		// モール連携ステータス
		result.Add(Constants.FIELD_ORDER_MALL_LINK_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_MALL_LINK_STATUS]));
		// 配送方法
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_METHOD]));
		// 領収書希望フラグ
		result.Add(Constants.FIELD_ORDER_RECEIPT_FLG, StringUtility.ToEmpty(searchParam[Constants.REQUEST_KEY_ORDER_RECEIPT_FLG]));
		// 請求書同梱フラグ
		result.Add(Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_INVOICE_BUNDLE_FLG]));
		// TwInvoice No
		result.Add(Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TW_INVOICE_NO]));
		// TwInvoice Status
		result.Add(Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TW_INVOICE_STATUS]));

		// 配送状態
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS]));
		// 配送先:都道府県
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, StringUtility.ToEmpty(string.Join("','", PrefectureUtility.GetPrefectures(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_PREFECTURE])))));

		result.Add(Constants.SEARCH_FIELD_ORDER_EXTEND_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_NAME]));
		result.Add(Constants.SEARCH_FIELD_ORDER_EXTEND_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_FLG]));
		result.Add(Constants.SEARCH_FIELD_ORDER_EXTEND_TYPE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TYPE]));
		result.Add(Constants.SEARCH_FIELD_ORDER_EXTEND_LIKE_ESCAPED, StringUtility.SqlLikeStringSharpEscape(Request[Constants.REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT]));

		// 完了状態コード
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_STATUS_CODE]));
		// 現在の状態
		result.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_SHIPPING_CURRENT_STATUS]));

		// 頒布会コースID
		result.Add(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchParam[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID]));
		// 頒布会購入回数
		var subscriptionBoxOrderCount = 0;
		result[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT + "_from"] = null;
		if (int.TryParse(Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_FROM], out subscriptionBoxOrderCount))
		{
			result[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT + "_from"] = subscriptionBoxOrderCount;
		}
		result[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT + "_to"] = null;
		if (int.TryParse(Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_TO], out subscriptionBoxOrderCount))
		{
			result[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT + "_to"] = subscriptionBoxOrderCount;
		}

		// Store pickup status
		result[Constants.FIELD_ORDER_STOREPICKUP_STATUS] = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_STORE_PICKUP_STATUS]);

		return result;
	}

	/// <summary>
	/// データバインド用注文情報詳細URL作成
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <returns>注文情報詳細URL</returns>
	protected string CreateOrderDetailUrl(string strOrderId)
	{
		return CreateOrderDetailUrl(
			strOrderId,
			false,
			false);
	}

	/// <summary>
	/// データバインド用注文一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>注文一覧遷移URL</returns>
	protected string CreateOrderListUrl(Hashtable htSearch, int iPageNumber)
	{
		return CreateOrderListUrl(htSearch, false) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		var url = CreateOrderListUrl(GetSearchInfoFromControl(), 1);

		// Check Max Length Url
		CheckMaxLengthUrl(url);

		Response.Redirect(url);
	}

	/// <summary>
	/// 送り状ダウンロードリンククリック
	/// </summary>
	/// <param name="sender">エベント元</param>
	/// <param name="e">エベント引数</param>
	protected void lbShippingLabelExport_Click(object sender, EventArgs e)
	{
		// セッションに検索情報を格納
		Session[Constants.SESSION_KEY_PARAM] = GetSearchSqlInfo(GetSearchInfoFromControl());

		// 注文関連ファイル出力ページに遷移
		Response.Redirect(string.Format(
			"{0}{1}?{2}={3}&{4}={5}",
			Constants.PATH_ROOT,
			Constants.PAGE_MANAGER_ORDERFILEEXPORT_LIST,
			Constants.REQUEST_KEY_ORDERFILE_ORDERPAGE,
			Server.UrlEncode(Constants.PAGE_MANAGER_ORDER_LIST),
			Constants.REQUEST_KEY_SHIPPING_LABEL_LINK,
			((LinkButton)sender).CommandArgument));
	}

	/// <summary>
	/// 納品書出力クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPdfOutput_Click(object sender, System.EventArgs e)
	{
		if (Constants.GLOBAL_OPTION_ENABLE == false)
		{
			// 検索情報をセッションに保存
			Session[Constants.SESSION_KEY_PARAM] = GetSearchSqlInfo(
				GetSearchInfoFromControl(),
				isOrderIdReplace: true);

			// PDF出力ページへ遷移
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PDF_OUTPUT
				+ "?" + Constants.REQUEST_KEY_PDF_OUTPUT + "=" + Constants.KBN_PDF_OUTPUT_ORDER
				+ "&" + Constants.REQUEST_KEY_PDF_KBN + "=" + Constants.KBN_PDF_OUTPUT_ORDER_INVOICE);
		}
		else
		{
			// グローバル対応の場合、非同期で納品書を出力する
			lbPdfOutputUnsync_Click(sender, e);
		}
	}

	/// <summary>
	/// 納品書出力クリック(非同期)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPdfOutputUnsync_Click(object sender, EventArgs e)
	{
		// ＰＤＦクリエータ起動
		Form_PdfOutput_PdfOutput.ExecPdfCreater(
			Session.SessionID,
			Constants.KBN_PDF_OUTPUT_ORDER,
			Constants.KBN_PDF_OUTPUT_ORDER_INVOICE,
			GetSearchSqlInfo(GetSearchInfoFromControl(), isOrderIdReplace: true));
	}

	/// <summary>
	/// トータルピッキングリスト出力クリック(非同期)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbTotalPickingListOutputUnsync_Click(object sender, EventArgs e)
	{
		// ＰＤＦクリエータ起動
		Form_PdfOutput_PdfOutput.ExecPdfCreater(
			Session.SessionID,
			Constants.KBN_PDF_OUTPUT_ORDER,
			Constants.KBN_PDF_OUTPUT_TOTAL_PICKING_LIST,
			GetSearchSqlInfo(GetSearchInfoFromControl()));
	}

	/// <summary>
	/// 受注明細書出力クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPdfOutputOrderStatement_Click(object sender, System.EventArgs e)
	{
		// 検索情報をセッションに保存
		Session[Constants.SESSION_KEY_PARAM] = GetSearchSqlInfo(GetSearchInfoFromControl());

		// PDF出力ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PDF_OUTPUT
			+ "?" + Constants.REQUEST_KEY_PDF_OUTPUT + "=" + Constants.KBN_PDF_OUTPUT_ORDER
			+ "&" + Constants.REQUEST_KEY_PDF_KBN + "=" + Constants.KBN_PDF_OUTPUT_ORDER_STATEMENT);
	}

	/// <summary>
	/// 受注明細書出力クリック(非同期)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPdfOutputOrderStatementUnsync_Click(object sender, EventArgs e)
	{
		// ＰＤＦクリエータ起動
		Form_PdfOutput_PdfOutput.ExecPdfCreater(
			Session.SessionID,
			Constants.KBN_PDF_OUTPUT_ORDER,
			Constants.KBN_PDF_OUTPUT_ORDER_STATEMENT,
			GetSearchSqlInfo(GetSearchInfoFromControl()));
	}

	/// <summary>
	/// 基幹システム連携用データ出力クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbInteractionDataExport_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 指定ファイルの連携用データ設定を取得
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			using (SqlStatement sqlStatement = new SqlStatement("Order", "GetIDOnly"))
			{

				var searchParam = GetSearchSqlInfo(GetSearchInfoFromControl());

				sqlStatement.ReplaceStatement("@@ orderby @@", StringUtility.ToEmpty(searchParam["sort_kbn"]));
				sqlStatement.ReplaceStatement("@@ multi_order_id @@", OrderCommon.GetOrderSearchMultiOrderId(searchParam));
				sqlStatement.ReplaceStatement("@@ order_shipping_addr1 @@", StringUtility.ToEmpty(searchParam[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]));
				sqlStatement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(sqlStatement.Statement, Constants.TABLE_ORDER, StringUtility.ToEmpty(searchParam[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));
				sqlStatement.ExecStatement(sqlAccessor, searchParam);
			}

			// Create data exporter
			DataExporterBase dataExporter = DataExporterCreater.CreateDataExporter(Constants.PROJECT_NO, int.Parse(((LinkButton)sender).CommandArgument), sqlAccessor);
			dataExporter.Process(Response);
		}
	}

	/// <summary>
	/// Link button print invoice order for taiwan EC Pay click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPrintInvoiceOrderForTwECPay_Click(object sender, EventArgs e)
	{
		using (var sqlAccessor = new SqlAccessor())
		using (var statement = new SqlStatement("Order", "GetDeliveryTranIdList"))
		{
			var searchParam = GetSearchSqlInfo(GetSearchInfoFromControl());

			statement.ReplaceStatement("@@ orderby @@", StringUtility.ToEmpty(searchParam["sort_kbn"]));
			statement.ReplaceStatement("@@ multi_order_id @@", OrderCommon.GetOrderSearchMultiOrderId(searchParam));
			statement.ReplaceStatement("@@ order_shipping_addr1 @@", StringUtility.ToEmpty(searchParam[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]));
			statement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(statement.Statement, Constants.TABLE_ORDER, StringUtility.ToEmpty(searchParam[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));
			var data = statement.SelectSingleStatementWithOC(sqlAccessor, searchParam);

			if ((data == null) || (data.Count == 0))
			{
				return;
			}

			// Create request data and get data from api
			var deliveryTranIdList = data.Cast<DataRowView>().Select(row =>
				StringUtility.ToEmpty(row[Constants.FIELD_ORDER_DELIVERY_TRAN_ID])).ToArray();
			var clientScript = ECPayUtility.CreateScriptForGetInvoiceOrder(deliveryTranIdList);
			ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "OrderInvoicePrinting", clientScript, true);
		}
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return GetSearchSqlInfo(GetParametersAndSetToControl());
	}

	/// <summary>
	/// 受注件数取得
	/// </summary>
	/// <param name="param">パラメータ</param>
	/// <param name="hasReceiptOnly">領収書希望ありの注文のみ</param>
	/// <returns>受注件数</returns>
	protected int GetOrderCounts(Hashtable param, bool hasReceiptOnly = false)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Order", "GetOrderCount"))
		{
			statement.UseLiteralSql = true;

			// 領収書希望ありの注文のみの件数を取得する場合、むりやり領収書希望ありの条件に変更
			if (hasReceiptOnly)
			{
				param[Constants.FIELD_ORDER_RECEIPT_FLG] = Constants.FLG_ORDER_RECEIPT_FLG_ON;
			}
			statement.ReplaceStatement(
				"@@ multi_order_id @@",
				(param[Constants.PARAM_REPLACEMENT_ORDER_ID_LIKE_ESCAPED] != null)
					? ((string)param[Constants.PARAM_REPLACEMENT_ORDER_ID_LIKE_ESCAPED])
						.Replace("'", "''").Replace(",", "','")
					: OrderCommon.GetOrderSearchMultiOrderId(param));
			statement.ReplaceStatement("@@ order_shipping_addr1 @@", StringUtility.ToEmpty(param[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]));
			statement.ReplaceStatement("@@ replacement_order_id_like_escaped @@", StringUtility.ToEmpty(param[Constants.PARAM_REPLACEMENT_ORDER_ID_LIKE_ESCAPED]).Replace("'", "''"));
			statement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(statement.Statement, Constants.TABLE_ORDER, StringUtility.ToEmpty(param[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));
			statement.ReplaceStatement("@@ real_shop_ids_condition @@", string.Empty);

			var counts = (int)statement.SelectSingleStatementWithOC(accessor, param)[0]["order_count"];
			return counts;
		}
	}

	/// <summary>
	/// 領収書出力クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPdfOutputReceipt_Click(object sender, System.EventArgs e)
	{
		// 領収書希望ありの条件を調整
		var searchInfo = GetSearchSqlInfo(GetSearchInfoFromControl());
		searchInfo[Constants.FIELD_ORDER_RECEIPT_FLG] = Constants.FLG_ORDER_RECEIPT_FLG_ON;

		// 検索情報をセッションに保存
		Session[Constants.SESSION_KEY_PARAM] = searchInfo;

		//領収書発行メール送信
		SendReceiptMail((Hashtable)Session[Constants.SESSION_KEY_PARAM]);

		// PDF出力ページへ遷移
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PDF_OUTPUT)
			.AddParam(Constants.REQUEST_KEY_PDF_OUTPUT, Constants.KBN_PDF_OUTPUT_ORDER)
			.AddParam(Constants.REQUEST_KEY_PDF_KBN, Constants.KBN_PDF_OUTPUT_RECEIPT)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 領収書発行メール送信
	/// </summary>
	/// <param name="param">検索パラメタ</param>
	protected void SendReceiptMail(Hashtable param)
	{
		var orders = new ReceiptCreater().GetOrders(param, Constants.KBN_PDF_OUTPUT_ORDER);
		var orderIdList = new List<string>();
		var runningTask = new List<Task>();
		if (Directory.Exists(ReceiptCreater.TempDirPath))
		{
			var dir = new DirectoryInfo(ReceiptCreater.TempDirPath);
			foreach (var file in dir.GetFiles())
			{
				if (file.LastWriteTime < DateTime.Now.AddMinutes(-10))
				{
					// 更新日時が10分たったものは削除
					file.Delete();
				}
			}
		}

		foreach (var drvOrder in orders)
		{
			var order = (DataRowView)drvOrder;
			if (orderIdList.Contains((string)order[Constants.REQUEST_KEY_ORDER_ID])) continue;
			orderIdList.Add((string)order[Constants.REQUEST_KEY_ORDER_ID]);

			var orderParam = new Hashtable
			{
				{ Constants.REQUEST_KEY_SHOP_ID, order[Constants.REQUEST_KEY_SHOP_ID] },
				{ Constants.HASH_KEY_ORDER_ID, order[Constants.REQUEST_KEY_ORDER_ID] },
				{ Constants.FIELD_ORDERITEM_DATE_CREATED, order[Constants.FIELD_ORDERITEM_DATE_CREATED] },
				{ Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, order[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL] },
			};
			runningTask.Add(Task.Run(() =>
			{
				var filePath = new ReceiptCreater().CreateMailFile(Constants.KBN_PDF_OUTPUT_ORDER, orderParam, (string)orderParam[Constants.HASH_KEY_ORDER_ID]);
				SendMailCommon.SendReceipFiletMail((string)orderParam[Constants.HASH_KEY_ORDER_ID], filePath);
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}));
		}

		foreach (var t in runningTask)
		{
			t.Wait();
		}

	}

	/// <summary>
	/// 領収書出力クリック（非同期）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPdfOutputReceiptUnsync_Click(object sender, EventArgs e)
	{
		// 領収書希望ありの条件を調整
		var searchInfo = GetSearchSqlInfo(GetSearchInfoFromControl());
		searchInfo[Constants.FIELD_ORDER_RECEIPT_FLG] = Constants.FLG_ORDER_RECEIPT_FLG_ON;

		//領収書発行メール送信
		SendReceiptMail(searchInfo);

		// ＰＤＦ作成起動
		Form_PdfOutput_PdfOutput.ExecPdfCreater(
			Session.SessionID,
			Constants.KBN_PDF_OUTPUT_ORDER,
			Constants.KBN_PDF_OUTPUT_RECEIPT,
			searchInfo);
	}

	/// <summary>
	/// Calculate search rowspan
	/// </summary>
	/// <returns>Number of rowspan</returns>
	protected int CalculateSearchRowSpan()
	{
		var result = DEFAULT_SEARCH_ROWSPAN
			+ (cbReturnExchange.Checked ? 3 : 0)
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
	/// <summary>Display NotSearch Default</summary>
	protected override bool IsNotSearchDefault
	{
		get
		{
			var isDisplayDataInList = (Request.QueryString.AllKeys.Any(x => x != Constants.REQUEST_KEY_FIRSTVIEW) == false) && (Constants.DISPLAY_NOT_SEARCH_DEFAULT);
			var isFirstViewOrderStatus = (this.IsFirstView && (string.IsNullOrEmpty(Constants.ORDERLIST_FIRSTVIEW_ORDERSTATUS) == false));

			return (isDisplayDataInList && (isFirstViewOrderStatus == false));
		}
	}
	/// <summary>初回表示</summary>
	protected bool IsFirstView
	{
		get
		{
			return (Request.QueryString.AllKeys.Any(x => x == Constants.REQUEST_KEY_FIRSTVIEW) || Request.QueryString.Count == 0);
		}
	}
	/// <summary>項目表示設定</summary>
	protected ManagerListDispSettingModel[] ColumnDisplaySettings
	{
		get
		{
			if (ViewState[Constants.FLG_COLUMNDISPLAY_SETTINGS_VIEW] == null)
			{
				var dispSettings = new ManagerListDispSettingService()
					.GetAllByDispSettingKbn(
						this.LoginOperatorShopId,
						Constants.FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERLIST).OrderBy(x => x.DispOrder)
					.ToArray();

				// Remove column store pickup status if condition not meet
				if (Constants.STORE_PICKUP_OPTION_ENABLED == false)
				{
					dispSettings = dispSettings.Where(ds => ds.DispColmunName != Constants.FIELD_ORDER_STOREPICKUP_STATUS).ToArray();
				}

				ViewState[Constants.FLG_COLUMNDISPLAY_SETTINGS_VIEW] = dispSettings;
			}
			return (ManagerListDispSettingModel[])ViewState[Constants.FLG_COLUMNDISPLAY_SETTINGS_VIEW];
		}
	}
	/// <summary>
	/// 受注情報一覧表示で連携OPを確認する
	/// </summary>
	/// <param name="dispColumnName"></param>
	/// <returns></returns>
	protected bool IsOptionCooperation(string dispColumnName)
	{
		switch (dispColumnName)
		{
			case Constants.FIELD_USER_MALL_ID:
				if (((Constants.MALLCOOPERATION_OPTION_ENABLED) == false)
					|| ((Constants.URERU_AD_IMPORT_ENABLED) == false)) return false;
				break;

			case Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS:
			case Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS:
				if ((Constants.REALSTOCK_OPTION_ENABLED) == false) return false;
				break;

			case Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG:
				if ((Constants.DIGITAL_CONTENTS_OPTION_ENABLED) == false) return false;
				break;

			case Constants.FIELD_ORDER_GIFT_FLG:
				if ((Constants.GIFTORDER_OPTION_ENABLED) == false) return false;
				break;
		}

		return true;
	}
}
