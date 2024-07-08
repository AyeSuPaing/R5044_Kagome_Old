/*
=========================================================================================================
  Module      : 発注入庫情報一覧ページ処理(StockOrderList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Util;

public partial class Form_StockOrder_StockOrderList : BasePage
{
	//------------------------------------------------------
	// パラメタキー
	//------------------------------------------------------
	// 発注ID
	const string REQUEST_KEY_STOCK_ORDER_ID = "osoid";
	// 発注連携ID
	const string REQUEST_KEY_RELATION_ID = "rtnid";
	// 商品ID
	const string REQUEST_KEY_PRODUCT_ID = "prid";
	// 発注ステータス
	const string REQUEST_KEY_ORDER_STATUS = "odrsts";
	// 入庫ステータス
	const string REQUEST_KEY_DELIVERY_STATUS = "dlvsts";
	// 発注日(FROM)
	const string REQUEST_KEY_ORDER_DATE_FROM = "odf";
	const string REQUEST_KEY_ORDER_TIME_FROM = "otf";
	// 発注日(TO)
	const string REQUEST_KEY_ORDER_DATE_TO = "odt";
	const string REQUEST_KEY_ORDER_TIME_TO = "ott";
	// 入庫日(FROM)
	const string REQUEST_KEY_DELIVERY_DATE_FROM = "ddf";
	const string REQUEST_KEY_DELIVERY_TIME_FROM = "dtf";
	// 入庫日(TO)
	const string REQUEST_KEY_DELIVERY_DATE_TO = "ddt";
	const string REQUEST_KEY_DELIVERY_TIME_TO = "dtt";
	// ソート区分
	const string REQUEST_KEY_SORT_KBN = "skbn";

	const string KBN_SORT_STOCKORDERID_ASC = "01";
	const string KBN_SORT_STOCKORDERID_DESC = "02";
	const string KBN_SORT_ORDERDATE_ASC = "03";
	const string KBN_SORT_ORDERDATE_DESC = "04";
	const string KBN_SORT_DELIVERYDATE_ASC = "05";
	const string KBN_SORT_DELIVERYDATE_DESC = "06";
	const string KBN_SORT_DEFAULT = KBN_SORT_STOCKORDERID_DESC;

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
			// 画面制御
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable htRequestParam = CreateRequestParam();

			//------------------------------------------------------
			// 情報一覧、ページャ表示
			//------------------------------------------------------
			DisplayList(htRequestParam);
		}
	}

	/// <summary>
	/// SQLパラメタハッシュテーブル作成
	/// </summary>
	/// <param name="htRequest"></param>
	/// <returns></returns>
	private Hashtable CreateSqlParam(Hashtable htRequest)
	{
		Hashtable htResult = new Hashtable();
		htResult.Add(Constants.FIELD_STOCKORDER_SHOP_ID, this.LoginOperatorShopId);
		htResult.Add(Constants.FIELD_STOCKORDER_STOCK_ORDER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htRequest[REQUEST_KEY_STOCK_ORDER_ID]));
		htResult.Add(Constants.FIELD_STOCKORDER_RELATION_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htRequest[REQUEST_KEY_RELATION_ID]));
		htResult.Add(Constants.FIELD_STOCKORDER_ORDER_STATUS, htRequest[REQUEST_KEY_ORDER_STATUS]);
		htResult.Add(Constants.FIELD_STOCKORDER_DELIVERY_STATUS, htRequest[REQUEST_KEY_DELIVERY_STATUS]);

		// 発注日、入庫日初期化
		htResult.Add("order_date_from", DBNull.Value);
		htResult.Add("order_date_to", DBNull.Value);
		htResult.Add("delivery_date_from", DBNull.Value);
		htResult.Add("delivery_date_to", DBNull.Value);
		// 発注日
		var orderDateFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(htRequest[REQUEST_KEY_ORDER_DATE_FROM]),
			StringUtility.ToEmpty(htRequest[REQUEST_KEY_ORDER_TIME_FROM]));
		if (Validator.IsDate(orderDateFrom))
		{
			htResult["order_date_from"] = DateTime.Parse(orderDateFrom);
		}
		var orderDateTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(htRequest[REQUEST_KEY_ORDER_DATE_TO]),
			StringUtility.ToEmpty(htRequest[REQUEST_KEY_ORDER_TIME_TO]));
		if (Validator.IsDate(orderDateTo))
		{
			htResult["order_date_to"] =
				DateTime.Parse(orderDateTo).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
		}
		// 入庫日
		var deliveryDateFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(htRequest[REQUEST_KEY_DELIVERY_DATE_FROM]),
			StringUtility.ToEmpty(htRequest[REQUEST_KEY_DELIVERY_TIME_FROM]));
		if (Validator.IsDate(deliveryDateFrom))
		{
			htResult[string.Format("{0}_from", Constants.FIELD_STOCKORDER_DELIVERY_DATE)] = DateTime.Parse(deliveryDateFrom);
		}
		var deliveryDateTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(htRequest[REQUEST_KEY_DELIVERY_DATE_TO]),
			StringUtility.ToEmpty(htRequest[REQUEST_KEY_DELIVERY_TIME_TO]));
		if (Validator.IsDate(deliveryDateTo))
		{
			htResult[string.Format("{0}_to", Constants.FIELD_STOCKORDER_DELIVERY_DATE)] =
				DateTime.Parse(deliveryDateTo).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
		}
		htResult.Add(Constants.FIELD_STOCKORDERITEM_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htRequest[REQUEST_KEY_PRODUCT_ID]));
		htResult.Add("sort_kbn", htRequest[REQUEST_KEY_SORT_KBN]);

		int iCurrentPageNumber = (int)htRequest[Constants.REQUEST_KEY_PAGE_NO];
		htResult.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1);
		htResult.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber);

		return htResult;
	}

	/// <summary>
	/// 一覧情報表示
	/// </summary>
	/// <param name="htRequestParam">リクエストパラメタ</param>
	private void DisplayList(Hashtable htRequestParam)
	{
		//------------------------------------------------------
		// 検索情報変換
		//------------------------------------------------------
		Hashtable htInput = CreateSqlParam(htRequestParam);

		//------------------------------------------------------
		// 一覧情報取得
		//------------------------------------------------------
		DataView dvStockOrderList = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("StockOrder", "GetStockOrderList"))
		{
			// SQL発行
			dvStockOrderList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		//------------------------------------------------------
		// 表示制御
		//------------------------------------------------------
		int iTotalCounts = 0;	// ページング可能総商品数

		// 注文データが存在する場合
		if (dvStockOrderList.Count != 0)
		{
			iTotalCounts = int.Parse(dvStockOrderList[0].Row["row_count"].ToString());

			// エラー非表示
			trListError.Visible = false;
		}
		else
		{
			iTotalCounts = 0;

			// エラー表示
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// データソースセット
		rList.DataSource = dvStockOrderList;
		rList.DataBind();

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string strNextUrl = CreateListUrl(htRequestParam);
		lbPager1.Text = WebPager.CreateDefaultListPager(iTotalCounts, (int)htRequestParam[Constants.REQUEST_KEY_PAGE_NO], strNextUrl);
	}


	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 発注ステータス
		ddlOrderStatus.Items.Add("");
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_STOCKORDER, Constants.FIELD_STOCKORDER_ORDER_STATUS))
		{
			ddlOrderStatus.Items.Add(li);
		}

		// 入庫ステータス
		ddlDeliveryStatus.Items.Add("");
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_STOCKORDER, Constants.FIELD_STOCKORDER_DELIVERY_STATUS))
		{
			ddlDeliveryStatus.Items.Add(li);
		}
	
		// ソート区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_STOCKORDER, REQUEST_KEY_SORT_KBN))
		{
			ddlSortKbn.Items.Add(li);
		}
		foreach (ListItem li in ddlSortKbn.Items)
		{
			li.Selected = (li.Value == KBN_SORT_DEFAULT);
		}
	}


	/// <summary>
	///　パラメタ取得
	/// </summary>
	/// <param name="Request">注文一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable CreateRequestParam()
	{
		Hashtable htResult = new Hashtable();

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		bool blParamError = false;

		// 発注ID
		string strStockOrderId = StringUtility.ToEmpty(Request[REQUEST_KEY_STOCK_ORDER_ID]);
		htResult.Add(REQUEST_KEY_STOCK_ORDER_ID, strStockOrderId);
		tbStockOrderId.Text = strStockOrderId;

		// 発注連携ID
		string strRelationId = StringUtility.ToEmpty(Request[REQUEST_KEY_RELATION_ID]);
		htResult.Add(REQUEST_KEY_RELATION_ID, strRelationId);
		tbRelationId.Text = strRelationId;

		// 商品ID
		string strProductId = StringUtility.ToEmpty(Request[REQUEST_KEY_PRODUCT_ID]);
		htResult.Add(REQUEST_KEY_PRODUCT_ID, strProductId);
		tbProductId.Text = strProductId;

		// 発注ステータス
		string strOrderStaus = StringUtility.ToEmpty(Request[REQUEST_KEY_ORDER_STATUS]);
		try
		{
			htResult.Add(REQUEST_KEY_ORDER_STATUS, strOrderStaus);
			ddlOrderStatus.SelectedValue = strOrderStaus;
		}
		catch
		{
			blParamError = true;
		}

		// 入庫ステータス
		string strDeliveryStaus = StringUtility.ToEmpty(Request[REQUEST_KEY_DELIVERY_STATUS]);
		try
		{
			htResult.Add(REQUEST_KEY_DELIVERY_STATUS, strDeliveryStaus);
			ddlDeliveryStatus.SelectedValue = strDeliveryStaus;
		}
		catch
		{
			blParamError = true;
		}

		// Set request key order date from date to
		htResult.Add(REQUEST_KEY_ORDER_DATE_FROM, StringUtility.ToEmpty(Request[REQUEST_KEY_ORDER_DATE_FROM]));
		htResult.Add(REQUEST_KEY_ORDER_DATE_TO, StringUtility.ToEmpty(Request[REQUEST_KEY_ORDER_DATE_TO]));

		// Set request key order time from time to
		htResult.Add(REQUEST_KEY_ORDER_TIME_FROM, StringUtility.ToEmpty(Request[REQUEST_KEY_ORDER_TIME_FROM]));
		htResult.Add(REQUEST_KEY_ORDER_TIME_TO, StringUtility.ToEmpty(Request[REQUEST_KEY_ORDER_TIME_TO]));

		// 発注日
		var orderDatePeriodFrom = string.Format("{0}{1}",
			StringUtility.ToEmpty(Request[REQUEST_KEY_ORDER_DATE_FROM])
				.Replace("/", string.Empty),
			StringUtility.ToEmpty(Request[REQUEST_KEY_ORDER_TIME_FROM])
				.Replace(":", string.Empty));

		var orderDatePeriodTo = string.Format("{0}{1}",
			StringUtility.ToEmpty(Request[REQUEST_KEY_ORDER_DATE_TO])
				.Replace("/", string.Empty),
			StringUtility.ToEmpty(Request[REQUEST_KEY_ORDER_TIME_TO])
				.Replace(":", string.Empty));

		ucOrderDatePeriod.SetPeriodDate(orderDatePeriodFrom, orderDatePeriodTo);

		// Set request delivery date from date to
		htResult.Add(REQUEST_KEY_DELIVERY_DATE_FROM, StringUtility.ToEmpty(Request[REQUEST_KEY_DELIVERY_DATE_FROM]));
		htResult.Add(REQUEST_KEY_DELIVERY_DATE_TO, StringUtility.ToEmpty(Request[REQUEST_KEY_DELIVERY_DATE_TO]));

		// Set request delivery time from time to
		htResult.Add(REQUEST_KEY_DELIVERY_TIME_FROM, StringUtility.ToEmpty(Request[REQUEST_KEY_DELIVERY_TIME_FROM]));
		htResult.Add(REQUEST_KEY_DELIVERY_TIME_TO, StringUtility.ToEmpty(Request[REQUEST_KEY_DELIVERY_TIME_TO]));

		// 入庫日・年
		var deliveryDatePeriodFrom = string.Format("{0}{1}",
			StringUtility.ToEmpty(Request[REQUEST_KEY_DELIVERY_DATE_FROM])
				.Replace("/", string.Empty),
			StringUtility.ToEmpty(Request[REQUEST_KEY_DELIVERY_TIME_FROM])
				.Replace(":", string.Empty));

		var deliveryDatePeriodTo = string.Format("{0}{1}",
			StringUtility.ToEmpty(Request[REQUEST_KEY_DELIVERY_DATE_TO])
				.Replace("/", string.Empty),
			StringUtility.ToEmpty(Request[REQUEST_KEY_DELIVERY_TIME_TO])
				.Replace(":", string.Empty));
		ucDeliveryDatePeriod.SetPeriodDate(deliveryDatePeriodFrom, deliveryDatePeriodTo);
		// ソート区分
		string strSortKbn = StringUtility.ToEmpty(Request[REQUEST_KEY_SORT_KBN]);
		switch (strSortKbn)
		{
			case KBN_SORT_STOCKORDERID_ASC:
			case KBN_SORT_STOCKORDERID_DESC:
			case KBN_SORT_ORDERDATE_ASC:
			case KBN_SORT_ORDERDATE_DESC:
			case KBN_SORT_DELIVERYDATE_ASC:
			case KBN_SORT_DELIVERYDATE_DESC:
				break;

			default:
				strSortKbn = KBN_SORT_DEFAULT;
				break;
		}
		htResult.Add(REQUEST_KEY_SORT_KBN, strSortKbn);
		foreach (ListItem li in ddlSortKbn.Items)
		{
			li.Selected = (li.Value == strSortKbn);
		}

		// ページ番号（ページャ動作時のみもちまわる）
		int iCurrentPageNumber = 1;
		try
		{
			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) == "")
			{
			}
			else
			{
				iCurrentPageNumber = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO]);
			}

			htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);
		}
		catch
		{
			blParamError = true;
		}

		//------------------------------------------------------
		// 不正パラメータが存在した場合エラー
		//------------------------------------------------------
		if (blParamError)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return htResult;
	}

	/// <summary>
	/// データバインド用注文情報詳細URL作成
	/// </summary>
	/// <param name="strStockOrderId">発注ID</param>
	/// <returns>注文情報詳細URL</returns>
	protected string CreateDetailUrl(string strStockOrderId)
	{
		string strResult = "";

		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_STOCKDELIVERY_REGIST;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_STOCKORDER_STOCK_ORDER_ID + "=" + HttpUtility.UrlEncode(strStockOrderId);

		return strResult;
	}

	/// <summary>
	/// 一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>一覧遷移URL</returns>
	protected string CreateListUrl(Hashtable htInput, int iPageNumber)
	{
		return CreateListUrl(htInput) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();
	}
	/// <summary>
	/// 一覧遷移URL作成
	/// </summary>
	/// <param name="htInput">検索情報</param>
	/// <returns>一覧遷移URL</returns>
	protected string CreateListUrl(Hashtable htInput)
	{
		StringBuilder sbResult = new StringBuilder();

		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_STOCKORDER_LIST);
		sbResult.Append("?");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_STOCK_ORDER_ID));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_RELATION_ID));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_PRODUCT_ID));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_ORDER_STATUS));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_DELIVERY_STATUS));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_DELIVERY_DATE_FROM));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_DELIVERY_TIME_FROM));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_DELIVERY_DATE_TO));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_DELIVERY_TIME_TO));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_ORDER_DATE_FROM));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_ORDER_TIME_FROM));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_ORDER_DATE_TO));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_ORDER_TIME_TO));
		sbResult.Append("&");
		sbResult.Append(CreateUrlParam(htInput, REQUEST_KEY_SORT_KBN));

		return sbResult.ToString();
	}

	/// <summary>
	/// URLパラメタ作成
	/// </summary>
	/// <param name="htParams"></param>
	/// <param name="strParamName"></param>
	/// <returns></returns>
	private string CreateUrlParam(Hashtable htParams, string strParamName)
	{
		return new StringBuilder(strParamName).Append("=").Append(HttpUtility.UrlEncode((string)htParams[strParamName])).ToString();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		Hashtable htInput = new Hashtable();
		htInput.Add(REQUEST_KEY_STOCK_ORDER_ID, tbStockOrderId.Text.Trim());
		htInput.Add(REQUEST_KEY_RELATION_ID, tbRelationId.Text.Trim());
		htInput.Add(REQUEST_KEY_PRODUCT_ID, tbProductId.Text.Trim());
		htInput.Add(REQUEST_KEY_ORDER_STATUS, ddlOrderStatus.SelectedValue);
		htInput.Add(REQUEST_KEY_DELIVERY_STATUS, ddlDeliveryStatus.SelectedValue);
		htInput.Add(REQUEST_KEY_ORDER_DATE_FROM, ucOrderDatePeriod.HfStartDate.Value);
		htInput.Add(REQUEST_KEY_ORDER_TIME_FROM, ucOrderDatePeriod.HfStartTime.Value);
		htInput.Add(REQUEST_KEY_ORDER_DATE_TO, ucOrderDatePeriod.HfEndDate.Value);
		htInput.Add(REQUEST_KEY_ORDER_TIME_TO, ucOrderDatePeriod.HfEndTime.Value);
		htInput.Add(REQUEST_KEY_DELIVERY_DATE_FROM, ucDeliveryDatePeriod.HfStartDate.Value);
		htInput.Add(REQUEST_KEY_DELIVERY_TIME_FROM, ucDeliveryDatePeriod.HfStartTime.Value);
		htInput.Add(REQUEST_KEY_DELIVERY_DATE_TO, ucDeliveryDatePeriod.HfEndDate.Value);
		htInput.Add(REQUEST_KEY_DELIVERY_TIME_TO, ucDeliveryDatePeriod.HfEndTime.Value);
		htInput.Add(REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);

		string strUrl = CreateListUrl(htInput, 1);

		// リダイレクト
		Response.Redirect(strUrl);
	}

	/// <summary>
	/// 新規発注ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOrderTop_Click(object sender, EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_STOCKORDER_REGIST);
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return CreateSqlParam(CreateRequestParam());
	}
}