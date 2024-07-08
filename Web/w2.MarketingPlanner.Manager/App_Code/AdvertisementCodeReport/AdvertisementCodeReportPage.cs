/*
=========================================================================================================
  Module      : AdvertisementCodeReportPage の概要の説明です (AdvertisementCodeReportPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using w2.App.Common.Util;

/// <summary>
/// AdvertisementCodeReportPage の概要の説明です
/// </summary>
public class AdvertisementCodeReportPage : BasePage
{
	#region 定数
	/// <summary>集計区分</summary>
	protected const string KBN_ADVC_TOTAL_KBN_USER = "User";
	protected const string KBN_ADVC_TOTAL_KBN_ORDER = "Order";
	protected const string KBN_ADVC_TOTAL_KBN_PV = "Pv";
	protected const string KBN_ADVC_TOTAL_KBN_CVR = "Cvr";

	/// <summary>出力ファイル名</summary>
	protected const string EXPORT_FILE_NAME_INITIALS = "AdvCodeReportList_";
	protected const string EXPORT_FILE_NAME_USER = EXPORT_FILE_NAME_INITIALS + KBN_ADVC_TOTAL_KBN_USER;
	protected const string EXPORT_FILE_NAME_ORDER = EXPORT_FILE_NAME_INITIALS + KBN_ADVC_TOTAL_KBN_ORDER;
	protected const string EXPORT_FILE_NAME_ORDER_PRODUCT = EXPORT_FILE_NAME_INITIALS + KBN_ADVC_TOTAL_KBN_ORDER + "Product";
	protected const string EXPORT_FILE_NAME_ORDER_OWNER = EXPORT_FILE_NAME_INITIALS + KBN_ADVC_TOTAL_KBN_ORDER + "Owner";
	protected const string EXPORT_FILE_NAME_PV = EXPORT_FILE_NAME_INITIALS + KBN_ADVC_TOTAL_KBN_PV;
	protected const string EXPORT_FILE_NAME_CVR = EXPORT_FILE_NAME_INITIALS + KBN_ADVC_TOTAL_KBN_CVR;

	/// <summary>フィールド名</summary>
	protected const string FIELD_ADVC_REGIST_COUNT = "regist_count";
	protected const string FIELD_ADVC_WITHDRAWAL_COUNT = "withdrawal_count";
	protected const string FIELD_ADVC_USER_COUNT = "user_count";
	protected const string FIELD_ADVC_ORDER_COUNT = "order_count";
	protected const string FIELD_ADVC_ORDER_PRICE = "order_price";
	protected const string FIELD_ADVC_CLICK_COUNT = "click_count";
	protected const string FIELD_ADVC_CLICK_UNIQUE_COUNT = "click_unique_count";
	protected const string FIELD_ADVC_ORDER_PRICE_AVG = "order_price_avg";
	protected const string FIELD_ADVC_TERM1_FROM = "term1_from";
	protected const string FIELD_ADVC_TERM1_TO = "term1_to";

	protected const string KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER = "0"; // 注文時の広告コード
	protected const string KBN_ADVC_ADVERTISEMENT_CODE_TARGET_FIRST_ORDER = "1"; // 初回の広告コード
	private const string KEY_ROW_COUNT = "row_count";
	#endregion

	/// <summary>
	/// 割合表示
	/// </summary>
	/// <param name="current">比較データ</param>
	/// <param name="target">比較対象データ</param>
	/// <returns>比較データ÷比較対象データの割合</returns>
	protected string DispRate(object current, object target)
	{
		string rate = AnalysisUtility.GetRateString(current, target, 1);

		return (rate ?? "－");
	}

	/// <summary>
	/// 日付型年数チェック(西暦の1753年未満をチェック)
	/// </summary>
	/// <param name="value">値</param>
	/// <returns>エラー判定</returns>
	/// <remarks>.NetとSQLServerのバージョンによってはDateTimeの有効範囲が異なるため、チェックを行う</remarks>
	protected bool CheckDateYear(object value)
	{
		DateTime date;
		if (DateTime.TryParse(
			StringUtility.ToEmpty(value),
			string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
				? CultureInfo.CurrentCulture
				: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE),
			DateTimeStyles.None,
			out date))
		{
			return (date.Year >= 1753);
		}
		return false;
	}

	/// <summary>
	/// Get data export
	/// </summary>
	/// <param name="data"> Data input</param>
	/// <param name="totalKbn"> Total kbn</param>
	/// <param name="totalParams"> Parameters of total line</param>
	/// <param name="titleRowData"> List row data of title </param>
	/// <returns> Data export</returns>
	protected string GetDataExport(DataView data, string totalKbn, List<string> totalParams, List<List<string>> titleRowData)
	{
		StringBuilder records = new StringBuilder();

		foreach (List<string> titleRow in titleRowData)
		{
			records.Append(CreateRecordCsv(titleRow));
		}

		records.Append("\r\n");
		records.Append(CreateRecordCsv(GetHeaderParam(totalKbn)));

		if ((totalParams != null) && (totalParams.Count > 0))
		{
			records.Append(CreateRecordCsv(totalParams));
		}

		if (data != null)
		{
			foreach (DataRowView dataRow in data)
			{
				records.Append(GetDataRow(dataRow, totalKbn));
			}
		}

		return records.ToString();
	}

	/// <summary>
	/// Get string term
	/// </summary>
	/// <param name="term"> Term</param>
	/// <returns> String term</returns>
	protected string GetStringDate(string term)
	{
		if ((term == "")
			|| (Validator.IsDate(term, Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false)
			|| (CheckDateYear(term) == false))
		{
			return "　";
		}
		var date = DateTimeUtility.ToStringForManager(term, DateTimeUtility.FormatType.LongDate2Letter);
		return date;
	}

	/// <summary>
	/// Get data of row
	/// </summary>
	/// <param name="dataRow"> Data row</param>
	/// <param name="totalKbn"> Total kbn</param>
	/// <returns> String data row</returns>
	protected string GetDataRow(DataRowView dataRow, string totalKbn)
	{
		List<string> dataParams = new List<string>();

		var mediaTypeName = dataRow.DataView.Table.Columns.Contains(Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME)
							? new List<string> { StringUtility.ToEmpty(dataRow[Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME]) }
							: new List<string>();
		switch (totalKbn)
		{
			case KBN_ADVC_TOTAL_KBN_USER:	// ユーザ集計
				dataParams.AddRange(mediaTypeName);
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE]));
				dataParams.Add(StringUtility.ToEmpty(StringUtility.ToEmpty(dataRow[Constants.FIELD_ADVCODE_MEDIA_NAME])));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_REGIST_COUNT]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_WITHDRAWAL_COUNT]));
				break;

			case KBN_ADVC_TOTAL_KBN_ORDER:	// 売上表示(出荷基準)
				dataParams.AddRange(mediaTypeName);
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE]));
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ADVCODE_MEDIA_NAME]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_USER_COUNT]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_ORDER_COUNT]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_ORDER_PRICE]));
				break;

			case KBN_ADVC_TOTAL_KBN_PV:	// PV集計
				dataParams.AddRange(mediaTypeName);
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE]));
				dataParams.Add(StringUtility.ToEmpty(StringUtility.ToEmpty(dataRow[Constants.FIELD_ADVCODE_MEDIA_NAME])));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_CLICK_COUNT]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_CLICK_UNIQUE_COUNT]));
				break;

			case KBN_ADVC_TOTAL_KBN_CVR:	// コンバージョンレート集計
				dataParams.AddRange(mediaTypeName);
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE]));
				dataParams.Add(StringUtility.ToEmpty(StringUtility.ToEmpty(dataRow[Constants.FIELD_ADVCODE_MEDIA_NAME])));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_CLICK_UNIQUE_COUNT]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_REGIST_COUNT]));
				dataParams.Add(DispRate(dataRow[FIELD_ADVC_REGIST_COUNT], dataRow[FIELD_ADVC_CLICK_UNIQUE_COUNT]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_WITHDRAWAL_COUNT]));
				dataParams.Add(DispRate(dataRow[FIELD_ADVC_WITHDRAWAL_COUNT], dataRow[FIELD_ADVC_CLICK_UNIQUE_COUNT]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_USER_COUNT]));
				dataParams.Add(DispRate(dataRow[FIELD_ADVC_USER_COUNT], dataRow[FIELD_ADVC_CLICK_UNIQUE_COUNT]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_ORDER_PRICE]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[FIELD_ADVC_ORDER_PRICE_AVG]));
				break;

			case Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY:	// 商品別売れ行きランキング
				dataParams.Add(StringUtility.ToEmpty(dataRow["rank"]));
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDERITEM_PRODUCT_ID]));
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDERITEM_VARIATION_ID]));
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDERITEM_PRODUCT_NAME]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]));
				dataParams.Add(DispRate(dataRow[Constants.FIELD_ORDERITEM_ITEM_QUANTITY], dataRow[Constants.FIELD_ORDERITEM_ITEM_QUANTITY + "_total"]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[Constants.FIELD_ORDERITEM_ITEM_PRICE]));
				dataParams.Add(DispRate(dataRow[Constants.FIELD_ORDERITEM_ITEM_PRICE], dataRow[Constants.FIELD_ORDERITEM_ITEM_PRICE + "_total"]));
				break;

			case Constants.KBN_ADVCODE_DISP_ORDER:	// 購入者詳細
				dataParams.Add(StringUtility.ToEmpty(dataRow["row_num"]));
				dataParams.Add(this.AdvertisementCodeTarget != KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER
									? StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDER_ADVCODE_FIRST])
									: StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDER_ADVCODE_NEW]));
				dataParams.Add(
					DateTimeUtility.ToStringForManager(
						dataRow[Constants.FIELD_ORDER_ORDER_DATE],
						DateTimeUtility.FormatType.ShortDate2Letter));
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDERITEM_PRODUCT_ID]));
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDERITEM_VARIATION_ID]));
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDERITEM_PRODUCT_NAME]));
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDER_USER_ID]));
				dataParams.Add(StringUtility.ToEmpty(dataRow[Constants.FIELD_ORDEROWNER_OWNER_NAME]));
				dataParams.Add(StringUtility.ToNumeric(dataRow[Constants.FIELD_ORDERITEM_ITEM_PRICE]));
				break;
		}

		return CreateRecordCsv(dataParams);
	}

	/// <summary>
	/// Get header parameters
	/// </summary>
	/// <param name="totalKbn"> Total kbn</param>
	/// <returns> Header parameters</returns>
	protected List<string> GetHeaderParam(string totalKbn)
	{
		List<string> headerParams = null;
		var headerMediaTypeName = "広告媒体区分";

		switch (totalKbn)
		{
			case KBN_ADVC_TOTAL_KBN_USER:	// ユーザ集計
				headerParams = new List<string> { headerMediaTypeName, "広告コード", "媒体名", "登録ユーザ数", "退会ユーザ数" };
				break;

			case KBN_ADVC_TOTAL_KBN_ORDER:	// 売上表示(出荷基準)
				headerParams = new List<string> { headerMediaTypeName, "広告コード", "媒体名", "注文ユーザ数", "注文件数", "注文金額" };
				break;

			case KBN_ADVC_TOTAL_KBN_PV:	// PV集計
				headerParams = new List<string> { headerMediaTypeName, "広告コード", "媒体名", "クリック流入数", "クリック流入数（ユニークユーザ数）" };
				break;

			case KBN_ADVC_TOTAL_KBN_CVR:	// コンバージョンレート集計
				headerParams = new List<string>
					{
						headerMediaTypeName,
						"広告コード",
						"媒体名",
						"クリック流入数(ユニーク)",
						"登録ユーザ数(人数)",
						"登録ユーザ数(CVR)",
						"退会ユーザ数(人数)",
						"退会ユーザ数(CVR)",
						"注文ユーザ数(人数)",
						"注文ユーザ数(CVR)",
						"注文金額",
						"顧客単価 "
					};
				break;

			case Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY:	// 商品別売れ行きランキング
				return new List<string> { "Rank", "商品ID", "バリエーションID", "注文商品名", "注文数(個)", "構成比(%)", "注文商品金額", "構成比(%)" };

			case Constants.KBN_ADVCODE_DISP_ORDER:	// 購入者詳細
				return new List<string> {"項番", "広告コード", "注文日", "商品ID", "バリエーションID", " 注文商品名", "ユーザーID", "注文者", "注文商品金額"};
		}

		return headerParams;
	}

	/// <summary>
	/// Export file csv
	/// </summary>
	/// <param name="fileName"> File name</param>
	/// <param name="content"> Content</param>
	protected void ExportFileCsv(string fileName, string content)
	{
		OutPutFileCsv(fileName, content);
	}

	/// <summary>
	/// Get list parameter title
	/// </summary>
	/// <param name="totalKbn"> Total kbn</param>
	/// <returns> List parameter title</returns>
	protected List<List<string>> GetTitleRowsData(string totalKbn)
	{
		List<List<string>> titleRowsData = new List<List<string>>();
		List<string> row1 = new List<string>();
		List<string> row2 = new List<string>();
		List<string> row3 = new List<string>();

		string term1 = GetStringDate(this.Term1From) + "～" + GetStringDate(this.Term1To);
		string term2 = GetStringDate(this.Term2From) + "～" + GetStringDate(this.Term2To);

		row1.Add("広告コードレポート 集計区分：" + this.TotalKbn + "　" + this.Career);
		titleRowsData.Add(row1);

		switch (totalKbn)
		{
			case KBN_ADVC_TOTAL_KBN_USER:
				row2.Add("期間指定1：" + term1 + "　期間指定2：" + term2);
				break;

			case KBN_ADVC_TOTAL_KBN_ORDER:
			case Constants.KBN_ADVCODE_DISP_ORDER:
				row2.Add(("期間指定1：" + term1) + "　対象広告コード：" + ((this.AdvertisementCodeTarget == KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER) ? "注文時の広告コード" : "初回の広告コード"));
				break;

			case KBN_ADVC_TOTAL_KBN_PV:
				row2.Add("期間指定1：" + term1);
				break;

			case KBN_ADVC_TOTAL_KBN_CVR:
				row2.Add("期間指定1：" + term1 + "　集計基準：" + this.AdvertisementConversionRateKbn);
				break;

			case Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY:
				row2.Add(("期間指定1：" + term1) + "　対象広告コード：" + ((this.AdvertisementCodeTarget == KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER) ? "注文時の広告コード" : "初回の広告コード"));
				row3.Add("広告コード：" + this.AdvCode + "　媒体名：" + this.MediaName);
				break;
		}

		if (row2.Count > 0)
		{
			titleRowsData.Add(row2);
		}

		if (row3.Count > 0)
		{
			titleRowsData.Add(row3);
		}

		return titleRowsData;
	}

	/// <summary>
	/// Term1 from
	/// </summary>
	protected string Term1From { get; set; }
	/// <summary>
	/// Term1 to
	/// </summary>
	protected string Term1To { get; set; }
	/// <summary>
	/// Term2 from
	/// </summary>
	protected string Term2From { get; set; }
	/// <summary>
	/// Term2 to
	/// </summary>
	protected string Term2To { get; set; }
	/// <summary>
	/// Total Kbn
	/// </summary>
	protected string TotalKbn { get; set; }
	/// <summary>
	/// Carreer
	/// </summary>
	protected string Career { get; set; }
	/// <summary>
	/// Advertiement code target
	/// </summary>
	protected string AdvertisementCodeTarget { get; set; }
	/// <summary>
	/// Advertisement conversion rate kbn
	/// </summary>
	protected string AdvertisementConversionRateKbn { get; set; }
	/// <summary>
	/// Advertiement code
	/// </summary>
	protected string AdvCode { get; set; }
	/// <summary>
	/// Media name
	/// </summary>
	protected string MediaName { get; set; }
	/// <summary>
	/// Total count data
	/// </summary>
	protected int TotalCount
	{
		get { return (int)ViewState[KEY_ROW_COUNT]; }
		set { ViewState[KEY_ROW_COUNT] = value; }
	}
}
