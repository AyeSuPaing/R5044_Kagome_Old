/*
=========================================================================================================
  Module      : 入荷通知メール情報一覧ページ処理(UserProductArrivalMailList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
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
using w2.Domain.MailTemplate;
using w2.App.Common.Order;
using w2.App.Common.Stock;
using w2.App.Common.Util;
using w2.App.Common.Mail;

public partial class Form_UserProductArrivalMail_UserProductArrivalMailList : ProductPage
{
	// 入荷通知メール区分チェックボックスON/OFF
	const string FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED = "1";		// ON
	const string FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_UNCHECKED = "0";	// OFF

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
			Hashtable htParam = GetParameters();

			if (this.IsNotSearchDefault) return;

			// 検索条件をセッションに保存
			Session[Constants.SESSIONPARAM_KEY_PRODUCT_SEARCH_INFO] = htParam;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			Hashtable htSqlParam = GetSearchSqlInfo(htParam);

			//------------------------------------------------------
			// 入荷通知メール情報一覧
			//------------------------------------------------------
			int iTotalCounts = 0;	// ページング可能総商品数
			// 入荷通知メール情報取得
			DataView dvUserProductArrivalMail = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("UserProductArrivalMail", "GetUserProductArrivalMaliList"))
			{
				htSqlParam.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1);
				htSqlParam.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo);

				dvUserProductArrivalMail = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
			}

			if (dvUserProductArrivalMail.Count != 0)
			{
				iTotalCounts = int.Parse(dvUserProductArrivalMail[0].Row["row_count"].ToString());
				// エラー非表示制御
				trListError.Visible = false;
			}
			else
			{
				iTotalCounts = 0;
				// エラー表示制御
				trListError.Visible = true;
				tdErrorMessage.InnerHtml =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

			}
			// データソースセット
			rList.DataSource = dvUserProductArrivalMail;

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			string strNextUrl = CreateUserProductArrivalMailListUrl(htParam, false);
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalCounts, this.CurrentPageNo, strNextUrl);

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		//-------------------------------------
		// メールテンプレート
		//-------------------------------------
		var notSendText = string.Format("{0}（{1}）",
			Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
				"@@DispText.mail_template.not_send@@",
				Constants.GLOBAL_OPTION_ENABLE
					? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
					: ""),
			Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
				"@@DispText.mail_template.remove_from_list@@",
				Constants.GLOBAL_OPTION_ENABLE
					? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
					: ""));
		ddlMailTemplateArrival.Items.Add(new ListItem(notSendText, "NOTSEND"));
		ddlMailTemplateRelease.Items.Add(new ListItem(notSendText, "NOTSEND"));
		ddlMailTemplateResale.Items.Add(new ListItem(notSendText, "NOTSEND"));

		// メールテンプレート一覧取得
		var mailTemplateList = GetMailTemplateUtility.GetMailTemplateForProductArrival(this.LoginOperatorShopId);
		if (mailTemplateList.Length != 0)
		{
			ddlMailTemplateArrival.Items.AddRange(
				mailTemplateList.Select(mail => new ListItem(mail.MailName, mail.MailId)).ToArray());
			ddlMailTemplateRelease.Items.AddRange(
				mailTemplateList.Select(mail => new ListItem(mail.MailName, mail.MailId)).ToArray());
			ddlMailTemplateResale.Items.AddRange(
				mailTemplateList.Select(mail => new ListItem(mail.MailName, mail.MailId)).ToArray());

			// 初期値設定
			ddlMailTemplateArrival.SelectedValue = Constants.DEFAULT_MAILTEMPLATE_ARRIVAL;
			ddlMailTemplateRelease.SelectedValue = Constants.DEFAULT_MAILTEMPLATE_RELEASE;
			ddlMailTemplateResale.SelectedValue = Constants.DEFAULT_MAILTEMPLATE_RESALE;
		}

		//-------------------------------------
		// チェックボックス
		//-------------------------------------
		// 初期値設定
		cbSendArrivalMail.Checked
			= cbSendReleaseMail.Checked
			= cbSendResaleMail.Checked
			= true;

		trListError.Visible = true;
		tdErrorMessage.InnerHtml =
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT);

		//-------------------------------------
		// 処理中メッセージ表示制御
		//-------------------------------------
		divSending.Visible = (Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_MAIL_SEND] != null);
	}

	/// <summary>
	/// 入荷通知メール情報一覧パラメタ取得
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters()
	{
		Hashtable htResult = new Hashtable();
		try
		{
			// 商品ID
			htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_ID]));
			tbProductId.Text = (string)htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_ID];

			// 商品名
			htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_NAME]));
			tbProductName.Text = (string)htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_NAME];

			// 在庫数
			htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_STOCK_COUNT, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_STOCK_COUNT]));
			tbProductStock.Text = (string)htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_STOCK_COUNT];

			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM]))
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM], string.Empty));
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM], string.Empty));
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM,
					Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM]);
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM,
					Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM]);

				var productSellFromDateStart = string.Format("{0} {1}",
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM]),
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM]));

				if (string.IsNullOrEmpty(productSellFromDateStart) == false)
				{
					ucProductSellFromDate.SetStartDate(DateTime.Parse(productSellFromDateStart));
				}
			}

			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO]))
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO], string.Empty));
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO], string.Empty));
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO,
					Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO]);
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO,
					Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO]);

				var productSellFromDateEnd = string.Format("{0} {1}",
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO]),
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO]));

				if (string.IsNullOrEmpty(productSellFromDateEnd) == false)
				{
					ucProductSellFromDate.SetEndDate(DateTime.Parse(productSellFromDateEnd));
				}
			}

			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM]))
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM], string.Empty));
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM], string.Empty));
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM,
					Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM]);
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM,
					Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM]);

				var productSellToDateStart = string.Format("{0} {1}",
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM]),
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM]));

				if (string.IsNullOrEmpty(productSellToDateStart) == false)
				{
					ucProductSellToDate.SetStartDate(DateTime.Parse(productSellToDateStart));
				}
			}

			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO]))
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO], string.Empty));
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO], string.Empty));
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO,
					Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO]);
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO,
					Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO]);

				var productSellToDateEnd = string.Format("{0} {1}",
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO]),
					StringUtility.ToEmpty(htResult[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO]));

				if (string.IsNullOrEmpty(productSellToDateEnd) == false)
				{
					ucProductSellToDate.SetEndDate(DateTime.Parse(productSellToDateEnd));
				}
			}

			// 販売期間
			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD]))
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD], string.Empty));
			}
			else
			{
				DateTime productSalesPeriodDate;
				if (DateTime.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD]), out productSalesPeriodDate))
				{
					var dateFromYear = productSalesPeriodDate.Year;
					var dateFromMonth = productSalesPeriodDate.Month;
					var dateFromDay = productSalesPeriodDate.Day;
					// 末日補正処理
					if (DateTimeUtility.IsLastDayOfMonth(dateFromYear, dateFromMonth, dateFromDay))
					{
						var lastDay = DateTimeUtility.GetLastDayOfMonth(dateFromYear, dateFromMonth).ToString();
						var productSalesPeriodStart = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
							dateFromYear,
							dateFromMonth,
							lastDay,
							productSalesPeriodDate.Hour,
							productSalesPeriodDate.Minute,
							productSalesPeriodDate.Second);
						productSalesPeriodDate = DateTime.Parse(productSalesPeriodStart);
					}

					htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD,
						StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD], productSalesPeriodDate.ToString()));
					ucProductSalesPeriod.SetStartDate(productSalesPeriodDate);
				}
			}

			// 表示期間
			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD]))
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD,
					StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD], string.Empty));
			}
			else
			{
				DateTime productDisplayPeriodDate;
				if (DateTime.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD]), out productDisplayPeriodDate))
				{
					var dateFromYear = productDisplayPeriodDate.Year;
					var dateFromMonth = productDisplayPeriodDate.Month;
					var dateFromDay = productDisplayPeriodDate.Day;
					// 末日補正処理
					if (DateTimeUtility.IsLastDayOfMonth(dateFromYear, dateFromMonth, dateFromDay))
					{
						var lastDay = DateTimeUtility.GetLastDayOfMonth(dateFromYear, dateFromMonth).ToString();
						var productDisplayPeriodStart = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
							dateFromYear,
							dateFromMonth,
							lastDay,
							productDisplayPeriodDate.Hour,
							productDisplayPeriodDate.Minute,
							productDisplayPeriodDate.Second);
						productDisplayPeriodDate = DateTime.Parse(productDisplayPeriodStart);
					}

					htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD,
						StringUtility.ToValue(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD], productDisplayPeriodDate.ToString()));
					ucProductDisplayPeriod.SetStartDate(productDisplayPeriodDate);
				}
			}

			// 商品有効フラグ
			var strProductValidFlg = (string)Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_VALID_FLG];
			if (strProductValidFlg != null)
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_VALID_FLG, strProductValidFlg);
				cbProductValidFlg.Checked = (strProductValidFlg == FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED);
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_VALID_FLG, FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED);
				cbProductValidFlg.Checked = true;
			}

			// 入荷通知メール区分・再入荷通知
			string strSearchArrivalMail = (string)Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_ARRIVAL_MAIL];
			if (strSearchArrivalMail != null)
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_ARRIVAL_MAIL, strSearchArrivalMail);
				cbSearchArrivalMail.Checked = (strSearchArrivalMail == FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED);
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_ARRIVAL_MAIL, FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED);
				cbSearchArrivalMail.Checked = true;
			}
			// 入荷通知メール区分・販売開始通知
			string strSearchReleaseMail = (string)Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RELEASE_MAIL];
			if (strSearchReleaseMail != null)
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RELEASE_MAIL, strSearchReleaseMail);
				cbSearchReleaseMail.Checked = (strSearchReleaseMail == FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED);
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RELEASE_MAIL, FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED);
				cbSearchReleaseMail.Checked = true;
			}
			// 入荷通知メール区分・再販売通知
			string strSearchResaleMail = (string)Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RESALE_MAIL];
			if (strSearchResaleMail != null)
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RESALE_MAIL, strSearchResaleMail);
				cbSearchResaleMail.Checked = (strSearchResaleMail == FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED);
			}
			else
			{
				htResult.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RESALE_MAIL, FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED);
				cbSearchResaleMail.Checked = true;
			}

			// ソート
			switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_PRODUCT_ID_ASC:			// 商品ID/昇順
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_PRODUCT_ID_DESC:			// 商品ID/降順
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_ASC:	// 販売開始日/昇順
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DESC:	// 販売開始日/降順
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_COUNT_ASC:	// 再入荷通知件数/昇順
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_COUNT_DESC:	// 再入荷通知件数/降順
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_RELEASE_MAIL_COUNT_ASC:	// 販売開始通知件数/昇順
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_RELEASE_MAIL_COUNT_DESC: // 販売開始通知件数/降順
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_RESALE_MAIL_COUNT_ASC:	// 再販売通知件数/昇順
				case Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_RESALE_MAIL_COUNT_DESC:	// 再販売通知件数/降順
					htResult.Add(Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN]);
					break;
				default:
					htResult.Add(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_USERPRODUCTARRIVALMAIL_DEFAULT);
					break;
			}
			ddlSortKbn.SelectedValue = (string)htResult[Constants.REQUEST_KEY_SORT_KBN];

			// ページ番号（ページャ動作時のみもちまわる）
			int iPageNo;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out iPageNo) == false)
			{
				iPageNo = 1;
			}
			htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iPageNo.ToString());
			this.CurrentPageNo = iPageNo;
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return htResult;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="htParam">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable htParam)
	{
		Hashtable htResult = new Hashtable();

		// 店舗ID
		htResult.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID, this.LoginOperatorShopId);
		// 商品ID
		htResult.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_ID]));
		// 商品名
		htResult.Add(Constants.FIELD_PRODUCT_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_NAME]));
		// 在庫数
		string strStockCount = (string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_STOCK_COUNT];
		if (Validator.IsHalfwidthNumber(strStockCount))
		{
			htResult.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, int.Parse(strStockCount));
		}
		else
		{
			htResult.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, System.DBNull.Value);
		}
		// 販売開始日(From)
		var sellFromDateFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM]),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM]));
		if (Validator.IsDate(sellFromDateFrom))
		{
			htResult.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_from", DateTime.Parse(sellFromDateFrom));
		}
		else
		{
			htResult.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_from", System.DBNull.Value);
		}
		// 販売開始日(To)
		var sellFromDateTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO]),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO]));
		if (Validator.IsDate(sellFromDateTo))
		{
			htResult.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_to",
				DateTime.Parse(sellFromDateTo).AddSeconds(1)
					.ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT));
		}
		else
		{
			htResult.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_to", System.DBNull.Value);
		}
		// 販売終了日(From)
		var sellToDateFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM]),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM]));
		if (Validator.IsDate(sellToDateFrom))
		{
			htResult.Add(Constants.FIELD_PRODUCT_SELL_TO + "_from", DateTime.Parse(sellToDateFrom));
		}
		else
		{
			htResult.Add(Constants.FIELD_PRODUCT_SELL_TO + "_from", System.DBNull.Value);
		}
		// 販売終了日(To)
		var sellToDateTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO]),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO]));
		if (Validator.IsDate(sellToDateTo))
		{
			htResult.Add(Constants.FIELD_PRODUCT_SELL_TO + "_to",
				DateTime.Parse(sellToDateTo).AddSeconds(1)
					.ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT));
		}
		else
		{
			htResult.Add(Constants.FIELD_PRODUCT_SELL_TO + "_to", System.DBNull.Value);
		}
		// 販売期間
		var salesPeriod =
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD]);
		if (Validator.IsDate(salesPeriod))
		{
			htResult.Add("sales_period", salesPeriod);
		}
		else
		{
			htResult.Add("sales_period", System.DBNull.Value);
		}
		// 表示期間
		var displayPeriod =
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD]);
		if (Validator.IsDate(displayPeriod))
		{
			htResult.Add("display_period", displayPeriod);
		}
		else
		{
			htResult.Add("display_period", System.DBNull.Value);
		}
		// 商品有効フラグ
		htResult.Add(Constants.FIELD_PRODUCT_VALID_FLG, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_VALID_FLG]));
		// 入荷通知メール区分・再入荷通知
		htResult.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN + "_arrival", StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_ARRIVAL_MAIL]));
		// 入荷通知メール区分・販売開始通知
		htResult.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN + "_release", StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RELEASE_MAIL]));
		// 入荷通知メール区分・再販売通知
		htResult.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN + "_resale", StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RESALE_MAIL]));
		// ソート区分
		htResult.Add("sort_kbn", (string)htParam[Constants.REQUEST_KEY_SORT_KBN]);

		return htResult;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfoFromControl()
	{
		Hashtable htSearch = new Hashtable();
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_ID, tbProductId.Text.Trim());					// 商品ID
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_NAME, tbProductName.Text.Trim());				// 商品名
		htSearch.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);											// ソート
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_STOCK_COUNT, tbProductStock.Text.Trim());		// 在庫数
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM,
			ucProductSellFromDate.StartDateString);																			// Product Sell From Date From
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM,
			string.IsNullOrEmpty(ucProductSellFromDate.HfStartTime.Value)
				? null
				: ucProductSellFromDate.HfStartTime.Value);																	// Product Sell From Time From
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO,
			ucProductSellFromDate.EndDateString);																			// Product Sell From Date To
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO,
			string.IsNullOrEmpty(ucProductSellFromDate.HfEndTime.Value)
				? null
				: ucProductSellFromDate.HfEndTime.Value);																	// Product Sell From Time To
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM,
			string.IsNullOrEmpty(ucProductSellToDate.HfStartDate.Value)
				? null
				: ucProductSellToDate.HfStartDate.Value);																	// Product Sell From Date From
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM,
			string.IsNullOrEmpty(ucProductSellToDate.HfStartTime.Value)
				? null
				: ucProductSellToDate.HfStartTime.Value);																	// Product Sell From Time From
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO,
			string.IsNullOrEmpty(ucProductSellToDate.HfEndDate.Value)
				? null
				: ucProductSellToDate.HfEndDate.Value);																	// Product Sell From Date To
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO,
			string.IsNullOrEmpty(ucProductSellToDate.HfEndTime.Value)
				? null
				: ucProductSellToDate.HfEndTime.Value);																	// Product Sell From Time To
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD,
			string.IsNullOrEmpty(ucProductSalesPeriod.StartDateTimeString)
				? null
				: ucProductSalesPeriod.StartDateTimeString);																	// 販売期間
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD,
			string.IsNullOrEmpty(ucProductDisplayPeriod.StartDateTimeString)
				? null
				: ucProductDisplayPeriod.StartDateTimeString);																	// 表示期間
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_VALID_FLG, cbProductValidFlg.Checked ? FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED : null);		// 商品有効フラグ
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_ARRIVAL_MAIL, cbSearchArrivalMail.Checked ? FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED : "");	// 再入荷通知
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RELEASE_MAIL, cbSearchReleaseMail.Checked ? FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED : "");	// 販売開始通知
		htSearch.Add(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RESALE_MAIL, cbSearchResaleMail.Checked ? FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED : "");		// 再販売通知

		return htSearch;
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// 検索用パラメタ作成し、同じ画面にリダイレクト
		Response.Redirect(CreateUserProductArrivalMailListUrl(GetSearchInfoFromControl(), false));
	}

	/// <summary>
	/// 入荷通知メール一覧遷移URL作成
	/// </summary>
	/// <param name="htParam">検索情報</param>
	/// <param name="blSending">送信処理中</param>
	/// <returns>入荷通知メール一覧遷移URL</returns>
	private string CreateUserProductArrivalMailListUrl(Hashtable htParam, bool blSending)
	{
		StringBuilder sbReturnUrl = new StringBuilder();
		sbReturnUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USERPRODUCTARRIVALMAIL_LIST);
		if (htParam != null)
		{
			sbReturnUrl.Append("?").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_ID]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_NAME).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_NAME]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_SORT_KBN]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_STOCK_COUNT).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_STOCK_COUNT]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_VALID_FLG).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_VALID_FLG]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_ARRIVAL_MAIL).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_ARRIVAL_MAIL]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RELEASE_MAIL).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RELEASE_MAIL]));
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RESALE_MAIL).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RESALE_MAIL]));
		}
		if (blSending)
		{
			sbReturnUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_MAIL_SEND).Append("=").Append("1");
		}

		return sbReturnUrl.ToString();
	}

	/// <summary>
	/// メール配信ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSend_Click(object sender, EventArgs e)
	{
		List<Hashtable> lhtTargetProducts = new List<Hashtable>();
		StringBuilder sbErrorMessages = new StringBuilder();

		StringBuilder sbParameters = new StringBuilder();

		//------------------------------------------------------
		// 対象商品取得
		//------------------------------------------------------
		foreach (RepeaterItem riTarget in rList.Items)
		{
			if (((CheckBox)riTarget.FindControl("cbTarget")).Checked)
			{
				// 送信対象の商品情報を取得
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID, ((HiddenField)riTarget.FindControl("hfShopId")).Value);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID, ((HiddenField)riTarget.FindControl("hfProductId")).Value);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID, ((HiddenField)riTarget.FindControl("hfVariationId")).Value);
				lhtTargetProducts.Add(htInput);
			}
		}

		//------------------------------------------------------
		// 送信対象存在チェック
		//------------------------------------------------------
		// 送信対象商品が0件だったらエラー
		if (lhtTargetProducts.Count == 0)
		{
			sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERPRODUCTARRIVALMAIL_TARGET_PRODUCT_NO_SELECTED_ERROR)).Append("<br />");
		}

		// 送信対象の入荷通知メール区分が全て未選択だったらエラー
		if ((cbSendArrivalMail.Checked == false) && (cbSendReleaseMail.Checked == false) && (cbSendResaleMail.Checked == false))
		{
			sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERPRODUCTARRIVALMAIL_TARGET_ARRIVAL_MAIL_KBN_NO_SELECTED_ERROR));
		}

		// エラーメッセージがあればエラーページへ遷移
		if (sbErrorMessages.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// バッチに渡すパラメータ作成+送信ステータスを「処理中」にする
		//------------------------------------------------------
		string strSendArrivalMailFlg = FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_UNCHECKED;
		string strSendReleaseMailFlg = FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_UNCHECKED;
		string strSendResaleMailFlg = FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_UNCHECKED;

		// 送信対象の入荷通知メール区分を取得
		if (cbSendArrivalMail.Checked)
		{
			sbParameters.Append(" -,").Append(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL).Append(",").Append(ddlMailTemplateArrival.SelectedValue);
			strSendArrivalMailFlg = FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED;
		}
		if (cbSendReleaseMail.Checked)
		{
			sbParameters.Append(" -,").Append(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE).Append(",").Append(ddlMailTemplateRelease.SelectedValue);
			strSendReleaseMailFlg = FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED;
		}
		if (cbSendResaleMail.Checked)
		{
			sbParameters.Append(" -,").Append(Constants.FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE).Append(",").Append(ddlMailTemplateResale.SelectedValue);
			strSendResaleMailFlg = FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_CHECKED;
		}

		// メール送信方法：管理画面からメール送信する場合、マニュアル送信を指定する
		sbParameters.Append(" +,").Append(Constants.MailSendMethod.Manual);

		// 送信対象の商品を取得
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			foreach (var targetProduct in lhtTargetProducts)
			{
				sbParameters.Append(" ").Append(targetProduct[Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID]);
				sbParameters.Append(",").Append(targetProduct[Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID]);
				sbParameters.Append(",").Append(targetProduct[Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID]);
				sbParameters.Append(",").Append(strSendArrivalMailFlg);
				sbParameters.Append(",").Append(strSendReleaseMailFlg);
				sbParameters.Append(",").Append(strSendResaleMailFlg);
				sbParameters.Append(",").Append("\"" + this.LoginOperatorName+ "\"");

				// 送信ステータス更新
				using (var statement = new SqlStatement("UserProductArrivalMail", "UpdateMailSendStatus"))
				{
					targetProduct.Add("send_arrival_mail_flg", strSendArrivalMailFlg);
					targetProduct.Add("send_release_mail_flg", strSendReleaseMailFlg);
					targetProduct.Add("send_resale_mail_flg", strSendResaleMailFlg);
					targetProduct.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS, Constants.FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_SENDING);
					targetProduct.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_LAST_CHANGED, this.LoginOperatorName);

					statement.ExecStatement(accessor, targetProduct);
				}
			}
			accessor.CommitTransaction();
		}

		//------------------------------------------------------
		// 送信バッチ実行
		//------------------------------------------------------
		StockCommon stockCommon = new StockCommon();
		stockCommon.ExecuteArrivalMailSendExeProsess(Constants.PHYSICALDIRPATH_ARRIVALMAILSEND_EXE, sbParameters.ToString());

		//------------------------------------------------------
		// リダイレクト（処理中メッセージ表示）
		//------------------------------------------------------
		Response.Redirect(CreateUserProductArrivalMailListUrl(GetSearchInfoFromControl(), true));
	}

	/// <summary>
	/// 商品名+バリエーション名作成
	/// </summary>
	/// <param name="drvUserProductArrivalMail">入荷通知メール情報</param>
	/// <returns>商品名+バリエーション名</returns>
	protected string CreateProductJointName(object objUserProductArrivalMail)
	{
		return ProductCommon.CreateProductJointName((DataRowView)objUserProductArrivalMail);
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return GetSearchSqlInfo(GetParameters());
	}

	/// <summary>カレントページNO</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_PAGE_NO]; }
		set { ViewState[Constants.REQUEST_KEY_PAGE_NO] = value; }
	}
	/// <summary>初回検索しないか</summary>
	protected override bool IsNotSearchDefault
	{
		get
		{
			if (Request.QueryString.AllKeys.Any(x => (x != Constants.REQUEST_KEY_FIRSTVIEW))) return false;

			if (this.IsFirstView.HasValue) return this.IsFirstView.Value;

			return base.IsNotSearchDefault;
		}
	}
	/// <summary>初回検索指定されているか</summary>
	protected bool? IsFirstView
	{
		get
		{
			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_FIRSTVIEW]) == false)
			{
				var firstview = 0;
				if (int.TryParse(Request[Constants.REQUEST_KEY_FIRSTVIEW], out firstview))
				{
					return (firstview == 0);
				}
			}

			return null;
		}
	}
}