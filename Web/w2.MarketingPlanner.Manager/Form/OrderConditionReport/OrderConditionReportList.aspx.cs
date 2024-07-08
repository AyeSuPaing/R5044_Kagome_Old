/*
=========================================================================================================
  Module      : 売上状況レポート一覧ページ処理(OrderConditionReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Product;
using w2.Common.Web;
using w2.Domain.ProductCategory;
using w2.Domain.ProductSale;

public partial class Form_OrderConditionReport_OrderConditionReportList : BasePage
{
	#region 表示定数
	protected int m_iCurrentYear = DateTime.Now.Year;
	protected int m_iCurrentMonth = DateTime.Now.Month;

	protected const string REQUEST_KEY_REPORT_TYPE = "rtype";
	protected const string KBN_REPORT_TYPE_MONTHLY_REPORT = "1";				// 月表示
	protected const string KBN_REPORT_TYPE_DAILY_REPORT = "0";					// 日表示
	protected const string REQUEST_KEY_SALES_TYPE = "stype";
	protected const string KBN_SALES_TYPE_ORDER_REPORT = "1";					// 注文基準
	protected const string KBN_SALES_TYPE_SHIP_REPORT = "0";					// 出荷基準

	protected const string REQUEST_KEY_PAYMENT_KBN = "pkbn";					// 支払区分
	protected const string REQUEST_KEY_ORDER_KBN = "okbn";						// 注文区分
	protected const string REQUEST_KEY_AGGREGATE_UNIT = "agkbn"; // 集計単位

	/// <summary>割引状況</summary>
	protected const string REQUEST_KEY_DISCOUNT_TYPE = "discount";
	/// <summary>割引前</summary>
	protected const string KBN_DISCOUNT_EXCLUDE = "discount_exclude";
	/// <summary>割引後</summary>
	protected const string KBN_DISCOUNT_INCLUDE = "discount_include";

	protected const string REQUEST_KEY_MALL_ID = "mlid";						// モールID（サイト）

	protected const string REQUEST_KEY_ORDER_TYPE = "ot";					// 通常注文/定期注文

	protected const string KBN_OPERATION_TYPE_AMOUNT = "0";						// 金額表示
	protected const string KBN_OPERATION_TYPE_COUNT = "1";						// 件数表示

	/// <summary>リクエストキー：セールID</summary>
	protected const string REQUEST_KEY_SALE_ID = "sid";
	/// <summary>リクエストキー：セール区分</summary>
	protected const string REQUEST_KEY_SALE_KBN = "skbn";

	protected const string KBN_AGGREGATE_UNIT_ORDER = "order"; // 合計（注文単位）
	protected const string KBN_AGGREGATE_UNIT_ORDERITEM = "item"; // 小計（商品単位）

	protected const string FIELD_ORDERCONDITION_TARGET_DATE = "tgt_date";				// 日付
	protected const string FIELD_ORDERCONDITION_ORDERED_AMOUNT = "ordered_amount";			// 売上金額(注文基準)
	protected const string FIELD_ORDERCONDITION_ORDERED_COUNT = "ordered_count";			// 売上件数(注文基準)
	protected const string FIELD_ORDERCONDITION_ORDERED_AMOUNT_AVG = "ordered_amount_avg";	// 売上平均購入単価(注文基準)
	protected const string FIELD_ORDERCONDITION_CANCELED_AMOUNT = "canceled_amount";		// キャンセル金額
	protected const string FIELD_ORDERCONDITION_CANCELED_COUNT = "canceled_count";			// キャンセル件数
	protected const string FIELD_ORDERCONDITION_SHIPPED_AMOUNT = "shipped_amount";				// 売上金額(出荷基準)
	protected const string FIELD_ORDERCONDITION_SHIPPED_COUNT = "shipped_count";				// 売上件数(出荷基準)
	protected const string FIELD_ORDERCONDITION_SHIPPED_AMOUNT_AVG = "shipped_amount_avg";		// 売上平均購入単価(出荷基準)
	protected const string FIELD_ORDERCONDITION_RETURNED_AMOUNT = "returned_amount";			// 返品金額
	protected const string FIELD_ORDERCONDITION_RETURNED_COUNT = "returned_count";			// 返品件数
	protected const string FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_AMOUNT = "subtotal_ordered_amount";	// 売上小計金額(注文基準)
	protected const string FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_COUNT = "subtotal_ordered_count";	// 売上小計件数(注文基準)
	protected const string FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_AMOUNT = "subtotal_shipped_amount";	// 売上小計金額(出荷基準)
	protected const string FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_COUNT = "subtotal_shipped_count";	// 売上小計件数(出荷基準)

	protected const string CONST_GET_ORDER_CONDITION_DAYS = "100"; // 期間指定取得日数

	// 抽出条件定数
	private const string FIELD_ORDERCONDITION_ORDER_PAYMENT_KBNS = "@@ order_payment_kbns @@";		// 決済種別(複数)
	private const string FIELD_ORDERCONDITION_ORDER_KBNS = "@@ order_kbns @@";		// 注文区分(複数)
	private const string FIELD_ORDERCONDITION_COUNTRY_ISO_CODE_KBNS = "@@ country_iso_code_kbns @@";		// 国ISOコード区分(複数)
	private const string FIELD_ORDERCONDITION_ORDER_TYPES = "@@ order_types @@";		// 通常注文/定期注文(複数)
	private const string FIELD_ORDERCONDITION_AMOUNT_FIELD_NAME = "@@ amount_field_name @@";		// 金額項目名
	private const string FIELD_ORDERCONDITION_COUNT_FIELD_NAME = "@@ count_field_name @@";		// 数項目名
	private const string FIELD_ORDERCONDITION_TABLE_NAME = "@@ table_name @@";		// テーブル名
	private const string FIELD_ORDERCONDITION_SALES_KBN_TARGET = "@@ ym @@";		// 日別月別指定
	private const string FIELD_ORDERCONDITION_ITEM_SEARCH_CONDITION = "@@ item_search_condition @@";		// 商品検索条件
	//private const string FIELD_ORDERCONDITION_ORDER_PAYMENT_KBNS_COUNT = "order_payment_kbns_count";	// 決済種別(複数)指定数
	private const string FIELD_ORDERCONDITION_GROUP_BY = "@@ group_by @@";		// 金額項目名

	/// <summary>ValueTextパラメータ：売上状況レポート</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT = "Order_Condition_Report";
	/// <summary>ValueTextパラメータ：商品セール区分（通常）</summary>
	public const string VALUETEXT_PARAM_PRODUCTSALE_KBN_NOMAL = "NM";

	/// <summary>Default end hours</summary>
	public const int DEFAULT_END_HOURS = 23;
	/// <summary>Default end minutes</summary>
	public const int DEFAULT_END_MINUTES = 59;
	/// <summary>Default end seconds</summary>
	public const int DEFAULT_END_SECONDS = 59;

	/// <summary>キャンセル件数インデックス</summary>
	private const int CANCEL_NUMBER_INDEX = 4;
	/// <summary>キャンセル注文表示URLにおける日付フォーマット</summary>
	private const string FORMAT_CANCEL_DATE = "{0}/{1:00}/{2:00}";
	/// <summary>一年の始まりの日付</summary>
	private const string YEARS_START_DATE = "/01/01";
	/// <summary>一年の終わりの日付</summary>
	private const string YEARS_END_DATE = "/12/31";
	/// <summary>一日の始まりの時間</summary>
	private const string DAILY_START_TIME = "00:00:00";
	/// <summary>一日の終わりの時間</summary>
	private const string DAILY_END_TIME = "23:59:59";

	/// <summary>注文ステータス：キャンセル</summary>
	private const string FLG_ORDER_ORDER_STATUS_ORDER_CANCELED = "ODR_CNSL";
	#endregion

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
			// コンポーネント初期化
			//------------------------------------------------------
			Initialize();

			//------------------------------------------------------
			// パラメタ取得・設定
			//------------------------------------------------------
			// 対象年月
			bool blCurrentYear = false;
			bool blCurrentMonth = false;
			var orderConditionYearFrom = ucOrderConditionDatePeriod.HfStartDate.Value.Split('/')[0];
			var orderConditionMonthFrom = ucOrderConditionDatePeriod.HfStartDate.Value.Split('/')[1];
			var orderConditionDayFrom = ucOrderConditionDatePeriod.HfStartDate.Value.Split('/')[2];

			var orderConditionYearTo = ucOrderConditionDatePeriod.HfEndDate.Value.Split('/')[0];
			var orderConditionMonthTo = ucOrderConditionDatePeriod.HfEndDate.Value.Split('/')[1];
			var orderConditionDayTo = ucOrderConditionDatePeriod.HfEndDate.Value.Split('/')[2];

			if (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_YEAR], out m_iCurrentYear))
			{
				orderConditionYearFrom = m_iCurrentYear.ToString();
				orderConditionYearTo = m_iCurrentYear.ToString();
				blCurrentYear = true;
			}
			else
			{
				m_iCurrentYear = DateTime.Now.Year;
			}

			if (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_MONTH], out m_iCurrentMonth))
			{
				orderConditionMonthFrom = m_iCurrentMonth.ToString("00");
				orderConditionMonthTo = m_iCurrentMonth.ToString("00");
				blCurrentMonth = true;
			}
			else
			{
				m_iCurrentMonth = DateTime.Now.Month;
			}

			if (blCurrentMonth && blCurrentYear)
			{
				orderConditionDayFrom = "01";
				orderConditionDayTo =
					DateTime.DaysInMonth(m_iCurrentYear, m_iCurrentMonth).ToString("00");
			}

			var orderConditionDateFrom = string.Format("{0}{1}{2}{3}",
				orderConditionYearFrom,
				orderConditionMonthFrom,
				orderConditionDayFrom,
				ucOrderConditionDatePeriod.StartTimeString);

			var orderConditionDateTo = string.Format("{0}{1}{2}{3}",
				orderConditionYearTo,
				orderConditionMonthTo,
				orderConditionDayTo,
				ucOrderConditionDatePeriod.EndTimeString);

			ucOrderConditionDatePeriod.SetPeriodDate(
				orderConditionDateFrom,
				orderConditionDateTo,
				"yyyyMMddHH:mm:ss.fff");

			ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = m_iCurrentYear;
			ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = m_iCurrentMonth;

			// 売り上げ基準
			switch (Request[REQUEST_KEY_SALES_TYPE])
			{
				case KBN_SALES_TYPE_ORDER_REPORT:
				case KBN_SALES_TYPE_SHIP_REPORT:
					foreach (ListItem li in rblSalesType.Items)
					{
						li.Selected = (li.Value == Request[REQUEST_KEY_SALES_TYPE]);
					}
					break;
				default:
					foreach (ListItem li in rblSalesType.Items)
					{
						li.Selected = (li.Value == KBN_SALES_TYPE_ORDER_REPORT);
					}
					break;
			}

			// レポート種別
			switch (Request[REQUEST_KEY_REPORT_TYPE])
			{
				case KBN_SALES_TYPE_ORDER_REPORT:
				case KBN_SALES_TYPE_SHIP_REPORT:
					foreach (ListItem li in rblReportType.Items)
					{
						li.Selected = (li.Value == Request[REQUEST_KEY_REPORT_TYPE]);
					}
					break;
				default:
					foreach (ListItem li in rblReportType.Items)
					{
						li.Selected = (li.Value == KBN_REPORT_TYPE_DAILY_REPORT);
					}
					break;
			}

			// 集計単位
			foreach (ListItem li in ValueText.GetValueItemList(VALUETEXT_PARAM_ORDER_CONDITION_REPORT, "aggregate_unit_type"))
			{
				switch (Request.Params[REQUEST_KEY_AGGREGATE_UNIT])
				{
					case KBN_AGGREGATE_UNIT_ORDER:
					case KBN_AGGREGATE_UNIT_ORDERITEM:
						li.Selected = (li.Value == Request[REQUEST_KEY_AGGREGATE_UNIT]);
						break;

					default:
						li.Selected = (li.Value == KBN_AGGREGATE_UNIT_ORDER);
						break;
				}
				rblAggregateUnit.Items.Add(li);
			}

			// 割引区分
			foreach (ListItem li in ValueText.GetValueItemList(VALUETEXT_PARAM_ORDER_CONDITION_REPORT, "discount_type"))
			{
				switch (Request.Params[REQUEST_KEY_DISCOUNT_TYPE])
				{
					case KBN_DISCOUNT_EXCLUDE:
					case KBN_DISCOUNT_INCLUDE:
						li.Selected = (li.Value == Request[REQUEST_KEY_DISCOUNT_TYPE]);
						break;

					default:
						li.Selected = (li.Value == KBN_DISCOUNT_EXCLUDE);
						break;
				}

				rblDiscountType.Items.Add(li);
			}

			// 決済種別
			if (Request.Params[REQUEST_KEY_PAYMENT_KBN] != null)
			{
				foreach (object objValue in Request.Params.GetValues(REQUEST_KEY_PAYMENT_KBN))
				{
					ListItem li = cblOrderPaymentKbns.Items.FindByValue(objValue.ToString());
					if (li != null)
					{
						li.Selected = true;
					}
				}
			}
			else
			{
				// デフォルトは全て選択
				foreach (ListItem li in cblOrderPaymentKbns.Items)
				{
					li.Selected = true;
				}
			}

			// サイト
			DataView dvMallCooperationSetting = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "GetMallCooperationSettingListFromShopId"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, this.LoginOperatorShopId);

				dvMallCooperationSetting = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}
			ddlSiteName.Items.AddRange(ValueText.GetValueItemArray("SiteName", "OwnSiteName"));
			foreach (DataRowView drvMallCooperationSetting in dvMallCooperationSetting)
			{
				ddlSiteName.Items.Add(
					new ListItem(
						CreateSiteNameForList(
							(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID],
							(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]),
							(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]));
			}
			if (Request.Params[REQUEST_KEY_MALL_ID] != null)
			{
				foreach (ListItem li in ddlSiteName.Items)
				{
					li.Selected = (li.Value == (string)Request.Params[REQUEST_KEY_MALL_ID]);
				}
			}

			// 通常注文/定期注文/頒布会注文
			cbOrderTypes.Items.AddRange(ValueText.GetValueItemArray(VALUETEXT_PARAM_ORDER_CONDITION_REPORT, "order_type"));
			var orderTypes = Request.Params.GetValues(REQUEST_KEY_ORDER_TYPE) ?? new string[0];
			cbOrderTypes.Items.Cast<ListItem>().ToList().ForEach(
				li => li.Selected = (orderTypes.Length == 0) || orderTypes.Contains(li.Value));

			// 頒布会OPがOFFの場合ValueTextに頒布会注文の記述があるケースでも選択項目から除外
			if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false)
			{
				cbOrderTypes.Items.Remove(
					cbOrderTypes.Items.Cast<ListItem>().FirstOrDefault(
						item => (item.Text == ValueText.GetValueText(VALUETEXT_PARAM_ORDER_CONDITION_REPORT, "order_type", "subscription_box"))));
			}

			// 購買区分
			if (Request.Params[REQUEST_KEY_ORDER_KBN] != null)
			{
				foreach (object objValue in Request.Params.GetValues(REQUEST_KEY_ORDER_KBN))
				{
					ListItem li = cblOrderKbns.Items.FindByValue(objValue.ToString());
					if (li != null)
					{
						li.Selected = true;
					}
				}
			}
			else
			{
				// デフォルトは全て選択
				foreach (ListItem li in cblOrderKbns.Items)
				{
					li.Selected = true;
				}
			}

			// 商品ID
			tbProductId.Text = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]);

			// ブランドID
			foreach (ListItem li in ddlBrandSearch.Items)
			{
				li.Selected = (li.Value == StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_BRAND_ID]));
			}

			// 商品セール区分
			ddlProductSaleKbn.Items.Cast<ListItem>().ToList().ForEach(li =>
				li.Selected = (li.Value == StringUtility.ToEmpty(Request[REQUEST_KEY_SALE_KBN])));

			// 商品セール名称
			ddlProductSaleName.Items.Cast<ListItem>().ToList().ForEach(li =>
				li.Selected = (li.Value == StringUtility.ToEmpty(Request[REQUEST_KEY_SALE_ID])));
		}
		else
		{
			m_iCurrentYear = (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR];
			m_iCurrentMonth = (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH];
		}

		// 消費税
		if (this.IsShippingsAndOrderItemAndIncludeDiscount)
		{
			// 売上基準が出荷基準かつ割引区分が割引後の場合、正確な返品税抜価格表示をできないため税抜を非表示にする
			rblTaxType.Items.Clear();

			foreach (ListItem li in ValueText.GetValueItemList(VALUETEXT_PARAM_ORDER_CONDITION_REPORT, "tax_type"))
			{
				li.Selected = false;
				if (li.Value == Constants.KBN_ORDERCONDITIONREPORT_TAX_EXCLUDE) continue;

				li.Selected = true;
				rblTaxType.Items.Add(li);
			}
		}
		else
		{
			var items = ValueText.GetValueItemList(VALUETEXT_PARAM_ORDER_CONDITION_REPORT, "tax_type");
			if (rblTaxType.Items.Count < items.Count)
			{
				rblTaxType.Items.Clear();
				// 消費税
				foreach (ListItem li in ValueText.GetValueItemList(VALUETEXT_PARAM_ORDER_CONDITION_REPORT, "tax_type"))
				{
					switch (Request.Params[Constants.REQUEST_KEY_TAX_DISPLAY_TYPE])
					{
						case Constants.KBN_ORDERCONDITIONREPORT_TAX_INCLUDED:
						case Constants.KBN_ORDERCONDITIONREPORT_TAX_EXCLUDE:
							li.Selected = (li.Value == Request[Constants.REQUEST_KEY_TAX_DISPLAY_TYPE]);
							break;

						default:
							li.Selected = (li.Value == Constants.KBN_ORDERCONDITIONREPORT_TAX_INCLUDED);
							break;
					}

					rblTaxType.Items.Add(li);
				}
			}
		}

		//------------------------------------------------------
		// 集計単位による絞込み条件切替
		//------------------------------------------------------
		// 注文単位の場合
		if (this.HasOrderAggregateUnit)
		{
			foreach (ListItem li in ddlBrandSearch.Items) li.Selected = false;
			if (Constants.PRODUCT_BRAND_ENABLED) ddlBrandSearch.Items[0].Selected = true;
			tbProductId.Text = "";
			ddlBrandSearch.Enabled = tbProductId.Enabled = ddlProductSaleKbn.Enabled = ddlProductSaleName.Enabled = false;
		}
		// 商品単位の場合
		else
		{
			ddlBrandSearch.Enabled = tbProductId.Enabled = ddlProductSaleKbn.Enabled = ddlProductSaleName.Enabled = true;
		}

		//------------------------------------------------------
		// カレンダ設定
		//------------------------------------------------------
		StringBuilder sbParamForCurrent = new StringBuilder();
		sbParamForCurrent.Append(REQUEST_KEY_SALES_TYPE).Append("=").Append(rblSalesType.SelectedValue);
		sbParamForCurrent.Append("&").Append(REQUEST_KEY_REPORT_TYPE).Append("=").Append(rblReportType.SelectedValue);
		sbParamForCurrent.Append("&").Append(REQUEST_KEY_PAYMENT_KBN).Append("=").Append("");	//	ダミー
		foreach (ListItem li in cblOrderPaymentKbns.Items)
		{
			if (li.Selected)
			{
				sbParamForCurrent.Append("&").Append(REQUEST_KEY_PAYMENT_KBN).Append("=").Append(li.Value);
			}
		}
		sbParamForCurrent.Append("&").Append(REQUEST_KEY_ORDER_KBN).Append("=").Append("");		// ダミー
		foreach (ListItem li in cblOrderKbns.Items)
		{
			if (li.Selected)
			{
				sbParamForCurrent.Append("&").Append(REQUEST_KEY_ORDER_KBN).Append("=").Append(li.Value);
			}
		}
		sbParamForCurrent.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(tbProductId.Text));
		sbParamForCurrent.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_BRAND_ID).Append("=").Append(ddlBrandSearch.SelectedValue);
		sbParamForCurrent.Append("&").Append(REQUEST_KEY_MALL_ID).Append("=").Append(ddlSiteName.SelectedValue);
		sbParamForCurrent.Append("&").Append(REQUEST_KEY_SALE_ID).Append("=").Append(ddlProductSaleName.SelectedValue);		// セール名
		sbParamForCurrent.Append("&").Append(REQUEST_KEY_SALE_KBN).Append("=").Append(ddlProductSaleKbn.SelectedValue);		// セール区分
		sbParamForCurrent.Append("&").Append(REQUEST_KEY_ORDER_TYPE).Append("=").Append("");		// ダミー
		cbOrderTypes.Items.Cast<ListItem>().Where(li => li.Selected).ToList().ForEach(li =>
		{
			sbParamForCurrent.Append("&").Append(REQUEST_KEY_ORDER_TYPE).Append("=").Append(li.Value);
		});
		sbParamForCurrent.Append("&").Append(REQUEST_KEY_AGGREGATE_UNIT).Append("=").Append(rblAggregateUnit.SelectedValue);
		sbParamForCurrent.Append("&").Append(Constants.REQUEST_KEY_TAX_DISPLAY_TYPE).Append("=").Append(rblTaxType.SelectedValue);
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(m_iCurrentYear, m_iCurrentMonth, Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ORDER_CONDITION_REPORT_LIST, sbParamForCurrent.ToString(), Constants.REQUEST_KEY_CURRENT_YEAR, Constants.REQUEST_KEY_CURRENT_MONTH);

		//------------------------------------------------------
		// 表示情報設定
		//------------------------------------------------------
		StringBuilder sbReportInfo = new StringBuilder();
		StringBuilder sbReportInfoOrderConditionDays = new StringBuilder();
		var displayTypeName = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT,
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TYPE,
			this.HasOrderAggregateUnit
				? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TYPE_ORDER
				: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TYPE_PRODUCT);
		if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT)
		{
			var periodFormat = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_PERIOD,
				KBN_REPORT_TYPE_DAILY_REPORT);
			sbReportInfo.Append(
				string.Format(
					periodFormat,
					displayTypeName,
					this.OrderConditionDatePeriodStartDateTimeString,
					this.OrderConditionDatePeriodEndDateTimeString
					));
		}
		else if (rblReportType.SelectedValue == KBN_REPORT_TYPE_MONTHLY_REPORT)
		{
			var periodFormat = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_PERIOD,
				KBN_REPORT_TYPE_MONTHLY_REPORT);
			sbReportInfo.Append(
				string.Format(
					periodFormat,
					m_iCurrentYear.ToString(),
					rblSalesType.SelectedItem.Text,
					displayTypeName));
		}

		lbReportInfo.Text =
			sbReportInfo + ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TAX,
				this.IsIncludedTax
					? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TAX_INCLUDED
					: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TAX_EXCLUDED);
		lbReportInfoOrderConditionDays.Text = sbReportInfoOrderConditionDays.ToString();

		// 商品セール名ドロップダウンリストを無効化（商品セール区分で通常を選択時）
		DisableProductSaleNameDropDownListWithSaleKbnNomal(ddlProductSaleKbn.SelectedValue);

		// 全検索にした場合に前の検索条件で集計されるのを防ぐ
		if (string.IsNullOrEmpty(ddlProductSaleKbn.SelectedValue))
		{
			ProcessProductSaleKbnTextChanged(ddlProductSaleKbn.SelectedValue);
		}

		// 集計データ取得
		GetData();

		// リピーターのデータバインド
		tdTotalCancelNumber.DataBind();
		rDataList.DataBind();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		//------------------------------------------------------
		// 決済種別チェックボックスリスト設定
		//------------------------------------------------------
		foreach (DataRowView drv in GetPaymentDataView(this.LoginOperatorShopId))
		{
			ListItem li = new ListItem((string)drv[Constants.FIELD_PAYMENT_PAYMENT_NAME], (string)drv[Constants.FIELD_PAYMENT_PAYMENT_ID]);
			cblOrderPaymentKbns.Items.Add(li);
		}

		//------------------------------------------------------
		// 注文区分チェックボックスリスト設定
		//------------------------------------------------------
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN))
		{
			// モバイルデータの表示OPがOFFの場合は注文区分がモバイル注文を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_ORDER_ORDER_KBN_MOBILE)) continue;
			cblOrderKbns.Items.Add(li);
		}

		//------------------------------------------------------
		// ブランドドロップダウン設定
		//------------------------------------------------------
		if (Constants.PRODUCT_BRAND_ENABLED)
		{
			ddlBrandSearch.Items.Add(
				new ListItem(
					Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
						"@@DispText.auto_text.unspecified@@",
						Constants.GLOBAL_OPTION_ENABLE
							? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
							: ""),
					""));
			foreach (DataRowView drv in ProductBrandUtility.GetProductBrandList())
			{
				ddlBrandSearch.Items.Add(new ListItem((string)drv[Constants.FIELD_PRODUCTBRAND_BRAND_ID], (string)drv[Constants.FIELD_PRODUCTBRAND_BRAND_ID]));
			}
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			cbCountryIsoCodeList.Items.AddRange(Constants.GLOBAL_CONFIGS.GlobalSettings.CountryIsoCodes.Select(cic => new ListItem(cic, cic)).ToArray());
		}

		// 期間指定ドロップダウン設定(デフォルトは今月)
		var orderConditionDateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		var orderConditionDateTo = new DateTime(
				DateTime.Now.Year,
				DateTime.Now.Month,
				DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month),
				DEFAULT_END_HOURS,
				DEFAULT_END_MINUTES,
				DEFAULT_END_SECONDS);
		ucOrderConditionDatePeriod.SetPeriodDate(
			orderConditionDateFrom,
			orderConditionDateTo);

		// 商品セール区分/商品セール名コントロールを初期化
		InitProductSaleControl();

		// データバインド
		DataBind();
	}

	/// <summary>
	/// データ取得・加工
	/// </summary>
	private void GetData()
	{
		Hashtable htInput = new Hashtable();
		int iGetDays = 0;

		//------------------------------------------------------
		// 期間指定
		//------------------------------------------------------
		if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT)
		{
			// 日毎
			DateTime dtDateFrom = new DateTime();
			DateTime dtDateTo = new DateTime();
			var strDateFrom = ucOrderConditionDatePeriod.StartDateTimeString;
			var strDateTo = ucOrderConditionDatePeriod.EndDateTimeString;

			if (Validator.IsDate(strDateFrom) && Validator.IsDate(strDateTo))
			{
				dtDateFrom = DateTime.Parse(strDateFrom);
				dtDateTo = DateTime.Parse(strDateTo);

				// From-ToがCONST_GET_ORDER_CONDITION_DAYSより大きかった場合は取得しない
				TimeSpan tsSubtractDate = new TimeSpan();
				tsSubtractDate = dtDateTo.Subtract(dtDateFrom);
				iGetDays = tsSubtractDate.Days + 1;
				if (iGetDays > int.Parse(CONST_GET_ORDER_CONDITION_DAYS))
				{
					lblOrderContitionError.Visible = true;
					lblOrderContitionError.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REPEAT_REPORT_OUT_OF_RANGE)
						.Replace("@@ 1 @@", CONST_GET_ORDER_CONDITION_DAYS);
					iGetDays = 0;
				}
				else if (iGetDays <= 0)
				{
					lblOrderContitionError.Visible = true;
					lblOrderContitionError.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REPEAT_REPORT_INCORRECT_DATE);
					iGetDays = 0;
				}
				else
				{
					lblOrderContitionError.Visible = false;
					lblOrderContitionError.Text = "";
				}
			}
			else
			{
				// エラー回避の為現在日を格納
				dtDateFrom = DateTime.Now;
				dtDateTo = DateTime.Now;
				lblOrderContitionError.Visible = true;
				lblOrderContitionError.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REPEAT_REPORT_INVALID_DATE);
				iGetDays = 0;
			}

			htInput.Add("year", dtDateFrom.Year);
			htInput.Add("month", dtDateFrom.Month);
			htInput.Add("day", dtDateFrom.Day);
			htInput.Add("yearto", dtDateTo.Year);
			htInput.Add("monthto", dtDateTo.Month);
			htInput.Add("dayto", dtDateTo.Day);
			htInput.Add("number_of_days", iGetDays);
			htInput.Add(Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_FROM, ucOrderConditionDatePeriod.StartTimeString);
			htInput.Add(Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_TO, ucOrderConditionDatePeriod.EndTimeString);
		}
		else
		{
			// 月毎
			htInput.Add("year", m_iCurrentYear.ToString());
			htInput.Add("month", m_iCurrentMonth.ToString());
		}

		htInput.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, tbProductId.Text);
		htInput.Add(Constants.FIELD_ORDERITEM_BRAND_ID, ddlBrandSearch.SelectedValue);
		htInput.Add(Constants.FIELD_ORDER_MALL_ID, ddlSiteName.SelectedValue);
		htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, ddlProductSaleName.SelectedValue);		// セールID
		htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN, ddlProductSaleKbn.SelectedValue);		// セール区分

		//------------------------------------------------------
		// 決済種別(複数)
		//------------------------------------------------------
		StringBuilder sbOrderPaymentKbns = new StringBuilder();
		foreach (ListItem li in cblOrderPaymentKbns.Items)
		{
			if (li.Selected)
			{
				if (sbOrderPaymentKbns.Length != 0)
				{
					sbOrderPaymentKbns.Append(",");
				}
				sbOrderPaymentKbns.Append("'" + li.Value.Replace("'", "''") + "'");
			}
		}
		sbOrderPaymentKbns.Append(sbOrderPaymentKbns.Length != 0 ? "" : "''");

		//------------------------------------------------------
		// 購買区分(複数)
		//------------------------------------------------------
		StringBuilder sbOrderKbns = new StringBuilder();
		foreach (ListItem li in cblOrderKbns.Items)
		{
			if (li.Selected)
			{
				if (sbOrderKbns.Length != 0)
				{
					sbOrderKbns.Append(",");
				}
				sbOrderKbns.Append("'" + li.Value.Replace("'", "''") + "'");
			}
		}
		sbOrderKbns.Append(sbOrderKbns.Length != 0 ? "" : "''");

		StringBuilder sbCountryIsoCode = new StringBuilder();
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			foreach (ListItem li in cbCountryIsoCodeList.Items)
			{
				if (li.Selected)
				{
					if (sbCountryIsoCode.Length != 0)
					{
						sbCountryIsoCode.Append(",");
					}
					sbCountryIsoCode.Append("'" + li.Value.Replace("'", "''") + "'");
				}
			}
		}

		StringBuilder sbCountryIsoCodeKbns = new StringBuilder();
		if (Constants.GLOBAL_OPTION_ENABLE && sbCountryIsoCode.Length > 0)
		{
			sbCountryIsoCodeKbns.Append(string.Format("{0}.{1} IN ({2})", Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE, sbCountryIsoCode.ToString()));
		}
		else
		{
			sbCountryIsoCodeKbns.Append("1 = 1");
		}

		// 通常注文/定期注文
		var orderTypes = new StringBuilder();
		var selectedOrderTypesAll = true;
		foreach (ListItem li in cbOrderTypes.Items)
		{
			if (li.Selected)
			{
				if (orderTypes.Length != 0) orderTypes.Append(" OR ");
				switch (li.Value)
				{
					// 通常注文
					case "order":
						orderTypes.Append(string.Format("{0}.{1} = ''", Constants.TABLE_ORDER, Constants.FIELD_ORDER_FIXED_PURCHASE_ID));
						break;

					// 定期注文
					case "fixed_purchase":
						orderTypes.Append(string.Format("{0}.{1} <> ''", Constants.TABLE_ORDER, Constants.FIELD_ORDER_FIXED_PURCHASE_ID));
						break;

					case "subscription_box":
						orderTypes.Append(string.Format("{0}.{1} <> ''", Constants.TABLE_ORDER, Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID));
						break;
				}
			}
			else
			{
				selectedOrderTypesAll = false;
			}
		}
		// 全選択
		if (selectedOrderTypesAll)
		{
			orderTypes = new StringBuilder().Append("1 = 1");
		}
		// 未選択
		if (orderTypes.Length == 0)
		{
			orderTypes = new StringBuilder().Append("1 <> 1");
		}

		//------------------------------------------------------
		// 金額項目名
		//------------------------------------------------------
		var amountFieldName = GetAmountFieldName();

		// 売上状況取得
		DataView dvDetail = null;
		DataView dvDetailExchangeCancel = null;
		dvDetail = GetOrderCondition(htInput, sbOrderPaymentKbns, sbOrderKbns, orderTypes, amountFieldName, sbCountryIsoCodeKbns);
		if (rblSalesType.SelectedValue == KBN_SALES_TYPE_SHIP_REPORT)
		{
			dvDetailExchangeCancel = GetOrderCondition(htInput, sbOrderPaymentKbns, sbOrderKbns, orderTypes, GetAmountFieldName(true), sbCountryIsoCodeKbns, true);
		}

		// セッション格納（商品別販売数ランキングページ引継用）
		htInput["order_payment_kbns"] = sbOrderPaymentKbns.ToString();
		htInput["order_kbns"] = sbOrderKbns.ToString();
		htInput["country_iso_code_kbns"] = sbCountryIsoCodeKbns.ToString();
		htInput["order_types"] = orderTypes.ToString();
		htInput["sales_type"] = rblSalesType.SelectedValue;
		htInput["tax_included_flg"] = this.IsIncludedTax ? "1" : "0";
		htInput["AmountType"] = this.HasOrderAggregateUnit;
		htInput[Constants.REQUEST_KEY_PRODUCT_UNIT_TYPE] = "1";
		Session[Constants.SESSION_KEY_PARAM] = htInput;

		//------------------------------------------------------
		// データ格納ループ回数を設定
		//------------------------------------------------------
		int iLastIndex = 0;
		if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT)
		{
			// 日毎
			iLastIndex = iGetDays;
		}
		else
		{
			// 月毎
			iLastIndex = 12;
		}

		//------------------------------------------------------
		// データ格納
		//------------------------------------------------------
		decimal dPriceTotal = 0;
		int iCountTotal = 0;
		decimal dMinusPriceTotal = 0;
		int iMinusCountTotal = 0;
		decimal dSubtotalPriceTotal = 0;
		int iSubtotalCounttTotal = 0;

		int iDataCount = 0;
		for (int iLoop = 1; iLoop <= iLastIndex; iLoop++)
		{
			// 日付取得
			Hashtable htData = new Hashtable();
			StringBuilder sbTargetDate = new StringBuilder();
			if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT)
			{
				// 日毎
				sbTargetDate.Append(
					DateTimeUtility.ToStringForManager(
						new DateTime(
							int.Parse(dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_YEAR].ToString()),
							int.Parse(dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_MONTH].ToString()),
							int.Parse(dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_DAY].ToString())),
						DateTimeUtility.FormatType.LongDateWeekOfDay1Letter));

				htData["class"] = GetWeekClass(int.Parse(dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_YEAR].ToString()), int.Parse(dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_MONTH].ToString()), int.Parse(dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_DAY].ToString()));
			}
			else
			{
				// 月毎
				sbTargetDate.Append(iLoop.ToString()).Append(string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) ? "月" : "");
			}
			htData[FIELD_ORDERCONDITION_TARGET_DATE] = sbTargetDate.ToString();
			htData[Constants.REQUEST_KEY_TARGET_YEAR] = dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_YEAR].ToString();
			htData[Constants.REQUEST_KEY_TARGET_MONTH] = dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_MONTH].ToString();
			var targetDay = StringUtility.ToEmpty(dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_DAY]);
			htData[Constants.REQUEST_KEY_TARGET_DAY] = targetDay;

			if (Validator.IsDate(ucOrderConditionDatePeriod.StartDateTimeString))
			{
				var startDate = StringUtility.ToEmpty(DateTime.Parse(ucOrderConditionDatePeriod.StartDateTimeString).Day);
				if (startDate == targetDay)
				{
					htData[Constants.REQUEST_KEY_REPORT_TIME_FROM] = ucOrderConditionDatePeriod.StartTimeString;
				};
			}

			if (Validator.IsDate(ucOrderConditionDatePeriod.EndDateTimeString))
			{
				var endDate = StringUtility.ToEmpty(DateTime.Parse(ucOrderConditionDatePeriod.EndDateTimeString).Day);
				if (endDate == targetDay)
				{
					htData[Constants.REQUEST_KEY_REPORT_TIME_TO] = ucOrderConditionDatePeriod.EndTimeString;
				};
			}

			// データ取得可否判定
			bool blGetData = false;
			if (iDataCount < dvDetail.Count)
			{
				if (((rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT))
					|| ((iLoop == int.Parse(dvDetail[iDataCount][Constants.REQUEST_KEY_TARGET_MONTH].ToString())) && (rblReportType.SelectedValue != KBN_REPORT_TYPE_DAILY_REPORT)))
				{
					blGetData = true;
				}
			}

			// 売上取得
			ArrayList alLine = new ArrayList();
			if (rblSalesType.SelectedValue == KBN_SALES_TYPE_ORDER_REPORT)
			{
				// 注文基準
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_ORDERED_AMOUNT, blGetData, KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_ORDERED_COUNT, blGetData, KBN_OPERATION_TYPE_COUNT);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_ORDERED_AMOUNT_AVG, blGetData, KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_CANCELED_AMOUNT, blGetData, KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_CANCELED_COUNT, blGetData, KBN_OPERATION_TYPE_COUNT);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_AMOUNT, blGetData, KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_COUNT, blGetData, KBN_OPERATION_TYPE_COUNT);

				if (blGetData)
				{
					dPriceTotal += (decimal)dvDetail[iDataCount][FIELD_ORDERCONDITION_ORDERED_AMOUNT];
					iCountTotal += (int)dvDetail[iDataCount][FIELD_ORDERCONDITION_ORDERED_COUNT];
					dMinusPriceTotal += (decimal)dvDetail[iDataCount][FIELD_ORDERCONDITION_CANCELED_AMOUNT];
					iMinusCountTotal += (int)dvDetail[iDataCount][FIELD_ORDERCONDITION_CANCELED_COUNT];
					dSubtotalPriceTotal += (decimal)dvDetail[iDataCount][FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_AMOUNT];
					iSubtotalCounttTotal += (int)dvDetail[iDataCount][FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_COUNT];
				}

			}
			else if (rblSalesType.SelectedValue == KBN_SALES_TYPE_SHIP_REPORT)
			{
				// 出荷基準
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_SHIPPED_AMOUNT, blGetData, KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_SHIPPED_COUNT, blGetData, KBN_OPERATION_TYPE_COUNT);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_SHIPPED_AMOUNT_AVG, blGetData, KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_RETURNED_AMOUNT, blGetData, KBN_OPERATION_TYPE_AMOUNT, dvDetailExchangeCancel[iDataCount]);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_RETURNED_COUNT, blGetData, KBN_OPERATION_TYPE_COUNT, dvDetailExchangeCancel[iDataCount]);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_AMOUNT, blGetData, KBN_OPERATION_TYPE_AMOUNT, dvDetailExchangeCancel[iDataCount]);
				CreateDispData(alLine, dvDetail[iDataCount], FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_COUNT, blGetData, KBN_OPERATION_TYPE_COUNT, dvDetailExchangeCancel[iDataCount]);

				if (blGetData)
				{
					dPriceTotal += (decimal)dvDetail[iDataCount][FIELD_ORDERCONDITION_SHIPPED_AMOUNT];
					iCountTotal += (int)dvDetail[iDataCount][FIELD_ORDERCONDITION_SHIPPED_COUNT];
					dMinusPriceTotal += (decimal)dvDetail[iDataCount][FIELD_ORDERCONDITION_RETURNED_AMOUNT] + (decimal)dvDetailExchangeCancel[iDataCount][FIELD_ORDERCONDITION_RETURNED_AMOUNT];
					iMinusCountTotal += (int)dvDetail[iDataCount][FIELD_ORDERCONDITION_RETURNED_COUNT] + (int)dvDetailExchangeCancel[iDataCount][FIELD_ORDERCONDITION_RETURNED_COUNT];
					dSubtotalPriceTotal += (decimal)dvDetail[iDataCount][FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_AMOUNT];
					iSubtotalCounttTotal += (int)dvDetail[iDataCount][FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_COUNT];
				}
			}

			htData.Add("data", alLine);
			this.DisplayDataList.Add(htData);

			if (blGetData == true)
			{
				iDataCount++;
			}
		}

		// 平均購入金額設定
		decimal dAvgTotal = 0;
		dAvgTotal = (dPriceTotal / (iCountTotal == 0 ? 1 : iCountTotal));

		// 合計
		lbPriceTotal.Text = dPriceTotal.ToPriceString(true);
		lbCountTotal.Text = StringUtility.ToNumeric(iCountTotal);
		lbAvgTotal.Text = dAvgTotal.ToPriceString(true);
		lbMinusPriceTotal.Text = dMinusPriceTotal.ToPriceString(true);
		lbMinusCountTotal.Text = StringUtility.ToNumeric(iMinusCountTotal);
		lbSubtotalPriceTotal.Text = dSubtotalPriceTotal.ToPriceString(true);
		lbSubtotalCountTotal.Text = StringUtility.ToNumeric(iSubtotalCounttTotal);

		// 「月日平均購入金額 日 Or 月」
		var date = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN,
				(rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT)
					? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN_DAY
					: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN_MONTH);
		// 「平均購入金額 Or 平均商品金額」
		tdDateAvgInfo.InnerText = date + ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_AVERAGE_KBN,
				this.HasOrderAggregateUnit
					? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_AVERAGE_KBN_PURCHASE
					: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_AVERAGE_KBN_PRODUCT);
		decimal averagePrice = (dSubtotalPriceTotal / (iDataCount == 0 ? 1 : iDataCount));
		tdDateAvgPriceInfo.InnerText = averagePrice.ToPriceString(true);
	}

	/// <summary>
	/// １カラム分表示データ作成
	/// </summary>
	/// <param name="alList">追加先リスト</param>
	/// <param name="drvSource">追加元DataView</param>
	/// <param name="strColumns">対象カラム</param>
	/// <param name="blInsertData">実データ追加可否</param>
	/// <param name="strOperation">表示タイプ</param>
	/// <param name="exchangeCancelSource">交換キャンセル</param>
	/// <returns>表示データの配列</returns>
	private void CreateDispData(ArrayList alList, DataRowView drvSource, string strColumns, bool blInsertData, string strOperation, DataRowView exchangeCancelSource = null)
	{
		if (blInsertData)
		{
			// 金額表示の場合
			if (strOperation == KBN_OPERATION_TYPE_AMOUNT)
			{
				var exchangeCancel = (exchangeCancelSource != null) ? (decimal?)exchangeCancelSource[strColumns] : 0;
				alList.Add(((decimal)drvSource[strColumns] + exchangeCancel).ToPriceString(true));
			}
			// 件数表示
			else if (strOperation == KBN_OPERATION_TYPE_COUNT)
			{
				var exchangeCancel = (exchangeCancelSource != null) ? (int?)exchangeCancelSource[strColumns] : 0;
				alList.Add(string.Format(
					((this.HasOrderAggregateUnit)
						? ReplaceTag("@@DispText.common_message.unit_of_quantity@@")
						: ReplaceTag("@@DispText.common_message.unit_of_quantity2@@")),
					StringUtility.ToNumeric((int?)drvSource[strColumns] + exchangeCancel)));
			}
		}
		else
		{
			alList.Add("－");
		}
	}

	/// <summary>
	/// 決済種別情報取得取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <returns>決済種別情報取得報データビュー</returns>
	private DataView GetPaymentDataView(string strShopId)
	{
		DataView dvResult = null;

		// ステートメントからカテゴリデータ取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("OrderConditionReport", "GetPaymentValidList"))
		{
			// パラメータ設定
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PAYMENT_SHOP_ID, strShopId);		// 店舗ID

			// SQL発行
			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		if (Constants.AMAZON_PAYMENT_OPTION_ENABLED == false)
		{
			dvResult.RowFilter = string.Format("{0} <> '{1}' ", Constants.FIELD_PAYMENT_PAYMENT_ID, Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT);
		}

		return dvResult;
	}

	/// <summary>
	/// 商品セール系のコントロールを初期化
	/// </summary>
	private void InitProductSaleControl()
	{
		// セール区分
		InitProductSaleKbnDropDownList();

		// セール名称
		InitProductSaleNameDropDownList();

		// 商品セール名ドロップダウンリストにデータバインド（初期表示は全件表示）
		BindProductSaleNameDropDownList(string.Empty);

		// 商品セール区分リクエストがあれば、リクエスト値で商品セール名DDLをデータバインド
		var saleKbn = StringUtility.ToEmpty(Request[REQUEST_KEY_SALE_KBN]);
		if (string.IsNullOrEmpty(saleKbn) == false)
		{
			ProcessProductSaleKbnTextChanged(saleKbn);
		}
	}

	/// <summary>
	/// 商品セール区分ドロップダウンリストを初期化
	/// </summary>
	private void InitProductSaleKbnDropDownList()
	{
		ddlProductSaleKbn.Items.Add(new ListItem("", ""));
		ddlProductSaleKbn.Items.AddRange(
			ValueText.GetValueItemArray(VALUETEXT_PARAM_ORDER_CONDITION_REPORT, Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN));
	}

	/// <summary>
	/// 商品セール名ドロップダウンリストを初期化
	/// </summary>
	private void InitProductSaleNameDropDownList()
	{
		ddlProductSaleName.Items.Add(new ListItem("", ""));
		new ProductSaleService().GetValidAllByProductSaleKbn(Constants.CONST_DEFAULT_SHOP_ID, ddlProductSaleKbn.SelectedValue)
			.ToList().ForEach(productSale => ddlProductSaleName.Items.Add(new ListItem(productSale.ProductsaleName, productSale.ProductsaleId)));
	}

	/// <summary>
	/// 商品セール区分テキスト変更時の処理
	/// </summary>
	/// <param name="productSaleKbn">商品セール区分</param>
	private void ProcessProductSaleKbnTextChanged(string productSaleKbn)
	{
		// 商品セール名ドロップダウンリストを無効化（商品セール区分で通常を選択時）
		DisableProductSaleNameDropDownListWithSaleKbnNomal(productSaleKbn);

		// 商品セール名ドロップダウンリストにデータバインド
		BindProductSaleNameDropDownList(productSaleKbn);
	}

	/// <summary>
	/// 商品名ドロップダウンリストを無効化する
	/// </summary>
	/// <param name="productSaleKbn">商品セール区分</param>
	private void DisableProductSaleNameDropDownListWithSaleKbnNomal(string productSaleKbn)
	{
		// セール区分に「通常」が選択されている場合、商品セール名ドロップダウンリストを無効化
		if (productSaleKbn == VALUETEXT_PARAM_PRODUCTSALE_KBN_NOMAL) ddlProductSaleName.Enabled = false;
	}

	/// <summary>
	/// 商品セール名ドロップダウンリストにデータバインド
	/// </summary>
	/// <param name="productSaleKbn">商品セール区分</param>
	private void BindProductSaleNameDropDownList(string productSaleKbn)
	{
		// 初期化
		ddlProductSaleName.Items.Clear();
		ddlProductSaleName.Items.Add(new ListItem("", ""));

		// 商品セール区分を元に商品セール名ドロップダウンリストをデータバインド
		GetProductSaleByProductSaleKbn(productSaleKbn)
			.ToList().ForEach(productSale => ddlProductSaleName.Items.Add(new ListItem(productSale.ProductsaleName, productSale.ProductsaleId)));
	}

	/// <summary>
	/// 商品セール区分から商品セール情報を取得
	/// </summary>
	/// <param name="productSaleKbn">商品セール区分</param>
	/// <returns>商品セール情報</returns>
	private ProductSaleModel[] GetProductSaleByProductSaleKbn(string productSaleKbn)
	{
		// 商品セール区分が空であれば商品セール情報を全取得。そうでない場合は商品セール区分に該当する商品セール情報を取得。
		return (string.IsNullOrEmpty(productSaleKbn)) ? new ProductSaleService().GetValidAll(Constants.CONST_DEFAULT_SHOP_ID)
			 : new ProductSaleService().GetValidAllByProductSaleKbn(Constants.CONST_DEFAULT_SHOP_ID, productSaleKbn);
	}

	/// <summary>
	/// 売上状況取得
	/// </summary>
	/// <param name="htInput">検索パラメータ</param>
	/// <param name="sbOrderPaymentKbns">決済種別</param>
	/// <param name="sbOrderKbns">購買区分</param>
	/// <param name="orderTypes">通常注文/定期注文</param>
	/// <param name="amountFieldName">金額項目名</param>
	/// <param name="sbCountryIsoCodeKbns">国ISOコード区分</param>
	/// <param name="isExchangeCancel">交換キャンセルか</param>
	/// <returns>売上状況</returns>
	private DataView GetOrderCondition(
		Hashtable htInput,
		StringBuilder sbOrderPaymentKbns,
		StringBuilder sbOrderKbns,
		StringBuilder orderTypes,
		StringBuilder amountFieldName,
		StringBuilder sbCountryIsoCodeKbns,
		bool isExchangeCancel = false)
	{
		var statementName = "";
		if (isExchangeCancel)
		{
			statementName = (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT) ? "GetOrderExchangeCancelConditionDay" : "GetOrderExchangeCancelConditionMonth";
		}
		else
		{
			statementName = (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT) ? "GetOrderConditionDay" : "GetOrderConditionMonth";
		}

		// レポート種別の選択が「日別レポート」なら日毎、そうでなければ月毎
		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement("OrderConditionReport", statementName))
		{
			// 決済種別を置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(FIELD_ORDERCONDITION_ORDER_PAYMENT_KBNS, sbOrderPaymentKbns.ToString());
			// 購買区分を置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(FIELD_ORDERCONDITION_ORDER_KBNS, sbOrderKbns.ToString());
			// 国ISOコード区分を置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(FIELD_ORDERCONDITION_COUNTRY_ISO_CODE_KBNS, sbCountryIsoCodeKbns.ToString());
			// 通常注文/定期注文
			sqlStatement.Statement = sqlStatement.Statement.Replace(FIELD_ORDERCONDITION_ORDER_TYPES, orderTypes.ToString());
			// 金額項目名
			sqlStatement.Statement = sqlStatement.Statement.Replace(FIELD_ORDERCONDITION_AMOUNT_FIELD_NAME, amountFieldName.ToString());
			// 件数項目名
			sqlStatement.Statement
				= sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_COUNT_FIELD_NAME,
					GetCountFieldName(isExchangeCancel));
			// テーブル名
			sqlStatement.Statement = sqlStatement.Statement.Replace(FIELD_ORDERCONDITION_TABLE_NAME, GetTableNameStatement(this.HasOrderAggregateUnit, isExchangeCancel));
			// 日別月別指定
			sqlStatement.Statement = sqlStatement.Statement.Replace(FIELD_ORDERCONDITION_SALES_KBN_TARGET, (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT) ? "ymd" : "ym");
			// 商品検索条件
			var isInputProductInfo
				= ((tbProductId.Text != "") || (ddlBrandSearch.SelectedValue != "") || (ddlProductSaleKbn.SelectedValue != "") || (ddlProductSaleName.SelectedValue != ""));
			var statement = GetProductSearchConditionStatement(this.HasOrderAggregateUnit, isInputProductInfo, ddlProductSaleKbn.SelectedValue);
			sqlStatement.Statement = sqlStatement.Statement.Replace(FIELD_ORDERCONDITION_ITEM_SEARCH_CONDITION, statement);
			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
	}

	/// <summary>
	/// テーブル名SQL文を取得
	/// </summary>
	/// <param name="hasOrderAggregateUnit">集計単位（注文基準か？）</param>
	/// <param name="isExchangeCansel">交換キャンセルか</param>
	/// <returns>テーブル名SQL文</returns>
	private string GetTableNameStatement(bool hasOrderAggregateUnit, bool isExchangeCansel = false)
	{
		var tableName = "FROM  w2_Order  ";

		if (hasOrderAggregateUnit == false)
		{
			tableName += "LEFT JOIN  w2_OrderItem  ON  w2_Order.order_id = w2_OrderItem.order_id  "
			+ "LEFT JOIN  w2_ProductSale  ON  w2_OrderItem.productsale_id = w2_ProductSale.productsale_id  ";
		}
		else if (isExchangeCansel)
		{
			tableName += "LEFT JOIN  w2_OrderItem  ON  w2_Order.order_id = w2_OrderItem.order_id  ";
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			tableName += "LEFT JOIN w2_OrderOwner ON  w2_Order.order_id = w2_OrderOwner.order_id  ";
		}



		return tableName;
	}

	/// <summary>
	/// 金額項目名
	/// </summary>
	/// <param name="isExchangeCancel">交換キャンセルか</param>
	/// <returns></returns>
	private StringBuilder GetAmountFieldName(bool isExchangeCancel = false)
	{
		var amountFieldName = new StringBuilder();
		var itemtable = isExchangeCancel
			? "OrderItems"
			: "w2_OrderItem";

		if ((this.HasOrderAggregateUnit && (isExchangeCancel == false)))
		{
			amountFieldName.Append("w2_Order.order_price_total")
				.Append(this.IsIncludedTax ? "" : " - w2_Order.order_price_tax");
		}
		else
		{
			if (this.IsIncludeDiscount)
			{
				amountFieldName.Append(
					string.Format(
						(this.IsIncludedTax)
							? "{0}.item_discounted_price"
							: "{0}.item_discounted_price / ( 1 + {0}.product_tax_rate / 100 )",
						itemtable));
			}
			else
			{
				amountFieldName.Append(
					string.Format(
						(this.IsIncludedTax)
							? "{0}.product_price_pretax * {0}.item_quantity"
							: "{0}.product_price_pretax  * {0}.item_quantity - {0}.item_price_tax",
						itemtable));
			}
		}

		return amountFieldName;
	}

	/// <summary>
	/// 件数項目名
	/// </summary>
	/// <param name="isExchangeCancel">交換キャンセルか</param>
	/// <returns>件数項目名</returns>
	private string GetCountFieldName(bool isExchangeCancel = false)
	{
		var tableName = this.HasOrderAggregateUnit ? "w2_Order" : "w2_OrderItem";
		var countFieldName = string.Format(
			this.HasOrderAggregateUnit
				? "COUNT({0}.order_id)"
				: "SUM({0}.item_quantity)",
				isExchangeCancel ? "OrderItems" : tableName);
		return countFieldName;
	}

	/// <summary>
	/// 商品検索条件SQL文を取得
	/// </summary>
	/// <param name="hasOrderAggregateUnit">集計単位（注文基準か？）</param>
	/// <param name="isInputProductInfo">商品検索の入力値があるか？</param>
	/// <param name="productSaleKbn">商品セール区分選択値</param>
	/// <returns>商品検索条件SQL文</returns>
	private string GetProductSearchConditionStatement(bool hasOrderAggregateUnit, bool isInputProductInfo, string productSaleKbn)
	{
		var result = ((hasOrderAggregateUnit == false) && (isInputProductInfo == false))
			? "AND  (w2_Order.subscription_box_fixed_amount IS NULL)"
			: (hasOrderAggregateUnit)
				? ""
			: (productSaleKbn == VALUETEXT_PARAM_PRODUCTSALE_KBN_NOMAL)
			? "AND  (@product_id = '' OR w2_OrderItem.product_id = @product_id)  "
			+ "AND  (@brand_id = '' OR w2_OrderItem.brand_id = @brand_id)  "
						+ "AND  (w2_OrderItem.productsale_id = '')  "
						+ "AND  (w2_Order.subscription_box_fixed_amount IS NULL)"
			: "AND  (@product_id = '' OR w2_OrderItem.product_id = @product_id)  "
			+ "AND  (@brand_id = '' OR w2_OrderItem.brand_id = @brand_id)  "
			+ "AND  (@productsale_kbn = '' OR w2_ProductSale.productsale_kbn = @productsale_kbn)  "
						+ "AND  (@productsale_id = '' OR w2_OrderItem.productsale_id = @productsale_id)  "
						+ "AND  (w2_Order.subscription_box_fixed_amount IS NULL)";
		return result;
	}

	/// <summary>
	/// 曜日用リストCSSクラス取得
	/// </summary>
	/// <param name="iYear">年</param>
	/// <param name="iMonth">月</param>
	/// <param name="iDay">日</param>
	/// <returns>曜日用リスト(土・日)</returns>
	protected string GetWeekClass(int iYear, int iMonth, int iDay)
	{
		string strrResult = "list_item_bg1";
		DateTime dt = new DateTime(iYear, iMonth, iDay);

		if (dt.DayOfWeek == DayOfWeek.Saturday)
		{
			strrResult = "list_item_bg_wk_sat";
		}
		else if (dt.DayOfWeek == DayOfWeek.Sunday)
		{
			strrResult = "list_item_bg_wk_sun";
		}

		return strrResult;
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		var sbRecords = new StringBuilder();

		//------------------------------------------------------
		// タイトル作成
		//------------------------------------------------------
		List<string> lTitleParams = new List<string>();
		lTitleParams.Add(string.Format(
			// 「売上状況レポート {0} {1}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_STATUS_REPORT),
			lbReportInfo.Text,
			lbReportInfoOrderConditionDays.Text));
		sbRecords.Append(CreateRecordCsv(lTitleParams));
		lTitleParams.Clear();
		sbRecords.Append(CreateRecordCsv(lTitleParams));

		//------------------------------------------------------
		// ヘッダ作成
		//------------------------------------------------------
		List<string> lHeaderParams = new List<string>();
		lHeaderParams.Add(string.Format(
			// 「{0}年」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN_YEAR),
			m_iCurrentYear));
		// 「件数 Or 個数」
		var orderUnit = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN,
				this.HasOrderAggregateUnit
					? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN_RESULT_QUANTITY
					: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN_ITEM_QUANTITY);
		if (rblSalesType.SelectedValue == KBN_SALES_TYPE_ORDER_REPORT)
		{
			lHeaderParams.Add(
				// 「売上(注文基準)：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_ORDER_BASIS_AMOUNT));

			lHeaderParams.Add(string.Format(
				// 「売上(注文基準)：{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_ORDER_BASIS),
				orderUnit));

			lHeaderParams.Add(
				// 「売上(注文基準)：平均購入単価」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_ORDER_BASIS_AVERAGE));

			lHeaderParams.Add(
				// 「キャンセル：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_CANCEL_AMOUNT));
			lHeaderParams.Add(string.Format(
				// 「キャンセル：{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_CANCEL),
				orderUnit));

			lHeaderParams.Add(
				// 「小計：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL_AMOUNT));
			lHeaderParams.Add(string.Format(
				// 「小計：{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL),
				orderUnit));
		}
		else if (rblSalesType.SelectedValue == KBN_SALES_TYPE_SHIP_REPORT)
		{
			lHeaderParams.Add(
				// 「売上(出荷基準)：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SHIPPING_STANDARD_AMOUNT));
			lHeaderParams.Add(string.Format(
				// 「売上(出荷基準)：{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SHIPPING_STANDARD),
				orderUnit));

			lHeaderParams.Add(
				// 「売上(出荷基準)：平均購入単価」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SHIPPING_STANDARD_AVERAGE));

			lHeaderParams.Add(
				// 「返品：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_RETURN_AMOUNT));
			lHeaderParams.Add(string.Format(
				// 「返品：{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_RETURN),
				orderUnit));

			lHeaderParams.Add(
				// 「小計：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL_AMOUNT));
			lHeaderParams.Add(string.Format(
				// 「小計：{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL),
				orderUnit));
		}
		StringBuilder sbFileName = new StringBuilder();
		sbFileName.Append("OrderConditionReportList_").Append(m_iCurrentYear.ToString()).Append((rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT ? m_iCurrentMonth.ToString("00") : ""));
		sbRecords.Append(CreateRecordCsv(lHeaderParams));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		List<string> lDataParams = new List<string>();
		// 合計
		lDataParams.Add(
			// 「合計」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TOTAL));
		lDataParams.Add(lbPriceTotal.Text.Replace(@"¥", ""));
		lDataParams.Add(lbCountTotal.Text);
		lDataParams.Add(lbAvgTotal.Text.Replace(@"¥", ""));
		lDataParams.Add(lbMinusPriceTotal.Text.Replace(@"¥", ""));
		lDataParams.Add(lbMinusCountTotal.Text);
		lDataParams.Add(lbSubtotalPriceTotal.Text.Replace(@"¥", ""));
		lDataParams.Add(lbSubtotalCountTotal.Text);
		sbRecords.Append(CreateRecordCsv(lDataParams));
		// 月別・日別
		foreach (Hashtable ht in this.DisplayDataList)
		{
			lDataParams.Clear();
			lDataParams.Add((string)ht[FIELD_ORDERCONDITION_TARGET_DATE]);
			foreach (string str in (ArrayList)ht["data"])
			{
				lDataParams.Add(str.Replace(@"¥", "").Replace("件", ""));
			}
			sbRecords.Append(CreateRecordCsv(lDataParams));
		}

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv(sbFileName.ToString(), sbRecords.ToString());
	}

	/// <summary>
	/// ドロップダウンリストチェンジイベント：商品セール区分
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlProductSaleKbn_TextChanged(object sender, EventArgs e)
	{
		ProcessProductSaleKbnTextChanged(ddlProductSaleKbn.SelectedValue);
	}

	/// <summary>
	/// 商品別販売個数ランキングレポートURL作成（合計）
	/// </summary>
	protected string CreateProductSaleRankingReportlUrlForTotal()
	{
		StringBuilder orderDetailUrl = new StringBuilder();
		orderDetailUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_PRODUCT_SALE_RANKING_REPORT);
		orderDetailUrl.Append("?").Append("total_flg").Append("=").Append("1");
		orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_YEAR).Append("=").Append(HttpUtility.UrlEncode(m_iCurrentYear.ToString()));
		orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=1");
		orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));


		return orderDetailUrl.ToString();
	}
	/// <summary>
	/// 商品別販売個数ランキングレポートURL作成（月別）
	/// </summary>
	/// <param name="month">月</param>
	protected string CreateProductSaleRankingReportlUrlForMonth(object month)
	{
		return CreateProductSaleRankingReportlUrl(m_iCurrentYear, month, null);
	}
	/// <summary>
	/// 商品別販売個数ランキングレポートURL作成(日別)
	/// </summary>
	/// <param name="year">年</param>
	/// <param name="month">月</param>
	/// <param name="day">日</param>
	/// <param name="timeFrom">Time from</param>
	/// <param name="timeTo">Time to</param>
	/// <returns>Product sale ranking reportl url</returns>
	protected string CreateProductSaleRankingReportlUrl(
		object year,
		object month,
		object day,
		object timeFrom = null,
		object timeTo = null)
	{
		StringBuilder orderDetailUrl = new StringBuilder();
		orderDetailUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_PRODUCT_SALE_RANKING_REPORT);
		orderDetailUrl.Append("?").Append(Constants.REQUEST_KEY_TARGET_YEAR).Append("=").Append(HttpUtility.UrlEncode(year.ToString()));
		orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_MONTH).Append("=").Append(HttpUtility.UrlEncode(month.ToString()));
		if (day != null) orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_DAY).Append("=").Append(HttpUtility.UrlEncode(day.ToString()));
		orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=1");
		orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));

		if (timeFrom != null)
		{
			orderDetailUrl
				.Append("&")
				.Append(Constants.REQUEST_KEY_REPORT_TIME_FROM)
				.Append("=")
				.Append(HttpUtility.UrlEncode(timeFrom.ToString()));
		}

		if (timeTo != null)
		{
			orderDetailUrl
				.Append("&")
				.Append(Constants.REQUEST_KEY_REPORT_TIME_TO)
				.Append("=")
				.Append(HttpUtility.UrlEncode(timeTo.ToString()));
		}

		return orderDetailUrl.ToString();
	}

	/// <summary>
	/// 時間別レポートページ遷移(日別)
	/// </summary>
	/// <param name="year">年</param>
	/// <param name="month">月</param>
	/// <param name="day">日</param>
	/// <param name="timeFrom">Time from</param>
	/// <param name="timeTo">Time to</param>
	/// <returns>時間毎日別レポートURL</returns>
	protected string CreateDayTimeReport(
		object year,
		object month,
		object day,
		object timeFrom,
		object timeTo)
	{
		var daytime = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TIME_REPORT)
			.AddParam(Constants.REQUEST_KEY_TARGET_YEAR, HttpUtility.UrlEncode(year.ToString()))
			.AddParam(Constants.REQUEST_KEY_TARGET_MONTH, HttpUtility.UrlEncode(month.ToString()));
		if (day != null)
		{
			daytime.AddParam(Constants.REQUEST_KEY_TARGET_DAY, HttpUtility.UrlEncode(day.ToString()));
		}

		if (timeFrom != null)
		{
			daytime.AddParam(
				Constants.REQUEST_KEY_REPORT_TIME_FROM,
				HttpUtility.UrlEncode(timeFrom.ToString()));
		}

		if (timeTo != null)
		{
			daytime.AddParam(
				Constants.REQUEST_KEY_REPORT_TIME_TO,
				HttpUtility.UrlEncode(timeTo.ToString()));
		}

		daytime.AddParam(Constants.REQUEST_KEY_PAGE_NO, "1")
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP))
			.AddParam("ReportSettingType", HttpUtility.UrlEncode(rblReportType.SelectedValue))
			.AddParam("ReportSalesType", HttpUtility.UrlEncode(rblSalesType.SelectedValue))
			.AddParam("ReportTaxType", HttpUtility.UrlEncode(rblTaxType.SelectedValue))
			.AddParam("BrandSearch", HttpUtility.UrlEncode(ddlBrandSearch.SelectedValue))
			.AddParam("ProductSaleKbn", HttpUtility.UrlEncode(ddlProductSaleKbn.SelectedValue))
			.AddParam("ProductSaleName", HttpUtility.UrlEncode(ddlProductSaleName.SelectedValue))
			.AddParam("ProductId", HttpUtility.UrlEncode(tbProductId.Text))
			.AddParam("year", HttpUtility.UrlEncode(year.ToString()))
			.AddParam("month", HttpUtility.UrlEncode(month.ToString()))
			.AddParam("day", HttpUtility.UrlEncode(day.ToString()))
			.AddParam("aggregateUnit", HttpUtility.UrlEncode(rblAggregateUnit.SelectedValue));

		return daytime.CreateUrl();
	}
	/// <summary>
	/// 時間別レポートページ遷移(月別)
	/// </summary>
	/// <param name="year">年</param>
	/// <param name="month">月</param>
	/// /// <returns>時間毎月別レポートURL</returns>
	protected string CreateMonthTimeReport(object year, object month)
	{
		var monthtime = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TIME_REPORT)
			.AddParam(Constants.REQUEST_KEY_TARGET_YEAR, HttpUtility.UrlEncode(m_iCurrentYear.ToString()))
			.AddParam(Constants.REQUEST_KEY_TARGET_MONTH, HttpUtility.UrlEncode(month.ToString()))
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP))
			.AddParam("ReportSettingType", HttpUtility.UrlEncode(rblReportType.SelectedValue))
			.AddParam("ReportSalesType", HttpUtility.UrlEncode(rblSalesType.SelectedValue))
			.AddParam("ReportTaxType", HttpUtility.UrlEncode(rblTaxType.SelectedValue))
			.AddParam("BrandSearch", HttpUtility.UrlEncode(ddlBrandSearch.SelectedValue))
			.AddParam("ProductSaleKbn", HttpUtility.UrlEncode(ddlProductSaleKbn.SelectedValue))
			.AddParam("ProductSaleName", HttpUtility.UrlEncode(ddlProductSaleName.SelectedValue))
			.AddParam("ProductId", HttpUtility.UrlEncode(tbProductId.Text))
			.AddParam("year", HttpUtility.UrlEncode(year.ToString()))
			.AddParam("month", HttpUtility.UrlEncode(month.ToString()))
			.AddParam("aggregateUnit", HttpUtility.UrlEncode(rblAggregateUnit.SelectedValue));

		return monthtime.CreateUrl();
	}

	/// <summary>
	/// キャンセル注文表示URL取得
	/// </summary>
	/// <param name="index">リピータのインデックス</param>
	/// <returns>キャンセル注文表示URL</returns>
	protected string CreateCancelOrderListUrl(int index = -1)
	{
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_LIST)
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_STATUS, FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
			.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS, Constants.FIELD_ORDER_ORDER_CANCEL_DATE)
			.AddParam(
				Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DEFAULT_START_TIME.Remove(8))
			.AddParam(
				Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DEFAULT_END_TIME.Remove(8))
			.AddParam(Constants.REQUEST_KEY_ORDER_MALL_ID, ddlSiteName.SelectedValue);
		var orderKbns = GetSelectedCheckBoxListValue(cblOrderKbns);
		if (orderKbns.Length == 1)
		{
			url.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_KBN, orderKbns[0]);
		}
		var orderPaymentKbns = GetSelectedCheckBoxListValue(cblOrderPaymentKbns);
		if (orderPaymentKbns.Length == 1)
		{
			url.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_PAYMENT_KBN, orderPaymentKbns[0]);
		}
		if (index >= 0)
		{
			// 個別件数対象
			var displayData = (Hashtable)this.DisplayDataList[index];
			if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT)
			{
				var urlDate = string.Format(
					FORMAT_CANCEL_DATE,
					displayData[Constants.REQUEST_KEY_TARGET_YEAR],
					int.Parse((string)displayData[Constants.REQUEST_KEY_TARGET_MONTH]),
					int.Parse((string)displayData[Constants.REQUEST_KEY_TARGET_DAY]));
				url.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM, urlDate)
					.AddParam(Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO, urlDate);
			}
			else
			{
				var lastDayOfMonth = DateTime.DaysInMonth(
					int.Parse((string)displayData[Constants.REQUEST_KEY_TARGET_YEAR]),
					int.Parse((string)displayData[Constants.REQUEST_KEY_TARGET_MONTH]));

				url.AddParam(
					Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM,
					string.Format(
						FORMAT_CANCEL_DATE,
						displayData[Constants.REQUEST_KEY_TARGET_YEAR],
						int.Parse((string)displayData[Constants.REQUEST_KEY_TARGET_MONTH]),
						int.Parse((string)displayData[Constants.REQUEST_KEY_TARGET_DAY]) + 1))
					.AddParam(
						Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO,
						string.Format(
							FORMAT_CANCEL_DATE,
							displayData[Constants.REQUEST_KEY_TARGET_YEAR],
							int.Parse((string)displayData[Constants.REQUEST_KEY_TARGET_MONTH]),
							lastDayOfMonth));
			}
		}
		else
		{
			// 合計件数対象
			if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT)
			{
				url.AddParam(
					Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM,
					ucOrderConditionDatePeriod.HfStartDate.Value)
					.AddParam(
						Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO,
						ucOrderConditionDatePeriod.HfEndDate.Value);
			}
			else
			{
				url.AddParam(
					Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM,
					string.Format(m_iCurrentYear + YEARS_START_DATE))
					.AddParam(
						Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO,
						string.Format(m_iCurrentYear + YEARS_END_DATE));
			}
		}
		return url.CreateUrl();
	}

	/// <summary>
	/// 選択されたチェックボックスリストの値取得
	/// </summary>
	/// <param name="list">チェックボックスリスト</param>
	/// <returns>選択されたチェックボックスリストの値</returns>
	private string[] GetSelectedCheckBoxListValue(CheckBoxList list)
	{
		var result = list.Items.Cast<ListItem>()
			.Where(i => i.Selected)
			.Select(i => i.Value)
			.ToArray();
		return result;
	}

	/// <summary>
	/// キャンセル件数リンクホバー時の表示文言取得
	/// </summary>
	/// <param name="format">表示文言フォーマット</param>
	/// <param name="index">リピータのインデックス</param>
	/// <returns>表示文言</returns>
	protected string GetTitleForCancel(string format, int index = -1)
	{
		if (index >= 0)
		{
			if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT)
			{
				return (string)((Hashtable)this.DisplayDataList[index])[FIELD_ORDERCONDITION_TARGET_DATE];
			}
			var displayData = (Hashtable)this.DisplayDataList[index];
			return string.Format(
				format,
				displayData[Constants.REQUEST_KEY_TARGET_YEAR],
				displayData[Constants.REQUEST_KEY_TARGET_MONTH]);
		}

		if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DAILY_REPORT)
		{
			return string.Format(
				format,
				this.OrderConditionDatePeriodStartDateTimeString,
				this.OrderConditionDatePeriodEndDateTimeString);
		}
		return string.Format(format, m_iCurrentYear);
	}

	/// <summary>
	/// キャンセル件数リンク化判定
	/// </summary>
	/// <param name="itemIndex">リピータアイテムインデックス</param>
	/// <returns>リンク表示かどうか</returns>
	protected bool IsCancelOrderListLinkVisible(int itemIndex)
	{
		return (itemIndex == CANCEL_NUMBER_INDEX);
	}
	#region プロパティ
	/// <summary>集計単位</summary>
	protected bool HasOrderAggregateUnit
	{
		get
		{
			return (rblAggregateUnit.SelectedValue == KBN_AGGREGATE_UNIT_ORDER);
		}
	}
	/// <summary>税込表示か</summary>
	protected bool IsIncludedTax { get { return (rblTaxType.SelectedValue == Constants.KBN_ORDERCONDITIONREPORT_TAX_INCLUDED); } }
	/// <summary>割引後金額か</summary>
	protected bool IsIncludeDiscount { get { return (rblDiscountType.SelectedValue == KBN_DISCOUNT_INCLUDE); } }
	/// <summary>「出荷基準」かつ「小計（商品単位）」かつ「割引後」か</summary>
	protected bool IsShippingsAndOrderItemAndIncludeDiscount
	{
		get { return (rblDiscountType.SelectedValue == KBN_DISCOUNT_INCLUDE && rblSalesType.SelectedValue == KBN_SALES_TYPE_SHIP_REPORT && rblAggregateUnit.SelectedValue == KBN_AGGREGATE_UNIT_ORDERITEM); }
	}
	/// <summary>「注文基準」かつ「合計（注文単位）」かつ「通常注文/定期注文/頒布会注文」全体選択かつ「購買区分」全体もしくは単数選択かつ「国ISOコード」未選択かつ「決済種別」全体もしくは単数選択か</summary>
	protected bool IsCancelNumberLinkDisplay
	{
		get
		{
			var selectedOrderKbnsLength = GetSelectedCheckBoxListValue(cblOrderKbns).Length;
			var selectedCountryIsoCodeListLength = GetSelectedCheckBoxListValue(cbCountryIsoCodeList).Length;
			var selectedOrderPaymentKbnsLength = GetSelectedCheckBoxListValue(cblOrderPaymentKbns).Length;
			return rblSalesType.SelectedValue == KBN_SALES_TYPE_ORDER_REPORT
				&& rblAggregateUnit.SelectedValue == KBN_AGGREGATE_UNIT_ORDER
				&& (ucOrderConditionDatePeriod.HfStartTime.Value == DAILY_START_TIME)
				&& (ucOrderConditionDatePeriod.HfEndTime.Value == DAILY_END_TIME)
				&& (GetSelectedCheckBoxListValue(cbOrderTypes).Length == cbOrderTypes.Items.Count)
				&& ((selectedOrderKbnsLength == 1)
					|| (selectedOrderKbnsLength == cblOrderKbns.Items.Count))
				&& (selectedCountryIsoCodeListLength == 0)
				&& ((selectedOrderPaymentKbnsLength == 1)
					|| (selectedOrderPaymentKbnsLength == cblOrderPaymentKbns.Items.Count));
		}
	}
	/// <summary>期間指定開始日付</summary>
	private string OrderConditionDatePeriodStartDateTimeString
	{
		get
		{
			return DateTimeUtility.ToStringForManager(
				ucOrderConditionDatePeriod.StartDateTimeString,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
		}
	}
	/// <summary>期間指定終了日付</summary>
	private string OrderConditionDatePeriodEndDateTimeString
	{
		get
		{
			return DateTimeUtility.ToStringForManager(
				ucOrderConditionDatePeriod.EndDateTimeString,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
		}
	}
	/// <summary>レポートデータ</summary>
	protected ArrayList DisplayDataList { set { m_alDispData = value; } get { return m_alDispData; } }
	/// <summary>レポートデータ</summary>
	private ArrayList m_alDispData = new ArrayList();
	#endregion
}
